
#include "SPG_General.h"

#ifdef SPG_General_USERINGREC

#include "SPG_Includes.h"

#include <memory.h>
#include <stdio.h>

int SPG_CONV RGR_Load(RING_REC& RGR, char* FileName)
{
	memset(&RGR,0,sizeof(RING_REC));
	FILE* F=fopen(FileName,"rb");
	SPG_CHECKTWO(F==0,"RGR_Load: Impossible d'ouvrir le fichier",FileName,return 0);
	fread(&RGR.SizeX,sizeof(int),1,F);
	fread(&RGR.SizeY,sizeof(int),1,F);
	fread(&RGR.NumS,sizeof(int),1,F);
	if(RGR_Init(RGR,RGR.SizeX,RGR.SizeY,RGR.NumS))
	{
		fread(RGR.D,RGR.SizeP*RGR.NumS,sizeof(BYTE),F);
	}
	fclose(F);
	return RGR.Etat;
}

int SPG_CONV RGR_M3DLoad(RING_REC& RGR, char* M3DFileName, WORD* *StartIndex)
{
	memset(&RGR,0,sizeof(RING_REC));
	FILE* F=fopen(M3DFileName,"rb");
	SPG_CHECKTWO(F==0,"RGR_Load: Impossible d'ouvrir le fichier",M3DFileName,return 0);
	if(StartIndex) *StartIndex=0;
	int Etat=0;
	fread(&Etat,sizeof(int),1,F);
	fread(&RGR.SizeX,sizeof(int),1,F);
	fread(&RGR.SizeY,sizeof(int),1,F);
	fread(&RGR.NumS,sizeof(int),1,F);
	if(RGR_Init(RGR,RGR.SizeX,RGR.SizeY,RGR.NumS))
	{
		//CHECK(Etat,"RGR_M3DLoad: File is a fringe record instead of a image sequence",
		if(Etat)
		{
			if(StartIndex==0)
			{
				fseek(F,RGR.SizeX*RGR.SizeY*sizeof(WORD),SEEK_CUR);
			}
			else
			{
				*StartIndex=SPG_TypeAlloc(RGR.SizeX*RGR.SizeY,WORD,"RGR_M3DLoad");
				if(*StartIndex)
				{
					fread(*StartIndex,RGR.SizeX*RGR.SizeY*sizeof(WORD),1,F);
				}
			}
		}
		fread(RGR.D,RGR.SizeP*RGR.NumS,sizeof(BYTE),F);
	}
	fclose(F);
	return RGR.Etat;
}

int SPG_CONV RGR_M3DLoadAndConvert(RING_REC& RGR, char* M3DFileName)
{
	memset(&RGR,0,sizeof(RING_REC));
	FILE* F=fopen(M3DFileName,"rb");
	SPG_CHECKTWO(F==0,"RGR_Load: Impossible d'ouvrir le fichier",M3DFileName,return 0);
	int Etat=0;
	fread(&Etat,sizeof(int),1,F);
	fread(&RGR.SizeX,sizeof(int),1,F);
	fread(&RGR.SizeY,sizeof(int),1,F);
	fread(&RGR.NumS,sizeof(int),1,F);
	WORD* StartIndex=SPG_TypeAlloc(RGR.SizeX*RGR.SizeY,WORD,"RGR_M3DLoadAndConvert");
	if(Etat)
	{
		fread(StartIndex,RGR.SizeX*RGR.SizeY*sizeof(WORD),1,F);
	}

	int iMin=-1;
	int iMax=-1;
	{for(int i=0;i<RGR.SizeX*RGR.SizeY;i++)
	{
		if(StartIndex[i]&32768)
		{
		}
		else
		{
			int iCur=StartIndex[i]&32767;
			if((iMin==-1)||(iCur<iMin)) iMin=iCur;
			if((iMax==-1)||(iCur>iMax)) iMax=iCur;
		}
	}}

	const int FileNumS=RGR.NumS;
	const int RGRNumS=RGR.NumS+iMax-iMin;
	if(RGR_Init(RGR,RGR.SizeX,RGR.SizeY,RGRNumS))
	{
		BYTE* LS=SPG_TypeAlloc(RGR.NumS,BYTE,"RGR_M3DLoadAndConvert");
		//CHECK(Etat,"RGR_M3DLoad: File is a fringe record instead of a image sequence",
		fread(RGR.D,RGR.SizeP*RGR.NumS,sizeof(BYTE),F);
		{for(int i=0;i<RGR.SizeX*RGR.SizeY;i++)
		{
			const int SI=StartIndex[i]&32767;
			BYTE* const RP=RGR.D+i;
			if(SI&32768)
			{
				memset(LS,0,RGRNumS);
			}
			else
			{
				for(int n=0;n<RGRNumS;n++)
				{
					int m=V_Sature((n+iMin),SI,(SI+FileNumS-1));
					LS[n]=RP[(m%FileNumS)*RGR.SizeP];
				}
			}
			for(int n=0;n<RGRNumS;n++)
			{
				RP[n*RGR.SizeP]=LS[n];
			}
		}}
		SPG_MemFree(LS);
	}

	SPG_MemFree(StartIndex);
	fclose(F);
	return RGR.Etat;
}

