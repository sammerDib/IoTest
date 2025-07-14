

#include "..\SrcC\SPG_General.h"

#ifdef SPG_General_USECutView

#include "..\SrcC\SPG_Includes.h"

#include <memory.h>
#include <string.h>
#include <stdio.h>

#define CutC CW.CD[CW.a].C

int SPG_CONV CView_Init(CUTVIEW& CW, G_Ecran& E, B_Lib* BL, C_Lib* CL, int PosX, int PosY, int SizeX, int SizeY, int MultiScale)
{								 
	memset(&CW,0,sizeof(CUTVIEW));

	CW.BL=BL;
	CW.CL=CL;
	CW.n=0;
	CW.a=-1;
	CW.MultiScale=MultiScale;

	int CONTROLWIDTH = 6*(CW.BL->SizeX[1]?CW.BL->SizeX[1]:16);//96
	int SCROLLHEIGHT = 3*(CW.BL->SizeX[1]?CW.BL->SizeX[1]:16);
	int SCROLLPX = CW.BL->SizeX[1];
	int SCROLLPY = CW.BL->SizeX[1];

	G_InitSousEcran(CW.ECurve,E,PosX,PosY,SizeX-CONTROLWIDTH,SizeY-SCROLLHEIGHT);
	G_InitSousEcran(CW.EControl,E,PosX+SizeX-CONTROLWIDTH,PosY,CONTROLWIDTH,SizeY);
	G_InitSousEcran(CW.EScroll,E,PosX,PosY+SizeY-SCROLLHEIGHT,SizeX-CONTROLWIDTH,SCROLLHEIGHT);
	//B_RedrawButtonsLib(*CW.BL,1);

	G_DrawRect(E,PosX,PosY,PosX+SizeX,PosY+SizeY,0x907080);

	CW.bHScroll=B_CreateHReglageCliquableNumeric(*CW.BL,CW.EScroll,*CW.CL,SCROLLPX,SCROLLPY,CW.EScroll.SizeX-8-SCROLLPX,4,&CW.CP.X,0.01,0,100);

	CW.bExit=B_CreateClickButton(*CW.BL,CW.EControl,3*SCROLLPX,SCROLLPY);

	CW.bLoad=B_CreateClickButton(*CW.BL,CW.EControl,3*SCROLLPX,3*SCROLLPY);

	CW.bSave=B_CreateClickButton(*CW.BL,CW.EControl,3*SCROLLPX,5*SCROLLPY);

	CW.bUnload=B_CreateClickButton(*CW.BL,CW.EControl,3*SCROLLPX,7*SCROLLPY);

	
	{for(int i=0;i<CUTVIEWMAXCUT;i++)
	{
		int PosY=9*SCROLLPY+2*i*SCROLLPY;
		if(V_IsBound(PosY,0,G_SizeY(CW.EControl)-32))
		{
			CW.CD[i].bSelected=B_CreateCheckButton(*CW.BL,CW.EControl,3*SCROLLPX,PosY);
		}
	}}
	

	CW.bVScroll=B_CreateVReglageCliquableNumeric(*CW.BL,CW.EControl,*CW.CL,SCROLLPX,SCROLLPY,CW.EControl.SizeY-32-SCROLLPY,3,&CW.CP.Z,0.2,0,100);

	B_RedrawButtonsLib(*CW.BL,0);
	{for(int i=0;i<5;i++)
	{
		G_Soften3x3(E);
		B_RedrawButtonsLib(*CW.BL,0);
		G_Soften3x3_Reversed(E);
		B_RedrawButtonsLib(*CW.BL,0);
	}}
	B_PrintLabel(*CW.BL,CW.bExit,*CW.CL,"Exit");
	B_PrintLabel(*CW.BL,CW.bLoad,*CW.CL,"Load");
	B_PrintLabel(*CW.BL,CW.bSave,*CW.CL,"Save");
	B_PrintLabel(*CW.BL,CW.bUnload,*CW.CL,"Unload");
	{for(int i=0;i<CUTVIEWMAXCUT;i++)
	{
		if(CW.CD[i].bSelected>0)
		{
			char Msg[256];
			sprintf(Msg,"Select %i",i);
			B_PrintLabel(*CW.BL,CW.CD[i].bSelected,*CW.CL,Msg);
		}
	}}

	return CW.Etat=-1;
}

