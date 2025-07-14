#pragma once
#include <opencv2/opencv.hpp>
#include <math.h>
#include <filesystem>
#include "CalibrationParameters.hpp"
#include "SystemCalibration.hpp"

#pragma unmanaged

namespace psd {
    cv::Mat AngularReflectionsCalibration(cv::Mat& cameraImg, cv::Mat& mask, cv::Mat& CMNormalized, std::map<int, std::complex<double>>& refractiveIndexByWavelengthTable, std::vector<int>& wavelengthsList, ExtrinsicScreenParameters extrinsicScreenParam, double patternThresholdForPolynomialFit = 0.80, bool useVerticalScreenPolarization = true, std::string directoryPathToStoreReport = "");

    cv::Mat ApplyUniformityCorrection(cv::Mat& brightfield, cv::Mat& correctionImage, cv::Mat& mask, int targetSaturationLevel, float acceptablePercentageOfSaturatedPixels);
}