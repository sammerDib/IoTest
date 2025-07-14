/*
 * $Id: LISE_ED_DLL_Create.cpp 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#include <windows.h>
#include <stdio.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
#include "../FD_FogaleProbe/FogaleProbeReturnValues.h"
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



// Configuration du système à partir d'un fichier
int CreateConfigSystem(LISE_ED& LiseEd,char* configPath, LISE_HCONFIG* HardwareConfig)
{
	//ConfigInitFromFile("Configuration_Systeme.txt");
	char temp[512];
	sprintf(temp, "%s", configPath);
	ConfigInitFromFile(LiseEd,temp);
    UpdateConfigFromHardwareConfig(LiseEd, HardwareConfig);
	return STATUS_OK;
}

// fonction de log pour le create de task NI
void LogInfoCreateNITask(LOGFILE& log,char* MessageLog,bool bDisplayInLogFile)
{
	if(bDisplayInLogFile)
	{
		LogfileF(log,MessageLog);
	}
}

// fonction pour la création de taches LiseEd
int CreateNITask(LISE_ED& LiseEd)
{
	int error = 1;
	int CountError = 0;

	// Clear de toutes les taches associées
	error = Clear_ED_EO_mx(LiseEd);
	
	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClear All NIDAQ Task - Fail",LiseEd.Lise.bDebug);CountError++;}
	else LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClear All NIDAQ Task - Success",LiseEd.Lise.bDebug);

	// Création du switch de puissance recoup
	error = CreateSwitchPRecoupTIntern(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Switch Puiss Recoup and Temperature Intern - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Switch Puiss Recoup and Temperature Intern - Success",LiseEd.Lise.bDebug);

	// Create de la ligne a retard
	error = CreateEnable_LR(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Enable LR - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Enable LR - Success",LiseEd.Lise.bDebug);

	// create du switch zero
	error = CreateSwitchZero(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Switch Zero - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Switch Zero - Success",LiseEd.Lise.bDebug);

	// Create alarme de la ligne à retard
	error = CreateAlarmLR(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Alarm LR - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Alarm LR - Success",LiseEd.Lise.bDebug);

	// Create de la source d'alarme
	error = CreateAlarmSource(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Alarm Source - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Digital Output Alarm Source - Success",LiseEd.Lise.bDebug);

	// Creation de la channel analog
	error = CreateAnalogChannel(LiseEd,LiseEd.Lise.UseTrigger,2*LiseEd.Lise.FrequencyHz);	// Use Trigger à initialiser grace au fichier de config

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation of 2 Inputs Analog Channel - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation of 2 Inputs Analog Channel - Success",LiseEd.Lise.bDebug);

	// Creation de la channel puissance
	error = CreatePuissanceChannel(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Puissance Channel - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Puissance Channel - Success",LiseEd.Lise.bDebug);

	// Creation de la source
	error = CreateSourceLevel(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Analog Output Source Level - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tCreation Analog Output Source Level - Success",LiseEd.Lise.bDebug);

	// error detected 
	if(CountError>0) return FP_TASK_CREATION_FAILED;

	return FP_OK;
}

// start de toutes les taches National Instrument
int StartNITask(LISE_ED& LiseEd)
{
	int error = 0;
	int CountError = 0;

	// Start de toutes les taches NiDAQmx
	error = StartSwitchPRecoupTIntern(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Switch Power Recoup - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Switch Power Recoup, Temperature Intern - Success",LiseEd.Lise.bDebug);

	// start enable alarme à retard
	error = StartEnable_LR(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Enable LR - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Enable LR - Success",LiseEd.Lise.bDebug);

	// start switch zero
	error = StartSwitchZero(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Switch Zero - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Switch Zero - Success",LiseEd.Lise.bDebug);

	// start alarme ligne à retard
	error = StartAlarmLR(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Alarm LR - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Alarm LR - Success",LiseEd.Lise.bDebug);

	// Start de l'alerme de source de la sonde
	error = StartAlarmSource(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Alarm Source - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Alarm Source - Success",LiseEd.Lise.bDebug);

	// start de la source
	error = StartSourceLevel(LiseEd);

	if(error!=1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Alarm Source - Fail",LiseEd.Lise.bDebug);CountError++;}
	else  LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tStart Task Alarm Source - Success",LiseEd.Lise.bDebug);

	// erreurs détectés
	if(CountError>0) return FP_TASK_START_FAILED;

	return FP_OK;
}

// Procédure qui permet de détruire toutes les taches et mettre les TaskHandle à zéro
int Clear_ED_EO_mx(LISE_ED& LiseEd)
{
// Arrête toutes les taches en cours, à vérifier les messages d'erreur lorsque l'on ferme une tache qui n'existe pas
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice Not Connected - Clear All Task");
	}
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_SwitchPRecoupTInterne);
		DAQmxClearTask(LiseEd.Lise.T_SwitchPRecoupTInterne);
		DAQmxStopTask(LiseEd.Lise.T_EnableLr);
		DAQmxClearTask(LiseEd.Lise.T_EnableLr);
		DAQmxStopTask(LiseEd.Lise.T_Switch0);
		DAQmxClearTask(LiseEd.Lise.T_Switch0);
		DAQmxStopTask(LiseEd.Lise.T_Switch1);
		DAQmxClearTask(LiseEd.Lise.T_Switch1);
		DAQmxStopTask(LiseEd.Lise.T_AlarmLr);
		DAQmxClearTask(LiseEd.Lise.T_AlarmLr);
		DAQmxStopTask(LiseEd.Lise.T_AlarmSource);
		DAQmxClearTask(LiseEd.Lise.T_AlarmSource);
		DAQmxStopTask(LiseEd.Lise.T_Trigger);
		DAQmxClearTask(LiseEd.Lise.T_Trigger);
		DAQmxStopTask(LiseEd.Lise.T_SplClk);
		DAQmxClearTask(LiseEd.Lise.T_SplClk);
		DAQmxStopTask(LiseEd.Lise.T_VoieAnalogIn);
		DAQmxClearTask(LiseEd.Lise.T_VoieAnalogIn);
		DAQmxStopTask(LiseEd.Lise.T_PuissanceREcouplee);
		DAQmxClearTask(LiseEd.Lise.T_PuissanceREcouplee);
		DAQmxStopTask(LiseEd.Lise.T_ControlSource1);
		DAQmxClearTask(LiseEd.Lise.T_ControlSource1);
		DAQmxStopTask(LiseEd.Lise.T_ControlSource2);
		DAQmxClearTask(LiseEd.Lise.T_ControlSource2);

	// On mes toutes les taches à Zero
		LiseEd.Lise.T_SwitchPRecoupTInterne = 0;
		LiseEd.Lise.T_EnableLr = 0;
		LiseEd.Lise.T_Switch0 = 0;
		LiseEd.Lise.T_Switch1 = 0;	
		LiseEd.Lise.T_AlarmLr = 0;
		LiseEd.Lise.T_AlarmSource = 0;
		LiseEd.Lise.T_Trigger = 0;
		LiseEd.Lise.T_SplClk = 0;
		LiseEd.Lise.T_VoieAnalogIn = 0;
		LiseEd.Lise.T_PuissanceREcouplee = 0;
		LiseEd.Lise.T_ControlSource1 = 0;
		LiseEd.Lise.T_ControlSource2 =0 ;
	}
	else
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice Not Connected - Clear All Task");
	}
#endif

	return STATUS_OK;
}

// fonction pour l'intialisation de la structure LiseEd
int InitLogFile(LISE_ED& LiseEd,LOGFILE* Log)
{
	// Utilisation du log externe au module LISE ED
	LiseEd.Lise.internalLog = false;
	if (Log)
	{
		LiseEd.Lise.Log = Log;
	}
	else
	{
		// La structure Log n'est pas définie
		// Ouverture d'un Log
		LiseEd.Lise.Log = (LOGFILE*)SPG_MemAlloc(sizeof(LOGFILE), "Log LISE ED");
		if (!LiseEd.Lise.Log)
		{
			return DLL_FAIL;
		}
		LogfileInit(*LiseEd.Lise.Log,LISEED_MODULE_NAME);
		LiseEd.Lise.internalLog = true;
	}

	return FP_OK;
}
int InitStructureLiseEd(LISE_ED& LiseEd)
{
	// 20080227 ATTENTION A VERIFIER.... On le met à l'init seulement car, fais planter si on le fait trop vite
	LiseEd.Lise.bStopDemande = false;
	LiseEd.Lise.bReadAllowed = false;
	LiseEd.Lise.bThreadActive = false;
	LiseEd.Lise.iHardwareTimeout = 0;
	LiseEd.Lise.DeviceNth = 2;
	LiseEd.Lise.FlagSavePics = false;
	LiseEd.Lise.FlagSaveSignal = false;
	LiseEd.Lise.FlagThickness = false;
	LiseEd.Lise.bDebugProcess = false;
	LiseEd.Lise.bReentrance = false;
	LiseEd.Lise.bNThicknessNotReady = false;
	LiseEd.Lise.bLimitedTime = false;
	LiseEd.Lise.iFirstMatchingSucces = false;
	// réentrance de la fonction GetRawSignal
	LiseEd.Lise.bGetRawSigReentrance = false;
	LiseEd.Lise.bGetThickness = false;
	// initialisation de la valeur de pulse de la dernière période
	LiseEd.PulsePlusWidthLastPeriod = 0;
	// initialisation de la variable de single shot acquisition
	LiseEd.Lise.AcqMode = SingleShot;
	LiseEd.IndiceCopieSignalEmule = 0;
	// Initialisation de définition de l'échantillon.
	LiseEd.Lise.sample.bDefined = false;
	sprintf(LiseEd.Lise.sample.sName, "");
	sprintf(LiseEd.Lise.sample.sSampleNumber, "");
	//for (int i=0; i<NB_PEAK_MAX; i++)
	//{
	//	LiseEd.Lise.sample.fIndex[i] = 0.0;
	//	LiseEd.Lise.sample.fIntensity[i] = 0.0;
	//}
	LiseEd.Lise.hThread = NULL;

	// on définit le mode d'acquisition comme stop
	LiseEd.AcquisitionMode = Stopped;
	LiseEd.DblEdMode = None;
	LiseEd.DblEdMeasurementMode = MeasSingleProbe;
	LiseEd.Lise.bSampleDefUpdated = false;

	return FP_OK;
}

int InitialisationVariablesBufferCirculaire(LISE_ED& LiseEd)
{
	// Initialisation des parametres de la structures
	LiseEd.Lise.FirstPass = true;
	LiseEd.Lise.iSampsPerChanToAcquire = 1000;
// Initialisation de l'indice Read Du Buffer circulaire
	LiseEd.Lise.Read.N = 0;
	LiseEd.Lise.Read.AbsN = 0;
	LiseEd.Lise.Read.Len = LiseEd.Lise.BufferLen;
// initialisation de l'indice de Cross Buffer
	LiseEd.Lise.ReadCrossBuffer.N = 0;
	LiseEd.Lise.ReadCrossBuffer.AbsN = 0;
	LiseEd.Lise.ReadCrossBuffer.Len = LiseEd.Lise.BufferLen;
// initialisation de l'indice Write Du buffer circualire
	LiseEd.Lise.Write.N = 0;
	LiseEd.Lise.Write.AbsN = 0;
	LiseEd.Lise.Write.Len = LiseEd.Lise.BufferLen;
// initialisation des indices circulaires
	LiseEd.PulseMoinsLeft.N = 0;
	LiseEd.PulseMoinsLeft.AbsN = 0;
	LiseEd.PulseMoinsLeft.Len = LiseEd.Lise.BufferLen;
	LiseEd.PulseMoinsRight.N = 0;
	LiseEd.PulseMoinsRight.AbsN = 0;
	LiseEd.PulseMoinsRight.Len = LiseEd.Lise.BufferLen;
	LiseEd.Lise.PulsePlusLeft.N = 0;
	LiseEd.Lise.PulsePlusLeft.AbsN = 0;
	LiseEd.Lise.PulsePlusLeft.Len = LiseEd.Lise.BufferLen;
	LiseEd.Lise.PulsePlusRight.N = 0;
	LiseEd.Lise.PulsePlusRight.AbsN = 0;
	LiseEd.Lise.PulsePlusRight.Len = LiseEd.Lise.BufferLen;
	LiseEd.PossiblePulseMoinsLeft.N = 0;
	LiseEd.PossiblePulseMoinsLeft.AbsN = 0;
	LiseEd.PossiblePulseMoinsLeft.Len = LiseEd.Lise.BufferLen;
	LiseEd.PossiblePulsePlusLeft.N = 0;
	LiseEd.PossiblePulsePlusLeft.AbsN = 0;
	LiseEd.PossiblePulsePlusLeft.Len = LiseEd.Lise.BufferLen;
	// Initialisation du Buffer de resultat
	LiseEd.Lise.WriteResult.N = 0;
	LiseEd.Lise.WriteResult.AbsN = 0;
	LiseEd.Lise.WriteResult.Len = LiseEd.Lise.PicResultLen;
	// Initialisation du buffer de resultat de la voie 2
	LiseEd.Lise.WriteResultSecondChannel.N = 0;
	LiseEd.Lise.WriteResultSecondChannel.AbsN = 0;
	LiseEd.Lise.WriteResultSecondChannel.Len = LiseEd.Lise.PicResultLen;
	// initialisation du trailer
	LiseEd.Lise.Trailer.N = 0;
	LiseEd.Lise.Trailer.AbsN = 0;
	LiseEd.Lise.Trailer.Len = LiseEd.Lise.BufferLen;
	LiseEd.Lise.SensPositif = false;

	// Variables de détection de pulse	
	LiseEd.bFirstPulseSample = true;
	LiseEd.bPulseDetection = false;
    LiseEd.cptSensDetect = 0;

	// 20080227 ATTENTION A VERIFIER.... On le met à l'init seulement car, fais planter si on le fait trop vite
//	LiseEd.Lise.bStopDemande = false;
//	LiseEd.Lise.bReadAllowed = false;
//	LiseEd.Lise.bThreadActive = false;
	LiseEd.Lise.bNeedRead = false;
	LiseEd.bPulse = false;


	LiseEd.Lise.Periode = 0;
	//LiseEd.Lise.IndiceDecimation = 0;
	LiseEd.Lise.CompteurDecimation = 0;

	LiseEd.Lise.Indice = 0;	// Compteur du nombre de pics dans une période
	LiseEd.Lise.iIndiceVoie2 = 0;
	LiseEd.NbrPics = 0;	// Nombre de pics du sens + dans une période

	LiseEd.Lise.IndicePeriod.AbsN = 0;
	LiseEd.Lise.IndicePeriod.N = 0;
	LiseEd.Lise.IndicePeriod.Len = LiseEd.Lise.ResultLen;

	LiseEd.Lise.NbSamplesLastPeriod = 0;
	LiseEd.Lise.iCounterMeasure = 0;

	LiseEd.Lise.iChannel = 1;

	return STATUS_OK;
}

int InitialisationBuffer(LISE_ED& LiseEd)
{
	// on initialise les différentes structures à zéro
	SPG_Memset(LiseEd.Lise.BufferIntensity,0,sizeof(double) * LiseEd.Lise.BufferLen);
	for (int i = 0;i<MAXVOIE;i++)
	{
		SPG_Memset(LiseEd.Lise.BufferResultat[i],0,sizeof(PICRESULT) * LiseEd.Lise.PicResultLen); 
	}
	SPG_Memset(LiseEd.Lise.Resultats,0, sizeof(PERIOD_RESULT) * LiseEd.Lise.ResultLen);

	return STATUS_OK;
}

// Entrée digitale pour récupérer la valeur du switch permettant de passer de la Puissance recouplée à la temperature Interne
int CreateSwitchPRecoupTIntern(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LiseED]\tDevice not connected - Create Switch Puissance Recouplée, Temperature Interne");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxCreateTask("Task Switch Puissance Recoup Temperature Intern",&LiseEd.Lise.T_SwitchPRecoupTInterne);	
		DisplayDAQmxError(LiseEd,error,"CreateSwitchPRecoupRIntern Error");
		error = DAQmxCreateDOChan (LiseEd.Lise.T_SwitchPRecoupTInterne,LiseEd.Lise.SwitchPRecoupTInterne,"",DAQmx_Val_ChanPerLine);	DisplayDAQmxError(LiseEd,error,"CreateSwitchPRecoupRIntern Error");
		
		if(error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Switch Puissance Recouplée, Temperature Interne");
		}
		return STATUS_OK;
	}
#endif
}

// Demmarage de la tache et on force la sortie à Zero
int StartSwitchPRecoupTIntern(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Puissance Recouplée, Temperature Intern");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxStartTask(LiseEd.Lise.T_SwitchPRecoupTInterne);	DisplayDAQmxError(LiseEd,error,"CreateSwitchPRecoupRIntern Error");
		uInt32 ValeurForcage = 0;
		int32 written;
		error = DAQmxWriteDigitalU32(LiseEd.Lise.T_SwitchPRecoupTInterne,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&ValeurForcage,&written,NULL);	DisplayDAQmxError(LiseEd,error,"CreateSwitchPRecoupRIntern Error");
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Puissance Recouplée, Temperature Intern");
		}
		return STATUS_OK;
	}
#endif
}

// Creation d'une entrée digitale pour la ligne à retard.
int CreateEnable_LR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Enable Delay Line");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;

		error = DAQmxCreateTask("Task Enable Ligne Retard",&LiseEd.Lise.T_EnableLr);	DisplayDAQmxError(LiseEd,error,"CreateEnable_LR Error");
		error = DAQmxCreateDOChan (LiseEd.Lise.T_EnableLr,LiseEd.Lise.EnableLr,"",DAQmx_Val_ChanPerLine);	DisplayDAQmxError(LiseEd,error,"CreateEnable_LR Error");

		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Enable Delay Line");
		}
		return STATUS_OK;
	}
#endif
}

int StartEnable_LR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Enable Delay Line");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int error;
		error = DAQmxStartTask(LiseEd.Lise.T_EnableLr);	DisplayDAQmxError(LiseEd,error,"CreateEnable_LR Error");

		uInt32 ValeurForcage = 0;
		int32 written;
		error = DAQmxWriteDigitalU32(LiseEd.Lise.T_EnableLr,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&ValeurForcage,&written,NULL);
		// Tempo de 6 secondes avant de permettre une mesure
		if (error == 0) {/*Sleep(6000)*/; return STATUS_OK;}
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Enable Delay Line");
		}
		return STATUS_OK;	
	}
