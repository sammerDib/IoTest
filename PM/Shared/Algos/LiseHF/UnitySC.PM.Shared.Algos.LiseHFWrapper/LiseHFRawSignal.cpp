#include "LiseHFRawSignal.h"
#include "LiseHFMacros.h"

#pragma unmanaged
#include <memory.h>

#pragma managed
using namespace System;
using namespace System::Runtime::InteropServices;

namespace UnitySCPMSharedAlgosLiseHFWrapper
{
#pragma managed(push, off)
    void NativeMemCopy(double* pSrc, double* pDest, unsigned int count)
    {
        auto arrSize = count * sizeof(double);
        memcpy_s(pDest, arrSize, pSrc, arrSize);
    }
#pragma managed(pop)

    LiseHFRawSignal::LiseHFRawSignal()
    {
        RawSignal = nullptr;
        IntegrationTime_ms = 0.0;
        Attenuation_ID = 0;

        _pNativeRawSignal = nullptr;
        ComputeNative();
    }

    LiseHFRawSignal::LiseHFRawSignal(List<double>^ rawsignal, double integrationtime_ms, int attenuationId)
    {
        RawSignal = rawsignal;
        IntegrationTime_ms = integrationtime_ms;
        Attenuation_ID = attenuationId;

        _pNativeRawSignal = nullptr;
        ComputeNative();    
    }

    LiseHFRawSignal::~LiseHFRawSignal()
    {
        this->!LiseHFRawSignal();
    }

    LiseHFRawSignal::!LiseHFRawSignal()
    {
        SAFE_DELETE_ARRAY(_pNativeRawSignal);
    }

    void LiseHFRawSignal::ComputeNative()
    {
        if (RawSignal == nullptr)
        {
            _signalLength = 0;
        }
        else
        {
            SAFE_DELETE_ARRAY(_pNativeRawSignal);

            _signalLength = RawSignal->Count;
            if (_signalLength != 0)
            {
                pin_ptr<double> pinPtr = &(RawSignal->ToArray())[0];
               // double* pPin = pinPtr;
                _pNativeRawSignal = new double[_signalLength];
                NativeMemCopy(pinPtr, _pNativeRawSignal, _signalLength);
            }
        }
    }

    inline double* LiseHFRawSignal::GetNativeRawSignal()
    {
        return _pNativeRawSignal;
    }

    double LiseHFRawSignal::CalcSaturationPct()
    {
        const double satthreshold = 65535.0;
        double sat = 0.0;
        if (_pNativeRawSignal != nullptr)
        {
            double* ptr = _pNativeRawSignal;
            for (unsigned int i = 0; i < _signalLength - 1; i++)
            {
                if (*ptr > sat) {
                    sat = *ptr;
                }
                ptr++;
            }
            if (*ptr > sat) {
                sat = *ptr;
            }
        }
        return sat / satthreshold;
    }

}
