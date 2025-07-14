#include <opencv2/core/core.hpp>

#include "CFilters.hpp"

#include "Filter.h"
#include "ImageData.h"

#pragma managed
using namespace System;
using namespace System::Runtime::InteropServices;

namespace UnitySCSharedAlgosOpenCVWrapper {
    ImageData^ Filter::blur(ImageData^ img, double gamma, BlurFilterMethod filterMethod) {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);

        // Call native method
        if (filterMethod == BlurFilterMethod::Deriche) {
            imgMat = filter::DericheBlur(imgMat, (float) gamma); // conversion from 'double' to 'float', possible loss of data // to do coralie
        }
        else {
            imgMat = filter::ShenBlur(imgMat, (float) gamma); // conversion from 'double' to 'float', possible loss of data // to do coralie
        }

        // Process output result
        ImageData^ bluredImg = gcnew ImageData();
        bluredImg->ByteArray = CreateByteArrayFromMat(imgMat);
        bluredImg->Type = CreateImageType(imgMat);
        bluredImg->Height = img->Height;
        bluredImg->Width = img->Width;

        return bluredImg;
    }

    ImageData^ Filter::canny(ImageData^ img, int cannyThreshold, DerivativeMethod derivativeMethod)
    {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);

        // Call native method
        if (derivativeMethod == DerivativeMethod::Scharr) {
            imgMat = filter::CannyScharr(imgMat, cannyThreshold);
        }
        else {
            imgMat = filter::CannySobel(imgMat, cannyThreshold);
        }

        // Process output result
        ImageData^ edgeImg = gcnew ImageData();
        edgeImg->ByteArray = CreateByteArrayFromMat(imgMat);
        edgeImg->Type = CreateImageType(imgMat);
        edgeImg->Height = img->Height;
        edgeImg->Width = img->Width;

        return edgeImg;
    }
}