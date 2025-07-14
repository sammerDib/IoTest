
#include "SPG_General.h"

#ifdef SPG_General_USEGraphicHistoryCurves

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include <string.h>
#include <float.h>

//fcts GH_xxx Donnees du graphe GHW_xxx fenetre d'affichage


// ##############################            DONNEES            ############################

int SPG_CONV GH_Init(GHISTORY& GH, int MaxCurves, int MaxRecords)
{
	SPG_ZeroStruct(GH);
	LTG_Create(GH,LT_FLAG_NOCHECK  ,LT_GH,0);

	LTG_EnterI(GH,LT_GH,0);
	GH.MaxCurves=MaxCurves;
	GH.MaxRecords=MaxRecords;
	GH.NumCurves=GH.MaxCurves;
	GH.szCurveHeader=sizeof(GHCURVESHEADER);
	GH.szCurve=sizeof(GHSAMPLE);
	GH.szRecord=GH.szCurveHeader+GH.MaxCurves*GH.szCurve;
	GH.HC=SPG_MemAlloc(GH.MaxRecords*GH.szRecord,"GH.HC");
	LTG_ExitI(GH,LT_GH,0);
	return -1;
}

void SPG_CONV GH_Close(GHISTORY& GH)
{
	LTG_EnterC(GH,LT_GH,0);
	SPG_MemFree(GH.HC);
	LTG_ExitC(GH,LT_GH,0);
	SPG_ZeroStruct(GH);
	return;
}

//determine le plus grand intervalle a sousechantillonner par deux pour liberer de l'espace pour les nouveaux echantillons
int SPG_CONV GH_FindLongestGroup(GHISTORY& GH, int& Weight, int& Start, int& Len)
{
	int rw=HCR(GH,0)->hch.Weight;
	Len=0;	int rStart=0;	int r;
	for(r=1;r<GH.NumRecords;r++)
	{
		int nw=HCR(GH,r)->hch.Weight;
		if(nw!=rw)
		{
			if((r-rStart)>Len)	{	Len=r-rStart;	Start=rStart;	Weight=rw;	}
			rStart=r;	rw=nw;
		}
	}
	if((r-rStart)>Len)	{	Len=r-rStart;	Start=rStart;	Weight=rw;	}
	return -1;
}

//reechantillonne un intervalle, lit les echantillons à l'endroit pointé par ReadR et ecrit à l'endroit pointé à WriteR.  Ecrit un echantillon pour deux lus.
int SPG_CONV GH_WriteGroup(GHISTORY& GH, int& WriteR, int& ReadR, int WeightWrite, int WeightRead, int NumRecords)
{
	BYTE* pW=GH.HC+WriteR*GH.szRecord;	BYTE* pR=GH.HC+ReadR*GH.szRecord;
	for(int i=0;i<NumRecords;i++)
	{
		GHRECORD& W=*(GHRECORD*)pW;		GHRECORD& R0=*(GHRECORD*)pR;		GHRECORD& R1=*(GHRECORD*)(pR+GH.szRecord);

		DbgCHECK((R0.hch.Weight!=WeightRead)||(R1.hch.Weight!=WeightRead),"GH_WriteGroup");
		W.hch.Weight=WeightWrite;	W.hch.FlagOR=R0.hch.FlagOR|R1.hch.FlagOR;	W.hch.FlagAND=R0.hch.FlagAND&R1.hch.FlagAND;
		for(int c=0;c<GH.NumCurves;c++)	{	W.hcs[c].fMin=V_Min(R0.hcs[c].fMin,R1.hcs[c].fMin);	W.hcs[c].fMean=0.5*(R0.hcs[c].fMean+R1.hcs[c].fMean);	W.hcs[c].fMax=V_Max(R0.hcs[c].fMax,R1.hcs[c].fMax);	}
		pW+=GH.szRecord;									pR+=2*GH.szRecord;
	}
	WriteR+=NumRecords;									ReadR+=2*NumRecords;
	return -1;
}

//decalle les données à la suite de la partie réechantillonnée pour remettre toutes les donnees consecutives
int SPG_CONV GH_WriteRemainer(GHISTORY& GH, int& WriteR, int& ReadR)
{
	CHECK(GH.NumRecords-ReadR<=0,"GH_WriteRemainer",return 0);
	memmove(GH.HC+WriteR*GH.szRecord, GH.HC+ReadR*GH.szRecord, (GH.NumRecords-ReadR)*GH.szRecord);
	GH.NumRecords+=WriteR-ReadR;
	return -1;
}

//operation complete de reechantillonnage: identifie un intervalle, le reechantillonne, et recalle les donnees situées apres
void SPG_CONV GH_Group(GHISTORY& GH)
{
	//int MaxExcess, MaxR;		GH_FindHighestGroup(GH,MaxExcess,MaxR);
	int Weight,Start,Len;			GH_FindLongestGroup(GH,Weight,Start,Len);
	int NumRecords=(Len+2)/3;
	int ReadR=Start;				GH_WriteGroup(GH, Start, ReadR, Weight+1, Weight, NumRecords);
												GH_WriteRemainer(GH, Start, ReadR);
}

