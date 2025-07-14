#pragma once

#include <opencv2/core/core.hpp>

namespace filter {
    namespace edge_detector {
        /**
         * This function executes successively prefiltering (Deriche smoothing filter) and gradient computing (Sobel mask).
         *
         * Deriche edge detector (often referred to as Canny-Deriche detector) is an edge detection operator developed by Rachid Deriche in 1987.
         * It's a multistep algorithm used to obtain an optimal result of edge detection in a discrete two-dimensional image.
         * This algorithm is based on John F. Canny's work related to the edge detection (Canny's edge detector) and his criteria for optimal edge detection:
         *  - Detection quality – all existing edges should be marked and no false detection should occur.
         *  - Accuracy - the marked edges should be as close to the edges in the real image as possible.
         *  - Unambiguity - a given edge in the image should only be marked once. No multiple responses to one edge in the real image should occur.
         *
         * Unlike the Canny edge detector, Deriche edge detector uses the IIR Deriche filter.
         * Using the IIR filter makes sense especially in cases where the processed image is noisy or a large amount of smoothing is required
         * (which leads to large convolution kernel for FIR filter). In these cases, the Deriche detector has considerable advantage over the Canny detector,
         * because it is able to process images in a short constant time independent of the desired amount of smoothing.
         *
         * @param  img       - input image array
         * @param  gamma     - filter parameter : defines the width of the filter, therefore the compromise between detection and localization.
         *                     The larger γ, the more we localize precisely the outline.The smaller it is, the easier it is to detect the presence of the
         *                     edges. Value between 0 (no filtering) et 1 (maximal filtering).
         * @param  removeNoise  - parameter to use an Otsu mask after the gradient (helps to clean up the noise)
         * @param  applyNormalization  - parameter to apply a normalization. If false, the output is in CV_32F.
         *
         * @return Filtered image
         */
        cv::Mat DericheGradient(const cv::Mat& img, float gamma, bool removeNoise, bool applyNormalization = true);

        /**
         * This function executes successively prefiltering (Shen smoothing filter) and gradient computing (Sobel mask).
         * See DericheGradient for more details.
         *
         * @param  img       - input image array
         * @param  gamma     - filter parameter : defines the width of the filter, therefore the compromise between detection and localization.
         *                     The larger γ, the more we localize precisely the outline.The smaller it is, the easier it is to detect the presence of the
         *                     edges. Value between 0 (no filtering) et 1 (maximal filtering).
         * @param  removeNoise  - parameter to use an Otsu mask after the gradient (helps to clean up the noise)
         * @param  applyNormalization  - parameter to apply a normalization. If false, the output is in CV_32F.
         * 
         *
         * @return Filtered image
         */
        cv::Mat ShenGradient(const cv::Mat& img, float gamma, bool removeNoise, bool applyNormalization = true);

    } // namespace edge_detector
} // namespace filter