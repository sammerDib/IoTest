

//struct SPG_Window;

typedef struct
{
	Profil PH;
	Profil PL;
	Profil PM;
	//Profil Pdest;
	Profil Psrc;
	G_Ecran E;

	int Frame;

	int Downsample;

#ifdef SPG_General_USEWindow
	int bShowWindow;
	SPG_Window SW;
#endif

	S_TIMER TTotal;
	S_TIMER TM;
	S_TIMER THL;
	S_TIMER TMHL;
	S_TIMER TN;
	S_TIMER TDraw;

} CamHP;

int SPG_CONV P_CamInit(CamHP& C, int SizeX, int SizeY, int Downsample=1, int bShowWindow=0);
int SPG_CONV P_CamClose(CamHP& C);
int SPG_CONV P_CamComputeContrast(CamHP& C, Profil& P);
int SPG_CONV P_CamUpdateContrast(CamHP& C, Profil& P);
int SPG_CONV P_CamProcess(CamHP& C, Profil& P);
int SPG_CONV P_CamSetDIBitsToDevice(CamHP& C, unsigned char* B, int Pitch, void* hDC);
