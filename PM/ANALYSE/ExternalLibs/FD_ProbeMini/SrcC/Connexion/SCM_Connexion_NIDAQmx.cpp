
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#ifdef SPG_General_USENIDAQmx

#include "..\SPG_Includes.h"
#include "SCM_ExtensContinuous.h"

#define NIDAQmxRELPATH "NIDAQ_SDK\\"
#include "..\..\NIDAQ_SDK\SPG_NIDAQmxConfig.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_NIDAQmx
#define sci_NAME sci_NAME_NIDAQmx

#include "SCM_Connexion_NIDAQmx_Internal.h"


// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface



// ###################################################

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	char DeviceName[32];
	char DeviceType[32];
	char DeviceSerial[32];
	int DeviceNth;

	char ChannelName[32];
	char ClockName[32];

	DWORD dwMode;

	float VoltsMin;
	float VoltsMax;
	int DifferentialInput; //1:DAQmx_Val_Diff 0:DAQmx_Val_PseudoDiff
	int UseMaxFrequency;
	double FrequencyHz;
	int BufferSamples;
} SCX_ADDRESS;


// ###################################################

#define NIDAQmx_errBuff 768

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;

	SCM_DEVICELIST L;
	
	TaskHandle taskHandle;
	TaskHandle CountertaskHandle;
	char errBuff[NIDAQmx_errBuff];

	char chFullName[64];
	char ClockFullName[64];
	char CounterFullName[64];

	float64 FrequencyHz;
	uInt32 DataSumChanSampleSize;

} SCX_STATE; //parametres d'une connexion en particulier

//efine NIDAQmx_USECallback

// ###################################################

#ifdef NIDAQmx_USECallback

static int32 CVICALLBACK EveryNCallback(TaskHandle taskHandle, int32 everyNsamplesEventType, uInt32 nSamples, void *callbackData)
{
	SCX_CONNEXION* C=(SCX_CONNEXION*)callbackData;
	SCX_STATE& State=*C->State;

	return 0;
}

static int32 CVICALLBACK DoneCallback(TaskHandle taskHandle, int32 status, void *callbackData)
{
	SCX_CONNEXION* C=(SCX_CONNEXION*)callbackData;
	SCX_STATE& State=*C->State;

	return 0;
}

#endif

#ifdef DebugList
static bool SPG_CONV DAQmxIsError(SCX_STATE& State, char* FctName, int RetVal)
{
	if( RetVal!=0 ) //DAQmxFailed(RetVal)
	{
		DAQmxGetExtendedErrorInfo(State.errBuff,NIDAQmx_errBuff);
		SPG_List2S(FctName,State.errBuff);
		//MessageBox((HWND)Global.hWndWin,State.errBuff,"FctName",MB_OK);
		return true;
	}
	else
		return false;
}
#else
#define DAQmxIsError(State,FctName,RetVal) DAQmxFailed(RetVal)
#endif

int SPG_CONV scxNIDAQBladeGetHandle(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxNIDAQBladeGetHandle");
	if(scxIsTypeUID(C,sci_UID)==0) return 0;
	SCX_STATE& State=*C->State;
	return (int)State.taskHandle;
}

int SPG_CONV scxNIDAQStart(SCX_CONNEXION* C)
{
	CHECK(C==0,sci_NAME "\r\n" "scxNIDAQStart",return 0);\
	CHECK(C->Etat!=scxOK,sci_NAME "\r\n" "scxNIDAQStart",return 0);\
	CHECK(C->State==0,sci_NAME "\r\n" "scxNIDAQStart",return 0);\
	SCX_STATE& State=*C->State;
	int r=-1;

	CHECKTWO(DAQmxIsError(State,"DAQmxStartTask",
		DAQmxStartTask(State.taskHandle) ),
	"scxNIDAQBladeStart",State.chFullName,r=0;);
	
	if(State.Address.dwMode&M_CounterClkTiming)
	{
		CHECKTWO(DAQmxIsError(State, "DAQmxStartTask",
			DAQmxStartTask(State.CountertaskHandle) ),
		"scxNIDAQBladeStart",State.CounterFullName,r=0;);
	}
	return r;
}

