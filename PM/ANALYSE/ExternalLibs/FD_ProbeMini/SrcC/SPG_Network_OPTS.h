
#ifdef SPG_General_USENetwork_OPTS

typedef struct
{
	int Etat;

	B_Lib BL;
	C_Lib CL;
	G_Ecran Ecran;
	G_Ecran CslEcran;
	G_Ecran ERemote;
	SPG_Console Console;
	//int ScreenDwnSample;

	int BIP0;
	int BIP1;
	int BIP2;
	int BIP3;
	int BPort;
	int BScreenView;
	int BRequestControl;
	int BPing;
	int BErrorReport;
	int BBreak;
	int BPacket;

	int IP0;
	int IP1;
	int IP2;
	int IP3;
	int Port;
	int ReqScreenRate;

	S_TIMER SPing;
	S_TIMER DebitTimer;
	int DebitCumule;
	//S_TIMER SRefreshScreen;
	//S_TIMER SPingRate;

	SPG_NET_PROTOCOL* SNP;
	SPG_NET_ADDR IPDest;

	MELINK_SCREEN_RCV_STATE ScreenRcvState;

} SPG_NET_OPTS;

#include "SPG_Network_OPTS.agh"

#define SNO_OK 1
#define SNO_AUTOUPDATE 2

#endif
