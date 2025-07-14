
#ifdef SPG_General_USEPrCV

typedef struct
{
	int Etat;
	G_Ecran FullScreen;
	G_Ecran ScreenBackup;
	G_Ecran E;
	G_Ecran EBut;
	B_Lib BL;
	C_Lib CL;
	Profil P;
	Profil PDisplay;
#ifdef SPG_General_USEPrXt
	ProfilExtract PrXt;
	ZoneProfile Selection;
#endif
#ifdef SPG_General_USEProfil3D
	PROFIL3D_STATE P3D;
	int IsStereo;
#endif
	SPG_INVERSEUR INVJ0;
	float ZMin;
	float ZMax;
	float RemDotsThreshold;

	int BLoad;
	int BFillB;
	int BRemB;
	int BSave;
	int BJ0;
	int BConv3x3;
	int BQuit;
#ifdef SPG_General_USEProfil3D
	int B3D;
#endif
	int BDivise;
	int BDirFilter;
	int BSub;
	int BLoadMsk;
	int BSaveMsk;
	int BSeuilBas;
	int BSeuilHaut;
	int BRemDots;
	int BDotsThreshold;
	int BFlatten;
	int B2DFlatten;
	int BStepHeight;
	int BOther;
	int BLevel;
	int BCrop;

}  CV_State;

#include "ProfilConvert.agh"

//CV_Update return codes
#define CV_OK 0
#define CV_ERROR 1
#define CV_FORCEUPDATE 2

#endif

