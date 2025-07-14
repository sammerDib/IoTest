
//#include "..\SrcC\SPG.h"
//#include "..\SPG.h"

#include "SCM_Connexion.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

//partie spécifique

#define sci_UID 2
#define sci_NAME "VOID"

typedef struct
{
	//parametres de l'interface
	int DummyInterfaceParam;
} SCI_Parameters; //parametres de l'interface

typedef struct
{
	int DummyConnexionParam;
} SCX_Parameters; //parametres d'une connexion en particulier

typedef struct
{
	int DummyAddress;
} SCX_Address;

//fin partie spécifique

//implémentations fonctions internes

static SPG_CONV SCX_CONNEXION* scxVOIDOpen(void* vCI, void* vAddress)
{
	SCI_CONNEXIONINTERFACE* CI=(SCI_CONNEXIONINTERFACE*)vCI;
	SCX_Address& Address=*(SCX_Address*)vAddress;
	sciCHECK(CI,"scxInit");

	SCX_CONNEXION* C=scxAlloc(SCX_CONNEXION);
	C->CI=CI;
	SCX_Parameters& scxParameters=*scxAlloc(SCX_Parameters);
	C->scxParameters=(void*)&scxParameters;

	scxParameters.DummyConnexionParam=0;//spécifique

	C->Etat=scxOK;
	return C;
}

static SPG_CONV int scxVOIDClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");

	scxFree(C->scxParameters);
	scxFree(C);
	return scxOK;
}

static SPG_CONV int scxVOIDWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");//variables crées CI,sciParameters,scxParameters
	return 0;
}

static SPG_CONV int scxVOIDRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");//variables crées CI,sciParameters,scxParameters
	return 0;
}

static SPG_CONV int scxVOIDSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

static SPG_CONV int scxVOIDGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

static SPG_CONV int sciVOIDDestroyConnexionInterface(void* vCI)
{
	SCI_CONNEXIONINTERFACE* CI=(SCI_CONNEXIONINTERFACE*)vCI;
	CHECK(CI==0,"scxDeleteConnexionInterface",return 0);
	CHECK(CI->TypeUID!=sci_UID,"scxDeleteConnexionInterface",return 0);
	scxFree(CI->sciParameters);
	scxFree(CI);
	return scxOK;
}

//fonctions exportées

SPG_CONV SCI_CONNEXIONINTERFACE* sciVOIDCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxAlloc(SCI_CONNEXIONINTERFACE);
	memset(CI,0,sizeof(SCI_CONNEXIONINTERFACE));

	CI->TypeUID=sci_UID;
	CI->Type=sci_BOTH;
	strcpy(CI->Name,sci_NAME);
	CI->sizeofAddress=sizeof(SCX_Address);

	SCI_Parameters& sciParameters=*scxAlloc(SCI_Parameters);
	CI->sciParameters=(void*)&sciParameters;

	CI->scxOpen=scxVOIDOpen;
	CI->scxClose=scxVOIDClose;
	CI->scxWrite=scxVOIDWrite;
	CI->scxRead=scxVOIDRead;
	CI->scxSetParameter=scxVOIDSetParameter;
	CI->scxGetParameter=scxVOIDGetParameter;

	CI->sciDestroyConnexionInterface=sciVOIDDestroyConnexionInterface;

	CI->maxMsgSize=256;//spécifique

	sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;

	return CI;
}

