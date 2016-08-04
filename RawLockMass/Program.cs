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

            var dist1MZ = new IsotopicDistribution(Compound1.GetChemicalFormula(), 1, 0.001).Masses.Select(b => b.ToMassToChargeRatio(1)).ToList();
            Console.WriteLine("NNNNN mz: " + string.Join(", ", dist1MZ));

            var chemFormulaOfCompound2 = Compound1.GetChemicalFormula();
            chemFormulaOfCompound2.Add(new ChemicalFormula("N-1H-2"));
            var dist2MZ = new IsotopicDistribution(chemFormulaOfCompound2, 1, 0.001).Masses.Select(b => b.ToMassToChargeRatio(1)).ToList();
            Console.WriteLine("Compound2 mz: " + string.Join(", ", dist2MZ));


            foreach (var arg in args)
            {
                using (System.IO.StreamWriter shiftsFile = new System.IO.StreamWriter(arg+".tsv"))
                {
                    var file = new ThermoRawFile(arg);
                    file.Open();

                    shiftsFile.WriteLine(file.FilePath);
                    Console.WriteLine(file.FilePath);
                    Console.WriteLine(file.FileType);

                    foreach (var scan in file)
                    {
                        double monoError = double.NaN;
                        double sumErrors = double.NaN;
                        var extract1 = scan.MassSpectrum.newSpectrumExtract(dist1MZ[0] - 0.004, dist1MZ[0] + 0.004);
                        int count = 0;
                        if (extract1.Count > 0)
                        {
                            monoError = dist1MZ[0] - extract1.PeakWithHighestY.MZ;
                            count += 1;

                            // Let's look for other isotopic peaks!
                            for (int i = 1; i < 4; i++)
                            {
                                var extract = scan.MassSpectrum.newSpectrumExtract(dist1MZ[i] - monoError - 0.0004, dist1MZ[i] - monoError + 0.0004);
                                if (extract.Count > 0)
                                {
                                    if (double.IsNaN(sumErrors))
                                        sumErrors = monoError;
                                    sumErrors += dist1MZ[i] - extract.PeakWithHighestY.MZ;
                                    count += 1;
                                }
                            }
                            sumErrors /= count;
                        }

                        if (count > 1)
                        {
                            Console.WriteLine("scan " + scan.ScanNumber);
                            Console.WriteLine("count = " + count);
                            Console.WriteLine("monoError = " + monoError);
                            Console.WriteLine("sumErrors = " + sumErrors);
                        }
                        shiftsFile.WriteLine(scan.ScanNumber + "\t" + monoError+"\t"+ sumErrors);
                        
                    }
                }
            }
        }
    }
}
