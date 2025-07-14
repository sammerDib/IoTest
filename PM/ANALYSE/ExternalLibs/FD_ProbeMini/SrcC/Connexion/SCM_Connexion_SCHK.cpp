
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USERingBuffer

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_SCHK
#define sci_NAME sci_NAME_SCHK

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface


// ###################################################

#define CONFIG_TST

#ifdef CONFIG_TST
typedef BYTE SCHKELT;
#else
typedef DWORD SCHKELT;
#endif
#define szSCHELT sizeof(SCHKELT)

typedef struct
{
	int NumDim;
	int LineLen;
	int InputLen;
	int OutputLen;
	int OutputDimLen;
	SCHKELT* Input;
	SCHKELT* Output;
} EncodeSCHKSTATE;

typedef struct
{
	PG_RINGBUFFER SI;
	int InputLen;
	int OutputLen;
#ifdef CONFIG_TST
#define PYRAMIDE 3
#else
#define PYRAMIDE 4
#endif
	EncodeSCHKSTATE S[PYRAMIDE+1];
	PG_RINGBUFFER SO;
} EncodePCHKSTATE;


typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
} SCHK_CONNEXION_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	SCHK_CONNEXION_ADDRESS R;
	SCHK_CONNEXION_ADDRESS W;
	//char Name[SCI_CONNEXION_NAME];
	//int BufferSize;
} SCX_ADDRESS;

// ###################################################

typedef struct
{
	SCX_CONNEXION* C;//connexion physique en lecture
} SCHK_CONNEXION_STATE;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;//obligatoire

	SCHK_CONNEXION_STATE R;
	SCHK_CONNEXION_STATE W;

	EncodePCHKSTATE eCHK;// e comme encode

} SCX_STATE; //parametres d'une connexion en particulier

#include "SCM_ExtensWriteThrough.h"

#include "SCM_ExtensProtocol.h"

// ###################################################

static SCX_CONNEXION* SPG_CONV scxSCHKOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	scxOpenProtocol(State,scxOpenFlag);
	scxProtocolInheritUserExtension(C,State.W.C,State.R.C);

#ifdef CONFIG_TST  //1+16+64=
#define MAXDIM 2
	State.eCHK.S[0].NumDim=2;
	State.eCHK.S[0].LineLen=4;
	State.eCHK.S[0].InputLen=16;//4^2
	State.eCHK.S[0].OutputDimLen=4;//4^1
	State.eCHK.S[0].OutputLen=8;//2*4^1

	State.eCHK.S[1].NumDim=1;
	State.eCHK.S[1].LineLen=16;
	State.eCHK.S[1].InputLen=16;//16^1
	State.eCHK.S[1].OutputDimLen=1;//16^0
	State.eCHK.S[1].OutputLen=1;//1*16^0
#else
#define MAXDIM 3  // 1+130+4225+50653+2097152=
	State.eCHK.S[0].NumDim=3;
	State.eCHK.S[0].LineLen=128;
	State.eCHK.S[0].InputLen=2097152;//128^3
	State.eCHK.S[0].OutputDimLen=16384;//128^2
	State.eCHK.S[0].OutputLen=49152;//3*128^2

	State.eCHK.S[1].NumDim=3;
	State.eCHK.S[1].LineLen=37;
	State.eCHK.S[1].InputLen=50653;//37^3
	State.eCHK.S[1].OutputDimLen=1369;//37^2
	State.eCHK.S[1].OutputLen=4107;//3*37^2

	State.eCHK.S[2].NumDim=2;
	State.eCHK.S[2].LineLen=65;
	State.eCHK.S[2].InputLen=4225;//65^2
	State.eCHK.S[2].OutputDimLen=65;//65^1
	State.eCHK.S[2].OutputLen=130;//2*65^1

	State.eCHK.S[3].NumDim=1;
	State.eCHK.S[3].LineLen=130;
	State.eCHK.S[3].InputLen=130;//130^1
	State.eCHK.S[3].OutputDimLen=1;//130^0
	State.eCHK.S[3].OutputLen=1;//1*130^0
#endif

	State.eCHK.InputLen=State.eCHK.S[0].InputLen;
	State.eCHK.OutputLen=V_Max(State.eCHK.S[0].OutputLen,State.eCHK.S[1].InputLen)
						+V_Max(State.eCHK.S[1].OutputLen,State.eCHK.S[2].InputLen)
#if PYRAMIDE==3
						+State.eCHK.S[2].OutputLen;
#elif PYRAMIDE>=3
						+V_Max(State.eCHK.S[2].OutputLen,State.eCHK.S[3].InputLen)
#endif
#if PYRAMIDE==4
						+State.eCHK.S[3].OutputLen;