int SPG_CONV scxNIDAQStop(SCX_CONNEXION* C)
{
	CHECK(C==0,sci_NAME "\r\n" "scxNIDAQStop",return 0);\
	CHECK(C->Etat!=scxOK,sci_NAME "\r\n" "scxNIDAQStop",return 0);\
	CHECK(C->State==0,sci_NAME "\r\n" "scxNIDAQStop",return 0);\
	SCX_STATE& State=*C->State;
	int r=-1;
	
	CHECKTWO(DAQmxIsError(State, "DAQmxStopTask", 
		DAQmxStopTask(State.taskHandle) ),
	"scxNIDAQBladeStop",State.chFullName,r=0;);

	if(State.Address.dwMode&M_CounterClkTiming)
	{
		CHECKTWO(DAQmxIsError(State, "DAQmxStopTask", 
			DAQmxStopTask(State.CountertaskHandle) ),
		"scxNIDAQBladeStop",State.chFullName,r=0;);
	}
	
	return r;
}

static SCX_EXTSTARTCONTINUOUSWRITE(scxNIDAQStartWrite) { return scxNIDAQStart(C); }

static SCX_EXTSTOPCONTINUOUSWRITE(scxNIDAQStopWrite) { return scxNIDAQStop(C); }

static SCX_EXTSTARTCONTINUOUSREAD(scxNIDAQStartRead) { return scxNIDAQStart(C); }

static SCX_EXTSTOPCONTINUOUSREAD(scxNIDAQStopRead) { return scxNIDAQStop(C); }

static SCX_EXTGETCONTINUOUSFREQUENCY(scxNIDAQGetFrequency) //double& Frequency, SCX_CONNEXION* C
{ 
	scxCHECK(C, "scxNIDAQGetFrequency");
	SCX_STATE& State=*C->State;
	return DAQmxGetSampClkRate(State.taskHandle, &Frequency); 
}

static SCX_CONNEXION* SPG_CONV scxNIDAQOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;
/*
	CHECK(DAQmxIsError(State,
	"",  ),
	"",return );
*/
	CHECK(State.Address.DeviceName[0]==0,"scxNIDAQOpen: No device specified",return 0);

	if(State.Address.DeviceName[0]=='*')
	{
		SCM_NIDAQmxList(State.L);

		int n=-1;
		CHECK((n=SCM_NIDAQmxGetNthOfType(State.L,State.Address.DeviceNth,State.Address.DeviceType))<0 ,
			   "scxNIDAQOpen : Nth Device of Type not found",
			   SCM_NIDAQmxListDisplay(State.L);return 0 );

		strncpy(State.Address.DeviceName,State.L.D[n].Name,31);
		sprintf(State.Address.DeviceSerial,"%0.4X",State.L.D[n].Serial);
	}

	SCM_NIDAQmxGetModeFlag(State.Address.ChannelName, State.Address.ClockName, State.Address.FrequencyHz, State.Address.dwMode);

	strcpy(State.chFullName,State.Address.DeviceName);
	strcat(State.chFullName,"/");
	strcat(State.chFullName,State.Address.ChannelName);

	strcpy(State.ClockFullName,State.Address.DeviceName);
	strcat(State.ClockFullName,"/");
	strcat(State.ClockFullName,State.Address.ClockName);

	CHECK(DAQmxIsError(State,
	"DAQmxCreateTask", DAQmxCreateTask("",&State.taskHandle) ),
	"scxNIDAQOpen",scxFree(C->State);scxFree(C);return 0);

	if(State.Address.dwMode&M_CreateAIVoltageChan)
	{
		CHECK(DAQmxIsError(State,
			"DAQmxCreateAIVoltageChan", DAQmxCreateAIVoltageChan(State.taskHandle,State.chFullName,"",State.Address.DifferentialInput?DAQmx_Val_Diff:DAQmx_Val_PseudoDiff,State.Address.VoltsMin,State.Address.VoltsMax,DAQmx_Val_Volts,NULL) ),
		"scxNIDAQOpen:DAQmxCreateAIVoltageChan",scxFree(C->State);scxFree(C);return 0);
	}
	else if(State.Address.dwMode&M_CreateAOVoltageChan)
	{
		CHECK(DAQmxIsError(State,
		"DAQmxCreateAOVoltageChan", DAQmxCreateAOVoltageChan(State.taskHandle,State.chFullName,"",State.Address.VoltsMin,State.Address.VoltsMax,DAQmx_Val_Volts,NULL) ),
		"scxNIDAQOpen:DAQmxCreateAOVoltageChan",scxFree(C->State);scxFree(C);return 0);
	}
	else if(State.Address.dwMode&M_CreateDIChan)
	{
		CHECK(DAQmxIsError(State,
		"DAQmxCreateDIChan", DAQmxCreateDIChan(State.taskHandle,State.chFullName,"",DAQmx_Val_ChanForAllLines) ),
		"scxNIDAQOpen:DAQmxCreateDIChan",scxFree(C->State);scxFree(C);return 0);
	}
	else if(State.Address.dwMode&M_CreateDOChan)
	{
		CHECK(DAQmxIsError(State,"DAQmxCreateDOChan",
			DAQmxCreateDOChan(State.taskHandle,State.chFullName,"",DAQmx_Val_ChanForAllLines) ),
		"scxNIDAQOpen:DAQmxCreateDOChan",scxFree(C->State);scxFree(C);return 0);
	}

	State.FrequencyHz=State.Address.FrequencyHz;

#ifdef NoDAQmxGetAIConvMaxRate
	DbgCHECK(State.Address.dwMode&M_GetAIConvMaxRate,"scxNIDAQOpen: Unsupported configuration");
#else
	if(State.Address.dwMode&M_GetAIConvMaxRate)
	{
		CHECK(DAQmxIsError(State, "DAQmxGetAIConvMaxRate",
			DAQmxGetAIConvMaxRate(State.taskHandle, &State.FrequencyHz) ),
		"scxNIDAQBladeOpen:DAQmxGetAIConvMaxRate",;);
	}
#endif

	if(State.Address.dwMode&M_CfgSampClkTiming)
	{
		CHECK(DAQmxIsError(State,"DAQmxCfgSampClkTiming",
			//DAQmxCfgSampClkTiming(State.taskHandle,State.ClockFullName,State.FrequencyHz,DAQmx_Val_Rising,DAQmx_Val_ContSamps,State.Address.BufferSamples) ),
			DAQmxCfgSampClkTiming(State.taskHandle,State.Address.ClockName,State.FrequencyHz,DAQmx_Val_Rising,DAQmx_Val_ContSamps,State.Address.BufferSamples) ),
		"scxNIDAQOpen:DAQmxCfgSampClkTiming",DAQmxClearTask(State.taskHandle);scxFree(C->State);scxFree(C);return 0);

		if(State.Address.dwMode&M_Read)
		{
			CHECK(DAQmxIsError(State,"DAQmxCfgInputBuffer",
				DAQmxCfgInputBuffer(State.taskHandle, State.Address.BufferSamples)),
			"scxNIDAQOpen:DAQmxCfgInputBuffer",;);
		}
		else if(State.Address.dwMode&M_Write)
		{
			CHECK(DAQmxIsError(State,"DAQmxCfgOutputBuffer",
				DAQmxCfgOutputBuffer(State.taskHandle, State.Address.BufferSamples)),
			"scxNIDAQOpen:DAQmxCfgOutputBuffer",;);
			DAQmxSetWriteRegenMode(State.taskHandle, DAQmx_Val_DoNotAllowRegen);
		}

		DAQmxGetSampClkRate(State.taskHandle, &State.FrequencyHz);
		State.Address.FrequencyHz=State.FrequencyHz;
	}
	
	if(State.Address.dwMode&M_CounterClkTiming)
	{
		
		strcpy(State.CounterFullName,State.Address.DeviceName);
		strcat(State.CounterFullName,"/ctr0");

		CHECK(DAQmxIsError(State,"DAQmxCreateTask",
			DAQmxCreateTask("",&State.CountertaskHandle) ),
		"scxNIDAQOpen:DAQmxCreateTask",;);

		
		
		CHECK(DAQmxIsError(State,"DAQmxCreateCOPulseChanFreq",
			DAQmxCreateCOPulseChanFreq(State.CountertaskHandle, State.CounterFullName,"",DAQmx_Val_Hz,DAQmx_Val_Low,0,State.FrequencyHz,0.5)),
		"scxNIDAQOpen:DAQmxCreateCOPulseChanFreq",;);
		
		/*
				
		CHECK(DAQmxIsError(State,"DAQmxCreateCOPulseChanTicks",
			DAQmxCreateCOPulseChanTicks(State.CountertaskHandle, State.CounterFullName,"","20MHzTimebase",DAQmx_Val_Low,0.0,3,3
			)),
		"scxNIDAQOpen:DAQmxCreateCOPulseChanTicks",;);
		*/
		
		CHECK(DAQmxIsError(State,"DAQmxSetExportedCtrOutEventOutputBehavior",
			DAQmxSetExportedCtrOutEventOutputBehavior(State.CountertaskHandle, DAQmx_Val_Pulse) ),
		"scxNIDAQOpen:DAQmxSetExportedCtrOutEventOutputBehavior",;);
		
		

		CHECK(DAQmxIsError(State,"DAQmxCfgImplicitTiming",
			DAQmxCfgImplicitTiming (State.CountertaskHandle,DAQmx_Val_ContSamps,State.Address.BufferSamples)),
		"scxNIDAQOpen:DAQmxCfgImplicitTiming",;);

		
		
		

	}


