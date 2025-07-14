
#include "SPG_General.h"

#include "SPG_Includes.h"

#include "SPG_SysInc.h" //pour le getasynckeystate

#include <memory.h>

int SPG_CONV SPG_FastConvInit(RSFASTCONVOLVE& F, int FilterLen)//FilterLen impair
{
	SPG_ZeroStruct(F);
	F.FilterLen=FilterLen;
	if(FilterLen) F.invFilterLen=1.0/FilterLen;
	//F.Delay=Delay;
	F.RingMsk=F.FilterLen;
	for(int i=0;i<5;i++) F.RingMsk|=F.RingMsk>>(1<<i); F.RingLen=F.RingMsk+1;
	F.Ring=SPG_TypeAlloc(F.RingLen,RS_SAMPLE,"SPG_FastConvInit");
	F.Sum=0;
	return -1;
}

void SPG_CONV SPG_FastConvReset(RSFASTCONVOLVE& F)
{
	memset(F.Ring,0,F.RingLen*sizeof(RS_SAMPLE));
	F.Sum=0;
}

int SPG_CONV SPG_FastConvCheck(RSFASTCONVOLVE& F)
{
	//si le filtre est de type entier il faut vérifier la saturation
	/*
	__int64 Sum=0;
	for(int x=F.x-F.FilterLen;x<F.x;x++)
	{
		Sum+=(__int64)F.Ring[x&F.RingMsk];
	}
	CHECK(F.Sum!=Sum,"SPG_FastConvCheck : Filter input out of range - Reduce signal offset, Reduce acquisition frequency or Reduce Filter Length (decrease filter cut-off frequency)",
		F.Sum=V_Sature(Sum,0xFFFFFFFF80000001i64,0x7FFFFFFFi64); return 1);
	*/

	//dans le cas contraire on se contente de recaler les erreurs d'arrondi sans jamais renvoyer d'erreur
	float Sum=0;
	for(int x=F.x-F.FilterLen;x<F.x;x++)
	{
		Sum+=F.Ring[x&F.RingMsk];
	}
	F.Sum=Sum;
	return 0;
}

void SPG_CONV SPG_FastConvClose(RSFASTCONVOLVE& F)
{
	SPG_MemFree(F.Ring);
	SPG_ZeroStruct(F);
	return;
}

int SPG_CONV SPG_FastConvInit(RBFASTCONVOLVE& F, int FilterLen)//FilterLen impair
{
	SPG_ZeroStruct(F);
	F.FilterLen=FilterLen;
	F.invFilterLen=1.0/FilterLen;
	//F.Delay=Delay;
	F.RingMsk=F.FilterLen;
	for(int i=0;i<5;i++) F.RingMsk|=F.RingMsk>>(1<<i); F.RingLen=F.RingMsk+1;
	F.Ring=SPG_TypeAlloc(F.RingLen,RB_SAMPLE,"SPG_FastConvInit");
	//F.Sum=0;
	return -1;
}

void SPG_CONV SPG_FastConvReset(RBFASTCONVOLVE& F)
{
	memset(F.Ring,0,F.RingLen*sizeof(RB_SAMPLE));
	//F.Sum=0;
}

void SPG_CONV SPG_FastConvClose(RBFASTCONVOLVE& F)
{
	SPG_MemFree(F.Ring);
	SPG_ZeroStruct(F);
	return;
}

#ifdef _DEBUG

#define THELOOP for(int i=0;i<=5;i++)
void SPG_CONV SPG_FastConvTst()
{
	RSFASTCONVOLVE CF[16];	THELOOP	{	SPG_FastConvInit(CF[i],i);	}
	FILE* F=fopen("SPG_FastConvTst.txt","wb+");
	for(int x=0;x<62;x++)
	{
		THELOOP	
		{	RS_SAMPLE f,g;
			//if(x>=30)
			//{
			//	g=f=1<<(30-(x/2));
			//}
			//else
			//{
			//	g=f=1<<(x/2);
			//}

			if(false) {}
			else if(x==2) g=f=32767; 
			else if(x==3) g=f=32767; 
			else if(x==12) g=f=32767; 
			else if(x==13) g=f=32767; 
			else if(x==14) g=f=32767; 
			else if(x==22) g=f=32767; 
			else if(x==32) g=f=-32767; 
			else if(x==33) g=f=-32767; 
			else if(x==34) g=f=-32767; 
			else if(x==35) g=f=-32767; 
			else if(x==42) g=f=32767; 
			else if(x==44) g=f=32767; 
			else if(x==52) g=f=-32767; 
			else if(x==54) g=f=-32767; 
			else g=f=0;

			//SPG_FastConvFilt(CF[i],f);
			SPG_FastConvz(CF[i],f);
			//fprintf(F,"x=%i\tf=%i\tSum=%i\t",x,g,f,CF[i].Sum);
			fprintf(F,"%i\t%i\t%.1f\t\t",g,f,CF[i].Sum);
		}
		fprintf(F,"\r\n");
	}
	fclose(F);
	THELOOP	{	SPG_FastConvClose(CF[i]);	}
	return;
}

