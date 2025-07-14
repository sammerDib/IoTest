#include <opencv2/core/core.hpp>

#include "ImageData.h"
#include "Registration.h"
#include <CEdgeDetector.hpp>

#pragma managed
using namespace System;
using namespace System::Runtime::InteropServices;

namespace UnitySCSharedAlgosOpenCVWrapper {
    RegistrationInfos^ Registration::ImgRegistration(ImageData^ refImg, ImageData^ sensedImg, RegionOfInterest^ regionOfInterest, int dilationSize, System::String^ reportPath) {
        // Process input parameters

        IntPtr ip = Marshal::StringToHGlobalAnsi(reportPath);
        std::string reportPathCPlusPlus = static_cast<const char*>(ip.ToPointer());
        Marshal::FreeHGlobal(ip);

        cv::Mat ref = CreateMatFromImageData(refImg);
        cv::Mat sensed = CreateMatFromImageData(sensedImg);
        cv::Rect roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, (int) regionOfInterest->Width, (int) regionOfInterest->Height);

        // Call native method to compute shift between sensed and ref image

        std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref, sensed, roi, registration::_defaultAngleSigmaTolerance, registration::_defaultScaleSigmaTolerance, dilationSize, reportPathCPlusPlus);
        double angleInDegrees = std::get<0>(angleAndShift);
        cv::Point2f shift = std::get<1>(angleAndShift);
        double confidence = std::get<2>(angleAndShift);

        // Process output result

        RegistrationInfos^ result = gcnew RegistrationInfos();
        result->PixelShiftX = shift.x;
        result->PixelShiftY = shift.y;
        result->AngleInDegrees = angleInDegrees;
        result->Confidence = confidence;

        return result;
    }

    RegistrationInfos^ Registration::ImgRegistration(ImageData^ refImg, ImageData^ sensedImg, RegionOfInterest^ regionOfInterest, double angleSigmaTolerance, double scaleSigmaTolerance, int dilationSize, System::String^ reportPath) {
        // Process input parameters

        IntPtr ip = Marshal::StringToHGlobalAnsi(reportPath);
        std::string reportPathCPlusPlus = static_cast<const char*>(ip.ToPointer());
        Marshal::FreeHGlobal(ip);

        cv::Mat ref = CreateMatFromImageData(refImg);
        cv::Mat sensed = CreateMatFromImageData(sensedImg);
        cv::Rect roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, (int)regionOfInterest->Width, (int)regionOfInterest->Height);

        // Call native method to compute shift between sensed and ref image

        std::tuple<double, cv::Point2f, double> angleAndShift = registration::ComputeAngleAndShift(ref, sensed, roi, angleSigmaTolerance, scaleSigmaTolerance, dilationSize, reportPathCPlusPlus);
        double angleInDegrees = std::get<0>(angleAndShift);
        cv::Point2f shift = std::get<1>(angleAndShift);
        double confidence = std::get<2>(angleAndShift);

        // Process output result

        RegistrationInfos^ result = gcnew RegistrationInfos();
        result->PixelShiftX = shift.x;
        result->PixelShiftY = shift.y;
        result->AngleInDegrees = angleInDegrees;
        result->Confidence = confidence;

        return result;
    }

    RegistrationQualityInfos^ Registration::ComputeQualityOfRegistration(ImageData^ refImg, ImageData^ sensedImg, RegistrationInfos^ registrationData, RegionOfInterest^ regionOfInterest, BlurFilterMethod filterMethod, double gamma, bool removeNoise) {
        // Process input parameters

        cv::Mat ref = CreateMatFromImageData(refImg);
        cv::Mat sensed = CreateMatFromImageData(sensedImg);
        cv::Rect roi = cv::Rect(regionOfInterest->X, regionOfInterest->Y, (int)regionOfInterest->Width, (int)regionOfInterest->Height);

        // Call native method

        double shiftX = -registrationData->PixelShiftX;
        double shiftY = -registrationData->PixelShiftY;

        // preprocess images to remove noise

        cv::Mat sensedImgEdges;
        cv::Mat referenceImgEdges;

        if (filterMethod == BlurFilterMethod::Deriche)
        {
            sensedImgEdges = filter::edge_detector::DericheGradient(sensed, (float) gamma, removeNoise, true);
            referenceImgEdges = filter::edge_detector::DericheGradient(ref, (float) gamma, removeNoise, true);
        }
        else
        {
            sensedImgEdges = filter::edge_detector::ShenGradient(sensed, (float) gamma, removeNoise, true);
            referenceImgEdges = filter::edge_detector::ShenGradient(ref, (float) gamma, removeNoise, true);
        }

        // apply the inverse shift on sensed image to realign it with ref image

        cv::Mat shiftMat = (cv::Mat_<double>(2, 3) << 1, 0, shiftX, 0, 1, shiftY);
        cv::Mat realignedImg = registration::ImgRegistration(ref, sensed, shiftMat);
        cv::Mat realignedImgEdges = registration::ImgRegistration(referenceImgEdges, sensedImgEdges, shiftMat);

        // preprocess images to remove useless informations before compute similarity score

        int roiX = (shiftX > 0) ? (int) ceil(shiftX) : 0;
        int roiY = (shiftY > 0) ? (int) ceil(shiftY) : 0;
        int roiCols = (shiftX > 0) ? ref.cols - (int)ceil(shiftX) : ref.cols - 1 + (int)floor(shiftX);
        int roiRows = (shiftY > 0) ? ref.rows - (int)ceil(shiftY) : ref.rows - 1 + (int)floor(shiftY);

        cv::Rect areaWithUsefulData = cv::Rect(roiX, roiY, roiCols, roiRows);
        cv::Mat mask = cv::Mat::zeros(ref.rows, ref.cols, CV_8U);
        mask(areaWithUsefulData) = 255;

        cv::Mat referenceImgPreprocessed = cv::Mat::ones(ref.rows, ref.cols, CV_8U);
        referenceImgEdges.copyTo(referenceImgPreprocessed, mask);

        cv::Mat realignedImgPreprocessed = cv::Mat::ones(ref.rows, ref.cols, CV_8U);
        realignedImgEdges.copyTo(realignedImgPreprocessed, mask);

        // measure the similarity score before and after the realignment

        double initialSimilarityScore = registration::ComputeSimilarity(referenceImgEdges, sensedImgEdges, roi);
        double finalSimilarityScore = registration::ComputeSimilarity(referenceImgPreprocessed, realignedImgPreprocessed, roi);

        // Process output result

        ImageData^ imgRegistered = gcnew ImageData();
        imgRegistered->ByteArray = CreateByteArrayFromMat(realignedImg);
        imgRegistered->Type = CreateImageType(realignedImg);
        imgRegistered->Height = refImg->Height;
        imgRegistered->Width = refImg->Width;

        RegistrationQualityInfos^ result = gcnew RegistrationQualityInfos();
        result->InitialSimilarityScore = initialSimilarityScore;
        result->FinalSimilarityScore = finalSimilarityScore;
        result->ImgRegistered = imgRegistered;

        return result;
    }

    ImageData^ Registration::RealignImages(ImageData^ refImg, ImageData^ sensedImg, RegistrationInfos^ registrationData) {
        // Process input parameters

        cv::Mat ref = CreateMatFromImageData(refImg);
        cv::Mat sensed = CreateMatFromImageData(sensedImg);

        double shiftX = -registrationData->PixelShiftX;
        double shiftY = -registrationData->PixelShiftY;

        // apply the inverse shift on sensed image to realign it with ref image

        cv::Mat shiftMat = (cv::Mat_<double>(2, 3) << 1, 0, shiftX, 0, 1, shiftY);
        cv::Mat realignedImg = registration::ImgRegistration(ref, sensed, shiftMat);

        // Process output result

        ImageData^ imgRegistered = gcnew ImageData();
        imgRegistered->ByteArray = CreateByteArrayFromMat(realignedImg);
        imgRegistered->Type = CreateImageType(realignedImg);
        imgRegistered->Height = refImg->Height;
        imgRegistered->Width = refImg->Width;

        return imgRegistered;
    }
}