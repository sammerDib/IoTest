using System;

namespace UnitySC.PM.Shared.MathLib.Geometry
{
    /// <summary>
    /// 4D Point with double precision
    /// </summary>
    [Serializable]
    public class Point4D
    {
        /// <summary>
        /// X component of point
        /// </summary>
        protected double _X;

        /// <summary>
        /// Y component of point
        /// </summary>
        protected double _Y;

        /// <summary>
        /// Z component of point
        /// </summary>
        protected double _Z;

        /// <summary>
        /// Z Lower component of point
        /// </summary>
        protected double _ZLower;

        /// <summary>
        /// Initializes a new instance of a point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point4D(double x, double y, double z, double zLower)
        {
            _X = x;
            _Y = y;
            _Z = z;
            _ZLower = zLower;
        }

        public Point4D(Point4D pt4D)
        {
            if (pt4D == null)
                return;
            _X = pt4D.X;
            _Y = pt4D.Y;
            _Z = pt4D.Z;
            _ZLower = pt4D.ZLower;
        }

        public Point4D()
        {
            _X = 0;
            _Y = 0;
            _Z = 0;
            _ZLower = 0;
        }

        /// <summary>
        /// Gets or sets the X component of the point
        /// </summary>
        public double X
        {
            get { return _X; }
            set { _X = value; }
        }

        /// <summary>
        /// Gets or sets the Y component of the point
        /// </summary>
        public double Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        /// <summary>
        /// Gets or sets the Z component of the point
        /// </summary>
        public double Z
        {
            get { return _Z; }
            set { _Z = value; }
        }

        /// <summary>
        /// Gets or sets the Z Lower component of the point
        /// </summary>
        public double ZLower
        {
            get { return _ZLower; }
            set { _ZLower = value; }
        }

        /// <summary>
        /// Makes a checks for if the points is spatially equal to another point.
        /// </summary>
        /// <param name="other">Point to check against</param>
        /// <returns>True if X and Y and Z values are the same</returns>
        public bool Equals3D(Point4D other)
        {
            return (X == other.X && Y == other.Y && Z == other.Z && ZLower == other.ZLower);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other">Offset to add</param>
        /// <returns></returns>
        public void AddOffset(Point4D other)
        {
            X += other.X;
            Y += other.Y;
            Z += other.Z;
            ZLower += other.ZLower;
        }
    }
}