#endif
}

int CreateSwitchZero(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Switch Zero");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxCreateTask("Task Switch Zero",&LiseEd.Lise.T_Switch0);	DisplayDAQmxError(LiseEd,error,"CreateSwitchZero");
		error = DAQmxCreateTask("Task Switch One",&LiseEd.Lise.T_Switch1);	DisplayDAQmxError(LiseEd,error,"CreateSwitchUn");

		error = DAQmxCreateDOChan (LiseEd.Lise.T_Switch0, LiseEd.Lise.Switch0,"", NULL);	DisplayDAQmxError(LiseEd,error,"CreateSwitchZero");
		error = DAQmxCreateDOChan (LiseEd.Lise.T_Switch1, LiseEd.Lise.Switch1,"", NULL);	DisplayDAQmxError(LiseEd,error,"CreateSwitchUn");

		if(error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Switch Zero");
		}
		return STATUS_OK;
	}
#endif
}

int StartSwitchZero(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Switch Zero");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int error;

		error =	DAQmxStartTask(LiseEd.Lise.T_Switch0);	DisplayDAQmxError(LiseEd,error,"CreateSwitchZero");
		error =	DAQmxStartTask(LiseEd.Lise.T_Switch1);	DisplayDAQmxError(LiseEd,error,"CreateSwitchUn");

		// On force toutes les valeurs de sortie à zéro
		uInt32 ValeurForcage = 0;
		int32 written;
		error = DAQmxWriteDigitalU32(LiseEd.Lise.T_Switch0,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&ValeurForcage,&written,NULL);
		error = DAQmxWriteDigitalU32(LiseEd.Lise.T_Switch1,1,1,LiseEd.Lise.Timeout,DAQmx_Val_GroupByChannel,&ValeurForcage,&written,NULL);

		if(error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Switch Zero");
		}
		return STATUS_OK;
	}
#endif
}

int CreateAlarmLR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Alarme Ligne à Retard");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxCreateTask("Task Alarm Ligne Retard",&LiseEd.Lise.T_AlarmLr);	DisplayDAQmxError(LiseEd,error,"CreateAlarm_LR Error");
		error = DAQmxCreateDOChan (LiseEd.Lise.T_AlarmLr,LiseEd.Lise.AlarmLr,"",NULL);	DisplayDAQmxError(LiseEd,error,"CreateAlarm_LR Error");
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Alarme Ligne à Retard");
		}
		return STATUS_OK;
	}
#endif
}

int StartAlarmLR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Alarme Ligne à Retard");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		//error = DAQmxRegisterSignalEvent(LiseEd.Lise.T_AlarmLr,DAQmx_Val_ChangeDetectionEvent,DAQmx_Val_SynchronousEventCallbacks,AlarmDetection,NULL);DisplayDAQmxError(LiseEd,error,"CreateAlarm_LR Error");
		error = DAQmxStartTask(LiseEd.Lise.T_AlarmLr);	DisplayDAQmxError(LiseEd,error,"CreateAlarm_LR Error");DisplayDAQmxError(LiseEd,error,"CreateAlarm_LR Error");
	// Il faudrait créer une CALLBACK pour arreter le moteur en cas d'alarme
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Alarme Ligne à Retard");
		}
		return STATUS_OK;
	}
