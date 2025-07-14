
#include "SPG_General.h"

#ifdef SPG_General_USEINVJ0

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <string.h>

int SPG_CONV INVJ0_Init(SPG_INVERSEUR& I, int NumP)
{
	memset(&I,0,sizeof(SPG_INVERSEUR));
	CHECK(NumP<2,"INVJ0_Init: Nombre de points insuffisant",return 0);
	CHECK((I.D=SPG_TypeAlloc(NumP,float,"J0 inv"))==0,"INVJ0_Init: Allocation echouee",return 0);
	I.NumP=NumP;
	I.Xmin=0;
	I.Xmax=INVJ0_DichotomicFindX(0,0,4,0.01f/NumP);
	I.Ymin=0;
	I.Ymax=1;
	for(int y=0;y<NumP;y++)
	{
		float fYSearch=y*(I.Ymax-I.Ymin)/NumP+I.Ymin;
		I.D[y]=INVJ0_DichotomicFindX(fYSearch,I.Xmin,I.Xmax,0.01f/NumP);
	}
	return -1;
}

float SPG_CONV INVJ0_DichotomicFindX(float YSearch,float Xmin, float Xmax, float Tol)
{
	float Y0=(float)_j0(Xmin);
	float Y1=(float)_j0(Xmax);
	if((Y0<=YSearch)&&(Y1>=YSearch))
	{
		while(1)
		{
			if(fabs(Y0-YSearch)<Tol) return Xmin;
			if(fabs(Y1-YSearch)<Tol) return Xmax;
			float NewX=0.5f*(Xmin+Xmax);
			float Yn=(float)_j0(NewX);
			if(Yn>YSearch) 
			{
				Y1=Yn;
				Xmax=NewX;
			}
			else
			{
				Y0=Yn;
				Xmin=NewX;
			}
		}
	}
	else
	{
		while(1)
		{
			if(fabs(Y0-YSearch)<Tol) return Xmin;
			if(fabs(Y1-YSearch)<Tol) return Xmax;
			float NewX=0.5f*(Xmin+Xmax);
			float Yn=(float)_j0(NewX);
			if(Yn<YSearch) 
			{
				Y1=Yn;
				Xmax=NewX;
			}
			else
			{
				Y0=Yn;
				Xmin=NewX;
			}
		}
	}
}

void SPG_CONV INVJ0_Invert(SPG_INVERSEUR& I, float& X, float Y)
{
	int iYIndex=V_FloatToInt((I.NumP-1)*((Y-I.Ymin)/(I.Ymax-I.Ymin)));
	X=I.D[V_Sature(iYIndex,0,I.NumP)];
}

void SPG_CONV INVJ0_Close(SPG_INVERSEUR& I)
{
	if(I.D) SPG_MemFree(I.D);
	memset(&I,0,sizeof(SPG_INVERSEUR));
}

#endif