#endif

	State.eCHK.S[0].Input=SPG_TypeAlloc((State.eCHK.InputLen+State.eCHK.OutputLen),SCHKELT,"scxSCHKOpen:eCHK.Input");
	State.eCHK.S[0].Output=State.eCHK.S[0].Input+State.eCHK.S[0].InputLen;

	State.eCHK.S[1].Input=State.eCHK.S[0].Output;
	State.eCHK.S[1].Output=State.eCHK.S[0].Output+State.eCHK.S[0].OutputLen;

	State.eCHK.S[2].Input=State.eCHK.S[1].Output;
	State.eCHK.S[2].Output=State.eCHK.S[1].Output+State.eCHK.S[1].OutputLen;
#if PYRAMIDE>=3
	State.eCHK.S[3].Input=State.eCHK.S[2].Output;
	State.eCHK.S[3].Output=State.eCHK.S[2].Output+State.eCHK.S[2].OutputLen;
#endif
	RING_Create(State.eCHK.SI,V_Max(CI->maxPacketSize,2*State.eCHK.InputLen*szSCHELT));
	RING_Create(State.eCHK.SO,V_Max(CI->maxPacketSize,2*(State.eCHK.InputLen+State.eCHK.OutputLen)*szSCHELT));

	C->Etat=scxOK;
	return C;
}



// ###################################################

static int SPG_CONV scxSCHKClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

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


	SPG_MemFree(State.eCHK.S[0].Input);
	RING_Close(State.eCHK.SI);
	RING_Close(State.eCHK.SO);
	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static void SPG_CONV SCHK_ENCODE(EncodePCHKSTATE& eCHK, bool ZeroPad=false)
{
	int n=RING_CanRead(eCHK.SI);
	{
		EncodeSCHKSTATE& S=eCHK.S[0];
		if(eCHK.S[0].InputLen*szSCHELT<n)
		{//la longueur est suffisante pour le premier niveau de checksum
			n=eCHK.S[0].InputLen*szSCHELT;
			RING_ReadBytes(eCHK.SI,(BYTE*)S.Input,n);
		}
		else if(ZeroPad)
		{//lit les données qui seront completees par zero
			RING_ReadBytes(eCHK.SI,(BYTE*)S.Input,n);
		}
		else return;
	}
	//initialise la partie contenant la checksum
	memset(eCHK.S[0].Input+n,0,(eCHK.InputLen+eCHK.OutputLen)*szSCHELT-n);

	for(int y=0;y<PYRAMIDE;y++)
	{//y est le niveau de la pyramide
		EncodeSCHKSTATE& S=eCHK.S[y];
		//les données sources de la pyramide sont S.Input longueur S.InputLen output S.Output

		int Idx[MAXDIM];memset(Idx,0,MAXDIM*sizeof(int));
		int Prj[MAXDIM];memset(Idx,0,MAXDIM*sizeof(int));

		for(int x=0;x<S.InputLen;x++)
		{
			{for(int p=0;p<S.NumDim;p++)
			{//axe de projection
				S.Output[ p*S.OutputDimLen + Prj[p] ] += S.Input[x];
			}}

			{for(int p=0;p<S.NumDim;p++)
			{//axe de projection
				Idx[p]++;
				if(Idx[p]!=S.LineLen) break;
				Idx[p]=0;
			}}

			{for(int p=0;p<S.NumDim;p++)
			{//axe de projection
				Prj[p]=0;
				for(int pp=0;pp<S.NumDim;pp++)
				{
					if(pp==p) continue;
					Prj[p]*=S.LineLen;
					Prj[p]+=Idx[pp];
				}
			}}
		}
	}

	RING_WriteBytes(eCHK.SO,(BYTE*)eCHK.S[0].Input,((eCHK.InputLen+eCHK.OutputLen)*szSCHELT));
	return;
}

static int SPG_CONV scxSCHKWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	int r=RING_WriteBytes(State.eCHK.SI,(BYTE*)Data,DataLen);

	SCHK_ENCODE(State.eCHK);

	return r;
}

// ###################################################

static int SPG_CONV scxSCHKRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	DataLen=V_Min(RING_CanRead(State.eCHK.SO),DataLen);
	return DataLen=RING_ReadBytes(State.eCHK.SO,(BYTE*)Data,DataLen);
}

// ###################################################

static void SPG_CONV scxSCHKCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		//Address->BufferSize=1024*1024;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	//Les noms R.Address et R.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);

	//Les noms W.Address et W.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);

	//CFG_IntParam(CFG,"BufferSize",&Address->BufferSize,0,1,CP_INT|CP_HASMIN,1);
	return;
}





// ###################################################

static int SPG_CONV scxSCHKSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxSCHKGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciSCHKDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciSCHKCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="SPG Checksum";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=2097152;

	CI->scxOpen=scxSCHKOpen;
	CI->scxClose=scxSCHKClose;
	CI->scxWrite=scxSCHKWrite;
	CI->scxRead=scxSCHKRead;
	CI->scxCfgAddress=scxSCHKCfgAddress;
	CI->scxSetParameter=scxSCHKSetParameter;
	CI->scxGetParameter=scxSCHKGetParameter;
	CI->sciDestroyConnexionInterface=sciSCHKDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif

