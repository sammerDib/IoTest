
/*
 * $Id: FogaleProbeCommonInterface.h 8984 2009-05-07 15:23:41Z S-PETITGRAND $
 */

typedef int (*FPROBE_INIT)(void* FProbeState, DBL_LISE_HCONFIG* HardwareConfigDual, LISE_HCONFIG* HardwareConfigTop, LISE_HCONFIG* HardwareConfigBottom, char* Name, LOGFILE* Log, double Param1, double Param2, double Param3);
typedef int (*FPROBE_LAMP_STATE)(void* FProbeState);

typedef int (*FPROBE_CLOSE)(void* FProbeState);

// ## measurement ##
typedef int (*FPROBE_START)(void* FProbeState);
typedef int (*FPROBE_STOP)(void* FProbeState);
typedef int (*FPROBE_DEFINE_SAMPLE)(void* FProbeState, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double Gain, double QualityThreshold);
typedef int (*FPROBE_GET_THICKNESS)(void* FProbeState, double* ThicknessArray, double* QualityValue, int iNbThickness);
typedef int (*FPROBE_GET_THICKNESSES)(void* FProbeState, double*Dates, double* ThicknessArray, double* QualityValue,int NumValues);

typedef int (*FPROBE_START_SINGLESHOT)(void* FProbeState);
typedef int (*FPROBE_START_CONTINUOUS)(void* FProbeState);
// ## measurement ##

// ## parameters ##
typedef int (*FPROBE_OPEN_PARAMS_WINDOW)(void* FProbeState,int ShowGainSetting);
typedef int (*FPROBE_UPDATE_PARAMS_WINDOW)(void* FProbeState);
typedef int (*FPROBE_CLOSE_PARAMS_WINDOW)(void* FProbeState);
typedef int (*FPROBE_GETPARAM)(void* FProbeState, void* Param, int ParamID);
typedef int (*FPROBE_SETPARAM)(void* FProbeState, void* Param, int ParamID);
// ## parameters ##

//pour simplifier l'interface interne à l'avenir remplacer l'usage des fonctions spécialisées suivantes par des combinaisons de SetParam/GetParams
typedef int (*FPROBE_GET_SYSTEM_CAPS)(void* FProbeState,char* Type, char*  SerialNumber, double& Range, int& Frequency, double& GainMin, double& GainMax, double& GainStep);
typedef int (*FPROBE_SET_STAGE_POSITION_INFO)(void* FProbeState, double* XSystemPos, double* YSystemPos, double* ZSystemPos);
typedef int (*FPROBE_GET_RAW_SIGNAL)(char* Password, void* FProbeState, double* I, int* NbSamples, float* StepX, int Voie, float* SaturationValue, double* SelectedPeaks, int* pnbSelectedPeaks, double* DiscardedPeaks, int* pnbDiscardedPeaks); //double* MissingPeaks, int* pnbMissingPeaks
typedef int (*FPROBE_CALIBRATE_DARK)(void* FProbeState);
typedef int (*FPROBE_CALIBRATE_THICKNESS)(void* FProbeState, float CalibrateValue);

// ## double probe functions ##
typedef int (*FPROBE_DEFINE_SAMPLE_DOUBLE)(void* FProbeState, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double* Gain, double* QualityThreshold);
typedef int (*FPROBE_GET_SYSTEM_CAPS_DOUBLE)(void* FProbeState,char* Type, char*  SerialNumber, double* Range, int* Frequency, double* GainMin, double* GainMax, double* GainStep);
// ## double probe functions ##

