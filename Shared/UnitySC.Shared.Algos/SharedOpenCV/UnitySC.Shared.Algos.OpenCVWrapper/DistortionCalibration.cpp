#include "CDistortionCalibration.hpp"

#include "DistortionCalibration.h"
#include "Tools.h"

#pragma managed
using namespace System;

namespace UnitySCSharedAlgosOpenCVWrapper {

    static array<float, 2>^ NativeToManagedCoeffs(const std::array<std::array<float, 10>, 2>& nativeDistortionCoeffs) {
        array<float, 2>^ distortionCoeffs = gcnew array<float, 2>(2, 10);

        for (int i = 0; i < 2; ++i) {
            for (int j = 0; j < 10; ++j) {
                distortionCoeffs[i, j] = nativeDistortionCoeffs[i][j];
            }
        }

        return distortionCoeffs;
    }

    static std::array<std::array<float, 10>, 2> ManagedToNativeCoeffs(array<float, 2>^ managedDistortionCoeffs) {
        std::array<std::array<float, 10>, 2> distortionCoeffs;

        for (int i = 0; i < 2; ++i) {
            for (int j = 0; j < 10; ++j) {
                distortionCoeffs[i][j] = managedDistortionCoeffs[i, j];
            }
        }

        return distortionCoeffs;
    }

    array<float, 2>^ DistortionCalibration::ComputeDistortion(ImageData^ img, float gridCircleDiameterInMicrons, float gridPeriodicityInMicrons, float pixelSizeInMicrons) {
        //This function just convert things in proper format before and after matrix computation 

        cv::Mat imgMat = CreateMatFromImageData(img);
        
        std::array<std::array<float, 10>, 2> nativeDistortionCoeffs = distortionCalibration::ComputeDistoMatrix(imgMat, gridCircleDiameterInMicrons, gridPeriodicityInMicrons, pixelSizeInMicrons);

        array<float, 2>^ distortionCoeffs = NativeToManagedCoeffs(nativeDistortionCoeffs);

        return distortionCoeffs;
    }

    ImageData^ DistortionCalibration::UndistortImage(ImageData^ img, array<float, 2>^ distortionCoeffs)
    {
        cv::Mat imgMat = CreateMatFromImageData(img);
        std::array<std::array<float, 10>, 2> nativeCoeffs = ManagedToNativeCoeffs(distortionCoeffs);

        cv::Mat undistortedImage = distortionCalibration::UndistortImage(imgMat, nativeCoeffs);

        ImageData^ result = gcnew ImageData();
        result->ByteArray = CreateByteArrayFromMat(undistortedImage);
        result->Type = CreateImageType(undistortedImage);
        result->Height = undistortedImage.rows;
        result->Width = undistortedImage.cols;

        return result;
    }

}