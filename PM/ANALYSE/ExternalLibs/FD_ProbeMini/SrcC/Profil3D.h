
#ifdef SPG_General_USEProfil3D

typedef struct
{
	int Etat;

	Profil HiP;
	Profil LowP;

	G_Ecran SEGraph;//ecran 3D
	G_Ecran SEBoutons;//ecran des boutons
	G_Ecran SEopts;//ecran associe aux parametres de rendu
	G_Ecran SECoupeX;//ecrans de coupes XY
	G_Ecran SECoupeY;

	B_Lib BL;

	//IDs de boutons
	int LightAjust;
	int HeightAjust;
	int ZoomAjust;
	int FocaleAjust;
	int LockPos;
	int FrontView;
	int ShowAxes;
	int NewColor;
	int DefaultColor;
	int LoadColors;
	int LoadAux;
	int BSaveColors;
	int BTexMap;
	int MecaMove;
	int HiResDraw;
	int SaveProfil;
	int SavePicture;
	int BMaskMin;
	int BMedianFilter;
	int BConvFilter;
	int BSpace;
	int BRevY;
	int BLoad;
	int BQuit;
	int BShowScale;
	int BManualScale;

	//IDs de boutons pour les coupes XY
	int XYCut;
	int PntSel;
	int BSaveCutX;
	int BSaveCutY;


	int XSel;
	int YSel;
	int XYSelOk;

	C_Lib CL;

	SG_PDV Vue;//parametres de la vue 3D
#ifdef SPG_General_USESTEREOVIS
	SPG_STEREOVIS SV;
	int IsStereo;
#endif
#ifdef SPG_General_USESGRAPH_OPTS
	SG_OPTS* SGopts;//boutons de reglage des parametres
#endif

	SG_FullTexture T;
	PixCoul* ColorTable;
	int STX;
	int STY;

	SG_FullBloc HiB;//l'objet 3D
	SG_FullBloc LowB;//l'objet 3D
	SG_FullBloc HiBRef;
	SG_FullBloc LowBRef;

	SG_FullBloc AXES;
	SG_FullBloc AXESRef;

	float TMin;
	float TMax;
	float TDiff;//echelle Z
	float ZScale;

	V_REPERE V_Rep;
	V_VECT Rotation;//rotation
	V_VECT Light;//position de la source de lumiere
	float LightIntensity;
	float Diffusivity;
	//int TextureMode;

	float DT;

	int MSSX;
	int MSSY;
	V_VECT TMSSR;
	int MSSS;
	int MSTX;
	int MSTY;
	V_VECT TMSTR;

	S_TIMER ImageTimer;
#ifdef DebugProfil3D
	S_TIMER Timer[16];
#endif

} PROFIL3D_STATE;

#include "Profil3D.agh"

#ifdef DebugProfil3D
#define TmGeneral 0
#define TmGenerate3D 1
#define TmLight 2
#define TmInitRender 3
#define TmRender 4
#define TmButtons 5
#define TmOutside 6
#endif

#define PROFIL3D_OK 1
#define PROFIL3D_TEXMAP 2
#define PROFIL3D_TEXLIGHT 4

#endif
