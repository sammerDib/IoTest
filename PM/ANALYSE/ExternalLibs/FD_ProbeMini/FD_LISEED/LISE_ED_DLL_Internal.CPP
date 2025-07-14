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

#include "..\FD_FogaleProbe\FogaleProbeReturnValues.h"
#include "..\FD_FogaleProbe\FogaleProbeParamID.h"

#include <time.h>

int LEDIGetVersion()
{
	return LISE_ED_DLL_VERSION;
}

int LEDISetParam(void* s,void* Param, int ParamID)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	LISE_ED* LiseEd = (LISE_ED*)s;

	if(ParamID == FPID_C_SAVESIGNAL)
	{
		// définition de la variable de longueur d'onde
		float RefWaveLength = 0.0;

		// paramètre pour sauvegarder la waveform
		LEDISaveWaveForm(s,(char*)Param,&RefWaveLength);
	}
	else if(ParamID == FPID_B_AUTOGAIN)
	{
		bool* bAutoGain = (bool*)Param;
		LiseEd->bUseAutoGain = *bAutoGain;
		// Si le mode de la probe est continu ou arrété, on procède au gain auto
		if((LiseEd->Lise.AcqMode == Continous)&&(LiseEd->bUseAutoGain == true))
		{
			return ProcessAutoGain_Continuous_Mode(LiseEd);
		}
	}
	else if(ParamID == FPID_B_AUTOAIRGAPCONFIG)
	{
		bool* bAAG = (bool*)Param;
		LiseEd->Lise.bUseAirGapAuto = *bAAG;
	}
	else if(ParamID == FPID_B_LIMITEDTIME_MODE)
	{
		bool* bLimited = (bool*)Param;
		LiseEd->Lise.bLimitedTime = *bLimited;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM){

		int* iAverage = (int*)Param;
		LiseEd->Lise.Moyenne = *iAverage;
	}
	else if(ParamID == FPID_D_COMPARISON_TOLERANCE)
	{
		LiseEd->Lise.ComparisonTolerance = *(double*)Param;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN)
	{
		LiseEd->Lise.GainMax = *(double*)Param;
	}
	else if(ParamID == FPID_D_SATURATION_THRESHOLD)
	{
		LiseEd->Lise.fThresholdSaturation = *(double*)Param;
	}
	else if(ParamID == FPID_D_WAVE_LENGTH)
	{
		double d_Wl = (double)(1530.0f + *(double*)Param);

		LiseEd->Lise.dRefWaveLengthNm = d_Wl;
	}
	else if(ParamID == FPID_C_NI_DEVICE)
	{
		strcpy(LiseEd->Lise.NIDevice,(char*)Param);
	}
	else if(ParamID == FPID_C_SERIAL)
	{
		strcpy(LiseEd->Lise.SerialNumber,(char*)Param);
	}
	else if(ParamID == FPID_C_TYPE)
	{
		strcpy(LiseEd->Lise.TypeDevice,(char*)Param);
	}
	else if(ParamID == FPID_C_PN)
	{
		strcpy(LiseEd->Lise.PartNumber,(char*)Param);
	}
	else if(ParamID == FPID_C_VERSION)
	{
		strcpy(LiseEd->Lise.HardwareVersion,(char*)Param);
	}
	else if(ParamID == FPID_S_SAVE_CURRENT_PARAMS)
	{
		return LEDISaveConfig(s,LiseEd->ConfigPath,(char*)Param);
	}
	else if(ParamID == FPID_D_AIRGAP_THRESHOLD || ParamID == FPID_D_AIRGAP_UPPER_THRESHOLD)
	{
		double _dTempVar = *(double*)Param;
		LiseEd->Lise.fThresholAmplitudeAirgap = (float)_dTempVar;
	}
	else if(ParamID == FPID_B_STANDBY)
	{
		bool* bStandby = (bool*)Param; 

		if (LiseEd->bEnableStandBy)
		{
			if(*bStandby)
			{
				// On passe en veille
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tSet Gain value to 0");
				// BackUp
				LiseEd->BackUpSourceValue = LiseEd->Lise.fSourceValue;
				LiseEd->Lise.fSourceValue = 0;
				LEDISetSourcePower(LiseEd,LiseEd->Lise.fSourceValue);
			}
			else
			{
				if(LiseEd->BackUpSourceValue < 0)
				{
					LiseEd->BackUpSourceValue = 0;
					LogfileF(*LiseEd->Lise.Log, "[LISEED]\tERROR Lise gain value has not been saved");
				}

				// On sort de veille
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tSet LISE Gain value to %f",LiseEd->BackUpSourceValue);
				LiseEd->Lise.fSourceValue = LiseEd->BackUpSourceValue;
				LEDISetSourcePower(LiseEd,LiseEd->Lise.fSourceValue);

				// -1 signifie que nous ne sommes pas en standby
				LiseEd->BackUpSourceValue = -1;
			}
		}
		else
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tEnable stand-by not activated.");
		}
	}
	else
	{
		return FP_ERROR_PARAM_ID;
	}

	return DLL_OK;
}
int LEDIGetParam(void* s,void* Param, int ParamID)
{
	// on récupère la structure
	LISE_ED* LiseEd= (LISE_ED*)s;

	if(ParamID == FPID_D_COUPLEDPOWER)
	{
		// on va lire la valeur de la puissance recouplée
		double Power = 0.0;
		//GetLightFeedback(S.Light,Power);
		LEDIReadPower(s,&Power);

		*(double*)Param=Power;
		return DLL_OK;
	}
	else if(ParamID == FPID_I_NUMCHANNEL)
	{ // pour retourner le nombre de channel définit dans la config du ED
		
		// on récupère la structure Lise Ed
		LISE_ED* LiseEd= (LISE_ED*)s;

		// on retourne le nombre de voie
		*(int*)Param = LiseEd->Lise.NombredeVoie;

		// retour OK vers la dll
		return DLL_OK;
	}
	else if(ParamID == FPID_D_AMPLITUDEPEAKS)
	{
		for(int x=0;x<NB_PEAK_MAX;x++) 
		{ 
			double &X=*((double*)Param+x);
			//X=x;
			X=LiseEd->Lise.LastAmplitudePics[x];
		}
	}
	else if(ParamID == FPID_I_TOTAL_PROBE_USED)
	{
		*(int*)Param = 1;
	}
	else if(ParamID == FPID_B_AUTOGAIN)
	{
		*(bool*)Param = LiseEd->bUseAutoGain;
	}
	else if(ParamID == FPID_B_AUTOAIRGAPCONFIG)
	{
		// on retourne la probe courante
		*(bool*)Param = LiseEd->Lise.bUseAirGapAuto;
	}
	else if(ParamID == FPID_D_CURSOURCEPOWER){

		double Power = LiseEd->Lise.fSourceValue;

		*(double*)Param=Power;
		return DLL_OK;
	}
	else if(ParamID == FPID_I_AVERAGEPARAM){

		int AverageP = LiseEd->Lise.Moyenne;

		*(int*)Param = AverageP;
		return DLL_OK;
	}
	else if(ParamID == FPID_B_LIMITEDTIME_MODE)
	{
		// on retourne la probe courante
		*(bool*)Param = LiseEd->Lise.bLimitedTime;
	}
	else if(ParamID == FPID_D_COMPARISON_TOLERANCE)
	{
		*(double*)Param = (double)LiseEd->Lise.ComparisonTolerance;
	}
	else if(ParamID == FPID_D_SINGLESHOTDURATION)
	{
		*(double*)Param = (double)LiseEd->Lise.Moyenne / (double)LiseEd->Lise.Frequency ;
	}
	else if(ParamID == FPID_D_PROBE_FREQUENCY)
	{
		*(double*)Param = LiseEd->Lise.Frequency;
	}
	else if(ParamID == FPID_D_MAXIMUM_GAIN)
	{
		*(double*)Param = LiseEd->Lise.GainMax;
	}
	else if(ParamID == FPID_D_SATURATION_THRESHOLD)
	{
		*(double*)Param = LiseEd->Lise.fThresholdSaturation;
	}
	else if(ParamID == FPID_D_WAVE_LENGTH)
	{
		*(double*)Param = (double)(LiseEd->Lise.dRefWaveLengthNm - 1530.0f);
	}
	else if(ParamID == FPID_C_NI_DEVICE)
	{
		strcpy((char*)Param,LiseEd->Lise.NIDevice);
	}
	else if(ParamID == FPID_C_SERIAL)
	{
		strcpy((char*)Param,LiseEd->Lise.SerialNumber);
	}
	else if(ParamID == FPID_C_TYPE)
	{
		strcpy((char*)Param,LiseEd->Lise.TypeDevice);
	}
	else if(ParamID == FPID_C_PN)
	{
		strcpy((char*)Param,LiseEd->Lise.PartNumber);
	}
	else if(ParamID == FPID_C_VERSION)
	{
		strcpy((char*)Param,LiseEd->Lise.HardwareVersion);
	}
	else if(ParamID == FPID_B_GET_PROBE_MEASURING_STATE)
	{
		if(LiseEd->Lise.IndicePeriod.AbsN <= 20)
		{
			*(bool*)Param = false;		
		}
		else{
			*(bool*)Param = true;
		}

		if(!LiseEd->bLiseEDConnected)
			*(bool*)Param = true;
	}
	else if(ParamID == FPID_D_AIRGAP_THRESHOLD || ParamID == FPID_D_AIRGAP_UPPER_THRESHOLD)
	{
		*(double*)Param = LiseEd->Lise.fThresholAmplitudeAirgap;
	}
	else if(ParamID == FPID_D_BASE_LINE)
	{
		*(double*)Param = LiseEd->Lise.LigneDeBase;
	}
	else if(ParamID == FPID_B_STANDBY)
	{
		// todo
		*(bool*)Param = false;
		
		if (LiseEd->bEnableStandBy)
		{
			if(LiseEd->Lise.fSourceValue < 0) 
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
		return FP_INVALIDPARAM;
	}

	return DLL_OK;
}
// Returns system Information
int LEDIGetSystemCaps(void* s,char* Type,char* SerialNumber,double& Range,int& Frequency,double& GainMin, double& GainMax, double& GainStep)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	LISE_ED* LiseEd= (LISE_ED*)s;

	if( Type == NULL || SerialNumber == NULL)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tInvalid Parameter Type or SerialNumber in prototype function GetSystemCaps");
		}
		return DLL_FAIL;
	}

	strcpy(Type,LiseEd->Lise.TypeDevice);
	strcpy(SerialNumber,LiseEd->Lise.SerialNumber);
	Range = LiseEd->Lise.Range;
	Frequency = LiseEd->Lise.Frequency;
	GainMin = LiseEd->Lise.GainMin;
	GainMax = LiseEd->Lise.GainMax;
	GainStep = LiseEd->Lise.GainStep;

	return DLL_OK;
}

// System Initialisation
int LEDIInit(void* s,char* configPath, LISE_HCONFIG* liseHConfig, LOGFILE* Log, double Param1, double Param2, double Param3)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	LISE_ED* LiseEd = (LISE_ED*)s;
	// initialisation du fichier de log
	InitLogFile(*LiseEd,Log);

	// précondition, le fichier ne doit pas être null
	if(!configPath)
	{
		LogLEDInfo(*LiseEd->Lise.Log,"[LiseEd]\tInvalid ConfigPath In LiseEdInit",LiseEd->Lise.bDebug);
		return DLL_FAIL;
	}
	LogfileF(*LiseEd->Lise.Log,"[LISEED]\t----------- Start LISE ED DLL ----------");

	GetNidaqmxVersion(*LiseEd);

	// Appel du fichier de config
	int error = CreateConfigSystem(*LiseEd,configPath, liseHConfig);

	// initialisation des variables de buffer circulaire
	InitialisationVariablesBufferCirculaire(*LiseEd);
	
	// init des variables de la structure dans l'état initial	
	InitStructureLiseEd(*LiseEd);

	LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLISE ED Serial Number : %s",LiseEd->Lise.SerialNumber);

#ifdef DEVICECONNECTED
	LogfileF(*LiseEd->Lise.Log,"[LISEED]\tDll define with DEVICECONNECTED - Nidacq OK");
#else
	LogfileF(*LiseEd->Lise.Log,"[LISEED]\tDll NOT define with DEVICECONNECTED - Nidacq NON OK - EMULATION");
#endif

	LogLEDInfo(*LiseEd->Lise.Log,"[LISEED]\tLise ED Acquisition Creation - Start",LiseEd->Lise.bDebug);

	// Allocation des buffers de traitement et résultats
	int BufferSize = LiseEd->Lise.BufferLen;
	LiseEd->Lise.BufferIntensity  = (double*) SPG_MemAlloc(BufferSize*sizeof(double),"LiseEd->Lise.BufferIntensity");

	int i =0;
	for (i = 0;i<MAXVOIE;i++)
	{
		LiseEd->Lise.BufferResultat[i] = (PICRESULT*) SPG_MemAlloc (LiseEd->Lise.PicResultLen*sizeof(PICRESULT),"LiseEd->Lise.BufferResultat");
	}
	LiseEd->Lise.Resultats = (PERIOD_RESULT*) SPG_MemAlloc (LiseEd->Lise.ResultLen*sizeof(PERIOD_RESULT),"LiseEd->Lise.Resultats");
	InitialisationBuffer(*LiseEd);

	// Détection de pic avec la méthode 4 sans seuil.
	K_FIT2_Init(LiseEd->Lise.fitDef);
	LiseEd->Lise.Fit=SPG_TypeAlloc(2*LiseEd->Lise.PeakWidthMax+1,float,"LiseEdAcqStart:Fit");

	// Utilisation de PeakMatch
	DefInit(LiseEd->Lise.sample, MAX_PEAKS );

	// on remplie le signal émulé
	if(!LiseEd->bLiseEDConnected) FillSignalEmule(*LiseEd);

	int RetCreate = 1;int RetStart=1;
	if(Param3!=LEDI_SLAVE_ED)
	{
		// fonction pour la création des différentes taches national Instrument
		RetCreate = CreateNITask(*LiseEd);

		// on démarre les taches nationnal instrument
		RetStart = StartNITask(*LiseEd);
	}
	else
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[DOUBLE_ED]\tDouble Lise Ed configuration required. This Lise Ed is considered as slave.");
	}

	if(RetCreate < 0 || RetStart < 0) 
	{
		LogLEDInfo(*LiseEd->Lise.Log,"[LISEED]\tEnd of Lise ED Acquisition Creation - Fail",LiseEd->Lise.bDebug);
		return FP_FAIL;
	}
	else
	{
		LogLEDInfo(*LiseEd->Lise.Log,"[LISEED]\tEnd of Lise ED Acquisition Creation - Success",LiseEd->Lise.bDebug);
		return FP_OK;
	}
}