int SPG_CONV RGR_Save(RING_REC& RGR, char* FileName)
{
	FILE* F=fopen(FileName,"wb+");
	SPG_CHECKTWO(F==0,"RGR_Save: Impossible d'ouvrir le fichier",FileName,return 0);
	fwrite(&RGR.SizeX,sizeof(int),1,F);
	fwrite(&RGR.SizeY,sizeof(int),1,F);
	fwrite(&RGR.NumS,sizeof(int),1,F);
	fwrite(RGR.D,RGR.SizeP*RGR.NumS,sizeof(BYTE),F);
	return -1;
}

int SPG_CONV RGR_M3DSave(RING_REC& RGR, char* M3DFileName)
{
	FILE* F=fopen(M3DFileName,"wb+");
	SPG_CHECKTWO(F==0,"RGR_Save: Impossible d'ouvrir le fichier",M3DFileName,return 0);
	int Etat=0;
	fwrite(&Etat,sizeof(int),1,F);
	fwrite(&RGR.SizeX,sizeof(int),1,F);
	fwrite(&RGR.SizeY,sizeof(int),1,F);
	fwrite(&RGR.NumS,sizeof(int),1,F);
	fwrite(RGR.D,RGR.SizeP*RGR.NumS,sizeof(BYTE),F);
	return -1;
}

int SPG_CONV M3D_RGR_Load(M3D_RGR& RGR, char* FileName)
{
	memset(&RGR,0,sizeof(M3D_RGR));
	FILE* F=fopen(FileName,"rb");
	SPG_CHECKTWO(F==0,"RGR_Load: Impossible d'ouvrir le fichier",FileName,return 0);
	fread(&RGR.Etat,sizeof(int),1,F);
	fread(&RGR.SizeX,sizeof(int),1,F);
	fread(&RGR.SizeY,sizeof(int),1,F);
	fread(&RGR.NumS,sizeof(int),1,F);
	RGR.SizeP=RGR.SizeX*RGR.SizeY;

	RGR.StartIndex=0;
	if(RGR.Etat)
	{
		RGR.StartIndex=SPG_TypeAlloc(RGR.SizeX*RGR.SizeY,WORD,"StartIndex");
		fread(RGR.StartIndex,RGR.SizeX*RGR.SizeY*sizeof(WORD),1,F);
	}
	RGR.D=SPG_TypeAlloc(RGR.SizeX*RGR.SizeY*RGR.NumS,BYTE,"ImgSeq");
	if(RGR.D) fread(RGR.D,RGR.SizeX*RGR.SizeY,RGR.NumS,F);//c'est un point delicat: charger un tableau egal a la RAM

	fclose(F);
	return -1;
}

void SPG_CONV M3D_RGR_Close(M3D_RGR& RGR)
{
	if(RGR.Etat) SPG_MemFree(RGR.StartIndex);
	if(RGR.D) SPG_MemFree(RGR.D);
	memset(&RGR,0,sizeof(M3D_RGR));
	return;
}

#endif

