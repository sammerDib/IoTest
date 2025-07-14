
#include "..\SrcC\SPG_General.h"

#ifdef SPG_General_USEScale

#include "SPG_MinIncludes.h"

#include <memory.h>
#include <float.h>

double SC_Divisions(SPG_Scale& SC, double Delta)
{
	DbgCHECK(Delta<0.1f,"SC_Divisions");//0.05-(-0.05)=0.1+1.5e-9
	DbgCHECK(Delta>1.0f,"SC_Divisions");
	double DivStep=Delta/SC.Divisions;
	if(DivStep<=0.0f) 
		return 0.01;
	if(DivStep<=0.02) 
		return 0.02;
	if(DivStep<=0.05) 
		return 0.05;
	if(DivStep<=0.1) 
		return 0.1;
	if(DivStep<=0.2) 
		return 0.2;
	if(DivStep<=0.5) 
		return 0.5;
	return 1.0;
}

void SC_Compute(SPG_Scale& SC)
{
	SC.Divisions=V_Sature(SC.Divisions,2,SCALE_MAX_TICK-1);
	double NormeGain=SC.fMax-SC.fMin;//0.05-(-0.05)=0.1+1.5e-9
	if(NormeGain<=0) NormeGain=1.0;
	int MagnitudeGain=V_Ceil(log10(NormeGain));
	double NormeUn=pow(10.0,MagnitudeGain);
	CHECK(NormeUn==0,"SC_Compute",NormeUn=NormeGain);
	CHECK(NormeUn>=FLT_MAX,"SC_Compute",NormeUn=NormeGain);
	double DivStep=SC_Divisions(SC,NormeGain/NormeUn)*NormeUn;
	
	int NumTickMin=V_Floor(SC.fMin/DivStep);
	int NumTickMax=V_Ceil(SC.fMax/DivStep);


	SC.NumTick=1+NumTickMax-NumTickMin;

	//CHECK(!V_InclusiveBound(SC.NumTick,2,16),"SC_Compute",
	SC.NumTick=V_Sature(SC.NumTick,2,SCALE_MAX_TICK-1);

	SC.ScaleMin=NumTickMin*DivStep;
	SC.ScaleMax=NumTickMax*DivStep;

	int Chiffres=SC.ExtraDecimal+V_Ceil(log10(V_Max(fabsf(SC.ScaleMax),fabsf(SC.ScaleMin))/DivStep));

	int istart=NumTickMin;
	
	for(int i=0;i<SC.NumTick;i++)
	{
		SC.T[i].Value=(i+istart)*DivStep;
		SC.T[i].Label[0]=0;
		CF_GetString(SC.T[i].Label,SC.T[i].Value,Chiffres);
	}

	return;
}

void SPG_CONV SC_Init(SPG_Scale& SC, int Divisions, int ExtraDecimal, double TimeCste)
{
	memset(&SC,0,sizeof(SPG_Scale));
	SC.Divisions=V_Sature(Divisions,2,SCALE_MAX_TICK-1);
	SC.TimeCste=V_Sature(TimeCste,0,1);
	SC.ExtraDecimal=1+V_Max(ExtraDecimal,0);
	return;
}

void SPG_CONV SC_Set(SPG_Scale& SC, double fMin, double fMax)
{
	SC.fMin=fMin;
	SC.fMax=fMax;
	SC_Compute(SC);
	return;
}

void SPG_CONV SC_Merge(SPG_Scale& SC, double fMin, double fMax)
{
	SC.fMin=V_Min(fMin,SC.fMin);
	SC.fMax=V_Max(fMax,SC.fMax);
	SC_Compute(SC);
	return;
}

void SPG_CONV SC_Update(SPG_Scale& SC, double fMin, double fMax)
{
	CHECK(_finite(fMin)==0,"SC_Update",return);
	CHECK(_finite(fMax)==0,"SC_Update",return);
	double tMin=SC.fMin*(1-SC.TimeCste)+SC.TimeCste*fMin;
	double tMax=SC.fMax*(1-SC.TimeCste)+SC.TimeCste*fMax;
	SC.fMin=V_Min(fMin,tMin);
	SC.fMax=V_Max(fMax,tMax);
	SC_Compute(SC);
	return;
}

#endif

