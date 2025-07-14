
#include "SPG_General.h"

#ifdef SPG_General_USEDEMODULE

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <memory.h>


//demodulation par fft à fenetre

void SPG_CONV DemFillWithCos(Cut& C)
{
	CHECK(C.Etat==0,"DemFillWithCos",return);
	double w=V_PI/(C.NumS-1);
	for(int i=0;i<C.NumS;i++)
	{
		C.D[i]=0.5-0.5*cos(w*i);
	}
	return;
}

int SPG_CONV DemInit(SPG_DEMODULEFFT& D, CutX* CAP, int MaxNumS, int SegNumS, int weightTNumS, int weightFNumS, int f0, int df, int Flag, char* WName)
{
	CHECK(MaxNumS==0,"",return 0);
	CHECK(CAP==0,"DemInit",return 0);
	CHECK(SegNumS==0,"",return 0);
	CHECK(weightTNumS==0,"",return 0);
	CHECK(weightFNumS==0,"",return 0);
	CHECK(f0==0,"",return 0);

	D.Flag=Flag;
	Cut_Create(D.Source,MaxNumS+weightTNumS,1,"x","y"); //données sources concaténées

	D.CAP=CAP;
	CutX_Create(*D.CAP,MaxNumS+SegNumS+weightTNumS,1,"f","MP"); //données finales sous forme module/phase
	
	// CutX_Create(D.ALY,MaxNumS+SegNumS+weightTNumS,1,"f","ReIm"); //données finales sous forme Re/Im
	CutX_Init(D.ALY,MaxNumS+SegNumS+weightTNumS,Cut_Data( (*D.CAP) ),0,0,Cut_Alias); //données sous forme Re/Im

	D.fftSegNumS=SFFT_GetAppropriateSize(SegNumS,FFT_UPPER);//determine la longueur d'une FFT la plus proche de la longueur SegNumS
	
	D.NumSegS=D.fftSegNumS-2*weightTNumS; //longueur d'un segment moins chevauchement gauche moins chevauchement droite

	D.f0=f0;
	D.df=df;

	Cut_Create(D.weightT,weightTNumS); //fonction de ponderation dans l'espace direct (zone de chevauchement)
	DemFillWithCos(D.weightT);
	SPG_MemFastCheck();

	Cut_Create(D.weightF,weightFNumS); //fonction de ponderation dans l'espace de fourier (bords du filtre passe bande)
	DemFillWithCos(D.weightF);
	SPG_MemFastCheck();

	CutX_Create(D.Seg,D.fftSegNumS); //segment courant, sur lequel on peut appliquer les FFTs
	//CutX_Create(D.CG,SegmentLen);

	if(D.Flag&DEMENVMAX)
	{//calcule l'envelope du module des segments
		Cut_Create(D.EnvMax,D.Seg.NumS/2,1,"f","M");
	}
	if(D.Flag&DEMLIVEDISP)
	{
		Cut_Create(D.EnvW,D.Seg.NumS/2);
		DemGetweightF(D,D.EnvW);
		SPG_MemFastCheck();
		Cut_Create(D.SrcW,D.Seg.NumS);
		DemGetweightT(D,D.SrcW);
		SPG_MemFastCheck();

		Cut_Init(D.Src,D.Seg.NumS+D.Seg.NumS/2,D.Source.D,0,0,Cut_Alias);
		Cut_Create(D.Res,D.Seg.NumS);
		Cut_Create(D.Env,D.Seg.NumS/2);
		Cut_Create(D.Phase,D.Seg.NumS+D.Seg.NumS/2);
		//Cut_Init(D.PSrc,D.Phase.NumS,D.Source.D,0,0,Cut_Alias);

		int K=2;
		int L=V_Min(K*D.Seg.NumS,512);
		int LH=V_Min(K*D.Seg.NumS/2,512);
		SPG_CreateWindow(D.W,SPGWT_UserFriendly|SPGWT_Moveable,G_STANDARDW,1,1,L+LH,512,0,WName?WName:"Demodulation","SPGDEM",0,&D,Global.hInstance);
		C_LoadCaracLib(D.CL,D.W.Ecran);
		G_InitSousEcran(D.E_Src,D.W.Ecran,	0,	0,						L,G_SizeY(D.W.Ecran)/2);
		G_InitSousEcran(D.E_Env,D.W.Ecran,	L,	0,						LH,G_SizeY(D.W.Ecran)/2);
		G_InitSousEcran(D.E_Phase,D.W.Ecran,0,	G_SizeY(D.W.Ecran)/2,	L+LH,G_SizeY(D.W.Ecran)/2);

	}

	return -1;
}

int SPG_CONV DemClose(SPG_DEMODULEFFT& D)
{
	if(D.Flag&DEMLIVEDISP)
	{
		SPG_CloseWindow(D.W);
		G_CloseEcran(D.E_Env);
		G_CloseEcran(D.E_Src);
		G_CloseEcran(D.E_Phase);
		C_CloseCaracLib(D.CL);
		Cut_Close(D.Env);
		Cut_Close(D.EnvW);
		Cut_Close(D.Src);
		Cut_Close(D.Res);
		Cut_Close(D.SrcW);
		Cut_Close(D.Phase);
	}
	if(D.Flag&DEMENVMAX)
	{
		Cut_Close(D.EnvMax);
	}

	Cut_Close(D.Source);
	if(D.CAP) CutX_Close(*D.CAP);
	CutX_Close(D.ALY);
	Cut_Close(D.weightT);
	Cut_Close(D.weightF);
	CutX_Close(D.Seg);
	//CutX_Close(D.CG);
	
	SPG_ZeroStruct(D);

	return -1;
}

//pour affichage uniquement : forme de la fenetre de ponderation en temps

void SPG_CONV DemGetweightT(SPG_DEMODULEFFT& D, Cut& C)
{
	int i;
	for(i=0;i<D.weightT.NumS;i++)
	{
		C.D[i]=D.weightT.D[i];
	}
	for(;i<D.Seg.NumS-D.weightT.NumS;i++)
	{
		C.D[i]=1;
	}
	for(;i<D.Seg.NumS;i++)
	{
		C.D[i]=D.weightT.D[D.Seg.NumS-1-i];
	}
	return;
}

//pour affichage uniquement : forme de la fenetre de ponderation en frequence

void SPG_CONV DemGetweightF(SPG_DEMODULEFFT& D, Cut& C)
{
	int fzmin=V_Sature(	D.f0-D.df-D.weightF.NumS,	0,D.Seg.NumS/2);
	int fpmin=V_Sature(	D.f0-D.df,					0,D.Seg.NumS/2);
	int fpmax=V_Sature(	D.f0+D.df,					0,D.Seg.NumS/2);
	int fzmax=V_Sature(	D.f0+D.df+D.weightF.NumS,	0,D.Seg.NumS/2);

	{
		int i;
		for(i=0;i<fzmin;i++)
		{
			C.D[i]=0;
		}
		for(;i<fpmin;i++)
		{
			C.D[i]=D.weightF.D[i-fzmin];
		}
		for(;i<fpmax;i++)
		{
			C.D[i]=1;
		}
		for(;i<fzmax;i++)
		{
			C.D[i]=D.weightF.D[fzmax-1-i];
		}
		for(;i<D.Seg.NumS/2;i++)
		{
			C.D[i]=0;
		}
	}
	return;
}

/*
*/

/*
void SPG_CONV DemClear(SPG_DEMODULEFFT& D)
{
	D.CurNumS=0;
	D.CurProcessed=0;
}
*/