void SPG_CONV CView_ReadSelectedButtons(CUTVIEW& CW)
{
	for(int i=0;i<CUTVIEWMAXCUT;i++)
	{
		if(B_IsClick((*CW.BL),CW.CD[i].bSelected)) {CW.a=i; return;}
	}
	return;
}

void SPG_CONV CView_WriteSelectedButtons(CUTVIEW& CW)
{
	for(int i=0;i<CUTVIEWMAXCUT;i++)
	{
		B_SetAndRedraw((*CW.BL),CW.CD[i].bSelected,(i==CW.a)?B_Click:B_Normal);
	}
}

void SPG_CONV CView_InitParams(CUTVIEW& CW, CUTVIEWPARAMS& CP)
{
	CHECK(CW.Etat!=-1,"CView_InitParams",return);
	CHECK(CW.a<0,"CView_InitParams: No active curve",return);
	CP.DrawStart=0;
	CP.DrawLen=V_Max(CP.DrawLen,CutC.NumS);
	float tmp;
	CW.CD[CW.a].CursPosX[0]=Cut_FindMax(CutC,tmp); CW.CD[CW.a].CursPosD[0]=1;
	CW.CD[CW.a].CursPosX[1]=Cut_FindMin(CutC,tmp); CW.CD[CW.a].CursPosD[1]=1;
	for(int i=2;i<CUTVIEWCURSORS;i++)
	{
		CW.CD[CW.a].CursPosX[i]=0;
		CW.CD[CW.a].CursPosD[i]=0;
	}
	return;
}

void SPG_CONV CView_Unload(CUTVIEW& CW)
{
	CHECK(CW.Etat!=-1,"CView_Unload",return);
	CHECK(CW.a<0,"CView_Unload: No active curve",return);
	Cut_Close(CutC);
	for(int i=CW.a;i<CW.n-1;i++)
	{
		CW.CD[i]=CW.CD[i+1];
	}
	CW.n--;
	if(CW.a>CW.n-1) CW.a=CW.n-1;
	return;
}

void SPG_CONV CView_Unload(CUTVIEW& CW, Cut& C)
{
	CHECK(CW.Etat!=-1,"CView_Unload",return);
	{
		int a;
		for(a=0;a<CW.n;a++)
		{
			if(CW.CD[a].C.D==C.D) break;
		}
		CHECK(a>=CW.n,"CView_Unload: Reference not found",return);
		CW.a=a;
	}
	Cut_Close(CutC);
	for(int i=CW.a;i<CW.n-1;i++)
	{
		CW.CD[i]=CW.CD[i+1];
	}
	CW.n--;
	if(CW.a>CW.n-1) CW.a=CW.n-1;
	return;
}

void SPG_CONV CView_Load(CUTVIEW& CW, char* SuggestedName)
{
	CHECK(CW.Etat!=-1,"CView_Load",return);
	CW.a=CW.n;
	Cut_Load(CutC,SuggestedName);
	CView_InitParams(CW,CW.CP);
	sprintf(CW.CD[CW.a].Label,"Cut%i",CW.a);
	CW.n++;
	return;
}

void SPG_CONV CView_Load(CUTVIEW& CW, Cut& C, const char* Label)
{
	CHECK(CW.Etat!=-1,"CView_Load",return);
	CW.a=CW.n;
	strncpy(CW.CD[CW.a].Label,Label,CUTVIEWLABEL-1);
	Cut_Init(CutC,Cut_NumS(C),C.D,C.Msk,C.Decor,Cut_Alias);
	CView_InitParams(CW,CW.CP);
	CW.n++;
	return;
}

void SPG_CONV CView_Save(CUTVIEW& CW)
{
	CHECK(CW.Etat!=-1,"CView_Save",return);
	CHECK(CW.a<0,"CView_Save: No active curve",return);
	SPG_SetExtens(CW.CD[CW.a].Label,CUTVIEWEXTENSIONSTR);
	Cut_Save(CutC,CW.CD[CW.a].Label);
	return;
}

void SPG_CONV CView_Close(CUTVIEW& CW)
{
	CHECK(CW.Etat!=-1,"CView_Close",return);
	while(CW.a>=0) {CView_Unload(CW);};
	G_CloseEcran(CW.ECurve);
	G_CloseEcran(CW.EControl);
	G_CloseEcran(CW.EScroll);
	memset(&CW,0,sizeof(CUTVIEW));
	return;
}