void SPG_CONV GH_Save(GHISTORY& GH, char* FileName)
{
	FILE* F = fopen(FileName,"ab+");

	int rw=HCR(GH,GH.NumRecords-1)->hch.Weight;
	int Len=1;

	for(int r=GH.NumRecords-2;r>=0;r--)
	{
		int nw=HCR(GH,r)->hch.Weight;
		if(nw==rw) {Len++;}	else {fprintf(F,"%i\t",Len,rw); Len=1; rw=nw;}
	}
	fprintf(F,"%i\t\r\n",Len,rw);
	fclose(F);
}

//ajoute un echantillon, regroupe pour liberer de la place si necessaire (environ une fois tous les dizaines de milliers)
void SPG_CONV GH_Add(GHISTORY& GH, double* f, BYTE FlagOR, BYTE FlagAND)
{
	LTG_EnterEH(GH,LT_GH,0);
	if(GH.NumRecords==GH.MaxRecords)  { /* GH_Save(GH,"GH.txt"); */ LTG_EnterEH(GH,LT_GH,0);GH_Group(GH);LTG_ExitEH(GH,LT_GH,0); /* GH_Save(GH,"GH.txt"); */ }
	CHECK(GH.NumRecords==GH.MaxRecords,"GH_Add",return);
	int r=GH.NumRecords++;
	GHRECORD& hc=*HCR(GH,r);
	hc.hch.Weight=1;		hc   .hch.FlagOR=FlagOR;		hc.hch.FlagOR=FlagAND;
	GHSAMPLE* hcs=hc.hcs;
	for(int c=0;c<GH.NumCurves;c++)		{	hcs[c].fMax=hcs[c].fMean=hcs[c].fMin=f[c];	}
	LTG_ExitEH(GH,LT_GH,0);
	return;
}

void SPG_CONV GH_Add(GHISTORY& GH, double* fmin, double* fmean, double* fmax, BYTE FlagOR, BYTE FlagAND)
{
	LTG_EnterEH(GH,LT_GH,0);
	if(GH.NumRecords==GH.MaxRecords)  { /* GH_Save(GH,"GH.txt"); */ GH_Group(GH); /* GH_Save(GH,"GH.txt"); */ }
	CHECK(GH.NumRecords==GH.MaxRecords,"GH_Add",return);
	int r=GH.NumRecords++;
	GHRECORD& hc=*HCR(GH,r);
	hc.hch.Weight=1;		hc.hch.FlagOR=FlagOR;		hc.hch.FlagOR=FlagAND;
	GHSAMPLE* hcs=hc.hcs;
	for(int c=0;c<GH.NumCurves;c++)		{	hcs[c].fMin=fmin[c];hcs[c].fMean=fmean[c];hcs[c].fMax=fmax[c];	}
	LTG_ExitEH(GH,LT_GH,0);
	return;
}

// ##############################            AFFICHAGE            ############################

DWORD SPG_CONV GHW_Color(int n)
{
	DWORD Coul=0;
	int nchannels=3;//B V R 
	int channelwidth=8;
	for(int depth=0;depth<channelwidth;depth++)
	{
		for(int nchannel=0;nchannel<3;nchannel++)
		{
			//src
			if(n&(1<<(nchannel+depth*nchannels))) Coul|=1<<(channelwidth-1-depth+channelwidth*nchannel);
		}
	}
	return Coul;
}

int SPG_CONV GHW_Init(GHISTORYWINDOW& GHW, GHISTORY& GH, double AutoTrigSpan, int AutoTrigPixSpan)
{
	SPG_ZeroStruct(GHW);
	LTG_Create(GHW,LT_FLAG_NOCHECK  ,LT_GHW,0);

	LTG_EnterI(GHW,LT_GHW,0);
	GHW.GH=&GH;
	GHW.ROX.AutoTrigSpan=AutoTrigSpan;
	GHW.ROX.AutoTrigPixSpan=AutoTrigPixSpan;

	GHW.ROC=SPG_TypeAlloc(GHW.GH->MaxCurves,GHCURVERENDEROPTS,"GHW.ROC");
	for(int c=0;c<GHW.GH->MaxCurves;c++)
	{
		if(c==0)	{ sprintf(GHW.ROC[c].Label,"X",c); } else { sprintf(GHW.ROC[c].Label,"Y[%i]",c); }
		GHW.ROC[c].LegendPosition=c;
		GHW.ROC[c].ScaleGroup=0;
		GHW.ROC[c].Mean.Coul=GHW_Color(c);
		GHW.ROC[c].Max.Coul=GHW.ROC[c].Min.Coul = ( (GHW.ROC[c].Mean.Coul&0xFCFCFC) >> 2 ) + 3 * ( (0xFCFCFC) >> 2 );
	}
	GHW.SCG=SPG_TypeAlloc(GHW.GH->MaxScales,GHCURVESCALEGROUP,"GHW.SCG");
	for(int sg=0;sg<GHW.GH->MaxScales;sg++)
	{
		GHW.SCG[sg].ScaleYMode=GSC_AUTO;//auto
		GHW.SCG[sg].k_min=4;
		GHW.SCG[sg].k_max=8;
		GHW.SCG[sg].NumDigits=CF_DIGITFLOAT;
		SC_Init(GHW.SCG[sg].SC,6,1,0.1);//le nombre de divisions est regle automatiquement dans GHW_ComputeScales
	}
	GHW.ROX.ScaleXMode=0;//full
	SC_Init(GHW.ROX.SC,6,1);//le nombre de divisions est regle automatiquement dans GHW_ComputeScales

	GHW.hcr[1]=(GHRECORD*)(GH.szRecord+(BYTE*)(GHW.hcr[0]=(GHRECORD*)SPG_MemAlloc(2*GH.szRecord,"GHW.hcs")));

	S_InitTimer(GHW.T,"GHW.T");
	S_StartTimer(GHW.T);
	LTG_ExitI(GHW,LT_GHW,0);
	return -1;
}

