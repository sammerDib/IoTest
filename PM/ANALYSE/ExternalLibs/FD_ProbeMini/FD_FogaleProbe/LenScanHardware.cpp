

#include "..\SrcC\SPG.h"
#include "..\SrcC\SPG_SysInc.h"

//nclude "..\NIDAQ_SDK\DAQmx ANSI C Dev\include\NIDAQmx.h"
#include "..\FD_FogaleProbe\NIDAQmxConfig.h"

#include "LenScanUI.h"
#include "LenScanHardware.h"

#ifndef NOHARDWARE

#include <memory.h>
#include <string.h>
#include <stdio.h>

int GetFirstLineNr(char* DigLine)
{
	char localDigLineName[NISTRLEN];//ce n'est pas indispensable de faire une copie locale si on accepte que le nom de device recu en argument passe en minuscule
	strncpy(localDigLineName,DigLine,NISTRLEN);
	_strlwr(localDigLineName);
	char* L=strstr(localDigLineName,"line");
	if(L==0) return 0;
	if(L[4]==0) return 0;
	return L[4]-'0';
}

#ifdef DebugList
static bool SPG_CONV DAQmxIsError(char* FctName, int RetVal)
{
	if( DAQmxFailed(RetVal) )
	{
		char errBuff[1024];
		DAQmxGetExtendedErrorInfo(errBuff,1024);
		SPG_List2S(FctName,errBuff);
		//MessageBox((HWND)Global.hWndWin,State.errBuff,"FctName",MB_OK);
		return true;
	}
	else
		return false;
}
#else
#define DAQmxIsError(State,FctName,RetVal) DAQmxFailed(RetVal)
#endif

#define VALID(s) (s&&stricmp(s,"void")&&stricmp(s,"none"))

//lecture d'entrées analogiques single shot (pression, temperature)
#define RDAVG 8

#define RDTIMEOUT 1.0

//############################################# LIGHT ##########################################

int SPG_CONV InitLight(niLight& niL, char Dev[NISTRLEN], char Light[NISTRLEN], float lVoltsMin, float lVoltsMax, char LightFeedback[NISTRLEN], float lfVoltsMin, float lfVoltsMax, float VoltsToW, float W_Offset, char LightAlarm[NISTRLEN])
{

	SPG_ZeroStruct(niL);
	CHECK(Dev==0,"InitLight",return 0);

	char FullName[NISTRLEN+NISTRLEN];
	if(VALID(Light))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,Light);
		DAQmxIsError("InitLight",DAQmxCreateTask("",&niL.thLight)); 
		DAQmxIsError("InitLight",DAQmxCreateAOVoltageChan(niL.thLight,FullName,"thLight",lVoltsMin,lVoltsMax,DAQmx_Val_Volts,NULL));
	}

	if(VALID(LightFeedback))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,LightFeedback);
		DAQmxCreateTask("",&niL.thLightFeedback); 
		if(DAQmxFailed(DAQmxCreateAIVoltageChan(niL.thLightFeedback,FullName,"thLightFeedback",DAQmx_Val_Diff,lfVoltsMin,lfVoltsMax,DAQmx_Val_Volts,NULL)))
		{
			DAQmxIsError("InitLight-Feedback",DAQmxCreateAIVoltageChan(niL.thLightFeedback,FullName,"thLightFeedback",DAQmx_Val_PseudoDiff,lfVoltsMin,lfVoltsMax,DAQmx_Val_Volts,NULL));
		}
		niL.VoltsToW=VoltsToW;
		niL.W_Offset=W_Offset;
	}

	if(VALID(LightAlarm))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,LightAlarm);
		DAQmxCreateTask("",&niL.thLightAlarm); 
		niL.laSHR=GetFirstLineNr(FullName);
		DAQmxIsError("InitLight-Alarm",DAQmxCreateDIChan(niL.thLightAlarm,FullName,"thLightAlarm",DAQmx_Val_ChanForAllLines));
	}

	return -1;
}

int SPG_CONV CloseLight(niLight& niL)
{
	SetLight(niL,0);
	DAQmxClearTask(niL.thLight);
	DAQmxClearTask(niL.thLightFeedback);
	DAQmxClearTask(niL.thLightAlarm);
	SPG_ZeroStruct(niL);
	return -1;
}

