
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USERingBuffer

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_MERGE
#define sci_NAME sci_NAME_MERGE

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface


// ###################################################

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
	int MergePacketSize;
//	int BufferSize;
} MERGE_CONNEXION_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	MERGE_CONNEXION_ADDRESS R;
	MERGE_CONNEXION_ADDRESS W;
} SCX_ADDRESS;








// ###################################################

typedef struct
{
	SCX_CONNEXION* C;//connexion physique en lecture
	PG_RINGBUFFER PB;
	BYTE* MergePacketSTBuffer;
} MERGE_CONNEXION_STATE;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;//obligatoire
	MERGE_CONNEXION_STATE R;
	MERGE_CONNEXION_STATE W;
} SCX_STATE; //parametres d'une connexion en particulier

#include "SCM_ExtensWriteThrough.h"

#include "SCM_ExtensProtocol.h"

// ###################################################

static SCX_CONNEXION* SPG_CONV scxMERGEOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	//State.Address.R.BufferSize=2*State.Address.R.MergePacketSize;
	//State.Address.W.BufferSize=2*State.Address.W.MergePacketSize;
	//CHECK(State.Address.R.BufferSize<2*State.Address.R.MergePacketSize,"scxMERGEOpen",State.Address.R.BufferSize=2*State.Address.R.MergePacketSize);
	//CHECK(State.Address.W.BufferSize<2*State.Address.W.MergePacketSize,"scxMERGEOpen",State.Address.W.BufferSize=2*State.Address.W.MergePacketSize);

	if(State.Address.R.MergePacketSize)
	{
		RING_Create(State.R.PB,2*State.Address.R.MergePacketSize);
		State.R.MergePacketSTBuffer=SPG_TypeAlloc(State.Address.R.MergePacketSize,BYTE,"scxMERGEOpen:R.MergePacketSTBuffer");
	}

	if(State.Address.W.MergePacketSize)
	{
		RING_Create(State.W.PB,2*State.Address.W.MergePacketSize);
		State.W.MergePacketSTBuffer=SPG_TypeAlloc(State.Address.W.MergePacketSize,BYTE,"scxMERGEOpen:W.MergePacketSTBuffer");
	}

	scxOpenProtocol(State,scxOpenFlag);
	scxProtocolInheritUserExtension(C,State.W.C,State.R.C);

	C->Etat=scxOK;
	return C;
}



// ###################################################

static int SPG_CONV scxMERGEClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	{//envoie les données en attente d'ecriture
		MERGE_CONNEXION_ADDRESS& AM=State.Address.W;
		if(AM.MergePacketSize) 
		{
			MERGE_CONNEXION_STATE& SM=State.W;
			int CanRead=RING_CanRead(SM.PB);
			CHECK(CanRead>AM.MergePacketSize,"scxMERGEClose: Données résiduelles supérieures à un packet",CanRead=AM.MergePacketSize);
			RING_ReadBytes(SM.PB,SM.MergePacketSTBuffer,CanRead);
			if(CanRead>0) scxWrite(SM.MergePacketSTBuffer,CanRead,SM.C);
		}
	}
	//pour ce qui est des données en attente de lecture on ne fait rien de spécial

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

	scxCloseProtocol(State);

	if(State.Address.R.MergePacketSize) 
	{
		RING_Close(State.R.PB);
		SPG_MemFree(State.R.MergePacketSTBuffer);
	}
	if(State.Address.W.MergePacketSize) 
	{
		RING_Close(State.W.PB);
		SPG_MemFree(State.W.MergePacketSTBuffer);
	}

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxMERGEWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;
	MERGE_CONNEXION_ADDRESS& AM=State.Address.W;
	MERGE_CONNEXION_STATE& SM=State.W;
	if(AM.MergePacketSize==0) return scxWrite(Data,DataLen,SM.C); //ecriture directe si le param PacketSize est zero

	int Written=0;
	while(DataLen>0)//pour toutes les données à écrire
	{
		{//Ecrit les données dans un ring buffer
			int CanWrite=RING_CanWrite(SM.PB);
			CHECK(CanWrite==0,"scxMERGEWrite:Ring buffer full",return Written);//le buffer est plein : toutes les données ne peuvent être écrites (ne se produit pas car on laisse au plus PacketSize-1 data dans le buffer)
			if(CanWrite>=DataLen) //ecrit toutes les données
			{
				RING_WriteBytes(SM.PB,(BYTE*)Data,DataLen);
				Written+=DataLen;
				DataLen=0;
			}
			else // le buffer circulaire ne peut pas contenir toutes les données
			{
				RING_WriteBytes(SM.PB,(BYTE*)Data,CanWrite);//n'ecrit que ce qui peut rentrer dans le buffer
				*(int*)&Data+=CanWrite;
				Written+=CanWrite;
				DataLen-=CanWrite;
			}
		}
		int CanRead=RING_CanRead(SM.PB);
		if(CanRead>=AM.MergePacketSize)//s'il y a un paquet entier dans le buffer on le sort et transmet
		{//Transmet les données contenues dans le ring buffer
			RING_ReadBytes(SM.PB,SM.MergePacketSTBuffer,AM.MergePacketSize);
			scxWrite(SM.MergePacketSTBuffer,AM.MergePacketSize,SM.C);
			//scxWrite(&"*",1,SM.C);
		}
	}
	return Written;
}

