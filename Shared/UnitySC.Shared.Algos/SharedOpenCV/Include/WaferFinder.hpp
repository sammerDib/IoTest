#pragma once

#include <opencv2/opencv.hpp>
#include "CShapeFinder.hpp"

#pragma unmanaged

namespace psd {
    /**
     * @brief Find the wafer circle on given image
     *
     * @param waferImg                      - Full wafer image
     * @param waferDiameter                 - Diameter of the searched wafer
     * @param detectionTolerance            - Tolerance threshold in diameter detection (waferDiameter +/- detectionTolerance)
     * @param cannyThreshold                - Canny edge detector parameter.
     * @param print                         - If set to true, displays debug images during processing.
     *
     * @return The wafer circle on image (center and diameter).
     */
    shape_finder::Circle FindWaferCircle(const cv::Mat& waferImg, float waferDiameter, float detectionTolerance, float cannyThreshold = 100, bool print = false);

    /**
     * @brief Find the wafer circle on given image (faster version)
     *
     * @param waferImg                      - Full wafer image
     * @param waferDiameterInMicrometers    - Diameter of the searched wafer in micrometers
     * @param waferToleranceInMicrometers   - Tolerance threshold in diameter detection (waferDiameter +/- detectionTolerance) in micrometers
     * @param pixelSizeInMicrometers        - Pixel size of the wafer image in micrometers.
     * @param reportPath                    - If not empty, writes report images in the specified folder path.
     *
     * @return The wafer circle on image (center and diameter).
     */
    shape_finder::Circle FasterFindWaferCircle(const cv::Mat& waferImg, float waferDiameterInMicrometers, float waferToleranceInMicrometers, double pixelSizeInMicrometer, std::string const& reportPath = "");
}