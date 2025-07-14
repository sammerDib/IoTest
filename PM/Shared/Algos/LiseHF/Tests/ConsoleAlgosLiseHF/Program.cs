using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnitySCPMSharedAlgosLiseHFWrapper;

namespace ConsoleAlgosLiseHF
{
    internal class Program
    {
       
        public static void Main(string[] args)
        {
            var saveBack = Console.BackgroundColor;
            var saveFront = Console.ForegroundColor;
            try
            {
                string outputDir = Directory.GetCurrentDirectory();
                string filePath = String.Empty;

                if (args == null || args.Length == 0)
                {
                    Console.WriteLine("No Arguments...\n");
                    ConsoleWriteUsage();
                    return;
                }

                if (args.Length > 2)
                {
                    Console.WriteLine("Too MUCH Arguments...\n");
                    ConsoleWriteUsage();
                    return;
                }


                if (args.Length >= 1 && (
                        (false == args[0].ToLower().EndsWith(".csv")) ||
                        (false == File.Exists(args[0]) ) )
                    ) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"# ERROR : Bad Input_CSV path <{args[0]}> does not exists or is not a CSV file\n");
                    Console.ForegroundColor = saveFront;
                    ConsoleWriteUsage();
                    return;
                }
                filePath = args[0];

                if (args.Length >= 2)
                {
                    string dir = args[1];
                    if (!dir.EndsWith("\\"))
                        dir += "\\";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    outputDir = dir;
                }

                if (!outputDir.EndsWith("\\"))
                    outputDir += "\\";

                var inputs = ReadAnalyseInputsSignals(filePath);

                var returns = Olovia_Algos.Compute(inputs);

                var outfile = $"{outputDir}LiseHFOutputs_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv";
                WriteReportOutputs(outfile,returns);

                if (returns.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Successfull Computation");
                    Console.ForegroundColor = saveFront;
                    Console.WriteLine("-------------------");

                    double Totaldepth_um = 0.0;
                    int layerid = 1;
                    foreach (var depth in returns.Outputs.MeasuredDepths)
                    {
                        // we do not count negative layers
                        if (depth < 0.0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Layer[{layerid}] = \t{depth} um\t(in={inputs.DepthLayers.Depths_um[layerid - 1]:F3}) -- SKIPPED < 0");
                            Console.ForegroundColor = saveFront;
                        }
                        else
                        {
                            Console.WriteLine($"Layer[{layerid}] = \t{depth} um\t(in={inputs.DepthLayers.Depths_um[layerid - 1]:F3})");
                            Totaldepth_um += depth;
                        }
                        layerid++;
                    }
                    Console.WriteLine("-------------------");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Total Depth = \t{Totaldepth_um} um");
                    Console.WriteLine($"Target Depth = \t{inputs.DepthLayers.Depths_um[0]} um  [+/- {inputs.DepthLayers.DepthsToleranceSearch_um[0]:F3}]");
                    Console.WriteLine($"Delta = \t{inputs.DepthLayers.Depths_um[0] - Totaldepth_um} um");
                    Console.WriteLine($"Quality =\t{returns.Outputs.Quality}");
                    Console.ForegroundColor = saveFront;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"*** Failed Computation : {returns.ErrorMessage}");
                    Console.ForegroundColor = saveFront;
                }

            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine($"### ERROR : {ex.Message}\n{ex.StackTrace}");

