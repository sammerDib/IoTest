
#include "SPG_General.h"

#ifdef SPG_General_USEProfil

//#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "SPG_Includes.h"

#ifdef SPG_General_USEFFT
extern float PI,DPI;
#endif


//necessaire aussi pour le unwrapping

/*
point de coupure pour exportation du code
*/

//cree un profil 2D float
int SPG_CONV P_Create(Profil& P, int SizeX, int SizeY, float XScale,float YScale, char* UnitX,char* UnitY,char* UnitZ, int WithMask)
{
	memset(&P,0,sizeof(Profil));
	CHECK(SizeX<=0,"P_Create: Taille X invalide",return 0);
	CHECK(SizeY<=0,"P_Create: Taille Y invalide",return 0);
	IF_CD_G_CHECK(13,return 0);

	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_Data(P)=SPG_TypeAlloc(P_SizeX(P)*P_SizeY(P),float,"ProfilD");
	CHECK(P_Data(P)==0,"P_Create: Allocation memoire echouee",return 0);
	if (WithMask)
	{
	P_Msk(P)=SPG_TypeAlloc(P_SizeX(P)*P_SizeY(P),BYTE,"ProfilMsk");
	CHECK(P_Msk(P)==0,"P_Create: Allocation memoire echouee",SPG_MemFree(P_Data(P));P_Data(P)=0;return 0);
	}
	else
	{
		P_Msk(P)=0;
	}

	P.H.XScale=XScale;
	P.H.YScale=YScale;
	if (UnitX) strncpy(P.H.UnitX,UnitX,MaxUnit-1);
	if (UnitY) strncpy(P.H.UnitY,UnitY,MaxUnit-1);
	if (UnitZ) strncpy(P.H.UnitZ,UnitZ,MaxUnit-1);

	P_Etat(P)=P_WithMEM;
	return -1;
}

//cree un profil 2D byte
int SPG_CONV P8_Create(Profil8& P, int SizeX, int SizeY, float XScale,float YScale, char*UnitX,char*UnitY,char*UnitZ, int WithMask)
{
	P_Etat(P)=0;
	CHECK(SizeX<=0,"P_Create: Taille X invalide",return 0);
	CHECK(SizeY<=0,"P_Create: Taille Y invalide",return 0);
	IF_CD_G_CHECK(21,return 0);

	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_Data(P)=SPG_TypeAlloc(P_SizeX(P)*P_SizeY(P),BYTE,"Profil8D");
	CHECK(P_Data(P)==0,"P_Create: Allocation memoire echouee",return 0);
	if (WithMask)
	{
	P_Msk(P)=SPG_TypeAlloc(P_SizeX(P)*P_SizeY(P),BYTE,"ProfilMsk");
	CHECK(P_Msk(P)==0,"P_Create: Allocation memoire echouee",SPG_MemFree(P_Data(P));P_Data(P)=0;return 0);
	}
	else
	{
		P_Msk(P)=0;
	}

	P_XScale(P)=XScale;
	P_YScale(P)=YScale;
	if (UnitX) strncpy(P_UnitX(P),UnitX,MaxUnit);
	if (UnitY) strncpy(P_UnitY(P),UnitY,MaxUnit);
	if (UnitZ) strncpy(P_UnitZ(P),UnitZ,MaxUnit);

	P_Etat(P)=P_WithMEM|P_BYTE;
	return -1;
}

#ifdef SPG_General_USEFFT
//cree un profil 2D complexe
int SPG_CONV PX_Create(ProfilX& P, int SizeX, int SizeY, float XScale,float YScale, char*UnitX,char*UnitY,char*UnitZ, int WithMask)
{
	IF_CD_G_CHECK(6,return 0);
	CHECK(P_Create((Profil&)P,2*SizeX,SizeY,XScale,YScale,UnitX,UnitY,UnitZ,WithMask)==0,"PX_Create: P_Create echoue",return 0);
	P_SizeX(P)=SizeX;
	P_Etat(P)|=P_COMPLEX;
	return -1;
}

#endif

//initialise un profil avec des donnees existantes, mode=P_WithThisMem ou P_Alias
int SPG_CONV P_Init(Profil& P, float* D, BYTE* Msk, int SizeX, int SizeY, int Mode)
{
	memset(&P,0,sizeof(Profil));
	CHECK((Mode!=P_WithThisMEM)&&(Mode!=P_Alias),"P_Init: Mode invalide",return 0);
	CHECK(SizeX<=0,"P_Init: Taille X invalide",return 0);
	CHECK(SizeY<=0,"P_Init: Taille Y invalide",return 0);
	CHECK(D==0,"P_Init: Donnees nulles",return 0);
	IF_CD_G_CHECK(31,return 0);

	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_Data(P)=D;
	P_Msk(P)=Msk;
	if (Mode==P_WithThisMEM) Mode=P_WithMEM;
	P_Etat(P)=Mode;
	return -1;
}

int SPG_CONV P8_Init(Profil8& P, BYTE* D, int SizeX, int SizeY, int Mode)
{
	memset(&P,0,sizeof(Profil));
	CHECK((Mode!=P_WithThisMEM)&&(Mode!=P_Alias),"P8_Init: Mode invalide",return 0);
	CHECK(SizeX<=0,"P_Init: Taille X invalide",return 0);
	CHECK(SizeY<=0,"P_Init: Taille Y invalide",return 0);
	CHECK(D==0,"P_Init: Donnees nulles",return 0);
	IF_CD_G_CHECK(18,return 0);

	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_Data(P)=D;
	P_Msk(P)=0;
	if (Mode==P_WithThisMEM) Mode=P_WithMEM;
	P_Etat(P)=Mode;
	return -1;
}

void SPG_CONV P_SetScale(Profil& P, float XScale, float YScale, char * UnitX, char * UnitY, char * UnitZ)
{
	P_XScale(P)=XScale;
	P_YScale(P)=YScale;
	if (UnitX) strncpy(P_UnitX(P),UnitX,MaxUnit);
	if (UnitY) strncpy(P_UnitY(P),UnitY,MaxUnit);
	if (UnitZ) strncpy(P_UnitZ(P),UnitZ,MaxUnit);
	return;
}

void SPG_CONV P_Close(Profil& P)
{
	if (P_Etat(P)&P_WithMEM)
	{
		DbgCHECK(P_Data(P)==0,"P_Close: Profil vide")
		if (P_Data(P))
			SPG_MemFree(P_Data(P));
		if (P_Msk(P))
			SPG_MemFree(P_Msk(P));
	}
	/*
	P_Etat(P)=0;
	P_Data(P)=0;
	P_Msk(P)=0;
	*/
	memset(&P,0,sizeof(Profil));
	return;
}

void SPG_CONV P8_Close(Profil8& P)
{
	if (P_Etat(P)&P_WithMEM)
	{
		DbgCHECK(P_Data(P)==0,"P_Close: Profil vide")
		if (P_Data(P))
			SPG_MemFree(P_Data(P));
		if (P_Msk(P))
			SPG_MemFree(P_Msk(P));
	}
	/*
	P_Etat(P)=0;
	P_Data(P)=0;
	P_Msk(P)=0;
	*/
	memset(&P,0,sizeof(Profil8));
	return;
}

#ifdef SPG_General_USEFFT

void SPG_CONV PX_Close(ProfilX& P)
{
	if (P_Etat(P)&P_WithMEM)
	{
		DbgCHECK(P_Data(P)==0,"PX_Close: Profil vide")
		if (P_Data(P))
			SPG_MemFree(P_Data(P));
		if (P_Msk(P))
			SPG_MemFree(P_Msk(P));
	}
	memset(&P,0,sizeof(ProfilX));
	return;
}

int SPG_CONV P_CreateForFFT(Profil& P,ProfilX& PFFT, int FFT_FLAG)
{
	P_SizeY(PFFT)=P_SizeX(PFFT)=SFFT_GetAppropriateSize(P_SizeX(P),FFT_FLAG);
	char UnitX[2*MaxUnit];
	strcpy(UnitX,"rad/");
	strncat(UnitX,P_UnitX(P),MaxUnit);
	char UnitY[2*MaxUnit];
	strcpy(UnitY,"rad/");
	strncat(UnitY,P_UnitY(P),MaxUnit);
	float XScale;
	CHECK((XScale=P_XScale(P))==0,"P_CreateForFFT: Profil sans echelles",XScale=1);
	float YScale;
	CHECK((YScale=P_YScale(P))==0,"P_CreateForFFT: Profil sans echelles",YScale=1);
	return PX_Create(PFFT,P_SizeX(PFFT),P_SizeY(PFFT),(float)V_2PI/(P_SizeX(PFFT)*XScale),(float)V_2PI/(P_SizeY(PFFT)*YScale),UnitX,UnitY,P_UnitZ(P),0);
}

