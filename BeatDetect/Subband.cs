using System;
using System.Collections.Generic;
using System.Text;

namespace BeatDetect
{
    public class Subband
    {
        // Fields
        private readonly float BAND_WIDTH;
        //private readonly float FREQUENCY_BANDWIDTH_MULTIPLIER;
        private int m_centerFrequencyHz;
        private int m_spectrumSize;
        private int m_width;


        // Methods
        public Subband(int centerFrequencyHz, int widthHz, int spectrumSize)
        {
            this.m_centerFrequencyHz = centerFrequencyHz;
            this.m_spectrumSize = spectrumSize;
            
            BAND_WIDTH = (44100 / 2 / m_spectrumSize);
            //FREQUENCY_BANDWIDTH_MULTIPLIER = (float)Math.Pow(2.0, 0.16666666666666666);

            this.m_width = (int)(widthHz / BAND_WIDTH);
        }

        public override string ToString()
        {
            return CenterFrequency;
        }

        // Properties
        public string CenterFrequency
        {
            get
            {
                return (m_centerFrequencyHz < 1000) ? m_centerFrequencyHz.ToString() :
                    string.Format("{0:F1}k", ((float)m_centerFrequencyHz) / 1000f);
            }
        }

        public int LowFrequency
        {
            //get { return (int)(((double)m_centerFrequency) / FREQUENCY_BANDWIDTH_MULTIPLIER); }
            get
            {
                return m_centerFrequencyHz - (int)(BAND_WIDTH * (m_width/2));
            }
        }

        public int HighFrequency
        {
            //get { return (int)(m_centerFrequency * FREQUENCY_BANDWIDTH_MULTIPLIER); }
            get
            {
                return m_centerFrequencyHz + (int)(BAND_WIDTH * (m_width / 2));
            }
        }




        public int HighFrequencyIndex
        {
            get
            {
                return Math.Min( (int)Math.Ceiling((double)(((float)this.HighFrequency) / BAND_WIDTH)), m_spectrumSize);
            }
        }

        public int LowFrequencyIndex
        {
            get
            {
                return (int) Math.Floor((double)(((float) this.LowFrequency) / BAND_WIDTH));
            }
        }

    }
 

}
