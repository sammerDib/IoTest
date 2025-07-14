#pragma once

#include "Point3d.h"
#include <cmath>

#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    public ref struct Plane {
        Plane(Point3d^ center, Point3d^ normal)
        {
            Center = center;
            Normal = normal;
        }
        
        double DistanceTo(Point3d^ point)
        {
            // Given a plane defined by its normal (A, B, C) and a point (x0, y0, z0), then the
            // the signed distance to a point (u, v, w) to the plane is
            // d = (A*(u-x0) + B*(v-y0) + C*(w-z0)) / sqrt(A² + B² + C²)
            double dividendX = Normal->X * (point->X - Center->X);
            double dividendY = Normal->Y * (point->Y - Center->Y);
            double dividendZ = Normal->Z * (point->Z - Center->Z);
            double divisor = sqrt(pow(Normal->X, 2) + pow(Normal->Y, 2) + pow(Normal->Z, 2));
            return (dividendX + dividendY + dividendZ) / divisor;
        }

        Point3d^ Normal;
        Point3d^ Center;
    };
}