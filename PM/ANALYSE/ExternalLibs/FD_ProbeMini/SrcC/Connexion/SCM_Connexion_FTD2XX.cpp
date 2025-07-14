
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USEFTD2XX

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include "..\..\FTD2XX_SDK\ftd2xx.h"
#pragma comment(lib,"..\\FTD2XX_SDK\\i386\\ftd2xx.lib")

#include "SCM_ConnexionDbg.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_FTD2XX
#define sci_NAME sci_NAME_FTD2XX

//----------------------------------------

#define MAXFTD2XXSIZE 0

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


	int Port;
	int Speed;

} SCX_ADDRESS;



//----------------------------------------

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;

	FT_HANDLE H;

} SCX_STATE; //parametres d'une connexion en particulier


//----------------------------------------

static SCX_CONNEXION* SPG_CONV scxFTD2XXOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	CHECK(FT_Open(0, &State.H)!=FT_OK,"scxFTD2XXOpen",scxFree(C->State);scxFree(C);return 0);

	CHECK(FT_SetBaudRate (State.H, State.Address.Speed)!=FT_OK,"scxFTD2XXOpen",;); //57600
	CHECK(FT_SetDataCharacteristics (State.H, 8, 0, 0)!=FT_OK,"scxFTD2XXOpen",;);

	C->Etat=scxOK;
	return C;
}

//----------------------------------------

static int SPG_CONV scxFTD2XXClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	CHECK(FT_Close(State.H)!=FT_OK,"scxFTD2XXClose failed",;);

	scxFree(C->State);scxFree(C);
	return scxOK;
}

//----------------------------------------

static int SPG_CONV scxFTD2XXWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	DWORD WrittenBytes=0;
	FT_Write(State.H,Data,DataLen,&WrittenBytes);

	return WrittenBytes;
}

//----------------------------------------

static int SPG_CONV scxFTD2XXRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	DWORD ReadBytes=0;
	FT_Read(State.H,Data,DataLen,&ReadBytes);

	return DataLen=ReadBytes;
}

//----------------------------------------

static void SPG_CONV scxFTD2XXCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->Port=0;
		Address->Speed=57600;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,CFG.FileName[0]?1:0);

	CFG_IntParam(CFG,"Port",&Address->Port,"UTA Device Nr (from 0)",1,CP_INT);
	CFG_IntParam(CFG,"Speed",&Address->Speed,"UTA Device Speed",1,CP_INT|CP_HASMIN,1);
	return;
}









//----------------------------------------

static int SPG_CONV scxFTD2XXSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV scxFTD2XXGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV sciFTD2XXDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


//----------------------------------------

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciFTD2XXCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="FTD2XX port";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=MAXFTD2XXSIZE;//spécifique

	CI->scxOpen=scxFTD2XXOpen;
	CI->scxClose=scxFTD2XXClose;
	CI->scxWrite=scxFTD2XXWrite;
	CI->scxRead=scxFTD2XXRead;
	CI->scxCfgAddress=scxFTD2XXCfgAddress;
	CI->scxSetParameter=scxFTD2XXSetParameter;
	CI->scxGetParameter=scxFTD2XXGetParameter;
	CI->sciDestroyConnexionInterface=sciFTD2XXDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

//----------------------------------------

#endif
#endif