//pour affichage uniquement
/*
void SPG_CONV DemGetRes(SPG_DEMODULEFFT& D, Cut& C)
{
	for(int i=0;i<C.NumS; i++) //D.Seg.NumS
	{
		C.D[i]=2*D.ALY.D[D.CurProcessed+i].re/D.Seg.NumS;
	}
	return;
}


//pour affichage uniquement

void SPG_CONV DemGetPhase(SPG_DEMODULEFFT& D, Cut& C)
{
	C.D[0]=D.CAP->D[D.CurProcessed].im-D.CAP->D[V_Max(D.CurProcessed-1,0)].im;
	V_WrapPi(C.D[0]);
	for(int i=1;i<C.NumS;i++)
	{
		C.D[i]=D.CAP->D[D.CurProcessed+i].im-D.CAP->D[D.CurProcessed+i-1].im;
		V_WrapPi(C.D[i]);
	}
	return;
}
*/
//pour affichage uniquement : enveloppe des modules des ffts des segments,  pour reperer la frequence centrale

void SPG_CONV DemUpdateEnvMax(SPG_DEMODULEFFT& D)
{
	for(int i=1;i<D.Seg.NumS/2;i++)
	{
		float M=CX_Module(D.Seg.D[i]);
		if(M>D.EnvMax.D[i]) D.EnvMax.D[i]=M;
	}
	return;
}


//pour affichage uniquement : modules de la fft du segment courant

void SPG_CONV DemUpdateEnv(SPG_DEMODULEFFT& D)
{
	for(int i=1;i<D.Seg.NumS/2;i++)
	{
		D.Env.D[i]=CX_Module(D.Seg.D[i]);
	}
	return;
}

void SPG_CONV DemOpenSignal(SPG_DEMODULEFFT& D)
{
	D.CurNumS=0;
	D.CurProcessed=0;
	Cut_Clear(D.Seg);
	CutX_Clear((*D.CAP));
	CutX_Clear(D.ALY);
	return;
}

void SPG_CONV DemCopySignalToSeg(SPG_DEMODULEFFT& D)
{// copie le segment D.Source[D.CurProcessed...+D.Seg]
	//SPG_MemFastCheck();
	int i;
	for(i=0;i<D.weightT.NumS;i++)
	{
		D.Seg.D[i].re=D.Source.D[D.CurProcessed+i]*D.weightT.D[i];
		D.Seg.D[i].im=0;
	}
	for(;i<D.Seg.NumS-D.weightT.NumS;i++)
	{
		D.Seg.D[i].re=D.Source.D[D.CurProcessed+i];
		D.Seg.D[i].im=0;
	}
	for(;i<D.Seg.NumS;i++)
	{
		D.Seg.D[i].re=D.Source.D[D.CurProcessed+i]*D.weightT.D[D.Seg.NumS-1-i];
		D.Seg.D[i].im=0;
	}
	//SPG_MemFastCheck();
	return;
}

void SPG_CONV DemCopyDemoduleSeg(SPG_DEMODULEFFT& D)
{ //FFT + filtre passe bande + FFT
	//SPG_MemFastCheck();
	CutX_FFT(D.Seg);
	if(D.Flag&DEMENVMAX) DemUpdateEnvMax(D);
	if(D.Flag&DEMLIVEDISP) DemUpdateEnv(D);

	float M=1.0/D.fftSegNumS;

	int fzmin=V_Sature(	D.f0-D.df-D.weightF.NumS,	0,D.Seg.NumS/2);
	int fpmin=V_Sature(	D.f0-D.df,					0,D.Seg.NumS/2);
	int fpmax=V_Sature(	D.f0+D.df,					0,D.Seg.NumS/2);
	int fzmax=V_Sature(	D.f0+D.df+D.weightF.NumS,	0,D.Seg.NumS/2);

	{//Filtre passe bande
		int i;
		for(i=0;i<V_Max(fzmin,1);i++)
		{
			D.Seg.D[i].re=0;
			D.Seg.D[i].im=0;
		}
		for(;i<fpmin;i++)
		{
			D.Seg.D[i].re*=M*D.weightF.D[i-fzmin];
			D.Seg.D[i].im*=-M*D.weightF.D[i-fzmin];
		}
		for(;i<fpmax;i++)
		{
			D.Seg.D[i].re*=M;
			D.Seg.D[i].im*=-M;
		}
		for(;i<fzmax;i++)
		{
			D.Seg.D[i].re*=M*D.weightF.D[fzmax-1-i];
			D.Seg.D[i].im*=-M*D.weightF.D[fzmax-1-i];
		}
		for(;i<D.Seg.NumS;i++)
		{
			D.Seg.D[i].re=0;
			D.Seg.D[i].im=0;
		}
	}
	CutX_FFT(D.Seg);
	//SPG_MemFastCheck();
	return;
}