// different mode of acquisition
int LEDIStartContinuousAcq(void* s)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	
	LISE_ED* LiseEd = (LISE_ED*)s;
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIStartContinousAcq - Start");
	}
	// 	SingleShot ou Continous
	LiseEd->Lise.AcqMode = Continous;

	return LEDIAcqStart(s);
}

int LEDIStartSingleShotAcq(void* s)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	
	LISE_ED* LiseEd = (LISE_ED*)s;
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIStartSingleShotAcq - Start");
	}
	// 	SingleShot ou Continous
	LiseEd->Lise.AcqMode = SingleShot;

	return LEDIAcqStart(s);
}

// single shot acq
int LEDIAcqStart(void* s)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	
	LISE_ED* LiseEd = (LISE_ED*)s;
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIAcqStart - Start");
	}

	// reinit des buffer circulaires
	InitialisationBuffer(*LiseEd);

	// on va changer le mode en fonction du mode courant
	LEDIRestartMeasurement(s);
	
	char temp[1024];
	sprintf(temp,"[LiseEd]\t Gain Value before Set Source Power: %f",LiseEd->Lise.fSourceValue);
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,temp);
	}
	
	LEDISetSourcePower(s,LiseEd->Lise.fSourceValue);

	StartAnalogChannel(*LiseEd);

	if(LiseEd->Lise.hThread != NULL)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tThread not initilized");
		}
		return DLL_FAIL;
	}

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tStarting Thread and Analog Ni Task");
	}

#ifdef _WATCHTIME_
	WatchTimeInit(LiseEd->WatchThreadUse,"ThreadUse");
	WatchTimeRestart(LiseEd->WatchThreadUse);
#endif

	// Configuration pour utiliser la thread
	LiseEd->Lise.hThread = CreateThread( 
        NULL,              // default security attributes
        0,                 // use default stack size  
        AcquisitionEtProcess,          // thread function 
        (LPVOID)(LiseEd),             // argument to thread function 
        0,                 // use default creation flags 
        &LiseEd->Lise.dwThreadId);   // returns the thread identifier 
    // Check the return value for success.
    // If failure, close existing thread handles, 
    // free memory allocation, and exit. 

	// la thread a été crée
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tThread created in FP dll");
	}

    if (LiseEd->Lise.hThread == NULL) 
    {
        CloseHandle(LiseEd->Lise.hThread);
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tLEDIAcqStart - End - Thread could not be initialized");
		}
		return DLL_FAIL;
    }

	// la thread a été crée
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tWaiting synchronisation with thread");
	}

	// on fait un sleep pour attendre le temps d'établissement de la thread et de la mesure
	Sleep(LiseEd->iMeasurementWaitTime);

	// attente pour que la thread soit active
	int TimeOut = 0;
	while(!LiseEd->Lise.bThreadActive)
	{
//FDE		DoEvents(0);
		TimeOut++;
		Sleep(1);
		if(TimeOut>100)
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[ERROR]\tTimeOut, thread not start in function LEDIAcqStart - End");
			}
			return DLL_FAIL;
		}
	}

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIAcqStart - End");
	}

	// tout s'est bien passé, on définit le mode de mesure
	LiseEd->AcquisitionMode = Measurement;

	// retour de la partie LISE ED
	return DLL_OK;
}

int LEDIAcqStop(void* s)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	
	LISE_ED* LiseEd = (LISE_ED*)s;

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIAcqStop - Start");
	}

	
	LiseEd->Lise.bStopDemande = true;
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log, "[LISEED]\tWaiting for end of acquisition thread");
    }

#ifdef _WATCHTIME_
	WatchTimeWatch(LiseEd->WatchThreadUse,"StopEnter");
#endif

	WaitForSingleObject(LiseEd->Lise.hThread, INFINITE);

#ifdef _WATCHTIME_
	WatchTimeWatch(LiseEd->WatchThreadUse,"WaitForSingleObj");
#endif

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log, "[LISEED]\tClosing acquisition thread");
	}
	
	// on va attendre que la ferm eture se soit bien déroulée
	//int Timeout = 0;
	//try
	//{
	//	while(CloseHandle(LiseEd->Lise.hThread) || Timeout > 100)
	//	{
	//		Sleep(10);
	//		Timeout++;
	//	}
	//}
	//catch(...)
	//{
	//	LogfileF(*LiseEd->Lise.Log, "[LISEED]\tException of first chance detected, trying to close handle Thread again");
	//}
	// on met le pointeur à null
	LiseEd->Lise.hThread = NULL;

	// On ferme tous les fichiers de sauvegardes
	if(LiseEd->Lise.FlagSavePics == true) fclose(LiseEd->Lise.FichierSavePics);
	if(LiseEd->Lise.FlagSaveSignal== true) fclose(LiseEd->Lise.FichierSaveSignal);
	if(LiseEd->Lise.FlagThickness == true) fclose(LiseEd->Lise.FichierSaveThickness);
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tClosing File peakMeasure.txt");
	}
	// Tous les flags pour les fichiers de sauvegarde repassent à false.
	LiseEd->Lise.FlagSavePics = false;
	LiseEd->Lise.FlagSaveSignal = false;
	LiseEd->Lise.FlagThickness = false;

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIAcqStop - End");
	}

	// tout s'est bien passé, on définit le mode de mesure
	LiseEd->AcquisitionMode = Stopped;

#ifdef _WATCHTIME_
	WatchTimeStop(LiseEd->WatchThreadUse);
	WatchTimeClose(LiseEd->WatchThreadUse);
#endif


	// retour ok de l afonction
	return DLL_OK;
}

int LEDIClose(void* s)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	
	LISE_ED* LiseEd = (LISE_ED*)s;

	int RetClose = 1;int RetDestroy =1;
	if(LiseEd->DblEdMode = Master)
	{
		// seul le master a eu ses task initialisé
		RetClose = CloseAllEdTaskNI(*LiseEd);
		RetDestroy = DestroyAllEdTaskNI(*LiseEd);
	}

	SPG_MemFree(LiseEd->Lise.Fit);
	
//Libère les ressources
	SPG_MemFree(LiseEd->Lise.BufferIntensity);
	for(int i = 0;i<MAXVOIE;i++)
	{
		SPG_MemFree(LiseEd->Lise.BufferResultat[i]);
	}
	SPG_MemFree(LiseEd->Lise.Resultats);
	
	// on libere la thread
	LiseEd->Lise.hThread = NULL;

	// Détection de pic avec la méthode 4 sans seuil.
	K_FIT2_Close(LiseEd->Lise.fitDef);
	DefClose(LiseEd->Lise.sample );

	if(LiseEd->Lise.FlagSavePics == true) fclose(LiseEd->Lise.FichierSavePics);
	if(LiseEd->Lise.FlagSaveSignal== true) fclose(LiseEd->Lise.FichierSaveSignal);
	if(LiseEd->Lise.FlagThickness == true) fclose(LiseEd->Lise.FichierSaveThickness);

	if(RetClose != 1 || RetDestroy != 1)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIClose - End - Failed");
		}	// Fermeture du log dans le cas où il n'est pas défini en externe
		if (LiseEd->Lise.internalLog)
		{
			LogfileClose(*LiseEd->Lise.Log);
			SPG_MemFree(LiseEd->Lise.Log);
		}
		return DLL_FAIL;
	}

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIClose - End - Success");
	}

	// Fermeture du log dans le cas où il n'est pas défini en externe
	if (LiseEd->Lise.internalLog)
	{
		LogfileClose(*LiseEd->Lise.Log);
		SPG_MemFree(LiseEd->Lise.Log);
	}
	return DLL_OK;
}
// Sauvegarde de certains paramètres de calibration
int LEDISaveConfig(void* s,char* ConfigFile,char* Password){
	int returnValue = DLL_OK;

	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}	
	LISE_ED* LiseEd = (LISE_ED*)s;

	if(strcmp(Password,"SiTuTrouvesLePassTuEsFort")!=0)
	{
		LogfileF(*LiseEd->Lise.Log, "[LISE_ED]\tError LEDISaveConfig - Wrong Password");
		return DLL_FAIL;
	}
	else
	{
		char charEmpty[MAXSTRING];
		sprintf(charEmpty,"");

		// On ouvre le fichier de params
		SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
		CFG_Init(CFG,0,ConfigFile,1024,0);

		if(CFG_SetFloatParam(CFG, "MaximumGain", LiseEd->Lise.GainMax, 0) == 0)
			returnValue = DLL_FAIL;

		if(CFG_SetFloatParam(CFG, "MinimumGain", LiseEd->Lise.GainMin, 0) == 0)
			returnValue = DLL_FAIL;

		float CalibWavelength = LiseEd->Lise.dRefWaveLengthNm - 1530.0f;

		if(CFG_SetFloatParam(CFG, "CalibWavelength", CalibWavelength, 0) == 0)
					returnValue = DLL_FAIL;

		if(CFG_SetFloatParam(CFG, "SaturThreshold", LiseEd->Lise.fThresholdSaturation, 0) == 0)
					returnValue = DLL_FAIL;

		if(CFG_SetIntParam(CFG, "NbChannels", LiseEd->Lise.NombredeVoie, 0) == 0)
					returnValue = DLL_FAIL;

		if(CFG_SetFloatParam(CFG, "ComparisonTol", LiseEd->Lise.ComparisonTolerance, 0) == 0)
					returnValue = DLL_FAIL;

		if(strcmp(LiseEd->Lise.NIDevice,charEmpty) != 0)
		if(CFG_SetStringParam(CFG, "NIDevice", LiseEd->Lise.NIDevice, 0) == 0)
					returnValue = DLL_FAIL;

		if(strcmp(LiseEd->Lise.TypeDevice,charEmpty) != 0)
		if(CFG_SetStringParam(CFG, "DeviceType", LiseEd->Lise.TypeDevice, 0) == 0)
					returnValue = DLL_FAIL;

		if(strcmp(LiseEd->Lise.SerialNumber,charEmpty) != 0)
		if(CFG_SetStringParam(CFG, "SerialNumber", LiseEd->Lise.SerialNumber, 0) == 0)
					returnValue = DLL_FAIL;

		if(strcmp(LiseEd->Lise.PartNumber,charEmpty) != 0)
		if(CFG_SetStringParam(CFG, "PartNumber", LiseEd->Lise.PartNumber, 0) == 0)
			returnValue = DLL_FAIL;

		if(strcmp(LiseEd->Lise.HardwareVersion,charEmpty) != 0)
		if(CFG_SetStringParam(CFG, "HardwareVersion", LiseEd->Lise.HardwareVersion, 0) == 0)
			returnValue = DLL_FAIL;


		// On ferme le fichier de params
		CFG_Close(CFG);
	}
	return returnValue;
}
// Fonction permettant de definir la puissance d'une des deux sources à n'importe quel moment entre Create et Destroy
int LEDISetSourcePower(void* s,double Volts)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	
	LISE_ED* LiseEd = (LISE_ED*)s;
	
	int error;
	LiseEd->Lise.fSourceValue = Volts;
	error = SetAnalogOutputSource1(*LiseEd,Volts);
	if(error == 1)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tSet Source Power OK");
		}
		return DLL_OK;
	}
	else 
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tSet Source Power Fail");
		}
		return DLL_FAIL;
	}
		
	return DLL_OK;
}

//######################################## Results #######################################
// Get the number of samples of the last waveform period
int LEDIGetNbSamplesWaveform(void* s,int* NbSamples)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	
	LISE_ED* LiseEd = (LISE_ED*)s;
	
	if(LiseEd->Lise.NombredeVoie == 2)
	{ // Cas des 2 voies
		*NbSamples = LiseEd->Lise.NbSamplesLastPeriod / 2;
	}
	else
	{ // Sinon dans tous les autres cas, on a qu'une voie
		*NbSamples = LiseEd->Lise.NbSamplesLastPeriod;
	}
	return DLL_OK;
}

int LEDIGetRawSignal(char* Password, void* s,double* I,int* NbSamples,float* StepX,int Voie,float* SaturationValue, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks)
{ // on doit être en start poour faire des GetRawSignal
	if(strcmp(Password,"SiTuTrouvesLePassTuEsFort") == 0)
	{
		LISE_ED* LiseEd = (LISE_ED*)s;
		// test sur la réentrance de la fonction pour ne pas l'appeler à outrance
		if(LiseEd->Lise.bGetRawSigReentrance == false)
		{
			// booléen de réentrance dans la fonction
			LiseEd->Lise.bGetRawSigReentrance = true;

			// acquisition de la waveform
			clock_t TimeStart = clock();
			clock_t TimeStop = TimeStart;
			LEDIGetWaveform(s,I,NbSamples,StepX,Voie, SelectedPeaks, nbSelectedPeaks, DiscardedPeaks, nbDiscardedPeaks);
			TimeStop = clock();

			//retour de la fonction
			LiseEd->Lise.bGetRawSigReentrance = false;
			*SaturationValue = LiseEd->Lise.fThresholdSaturation;
			*StepX = (LiseEd->Lise.dRefWaveLengthNm / 4);
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[DEBUG_FPMS]\tTime for GetRawSignal = %d",TimeStop - TimeStart);
			}
			return DLL_OK;
		}
		else
		{
			//reentrance dans la fonction
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[DEBUG_FPMS]\tAlready in GetRawSignal - reentrance non possible");
			}

			// retour DLL_FAIl ou DLL_OK???
			return DLL_OK;
		}
	}
	else
	{
		return DLL_FAIL;
	}
}

