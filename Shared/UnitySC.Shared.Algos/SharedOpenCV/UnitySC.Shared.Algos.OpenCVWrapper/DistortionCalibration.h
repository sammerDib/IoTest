#pragma once

#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    public ref class DistortionCalibration {
    public:

        static array<float, 2>^ ComputeDistortion(ImageData^ img, float gridCircleDiameterInMicrons, float gridPeriodicityInMicrons, float pixelSizeInMicrons);

        static ImageData^ UndistortImage(ImageData^ img, array<float, 2>^ distortionCoeffs);
    };
}