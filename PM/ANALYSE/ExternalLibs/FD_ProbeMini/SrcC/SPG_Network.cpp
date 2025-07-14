
#include "SPG_General.h"

#ifdef SPG_General_USENetwork
#pragma comment(lib,"ws2_32.lib")

#include "SPG_Includes.h"

#include <memory.h>
#include <stdlib.h>

#ifdef DebugNetwork
#include <string.h>
#include <stdio.h>

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

int SPG_CONV SPG_InitUDP(SPG_NETWORK& SN,int Port,int ReusePort)
{
	SPG_ZeroStruct(SN);

	CHECK(WSAStartup(0x202,&SN.wsaData)==SOCKET_ERROR,
		"WSAStartup echoue",
		WSACleanup();return 0)

	/* Open a socket */
	CHECK((SN.conn_socket=socket(AF_INET,SOCK_DGRAM,0))==INVALID_SOCKET,
		"socket echoue",
		WSACleanup();memset(&SN,0,sizeof(SPG_NETWORK));return 0);

	{
		int U=-1;
		if(ReusePort) setsockopt(SN.conn_socket,SOL_SOCKET,SO_REUSEADDR,(const char *)&U,sizeof(int));
		setsockopt(SN.conn_socket,IPPROTO_TCP,TCP_NODELAY,(const char *)&U,sizeof(int));
		U=-1;
		setsockopt(SN.conn_socket,IPPROTO_TCP,TCP_NODELAY,(const char *)&U,sizeof(int));
		U=4;
		int RcvBuff=0;
		getsockopt(SN.conn_socket,SOL_SOCKET,SO_RCVBUF,(char *)&RcvBuff,&U);
		if(RcvBuff<65536) 
		{
			RcvBuff=65536;
			setsockopt(SN.conn_socket,SOL_SOCKET,SO_RCVBUF,(const char *)&RcvBuff,sizeof(int));
		}
		U=4;
		int SndBuff=0;
		getsockopt(SN.conn_socket,SOL_SOCKET,SO_SNDBUF,(char *)&SndBuff,&U);
		if(SndBuff<65536) 
		{
			SndBuff=65536;
			setsockopt(SN.conn_socket,SOL_SOCKET,SO_SNDBUF,(const char *)&SndBuff,sizeof(int));
		}
	}


	SN.local.sin_family = AF_INET;
	if(Port)
	{
		SN.LocalNetAddr.Port = SN.local.sin_port = htons((u_short)Port);
	}
	else
	{
	}

	SN.local.sin_addr.s_addr = INADDR_ANY;

#ifdef DebugNetwork
	CHECK(bind(SN.conn_socket,(struct sockaddr*)&SN.local,sizeof(sockaddr_in))==SOCKET_ERROR,
		"bind echoue",
		WSACleanup();memset(&SN,0,sizeof(SPG_NETWORK));return 0);
#else
	if(bind(SN.conn_socket,(struct sockaddr*)&SN.local,sizeof(sockaddr_in))==SOCKET_ERROR)
	{
		WSACleanup();
		memset(&SN,0,sizeof(SPG_NETWORK));
		return 0;
	}
#endif

	gethostname(SN.ServerName,64);

	{
	int U=sizeof(sockaddr_in);
	sockaddr_in local;
	if(getsockname(SN.conn_socket,(struct sockaddr*)&local,&U)==0)
	{
		SN.local=local;
		//SN.LocalNetAddr.IP=SN.local.sin_addr.s_addr;
		SN.LocalNetAddr.Port = ntohs(SN.local.sin_port);//remplit le port
	}
	}

	SPG_Resolve(SN.LocalNetAddr,SN.ServerName);//remplit l'IP



	{//NON BLOCKING MODE
	unsigned long T=0xFFFFFFFF;
	ioctlsocket(SN.conn_socket,FIONBIO,&T);
	}

	return SN.Etat=-1;
}

#pragma warning( disable : 4706)//assignment within conditional expression

int SPG_CONV SPG_Resolve(SPG_NET_ADDR& SNA,char *server_name)
{
	CHECK(server_name==0||(server_name[0]==0),"SPG_Resolve",return 0);

	char* i;
	//assignation volontaire
	if(i=SPG_StrFind(server_name,":"))//assignement volontaire
	{
		SNA.Port=atoi(i+1);
		*i=0;
		if(SPG_Resolve(SNA,server_name)==0) return 0;
		*i=':';
		return -1;
	}

	if (isalpha(server_name[0]))
	{   /* server address is a name */
		hostent *hp = gethostbyname(server_name);
			if (hp == NULL ) {
				SNA.IP=0;
				return 0;
			}
			if (hp->h_addr_list==0) {
				SNA.IP=0;
				return 0;
			}
		SNA.IP=*((DWORD*)(hp->h_addr_list[0]));
		if(SNA.IP==INADDR_NONE) 
		{
			SNA.IP=0;
			return 0;
		}
	}
	else
	{
		SNA.IP = inet_addr(server_name);
		if((SNA.IP==INADDR_NONE)||(SNA.IP==0)) 
		{
			SNA.IP=0;
			return 0;
		}
	}
	return -1;
}

void SPG_CONV SPG_CloseUDP(SPG_NETWORK& SN)
{
	CHECK(SN.Etat==0,"SPG_CloseUDP: SPG_NETWORK nul",memset(&SN,0,sizeof(SPG_NETWORK));return);
	closesocket(SN.conn_socket);
	WSACleanup();
	memset(&SN,0,sizeof(SPG_NETWORK));
	return;
}

int SPG_CONV SPG_SendUDP(SPG_NETWORK& SN,SPG_NET_ADDR& SNA,void * Data, int LenData)
{
	CHECK(SN.Etat==0,"SPG_SendUDP: SPG_NETWORK nul",return 0);
	CHECK(SPG_IsValidNetAddr(SNA)==0,"SPG_SendUDP: destination nulle",return 0);
	G_LogTimeEV(LT_NETSEND,Len);
#ifdef DebugNetworkTimer
		S_StartTimer(Global.T_NET_Snd);
#endif
	sockaddr_in Dest=SN.local;
	Dest.sin_addr.s_addr=SNA.IP;
	if(SNA.Port) Dest.sin_port=htons(SNA.Port);
	int retval = sendto(SN.conn_socket,(char*)Data,LenData,0,
			(sockaddr*)&Dest,sizeof(sockaddr));
#ifdef DebugNetworkTimer
		S_StopTimer(Global.T_NET_Snd);
#endif
#ifdef DebugNetwork
	char Msg[256];
	sprintf(Msg,"Snd %i.%i.%i.%i:%i %i Bytes ",SNA.IP0,SNA.IP1,SNA.IP2,SNA.IP3,SNA.Port,LenData);
	int i;
	for(i=0;i<V_Min(LenData/2,6);i++)
	{
		SPG_ArrayStackAlloc(char,B,64);
		sprintf(B,"%X.",((WORD*)Data)[i]);
		SPG_ArrayStackCheck(B);
		strcat(Msg,B);
	}
	if(i<retval/2) strcat(Msg,"..");
	Console_Add(SN.COut,Msg);
#endif
	CHECK(retval!=LenData,"SPG_SendUDP",return 0);
	G_LogTimeRV(LT_NETSEND,Len);
	return LenData;
}

int SPG_CONV SPG_ReadUDP(SPG_NETWORK& SN,SPG_NET_ADDR& SNA,void * Data, int LenData)
{
	CHECK(SN.Etat==0,"SPG_ReadUDP: SPG_NETWORK nul",return 0);
	G_LogTimeEV(LT_NETREAD,0);
#ifdef DebugNetworkTimer
		S_StartTimer(Global.T_NET_Rcv);
#endif
	int retval;

#ifdef DebugNetwork
	int BufferInputBytes=0;
	ioctlsocket(SN.conn_socket,FIONREAD,(unsigned long*)&BufferInputBytes);
#endif

	struct sockaddr_in serverrecu;
	int lenserverrecu=sizeof(struct sockaddr_in);
	retval = recvfrom(SN.conn_socket,(char*)Data,LenData,0,
			(sockaddr*)(&serverrecu),&lenserverrecu);//retval=octetslus
	if (retval<=0) 
	{
		switch (retval)
		{
		case -1:
			//pas de données à lire
#ifdef DebugNetwork
			DbgCHECK(BufferInputBytes>0,"SPG_ReadUDP: LenData<Message size - Message lost");
#endif
			break;
		case WSAEWOULDBLOCK:
			//pas de données à lire
			break;
		case WSAEMSGSIZE:
			DbgCHECK(retval==WSAEMSGSIZE,"SPG_ReadUDP: LenData<Message size - Part of message lost");
			break;
		default:
			DbgCHECK(1,"SPG_ReadUDP: read error");
		}

#ifdef DebugNetworkTimer
		S_StopTimer(Global.T_NET_Rcv);
#endif
		G_LogTimeRV(LT_NETREAD,0);
		return 0;
	}

	SNA.IP=serverrecu.sin_addr.s_addr;
	SNA.Port=ntohs(serverrecu.sin_port);

#ifdef DebugNetworkTimer
		S_StopTimer(Global.T_NET_Rcv);
#endif

#ifdef DebugNetwork
	char Msg[256];
	sprintf(Msg,"Rcv %i.%i.%i.%i:%i %i Bytes ",SNA.IP0,SNA.IP1,SNA.IP2,SNA.IP3,SNA.Port,retval);
	int i;
	for(i=0;i<V_Min(retval/2,6);i++)
	{
		SPG_ArrayStackAlloc(char,B,64);
		sprintf(B,"%X.",((WORD*)Data)[i]);
		SPG_ArrayStackCheck(B);
		strcat(Msg,B);
	}
	if(i<retval/2) strcat(Msg,"..");
	Console_Add(SN.CIn,Msg);
#endif

	G_LogTimeRV(LT_NETREAD,retval);
	return retval;
}


#endif

