using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CV = Emgu.CV;

namespace UnitySC.PM.WOTAN.Common.Converters
{
    public class PolarConverter
    {
        private readonly double _pixelSize;  // µm/px
        private readonly int _angularSteps;
        private readonly double _innerMarginPx, _outerMarginPx;
        private readonly Ellipse _ellipse;

        public PolarConverter(
            double pixelSize, 
            int angularSteps,
            int innerMarginUm, 
            int outerMarginUm,
            Ellipse ellipse)
        {
            _pixelSize = pixelSize;
            _angularSteps = angularSteps;
            _ellipse = ellipse;
            _innerMarginPx = innerMarginUm / _pixelSize;
            _outerMarginPx = outerMarginUm / _pixelSize; 
        }

        public CV.Mat ConvertCartesianToPolarImage(CV.Mat cartesianImage)
        {
            int polarWidth = (int)Math.Round(_outerMarginPx + _innerMarginPx);

            CV.Mat polarImage = new CV.Mat(
                rows: _angularSteps,
                cols: polarWidth,
                type: cartesianImage.Depth, // Mat.Depth return the type of the elements in the matrix, not the depth of the matrix
                channels: cartesianImage.NumberOfChannels);

            CV.Matrix<float> cosSteps = new CV.Matrix<float>(
                rows: polarImage.Height,
                cols: 1);

            CV.Matrix<float> sinSteps = new CV.Matrix<float>(
                rows: polarImage.Height,
                cols: 1);

            double radCoef = 2 * Math.PI / polarImage.Height;

            for (int angle = 0; angle < polarImage.Height; angle++)
            {
                cosSteps[angle, 0] = (float)Math.Cos(angle * radCoef);
                sinSteps[angle, 0] = (float)Math.Sin(angle * radCoef);
            }

            CV.Matrix<float> distSteps = new CV.Matrix<float>(
                rows: 1,
                cols: polarImage.Width);

            int startDistance = (int)(_ellipse.AverageRadius - _innerMarginPx);
            for (int dist = 0; dist < polarImage.Width; dist++)
            {
                distSteps[0, dist] = dist + startDistance;
            }

            CV.Matrix<float> conversionMapX = new CV.Matrix<float>(
                rows: polarImage.Height,
                cols: polarImage.Width);

            CV.Matrix<float> conversionMapY = new CV.Matrix<float>(
                rows: polarImage.Height,
                cols: polarImage.Width);

            conversionMapX = cosSteps * distSteps + _ellipse.CenterX;
            conversionMapY = sinSteps * distSteps + _ellipse.CenterY;

            CV.CvInvoke.Remap(
                src: cartesianImage,
                dst: polarImage,
                map1: conversionMapX,
                map2: conversionMapY,
                interpolation: CV.CvEnum.Inter.Linear,
                borderMode: CV.CvEnum.BorderType.Constant,
                borderValue: new CV.Structure.MCvScalar(0));

            return polarImage;
        }
        public Point ConvertCartesianToPolarPoint(Point p)
        {
            double angle = Math.Atan2(
                y: p.Y - _ellipse.CenterY, 
                x: p.X - _ellipse.CenterX);    
            double distance = Math.Sqrt(Math.Pow(p.Y - _ellipse.CenterY, 2) + Math.Pow(p.X - _ellipse.CenterX, 2));
            
            while (angle < 0)
            {
                angle += 2 * Math.PI;
            } // we need a [0; 2.PI[ range for the next step

            angle *= _angularSteps / (2 * Math.PI);
            distance -= _ellipse.AverageRadius + _innerMarginPx;

            return new Point(x: (int)distance, y: (int)angle);
        }
        public Point ConvertPolarToCartesianPoint(Point p)
        {
            double distance = p.X;
            double angle = p.Y;

            distance += _ellipse.AverageRadius - _innerMarginPx;
            angle *= (2 * Math.PI) / _angularSteps;

            double x = distance * Math.Cos(angle) + _ellipse.CenterX;
            double y = distance * Math.Sin(angle) + _ellipse.CenterY;

            return new Point((int)x, (int)y);
        }

        public int ConvertAngleToAngularPosition(double angle, AngleUnit unit)
        {
            switch (unit)
            {
                case AngleUnit.Deg:
                    angle %= 360;
                    angle /= 360;
                    break;
                case AngleUnit.Rad:
                    angle %= 2 * Math.PI;
                    angle /= 2 * Math.PI;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return (int)Math.Round(angle * _angularSteps);
        }

        public double ConvertAngularPositionToAngle(int angularPosition, AngleUnit unit)
        {
            double angle = (double)angularPosition / _angularSteps;
            switch (unit)
            {
                case AngleUnit.Deg:
                    return (angle * 360) % 360;
                case AngleUnit.Rad:
                    return angle * 2 * Math.PI;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
