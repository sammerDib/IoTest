#include <opencv2/opencv.hpp>

#include "CEdgeDetector.hpp"
#include "CRegistration.hpp"
#include "ErrorLogging.hpp"

#include "CFilters.hpp"
#include "FourierTransform.hpp"

#pragma unmanaged
using namespace std;
using namespace cv;

namespace registration {
    namespace {
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
         * @param  roi           - roi on the ref image
         * @param reportPath     - path to report images
         * @param angleSigmaTolerance - Tolerance sigma in degrees in which the rotation angle is considered acceptable ( angles values too divergent from the sigma will be forced to 0.0)
         * @param scaleSigmaTolerance - Tolerance sigma in which the scale is considered acceptable ( scale values too divergent from the sigma will be forced to 1.0)
         * 
         * @return the angle, shift vector and confidence
         */
        tuple<double, Point2f, double> FourierMellinTransform(const Mat& refImg, const Mat& sensedImg, const Rect&roi = Rect(), double angleSigmaTolerance = _defaultAngleSigmaTolerance, double scaleSigmaTolerance = _defaultScaleSigmaTolerance, int dilationSize = 0, std::string reportPath = "");
    } // namespace

    tuple<double, Point2f, double> ComputeAngleAndShift(const Mat& refImg, const Mat& sensedImg, const Rect& roi, double angleSigmaTolerance, double scaleSigmaTolerance, int dilationSize, std::string reportPath) {
        if (!roi.empty()) {
            cv::Mat mask = cv::Mat::zeros(refImg.rows, refImg.cols, CV_8U);
            mask(roi) = 255;
            cv::Mat maskedRefImg = cv::Mat::ones(refImg.rows, refImg.cols, CV_8U);
            refImg.copyTo(maskedRefImg, mask);

            return FourierMellinTransform(maskedRefImg, sensedImg, roi, angleSigmaTolerance, scaleSigmaTolerance, dilationSize, reportPath);
        }

        return FourierMellinTransform(refImg, sensedImg, roi, angleSigmaTolerance, scaleSigmaTolerance, dilationSize, reportPath);
    }

    double ShiftImagesAndComputeSimilarity(const Mat& refImg, const Mat& sensedImg, Point2f shift, const Rect& roi, int dilationSize, std::string reportPath)
    {
        Mat convertedRefImg = refImg.clone();
        Mat convertedSensedImg = sensedImg.clone();

        if (convertedRefImg.channels() == 3) {
            cvtColor(convertedRefImg, convertedRefImg, COLOR_BGR2GRAY);
        }
        if (convertedSensedImg.channels() == 3) {
            cvtColor(convertedSensedImg, convertedSensedImg, COLOR_BGR2GRAY);
        }

        if (convertedRefImg.type() != CV_8UC1) {
            convertedRefImg.convertTo(convertedRefImg, CV_8UC1, 255.0);
        }
        if (convertedSensedImg.type() != CV_8UC1) {
            convertedSensedImg.convertTo(convertedSensedImg, CV_8UC1, 255.0);
        }

        cv::Mat shift_mat = (cv::Mat_<double>(2, 3) << 1, 0, shift.x, 0, 1, shift.y);
        cv::Mat realignedImgEdge = registration::ImgRegistration(convertedRefImg, convertedSensedImg, shift_mat);

        // roi to make sure to crop the unwanted black pixels where both images don't overlap
        int roiX = (shift.x > 0.0f) ? (int)ceil(shift.x) : 0;
        int roiY = (shift.y > 0.0f) ? (int)ceil(shift.y) : 0;
        int roiCols = (shift.x > 0.0f) ? refImg.cols - (int)ceil(shift.x) : refImg.cols - 1 + (int)floor(shift.x);
        int roiRows = (shift.y > 0.0f) ? refImg.rows - (int)ceil(shift.y) : refImg.rows - 1 + (int)floor(shift.y);

        cv::Rect areaWithUsefulData = cv::Rect(roiX, roiY, roiCols, roiRows);
        cv::Mat mask = cv::Mat::zeros(refImg.rows, refImg.cols, CV_8U);
        mask(areaWithUsefulData) = 255;

        cv::Mat refImgPreprocessed = cv::Mat::ones(refImg.rows, refImg.cols, CV_8U);
        convertedRefImg.copyTo(refImgPreprocessed, mask);

        cv::Mat realignedImgPreprocessed = cv::Mat::ones(refImg.rows, refImg.cols, CV_8U);
        realignedImgEdge.copyTo(realignedImgPreprocessed, mask);

        int t1 = refImgPreprocessed.type();
        int t2 = realignedImgPreprocessed.type();

        return ComputeSimilarity(refImgPreprocessed, realignedImgPreprocessed, roi, dilationSize, reportPath);
    }

