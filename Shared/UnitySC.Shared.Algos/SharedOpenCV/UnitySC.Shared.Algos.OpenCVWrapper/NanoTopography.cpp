#include "NanoTopography.h"
#include "NormalEstimation.hpp"
#include "Tools.h"
#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    void NanoTopography::ComputeNanoTopography(ImageData^ unwrappedX, ImageData^ unwrappedY, ImageData^ mask, CalibrationParameters^ calibrationParams, ExtrinsicCameraParameters^ extrinsicCameraParams, ExtrinsicScreenParameters^ extrinsicScreenParams, float screenPixelSizeInMm, int fringePeriodInPixels, String^ reportPath)
    {
        std::string cppStringReportPath = "";

        if (reportPath != nullptr)
        {
            cppStringReportPath = CSharpStringToCppString(reportPath);
        }

        cv::Mat unwrappedXMat = CreateMatFromImageData(unwrappedX);
        cv::Mat unwrappedYMat = CreateMatFromImageData(unwrappedY);

        cv::Mat maskMat = cv::Mat::ones(unwrappedXMat.size(), CV_8UC1);
        if (mask != (ImageData^) nullptr)
        {
            maskMat = CreateMatFromImageData(mask);
        }

        cv::Mat cameraIntrinsicMatrix = cv::Mat::zeros(3, 3, CV_64FC1);
        cameraIntrinsicMatrix.at<double>(0, 0) = calibrationParams->CameraIntrinsic[0, 0];
        cameraIntrinsicMatrix.at<double>(0, 1) = calibrationParams->CameraIntrinsic[0, 1];
        cameraIntrinsicMatrix.at<double>(0, 2) = calibrationParams->CameraIntrinsic[0, 2];
        cameraIntrinsicMatrix.at<double>(1, 0) = calibrationParams->CameraIntrinsic[1, 0];
        cameraIntrinsicMatrix.at<double>(1, 1) = calibrationParams->CameraIntrinsic[1, 1];
        cameraIntrinsicMatrix.at<double>(1, 2) = calibrationParams->CameraIntrinsic[1, 2];
        cameraIntrinsicMatrix.at<double>(2, 0) = calibrationParams->CameraIntrinsic[2, 0];
        cameraIntrinsicMatrix.at<double>(2, 1) = calibrationParams->CameraIntrinsic[2, 1];
        cameraIntrinsicMatrix.at<double>(2, 2) = calibrationParams->CameraIntrinsic[2, 2];

        cv::Mat cameraDistorsionMatrix = cv::Mat::zeros(5, 1, CV_64FC1);
        cameraDistorsionMatrix.at<double>(0, 0) = calibrationParams->Distortion[0];
        cameraDistorsionMatrix.at<double>(1, 0) = calibrationParams->Distortion[1];
        cameraDistorsionMatrix.at<double>(2, 0) = calibrationParams->Distortion[2];
        cameraDistorsionMatrix.at<double>(3, 0) = calibrationParams->Distortion[3];
        cameraDistorsionMatrix.at<double>(4, 0) = calibrationParams->Distortion[4];

        std::vector<cv::Mat> rotationMatrix;
        std::vector<cv::Mat> translationMatrix;

        psd::CalibrationParameters intrinsicCameraParamsNative = psd::CalibrationParameters(cameraIntrinsicMatrix, cameraDistorsionMatrix, rotationMatrix, translationMatrix, calibrationParams->RMS);

        cv::Mat rWaferToCamera = cv::Mat::zeros(3, 3, CV_64F);
        cv::Mat tWaferToCamera = cv::Mat::zeros(3, 1, CV_64F);

        rWaferToCamera.at<double>(0, 0) = extrinsicCameraParams->RWaferToCamera[0, 0];
        rWaferToCamera.at<double>(0, 1) = extrinsicCameraParams->RWaferToCamera[0, 1];
        rWaferToCamera.at<double>(0, 2) = extrinsicCameraParams->RWaferToCamera[0, 2];
        rWaferToCamera.at<double>(1, 0) = extrinsicCameraParams->RWaferToCamera[1, 0];
        rWaferToCamera.at<double>(1, 1) = extrinsicCameraParams->RWaferToCamera[1, 1];
        rWaferToCamera.at<double>(1, 2) = extrinsicCameraParams->RWaferToCamera[1, 2];
        rWaferToCamera.at<double>(2, 0) = extrinsicCameraParams->RWaferToCamera[2, 0];
        rWaferToCamera.at<double>(2, 1) = extrinsicCameraParams->RWaferToCamera[2, 1];
        rWaferToCamera.at<double>(2, 2) = extrinsicCameraParams->RWaferToCamera[2, 2];
        tWaferToCamera.at<double>(0, 0) = extrinsicCameraParams->TWaferToCamera[0];
        tWaferToCamera.at<double>(1, 0) = extrinsicCameraParams->TWaferToCamera[1];
        tWaferToCamera.at<double>(2, 0) = extrinsicCameraParams->TWaferToCamera[2];

        psd::ExtrinsicCameraParameters extrinsicCameraParamsNative = psd::ExtrinsicCameraParameters(rWaferToCamera, tWaferToCamera);

        cv::Mat rScreenToWafer = cv::Mat::zeros(3, 3, CV_64F);
        cv::Mat tScreenToWafer = cv::Mat::zeros(3, 1, CV_64F);

        rScreenToWafer.at<double>(0, 0) = extrinsicScreenParams->RScreenToWafer[0, 0];
        rScreenToWafer.at<double>(0, 1) = extrinsicScreenParams->RScreenToWafer[0, 1];
        rScreenToWafer.at<double>(0, 2) = extrinsicScreenParams->RScreenToWafer[0, 2];
        rScreenToWafer.at<double>(1, 0) = extrinsicScreenParams->RScreenToWafer[1, 0];
        rScreenToWafer.at<double>(1, 1) = extrinsicScreenParams->RScreenToWafer[1, 1];
        rScreenToWafer.at<double>(1, 2) = extrinsicScreenParams->RScreenToWafer[1, 2];
        rScreenToWafer.at<double>(2, 0) = extrinsicScreenParams->RScreenToWafer[2, 0];
        rScreenToWafer.at<double>(2, 1) = extrinsicScreenParams->RScreenToWafer[2, 1];
        rScreenToWafer.at<double>(2, 2) = extrinsicScreenParams->RScreenToWafer[2, 2];
        tScreenToWafer.at<double>(0, 0) = extrinsicScreenParams->TScreenToWafer[0];
        tScreenToWafer.at<double>(1, 0) = extrinsicScreenParams->TScreenToWafer[1];
        tScreenToWafer.at<double>(2, 0) = extrinsicScreenParams->TScreenToWafer[2];

        psd::ExtrinsicScreenParameters screenParams = psd::ExtrinsicScreenParameters(rScreenToWafer, tScreenToWafer);

        psd::CMResult cmResult = psd::ComputeCMNormalizedAndM(unwrappedXMat.size(), intrinsicCameraParamsNative, extrinsicCameraParamsNative);
        psd::MPResult mpResult = ComputeMPNormalizedAndP(cmResult.M, unwrappedXMat, unwrappedYMat, screenParams, screenPixelSizeInMm, fringePeriodInPixels);

        if (cppStringReportPath != "")
        {
            cv::Mat mX, mY, mZ;
            std::vector<cv::Mat> mChannels(3);
            split(cmResult.M, mChannels);
            mX = mChannels[0];
            mY = mChannels[1];
            mZ = mChannels[2];
            cv::imwrite(cppStringReportPath + "\\mX.tif", mX);
            cv::imwrite(cppStringReportPath + "\\mY.tif", mY);
            cv::imwrite(cppStringReportPath + "\\mZ.tif", mZ);
        }

        cv::Mat normals = psd::ComputeNormalizedNormal(cmResult.M, cmResult.CMNormalized, mpResult.MPNormalized, maskMat, cppStringReportPath);
    }
}