#endif
}

int CreateAlarmSource(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Alarm Source");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxCreateTask("Task Alarm Source",&LiseEd.Lise.T_AlarmSource);	DisplayDAQmxError(LiseEd,error,"CreateAlarmSource Error");
		error = DAQmxCreateDIChan (LiseEd.Lise.T_AlarmSource,LiseEd.Lise.AlarmSource,"",DAQmx_Val_ChanPerLine);	DisplayDAQmxError(LiseEd,error,"CreateAlarmSource Error");
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Alarm Source");
		}
		return STATUS_OK;
	}
#endif
}

int StartAlarmSource(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Alarm Source");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxStartTask(LiseEd.Lise.T_AlarmSource);	DisplayDAQmxError(LiseEd,error,"StartAlarmSource Error");
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Alarm Source");
		}
		return STATUS_OK;
	}
#endif
}
/*
// Attention, la création de la Callback a été testé et provoque une erreur NIDAQ 
// Callback pour recuperation d'échantillons
int32 CVICALLBACK AlarmeDetection(TaskHandle taskHandle, int32 everyNsamplesEventType, uInt32 nSamples, void *callbackData)
{
	if(taskHandle!=0)
	{
		int r=AcqAndProcess();
		return r;
	}
	else return STATUS_FAIL;
}*/