#endif

FASTCONVOLVE_CHANNEL_FILT* SPG_CONV CF_Init(int NumChannels, double FrequencyHz, float* fMin, float* fMax)
{
	FASTCONVOLVE_CHANNEL_FILT* CF=SPG_TypeAlloc(NumChannels,FASTCONVOLVE_CHANNEL_FILT,"CF_Init");
	for(int c=0;c<NumChannels;c++)
	{
		CHECK((fMin[c]>0) && (fMin[c] < FrequencyHz/262144), "CF_Init: High pass capability exceeded - Lower the sampling frequency when using small value of LowRPM", fMin[c]=FrequencyHz/262144);//limitation du filtre sur dépassement/normalisation quand le signal a un offset; baisser la frequence d'échantillonnage si cela se produit
		CHECK(fMin[c] > FrequencyHz/8, "CF_Init: Sampling frequency too small / LowRPM too high", fMin[c]=FrequencyHz/8);//le passe haut doit laisser passer les aubes
		CHECK((fMin[c]>0) && (fMax[c] < fMin[c]), "CF_Init: Min/Max rotating speed overlap", fMax[c]=fMin[c]);//le passe bas doit laisser passer ce que sort le passe haut

		int HighPassSamples0=0; int HighPassSamples1=0; int LowPassSamples0=0; int LowPassSamples1=0;

		if(fMin[c]>0)
		{
			HighPassSamples0=V_Ceil(FrequencyHz/fMin[c]);//le delai pur ne fait que des nombres entiers d'echantillons, impose que les filtres aient un retard entier, donc nbre de coeffs impair
			HighPassSamples1=((HighPassSamples0*5)/7);//ratio pour que les zeros de sinc tombent sur les rebonds du filtre précédent
		}
		if(fMax[c]>0)
		{
			LowPassSamples0=V_Floor(FrequencyHz/fMax[c]);//le delai pur ne fait que des nombres entiers d'echantillons, impose que les filtres aient un retard entier, donc nbre de coeffs impair
			LowPassSamples1=((LowPassSamples0*7)/5);//ratio pour que les zeros de sinc tombent sur les rebonds du filtre précédent
		}

		HighPassSamples0|=1; HighPassSamples1|=1; LowPassSamples0|=1; LowPassSamples1|=1;//le delai pur ne fait que des nombres entiers d'echantillons, impose que les filtres aient un retard entier, donc nbre de coeffs impair

		//int DelayH=((HighPassSamples0|1)-1)/2+((HighPassSamples1|1)-1)/2;		int DelayL=((LowPassSamples0|1)-1)/2+((LowPassSamples1|1)-1)/2;
		//int DelayZ=V_Max(DelayH,DelayL);//equilibre les retards des filtres low, high et neutre
		//int zDelayL=DelayZ-DelayL;	int zDelayH=DelayZ-DelayH;	

		//SPG_FastConvInit(CF[c].FZ,DelayZ);

		SPG_FastConvInit(CF[c].FL0,LowPassSamples0);					SPG_FastConvInit(CF[c].FH0,HighPassSamples0);		
		SPG_FastConvInit(CF[c].FL1,LowPassSamples1);					SPG_FastConvInit(CF[c].FH1,HighPassSamples1);	
		//SPG_FastConvInit(CF[c].FLZ,zDelayL);									SPG_FastConvInit(CF[c].FHZ,zDelayH);
	}

	int MaxDelayZ=0;
	for(int c=0;c<NumChannels;c++)
	{
		int DelayH=(CF[c].FH0.FilterLen-1)/2+(CF[c].FH1.FilterLen-1)/2;		int DelayL=(CF[c].FL0.FilterLen-1)/2+(CF[c].FL1.FilterLen-1)/2;
		CHECK((DelayL>0) && (DelayH<=DelayL),"CF_Init: Check frequency range",;);
		int DelayZ=V_Max(DelayH,DelayL);//equilibre les retards des filtres low, high
		MaxDelayZ=V_Max(MaxDelayZ,DelayZ);
	}

	for(int c=0;c<NumChannels;c++)
	{
		int DelayH=(CF[c].FH0.FilterLen-1)/2+(CF[c].FH1.FilterLen-1)/2;		int DelayL=(CF[c].FL0.FilterLen-1)/2+(CF[c].FL1.FilterLen-1)/2;
		int zDelayL=MaxDelayZ-DelayL;	int zDelayH=MaxDelayZ-DelayH;	
		SPG_FastConvInit(CF[c].FZ,1+MaxDelayZ); //filtre neutre pour AI non filtré et pour DI
		SPG_FastConvInit(CF[c].FBZ,1+MaxDelayZ); //filtre neutre pour AI non filtré et pour DI (note : on n'utilise que CF[0] pour le signal binaire le reste est inutile)
		SPG_FastConvInit(CF[c].FLZ,1+zDelayL);									SPG_FastConvInit(CF[c].FHZ,1+zDelayH);
	}
	return CF;
}

