
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#include "SPG_Includes.h"

#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#ifdef DebugFloat
#include <float.h>
#endif
#define MaxTex SG_MAX_TEX
#ifndef SPG_General_PGLib
#define MaxBloc SG_MAX_BLOC
//#define MaxTri SG_MAX_TRI

#endif

#ifdef DebugSGRAPH


int SPG_CONV SG_VueCheck(SG_PDV& Vue)
{
#ifndef SPG_General_PGLib
	int i,j;
	int SB=0;
	int SP=0;
	int SF=0;

#ifndef SGE_EMC
	DbgCHECK(sizeof(SG_PNT3D)!=20,"SG_VueCheck: Erreur type PNT3D");
	DbgCHECK(sizeof(SG_FACE)!=40,"SG_VueCheck: Erreur type FACE");
	DbgCHECK(sizeof(SG_Bloc)!=16,"SG_VueCheck: Erreur type Bloc");
	DbgCHECK(sizeof(SG_PDV)!=132,"SG_VueCheck: Erreur type Vue");
#endif

	//SPG_List1("Objets",Vue.NombreB);
	SB+=Vue.NombreB;
	for(i=0; i<Vue.NombreB; i++)
	{
		//SPG_List1("Points", Vue.MemBloc[i].NombreP);
		SP+=Vue.MemBloc[i].NombreP;
		for(j=0; j<Vue.MemBloc[i].NombreP; j++)
		{
			CHECK(Vue.MemBloc[i].MemPoints==0,"SG_VueCheck: Bloc vide",return 0);
			if(fabs(Vue.MemBloc[i].MemPoints[j].P.x)+fabs(Vue.MemBloc[i].MemPoints[j].P.y)+fabs(Vue.MemBloc[i].MemPoints[j].P.z)>10000)
			{
				SPG_List("Warning: Point trop distant");
				break;
			}
		}
		//SPG_List1("Faces",Vue.MemBloc[i].NombreF);
		SF+=Vue.MemBloc[i].NombreF;
		for(j=0; j<Vue.MemBloc[i].NombreF; j++)
		{
			if(
				V_IsBound(Vue.MemBloc[i].MemFaces[j].NumP1,Vue.MemBloc[i].MemPoints,Vue.MemBloc[i].MemPoints+Vue.MemBloc[i].NombreP)&&
				V_IsBound(Vue.MemBloc[i].MemFaces[j].NumP2,Vue.MemBloc[i].MemPoints,Vue.MemBloc[i].MemPoints+Vue.MemBloc[i].NombreP)&&
				V_IsBound(Vue.MemBloc[i].MemFaces[j].NumP3,Vue.MemBloc[i].MemPoints,Vue.MemBloc[i].MemPoints+Vue.MemBloc[i].NombreP)&&
				V_IsBound(Vue.MemBloc[i].MemFaces[j].NumP4,Vue.MemBloc[i].MemPoints,Vue.MemBloc[i].MemPoints+Vue.MemBloc[i].NombreP)
				)
			{
			}
			else
			{
				SPG_List1("Erreur de pointeur face->point\nBloc",i);
				//return 0;
			}
			
			if(Vue.MemBloc[i].MemFaces[j].Style&SG_RENDER_TEX)
			{
			int a = Vue.MemBloc[i].MemFaces[j].NumTex;
			if((a<0)||(a>32))
			{
				SPG_List("SG_VueCheck: Indice de texture anormal");
				//return 0;
			}

			int L = Vue.MemTex[a].LarT;
			if((L<0)||(L>13))
			{
				SPG_List1("Texture mal initialisee nr",a);
				//return 0;
			}
			}
		}
	}
/*
	if (SB) SPG_List3("Blocs",SB,"\nPoints",SP,"\nFaces",SF);
#ifdef DebugMem
	SPG_MemGetTotal(0);
#endif
*/
#endif
	return -1;
}

