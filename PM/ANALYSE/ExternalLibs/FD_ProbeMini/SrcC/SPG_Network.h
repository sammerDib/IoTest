
#ifdef SPG_General_USENetwork

#include "Config\SPG_Warning.h"

typedef struct
{
	union
	{
	DWORD IP;
	struct
	{
		BYTE IParray[4];
	};
	struct
	{
		BYTE IP0;
		BYTE IP1;
		BYTE IP2;
		BYTE IP3;
	};
	};
	union
	{
	WORD Port;
	struct
	{
		BYTE P0;
		BYTE P1;
	};
	};
} SPG_NET_ADDR;

typedef struct
{
	int Etat;
	char ServerName[64];

	WSADATA wsaData;
	SOCKET  conn_socket;
	sockaddr_in local;

	SPG_NET_ADDR LocalNetAddr;
#if(defined(DebugNetwork)&&defined(SPG_General_USEConsole))
	SPG_Console* CIn;
	SPG_Console* COut;
#endif
} SPG_NETWORK;


#include "SPG_Network.agh"

#define SPG_IsValidNetAddr(SNA) (SNA.IP)
#define SPG_IsEqualNetAddr(SNA0,SNA1) ((SNA0.IP==SNA1.IP)&&(SNA0.Port==SNA1.Port))
#define SPG_IsLocalHost(SNA0,SNA1) ((SNA1.IP==0x0100007f)&&(SNA0.Port==SNA1.Port))

#endif



