
#include <windows.h>
#include <stdio.h>
#include <string.h>


// ## public headers ##
#include "FogaleProbeReturnValues.h"
#include "FogaleProbeParamID.h"
#define FOGALEPROBE_EXPORTS
#include "../LiseHardwareConfiguration.h"
#include "FogaleProbe.h"
// ## public headers ##

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "..\SrcC\BreakHook.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
// ## probe-common headers ##

// ## probe-specific headers ##
//#include "../FP_CHROM/Comm.h"
//#include "../FP_CHROM/Chrom.h"
//#include "../FP_CHROM/CHR_Module.h"
//#include "../FP_CHROM/CHR_Double_Module.h"
//
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

// #### LENSCAN ####
// // #include "..\FD_LenScan\LenScanUI.h"
//#include "..\FD_LenScanMotorAxis\MotorAxis.h"
//#include "..\FD_LenScanMotorAxis\MotorAxisWindow.h"
// #include "..\FD_LenScan\LenScanHardware.h"
// #include "..\FD_LenScan\LenScanDemodulation.h"
// #include "..\FD_LenScan\LenScanSignal.h"
// #include "..\FD_LenScan\LMatch.h"
//#include "..\FD_LenScanMotorAxis\MotorAxis.h"
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

#define FlagHelper_Log 0


extern FPDLL_STATE s; //déclare la variable implémentée dans FogaleProbe.cpp


// Get raw signal
FPDLLEXP int FP_API FPGetRawSignal(int ProbeID, char* Password, double* I, int* NbSamples, float* StepX, int Voie, float* SaturationValue, double* SelectedPeaks, int* pnbSelectedPeaks, double* DiscardedPeaks, int* pnbDiscardedPeaks) //double* MissingPeaks, int* pnbMissingPeaks
{
	//try
	{
	FP_ENTERnolock(fpAnyState);//AnyState autorise quand même le recompute pour lenscan

	CHECK(
		(s.Probe[ProbeID].Type != fpSPIRO) &&
		(s.Probe[ProbeID].Type != fpLenScan) &&
		(s.Probe[ProbeID].state!=fpStarted),
		"Probe Acquisition must be started before calling FPGetRawSignal",
		FP_RETURNnolock(FP_FAIL));

		int r=0;

		// on assigne le probe state au probe ID correspondant
		FPROBE_STATE* curProbe = &s.Probe[ProbeID];

		// log de réentrance passé
		if(FlagHelper_Log) LogfileF(s.Log,"Reentrance test sucess can continue execution of function GetRawSignal");

		if(NbSamples) CHECK((I==0)&&(*NbSamples)!=0,"",return FP_INVALIDPARAM);
		if(pnbSelectedPeaks) CHECK((SelectedPeaks==0)&&(*pnbSelectedPeaks!=0),"FPGetRawSignal invalid params combination",return FP_INVALIDPARAM);
		if(pnbDiscardedPeaks) CHECK((DiscardedPeaks==0)&&(*pnbDiscardedPeaks!=0),"FPGetRawSignal invalid params combination",return FP_INVALIDPARAM);

		r = s.Probe[ProbeID].FPGetRawSignal(Password,&s.Probe[ProbeID].FProbeState, I, NbSamples, StepX, Voie, SaturationValue, SelectedPeaks, pnbSelectedPeaks, DiscardedPeaks, pnbDiscardedPeaks); //MissingPeaks, &nbMissingPeaks
		
        if(FlagHelper_Log)
        {
		    // log des valeurs de retours
		    char MessageTemp[1024];
		    sprintf(MessageTemp,"Samples Number returned = %i, Channel = %i",*NbSamples,Voie);
		    Logfile(s.Log, MessageTemp);
        }

		FP_RETURNnolock(r);
//	}
//	catch(...)
//	{
//#ifdef DebugList
//		SPG_List("FPGetRawSignal: exception");
//#ifdef SPG_DEBUGCONFIG
//		BreakHook();
//#endif
//#endif
		FP_RETURNnolock(FP_FAIL);
	}
}


FPDLLEXP int FP_API FPGetSystemCaps(int ProbeID, char*  Type, char*  SerialNumber,double* Range,int* Frequency,double* GainMin, double* GainMax, double* GainStep)
{
	FP_ENTER(fpAnyState);

	if(s.Probe[ProbeID].FPGetSystemCaps==0) FP_RETURN(FP_UNAVAILABLE);

	int r = s.Probe[ProbeID].FPGetSystemCaps(&s.Probe[ProbeID].FProbeState,Type,SerialNumber,*Range,*Frequency,*GainMin,*GainMax,*GainStep);

	FP_RETURN(r);
}


FPDLLEXP int FP_API FPSetStagePositionInfo(int ProbeID, double* XStagePosition, double* YStagePosition, double* ZStagePosition)
{
	FP_ENTER(fpAnyState);

	if(s.Probe[ProbeID].FProbeSetStagePositionInfo==0) FP_RETURN(FP_UNAVAILABLE);
	int r = s.Probe[ProbeID].FProbeSetStagePositionInfo(&s.Probe[ProbeID].FProbeState,XStagePosition,YStagePosition,ZStagePosition);
	FP_RETURN(r);
}

// dark calibration
FPDLLEXP int FP_API FPCalibrateDark(int ProbeID)
{
	FP_ENTER(fpAnyState);

	if(s.Probe[ProbeID].FPCalibrateDark==0) FP_RETURN(FP_UNAVAILABLE);

	int r = s.Probe[ProbeID].FPCalibrateDark(&s.Probe[ProbeID].FProbeState);

	FP_RETURN(r);
}

FPDLLEXP int FP_API FPCalibrateThickness(int ProbeID,double Value)
{
	FP_ENTER(fpAnyState);

	if(s.Probe[ProbeID].FPCalibrateThickness==0) FP_RETURN(FP_UNAVAILABLE);

	int r = s.Probe[ProbeID].FPCalibrateThickness(&s.Probe[ProbeID].FProbeState,Value);

	FP_RETURN(r);
}

