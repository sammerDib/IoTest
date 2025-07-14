
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h" //SleepEx

#include "SCM_ConnexionDbg.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_AHEX
#define sci_NAME sci_NAME_AHEX

//----------------------------------------

#define MAXAHEXSIZE 8192

//----------------------------------------

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface


//----------------------------------------


#define SCX_LENFILENAME 1024

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
	int Delay_ms;
	int Separator;//0:space 1:tab 2:cr+lf
} AHEXW_ADDRESS;

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
	int Delay_ms;
} AHEXR_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	AHEXR_ADDRESS R;
	AHEXW_ADDRESS W;
} SCX_ADDRESS;

//----------------------------------------

typedef struct
{
	SCX_CONNEXION* C;
	SPGSTR b;
	SPGSTR h;
} AHEXW_STATE;

typedef struct
{
	SCX_CONNEXION* C;
	SPGSTR b;
	SPGSTR h;
} AHEXR_STATE;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	AHEXW_STATE W;
	AHEXR_STATE R;
	//...
} SCX_STATE; //parametres d'une connexion en particulier

//----------------------------------------

#include "SCM_ExtensWriteThrough.h"

#include "SCM_ExtensProtocol.h"

//----------------------------------------

static SCX_CONNEXION* SPG_CONV scxAHEXOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	CHECK(scxOpenProtocol(State,scxOpenFlag)==0,"scxAHEXOpen",scxCloseProtocol(State);scxFree(C->State);scxFree(C);return 0);
	scxProtocolInheritUserExtension(C,State.W.C,State.R.C);

	StrInit(State.W.b,MAXAHEXSIZE);
	StrInit(State.W.h,3*MAXAHEXSIZE);
	StrInit(State.R.b,MAXAHEXSIZE);
	StrInit(State.R.h,3*MAXAHEXSIZE);

	C->Etat=scxOK;
	return C;
}


//----------------------------------------

static int SPG_CONV scxAHEXClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;


	scxCloseProtocol(State);

	//desallocation STATE
	StrClose(State.W.b);
	StrClose(State.W.h);
	StrClose(State.R.b);
	StrClose(State.R.h);

	scxFree(C->State);scxFree(C);
	return scxOK;
}

//----------------------------------------

static int SPG_CONV scxAHEXWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	AHEXW_ADDRESS& WA=C->State->Address.W;
	AHEXW_STATE& W=C->State->W;

	StrCpy(W.b,Data,DataLen);
	StrCpyByteToAsciiHex(W.h,W.b);//dest,source
	if(WA.Separator==0)
	{
		StrCat(W.h," ");
	}
	else if(WA.Separator==1)
	{
		StrCat(W.h,"\t");
	}
	else if(WA.Separator==2)
	{
		StrCat(W.h,"\r\n");
	}
	scxWrite(W.h.D,W.h.Len,W.C);

	if(WA.Delay_ms) SPG_Sleep(WA.Delay_ms);

	return DataLen;
}

//----------------------------------------

static int SPG_CONV scxAHEXRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	AHEXR_ADDRESS& RA=C->State->Address.R;
	AHEXR_STATE& R=C->State->R;




	//memset(W.h,0,W.MaxLen);
	//fgets((char*)State.S.D,State.S.MaxLen,State.Input);
	//StrZtoStr(W.h);

	StrCat(R.h,R.C);

	int i1=StrFind(R.h,'\r');
	int i2=StrFind(R.h,'\n');
	int e=-1;
	int el=0;

	if((i1>=0)&&(i2==-1)) {e=i1;el=1;}
	else if((i1==-1)&&(i2>=0)) {e=i2;el=1;}
	else if((i1>=0)&&(i2>=0)) { e=V_Min(i1,i2);if(i2==(i1+1)) {el=2;} else {el=1;} }
	else if(R.h.Len==R.h.MaxLen) {e=R.h.Len;}
	else return DataLen=0;

	V_SWAP(int,e,R.h.Len);
	StrCatAsciiHexToByte(R.b,R.h);//dest,source
	V_SWAP(int,e,R.h.Len);
	StrSupprLeft(R.h,e+el);

	if(DataLen>R.b.Len) DataLen=R.b.Len;
	if(DataLen) memcpy(Data,R.b.D,DataLen);
	StrSupprLeft(R.b,DataLen);

	if(RA.Delay_ms) SPG_Sleep(RA.Delay_ms);

	return DataLen;
}

//----------------------------------------

static void SPG_CONV scxAHEXCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->R.Address=0;
		Address->R.CI=0;
		Address->R.Delay_ms=0;

		Address->W.Address=0;
		Address->W.CI=0;
		Address->W.Delay_ms=0;
		Address->W.Separator=2;//CR+LF
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,CFG.FileName[0]?1:0);

	//Les noms R.Address et R.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.Delay_ms",	(int*)&Address->W.Delay_ms,0,1);
	CFG_IntParam(CFG,"W.Separator",	(int*)&Address->W.Separator,"0:none 1:space 2:cr+lf",1,CP_INT|CP_HASMIN|CP_HASMAX,0,2);

	//Les noms R.Address et R.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.Delay_ms",	(int*)&Address->R.Delay_ms,0,1);

	return;
}









//----------------------------------------

static int SPG_CONV scxAHEXSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV scxAHEXGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV sciAHEXDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


//----------------------------------------

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciAHEXCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE|sciPROTOCOL);
	strcpy(CI->Name,sci_NAME);

	CI->Description="AHEX port";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=MAXAHEXSIZE;//spécifique

	CI->scxOpen=scxAHEXOpen;
	CI->scxClose=scxAHEXClose;
	CI->scxWrite=scxAHEXWrite;
	CI->scxRead=scxAHEXRead;
	CI->scxCfgAddress=scxAHEXCfgAddress;
	CI->scxSetParameter=scxAHEXSetParameter;
	CI->scxGetParameter=scxAHEXGetParameter;
	CI->sciDestroyConnexionInterface=sciAHEXDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

//----------------------------------------

#endif
