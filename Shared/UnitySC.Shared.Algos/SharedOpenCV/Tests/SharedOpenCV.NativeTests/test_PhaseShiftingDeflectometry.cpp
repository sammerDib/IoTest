#include <opencv2/opencv.hpp>
#include <chrono>
#include <filesystem>
#include <iostream>
#include <fstream>

#include "CppUnitTest.h"
#include "CPhaseShiftingDeflectometry.hpp"
#include "MultiperiodUnwrap.hpp"
#include "ReportingUtils.hpp"
#include "CImageTypeConvertor.hpp"
#include <WaferDetector.hpp>

using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace psd;

#pragma unmanaged

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(PhaseShiftingDeflectometryTests)
    {
    protected:

        Mat MatRead(const string& filename)
        {
            ifstream fs(filename, fstream::binary);

            // Header
            int rows, cols;
            int type = CV_32FC1;
            fs.read((char*)&cols, sizeof(int));         // cols
            fs.read((char*)&rows, sizeof(int));         // rows

            // Data
            Mat mat(rows, cols, type);
            fs.read((char*)mat.data, CV_ELEM_SIZE(type) * rows * cols);

            return mat;
        }

        static vector<Mat> LoadImages(string testDataPath) {
            char buf[255];

            vector<Mat> interferoImgs;

            for (const auto& entry : filesystem::directory_iterator(testDataPath))
            {
                string imgPath = entry.path().string();
                if (imgPath.find(".tif") != std::string::npos) {
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

        static vector<Mat> LoadRealPSDXImages() {

            // Create psd folder at this location and move interferograms into it
            string testDataPath = string(".\\..\\..\\Tests\\Data\\psd\\X\\");
            return LoadImages(testDataPath);
        }

        static vector<Mat> LoadRealPSDYImages() {

            // Create psd folder at this location and move interferograms into it
            string testDataPath = string(".\\..\\..\\Tests\\Data\\psd\\Y\\");
            return LoadImages(testDataPath);
        }

    public:

        /*
        TEST_METHOD(only_for_manual_testing_and_debug)
        {
            wchar_t wbuf[255];

            // Given
            auto interferoImgsX = LoadRealPSDXImages();
            auto interferoImgsY = LoadRealPSDYImages();
            int stepNb = 8;
            filesystem::create_directories(".\\..\\..\\Tests\\Data\\psd\\Reporting");
            filesystem::path directoryPathToStoreReport = ".\\..\\..\\Tests\\Data\\psd\\Reporting";

            // When
            auto start = chrono::high_resolution_clock::now();

            auto phaseMapX = psd::ComputePhaseMap(interferoImgsX, stepNb, psd::X, directoryPathToStoreReport);
            auto maskX = psd::ComputeMask(phaseMapX, 15, 10, directoryPathToStoreReport);
            auto curvatureX = psd::ComputeCurvature(phaseMapX, maskX, stepNb, psd::X, directoryPathToStoreReport);

            auto phaseMapY = psd::ComputePhaseMap(interferoImgsY, stepNb, psd::Y, directoryPathToStoreReport);
            auto maskY = psd::ComputeMask(phaseMapY, 15, 10, directoryPathToStoreReport);
            auto curvatureY = psd::ComputeCurvature(phaseMapY, maskY, stepNb, psd::Y, directoryPathToStoreReport);

            float stdBackground = psd::CalibrateCurvatureDynamics(curvatureX, curvatureY, maskX);
            auto calibratedCurvatureX = psd::ApplyDynamicCalibration(curvatureX, maskX, stdBackground, 20.0f, 1.0f, directoryPathToStoreReport);
            auto calibratedCurvatureY = psd::ApplyDynamicCalibration(curvatureY, maskY, stdBackground, 20.0f, 1.0f, directoryPathToStoreReport);

            auto dark = psd::ComputeDark(phaseMapX.Dark, phaseMapY.Dark, maskX, FitSurface::PolynomeOrder2, directoryPathToStoreReport);
            auto finalDark = psd::ApplyDynamicCoefficient(dark, maskX, 2.0, 0.03, directoryPathToStoreReport);

            auto stop = chrono::high_resolution_clock::now();
            auto duration = chrono::duration_cast<chrono::seconds>(stop - start);
            _snwprintf_s(wbuf, 255, L"Topo reconstruction from phase duration: %p\n", duration);
            Logger::WriteMessage(wbuf);
        }*/

        /*
        TEST_METHOD(only_for_manual_testing_and_debug_multiperiod_unwrapping)
        {
            wchar_t wbuf[255];
            filesystem::create_directories(".\\..\\..\\Tests\\Data\\psd\\Reporting");
            filesystem::path directoryPathToStoreReport = ".\\..\\..\\Tests\\Data\\psd\\Reporting";

            // Given

            int periodNb = 8;
            int stepNb = 10; //nb image per period

            vector<int> periods = { 36, 72, 144, 288, 576, 1152, 2304, 4608 };

            vector<Mat> interferoImgsXPeriod1 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\36\\"));
            vector<Mat> interferoImgsXPeriod2 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\72\\"));
            vector<Mat> interferoImgsXPeriod3 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\144\\"));
            vector<Mat> interferoImgsXPeriod4 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\288\\"));
            vector<Mat> interferoImgsXPeriod5 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\576\\"));
            vector<Mat> interferoImgsXPeriod6 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\1152\\"));
            vector<Mat> interferoImgsXPeriod7 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\2304\\"));
            vector<Mat> interferoImgsXPeriod8 = LoadImages(string(".\\..\\..\\Tests\\Data\\psd\\X\\4608\\"));

            // When

            auto phaseMapXPeriod1 = psd::ComputePhaseMap(interferoImgsXPeriod1, stepNb, psd::X);
            auto phaseMapXPeriod2 = psd::ComputePhaseMap(interferoImgsXPeriod2, stepNb, psd::X);
            auto phaseMapXPeriod3 = psd::ComputePhaseMap(interferoImgsXPeriod3, stepNb, psd::X);
            auto phaseMapXPeriod4 = psd::ComputePhaseMap(interferoImgsXPeriod4, stepNb, psd::X);
            auto phaseMapXPeriod5 = psd::ComputePhaseMap(interferoImgsXPeriod5, stepNb, psd::X);
            auto phaseMapXPeriod6 = psd::ComputePhaseMap(interferoImgsXPeriod6, stepNb, psd::X);
            auto phaseMapXPeriod7 = psd::ComputePhaseMap(interferoImgsXPeriod7, stepNb, psd::X);
            auto phaseMapXPeriod8 = psd::ComputePhaseMap(interferoImgsXPeriod8, stepNb, psd::X);

            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod36.png", phaseMapXPeriod1.Phase);
            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod72.png", phaseMapXPeriod2.Phase);
            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod144.png", phaseMapXPeriod3.Phase);
            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod288.png", phaseMapXPeriod4.Phase);
            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod576.png", phaseMapXPeriod5.Phase);
            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod1152.png", phaseMapXPeriod6.Phase);
            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod2304.png", phaseMapXPeriod7.Phase);
            cv::imwrite(directoryPathToStoreReport.string() + "\\X\\phaseMapXPeriod4608.png", phaseMapXPeriod8.Phase);

            auto mask = psd::ComputeMask(phaseMapXPeriod1, 15, 10);
            Reporting::writePngImage(mask, directoryPathToStoreReport.string() + "\\X\\mask.png");

            vector<Mat> phaseMapX = {
                phaseMapXPeriod1.Phase,
                phaseMapXPeriod2.Phase,
                phaseMapXPeriod3.Phase,
                phaseMapXPeriod4.Phase,
                phaseMapXPeriod5.Phase,
                phaseMapXPeriod6.Phase,
                phaseMapXPeriod7.Phase,
                phaseMapXPeriod8.Phase };

            auto start = chrono::high_resolution_clock::now();

            Mat unwrappedPhaseX = phase_unwrapping::MultiperiodUnwrap(phaseMapX, mask, periods, periodNb, true);

            auto stop = chrono::high_resolution_clock::now();
            auto duration = chrono::duration_cast<chrono::seconds>(stop - start);
            _snwprintf_s(wbuf, 255, L"Topo reconstruction from phase duration: %p\n", duration);
            Logger::WriteMessage(wbuf);

            Reporting::writePngImage(unwrappedPhaseX, directoryPathToStoreReport.string() + "\\unwrappedPhaseX.png");
            //filesystem::remove_all(".\\ResidualFringeRemovingTestsReporting");
        }
        */

        /*
        TEST_METHOD(only_for_manual_testing_and_debug_multiperiod_unwrapping_with_only_one_period)
        {
            wchar_t wbuf[255];
            filesystem::create_directories(".\\..\\..\\Tests\\Data\\psd\\Reporting");
            filesystem::path directoryPathToStoreReport = ".\\..\\..\\Tests\\Data\\psd\\Reporting";

            // Given

            int periodNb = 1;
            int stepNb = 1; //nb image per period
            vector<int> periods = { 36 };

            Mat unwrapX = MatRead(string(".\\..\\..\\Tests\\Data\\PSDCalibration\\system\\UT_UwPhase_X.bin"));
            Mat checkerBoardImg = cv::imread(string(".\\..\\..\\Tests\\Data\\PSDCalibration\\system\\UT_WI.png"), cv::IMREAD_GRAYSCALE);
            Mat mask = CreateWaferMask(checkerBoardImg, 0.030, 150, 0, directoryPathToStoreReport);

            Reporting::writePngImage(mask, directoryPathToStoreReport.string() + "\\X\\mask.png");

            // When

            vector<Mat> phaseMapX = { unwrapX };
            Mat unwrappedPhaseX = phase_unwrapping::MultiperiodUnwrap(phaseMapX, mask, periods, periodNb, true);

            Reporting::writePngImage(unwrapX, directoryPathToStoreReport.string() + "\\unwrapX.png");
            Reporting::writePngImage(unwrappedPhaseX, directoryPathToStoreReport.string() + "\\unwrappedPhaseX.png");
            //filesystem::remove_all(directoryPathToStoreReport);
        }*/
    };
}