/*
RSFASTCONVOLVE_CHANNEL_FILT* SPG_CONV CF_Init(int NumChannels, int HighPassSamples0, int HighPassSamples1, int LowPassSamples0, int LowPassSamples1)
{//Obsolète
	RSFASTCONVOLVE_CHANNEL_FILT* CF=SPG_TypeAlloc(NumChannels,RSFASTCONVOLVE_CHANNEL_FILT,"CF_Init");
	HighPassSamples0|=1;//le delai pur ne fait que des nombres entiers d'echantillons, impose que les filtres aient un retard entier, donc nbre de coeffs impair
	HighPassSamples1|=1;
	LowPassSamples0|=1;//le delai pur ne fait que des nombres entiers d'echantillons, impose que les filtres aient un retard entier, donc nbre de coeffs impair
	LowPassSamples1|=1;
	for(int c=0;c<NumChannels;c++)
	{
		int DelayH=((HighPassSamples0|1)-1)/2+((HighPassSamples1|1)-1)/2;		int DelayL=((LowPassSamples0|1)-1)/2+((LowPassSamples1|1)-1)/2;
		int DelayZ=V_Max(DelayH,DelayL);//equilibre les retards des filtres low, high et neutre
		int zDelayL=DelayZ-DelayL;	int zDelayH=DelayZ-DelayH;	

		SPG_FastConvInit(CF[c].FZ,DelayZ);

		SPG_FastConvInit(CF[c].FL0,LowPassSamples0);					SPG_FastConvInit(CF[c].FH0,HighPassSamples0);		
		SPG_FastConvInit(CF[c].FL1,LowPassSamples1);					SPG_FastConvInit(CF[c].FH1,HighPassSamples1);	
		SPG_FastConvInit(CF[c].FLZ,zDelayL);									SPG_FastConvInit(CF[c].FHZ,zDelayH);
	}
	return CF;
}
*/

int SPG_CONV CF_Check(FASTCONVOLVE_CHANNEL_FILT& CF)
{//ne teste que les filtres pas les lignes à retard
	return 		SPG_FastConvCheck(CF.FH0) || SPG_FastConvCheck(CF.FH1) ||
					SPG_FastConvCheck(CF.FL0) || SPG_FastConvCheck(CF.FL1);
}

void SPG_CONV CF_Reset(int NumChannels, FASTCONVOLVE_CHANNEL_FILT* CF)
{
	for(int c=0;c<NumChannels;c++)
	{
		SPG_FastConvReset(CF[c].FZ);
		SPG_FastConvReset(CF[c].FBZ);
		SPG_FastConvReset(CF[c].FH0);	SPG_FastConvReset(CF[c].FH1);	SPG_FastConvReset(CF[c].FHZ);
		SPG_FastConvReset(CF[c].FL0);		SPG_FastConvReset(CF[c].FL1);		SPG_FastConvReset(CF[c].FLZ);
	}
}