// Get the last waveform period
int LEDIGetWaveform(void* s,double* _I,int* NbSamples,float* StepX,int Voie, double* _SelectedPeaks, int* nbSelectedPeaks, double* _DiscardedPeaks, int* nbDiscardedPeaks)
{
	//__________________________________________________ Preconditions
	if (!s)
	{
		return DLL_FAIL;
	}

	SC_CACHED_ARRAY( double, I, _I, *NbSamples );
	SC_CACHED_ARRAY( double, SelectedPeaks, _SelectedPeaks, *nbSelectedPeaks );
	SC_CACHED_ARRAY( double, DiscardedPeaks, _DiscardedPeaks, *nbDiscardedPeaks );
	
	LISE_ED* LiseEd = (LISE_ED*)s;
	int Timeout = LEDI_TIMEOUT_GETTH;

	if (!I)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid Parameter In Function LiseEdGetWaveform");
		}
		return DLL_FAIL;
	}
	//__________________________________________________ 


	//__________________________________________________ Synchronisation
	// Parametre pour la thread, demande de lecture
	LiseEd->Lise.bNeedRead = true;	
	// On teste si la thread est active
	if(LiseEd->Lise.bThreadActive == true)
	{
		if(LiseEd->Lise.bDebug)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\t Waiting Synchronisation with thread");
		}	
		int c=0;
		clock_t TimeStart = clock();
		// parametre pour la thread, on attend la permission de lire
		while(!LiseEd->Lise.bReadAllowed)	
		{
			if(clock() - TimeStart > Timeout)
			{
				LiseEd->Lise.bNeedRead = false;
				*NbSamples=0;
				if(LiseEd->Lise.bDebug)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\tTimeout of thread, returning of GetrawSignal with DLL_OK");
				}
				LiseEd->Lise.bReentrance = false;
				// on ne valide pas les valeur d'épaisseur dans le cas présent
				return DLL_OK;
			}
			Sleep(1);
		}
	}
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tReading sample in buffer");
	}
	//__________________________________________________ 


	if(LiseEd->Lise.NombredeVoie == 2)
	{ // Cas des deux voies de mesure
		if(LiseEd->Lise.iConfigSensUnique == 0)
		{
			if(LiseEd->Lise.NbSamplesLastPeriod > LiseEd->Lise.iNbSampleMaxBuffer)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tWarning - Number of points too important in period. Problem with electronic signal, Pulse not found or too many points in waveform");
				if(Global.EnableList>=1) MessageBox(0,L"Problem with electronic signal. Pulse not found or too many points in waveform. Restarting acquisition",L"Acquisition error",0);
				SaveLastWaveformError(*LiseEd,0);
				//Restart de l'acquisition //
				CloseAnalogChannel(*LiseEd);
				StartAnalogChannel(*LiseEd);
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tStopping and Restarting the Acquisition in LEDIGetWaveform");
				LiseEd->Lise.iThicknessAboveQThreshold = 0;
				// rajout MA pour le traitement sur la qualité
				LiseEd->Lise.iThicknessMatchingPeakMode = 0;
				LiseEd->Lise.iThicknessBestPeakMode = 0;
				// fin de modif MA
				*NbSamples = 0;
				return DLL_FAIL;
			}
			else
			{
				*NbSamples = LiseEd->Lise.NbSamplesLastPeriod; // ATTENTION PEUT POSER PB VOIR EN DESSOUS LE NBSAMPLE = NPPERIOD / 2
				int j = 0;
				if (LiseEd->Lise.PulsePlusLeft.N > *NbSamples)
				{
					if(Voie == 1)
					{ // Affichage de la voie 1
						int begin = LiseEd->Lise.PulsePlusLeft.N - *NbSamples;
						int end = LiseEd->Lise.PulsePlusLeft.N;
						for(int i=0;i<(end - begin)/2;i++)
						{
							if(j+begin < LiseEd->Lise.BufferLen)
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin];
								j = j + 2;
							}
							else
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin - LiseEd->Lise.BufferLen];
								j = j + 2;
							}
						}
					}
					else if(Voie == 2)
					{ // Affichage de la voie 2
						int begin = LiseEd->Lise.PulsePlusLeft.N - *NbSamples + 1; // +1 pour commencer au premier échantillon de la voie 2
						int end = LiseEd->Lise.PulsePlusLeft.N;
						for(int i=0;i<(end - begin)/2 + 1;i++)
						{
							if(j+begin < LiseEd->Lise.BufferLen)
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin];
								j = j + 2;
							}
							else
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin - LiseEd->Lise.BufferLen];
								j = j + 2;
							}
						}
					}
					else if(Voie == 3)
					{ // configuration deux voie, mais on demande la voie 3 pour récupérer dans le buffer la ligne de saturation
						int begin = LiseEd->Lise.PulsePlusLeft.N - *NbSamples;
						int end = LiseEd->Lise.PulsePlusLeft.N;
						for(int i = 0;i<(end - begin)/2;i++)
						{
							I[i] = LiseEd->Lise.fThresholdSaturation;
						}
					}
					else
					{
						if(LiseEd->Lise.bDebug == true)
						{
							LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid parameter Channel in function call");
						}
						return DLL_FAIL;
					}
				}
				else
				{
					if(Voie==1)
					{
						int begin = LiseEd->Lise.PulsePlusLeft.N - *NbSamples + LiseEd->Lise.BufferLen;
						int end = LiseEd->Lise.PulsePlusLeft.N;
						for(int i=0;i<*NbSamples/2;i++)
						{
 							if (j<*NbSamples-LiseEd->Lise.PulsePlusLeft.N)
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin];
								j = j + 2;
							}
							else
							{
								I[i] = LiseEd->Lise.BufferIntensity[j-*NbSamples+LiseEd->Lise.PulsePlusLeft.N];
								j = j + 2;
							}
						}
					}
					else if(Voie == 2)
					{
						int begin = LiseEd->Lise.PulsePlusLeft.N - *NbSamples + LiseEd->Lise.BufferLen + 1;
						int end = LiseEd->Lise.PulsePlusLeft.N;
						for(int i=0;i<*NbSamples/2 + 1;i++)
						{
 							if (j<*NbSamples-LiseEd->Lise.PulsePlusLeft.N)
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin];
								j = j + 2;
							}
							else
							{
								I[i] = LiseEd->Lise.BufferIntensity[j-*NbSamples+LiseEd->Lise.PulsePlusLeft.N];
								j = j + 2;
							}
						}
					}
					else if(Voie == 3)
					{ // configuration deux voie, mais on demande la voie 3 pour récupérer dans le buffer la ligne de saturation
						for(int i = 0;i<*NbSamples/2+1;i++)
						{
							I[i] = LiseEd->Lise.fThresholdSaturation;
						}
					}
					else
					{
						if(LiseEd->Lise.bDebug == true)
						{
							LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid parameter Channel in function call");
						}
						return DLL_FAIL;
					}
				}
				*NbSamples = LiseEd->Lise.NbSamplesLastPeriod / 2;

				//____________________________________ MAJ MP 17/03/11

				// Position des pics sélectionnés et non-sélectionnés
				if (SelectedPeaks != NULL && DiscardedPeaks != NULL && nbSelectedPeaks != NULL && nbDiscardedPeaks != NULL)
				{
					if (Voie == 1 || Voie == 2)
					{
						PERIOD_RESULT* result = &LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)];
						//PERIOD_RESULT* result = &PERIOD_RESULT(LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)]);
						
						int j = 0;
						int k = 0;
						double PulseWidth = LiseEd->PulsePlusWidthLastPeriod/2;
						int iNbPicsPlus = result->NbPicsPlusVoie1;
						int iNbPicsMoins = result->NbPicsMoinsVoie1;

						if(Voie == 2){
							iNbPicsPlus = result->NbPicsPlusVoie2;
							iNbPicsMoins = result->NbPicsMoinsVoie2;
						}
						int iNbSelectedPics = 0;
						int iNbDiscartedPics = 0;
						int iNumCurrPic = 0;
						for (int i=0; i<result->NbPicsPlusVoie1 + result->NbPicsPlusVoie2; i++)
							{
							if(result->PkOrigPlus[i] == Voie){ // supprimer les tests subsequants de cette condition
								if ((result->PkIndexPlus[j] == i) && (result->fQuality > 0.0) && (j <= (LiseEd->Lise.sample.NbThickness + 1)) )
								{
									if( iNbSelectedPics < *nbSelectedPeaks )
									{
										if(Voie == 1){
											SelectedPeaks[iNbSelectedPics] = result->PicsPlusVoie1[iNumCurrPic].XRel + PulseWidth;
										}
										else{
											SelectedPeaks[iNbSelectedPics] = result->PicsPlusVoie2[iNumCurrPic].XRel + PulseWidth;
										}
										if(LiseEd->Lise.bDebugProcess == true)
										{
											LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tPeak Selected Index = %i and peak ref value = %f",result->PkIndexPlus[j],SelectedPeaks[j]);
										}
										iNbSelectedPics ++;
										j++;
									}
								}
								else
								{
									if(result->PkOrigPlus[i] == Voie) // NOTE SPG TODO : supprimer repetition de la condition du if englobant
									{
										if( iNbDiscartedPics < *nbDiscardedPeaks )
										{
											if(Voie == 1)
												DiscardedPeaks[k] = result->PicsPlusVoie1[iNumCurrPic].XRel + PulseWidth;
											else
												DiscardedPeaks[k] = result->PicsPlusVoie2[iNumCurrPic].XRel + PulseWidth;

											if(LiseEd->Lise.bDebugProcess == true)
											{
												LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tPeak Discarted Index = %i and peak ref value = %f",i,DiscardedPeaks[k]);
											}
											k++;
											iNbDiscartedPics++;
										}
									}
								}
								iNumCurrPic++;
							}
							else if ((result->PkIndexPlus[j] == i) && (result->fQuality > 0.0) && (j <= (LiseEd->Lise.sample.NbThickness + 1)) )
							{
								j++;
							}
						}
						
						iNumCurrPic = 0;
						j=0;
						int inbPics = result->NbPicsMoinsVoie1 +result->NbPicsMoinsVoie2;
						for (int i=0; i<inbPics; i++)
						{
							if(result->PkOrigMoins[i] == Voie){
								if ((result->PkIndexMoins[j/*j - iNbSelectedPics*/] == i) && (result->fQuality > 0.0))
								{									
									if( iNbSelectedPics < *nbSelectedPeaks )
									{
										if(Voie == 1)
											SelectedPeaks[iNbSelectedPics] = result->NbSamplesPeriod - result->PicsMoinsVoie1[iNumCurrPic].XRel;
										else
											SelectedPeaks[iNbSelectedPics] = result->NbSamplesPeriod - result->PicsMoinsVoie2[iNumCurrPic].XRel;
										j++;		
										iNbSelectedPics++;							
									}
								}
								else
								{
									if( iNbDiscartedPics < *nbDiscardedPeaks )
									{
										if(Voie == 1)
											DiscardedPeaks[k] = result->NbSamplesPeriod - result->PicsMoinsVoie1[iNumCurrPic].XRel;
										else
											DiscardedPeaks[k] = result->NbSamplesPeriod - result->PicsMoinsVoie2[iNumCurrPic].XRel;

										k++;
										iNbDiscartedPics++;
									}
								}
								iNumCurrPic++;
							}
							else if ((result->PkIndexMoins[j] == i) && (result->fQuality > 0.0))
							{
								j++;
							}
						}
						*nbSelectedPeaks = iNbSelectedPics;
						*nbDiscardedPeaks = iNbDiscartedPics;
						//if(2*(LiseEd->Lise.sample.NbThickness+1) != *nbSelectedPeaks)
						{
							if(LiseEd->Lise.bDebugProcess == true)
							{
								LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tnbSelectedPeaks = %i",*nbSelectedPeaks);
								for(int i = 0; i<NB_PEAK_MAX;i++)
								{
									LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tPkPlusIndex = %i",result->PkIndexPlus[i]);
								}
							}
						}
					}
					else
					{
						*nbSelectedPeaks = 0;
						*nbDiscardedPeaks = 0;
					}
				}
				//____________________________________ Fin MAJ MP 17/03/11
			}
		}
		else
		{ // cas de la selection du sens positif
			int begin = LiseEd->PulseMoinsLeft.N - LiseEd->LtPlus;
			int end = LiseEd->PulseMoinsLeft.N;
			
			int j = 0;

			if(LiseEd->PulseMoinsLeft.N > LiseEd->LtPlus)
			{ // Cas ou l'on a Pulse Plus Right inferieur a Pulse Moins Left
				if(Voie == 1)
				{
					*NbSamples = (end - begin) / 2;
					for(int i = 0;i<(end - begin)/2;i++)
					{
						if(j+begin < LiseEd->Lise.BufferLen)
						{
							I[i] = LiseEd->Lise.BufferIntensity[j+begin];
							j = j + 2;
						}
						else
						{
							I[i] = LiseEd->Lise.BufferIntensity[j+begin-LiseEd->Lise.BufferLen];
							j = j + 2;
						}
					}
				}
				else if(Voie == 2)
				{
					begin = begin + 1;
					*NbSamples = (end - begin) / 2;
					for(int i = 0;i<(end - begin)/2;i++)
					{
						if(j+begin < LiseEd->Lise.BufferLen)
						{
							I[i] = LiseEd->Lise.BufferIntensity[j+begin];
							j = j + 2;
						}
						else
						{
							I[i] = LiseEd->Lise.BufferIntensity[j+begin-LiseEd->Lise.BufferLen];
							j = j + 2;
						}
					}
				}
				else if(Voie == 3)
				{ // configuration deux voie, mais on demande la voie 3 pour récupérer dans le buffer la ligne de saturation
					for(int i = 0;i<(end - begin)/2;i++)
					{
						I[i] = LiseEd->Lise.fThresholdSaturation;
					}
				}
				else
				{
					if(LiseEd->Lise.bDebug == true)
					{
						LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid parameter Channel in function call");
					}
					return DLL_FAIL;
				}
			}
			else
			{
				if(Voie == 1)
				{
					int begin = LiseEd->PulseMoinsLeft.N - LiseEd->LtPlus + LiseEd->Lise.BufferLen;
					int end = LiseEd->PulseMoinsLeft.N;
					*NbSamples = LiseEd->LtPlus / 2;
					for(int i=0;i<LiseEd->LtPlus/2;i++)
					{
 						if (j<LiseEd->LtPlus - LiseEd->PulseMoinsLeft.N)
						{
							I[i] = LiseEd->Lise.BufferIntensity[j+begin];
							j = j + 2;
						}
						else
						{
							I[i] = LiseEd->Lise.BufferIntensity[j - LiseEd->LtPlus+LiseEd->PulseMoinsLeft.N];
							j = j + 2;
						}
					}
				}
				else if(Voie == 2)
				{
					int begin = LiseEd->PulseMoinsLeft.N - LiseEd->LtPlus + LiseEd->Lise.BufferLen + 1;
					int end = LiseEd->PulseMoinsLeft.N;
					*NbSamples = LiseEd->LtPlus / 2;
					for(int i=0;i<LiseEd->LtPlus/2;i++)
					{
 						if (j<LiseEd->LtPlus - LiseEd->PulseMoinsLeft.N)
						{
							I[i] = LiseEd->Lise.BufferIntensity[j+begin];
							j = j + 2;
						}
						else
						{
							I[i] = LiseEd->Lise.BufferIntensity[j - LiseEd->LtPlus+LiseEd->PulseMoinsLeft.N];
							j = j + 2;
						}
					}
				}
				else if(Voie == 3)
				{ // configuration deux voie, mais on demande la voie 3 pour récupérer dans le buffer la ligne de saturation
					for(int i = 0;i<LiseEd->LtPlus/2;i++)
					{
						I[i] = LiseEd->Lise.fThresholdSaturation;
					}
				}
				else
				{
					if(LiseEd->Lise.bDebug == true)
					{
						LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid parameter Channel in function call");
					}
					return DLL_FAIL;
				}
			}
		}
	}
	else
	{ 
		// Cas ou l'on n'a qu'une voie et tous les autres cas
		if(LiseEd->Lise.NbSamplesLastPeriod > LiseEd->Lise.iNbSampleMaxBuffer)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tWarning - Number of points too important in period. Problem with electronic signal, Pulse not found or too many points in waveform");
			if(Global.EnableList>=1) MessageBox(0,L"Problem with electronic signal. Pulse not found or too many points in waveform. Restarting acquisition",L"Acquisition error",0);
			//Restart de l'acsuisition //
			SaveLastWaveformError(*LiseEd,0);
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tStopping and Restarting the Acquisition in LEDIGetWaveform");
			CloseAnalogChannel(*LiseEd);
			StartAnalogChannel(*LiseEd);
			LiseEd->Lise.iThicknessAboveQThreshold = 0;
			// rajout MA pour le traitement sur les la qualité
			LiseEd->Lise.iThicknessMatchingPeakMode = 0;
			LiseEd->Lise.iThicknessBestPeakMode = 0;
			// fin de modif MA
			*NbSamples = 0;
			return DLL_FAIL;
		}
		else
		{
			if(LiseEd->Lise.iConfigSensUnique == 0)
			{ 
				// cas ou l'on récupère toute la waveform
				*NbSamples = LiseEd->Lise.NbSamplesLastPeriod;
				if (LiseEd->Lise.PulsePlusLeft.N > LiseEd->Lise.NbSamplesLastPeriod)
				{
					int begin = LiseEd->Lise.PulsePlusLeft.N - LiseEd->Lise.NbSamplesLastPeriod;
					int end = LiseEd->Lise.PulsePlusLeft.N;
					if(Voie == 2)
					{ // configuration une voie, mais on demande la voie 2 pour récupérer dans le buffer la ligne de saturation
						for(int i = 0;i<(end - begin);i++)
						{
							I[i] = LiseEd->Lise.fThresholdSaturation;
						}
					}
					else
					{
						for(int i=0;i<end - begin;i++)
						{
 							I[i] = LiseEd->Lise.BufferIntensity[i+begin];
						}
					}
				}
				else
				{
					int begin = LiseEd->Lise.PulsePlusLeft.N - LiseEd->Lise.NbSamplesLastPeriod + LiseEd->Lise.BufferLen;
					int end = LiseEd->Lise.PulsePlusLeft.N;
					if(Voie == 2)
					{ // configuration une voie, mais on demande la voie 2 pour récupérer dans le buffer la ligne de saturation
						for(int i = 0;i<LiseEd->Lise.NbSamplesLastPeriod;i++)
						{
							I[i] = LiseEd->Lise.fThresholdSaturation;
						}
					}
					else
					{
						for(int i=0;i<LiseEd->Lise.NbSamplesLastPeriod;i++)
						{
 							if (i<LiseEd->Lise.NbSamplesLastPeriod-LiseEd->Lise.PulsePlusLeft.N)
							{
								I[i] = LiseEd->Lise.BufferIntensity[i+begin];
							}
							else
							{
								I[i] = LiseEd->Lise.BufferIntensity[i-LiseEd->Lise.NbSamplesLastPeriod+LiseEd->Lise.PulsePlusLeft.N];
							}
						}
					}
				}
				// Position des pics sélectionnés et non-sélectionnés
				if (SelectedPeaks != NULL && DiscardedPeaks != NULL && nbSelectedPeaks != NULL && nbDiscardedPeaks != NULL)
				{
					if (Voie == 1)
					{
						PERIOD_RESULT* result = &LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)];
						int j = 0;
						int k = 0;
						{for (int i=0; i<result->NbPicsPlusVoie1; i++)
						{
							if ((j < *nbSelectedPeaks) && (result->PkIndexPlus[j] == i && result->fQuality > 0.0 && j <= (LiseEd->Lise.sample.NbThickness + 1) ) )
							{
								// Attention, les pics sont décalés de la largeur de Pulse Plus sur l'affichage.
								// Attention, ici, c'est la largeur de la période suivante qui est utilisée, il faudrait créer une autre variable.
								//SelectedPeaks[j] = result->PicsPlusVoie1[i].XRel + LiseEd->Lise.PulsePlusRight.AbsN - LiseEd->Lise.PulsePlusLeft.AbsN;
								// MA 22/01/2009 : modif pour que les pics soient en face de la waveform
								SelectedPeaks[j] = result->PicsPlusVoie1[i].XRel + LiseEd->PulsePlusWidthLastPeriod;
								if(LiseEd->Lise.bDebugProcess == true)
								{
									LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tPeak Selected Index = %i and peak ref value = %f",result->PkIndexPlus[j],SelectedPeaks[j]);
								}
								j++;
							}
							else if ( k < *nbDiscardedPeaks )
							{
								DiscardedPeaks[k] = result->PicsPlusVoie1[i].XRel + LiseEd->PulsePlusWidthLastPeriod;
								if(LiseEd->Lise.bDebugProcess == true)
								{
									LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tPeak Discarted Index = %i and peak ref value = %f",i,DiscardedPeaks[k]);
								}
								k++;
							}
						}}
						int nbSelectedPeaksVoie1 = j;
						{for (int i=0; i<result->NbPicsMoinsVoie1; i++)
						{
							if ((j < *nbSelectedPeaks) && (result->PkIndexMoins[j - nbSelectedPeaksVoie1] == i && result->fQuality > 0.0) )
							{
								SelectedPeaks[j] = result->NbSamplesPeriod - result->PicsMoinsVoie1[i].XRel;
								j++;
							}
							else if ( k < *nbDiscardedPeaks )
							{
								DiscardedPeaks[k] = result->NbSamplesPeriod - result->PicsMoinsVoie1[i].XRel;
								k++;
							}
						}}
						*nbSelectedPeaks = j;
						*nbDiscardedPeaks = k;
						//if(2*(LiseEd->Lise.sample.NbThickness+1) != *nbSelectedPeaks)
						{
							if(LiseEd->Lise.bDebugProcess == true)
							{
								LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tnbSelectedPeaks = %i",*nbSelectedPeaks);
								for(int i = 0; i<NB_PEAK_MAX;i++)
								{
									LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tPkPlusIndex = %i",result->PkIndexPlus[i]);
								}
							}
						}
					}
					else
					{
						*nbSelectedPeaks = 0;
						*nbDiscardedPeaks = 0;
					}
				}
			}
			else
			{ //Sens unique sur une seule voie, on récupère que la première partie de la waveform
				int begin = LiseEd->PulseMoinsLeft.N - LiseEd->LtPlus;
				int end = LiseEd->PulseMoinsLeft.N;

				int j = 0;
				if(LiseEd->PulseMoinsLeft.N > LiseEd->LtPlus)
				{ // Cas ou l'on a Pulse Plus Right inferieur a Pulse Moins Left
					*NbSamples = (end - begin);
									// cas ou l'on doit remplir le buffer de la valeur de saturation
					if(Voie == 2)
					{ // configuration une voie, mais on demande la voie 2
						for(int i = 0;i<(end - begin);i++)
						{
							I[i] = LiseEd->Lise.fThresholdSaturation;
						}
					}
					else
					{ // cas normal on récupère le buffer
						for(int i = 0;i<(end - begin);i++)
						{
							if(j+begin < LiseEd->Lise.BufferLen)
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin];
								j++;
							}
							else
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin-LiseEd->Lise.BufferLen];
								j++;
							}
						}
					}
				}
				else
				{
					int begin = LiseEd->PulseMoinsLeft.N - LiseEd->LtPlus + LiseEd->Lise.BufferLen;
					int end = LiseEd->PulseMoinsLeft.N;
					*NbSamples = LiseEd->LtPlus;
					if(Voie == 2)
					{ // configuration une voie, mais on demande la voie 2 pour remplir le buffer de la valeur de saturation
						for(int i = 0;i<LiseEd->LtPlus;i++)
						{
							I[i] = LiseEd->Lise.fThresholdSaturation;
						}
					}
					else
					{ // cas normal, on récupère le buffer
						for(int i=0;i<LiseEd->LtPlus;i++)
						{
 							if (j<LiseEd->LtPlus - LiseEd->PulseMoinsLeft.N)
							{
								I[i] = LiseEd->Lise.BufferIntensity[j+begin];
								j++;
							}
							else
							{
								I[i] = LiseEd->Lise.BufferIntensity[j - LiseEd->LtPlus+LiseEd->PulseMoinsLeft.N];
								j ++;
							}
						}
					}
				}
				// Position des pics sélectionnés et non-sélectionnés
				if (SelectedPeaks != NULL && DiscardedPeaks != NULL && nbSelectedPeaks != NULL && nbDiscardedPeaks != NULL)
				{
					SC_FIXED_ARRAY(char, Indextab, MAX_PEAKS);
					if (Voie == 1)
					{
						PERIOD_RESULT* result = &LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)];					
						int j = 0;
						int k = 0;
						for (int i=0; i<result->NbPicsPlusVoie1; i++)
						{
							if ((j < *nbSelectedPeaks) && (result->PkIndexPlus[j] == i && result->fQualityPlus > 0.0) )
							{
								// Attention, les pics sont décalés de la largeur de Pulse Plus sur l'affichage.
								// Attention, ici, c'est la largeur de la période suivante qui est utilisée, il faudrait créer une autre variable.
								//SelectedPeaks[j] = result->PicsPlusVoie1[i].XRel + LiseEd->Lise.PulsePlusRight.AbsN - LiseEd->Lise.PulsePlusLeft.AbsN;
								// MA 22/01/2009 : modif pour que les pics soient en face de la waveform
								SelectedPeaks[j] = result->PicsPlusVoie1[i].XRel + LiseEd->PulsePlusWidthLastPeriod;
								j++;
							}
							else if ( k < *nbDiscardedPeaks )
							{
								//DiscardedPeaks[k] = result->PicsPlusVoie1[i].XRel + LiseEd->Lise.PulsePlusRight.AbsN - LiseEd->Lise.PulsePlusLeft.AbsN;
								DiscardedPeaks[k] = result->PicsPlusVoie1[i].XRel + LiseEd->PulsePlusWidthLastPeriod;
								k++;
							}
						}
						*nbSelectedPeaks = j;
						*nbDiscardedPeaks = k;
					}
					else
					{
						*nbSelectedPeaks = 0;
						*nbDiscardedPeaks = 0;
					}
					if(2*(LiseEd->Lise.sample.NbThickness+1) != *nbSelectedPeaks)
					{
						if(LiseEd->Lise.bDebugProcess == true)
						{
							LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tnbSelectedPeaks = %i",*nbSelectedPeaks);
							for(int i = 0; i<MAX_PEAKS;i++)
							{
								LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[SELECTED PEAKS]\tPkPlusIndex = %i",Indextab[i]);
							}
						}
					}
				}
			}
		}
	}
	LiseEd->Lise.bNeedRead = false;	//parametre pour la thread
	*StepX = (float)LiseEd->Lise.dRefWaveLengthNm;
	return DLL_OK;
}