int SPG_CONV P_CopyToComplex(Profil& P,ProfilX& PFFT)
{
	CHECK(P_Etat(P)==0,"P_CopyToComplex: Profil source nul",return 0);
	CHECK(P_Data(P)==0,"P_CopyToComplex: Profil source vide",return 0);
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil destination vide",return 0);
	memset(P_Data(PFFT),0,P_SizeX(PFFT)*P_SizeY(PFFT)*sizeof(SPG_COMPLEX));
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	IF_CD_G_CHECK(19,return 0);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			Dest[x].re=Src[x];
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}

//attention dans les fonctions getxx src est la destination(float) et dest la source(spg_complex)
int SPG_CONV PX_GetRe(ProfilX& PFFT,Profil& P)
{
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil source vide",return 0);
	CHECK(P_Etat(P)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(P)==0,"P_GetModule: Profil destination vide",return 0);
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			Src[x]=Dest[x].re;
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}

int SPG_CONV PX_GetIm(ProfilX& PFFT,Profil& P)
{
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil source vide",return 0);
	CHECK(P_Etat(P)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(P)==0,"P_GetModule: Profil destination vide",return 0);
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			Src[x]=Dest[x].im;
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}


//sort la phase entre -pi inclus et pi non inclus
int SPG_CONV PX_GetPhase(ProfilX& PFFT,Profil& P)
{
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil source vide",return 0);
	CHECK(P_Etat(P)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(P)==0,"P_GetModule: Profil destination vide",return 0);
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
#define DefSPGArg1 Dest[x]
#define DefSPGArg2 Src[x]
#include "SPG_Argum_Inline.cpp"
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}

//sort la phase entre 0 inclus et 2pi non inclus
int SPG_CONV PX_GetPhase_0_2pi(ProfilX& PFFT,Profil& P)
{
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil source vide",return 0);
	CHECK(P_Etat(P)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(P)==0,"P_GetModule: Profil destination vide",return 0);
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
#define DefSPGArg1 Dest[x]
#define DefSPGArg2 Src[x]
#include "SPG_Argum_Inline_0_2pi.cpp"
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}

//sort la phase entre 0 inclus et 1 non inclus
int SPG_CONV PX_GetPhase_0_1(ProfilX& PFFT,Profil& P)
{
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil source vide",return 0);
	CHECK(P_Etat(P)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(P)==0,"P_GetModule: Profil destination vide",return 0);
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
#define DefSPGArg1 Dest[x]
#define DefSPGArg2 Src[x]
#include "SPG_Argum_Inline_0_1.cpp"
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}

int SPG_CONV PX_GetModule(ProfilX& PFFT,Profil& P)
{
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil source vide",return 0);
	CHECK(P_Etat(P)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(P)==0,"P_GetModule: Profil destination vide",return 0);
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			Src[x]=CX_Module(Dest[x]);
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}

int SPG_CONV PX_GetModuleAtNthPower(ProfilX& PFFT,Profil& P,float Power)
{
	CHECK(P_Etat(PFFT)==0,"P_GetModule: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_GetModule: Profil source vide",return 0);
	CHECK(P_Etat(P)==0,"P_GetModule: Profil destination nul",return 0);
	CHECK(P_Data(P)==0,"P_GetModule: Profil destination vide",return 0);
	int SizeX=V_Min(PX_SizeX(PFFT),P_SizeX(P));
	int SizeY=V_Min(PX_SizeY(PFFT),P_SizeY(P));
	int xsrc=V_Max(0,(P_SizeX(P)-SizeX)>>1);
	int ysrc=V_Max(0,(P_SizeY(P)-SizeY)>>1);
	int xdest=V_Max(0,(PX_SizeX(PFFT)-SizeX)>>1);
	int ydest=V_Max(0,(PX_SizeY(PFFT)-SizeY)>>1);
	float T=0.5f*Power;
	float* Src=P_Pointeur(P,xsrc,ysrc);
	SPG_COMPLEX* Dest=PX_Pointeur(PFFT,xdest,ydest);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			Src[x]=pow(CX_Module2(Dest[x]),T);
		}
		Src+=P_SizeX(P);
		Dest+=PX_SizeX(PFFT);
	}
	return -1;
}

