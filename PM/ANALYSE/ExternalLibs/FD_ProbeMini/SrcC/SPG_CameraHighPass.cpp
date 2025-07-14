

#include "SPG_General.h"

#ifdef SPG_General_USECameraHighPass

#include "SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <float.h>

int SPG_CONV P_CamInit(CamHP& C, int SizeX, int SizeY, int Downsample, int bShowWindow)
{
	SPG_ZeroStruct(C);
	C.Downsample=Downsample;
	SizeX/=C.Downsample;
	SizeY/=C.Downsample;
	P_Create(C.PH, SizeX, SizeY);
	P_Create(C.PL, SizeX, SizeY);
	P_Create(C.PM, SizeX, SizeY);

	P_Create(C.Psrc, SizeX, SizeY);
	//P_Create(C.Pdest, SizeX, SizeY);

	//int Depth=GetDeviceCaps((HDC)Global.HDCWND,BITSPIXEL);

	G_InitEcranWindows(C.E,4,SizeX,SizeY,0,1);

	S_InitTimer(C.TTotal,"");
	S_InitTimer(C.TM,"");
	S_InitTimer(C.THL,"");
	S_InitTimer(C.TMHL,"");
	S_InitTimer(C.TN,"");
	S_InitTimer(C.TDraw,"");

#ifdef SPG_General_USEWindow
	C.bShowWindow=bShowWindow;
	if(C.bShowWindow)
	{
		SPG_CreateWindow(C.SW,SPGWT_UserFriendly|SPGWT_Moveable,G_STANDARDW,0,-1,SizeX,SizeY,0,"CamHP","CamHP",0,&C,Global.hInstance);
	}
#endif

	return -1;
}

int SPG_CONV P_CamClose(CamHP& C)
{
#ifdef SPG_General_USEWindow
	if(C.bShowWindow)
	{
		SPG_CloseWindow(C.SW);
	}
#endif

	S_PrintRatio(&C.TTotal,6);

	S_CloseTimer(C.TTotal);
	S_CloseTimer(C.TM);
	S_CloseTimer(C.THL);
	S_CloseTimer(C.TMHL);
	S_CloseTimer(C.TN);
	S_CloseTimer(C.TDraw);

	P_Close(C.PH);
	P_Close(C.PL);
	P_Close(C.PM);
	P_Close(C.Psrc);
	G_CloseEcran(C.E);
	SPG_ZeroStruct(C);
	return -1;
}

#define M 1.4

#define NOISE 7
#define NOISE4 NOISE*NOISE*NOISE*NOISE

int SPG_CONV P_CamComputeContrast(CamHP& C, Profil& P)
{
	CHECK((P_SizeX(C.PM)!=P_SizeX(P))||(P_SizeY(C.PM)!=P_SizeY(P)),"P_CamUpdateContrsat",return 0);
	S_StartTimer(C.TTotal);

	S_StartTimer(C.TM);
	P_Copy(C.PM,P);
	P_FastConvLowPass(C.PM,P_SizeX(P)/8);
	P_FastConvLowPass(C.PM,P_SizeX(P)/12);
	S_StopTimer(C.TM);

	S_StartTimer(C.THL);
	P_ForAll(P,i,
		float x=P_Data(P)[i]-P_Data(C.PM)[i];
		if(x>=0) {x*=x;x*=x;P_Data(C.PH)[i]=x+NOISE4;P_Data(C.PL)[i]=NOISE4;}
		else {x*=x;x*=x;P_Data(C.PL)[i]=x+NOISE4;P_Data(C.PH)[i]=NOISE4;}
				);
	S_StopTimer(C.THL);

	S_StartTimer(C.TMHL);
	P_FastConvLowPass(C.PH,P_SizeX(P)/4);
	//P_FastConvLowPass(C.PH,P_SizeX(P)/3);

	P_FastConvLowPass(C.PL,P_SizeX(P)/4);
	//P_FastConvLowPass(C.PL,P_SizeX(P)/3);
	S_StopTimer(C.TMHL);
	
	S_StopTimer(C.TTotal);

	return -1;
}

