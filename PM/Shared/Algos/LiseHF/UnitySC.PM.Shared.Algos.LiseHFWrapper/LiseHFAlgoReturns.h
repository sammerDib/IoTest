#pragma once
#include "LiseHFAlgoOutputs.h"

#pragma managed
using namespace System;

namespace UnitySCPMSharedAlgosLiseHFWrapper {

    public ref class LiseHFAlgoReturns
    {
    public :
        LiseHFAlgoReturns(bool bSuccess, bool bFFTDone, bool bAnalysisDone, String^ errorMessage, LiseHFAlgoOutputs^ outputs); 

        ~LiseHFAlgoReturns();
        !LiseHFAlgoReturns();

    public:
        bool IsSuccess ;
        bool FFTDone;
        bool AnalysisDone;
        String^ ErrorMessage;

        LiseHFAlgoOutputs^ Outputs;
    };

    public ref class LiseHFBeamPositionReturns
    {
    public:
        LiseHFBeamPositionReturns(bool bSuccess, double xSpotPos_um, double ySpotPos_um, String^ errorMessage);

        ~LiseHFBeamPositionReturns();
        !LiseHFBeamPositionReturns();

    public:
        bool IsSuccess;
        String^ ErrorMessage;

        double xSpotPosition_um; //dxGauss
        double ySpotPosition_um; //dyGauss

        // advance wolfgang values
        double dRadius;
        double dBackground;
        double dNorm;
        double dWeightedNorm;
        double dRatioOfAxisOfEllipse;
        double dAngleOfEllipse;
        double dAmpl;
    };
}

