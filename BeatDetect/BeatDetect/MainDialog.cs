using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BeatDetect
{
    public partial class MainDialog : Form
    {
        private Vixen.EventSequence m_sequence;

        public MainDialog(Vixen.EventSequence sequence)
        {
            InitializeComponent();

            m_sequence = sequence;
            foreach (Vixen.Channel channel in m_sequence.OutputChannels)
            {
                comboBoxBeatTrack.Items.Add(String.Format("{0,-3} - {1}", channel.OutputChannel+1, channel.Name));
            }
            comboBoxBeatTrack.SelectedIndex = 0;
        }

        public int HistoryLength
        {
            get { return Int32.Parse(textBoxHistLength.Text); }
        }

        public int Constant
        {
            get { return Int32.Parse(textBoxConstant.Text); }
        }

        public int SubbandThreshold
        {
            get { return Int32.Parse(textBoxSubbandThreshold.Text); }
        }

        public int BeatTrackChannel
        {
            get { return comboBoxBeatTrack.SelectedIndex; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (dataExists(BeatTrackChannel))
            {
                DialogResult result = MessageBox.Show("The selected channel contains data. OK to overwrite?",
                    "Data Exists", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            
        }

        private bool dataExists(int channel)
        {
            for (int i = 0; i < m_sequence.TotalEventPeriods; i++)
            {
                if (m_sequence.EventValues[channel, i] > 0)
                    return true;
            }
            return false;
        }


    }
}
