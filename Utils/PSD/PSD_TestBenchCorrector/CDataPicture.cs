using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System;

using UnitySCSharedAlgosOpenCVWrapper;

namespace AppsTestBenchCorrector
{
    public class CDataPicture
    {
        public struct Point
        {
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X;
            public double Y;
        }

        public struct Circle
        {
            public Circle(Point center, double diameter)
            {
                Center = center;
                Diameter = diameter;
            }

            public Point Center;
            public double Diameter;
        }

        public struct BenchCorrectorInput
        {
            public string FilePath { get; set; }
            public int WaferSize { get; set; }
            public int PixelSize { get; set; }
            public string AngleValueSearch { get; set; }
        }

        public struct BenchCorrectorResult
        {
            public double XCorrection { get; set; }
            public double YCorrection { get; set; }
            public double ThetaCorrection { get; set; }
        }

        public BenchCorrectorInput Input;

        public BenchCorrectorResult Result;

        public Point NotchCenter;

        public Circle WaferCircle;

        public Image DisplayImage
        { get { return _displayPicture; } }

        private byte[] _data;
        private int _sizeX;
        private int _sizeY;
        private int _bpp;
        private Image _displayPicture;
        private ImageData _imageData;
        private Graphics _overlayGraphique;

        public CDataPicture()
        {
        }

        public bool LoadImage()
        {
            FileInfo FI = new FileInfo(Input.FilePath);
            if (FI.Exists)
            {
                Bitmap _picture = (Bitmap)Image.FromFile(Input.FilePath);

                // Todo: Find a faster way to convert Image to byte[]
                byte[] outImage = new byte[_picture.Width * _picture.Height];

                for (int i = 0; i < _picture.Width; i++)
                {
                    for (int j = 0; j < _picture.Height; j++)
                    {
                        Color _color = _picture.GetPixel(i, j);
                        outImage[_picture.Width * j + i] = _color.R;
                    }
                }
                _data = outImage;
                _bpp = Image.GetPixelFormatSize(_picture.PixelFormat);
                _displayPicture = Image.FromFile(Input.FilePath);
                _sizeX = _displayPicture.Width;
                _sizeY = _displayPicture.Height;
                _imageData = new ImageData(_data, _sizeX, _sizeY, _bpp);

                return true;
            }
            else { MessageBox.Show("Wrong picture file"); return false; }
        }

        private NotchLocation ComputeNotchLocation(string angle)
        {
            switch (angle)
            {
                case "Bottom":
                    return NotchLocation.Bottom;

                case "Right":
                    return NotchLocation.Right;

                case "Left":
                    return NotchLocation.Left;

                case "Top":
                    return NotchLocation.Top;

                default:
                    return NotchLocation.Bottom;
            }
        }

        private Point ComputeTheoricalNotchCenter(Circle wafer, NotchLocation notchLocation)
        {
            var waferDiameter = wafer.Diameter;
            var theoricalNotchPosX = wafer.Center.X;
            var theoricalNotchPosY = wafer.Center.Y;

            switch (notchLocation)
            {
                case NotchLocation.Left:
                    theoricalNotchPosX = wafer.Center.X - (waferDiameter / 2);
                    break;

                case NotchLocation.Right:
                    theoricalNotchPosX = wafer.Center.X + (waferDiameter / 2);
                    break;

                case NotchLocation.Top:
                    theoricalNotchPosY = wafer.Center.Y - (waferDiameter / 2);
                    break;

                case NotchLocation.Bottom:
                    theoricalNotchPosY = wafer.Center.Y + (waferDiameter / 2);
                    break;
            }

            return new Point(theoricalNotchPosX, theoricalNotchPosY);
        }

        public void CorrectorXYTheta()
        {
            var waferImg = _imageData;

            float waferDiameterInMicrometer = Input.WaferSize * 1000;
            float toleranceInMicrometer = 10 * 1000; //default value: 10 millimeter
            float pixelSizeInMicrometer = Input.PixelSize;
            float waferDiameterInPixel = waferDiameterInMicrometer / pixelSizeInMicrometer;
            float detectionToleranceInPixel = toleranceInMicrometer / pixelSizeInMicrometer;
            int cannyThreeshold = 100; //default value
            var waferCircle = WaferDetector.DetectWaferCircle(waferImg, waferDiameterInPixel, detectionToleranceInPixel, cannyThreeshold);

            WaferCircle = new Circle(new Point(waferCircle.Center.X, waferCircle.Center.Y), waferCircle.Diameter);

            var notchLocation = ComputeNotchLocation(Input.AngleValueSearch);
            // // CODE HAS CHANGED -- from 1.0.3.7 and in  1.0.5.9 
            //float gamma = (float)0.5; //default value
            //int roiMaxSize = Math.Max(_sizeX, _sizeY) / 8; //default value
            //var notchCenter = NotchDetector.DetectNotchCenter(waferImg, waferCircle, notchLocation, gamma, roiMaxSize);
            // return some dummy results
            var notchCenter = new Point2d(0.5 *(double)_sizeX, 0.5 * (double)_sizeY);

            NotchCenter = new Point(notchCenter.X, notchCenter.Y);

            var theoricalWaferCenter = new Point(_sizeX / 2, _sizeY / 2);
            var shiftX = theoricalWaferCenter.X - waferCircle.Center.X;
            var shiftY = theoricalWaferCenter.Y - waferCircle.Center.Y;

            var theoricalNotchCenter = ComputeTheoricalNotchCenter(WaferCircle, notchLocation);
            var deltaX = theoricalNotchCenter.X - notchCenter.X;
            var deltaY = theoricalNotchCenter.Y - notchCenter.Y;
            var dist = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
            var radius = waferCircle.Diameter / 2;
            var theta = 2 * Math.Atan2((dist / 2), radius);

            // Translation and rotation to apply to center wafer in the middle of the image and straighten the notch
            Result.XCorrection = shiftX * pixelSizeInMicrometer;
            Result.YCorrection = shiftY * pixelSizeInMicrometer;
            Result.ThetaCorrection = theta;
        }

        public void CreateGraphics()
        {
            if (_displayPicture != null)
            {
                Bitmap orig = (Bitmap)_displayPicture;
                Bitmap rgb = new Bitmap(orig.Width, orig.Height, PixelFormat.Format32bppPArgb);

                _overlayGraphique = Graphics.FromImage(rgb);

                _overlayGraphique.DrawImage(orig, new Rectangle(0, 0, rgb.Width, rgb.Height));

                _displayPicture.Dispose();
                _displayPicture = rgb;
            }
            else { MessageBox.Show("No image loaded"); }
        }

        public void DrawColorCross(Point position, Color color)
        {
            Pen pen = new Pen(color, 1);
            _overlayGraphique.DrawLine(pen, (float)position.X - 10, (float)position.Y, (float)position.X + 10, (float)position.Y);
            _overlayGraphique.DrawLine(pen, (float)position.X, (float)position.Y - 10, (float)position.X, (float)position.Y + 10);
        }

        public void DrawColorEncoumpassingCircle(Circle circle, Color color)
        {
            float radius = (float)circle.Diameter / 2;
            Point upperLeftCorner = new Point(circle.Center.X - radius, circle.Center.Y - radius);

            Pen pen = new Pen(color, 1);
            _overlayGraphique.DrawEllipse(pen, (float)upperLeftCorner.X, (float)upperLeftCorner.Y, (float)circle.Diameter, (float)circle.Diameter);
        }
    }
}