void SPG_CONV SG_CheckRepere(SG_Mobil& Rep)
{
	char msg[2048];
	sprintf(msg,
		"x=%f y=%f z=%f\naxex.x=%f axex.y=%f axex.z=%f\naxey.x=%f axey.y=%f axey.z=%f\naxez.x=%f axez.y=%f axez.z=%f",
		Rep.pos.x,Rep.pos.y,Rep.pos.z,
		Rep.axex.x,Rep.axex.y,Rep.axex.z,
		Rep.axey.x,Rep.axey.y,Rep.axey.z,
		Rep.axez.x,Rep.axez.y,Rep.axez.z);
	SPG_List(msg);
	return;
}

/*
void SG_DefFce(SG_FACE &F,
			   SG_PNT3D *p1,SG_PNT3D *p2,SG_PNT3D *p3,SG_PNT3D *p4,
			   DWORD coul,int style,int numtex,
			   int xt1,int yt1,int xt2,int yt2,int xt3,int yt3,int xt4,int yt4)
{
	F.NumP1 = p1;
	F.NumP2 = p2;
	F.NumP3 = p3;
	F.NumP4 = p4;
	F.Couleur = coul;
	F.Style = style;
	F.NumTex = numtex;
	F.XT1 = xt1;
	F.YT1 = yt1;
	F.XT2 = xt2;
	F.YT2 = yt2;
	F.XT3 = xt3;
	F.YT3 = yt3;
	F.XT4 = xt4;
	F.YT4 = yt4;
	return;
}
*/
#endif

int SPG_CONV SG_InitVue(SG_PDV &Vue,G_Ecran &E,float DistMax,float DistTex,float DistEcr,float DistPos, float VertPos, DWORD SkyColor, DWORD FogColor, int MaxTri)
{
	memset(&Vue,0,sizeof(SG_PDV));

	CHECK(E.Etat==0,"SG_InitVue: Ecran nul",return 0);
	CHECK(!V_InclusiveBound(E.SizeX,1,16384),"SG_InitVue: Taille X Ecran invalide",return 0);
	CHECK(!V_InclusiveBound(E.SizeY,1,16384),"SG_InitVue: Taille Y Ecran invalide",return 0);

	V_SetXYZ(Vue.Rep.pos,0,-DistPos,VertPos);
	if(DistPos==0)
	{
		V_SetXYZ(Vue.Rep.axex,1,0,0);
	}
	else
	{
		V_Operate2(Vue.Rep.axex, =-Vue.Rep.pos);
	}

	V_SetXYZ(Vue.Rep.axey,Vue.Rep.axex.y,-Vue.Rep.axex.x,0);
	V_SetXYZ(Vue.Rep.axez,0,0,1);
	V_Orthonorme(Vue.Rep);
	Vue.DEcr = DistEcr;
	Vue.DMax = DistMax;
#if defined(SGE_EMC)||defined(SPG_General_PGLib)
	Vue.DTex = DistTex;
#else
	Vue.DTex = 4 * DistTex;
#endif

#ifndef SPG_General_PGLib
#ifndef SGE_EMC
//ASM
	Vue.Davnt = 4*Vue.DEcr;
	Vue.Darre = -15*Vue.DEcr;
	Vue.InvDiff = Vue.DEcr/(Vue.Davnt-Vue.Darre);
#endif
#endif

#ifndef SPG_General_PGLib
	Vue.NombreB = 0;
	Vue.MemBloc = SPG_TypeAlloc(MaxBloc,SG_Bloc,"DescrBloc");
	CHECK(Vue.MemBloc==0,"SG_InitVue: Erreur allocation descripteur de descripteurs de blocs",SG_CloseVue(Vue);return 0)
	Vue.NombreT = MaxTri;
	Vue.MemTri = SPG_TypeAlloc(Vue.NombreT * 2,SG_Tri, "Tri");
	CHECK(Vue.MemTri==0,"SG_InitVue: Erreur allocation tri",SG_CloseVue(Vue);return 0)
	//Vue.MemEndTriSecondPart = Vue.MemTri+2*Vue.NombreT;
	Vue.TriFait = 0;
#endif
	
#ifdef SGE_EMC
//EMC
	Vue.TriCat = SPG_TypeAlloc(SG_TriCatNum,SG_TriCatType, "Categories");
	Vue.TriPtrCat = SPG_TypeAlloc(SG_TriCatNum,SG_Tri*, "CaterBase");
#endif

#ifndef SGE_EMC
#ifndef SPG_General_PGLib
//ASM
	Vue.Etapes = 0;// '0=infini 1=une étape ...
	Vue.RetMax = 2;
	Vue.RetDist = 0;
	Vue.RetArr = 0;

	Vue.MemEcr = E.MECR;
	Vue.Pitch = E.Pitch;

	Vue.CallBack = 0;

	Vue.PixTX = E.SizeX;
	Vue.PixTY = E.SizeY;
	Vue.PixMX = Vue.PixTX / 2;
	Vue.PixMY = Vue.PixTY / 2;
#endif
#endif

//TOUS
	Vue.PixlXPM = V_FloatToShort(E.SizeY / 0.22);
	Vue.PixlYPM = V_FloatToShort(-E.SizeY / 0.22);
#if defined(SPG_General_PGLib)||defined(SGE_EMC)
#ifdef DebugSGRAPH
#ifdef SGE_EMC
		G_InitSousEcran(Vue.Ecran,E,2,2,E.SizeX-4,E.SizeY-4);
#else
		Vue.Ecran=E;
#endif
#else
		Vue.Ecran=E;
#endif
#endif

#ifndef SPG_General_PGLib
//EMC ou ASM
	Vue.MemTex = SPG_TypeAlloc(MaxTex,SG_Texture,"DescrTex");
	CHECK(Vue.MemTex==0,"SG_InitVue: Erreur allocation descripteur de textures",SG_CloseVue(Vue);return 0)
#endif

#ifdef SGE_EMC
//EMC
	Vue.RenderMode=SGR_NORMAL;
#endif
#if defined(SGE_EMC)||defined(SPG_General_PGLib)
//EMC ou PGLib
	Vue.SkyColor=*(PixCoul*)&SkyColor;
#endif
#ifdef SGE_EMC
//EMC
	Vue.FogColor=*(PixCoul*)&FogColor;
	Vue.DFog=Vue.DTex;
	Vue.NombreLight=0;
	Vue.MemLight=SPG_TypeAlloc(SG_MAX_LIGHT,SG_LIGHT,"Lumieres");
	//SG_SetFog(Vue,Vue.DTex,SkyColor);
#endif

#ifndef SGE_EMC
#ifndef SPG_General_PGLib
	Vue.BlendC = SG_NORMAL_ALPHA;
	Vue.Filaire = 0;
	Vue.Copro = 0;
	Vue.ModeV = 8*E.POCT;
#endif
#endif

	return -1;
}

void SPG_CONV SG_SetViewAxis(SG_PDV& Vue, V_VECT& ObjectPos)
{
	V_VECT RelativePos;
	V_Operate3(RelativePos,=ObjectPos,-Vue.Rep.pos);
	V_NormalizeSafe(RelativePos);
	V_VECT Rotation;
	Rotation.x=0;
	V_ScalVect(RelativePos,Vue.Rep.axey,Rotation.z);
	V_ScalVect(RelativePos,Vue.Rep.axez,Rotation.y);
	Rotation.z=1.f*Rotation.z;
	Rotation.y=-1.f*Rotation.y;
	V_RotateRepS(Vue.Rep,Rotation);
	return;
}

