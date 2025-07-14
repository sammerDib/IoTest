
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID 7
#define sci_NAME "TRANSCODE"

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
	int TRANSCODEPacketSize;
	int BufferSize;
} TRANSCODE_CONNEXION_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	TRANSCODE_CONNEXION_ADDRESS R;
	TRANSCODE_CONNEXION_ADDRESS W;
} SCX_ADDRESS;








// ###################################################

typedef struct
{
	SCX_CONNEXION* C;//connexion physique en lecture
	PG_RINGBUFFER PB;
	BYTE* TRANSCODEPacketSTBuffer;
} TRANSCODE_CONNEXION_STATE;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;//obligatoire
	TRANSCODE_CONNEXION_STATE R;
	TRANSCODE_CONNEXION_STATE W;
} SCX_STATE; //parametres d'une connexion en particulier


// ###################################################

#include "SCM_ExtensProtocol.h"

// ###################################################

static SCX_CONNEXION* SPG_CONV scxTRANSCODEOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

//	CHECK(State.Address.R.BufferSize<2*State.Address.R.TRANSCODEPacketSize,"scxTRANSCODEOpen",State.Address.R.BufferSize=2*State.Address.R.TRANSCODEPacketSize);
//	CHECK(State.Address.W.BufferSize<2*State.Address.W.TRANSCODEPacketSize,"scxTRANSCODEOpen",State.Address.W.BufferSize=2*State.Address.W.TRANSCODEPacketSize);

	State.Address.R.TRANSCODEPacketSize=4;
	State.Address.W.TRANSCODEPacketSize=4;

	RING_Create(State.R.PB,2*State.Address.R.TRANSCODEPacketSize);
	State.R.TRANSCODEPacketSTBuffer=SPG_TypeAlloc(State.Address.R.TRANSCODEPacketSize,BYTE,"scxTRANSCODEOpen:R.TRANSCODEPacketSTBuffer");

	RING_Create(State.W.PB,2*State.Address.W.TRANSCODEPacketSize);
	State.W.TRANSCODEPacketSTBuffer=SPG_TypeAlloc(State.Address.W.TRANSCODEPacketSize,BYTE,"scxTRANSCODEOpen:W.TRANSCODEPacketSTBuffer");

	scxOpenProtocol(State, scxOpenFlag);

	C->Etat=scxOK;
	return C;
}



// ###################################################

static int SPG_CONV scxTRANSCODEClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;

	{//envoie les données en attente d'ecriture
		TRANSCODE_CONNEXION_ADDRESS& AM=State.Address.W;
		TRANSCODE_CONNEXION_STATE& SM=State.W;
		int CanRead=RING_CanRead(SM.PB);
		CHECK(CanRead>AM.TRANSCODEPacketSize,"scxTRANSCODEClose: Données résiduelles supérieures à un packet",CanRead=AM.TRANSCODEPacketSize);
		RING_ReadBytes(SM.PB,SM.TRANSCODEPacketSTBuffer,CanRead);
		scxWrite(SM.TRANSCODEPacketSTBuffer,CanRead,SM.C);
	}
	//pour ce qui est des données en attente de lecture on ne fait rien de spécial

	RING_Close(State.R.PB);
	RING_Close(State.W.PB);

	scxCloseProtocol(State);

/*
	if(State.W.C==State.R.C)
	{
		if(State.R.C) scxClose(State.R.C);
		State.W.C=State.R.C=0;
	}
	else
	{
		if(State.R.C) scxClose(State.R.C);
		if(State.W.C) scxClose(State.W.C);
		State.W.C=State.R.C=0;
	}


	if(State.Address.R.Address==State.Address.W.Address)
	{
		if(State.Address.R.Address) scxDestroyAddress(State.Address.R.Address);
	}
	else
	{
		if(State.Address.R.Address) scxDestroyAddress(State.Address.R.Address);
		if(State.Address.W.Address) scxDestroyAddress(State.Address.W.Address);
	}
*/

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxTRANSCODEWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;
	TRANSCODE_CONNEXION_ADDRESS& AM=State.Address.W;
	TRANSCODE_CONNEXION_STATE& SM=State.W;

	int Written=0;
	while(DataLen>0)
	{
		{//Ecrit les données dans un ring buffer
			int CanWrite=RING_CanWrite(SM.PB);
			CHECK(CanWrite==0,"scxTRANSCODEWrite:Ring buffer full",return Written);
			if(CanWrite>=DataLen) 
			{
				RING_WriteBytes(SM.PB,(BYTE*)Data,DataLen);
				Written+=DataLen;
				DataLen=0;
			}
			else // CanWrite>0 implicite
			{
				RING_WriteBytes(SM.PB,(BYTE*)Data,CanWrite);
				*(int*)&Data+=CanWrite;
				Written+=CanWrite;
				DataLen-=CanWrite;
			}
		}
		int CanRead=RING_CanRead(SM.PB);
		if(CanRead>=AM.TRANSCODEPacketSize)
		{//Transmet les données contenues dans le ring buffer
			RING_ReadBytes(SM.PB,SM.TRANSCODEPacketSTBuffer,AM.TRANSCODEPacketSize);
			scxWrite(SM.TRANSCODEPacketSTBuffer,AM.TRANSCODEPacketSize,SM.C);
		}
	}
	return 0;
}

