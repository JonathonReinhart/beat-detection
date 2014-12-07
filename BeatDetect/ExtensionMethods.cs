using System;

namespace BeatDetect
{

    public static class ExtensionMedthods
    {

        #region Int Array

        public static int Sum(this int[] array)
        {
            int sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }

        public static float Average(this int[] array)
        {
            return ((float)array.Sum() / (float)array.Length);
        }

        #endregion


        #region float Array

        public static float Sum(this float[] array)
        {
            float sum = 0.0F;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }

        public static float Average(this float[] array)
        {
            return (array.Sum() / array.Length);
        }

        /// <summary>
        /// Shifts each value in the array numCells elements.
        /// Positive values shift cells right (from low index to high).
        /// </summary>
        /// <param name="array"></param>
        /// <param name="cells"></param>
        public static void Shift(this float[] array, int shiftAmount)
        {
            if (Math.Abs(shiftAmount) > array.Length)
                throw new ArgumentException("Shift amount must be less than or equal to the length of the array.", "shiftAmount");

            if (shiftAmount > 0)
            {
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    // Shift each element to the right.
                    if (i >= shiftAmount)
                        array[i] = array[i - shiftAmount];
                    
                    // Fill in remaining values with zeros
                    else
                        array[i] = 0.0F;
                }
            }
            if (shiftAmount < 0)
            {
                shiftAmount = -shiftAmount;

                for(int i=0; i<array.Length; i++)
                {
                    // Shift each element to the left.
                    if (i < array.Length-shiftAmount)
                        array[i] = array[i+shiftAmount];
                    
                    // Fill in remaining values with zeros
                    else
                        array[i] = 0.0F;
                }
            }
        }

        #endregion



        #region float 2D Array

        public static float[] Sum(this float[,] array)
        {
            int ROWS = array.GetUpperBound(0);
            int COLS = array.GetUpperBound(1);
            float[] sums = new float[ROWS];
            for (int row = 0; row < ROWS; row++)
            {
                sums[row] = 0.0F;
                for (int col = 0; col < COLS; col++)
                {
                    sums[row] += array[row, col];
                }
            }
            return sums;
        }

        public static float[] Average(this float[,] array)
        {
            int ROWS = array.GetUpperBound(0);
            int COLS = array.GetUpperBound(1);
            float[] averages = new float[ROWS];
            float[] sums = array.Sum();
            for (int row = 0; row < ROWS; row++)
                averages[row] = sums[row] / COLS;
            return averages;
        }

        #endregion


    }
}