//#define RTi_BATCH_TESTCD_SKHYNIX
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;

using UnitySCSharedAlgosOpenCVWrapper;


#if RTi_BATCH_TESTCD_SKHYNIX
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
#endif

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestMetroCD
    {
        private TestContext _testContextInstance;

        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }

        [TestMethod]
        public void _00_Test_MetroCDFlags()
        {
            var dflag = MetroCDDrawFlag.DrawSeekers | MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawSkipDetection;

            Assert.IsTrue(MetroCDFlags.HasDrawFlag(dflag, MetroCDDrawFlag.DrawSeekers));
            Assert.IsTrue(MetroCDFlags.HasDrawFlag(dflag, MetroCDDrawFlag.DrawFit));
            Assert.IsTrue(MetroCDFlags.HasDrawFlag(dflag, MetroCDDrawFlag.DrawSkipDetection));
            Assert.IsFalse(MetroCDFlags.HasDrawFlag(dflag, MetroCDDrawFlag.DrawDetection));
            Assert.IsFalse(MetroCDFlags.HasDrawFlag(dflag, MetroCDDrawFlag.DrawCenterFit));

            uint uflg = (uint)dflag;

            Assert.IsTrue(MetroCDFlags.HasDrawFlag(uflg, MetroCDDrawFlag.DrawSeekers));
            Assert.IsTrue(MetroCDFlags.HasDrawFlag(uflg, MetroCDDrawFlag.DrawFit));
            Assert.IsTrue(MetroCDFlags.HasDrawFlag(uflg, MetroCDDrawFlag.DrawSkipDetection));
            Assert.IsFalse(MetroCDFlags.HasDrawFlag(uflg, MetroCDDrawFlag.DrawDetection));
            Assert.IsFalse(MetroCDFlags.HasDrawFlag(uflg, MetroCDDrawFlag.DrawCenterFit));

            dflag |= MetroCDDrawFlag.DrawDetection;
            Assert.IsTrue(MetroCDFlags.HasDrawFlag(dflag, MetroCDDrawFlag.DrawDetection), "enum flag has changed and should contain new flag");
            Assert.IsFalse(MetroCDFlags.HasDrawFlag(uflg, MetroCDDrawFlag.DrawDetection), "associated uint flag has NOT changed and should NOT contain new flag");
        }

        [TestMethod]
        public void _01_CircleSeeker_DefaultParams_NoReport()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("16_circles_of_60_pixels_in_diameter.tif"));
            var inputs = new MetroCDCircleInput(new Point2d(632.0, 508.0), 30.0, 5.0);
            double expecteddiameter_px = 58.9;

            // When
            var circlResult = MetroCDCircle.DetectCircle(img, inputs, null);

            Assert.IsNotNull(circlResult);
            Assert.IsTrue(circlResult.IsSuccess);

            Assert.AreEqual(expecteddiameter_px, circlResult.foundRadius * 2.0, 0.5);

        }

        [TestMethod]
        public void _01_CircleSeeker_DefaultParams_withReport()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("16_circles_of_60_pixels_in_diameter.tif"));
            var inputs = new MetroCDCircleInput(new Point2d(632.0, 508.0), 30.0, 5.0);
            double expecteddiameter_px = 58.9;

            string currentPath = Directory.GetCurrentDirectory() + "\\reports\\01-" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(currentPath);
            var report = new MetroCDReport()
            {
                ReportPathBase = currentPath + "TSvtest01",
                Drawflag = (uint)(MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawDetection | MetroCDDrawFlag.DrawSkipDetection | MetroCDDrawFlag.DrawSeekers),
                IsReportOverlay = true,
                IsReportCsv = true,
            };

            // When
            var circlResult = MetroCDCircle.DetectCircle(img, inputs, report);

            Assert.IsNotNull(circlResult);
            Assert.IsTrue(circlResult.IsSuccess);

            Assert.AreEqual(expecteddiameter_px, circlResult.foundRadius * 2.0, 0.5);

            Assert.IsTrue(File.Exists(report.ReportPathBase + "CircleSeekerOverlay.png"));

            var anglestep_dg = 11.25;
            for (var i = 0.0; i < 360.0; i += anglestep_dg)
            {
                Assert.IsTrue(File.Exists(report.ReportPathBase + $"CircleSeeker.{i}.csv"));
            }

            Directory.Delete(currentPath, true);

        }

        [TestMethod]
        public void _02_CircleSeeker_TSV_DefaultParams_ReportOverlay()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("4_circles_of_30_pixels_in_diameter.png"));
            var inputs = new MetroCDCircleInput(new Point2d(644.0, 516.0), 15.0, 8.0);
            double expectedRadius_px = 13.63;
            double precision = 0.01;

            string currentPath = Directory.GetCurrentDirectory() + "\\reports\\02-" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(currentPath);
            var report = new MetroCDReport()
            {
                ReportPathBase = currentPath + "TSvtest02",
                Drawflag = (uint)(MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawDetection | MetroCDDrawFlag.DrawSkipDetection),
                IsReportOverlay = true,
                IsReportCsv = false,
            };
            //default auto seeker prm for this radius n = 16, h = 5 
            // When
            var circlResult = MetroCDCircle.DetectCircle(img, inputs, report);

            Assert.IsNotNull(circlResult);
            Assert.IsTrue(circlResult.IsSuccess);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            Assert.IsTrue(File.Exists(report.ReportPathBase + "CircleSeekerOverlay.png"));

            inputs.center.Y = 99.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            inputs.center.Y = 308.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            inputs.center.Y = 725.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            Directory.Delete(currentPath, true);
        }

        [TestMethod]
        public void _03_CircleSeeker_TSV_SetAdvParams_ReportOverlay()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("4_circles_of_30_pixels_in_diameter.png"));
            var inputs = new MetroCDCircleInput(new Point2d(644.0, 516.0), 15.0, 8.0);
            inputs.seekerNumber = 6; // auto =-> 16
            inputs.seekerWidth = 3.0;// auto -> 5
            double expectedRadius_px = 13.63;
            double precision = 0.065;

            string currentPath = Directory.GetCurrentDirectory() + "\\reports\\03-" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(currentPath);
            var report = new MetroCDReport()
            {
                ReportPathBase = currentPath + "TSvtest03",
                Drawflag = (uint)(MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawDetection | MetroCDDrawFlag.DrawSkipDetection | MetroCDDrawFlag.DrawSeekers),
                IsReportOverlay = true,
                IsReportCsv = false,
            };

            // When
            var circlResult = MetroCDCircle.DetectCircle(img, inputs, report);

            Assert.IsNotNull(circlResult);
            Assert.IsTrue(circlResult.IsSuccess);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            Assert.IsTrue(File.Exists(report.ReportPathBase + "CircleSeekerOverlay.png"));

            inputs.center.Y = 99.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            inputs.center.Y = 308.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            inputs.center.Y = 725.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);

            Directory.Delete(currentPath, true);
        }

        [TestMethod]
        public void _04_CircleSeeker_TSV_SeekerPositionCenterInstable()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("4_circles_of_30_pixels_in_diameter.png"));
            var baseCenter = new Point2d(644.0, 516.0);
            var inputs = new MetroCDCircleInput(baseCenter, 15.0, 8.0);
            var expectedCenter = new Point2d(643.33, 515.93);

            double expectedRadius_px = 13.63;
            double precision = 0.15;

            string currentPath = Directory.GetCurrentDirectory() + "\\reports\\04-" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "\\";
            Directory.CreateDirectory(currentPath);
            var report = new MetroCDReport()
            {
                ReportPathBase = currentPath + "TSvtest04",
                Drawflag = (uint)(MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawDetection | MetroCDDrawFlag.DrawCenterFit | MetroCDDrawFlag.DrawSeekers),
                IsReportOverlay = true,
                IsReportCsv = false,
            };

            // When
            var circlResult = MetroCDCircle.DetectCircle(img, inputs, report);

            Assert.IsNotNull(circlResult);
            Assert.IsTrue(circlResult.IsSuccess);
            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);
            Assert.AreEqual(expectedCenter.X, circlResult.foundCenter.X, precision);
            Assert.AreEqual(expectedCenter.Y, circlResult.foundCenter.Y, precision);

            Assert.IsTrue(File.Exists(report.ReportPathBase + "CircleSeekerOverlay.png"));

            inputs.center.X = baseCenter.X + 2.5;
            inputs.center.Y = baseCenter.Y + 2.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);

            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);
            Assert.AreEqual(expectedCenter.X, circlResult.foundCenter.X, precision);
            Assert.AreEqual(expectedCenter.Y, circlResult.foundCenter.Y, precision);

            inputs.center.X = baseCenter.X - 2.0;
            inputs.center.Y = baseCenter.Y - 3.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);

            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);
            Assert.AreEqual(expectedCenter.X, circlResult.foundCenter.X, precision);
            Assert.AreEqual(expectedCenter.Y, circlResult.foundCenter.Y, precision);

            inputs.center.X = baseCenter.X - 5.0;
            inputs.center.Y = baseCenter.Y + 4.0;
            circlResult = MetroCDCircle.DetectCircle(img, inputs, report);

            Assert.AreEqual(expectedRadius_px, circlResult.foundRadius, precision);
            Assert.AreEqual(expectedCenter.X, circlResult.foundCenter.X, precision);
            Assert.AreEqual(expectedCenter.Y, circlResult.foundCenter.Y, precision);

            Directory.Delete(currentPath, true);
        }

