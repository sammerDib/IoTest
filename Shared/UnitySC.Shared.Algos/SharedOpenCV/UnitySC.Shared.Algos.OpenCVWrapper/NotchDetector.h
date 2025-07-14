#pragma once

#include "NotchFinder.hpp"
#include "ImageData.h"
#include "ShapeDetector.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public enum class NotchLocation {
        Left,
        Right,
        Top,
        Bottom
    };

    public ref class NotchDetector {
    public:
        /**
         * @brief Finds the notch angle in relation to the detected wafer center on a given image
         * @details The reliability of this algorithm depends directly on the reliability of the computation of the circle corresponding to the wafer, provided as input to this algorithm.
         *
         * @param img                           - Full wafer image.
         * @param wafer                         - Circle (center & diameter) corresponding to the wafer on the image.
         * @param notchLocation                 - Notch location on wafer.
         * @param notchWidthInPixels            - Approximate notch width in pixels to detect on the image
         * @param roiWidthFactor                - Number of notchWidthInPixels to fit inside the roi search area width (roi search area width = roiWidthFactor * notchWidthInPixels) (default good value = 18)
         * @param deviationFactor               - Influences how sensible the detector is to small changes in grey intensity (default good value = 3.7)
         * @param similarityThreshold           - Threshold of similarity to accept / reject shapes that are not symmetrical enough (1.0 is perfect symmetry, 0.0 is no symmetry) (default good value = 0.6)
         *
         *
         * @return The notch center (the x and y coordinates are both rectified by the algorithm)
         */
        static double DetectNotchAngle(ImageData^ img, Circle^ wafer, NotchLocation notchLocation, int notchWidthInPixels, int roiWidthFactor, double deviationFactor, double similarityThreshold);
    };
}
#pragma once
