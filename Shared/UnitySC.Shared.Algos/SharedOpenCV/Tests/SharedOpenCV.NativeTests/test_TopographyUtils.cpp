#include <opencv2/opencv.hpp>

#include "CppUnitTest.h"
#include "Utils.hpp"

#pragma unmanaged
using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(TopographyUtilsTests)
    {
    private:
        const int _cols = 300;
        const int _rows = 10;

        std::vector<cv::Mat> create7Interferograms() {
            cv::Mat img1(_rows, _cols, CV_32FC1, Scalar(0));
            cv::Mat img2(_rows, _cols, CV_32FC1, Scalar(25));
            cv::Mat img3(_rows, _cols, CV_32FC1, Scalar(50));
            cv::Mat img4(_rows, _cols, CV_32FC1, Scalar(100));
            cv::Mat img5(_rows, _cols, CV_32FC1, Scalar(150));
            cv::Mat img6(_rows, _cols, CV_32FC1, Scalar(200));
            cv::Mat img7(_rows, _cols, CV_32FC1, Scalar(255));

            std::vector<cv::Mat> allImgs{ img1, img2, img3, img4, img5, img6, img7 };
            return allImgs;
        }

    public:

        TEST_METHOD(when_average_interferogram_then_final_interferograms_​​correspond_to_mean_interferogram_for_each_step)
        {
            Logger::WriteMessage("Start\n");
            wchar_t wbuf[255];

            // Given : A constant number of interferograms at each step
            std::vector<cv::Mat> interferoImgs = create7Interferograms();
            std::vector<cv::Mat> interferogramsToAverage;
            int imageNbPerStep = 10;
            int stepNb = (int)interferoImgs.size();
            for (int step = 0; step < stepNb; step++) {
                for (int index = 0; index < imageNbPerStep; index++) {
                    interferogramsToAverage.push_back(interferoImgs.at(step));
                }
            }

            // When : We average the interferograms corresponding to the same step, to have only one average interferogram for each step
            std::vector<cv::Mat> averagedInterferosImgs = AverageInterferogramsPerStep(interferogramsToAverage, stepNb);

            // Then :Final interferograms ​​correspond to mean interferogram for each step
            for (int step = 0; step < stepNb; step++) {
                for (int i = 0; i < _rows; ++i) {
                    for (int j = 0; j < _cols; ++j) {
                        _snwprintf_s(wbuf, 255, L"Outside tolerance at [%d,%d]\n", i, j);
                        Assert::AreEqual(interferoImgs.at(step).at<float>(i, j), averagedInterferosImgs.at(step).at<float>(i, j), 0.1f, wbuf);
                    }
                }
            }

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(when_average_interferogram_then_averaged_interferograms_keep_same_type)
        {
            Logger::WriteMessage("Start\n");
            wchar_t wbuf[255];

            // Given : Interferograms with the same type
            cv::Mat img1(_rows, _cols, CV_32FC1, Scalar(0));
            cv::Mat img2(_rows, _cols, CV_32FC1, Scalar(25));
            cv::Mat img3(_rows, _cols, CV_32FC1, Scalar(50));
            std::vector<cv::Mat> interferogramsToAverage{ img1, img2, img3 };
            int stepNb = (int)interferogramsToAverage.size();

            // When : We average the interferograms corresponding to the same step, to have only one average interferogram for each step
            std::vector<cv::Mat> averagedInterferosImgs = AverageInterferogramsPerStep(interferogramsToAverage, stepNb);

            // Then : Type of the averaged interferograms  is same than initial interferograms type
            int initialType = interferogramsToAverage[0].type();

            for (int step = 0; step < stepNb; step++) {
                for (int i = 0; i < _rows; ++i) {
                    for (int j = 0; j < _cols; ++j) {
                        int finalType = averagedInterferosImgs.at(step).type();
                        _snwprintf_s(wbuf, 255, L" Type of the averaged interferogram [%i] is not same than initial interferograms type [%i]\n", finalType, initialType);
                        Assert::AreEqual(initialType, finalType, wbuf);
                    }
                }
            }

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(when_average_interferogram_then_all_interferograms_must_have_the_same_type_to_be_averaged)
        {
            Logger::WriteMessage("Start\n");
            wchar_t wbuf[255];

            // Given : Interferograms with various type
            cv::Mat img1(_rows, _cols, CV_32FC1, Scalar(0));
            cv::Mat img2(_rows, _cols, CV_8UC3, Scalar(1));
            cv::Mat img3(_rows, _cols, CV_32FC1, Scalar(50));
            cv::Mat img4(_rows, _cols, CV_32FC1, Scalar(45));
            std::vector<cv::Mat> interferogramsToAverage{ img1, img2, img3, img4 };
            int stepNb = 2;

            // When : We average the interferograms corresponding to the same step, to have only one average interferogram for each step
            auto func = [&interferogramsToAverage, stepNb] { AverageInterferogramsPerStep(interferogramsToAverage, stepNb); };

            // Then
            _snwprintf_s(wbuf, 255, L"Fail if interferograms are not all the same type\n");
            Assert::ExpectException<std::exception>(func, wbuf);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(when_average_interferogram_then_all_interferograms_must_have_the_same_size_to_be_averaged)
        {
            Logger::WriteMessage("Start\n");
            wchar_t wbuf[255];

            // Given : Interferograms with various size
            cv::Mat img1(_rows, _cols, CV_32FC1, Scalar(0));
            cv::Mat img2(_rows, _cols + 10, CV_32FC1, Scalar(1));
            cv::Mat img3(_rows, _cols, CV_32FC1, Scalar(50));
            cv::Mat img4(_rows, _cols, CV_32FC1, Scalar(45));
            std::vector<cv::Mat> interferogramsToAverage{ img1, img2, img3, img4 };
            int stepNb = 2;

            // When : We average the interferograms corresponding to the same step, to have only one average interferogram for each step
            auto func = [&interferogramsToAverage, stepNb] { AverageInterferogramsPerStep(interferogramsToAverage, stepNb); };

            // Then
            _snwprintf_s(wbuf, 255, L"Fail if interferograms are not all the same size\n");
            Assert::ExpectException<std::exception>(func, wbuf);

            Logger::WriteMessage("Done\n");
        }

        TEST_METHOD(when_average_interferogram_then_step_must_have_the_same_number_of_interferograms)
        {
            Logger::WriteMessage("Start\n");
            wchar_t wbuf[255];

            // Given : Three interferograms for two step (one step with one interferograms and the other with two)
            cv::Mat img1(_rows, _cols, CV_32FC1, Scalar(0));
            cv::Mat img2(_rows, _cols, CV_32FC1, Scalar(25));
            cv::Mat img3(_rows, _cols, CV_32FC1, Scalar(50));
            std::vector<cv::Mat> interferogramsToAverage{ img1, img2, img3 };
            int stepNb = 2;

            // When : We average the interferograms corresponding to the same step, to have only one average interferogram for each step
            auto func = [&interferogramsToAverage, stepNb] { AverageInterferogramsPerStep(interferogramsToAverage, stepNb); };

            // Then
            _snwprintf_s(wbuf, 255, L"Fail if steps do not have the same number of interferograms\n");
            Assert::ExpectException<std::exception>(func, wbuf);

            Logger::WriteMessage("Done\n");
        }
    };
}