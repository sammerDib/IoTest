#pragma once

#include <vector>
#include "Point3d.h"
#include "Plane.h"

#pragma unmanaged
namespace least_square_fitter {
    geometry::Plane FitLeastSquarePlane(std::vector <geometry::Point3d> points);
}