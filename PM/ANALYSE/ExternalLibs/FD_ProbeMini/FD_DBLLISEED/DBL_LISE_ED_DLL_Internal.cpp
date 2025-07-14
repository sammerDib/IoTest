
/*
 * $Id: LISE_ED_DLL_Internal.CPP 8372 2009-02-26 10:18:16Z y-randle $
 */

#include <windows.h>
#include <stdio.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "..\SrcC\BreakHook.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

// #include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"
// ## probe-specific headers from simple LiseED ##

#include "../FD_LISEED/LISE_ED_DLL_UI_Struct.h"
#include "../FD_LISEED/LISE_ED_DLL_Internal.h"
#include "../FD_LISEED/LISE_ED_DLL_Acquisition.h"
#include "../FD_LISEED/LISE_ED_DLL_Config.h"
#include "../FD_LISEED/LISE_ED_DLL_Create.h"
#include "../FD_LISEED/LISE_ED_DLL_General.h"
#include "../FD_LISEED/LISE_ED_DLL_Process.h"
#include "../FD_LISEED/LISE_ED_DLL_Log.h"
#include "../FD_LISEED/LISE_ED_DLL_Reglages.h"
#include "../LiseHardwareConfiguration.h"

// ## probe-specific headers from double LiseED ##
#include "DBL_LISE_ED_DLL_Internal.h"
#include "DBL_LISE_ED_DLL_Log.h"
#include "DBL_LISE_ED_DLL_Config.h"
#include "DBL_LISE_ED_DLL_Settings.h"

#include "..\FD_FogaleProbe\FogaleProbeReturnValues.h"
#include "..\FD_FogaleProbe\FogaleProbeParamID.h"

#include <time.h>


#define CHECK_PROBE_DBL_ED(){if(DblLiseEd->iStatusProbe != PROBE_INIT){	if(Global.EnableList>=1) MessageBox(0,"Probe double lise ed non initialised.\r\nPlease close software and restart it.","Double Lise ed Init Failed",0);}}

// Returns LISE_ED_DLL_VERSION (to check correspondance between header and DLL)
int DBL_LEDIGetVersion()
{
	return DBL_LISE_ED_DLL_VERSION;
}
// set parameters internal to Lise Ed
int DBL_LEDISetParam(void* s,void* Param, int ParamID)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}
	
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	if(ParamID == FPID_I_SETCURRENTPROBE)
	{
		if(*(int*)Param < 0 || *(int*)Param > 1)
		{
			return FP_ERROR_PARAM_ID;
		}

		// si les deux probes sont definit, il faut simplement changer la probe visible
		if(DblLiseEd->CfgMeas == BothProbe)
		{
			return DBL_SetVisibleProbe(s,*(int*)Param);
		}
		else
		{
			return DBL_SetCurrentProbe(s,*(int*)Param,true);
		}
	}
	else if(ParamID == FPID_I_SETCURRENTPROBE_ONLY)
	{
		if(*(int*)Param < 0 || *(int*)Param > 1)
		{
			return FP_ERROR_PARAM_ID;
		}

		return DBL_SetCurrentProbe(s,*(int*)Param,true);
	}
	else if(ParamID == FPID_I_SETVISIBLEPROBE)
	{
		if(*(int*)Param < 0 || *(int*)Param > 1)
		{
			return FP_ERROR_PARAM_ID;
		}

		// si les deux probes sont definit, il faut simplement changer la probe visible
		return DBL_SetVisibleProbe(s,*(int*)Param);
	}
	else if(ParamID == FPID_D_CALIBRATE_TOTAL_TH)
	{
		//DblLiseEd->dTotalThickness = *(double*)Param;
		double* CalibrationArray = (double*)Param;
		int RetCalib = DBL_CalibrateDblLise(s,CalibrationArray);

		// on va marquer la valeur dans le fichier de config
		SetTotalThicknessInCfgFile(*DblLiseEd,DblLiseEd->dTotalThickness);

		return RetCalib;
	}
	else if(ParamID == FPID_D_FORCE_CALIBRATE_TOTAL_TH)
	{
		DblLiseEd->dTotalThickness = *(double*)Param;
	}
	else if(ParamID == FPID_I_CALIBRATION_MODE)
	{
		int* iParam= (int*)Param;
		DblLiseEd->iCalibrationMode = *iParam;
		
		return true;
	}
	else if(ParamID == FPID_D_XVALUE_TOTAL_TH)
	{
		double* XcalibrationValue = (double*)Param;
		DblLiseEd->XCalibrationValue = *XcalibrationValue;
	}
	else if(ParamID == FPID_D_YVALUE_TOTAL_TH)
	{
		double* YcalibrationValue = (double*)Param;
		DblLiseEd->YCalibrationValue = *YcalibrationValue;
	}
	else if(ParamID == FPID_D_ZVALUE_TOTAL_TH)
	{
		double* ZcalibrationValue = (double*)Param;
		SetZValueInCfgFile(*DblLiseEd, *ZcalibrationValue);
	}
	else if(ParamID == FPID_I_CLEAR_PEAK_MOYENNE)
	{
		ClearPicMoyenne(s);
	}
	else if(ParamID == FPID_B_AUTOAIRGAPCONFIG)
	{
		bool* bAAG = (bool*)Param;
		DblLiseEd->LiseEd[0].Lise.bUseAirGapAuto = *bAAG;
		DblLiseEd->LiseEd[1].Lise.bUseAirGapAuto = *bAAG;
	}
	else if(ParamID == FPID_B_AUTOGAIN)
	{
		bool* bAutoGain = (bool*)Param;
	
		return SetParamAutoGain_Double(s,*bAutoGain);
	}
	else if(ParamID == FPID_B_LIMITEDTIME_MODE)
	{
		bool* bLimited = (bool*)Param;
		DblLiseEd->LiseEd[0].Lise.bLimitedTime = *bLimited;
		DblLiseEd->LiseEd[1].Lise.bLimitedTime = *bLimited;
	}
	else if(ParamID == FPID_D_COMPARISON_TOLERANCE)
	{
		DblLiseEd->LiseEd[0].Lise.ComparisonTolerance = *(double*)Param;
		DblLiseEd->LiseEd[1].Lise.ComparisonTolerance = *(double*)Param;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM){
		int* iAverage = (int*)Param;
		DblLiseEd->LiseEd[0].Lise.Moyenne = *iAverage;
		DblLiseEd->LiseEd[1].Lise.Moyenne = *iAverage;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM_UPPER){

		int* iAverage = (int*)Param;
		DblLiseEd->LiseEd[0].Lise.Moyenne = *iAverage;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM_LOWER){

		int* iAverage = (int*)Param;
		DblLiseEd->LiseEd[1].Lise.Moyenne = *iAverage;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN)
	{
		DblLiseEd->LiseEd[0].Lise.GainMax = *(double*)Param;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN_UPPER)
	{
		DblLiseEd->LiseEd[0].Lise.GainMax = *(double*)Param;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN_LOWER)
	{
		DblLiseEd->LiseEd[1].Lise.GainMax = *(double*)Param;
	}
	else if(ParamID == FPID_I_REPEATS_CALIBRATION)
	{
		DblLiseEd->iNbCalibrationRepeat = *(int*)Param;
	}
	else if(ParamID == FPID_B_CANCEL_CALIBRATION)
	{
		// Gestion du cancel de la calibration
		DblLiseEd->bCalibrationCancel = *(bool*)Param;
	}
	else if(ParamID == FPID_D_SATURATION_THRESHOLD)
	{
		DblLiseEd->LiseEd[0].Lise.fThresholdSaturation = *(double*)Param;
		DblLiseEd->LiseEd[1].Lise.fThresholdSaturation = *(double*)Param;
	}
	else if(ParamID == FPID_D_WAVE_LENGTH)
	{
		double d_Wl = (double)(1530.0f + *(double*)Param);

		DblLiseEd->LiseEd[0].Lise.dRefWaveLengthNm = d_Wl;
		DblLiseEd->LiseEd[1].Lise.dRefWaveLengthNm = d_Wl;
	}
	else if(ParamID == FPID_C_NI_DEVICE)
	{
		strcpy(DblLiseEd->LiseEd[0].Lise.NIDevice,(char*)Param);
		strcpy(DblLiseEd->LiseEd[1].Lise.NIDevice,(char*)Param);
	}
	else if(ParamID == FPID_C_SERIAL)
	{
		try{
			// On va essayer de separer le SN en 2
			char* STR1;
			char* STR2;
			STR1 = strtok((char*)Param,":");
			STR2 = strtok(NULL,":");

			strcpy(DblLiseEd->LiseEd[0].Lise.SerialNumber,STR1);
			strcpy(DblLiseEd->LiseEd[1].Lise.SerialNumber,STR2);
		}
		catch(...){
			return FP_FAIL;
		}
	}
	else if(ParamID == FPID_C_TYPE)
	{
		strcpy(DblLiseEd->strTypeDevice,(char*)Param);
	}
	else if(ParamID == FPID_C_PN)
	{
		try{
			char* STR1;
			char* STR2;
			STR1 = strtok((char*)Param,":");
			STR2 = strtok(NULL,":");

			strcpy(DblLiseEd->LiseEd[0].Lise.PartNumber,STR1);
			strcpy(DblLiseEd->LiseEd[1].Lise.PartNumber,STR2);
		}
		catch(...){
			return FP_FAIL;
		}
	}
	else if(ParamID == FPID_C_VERSION)
	{
		try{
			char* STR1;
			char* STR2;
			STR1 = strtok((char*)Param,":");
			STR2 = strtok(NULL,":");

			strcpy(DblLiseEd->LiseEd[0].Lise.HardwareVersion,STR1);
			strcpy(DblLiseEd->LiseEd[1].Lise.HardwareVersion,STR2);
		}
		catch(...){
			return FP_FAIL;
		}
	}
	else if(ParamID == FPID_S_SAVE_CURRENT_PARAMS)
	{
		return DBL_LEDISaveConfig(s,DblLiseEd->LiseEd[0].ConfigPath,DblLiseEd->LiseEd[1].ConfigPath,(char*)Param);
	}
	else if(ParamID == FPID_D_AIRGAP_THRESHOLD || ParamID == FPID_D_AIRGAP_UPPER_THRESHOLD)
	{
		double _dTempVar = *(double*)Param;
		DblLiseEd->LiseEd[0].Lise.fThresholAmplitudeAirgap = (float)_dTempVar;
	}
	else if(ParamID == FPID_D_AIRGAP_LOWER_THRESHOLD)
	{
		double _dTempVar = *(double*)Param;
		DblLiseEd->LiseEd[1].Lise.fThresholAmplitudeAirgap = (float)_dTempVar;
	}
	else if(ParamID == FPID_D_AIRGAP_TH_SWITCH_MODE)
	{
		double _dTempVar = *(double*)Param;
		DblLiseEd->LiseEd[0].Lise.fThresholdSwithAGAutoAgTh = (float)_dTempVar;
		DblLiseEd->LiseEd[1].Lise.fThresholdSwithAGAutoAgTh = (float)_dTempVar;
	}
	else if(ParamID == FPID_B_STANDBY)
	{
		bool* bStandby = (bool*)Param; 

		if (DblLiseEd->LiseEd[0].bEnableStandBy && DblLiseEd->LiseEd[1].bEnableStandBy)
		{
			if(*bStandby)
			{
				// On passe en veille
				LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_LISEED]\tSet Gain value to 0");
				// BackUp
				DblLiseEd->LiseEd[0].BackUpSourceValue = DblLiseEd->LiseEd[0].Lise.fSourceValue;
				DblLiseEd->LiseEd[1].BackUpSourceValue = DblLiseEd->LiseEd[1].Lise.fSourceValue;
				DblLiseEd->LiseEd[0].Lise.fSourceValue = 0;
				LEDISetSourcePower(&DblLiseEd->LiseEd[0],DblLiseEd->LiseEd[0].Lise.fSourceValue);
				DblLiseEd->LiseEd[1].Lise.fSourceValue = 0;
				LEDISetSourcePower(&DblLiseEd->LiseEd[1],DblLiseEd->LiseEd[1].Lise.fSourceValue);
			}
			else
			{
				if(DblLiseEd->LiseEd[0].BackUpSourceValue < 0)
				{
					DblLiseEd->LiseEd[0].BackUpSourceValue = 0;
					LogDblED(*DblLiseEd,PRIO_ERROR, "[DBL_LISEED]\tLise 0 gain value has not been saved");
				}
				if(DblLiseEd->LiseEd[1].BackUpSourceValue < 0)
				{
					DblLiseEd->LiseEd[1].BackUpSourceValue = 0;
					LogDblED(*DblLiseEd,PRIO_ERROR, "[DBL_LISEED]\tLise 1 gain value has not been saved");
				}

				// On sort de veille
				LogDblED(*DblLiseEd,PRIO_INFO, "[DBL_LISEED]\tSet LISE 0 Gain value to %f",DblLiseEd->LiseEd[0].BackUpSourceValue);
				DblLiseEd->LiseEd[0].Lise.fSourceValue = DblLiseEd->LiseEd[0].BackUpSourceValue;
				LEDISetSourcePower(&DblLiseEd->LiseEd[0],DblLiseEd->LiseEd[0].Lise.fSourceValue);

				LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_LISEED]\tSet LISE 1 Gain value to %f",DblLiseEd->LiseEd[1].BackUpSourceValue);
				DblLiseEd->LiseEd[1].Lise.fSourceValue = DblLiseEd->LiseEd[1].BackUpSourceValue;
				LEDISetSourcePower(&DblLiseEd->LiseEd[1],DblLiseEd->LiseEd[1].Lise.fSourceValue);

				// -1 signifie que nous ne sommes pas en standby
				DblLiseEd->LiseEd[0].BackUpSourceValue = -1;
				DblLiseEd->LiseEd[1].BackUpSourceValue = -1;
			}
		}
		else
		{
			LogDblED(*DblLiseEd,PRIO_INFO, "[DBL_LISEED]\tEnable stand-by not activated for both LISE ED.");
		}
	}
	else
	{
		return FP_ERROR_PARAM_ID;
	}
	return FP_OK;
}
// get parameters internal to Lise Ed
int DBL_LEDIGetParam(void* s,void* Param, int ParamID)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}
	
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	if(ParamID == FPID_D_CALIBRATE_TOTAL_TH)
	{
		*(double*)Param = DblLiseEd->dTotalThickness;
	}
	else if(ParamID == FPID_I_CALIBRATION_MODE)
	{
		*(int*)Param = DblLiseEd->iCalibrationMode;
	}
	else if(ParamID == FPID_D_CALIBRATE_LOWER_AG)
	{
		*(double*)Param = DblLiseEd->dCalibrateLowerAirgap;
	}
	else if(ParamID == FPID_D_CALIBRATE_UPPER_AG)
	{
		*(double*)Param = DblLiseEd->dCalibrateUpperAirgap;
	}
	else if(ParamID == FPID_D_CALIBRATE_TH_USED)
	{
		*(double*)Param = DblLiseEd->dCalibrateThicknessUsed;
	}
	else if(ParamID == FPID_I_TOTAL_PROBE_USED)
	{
		*(int*)Param = 2;
	}
	else if(ParamID == FPID_I_NUM_OF_PROBE)
	{
		if(DblLiseEd->CfgMeas == UpProbe || DblLiseEd->CfgMeas == DownProbe)
		{
			*(int*)Param = 1;
		}
		else if(DblLiseEd->CfgMeas == BothProbe)
		{
			*(int*)Param = 2;
		}
		else
		{
			*(int*)Param = 1;
		}
	}
	else if(ParamID == FPID_D_XVALUE_TOTAL_TH)
	{
		// on retourne la probe courante
		*(double*)Param = DblLiseEd->XCalibrationValue;
	}
	else if(ParamID == FPID_D_YVALUE_TOTAL_TH)
	{
		// on retourne la probe courante
		*(double*)Param = DblLiseEd->YCalibrationValue;
	}
	else if(ParamID == FPID_D_ZVALUE_TOTAL_TH)
	{
		// on retourne la probe courante
		*(double*)Param = DblLiseEd->ZCalibrationValue;
	}
	else if(ParamID == FPID_I_GETCURRENTPROBE)
	{
		// on retourne la probe courante
		*(int*)Param = DblLiseEd->iCurrentProbe;
	}
	else if(ParamID == FPID_B_AUTOGAIN)
	{
		if(DblLiseEd->CfgMeas == UpProbe)
		{
			*(bool*)Param = DblLiseEd->LiseEd[0].bUseAutoGain;
		}
		else if(DblLiseEd->CfgMeas == DownProbe){
			*(bool*)Param = DblLiseEd->LiseEd[1].bUseAutoGain;
		}
		else{
			*(bool*)Param = DblLiseEd->LiseEd[0].bUseAutoGain;
		}
	}
	else if(ParamID == FPID_B_LIMITEDTIME_MODE)
	{
		if(DblLiseEd->CfgMeas == UpProbe)
		{
			*(bool*)Param = DblLiseEd->LiseEd[0].Lise.bLimitedTime;
		}
		else if(DblLiseEd->CfgMeas == DownProbe){
			*(bool*)Param = DblLiseEd->LiseEd[1].Lise.bLimitedTime;
		}
		else{
			*(bool*)Param = DblLiseEd->LiseEd[0].Lise.bLimitedTime;
		}
	}
	else if(ParamID == FPID_D_COMPARISON_TOLERANCE)
	{
		*(double*)Param = DblLiseEd->LiseEd[0].Lise.ComparisonTolerance;
	}
	else if(ParamID == FPID_D_CURSOURCEPOWER){

		double Power = DblLiseEd->LiseEd[0].Lise.fSourceValue;

		*(double*)Param=Power;
		return DLL_OK;
	}
	else if(ParamID == FPID_D_CURSOURCEPOWER_UPPER){

		double Power = DblLiseEd->GainValue[0];

		*(double*)Param=Power;
		return DLL_OK;
	}
	else if(ParamID == FPID_D_CURSOURCEPOWER_LOWER){

		double Power = DblLiseEd->GainValue[1];

		*(double*)Param=Power;
		return DLL_OK;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM){

		int AverageP = DblLiseEd->LiseEd[0].Lise.Moyenne;

		*(int*)Param = AverageP;
		return DLL_OK;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM_UPPER){

		int AverageP = DblLiseEd->LiseEd[0].Lise.Moyenne;

		*(int*)Param = AverageP;
		return DLL_OK;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM_LOWER){

		int AverageP = DblLiseEd->LiseEd[1].Lise.Moyenne;

		*(int*)Param = AverageP;
		return DLL_OK;
	}
	else if(ParamID == FPID_D_SINGLESHOTDURATION)
	{
		*(double*)Param = ((double)DblLiseEd->LiseEd[0].Lise.Moyenne + (double)DblLiseEd->LiseEd[1].Lise.Moyenne) / (double)DblLiseEd->LiseEd[1].Lise.Frequency + ((double)DblLiseEd->iWaitAfterSwitch/1000.0);
	}
	else if(ParamID == FPID_D_PROBE_FREQUENCY)
	{
		*(double*)Param = DblLiseEd->LiseEd[0].Lise.Frequency;
	}	
	else if(ParamID == FPID_I_VISIBLE_PROBE)
	{
		// on retourne la probe courante
		*(int*)Param = DblLiseEd->iVisibleProbe;
	}
	else if(ParamID == FPID_B_AUTOAIRGAPCONFIG)
	{
		// on retourne la probe courante
		*(bool*)Param = DblLiseEd->LiseEd[0].Lise.bUseAirGapAuto;
	}
	else if(ParamID == FPID_I_NUMCHANNEL)
	{ // pour retourner le nombre de channel définit dans la config du ED
		
		// on retourne le nombre de voie
		*(int*)Param = DblLiseEd->LiseEd[0].Lise.NombredeVoie;

		// retour OK vers la dll
		return DLL_OK;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN)
	{
		*(double*)Param = DblLiseEd->LiseEd[0].Lise.GainMax;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN_UPPER)
	{
		*(double*)Param = DblLiseEd->LiseEd[0].Lise.GainMax;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN_LOWER)
	{
		*(double*)Param = DblLiseEd->LiseEd[1].Lise.GainMax;
	}
	else if(ParamID == FPID_I_REPEATS_CALIBRATION)
	{
		*(int*)Param = DblLiseEd->iNbCalibrationRepeat;
	}
	else if(ParamID == FPID_D_SATURATION_THRESHOLD)
	{
		*(double*)Param = DblLiseEd->LiseEd[0].Lise.fThresholdSaturation;
	}
	else if(ParamID == FPID_D_AIRGAP_THRESHOLD || ParamID == FPID_D_AIRGAP_UPPER_THRESHOLD)
	{
		*(double*)Param = DblLiseEd->LiseEd[0].Lise.fThresholAmplitudeAirgap;
	}
	else if(ParamID == FPID_D_AIRGAP_LOWER_THRESHOLD)
	{
		*(double*)Param = DblLiseEd->LiseEd[1].Lise.fThresholAmplitudeAirgap;
	}
	else if(ParamID == FPID_D_WAVE_LENGTH)
	{
		double d_Wl = (double)(DblLiseEd->LiseEd[0].Lise.dRefWaveLengthNm - 1530.0f);

		*(double*)Param = d_Wl;
	}
	else if(ParamID == FPID_C_NI_DEVICE)
	{
		strcpy((char*)Param,DblLiseEd->LiseEd[1].Lise.NIDevice);
	}
	else if(ParamID == FPID_C_SERIAL)
	{
		strcpy((char*)Param,DblLiseEd->LiseEd[0].Lise.SerialNumber);
		strcat((char*)Param,":");
		strcat((char*)Param,DblLiseEd->LiseEd[1].Lise.SerialNumber);
	}
	else if(ParamID == FPID_C_TYPE)
	{
		strcpy((char*)Param,DblLiseEd->strTypeDevice);
	}
	else if(ParamID == FPID_C_PN)
	{
		strcpy((char*)Param,DblLiseEd->LiseEd[0].Lise.PartNumber);
		strcat((char*)Param,":");
		strcat((char*)Param,DblLiseEd->LiseEd[1].Lise.PartNumber);
	}
	else if(ParamID == FPID_C_VERSION)
	{
		strcpy((char*)Param,DblLiseEd->LiseEd[0].Lise.HardwareVersion);
		strcat((char*)Param,":");
		strcat((char*)Param,DblLiseEd->LiseEd[1].Lise.HardwareVersion);
	}
	else if(ParamID == FPID_B_GET_PROBE_MEASURING_STATE)
	{
		if(DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe].Lise.IndicePeriod.AbsN <= 20)
		{
			*(bool*)Param = false;		
		}
		else{
			*(bool*)Param = true;
		}

		if(!DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe].bLiseEDConnected)
			*(bool*)Param = true;

	}
	else if(ParamID == FPID_D_UPPER_BASE_LINE || ParamID == FPID_D_BASE_LINE)
	{
		*(double*)Param = DblLiseEd->LiseEd[0].Lise.LigneDeBase;
	}
	else if(ParamID == FPID_D_LOWER_BASE_LINE)
	{
		*(double*)Param = DblLiseEd->LiseEd[1].Lise.LigneDeBase;
	}
	else if(ParamID == FPID_B_STANDBY)
	{
		*(bool*)Param = false;
		
		if (DblLiseEd->LiseEd[0].bEnableStandBy && DblLiseEd->LiseEd[1].bEnableStandBy)
		{
			if((DblLiseEd->LiseEd[0].BackUpSourceValue < 0) || (DblLiseEd->LiseEd[1].BackUpSourceValue< 0))
			{
				*(bool*)Param = true;
			}
		}
	}
	else if(ParamID == FPID_D_STANDBY_VALUE)
	{
		*(double*)Param  = 0.0;
	}
	else if(ParamID == FPID_D_FULLINTENSITY_VALUE)
	{
		*(double*)Param = 100.0;	
	}
	else
	{
		return FP_ERROR_PARAM_ID;
	}

	return FP_OK;
}

