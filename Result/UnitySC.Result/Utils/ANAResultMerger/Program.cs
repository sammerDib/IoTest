using System;
using System.CommandLine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Format.Base;
using System.Threading;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Format.Metro.NanoTopo;
using Microsoft.Win32;
using System.Linq.Expressions;



namespace ANAResultMerger
{
    internal class Program
    {
        public static bool ArgsErrors = false;
        public static string ResultDirPath;
        public static int SlotID;
        public static string ResTypeExt;
        public static bool NoSubFolders = false;
        public static string OutputDirPath;

        static void Main(string[] args)
        {
           BootStartRegister();

            var rootCommand = new RootCommand();

            var filerestypeOption = new Option<string>("--resutType", "[Mandatory] Use a result definition like *.anatsv ");
            filerestypeOption.AddAlias("-rt");

            var slotOption = new Option<int>("--slot", "[Mandatory] select slot id to compute");
            slotOption.AddAlias("-s");
  
            var folderOption = new Option<string>("--folder", "[Mandatory] Use a specific Lot/Recipe folder.");
            folderOption.AddAlias("-f");

            var outputDirOption = new Option<string>("--output", "[Optional] Merger output directory. otherwise use by Default current exe folder.");
            outputDirOption.AddAlias("-o");

            var NoSubfoldersOption = new Option<bool>("--TopDir", "[Optional] do not Search in subfolders .");
            NoSubfoldersOption.AddAlias("-top");

            var verboseOption = new Option<bool>("--verboseLog", "[Optional] display more information in console .");
            verboseOption.AddAlias("-v");

            rootCommand.AddOption(filerestypeOption);
            rootCommand.AddOption(slotOption);
            rootCommand.AddOption(folderOption);
            rootCommand.AddOption(outputDirOption);
            rootCommand.AddOption(NoSubfoldersOption);
            rootCommand.AddOption(verboseOption);

            rootCommand.TreatUnmatchedTokensAsErrors = true;

            rootCommand.Description = "Use options -rt and -f are mandatory\nExample: ANAResultMerger -rt anatsv -f \"c:\\MyLot\\MyRecipe\\\" \n\n WARNING : DO NOT MIX DIFFERENT RESULT RECIPE or WAFERS Kind !";

            rootCommand.SetHandler((filerestype, slot, folder, outputDir, noSubfolder, verbose) =>
            {
                MergeResults(filerestype, slot, folder, outputDir, noSubfolder, verbose);
            },
            filerestypeOption, slotOption, folderOption, outputDirOption, NoSubfoldersOption, verboseOption);

            int exitCode = rootCommand.Invoke(args);

            if (exitCode != 0 || ArgsErrors)
            {

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Command line invocation failed: {exitCode} \n");
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("-------------------");
            Console.WriteLine("Press ENTER to Exit");
            Console.ReadLine();

        }

        static private bool ApplyConfigOptions(string filerestype, int slotId, string folderToScan, string outputDir, bool noSubfolder)
        {

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Parameters :");
            if (!string.IsNullOrEmpty(filerestype))
            {
                Console.Write($"- Find results type : {filerestype}");

                var restyp = ResultFormatExtension.GetResultTypeFromExtension(filerestype);
                if (restyp == ResultType.NotDefined)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($" => ERROR NOT DEFINED");
                    ArgsErrors = true;
                }
                else if (restyp.GetActorType() != ActorType.ANALYSE)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($" => NOT a ANALYSE results");
                    ArgsErrors = true;
                }
                else
                    ResTypeExt = filerestype;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"- ERROR : : missing result type | -t <fileextension>");
                ArgsErrors = true;
            }

