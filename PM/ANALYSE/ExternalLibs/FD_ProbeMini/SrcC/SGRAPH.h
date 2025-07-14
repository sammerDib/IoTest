
#ifdef SPG_General_USESGRAPH

#undef FORCEBILINEAR
#define SG_MAX_LIGHT 256
//#define SG_MAX_TRI 65536
#define SG_MAX_BLOC 2048
#define SG_MAX_TEX 512

#define SG_TriCatNum 65536
#define SG_TriCatType WORD
//#define SG_TriMax 65536
#ifdef SGE_EMC
#define SGE_TrieBlocs
#endif

#define REM_DEBILUS_BROUILLARDUS

#ifndef REM_DEBILUS_BROUILLARDUS
#define SG_MAX_FOG 8
#endif

typedef V_VECT SG_VECT;

typedef G_PixCoord SG_PixCoord;

typedef SG_PixCoord SG_TexCoord;

typedef WORD SG_INDEX;

//Type PNT3D
typedef struct
{
SG_VECT P;
#ifndef SPG_General_PGLib
#ifdef SGE_EMC
float Prof;
union
{
SG_PixCoord PECR;
struct
{
	SHORT XECR;
	SHORT YECR;
};
};
#else
LONG Prof;
SHORT XECR;
SHORT YECR;
#endif
#endif
} SG_PNT3D;

//Type FACE
typedef struct
{
union 
{
	struct
	{
SG_PNT3D* restrict NumP1;
SG_PNT3D* restrict NumP2;
SG_PNT3D* restrict NumP3;
SG_PNT3D* restrict NumP4;
	};
SG_PNT3D* restrict NumP[4];
};
union
{
	struct
	{
SHORT XT1;
SHORT YT1;
SHORT XT2;
SHORT YT2;
SHORT XT3;
SHORT YT3;
SHORT XT4;
SHORT YT4;
	};
	SG_TexCoord T[4];
};
/*
union
{
DWORD Couleur;

	struct
	{
	BYTE B;
	BYTE V;
	BYTE R;
	BYTE A;
	};
};
*/
PixCoul Couleur;

#ifdef SGE_EMC
	V_VECT Normale;
#endif

BYTE Style;
BYTE TriState;
#ifndef SPG_General_PGLib
SHORT NumTex;
#else
PGLTexture* Texture;
#endif
} SG_FACE;

//#define LenFACE 40
/*
asm:
0uni normal        /8texture normal
1uni normal        /9texture bilinear T
2uni translucide~T /10texture translucide~T
3uni transparent~T /11texture transparent~T
4uni rien          /12texture perce
5..7 rien          /13..15 rien
*/


//styles de faces
#ifdef SGE_EMC
#define SG_NOTRACE 0
#define SG_RENDER_NORMAL 1
#define SG_RENDER_TRANSL 2
//A FINIR
#define SG_RENDER_TRANSP 4
#define SG_RENDER_TEX 8
#define SG_MASKUNI 7
#define SG_RENDER_TEXMSK 16
#define SG_RENDER_FIL 32
#define SG_RENDER_NOBACKSIDE 64
//A FINIR
//#define SG_RENDER_TEXPERCE 4


#define SG_UNI 1
#define SG_UNITRANSL 2
#define SG_UNITRANSP 4
#define SG_TEX 9
#define SG_TEXTRANSL 10
#define SG_TEXTRANSP 12
#else
#define SG_UNI 0
#define SG_UNITRANSL 2
#define SG_UNITRANSP 3
#ifdef SG_FORCEBILINEAR
#define SG_TEX 9
#else
#define SG_TEX 8
#endif
#define SG_TEXBIL 9
#define SG_TEXTRANSL 10
#define SG_TEXTRANSP 11
#define SG_TEXPERCE 12

#define SG_MASKUNI 7
#define SG_NOTRACE 32
#define SG_RENDER_NOBACKSIDE 0
#endif