#ifdef SPG_General_USEFFT
int SPG_CONV P_FFT(Profil& P,ProfilX& PFFT,int Flag)
{
	CHECK(P_Etat(P)==0,"P_FFT: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_FFT: Profil nul",return 0);
	CHECK(P_Etat(PFFT)==0,"P_FFT: Profil nul",return 0);
	CHECK(P_Data(PFFT)==0,"P_FFT: Profil nul",return 0);
	IF_CD_G_CHECK(17,return 0);

	P_CopyToComplex(P,PFFT);

	int y;
	for(y=0;y<P_SizeY(PFFT);y++)
	{
		SFFT_GENERAL(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
		SFFTSWAP(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
	}

	for(y=0;y<V_Min(P_SizeX(PFFT),P_SizeY(PFFT));y++)
	{
		for(int x=0;x<y;x++)
		{
			V_Swap(SPG_COMPLEX,P_Element(PFFT,x,y),P_Element(PFFT,y,x));
		}
	}

	for(y=0;y<P_SizeY(PFFT);y++)
	{
		SFFT_GENERAL(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
		SFFTSWAP(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
	}

	if ((Flag&P_NOTRANSPOSE)==0)
	{
	for(y=0;y<V_Min(P_SizeX(PFFT),P_SizeY(PFFT));y++)
		for(int x=0;x<y;x++)
		{
			V_Swap(SPG_COMPLEX,P_Data(PFFT)[x+P_SizeX(PFFT)*y],P_Data(PFFT)[y+P_SizeX(PFFT)*x]);
		}
	}

	return -1;
}

int SPG_CONV PX_FFT(ProfilX& PFFT,int Flag)
{
	CHECK(P_Etat(PFFT)==0,"PX_FFT: Profil nul",return 0);
	CHECK(P_Data(PFFT)==0,"PX_FFT: Profil nul",return 0);
	IF_CD_G_CHECK(28,return 0);

	int y;
	for(y=0;y<P_SizeY(PFFT);y++)
	{
		SFFT_GENERAL(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
		SFFTSWAP(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
	}

	for(y=0;y<V_Min(P_SizeX(PFFT),P_SizeY(PFFT));y++)
	{
		for(int x=0;x<y;x++)
		{
			V_Swap(SPG_COMPLEX,P_Element(PFFT,x,y),P_Element(PFFT,y,x));
		}
	}

	for(y=0;y<P_SizeY(PFFT);y++)
	{
		SFFT_GENERAL(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
		SFFTSWAP(P_Data(PFFT)+y*P_SizeX(PFFT),P_SizeX(PFFT));
	}

	if ((Flag&P_NOTRANSPOSE)==0)
	{
	for(y=0;y<V_Min(P_SizeX(PFFT),P_SizeY(PFFT));y++)
		for(int x=0;x<y;x++)
		{
			V_Swap(SPG_COMPLEX,P_Data(PFFT)[x+P_SizeX(PFFT)*y],P_Data(PFFT)[y+P_SizeX(PFFT)*x]);
		}
	}

	return -1;
}
#endif

int SPG_CONV PX_CircularFourier(ProfilX& PFFT, int mx, int my, int RadiusSquare)
{
	CHECK(P_Etat(PFFT)==0,"PX_CircularFourier: Profil source nul",return 0);
	CHECK(P_Data(PFFT)==0,"PX_CircularFourier: Profil source vide",return 0);
	CHECK(mx*mx+my*my<=RadiusSquare,"PX_CircularFourier: L'origine est incluse",return 0);

	for(int y=0;y<P_SizeY(PFFT);y++)
	{
		int dy=y-my;
		if(2*dy>P_SizeY(PFFT)) dy-=P_SizeY(PFFT);
		if(2*dy<-P_SizeY(PFFT)) dy+=P_SizeY(PFFT);
		for(int x=0;x<P_SizeX(PFFT);x++)
		{
			int dx=x-mx;
			if(2*dx>P_SizeY(PFFT)) dx-=P_SizeX(PFFT);
			if(2*dx<-P_SizeY(PFFT)) dx+=P_SizeX(PFFT);
			if(dx*dx+dy*dy>RadiusSquare) 
				P_Element(PFFT,x,y).im=P_Element(PFFT,x,y).re=0;
			else
				P_Element(PFFT,x,y).im=-P_Element(PFFT,x,y).im;
		}
	}

	return -1;
}

void SPG_CONV PX_TranslateToCenter_FFT_AlternateSign(ProfilX& PFFT)
{
	CHECK(P_Etat(PFFT)==0,"PX_TranslateToCenter_FFT_AlternateSign: Profil invalide",return);
	for(int y=0;y<P_SizeY(PFFT);y++)
	{
		for(int x=(y&1);x<P_SizeX(PFFT);x+=2)
		{
			PFFT.D[x+P_SizeX(PFFT)*y].re=-PFFT.D[x+P_SizeX(PFFT)*y].re;
			PFFT.D[x+P_SizeX(PFFT)*y].im=-PFFT.D[x+P_SizeX(PFFT)*y].im;
		}
	}

	return;
}

int SPG_CONV PX_FindCarrier(ProfilX& PFFT, int& X, int& Y, int MaxSizeX, int MaxSizeY, int AvoidSize)
{
	CHECK(PX_Etat(PFFT)==0,"PX_FindCarrier: Profil nul",return 0);
	CHECK(MaxSizeX>PX_SizeX(PFFT),"PX_FindCarrier: Frequence max X specifiee trop grande",return 0);
	CHECK(MaxSizeY>PX_SizeY(PFFT),"PX_FindCarrier: Frequence max Y specifiee trop grande",return 0);
	float Modulation=0;
	for(int y=0;y<MaxSizeY;y++)
	{
		SPG_COMPLEX* SrcL=PX_Data(PFFT)+y*PX_SizeX(PFFT);
		for(int x=0;x<MaxSizeX;x++)
		{
			if((x>AvoidSize)||(y>AvoidSize))
			{
				float Module2=CX_Module2(SrcL[x]);
				if(Module2>Modulation)
				{
					Modulation=Module2;
					X=x;
					Y=y;
				}
			}
		}
	}
	return -1;
}

int SPG_CONV PX_Mul(ProfilX& P, Profil& Factor)
{
	CHECK(PX_Etat(P)==0,"PX_Mul: Profil nul",return 0);
	CHECK(PX_Data(P)==0,"PX_Mul: Profil nul",return 0);
	CHECK(P_Etat(Factor)==0,"PX_Mul: Profil nul",return 0);
	CHECK(P_Data(Factor)==0,"PX_Mul: Profil nul",return 0);
	CHECK(P_SizeX(P)!=P_SizeX(Factor),"PX_Mul: Tailles inegales",return 0);
	CHECK(P_SizeY(P)!=P_SizeY(Factor),"PX_Mul: Tailles inegales",return 0);

	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		PX_Data(P)[i].re*=P_Data(Factor)[i];
		PX_Data(P)[i].im*=P_Data(Factor)[i];
	}
	return -1;
}

int SPG_CONV PX_MulComplement(ProfilX& PRes, ProfilX& PRef)
{
	CHECK(P_Etat(PRes)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_Data(PRes)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_Etat(PRef)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_Data(PRef)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_SizeX(PRes)!=P_SizeX(PRef),"PX_Intercorrelle: Tailles inegales",return 0);
	CHECK(P_SizeY(PRes)!=P_SizeY(PRef),"PX_Intercorrelle: Tailles inegales",return 0);

	for(int y=0;y<P_SizeY(PRes);y++)
	{
		for(int x=0;x<P_SizeX(PRes);x++)
		{
			SPG_COMPLEX Res=P_Element(PRes,x,y);
			SPG_COMPLEX Ref=P_Element(PRef,x,y);
			CX_MulComplement(Res,Ref,P_Element(PRes,x,y));
		}
	}
	return -1;
}

int SPG_CONV PX_ComplementMul(ProfilX& PRes, ProfilX& PRef)
{
	CHECK(P_Etat(PRes)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_Data(PRes)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_Etat(PRef)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_Data(PRef)==0,"PX_Intercorrelle: Profil nul",return 0);
	CHECK(P_SizeX(PRes)!=P_SizeX(PRef),"PX_Intercorrelle: Tailles inegales",return 0);
	CHECK(P_SizeY(PRes)!=P_SizeY(PRef),"PX_Intercorrelle: Tailles inegales",return 0);

	for(int y=0;y<P_SizeY(PRes);y++)
	{
		for(int x=0;x<P_SizeX(PRes);x++)
		{
			SPG_COMPLEX Res=P_Element(PRes,x,y);
			SPG_COMPLEX Ref=P_Element(PRef,x,y);
			CX_MulComplement(Ref,Res,P_Element(PRes,x,y));
		}
	}
	return -1;
}

#endif

int SPG_CONV P_CopyAt(float* dD, int dStepXFloat, int dPitchFloat, int dPosX, int dPosY,
			 float* sD, int sStepXFloat, int sPitchFloat, int sPosX, int sPosY, int SizeX, int SizeY)
{
	CHECK(dD==0,"P_CopyAt: Destination nulle",return 0);
	CHECK(sD==0,"P_CopyAt: Source nulle",return 0);
	CHECK(dPosX<0,"P_CopyAt: Destination X incorrecte",return 0);
	CHECK(dPosY<0,"P_CopyAt: Destination Y incorrecte",return 0);
	CHECK(sPosX<0,"P_CopyAt: Source X incorrecte",return 0);
	CHECK(sPosY<0,"P_CopyAt: Source Y incorrecte",return 0);

	dD+=dPosX+dPitchFloat*dPosY;
	sD+=sPosX+sPitchFloat*sPosY;

	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			dD[dStepXFloat*x]=sD[sStepXFloat*x];
		}
		dD+=dPitchFloat;
		sD+=sPitchFloat;
	}

	return -1;
}


int SPG_CONV P8_CopyAt(BYTE* dD, int dStepXByte, int dPitchByte, int dPosX, int dPosY,
			 BYTE* sD, int sStepXByte, int sPitchByte, int sPosX, int sPosY, int SizeX, int SizeY)
{
	CHECK(dD==0,"P8_CopyAt: Destination nulle",return 0);
	CHECK(sD==0,"P8_CopyAt: Source nulle",return 0);
	CHECK(dPosX<0,"P8_CopyAt: Destination X incorrecte",return 0);
	CHECK(dPosY<0,"P8_CopyAt: Destination Y incorrecte",return 0);
	CHECK(sPosX<0,"P8_CopyAt: Source X incorrecte",return 0);
	CHECK(sPosY<0,"P8_CopyAt: Source Y incorrecte",return 0);

	dD+=dPosX+dPitchByte*dPosY;
	sD+=sPosX+sPitchByte*sPosY;

	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			dD[dStepXByte*x]=sD[sStepXByte*x];
		}
		dD+=dPitchByte;
		sD+=sPitchByte;
	}

	return -1;
}
/*
int SPG_CONV P_CopyAt(float* sD, int sPasXFloat, int sPitchFloat,
			 int sPosX, int sPosY, int sSizeX, int sSizeY,
			 float* dD, int dPasXFloat, int dPitchFloat,
			 int dPosX, int dPosY, int dSizeX, int dSizeY)
{
	if (sPosX<0)
	{
		dPosX-=sPosX;
		sPosX=0;
	}
	if (sPosY<0)
	{
		dPosY-=sPosY;
		sPosY=0;
	}
	if (dPosX<0)
	{
		sPosX-=dPosX;
		dPosX=0;
	}
	if (dPosY<0)
	{
		sPosY-=dPosY;
		dPosY=0;
	}

	int CPySizeX=V_Min((sSizeX-sPosX),(dSizeX-dPosX));
	CHECK(CPySizeX<=0,"P_CopyAt: Rien a copier selon X",return 0);
	int CPySizeY=V_Min((sSizeY-sPosY),(dSizeY-dPosY));
	CHECK(CPySizeY<=0,"P_CopyAt: Rien a copier selon Y",return 0);

	sD+=sPosX*sPasXFloat+sPosY*sPitchFloat;
	dD+=dPosX*dPasXFloat+dPosY*dPitchFloat;
	for(int y=0;y<CPySizeY;y++)
	{
		for(int x=0;x<CPySizeX;x++)
		{
			dD[dPasXFloat*x]=sD[sPasXFloat*x];
		}
		sD+=sPitchFloat;
		dD+=dPitchFloat;
	}

	return -1;
}

int SPG_CONV P8_CopyAt(BYTE* sD, int sPasXByte, int sPitchByte,
			 int sPosX, int sPosY, int sSizeX, int sSizeY,
			 BYTE* dD, int dPasXByte, int dPitchByte,
			 int dPosX, int dPosY, int dSizeX, int dSizeY)
{
	if (sPosX<0)
	{
		dPosX-=sPosX;
		sPosX=0;
	}
	if (sPosY<0)
	{
		dPosY-=sPosY;
		sPosY=0;
	}
	if (dPosX<0)
	{
		sPosX-=dPosX;
		dPosX=0;
	}
	if (dPosY<0)
	{
		sPosY-=dPosY;
		dPosY=0;
	}

	int CPySizeX=V_Min((sSizeX-sPosX),(dSizeX-dPosX));
	CHECK(CPySizeX<=0,"P8_CopyAt: Rien a copier selon X",return 0);
	int CPySizeY=V_Min((sSizeY-sPosY),(dSizeY-dPosY));
	CHECK(CPySizeY<=0,"P8_CopyAt: Rien a copier selon Y",return 0);

	sD+=sPosX*sPasXByte+sPosY*sPitchByte;
	dD+=dPosX*dPasXByte+dPosY*dPitchByte;
	for(int y=0;y<CPySizeY;y++)
	{
		for(int x=0;x<CPySizeX;x++)
		{
			dD[dPasXByte*x]=sD[sPasXByte*x];
		}
		sD+=sPitchByte;
		dD+=dPitchByte;
	}

	return -1;
}
*/

void SPG_CONV P_Dupliquate(Profil& Pdest, Profil& Psrc)
{
	memset(&Pdest,0,sizeof(Profil));
	CHECK(P_Etat(Psrc)==0,"P_Dupliquate: Le profil source est nul",return);
	CHECK(P_Create(Pdest,Psrc.H.SizeX,Psrc.H.SizeY,Psrc.H.XScale,Psrc.H.YScale,Psrc.H.UnitX,Psrc.H.UnitY,Psrc.H.UnitZ,(int)P_Msk(Psrc))==0,"P_Dupliquate: P_Create echoue",return);
	SPG_MemCatName(Pdest.D,"dup");
	P_Copy(Pdest,Psrc);
	return;
}

void SPG_CONV P_DupliquateWithNoMsk(Profil& Pdest, Profil& Psrc)
{
	memset(&Pdest,0,sizeof(Profil));
	CHECK(P_Etat(Psrc)==0,"P_Dupliquate: Le profil source est nul",return);
	CHECK(P_Create(Pdest,Psrc.H.SizeX,Psrc.H.SizeY,Psrc.H.XScale,Psrc.H.YScale,Psrc.H.UnitX,Psrc.H.UnitY,Psrc.H.UnitZ,0)==0,"P_Dupliquate: P_Create echoue",return);
	P_Copy(Pdest,Psrc);
	return;
}

void SPG_CONV P_DupliquateWithMsk(Profil& Pdest, Profil& Psrc)
{
	memset(&Pdest,0,sizeof(Profil));
	CHECK(P_Etat(Psrc)==0,"P_Dupliquate: Le profil source est nul",return);
	CHECK(P_Create(Pdest,Psrc.H.SizeX,Psrc.H.SizeY,Psrc.H.XScale,Psrc.H.YScale,Psrc.H.UnitX,Psrc.H.UnitY,Psrc.H.UnitZ,1)==0,"P_Dupliquate: P_Create echoue",return);
	P_Copy(Pdest,Psrc);
	return;
}

void SPG_CONV P_Copy(Profil& Pdest, Profil& Psrc)
{
	int y;
	CHECK(((P_Etat(Pdest))&&(P_Etat(Psrc))&&(P_Data(Pdest))&&(P_Data(Psrc)))==0,"P_Copy: L'un au moins des profils est nul",return);
	for(y=0;y<V_Min(P_SizeY(Pdest),P_SizeY(Psrc));y++)
	{
		int dD=y*P_SizeX(Pdest);
		int dS=y*P_SizeX(Psrc);
		for(int x=0;x<V_Min(P_SizeX(Pdest),P_SizeX(Psrc));x++)
		{
			Pdest.D[dD+x]=Psrc.D[dS+x];
		}
	}

	if((Pdest.Msk)&&(Psrc.Msk))
	{
	for(y=0;y<V_Min(P_SizeY(Pdest),P_SizeY(Psrc));y++)
	{
		int dD=y*P_SizeX(Pdest);
		int dS=y*P_SizeX(Psrc);
		for(int x=0;x<V_Min(P_SizeX(Pdest),P_SizeX(Psrc));x++)
		{
			Pdest.Msk[dD+x]=Psrc.Msk[dS+x];
		}
	}
	}

	return;
}

void SPG_CONV P_ResamCopy(Profil& Pdest, Profil& Psrc)
{
	CHECK(((P_Etat(Pdest))&&(P_Etat(Psrc))&&(P_Data(Pdest))&&(P_Data(Psrc)))==0,"P_ResamCopy: L'un au moins des profils est nul",return);
	for(int y=0;y<P_SizeY(Pdest);y++)
	{
		float* dD=P_Data(Pdest)+y*P_SizeX(Pdest);

		float fy=(y*(P_SizeY(Psrc)-1.0001))/(float)(P_SizeY(Pdest)-1);
		int oy=V_Floor(fy);
		float ay=fy-oy;

		float* dS=P_Data(Psrc) + oy*P_SizeX(Psrc);

		float fx=0;//(x*(P_SizeX(Psrc)-1.0001))/(float)(P_SizeX(Pdest)-1);
		float ifx=(P_SizeX(Psrc)-1.0001)/(float)(P_SizeX(Pdest)-1);
		for(int x=0;x<P_SizeX(Pdest);x++)
		{
			//Pdest.D[dD+x]=Psrc.D[dS+x];


			//float fx=(x*(P_SizeX(Psrc)-1.0001))/(float)(P_SizeX(Pdest)-1);
			int ox=V_Floor(fx);
			float ax=fx-ox;
			fx+=ifx;

			float& D00=dS[  ox   ];
			float& D10=dS[(ox+1) ];
			float& D01=dS[  ox   + P_SizeX(Psrc)];
			float& D11=dS[(ox+1) + P_SizeX(Psrc)];

			float DX0=D00*(1-ax)+D10*ax;
			float DX1=D01*(1-ax)+D11*ax;

			dD[x]=DX0*(1-ay)+DX1*ay;
		}
	}

	return;
}

#ifdef SPG_General_USEFFT

void SPG_CONV PX_Copy(ProfilX& Pdest, ProfilX& Psrc)
{
	int y;
	CHECK(((P_Etat(Pdest))&&(P_Etat(Psrc))&&(P_Data(Pdest))&&(P_Data(Psrc)))==0,"PX_Copy: L'un au moins des profils est nul",return);
	for(y=0;y<V_Min(P_SizeY(Pdest),P_SizeY(Psrc));y++)
	{
		int dD=y*P_SizeX(Pdest);
		int dS=y*P_SizeX(Psrc);
		for(int x=0;x<V_Min(P_SizeX(Pdest),P_SizeX(Psrc));x++)
		{
			Pdest.D[dD+x]=Psrc.D[dS+x];
		}
	}

	if((Pdest.Msk)&&(Psrc.Msk))
	{
	for(y=0;y<V_Min(P_SizeY(Pdest),P_SizeY(Psrc));y++)
	{
		int dD=y*P_SizeX(Pdest);
		int dS=y*P_SizeX(Psrc);
		for(int x=0;x<V_Min(P_SizeX(Pdest),P_SizeX(Psrc));x++)
		{
			Pdest.Msk[dD+x]=Psrc.Msk[dS+x];
		}
	}
	}

	return;
}

int SPG_CONV PX_PadCopy(ProfilX& PDest, ProfilX& PSrc)
{
	CHECK(P_Etat(PDest)==0,"PX_PadCopy: Profil nul",return 0);
	CHECK(P_Etat(PSrc)==0,"PX_PadCopy: Profil nul",return 0);
	CHECK(P_Data(PDest)==0,"PX_PadCopy: Profil nul",return 0);
	CHECK(P_Data(PSrc)==0,"PX_PadCopy: Profil nul",return 0);

	PX_Clear(PDest);

	int LenY=V_Min(P_SizeY(PDest),P_SizeY(PSrc));
	LenY>>=1;
	int LenX=V_Min(P_SizeX(PDest),P_SizeX(PSrc));
	LenX>>=1;

	for(int y=0;y<LenY;y++)
	{
		for(int x=0;x<LenX;x++)
		{
			PX_Element(PDest,
				x,
				y)=
				PX_Element(PSrc,
				x,
				y);
			PX_Element(PDest,
				x+P_SizeX(PDest)-LenX,
				y)=
				PX_Element(PSrc,
				x+P_SizeX(PSrc)-LenX,
				y);
			PX_Element(PDest,
				x,
				y+P_SizeY(PDest)-LenY)=
				PX_Element(PSrc,
				x,
				y+P_SizeY(PSrc)-LenY);
			PX_Element(PDest,
				x+P_SizeX(PDest)-LenX,
				y+P_SizeY(PDest)-LenY)=
				PX_Element(PSrc,
				x+P_SizeX(PSrc)-LenX,
				y+P_SizeY(PSrc)-LenY);
		}
	}

	return -1;
}

void SPG_CONV P_CosWindow(Profil& P, int BorderSize)
{
	CHECK(P_Etat(P)==0,"P_CosWindow",return);
	int T=BorderSize;
	for(int y=0;y<P_SizeY(P);y++)
	{
		int x;
		for(x=0;x<BorderSize;x++)
		{
			float C=1;
			if(x<T) C*=0.5f-0.5f*cos(PI*x/T);
			if(y<T) C*=0.5f-0.5f*cos(PI*y/T);
			if((P_SizeX(P)-x)<T) C*=0.5f-0.5f*cos(PI*(P_SizeX(P)-x)/T);
			if((P_SizeY(P)-y)<T) C*=0.5f-0.5f*cos(PI*(P_SizeY(P)-y)/T);
			P_Element(P,x,y)*=C;
		}
		for(x=P_SizeX(P)-BorderSize;x<P_SizeX(P);x++)
		{
			float C=1;
			if(x<T) C*=0.5f-0.5f*cos(PI*x/T);
			if(y<T) C*=0.5f-0.5f*cos(PI*y/T);
			if((P_SizeX(P)-x)<T) C*=0.5f-0.5f*cos(PI*(P_SizeX(P)-x)/T);
			if((P_SizeY(P)-y)<T) C*=0.5f-0.5f*cos(PI*(P_SizeY(P)-y)/T);
			P_Element(P,x,y)*=C;
		}
	}
	return;
}

void SPG_CONV P_EstimateSpectrum(Profil& P, Profil& PEnveloppe, int BorderSize)
{
	CHECK((P_Etat(P)==0)||(P_Etat(PEnveloppe)==0),"P_EstimateSpectrum",return);
	CHECK(P_SizeX(PEnveloppe)!=P_SizeY(PEnveloppe),"P_EstimateSpectrum",return);
	P_Clear(PEnveloppe);
	int NumX=1+P_SizeX(P)/(P_SizeX(PEnveloppe)-2*BorderSize);
	int NumY=1+P_SizeY(P)/(P_SizeY(PEnveloppe)-2*BorderSize);
	ProfilX PFFT;
	PX_Create(PFFT,P_SizeX(PEnveloppe),P_SizeY(PEnveloppe));
	for(int nx=0;nx<NumX;nx++)
	{
		int xsrc=(nx*(P_SizeX(P)-P_SizeX(PEnveloppe)))/(NumX-1);
		for(int ny=0;ny<NumY;ny++)
		{
			int ysrc=(ny*(P_SizeY(P)-P_SizeY(PEnveloppe)))/(NumY-1);
			for(int y=0;y<P_SizeY(PEnveloppe);y++)
			{
				for(int x=0;x<P_SizeX(PEnveloppe);x++)
				{
					float C=1;
					if(x<BorderSize) C*=0.5f-0.5f*cos(PI*x/BorderSize);
					if(y<BorderSize) C*=0.5f-0.5f*cos(PI*y/BorderSize);
					if((P_SizeX(PEnveloppe)-x)<BorderSize) C*=0.5f-0.5f*cos(PI*(P_SizeX(PEnveloppe)-x)/BorderSize);
					if((P_SizeY(PEnveloppe)-y)<BorderSize) C*=0.5f-0.5f*cos(PI*(P_SizeY(PEnveloppe)-y)/BorderSize);

					PX_Data(PFFT)[x+y*P_SizeX(PFFT)].re=P_Data(P)[(x+xsrc)+(y+ysrc)*P_SizeX(P)]*C;
					PX_Data(PFFT)[x+y*P_SizeX(PFFT)].im=0;
				}
			}
			PX_TranslateToCenter_FFT_AlternateSign(PFFT);
			PX_FFT(PFFT);
			P_ForAll(PEnveloppe,i,P_Data(PEnveloppe)[i]+=PX_Data(PFFT)[i].re*PX_Data(PFFT)[i].re+PX_Data(PFFT)[i].im*PX_Data(PFFT)[i].im);
		}
	}
	PX_Close(PFFT);
	P_ForAll(PEnveloppe,i,P_Data(PEnveloppe)[i]=sqrt(P_Data(PEnveloppe)[i]/(NumX*NumY)));
	return;
}

void SPG_CONV P_FFTFilter(Profil& Pdst, Profil& Psrc, Profil& PEnveloppe, int BorderSize)
{
	CHECK((P_Etat(Pdst)==0)||(P_Etat(Psrc)==0)||(P_Etat(PEnveloppe)==0),"P_EstimateSpectrum",return);
	CHECK(P_SizeX(PEnveloppe)!=P_SizeY(PEnveloppe),"P_EstimateSpectrum",return);
	CHECK(P_SizeX(Pdst)!=P_SizeX(Psrc),"P_EstimateSpectrum",return);
	CHECK(P_SizeY(Pdst)!=P_SizeY(Psrc),"P_EstimateSpectrum",return);
	P_Clear(Pdst);
	int NumX=1+P_SizeX(Psrc)/(P_SizeX(PEnveloppe)-2*BorderSize);
	int NumY=1+P_SizeY(Psrc)/(P_SizeY(PEnveloppe)-2*BorderSize);
	int DestBorderSize=(V_Min(
		(NumX*P_SizeX(PEnveloppe)-P_SizeX(Psrc))/NumX,
		(NumY*P_SizeY(PEnveloppe)-P_SizeY(Psrc))/NumY))/2-1;
	ProfilX PFFT;
	PX_Create(PFFT,P_SizeX(PEnveloppe),P_SizeY(PEnveloppe));
	for(int nx=0;nx<NumX;nx++)
	{
		int xsrc=(nx*(P_SizeX(Psrc)-P_SizeX(PEnveloppe)))/(NumX-1);
		for(int ny=0;ny<NumY;ny++)
		{
			int ysrc=(ny*(P_SizeY(Psrc)-P_SizeY(PEnveloppe)))/(NumY-1);
			int y;
			for(y=0;y<P_SizeY(PEnveloppe);y++)
			{
				for(int x=0;x<P_SizeX(PEnveloppe);x++)
				{
					float C=1;
					if(x<BorderSize) C*=0.5f-0.5f*cos(PI*x/BorderSize);
					if(y<BorderSize) C*=0.5f-0.5f*cos(PI*y/BorderSize);
					if((P_SizeX(PEnveloppe)-x)<BorderSize) C*=0.5f-0.5f*cos(PI*(P_SizeX(PEnveloppe)-x)/BorderSize);
					if((P_SizeY(PEnveloppe)-y)<BorderSize) C*=0.5f-0.5f*cos(PI*(P_SizeY(PEnveloppe)-y)/BorderSize);

					PX_Data(PFFT)[x+y*P_SizeX(PFFT)].re=P_Data(Psrc)[(x+xsrc)+(y+ysrc)*P_SizeX(Psrc)]*C;
					PX_Data(PFFT)[x+y*P_SizeX(PFFT)].im=0;
				}
			}
			PX_TranslateToCenter_FFT_AlternateSign(PFFT);
			PX_FFT(PFFT);
			P_ForAll(PEnveloppe,i,PX_Data(PFFT)[i].re*=P_Data(PEnveloppe)[i];PX_Data(PFFT)[i].im*=-P_Data(PEnveloppe)[i]);
			PX_FFT(PFFT);
			PX_TranslateToCenter_FFT_AlternateSign(PFFT);
			for(y=DestBorderSize;y<P_SizeY(PEnveloppe)-DestBorderSize;y++)
			{
				for(int x=DestBorderSize;x<P_SizeX(PEnveloppe)-DestBorderSize;x++)
				{
					P_Data(Pdst)[(x+xsrc)+(y+ysrc)*P_SizeX(Pdst)]=PX_Data(PFFT)[x+y*P_SizeX(PFFT)].re;
				}
			}
		}
	}
	PX_Close(PFFT);
	return;
}

void SPG_CONV P_Equalize(Profil& P, float Epsilon, int BorderSize, int Iter)
{
	P_RemoveOffset(P);
	int T=BorderSize;
	int y;
	for(y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			float C=1;
			if(x<T) C*=0.5f-0.5f*cos(PI*x/T);
			if(y<T) C*=0.5f-0.5f*cos(PI*y/T);
			if((P_SizeX(P)-x)<T) C*=0.5f-0.5f*cos(PI*(P_SizeX(P)-x)/T);
			if((P_SizeY(P)-y)<T) C*=0.5f-0.5f*cos(PI*(P_SizeY(P)-y)/T);
			P_Element(P,x,y)*=C;
		}
	}

	ProfilX PFFT;
	int S=SFFT_GetAppropriateSize(V_Max(P_SizeX(P),P_SizeY(P)),FFT_UPPER);
	PX_Create(PFFT,S,S);
	P_CopyToComplex(P,PFFT);
	/*
	Profil PRe;
	P_Create(PRe,S,S);
	PX_GetRe(PFFT,PRe);
	P_Save(PRe,"P_Re.bmp");
	P_Close(PRe);
	*/
	PX_TranslateToCenter_FFT_AlternateSign(PFFT);
	PX_FFT(PFFT);
	Profil PEnveloppe;
	P_Create(PEnveloppe,S,S);
	PX_GetModule(PFFT,PEnveloppe);
	P_Save(PEnveloppe,"EnveloppeBrute.bmp");
	{for(int i=0;i<Iter;i++) P_FastConvLowPass(PEnveloppe,15);}
	P_Save(PEnveloppe,"EnveloppeFiltree.bmp");

	Epsilon*=S;
	
	//for(int i=0;i<P_SizeX(PEnveloppe)*P_SizeY(PEnveloppe);i++)
	for(y=0;y<P_SizeY(PEnveloppe);y++)
	{
		for(int x=0;x<P_SizeY(PEnveloppe);x++)
		{
			int i=x+y*P_SizeX(PEnveloppe);
			//fenetrage bande passante
			float tr=(0.5f*((x-S/2)*(x-S/2)+(y-S/2)*(y-S/2)))/(S*S);
			float C=V_Max(1.0f-tr,0)/(Epsilon+P_Data(PEnveloppe)[i]);
			PX_Data(PFFT)[i].re=PX_Data(PFFT)[i].re*C;
			PX_Data(PFFT)[i].im=-PX_Data(PFFT)[i].im*C;
		}
	}
	
	P_Close(PEnveloppe);
	PX_FFT(PFFT);
	PX_TranslateToCenter_FFT_AlternateSign(PFFT);
	PX_GetRe(PFFT,P);
	PX_Close(PFFT);
	return;
}

#endif

int SPG_CONV P8_CopyToFloat(Profil& P, Profil8& P8, int X, int Y, int SizeX, int SizeY)
{
	int y;
	CHECK(((P_Etat(P))&&(P_Data(P)))==0,"P8_CopyToFloat: Le profil destination est nul",return 0);
	CHECK(((P_Etat(P8))&&(P_Data(P8)))==0,"P8_CopyToFloat: Le profil source est nul",return 0);
	CHECK(!V_IsBound(X,0,P8_SizeX(P8)),"P8_CopyToFloat: Position X incorrecte",return 0);
	CHECK(!V_IsBound(Y,0,P8_SizeY(P8)),"P8_CopyToFloat: Position Y incorrecte",return 0);
	CHECK(!V_InclusiveBound((X+SizeX),0,P8_SizeX(P8)),"P8_CopyToFloat: Taille X incorrecte",return 0);
	CHECK(!V_InclusiveBound((Y+SizeY),0,P8_SizeY(P8)),"P8_CopyToFloat: Taille Y incorrecte",return 0);

	SizeX&=~1;
	BYTE * Src=P8_Data(P8)+X+P8_SizeX(P8)*Y;
	float * Dst=P_Data(P);
	for(y=0;y<SizeY;y++)
	{
		BYTE* SrcL=Src;
		float* DstL=Dst;

		for(int x=0;x<SizeX;x+=2)
		{
			*DstL=(float)(*SrcL);
			*(DstL+1)=(float)(*(SrcL+1));
			SrcL+=2;
			DstL+=2;
		}
		Src+=P8_SizeX(P8);
		Dst+=P_SizeX(P);
	}
	return -1;
}

int SPG_CONV P_CopyFromByte(Profil& P, BYTE* GreyScale8BitsImage, int Pitch)
{
	int y;
	CHECK(((P_Etat(P))&&(P_Data(P)))==0,"P_ReadByte: Le profil destination est nul",return 0);
	CHECK(GreyScale8BitsImage==0,"P_ReadByte: Le GreyScale8BitsImage est nul",return 0);
	if(Pitch==0) 
	{
		Pitch=P_SizeX(P);
	}
	float * Dst=P_Data(P);
	for(y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x+=2)
		{
			*Dst=(float)(*GreyScale8BitsImage);
			*(Dst+1)=(float)(*(GreyScale8BitsImage+1));
			GreyScale8BitsImage+=2;
			Dst+=2;
		}
		GreyScale8BitsImage+=Pitch-P_SizeX(P);
	}
	return -1;
}

