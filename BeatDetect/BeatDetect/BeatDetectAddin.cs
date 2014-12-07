using FMOD;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Vixen;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace BeatDetect
{
    public class BeatDetectAddin : IAddIn
    {
        private IBeatDetector m_beatDetect;   

        public bool Execute(EventSequence sequence)
        {
            if (sequence == null)
                throw new Exception("Beat Detection requires a sequence to be open.");
            if (sequence.Audio == null)
                throw new Exception("Beat Detection requires this sequence to have Audio assigned.");

            MainDialog dialog = new MainDialog(sequence);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Clear the event data on the beat track
                for (int i = 0; i < sequence.TotalEventPeriods; i++)
                {
                    sequence.EventValues[dialog.BeatTrackChannel, i] = 0;
                }

                // Load beat detector
                m_beatDetect = new VariableWidthFrequencySelectedBD(
                    sequence, dialog.HistoryLength, dialog.Constant,
                    dialog.SubbandThreshold, dialog.BeatTrackChannel);
                return m_beatDetect.Execute();
            }

            return false;
        }


        

        public LoadableDataLocation DataLocationPreference
        {
            get { return LoadableDataLocation.Sequence; }
        }

        public void Loading(XmlNode dataNode)
        {
            
        }


        public void Unloading()
        {
            
        }

        

        public string Author
        {
            get { return "Jonathon Reinhart"; }
        }

        public string Description
        {
            get { return "Automatic Beat Detection Add-In"; }
        }

        public string Name
        {
            get { return "Beat Detect"; }
        }

        
    }
}
