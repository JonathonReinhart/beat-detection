using System;
using System.Collections.Generic;
using System.Text;

namespace BeatDetect
{
    class Program
    {
        static void Main(string[] args)
        {
            Subband s = new Subband(1000, 32, 1024);
            Console.WriteLine(s);

            Console.Read();
            
        }
    }
}