#ifdef SPG_General_USECONFIGFILE
//lit le fichier de parametres pour permettre un reglage manuel des echelles qui soit different des valeurs par defaut mises par le soft
void SPG_CONV GHW_ReadScaleMode(GHISTORYWINDOW& GHW, SPG_CONFIGFILE& CFG, char* Prefix)
{
	if(Prefix==0) Prefix="";
	for(int sg=0;sg<=GHW.GH->MaxScales;sg++)
	{
		char cmt[1024];	char* cvol=cmt;	*cvol=0;

		int c;
		for(c=1;c<GHW.GH->MaxCurves;c++)	{	if(GHW.ROC[c].ScaleGroup==sg)	{	cvol+=sprintf(cvol,"%s",GHW.ROC[c].Label);	break;	}	}
		if(c==GHW.GH->MaxCurves) continue;
		for(c++;c<GHW.GH->MaxCurves;c++)	{	if(GHW.ROC[c].ScaleGroup==sg)	{	cvol+=sprintf(cvol,",%s",GHW.ROC[c].Label);	if(cvol-cmt>512) {cvol+=sprintf(cvol,", ...",GHW.ROC[c].Label);break;}	}	}
		
		char Name[128];	sprintf(Name,"%sGHW.SCG[%i].ScaleYMode",Prefix,sg);
		CFG_IntParam(CFG,Name,(int*)&GHW.SCG[sg].ScaleYMode,"0:auto 1:auto,zero included 2:fixed to specified min,max",1);

		sprintf(Name,"%sGHW.SCG[%i].fMin",Prefix,sg);		CFG_DoubleParam(CFG,Name,&GHW.SCG[sg].fMin,cmt,1);
		sprintf(Name,"%sGHW.SCG[%i].fMax",Prefix,sg);	CFG_DoubleParam(CFG,Name,&GHW.SCG[sg].fMax,cmt,1);
		sprintf(Name,"%sGHW.SCG[%i].NumDigits",Prefix,sg);	CFG_IntParam(CFG,Name,(int*)&GHW.SCG[sg].NumDigits,"Digits on text display",CF_DIGITFLOAT);
	}
	return;
}
#endif

void SPG_CONV GHW_Close(GHISTORYWINDOW& GHW)
{
	LTG_EnterC(GHW,LT_GHW,0);
	S_StopTimer(GHW.T);
	S_CloseTimer(GHW.T);
	SPG_MemFree(GHW.ROC);
	SPG_MemFree(GHW.SCG);
	//SPG_MemFree(GHW.ROX);
	SPG_MemFree(GHW.hcr[0]);
	LTG_ExitC(GHW,LT_GHW,0);
	SPG_ZeroStruct(GHW);
}

//fonction d'ecriture de texte sur la fenetre
void SPG_CONV GHW_Print(G_Ecran& E, int X,int Y, char* Msg, C_Lib& CL,int Alignement, DWORD Couleur)
{
	C_PrintUni(E, X        , Y         , Msg,CL, Alignement, 0xFFFFFF );
	C_PrintUni(E, X        , Y+1    , Msg,CL, Alignement, 0xFFFFFF );
	C_PrintUni(E, X        , Y+2    , Msg,CL, Alignement, 0xFFFFFF );

	C_PrintUni(E, X+1    , Y        , Msg,CL, Alignement, 0xFFFFFF );

	C_PrintUni(E, X+1    , Y+2  , Msg,CL, Alignement, 0xC0C0C0 );

	C_PrintUni(E, X+2    , Y       , Msg,CL, Alignement, 0xC0C0C0 );
	C_PrintUni(E, X+2    , Y+1  , Msg,CL, Alignement, 0xC0C0C0 );
	C_PrintUni(E, X+2    , Y+2  , Msg,CL, Alignement, 0x808080 );

	C_PrintUni(E, X+1        , Y+1      , Msg,CL, Alignement, Couleur);

	return;
}

//determine la plage X de la fenetre, selon le type d'echelle, le zoom, etc ...
void SPG_CONV GHW_ZoomPanXScale(GHISTORYWINDOW& GHW, bool HasFocus)
{
	GHXRENDEROPTS& GX = GHW.ROX;

#ifdef SPG_General_USETimer
	S_GetTimerRunningTimeAndRestart(GHW.T,GX.dt);
	GX.dt=V_Min(GX.dt,0.2);
#else
	GX.dt=0.02;
#endif

	if(GX.ScaleXMode==0)
	{//See all
		GX.Pan=GX.xMax;
		GX.Zoom=0;
		GX.LR=GX.xMax-GX.xMin;
		if(KEYPRESSED(VK_UP)) GX.ScaleXMode=2;
		if(KEYPRESSED(VK_LEFT)) GX.ScaleXMode=1;
		if( KEYSTROKE(VK_SPACE) && GX.Trigged && (GHW.GH->NumRecords >= GHW.ROX.AutoTrigPixSpan) && (GHW.ROX.LR>GHW.ROX.AutoTrigSpan) )
		{
			GX.ScaleXMode=2+8;
			//GX.LR=V_Min(GX.LR,GX.AutoTrigSpan);
			//double xMin=HCS((*(GHW.GH)),(GHW.GH->NumRecords-GHW.ROX.AutoTrigPixSpan))[0].fMin;
			//double xMax=HCS((*(GHW.GH)),(V_Max(GHW.GH->NumRecords-1,0)))[0].fMax;
			//GX.LR=V_Max(GX.LR,xMax-xMin);
		}
	}
	else if(GX.ScaleXMode==1)
	{//Pan / Zoom with cursor
		bool ZoomWasPreviouslyZero=(GX.Zoom<=0);
		GX.Pan=V_Sature(GX.Pan,GX.xMin,GX.xMax);
		GX.Zoom=V_Sature(GX.Zoom,0,64);
		if(KEYPRESSED(VK_UP)) GX.Zoom+=2*GX.dt;
		if(KEYPRESSED(VK_DOWN)) GX.Zoom-=2*GX.dt;
		if(KEYPRESSED(VK_LEFT)) GX.Pan-=0.8*GX.dt*(GX.xMax-GX.xMin)/pow(2,GX.Zoom);
		if(KEYPRESSED(VK_RIGHT)) GX.Pan+=0.8*GX.dt*(GX.xMax-GX.xMin)/pow(2,GX.Zoom);
		if(GX.Pan>=GX.xMax) GX.ScaleXMode=2;
		GX.Pan=V_Sature(GX.Pan,GX.xMin,GX.xMax);
		if((GX.Zoom<0)&&(ZoomWasPreviouslyZero)) GX.ScaleXMode=0;
		GX.Zoom=V_Sature(GX.Zoom,0,64);
		double L=GX.Pan-GX.xMin;
		double R=GX.xMax-GX.Pan;
		double X=GX.Pan;
		L/=pow(2,GX.Zoom);
		R/=pow(2,GX.Zoom);
		GX.xMin=X-L;
		GX.xMax=X+R;
		GX.LR=L+R;
		if(KEYSTROKE(VK_SPACE)) GX.ScaleXMode=2+8;
	}
	else if(GX.ScaleXMode==2)
	{//Pan fixed width at end
		if(KEYPRESSED(VK_UP)) GX.LR/=1+3*GX.dt*log((double)2);
		if(KEYPRESSED(VK_DOWN)) GX.LR*=1+3*GX.dt*log((double)2);
		if(GX.LR>GX.xMax-GX.xMin) GX.ScaleXMode=0;
		GX.LR=V_Min(GX.LR,GX.xMax-GX.xMin);
		GX.xMin=V_Max(GX.xMin,GX.xMax-GX.LR);
		GX.Pan=GX.xMax;
		GX.Zoom=log((double)GX.xMax/GX.LR)/log((double)2);
		if(KEYPRESSED(VK_LEFT)) GX.ScaleXMode=1;
		if(KEYSTROKE(VK_SPACE)) GX.ScaleXMode=0+8;
	}
	else if(GX.ScaleXMode==(0+8))
	{//switch to full view
		double LR=GX.xMax-GX.xMin;
		GX.LR/=(1-V_Min(2*GHW.ROX.dt,0.9));
		if(GX.LR>=LR)
		{
			GX.LR=LR;
			GX.ScaleXMode=0;
		}
		GX.xMin=V_Max(GX.xMin,GX.xMax-GX.LR);
		GX.Pan=GX.xMax;
	}
	else if(GX.ScaleXMode==(2+8))
	{//switch from full view to fixed pan with predefined width
			//GX.LR=V_Min(GX.LR,GX.AutoTrigSpan);
			double xMin=HCS((*(GHW.GH)),(V_Max(0,GHW.GH->NumRecords-GHW.ROX.AutoTrigPixSpan)))[0].fMin;
			double xMax=HCS((*(GHW.GH)),(V_Max(GHW.GH->NumRecords-1,0)))[0].fMax;
			double LR=V_Min(xMax-xMin,GHW.ROX.AutoTrigSpan);
			GX.LR*=(1-V_Min(2*GHW.ROX.dt,0.9));
			if(LR>=GX.LR)
			{
				GX.LR=LR;
				GX.ScaleXMode=2;
			}
			GX.xMin=V_Max(GX.xMin,GX.xMax-GX.LR);
			GX.Pan=GX.xMax;
	}
	return;
}