void SPG_CONV DemCopySegtoA(SPG_DEMODULEFFT& D)
{ //copie le segment dans les tableaux de resultats
	//SPG_MemFastCheck();
	int i;
	for(i=0;i<D.Seg.NumS;i++) //D.weightT.NumS
	{
		SPG_COMPLEX& Z=D.ALY.D[D.CurProcessed+i];
		Z.re+=D.Seg.D[i].re;
		Z.im+=D.Seg.D[i].im;
	}
	//SPG_MemFastCheck();
	return;
}

void SPG_CONV DemLiveDisp(SPG_DEMODULEFFT& D)
{
//	SPG_MemFastCheck();
	if(D.Flag&DEMLIVEDISP)
	{
		D.Src.D=D.Source.D+V_Min(D.CurProcessed,D.Source.NumS-D.Src.NumS);
		DemGetweightT(D, D.SrcW);
		DemGetweightF(D, D.EnvW);
		//DemGetRes(D,D.Res);
		//DemGetPhase(D,D.Phase);

		//Cut_Draw2(D.Src,D.Res,D.E_Src, 0,0xFF0000,D.CL);
		Cut_Draw2(D.EnvMax,D.Env,D.E_Env,0x808080,0,D.CL);
		Cut_Draw_General(D.SrcW,D.E_Src,0x00FF00,D.CL,0,-1,2,0,0);


		Cut_Draw2(D.EnvMax,D.Env,D.E_Env,0x808080,0,D.CL);
		Cut_Draw_General(D.EnvW,D.E_Env,0x00FF00,D.CL,0,0,1,0,0);

		int NumS=D.Phase.NumS;
		V_SWAP(int,NumS,D.Src.NumS);
		D.Src.D=D.Source.D+V_Max(D.CurProcessed+D.Seg.NumS-D.weightT.NumS-D.Src.NumS,0);
		//Cut_Draw2MultiScale(D.Src,D.Phase,D.E_Phase,0,0x00FF00,D.CL);
		V_SWAP(int,NumS,D.Src.NumS);

		G_BlitEcran(D.W.Ecran);
		DoEvents(SPG_DOEV_ALL);
		//SPG_Sleep(2000);
		SPG_MemFastCheck();
	}
	return;
}


void SPG_CONV DemUpdateSignal(SPG_DEMODULEFFT& D, short int* Signal, int Pitch, int NumS)
{
	//SPG_MemFastCheck();
	float* SD=D.Source.D+D.CurNumS;
	int h=0;
	for(int i=0;i<NumS;i++)
	{
		SD[i]=Signal[h];
		h+=Pitch;
	}
	D.CurNumS+=NumS;

	while(D.CurNumS-D.CurProcessed>D.Seg.NumS)
	{
		DemCopySignalToSeg(D);
		DemCopyDemoduleSeg(D);
		DemCopySegtoA(D);
		DemLiveDisp(D);
		D.CurProcessed+=D.Seg.NumS-D.weightT.NumS;
	}
	//SPG_MemFastCheck();
	return;
}

