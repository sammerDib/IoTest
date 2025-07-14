
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USECAMERAEMULEE

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>


#define sci_UID sci_UID_CAMEMULEE
#define sci_NAME sci_NAME_CAMEMULEE

#define SCX_LENFILENAME 1024

// ###################################################

#include "SCM_ExtensCamera.h"

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

#define DEFAULTFILENAME "CamEmulee.bmp"

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	char FileName[SCX_LENFILENAME];
	int BufferCount;
	float PixelSizeX;
	float PixelSizeY;
} SCX_ADDRESS;


// ###################################################

typedef struct
{
	//int SizeE;
	SNAKE SFree;
	//SNAKE SActive;
	SNAKE SFull;
	BYTE* B;
} CAMERASTATE;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	G_Ecran E;
	CAMERASTATE CS;
	int Frame;
	LOGFILE LF;
} SCX_STATE; //parametres d'une connexion en particulier

static SCX_EXTCAMGETSIZE(scxCAMEMULEEGetSize); //int& SizeX, int& SizeY, int& SizePix, float& PixelSizeX, float& PixelSizeY
static SCX_EXTCAMGETSTATUS(scxCAMEMULEEGetStatus); //int& Flag

// ###################################################

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

static SCX_CONNEXION* SPG_CONV scxCAMEMULEEOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	G_InitEcranFromFile(State.E,1,0,State.Address.FileName);

	//State.CS.SizeE=G_SizeX(State.E)*G_SizeY(State.E)*G_POCT(State.E);

	SnakeInit(State.CS.SFree,1,SNAKE_UNORDERED,"Free");
	//SnakeInit(State.CS.SActive,1,0);
	SnakeInit(State.CS.SFull,1,0,"Full");
	State.CS.B=SPG_TypeAlloc(State.E.Size*State.Address.BufferCount,BYTE,"scxCAMEMULEEOpen");
	SnakeAppend(State.CS.SFree,State.CS.B,State.E.Size*State.Address.BufferCount);

	LogfileInit(State.LF,0,0,1024*1024,1,0);
	LogfileEmptyLine(State.LF);

	scxGetSnakeStatus("scxCAMEMULEEOpen",State);

	C->UserFctPtr[sci_EXT_CAMGETSIZE]=(SCX_USEREXTENSION)scxCAMEMULEEGetSize;
	C->UserFctPtr[sci_EXT_CAMGETSTATUS]=(SCX_USEREXTENSION)scxCAMEMULEEGetStatus;
	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxCAMEMULEEClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	LogfileEmptyLine(State.LF);
	scxGetSnakeStatus("scxCAMEMULEEClose",State);
	SnakeClose(State.CS.SFree);
	//SnakeClose(State.CS.SActive);
	SnakeClose(State.CS.SFull);
	G_CloseEcran(State.E);
	SPG_MemFree(State.CS.B);

	LogfileClose(State.LF);

	scxFree(C->State); scxFree(C); 
	return scxOK;
}

// ###################################################

static int SPG_CONV scxCAMEMULEEWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	return 0;
}


static SCX_EXTCAMGETSIZE(scxCAMEMULEEGetSize) //int& SizeX, int& SizeY, int& SizePix, float& PixelSizeX, float& PixelSizeY
{
	scxCHECK(C, "scxCAMEMULEEGetSize");
	SCX_STATE& State=*C->State;
	
	SizeX=G_SizeX(State.E);
	SizeY=G_SizeY(State.E);
	SizePix=G_POCT(State.E);
	PixelSizeX=State.Address.PixelSizeX;
	PixelSizeY=State.Address.PixelSizeY;

	return scxOK;
}

static SCX_EXTCAMGETSTATUS(scxCAMEMULEEGetStatus) //int& Flag
{
	scxCHECK(C, "scxCAMEMULEEGetStatus");
	return scxCamReady;
}

// ###################################################

static void SPG_CONV scxCAMEMULEEUpdate(SCX_CONNEXION* C)
{
	SCX_STATE& State=*C->State;

	G_DrawCircle(State.E,5+State.Frame,6+2*State.Frame,3+State.Frame,0);
	G_DrawCircle(State.E,5+State.Frame,6+2*State.Frame,4+State.Frame,-1);
	G_DrawCircle(State.E,5+State.Frame,256-2*State.Frame,3+State.Frame,0);
	G_DrawCircle(State.E,5+State.Frame,256-2*State.Frame,4+State.Frame,-1);
	State.Frame++;

	LogfileEmptyLine(State.LF);
	scxGetSnakeStatus("scxCAMEMULEEUpdate",State);
	SNAKESEG G0;
	if(SnakeLockLinear(State.CS.SFree,G0)<State.E.Size) 
	{
		scxGetSnakeStatus("scxCAMEMULEEUpdate - Insuficient available lock length",State);
		return;
	}
	memcpy(G0.D,G_MECR(State.E),State.E.Size);
	SnakeCommit(State.CS.SFree,G0.bD+State.E.Size,&State.CS.SFull);
	scxGetSnakeStatus("scxCAMEMULEEUpdate - Commited",State);
	return;
}

static int SPG_CONV scxCAMEMULEERead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	scxCAMEMULEEUpdate(C);

	LogfileEmptyLine(State.LF);
	scxGetSnakeStatus("scxCAMEMULEERead",State);
	int ReadLen=0;

	for(int i=0;i<MAXSNAKESEG;i++)
	{
		SNAKESEG G0;
		if(SnakeLock1(State.CS.SFull,G0)==0) 
		{
			scxGetSnakeStatus("scxCAMEMULEERead - Return",State);
			return DataLen=ReadLen;
		}
		int L=V_Min(G0.N*State.CS.SFull.sz,DataLen-ReadLen);
		memcpy(((BYTE*)Data)+ReadLen,G0.D,L);
		ReadLen+=L;
		SnakeCommit(State.CS.SFull,G0.bD+L,&State.CS.SFree);
		if(ReadLen==DataLen) return DataLen;
	}
	DbgCHECK(1,"scxCAMEMULEERead : SNAKE buffer read error");
	return DataLen=ReadLen;
}

// ###################################################

static void SPG_CONV scxCAMEMULEECfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->BufferCount=3;
		Address->PixelSizeX=1;
		Address->PixelSizeY=1;
		strcpy(Address->FileName,DEFAULTFILENAME);
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	CFG_FloatParam(CFG,"PixelSizeX",&Address->PixelSizeX,"Physical dimension of pixel",1);
	CFG_FloatParam(CFG,"PixelSizeY",&Address->PixelSizeY,"Physical dimension of pixel",1);
	CFG_IntParam(CFG,"BufferCount",&Address->BufferCount,"# of buffers (recommended 3)",1);
	CFG_StringParam(CFG,"FileName",Address->FileName,0,1);
	return;
}

// ###################################################

static int SPG_CONV scxCAMEMULEESetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV scxCAMEMULEEGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV sciCAMEMULEEDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}

// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciCAMEMULEECreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Camera Emulee";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;

	CI->scxOpen=scxCAMEMULEEOpen;
	CI->scxClose=scxCAMEMULEEClose;
	CI->scxWrite=scxCAMEMULEEWrite;
	CI->scxRead=scxCAMEMULEERead;
	CI->scxCfgAddress=scxCAMEMULEECfgAddress;
	CI->scxSetParameter=scxCAMEMULEESetParameter;
	CI->scxGetParameter=scxCAMEMULEEGetParameter;
	CI->sciDestroyConnexionInterface=sciCAMEMULEEDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif
