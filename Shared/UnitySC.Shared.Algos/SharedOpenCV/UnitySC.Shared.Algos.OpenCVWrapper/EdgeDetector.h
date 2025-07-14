#pragma once

#include "ImageData.h"
#include "Tools.h"
#include "Filter.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    public ref class EdgeDetector {

    public:
        /**
        * Compute an edge detection on given image
        * @param  img                  - input image
        * @param  gamma                - filter parameter : defines the width of the filter, therefore the compromise between detection and localization.
        *                                The larger γ, the more we localize precisely the outline.The smaller it is, the easier it is to detect the presence of the edges.
                                         Value between 0 (no filtering) et 1 (maximal filtering).
        * @param regionOfInterest      - region of interest on given image
        * @param  filterMethod         - filtering methode (Deriche or Shen) using to perform blur.
        * @param  removeNoise          - parameter to use an Otsu mask after the gradient (helps to clean up the noise)
        * 
        * @return Filtered image with edge detection
        */
        static ImageData^ edgeDetection(ImageData^ img, double gamma, RegionOfInterest^ regionOfInterest, BlurFilterMethod filterMethod, bool removeNoise);
    };
}