#if RTi_BATCH_TESTCD_SKHYNIX
        public void SaveToFile(string filePath, ImageData imageData)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = null;
                string ext = Path.GetExtension(filePath).ToLower().ToLower();
                switch (ext)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;

                    case ".tif":
                    case ".tiff":
                        encoder = new TiffBitmapEncoder();
                        break;

                    case ".jpg":
                    case ".jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case ".bmp":
                    default:
                        encoder = new BmpBitmapEncoder();
                        break;
                }
                if (encoder != null)
                {
                    encoder.Frames.Add(BitmapFrame.Create(ConvertToWriteableBitmap(imageData)));
                    encoder.Save(fileStream);
                }
            }
        }

        private WriteableBitmap ConvertToWriteableBitmap(ImageData imageData)
        {
            int Depth = 8; //24 ?
            byte[] Data = imageData.ByteArray;
            // Création de la WriteableBitmap
            //...............................
            var format = System.Windows.Media.PixelFormats.Gray8;
            int sizeX = imageData.Width;
            int sizeY = imageData.Height;
            var writeableBitmap = new WriteableBitmap(sizeX, sizeY, 96, 96, format, null);

            // Copie des pixels
            //.................
            unsafe
            {
                int pitch = sizeX * (Depth / 8);
                int bufferSize = pitch * sizeY;

                fixed (byte* pSrc = &Data[0])
                    writeableBitmap.WritePixels(new Int32Rect(0, 0, sizeX, sizeY), new IntPtr(pSrc), bufferSize, pitch);
            }

            return writeableBitmap;
        }

        private Bitmap createBitmapFromImageData(ImageData image)
        {

            Bitmap dstBmp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            unsafe
            {
                fixed (byte* pDataSource = image.ByteArray)
                {
                    //Create a BitmapData and Lock all pixels to be written 
                    BitmapData bmpData = dstBmp.LockBits(new Rectangle(0, 0, dstBmp.Width, dstBmp.Height), ImageLockMode.WriteOnly, dstBmp.PixelFormat);
                    // Get color components count
                    int cCount = System.Drawing.Bitmap.GetPixelFormatSize(dstBmp.PixelFormat) / 8; // here we assume depth == 24, we need to set Red, Green And Blue pixels
                    byte* pStartArray = pDataSource;

                    int width = image.Width;
                    Parallel.For(0, dstBmp.Height, y =>
                    //for (int y = 0; y < dstBmp.Height; y++)
                    {
                        byte* pRow = (byte*)bmpData.Scan0 + (y * bmpData.Stride);
                        byte* pSrcRow = (byte*)(pStartArray + (y * width));
                        for (int x = 0; x < width; x++)
                        {
                            pRow[2] = pRow[1] = pRow[0] = *pSrcRow;
                            pRow += cCount;
                            pSrcRow++;
                        }
                    });  // Parallel.For

                    //Unlock the pixels
                    dstBmp.UnlockBits(bmpData);
                }
            }
            return dstBmp;
        }
        private void DrawCirclesOnGraphics(Graphics graphics, Circle[] circles, Color color)
        {
            Pen pen = new Pen(color, 1);

            foreach (var circle in circles)
            {
                graphics.DrawEllipse(pen, (float)circle.Center.X - (float)circle.Diameter / 2, (float)circle.Center.Y - (float)circle.Diameter / 2, (float)circle.Diameter, (float)circle.Diameter);
            }
        }

        private void DrawROIOnGraphics(Graphics graphics, Rectangle roi, Color color)
        {
            Pen pen = new Pen(color, 1);
            graphics.DrawRectangle(pen, roi);
        }

        private void Save_DrawCirclesOnImage(string filePath, ImageData image, Circle[] circles)
        {
            Bitmap bitmap = createBitmapFromImageData(image);
            Graphics graphics = Graphics.FromImage(bitmap);
            DrawCirclesOnGraphics(graphics, circles, Color.FromArgb(255, 0, 0));

            bitmap.Save(filePath);

            bitmap.Dispose();
            graphics.Dispose();
        }

        private void SaveCDFromImage(int dbgDieIdx, int dbgPointIdx, int dbgLightIdx, int dbgIdx, string inpath, string outpath, MetroCDCircleInput inputs, bool reportcsv = false, bool reportoverlay = false)
        {
            var imgone = Helpers.CreateImageDataFromFile($"{inpath}Die{dbgDieIdx}\\Pt{dbgPointIdx}-L{dbgLightIdx}-{dbgIdx:000}.png");

            string reportPath = $"{outpath}Die{dbgDieIdx}\\";
            Directory.CreateDirectory(reportPath);
            var report = new MetroCDReport()
            {
                ReportPathBase = reportPath + $"Pt{dbgPointIdx}-L{dbgLightIdx}-{dbgIdx:000}-",
                Drawflag = (uint)(MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawDetection | MetroCDDrawFlag.DrawCenterFit | MetroCDDrawFlag.DrawSkipDetection),
                IsReportOverlay = reportoverlay, 
                IsReportCsv = reportcsv,
            };

            var circelResult = MetroCDCircle.DetectCircle(imgone, inputs, report);

            var circles = new Circle[] { new Circle(circelResult.foundCenter, circelResult.foundRadius * 2.0) };
            Save_DrawCirclesOnImage(reportPath + $"CD_Control_{dbgDieIdx}-{dbgPointIdx}-{dbgLightIdx}-{dbgIdx}.png", imgone, circles);

        }

        [TestMethod]
        public void Batch_NewdMethod_CD_CSV()
        {
            string outpath = @"C:\Work\Data\ANA-CD\Out\";
            string inpath = @"C:\Work\Data\ANA-CD\In\";


            double pixelSize = 0.21802;

            double approximateDiameter_px = 5.5 / pixelSize;
            double detectionTolerance_px = 3.0 / pixelSize;
            //double approximateDiameter_px = 7.5 / pixelSize;
            //double detectionTolerance_px = 1.5 / pixelSize;
            double distBetweenCircles = approximateDiameter_px;

            // image center -  image size should be 1280 x 1024
            var baseCenter = new Point2d(1280.0 * 0.5, 1024.0 * 0.5);
            var inputs = new MetroCDCircleInput(baseCenter, approximateDiameter_px / 2.0, detectionTolerance_px);


            if (false)
            {
                // to output imag that we want details one
                int dbgDieIdx = 1;
                int dbgPointIdx = 2;
                int dbgLightIdx = 1;
                int dbgIdx = 5;

                SaveCDFromImage(dbgDieIdx, dbgPointIdx, dbgLightIdx, dbgIdx, inpath, outpath, inputs, true, true);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"### Auto Parameters; PixelSize = {pixelSize}; NO-Roi");
            sb.AppendLine($"Die; Light; SiteId; Idx; Diameter (um); Diameters (px); Center X in image (px); Center Y in image (px); dx; dy;");

            for (int dieIdx = 1; dieIdx <= 3; dieIdx++)
            {
                for (int lightIdx = 1; lightIdx <= 3; lightIdx++)
                {
                    for (int siteId = 1; siteId <= 3; siteId++)
                    {
                        var folderDie = $"Die{dieIdx}";
                        DirectoryInfo dir = new DirectoryInfo(inpath + "\\" + folderDie);

                        int nCount = 0;
                        FileInfo[] Files = dir.GetFiles($"Pt{siteId}-L{lightIdx}-*.png");
                        foreach (FileInfo file in Files)
                        {
                            ++nCount;

                            string fileName = file.Name;
                            var img = Helpers.CreateImageDataFromFile(file.FullName);

                            TestContext.WriteLine($"{folderDie}\\{fileName}");


                            /*  string reportPath = $"{outpath}Die{dieIdx}\\";
                              Directory.CreateDirectory(reportPath);
                              var report = new MetroCDReport()
                              {
                                  ReportPathBase = reportPath + $"Pt{ siteId }-L{ lightIdx }-{ nCount:000}-",
                                  Drawflag = (uint)(MetroCDDrawFlag.DrawFit | MetroCDDrawFlag.DrawDetection | MetroCDDrawFlag.DrawCenterFit | MetroCDDrawFlag.DrawSkipDetection | MetroCDDrawFlag.DrawSeekers),
                                  IsReportOverlay = true,
                                  IsReportCsv = false,
                              };*/
                            MetroCDReport report = null;



                            var circleResult = MetroCDCircle.DetectCircle(img, inputs, report);

                            Point2d expectedCenter = new Point2d(img.Width / 2, img.Height / 2);

                            Assert.IsNotNull(circleResult);
                            Assert.IsTrue(circleResult.IsSuccess);

                            if (true)
                            {
                                SaveCDFromImage(dieIdx, siteId, lightIdx, nCount, inpath, outpath, inputs);
                            }

                            // sb.AppendLine($"Die; Light; SiteId; Idx; " +
                            //     $"Diameter (um); Diameters (px); " +
                            //     $"Center X in image (px); Center Y in image (px); dx; dy;");
                            sb.AppendLine($"{dieIdx};{lightIdx};{siteId};{nCount};" +
                                $"{pixelSize * circleResult.foundRadius * 2.0};{circleResult.foundRadius * 2.0};" +
                                $"{circleResult.foundCenter.X};{circleResult.foundCenter.Y};{expectedCenter.X - circleResult.foundCenter.X};{expectedCenter.Y - circleResult.foundCenter.Y};");

                            Assert.AreEqual(expectedCenter.X, circleResult.foundCenter.X, 10.0);
                            Assert.AreEqual(expectedCenter.Y, circleResult.foundCenter.Y, 10.0);
                            Assert.AreEqual(approximateDiameter_px, circleResult.foundRadius * 2.0, detectionTolerance_px);
                        }
                    }
                }
            }

            File.WriteAllText(outpath + $"BatchNewdMethod.csv", sb.ToString());

        }
#endif

    }
}

