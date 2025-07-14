#pragma once

#include "ImageData.h"
#include "Tools.h"
#include "CImageTypeConvertor.hpp"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref class Converter {
    public:
        /**
        * Convert the given image into 8bits grayscale
        * @param  img                  - input image
        *
        * @return Converted image
        */
        static ImageData^ convertTo8UC1(ImageData^ img);

        /**
        * Convert the given image into 32bits grayscale
        * @param  img                  - input image
        *
        * @return Converted image
        */
        static ImageData^ convertTo32FC1(ImageData^ img);
    };
}
