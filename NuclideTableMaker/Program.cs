using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefulProteomicsDatabases;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using Chemistry;

namespace NuclideTableMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            Loaders.LoadElements(@"elements.dat");

            Excel.Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet;
            Excel.Range oRng;

            try
            {
                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = oXL.Workbooks.Add(Missing.Value);
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                for (int i = 1; i <= 82; i++)
                {
                    try
                    {
                        var element = PeriodicTable.GetElement(i);
                        oSheet.Cells[i, 1] = element.AtomicSymbol + "\n" + Math.Round(element.AverageMass,5);
                        var yeh = oSheet.Cells[i, 1];
                        Excel.Borders borderh = yeh.Borders;
                        borderh.LineStyle = Excel.XlLineStyle.xlContinuous;
                        borderh.Weight = 2d;
                        yeh.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        yeh.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        yeh.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleGoldenrod);

                        //int shift = element.Isotopes.First().Neutrons - 3;
                        int shift = element.Protons-3;

                        foreach (var ok in element.Isotopes)
                        {
                            oSheet.Cells[i, ok.Neutrons - shift] = element.AtomicSymbol + " " + ok.MassNumber + "\n" + Math.Round(ok.AtomicMass,5) + "\n" + Math.Round(ok.RelativeAbundance,6);
                            oSheet.Cells[i, ok.Neutrons - shift].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            oSheet.Cells[i, ok.Neutrons - shift].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            oSheet.Cells[i, ok.Neutrons - shift].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);

                            var ye = oSheet.Cells[i, ok.Neutrons - shift];

                            Excel.Borders border = ye.Borders;
                            border.LineStyle = Excel.XlLineStyle.xlContinuous;
                            border.Weight = 2d;

                            if (element.PrincipalIsotope == ok)
                            {
                                oSheet.Cells[i, ok.Neutrons - shift].Font.Bold = true;
                                oSheet.Cells[i, ok.Neutrons - shift].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Turquoise);
                            }
                        }
                    }
                    catch
                    {

                    }
                }


                oXL.Visible = true;
                oXL.UserControl = true;
            }
            catch (Exception theException)
            {
                string errorMessage;
                errorMessage = "Error: ";
                errorMessage = string.Concat(errorMessage, theException.Message);
                errorMessage = string.Concat(errorMessage, " Line: ");
                errorMessage = string.Concat(errorMessage, theException.Source);

                Console.WriteLine(errorMessage);
            }

        }
    }
}
