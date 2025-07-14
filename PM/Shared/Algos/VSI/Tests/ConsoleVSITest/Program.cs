using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Data.Extensions;
using UnitySC.Shared.Data.FormatFile;

using UnitySCPMSharedAlgosVSIWrapper;

namespace ConsoleVSITest
{
    internal class Program
    {
        private static byte[] LoadPng(string imgPath, string extension, ref int width, ref int height)
        {
            Stream imageStreamSource = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapDecoder decoder = null;
            switch (extension)
            {
                case ".png":
                    decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".bmp":
                    decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".tif":
                case ".tiff":
                    decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;
                default:
                    throw new ApplicationException("unkwown file extension: " + extension);
            }

            BitmapSource img = decoder.Frames[0];
            byte[] data = img.ConvertToByteArray();
            imageStreamSource.Close();

            return data;
        }

        static void Main(string[] args)
        {
            try
            {
                if (args == null || args.Length == 0)
                {
                    Console.WriteLine($"No arguments, please add entry folder containing VSI acquisition");

                    Console.WriteLine($"-------------------");
                    Console.WriteLine($"Press Enter to exit");
                    Console.ReadLine();

                    return;
                }


                if (args.Length == 0 || !Directory.Exists(args[0]))
                {
                    Console.WriteLine($"ERROR : Directory <{args[0]}> does not exists, cannot execute vsi algorithm");

                    Console.WriteLine($"-------------------");
                    Console.WriteLine($"Press Enter to exit");
                    Console.ReadLine();

                    return;
                }


                string dpath = args[0];

                Console.WriteLine("Start VSI test from :");

                List<KeyValuePair<string, double>> time_msMes = new List<KeyValuePair<string, double>>();
                Stopwatch sw = Stopwatch.StartNew();

                List<byte[]> imageStackList = new List<byte[]>();
                //DirectoryInfo d = new DirectoryInfo("C:\\Users\\pierre.davant\\Desktop\\Images\\VSI\\50x INT VSI 88nm 10nm\\Multi\\1_0\\RawImages");
                //DirectoryInfo d = new DirectoryInfo("C:\\Users\\pierre.davant\\Desktop\\Images\\VSI\\Samsung CuProtrusion_LotRawImages_12_6_2023_9h16min15sec\\Profilometry\\Multi\\test\\RawImages");
                //DirectoryInfo d = new DirectoryInfo("C:\\Users\\pierre.davant\\Desktop\\Images\\VSI\\VSIFlow\\20230627T151758");
                //DirectoryInfo d = new DirectoryInfo("C:\\Users\\pierre.davant\\Desktop\\Images\\VSI\\Cale\\20230628T144049");
                //DirectoryInfo d = new DirectoryInfo(@"C:\Analyse_Test\VSI-LAND2-Light5\VSI-LAND2");
                //DirectoryInfo d = new DirectoryInfo(@"C:\Analyse_Test\20230627T151758"); 

                DirectoryInfo d = new DirectoryInfo(dpath);
                Console.WriteLine($"{d.FullName}");
                FileInfo[] filePaths = d.GetFiles().OrderBy(file => Regex.Replace(file.Name, @"\d+", match => match.Value.PadLeft(4, '0'))).ToArray();

                int width = 0;
                int height = 0;


                time_msMes.Add(new KeyValuePair<string, double>("load filenames", sw.ElapsedMilliseconds));
                Console.WriteLine($"found {filePaths.Length} files in {time_msMes.Last().Value:0.00} ms");
                sw.Restart();

                foreach (var filePath in filePaths)
                {
                    if (filePath.Extension != ".png")
                        continue;
                    byte[] byteArray = LoadPng(filePath.FullName, filePath.Extension, ref width, ref height);
                    imageStackList.Add(byteArray);
                }

                time_msMes.Add(new KeyValuePair<string, double>("load PNG from filenames", sw.ElapsedMilliseconds));
                Console.WriteLine($"found {imageStackList.Count} PNG in {time_msMes.Last().Value:0.00} ms");
                sw.Restart();
                byte[][] imageStack = imageStackList.ToArray();

                time_msMes.Add(new KeyValuePair<string, double>("to images stacks", sw.ElapsedMilliseconds));
                Console.WriteLine($"to stack array in {time_msMes.Last().Value:0.00} ms");
                sw.Restart();

                //TO DO add rule step and/or wavelength to arguments, pixelsize ?

                double ruleStep = 50.0e-9; // land2
                double lambdaCenter = 630e-9;     // wavelength [m]

                //double ruleStep = 10.0e-9; // refcam1758
                //double lambdaCenter = 633e-9;

                double fwhmLambda = 124e-09;    // spectral bandwidth [m]
                double noiseLevel = 0.5;          // noise [LSB]

                double maskThreshold = 0.6;

                VSIOutput vsiOutput = VSI.ComputeTopography(imageStack, width, height, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);

                time_msMes.Add(new KeyValuePair<string, double>("VSI.ComputeTopography", sw.ElapsedMilliseconds));
                Console.WriteLine($"VSI Algo done in {time_msMes.Last().Value:0.00} ms -- res == {vsiOutput.ResultArray != null}");
                sw.Restart();

                float[] result = vsiOutput.ResultArray;

                if (result != null)
                {
                    bool useCompression = false;

                    var addData = new MatrixFloatFile.AdditionnalHeaderData(2.09863, 2.09863, "µm", "µm", "µm");   // x5-INT
                                                                                                                   //var addData = new MatrixFloatFile.AdditionnalHeaderData(2.123, 2.564, "µm", "µm", "µm"); //old
                    var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);
                    // direct write with compression
                    using (var format3daFile_W = new MatrixFloatFile())
                    {
                        format3daFile_W.WriteInFile("test.3da", headerdata, result, useCompression);
                        format3daFile_W.CloseFile();
                        // implict close file
                    }
                    time_msMes.Add(new KeyValuePair<string, double>("write 3da", sw.ElapsedMilliseconds));
                    Console.WriteLine($"write 3da done in {time_msMes.Last().Value:0.00} ms -- res == {vsiOutput.ResultArray != null}");
                    sw.Restart();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"### ERROR : {ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine($"-------------------");
                Console.WriteLine($"Press Enter to exit");
                Console.ReadLine();
            }
        }
    }
}
