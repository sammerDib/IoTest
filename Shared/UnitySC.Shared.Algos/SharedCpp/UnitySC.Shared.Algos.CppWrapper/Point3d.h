#pragma once
#pragma managed

namespace UnitySCSharedAlgosCppWrapper {
    public ref struct Point3d {
        Point3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        double X;
        double Y;
        double Z;
    };
}