#ifndef NOHARDWARE
// Callback pour la surveillance de l'alarme
int32 CVICALLBACK AlarmDetection(TaskHandle taskHandle, int32 status, void *callbackData)
{
	if(Global.EnableList>=1) MessageBox(0,L"Dalay Line Error",L"Alarme Error",0);
	//if(LiseEd.Lise.bDebug == true)
	//{
		//LogfileF(LiseEd.Lise.Log,"{[LiseEd.Lise ED]\tAlarm Detection"); 
	//}
	return STATUS_OK;
}
#endif

int CreateAnalogChannel(LISE_ED& LiseEd,int UseTrigger, int SampleCount)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Analog Input Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxCreateTask("Task Analog Channel Signal",&LiseEd.Lise.T_VoieAnalogIn); DisplayDAQmxError(LiseEd,error,"CreateAnalogChannel Error");
		
		for(int i=0;i<LiseEd.Lise.NombredeVoie;i++)
		{
			error = DAQmxCreateAIVoltageChan(LiseEd.Lise.T_VoieAnalogIn,LiseEd.Lise.VoieAnalogIn[i],"", DAQmx_Val_Cfg_Default, LiseEd.Lise.ValMin, LiseEd.Lise.ValMax, DAQmx_Val_Volts, "");
			DisplayDAQmxError(LiseEd,error,"LISE_ED_Create");
		}

		//// on associe la voie 1 à cette tache
		//error = DAQmxCreateAIVoltageChan(LiseEd.Lise.T_VoieAnalogIn,LiseEd.Lise.VoieAnalogIn[0],"", DAQmx_Val_Cfg_Default, LiseEd.Lise.ValMin, LiseEd.Lise.ValMax, DAQmx_Val_Volts, "");
		//// On associe egalement la voie 2 a cette tache
		//if(LiseEd.Lise.NombredeVoie == 2)
		//{
		//	error = DAQmxCreateAIVoltageChan(LiseEd.Lise.T_VoieAnalogIn,LiseEd.Lise.VoieAnalogIn[1],"", DAQmx_Val_Cfg_Default, LiseEd.Lise.ValMin, LiseEd.Lise.ValMax, DAQmx_Val_Volts, "");
		//}

		error =	DAQmxCfgSampClkTiming (LiseEd.Lise.T_VoieAnalogIn,LiseEd.Lise.SplClk,SampleCount,DAQmx_Val_Rising,DAQmx_Val_ContSamps,LiseEd.Lise.iSampsPerChanToAcquire);

		if(UseTrigger != 0)	
		{
			error = DAQmxCfgDigEdgeStartTrig (LiseEd.Lise.T_VoieAnalogIn,LiseEd.Lise.Trigger,DAQmx_Val_Rising);	DisplayDAQmxError(LiseEd,error,"CreateAnalogChannel Error");
		}
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Analog Input Channel");
		}
		return STATUS_OK;
	}
