#include <opencv2/core/core.hpp>

#include <ImageData.h>
#include <Registration.h>

using namespace AlgosLibrary;
using namespace System;
using namespace System::Runtime::InteropServices;

namespace AlgosLibrary {

    RegistrationInfos^ Registration::ImgRegistration(ImageData^ refImg, ImageData^ sensedImg, RegionOfInterest^ regionOfInterest) {

        // Process input parameters

        cv::Mat ref = CreateMatFromImageData(refImg);
        cv::Mat sensed = CreateMatFromImageData(sensedImg);
        cv::Rect roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, regionOfInterest->Width, regionOfInterest->Height);

        // Call native method

        // compute shift between sensed and ref image
        std::pair<double, cv::Point2f> angleAndShift = registration::ComputeAngleAndShift(ref, sensed, roi);
        double angleInDegrees = angleAndShift.first;
        cv::Point2f shift = angleAndShift.second;
        // apply the inverse shift on sensed image to realign it with ref image
        cv::Mat shiftMat = (cv::Mat_<double>(2, 3) << 1, 0, -shift.x, 0, 1, -shift.y);
        cv::Mat sensedRealigned = registration::ImgRegistration(ref, sensed, shiftMat);
        // measure the similarity score before and after the realignment
        double initialSimilarityScore = registration::ComputeSimilarity(ref, sensed, roi);
        double finalSimilarityScore = registration::ComputeSimilarity(ref, sensedRealigned, roi);

        // Process output result

        ImageData^ imgRegistered = gcnew ImageData();
        imgRegistered->ByteArray = CreateByteArrayFromMat(sensedRealigned);
        imgRegistered->Depth = refImg->Depth;
        imgRegistered->Height = refImg->Height;
        imgRegistered->Width = refImg->Width;

        RegistrationInfos^ result = gcnew RegistrationInfos();
        result->PixelShiftX = shift.x;
        result->PixelShiftY = shift.y;
        result->AngleInDegrees = angleInDegrees;
        result->InitialSimilarityScore = initialSimilarityScore;
        result->FinalSimilarityScore = finalSimilarityScore;
        result->ImgRegistered = imgRegistered;

        return result;
    }
}