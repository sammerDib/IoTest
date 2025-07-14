#include "SystemCalibration.hpp"
#include "CameraCalibration.hpp"
#include "CImageTypeConvertor.hpp"
#include "ReportingUtils.hpp"
#include "CheckerBoardsDetector.hpp"
#include "CShapeFinder.hpp"

#include <opencv2/calib3d.hpp>
#include <random>
#include "WaferDetector.hpp"

using namespace std;
using namespace cv;

namespace psd {
    namespace {
        /**
        * Use of a calibration mire to define the position of the camera relative to the wafer.
        *
        * This function returns the rotation and the translation vectors that transform a point expressed in the wafer coordinate referential to the camera coordinate referential :
        * waferCoordinate * RotationWaferToCamera + TranslationWaferToCamera = cameraCoordinate
        */
        ExtrinsicCameraParameters ComputeExtrinsicCameraParameters(Mat calibrationWaferImg, Mat cameraIntrinsic, Mat cameraDistortion, InputSystemParameters params, filesystem::path directoryPathToStoreReport);

        /**
        * Using phase map to define the position of the screen relative to the wafer
        *
        * This function returns the rotation and the translation vectors that transform a point expressed in the screen coordinate referential to the wafer coordinate referential :
        * screenCoordinate * RotationScreenToWafer + TranslationScreenToWafer = waferCoordinate
        */
        ExtrinsicScreenParameters ComputeExtrinsicScreenParameters(Mat phaseMapX, Mat phaseMapY, Mat mask, Mat cameraIntrinsic, Mat cameraDistortion, ExtrinsicCameraParameters extrinsicCameraParameters, InputSystemParameters params, filesystem::path directoryPathToStoreReport);

        /**
        * Compute mean value inside given area
        */
        float ComputeMeanValue(Mat img, Mat mask);
    }

    SystemParameters CalibrateSystem(Mat phaseMapX, Mat phaseMapY, Mat calibrationWaferImg, Mat cameraIntrinsic, Mat cameraDistortion, InputSystemParameters params, cv::Mat mask, filesystem::path directoryPathToStoreReport) {
        ExtrinsicCameraParameters cameraParams = ComputeExtrinsicCameraParameters(calibrationWaferImg, cameraIntrinsic, cameraDistortion, params, directoryPathToStoreReport);

        Mat maskWithoutEdge = !mask.empty() ? mask : CreateWaferMask(calibrationWaferImg, params.CheckerBoards.PixelSizeMm, params.WaferRadius, params.EdgeExclusion, directoryPathToStoreReport);
        ExtrinsicScreenParameters screenParams = ComputeExtrinsicScreenParameters(phaseMapX, phaseMapY, maskWithoutEdge, cameraIntrinsic, cameraDistortion, cameraParams, params, directoryPathToStoreReport);

        Mat rotationScreenToCamera = cameraParams.RWaferToCamera * screenParams.RScreenToWafer;
        Mat translationScreenToCamera = cameraParams.RWaferToCamera * screenParams.TScreenToWafer + cameraParams.TWaferToCamera;
        ExtrinsicSystemParameters systemParameters = ExtrinsicSystemParameters(rotationScreenToCamera, translationScreenToCamera);

        return SystemParameters(cameraParams, screenParams, systemParameters);
    }

    namespace {
        ExtrinsicCameraParameters ComputeExtrinsicCameraParameters(Mat calibrationWaferImg, Mat cameraIntrinsic, Mat cameraDistortion, InputSystemParameters params, filesystem::path directoryPathToStoreReport)
        {
            CornersPoints cornersPoints = ComputeCornersPoints(vector<Mat> {calibrationWaferImg}, params.CheckerBoards, true, directoryPathToStoreReport);
            vector<vector<Point3f>> objWaferCheckerBoards = cornersPoints.CornersObjPoints;
            vector<vector<Point2f>> imageCameraCheckerBoards = cornersPoints.CornersImgPoints;

            vector<Point3f> realWaferPoints;
            vector<Point2f> imageCameraPoints;

            if (params.CheckerBoards.UseAllCheckerBoards)
            {
                for (vector<Point3f> cornersObjWafer : objWaferCheckerBoards)
                {
                    realWaferPoints.insert(realWaferPoints.end(), cornersObjWafer.begin(), cornersObjWafer.end());
                }
                for (vector<Point2f> cornersImageCamera : imageCameraCheckerBoards)
                {
                    imageCameraPoints.insert(imageCameraPoints.end(), cornersImageCamera.begin(), cornersImageCamera.end());
                }
            }
            else if (!objWaferCheckerBoards.empty() && !imageCameraCheckerBoards.empty()) {
                realWaferPoints = objWaferCheckerBoards[0];
                imageCameraPoints = imageCameraCheckerBoards[0];
            }

            // Check validity
            bool validCornersPoints =
                !realWaferPoints.empty() &&
                !imageCameraPoints.empty() &&
                realWaferPoints.size() == imageCameraPoints.size();

            if (!validCornersPoints)
            {
                throw std::exception("Invalid checker boards detected.");
            }

            // Compute extrinsic parameters of camera (PnP apply undistort image camera points)
            Mat rotationWaferToCameraOmc;
            Mat translationWaferToCamera;

            bool success = solvePnP(realWaferPoints, imageCameraPoints, cameraIntrinsic, cameraDistortion, rotationWaferToCameraOmc, translationWaferToCamera);

            Mat rotationWaferToCamera = Mat::zeros(Size(3, 3), CV_32FC1);
            Rodrigues(rotationWaferToCameraOmc, rotationWaferToCamera);

            return ExtrinsicCameraParameters(rotationWaferToCamera, translationWaferToCamera);
        }

