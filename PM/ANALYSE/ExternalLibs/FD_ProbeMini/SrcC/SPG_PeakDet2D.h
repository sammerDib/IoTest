#ifdef INC_SPG_PeakDet2D_INC
#error SPG_PeakDet2D Included twice
#endif
#define	INC_SPG_PeakDet2D_INC

//#define DebugPeakDet2D
//#define DebugRGBCorrel

typedef struct 
{
	int Etat;// 0=invalide	1=resultat int	2=resultat float
	//int DbgAbscisse;
	int PosInt;//position Y donnée par la première détection
	float IntensityInt;
	int WidthInt;
	float Position;//position donnée par le fit
	float Intensity;//intensité de la ligne (sommet de la parabole)
	float Width;
	int FitSize;
	int fit_y;//position Y du calcul de la parabole
	float fit_a;//parametres de la parabole
	float fit_b;
	float fit_c;
	//float x;
} SPG_PEAKPOSITION;

#define SPG_PEAKPOSITIONINVALID 0
#define SPG_PEAKPOSITIONINT 1
#define SPG_PEAKPOSITIONFLOAT 2
/*
typedef struct
{
	//int MaxResult=SPG_PEAK_MAXROWRESULT;
	//int NumResult;
	SPG_PEAKPOSITION PositionList[SPG_PEAK_MAXROWRESULT];
} SPG_PEAKROWRESULT;
*/

#ifdef DebugTimer
#define D2DT_Total					D2D.T[0]
#define D2DT_Update					D2D.T[1]
#define D2DT_UpdateRGB				D2D.T[2]
#define D2DT_GetLine				D2D.T[3]
#define D2DT_ComputeSource			D2D.T[4]
#define D2DT_ComputeReference		D2D.T[5]
#define D2DT_ComputeLevels			D2D.T[6]
#define D2DT_Regularise				D2D.T[7]
#define D2DT_ComputeWeight			D2D.T[8]
#define D2DT_FilterWeight			D2D.T[9]
#define D2DT_FindYMaxAndErase		D2D.T[10]
#define D2DT_GetYList				D2D.T[11]
#define D2DT_RefineY				D2D.T[12]
#define D2DT_DrawResultRGB			D2D.T[13]
#define D2DT_RGB24toFloat			D2D.T[14]
#define D2DT_RGB24toFloatAndRotate	D2D.T[15]
#define D2DT_MAX					16
#endif

typedef struct
{
	int Etat;

	int SizeX;
	int SizeY;
	int SizeT;//sizex*sizey

	int Reset;
	int Fast;
	int RegularisedRefine;
	bool Rotate;

	int SpatialFilterKernelSize;

	int Etape;//juste pour le débogage, s'assurer qu'on effectue les étapes du calcul dans l'ordre et sans en oublier
	int SideEffect;//taille des effets de bord, augmenté comme il se doit chaque fois qu'une opération est effectuée

	float LowPassTimeCste;//moyennage temporel des données brutes

	//float LowTimeCste;//temps de réponse des calcul de niveau de bruit
	//float PeakTimeCste;//temps de réponse des calculs de niveau de signal

	Profil Signal;//signal converti depuis l'image RGB (rotation à 90° en option,
	//car l'algo ne traite que les lignes horizontales pour l'instant)

	Profil Average;//profil apres passe-bas temporel

	Profil* Source;//etape 1, source pointe soit sur les données originales 
	//recue en paramètre, soit sur Average si on a selectionné un filtrage passe bas
	//PeakDet2D_ComputeAverage

	int RegulariseKernelSize;

	float PowLow;// puissance X
	Profil ReferencePL;//reference puissance X<1 //etape 2 
	Profil NoiseLowPL;//noise low puissance X<1 //etape 3 

	float PowHigh;// puissance X
	Profil ReferencePH;//reference puissance X>2 //etape 2 
	Profil SignalHighPH;//signal high puissance X>2 //etape 3 

	Profil NoiseLow;//niveau bas du bruit (minorant du bruit) //etape 4
	//Profil NoiseHigh;//niveau haut du bruit (majorant du bruit non utilisé en 2D)
	//Profil SignalLow;//=PEAKDET_SIGNAL(D2D) //seuil de signal utile (minorant du signal utile, non utilisé en 2D)
	Profil SignalHigh;//sommet du signal (majorant du signal utile)

	float MinSignalLevel;

	Profil Weight;//resultat normalisé //etape 5 PeakDet2D_ComputeResult


	int WeightFilterMode;
	int WeightFilterKernelSize;
	float WeightFilterAngle;
	Profil Kernel;

	Profil WeightApriori;//resultat tenant compte de la forme a priori des motifs cherchés
	//etape 6 PeakDet2D_ComputeApriori
	Profil WeightAprioriErase;//resultat tenant compte de la forme a priori des motifs cherchés

	int MaxResultPerRow;
	SPG_PEAKPOSITION* Result;//size=MaxResultPerRow * D2D.SizeX

	float Selectivity;
	float Threshold;

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_TIMER T[D2DT_MAX];
#endif
#endif

#ifdef DebugPeakDet2D
	SPG_Console C;
#endif
} SPG_PEAKDET2D;		 

#define PEAKDET_OK 1
//#define PEAKDET_ON 2
//#define PEAKDET_SNR 3


