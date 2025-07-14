
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USECAMERAPAD

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>


#define sci_UID sci_UID_CAMPAD
#define sci_NAME sci_NAME_CAMPAD

#define SCX_LENFILENAME 1024



// ###################################################


typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
} PADR_ADDRESS;

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
} PADW_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	PADW_ADDRESS R;
	PADW_ADDRESS W;
	
	int BufferCount;
	int SizeX;
	int SizeY;
	int SizePix;
	float PixelSizeX;
	float PixelSizeY;
	int FileFrameDelay;


} SCX_ADDRESS;


// ###################################################

typedef struct
{
	SCX_CONNEXION* C;
} PADR_STATE;

typedef struct
{
	SCX_CONNEXION* C;
} PADW_STATE;

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
	PADR_STATE R;
	PADW_STATE W;
	CAMERASTATE CS;

	int szFrame;
	int szHeader;
	int szData;
	int szTrailer;

	int Frame;
	DWORD ReadTime;
	//LOGFILE LF;
} SCX_STATE; //parametres d'une connexion en particulier


// ###################################################

#include "SCM_ExtensCamera.h"
#include "SCM_ExtensProtocol.h"


#define CHAR_STX '\02'
#define CHAR_ETX '\03'

static int SPG_CONV scxCAMPADGetStatus(int& Flag, SCX_CONNEXION* C);
static SCX_EXTCAMGETSIZE(scxCAMPADGetSize); //int& SizeX, int& SizeY, int& SizePix, float& PixelSizeX, float& PixelSizeY

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

static SCX_CONNEXION* SPG_CONV scxCAMPADOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	CHECK(scxOpenProtocol(State,scxOpenFlag)==0,"scxCAMPADOpen",scxFree(C->State);scxFree(C);return 0);

	SnakeInit(State.CS.SFree,1,SNAKE_UNORDERED,"Free");

	SnakeInit(State.CS.SFull,1,0,"Full");


	State.szHeader=1;
	State.szData=State.Address.SizeX*State.Address.SizeY*State.Address.SizePix;
	State.szTrailer=1;
	State.szFrame=State.szHeader+State.szData+State.szTrailer;

	State.CS.B=SPG_TypeAlloc(State.szFrame*State.Address.BufferCount,BYTE,"scxCAMPADOpen");

	SnakeAppend(State.CS.SFree,State.CS.B,State.szFrame*State.Address.BufferCount);

	/*
	LogfileInit(State.LF,0,0,1024*1024,1,0);
	LogfileEmptyLine(State.LF);
	scxGetSnakeStatus("scxCAMPADOpen",State);
	*/

	C->UserFctPtr[sci_EXT_CAMGETSIZE]=(SCX_USEREXTENSION)scxCAMPADGetSize;
	C->UserFctPtr[sci_EXT_CAMGETSTATUS]=(SCX_USEREXTENSION)scxCAMPADGetStatus;

	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxCAMPADClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	/*
	LogfileEmptyLine(State.LF);
	scxGetSnakeStatus("scxCAMPADClose",State);
	*/
	SnakeClose(State.CS.SFree);
	//SnakeClose(State.CS.SActive);
	SnakeClose(State.CS.SFull);

	SPG_MemFree(State.CS.B);

	/*
	LogfileClose(State.LF);
	*/

	scxCloseProtocol(State);

	scxFree(C->State); scxFree(C); 
	return scxOK;
}

// ###################################################

static int SPG_CONV scxCAMPADWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	return scxWrite(Data,DataLen,State.W.C);
}


static SCX_EXTCAMGETSIZE(scxCAMPADGetSize) //int& SizeX, int& SizeY, int& SizePix, float& PixelSizeX, float& PixelSizeY
{
	scxCHECK(C, "scxCAMPADGetSize");
	SCX_STATE& State=*C->State;
	
	SizeX=State.Address.SizeX;
	SizeY=State.Address.SizeY;
	SizePix=State.Address.SizePix;
	PixelSizeX=State.Address.PixelSizeX;
	PixelSizeY=State.Address.PixelSizeY;

	return scxOK;
}

// ###################################################

static void SPG_CONV scxCAMPADUpdate(SCX_CONNEXION* C)
{
	SCX_STATE& State=*C->State;
	SNAKESEG G0;
	//CHECK(SnakeLock1(State.CS.SFree,G0)<=0,"scxCAMPADUpdate: Insufficient buffering",return);
	if(SnakeLock1(State.CS.SFree,G0)<=0) return;
	scxRead(G0.D,G0.N,State.R.C);
	State.ReadTime=GetTickCount();
	if(G0.N) SnakeCommit(State.CS.SFree,G0.bD+G0.N,&State.CS.SFull);
	return;
}

static int SPG_CONV scxCAMPADGetStatus(int& Flag, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxCAMPADGetStatus");
	SCX_STATE& State=*C->State;

	if(SnakeGetTotalLength(State.CS.SFull)<State.szFrame) 
		return scxCamWaiting;
	else
		return scxCamReady;
}

static int SPG_CONV scxCAMPADRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	CHECK(DataLen<State.szData,"",return DataLen=0);

	if( scxIsTypeUID(State.R.C,sci_UID_FILE) &&
		( (GetTickCount()-State.ReadTime) < State.Address.FileFrameDelay ) ) return DataLen=0;

	scxCAMPADUpdate(C);


