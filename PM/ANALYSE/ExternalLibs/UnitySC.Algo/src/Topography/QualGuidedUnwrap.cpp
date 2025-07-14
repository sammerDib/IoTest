#include <math.h>
#include <opencv2/opencv.hpp>
#include <stdio.h>
#include <stdlib.h>

#include <Logger.hpp>
#include <Topography/QualGuidedUnwrap.hpp>

using namespace cv;
using namespace std;

#define BORDER 0x20    // 6th bit
#define UNWRAPPED 0x40 // 7th bit
#define POSTPONED 0x80 // 8th bit
#define CV_2PI CV_PI * CV_PI

namespace phase_unwrapping {

    namespace {
      /**
       * Generate quality map from wapped phase data
       *
       * @param wrappedPhaseMap           - the wrapped phase map
       * @param bitFlags                  - the bit flags array
       * @param mode                      - technique used to compute quality
       *
       * @return The quality map
       */
        Mat QualityMap(Mat& wrappedPhaseMap, Mat& bitFlags, QualityMode mode);

        /**
         * Unwrap the phase data following the quality path.
         *
         * @param wrappedPhaseMap       - the wrapped phase map
         * @param qualityMap            - the quality map
         * @param bitFlags              - the bit flags array
         *
         * @return Return the unwrapped phase map
         */
        Mat QualityGuidedPathFollower(Mat& wrappedPhaseMap, Mat& qualityMap, Mat& bitFlags);

        /**
         * Compute pseudo-correlation quality map
         *
         * @param wrappedPhaseMap          - the wrapped phase map
         * @param qualityMap               - the quality map filled with pseudo-correlation scores
         * @param kernelSize               - size of the averaging windows (used to averaging filter)
         */
        void PseudoCorrelationQualityMap(Mat& wrappedPhaseMap, Mat& qualityMap, int kernelSize = 3);

        /**
         * Compute maximum phase gradient quality map
         *
         * @param wrappedPhaseMap          - the wrapped phase map
         * @param qualityMap               - the quality map filled with pseudo-correlation scores
         * @param kernelSize               - size of the averaging windows (used to averaging filter)
         */
        void GradientQualityMap(Mat& wrappedPhaseMap, Mat& qualityMap, int kernelSize = 3);

        /**
         * Compute variance of phase derivative as quality map
         *
         * @param wrappedPhaseMap          - the wrapped phase map
         * @param qualityMap               - the quality map filled with pseudo-correlation scores
         * @param kernelSize               - size of the averaging windows (used to averaging filter)
         */
        void VarianceQualityMap(Mat& wrappedPhaseMap, Mat& qualityMap, int kernelSize = 3);

        /**
         * Update the four neighboring pixels of the given pixel to process unwrapping
         *
         * @param wrappedPhaseMap         - the wrapped phase map
         * @param unwrappedPhase          - the unwrapped phase map to compute
         * @param qualityMap              - the quality map filled with pseudo-correlation scores
         * @param bitFlags                - the bit flags array
         * @param pixel                   - the given pixel
         * @param pixelList               - the list of pixels ordered by quality
         */
        void UpdateNeighboringPixels(Mat& wrappedPhaseMap, Mat& unwrappedPhase, Mat& qualityMap, Mat& bitFlags, QualityPixel pixel, vector<QualityPixel>& pixelList);

        /**
         * Process pixel to insert into the list of pixels and eventually store new unwrapped value
         *
         * @param wrappedPhaseMap         - the wrapped phase map
         * @param unwrappedPhase          - the unwrapped phase map to compute
         * @param qualityMap              - the quality map filled with pseudo-correlation scores
         * @param bitFlags                - the bit flags array
         * @param pixel                   - the given pixel
         * @param pixelList               - the list of pixels ordered by quality
         */
        void ProcessPixel(Mat& phase, Mat& unwrappedPhase, Mat& qualityMap, Mat& bitFlags, QualityPixel pixel, float val, vector<QualityPixel>& pixelList);

        /**
         * Orderly insertion of a quality pixel into a quality pixel vector
         *
         * @param pixels                - the list of pixels ordered by quality
         * @param pixel                 - the pixel to insert
         */
        void OrderedInsert(vector<QualityPixel>& pixels, QualityPixel pixel);

        /**
         * Calculate gradient between two pixels
         *
         * @param p1     - First pixel
         * @param p2     - Second pixel
         *
         * @return Gradient value
         */
        float Gradient(float p1, float p2);
    } // namespace


    Mat QualityGuidedUnwrap(Mat& wrappedPhaseMap, QualityMode mode) {
        if (wrappedPhaseMap.type() != CV_32FC1) {
            stringstream strStrm;
            strStrm << "[Quality guided unwrap] Input phase map should be stored into a single channel 32-bits float";
            string message = strStrm.str();
            Logger::Error(message);
            return Mat();
        }

        double minMax = CV_PI + 0.01;
        bool validRangeValues = checkRange(wrappedPhaseMap, true, 0, -minMax, minMax) && !checkRange(wrappedPhaseMap, true, 0, 0, 1);
        if (!validRangeValues) {
            stringstream strStrm;
            strStrm << "[Quality guided unwrap] Input phase map should be wrapped between [-pi, pi]";
            string message = strStrm.str();
            Logger::Error(message);
            return Mat();
        }

        // Compute initial bit flags with border flag
        int height = wrappedPhaseMap.rows;
        int width = wrappedPhaseMap.cols;
        Mat bitFlags(height, width, CV_8UC1);
        bitFlags = Scalar::all(0);
        bitFlags(Rect(1, 1, width - 2, height - 2)) = 1;

        // Compute quality map
        Mat qualityMap = QualityMap(wrappedPhaseMap, bitFlags, mode);

        // Unwrap
        Mat unwrappedPhase = QualityGuidedPathFollower(wrappedPhaseMap, qualityMap, bitFlags);

        return unwrappedPhase;
    }

    namespace {

        Mat QualityMap(Mat& wrappedPhaseMap, Mat& bitFlags, QualityMode mode) {
            int height = wrappedPhaseMap.rows;
            int width = wrappedPhaseMap.cols;
            int size = height * width;

            Mat qualityMap = Mat::ones(wrappedPhaseMap.rows, wrappedPhaseMap.cols, CV_32FC1);

            // PSEUDO CORRELATION
            if (mode == QualityMode::PseudoCorrelation) {
                PseudoCorrelationQualityMap(wrappedPhaseMap, qualityMap);
                for (int pixelId = 0; pixelId < size; pixelId++) {
                    if (!bitFlags.empty() && (bitFlags.at<uchar>(pixelId) & BORDER))
                        qualityMap.at<float>(pixelId) = 0.0;
                }
            }

            // PHASE VARIANCE & PHASE GRADIENT
            else if (mode == QualityMode::Variance || mode == QualityMode::Gradient) {
                // Calculate quality map
                if (mode == QualityMode::Variance) {
                    VarianceQualityMap(wrappedPhaseMap, qualityMap);
                }
                else {
                    GradientQualityMap(wrappedPhaseMap, qualityMap);
                }
                // Scale to interval (0,1)
                double min, max;
                minMaxLoc(qualityMap, &min, &max);
                double scale = (min != max) ? 1.0 / (max - min) : 0.0;
                for (int pixelId = 0; pixelId < size; pixelId++) {
                    qualityMap.at<float>(pixelId) = (max - qualityMap.at<float>(pixelId)) * scale;
                    if (!bitFlags.empty() && (bitFlags.at<uchar>(pixelId) & BORDER))
                        qualityMap.at<float>(pixelId) = 0.0;
                }
            }

            // NONE
            else {
                for (int pixelId = 0; pixelId < size; pixelId++) {
                    qualityMap.at<float>(pixelId) = 1.0;
                    if (!bitFlags.empty() && (bitFlags.at<uchar>(pixelId) & BORDER))
                        qualityMap.at<float>(pixelId) = 0.0;
                }
            }

            return qualityMap;
        }

