#pragma once

#include <BaseAlgos/ImageOperators.hpp>
#include <ImageData.h>
#include <Tools.h>

namespace AlgosLibrary {

    public ref class ImageOperators {

    public:
        /**
         * Focus measurement
         *
         * @param img     - image on which to calculate the focus measure
         *
         * @return Focus value
         */
        static double FocusMeasurement(ImageData^ img);

        /**
         * Contrast measurement
         *
         * @param img     - image on which to calculate the contrast measure
         *
         * @return Contrast value
         */
        static double ContrastMeasurement(ImageData^ img);

        /**
         * Saturation measurement
         *
         * @param img     - image on which to calculate the saturation measure
         *
         * @return Saturation value
         */
        static double SaturationMeasurement(ImageData^ img);
    };
}