void SPG_CONV CF_Close(int NumChannels, FASTCONVOLVE_CHANNEL_FILT* &CF)
{
	if(CF==0) return;
	for(int c=0;c<NumChannels;c++)
	{
		SPG_FastConvClose(CF[c].FZ);
		SPG_FastConvClose(CF[c].FBZ);
		SPG_FastConvClose(CF[c].FH0);	SPG_FastConvClose(CF[c].FH1);	SPG_FastConvClose(CF[c].FHZ);
		SPG_FastConvClose(CF[c].FL0);		SPG_FastConvClose(CF[c].FL1);		SPG_FastConvClose(CF[c].FLZ);
	}
	SPG_MemFree(CF);	//remet à zero même le pointeur, lui-même recu et passé par référence
}

//differentes implementations
//efine CF_OneChannel(n,NumChannels,c) {RS_SAMPLE& S=*DataInOutPureDelay++; RS_SAMPLE H,L; SPG_FastConvFiltIn(CF[c].FH0,S); H=SPG_FastConvFiltOut(CF[c].FH0); SPG_FastConvFiltIn(CF[c].FL0,S); L=SPG_FastConvFiltOut(CF[c].FL0); SPG_FastConvFilt(CF[c].FH1,H); SPG_FastConvFilt(CF[c].FL1,L); SPG_FastConvz(CF[c].FHZ,H); SPG_FastConvz(CF[c].FLZ,L); *DataOutFilt++=L-H; SPG_FastConvz(CF[c].FZ,S); }
//efine CF_NextSample(NumChannels)
//efine CF_OneChannel(n,NumChannels,c) {RS_SAMPLE& S=DataInOutPureDelay[n*NumChannels+c]; SPG_FastConvFiltIn(CF[c].FH0,S); SPG_FastConvFiltIn(CF[c].FL0,S); SPG_FastConvz(CF[c].FZ,S); RS_SAMPLE H,L; H=SPG_FastConvFiltOut(CF[c].FH0); L=SPG_FastConvFiltOut(CF[c].FL0); SPG_FastConvFilt(CF[c].FH1,H); SPG_FastConvFilt(CF[c].FL1,L); SPG_FastConvz(CF[c].FHZ,H); SPG_FastConvz(CF[c].FLZ,L); DataOutFilt[n*NumChannels+c]=L-H;  }
//efine CF_NextSample(NumChannels)
//efine CF_OneChannel(n,NumChannels,c) { SPG_FastConvFiltIn(CF[c].FL0,DataInOutPureDelay[c]); SPG_FastConvFiltIn(CF[c].FH0,DataInOutPureDelay[c]); SPG_FastConvz(CF[c].FZ,DataInOutPureDelay[c]); SPG_FastConvFiltIn(CF[c].FL1,SPG_FastConvFiltOut(CF[c].FL0)); SPG_FastConvFiltIn(CF[c].FH1,SPG_FastConvFiltOut(CF[c].FH0)); SPG_FastConvzIn(CF[c].FLZ,SPG_FastConvFiltOut(CF[c].FL1)); DataOutFilt[c]=SPG_FastConvzOut(CF[c].FLZ)-SPG_FastConvFiltOut(CF[c].FH1);  }

//FASTCONVOLVE_CHANNEL_FILT* __restrict CF

/*
#define CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c) {																																										\
	SPG_FastConvFiltIn(CF[c].FL0,DataInOutPureDelay[c]);					SPG_FastConvFiltIn(CF[c].FH0,DataInOutPureDelay[c]);					SPG_FastConvz(CF[c].FZ,DataInOutPureDelay[c]);	\
	SPG_FastConvFiltIn(CF[c].FL1,SPG_FastConvFiltOut(CF[c].FL0));	SPG_FastConvFiltIn(CF[c].FH1,SPG_FastConvFiltOut(CF[c].FH0));																						\
	SPG_FastConvzIn(CF[c].FLZ,SPG_FastConvFiltOut(CF[c].FL1));		SPG_FastConvzIn(CF[c].FHZ,SPG_FastConvFiltOut(CF[c].FH1));																							\
	DataOutFilt[c]=SPG_FastConvzOut(CF[c].FLZ)-SPG_FastConvzOut(CF[c].FHZ);																																											}
*/



