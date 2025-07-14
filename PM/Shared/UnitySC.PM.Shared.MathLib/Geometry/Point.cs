using System;
using System.ComponentModel;

namespace UnitySC.PM.Shared.MathLib.Geometry
{
    /// <summary>
    /// 2D Point with double precision
    /// </summary>
    [Serializable]
    public class Point
    {
        [Browsable(true), Category("Common")]
        public string Label { get; set; }

        /// <summary>
        /// Initializes a new instance of a point
        /// </summary>
        public Point()
        {
            X = 0;
            Y = 0;
            Validity = false;
        }

        /// <summary>
        /// Initializes a new instance of a point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Validity = true;
        }

        public Point(double x, double y, bool validity)
        {
            X = x;
            Y = y;
            Validity = validity;
        }

        public Point(double x, double y, string szLabel)
        {
            X = x;
            Y = y;
            Validity = true;
            Label = szLabel;
        }

        /// <summary>
        /// Gets or sets the X component of the point
        /// </summary>
        [Browsable(true), Category("Common")]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the Y component of the point
        /// </summary>
        [Browsable(true), Category("Common")]
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the validity component of the point
        /// </summary>
        [Browsable(true), Category("Common")]
        public bool Validity { get; set; }

        /// <summary>
        /// Makes a planar checks for if the points is spatially equal to another point.
        /// </summary>
        /// <param name="other">Point to check against</param>
        /// <returns>True if X and Y values are the same</returns>
        public bool Equals2D(Point other)
        {
            return (X == other.X && Y == other.Y);
        }

        public bool Equals2D_Tolerance(Point other, double tolerance)
        {
            bool bEqual = false;

            if ((Math.Abs(X - other.X) < tolerance) && (Math.Abs(Y - other.Y) < tolerance))
                bEqual = true;

            return bEqual;
        }

        public double[] ToArray()
        {
            double[] pt = new double[2];
            pt[0] = X;
            pt[1] = Y;
            return pt;
        }

        public override string ToString()
        {
            return Label + ", (" + X + "," + Y + "), " + (Validity ? "Valid" : "Invalid");
        }
    }
}