#include "PhaseMappingWithResidualFringeRemoving.hpp"
#include "CFilters.hpp"
#include "FourierTransform.hpp"
#include "LoggerOCV.hpp"
#include "PhaseMapping.hpp"
#include "CImageTypeConvertor.hpp"
#include "Utils.hpp"

#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>

using namespace std;
using namespace cv;

#pragma unmanaged
namespace residual_fringe_removing {
    namespace {
        /**
         * @brief Calculate the average magnitude spectrum from an images set.
         * This spectral analysis of periodic signals allows you to highlight a periodic pattern repeated through the different images.
         *
         * @param imgs - images with a similar periodic pattern
         * @return The magnitude spectrum in which the quadrants have been rearranged so that the origin is at the center of the image.
         */
        Mat ComputeAveragedMagnitudeSpectrum(const vector<Mat>& imgs, int stepNb);

        /**
         * @brief Find the position in the frequency spectrum of the maximum value excluding the origin.
         *
         * @param spectrum - spectrum in which the quadrants have been rearranged so that the origin is at the center of the image.
         * @return the maxmimum value position on spectrum
         */
        Point FindMaxValuePositionOnFrequencySpectrum(const Mat& spectrum);

        /**
         * @brief Calculate the magnitude and phase value at a given location in the spectrum, for a given image list.
         * @param imgs - images of interest
         * @param positionInSpectrum - position (x,y) of interest in the frequency spectrum
         * @return The list of magnitude and phase values at position of interest ​​for each image
         */
        pair<vector<double>, vector<double>> ComputeMagnitudeAndPhaseAtPositionOfInterest(const vector<Mat>& imgs, const Point& positionInSpectrum);

        /**
         * @brief Calculate the wrapped phase map from a list of interferograms on which appear interference fringes, the magnitude of these fringes and the phase of these fringes.
         *
         * @details The calculation consists of solving the linear equation : Intensity(x,y) = b(x,y) + a1 * Magnitude * cos(Phase) - a2 * Magnitude * sin(Phase)
         * The intensity corresponds to the value of the pixel on the interferograms.
         * The Magnitude and the Phase correspond to the magnitude and phase of the interference fringe of the interferograms.
         * We must solve equation with three unknown variables: b, a1, a2.
         * To solve this equation, we need at least three interferograms acquired at different steps (phase shift) and their associated amplitude and phase.
         *
         * @param interferograms - At least three interferograms acquired at different steps (phase shift).
         * @param magnitudes - Magnitude of the interference pattern associated with each interferogram.
         * @param phase - Phase of the interference pattern associated with each interferogram.
         * @return The wrapped phase map generated from the input data.
         */
        WrappedPhaseMap ComputePhaseMap(const vector<Mat>& interferograms, const vector<double>& magnitudes, const vector<double>& phase);
    }

