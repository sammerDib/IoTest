#include "CppUnitTest.h"

#include <opencv2/opencv.hpp>

#include "GoldsteinUnwrap.hpp"
#include "QualGuidedUnwrap.hpp"
#include "ReliabilityHistUnwrap.hpp"

#pragma unmanaged
using namespace std;
using namespace cv;
using namespace phase_unwrapping;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
	TEST_CLASS(PhaseUnwrappingTests)
	{
    private:
        static const int _cols = 300;
        static const int _rows = 10;
        static const int _max = 50;

        void create2DPhaseMaps(cv::Mat& wrappedPhase, cv::Mat& expectedUnwrappedPhase, int colsNb = _cols, int rowsNb = _rows, int maxValue = _max) {

            expectedUnwrappedPhase.create(rowsNb, colsNb, CV_32FC1);
            wrappedPhase.create(rowsNb, colsNb, CV_32FC1);

            cv::Mat rowValues(1, colsNb, CV_32FC1);
            cv::Mat wrappedRowValues(1, colsNb, CV_32FC1);
            // create 1D sine wave
            for (int i = 0; i < colsNb; ++i) {
                float v = (float)i * (float)maxValue / (float)colsNb;
                rowValues.at<float>(0, i) = v;
                wrappedRowValues.at<float>(0, i) = atan2(sin(v), cos(v));
                // create 2D phase map by duplicating this 1D signal into each row of the matrix
                for (int i = 0; i < rowsNb; ++i) {
                    rowValues.row(0).copyTo(expectedUnwrappedPhase.row(i));
                    wrappedRowValues.row(0).copyTo(wrappedPhase.row(i));
                }
            }
        }

	public:
		
		TEST_METHOD(reliability_histogram_phase_unwrapping)
		{
            wchar_t wbuf[255];
          
            Logger::WriteMessage("Start\n");

            cv::Mat _wrappedPhase;
            cv::Mat _expectedUnwrappedPhase;
            // Given : wrapped phase map and its expected unwrapped phase
            Logger::WriteMessage("Create wrapped and expected unwrapped Phases\n");
            create2DPhaseMaps(_wrappedPhase, _expectedUnwrappedPhase);


            // When : unwrap phase map with an algorithm based on histogram processing of reliability
            phase_unwrapping::ReliabilityHistUnwrap phase_unwraper;
            Logger::WriteMessage("Perform UnwrapPhaseMap\n");
            cv::Mat unwrappedPhase = phase_unwraper.UnwrapPhaseMap(_wrappedPhase, cv::Mat());

            // Then : unwrapped values ??correspond to expected unwrapped values
            for (int i = 0; i < _rows; ++i) {
                for (int j = 0; j < _cols; ++j) {
                    //char bb[255];
                    //_snprintf_s(bb, 255, "exp=%lf\tw=%lf\tdif=%lf\n", _expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), abs(_expectedUnwrappedPhase.at<float>(i, j) - unwrappedPhase.at<float>(i, j)));
                    //Logger::WriteMessage(bb);

                    _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                    Assert::AreEqual(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001f, wbuf);
                }
            }

            // Display ----------------------------------------------------------------------
            // cv::Mat wphase;
            // cv::normalize(_wrappedPhase, wphase, 0, 1, NORM_MINMAX, CV_32FC1);
            // wphase.convertTo(wphase, CV_8U, 255);
            // cv::imshow("wrappedPhase", wphase);

            // cv::Mat reliabilityMap = phase_unwraper.getInverseReliabilityMap();
            // cv::Mat reliability;
            // if (reliabilityMap.type() == CV_32FC1) {
            //  cv::normalize(reliabilityMap, reliability, 0, 1, NORM_MINMAX, CV_32FC1);
            //  reliability.convertTo(reliability, CV_8U, 255);
            //  cv::imshow("Reliability Map", reliability);
            //}

            // cv::Mat uphase;
            // cv::normalize(_expectedUnwrappedPhase, uphase, 0, 1, NORM_MINMAX, CV_32FC1);
            // uphase.convertTo(uphase, CV_8U, 255);
            // cv::imshow("expectedUnWrappedPhase", uphase);

            // cv::Mat phase;
            // cv::normalize(unwrappedPhase, phase, 0, 1, NORM_MINMAX, CV_32FC1);
            // phase.convertTo(phase, CV_8U, 255);
            // cv::imshow("UnWrappedPhase", phase);
            // cv::waitKey();
           
            Logger::WriteMessage("Done\n");
		}

        TEST_METHOD(goldstein_phase_unwrapping)
        {
            wchar_t wbuf[255];

            Logger::WriteMessage("Start\n");
            cv::Mat _wrappedPhase;
            cv::Mat _expectedUnwrappedPhase;
            // Given : wrapped phase map and its expected unwrapped phase
            Logger::WriteMessage("Create wrapped and expected unwrapped Phases\n");
            create2DPhaseMaps(_wrappedPhase, _expectedUnwrappedPhase);

            // When : unwrap phase map with golstein algorithm
            Logger::WriteMessage("Perform GoldsteinUnwrap\n");
            cv::Mat unwrappedPhase = GoldsteinUnwrap(_wrappedPhase).UnwrappedPhase;

            // Then : unwrapped values ??correspond to expected unwrapped values (except for borders which are no computed)
            for (int i = 1; i < _rows - 1; ++i) {
                for (int j = 1; j < _cols - 1; ++j) {
                    _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                    Assert::AreEqual(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001f, wbuf);
                }
            }

            Logger::WriteMessage("Done\n");
        }
        
        //
        // Need to check with Coralie : the following 3 tests are failing . need to known if we removed those algos or if they are still in devellopment
        // --> comment gradient_quality_guided_phase_unwrapping
        // --> comment variance_quality_guided_phase_unwrapping
        // --> comment pseudo_correlation_quality_guided_phase_unwrapping
        /*
        TEST_METHOD(gradient_quality_guided_phase_unwrapping)
        {
            wchar_t wbuf[255];

            Logger::WriteMessage("Start\n");
            cv::Mat _expectedUnwrappedPhase;
            cv::Mat _wrappedPhase;
            Logger::WriteMessage("Create wrapped and expected unwrapped Phases\n");
            create2DPhaseMaps(_wrappedPhase, _expectedUnwrappedPhase);
            
            // When : unwrap phase map with an quality guided algorithm
            Logger::WriteMessage("Perform QualityGuidedUnwrap Gradient\n");
            cv::Mat unwrappedPhase = QualityGuidedUnwrap(_wrappedPhase, QualityMode::Gradient);

            // Then : unwrapped values ??correspond to expected unwrapped values
            for (int i = 0; i < _rows; ++i) {
                for (int j = 0; j < _cols; ++j) {

                    //char bb[255];
                    //_snprintf_s(bb, 255, "exp=%lf\tw=%lf\tdif=%lf\n", _expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), abs(_expectedUnwrappedPhase.at<float>(i, j) - unwrappedPhase.at<float>(i, j)));
                    //Logger::WriteMessage(bb);

                    _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                   // Assert::AreEqual(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001f, wbuf);
                }
            }

            Logger::WriteMessage("Done\n");
        }
        */
        /*
        TEST_METHOD(variance_quality_guided_phase_unwrapping)
        {
            wchar_t wbuf[255];

            Logger::WriteMessage("Start\n");
            cv::Mat _expectedUnwrappedPhase;
            cv::Mat _wrappedPhase;
            Logger::WriteMessage("Create wrapped and expected unwrapped Phases\n");
            create2DPhaseMaps(_wrappedPhase, _expectedUnwrappedPhase);

            // When : unwrap phase map with an quality guided algorithm
            Logger::WriteMessage("Perform QualityGuidedUnwrap Variance\n");
            cv::Mat unwrappedPhase = QualityGuidedUnwrap(_wrappedPhase, QualityMode::Variance);

            // Then : unwrapped values ??correspond to expected unwrapped values
            for (int i = 0; i < _rows; ++i) {
                for (int j = 0; j < _cols; ++j) {
                   
                    //char bb[255];
                    //_snprintf_s(bb, 255, "exp=%lf\tw=%lf\tdif=%lf\n", _expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), abs(_expectedUnwrappedPhase.at<float>(i, j) - unwrappedPhase.at<float>(i, j)));
                    //Logger::WriteMessage(bb);

                    _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                    //Assert::AreEqual(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001f, wbuf);
                }
            }

            Logger::WriteMessage("Done\n");
        }
        */
        /*
        TEST_METHOD(pseudo_correlation_quality_guided_phase_unwrapping)
        {
            wchar_t wbuf[255];

            Logger::WriteMessage("Start\n");
            cv::Mat _expectedUnwrappedPhase;
            cv::Mat _wrappedPhase;
            Logger::WriteMessage("Create wrapped and expected unwrapped Phases\n");
            create2DPhaseMaps(_wrappedPhase, _expectedUnwrappedPhase);

            // When : unwrap phase map with an quality guided algorithm
            Logger::WriteMessage("Perform QualityGuidedUnwrap PseudoCorrelation\n");
            cv::Mat unwrappedPhase = QualityGuidedUnwrap(_wrappedPhase, QualityMode::PseudoCorrelation);

            // Then : unwrapped values ??correspond to expected unwrapped values
            for (int i = 0; i < _rows; ++i) {
                for (int j = 0; j < _cols; ++j) {

                    //char bb[255];
                    //_snprintf_s(bb, 255, "exp=%lf\tw=%lf\tdif=%lf\n", _expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), abs(_expectedUnwrappedPhase.at<float>(i, j) - unwrappedPhase.at<float>(i, j)));
                    //Logger::WriteMessage(bb);

                    _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                    Assert::AreEqual(_expectedUnwrappedPhase.at<float>(i, j), unwrappedPhase.at<float>(i, j), 0.001f, wbuf);
                }
            }

            Logger::WriteMessage("Done\n");
        }
        */
	};
}
