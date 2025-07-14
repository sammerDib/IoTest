
#include "SPG_General.h"

#ifdef SPG_General_USERINGREC

#include "SPG_Includes.h"

#include <string.h>

int SPG_CONV RGR_Init(RING_REC& RGR,int SizeX,int SizeY,int NumS)
{
	memset(&RGR,0,sizeof(RING_REC));
	CHECK(SizeX<=0,"RGR_Init: Taille invalide",return 0);
	CHECK(SizeY<=0,"RGR_Init: Taille invalide",return 0);
	CHECK(NumS<=0,"RGR_Init: Taille invalide",return 0);
//	CHECK(SizeX*SizeY*NumS>32*1024*1024,"RGR_Init: Taille trop grande",return 0);
	CHECK((RGR.D=SPG_TypeAlloc(SizeX*SizeY*NumS,BYTE,"RGR Data"))==0,"RGR_Init: Allocation echouee",return 0);
	RGR.SizeX=SizeX;
	RGR.SizeY=SizeY;
	RGR.SizeP=SizeX*SizeY;
	RGR.NumS=NumS;
	RGR.Etat=RGR_OK;
	return -1;
}

void SPG_CONV RGR_Copy(RING_REC& RGR, int n, BYTE* ESrc, int Pitch)
{
	CHECK(RGR.Etat!=RGR_OK,"RGR_Copy: RING_REC invalide",return);
	CHECK(ESrc==0,"RGR_Copy",return);
	CHECK(!V_IsBound(n,0,RGR.NumS),"RGR_Copy: Numero de plan invalide",return);
	BYTE* RGRDest=RGR_Plane(RGR,n);
	for(int y=0;y<RGR.SizeY;y++)
	{
		memcpy(RGRDest,ESrc,RGR.SizeX);
		RGRDest+=RGR.SizeX;
		ESrc+=Pitch;
	}
	return;
}

#ifdef SPG_General_USECut

void SPG_CONV RGR_CreateCut(RING_REC& RGR,Cut& C,int X,int Y)
{
	memset(&C,0,sizeof(Cut));
	CHECK(RGR.Etat!=RGR_OK,"RGR_CreateCut: RING_REC invalide",return);
	CHECK(V_IsBound(X,0,RGR.SizeX),"RGR_CreateCut: Position X invalide",return);
	CHECK(V_IsBound(Y,0,RGR.SizeY),"RGR_CreateCut: Position Y invalide",return);
	Cut_Create(C,RGR.NumS,0,0,0);
	RGR_CutCopy(RGR,C,X,Y);
	return;
}

void SPG_CONV RGR_CutCopy(RING_REC& RGR,Cut& C,int X,int Y)
{
	CHECK(RGR.Etat!=RGR_OK,"RGR_CutCopy: RING_REC invalide",return);
	CHECK(C.Etat==0,"RGR_CutCopy: Coupe nulle",return);
	CHECK(C.D==0,"RGR_CutCopy: Coupe nulle",return);
	CHECK(C.NumS!=RGR.NumS,"RGR_CutCopy: Taille de coupe discordante",return);
	CHECK(!V_IsBound(X,0,RGR.SizeX),"RGR_CutCopy: Position X invalide",return);
	CHECK(!V_IsBound(Y,0,RGR.SizeY),"RGR_CutCopy: Position Y invalide",return);
	BYTE*BLine=RGR.D+X+RGR.SizeX*Y;
	for(int i=0;i<C.NumS;i++)
	{
		C.D[i]=(float)BLine[i*RGR.SizeP];
	}
	//Cut_Init(C,RGR.NumS,RGR_Element(RGR,X,Y,0),0,0,C_Alias);
	return;
}

#endif

#ifdef SPG_General_USEProfil

void SPG_CONV RGR_ProCopy(RING_REC& RGR,Profil& P,int n)
{
	CHECK(RGR.Etat!=RGR_OK,"RGR_ProCopy: RING_REC invalide",return);
	CHECK(P_Etat(P)==0,"RGR_ProCopy: Profil nul",return);
	CHECK(P_Data(P)==0,"RGR_ProCopy: Profil nul",return);
	CHECK(P_SizeX(P)!=RGR.SizeX,"RGR_ProCopy: Taille de profil incorrecte",return);
	CHECK(P_SizeY(P)!=RGR.SizeY,"RGR_ProCopy: Taille de profil incorrecte",return);
	CHECK(!V_IsBound(n,0,RGR.NumS),"RGR_ProCopy: Numero de plan invalide",return);

	float*PLine=P.D;
	BYTE*BLine=RGR.D+RGR.SizeP*n;
	for(int i=0;i<RGR.SizeP;i++)
	{
		PLine[i]=(float)BLine[i];
	}
	//Cut_Init(C,RGR.NumS,RGR_Element(RGR,X,Y,0),0,0,C_Alias);
	return;
}

void SPG_CONV RGR_ProXCopy(RING_REC& RGR,Profil& P,int Y)
{
	CHECK(RGR.Etat!=RGR_OK,"RGR_ProXCopy: RING_REC invalide",return);
	CHECK(P_Etat(P)==0,"RGR_ProXCopy: Profil nul",return);
	CHECK(P_Data(P)==0,"RGR_ProXCopy: Profil nul",return);
	CHECK(P_SizeX(P)!=RGR.SizeX,"RGR_ProXCopy: Taille de profil incorrecte",return);
	CHECK(P_SizeY(P)!=RGR.NumS,"RGR_ProXCopy: Taille de profil incorrecte",return);
	CHECK(!V_IsBound(Y,0,RGR.SizeY),"RGR_ProXCopy: Numero de plan invalide",return);

	for(int n=0;n<RGR.NumS;n++)
	{
		float*PLine=P.D+P_SizeX(P)*n;
		BYTE*BLine=RGR.D+RGR.SizeX*Y+RGR.SizeP*n;
		for(int x=0;x<RGR.SizeX;x++)
		{
			PLine[x]=(float)BLine[x];
		}
	}
	//Cut_Init(C,RGR.NumS,RGR_Element(RGR,X,Y,0),0,0,C_Alias);
	return;
}

void SPG_CONV RGR_ProYCopy(RING_REC& RGR,Profil& P,int X)
{
	CHECK(RGR.Etat!=RGR_OK,"RGR_ProYCopy: RING_REC invalide",return);
	CHECK(P_Etat(P)==0,"RGR_ProYCopy: Profil nul",return);
	CHECK(P_Data(P)==0,"RGR_ProYCopy: Profil nul",return);
	CHECK(P_SizeX(P)!=RGR.NumS,"RGR_ProYCopy: Taille de profil incorrecte",return);
	CHECK(P_SizeY(P)!=RGR.SizeY,"RGR_ProYCopy: Taille de profil incorrecte",return);
	CHECK(!V_IsBound(X,0,RGR.SizeX),"RGR_ProYCopy: Numero de plan invalide",return);

	for(int y=0;y<RGR.SizeY;y++)
	{
		float*PLine=P.D+P_SizeX(P)*y;
		BYTE*BLine=RGR.D+X+RGR.SizeX*y;
		for(int n=0;n<RGR.NumS;n++)
		{
			PLine[n]=(float)BLine[RGR.SizeP*n];
		}
	}
	return;
}

#endif

void SPG_CONV RGR_Close(RING_REC& RGR)
{
	if (RGR.D) SPG_MemFree(RGR.D);
	memset(&RGR,0,sizeof(RING_REC));
	return;
}

#endif

