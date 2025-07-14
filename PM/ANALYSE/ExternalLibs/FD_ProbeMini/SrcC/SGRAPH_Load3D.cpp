
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#include "SPG_Includes.h"

#include <stdio.h>
#include <string.h>
#define MaxTex SG_MAX_TEX


//Type FACE
typedef struct
{
union 
{
	struct
	{
LONG NumP1;
LONG NumP2;
LONG NumP3;
LONG NumP4;
	};
LONG NumP[4];
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
PixCoul Couleur;

WORD Style;
SHORT NumTex;
} F3D_FACE;

#define F3D_UNI 0
#define F3D_UNITRANSL 2
#define F3D_UNITRANSP 3
#define F3D_TEX 8
#define F3D_TEXBIL 9
#define F3D_TEXTRANSL 10
#define F3D_TEXTRANSP 11
#define F3D_TEXPERCE 12

#define F3D_MASKUNI 7
#define F3D_NOTRACE 32

WORD SPG_CONV F3D_GetStyle(WORD FileStyle)
{
	switch(FileStyle)
	{
	case F3D_UNI:
		return SG_UNI;
	case F3D_UNITRANSL:
		return SG_UNITRANSL;
	case F3D_UNITRANSP:
		return SG_UNITRANSP;
	case F3D_TEX:
		return SG_TEX;
	case F3D_TEXBIL:
		return SG_TEX;
	case F3D_TEXTRANSL:
		return SG_TEXTRANSL;
	case F3D_TEXTRANSP:
		//A FINIR
		return SG_TEXTRANSL;
	case F3D_TEXPERCE:
#ifdef SGE_EMC
		//A FINIR
		return SG_RENDER_FIL;
#else
		return SG_TEXPERCE;
#endif
	}
	return SG_NOTRACE;
}

void SPG_CONV SG_PurgeSpaces(char S[64])
{
	for (int i=63;i>=0;i--)
	{
		if (S[i]==32) 
			S[i]=0;
		else
			break;
	}
}

int SPG_CONV SG_GetInfos(int &NombreB,int &NombreP,int &NombreF,int &NombreTex,char *F3D)
{
	NombreB = 0;
	NombreP = 0;
	NombreF = 0;

	FILE *F = fopen(F3D,"rb");
	CHECKTWO(F==0,"SG_GetInfos: Impossible d'ouvrir le fichier",F3D,return 0);
	char S[64];
	CHECK(fread(S,64,1,F)==0,"SG_GetInfos",goto OutE);
	SG_PurgeSpaces(S);
	CHECK(strcmp(S,"spg3d"),"SG_GetInfos",goto OutE);

	{
		LONG OSkip;
		fread(&OSkip,4,1,F);
		CHECK(V_IsBound(OSkip,0,1024)==0,"SG_GetInfos",goto OutE);
		CHECK(fseek(F,OSkip,SEEK_CUR),"SG_GetInfos",goto OutE);
	}
	
	{
		LONG F_NT;
		fread(&F_NT,4,1,F);
		CHECK(V_IsBound(F_NT,0,1024)==0,"SG_GetInfos",goto OutE);
		NombreTex=F_NT;
		CHECK(fseek(F,F_NT*(64+4+4),SEEK_CUR),"SG_GetInfos",goto OutE);
	}
	
	while(1) //condition constante volontaire
	{
		{
			LONG OSkip;
			fread(&OSkip,4,1,F);
			CHECK(V_IsBound(OSkip,0,1024)==0,"SG_GetInfos",goto OutE);
			CHECK(fseek(F,OSkip,SEEK_CUR),"SG_GetInfos",goto OutE);
		}

		CHECK(fseek(F,4+sizeof(SG_VECT),SEEK_CUR),"SG_GetInfos",goto OutE);
		
		{
		DWORD F_NP;
			fread(&F_NP,4,1,F);
			NombreP += F_NP;
		DWORD F_NF;
			fread(&F_NF,4,1,F);
			NombreF += F_NF;
			if ((F_NP)||(F_NF))
			{
				CHECK(fseek(F,F_NP*sizeof(SG_VECT)+F_NF*sizeof(F3D_FACE),SEEK_CUR),"SG_GetInfos",goto OutE);
				NombreB++;
			}
			
			else
				break;
				
		}
	}
	fclose(F);
	return -1;
	
OutE:
	fclose(F);
	return 0;
	
}

//#ifndef SPG_General_PGLib
int SPG_CONV SG_Load3D(SG_PDV &Vue,
			  SG_3D &S3D,
			  char *WorkDir,char *File3D)
{
	int TotalP,TotalF;

	memset(&S3D,0,sizeof(SG_3D));

	char FullName[1024];

	SPG_ConcatPath(FullName,WorkDir,File3D);
	CHECKTWO(SG_GetInfos(S3D.NombreB,TotalP,TotalF,S3D.NombreT,FullName)==0,"SG_Load3D: SG_GetInfos echoue sur le fichier",FullName,return 0);

//attention si la liste des texture a vecu
//ca peut merder
	/*
	for(NumFirstTex=0;NumFirstTex<MaxTex;NumFirstTex++)
		if (Vue.MemTex[NumFirstTex].MemTex==0) break;
	if (NumFirstTex+S3D.NombreT>=MaxTex) return 0;
	//voila ca merdera pas
	*/

	S3D.FB = SPG_TypeAlloc(S3D.NombreB,SG_FullBloc,"DescrBlocs");
	CHECK(S3D.FB==0,"SG_Load3D: Erreur allocation descripteurs de blocs",SG_Unload3D(Vue,S3D);return 0);
	S3D.FT = SPG_TypeAlloc(S3D.NombreT,SG_FullTexture,"DescrTex");
	CHECK(S3D.FT==0,"SG_Load3D: Erreur allocation descripteurs de textures",SG_Unload3D(Vue,S3D);return 0);

	SPG_ConcatPath(FullName,WorkDir,File3D);
	FILE *F = fopen(FullName,"rb");
	CHECK(F==0,"SG_Load3D",SG_Unload3D(Vue,S3D);return 0);
	{
	char S[64];
	CHECK(fread(S,64,1,F)==0,"SG_Load3D",goto OutE);
	SG_PurgeSpaces(S);
	CHECK(strcmp(S,"spg3d"),"SG_Load3D: SPG3D non trouve",goto OutE);
	}
	
	{
		LONG OSkip;
		fread(&OSkip,4,1,F);
		CHECK(V_IsBound(OSkip,0,1024)==0,"SG_Load3D",goto OutE);
		CHECK(fseek(F,OSkip,SEEK_CUR),"SG_Load3D",goto OutE);
	}
	
	{
		int F_NT;
		fread(&F_NT,4,1,F);
		CHECK(V_IsBound(F_NT,0,1024)==0,"SG_Load3D",goto OutE);
		CHECK(S3D.NombreT!=F_NT,"SG_Load3D",goto OutE);
		int i;
		for(i=0; i<S3D.NombreT; i++)
		{
			char S[64];
			fread(S,64,1,F);
			DWORD LarT;
			fread(&LarT,4,1,F);
			CHECK(fseek(F,4,SEEK_CUR),"SG_Load3D",goto OutE);//LarT et SizeY

			SG_PurgeSpaces(S);

			SPG_ConcatPath(FullName,WorkDir,S);
			if (SG_LoadTexture(S3D.FT[i],FullName)) 
			{
#ifndef SPG_General_PGLib
			int NumAttach;
			SG_AttachTexture(Vue,S3D.FT[i],NumAttach=SG_FirstFreeTex(Vue));
			CHECK(LarT!=Vue.MemTex[NumAttach].LarT,"SG_Load3D",goto OutE);
#else
			SG_AttachTexture(Vue,S3D.FT[i],SG_FirstFreeTex(Vue));
#endif
			}
#ifdef DebugList
			else
				SPG_ListSSN("Texture non chargee",FullName,i);
#endif
		}
	}

	int NB;
	for(NB=0; NB<S3D.NombreB; NB++)
	{
		{
			LONG OSkip;
			fread(&OSkip,4,1,F);
			CHECK(V_IsBound(OSkip,0,1024)==0,"SG_Load3D",goto OutE);
			CHECK(fseek(F,OSkip,SEEK_CUR),"SG_Load3D",goto OutE);
		}
		V_VECT BRef;
		float Rayon;
		fread(&BRef,sizeof(SG_VECT),1,F);
		fread(&Rayon,sizeof(float),1,F);
		{
			int F_NP;
			fread(&F_NP,4,1,F);
			int F_NF;
			fread(&F_NF,4,1,F);
			CHECK(SG_CreateBloc(S3D.FB[NB],F_NP,F_NF,SG_WithPOINTS|SG_WithFACES)==0,"SG_Load3D",goto OutE);
			S3D.FB[NB].BRef=BRef;
			S3D.FB[NB].Rayon=Rayon;
			int i;
			for(i=0; i<S3D.FB[NB].DB.NombreP; i++)
			{
				V_VECT FileP;
				fread(&FileP,sizeof(SG_VECT),1,F);//vect et pnt3d ne font pas la meme taille
				S3D.FB[NB].DB.MemPoints[i].P=FileP;//vect et pnt3d ne font pas la meme taille
			}
				//fread(S3D.FB[NB].DB.MemFaces,sizeof(SG_FACE)*S3D.FB[NB].DB.NombreF,1,F);
			for(i=0; i<S3D.FB[NB].DB.NombreF; i++)
			{
				F3D_FACE F3D;
				fread(&F3D,sizeof(F3D_FACE),1,F);
				DbgCHECK(V_IsBound(F3D.NumTex,0,S3D.NombreT)==0,"SG_Load3D: Mauvais index de texture");
				SG_DefTexFce(S3D.FB[NB].DB.MemFaces[i],
					S3D.FB[NB].DB.MemPoints+F3D.NumP1,
					S3D.FB[NB].DB.MemPoints+F3D.NumP2,
					S3D.FB[NB].DB.MemPoints+F3D.NumP3,
					S3D.FB[NB].DB.MemPoints+F3D.NumP4,
					F3D.Couleur.Coul,
					F3D_GetStyle(F3D.Style),
					S3D.FT[F3D.NumTex],
					F3D.XT1,F3D.YT1,
					F3D.XT2,F3D.YT2,
					F3D.XT3,F3D.YT3,
					F3D.XT4,F3D.YT4);

				//if (Vue.MemTex[S3D.FB[NB].DB.MemFaces[i].NumTex += NumFirstTex].MemTex==0) S3D.FB[NB].DB.MemFaces[i].Style&=SG_MASKUNI;

				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].XT1,0,SG_TexSizeX(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee XT1");
				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].YT1,0,SG_TexSizeY(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee YT1");
				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].XT2,0,SG_TexSizeX(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee XT2");
				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].YT2,0,SG_TexSizeY(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee YT2");
				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].XT3,0,SG_TexSizeX(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee XT3");
				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].YT3,0,SG_TexSizeY(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee YT3");
				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].XT4,0,SG_TexSizeX(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee XT4");
				DbgCHECK((V_IsBound(S3D.FB[NB].DB.MemFaces[i].YT4,0,SG_TexSizeY(S3D.FT[F3D.NumTex]))==0)&&(S3D.FB[NB].DB.MemFaces[i].Style&~SG_MASKUNI),"SG_Load3D: Mauvaise coordonnee YT4");

				//DbgCHECK(V_IsBound(S3D.FB[NB].DB.MemFaces[i].NumTex ,0,MaxTex)==0,"SG_Load3D: Mauvais index de texture");
			}
		}
	}
	fclose(F);
	return -1;
	
OutE:
	fclose(F);
	SG_Unload3D(Vue,S3D);
	return 0;
}


void SPG_CONV SG_Unload3D(SG_PDV& Vue, SG_3D& S3D)
{
	int i;
	DbgCHECK(S3D.FB==0,"SG_Unload3D: Descripteur de bloc nul");
	if (S3D.FB)
	{
		for (i=0; i<S3D.NombreB; i++)
		{
		SG_CloseBloc(S3D.FB[i]);
		}
		SPG_MemFree(S3D.FB);
	}
	DbgCHECK(S3D.FT==0,"SG_Unload3D: Descripteur de texture  nul");
	if (S3D.FT)
	{
	for (i=0; i<S3D.NombreT; i++)
	{
	SG_DetachTexture(Vue,S3D.FT[i]);
	SG_CloseTexture(S3D.FT[i]);
	}
		SPG_MemFree(S3D.FT);
	}

	memset(&S3D,0,sizeof(SG_3D));

	return;
}
//#endif

#endif

