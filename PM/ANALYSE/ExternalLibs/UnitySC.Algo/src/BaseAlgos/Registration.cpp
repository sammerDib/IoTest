#include <opencv2/opencv.hpp>

#include <BaseAlgos/2DSignalAnalysis.hpp>
#include <BaseAlgos/EdgeDetector.hpp>
#include <BaseAlgos/Registration.hpp>
#include <Logger.hpp>

using namespace std;
using namespace cv;

namespace registration {

    namespace {
      /**
       * Preprocess image
       *
       * @param refImgPreprocessed           - reference image to preprocess
       * @param sensedImgPreprocessed        - image to align to preprocess
       * @param roi                          - rectangle to select the ROI on reference img
       */
        void Preprocess(Mat& refImgPreprocessed, Mat& sensedImgPreprocessed, const Rect& roi = Rect());

        /**
         * Extract the translation component from an affine transformation matrix
         *
         * @param  affine - affine transformation matrix
         *
         * @return the translation associated
         */
        Point2f ExtractTranslationFromAffine(const Mat& affine);

        /**
         * Fourier-Mellin based registration : compute the shift between reference and sensed images
         *
         * @param  refImg        - ref image
         * @param  sensedImg     - sensed image
         *
         * @return the angle and shift vector
         */
        pair<double, Point2f> FourierMellinTransform(const Mat& refImg, const Mat& sensedImg);
    } // namespace


    pair<double, Point2f> ComputeAngleAndShift(const Mat& refImg, const Mat& sensedImg, const Rect& roi) {
        // Edge detection
        Mat ref = refImg.clone();
        Mat sensed = sensedImg.clone();
        Preprocess(ref, sensed, roi);

        // Compute fmt
        return FourierMellinTransform(ref, sensed);
    }

    double ComputeSimilarity(const Mat& refImg, const Mat& sensedImg, const Rect& roi) {
        // Take into account the ROI
        Mat img1;
        Mat img2;
        if (!roi.empty()) {
            img1 = refImg(roi).clone();
            img2 = sensedImg(roi).clone();
        } else {
            img1 = refImg;
            img2 = sensedImg;
        }

        // MatchTemplate of two images that are the same size, returns a single value : a measure of similarity.
        Mat scoreImg;
        double maxScore;
        matchTemplate(img1, img2, scoreImg, TM_CCOEFF_NORMED);
        minMaxLoc(scoreImg, 0, &maxScore);
        return maxScore;
    }

    Mat ImgRegistration(const Mat& refImg, const Mat& sensedImg, const Mat& transformation) {
        if (transformation.rows == 2 && transformation.cols == 3) {
            // Use affine transformation matrix to realign sensed image
            Mat imgRealigned;
            warpAffine(sensedImg, imgRealigned, transformation, sensedImg.size());

            // replace the borders due to realignment (for which we have no information) with the corresponding areas in the reference image (so as not to compromise the similarity measurement)
            Point2f tr = ExtractTranslationFromAffine(transformation);
            int roiX = (tr.x > 0) ? ceil(tr.x) : 0;
            int roiY = (tr.y > 0) ? ceil(tr.y) : 0;
            int roiCols = (tr.x > 0) ? sensedImg.cols - ceil(tr.x) : sensedImg.cols - 1 + floor(tr.x);
            int roiRows = (tr.y > 0) ? sensedImg.rows - ceil(tr.y) : sensedImg.rows - 1 + floor(tr.y);
            int f = floor(tr.y);
            int ff = floor(tr.x);
            int fff = ceil(tr.x);
            int ffff = ceil(tr.y);

            Rect roi = Rect(roiX, roiY, roiCols, roiRows);
            Mat mask = Mat::zeros(refImg.rows, refImg.cols, CV_8U); // all 0
            mask(roi) = 255;
            Mat realignImgInterpolated = refImg.clone();
            imgRealigned.copyTo(realignImgInterpolated, mask);

            return realignImgInterpolated;
        } else {
            return sensedImg;
        }
    }

    namespace {
        void Preprocess(Mat& refImg, Mat& sensedImg, const Rect& roi) {
            if (!roi.empty()) {
                refImg = refImg(roi);
                sensedImg = sensedImg(roi);
            }
        }