    WrappedPhaseMap WrappedPhaseMapWithoutResidualFringe(vector<Mat>& imgs, int stepNb, filesystem::path directoryPathToStoreReport)
    {
        if (stepNb < 3) {
            stringstream strStrm;
            strStrm << "[Wrapped phase map without residual fringe] The number of steps must be at least 3.";
            string message = strStrm.str();
            LoggerOCV::Error(message);
            throw invalid_argument(message.c_str());
        }

        if (imgs.size() % stepNb != 0) {
            stringstream strStrm;
            strStrm << "[Wrapped phase map without residual fringe] The interferograms count must be multiple of the number of steps.";
            string message = strStrm.str();
            LoggerOCV::Error(message);
            throw invalid_argument(message.c_str());
        }

        // average all images from the same step
        vector<Mat> interferogramsCV32F = Convertor::ConvertAllToCV32FC1(imgs);
        vector<Mat> averagedImgPerStep = AverageInterferogramsPerStep(interferogramsCV32F, stepNb);

        Mat averagedMagnitudeSpectrum = ComputeAveragedMagnitudeSpectrum(averagedImgPerStep, stepNb);

        Point locationOfInterferenceFringeMagnitude = FindMaxValuePositionOnFrequencySpectrum(averagedMagnitudeSpectrum);

        // Display -------------------------------------------------------------------------------
        //averagedMagnitudeSpectrum.at<float>(locationOfInterferenceFringeMagnitude.y, locationOfInterferenceFringeMagnitude.x) = 0;
        //Mat averagedMagnitudeSpectrumNormalized;
        //normalize(averagedMagnitudeSpectrum, averagedMagnitudeSpectrumNormalized, 1, 0, NORM_INF);
        //averagedMagnitudeSpectrumNormalized.convertTo(averagedMagnitudeSpectrumNormalized, CV_8U, 255);
        //imshow("Averaged magnitude with max located", averagedMagnitudeSpectrumNormalized);
        //waitKey();
        // ---------------------------------------------------------------------------------------

        pair<vector<double>, vector<double>> paramsOfInterferenceFringe = ComputeMagnitudeAndPhaseAtPositionOfInterest(interferogramsCV32F, locationOfInterferenceFringeMagnitude);
        vector<double> interferenceFringeMagnitudePerStep = paramsOfInterferenceFringe.first;
        vector<double> interferenceFringePhasePerStep = paramsOfInterferenceFringe.second;

        WrappedPhaseMap wrappedPhaseMap = ComputePhaseMap(interferogramsCV32F, interferenceFringeMagnitudePerStep, interferenceFringePhasePerStep);

        // Display -------------------------------------------------------------------------------
        //imshow("WrappedPhase", wrappedPhaseMap.WrappedPhase);
        //imshow("Background", wrappedPhaseMap.Background);
        //waitKey();
        // ---------------------------------------------------------------------------------------

        return wrappedPhaseMap;
    }

    namespace {
        Mat ComputeAveragedMagnitudeSpectrum(const vector<Mat>& imgs, int stepNb)
        {
            vector<Mat> magnitudeAtEachStep;
            for (Mat img : imgs)
            {
                // apply hanning windows to avoid the “plus” artifact caused by borders
                Mat averagedImgWithHann = filter::HanningApodization(img);

                // compute real part of fourier transform
                Mat complexMatrix = fourier_transform::DiscretFourierTransform(averagedImgWithHann);
                Mat magnitude = fourier_transform::MagnitudeSpectrum(complexMatrix);

                magnitudeAtEachStep.push_back(magnitude);
            }

            // accumulate all magnitude spectrums
            Mat averagedMagnitude(magnitudeAtEachStep[0].size(), magnitudeAtEachStep[0].type(), Scalar(0));
            for (Mat magnitude : magnitudeAtEachStep)
            {
                accumulate(magnitude, averagedMagnitude);
            }
            averagedMagnitude / stepNb;

            return averagedMagnitude;
        }

        Point FindMaxValuePositionOnFrequencySpectrum(const Mat& spectrum)
        {
            Mat spectrumCloned = spectrum.clone();
            Mat croppedSpectrum = spectrumCloned(Range(spectrum.rows / 2, spectrum.rows), Range(0, spectrum.cols));
            croppedSpectrum.at<float>(0, spectrum.cols / 2) = 0;

            double minVal;
            double maxVal;
            Point minLoc;
            Point maxLoc;
            minMaxLoc(croppedSpectrum, &minVal, &maxVal, &minLoc, &maxLoc);

            Point maxLocOnUncroppedSpectrum = Point(maxLoc.x, spectrum.rows / 2 + maxLoc.y);

            return maxLocOnUncroppedSpectrum;
        }

