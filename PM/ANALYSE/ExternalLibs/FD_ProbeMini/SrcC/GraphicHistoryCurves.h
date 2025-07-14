

// ##############################            DONNEES            ############################

//entete pour un echantillon 
typedef struct
{
	BYTE Weight; //niveau de sous-echantillonnage
	BYTE FlagOR;
	BYTE FlagAND;
	BYTE Unused0;
	DWORD DUmmy;
} GHCURVESHEADER;

typedef struct //point d'une courbe, pour un point unique fmin=fmax=fmean=valeur de la courbe, apres sous-echantillonnage fmin=min(tous les echantillons d'origine), fmax=max(...) fmean=mean(...)
{
	double fMin;
	double fMean;
	double fMax;
} GHSAMPLE;

typedef struct//un echantillon est un entete plus un nombre de courbes défini à l'init des données
{
	GHCURVESHEADER hch;
	GHSAMPLE hcs[];
} GHRECORD;

typedef struct //la structure complete
{
	int LT_GH;
	union
	{
		int MaxCurves;
		int MaxScales;
	};
	int MaxRecords;
	int NumCurves;
	int NumRecords;
	int szRecord;
	int szCurve;
	int szCurveHeader;
	BYTE* HC;//GHCURVES[NumRecords]
} GHISTORY;

//efine HCW(GH,r) ((GHCURVESHEADER*)(GH.HC+(r)*GH.szRecord))->Weight
#define HCR(GH,r) ((GHRECORD*)(GH.HC+(r)*GH.szRecord))
#define HCS(GH,r) ((GHSAMPLE*)(GH.HC+(r)*GH.szRecord+GH.szCurveHeader))

int SPG_CONV GH_Init(GHISTORY& GH, int MaxCurves, int MaxRecords);
void SPG_CONV GH_Close(GHISTORY& GH);
void SPG_CONV GH_Add(GHISTORY& GH, double* f, BYTE FlagOR=0, BYTE FlagAND=0xFF);
void SPG_CONV GH_Add(GHISTORY& GH, double* fmin, double* fmean, double* fmax, BYTE FlagOR=0, BYTE FlagAND=0xFF);

// ##############################            AFFICHAGE            ############################

typedef struct
{
	PixCoul Min;
	union
	{
		PixCoul Mean;
		PixCoul Text;
	};
	PixCoul Max;

	int ScaleGroup;//numero d'axe Y dans le tableau des GHCURVESCALEGROUP
	char Label[256];
	int LegendPosition;
	int DrawThick;

	double fMean;
	int nMean;
	int OffsetCompensation;

} GHCURVERENDEROPTS; //Options, numero d'axe Y, couleur, label pour une courbe

typedef enum
{
	GSC_AUTO,
	GSC_AUTOZERO,
	GSC_CUSTOM
} SCALEYMODE;


// ################################################################################################## //
//http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance

typedef struct
{
	double mean;
	double S;
	double sumweight;

	double fMin;
	double fMax;
//    Variance = S / (sumweight-1)  # if sample is the population, omit -1
} VAREST;
// ################################################################################################## //


typedef struct
{
	SCALEYMODE ScaleYMode; //0 = auto 1 = auto incl zero et limite basse pour ymax 2 = custom
	double LowerLimit;

	VAREST V;
	float k_min;//3 (empirique) pour rapport entre peak à peak et rms sur bruit gaussien
	float k_max;//3 (empirique) pour rapport entre peak à peak et rms sur bruit gaussien

	double fMin;
	double fMax;

	int NumDigits;

	//int Divisions;
	SPG_Scale SC;
} GHCURVESCALEGROUP; //definition d'un axe Y. Un seul axe Y (le premier) est calculé et dessiné, mais la structure supporte plusieurs axes Y et chaque courbe se rapporte à un axe Y indexe par ScaleGroup.

typedef struct
{
	int ScaleXMode; //0 = view all, 1 = zoom/pan, 2 = custom (GHW_ZoomPanXScale)
	int InhibitRescaleX;
	//int AutoTrigSamples;
	double AutoTrigSpan;
	int AutoTrigPixSpan;
	int Trigged;

	double  dt;
	double  Zoom;
	double  Pan;
	double  LR;

	double  xMin;
	double  xMax;
	double  xStep;
	//int Divisions;
	int RecordOriginOnScale;

	SPG_Scale SC;
} GHXRENDEROPTS; //definition de l'axe X. il n'y a qu'un axe X.

typedef struct
{
	int XGL;
	int XGR;
	int YGT;
	int YGB;
} GRENDERZONE; //zone de trace, laisse les marges pour le dessin des echelles

typedef struct
{
	int LT_GHW;
	GHISTORY* GH; //les donnees. Par convention la courbe zero se réfère à l'axe X
	GHCURVERENDEROPTS* ROC; //les options de dessin (une par courbe)
	GHCURVESCALEGROUP* SCG; //les echelles Y (on provisionne de quoi stocker une par courbe, mais en générale plusieurs courbes se réferent au même axe Y)
	GHXRENDEROPTS ROX;//rendu de l'axe X

	//GRENDERZONE GZ;
	GHRECORD* hcr[2];//stockage local de l'échantillon en cours de tracé (GHW_DrawCurves)
#ifdef SPG_General_USETimer
	S_TIMER T;
#endif
} GHISTORYWINDOW; //fenetre complete

int SPG_CONV GHW_Init(GHISTORYWINDOW& GHW, GHISTORY& GH, double AutoTrigSpan=30, int AutoTrigPixSpan=512);
#ifdef SPG_General_USECONFIGFILE
void SPG_CONV GHW_ReadScaleMode(GHISTORYWINDOW& GHW, SPG_CONFIGFILE& CFG, char* Prefix=0);
#endif
void SPG_CONV GHW_Close(GHISTORYWINDOW& GHW);
void SPG_CONV GHW_Print(G_Ecran& E, int X,int Y, char* Msg, C_Lib& CL,int Alignement, DWORD Couleur);

void SPG_CONV GHW_ZoomPanXScale(GHISTORYWINDOW& GHW, bool HasFocus);
int SPG_CONV GHW_ComputeScales(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL, bool HasFocus);
int SPG_CONV GHW_RenderXScale(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL);
int SPG_CONV GHW_RenderYScale(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL, int c);
int SPG_CONV GHW_RenderCurves(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL);
int SPG_CONV GHW_Render(GHISTORYWINDOW& GHW, G_Ecran& E, C_Lib* CL, bool HasFocus);

#define GHWX(GZ,ROX,f) (GZ.XGL)+(GZ.XGR-GZ.XGL)*(f-ROX.xMin)/(ROX.xMax-ROX.xMin)
#define GHWY(GZ,SCG,f) (GZ.YGT)+(GZ.YGB-GZ.YGT)*(SCG.SC.fMax-f)/(SCG.SC.fMax-SCG.SC.fMin)
#define GHWYR(GZ,SCG,f) DY*(SCG.SC.fMax-f)*invfY

//int SPG_CONV GHW_UpdateButtons(GHISTORYWINDOW& GHW, int MouseX, int MouseY, int MouseLeft);