// Return System Information
int DBL_LEDIGetSystemCaps(void* s,char* Type,char* SerialNumber,double& Range,int& Frequency,double& GainMin, double& GainMax, double& GainStep)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}
	DBL_LISE_ED* DblLiseEd= (DBL_LISE_ED*)s;

	if( Type == NULL || SerialNumber == NULL)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"Invalid Parameter Type or SerialNumber in prototype function GetSystemCaps");
		return FP_FAIL;
	}

	strcpy(Type,DblLiseEd->strTypeDevice);
	
	// on concatène les deux Serial Number SN1:SN2 comme le chromatic
	strcpy(SerialNumber,DblLiseEd->LiseEd[0].Lise.SerialNumber);
	strcat(SerialNumber,":");
	strcat(SerialNumber,DblLiseEd->LiseEd[1].Lise.SerialNumber);
	
	Range = DblLiseEd->LiseEd[0].Lise.Range;
	Frequency = DblLiseEd->LiseEd[0].Lise.Frequency;
	GainMin = DblLiseEd->LiseEd[0].Lise.GainMin;
	GainMax = DblLiseEd->LiseEd[0].Lise.GainMax;
	GainStep = DblLiseEd->LiseEd[0].Lise.GainStep;

	return FP_OK;
}


// Return System Information
int DBL_LEDIGetSystemCapsDouble(void* s,char* Type,char* SerialNumber,double* Range,int* Frequency,double* GainMin, double* GainMax, double* GainStep)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}
	DBL_LISE_ED* DblLiseEd= (DBL_LISE_ED*)s;

	if( Type == NULL || SerialNumber == NULL)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"Invalid Parameter Type or SerialNumber in prototype function GetSystemCaps");
		return FP_FAIL;
	}

	strcpy(Type,DblLiseEd->strTypeDevice);
	
	// on concatène les deux Serial Number SN1:SN2 comme le chromatic
	strcpy(SerialNumber,DblLiseEd->LiseEd[0].Lise.SerialNumber);
	strcat(SerialNumber,":");
	strcat(SerialNumber,DblLiseEd->LiseEd[1].Lise.SerialNumber);
	
	Range[0] = DblLiseEd->LiseEd[0].Lise.Range;
	Frequency[0] = DblLiseEd->LiseEd[0].Lise.Frequency;
	GainMin[0] = DblLiseEd->LiseEd[0].Lise.GainMin;
	GainMax[0] = DblLiseEd->LiseEd[0].Lise.GainMax;
	GainStep[0] = DblLiseEd->LiseEd[0].Lise.GainStep;
	
	Range[1] = DblLiseEd->LiseEd[1].Lise.Range;
	Frequency[1] = DblLiseEd->LiseEd[1].Lise.Frequency;
	GainMin[1] = DblLiseEd->LiseEd[1].Lise.GainMin;
	GainMax[1] = DblLiseEd->LiseEd[1].Lise.GainMax;
	GainStep[1] = DblLiseEd->LiseEd[1].Lise.GainStep;

	return FP_OK;
}

// DLL initialisation
int DBL_LEDIInit(void* s, DBL_LISE_HCONFIG* HardwareConfigDual, LISE_HCONFIG* HardwareConfigTop, LISE_HCONFIG* HardwareConfigBottom, char* configPath, LOGFILE* Log, double Param1, double Param2, double Param3)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	// on marque la probe comme non initialisé
	DblLiseEd->iStatusProbe = PROBE_NON_INIT;

	if(!configPath)
	{
		return FP_FAIL;
	}

	// Utilisation du log externe au module LISE ED
	DblLiseEd->binternalLog = false;
	if (Log)
	{
		DblLiseEd->Log = Log;
	}
	else
	{
		// La structure Log n'est pas définie
		DblLiseEd->Log = (LOGFILE*)SPG_MemAlloc(sizeof(LOGFILE), "Log DOUBLE LISE ED");
		if (!DblLiseEd->Log)
		{
			return FP_FAIL;
		}
		LogfileInit(*DblLiseEd->Log,DBL_LISEED_MODULE_NAME,1|2,1024*1024*4,20);
		DblLiseEd->binternalLog = true;
	}
	
	// log du démarrage de la sonde
	LogDblED(*DblLiseEd,PRIO_INFO,"------------ Startup Double Lise Ed ------------");

	CreateConfigFromFile(*DblLiseEd,configPath);
    UpdateConfigFromHardwareConfig(*DblLiseEd, HardwareConfigDual);

	// initialisation du premier lise ed	
	LogDblED(*DblLiseEd,PRIO_INFO,"Attempted to initialise First Ed from cfgFile : %s",DblLiseEd->strCfgTileFirstProbe);

	void* Pt1stStruct = &DblLiseEd->LiseEd[0];
	int RetInitFstEd = LEDIInit(Pt1stStruct,DblLiseEd->strCfgTileFirstProbe, HardwareConfigTop, DblLiseEd->Log,0,0,0);
	if(RetInitFstEd < 0)	
	{
		LogDblED(*DblLiseEd,PRIO_ERROR,"First Init of Ed Failed, please check your configuration");
		return FP_FIRST_ED_INIT_FAILED;
	}
	else LogDblED(*DblLiseEd,PRIO_INFO,"First Init of Ed Success");

	// initialisation du deuxième Ed
	LogDblED(*DblLiseEd,PRIO_INFO,"Attempted to initialise Second Ed from cfgFile : %s",DblLiseEd->strCfgTileSecondProbe);

	void* Pt2ndStruct = &DblLiseEd->LiseEd[1];
	int RetSecondEd = LEDIInit(Pt2ndStruct,DblLiseEd->strCfgTileSecondProbe, HardwareConfigBottom, DblLiseEd->Log,0,0,LEDI_SLAVE_ED);
	if(RetSecondEd < 0)	
	{
		LogDblED(*DblLiseEd,PRIO_ERROR,"Second Init of Ed Failed, please check your configuration");
		return FP_SECOND_ED_INIT_FAILED;
	}
	else LogDblED(*DblLiseEd,PRIO_INFO,"Second Init of Ed Success");

	// init des modes de Ed, le premier est Master et le deuxième slave
	DblLiseEd->LiseEd[0].DblEdMode = Master;
	DblLiseEd->LiseEd[1].DblEdMode = Slave;

	// on définit les tache d'acquisition du slave comme celle du master
	int RetValue = DBL_SetMasterDevice(*DblLiseEd);
	if(RetValue != FP_OK) return RetValue;

	// initialisation du mode de mesure
	DblLiseEd->CfgMeas = NoProbeDefine;
	DblLiseEd->iThicknessDefineProbe1 = 0;
	DblLiseEd->iThicknessDefineProbe2 = 0;
	DblLiseEd->bSuspendDuringDF = false;
	//DblLiseEd->bProbeRessourceReserved = false;
	//DblLiseEd->ProbeMutex = CreateMutex(NULL,FALSE,NULL);

	// initialisation du gain en double probe
	DblLiseEd->GainValue[0] = 0.0;
	DblLiseEd->GainValue[1] = 0.0;
	DblLiseEd->dThicknessUnkownLayer = 0.0;
	DblLiseEd->dToleranceUnknownLayer = 0.0;

	if(DblLiseEd->iCurrentProbe == -1)
	{
		// on change le switch pour se remettre en probe 0
		DBL_SetCurrentProbe(s,0,true);
	}

	// modification du status de la probe
	DblLiseEd->iStatusProbe = PROBE_INIT;
	DblLiseEd->ProbeState = DblEdStopped;
	DblLiseEd->LiseEd[0].CurrentProbeFromParentStruct = &DblLiseEd->iCurrentProbe;
	DblLiseEd->LiseEd[1].CurrentProbeFromParentStruct = &DblLiseEd->iCurrentProbe;
	DblLiseEd->LiseEd[0].ThisProbe = 0;
	DblLiseEd->LiseEd[1].ThisProbe = 1;
	DblLiseEd->bProbeInCalibration = false;

	// création du fichier de log de cale étalon
	CreateCalibrationFile(*DblLiseEd);

	return FP_OK;
}

// Begin continuous acquisition and process
int DBL_LEDIStartContinuousAcq(void* s)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
	LogDblED(*DblLiseEd,PRIO_INFO,"Start continous acquisition");

	// definition des modes continus sur les deux sondes
	DblLiseEd->LiseEd[0].Lise.AcqMode = Continous;
	DblLiseEd->LiseEd[1].Lise.AcqMode = Continous;

	// start général
	DBL_LEDIAcqStart(s);

	return FP_OK;
}

