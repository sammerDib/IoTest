
#include "SPG_General.h"

#ifdef SPG_General_USEACC

#include <string.h>

#ifdef DebugFloat
#include <float.h>
#endif

#include "SPG_Includes.h"

int SPG_CONV Acc_Create(SPG_ACCUMULATEUR& AC, int MaxVals)
{
	memset(&AC,0,sizeof(SPG_ACCUMULATEUR));
	CHECK(MaxVals<=0,"Acc_Create: La taille doit etre specifiee",return 0);
	CHECK((AC.AV=SPG_TypeAlloc(MaxVals,SPG_ACC_VAL,"Accumulateur"))==0,"Acc_Create: Allocation echouee",return 0);
	AC.NumS=MaxVals;
	//AC.EmptyPlaces=MaxVals;
	AC.Pos=0;
	return -1;
}

void SPG_CONV Acc_Close(SPG_ACCUMULATEUR& AC)
{
	if(AC.AV) SPG_MemFree(AC.AV);
	memset(&AC,0,sizeof(SPG_ACCUMULATEUR));
	return;
}

void SPG_CONV Acc_SetVal(SPG_ACCUMULATEUR& AC, float X, float Y, float Ponderation)
{
	CHECK(AC.AV==0,"Acc_SetVal: Accumulateur non initialise",return);
	CHECK(AC.Pos==AC.NumS,"Acc_SetVal: Accumulateur plein",Acc_Clear(AC));
	CHECK(Ponderation==0,"Acc_SetVal: Ponderation nulle",return);
	//CHECK(AC.EmptyPlaces==0,"Acc_SetVal: Accumulateur plein",return);
	//while(AC.AV[AC.Pos].Ponderation!=0)
	//{
	//	AC.Pos++;
	//	if(AC.Pos>=AC.NumS) AC.Pos=0;
	//}
	AC.AV[AC.Pos].X=X;
	AC.AV[AC.Pos].Y=Y;
	if(AC.Pos==0) 
	{
		AC.XMax=AC.XMin=X;
		AC.YMax=AC.YMin=Y;
	}
	if(X<AC.XMin) AC.XMin=X;
	if(X>AC.XMax) AC.XMax=X;
	if(Y<AC.YMin) AC.YMin=Y;
	if(Y>AC.YMax) AC.YMax=Y;
	AC.AV[AC.Pos].Ponderation=Ponderation;
	//AC.EmptyPlaces--;
	AC.Pos++;
	return;
}

void SPG_CONV Acc_GetMoyenne(SPG_ACCUMULATEUR& AC,float& M)
{
	M=0;
	CHECK(AC.AV==0,"Acc_GetMoyenne: Accumulateur non initialise",return);
	CHECK(AC.Pos==0,"Acc_GetMoyenne: Accumulateur vide",return);
	float P=0;
	for(int i=0;i<AC.Pos;i++)
	{
		M+=AC.AV[i].Y*AC.AV[i].Ponderation;
		P+=AC.AV[i].Ponderation;
	}
	M/=P;
	return;
}

void SPG_CONV Acc_GetFirstMoment(SPG_ACCUMULATEUR& AC,float& M)
{
	M=0;
	CHECK(AC.AV==0,"Acc_GetFirstMoment: Accumulateur non initialise",return);
	CHECK(AC.Pos==0,"Acc_GetFirstMoment: Accumulateur vide",return);
	float P=0;
	for(int i=0;i<AC.Pos;i++)
	{
		float KX=AC.AV[i].X*AC.AV[i].Ponderation;
		M+=AC.AV[i].Y*KX;
		P+=KX;
	}
	M/=P;
	return;
}

void SPG_CONV Acc_GetSecondMoment(SPG_ACCUMULATEUR& AC,float& M)
{
	M=0;
	CHECK(AC.AV==0,"Acc_GetSecondMoment: Accumulateur non initialise",return);
	CHECK(AC.Pos==0,"Acc_GetSecondMoment: Accumulateur vide",return);
	float P=0;
	for(int i=0;i<AC.Pos;i++)
	{
		float KXX=AC.AV[i].X*AC.AV[i].X*AC.AV[i].Ponderation;
		M+=AC.AV[i].Y*KXX;
		P+=KXX;
	}
	M/=P;
	return;
}

void SPG_CONV Acc_Draw(SPG_ACCUMULATEUR& AC, G_Ecran& E)
{
	CHECK(AC.AV==0,"Acc_Draw: Accumulateur non initialise",return);
	CHECK(AC.Pos==0,"Acc_Draw: Accumulateur vide",return);
	if(AC.XMin==AC.XMax) return;
	if(AC.YMin==AC.YMax) return;
	//G_DrawRect(E,0,0,E.SizeX,E.SizeY,0xffffff);
	for(int i=0;i<AC.Pos;i++)
	{
		int X=V_FloatToInt((E.SizeX-1)*(AC.AV[i].X-AC.XMin)/(AC.XMax-AC.XMin));
		int Y=V_FloatToInt((E.SizeY-1)*(AC.AV[i].Y-AC.YMin)/(AC.YMax-AC.YMin));
		G_DrawPixel(E,X-1,Y,0xff0000);
		G_DrawPixel(E,X,Y,0xff0000);
		G_DrawPixel(E,X+1,Y,0xff0000);
		G_DrawPixel(E,X,Y-1,0xff0000);
		G_DrawPixel(E,X,Y+1,0xff0000);
	}
	/*
	float C;
	Acc_GetMoyenne(AC,C);
	float B;
	Acc_GetFirstMoment(AC,B);
	float A;
	Acc_GetSecondMoment(AC,A);
	for(i=0;i<E.SizeX;i++)
	{
		float X=i*(AC.XMax-AC.XMin)/(E.SizeX-1)+AC.XMin;
		float P=A*X*X+B*X+C;
		int Y=(E.SizeY-1)*(P-AC.YMin)/(AC.YMax-AC.YMin);
		G_DrawPixel(E,X,Y,0x000000);
	}
	*/
	return;
}


void SPG_CONV Acc_VolatileDraw(SPG_ACCUMULATEUR& AC, G_Ecran& E, float x, float y, DWORD Couleur)
{
	CHECK(AC.AV==0,"Acc_Draw: Accumulateur non initialise",return);
	CHECK(AC.Pos==0,"Acc_Draw: Accumulateur vide",return);
	if(AC.XMin==AC.XMax) return;
	if(AC.YMin==AC.YMax) return;
	int X=V_FloatToInt((E.SizeX-1)*(x-AC.XMin)/(AC.XMax-AC.XMin));
	int Y=V_FloatToInt((E.SizeY-1)*(y-AC.YMin)/(AC.YMax-AC.YMin));
	G_DrawPixel(E,X-1,Y,Couleur);
	G_DrawPixel(E,X,Y,Couleur);
	G_DrawPixel(E,X+1,Y,Couleur);
	G_DrawPixel(E,X,Y-1,Couleur);
	G_DrawPixel(E,X,Y+1,Couleur);
	return;
}

#endif


