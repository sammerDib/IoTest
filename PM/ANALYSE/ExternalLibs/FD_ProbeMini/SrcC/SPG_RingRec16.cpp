
#include "SPG_General.h"

#ifdef SPG_General_USERINGREC16

#include "SPG_Includes.h"

#ifndef DebugMem
#ifdef SPG_General_USEWindows
#include <windows.h>
#endif
#endif

#include <string.h>

int SPG_CONV RGR16_Init(RING_REC16& RGR,int SizeX,int SizeY,int NumS)
{
	memset(&RGR,0,sizeof(RING_REC));
	SPG_CHECK(SizeX<=0,"RGR_Init: Taille invalide",return 0);
	SPG_CHECK(SizeY<=0,"RGR_Init: Taille invalide",return 0);
	SPG_CHECK(NumS<=0,"RGR_Init: Taille invalide",return 0);
//	SPG_CHECK(SizeX*SizeY*NumS>32*1024*1024,"RGR_Init: Taille trop grande",return 0);
	SPG_CHECK((RGR.D=SPG_TypeAlloc(SizeX*SizeY*NumS,WORD,"RGR Data"))==0,"RGR16_Init: Allocation echouee",return 0);
	RGR.SizeX=SizeX;
	RGR.SizeY=SizeY;
	RGR.SizeP=SizeX*SizeY;
	RGR.NumS=NumS;
	RGR.Etat=RGR_OK;
	return -1;
}

#ifdef SPG_General_USECut
void SPG_CONV RGR16_CreateCut(RING_REC16& RGR,Cut& C,int X,int Y)
{
	memset(&C,0,sizeof(Cut));
	SPG_CHECK(RGR.Etat!=RGR_OK,"RGR16_CreateCut: RING_REC invalide",return);
	SPG_CHECK(V_IsBound(X,0,RGR.SizeX),"RGR16_CreateCut: Position X invalide",return);
	SPG_CHECK(V_IsBound(Y,0,RGR.SizeY),"RGR16_CreateCut: Position Y invalide",return);
	Cut_Create(C,RGR.NumS,0,0,0);
	RGR16_CutCopy(RGR,C,X,Y);
	return;
}

void SPG_CONV RGR16_CutCopy(RING_REC16& RGR,Cut& C,int X,int Y)
{
	SPG_CHECK(RGR.Etat!=RGR16_OK,"RGR16_CutCopy: RING_REC invalide",return);
	SPG_CHECK(C.Etat==0,"RGR16_CutCopy: Coupe nulle",return);
	SPG_CHECK(C.D==0,"RGR16_CutCopy: Coupe nulle",return);
	SPG_CHECK(C.NumS!=RGR.NumS,"RGR16_CutCopy: Taille de coupe discordante",return);
	SPG_CHECK(!V_IsBound(X,0,RGR.SizeX),"RGR16_CutCopy: Position X invalide",return);
	SPG_CHECK(!V_IsBound(Y,0,RGR.SizeY),"RGR16_CutCopy: Position Y invalide",return);
	WORD*BLine=RGR.D+X+RGR.SizeX*Y;
	for(int i=0;i<C.NumS;i++)
	{
		C.D[i]=(float)BLine[i*RGR.SizeP];
	}
	//Cut_Init(C,RGR.NumS,RGR_Element(RGR,X,Y,0),0,0,C_Alias);
	return;
}

#endif

#ifdef SPG_General_USEProfil

void SPG_CONV RGR16_ProCopy(RING_REC16& RGR,Profil& P,int n)
{
	SPG_CHECK(RGR.Etat!=RGR_OK,"RGR16_ProCopy: RING_REC invalide",return);
	SPG_CHECK(P_Etat(P)==0,"RGR16_ProCopy: Profil nul",return);
	SPG_CHECK(P_Data(P)==0,"RGR16_ProCopy: Profil nul",return);
	SPG_CHECK(P_SizeX(P)!=RGR.SizeX,"RGR16_ProCopy: Taille de profil incorrecte",return);
	SPG_CHECK(P_SizeY(P)!=RGR.SizeY,"RGR16_ProCopy: Taille de profil incorrecte",return);
	SPG_CHECK(!V_IsBound(n,0,RGR.NumS),"RGR16_ProCopy: Numero de plan invalide",return);

	float*PLine=P.D;
	WORD*BLine=RGR.D+RGR.SizeP*n;
	for(int i=0;i<RGR.SizeP;i++)
	{
		PLine[i]=(float)BLine[i];
	}
	//Cut_Init(C,RGR.NumS,RGR_Element(RGR,X,Y,0),0,0,C_Alias);
	return;
}

#endif

void SPG_CONV RGR16_Close(RING_REC16& RGR)
{
	if (RGR.D) SPG_MemFree(RGR.D);
	memset(&RGR,0,sizeof(RING_REC));
	return;
}

#endif

