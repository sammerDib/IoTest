#pragma once

#include "ImageData.h"

#pragma unmanaged
#include <opencv2/core/core.hpp>
#include <vector>

#pragma managed
using namespace System;
using namespace System::Collections::Generic;

namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref struct Point2i {
        Point2i(int x, int y) {
            X = x;
            Y = y;
        }
        int X;
        int Y;
    };
    cv::Point2i ToCVPoint2i(Point2i^ pt);

    public ref struct Point2d {
        Point2d(double x, double y) {
            X = x;
            Y = y;
        }
        double X;
        double Y;
    };
    cv::Point2d ToCVPoint2d(Point2d^ pt);

    public ref struct Point2f {
        Point2f(float x, float y) {
            X = x;
            Y = y;
        }
        float X;
        float Y;
    };
    cv::Point2f ToCVPoint2d(Point2f^ pt);
    
    public ref struct RegionOfInterest {
        int X;
        int Y;
        double Width;   // dans le native on utilise des int...
        double Height;  // dans le native on utilise des int...
    };
    cv::Rect ToCVRect(RegionOfInterest^ roi);
    cv::Rect2d ToCVRect2d(RegionOfInterest^ roi);
    RegionOfInterest^ FromCVRect2d(const cv::Rect2d &rect);
    

    public ref struct CircularRegionOfInterest {
        CircularRegionOfInterest(int radius, Point2i^ center)
        {
            Radius = radius;
            Center = center;
        }
        int Radius;
        Point2i^ Center;
    };

    public ref struct ImageDataStatistics {
        double Min;
        double Max;
        double Mean;
        double StandardDeviation;
    };

    public enum class StatusCode : int {
        OK = 0,
        UNKNOWN_ERROR = -1,
        MISSING_EDGE_IMAGE = -2,
        MISSING_NOTCH_IMAGE = -3,
        BWA_FIT_FAILED = -4,
        CANNOT_DETECT_EDGE_ON_NOTCH_IMAGE = -5,
        BWA_ANGLE_FAILED = -6
    };

    public ref class ExecutionStatus {
    public:
        StatusCode Code;
        System::String^ Message;
        double Confidence;

        static StatusCode CodeFrom(int integerValue) {
            switch (integerValue) {
            case 0:
                return StatusCode::OK;
            case -1:
                return StatusCode::UNKNOWN_ERROR;
            case -2:
                return StatusCode::MISSING_EDGE_IMAGE;
            case -3:
                return StatusCode::MISSING_NOTCH_IMAGE;
            case -4:
                return StatusCode::BWA_FIT_FAILED;
            case -5:
                return StatusCode::CANNOT_DETECT_EDGE_ON_NOTCH_IMAGE;
            case -6:
                return StatusCode::BWA_ANGLE_FAILED;
            default:
                return StatusCode::UNKNOWN_ERROR;
            }
        }
    };

    template <class T> array<T>^ CreateArrayFromVector(const std::vector<T>& dataVec);

    array<System::Byte>^ CreateByteArrayFromMat(const cv::Mat& data);

    ImageType CreateImageType(const cv::Mat& img);

    cv::Mat CreateMatFromImageData(ImageData^ img);

    const char* StringToCharArray(System::String^ string);

    std::string CSharpStringToCppString(System::String^ string);

    Exception^ HandleAndRethrowCppException(const std::exception& exception);
}