//#define PEAKDET_SIGNAL(D2D) D2D.NoiseHigh+(D2D.NoiseHigh-D2D.NoiseLow)/D2D.PeakTimeCste
//#define PEAKDET2D_SIGNALLOW(D2D,i) ((1+PEAKDET_SNR)*P_Data(D2D.NoiseHigh)[i]-PEAKDET_SNR*P_Data(D2D.NoiseLow)[i])
//#define PEAKDET2D_SIGNALHIGH(D2D,i) ((1+2*PEAKDET_SNR)*P_Data(D2D.NoiseHigh)[i]-2*PEAKDET_SNR*P_Data(D2D.NoiseLow)[i])
//#define PEAKDET2D_ISSIGNAL(D2D,V,i) (P_Data(V)[i]>P_Data(D2D.SignalLow)[i])
//#define PEAKDET2D_WEIGHT(D2D,V,i) (P_Data(V)[i]-P_Data(D2D.SignalLow)[i])/(P_Data(D2D.SignalHigh)[i]-P_Data(D2D.SignalLow)[i])

/* fonctions de l'interface */
void SPG_CONV D2DLoadFromCFG(PEAKDET2DPARAMS& D2DParams, SPG_CONFIGFILE& CFG);
int SPG_CONV PeakDet2D_Init(SPG_PEAKDET2D& D2D, PEAKDET2DPARAMS& Params, int SizeOfParams);//int SizeX, int SizeY, int Rotate, int SpatialFilterKernelSize, float LowPassTimeCste, float PowLow, float PowHigh, float MinSignalLevel, int RegulariseKernelSize, int WeightFilterKernelSize, float WeightFilterAngle, int MaxResultPerRow, int RegularizedRefine, float Selectivity, float Threshold);
void SPG_CONV PeakDet2D_Close(SPG_PEAKDET2D& D2D);
//si Reset=0 dans ce cas on a les modes suivant
//Fast=0: mode normal (mise à jour des niveaux, filtrage spatial)
//Fast=1: mode 'preview' (pas de mise à jour des niveaux, normalistation et filtrage spatial)
//Fast=2: mode 'raw' (pas de mise à jour des niveaux, pas de normalisation, pas de filtrage spatial)
//si Reset=1: 
//recalcule de zero les niveaux de signal (lent mais nécessaire à la première image d'une séquence)
//Fast est ignoré
int SPG_CONV PeakDet2D_Update(SPG_PEAKDET2D& D2D, Profil& Signal, int Reset=1, int Fast=0);
int SPG_CONV PeakDet2D_UpdateRGB(SPG_PEAKDET2D& D2D, BYTE* srcRGB24, int Pitch, int POCT, int Reset=1, int Fast=0);

/* fonctions internes */
int SPG_CONV PeakDet2D_ComputeSource(SPG_PEAKDET2D& D2D, Profil& Current);
int SPG_CONV PeakDet2D_ComputeReference(SPG_PEAKDET2D& D2D);
int SPG_CONV PeakDet2D_ComputeLevels(SPG_PEAKDET2D& D2D);
int SPG_CONV PeakDet2D_Regularise(SPG_PEAKDET2D& D2D, int RegulariseKernelSize, int Iter);
int SPG_CONV PeakDet2D_ComputeWeight(SPG_PEAKDET2D& D2D);
int SPG_CONV PeakDet2D_InitKernelFilterWeight(Profil& Kernel, int KernelSize, float Angle);
//int SPG_CONV PeakDet2D_FindYMaxAndErase(SPG_PEAKDET2D& D2D, Profil& PW, int x, int& yFound, int& yWidth);
int SPG_CONV PeakDet2D_GetYList(SPG_PEAKDET2D& D2D,Profil& WErase,int x,SPG_PEAKPOSITION* Result);
int SPG_CONV PeakDet2D_RefineY(SPG_PEAKDET2D& D2D, Profil& W, int x, SPG_PEAKPOSITION& Result);
//int SPG_CONV PeakDet2D_GetLine(SPG_PEAKDET2D& D2D, float* Y, BYTE* Yvalid, float* YConsigne, float* YMin, float* YMax, BYTE* YConsigneValid, float Selectivity);
int SPG_CONV PeakDet2D_GetLine(SPG_PEAKDET2D& D2D, float* Y, BYTE* Yvalid=0, float* YConsigne=0, float* YMin=0, float* YMax=0, BYTE* YConsigneValid=0, SPG_PEAKPOSITION* YParameters=0);

int SPG_CONV PeakDet2D_DrawResultRGB(SPG_PEAKDET2D& D2D, BYTE* dstRGB24, int Pitch, int POCT, int DisplayType);
void SPG_CONV PeakDet2D_DrawLaserParameters(G_Ecran& EPair,PixCoul& Color,SPG_PEAKPOSITION* YParameters,int NumY);

void RGB24toFloat(SPG_PEAKDET2D& D2D, Profil& Pdst, BYTE* srcRGB24, int SizeX, int SizeY, int PitchSrc, int POCT, bool FilterInPlace=false);
void RGB24toFloatAndRotate(SPG_PEAKDET2D& D2D, float* dst, BYTE* srcRGB24, int SizeX, int SizeY, int PitchSrc, int POCT);
