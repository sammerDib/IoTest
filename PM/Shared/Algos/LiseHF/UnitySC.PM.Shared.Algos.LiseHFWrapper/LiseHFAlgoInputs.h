#pragma once
#include "LiseHFRawSignal.h"
#include "LiseHFLayers.h"

using namespace System;
using namespace System::Collections::Generic;

namespace UnitySCPMSharedAlgosLiseHFWrapper {
    public enum class LiseHFMode : int { FFTOnly, SignalPeakSearch, GridSearch };

    public ref class LiseHFAlgoInputs
    {
    public:
        LiseHFAlgoInputs();

        ~LiseHFAlgoInputs();
        !LiseHFAlgoInputs();

        bool CheckValidity(String^% errorMessage);
        bool ComputeNatives();

    public:
        // Signals Spectrum (Measure + Calibration (Dark+Ref))
        List<double>^ Wavelength_nm;
        LiseHFRawSignal^ Spectrum;
        LiseHFRawSignal^ DarkSpectrum;
        LiseHFRawSignal^ RefSpectrum;

        // Opticlal Layers
        LiseHFLayers^ DepthLayers;

        //
        double TSVDiameter_um;
        LiseHFMode OpMode = LiseHFMode::GridSearch;

        double Threshold_signal_pct;        //[0.0 .. 1.0], Results are invalid when the highest peak of the FT to be analyzed is smaller than Threshold_signal_pct * peak height at z = 0
        double Threshold_peak_pct = 0.00;   //[0.0 .. 1.0], a local maximum of the FT is considered as meaningful peak, when it has at least the amplitude Threshold_peak_pct * amplitude of highest peak in considered window (z-region)
       
        bool PeakDetectionOnRight = false;
        bool NewPeakDetection = false;

    protected:
        const int SpectrometerPixels = 4094; // to check with Signal and wavelength
    };


    public ref class LiseHFBeamPositionInputs
    {
    public:
        LiseHFBeamPositionInputs(array<System::Byte>^ imgbuffer, int imgwidth, int imgheight, double pixelSizeX_um, double pixelSizeY_um);

        ~LiseHFBeamPositionInputs();
        !LiseHFBeamPositionInputs();

        bool CheckValidity(String^% errorMessage);
        int* GetNativeImgPtr() { return _pNativeImageBuffer; };

    public:
        int Width;
        int Height;
        double PixelSizeX;
        double PixelSizeY;

    private:
        int* _pNativeImageBuffer = nullptr;
    };
}
