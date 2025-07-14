#include <opencv2/opencv.hpp>
#include <chrono>
#include <filesystem>
#include <iostream>
#include <fstream>

#include "CppUnitTest.h"
#include "CameraCalibration.hpp"
#include "SystemCalibration.hpp"
#include "MultiperiodUnwrap.hpp"
#include "WaferDetector.hpp"
#include "ReportingUtils.hpp"
#include "CImageTypeConvertor.hpp"
#include <CPhaseShiftingDeflectometry.hpp>

using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace psd;

#pragma unmanaged
namespace SharedOpenCVNativeTests
{
    TEST_CLASS(SystemCalibrationTest)
    {
    protected:

        void MatWrite(const string& filename, const Mat& mat)
        {
            ofstream fs(filename, fstream::binary);

            // Header
            int type = mat.type();
            fs.write((char*)&mat.cols, sizeof(int));    // cols
            fs.write((char*)&mat.rows, sizeof(int));    // rows

            // Data
            if (mat.isContinuous())
            {
                fs.write(mat.ptr<char>(0), (mat.dataend - mat.datastart));
            }
            else
            {
                int rowsz = CV_ELEM_SIZE(type) * mat.cols;
                for (int r = 0; r < mat.rows; ++r)
                {
                    fs.write(mat.ptr<char>(r), rowsz);
                }
            }
        }

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

        static vector<Mat> LoadImages(string testDataPath, string extension) {
            char buf[255];

            vector<Mat> interferoImgs;

            for (const auto& entry : filesystem::directory_iterator(testDataPath))
            {
                string imgPath = entry.path().string();
                if (imgPath.find(extension) != std::string::npos) {
                    Mat img = cv::imread(imgPath, cv::IMREAD_GRAYSCALE);
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

        static vector<Mat> LoadRealCalibImages(string extension) {
            // Create calib folder at this location and move images into it
            string testDataPath = string(".\\..\\..\\Tests\\Data\\PSDCalibration\\camera\\");
            return LoadImages(testDataPath, extension);
        }

    public:

        /*
        TEST_METHOD(testing_system_calibration_on_simulated_images)
        {
            wchar_t wbuf[255];
            filesystem::create_directories(".\\..\\..\\Tests\\Data\\PSDCalibration\\Reporting");
            filesystem::path directoryPathToStoreReport = ""; // ".\\..\\..\\Tests\\Data\\calib\\Reporting\\simulatedData";

            // Given

            Mat intrinsicCameraMatrix = Mat::zeros(3, 3, CV_64F);
            Mat distortionMatrix = Mat::zeros(5, 1, CV_64F);
            vector<Mat> rotationMatrix;
            vector<Mat> translationMatrix;

            intrinsicCameraMatrix.at<double>(0, 0) = 17767.256623; //fx
            intrinsicCameraMatrix.at<double>(1, 1) = 17916.059808; //fy
            intrinsicCameraMatrix.at<double>(0, 2) = 5355.323234; //cx
            intrinsicCameraMatrix.at<double>(1, 2) = 5568.007679; //cy
            intrinsicCameraMatrix.at<double>(2, 2) = 1;
            distortionMatrix.at<double>(0, 0) = -0.032183; //k1
            distortionMatrix.at<double>(1, 0) = 0.222716; //k2
            distortionMatrix.at<double>(2, 0) = 0.001354; //p1
            distortionMatrix.at<double>(3, 0) = 0.000758; //p2
            distortionMatrix.at<double>(4, 0) = -0.081283; //k3
            float rms = 0;

            CalibrationParameters intrinsicCameraParams = CalibrationParameters(intrinsicCameraMatrix, distortionMatrix, rotationMatrix, translationMatrix, rms);

            Mat checkerBoardImg = cv::imread(string(".\\..\\..\\Tests\\Data\\PSDCalibration\\system\\UT_WI.png"), cv::IMREAD_GRAYSCALE);
            Mat unwrappedPhaseX = MatRead(string(".\\..\\..\\Tests\\Data\\PSDCalibration\\system\\UT_UwPhase_X.bin"));
            Reporting::writePngImage(unwrappedPhaseX, directoryPathToStoreReport.string() + "\\unwrappedPhaseX.png");
            Mat unwrappedPhaseY = MatRead(string(".\\..\\..\\Tests\\Data\\PSDCalibration\\system\\UT_UwPhase_Y.bin"));
            Reporting::writePngImage(unwrappedPhaseY, directoryPathToStoreReport.string() + "\\unwrappedPhaseY.png");

            cv::GaussianBlur(checkerBoardImg, checkerBoardImg, cv::Size(5, 5), 3);

            int period = 32;

            Point2f leftCheckerBoard = Point2f(-130.97, 31.5);
            Point2f topCheckerBoard = Point2f(-31.5, 130.97);
            Point2f rightCheckerBoard = Point2f(67.97, 31.5);
            Point2f bottomCheckerBoard = Point2f(-31.5, -67.97);
            CheckerBoardsOrigins checkerBoardsOrigins = CheckerBoardsOrigins(leftCheckerBoard, bottomCheckerBoard, rightCheckerBoard, topCheckerBoard);

            bool useAllCheckerBoards = true;
            int squareXNumber = 11;
            int squareYNumber = 11;
            float squareSizeMm = 7.0f;
            float pixelSize = 0.030; //0.30 (30 microns)
            CheckerBoardsSettings checkerBoardsSettings = CheckerBoardsSettings(checkerBoardsOrigins, squareXNumber, squareYNumber, squareSizeMm, pixelSize, useAllCheckerBoards);

            float edgeExclusion = 50;
            float waferRadius = 150;
            float nbPtsScreen = 500;
            float frangePixels = period;
            float screenPixelSize = 0.2451; //245.1 microns
            InputSystemParameters params = InputSystemParameters(checkerBoardsSettings, edgeExclusion, waferRadius, nbPtsScreen, frangePixels, screenPixelSize);

            // When

            Mat maskWithoutEdge = CreateWaferMask(checkerBoardImg, params.CheckerBoards.PixelSizeMm, params.WaferRadius, params.EdgeExclusion, directoryPathToStoreReport);
            SystemParameters system = CalibrateSystem(unwrappedPhaseX, unwrappedPhaseY, checkerBoardImg, intrinsicCameraParams.CameraIntrinsic, intrinsicCameraParams.Distortion, params, maskWithoutEdge, directoryPathToStoreReport);

            // Create csv to store params
            double fx = intrinsicCameraParams.CameraIntrinsic.at<double>(0, 0);
            double fy = intrinsicCameraParams.CameraIntrinsic.at<double>(1, 1);
            double cx = intrinsicCameraParams.CameraIntrinsic.at<double>(0, 2);
            double cy = intrinsicCameraParams.CameraIntrinsic.at<double>(1, 2);
            double k1 = intrinsicCameraParams.Distortion.at<double>(0, 0);
            double k2 = intrinsicCameraParams.Distortion.at<double>(1, 0);
            double p1 = intrinsicCameraParams.Distortion.at<double>(2, 0);
            double p2 = intrinsicCameraParams.Distortion.at<double>(3, 0);
            double k3 = intrinsicCameraParams.Distortion.at<double>(4, 0);

            std::ofstream myfile;
            myfile.open(directoryPathToStoreReport.string() + "\\SystemCalibration_SimulatedImages.csv");
            myfile << "Input Camera intrinsic calibration\n";
            myfile << "fx," + to_string(fx) + ",\n";
            myfile << "fy," + to_string(fy) + ",\n";
            myfile << "cx," + to_string(cx) + ",\n";
            myfile << "cy," + to_string(cy) + ",\n";
            myfile << "k1," + to_string(k1) + ",\n";
            myfile << "k2," + to_string(k2) + ",\n";
            myfile << "p1," + to_string(p1) + ",\n";
            myfile << "p2," + to_string(p2) + ",\n";
            myfile << "k3," + to_string(k3) + ",\n";

            ExtrinsicCameraParameters cameraParams = system.ExtrinsicCamera;

            double cameraParams_r00 = cameraParams.RWaferToCamera.at<double>(0, 0);
            double cameraParams_r01 = cameraParams.RWaferToCamera.at<double>(0, 1);
            double cameraParams_r02 = cameraParams.RWaferToCamera.at<double>(0, 2);
            double cameraParams_r10 = cameraParams.RWaferToCamera.at<double>(1, 0);
            double cameraParams_r11 = cameraParams.RWaferToCamera.at<double>(1, 1);
            double cameraParams_r12 = cameraParams.RWaferToCamera.at<double>(1, 2);
            double cameraParams_r20 = cameraParams.RWaferToCamera.at<double>(2, 0);
            double cameraParams_r21 = cameraParams.RWaferToCamera.at<double>(2, 1);
            double cameraParams_r22 = cameraParams.RWaferToCamera.at<double>(2, 2);
            double cameraParams_t0 = cameraParams.TWaferToCamera.at<double>(0, 0);
            double cameraParams_t1 = cameraParams.TWaferToCamera.at<double>(1, 0);
            double cameraParams_t2 = cameraParams.TWaferToCamera.at<double>(2, 0);

            Mat RWaferToCameraOCM;
            Rodrigues(cameraParams.RWaferToCamera, RWaferToCameraOCM);
            double cameraParams_r0 = RWaferToCameraOCM.at<double>(0, 0);
            double cameraParams_r1 = RWaferToCameraOCM.at<double>(1, 0);
            double cameraParams_r2 = RWaferToCameraOCM.at<double>(2, 0);

            Assert::AreEqual(2.713013, cameraParams_r0, 0.01);
            Assert::AreEqual(-0.000621, cameraParams_r1, 0.01);
            Assert::AreEqual(0.002741, cameraParams_r2, 0.01);
            Assert::AreEqual(-1, cameraParams_t0, 1);
            Assert::AreEqual(-16, cameraParams_t1, 1);
            Assert::AreEqual(534, cameraParams_t2, 1);

            myfile << "Camera extrinsic calibration (convert from wafer to camera referential)\n";
            myfile << "r0," + to_string(cameraParams_r0) + ",\n";
            myfile << "r1," + to_string(cameraParams_r1) + ",\n";
            myfile << "r2," + to_string(cameraParams_r2) + ",\n";
            myfile << "r00," + to_string(cameraParams_r00) + ",\n";
            myfile << "r01," + to_string(cameraParams_r01) + ",\n";
            myfile << "r02," + to_string(cameraParams_r02) + ",\n";
            myfile << "r10," + to_string(cameraParams_r10) + ",\n";
            myfile << "r11," + to_string(cameraParams_r11) + ",\n";
            myfile << "r12," + to_string(cameraParams_r12) + ",\n";
            myfile << "r20," + to_string(cameraParams_r20) + ",\n";
            myfile << "r21," + to_string(cameraParams_r21) + ",\n";
            myfile << "r22," + to_string(cameraParams_r22) + ",\n";
            myfile << "t0," + to_string(cameraParams_t0) + ",\n";
            myfile << "t1," + to_string(cameraParams_t1) + ",\n";
            myfile << "t2," + to_string(cameraParams_t2) + ",\n";

            ExtrinsicScreenParameters screenParams = system.ExtrinsicScreen;

            double screenParams_r00 = screenParams.RScreenToWafer.at<double>(0, 0);
            double screenParams_r01 = screenParams.RScreenToWafer.at<double>(0, 1);
            double screenParams_r02 = screenParams.RScreenToWafer.at<double>(0, 2);
            double screenParams_r10 = screenParams.RScreenToWafer.at<double>(1, 0);
            double screenParams_r11 = screenParams.RScreenToWafer.at<double>(1, 1);
            double screenParams_r12 = screenParams.RScreenToWafer.at<double>(1, 2);
            double screenParams_r20 = screenParams.RScreenToWafer.at<double>(2, 0);
            double screenParams_r21 = screenParams.RScreenToWafer.at<double>(2, 1);
            double screenParams_r22 = screenParams.RScreenToWafer.at<double>(2, 2);
            double screenParams_t0 = screenParams.TScreenToWafer.at<double>(0, 0);
            double screenParams_t1 = screenParams.TScreenToWafer.at<double>(1, 0);
            double screenParams_t2 = screenParams.TScreenToWafer.at<double>(2, 0);

            Mat RScreenToWaferOCM;
            Rodrigues(screenParams.RScreenToWafer, RScreenToWaferOCM);
            double screenParams_r0 = RScreenToWaferOCM.at<double>(0, 0);
            double screenParams_r1 = RScreenToWaferOCM.at<double>(1, 0);
            double screenParams_r2 = RScreenToWaferOCM.at<double>(2, 0);

            Assert::AreEqual(-0.1, screenParams_t0, 10);
            Assert::AreEqual(146, screenParams_t1, 10);
            Assert::AreEqual(295, screenParams_t2, 10);

            myfile << "Screen extrinsic calibration (convert from screen to wafer referential)\n";
            myfile << "r0," + to_string(screenParams_r0) + ",\n";
            myfile << "r1," + to_string(screenParams_r1) + ",\n";
            myfile << "r2," + to_string(screenParams_r2) + ",\n";
            myfile << "r00," + to_string(screenParams_r00) + ",\n";
            myfile << "r01," + to_string(screenParams_r01) + ",\n";
            myfile << "r02," + to_string(screenParams_r02) + ",\n";
            myfile << "r10," + to_string(screenParams_r10) + ",\n";
            myfile << "r11," + to_string(screenParams_r11) + ",\n";
            myfile << "r12," + to_string(screenParams_r12) + ",\n";
            myfile << "r20," + to_string(screenParams_r20) + ",\n";
            myfile << "r21," + to_string(screenParams_r21) + ",\n";
            myfile << "r22," + to_string(screenParams_r22) + ",\n";
            myfile << "t0," + to_string(screenParams_t0) + ",\n";
            myfile << "t1," + to_string(screenParams_t1) + ",\n";
            myfile << "t2," + to_string(screenParams_t2) + ",\n";

            ExtrinsicSystemParameters systemParams = system.ExtrinsicSystem;

            double systemParams_r00 = systemParams.RScreenToCamera.at<double>(0, 0);
            double systemParams_r01 = systemParams.RScreenToCamera.at<double>(0, 1);
            double systemParams_r02 = systemParams.RScreenToCamera.at<double>(0, 2);
            double systemParams_r10 = systemParams.RScreenToCamera.at<double>(1, 0);
            double systemParams_r11 = systemParams.RScreenToCamera.at<double>(1, 1);
            double systemParams_r12 = systemParams.RScreenToCamera.at<double>(1, 2);
            double systemParams_r20 = systemParams.RScreenToCamera.at<double>(2, 0);
            double systemParams_r21 = systemParams.RScreenToCamera.at<double>(2, 1);
            double systemParams_r22 = systemParams.RScreenToCamera.at<double>(2, 2);
            double systemParams_t0 = systemParams.TScreenToCamera.at<double>(0, 0);
            double systemParams_t1 = systemParams.TScreenToCamera.at<double>(1, 0);
            double systemParams_t2 = systemParams.TScreenToCamera.at<double>(2, 0);

            Mat TScreenToCameraOCM;
            Rodrigues(systemParams.TScreenToCamera, TScreenToCameraOCM);
            double systemParams_r0 = TScreenToCameraOCM.at<double>(0, 0);
            double systemParams_r1 = TScreenToCameraOCM.at<double>(1, 0);
            double systemParams_r2 = TScreenToCameraOCM.at<double>(2, 0);

            Assert::AreEqual(-0, systemParams_t0, 10);
            Assert::AreEqual(-267, systemParams_t1, 10);
            Assert::AreEqual(329, systemParams_t2, 10);

            myfile << "System extrinsic calibration (convert from screen to camera referential)\n";
            myfile << "r0," + to_string(systemParams_r0) + ",\n";
            myfile << "r1," + to_string(systemParams_r1) + ",\n";
            myfile << "r2," + to_string(systemParams_r2) + ",\n";
            myfile << "r00," + to_string(systemParams_r00) + ",\n";
            myfile << "r01," + to_string(systemParams_r01) + ",\n";
            myfile << "r02," + to_string(systemParams_r02) + ",\n";
            myfile << "r10," + to_string(systemParams_r10) + ",\n";
            myfile << "r11," + to_string(systemParams_r11) + ",\n";
            myfile << "r12," + to_string(systemParams_r12) + ",\n";
            myfile << "r20," + to_string(systemParams_r20) + ",\n";
            myfile << "r21," + to_string(systemParams_r21) + ",\n";
            myfile << "r22," + to_string(systemParams_r22) + ",\n";
            myfile << "t0," + to_string(systemParams_t0) + ",\n";
            myfile << "t1," + to_string(systemParams_t1) + ",\n";
            myfile << "t2," + to_string(systemParams_t2) + ",\n";

            myfile.close();

            filesystem::remove_all(directoryPathToStoreReport);
        }*/

        /*
        TEST_METHOD(only_for_manual_testing_and_debug_on_real_images)
        {
            wchar_t wbuf[255];
            filesystem::create_directories(".\\..\\..\\Tests\\Data\\calib\\Reporting");
            filesystem::path directoryPathToStoreReport = ""; // ".\\..\\..\\Tests\\Data\\calib\\Reporting";

            // Given

            vector<Mat> cameraCalibImgs = LoadRealCalibImages("tiff");

            Mat checkerBoardImg = cv::imread(string(".\\..\\..\\Tests\\Data\\calib\\system\\Im36_S99_brightfield.tif"), cv::IMREAD_GRAYSCALE);

            Mat wrappedPhaseMapXPeriod1 = cv::imread(string(".\\..\\..\\Tests\\Data\\calib\\system\\Im36_S99_wrapped-phase-X.tif"), cv::IMREAD_UNCHANGED);
            Mat wrappedPhaseMapXPeriod2 = cv::imread(string(".\\..\\..\\Tests\\Data\\calib\\system\\Im288_S99_wrapped-phase-X.tif"), cv::IMREAD_UNCHANGED);
            Mat wrappedPhaseMapXPeriod3 = cv::imread(string(".\\..\\..\\Tests\\Data\\calib\\system\\Im2304_S99_wrapped-phase-X.tif"), cv::IMREAD_UNCHANGED);
            Mat wrappedPhaseMapYPeriod1 = cv::imread(string(".\\..\\..\\Tests\\Data\\calib\\system\\Im36_S99_wrapped-phase-Y.tif"), cv::IMREAD_UNCHANGED);
            Mat wrappedPhaseMapYPeriod2 = cv::imread(string(".\\..\\..\\Tests\\Data\\calib\\system\\Im288_S99_wrapped-phase-Y.tif"), cv::IMREAD_UNCHANGED);
            Mat wrappedPhaseMapYPeriod3 = cv::imread(string(".\\..\\..\\Tests\\Data\\calib\\system\\Im2304_S99_wrapped-phase-Y.tif"), cv::IMREAD_UNCHANGED);

            vector<int> periods = { 36, 288, 2304 };
            int periodNb = periods.size();

            Point2f leftCheckerBoard = Point2f(-130.97, 31.5);
            Point2f topCheckerBoard = Point2f(-31.5, 130.97);
            Point2f rightCheckerBoard = Point2f(67.97, 31.5);
            Point2f bottomCheckerBoard = Point2f(-31.5, -67.97);
            CheckerBoardsOrigins checkerBoardsOrigins = CheckerBoardsOrigins(leftCheckerBoard, bottomCheckerBoard, rightCheckerBoard, topCheckerBoard);

            bool useAllCheckerBoards = true;
            int squareXNumber = 11;
            int squareYNumber = 11;
            float squareSizeMm = 7.0f;
            float pixelSize = 0.030; //0.30 (30 microns) for real calibration images (not simulated)
            CheckerBoardsSettings checkerBoardsSettings = CheckerBoardsSettings(checkerBoardsOrigins, squareXNumber, squareYNumber, squareSizeMm, pixelSize, useAllCheckerBoards);

            float edgeExclusion = 50;
            float waferRadius = 150;
            float nbPtsScreen = 500;
            float frangePixels = periods[0];
            float screenPixelSize = 0.2451; //245.1 microns
            InputSystemParameters params = InputSystemParameters(checkerBoardsSettings, edgeExclusion, waferRadius, nbPtsScreen, frangePixels, screenPixelSize);

            // When

            CalibrationParameters intrinsicCameraParams = CalibrateCamera(cameraCalibImgs, checkerBoardsSettings, 0);

            vector<Mat> phaseMapX = {
                wrappedPhaseMapXPeriod1,
                wrappedPhaseMapXPeriod2,
                wrappedPhaseMapXPeriod3 };

            vector<Mat> phaseMapY = {
                wrappedPhaseMapYPeriod1,
                wrappedPhaseMapYPeriod2,
                wrappedPhaseMapYPeriod3 };

            Mat mask = CreateWaferMask(checkerBoardImg, pixelSize, waferRadius, 0);

            Mat unwrappedPhaseX = phase_unwrapping::MultiperiodUnwrap(phaseMapX, mask, periods, periodNb, false);
            Mat unwrappedPhaseY = phase_unwrapping::MultiperiodUnwrap(phaseMapY, mask, periods, periodNb, false);

            //Reporting::writePngImage(unwrappedPhaseX, directoryPathToStoreReport.string() + "\\unwrappedPhaseX.png");
            //Reporting::writePngImage(unwrappedPhaseY, directoryPathToStoreReport.string() + "\\unwrappedPhaseY.png");

            SystemParameters system = CalibrateSystem(unwrappedPhaseX, unwrappedPhaseY, checkerBoardImg, intrinsicCameraParams.CameraIntrinsic, intrinsicCameraParams.Distortion, params, cv::Mat());

            // Create csv to store params
            double fx = intrinsicCameraParams.CameraIntrinsic.at<double>(0, 0);
            double fy = intrinsicCameraParams.CameraIntrinsic.at<double>(1, 1);
            double cx = intrinsicCameraParams.CameraIntrinsic.at<double>(0, 2);
            double cy = intrinsicCameraParams.CameraIntrinsic.at<double>(1, 2);
            double k1 = intrinsicCameraParams.Distortion.at<double>(0, 0);
            double k2 = intrinsicCameraParams.Distortion.at<double>(1, 0);
            double p1 = intrinsicCameraParams.Distortion.at<double>(2, 0);
            double p2 = intrinsicCameraParams.Distortion.at<double>(3, 0);
            double k3 = intrinsicCameraParams.Distortion.at<double>(4, 0);

            std::ofstream myfile;
            myfile.open(directoryPathToStoreReport.string() + "\\cameraAndSystemCalibration.csv");
            myfile << "Camera intrinsic calibration\n";
            myfile << "fx," + to_string(fx) + ",\n";
            myfile << "fy," + to_string(fy) + ",\n";
            myfile << "cx," + to_string(cx) + ",\n";
            myfile << "cy," + to_string(cy) + ",\n";
            myfile << "k1," + to_string(k1) + ",\n";
            myfile << "k2," + to_string(k2) + ",\n";
            myfile << "p1," + to_string(p1) + ",\n";
            myfile << "p2," + to_string(p2) + ",\n";
            myfile << "k3," + to_string(k3) + ",\n";

            ExtrinsicCameraParameters cameraParams = system.ExtrinsicCamera;

            double cameraParams_r00 = cameraParams.RWaferToCamera.at<double>(0, 0);
            double cameraParams_r01 = cameraParams.RWaferToCamera.at<double>(0, 1);
            double cameraParams_r02 = cameraParams.RWaferToCamera.at<double>(0, 2);
            double cameraParams_r10 = cameraParams.RWaferToCamera.at<double>(1, 0);
            double cameraParams_r11 = cameraParams.RWaferToCamera.at<double>(1, 1);
            double cameraParams_r12 = cameraParams.RWaferToCamera.at<double>(1, 2);
            double cameraParams_r20 = cameraParams.RWaferToCamera.at<double>(2, 0);
            double cameraParams_r21 = cameraParams.RWaferToCamera.at<double>(2, 1);
            double cameraParams_r22 = cameraParams.RWaferToCamera.at<double>(2, 2);
            double cameraParams_t0 = cameraParams.TWaferToCamera.at<double>(0, 0);
            double cameraParams_t1 = cameraParams.TWaferToCamera.at<double>(1, 0);
            double cameraParams_t2 = cameraParams.TWaferToCamera.at<double>(2, 0);

            myfile << "Camera extrinsic calibration (convert from wafer to camera referential)\n";
            myfile << "r00," + to_string(cameraParams_r00) + ",\n";
            myfile << "R01," + to_string(cameraParams_r01) + ",\n";
            myfile << "r02," + to_string(cameraParams_r02) + ",\n";
            myfile << "r10," + to_string(cameraParams_r10) + ",\n";
            myfile << "r11," + to_string(cameraParams_r11) + ",\n";
            myfile << "r12," + to_string(cameraParams_r12) + ",\n";
            myfile << "r20," + to_string(cameraParams_r20) + ",\n";
            myfile << "r21," + to_string(cameraParams_r21) + ",\n";
            myfile << "r22," + to_string(cameraParams_r22) + ",\n";
            myfile << "t0," + to_string(cameraParams_t0) + ",\n";
            myfile << "t1," + to_string(cameraParams_t1) + ",\n";
            myfile << "t2," + to_string(cameraParams_t2) + ",\n";

            ExtrinsicScreenParameters screenParams = system.ExtrinsicScreen;

            double screenParams_r00 = screenParams.RScreenToWafer.at<double>(0, 0);
            double screenParams_r01 = screenParams.RScreenToWafer.at<double>(0, 1);
            double screenParams_r02 = screenParams.RScreenToWafer.at<double>(0, 2);
            double screenParams_r10 = screenParams.RScreenToWafer.at<double>(1, 0);
            double screenParams_r11 = screenParams.RScreenToWafer.at<double>(1, 1);
            double screenParams_r12 = screenParams.RScreenToWafer.at<double>(1, 2);
            double screenParams_r20 = screenParams.RScreenToWafer.at<double>(2, 0);
            double screenParams_r21 = screenParams.RScreenToWafer.at<double>(2, 1);
            double screenParams_r22 = screenParams.RScreenToWafer.at<double>(2, 2);
            double screenParams_t0 = screenParams.TScreenToWafer.at<double>(0, 0);
            double screenParams_t1 = screenParams.TScreenToWafer.at<double>(1, 0);
            double screenParams_t2 = screenParams.TScreenToWafer.at<double>(2, 0);

            myfile << "Screen extrinsic calibration (convert from screen to wafer referential)\n";
            myfile << "r00," + to_string(screenParams_r00) + ",\n";
            myfile << "R01," + to_string(screenParams_r01) + ",\n";
            myfile << "r02," + to_string(screenParams_r02) + ",\n";
            myfile << "r10," + to_string(screenParams_r10) + ",\n";
            myfile << "r11," + to_string(screenParams_r11) + ",\n";
            myfile << "r12," + to_string(screenParams_r12) + ",\n";
            myfile << "r20," + to_string(screenParams_r20) + ",\n";
            myfile << "r21," + to_string(screenParams_r21) + ",\n";
            myfile << "r22," + to_string(screenParams_r22) + ",\n";
            myfile << "t0," + to_string(screenParams_t0) + ",\n";
            myfile << "t1," + to_string(screenParams_t1) + ",\n";
            myfile << "t2," + to_string(screenParams_t2) + ",\n";

            ExtrinsicSystemParameters systemParams = system.ExtrinsicSystem;

            double systemParams_r00 = systemParams.RScreenToCamera.at<double>(0, 0);
            double systemParams_r01 = systemParams.RScreenToCamera.at<double>(0, 1);
            double systemParams_r02 = systemParams.RScreenToCamera.at<double>(0, 2);
            double systemParams_r10 = systemParams.RScreenToCamera.at<double>(1, 0);
            double systemParams_r11 = systemParams.RScreenToCamera.at<double>(1, 1);
            double systemParams_r12 = systemParams.RScreenToCamera.at<double>(1, 2);
            double systemParams_r20 = systemParams.RScreenToCamera.at<double>(2, 0);
            double systemParams_r21 = systemParams.RScreenToCamera.at<double>(2, 1);
            double systemParams_r22 = systemParams.RScreenToCamera.at<double>(2, 2);
            double systemParams_t0 = systemParams.TScreenToCamera.at<double>(0, 0);
            double systemParams_t1 = systemParams.TScreenToCamera.at<double>(1, 0);
            double systemParams_t2 = systemParams.TScreenToCamera.at<double>(2, 0);

            myfile << "System extrinsic calibration (convert from screen to camera referential)\n";
            myfile << "r00," + to_string(systemParams_r00) + ",\n";
            myfile << "R01," + to_string(systemParams_r01) + ",\n";
            myfile << "r02," + to_string(systemParams_r02) + ",\n";
            myfile << "r10," + to_string(systemParams_r10) + ",\n";
            myfile << "r11," + to_string(systemParams_r11) + ",\n";
            myfile << "r12," + to_string(systemParams_r12) + ",\n";
            myfile << "r20," + to_string(systemParams_r20) + ",\n";
            myfile << "r21," + to_string(systemParams_r21) + ",\n";
            myfile << "r22," + to_string(systemParams_r22) + ",\n";
            myfile << "t0," + to_string(systemParams_t0) + ",\n";
            myfile << "t1," + to_string(systemParams_t1) + ",\n";
            myfile << "t2," + to_string(systemParams_t2) + ",\n";

            myfile.close();

            //filesystem::remove_all(directoryPathToStoreReport);
        }*/
    };
}