#include "WaferDetector.h"
#include "WaferDetector.hpp"

#pragma managed
using namespace System;

namespace UnitySCSharedAlgosOpenCVWrapper {
    Circle^ WaferDetector::DetectWaferCircle(ImageData^ waferImg, float waferDiameter, float detectionTolerance, float cannyThreshold) {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(waferImg);

        // Call native method
        shape_finder::Circle circle = psd::FindWaferCircle(image, waferDiameter, detectionTolerance, cannyThreshold);

        // Process output result
        Point2d^ waferCenter = gcnew Point2d(circle.CenterPos.x, circle.CenterPos.y);
        Circle^ waferCircle = gcnew Circle(waferCenter, circle.Diameter);
        return waferCircle;
    }

    Circle^ WaferDetector::FasterDetectWaferCircle(ImageData^ waferImg, float waferDiameterInMicrometers, float detectionToleranceInMicrometers, double pixelSizeInMicrometers) {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(waferImg);

        // Call native method
        shape_finder::Circle circle = psd::FasterFindWaferCircle(image, waferDiameterInMicrometers, detectionToleranceInMicrometers, pixelSizeInMicrometers);

        // Process output result
        Point2d^ waferCenter = gcnew Point2d(circle.CenterPos.x, circle.CenterPos.y);
        Circle^ waferCircle = gcnew Circle(waferCenter, circle.Diameter);
        return waferCircle;
    }

    ImageData^ WaferDetector::CreateWaferMask(ImageData^ waferImg, float pixelSize, float waferRadius, float edgeExclusion)
    {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(waferImg);

        // Call native method
        cv::Mat mask = psd::CreateWaferMask(image, pixelSize, waferRadius, edgeExclusion);

        // Process output result
        ImageData^ maskImage = gcnew ImageData();
        maskImage->ByteArray = CreateByteArrayFromMat(mask);
        maskImage->Type = CreateImageType(mask);
        maskImage->Height = mask.size().height;
        maskImage->Width = mask.size().width;
        return maskImage;
    }
}