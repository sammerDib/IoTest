using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    public class ShapeDetector
    {
        static public Color LightSkyBlue = Color.FromArgb(250, 206, 135);
        static public Color Red = Color.FromArgb(0, 0, 255);

        public struct EllipseAreaFinderParams
        {
            public EllipseAreaFinderParams(Length length, Length width, Length lengthTolerance, Length widthTolerance, Length pixelSize, int cannyThreshold)
            {
                Length = length;
                Width = width;
                LengthTolerance = lengthTolerance;
                WidthTolerance = widthTolerance;
                PixelSize = pixelSize;
                CannyThreshold = cannyThreshold;
            }

            public Length Length;
            public Length Width;
            public Length LengthTolerance;
            public Length WidthTolerance;
            public Length PixelSize;
            public int CannyThreshold;
        }

        public struct EllipseFinderResult
        {
            public EllipseFinderResult(EllipseAxis axis, List<EllipseAxis> ellipses, ServiceImage preprocessedImage, ServiceImage imageWithEllipsesDrawn)
            {
                Axis = axis;
                Ellipses = ellipses;
                PreprocessedImage = preprocessedImage;
                ImageWithEllipses = imageWithEllipsesDrawn;
            }

            public EllipseAxis Axis;
            public List<EllipseAxis> Ellipses;
            public ServiceImage PreprocessedImage;
            public ServiceImage ImageWithEllipses;
        }

        public struct EllipseAxis
        {
            public EllipseAxis(Length length, Length width)
            {
                Length = length;
                Width = width;
            }

            public Length Length;
            public Length Width;
        }

        public struct CircleAreaFinderParams
        {
            public CircleAreaFinderParams(Length diameter, Length diameterTolerance, Length pixelSize, int cannyThreshold, bool useScharrAlgorithm, bool useMorphologicalOperations)
            {
                Diameter = diameter;
                DiameterTolerance = diameterTolerance;
                PixelSize = pixelSize;
                CannyThreshold = cannyThreshold;
                UseScharrAlgorithm = useScharrAlgorithm;
                UseMorphologicalOperations = useMorphologicalOperations;
            }

            public Length Diameter;
            public Length DiameterTolerance;
            public Length PixelSize;
            public int CannyThreshold;
            public bool UseScharrAlgorithm;
            public bool UseMorphologicalOperations;
        }

        public class CircleCentralFinderParams
        {
            public CircleCentralFinderParams(Length diameter, Length diameterTolerance, Length pixelSize)
            {
                Diameter = diameter;
                DiameterTolerance = diameterTolerance;
                PixelSize = pixelSize;
            }

            public Length Diameter;
            public Length DiameterTolerance;
            public Length PixelSize;

            // advanced "just-in case"
            public int? SeekerNumber;
            public double? SeekerWidth;
            public int? Mode;
            public int? KernelSize;
            public int? EdgeLocaliz;
            public double? SigAnalysisThreshold;
            public int? SigAnalysisPeakWindowSize;

        }


        public struct CircleFinderResult
        {
            public CircleFinderResult(Length diameter, List<Length> circles, ServiceImage preprocessedImage, ServiceImage imageWithCirclesDrawn)
            {
                Diameter = diameter;
                Circles = circles;
                PreprocessedImage = preprocessedImage;
                ImageWithCircles = imageWithCirclesDrawn;
            }

            public Length Diameter;
            public List<Length> Circles;
            public ServiceImage PreprocessedImage;
            public ServiceImage ImageWithCircles;
        }

        public virtual EllipseFinderResult ComputeEllipseDetection(ServiceImage img, EllipseAreaFinderParams parameters, Interface.Algo.CenteredRegionOfInterest roi)
        {
            var ellipseFinderParams = AlgorithmLibraryUtils.CreateEllipseFinderParams(parameters.Length, parameters.Width, parameters.LengthTolerance, parameters.WidthTolerance, parameters.PixelSize, parameters.CannyThreshold);
            var imgData = AlgorithmLibraryUtils.CreateImageData(img);

            var roiInPixels = AlgorithmLibraryUtils.CreateRegionOfInterest(img, roi, parameters.PixelSize);

            var result = UnitySCSharedAlgosOpenCVWrapper.ShapeDetector.EllipseDetect(imgData, ellipseFinderParams, roiInPixels);
            var ellipsesDetected = result.Ellipses;
            var axisMeasures = CalculateAverageAxesOfEllipses(ellipsesDetected, parameters.PixelSize);
            var ellipsesAxis = CalculateAxesOfEllipses(ellipsesDetected, parameters.PixelSize);
            var preprocessedImage = AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(result.PreprocessedImage);
            var preprocessedImageWithROI = DrawROIAndEllipsesOnImage(preprocessedImage, ellipsesDetected, roiInPixels);
            var imageWithEllipsesDrawn = DrawEllipsesOnImage(img, ellipsesDetected);

            return new EllipseFinderResult(axisMeasures, ellipsesAxis, preprocessedImageWithROI, imageWithEllipsesDrawn);
        }

        public virtual CircleFinderResult ComputeCircleAreaDetection(ServiceImage img, CircleAreaFinderParams parameters, Interface.Algo.CenteredRegionOfInterest roi)
        {
            var circleFinderParams = AlgorithmLibraryUtils.CreateCircleFinderParams(parameters.Diameter, parameters.DiameterTolerance, parameters.PixelSize, parameters.CannyThreshold, parameters.UseScharrAlgorithm, parameters.UseMorphologicalOperations);
            var imgData = AlgorithmLibraryUtils.CreateImageData(img);

            var roiInPixels = AlgorithmLibraryUtils.CreateRegionOfInterest(img, roi, parameters.PixelSize);

            var result = UnitySCSharedAlgosOpenCVWrapper.ShapeDetector.CircleDetect(imgData, circleFinderParams, roiInPixels);
            var circlesDetected = result.Circles;
            var averageDiameter = CalculateAverageDiameterOfCircles(circlesDetected, parameters.PixelSize);
            var diameters = CalculateAllDiameterOfCircles(circlesDetected, parameters.PixelSize);
            var preprocessedImage = AlgorithmLibraryUtils.ConvertToGrayscaleServiceImage(result.PreprocessedImage);
            var preprocessedImageWithROI = DrawROIAndCirclesOnImage(preprocessedImage, circlesDetected, roiInPixels);
            var imageWithCirclesDrawn = DrawCirclesOnImage(img, circlesDetected);

            return new CircleFinderResult(averageDiameter, diameters, preprocessedImageWithROI, imageWithCirclesDrawn);
        }

        public virtual CircleFinderResult ComputeCircleCentralDetection(ServiceImage img, CircleCentralFinderParams parameters, bool useReport, string reportFolder, uint drawflags)
        {
            var imgData = AlgorithmLibraryUtils.CreateImageData(img);

            // we assume circle center is located very nearby image center
            var inputs = AlgorithmLibraryUtils.CreateMetroCDCircleInput(img, parameters);
            MetroCDReport report =  null;
            if (useReport)
            {
                report = new MetroCDReport()
                {
                    ReportPathBase = Path.Combine(reportFolder, "CDCircle"),
                    Drawflag = drawflags,
                    IsReportOverlay = true,
                    IsReportCsv = true,
                };
            }
            var result = MetroCDCircle.DetectCircle(imgData, inputs, report);

            var circleresult = new CircleFinderResult();
            if (result != null && result.IsSuccess)
            {
                var circlesDetected = new Circle[] { new Circle(result.foundCenter, result.foundRadius * 2.0) };
                circleresult.ImageWithCircles = DrawCirclesOnImage(img, circlesDetected);

                var diameter = (parameters.PixelSize.Micrometers * circlesDetected[0].Diameter).Micrometers();
                circleresult.Diameter = diameter;
                circleresult.Circles = new List<Length>() { diameter };
            }
            else
            {
                circleresult.Diameter = 0.Micrometers();
                circleresult.Circles = new List<Length>();
            }

            return circleresult;
        }

        private EllipseAxis CalculateAverageAxesOfEllipses(Ellipse[] ellipses, Length pixelSize)
        {
            if (ellipses.Length == 0)
            {
                return new EllipseAxis(0.Micrometers(), 0.Micrometers());
            }

            double heightAxisAveraged = 0;
            double widthAxisAveraged = 0;
            foreach (var ellipse in ellipses)
            {
                widthAxisAveraged += ellipse.WidthAxis;
                heightAxisAveraged += ellipse.HeightAxis;
            }
            widthAxisAveraged /= ellipses.Length;
            heightAxisAveraged /= ellipses.Length;

            widthAxisAveraged *= pixelSize.Millimeters;
            heightAxisAveraged *= pixelSize.Millimeters;

            var length = widthAxisAveraged > heightAxisAveraged ? widthAxisAveraged : heightAxisAveraged;
            var width = widthAxisAveraged <= heightAxisAveraged ? widthAxisAveraged : heightAxisAveraged;

            //from Millimeters to Microns
            length *= 1000.0;
            width *= 1000.0;

            return new EllipseAxis(length.Micrometers(), width.Micrometers());
        }

        private List<EllipseAxis> CalculateAxesOfEllipses(Ellipse[] ellipses, Length pixelSize)
        {
            var ellipsesAxis = new List<EllipseAxis>();
            foreach (var ellipse in ellipses)
            {
                var widthAxis = ellipse.WidthAxis * pixelSize.Millimeters;
                var heightAxis = ellipse.HeightAxis * pixelSize.Millimeters;
                ellipsesAxis.Add(new EllipseAxis(heightAxis.Millimeters(), widthAxis.Millimeters()));
            }
            return ellipsesAxis;
        }

        private Length CalculateAverageDiameterOfCircles(Circle[] circles, Length pixelSize)
        {
            if (circles.Length == 0)
            {
                return 0.Micrometers();
            }

            double diameterAveraged = 0;
            foreach (var circle in circles)
            {
                diameterAveraged += circle.Diameter;
            }
            diameterAveraged /= circles.Length;
            diameterAveraged *= pixelSize.Millimeters;
            diameterAveraged *= 1000.0;  //from Millimeters to Micrometers
            return diameterAveraged.Micrometers();
        }

        private List<Length> CalculateAllDiameterOfCircles(Circle[] circles, Length pixelSize)
        {
            var diameters = new List<Length>();
            foreach (var circle in circles)
            {
                var diameter = circle.Diameter * pixelSize.Millimeters;
                diameter *= 1000.0;  //from Millimeters to Micrometers
                diameters.Add(diameter.Micrometers());
            }
            return diameters;
        }

        private ServiceImage DrawROIAndEllipsesOnImage(ServiceImage image, Ellipse[] ellipses, RegionOfInterest roi)
        {
            Bitmap bitmap = createBitmapFromServiceImage(image);
            Graphics graphics = Graphics.FromImage(bitmap);
            DrawROIOnGraphics(graphics, new Rectangle(roi.X, roi.Y, (int)roi.Width, (int)roi.Height), Red);
            DrawEllipsesOnGraphics(graphics, ellipses, Red);

            var img = createServiceImageFromBitmap(bitmap);

            bitmap.Dispose();
            graphics.Dispose();

            return img;
        }

        private ServiceImage DrawROIAndCirclesOnImage(ServiceImage image, Circle[] circles, RegionOfInterest roi)
        {
            Bitmap bitmap = createBitmapFromServiceImage(image);
            Graphics graphics = Graphics.FromImage(bitmap);
            DrawROIOnGraphics(graphics, new Rectangle(roi.X, roi.Y, (int)roi.Width, (int)roi.Height), Red);
            DrawCirclesOnGraphics(graphics, circles, Red);

            var img = createServiceImageFromBitmap(bitmap);

            bitmap.Dispose();
            graphics.Dispose();

            return img;
        }

        private ServiceImage DrawEllipsesOnImage(ServiceImage image, Ellipse[] ellipses)
        {
            Bitmap bitmap = createBitmapFromServiceImage(image);
            Graphics graphics = Graphics.FromImage(bitmap);
            DrawEllipsesOnGraphics(graphics, ellipses, LightSkyBlue);

            var img = createServiceImageFromBitmap(bitmap);

            bitmap.Dispose();
            graphics.Dispose();

            return img;
        }

        private ServiceImage DrawCirclesOnImage(ServiceImage image, Circle[] circles)
        {
            Bitmap bitmap = createBitmapFromServiceImage(image);
            Graphics graphics = Graphics.FromImage(bitmap);
            DrawCirclesOnGraphics(graphics, circles, LightSkyBlue);

            var img = createServiceImageFromBitmap(bitmap);

            bitmap.Dispose();
            graphics.Dispose();

            return img;
        }

        private Bitmap createBitmapFromServiceImage(ServiceImage image)
        {
            if(image == null || image.Data == null || (image.Type == ServiceImage.ImageType._3DA))
                return new Bitmap(10, 10, PixelFormat.Format24bppRgb);

            Bitmap dstBmp = new Bitmap(image.DataWidth, image.DataHeight, PixelFormat.Format24bppRgb);
            unsafe
            {
                fixed (byte* pDataSource = image.Data)
                {
                    //Create a BitmapData and Lock all pixels to be written 
                    BitmapData bmpData = dstBmp.LockBits(new Rectangle(0, 0, dstBmp.Width, dstBmp.Height), ImageLockMode.WriteOnly, dstBmp.PixelFormat);
                    // Get color components count
                    int cCount = System.Drawing.Bitmap.GetPixelFormatSize(dstBmp.PixelFormat) / 8; // here we assume depth == 24, we need to set Red, Green And Blue pixels
                    byte* pStartArray = pDataSource;

                    int width = image.DataWidth;
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

        private ServiceImage createServiceImageFromBitmap(Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var bitmapSource = System.Windows.Media.Imaging.BitmapSource.Create(bitmapData.Width, bitmapData.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution, System.Windows.Media.PixelFormats.Rgb24, null, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);
            var img = new ServiceImage();
            img.CreateFromBitmap(bitmapSource);
            return img;
        }

        private void DrawEllipsesOnGraphics(Graphics graphics, Ellipse[] ellipses, Color color)
        {
            Pen pen = new Pen(color, 1);

            foreach (var ellipse in ellipses)
            {
                //Rotate the graphics object the required amount around ellipse center before draw ellipse & rotate back to normal
                var matrix = new Matrix();

                float ellipseCenterX = (float)ellipse.Center.X;
                float ellipseCenterY = (float)ellipse.Center.Y;

                matrix.RotateAt((float)ellipse.Angle, new PointF(ellipseCenterX, ellipseCenterY));
                graphics.Transform = matrix;

                graphics.DrawEllipse(pen, (float)ellipse.Center.X - (float)ellipse.WidthAxis / 2, (float)ellipse.Center.Y - (float)ellipse.HeightAxis / 2, (float)ellipse.WidthAxis, (float)ellipse.HeightAxis);

                matrix.RotateAt(-(float)ellipse.Angle, new PointF(ellipseCenterX, ellipseCenterY));
                graphics.Transform = matrix;
            }
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
    }
}