#define CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c) {			FASTCONVOLVE_CHANNEL_FILT& cCF=CF[c];																			\
	SPG_FastConvFiltIn(cCF.FL0,DataInOutPureDelay[c]);				SPG_FastConvFiltIn(cCF.FH0,DataInOutPureDelay[c]);					SPG_FastConvz(cCF.FZ,DataInOutPureDelay[c]);	\
	SPG_FastConvFiltIn(cCF.FL1,SPG_FastConvFiltOut(cCF.FL0));	SPG_FastConvFiltIn(cCF.FH1,SPG_FastConvFiltOut(cCF.FH0));																						\
	SPG_FastConvzIn(cCF.FLZ,SPG_FastConvFiltOut(cCF.FL1));		SPG_FastConvzIn(cCF.FHZ,SPG_FastConvFiltOut(cCF.FH1));																							\
	DataOutFilt[c]=SPG_FastConvzOut(cCF.FLZ)-SPG_FastConvzOut(cCF.FHZ);																																											}

/*
__inline void CF_OneChannel(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, RS_SAMPLE* __restrict DataInOutPureDelay, RS_SAMPLE* __restrict DataOutFilt, int n, int NumChannels, int c) {																																																									\
	SPG_FastConvFiltIn(CF[c].FL0,DataInOutPureDelay[c]);					SPG_FastConvFiltIn(CF[c].FH0,DataInOutPureDelay[c]);					SPG_FastConvz(CF[c].FZ,DataInOutPureDelay[c]);	\
	SPG_FastConvFiltIn(CF[c].FL1,SPG_FastConvFiltOut(CF[c].FL0));	SPG_FastConvFiltIn(CF[c].FH1,SPG_FastConvFiltOut(CF[c].FH0));																						\
	SPG_FastConvzIn(CF[c].FLZ,SPG_FastConvFiltOut(CF[c].FL1));		SPG_FastConvzIn(CF[c].FHZ,SPG_FastConvFiltOut(CF[c].FH1));																							\
	DataOutFilt[c]=SPG_FastConvzOut(CF[c].FLZ)-SPG_FastConvzOut(CF[c].FHZ);																																											}
*/
#define CF_NextSample(NumChannels) { DataInOutPureDelay+=NumChannels; DataOutFilt+=NumChannels; }

/*

// ############ TECHNIQUE 1 : UNROLLING IN EXTENSO

//implementation avec nbre de channel fixe (switch/case au cas par cas)
void SPG_CONV CF_Process_Unroll1(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,1,0);	CF_NextSample(1); }	}
void SPG_CONV CF_Process_Unroll2(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,2,0);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,2,1); CF_NextSample(2);	}	}
void SPG_CONV CF_Process_Unroll3(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,3,0);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,3,1);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,3,2); CF_NextSample(3);	}	}
void SPG_CONV CF_Process_Unroll4(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,4,0);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,4,1);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,4,2);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,4,3); CF_NextSample(4);	}	}
void SPG_CONV CF_Process_Unroll5(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,5,0);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,5,1);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,5,2);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,5,3);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,5,4); CF_NextSample(5);	}	}
void SPG_CONV CF_Process_Unroll6(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,0);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,1);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,2);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,3);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,4);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,5); CF_NextSample(6);	}	}
void SPG_CONV CF_Process_Unroll7(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,0);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,1);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,2);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,3);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,4);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,5);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,6); CF_NextSample(7);	}	}
void SPG_CONV CF_Process_Unroll8(FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)	{	for(int n=0;n<NumS;n++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,0);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,1);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,2);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,3);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,4);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,5);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,6);CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,7); CF_NextSample(8);	}	}

void SPG_CONV CF_Process(const int NumChannels, FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)
{
	switch(NumChannels)
	{
	case 1: CF_Process_Unroll1(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 2: CF_Process_Unroll2(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 3: CF_Process_Unroll3(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 4: CF_Process_Unroll4(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 5: CF_Process_Unroll5(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 6: CF_Process_Unroll6(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 7: CF_Process_Unroll7(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 8: CF_Process_Unroll8(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	default:
		{
			for(int n=0;n<NumS;n++)	{	for(int c=0;c<NumChannels;c++)	{ CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c); } CF_NextSample(NumChannels);	}
		} break;
	}
	return;
}

void SPG_CONV CF_Process(const int NumChannels, FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, BYTE* __restrict BDataInOutPureDelay, const int NumS)
{
	switch(NumChannels)
	{
	case 1: CF_Process_Unroll1(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 2: CF_Process_Unroll2(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 3: CF_Process_Unroll3(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 4: CF_Process_Unroll4(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 5: CF_Process_Unroll5(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 6: CF_Process_Unroll6(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 7: CF_Process_Unroll7(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	case 8: CF_Process_Unroll8(CF,DataInOutPureDelay,DataOutFilt,NumS); break;
	default:
		{
			for(int n=0;n<NumS;n++)	{	for(int c=0;c<NumChannels;c++)	{ CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c); } CF_NextSample(NumChannels);	}
		} break;
	}
	{for(int n=0;n<NumS;n++)
	{
		SPG_FastConvz(CF[0].FBZ,BDataInOutPureDelay[n]);
	}}
	return;
}
// ############ TECHNIQUE 1 : UNROLLING IN EXTENSO

*/


