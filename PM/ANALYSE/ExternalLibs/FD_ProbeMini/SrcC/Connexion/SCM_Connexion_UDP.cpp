
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USESCXUDP

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h" //inclue les declarations de winsock2.h

#pragma comment(lib,"ws2_32.lib")


#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_UDP
#define sci_NAME sci_NAME_UDP

// ###################################################

#define MAXUDPSIZE 65536 //pour le dimentionnement, la limite réelle peut être plus basse

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	WSADATA wsadata;
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface



// ###################################################

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	//read
	int LocalPort;
	//write
	int AnswerMode; //0 : to last sender 1 : to fixed address (specified)
	int RemotePort;
	int wIP0;	int wIP1;	int wIP2;	int wIP3;
} SCX_ADDRESS;

// ###################################################

typedef struct
{
	SOCKET s;
	sockaddr_in a;
} SCXSOCK;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;//obligatoire
	union
	{
		SCXSOCK R;
		SCXSOCK W;
	};

	char LocalName[256];
	sockaddr_in Local;
} SCX_STATE; //parametres d'une connexion en particulier


// ###################################################

static SCX_CONNEXION* SPG_CONV scxUDPOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	CHECK((State.R.s=socket(AF_INET,SOCK_DGRAM,IPPROTO_UDP))==INVALID_SOCKET,"socket failed",scxFree(C->State);scxFree(C);return 0);

	{		unsigned long t=0xffffffff;	ioctlsocket(State.R.s,FIONBIO,&t);	} //NON BLOCKING MODE

	State.R.a.sin_family = AF_INET; State.R.a.sin_addr.S_un.S_addr = htonl(INADDR_ANY); State.R.a.sin_port = htons(State.Address.LocalPort);
	CHECKV(bind(State.R.s, (sockaddr*)&State.R.a, sizeof(sockaddr_in)),"Bind failed on local port:",State.Address.LocalPort,closesocket(State.R.s);scxFree(C->State);scxFree(C);return 0);

	//buffers
	int sz=4;	int b=0;	getsockopt(State.R.s,SOL_SOCKET,SO_RCVBUF,(char *)&b,&sz); if(b<MAXUDPSIZE) { 	b=MAXUDPSIZE; 	setsockopt(State.R.s,SOL_SOCKET,SO_RCVBUF,(const char *)&b,sizeof(int)); }
	sz=4;	b=0;			getsockopt(State.R.s,SOL_SOCKET,SO_SNDBUF,(char *)&b,&sz); if(b<MAXUDPSIZE) { 	b=MAXUDPSIZE; 	setsockopt(State.R.s,SOL_SOCKET,SO_SNDBUF,(const char *)&b,sizeof(int)); }

	//infos generiques
	CHECK(gethostname(State.LocalName,256)!=0,"Failed to get local address",;);
	sz=sizeof(sockaddr_in);	CHECKTWO(getsockname(State.R.s,(sockaddr*)&State.Local,&sz),"Failed to resolve name:",State.LocalName,;);

	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxUDPClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;
	closesocket(State.R.s);
	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxUDPWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	if(State.Address.AnswerMode!=0)//sinon write repond à l'adresse du dernier message recu
	{
		State.W.a.sin_addr.S_un.S_un_b.s_b1=State.Address.wIP0;	State.W.a.sin_addr.S_un.S_un_b.s_b2=State.Address.wIP1;	State.W.a.sin_addr.S_un.S_un_b.s_b3=State.Address.wIP2;	State.W.a.sin_addr.S_un.S_un_b.s_b4=State.Address.wIP3;
		State.W.a.sin_port=htons(State.Address.RemotePort);
	}
	DataLen=sendto(State.W.s,(char*)Data,DataLen,0,(sockaddr*)&State.W.a,sizeof(sockaddr));
	CHECK(DataLen<0,"sendto failed",DataLen=0);
	return DataLen;
}

// ###################################################

static int SPG_CONV scxUDPRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	int sz=sizeof(struct sockaddr_in);
	DataLen = recvfrom(State.R.s,(char*)Data,DataLen,0,(sockaddr*)(&State.R.a),&sz); //retval=octetslus
	if(DataLen>=0)  return DataLen; //cas normal (attn: scxUDPRead confond no data et null datagram)
	if((DataLen==-1)||(DataLen==WSAEWOULDBLOCK)) return DataLen=0;//no data (different from null datagram) //no data ready
	CHECK(DataLen==WSAEMSGSIZE,"UDP message size error - message lost",return DataLen=0);//buffer too small
	CHECKV(1,"UDP read error:",DataLen,return DataLen=0);//msg for other error and return
}

// ###################################################

static void SPG_CONV scxUDPCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->LocalPort=1978;
		Address->AnswerMode=0;
		Address->RemotePort=1978;
		Address->wIP0=127;	Address->wIP1=0;	Address->wIP2=0;	Address->wIP3=1;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,CFG.FileName[0]?1:0);

	CFG_IntParam(CFG,"LocalPort",&Address->LocalPort,"Local port",CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,65535);
	CFG_IntParam(CFG,"AnswerMode",&Address->AnswerMode,"0:Reply to last sender 1:reply to fixed address specified by WritePort and WriteIP",CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"RemotePort",&Address->RemotePort,"Write to specified port",CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,65535);
	CFG_IntParam(CFG,"wIP0",&Address->wIP0,"Write to specified IP",CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,255);
	CFG_IntParam(CFG,"wIP1",&Address->wIP1,"Write to specified IP",CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,255);
	CFG_IntParam(CFG,"wIP2",&Address->wIP2,"Write to specified IP",CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,255);
	CFG_IntParam(CFG,"wIP3",&Address->wIP3,"Write to specified IP",CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,255);
	return;
}

// ###################################################

static int SPG_CONV scxUDPSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxUDPGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciUDPDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	WSACleanup();
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciUDPCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETRESPECT);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Ethernet UDP";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=MAXUDPSIZE;//spécifique

	CI->scxOpen=scxUDPOpen;
	CI->scxClose=scxUDPClose;
	CI->scxWrite=scxUDPWrite;
	CI->scxRead=scxUDPRead;
	CI->scxCfgAddress=scxUDPCfgAddress;
	CI->scxSetParameter=scxUDPSetParameter;
	CI->scxGetParameter=scxUDPGetParameter;
	CI->sciDestroyConnexionInterface=sciUDPDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	CHECK(WSAStartup(0x202,&sciParameters.wsadata),"WSAStartup echoue",	WSACleanup();scxFree(CI->sciParameters);return 0);

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif
