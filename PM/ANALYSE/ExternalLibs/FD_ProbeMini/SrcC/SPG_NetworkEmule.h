
#ifdef SPG_General_USENetworkEmule

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
	SPG_NET_ADDR LocalNetAddr;
	void* hMapFile;
	BYTE* MapAddress;
#if(defined(DebugNetwork)&&defined(SPG_General_USEConsole))
	SPG_Console* CIn;
	SPG_Console* COut;
#endif
} SPG_NETWORK;


#include "SPG_Network.agh"

#define SPG_IsValidNetAddr(SNA) (SNA.IP)
#define SPG_IsEqualNetAddr(SNA0,SNA1) ((SNA0.IP==SNA1.IP)&&(SNA0.Port==SNA1.Port))

#endif



