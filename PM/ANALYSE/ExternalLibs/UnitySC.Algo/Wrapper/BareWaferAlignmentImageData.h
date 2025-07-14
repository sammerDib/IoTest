#pragma once

#include <ImageData.h>
#include <Tools.h>

namespace AlgosLibrary {

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

    public ref class BareWaferAlignmentImageData : ImageData {

    public:
        BareWaferAlignmentImageData(int width, int height) : ImageData(width, height)
        {
            Scale = gcnew PixelSize(1, 1);
            Centroid = gcnew Point2d(0, 0);
            ExpectedShape = WaferEdgeShape::ANY;
        }

        BareWaferAlignmentImageData()
        {
            Scale = gcnew PixelSize(1, 1);
            Centroid = gcnew Point2d(0, 0);
            ExpectedShape = WaferEdgeShape::ANY;
        }

        BareWaferAlignmentImageData(array<System::Byte>^ byteArray, int width, int height, int depth) : ImageData(byteArray, width, height, depth) {}

        Point2d^ Centroid;
        PixelSize^ Scale;
        WaferEdgeShape ExpectedShape;
        EdgePosition EdgePosition;
    };
}