/*
	DAQmxErrChk (DAQmxCreateDIChan(taskHandleDig,"Dev1/port0/line0:7","",DAQmx_Val_ChanForAllLines));
	DAQmxErrChk (DAQmxCfgSampClkTiming (taskHandleDig,"/Dev1/Ctr0InternalOutput",rate,DAQmx_Val_Rising,DAQmx_Val_FiniteSamps,sampsPerChanToAcquire));
	
	DAQmxErrChk (DAQmxCreateCOPulseChanFreq(taskHandleCtr, "Dev1/ctr0","",DAQmx_Val_Hz,DAQmx_Val_Low,0,rate,0.5));
	DAQmxErrChk (DAQmxCfgImplicitTiming (taskHandleCtr,DAQmx_Val_FiniteSamps,sampsPerChanToAcquire));
	
	DAQmxErrChk (DAQmxCfgDigEdgeStartTrig(taskHandleCtr,"/Dev1/PFI0",DAQmx_Val_Rising));
*/


#ifdef NIDAQmx_USECallback

	CHECK(DAQmxIsError(State,
	"DAQmxRegisterEveryNSamplesEvent", DAQmxRegisterEveryNSamplesEvent(State.TaskHandle,DAQmx_Val_Acquired_Into_Buffer,State.BufferSamples,0,EveryNCallback,C) ),
	"scxNIDAQOpen:DAQmxRegisterEveryNSamplesEvent",DAQmxClearTask(State.taskHandle);scxFree(C->State);scxFree(C);return 0);
	
	CHECK(DAQmxIsError(State,
	"DAQmxRegisterDoneEvent", DAQmxRegisterDoneEvent(State.TaskHandle,0,DoneCallback,C) ),
	"scxNIDAQOpen:DAQmxRegisterDoneEvent",DAQmxClearTask(State.taskHandle);scxFree(C->State);scxFree(C);return 0);

#endif

	State.DataSumChanSampleSize=0;
	if(State.Address.dwMode&M_Read)
	{
		DAQmxGetReadRawDataWidth(State.taskHandle,&State.DataSumChanSampleSize);
	}
	else if(State.Address.dwMode&M_Write)
	{
		DAQmxGetWriteRawDataWidth(State.taskHandle,&State.DataSumChanSampleSize);
	}
	else
	{
		DbgCHECK(1,"scxNIDAQOpen: State.Address.dwMode contains neither M_Read nor M_Write");
	}

	CHECK(State.DataSumChanSampleSize==0,"scxNIDAQOpen",return 0);

	C->UserFctPtr[sci_EXT_STARTCONTINUOUSREAD]=(SCX_USEREXTENSION)scxNIDAQStartRead;
	C->UserFctPtr[sci_EXT_STOPCONTINUOUSREAD]=(SCX_USEREXTENSION)scxNIDAQStopRead;
	C->UserFctPtr[sci_EXT_STARTCONTINUOUSWRITE]=(SCX_USEREXTENSION)scxNIDAQStartWrite;
	C->UserFctPtr[sci_EXT_STOPCONTINUOUSWRITE]=(SCX_USEREXTENSION)scxNIDAQStopWrite;
	C->UserFctPtr[sci_EXT_GETCONTINUOUSFREQUENCY]=(SCX_USEREXTENSION)scxNIDAQGetFrequency;

	C->Etat=scxOK;

	if(State.Address.dwMode&M_Start) scxNIDAQStart(C);

	return C;
}






// ###################################################

static int SPG_CONV scxNIDAQClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;

	if(State.Address.dwMode&M_Stop) scxNIDAQStop(C);

	C->Etat=scxINVALID;
	DAQmxClearTask(State.taskHandle);
	if(State.CountertaskHandle)
	{
		DAQmxClearTask(State.CountertaskHandle);
	}
	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxNIDAQWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	CHECK((State.Address.dwMode&M_Write)==0,"scxNIDAQWrite",return 0)

	int WrittenLen=0;
	int32 sampsPerChanWrite=DataLen/State.DataSumChanSampleSize;


	if(State.Address.dwMode&M_RAW)
	{
		CHECK(Data==0,"scxNIDAQWrite",return DataLen=0;)

		int32 BytesPerSamp=0; int32 WrittenSam=0;
//int32 __CFUNC     DAQmxWriteRaw(TaskHandle taskHandle, int32 numSamps, bool32 autoStart, float64 timeout, void *writeArray, int32 *sampsPerChanWritten, bool32 *reserved);

		int32 AcceptableSam=sampsPerChanWrite;
		int32 RetVal=DAQmxWriteRaw(State.taskHandle,  sampsPerChanWrite*State.DataSumChanSampleSize, 0, 0, Data, &WrittenSam, 0);

		if(RetVal==DAQmxErrorSamplesCanNotYetBeWritten) //-200292
		{
			//cas normal : le buffer n'est pas completement vide entre deux ecritures
		} 
		else if(RetVal==DAQmxErrorOutputFIFOUnderflow2) //-200621
		{
			//le buffer est completement vide
			//force un restart de la generation
			DAQmxStopTask(State.taskHandle);//pour eviter le message d'erreur
			return -1;
		}
		else if(RetVal==DAQmxErrorNoMoreSpace) //-200293 (prebuffering avant generation)
		{
			//cas normal : le buffer est completement plein apres avoir ete ecrit
		}
		else if(DAQmxIsError(State, "scxNIDAQWrite", RetVal))
		{
			return DataLen=0;
		}
		//else if(RetVal==DAQmxWarningPotentialGlitchDuringWrite) //+200015
		//{
		//	//cas normal : le buffer est completement plein apres avoir ete ecrit
		//}

		//DbgCHECKV(BytesPerSamp!=State.DataSumChanSampleSize,"scxNIDAQWrite\nBytesPerSamp:",BytesPerSamp);
		//DbgCHECKV(BytesPerSamp!=State.DataSumChanSampleSize,"scxNIDAQWrite\nState.DataSumChanSampleSize:",State.DataSumChanSampleSize);
		CHECK(WrittenSam*State.DataSumChanSampleSize>DataLen,"scxNIDAQWrite : Dépassement du buffer en lecture",;);
		WrittenLen=WrittenSam*State.DataSumChanSampleSize;//longueur réellement lue
	}
	else if(State.Address.dwMode&M_AnalogF64)
	{
#define F64_Size 8
		uInt32 NumChans=1;
		DAQmxGetWriteNumChans(State.taskHandle, &NumChans);
		CHECK(NumChans==0,"scxNIDAQWrite:DAQmxWriteAnalogF64:DAQmxGetWriteNumChans",return DataLen=0);
		int32 numSampsPerChan=DataLen/(F64_Size*NumChans);
		int32 RetVal=0;
		int32 sampsPerChanWritten=0;
		CHECK(numSampsPerChan*F64_Size*(int)NumChans!=DataLen,"scxNIDAQWrite:DAQmxWriteAnalogF64 incorrect argument size",return DataLen=0);
		CHECK(DAQmxIsError(State,
		"DAQmxWriteAnalogF64", 
		RetVal=DAQmxWriteAnalogF64(State.taskHandle, numSampsPerChan, 0, 0, DAQmx_Val_GroupByScanNumber, (double*)Data, &sampsPerChanWritten, 0)
		),"scxNIDAQWrite:DAQmxWriteAnalogF64",;);
		CHECK(RetVal<0,"scxNIDAQWrite:DAQmxWriteAnalogF64 write error",DAQmxStopTask(State.taskHandle);DAQmxStartTask(State.taskHandle);return DataLen=0);
		WrittenLen=sampsPerChanWritten*F64_Size;
	}
	else if(State.Address.dwMode&M_DigitalLines)
	{
//int32 DAQmxWriteDigitalLines (TaskHandle taskHandle, int32 numSampsPerChan, bool32 autoStart, float64 timeout, bool32 dataLayout, uInt8 writeArray[], int32 *sampsPerChanWritten, bool32 *reserved);
		uInt32 NumChans=1;
		DAQmxGetWriteNumChans(State.taskHandle, &NumChans);
		CHECK(NumChans==0,"scxNIDAQWrite:DAQmxWriteDigitalLines:DAQmxGetWriteNumChans",return DataLen=0);
		int32 numSampsPerChan=DataLen/NumChans;
		int32 sampsPerChanWritten=0;
		CHECK(DAQmxIsError(State,
		"DAQmxWriteDigitalLines", DAQmxWriteDigitalLines(State.taskHandle, numSampsPerChan, 0, 0, DAQmx_Val_GroupByScanNumber, (BYTE*)Data, &sampsPerChanWritten, 0)
		),"scxNIDAQWrite:DAQmxWriteDigitalLines",;);
		WrittenLen=sampsPerChanWritten;
	}
	else
	{
		DbgCHECK(1,"scxNIDAQWrite: No action defined in this configuration");
	}

	CHECK(WrittenLen>DataLen,"scxNIDAQWrite","Dépassement du buffer en lecture";);
	return WrittenLen;
}

// ###################################################

static int SPG_CONV scxNIDAQRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	CHECK((State.Address.dwMode&M_Read)==0,"scxNIDAQRead",return 0)

	int ReadLen=0;
	if(State.Address.dwMode&M_RAW)
	{
		uInt32 NumChans=1;
		DAQmxGetReadNumChans(State.taskHandle, &NumChans);
		CHECK(NumChans==0,"scxNIDAQRead:DAQmxReadRaw:DAQmxGetReadNumChans",return DataLen=0);
		uInt32 arraySizeBytes=DataLen;
		int32  RetVal=0;
		int32 sampsPerChanRead=0;
		int32 BytesPerSamp=0;
		CHECK(DAQmxIsError(State,
		"DAQmxReadRaw", 
//int32 DAQmxReadRaw (TaskHandle taskHandle, int32 numSampsPerChan, float64 timeout, void *readArray, uInt32 arraySizeInBytes, int32 *sampsRead, int32 *numBytesPerSamp, bool32 *reserved);

		//RetVal=DAQmxReadBinaryI16(State.taskHandle, DAQmx_Val_Auto, 0, DAQmx_Val_GroupByScanNumber, (short*)Data, arraySizeInSamps, &sampsPerChanRead, 0)
		RetVal=DAQmxReadRaw(State.taskHandle, DAQmx_Val_Auto, 0, Data, arraySizeBytes, &sampsPerChanRead, &BytesPerSamp, 0)
		),"scxNIDAQRead:DAQmxReadRaw",;);
		CHECK(RetVal<0,"scxNIDAQRead:DAQmxReadRaw Potential overrun",DAQmxStopTask(State.taskHandle);DAQmxStartTask(State.taskHandle);return DataLen=0);
#ifdef DebugList
		{
			char Msg[256];
			sprintf(Msg,"scxNIDAQRead\r\nNumChans=%i\r\nsampsPerChanRead=%i\r\nBytesPerSamp=%i",NumChans,sampsPerChanRead,BytesPerSamp);
			SPG_List(Msg);
		}
#endif
		ReadLen=sampsPerChanRead*BytesPerSamp;
	}
	else if(State.Address.dwMode&M_BinaryI16)
	{
#define I16_Size 2
		uInt32 NumChans=1;
		DAQmxGetReadNumChans(State.taskHandle, &NumChans);
		CHECK(NumChans==0,"scxNIDAQRead:DAQmxReadBinaryI16:DAQmxGetReadNumChans",return DataLen=0);
		uInt32 arraySizeInSamps=DataLen/I16_Size;
		int32  RetVal=0;
		int32 sampsPerChanRead=0;
		CHECK(DAQmxIsError(State,
		"DAQmxReadBinaryI16", 
		RetVal=DAQmxReadBinaryI16(State.taskHandle, DAQmx_Val_Auto, 0, DAQmx_Val_GroupByScanNumber, (short*)Data, arraySizeInSamps, &sampsPerChanRead, 0)
		),"scxNIDAQRead:DAQmxReadBinaryI16",;);
		CHECK(RetVal<0,"scxNIDAQRead:DAQmxReadBinaryI16 Potential overrun",DAQmxStopTask(State.taskHandle);DAQmxStartTask(State.taskHandle);return DataLen=0);
		ReadLen=I16_Size*NumChans*sampsPerChanRead;
	}
	else if(State.Address.dwMode&M_DigitalLines)
	{
//int32 DAQmxReadDigitalLines(TaskHandle taskHandle, int32 numSampsPerChan, float64 timeout, bool32 fillMode, uInt8 readArray[], uInt32 arraySizeInBytes, int32 *sampsPerChanRead, int32 *numBytesPerSamp, bool32 *reserved);
		uInt32 NumChans=1;
		DAQmxGetReadNumChans(State.taskHandle, &NumChans);
		CHECK(NumChans==0,"scxNIDAQRead:DAQmxReadDigitalLines:DAQmxGetReadNumChans",return DataLen=0);
		uInt32 arraySizeInBytes=DataLen;
		int32 RetVal=0;
		int32 sampsPerChanRead=0;
		int32 numBytesPerSamp=1;
		CHECK(DAQmxIsError(State,
		"DAQmxReadBinaryI16", 
		RetVal=DAQmxReadDigitalLines(State.taskHandle, DAQmx_Val_Auto, 1, DAQmx_Val_GroupByScanNumber, (BYTE*)Data, arraySizeInBytes, &sampsPerChanRead, &numBytesPerSamp, 0)
		),"scxNIDAQRead:DAQmxReadDigitalLines",;);
		CHECK(RetVal<0,"scxNIDAQRead:DAQmxReadDigitalLines read error",DAQmxStopTask(State.taskHandle);DAQmxStartTask(State.taskHandle);return DataLen=0);
		ReadLen=sampsPerChanRead*NumChans*numBytesPerSamp;
		CHECK(numBytesPerSamp!=1,"scxNIDAQRead:DAQmxReadDigitalLines",return DataLen=0);
	}
	else
	{
		DbgCHECK(1,"scxNIDAQRead: No action defined in this configuration");
	}
	CHECK(ReadLen>DataLen,"scxNIDAQRead:Dépassement du buffer en écriture",;);
	return DataLen=ReadLen;
}

