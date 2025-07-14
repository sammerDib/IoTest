#include <vector>

#include "PhaseMappingWithHariharan.hpp"
#include "CImageTypeConvertor.hpp"
#include "ErrorLogging.hpp"
#include "WrappedPhaseMap.hpp"
#include "Utils.hpp"

using namespace std;
using namespace cv;

#pragma unmanaged
namespace hariharan {
    namespace {
        /**
         * Phase mapping algorithm based on 7 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs     - The interferometric images shifted by pi/2 at each step
         */
        WrappedPhaseMap Hariharan7Steps(vector<Mat> interferometricImgs);

        /**
         * Phase mapping algorithm based on 6 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step
         */
        WrappedPhaseMap Hariharan6Steps(vector<Mat> interferometricImg);

        /**
         * Phase mapping algorithm based on 5 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step
         */
        WrappedPhaseMap Hariharan5Steps(vector<Mat> interferometricImgs);

        /**
         * Phase mapping algorithm based on 4 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step
         */
        WrappedPhaseMap Hariharan4Steps(vector<Mat> interferometricImgs);

        /**
         * Phase mapping algorithm based on 3 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step.
         */
        WrappedPhaseMap Hariharan3Steps(vector<Mat> interferometricImgs);
    } // namespace

    WrappedPhaseMap HariharanPhaseMapping(vector<Mat> interferograms, int stepNb) {
        if (stepNb < 3 || stepNb > 7) {
            ErrorLogging::LogErrorAndThrow("[Phase mapping with Hariharan] The number of input interferograms must be within [3, 7]: ", interferograms.size(), " given");
        }

        if (interferograms.size() % stepNb != 0) {
            ErrorLogging::LogErrorAndThrow("[Phase mapping with Hariharan] The interferograms count must be multiple of the number of steps.");
        }

        // average all images from the same step
        vector<Mat> interferogramsCV32F = Convertor::ConvertAllToCV32FC1(interferograms);
        vector<Mat> averagedInterferogramsPerStep = AverageInterferogramsPerStep(interferogramsCV32F, stepNb);

        Size size = averagedInterferogramsPerStep.at(0).size();
        for (Mat img : averagedInterferogramsPerStep) {
            if (img.type() != CV_32FC1) {
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Input interferograms should all be stored into a single channel 32-bits float: ", img.type(), " image type given instead (see CV_MAKETYPE).");
            }
            if ((img.size() != size)) {
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Input interferograms must all be the same size: ", img.size(), " != ", size);
            }
        }

        WrappedPhaseMap wrappedPhaseMap;

        if (stepNb == 7) {
            wrappedPhaseMap = Hariharan7Steps(averagedInterferogramsPerStep);
        }
        else if (stepNb == 6) {
            wrappedPhaseMap = Hariharan6Steps(averagedInterferogramsPerStep);
        }
        else if (stepNb == 5) {
            wrappedPhaseMap = Hariharan5Steps(averagedInterferogramsPerStep);
        }
        else if (stepNb == 4) {
            wrappedPhaseMap = Hariharan4Steps(averagedInterferogramsPerStep);
        }
        else if (stepNb == 3) {
            wrappedPhaseMap = Hariharan3Steps(averagedInterferogramsPerStep);
        }

        if (wrappedPhaseMap.Phase.empty() || wrappedPhaseMap.Background.empty()) {
            stringstream strStrm;
            strStrm << "[Phase mapping] Algorithm failed to compute phase map.";
            string message = strStrm.str();
            LoggerOCV::Error(message);
            throw exception(message.c_str());
        }

        return wrappedPhaseMap;
    }

    namespace {
        WrappedPhaseMap Hariharan7Steps(vector<Mat> interferometricImgs) {
            if (interferometricImgs.size() != 7)
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Expected 7 interferograms to run Hariharan 7 steps algorithm: ", interferometricImgs.size(), " != 7");

            Size size = interferometricImgs.at(0).size();
            Mat amplitudeMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap = Mat::zeros(size, CV_32FC1);
            Mat backgroundMap = Mat::zeros(size, CV_32FC1);

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);
                    float I3 = interferometricImgs[3].at<float>(r, c);
                    float I4 = interferometricImgs[4].at<float>(r, c);
                    float I5 = interferometricImgs[5].at<float>(r, c);
                    float I6 = interferometricImgs[6].at<float>(r, c);