//calcule les graduations X
int SPG_CONV GHW_ComputeXScale(GHISTORYWINDOW& GHW, bool HasFocus)
{
	if(GHW.ROX.InhibitRescaleX==0)
	{
		GHW.ROX.xMin=HCS((*(GHW.GH)),0)[0].fMin;
		GHW.ROX.xMax=HCS((*(GHW.GH)),(GHW.GH->NumRecords-1))[0].fMax;
		GHW_ZoomPanXScale(GHW, HasFocus);
		if( (GHW.ROX.ScaleXMode==0) && (GHW.ROX.Trigged==0) )
		{
			if( (GHW.GH->NumRecords >= GHW.ROX.AutoTrigPixSpan) && (GHW.ROX.LR>GHW.ROX.AutoTrigSpan) ) 
			{
					GHW.ROX.Trigged=1;
					GHW.ROX.ScaleXMode=2;
					GHW.ROX.LR=V_Min(GHW.ROX.LR,GHW.ROX.AutoTrigSpan);
					double xMin=HCS((*(GHW.GH)),(V_Max(0,GHW.GH->NumRecords-GHW.ROX.AutoTrigPixSpan)))[0].fMin;
					double xMax=HCS((*(GHW.GH)),(V_Max(GHW.GH->NumRecords-1,0)))[0].fMax;
					GHW.ROX.LR=V_Max(GHW.ROX.LR,xMax-xMin);
			}
		}
	}
	SC_Set(GHW.ROX.SC,GHW.ROX.xMin,GHW.ROX.xMax);
	return -1;
}

//dessine les graduations X
int SPG_CONV GHW_RenderXScale(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL)
{
	if(CL==0) return 0;
	SPG_Scale& SC=GHW.ROX.SC;
	for(int tick=0;tick<SC.NumTick;tick++)
	{	
		int X=GHWX(GZ,GHW.ROX,SC.T[tick].Value);	int Y=GZ.YGB+1;
		if(V_IsBound(X,GZ.XGL,GZ.XGR))
		{
			GHW_Print(E,V_Max(X-strlen(SC.T[tick].Label)*CL->SizeX/2,0),Y,SC.T[tick].Label,*CL,0,GHW.ROC[0].Text.Coul);
			G_DrawLine(E,X,GZ.YGT,X,GZ.YGB,0x606060);
		}
	}
	{
		int X=GHWX(GZ,GHW.ROX,GHW.ROX.Pan);	int Y=GZ.YGB+1;
		G_DrawLine(E,X,GZ.YGT,X,GZ.YGB,0x00FF0000);
	}
	return -1;
}

//recherche les données correspondant à la zone visible
int SPG_CONV GHW_FindOrigin(GHISTORYWINDOW& GHW, int c)
{
	int s=0;
	int N=GHW.GH->NumRecords-1; for(int i=0;i<5;i++) N|=N>>(1<<i); N++;
	DbgCHECK(N&(N-1),"GHW_DrawCurve");
	for(int L=N/2;L>0;L>>=1)
	{
		if((s+L)>=GHW.GH->NumRecords) continue;
		GHRECORD* hcr=HCR((*(GHW.GH)),(s+L));
		if( hcr->hcs[0].fMax < GHW.ROX.xMin ) s+=L;
	}

	GHW.ROX.RecordOriginOnScale=s;
	return -1;
}

//compensation d'offset Y (spécifique affichage DTT) - la macro est nulle dans le cas general
#define ROCOFC(roc) (roc.OffsetCompensation?roc.fMean/V_Max(1,roc.nMean):0)

//compensation d'offset Y (spécifique affichage DTT)
int SPG_CONV GHW_ComputeOffset(GHISTORYWINDOW& GHW, int c)
{
	GHW.ROC[c].fMean=0;
	GHW.ROC[c].nMean=0;
	if(GHW.ROC[c].OffsetCompensation)
	{
		for(int s=GHW.ROX.RecordOriginOnScale;s<GHW.GH->NumRecords;s++)
		{
			GHSAMPLE* hcs=HCS((*(GHW.GH)),s); /*if(hcs[0].fMax<GHW.ROX.xMin) continue;*/ if(hcs[0].fMin>GHW.ROX.xMax) break;
			GHW.ROC[c].fMean+=hcs[c].fMean;
			GHW.ROC[c].nMean++;
		}
	}
	return -1;
}


//determine l'échelle Y (attn. il faut regarder une ou plusieurs courbes selon le mode pour déterminer min et max)
int SPG_CONV GHW_ResetYScale(GHISTORYWINDOW& GHW, int c)
{
	double cof=ROCOFC(GHW.ROC[c]);
	GHCURVESCALEGROUP& scg=GHW.SCG[GHW.ROC[c].ScaleGroup];
	int s=GHW.ROX.RecordOriginOnScale;
	GHSAMPLE* hcs=HCS((*(GHW.GH)),s);	// /*if(hcs[0].fMax<GHW.ROX.xMin) continue;*/	 if(hcs[0].fMin>GHW.ROX.xMax) break;
	if((scg.ScaleYMode==GSC_AUTO)||(scg.ScaleYMode==GSC_AUTOZERO)) { scg.fMin=hcs[c].fMin-cof; scg.fMax=hcs[c].fMax-cof; }
	return -1;
}

//determine l'échelle Y pour une courbe donnée
int SPG_CONV GHW_ComputeYScale(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, int c)
{
	GHCURVESCALEGROUP& scg=GHW.SCG[GHW.ROC[c].ScaleGroup];
	double cof=ROCOFC(GHW.ROC[c]);
	for(int s=GHW.ROX.RecordOriginOnScale;s<GHW.GH->NumRecords;s++)
	{
		GHSAMPLE* hcs=HCS((*(GHW.GH)),s); /*if(hcs[0].fMax<GHW.ROX.xMin) continue;*/ if(hcs[0].fMin>GHW.ROX.xMax) break;
		if((scg.ScaleYMode==GSC_AUTO)||(scg.ScaleYMode==GSC_AUTOZERO)) { scg.fMin=V_Min(scg.fMin,(hcs[c].fMin-cof)); scg.fMax=V_Max(scg.fMax,(hcs[c].fMax-cof)); }
	}
	return -1;
}

// ################################################################################################## //

//http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance
/*
def weighted_incremental_variance(dataWeightPairs):
    mean = 0
    S = 0
    sumweight = 0
    for x, weight in dataWeightPairs: # Alternately "for x in zip(data, weight):"
        temp = weight + sumweight
        Q = x - mean
        R = Q * weight / temp
        S = S + sumweight * Q * R
        mean = mean + R
        sumweight = temp
    Variance = S / (sumweight-1)  # if sample is the population, omit -1
    return Variance
*/

void VAREST_Reset(VAREST& V)	//	, double fMinMax)
{
	V.mean=0;
	V.S=0;
	V.sumweight=0;
	//V.fMax=V.fMin=fMinMax;
}

void VAREST_Add(VAREST& V, double x, double weight)
{
	//V.fMin=V_Min(V.fMin,x);		V.fMax=V_Max(V.fMax,x);
	double temp = weight + V.sumweight;
	double Q = x - V.mean;
	double R = Q * weight / temp;
	V.S = V.S + V.sumweight * Q * R;
	V.mean = V.mean + R;
	V.sumweight = temp;
}

double VAREST_Variance(VAREST& V)
{
	if(V.sumweight<=1) return 0;
	return V.S / (V.sumweight-1);
}

// ################################################################################################## //

int SPG_CONV GHW_ResetYScaleEx(GHISTORYWINDOW& GHW, int c)
{
	GHCURVESCALEGROUP& scg=GHW.SCG[GHW.ROC[c].ScaleGroup];
	double cof=ROCOFC(GHW.ROC[c]);
	int s=GHW.ROX.RecordOriginOnScale;	//s<GHW.GH->NumRecords;s++)

	//for(int s=GHW.ROX.RecordOriginOnScale;s<GHW.GH->NumRecords;s++)
	{
		GHSAMPLE* hcs=HCS((*(GHW.GH)),s);	// /*if(hcs[0].fMax<GHW.ROX.xMin) continue;*/	if(hcs[0].fMin>GHW.ROX.xMax) break;
		if((scg.ScaleYMode==GSC_AUTO)||(scg.ScaleYMode==GSC_AUTOZERO)) 
		{ 
			scg.fMin=hcs[c].fMin-cof; 
			scg.fMax=hcs[c].fMax-cof; 
			VAREST_Reset(scg.V); //	,hcs[c].fMin-ROCOFC(GHW.ROC[c]));
			return -1;
		}
	}
	return -1;
}

int SPG_CONV GHW_ComputeYScaleEx(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, int c)
{
	GHCURVESCALEGROUP& scg=GHW.SCG[GHW.ROC[c].ScaleGroup];
	double cof=ROCOFC(GHW.ROC[c]);
	if((scg.ScaleYMode==GSC_AUTO)||(scg.ScaleYMode==GSC_AUTOZERO)) 
	{ 
		for(int s=GHW.ROX.RecordOriginOnScale;s<GHW.GH->NumRecords;s++)
		{
			GHRECORD* hcr=HCR((*(GHW.GH)),s); /*if(hcs[0].fMax<GHW.ROX.xMin) continue;*/ if(hcr->hcs[0].fMin>GHW.ROX.xMax) break;
			double xw=0.1+V_Max((hcr->hcs[0].fMean-GHW.ROX.xMin)/(GHW.ROX.xMax-GHW.ROX.xMin),0);
			scg.fMin=V_Min(scg.fMin,(hcr->hcs[c].fMin-cof)); 
			scg.fMax=V_Max(scg.fMax,(hcr->hcs[c].fMax-cof)); 
			__int64 W=(((__int64)1)<<hcr->hch.Weight);
			VAREST_Add(scg.V,hcr->hcs[c].fMin-cof,xw*(double)W);	VAREST_Add(scg.V,hcr->hcs[c].fMax-cof,xw*(double)W);
		}
	}
	return -1;
}