void SPG_CONV CView_Draw(CUTVIEW& CW,int WithMsk)
{
	CHECK(CW.Etat!=-1,"CView_Draw",return);
	//CHECK(CW.a<0,"CView_Draw: No active curve",return);
	if(CW.a<0) return;
	//CView_WriteSelectedButtons(CW);

	int NumS=0;
	{for(int i=0;i<CW.n;i++)
	{
		NumS=V_Max(NumS,CW.CD[i].C.NumS);
	}}
	//CW.CP.DrawLen=V_Sature(10*NumS/(10+CW.CP.Z*CW.CP.Z),1,NumS);
	CW.CP.DrawLen=V_Sature(NumS*(100-CW.CP.Z)/100*(100-CW.CP.Z)/100,2,NumS);
	CW.CP.DrawStart=V_Sature(NumS*CW.CP.X/100-CW.CP.DrawLen/2,0,NumS-CW.CP.DrawLen);

	int d=0;
	SPG_ArrayStackAlloc(Cut*,Index,CUTVIEWMAXCUT);
	SPG_ArrayStackAlloc(DWORD,Color,CUTVIEWMAXCUT);
	{for(int a=0;a<CW.n;a++)
	{
		//CHECK(!V_IsBound(CW.CP.DrawStart,0,CW.CD[a].C.NumS),"CView_Draw",continue);
		if(!V_IsBound(CW.CP.DrawStart,0,CW.CD[a].C.NumS)) continue;
		//CHECK(!V_InclusiveBound((CW.CP.DrawStart+CW.CP.DrawLen),CW.CP.DrawStart,CW.CD[a].C.NumS),"CView_Draw",continue);
		if(!V_InclusiveBound((CW.CP.DrawStart+CW.CP.DrawLen),CW.CP.DrawStart,CW.CD[a].C.NumS)) continue;
		
		Cut_Init(CW.CP.CDraw[d],CW.CP.DrawLen,
			CW.CD[a].C.D+CW.CP.DrawStart,
			(CW.CD[a].C.Msk&&WithMsk)?(CW.CD[a].C.Msk+CW.CP.DrawStart):0,
			(CW.CD[a].C.Decor&&WithMsk)?(CW.CD[a].C.Decor+CW.CP.DrawStart):0,
			Cut_Alias);
		Index[d]=CW.CP.CDraw+d;
		Color[d]=((d&1)?0x000000FF:0) + ((d&2)?0x0000FF00:0) + ((d&4)?0x00FF0000:0);
		d++;
	}}

	SPG_ArrayStackCheck(Index);
	SPG_ArrayStackCheck(Color);
	
	Cut_DrawList(Index,CW.ECurve,Color,*CW.CL,d,CW.MultiScale);

	{for(int i=0;i<d;i++)
	{
		Cut_Close(CW.CP.CDraw[i]);
	}}

	char Msg[256];
	sprintf(Msg,"Start:%i Len:%i",CW.CP.DrawStart,CW.CP.DrawLen);
	C_Print(CW.ECurve,128,32,Msg,*CW.CL);
	return;
}

int SPG_CONV CView_Update(CUTVIEW& CW, int MouseX, int MouseY, int MouseLeft, bool bForced)
{
	CHECK(CW.Etat!=-1,"CView_InitParams",return 0);
	B_UpdateButtonsLib(*CW.BL,MouseX,MouseY,MouseLeft);

	int n;
	{
		for(n=1;n<MaxButton;n++)
		{
			if ((*CW.BL).B[n].Mode==0) break;
			if(B_IsChanged((*CW.BL),n)) {n=-1;break;}
		}
	}
	CView_ReadSelectedButtons(CW);

	if(B_IsChangedToClick((*CW.BL),CW.bExit))
	{
		return -1;
	}
	if(B_IsChangedToClick((*CW.BL),CW.bLoad))
	{
		CView_Load(CW,"*" CUTVIEWEXTENSIONSTR );
	}
	if(B_IsChangedToClick((*CW.BL),CW.bSave))
	{
		CView_Save(CW);
	}
	if(B_IsChangedToClick((*CW.BL),CW.bUnload))
	{
		CView_Unload(CW);
	}

	if((n>=1)||(bForced!=0)) CView_Draw(CW,1);

	return -1; // bien renvoyer -1 si bForced
}

#endif
