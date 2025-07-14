#pragma once
#include <opencv2/opencv.hpp>
#include <math.h>
#include <filesystem>
#include "CalibrationParameters.hpp"
#include "SystemCalibration.hpp"

#pragma unmanaged

namespace psd {
    struct CMResult {
        cv::Mat CMNormalized;
        cv::Mat M;
    };

    struct MPResult {
        cv::Mat MPNormalized;
        cv::Mat P;
    };

    CMResult ComputeCMNormalizedAndM(cv::Size imgSize, CalibrationParameters intrinsicCameraParam, ExtrinsicCameraParameters extrinsicCameraParam);

    MPResult ComputeMPNormalizedAndP(const cv::Mat& M, const cv::Mat& unwrapX, const cv::Mat& unwrapY, ExtrinsicScreenParameters extrinsicScreenParam, float pixelSizeInMillimeters, int fringePeriodInPixels);

    cv::Mat ComputeNormalizedNormal(const cv::Mat& M, const cv::Mat& CMNormalized, const cv::Mat& MPNormalized, const cv::Mat& mask, std::string directoryPathToStoreReport = "");
}