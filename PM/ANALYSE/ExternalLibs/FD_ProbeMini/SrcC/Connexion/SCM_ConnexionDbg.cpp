

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef DebugNetwork

#include "..\SPG_Includes.h"
#include <string.h>
#include <stdio.h>

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	BYTE Private[];
} SCX_ADDRESS;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
} SCX_STATE; //parametres d'une connexion en particulier

#define DATASTRINGLEN 256
#define ADDRESSHEXLEN 256
#define ADDRESSCFGLEN 512

typedef struct
{
	char Name[SCI_CONNEXION_NAME];
	char FctName[32];
	char Message[64];
	char AddressHEX[ADDRESSHEXLEN];
	char AddressCFG[ADDRESSCFGLEN];
	char DataPtr[16];
	char DataLen[16];
	char DataString[DATASTRINGLEN];
	char DataHex[DATASTRINGLEN];
	char Retval[16];
} GLOBAL_MESSAGE;

typedef struct
{
	void* UserData;
	int MemCheckVal0;
	MSGCALLBACK MsgCallback;
	int MemCheckVal1;
	GLOBAL_MESSAGE GM;
	int MemCheckVal2;
	char WholeMessage[1024];
	int MemCheckVal3;
} GLOBAL_CONNEXION;

GLOBAL_CONNEXION GlobalConnexion;

void SPG_CONV scxStartDebug(MSGCALLBACK MsgCallback, void* UserData)
{
	CHECK(MsgCallback==0,"scxStartDebug",return);
	SPG_ZeroStruct(GlobalConnexion);
	GlobalConnexion.MemCheckVal3=GlobalConnexion.MemCheckVal2=GlobalConnexion.MemCheckVal1=GlobalConnexion.MemCheckVal0=0x55AA55AA;
	GlobalConnexion.MsgCallback=MsgCallback;
	GlobalConnexion.UserData=UserData;
/*
	MSGCALLBACK lMsgCallback=0;
	V_SWAP(MSGCALLBACK,GlobalConnexion.MsgCallback,lMsgCallback);//pour éviter la réentrance
	lMsgCallback("scxStartDebug",UserData);
	V_SWAP(MSGCALLBACK,GlobalConnexion.MsgCallback,lMsgCallback);//pour éviter la réentrance
*/
	return;
}

void SPG_CONV scxStopDebug()
{
	if(GlobalConnexion.MsgCallback==0) return;
/*
	MSGCALLBACK MsgCallback=0;
	V_SWAP(MSGCALLBACK,GlobalConnexion.MsgCallback,MsgCallback);//pour éviter la réentrance
	MsgCallback("scxStopDebug",GlobalConnexion.UserData);
	V_SWAP(MSGCALLBACK,GlobalConnexion.MsgCallback,MsgCallback);//pour éviter la réentrance
*/
	GlobalConnexion.MsgCallback=0;
	return;
}

