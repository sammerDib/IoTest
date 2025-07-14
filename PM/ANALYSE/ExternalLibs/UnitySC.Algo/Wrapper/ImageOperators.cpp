#include <ImageOperators.h>

using namespace AlgosLibrary;
using namespace System;

namespace AlgosLibrary {

    double ImageOperators::FocusMeasurement(ImageData^ img) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        double focusValue = img_operators::ModifiedLaplacian(imgMat);
        return focusValue;
    }

    double ImageOperators::ContrastMeasurement(ImageData^ img) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        double contrastValue = img_operators::NormalizedGraylevelVariance(imgMat);
        return contrastValue;
    }

    double ImageOperators::SaturationMeasurement(ImageData^ img) {
        cv::Mat imgMat = CreateMatFromImageData(img);
        double saturationValue = img_operators::Saturation(imgMat);
        return saturationValue;
    }
}