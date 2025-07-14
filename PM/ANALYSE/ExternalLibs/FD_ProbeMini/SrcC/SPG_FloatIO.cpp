
#include "SPG_General.h"

#ifdef SPG_General_USEFLOATIO

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>

#define MaxTryHeader 4096

float* SPG_CONV FIO_ReadRF2D(const char * FName,int& SizeX, int& SizeY)
{
	SizeX=SizeY=0;
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"FIO_ReadFloat2D: Impossible d'ouvrir le fichier",FName,return 0);

	fseek(F,0,SEEK_END);
	int FileLen=ftell(F);
	fseek(F,0,SEEK_SET);

	int TryHeaderSize;
	for(TryHeaderSize=0;TryHeaderSize<V_Min(FileLen,MaxTryHeader);TryHeaderSize++)
	{
		SizeY=sqrtInt((FileLen-TryHeaderSize)/sizeof(float));
		if((FileLen-TryHeaderSize)%(SizeY*sizeof(float))==0)
		{
			SizeX=(FileLen-TryHeaderSize)/(SizeY*sizeof(float));
			break;
		}
	}
	if(SizeX==0) 
	{
		SizeY=0;
		return 0;
	}

	char Msg[512];
	sprintf(Msg,"%s\n%d Colonnes, %d Lignes",FName,SizeX,SizeY);

