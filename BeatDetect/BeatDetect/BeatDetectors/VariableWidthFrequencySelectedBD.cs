using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Vixen;
using FMOD;

namespace BeatDetect
{
    class VariableWidthFrequencySelectedBD : IBeatDetector
    {
        private _System m_system;
        private EventSequence m_sequence;

        private const int NUM_VALUES = 1024;
        private int m_historyLength;
        private int m_sensitivityConstant;
        private int m_subbandThreshold;
        private int m_beatTrackChannel;

        private Sound m_sound;
        private FMOD.Channel m_channel;

        private float[] m_instantSubbandEnergy;
        private float[][] m_subbandEnergyHistory;

        private static bool ERRCHECK(RESULT result)
        {
            if (result != RESULT.OK)
            {
                String message = String.Format("FMOD Error: {0}\n{1}", result, Error.String(result));
                MessageBox.Show(message, "Beat Detect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /*
        private int[] subbandWidths = {
            2,2,3,3,3,3,3,3,3,3,
            3,3,3,3,4,4,4,4,4,4,
            4,4,4,4,4,4,4,5,5,5,
            5,5,5,5,5,5,5,5,5,5,
            6,6,6,6,6,6,6,6,6,6,
            6,6,7,7,7,7,7,7,7,7,
            7,7,7,7,7,8,8,8,8,8,
            8,8,8,8,8,8,8,8,9,9,
            9,9,9,9,9,9,9,9,9,9,
            10,10,10,10,10,10,10,10,10,10,
            10,10,10,11,11,11,11,11,11,11,
            11,11,11,11,11,11,12,12,12,12,
            12,12,12,12,12,12,12,12
        };
         */

        private int[] subbandWidths = {
            32,32,32,32,32,32,32,32,32,32,
            32,32,32,32,32,32,32,32,32,32,
            32,32,32,32,32,32,32,32,32,32,
            32,32
        };

        private int NumSubbands { get { return subbandWidths.Length; } }

        private float subbandSensitivity(int subband)
        {
            //return 2.0F / (float)subbandWidths[subband];
            return 1.0F;
        }


        public VariableWidthFrequencySelectedBD(
            EventSequence sequence,
            int historyLength,
            int sensitivityConstant,
            int subbandThreshold,
            int beatTrackChannel)
        {
            m_sequence = sequence;
            m_historyLength = historyLength;
            m_sensitivityConstant = sensitivityConstant;
            m_subbandThreshold = subbandThreshold;
            m_beatTrackChannel = beatTrackChannel;

            if (subbandWidths.Sum() > NUM_VALUES)
                throw new Exception("Subwand widths sum exceeds number of FFT values");
        }


        private bool init()
        {
            // Create singleton System object.
            if (!ERRCHECK(Factory.System_Create(ref m_system)))
                return false;

            // Setup output to no sound.
            if (!ERRCHECK(m_system.setOutput(OUTPUTTYPE.NOSOUND_NRT)))
                return false;

            // Initialize system object, indicating that stream changes come from calls to update.s
            if (!ERRCHECK(m_system.init(0x20, INITFLAG.STREAM_FROM_UPDATE, IntPtr.Zero)))
                return false;

            // Load the audio stream.
            string filename = Path.Combine(Paths.AudioPath, m_sequence.Audio.FileName);
            if (!ERRCHECK(m_system.createStream(filename, MODE._2D | MODE.SOFTWARE, ref m_sound)))
                return false;

            m_instantSubbandEnergy = new float[NumSubbands];
            
            //float[,] subbandEnergyHistory = new float[subbandWidths.Length, historyLength];
            m_subbandEnergyHistory = new float[NumSubbands][];
            for (int subband = 0; subband < NumSubbands; subband++)
            {
                m_subbandEnergyHistory[subband] = new float[m_historyLength];
            }

            return true;
        }

        public bool Execute()
        {
            uint length = 0;
            uint position = 0;

            if (!init())
                return false;

            // Get the length of the audio stream
            m_sound.getLength(ref length, TIMEUNIT.MS);

            // Get information about the system software settings.
            int sampleRate = 0;
            SOUND_FORMAT format = SOUND_FORMAT.NONE;
            int numOutputChannels = 0;
            int maxInputChannels = 0;
            DSP_RESAMPLER method = DSP_RESAMPLER.NOINTERP;
            int bits = 0;
            m_system.getSoftwareFormat(ref sampleRate, ref format, ref numOutputChannels, ref maxInputChannels, ref method, ref bits);

            // Begin playing the stream
            m_system.playSound(CHANNELINDEX.FREE, m_sound, false, ref m_channel);

            // Show the progress dialog.
            ProgressDialog progdialog = new ProgressDialog(length);
            progdialog.Show();


            while (position < length)
            { 
                // Get the position in msec
                m_channel.getPosition(ref position, TIMEUNIT.MS);     

                // Update the progress dialog.
                progdialog.CurrentTime = position;
                
                // Calculate the index into the vixen sequence
                int eventNum = (int)(((long)position) / ((long)m_sequence.EventPeriod));
                if (eventNum >= m_sequence.TotalEventPeriods)
                    break;

                // get the spectrum data for all channels.
                float[][] chanSpectrums = new float[numOutputChannels][];
                for(int chan=0; chan < numOutputChannels; chan++)
                {
                    chanSpectrums[chan] = new float[NUM_VALUES];
                    m_system.getSpectrum(chanSpectrums[chan], NUM_VALUES, chan, DSP_FFT_WINDOW.BLACKMAN);
                }

                // average the channel spectrum data to get a single array
                float[] avgSpectrum = new float[NUM_VALUES];
                for (int i = 0; i < NUM_VALUES; i++)
                {
                    avgSpectrum[i] = 0F;
                    for (int chan = 0; chan < numOutputChannels; chan++)
                        avgSpectrum[i] += chanSpectrums[chan][i];
                    avgSpectrum[i] /= (float)numOutputChannels;
                }

                // square the spectrum data to get the energy
                for (int i = 0; i < NUM_VALUES; i++)
                {
                    avgSpectrum[i] *= avgSpectrum[i];
                }


                // Divide the entire energy spectrum into subbands by averaging
                // the spectral values in that subband.
                int count=0;
                for (int subband = 0; subband < subbandWidths.Length; subband++)
                {
                    float sum = 0.0F;
                    for (int i=0; i<subbandWidths[subband]; i++, count++)
                        sum += avgSpectrum[count];
                    m_instantSubbandEnergy[subband] = sum / subbandWidths[subband];
                }


                // Compute the average energy of each subband's history
                float[] subbandEnergyHistoryAverage = new float[subbandWidths.Length];
                for (int subband = 0; subband < subbandWidths.Length; subband++)
                {
                    float sum = 0.0F;
                    for (int i = 0; i < m_historyLength; i++)
                        sum += m_subbandEnergyHistory[subband][i];
                    subbandEnergyHistoryAverage[subband] = sum / m_historyLength;
                }


                for (int subband = 0; subband < subbandWidths.Length; subband++)
                {
                    // Shift the subband history to the right, making room for the new
                    // subband energy value, removing the oldest.
                    for (int i = m_historyLength - 1; i > 0; i--)
                        m_subbandEnergyHistory[subband][i] = m_subbandEnergyHistory[subband][i - i];

                    // Save the instantaneous energy values into the 0th index
                    m_subbandEnergyHistory[subband][0] = m_instantSubbandEnergy[subband];
                }

                // Determine if we have a beat on each subband
                int beatDeterm = 0;   // the number of subbands on which we determined there is a beat
                for (int subband = 0; subband < subbandWidths.Length; subband++)
                {
                    if (m_instantSubbandEnergy[subband] > m_sensitivityConstant * subbandEnergyHistoryAverage[subband] * subbandSensitivity(subband))
                    {
                        beatDeterm++;
                        //if (subband <= m_sequence.ChannelCount)
                        //    m_sequence.EventValues[subband, eventNum] = m_sequence.MaximumLevel;
                    }
                }

                // Insert the beat into the beattrack if there was a beat determined.
                if (beatDeterm >= m_subbandThreshold)
                {
                    m_sequence.EventValues[m_beatTrackChannel, eventNum] = m_sequence.MaximumLevel;
                }

                m_system.update();
            }
            m_channel.stop();

            progdialog.Hide();
            progdialog.Dispose();

            return true;
        }

    }
}
