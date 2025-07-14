#pragma once
#include "ImageData.h"
namespace UnitySCPMSharedAlgosVSIWrapper {

	using namespace System;
	using namespace System::Collections::Generic;

    public enum class StatusCode : int {
        OK = 0,
        NONSUPPORTED_WINDOWTYPE = -1,
        UNKNOWN_FREQUENCYMETHOD = -2,
        DF_SRCWAVELENGTH_ERROR = -3,
        UNKNOWNPIXELSET = -4,
        DP_SRCWAVELENGTH_ERROR = -5,
        NOT_ENOUGHIMAGES = -6,
        NOTVALIDFILENAME = -7,
        READFILEFAILED = -8,
        LP_ERROR = -9,
        LP_NOT_ENOUGH_PIXELS = -10,
        HP_NOT_ENOUGH_PIXELS = -12,
        OUTOFMEMORY = -13,
        FFT_FAILS = -14,
        FILE_ISSUE = -15,
        DATACONTAIN0 = -16,
        NOTYET_IMPLEMENTED = -75,
        GENERIC_ERROR = -100,
        UNKNOWN_ERROR = -101,
    };

    public ref struct VSIOutput {
        StatusCode Status;
        array<float>^ ResultArray;

        VSIOutput() : Status(StatusCode::UNKNOWN_ERROR), ResultArray(nullptr) {};
    };

	public ref class VSI {


	public:
		static VSIOutput^ ComputeTopography(array<array<System::Byte>^>^ images, int width, int height, double ruleStep, double lambdaCenter, double fwhmLambda, double noiseLevel, double maskThreshold);
	};
}
