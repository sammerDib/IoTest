#include <vector>

#include <Logger.hpp>
#include <Topography/PhaseMapping.hpp>
#include <ImageTypeConvertor.hpp>

using namespace std;
using namespace cv;

namespace phase_mapping {

    namespace {
      /**
       * Phase mapping algorithm based on 7 phase-shifted interferograms by pi / 2 at each step
       *
       * @param interferometricImgs     - The interferometric images shifted by pi/2 at each step
       * @param phaseMap                - The 2D wrapped phase map computed by this algorithm
       * @param intensityMap            - The 2D intensity map associated at the phase map, computed by this algorithm
       * @param backgroundMap           - The 2D background associated at the phase map, computed by this algorithm
       */
        void Hariharan7Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap);

        /**
         * Phase mapping algorithm based on 6 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step
         * @param phaseMap                 - The 2D wrapped phase map computed by this algorithm
         * @param intensityMap             - The 2D intensity map associated at the phase map, computed by this algorithm
         * @param backgroundMap            - The 2D background associated at the phase map, computed by this algorithm
         */
        void Hariharan6Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap);

        /**
         * Phase mapping algorithm based on 5 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step
         * @param phaseMap                 - The 2D wrapped phase map computed by this algorithm
         * @param intensityMap             - The 2D intensity map associated at the phase map, computed by this algorithm
         * @param backgroundMap            - The 2D background associated at the phase map, computed by this algorithm
         */
        void Hariharan5Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap);

        /**
         * Phase mapping algorithm based on 4 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step
         * @param phaseMap                 - The 2D wrapped phase map computed by this algorithm
         * @param intensityMap             - The 2D intensity map associated at the phase map, computed by this algorithm
         * @param backgroundMap            - The 2D background associated at the phase map, computed by this algorithm
         */
        void Hariharan4Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap);

        /**
         * Phase mapping algorithm based on 3 phase-shifted interferograms by pi / 2 at each step
         *
         * @param interferometricImgs      - The interferometric images shifted by pi/2 at each step
         * @param phaseMap                 - The 2D wrapped phase map computed by this algorithm
         * @param intensityMap             - The 2D intensity map associated at the phase map, computed by this algorithm
         * @param backgroundMap            - The 2D background associated at the phase map, computed by this algorithm
         */
        void Hariharan3Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap);
    } // namespace


    vector<Mat> AverageImgs(vector<Mat> interferometricImgs, int stepsNb) {
        if (interferometricImgs.size() % stepsNb != 0) {
            stringstream strStrm;
            strStrm << "[Phase mapping] The interferograms to be averaged must be multiple of the number of steps.";
            string message = strStrm.str();
            Logger::Error(message);
            throw exception(message.c_str());
        }

        Size size = interferometricImgs.at(0).size();

        for (Mat img : interferometricImgs) {
            if ((img.size() != size)) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Input interferograms must all be the same size.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }
        }

        vector<Mat> averagedInterferometricImgs;
        int imageNbPerStep = interferometricImgs.size() / stepsNb;
        for (int step = 0; step < stepsNb; step++) {
            Mat temp = Mat::zeros(size, CV_32FC1);
            for (int img = 0; img < imageNbPerStep; img++) {
                int index = step * imageNbPerStep + img;
                if (interferometricImgs.at(index).type() != CV_32FC1) {
                    temp += Convertor::ConvertTo32FC1(interferometricImgs.at(index));
                }else {
                    temp += interferometricImgs.at(index);
                }
            }
            temp /= imageNbPerStep;
            averagedInterferometricImgs.push_back(temp);
        }

        return averagedInterferometricImgs;
    }

    Mat PhaseMapping(vector<Mat> interferograms, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap) {
        if (interferograms.size() < 3) {
            stringstream strStrm;
            strStrm << "[Phase mapping] The number of input interferograms must be a minimum of 3 and a maximum of 7.";
            string message = strStrm.str();
            Logger::Error(message);
            throw exception(message.c_str());
        }

        Size size = interferograms.at(0).size();
        for (Mat img : interferograms) {
            if (img.type() != CV_32FC1) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Input interferograms should all be stored into a single channel 32-bits float.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }
            if ((img.size() != size)) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Input interferograms must all be the same size.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }
        }

        Size imgSize = interferograms.at(0).size();
        intensityMap = Mat::zeros(imgSize, CV_32FC1);
        phaseMap = Mat::zeros(imgSize, CV_32FC1);
        backgroundMap = Mat::zeros(imgSize, CV_32FC1);

        if (interferograms.size() % 7 == 0) {
            Hariharan7Steps(interferograms, phaseMap, intensityMap, backgroundMap);
        }
        else if (interferograms.size() % 6 == 0) {
            Hariharan6Steps(interferograms, phaseMap, intensityMap, backgroundMap);
        }
        else if (interferograms.size() % 5 == 0) {
            Hariharan5Steps(interferograms, phaseMap, intensityMap, backgroundMap);
        }
        else if (interferograms.size() % 4 == 0) {
            Hariharan4Steps(interferograms, phaseMap, intensityMap, backgroundMap);
        }
        else if (interferograms.size() % 3 == 0) {
            Hariharan3Steps(interferograms, phaseMap, intensityMap, backgroundMap);
        }

        if (phaseMap.empty() || intensityMap.empty() || backgroundMap.empty()) {
            stringstream strStrm;
            strStrm << "[Phase mapping] Algorithm failed to compute phase map.";
            string message = strStrm.str();
            Logger::Error(message);
            throw exception(message.c_str());
        }

        return phaseMap;
    }

    namespace {

        void Hariharan7Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap) {
            Size size = interferometricImgs[0].size();
            if (interferometricImgs.size() != 7) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Expected 7 interferograms to run Hariharan 7 steps algorithm.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);
                    float I3 = interferometricImgs[3].at<float>(r, c);
                    float I4 = interferometricImgs[4].at<float>(r, c);
                    float I5 = interferometricImgs[5].at<float>(r, c);
                    float I6 = interferometricImgs[6].at<float>(r, c);

                    float num = 4 * I1 - 8 * I3 + 4 * I5;
                    float denom = -I0 + 7 * I2 - 7 * I4 + I6;
                    float amplitude = 1000 * sqrt(pow(num, 2) + pow(denom, 2) * 0.5);
                    float phase = atan2(num, denom);
                    float background = 0.25f * (0.5f * I0 + 0.5f * I1 + 0.5f * I2 + I3 + 0.5f * I4 + 0.5f * I5 + 0.5f * I6);
                    intensityMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }
        }

        void Hariharan6Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap) {
            Size size = interferometricImgs[0].size();
            if (interferometricImgs.size() != 6) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Expected 6 interferograms to run Hariharan 6 steps algorithm.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

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
                    float amplitude = sqrt(pow(num, 2) + pow(denom, 2));
                    float phase = atan2(num, denom);
                    float background = 0.25f * (0.5f * I0 + 0.5f * I1 + I2 + I3 + 0.5f * I4 + 0.5f * I5);
                    intensityMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }
        }

        void Hariharan5Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap) {
            Size size = interferometricImgs[0].size();
            if (interferometricImgs.size() != 5) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Expected 5 interferograms to run Hariharan 5 steps algorithm.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);
                    float I3 = interferometricImgs[3].at<float>(r, c);
                    float I4 = interferometricImgs[4].at<float>(r, c);

                    float num = 2 * (I1 - I3);
                    float denom = (2 * I2 - I4 - I0);
                    float amplitude = sqrt(pow(num, 2) + pow(denom, 2));
                    float phase = atan2(num, denom);
                    float background = 0.25f * (0.5f * I0 + I1 + I2 + I3 + 0.5f * I4);
                    intensityMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }
        }

        void Hariharan4Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap) {
            Size size = interferometricImgs[0].size();
            if (interferometricImgs.size() != 4) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Expected 4 interferograms to run Carre 4 steps algorithm.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);
                    float I3 = interferometricImgs[3].at<float>(r, c);

                    float num = I1 - I3;
                    float denom = -I0 + I2;
                    float phase = atan2(num, denom);
                    float amplitude = sqrt(pow(num, 2) + pow(denom, 2));
                    float background = 0.25f * (I0 + I1 + I2 + I3);
                    intensityMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }
        }

        void Hariharan3Steps(vector<Mat> interferometricImgs, Mat& phaseMap, Mat& intensityMap, Mat& backgroundMap) {
            Size size = interferometricImgs[0].size();
            if (interferometricImgs.size() != 3) {
                stringstream strStrm;
                strStrm << "[Phase mapping] Expected 3 interferograms to run Carre 4 steps algorithm.";
                string message = strStrm.str();
                Logger::Error(message);
                throw exception(message.c_str());
            }

            for (int r = 0; r < size.height; r++) {
                for (int c = 0; c < size.width; c++) {
                    float I0 = interferometricImgs[0].at<float>(r, c);
                    float I1 = interferometricImgs[1].at<float>(r, c);
                    float I2 = interferometricImgs[2].at<float>(r, c);

                    float num = I2 - I1;
                    float denom = I0 - I1;
                    float phase = atan2(num, denom);
                    float amplitude = sqrt(pow(num, 2) + pow(denom, 2));
                    float background = 0.3333333f * (I0 + I1 + I2);
                    intensityMap.at<float>(r, c) = amplitude;
                    phaseMap.at<float>(r, c) = phase;
                    backgroundMap.at<float>(r, c) = background;
                }
            }
        }
    } // namespace
} // namespace phase_mapping