
#include "SPG_General.h"

#ifdef SPG_General_USENetworkEmule

#include "SPG_Includes.h"
#include "SPG_NetworkEmule.h"
#include "SPG_SysInc.h"
#include <string.h>
#include <stdio.h>

#define NetworkBuffer 16384
#define NetworkOffset 4
#define MsgHeader 10

#ifdef DebugNetwork
int SPG_CONV SPG_SetHookUDP(SPG_NETWORK& SN,SPG_Console* CIn,SPG_Console* COut)
{
	SN.CIn=CIn;
	SN.COut=COut;
	char Msg[256];
	sprintf(Msg,"Network Hook Enabled %s:%i.%i.%i.%i:%i",SN.ServerName,SN.LocalNetAddr.IP0,SN.LocalNetAddr.IP1,SN.LocalNetAddr.IP2,SN.LocalNetAddr.IP3,SN.LocalNetAddr.Port);
	Console_Add(SN.CIn,Msg);
	Console_Add(SN.COut,Msg);
	return -1;
}

int SPG_CONV SPG_CloseHookUDP(SPG_NETWORK& SN)
{
	char Msg[256];
	sprintf(Msg,"Network Hook Disabled %s:%i.%i.%i.%i:%i",SN.ServerName,SN.LocalNetAddr.IP0,SN.LocalNetAddr.IP1,SN.LocalNetAddr.IP2,SN.LocalNetAddr.IP3,SN.LocalNetAddr.Port);
	Console_Add(SN.CIn,Msg);
	Console_Add(SN.COut,Msg);
	SN.CIn=0;
	SN.COut=0;
	return -1;
}
#endif

int SPG_CONV SPG_InitUDP(SPG_NETWORK& SN, int Port)
{
	memset(&SN,0,sizeof(SPG_NETWORK));


	SN.LocalNetAddr.IP=(GetTickCount()<<8);
	SN.LocalNetAddr.IP0=192;
	SN.LocalNetAddr.IP3=0;
	if(Port)
	{
		SN.LocalNetAddr.Port=Port;
	}
	else
	{
		SN.LocalNetAddr.Port=GetTickCount()>>12;
	}
	sprintf(SN.ServerName,"IP_%i_%i_%i_%i_%i",SN.LocalNetAddr.IP0,SN.LocalNetAddr.IP1,SN.LocalNetAddr.IP2,SN.LocalNetAddr.IP3,SN.LocalNetAddr.Port);

	//file mapping
	SN.hMapFile = CreateFileMapping((void *)-1,    // Current file handle. 
		NULL,                              // Default security. 
		PAGE_READWRITE,                    // Read/write permission. 
		0,                                 // Max. object size. 
		NetworkBuffer,                          // Size of hFile. 
		SN.ServerName);            // Name of mapping object. 
 
	CHECK(SN.hMapFile==0,"SPG_InitUDP:CreateFileMapping",return 0;)

	SN.MapAddress = (BYTE*)MapViewOfFile(SN.hMapFile, // Handle to mapping object. 
		FILE_MAP_ALL_ACCESS,               // Read/write permission 
		0,                                 // Max. object size. 
		0,                                 // Size of hFile. 
		0);                                // Map entire file. 
 
	CHECK(SN.MapAddress==0,"SPG_InitUDP:MapViewOfFile",return 0;);

	memset(SN.MapAddress,0,NetworkBuffer);

	return SN.Etat=-1;
}

int SPG_CONV SPG_Resolve(SPG_NET_ADDR& SNA,char *server_name) 
{
	return 0;
}

void SPG_CONV SPG_CloseUDP(SPG_NETWORK& SN)
{
	CHECK(SN.Etat==0,"SPG_CloseUDP: SPG_NETWORK nul",memset(&SN,0,sizeof(SPG_NETWORK));return);
	memset(SN.MapAddress,0,NetworkBuffer);
	if(SN.MapAddress!=0) UnmapViewOfFile(SN.MapAddress);
	if(SN.hMapFile!=0) CloseHandle(SN.hMapFile);
	memset(&SN,0,sizeof(SPG_NETWORK));
	return;
}

int SPG_CONV SPG_SendUDP(SPG_NETWORK& SN,SPG_NET_ADDR& SNA,void * Data, int LenData)
{
	CHECK(SN.Etat==0,"SPG_SendUDP: SPG_NETWORK nul",return 0);
	CHECK(SPG_IsValidNetAddr(SNA)==0,"SPG_SendUDP: destination nulle",return 0;);

	char DestName[64];
	sprintf(DestName,"IP_%i_%i_%i_%i_%i",SNA.IP0,SNA.IP1,SNA.IP2,SNA.IP3,SNA.Port);

HANDLE hMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS, // Read/write permission. 
    FALSE,                             // Do not inherit the name
    DestName);            // of the mapping object. 
 
CHECKTWO(hMapFile==0,"SPG_SendUDP:OpenFileMapping",DestName,return 0;)
 
BYTE* MapAddress = (BYTE*)MapViewOfFile(hMapFile, // Handle to mapping object. 
    FILE_MAP_ALL_ACCESS,               // Read/write permission. 
    0,                                 // Max. object size. 
    0,                                 // Size of hFile. 
    0);                                // Map entire file. 
 
