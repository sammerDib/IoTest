#pragma once
#pragma unmanaged

namespace geometry {
    struct Point2d {
        constexpr Point2d() : Point2d(0.0, 0.0)
        {}

        constexpr Point2d(double x, double y) : X(x), Y(y)
        {}

        double X;
        double Y;
    };
}