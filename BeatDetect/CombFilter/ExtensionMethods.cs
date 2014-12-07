using System;
using MathNet.Numerics;
using MathNet.Numerics.Transformations;

namespace CombFilter
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

        #region Int 2D Array


        /// <summary>
        /// Returns an array of the averages of each column of the 2D array. 
        /// </summary>
        public static int[] ColumnAverages(this int[,] array)
        {
            int ROWS = array.GetLength(0);
            int COLS = array.GetLength(1);
            int[] averages = new int[COLS];
            for (int col = 0; col < COLS; col++)
            {
                // Sum this column.
                int column_sum = 0;
                for (int row = 0; row < ROWS; row++)
                    column_sum += array[row, col];
                averages[col] = (int)((double)column_sum / (double)ROWS);
            }

            return averages;
        }

        #endregion



        public static double Sum(this double[] array)
        {
            double sum = 0.0F;
            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }






        


        #region 2D Array Row/Column Get/Set functions

        public static int NumRows(this Array ar)
        {
            CheckRank2(ar, 2);
            return ar.GetLength(0);
        }

        public static int NumCols(this Array ar)
        {
            CheckRank2(ar, 2);
            return ar.GetLength(1);
        }

        public static double[] GetRow(this double[,] array2d, int row)
        {
            CheckRank2(array2d, 2);
            if (row >= array2d.NumRows())
                throw new ArgumentOutOfRangeException("row", row, "row must be less than the number of rows in the array.");

            double[] result = new double[array2d.NumCols()];
            for (int j = 0; j < array2d.NumCols(); j++)
                result[j] = array2d[row, j];

            return result;
        }

        public static double[] GetColumn(this double[,] array2d, int col)
        {
            CheckRank2(array2d, 2);
            if (col >= array2d.NumCols())
                throw new ArgumentOutOfRangeException("col", col, "col must be less than the number of columns in the array.");

            double[] result = new double[array2d.NumRows()];
            for (int j = 0; j < array2d.NumRows(); j++)
                result[j] = array2d[j, col];

            return result;
        }


        public static void SetRow(this double[,] array2d, int row, double[] array)
        {
            CheckRank2(array2d, 2);
            if (row >= array2d.NumRows())
                throw new ArgumentOutOfRangeException("row", row, "row must be less than the number of rows in the array.");
            if (array.Length != array2d.NumCols())
                throw new Exception("Provided array length must agree with number of columns in 2D array.");

            for (int i = 0; i < array2d.NumCols(); i++)
            {
                array2d[row, i] = array[i];
            }
        }

        public static void SetColumn(this double[,] array2d, int col, double[] array)
        {
            CheckRank2(array2d, 2);
            if (col >= array2d.NumCols())
                throw new ArgumentOutOfRangeException("col", col, "col must be less than the number of columns in the array.");
            if (array.Length != array2d.NumRows())
                throw new Exception("Provided array length must agree with number of rows in 2D array.");

            for (int i = 0; i < array2d.NumRows(); i++)
            {
                array2d[i, col] = array[i];
            }
        }

        private static void CheckRank2(Array array, int requiredRank)
        {
            if (array.Rank != requiredRank)
                throw new Exception(String.Format("Array with rank {0} required.", requiredRank));
        }

        #endregion







        #region Complex

        public static Complex[] GetRow(this Complex[,] array2d, int row)
        {
            CheckRank2(array2d, 2);
            if (row >= array2d.NumRows())
                throw new ArgumentOutOfRangeException("row", row, "row must be less than the number of rows in the array.");

            Complex[] result = new Complex[array2d.NumCols()];
            for (int j = 0; j < array2d.NumCols(); j++)
                result[j] = array2d[row, j];

            return result;
        }



        #endregion


    }
}