#ifndef SGE_EMC
//Type FACEUnique
typedef struct
{
SHORT X1;
SHORT Y1;
SHORT X2;
SHORT Y2;
SHORT X3;
SHORT Y3;
SHORT X4;
SHORT Y4;
SHORT XT1;
SHORT YT1;
SHORT XT2;
SHORT YT2;
SHORT XT3;
SHORT YT3;
SHORT XT4;
SHORT YT4;

union
{
DWORD Coul;

	struct
	{
	BYTE B;
	BYTE V;
	BYTE R;
	BYTE Blend;
	};
};

BYTE Style;
BYTE TriState;
SHORT NumTex;
} SG_FACEUnique;
#endif

//Type Bloc
typedef struct
{
LONG NombreP;
SG_PNT3D* restrict MemPoints;
LONG NombreF;
SG_FACE* restrict MemFaces;
#ifdef SGE_TrieBlocs
float Profondeur;//Profondeur du bloc pour le tri
#endif
/*
#ifdef SGE_EMC
LONG NombreN;
SG_VECT *MemNorm;
#endif
*/
} SG_Bloc;

//#define LenBloc 16

//Type FullBloc
typedef struct
{
SG_Bloc DB;
//ceci est lu d'un bloc dans le fichier****
SG_VECT BRef;
float Rayon;
//ne pas séparer BRef et Rayon*************
int Etat;
//int NumAttach;
/*
#ifdef SGE_EMC
V_REPERE LightInvertTransform;
//V_VECT* OriginalMemPoints;
#endif
*/
#ifdef SPG_General_PGLib
//tableau de NombreTex pointeurs de pglblocs
PGLBloc** BlocByTex;
//tableau de NombreTex pointeurs de pgltexture
PGLTexture** Texture;
//tableau de NombreTex tableaux de pglVertex
SG_PNT3D** OpenGLpnt;
//tableau de NombreTex tableaux de pglIndex
SG_INDEX** OpenGLindex;
//tableau de NombreTex tableaux de pglColor
PGLColor** OpenGLcolor;
PGLTexCoord** OpenGLTexCoord;
int NombreTex;
#endif

} SG_FullBloc;

#define LenFullBloc sizeof(SG_FullBloc)

//est-ce que le bloc possede:
#define SG_NoPOINTS 0
#define SG_NoFACES 0
#define SG_OK 1
#define SG_WithPOINTS 2
#define SG_WithFACES 4
#define SG_WithTEXTURE 8
//sinon il pointe sur des points ou
//des faces qui appartiennent à un
//autre bloc

//Type Texture
typedef struct
{
BYTE * MemTex;
DWORD LarT;
} SG_Texture;

//#define LenTexture 8

//Type FullTexture
#ifndef SPG_General_PGLib
typedef struct
{
SG_Texture T;
int NumAttach;
int SizeX;
int SizeY;
} SG_FullTexture;
#else
typedef PGLTexture* SG_FullTexture;
#endif

//#define LenFullTexture sizeof(SG_FullTexture)

//#ifndef SPG_General_PGLib
//Type SG_3D
typedef struct
{
	int NombreB;
	SG_FullBloc* restrict FB;
	int NombreT;
	SG_FullTexture* restrict FT;
} SG_3D;

//#define Len3D sizeof(SG_3D)
//#endif

#ifndef SPG_General_PGLib
//Type Tri
typedef struct
{
SG_FACE *Face;
#ifdef SGE_EMC
union
{
LONG Prof;
struct
{
	BYTE Low0;
	BYTE Low1;
	WORD BProf;
	//BYTE Hi0;
};
};
#else
DWORD Prof;
#endif
} SG_Tri;

#define SG_OVERALL_DIST 0x01fffffff
//#define LenTri 8
#endif

//Type Mobil
typedef V_REPERE SG_Mobil;
/*
{
float X;
float Y;
float Z;

float XDIR;
float YDIR;
float ZDIR;

float XDPX;
float YDPX;
float ZDPX;

float XDPY;
float YDPY;
float ZDPY;
} SG_Mobil;
*/

typedef struct
{
	int Etat;
	PixCoul LightColor;
	SG_VECT LightPos;
	SG_VECT LightDir;
	float LightDistMin;
	float LightDistMax;
	float LightCone;
} SG_LIGHT;


//lumieres
/*
#define L_OFF 0
#define L_ON 1
*/
#define L_AMBIENT 1
#define L_LOCALISE 2

#define L_ISOTROPE 4
#define L_DIRECTIVE 8

//Type PDV
typedef struct
{
//TOUS
SG_Mobil Rep;

//TOUS
float DEcr;
float DMax;
float DTex;

#ifndef SPG_General_PGLib
#ifndef SGE_EMC
//ASM
float Davnt;
float Darre;
float InvDiff;
#endif
#endif

#ifndef SPG_General_PGLib
//EMC ou ASM
LONG NombreB;
SG_Bloc* restrict MemBloc;
LONG NombreT;
SG_Tri* restrict MemTri;
LONG TriFait;
#endif

#ifdef SGE_EMC
//EMC
SG_TriCatType* restrict TriCat;
SG_Tri** restrict TriPtrCat;
#endif

#ifndef SGE_EMC
#ifndef SPG_General_PGLib
//ASM
SHORT Etapes;// '0=infini 1=une étape ...
SHORT RetMax;
SHORT RetDist;
SHORT RetArr;
BYTE* restrict MemEcr;
DWORD Pitch;

DWORD CallBack;

SHORT PixTX;
SHORT PixTY;

SHORT PixMX;
SHORT PixMY;
#endif
#endif

//TOUS
SHORT PixlXPM;
SHORT PixlYPM;
#if defined(SPG_General_PGLib)||defined(SGE_EMC)
G_Ecran Ecran;
#endif

#ifndef SPG_General_PGLib
//EMC ou ASM
SG_Texture *MemTex;
#endif

#ifdef SGE_EMC
//EMC
DWORD RenderMode;
#endif

#if defined(SGE_EMC)||defined(SPG_General_PGLib)
PixCoul SkyColor;
#endif

#ifdef SGE_EMC
PixCoul FogColor;
float DFog;
int NombreLight;
SG_LIGHT* MemLight;
#endif

#ifndef SGE_EMC
#ifndef SPG_General_PGLib
//ASM
BYTE BlendC;//'blend des nouvelles faces normal=32
BYTE Filaire;
BYTE Copro;
BYTE ModeV;
#endif
#endif

} SG_PDV;


typedef struct
{
	float XMin;
	float YMin;
	float XMax;
	float YMax;
	float InvMaxMinX;
	float InvMaxMinY;
	int BlocX;
	int BlocY;
	SG_FullBloc* restrict Bloc;
} SG_DispatchDescr;


//#define LenPDV 132

#ifdef SPG_General_PGLib
#define SG_NORMAL_ALPHA G_NORMAL_ALPHA
#define SG_MAX_ALPHA G_MAX_ALPHA
#else
#ifdef SGE_EMC
#define SG_NORMAL_ALPHA G_NORMAL_ALPHA
#define SG_MAX_ALPHA G_MAX_ALPHA
#else
#define SG_NORMAL_ALPHA 32
#define SG_MAX_ALPHA 63
#endif
#endif

#ifndef SPG_General_PGLib
#ifndef SGE_EMC
#define SG_NOCHANGEBLEND 255
#endif
#endif
//n'existe plus semble t-il

//modes d'affichage
#ifdef SGE_EMC
#define SGR_NORMAL 1
#define SGR_FILAIRE 2
#define SGR_UNI 4
#define SGR_POINT 8
#define SGR_BLOC_TRI 16
#define SGR_FACE_NOTRI 32
#define SGR_SKY_NOCLEAR 64
#endif

/*
#if (sizeof(SG_PNT3D)!=LenPNT3D)
#error Type PNT3D incorrect
#endif
#if (sizeof(SG_FACE)!=LenFACE)
#error Type Face incorrect
#endif
#if (sizeof(SG_Bloc)!=LenBloc)
#error Type Bloc incorrect
#endif
*/

