
using System;
using FMOD;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CombFilter
{
    public static class FMODExtensionMethods
    {
        public static int getNumChannels(this Sound sound)
        {
            SOUND_TYPE type = 0;
            SOUND_FORMAT format = 0;
            int channels = 0, bits = 0;
            sound.getFormat(ref type, ref format, ref channels, ref bits);
            return channels;
        }

        public static int getBits(this Sound sound)
        {
            SOUND_TYPE type = 0;
            SOUND_FORMAT format = 0;
            int channels = 0, bits = 0;
            sound.getFormat(ref type, ref format, ref channels, ref bits);
            return bits;
        }

        public static float getFrequency(this Sound sound)
        {
            float frequency = 0F, volume = 0F, pan = 0F;
            int priority = 0;
            sound.getDefaults(ref frequency, ref volume, ref pan, ref priority);
            return frequency;
        }

        public static uint getPCMSamples(this Sound sound)
        {
            uint totalSamples = 0;
            sound.getLength(ref totalSamples, TIMEUNIT.PCM);
            return totalSamples;
        }

        /*
        public static Int32[,] getRawData(this Sound sound)
        {
            int channels = sound.getNumChannels();
            int bits = sound.getBits();

            if (!(bits == 8 || bits == 16))
                throw new Exception("Can only deal with 8 or 16 bit PCM data");

            uint pcmSamples = 0, pcmBytes = 0;
            sound.getLength(ref pcmSamples, TIMEUNIT.PCM);
            sound.getLength(ref pcmBytes, TIMEUNIT.PCMBYTES);

            int bytesPerSample = (bits >> 3) * channels;
            Debug.Assert(pcmBytes == (bytesPerSample * pcmSamples));

            // Dimension 0: Channel
            // Dimension 1: Sample
            Int32[,] data = new Int32[channels, pcmSamples];

            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            uint len1 = 0, len2 = 0;
            byte[] buffer = new byte[pcmBytes];

            RESULT result = sound.@lock(0, pcmBytes, ref ptr1, ref ptr2, ref len1, ref len2);
            if (result != RESULT.OK)
                throw new Exception("FMOD Error:\n" + Error.String(result));

            Marshal.Copy(ptr1, buffer, 0, (int)pcmBytes);

            int src_i = 0;  // Index into the source buffer.
            for (int samp_i=0; samp_i<pcmSamples; samp_i++)
            {
                for (int dst_chan = 0; dst_chan < channels; dst_chan++)
                {
                    int mask = getMask(bits);
                    int temp = BitConverter.ToInt16(buffer, src_i) & mask;
                    data[dst_chan, samp_i] = temp;
                    src_i += (bits >> 3);               // Move to next channel's data 
                }
            }

            sound.unlock(ptr1, ptr2, len1, len2);
            return data;
        }
         */


        
        public static double[,] getRawDataByTime(this Sound sound, uint startTimeMsec, uint durationMsec)
        {
            float samplesPerMsec = sound.getFrequency() / 1000F;
            uint offset = (uint)Math.Floor(startTimeMsec * samplesPerMsec);
            uint samples = (uint)Math.Floor(durationMsec * samplesPerMsec);
            return getRawData(sound, offset, samples);

        }

        /// <summary>
        /// Gets a specified region of raw audio, from the PCM Data
        /// </summary>
        /// <returns>A 2D array of data, where each row is a channel,
        /// and each column is a sample.</returns>
        public static double[,] getRawData(this Sound sound)
        {
            return getRawData(sound, 0, sound.getPCMSamples());
        }

        
        /// <summary>
        /// Gets a specified region of raw audio, from the PCM Data
        /// </summary>
        /// <param name="offset">The first sample to get.</param>
        /// <param name="samples">The number of samples to get.</param>
        public static double[,] getRawData(this Sound sound, uint offset, uint samples)
        {
            // http://msdn.microsoft.com/en-us/library/ms707957(VS.85).aspx#pcmwaveformaudiodataformat
            int channels = sound.getNumChannels();
            int bits = sound.getBits();

            if (!(bits == 8 || bits == 16))
                throw new Exception("Can only deal with 8 or 16 bit PCM data");

            uint totalSamples = sound.getPCMSamples();

            if ((offset + samples) > totalSamples)
                throw new Exception("(offset + samples) must be less than the total number of samples");

            int bytesPerSample = (bits >> 3) * channels;

            // Dimension 0: Channel
            // Dimension 1: Sample
            double[,] data = new double[channels, samples];

            // Calculate the number of bytes to copy.
            uint bytes = (uint)(samples * bytesPerSample);

            // Get a pointer to the specified region of PCM data.
            IntPtr ptr1 = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;
            uint len1 = 0, len2 = 0;
            RESULT result = sound.@lock(offset, bytes, ref ptr1, ref ptr2, ref len1, ref len2);
            if (result != RESULT.OK)
                throw new Exception("FMOD Error:\n" + Error.String(result));

            // Copy the data to a managed byte array.
            byte[] buffer = new byte[bytes];
            Marshal.Copy(ptr1, buffer, 0, (int)bytes);

            // Copy the data from the byte array into the 2D channel/sample array.
            int src_i = 0;  // Index into the source buffer.
            for (int samp_i = 0; samp_i < samples; samp_i++)
            {
                for (int dst_chan = 0; dst_chan < channels; dst_chan++)
                {
                    double temp;
                    switch (bits)
                    {
                        case 8:
                            // Unsigned 8-bit PCM
                            temp = ((double)((int)buffer[src_i] - 128)) / (128.0F);
                            break;
                        case 16:
                            // Signed 16-bit PCM
                            temp = ((double)BitConverter.ToInt16(buffer, src_i)) / (32768.0F);
                            break;
                        default:
                            temp = 0;
                            break;
                    }
                    
                    data[dst_chan, samp_i] = temp;

                    // Move to next channel's data, X bytes away.
                    src_i += (bits >> 3);               
                }
            }

            // Unlock the locked region.
            sound.unlock(ptr1, ptr2, len1, len2);
            return data;
        }


        private static int getMask(int bits)
        {
            // build a mask equal to the number of bytes the PCM data is.
            int mask = 0;
            for (int b = 0; b < bits; b++)
            {
                mask |= (1 << b);
            }
            return mask;
        }

    }


}