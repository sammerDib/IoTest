#pragma once

#pragma unmanaged

#include <opencv2/opencv.hpp>
#include <opencv2/core/types.hpp>

namespace img_operators {
    enum ThresholdType {
        StrictlyAboveThreshold,
        StrictlyBelowThreshold,
        AboveOrEqualThreshold,
        BelowOrEqualThreshold,
        InsideRange
    };

    struct ImageDataStatistics
    {
        double Min;
        double Max;
        double Mean;
        double StandardDeviation;
    };

    /**
     * Simple standard deviation value computation of an image.
     *
     * @param img         - image on which to compute the standard deviation
     *
     * @return standard deviation value
     */
    double StandardDeviation(cv::Mat img);

     /**
     * Measurement of the median grayscale value of an image.
     * Histogram-based operator: based on the histogram or distribution of grayscale levels in the image.
     * Follows the assumption that images with a wide variety of grayscale levels have a more informative median value.
     *
     * @param img         - image on which to calculate the median value measure
     *
     * @return Median grayscale value
     */
    int GrayscaleMedian(cv::Mat channel);

    double standartDeviationZoneByZone(const cv::Mat& img, int period);
    /**
     * Focus measurement based on Tenenbaum gradient (Tenengrad).
     * Gradient-based operator : based on the gradient or first derivative of the image.
     * Follow the assumption that focused images present more sharp edges than blurred ones.
     * Reference : Yeo et al. (1993) and Krotkov (1987)
     *
     * @param img         - image on which to calculate the focus measure
     * @param kernelSize  - size of the kernel
     *
     * @return Focus measure
     */
    double TenenbaumGradient(const cv::Mat& img, int kernelSize = 5);

    /**
     * Focus measurement based on an alternative Laplacian definition.
     * Laplacian-based operators : based on the Laplacian or second derivative of the image.
     * Follow the assumption that focused images present more sharp edges than blurred ones.
     * Reference : [1989, Nayar] Shape from Focus
     *
     * @param img     - image on which to calculate the focus measure
     *
     * @return Focus measure
     */
    double SumOfModifiedLaplacian(const cv::Mat& img);

    /**
     * Focus measurement based on Laplacian variance.
     * Laplacian-based operators : based on the Laplacian or second derivative of the image.
     * Follow the assumption that focused images present more sharp edges than blurred ones.
     *
     * @param img     - image on which to calculate the focus measure
     *
     * @return Focus measure
     */
    double VarianceOfLaplacian(const cv::Mat& img);

    /**
     * Focus measurement based on Vollath F4.
     * Based on the autocorrelation function
     * Reference : Vollath (1987, 1988)
     *
     * @param img         - image on which to calculate the focus measure
     *
     * @return Focus measure
     */
    double VollathF4(const cv::Mat& img);

    /**
     * Focus measurement based on normalized gray level variance.
     * Statistics-based operators : based on image contrast or gray level variance.
     * Follow the assumption that the perceptibility of objects in the scene increases
     * with the difference in brightness between objects and their background.
     *
     * @param img     - image on which to calculate the contrast measure
     *
     * @return Contrast measure
     */
    double NormalizedVariance(const cv::Mat& img);

    /**
     * Saturation measurement based on Hue, Saturation, Lightness representation of image,
     * saturation being the average of the lightness channel of this image.
     *
     * @param img     - image on which to calculate the saturation measure
     *
     * @return Saturation measure
     */
    double Saturation(const cv::Mat& img);

    /**
     * Extract the value in the pixel along the line between two pixel position
     *
     * @param img     - image to extract the profile
     * @param firstPixel     - first pixel of the line
     * @param secondPixel     - last pixek of the line
     *
     * @return array of the points, where x is the position along the line and y is the intensity of the pixel
     */
    std::vector<cv::Point2d> ExtractIntensityProfile(const cv::Mat& img, cv::Point2i startPixel, cv::Point2i endPixel);

    /**
     * Resize an image to the region of interest with a different resolution
     *
     * @param img     - image to rezie
     * @param roi     - region of interest that will be cropped on the image
     * @param scale   - scale of resolution to apply on the final cropped image
     *
     * @return an image croped to the roi and rescaled
     */
    cv::Mat Resize(const cv::Mat& img, cv::Rect roi, double scale);

    /**
     * Normalize the image between a lower and upper intensity bound while saturating any value outside the bounds
     *
     * @param img     - image to normalize
     * @param lowerIntensity     - lower intensity bound for the normalization (anything lower than this will be 0)
     * @param upperIntensity     - upper intensity bound for the normalization (anything higher than this will be max intensity)
     *
     * @return a normalized image
     */
    cv::Mat SaturatedNormalization(const cv::Mat& img, int lowerIntensity, int upperIntensity);

    /**
     * Calculate histogram of given image
     *
     * @param img                 - image (only unsigned 8-bits and float 32-bits images are supported)
     * @param mask                - mask image (can be empty, if not, must be an unsigned 8-bit image same size as img)
     * @param binsNb              - number of bins
     *
     * @return histogram
     */
    std::vector<float> CalculateHistogram(const cv::Mat& img, const cv::Mat& mask, int binsNb);
    
    /**
     * Compute the saturation level of a greyscale image while discardidng a given percentage of saturated pixels
     *
     * @param img                                   - image (only unsigned 8-bits and float 32-bits images are supported)
     * @param mask                                  - mask image (can be empty, if not, must be an unsigned 8-bit image same size as img)
     * @param acceptablePercentageOfSaturatedPixels - percentage of sturated pixels to discard
     */
    double ComputeGreyLevelSaturation(const cv::Mat& img, const cv::Mat& mask, float acceptablePercentageOfSaturatedPixels);

    /**
     * Find all pixel coordinates of given range value
     *
     * @param img                   - image
     * @param type                  - threshold type use to find pixel location (above value, below value or between values)
     * @param threshold1            - threshold value
     * 0param threshold2            - optional second threshold value, use only to find pixel inside range
     *
     * @return all pixel coordinates
     */
    std::vector<cv::Point> FindPixelCoordinates(const cv::Mat& img, ThresholdType type, float threshold1, float threshold2 = NAN);

    /**
    * Combine two 32bit images into one using the root sum of squares
    *
    * @param firstImg       - The first image to use for the root sum of squares combination
    * @param secondImg      - The second image to use for the root sum of squares combination
    *
    * @return the combined (root sum of squares) image
    */
    cv::Mat RootSumOfSquares32BitImage(const cv::Mat& firstImg, const cv::Mat& secondImg);
} // namespace img_operators