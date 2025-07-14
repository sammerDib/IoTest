#include <opencv2/opencv.hpp>
#include <chrono>
#include <filesystem>
#include <iostream>
#include <fstream>

#include "CppUnitTest.h"
#include "CameraCalibration.hpp"
#include "SystemCalibration.hpp"
#include "NormalEstimation.hpp"
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
    TEST_CLASS(NormalEstimationTest)
    {
    public:

        /*
        TEST_METHOD(only_for_manual_testing_and_debug_on_real_images)
        {
            wchar_t wbuf[255];
            filesystem::create_directories(".\\..\\..\\Tests\\Data\\normalEstimation\\Reporting");
            filesystem::path directoryPathToStoreReport = ".\\..\\..\\Tests\\Data\\normalEstimation\\Reporting";

            // Given

            Mat intrinsicCameraMatrix = Mat::zeros(3, 3, CV_64F);
            Mat distortionMatrix = Mat::zeros(5, 1, CV_64F);
            vector<Mat> rotationMatrix;
            vector<Mat> translationMatrix;

            intrinsicCameraMatrix.at<double>(0, 0) = 17739.306414; //fx
            intrinsicCameraMatrix.at<double>(1, 1) = 17822.042771; //fy
            intrinsicCameraMatrix.at<double>(0, 2) = 7176.051144; //cx
            intrinsicCameraMatrix.at<double>(1, 2) = 5273.188729; //cy
            intrinsicCameraMatrix.at<double>(2, 2) = 1;
            distortionMatrix.at<double>(0, 0) = 0; //k1
            distortionMatrix.at<double>(1, 0) = 0; //k2
            distortionMatrix.at<double>(2, 0) = 0; //p1
            distortionMatrix.at<double>(3, 0) = 0; //p2
            distortionMatrix.at<double>(4, 0) = 0; //k3
            float rms = 0.06717731030966706;

            CalibrationParameters intrinsicCameraParams = CalibrationParameters(intrinsicCameraMatrix, distortionMatrix, rotationMatrix, translationMatrix, rms);

            Mat rWaferToCamera = Mat::zeros(3, 3, CV_64F);
            Mat tWaferToCamera = Mat::zeros(3, 1, CV_64F);

            rWaferToCamera.at<double>(0, 0) = 0.999695;
            rWaferToCamera.at<double>(0, 1) = -0.012180;
            rWaferToCamera.at<double>(0, 2) = -0.021504;
            rWaferToCamera.at<double>(1, 0) = -0.019477;
            rWaferToCamera.at<double>(1, 1) = -0.923878;
            rWaferToCamera.at<double>(1, 2) = -0.382190;
            rWaferToCamera.at<double>(2, 0) = -0.015212;
            rWaferToCamera.at<double>(2, 1) = 0.382492;
            rWaferToCamera.at<double>(2, 2) = -0.923833;
            tWaferToCamera.at<double>(0, 0) = 13.804664;
            tWaferToCamera.at<double>(1, 0) = -23.654983;
            tWaferToCamera.at<double>(2, 0) = 502.423881;

            ExtrinsicCameraParameters extrinsicCameraParams = ExtrinsicCameraParameters(rWaferToCamera, tWaferToCamera);

            Mat rScreenToWafer = Mat::zeros(3, 3, CV_64F);
            Mat tScreenToWafer = Mat::zeros(3, 1, CV_64F);

            rScreenToWafer.at<double>(0, 0) = -0.999686;
            rScreenToWafer.at<double>(0, 1) = 0.014912;
            rScreenToWafer.at<double>(0, 2) = -0.020119;
            rScreenToWafer.at<double>(1, 0) = 0.021785;
            rScreenToWafer.at<double>(1, 1) = 0.914059;
            rScreenToWafer.at<double>(1, 2) = -0.404996;
            rScreenToWafer.at<double>(2, 0) = 0.012350;
            rScreenToWafer.at<double>(2, 1) = -0.405307;
            rScreenToWafer.at<double>(2, 2) = -0.914097;
            tScreenToWafer.at<double>(0, 0) = -64.408877;
            tScreenToWafer.at<double>(1, 0) = 116.898231;
            tScreenToWafer.at<double>(2, 0) = 266.099273;

            ExtrinsicScreenParameters screenParams = ExtrinsicScreenParameters(rScreenToWafer, tScreenToWafer);

            Mat rScreenToCamera = Mat::zeros(3, 3, CV_64F);
            Mat tScreenToCamera = Mat::zeros(3, 1, CV_64F);

            rScreenToCamera.at<double>(0, 0) = -0.999912;
            rScreenToCamera.at<double>(0, 1) = 0.012490;
            rScreenToCamera.at<double>(0, 2) = 0.004477;
            rScreenToCamera.at<double>(1, 0) = -0.005376;
            rScreenToCamera.at<double>(1, 1) = -0.689865;
            rScreenToCamera.at<double>(1, 2) = 0.723918;
            rScreenToCamera.at<double>(2, 0) = 0.012130;
            rScreenToCamera.at<double>(2, 1) = 0.723830;
            rScreenToCamera.at<double>(2, 2) = 0.689872;
            tScreenToCamera.at<double>(0, 0) = -57.730473;
            tScreenToCamera.at<double>(1, 0) = -232.100795;
            tScreenToCamera.at<double>(2, 0) = 302.284952;

            ExtrinsicSystemParameters systemParams = ExtrinsicSystemParameters(rScreenToCamera, tScreenToCamera);

            // When
            Mat brightfield = cv::imread(string(".\\..\\..\\Tests\\Data\\normalEstimation\\HandlingSi_S1_brightfield.tif"), cv::IMREAD_UNCHANGED);
            CMResult cmResult = ComputeCMNormalizedAndM(brightfield, intrinsicCameraParams, extrinsicCameraParams);

            Mat unwrappedPhaseX = cv::imread(string(".\\..\\..\\Tests\\Data\\normalEstimation\\HandlingSi_S1_SlopeX.tif"), cv::IMREAD_UNCHANGED);
            Mat unwrappedPhaseY = cv::imread(string(".\\..\\..\\Tests\\Data\\normalEstimation\\HandlingSi_S1_SlopeY.tif"), cv::IMREAD_UNCHANGED);
            Reporting::writePngImage(unwrappedPhaseX, directoryPathToStoreReport.string() + "\\unwrappedPhaseX.png");
            Reporting::writePngImage(unwrappedPhaseY, directoryPathToStoreReport.string() + "\\unwrappedPhaseY.png");
            MPResult mpResult = ComputeMPNormalizedAndP(unwrappedPhaseX, unwrappedPhaseY, screenParams);

            Mat mask = Mat::ones(cmResult.M.size(), CV_8UC1);
            Mat normalNormalized = ComputeNormalizedNormal(cmResult.M, cmResult.CMNormalized, mpResult.MPNormalized, mask);

            Mat normalNormalizedX, normalNormalizedY, normalNormalizedZ;
            vector<Mat> channels(3);
            split(normalNormalized, channels);
            normalNormalizedX = channels[0];
            normalNormalizedY = channels[1];
            normalNormalizedZ = channels[2];
            Reporting::writePngImage(normalNormalizedX, directoryPathToStoreReport.string() + "\\normalNormalizedX.png");
            Reporting::writePngImage(normalNormalizedY, directoryPathToStoreReport.string() + "\\normalNormalizedY.png");
            Reporting::writePngImage(normalNormalizedZ, directoryPathToStoreReport.string() + "\\normalNormalizedZ.png");

            //filesystem::remove_all(directoryPathToStoreReport);
        }
        */
    };
}