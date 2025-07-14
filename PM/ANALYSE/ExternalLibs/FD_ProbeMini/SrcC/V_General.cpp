
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#ifdef SPG_General_PGLib
#include "PGLib\PGlib.h"
#pragma SPGMSG(__FILE__,__LINE__,"V_General using PGLib")
#endif

#include "V_General.h"
//#include "SPG_Graphics.h"
//#include "SGRAPH.h"

/*
long V_Max(long A, long B)
{
	return (A>B)?A:B;
}

long V_Min(long A, long B)
{
	return (A<B)?A:B;
}
*/
/*
void ZeroV(V_VECT& V)
{
	V.z=V.y=V.x=0;
}

void V_AddVect(V_VECT& V1, V_VECT& V2, V_VECT& V_VRES)
{
	V_VRES.x=V1.x+V2.x;
	V_VRES.y=V1.y+V2.y;
	V_VRES.z=V1.z+V2.z;
	return;
}

void V_SubVect(V_VECT& V1, V_VECT& V2, V_VECT& V_VRES)
{
	V_VRES.x=V1.x-V2.x;
	V_VRES.y=V1.y-V2.y;
	V_VRES.z=V1.z-V2.z;
	return;
}

void V_AddCVect(float C1, V_VECT& V1,float C2, V_VECT& V2, V_VECT& V_VRES)
{
	V_VRES.x=C1*V1.x+C2*V2.x;
	V_VRES.y=C1*V1.y+C2*V2.y;
	V_VRES.z=C1*V1.z+C2*V2.z;
	return;
}

void V_MulVect(float C, V_VECT& V_VRES)
{
	V_VRES.x=C*V_VRES.x;
	V_VRES.y=C*V_VRES.y;
	V_VRES.z=C*V_VRES.z;
	return;
}
*/
/*
float V_ScalVect(V_VECT& V1, V_VECT& V2)
{
	return V1.x*V2.x+V1.y*V2.y+V1.z*V2.z;
}
*/

int SPG_CONV V_BitReverse(int x, int nb)
{
	int R=0;
	for(;nb>0;nb--)
	{
		R<<=1;
		R|=x&1;
		x>>=1;
	}
	return R;
}

/*
void SPG_FASTCONV V_Normalise(V_VECT& V)
{
	float N=1/V_ModVect(V);
	V_Operate1(V,*=N);
	return;
}
*/
/*
void SPG_FASTCONV V_VectVect(V_VECT& V1, V_VECT& V2, V_VECT& V_VRES)
{
	V_VRES.x=V1.y*V2.z-V1.z*V2.y;
	V_VRES.y=V1.z*V2.x-V1.x*V2.z;
	V_VRES.z=V1.x*V2.y-V1.y*V2.x;
	return;
}
*/
/*
float V_Mod2Vect(V_VECT& V1)
{
	return V1.x*V1.x+V1.y*V1.y+V1.z*V1.z;
}

float V_ModVect(V_VECT& V1)
{
	return (float)sqrt(V1.x*V1.x+V1.y*V1.y+V1.z*V1.z);
}
*/
/*
void SPG_FASTCONV V_TranslateRep(V_REPERE& Rep, V_VECT& Transl,float DT)
{
	V_Operate4(Rep.pos,+=DT*Transl.x*Rep.axex,+DT*Transl.y*Rep.axey,+DT*Transl.z*Rep.axez);
}

void SPG_FASTCONV V_RotateRep(V_REPERE& Rep, V_VECT& Rotat,float DT)
{
	V_REPERE NewRep=Rep;
	V_Operate3(NewRep.axex,+=DT*Rotat.z*Rep.axey,-DT*Rotat.y*Rep.axez);
	V_Operate3(NewRep.axey,+=DT*Rotat.x*Rep.axez,-DT*Rotat.z*Rep.axex);
	V_Operate3(NewRep.axez,+=DT*Rotat.y*Rep.axex,-DT*Rotat.x*Rep.axey);
	V_Orthonorme(NewRep);
	Rep=NewRep;
}
*/

/*
//calcule la position absolue d'un point a partir de ses coordonnees relatives
void V_CalcAbs(V_REPERE& Rep,V_VECT& V,V_VECT& Absolu)
{	
	V_Operate5(Absolu,=Rep.pos,+V.x*Rep.axex,+V.y*Rep.axey,+V.z*Rep.axez);
	return;
}
*/

/*
//projette un vecteur (FORCE) dans un repere
void SPG_FASTCONV V_ProjRep(V_REPERE &Rep,V_VECT &V,V_VECT &V_Res)
{
	V_Res.x=V_ScalVect(V,Rep.axex);
	V_Res.y=V_ScalVect(V,Rep.axey);
	V_Res.z=V_ScalVect(V,Rep.axez);
	return;
}
*/

/*
//calcule la position relative d'un point a partir des ses coordonnees absolues
void SPG_FASTCONV V_CalcRep(V_REPERE &Rep,V_VECT &V,V_VECT &V_Res)
{
	V_VECT Absolu;
	V_Operate3(Absolu,=V,-Rep.pos);
	V_Res.x=V_ScalVect(Absolu,Rep.axex);
	V_Res.y=V_ScalVect(Absolu,Rep.axey);
	V_Res.z=V_ScalVect(Absolu,Rep.axez);
	return;
}
*/

/*
//calcule la position relative d'un point a partir des ses coordonnees relatives
void SPG_FASTCONV V_CalcRepRep(V_REPERE& Rep, V_VECT& V, V_REPERE& NewRep, V_VECT& NewV)
{
	V_VECT Absolu;
	V_CalcAbs(Rep, V, Absolu);
	
	V_VECT T;
	//V_SubVect(Absolu,NewRep.pos,T);
	V_Operate3(T,=Absolu,-NewRep.pos);
	NewV.x=V_ScalVect(T,NewRep.axex);
	NewV.y=V_ScalVect(T,NewRep.axey);
	NewV.z=V_ScalVect(T,NewRep.axez);
	return;
}
*/

/*
void SPG_FASTCONV V_Orthonorme(V_REPERE& Rep)
{
	V_Normalise(Rep.axex);
	{
	float s1=V_ScalVect(Rep.axex,Rep.axey);
	V_Operate2(Rep.axey,-=s1*Rep.axex);
	}
	V_Normalise(Rep.axey);
	{
	float s1=V_ScalVect(Rep.axex,Rep.axez);
	V_Operate2(Rep.axez,-=s1*Rep.axex);
	}
	{
	float s1=V_ScalVect(Rep.axey,Rep.axez);
	V_Operate2(Rep.axez,-=s1*Rep.axey);
	}
	V_Normalise(Rep.axez);
}
*/

#endif

