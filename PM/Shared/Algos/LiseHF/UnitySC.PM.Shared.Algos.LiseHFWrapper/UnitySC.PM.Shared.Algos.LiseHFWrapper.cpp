#include "UnitySC.PM.Shared.Algos.LiseHFWrapper.h"
#include "LiseHFMacros.h"

#pragma unmanaged
#include "olovia.h"
#include <memory.h>
#include <ctime>

#pragma managed
using namespace System;
using namespace System::Collections::Generic;

namespace UnitySCPMSharedAlgosLiseHFWrapper
{
#pragma managed(push, off)
    void NativeMemCopy2(double* pSrc, double* pDest, unsigned int count)
    {
        auto arrSize = count * sizeof(double);
        memcpy_s(pDest, arrSize, pSrc, arrSize);
    }

    void NativeZeroMemset(double* pPtr, unsigned int count)
    {
        auto arrSize = count * sizeof(double);
        memset(pPtr, 0, arrSize);
    }

    int RandUniqueID()
    {
        srand((unsigned)time(0));
        return rand();
    }

#pragma managed(pop)

    LiseHFAlgoReturns^ Olovia_Algos::Compute(LiseHFAlgoInputs^ inputs)
    {
        if (inputs == nullptr)
            return gcnew LiseHFAlgoReturns(false, false, false, "Algo INPUTS is Null ", nullptr);

        String^ errorValidityMessage = "Algo Inputs is invalid due to the following issue(s) :\n";
        if (!inputs->CheckValidity(errorValidityMessage))
            return gcnew LiseHFAlgoReturns(false, false, false, errorValidityMessage, nullptr);

        int No_Layer = (int)inputs->DepthLayers->GetLayerCount();

        inputs->ComputeNatives();

        pin_ptr<double> pinWaveLengthPtr = &(inputs->Wavelength_nm->ToArray())[0];
        auto wavesignalpixelsCount = inputs->Wavelength_nm->Count;
        //   double* pPinWave = pinWaveLengthPtr;
        double* pNativeWaveLengthSignal = new double[wavesignalpixelsCount];
        NativeMemCopy2(pinWaveLengthPtr, pNativeWaveLengthSignal, wavesignalpixelsCount);

        double* pNativeRefindex_Imaginary = new double[No_Layer];
        NativeZeroMemset(pNativeRefindex_Imaginary, No_Layer);

        const int iNo_char = 512;
        //char nativeLibAdditional_message[iNo_char] = "";
        stringstream nativeLibAdditional_message("");

        bool DebugOutputToFile = false;
        int DebugOutputUniqueID = 0;
        if (DebugOutputToFile)
            DebugOutputUniqueID = RandUniqueID();

        bool bFT_done = false;
        bool bSignal_analysis_done = false;
        double dAsymptStdErr, dNormalizedResidual; //estimated TSV and layer thicknesses, asymptotic standard error for it, sum of remaining squares
        double dSignalQuality; //quality of the signal: amplitude of highest peak in window / dResidual of least square fit; the larger the better;
        double dMaxPeakFFTSignal_WithinWindowSearch = 0.0;
        double intensityfactor = inputs->Spectrum->IntegrationTime_ms / inputs->RefSpectrum->IntegrationTime_ms;

        int FTDim = inputs->DepthLayers->_FTdim; // Fourier Transform dimension
        int NoDetectedPeaks = 0; //number of analyzed peaks is equal to number of material interfaces at the top of the TSV; no layer <=> 1 interface (Si-air)

        //
        // Enter parameters hell nightmare
        //---------------------------------------------
        //  _          _ _       ,    ,    /\   /\
        // | |        | | |     /( /\ )\  _\ \_/ /_
        // | |__   ___| | |     |\_||_/| < \_   _/ >
        // | '_ \ / _ \ | |     \______/  \|0   0|/
        // | | | |  __/ | |       _\/_   _(_  ^  _)_
        // |_| |_|\___|_|_|      ( () ) /`\|V"""V|/`\
        //                         {}   \  \_____/  /
        //                         ()   /\   )=(   /\
        //                         {}  /  \_/\=/\_/  \
        //
        //----------------------------------------------
        vLISE_HF_main(inputs->TSVDiameter_um,
            No_Layer,
            inputs->DepthLayers->GetNativeOpticalDepths(), // optical depth or thickness
            inputs->DepthLayers->GetNativeOpticalTolerancesSearch(), // optical tolerance aka spectrum half search window size
            inputs->DepthLayers->GetNativeRefIndex(), // Refractive Index (Real part)
            pNativeRefindex_Imaginary, // Refractive Index (Imaginary part) -- see if later use (cf W.Iff) or remove when clean
            wavesignalpixelsCount, // raw signal spectrometers "pixels" (aka its size)
            pNativeWaveLengthSignal, // raw signal lambdas (X) - nm
            inputs->DarkSpectrum->GetNativeRawSignal(), // Dark Signal Calibration
            inputs->RefSpectrum->GetNativeRawSignal(), // Reference Signal Calibration
            inputs->Spectrum->GetNativeRawSignal(), // raw signal spectrum (Y) - cnt
            nullptr, //planar layers Not Yet use -- see if use later (cf W.Iff) or remove when clean
            FTDim, // varaible en reférence qui n'a pas lieu d'etre -- remove passage par référence when clean
            (int)inputs->OpMode,
            inputs->PeakDetectionOnRight,
            inputs->NewPeakDetection,
            inputs->Threshold_signal_pct,
            inputs->Threshold_peak_pct,
            0.0, //desired remaining z-resolution after smoothening of FT-data -- see if use later (cf W.Iff) or remove when clean
            DebugOutputToFile,
            DebugOutputUniqueID,
            dAsymptStdErr, dNormalizedResidual, dSignalQuality, 
            &nativeLibAdditional_message,//nativeLibAdditional_message, iNo_char,
            inputs->DepthLayers->_pFTMod,
            inputs->DepthLayers->_pFTz,
            NoDetectedPeaks,
            inputs->DepthLayers->_pNativePeaks,
            inputs->DepthLayers->_pNativePeaksAmplitude,
            inputs->DepthLayers->_pNativeMeasuredDepths,
            bFT_done,
            bSignal_analysis_done, 
            dMaxPeakFFTSignal_WithinWindowSearch);

        SAFE_DELETE_ARRAY(pNativeRefindex_Imaginary);
        SAFE_DELETE_ARRAY(pNativeWaveLengthSignal);

        LiseHFAlgoOutputs^ outputs = gcnew LiseHFAlgoOutputs();

        if (bFT_done)
        {
            outputs->FTTx = gcnew List<double>(FTDim / 2 + 1);
            outputs->FTTy = gcnew List<double>(FTDim / 2 + 1);
            double* ptrfftx = inputs->DepthLayers->_pFTz;
            double* ptrffty = inputs->DepthLayers->_pFTMod;
            for (int i = 0; i < (FTDim / 2 + 1); i++, ptrfftx++, ptrffty++)
            {
                outputs->FTTx->Add(*ptrfftx);
                outputs->FTTy->Add(*ptrffty);
            }

            outputs->SaturationPercentage = 100.0 * inputs->Spectrum->CalcSaturationPct() * intensityfactor;
            outputs->ThresholdSignal = inputs->DepthLayers->_pFTMod[0] * inputs->Threshold_signal_pct;
            outputs->ThresholdPeak = dMaxPeakFFTSignal_WithinWindowSearch * inputs->Threshold_peak_pct; // could be 0 if signal not analyzed coorectly or if no peak in no noise zone
        }

        if (bSignal_analysis_done)
        {
            outputs->PeaksX = gcnew List<double>(NoDetectedPeaks);
            outputs->PeaksY = gcnew List<double>(NoDetectedPeaks);
            outputs->MeasuredDepths = gcnew List<double>(NoDetectedPeaks);
            for (int nlayer = 0; nlayer < NoDetectedPeaks; nlayer++)
            {
                outputs->PeaksX->Add(inputs->DepthLayers->_pNativePeaks[nlayer]);
                outputs->PeaksY->Add(inputs->DepthLayers->_pNativePeaksAmplitude[nlayer]);
                outputs->MeasuredDepths->Add(inputs->DepthLayers->_pNativeMeasuredDepths[nlayer]);
            }

            outputs->NormalizedResidual = dNormalizedResidual;
            outputs->dAsymptStdErr = dAsymptStdErr;
            outputs->Quality = dSignalQuality;
        }

        String^ libmessage = gcnew String(nativeLibAdditional_message.str().c_str());
        bool bSuccess = false;
        if (inputs->OpMode == LiseHFMode::FFTOnly)
            bSuccess = bFT_done;
        else
            bSuccess = bSignal_analysis_done;

        return gcnew LiseHFAlgoReturns(bSuccess, bFT_done, bSignal_analysis_done, libmessage, outputs);
    }