int SPG_CONV SetLight(niLight& niL, float Level)
{
	double Value=Level;
	DAQmxIsError("SetLight",DAQmxWriteAnalogScalarF64(niL.thLight,1,0,Value,0));
	return -1;
}

int SPG_CONV GetLightFeedback(niLight& niL, float& Power)
{
	if(niL.thLightFeedback==0) return 0;
	double Value=0;
	DAQmxIsError("GetLightFeedback",DAQmxReadAnalogScalarF64(niL.thLightFeedback,RDTIMEOUT,&Value,0));
	Power=Value*niL.VoltsToW+niL.W_Offset;
	return -1;
}

int SPG_CONV GetLightAlarm(niLight& niL, int& Alarm)
{
	DAQmxIsError("GetLightAlarm",DAQmxReadDigitalScalarU32(niL.thLightAlarm,0,(DWORD*)&Alarm,0));
	Alarm>>=niL.laSHR;
	return -1;
}

//############################################# MOTOR ##########################################

int SPG_CONV InitMotor(niMotor& niM, char Dev[NISTRLEN], char Cmd[NISTRLEN], char EOT[NISTRLEN])
{
	SPG_ZeroStruct(niM);
	CHECK(Dev==0,"InitMotor",return 0);

	char FullName[NISTRLEN+NISTRLEN];
	if(VALID(Cmd))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,Cmd);
		DAQmxCreateTask("",&niM.thCommand); 
		niM.cmdSHR=GetFirstLineNr(FullName);
		DAQmxIsError("InitMotor-Command",DAQmxCreateDOChan(niM.thCommand,FullName,"thCommand",DAQmx_Val_ChanForAllLines));
	}

	if(VALID(EOT))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,EOT);
		DAQmxCreateTask("",&niM.thEOT); 
		niM.eotSHR=GetFirstLineNr(FullName);
		DAQmxIsError("InitMotor-EOT",DAQmxCreateDIChan(niM.thEOT,FullName,"thEOT",DAQmx_Val_ChanForAllLines));
	}
	return -1;
}

int SPG_CONV CloseMotor(niMotor& niM)
{
	StopMotor(niM);
	DAQmxClearTask(niM.thCommand);
	DAQmxClearTask(niM.thEOT);
	SPG_ZeroStruct(niM);
	return -1;
}

int SPG_CONV StartMotor(niMotor& niM, int Cmd)
{
	//DAQmxWriteRaw(niM.thCommand,1,1,0,&Command,&sampsPerChanWritten,0);
	DWORD Command=Cmd<<niM.cmdSHR;
	DAQmxIsError("StartMotor",DAQmxWriteDigitalScalarU32(niM.thCommand,1,0,Command,0));
	return -1;
}

int SPG_CONV StopMotor(niMotor& niM)
{
	StartMotor(niM,0);
	return -1;
}

#define MAXRD 3 //au bout de trois mesures (plus la mesure préalable) sans que l'on ai eu la même valeur deux fois d'affilée on sort dé(ca)pité
#define STABLERD 1 //si deux mesures de suite donnent le même resultat on sort ok

int SPG_CONV GetMotorEOT(niMotor& niM, DWORD& EOT)
{
	//mesure préalable
	CHECK(DAQmxIsError("GetMotorEOT",DAQmxReadDigitalScalarU32(niM.thEOT,0,&EOT,0))==true,"GetMotorEOT",return 0);
	int numStable=0;
	for(int i=0;(i<MAXRD)&&(numStable<STABLERD);i++)
	{
		DWORD MTMP_EOT;
		CHECK(DAQmxIsError("GetMotorEOT",DAQmxReadDigitalScalarU32(niM.thEOT,0,&MTMP_EOT,0))==true,"GetMotorEOT",return 0);
		if(EOT!=MTMP_EOT) {EOT=MTMP_EOT;numStable=0;} else numStable++;
	}
	EOT>>=niM.eotSHR;
	return (numStable<STABLERD)?0:-1;
}


//############################################# SCAN ##########################################

