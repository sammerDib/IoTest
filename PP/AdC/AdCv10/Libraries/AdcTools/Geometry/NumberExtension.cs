using System;

namespace AdcTools
{
    public static class NumberExtension
    {
        private const double π = Math.PI;

        //=================================================================
        // Projection d'un nombre sur un interval
        // Comment fait-on un template en C# ?
        //=================================================================
        public static double ClampTo(this double x, double a, double b)
        {
            if (x < a)
                return a;
            else if (x > b)
                return b;
            else
                return x;
        }

        public static float ClampTo(this float x, float a, float b)
        {
            if (x < a)
                return a;
            else if (x > b)
                return b;
            else
                return x;
        }

        public static int ClampTo(this int x, int a, int b)
        {
            if (x < a)
                return a;
            else if (x > b)
                return b;
            else
                return x;
        }

        //=================================================================
        // Arrondi superieur/inferieur
        //=================================================================
        public static int Floor(this int x, int step)
        {
            if (x < 0)
            {
                return -Ceil(-x, step);
            }
            else
            {
                x /= step;
                x *= step;

                return x;
            }
        }

        public static int Ceil(this int x, int step)
        {
            if (x < 0)
            {
                return -Floor(-x, step);
            }
            else
            {
                if (x % step > 0)
                {
                    x += step;
                    x /= step;
                    x *= step;
                }
                return x;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public static bool IsInRange(this int x, int a, int b)
        {
            return (a <= x && x <= b);
        }

        //=================================================================
        // 
        //=================================================================
        public static double DegreeToRadian(double deg)
        {
            double rad = deg / 180 * π;
            return rad;
        }

        public static double RadianToDegree(double rad)
        {
            double deg = rad * 180 / π;
            return rad;
        }

        //=================================================================
        /// <summary> Remet l´angle entre 0 et 2π </summary>
        //=================================================================
        public static double NormalizeAngle(double angle)
        {
            angle %= 2 * π;
            if (angle < 0)
                angle += 2 * π;
            return angle;
        }

        //=================================================================
        // 
        //=================================================================
        private const double Epsilon = 1e-20;

        public static bool IsZeroEpsilon(this double d)
        {
            return Math.Abs(d) < Epsilon;
        }
        public static bool IsZeroEpsilon(this float d)
        {
            return Math.Abs(d) < Epsilon;
        }

        //=================================================================
        // 
        //=================================================================
        public static string ToMathString(this double d)
        {
            if (double.IsNegativeInfinity(d))
                return "-∞";
            else if (double.IsPositiveInfinity(d))
                return "+∞";
            else if (double.IsNaN(d))
                return "∅";
            else if (d == π)
                return "π";
            else if (d == -π)
                return "-π";
            else
                return d.ToString();
        }

        public static string ToMathString(this float d)
        {
            if (float.IsNegativeInfinity(d))
                return "-∞";
            else if (float.IsPositiveInfinity(d))
                return "+∞";
            else if (float.IsNaN(d))
                return "∅";
            else if (d == (float)π)
                return "π";
            else if (d == -(float)π)
                return "-π";
            else
                return d.ToString();
        }

        public static string MinMaxString(float min, float max, string var)
        {
            string str = "";

            if (!float.IsNegativeInfinity(min) && !float.IsPositiveInfinity(max))
                str += " " + min + " <= " + var + " <=" + max + " ";
            else if (!float.IsNegativeInfinity(min))
                str += " " + min + " <= " + var + " ";
            else if (!float.IsPositiveInfinity(max))
                str += " " + var + " <= " + max + " ";

            return str;
        }

        public static string MinMaxString(double min, double max, string var)
        {
            string str = "";

            if (!double.IsNegativeInfinity(min) && !double.IsPositiveInfinity(max))
                str += " " + min + " <= " + var + " <=" + max + " ";
            else if (!double.IsNegativeInfinity(min))
                str += " " + min + " <= " + var + " ";
            else if (!double.IsPositiveInfinity(max))
                str += " " + var + " <= " + max + " ";

            return str;
        }

    }
}
