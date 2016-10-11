using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotchesExtractor
{
    class Program
    {
        static void Main(string[] args)
        {

            double tol = 0.0005;

            List<double> massDiffs = new List<double>();
            using (StreamReader sr = new StreamReader(args[0]))
            {
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    massDiffs.Add(Convert.ToDouble(line));
                }
            }
            //Console.WriteLine("Total mass diffs: " + massDiffs.Count);

            massDiffs.Sort();
            //Console.WriteLine("lowest: " + massDiffs.First());
            //Console.WriteLine("highest: " + massDiffs.Last());

            int i = 0;
            int j = 0;

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(args[0] + ".notches"))
            {

                file.WriteLine("Lowest\tHighest\tWidth\tCount\tAverage\tMedian\tShapiroWilkPvalue\tAndersonDarlingPvalue");
                while (j + 1 < massDiffs.Count)
                {
                    if (Math.Abs(massDiffs[j] - massDiffs[j + 1]) < tol)
                    {
                        //Console.WriteLine(massDiffs[j + 1] + " in same notch");
                    }
                    else
                    {
                        //Console.WriteLine(massDiffs[j + 1] + " NOT in same notch");

                        var theRange = massDiffs.GetRange(i, (j - i + 1));

                        if (j > 0 && theRange.Count >= 4)
                        {
                            //Console.WriteLine(string.Join(",", theRange));
                            //Accord.Statistics.Testing.ShapiroWilkTest ye = new Accord.Statistics.Testing.ShapiroWilkTest(theRange.ToArray());

                            //Accord.Statistics.Distributions.IUnivariateDistribution hypothesizedDistribution = new Accord.Statistics.Distributions.Univariate.NormalDistribution(theRange.Average(), theRange.StandardDeviation());

                            //Accord.Statistics.Testing.AndersonDarlingTest ye2 = new Accord.Statistics.Testing.AndersonDarlingTest(theRange.ToArray(),hypothesizedDistribution);

                            file.WriteLine(theRange.First() + "\t" + theRange.Last() + "\t" + (theRange.Last() - theRange.First()) + "\t" + (j - i + 1) + "\t" + theRange.Average() + "\t" + theRange.Median());
                            //file.WriteLine(theRange.First() + "\t" + theRange.Last() + "\t" + (theRange.Last() - theRange.First()) + "\t" + (j - i + 1) + "\t" + theRange.Average() + "\t" + theRange.Median() + "\t" + ye.PValue + "\t" + ye2.PValue);
                        }
                        i = j + 1;
                    }

                    j += 1;
                }

            }

        }
    }
}