int SPG_CONV InitScan(niScan& niS, char Dev[NISTRLEN], char aiChanName[NISTRLEN], int MaxOnBoardSamples, float aiVoltsMin, float aiVoltsMax, char Clock[NISTRLEN], float OnboardClockFreq, char Trigger[NISTRLEN])
{
	SPG_ZeroStruct(niS);
	CHECK(Dev==0,"InitScan",return 0);

	//char FullName[NISTRLEN+NISTRLEN];
	if(VALID(aiChanName))
	{
		strcpy(niS.aiChanName,Dev);strcat(niS.aiChanName,"/");strcat(niS.aiChanName,aiChanName);
		DAQmxIsError("InitScan",DAQmxCreateTask("",&niS.thAI)); 
		if(DAQmxFailed(DAQmxCreateAIVoltageChan(niS.thAI,niS.aiChanName,"thAI", DAQmx_Val_Diff, aiVoltsMin, aiVoltsMax, DAQmx_Val_Volts, "")))
		{
			DAQmxIsError("InitScan",DAQmxCreateAIVoltageChan(niS.thAI,niS.aiChanName,"thAI", DAQmx_Val_PseudoDiff, aiVoltsMin, aiVoltsMax, DAQmx_Val_Volts, ""));
		}
		DAQmxIsError("InitScan",DAQmxCfgSampClkTiming(niS.thAI,VALID(Clock)?Clock:"OnboardClock",OnboardClockFreq,DAQmx_Val_Rising,DAQmx_Val_FiniteSamps,MaxOnBoardSamples));
		if(VALID(Trigger)) {DAQmxIsError("InitScan",DAQmxCfgDigEdgeStartTrig(niS.thAI,Trigger,DAQmx_Val_Rising));}

		uInt32 NumChans=1;
		DAQmxGetReadNumChans(niS.thAI, &NumChans);
		niS.NumChans=NumChans;
	}

	//niS.MaxNumS=MaxNumS;
	//niS.Data=SPG_MemAllocZ(niS.MaxNumS*niS.NumChans*sizeof(short),"InitScan:niS.Data");



	return -1;
}

int SPG_CONV CloseScan(niScan& niS)
{
	DAQmxIsError("CloseScan",DAQmxClearTask(niS.thAI));
	//SPG_MemFree(niS.Data);
	SPG_ZeroStruct(niS);
	return -1;
}

int SPG_CONV StartScan(niScan& niS, SAMRAWSCAN* Data, int MaxNumS)
{
	niS.Data=Data;
	niS.MaxNumS=MaxNumS;
	niS.CurNumS=0;
	DAQmxIsError("StartScan",DAQmxStartTask(niS.thAI));
	return -1;
}

int SPG_CONV UpdateScan(niScan& niS)
{
	CHECK(niS.Data==0,"UpdateScan",return 0);
	if(niS.CurNumS<niS.MaxNumS)
	{
		long sampsRead=0;
		long numBytesPerSamp=0;
		int rw=DAQmxReadRaw(niS.thAI, DAQmx_Val_Auto, 0, niS.Data+(niS.CurNumS*niS.NumChans), (niS.MaxNumS-niS.CurNumS)*niS.NumChans*sizeof(SAMRAWSCAN), &sampsRead, &numBytesPerSamp, 0);
		DbgCHECKTWO(numBytesPerSamp!=niS.NumChans*sizeof(SAMRAWSCAN),"UpdateScan",niS.aiChanName);
		if(rw!=DAQmxErrorSamplesNotYetAvailable) DAQmxIsError("UpdateScan",rw); //l'erreur -200284 DAQmxErrorSamplesNotYetAvailable ne devrait pas exister - c'est juste que le buffer est encore vide au moment de la premiere lecture du buffer, ce qui peut se produire épisodiquement quand on attend le trigger ou que l'horloge externe est un peu lente, c'est un peu débile de faire un erreur pour ca
		niS.CurNumS+=sampsRead;
	}
	return niS.CurNumS;
}

int SPG_CONV StopScan(niScan& niS)
{
	DAQmxIsError("StopScan",DAQmxStopTask(niS.thAI));
	//niS.Data=0;
	//niS.MaxNumS=0;
	return niS.CurNumS;
}