// ###################################################

static void SPG_CONV scxNIDAQCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->dwMode=M_Start|M_Stop|M_RAW;//7
		strcpy(Address->DeviceName,"Dev1");
		strcpy(Address->DeviceType,"PXI-6133");
		strcpy(Address->ChannelName,"ai0");//ai0:1//ao0//line0:7
		strcpy(Address->ClockName,ONBOARDCLOCK);
		Address->VoltsMin=-10;
		Address->VoltsMax=10;
		Address->DifferentialInput=1;
		Address->UseMaxFrequency=0;
		Address->FrequencyHz=1000000;
		Address->BufferSamples=1048576;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,CFG.FileName[0]?1:0);

	CFG_IntParam(CFG,"dwMode",(int*)&Address->dwMode,"1:M_Start 2:M_Stop 4:M_RAW",	1);
	CFG_StringParam(CFG,"DeviceName",Address->DeviceName,"Nom NIDAQmx du device ex:Dev1 ou PXI1Slot2, ou * pour une recherche par type et numero",1);
	CFG_StringParam(CFG,"DeviceType",Address->DeviceType,"Type de device (necessite DeviceName = *, ignore sinon) ex:PXI-6133",1);
	CFG_IntParam(CFG,"DeviceNth",&Address->DeviceNth,"Nieme device du type considéré, partant de zero (necessite DeviceName = *), ignore sinon",1);
	CFG_StringParam(CFG,"ChannelName",Address->ChannelName,0,1);
	CFG_StringParam(CFG,"ClockName",Address->ClockName,"OnboardClock ai/SampleClock Ctr0InternalOutput",1);
	CFG_FloatParam(CFG,"VoltsMin",&Address->VoltsMin,0,	1);
	CFG_FloatParam(CFG,"VoltsMax",&Address->VoltsMax,0,	1);
	CFG_IntParam(CFG,"DifferentialInput",&Address->DifferentialInput,"1:DAQmx_Val_Diff 0:DAQmx_Val_PseudoDiff",	1);
