#include <opencv2/imgproc/imgproc.hpp>

#include "CEdgeDetector.hpp"
#include "CFilters.hpp"

#pragma unmanaged
using namespace cv;

namespace filter {

    namespace edge_detector {

        namespace {
            /*
            * Compute an image gradient (that is, the partial derivatives of the image along the x and y directions and add these two gradient)
            */
            void ComputeImageGradient(Mat& image, bool applyNormalization = true);
        }

        /*
         * Deriche edge detector, like Canny edge detector, consists on the following 4 steps:
         *  1 - Smoothing
         *  2 - Calculation of magnitude and gradient direction
         *  3 - Non-maximum suppression
         *  4 - Hysteresis thresholding (using two thresholds)
         * The essential difference is in the implementation of the first two steps of the algorithm.
         */
        Mat DericheGradient(const Mat& img, float gamma, bool removeNoise, bool applyNormalization) {

             // Prefilter the image before applying the derivation operator (which naturally amplify the high-frequency noise)
            Mat result = filter::DericheBlur(img, gamma);

            // Compute an image gradient
            ComputeImageGradient(result, applyNormalization);

            if (removeNoise) filter::NoiseRemovedBelowThreshold(result);
            return result;
        }

        /*
         * Shen edge detector follows the same logic than DericheGradient, but use Shen filter for the first step of blur
         * */
        Mat ShenGradient(const Mat& img, float gamma, bool removeNoise, bool applyNormalization) {

            // Prefilter the image before applying the derivation operator (which naturally amplify the high-frequency noise)
            Mat result = filter::ShenBlur(img, gamma);

            // Compute an image gradient
            ComputeImageGradient(result, applyNormalization);

            if (removeNoise) filter::NoiseRemovedBelowThreshold(result);
            return result;
        }


        namespace {

            void ComputeImageGradient(Mat& image, bool applyNormalization)
            {
                Mat gx, gy, agx, agy;
                Sobel(image, gx, CV_32F, 1, 0, 1);
                Sobel(image, gy, CV_32F, 0, 1, 1);
                convertScaleAbs(gx, agx); // Convert 8 bits unsigned
                convertScaleAbs(gy, agy); // Convert 8 bits unsigned
                addWeighted(agx, .5, agy, .5, 0, image);
                if (applyNormalization) {
                    normalize(image, image, 0, 255, NORM_MINMAX);
                }
            }
        }
    } // namespace edge_detector
} // namespace filter