// Get the number of peaks of the last period
int LEDIGetNbPeaksPeriod(void* s,int* NbPeaks,int* Voie)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	LISE_ED* LiseEd = (LISE_ED*)s;

	if(LiseEd->Lise.bDebug)
	{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tEntering LiseEDGetNbPeaksPeriod");
	}	
	if(*Voie == 1)
	{
		*NbPeaks = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie2 + LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsMoinsVoie2;
	}
	else
	{
		*NbPeaks = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie1 + LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsMoinsVoie1;
	}
	if(LiseEd->Lise.bDebug)
	{
		char temp[1024];
		sprintf(temp,"[LiseED]\tNbPeaks detected %i",*NbPeaks);
		LogfileF(*LiseEd->Lise.Log,temp);
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tFunction LiseEDGetNbPeaksPeriod Success");
	}
	return DLL_OK;
}

// Get the peaks of the last period
int LEDIGetPeaksPeriod(void* s,double* XPosRel, double* Intensite, double* Quality, int* Sens, int* Flag, int* NbPeaks,int* Voie)
{
	// Preconditions
	if (!s)
	{
		return DLL_FAIL;
	}
	LISE_ED* LiseEd = (LISE_ED*)s;
	if (!XPosRel || !Intensite || !Quality || !Sens || !Flag)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid parameter in LiseEDGetPeaksPeriod");
		}
		return DLL_FAIL;
	}
	if (*NbPeaks < LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie1 + LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie1)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tValue of NbPeaks too small in LiseEDGetPeaks");
		}
		return DLL_FAIL;
	}

	LiseEd->Lise.bNeedRead = true;	// parametre pour la thread
	if(LiseEd->Lise.bDebug)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tEntering LiseEDGetPeaksPeriod");
	}	
	if(LiseEd->Lise.bThreadActive == true)
	{
		if(LiseEd->Lise.bDebug)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\t Waiting Synchronisation with thread");
		}	
		int c=0;
		while(!LiseEd->Lise.bReadAllowed)	// parametre pour la thread
		{
			Sleep(10);
			if(c++>50) 
			{
				LiseEd->Lise.bNeedRead = false;
				if(LiseEd->Lise.bDebug)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\tTimeout of thread, returning of GetPeaksPeriod with DLL_OK");
				}	
				return DLL_OK;
			}
		}
	}

	if(LiseEd->Lise.bDebug)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\t Synchronisation with thread in GetPeaksPeriod Success, Get peaks");
	}	

	RING_BUFFER_POS IndiceTempCopyPeak;
	int Indice1=0,Indice2=0;
	if(*Voie == 1)
	{
		*NbPeaks = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie2 + LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsMoinsVoie2;
		IndiceTempCopyPeak = LiseEd->Lise.WriteResultSecondChannel;
		Indice1 = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie2;
		Indice2 = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsMoinsVoie2;
	}
	else
	{
		if(*NbPeaks > LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie1 + LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsMoinsVoie1)
		{
			*NbPeaks = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie1 + LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsMoinsVoie1;
			Indice1 = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsPlusVoie1;
			Indice2 = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].NbPicsMoinsVoie1;
		}
		else
		{ // a tester
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tArgument NbPeaks is smaller - Please Increase Argument NbPeaks In Function LiseGetPeaksPeriod");
			}
			Indice1 = *NbPeaks / 2;
			Indice2 = *NbPeaks / 2;
		}
		IndiceTempCopyPeak = LiseEd->Lise.WriteResult;
	}

	if(*Voie == 1)
	{
		int j = 0;
		int i = 0;
		for(i = 0;i<Indice1;i++)
		{ // Copie des pics de la voie 2 pour le sens Positif
			if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie2[i].Intensite > LiseEd->Lise.fIntensityThreshold && LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie2[i].Qualite > LiseEd->Lise.fQualityThreshold)
			{
				XPosRel[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie2[i].XRel;
				Intensite[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie2[i].Intensite;
				Sens[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie2[i].Sens;
				Quality[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie2[i].Qualite;
				j++;
			}
		}
		if(LiseEd->Lise.iConfigSensUnique == 0)
		{
			int k = 0;
			for(i = 0;i<Indice2;i++)
			{ // copie des pics de la voie 2 pour le sens négatif 
				if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie2[i].Intensite > LiseEd->Lise.fIntensityThreshold && LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie2[i].Qualite > LiseEd->Lise.fQualityThreshold)
				{
					XPosRel[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie2[i].XRel;
					Intensite[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie2[i].Intensite;
					Sens[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie2[i].Sens;
					Quality[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie2[i].Qualite;
					k++;
				}
			}
		}
	}
	else
	{
		int j = 0;
		int i = 0;
		for(i = 0;i<Indice1;i++)
		{ // Copie des pics de la voie 1 pour le sens Positif
			if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie1[i].Intensite > LiseEd->Lise.fIntensityThreshold && LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie1[i].Qualite > LiseEd->Lise.fQualityThreshold)
			{
				XPosRel[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie1[i].XRel;
				Intensite[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie1[i].Intensite;
				Sens[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie1[i].Sens;
				Quality[j] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsPlusVoie1[i].Qualite;
				j++;
			}
		}
		if(LiseEd->Lise.iConfigSensUnique == 0)
		{	
			int k = 0;
			for(i = 0;i<Indice2;i++)
			{ // copie des pics de la voie 1 pour le sens négatif 
				if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie1[i].Intensite > LiseEd->Lise.fIntensityThreshold && LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie1[i].Qualite > LiseEd->Lise.fQualityThreshold)
				{
					XPosRel[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie1[i].XRel;
					Intensite[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie1[i].Intensite;
					Sens[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie1[i].Sens;
					Quality[j+k] = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].PicsMoinsVoie1[i].Qualite;
					k++;
				}
			}
		}
	}
	LiseEd->Lise.bNeedRead = false;
	if(LiseEd->Lise.bDebug)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\t Function GetPeaks Period success");
	}	

	return DLL_OK;
}

// Read temperature
int LEDIReadTemperature(void* s,double* Temperature)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	LISE_ED* LiseEd = (LISE_ED*)s;
	
	int error;
	StartPuissanceChannel(*LiseEd);
	error = SwitchPRecoupZero(*LiseEd);
	if(error == 1) return DLL_OK;
	else return DLL_FAIL;

	error = ReadPuissTemperature(*LiseEd,Temperature);
	ClosePuissanceChannel(*LiseEd);
	if(error == 1) return DLL_OK;
	else return DLL_FAIL;

	*Temperature = 25.0;
	return DLL_OK;
}

// Read optical power received
int LEDIReadPower(void* s,double* Power)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	LISE_ED* LiseEd = (LISE_ED*)s;
	
	int error;

	// si on est déjà en mesure, il faut fermer la thread
	if(LiseEd->AcquisitionMode == Measurement || LiseEd->AcquisitionMode == Stopped)
	{
		LiseEd->Lise.bStopDemande = true;
		// Attente que la thread se termine
		WaitForSingleObject(LiseEd->Lise.hThread, INFINITE);
		// Fermeture de la thread
		CloseHandle(LiseEd->Lise.hThread);
		// Fermeture de la voie analogique
		CloseAnalogChannel(*LiseEd);
		// Commencement de la tache de Puissance recouplée
		StartPuissanceChannel(*LiseEd);
	}

	// on change le mode
	LiseEd->AcquisitionMode = RecoupPower;

	// on va aller lire la valeur de puissance
	//if(GetAsyncKeyState(VK_CONTROL))
	//{//ancien style
		error = ReadPuissTemperature(*LiseEd,Power);
	//}
	//else
	//{
	//	error = ReadPuissAveraged(*LiseEd,Power);
	//}
#ifndef DEVICECONNECTED
	*Power = 0.27;	// Retour d'une valeur aléatoire pour le soft Labview
#endif

	if(LiseEd->bLiseEDConnected == false)
	{ // Cas ou DEVICEONNECTED est défini, mais pas le booléen dans le fichier de config
		*Power = 0.27;	// Retour d'une valeur aléatoire pour le soft Labview
	}

	// retour ok
	return DLL_OK;
}

int LEDIRestartMeasurement(void* s)
{
	// on caste de manière à récupérer la structure LiseED
	LISE_ED* LiseEd = (LISE_ED*)s;

	// on vérifie le cas ou l'on était pas en mesure de puissance recouplée
	if(LiseEd->AcquisitionMode == RecoupPower || LiseEd->AcquisitionMode == Stopped)
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tClosing recoup channel");
		}

		// on arrête la tache de puissance recouplée
		ClosePuissanceChannel(*LiseEd);

		// 	SingleShot ou Continous??
		if(LiseEd->Lise.AcqMode == SingleShot)
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tStarting acquisition single shot mode ");
			}
		}
		else // Continous
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tStarting acquisition continous mode ");
			}
		}
		// on change le mode
		LiseEd->AcquisitionMode = Measurement;
	}

	// on réinitialise le booléen de stop de la thread
	LiseEd->Lise.bStopDemande = false;

	// on fait un retour OK
	return DLL_OK;
}


