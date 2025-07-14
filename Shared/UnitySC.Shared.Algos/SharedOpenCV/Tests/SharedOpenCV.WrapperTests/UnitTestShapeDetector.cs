//#define RTi_BATCH_TESTCD_SKHYNIX
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

#if RTi_BATCH_TESTCD_SKHYNIX
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
#endif

using UnitySCSharedAlgosOpenCVWrapper;

namespace SharedOpenCV.WrapperTests
{
    [TestClass]
    public class UnitTestShapeDetector
    {
        private TestContext _testContextInstance;

        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
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
            int Depth = 8 ; //24 ?
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

        private void Save_DrawROIAndCirclesOnImage(string filePath, ImageData image, Circle[] circles, RegionOfInterest roi)
        {
            Bitmap bitmap = createBitmapFromImageData(image);
            Graphics graphics = Graphics.FromImage(bitmap);
            DrawROIOnGraphics(graphics, new Rectangle(roi.X, roi.Y, (int)roi.Width, (int)roi.Height), Color.FromArgb(0,255,0));
            DrawCirclesOnGraphics(graphics, circles, Color.FromArgb(255, 0, 0));

            bitmap.Save(filePath);

            bitmap.Dispose();
            graphics.Dispose();
        }

        private void SaveCDFromImage(int dbgDieIdx,  int dbgPointIdx, int dbgLightIdx , int dbgIdx, string inpath, string outpath, int roiWidth_px, int roiHeight_px, CircleFinderParams param)
        {
            var imgone = Helpers.CreateImageDataFromFile($"{inpath}Die{dbgDieIdx}\\Pt{dbgPointIdx}-L{dbgLightIdx}-{dbgIdx:000}.png");
            var roione = new RegionOfInterest()
            {
                X = imgone.Width / 2 - roiWidth_px / 2,
                Y = imgone.Height / 2 - roiHeight_px / 2,
                Width = roiWidth_px,
                Height = roiHeight_px
            };
            var circlesResultOne = ShapeDetector.CircleDetect(imgone, param, roione);
            Save_DrawROIAndCirclesOnImage(outpath + $"CD_Control_{dbgDieIdx}-{dbgPointIdx}-{dbgLightIdx}-{dbgIdx}.png", imgone, circlesResultOne.Circles, roione);
            SaveToFile(outpath + $"Preproc_{dbgDieIdx}-{dbgPointIdx}-{dbgLightIdx}-{dbgIdx}.png", circlesResultOne.PreprocessedImage);
        }

        [TestMethod]
        public void Batch_OldMethod_CD_CSV()
        {
            string outpath = @"C:\Work\Data\ANA-CD\Out\";
            string inpath = @"C:\Work\Data\ANA-CD\In\";

            double pixelSize = 0.21802;
            int roiWidth_px = 230;
            int roiHeight_px = 194;

            double approximateDiameter_px = 5.5 / pixelSize;
             double detectionTolerance_px = 3.0 / pixelSize;
             double distBetweenCircles = approximateDiameter_px;

             int cannyThreshold=300;
             bool useScharrAlgorithm=true;
             bool useMorphologicialOperations=true;

            int expectedCircleNb = 1;
            var param = new CircleFinderParams(approximateDiameter_px, approximateDiameter_px, detectionTolerance_px, cannyThreshold, useScharrAlgorithm, useMorphologicialOperations);

            if (false)
            {
                // to output imag that we want details one
                int dbgDieIdx = 2;
                int dbgPointIdx = 1;
                int dbgLightIdx = 2;
                int dbgIdx = 7;

                SaveCDFromImage(dbgDieIdx,dbgPointIdx,dbgLightIdx,dbgIdx,inpath, outpath, roiWidth_px, roiHeight_px, param);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"### Canny = {cannyThreshold}; PixelSize = {pixelSize}; Roi = [{roiWidth_px},{roiHeight_px}]");
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
                            var roi = new RegionOfInterest()
                            {
                                X = img.Width / 2 - roiWidth_px / 2,
                                Y = img.Height / 2 - roiHeight_px / 2,
                                Width = roiWidth_px,
                                Height = roiHeight_px
                            };

                            TestContext.WriteLine($"{folderDie}\\{fileName}");

                            var circlesResult = ShapeDetector.CircleDetect(img, param, roi);
                           
                            Point2d expectedCenter = new Point2d(img.Width / 2, img.Height / 2);

                            Assert.IsNotNull(circlesResult);
                            Assert.IsTrue(circlesResult.Circles.Length > 0);
                            if ((circlesResult.Circles.Length > expectedCircleNb) && false)
                            {
                                TestContext.WriteLine($"===> More Circle found {circlesResult.Circles.Length}");
                                SaveCDFromImage(dieIdx, siteId, lightIdx, nCount, inpath, outpath, roiWidth_px, roiHeight_px, param);
                            }

                           // sb.AppendLine($"Die; Light; SiteId; Idx; " +
                           //     $"Diameter (um); Diameters (px); " +
                           //     $"Center X in image (px); Center Y in image (px); dx; dy;");
                            sb.AppendLine($"{dieIdx};{lightIdx};{siteId};{nCount};" +
                                $"{pixelSize * circlesResult.Circles[0].Diameter};{circlesResult.Circles[0].Diameter};" +
                                $"{circlesResult.Circles[0].Center.X};{circlesResult.Circles[0].Center.Y};{expectedCenter.X - circlesResult.Circles[0].Center.X};{expectedCenter.Y - circlesResult.Circles[0].Center.Y};");

                            //Assert.AreEqual(expectedCircleNb, circlesResult.Circles.Length);
                            //for (int i = 0; i < circlesResult.Circles.Length; i++)
                            for (int i = 0; i < expectedCircleNb; i++)
                            {
                                Assert.AreEqual(expectedCenter.X, circlesResult.Circles[i].Center.X, 10.0);
                                Assert.AreEqual(expectedCenter.Y, circlesResult.Circles[i].Center.Y, 10.0);
                                Assert.AreEqual(approximateDiameter_px, circlesResult.Circles[i].Diameter, detectionTolerance_px);
                            }
                        }
                    }
                }
            }

            File.WriteAllText(outpath + $"BatchOldMethod.csv", sb.ToString());

        }
