using System;
using System.Drawing;

namespace AdcTools
{

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Cercle en coordonnées flottantes
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class CircleF
    {
        public double radius;
        public PointF center;

        public double Diameter
        {
            get { return radius * 2; }
            set { radius = value / 2; }
        }

        //=================================================================
        // 
        //=================================================================
        public CircleF()
        { }

        public CircleF(PointF center, double radius)
        {
            if (radius < 0)
                throw new ApplicationException("invalid circle radius: " + radius);

            this.center = center;
            this.radius = radius;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return center.ToString() + ", r=" + radius;
        }

        //=================================================================
        // 
        //=================================================================
        public PointF PointFromAngle(double angle)
        {
            PointF p = new PointF();
            p.X = center.X + (float)(radius * Math.Cos(angle));
            p.Y = center.Y + (float)(radius * Math.Sin(angle));
            return p;
        }

        //=================================================================
        // 
        //=================================================================
        public RectangleF SurroundingRectangle
        {
            get
            {
                float x = center.X - (float)radius;
                float y = center.Y - (float)radius;
                return new RectangleF(x, y, (float)Diameter, (float)Diameter);
            }
        }

        /// <summary>
        /// Retourne le rectangle englobant d'un arc de cercle
        /// </summary>
        public RectangleF GetSurroundingRectangle(double startAngle, double endAngle)
        {
            RectangleF box = new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);

            PointF p = PointFromAngle(startAngle);
            box.Union(p);
            p = PointFromAngle(endAngle);
            box.Union(p);

            const double π = Math.PI;
            for (double angle = -2 * π; angle < 2 * π; angle += π / 2)
            {
                if (startAngle < angle && angle < endAngle)
                {
                    p = PointFromAngle(angle);
                    box.Union(p);
                }
            }

            return box;
        }

    }
}