void SPG_CONV P8_CopyFromFloat(Profil8& Pdest, Profil& Psrc)
{
	CHECK(((P_Etat(Pdest))&&(P_Etat(Psrc))&&(P_Data(Pdest))&&(P_Data(Psrc)))==0,"P8_CopyFromFloat: L'un au moins des profils est nul",return);
	/*
	CHECK(P_SizeX(Pdest)!=P_SizeX(Psrc),"P8_CopyFromFloat: Taille X discordante",return);
	CHECK(P_SizeY(Pdest)!=P_SizeY(Psrc),"P8_CopyFromFloat: Taille Y discordante",return);
	*/

	//attention cette fonction prend le min et le max
	//sur tout le profil pour normaliser a 0 255 mais
	//gere la copie dans une destination de taille differente de la source
	//donc le min et le max ne seront pas forcement visibles (piege potentiel)
	float fMin,fMax;
	P_FindMinMax(Psrc,fMin,fMax);
	float deltaF=fMax-fMin;

	CHECK(deltaF==0,"P8_CopyFromFloat: Profil partout nul",return);

	deltaF=255.99f/deltaF;
	
	float* BSLine=Psrc.D;
	BYTE* ESLine=Pdest.D;
	int LLine=V_Min(P_SizeX(Psrc),P_SizeX(Pdest));//longueur a copier par ligne
	int y;
	for(y=0;y<V_Min(P_SizeY(Psrc),P_SizeY(Pdest));y++)
	{//chaque ligne
		float* BLine=BSLine;//pointeur de debut de ligne source
		BYTE* ELine=ESLine;//pointeur de debut de ligne destination
		for(int x=0;x<LLine;x+=2)
		{
			*ELine=V_FloatToByte((*BLine-fMin)*deltaF);
			*(ELine+1)=V_FloatToByte((*(BLine+1)-fMin)*deltaF);
			ELine+=2;//avance sur chaque pixel de la ligne
			BLine+=2;
		}
		BSLine+=P_SizeX(Psrc);//avance d'une ligne le pointeur de debut de ligne
		ESLine+=P_SizeX(Pdest);
	}

	if((Psrc.Msk)&&(Pdest.Msk)) 
	{
		for(y=0;y<V_Min(P_SizeY(Psrc),P_SizeY(Pdest));y++)
		{
			SPG_Memcpy(Pdest.Msk+y*P_SizeX(Pdest),Psrc.Msk+y*P_SizeX(Psrc),LLine);
		}
	}

	return;
}