#endif

        [TestMethod]
        public void Ellipse_detector_correctly_detects_ellipse()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("1_centered_ellipse.png"));
            Point2d expectedCenter = new Point2d(img.Width / 2, img.Height / 2);
            int expectedEllipseNb = 1;
            Tuple<double, double> expectedAxes = new Tuple<double, double>(295, 385);
            int cannyThreshold = 100;
            int majorAxisTolerance = 10;
            int minorAxisTolerance = 10;
            var param = new EllipseFinderParams(expectedAxes, majorAxisTolerance, minorAxisTolerance, cannyThreshold);

            // When
            var ellipsesResult = ShapeDetector.EllipseDetect(img, param, null);

            // Then : We obtain correct values
            double expectedMajorAxis = expectedAxes.Item1 > expectedAxes.Item2 ? expectedAxes.Item1 : expectedAxes.Item2;
            double expectedMinorAxis = expectedAxes.Item1 <= expectedAxes.Item2 ? expectedAxes.Item1 : expectedAxes.Item2;

            Assert.AreEqual(expectedEllipseNb, ellipsesResult.Ellipses.Length);
            for (int i = 0; i < ellipsesResult.Ellipses.Length; i++)
            {
                var majorAxis = ellipsesResult.Ellipses[i].WidthAxis > ellipsesResult.Ellipses[i].HeightAxis ? ellipsesResult.Ellipses[i].WidthAxis : ellipsesResult.Ellipses[i].HeightAxis;
                var minorAxis = ellipsesResult.Ellipses[i].WidthAxis <= ellipsesResult.Ellipses[i].HeightAxis ? ellipsesResult.Ellipses[i].WidthAxis : ellipsesResult.Ellipses[i].HeightAxis;
                Assert.AreEqual(expectedCenter.X, ellipsesResult.Ellipses[i].Center.X, 11);
                Assert.AreEqual(expectedCenter.Y, ellipsesResult.Ellipses[i].Center.Y, 11);
                Assert.AreEqual(expectedMinorAxis, minorAxis, 5);
                Assert.AreEqual(expectedMajorAxis, majorAxis, 5);
            }
        }

        [TestMethod]
        public void Cicle_detector_correctly_detects_circle()
        {
            // Given
            var img = Helpers.CreateImageDataFromFile(Helpers.CreateNativeLibAlgoDataPath("1_centered_circle.jpg"));
            Point2d expectedCenter = new Point2d(img.Width / 2, img.Height / 2);
            int expectedCircleNb = 1;
            int expectedCircleDiameter = 170;
            double minDistBetweenTwoCircles = 10;
            int cannyThreshold = 100;
            int detectionTolerance = 10;
            var param = new CircleFinderParams(minDistBetweenTwoCircles, expectedCircleDiameter, detectionTolerance, cannyThreshold);

            // When
            var circlesResult = ShapeDetector.CircleDetect(img, param, null);

            // Then : We obtain correct values
            Assert.AreEqual(expectedCircleNb, circlesResult.Circles.Length);
            for (int i = 0; i < circlesResult.Circles.Length; i++)
            {
                Assert.AreEqual(expectedCenter.X, circlesResult.Circles[i].Center.X, 10);
                Assert.AreEqual(expectedCenter.Y, circlesResult.Circles[i].Center.Y, 10);
                Assert.AreEqual(expectedCircleDiameter, circlesResult.Circles[i].Diameter, 6);
            }
        }
    }
}