/*
Type Infos3D
NombreT As Long
NombreB As Long
NombreP As Long
NombreF As Long
End Type
*/

#include "SGRAPH.agh"
#include "SGRAPH_bloc.agh"
#include "SGRAPH_tex.agh"
#include "SGRAPH_Normales.agh"
#include "SGRAPH_geom.h"
#include "SGRAPH_Load3D.agh"

#ifdef SPG_General_PGLib
#include "SGRAPH_pgl.agh"
#endif
//#include "SGRAPH_opts.h"
#ifdef SGE_EMC
#include "SGRAPH_emc.h"
#endif

#define SG_GetPnt(FB,index) FB.DB.MemPoints[index]
#define SG_GetFce(FB,index) FB.DB.MemFaces[index]
#define SG_DefPnt(A,D) A.P=D
#define SG_DefPntI(FB,index,D) SG_GetPnt(FB,index).P=D
#define SG_DefPntXYZI(FB,index,x,y,z) V_SetXYZ(SG_GetPnt(FB,index).P,x,y,z)
#define SG_DefPntXYZ(A,X,Y,Z) {A.P.x=X;A.P.y=Y;A.P.z=Z;}
#ifdef SGE_EMC
#define SG_DefNormale(F,N) F.Normale=N
#else
#define SG_DefNormale(F,N)
#endif

#ifdef DebugSGRAPH
#define SG_CheckVDiff(P1,P2,Msg,RET) {float DP;V_Mod2DiffVect(P1,P2,DP);CHECK(DP<1e-6,"Points "#P1" "#P2" confondus",RET);}
#define SG_CheckVDist(P1,P2,Msg,RET) SG_CheckVDiff(P1,P2,Msg,RET)
#define SG_CheckPntDiff(P1,P2,Msg,RET) {float DP;V_Mod2DiffVect(P1->P,P2->P,DP);CHECK(DP<1e-6,"Points "#P1" "#P2" confondus",RET);}
#define SG_CheckPntDist(P1,P2,Msg,RET) SG_CheckPntDiff(P1,P2,Msg,RET)
#define SG_CheckFce(F,Msg,RET) SG_CheckPntDiff(F.NumP1,F.NumP2,Msg"\nFace "#F":",RET);SG_CheckPntDiff(F.NumP2,F.NumP3,Msg"\nFace "#F":",RET);SG_CheckPntDiff(F.NumP3,F.NumP4,Msg" Face "#F":",RET);SG_CheckPntDiff(F.NumP4,F.NumP1,Msg"\nFace "#F":",RET)
#else
#define SG_CheckVDiff(P1,P2,Msg,RET)
#define SG_CheckVDist(P1,P2,Msg,RET)
#define SG_CheckPntDiff(P1,P2,Msg,RET)
#define SG_CheckPntDist(P1,P2,Msg,RET)
#define SG_CheckFce(F,Msg,RET)
#define SG_VueCheck(Vue)
#endif
#ifdef SGE_DrawNormales
#define SG_CheckPntDiffFnct(P1,P2) (((P1->P.x-P2->P.x)*(P1->P.x-P2->P.x)+(P1->P.y-P2->P.y)*(P1->P.y-P2->P.y)+(P1->P.z-P2->P.z)*(P1->P.z-P2->P.z))<1e-6)
#define SG_CheckFceFnct(F) (SG_CheckPntDiffFnct(F.NumP1,F.NumP2)||SG_CheckPntDiffFnct(F.NumP2,F.NumP3)||SG_CheckPntDiffFnct(F.NumP3,F.NumP4)||SG_CheckPntDiffFnct(F.NumP4,F.NumP1))
#endif

#ifndef SPG_General_PGLib
#ifdef SGE_TEXTURE32
#define Texel(T,x,y) (*(PixCoul*)(T.MemTex+4*(x+(y<<T.LarT))))
#define TexelPTR(T,x,y) (T.MemTex+4*(x+(y<<T.LarT)))
#else
#define Texel(T,x,y) (*(PixCoul24*)(T.MemTex+3*(x+(y<<T.LarT))))
#define TexelPTR(T,x,y) (T.MemTex+3*(x+(y<<T.LarT)))
#endif
#endif

