
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USEWindows

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include "SCM_ConnexionDbg.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_RS232
#define sci_NAME sci_NAME_RS232

//----------------------------------------

#define MAXRS232SIZE 0

//----------------------------------------

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface



//----------------------------------------

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	int Port;//1:COM1 2:COM2 ...
	int Speed;//bps ...9600...115200
	int DTRControl;
	int RTSControl;
	int Parity;
	int StopBit;
	int InQueue;
	int OutQueue;
	int DelayIO;
	int QuietOverrun;
	int ExtendedSyntax; // Standard:COM%i Extended:\\.\COM%i
} SCX_ADDRESS;

/*
//
// DTR Control Flow Values.
//
#define DTR_CONTROL_DISABLE    0x00
#define DTR_CONTROL_ENABLE     0x01
#define DTR_CONTROL_HANDSHAKE  0x02

//
// RTS Control Flow Values
//
#define RTS_CONTROL_DISABLE    0x00
#define RTS_CONTROL_ENABLE     0x01
#define RTS_CONTROL_HANDSHAKE  0x02
#define RTS_CONTROL_TOGGLE     0x03

//
// DTR Control Flow Values.
//
#define DTR_CONTROL_DISABLE    0x00
#define DTR_CONTROL_ENABLE     0x01
#define DTR_CONTROL_HANDSHAKE  0x02

//
// RTS Control Flow Values
//
#define RTS_CONTROL_DISABLE    0x00
#define RTS_CONTROL_ENABLE     0x01
#define RTS_CONTROL_HANDSHAKE  0x02
#define RTS_CONTROL_TOGGLE     0x03

#define NOPARITY            0
#define ODDPARITY           1
#define EVENPARITY          2
#define MARKPARITY          3
#define SPACEPARITY         4

#define ONESTOPBIT          0
#define ONE5STOPBITS        1
#define TWOSTOPBITS         2
*/

//----------------------------------------

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	char Name[32];
	HANDLE hCom;//SPG_SERIALCOMM SSC;
	//DWORD LastIO;
	S_TIMER LastIO;
	DCB dcb;
	COMMTIMEOUTS ct;
	COMSTAT cst;
	DWORD CodeErreur;
	//...
} SCX_STATE; //parametres d'une connexion en particulier


//----------------------------------------

static SCX_CONNEXION* SPG_CONV scxRS232Open(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;
	sprintf(State.Name,"\\\\.\\COM%i",State.Address.Port);
	char* stdName = State.Name+4;

	State.hCom=CreateFile(State.Address.ExtendedSyntax?State.Name:stdName,GENERIC_READ|GENERIC_WRITE,0,0,OPEN_EXISTING,0,NULL);

#ifdef DebugList
	char Msg[256]; SPG_GetLastWinError(Msg,256);
	CHECKTHREE(State.hCom==INVALID_HANDLE_VALUE,"scxRS232Open failed: Check COM port parameter, or port already open:",State.Address.ExtendedSyntax?State.Name:stdName,Msg,scxFree(C->State);scxFree(C);return 0);
#else
	CHECKTWO(State.hCom==INVALID_HANDLE_VALUE,"scxRS232Open failed: Check COM port parameter, or port already open:",State.Address.ExtendedSyntax?State.Name:stdName,scxFree(C->State);scxFree(C);return 0);
#endif

	DbgCHECKTWO(SetupComm(State.hCom,State.Address.InQueue,State.Address.OutQueue)==0,"scxRS232Open: Invalid InQueue/OutQueue parameter",State.Name);

	State.dcb.DCBlength=sizeof(DCB); GetCommState(State.hCom,&State.dcb);
	State.dcb.BaudRate=State.Address.Speed; 
	State.dcb.ByteSize=8;
	State.dcb.fParity=(State.Address.Parity==NOPARITY)?1:0;// Enable parity checking 
	State.dcb.Parity=State.Address.Parity;// 0-4=no,odd,even,mark,space 
	State.dcb.StopBits=State.Address.StopBit;
	State.dcb.fDtrControl=State.Address.DTRControl;
	State.dcb.fRtsControl=State.Address.RTSControl;
	State.dcb.fBinary=1; State.dcb.fOutxCtsFlow=0; State.dcb.fOutxDsrFlow=0; State.dcb.fDsrSensitivity=0;
	State.dcb.fTXContinueOnXoff=0; State.dcb.fOutX=0; State.dcb.fInX=0; State.dcb.fAbortOnError=0;
	CHECK(SetCommState(State.hCom,&State.dcb)==0,"scxRS232Open: Check COM parameters\nspeed, flags, port",;);

	State.ct.ReadIntervalTimeout=MAXDWORD;
	State.ct.ReadTotalTimeoutMultiplier=0;
	State.ct.ReadTotalTimeoutConstant=0;
	State.ct.WriteTotalTimeoutMultiplier=5;
	State.ct.WriteTotalTimeoutConstant=5;
	CHECK(SetCommTimeouts(State.hCom,&State.ct)==0,"scxRS232Open",;);

	PurgeComm(State.hCom,PURGE_TXABORT|PURGE_RXABORT|PURGE_TXCLEAR|PURGE_RXCLEAR);
	

	//C->State->LastIO=GetTickCount();
	S_InitTimer(State.LastIO,"LastIO");
	S_StartTimer(State.LastIO);

	C->Etat=scxOK;
	return C;
}

//----------------------------------------

static void SPG_CONV scxRS232ClearCommError(SCX_STATE& State)
{
	CHECKTWO(ClearCommError(State.hCom,&State.CodeErreur,&State.cst)==0,"scxRS232 ClearCommError failed",State.Name,return);//ClearCommError failed
	DbgCHECKTWO(State.CodeErreur&CE_RXOVER,"scxRS232: Receive Queue overflow",State.Name);
#ifdef DebugList
	if(State.Address.QuietOverrun==0) DbgCHECKTWO(State.CodeErreur&CE_OVERRUN,"scxRS232: Receive Overrun Error",State.Name);
#endif
	DbgCHECKTWO(State.CodeErreur&CE_RXPARITY,"scxRS232: Receive Parity Error",State.Name);
	DbgCHECKTWO(State.CodeErreur&CE_FRAME,"scxRS232: Receive Framing error",State.Name);
	DbgCHECKTWO(State.CodeErreur&CE_BREAK,"scxRS232: Break Detected",State.Name);
	DbgCHECKTWO(State.CodeErreur&CE_TXFULL,"scxRS232: TX Queue is full",State.Name);
	DbgCHECKTWO(State.CodeErreur&~(CE_RXOVER|CE_OVERRUN|CE_RXPARITY|CE_FRAME|CE_BREAK|CE_TXFULL),"scxRS232",State.Name);
	return;
}

//----------------------------------------

static int SPG_CONV scxRS232Close(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;
	S_StopTimer(State.LastIO);
	S_CloseTimer(State.LastIO);

	CHECK(	CloseHandle(State.hCom)==0,
			"scxRS232Close failed",
			{scxRS232ClearCommError(State);CHECK(CloseHandle(State.hCom)==0,"scxRS232Close failed",;)}
			);

	scxFree(C->State);scxFree(C);
	return scxOK;
}

/*
static void SPG_CONV scxRS232DelayIO(SCX_STATE& State)
{
	DWORD dwT;
	while(
		dwT=GetTickCount()-State.LastIO,
		(dwT<State.Address.DelayIO)
		&& (SPG_GLOBAL_ETAT(MUSTEXIT)==0)
		)
	{
		//scxSendDebugConnexion("scxRS232Write","DelayIO: extra delay",C);
		DoEvents(SPG_DOEV_ALL);
		SPG_Sleep(V_Min(40,State.Address.DelayIO-dwT));
	}
	//scxSendDebugConnexion("scxRS232Write","DelayIO: end extra delay",C);
	return;
}
*/

static void SPG_CONV scxRS232DelayIO(SCX_STATE& State)
{
	double T;
	S_GetTimerRunningTime(State.LastIO,T);
	while( (1000*T < State.Address.DelayIO)
		   &&(SPG_GLOBAL_ETAT(MUSTEXIT)==0)
		 )
	{
		//scxSendDebugConnexion("scxRS232Write","DelayIO: extra delay",C);
		DoEvents(SPG_DOEV_ALL);
		SPG_Sleep(V_Min(40,State.Address.DelayIO-1000*T));
		S_GetTimerRunningTime(State.LastIO,T);
	}
	//scxSendDebugConnexion("scxRS232Write","DelayIO: end extra delay",C);
	return;
}

//----------------------------------------