//une fois les valeur immédiates de min et max calculées on les affecte au graphique
int SPG_CONV GHW_CommitYScaleEx(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, int sc)
{
	GHCURVESCALEGROUP& scg=GHW.SCG[sc];
	if((scg.ScaleYMode==GSC_AUTO)||(scg.ScaleYMode==GSC_AUTOZERO))
	{
		float k=GHW.SCG[sc].k_max;
		double vV=sqrt(VAREST_Variance(scg.V));
		double vMin=scg.V.mean-k*vV;				double vMax=scg.V.mean+k*vV;
		scg.fMin=V_Max(scg.fMin,vMin);				scg.fMax=V_Min(scg.fMax,vMax);
	}
	{
		float k=GHW.SCG[sc].k_min;
		double vV=sqrt(VAREST_Variance(scg.V));
		double vMin=scg.V.mean-k*vV;				double vMax=scg.V.mean+k*vV;
		scg.fMin=V_Min(scg.fMin,vMin);				scg.fMax=V_Max(scg.fMax,vMax);
	}
	CHECK(_finite(scg.fMin)==0,"SC_Update",return 0);
	CHECK(_finite(scg.fMax)==0,"SC_Update",return 0);
	if(scg.ScaleYMode==GSC_AUTO)				{double D=scg.fMax-scg.fMin;		scg.fMin-=2*D/V_Max((GZ.YGB-GZ.YGT-4),4);								scg.fMax+=2*D/V_Max((GZ.YGB-GZ.YGT-4),4);}//evite que la courbe ne se colle aux parois
	if(scg.ScaleYMode==GSC_AUTOZERO)	{														scg.fMin=0; double D=scg.fMax-scg.fMin;		scg.fMax+=2*D/V_Max((GZ.YGB-GZ.YGT-4),4); scg.fMax=V_Max(scg.fMax,scg.LowerLimit);}
	scg.SC.TimeCste=V_Min(2*GHW.ROX.dt,1); SC_Update(scg.SC,scg.fMin,scg.fMax);
	return -1;
}

//dessine l'echelle Y
int SPG_CONV GHW_RenderYScale(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL, int c)
{
	if(CL==0) return 0;
	int sc; SPG_Scale& SC=GHW.SCG[sc=GHW.ROC[c].ScaleGroup].SC;
	for(int tick=0;tick<SC.NumTick;tick++)
	{
		int X=GZ.XGR+1;		int Y=GHWY(GZ,GHW.SCG[sc],SC.T[tick].Value);
		if(V_IsBound(Y,GZ.YGT,GZ.YGB))
		{
			GHW_Print(E,X,Y,SC.T[tick].Label,*CL,YCENTER,GHW.ROC[c].Text.Coul);
			G_DrawLine(E,GZ.XGL,Y,GZ.XGR,Y,0x606060);
		}
	}
	return -1;
}

//dessine la legende des courbes
int SPG_CONV GHW_RenderText(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL, int c)
{
	if(CL)
	{
		char Msg[256];	strcpy(Msg,GHW.ROC[c].Label);
		if(GHW.GH->NumRecords)
		{
				strcat(Msg," = ");	CF_GetString(Msg,HCS((*(GHW.GH)),(GHW.GH->NumRecords-1))[c].fMean,GHW.SCG[GHW.ROC[c].ScaleGroup].NumDigits);
		}
		else
		{
			strcat(Msg," no data");	
		}
		int X=2*CL->SizeX;	int Y=(3+GHW.ROC[c].LegendPosition)*CL->SpaceY;

		GHW_Print(E,X,Y,Msg,*CL,0,GHW.ROC[c].Text.Coul);
	}
	return -1;
}

//dessine les courbes
int SPG_CONV GHW_DrawCurves(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E)
{
	int x=0;	int s=GHW.ROX.RecordOriginOnScale;	GHRECORD* hcr=HCR((*(GHW.GH)),s);
	BYTE* ColumnPtr=E.MECR+E.POCT*GZ.XGL+E.Pitch*GZ.YGT;
	int DY=GZ.YGB-GZ.YGT;
	GHW.ROX.xStep=(GHW.ROX.xMax-GHW.ROX.xMin)/(GZ.XGR-GZ.XGL);

	do
	{
		memcpy(GHW.hcr[x&1],hcr,GHW.GH->szRecord); //copie locale de l'echantillon courant
		//int Weight=1<<GHW.hcr[x&1]->hch.Weight;	//for(int c=0;c<GHW.GH->NumCurves;c++)	{	GHW.hcr[x&1]->hcs[c].fMean*=Weight;	}
		__int64 Weight=1;
		for(;s<GHW.GH->NumRecords;s++)
		{
			hcr=HCR((*(GHW.GH)),s);
			if( hcr->hcs[0].fMean >= GHW.ROX.xMin+(x+1)*GHW.ROX.xStep ) break;	//int w=1<<hcr->hch.Weight;
			for(int c=1;c<GHW.GH->NumCurves;c++)
			{
				GHSAMPLE& ghcs=GHW.hcr[x&1]->hcs[c];	GHSAMPLE& hcs=hcr->hcs[c];
				ghcs.fMin=V_Min(ghcs.fMin,hcs.fMin);	ghcs.fMean+=/* w* */ hcs.fMean;	ghcs.fMax=V_Max(ghcs.fMax,hcs.fMax);
			}
			Weight++;
		}

		for(int c=1;c<GHW.GH->NumCurves;c++)	
		{	
			if(Weight) GHW.hcr[x&1]->hcs[c].fMean/=Weight;
			GHCURVERENDEROPTS& roc = GHW.ROC[c];
			GHCURVESCALEGROUP& scg = GHW.SCG[roc.ScaleGroup];
			GHW.hcr[x&1]->hcs[c].fMin-=ROCOFC(roc);	
			GHW.hcr[x&1]->hcs[c].fMean-=ROCOFC(roc);
			GHW.hcr[x&1]->hcs[c].fMax-=ROCOFC(roc);	
		}

		for(int c=GHW.GH->NumCurves-1;c>=1;c--)
		{
			GHSAMPLE& ghcs=GHW.hcr[x&1]->hcs[c];
			GHCURVERENDEROPTS& roc = GHW.ROC[c];
			GHCURVESCALEGROUP& scg = GHW.SCG[roc.ScaleGroup];
			double invfY=1.0/(scg.SC.fMax-scg.SC.fMin);
			int yMin=GHWYR(GZ,scg,ghcs.fMin);	int yMean=GHWYR(GZ,scg,ghcs.fMean);	int pyMean=yMean;
			if(x>0) pyMean=GHWYR(GZ,scg,GHW.hcr[(x&1)^1]->hcs[c].fMean);
			if(pyMean>yMean) V_SWAP(int,pyMean,yMean);
			int yMax=GHWYR(GZ,scg,ghcs.fMax);
			int y=V_Min(yMax,pyMean);	y=V_Max(y,0);
			BYTE* LinePtr=ColumnPtr+y*E.Pitch;
			for(;y<pyMean;y++)	{	if(y>=DY) break;	*(DWORD*)LinePtr=(((roc.Max.Coul&0xFEFEFE)+(*(DWORD*)LinePtr&0xFEFEFE))>>1); LinePtr+=E.Pitch;	}
			for(;y<=(yMean+(roc.DrawThick?1:0));y++)	{	if(y>=DY) break;	*(DWORD*)LinePtr=roc.Mean.Coul; if(roc.DrawThick) *(DWORD*)(LinePtr-E.POCT)=roc.Mean.Coul; LinePtr+=E.Pitch;  }
			for(;y<yMin;y++)		{	if(y>=DY) break;	*(DWORD*)LinePtr=(((roc.Min.Coul&0xFEFEFE)+(*(DWORD*)LinePtr&0xFEFEFE))>>1); LinePtr+=E.Pitch;}
		}
		x++;
		ColumnPtr+=E.POCT;
	} while(x<(GZ.XGR-GZ.XGL));
	
	return -1;
}

