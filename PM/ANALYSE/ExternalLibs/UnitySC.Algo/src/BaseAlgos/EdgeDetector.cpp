#include <BaseAlgos/Filters.hpp>
#include <Logger.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace cv;

namespace filter {

    namespace edge_detector {
        /*
         * Deriche edge detector, like Canny edge detector, consists on the following 4 steps:
         *  1 - Smoothing
         *  2 - Calculation of magnitude and gradient direction
         *  3 - Non-maximum suppression
         *  4 - Hysteresis thresholding (using two thresholds)
         * The essential difference is in the implementation of the first two steps of the algorithm.
         */
        Mat DericheGradient(const Mat& img, float gamma) {

             // Prefilter the image before applying the derivation operator (which naturally amplify the high-frequency noise)
            Mat result = filter::DericheBlur(img, gamma);

            // Compute an image gradient (that is, the partial derivatives of the image along the x and y directions and add these two gradient)
            Mat gx, gy, agx, agy;
            Sobel(result, gx, CV_32F, 1, 0, 1);
            Sobel(result, gy, CV_32F, 0, 1, 1);
            convertScaleAbs(gx, agx); // Convert 8 bits unsigned
            convertScaleAbs(gy, agy); // Convert 8 bits unsigned
            addWeighted(agx, .5, agy, .5, 0, result);
            normalize(result, result, 0, 255, NORM_MINMAX);

            return result;
        }
    } // namespace edge_detector
} // namespace filter