    double ComputeSimilarity(const Mat& refImg, const Mat& sensedImg, const Rect& roi, int dilationSize, std::string reportPath) {

        // Take into account the ROI
        Mat img1;
        Mat img2;
        if (!roi.empty()) {
            img1 = refImg(roi).clone();
            img2 = sensedImg(roi).clone();
        }
        else {
            img1 = refImg;
            img2 = sensedImg;
        }

        if (img1.channels() == 3) {
            cvtColor(img1, img1, COLOR_BGR2GRAY);
        }
        if (img2.channels() == 3) {
            cvtColor(img2, img2, COLOR_BGR2GRAY);
        }

        // Removing the noise on both image since we will be comparing them pixel by pixel
        filter::NoiseRemovedBelowThreshold(img1);
        filter::NoiseRemovedBelowThreshold(img2);

        if (!reportPath.empty())
        {
            imwrite(reportPath + "/compareRef.png", img1);
            imwrite(reportPath + "/compareSensed.png", img2);
        }

        // Edge case where both images are black, we don't want a high confidence there
        if (countNonZero(img1) == 0 || countNonZero(img2) == 0) return 0.0;

        Mat refMask = cv::Mat();

        if (dilationSize > 0)
        {
            threshold(img1, refMask, 1, 255, cv::THRESH_BINARY);

            dilate(refMask, refMask, getStructuringElement(MORPH_ELLIPSE, Size(dilationSize, dilationSize)));
        }

        // MatchTemplate of two images that are the same size, returns a single value : a measure of similarity.
        Mat scoreImg;
        double maxScore;

        matchTemplate(img2, img1, scoreImg, TM_CCOEFF_NORMED, refMask);
        minMaxLoc(scoreImg, 0, &maxScore);
        return maxScore;
    }

    Mat ImgRegistration(const Mat& refImg, const Mat& sensedImg, const Mat& transformation) {
        if (transformation.rows == 2 && transformation.cols == 3)
        {
            // Use affine transformation matrix to realign sensed image
            Mat imgRealigned;
            warpAffine(sensedImg, imgRealigned, transformation, sensedImg.size());

            return imgRealigned;
        }
        else {
            return sensedImg;
        }
    }

    namespace {
        Point2f ExtractTranslationFromAffine(const Mat& affine) {
            if (affine.rows != 2 || affine.cols != 3)
                ErrorLogging::LogErrorAndThrow("[ExtractTranslationFromAffine] Affine matrix must contain 2 rows and 3 cols: ", affine.rows, " rows and ", affine.cols, " cols found");

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
                return Point2f((float)affine.at<int32_t>(0, 2), (float)affine.at<int32_t>(1, 2)); //possible loss of data
            case CV_32F:
                return Point2f(affine.at<float>(0, 2), affine.at<float>(1, 2));
            case CV_64F:
                return Point2f((float)affine.at<double>(0, 2), (float)affine.at<double>(1, 2)); //possible loss of data
            default:
                return Point2f(NAN, NAN);
            }
        }

