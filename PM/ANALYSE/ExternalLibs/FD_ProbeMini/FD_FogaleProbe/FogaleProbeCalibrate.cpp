
#include "..\SrcC\SPG.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
//#include "..\SrcC\SPG_SysInc.h"
#include <windows.h>


#include "FogaleProbe.h"
#include "FogaleProbeParamID.h"
#include "FogaleProbeReturnValues.h"


int FPAGetIndex(int ProbeID, char* MaterialName, double* Index)
{
	{
		int NumberOfMaterialsInCatalogue=0;
		FPGetParam(ProbeID,&NumberOfMaterialsInCatalogue,FPID_I_GETNUMMATERIALS);

		for(int MaterialIndex=0; MaterialIndex<NumberOfMaterialsInCatalogue; MaterialIndex++)
		{
			char lName[256];
			FPGetParam(ProbeID,lName, FPID_C_GETMATERIALNAME+MaterialIndex);

			if(stricmp(MaterialName,
					  lName)==0)
			{
				FPGetParam(ProbeID,Index, FPID_D_GETMATERIALINDEX+MaterialIndex);
				return 1;
			}
		}
	}

	/* The material has *not* been found. We check if the material string equals a number, i.e if the user directly specified the group refractive index. */
	//CHECK(IsNumeric(MaterialName),"GetMaterialIndex: MaterialName not found in database and not numeric",Index=1;return 0);

	*Index = strtod(MaterialName, NULL);
	//la convention pour IsNumeric c'est indice absolu (hypothèse) on ne convertit pas //ConvertAirRelativeIndicesToVacuumRelativeIndices(M, Wavelength_m, TEMPERATURE_ABSOLUTE_ZERO_IN_K + 20, 101330, 50, RefractiveIndex, GroupRefractiveIndex);
	return 2;//c'est une valeur numerique d'indice de groupe, l'indice de phase n'est pas exact
}


int FPADefineSample(int ProbeID, char* Name, char* SampleNumber, double* Thickness, double* Tolerance, char** MatName, double* Type, int NbThickness, double Gain, double QualityThreshold)
{
	double Index[256];
	{for(int i=0;i<NbThickness;i++)
	{
		FPAGetIndex(ProbeID,MatName[i],Index+i);
	}}
	return FPDefineSample(ProbeID,Name,SampleNumber,Thickness,Tolerance,Index,Type,NbThickness,Gain,QualityThreshold);
}

int FPADefineZ20Sample(int ProbeID, double Gain)
{
	double Index[4], Thickness[4], Tolerance[4], Type[4];
	int NbThickness=4;
	FPAGetIndex(ProbeID,"Air",		Index+0); Thickness[0]=159000; Tolerance[0]=10000; Type[0]=0;
	FPAGetIndex(ProbeID,"Zerodur",	Index+1); Thickness[1]=5000;   Tolerance[1]=1000;  Type[0]=0;
	FPAGetIndex(ProbeID,"Air",		Index+2); Thickness[2]=20000;  Tolerance[2]=2000;  Type[0]=0;
	FPAGetIndex(ProbeID,"Zerodur",	Index+3); Thickness[3]=5000;   Tolerance[3]=1000;  Type[0]=0;
	return FPDefineSample(ProbeID, "Zerodur20", "", Thickness, Tolerance, Index, Type, NbThickness, Gain, 0);
}

int FPAGainLoopZ20(int ProbeID)
{
	FILE* F=fopen("FPAGainLoopZ20.txt","wb+");
	for(int i=0;i<250;i++)
	{
		Sleep(1000);
		double Gain=0.7+0.005*i;
		FPADefineZ20Sample(ProbeID,Gain);
		double T[4],Q[4];
		FPGetThickness(ProbeID,T,Q,4);
		fprintf(F,"%.4f\t",Gain);
		{for(int i=0;i<4;i++)
		{
			fprintf(F,"%.4f\t",T[i]);
		}}
		{for(int i=0;i<4;i++)
		{
			fprintf(F,"%.4f\t",Q[i]);
		}}
		fprintf(F,"\r\n");
	}
	fclose(F);
	return -1;
}