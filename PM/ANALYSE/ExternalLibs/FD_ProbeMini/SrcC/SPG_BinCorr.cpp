

#include "SPG_General.h"

#ifdef SPG_General_USEBINCORR

#include <string.h>

#ifdef DebugFloat
#include <float.h>
#endif

#include "SPG_Includes.h"

#include "SPG_BinCorr.h"


//fonctions 'externes'
int BC_Init(SPG_BinCorr& BC, TBIN* Reference, int SizeX, int SizeY)
{
	memset(&BC,0,sizeof(SPG_BinCorr));
	CHECK(Reference==0,"BC_Init",return 0)
	BC_CreateBin(BC.Reference,SizeX,SizeY,Reference);
	//BC_Transform(BC,BC.Reference,BC.ReferenceT);
	return BC.Etat=-1;
}

void BC_Close(SPG_BinCorr& BC)
{
	CHECK(BC.Etat==0,"BinCorr: non initialisé",return);
	//BC_DeleteBinT(BC.ReferenceT);
	BC_DeleteBin(BC.Reference);
	BC_DeleteBinCorrResult(BC.R);
	memset(&BC,0,sizeof(SPG_BinCorr));
	return;
}

int BC_FindFullPos(SPG_BinCorr& BC, TBIN* Current, int SizeX, int SizeY)
{
	return BC_FindPos(BC, Current, SizeX, SizeY, -SizeX, -SizeY, BC.Reference.SizeX, BC.Reference.SizeY);
}

int BC_FindPos(SPG_BinCorr& BC, TBIN* Current, int SizeX, int SizeY, int OffsetX0, int OffsetY0, int OffsetX1, int OffsetY1)
{
	CHECK(BC.Etat==0,"BC_FindPos: non initialisé",return 0);
	CHECK(BC.Reference.Etat==0,"BC_FindPos: non initialisé",return 0);
	CHECK(Current==0,"BC_FindPos: non initialisé",return 0);

	BC_CreateBin(BC.Current,SizeX,SizeY,Current);

	if(BC_CreateBinCorrResult(BC.R,OffsetX1-OffsetX0,OffsetY1-OffsetY0)==0) return 0;

	int xMax=0;
	int yMax=0;
	TSUM SumMax=0;

	for(int y=OffsetY0;y<OffsetY1;y++)
	{
		for(int x=OffsetX0;x<OffsetX1;x++)
		{
			TSUM Sum;
			BC.R.D[x+y*BC.R.SizeX]=Sum=BC_Compute(BC.Reference,BC.Current,x,y);
			if(Sum>SumMax)
			{
				SumMax=Sum;
				xMax=x;
				yMax=y;
			}
		}
	}

	BC.R.Max=SumMax;
	BC.R.X=xMax;
	BC.R.Y=yMax;

	return SumMax;
}

//fonctions 'internes'
//int BC_Compute(SPG_BinCorr& BC, SPG_ProfilBinT ReferenceT, SPG_ProfilBin Current, SPG_BinCorrResult& BR); 

int SPG_GetCommonRect(SPG_CommonRect& CR, TBIN* D1, int SizeX1, int SizeY1, TBIN* D2, int SizeX2, int SizeY2, int OffsetX, int OffsetY)
{//détermine les pointeurs et les dimensions de la zone commune entre deux images décalées d'un offset
//renvoie 0 si pas de zone commune, renvoie -1 et les pointeurs de début de la zone commune dans la structure sinon
	CHECK(D1==0,"SPG_GetCommonRect",return 0);
	CHECK(D2==0,"SPG_GetCommonRect",return 0);

	int XStart=V_Max(OffsetX,0);
	int YStart=V_Max(OffsetY,0);
	int XStop=V_Min(SizeX1,OffsetX+SizeX2);
	int YStop=V_Max(SizeY1,OffsetY+SizeY2);

	if((CR.SizeX=XStop-XStart)<=0) return 0;
	if((CR.SizeY=YStop-YStart)<=0) return 0;

	CR.Pitch1=SizeX1;
	CR.D1=D1+XStart+YStart*SizeX1;

	CR.Pitch2=SizeX2;
	CR.D2=D2+(XStart-OffsetX)+(YStart-OffsetY)*SizeX2;

	return -1;
}

int BC_Compute(SPG_ProfilBin& Reference, SPG_ProfilBin& Current, int X, int Y)
{//calcule la corrélation entre deux images binaires à une position X,Y (attention c'est une somme de & binaire)
	SPG_CommonRect CR;

	if(SPG_GetCommonRect(CR,
		Reference.D,Reference.SizeX,Reference.SizeY,
		Current.D,Current.SizeX,Current.SizeY,
		X,Y)==0) return 0;

	TSUM Sum=0;
	for(int y=0;y<CR.SizeY;y++)
	{
		for(int x=0;x<CR.SizeX;x++)
		{
			Sum+=BINtoSUM(CR.D1[x],CR.D2[x]);
		}
		CR.D1+=CR.Pitch1;
		CR.D2+=CR.Pitch2;
	}
	return Sum;
}

int BC_CreateBin(SPG_ProfilBin& P, int SizeX, int SizeY, TBIN* D)
{
	memset(&P,0,sizeof(SPG_ProfilBin));
	P.SizeX=SizeX;
	P.SizeY=SizeY;
	if(D)
	{
		P.D=D;
		P.Etat=BC_Alias;
	}
	else
	{
		P.D=SPG_TypeAlloc(P.SizeX*P.SizeY,TBIN,"BC_CreateBin");
		P.Etat=BC_WithMem;
	}
	return P.Etat=-1;
}

void BC_DeleteBin(SPG_ProfilBin& P)
{
	if(P.Etat==BC_WithMem)
	{
		SPG_MemFree(P.D);
	}
	memset(&P,0,sizeof(SPG_ProfilBin));
	return;
}

int BC_CreateBinCorrResult(SPG_BinCorrResult& R, int SizeX, int SizeY)
{
	memset(&R,0,sizeof(SPG_BinCorrResult));
	CHECK(SizeX<=0,"BC_CreateBinCorrResult",return 0);
	CHECK(SizeY<=0,"BC_CreateBinCorrResult",return 0);
	R.SizeX=SizeX;
	R.SizeY=SizeY;
	R.D=SPG_TypeAlloc(R.SizeX*R.SizeY,TSUM,"BC_BinCorrResult");
	return R.Etat=-1;
}

void BC_DeleteBinCorrResult(SPG_BinCorrResult& R)
{
	CHECK(R.Etat==0,"BC_DeleteBinCorrResult",return);
	SPG_MemFree(R.D);
	memset(&R,0,sizeof(SPG_BinCorrResult));
	return;
}

/*
int BC_CreateBinT(SPG_ProfilBin& PT, int SizeX, int SizeY);
int BC_DeleteBinT(SPG_ProfilBin& PT, int SizeX, int SizeY);
int BC_Transform(SPG_ProfilBin& Reference, SPG_ProfilBinT& ReferenceT);
int BC_InvTransform(SPG_ProfilBin& Reference, SPG_ProfilBinT& ReferenceT);
*/

#endif

