
#define DF_VERSION 5

typedef BYTE SPG_CAMPIXEL;

typedef struct
{
	float S;
	float DWX;//(1/(ecart type²)) * 1/gradX
#ifndef DF_FASTEST
	float DWD;
#endif
	float DWY;
#ifndef DF_FASTEST
	float DWE;
#endif
} SPG_PIXGRADINFO;

typedef struct
{
	SPG_PIXGRADINFO* restrict GradRef;
	//parametres reference pour calcul position (computegradinfo)
	float SWX;//ecart type selon X,D,Y,E sigmaX²=1/somme(1/(ecart type²))
#ifndef DF_FASTEST
	float SWD;
#endif
	float SWY;
#ifndef DF_FASTEST
	float SWE;
#endif
#if(DF_VERSION<5)
	float SWXX;
	float SWYY;
#ifndef DF_FASTEST
	float SWDE;
	float SWXD;
	float SWXE;
	float SWYD;
	float SWYE;
#endif
#endif
	//parametre reference pour correction contraste (computegradinfo)
	float FirstOrder;
	float SecondOrder;
} SPG_GRADINFO;

typedef struct
{
	//SPG_PIXGRADINFO* restrict GradRef;
	//parametres reference pour calcul position (computegradinfo)
	float SWX;
#ifndef DF_FASTEST
	float SWD;
#endif
	float SWY;
#ifndef DF_FASTEST
	float SWE;
#endif
#if(DF_VERSION<5)
	float SWXX;
	float SWYY;
#ifndef DF_FASTEST
	float SWDE;
	float SWXD;
	float SWXE;
	float SWYD;
	float SWYE;
#endif
#endif
	//parametre reference pour correction contraste (computegradinfo)
	//float FirstOrder;
	//float SecondOrder;
} SPG_GRADINFOIMMED;

typedef struct
{
	int SuperResolution;
	SPG_WB WB;
	float PosX;
	float PosY;
	float SpeedIntegrationThreshold;
} SPG_DF_SUPERRES;

typedef struct
{			   
	int Etat;
	int SizeX;
	int SizeY;
#ifdef DF_PREFILTER
	float* PreFilterIn;//buffer temporaire
	float* PreFilterOut;//buffer temporaire
#endif
	float* restrict Reference;
	float* restrict Current;

#if(DF_VERSION==3)
	SPG_PIXINT_RELAX PX;
#elif(DF_VERSION>=4)
	SPG_PIXINT* PX;
	float* restrict DX_Kernel;
#ifndef DF_FASTEST
	float* restrict DD_Kernel; //calcule dynamiquement	(DX+DY)*INVSQRT2
#endif
	float* restrict DY_Kernel;
#ifndef DF_FASTEST
	float* restrict DE_Kernel; //calcule dynamiquement (DY-DX)*INVSQRT2
#endif
	float* restrict Interpole_Kernel;//contenu variable utilisé dans move reference to current
#endif

	int ReferenceIntegration;
	int ReferenceIntegrationCount;

	//parametres
#ifdef DF_PREFILTER
	int PreFilterSize;//bande passante du passe haut 0=non filtre
#endif
	float NoiseLevel;//niveau de bruit (ponderation des gradients, seuil des vecteurs)
	float CorrelationThreshold;//0=pas de limite de décorelation 1=pas de décorrelation
	int InterpolationBorderSize;

	SPG_GRADINFO GRInfo;

	SPG_DF_SUPERRES DS;

	//matrice de corrélation/etalonnage
	float UxP;
	float UyP;
	float UxN;
	float UyN;
	float VxP;
	float VyP;
	float VxN;
	float VyN;

} SPG_DISPFIELD;

#define SPG_DF_VectorThreshold(DF,Seuil) (DF.GRInfo.SWX+DF.GRInfo.SWD+DF.GRInfo.SWY+DF.GRInfo.SWE>Seuil*DF.NoiseLevel)
//#define SPG_DF_VectorThreshold(DF) 1