static int SPG_CONV scxRS232Write(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	if(State.Address.DelayIO) scxRS232DelayIO(State);
	scxRS232ClearCommError(State);

	DWORD WrittenBytes=0;
	WriteFile(State.hCom,Data,DataLen,&WrittenBytes,0);
	//State.LastIO=GetTickCount();
	S_RestartTimer(State.LastIO);

	return WrittenBytes;
}

//----------------------------------------

static int SPG_CONV scxRS232Read(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	scxRS232ClearCommError(State);

	DWORD ReadBytes=0;
	ReadFile(State.hCom,Data,DataLen,&ReadBytes,NULL);
	if(ReadBytes) S_RestartTimer(State.LastIO);

	return DataLen=ReadBytes;
}

//----------------------------------------

static void SPG_CONV scxRS232CfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
/*
//
// DTR Control Flow Values.
//
#define DTR_CONTROL_DISABLE    0x00
#define DTR_CONTROL_ENABLE     0x01
#define DTR_CONTROL_HANDSHAKE  0x02

//
// RTS Control Flow Values
//
#define RTS_CONTROL_DISABLE    0x00
#define RTS_CONTROL_ENABLE     0x01
#define RTS_CONTROL_HANDSHAKE  0x02
#define RTS_CONTROL_TOGGLE     0x03

#define NOPARITY            0
#define ODDPARITY           1
#define EVENPARITY          2
#define MARKPARITY          3
#define SPACEPARITY         4

#define ONESTOPBIT          0
#define ONE5STOPBITS        1
#define TWOSTOPBITS         2
*/

		Address->Port=1;
		Address->Speed=115200;
		Address->DTRControl=DTR_CONTROL_ENABLE;
		Address->RTSControl=0;
		Address->Parity=NOPARITY; //no parity
		Address->StopBit=ONESTOPBIT; //0
		Address->InQueue=2048;
		Address->OutQueue=2048;
		Address->DelayIO=0;
		Address->QuietOverrun=0;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,CFG.FileName[0]?1:0);

	CFG_IntParam(CFG,"Port",&Address->Port,"1:COM1 2:COM2 ...",1,CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"Speed",&Address->Speed,"9600...115200 bps",1,CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"DTRControl",&Address->DTRControl,"0:disable 1:enable 2:handshake",1,CP_INT|CP_HASMINMAX,0,2);
	CFG_IntParam(CFG,"RTSControl",&Address->RTSControl,"0:disable 1:enable 2:handshake 3:toggle",1,CP_INT|CP_HASMINMAX,0,3);
	CFG_IntParam(CFG,"Parity",&Address->Parity,"0:none 1:odd 2:even 3:mark 4:space",1,CP_INT|CP_HASMINMAX,0,4);
	CFG_IntParam(CFG,"StopBit",&Address->StopBit,"0:one 1:one and half 2:two",1,CP_INT|CP_HASMINMAX,0,2);
	CFG_IntParam(CFG,"InQueue",&Address->InQueue,0,1,CP_INT|CP_HASMIN,0);
	CFG_IntParam(CFG,"OutQueue",&Address->OutQueue,0,1,CP_INT|CP_HASMIN,0);
	CFG_IntParam(CFG,"DelayIO",&Address->DelayIO,"Delay between successive I/O (ms)",1,CP_INT|CP_HASMIN,0);
	CFG_IntParam(CFG,"QuietOverrun",&Address->QuietOverrun,"Disable Overrun error report",1,CP_INT|CP_HASMIN,0);
	CFG_IntParam(CFG,"ExtendedSyntax",&Address->ExtendedSyntax,"Standard:COM1-COM9 Extended:\\\\.\\COM10",1,CP_INT|CP_HASMIN,0);
	return;
}









//----------------------------------------

static int SPG_CONV scxRS232SetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV scxRS232GetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV sciRS232DestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


//----------------------------------------

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciRS232CreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="RS232 port";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=MAXRS232SIZE;//spécifique

	CI->scxOpen=scxRS232Open;
	CI->scxClose=scxRS232Close;
	CI->scxWrite=scxRS232Write;
	CI->scxRead=scxRS232Read;
	CI->scxCfgAddress=scxRS232CfgAddress;
	CI->scxSetParameter=scxRS232SetParameter;
	CI->scxGetParameter=scxRS232GetParameter;
	CI->sciDestroyConnexionInterface=sciRS232DestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

//----------------------------------------

#endif
#endif
