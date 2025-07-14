#pragma once

#include "ImageData.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public enum class FocusMeasureMethod {
        VarianceOfLaplacian,
        NormalizedVariance,
        TenenbaumGradient,
        SumOfModifiedLaplacian,
        VollathF4,
    };

    public enum class ThresholdType {
        StrictlyAboveThreshold,
        StrictlyBelowThreshold,
        AboveOrEqualThreshold,
        BelowOrEqualThreshold
    };

    public ref class ImageOperators {
    public:

        /**
         * Simple standard deviation computation
         *
         * @param img     image on which to compute the standard deviation
         *
         * @return        standard deviation
         */
        static double ImageOperators::StandardDeviation(ImageData^ img);
        /**
         * Grayscale median computing
         *
         * @param img     - image on which to calculate the vignetting effect measure
         *
         * @return grayscale median
         */
        static int ImageOperators::GrayscaleMedianComputation(ImageData^ img);

        /**
         * Vignetting Operator
         *
         * @param img     - image on which to calculate the vignetting effect measure
         * @param periodPixel - number of pixels between two elements of the test patern
         *
         * @return Vignetting effect value
         */
        static double VignettingOperator(ImageData^ img, int periodPixel);

        /**
         * Focus measurement
         *
         * @param img     - image on which to calculate the focus measure
         * @param method  - operator used to calculate focus
         *
         * @return Focus value
         */
        static double FocusMeasurement(ImageData^ img, FocusMeasureMethod method);

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

        /**
         * Extract the value in the pixel along the line between two pixel position
         *
         * @param img     - image to extract the profile
         * @param firstPixel     - first pixel of the line
         * @param secondPixel     - last pixek of the line
         *
         * @return array of the points, where x is the position along the line and y is the intensity of the pixel
         */
        static array<Point2d^>^ ExtractIntensityProfile(ImageData^ img, Point2i^ startPixel, Point2i^ endPixel);

        /**
         * Resize an image to the region of interest with a different resolution
         *
         * @param img     - image to rezie
         * @param roi     - region of interest that will be cropped on the image
         * @param scale   - scale of resolution to apply on the final cropped image
         *
         * @return an image croped to the roi and rescaled
         */
        static ImageData^ Resize(ImageData^ img, RegionOfInterest^ roi, double scale);

        /**
         * Normalize the image between a lower and upper intensity value while saturating any value outside the bounds
         *
         * @param img     - image to normalize
         * @param lowerIntensity     - lower value for the normalization (anything lower than this will be 0)
         * @param upperIntensity     - upper value for the normalization (anything higher than this will be max intensity)
         *
         * @return a normalized image
         */
        static ImageData^ SaturatedNormalization(ImageData^ img, int lowerIntensity, int upperIntensity);

        /**
         * Calculate histogram of given image
         *
         * @param img                 - image (only unsigned 8-bits and float 32-bits images are supported)
         * @param binsNb              - number of bins
         *
         * @return histogram
         */
        static array<float>^ CalculateHistogram(ImageData^ img, int binsNb);

        /**
        * Calculate histogram of given image
        *
        * @param img                 - image (only unsigned 8-bits and float 32-bits images are supported)
        * @param mask                - mask for the histogram calculation (only unsigned 8-bits, same size as img)
        * @param binsNb              - number of bins
        *
        * @return histogram
        */
        static array<float>^ CalculateHistogram(ImageData^ img, ImageData^ mask, int binsNb);
        
        static double ComputeGreyLevelSaturation(ImageData^ img, float acceptablePercentageOfSaturatedPixels);
        
        static double ComputeGreyLevelSaturation(ImageData^ img, ImageData^ mask, float acceptablePercentage);

        /**
         * Find all pixel coordinates of given range value
         *
         * @param img                 - image
         * @param threshold           - threshold value
         * 0param type                - threshold type use to find pixel location
         *
         * @return all pixel coordinates
         */
        static array<Point2i^>^ FindPixelCoordinatesByThresholding(ImageData^ img, float threshold, ThresholdType type);

        /**
         * Find all pixel coordinates of given range value
         *
         * @param img                 - image
         * @param minThreshold        - min value of range (included)
         * 0param maxThreshold        - max value of range (included)
         *
         * @return all pixel coordinates
         */
        static array<Point2i^>^ FindPixelCoordinatesByRange(ImageData^ img, float minThreshold, float maxThreshold);

        /**
        * Combine two 32bit images into one using the root sum of squares
        *
        * @param firstImg       - The first image to use for the root sum of squares combination
        * @param secondImg      - The second image to use for the root sum of squares combination
        *
        * @return the combined root sum of squares image
        */
        static ImageData^ RootSumOfSquares32BitImage(ImageData^ firstImg, ImageData^ secondImg);
    };
}