        tuple<double, Point2f, double> FourierMellinTransform(const Mat& refImg, const Mat& sensedImg, const Rect& roi, double angleSigmaTolerance, double scaleSigmaTolerance, int dilationSize, std::string reportPath) {
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
            if (refImg.empty() || sensedImg.empty())
                ErrorLogging::LogErrorAndThrow("[FourierMellinTransform] Reference and sensed images must not be empty.");

            if (refImg.size() != sensedImg.size())
                ErrorLogging::LogErrorAndThrow("[FourierMellinTransform] Reference and sensed images must have the same size: ", refImg.size(), " != ", sensedImg.size());

            Mat convertedRefImg = refImg.clone();
            Mat convertedSensedImg = sensedImg.clone();

            if (convertedRefImg.channels() == 3) {
                cvtColor(convertedRefImg, convertedRefImg, COLOR_BGR2GRAY);
            }
            if (convertedSensedImg.channels() == 3) {
                cvtColor(convertedSensedImg, convertedSensedImg, COLOR_BGR2GRAY);
            }
            if (convertedRefImg.type() == CV_8UC1) {
                convertedRefImg.convertTo(convertedRefImg, CV_64FC1, 1.0 / 255.0);
            }
            if (convertedSensedImg.type() == CV_8UC1) {
                convertedSensedImg.convertTo(convertedSensedImg, CV_64FC1, 1.0 / 255.0);
            }
            if (convertedRefImg.type() == CV_32FC1) {
                convertedRefImg.convertTo(convertedRefImg, CV_64FC1);
            }
            if (convertedSensedImg.type() == CV_32FC1) {
                convertedSensedImg.convertTo(convertedSensedImg, CV_64FC1);
            }

            // 1- Discrete Fourier Transform (DFT) to convert images into frequency domain (apply hanning windows to avoid the “plus” artifact caused by borders)
            Mat refImgDFT = fourier_transform::DiscretFourierTransform(convertedRefImg);
            Mat sensedImgDFT = fourier_transform::DiscretFourierTransform(convertedSensedImg);
            Mat refMagnitudeSpectrum = fourier_transform::LogMagnitudeSpectrum(refImgDFT);
            Mat sensedMagnitudeSpectrum = fourier_transform::LogMagnitudeSpectrum(sensedImgDFT);

            // resizing the image so that the DFT are square images
            resize(refMagnitudeSpectrum, refMagnitudeSpectrum, Size(refMagnitudeSpectrum.rows, refMagnitudeSpectrum.rows), INTER_LINEAR);
            resize(sensedMagnitudeSpectrum, sensedMagnitudeSpectrum, Size(sensedMagnitudeSpectrum.rows, sensedMagnitudeSpectrum.rows), INTER_LINEAR);

            normalize(refMagnitudeSpectrum, refMagnitudeSpectrum, 0, 1, NORM_MINMAX);
            normalize(sensedMagnitudeSpectrum, sensedMagnitudeSpectrum, 0, 1, NORM_MINMAX);

            // 2- Log-polar transform to convert rotation and scaling in the Cartesian coordinate system to translations in the log-polar coordinate system
            // The edges of the square inset cause very important contrast in the polar transform, the correlation would pay much more attention to that than to the actual image content.
            // We should ignore the rightmost ~60% of the polar transformed images, which only contains the edges of the square image and the stuff in the corners of the image that are not very important.
            // That is, look for rotation only in a circular part of the image.
            Mat refMagnitudeLogPolar, sensedMagnitudeLogPolar;
            Point2f center(refMagnitudeSpectrum.size() / 2);
            double radius = (double)refMagnitudeSpectrum.rows / 4;
            double M = (double)refMagnitudeSpectrum.cols / log(radius);
            logPolar(refMagnitudeSpectrum, refMagnitudeLogPolar, center, M, INTER_LINEAR + WARP_FILL_OUTLIERS);
            logPolar(sensedMagnitudeSpectrum, sensedMagnitudeLogPolar, center, M, INTER_LINEAR + WARP_FILL_OUTLIERS);

            // 3- Log-polar phase correlation to estimate the rotation and scale difference between the two input images.
            Point2f peakPosition = phaseCorrelate(refMagnitudeLogPolar, sensedMagnitudeLogPolar);

            // 4- Rotate and scale the first (reference) image to match the second (sensed) image.
            double angleInDegreesAnticlockwize = -peakPosition.y * 360.0 / refMagnitudeLogPolar.rows;
            if (abs(angleInDegreesAnticlockwize) > 90) {
                angleInDegreesAnticlockwize += 180;
            }
            double angleIndegreesClockwize = fmod(-angleInDegreesAnticlockwize, 360);
            double scale = exp(peakPosition.x / M);

            if (!(angleIndegreesClockwize < angleSigmaTolerance) || !(angleIndegreesClockwize > -angleSigmaTolerance))
            {
                angleIndegreesClockwize = 0.0;
            }

            if ((scale < 1.0 - scaleSigmaTolerance) || (scale > 1.0 + scaleSigmaTolerance))
            {
                scale = 1.0;
            }

            Point2f origCenter(convertedSensedImg.size() / 2);
            Mat transformMat = getRotationMatrix2D(origCenter, angleIndegreesClockwize, scale);
            Mat warpedSensedImg;
            warpAffine(convertedSensedImg, warpedSensedImg, transformMat, convertedSensedImg.size());

            // 5- final phase correlation used to find the translational offset between rotate and scale sensed image and the reference image (warpedShift)

            Point2f warpedShift = phaseCorrelate(convertedRefImg, warpedSensedImg);

            Point2f positionOrignRefInSensedRotated = warpedShift + origCenter;

            // Finding the final shift between the original unrotated sensed image and the original reference image
            Mat invTransformMat;
            invertAffineTransform(transformMat, invTransformMat);
            Point2f positionOrignRefInSensed;
            // Same thing as the warpAffine method but on 1 single point
            // not rti : ici mélneg de calcul sur float et sur double !! uniformiser celà
#pragma warning(push)
#pragma warning (disable: 4244) // initializing': conversion from 'float' to 'double', possible loss of data
            positionOrignRefInSensed.x = invTransformMat.at<double>(0, 0) * positionOrignRefInSensedRotated.x + invTransformMat.at<double>(0, 1) * positionOrignRefInSensedRotated.y + invTransformMat.at<double>(0, 2);
            positionOrignRefInSensed.y = invTransformMat.at<double>(1, 0) * positionOrignRefInSensedRotated.x + invTransformMat.at<double>(1, 1) * positionOrignRefInSensedRotated.y + invTransformMat.at<double>(1, 2);
#pragma warning(pop)
            Point2f finalShift = positionOrignRefInSensed - origCenter;

            // If the roi is decentered, we calculate its centered in the rotated sensed image to get the actual shift
            if (!roi.empty())
            {
                Point2f positionOrignRois = Point2f(static_cast<float>(roi.x + roi.width / 2), static_cast<float>(roi.y + roi.height / 2));
                Point2f positionOrignRoiInSensed;
                // Same thing as the warpAffine method but on 1 single point
#pragma warning(push)
#pragma warning (disable: 4244) // initializing': conversion from 'float' to 'double', possible loss of data
                positionOrignRoiInSensed.x = invTransformMat.at<double>(0, 0) * positionOrignRois.x + invTransformMat.at<double>(0, 1) * positionOrignRois.y + invTransformMat.at<double>(0, 2);
                positionOrignRoiInSensed.y = invTransformMat.at<double>(1, 0) * positionOrignRois.x + invTransformMat.at<double>(1, 1) * positionOrignRois.y + invTransformMat.at<double>(1, 2);
#pragma warning(pop)
                finalShift -= positionOrignRois - positionOrignRoiInSensed;
            }

            // We use the rotated shift to compute the similarity since the rotated sensed and original reference image should be lined up
            double confidence = ShiftImagesAndComputeSimilarity(refImg, warpedSensedImg, -warpedShift, roi, dilationSize, reportPath);

            if (!reportPath.empty())
            {
                Mat refLogCheck, sensedLogCheck, refMagCheck, sensedMagCheck, warpedSensedCheck;

                normalize(refMagnitudeSpectrum, refMagCheck, 0, 255, NORM_MINMAX);
                normalize(sensedMagnitudeSpectrum, sensedMagCheck, 0, 255, NORM_MINMAX);
                normalize(refMagnitudeLogPolar, refLogCheck, 0, 255, NORM_MINMAX);
                normalize(sensedMagnitudeLogPolar, sensedLogCheck, 0, 255, NORM_MINMAX);
                normalize(warpedSensedImg, warpedSensedCheck, 0, 255, NORM_MINMAX);

                imwrite(reportPath + "/logRef.png", refLogCheck);
                imwrite(reportPath + "/logSensed.png", sensedLogCheck);
                imwrite(reportPath + "/dftRef.png", refMagCheck);
                imwrite(reportPath + "/dftSensed.png", sensedMagCheck);
                imwrite(reportPath + "/warpedSensed.png", warpedSensedCheck);
            }

            return std::make_tuple(angleIndegreesClockwize, finalShift, confidence);
        }
    } // namespace
};  // namespace registration