// Get thickness, On remplit un tableau d'épaisseurs de la dernière periode de la structure PERIOD_RESULT
int LEDIGetThickness(void* s,double* _Thickness, double* _Quality, int _iNbThickness)
{ 
	// Precondition
	if (!s) { return DLL_FAIL; }
    SC_CACHED_ARRAY(double, Thickness, _Thickness, _iNbThickness);
    SC_CACHED_ARRAY(double, Quality, _Quality, _iNbThickness);
	int NbThicknesses = 0;
	int Timeout = LEDI_TIMEOUT_GETTH;
	LISE_ED* LiseEd = (LISE_ED*)s;

	// Incoherence entre define sample et tableaux alloues pour le getThickness
    if(_iNbThickness != LiseEd->Lise.sample.NbThickness)
	{
		LogfileF(*LiseEd->Lise.Log, "[LEDIGetThickness]\t[FATAL ERROR]\tIncoherence between defined sample (%i) and allocation (%i) ",LiseEd->Lise.sample.NbThickness,_iNbThickness);
	}

	CHECK(Thickness==0,"LEDIGetThickness",return 0);
	CHECK(Quality==0,"LEDIGetThickness",return 0);

	// on fait un clear des tableaux de qualité, thickness
	memset(Thickness,0,V_Min(_iNbThickness,LiseEd->Lise.sample.NbThickness)*sizeof(double));
	memset(Quality,0,V_Min(_iNbThickness,LiseEd->Lise.sample.NbThickness)*sizeof(double));

	CHECK(LiseEd->Lise.bReentrance,"LEDIGetThickness : reentrance",return 0);

	LiseEd->Lise.bReentrance=true;

	// Check de la probe courante dans le cas d'un double Ed
	if(LiseEd->CurrentProbeFromParentStruct)
	{
#ifdef _DEBUG
		if(LiseEd->ThisProbe != *LiseEd->CurrentProbeFromParentStruct)
		{
			BreakHook();
		}
#endif
	}

	// cas de l'échantillon défini
	if(LiseEd->Lise.sample.bDefined == true)
	{ 
		// Si l'échantillon est défini, alors on renvoie le nombre dépaisseurs définit dans l'échantillon
		NbThicknesses = LiseEd->Lise.sample.NbThickness;
	}
	else
	{ 
		// Si l'échantillon n'est pas défini, on renvoie le nombre d'épaisseurs détectés dans le process
		NbThicknesses = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].iNbThickness;

		// log de l'information
		logF(LiseEd, "[LISEED]\tWarning - Sample not Define, please define a sample");
	}

	// Cas d'utilisation du Gain automatique
	if(LiseEd->bUseAutoGain && (LiseEd->Lise.AcqMode == SingleShot)){
				
		//if(LiseEd->Lise.AcqMode == SingleShot)
		{
			logF(LiseEd,"[LISEED]\t---- Single shot mode, AutoGain Process ----");
			// Mode Single Shot
			ProcessAutoGain_SingleShot_Mode(LiseEd);
		}
		/*else if(LiseEd->Lise.AcqMode == Continous)
		{
			logF(LiseEd,"[LISEED]\t---- Continuous mode, AutoGain Process ----");
			// Mode Continuous
			ProcessAutoGain_Continuous_Mode(LiseEd,Thickness, Quality,NbThicknesses);
		}*/
	}

	if(LiseEd->Lise.AcqMode == Continous)
	{
		// on incremente l'index sur la mesure
		INC_Index_Measure(LiseEd->Lise.iCounterMeasure);

		int RetVal = LEDGetContinousThickness(*LiseEd,Thickness,Quality,NbThicknesses);
		return RetVal;
	}
	else if(LiseEd->Lise.AcqMode == SingleShot)
	{
		logF(LiseEd,"[LISEED]\t---- Single shot Mode define, return n measures valid ----");

		// mode single shot, on attend une mesure moyennée sur n mesures
		LiseEd->Lise.bGetThickness =  true;
		if(LiseEd->Lise.bThreadActive == true)
		{ 
			// Cas ou la thread est active il va falloir synchroniser avec elle
			int c = 0;	// Variable que l'on va utiliser en temps que compteur pour ne pas rester bloquer dans la fonction
			LiseEd->Lise.bNThicknessNotReady = true;	// Variable qui va nous permettre d'attendre les n points intéressants
			LiseEd->Lise.WaitNThicknessStart = LiseEd->Lise.IndicePeriod;
			LiseEd->Lise.WaitNThicknessStop = LiseEd->Lise.IndicePeriod;
			LiseEd->Lise.iThicknessAboveQThreshold = 0;
			
			// rajout MA pour le traitement sur les la qualité
			LiseEd->Lise.iThicknessMatchingPeakMode = 0;
			LiseEd->Lise.iThicknessBestPeakMode = 0;
			// fin de modif MA
			logF(LiseEd, "[LISEED]\tWaiting for acquisition thread synchronization");

			clock_t clk_TimeStart = 0;
			clk_TimeStart = clock();

			// on déclenche le mode pour l'update de l'air gap
			LiseEd->Lise.iFirstMatchingSucces = true;
			
			// Tant que l'on n'a pas la permission de lecture et qu'on a pas eu les n points intéressants on reboucle
			while(LiseEd->Lise.bNThicknessNotReady == true) // Boucle d'attente des n points intéressants
			{ 
				// cas d'un TimeOut, on calcule et renvoie quand même des valeurs
				if(clock() - clk_TimeStart > Timeout)
				{
					// on incremente l'index sur la mesure
					INC_Index_Measure(LiseEd->Lise.iCounterMeasure);

					// get thickness single shot en timeout
                    return LEDGetSingleShotThickness(*LiseEd, Thickness, Quality, NbThicknesses); // TODO Sogilis: Ajouté par Julien
					//return LEDGetSingleShotThicknessWithTimeout(*LiseEd,Thickness,Quality,NbThicknesses);
				}

				// Attente de 1 ms
				Sleep(1);	
			}
		}
		else
		{
			LiseEd->Lise.bNeedRead = false;	// On change la valeur de bNeedRead
			logF(LiseEd, "[LISEED]\tThread not yet active DLL_OK - No Synchronisation with thread");
			LiseEd->Lise.bGetThickness = false;
			LiseEd->Lise.bReentrance = false;
			// on ne valide pas les valeur d'épaisseur dans le cas présent
			for(int k = 0;k<NbThicknesses;k++)
			{
				Thickness[k] = 0.0;
				Quality[k] = 0;	// on retourne une qualité de Zéro
			}
			for(int k = 0;k<NB_PEAK_MAX;k++)
			{
				LiseEd->Lise.LastAmplitudePics[k] = 0.0;
			}
			LiseEd->Lise.bReentrance = false;
			return DLL_OK;	// retour du status DLL_OK
		}

		// on incremente l'index sur la mesure
		INC_Index_Measure(LiseEd->Lise.iCounterMeasure);

        int RetGetSingleShot = LEDGetSingleShotThickness(*LiseEd, Thickness, Quality, NbThicknesses); // TODO Sogilis: Ajouté par Julien
		//int RetGetSingleShot = LEDGetSingleShotThicknessWithTimeout(*LiseEd,Thickness,Quality,NbThicknesses);

		// Check de la probe courante dans le cas d'un double Ed
		if(LiseEd->CurrentProbeFromParentStruct)
		{
	#ifdef _DEBUG
			if(LiseEd->ThisProbe != *LiseEd->CurrentProbeFromParentStruct)
			{
				BreakHook();
			}
	#endif
		}

		if(RetGetSingleShot != FP_OK) return RetGetSingleShot;
	}

	DefRestore(LiseEd->Lise.sample);
	LiseEd->Lise.iFirstMatchingSucces = false;
	LiseEd->Lise.bGetThickness = false;
	return DLL_OK;
}



