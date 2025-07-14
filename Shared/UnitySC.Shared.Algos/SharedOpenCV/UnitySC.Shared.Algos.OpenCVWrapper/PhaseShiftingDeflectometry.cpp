#include "PhaseShiftingDeflectometry.h"
#include "Tools.h"

#include "CPhaseShiftingDeflectometry.hpp"
#include "WrappedPhaseMap.hpp"
#include "MultiperiodUnwrap.hpp"

using namespace System;

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    namespace {
        psd::FringesDisplacement ConvertToCppType(FringesDisplacement fringesDisplacement)
        {
            switch (fringesDisplacement)
            {
            case FringesDisplacement::X:
                return psd::FringesDisplacement::X;
            default:
            case FringesDisplacement::Y:
                return psd::FringesDisplacement::Y;
            }
        }

        psd::FitSurface ConvertToCppType(FitSurface fitSurface)
        {
            switch (fitSurface)
            {
            case FitSurface::PolynomeOrder2:
                return psd::FitSurface::PolynomeOrder2;
            case FitSurface::PolynomeOrder3:
                return psd::FitSurface::PolynomeOrder3;
            case FitSurface::PolynomeOrder4:
                return psd::FitSurface::PolynomeOrder4;
            default:
            case FitSurface::None:
                return psd::FitSurface::None;
            }
        }
    }

    PSDResult^ PhaseShiftingDeflectometry::ComputePhaseMap(array<ImageData^>^ input, PSDParams^ params) {
        // Process input parameters
        std::vector<cv::Mat> imgs;
        for each (ImageData ^ data in input) { imgs.push_back(CreateMatFromImageData(data)); }

        // Call native method
        WrappedPhaseMap phaseMapResult = psd::ComputePhaseMap(imgs, params->StepNb, ConvertToCppType(params->FringesDisplacement));
        cv::Mat wrappedPhaseMap = phaseMapResult.Phase;
        cv::Mat wrappedPhaseMap2 = phaseMapResult.Phase2;
        cv::Mat amplitude = phaseMapResult.Amplitude;
        cv::Mat background = phaseMapResult.Background;
        cv::Mat dark = phaseMapResult.Dark;

        // Process output result
        ImageData^ wrappedPhaseMapImg = gcnew ImageData();
        wrappedPhaseMapImg->ByteArray = CreateByteArrayFromMat(wrappedPhaseMap);
        wrappedPhaseMapImg->Type = CreateImageType(wrappedPhaseMap);
        wrappedPhaseMapImg->Height = wrappedPhaseMap.size().height;
        wrappedPhaseMapImg->Width = wrappedPhaseMap.size().width;

        ImageData^ wrappedPhaseMap2Img = gcnew ImageData();
        wrappedPhaseMap2Img->ByteArray = CreateByteArrayFromMat(wrappedPhaseMap2);
        wrappedPhaseMap2Img->Type = CreateImageType(wrappedPhaseMap2);
        wrappedPhaseMap2Img->Height = wrappedPhaseMap2.size().height;
        wrappedPhaseMap2Img->Width = wrappedPhaseMap2.size().width;

        ImageData^ amplitudeMapImg = gcnew ImageData();
        amplitudeMapImg->ByteArray = CreateByteArrayFromMat(amplitude);
        amplitudeMapImg->Type = CreateImageType(amplitude);
        amplitudeMapImg->Height = amplitude.size().height;
        amplitudeMapImg->Width = amplitude.size().width;

        ImageData^ backgroundImg = gcnew ImageData();
        backgroundImg->ByteArray = CreateByteArrayFromMat(background);
        backgroundImg->Type = CreateImageType(background);
        backgroundImg->Height = background.size().height;
        backgroundImg->Width = background.size().width;

        ImageData^ darkImg = gcnew ImageData();
        darkImg->ByteArray = CreateByteArrayFromMat(dark);
        darkImg->Type = CreateImageType(dark);
        darkImg->Height = dark.size().height;
        darkImg->Width = dark.size().width;

        PSDResult^ result = gcnew PSDResult();
        result->WrappedPhaseMap = wrappedPhaseMapImg;
        result->WrappedPhaseMap2 = wrappedPhaseMap2Img;
        result->Amplitude = amplitudeMapImg;
        result->Background = backgroundImg;
        result->Dark = darkImg;

        return result;
    }

    ImageData^ PhaseShiftingDeflectometry::ComputeMask(PSDResult^ wrappedPhaseResult, MaskParams^ maskParams) {
        // Process input parameters
        WrappedPhaseMap phaseMapX = WrappedPhaseMap(
            CreateMatFromImageData(wrappedPhaseResult->WrappedPhaseMap),
            CreateMatFromImageData(wrappedPhaseResult->WrappedPhaseMap2),
            CreateMatFromImageData(wrappedPhaseResult->Amplitude),
            CreateMatFromImageData(wrappedPhaseResult->Background),
            CreateMatFromImageData(wrappedPhaseResult->Dark));

        // Call native method
        cv::Mat mask = psd::ComputeMask(phaseMapX, maskParams->PixelSizeInMicrons, maskParams->WaferDiameterInMicrons, maskParams->WaferShiftXInMicrons, maskParams->WaferShiftYInMicrons, maskParams->FillEdgeExclusionInMicrons, maskParams->UseWaferFill);

        // Process output result
        ImageData^ maskImg = gcnew ImageData();
        maskImg->ByteArray = CreateByteArrayFromMat(mask);
        maskImg->Type = CreateImageType(mask);
        maskImg->Height = mask.size().height;
        maskImg->Width = mask.size().width;

        return maskImg;
    }

    ImageData^ PhaseShiftingDeflectometry::ComputeCurvature(PSDResult^ wrappedPhaseResult, ImageData^ mask, PSDParams^ params) {
        // Process input parameters
        WrappedPhaseMap phaseMap = WrappedPhaseMap(
            CreateMatFromImageData(wrappedPhaseResult->WrappedPhaseMap),
            CreateMatFromImageData(wrappedPhaseResult->WrappedPhaseMap2),
            CreateMatFromImageData(wrappedPhaseResult->Amplitude),
            CreateMatFromImageData(wrappedPhaseResult->Background),
            CreateMatFromImageData(wrappedPhaseResult->Dark));

        cv::Mat maskMat = CreateMatFromImageData(mask);

        // Call native method
        cv::Mat curvature = psd::ComputeCurvature(phaseMap, maskMat, params->StepNb, ConvertToCppType(params->FringesDisplacement));

        // Process output result
        ImageData^ curvatureImg = gcnew ImageData();
        curvatureImg->ByteArray = CreateByteArrayFromMat(curvature);
        curvatureImg->Type = CreateImageType(curvature);
        curvatureImg->Height = curvature.size().height;
        curvatureImg->Width = curvature.size().width;

        return curvatureImg;
    }

    ImageData^ PhaseShiftingDeflectometry::ComputeDark(ImageData^ darkXImg, ImageData^ darkYImg, ImageData^ mask, FitSurface removeBackgroundSurfaceMethod) {
        // Process input parameters
        cv::Mat darkX = CreateMatFromImageData(darkXImg);
        cv::Mat darkY = CreateMatFromImageData(darkYImg);
        cv::Mat maskMat = CreateMatFromImageData(mask);

        // Call native method
        cv::Mat dark = psd::ComputeDark(darkX, darkY, maskMat, ConvertToCppType(removeBackgroundSurfaceMethod));

        // Process output result
        ImageData^ darkImg = gcnew ImageData();
        darkImg->ByteArray = CreateByteArrayFromMat(dark);
        darkImg->Type = CreateImageType(dark);
        darkImg->Height = dark.size().height;
        darkImg->Width = dark.size().width;

        return darkImg;
    }

    float PhaseShiftingDeflectometry::CalibrateCurvatureDynamics(ImageData^ curvatureX, ImageData^ curvatureY, ImageData^ mask) {
        // Process input parameters
        cv::Mat curvatureXMat = CreateMatFromImageData(curvatureX);
        cv::Mat curvatureYMat = CreateMatFromImageData(curvatureY);
        cv::Mat maskMat = CreateMatFromImageData(mask);

        // Call native method
        float stdBackground = psd::CalibrateCurvatureDynamics(curvatureXMat, curvatureYMat, maskMat);

        // Process output result
        return stdBackground;
    }

    ImageData^ PhaseShiftingDeflectometry::ApplyDynamicCalibration(ImageData^ img, ImageData^ mask, float calibrationDynamicCoef, float noisyGrayLevel, float userDynamicCoef) {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Mat maskMat = CreateMatFromImageData(mask);

        // Call native method
        cv::Mat calibratedImg = psd::ApplyDynamicCalibration(imgMat, maskMat, calibrationDynamicCoef, noisyGrayLevel, userDynamicCoef);

        // Process output result
        ImageData^ calibratedImgData = gcnew ImageData();
        calibratedImgData->ByteArray = CreateByteArrayFromMat(calibratedImg);
        calibratedImgData->Type = CreateImageType(calibratedImg);
        calibratedImgData->Height = calibratedImg.size().height;
        calibratedImgData->Width = calibratedImg.size().width;

        return calibratedImgData;
    }

    ImageData^ PhaseShiftingDeflectometry::ApplyDynamicCoefficient(ImageData^ img, ImageData^ mask, float dynamicCoef, float percentageOfLowSaturation) {
        // Process input parameters
        cv::Mat imgMat = CreateMatFromImageData(img);
        cv::Mat maskMat = CreateMatFromImageData(mask);

        // Call native method
        cv::Mat dynamicImg = psd::ApplyDynamicCoefficient(imgMat, maskMat, dynamicCoef, percentageOfLowSaturation);

        // Process output result
        ImageData^ dynamicImgData = gcnew ImageData();
        dynamicImgData->ByteArray = CreateByteArrayFromMat(dynamicImg);
        dynamicImgData->Type = CreateImageType(dynamicImg);
        dynamicImgData->Height = dynamicImg.size().height;
        dynamicImgData->Width = dynamicImg.size().width;

        return dynamicImgData;
    }

    ImageData^ PhaseShiftingDeflectometry::MultiperiodUnwrap(array<ImageData^>^ wrappedPhaseMaps, ImageData^ mask, array<int>^ periods, int nbPeriod) {
        // Process input parameters
        std::vector<cv::Mat> imgsCpp;
        for each (ImageData ^ data in wrappedPhaseMaps) { imgsCpp.push_back(CreateMatFromImageData(data)); }

        cv::Mat maskCpp = CreateMatFromImageData(mask);

        std::vector<int> periodsCpp;
        for each (int period in periods) { periodsCpp.push_back(period); }

        // Call native method
        cv::Mat unwrappedPhaseMapCpp = phase_unwrapping::MultiperiodUnwrap(imgsCpp, maskCpp, periodsCpp, nbPeriod);

        // Process output result
        ImageData^ unwrappedPhaseMap = gcnew ImageData();
        unwrappedPhaseMap->ByteArray = CreateByteArrayFromMat(unwrappedPhaseMapCpp);
        unwrappedPhaseMap->Type = CreateImageType(unwrappedPhaseMapCpp);
        unwrappedPhaseMap->Height = unwrappedPhaseMapCpp.size().height;
        unwrappedPhaseMap->Width = unwrappedPhaseMapCpp.size().width;

        return unwrappedPhaseMap;
    }

    ImageData^ PhaseShiftingDeflectometry::SubstractPlaneFromUnwrapped(ImageData^ unwrappedPhase, ImageData^ mask)
    {
        cv::Mat unwrappedPhaseMat = CreateMatFromImageData(unwrappedPhase);
        cv::Mat maskMat = CreateMatFromImageData(mask);

        phase_unwrapping::SubstractPlaneFromUnwrapped(unwrappedPhaseMat, maskMat);

        ImageData^ substractedPlaneUnwrapped = gcnew ImageData();
        substractedPlaneUnwrapped->ByteArray = CreateByteArrayFromMat(unwrappedPhaseMat);
        substractedPlaneUnwrapped->Type = CreateImageType(unwrappedPhaseMat);
        substractedPlaneUnwrapped->Height = unwrappedPhaseMat.size().height;
        substractedPlaneUnwrapped->Width = unwrappedPhaseMat.size().width;

        return substractedPlaneUnwrapped;
    }
}