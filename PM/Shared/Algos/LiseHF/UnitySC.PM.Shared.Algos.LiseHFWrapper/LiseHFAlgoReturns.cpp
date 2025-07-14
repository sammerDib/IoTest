#include "LiseHFAlgoReturns.h"
#include "LiseHFMacros.h"

#pragma managed
using namespace System;

namespace UnitySCPMSharedAlgosLiseHFWrapper 
{
    LiseHFAlgoReturns::LiseHFAlgoReturns(bool bSuccess, bool bFFTDone, bool bAnalysisDone, String^ errorMessage, LiseHFAlgoOutputs^ outputs)
    {
        IsSuccess = bSuccess;
        FFTDone = bFFTDone;
        AnalysisDone = bAnalysisDone;
        ErrorMessage = errorMessage;

        Outputs = outputs;
    }

    LiseHFAlgoReturns::~LiseHFAlgoReturns()
    {
        this->!LiseHFAlgoReturns();
    }

    LiseHFAlgoReturns::!LiseHFAlgoReturns()
    {
      // Nothing to do 
    }

    //
    // LiseHFSpotReturns
    //

    LiseHFBeamPositionReturns::LiseHFBeamPositionReturns(bool bSuccess, double xSpotPos_um, double ySpotPos_um, String^ errorMessage)
    {
        IsSuccess = bSuccess;
        xSpotPosition_um = xSpotPos_um;
        ySpotPosition_um = ySpotPos_um;

        ErrorMessage = errorMessage;
    }
    LiseHFBeamPositionReturns::~LiseHFBeamPositionReturns()
    {
        this->!LiseHFBeamPositionReturns();
    }
    LiseHFBeamPositionReturns::!LiseHFBeamPositionReturns()
    {
        // Nothing to do 
    }
}