// Get thicknesses
int LEDIGetThicknesses(void* s,double* Dates, double* Thicknesses, double* Quality,int NumValues)
{
	return FP_OK;
}
/*
// Get thickness
int LEDIGetThickness_Old(void* s,double* Thickness, double* Quality)
{ // On remplit un tableau d'épaisseurs de la dernière periode de la structure PERIOD_RESULT
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	int NbThicknesses = 0;
	int Timeout = 370;

	LISE_ED* LiseEd = (LISE_ED*)s;

	// on fait un clear des tableaux de qualité, thickness
	memset(Thickness,0,LiseEd->Lise.sample.NbThickness*sizeof(double));
	memset(Quality,0,LiseEd->Lise.sample.NbThickness*sizeof(double));

	// précondition, on ne fait pas de réentrance
	if(LiseEd->Lise.bReentrance == false)
	{
		// on définit la réentrance dans GetThickness
		LiseEd->Lise.bReentrance = true;
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tLEDIGetThickness - Start");
		}

		// précondition pour savoir si on a un tableau de thicnkess
		if (!Thickness)
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid parameter in LiseEDGetThickness");
			}

			// on relache la réentrance
			LiseEd->Lise.bReentrance = false;
			
			// retour de dll FAIL
			return DLL_FAIL;
		}

		// cas de l'échantillon défini
		if(LiseEd->Lise.sample.bDefined == true)
		{ 
			// Si l'échantillon est défini, alors on renvoie le nombre dépaisseur définit dans l'échantillon
			NbThicknesses = LiseEd->Lise.sample.NbThickness;
		}
		else
		{ 
			// Si l'échantillon n'est pas défini, on renvoie le nombre d'épaisseurs détectés dans le process
			NbThicknesses = LiseEd->Lise.Resultats[RBP_GetNMinusOne(LiseEd->Lise.IndicePeriod)].iNbThickness;
			
			// log de l'information
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tWarning - Sample not Define, please define a sample");
			}
		}

		//LiseEd->Lise.AcqMode = Continous;
		// mode continous acq
		if(LiseEd->Lise.AcqMode == Continous)
		{
			// log du mode continu dans le fichier de log
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\t---- Continous Mode define, return last valid value ----");
			}

			// on récupère le dernier indice de période à l'instant ou l'on a appelé la fonction GetThickness
			RING_BUFFER_POS IndexMoyenne = LiseEd->Lise.IndicePeriod;
			
			// commentaire MA pour modification juillet 2009 
			if(LiseEd->Lise.IndicePeriod.AbsN <= 20)
			{
				// pas assez de valeurs dispo pour fournir une mesure valide
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\tNot enough measurement available to average the measurement with the actual parameters.");
				}
				LiseEd->Lise.bReentrance = false;
				//Quality = 0;
				return DLL_OK;
			}

			// on recherche une première fois quelle mesure on va renvoyer, un best peak ou MatchingSucess
			int ValeursATester = 36;
			int ValeurCourante = 0;
			int NbBestPeak = 0;
			int NbMatchingSucess = 0;
			MATCHINGMODE ModeToUse;

			// boucle sur les dernière valeur à tester
			while(ValeurCourante < ValeursATester)
			{
				RBP_Dec(IndexMoyenne);
				PERIOD_RESULT LastMeasure = LiseEd->Lise.Resultats[IndexMoyenne.N];

				// test sur les dernière mesures pour savoir si on a plus de BestPeak ou Matching Sucess
				if(LastMeasure.MatchMode == BestPeak)	NbBestPeak++;
				else if(LastMeasure.MatchMode == MatchingSucess) NbMatchingSucess++;

				// on incrémente la valeur courante de boucle
				ValeurCourante++;
			}

			// cas ou on est sorti de la boucle et que l'on a aucun des deux modes qui s'est détaché, cas Impossible
			if(NbBestPeak < LiseEd->Lise.Moyenne && NbMatchingSucess < LiseEd->Lise.Moyenne)
			{
				// cas ou l'on n'a eu ni assez de BestPeak, ni de matching peak, alors on retourne une erreur (normalement se produit jamais)
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\tNot enough measures in CONTINOUS MODE available to average the measurement with the actual parameters");
				}
				LiseEd->Lise.bReentrance = false;
				return DLL_FAIL;
			}
			else if(NbBestPeak >= LiseEd->Lise.Moyenne)
			{
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tBestPeak");
				// on définit le mode à utiliser comme le bestPeak
				ModeToUse = BestPeak;
			}
			else if(NbMatchingSucess >= LiseEd->Lise.Moyenne)
			{
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tMoyenne");
				// on définit le mode à utiliser comme Matching Success
				ModeToUse = MatchingSucess;
			}

			// on positionne à nouveaux l'index sur la valeur de départ
			IndexMoyenne = LiseEd->Lise.IndicePeriod;

			// on va chercher les dernière mesures valide, en se limitant à un nombre définit de mesure pour ne pas rester bloquer dans la fonction
			double ThicknessMoyenne[MAX_THICKNESS]; // nombre de thickness max possible à mesurer
			double QualMoyenne[MAX_THICKNESS]; // nombre de thickness max possible à mesurer
			for(int k = 0; k< MAX_THICKNESS ;k++)	{ThicknessMoyenne[k] = 0.0;QualMoyenne[k] = 0.0;}	// on initialise le tableau à zéro

			//double QualityMoyenne = 0.0;	// Moyenne de la qualité
			int IndexError = 0;	// Paramètre permettant de mesures testées, bonne ou mauvaise
			int ValeursBonnes = 0;	// nombre de valeurs OK
			int NbThickness = 0;	// nombre d'épaisseurs

			// condition de sortie de la mesure, soit on a les 16 bonnes valeurs, soit un "TimeOut" de 20 mesures non valides ou pas
			while((ValeursBonnes+1) <= LiseEd->Lise.Moyenne)
			{
				// on commence par décrémenter l'index, bon pour la première mesure car la première mesure valide est a GetNMinusOne 
				RBP_Dec(IndexMoyenne);

				// on récupère la dernière mesure valide
				PERIOD_RESULT LastNMeasure = LiseEd->Lise.Resultats[IndexMoyenne.N];

				// on récupère le nombre d'épaisseurs
				NbThickness = LastNMeasure.iNbThickness;
				
				// On va tester si la mesure est bonne => suppérieure à la qualité
				if(NbThickness > 0 && ModeToUse == LastNMeasure.MatchMode)
				{ 
					// si la mesure est valide alors on l'ajoute au tableau de résultats
					for(int i = 0;i<NbThickness;i++)
					{			
						LogfileF(*LiseEd->Lise.Log, "[LISEED]\tAcqMode Continuous i=%i ThicknessMoyenne=%.3f QualMoyenne=%.3f",LastNMeasure.fThickness[i],LastNMeasure.fQuality);
						ThicknessMoyenne[i] += LastNMeasure.fThickness[i];
						QualMoyenne[i] += LastNMeasure.fQuality;//approximation tous les pics de même qualité
					}

					// on incrémente le compteur de valeurs bonnes
					ValeursBonnes++;
				}

				// index survant à surveiller le nombre de mesures max à laisser passer
				IndexError++;

				// cas ou l'on a pas trouvé 2*Moyenne résultats valide, il y a un problème et on sort
				if(IndexError >= 2*LiseEd->Lise.Moyenne)
				{
					if(LiseEd->Lise.bDebug == true)
					{
						LogfileF(*LiseEd->Lise.Log,"[LISEED]\tNot all thickness valid found in Continous GetThickness, can't find %d measures with the same matching mode",LiseEd->Lise.Moyenne);
					}
					LiseEd->Lise.bReentrance = false;

					// on retourne une qualité de zéro
					return DLL_FAIL;
				}
			}

			// on teste et vérifie que le nombre de mesure valide est en accords avec la moyenne
			if(ValeursBonnes != LiseEd->Lise.Moyenne)
			{
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\tNumber of good value found is different of average parameter");
				}
				LiseEd->Lise.bReentrance = false;
				return DLL_FAIL;
			}
			int i = 0;

			// on copie les n valeurs de thickness trouvé
			for(i=0;i<NbThickness;i++)
			{
				Thickness[i] = ThicknessMoyenne[i] / LiseEd->Lise.Moyenne;
				Quality[i] = QualMoyenne[i] / LiseEd->Lise.Moyenne;
				//LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINOUS]\tQualMoyenne %i: %f",i,Thickness[i]);
				//LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINOUS]\tThicknessMoyenne %i: %f",i,QualMoyenne[i]);
			}

			// on loggue les info
			//if(LiseEd->Lise.bDebug == true)
			//{
			//	LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINOUS]\t-------------------");
			//	LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINOUS]\tNumber of thickness: %i",NbThickness);
			//	LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINOUS]\tQuality: %f",Quality[0]);
			//	for(int i =0;i<8;i++) LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINOUS]\tThickness %i: %f",i,Thickness[i]);
			//}

			if(LiseEd->Lise.bDebug == true)
			{
				for(i=0;i<LiseEd->Lise.sample.NbThickness;i++) LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINUOUS]\tThicknessMoyenne %i: %f",i,Thickness[i]);
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[CONTINUOUS]\tQuality value = %f",Quality[0]);
			}

			// booléen de réentrance à false pour pouvoir à nouveau entrer dans la fonction
			LiseEd->Lise.bReentrance = false;

			// retour de la fonction
			return DLL_OK;
		}
		else if(LiseEd->Lise.AcqMode == SingleShot)
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\t---- Single shot Mode define, return n measure valid----");
			}
			// mode single shot, on attend une mesure moyennée sur n mesures
			LiseEd->Lise.bGetThickness =  true;
			if(LiseEd->Lise.bThreadActive == true)
			{ // Cas ou la thread est active il va falloir synchroniser avec elle
				int c = 0;	// Variable que l'on va utiliser en temps que compteur pour ne pas rester bloquer dans la fonction
				LiseEd->Lise.bNThicknessNotReady = true;	// Variable qui va nous permettre d'attendre que l'on d'attendre les n points intéressant
				LiseEd->Lise.WaitNThicknessStart = LiseEd->Lise.IndicePeriod;
				LiseEd->Lise.WaitNThicknessStop = LiseEd->Lise.IndicePeriod;

				LiseEd->Lise.iThicknessAboveQThreshold = 0;
				// rajout MA pour le traitement sur les la qualité
				LiseEd->Lise.iThicknessMatchingPeakMode = 0;
				LiseEd->Lise.iThicknessBestPeakMode = 0;
				// fin de modif MA
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log, "[LISEED]\tWaiting for acquisition thread synchronization");
				}
				clock_t clk_TimeStart = 0;

				clk_TimeStart = clock();

				// Tant que l'on n'a pas la permission de lecture et qu'on a pas eu les n points intéressant on reboucle
				while(LiseEd->Lise.bNThicknessNotReady == true) // Boucle d'attente des n points intéressant
				{ 
					// cas d'un TimeOut
					if(clock() - clk_TimeStart > Timeout)
					{
						Quality = 0;	// on retourne une qualité de Zéro
						LiseEd->Lise.bNeedRead = false;	// On change la valeur de bNeedRead
						if(LiseEd->Lise.bDebug == true)
						{
							LogfileF(*LiseEd->Lise.Log, "[LISEED]\tReturning Gethickness with no Thickness and DLL_OK - No Synchronisation with thread");
						}
						LiseEd->Lise.bReentrance = false;
						LiseEd->Lise.bGetThickness = false;
						
						// on initialise le tableau d'épaisseur
						{
							for(int k=0;k<8;k++)	
							{
								Thickness[k] = 0.0;
								LiseEd->Lise.LastAmplitudePics[k] = 0.0;
							}
						}
						
						// on réinitialise le tableau d'épaisseur
						RING_BUFFER_POS IndiceCalculMoyenne = LiseEd->Lise.WaitNThicknessStop;
						
						// on calcule les n valeurs d'épaisseurs
						int CompteurMoyenneBestPeak = 0;
						
						// tableau des épaisseurs des bestPeak
                        double BestPeakThickness[MAX_PEAKS];
						{for(int i = 0;i<MAX_PEAKS;i++) BestPeakThickness[i] = 0.0;}
						
						// compteur des n valeurs de thcikness
						int CompteurMoyenneMatchingPeak = 0;
						
						// tableau des épaisseurs des MatchingPeak
						double MatchingSucessThickness[MAX_PEAKS];
						{for(int i = 0;i<MAX_PEAKS;i++) MatchingSucessThickness[i] = 0.0;}

						// indice qui va pointer sur le buffer de résultat entre le ThicknessStart et Stop
						while(IndiceCalculMoyenne.AbsN >= LiseEd->Lise.WaitNThicknessStart.AbsN)
						{
							// on décrémente l'indice de décimation
							RBP_Dec(IndiceCalculMoyenne);
							if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].MatchMode == BestPeak)
							{
								LogfileF(*LiseEd->Lise.Log, "[LISEED]\tMode best peak");
								// on ajoute l'épaisseur courante aux épaisseur moyenne
								for (int cptThick=0; cptThick<LiseEd->Lise.sample.NbThickness; cptThick++)
								{
									BestPeakThickness[cptThick] += LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[cptThick];
									// mise à jour des valeurs d'amplitude
									LiseEd->Lise.LastAmplitudePics[cptThick] += cptThick;//(LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Intensite + LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Intensite) / 2.0;
									LiseEd->Lise.LastPositionPicsPlus[cptThick] += LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Position;
									LiseEd->Lise.LastPositionPicsMoins[cptThick] += LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Position;
									// log du pic
									//LogfileF(*LiseEd->Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tA%i :\t%.3f\t%.3f\t%.3f\t%.3f",
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Intensite,
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Intensite,
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Position,
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Position );
								}

								// on incrémente le compteur de valeur bonne pour faire une moyenne
								CompteurMoyenneBestPeak++;
							}
							else if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].MatchMode == MatchingSucess)
							{
								LogfileF(*LiseEd->Lise.Log, "[LISEED]\tMode Matching success");
								// on ajoute l'épaisseur courante aux épaisseur moyenne
								for (int cptThick=0; cptThick<LiseEd->Lise.sample.NbThickness; cptThick++)
								{
									MatchingSucessThickness[cptThick] += LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[cptThick];

									// mise à jour des valeurs d'amplitude
									LiseEd->Lise.LastAmplitudePics[cptThick] += cptThick + 100; //(LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Intensite + LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Intensite) / 2.0;
									// log du pic
									//LogfileF(*LiseEd->Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tA%i :\t%.3f\t%.3f\t%.3f\t%.3f",
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Intensite,
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Intensite,
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Position,
									//	LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Position );
								}

								// on incrémente le compteur de valeur bonne pour faire une moyenne
								CompteurMoyenneMatchingPeak++;
							}
						}
						{for(int k=0;k<LiseEd->Lise.sample.NbThickness;k++)
						{
							if(CompteurMoyenneBestPeak >= CompteurMoyenneMatchingPeak && CompteurMoyenneBestPeak > 0)
							{
								Thickness[k] = BestPeakThickness[k]/(double)CompteurMoyenneBestPeak;
								LiseEd->Lise.LastAmplitudePics[k]/=(double)CompteurMoyenneBestPeak;
								// log du pic
								//LogfileF(*LiseEd->Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tBP A%i : %f",k,LiseEd->Lise.LastAmplitudePics[k]);

							}
							else if(CompteurMoyenneBestPeak < CompteurMoyenneMatchingPeak && CompteurMoyenneMatchingPeak > 0)
							{
								Thickness[k] = MatchingSucessThickness[k]/(double)CompteurMoyenneMatchingPeak;
								LiseEd->Lise.LastAmplitudePics[k]/=(double)CompteurMoyenneMatchingPeak;
								// log du pic
								//LogfileF(*LiseEd->Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tBP A%i : %f",k,LiseEd->Lise.LastAmplitudePics[k]);
							}
							else
							{
								Thickness[k] = 0.0;
								LiseEd->Lise.LastAmplitudePics[k] =0.0;
							}
						}}
						
						// retour du status DLL_OK
						return DLL_OK;	
					}

					// Attente de 1 ms
					Sleep(1);	
				}
			}
			else
			{
				Quality = 0;	// on retourne une qualité de Zéro
				LiseEd->Lise.bNeedRead = false;	// On change la valeur de bNeedRead
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log, "[LISEED]\tThread not yet active DLL_OK - No Synchronisation with thread");
				}
				LiseEd->Lise.bGetThickness = false;
				LiseEd->Lise.bReentrance = false;
				// on ne valide pas les valeur d'épaisseur dans le cas présent
				for(int k = 0;k<NbThicknesses;k++)
				{
					Thickness[k] = 0.0;
				}
                for(int k = 0;k<NB_PEAK_MAX;k++)
				{
					LiseEd->Lise.LastAmplitudePics[k] = 0.0;
				}
				return DLL_OK;	// retour du status DLL_OK
			}
			
			// parametre pour la thread, demande de lecture
			LiseEd->Lise.bNeedRead = true;	
			// on teste si la thread est active
			if(LiseEd->Lise.bThreadActive == true)
			{
				if(LiseEd->Lise.bDebug)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\t Waiting Synchronisation with thread");
				}	
				int c=0;
				clock_t TimeStart = clock();
				// parametre pour la thread, on attend la permission de lire
				while(!LiseEd->Lise.bReadAllowed)	
				{
					if(clock() - TimeStart > Timeout)
					{
						LiseEd->Lise.bNeedRead = false;
						if(LiseEd->Lise.bDebug)
						{
							LogfileF(*LiseEd->Lise.Log,"[LISEED]\tTimeout of thread, returning of GetThickness with DLL_OK");
						}
						LiseEd->Lise.bReentrance = false;
						LiseEd->Lise.bGetThickness = false;
						// on ne valide pas les valeur d'épaisseur dans le cas présent
				        for(int k = 0;k<NbThicknesses;k++)
				        {
					        Thickness[k] = 0.0;
				        }
                        for(int k = 0;k<NB_PEAK_MAX;k++)
				        {
					        LiseEd->Lise.LastAmplitudePics[k] = 0.0;
				        }
						return DLL_OK;
					}
					Sleep(1);
				}
			}

			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tFind N thickness with good quality");
			}
			// début de modif
			// MA 20/02/2009 on calcule la valeur de qualité par rapport au mode de matching
			// on recherche une première fois quelle mesure on va renvoyer, un best peak ou MatchingSucess
			int ValeursATester = 36;
			int ValeurCourante = 0;
			int NbBestPeak = 0;
			int NbMatchingSucess = 0;
			MATCHINGMODE ModeToUse;
			RING_BUFFER_POS IndexMoyenne = LiseEd->Lise.WaitNThicknessStop;
			while(ValeurCourante < ValeursATester)
			{
				RBP_Dec(IndexMoyenne);
				PERIOD_RESULT LastMeasure = LiseEd->Lise.Resultats[IndexMoyenne.N];

				// test sur les dernière mesures pour savoir si on a plus de BestPeak ou Matching Sucess
				if(LastMeasure.MatchMode == BestPeak)	NbBestPeak++;
				else if(LastMeasure.MatchMode == MatchingSucess) NbMatchingSucess++;

				// on incrémente la valeur courante de boucle
				ValeurCourante++;
			}
			if(NbBestPeak < 16 && NbMatchingSucess < 16)
			{
				// cas ou l'on n'a eu ni assez de BestPeak, ni de matching peak, alors on retourne une erreur (normalement se produit jamais)
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\tNot enough measures available in SINGLE SHOT MEASUREMENT to average the measurement with the actual parameters");
				}
				LiseEd->Lise.bReentrance = false;

				// on ne valide pas les valeur d'épaisseur dans le cas présent
				for(int k = 0;k<8;k++)	
				{
					Thickness[k] = 0.0;
					LiseEd->Lise.LastAmplitudePics[k] = 0.0;
				}

				// qualité nulle
				return DLL_FAIL;
			}
			else if(NbBestPeak >= 16) ModeToUse = BestPeak;
			else if(NbMatchingSucess >= 16) ModeToUse = MatchingSucess;

			// on initialise le tableau des thickness moyenne
			double Moyenne[NB_PEAK_MAX];
			for (int i=0; i<NB_PEAK_MAX; i++)
			{
				Moyenne[i] = 0.0;
				LiseEd->Lise.LastAmplitudePics[i] = 0.0;
			}

			// on initilise la valeur de qualité moyenne
			double moyQual = 0.0;
			RING_BUFFER_POS IndiceCalculMoyenne;
			IndiceCalculMoyenne = LiseEd->Lise.WaitNThicknessStop;
			if(LiseEd->Lise.bDebug)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tStart index :%d Stop Index : %d, Average Value = %d ",LiseEd->Lise.WaitNThicknessStart.N,LiseEd->Lise.WaitNThicknessStop.N,LiseEd->Lise.Moyenne);
			}
			
			int cptThick;

			int CompteurMoyenne = 0;
			int CompteurError = 0;
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tQuality Threshold in Sample Definition %f",LiseEd->Lise.sample.QualityThreshold);
			}

			// modif de la condition while MA 22/01/2009
			RBP_Inc(IndiceCalculMoyenne);

			// booléen poru surveiller si on a les n valeurs au dessus du seuil de qualité
			bool bAllValueAboveQTh = true;

			// boucle permettant d'aller tester les n épaisseur contenues entre Start et Stop
			while(IndiceCalculMoyenne.AbsN >= LiseEd->Lise.WaitNThicknessStart.AbsN)
			{
				// on décrémente l'indice de décimation
				RBP_Dec(IndiceCalculMoyenne);
				if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].MatchMode == ModeToUse && CompteurMoyenne < 16)
				{
					// on ajoute l'épaisseur courante aux épaisseur moyenne
					for (cptThick=0; cptThick<NbThicknesses; cptThick++)
					{
						Moyenne[cptThick] += LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[cptThick];

						// mise à jour des valeurs d'amplitude
						LiseEd->Lise.LastAmplitudePics[cptThick] += (LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[cptThick].Intensite + LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[cptThick].Intensite) / 2.0;
						// log du pic
						//LogfileF(*LiseEd->Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tA%i : %f",cptThick,LiseEd->Lise.LastAmplitudePics[cptThick]);
					}

					// on incrémente le compteur de valeur bonne pour faire une moyenne
					CompteurMoyenne++;

					// On ajoute la valeur à la moyenne de Qualité
					moyQual += LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fQuality;

					// Si une valeur du seuil de qualité n'est pas bonne alors on change le booléen de surveillance
					if(LiseEd->Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fQuality <= LiseEd->Lise.sample.QualityThreshold)
					{
						bAllValueAboveQTh = false;
					}
				}

				// on incrémente le compteur d'erreur
				CompteurError++;
			}

			// modif MA 22/01/2009
			if(LiseEd->Lise.Moyenne > CompteurMoyenne) // Pour sortir de la boucle
			{ // Cas ou l'on sort de la fonction pour ne pas rester bloquer dans le while
				LiseEd->Lise.bNeedRead = false;
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log, "Start in buffer resultats : %i",LiseEd->Lise.WaitNThicknessStart.AbsN);
					LogfileF(*LiseEd->Lise.Log, "Stop in buffer resultats : %i",LiseEd->Lise.WaitNThicknessStop.AbsN);
					LogfileF(*LiseEd->Lise.Log, "[LISEED]\tCan't find N thickness with quality superior or equivalent to Quality threshold");
					for(int k = 0;k<(LiseEd->Lise.WaitNThicknessStop.AbsN - LiseEd->Lise.WaitNThicknessStart.AbsN);k++)
					{
						int indice = LiseEd->Lise.WaitNThicknessStart.N + k;
						if (indice > LiseEd->Lise.WaitNThicknessStart.Len)	indice = k;
						//LogfileF(*LiseEd->Lise.Log, "[LISEED]\tQuality Value n%i : %f",k,LiseEd->Lise.Resultats[indice].fQuality);	
					}
				}
				LiseEd->Lise.bReentrance = false;
				LiseEd->Lise.bGetThickness = false;
				// on ne valide pas les valeur d'épaisseur dans le cas présent
				for(int k = 0;k<NbThicknesses;k++)	Thickness[k] = 0.0;
				return DLL_FAIL;
			}
			// fin de modif Ma

			// On print dans le fichier les peaks utilisés pour sortir l'épaisseur
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tN Thickness find for quality average");
			}

			// MA 22/01/2009 pour debug
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log, "Start in buffer resultats : %i",LiseEd->Lise.WaitNThicknessStart.AbsN);
				LogfileF(*LiseEd->Lise.Log, "Stop in buffer resultats : %i",LiseEd->Lise.WaitNThicknessStop.AbsN);
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tFind N thickness with quality superior or equivalent to Quality threshold");
				for(int k = 0;k<(LiseEd->Lise.WaitNThicknessStop.AbsN - LiseEd->Lise.WaitNThicknessStart.AbsN);k++)
				{
					int indice = LiseEd->Lise.WaitNThicknessStart.N + k;
					if (indice > LiseEd->Lise.WaitNThicknessStart.Len)	indice = k;
				}
			}

			// MA 22/01/2009 fin de pour debug
			//for (cptThick=0; cptThick<NbThicknesses; cptThick++)
			//{
			//	Thickness[cptThick] = Moyenne[cptThick] / (double)CompteurMoyenne;
			//	LiseEd->Lise.LastAmplitudePics[cptThick] = LiseEd->Lise.LastAmplitudePics[cptThick] / (double)CompteurMoyenne;
			//	char temp[LONG_STR];
			//	sprintf(temp, "[LISEED]\tThickness[%i] = %f", cptThick, Thickness[cptThick]);
			//	if(LiseEd->Lise.bDebug == true)
			//	{
			//		LogfileF(*LiseEd->Lise.Log,temp);
			//	}
			//	// pour debug de l'amplitude pic
			//	sprintf(temp, "[LISEED]\tLiseEd->Lise.LastAmplitudePics[%i] = %f", cptThick, LiseEd->Lise.LastAmplitudePics[cptThick]);
			//	if(LiseEd->Lise.bDebug == true)
			//	{
			//		LogfileF(*LiseEd->Lise.Log,temp);
			//	}
			//	// log du pic
			//	LogfileF(*LiseEd->Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tA%i : %f",cptThick,LiseEd->Lise.LastAmplitudePics[cptThick]);
			//}
			// pour debug
			double A1 = LiseEd->Lise.LastAmplitudePics[0];
			double A2 = LiseEd->Lise.LastAmplitudePics[1];

			if(CompteurMoyenne == 0)
			{
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\tGetThickness Fail - Impossible to divide by Zero");
				}
				LiseEd->Lise.bNeedRead = false;
				LiseEd->Lise.bReentrance = false;
				LiseEd->Lise.bGetThickness = false;
				return DLL_FAIL;
			}	
			for (cptThick=0; cptThick<NbThicknesses; cptThick++)
			{//approximation: tous les pics de même qualite
				Thickness[cptThick] = Moyenne[cptThick] / (double)CompteurMoyenne;
				Quality[cptThick] = moyQual / (double)CompteurMoyenne;
			}
			//	// on teste le cas ou on a pas eu n valeurs au dessus du seuil de qualité et que la valeur moyenne soit supérieure au seuil de qualité
			//	if(bAllValueAboveQTh ==false && Quality >= LiseEd->Lise.sample.QualityThreshold)
			//	{
			//		Quality = LiseEd->Lise.sample.QualityThreshold;
			//	}

			LiseEd->Lise.bNeedRead = false;

			if(LiseEd->Lise.bDebug == true)
			{
				// chaine de caractère temporaire pour écrire le message
				char messTemp[LONG_STR];

				sprintf(messTemp, "[LISEED]\tNumber of thickness returned : %i", NbThicknesses);
				LogfileF(*LiseEd->Lise.Log, messTemp);
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tQuality Value %f",Quality[0]);
				LogfileF(*LiseEd->Lise.Log, "[LISEED]\tLEDIGetThickness - End");
			}

			// on relache la réentrance
			LiseEd->Lise.bReentrance = false;
		}
	}
	else
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tCan't enter in function GetThickness again - End");
		}
		LiseEd->Lise.bGetThickness = false;
		return DLL_FAIL;
	}
	LiseEd->Lise.bGetThickness = false;
	return DLL_OK;
}
*/
// Define sample
int LEDIDefineSample(void* s,char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int NbThickness, double Gain, double QualityThreshold)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}


	LISE_ED* LiseEd = (LISE_ED*)s;
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIDefineSample - Start");
	}

