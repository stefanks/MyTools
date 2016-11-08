using Chemistry;
using IO.Thermo;
using Proteomics;
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
            var tol = 0.004;

            var Compound1 = new Peptide("NNNNN");

            var regularMZ = new IsotopicDistribution(Compound1.GetChemicalFormula(), 0.1, 0.001).Masses.Select(b => b.ToMassToChargeRatio(1)).ToList();
            Console.WriteLine("NNNNN mz: " + string.Join(", ", regularMZ));

            var withAmmoniaLoss = Compound1.GetChemicalFormula();
            withAmmoniaLoss.Add(new ChemicalFormula("N-1H-2"));
            var withAmmoniaLossMZ = new IsotopicDistribution(withAmmoniaLoss, 0.1, 0.001).Masses.Select(b => b.ToMassToChargeRatio(1)).ToList();
            Console.WriteLine("withAmmoniaLoss mz: " + string.Join(", ", withAmmoniaLossMZ));

            var deamidated = Compound1.GetChemicalFormula();
            deamidated.Add(new ChemicalFormula("H-1N-1O"));
            var deamidatedMZ = new IsotopicDistribution(deamidated, 0.1, 0.001).Masses.Select(b => b.ToMassToChargeRatio(1)).ToList();
            Console.WriteLine("deamidated mz: " + string.Join(", ", deamidatedMZ));

            List<List<double>> allDistributions = new List<List<double>>() { regularMZ, withAmmoniaLossMZ, deamidatedMZ };

            foreach (var arg in args)
            {
                using (System.IO.StreamWriter shiftsFile = new System.IO.StreamWriter(arg + ".tsv"))
                {
                    var file = new ThermoRawFile(arg);
                    file.Open();

                    shiftsFile.WriteLine(file.FilePath);
                    Console.WriteLine(file.FilePath);

                    foreach (var scan in file)
                    {
                        double bestIntensity = 0;
                        double monoError = double.NaN;
                        foreach (var dist in allDistributions)
                        {
                            var monoisotopicPeak = scan.MassSpectrum.newSpectrumExtract(dist[0] - tol, dist[0] + tol).PeakWithHighestY;
                            if (monoisotopicPeak != null && bestIntensity < monoisotopicPeak.Intensity)
                            {
                                bestIntensity = monoisotopicPeak.Intensity;
                                monoError = dist[0] - monoisotopicPeak.MZ;
                            }
                        }
                        shiftsFile.WriteLine(scan.ScanNumber + "\t" + monoError);
                    }
                }
            }
        }
    }
}
