
#include "SPG_General.h"

#ifdef SPG_General_USECut

#include "SPG_Includes.h"

#include <memory.h>

int SPG_CONV SPG_MCV_Init(SPG_MCV& MCV, int NumChannel, int MaxMesure)
{
	memset(&MCV,0,sizeof(SPG_MCV));
	MCV.NumChannel=NumChannel;
	MCV.MaxMesure=MaxMesure;
	MCV.MultiCut=SPG_TypeAlloc(MCV.NumChannel,MCV_Cut,"SPG_MCV_Init");
	MCV.MultiCutSections=SPG_TypeAlloc(MCV.NumChannel,Cut,"SPG_MCV_Init");
	MCV.MultiCutSectionsPtr=SPG_TypeAlloc(MCV.NumChannel,Cut*,"SPG_MCV_Init");
	MCV.MultiCutColors=SPG_TypeAlloc(MCV.NumChannel,DWORD,"SPG_MCV_Init");
	for(int i=0;i<MCV.NumChannel;i++)
	{
		MCV.MultiCut[i].Draw=1;
		MCV.MultiCut[i].Color=0;
		Cut_Create(MCV.MultiCut[i].C,MCV.MaxMesure);
		for(int j=0;j<MCV_CURSORS;j++)
		{
			MCV.MultiCut[i].CursorPos[j]=-1;
		}
	}
	MCV.NrMesure=0;
	MCV.DisplayOrigin=0;
	MCV.DisplaySize=MaxMesure;
	return MCV.Etat=-1;
}

void SPG_CONV SPG_MCV_Close(SPG_MCV& MCV)
{
	CHECK(MCV.Etat==0,"SPG_MCV_Close",return);
#ifdef DebugMem
	CHECKPOINTER_L(MCV.MultiCut,MCV.NumChannel*sizeof(MCV_Cut),"SPG_MCV_Close",return);
#endif
	for(int i=0;i<MCV.NumChannel;i++)
	{
		Cut_Close(MCV.MultiCut[i].C);
	}
	SPG_MemFree(MCV.MultiCut);
	SPG_MemFree(MCV.MultiCutSections);
	SPG_MemFree(MCV.MultiCutSectionsPtr);
	SPG_MemFree(MCV.MultiCutColors);
	memset(&MCV,0,sizeof(SPG_MCV));
	return;
}

void SPG_CONV SPG_MCV_Add(SPG_MCV& MCV, int Channel, float Value)
{
	CHECK(MCV.Etat==0,"SPG_MCV_Add",return);
#ifdef DebugMem
	CHECKPOINTER_L(MCV.MultiCut,MCV.NumChannel*sizeof(MCV_Cut),"SPG_MCV_Close",return);
#endif
	CHECK(!V_IsBound(Channel,0,MCV.NumChannel),"SPG_MCV_Add",return);
	MCV.MultiCut[Channel].C.D[MCV.NrMesure]=Value;
	return;
}

void SPG_CONV SPG_MCV_Update(SPG_MCV& MCV)
{
	CHECK(MCV.Etat==0,"SPG_MCV_Update",return);
#ifdef DebugMem
	CHECKPOINTER_L(MCV.MultiCut,MCV.NumChannel*sizeof(MCV_Cut),"SPG_MCV_Close",return);
#endif
	bool Follow=(MCV.NrMesure==MCV.DisplayOrigin);
	MCV.NrMesure++;
	if(MCV.NrMesure==MCV.MaxMesure) MCV.NrMesure=0;
	if(Follow) MCV.DisplayOrigin=MCV.NrMesure;
	return;
}

