using Proteomics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulProteomicsDatabases;

namespace AAptmlist
{
    class Program
    {
        static double tol = 0.0001;
        static void Main(string[] args)
        {
            Loaders.LoadElements("ok.txt");
            List<char> aminoAcidsToConsider = new List<char>() { 'G', 'A', 'S', 'P', 'V', 'T', 'C', 'I', 'L', 'N', 'D', 'Q', 'K', 'E', 'M', 'H', 'F', 'R', 'Y', 'W' };
            List<Tuple<AminoAcid, AminoAcid, double>> tuples = new List<Tuple<AminoAcid, AminoAcid, double>>();
            foreach (var ok in aminoAcidsToConsider)
            {
                var aa1 = AminoAcid.GetResidue(ok);
                foreach (var ok2 in aminoAcidsToConsider)
                {
                    var aa2 = AminoAcid.GetResidue(ok2);

                    var diff = aa2.MonoisotopicMass - aa1.MonoisotopicMass;

                    var tuple = new Tuple<AminoAcid, AminoAcid, double>(aa1, aa2, diff);
                    tuples.Add(tuple);
                }
            }

            // IGNORE ALL ZERO MASSES AND SUBSTITUTIONS TO ISOLEUCINE
            tuples = tuples.OrderBy(b => b.Item3).Where(b => b.Item3 != 0).ToList();

            Console.WriteLine("FOR EXCEL");

            double prevDiff = double.MinValue;
            foreach (var ok in tuples)
            {
                if (prevDiff > ok.Item3 - tol && prevDiff < ok.Item3 + tol)
                    Console.Write(" or " + ok.Item1.Name + "->" + ok.Item2.Name);
                else
                {
                    Console.Write("\n" + ok.Item3 + ";" + ok.Item1.Name + "->" + ok.Item2.Name);
                    prevDiff = ok.Item3;
                }
            }

            Console.WriteLine("\n\nFOR PTMLIST");
            foreach (var ok in tuples)
            {
                Console.WriteLine("ID   " + ok.Item1.Name + "->" + ok.Item2.Name);
                Console.WriteLine("FT   MOD_RES");
                Console.WriteLine("TG   " + ok.Item1.Name);
                Console.WriteLine("PP   Anywhere");
                Console.WriteLine("MM   " + ok.Item3);
                Console.WriteLine("//");
            }

            Console.WriteLine("\n\nFOR EXCEL");

            aminoAcidsToConsider.Reverse();

            foreach (var ok in aminoAcidsToConsider)
            {
                var aa1 = AminoAcid.GetResidue(ok);
                Console.WriteLine(-aa1.MonoisotopicMass + ";" + aa1.Name + " not here");
            }

            Console.WriteLine("\n\nFOR PTMLIST");
            foreach (var ok in aminoAcidsToConsider)
            {
                var aa1 = AminoAcid.GetResidue(ok);
                Console.WriteLine("ID   " + aa1.Name + " not here");
                Console.WriteLine("FT   MOD_RES");
                Console.WriteLine("TG   " + aa1.Name);
                Console.WriteLine("PP   Anywhere");
                Console.WriteLine("MM   -" + aa1.MonoisotopicMass);
                Console.WriteLine("//");
            }

            Console.Read();
        }
    }
}