int SPG_CONV SG_SetLight(SG_PDV& Vue, PixCoul& LightColor, SG_VECT& LightPos, SG_VECT& LightDir, float LightDistMin, float LightDistMax, float LightCone, int Etat)
{
#ifdef SGE_EMC
	CHECK(Vue.NombreLight==SG_MAX_LIGHT,"Nombre max de lumieres atteint",return -1);
	Vue.MemLight[Vue.NombreLight].Etat=Etat;//|L_ON;
	Vue.MemLight[Vue.NombreLight].LightColor=LightColor;
	Vue.MemLight[Vue.NombreLight].LightPos=LightPos;
	Vue.MemLight[Vue.NombreLight].LightDir=LightDir;
	Vue.MemLight[Vue.NombreLight].LightDistMin=LightDistMin;
	Vue.MemLight[Vue.NombreLight].LightDistMax=LightDistMax;
	Vue.MemLight[Vue.NombreLight].LightCone=LightCone;
	Vue.NombreLight++;
	return Vue.NombreLight-1;
#else
	return 0;
#endif
}

void SPG_CONV SG_CloseVue(SG_PDV &Vue)
{

#ifndef SPG_General_PGLib
//DbgCHECK(Vue.NombreB!=0,"SG_CloseVue: Blocs non desalloues");
DbgCHECK(Vue.MemBloc==0,"SG_CloseVue: Descripteur de blocs nul");
	if (Vue.MemBloc)
		SPG_MemFree(Vue.MemBloc);
	Vue.MemBloc = 0;

DbgCHECK(Vue.MemTri==0,"SG_CloseVue: Tri nul");
	if (Vue.MemTri)
		SPG_MemFree(Vue.MemTri);
	Vue.MemTri = 0;

	if (Vue.MemTex)
	{

#ifdef DebugSGRAPH
		for(int i=0; i<MaxTex; i++)
		{
			DbgCHECK(Vue.MemTex[i].MemTex,"SG_CloseVue: Texture non desallouee");
			//if(Vue.MemTex[i].MemTex!=0) UnloadTex(Vue.MemTex[i]);
		}
#endif
		DbgCHECK(Vue.MemTex==0,"SG_CloseVue: Descripteur de descripteurs de textures nul");

		SPG_MemFree(Vue.MemTex);
	}
	Vue.MemTex=0;
#ifdef SGE_EMC
	if (Vue.TriCat)
		SPG_MemFree(Vue.TriCat);
	Vue.TriCat=0;
	if (Vue.TriPtrCat)
		SPG_MemFree(Vue.TriPtrCat);
	Vue.TriPtrCat=0;
	if (Vue.MemLight)
		SPG_MemFree(Vue.MemLight);
	Vue.MemLight=0;
#endif

#endif

	return;
}

/*
void SPG_CONV SG_AdjustVue(SG_PDV& Vue,V_REPERE& VueRep, V_VECT& VuePos)
{

	Vue.Rep=VueRep;
	V_Operate4(Vue.Rep.pos,+=VuePos.x*Vue.Rep.axex,+VuePos.y*Vue.Rep.axey,+VuePos.z*Vue.Rep.axez);
	
	return;
}
*/