// ############ TECHNIQUE 2 : BOUCLE NON UNROLLEE, CAS PAR CAS

//meilleure implémentation au 01/04/2011

void SPG_CONV CF_Process(const int NumChannels, FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)
{
	switch(NumChannels)
	{
	case 1: for(int n=0;n<NumS;n++) { for(int c=0;c<1;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,1,c); }	CF_NextSample(NumChannels); } break;
	case 2: for(int n=0;n<NumS;n++) { for(int c=0;c<2;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,2,c); }	CF_NextSample(NumChannels); } break;
	case 3: for(int n=0;n<NumS;n++) { for(int c=0;c<3;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,3,c); }	CF_NextSample(NumChannels); } break;
	case 4: for(int n=0;n<NumS;n++) { for(int c=0;c<4;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,4,c); }	CF_NextSample(NumChannels); } break;
	case 5: for(int n=0;n<NumS;n++) { for(int c=0;c<5;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,5,c); }	CF_NextSample(NumChannels); } break;
	case 6: for(int n=0;n<NumS;n++) { for(int c=0;c<6;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,c); }	CF_NextSample(NumChannels); } break;
	case 7: for(int n=0;n<NumS;n++) { for(int c=0;c<7;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,c); }	CF_NextSample(NumChannels); } break;
	case 8: for(int n=0;n<NumS;n++) { for(int c=0;c<8;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,c); }	CF_NextSample(NumChannels); } break;
	default:
		{
			for(int n=0;n<NumS;n++)	{	for(int c=0;c<NumChannels;c++)	{ CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c); } CF_NextSample(NumChannels);	}
		} break;
	}
	return;
}

void SPG_CONV CF_Process(const int NumChannels, FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, BYTE* __restrict BDataInOutPureDelay, const int NumS)
{
	switch(NumChannels)
	{
	case 1: for(int n=0;n<NumS;n++) { for(int c=0;c<1;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,1,c); }	CF_NextSample(NumChannels); } break;
	case 2: for(int n=0;n<NumS;n++) { for(int c=0;c<2;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,2,c); }	CF_NextSample(NumChannels); } break;
	case 3: for(int n=0;n<NumS;n++) { for(int c=0;c<3;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,3,c); }	CF_NextSample(NumChannels); } break;
	case 4: for(int n=0;n<NumS;n++) { for(int c=0;c<4;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,4,c); }	CF_NextSample(NumChannels); } break;
	case 5: for(int n=0;n<NumS;n++) { for(int c=0;c<5;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,5,c); }	CF_NextSample(NumChannels); } break;
	case 6: for(int n=0;n<NumS;n++) { for(int c=0;c<6;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,6,c); }	CF_NextSample(NumChannels); } break;
	case 7: for(int n=0;n<NumS;n++) { for(int c=0;c<7;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,7,c); }	CF_NextSample(NumChannels); } break;
	case 8: for(int n=0;n<NumS;n++) { for(int c=0;c<8;c++)	{	CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,8,c); }	CF_NextSample(NumChannels); } break;
	default:
		{
			for(int n=0;n<NumS;n++)	{	for(int c=0;c<NumChannels;c++)	{ CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c); } CF_NextSample(NumChannels);	}
		} break;
	}
	{for(int n=0;n<NumS;n++)
	{
		SPG_FastConvz(CF[0].FBZ,BDataInOutPureDelay[n]);
	}}
	return;
}

// ############ TECHNIQUE 2 : BOUCLE NON UNROLLEE, CAS PAR CAS