// ###################################################

static int SPG_CONV scxTRANSCODERead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;
	TRANSCODE_CONNEXION_ADDRESS& AM=State.Address.R;
	TRANSCODE_CONNEXION_STATE& SM=State.R;

	int CanWrite=RING_CanWrite(SM.PB);
	if(CanWrite>=AM.TRANSCODEPacketSize)
	{//Recoit les données dans un ring buffer par des requetes de taille fixe
		int ReadSize=AM.TRANSCODEPacketSize;
		if(scxRead(SM.TRANSCODEPacketSTBuffer,ReadSize,SM.C)>0)
		{
			RING_WriteBytes(SM.PB,SM.TRANSCODEPacketSTBuffer,ReadSize);
		}
	}

	int CanRead=RING_CanRead(SM.PB);
	if(CanRead>=DataLen)
	{
		RING_ReadBytes(SM.PB,(BYTE*)Data,DataLen);
	}
	else
	{//on ne renvoie que des groupes entiers
		DataLen=0;
		//on renvoie des blocs partiels
		RING_ReadBytes(SM.PB,(BYTE*)Data,CanRead);
		DataLen=CanRead;
	}

	return DataLen;
}

// ###################################################

static void SPG_CONV scxTRANSCODECfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values

		Address->R.Address=0;
		Address->R.CI=0;
//		Address->R.BufferSize=65536;
//		Address->R.TRANSCODEPacketSize=32768;

		Address->W.Address=0;
		Address->W.CI=0;
//		Address->W.BufferSize=65536;
//		Address->W.TRANSCODEPacketSize=32768;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);

	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
//	CFG_IntParam(CFG,"R.BufferSize",	&Address->R.BufferSize,0,1,			CP_INT|CP_HASMIN,1);
//	CFG_IntParam(CFG,"R.TRANSCODEPacketSize",&Address->R.TRANSCODEPacketSize,0,1,	CP_INT|CP_HASMIN,1);

	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
//	CFG_IntParam(CFG,"W.BufferSize",	&Address->W.BufferSize,0,1,			CP_INT|CP_HASMIN,1);
//	CFG_IntParam(CFG,"W.TRANSCODEPacketSize",&Address->W.TRANSCODEPacketSize,0,1,	CP_INT|CP_HASMIN,1);

/*
	scxCfgAddress(CFG, Address->RAddress, Address->RCI, Flag);
	scxCfgAddress(CFG, Address->WAddress, Address->WCI, Flag);
*/
	return;
}





// ###################################################

static int SPG_CONV scxTRANSCODESetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxTRANSCODEGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciTRANSCODEDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciTRANSCODECreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE|sciPROTOCOL);//PACKET Merge, car un message incomplet ne peut être transcodé dans le cas ou le nombre d'octets est modifié par le transcodage
	strcpy(CI->Name,sci_NAME);

	CI->Description="TRANSCODE to fixed packet size";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;

	CI->scxOpen=scxTRANSCODEOpen;
	CI->scxClose=scxTRANSCODEClose;
	CI->scxWrite=scxTRANSCODEWrite;
	CI->scxRead=scxTRANSCODERead;
	CI->scxCfgAddress=scxTRANSCODECfgAddress;
	CI->scxSetParameter=scxTRANSCODESetParameter;
	CI->scxGetParameter=scxTRANSCODEGetParameter;
	CI->sciDestroyConnexionInterface=sciTRANSCODEDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif

