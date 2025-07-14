using System;
using System.Drawing;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Matrices de conversion pixel <-> microns avec rotation.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class AffineMatrix : RectangularMatrix
    {
        private double _angle;
        protected double _sin = 0, _cos = 1;
        public double Angle // radians
        {
            get { return _angle; }
            set
            {
                if (_angle == value)
                    return;
                _angle = value;
                _sin = Math.Sin(_angle);
                _cos = Math.Cos(_angle);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override PointF pixelToMicron(Point pix)
        {
            // Translation / Scale
            //....................
            PointF p = base.pixelToMicron(pix);

            // Rotation
            //.........
            PointF mic = new PointF();
            mic.X = (float)(p.X * _cos + p.Y * _sin);
            mic.Y = (float)(-p.X * _sin + p.Y * _cos);

            return mic;
        }

        //=================================================================
        // 
        //=================================================================
        public override Point micronToPixel(PointF mic)
        {
            // Rotation
            //.........
            PointF p = new PointF();
            p.X = (float)(mic.X * _cos - mic.Y * _sin);
            p.Y = (float)(mic.X * _sin + mic.Y * _cos);

            // Translation / Scale
            //....................
            Point pix = base.micronToPixel(p);

            return pix;
        }

    }
}
