#include "PSDCalibration.h"

#include "CameraCalibration.hpp"
#include "SystemCalibration.hpp"
#include "CheckerBoardsSettings.hpp"
#include "CheckerBoardOrigins.hpp"
#include "InputSystemParameters.hpp"
#include "NormalEstimation.hpp"
#include "WaferDetector.hpp"
#include "AngularReflectionsCalibration.hpp"

using namespace System;
using namespace std::complex_literals;

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    ImageData^ PSDCalibration::ApplyUniformityCorrection(ImageData^ brightfield, ImageData^ correctionImage, float pixelSizeInMm, float waferRadiusInMm, int targetSaturationLevel, float acceptablePercentageOfSaturatedPixels)
    {
        cv::Mat brightfieldMat = CreateMatFromImageData(brightfield);
        cv::Mat correctionImageMat = CreateMatFromImageData(correctionImage);

        cv::Mat mask = psd::CreateWaferMask(brightfieldMat, pixelSizeInMm, waferRadiusInMm, 0);

        cv::Mat resultImgMat = psd::ApplyUniformityCorrection(brightfieldMat, correctionImageMat, mask, targetSaturationLevel, acceptablePercentageOfSaturatedPixels);

        ImageData^ resultImg = gcnew ImageData();
        resultImg->ByteArray = CreateByteArrayFromMat(resultImgMat);
        resultImg->Type = CreateImageType(brightfieldMat);
        resultImg->Height = brightfield->Height;
        resultImg->Width = brightfield->Width;

        return resultImg;
    }

    ImageData^ PSDCalibration::CalibrateUniformityCorrection(ImageData^ brightfield, float waferRadiusInMm, float pixelSizeInMm, CalibrationParameters^ calibrationParams, ExtrinsicCameraParameters^ extrinsicCameraParams, ExtrinsicScreenParameters^ extrinsicScreenParams, Dictionary<int, Tuple<double, double>^>^ refractiveIndexByWavelength, array<int>^ wavelengths, double patternThresholdForPolynomialFit, bool screenPolarisationIsVertical, String^ reportPath)
    {
        std::string cppStringReportPath = "";

        if (reportPath != nullptr)
        {
            cppStringReportPath = CSharpStringToCppString(reportPath);
        }

        cv::Mat brightfieldMat = CreateMatFromImageData(brightfield);

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

        std::map<int, std::complex<double>> refractiveIndexByWavelengthNative;

        for each (KeyValuePair<int, Tuple<double, double>^>^ kvp in refractiveIndexByWavelength)
        {
            int wavelength = kvp->Key;
            double realPart = kvp->Value->Item1;
            double imaginaryPart = kvp->Value->Item2;
            std::complex<double> complexNumber = realPart + 1i * imaginaryPart;
            refractiveIndexByWavelengthNative.insert(std::pair<int, std::complex<double>>(wavelength, complexNumber));
        }

        std::vector<int> wavelengthsNative;
        wavelengthsNative.resize(wavelengths->Length);
        pin_ptr<int> pinnedWavelengths = &wavelengths[0];
        std::memcpy(wavelengthsNative.data(), pinnedWavelengths, wavelengths->Length * sizeof(int));

        psd::CMResult cmResult = ComputeCMNormalizedAndM(brightfieldMat.size(), intrinsicCameraParamsNative, extrinsicCameraParamsNative);

        cv::Mat mask = psd::CreateWaferMask(brightfieldMat, pixelSizeInMm, waferRadiusInMm, 0);

        cv::Mat polynomialCorrection = psd::AngularReflectionsCalibration(brightfieldMat, mask, cmResult.CMNormalized, refractiveIndexByWavelengthNative, wavelengthsNative, screenParams, patternThresholdForPolynomialFit, screenPolarisationIsVertical, cppStringReportPath);


        ImageData^ result = gcnew ImageData();
        result->ByteArray = CreateByteArrayFromMat(polynomialCorrection);
        result->Type = CreateImageType(polynomialCorrection);
        result->Height = brightfield->Height;
        result->Width = brightfield->Width;

        return result;
    }

	CalibrationParameters^ PSDCalibration::CalibrateCamera(array<ImageData^>^ chessboardImages, CheckerBoardsSettings^ checkerBoardsSettings, bool fixAspectRatio) {
		// Process input parameters
		std::vector<cv::Mat> imgs;
		for each (ImageData ^ data in chessboardImages) { imgs.push_back(CreateMatFromImageData(data)); }
		psd::CheckerBoardsOrigins checkerBoardOrigins = psd::CheckerBoardsOrigins(
			cv::Point2f(checkerBoardsSettings->CheckerBoardsTopLeftOrigins->LeftCheckerBoardOrigin->X, checkerBoardsSettings->CheckerBoardsTopLeftOrigins->LeftCheckerBoardOrigin->Y),
			cv::Point2f(checkerBoardsSettings->CheckerBoardsTopLeftOrigins->BottomCheckerBoardOrigin->X, checkerBoardsSettings->CheckerBoardsTopLeftOrigins->BottomCheckerBoardOrigin->Y),
			cv::Point2f(checkerBoardsSettings->CheckerBoardsTopLeftOrigins->RightCheckerBoardOrigin->X, checkerBoardsSettings->CheckerBoardsTopLeftOrigins->RightCheckerBoardOrigin->Y),
			cv::Point2f(checkerBoardsSettings->CheckerBoardsTopLeftOrigins->TopCheckerBoardOrigin->X, checkerBoardsSettings->CheckerBoardsTopLeftOrigins->TopCheckerBoardOrigin->Y));
		psd::CheckerBoardsSettings inputSettings = psd::CheckerBoardsSettings(
			checkerBoardOrigins,
			checkerBoardsSettings->SquareXNb,
			checkerBoardsSettings->SquareYNb,
			checkerBoardsSettings->SquareSizeMm,
			checkerBoardsSettings->PixelSizeMm,
			checkerBoardsSettings->UseAllCheckerBoards);

		// Call native method
		psd::CalibrationParameters intrinsicCameraParams = psd::CalibrateCamera(imgs, inputSettings, fixAspectRatio);
		double fx = intrinsicCameraParams.CameraIntrinsic.at<double>(0, 0);
		double fy = intrinsicCameraParams.CameraIntrinsic.at<double>(1, 1);
		double cx = intrinsicCameraParams.CameraIntrinsic.at<double>(0, 2);
		double cy = intrinsicCameraParams.CameraIntrinsic.at<double>(1, 2);
		double k1 = intrinsicCameraParams.Distortion.at<double>(0, 0);
		double k2 = intrinsicCameraParams.Distortion.at<double>(1, 0);
		double p1 = intrinsicCameraParams.Distortion.at<double>(2, 0);
		double p2 = intrinsicCameraParams.Distortion.at<double>(3, 0);
		double k3 = intrinsicCameraParams.Distortion.at<double>(4, 0);
		double rms = intrinsicCameraParams.RMS;

		// Process output result
		array<double, 2>^ cameraIntrinsicMatrix = gcnew array<double, 2>{ {fx, 0, cx}, { 0, fy, cy }, { 0, 0, 1 } };

		array<double>^ distorsionVector = gcnew array<double> {k1, k2, p1, p2, k3};

		CalibrationParameters^ result = gcnew CalibrationParameters(cameraIntrinsicMatrix, distorsionVector, rms);

		return result;
	}

	SystemParameters^ PSDCalibration::CalibrateSystem(ImageData^ phaseMapX, ImageData^ phaseMapY, ImageData^ calibrationWaferImg, array<double, 2>^ cameraIntrinsic, array<double>^ cameraDistortion, InputSystemParameters^ params) {
		// Process input parameters
		cv::Mat phaseMapXMat = CreateMatFromImageData(phaseMapX);
		cv::Mat phaseMapYMat = CreateMatFromImageData(phaseMapY);
		cv::Mat checkerBoardImg = CreateMatFromImageData(calibrationWaferImg);

		cv::Mat cameraIntrinsicMatrix = cv::Mat::zeros(cv::Size(3, 3), CV_64FC1);
		cameraIntrinsicMatrix.at<double>(0, 0) = cameraIntrinsic[0, 0];
		cameraIntrinsicMatrix.at<double>(0, 1) = cameraIntrinsic[0, 1];
		cameraIntrinsicMatrix.at<double>(0, 2) = cameraIntrinsic[0, 2];
		cameraIntrinsicMatrix.at<double>(1, 0) = cameraIntrinsic[1, 0];
		cameraIntrinsicMatrix.at<double>(1, 1) = cameraIntrinsic[1, 1];
		cameraIntrinsicMatrix.at<double>(1, 2) = cameraIntrinsic[1, 2];
		cameraIntrinsicMatrix.at<double>(2, 0) = cameraIntrinsic[2, 0];
		cameraIntrinsicMatrix.at<double>(2, 1) = cameraIntrinsic[2, 1];
		cameraIntrinsicMatrix.at<double>(2, 2) = cameraIntrinsic[2, 2];

		cv::Mat cameraDistorsionMatrix = cv::Mat::zeros(cv::Size(1, 5), CV_64FC1);
		cameraDistorsionMatrix.at<double>(0, 0) = cameraDistortion[0];
		cameraDistorsionMatrix.at<double>(1, 0) = cameraDistortion[1];
		cameraDistorsionMatrix.at<double>(2, 0) = cameraDistortion[2];
		cameraDistorsionMatrix.at<double>(3, 0) = cameraDistortion[3];
		cameraDistorsionMatrix.at<double>(4, 0) = cameraDistortion[4];

		psd::CheckerBoardsOrigins checkerBoardOrigins = psd::CheckerBoardsOrigins(
			cv::Point2f(params->CheckerBoards->CheckerBoardsTopLeftOrigins->LeftCheckerBoardOrigin->X, params->CheckerBoards->CheckerBoardsTopLeftOrigins->LeftCheckerBoardOrigin->Y),
			cv::Point2f(params->CheckerBoards->CheckerBoardsTopLeftOrigins->BottomCheckerBoardOrigin->X, params->CheckerBoards->CheckerBoardsTopLeftOrigins->BottomCheckerBoardOrigin->Y),
			cv::Point2f(params->CheckerBoards->CheckerBoardsTopLeftOrigins->RightCheckerBoardOrigin->X, params->CheckerBoards->CheckerBoardsTopLeftOrigins->RightCheckerBoardOrigin->Y),
			cv::Point2f(params->CheckerBoards->CheckerBoardsTopLeftOrigins->TopCheckerBoardOrigin->X, params->CheckerBoards->CheckerBoardsTopLeftOrigins->TopCheckerBoardOrigin->Y));
		psd::CheckerBoardsSettings checkerBoardsSettings = psd::CheckerBoardsSettings(
			checkerBoardOrigins,
			params->CheckerBoards->SquareXNb,
			params->CheckerBoards->SquareYNb,
			params->CheckerBoards->SquareSizeMm,
			params->CheckerBoards->PixelSizeMm,
			params->CheckerBoards->UseAllCheckerBoards);
		psd::InputSystemParameters inputSettings = psd::InputSystemParameters(
			checkerBoardsSettings,
			params->EdgeExclusion,
			params->WaferRadius,
			params->NbPtsScreen,
			params->FrangePixels,
			params->ScreenPixelSizeMm);

		// Call native method
		psd::SystemParameters system = psd::CalibrateSystem(phaseMapXMat, phaseMapYMat, checkerBoardImg, cameraIntrinsicMatrix, cameraDistorsionMatrix, inputSettings);

		psd::ExtrinsicCameraParameters cameraParams = system.ExtrinsicCamera;
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

		psd::ExtrinsicScreenParameters screenParams = system.ExtrinsicScreen;
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

		psd::ExtrinsicSystemParameters systemParams = system.ExtrinsicSystem;
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

		// Process output result
		array<double, 2>^ RWaferToCameraMatrix = gcnew array<double, 2> {
			{ cameraParams_r00, cameraParams_r01, cameraParams_r02},
			{ cameraParams_r10, cameraParams_r11, cameraParams_r12 },
			{ cameraParams_r20, cameraParams_r21, cameraParams_r22 } };
		array<double>^ TWaferToCameraVector = gcnew array<double> {
			cameraParams_t0, cameraParams_t1, cameraParams_t2};
		ExtrinsicCameraParameters^ extrinsicCameraParams = gcnew ExtrinsicCameraParameters(RWaferToCameraMatrix, TWaferToCameraVector);

		array<double, 2>^ RScreenToWaferMatrix = gcnew array<double, 2> {
			{ screenParams_r00, screenParams_r01, screenParams_r02},
			{ screenParams_r10, screenParams_r11, screenParams_r12 },
			{ screenParams_r20, screenParams_r21, screenParams_r22 } };
		array<double>^ TScreenToWaferVector = gcnew array<double> {
			screenParams_t0, screenParams_t1, screenParams_t2};
		ExtrinsicScreenParameters^ extrinsicScreenParams = gcnew ExtrinsicScreenParameters(RScreenToWaferMatrix, TScreenToWaferVector);

		array<double, 2>^ RScreenToCameraMatrix = gcnew array<double, 2> {
			{ systemParams_r00, systemParams_r01, systemParams_r02},
			{ systemParams_r10, systemParams_r11, systemParams_r12 },
			{ systemParams_r20, systemParams_r21, systemParams_r22 } };
		array<double>^ TScreenToCameraVector = gcnew array<double> {
			systemParams_t0, systemParams_t1, systemParams_t2};
		ExtrinsicSystemParameters^ extrinsicSystemParams = gcnew ExtrinsicSystemParameters(RScreenToCameraMatrix, TScreenToCameraVector);

		SystemParameters^ result = gcnew SystemParameters(extrinsicCameraParams, extrinsicScreenParams, extrinsicSystemParams);

		return result;
	}
}