void SPG_CONV DemCloseSignal(SPG_DEMODULEFFT& D)
{
	SPG_MemFastCheck();
	CHECK(D.Source.Etat==0,"DemCloseSignal",return);
	float* SD=D.Source.D+D.CurNumS;
	for(int i=0;i<D.weightT.NumS;i++)
	{
		SD[i]=0;
	}
	D.CurNumS+=D.weightT.NumS;
	if(D.CurNumS-D.CurProcessed>D.Seg.NumS)
	{
		DemCopySignalToSeg(D);
		DemCopyDemoduleSeg(D);
		DemCopySegtoA(D);
		DemLiveDisp(D);
		D.CurProcessed+=D.Seg.NumS-D.weightT.NumS;
	}
	for(int i=0;i<D.CurProcessed;i++)
	{
		SPG_COMPLEX Z=D.ALY.D[i];
		D.CAP->D[i].re=CX_Module(Z);
		D.CAP->D[i].im=CX_Arg(Z);
	}
	//SPG_MemFastCheck();
	return;
}

//demodulation larkin 90° tolerante au pas (ape/bruno/rainer - repris de APEIM\Source\Demodulation.cpp)

int SPG_CONV DemInit(SPG_DEMODULEDECPHASE& D, CutX* CAP, int MaxNumS, int DecalageDePhaseID)
{
	CHECK(MaxNumS==0,"",return 0);
	CHECK(CAP==0,"DemInit",return 0);

	SPG_DecalageDePhaseInit(D.DEC,(DECALAGEDEPHASEID)DecalageDePhaseID);

	Cut_Create(D.Source,MaxNumS+D.DEC.NumSteps);
	D.CAP=CAP;
	CutX_Create(*D.CAP,MaxNumS+D.DEC.NumSteps);
	return -1;
}

int SPG_CONV DemClose(SPG_DEMODULEDECPHASE& D)
{
	SPG_DecalageDePhaseClose(D.DEC);

	Cut_Close(D.Source);
	if(D.CAP) CutX_Close(*D.CAP);
	SPG_ZeroStruct(D);
	return -1;
}

void SPG_CONV DemOpenSignal(SPG_DEMODULEDECPHASE& D)
{
	D.CurNumS=0;
	D.CurProcessed=0;
	CutX_Clear((*D.CAP));
	return;
}

void SPG_CONV DemUpdateSignal(SPG_DEMODULEDECPHASE& D, short int* Signal, int Pitch, int NumS)
{
	SPG_MemFastCheck();
	float* SD=D.Source.D+D.CurNumS;
	int h=0;
	for(int i=0;i<NumS;i++)
	{
		SD[i]=Signal[h];
		h+=Pitch;
	}
	D.CurNumS+=NumS;

	SPG_MemFastCheck();
	int N=D.CurNumS-D.CurProcessed-D.DEC.NumSteps;
	if(N>0)
	{
		SPG_DecalageDePhase_Process(D.DEC,N,&D.CAP->D[D.CurProcessed].im, &D.CAP->D[D.CurProcessed].re,2,D.Source.D+D.CurProcessed,1,1);
		D.CurProcessed+=N;
	}
	SPG_MemFastCheck();
	return;
}

void SPG_CONV DemCloseSignal(SPG_DEMODULEDECPHASE& D)
{
	SPG_MemFastCheck();
	float* SD=D.Source.D+D.CurNumS;
	for(int i=0;i<D.DEC.NumSteps;i++)
	{
		SD[i]=0;
	}
	D.CurNumS+=D.DEC.NumSteps;

	int N=D.CurNumS-D.CurProcessed-D.DEC.NumSteps;
	if(N>0)
	{
		SPG_DecalageDePhase_Process(D.DEC,N,&D.CAP->D[D.CurProcessed].im, &D.CAP->D[D.CurProcessed].re,2,D.Source.D+D.CurProcessed,1,1);
		D.CurProcessed+=N;
	}
	SPG_MemFastCheck();
	return;
}

#endif

