using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Tools
{
    public static class MathTools
    {
        public static Length LineLength(XYPosition linePointA, XYPosition linePointB)
        {
            return Math.Sqrt(Math.Pow(linePointA.X - linePointB.X, 2) + Math.Pow(linePointA.Y - linePointB.Y, 2)).Millimeters();
        }

        public static XYPosition OrthogonalProjectionOfPointOntoLine(XYPosition linePointA, XYPosition linePointB, XYPosition pointC)
        {
            double dx = linePointB.X - linePointA.X;
            double dy = linePointB.Y - linePointA.Y;
            double mag = Math.Sqrt(dx * dx + dy * dy);

            if (mag == 0)
            {
                return pointC;
            }

            dx /= mag;
            dy /= mag;

            double lambda = (dx * (pointC.X - linePointA.X)) + (dy * (pointC.Y - linePointA.Y));
            double x = (dx * lambda) + linePointA.X;
            double y = (dy * lambda) + linePointA.Y;

            return new XYPosition(new WaferReferential(), x, y);
        }

        public static Angle ComputeAngleFromTwoPositions(XYPosition posA, XYPosition posB)
        {
            if (posA == posB)
            {
                return new Angle(0, AngleUnit.Radian);
            }

            double dx = posB.X - posA.X;
            double dy = posB.Y - posA.Y;
            double angle = Math.Atan2(dy, dx);

            return new Angle(angle, AngleUnit.Radian);
        }

        public static Angle ComputeAngleFromTwoVectors(XYPosition originVect1, XYPosition destVect1, XYPosition originVect2, XYPosition destVect2)
        {
            var vect1 = new Vector(destVect1.X - originVect1.X, destVect1.Y - originVect1.Y);
            var vect2 = new Vector(destVect2.X - originVect2.X, destVect2.Y - originVect2.Y);

            return new Angle(Vector.AngleBetween(vect1, vect2), AngleUnit.Degree);
        }

        public static void ApplyAntiClockwiseRotation(Angle angle, XYZTopZBottomPosition position, XYPosition rotationCenter)
        {
            var xyPosition = position.ToXYPosition();
            ApplyAntiClockwiseRotation(angle, xyPosition, rotationCenter);
            position.X = xyPosition.X;
            position.Y = xyPosition.Y;
        }

        public static void ApplyAntiClockwiseRotation(Angle angle, XYPosition position, XYPosition rotationCenter)
        {
            if (position.Referential != rotationCenter.Referential)
            {
                throw new ArgumentException($"The rotation center referential must be the same as that of the position to which you want to apply a rotation.");
            }

            if (angle.Degrees != 0)
            {
                angle = -angle;
                var x = position.X - rotationCenter.X;
                var y = position.Y - rotationCenter.Y;

                double phi = Math.Atan2(y, x) - angle.Radians;
                double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                x = r * Math.Cos(phi);
                y = r * Math.Sin(phi);

                position.X = rotationCenter.X + x;
                position.Y = rotationCenter.Y + y;
            }
        }
        public static double Variance<T>(this IEnumerable<T> list, Func<T, double> selectA, Func<T, double> selectB)
        {
            return list.Average(p => selectA(p) * selectB(p)) - (list.Average(p => selectA(p)) * list.Average(p => selectB(p)));
        }
    }
}