        pair<vector<double>, vector<double>> ComputeMagnitudeAndPhaseAtPositionOfInterest(const vector<Mat>& imgs, const Point& positionInSpectrum)
        {
            vector<double> maxMagnitudes;
            vector<double> maxPhases;

            for (Mat img : imgs)
            {
                // apply hanning windows to avoid the “plus” artifact caused by borders
                Mat hann = filter::HanningApodization(img);

                // compute fourier transform and split it into magnitude and phase
                Mat complexMatrix = fourier_transform::DiscretFourierTransform(hann);
                Mat magnitude = fourier_transform::MagnitudeSpectrum(complexMatrix);
                Mat phase = fourier_transform::PhaseSpectrum(complexMatrix);

                maxPhases.push_back(phase.at<float>(positionInSpectrum.y, positionInSpectrum.x));
                float magnitudeNormalizedBySize = magnitude.at<float>(positionInSpectrum.y, positionInSpectrum.x) / (img.cols * img.rows);
                maxMagnitudes.push_back(magnitudeNormalizedBySize);
            }

            return pair<vector<double>, vector<double>>(maxMagnitudes, maxPhases);
        }

        WrappedPhaseMap ComputePhaseMap(const vector<Mat>& interferograms, const vector<double>& magnitudes, const vector<double>& phase)
        {
            int ImgNb = (int)interferograms.size();

            // Solve linear equation : B = Ax
            // IntensityImg1(x,y) = background(x,y) + AmplitudeImg1 * a1(x,y) * cos(PhaseImg1) - AmplitudeImg1 * a2(x,y) * sin(PhaseImg1)}
            // IntensityImg2(x,y) = background(x,y) + AmplitudeImg2 * a1(x,y) * cos(PhaseImg2) - AmplitudeImg2 * a2(x,y) * sin(PhaseImg2)}
            // IntensityImg3(x,y) = background(x,y) + AmplitudeImg3 * a1(x,y) * cos(PhaseImg3) - AmplitudeImg3 * a2(x,y) * sin(PhaseImg3)}
            // etc.

            // A =  [1]    [AmplitudeImg1 * cos(PhaseImg1)]    [- AplitudeImg1 * sin(PhaseImg1)]
            //      [1]    [AmplitudeImg2 * cos(PhaseImg2)]    [- AplitudeImg2 * sin(PhaseImg2)]
            //      [1]    [AmplitudeImg3 * cos(PhaseImg3)]    [- AplitudeImg3 * sin(PhaseImg3)]
            //      etc.
            Mat A = Mat::zeros(ImgNb, 3, CV_32FC1);

            // B =  [IntensityImg1(x,y)]
            //      [IntensityImg2(x,y)]
            //      [IntensityImg3(x,y)]
            //      etc.
            Mat B = Mat::zeros(ImgNb, 1, CV_32FC1);

            // x =  [background(x,y)]
            //      [a1(x,y)]
            //      [a2(x,y)]
            Mat x = Mat::zeros(3, 1, CV_32FC1);

            Mat wrappedPhase = Mat::zeros(interferograms[0].rows, interferograms[0].cols, CV_32FC1);
            Mat amplitude = Mat::zeros(interferograms[0].rows, interferograms[0].cols, CV_32FC1);
            Mat background = Mat::zeros(interferograms[0].rows, interferograms[0].cols, CV_32FC1);

            for (int i = 0; i < ImgNb; i++)
            {
                A.at<float>(i, 0) = 1.0f;
                A.at<float>(i, 1) = static_cast<float>(magnitudes[i] * cos(phase[i]));
                A.at<float>(i, 2) = static_cast<float>(-magnitudes[i] * sin(phase[i]));
            }

            for (int r = 0; r < interferograms[0].rows; r++)
            {
                for (int c = 0; c < interferograms[0].cols; c++)
                {
                    for (int i = 0; i < ImgNb; i++)
                    {
                        B.at<float>(i, 0) = interferograms[i].at<float>(r, c);
                    }

                    solve(A, B, x, DECOMP_QR);
                    int t = x.type();
                    float b = x.at<float>(0, 0);
                    float a1 = x.at<float>(1, 0);
                    float a2 = x.at<float>(2, 0);

                    wrappedPhase.at<float>(r, c) = atan2(a2, a1);
                    amplitude.at<float>(r, c) = (float) sqrt(pow(a2, 2) + pow(a1, 2));
                    background.at<float>(r, c) = b;
                }
            }

            return WrappedPhaseMap(wrappedPhase, amplitude, background);
        }
    }
}