//############################################# MUX ##########################################

int SPG_CONV InitMux(niMux& niM, char Dev[NISTRLEN], char MuxLines[NISTRLEN])
{
	SPG_ZeroStruct(niM);
	CHECK(Dev==0,"InitMux",return 0);
	char FullName[NISTRLEN+NISTRLEN];
	if(VALID(MuxLines))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,MuxLines);
		DAQmxIsError("InitMux",DAQmxCreateTask("",&niM.thMux));	
		niM.muxSHR=GetFirstLineNr(FullName);
		DAQmxIsError("InitMux",DAQmxCreateDOChan(niM.thMux,FullName,"thMux",DAQmx_Val_ChanForAllLines));
	}
	return -1;
}

int SPG_CONV CloseMux(niMux& niM)
{
	SetMux(niM,0);
	DAQmxClearTask(niM.thMux);
	SPG_ZeroStruct(niM);
	return -1;
}

int SPG_CONV SetMux(niMux& niM, int Voie)
{
	DWORD Command=Voie<<niM.muxSHR;
	DAQmxIsError("SetMux",DAQmxWriteDigitalScalarU32(niM.thMux,1,0,Command,0));
	return -1;
}

//############################################# TEMPERATURE ##########################################

int SPG_CONV InitTemperature(niTemperature& niT, char Dev[NISTRLEN], char T[NISTRLEN], float VoltsMin, float VoltsMax, float VoltsToDegC, float DegC_Offset)
{
	SPG_ZeroStruct(niT);
	CHECK(Dev==0,"InitTemperature",return 0);
	char FullName[NISTRLEN+NISTRLEN];
	if(VALID(T))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,T);
		DAQmxIsError("InitTemperature",DAQmxCreateTask("",&niT.thT)); 
		DAQmxIsError("InitTemperature",DAQmxCreateAIVoltageChan(niT.thT,FullName,"thT",DAQmx_Val_Cfg_Default,VoltsMin,VoltsMax,DAQmx_Val_Volts,NULL));
	}
	niT.VoltsToDegC=VoltsToDegC;
	niT.DegC_Offset=DegC_Offset;
	return -1;
}

int SPG_CONV CloseTemperature(niTemperature& niT)
{
	DAQmxClearTask(niT.thT);
	SPG_ZeroStruct(niT);
	return -1;
}

#define ReadAvg(tsk,var,fct) if(tsk==0) return 0; float var=0; for(int MTMP_i=0;MTMP_i<RDAVG;MTMP_i++) {double MTMP_VAL=0; CHECK(DAQmxIsError(fct,DAQmxReadAnalogScalarF64(tsk,RDTIMEOUT,&MTMP_VAL,0))==true,fct,return 0); var+=MTMP_VAL;} var/=RDAVG;

int SPG_CONV GetTemperature(niTemperature& niT, float& T)
{
	ReadAvg(niT.thT,Value,"GetTemperature");
	T=niT.DegC_Offset+niT.VoltsToDegC*Value;
	return -1;
}

//############################################# PRESSURE ##########################################

//(copier coller de la temperature)
int SPG_CONV InitPressure(niPressure& niP, char Dev[NISTRLEN], char P[NISTRLEN], float VoltsMin, float VoltsMax, float VoltsToPa, float Pa_Offset)
{
	SPG_ZeroStruct(niP);
	CHECK(Dev==0,"InitPressure",return 0);
	char FullName[NISTRLEN+NISTRLEN];
	if(VALID(P))
	{
		strcpy(FullName,Dev);strcat(FullName,"/");strcat(FullName,P);
		DAQmxIsError("InitPressure",DAQmxCreateTask("",&niP.thP)); 
		DAQmxIsError("InitPressure",DAQmxCreateAIVoltageChan(niP.thP,FullName,"thP",DAQmx_Val_Cfg_Default,VoltsMin,VoltsMax,DAQmx_Val_Volts,NULL));
	}
	niP.VoltsToPa=VoltsToPa;
	niP.Pa_Offset=Pa_Offset;
	return -1;
}

