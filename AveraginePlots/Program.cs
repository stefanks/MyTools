using Chemistry;
using Proteomics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulProteomicsDatabases;

namespace AveraginePlots
{
    class Program
    {
        static void Main(string[] args)
        {
            Loaders.LoadElements(@"elements.dat");

            const double averageC = 4.9384;
            const double averageH = 7.7583;
            const double averageO = 1.4773;
            const double averageN = 1.3577;
            const double averageS = 0.0417;

            for (int i = 11; i <=11; i++)
            {
                double factor = Math.Pow(2, 0.5 * i);
                Console.WriteLine("Approx number of amino acids: " + Convert.ToInt32(factor));

                ChemicalFormula chemicalFormula = new ChemicalFormula();

                chemicalFormula.Add("C", Convert.ToInt32(averageC * factor));
                chemicalFormula.Add("H", Convert.ToInt32(averageH * factor));
                chemicalFormula.Add("O", Convert.ToInt32(averageO * factor));
                chemicalFormula.Add("N", Convert.ToInt32(averageN * factor));
                chemicalFormula.Add("S", Convert.ToInt32(averageS * factor));
                
                Console.WriteLine("Formula," + chemicalFormula.Formula);
                Console.WriteLine("Monoisotopic mass," + chemicalFormula.MonoisotopicMass);
                Console.WriteLine("Average mass," + chemicalFormula.AverageMass);

                var numPeaks = MathNet.Numerics.Combinatorics.Combinations(Convert.ToInt32(averageC * factor) + 1, 1) *
                    MathNet.Numerics.Combinatorics.Combinations(Convert.ToInt32(averageH * factor) + 1, 1) *
                    MathNet.Numerics.Combinatorics.Combinations(Convert.ToInt32(averageO * factor) + 2, 2) *
                    MathNet.Numerics.Combinatorics.Combinations(Convert.ToInt32(averageN * factor) + 1, 1) *
                    MathNet.Numerics.Combinatorics.Combinations(Convert.ToInt32(averageS * factor) + 3, 3);
                Console.WriteLine("Combinatorial number of peaks," + numPeaks);
                
                IsotopicDistribution ye;
                double fineRes;

                #region Find lowest possible width that is same as widest one for zero min prob

                fineRes = Math.Pow(2, -1);
                ye = new IsotopicDistribution(chemicalFormula, fineRes, 0);
                int firstCount = ye.Intensities.Count;
                do
                {
                    fineRes /= 2;
                    ye = new IsotopicDistribution(chemicalFormula, fineRes, 0);
                } while (ye.Intensities.Count <= firstCount);
                fineRes *= 2;
                ye = new IsotopicDistribution(chemicalFormula, fineRes, 0);

                Console.WriteLine(" fineRes" + ", " + fineRes);

                OutputMassesAndIntensities(ye, fineRes, 0, ye.Masses.Count);

                for (int j = 0; j < ye.Masses.Count - 1; j++)
                    Console.Write((j + 1) + ",");
                Console.WriteLine("");
                for (int j = 0; j< ye.Masses.Count - 1; j++)
                    Console.Write((ye.Masses[j + 1] - ye.Masses[j])+",");
                Console.WriteLine("");

                #endregion

                #region Redo with only high probabilities

                var maxIntensity = ye.Intensities.Max();

                fineRes = Math.Pow(2, -1);
                ye = new IsotopicDistribution(chemicalFormula, fineRes, maxIntensity / 10000);
                firstCount = ye.Intensities.Count;
                do
                {
                    fineRes /= 2;
                    ye = new IsotopicDistribution(chemicalFormula, fineRes, maxIntensity / 10000);
                } while (ye.Intensities.Count <= firstCount);
                fineRes *= 2;
                ye = new IsotopicDistribution(chemicalFormula, fineRes, maxIntensity / 10000);

                Console.WriteLine(" fineRes" + ", " + fineRes);

                OutputMassesAndIntensities(ye, fineRes, 0, ye.Masses.Count);

                #endregion

                //#region For 0 min probability try to reach numPeaks

                //fineRes = Math.Pow(2, -2);
                //do
                //{
                //    ye = new IsotopicDistribution(chemicalFormula, fineRes, 0);
                //    Console.WriteLine(ye.Intensities.Count);
                //    fineRes /= 2;
                //} while (ye.Intensities.Count < numPeaks);

                //#endregion

                #region For a given high resolution, use lowest allowed probability

                fineRes = Math.Pow(2, -18);
                double minProb = maxIntensity / 10000;

                ye = new IsotopicDistribution(chemicalFormula, fineRes, minProb);

                Console.WriteLine(" minProbability" + ", " + minProb);
                Console.WriteLine(" fineRes" + ", " + fineRes);


                OutputMassesAndIntensities(ye, fineRes, 0, ye.Masses.Count);


                #endregion

                #region For the selected run, zoom in

                var maxInt = ye.Intensities.Skip(1).Max();
                var indexOfMax = ye.Intensities.IndexOf(maxInt);

                var massOfMax = ye.Masses[indexOfMax];
                var filteredMasses = ye.Masses.Where(b => b > massOfMax - 0.5 && b < massOfMax + 0.5);

                var lowestIndex = ye.Masses.IndexOf(filteredMasses.First());
                var highestIndex = ye.Masses.IndexOf(filteredMasses.Last());


                Console.WriteLine("Zoomed in:");

                OutputMassesAndIntensities(ye, fineRes, lowestIndex, highestIndex);
                #endregion

            }
            Console.WriteLine("done");
            Console.Read();
        }

        private static void OutputMassesAndIntensities(IsotopicDistribution ye, double fineRes, int lowestIndex, int highestIndex)
        {
            Console.WriteLine(string.Join(", ", ye.Masses));
            Console.WriteLine(string.Join(",", ye.Intensities));
            for (int k = lowestIndex; k < highestIndex; k++)
            {
                Console.Write(ye.Masses[k] - fineRes / 2);
                Console.Write(",");
                Console.Write(ye.Masses[k]);
                Console.Write(",");
                Console.Write(ye.Masses[k] + fineRes / 2);
                Console.Write(",");
            }
            Console.WriteLine();
            for (int k = lowestIndex; k < highestIndex; k++)
            {
                Console.Write(-1);
                Console.Write(",");
                Console.Write(ye.Intensities[k]);
                Console.Write(",");
                Console.Write(-1);
                Console.Write(",");
            }

            Console.WriteLine();

        }
    }
}
