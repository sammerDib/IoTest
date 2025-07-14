#pragma once

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    public ref class PixelSize {
    public:
        PixelSize(double x, double y) {
            X = x;
            Y = y;
        }
        double X;
        double Y;
    };

    public enum class WaferEdgeShape : int {
        EDGE,
        NOTCH,
        ANY
    };

    public enum class EdgePosition : int {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT
    };
}