#ifndef SPG_General_PGLib
#define SG_IsValidTex(T) (T.NumAttach!=-1)
#define SG_SetFceTex(F,T) F.NumTex=(SHORT)T.NumAttach
//#define SG_SetFceTex(F,T) F.NumTex=T.NumAttach
#define SG_GetFceTex(F) F.NumTex
#define SG_GetTexUID(TX) TX.NumAttach
#define SG_SetToNullTex(TX) {TX.NumAttach=-1;TX.T.MemTex=0;}
#define SG_TexSizeX(T) T.SizeX
#define SG_TexSizeY(T) T.SizeY
#ifdef SGE_EMC
#define SG_MECR(Vue) (Vue.Ecran.MECR)
#define SG_POCT(Vue) (Vue.Ecran.POCT)
#define SG_Pitch(Vue) (Vue.Ecran.Pitch)
#define SG_SizeX(Vue) (Vue.Ecran.SizeX)
#define SG_SizeY(Vue) (Vue.Ecran.SizeY)
#define SG_TFACES(Vue) SGE_TransformAndRender(Vue)
#define SG_compatTFACES(Ecran,Coul,Vue) SGE_TransformAndRender(Vue)
//	if ((V.RenderMode&SGR_SKY_NOCLEAR)==0) G_DrawRect(V.Ecran,0,0,V.Ecran.SizeX,V.Ecran.SizeY,GlobalColor.Coul);
#else
#define SG_MECR(Vue) (Vue.MemEcr)
#define SG_POCT(Vue) (Vue.ModeV/8)
#define SG_Pitch(Vue) (Vue.Pitch)
#define SG_SizeX(Vue) (Vue.PixTX)
#define SG_SizeY(Vue) (Vue.PixTY)
#define SG_TFACES(Vue) if(Vue.NombreB) TFACES(&Vue);
#define SG_compatTFACES(Ecran,Coul,Vue) {G_DrawRect(Ecran,0,0,Ecran.SizeX,Ecran.SizeY,Coul);SG_TFACES(Vue);}
#endif
#else
#define SG_IsValidTex(T) (T!=0)
#define SG_SetFceTex(F,T) F.Texture=T
#define SG_GetFceTex(F) F.Texture
#define SG_GetTexUID(TX) TX
#define SG_SetToNullTex(T) T=0
#define SG_TexSizeX(T) (SHORT)(T->width)
#define SG_TexSizeY(T) (SHORT)(T->height)
//#define SG_compatTFACES(Ecran,Coul,Vue)	pglSwapBuffers(Global.display);
#define SG_compatTFACES(Ecran,Coul,Vue)

#endif
//V_Round(NInvMaxMinY*(j->P.y-MinY))
#define SG_DispatchX(SGDISPATCH,fx) V_Floor(SGDISPATCH.InvMaxMinX*(fx-SGDISPATCH.XMin))
#define SG_DispatchY(SGDISPATCH,fy) V_Floor(SGDISPATCH.InvMaxMinY*(fy-SGDISPATCH.YMin))
#define SG_DispatchXS(SGDISPATCH,fx) V_Sature(V_Floor(SGDISPATCH.InvMaxMinX*(fx-SGDISPATCH.XMin)),0,SGDISPATCH.BlocX-1)
#define SG_DispatchYS(SGDISPATCH,fy) V_Sature(V_Floor(SGDISPATCH.InvMaxMinY*(fy-SGDISPATCH.YMin)),0,SGDISPATCH.BlocY-1)
#define SG_DispatchXYS(SGDISPATCH,fx,fy) (SG_DispatchXS(SGDISPATCH,fx)+SGDISPATCH.BlocX*SG_DispatchYS(SGDISPATCH,fy))
#define SG_FaceCG(V,FACE) V.x=0.25f*(FACE.NumP1->P.x+FACE.NumP2->P.x+FACE.NumP3->P.x+FACE.NumP4->P.x);V.y=0.25f*(FACE.NumP1->P.y+FACE.NumP2->P.y+FACE.NumP3->P.y+FACE.NumP4->P.y);V.z=0.25f*(FACE.NumP1->P.z+FACE.NumP2->P.z+FACE.NumP3->P.z+FACE.NumP4->P.z)
#define SG_DispatchNombreB(SGDISPATCH) SGDISPATCH.BlocX*SGDISPATCH.BlocY
#define SG_DispatchIndex(SGDISPATCH,i) SGDISPATCH.Bloc[i]

