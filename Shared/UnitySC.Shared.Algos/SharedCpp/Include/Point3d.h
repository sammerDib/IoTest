#pragma once
#pragma unmanaged

namespace geometry {
    struct Point3d {
        Point3d() : Point3d(0.0, 0.0, 0.0)
        {}

        Point3d(double x, double y, double z) : X(x), Y(y), Z(z)
        {}

        double X;
        double Y;
        double Z;

        Point3d& operator+=(const Point3d& rhs) {
            X += rhs.X;
            Y += rhs.Y;
            Z += rhs.Z;
            return *this;
        }

        Point3d& operator/=(double divisor) {
            X /= divisor;
            Y /= divisor;
            Z /= divisor;
            return *this;
        }

        Point3d operator-(const Point3d& rhs) const {
            return Point3d(X - rhs.X, Y - rhs.Y, Z - rhs.Z);
        }
    };
}