#ifdef SPG_General_USEWindows
#ifdef SPG_General_USEGlobal
	if (MessageBox((HWND)Global.hWndWin,Msg,"FIO_ReadRF2D",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#else
	if (MessageBox(0,Msg,"FIO_ReadRF2D",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#endif
#endif

	float* Data=SPG_TypeAlloc(SizeX*SizeY,float,"RF2D");
	fseek(F,TryHeaderSize,SEEK_SET);
	fread(Data,SizeX*SizeY*sizeof(float),1,F);
	fclose(F);
	return Data;
}

float* SPG_CONV FIO_ReadRF1D(const char * FName,int& NumS)
{
	NumS=0;
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"P_ReadFloat: Impossible d'ouvrir le fichier",FName,return 0);

	fseek(F,0,SEEK_END);
	int FileLen=ftell(F);
	fseek(F,0,SEEK_SET);

	NumS=FileLen/sizeof(float);

	if(NumS==0) 
	{
		return 0;
	}
	float* Data=SPG_TypeAlloc(NumS,float,"Raw float data");
	fread(Data,NumS*sizeof(float),1,F);
	fclose(F);
	return Data;
}

float* SPG_CONV FIO_ReadRS2D(const char * FName,int& SizeX, int& SizeY)
{
	SizeX=SizeY=0;
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"FIO_ReadShort2D: Impossible d'ouvrir le fichier",FName,return 0);

	fseek(F,0,SEEK_END);
	int FileLen=ftell(F);
	fseek(F,0,SEEK_SET);

	int TryHeaderSize;
	for(TryHeaderSize=0;TryHeaderSize<V_Min(FileLen,MaxTryHeader);TryHeaderSize++)
	{
		SizeY=sqrtInt((FileLen-TryHeaderSize)/sizeof(SHORT));
		if((FileLen-TryHeaderSize)%(SizeY*sizeof(SHORT))==0)
		{
			SizeX=(FileLen-TryHeaderSize)/(SizeY*sizeof(SHORT));
			break;
		}
	}
	if(SizeX==0) 
	{
		SizeY=0;
		return 0;
	}

	char Msg[512];
	sprintf(Msg,"%s\n%d Colonnes, %d Lignes",FName,SizeX,SizeY);

#ifdef SPG_General_USEWindows
#ifdef SPG_General_USEGlobal
	if (MessageBox((HWND)Global.hWndWin,Msg,"FIO_ReadRS2D",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#else
	if (MessageBox(0,Msg,"FIO_ReadRS2D",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#endif
#endif

	
	float* Data=SPG_TypeAlloc(SizeX*SizeY,float,"RS2D");
	fseek(F,TryHeaderSize,SEEK_SET);
	for(int i=0;i<SizeX*SizeY;i++)
	{
		SHORT Z;
		fread(&Z,sizeof(SHORT),1,F);
		Data[i]=(float)Z;
	}
	fclose(F);
	return Data;
}

float* SPG_CONV FIO_ReadRS1D(const char * FName,int& NumS)
{
	NumS=0;
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"FIO_ReadShort1D: Impossible d'ouvrir le fichier",FName,return 0);

	fseek(F,0,SEEK_END);
	int FileLen=ftell(F);
	fseek(F,0,SEEK_SET);

	NumS=FileLen/sizeof(SHORT);

	if(NumS==0) 
	{
		return 0;
	}
	float* Data=SPG_TypeAlloc(NumS,float,"RS1D");
	fseek(F,0,SEEK_SET);
	for(int i=0;i<NumS;i++)
	{
		SHORT Z;
		fread(&Z,sizeof(SHORT),1,F);
		Data[i]=(float)Z;
	}
	fclose(F);
	return Data;
}

float* SPG_CONV FIO_ReadRUS2D(const char * FName,int& SizeX, int& SizeY)
{
	SizeX=SizeY=0;
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"FIO_ReadUShort: Impossible d'ouvrir le fichier",FName,return 0);

	fseek(F,0,SEEK_END);
	int FileLen=ftell(F);
	fseek(F,0,SEEK_SET);

	int TryHeaderSize;
	for(TryHeaderSize=0;TryHeaderSize<V_Min(FileLen,MaxTryHeader);TryHeaderSize++)
	{
		SizeY=V_FloatToInt(sqrt((float)(FileLen-TryHeaderSize)/sizeof(WORD)));
		if((FileLen-TryHeaderSize)%(SizeY*sizeof(WORD))==0)
		{
			SizeX=(FileLen-TryHeaderSize)/(SizeY*sizeof(WORD));
			break;
		}
	}
	if(SizeX==0) 
	{
		SizeY=0;
		return 0;
	}

	char Msg[512];
	sprintf(Msg,"%s\n%d Colonnes, %d Lignes",FName,SizeX,SizeY);
	
#ifdef SPG_General_USEWindows
#ifdef SPG_General_USEGlobal
	if (MessageBox((HWND)Global.hWndWin,Msg,"FIO_ReadRUS2D",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#else
	if (MessageBox(0,Msg,"FIO_ReadRUS2D",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#endif
#endif
	
	float* Data=SPG_TypeAlloc(SizeX*SizeY,float,"RUS2D");
	fseek(F,TryHeaderSize,SEEK_SET);
	for(int i=0;i<SizeX*SizeY;i++)
	{
		WORD Z;
		fread(&Z,sizeof(WORD),1,F);
		Data[i]=(float)Z;
	}
	fclose(F);
	return Data;
}

float* SPG_CONV FIO_ReadRUS1D(const char * FName,int& NumS)
{
	NumS=0;
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"FIO_ReadUShort: Impossible d'ouvrir le fichier",FName,return 0);

	fseek(F,0,SEEK_END);
	int FileLen=ftell(F);
	fseek(F,0,SEEK_SET);

	NumS=FileLen/sizeof(WORD);

	if(NumS==0) 
	{
		return 0;
	}
	float* Data=SPG_TypeAlloc(NumS,float,"RUS1D");
	fseek(F,0,SEEK_SET);
	for(int i=0;i<NumS;i++)
	{
		WORD Z;
		fread(&Z,sizeof(WORD),1,F);
		Data[i]=(float)Z;
	}
	fclose(F);
	return Data;
}

float* SPG_CONV FIO_ReadRB1D(const char * FName,int& NumS)
{
	NumS=0;
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"FIO_ReadUShort: Impossible d'ouvrir le fichier",FName,return 0);

	fseek(F,0,SEEK_END);
	int FileLen=ftell(F);
	fseek(F,0,SEEK_SET);

	NumS=FileLen/sizeof(BYTE);

	if(NumS==0) 
	{
		return 0;
	}
	float* Data=SPG_TypeAlloc(NumS,float,"RB1D");
	fseek(F,0,SEEK_SET);
	for(int i=0;i<NumS;i++)
	{
		BYTE Z;
		fread(&Z,sizeof(BYTE),1,F);
		Data[i]=(float)Z;
	}
	fclose(F);
	return Data;
}


#endif