#ifndef SG_ClearVue
void SPG_CONV SG_ClearVue(SG_PDV &Vue)
{
#ifndef SPG_General_PGLib
	Vue.NombreB = 0;
#ifdef SGE_EMC
	Vue.NombreLight=0;
#endif
	return;
#else

	pglDepthMode(utrue,utrue,PGL_LESS);
	//MEMOIRE OPENGL RGBARGBA
	PGLColor CC;
	CC.b=Vue.SkyColor.B;
	CC.g=Vue.SkyColor.V;
	CC.r=Vue.SkyColor.R;
	pglClearColor(CC);
	pglClear(PGL_COLOR_BUFFER_BIT|PGL_DEPTH_BUFFER_BIT);
	pglProjectionLoadIdentity();
	//pglPerspective(90,-4/3,Vue.DEcr,Vue.DMax);
	pglPerspective(360.0*atan(0.5*Vue.Ecran.SizeX/(Vue.PixlXPM*Vue.DEcr))/V_PI,-4/3,Vue.DEcr,Vue.DMax);
	float lkatx=Vue.Rep.pos.x+Vue.Rep.axex.x;
	float lkaty=Vue.Rep.pos.y+Vue.Rep.axex.y;
	float lkatz=Vue.Rep.pos.z+Vue.Rep.axex.z;
	pglLookAt(Vue.Rep.pos.x,Vue.Rep.pos.y,Vue.Rep.pos.z,lkatx,lkaty,lkatz,Vue.Rep.axez.x,Vue.Rep.axez.y,Vue.Rep.axez.z);
	pglModelLoadIdentity();

	/*
#ifdef DebugList
	SPG_List("SG_CreateTexture: Eviter de l'appeler en SPG_General_PGLib");
#endif
	*/
	return;
#endif
}
#endif//Fin ifndef SG_ClearVue

//doit etre statique ou dynamique avec sgraph
//et ? avec opengl
#ifndef SG_AddToVue
int SPG_CONV SG_AddToVue(SG_PDV &Vue,SG_FullBloc &Bloc)
{
#ifdef SGE_EMC//SGE
	CHECK(V_IsBound(Vue.NombreB,0,MaxBloc)==0,"SG_AddToVue: Trop de blocs",return 0);
	CHECK(Bloc.DB.NombreP==0,"SG_AddToVue: Bloc sans points",return 0);
	CHECK(Bloc.DB.MemPoints==0,"SG_AddToVue: Bloc sans points",return 0);
	CHECK(Bloc.DB.NombreF==0,"SG_AddToVue: Bloc sans faces",return 0);
	CHECK(Bloc.DB.MemFaces==0,"SG_AddToVue: Bloc sans faces",return 0);

if (Vue.RenderMode&SGR_BLOC_TRI)
{
	if (SGE_BlocVisible(Vue,Bloc))
	{
	for(int i=0;i<Vue.NombreB;i++)
	{
		if(Vue.MemBloc[i].Profondeur>Bloc.DB.Profondeur)
		{
			break;
		}
	}
	//Vue.NombreB++;
	for(int j=Vue.NombreB;j>i;j--)
	{
		Vue.MemBloc[j]=Vue.MemBloc[j-1];
	}
	Vue.MemBloc[i]=Bloc.DB;
	Vue.NombreB++;
	}
}
else
{
	if (SGE_BlocVisible(Vue,Bloc)) Vue.MemBloc[Vue.NombreB++] = Bloc.DB;
}
	return -1;
#else//PAS SGE

#ifdef SPG_General_PGLib//PGLib
	pglModelLoadIdentity();
	if(SG_PGLBlocVisible(Vue,Bloc))
	{
	if(Bloc.BlocByTex==0)
	{
#ifdef DebugList
		SPG_List("Bloc non DispatchByTex-ifié");
#endif
		if(SG_PGLDispatchByTex(Bloc,PGL_QUADS,1,1)==0) return 0;
	}
	if(Bloc.BlocByTex)
	{//renderer les textures une par une
		pglRenderBlocs(Bloc.NombreTex,Bloc.BlocByTex);
		return -1;
	}
	}
	return 0;
#else//Ni SGE Ni PGLib = SGRAPH
	float VDMax=Vue.DMax;///4;
			if (
			(Bloc.Etat)
			&&
			(
			ESTVISIBLE(&Vue.Rep,&VDMax,&VDMax,&VDMax,
			&Bloc.BRef,&Bloc.Rayon)
			))
			Vue.MemBloc[Vue.NombreB++] = Bloc.DB;
	return -1;
#endif//Fin if PGLib
#endif//Fin if SGE
}

#endif//Fin ifdef SG_AddToVue

#endif



