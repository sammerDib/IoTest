/*
 * $Id: FogaleProbe.h 9539 2009-07-03 07:10:24Z m-abet $
 */

//////////////////////////////////////////////////////////////////////
//																	//
//							Fogale Nanotech							//
//					       FogaleProbe header						//
//																	//
//////////////////////////////////////////////////////////////////////


#ifdef INC_FOGALEPROBE_INC
#error FogaleProbe.h Included twice
#endif
#define	INC_FOGALEPROBE_INC
// Header version
#define FOGALEPROBE_VERSION 340

// DLL_EXPORTS is defined only in the project which compiles the dll.
// In this case, the functions are declared dllexports (add in C/C++ preprocessor definitions).

#ifdef FOGALEPROBE_EXPORTS
#define FPDLLEXP __declspec(dllexport)
#else
#define FPDLLEXP __declspec(dllimport)
#endif

#define FP_API __stdcall

#ifdef __cplusplus
//ragma message (__FILE__"(33): using extern C")
extern "C"
{
#endif

// Fogale probe fonctions
	
#ifdef _NO_FOGALE_PROBE

//#define FPDLLInit() true
//#define FP_API FPInitialize(Name, Param1, Param2, Param3) 0

int FPGetVersion(){ return 0;}
int FPDLLInit(){ return 0;}
int FPDLLClose(){ return 0;}
int FPInitialize(char* Name, int Param1, int Param2, int Param3){ return 0;}
int FPLampState(int ProbeID, int *itSate){ return 0;}
int FPClose(int ProbeID){ return 0;}
int FPDoSettings(int ProbeID){ return 0;}
int FPOpenSettingsWindow(int ProbeID){ return 0;}
int FPUpdateSettingsWindow(int ProbeID){ return 0;}
int FPCloseSettingsWindow(int ProbeID){ return 0;}
int FPDefineSample(int ProbeID, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double Gain, double QualityThreshold){ return 0;}
int FPStartSingleShotAcq(int ProbeID){ return 0;}
int FPStopSingleShotAcq(int ProbeID){ return 0;}
int FPStartContinuousAcq(int ProbeID){ return 0;}
int FPStopContinuousAcq(int ProbeID){ return 0;}
int FPGetThickness(int ProbeID, double* ThicknessArray, double* QualityValue,int _iNumThickness){ return 0;}
int FPGetThicknesses(int ProbeID, double* Dates, double* Thicknesses, double* Quality,int NumValues){ return 0;}
int FPGetParam(int ProbeID, void* Param, int ParamID){ return 0;}
int FPSetParam(int ProbeID, void* Param, int ParamID){ return 0;}
int FPGetSystemCaps(int ProbeID, char*  Type, char*  SerialNumber,double* Range,int* Frequency,double* GainMin, double* GainMax, double* GainStep){ return 0;}
int FPGetRawSignal(int ProbeID, char* Password, double* I, int* NbSamples, float* StepX, int Voie, float* SaturationValue, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks){ return 0;}
int FPSetStagePositionInfo(int ProbeID, double* XStagePosition, double* YStagePosition, double* ZStagePosition){ return 0;}
int FPCalibrateDark(int ProbeID){ return 0;}
int FPCalibrateThickness(int ProbeID,double Value){ return 0;}
int FPDefineSampleDouble(int ProbeID, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double* Gain, double* QualityThreshold){ return 0;}
int FPGetSystemCapsDouble(int ProbeID,char* Type, char*  SerialNumber, double* Range, int* Frequency, double* GainMin, double* GainMax, double* GainStep){ return 0;}

#else

// Returns FOGALEPROBE_VERSION (to check correspondance between header and DLL)
FPDLLEXP int FP_API FPGetVersion();

// DLL initialisation - call this function on main application startup
FPDLLEXP int FP_API FPDLLInit();

// Close DLL - call this on main application exit
FPDLLEXP int FP_API FPDLLClose();

// Probe initialisation, returns the probe ID - several probes can be initialized (will return different ID) - Probe ID is used in subsequent function call to identify the probe
FPDLLEXP int FP_API FPInitialize(DBL_LISE_HCONFIG* HardwareConfig, LISE_HCONFIG* HardwareConfigTop, LISE_HCONFIG* HardwareConfigBottom, char* Name, int Param1, int Param2, int Param3);

FPDLLEXP int FP_API FPLampState(int ProbeID, int *itSate);

#define FP_USERSELECTEDPROBE NULL//((char*)(-1)) //can be specified in place of name to open a file selection window

// Close probe, ProbeID can not be used anymore
FPDLLEXP int FP_API FPClose(int ProbeID);

// Open setting window - blocking version - this ensure we'll be able to setup the measurement parameters - the main application must let the user call this function (with a button ...) to set the probe parameters - the main application must ensure the probe must be on the wafer in measuring position 
FPDLLEXP int FP_API FPDoSettings(int ProbeID);

// Open setting window - non blocking version - this ensure we'll be able to setup the measurement parameters - the main application must let the user call this function (with a button ...) to set the probe parameters - the main application must ensure the probe must be on the wafer in measuring position 
FPDLLEXP int FP_API FPOpenSettingsWindow(int ProbeID);

// Update setting window - this ensure we'll be able to setup the measurement parameters - the main application must let the user call this function (with a button ...) to set the probe parameters - the main application must ensure the probe must be on the wafer in measuring position 
FPDLLEXP int FP_API FPUpdateSettingsWindow(int ProbeID);

// Close setting window - this ensure we'll be able to setup the measurement parameters - the main application must let the user call this function (with a button ...) to set the probe parameters - the main application must ensure the probe must be on the wafer in measuring position 
FPDLLEXP int FP_API FPCloseSettingsWindow(int ProbeID);

// Define sample - all probe will need a sample description to ensure correct measurement and confidence level calculation - the main application must setup a dialog box to let the user define the thickness, refraction index, tolerance and type for a variable number of layers (typically 1 to 3 - no need to prepare more than 5 in the main application)
FPDLLEXP int FP_API FPDefineSample(int ProbeID, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double Gain, double QualityThreshold);

// Begin single shot mode
FPDLLEXP int FP_API FPStartSingleShotAcq(int ProbeID);

// Stop single shot mode
FPDLLEXP int FP_API FPStopSingleShotAcq(int ProbeID);

// Begin continuous mode - future use
FPDLLEXP int FP_API FPStartContinuousAcq(int ProbeID);

// Stop continuous mode - future use
FPDLLEXP int FP_API FPStopContinuousAcq(int ProbeID);

// Get thickness - the main measurement function - measurement starts when the fonctions starts and return after the measurement is done - the ThicknessArray size must match the value defined in FPDefineSample - please ignore the measurement when Quality < user defined quality threshold
FPDLLEXP int FP_API FPGetThickness(int ProbeID, double* ThicknessArray, double* QualityValue,int _iNumThickness);

// Get thicknesses
FPDLLEXP int FP_API FPGetThicknesses(int ProbeID, double* Dates, double* Thicknesses, double* Quality,int NumValues);

//General purpose access to probe parameters - see table in FogaleProbeParamID.h
FPDLLEXP int FP_API FPGetParam(int ProbeID, void* Param, int ParamID);

//General purpose access to probe parameters - see table in FogaleProbeParamID.h
FPDLLEXP int FP_API FPSetParam(int ProbeID, void* Param, int ParamID);

//Helper functions

// Returns Information about system (FPID_C_TYPE + FPID_C_SERIAL + FPID_D_RANGE + ...)
FPDLLEXP int FP_API FPGetSystemCaps(int ProbeID, char*  Type, char*  SerialNumber,double* Range,int* Frequency,double* GainMin, double* GainMax, double* GainStep);

// Get Signal Brut
FPDLLEXP int FP_API FPGetRawSignal(int ProbeID, char* Password, double* I, int* NbSamples, float* StepX, int Voie, float* SaturationValue, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks);

// Set Stage Position Info
FPDLLEXP int FP_API FPSetStagePositionInfo(int ProbeID, double* XStagePosition, double* YStagePosition, double* ZStagePosition);

// Dark Calibration
FPDLLEXP int FP_API FPCalibrateDark(int ProbeID);

//Calibrate Thickness
FPDLLEXP int FP_API FPCalibrateThickness(int ProbeID,double Value);

// double fonctions

// Define sample - all probe will need a sample description to ensure correct measurement and confidence level calculation - the main application must setup a dialog box to let the user define the thickness, refraction index, tolerance and type for a variable number of layers (typically 1 to 3 - no need to prepare more than 5 in the main application)
FPDLLEXP int FP_API FPDefineSampleDoubleEx(int ProbeID, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double* Gain, double* QualityThreshold);
// Define sample - all probe will need a sample description to ensure correct measurement and confidence level calculation - the main application must setup a dialog box to let the user define the thickness, refraction index, tolerance and type for a variable number of layers (typically 1 to 3 - no need to prepare more than 5 in the main application)
FPDLLEXP int FP_API FPDefineSampleDouble(int ProbeID, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double* Gain, double QualityThreshold);

// get system caps pour une probe double
FPDLLEXP int FP_API FPGetSystemCapsDouble(int ProbeID,char* Type, char*  SerialNumber, double* Range, int* Frequency, double* GainMin, double* GainMax, double* GainStep);

#endif

#ifdef __cplusplus
}
#endif
