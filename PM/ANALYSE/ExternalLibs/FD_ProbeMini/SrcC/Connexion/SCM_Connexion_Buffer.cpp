
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USERingBuffer

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_BUFFER
#define sci_NAME sci_NAME_BUFFER

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

	int BufferSize;
} SCX_ADDRESS;








// ###################################################

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;//obligatoire

	PG_RINGBUFFER R;
} SCX_STATE; //parametres d'une connexion en particulier



// ###################################################

static SCX_CONNEXION* SPG_CONV scxBUFFOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	RING_Create(State.R,State.Address.BufferSize);

	C->Etat=scxOK;
	return C;
}



// ###################################################

static int SPG_CONV scxBUFFClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	RING_Close(State.R);

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxBUFFWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	return RING_WriteBytes(State.R,(BYTE*)Data,DataLen);
}

// ###################################################

static int SPG_CONV scxBUFFRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	DataLen=V_Min(RING_CanRead(State.R),DataLen);
	return DataLen=RING_ReadBytes(State.R,(BYTE*)Data,DataLen);
}

// ###################################################

static void SPG_CONV scxBUFFCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->BufferSize=1024*1024;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	CFG_IntParam(CFG,"BufferSize",&Address->BufferSize,0,1,CP_INT|CP_HASMIN,1);
	return;
}





// ###################################################

static int SPG_CONV scxBUFFSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxBUFFGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciBUFFDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciBUFFCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Ring buffer";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;

	CI->scxOpen=scxBUFFOpen;
	CI->scxClose=scxBUFFClose;
	CI->scxWrite=scxBUFFWrite;
	CI->scxRead=scxBUFFRead;
	CI->scxCfgAddress=scxBUFFCfgAddress;
	CI->scxSetParameter=scxBUFFSetParameter;
	CI->scxGetParameter=scxBUFFGetParameter;
	CI->sciDestroyConnexionInterface=sciBUFFDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif

