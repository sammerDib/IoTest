#include <opencv2/core/core.hpp>

#include <BaseAlgos/EdgeDetector.hpp>
#include <BaseAlgos/Registration.hpp>

#include <EdgeDetector.h>
#include <ImageData.h>

using namespace AlgosLibrary;
using namespace System;
using namespace System::Runtime::InteropServices;

namespace AlgosLibrary {

    ImageData^ EdgeDetector::edgeDetection(ImageData^ img, double gamma, RegionOfInterest^ regionOfInterest) {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Rect roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, regionOfInterest->Width, regionOfInterest->Height);

        // Call native method
        imgMat = filter::edge_detector::DericheGradient(imgMat, gamma);
        if (!roi.empty()) {
            cv::Mat mask = cv::Mat::zeros(imgMat.rows, imgMat.cols, CV_8U); // all 0
            mask(roi) = 255;
            cv::Mat temp;
            imgMat.copyTo(temp, mask);
            imgMat = temp;
        }

        // Process output result
        ImageData^ edgeImg = gcnew ImageData();
        edgeImg->ByteArray = CreateByteArrayFromMat(imgMat);
        edgeImg->Depth = img->Depth;
        edgeImg->Height = img->Height;
        edgeImg->Width = img->Width;

        return edgeImg;
    }
}