        Point2f ExtractTranslationFromAffine(const Mat& affine) {
            if (affine.rows != 2 || affine.cols != 3) {
                stringstream strStrm;
                strStrm << "[ExtractTranslationFromAffine] Affine matrix must contain 2 rows and 3 cols.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

            // The general rule for Matrix typenames in OpenCV is: CV_<bit_depth>(S|U|F)C<number_of_channels>
            // S = Signed integer
            // U = Unsigned integer
            // F = Float
            switch (affine.depth()) {
            case CV_8U:
                return Point2f(affine.at<uint8_t>(0, 2), affine.at<uint8_t>(1, 2));
            case CV_8S:
                return Point2f(affine.at<int8_t>(0, 2), affine.at<int8_t>(1, 2));
            case CV_16U:
                return Point2f(affine.at<uint16_t>(0, 2), affine.at<uint16_t>(1, 2));
            case CV_16S:
                return Point2f(affine.at<int16_t>(0, 2), affine.at<int16_t>(1, 2));
            case CV_32S:
                return Point2f(affine.at<int32_t>(0, 2), affine.at<int32_t>(1, 2));
            case CV_32F:
                return Point2f(affine.at<float>(0, 2), affine.at<float>(1, 2));
            case CV_64F:
                return Point2f(affine.at<double>(0, 2), affine.at<double>(1, 2));
            default:
                return Point2f(NAN, NAN);
            }
        }

        pair<double, Point2f> FourierMellinTransform(const Mat& refImg, const Mat& sensedImg) {
            /**
             * Fourier-Mellin Transformation:
             *  - 1. Discrete Fourier Transform (DFT) to convert images into frequency domain.
             *  - 2. Smoothing and hight-pass filter to avoid the "plus" artifact caused by borders and aliasing artifacts due to rotation.
             *  - 3. Log-polar transform to convert rotation and scaling in the Cartesian coordinate system to translations in the log-polar coordinate system.
             *  - 4. First phase correlation to estimate the rotation and scale difference between the two input images.
             *  - 5. The sensed image is then rotated and scaled to match the second image.
             *  - 6. Second phase correlation to find the translational offset between previous computed image and ref image.
             *
             * It is necessary to first de-rotate and de-scale the image before finding the translational offset.
             * This is because the recovery of translation is not invariant to rotation and scale, whereas the recovery of rotation and scale is invariant to
             * translation. It's the aim of the first phase correlation, done done after first representing the rotation and scaling as translations using the
             * log-polar transform of the magnitude of the Fourier transforms of the images.
             */
            if (refImg.empty() || sensedImg.empty()) {
                stringstream strStrm;
                strStrm << "[FourierMellinTransform] Reference and sensed images must not be empty.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

            if (refImg.size() != sensedImg.size()) {
                stringstream strStrm;
                strStrm << "[FourierMellinTransform] Reference and sensed images must have the same size.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

            Mat block_a = refImg.clone();
            Mat block_b = sensedImg.clone();

            if (block_a.channels() == 3) {
                cvtColor(block_a, block_a, COLOR_BGR2GRAY);
            }
            if (block_b.channels() == 3) {
                cvtColor(block_b, block_b, COLOR_BGR2GRAY);
            }
            if (block_a.type() == CV_8UC1) {
                block_a.convertTo(block_a, CV_64FC1, 1.0 / 255.0);
            }
            if (block_b.type() == CV_8UC1) {
                block_b.convertTo(block_b, CV_64FC1, 1.0 / 255.0);
            }
            if (block_a.type() == CV_32FC1) {
                block_a.convertTo(block_a, CV_64FC1);
            }
            if (block_b.type() == CV_32FC1) {
                block_b.convertTo(block_b, CV_64FC1);
            }

            CV_Assert(block_a.type() == CV_32FC1 || block_a.type() == CV_64FC1);
            CV_Assert(block_b.type() == CV_32FC1 || block_b.type() == CV_64FC1);

            // 1- Discrete Fourier Transform (DFT) to convert images into frequency domain (apply hanning windows to avoid the “plus” artifact caused by borders)
            Mat hann_a = signal_2D::HanningApodization(block_a);
            Mat hann_b = signal_2D::HanningApodization(block_b);
            Mat ft_a = signal_2D::DiscretFourierTransform(hann_a);
            Mat ft_b = signal_2D::DiscretFourierTransform(hann_b);

            // 2- Log-polar transform to convert rotation and scaling in the Cartesian coordinate system to translations in the log-polar coordinate system
            // The edges of the square inset cause very important contrast in the polar transform, the correlation would pay much more attention to that than to the actual image content.
            // We should ignore the rightmost ~60% of the polar transformed images, which only contains the edges of the square image and the stuff in the corners of the image that are not very important.
            // That is, look for rotation only in a circular part of the image.
            Mat lp_a, lp_b;
            Point2f center(ft_a.size() / 2);
            double radius = (double)ft_a.rows / 4;
            double M = (double)ft_a.cols / log(radius);
            logPolar(ft_a, lp_a, center, M, INTER_LINEAR + WARP_FILL_OUTLIERS);
            logPolar(ft_b, lp_b, center, M, INTER_LINEAR + WARP_FILL_OUTLIERS);

            // 3- Log-polar phase correlation to estimate the rotation and scale difference between the two input images.
            Point2f peakPosition = phaseCorrelate(lp_a, lp_b);

            // 4- Rotate and scale the first (reference) image to match the second (sensed) image.
            double angle_in_degrees_anticlockwize = -peakPosition.y * 360.0 / block_a.rows;
            if (abs(angle_in_degrees_anticlockwize) > 90) {
                angle_in_degrees_anticlockwize += 180;
            }
            double angle_in_degrees_clockwize = fmod(-angle_in_degrees_anticlockwize, 360);
            double scale = 1.0 / exp(peakPosition.x / M);

            Mat transformMat = getRotationMatrix2D(center, angle_in_degrees_clockwize, scale);
            Mat block_b_rs;
            warpAffine(block_b, block_b_rs, transformMat, block_b.size());

            // 5- final phase correlation used to find the translational offset between rotate and scale sensed image and the reference image (apply hanning windows to avoid the “plus” artifact caused by borders)
            Mat hann_b_rs = signal_2D::HanningApodization(block_b_rs);

            Point2f shift = phaseCorrelate(hann_a, hann_b_rs);
            return pair(angle_in_degrees_clockwize, shift);
        }
    } // namespace
};  // namespace registration