    LiseHFBeamPositionReturns^ Olovia_Algos::ComputeBeamPosition(LiseHFBeamPositionInputs^ inputs)
    {
        if (inputs == nullptr)
            return gcnew LiseHFBeamPositionReturns(false, 0.0, 0.0, "INPUTS are Null ");

        String^ errorValidityMessage = "Beam Inputs are invalid due to the following issue(s) :\n";
        if (!inputs->CheckValidity(errorValidityMessage))
            return gcnew LiseHFBeamPositionReturns(false, 0.0, 0.0, errorValidityMessage);

        const int iNo_char = 512;
        char nativeLibAdditional_message[iNo_char] = "";

        bool bSuccess = false;
        double dBackground = 6.5;
        double dMaxIntensity = 255.0;
        double dNorm = DBL_MAX;
        double dAmpl = 255.0 * 0.7;
        double dWeightedNorm = DBL_MAX;

        int iNoLines = inputs->Height;
        int iNoColumns = inputs->Width;

        int iNoLinesSub = (int)(iNoLines * 0.5 + 0.5);
        iNoLinesSub = 2 * (iNoLinesSub / 2 + 1); 
        int iNoColumnsSub = iNoLinesSub;
        double dxGauss = 0.0;
        double dyGauss = 0.0; //The Gaussian spot is approx. at the centre of the image.
        double dRadius = 11.0;
        double dRatioOfAxisOfEllipse = 0.0;
        double dAngleOfEllipse = 0.0;
        double dPixelSize_x = inputs->PixelSizeX;
        double dPixelSize_y = inputs->PixelSizeY;
        int* piImage = inputs->GetNativeImgPtr();
        double* pdImageSub = new double[iNoLinesSub * iNoColumnsSub];

        bSuccess = bBeamProfiler(iNoLines, iNoLinesSub, dPixelSize_y, iNoColumns, iNoColumnsSub, dPixelSize_x, piImage, pdImageSub, 
            dAmpl, dxGauss, dyGauss, dRadius, dBackground, dMaxIntensity, dNorm, dWeightedNorm,
            dRatioOfAxisOfEllipse, dAngleOfEllipse);

        SAFE_DELETE_ARRAY(pdImageSub);

        String^ libmessage = gcnew String(nativeLibAdditional_message);
        LiseHFBeamPositionReturns^ ret = gcnew LiseHFBeamPositionReturns(bSuccess, dxGauss, dyGauss, libmessage);

        ret->dRadius = dRadius;
        ret->dAngleOfEllipse = dAngleOfEllipse;
        ret->dRatioOfAxisOfEllipse = dRatioOfAxisOfEllipse;
        ret->dBackground = dBackground;
        ret->dNorm = dNorm;
        ret->dWeightedNorm = dWeightedNorm;
        ret->dAmpl = dAmpl;

        return ret;

    }
}