//identique sauf les types d'entree
void SPG_CONV P_CopyToByte(Profil& Psrc, BYTE* Pdest)
{
	CHECK(((P_Etat(Psrc))&&(P_Data(Psrc)))==0,"P_CopyToByte: Profils source nul",return);
	CHECK(Pdest==0,"P_CopyToByte: Profil destination nul",return);
	/*
	CHECK(P_SizeX(Pdest)!=P_SizeX(Psrc),"P8_CopyFromFloat: Taille X discordante",return);
	CHECK(P_SizeY(Pdest)!=P_SizeY(Psrc),"P8_CopyFromFloat: Taille Y discordante",return);
	*/

	//attention cette fonction prend le min et le max
	//sur tout le profil pour normaliser a 0 255 mais
	//gere la copie dans une destination de taille differente de la source
	//donc le min et le max ne seront pas forcement visibles (piege potentiel)
	float fMin,fMax;
	P_FindMinMax(Psrc,fMin,fMax);
	float deltaF=fMax-fMin;

	CHECK(deltaF==0,"P8_CopyFromFloat: Profil partout nul",return);

	deltaF=255.99f/deltaF;
	
	float* BSLine=Psrc.D;
	BYTE* ESLine=Pdest;
	int LLine=P_SizeX(Psrc);//longueur a copier par ligne
	for(int y=0;y<P_SizeY(Psrc);y++)
	{//chaque ligne
		float* BLine=BSLine;//pointeur de debut de ligne source
		BYTE* ELine=ESLine;//pointeur de debut de ligne destination
		for(int x=0;x<LLine;x+=2)
		{
			*ELine=V_FloatToByte((*BLine-fMin)*deltaF);
			*(ELine+1)=V_FloatToByte((*(BLine+1)-fMin)*deltaF);
			ELine+=2;//avance sur chaque pixel de la ligne
			BLine+=2;
		}
		BSLine+=P_SizeX(Psrc);//avance d'une ligne le pointeur de debut de ligne
		ESLine+=P_SizeX(Psrc);
	}
	return;
}

