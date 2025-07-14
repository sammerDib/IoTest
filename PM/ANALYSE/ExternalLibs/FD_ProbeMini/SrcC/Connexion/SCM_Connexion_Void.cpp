
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_VOID
#define sci_NAME sci_NAME_VOID



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
	//int DummyAddress;
} SCX_ADDRESS;



// ###################################################

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	//int DummyConnexionParam;
} SCX_STATE; //parametres d'une connexion en particulier





// ###################################################

static SCX_CONNEXION* SPG_CONV scxVOIDOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	//State.DummyConnexionParam=0;//spécifique

	C->Etat=scxOK;
	return C;
}




// ###################################################

static int SPG_CONV scxVOIDClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	C->Etat=scxINVALID;

	scxFree(C->State);scxFree(C);
	return scxOK;
}






// ###################################################

static int SPG_CONV scxVOIDWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");//variables crées CI,sciParameters,scxParameters
	SCX_STATE& State=*C->State;

	return 0;
}

// ###################################################

static int SPG_CONV scxVOIDRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");//variables crées CI,sciParameters,scxParameters
	SCX_STATE& State=*C->State;

	return DataLen=0;
}

// ###################################################

static void SPG_CONV scxVOIDCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		//Address->DummyAddress=1978;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	//CFG_IntParam(CFG,"DummyAddress",&Address->DummyAddress,0,CFG.FileName[0]?1:0);
	return;
}







// ###################################################

static int SPG_CONV scxVOIDSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV scxVOIDGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV sciVOIDDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciVOIDCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=sciBOTH;
	strcpy(CI->Name,sci_NAME);

	CI->Description="Null connexion, write: ignore data, read: return null";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;//spécifique

	CI->scxOpen=scxVOIDOpen;
	CI->scxClose=scxVOIDClose;
	CI->scxWrite=scxVOIDWrite;
	CI->scxRead=scxVOIDRead;
	CI->scxCfgAddress=scxVOIDCfgAddress;
	CI->scxSetParameter=scxVOIDSetParameter;
	CI->scxGetParameter=scxVOIDGetParameter;
	CI->sciDestroyConnexionInterface=sciVOIDDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;

	return CI;
}

// ###################################################

#endif
