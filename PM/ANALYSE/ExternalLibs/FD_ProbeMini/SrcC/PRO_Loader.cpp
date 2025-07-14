

#include "SPG_General.h"

#ifdef SPG_General_USEPRO

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>
#include <string.h>

#include "DigitalSurf.h"

int SPG_CONV Cut_ReadPRO(Cut& C, char* FName)
{
	memset(&C,0,sizeof(Cut));

	FILE*F=fopen(FName,"rb");
	CHECKTWO(F==0,"PRO_Read: Ne peut ouvrir le fichier",FName,return 0);

	SURF Surf;
	fread(&Surf,sizeof(Surf),1,F);

	/*
	P_Create(P,Surf.nb_pt_ligne,Surf.nb_ligne,
		Surf.pas_x,Surf.pas_y,
		Surf.nom_axe_x,Surf.nom_axe_y,Surf.nom_axe_z,0);
	*/
	Cut_Create(C,Surf.nb_total_pt,Surf.pas_x,"µm","I");

	//float*Result=SPG_TypeAlloc(SizeX*SizeY,float,"HDF_Read data");

	for(int x=0;x<C.NumS;x++)
	{
		SHORT Temp16;
		fread(&Temp16,2,1,F);
		C.D[x]=(float)Temp16;
	}

	fclose(F);
	return -1;
}


int SPG_CONV Cut_WritePRO(Cut& C, char* FName)
{
#define AFAIRE_CUTWRITEPRO
#ifdef DebugList
	SPG_List("Cut_WritePRO: Cette fonctionnalité n'est pas encore implementee");
#endif
	return 0;
}

void SPG_CONV PRO_BatchConvertToTXT(char * WorkDir)
{
	Cut C;

	char FName[1024];
	SPG_ConcatPath(FName,WorkDir,"*.pro");

	
	WIN32_FIND_DATA FFD;

	HANDLE H=FindFirstFile(FName,&FFD);
	if(H==INVALID_HANDLE_VALUE) return;

	do
	{
		char CWName[1024];
		SPG_ConcatPath(CWName,WorkDir,FFD.cFileName);
		if (Cut_ReadPRO(C,CWName)) 
		{
			SPG_SetExtens(CWName,".txt");
			Cut_WriteTXT(C,CWName,6,S_RetourChariot,0);
			Cut_Close(C);
		}
	} while(FindNextFile(H,&FFD));

	FindClose(H);

	return;
}

#endif