#endif
}

int StartAnalogChannel(LISE_ED& LiseEd)
{
	InitialisationVariablesBufferCirculaire(LiseEd);
	InitialisationBuffer(LiseEd);
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - start Task Analog Input Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxStartTask(LiseEd.Lise.T_VoieAnalogIn);
		DisplayDAQmxError(LiseEd,error,"Start analog channel Error");
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - start Task Analog Input Channel");
		}
		return STATUS_OK;
	}
#endif
}

int CreatePuissanceChannel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Analog Output Puissance Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxCreateTask("Task Puissance Channel",&LiseEd.Lise.T_PuissanceREcouplee); DisplayDAQmxError(LiseEd,error,"CreateAnalogChannel Error");
		error = DAQmxCreateAIVoltageChan(LiseEd.Lise.T_PuissanceREcouplee,LiseEd.Lise.PuissanceREcouplee,"", DAQmx_Val_Cfg_Default, LiseEd.Lise.ValMin, LiseEd.Lise.ValMax, DAQmx_Val_Volts, "");
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Analog Output Puissance Channel");
		}
		return STATUS_OK;
	}
#endif
}

int StartPuissanceChannel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Analog Input Puissance Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxStartTask(LiseEd.Lise.T_PuissanceREcouplee);	DisplayDAQmxError(LiseEd,error,"StartPuissanceChannel Error");
		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Analog Input Puissance Channel");
		}
		return STATUS_OK;
	}
