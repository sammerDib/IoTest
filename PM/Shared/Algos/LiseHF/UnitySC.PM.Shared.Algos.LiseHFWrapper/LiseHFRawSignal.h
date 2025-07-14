#pragma once
using namespace System;
using namespace System::Collections::Generic;

namespace UnitySCPMSharedAlgosLiseHFWrapper {

    public ref class LiseHFRawSignal
    {
    public:
        LiseHFRawSignal();
        LiseHFRawSignal(List<double>^ rawsignal, double integrationtime_ms, int attenuationId);

        ~LiseHFRawSignal();
        !LiseHFRawSignal();


        void ComputeNative();
        bool HasNativeBeenComputed() { return (_pNativeRawSignal != nullptr) ? (RawSignal->Count == _signalLength) : false; };

        double* GetNativeRawSignal();
        unsigned int GetRawSignalLength() { return HasNativeBeenComputed() ? _signalLength : (RawSignal ? RawSignal->Count : 0u ); };

        double CalcSaturationPct();

    public:
        List<double>^ RawSignal;
        double IntegrationTime_ms;
        int Attenuation_ID;

    private :
        double* _pNativeRawSignal = nullptr;
        unsigned int  _signalLength = 0;



    };
}