// ###################################################

static int SPG_CONV scxMERGERead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;
	MERGE_CONNEXION_ADDRESS& AM=State.Address.R;
	MERGE_CONNEXION_STATE& SM=State.R;
	if(AM.MergePacketSize==0) return scxRead(Data,DataLen,SM.C); //ecriture directe si le param PacketSize est zero

	int CanWrite=RING_CanWrite(SM.PB);
	if(CanWrite>=AM.MergePacketSize)
	{//Recoit les données dans un ring buffer par des requetes de taille fixe
		int ReadSize=AM.MergePacketSize;
		if(scxRead(SM.MergePacketSTBuffer,ReadSize,SM.C)>0)
		{
			RING_WriteBytes(SM.PB,SM.MergePacketSTBuffer,ReadSize);
		}
	}

	int CanRead=RING_CanRead(SM.PB);//la fonction read retourne la concaténation de ce qui a été lu
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

static void SPG_CONV scxMERGECfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);

	if(Flag&scxCFGTERMINALREAD)  { scxCfgAddress(CFG,Address->R.Address,Address->R.CI,Flag); return; }
	if(Flag&scxCFGTERMINALWRITE) { scxCfgAddress(CFG,Address->W.Address,Address->W.CI,Flag); return; }

	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values

		Address->R.Address=0;
		Address->R.CI=0;
//		Address->R.BufferSize=65536;
		Address->R.MergePacketSize=32768;

		Address->W.Address=0;
		Address->W.CI=0;
//		Address->W.BufferSize=65536;
		Address->W.MergePacketSize=32768;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);

	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
//	CFG_IntParam(CFG,"R.BufferSize",	&Address->R.BufferSize,0,1,			CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"R.MergePacketSize",&Address->R.MergePacketSize,0,1,	CP_INT|CP_HASMIN,1);

	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
//	CFG_IntParam(CFG,"W.BufferSize",	&Address->W.BufferSize,0,1,			CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"W.MergePacketSize",&Address->W.MergePacketSize,0,1,	CP_INT|CP_HASMIN,1);

/*
	scxCfgAddress(CFG, Address->RAddress, Address->RCI, Flag);
	scxCfgAddress(CFG, Address->WAddress, Address->WCI, Flag);
*/
	return;
}





// ###################################################

static int SPG_CONV scxMERGESetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxMERGEGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciMERGEDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciMERGECreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE|sciPROTOCOL);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Merge to fixed packet size";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;

	CI->scxOpen=scxMERGEOpen;
	CI->scxClose=scxMERGEClose;
	CI->scxWrite=scxMERGEWrite;
	CI->scxRead=scxMERGERead;
	CI->scxCfgAddress=scxMERGECfgAddress;
	CI->scxSetParameter=scxMERGESetParameter;
	CI->scxGetParameter=scxMERGEGetParameter;
	CI->sciDestroyConnexionInterface=sciMERGEDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif

