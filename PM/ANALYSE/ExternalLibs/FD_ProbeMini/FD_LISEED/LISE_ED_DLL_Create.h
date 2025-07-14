/*
 * $Id: LISE_ED_DLL_Create.h 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#ifndef LISE_ED_DLL_CREATE_H
#define LISE_ED_DLL_CREATE_H

#include "LISE_ED_DLL_Internal.h"


// ## function to create and init params from config file
int CreateConfigSystem(LISE_ED& LiseEd,char* configPath, LISE_HCONFIG* HardwareConfig);

// ## Fonction for initialisation of task variable
int InitLogFile(LISE_ED& LiseEd,LOGFILE* Log);
int InitStructureLiseEd(LISE_ED& LiseEd);
int InitialisationVariablesBufferCirculaire(LISE_ED& LiseEd);
int InitialisationBuffer(LISE_ED& LiseEd);
int Clear_ED_EO_mx(LISE_ED& LiseEd);


// ## Functions for creation of all NI task
int CreateNITask(LISE_ED& LiseEd);
int CreateSwitchPRecoupTIntern(LISE_ED& LiseEd);
int CreateEnable_LR(LISE_ED& LiseEd);
int CreateSwitchZero(LISE_ED& LiseEd);
int CreateAlarmLR(LISE_ED& LiseEd);
int CreateAlarmSource(LISE_ED& LiseEd);
int CreateAnalogChannel(LISE_ED& LiseEd,int UseTrigger, int SampleCount);
int CreatePuissanceChannel(LISE_ED& LiseEd);
int CreateSourceLevel(LISE_ED& LiseEd);


// ## functions for starting NI task
int StartNITask(LISE_ED& LiseEd);
int StartSwitchPRecoupTIntern(LISE_ED& LiseEd);
int StartEnable_LR(LISE_ED& LiseEd);
int StartSwitchZero(LISE_ED& LiseEd);
int StartAlarmLR(LISE_ED& LiseEd);
int StartAlarmSource(LISE_ED& LiseEd);
int StartAnalogChannel(LISE_ED& LiseEd);
int StartPuissanceChannel(LISE_ED& LiseEd);
int StartSourceLevel(LISE_ED& LiseEd);

#ifdef DEVICECONNECTED
#ifndef NOHARDWARE
int32 CVICALLBACK AlarmDetection(TaskHandle taskHandle, int32 status, void *callbackData);
#endif
#endif

// ## functions to close all NI Task from Probe Ed
int CloseAllEdTaskNI(LISE_ED& LiseEd);

int CloseSwitchPRecoupTIntern(LISE_ED& LiseEd);
int CloseEnable_LR(LISE_ED& LiseEd);
int CloseSwitchZero(LISE_ED& LiseEd);
int CloseAlarmLR(LISE_ED& LiseEd);
int CloseAlarmSource(LISE_ED& LiseEd);
int CloseAnalogChannel(LISE_ED& LiseEd);
int ClosePuissanceChannel(LISE_ED& LiseEd);
int CloseSourceLevel(LISE_ED& LiseEd);

// ## functions to destroy all NI Task 
int DestroyAllEdTaskNI(LISE_ED& LiseEd);

int DestroySwitchPRecoupTIntern(LISE_ED& LiseEd);
int DestroyEnable_LR(LISE_ED& LiseEd);
int DestroySwitchZero(LISE_ED& LiseEd);
int DestroyAlarmLR(LISE_ED& LiseEd);
int DestroyAlarmSource(LISE_ED& LiseEd);
int DestroyAnalogChannel(LISE_ED& LiseEd);
int DestroyPuissanceChannel(LISE_ED& LiseEd);
int DestroySourceLevel(LISE_ED& LiseEd);

#endif