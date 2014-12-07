using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BeatDetect
{
    public partial class ProgressDialog : Form
    {
        private uint m_totalMsec;

        public ProgressDialog(uint totalMsec)
        {
            InitializeComponent();
            m_totalMsec = totalMsec;
            progressBar1.Maximum = (int)totalMsec;
        }

        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }

        public uint CurrentTime
        {
            set
            {
                TimeSpan t = new TimeSpan(0,0,0,0,(int)value);
                labelCurrentTime.Text = String.Format("Current Time: {0}:{1:00}.{2:000}",
                    t.Minutes, t.Seconds, t.Milliseconds);

                labelProgress.Text = String.Format("Progress: {0:00}%",
                    (100.0F * ((float)value / (float)m_totalMsec)));

                progressBar1.Value = (int)value;

                this.Refresh();
            }
        }


    }
}