CHECK(MapAddress==0,"SPG_SendUDP:MapViewOfFile",CloseHandle(hMapFile);return 0;)

	int LenMax=NetworkBuffer-NetworkOffset;
	if(*(DWORD*)MapAddress!=0)
	{
		SPG_Sleep(5);
	}
	if(*(DWORD*)MapAddress!=0)
	{
		SPG_Sleep(5);
	}
	if(*(DWORD*)MapAddress!=0)
	{
		SPG_Sleep(5);
	}
	CHECK(*(DWORD*)MapAddress!=0,"SPG_SendUDP:Buffer Lock",return 0);
	(*(DWORD*)MapAddress)++;
	CHECK(*(DWORD*)MapAddress!=1,"SPG_SendUDP:Buffer Lock",(*(DWORD*)MapAddress)--;return 0);
	BYTE* MsgAddress=MapAddress+NetworkOffset;

	while(*(DWORD*)MsgAddress>0)
	{
		CHECK(!V_IsBound(*(DWORD*)MsgAddress,0,NetworkBuffer),"SPG_SendUDP",(*(DWORD*)MapAddress)--;UnmapViewOfFile(MapAddress);CloseHandle(hMapFile);return 0);
		LenMax-=*(DWORD*)MsgAddress;
		CHECK(LenMax<MsgHeader+LenData,"SPG_SendUDP",(*(DWORD*)MapAddress)--;UnmapViewOfFile(MapAddress);CloseHandle(hMapFile);return 0;);
		MsgAddress+=*(DWORD*)MsgAddress+MsgHeader;
	}
	*(DWORD*)MsgAddress=LenData;
	*((DWORD*)MsgAddress+1)=SN.LocalNetAddr.IP;
	*((WORD*)MsgAddress+4)=SN.LocalNetAddr.Port;
	memcpy(MsgAddress+MsgHeader,Data,LenData);
	memset(MsgAddress+MsgHeader+LenData,0,sizeof(DWORD));

	(*(DWORD*)MapAddress)--;

	UnmapViewOfFile(MapAddress);
	CloseHandle(hMapFile);

#ifdef DebugNetwork
	char Msg[256];
	sprintf(Msg,"Snd %i.%i.%i.%i:%i %i Bytes ",SNA.IP0,SNA.IP1,SNA.IP2,SNA.IP3,SNA.Port,LenData);
	for(int i=0;i<V_Min(LenData/2,6);i++)
	{
		char B[64];
		sprintf(B,"%X.",((DWORD*)Data)[i]);
		strcat(Msg,B);
	}
	strcat(Msg,"..");
	Console_Add(SN.COut,Msg);
#endif
	return LenData;
}

int SPG_CONV SPG_ReadUDP(SPG_NETWORK& SN,SPG_NET_ADDR& SNA,void * Data, int LenData)
{
	CHECK(SN.Etat==0,"SPG_ReadUDP: SPG_NETWORK nul",return 0);

	BYTE* MapAddress=SN.MapAddress;

	int LenMax=NetworkBuffer-NetworkOffset;
	if(*(DWORD*)MapAddress!=0)
	{
		SPG_Sleep(5);
	}
	if(*(DWORD*)MapAddress!=0)
	{
		SPG_Sleep(5);
	}
	if(*(DWORD*)MapAddress!=0)
	{
		SPG_Sleep(5);
	}
	CHECK(*(DWORD*)MapAddress!=0,"SPG_ReadUDP:Buffer Lock",return 0);
	(*(DWORD*)MapAddress)++;
	CHECK(*(DWORD*)MapAddress!=1,"SPG_ReadUDP:Buffer Lock",(*(DWORD*)MapAddress)--;return 0);
	BYTE* MsgAddress=MapAddress+NetworkOffset;

#ifdef DebugNetwork
	{
		char Msg[256];
		int LenMsg=*(DWORD*)MsgAddress;
		BYTE* MessageIP=MsgAddress+4;
		if(LenMsg) Console_Add(SN.CIn,"\n");
		while(LenMsg>0)
		{
			sprintf(Msg,"Rcv %i.%i.%i.%i:%i %i Bytes ",
				(int)MessageIP[0],(int)MessageIP[1],(int)MessageIP[2],(int)MessageIP[3],(int)*((WORD*)(MessageIP+4)),LenData);
			BYTE* Message=MessageIP+6;
			for(int i=0;i<V_Min(LenData/4,4);i++)
			{
				char B[64];
				sprintf(B,"%X.",((DWORD*)Message)[i]);
				strcat(Msg,B);
			}
			strcat(Msg,"..");
			Console_Add(SN.CIn,Msg);
			LenMsg=*(DWORD*)(Message+LenData);
			MessageIP=Message+LenData+6;
		}
	}
#endif

	int LenMsg=*(DWORD*)MsgAddress;
	if(LenMsg>0)
	{
		CHECK(LenData<LenMsg,"SPG_ReadUDP",(*(DWORD*)MapAddress)--;return 0);
		SNA.IP=*(DWORD*)(MsgAddress+4);
		SNA.Port=*(WORD*)(MsgAddress+8);
		memcpy(Data,MsgAddress+MsgHeader,LenMsg);
		memmove(MsgAddress,MsgAddress+MsgHeader+LenMsg,LenMax-MsgHeader-LenMsg);
	}
	(*(DWORD*)MapAddress)--;
	return LenMsg;
}

#endif