                Console.BackgroundColor = saveBack;
                Console.ForegroundColor = saveFront;
            }
            finally
            {
                ConsolePressEnterToExit();
            }
        }

        private static void ConsoleWriteUsage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Usage : ConsoleAlgosLiseHF.exe LiseHFInputs_CSV_reportPath [Outputs_CSV_FolderPath]  ");
            Console.WriteLine($"   'LiseHFInputs_CSV_reportPath' is an Input and should have a csv extension");
            Console.WriteLine($"   [Optional] 'Outputs_CSV_FolderPath' is where output report will be stored");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n Example :");
            Console.WriteLine($"ConsoleAlgosLiseHF C:\\LiseHF\\InputData\\LiseHFInputs_44.csv C:\\LiseHF\\OutputData");
            Console.WriteLine();
        }

        private static void ConsolePressEnterToExit()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"-------------------");
            Console.WriteLine($"Press Enter to exit");
            Console.ReadLine();
        }

        private static LiseHFAlgoInputs ReadAnalyseInputsSignals(string filepath)
        {
            string sep = System.Globalization.CultureInfo.InstalledUICulture.TextInfo.ListSeparator;
            var cSep = new char[] { sep[0] };


            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            LiseHFAlgoInputs inputs = new LiseHFAlgoInputs();
            inputs.OpMode = LiseHFMode.GridSearch; //LiseHFMode.FFTOnly : LiseHFMode.GridSearch;
            using (var sr = new StreamReader(filepath))
            {
                var firstline = sr.ReadLine();
                if (firstline.StartsWith($"# ProbeLiseHF signals (c) UNITY SC") == false)
                    throw new Exception($"Bad CSV Format - expect csv files coming from ANALYSE TSV depth reports");

                sr.ReadLine(); // #

                var prmline = sr.ReadLine();// # Integ. Time(ms) xx
                var val = prmline.Split(cSep, StringSplitOptions.RemoveEmptyEntries);
                int.TryParse(val[2], out int IntegerTime);

                prmline = sr.ReadLine();// # Attenuation ID xx
                val = prmline.Split(cSep, StringSplitOptions.RemoveEmptyEntries);
                int.TryParse(val[2], out int AttID);

                var thresholds = sr.ReadLine();// # Th1 xx.xxxx Th2 xx.xxxx
                var ths = thresholds.Split(cSep, StringSplitOptions.RemoveEmptyEntries);
                if (!double.TryParse(ths[2], out double th1))
                    throw new Exception($"Bad CSV Format - could Not Parse Th1 <{ths[1]}>");
                inputs.Threshold_signal_pct = th1;

                if (!double.TryParse(ths[4], out double th2))
                    throw new Exception($"Bad CSV Format - could Not Parse Th2 <{ths[2]}>");
                inputs.Threshold_peak_pct = th2;

                var TSVDiameterLine = sr.ReadLine();// # TSV Diameter (um) xx.xxxx
                var diam = TSVDiameterLine.Split(cSep, StringSplitOptions.RemoveEmptyEntries);
                if (!double.TryParse(diam[2], out double diameter))
                    throw new Exception($"Bad CSV Format - could Not Parse TSV Diameter <{diam[1]}> ");
                inputs.TSVDiameter_um = diameter;

                // layers
                sr.ReadLine();// # Layer No    Depth(um)    SearchTolerance(um)  RI
                bool layerdone = false; 
                var layers = new LiseHFLayers();
                int layerid = 0;
                do
                {
                   ++layerid;
                   string layerline = sr.ReadLine();
                   var layerprm = layerline.Split(cSep, StringSplitOptions.RemoveEmptyEntries);
                   layerdone = (layerprm.Length != 5);
                   if (!layerdone)
                   {
                        if (!double.TryParse(layerprm[2], out double depth_um))
                            throw new Exception($"Bad CSV Format - could Not Parse Layer[{layerid}] Depth <{layerprm[2]}> ");
                        if (!double.TryParse(layerprm[3], out double tol_um))
                            throw new Exception($"Bad CSV Format - could Not Parse Layer[{layerid}] Search Tolerance <{layerprm[3]}> ");
                        if (!double.TryParse(layerprm[4], out double ri))
                            throw new Exception($"Bad CSV Format - could Not Parse Layer[{layerid}] RI <{layerprm[4]}> ");
                        
                        layers.AddNewDepthLayer(depth_um, tol_um, ri);
                    }
                }
                while (!layerdone);
                layers.ComputeNative();
                inputs.DepthLayers = layers;

                const int defsize = 4096;
                List<double> wave = new List<double>(defsize);
                List<double> spectrum = new List<double>(defsize);
                List<double> darkcal = new List<double>(defsize);
                List<double> refcal = new List<double>(defsize);

                sr.ReadLine();// Wave(nm)    Signal     Dark Calib     Ref Calib
                while (sr.Peek() >= 0)
                {
                    // Read here
                    var line = (sr.ReadLine()).Trim();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var cols = line.Split(cSep,  StringSplitOptions.RemoveEmptyEntries);
                        if (cols.Length == 4)
                        {
                            if (double.TryParse(cols[0], out double dwav)) wave.Add(dwav);
                            if (double.TryParse(cols[1], out double dsig)) spectrum.Add(dsig);
                            if (double.TryParse(cols[2], out double ddark)) darkcal.Add(ddark);
                            if (double.TryParse(cols[3], out double dref)) refcal.Add(dref);
                        }
                    }
                }

                inputs.Wavelength_nm = wave;
                inputs.Spectrum = new LiseHFRawSignal(spectrum, IntegerTime, AttID);
                inputs.DarkSpectrum = new LiseHFRawSignal(darkcal, IntegerTime, AttID);
                inputs.RefSpectrum = new LiseHFRawSignal(refcal, IntegerTime, AttID);

            }
            return inputs;
        }

        private static void WriteReportOutputs(string filepath, LiseHFAlgoReturns algoret)
        {
            try
            {
                if (System.Threading.Thread.CurrentThread.CurrentCulture != System.Globalization.CultureInfo.InvariantCulture)
                    System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

                var success = algoret.IsSuccess ? "Success" : "FAIL";
                var fft = algoret.FFTDone ? "FFT Done" : "FFT FAIL";
                var analysis = "Analyze" + (algoret.AnalysisDone ? " OK" : " NOK");

                var csv = new StringBuilder(1024 * 256);
                string sep = System.Globalization.CultureInfo.InstalledUICulture.TextInfo.ListSeparator; ;
                csv.AppendLine($"# ProbeLiseHF outputs (c) UNITY SC{sep}");
                csv.AppendLine($"# {sep}");
                csv.AppendLine($"# {sep}{success}{sep}{fft}{sep}{fft}{analysis}");
                csv.AppendLine($"# {sep}{algoret.ErrorMessage}{sep}");
                csv.AppendLine($"# {sep}");

                if (algoret.Outputs != null)
                {
                    var o = algoret.Outputs;
                    csv.AppendLine($"# {sep}Sat %{sep}{o.SaturationPercentage:F1}{sep}Th S{sep}{o.ThresholdSignal}{sep}Th P{sep}{o.ThresholdPeak}");
                    csv.AppendLine($"# {sep}");

                    int fftmax = (o.FTTy != null) ? o.FTTy.Count : 0;
                    int pmax = (o.PeaksY != null) ? o.PeaksY.Count : 0;
                    int dmax = (o.MeasuredDepths != null) ? o.MeasuredDepths.Count : 0; ;
                    int gmax = Math.Max(fftmax, Math.Max(pmax, dmax));

                    if (gmax > 0)
                    {
                        csv.AppendLine($"X{sep}Y{sep} - {sep}Depths (um){sep} - {sep}Px{sep}Py{sep}-Quality ={sep}{o.Quality}");
                        for (int i = 0; i < gmax; i++)
                        {
                            if (i < fftmax)
                                csv.Append($"{o.FTTx[i]:F4}{sep}{o.FTTy[i]:F4}{sep}{sep}");
                            else
                                csv.Append($"{sep}{sep}{sep}");

                            if (i < dmax)
                                csv.Append($"{o.MeasuredDepths[i]:F6}{sep}{sep}");
                            else
                                csv.Append($"{sep}{sep}");

                            if (i < pmax)
                                csv.Append($"{o.PeaksX[i]:F4}{sep}{o.PeaksY[i]:F4}{sep}");
                            else
                                csv.Append($"{sep}{sep}");

                            csv.AppendLine();
                        }
                    }
                }
                File.WriteAllText(filepath, csv.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fail to Report LiseHF in <{filepath}>");
                Console.WriteLine($"Message : {ex.Message}\n");
                Console.WriteLine($"StackTrace : {ex.StackTrace}");
            }
        }

    }
}
