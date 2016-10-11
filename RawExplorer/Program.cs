using IO.Thermo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = new ThermoRawFile(args[0]);
            file.Open();
            
            Console.WriteLine(file);
            
            var ye = file.GetScan(14317);
            Console.WriteLine(ye);
            
            var spectrum = ye.MassSpectrum;

            Console.WriteLine(spectrum);


            Console.WriteLine(spectrum[2088]);
            Console.WriteLine(spectrum[2089]);
            Console.WriteLine(spectrum[2090]);
            Console.WriteLine(spectrum[2091]);
            Console.WriteLine(spectrum[2092]);
            Console.WriteLine(spectrum[2093]);
            Console.WriteLine(spectrum[2094]);
            Console.WriteLine(spectrum[2095]);
            Console.WriteLine(spectrum[2096]);

            Console.WriteLine(spectrum[2089].Charge);
            Console.WriteLine(spectrum[2089].Intensity);
            Console.WriteLine(spectrum[2089].MZ);
            Console.WriteLine(spectrum[2089].Noise);
            Console.WriteLine(spectrum[2089].Resolution);
            Console.WriteLine(spectrum[2089].SignalToNoise);

            Console.Read();
        }
    }
}
