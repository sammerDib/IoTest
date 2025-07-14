
#include "SPG_General.h"

#ifdef SPG_General_USEWAVIO

#include "SPG_Includes.h"

#include <memory.h>
#include <stdio.h>

typedef struct
{
	SHORT wFormatTag;
	SHORT nChannels;
	LONG SamplesPerSec;
	LONG nAvgBytesPerSec;
	SHORT nBlockAlign;
	SHORT wBitsPerSample;
} SPG_WAVEFORMAT;

typedef struct
{
	LONG SRIFF;
	LONG TM8;
	LONG SWAVE;
	LONG SFMT;
	LONG TEnt;
	SPG_WAVEFORMAT WVF;
	LONG SDATA;
	LONG LGData;
} SPG_ENTETEWAV;


float* SPG_CONV WAV_ReadFloatMono(char* FName, int& NumS, int& Frequence)
{
	FILE *F;
	SPG_StackAllocZ(SPG_ENTETEWAV,EntWav);
	NumS=0;
	F=fopen(FName,"rb");
	CHECKTWO(F==0,"WAV_Read: Impossible d'ouvrir le fichier",FName,return 0);

	fread(&EntWav,sizeof(SPG_ENTETEWAV),1,F);
	
	NumS=8*EntWav.LGData/(EntWav.WVF.wBitsPerSample*EntWav.WVF.nChannels);
	Frequence=EntWav.WVF.SamplesPerSec;
	int Channels=EntWav.WVF.nChannels;

	CHECK((EntWav.WVF.wBitsPerSample!=8)&&(EntWav.WVF.wBitsPerSample!=16)&&(EntWav.WVF.wBitsPerSample!=32),"WAV_Read: Format incorrect",return 0);
	CHECK(!V_InclusiveBound(EntWav.WVF.nChannels,1,4),"WAV_Read: Format incorrect",return 0);
	CHECK(EntWav.WVF.wFormatTag!=1,"WAV_Read: Format incorrect",return 0);

	SPG_PtrAlloc(Data,NumS,float,"WAV_Read");

	if(EntWav.WVF.wBitsPerSample==8)
	{
	for(int i=0;i<NumS;i++)
	{
		Data[i]=0;
		BYTE S=0;
		for(int j=0;j<Channels;j++)
		{
			fread(&S,1,1,F);
			Data[i]+=(float)S;
		}
	}
	}
	else if(EntWav.WVF.wBitsPerSample==16)
	{
	for(int i=0;i<NumS;i++)
	{
		Data[i]=0;
		SHORT S=0;
		for(int j=0;j<Channels;j++)
		{
			fread(&S,2,1,F);
			Data[i]+=(float)S;
		}
	}
	}
	else if(EntWav.WVF.wBitsPerSample==32)
	{
	for(int i=0;i<NumS;i++)
	{
		Data[i]=0;
		LONG S=0;
		for(int j=0;j<Channels;j++)
		{
			fread(&S,4,1,F);
			Data[i]+=(float)S;
		}
	}
	}

	fclose(F);

	return Data;
}

int SPG_CONV WAV_ReadFloatStereo(char* FName, float* &Left, float* &Right, int& NumS, int& Frequence)
{
	FILE *F;
	SPG_StackAllocZ(SPG_ENTETEWAV,EntWav);
	NumS=0;
	F=fopen(FName,"rb");
	CHECKTWO(F==0,"WAV_Read: Impossible d'ouvrir le fichier",FName,return 0);

	fread(&EntWav,sizeof(SPG_ENTETEWAV),1,F);
	
	NumS=8*EntWav.LGData/(EntWav.WVF.wBitsPerSample*EntWav.WVF.nChannels);
	Frequence=EntWav.WVF.SamplesPerSec;
	int Channels=EntWav.WVF.nChannels;

	CHECK((EntWav.WVF.wBitsPerSample!=8)&&(EntWav.WVF.wBitsPerSample!=16)&&(EntWav.WVF.wBitsPerSample!=32),"WAV_Read: Format incorrect",return 0);
	CHECK(!V_InclusiveBound(EntWav.WVF.nChannels,1,4),"WAV_Read: Format incorrect",return 0);
	CHECK(EntWav.WVF.wFormatTag!=1,"WAV_Read: Format incorrect",return 0);

	Left=SPG_TypeAlloc(NumS,float,"WAV_Read");
	Right=SPG_TypeAlloc(NumS,float,"WAV_Read");

	if(EntWav.WVF.wBitsPerSample==8)
	{
	for(int i=0;i<NumS;i++)
	{
		BYTE S=0;
		fread(&S,1,1,F);
		Left[i]=Right[i]=(float)S;
		for(int j=1;j<Channels;j++)
		{
			fread(&S,1,1,F);
			Right[i]=(float)S;
		}
	}
	}
	else if(EntWav.WVF.wBitsPerSample==16)
	{
	for(int i=0;i<NumS;i++)
	{
		SHORT S=0;
		fread(&S,2,1,F);
		Left[i]=Right[i]=(float)S;
		for(int j=1;j<Channels;j++)
		{
			fread(&S,2,1,F);
			Right[i]=(float)S;
		}
	}
	}
	else if(EntWav.WVF.wBitsPerSample==32)
	{
	for(int i=0;i<NumS;i++)
	{
		LONG S=0;
		fread(&S,4,1,F);
		Left[i]=Right[i]=(float)S;
		for(int j=1;j<Channels;j++)
		{
			fread(&S,4,1,F);
			Right[i]=(float)S;
		}
	}
	}

	fclose(F);

	return -1;
}

int SPG_CONV WAV_WriteFloat(float* Df, int NumS, int Frequence, float VMin, float VMax, char* FName)
{
	CHECK(Df==0,"WAV_WriteFloat: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"WAV_WriteFloat: Impossible d'ouvrir le fichier",FName,return 0);

	SPG_StackAllocZ(SPG_ENTETEWAV,EntWav);

	EntWav.SRIFF=*(LONG*)"RIFF";
	EntWav.TM8=sizeof(SPG_ENTETEWAV)+NumS*sizeof(SHORT)-8;
	EntWav.SWAVE=*(LONG*)"WAVE";
	EntWav.SFMT=*(LONG*)"fmt ";
	EntWav.TEnt=sizeof(SPG_WAVEFORMAT);
	EntWav.WVF.wFormatTag=1;
	EntWav.WVF.nChannels=1;
	EntWav.WVF.SamplesPerSec=Frequence;
	EntWav.WVF.nAvgBytesPerSec=Frequence*sizeof(SHORT);
	EntWav.WVF.nBlockAlign=sizeof(SHORT);
	EntWav.WVF.wBitsPerSample=8*sizeof(SHORT);
	EntWav.SDATA=*(LONG*)"data";
	EntWav.LGData=NumS*sizeof(SHORT);


	fwrite(&EntWav,sizeof(SPG_ENTETEWAV),1,F);
	
	for(int i=0;i<NumS;i++)
	{
		float VWAV=65535.99f*(Df[i]-VMin)/(VMax-VMin)-32768.0f;
		SHORT Z=(SHORT)V_Sature(VWAV,-32768.0f,32767.0f);
		fwrite(&Z,sizeof(SHORT),1,F);
	}

	fclose(F);

	return -1;
}

int SPG_CONV WAV_WriteFloatStereo(float* DfL, float* DfR, int NumS, int Frequence, float VMin, float VMax, char* FName)
{
	CHECK(DfL==0,"WAV_WriteFloat: Donnees nulles",return 0);
	CHECK(DfR==0,"WAV_WriteFloat: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"WAV_WriteFloat: Impossible d'ouvrir le fichier",FName,return 0);

	SPG_StackAllocZ(SPG_ENTETEWAV,EntWav);

	EntWav.SRIFF=*(LONG*)"RIFF";
	EntWav.TM8=sizeof(SPG_ENTETEWAV)+NumS*sizeof(SHORT)-8;
	EntWav.SWAVE=*(LONG*)"WAVE";
	EntWav.SFMT=*(LONG*)"fmt ";
	EntWav.TEnt=sizeof(SPG_WAVEFORMAT);
	EntWav.WVF.wFormatTag=1;
	EntWav.WVF.nChannels=2;
	EntWav.WVF.SamplesPerSec=Frequence;
	EntWav.WVF.nAvgBytesPerSec=EntWav.WVF.nChannels*Frequence*sizeof(SHORT);
	EntWav.WVF.nBlockAlign=(short)(EntWav.WVF.nChannels*sizeof(SHORT));
	EntWav.WVF.wBitsPerSample=8*sizeof(SHORT);
	EntWav.SDATA=*(LONG*)"data";
	EntWav.LGData=NumS*EntWav.WVF.nChannels*sizeof(SHORT);


	fwrite(&EntWav,sizeof(SPG_ENTETEWAV),1,F);
	
	for(int i=0;i<NumS;i++)
	{
		{
		float VWAV=65535.99f*(DfL[i]-VMin)/(VMax-VMin)-32768.0f;
		SHORT Z=(SHORT)V_Sature(VWAV,-32768.0f,32767.0f);
		fwrite(&Z,sizeof(SHORT),1,F);
		}
		{
		float VWAV=65535.99f*(DfR[i]-VMin)/(VMax-VMin)-32768.0f;
		SHORT Z=(SHORT)V_Sature(VWAV,-32768.0f,32767.0f);
		fwrite(&Z,sizeof(SHORT),1,F);
		}
	}

	fclose(F);

	return -1;
}



#endif