#endif
}

int CreateSourceLevel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Analog Output Source Level");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxCreateTask("Task Source Level Channel One",&LiseEd.Lise.T_ControlSource1);
		error = DAQmxCreateTask("Task Source Level Channel Two",&LiseEd.Lise.T_ControlSource2);
		error = DAQmxCreateAOVoltageChan(LiseEd.Lise.T_ControlSource1,LiseEd.Lise.ControlSource1,"", LiseEd.Lise.ValMin, LiseEd.Lise.ValMax, DAQmx_Val_Volts,"");
		error = DAQmxCreateAOVoltageChan(LiseEd.Lise.T_ControlSource2,LiseEd.Lise.ControlSource2,"", LiseEd.Lise.ValMin, LiseEd.Lise.ValMax, DAQmx_Val_Volts,"");

		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Create Analog Output Source Level");
		}
		return STATUS_OK;
	}
#endif
}

int StartSourceLevel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Analog Output Source Level");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxStartTask(LiseEd.Lise.T_ControlSource1);
		error = DAQmxStartTask(LiseEd.Lise.T_ControlSource2);
		// On force la valeur des sorties à un
		int32 ForcingValue = 0;
		DAQmxWriteAnalogScalarF64 (LiseEd.Lise.T_ControlSource1, 1, LiseEd.Lise.Timeout , ForcingValue, NULL);
		DAQmxWriteAnalogScalarF64 (LiseEd.Lise.T_ControlSource2, 1, LiseEd.Lise.Timeout , ForcingValue, NULL);

		if (error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Start Task Analog Output Source Level");
		}
		return STATUS_OK;
	}
#endif
}


// ## Functions to close and destroy all task National Instrument

int CloseAllEdTaskNI(LISE_ED& LiseEd)
{
	int error;
	int compteur = 0;

	// debut de la fermeture des tasks
	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tLEDIClose - Start",LiseEd.Lise.bDebug);

	error = CloseSwitchPRecoupTIntern(LiseEd);	
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Switch Puiss Recouplee, Temperature Interne - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Switch Puiss Recouplee, Temperature Interne - Success",LiseEd.Lise.bDebug);

	error = CloseEnable_LR(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Enable LR - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Enable LR - Success",LiseEd.Lise.bDebug);

	error = CloseSwitchZero(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Switch Zero - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Switch Zero - Success",LiseEd.Lise.bDebug);

	error = CloseAlarmLR(LiseEd);
	if(error != 1) {LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm LR - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm LR - Success",LiseEd.Lise.bDebug);

	error = CloseAlarmSource(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm Source - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm Source - Success",LiseEd.Lise.bDebug);

	error = ClosePuissanceChannel(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Puissance Channel - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Puissance Channel - Success",LiseEd.Lise.bDebug);

	error = CloseSourceLevel(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Output Source Level - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Output Source Level - Success",LiseEd.Lise.bDebug);

	if(compteur >0)
	{
		return FP_CLOSE_NI_TASK_FAIL;
	}

	return FP_OK;
}

int DestroyAllEdTaskNI(LISE_ED& LiseEd)
{
	int	compteur = 0;

	int error = DestroySwitchPRecoupTIntern(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Lise ED Switch Puiss Recoup, Temperature Intern - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Lise ED Switch Puiss Recoup, Temperature Intern - Success",LiseEd.Lise.bDebug);

	error = DestroyEnable_LR(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Lise ED Destroy Enable LR - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Lise ED Destroy Enable LR - Success",LiseEd.Lise.bDebug);

	error = DestroySwitchZero(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose - Lise ED Switch Zero - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose - Lise ED Switch Zero - Success",LiseEd.Lise.bDebug);

	error = DestroyAlarmLR(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm LR - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm LR - Success",LiseEd.Lise.bDebug);

	error = DestroyAlarmSource(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm Source - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Alarm Source - Success",LiseEd.Lise.bDebug);

	error = CloseAnalogChannel(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Input Channel- Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Input Channel - Success",LiseEd.Lise.bDebug);

	error = DestroyAnalogChannel(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log, "[LISEED]\tClose Analog Input Channel - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Input Channel - Success",LiseEd.Lise.bDebug);

	error = DestroyPuissanceChannel(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log, "[LISEED]\tClose Analog Input Puissance Channel - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Input Puissance Channel - Success",LiseEd.Lise.bDebug);

	error = DestroySourceLevel(LiseEd);
	if(error != 1) 	{LogInfoCreateNITask(*LiseEd.Lise.Log, "[LISEED]\tClose Analog Output Source Level - Fail",LiseEd.Lise.bDebug);compteur++;}
	else 	LogInfoCreateNITask(*LiseEd.Lise.Log,"[LISEED]\tClose Analog Output Source Level- Success",LiseEd.Lise.bDebug);

	if(compteur >0)
	{
		return FP_DESTROY_NI_TASK_FAIL;
	}

	return FP_OK;
}

int CloseSwitchPRecoupTIntern(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Switch Puissance Recouplee, Temperature Interne");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_SwitchPRecoupTInterne);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Switch Puissance Recouplee, Temperature Interne");
		}
		return STATUS_OK;
	}
#endif
}

int DestroySwitchPRecoupTIntern(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Switch Puissance Recouplee Temperature Interne");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_SwitchPRecoupTInterne);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Switch Puissance Recouplee Temperature Interne");
		}
		return STATUS_OK;
	}
#endif
}

// close d'une entrée digitale pour la ligne à retard.
int CloseEnable_LR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Enable Delay Line");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_EnableLr);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Enable Delay Line");
		}
		return STATUS_OK;
	}
#endif
}

int DestroyEnable_LR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task enable Delay Line");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_EnableLr);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task enable Delay Line");
		}
		return STATUS_OK;
	}
