#pragma once
using namespace System;
using namespace System::Collections::Generic;

namespace UnitySCPMSharedAlgosLiseHFWrapper
{

    public ref class LiseHFAlgoOutputs
    {
    public :
        LiseHFAlgoOutputs();

        ~LiseHFAlgoOutputs();
        !LiseHFAlgoOutputs();

        double GetDetectedPeaksNumber() { return ((MeasuredDepths != nullptr) ? MeasuredDepths->Count : 0); }

    public:
        double SaturationPercentage;
        double ThresholdSignal;
        double ThresholdPeak;

        List<double>^ FTTx;
        List<double>^ FTTy;

        List<double>^ PeaksX;
        List<double>^ PeaksY;

        List<double>^ MeasuredDepths;

        double NormalizedResidual;
        double dAsymptStdErr; //asymptotic standard error (similar to a standard deviation) of the computed layer thicknesses
        double Quality; //sqrt( amplitude of smallest fitted peaks) / (dNormalizedResidual + 1.0e-12); this is a measure for the signal quality


    };
}