scxCAMPADRead_FindHeader:

	{
		int i=0;
		for(;i<MAXSNAKESEG;i++)
		{
			SNAKESEG G0;
			if(SnakeLock1(State.CS.SFull,G0)==0) 
			{
				return DataLen=0; // no more data to read/STX not found
			}
			int p=0;
			for(;p<G0.N;p++)
			{
				if(G0.bD[p]==CHAR_STX) break;
			}
			SnakeCommit(State.CS.SFull,G0.bD+p,&State.CS.SFree);//discard everything before STX
			if(p<G0.N) break;
		}
		if(i==MAXSNAKESEG) return DataLen=0; //no STX
	}

	//if(SnakeGetTotalLength(State.CS.SFull)<State.szFrame) return DataLen=0; //pas de données suffisantes
	BYTE* B=SnakeGetReadPtr(State.CS.SFull,State.szFrame-1);
	if(B==0) return DataLen=0; //pas de données suffisantes
	if(*B!=CHAR_ETX) 
	{
		//DbgCHECK(1,"scxCAMPADRead: Check parameter file\nSizePix = 2 (Pad 8 bits)\nSizePix = 4 (Pad 12 bits)");
		SNAKESEG G0; SnakeLock1(State.CS.SFull,G0); SnakeCommit(State.CS.SFull,G0.bD+1,&State.CS.SFree);
		goto scxCAMPADRead_FindHeader;//consomme le stx mal placé et relance la recherche
	}

	int HeaderLen=0;
	DataLen=0; // cas DataLen<State.szData already discarded
	int TrailerLen=0;

	int i;
	{for(i=0;i<MAXSNAKESEG;i++) //while(1) + borne au nombre d'iteration
	{
		SNAKESEG G0;
		CHECK(SnakeLock1(State.CS.SFull,G0)==0,"Inconsistent SnakeGetTotalLength/Lock", return DataLen);

		BYTE* pFirst=G0.bD;
		int L=V_Min(G0.N,State.szFrame-DataLen);
		
		if((L)&&(HeaderLen<State.szHeader))
		{
			int a=V_Min(L,State.szHeader-HeaderLen);
			HeaderLen+=a;
			CHECK(*G0.bD!=CHAR_STX,"scxCAMPADRead",goto scxCAMPADRead_FindHeader); //condition assurée par la boucle précédente
			G0.bD+=a;
			L-=a;
		}
		
		if((L)&&(DataLen<State.szData))
		{
			int a=V_Min(L,State.szData-DataLen);
			memcpy(((BYTE*)Data)+DataLen,G0.D,a);
			DataLen+=a;
			G0.bD+=a;
			L-=a;
		}

		if((L)&&(TrailerLen<State.szTrailer))
		{
			int a=V_Min(L,State.szTrailer-TrailerLen);
			TrailerLen+=a;
			CHECK(*G0.bD!=CHAR_ETX,"scxCAMPADRead",{SnakeCommit(State.CS.SFull,pFirst+1,&State.CS.SFree); goto scxCAMPADRead_FindHeader;});//discard current frame and search next header next time
			G0.bD+=a;
			L-=a;
		}
		
		SnakeCommit(State.CS.SFull,G0.bD,&State.CS.SFree);

		if(TrailerLen==State.szTrailer) break;//on a lu jusqu'à ETX
	}}
	DbgCHECK(i==MAXSNAKESEG,"scxCAMPADRead");//test de sortie du while(1)

	return DataLen;
}

// ###################################################

static void SPG_CONV scxCAMPADCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->BufferCount=3;
		Address->SizeX=16;
		Address->SizeY=16;
		Address->SizePix=4;
		Address->PixelSizeX=1;
		Address->PixelSizeY=1;
		Address->FileFrameDelay=80;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	//Les noms W.Address et W.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);

	//Les noms R.Address et R.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);

	CFG_IntParam(CFG,"BufferCount",&Address->BufferCount,"# of buffers (recommended 3)",1);
	CFG_IntParam(CFG,"SizeX",&Address->SizeX,0,1);
	CFG_IntParam(CFG,"SizeY",&Address->SizeY,0,1);
	CFG_IntParam(CFG,"SizePix",&Address->SizePix,"bytes per pixel transmitted through the comm link (2:Pad8bits or 4:Pad12bits)",1);
	CFG_FloatParam(CFG,"PixelSizeX",&Address->PixelSizeX,"Physical dimension of pixel",1);
	CFG_FloatParam(CFG,"PixelSizeY",&Address->PixelSizeY,"Physical dimension of pixel",1);
	CFG_IntParam(CFG,"FileFrameDelay",&Address->FileFrameDelay,0,1);
	return;
}

// ###################################################

static int SPG_CONV scxCAMPADSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV scxCAMPADGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV sciCAMPADDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}

// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciCAMPADCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciREAD|sciPACKETRESPECT|sciPROTOCOL);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Camera Pad";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;

	CI->scxOpen=scxCAMPADOpen;
	CI->scxClose=scxCAMPADClose;
	CI->scxWrite=scxCAMPADWrite;
	//CI->UserFctPtr[sci_EXT_CAMGETSIZE]=(SCX_USEREXTENSION)scxCAMPADGetSize;
	//CI->UserFctPtr[sci_EXT_CAMGETSTATUS]=(SCX_USEREXTENSION)scxCAMPADGetStatus;
	CI->scxRead=scxCAMPADRead;
	CI->scxCfgAddress=scxCAMPADCfgAddress;
	CI->scxSetParameter=scxCAMPADSetParameter;
	CI->scxGetParameter=scxCAMPADGetParameter;
	CI->sciDestroyConnexionInterface=sciCAMPADDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif
