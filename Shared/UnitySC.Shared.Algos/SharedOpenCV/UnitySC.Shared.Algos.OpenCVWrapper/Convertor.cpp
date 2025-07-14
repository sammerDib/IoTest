#include <opencv2/core/core.hpp>

#include "CImageTypeConvertor.hpp"

#include "Convertor.h"
#include "ImageData.h"

#pragma managed
using namespace System;
using namespace System::Runtime::InteropServices;

namespace UnitySCSharedAlgosOpenCVWrapper {
    ImageData^ Converter::convertTo8UC1(ImageData^ img) {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);

        // Call native method
        cv::Mat convertedImgMat = Convertor::ConvertTo8UC1(imgMat);

        // Process output result
        ImageData^ convertedImg = gcnew ImageData();
        convertedImg->ByteArray = CreateByteArrayFromMat(convertedImgMat);
        convertedImg->Type = CreateImageType(convertedImgMat);
        convertedImg->Height = img->Height;
        convertedImg->Width = img->Width;

        return convertedImg;
    }

    ImageData^ Converter::convertTo32FC1(ImageData^ img) {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);

        // Call native method
        cv::Mat convertedImgMat = Convertor::ConvertTo32FC1(imgMat);

        // Process output result
        ImageData^ convertedImg = gcnew ImageData();
        convertedImg->ByteArray = CreateByteArrayFromMat(convertedImgMat);
        convertedImg->Type = CreateImageType(convertedImgMat);
        convertedImg->Height = img->Height;
        convertedImg->Width = img->Width;

        return convertedImg;
    }
}