#include "SPG_General.h"

#ifdef SPG_General_USEWEIGHTBUFFER

#include "SPG_Includes.h"

#include <memory.h>

int SPG_CONV WB_Init(SPG_WB& WB, int SizeX, int SizeY, int Oversampling)
{
	memset(&WB,0,sizeof(SPG_WB));
	WB.SizeX=SizeX;
	WB.SizeY=SizeY;
	WB.Oversampling=Oversampling;
	WB.FullSizeX=WB.SizeX*WB.Oversampling;
	WB.FullSizeY=WB.SizeY*WB.Oversampling;
	WB.P=SPG_TypeAlloc(WB.FullSizeX*WB.FullSizeY,float,"WB_Init");
	WB.W=SPG_TypeAlloc(WB.Oversampling*WB.Oversampling,WORD,"WB_Init");
	return WB.Etat=-1;
}

void SPG_CONV WB_Close(SPG_WB& WB)
{
	SPG_MemFree(WB.P);
	SPG_MemFree(WB.W);
	memset(&WB,0,sizeof(SPG_WB));
	return;
}

void SPG_CONV WB_Clear(SPG_WB& WB)
{
	CHECK(WB.Etat==0,"WB_Clear",return);
	memset(WB.P,0,WB.FullSizeX*WB.FullSizeY*sizeof(float));
	memset(WB.W,0,WB.Oversampling*WB.Oversampling*sizeof(WORD));
	return;
}

void SPG_CONV WB_Copy(SPG_WB& WB, BYTE* E, int Pitch, int xd, int yd)
{
	CHECK(WB.Etat==0,"WB_Copy",return);
	CHECK(!V_IsBound(xd,0,WB.Oversampling),"WB_Copy",return);
	CHECK(!V_IsBound(yd,0,WB.Oversampling),"WB_Copy",return);
	CHECK(E==0,"WB_Copy",return);
	for(int y=0;y<WB.SizeY;y++)
	{
		float* ydD=WB.P+(y*WB.Oversampling+yd)*WB.FullSizeX+xd;
		for(int x=0;x<WB.SizeX;x++)
		{
			ydD[x*WB.Oversampling]+=(float)E[x];
		}
		E+=Pitch;
	}
	WB.W[xd+yd*WB.Oversampling]++;
	return;
}

void SPG_CONV WB_Copy(SPG_WB& WB, float* E, int xd, int yd)
{
	CHECK(WB.Etat==0,"WB_Copy",return);
	CHECK(!V_IsBound(xd,0,WB.Oversampling),"WB_Copy",return);
	CHECK(!V_IsBound(yd,0,WB.Oversampling),"WB_Copy",return);
	CHECK(E==0,"WB_Copy",return);
	for(int y=0;y<WB.SizeY;y++)
	{
		float* ydD=WB.P+(y*WB.Oversampling+yd)*WB.FullSizeX+xd;
		for(int x=0;x<WB.SizeX;x++)
		{
			ydD[x*WB.Oversampling]+=E[x];
		}
		E+=WB.SizeX;
	}
	WB.W[xd+yd*WB.Oversampling]++;
	return;
}

void SPG_CONV WB_SmoothNormalize(SPG_WB& WB)
{
	Profil P;
	P_Create(P,WB.FullSizeX,WB.FullSizeY);
	int OversamplingCenter=WB.Oversampling/2;
	for(int y=0;y<WB.FullSizeY;y++)
	{
		for(int x=0;x<WB.FullSizeX;x++)
		{
			float Sum=0;
			float Weight=0;
			for(int iy=-OversamplingCenter;iy<=OversamplingCenter;iy++)
			{
				int yr=V_Sature((y+iy),0,(WB.FullSizeY-1));
				float* PR=WB.P+yr*WB.FullSizeX;//pointeur de ligne
				WORD* WR=WB.W+(yr%WB.Oversampling)*WB.Oversampling;//ligne de count
				int iycarre=iy*iy;//distance
				for(int ix=-OversamplingCenter;ix<=OversamplingCenter;ix++)
				{
					int xr=V_Sature((x+ix),0,(WB.FullSizeX-1));
					WORD W=WR[xr%WB.Oversampling];//count
					float EffW=sqrt((float)W)/(1+16*(ix*ix+iycarre));//weight en racine(count)
					Weight+=EffW;
					Sum+=EffW*(PR[xr]/V_Max(W,1));
				}
			}
			if(Weight>0)
			{
				P_Element(P,x,y)=Sum/Weight;
			}
		}
	}
	V_SWAP(float*,P.D,WB.P);
	P_Close(P);
	return;
}

void SPG_CONV WB_BrutNormalize(SPG_WB& WB)
{
	CHECK(WB.Etat==0,"WB_Normalize",return);
	float* P=WB.P;
	for(int y=0;y<WB.SizeY;y++)
	{
		for(int x=0;x<WB.SizeX;x++)
		{
			for(int iy=0;iy<WB.Oversampling;iy++)
			{
				for(int ix=0;ix<WB.Oversampling;ix++)
				{
					P[x*WB.Oversampling+ix+iy*WB.FullSizeX]/=V_Max(WB.W[ix+iy*WB.Oversampling],1);
				}
			}
		}
		P+=WB.FullSizeX*WB.Oversampling;
	}
	return;
}

void SPG_CONV WB_Save(SPG_WB& WB, char* SuggestedName)
{
	CHECK(WB.Etat==0,"WB_Save",return);
	Profil P;
	P_Init(P,WB.P,0,WB.FullSizeX,WB.FullSizeY,P_Alias);
	P_Save(P,SuggestedName);
	P_Close(P);
	return;
}

#endif

