namespace UnitySC.PM.Shared.MathLib.Geometry
{
    public class Point3D<T>
    {
        /// <summary>
        /// X component of point
        /// </summary>
        protected T _X;

        /// <summary>
        /// Y component of point
        /// </summary>
        protected T _Y;

        /// <summary>
        /// Z component of point
        /// </summary>
        protected T _Z;

        /// <summary>
        /// Initializes a new instance of a point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point3D(T x, T y, T z)
        {
            _X = x;
            _Y = y;
            _Z = z;
        }

        public Point3D(Point3D<T> pt3D)
        {
            if (pt3D == null)
                return;
            _X = pt3D.X;
            _Y = pt3D.Y;
            _Z = pt3D.Z;
        }

        /// <summary>
        /// Gets or sets the X component of the point
        /// </summary>
        public T X
        {
            get { return _X; }
            set { _X = value; }
        }

        /// <summary>
        /// Gets or sets the Y component of the point
        /// </summary>
        public T Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        /// <summary>
        /// Gets or sets the Z component of the point
        /// </summary>
        public T Z
        {
            get { return _Z; }
            set { _Z = value; }
        }
    }
}