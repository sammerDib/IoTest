#include <opencv2/opencv.hpp>
#include <chrono>

#include "CppUnitTest.h"
#include "CPhaseShiftingInterferometry.hpp"

using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace psi;

#pragma unmanaged

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(PhaseShiftingInterferometryTests)
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
            auto result = psi::TopoReconstructionFromPhase(interferoImgs, 618, stepNb, UnwrapMode::Goldstein, true, directoryPathToStoreReport);
            auto stop = chrono::high_resolution_clock::now();
            auto duration = chrono::duration_cast<chrono::seconds>(stop - start);
            _snwprintf_s(wbuf, 255, L"Topo reconstruction from phase duration: %p\n", duration);
            Logger::WriteMessage(wbuf);

            //filesystem::remove_all(".\\ResidualFringeRemovingTestsReporting");
        }*/
    };
}