                    float num = 4.f * I1 - 8.f * I3 + 4.f* I5;
                    float denom = -I0 + 7.f * I2 - 7.f * I4 + I6;
                    float amplitude = static_cast<float>(1000.0 * sqrt(pow(num, 2) + pow(denom, 2) * 0.5));
                    float phase = -atan2(num, denom);
                    float background = 0.25f * (0.5f * I0 + 0.5f * I1 + 0.5f * I2 + I3 + 0.5f * I4 + 0.5f * I5 + 0.5f * I6);
                    amplitudeMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }

            return WrappedPhaseMap(phaseMap, amplitudeMap, backgroundMap);
        }

        WrappedPhaseMap Hariharan6Steps(vector<Mat> interferometricImgs) {
            if (interferometricImgs.size() != 6)
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Expected 6 interferograms to run Hariharan 6 steps algorithm: ", interferometricImgs.size(), " != 6");

            Size size = interferometricImgs.at(0).size();
            Mat amplitudeMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap = Mat::zeros(size, CV_32FC1);
            Mat backgroundMap = Mat::zeros(size, CV_32FC1);

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);
                    float I3 = interferometricImgs[3].at<float>(r, c);
                    float I4 = interferometricImgs[4].at<float>(r, c);
                    float I5 = interferometricImgs[5].at<float>(r, c);

                    float num = -3 * I1 + 4 * I3 - I5;
                    float denom = I0 - 4 * I2 + 3 * I4;
                    float amplitude = (float) sqrt(pow(num, 2) + pow(denom, 2));
                    float phase = -atan2(num, denom);
                    float background = 0.25f * (0.5f * I0 + 0.5f * I1 + I2 + I3 + 0.5f * I4 + 0.5f * I5);
                    amplitudeMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }

            return WrappedPhaseMap(phaseMap, amplitudeMap, backgroundMap);
        }

        WrappedPhaseMap Hariharan5Steps(vector<Mat> interferometricImgs) {
            if (interferometricImgs.size() != 5)
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Expected 5 interferograms to run Hariharan 5 steps algorithm: ", interferometricImgs.size(), " != 5");

            Size size = interferometricImgs.at(0).size();
            Mat amplitudeMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap = Mat::zeros(size, CV_32FC1);
            Mat backgroundMap = Mat::zeros(size, CV_32FC1);

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);
                    float I3 = interferometricImgs[3].at<float>(r, c);
                    float I4 = interferometricImgs[4].at<float>(r, c);

                    float num = 2 * (I1 - I3);
                    float denom = (2 * I2 - I4 - I0);
                    float amplitude = static_cast<float>(sqrt(pow(num, 2) + pow(denom, 2)));
                    float phase = -atan2(num, denom);
                    float background = 0.25f * (0.5f * I0 + I1 + I2 + I3 + 0.5f * I4);
                    amplitudeMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }

            return WrappedPhaseMap(phaseMap, amplitudeMap, backgroundMap);
        }

        WrappedPhaseMap Hariharan4Steps(vector<Mat> interferometricImgs) {
            if (interferometricImgs.size() != 4)
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Expected 4 interferograms to run Hariharan 4 steps algorithm: ", interferometricImgs.size(), " != 4");

            Size size = interferometricImgs.at(0).size();
            Mat amplitudeMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap = Mat::zeros(size, CV_32FC1);
            Mat backgroundMap = Mat::zeros(size, CV_32FC1);

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);
                    float I3 = interferometricImgs[3].at<float>(r, c);

                    float num = I1 - I3;
                    float denom = -I0 + I2;
                    float phase = -atan2(num, denom);
                    float amplitude = (float) sqrt(pow(num, 2) + pow(denom, 2));
                    float background = 0.25f * (I0 + I1 + I2 + I3);
                    amplitudeMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }

            return WrappedPhaseMap(phaseMap, amplitudeMap, backgroundMap);
        }

        WrappedPhaseMap Hariharan3Steps(vector<Mat> interferometricImgs) {
            if (interferometricImgs.size() != 3)
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Expected 3 interferograms to run Hariharan 3 steps algorithm: ", interferometricImgs.size(), " != 3");

            Size size = interferometricImgs.at(0).size();
            Mat amplitudeMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap = Mat::zeros(size, CV_32FC1);
            Mat backgroundMap = Mat::zeros(size, CV_32FC1);

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);

                    float num = I2 - I1;
                    float denom = I0 - I1;
                    float phase = -atan2(num, denom);
                    float amplitude = (float) sqrt(pow(num, 2) + pow(denom, 2));
                    float background = 0.3333333f * (I0 + I1 + I2);
                    amplitudeMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }

            return WrappedPhaseMap(phaseMap, amplitudeMap, backgroundMap);
        }
    } // namespace
} // namespace hariharan