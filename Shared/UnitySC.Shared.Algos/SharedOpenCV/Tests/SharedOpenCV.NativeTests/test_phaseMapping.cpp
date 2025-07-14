#include <opencv2/opencv.hpp>

#include "CppUnitTest.h"
#include "GoldsteinUnwrap.hpp"
#include "PhaseMapping.hpp"
#include "GeneralizedPhaseMapping.hpp"
#include "QualGuidedUnwrap.hpp"
#include "ReliabilityHistUnwrap.hpp"
#include "CImageTypeConvertor.hpp"

#pragma unmanaged
using namespace std;
using namespace cv;
using namespace phase_mapping;
using namespace generalized_phase_mapping;
using namespace phase_unwrapping;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(PhaseMappingTests)
    {
    private:
        const int _cols = 300;
        const int _rows = 10;
        const int _max = 50;

        std::vector<cv::Mat> create7PhaseShifted1DSignals(double phaseShift = CV_PI / 2) {
            cv::Mat wrappedRowValues1(1, _cols, CV_32FC1);
            cv::Mat wrappedRowValues2(1, _cols, CV_32FC1);
            cv::Mat wrappedRowValues3(1, _cols, CV_32FC1);
            cv::Mat wrappedRowValues4(1, _cols, CV_32FC1);
            cv::Mat wrappedRowValues5(1, _cols, CV_32FC1);
            cv::Mat wrappedRowValues6(1, _cols, CV_32FC1);
            cv::Mat wrappedRowValues7(1, _cols, CV_32FC1);

            for (int i = 0; i < _cols; ++i) {
                float v = (float)i * (float)_max / (float)_cols;
                wrappedRowValues1.at<float>(0, i) = static_cast<float>(atan2(sin(v), cos(v)));
                wrappedRowValues2.at<float>(0, i) = static_cast<float>(atan2(sin(v + phaseShift), cos(v + phaseShift)));
                wrappedRowValues3.at<float>(0, i) = static_cast<float>(atan2(sin(v + 2 * phaseShift), cos(v + 2 * phaseShift)));
                wrappedRowValues4.at<float>(0, i) = static_cast<float>(atan2(sin(v + 3 * phaseShift), cos(v + 3 * phaseShift)));
                wrappedRowValues5.at<float>(0, i) = static_cast<float>(atan2(sin(v + 4 * phaseShift), cos(v + 4 * phaseShift)));
                wrappedRowValues6.at<float>(0, i) = static_cast<float>(atan2(sin(v + 5 * phaseShift), cos(v + 5 * phaseShift)));
                wrappedRowValues7.at<float>(0, i) = static_cast<float>(atan2(sin(v + 6 * phaseShift), cos(v + 6 * phaseShift)));
            }

            std::vector<cv::Mat> phaseShifted1DSignal{ wrappedRowValues1, wrappedRowValues2, wrappedRowValues3, wrappedRowValues4, wrappedRowValues5, wrappedRowValues6, wrappedRowValues7 };
            return phaseShifted1DSignal;
        }

        std::vector<cv::Mat> create7PhaseShifted2DSignals(double phaseShift = CV_PI / 2) {
            std::vector<cv::Mat> phaseShifted1DSignal = create7PhaseShifted1DSignals();

            cv::Mat interferogram1(_rows, _cols, CV_32FC1);
            cv::Mat interferogram2(_rows, _cols, CV_32FC1);
            cv::Mat interferogram3(_rows, _cols, CV_32FC1);
            cv::Mat interferogram4(_rows, _cols, CV_32FC1);
            cv::Mat interferogram5(_rows, _cols, CV_32FC1);
            cv::Mat interferogram6(_rows, _cols, CV_32FC1);
            cv::Mat interferogram7(_rows, _cols, CV_32FC1);

            for (int i = 0; i < _rows; ++i) {
                phaseShifted1DSignal[0].row(0).copyTo(interferogram1.row(i));
                phaseShifted1DSignal[1].row(0).copyTo(interferogram2.row(i));
                phaseShifted1DSignal[2].row(0).copyTo(interferogram3.row(i));
                phaseShifted1DSignal[3].row(0).copyTo(interferogram4.row(i));
                phaseShifted1DSignal[4].row(0).copyTo(interferogram5.row(i));
                phaseShifted1DSignal[5].row(0).copyTo(interferogram6.row(i));
                phaseShifted1DSignal[6].row(0).copyTo(interferogram7.row(i));
            }

            std::vector<cv::Mat> phaseShifted2DSignal{ interferogram1, interferogram2, interferogram3, interferogram4, interferogram5, interferogram6, interferogram7 };
            return phaseShifted2DSignal;
        }

        std::vector<cv::Mat> createPhaseShifted1DSignals(int stepNb) {
            double phaseShift = CV_2PI / stepNb;

            std::vector<cv::Mat> phaseShifted1DSignal;
            for (int step = 0; step < stepNb; step++)
            {
                cv::Mat wrappedRowValues(1, _cols, CV_32FC1);
                for (int i = 0; i < _cols; ++i) {
                    float v = (float)i * (float)_max / (float)_cols;
                    wrappedRowValues.at<float>(0, i) = (float) atan2(sin(v + step * phaseShift), cos(v + step * phaseShift));
                }
                phaseShifted1DSignal.push_back(wrappedRowValues);
            }

            return phaseShifted1DSignal;
        }

        std::vector<cv::Mat> createPhaseShifted2DSignals(int stepNb) {
            std::vector<cv::Mat> phaseShifted1DSignal = createPhaseShifted1DSignals(stepNb);

            std::vector<cv::Mat> phaseShifted2DSignal;
            for (int step = 0; step < stepNb; step++)
            {
                cv::Mat interferogram(_rows, _cols, CV_32FC1);
                for (int i = 0; i < _rows; ++i) {
                    phaseShifted1DSignal[step].row(0).copyTo(interferogram.row(i));
                }
                phaseShifted2DSignal.push_back(interferogram);
            }

            return phaseShifted2DSignal;
        }

        void checkFloatWrappedPhase(std::vector<cv::Mat> possibleWrappedPhaseMap, cv::Mat phaseMapToCheck, float precision = 0.1f)
        {
            wchar_t wbuf[255];
            // Check if wrapped phase values ​​correspond to expected wrapped phase values
            int indexOfInterferogramToCompare = 0;
            float dist = abs(possibleWrappedPhaseMap.at(0).at<float>(0, 0) - phaseMapToCheck.at<float>(0, 0));
            for (int index = 1; index < possibleWrappedPhaseMap.size(); ++index) {
                float currentDist = abs(possibleWrappedPhaseMap.at(index).at<float>(0, 0) - phaseMapToCheck.at<float>(0, 0));
                if (currentDist < dist)
                {
                    dist = currentDist;
                    indexOfInterferogramToCompare = index;
                }
            }
            for (int i = 0; i < _rows; ++i) {
                for (int j = 0; j < _cols; ++j) {
                    _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                    Assert::AreEqual(possibleWrappedPhaseMap.at(indexOfInterferogramToCompare).at<float>(i, j), phaseMapToCheck.at<float>(i, j), precision, wbuf);
                }
            }
        }

        void checkUcharWrappedPhase(std::vector<cv::Mat> possibleWrappedPhaseMap, cv::Mat phaseMapToCheck, float precision = 0.1)
        {
            wchar_t wbuf[255];
            // Check if wrapped phase values ​​correspond to expected wrapped phase values
            int indexOfInterferogramToCompare = 0;
            float dist = abs((float)possibleWrappedPhaseMap.at(0).at<uchar>(0, 0) - (float)phaseMapToCheck.at<uchar>(0, 0));
            for (int index = 1; index < possibleWrappedPhaseMap.size(); ++index) {
                float currentDist = abs((float)possibleWrappedPhaseMap.at(index).at<uchar>(0, 0) - (float)phaseMapToCheck.at<uchar>(0, 0));
                if (currentDist < dist)
                {
                    dist = currentDist;
                    indexOfInterferogramToCompare = index;
                }
            }
            for (int i = 0; i < _rows; ++i) {
                for (int j = 0; j < _cols; ++j) {
                    _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                    Assert::AreEqual((float)possibleWrappedPhaseMap.at(indexOfInterferogramToCompare).at<uchar>(i, j), (float)phaseMapToCheck.at<uchar>(i, j), precision, wbuf);
                }
            }
        }

    public:

        TEST_METHOD(hariharan_7steps_phase_mapping)
        {
            Logger::WriteMessage("Start\n");

            std::vector<cv::Mat> interferoImgs90 = create7PhaseShifted2DSignals();
            // Given : 7 phase-shifted interferograms of a constant and increasing pi/2 phase

            // When : Compute phase map
            WrappedPhaseMap result = PhaseMapping(interferoImgs90, 7);
            cv::Mat phaseMap = result.Phase;
            cv::Mat backgroundMap = result.Background;
            cv::Mat wrappedPhaseMap = -phaseMap + backgroundMap;
            cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

            // Display ----------------------------------------------------------------------
            //cv::imshow("Interferogram 1", _interferoImgs90[0]);
            //cv::imshow("Interferogram 2", _interferoImgs90[1]);
            //cv::imshow("Interferogram 3", _interferoImgs90[2]);
            //cv::imshow("Interferogram 4", _interferoImgs90[3]);
            //cv::imshow("Interferogram 5", _interferoImgs90[4]);
            //cv::imshow("Interferogram 6", _interferoImgs90[5]);
            //cv::imshow("Interferogram 7", _interferoImgs90[6]);

            //cv::imshow("wrappedPhase", wrappedPhaseMap);

            //cv::waitKey();
            //-------------------------------------------------------------------------------

            // Then : wrapped phase values correspond to expected wrapped phase values
            checkFloatWrappedPhase(interferoImgs90, wrappedPhaseMap);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(hariharan_6steps_phase_mapping)
        {
            Logger::WriteMessage("Start\n");

            std::vector<cv::Mat> interferoImgs90 = create7PhaseShifted2DSignals();
            // Given : 6 phase-shifted interferograms of a constant and increasing pi/2 phase
            std::vector<cv::Mat> subInterferosImgs90 = { interferoImgs90.begin(), interferoImgs90.begin() + 6 };

            // When : Compute phase map
            WrappedPhaseMap result = PhaseMapping(subInterferosImgs90, 6);
            cv::Mat phaseMap = result.Phase;
            cv::Mat backgroundMap = result.Background;
            cv::Mat wrappedPhaseMap = -phaseMap + backgroundMap;
            cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

            // Then : wrapped phase values ​​correspond to expected wrapped phase values
            checkFloatWrappedPhase(interferoImgs90, wrappedPhaseMap);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(hariharan_5steps_phase_mapping)
        {
            Logger::WriteMessage("Start\n");

            std::vector<cv::Mat> interferoImgs90 = create7PhaseShifted2DSignals();
            // Given : 5 phase-shifted interferograms of a constant and increasing pi/2 phase
            std::vector<cv::Mat> subInterferosImgs90 = { interferoImgs90.begin(), interferoImgs90.begin() + 5 };

            // When : Compute phase map
            WrappedPhaseMap result = PhaseMapping(subInterferosImgs90, 5);
            cv::Mat phaseMap = result.Phase;
            cv::Mat backgroundMap = result.Background;
            cv::Mat wrappedPhaseMap = -phaseMap + backgroundMap;
            cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

            // Then : wrapped phase values ​​correspond to expected wrapped phase values
            checkFloatWrappedPhase(interferoImgs90, wrappedPhaseMap);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(hariharan_4steps_phase_mapping)
        {
            Logger::WriteMessage("Start\n");

            std::vector<cv::Mat> interferoImgs90 = create7PhaseShifted2DSignals();
            // Given : 4 phase-shifted interferograms of a constant and increasing pi/2 phase
            std::vector<cv::Mat> subInterferosImgs90 = { interferoImgs90.begin() + 3, interferoImgs90.begin() + 7 };

            // When : Compute phase map
            WrappedPhaseMap result = PhaseMapping(subInterferosImgs90, 4);
            cv::Mat phaseMap = result.Phase;
            cv::Mat backgroundMap = result.Background;
            cv::Mat wrappedPhaseMap = -phaseMap + backgroundMap;
            cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

            // Then : wrapped phase values ​​correspond to expected wrapped phase values
            checkFloatWrappedPhase(interferoImgs90, wrappedPhaseMap);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(hariharan_3steps_phase_mapping)
        {
            Logger::WriteMessage("Start\n");
            std::vector<cv::Mat> interferoImgs90 = create7PhaseShifted2DSignals();
            // Given : 3 phase-shifted interferograms of a constant and increasing pi/2 phase
            std::vector<cv::Mat> subInterferosImgs90 = { interferoImgs90.begin(), interferoImgs90.begin() + 3 };

            // When : Compute phase map
            WrappedPhaseMap result = PhaseMapping(subInterferosImgs90, 3);
            cv::Mat phaseMap = result.Phase;
            cv::Mat backgroundMap = result.Background;
            cv::Mat wrappedPhaseMap = -phaseMap + backgroundMap;
            cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

            // Then : wrapped phase values ​​correspond to expected wrapped phase values
            checkFloatWrappedPhase(interferoImgs90, wrappedPhaseMap);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(generalized_phase_mapping_high_precision)
        {
            Logger::WriteMessage("Start\n");

            // Given
            int stepNb = 7;
            std::vector<cv::Mat> interferoImgs = createPhaseShifted2DSignals(stepNb);

            // When : Compute phase map
            WrappedPhaseMap result = GeneralizedPhaseMapping(interferoImgs, stepNb, High);
            cv::Mat phaseMap = result.Phase;
            cv::Mat backgroundMap = result.Background;
            cv::Mat wrappedPhaseMap = -phaseMap + backgroundMap;
            cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

            // Then : wrapped phase values ​​correspond to expected wrapped phase values
            checkFloatWrappedPhase(interferoImgs, wrappedPhaseMap);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(residual_fringe_removing_is_correct_when_input_interferograms_have_not_residual_fringe)
        {
            Logger::WriteMessage("Start\n");

            // Given
            auto interferoImgs = create7PhaseShifted2DSignals();

            // When
            WrappedPhaseMap result = PhaseMapping(interferoImgs, 7, true);
            cv::Mat phaseMap = result.Phase;
            cv::Mat backgroundMap = result.Background;
            cv::Mat wrappedPhaseMap = -phaseMap + backgroundMap;
            cv::normalize(wrappedPhaseMap, wrappedPhaseMap, -CV_PI, CV_PI, cv::NORM_MINMAX);

            // Display -------------------------------------------------------------------------------
            //cv::imshow("Interferogram 1", interferoImgs[0]);
            //cv::imshow("Interferogram 2", interferoImgs[1]);
            //cv::imshow("Interferogram 3", interferoImgs[2]);
            //cv::imshow("Interferogram 4", interferoImgs[3]);
            //cv::imshow("Interferogram 5", interferoImgs[4]);
            //cv::imshow("Interferogram 6", interferoImgs[5]);
            //cv::imshow("Interferogram 7", interferoImgs[6]);

            //imshow("Wrapped phase", result.WrappedPhase);
            //imshow("Background", result.Background);
            //imshow("Wrapped phase map", wrappedPhaseMap);

            //waitKey();
            // ---------------------------------------------------------------------------------------

            // Then : wrapped phase values correspond to expected wrapped phase values

            checkFloatWrappedPhase(interferoImgs, wrappedPhaseMap, 0.5f);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(hariharan_need_at_least_3steps_to_compute_phase_mapping)
        {
            Logger::WriteMessage("Start\n");

            // Given : 2 phase-shifted interferograms of a constant and increasing pi/2 phase
            std::vector<cv::Mat> interferoImgs90 = create7PhaseShifted2DSignals();
            std::vector<cv::Mat> subInterferosImgs90 = { interferoImgs90.begin(), interferoImgs90.begin() + 2 };

            // When : Compute phase map
            auto func = [&subInterferosImgs90] { PhaseMapping(subInterferosImgs90, 2); };

            // Then
            Assert::ExpectException<std::exception>(func);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(hariharan_need_at_most_7steps_to_compute_phase_mapping)
        {
            Logger::WriteMessage("Start\n");

            // Given : 8 phase-shifted interferograms of a constant and increasing pi/2 phase
            std::vector<cv::Mat> interferoImgs90 = create7PhaseShifted2DSignals();
            std::vector<cv::Mat> subInterferosImgs90 = { interferoImgs90.begin(), interferoImgs90.begin() + 7 };
            subInterferosImgs90.push_back(interferoImgs90.at(0));

            // When : Compute phase map
            auto func = [&subInterferosImgs90] { PhaseMapping(subInterferosImgs90, 8); };

            // Then
            Assert::ExpectException<std::exception>(func);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(generalized_phase_mapping_need_at_least_3steps)
        {
            Logger::WriteMessage("Start\n");

            // Given
            int stepNb = 2;
            std::vector<cv::Mat> interferoImgs = createPhaseShifted2DSignals(stepNb);

            // When : Compute phase map
            auto func = [&interferoImgs, stepNb] { PhaseMapping(interferoImgs, stepNb); };

            // Then
            Assert::ExpectException<std::exception>(func);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(residual_fringe_removing_need_at_least_3steps_to_compute_phase_mapping)
        {
            Logger::WriteMessage("Start\n");

            // Given : 3 phase-shifted interferograms of a constant and increasing pi/2 phase
            std::vector<cv::Mat> interferoImgs7Steps = create7PhaseShifted2DSignals();
            std::vector<cv::Mat> interferoImgs2Steps = { interferoImgs7Steps.begin(), interferoImgs7Steps.begin() + 2 };

            // When : Compute phase map
            auto func = [&interferoImgs2Steps] { PhaseMapping(interferoImgs2Steps, 2, true); };

            // Then
            Assert::ExpectException<std::invalid_argument>(func);

            Logger::WriteMessage("Done\n");
        }
    };
}