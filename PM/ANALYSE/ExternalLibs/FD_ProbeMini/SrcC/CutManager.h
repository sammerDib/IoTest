
#ifdef SPG_General_USECut

#define MaxUnit 32

typedef struct
{
	int Etat;
	float* D; //donnees
	BYTE* Msk; //masque (facultatif) (donnees invalides Msk[i]!=0)
	BYTE* Decor; //Decorations (facultatif)
	int NumS;   //nombre de samples
	float XScale; //pas sur X
	char UnitX[MaxUnit]; //unite de l'echelle X
	char UnitY[MaxUnit]; //unite de l'echelle Y
} Cut;

#ifdef SPG_General_USEFFT
typedef struct
{
	int Etat;
	SPG_COMPLEX* D;//donnees
	BYTE* Msk;   //masque (donnees invalides Msk!=0)
	BYTE* Decor;//Decorations
	int NumS;  //nombre de samples
	float XScale; //pas sur X
	char UnitX[MaxUnit]; //unite de l'echelle X
	char UnitY[MaxUnit]; //unite de l'echelle Y
} CutX;
#else
#define CutX void*
#endif

//Flags d'etat
#define Cut_Null 0
//alloue et desalloue
#define Cut_WithMEM 1
//n'alloue pas et ne desalloue pas
#define Cut_Alias 2
//desalloue mais n'alloue pas
#define Cut_WithThisMEM 4
#define Cut_COMPLEX 8

#define Cut_CopyMsk 1
#define Cut_CopyDecor 2

#define FileC_Sign "SPGP"

//Separateurs
#define S_Tabulation 0
#define S_Virgule 1
#define S_RetourChariot 2

#include "CutManager.agh"
#include "CutManagerFileIO.agh"

#define Cut_ScrollLeft(C) if(C.Etat) memmove(C.D,C.D+1,(C.NumS-1)*sizeof(float))
#ifdef SPG_General_USEFFT
#define CutX_ScrollLeft(C) if(C.Etat) memmove(C.D,C.D+1,(C.NumS-1)*sizeof(SPG_COMPLEX))
#define CutX_Clear(C) if(C.D) memset(C.D,0,C.NumS*sizeof(SPG_COMPLEX))
#endif
#define Cut_Clear(C) if(C.D) memset(C.D,0,C.NumS*sizeof(float))
#define Cut_ClearMsk(C) if(C.Msk) memset(C.Msk,0,C.NumS)
#define Cut_ClearDecor(C) if(C.Decor) memset(C.Decor,0,C.NumS)
#define Cut_CopyScale(CDest,CSrc) (CDest).XScale=(CSrc).XScale;strcpy((CDest).UnitX,(CSrc).UnitX);strcpy((CDest).UnitY,(CSrc).UnitY);
#define Cut_Normalise(CRes,CDiviseur) Cut_Normalize(CRes,CDiviseur)
#define Cut_UnwrapGeneric(F,StepCount,LastZ) {StepCount+=V_Round(F-LastZ);LastZ=F;F-=StepCount;}
#define Cut_ConvolveSample(C, Sample, Filtre) {float Sum=0;for(int n=0;n<Filtre.NumS;n++){Sum+=C.D[Sample+n]*Filtre.D[n];}C.D[Sample]=Sum;}
#define Cut_Etat(C) C.Etat
#define Cut_NumS(C) C.NumS
#define Cut_Data(C) C.D
#define Cut_Msk(C) C.Msk
#define Cut_Decor(C) C.Decor
#define Cut_XScale(C) C.XScale
#define Cut_UnitX(C) C.UnitX
#define Cut_UnitY(C) C.UnitY

#endif


