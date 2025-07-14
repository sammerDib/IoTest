
#define PIXINT_RELAX_OVERSAMPLING 3

typedef struct
{
	float v[3][3];
} PIXINT_RELAX_Kernel;

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	//int Downsampling;
	//float* PixBrut;
	float* PixInterpole;
	float* PixInterpoleDelta;
	PIXINT_RELAX_Kernel* Sonde;
	PIXINT_RELAX_Kernel* Matrix;
} SPG_PIXINT_RELAX;

typedef struct
{
	int Etat;
	int KernelSize;//dimension de la matrice GetInterpolationKernel
	int PixSize;//nbre de pixels lateraux (inclus effet de bord)
	int Oversampling;//nbre d'ech supplementaires de part et d'autre du centre de chaque pixel
	int CenterIndex;//centre du kernel
	int Size;//taille X=Y du kernel
	int PixelCount;//surface active d'un pixel
	BYTE* restrict CCDMask;
	float* restrict Kernel;//=P_Data(PX.P[PX.CurrentProfil])
	int CurrentProfil;
	Profil P[2];
	Profil PSmoothKernel;
} SPG_PIXINT;

#define PX_OK 1

#define PX_MEMALLOC 1
#define PX_GEOMETRY 2
#define PX_FILTER 4

//#define PIXINT_GetFloatTmpSize(PX) PX.SizeX*PX.Downsampling*PX.SizeY*PX.Downsampling
#define PIXINT_RELAX_GetTempSize(SizeX,SizeY) SizeX*SizeY*PIXINT_RELAX_OVERSAMPLING*PIXINT_RELAX_OVERSAMPLING*2

int SPG_CONV PIXINT_RELAX_Init(SPG_PIXINT_RELAX& PX, int SizeX, int SizeY, float* InterpoleTemp);
void SPG_CONV PIXINT_RELAX_Close(SPG_PIXINT_RELAX& PX);
void SPG_CONV PIXINT_RELAX_Interpole(SPG_PIXINT_RELAX& PX, float* PixBrut, int Iterations=12);

#define PIXINT_Create_R9(PX) SPG_PIXINT PX;PIXINT_Init(PX,2,4);PIXINT_Compute(PX,1,1,6,1.4f);
#define PIXINT_Create_RL9(PX) SPG_PIXINT PX;PIXINT_Init(PX,2,4);PIXINT_ComputeBilinear(PX);
#define PIXINT_Round(v) if((v<1e-5f)&&(v>-1e-5f)) v=0;

int SPG_CONV PIXINT_Init(SPG_PIXINT& PX, int Size=2, int Oversampling=4);
void SPG_CONV PIXINT_Close(SPG_PIXINT& PX);
void SPG_CONV PIXINT_Compute(SPG_PIXINT& PX, int DeadZoneX=1, int DeadZoneY=1, int NIter=6, float Lambda=1.4f);
void SPG_CONV PIXINT_ComputeBilinear(SPG_PIXINT& PX);
void SPG_CONV PIXINT_ComputeCosinus(SPG_PIXINT& PX);
void SPG_CONV PIXINT_Save(SPG_PIXINT& PX);
void SPG_CONV PIXINT_SetCCDGeometry(SPG_PIXINT& PX, int DeadZoneX=1, int DeadZoneY=1);
void SPG_CONV PIXINT_UseCCDGeometry(SPG_PIXINT& PX, G_Ecran& Edest, G_Ecran& Esrc);
void SPG_CONV PIXINT_UseCCDGeometry(SPG_PIXINT& PX, float* Dest, int DestSizeX, int DestSizeY, float* Src, int SrcSizeX, int SrcSizeY);
void SPG_CONV PIXINT_IntegreKernel(SPG_PIXINT& PX, const float Lambda=1.25f);
void SPG_CONV PIXINT_Smooth(SPG_PIXINT& PX);
void SPG_CONV PIXINT_SelectiveSmooth(SPG_PIXINT& PX);
void SPG_CONV PIXINT_Normalize(SPG_PIXINT& PX);
void SPG_CONV PIXINT_GetInterpolationKernel(SPG_PIXINT& PX, int PosX, int PosY, float* Kernel);
void SPG_CONV PIXINT_AddInterpolationKernel(SPG_PIXINT& PX, int PosX, int PosY, float* Kernel, float Coeff=1.0f);
void SPG_CONV PIXINT_GetDXKernel(SPG_PIXINT& PX, float* Kernel);
void SPG_CONV PIXINT_GetDYKernel(SPG_PIXINT& PX, float* Kernel);
void SPG_CONV PIXINT_Interpole(SPG_PIXINT& PX, float* Dest, int DestSizeX, int DestSizeY, float* Src, int SrcSizeX, int SrcSizeY);