#ifndef NoDAQmxGetAIConvMaxRate
	CFG_IntParam(CFG,"UseMaxFrequency",&Address->UseMaxFrequency,"0: Use specified FrequencyHz  1: Override with DAQmxGetAIConvMaxRate",1,CP_INT|CP_HASMIN|CP_HASMAX,0,1);
#endif
	CFG_DoubleParam(CFG,"FrequencyHz",&Address->FrequencyHz,0,1);
	CFG_IntParam(CFG,"BufferSamples",&Address->BufferSamples,0,	1);
	return;
}

// ###################################################

static int SPG_CONV scxNIDAQSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");
	return 0;
}

// ###################################################

static int SPG_CONV scxNIDAQGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");
	return 0;
}

// ###################################################

static int SPG_CONV sciNIDAQDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciNIDAQCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="NIDAQmx";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;//spécifique

	CI->scxOpen=scxNIDAQOpen;
	CI->scxClose=scxNIDAQClose;
	CI->scxWrite=scxNIDAQWrite;
	CI->scxRead=scxNIDAQRead;
	CI->scxCfgAddress=scxNIDAQCfgAddress;
	CI->scxSetParameter=scxNIDAQSetParameter;
	CI->scxGetParameter=scxNIDAQGetParameter;
	CI->sciDestroyConnexionInterface=sciNIDAQDestroyConnexionInterface;
	/*
	CI->UserFctPtr[sci_EXT_STARTCONTINUOUSREAD]=(SCX_USEREXTENSION)scxNIDAQStartRead;
	CI->UserFctPtr[sci_EXT_STOPCONTINUOUSREAD]=(SCX_USEREXTENSION)scxNIDAQStopRead;
	CI->UserFctPtr[sci_EXT_STARTCONTINUOUSWRITE]=(SCX_USEREXTENSION)scxNIDAQStartWrite;
	CI->UserFctPtr[sci_EXT_STOPCONTINUOUSWRITE]=(SCX_USEREXTENSION)scxNIDAQStopWrite;
	*/

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#else

#pragma SPGMSG(__FILE__,__LINE__,"Connexion : NIDAQmx disabled")

#endif //if SPG_General_USENIDAQmx

#endif //if SPG_General_USECONNEXION