// Synchronisation avec la thread	
	LiseEd->Lise.bNeedRead = true;	// parametre pour la thread
	if(LiseEd->Lise.bDebug)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tEntering LiseEDDefineSample");
	}	
	if(LiseEd->Lise.bThreadActive == true)
	{
		if(LiseEd->Lise.bDebug)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\t Waiting Synchronisation with thread");
		}	
		int c=0;
		clock_t TimeStart = clock();
		while(!LiseEd->Lise.bReadAllowed)	// parametre pour la thread
		{
			SPG_MemFastCheck();
			Sleep(10);
			if(clock() - TimeStart > 370)
			{
				LiseEd->Lise.bNeedRead = false;
				if(LiseEd->Lise.bDebug)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[ERROR]\tTimeout of thread, returning of DefineSample with FP_FAIL");
				}	
				return FP_FAIL;
			}
		}
	}
	if(LiseEd->Lise.bDebug)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\t Synchronisation with thread in LEDIDefineSample Success");
	}	

	char temp[LONG_STR];
	char temp2[LONG_STR];
	sprintf(temp,"[LISEED]\tGain=%f, QualityThreshold=%f",(float)Gain,(float)QualityThreshold);
	sprintf(temp2,"[LISEED]\tNAME =%s",Name);
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log, temp);
		LogfileF(*LiseEd->Lise.Log, temp2);
	}

	double LambdaSurQuatre = LiseEd->Lise.dRefWaveLengthNm / 4.0 / 1000.0;
	DefClear(LiseEd->Lise.sample);

	if (strcmp(Name, "NOSAMPLE") == 0)
	{ // Si NOSAMPLE est défini, alors on veut une détection automatique de l'épaisseur, on aura une mauvaise qualité
		if (NbThickness > NB_PEAK_MAX-1)
		{
			char temp[1024];
			sprintf(temp,"[LISEED]\tToo many layers in LISEEDDefineSample, greater than %d", NB_PEAK_MAX-1);
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,temp);
			}
			LiseEd->Lise.sample.bDefined = false;
			LiseEd->Lise.bNeedRead = false;
			return DLL_FAIL;
		}
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, "[LISEED]\tSample not defined");
		}

		LiseEd->Lise.sample.bDefined = true;
		LiseEd->Lise.sample.NbThickness = NbThickness;

		// Définition du premier pic.
		if (LiseEd->iRefOpt)
		{
			// Avec pic de référence, la position et la tolérance du premier pic sont définies dans le fichier de config.
			DefAdd(LiseEd->Lise.sample,PkOptRef,PkRefAbsolute,LiseEd->fPositionRefOpt / LambdaSurQuatre,LiseEd->fToleranceRefOpt / LambdaSurQuatre,WeightUniform);
		}
		else
		{
			// Sans pic de référence, le premier pic est défini au centre de la profondeur de champ, avec la tolérance la plus large possible.
			DefAdd(LiseEd->Lise.sample,PkSample,PkRefAbsolute,(LiseEd->Lise.Range/2.0) / LambdaSurQuatre,(LiseEd->Lise.Range/2.0) / LambdaSurQuatre,WeightUniform);
		}
		// Définition des pics suivants.
		for(int i = 0;i< NbThickness;i++)
		{
			LiseEd->Lise.sample.fIndex[i] = 1;
			LiseEd->Lise.sample.fIntensity[i] = 0;
			PEAKWEIGHT pkwt;
			if (Intensity[i] == 0.0) pkwt = WeightUniform;
			else if (Intensity[i] == 1.0) pkwt = WeightCos;
			else pkwt = WeightPeak;
			DefAdd(LiseEd->Lise.sample,PkSample,PkRefPrevious,(LiseEd->Lise.Range/2.0) / LambdaSurQuatre,(LiseEd->Lise.Range/2.0) / LambdaSurQuatre,pkwt);
		}

		//if (Gain != -1.0)
		if(Gain >= 0.0)	// On en prends pas en compte les valeurs -1.0 et -2.0
		{
			LiseEd->Lise.fSourceValue = Gain;
			LiseEd->Lise.sample.QualityThreshold = QualityThreshold;

			// en double Lise Ed double, on ne met pas a jour, c'est le start qui s'en charge 
			if(LiseEd->DblEdMeasurementMode != MeasBothProbe)  LEDISetSourcePower(s,LiseEd->Lise.fSourceValue);
		}
		if (LiseEd->Lise.sample.NumPks != NbThickness+1)
		{
			LiseEd->Lise.sample.bDefined = false;
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid sample definition, number of peaks different from number of interfaces.");
			}
			LiseEd->Lise.bNeedRead = false;
			return DLL_FAIL;
		}
	}
	else
	{
		if (NbThickness > NB_PEAK_MAX-1)
		{
			char temp[1024];
			sprintf(temp,"[LISEED]\tToo many layers in LiseEDDefineSample, greater than %d", NB_PEAK_MAX-1);
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,temp);
			}
			LiseEd->Lise.sample.bDefined = false;
			LiseEd->Lise.bNeedRead = false;
			return DLL_FAIL;
		}
		else if (NbThickness < 1)
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tZero layer in LISEEDDefineSample");
			}
			LiseEd->Lise.sample.bDefined = false;
			LiseEd->Lise.bNeedRead = false;
			return DLL_FAIL;
		}
		
		LiseEd->Lise.sample.bDefined = true;
		sprintf(LiseEd->Lise.sample.sName, "%s", Name);
		sprintf(LiseEd->Lise.sample.sSampleNumber, "%s", SampleNumber);
		LiseEd->Lise.sample.NbThickness = NbThickness;
		int i;
		for (i=0; i< NbThickness; i++)
		{
			LiseEd->Lise.sample.fIndex[i] = Index[i];
			LiseEd->Lise.sample.fIntensity[i] = Intensity[i];
			char temp[LONG_STR];
			sprintf(temp, "[LISEED]\tLayer %i: thickness=%fµm; tolerance=%fµm; index=%f; type=%f", i, Thickness[i], Tolerance[i], Index[i], Intensity[i]);
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log, temp);
			}
		}
		//if (Gain != -1.0)
		if(Gain >= 0.0)	// On en prends pas en compte les valeurs -1.0 et -2.0
		{
			LiseEd->Lise.fSourceValue = Gain;
			LiseEd->Lise.sample.QualityThreshold = QualityThreshold;
			//LEDISetSourcePower(s,LiseEd->Lise.fSourceValue);

			// en double Lise Ed double, on nr met pas a jour, c'est le start qui s'en charge 
			if(LiseEd->DblEdMeasurementMode != MeasBothProbe) LEDISetSourcePower(s,LiseEd->Lise.fSourceValue);
		}
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log, temp);
		}

		// Définition du premier pic.
		if (LiseEd->iRefOpt)
		{
			// Avec pic de référence, la position et la tolérance du premier pic sont définies dans le fichier de config.
			DefAdd(LiseEd->Lise.sample,PkOptRef,PkRefAbsolute,LiseEd->fPositionRefOpt / LambdaSurQuatre,LiseEd->fToleranceRefOpt / LambdaSurQuatre,WeightUniform);
		}
		else
		{
			// Sans pic de référence, le premier pic est défini au centre de la profondeur de champ, avec la tolérance la plus large possible.
			DefAdd(LiseEd->Lise.sample,PkSample,PkRefAbsolute,(LiseEd->Lise.Range/2.0) / LambdaSurQuatre,(LiseEd->Lise.Range/2.0) / LambdaSurQuatre,WeightUniform);
		}
		// Définition des pics suivants.
		for (i=0; i < NbThickness; i++)
		{
			PEAKWEIGHT pkwt;
			if (Intensity[i] == 0.0) pkwt = WeightUniform;
			else if (Intensity[i] == 1.0) pkwt = WeightCos;
			else pkwt = WeightPeak;
			DefAdd(LiseEd->Lise.sample,PkSample,PkRefPrevious,Thickness[i] * Index[i] / LambdaSurQuatre,Tolerance[i] * Index[i] / LambdaSurQuatre,pkwt);
		}

		if (LiseEd->Lise.sample.NumPks != NbThickness+1)
		{
			LiseEd->Lise.sample.bDefined = false;
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tInvalid sample definition, number of peaks different from number of interfaces.");
			}
			LiseEd->Lise.bNeedRead = false;
			return DLL_FAIL;
		}
	}

	// backup de la définition de l'échantillon
	LiseEd->Lise.bSampleDefUpdated = false;
	DefBackup(LiseEd->Lise.sample);

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIDefineSample - End");
	}
	LiseEd->Lise.bNeedRead = false;
	return DLL_OK;
}