#ifndef SPG_General_PGLib
#define SG_PGLDispatchByTex(Bloc,Primitive,Indexe,Linke)
#define SG_PGLCloseDispatch(Bloc)
#else
#define SG_AttachTexture(Vue,DescrTex,NumTex)
#define SG_DetachTexture(Vue,DescrTex)
#endif

#ifdef SGE_EMC
#define IF_SG_RenderWithoutTexNorLight(Vue) if(Vue.RenderMode&SGR_NORMAL)
#else
#ifndef SPG_General_PGLib
#define IF_SG_RenderWithoutTexNorLight(Vue) if(Vue.Filaire==0)
#else
#define IF_SG_RenderWithoutTexNorLight(Vue)	1
#endif
#endif

#ifdef SGE_EMC
//#define SG_AddToVue(Vue,Bloc) if (SGE_BlocVisible(Vue,Bloc)) Vue.MemBloc[Vue.NombreB++] = Bloc.DB
#define SG_AddToVue(Vue,Bloc) SGE_IfBlocVisible(Vue,Bloc,Vue.MemBloc[Vue.NombreB++]=Bloc.DB);
#define SG_ClearVue(Vue) Vue.NombreB=Vue.NombreLight=0;
#endif

#define SG_AdjustVue(Vue,VueRep,VuePos) {Vue.Rep=VueRep;V_Operate4(Vue.Rep.pos,+=VuePos.x*Vue.Rep.axex,+VuePos.y*Vue.Rep.axey,+VuePos.z*Vue.Rep.axez);}

#define SG_AddDispatchToVue(Vue,SDD) {int NBlocs=SG_DispatchNombreB(SDD);for(int i=0;i<NBlocs;i++){if (SG_DispatchIndex(SDD,i).Etat) SG_AddToVue(Vue,SG_DispatchIndex(SDD,i));}}
//#define SG_DupliqueBloc(Bloc,BlocRef,MODE) SG_InitBloc(Bloc,BlocRef.DB.MemPoints,BlocRef.DB.NombreP,BlocRef.DB.MemFaces,BlocRef.DB.NombreF,MODE)
#define SG_DupliqueBloc_NewMacroVersion(Bloc,BlocRef,MODE) SG_InitBloc(Bloc,BlocRef.DB.MemPoints,BlocRef.DB.NombreP,BlocRef.DB.MemFaces,BlocRef.DB.NombreF,MODE)


#define SG_FOR_ALL_FACES(Bloc,F,Instructions) {DbgCHECK(Bloc.Etat==0,"SG_FOR_ALL_FACES: Bloc nul");DbgCHECK(Bloc.DB.MemFaces==0,"SG_FOR_ALL_FACES: Bloc vide");if((Bloc.Etat)&&(Bloc.DB.MemFaces)){for(int SG_FAF=0;SG_FAF<Bloc.DB.NombreF;SG_FAF++){SG_FACE*F=Bloc.DB.MemFaces+SG_FAF;Instructions;};};}
#define SG_SetBlocStyle(B,NouveauStyle) SG_FOR_ALL_FACES(B,SG_SBS,SG_SBS->Style=NouveauStyle)

#ifndef DebugSGRAPH
#define SG_CheckDispatchDescr(DispDescr)
#endif