// Begin Single Shot Acquisition acquisition and process
int DBL_LEDIStartSingleShotAcq(void* s)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
	LogDblED(*DblLiseEd,PRIO_INFO,"Start single shot acquisition");

	// definition des modes continus sur les deux sondes
	DblLiseEd->LiseEd[0].Lise.AcqMode = SingleShot;
	DblLiseEd->LiseEd[1].Lise.AcqMode = SingleShot;

	// start général
	DBL_LEDIAcqStart(s);

	return FP_OK;
}

// Begin acquisition without notion of continous or Single Shot (default mode Single Shot)
int DBL_LEDIAcqStart(void* s)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
	LogDblED(*DblLiseEd,PRIO_INFO,"starting current probe %i.",DblLiseEd->iCurrentProbe);

	// on démarre la probe
	void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
	LEDIAcqStart(PtrStruct);

	// on passe la probe a l'etat started
	DblLiseEd->ProbeState = DblEdStarted;

	return FP_OK;
}

// Stop continuous acquisition and process
int DBL_LEDIAcqStop(void* s)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
	LogDblED(*DblLiseEd,PRIO_INFO,"Stopping current probe %i.",DblLiseEd->iCurrentProbe);

	// on arrete la dernière probe démarée
	void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
	LEDIAcqStop(PtrStruct);

	// on passe la probe a l'etat stopped
	DblLiseEd->ProbeState = DblEdStopped;

	return FP_OK;
}

// Close DLL
int DBL_LEDIClose(void* s)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	// initialisation du premier lise ed	
	LogDblED(*DblLiseEd,PRIO_INFO,"Attempted to close first Lise ed probe");

	// on change le switch pour se remettre en probe 0
	DBL_SetCurrentProbe(s,0,true);

	// on fait un stop de la probe sourante
	DBL_LEDIAcqStop(s);

	// fermeture du fichier de log de cale étalon
	CloseCalibrationFile(*DblLiseEd);

	void* Pt1stStruct = &DblLiseEd->LiseEd[0];
	int Ret1stClose = LEDIClose(Pt1stStruct);

	if(Ret1stClose==FP_OK) 	LogDblED(*DblLiseEd,PRIO_INFO,"Close 1st Lise ed probe Success");
	else LogDblED(*DblLiseEd,PRIO_ERROR,"Close 1st Lise ed probe FAIL");

	// initialisation du premier lise ed	
	LogDblED(*DblLiseEd,PRIO_INFO,"Attempted to close second Lise ed probe");

	void* Pt2ndStruct = &DblLiseEd->LiseEd[1];
	int Ret2ndClose = LEDIClose(Pt2ndStruct);

	if(Ret2ndClose==FP_OK) 	LogDblED(*DblLiseEd,PRIO_INFO,"Close 2nd Lise ed probe Success");
	else LogDblED(*DblLiseEd,PRIO_ERROR,"Close 2nd Lise ed probe FAIL");

	// on marque la probe comme non initialisé
	DblLiseEd->iStatusProbe = PROBE_NON_INIT;

	return FP_OK;
}

// Sauvegarde de certains paramètres de calibration
int DBL_LEDISaveConfig(void* s,char* ConfigFile1,char* ConfigFile2,char* Password){
	int returnValue = DLL_OK;

	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}	
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	if(strcmp(Password,"SiTuTrouvesLePassTuEsFort")!=0)
	{
		LogfileF(*DblLiseEd->Log, "[LISE_ED]\tError DBL_LEDISaveConfig - Wrong Password");
		return DLL_FAIL;
	}
	else
	{

		// On ouvre le fichier de params
		SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
		CFG_Init(CFG,0,DblLiseEd->strCfgFilePath,1024,0);

		if(strcmp(DblLiseEd->strTypeDevice,"") != 0){
			if(CFG_SetStringParam(CFG, "DeviceType", DblLiseEd->strTypeDevice, 0) == 0)
						returnValue = DLL_FAIL;
		}
		// On ferme le fichier de params
		CFG_Close(CFG);


		// Probe 1
		LEDISaveConfig(&DblLiseEd->LiseEd[0],ConfigFile1,Password);

		// Probe 2
		LEDISaveConfig(&DblLiseEd->LiseEd[1],ConfigFile2,Password);

	}
	return returnValue;
}
// Set source power
int DBL_LEDISetSourcePower(void* s,double Volts)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
	LEDISetSourcePower(PtrStruct,Volts);

	return FP_OK;
}

//######################################## Results #######################################

// Get the number of samples of the last waveform period
int DBL_LEDIGetNbSamplesWaveform(void* s,int* NbSamples)
{
	return FP_OK;
}

// Get Waveform Securised
int DBL_LEDIGetRawSignal(char* Password, void* s,double* I,int* NbSamples,float* StepX,int Voie,float* SaturationValue, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
	LogDblED(*DblLiseEd,PRIO_INFO,"GetRawSignal with the both probe.");

	if(DblLiseEd->bSuspendDuringDF)
		return FP_OK;

	// timeout de Synchro
	clock_t TimeStart = clock(); clock_t Timeout = 200;

	// on récupère le signal sur la waveform visible
 	void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iVisibleProbe];
	LEDIGetRawSignal(Password,PtrStruct,I,NbSamples,StepX,Voie,SaturationValue,SelectedPeaks,nbSelectedPeaks,DiscardedPeaks,nbDiscardedPeaks);

	return FP_OK;
}

// Get the last waveform period
int DBL_LEDIGetWaveform(void* s,double* I,int* NbSamples,float* StepX, int Voie, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	if(DblLiseEd->bSuspendDuringDF)
		return FP_OK;

	double SatValue = 0.0;
	DBL_LEDIGetRawSignal("SiTuTrouvesLePassTuEsFort",s,I,NbSamples,StepX,Voie,(float*)SelectedPeaks,&SatValue,nbSelectedPeaks,DiscardedPeaks,nbDiscardedPeaks);

	return FP_OK;
}

// Get the number of peaks of the last period
int DBL_LEDIGetNbPeaksPeriod(void* s,int* NbPeaks,int* Voie)
{
	return FP_OK;
}

// Get the peaks of the last period
int DBL_LEDIGetPeaksPeriod(void* s,double* XPosRel, double* Intensite, double* Quality, int* Sens, int* Flag, int* NbPeaks,int* Voie)
{
	return FP_OK;
}

// Read temperature
int DBL_LEDIReadTemperature(void* s,double* Temperature)
{
	return FP_OK;
}

// Read optical power received
int DBL_LEDIReadPower(void* s,double* Power)
{ 
	return FP_OK;
}

// Get thickness
int DBL_LEDIGetThickness(void* s,double* _Thickness, double* _Quality,int _iNumThickness)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}

    SC_CACHED_ARRAY(double, Thickness, _Thickness, _iNumThickness);
    SC_CACHED_ARRAY(double, Quality, _Quality, _iNumThickness);

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
	LogDblED(*DblLiseEd,PRIO_INFO,"Entering in Getthickness function");

	if(DblLiseEd->bSuspendDuringDF)
		return FP_OK;

	// différents cas de configuration
	if(DblLiseEd->CfgMeas == NoProbeDefine)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"No probe define, GetThickness for probe 1.");
	
		// on démarre la probe du haut
		void* PtrStruct1 = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
		return LEDIGetThickness(PtrStruct1,Thickness,Quality, _iNumThickness);
	}
	else if(DblLiseEd->CfgMeas == UpProbe)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"GetThickness of the up/master probe.");

		// on démarre la probe du haut
		void* PtrStruct1 = &DblLiseEd->LiseEd[0];
		return LEDIGetThickness(PtrStruct1,Thickness,Quality, _iNumThickness);
	}
	else if(DblLiseEd->CfgMeas == DownProbe)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"GetThickness of the second down/slave probe.");

		// on démarre la probe du bas
		void* PtrStruct2 = &DblLiseEd->LiseEd[1];
		int RetDown = LEDIGetThickness(PtrStruct2,Thickness,Quality, _iNumThickness);

		// invert des valeurs // Modif MP 08/11/2012, on inverse aussi l airgap
		/*int NumThToInvert=0;*/
		SC_FIXED_ARRAY(double, THInvert,MAX_THICKNESS); memset(THInvert,0,MAX_THICKNESS*sizeof(double));
		SC_FIXED_ARRAY(double, QInvert,MAX_THICKNESS); memset(QInvert,0,MAX_THICKNESS*sizeof(double));
		{for(int i=0;i<DblLiseEd->iThicknessDefineProbe2;i++)
		{
			THInvert[i]=Thickness[i];
			QInvert[i]=Quality[i];
		}}
		{for(int i=0;i < DblLiseEd->iThicknessDefineProbe2;i++)
		{
			Thickness[DblLiseEd->iThicknessDefineProbe2 -i]=THInvert[i];
			Quality[DblLiseEd->iThicknessDefineProbe2 -i]=QInvert[i];
		}}
		// Upper AG = - lower AG
		Thickness[0] = -THInvert[0];

		return RetDown;
	}
	else if(DblLiseEd->CfgMeas == BothProbe)
	{

		int bSaveCurrentProbe = DblLiseEd->iCurrentProbe;
		LogDblED(*DblLiseEd,PRIO_INFO,"GetThickness with the both probe.");

		// Dans ce cas, le mode continuous n'existe pas
		bool ContinuousAsked = false;
		if(DblLiseEd->LiseEd[1].Lise.AcqMode == Continous){
			DblLiseEd->LiseEd[1].Lise.AcqMode = SingleShot;
			DblLiseEd->LiseEd[0].Lise.AcqMode = SingleShot;
			ContinuousAsked = true;
		}
		// définition des tableaux de résultats
		SC_FIXED_ARRAY(double, ThicknessPb1,MAX_THICKNESS);		SC_FIXED_ARRAY(double, ThicknessPb2,MAX_THICKNESS);
		SC_FIXED_ARRAY(double, QualityPb1,MAX_THICKNESS);		SC_FIXED_ARRAY(double, QualityPb2,MAX_THICKNESS);

		memset(ThicknessPb1,0,MAX_THICKNESS*sizeof(double));
		memset(ThicknessPb2,0,MAX_THICKNESS*sizeof(double));
		memset(QualityPb1,0,MAX_THICKNESS*sizeof(double));
		memset(QualityPb2,0,MAX_THICKNESS*sizeof(double));

		// on change de probe
		void* PtrStruct1 = &DblLiseEd->LiseEd[0];
		if(DBL_SetCurrentProbe(s,0,false) != FP_OK)
		{
			LogDblED(*DblLiseEd,PRIO_ERROR,"[SWITCH]\tProbe 1 selection failed.");		
#ifdef _DEBUG
			BreakHook();
#endif
			//DblLiseEd->bProbeRessourceReserved = false;
			//ReleaseMutex(DblLiseEd->ProbeMutex);
			if(ContinuousAsked){
				DblLiseEd->LiseEd[1].Lise.AcqMode = Continous;
				DblLiseEd->LiseEd[0].Lise.AcqMode = Continous;
			}
			return FP_SELECTCHANNEL_FAILED;
		}		

		if(DblLiseEd->iCurrentProbe != 0) 
		{
#ifdef _DEBUG
			BreakHook();
#endif
		}
		
		// on démarre la probe du haut
		LogDblED(*DblLiseEd,PRIO_INFO,"GetThickness Probe 1.\n");
		int RetGet1 = LEDIGetThickness(PtrStruct1,ThicknessPb1,QualityPb1, _iNumThickness);

		if(DblLiseEd->iCurrentProbe != 0) 
		{
#ifdef _DEBUG
			BreakHook();
#endif
		}

		// on change de probe
		void* PtrStruct2 = &DblLiseEd->LiseEd[1];
		if(DBL_SetCurrentProbe(s,1,false) != FP_OK)
		{
			LogDblED(*DblLiseEd,PRIO_ERROR,"[SWITCH]\tProbe 2 selection failed.");		
#ifdef _DEBUG
			BreakHook();
#endif
			if(ContinuousAsked){
				DblLiseEd->LiseEd[0].Lise.AcqMode = Continous;
				DblLiseEd->LiseEd[1].Lise.AcqMode = Continous;
			}

			//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
			return FP_SELECTCHANNEL_FAILED;
		}

		if(DblLiseEd->iCurrentProbe != 1) 
		{
#ifdef _DEBUG
			BreakHook();
#endif
		}

		// on démarre la probe du haut
		LogDblED(*DblLiseEd,PRIO_INFO,"GetThickness Probe 2.\n");
		int RetGet2 = LEDIGetThickness(PtrStruct2,ThicknessPb2,QualityPb2, _iNumThickness);

		if(DblLiseEd->iCurrentProbe != 1) 
		{
#ifdef _DEBUG
			BreakHook();
#endif
		}

		if(ContinuousAsked){
			DblLiseEd->LiseEd[1].Lise.AcqMode = Continous;
			DblLiseEd->LiseEd[0].Lise.AcqMode = Continous;
		}

		//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
		// retour d'erreur lie a getthickness
		if (RetGet1 != FP_OK)
		{
			return FP_GETH1_FAILED;
		}
		if (RetGet2 != FP_OK)
		{
			return FP_GETH2_FAILED;
		}


		LogDblED(*DblLiseEd,PRIO_INFO,"Quality compute.\n");
		double TotalThicknessProbe1 = 0;
		for(int i=0;i<DblLiseEd->iThicknessDefineProbe1;i++)
		{
			TotalThicknessProbe1 += ThicknessPb1[i];
			// on copier les éléments dans le tacleau
			Thickness[i] = ThicknessPb1[i];
			Quality[i] = QualityPb1[i];
		}

		double TotalThicknessProbe2 = 0;
		for(int i=0;i<DblLiseEd->iThicknessDefineProbe2;i++)
		{
			TotalThicknessProbe2 += ThicknessPb2[i];
			// copie des éléments à l'indice après la valeur de zéro
			Thickness[i + (DblLiseEd->iThicknessDefineProbe1 + 1)] = ThicknessPb2[(DblLiseEd->iThicknessDefineProbe2 - 1) - i ];
			Quality[i + (DblLiseEd->iThicknessDefineProbe1 + 1)] = QualityPb2[(DblLiseEd->iThicknessDefineProbe2 - 1) - i ];
		}

		// calcul et copie de l'épaisseur target dans la cible
		double UnknownThickness = DblLiseEd->dTotalThickness - TotalThicknessProbe1 - TotalThicknessProbe2;
		Thickness[DblLiseEd->iThicknessDefineProbe1] = UnknownThickness;
		Quality[DblLiseEd->iThicknessDefineProbe1] = min(QualityPb1[0],QualityPb2[DblLiseEd->iThicknessDefineProbe2-1]);
		double QualityLayerUnknown = Quality[DblLiseEd->iThicknessDefineProbe1];

		if(/*UnknownThickness < 0 ||*/ RetGet1 != FP_OK || RetGet2 != FP_OK)
		{
			for(int i=0;i<_iNumThickness;i++) Quality[i]=0.0;
			return FP_ERROR_CALCULATETH;
		}
		else if(QualityPb1[0] <= 0.0 || QualityPb2[0] <= 0.0)
		{
			for(int i=0;i<_iNumThickness;i++) Quality[i]=0.0;
		}
		else
		{
			// on vérifie etre dans la tolérance
			if(UnknownThickness > DblLiseEd->dThicknessUnkownLayer + DblLiseEd->dToleranceUnknownLayer ||  UnknownThickness < DblLiseEd->dThicknessUnkownLayer - DblLiseEd->dToleranceUnknownLayer)
			{
				// il ne faut pas etre en calibration
				if(!DblLiseEd->bProbeInCalibration)
				{
					for(int i=0;i<_iNumThickness;i++) Quality[i] = 0.0;
				}
			}
		}

		DBL_SetCurrentProbe(s,bSaveCurrentProbe,false);
	}
	else
	{
		LogDblED(*DblLiseEd,PRIO_ERROR,"Wrong probe configuration. No config measure mode defined.");
		return FP_CFG_MEA_ERROR;
	}

	return FP_OK;
}



// Get thicknesses
int DBL_LEDIGetThicknesses(void* s, double* Dates, double* Thicknesses, double* Quality,int NumValues)
{
	return FP_OK;
}

