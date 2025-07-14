using System;

using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Data
{
    public static class MathUtils
    {
        public static bool IsInsideCircle(PointUnits point, PointUnits center, Length radius)
        {
            var isInside = Math.Sqrt(Math.Pow(point.X.Millimeters - center.X.Millimeters, 2) + Math.Pow(point.Y.Millimeters - center.Y.Millimeters, 2) )<= radius.Millimeters;
            return isInside;
        }

        public static bool IsInsideRectangle(PointUnits point, PointUnits center, Length height, Length width)
        {
            var minX = center.X - width / 2;
            var maxX = center.X + width / 2;
            var minY = center.Y - height / 2;
            var maxY = center.Y + height / 2;

            var isInside = (point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY);
            return isInside;
        }

        /// <summary>
        /// Give position of a point from line
        /// </summary>
        /// <param name="p1"> First point on line</param>
        /// <param name="p2"> Second point on line</param>
        /// <param name="point"> Point</param>
        /// <returns>
        /// Si > 0, alors le point est à gauche de la droite
        /// Si = 0, alors le point est sur la droite
        /// Si < 0, alors le point est à droite de la droite
        /// </returns>
        public static double PositionFromLine(PointUnits p1, PointUnits p2, PointUnits point)
        {
            var d = (p2.X.Millimeters - p1.X.Millimeters) * (point.Y.Millimeters - p1.Y.Millimeters) - (p2.Y.Millimeters - p1.Y.Millimeters) * (point.X.Millimeters - p1.X.Millimeters);
            return d;
        }
    }
}
