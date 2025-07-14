#pragma once

#include <opencv2/opencv.hpp>
#include "CShapeFinder.hpp"

#pragma unmanaged

namespace psd {
    enum class NotchLocation {
        Left,
        Right,
        Top,
        Bottom
    };

    /**
     * @brief Find the notch position center on given image
     * @details The reliability of this algorithm depends directly on the reliability of the computation of the circle corresponding to the wafer, provided as input to this algorithm.
     *
     * @param img                           - Full wafer image.
     * @param wafer                         - Circle (center & diameter) corresponding to the wafer on the image.
     * @param notchLocation                 - Notch location on wafer.
     * @param edgeDetectorGamma             - Filter parameter used to compute edge detection: defines the width of the filter, therefore the compromise between detection and localization.
     *                                        The larger γ, the more we localize precisely the outline.The smaller it is, the easier it is to detect the presence of the edges.
     *                                        Value between 0 (no filtering) et 1 (maximal filtering).
     * @param roiMaxSize                    - Maximum size of the region of interest around the notch.
     *                                        Should not be too small to ensure that the notch will be included in it despite its positional uncertainties,
     *                                        and not too large to prevent the area from including too much artifact in addition to the notch.
     * @param print                         - If set to true, displays debug images during processing.
     *
     * @return The notch center (the x and y coordinates are both rectified by the algorithm)
     */
    cv::Point FindNotchCenterByPatternRecognition(const cv::Mat& img, const shape_finder::Circle& wafer, NotchLocation notchLocation, float edgeDetectorGamma = 0.5, int roiMaxSize = 400, bool print = false);

    /**
     * @brief Find the notch position center on given image
     * @details The reliability of this algorithm depends directly on the reliability of the computation of the circle corresponding to the wafer, provided as input to this algorithm.
     *
     * @param img                           - Full wafer image.
     * @param wafer                         - Circle (center & diameter) corresponding to the wafer on the image.
     * @param notchLocation                 - Notch location on wafer.
     * @param edgeDetectorGamma             - Filter parameter used to compute edge detection: defines the width of the filter, therefore the compromise between detection and localization.
     *                                        The larger γ, the more we localize precisely the outline.The smaller it is, the easier it is to detect the presence of the edges.
     *                                        Value between 0 (no filtering) et 1 (maximal filtering).
     * @param roiMaxSize                    - Maximum size of the region of interest around the notch.
     *                                        Should not be too small to ensure that the notch will be included in it despite its positional uncertainties,
     *                                        and not too large to prevent the area from including too much artifact in addition to the notch.
     * @param print                         - If set to true, displays debug images during processing.
     *
     * @return The notch center (only the x coordinate is rectified by the algorithm, the y coordinate is the theoretical coordinate)
     */
    cv::Point FindNotchCenterByCorrelation(const cv::Mat& img, const shape_finder::Circle& wafer, NotchLocation notchLocation, float edgeDetectorGamma = 0.5, int roiMaxSize = 400, bool print = false);

    /**
     * @brief Find the notch angle from the wafer center
     * @details The reliability of this algorithm depends directly on the reliability of the computation of the circle corresponding to the wafer, provided as input to this algorithm.
     *
     * @param img                           - Full wafer image.
     * @param wafer                         - Circle (center & diameter) corresponding to the wafer on the image.
     * @param notchLocation                 - Notch location on wafer.
     * @param notchWidthInPixel             - Width in pixel of the notch used as a reference for the size of the search area
     * @param widthFactor                   - Factor to multiply notchWidthInPixel with to get the size of the search area
     * @param deviationFactor               - Influences how sensible the detector is to small changes in intensity
     * @param similarityThreshold           - Threshold to reject shapes that are not symmetrical enough
     * @param print                         - If set to true, displays debug images during processing.
     *
     * @return The notch angle in degrees from the wafer center
     */
    double FindNotchCenterByPolarStats(const cv::Mat& img, const shape_finder::Circle& wafer, NotchLocation notchLocation, int notchWidthInPixel, int widthFactor = 18, double deviationFactor = 3.7, double similarityThreshold = 0.6, bool print = false);


    /**
         * @brief Applies a polar transform to an image using a known circle

         * @param img                           - Full wafer image.
         * @param wafer                         - Circle (center & diameter) corresponding to the wafer on the image.
         * @param polarMargin                   - Optional margin to add the the polar image in case the detected wafer isn't cropped properly

         *
         * @return The transformed polar image
         */
    cv::Mat TransformImageToPolar(const cv::Mat& img, const shape_finder::Circle& wafer, int polarMargin = 0);
}