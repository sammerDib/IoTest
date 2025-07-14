#pragma once

#include "ImageData.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public enum class BlurFilterMethod {
        Deriche,
        Shen
    };

    public enum class DerivativeMethod {
        Sobel,
        Scharr
    };

    public ref class Filter {
    public:
        /**
        * Blur the given image
        * @param  img                  - input image
        * @param  gamma                - blur parameter
        * @param  filterMethod         - filtering methode (Deriche or Shen) using to perform blur.
        *
        * @return Blured image
        */
        static ImageData^ blur(ImageData^ img, double gamma, BlurFilterMethod filterMethod);

        /**
        * Apply canny filter on the given image (edge detection)
        * @param  img                  - input image
        * @param  cannyThreshold       - canny threshold (depending of derivative method used)
        * @param  derivativeMethod     - derivative methode (Sobel or Scharr) used to find the derivative of an image.
        *                                Scharr is said to give more accurate results where Sobel filter fails to work correctly.
        *
        * @return Filtered image with canny filter
        */
        static ImageData^ canny(ImageData^ img, int cannyThreshold, DerivativeMethod derivativeMethod);
    };
}
