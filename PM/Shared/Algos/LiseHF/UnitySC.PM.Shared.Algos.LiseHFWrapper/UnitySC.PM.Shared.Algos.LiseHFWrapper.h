#pragma once
#include "LiseHFAlgoInputs.h"
#include "LiseHFAlgoOutputs.h"
#include "LiseHFAlgoReturns.h"
#include "LiseHFLayers.h"
#include "LiseHFRawSignal.h"

using namespace System;

namespace UnitySCPMSharedAlgosLiseHFWrapper
{
    public ref class Olovia_Algos {

    public:
        static LiseHFAlgoReturns^ Compute(LiseHFAlgoInputs^ inputs);

        static LiseHFBeamPositionReturns^ ComputeBeamPosition(LiseHFBeamPositionInputs^ inputs);
    };
}