int SPG_CONV ClosePressure(niPressure& niP)
{
	DAQmxClearTask(niP.thP);
	SPG_ZeroStruct(niP);
	return -1;
}

int SPG_CONV GetPressure(niPressure& niP, float& P)
{
	ReadAvg(niP.thP,Value,"GetPressure");
	P=niP.Pa_Offset+niP.VoltsToPa*Value;
	return -1;
}

//############################################# LENSCAN COMPLET ##########################################

int SPG_CONV LenScanHardwareInit(LenScanHardware& LH, SPG_CONFIGFILE& CFG)
{
	SPG_ZeroStruct(LH);

	LH.EnableDebugWindow=1; CFG_GetInt(CFG,LH.EnableDebugWindow);

	strcpy(LH.DeviceName,"Dev1"); CFG_StringParam(CFG,"LH.DeviceName",LH.DeviceName,"NIDAQmx device name",1);

	SCM_NIDAQmxList(LH.L);

	if(SCM_NIDAQmxIsInList(LH.L,LH.DeviceName)==-1)
	{
		if(LH.L.n==1)
		{
			SPG_List2S("LenScanHardwareInit: Specified device not existing\nChanging to:",CFG.FileName);
		}
		else
		{
			SPG_List2S("LenScanHardwareInit: Specified device not existing\nEdit LH.DeviceName in configuration file:",CFG.FileName);
			SCM_NIDAQmxListDisplay(LH.L);
			return 0;
		}
	}


	InitLight(LH.Light, LH.DeviceName, 
		CFG_GetStringParam(CFG,"LH.Light.Command","ao0"), CFG_GetFloatParam(CFG,"LH.Command.VoltsMin",-10), CFG_GetFloatParam(CFG,"LH.Command.VoltsMax",10), 
		CFG_GetStringParam(CFG,"LH.Light.LightFeedback","ai1"), CFG_GetFloatParam(CFG,"LH.Light.VoltsMin",-10), CFG_GetFloatParam(CFG,"LH.Light.VoltsMax",10), CFG_GetFloatParam(CFG,"LH.Light.VoltsToW",-1.17e-6), CFG_GetFloatParam(CFG,"LH.Light.W_Offset",0),//Puissance(W)=-1.17e-6xTension(V) [0.85A/W x -1000000V/A ampli de courant, 0.85V/µW dans PowerMonitor.vi] 
		CFG_GetStringParam(CFG,"LH.Light.LightAlarm","port0/line7"));

	InitMotor(LH.Motor, LH.DeviceName, 
		CFG_GetStringParam(CFG,"LH.Motor.Command","port0/line0:1"), CFG_GetStringParam(CFG,"LH.Motor.EOT","port0/line5:6"));

	int MaxOnBoardSamples=CFG_GetIntParam(CFG,"LH.MaxOnBoardSamples",4*1024*1024);
	InitScan(LH.Scan, LH.DeviceName, 
		CFG_GetStringParam(CFG,"LH.Scan.ai","ai0"), MaxOnBoardSamples, CFG_GetFloatParam(CFG,"LH.Scan.VoltsMin",-10), CFG_GetFloatParam(CFG,"LH.Scan.VoltsMax",10), 
		CFG_GetStringParam(CFG,"LH.Scan.Clock","PFI9"), CFG_GetFloatParam(CFG,"LH.Scan.OnboardClockFreq",206452), CFG_GetStringParam(CFG,"LH.Scan.Trigger","PFI1"));
	{int i=CFG_ParamFromName(CFG,"LH.Scan.ai"); if(i) {strcpy(CFG.CP[i].Comment,"LS(LenScanMode=1): ai0  LI(LenScanMode=2): ai0:1");}}

	InitMux(LH.Mux, LH.DeviceName, CFG_GetStringParam(CFG,"LH.Mux.MuxLines","port0/line2:4"));

	InitTemperature(LH.tDL, LH.DeviceName, CFG_GetStringParam(CFG,"LH.tDL.ai","ai4"), 
		CFG_GetFloatParam(CFG,"LH.tDL.VoltsMin",-10), CFG_GetFloatParam(CFG,"LH.tDL.VoltsMax",10), 
		CFG_GetFloatParam(CFG,"LH.tDL.VoltsToDeg",100), CFG_GetFloatParam(CFG,"LH.tDL.DegC_Offset",0));//LS:100°C/Volt Offset 0 //LI et centrale meteo 1°C/Volt + 18°C

	InitTemperature(LH.tExt, LH.DeviceName, CFG_GetStringParam(CFG,"LH.tExt.ai","ai5"), 
		CFG_GetFloatParam(CFG,"LH.tExt.VoltsMin",-10), CFG_GetFloatParam(CFG,"LH.tExt.VoltsMax",10), 
		CFG_GetFloatParam(CFG,"LH.tExt.VoltsToDeg",1), CFG_GetFloatParam(CFG,"LH.tExt.DegC_Offset",18));//LI et centrale meteo 1°C/Volt + 18°C (mettre tExt=none sur le LS)

	InitPressure(LH.pExt, LH.DeviceName, CFG_GetStringParam(CFG,"LH.pExt.ai","ai6"),
		CFG_GetFloatParam(CFG,"LH.pExt.VoltsMin",-10), CFG_GetFloatParam(CFG,"LH.pExt.VoltsMax",10), 
		CFG_GetFloatParam(CFG,"LH.pExt.VoltsToPa",5300), CFG_GetFloatParam(CFG,"LH.pExt.Pa_Offset",80000));//centrale meteo 5300Pa/V + 80000Pa (mettre pExt=none sur le LS)

	if(LH.EnableDebugWindow) SignalWindowInit(LH.SW,1,1,960,320,"LenScan Hardware (LH)",1);

	return -1;
} 

