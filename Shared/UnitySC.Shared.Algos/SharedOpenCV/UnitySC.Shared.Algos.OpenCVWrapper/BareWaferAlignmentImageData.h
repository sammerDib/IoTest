#pragma once

#include "PositionImageData.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref class BareWaferAlignmentImageData : PositionImageData {
    public:
        BareWaferAlignmentImageData(int width, int height) : PositionImageData(width, height)
        {
            ExpectedShape = WaferEdgeShape::ANY;
        }

        BareWaferAlignmentImageData()
        {
            ExpectedShape = WaferEdgeShape::ANY;
        }

        BareWaferAlignmentImageData(array<System::Byte>^ byteArray, int width, int height, ImageType type) : PositionImageData(byteArray, width, height, type) {}

        WaferEdgeShape ExpectedShape;
        EdgePosition EdgePosition;
    };
}
