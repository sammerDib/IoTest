
#include "..\SrcC\SPG.h"

#include "SPG_LearningSet.h"

#include <memory.h>

#define ALLOCPOOL 256

int SPG_CONV LearningSet_Init(LEARNINGSTATE& L, int szTag, int szInputSet, int szOutputSet, int MaxSet)
{
	SPG_ZeroStruct(L);
	L.szTag=szTag;
	L.szInputSet=szInputSet;
	L.szOutputSet=szOutputSet;
	L.Size=szTag+(szInputSet+szOutputSet)*sizeof(float);
	L.MaxSet=MaxSet;
	L.S=SPG_TypeAlloc(L.MaxSet,LEARNINGSET,"LearningSet");
	L.M=SPG_TypeAlloc(L.szInputSet*L.szOutputSet,float,"LearningSet:M");

	int OX=V_Max(V_Round(sqrtf(L.szOutputSet)),1);
	int OY=L.szOutputSet/OX;
	int IX=V_Max(V_Round(sqrtf(L.szInputSet)),1);
	int IY=L.szInputSet/IX;
	for(int o=0;o<L.szOutputSet;o++)
	{
		int ox=o%OX;
		int oy=o/OX;
		int ix=ox-(OX-IX)/2;
		int iy=oy-(OY-IY)/2;
		ix=V_Sature(ix,0,IX-1);
		iy=V_Sature(iy,0,IY-1);
		L.M[ o * L.szInputSet + iy*IX + ix]=1;
	}
	return -1;
}

void SPG_CONV LearningSet_Close(LEARNINGSTATE& L)
{
	for(int i=0;i<L.NumSet;i+=ALLOCPOOL)
	{
		SPG_MemFree(L.S[i].Tag);
	}
	SPG_MemFree(L.S);
	SPG_MemFree(L.M);
	SPG_ZeroStruct(L);
}

int SPG_CONV LearningSet_Add(LEARNINGSTATE& L, BYTE* Tag, float* InputSet, float* OutputSet)
{
	CHECK(L.NumSet==L.MaxSet,"LearningSet_Add",return 0);
	LEARNINGSET& S=L.S[L.NumSet];
	if((L.NumSet&(ALLOCPOOL-1))==0)
	{
		S.Tag = SPG_MemAlloc(L.Size*ALLOCPOOL,"LearningSet_Add");
	}
	else
	{
		S.Tag = L.S[L.NumSet&~(ALLOCPOOL-1)].Tag + L.Size*(L.NumSet&(ALLOCPOOL-1));
	}

	S.Input = (float*) ( S.Tag + L.szTag );
	S.Output = (float*) ( S.Tag + L.szTag + L.szInputSet*sizeof(float) );

	memcpy(S.Tag,Tag,L.szTag);
	memcpy(S.Input,InputSet,L.szInputSet*sizeof(float));
	memcpy(S.Output,OutputSet,L.szOutputSet*sizeof(float));
	return ++L.NumSet;
}

int SPG_CONV LearningSet_Save(LEARNINGSTATE& L, char* FName)
{
	return Text_Write(L.M,L.szInputSet,L.szOutputSet,FName,CF_DIGITFLOAT,S_Tabulation);
}

int SPG_CONV LearningSet_Load(LEARNINGSTATE& L, char* FName)
{
	int X=0; int Y=0;
	Text_Read(FName,X,Y,1);
	CHECKTWO((X==0)&&(Y==0),"LearningSet_Load : Please calibrate",FName,return 0);
	CHECKTWO((X!=L.szInputSet)||(Y!=L.szOutputSet),"LearingSet_Load : Size mismatch - Please calibrate",FName,return 0);
	SPG_MemFree(L.M);
	L.M=Text_Read(FName,X,Y,0);
	return L.M!=0;
}

int SPG_CONV LearningSet_Apply(LEARNINGSTATE& L, float* InputSet, float* OutputSet)
{
	float* M=L.M;
	for(int o=0;o<L.szOutputSet;o++)
	{
		float& OS=OutputSet[o]; OS=0;
		for(int i=0;i<L.szInputSet;i++)
		{
			OS+=M[i]*InputSet[i];
		}
		M+=L.szInputSet;
	}
	return -1;
}
