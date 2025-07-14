#include <opencv2/core/core.hpp>

#include "CEdgeDetector.hpp"
#include "CRegistration.hpp"

#include "EdgeDetector.h"
#include "ImageData.h"

#pragma managed
using namespace System;
using namespace System::Runtime::InteropServices;

namespace UnitySCSharedAlgosOpenCVWrapper {
    ImageData^ EdgeDetector::edgeDetection(ImageData^ img, double gamma, RegionOfInterest^ regionOfInterest, BlurFilterMethod filterMethod, bool removeNoise) {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Rect roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, (int)regionOfInterest->Width, (int)regionOfInterest->Height);

        // Call native method
        if (filterMethod == BlurFilterMethod::Deriche) {
            imgMat = filter::edge_detector::DericheGradient(imgMat, (float)gamma, removeNoise, true); // conversion from 'double' to 'float', possible loss of data // to do coralie
        }
        else {
            imgMat = filter::edge_detector::ShenGradient(imgMat, (float)gamma, removeNoise, true); // conversion from 'double' to 'float', possible loss of data // to do coralie
        }

        if (!roi.empty()) {
            cv::Mat mask = cv::Mat::zeros(imgMat.rows, imgMat.cols, CV_8U); // all 0
            mask(roi) = 1;
            imgMat = imgMat.mul(mask);
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