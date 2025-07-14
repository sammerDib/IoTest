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
#include "AngularReflectionsCalibration.hpp"
#include "CPhaseShiftingDeflectometry.hpp"

using namespace std;
using namespace cv;
using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace psd;

#pragma unmanaged
namespace SharedOpenCVNativeTests
{
    TEST_CLASS(AngularReflectionsCalibrationTest)
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
            double rms = 0.06717731030966706;

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

            Mat brightfield = cv::imread(string(".\\..\\..\\Tests\\Data\\normalEstimation\\HandlingSi_S1_brightfield.tif"), cv::IMREAD_UNCHANGED);
            psd::CMResult cmResult = ComputeCMNormalizedAndM(brightfield, intrinsicCameraParams, extrinsicCameraParams);

            Mat unwrappedPhaseX = cv::imread(string(".\\..\\..\\Tests\\Data\\normalEstimation\\HandlingSi_S1_SlopeX.tif"), cv::IMREAD_UNCHANGED);
            Mat unwrappedPhaseY = cv::imread(string(".\\..\\..\\Tests\\Data\\normalEstimation\\HandlingSi_S1_SlopeY.tif"), cv::IMREAD_UNCHANGED);
            Reporting::writePngImage(unwrappedPhaseX, directoryPathToStoreReport.string() + "\\unwrappedPhaseX.png");
            Reporting::writePngImage(unwrappedPhaseY, directoryPathToStoreReport.string() + "\\unwrappedPhaseY.png");
            psd::MPResult mpResult = ComputeMPNormalizedAndP(unwrappedPhaseX, unwrappedPhaseY, screenParams);

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

            std::map<int, std::complex<double>> refractiveIndexByWavelengthTable{
                { 300, 4.9760 + 1i * 4.199},
                { 325, 5.0574 + 1i * 3.2074},
                { 350, 5.4754 + 1i * 3.0024},
                { 375, 6.6971 + 1i * 1.4081},
                { 400, 5.5674 + 1i * 0.38612},
                { 425, 5.0070 + 1i * 0.21462},
                { 450, 4.6784 + 1i * 0.14851},
                { 475, 4.4571 + 1i * 0.094294},
                { 500, 4.2992 + 1i * 0.070425},
                { 525, 4.1807 + 1i * 0.055520},
                { 550, 4.0870 + 1i * 0.040882},
                { 575, 4.0109 + 1i * 0.031149},
                { 600, 3.9485 + 1i * 0.027397},
                { 625, 3.8968 + 1i * 0.021061},
                { 650, 3.8515 + 1i * 0.01646},
                { 675, 3.8154 + 1i * 0.01414},
                { 700, 3.7838 + 1i * 0.01217},
                { 725, 3.7567 + 1i * 0.010319},
                { 750, 3.7348 + 1i * 0.0090921},
                { 775, 3.7139 + 1i * 0.0079942},
                { 800, 3.6941 + 1i * 0.0065435}
            };

            std::map<int, std::vector<float>> reflectionCoeffsByWavelengthTable = {
                { 450, std::vector<float>{0.42002, 0.42002, 0.41989, 0.41931, 0.41762, 0.41377, 0.40679, 0.39997, 0.43395, 0.54787, 1.00}},
                { 550, std::vector<float>{0.36829, 0.36829, 0.36824, 0.36799, 0.36731, 0.36593, 0.36457, 0.37024, 0.43170, 0.56498, 1.00}},
                { 630, std::vector<float>{0.34907, 0.34907, 0.34904, 0.34893, 0.34866, 0.34829, 0.34919, 0.35978, 0.43147, 0.57132, 1.00}}
            };

            std::vector<int> wavelengthsList{
                450, 550, 630 };

            std::vector<float> incidentAnglesList{
                0 * CV_PI / 180,
                10 * CV_PI / 180,
                20 * CV_PI / 180,
                30 * CV_PI / 180,
                40 * CV_PI / 180,
                50 * CV_PI / 180,
                60 * CV_PI / 180,
                70 * CV_PI / 180,
                80 * CV_PI / 180,
                85 * CV_PI / 180,
                90 * CV_PI / 180 };

            // When

            Mat mask = CreateWaferMask(brightfield, 0.030, 150, 0);
            cv::Mat polynomialCorrection = AngularReflectionsCalibration(brightfield, mask, cmResult.CMNormalized, refractiveIndexByWavelengthTable, reflectionCoeffsByWavelengthTable, wavelengthsList, incidentAnglesList, screenParams, directoryPathToStoreReport);

            Reporting::writePngImage(polynomialCorrection, directoryPathToStoreReport.string() + "\\PolynomialCorrection.png");

            //filesystem::remove_all(directoryPathToStoreReport);
        }
        */
    };
}