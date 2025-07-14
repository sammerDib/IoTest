#pragma once

#include "WaferFinder.hpp"
#include "ImageData.h"
#include "ShapeDetector.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref class WaferDetector {
    public:
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
        static Circle^ DetectWaferCircle(ImageData^ waferImg, float waferDiameter, float detectionTolerance, float cannyThreshold);

        /**
         * @brief Faster method to find the wafer circle on given image
         *
         * @param waferImg                          - Full wafer image
         * @param waferDiameterInMicrometers        - Diameter of the searched wafer in microns
         * @param detectionToleranceInMicrometers   - Tolerance threshold in diameter detection (waferDiameter +/- detectionTolerance) in microns
         * @param pixelSizeInMicrometers            - Pixel size of the image in microns
         *
         * @return The wafer circle on image (center and diameter).
         */
        static Circle^ FasterDetectWaferCircle(ImageData^ waferImg, float waferDiameterInMicrometers, float detectionToleranceInMicrometers, double pixelSizeInMicrometers);

        /**
        * Create a mask to select the area inside the wafer, excluding the edge area
        *
        * @param img                                - image of wafer on stage
        * @param pixelSize                          - Pixel size to convert pixels to millimeters
        * @param waferRadius                        - radius of wafer to be detected
        * @param edgeExclusion                      - edge size to exclude
        * @param directoryPathToStoreReport         - directory path to store report if not empty
        *
        * @return Mask of the region of interest, inside wafer
        */
        static ImageData^ CreateWaferMask(ImageData^ img, float pixelSize, float waferRadius, float edgeExclusion);
    };
}
#pragma once
