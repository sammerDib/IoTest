#include "LiseHFAlgoInputs.h"
#include "LiseHFMacros.h"

#pragma managed
using namespace System;

namespace UnitySCPMSharedAlgosLiseHFWrapper
{
    LiseHFAlgoInputs::LiseHFAlgoInputs()
    {
    }

    // destructeur
    LiseHFAlgoInputs::~LiseHFAlgoInputs()
    {
        this->!LiseHFAlgoInputs();
    }

    //Finalizer
    LiseHFAlgoInputs::!LiseHFAlgoInputs()
    {
    }

    bool LiseHFAlgoInputs::CheckValidity(String^% errorMessage)
    {
        bool isValid = true;

        if (Wavelength_nm == nullptr || Wavelength_nm->Count == 0)
        {
            errorMessage += "Wavelegnth is Null or Empty\n";   isValid = false;
        }
        else
        {
            if (Wavelength_nm->Count < SpectrometerPixels)
            {
                errorMessage += "Raw Signal is Incomplete - missing some spectrometer pixels\n";   isValid = false;
            }
        }
        if (Spectrum == nullptr || Spectrum->GetRawSignalLength() == 0)
        {
            errorMessage += "Spectrum is Null or Empty\n";   isValid = false;
        }
        if (DarkSpectrum == nullptr || DarkSpectrum->GetRawSignalLength() == 0)
        {
            errorMessage += "DarkSpectrum is Null or Empty\n";   isValid = false;
        }
        if (RefSpectrum == nullptr || RefSpectrum->GetRawSignalLength() == 0)
        {
            errorMessage += "RefSpectrum is Null or Empty\n";   isValid = false;
        }

        if (OpMode != LiseHFMode::FFTOnly)
        {
            if (DepthLayers == nullptr)
            {
                errorMessage += "DepthLayers is Null\n";   isValid = false;
            }
            else if (DepthLayers->HasNativeBeenComputed())
            {
                auto nbLayer = DepthLayers->GetLayerCount();
                if (nbLayer == 0)
                {
                    errorMessage += "DepthLayers is Empty\n";   isValid = false;
                }
            }
            if (TSVDiameter_um < 1.9)
            {
                errorMessage += "TSV diameter too small : min = 1.9um \n";   isValid = false;
            }
        }
        else
        {
            if (DepthLayers == nullptr)
            {
                errorMessage += "DepthLayers is Null\n";   isValid = false;
            }
           if(TSVDiameter_um == 0)
               TSVDiameter_um = 3.0; // en attendant meiux
        }

        if (Threshold_signal_pct < 0.0 || Threshold_signal_pct > 1.0)
        {
            errorMessage += "Bad Threshold_signal : outside range [0..1] \n";   isValid = false;
        }
        if (Threshold_peak_pct < 0.0 || Threshold_peak_pct > 1.0)
        {
            errorMessage += "Bad Threshold_signal : outside range [0..1] \n";   isValid = false;
        }

        // do we check if integration time and attenuation is coherent here ? TODO if needed

        return isValid;
    }

    bool LiseHFAlgoInputs::ComputeNatives()
    {
        if (!Spectrum->HasNativeBeenComputed())
            Spectrum->ComputeNative();
        if (!DarkSpectrum->HasNativeBeenComputed())
            DarkSpectrum->ComputeNative();
        if (!RefSpectrum->HasNativeBeenComputed())
            RefSpectrum->ComputeNative();
        if (!DepthLayers->HasNativeBeenComputed())
            DepthLayers->ComputeNative();

        return false;
    }

    //
    // LiseHFBeamPositionInputs
    //

    LiseHFBeamPositionInputs::LiseHFBeamPositionInputs(array<System::Byte>^ imgbuffer, int imgwidth, int imgheight, double pixelSizeX_um, double pixelSizeY_um)
    {
        Width = imgwidth,
        Height = imgheight;
        PixelSizeX = pixelSizeX_um;
        PixelSizeY = pixelSizeY_um;

        _pNativeImageBuffer = new int[Width * Height];
        for (int i = 0; i < Width * Height; i++)
        {
            _pNativeImageBuffer[i] = (int)imgbuffer[i];
        }
    }
    LiseHFBeamPositionInputs::~LiseHFBeamPositionInputs()
    {
        this->!LiseHFBeamPositionInputs();
    }

    LiseHFBeamPositionInputs::!LiseHFBeamPositionInputs()
    {
        if (_pNativeImageBuffer != nullptr)
        {
            delete[] _pNativeImageBuffer; 
            _pNativeImageBuffer = nullptr;
        };
    }

    bool LiseHFBeamPositionInputs::CheckValidity(String^% errorMessage)
    {
        bool isValid = true;

        if (Width == 0 || Height == 0)
        {
            errorMessage += "Empty Image W=0  H=0 \n";   isValid = false;
        }
        else  if(_pNativeImageBuffer == nullptr)
        {
            errorMessage += "Could not allocate image buffer\n";   isValid = false;
        }
        
        if (PixelSizeX <= 0.0)
        {
            errorMessage += "Bad PixelSize X\n";   isValid = false;
        }

        if (PixelSizeY <= 0.0)
        {
            errorMessage += "Bad PixelSize Y\n";   isValid = false;
        }
        return isValid;
    }
}