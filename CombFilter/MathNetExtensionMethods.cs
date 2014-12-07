using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Transformations;


namespace CombFilter
{
    public static class MathNetExtensionMethods
    {

        public static void TransformForward(this RealFourierTransformation transformer,
            double[] samples, out Complex[] fftComplex)
        {
            double[] fftReal;
            double[] fftImag;
            transformer.TransformForward(samples, out fftReal, out fftImag);
            fftComplex = new Complex[fftReal.Length];

            for (int i = 0; i < fftComplex.Length; i++)
            {
                fftComplex[i].Real = fftReal[i];
                fftComplex[i].Imag = fftImag[i];
            }
        }


        public static void TransformForward(this RealFourierTransformation transformer,
            double[,] samples, out double[,] fftReal, out double[,] fftImag)
        {
            int ROWS = samples.NumRows();
            int COLS = samples.NumCols();

            fftReal = new double[ROWS, COLS];
            fftImag = new double[ROWS, COLS];

            double[] tempReal, tempImag;
            for (int row = 0; row < ROWS; row++)
            {
                transformer.TransformForward(samples.GetRow(row), out tempReal, out tempImag);
                fftReal.SetRow(row, tempReal);
                fftImag.SetRow(row, tempImag);
            }
        }

        public static void TransformForward(this RealFourierTransformation transformer,
            double[,] samples, out Complex[,] fftComplex)
        {
            double[,] fftReal, fftImag;
            transformer.TransformForward(samples, out fftReal, out fftImag);
            fftComplex = new Complex[fftReal.NumRows(), fftReal.NumCols()];
            for (int row = 0; row < fftComplex.NumRows(); row++)
            {
                for (int col = 0; col < fftComplex.NumCols(); col++)
                {
                    fftComplex[row, col].Real = fftReal[row, col];
                    fftComplex[row, col].Imag = fftImag[row, col];
                }
            }
        }


        public static void TransformBackward(this RealFourierTransformation transformer,
            Complex[] fftComplex, out double[] samples)
        {
            transformer.TransformBackward(fftComplex.GetReal(), fftComplex.GetImag(), out samples);
        }


        public static void TransformBackward(this RealFourierTransformation transformer,
            Complex[,] fftComplex, out double[,] samples)
        {
            samples = new double[fftComplex.NumRows(), fftComplex.NumCols()];
            for (int row = 0; row < fftComplex.NumRows(); row++)
            {
                double[] temp;
                transformer.TransformBackward(fftComplex.GetRow(row), out temp);
                samples.SetRow(row, temp);
            }
        }








        public static double[] GetReal(this Complex[] complex)
        {
            double[] real = new double[complex.Length];
            for (int i=0; i<complex.Length; i++)
                real[i] = complex[i].Real;
            return real;
        }

        public static double[] GetImag(this Complex[] complex)
        {
            double[] imag = new double[complex.Length];
            for (int i=0; i<complex.Length; i++)
                imag[i] = complex[i].Imag;
            return imag;
        }

        
        // Also referred to as Magnitude
        public static double[] GetModulus(this Complex[] complex)
        {
            double[] mod = new double[complex.Length];
            for (int i = 0; i < complex.Length; i++)
                mod[i] = complex[i].Modulus;
            return mod;
        }

        public static double[] GetModulusSquared(this Complex[] complex)
        {
            double[] mod = new double[complex.Length];
            for (int i = 0; i < complex.Length; i++)
                mod[i] = complex[i].ModulusSquared;
            return mod;
        }



        // Also referred to as Phase
        public static double[] GetArgument(this Complex[] complex)
        {
            double[] argument = new double[complex.Length];
            for (int i = 0; i < complex.Length; i++)
                argument[i] = complex[i].Argument;
            return argument;
        }










    }
}