        Mat QualityGuidedPathFollower(Mat& wrappedPhaseMap, Mat& qualityMap, Mat& bitFlags) {
            int height = wrappedPhaseMap.rows;
            int width = wrappedPhaseMap.cols;
            int size = height * width;

            Mat unwrappedPhase = Mat::zeros(height, width, CV_32FC1);
            vector<QualityPixel> pixelList = vector<QualityPixel>();

            // Unwrapping
            float value;
            for (int row = 0; row < height; row++) {
                for (int col = 0; col < width; col++) {
                    if (!(bitFlags.at<uchar>(row, col) & (BORDER | UNWRAPPED))) {
                        if (!(bitFlags.at<uchar>(row, col) & (POSTPONED))) {
                            // already stores the unwrapped value
                            value = unwrappedPhase.at<float>(row, col);
                        }
                        else {
                            value = unwrappedPhase.at<float>(row, col) = wrappedPhaseMap.at<float>(row, col);
                        }
                        bitFlags.at<uchar>(row, col) |= UNWRAPPED;
                        bitFlags.at<uchar>(row, col) &= ~POSTPONED;

                        QualityPixel currentPixel(row, col, value);
                        UpdateNeighboringPixels(wrappedPhaseMap, unwrappedPhase, qualityMap, bitFlags, currentPixel, pixelList);
                        while (!pixelList.empty()) {
                            // Get next pixel to unwrap into pixel list
                            QualityPixel lastPixel = pixelList.back();
                            int col = lastPixel.col;
                            int row = lastPixel.row;
                            int index = row * width + col;
                            pixelList.pop_back();
                            bitFlags.at<uchar>(row, col) |= UNWRAPPED;
                            bitFlags.at<uchar>(row, col) &= ~POSTPONED;
                            float value = unwrappedPhase.at<float>(row, col);
                            QualityPixel currentPixel(row, col, value);
                            UpdateNeighboringPixels(wrappedPhaseMap, unwrappedPhase, qualityMap, bitFlags, currentPixel, pixelList);
                        }
                    }
                }
            }
            return unwrappedPhase;
        }

        void PseudoCorrelationQualityMap(Mat& wrappedPhaseMap, Mat& qualityMap, int kernelSize) {
            int height = wrappedPhaseMap.rows;
            int width = wrappedPhaseMap.cols;
            int size = height * width;

            // Extracting cos's
            Mat temp = Mat::ones(wrappedPhaseMap.rows, wrappedPhaseMap.cols, CV_32FC1);
            for (int pixelId = 0; pixelId < size; pixelId++) {
                temp.at<float>(pixelId) = cos(CV_2PI * wrappedPhaseMap.at<float>(pixelId));
            }

            // Filtering cos's with a mean filter & overwrite the values in quality map
            Mat temp2 = Mat::ones(wrappedPhaseMap.rows, wrappedPhaseMap.cols, CV_32FC1);
            blur(temp, temp2, Size(kernelSize, kernelSize));
            qualityMap = temp2;

            // Extracting sin's
            for (int pixelId = 0; pixelId < size; pixelId++) {
                temp.at<float>(pixelId) = sin(CV_2PI * wrappedPhaseMap.at<float>(pixelId));
            }

            // Filtering sin's with a mean filter & add the result to the values in quality map
            blur(temp, temp2, Size(kernelSize, kernelSize));
            pow(temp2, 2, temp);
            qualityMap += temp;

            // Square root
            for (int pixelId = 0; pixelId < size; pixelId++)
                qualityMap.at<float>(pixelId) = 1.0 - sqrt(qualityMap.at<float>(pixelId));
        }

        void GradientQualityMap(Mat& wrappedPhaseMap, Mat& qualityMap, int kernelSize) {
            Mat kernel = Mat::ones(kernelSize, kernelSize, CV_32F);

            // Calculate the gradient in x direction
            Mat temp = Mat::ones(wrappedPhaseMap.rows, wrappedPhaseMap.cols, CV_32FC1);
            Sobel(wrappedPhaseMap, temp, CV_32FC1, 1, 0, kernelSize);

            // Extracting dx max's (perform max filtering on image using dilate)
            Mat temp2 = Mat::ones(wrappedPhaseMap.rows, wrappedPhaseMap.cols, CV_32FC1);
            dilate(temp, temp2, kernel);
            qualityMap = temp2;

            // Calculate the gradient in y direction
            Sobel(wrappedPhaseMap, temp, CV_32FC1, 0, 1, kernelSize);

            // Extracting dy max's & add to dx max's
            dilate(temp2, temp, kernel);
            qualityMap += temp;
        }

        void VarianceQualityMap(Mat& wrappedPhaseMap, Mat& qualityMap, int kernelSize) { 
            Laplacian(wrappedPhaseMap, qualityMap, CV_32FC1, kernelSize);
        }