        ExtrinsicScreenParameters ComputeExtrinsicScreenParameters(cv::Mat phaseMapX, cv::Mat phaseMapY, Mat mask, cv::Mat cameraIntrinsic, cv::Mat cameraDistortion, ExtrinsicCameraParameters extrinsicCameraParameters, InputSystemParameters params, std::filesystem::path directoryPathToStoreReport)
        {
            // The value of the phases must be passed in metric (millimeter) to be able to establish the correspondence
            // between the coordinates on the wafer and the corresponding coordinates on the screen.
            cv::Mat metricPhaseMapX = phaseMapX * (params.FrangePixels * params.ScreenPixelSizeMm / CV_2PI);
            cv::Mat metricPhaseMapY = phaseMapY * (params.FrangePixels * params.ScreenPixelSizeMm / CV_2PI);

            // Randomly take pixels in valid area (wafer center without pattern)

            float mean = ComputeMeanValue(metricPhaseMapX, mask);
            float threshold = mean;

            int validPhaseNb = 0;
            for (int r = 0; r < metricPhaseMapX.size().height; r++) {
                for (int c = 0; c < metricPhaseMapX.size().width; c++) {
                    if (mask.at<uchar>(r, c) != 0)
                    {
                        float valueX = metricPhaseMapX.at<float>(r, c);
                        float valueY = metricPhaseMapY.at<float>(r, c);
                        bool isValidValue = valueX > threshold && valueY > threshold;
                        if (isValidValue)
                        {
                            validPhaseNb++;
                        }
                    }
                }
            }

            std::vector<int> subSampleOfValidPhaseId(validPhaseNb);
            std::generate(subSampleOfValidPhaseId.begin(), subSampleOfValidPhaseId.end(), [value = 0]() mutable { return value++; });
            std::shuffle(subSampleOfValidPhaseId.begin(), subSampleOfValidPhaseId.end(), std::mt19937{ std::random_device{}() });
            subSampleOfValidPhaseId.resize((size_t)params.NbPtsScreen);

            vector<Point2f> coordinatesOnCamera;
            vector<Point3f> coordinatesOnScreen;

            int validPhaseId = 0;
            for (int r = 0; r < metricPhaseMapX.size().height; r++) {
                for (int c = 0; c < metricPhaseMapX.size().width; c++) {
                    if (mask.at<uchar>(r, c) != 0)
                    {
                        bool isValidValue = metricPhaseMapX.at<float>(r, c) > threshold && metricPhaseMapY.at<float>(r, c) > threshold;
                        if (isValidValue)
                        {
                            bool isPartOfTheSample = std::find(subSampleOfValidPhaseId.begin(), subSampleOfValidPhaseId.end(), validPhaseId) != subSampleOfValidPhaseId.end();
                            if (isPartOfTheSample)
                            {
                                coordinatesOnCamera.push_back(Point2f((float)c, (float)r));
                                coordinatesOnScreen.push_back(Point3f(metricPhaseMapX.at<float>(r, c), metricPhaseMapY.at<float>(r, c), 0.0));
                            }
                            validPhaseId++;
                        }
                    }
                }
            }

            // Compute extrinsic parameters: Rotation and Translation vectors to transform wafer coordinate to IMAGE screen coordinate
            // (PnP apply undistort camera coordinates)
            Mat rotationImageScreenToCameraOmc;
            Mat translationImageScreenToCamera;
            bool success = solvePnP(coordinatesOnScreen, coordinatesOnCamera, cameraIntrinsic, cameraDistortion, rotationImageScreenToCameraOmc, translationImageScreenToCamera);

            Mat rotationImageScreenToCamera;
            Rodrigues(rotationImageScreenToCameraOmc, rotationImageScreenToCamera);

            Mat rotationImageScreenToWafer = extrinsicCameraParameters.RWaferToCamera.t() * rotationImageScreenToCamera;
            Mat translationImageScreenToWafer = extrinsicCameraParameters.RWaferToCamera.t() * (translationImageScreenToCamera - extrinsicCameraParameters.TWaferToCamera);

            // Image of the screen to real Screen location into wafer referential :
            // ImageScreenInWaferRef(RealScreenInWaferRef.x, RealScreenInWaferRef.y, -RealScreenInWaferRef.z)
            // Image screen is located as if the screen were behind the wafer, so we need to compute mirror symmetry to have REAL screen location

            Mat rotationWaferToImageScreen = rotationImageScreenToWafer.t();
            Mat rotationWaferToImageScreenOmc;
            Rodrigues(rotationWaferToImageScreen, rotationWaferToImageScreenOmc);
            rotationWaferToImageScreenOmc.at<double>(0, 0) *= -1;
            rotationWaferToImageScreenOmc.at<double>(1, 0) *= -1;
            Mat rotationWaferToScreen;
            Rodrigues(rotationWaferToImageScreenOmc, rotationWaferToScreen);
            Mat rotationScreenToWafer = rotationWaferToScreen.t();

            Mat translationScreenToWafer;
            translationImageScreenToWafer.copyTo(translationScreenToWafer);
            translationScreenToWafer.at<double>(2, 0) *= -1;

            return ExtrinsicScreenParameters(rotationScreenToWafer, translationScreenToWafer);
        }

        float ComputeMeanValue(Mat img, Mat mask)
        {
            float mean = 0;
            int count = 0;
            for (int r = 0; r < img.size().height; r++) {
                for (int c = 0; c < img.size().width; c++) {
                    if (mask.at<uchar>(r, c) != 0)
                    {
                        mean += img.at<float>(r, c);
                        count++;
                    }
                }
            }
            mean /= count;
            return mean;
        }
    }
}