            if (String.IsNullOrEmpty(folderToScan) || String.IsNullOrEmpty(folderToScan) || File.Exists(folderToScan))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"- ERROR : Target folder is missing or does not exist | -f <LotFolder>");
                ArgsErrors = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"- Find result in <{folderToScan}>");
                ResultDirPath = folderToScan;
            }

            if (slotId != 0 && (1 <= slotId && slotId <= 25))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"- Find Slot result : S{slotId}");
                SlotID = slotId;
                ResTypeExt = $"*_S{SlotID}_*.{ResTypeExt}";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"- WARNING : Find ALL Slot results | or specify one -s <SlotId [1-25]>");
                if (!ArgsErrors)
                    ResTypeExt = $"*_S*.{ResTypeExt}";
            }

            if (string.IsNullOrEmpty(outputDir))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                OutputDirPath = Directory.GetCurrentDirectory();
                Console.WriteLine($"- WARNING : use default output dir <{OutputDirPath}>");
            }
            else
            {
                try
                {
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }
                    OutputDirPath = outputDir;
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    OutputDirPath = Directory.GetCurrentDirectory(); 
                    Console.WriteLine($"- WARNING : use default output dir <{OutputDirPath}> due to exception:\n  *  {ex.Message}");
                }
            }

            NoSubFolders = noSubfolder;
            return !ArgsErrors;
        }


        static private void MergeResults(string filerestype, int slotId, string folderToScan, string outputDir, bool noSubfolder, bool verbose)
        {
            if (!ApplyConfigOptions(filerestype, slotId, folderToScan, outputDir, noSubfolder))
                return;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nArgs : folder = {ResultDirPath}\ntype = {ResTypeExt}\nslot = {SlotID}\noutputdir= {OutputDirPath}\n");
            Console.ForegroundColor = ConsoleColor.White;

            // find all files to parse
            string[] fileEntries = Directory.GetFiles(ResultDirPath, ResTypeExt, NoSubFolders ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
            int initIdStart = 0;
            int id = initIdStart;

            if (fileEntries.Length < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR : Not Enougth files to merge [Found = {fileEntries.Length}]");
                return;
            }

            // create result
            var restyp = ResultFormatExtension.GetResultTypeFromExtension(Path.GetExtension(ResTypeExt));
            MetroResult metroResult = null;

            string sdatetime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            string mergeFileName = Path.Combine(OutputDirPath, $"Merger_{sdatetime}", $"M-{Path.GetFileName(fileEntries[0])}");

            //How many Slot / How many measure

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"-----------------");
            Console.WriteLine($"Start Merging {fileEntries.Length} files...\n");
            foreach (string entry in fileEntries)
            {

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"{id+1:00}/{fileEntries.Length:00} : {entry}");

                if (id == initIdStart)
                {
                    metroResult = new MetroResult(restyp, -1, entry);
                    metroResult.ResFilePath = mergeFileName;
                    foreach (var pt in metroResult.MeasureResult.Points)
                    {
                        pt.Datas.Clear();
                    }
                    foreach (var die in metroResult.MeasureResult.Dies)
                    {
                        foreach (var pt in die.Points)
                        {
                            pt.Datas.Clear();
                        }
                    }
                }

                var result = new MetroResult(restyp, -1, entry);
                UpdateRepetaId(metroResult, result, id, verbose);
                CopyExternalFile(metroResult, result, id, verbose);
                
                ++id;
            }


            if (!metroResult.WriteInFile(mergeFileName, out string errorWrite))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR while saving merged result = {errorWrite}]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Result Successfully merged in <{mergeFileName}>");

            }
        }

        private static void UpdateRepetaId(MetroResult mergeResult, MetroResult repetResult, int repId, bool verbose)
        {
            if (repetResult.MeasureResult.Points != null)
            {
                foreach (var pt in repetResult.MeasureResult.Points)
                {
                    // find pt in merge sinon on l'ajoute
                    var mergePointRes = mergeResult.MeasureResult.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));
                    if (mergePointRes == null)
                    {

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"   add NEW Point - SiteId={pt.SiteId} - X={pt.XPosition} - Y={pt.YPosition}");

                        // add to do 
                        mergeResult.MeasureResult.Points.Add(pt);
                        foreach (var ptdata in pt.Datas)
                        {
                            //update point data with id == to rep id
                            var measurePointResult = ptdata;
                            measurePointResult.IndexRepeta = repId;
                        }
                    }
                    else
                    {
                        var data = pt.Datas.ToList();
                        foreach (var ptdata in data)
                        {
                            //add point data with id == to rep id
                            var measurePointResult = ptdata;
                            measurePointResult.IndexRepeta = repId;
                            mergePointRes.Datas.Add(measurePointResult);
                            if (verbose)
                            {
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine($"   add {measurePointResult.ToString()}");
                            }
                        }
                    }
                }
            }

            if (repetResult.MeasureResult.Dies != null)
            {
                foreach (var die in repetResult.MeasureResult.Dies)
                {
                    var mergeDie = mergeResult.MeasureResult.Dies.Find(x => (x.ColumnIndex == die.ColumnIndex) && (x.RowIndex == die.RowIndex));
                    if (mergeDie == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"   add NEW Die - [{die.ColumnIndex} - {die.RowIndex}]");

                        // add die  
                        mergeResult.MeasureResult.Dies.Add(die);
                        foreach (var pt in die.Points)
                        {
                            foreach (var ptdata in pt.Datas)
                            {
                                //update point data with id == to rep id
                                var measurePointResult = ptdata;
                                measurePointResult.IndexRepeta = repId;
                            }
                        }
                    }
                    else
                    {
                        foreach (var pt in die.Points)
                        {
                            // find pt in merge sinon on l'ajoute
                            var mergePointRes = mergeDie.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));
                            if (mergePointRes == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"   add NEW Die Point - SiteId={pt.SiteId} - X={pt.XPosition} - Y={pt.YPosition}");

                                // add to do 
                                mergeDie.Points.Add(pt);
                                foreach (var ptdata in pt.Datas)
                                {
                                    //update point data with id == to rep id
                                    var measurePointResult = ptdata;
                                    measurePointResult.IndexRepeta = repId;
                                }
                            }
                            else
                            {
                                var data = pt.Datas.ToList();
                                foreach (var ptdata in data)
                                {
                                    //add point data with id == to rep id
                                    var measurePointResult = ptdata;
                                    measurePointResult.IndexRepeta = repId;
                                    mergePointRes.Datas.Add(measurePointResult);
                                    if (verbose)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                        Console.WriteLine($"   add {measurePointResult.ToString()}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CopyExternalFile(MetroResult mergeResult, MetroResult repetResult, int repId, bool verbose)
        {
            var baseMergeFile = Path.GetDirectoryName(mergeResult.ResFilePath);
            var baseRepetaFile = Path.GetDirectoryName(repetResult.ResFilePath);

            switch ((int)mergeResult.ResType)
            {
                case (int)ResultType.ANALYSE_TSV: // ResultImageFileName

                    if (repetResult.MeasureResult.Points != null)
                    {
                        foreach (var pt in repetResult.MeasureResult.Points)
                        {
                            var mergePointRes = mergeResult.MeasureResult.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));
                           
                            var data = pt.Datas.OfType<TSVPointData>().ToList();
                            foreach (var ptdata in data)
                            {
                                var thumbfile = (ptdata as TSVPointData).ResultImageFileName;
                                if (thumbfile != null)
                                {
                                    CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, thumbfile, verbose, "TSV Thumbnail");
                                    var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                    (mergePtData as TSVPointData).ResultImageFileName = $"{repId:000}-{thumbfile}";
                                }
                            }
                        }
                    }
                    if (repetResult.MeasureResult.Dies != null)
                    {
                        foreach (var die in repetResult.MeasureResult.Dies)
                        {
                            foreach (var pt in die.Points)
                            {
                                var mergePointRes = die.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));

                                var data = pt.Datas.OfType<TSVPointData>().ToList();
                                foreach (var ptdata in data)
                                {
                                    var thumbfile = (ptdata as TSVPointData).ResultImageFileName;
                                    if (thumbfile != null)
                                    {
                                        CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, thumbfile, verbose, "Die TSV Thumbnail");
                                        var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                        (mergePtData as TSVPointData).ResultImageFileName = $"{repId:000}-{thumbfile}";
                                    }
                                }
                            }
                        }
                    }
                    break;

                case (int)ResultType.ANALYSE_Topography: // ResultImageFileName / ReportFileName 
                    if (repetResult.MeasureResult.Points != null)
                    {
                        foreach (var pt in repetResult.MeasureResult.Points)
                        {
                            var mergePointRes = mergeResult.MeasureResult.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));

                            var data = pt.Datas.OfType<TopographyPointData>().ToList();
                            foreach (var ptdata in data)
                            {
                                var thumbfile = (ptdata as TopographyPointData).ResultImageFileName;
                                if (thumbfile != null)
                                {
                                    CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, thumbfile, verbose, "TOPO Thumbnail");
                                    var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                    (mergePtData as TopographyPointData).ResultImageFileName = $"{repId:000}-{thumbfile}";
                                }

                                var report = (ptdata as TopographyPointData).ReportFileName;
                                if (report != null)
                                {
                                    CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, report, verbose, "TOPO Report");
                                    var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                    (mergePtData as TopographyPointData).ReportFileName = $"{repId:000}-{report}";
                                }
                            }
                        }
                    }
                    if (repetResult.MeasureResult.Dies != null)
                    {
                        foreach (var die in repetResult.MeasureResult.Dies)
                        {
                            foreach (var pt in die.Points)
                            {
                                var mergePointRes = die.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));

                                var data = pt.Datas.OfType<TopographyPointData>().ToList();
                                foreach (var ptdata in data)
                                {
                                    var thumbfile = (ptdata as TopographyPointData).ResultImageFileName;
                                    if (thumbfile != null)
                                    {
                                        CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, thumbfile, verbose, "Die TOPO Thumbnail");
                                        var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                        (mergePtData as TopographyPointData).ResultImageFileName = $"{repId:000}-{thumbfile}";
                                    }

                                    var report = (ptdata as TopographyPointData).ReportFileName;
                                    if (report != null)
                                    {
                                        CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, report, verbose, "Die TOPO Report");
                                        var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                        (mergePtData as TopographyPointData).ReportFileName = $"{repId:000}-{report}";
                                    }
                                }
                            }
                        }
                    }
                    break;
                case (int)ResultType.ANALYSE_NanoTopo: // ResultImageFileName / ReportFileName 
                    if (repetResult.MeasureResult.Points != null)
                    {
                        foreach (var pt in repetResult.MeasureResult.Points)
                        {
                            var mergePointRes = mergeResult.MeasureResult.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));

                            var data = pt.Datas.OfType<NanoTopoPointData>().ToList();
                            foreach (var ptdata in data)
                            {
                                var thumbfile = (ptdata as NanoTopoPointData).ResultImageFileName;
                                if (thumbfile != null)
                                {
                                    CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, thumbfile, verbose, "NTP Thumbnail");
                                    var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                    (mergePtData as NanoTopoPointData).ResultImageFileName = $"{repId:000}-{thumbfile}";
                                }

                                var report = (ptdata as NanoTopoPointData).ReportFileName;
                                if (report != null)
                                {
                                    CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, report, verbose, "NTP Report");
                                    var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                    (mergePtData as NanoTopoPointData).ReportFileName = $"{repId:000}-{report}";
                                }
                            }
                        }
                    }
                    if (repetResult.MeasureResult.Dies != null)
                    {
                        foreach (var die in repetResult.MeasureResult.Dies)
                        {
                            foreach (var pt in die.Points)
                            {
                                var mergePointRes = die.Points.Find(x => (x.SiteId == pt.SiteId) && x.XPosition.Near(pt.XPosition, 0.01) && x.YPosition.Near(pt.YPosition, 0.01));

                                var data = pt.Datas.OfType<NanoTopoPointData>().ToList();
                                foreach (var ptdata in data)
                                {
                                    var thumbfile = (ptdata as NanoTopoPointData).ResultImageFileName;
                                    if (thumbfile != null)
                                    {
                                        CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, thumbfile, verbose, "Die NTP Thumbnail");
                                        var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                        (mergePtData as NanoTopoPointData).ResultImageFileName = $"{repId:000}-{thumbfile}";
                                    }

                                    var report = (ptdata as NanoTopoPointData).ReportFileName;
                                    if (report != null)
                                    {
                                        CopyOneExtResultFile(repId, baseRepetaFile, baseMergeFile, report, verbose, "Die NTP Report");
                                        var mergePtData = mergePointRes.Datas.Find(x => (x.IndexRepeta == repId));
                                        (mergePtData as NanoTopoPointData).ReportFileName = $"{repId:000}-{report}";
                                    }
                                }
                            }
                        }
                    }
                    break;
                default:break;
            }
        }


        private static void  CopyOneExtResultFile(int repId, string baseSrc, string baseDest, string filpath, bool verbose, string kind)
        {
            if (!string.IsNullOrEmpty(filpath))
            {
                var dest = Path.Combine(baseDest, $"{repId:000}-{filpath}");
                if (verbose)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"   Copying {kind} {filpath} to {dest}");
                }
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    File.Copy(Path.Combine(baseSrc, filpath), dest);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   Failed to copy {kind} - {ex.Message}");
                    if (verbose)
                    {
                        Console.WriteLine($"   Full Stack :\n{ex.InnerException.Message}\n{ex.StackTrace}");
                    }
                }
            }
        }

        private static void BootStartRegister()
        {
          
        }

      
    }
}