// function to invert sample definition
void InvertArray(double* ArrayToInvert,int NumLayers)
{
	// déclaration du tableau à inverser
	SC_FIXED_ARRAY(double, ArrayInverted,MAX_THICKNESS);

	// boucle d'inverstion
	{for(int i= 0;i<NumLayers;i++)
	{
		ArrayInverted[NumLayers - 1 - i] = ArrayToInvert[i];
	}}

	// on copie les valeurs apr écrasement
	{for(int i=0;i<NumLayers;i++)
	{
		ArrayToInvert[i]=ArrayInverted[i];
	}}
}
// Define sample
int DBL_LEDIDefineSample(void* s,char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int NbThickness, double Gain, double QualityThreshold)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}
	
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	// init du tableau de gain
	double GainValue[2]; memset(GainValue,0,2*sizeof(double));
	double QualityThresholdValue[2]; memset(QualityThresholdValue,0,2*sizeof(double));
	

	// on envoie la double valeur de gain
	GainValue[0] = Gain;
	GainValue[1] = Gain;

	QualityThresholdValue[0] = QualityThreshold;
	QualityThresholdValue[1] = QualityThreshold;

	// on apelle la fonction gain double
	return DBL_LEDIDefineSampleDouble(s,Name,SampleNumber,Thickness,Tolerance,Index,Intensity,NbThickness,GainValue,QualityThresholdValue);
}
// Define sample double pour envoyer deux valeurs de gain différents
int DBL_LEDIDefineSampleDouble(void* s,char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int NbThickness, double* Gain, double* QualityThreshold)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}
	
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	// On prend le mutex
	/*if(WaitForSingleObject(DblLiseEd->ProbeMutex,2000)==0){
		return FP_BUSY;
	}*/

	// déclaration des variables
	SC_FIXED_ARRAY(double, ThicknessDef1,MAX_THICKNESS);	SC_FIXED_ARRAY(double, ThicknessDef2,MAX_THICKNESS);
	SC_FIXED_ARRAY(double, ToleranceDef1,MAX_THICKNESS);	SC_FIXED_ARRAY(double, ToleranceDef2,MAX_THICKNESS);
	SC_FIXED_ARRAY(double, IndexDef1,MAX_THICKNESS);		SC_FIXED_ARRAY(double, IndexDef2,MAX_THICKNESS);
	SC_FIXED_ARRAY(double, IntensityDef1,MAX_THICKNESS);	SC_FIXED_ARRAY(double, IntensityDef2,MAX_THICKNESS);
	int NumLayerDef1 = 0; int NumLayerDef2 = 0;

	// init des tableaux
	memset(ThicknessDef1,0.0,MAX_THICKNESS * sizeof(double));	memset(ThicknessDef2,0.0,MAX_THICKNESS* sizeof(double));
	memset(ToleranceDef1,0.0,MAX_THICKNESS* sizeof(double));	memset(ToleranceDef2,0.0,MAX_THICKNESS* sizeof(double));
	memset(IndexDef1,0.0,MAX_THICKNESS* sizeof(double));		memset(IndexDef2,0.0,MAX_THICKNESS* sizeof(double));
	memset(IntensityDef1,0.0,MAX_THICKNESS* sizeof(double));	memset(IntensityDef2,0.0,MAX_THICKNESS* sizeof(double));

	// init des variables de boucle pour def
	bool bFillFirstDefineSample = true;
	bool bUseDef1 = false;
	bool bUseDef2 = false;

	// on modifie les valeurs des gains courants
	DblLiseEd->GainValue[0] = Gain[0];
	DblLiseEd->GainValue[1] = Gain[1];
	DblLiseEd->LiseEd[0].Lise.fSourceValue = (float)Gain[0];
	DblLiseEd->LiseEd[1].Lise.fSourceValue = (float)Gain[1];

	LogDblED(*DblLiseEd,PRIO_INFO,"NbThickness %i",NbThickness);
	// on fait une copie des pour les deux define sample
	DbgCHECK(NbThickness>=MAX_THICKNESS, "DBL_LEDIDefineSampleDouble check MAX_THICKNESS value");
	for(int i =0;i<__min(NbThickness,MAX_THICKNESS);i++)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"Index[i] %f",Index[i]);
		if(Index[i] > 0.0)
		{
			if(bFillFirstDefineSample)
			{
				LogDblED(*DblLiseEd,PRIO_INFO,"Fill First");
				ThicknessDef1[NumLayerDef1] = Thickness[i];
				ToleranceDef1[NumLayerDef1] = Tolerance[i];
				IndexDef1[NumLayerDef1] = Index[i];
				IntensityDef1[NumLayerDef1] = Intensity[i];
				NumLayerDef1++;
				bUseDef1 = true;
			}
			else
			{
				LogDblED(*DblLiseEd,PRIO_INFO,"Fill Second");
				ThicknessDef2[NumLayerDef2] = Thickness[i];
				ToleranceDef2[NumLayerDef2] = Tolerance[i];
				IndexDef2[NumLayerDef2] = Index[i];
				IntensityDef2[NumLayerDef2] = Intensity[i];
				NumLayerDef2++;
				bUseDef2 = true;
			}
		}
		else
		{
			LogDblED(*DblLiseEd,PRIO_INFO,"Fill Unknown");
			DblLiseEd->dThicknessUnkownLayer = Thickness[i];
			DblLiseEd->dToleranceUnknownLayer = Tolerance[i];
			bFillFirstDefineSample = false;
		}
	}

	// definition du mode de la probe
	if(bUseDef1 && bUseDef2)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"Ed Probe 1 and 2 Cfg");
		DblLiseEd->CfgMeas = BothProbe;
		DblLiseEd->LiseEd[0].DblEdMeasurementMode = MeasBothProbe;
		DblLiseEd->LiseEd[1].DblEdMeasurementMode = MeasBothProbe;
	}
	else if(bUseDef1 && DblLiseEd->iCurrentProbe == 0)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"Ed Probe 1 Cfg");
		DblLiseEd->CfgMeas = UpProbe;
		DblLiseEd->LiseEd[0].DblEdMeasurementMode = MeasSingleProbe;
		DblLiseEd->LiseEd[1].DblEdMeasurementMode = MeasSingleProbe;
	}
	else if(bUseDef1 && DblLiseEd->iCurrentProbe == 1)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"Ed Probe 2 Cfg");
		DblLiseEd->CfgMeas = DownProbe;
		DblLiseEd->LiseEd[0].DblEdMeasurementMode = MeasSingleProbe;
		DblLiseEd->LiseEd[1].DblEdMeasurementMode = MeasSingleProbe;
	}
	else
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"Ed Probe unknown");

		if (strcmp(Name, "NOSAMPLE") != 0)
		{
			LogDblED(*DblLiseEd,PRIO_ERROR,"Probe configuration WRONG");
			DblLiseEd->CfgMeas = NoProbeDefine;
			DblLiseEd->LiseEd[0].DblEdMeasurementMode = MeasSingleProbe;
			DblLiseEd->LiseEd[1].DblEdMeasurementMode = MeasSingleProbe;

			// Dans ce cas on ne sort pas de la fonction, on permet de regler le gain
			return FP_DEF_MODE_INCORRECT;
		}	
	}

	// init des retours de fonctions
	int RetDef1 = FP_OK; int RetDef2 = FP_OK;

	// cas de la double définition
	if(bUseDef1 && bUseDef2)
	{
		DblLiseEd->bSuspendDuringDF = true;
		// exécution des define sample
		if(bUseDef1)
		{
			// log du démarrage de la sonde
			LogDblED(*DblLiseEd,PRIO_INFO,"Define Sample for the first probe");
			DblLiseEd->iThicknessDefineProbe1 = NumLayerDef1;
			DblLiseEd->iThicknessDefineProbe2 = 0;
			
			void* PtrStruct1 = &DblLiseEd->LiseEd[0];
			RetDef1 = LEDIDefineSample(PtrStruct1,Name,SampleNumber,ThicknessDef1,ToleranceDef1,IndexDef1,IntensityDef1,NumLayerDef1,Gain[0],QualityThreshold[0]);
			if(RetDef1 != FP_OK)
			{
				LogDblED(*DblLiseEd,PRIO_ERROR,"Define sample for probe 1 FAILED");
			}
		}

		if(bUseDef2)
		{
			// log du démarrage de la sonde
			LogDblED(*DblLiseEd,PRIO_INFO,"Define Sample for the second probe");
			if(bUseDef1)	DblLiseEd->iThicknessDefineProbe1 = NumLayerDef1;
			else DblLiseEd->iThicknessDefineProbe1 = 0;
			DblLiseEd->iThicknessDefineProbe2 = NumLayerDef2;
		
			// on inverse la définition d'échantillon
			InvertArray(ThicknessDef2,NumLayerDef2);
			InvertArray(ToleranceDef2,NumLayerDef2);
			InvertArray(IndexDef2,NumLayerDef2);
			InvertArray(IntensityDef2,NumLayerDef2);

			void* PtrStruct2 = &DblLiseEd->LiseEd[1];
			RetDef2 = LEDIDefineSample(PtrStruct2,Name,SampleNumber,ThicknessDef2,ToleranceDef2,IndexDef2,IntensityDef2,NumLayerDef2,Gain[1],QualityThreshold[1]);
			if(RetDef2 != FP_OK)
			{
				LogDblED(*DblLiseEd,PRIO_ERROR,"Define sample for probe 2 FAILED");
			}
		}

		DblLiseEd->bSuspendDuringDF = false;
		//ReleaseMutex(DblLiseEd->ProbeMutex);
	
		// retour en fonction des codes erreurs
		if(RetDef1 != FP_OK)
		{
			return FP_DEFINESPL1_FAILED;
		}
		if(RetDef2 != FP_OK)
		{
			return FP_DEFINESPL2_FAILED;
		}
	}
	else
	{
		// cas définition simple, on choisit de lancer la définition sur la probe courante
		LogDblED(*DblLiseEd,PRIO_INFO,"Define Sample for current probe %i",DblLiseEd->iCurrentProbe);
		LogDblED(*DblLiseEd,PRIO_INFO,"Num Layer %i",NumLayerDef1);
		// Modif YR : on tolere NumLayerDef1 == 0
		int NumLayer; 
		if (NumLayerDef1 == 0) NumLayer = 0;
		else NumLayer = NumLayerDef1 - 1;
		
		if(DblLiseEd->iCurrentProbe == 0){
			DblLiseEd->iThicknessDefineProbe1 = NumLayer;
		}
		else
		{
			DblLiseEd->iThicknessDefineProbe2 = NumLayer;
	
			// on inverse la définition d'échantillon
			InvertArray(ThicknessDef1,NumLayerDef1);
			InvertArray(ToleranceDef1,NumLayerDef1);
			InvertArray(IndexDef1,NumLayerDef1);
			InvertArray(IntensityDef1,NumLayerDef1);
		}

		// on lance la définition sur la probe courante
		void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
		if(LEDIDefineSample(PtrStruct,Name,SampleNumber,ThicknessDef1,ToleranceDef1,IndexDef1,IntensityDef1,NumLayer,Gain[DblLiseEd->iVisibleProbe],QualityThreshold[DblLiseEd->iVisibleProbe]) != FP_OK)
		{
			LogDblED(*DblLiseEd,PRIO_ERROR,"Define sample for probe %i failed",DblLiseEd->iCurrentProbe);
			DblLiseEd->bSuspendDuringDF = false;	
			//ReleaseMutex(DblLiseEd->ProbeMutex);
			if (DblLiseEd->iCurrentProbe == 0) return FP_DEFINESPL1_FAILED;
			if (DblLiseEd->iCurrentProbe == 1) return FP_DEFINESPL2_FAILED;
		}
	}

	return FP_OK;
}

