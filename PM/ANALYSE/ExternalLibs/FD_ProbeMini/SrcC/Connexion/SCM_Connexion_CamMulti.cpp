
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USECAMMULTI

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>


#define sci_UID sci_UID_CAMMULTI
#define sci_NAME sci_NAME_CAMMULTI

#define SCX_LENFILENAME 1024

#include "..\..\Multicam_SDK\Include\multicam.h"
#pragma comment(lib,"..\\Multicam_SDK\\lib\\multicam.lib")


// ###################################################


typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	
	int Topology;
	char Connector[SPG_CONFIGSTRINGLEN];
	char CamFile[SPG_CONFIGSTRINGLEN];
	int BufferCount;
	float PixelSizeX;
	float PixelSizeY;
	int bSetGainAndOffset;
	int Gain;
	int Offset;
	int ActiveOnInit;

} SCX_ADDRESS;


// ###################################################

typedef struct
{
	//int SizeE;
	SNAKE SFree;
	SPINLOCK LFree;

	SNAKE SFull;
	SPINLOCK LFull;

	int BLen;
	BYTE* B;

	SPINLOCK L;
} CAMERASTATE;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	CAMERASTATE CS;
	unsigned int  m_Channel;

	int SizeX;
	int SizeY;
	int SizePix;
	int Pitch;

	int szFrame;


	int Frame;
	//LOGFILE LF;
} SCX_STATE; //parametres d'une connexion en particulier


// ###################################################

#include "SCM_ExtensCamera.h"

void WINAPI scxCAMMULTICallback(PMCSIGNALINFO SigInfo);

// ###################################################

/*
static void SPG_CONV scxGetSnakeStatus(char* Fct, SCX_STATE& State)
{
	LogfileEmptyLine(State.LF);
	Logfile(State.LF,Fct);
	char Msg[512];
	SnakeGetStatus(Msg, State.CS.SFree);
	LogfileEmptyLine(State.LF);
	Logfile(State.LF,Msg);
	SnakeGetStatus(Msg, State.CS.SFull);
	LogfileEmptyLine(State.LF);
	Logfile(State.LF,Msg);
	return;
}
*/

/*
static bool SPG_CONV scxCSLocked(CAMERASTATE& CS)
{
	bool L=true;
	for(int i=0;i<5;i++)
	{
		L=true;
		V_SWAP(bool,L,CS.Locked);
		if(L==false) break;
		SPG_Sleep(0);
	}
	return L;
}
*/

static int SPG_CONV scxCAMMULTIWrite(void* Data, int DataLen, SCX_CONNEXION* C);
static SCX_EXTCAMGETSIZE(scxCAMMULTIGetSize); //int& SizeX, int& SizeY, int& SizePix, float& PixelSizeX, float& PixelSizeY
static int SPG_CONV scxCAMMULTIGetStatus(SCX_CONNEXION* C);
static int SPG_CONV scxCAMMULTIRead(void* Data, int& DataLen, SCX_CONNEXION* C);


