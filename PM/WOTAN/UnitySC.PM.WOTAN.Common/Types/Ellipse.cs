using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CV = Emgu.CV;

namespace UnitySC.PM.WOTAN.Common
{
    public class Ellipse
    {
        private double _centerX, _centerY;
        private double _halfAxisX, _halfAxisY;
        private double _angle;

        public Ellipse(CV.Structure.RotatedRect cvEllipse)
        {
            _centerX = cvEllipse.Center.X;
            _centerY = cvEllipse.Center.Y;
            _halfAxisX = cvEllipse.Size.Width / 2;
            _halfAxisY = cvEllipse.Size.Height / 2;
            _angle = cvEllipse.Angle;
        }

        public Ellipse(double centerX, double centerY, double halfAxisX, double halfAxisY, double angle)
        {
            _centerX = centerX;
            _centerY = centerY;
            _halfAxisX = halfAxisX;
            _halfAxisY = halfAxisY;
            _angle = angle;
        }

        public Ellipse(PointF center, SizeF size, float angle)
        {
            _centerX = center.X;
            _centerY = center.Y;
            _halfAxisX = size.Width;
            _halfAxisY = size.Height;
            _angle = angle;
        }

        public double CenterX { get => _centerX; set => _centerX = value; }
        public double CenterY { get => _centerY; set => _centerY = value; }
        public double HalfAxisX { get => _halfAxisX; set => _halfAxisX = value; }
        public double HalfAxisY { get => _halfAxisY; set => _halfAxisY = value; }
        public double AverageRadius { get => (_halfAxisX + _halfAxisY) / 2; }
        public double AngleDeg { get => _angle; set => _angle = value; }
        public double AngleRad { get => _angle / 360 * (2 * Math.PI); set => _angle = value / (2 * Math.PI) * 360; }

        public double cosEllipse { get => Math.Cos(AngleRad); }
        public double sinEllipse { get => Math.Sin(AngleRad); }

        public PointF Center { get => new PointF((float)_centerX, (float)_centerY); }
        public Point CenterInt { get => new Point((int)_centerX, (int)_centerY); }

        public SizeF Size { get => new SizeF((float)_halfAxisX, (float)_halfAxisY); }
        public Size SizeInt { get => new Size((int)_halfAxisX, (int)_halfAxisY); }

        public Size SizeTwice{ get => new Size((int)_halfAxisX * 2, (int)_halfAxisY * 2); }

        public CV.Structure.RotatedRect AsCvEllipse()
        {
            return new CV.Structure.RotatedRect(
                center: new PointF((float)_centerX, (float)_centerY),
                size:   new SizeF((float)_halfAxisX * 2, (float)_halfAxisY * 2),
                angle: (float)_angle);
        }

        public Point CartesianPointFromAngle(double angle, AngleUnit unit)
        {
            switch (unit)
            {
                case AngleUnit.Deg:
                    angle *= 2 * Math.PI / 360;
                    break;
                case AngleUnit.Rad:
                    break;
                default:
                    throw new NotImplementedException();
            }

            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);
            return new Point(
                x: (int)(_centerX + _halfAxisX * cosAngle * cosEllipse - _halfAxisY * sinAngle * sinEllipse),
                y: (int)(_centerY + _halfAxisX * cosAngle * sinEllipse + _halfAxisY * sinAngle * cosEllipse));
        }

    }
}
