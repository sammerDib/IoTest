/*
 * $Id: LISE_ED_DLL_External.cpp 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */
#ifdef FDE
#include <winsock2.h>
#include <windows.h>

#include "..\SrcC\SPG.h"
#include "LISE_ED_DLL_External.h"
#include "LISE_ED_DLL_Internal.h"
#include "LISE_ED_DLL_General.h"
#include "LISE_ED_DLL_Create.h"
#include "LISE_ED_DLL_Acquisition.h"
#include "LISE_ED_DLL_Log.h"
#include "LISE_ED_DLL_Reglages.h"
#include "LISE_ED_DLL_Process.h"
#include "..\FP_LIDE_General\PeakMatch.h"

#include <time.h>

DLL_STATIC_STATE s;

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
    switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
			//DllInit();
			break;
		case DLL_PROCESS_DETACH:
			//DllClose();
			break;
			/*
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
			*/
    }
    return TRUE;
}


DLLEXP int LISE_ED_DLL_API LiseEDGetVersion()
{
	return LEDIGetVersion();
}

DLLEXP int LISE_ED_DLL_API LiseEDGetSystemCaps(char* Type,char* SerialNumber,double* Range,int* Frequency,double* GainMin, double* GainMax, double* GainStep)
{
	return LEDIGetSystemCaps(&s.LISE,Type,SerialNumber,Range,Frequency,GainMin,GainMax,GainStep);
}

DLLEXP int LISE_ED_DLL_API LiseEDInit(char* configPath)
{
	int Param1,Param2,Param3;
	return LEDIInit(&s.LISE,configPath,NULL,Param1,Param2,Param3);
}

DLLEXP int LISE_ED_DLL_API LiseEDAcqStop()
{
	return LEDIAcqStop(&s.LISE);
}

DLLEXP int LISE_ED_DLL_API LiseEDAcqStart()
{
	return LEDIAcqStart(&s.LISE);
}

DLLEXP int LISE_ED_DLL_API LiseEDClose()
{
	return LEDIClose(&s.LISE);
}

// Fonction permettant de definir la puissance d'une des deux sources à n'importe quel moment entre Create et Destroy
DLLEXP int LISE_ED_DLL_API LiseEDSetSourcePower(double* Volts, int* Voie)
{
	return LEDISetSourcePower(&s.LISE,*Volts);
}

//######################################## Results #######################################
// Get the number of samples of the last waveform period
DLLEXP int LISE_ED_DLL_API LiseEDGetNbSamplesWaveform(int* NbSamples)
{
	return LEDIGetNbSamplesWaveform(&s.LISE,NbSamples);
}

// Get the last waveform period
DLLEXP int LISE_ED_DLL_API LiseEDGetWaveform(double* I,int* NbSamples,float* StepX,int Voie)
{
	return LEDIGetWaveform(&s.LISE,I,NbSamples,StepX,Voie,NULL,NULL,NULL,NULL);
}

// Get the number of peaks of the last period
DLLEXP int LISE_ED_DLL_API LiseEDGetNbPeaksPeriod(int* NbPeaks,int* Voie)
{
	return LEDIGetNbPeaksPeriod(&s.LISE,NbPeaks,Voie);
}

// Get the peaks of the last period
DLLEXP int LISE_ED_DLL_API LiseEDGetPeaksPeriod(double* XPosRel, double* Intensite, double* Quality, int* Sens, int* Flag, int* NbPeaks,int* Voie)
{
	return LEDIGetPeaksPeriod(&s.LISE,XPosRel,Intensite,Quality,Sens,Flag,NbPeaks,Voie);
}

// Read temperature
DLLEXP int LISE_ED_DLL_API LiseEDReadTemperature(double* Temperature)
{
	return LEDIReadTemperature(&s.LISE,Temperature);
}

// Read optical power received
DLLEXP int LISE_ED_DLL_API LiseEDReadPower(double* Power)
{
	return LEDIReadPower(&s.LISE,Power);
}

// Get thickness
DLLEXP int LISE_ED_DLL_API LiseEDGetThickness(double* ThicknessArray, double& QualityValue, int _iNbThickness)
{
	return LEDIGetThickness(&s.LISE,Thickness,Quality, _iNbThickness);
}

// Define sample
DLLEXP int LISE_ED_DLL_API LiseEDDefineSample(char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int *NbThickness)
{
	double Gain = -1.0;
	double QualityThreshold = 0.0;
	return LEDIDefineSample(&s.LISE,Name,SampleNumber,Thickness,Tolerance,Index,Intensity,*NbThickness,Gain,QualityThreshold);
}

DLLEXP int LISE_ED_DLL_API LiseEDDisplaySettingWindow(int ShowGainSetting)
{
	LEDIOpenSettingsWindow(&s.LISE,ShowGainSetting);
	return DLL_OK;
}

DLLEXP int LISE_ED_DLL_API LiseEDDestroySettingWindow()
{
	LEDICloseSettingsWindow(&s.LISE);
	return DLL_OK;
}

DLLEXP int LISE_ED_DLL_API LiseEDSetStagePositionInfo(double* XSystemPosition, double* YSystemPosition, double *ZSystemPosition)
{
	return LEDISetStagePositionInfo(&s.LISE,XSystemPosition,YSystemPosition,ZSystemPosition);
}

//####################################### Sauvegardes ######################################

// Sauve les resultats de waveform en fichier texte
DLLEXP int LISE_ED_DLL_API LiseEDSaveWaveForm(char* FileName,float* StepX)
{
	return LEDISaveWaveForm(&s.LISE,FileName,StepX);
}

// Fonction permettant de copier tout le siganl dans un fichier texte depuis le début de l'acquisition
DLLEXP int LISE_ED_DLL_API LiseEDSavePeaks(char* FileName)
{
	return LEDISavePeaks(&s.LISE,FileName);
}

// fonction permettant la sauvegarde des epaisseurs dans les deux sens
DLLEXP int LISE_ED_DLL_API LiseEDAcqSaveThickness(char* FileName)
{
	return LEDIAcqSaveThickness(&s.LISE,FileName);
}
#endif