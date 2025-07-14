#pragma once

#include "Plane.h"

#pragma managed
namespace UnitySCSharedAlgosCppWrapper {
    public ref class PlaneDetector {
    public:
        static Plane^ FindLeastSquarePlane(array<Point3d^>^ points);
    };
}