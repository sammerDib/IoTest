/*
 * $Id: FogaleProbeDouble.cpp 9539 2009-07-03 07:10:24Z m-abet $
 */

#include <windows.h>
#include <stdio.h>
#include <string.h>
#include "crtdbg.h"

//FDE
#define FOGALEPROBE_EXPORTS

// ## public headers ##
#include "FogaleProbeReturnValues.h"
#include "FogaleProbeParamID.h"
#include "../LiseHardwareConfiguration.h"
#include "FogaleProbe.h"
// ## public headers ##

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
// ## probe-common headers ##

// ## probe-specific headers ##
//FDE
//#include "../FP_CHROM/Comm.h"
//#include "../FP_CHROM/Chrom.h"
//#include "../FP_CHROM/CHR_Module.h"
//#include "../FP_CHROM/CHR_Double_Module.h"
//// STIL
//#include "../FP_STIL/STIL_Module.h"
//#include "../FP_SPIRO/SPIRO_Module.h"

#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

// #include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"

#include "../FD_LISEED/LISE_ED_DLL_UI_Struct.h"
#include "../FD_LISEED/LISE_ED_DLL_Internal.h"
#include "../FD_DBLLISEED/DBL_LISE_ED_DLL_Internal.h"

#include "../FD_DBLLISEED/DBL_LISE_ED_DLL_Internal.h"

//FDE #include "../FP_LISE_ED_EXTENDED/LISE_ED_EXT_Internal.h"

// #### LENSCAN ####
// // #include "..\FD_LenScan\LenScanUI.h"
//FDE
//#include "..\FD_LenScanMotorAxis\MotorAxis.h"
//#include "..\FD_LenScanMotorAxis\MotorAxisWindow.h"
// #include "..\FD_LenScan\LenScanHardware.h"
// #include "..\FD_LenScan\LenScanDemodulation.h"
// #include "..\FD_LenScan\LenScanSignal.h"
// #include "..\FD_LenScan\LMatch.h"
// #include "..\FD_LenScan\GBPMConstants.h" 
// #include "..\FD_LenScan\MaterialDatabase.h"
// #include "..\FD_LenScan\LenScanMeas.h"
// #include "..\FD_LenScan\LenScanProbeStruct.h"
// #include "..\FD_LenScan\LenScanProbe.h"
// #### LENSCAN ####

// ## internal headers ##
#include "FogaleProbeCommonInterface.h"
#include "FogaleProbeInternal.h"
// ## internal headers ##

extern FPDLL_STATE s; //déclare la variable implémentée dans FogaleProbe.cpp

// Define sample - all probe will need a sample description to ensure correct measurement and confidence level calculation - the main application must setup a dialog box to let the user define the thickness, refraction index, tolerance and type for a variable number of layers (typically 1 to 3 - no need to prepare more than 5 in the main application)
FPDLLEXP int FP_API FPDefineSampleDouble(int ProbeID, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double* Gain, double QualityThreshold)
{
	FP_ENTERnolock(fpAnyState);
	double QualityDouble[2];
	QualityDouble[0] = QualityThreshold;
	QualityDouble[1] = QualityThreshold;
	// define sample double de la probe
	int r = s.Probe[ProbeID].FPDefineSampleDouble(&s.Probe[ProbeID].FProbeState, Name, SampleInfo, ThicknessArray, ToleranceArray, IndexArray, TypeArray, NbThickness, Gain, QualityDouble);

	FP_RETURNnolock(r);
}

// Define sample - all probe will need a sample description to ensure correct measurement and confidence level calculation - the main application must setup a dialog box to let the user define the thickness, refraction index, tolerance and type for a variable number of layers (typically 1 to 3 - no need to prepare more than 5 in the main application)
FPDLLEXP int FP_API FPDefineSampleDoubleEx(int ProbeID, char* Name, char* SampleInfo, double* ThicknessArray, double* ToleranceArray, double* IndexArray, double* TypeArray, int NbThickness, double* Gain, double* QualityThreshold)
{
	FP_ENTERnolock(fpAnyState);

	// define sample double de la probe
	int r = s.Probe[ProbeID].FPDefineSampleDouble(&s.Probe[ProbeID].FProbeState, Name, SampleInfo, ThicknessArray, ToleranceArray, IndexArray, TypeArray, NbThickness, Gain, QualityThreshold);

	FP_RETURNnolock(r);
}

FPDLLEXP int FP_API FPGetSystemCapsDouble(int ProbeID,char* Type, char*  SerialNumber, double* Range, int* Frequency, double* GainMin, double* GainMax, double* GainStep)
{
	FP_ENTER(fpAnyState);

	if(s.Probe[ProbeID].FPGetSystemCapsDouble==0) FP_RETURN(FP_UNAVAILABLE);

	int r = s.Probe[ProbeID].FPGetSystemCapsDouble(&s.Probe[ProbeID].FProbeState,Type,SerialNumber,Range,Frequency,GainMin,GainMax,GainStep);

	FP_RETURN(r);
}