void SPG_CONV SPG_MCV_Draw(SPG_MCV& MCV, G_Ecran& E, C_Lib& CL)
{
	CHECK(MCV.Etat==0,"SPG_MCV_Draw",return);
#ifdef DebugMem
	CHECKPOINTER_L(MCV.MultiCut,MCV.NumChannel*sizeof(MCV_Cut),"SPG_MCV_Draw",return);
	CHECKPOINTER_L(MCV.MultiCutSections,MCV.NumChannel*sizeof(Cut),"SPG_MCV_Draw",return);
	CHECKPOINTER_L(MCV.MultiCutSectionsPtr,MCV.NumChannel*sizeof(Cut*),"SPG_MCV_Draw",return);
#endif

	MCV.DisplayOrigin=V_Sature(MCV.DisplayOrigin,0,MCV.NrMesure);
	int Start=V_Sature(MCV.DisplayOrigin-MCV.DisplaySize,0,MCV.NrMesure);
	int Stop=MCV.DisplayOrigin;
	if(Stop>Start)
	{
		//Cut CBatt,CTemp,CClock,CVolt;
		int i;
		int j=0;
		for(i=0;i<MCV.NumChannel;i++)
		{
			if(MCV.MultiCut[i].Draw)
			{
				Cut_Extract(MCV.MultiCutSections[j],MCV.MultiCut[i].C,Start,Stop,Cut_Alias);
				MCV.MultiCutSectionsPtr[j]=MCV.MultiCutSections+j;
				MCV.MultiCutColors[j]=MCV.MultiCut[i].Color;
				j++;
			}
		}
		if(j)
		{
			Cut_DrawList(MCV.MultiCutSectionsPtr,E,MCV.MultiCutColors,CL,j,0);
			for(i=0;i<j;i++)
			{
				Cut_Close(MCV.MultiCutSections[i]);
			}
		}
	}
	return;
}

void SPG_CONV SPG_MCV_Zoom(SPG_MCV& MCV, int Delta)
{
	CHECK(MCV.Etat==0,"SPG_MCV_Zoom",return);
#ifdef DebugMem
	CHECKPOINTER_L(MCV.MultiCut,MCV.NumChannel*sizeof(MCV_Cut),"SPG_MCV_Zoom",return);
#endif
	if((Delta>0)&&(MCV.DisplaySize>MCV.NrMesure))
	{
		MCV.DisplaySize=MCV.NrMesure;
	}
	if(Delta>0) 
	{
		MCV.DisplaySize=(3*(MCV.DisplaySize-4)/4);
		if(MCV.DisplaySize<16) MCV.DisplaySize=16;
	}
	if(Delta<0) 
	{
		MCV.DisplaySize=(4*MCV.DisplaySize/3+4);
		if(MCV.DisplaySize>=MCV.NrMesure)
		{
			MCV.DisplaySize=MCV.MaxMesure;
		}
	}
	if(MCV.DisplaySize>MCV.DisplayOrigin)
	{
		MCV.DisplayOrigin=V_Min(MCV.DisplaySize,MCV.NrMesure);
	}
	return;
}

void SPG_CONV SPG_MCV_Scroll(SPG_MCV& MCV, int Delta)
{
	CHECK(MCV.Etat==0,"SPG_MCV_Scroll",return);
#ifdef DebugMem
	CHECKPOINTER_L(MCV.MultiCut,MCV.NumChannel*sizeof(MCV_Cut),"SPG_MCV_Scroll",return);
#endif
	if(Delta<0)
	{
		MCV.DisplayOrigin-=V_Max(1,MCV.DisplaySize/32);
	}
	else if(Delta>0)
	{
		MCV.DisplayOrigin+=V_Max(1,MCV.DisplaySize/32);
	}
	if(MCV.DisplayOrigin<MCV.DisplaySize) MCV.DisplayOrigin=MCV.DisplaySize;
	if(MCV.DisplayOrigin>MCV.NrMesure) MCV.DisplayOrigin=MCV.NrMesure;
	return;
}

void SPG_CONV SPG_MCV_SetCursor(SPG_MCV& MCV, int Channel, int CursorNr, float Position)
{
	CHECK(MCV.Etat==0,"SPG_MCV_Add",return);
	CHECK(!V_IsBound(Channel,0,MCV.NumChannel),"SPG_MCV_SetCursor",return);
	CHECK(!V_IsBound(CursorNr,0,MCV_CURSORS),"SPG_MCV_SetCursor",return);
	if(Position>=0)
	{
		MCV.MultiCut[Channel].CursorPos[CursorNr]=V_Sature(Position,0,MCV.MultiCut[Channel].C.NumS);
	}
	else
	{
		MCV.MultiCut[Channel].CursorPos[CursorNr]=-1;
	}
	return;
}

#endif