static SCX_CONNEXION* SPG_CONV scxCAMMULTIOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

    // Set the board topology
    // Uncomment the folowing lines and set the board topology corresponding to your setup.
	
    //CHECK(	McSetParamInt(MC_BOARD + 0, MC_BoardTopology, MC_BoardTopology_2_2)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_exit);
	//CHECK(	McSetParamStr(MC_BOARD + 0, MC_BoardTopology, "1_1")	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_exit);
	CHECK(	McSetParamInt(MC_BOARD + 0, MC_BoardTopology, State.Address.Topology)	!=MC_OK,"scxCAMMULTIOpen\nBoard 0 not existing or Camera already in use\n",goto scxCAMMULTI_exit);

    // Create a channel and associate it with the first connector on the first board
   	CHECK(	McCreate(MC_CHANNEL, &State.m_Channel)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_exit);
   	//CHECK(	State.m_Channel==0x80000000,"scxCAMMULTIOpen",goto scxCAMMULTI_exit);
   	CHECK(	McSetParamInt(State.m_Channel, MC_DriverIndex, 0)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);

    CHECK(	McSetParamStr(State.m_Channel, MC_Connector, State.Address.Connector)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);

    // Choose the video standard
    CHECK(	McSetParamStr(State.m_Channel, MC_CamFile, State.Address.CamFile)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);

    // Choose the camera expose duration
	//Address.Exposure=20000;
    //McSetParamInt(m_Channel, MC_Expose_us, 20000);
    // Choose the pixel color format
    CHECK(	McSetParamInt(State.m_Channel, MC_ColorFormat, MC_ColorFormat_Y8)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);
	State.SizePix=1;

    // Choose the way the first acquisition is triggered
    CHECK(	McSetParamInt(State.m_Channel, MC_TrigMode, MC_TrigMode_IMMEDIATE)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);
    // Choose the triggering mode for subsequent acquisitions
    CHECK(	McSetParamInt(State.m_Channel, MC_NextTrigMode, MC_NextTrigMode_REPEAT)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);
    // Choose the number of images to acquire
    CHECK(	McSetParamInt(State.m_Channel, MC_SeqLength_Fr, MC_INDETERMINATE)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);

	if(State.Address.bSetGainAndOffset)
	{
		CHECK(	McSetParamInt(State.m_Channel, MC_Gain, State.Address.Gain)	!=MC_OK,"scxCAMMULTIOpen",;);
		CHECK(	McSetParamInt(State.m_Channel, MC_Offset, State.Address.Offset)	!=MC_OK,"scxCAMMULTIOpen",;);
	}

    // Retrieve image dimensions
    CHECK(	McGetParamInt(State.m_Channel, MC_ImageSizeX, &State.SizeX)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);
    CHECK(	McGetParamInt(State.m_Channel, MC_ImageSizeY, &State.SizeY)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);
    CHECK(	McGetParamInt(State.m_Channel, MC_BufferPitch, &State.Pitch)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_chexit);

	SPL_Init(State.CS.LFull,3,"scxCAMMULTIOpen"); //3 -> 6ms de timeout, 30 -> 450ms de timeout
	SPL_Init(State.CS.LFree,3,"scxCAMMULTIOpen"); //3 -> 6ms de timeout, 30 -> 450ms de timeout

	State.szFrame=State.SizeX*State.SizeY;
	SnakeInit(State.CS.SFree,1,SNAKE_UNORDERED,"Free");
	SnakeInit(State.CS.SFull,1,0,"Full");
	State.CS.BLen=State.szFrame*State.Address.BufferCount;
	State.CS.B=SPG_TypeAlloc(State.CS.BLen,BYTE,"scxCAMMULTIOpen");
	SnakeAppend(State.CS.SFree,State.CS.B,State.szFrame*State.Address.BufferCount);

	C->UserFctPtr[sci_EXT_CAMGETSIZE]=(SCX_USEREXTENSION)scxCAMMULTIGetSize;
	C->UserFctPtr[sci_EXT_CAMGETSTATUS]=(SCX_USEREXTENSION)scxCAMMULTIGetStatus;
	C->Etat=scxOK;

    CHECK(	McSetParamInt(State.m_Channel, MC_SignalEnable + MC_SIG_SURFACE_PROCESSING, MC_SignalEnable_ON)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_lkexit);
    CHECK(	McRegisterCallback(State.m_Channel, scxCAMMULTICallback, C)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_lkexit);
	if(State.Address.ActiveOnInit) CHECK(	McSetParamInt(State.m_Channel, MC_ChannelState, MC_ChannelState_ACTIVE)	!=MC_OK,"scxCAMMULTIOpen",goto scxCAMMULTI_lkexit);
	SPL_Exit(State.CS.LFull);
	SPL_Exit(State.CS.LFree);
	return C;

scxCAMMULTI_lkexit:
	SnakeClose(State.CS.SFree);
	SPL_Close(State.CS.LFree);
	SnakeClose(State.CS.SFull);
	SPL_Close(State.CS.LFull);
	SPG_MemFree(State.CS.B);
	SPL_Close(State.CS.L);

scxCAMMULTI_chexit:
	McDelete(State.m_Channel);

scxCAMMULTI_exit:
	scxFree(C->State); scxFree(C); 
	return 0;
}

// ###################################################

static int SPG_CONV scxCAMMULTIClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;

	C->Etat=scxINVALID; //empeche la callback de chercher le lock

	CHECK(	McSetParamInt(State.m_Channel, MC_ChannelState, MC_ChannelState_IDLE)	!=MC_OK,"scxCAMMULTIClose",;);
	//CHECK(	McRegisterCallback(State.m_Channel, 0, 0)	!=MC_OK,"scxCAMMULTIClose",;);

	CHECK(SPL_Enter(State.CS.LFull,"scxCAMMULTIClose")==SPL_TIMEOUT,"scxCAMMULTIClose",;);
	CHECK(SPL_Enter(State.CS.LFree,"scxCAMMULTIClose")==SPL_TIMEOUT,"scxCAMMULTIClose",;);

	McDelete(State.m_Channel);
	State.m_Channel=0;

	SnakeClose(State.CS.SFree);
	SnakeClose(State.CS.SFull);
	SPG_MemFree(State.CS.B);

	SPL_Close(State.CS.LFree);
	SPL_Close(State.CS.LFull);
	scxFree(C->State); scxFree(C); 
	return scxOK;
}

// ###################################################

static int SPG_CONV scxCAMMULTIWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	return DataLen=0;
}

static SCX_EXTCAMGETSIZE(scxCAMMULTIGetSize) //int& SizeX, int& SizeY, int& SizePix, float& PixelSizeX, float& PixelSizeY
{
	scxCHECK(C, "scxOverwrite");
	SCX_STATE& State=*C->State;
	
	SizeX=State.SizeX;
	SizeY=State.SizeY;
	SizePix=State.SizePix;
	PixelSizeX=State.Address.PixelSizeX;
	PixelSizeY=State.Address.PixelSizeY;

	return scxOK;
}

// ###################################################

void WINAPI scxCAMMULTICallback(PMCSIGNALINFO SigInfo)
{
    if ( (SigInfo==0) || (SigInfo->Context==0) ) return;

	switch(SigInfo->Signal)
    {
        case MC_SIG_SURFACE_PROCESSING:
			{
				SCX_CONNEXION* C = (SCX_CONNEXION*) SigInfo->Context;
				if(C==0) return; if(C->Etat!=scxOK) return;
				CHECK(C->State==0,"scxCAMMULTICallback",return); 
				SCX_STATE& State=*C->State; CAMERASTATE& CS=State.CS;

				// Retreive "current" surface address pointer
				BYTE* pSurface=0; MCHANDLE hSurface = (MCHANDLE)(SigInfo->SignalInfo); CHECK(hSurface==0,"scxCAMMULTICallback",goto scxCallbackNormalExit);
				McGetParamInt(hSurface,MC_SurfaceAddr,(PINT32)&pSurface); CHECK(pSurface==0,"scxCAMMULTICallback",goto scxCallbackFreeExit);

				SNAKESEG G0; 
				if(SnakeRead1(CS.SFree,CS.LFree,G0,State.szFrame))
				{
					BYTE* D=(BYTE*)G0.D; CHECK(D==0,"scxCAMMULTICallback",goto scxCallbackFreeExit);
					int Line=State.SizeX*State.SizePix;
					for(int y=0;y<State.SizeY;y++)
					{
						CHECK(!V_InclusiveBound(D,State.CS.B,State.CS.B+State.CS.BLen-Line),"scxCAMMULTICallback",goto scxCallbackFreeExit);
						memcpy(D,pSurface,Line);
						D+=Line; pSurface+=State.Pitch;
					}
					SnakeWrite1(CS.SFull,CS.LFull,G0);
				}
scxCallbackFreeExit:
				McSetParamInt(hSurface,MC_SurfaceState,MC_SurfaceState_FREE);
scxCallbackNormalExit:
				return;
			}
            break;
        //case MC_SIG_ACQUISITION_FAILURE:
	    default:
            break;

    }
	return;
}

