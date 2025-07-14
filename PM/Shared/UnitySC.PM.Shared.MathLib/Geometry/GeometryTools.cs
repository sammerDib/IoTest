using System;
using System.Drawing;

namespace UnitySC.PM.Shared.MathLib.Geometry
{
    public static class GeometryTools
    {
        /// <summary>
        /// Compute the distance between 2 points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double ComputeDistance(PointF p1, PointF p2)
        {
            return ComputeDistance(p1.X, p1.Y, p2.X, p2.Y);
        }

        /// <summary>
        /// Compute the distance between 2 points
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double ComputeDistance(double x1, double y1, double x2, double y2)
        {
            double d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
            return Math.Sqrt(d2);
        }

        /// <summary>
        ///  Compute the average between 2 double
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double ComputeAverage(double a, double b)
        {
            return (a + b) / 2.0;
        }

        /// <summary>
        ///  Compute the average between 2 points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static PointF ComputeAverage(PointF pt1, PointF pt2)
        {
            float x = (float)ComputeAverage(pt1.X, pt2.X);
            float y = (float)ComputeAverage(pt1.Y, pt2.Y);
            return new PointF(x, y);
        }

        /// <summary>
        /// Compute the distance between a point and a segment
        /// </summary>
        /// <param name="ps"> Start of segment</param>
        /// <param name="pe"> End of segment</param>
        /// <param name="p"> Distant point</param>
        /// <returns></returns>
        public static double ComputeDistanceToSegment(PointF ps, PointF pe, PointF p)
        {
            if (ps.X == pe.X && ps.Y == pe.Y)
                return ComputeDistance(ps, p);

            float sx = pe.X - ps.X;
            float sy = pe.Y - ps.Y;

            float ux = p.X - ps.X;
            float uy = p.Y - ps.Y;

            float dp = sx * ux + sy * uy;
            if (dp < 0) return ComputeDistance(ps, p);

            float sn2 = sx * sx + sy * sy;
            if (dp > sn2) return ComputeDistance(pe, p);

            double ah2 = dp * dp / sn2;
            float un2 = ux * ux + uy * uy;
            return Math.Sqrt(un2 - ah2);
        }

        /// <summary>
        /// Draw a Cross
        /// </summary>
        /// <param name="gGraphic"></param>
        /// <param name="point"></param>
        /// <param name="penInter"></param>
        public static void DrawCross(Graphics gGraphic, PointF point, Pen penInter)
        {
            gGraphic.DrawLine(penInter, point.X - 6, point.Y, point.X + 6, point.Y);
            gGraphic.DrawLine(penInter, point.X, point.Y - 6, point.X, point.Y + 6);
        }
    }
}