// Set Stage Position Info
int DBL_LEDISetStagePositionInfo(void* s,double* XSystemPosition, double* YSystemPosition, double *ZSystemPosition)
{
	return FP_OK;
}

//####################################### Sauvegardes ######################################

// Save the last period wafeform in a text file
int DBL_LEDISaveWaveForm(void* s,char* FileName,float* StepX)
{
	return FP_OK;
}


// Save peaks in a text file
int DBL_LEDISavePeaks(void* s,char* FileName)
{
	return FP_OK;
}

// Save thickness in a text file
int DBL_LEDIAcqSaveThickness(void* s,char* FileName)
{
	return FP_OK;
}

int DBL_LEDICalibrateDark(void* s)
{
	return FP_OK;
}

int DBL_LEDICalibrateThickness(void* s, float Value)
{
	return FP_OK;
}

int DBL_LEDIRestartMeasurement(void* s)
{
	return FP_OK;
}

//####################################### user interface ######################################

#ifdef FDE
int DBL_LEDIOpenSettingsWindow(void* s,int ShowGainSetting)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}
	
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
	LEDIOpenSettingsWindow(PtrStruct,ShowGainSetting);

	return FP_OK;
}

int DBL_LEDIUpdateSettingsWindow(void* s)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}
	
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
	LEDIUpdateSettingsWindow(PtrStruct);

	return FP_OK;
}

int DBL_LEDICloseSettingsWindow(void* s)
{
	// Precondition
	if (!s)
	{
		return FP_FAIL;
	}
	
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	void* PtrStruct = &DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe];
	LEDICloseSettingsWindow(PtrStruct);

	return FP_OK;
}
#endif

// Set Param Auto Gain
int SetParamAutoGain_Double(void* s, bool bAutoGain)
{
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	// On attend que la probe soit liberee
	/*int iTimeOut = 0;
	while(DblLiseEd->bProbeRessourceReserved && (iTimeOut < 10)){
		Sleep(100);
		iTimeOut ++;
	}
	if(iTimeOut >= 10){
		return FP_BUSY;
	}
	// On reserve la probe
	DblLiseEd->bProbeRessourceReserved = true;*/
	/*if(WaitForSingleObject(DblLiseEd->ProbeMutex,1000)==0){
		return FP_BUSY;
	}*/
	//DblLiseEd->GainValue[0];
	int Ret = FP_OK;

	if(DblLiseEd->CfgMeas == UpProbe)
	{
		DblLiseEd->LiseEd[0].bUseAutoGain = bAutoGain;
		// Si le mode de la probe est continu ou arrété, on procède au gain auto
		if(DblLiseEd->LiseEd[0].Lise.AcqMode == Continous)
		{
			Ret = ProcessAutoGain_Continuous_Mode(&DblLiseEd->LiseEd[0]);
			DblLiseEd->GainValue[0] = DblLiseEd->LiseEd[0].Lise.fSourceValue;
			//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
		}
	}
	else if(DblLiseEd->CfgMeas == DownProbe){
		DblLiseEd->LiseEd[1].bUseAutoGain = bAutoGain;
		// Si le mode de la probe est continu ou arrété, on procède au gain auto
		if(DblLiseEd->LiseEd[1].Lise.AcqMode == Continous)
		{
			Ret = ProcessAutoGain_Continuous_Mode(&DblLiseEd->LiseEd[1]);
			DblLiseEd->GainValue[1] = DblLiseEd->LiseEd[1].Lise.fSourceValue;
			//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
		}
	}
	else{
		int NumCurrentProbe = DblLiseEd->iCurrentProbe;

		int UPreturn, DOWNreturn;
		DblLiseEd->LiseEd[0].bUseAutoGain = bAutoGain;
		if(DblLiseEd->LiseEd[0].Lise.AcqMode == Continous)
		{
			if(DBL_SetCurrentProbe(DblLiseEd,0,false) == FP_OK){
				UPreturn = ProcessAutoGain_Continuous_Mode(&DblLiseEd->LiseEd[0]);
			}
			else{
				UPreturn = FP_SELECTCHANNEL_FAILED;
			}
			DblLiseEd->GainValue[0] = DblLiseEd->LiseEd[0].Lise.fSourceValue;
		}
		else{
			UPreturn = FP_OK;
		}
		DblLiseEd->LiseEd[1].bUseAutoGain = bAutoGain;
		if(DblLiseEd->LiseEd[1].Lise.AcqMode == Continous)
		{
			if(DBL_SetCurrentProbe(DblLiseEd,1,false) == FP_OK){
				DOWNreturn = ProcessAutoGain_Continuous_Mode(&DblLiseEd->LiseEd[1]);
			}
			else{
				DOWNreturn = FP_SELECTCHANNEL_FAILED;
			}
			DblLiseEd->GainValue[1] = DblLiseEd->LiseEd[1].Lise.fSourceValue;
		}
		else{
			DOWNreturn = FP_OK;
		}

		DBL_SetCurrentProbe(DblLiseEd,NumCurrentProbe,false);
		//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
		if(UPreturn != FP_OK){
			return UPreturn;
		}
		if(DOWNreturn != FP_OK){
			return DOWNreturn;
		}
		
		return FP_OK;
	}


	return Ret;
}