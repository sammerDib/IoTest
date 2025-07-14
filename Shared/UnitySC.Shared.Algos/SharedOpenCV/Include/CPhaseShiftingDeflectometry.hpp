#pragma once
#include <filesystem>
#include <opencv2/opencv.hpp>

#include "WrappedPhaseMap.hpp"

#pragma unmanaged
namespace psd {
    enum FringesDisplacement {
        X,
        Y
    };

    enum class FitSurface {
        None,
        PolynomeOrder2,
        PolynomeOrder3,
        PolynomeOrder4
    };

    /**
    * Calculate wrapped phase map from interferometry images
    *
    * @param imgs                          - interferometry images
    * @param stepNb                        - number of phase shifts used to capture interferometry images
    * @param fringesDisplacement           - direction of displacement of the fringes to know if we calculate the X or Y curvature
    * @param directoryPathToStoreReport    - directory path to store report if not empty
    *
    * @return The wrapped phase map, amplitude, background, dark
    */
    WrappedPhaseMap ComputePhaseMap(std::vector<cv::Mat> imgs, int stepNb, FringesDisplacement fringesDisplacement, std::filesystem::path directoryPathToStoreReport = "");

    /**
    * Calculate mask of wafer
    *
    * @param phaseMap                           - phase map calculated from interferometry images
    * @param pixelSizeInMicrons                 - pixel size of the wrapped phase in microns
    * @param waferDiameterInMicrons             - diameter of the wafer present in the wrapped phase in microns
    * @param waferShiftXInMicrons               - wafer shift (X) from the image center (used to accurately position the circle fill)
    * @param waferShiftYInMicrons               - wafer shift (Y) from the image center (used to accurately position the circle fill)
    * @param useWaferFill                       - whether or not to fill the mask with a circle
    * @param fillEdgeExclusionInMicrons         - edge size to exclude from the mask circle fill
    * @param amplitudeMinCurvature              - threshold to compute mask (default value is 15 if the initial interfometric images are 8 bits, divided by 255 if they are 32 bits)
    * @param intensityMinCurvature              - threshold to compute mask (default value is 10 if the initial interfometric images are 8 bits, divided by 255 if they are 32 bits)
    * @param directoryPathToStoreReport         - directory path to store report if not empty
    *
    * @return The mask
    */
    cv::Mat ComputeMask(WrappedPhaseMap phaseMap, double pixelSizeInMicrons, double waferDiameterInMicrons, double waferShiftXInMicrons, double waferShiftYInMicrons, bool useWaferFill = true, double fillEdgeExclusionInMicrons = 3000.0, float amplitudeMinCurvature = 15.0f, float intensityMinCurvature = 10.0f, std::filesystem::path directoryPathToStoreReport = "");

    /**
    * Calculate curvature map from wrapped phase map
    *
    * @param phaseMap                          - phase map calculated from interferometry images
    * @param mask                              - mask to apply
    * @param stepNb                            - number of phase shifts used to capture interferometry images
    * @param fringesDisplacement               - direction of the fringes to know if we calculate the X or Y curvature
    * @param directoryPathToStoreReport        - directory path to store report if not empty
    *
    * @return The curvature map
    */
    cv::Mat ComputeCurvature(WrappedPhaseMap phaseMap, cv::Mat& mask, int stepNb, FringesDisplacement fringesDisplacement, std::filesystem::path directoryPathToStoreReport = "");

    /**
    * Calculate the final dark as the average of both directions
    *
    * @param darkX                          - dark images in X direction
    * @param darkY                          - dark images in Y direction
    * @param mask                           - mask to apply
    * @param removeBackgroundSurfaceMethod  - methode use to remove background wafer surface after dark calculation (default value is PolynomeOrder2)
    * @param directoryPathToStoreReport     - directory path to store report if not empty
    *
    * @return The final dark
    */
    cv::Mat ComputeDark(cv::Mat& darkX, cv::Mat& darkY, cv::Mat& mask, FitSurface removeBackgroundSurfaceMethod, std::filesystem::path directoryPathToStoreReport = "");

    /**
    * The curvature dynammic calibration needs the acquisition of an image of the calibration wafer and computing the raw curvature maps, including mask.
    * Then, this function compute their background level.
    *
    * @param curvatureX                     - curvature image in X direction
    * @param curvatureY                     - curvature image in Y direction
    * @param mask                           - mask to apply
    *
    * @return the background standard deviation averaged on X and Y maps
    */
    float CalibrateCurvatureDynamics(const cv::Mat& curvatureX, const cv::Mat& curvatureY, const cv::Mat& mask);

    /**
    * Using dynamics calibration to adjust background noise
    *
    * @param img                            - initial image
    * @param mask                           - mask to apply
    * @param calibrationDynamicCoef         - noise level of the calibration wafer, obtained by calibration of current machine (for tool matching) in radians / pixel (default value is 0.005 and coming from Matlab calculation on PSD prototype).
    * @param noisyGrayLevel                 - target background gray level (default value is 20).
    * @param userDynamicCoef                - additional term from user, for special defects. Must be > 0 (default value is 1).
    * @param directoryPathToStoreReport     - directory path to store report if not empty
    */
    cv::Mat ApplyDynamicCalibration(cv::Mat& img, cv::Mat& mask, float calibrationDynamicCoef = 0.0055f, float noisyGrayLevel = 20.0f, float userDynamicCoef = 1.0f, std::filesystem::path directoryPathToStoreReport = "");

    /**
    * Using dynamics coefficient to adjust dark image
    *
    * @param dark                           - initial image
    * @param mask                           - mask to apply
    * @param dynamicCoef                    - dynamic coeficient (default value is 20).
    * @param percentageOfLowSaturation      - percentage of low saturation (default value is 0.03).
    * @param directoryPathToStoreReport     - directory path to store report if not empty
    */
    cv::Mat ApplyDynamicCoefficient(cv::Mat& dark, cv::Mat& mask, float dynamicCoef = 20.0f, float percentageOfLowSaturation = 0.03f, std::filesystem::path directoryPathToStoreReport = "");
} // namespace psd