#ifdef SPG_General_USEGraphics
void SPG_CONV P_Draw(Profil& P, G_Ecran& E, int PosX, int PosY, DWORD MskColor)
{
	float fMin,fMax;
	P_FindMinMax(P,fMin,fMax);
	if(MskColor)
	{
		P_DrawInternalWithMask(P,E,PosX,PosY,fMin,fMax,MskColor);
	}
	else
	{
		P_DrawInternal(P,E,PosX,PosY,fMin,fMax);
	}
	return;
}

void SPG_CONV P_DrawInternal(Profil& P, G_Ecran& E, int PosX,int PosY, float fMin, float fMax)
{
	CHECK(P_Etat(P)==0,"P_Draw: Profil nul",return);
	CHECK(E.Etat==0,"P_Draw: Ecran nul",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"P_Draw: Ecran non accessible",return);
	float deltaF=fMax-fMin;

	//CHECK(deltaF==0,"P_Draw: Profil partout nul",return);
	if(deltaF==0) return;

#ifdef DebugProfilManagerTimer
	S_StartTimer(Global.T_P_Draw);
#endif

	deltaF=255.49f/deltaF;
/*
	int SizeX=V_Min(P_SizeX(P),E.SizeX);
	int SizeY=V_Min(P_SizeY(P),E.SizeY);
*/
	int SrcPosX=V_Max(0,-PosX);
	int SrcPosY=V_Max(0,-PosY);
	int SrcSizeX=P_SizeX(P)-SrcPosX;
	int SrcSizeY=P_SizeY(P)-SrcPosY;

	int DestPosX=V_Max(PosX,0);
	int DestPosY=V_Max(PosY,0);
	int DestSizeX=E.SizeX-DestPosX;
	int DestSizeY=E.SizeY-DestPosY;

	int SizeX=V_Min(SrcSizeX,DestSizeX);
	int SizeY=V_Min(SrcSizeY,DestSizeY);

//	int LLine=SizeX*E.POCT;
	float* BLineY=P_Data(P)+SrcPosX+P_SizeX(P)*SrcPosY;

	if (E.POCT==4)
	{
	//DWORD* ELine=(DWORD*)PixEcrPTR(E,DestPosX,DestPosY);
	BYTE* ELineY=PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		BYTE* ELine=ELineY;
		for(int xt=0;xt<SizeX-3;xt+=4)
		{
			BYTE  B0=V_FloatToByte((BLineY[xt]-fMin)*deltaF);
			BYTE  B1=V_FloatToByte((BLineY[xt+1]-fMin)*deltaF);
			BYTE  B2=V_FloatToByte((BLineY[xt+2]-fMin)*deltaF);
			BYTE  B3=V_FloatToByte((BLineY[xt+3]-fMin)*deltaF);
			ELine[0]=B0;
			ELine[1]=B0;
			ELine[2]=B0;
			ELine[3]=B0;
			ELine[4]=B1;
			ELine[5]=B1;
			ELine[6]=B1;
			ELine[7]=B1;
			ELine[8]=B2;
			ELine[9]=B2;
			ELine[10]=B2;
			ELine[11]=B2;
			ELine[12]=B3;
			ELine[13]=B3;
			ELine[14]=B3;
			ELine[15]=B3;
			ELine+=4*4;
		}
		BLineY+=P_SizeX(P);
		(*(int*)&ELineY)+=E.Pitch;
	}
	}
	else if (E.POCT==3)
	{
	BYTE* ELineY=PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		BYTE* ELine=ELineY;
		for(int xt=0;xt<SizeX-3;xt+=4)
		{
			BYTE B0=V_FloatToByte((BLineY[xt]-fMin)*deltaF);
			BYTE B1=V_FloatToByte((BLineY[xt+1]-fMin)*deltaF);
			BYTE B2=V_FloatToByte((BLineY[xt+2]-fMin)*deltaF);
			BYTE B3=V_FloatToByte((BLineY[xt+3]-fMin)*deltaF);
			ELine[0]=B0;
			ELine[1]=B0;
			ELine[2]=B0;
			ELine[3]=B1;
			ELine[4]=B1;
			ELine[5]=B1;
			ELine[6]=B2;
			ELine[7]=B2;
			ELine[8]=B2;
			ELine[9]=B3;
			ELine[10]=B3;
			ELine[11]=B3;
			ELine+=3*4;
		}
		BLineY+=P_SizeX(P);
		(*(int*)&ELineY)+=E.Pitch;
	}
	}
	else if (E.POCT==2)
	{
	WORD* ELine=(WORD*)PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			DWORD CoulIn=0x010101*V_FloatToInt((BLineY[xt]-fMin)*deltaF);
			G_MakeCompatibleFrom24(E,CoulIn,ELine[xt]);
			//ELine[xt]=(0x0841)*(int)(31.0*(BLine[xt]-fMin)*deltaF);
		}
		BLineY+=P_SizeX(P);
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==1)
	{
	BYTE* ELine=(BYTE*)PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			ELine[xt]=V_FloatToByte((BLineY[xt]-fMin)*deltaF);
			//ELine[xt]=(0x0841)*(int)(31.0*(BLine[xt]-fMin)*deltaF);
		}
		BLineY+=P_SizeX(P);
		(*(int*)&ELine)+=E.Pitch;
	}
	}

	/*
	else if (E.POCT==1)
	{
	BYTE* BLine=Button;
	BYTE* ELine=PixEcrPTR(E,X,Y);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if (BLine[xt]!=CoulTransp) ELine[xt]=BLine[xt];
		}
		(*(int*)&BLine)+=Pitch;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	*/

#ifdef DebugProfilManagerTimer
	S_StopTimer(Global.T_P_Draw);
#endif
	return;

}

void SPG_CONV P_DrawPalette(Profil& P, G_Ecran& E, int PosX,int PosY, float fMin, float fMax, PixCoul* PX, int NPX)
{
	CHECK(P_Etat(P)==0,"P_DrawPalette: Profil nul",return);
	CHECK(E.Etat==0,"P_DrawPalette: Ecran nul",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"P_DrawPalette: Ecran non accessible",return);
	float deltaF=fMax-fMin;

	if(deltaF==0) return;

#ifdef DebugProfilManagerTimer
	S_StartTimer(Global.T_P_Draw);
#endif

	deltaF=(NPX-0.51f)/deltaF;

	int SrcPosX=V_Max(0,-PosX);
	int SrcPosY=V_Max(0,-PosY);
	int SrcSizeX=P_SizeX(P)-SrcPosX;
	int SrcSizeY=P_SizeY(P)-SrcPosY;

	int DestPosX=V_Max(PosX,0);
	int DestPosY=V_Max(PosY,0);
	int DestSizeX=E.SizeX-DestPosX;
	int DestSizeY=E.SizeY-DestPosY;

	int SizeX=V_Min(SrcSizeX,DestSizeX);
	int SizeY=V_Min(SrcSizeY,DestSizeY);

	float* BLineY=P_Data(P)+SrcPosX+P_SizeX(P)*SrcPosY;

	if (E.POCT==4)
	{
		BYTE* ELineY=PixEcrPTR(E,DestPosX,DestPosY);
		int yt;
		for(yt=0;yt<SizeY;yt++)
		{
			PixCoul* ELine=(PixCoul*)ELineY;
			for(int xt=0;xt<SizeX;xt++)
			{
				int n=V_FloatToInt((BLineY[xt]-fMin)*deltaF);
				*ELine=PX[V_Sature(n,0,NPX-1)];
				ELine++;
			}
			BLineY+=P_SizeX(P);
			(*(int*)&ELineY)+=E.Pitch;
		}
	}
	else if (E.POCT==3)
	{
		BYTE* ELineY=PixEcrPTR(E,DestPosX,DestPosY);
		int yt;
		for(yt=0;yt<SizeY;yt++)
		{
			PixCoul24* ELine=(PixCoul24*)ELineY;
			for(int xt=0;xt<SizeX;xt++)
			{
				int n=V_FloatToInt((BLineY[xt]-fMin)*deltaF);
				*ELine=PX[V_Sature(n,0,NPX-1)].P24;
				ELine++;
			}
			BLineY+=P_SizeX(P);
			(*(int*)&ELineY)+=E.Pitch;
		}
	}

#ifdef DebugProfilManagerTimer
	S_StopTimer(Global.T_P_Draw);
#endif
	return;

}

void SPG_CONV P_DrawInternalWithMask(Profil& P, G_Ecran& E, int PosX,int PosY, float fMin, float fMax, DWORD MskColor)
{
	CHECK(P_Etat(P)==0,"P_Draw: Profil nul",return);
	CHECK(E.Etat==0,"P_Draw: Ecran nul",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"P_Draw: Ecran non accessible",return);
	if(P_Msk(P)==0)
	{
		P_DrawInternal(P,E,PosX,PosY,fMin,fMax);
		return;
	}
	float deltaF=fMax-fMin;

	//CHECK(deltaF==0,"P_Draw: Profil partout nul",return);
	if(deltaF==0) return;

#ifdef DebugProfilManagerTimer
	S_StartTimer(Global.T_P_Draw);
#endif

	deltaF=255.49f/deltaF;
/*
	int SizeX=V_Min(P_SizeX(P),E.SizeX);
	int SizeY=V_Min(P_SizeY(P),E.SizeY);
*/
	int SrcPosX=V_Max(0,-PosX);
	int SrcPosY=V_Max(0,-PosY);
	int SrcSizeX=P_SizeX(P)-SrcPosX;
	int SrcSizeY=P_SizeY(P)-SrcPosY;

	int DestPosX=V_Max(PosX,0);
	int DestPosY=V_Max(PosY,0);
	int DestSizeX=E.SizeX-DestPosX;
	int DestSizeY=E.SizeY-DestPosY;

	int SizeX=V_Min(SrcSizeX,DestSizeX);
	int SizeY=V_Min(SrcSizeY,DestSizeY);

//	int LLine=SizeX*E.POCT;
	float* BLineY=P_Data(P)+SrcPosX+P_SizeX(P)*SrcPosY;
	BYTE* MLineY=P_Msk(P)+SrcPosX+P_SizeX(P)*SrcPosY;

	if (E.POCT==4)
	{
	//DWORD* ELine=(DWORD*)PixEcrPTR(E,DestPosX,DestPosY);
	BYTE* ELineY=PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		BYTE* ELine=ELineY;
		for(int xt=0;xt<SizeX;xt++)
		{
			if(MLineY[xt])
			{
				*(DWORD*)ELine=MskColor;
			}
			else
			{
				BYTE  B0=V_FloatToByte((BLineY[xt]-fMin)*deltaF);
				ELine[0]=B0;
				ELine[1]=B0;
				ELine[2]=B0;
				ELine[3]=B0;
			}
			ELine+=4;
		}
		BLineY+=P_SizeX(P);
		MLineY+=P_SizeX(P);
		(*(int*)&ELineY)+=E.Pitch;
	}
	}
	else if (E.POCT==3)
	{
	BYTE* ELineY=PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		BYTE* ELine=ELineY;
		for(int xt=0;xt<SizeX;xt++)
		{
			if(MLineY[xt])
			{
				*(PixCoul24*)ELine=*(PixCoul24*)&MskColor;
			}
			else
			{
				BYTE B0=V_FloatToByte((BLineY[xt]-fMin)*deltaF);
				ELine[0]=B0;
				ELine[1]=B0;
				ELine[2]=B0;
			}
			ELine+=3;
		}
		BLineY+=P_SizeX(P);
		MLineY+=P_SizeX(P);
		(*(int*)&ELineY)+=E.Pitch;
	}
	}
	else if (E.POCT==2)
	{
	WORD* ELine=(WORD*)PixEcrPTR(E,DestPosX,DestPosY);
	WORD wMskColor=G_Make16From24(MskColor);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if(MLineY[xt])
			{
				ELine[xt]=wMskColor;
			}
			else
			{
				DWORD CoulIn=0x010101*V_FloatToInt((BLineY[xt]-fMin)*deltaF);
				G_MakeCompatibleFrom24(E,CoulIn,ELine[xt]);
			}
			//ELine[xt]=(0x0841)*(int)(31.0*(BLine[xt]-fMin)*deltaF);
		}
		BLineY+=P_SizeX(P);
		MLineY+=P_SizeX(P);
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==1)
	{
	BYTE* ELine=(BYTE*)PixEcrPTR(E,DestPosX,DestPosY);
	BYTE bMskColor=G_Make8From24(MskColor);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if(MLineY[xt])
			{
				ELine[xt]=bMskColor;
			}
			else
			{
				ELine[xt]=V_FloatToByte((BLineY[xt]-fMin)*deltaF);
			}
			//ELine[xt]=(0x0841)*(int)(31.0*(BLine[xt]-fMin)*deltaF);
		}
		BLineY+=P_SizeX(P);
		MLineY+=P_SizeX(P);
		(*(int*)&ELine)+=E.Pitch;
	}
	}

	/*
	else if (E.POCT==1)
	{
	BYTE* BLine=Button;
	BYTE* ELine=PixEcrPTR(E,X,Y);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if (BLine[xt]!=CoulTransp) ELine[xt]=BLine[xt];
		}
		(*(int*)&BLine)+=Pitch;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	*/

#ifdef DebugProfilManagerTimer
	S_StopTimer(Global.T_P_Draw);
#endif
	return;

}

void SPG_CONV P8_Draw(Profil8& P, G_Ecran& E, int PosX,int PosY)
{
	CHECK(P_Etat(P)==0,"P8_Draw: Profil vide",return);
	CHECK(E.Etat==0,"P8_Draw: Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"P8_Draw: Ecran non accessible",return);

#ifdef DebugProfilManagerTimer
	S_StartTimer(Global.T_P8_Draw);
#endif

	int SrcPosX=V_Max(0,-PosX);
	int SrcPosY=V_Max(0,-PosY);
	int SrcSizeX=P_SizeX(P)-SrcPosX;
	int SrcSizeY=P_SizeY(P)-SrcPosY;

	int DestPosX=V_Max(PosX,0);
	int DestPosY=V_Max(PosY,0);
	int DestSizeX=E.SizeX-DestPosX;
	int DestSizeY=E.SizeY-DestPosY;

	int SizeX=V_Min(SrcSizeX,DestSizeX);
	int SizeY=V_Min(SrcSizeY,DestSizeY);

	SizeX&=~1;
	int LLine=SizeX*E.POCT;
	BYTE* BLine=P_Data(P)+SrcPosX+P_SizeX(P)*SrcPosY;

	if (E.POCT==4)
	{
	//DWORD* ELine=(DWORD*)PixEcrPTR(E,DestPosX,DestPosY);
	BYTE* ELine=PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<LLine;xt+=8)
		{
			BYTE B=*BLine;
			BYTE B1=*(BLine+1);
			ELine[xt+2]=ELine[xt+1]=ELine[xt]=B;
			ELine[xt+6]=ELine[xt+5]=ELine[xt+4]=B1;
			BLine+=2;
		}
		BLine+=P_SizeX(P)-SizeX;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==3)
	{
	BYTE* ELine=PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<LLine;xt+=6)
		{
			BYTE B=*BLine;
			BYTE B1=*(BLine+1);
			ELine[xt+2]=ELine[xt+1]=ELine[xt]=B;
			ELine[xt+5]=ELine[xt+4]=ELine[xt+3]=B1;
			BLine+=2;
		}
		BLine+=P_SizeX(P)-SizeX;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==2)
	{
	WORD* ELine=(WORD*)PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			DWORD CoulIn=0x010101*(*BLine);
			G_MakeCompatibleFrom24(E,CoulIn,ELine[xt]);
			//ELine[xt]=(0x0841)*(int)(31.0*(BLine[xt]-fMin)*deltaF);
			BLine++;
		}
		BLine+=P_SizeX(P)-SizeX;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==1)
	{
	BYTE* ELine=(BYTE*)PixEcrPTR(E,DestPosX,DestPosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			ELine[xt]=*BLine;
			//ELine[xt]=(0x0841)*(int)(31.0*(BLine[xt]-fMin)*deltaF);
			BLine++;
		}
		BLine+=P_SizeX(P)-SizeX;
		(*(int*)&ELine)+=E.Pitch;
	}
	}

	/*
	else if (E.POCT==1)
	{
	BYTE* BLine=Button;
	BYTE* ELine=PixEcrPTR(E,X,Y);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if (BLine[xt]!=CoulTransp) ELine[xt]=BLine[xt];
		}
		(*(int*)&BLine)+=Pitch;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	*/
#ifdef DebugProfilManagerTimer
	S_StopTimer(Global.T_P8_Draw);
#endif
	return;

}
#endif
#endif

