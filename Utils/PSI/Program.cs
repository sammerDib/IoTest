using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using CommandLine;

using UnitySC.Shared.Data.FormatFile;

using UnitySCSharedAlgosOpenCVWrapper;

namespace QuickStart
{
    internal class Program
    {
        public class Options
        {
            [Value(0, MetaName = "stepNumber", Required = true, HelpText = "Nombre de steps utilisé pour acquérir les images d'interférométries.\n" +
                                                                           "/!\\ Doit être compris entre 3 et 7.")]
            public int StepNumber { get; set; }

            [Value(1, MetaName = "wavelenght", Required = true, HelpText = "Longueur d'onde utilisé pour acquérir les images d'interférométries.")]
            public int Wavelenght { get; set; }

            [Value(2, MetaName = "inputDirectoryPath", Required = true, HelpText = "Chemin vers le répertoire contenant les images d'interférométries.\n" +
                                                                              "/!\\ Les images doivent toutes être dans le même dossier (pas de sous dossiers) et ordonnées alphabétiquement pour que les images d'un même step se suivent.")]
            public string InputDirectoryPath { get; set; }

            [Value(3, MetaName = "outputDirectoryPath", Required = true, HelpText = "Chemin vers le répertoire de sortie de l'image topographique.")]
            public string OutputDirectoryPath { get; set; }

            [Value(4, MetaName = "pixelSizeMillimeters", Required = false, Default = 1, HelpText = "Nombre de millimètres que représente un pixel.")]
            public double PixelSizeMillimeters { get; set; }

            [Option('w', "wrapping", Required = false, Default = "Hariharan", HelpText = "Permet de spécifier l'algorithme à utiliser pour déduire la carte de phase à partir des images d'interférométries:\n" +
                                                                                         " - Hariharan: algorithme Hariharan\n" +
                                                                                         " - FFT: utilise la tranformée de fourier pour supprimer les franges résiduelles")]
            public string Wrapping { get; set; }

            [Option('u', "unwrapping", Required = false, Default = "Goldstein", HelpText = "Permet de spécifier l'algorithme de dépliage à utiliser:\n" +
                                                                                           " - Goldstein: algorithme Goldstein\n" +
                                                                                           " - HaiLei: algorithme Hai Lei")]
            public string Unwrapping { get; set; }
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
        }

        private static void RunOptions(Options opts)
        {
            Console.WriteLine($"Current Arguments: -stepNumber {opts.StepNumber}");
            Console.WriteLine($"Current Arguments: -wavelenght {opts.Wavelenght}");
            Console.WriteLine($"Current Arguments: -inputDirectoryPath {opts.InputDirectoryPath}");
            Console.WriteLine($"Current Arguments: -outputDirectoryPath {opts.OutputDirectoryPath}");
            Console.WriteLine($"Current Arguments: -pixelSizeMillimeters {opts.PixelSizeMillimeters}");
            Console.WriteLine($"Current Arguments: -wrapping {opts.Wrapping}");
            Console.WriteLine($"Current Arguments: -unwrapping {opts.Unwrapping}");

            if (opts.InputDirectoryPath != null && opts.OutputDirectoryPath != null)
            {
                if (!Directory.Exists(opts.InputDirectoryPath))
                {
                    Console.WriteLine($"Input directory path invalid. The directory specified does not exist: {opts.InputDirectoryPath}");
                    return;
                }

                if (!Directory.Exists(opts.OutputDirectoryPath))
                {
                    Directory.CreateDirectory(opts.OutputDirectoryPath);
                }

                List<string> imagesPath = GetImagesPath(opts.InputDirectoryPath);
                List<ImageData> imagesData = new List<ImageData>();
                foreach (string imgPath in imagesPath)
                {
                    imagesData.Add(LoadImageDataFromFile(imgPath));
                }
                UnwrapMode unwrap = opts.Unwrapping == "HaiLei" ? UnwrapMode.HistogramReliabilityPath : UnwrapMode.Goldstein;
                bool residualFringesRemoving = opts.Wrapping == "FFT" ? true : false;

                PSIParams psiParams = new PSIParams(opts.Wavelenght, opts.StepNumber, residualFringesRemoving, unwrap);
                TopographyPSI psiResult = PhaseShiftingInterferometry.ComputeTopography(imagesData.ToArray(), psiParams);

                SaveTopographyImageAs3DA(opts.OutputDirectoryPath, psiResult.TopographyMap, opts.PixelSizeMillimeters, opts.PixelSizeMillimeters);
            }
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (Error e in errs)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static List<string> GetImagesPath(string folderName)
        {
            return Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories).ToList();
        }

        private static ImageData LoadImageDataFromFile(string imgPath)
        {
            BitmapImage img = new BitmapImage(new Uri(imgPath));

            FormatConvertedBitmap formatedImg = new FormatConvertedBitmap();
            formatedImg.BeginInit();
            formatedImg.Source = img;
            formatedImg.DestinationFormat = System.Windows.Media.PixelFormats.Gray8;
            formatedImg.EndInit();

            BitmapSource imgSource = formatedImg;

            int bpp = 8;
            long size = imgSource.PixelHeight * imgSource.PixelWidth * (bpp / 5);
            byte[] data = new byte[size];
            imgSource.CopyPixels(data, imgSource.PixelWidth * (bpp / 8), 0);

            var imgByteArray = data;
            var imgHeight = imgSource.PixelHeight;
            var imgWidth = imgSource.PixelWidth;
            var imgDepth = bpp;

            return new ImageData(imgByteArray, imgWidth, imgHeight, imgDepth);
        }

        private static void SaveTopographyImageAs3DA(string outputDirectoryPath, ImageData topographyImage, double pixelSizeXMillimeters, double pixelSizeYMillimeters)
        {
            var byteArrayCopied = new List<byte>();
            for (int i = 0; i < topographyImage.ByteArray.Length; i++)
            {
                byteArrayCopied.Add(topographyImage.ByteArray.ElementAt(i));
            }

            var floatArray = ConvertByteArrayToFloat(byteArrayCopied.ToArray());

            string filename = Path.Combine(outputDirectoryPath, $"topography_image.3da");

            var addData = new MatrixFloatFile.AdditionnalHeaderData(pixelSizeXMillimeters, pixelSizeYMillimeters, "mm", "mm", "μm");
            var headerdata = new MatrixFloatFile.HeaderData(topographyImage.Height, topographyImage.Width, addData);
            bool useCompression = true;
            using (var format3daFile = new MatrixFloatFile())
            {
                format3daFile.WriteInFile(filename, headerdata, floatArray, useCompression);
            }
        }

        private static float[] ConvertByteArrayToFloat(byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException("byteArray");

            if (byteArray.Length % 4 != 0)
                throw new ArgumentException("Byte array does not represent a sequence of floats");

            // create a float array and copy the bytes into it...
            var floatArray = new float[byteArray.Length / 4];
            Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
            return floatArray;
        }
    }
}
