#pragma once

#include "Point3d.h"

#pragma unmanaged

namespace geometry {
    struct Plane {
        Plane(Point3d center, Point3d normal) : Center(center), Normal(normal)
        {}

        Point3d Center;
        Point3d Normal;
    };
}