//attention la normale de la face n'est pas generee, utiliser defnormale
//#define SG_IndexArg(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3) SG_GetFce(FB,IndexFce),SG_GetPnt(FB,IndexPnt0),SG_GetPnt(FB,IndexPnt1),SG_GetPnt(FB,IndexPnt2),SG_GetPnt(FB,IndexPnt3)
//I=prend des index de points sinon c'est des pointeurs
#define SG_DefUniFce(F,p1,p2,p3,p4,coul) {F.NumP1 = p1;F.NumP2 = p2;F.NumP3 = p3;F.NumP4 = p4;F.Couleur.Coul = coul;F.Style = (BYTE)SG_UNI;SG_CheckFce(F,"SG_DefUniFce",;);}
#define SG_DefUniFce_Unsafe(F,p1,p2,p3,p4,coul) {F.NumP1 = p1;F.NumP2 = p2;F.NumP3 = p3;F.NumP4 = p4;F.Couleur.Coul = coul;F.Style = (BYTE)SG_UNI;}
#define SG_DefUniFceI(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul) SG_DefUniFce(SG_GetFce(FB,IndexFce),FB.DB.MemPoints+IndexPnt0,FB.DB.MemPoints+IndexPnt1,FB.DB.MemPoints+IndexPnt2,FB.DB.MemPoints+IndexPnt3,coul)
#define SG_DefUniFceI_Unsafe(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul) SG_DefUniFce_Unsafe(SG_GetFce(FB,IndexFce),FB.DB.MemPoints+IndexPnt0,FB.DB.MemPoints+IndexPnt1,FB.DB.MemPoints+IndexPnt2,FB.DB.MemPoints+IndexPnt3,coul)
#define SG_DefUniSFce(F,p1,p2,p3,p4,coul,style) {SG_DefUniFce(F,p1,p2,p3,p4,coul);F.Style=(BYTE)style;}
#define SG_DefUniSFce_Unsafe(F,p1,p2,p3,p4,coul,style) {SG_DefUniFce_Unsafe(F,p1,p2,p3,p4,coul);F.Style=(BYTE)style;}
#define SG_DefUniSFceI(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul,style) {SG_DefUniFceI(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul);SG_GetFce(FB,IndexFce).Style=(BYTE)style;}
#define SG_DefUniSFceI_Unsafe(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul,style) {SG_DefUniFceI_Unsafe(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul);SG_GetFce(FB,IndexFce).Style=(BYTE)style;}
#define SG_DefTexFce(F,p1,p2,p3,p4,coul,style,T,xt1,yt1,xt2,yt2,xt3,yt3,xt4,yt4) {SG_DefUniSFce(F,p1,p2,p3,p4,coul,style);SG_SetFceTex(F,T);F.XT1 = (SHORT)xt1;F.YT1 = (SHORT)yt1;F.XT2 = (SHORT)xt2;F.YT2 = (SHORT)yt2;F.XT3 = (SHORT)xt3;F.YT3 = (SHORT)yt3;F.XT4 = (SHORT)xt4;F.YT4 = (SHORT)yt4;}
#define SG_DefTexFce_Unsafe(F,p1,p2,p3,p4,coul,style,T,xt1,yt1,xt2,yt2,xt3,yt3,xt4,yt4) {SG_DefUniSFce_Unsafe(F,p1,p2,p3,p4,coul,style);SG_SetFceTex(F,T);F.XT1 = (SHORT)xt1;F.YT1 = (SHORT)yt1;F.XT2 = (SHORT)xt2;F.YT2 = (SHORT)yt2;F.XT3 = (SHORT)xt3;F.YT3 = (SHORT)yt3;F.XT4 = (SHORT)xt4;F.YT4 = (SHORT)yt4;}
#define SG_DefTexFceI(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul,style,T,xt1,yt1,xt2,yt2,xt3,yt3,xt4,yt4) {SG_DefUniSFceI(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul,style);SG_SetFceTex(SG_GetFce(FB,IndexFce),T);SG_GetFce(FB,IndexFce).XT1 = (SHORT)xt1;SG_GetFce(FB,IndexFce).YT1 = (SHORT)yt1;SG_GetFce(FB,IndexFce).XT2 = (SHORT)xt2;SG_GetFce(FB,IndexFce).YT2 = (SHORT)yt2;SG_GetFce(FB,IndexFce).XT3 = (SHORT)xt3;SG_GetFce(FB,IndexFce).YT3 = (SHORT)yt3;SG_GetFce(FB,IndexFce).XT4 = (SHORT)xt4;SG_GetFce(FB,IndexFce).YT4 = (SHORT)yt4;}
#define SG_DefTexFceI_Unsafe(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul,style,T,xt1,yt1,xt2,yt2,xt3,yt3,xt4,yt4) {SG_DefUniSFceI_Unsafe(FB,IndexFce,IndexPnt0,IndexPnt1,IndexPnt2,IndexPnt3,coul,style);SG_SetFceTex(SG_GetFce(FB,IndexFce),T);SG_GetFce(FB,IndexFce).XT1 = (SHORT)xt1;SG_GetFce(FB,IndexFce).YT1 = (SHORT)yt1;SG_GetFce(FB,IndexFce).XT2 = (SHORT)xt2;SG_GetFce(FB,IndexFce).YT2 = (SHORT)yt2;SG_GetFce(FB,IndexFce).XT3 = (SHORT)xt3;SG_GetFce(FB,IndexFce).YT3 = (SHORT)yt3;SG_GetFce(FB,IndexFce).XT4 = (SHORT)xt4;SG_GetFce(FB,IndexFce).YT4 = (SHORT)yt4;}
#define SG_ToPntIndex(BLOC,pP3D) (((int)(pP3D)-(int)(BLOC.DB.MemPoints))/sizeof(SG_PNT3D))

