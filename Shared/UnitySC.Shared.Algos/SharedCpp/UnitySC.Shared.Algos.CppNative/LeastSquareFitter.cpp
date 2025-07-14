#include "LeastSquareFitter.h"

#include <cmath>
#include <iostream>

using namespace geometry;

#pragma unmanaged
namespace least_square_fitter {
    /// <summary>
    /// Finds the plane that minimizes the sum of least square distance with each point.
    /// More documentation on: https://www.geometrictools.com/Documentation/LeastSquaresFitting.pdf
    /// </summary>
    Plane FitLeastSquarePlane(std::vector <Point3d> points) {
        if (points.size() < 3)
            throw std::exception("Impossible to compute least square plane: to few points (<3).");

        // Compute the mean of the points.
        Point3d mean = Point3d();
        for (const Point3d& point : points)
        {
            mean += point;
        }
        mean /= (double)points.size();

        if (!std::isfinite(mean.X) || !std::isfinite(mean.Y) || !std::isfinite(mean.Z))
            throw std::exception("Impossible to compute least square plane: infinite/NaN mean of points.");

        // Compute the covariance matrix of the points.
        double covarXX = 0, covarXY = 0, covarXZ = 0;
        double covarYY = 0, covarYZ = 0;
        for (const Point3d& point : points)
        {
            Point3d diff = point - mean;
            covarXX += diff.X * diff.X;
            covarXY += diff.X * diff.Y;
            covarXZ += diff.X * diff.Z;
            covarYY += diff.Y * diff.Y;
            covarYZ += diff.Y * diff.Z;
        }

        // Decompose the covariance matrix.
        double det = covarXX * covarYY - covarXY * covarXY;
        // A 0 determinant can happen if we have a "vertical" plane, when the Z component of the normal is 0.
        // It is currently not specifically handled.
        if (det == 0.0)
            throw std::exception("Impossible to compute least square plane: 0 determinant.");

        double invDet = 1 / det;
        double xNormal = (covarYY * covarXZ - covarXY * covarYZ) * invDet;
        double yNormal = (covarXX * covarYZ - covarXY * covarXZ) * invDet;
        double zNormal = -1.0;
        Point3d normal(xNormal, yNormal, zNormal);
        return Plane(mean, normal);
    }
}