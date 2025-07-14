
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#include "SPG_Includes.h"

#include <string.h>
#include <stdio.h>
//histobits doit diviser 8
#define HistoBits 4
#define HistoVal 16
#define HistoMsk 0x000F0F0F

#define MaxTex SG_MAX_TEX

//SPG_FASTCONV?
int SPG_CONV SG_FirstFreeTex(SG_PDV &Vue)
{
#ifdef DebugSGRAPH
	CHECK(Vue.MemTex==0,"SG_FirstFreeTex",return 0);
#endif
#ifndef SPG_General_PGLib
	int NumFirstTex;
	for(NumFirstTex=0; NumFirstTex<MaxTex; NumFirstTex++)
		if (Vue.MemTex[NumFirstTex].MemTex==0) break;
	if (NumFirstTex>=MaxTex) return -1;
	return NumFirstTex;
#else
#ifdef DebugList
	//SPG_List("SG_FirstFreeTex: Eviter de l'appeler en SPG_General_PGLib");
#endif
	return 0;
#endif
}

int SPG_CONV SG_CreateTexture(SG_FullTexture& DescrTex, int SizeX,int SizeY)
{
#ifndef SPG_General_PGLib

	memset(&DescrTex,0,sizeof(SG_FullTexture));
	DescrTex.NumAttach=-1;

	CHECK((SizeX<=0)||(SizeY<=0),"SG_CreateTexture: Taille invalide",return 0);

	int L;
	for(L=0; (1<<L)<SizeX; L++);

	DescrTex.T.MemTex = (BYTE*)SPG_MemAlloc(3*(1<<L)*SizeY,"CreateTexture");
	CHECK(DescrTex.T.MemTex==0,"SG_CreateTexture: Allocation echouee",SG_CloseTexture(DescrTex);return 0);
	
	DescrTex.T.LarT = L;
	DescrTex.SizeX = SizeX;
	DescrTex.SizeY = SizeY;
	return -1;
#else
	memset(&DescrTex,0,sizeof(SG_FullTexture));
#ifdef DebugList
	//SPG_List("SG_CreateTexture: Eviter de l'appeler en SPG_General_PGLib");
#endif
	return 0;
#endif
}

//ne verifie absoluement pas le format
//etendre aux bitmaps 8bits
//etendre au jpeg
int SPG_CONV SG_LoadTexture(SG_FullTexture &DescrTex,char *Name)
{
#ifdef SPG_General_PGLib
	CHECK((DescrTex=pglLoadTexture(Global.display,PGL_RGBA,Name,SG_NORMAL_ALPHA,PGL_LINEAR))==0,"SG_LoadTexture: pglLoadTexture echouee",return 0);
	return -1;
#else

	memset(&DescrTex,0,sizeof(SG_FullTexture));
	DescrTex.NumAttach=-1;

	G_Ecran ETex;
	CHECKTWO(G_InitEcranFromFile(ETex,3,1,Name)==0,"SG_LoadTexture: SG_InitEcranFromFile echouee sur le fichier",Name,return 0);
	CHECK(SG_ConvertEcranToTexture(DescrTex,ETex)==0,"SG_LoadTexture: SG_ConvertEcranToTexture echouee",return 0);
	SPG_SetMemName(DescrTex.T.MemTex,SPG_NameOnly(Name));
	return -1;
#endif
}

#ifndef SPG_General_PGLib
void SPG_CONV SG_SaveTexture(char *Name,SG_PDV &Vue,int NumTex,int SizeY)
{
	CHECK(V_IsBound(NumTex,0,MaxTex)==0,"SG_SaveTexture: Mauvais n° de texture",return);
	G_SaveToBMP(Name,Vue.MemTex[NumTex].MemTex,3,3*(1<<Vue.MemTex[NumTex].LarT),1<<Vue.MemTex[NumTex].LarT,SizeY);
	return;
}

void SPG_CONV SG_SaveTexture(char *Name,SG_FullTexture& T)
{
	CHECK(T.T.MemTex==0,"Texture vide",return);
	G_SaveToBMP(Name,T.T.MemTex,3,3<<T.T.LarT,T.SizeX,T.SizeY);
	return;
}
#endif

void SPG_CONV SG_CloseTexture(SG_FullTexture &DescrTex)
{
#ifdef SPG_General_PGLib
	if (DescrTex) pglDestroyTexture(DescrTex);
#else
	DbgCHECK(DescrTex.NumAttach!=-1,"SG_CloseTex: Desallocation d'une texture en usage")
	if (DescrTex.NumAttach!=-1) return;
	DescrTex.T.LarT=0;
	if(DescrTex.T.MemTex)
		SPG_MemFree(DescrTex.T.MemTex);
	DescrTex.T.MemTex=0;
#endif
	return;
}


#ifndef SPG_General_PGLib

int SPG_CONV SG_AttachTexture(SG_PDV &Vue,SG_FullTexture &DescrTex,int NumTex)
{
	CHECK(Vue.Ecran.Etat==0,"SG_AttachTexture",return 0);
	CHECK(V_IsBound(NumTex,0,MaxTex)==0,"SG_AttachTexture: Mauvais index de texture",DescrTex.NumAttach=-1;return 0);
	CHECK(DescrTex.T.MemTex==0,"SG_AttachTexture: Texture nulle",DescrTex.NumAttach=-1;return 0);
	CHECK(Vue.MemTex[NumTex].MemTex!=0,"SG_AttachTexture: Une texture est deja chargee",DescrTex.NumAttach=-1;return 0);
	DbgCHECK(DescrTex.NumAttach!=-1,"SG_AttachTexture: Cette texture est deja attachee");
//if (Vue.MemTex[NumTex].MemTex!=0) SG_CloseTex(Vue.MemTex[NumTex]);
	Vue.MemTex[NumTex] = DescrTex.T;
	DescrTex.NumAttach=NumTex;
	return -1;
}

int SPG_CONV SG_DetachTexture(SG_PDV &Vue,SG_FullTexture &DescrTex)
{
	DbgCHECK((DescrTex.NumAttach==-1)&&(DescrTex.T.MemTex==0),"SG_DetachTexture: Faces unies, IGNORER LE PROCHAIN MESSAGE");
	CHECK(V_IsBound(DescrTex.NumAttach,0,MaxTex)==0,"SG_DetachTexture: Faces unies ou bien Mauvais index de texture",return 0);
	DbgCHECK(Vue.MemTex[DescrTex.NumAttach].MemTex==0,"SG_DetachTexture: Texture vide");

//if (Vue.MemTex[NumTex].MemTex!=0) SG_CloseTex(Vue.MemTex[NumTex]);
	CHECK(Vue.MemTex[DescrTex.NumAttach].MemTex!=DescrTex.T.MemTex,"SG_DetachTexture: Une autre texture a remplace ou\nCe descripteur est invalide ou\nCette vue est invalide",return 0);
	CHECK(Vue.MemTex[DescrTex.NumAttach].LarT!=DescrTex.T.LarT,"SG_DetachTexture: Une autre texture a remplace ou\nCe descripteur est invalide ou\nCette vue est invalide",return 0);
	Vue.MemTex[DescrTex.NumAttach].MemTex = 0;
	Vue.MemTex[DescrTex.NumAttach].LarT = 0;
	DescrTex.NumAttach=-1;
	return -1;
}

int SPG_CONV SG_InitEcranFromTexture(G_Ecran& E, SG_FullTexture& T)
{
	CHECK(T.T.MemTex==0,"SG_InitEcranFromTexture: Texture invalide",memset(&E,0,sizeof(G_Ecran));return 0);
	return G_InitEcran(E,G_ALIAS_MEMOIRE,
		T.T.MemTex,3*(1<<T.T.LarT),3,
		T.SizeX,T.SizeY,
		0,0,
		0);
}

int SPG_CONV SG_CreateTextureFromEcran(SG_FullTexture& DescrTex,G_Ecran& E,int ForceRealloc)
{
	memset(&DescrTex,0,sizeof(SG_FullTexture));
	DescrTex.NumAttach=-1;
	CHECK(((E.Etat&G_MEMORYAVAILABLE)&&(E.Etat&G_ALLOC_MEMOIRE))==0,"SG_CreateTextureFromEcran: Ecran invalide pour cette opération",return 0);
	CHECK(G_POCT(E)!=3,"SG_CreateTextureFromEcran: L'ecran doit etre 24 bits",return 0);
	CHECK(G_GetPix(E)==0,"SG_CreateTextureFromEcran: Ecran vide",return 0);

	CHECK((E.SizeX<=0)||(E.SizeY<=0),"SG_CreateTextureFromEcran: Taille invalide",return 0);

	int L;
	for(L=0; (1<<L)<E.SizeX; L++);
	int Pitch=1<<L;

	if((Pitch==E.SizeX)&&(ForceRealloc==0))
	{
		DescrTex.T.MemTex = G_GetPix(E);
		E.Etat&=~G_ALLOC_MEMOIRE;
		E.Etat|=G_ALIAS_MEMOIRE;
	}
	else
	{
		DescrTex.T.MemTex = SPG_TypeAlloc(Pitch*3*E.SizeY,BYTE,"TextureFromEcranRealloc");
		for(int y=0;y<E.SizeY;y++)
		{
			memcpy(DescrTex.T.MemTex+Pitch*3*y,G_GetPix(E)+G_Pitch(E)*y,G_POCT(E)*G_SizeX(E));
		}
	}
	
	DescrTex.T.LarT = L;
	DescrTex.SizeX = E.SizeX;
	DescrTex.SizeY = E.SizeY;
	return -1;
}

int SPG_CONV SG_ConvertEcranToTexture(SG_FullTexture& T,G_Ecran& E)
{
	CHECK(SG_CreateTextureFromEcran(T,E,0)==0,"SG_ConvertEcranToTexture: SG_CreateTextureFromEcran echoue",G_CloseEcran(E);return 0);
	G_CloseEcran(E);
	return -1;
}

#endif

#ifndef SPG_General_PGLib
//si l'histogramme n'a qu'une valeur rendre la face unie!
DWORD SPG_CONV SG_FindBestColor(SG_Texture &T,int xmin,int ymin,int xmax,int ymax)
{
	int Cater[HistoVal*HistoVal*HistoVal];
	
	PixCoul PMin;
	PixCoul PMax;
	
	PMin.Coul=0;
	PMax.Coul=0x00ffffff;
	
	int Shift;
	
	for(Shift=8-HistoBits; Shift>=0; Shift-=HistoBits)
	{
		int y;
		for (y=0; y<HistoVal*HistoVal*HistoVal; y++)
		{
			Cater[y]=0;
		}
		
		for(y=ymin; y<=ymax; y++)
		{
			int x;
			for(x=xmin; x<=xmax; x++)
			{
#ifdef SGE_TEXTURE32
				PixCoul Coul=Texel(T,x,y);
#else
				PixCoul Coul=*(PixCoul*)TexelPTR(T,x,y);
#endif
				if ((Coul.R>=PMin.R)&&(Coul.V>=PMin.V)&&(Coul.B>=PMin.B)&&
					(Coul.R<=PMax.R)&&(Coul.V<=PMax.V)&&(Coul.B<=PMax.B)
					)
				{
					Coul.Coul=(Coul.Coul>>Shift)&HistoMsk;
					Cater[Coul.B+(Coul.V<<HistoBits)+(Coul.R<<(2*HistoBits))]++;
				}
			}
		}
		int nmax = 0;
		int n = 0;
		for (y=0; y<HistoVal*HistoVal*HistoVal; y++)
		{
			if (Cater[y]>nmax) 
			{
				nmax=Cater[y];
				n=y;
			}
		}
		PixCoul PN;
		PN.B = (BYTE)(n&(HistoVal-1));
		PN.V = (BYTE)((n>>HistoBits)&(HistoVal-1));
		PN.R = (BYTE)((n>>(2*HistoBits))&(HistoVal-1));
		PN.Coul <<= Shift;
		PMin.Coul |= PN.Coul;
		PMax.Coul = PMin.Coul+((0x00010101<<(HistoBits+Shift))-0x00010101);
	}
	return PMin.Coul;//(PMin.Coul|0x200000ff);
}
#endif
/*
DWORD SG_FindBestColorFast(SG_Texture &T,int xmin,int ymin,int xmax,int ymax)
{
	return Texel(T,((xmin+xmax)>>1),((ymin+ymax)>>1)).Coul;
}
*/

void SPG_CONV SG_MakeCoulFromTex(SG_PDV &Vue,SG_Bloc &Bloc)
{
#ifndef SPG_General_PGLib
	int i;
	for(i=0; i<Bloc.NombreF; i++)
		if ((Bloc.MemFaces[i].Style&SG_TEX)&&
			(Vue.MemTex[Bloc.MemFaces[i].NumTex].MemTex)
			)
		{
			Bloc.MemFaces[i].Couleur.Coul=SG_FindBestColor(Vue.MemTex[Bloc.MemFaces[i].NumTex],
				V_Min(
				V_Min(Bloc.MemFaces[i].XT1,Bloc.MemFaces[i].XT2),
				V_Min(Bloc.MemFaces[i].XT3,Bloc.MemFaces[i].XT4)
				),
				V_Min(
				V_Min(Bloc.MemFaces[i].YT1,Bloc.MemFaces[i].YT2),
				V_Min(Bloc.MemFaces[i].YT3,Bloc.MemFaces[i].YT4)
				),
				V_Max(
				V_Max(Bloc.MemFaces[i].XT1,Bloc.MemFaces[i].XT2),
				V_Max(Bloc.MemFaces[i].XT3,Bloc.MemFaces[i].XT4)
				),
				V_Max(
				V_Max(Bloc.MemFaces[i].YT1,Bloc.MemFaces[i].YT2),
				V_Max(Bloc.MemFaces[i].YT3,Bloc.MemFaces[i].YT4)
				)
				);
		}
#endif
	return;
}

#endif