void SPG_CONV scxSendDebug()
{
	if(GlobalConnexion.MsgCallback==0) return;

	CHECK(GlobalConnexion.MemCheckVal0!=0x55AA55AA,"scxSendDebug",return);
	CHECK(GlobalConnexion.MemCheckVal1!=0x55AA55AA,"scxSendDebug",return);
	CHECK(GlobalConnexion.MemCheckVal2!=0x55AA55AA,"scxSendDebug",return);
	CHECK(GlobalConnexion.MemCheckVal3!=0x55AA55AA,"scxSendDebug",return);
	GLOBAL_MESSAGE& GM=GlobalConnexion.GM;

	GlobalConnexion.WholeMessage[0]=0;

	if(GM.Name			[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),    "Name=%s",			GM.Name			);
	if(GM.FctName		[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\tFctName=%s",		GM.FctName		);
	if(GM.Message		[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\tMessage=%s",		GM.Message		);
	//if(GM.AddressHEX	[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\r\nAddressHEX=%s",		GM.AddressHEX	);
	//if(GM.AddressCFG	[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\r\nAddressCFG:\r\n%s",	GM.AddressCFG	);
	//if(GM.DataPtr		[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\r\nDataPtr=%s",		GM.DataPtr		);
	if(GM.DataLen		[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\r\nDataLen=%s",		GM.DataLen		);
	if(GM.DataString	[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\r\nDataString=%s",		GM.DataString	);
	if(GM.DataHex		[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\r\nDataHex=%s",		GM.DataHex		);
	if(GM.DataString||GM.DataHex)
	{
		if(GM.Retval		[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\r\nRetval=%s",			GM.Retval		);
	}
	else
	{
		if(GM.Retval		[0]) sprintf(GlobalConnexion.WholeMessage+strlen(GlobalConnexion.WholeMessage),"\tRetval=%s",			GM.Retval		);
	}

	CHECK(GlobalConnexion.MemCheckVal0!=0x55AA55AA,"scxSendDebug",return);
	CHECK(GlobalConnexion.MemCheckVal1!=0x55AA55AA,"scxSendDebug",return);
	CHECK(GlobalConnexion.MemCheckVal2!=0x55AA55AA,"scxSendDebug",return);
	CHECK(GlobalConnexion.MemCheckVal3!=0x55AA55AA,"scxSendDebug",return);

	MSGCALLBACK MsgCallback=0;
	V_SWAP(MSGCALLBACK,GlobalConnexion.MsgCallback,MsgCallback);//pour éviter la réentrance

	MsgCallback(GlobalConnexion.WholeMessage,GlobalConnexion.UserData);

	V_SWAP(MSGCALLBACK,GlobalConnexion.MsgCallback,MsgCallback);

	SPG_ZeroStruct(GlobalConnexion.GM);
	return;
}

void SPG_CONV scxPartialFillAddress(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address)
{
	if(GlobalConnexion.MsgCallback==0) return;
	//horrible - debug only !
	//scxDataToHEX((void*)((BYTE*)Address+sizeof(SCX_ADDRESSHEADER)),CI->sizeofAddress-sizeof(SCX_ADDRESSHEADER),GlobalConnexion.GM.AddressHEX,ADDRESSHEXLEN);
	//sciDataToHex(Address->Private,CI->sizeofAddress-sizeof(SCX_ADDRESSHEADER),GlobalConnexion.GM.AddressHEX,ADDRESSHEXLEN);
	SPG_StackAllocZ(SPG_CONFIGFILE,CFG);
	if(scxCfgAddress(CFG,Address,CI,scxCFGREADONLY))
	{
#define OutputStringLen 128
		SPG_ArrayStackAlloc(char,OutputString,OutputStringLen);
		for(int i=1;i<CFG.NumParams;i++)
		{
			if(i>1) strcat(GlobalConnexion.GM.AddressCFG,"\r\n");
			CFG_ParamVarFToString(CFG,CFG.CP[i],SPG_CFG_FORMAT_TAB,OutputString,OutputStringLen);
			strcat(GlobalConnexion.GM.AddressCFG,OutputString);
		}
		SPG_ArrayStackCheck(OutputString);
		CFG_Close(CFG);
	}
	return;
}

void SPG_CONV scxSendDebugConnexionAddress(char* FctName, char* Message, SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address)
{
	if(GlobalConnexion.MsgCallback==0) return;

	strcpy(GlobalConnexion.GM.Name,CI->Name);
	strcpy(GlobalConnexion.GM.FctName,FctName);
	strcpy(GlobalConnexion.GM.Message,Message);
	scxPartialFillAddress(CI,Address);
	scxSendDebug();
	return;
}

void SPG_CONV scxSendDebugConnexion(char* FctName, char* Message, SCX_CONNEXION* C)
{
	if(GlobalConnexion.MsgCallback==0) return;

	strcpy(GlobalConnexion.GM.Name,C->CI->Name);
	strcpy(GlobalConnexion.GM.FctName,FctName);
	strcpy(GlobalConnexion.GM.Message,Message);
	scxPartialFillAddress(C->CI,C->Address);
	scxSendDebug();
	return;
}

void SPG_CONV scxSendDebugDataRetval(char* FctName, char* Message, void* Data, int DataLen, int Retval, SCX_CONNEXION* C)
{
	if(GlobalConnexion.MsgCallback==0) return;

	strcpy(GlobalConnexion.GM.Name,C->CI->Name);
	strcpy(GlobalConnexion.GM.FctName,FctName);
	strcpy(GlobalConnexion.GM.Message,Message);
	scxPartialFillAddress(C->CI,C->Address);
	sciDataPtrToHEX(Data,GlobalConnexion.GM.DataPtr);
	sprintf(GlobalConnexion.GM.DataLen,"%i",DataLen);
	sciDataToString(Data,DataLen,GlobalConnexion.GM.DataString,DATASTRINGLEN);
	sciDataToHex(Data,DataLen,GlobalConnexion.GM.DataHex,DATASTRINGLEN);
	sprintf(GlobalConnexion.GM.Retval,"%i",Retval);
	scxSendDebug();
	return;
}

void SPG_CONV scxSendDebugRetval(char* FctName, char* Message, int Retval)
{
	if(GlobalConnexion.MsgCallback==0) return;

	strcpy(GlobalConnexion.GM.FctName,FctName);
	strcpy(GlobalConnexion.GM.Message,Message);
	sprintf(GlobalConnexion.GM.Retval,"%i",Retval);
	scxSendDebug();
	return;
}

void SPG_CONV scxSendDebugConnexionRetval(char* FctName, char* Message, int Retval, SCX_CONNEXION* C)
{
	if(GlobalConnexion.MsgCallback==0) return;

	strcpy(GlobalConnexion.GM.Name,C->CI->Name);
	strcpy(GlobalConnexion.GM.FctName,FctName);
	strcpy(GlobalConnexion.GM.Message,Message);
	scxPartialFillAddress(C->CI,C->Address);
	sprintf(GlobalConnexion.GM.Retval,"%i",Retval);
	scxSendDebug();
	return;
}

void SPG_CONV scxSendDebugData(char* FctName, char* Message, void* Data, int DataLen, SCX_CONNEXION* C)
{
	if(GlobalConnexion.MsgCallback==0) return;

	strcpy(GlobalConnexion.GM.Name,C->CI->Name);
	strcpy(GlobalConnexion.GM.FctName,FctName);
	strcpy(GlobalConnexion.GM.Message,Message);
	scxPartialFillAddress(C->CI,C->Address);
	sciDataPtrToHEX(Data,GlobalConnexion.GM.DataPtr);
	sprintf(GlobalConnexion.GM.DataLen,"%i",DataLen);
	sciDataToString(Data,DataLen,GlobalConnexion.GM.DataString,DATASTRINGLEN);
	sciDataToHex(Data,DataLen,GlobalConnexion.GM.DataHex,DATASTRINGLEN);
	scxSendDebug();
	return;
}

void SPG_CONV scxSendDebugConnexionLen(char* FctName, char* Message, void* Data, int DataLen, SCX_CONNEXION* C)
{
	if(GlobalConnexion.MsgCallback==0) return;

	strcpy(GlobalConnexion.GM.Name,C->CI->Name);
	strcpy(GlobalConnexion.GM.FctName,FctName);
	strcpy(GlobalConnexion.GM.Message,Message);
	scxPartialFillAddress(C->CI,C->Address);
	sciDataPtrToHEX(Data,GlobalConnexion.GM.DataPtr);
	sprintf(GlobalConnexion.GM.DataLen,"%i",DataLen);
	scxSendDebug();
	return;
}

#endif
#endif

