using System;
using FMOD;

using MathNet;
using MathNet.Numerics;
using MathNet.Numerics.Transformations;


namespace CombFilter
{
    public class CombFilterBPMCalc
    {
        // Audio file
        private string m_audioFilename;
        private Sound m_sound;
        private double m_maxFreq;

        // Math.NET objects
        private RealFourierTransformation m_fft;

        // Parameters
        private double[] m_bandLimits;
        private int m_samplesToRead;
        private int m_maxCombPulses;
        private double m_hannWindowLength;

        private double m_startOffset;
        private double m_duration;





        public CombFilterBPMCalc(string audioFilename)
        {
            m_audioFilename = audioFilename;
            init();
            setDefaults();
        }


        private void setDefaults()
        {
            //m_bandLimits = new double[] { 0, 200, 400, 800, 1600, 3200, 4600 };
            m_bandLimits = new double[] { 0, 200, 400, 800, 1600, 3200, 4600 };
            m_samplesToRead = (int)Math.Floor(m_sound.getFrequency() * 2.2);    // 2.2 seconds
            m_maxCombPulses = 3;
            m_hannWindowLength = 0.1;

            m_startOffset = 0.0;
        }


        private void init()
        {
            // FFT
            m_fft = new RealFourierTransformation(TransformationConvention.Matlab);

            // Load sound object
            ERRCHECK(fmod.GetInstance(-1).SystemObject.createSound(
                m_audioFilename, (MODE._2D | MODE.HARDWARE | MODE.CREATESAMPLE), ref m_sound));

            m_maxFreq = m_sound.getFrequency() / 2;
        }



        private int NumBands { get { return m_bandLimits.Length; } }
        private double MaxFreq { get { return m_maxFreq; } }

        public double[] BandLimits
        {
            get { return m_bandLimits; }
            set { m_bandLimits = value; }
        }

        public double HannWindowLength
        {
            get { return m_hannWindowLength; }
            set { m_hannWindowLength = value; }
        }

        public double StartOffset
        {
            get { return m_startOffset; }
            set { m_startOffset = value; }
        }

        public double Duration
        {
            get { return m_duration; }
            set { m_duration = value; }
        }




        public double EstimateBPM()
        {
            DateTime ts;

            // Calculate the sample range;
            uint offset = (uint)Math.Floor(m_startOffset * m_sound.getFrequency());
            uint samples = (uint)Math.Floor(m_duration * m_sound.getFrequency());

            // Read the PCM Data into a scaled double multichannel array
            Console.Write("Reading PCM Data...");
            ts = DateTime.Now;
            double[,] multiChannelAudioData = m_sound.getRawData(offset, samples);
            Console.WriteLine("done ({0}ms)", (DateTime.Now - ts).TotalMilliseconds);

            // Adjust the length of the array to be a power of two (for the FFT)
            double[,] paddedMultiChannelAudioData = ZeroPadToLengthPowerOf2(multiChannelAudioData);

            // Bring each channel into the frequency domain.
            Complex[,] muliChannelFFTComplex;
            Console.Write("Performing FFT on input data...");
            ts = DateTime.Now;
            m_fft.TransformForward(paddedMultiChannelAudioData, out muliChannelFFTComplex);
            Console.WriteLine("done ({0}ms)", (DateTime.Now - ts).TotalMilliseconds);

            // Average the freqency content to make a mono track.
            Console.Write("Averaging channels into mono channel...");
            ts = DateTime.Now;
            Complex[] fftComplex = new Complex[muliChannelFFTComplex.NumCols()];
            int ROWS = muliChannelFFTComplex.NumRows();
            for (int col = 0; col < fftComplex.Length; col++)
            {
                Complex sum = new Complex(0, 0);
                for (int row = 0; row < ROWS; row++)
                {
                    sum += muliChannelFFTComplex[row, col];
                }
                fftComplex[col] = sum / ROWS;
            }
            Console.WriteLine("done ({0}ms)", (DateTime.Now - ts).TotalMilliseconds);


            // Run the FFT data through a filterband to separate it into NumBands different bands.
            Console.Write("Separating audio through filterbank...");
            ts = DateTime.Now;
            Complex[,] filtered = FilterBank(fftComplex);
            Console.WriteLine("done ({0}ms)", (DateTime.Now - ts).TotalMilliseconds);


            // Bring the filtered data back into the time domain
            double[,] filteredTDAudio;
            m_fft.TransformBackward(filtered, out filteredTDAudio);

            // Full-wave rectification
            double[,] fwrAudio = FullWaveRectify(filteredTDAudio);

            // Hanning window
            Console.Write("Applying Hanning window of {0} sec...", m_hannWindowLength);
            ts = DateTime.Now;
            double[,] hwndAudio = ApplyHanningWindow(fwrAudio, m_hannWindowLength);
            Console.WriteLine("done ({0}ms)", (DateTime.Now - ts).TotalMilliseconds);

            // Diff-Rect
            Console.Write("Performing diff-rect...");
            ts = DateTime.Now;
            double[,] diffAudio = Differentiate(hwndAudio);
            double[,] hwrectAudio = HalfWaveRectify(diffAudio);
            Console.WriteLine("done ({0}ms)", (DateTime.Now - ts).TotalMilliseconds);



            // Comb filter
            double bestBPM = 0.0;

            Console.WriteLine("\n    Best Guess: {0} BPM    Focusing with resolution: {1} BPM/step", bestBPM, 10.0);
            bestBPM = CombBPMRange(hwrectAudio, 60.0, 240.0, 10);

            Console.WriteLine("\n    Best Guess: {0} BPM    Focusing with resolution: {1} BPM/step", bestBPM, 1.0);
            bestBPM = CombBPMRange(hwrectAudio, bestBPM-9.0, bestBPM+9.0, 1);

            Console.WriteLine("\n    Best Guess: {0} BPM    Focusing with resolution: {1} BPM/step", bestBPM, 0.1);
            bestBPM = CombBPMRange(hwrectAudio, bestBPM-0.9, bestBPM+0.9, 0.1);

            //Console.WriteLine("\n    Best Guess: {0} BPM    Focusing with resolution: {1} BPM/step", bestBPM, 0.01);
            //bestBPM = CombBPMRange(hwrectAudio, bestBPM-0.09, bestBPM+0.09, 0.01);


            return bestBPM;
        }
















        private static double[,] ZeroPadToLengthPowerOf2(double[,] array)
        {
            int powerOfTwo = MathNet.Numerics.Fn.CeilingToPowerOf2(array.NumCols());
            double[,] output = new double[array.NumRows(), powerOfTwo];
            for (int row = 0; row < array.NumRows(); row++)
            {
                output.SetRow(row, ZeroPadToLengthPowerOf2(array.GetRow(row)));
            }
            return output;
        }

        private static double[] ZeroPadToLengthPowerOf2(double[] array)
        {
            int powerOfTwo = MathNet.Numerics.Fn.CeilingToPowerOf2(array.Length);
            double[] output = new double[powerOfTwo];
            for (int i = 0; i < powerOfTwo; i++)
            {
                output[i] = (i < array.Length) ? array[i] : 0.0F;
            }
            return output;
        }



        private Complex[,] FilterBank(Complex[] fftComplex)
        {
            int n = fftComplex.Length;
            Complex[,] filtered = new Complex[NumBands, n];

            for (int band = 0; band < NumBands; band++)
            {
                int leftindex, rightindex;
                double leftDesired, rightDesired;

                // Calculate this frequency's index into the FFT data.
                leftDesired = m_bandLimits[band];
                leftindex = (int)Math.Floor((leftDesired / MaxFreq) * (n / 2));
                if (band < (NumBands - 1))
                {
                    rightDesired = m_bandLimits[band + 1];
                    rightindex = (int)Math.Floor((rightDesired / MaxFreq) * (n / 2));
                }
                else
                {
                    rightDesired = MaxFreq;
                    rightindex = (int)Math.Floor((double)n / 2);

                }

                for (int i = leftindex; i <= rightindex; i++)
                {
                    filtered[band, i] = fftComplex[i];          // Left side
                    filtered[band, n - i - 1] = fftComplex[n - i - 1];  // Right side
                }

            }

            return filtered;
        }





        private static double[,] FullWaveRectify(double[,] signal)
        {
            double[,] result = new double[signal.NumRows(), signal.NumCols()];
            for (int row = 0; row < signal.NumRows(); row++)
            {
                for (int col = 0; col < signal.NumCols(); col++)
                {
                    result[row, col] = Math.Abs(signal[row, col]);
                }
            }
            return result;
        }




