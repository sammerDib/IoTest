#pragma once

#include "ImageData.h"
#include "PixelSize.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref class PositionImageData : ImageData {
    public:
        PositionImageData(int width, int height) : ImageData(width, height)
        {
            Scale = gcnew PixelSize(1, 1);
            Centroid = gcnew Point2d(0, 0);
            StitchColumns = 1;
            StitchRows = 1;
        }

        PositionImageData()
        {
            Scale = gcnew PixelSize(1, 1);
            Centroid = gcnew Point2d(0, 0);
            StitchColumns = 1;
            StitchRows = 1;
        }

        PositionImageData(array<System::Byte>^ byteArray, int width, int height, ImageType type) : ImageData(byteArray, width, height, type) {}

        Point2d^ Centroid;
        PixelSize^ Scale;
        int StitchColumns;
        int StitchRows;
    };
}