static int SPG_CONV scxCAMMULTIGetStatus(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxCAMMULTIGetStatus");
	SCX_STATE& State=*C->State;
	if(SnakeGetTotalLength(State.CS.SFull)>=State.szFrame) return scxCamReady;
	return scxCamWaiting;
}

static int SPG_CONV scxCAMMULTIRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;
	CHECK(DataLen<State.szFrame,"scxCAMMULTIRead",return DataLen=0);
	SNAKESEG G0; G0.N=0;
	if(SnakeRead1(State.CS.SFull,State.CS.LFull,G0,State.szFrame)) { memcpy(Data,G0.D,G0.N); SnakeWrite1(State.CS.SFree,State.CS.LFree,G0); }
	return DataLen=G0.N;
}

// ###################################################

static void SPG_CONV scxCAMMULTICfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->Topology = MC_BoardTopology_2_2;
 		strcpy(Address->Connector,"Y");
 		strcpy(Address->CamFile,"XC-HR58_P50SA");//.cam
		Address->BufferCount=3;
		Address->PixelSizeX=1;
		Address->PixelSizeY=1;
		Address->bSetGainAndOffset=0;
		Address->Gain=0;
		Address->Offset=0;
		Address->ActiveOnInit=1;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	/*
C:\USERS\SYLVAIN\DEVC\MULTICAM_SDK\INCLUDE\McParams.h(188):#define MC_BoardTopology_2_2                      14
C:\USERS\SYLVAIN\DEVC\MULTICAM_SDK\INCLUDE\McParams.h(189):#define MC_BoardTopology_2_1                      15
C:\USERS\SYLVAIN\DEVC\MULTICAM_SDK\INCLUDE\McParams.h(190):#define MC_BoardTopology_1_1                      16
C:\USERS\SYLVAIN\DEVC\MULTICAM_SDK\INCLUDE\McParams.h(6562):#define MC_BoardTopology_1_2                      30
	*/

	CFG_IntParam(CFG,"Topology",&Address->Topology,"16:1_1 14:2_2 15:2_1 30:1_2",1);
	CFG_StringParam(CFG,"Connector",Address->Connector,"X or Y",1);
	CFG_StringParam(CFG,"CamFile",Address->CamFile,0,1);
	CFG_IntParam(CFG,"BufferCount",&Address->BufferCount,"# of buffers (recommended 3)",1);
	CFG_FloatParam(CFG,"PixelSizeX",&Address->PixelSizeX,"Physical dimension of pixel",1);
	CFG_FloatParam(CFG,"PixelSizeY",&Address->PixelSizeY,"Physical dimension of pixel",1);
	CFG_IntParam(CFG,"bSetGainAndOffset",&Address->bSetGainAndOffset,0,1);
	CFG_IntParam(CFG,"Gain",&Address->Gain,0,1);
	CFG_IntParam(CFG,"Offset",&Address->Offset,0,1);
	CFG_IntParam(CFG,"ActiveOnInit",&Address->ActiveOnInit,0,1);

	return;
}

// ###################################################

static int SPG_CONV scxCAMMULTISetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV scxCAMMULTIGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV sciCAMMULTIDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");

	McCloseDriver();

	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}

// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciCAMMULTICreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciREAD|sciPACKETRESPECT);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Camera Multicam";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;

	CI->scxOpen=scxCAMMULTIOpen;
	CI->scxClose=scxCAMMULTIClose;
	CI->scxWrite=scxCAMMULTIWrite;
	CI->scxRead=scxCAMMULTIRead;
	CI->scxCfgAddress=scxCAMMULTICfgAddress;
	CI->scxSetParameter=scxCAMMULTISetParameter;
	CI->scxGetParameter=scxCAMMULTIGetParameter;
	CI->sciDestroyConnexionInterface=sciCAMMULTIDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	McOpenDriver(0);
	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif
