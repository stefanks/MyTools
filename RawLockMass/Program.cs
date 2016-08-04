using Chemistry;
using IO.Thermo;
using Proteomics;
using Spectra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RawLockMass
{
    class Program
    {
        static void Main(string[] args)
        {
            UsefulProteomicsDatabases.Loaders.LoadElements("elements.dat");


            var Compound1 = new Peptide("NNNNN");

            var dist1 = new IsotopicDistribution(Compound1.GetChemicalFormula(), 1, 0.001);
            Console.WriteLine("NNNNN masses: " + string.Join(", ", dist1.Masses));
            var dist1MZ = dist1.Masses.Select(b => b.ToMassToChargeRatio(1));
            Console.WriteLine("NNNNN mz: " + string.Join(", ", dist1MZ));

            var chemFormulaOfCompound2 = Compound1.GetChemicalFormula();
            chemFormulaOfCompound2.Add(new ChemicalFormula("N-1H-2"));
            var dist2 = new IsotopicDistribution(chemFormulaOfCompound2, 1, 0.001);
            Console.WriteLine("Compound2 masses: " + string.Join(", ", dist2.Masses));
            var dist2MZ = dist2.Masses.Select(b => b.ToMassToChargeRatio(1));
            Console.WriteLine("Compound2 mz: " + string.Join(", ", dist2MZ));


            using (System.IO.StreamWriter shiftsFile = new System.IO.StreamWriter(@"shifts.txt"))
            {
                foreach (var arg in args)
                {
                    var file = new ThermoRawFile(arg);
                    file.Open();

                    shiftsFile.WriteLine(file.FilePath);
                    shiftsFile.WriteLine(1);
                    Console.WriteLine(file.FilePath);
                    Console.WriteLine(file.FileType);

                    //foreach (var a in file.GetScan(911).MassSpectrum)
                    //{
                    //    Console.WriteLine(a);
                    //    if (a.MZ > 600)
                    //        Console.WriteLine("h");
                    //}


                    foreach (var scan in file)
                    {
                        Console.WriteLine(" " + scan);

                        List<MzPeak> theoreticalPeaksForCompound1;
                        List<MzPeak> theoreticalPeaksForCompound2;
                        List<MzPeak> experimentalPeaksForCompound1;
                        List<MzPeak> experimentalPeaksForCompound2;

                        if (scan.ScanNumber == 911)
                        {
                            Console.WriteLine("okay...");
                        }

                        var extract = scan.MassSpectrum.newSpectrumExtract(dist1MZ.First() - 0.004, dist1MZ.First() + 0.004);


                        //Console.WriteLine(" Error of most intense peak: " +);
                        //Console.WriteLine(" Simple average of all matched peaks: " +);
                        //Console.WriteLine(" Simple average of most intense peak: " +);
                        //Console.WriteLine(" Simple average of all matched peaks: " +);
                        //Console.WriteLine(" Simple average of all matched peaks: "  +);
                        //Console.WriteLine(" Simple average of all matched peaks: " +);
                    }
                }
            }
        }
    }
}