//calcule l'ensemble des echelles X et Y
int SPG_CONV GHW_ComputeScales(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL, bool HasFocus)
{
	if(GHW.GH->NumRecords==0) return 0;

	if(CL) 
	{
		GZ.XGL=(CL->SizeX+1)/2;			GZ.XGR=G_SizeX(E);
		GZ.YGT=(CL->SizeX+1)/2;			GZ.YGB=G_SizeY(E);
		GZ.XGR-=7*CL->SizeX+3;	GZ.YGB-=CL->SpaceY+2;	
	}
	else
	{
		GZ.XGL=0;			GZ.XGR=G_SizeX(E)-1;
		GZ.YGT=0;			GZ.YGB=G_SizeY(E)-1;
	}

																				{	GHW.ROX.SC.Divisions=V_Max(3,(GZ.XGR-GZ.XGL)/(7*V_Max(CL->SizeX,4)));	}
	for(int c=0;c<GHW.GH->MaxCurves;c++)		{	GHW.SCG[c].SC.Divisions=V_Max(3,(GZ.YGB-GZ.YGT)/(3*V_Max(CL->SpaceY,4)));	}

	GHW_ComputeXScale(GHW,HasFocus);
	for(int c=1;c<GHW.GH->NumCurves;c++)		{	GHW_FindOrigin(GHW,c);	}
	for(int c=1;c<GHW.GH->NumCurves;c++)		{	GHW_ComputeOffset(GHW,c);	}
	for(int c=1;c<GHW.GH->NumCurves;c++)		{	GHW_ResetYScaleEx(GHW,c);	}
	for(int c=1;c<GHW.GH->NumCurves;c++)		{	GHW_ComputeYScaleEx(GHW,GZ,c);	}
	for(int sc=0;sc<GHW.GH->MaxCurves;sc++)	
	{	
		GHW_CommitYScaleEx(GHW,GZ,sc);
	}
	return -1;
}

//dessine l'ensemble des courbes puis les labels
int SPG_CONV GHW_RenderCurves(GHISTORYWINDOW& GHW, GRENDERZONE& GZ, G_Ecran& E, C_Lib* CL)
{
	if(GHW.GH->NumRecords) GHW_DrawCurves(GHW,GZ,E);
	for(int c=0;c<GHW.GH->NumCurves;c++)	{	GHW_RenderText(GHW,GZ,E,CL,c);	}
	return -1;
}

//dessine l'ensemble de la fenetre : calcule des echelles X et Y,  dessin des echelles, dessin des courbes avec les labels
int SPG_CONV GHW_Render(GHISTORYWINDOW& GHW, G_Ecran& E, C_Lib* CL, bool HasFocus)
{
	if(GHW.GH->NumRecords==0) return 0;
	LTG_EnterEH(GHW,LT_GHW,0);
	GRENDERZONE GZ;	GHW_ComputeScales(GHW,GZ,E,CL,HasFocus);
	GHW_RenderXScale(GHW,GZ,E,CL);		GHW_RenderYScale(GHW,GZ,E,CL,1);
	G_DrawOutRect(E,GZ.XGL,GZ.YGT,GZ.XGR,GZ.YGB,0);
	GHW_RenderCurves(GHW,GZ,E,CL);
	LTG_ExitEHNZ(GHW,LT_GHW,0);
	//return 0;
}

#endif