        void UpdateNeighboringPixels(Mat& phase, Mat& unwrappedPhase, Mat& qualityMap, Mat& bitFlags, QualityPixel pixel, vector<QualityPixel>& pixelList) {
            // Insert the four neighboring pixels of the given pixel (x,y) into the list.

            if (pixel.col - 1 >= 0 && !(bitFlags.at<uchar>(pixel.row, pixel.col - 1) & (BORDER | UNWRAPPED))) {
                float grad = Gradient(phase.at<float>(pixel.row, pixel.col - 1), phase.at<float>(pixel.row, pixel.col));
                QualityPixel neighboringPixel(pixel.row, pixel.col - 1, qualityMap.at<float>(pixel.row, pixel.col - 1));
                ProcessPixel(phase, unwrappedPhase, qualityMap, bitFlags, neighboringPixel, pixel.quality + grad, pixelList);
            }

            if (pixel.col + 1 < phase.cols && !(bitFlags.at<uchar>(pixel.row, pixel.col + 1) & (BORDER | UNWRAPPED))) {
                float grad = -Gradient(phase.at<float>(pixel.row, pixel.col), phase.at<float>(pixel.row, pixel.col + 1));
                QualityPixel neighboringPixel(pixel.row, pixel.col + 1, qualityMap.at<float>(pixel.row, pixel.col + 1));
                ProcessPixel(phase, unwrappedPhase, qualityMap, bitFlags, neighboringPixel, pixel.quality + grad, pixelList);
            }

            if (pixel.row - 1 >= 0 && !(bitFlags.at<uchar>(pixel.row - 1, pixel.col) & (BORDER | UNWRAPPED))) {
                float grad = Gradient(phase.at<float>(pixel.row - 1, pixel.col), phase.at<float>(pixel.row, pixel.col));
                QualityPixel neighboringPixel(pixel.row - 1, pixel.col, qualityMap.at<float>(pixel.row - 1, pixel.col));
                ProcessPixel(phase, unwrappedPhase, qualityMap, bitFlags, neighboringPixel, pixel.quality + grad, pixelList);
            }

            if (pixel.row + 1 < phase.rows && !(bitFlags.at<uchar>(pixel.row + 1, pixel.col) & (BORDER | UNWRAPPED))) {
                float grad = -Gradient(phase.at<float>(pixel.row, pixel.col), phase.at<float>(pixel.row + 1, pixel.col));
                QualityPixel neighboringPixel(pixel.row + 1, pixel.col, qualityMap.at<float>(pixel.row + 1, pixel.col));
                ProcessPixel(phase, unwrappedPhase, qualityMap, bitFlags, neighboringPixel, pixel.quality + grad, pixelList);
            }
        }

        void ProcessPixel(Mat& phase, Mat& unwrappedPhase, Mat& qualityMap, Mat& bitFlags, QualityPixel pixel, float val, vector<QualityPixel>& pixelList) {
            float minQuality = -1.0E+10;
            float quality = qualityMap.at<float>(pixel.row, pixel.col);

            // if not postponed, store new unwrapped value (if postponed, leave old value)
            if (~(bitFlags.at<uchar>(pixel.row, pixel.col) & POSTPONED)) {
                unwrappedPhase.at<float>(pixel.row, pixel.col) = val;
            }

            // if quality is too low, postpone it
            if (minQuality && quality < minQuality) {
                bitFlags.at<uchar>(pixel.row, pixel.col) |= POSTPONED;
                return;
            }
            // otherwise, insert in list in order from lowest to highest quality
            else {
                OrderedInsert(pixelList, pixel);
                bitFlags.at<uchar>(pixel.row, pixel.col) |= UNWRAPPED;
                bitFlags.at<uchar>(pixel.row, pixel.col) &= (~POSTPONED);
            }
        }

        void OrderedInsert(vector<QualityPixel>& pixels, QualityPixel value) {
            // Find proper position in descending order & insert before iterator it
            vector<QualityPixel>::iterator it = lower_bound(pixels.begin(), pixels.end(), value, greater<QualityPixel>());
            pixels.insert(it, value);
        }

        float Gradient(float p1, float p2) {
            float grad = p1 - p2;
            if (grad > CV_PI)
                grad -= CV_2PI;
            if (grad < -CV_PI)
                grad += CV_2PI;
            return grad;
        }
    } // namespace
} // namespace phase_unwrapping