/*
// ############ TECHNIQUE 3 : BOUCLE NON UNROLLEE GENERIQUE

//Implementation par boucle sur les channels
void SPG_CONV CF_Process(const int NumChannels, FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, const int NumS)
{
	for(int n=0;n<NumS;n++)
	{
		__assume(NumChannels<9);
		for(int c=0;c<NumChannels;c++)
		{
			CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c);
		}
		CF_NextSample(NumChannels);
	}
	return;
}

//Implementation par boucle sur les channels+DI
void SPG_CONV CF_Process(const int NumChannels, FASTCONVOLVE_CHANNEL_FILT* __restrict CF, short* __restrict DataInOutPureDelay, short* __restrict DataOutFilt, BYTE* __restrict BDataInOutPureDelay, const int NumS)
{
	{for(int n=0;n<NumS;n++)
	{
		__assume(NumChannels<9);
		for(int c=0;c<NumChannels;c++)
		{
			CF_OneChannel(CF,DataInOutPureDelay,DataOutFilt,n,NumChannels,c);
		}
		CF_NextSample(NumChannels);
	}}
	{for(int n=0;n<NumS;n++)
	{
		SPG_FastConvz(CF[0].FBZ,BDataInOutPureDelay[n]);
	}}
	return;
}

// ############ TECHNIQUE 3 : BOUCLE NON UNROLLEE GENERIQUE
*/
/*
void SPG_CONV CF_ProcessShowFiltered(int NumChannels, RSFASTCONVOLVE_CHANNEL_FILT* __restrict CF, RS_SAMPLE* __restrict DataInOutPureDelay, RS_SAMPLE* __restrict DataOutFilt, int NumS)
{
	for(int n=0;n<NumS;n++)
	{
		for(int c=0;c<NumChannels;c++)
		{
			RS_SAMPLE& S=DataInOutPureDelay[n*NumChannels+c];

			RS_SAMPLE H,L;
			SPG_FastConvFiltIn(CF[c].FH0,S);	H=SPG_FastConvFiltOut(CF[c].FH0);				SPG_FastConvFiltIn(CF[c].FL0,S);	L=SPG_FastConvFiltOut(CF[c].FL0);
			SPG_FastConvFiltIn(CF[c].FH1,H);	H=SPG_FastConvFiltOut(CF[c].FH1);				SPG_FastConvFiltIn(CF[c].FL1,L);	L=SPG_FastConvFiltOut(CF[c].FL1);
			SPG_FastConvzIn(CF[c].FHZ,H);	H=SPG_FastConvzOut(CF[c].FHZ);						SPG_FastConvzIn(CF[c].FLZ,L);	L=SPG_FastConvzOut(CF[c].FLZ);
			DataOutFilt[n*NumChannels+c]=S=L-H;
		}
	}
	return;
}
*/



#ifdef SPG_General_USEBackupMe

int SPG_CONV BackupMe(char* decl, RSFASTCONVOLVE& F, SCX_CONNEXION* C)
{
	BMW(sprintf(S,"%s = {",decl));
	SBM(F,	int,	x,		C);
	SBM(F,	int,	FilterLen,		C);
	SBM(F,	float,	invFilterLen,		C);
	SBM(F,	int,	RingMsk,		C);
	SBM(F,	int,	RingLen,		C);
	SBM(F,	int,	Sum,		C);
	scxWriteStrZ("}\r\n",C);
	return -1;
}

int SPG_CONV BackupMe(char* decl, RBFASTCONVOLVE& F, SCX_CONNEXION* C)
{
	BMW(sprintf(S,"%s = {",decl));
	SBM(F,	int,	x,		C);
	SBM(F,	int,	FilterLen,		C);
	SBM(F,	float,	invFilterLen,		C);
	SBM(F,	int,	RingMsk,		C);
	SBM(F,	int,	RingLen,		C);
	scxWriteStrZ("}\r\n",C);
	return -1;
}


int SPG_CONV BackupMe(char* decl, FASTCONVOLVE_CHANNEL_FILT& F, SCX_CONNEXION* C)
{
	BMW(sprintf(S,"%s = {",decl));
	SBM(F,	RBFASTCONVOLVE, FBZ,		C);
	SBM(F,	RSFASTCONVOLVE, FL0,		C);
	SBM(F,	RSFASTCONVOLVE, FH0,		C);
	SBM(F,	RSFASTCONVOLVE, FZ,		C);
	SBM(F,	RSFASTCONVOLVE, FL1,		C);
	SBM(F,	RSFASTCONVOLVE, FH1,		C);
	SBM(F,	RSFASTCONVOLVE, FLZ,		C);
	SBM(F,	RSFASTCONVOLVE, FHZ,		C);
	scxWriteStrZ("}\r\n",C);
	return -1;
}

#endif