#endif
}

int CloseSwitchZero(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Slose Task Switch Zero");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_Switch0);
		DAQmxStopTask(LiseEd.Lise.T_Switch1);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Slose Task Switch Zero");
		}
		return STATUS_OK;
	}
#endif
}

int DestroySwitchZero(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Switch Zero");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_Switch0);
		DAQmxClearTask(LiseEd.Lise.T_Switch1);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Switch Zero");
		}
		return STATUS_OK;
	}
#endif
}
// ici a changer les lignes de textes
int CloseAlarmLR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Delay Line");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_AlarmLr);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Delay Line");
		}
		return STATUS_OK;
	}
#endif
}

int DestroyAlarmLR(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Delay Line");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_AlarmLr);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Delay Line");
		}
		return STATUS_OK;
	}
#endif
}

int CloseAlarmSource(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Alarm Source");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_AlarmSource);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Alarm Source");
		}
		return STATUS_OK;
	}
#endif
}

int DestroyAlarmSource(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Alarm Source");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_AlarmSource);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Alarm Source");
		}
		return STATUS_OK;
	}
#endif
}

int CloseAnalogChannel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Analog Input Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_VoieAnalogIn);
	//	DAQmxClearTask(LiseEd.Lise.T_VoieAnalogIn);
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Analog Input Channel");
		}
		return STATUS_OK;
	}
	return STATUS_OK;
#endif
}

int DestroyAnalogChannel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Analog Input Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_VoieAnalogIn);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Analog Input Channel");
		}
		return STATUS_OK;
	}
#endif
}

int ClosePuissanceChannel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Analog Input Puissance Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_PuissanceREcouplee);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Analog Input Puissance Channel");
		}
		return STATUS_OK;
	}
#endif
}

int DestroyPuissanceChannel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Analog Input Power Channel");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_PuissanceREcouplee);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Clear Task Analog Input Power Channel");
		}
		return STATUS_OK;
	}
#endif
}

int CloseSourceLevel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Analog Output Source Level");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxStopTask(LiseEd.Lise.T_ControlSource1);
		DAQmxStopTask(LiseEd.Lise.T_ControlSource2);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task Analog Output Source Level");
		}
		return STATUS_OK;
	}
#endif
}

int DestroySourceLevel(LISE_ED& LiseEd)
{
#ifndef DEVICECONNECTED
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task analog Output Source Level");
	}
	return STATUS_OK;
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		DAQmxClearTask(LiseEd.Lise.T_ControlSource1);
		DAQmxClearTask(LiseEd.Lise.T_ControlSource2);
		return STATUS_OK;
	}
	else
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tDevice not connected - Close Task analog Output Source Level");
		}
		return STATUS_OK;
	}
#endif
}
