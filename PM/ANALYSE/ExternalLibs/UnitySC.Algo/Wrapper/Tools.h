#pragma once

#include <ImageData.h>
#include <Wafer.hpp>
#include <opencv2/core/core.hpp>
#include <vector>

using namespace System;
using namespace System::Collections::Generic;

namespace AlgosLibrary {

    public ref struct Point2i {
        Point2i(int x, int y) {
            X = x;
            Y = y;
        }
        int X;
        int Y;
    };

    public ref struct Point2d {
        Point2d(double x, double y) {
            X = x;
            Y = y;
        }
        double X;
        double Y;
    };

    public ref struct RegionOfInterest {
        int X;
        int Y;
        double Width;
        double Height;
    };

    public enum class StatusCode : int {
        OK = 0,
        UNKNOWN_ERROR = -1,
        MISSING_EDGE_IMAGE = -2,
        MISSING_NOTCH_IMAGE = -3,
        BWA_FIT_FAILED = -4,
        CANNOT_DETECT_EDGE_ON_NOTCH_IMAGE = -5
    };

    public ref class ExecutionStatus {
    public:
        StatusCode Code;
        String^ Message;
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
            default:
                return StatusCode::UNKNOWN_ERROR;
            }
        }
    };

    template <class T> array<T>^ CreateArrayFromVector(const std::vector<T>& dataVec);

    array<System::Byte>^ CreateByteArrayFromMat(const cv::Mat& data);

    cv::Mat CreateMatFromImageData(ImageData^ img);

    const char* StringToCharArray(String^ string);
}