#if(DF_VERSION==3)
#ifdef DF_PREFILTER
int SPG_CONV SPG_DF_Init(SPG_DISPFIELD& DF, int SizeX, int SizeY, float* InterpoleDeltaTmp, float NoiseLevel=1.0f, float CorrelationThreshold=0.4f, int PreFilterSize=8);
#else
int SPG_CONV SPG_DF_Init(SPG_DISPFIELD& DF, int SizeX, int SizeY, float* InterpoleDeltaTmp, float NoiseLevel=1.0f, float CorrelationThreshold=0.4f, int ReferenceIntegration=64);
#endif
#elif(DF_VERSION>=4)
int SPG_CONV SPG_DF_Init(SPG_DISPFIELD& DF, SPG_PIXINT& PX, int SizeX, int SizeY, float NoiseLevel=1.0f, float CorrelationThreshold=0.4f, int ReferenceIntegration=64, int SuperResolution=0);
#endif
void SPG_CONV SPG_DF_Close(SPG_DISPFIELD& DF);
//void SPG_CONV SPG_DF_ComputeWeight(float& DW, float& W, float& D, float SigmaD, float SigmaT);
#if(DF_VERSION==5)
void SPG_CONV SPG_DF_Reduction(float& PosX, float& PosY, float X, float WX, float D, float WD, float Y, float WY, float E, float WE);
#endif
#if(DF_VERSION==3)
void SPG_CONV SPG_DF_ComputeIntensity(SPG_DISPFIELD& DF);
#endif
void SPG_CONV SPG_DF_ComputeGradInfo(SPG_DISPFIELD& DF, SPG_GRADINFO& GRInfo, float* restrict Reference);
#if(DF_VERSION>=4)
void SPG_CONV SPG_DF_ComputePositionImmed(SPG_DISPFIELD& DF, float& PosX, float& PosY, float* restrict Reference, int RefPitch, float* restrict Current, int CurPitch);
#endif
void SPG_CONV SPG_DF_Decorrelle(SPG_DISPFIELD& DF, float& PosX, float& PosY, float a, float b);
void SPG_CONV SPG_DF_ComputePosition(SPG_DISPFIELD& DF, float& PosX, float& PosY, int UpdateSuperRes=0);
void SPG_CONV SPG_DF_MoveReferenceToCurrent(SPG_DISPFIELD& DF, int dX, int dY);
void SPG_CONV SPG_DF_Calibrate(SPG_DISPFIELD& DF);
#ifdef DF_PREFILTER
void SPG_CONV SPG_DF_HighPass(float* Dst, float* Src, int SrcPitch, int SizeX, int SizeY, int Width=8);
#endif

void SPG_CONV SPG_DF_LightControl(float& Mul, float& Add, float* restrict Reference, int PitchRef, int SizeX, int SizeY, float* restrict Current, int PitchCur=0);

int SPG_CONV SPG_DF_UpdateReferenceWithCurrent(SPG_DISPFIELD& DF);

//SPG_DispFieldByte.cpp
void SPG_CONV SPG_DF_SetByteFrame(SPG_DISPFIELD& DF, float* Dst, SPG_CAMPIXEL* E, int Pitch=0);
void SPG_CONV SPG_DF_UpdateByteFrame(SPG_DISPFIELD& DF, float* Dst, SPG_CAMPIXEL* E, int Pitch, float TimeCste_0_1);
void SPG_CONV SPG_DF_SetByteLightFrame(SPG_DISPFIELD& DF, float* restrict Dst, SPG_CAMPIXEL* restrict E, int Pitch, float Mul, float Add);
void SPG_CONV SPG_DF_UpdateByteLightFrame(SPG_DISPFIELD& DF, float* restrict Dst, SPG_CAMPIXEL const* restrict E, const int Pitch, const float TimeCste_0_1, const float Mul, const float Add, const float NonLinearMaxAmplitude);

void SPG_CONV SPG_DF_SetCurrentByteFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* E, int Pitch=0);
void SPG_CONV SPG_DF_UpdateCurrentByteFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* E, int Pitch, float TimeCste_0_1);
void SPG_CONV SPG_DF_SetCurrentByteLightFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* restrict E, int Pitch);
void SPG_CONV SPG_DF_UpdateCurrentByteLightFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* restrict E, int Pitch, float TimeCste_0_1, float NonLinearMaxAmplitude=0);
void SPG_CONV SPG_DF_SetReferenceByteFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* E, int Pitch=0);

void SPG_CONV SPG_DF_SetFloatFrame(SPG_DISPFIELD& DF, float* Dst, float* Src, int Pitch=0);
void SPG_CONV SPG_DF_UpdateFloatFrame(SPG_DISPFIELD& DF, float* Dst, float* Src, int Pitch, float TimeCste_0_1);
void SPG_CONV SPG_DF_SetFloatLightFrame(SPG_DISPFIELD& DF, float* Dst, float* Src, int Pitch, float Mul, float Add);
void SPG_CONV SPG_DF_UpdateFloatLightFrame(SPG_DISPFIELD& DF, float* Dst, float* Src, int Pitch, float TimeCste_0_1, const float Mul, const float Add, const float NonLinearMaxAmplitude);

//SPG_DispFieldFloat.cpp
void SPG_CONV SPG_DF_SetCurrentFloatFrame(SPG_DISPFIELD& DF, float* E, int Pitch=0);
void SPG_CONV SPG_DF_UpdateCurrentFloatFrame(SPG_DISPFIELD& DF, float* E, int Pitch, float TimeCste_0_1);
void SPG_CONV SPG_DF_SetCurrentFloatLightFrame(SPG_DISPFIELD& DF, float* E, int Pitch);
void SPG_CONV SPG_DF_UpdateCurrentFloatLightFrame(SPG_DISPFIELD& DF, float* E, int Pitch, float TimeCste_0_1, float NonLinearMaxAmplitude=0);
void SPG_CONV SPG_DF_SetReferenceFloatFrame(SPG_DISPFIELD& DF, float* E, int Pitch=0);

