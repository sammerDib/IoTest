/*
 * $Id: LISE_ED_DLL_Reglages.cpp 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#include <windows.h>
#include <stdio.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

// #include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"
// ## probe-specific headers ##

#include "LISE_ED_DLL_UI_Struct.h"
#include "LISE_ED_DLL_Internal.h"
#include "LISE_ED_DLL_Acquisition.h"
#include "LISE_ED_DLL_Config.h"
#include "LISE_ED_DLL_Create.h"
#include "LISE_ED_DLL_General.h"
#include "LISE_ED_DLL_Process.h"
#include "LISE_ED_DLL_Log.h"
#include "LISE_ED_DLL_Reglages.h"

int SetAnalogOutputSource1(LISE_ED& LiseEd,float ValeurSource1)
{
#ifdef DEVICECONNECTED
	if(LiseEd.Lise.T_ControlSource1!=0)
	{
		int32 error;
		error = DAQmxWriteAnalogScalarF64 (LiseEd.Lise.T_ControlSource1, 1, LiseEd.Lise.Timeout , ValeurSource1, NULL);
		DisplayDAQmxError(LiseEd,error,"SetAnalogOutputSource1 Error");
		if(error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else	return STATUS_FAIL;
#else //DEVICECONNECTED
	return STATUS_OK;
#endif
}

int SetAnalogOutputSource2(LISE_ED& LiseEd,float ValeurSource2)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if(LiseEd.Lise.T_ControlSource2!=0)
		{
			int32 error;
			error = DAQmxWriteAnalogScalarF64 (LiseEd.Lise.T_ControlSource2, 1, LiseEd.Lise.Timeout , ValeurSource2, NULL);
			DisplayDAQmxError(LiseEd,error,"SetAnalogOutputSource2 Error");
			if(error == 0) return STATUS_OK;
			else return STATUS_FAIL;
		}
		else	return STATUS_FAIL;
	}
	else return STATUS_OK;
#else //DEVICECONNECTED
	return STATUS_OK;
#endif
}

int SetAnalogOutput(LISE_ED& LiseEd,float ValeurSource1, float ValeurSource2)
{
	int ErrorSource1,ErrorSource2;
	ErrorSource1 = SetAnalogOutputSource1(LiseEd,ValeurSource1);
	ErrorSource2 = SetAnalogOutputSource2(LiseEd,ValeurSource2);
	if((ErrorSource1 == 1) && (ErrorSource2 == 1))	return STATUS_OK;
	else STATUS_FAIL;
	return STATUS_OK;
}

int EnableScanOn(LISE_ED& LiseEd)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if(LiseEd.Lise.T_EnableLr != 0)
		{
			int32 ErrorAlarmLr, ErrorEnableLr;
			uInt32 AlarmLr;
			int32 read, written;
			ErrorAlarmLr =  DAQmxReadDigitalU32(LiseEd.Lise.T_AlarmLr,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&AlarmLr,1,&read,NULL);	DisplayDAQmxError(LiseEd,ErrorAlarmLr,"EnableScanOn Error Alarm Not Valid");
			if (AlarmLr == 0)
			{
				uInt32 EnableLr = -1;
				ErrorEnableLr = DAQmxWriteDigitalU32(LiseEd.Lise.T_EnableLr,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&EnableLr,&written,NULL);
				DisplayDAQmxError(LiseEd,ErrorEnableLr,"EnableScanOn Error");
				return STATUS_OK;
			}
			else return STATUS_FAIL;
		}
		else return STATUS_FAIL;
	}
	else return STATUS_OK;
#else //DEVICECONNECTED
	return STATUS_OK;
#endif
}

int EnableScanOff(LISE_ED& LiseEd)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if(LiseEd.Lise.T_EnableLr != 0)
		{
			int32 ErrorEnableLr,written;
			uInt32 EnableLr = 0;
			ErrorEnableLr = DAQmxWriteDigitalU32(LiseEd.Lise.T_EnableLr,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&EnableLr,&written,NULL);
			DisplayDAQmxError(LiseEd,ErrorEnableLr,"EnableScanOff Error");
			return STATUS_OK;
		}
		else return STATUS_FAIL;
	}
	else return STATUS_OK;
#else //DEVICECONNECTED
	return STATUS_OK;
#endif //DEVICECONNECTED
}

int SwitchPRecoupOne(LISE_ED& LiseEd)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if(LiseEd.Lise.T_SwitchPRecoupTInterne!=0)
		{
			int32 ErrorSwitch, written;
			uInt32 Switch = -1;
			ErrorSwitch = DAQmxWriteDigitalU32(LiseEd.Lise.T_SwitchPRecoupTInterne,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&Switch,&written,NULL);
			DisplayDAQmxError(LiseEd,ErrorSwitch,"Switch Puissance Recouplee Temperature Interne Error");
			if(ErrorSwitch == 0) return STATUS_OK; 
			return STATUS_FAIL;
		}
		return STATUS_FAIL;
	}
	return STATUS_OK;
#else //DEVICECONNECTED
	return STATUS_OK;
#endif //DEVICECONNECTED
}

int SwitchPRecoupZero(LISE_ED& LiseEd)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if(LiseEd.Lise.T_SwitchPRecoupTInterne!=0)
		{
			int32 ErrorSwitch, written;
			uInt32 Switch = -1;
			ErrorSwitch = DAQmxWriteDigitalU32(LiseEd.Lise.T_SwitchPRecoupTInterne,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&Switch,&written,NULL);
			DisplayDAQmxError(LiseEd,ErrorSwitch,"Switch Puissance Recouplee Temperature Interne Error");
			if(ErrorSwitch == 0) return STATUS_OK; 
			return STATUS_FAIL;
		}
		return STATUS_FAIL;
	}
	else return STATUS_OK;
#else //DEVICECONNECTED
	return STATUS_OK;
#endif //DEVICECONNECTED
}

int LRIsValid(LISE_ED& LiseEd)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if(LiseEd.Lise.T_AlarmLr!=0)
		{
			uInt32 AlarmLr;
			int32 read,ErrorAlarmLr;
			ErrorAlarmLr =  DAQmxReadDigitalU32(LiseEd.Lise.T_AlarmLr,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&AlarmLr,1,&read,NULL);	DisplayDAQmxError(LiseEd,ErrorAlarmLr,"Alarm Not Valid");
			if(AlarmLr == 0)	return STATUS_OK;
			else return STATUS_FAIL;
		}
		else return STATUS_FAIL;
	}
	else return STATUS_OK;
#else //DEVICECONNECTED
	return STATUS_OK;
#endif //DEVICECONNECTED
}