int SPG_CONV P_CamUpdateContrast(CamHP& C, Profil& P)
{
	CHECK((P_SizeX(C.PM)!=P_SizeX(P))||(P_SizeY(C.PM)!=P_SizeY(P)),"P_CamUpdateContrsat",return 0);
	S_StartTimer(C.TTotal);

	float mdt=0.5;
	float cdt=0.5;

	C.Frame++;
	for(int py=0;py<P_SizeY(P);py++)
	{
		int i0=py*P_SizeX(P);
	for(int px=(C.Frame+py)&1;px<P_SizeX(P);px+=2)
	{
		int i=i0+px;
		float x=P_Data(P)[i]-P_Data(C.PM)[i];
		P_Data(C.PM)[i]+=mdt*x;
		if(x>=0) 
		{x*=x;x*=x;P_Data(C.PH)[i]+=cdt*(x+(NOISE4-P_Data(C.PH)[i]));P_Data(C.PL)[i]+=cdt*(NOISE4-P_Data(C.PL)[i]);}
		else 
		{x*=x;x*=x;P_Data(C.PL)[i]+=cdt*(x+NOISE4-P_Data(C.PL)[i]);P_Data(C.PH)[i]+=cdt*(NOISE4-P_Data(C.PH)[i]);}
	}
	}

	P_FastConvLowPass(C.PH,P_SizeX(P)/10);
	P_FastConvLowPass(C.PL,P_SizeX(P)/10);
	P_FastConvLowPass(C.PM,P_SizeX(P)/10);

	S_StopTimer(C.TTotal);

	return -1;
}

int SPG_CONV P_CamProcess(CamHP& C, Profil& P)
{

	CHECK((P_SizeX(C.PM)!=P_SizeX(P))||(P_SizeY(C.PM)!=P_SizeY(P)),"P_CamProcess",return 0);
	S_StartTimer(C.TTotal);

#ifdef SPG_General_USEWindow
	if(C.bShowWindow>1)
	{
		P_DrawInternal(P,C.SW.Ecran,0,0,0,255);
	}
#endif

	S_StartTimer(C.TN);
	P_ForAll(P,i,
		float& x=P_Data(P)[i];
		x-=P_Data(C.PM)[i];
		if(x>=0) 
		{
			//V_CheckFloat(P_Data(C.PH)[i],"P_CamProcess");
			//DbgCHECK((NOISE+P_Data(C.PH)[i])<0,"P_CamProcess");
			x=M-M/(1+x/(sqrtf(sqrtf(P_Data(C.PH)[i]))));
			//V_CheckFloat(x,"P_CamProcess");
			//if(x>(1-FLT_EPSILON)) x=(1-FLT_EPSILON);
		}
		else
		{
			//V_CheckFloat(P_Data(C.PL)[i],"P_CamProcess");
			//DbgCHECK((NOISE+P_Data(C.PL)[i])<0,"P_CamProcess");
			x=-M+M/(1-x/(sqrtf(sqrtf(P_Data(C.PL)[i]))));
			//V_CheckFloat(x,"P_CamProcess");
			//if(x<(FLT_EPSILON-1)) x=(FLT_EPSILON-1);
		}
		//P_Data(P)[i]=x;
	);
	S_StopTimer(C.TN);

#ifdef SPG_General_USEWindow
	if(C.bShowWindow)
	{
		int VS=G_SizeY(C.SW.Ecran)/2;
		if(C.bShowWindow>1) V_Swap(int,VS,G_SizeY(C.SW.Ecran));
		P_DrawInternal(P,C.SW.Ecran,0,0,-1,1);
		if(C.bShowWindow>1) V_Swap(int,VS,G_SizeY(C.SW.Ecran));
		G_BlitEcran(C.SW.Ecran);
	}
#endif
	S_StopTimer(C.TTotal);
	return -1;
}


int SPG_CONV P_CamSetDIBitsToDevice(CamHP& C, unsigned char* B, int Pitch, void* hDC)
{
	float* D=P_Data(C.Psrc);
	if(C.Downsample==1)
	{
		for(int y=0;y<P_SizeY(C.Psrc);y++)
		{
			for(int x=0;x<P_SizeX(C.Psrc);x++)
			{
				D[x]=(float)B[x];
			}
			D+=P_SizeX(C.Psrc);
			B+=Pitch;
		}
	}
	else if(C.Downsample==2)
	{
		for(int y=0;y<P_SizeY(C.Psrc);y++)
		{
			for(int x=0;x<P_SizeX(C.Psrc);x++)
			{
				D[x]=(float)B[x+x]+(float)B[x+x+1]+(float)B[x+x+Pitch]+(float)B[x+x+Pitch+1];
			}
			D+=P_SizeX(C.Psrc);
			B+=Pitch+Pitch;
		}
	}

	P_CamProcess(C,C.Psrc);

	S_StartTimer(C.TDraw);//5%
	P_DrawInternal(C.Psrc,C.E,0,0,-1,1);

	C.E.HECR=hDC;
	G_BlitEcran(C.E);
	S_StopTimer(C.TDraw);

	return -1;
}

#endif
