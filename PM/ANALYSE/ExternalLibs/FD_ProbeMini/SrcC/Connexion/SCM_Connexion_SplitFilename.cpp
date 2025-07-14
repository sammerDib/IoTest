

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#include "SCM_Connexion_File_Internal.h"

// ###################################################

#include "SCM_ExtensOverwrite.h"

#define sci_UID sci_UID_SPLITFILE
#define sci_NAME sci_NAME_SPLITFILE

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

// ###################################################

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
} SPLITFW_CONNEXION_ADDRESS;

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
} SPLITFR_CONNEXION_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	int SplitSizeMin;
	int SplitSizeMax;

	SPLITFW_CONNEXION_ADDRESS W;
	SPLITFR_CONNEXION_ADDRESS R;
} SCX_ADDRESS;

typedef struct
{
	SCX_CONNEXION* C;
	int Total;
	int N;
	char BaseFilename[SCX_LENFILENAME];
	char ExtFilename[SCX_LENFILENAME];
} SPLITF_CONNEXION_STATE;

// ###################################################

#define P_FASTCONV 255

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	SPLITF_CONNEXION_STATE W;
	SPLITF_CONNEXION_STATE R;
} SCX_STATE; //parametres d'une connexion en particulier

// ###################################################

//nclude "SCM_ExtensWriteThrough.h"

#include "SCM_ExtensProtocol.h"

// ######### FONCTIONS SPECIFIQUES ######################

static int SPG_CONV scxSPLITFSetFileName(SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* CI, int scxCfgTerminal, char* BaseFilename, int N, char* ExtFilename)
{
	CHECK(Address==0,"scxSPLITFSetFileName",return 0);
	SPG_CONFIGFILE CFG; 
	SPG_ZeroStruct(CFG); 
	scxCfgAddress(CFG,Address,CI,scxCfgTerminal);
	char* F=0;
	CHECKTWO( ( F=CFG_GetStringParam(CFG,"FileName",0) ) == 0, "scxSPLITFSetFileName", BaseFilename, CFG_Close(CFG);return 0); 
	sprintf(F,"%s_%.03i.%s",BaseFilename,N,ExtFilename); 
	CFG_Close(CFG);
	return -1;
}

static int SPG_CONV scxSPLITFOpenRead(SCX_STATE& State)
{
	if(State.Address.R.Address)
	{
		State.R.Total=0;
		scxSPLITFSetFileName(State.Address.R.Address,State.Address.R.CI,scxCFGTERMINALREAD,State.R.BaseFilename,State.R.N,State.R.ExtFilename);
		State.R.C=scxOpen(State.Address.R.CI,State.Address.R.Address,SCXOPENPRESERVEADDRESS); //open preserve l'address R
	}
	return -1;
}

static int SPG_CONV scxSPLITFCloseRead(SCX_STATE& State)
{
	if(State.R.C) scxClose(State.R.C);
	State.R.Total=0;
	State.R.N++;
	return -1;
}

static int SPG_CONV scxSPLITFOpenWrite(SCX_STATE& State)
{
	if(State.Address.W.Address)
	{
		State.W.Total=0;
		scxSPLITFSetFileName(State.Address.W.Address,State.Address.W.CI,scxCFGTERMINALWRITE,State.W.BaseFilename,State.W.N,State.W.ExtFilename);
		State.W.C=scxOpen(State.Address.W.CI,State.Address.W.Address,SCXOPENPRESERVEADDRESS); //open preserve l'address W
	}
	return -1;
}

static int SPG_CONV scxSPLITFCloseWrite(SCX_STATE& State)
{
	if(State.W.C) scxClose(State.W.C);
	State.W.Total=0;
	State.W.N++;
	return -1;
}

// ###################################################

static SCX_EXTOVERWRITE(scxSPLITFOverwrite);

