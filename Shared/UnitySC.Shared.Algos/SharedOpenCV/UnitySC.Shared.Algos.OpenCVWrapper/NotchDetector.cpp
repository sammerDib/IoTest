#include "NotchDetector.h"

#pragma managed
using namespace System;

namespace UnitySCSharedAlgosOpenCVWrapper {
    namespace {
        psd::NotchLocation ConvertToCppType(NotchLocation location)
        {
            switch (location)
            {
            default:
            case NotchLocation::Left:
                return psd::NotchLocation::Left;
            case NotchLocation::Right:
                return psd::NotchLocation::Right;
            case NotchLocation::Top:
                return psd::NotchLocation::Top;
            case NotchLocation::Bottom:
                return psd::NotchLocation::Bottom;
            }
        }
    }

    double NotchDetector::DetectNotchAngle(ImageData^ img, Circle^ wafer, NotchLocation notchLocation, int notchWidthInPixels, int roiWidthFactor, double deviationFactor, double similarityThreshold) {
        // Process input parameters
        cv::Mat image = CreateMatFromImageData(img);
        shape_finder::Circle waferCircle = shape_finder::Circle(cv::Point2d(wafer->Center->X, wafer->Center->Y), (float) wafer->Diameter);
        psd::NotchLocation location = ConvertToCppType(notchLocation);

        // Call native method
        double angle = psd::FindNotchCenterByPolarStats(image, waferCircle, location, notchWidthInPixels, roiWidthFactor, deviationFactor, similarityThreshold);

        return angle;
    }
}