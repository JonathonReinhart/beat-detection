using FMOD;

using MathNet;
using MathNet.Numerics;
using MathNet.Numerics.Transformations;

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace CombFilter
{
    class Program
    {
        public const string MATLAB_PATH = "C:\\Documents and Settings\\Jonathon\\My Documents\\MATLAB\\";

        static void Main(string[] args)
        {
            string filename = PromptString("Please enter the audio filename:");
            CombFilterBPMCalc comb = new CombFilterBPMCalc(filename);

            comb.StartOffset = PromptDouble("Starting point in audio (sec):");
            comb.Duration = PromptDouble("Audio Duration (sec): ");
            
            comb.HannWindowLength = PromptDouble("Hanning Window length (sec):");
            double finalBPM = comb.EstimateBPM();

            Console.WriteLine("\nFinal BPM: {0}\n", finalBPM);
            exit();
        }



        private static double PromptDouble(string prompt)
        {
            Console.Write(prompt + " ");
            return double.Parse(Console.ReadLine());
        }

        private static string PromptString(string prompt)
        {
            Console.Write(prompt + " ");
            return Console.ReadLine();
        }


        private static void exit()
        {
            exit(0);
        }
        private static void exit(int exitCode)
        {
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
            Environment.Exit(exitCode);
        }


    }
}


