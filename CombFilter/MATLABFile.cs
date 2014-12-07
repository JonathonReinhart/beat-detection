using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MathNet.Numerics;

namespace CombFilter
{
    public class MATLABFile : IDisposable
    {
        private StreamWriter m_writer;

        public MATLABFile(string Filename)
        {
            m_writer = new StreamWriter(Filename, false);
        }



        public void WriteMatrix(string varname, double[,] array)
        {
            WriteMatrix(varname, array, false);
        }
        public void WriteMatrix(string varname, double[,] array, bool transpose)
        {
            m_writer.WriteLine("{0} = [", varname);
            int COLS = (transpose) ? array.NumRows() : array.NumCols();
            int ROWS = (transpose) ? array.NumCols() : array.NumRows();

            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    m_writer.Write("{0:0.000000000000000} ", (transpose ? array[col, row] : array[row, col]));
                }
                m_writer.WriteLine();
            }
            m_writer.WriteLine("];\n");
            m_writer.Flush();
        }



        public void WriteRowVector(string varname, double[] array)
        {
            WriteVector(varname, array, VectorType.Row);
        }

        public void WriteColumnVector(string varname, double[] array)
        {
            WriteVector(varname, array, VectorType.Column);
        }

        private enum VectorType { Row, Column };

        private void WriteVector(string varname, double[] array, VectorType type)
        {
            m_writer.Write("{0} = [", varname);
            for (int i = 0; i < array.Length; i++)
            {
                m_writer.Write("{0:0.000000000000000}", array[i]);
                if (i != array.Length - 1)  // If not on the last element
                {
                    if (type == VectorType.Column)
                        m_writer.Write(";");
                    m_writer.Write(" ");
                }
            }
            m_writer.WriteLine("];\n");
            m_writer.Flush();
        }





        public void WriteRowVector(string varname, Complex[] array)
        {
            WriteVector(varname, array, VectorType.Row);
        }

        public void WriteColumnVector(string varname, Complex[] array)
        {
            WriteVector(varname, array, VectorType.Column);
        }

        private void WriteVector(string varname, Complex[] array, VectorType type)
        {
            m_writer.Write("{0} = [", varname);
            for (int i = 0; i < array.Length; i++)
            {
                m_writer.Write("{0:0.###############}+{1:0.###############}i", array[i].Real, array[i].Imag);
                if (i != array.Length - 1)  // If not on the last element
                {
                    if (type == VectorType.Column)
                        m_writer.Write(";");
                    m_writer.Write(" ");
                }
            }
            m_writer.WriteLine("];\n");
            m_writer.Flush();
        }





        public void WriteComment(string comment)
        {
            m_writer.WriteLine("%{0}\n", comment);
            m_writer.Flush();
        }

        public void WriteString(string s)
        {
            m_writer.WriteLine("{0}\n", s);
            m_writer.Flush();
        }

        #region IDisposable Members

        public void Dispose()
        {
            m_writer.Close();
            m_writer.Dispose();
        }

        #endregion
    }
}
