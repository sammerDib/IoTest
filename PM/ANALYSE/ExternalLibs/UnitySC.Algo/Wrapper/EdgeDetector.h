#pragma once

#include <ImageData.h>
#include <Tools.h>

namespace AlgosLibrary {

    public ref class EdgeDetector {

    public:
        /**
        * Compute an edge detection on given image
        * @param  img                  - input image
        * @param  gamma                - filter parameter : defines the width of the filter, therefore the compromise between detection and localization.
        *                                The larger γ, the more we localize precisely the outline.The smaller it is, the easier it is to detect the presence of the edges.
                                         Value between 0 (no filtering) et 1 (maximal filtering).
        * @param regionOfInterest      - region of interest on given image
        *
        * @return Filtered image with edge detection
        */
        static ImageData^ edgeDetection(ImageData^ img, double gamma, RegionOfInterest^ regionOfInterest);
    };
}
