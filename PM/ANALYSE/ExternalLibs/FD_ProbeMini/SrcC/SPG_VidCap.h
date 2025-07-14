
typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	int POCT;
	G_Ecran E;
	int EcranReady;
	int ReverseY;
	void* SVID_Internal;
} SVID_STATE;

#define SVID_OK 1
#define SVID_CAPTURESTREAM 2

int SPG_CONV SVID_Init(SVID_STATE& SV, int PosX=64, int PosY=64, int capDeviceIndex=0, int DoConfig=1, int ReverseY=1);
void SPG_CONV SVID_Configure(SVID_STATE& SV, int ConfigSource=1, int ConfigFormat=1, int ConfigDisplay=1);
int SPG_CONV SVID_Update(SVID_STATE& SV);
int SPG_CONV SVID_StartContinuousGrab(SVID_STATE& SV);
void SPG_CONV SVID_StopContinuousGrab(SVID_STATE& SV);
void SPG_CONV SVID_Close(SVID_STATE& SV);

