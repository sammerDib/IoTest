
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#if 0

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

// ###################################################

#define sci_UID sci_UID_BPRS
#define sci_NAME sci_NAME_BPRS

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

// ###################################################


// ###################################################

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
} BPRSW_CONNEXION_ADDRESS;

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
} BPRSR_CONNEXION_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	int Channels;
	int HighPassSamples0;
	int HighPassSamples1;
	int LowPassSamples0;
	int LowPassSamples1;

	BPRSW_CONNEXION_ADDRESS W;
	BPRSR_CONNEXION_ADDRESS R;
} SCX_ADDRESS;

typedef struct
{
	SCX_CONNEXION* C;
	BPRS_CHANNEL_FILT* CF;
} BPRS_CONNEXION_STATE;


// ###################################################

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	int CommonDelay;
	BPRS_CONNEXION_STATE W;
	BPRS_CONNEXION_STATE R;
} SCX_STATE; //parametres d'une connexion en particulier

// ###################################################

#include "SCM_ExtensWriteThrough.h"

#include "SCM_ExtensProtocol.h"

// ###################################################

//efine MAXBPRSSIZE 8192

// ###################################################

static SCX_CONNEXION* SPG_CONV scxBPRSOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	CHECK(scxOpenProtocol(State,scxOpenFlag)==0,"scxBPRSOpen",scxCloseProtocol(State);scxFree(C->State);scxFree(C);return 0);
	scxProtocolInheritUserExtension(C,State.W.C,State.R.C);

	State.R.CF=CF_Init(State.Address);		State.W.CF=CF_Init(State.Address);
	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxBPRSClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	scxCloseProtocol(State);

	//desallocation STATE

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static void SPG_CONV PBRS_Process(SCX_STATE& State, BPRS_CONNEXION_STATE& SP, short* Data, int N)
{
	CHECK(N%State.Address.Channels,"PBRS_Process",return);
	int x_max=N/State.Address.Channels;

	for(int x=0;x<x_max;x++)
	{
		for(int c=0;c<State.Address.Channels;c++)
		{
			RS_SAMPLE& S=Data[N*x+c];

			RS_SUM H,L;
			SPG_FastConvFiltIn(SP.CF[c].FH0,S);	SPG_FastConvFiltOut(SP.CF[c].FH0,H);
			SPG_FastConvFiltIn(SP.CF[c].FL0,S);	SPG_FastConvFiltOut(SP.CF[c].FL0,L);
			SPG_FastConvFiltIn(SP.CF[c].FH1,H);	SPG_FastConvFiltOut(SP.CF[c].FH1,H);
			SPG_FastConvFiltIn(SP.CF[c].FL1,L);	SPG_FastConvFiltOut(SP.CF[c].FL1,L);

			RS_SUM Z;
			SPG_FastConvzIn(SP.CF[c].FZ,S); SPG_FastConvzOut(SP.CF[c].FZ,Z);

			//normalisation à faire
			//RS_SUM T=L/State.Address.LowPassSamples-H/State.Address.HighPassSamples;
			//S=V_Sature(T,-32768,32767);
		}
	}
	return;
}


// ###################################################

static int SPG_CONV scxBPRSWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;
	CHECK(State.W.C==0,"scxBPRSWrite",return 0);
	BPRSW_CONNEXION_ADDRESS& AP=State.Address.W;
	BPRS_CONNEXION_STATE& SP=State.W;

	CHECK(DataLen%(State.Address.Channels*sizeof(SHORT)),"scxBPRSWrite",return 0);
	PBRS_Process(State,SP,(short*)Data,DataLen/(sizeof(short)*State.Address.Channels));

	return scxWrite(Data,DataLen,SP.C);
}

// ###################################################

//#define PELT (*pkBufferGetLastPacket(PB)) //can not be null
//PELT = PB.P[P.NumPacket-1]

static int SPG_CONV scxBPRSRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxBPRSRead",return DataLen=0);
	BPRSR_CONNEXION_ADDRESS& AP=State.Address.R;
	BPRS_CONNEXION_STATE& SP=State.R;

	scxRead(Data,DataLen,SP.C);

	PBRS_Process(State,SP,(short*)Data,DataLen/(sizeof(short)*State.Address.Channels));

	return DataLen;
}

// ###################################################

static void SPG_CONV scxBPRSCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values

		Address->W.Address=0;
		Address->W.CI=0;

		Address->R.Address=0;
		Address->R.CI=0;

		Address->HighPassSamples0=1024;
		Address->HighPassSamples1=1600;
		Address->LowPassSamples0=7;
		Address->LowPassSamples1=5;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);

	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	//Les noms W.Address et W.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);

	//Les noms R.Address et R.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	
	CFG_IntParam(CFG,"HighPassSamples0",	&Address->HighPassSamples0,0,1,		CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"HighPassSamples1",	&Address->HighPassSamples1,0,1,		CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"LowPassSamples0",	&Address->LowPassSamples0,0,1,		CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"LowPassSamples1",	&Address->LowPassSamples1,0,1,		CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"Channels",	&Address->Channels,0,1,					CP_INT);

	
	return;
}

// ###################################################

static int SPG_CONV scxBPRSSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxBPRSGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciBPRSDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciBPRSCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPROTOCOL);
	strcpy(CI->Name,sci_NAME);

	CI->Description="BandPassRawShort";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;//spécifique

	CI->scxOpen=scxBPRSOpen;
	CI->scxClose=scxBPRSClose;
	CI->scxWrite=scxBPRSWrite;
	CI->scxRead=scxBPRSRead;
	CI->scxCfgAddress=scxBPRSCfgAddress;
	CI->scxSetParameter=scxBPRSSetParameter;
	CI->scxGetParameter=scxBPRSGetParameter;
	CI->sciDestroyConnexionInterface=sciBPRSDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif
