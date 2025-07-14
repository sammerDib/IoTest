

#include "SPG_General.h"

#ifdef SPG_General_USEHDF

#include "SPG_Includes.h"

#include <stdio.h>

float* SPG_CONV HDF_Read(char*FName, int &SizeX, int &SizeY)
{
	FILE*F=fopen(FName,"rb");
	CHECKTWO(F==0,"HDF_Read: Ne peut ouvrir le fichier",FName,return 0);
	fseek(F,0,SEEK_END);
	int TotalSize=ftell(F);
	int ApproxSize=V_Floor(sqrt((float)TotalSize/2));//2=16bits
	int Sz;
	for(Sz=4;Sz<ApproxSize;Sz<<=1);
	Sz>>=1;
	SizeX=Sz;
	SizeY=Sz;
	fseek(F,TotalSize-SizeX*SizeY*2,SEEK_SET);// *2=16bits
	
	float*Result=SPG_TypeAlloc(SizeX*SizeY,float,"HDF_Read data");

	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			
			SHORT Temp16;
			fread(&Temp16,2,1,F);
			Result[x+SizeX*y]=(float)Temp16;
			
			/*
			char Temp8;
			fread(&Temp8,1,1,F);
			Result[x+SizeX*y]=(float)Temp8;
			*/
			
		}
	}

	fclose(F);
	return Result;
}

#endif