int SPG_CONV LenScanHardwareClose(LenScanHardware& LH)
{
	if(LH.EnableDebugWindow) 
	{
		SignalWindowClose(LH.SW);
		for(int i=0;i<LH.Scan.NumChans;i++)
		{
			if(LH.DbgC[i].NumS) Cut_Close(LH.DbgC[i]);
		}
	}

	CloseLight(LH.Light);
	CloseMotor(LH.Motor);
	CloseScan(LH.Scan);
	CloseMux(LH.Mux);
	CloseTemperature(LH.tDL);
	CloseTemperature(LH.tExt);

	SPG_ZeroStruct(LH);
	return -1;
} 

#define FIRSTSAMTIMEOUT 2000
#define EOTSETUPTIMEOUT 30000
#define ALLSAMTIMEOUT 30000
#define MotorReverseTempo Sleep(100)

#define DEFPRESS 101300
#define DEFDLTEMP 20 //todo: remplacer par les parametres de l'utilisateur
#define DEFRH 50


int SPG_CONV LenScanHardwareScan(LenScanHardware& LH, SAMRAWSCAN* Signal, int& NumC, int& NumS)
{
	//CHECK(LH.Scan.MaxNumS==0,"LenScanHardwareScan",return 0);
	//CHECK(LH.Scan.NumChans==0,"LenScanHardwareScan",return 0);
	//raffraichit les valeurs de temperature/pression
	if(GetTemperature(LH.tDL,LH.scan_tDL)==0) LH.scan_tDL=DEFDLTEMP;
	if(GetTemperature(LH.tExt,LH.scan_tExt)==0) LH.scan_tExt=DEFDLTEMP;//todo: remplacer par le user setting si la sonde n'existe pas
	if(GetPressure(LH.pExt,LH.scan_pExt)==0) LH.scan_pExt=DEFPRESS;

	{//attend que le moteur soit placé en position de départ
		DWORD T=GetTickCount(); int LoopCount=0;
		do { GetMotorEOT(LH.Motor,LH.EOT); LoopCount++; } while	( ( (LH.EOT&(1<<LH.Direction)) == 0 )&&( (GetTickCount()-T)<EOTSETUPTIMEOUT ) );
		if(LoopCount>1) MotorReverseTempo;//on a du attendre que le moteur arrive pour sortir de la boucle, on attend avant de le relancer pour ne pas stresser ni le motoreducteur, ni la courroie, ni l'alim du moteur, ni fabrice, ...
	} 
	CHECK((LH.EOT==0)||(LH.EOT==3),"MeasDoScan: Invalid EOT switch configuration",return 0);//rejeter la faute sur le hardware au cas ou on se serait planté de convention des switchs - une injustice - en fait le chariot pourrait rester en plein milieu du trajet

	DWORD sEOT=LH.EOT^3;//pour le scan on s'arretera sur l'autre fin de course
	LH.Direction=1-LH.Direction;
	StartMotor(LH.Motor,2+LH.Direction);//le bit 2 c'est motor on

	DWORD T=GetTickCount();
	while(//(GetAsyncKeyState(VK_ESCAPE)==0)&&(GetAsyncKeyState(VK_SPACE)==0)&&
		(LH.EOT)&&					//attend le decollage du switch
		((GetTickCount()-T)<FIRSTSAMTIMEOUT)
		)
	{
		Sleep(30);
		GetMotorEOT(LH.Motor,LH.EOT);
	}

	CHECK_ELSE(LH.EOT!=0,"MeasDoScan: Switch not released FIRSTSAMTIMEOUT after motor on, motor not moving",;);

	{//LH.EOT==0
		//#### acquisition nidaqmx ####
		StartScan(LH.Scan,Signal,NumS);//scan continu, paramétré pour démarrer au trigger de la regle

		int sNumSample=0;

		//GetAsyncKeyState(VK_ESCAPE);GetAsyncKeyState(VK_SPACE);
		//les conditions d'arret: on appuie sur escape, espace, fin de course moteur, plus d'échantillon que spécifié, toujours pas d'échantillon apres une seconde, temps total supérieur à 100 secondes
		DWORD T=GetTickCount();
		while(
			//(GetAsyncKeyState(VK_ESCAPE)==0)&&(GetAsyncKeyState(VK_SPACE)==0)&&
			((LH.EOT&(1<<LH.Direction))==0)&&							//switch de fin de course
			(LH.Scan.CurNumS<LH.Scan.MaxNumS)&&							//nombre total d'echantillons atteint
			(((sNumSample>0)||(GetTickCount()-T)<FIRSTSAMTIMEOUT))&&	//aucun echantillon enregistré apres un delai FIRSTSAMTIMEOUT
			((GetTickCount()-T)<ALLSAMTIMEOUT)							//temps total ALLSAMTIMEOUT dépassé
			)
		{
			UpdateScan(LH.Scan);





			GetMotorEOT(LH.Motor,LH.EOT);
		}
		NumS=StopScan(LH.Scan);//fin de l'acquisition
	}
	StopMotor(LH.Motor); MotorReverseTempo; LH.Direction=1-LH.Direction; StartMotor(LH.Motor,2+LH.Direction);//renvoie l'ascenseur

	//debug scan.raw output
	if(LH.EnableDebugWindow) 
	{
		SPG_SaveFile("Scan.raw",(BYTE*)LH.Scan.Data,LH.Scan.CurNumS*LH.Scan.NumChans*sizeof(SAMRAWSCAN),0);

		{for(int i=0;i<NumC;i++)
		{
			if(LH.SW.CView.n>0) CView_Unload(LH.SW.CView,LH.DbgC[i]);
		}}

		{for(int i=0;i<NumC;i++)
		{
			if(NumS>LH.DbgC[i].NumS)
			{
				if(LH.DbgC[i].NumS) Cut_Close(LH.DbgC[i]);
				Cut_Create(LH.DbgC[i],NumS);SPG_CatMemName(LH.DbgC[i].D,"LH.DbgC[i]");
			}
		}}

		{for(int i=0;i<NumC;i++)
		{
			for(int x=0;x<NumS;x++)
			{
				LH.DbgC[i].D[x]=Signal[i+x*NumC];
			}
			//LH.DbgC[i].NumS=NumS;
			char F[256];
			sprintf(F,"HardwareScan_SignalIndex%i.pgc",i);
			Cut_SaveToFile(LH.DbgC[i],F);
			CView_pLoad(LH.SW.CView,LH.DbgC[i]);
		}}
		SignalWindowUpdate(LH.SW);
	}
	return -1;
} 

#endif

