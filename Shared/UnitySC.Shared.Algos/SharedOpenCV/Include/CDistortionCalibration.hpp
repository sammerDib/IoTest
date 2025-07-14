#pragma once
#include <opencv2/opencv.hpp>
#include <math.h>
#include <filesystem>
#include "CalibrationParameters.hpp"
#include "CheckerBoardsSettings.hpp"

#pragma unmanaged

namespace distortionCalibration {

    std::array<std::array<float, 10>, 2> ComputeDistoMatrix(cv::Mat& img, float gridCircleDiameterInMicrons, float gridPeriodicityInMicrons, float pixelSizeInMicrons);

    cv::Mat UndistortImage(const cv::Mat& img, std::array<std::array<float, 10>, 2> distortionCoeffs);
}