static SCX_CONNEXION* SPG_CONV scxSPLITFOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	if(State.Address.W.Address)
	{
		SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG); scxCfgAddress(CFG,State.Address.W.Address,State.Address.W.CI,scxCFGTERMINALWRITE);
		CFG_StringParam(CFG,"FileName",State.W.BaseFilename,0,1);
		strcpy(State.W.ExtFilename,SPG_ExtensOnly(State.W.BaseFilename)); SPG_SetExtens(State.W.BaseFilename,"");
		CFG_Close(CFG);
		State.W.N=0;
		scxSPLITFOpenWrite(State);
	}
	if(State.Address.R.Address==State.Address.W.Address)
	{
		State.R=State.W;
	}
	else	if(State.Address.R.Address)
	{
		SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG); scxCfgAddress(CFG,State.Address.R.Address,State.Address.R.CI,scxCFGTERMINALREAD);
		CFG_StringParam(CFG,"FileName",State.R.BaseFilename,0,1);
		strcpy(State.R.ExtFilename,SPG_ExtensOnly(State.R.BaseFilename)); SPG_SetExtens(State.R.BaseFilename,"");
		CFG_Close(CFG);
		State.R.N=0;
		scxSPLITFOpenRead(State);
	}



	scxProtocolInheritUserExtension(C,State.W.C,State.R.C);
	C->UserFctPtr[sci_EXT_OVERWRITE]=(SCX_USEREXTENSION)scxSPLITFOverwrite;//même si la connexion sous-jacente a une extension overwrite, on utilise celle de SPLITF
	C->UserFctData[sci_EXT_OVERWRITE]=0;

	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxSPLITFClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	if(State.R.C!=State.W.C)
	{
		scxSPLITFCloseWrite(State);
		scxSPLITFCloseRead(State);
		if(State.Address.W.Address) sciDestroyAddress(State.Address.W.Address,State.Address.W.CI);
		if(State.Address.R.Address) sciDestroyAddress(State.Address.R.Address,State.Address.R.CI);
	}
	else
	{
		scxSPLITFCloseWrite(State);
		if(State.Address.W.Address) sciDestroyAddress(State.Address.W.Address,State.Address.W.CI);
	}

	//desallocation STATE

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxSPLITFWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;
	CHECK(State.W.C==0,"scxSPLITFWrite",return 0);
	SPLITFW_CONNEXION_ADDRESS& AP=State.Address.W;
	SPLITF_CONNEXION_STATE& SP=State.W;

	int W=0;
	if(State.Address.SplitSizeMax > 0)
	{
		while(W<DataLen)
		{//si les données dépassent splitSizeMax on découpe le paquet à l'intérieur d'un write
			int w=scxWrite((BYTE*)Data+W, V_Min( (State.Address.SplitSizeMax-SP.Total), (DataLen-W) ), SP.C);
			SP.Total+=w;
			W+=w;
			if(SP.Total>=State.Address.SplitSizeMax) {scxSPLITFCloseWrite(State); scxSPLITFOpenWrite(State);}
		}
	}

	//si les données dépassent splitSizeMin on coupe entre deux écriture
	if( (State.Address.SplitSizeMin>0) && (SP.Total>=State.Address.SplitSizeMin) ) {scxSPLITFCloseWrite(State); scxSPLITFOpenWrite(State);}
	return W;
}

static SCX_EXTOVERWRITE(scxSPLITFOverwrite) //scxSPLITFOverwrite(void* Data, int DataLen, int Offset, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxOverwrite");
	SCX_STATE& State=*C->State;
	int w=0;
	SCX_CONNEXION* OWRC=State.W.C;

	if(State.W.N)
	{
		SCI_CONNEXIONINTERFACE* CI=0;
		SCX_ADDRESS* Address=sciAddressDuplicate(State.Address.W.CI,State.Address.W.Address,&CI);
		scxSPLITFSetFileName(Address,CI,scxCFGTERMINALWRITE,State.W.BaseFilename,0,State.W.ExtFilename);
		OWRC=scxOpen(CI,Address,0); //open preserve l'address W
	}

	w=scxOverwrite(Data,DataLen,Offset,OWRC);

	if(State.W.N)
	{
		if(OWRC) scxClose(OWRC);
	}
	return w;
}

// ###################################################

//#define PELT (*pkBufferGetLastPacket(PB)) //can not be null
//PELT = PB.P[P.NumPacket-1]

static int SPG_CONV scxSPLITFRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxSPLITFRead",return DataLen=0);
	SPLITFR_CONNEXION_ADDRESS& AP=State.Address.R;
	SPLITF_CONNEXION_STATE& SP=State.R;

	int R=0; int nullReadCount=0;
	do
	{
		int r=DataLen-R;
		scxRead((BYTE*)Data+R,r,SP.C);
		R+=r;
		if(r) nullReadCount=0; else nullReadCount++;
		if(R<DataLen) {scxSPLITFCloseRead(State);scxSPLITFOpenRead(State);}
	} while( (nullReadCount<2) && (R<DataLen) );

	return DataLen=R;
}

// ###################################################

static void SPG_CONV scxSPLITFCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);

	if(Flag&scxCFGTERMINALREAD)  { scxCfgAddress(CFG,Address->R.Address,Address->R.CI,Flag); return; }
	if(Flag&scxCFGTERMINALWRITE) { scxCfgAddress(CFG,Address->W.Address,Address->W.CI,Flag); return; }

	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values

		Address->W.Address=0;
		Address->W.CI=0;

		Address->R.Address=0;
		Address->R.CI=0;

		Address->SplitSizeMin=768*1024*1024;
		Address->SplitSizeMax=960*1024*1024;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);

	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	//Les noms W.Address et W.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);

	//Les noms R.Address et R.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	
	CFG_IntParam(CFG,"SplitSizeMin",	&Address->SplitSizeMin,0,1);
	CFG_IntParam(CFG,"SplitSizeMax",	&Address->SplitSizeMax,0,1);
	
	return;
}

// ###################################################

static int SPG_CONV scxSPLITFSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxSPLITFGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciSPLITFDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciSPLITFCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPROTOCOL);
	strcpy(CI->Name,sci_NAME);

	CI->Description="SplitFile";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;//spécifique

	CI->scxOpen=scxSPLITFOpen;
	CI->scxClose=scxSPLITFClose;
	CI->scxWrite=scxSPLITFWrite;
	CI->scxRead=scxSPLITFRead;
	CI->scxCfgAddress=scxSPLITFCfgAddress;
	CI->scxSetParameter=scxSPLITFSetParameter;
	CI->scxGetParameter=scxSPLITFGetParameter;
	CI->sciDestroyConnexionInterface=sciSPLITFDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
