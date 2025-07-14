

#include "SPG_General.h"

#ifdef SPG_General_USEDCRZ

#include "SPG_Includes.h"

#include <string.h>

void SPG_CONV DCRZ_SetEquiDist(DCRZ_TYPE& D)
{
	for(int n=0;n<=D.NumLevels;n++)
	{
		D.LevelFutur[n]=D.LevelActuel[n]=(n*MAX_LEVEL_VALUE)/D.NumLevels;
	}
	return;
}

int SPG_CONV DCRZ_Init(DCRZ_TYPE& D, int NumLevels)
{
	memset(&D,0,sizeof(DCRZ_TYPE));
	CHECK((D.LevelActuel=SPG_TypeAlloc(NumLevels+1,DCRZ_LEVEL_TYPE,"Discretiseur"))==0,"DCRZ_Init: Allocation echouee",return 0);
	CHECK((D.LevelFutur=SPG_TypeAlloc(NumLevels+1,DCRZ_LEVEL_TYPE,"Discretiseur"))==0,"DCRZ_Init: Allocation echouee",return 0);
	CHECK((D.Cumul=SPG_TypeAlloc(NumLevels,DCRZ_CUMUL_TYPE,"Discretiseur"))==0,"DCRZ_Init: Allocation echouee",return 0);
	D.NumLevels=NumLevels;
	DCRZ_SetEquiDist(D);
	DCRZ_ResetCumul(D);
	return -1;
}

void SPG_CONV DCRZ_Close(DCRZ_TYPE& D)
{
	if (D.LevelActuel) SPG_MemFree(D.LevelActuel);
	if (D.LevelFutur) SPG_MemFree(D.LevelFutur);
	if (D.Cumul) SPG_MemFree(D.Cumul);
	memset(&D,0,sizeof(DCRZ_TYPE));
	return;
}

#ifdef SPG_General_USEHIST

void SPG_CONV DCRZ_SetFromHist(DCRZ_TYPE& D,HIST_TYPE& H)
{
	CHECK(D.NumLevels==H.NumCater,"Tailles Discretiseur et Histogramme discordantes",return);
	for(int n=0;n<D.NumLevels;n++)
	{
		D.LevelFutur[n]=(H.Cumul[n-1]*D.LevelActuel[n-1]+H.Cumul[n]*D.LevelActuel[n+1])/(H.Cumul[n-1]+H.Cumul[n]);
	}
	V_Swap(DCRZ_LEVEL_TYPE*,D.LevelFutur,D.LevelActuel);
	return;
}

#endif

void SPG_CONV DCRZ_Discretise(DCRZ_TYPE& DCRZ_VAR,float FLOAT_ZERO_ONE,int& RESULTAT)
{
RESULTAT=0;
float DCRZ_D_MACRO=FLOAT_ZERO_ONE;
while(
	  (RESULTAT<(DCRZ_VAR.NumLevels-1))
	  &&
	  ((DCRZ_D_MACRO*DCRZ_VAR.LevelActuel[DCRZ_VAR.NumLevels])>DCRZ_VAR.LevelActuel[RESULTAT+1])
	  )
RESULTAT++;
return;
}


void SPG_CONV DCRZ_DrawLevels(DCRZ_TYPE& D,G_Ecran& E, DWORD Couleur)
{
	for(int x=0;x<D.NumLevels;x++)
	{
		G_DrawLine(E,
			(x*E.SizeX)/D.NumLevels,
			(D.LevelActuel[x]*E.SizeY)/D.LevelActuel[D.NumLevels],
			((x+1)*E.SizeX)/D.NumLevels,
			(D.LevelActuel[x+1]*E.SizeY)/D.LevelActuel[D.NumLevels],
			Couleur);
		G_DrawLine(E,
			(x*E.SizeX)/D.NumLevels,
			(D.LevelFutur[x]*E.SizeY)/D.LevelFutur[D.NumLevels],
			((x+1)*E.SizeX)/D.NumLevels,
			(D.LevelFutur[x+1]*E.SizeY)/D.LevelFutur[D.NumLevels],
			Couleur&0x555555);
	}
	return;
}

void SPG_CONV DCRZ_SetLevels(DCRZ_TYPE& D)
{
	//return;

	for(int n=1;n<D.NumLevels;n++)
	{
int Ecart=D.Cumul[n]-D.Cumul[n-1];
if(Ecart>0)
{
	D.LevelFutur[n]=
		D.LevelActuel[n]+(Ecart>>1)*
		(D.LevelActuel[n+1]-D.LevelActuel[n])
		/D.Cumul[n];
}
else if(Ecart<0)
{
	D.LevelFutur[n]=
		D.LevelActuel[n]-(Ecart>>1)*
		(D.LevelActuel[n-1]-D.LevelActuel[n])
		/D.Cumul[n-1];
}
else
{
	D.LevelFutur[n]=D.LevelActuel[n];
}
		CHECK(V_IsBound(D.LevelFutur[n],D.LevelActuel[n-1],D.LevelActuel[n+1])==0,"DCRZ_SetLevels: Depassement interne",return);
	}

	for(n=1;n<D.NumLevels;n++)
	{
		//DCRZ_LEVEL_TYPE L=((D.LevelActuel[n]+D.LevelFutur[n])>>1);
		//DCRZ_LEVEL_TYPE L=((D.LevelActuel[n]+3*D.LevelFutur[n])>>2);
		DCRZ_LEVEL_TYPE L=D.LevelFutur[n];
		D.LevelActuel[n]=V_Max(L,(2+D.LevelActuel[n-1]));
		//D.LevelActuel[n]=(n*MAX_LEVEL_VALUE)/D.NumLevels;//((3*(n*MAX_LEVEL_VALUE)/D.NumLevels+D.LevelFutur[n])>>2);
	}
	/*
	D.LevelActuel[D.NumLevels]=V_Max(MAX_LEVEL_VALUE,(2+D.LevelActuel[n-1]));
	if(D.LevelActuel[D.NumLevels]>MAX_LEVEL_VALUE)
	{
	for(n=1;n<=D.NumLevels;n++)
	{
		D.LevelActuel[n]>>=1;
		SPG_List("DCRZ_SetLevels: Renormalisation :2");
	}
	}
	else if(D.LevelActuel[D.NumLevels]<MAX_LEVEL_VALUEd2)
	{
	for(n=1;n<=D.NumLevels;n++)
	{
		D.LevelActuel[n]<<=1;
		SPG_List("DCRZ_SetLevels: Renormalisation x2");
	}
	}
	*/


	//V_Swap(DCRZ_LEVEL_TYPE*,D.LevelFutur,D.LevelActuel);
	return;
}

#endif


