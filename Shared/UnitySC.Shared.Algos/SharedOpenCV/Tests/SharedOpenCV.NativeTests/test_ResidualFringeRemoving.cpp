#include <opencv2/opencv.hpp>
#include <chrono>

#include "CppUnitTest.h"
#include "PhaseMappingWithResidualFringeRemoving.hpp"

using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace residual_fringe_removing;

#pragma unmanaged

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(ResidualFringeRemovingTests)
    {
    protected:

        static vector<Mat> LoadRealPSIImages() {
            char buf[255];

            // Create psi folder at this location and move interferograms into it (7 steps)
            string testDataPath = string(".\\..\\..\\Tests\\Data\\psi\\");
            vector<Mat> interferoImgs;

            for (const auto& entry : filesystem::directory_iterator(testDataPath))
            {
                string imgPath = entry.path().string();
                if (imgPath.find(".bmp") != std::string::npos) {
                    Mat img = cv::imread(imgPath);
                    if (img.empty()) {
                        _snprintf_s(buf, 255, "Could not read the image: %s\n", imgPath.c_str());
                        Logger::WriteMessage(buf);
                    }
                    Assert::IsFalse(img.empty());
                    interferoImgs.push_back(img);
                }
            }

            return interferoImgs;
        }

    public:

        /*
        TEST_METHOD(only_for_manual_testing_and_debug)
        {
            wchar_t wbuf[255];

            // Given
            auto interferoImgs = LoadRealPSIImages();
            int stepNb = 7;
            filesystem::create_directories(".\\..\\..\\Tests\\Data\\psi\\ResidualFringeRemovingTestsReporting");
            filesystem::path directoryPathToStoreReport = ".\\..\\..\\Tests\\Data\\psi\\ResidualFringeRemovingTestsReporting";

            // When
            auto start = chrono::high_resolution_clock::now();
            auto result = WrappedPhaseMapWithoutResidualFringe(interferoImgs, stepNb, directoryPathToStoreReport);
            auto stop = chrono::high_resolution_clock::now();
            auto duration = chrono::duration_cast<chrono::seconds>(stop - start);
            _snwprintf_s(wbuf, 255, L"Wrapped phase map without residual fringe duration: %p\n", duration);
            Logger::WriteMessage(wbuf);

            Mat wrappedPhaseMap = result.WrappedPhase + result.Background;

            // Display -------------------------------------------------------------------------------
            imshow("Wrapped phase", result.WrappedPhase);
            imshow("Background", result.Background);
            imshow("Wrapped phase map", wrappedPhaseMap);
            waitKey();
            // ---------------------------------------------------------------------------------------

            filesystem::remove_all(".\\ResidualFringeRemovingTestsReporting");
        }
        */

        TEST_METHOD(steps_number_must_be_positif)
        {
            // Given
            vector<Mat> interferoImgs;
            interferoImgs.push_back(Mat::zeros(100, 100, CV_32FC1));
            interferoImgs.push_back(Mat::zeros(100, 100, CV_32FC1));
            int stepNb = -2;

            // When
            auto func = [&interferoImgs, stepNb] { WrappedPhaseMapWithoutResidualFringe(interferoImgs, stepNb); };

            // Then
            Assert::ExpectException<std::invalid_argument>(func);
        }

        TEST_METHOD(steps_number_must_be_greater_than_zero)
        {
            // Given
            vector<Mat> interferoImgs;
            interferoImgs.push_back(Mat::zeros(100, 100, CV_32FC1));
            int stepNb = 0;

            // When
            auto func = [&interferoImgs, stepNb] { WrappedPhaseMapWithoutResidualFringe(interferoImgs, stepNb); };

            // Then
            Assert::ExpectException<std::invalid_argument>(func);
        }

        TEST_METHOD(interferograms_count_must_be_multiple_of_the_number_of_steps)
        {
            // Given
            vector<Mat> interferoImgs;
            interferoImgs.push_back(Mat::zeros(100, 100, CV_32FC1));
            interferoImgs.push_back(Mat::zeros(100, 100, CV_32FC1));
            interferoImgs.push_back(Mat::zeros(100, 100, CV_32FC1));
            int stepNb = 2;

            // When
            auto func = [&interferoImgs, stepNb] { WrappedPhaseMapWithoutResidualFringe(interferoImgs, stepNb); };

            // Then
            Assert::ExpectException<std::invalid_argument>(func);
        }
    };
}