int LEDISetStagePositionInfo(void* s,double* XSystemPosition, double* YSystemPosition, double *ZSystemPosition)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	LISE_ED* LiseEd = (LISE_ED*)s;
	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIGetStagePosition- Start");
	}

	// On mémorise les information de sur X, Y et Z dans la structure
	LiseEd->Lise.XSystemPos = *XSystemPosition;
	LiseEd->Lise.YSystemPos = *YSystemPosition;
	LiseEd->Lise.ZSystemPos = *ZSystemPosition;

	if(LiseEd->Lise.bDebug == true)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\tLEDIGetStagePosition- Stop");
	}
	return DLL_OK;
}

//####################################### Sauvegardes ######################################
// Sauve les resultats de waveform en fichier texte
int LEDISaveWaveForm(void* s,char* FileName,float* StepX)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	LISE_ED* LiseEd = (LISE_ED*)s;
	
	LiseEd->Lise.bNeedRead = true;	// parametre pour la thread
	if(LiseEd->Lise.bThreadActive == true)
	{
		while(!LiseEd->Lise.bReadAllowed)	// parametre pour la thread
		{
			Sleep(1);
		}
	}

	FILE* fichier = fopen(FileName, "wb");
	fprintf(fichier,"Sauvegarde de données du Signal - LiseEDAcqSaveWaveForm\r\n");

	fprintf(fichier, "Period: %i\r\n", LiseEd->Lise.Periode-1);

	RING_BUFFER_POS SaveLastPeriodIndex;
	SaveLastPeriodIndex = LiseEd->Lise.PulsePlusLeft;
	int NbSamples = LiseEd->Lise.NbSamplesLastPeriod;
	RBP_Sub(SaveLastPeriodIndex,NbSamples);
	// Pour sauvegarder la dernière periode de Signal
	while(SaveLastPeriodIndex.AbsN<LiseEd->Lise.PulsePlusLeft.AbsN)
	{
		char ma_chaine[128];
		
		sprintf(ma_chaine,"%f",LiseEd->Lise.BufferIntensity[SaveLastPeriodIndex.N]);
		// On remplace le point par une virgule pour le fichier de sortie	
		for(int i = 0;i<sizeof(ma_chaine);i++)
		{
			if(ma_chaine[i] == '.')	
			{
				ma_chaine[i] = ',';
			}
		}

		fprintf(fichier,"%s\r\n",ma_chaine);
		RBP_Inc(SaveLastPeriodIndex);
	}
	fclose(fichier);
	*StepX = (float)LiseEd->Lise.dRefWaveLengthNm / 4.0;
	LiseEd->Lise.bNeedRead = false;

	return DLL_OK;
}

// Fonction permettant de copier tout le siganl dans un fichier texte depuis le début de l'acquisition
int LEDISavePeaks(void* s,char* FileName)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}

	LISE_ED* LiseEd = (LISE_ED*)s;

	// on va créer le fichier dans le répertoire qui va bien
	char AbsModulePath[512]; char FileNamePath[512]; 
#ifdef FDE
	GetModuleFileName(GetModuleHandle("FogaleProbe.dll"),AbsModulePath,512);
	SPG_PathOnly(AbsModulePath);
#else
	wchar_t path[512];
	GetModuleFileName(GetModuleHandle(NULL), path, 512);
	strcpy_s(AbsModulePath, fdstring(&path[0]));
#endif
	SPG_ConcatPath(FileNamePath,AbsModulePath,LiseEd->Lise.FileNameSavePeaks);
	
	if(LiseEd->Lise.bAllowSavePeaks == true)
	{		
		if(!LiseEd->Lise.FileNamesaveThickness)
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tWarning - File Name is not define in config file for function LiseEDSavePeaks");
			}
			char* FileSavePeaks = "ThicknessDefaultName.txt";
			strcpy(AbsModulePath,FileSavePeaks);
		}
		else
		{
			// on va copier le nouveau nom de répertoire dans la variable de la structure
			strcpy(AbsModulePath,FileNamePath);
		}

		LiseEd->Lise.FichierSavePics = fopen(AbsModulePath, "wb");

		SYSTEMTIME systime;
		GetSystemTime(&systime);

		// on loggue le nom du chier de sauvegarde ainsi que sa date de création
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tFile save tickness: %s",LiseEd->Lise.FileNameSavePeaks);
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tFile Created: %i-%i-%i \t %i : %i : %i",systime.wDay,systime.wMonth,systime.wYear,systime.wHour + 1,systime.wMinute,systime.wSecond);		
		}
		// Attention pour l'heure c'est +1 pour l'heure d'hivers et +2 pour l'heure d'été
		fprintf(LiseEd->Lise.FichierSavePics,"%i-%i-%i \t %i : %i : %i \r\n",systime.wDay,systime.wMonth,systime.wYear,systime.wHour + 1,systime.wMinute,systime.wSecond);

		// infos sur l'échantillons
		if (LiseEd->Lise.sample.bDefined)
		{
			fprintf(LiseEd->Lise.FichierSavePics,"Sample Name: %s\r\n", LiseEd->Lise.sample.sName);
			fprintf(LiseEd->Lise.FichierSavePics,"Sample Number: %s\r\n", LiseEd->Lise.sample.sSampleNumber);
		}

		if(LiseEd->Lise.WritePeaksForCalibration == 1)
		{
			fprintf(LiseEd->Lise.FichierSavePics,"Period\tDir\tPic\tPosition\tQuality\t...");
		}
		else
		{
			fprintf(LiseEd->Lise.FichierSavePics,"Period\tDir\tPic\tPosition\tQuality\t\tIntensity...");
		}

		LiseEd->Lise.FlagSavePics = true;
		
		return DLL_OK;
	}
	else
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tSave Peaks Success, but the configuration don't allow peaks save");
		}
		return DLL_OK;
	}
}

// fonction permettant la sauvegarde des epaisseurs dans les deux sens
int LEDIAcqSaveThickness(void* s,char* FileName)
{
	// Precondition
	if (!s)
	{
		return DLL_FAIL;
	}
	

	LISE_ED* LiseEd = (LISE_ED*)s;

	// on va créer le fichier dans le répertoire qui va bien
	char AbsModulePath[512]; char FileNamePath[512];
#ifdef FDE
	GetModuleFileName(GetModuleHandle("FogaleProbe.dll"),AbsModulePath,512);
	SPG_PathOnly(AbsModulePath);
#else
	wchar_t path[512];
	GetModuleFileName(GetModuleHandle(NULL), path, 512);
	strcpy_s(AbsModulePath, fdstring(&path[0]));
#endif
	SPG_ConcatPath(FileNamePath,AbsModulePath,LiseEd->Lise.FileNamesaveThickness);

	if(LiseEd->Lise.bAllowSaveThickness == true)
	{
		if(!LiseEd->Lise.FileNamesaveThickness)
		{
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\tFile Name is not define in Config file for function LiseEDThickness");
			}
			char* FileSavePeaks = "ThicknessDefaultName.txt";
			strcpy(AbsModulePath,FileSavePeaks);
		}
		else
		{
			// on va copier le nouveau nom de répertoire dans la variable de la structure
			strcpy(AbsModulePath,FileNamePath);
		}

		LiseEd->Lise.FichierSaveThickness = fopen(AbsModulePath, "wb");

		SYSTEMTIME systime;
		GetSystemTime(&systime);
		// on loggue le nom du chier de sauvegarde ainsi que sa date de création
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tFile save tickness: %s",LiseEd->Lise.FileNamesaveThickness);
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tFile Created: %i-%i-%i \t %i : %i : %i",systime.wDay,systime.wMonth,systime.wYear,systime.wHour + 1,systime.wMinute,systime.wSecond);		
		}
		// Attention pour l'heure c'est +1 pour l'heure d'hivers et +2 pour l'heure d'été
		fprintf(LiseEd->Lise.FichierSaveThickness,"%i-%i-%i \t %i : %i : %i \r\n",systime.wDay,systime.wMonth,systime.wYear,systime.wHour + 1,systime.wMinute,systime.wSecond);

		// infos sur l'échantillons
		if (LiseEd->Lise.sample.bDefined)
		{
			fprintf(LiseEd->Lise.FichierSaveThickness,"Sample Name: %s\r\n", LiseEd->Lise.sample.sName);
			fprintf(LiseEd->Lise.FichierSaveThickness,"Sample Number: %s\r\n", LiseEd->Lise.sample.sSampleNumber);
		}
		fprintf(LiseEd->Lise.FichierSaveThickness,"Period\tDir\tThickness...\tQuality\t\tPeaks...");
		fflush(LiseEd->Lise.FichierSaveThickness);
		LiseEd->Lise.FlagThickness = true;
	}
	else
	{
		if(LiseEd->Lise.bDebug == true)
		{
			LogfileF(*LiseEd->Lise.Log,"[LISEED]\tSave Thickness Success, but the configuration don't allow thickness save");
		}
		return DLL_OK;
	}
	
	return DLL_OK;
}

int LEDICalibrateDark(void* s)
{
	return DLL_OK;
}

int LEDICalibrateThickness(void* s, float Value)
{
	return DLL_OK;
}