#ifndef SPG_General_PGLib
#define SG_DrawFPS(Ecran,CaracLib,DT) {char fpsString[32];fpsString[0]=0;CF_GetString(fpsString,1.0/DT,4);C_PrintWithBorder(Ecran,2,2,fpsString,CaracLib,0,0xffffff);}
#else
#define SG_DrawFPS(Ecran,CaracLib,DT)
#endif

//#ifndef SPG_General_PGLib
//#ifndef SGE_EMC

#ifndef SGE_EMC
#ifndef SPG_General_PGLib

#pragma SPGMSG(__FILE__,__LINE__,"SGRAPH : Needs SGraph.lib and SGraph.dll")

extern "C"
{
void __stdcall FACE (SG_FACEUnique* F, BYTE* MECR, LONG Pitch, LONG PixTX, LONG PixTY, BYTE* MTEX, LONG LarT, LONG Mode256);
void __stdcall TFACES(SG_PDV* Vue);
void __stdcall GENEREBLOC(SG_Bloc* BlocSrc, SG_Bloc* BlocDst, SG_Mobil* Repere);
void __stdcall ORTHONORME(SG_Mobil* Vue);
//void __stdcall ORTHONORME(V_REPERE* Vue);
LONG __stdcall ESTVISIBLE(SG_Mobil* V, float* DmaxX, float* DmaxY, float* DmaxZ, SG_VECT* BlocPos, float* Rayon);
void __stdcall CLBD(BYTE* PMem,LONG SizeX,LONG SizeY,LONG CoulInit,LONG CoulFinal,LONG Pitch);
void __stdcall CLBD32(BYTE* PMem, LONG SizeX, LONG SizeY, LONG CoulInit, LONG CoulFinal, LONG Pitch);
void __stdcall CLBDSMOOTH(BYTE* PMem,LONG SizeX,LONG SizeY,LONG CoulInit,LONG CoulFinal,LONG Pitch);
void __stdcall CLBD32SMOOTH(BYTE* PMem, LONG SizeX, LONG SizeY, LONG CoulInit, LONG CoulFinal, LONG Pitch);
void __stdcall CLBD256(BYTE* PMem,LONG SizeX,LONG SizeY,LONG Coul,LONG Pitch);
}

#endif
#endif

#endif

