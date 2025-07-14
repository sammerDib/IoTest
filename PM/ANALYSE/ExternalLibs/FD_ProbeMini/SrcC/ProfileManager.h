
#ifdef SPG_General_USEProfil

#define MaxUnit 32

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	float XScale;//pas sur X
	float YScale;//pas sur Y
	char UnitX[MaxUnit];//unite de l'echelle X
	char UnitY[MaxUnit];//unite de l'echelle Y
	char UnitZ[MaxUnit];//unite de l'echelle Z
} ProfilHeader;

typedef struct
{
	ProfilHeader H;
	float* restrict D;
	BYTE* restrict Msk;//facultatif
} Profil;

typedef struct
{
	ProfilHeader H;
	BYTE* restrict D;
	BYTE* restrict Msk;
} Profil8;

#ifdef SPG_General_USEFFT
typedef struct
{
	ProfilHeader H;
	SPG_COMPLEX* restrict D;
	BYTE* restrict Msk;
} ProfilX;
#else
#define ProfilX void*
#endif

#define P_SizeX(P) P.H.SizeX
#define P_SizeY(P) P.H.SizeY
#define P_Size(P) P.H.SizeX*P.H.SizeY
#define P_UnitX(P) P.H.UnitX
#define P_UnitY(P) P.H.UnitY
#define P_UnitZ(P) P.H.UnitZ
#define P_XScale(P) P.H.XScale
#define P_YScale(P) P.H.YScale
#define P_Etat(P) P.H.Etat
#define P_Data(P) P.D
#define P_Msk(P) P.Msk
#define P_Header(P) P.H
#define P8_Data(P) P_Data(P)
#define P8_Etat(P) P_Etat(P)
#define P8_SizeX(P) P_SizeX(P)
#define P8_SizeY(P) P_SizeY(P)
#define PX_SizeX(P) P.H.SizeX
#define PX_SizeY(P) P.H.SizeY
#define PX_UnitX(P) P.H.UnitX
#define PX_UnitY(P) P.H.UnitY
#define PX_UnitZ(P) P.H.UnitZ
#define PX_XScale(P) P.H.XScale
#define PX_YScale(P) P.H.YScale
#define PX_Etat(P) P.H.Etat
#define PX_Data(P) P.D
#define PX_Msk(P) P.Msk
#define PX_Header(P) P.H

#define P_IsInProfil(P,x,y) (V_IsBound(x,0,P.H.SizeX)&&V_IsBound(y,0,P.H.SizeY))
#define P_Element(P,x,y) P.D[x+P.H.SizeX*(y)]
#define P_ElementSafe(P,x,y) P_Element(P,(V_Sature((x),0,(P.H.SizeX-1))),(V_Sature((y),0,(P.H.SizeY-1))))
#define P_ElementPtr(P,x,y) (P.D+x+P.H.SizeX*(y))
#define P_ElementPtr_Safe(P,x,y) P_ElementPtr(P,(V_Sature((x),0,(P.H.SizeX-1))),(V_Sature((y),0,(P.H.SizeY-1))))
#define PX_Element(P,x,y) P.D[x+P.H.SizeX*(y)]
#define P_Pointeur(P,x,y) (P.D+x+P.H.SizeX*(y))
#define PX_Pointeur(P,x,y) (P.D+x+P.H.SizeX*(y))
#define P_ElementMsk(P,x,y) P.Msk[x+P.H.SizeX*(y)]
#define P_Clear(P) if(P_Data(P)) memset(P_Data(P),0,P_SizeX(P)*P_SizeY(P)*sizeof(float))
#define PX_Clear(PX) if(PX.D) memset(PX_Data(PX),0,PX_SizeX(PX)*PX_SizeY(PX)*sizeof(SPG_COMPLEX))
#define P_ClearMsk(P,v) if(P_Msk(P)) memset(P_Msk(P),v,P_SizeX(P)*P_SizeY(P)*sizeof(BYTE))
#define P_Duplicate(Pdest,Psrc) P_Dupliquate(Pdest,Psrc)
#define P_DuplicateWithNoMsk(Pdest,Psrc) P_DupliquateWithNoMsk(Pdest,Psrc)
#define P_DuplicateWithMsk(Pdest,Psrc) P_DupliquateWithMsk(Pdest,Psrc)
#define P_ForAll(P,i,instruction) CHECK_ELSE(P_Etat(P)==0,"P_ForAll",;) else {for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++){instruction;}}

//indique qu'on doit travailler avec les bitmaps de bas en haut
#define P_ManagerRevBMP

//Flags d'etat
#define P_Null 0
#define P_WithMEM 1
#define P_WithMem 1
#define P_Alias 2
#define P_AliasMem 2
#define P_MemAlias 2
#define P_WithThisMEM 4
#define P_WithThisMem 4
#define P_COMPLEX 8
#define P_BYTE 16

#define FileP_WithMask 1
#define FileP_Sign "SPGP"

#define P_TRANSPOSE 0
#define P_NOTRANSPOSE 1

#include "ProfileManager.agh"
#include "ProfileManagerFileIO.agh"

#define P_Normalise(PRes,PDiviseur) P_Normalize(PRes,PDiviseur)
#define P_UnwrapGeneric(F,StepCount,LastZ) {StepCount+=V_Round(F-LastZ);LastZ=F;F-=StepCount;}

#endif