        private double[,] ApplyHanningWindow(double[,] signal, double WindowLength)
        {
          //MATLABFile matlab = new MATLABFile(MATLAB_PATH + "IMPORT_hannwindow.m");
            Complex[,] fftComplex;
            m_fft.TransformForward(signal, out fftComplex);

            // Build the half-Hanning window
            int n = signal.NumCols();
            int hannlen = (int)(WindowLength * 2 * MaxFreq);
            double[] hannWindow = new double[n];
            for (int a = 0; a < hannlen; a++)
            {
                //hann(a) = (cos(a*pi/hannlen/2)).^2;
                hannWindow[a] = Math.Pow((Math.Cos(a*Math.PI/hannlen/2)), 2);
            }
          //matlab.WriteColumnVector("CS_hannWindow", hannWindow);

            Complex[] hannFFT;
            m_fft.TransformForward(hannWindow, out hannFFT);
          //matlab.WriteColumnVector("CS_hannFFT", hannFFT);


            // Convolving with half-Hanning same as multiplying in
            // frequency. Multiply half-Hanning FFT by signal FFT.
            for (int band = 0; band < signal.NumRows(); band++)
            {
                for (int i = 0; i < signal.NumCols(); i++)
                {
                    fftComplex[band, i] *= hannFFT[i];
                }
            }

            // Inverse transform to get output in the time domain.
            double[,] output;
            m_fft.TransformBackward(fftComplex, out output);

            return output;
        }



        private static double[,] Differentiate(double[,] signal)
        {
            double[,] result = new double[signal.NumRows(), signal.NumCols()];

            for (int row = 0; row < signal.NumRows(); row++)
            {
                result[row, 0] = 0.0F;
                for (int i = 1; i < signal.NumCols(); i++)
                {
                    result[row, i] = signal[row, i] - signal[row, i-1];
                }
            }
            return result;
        }

        private static double[] Differentiate(double[] signal)
        {
            double[] result = new double[signal.Length];
            result[0] = 0.0F;
            for (int i = 1; i < signal.Length; i++)
            {
                result[i] = signal[i] - signal[i-1];
            }
            return result;
        }





        private static double[,] HalfWaveRectify(double[,] signal)
        {
            double[,] result = new double[signal.NumRows(), signal.NumCols()];
            for (int row = 0; row < signal.NumRows(); row++)
            {
                for (int i = 0; i < signal.NumCols(); i++)
                {
                    result[row, i] = (signal[row, i] > 0.0) ? signal[row, i] : 0.0F;
                }
            }
            return result;
        }


        private static double[] HalfWaveRectify(double[] signal)
        {
            double[] result = new double[signal.Length];
            for (int i = 0; i < signal.Length; i++)
            {
                result[i] = (signal[i] > 0.0) ? signal[i] : 0.0F;
            }
            return result;
        }



        private double CombBPMRange(double[,] signal, double MinBPM, double MaxBPM, double BPMresolution)
        {
            int BPMTries = (int)Math.Ceiling((((double)MaxBPM - (double)MinBPM) / (double)BPMresolution)) + 1;
            double[] energy = new double[BPMTries];
            double[] BPMvals = new double[BPMTries];
            int i = 0;
            double maxEnergy=0.0, bestBPM=0.0;
            for (double BPM=MinBPM; BPM<=MaxBPM; BPM+=BPMresolution)
            {
                Console.Write("Combing with BPM={0}...  ", BPM);
                BPMvals[i] = BPM;
                energy[i] = CombFilterEnergies(signal, BPM).Sum();
                if (energy[i] > maxEnergy)
                {
                    maxEnergy = energy[i];
                    bestBPM = BPM;
                    Console.Write("*");
                }
                Console.WriteLine();
                i++;
            }
            return bestBPM;
        }




        private double[] CombFilterEnergies(double[,] signal, double BPM)
        {
            int n = signal.NumCols();
            int nbands = signal.NumRows();
            int nstep = (int)Math.Floor(120F / BPM * MaxFreq);

            // Convert the signal to the frequency domain
            Complex[,] signalFFT;
            m_fft.TransformForward(signal, out signalFFT);

            // Build the comb filter in the time domain
            double[] timecomb = new double[n];
            for (int a = 0; (a < m_maxCombPulses) && (a*nstep < timecomb.Length); a++)
            {
                timecomb[a * nstep] = 1.0F;
            }

            // Bring filter into frequency domain.
            Complex[] filterFFT;
            m_fft.TransformForward(timecomb, out filterFFT);

            // Convolve each band of the signal with filter by multiplying in frequency domain.
            Complex[,] conv = new Complex[nbands, n];
            for (int band = 0; band < nbands; band++)
            {
                for (int i = 0; i < n; i++)
                {
                    conv[band, i] = signalFFT[band, i] * filterFFT[i];
                }
            }

            // Calculate the energy in each band of the convolved signal.
            // Due to Parseval's theorem, the energy contained in the frequency
            // domain is equal to that in the time domain.
            double[] energies = new double[nbands];
            for (int band = 0; band < nbands; band++)
            {
                energies[band] = (conv.GetRow(band)).GetModulusSquared().Sum();
            }

            return energies;
        }


















        private void ERRCHECK(RESULT result)
        {
            if (result != RESULT.OK)
            {
                throw new Exception(String.Format("FMOD Error: {0}\n{1}", result, Error.String(result)));;
            }
        }



    }
}
