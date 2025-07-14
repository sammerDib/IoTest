

#include "SPG_General.h"

#ifdef SPG_General_USEButtons

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

//librairie de boutons
#include <stdio.h>
#include <string.h>


int SPG_CONV B_LoadButtonsLib(B_Lib& LB, G_Ecran& E, int ClearScreen, int AutoUpdate, const char * WorkDir,const char * S)
{
	DbgCHECK(!SPG_GLOBAL_ETAT(OK),"B_LoadButtonsLib: Call SPG_Initialise first");

	char FullName[1024];

	memset(&LB,0,sizeof(B_Lib));

	if(WorkDir) 
		SPG_ConcatPath(FullName,Global.ProgDir,WorkDir);
	else
		strcpy(FullName,Global.ProgDir);
	SPG_ConcatPath(FullName,FullName,S);
	//lire le nom du fichier contenant les images des boutons
	//lire toutes les tailles de boutons, initialiser
	//leurs pointeurs de texture
	CHECK(E.Etat==0,"B_LoadButtonsLib: Ecran nul",return 0);
	CHECKTWO(G_InitEcranFromFile(LB.EIMG,E.POCT,0,FullName)==0,"B_LoadButtonsLib: Chargement echoue sur le fichier",FullName,return 0);
	SPG_SetMemName(LB.EIMG.MECR,"ButtonsLib");

	LB.ThreadId=GetCurrentThreadId();

	LB.Pitch=LB.EIMG.Pitch;

	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*0],LB.BC.Black.Coul);	  LB.BC.Black.A=G_NORMAL_ALPHA;
	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*1],LB.BC.White.Coul);	  LB.BC.White.A=G_NORMAL_ALPHA;
	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*2],LB.BC.BlueColor.Coul);  LB.BC.BlueColor.A=G_NORMAL_ALPHA;
	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*3],LB.BC.GreenColor.Coul); LB.BC.GreenColor.A=G_NORMAL_ALPHA;
	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*4],LB.BC.RedColor.Coul);   LB.BC.RedColor.A=G_NORMAL_ALPHA;
	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*5],LB.BC.BackGround.Coul); LB.BC.BackGround.A=G_NORMAL_ALPHA;
	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*6],LB.BC.ForeGround.Coul); LB.BC.ForeGround.A=G_NORMAL_ALPHA;
	G_Make24(LB.EIMG,LB.EIMG.MECR[LB.EIMG.POCT*7],LB.BC.FillColor.Coul);  LB.BC.FillColor.A=G_NORMAL_ALPHA;

	if (ClearScreen) G_DrawRect(E,0,0,E.SizeX,E.SizeY,LB.BC.BackGround.Coul);

	int BSX=LB.EIMG.SizeX/2;
	int BSY=LB.EIMG.SizeX/2;

	LB.SizeX[0]=1;
	LB.SizeY[0]=1;
	LB.TP[0]=LB.EIMG.MECR;

	LB.SizeX[ClickButton]=BSX;
	LB.SizeY[ClickButton]=BSY;
	LB.TP[ClickButton]=PixEcrPTR(LB.EIMG,0,BSY*ClickSprite);

	LB.SizeX[CheckButton]=BSX;
	LB.SizeY[CheckButton]=BSY;
	LB.TP[CheckButton]=PixEcrPTR(LB.EIMG,0,BSY*CheckSprite);

	LB.SizeX[CheckIntButton]=BSX;
	LB.SizeY[CheckIntButton]=BSY;
	LB.TP[CheckIntButton]=PixEcrPTR(LB.EIMG,0,BSY*CheckIntSprite);

	LB.SizeX[AutoINCButton]=BSX;
	LB.SizeY[AutoINCButton]=BSY;
	LB.TP[AutoINCButton]=PixEcrPTR(LB.EIMG,0,BSY*AutoINCSprite);

	LB.SizeX[AutoDECButton]=BSX;
	LB.SizeY[AutoDECButton]=BSY;
	LB.TP[AutoDECButton]=PixEcrPTR(LB.EIMG,0,BSY*AutoDECSprite);

	LB.SizeX[AutoMultiINCButton]=BSX;
	LB.SizeY[AutoMultiINCButton]=BSY;
	LB.TP[AutoMultiINCButton]=PixEcrPTR(LB.EIMG,0,BSY*AutoMultiINCSprite);

	LB.SizeX[AutoMultiDECButton]=BSX;
	LB.SizeY[AutoMultiDECButton]=BSY;
	LB.TP[AutoMultiDECButton]=PixEcrPTR(LB.EIMG,0,BSY*AutoMultiDECSprite);

	LB.SizeX[ReglageHButton]=BSX;
	LB.SizeY[ReglageHButton]=BSY;
	LB.TP[ReglageHButton]=PixEcrPTR(LB.EIMG,0,BSY*ReglageHButton);

	LB.SizeX[ReglageVButton]=BSX;
	LB.SizeY[ReglageVButton]=BSY;
	LB.TP[ReglageVButton]=PixEcrPTR(LB.EIMG,0,BSY*ReglageVButton);

	//LB.ButtonList=(B_Generic*)SPG_MemAlloc(sizeof(B_Generic*),"Boutons list")

	LB.Etat=BL_OK;
	LB.FocusButton=0;

	if (AutoUpdate) 
	{
		LB.Etat|=BL_AUTOUPDATE;
		SPG_AddUpdateOnDoEvents((SPG_CALLBACK)B_UpdateButtonsLibFromDoEvents,&LB,1);
	}

	CD_G_CHECK_EXIT(29,14);

	return -1;
}

void SPG_CONV B_CloseButtonsLib(B_Lib& LB)
{
	if (LB.Etat&BL_AUTOUPDATE) SPG_KillUpdateOnDoEvents((SPG_CALLBACK)B_UpdateButtonsLibFromDoEvents,&LB);
	int i;
	for(i=0;i<MaxButton;i++)
	{
		if (LB.B[i].Mode) B_CloseButton(LB.B[i]);
	}

	G_CloseEcran(LB.EIMG);
	LB.Etat=0;
	LB.FocusButton=0;
	return;
}

void SPG_CONV B_UpdateButtonsLib(B_Lib& LB, int MouseX, int MouseY, int MouseState)
{
	CHECK(LB.Etat==0,"Librairie de boutons vide",return);
	//attention lorsque LB.FocusButton=-1!!!!
	if ((LB.FocusButton>0)&&(LB.B[LB.FocusButton].Mode))//&&MouseState&&(LB.B[LB.FocusButton].Mode!=AutoMultiINCButton)&&(LB.B[LB.FocusButton].Mode!=AutoMultiDECButton))
	{
		int ShouldFocus=1;
		B_UpdateButton(LB.B[LB.FocusButton],MouseX,MouseY,MouseState,ShouldFocus,LB.ThreadId);
		if(ShouldFocus==0) LB.FocusButton=0;
	for(int i=1;i<MaxButton;i++)
	{
		int dummyShouldFocus=0;
		if ((LB.B[i].Mode!=0)&&(i!=LB.FocusButton)) B_UpdateButton(LB.B[i],MouseX,MouseY,0,dummyShouldFocus,LB.ThreadId);
	}
	}
	else
	{
	LB.FocusButton=0;
	/*
	int i;
	for(i=0;i<MaxButton;i++)
	{
		int ShouldFocus=0;
		if (LB.B[i].Mode==CliquableNumericButton) B_UpdateButton(LB.B[i],MouseX,MouseY,MouseState,ShouldFocus);
		if ((ShouldFocus)&&(LB.FocusButton==0)) LB.FocusButton=i;
	}
	*/
	/*
	if(LB.FocusButton==0)
	{
	*/
	int i;
	for(i=1;i<MaxButton;i++)
	{//traitement prioritaire des cliquablenumericbutton
		int ShouldFocus=0;
		if ((LB.B[i].Mode==CliquableNumericButton)||(LB.B[i].Mode==CliquableNumericIntButton)) B_UpdateButton(LB.B[i],MouseX,MouseY,MouseState,ShouldFocus,LB.ThreadId);
		if ((ShouldFocus)&&(LB.FocusButton==0)) {LB.FocusButton=i;break;}
	}
	if(i<MaxButton)
	{
		for(;i<MaxButton;i++)
		{
			int ShouldFocus=0;
			if ((LB.B[i].Mode!=0)&&(i!=LB.FocusButton)) B_UpdateButton(LB.B[i],MouseX,MouseY,0,ShouldFocus,LB.ThreadId);
		}
	}
	else
	{//traitement secondaire
		for(i=1;i<MaxButton;i++)
		{
			int ShouldFocus=0;
			if (LB.B[i].Mode!=0) B_UpdateButton(LB.B[i],MouseX,MouseY,MouseState,ShouldFocus,LB.ThreadId);
			if ((ShouldFocus)&&(LB.FocusButton==0)) {LB.FocusButton=i;break;}
		}
		for(i=i+1;i<MaxButton;i++)
		{
			int ShouldFocus=0;
			if ((LB.B[i].Mode!=0)&&(i!=LB.FocusButton)) B_UpdateButton(LB.B[i],MouseX,MouseY,0,ShouldFocus,LB.ThreadId);
		}
	}
	}
	CD_G_CHECK_EXIT(7,21);
	/*
	}
	*/
	return;
}


void SPG_CONV B_UpdateButtonsLibFromDoEvents(B_Lib& LB)
{
	CD_G_CHECK_EXIT(4,19);
	CHECK(LB.ThreadId!=GetCurrentThreadId(),"B_UpdateButtonsLibFromDoEvents",return);
	B_UpdateButtonsLib(LB, SPG_Global_MouseX, SPG_Global_MouseY, SPG_Global_MouseLeft);
	return;
}


void SPG_CONV B_RedrawButtonsLib(B_Lib& LB, int EraseBackGround)
{
	CHECK(LB.Etat==0,"B_RedrawButtonsLib: B_Lib nulle",return);
	int i;

	if(EraseBackGround)
	{
	G_Ecran*E=0;
	for(i=1;i<MaxButton;i++)
	{
		if (LB.B[i].Mode) 
		{
			if ((E!=LB.B[i].E)&&(LB.B[i].E!=0))
			{
				E=LB.B[i].E;
				G_DrawRect(*E,0,0,E->SizeX,E->SizeY,LB.BC.BackGround.Coul);
			}
		}
	}
	}
	IF_CD_G_CHECK(15,return);
	for(i=1;i<MaxButton;i++)
	{
		if (LB.B[i].Mode) B_DrawButton(LB.B[i]);
	}
	return;
}

int SPG_CONV B_CreateButton(B_Lib &LB, G_Ecran* E, C_Lib* CL, int PosX,int PosY, int MODE, int NumDigits, void * UntypedToChange, DWORD UntypedIncrement, float ValMin, float ValMax)
{
	if (LB.Etat==0) return 0;

	CHECK(E==0,"B_CreateButton: Ecran invalide",return 0);
	CHECK(!(*E).Etat,"B_CreateButton: Ecran invalide",return 0);
	CHECK(!V_IsBound(PosX,0,(*E).SizeX),"B_CreateButton: Mauvais placement X",return 0);
	CHECK(!V_IsBound(PosY,0,(*E).SizeY),"B_CreateButton: Mauvais placement Y",return 0);
	CHECK(!V_InclusiveBound(PosX+LB.SizeX[MODE],0,(*E).SizeX),"B_CreateButton: Mauvais placement X",return 0);
	CHECK(!V_InclusiveBound(PosY+LB.SizeY[MODE],0,(*E).SizeY),"B_CreateButton: Mauvais placement Y",return 0);

	int i;
	for(i=1;i<MaxButton;i++)
	{
		if (LB.B[i].Mode==0) break;
	}

	CHECK(i==MaxButton,"Trop de boutons",return 0)

	LB.B[i].Mode=MODE;
	LB.B[i].E=E;
	LB.B[i].CL=CL;

	LB.B[i].PosX=PosX;
	LB.B[i].PosY=PosY;

#ifdef SPG_General_USECarac
	if (MODE==NumericButton)
	{
		LB.B[i].SizeX=(NumDigits+4)*((*CL).SizeX)+2;
		LB.B[i].SizeY=((*CL).SizeY)+4;
	}
	else if (MODE==NumericIntButton)
	{
		LB.B[i].SizeX=(NumDigits+1)*((*CL).SizeX)+2;
		LB.B[i].SizeY=((*CL).SizeY)+4;
	}
	else 
#endif
	{
		LB.B[i].SizeX=LB.SizeX[MODE];
		LB.B[i].SizeY=LB.SizeY[MODE];
	}

	LB.B[i].Pitch=LB.Pitch;
	LB.B[i].TP=LB.TP[MODE];

	LB.B[i].Etat=0;

	LB.B[i].CL=CL;
	LB.B[i].NumDigits=NumDigits;

	LB.B[i].UntypedToChange=UntypedToChange;

//if (MODE==NumericIntButton) VToChange=0;

	if (UntypedToChange) 
		LB.B[i].OldUntyped=*(DWORD*)UntypedToChange;
	else
		LB.B[i].OldV=0;

	LB.B[i].UntypedIncrement=UntypedIncrement;
	LB.B[i].ValMin=ValMin;
	LB.B[i].ValMax=ValMax;

	LB.B[i].BC=LB.BC;

	//B_DrawButton(LB,LB.B[i]);

	return i;
}

int SPG_CONV B_CreateClickButton(B_Lib &LB, G_Ecran& E, int PosX,int PosY)
{
	return B_CreateButton(LB,&E,0,PosX,PosY,ClickButton,0,0,0,0,0);
}

int SPG_CONV B_CreateCheckButton(B_Lib &LB, G_Ecran& E, int PosX,int PosY)
{
	return B_CreateButton(LB,&E,0,PosX,PosY,CheckButton,0,0,0,0,0);
}

int SPG_CONV B_CreateCheckIntButton(B_Lib &LB, G_Ecran& E, int PosX,int PosY, int Flag, int * VToChange)
{
	return B_CreateButton(LB,&E,0,PosX,PosY,CheckIntButton,0,VToChange,Flag,0,0);
}

int SPG_CONV B_CreateNumericButton(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX,int PosY, int NumDigits, float * VToChange)
{
	return B_CreateButton(LB,&E,&CL,PosX,PosY,NumericButton,NumDigits,VToChange,0,0,0);
}

int SPG_CONV B_CreateVectorButton(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX,int PosY, int NumDigits, V_VECT* V)
{
	int i=B_CreateButton(LB,&E,&CL,PosX,PosY,NumericButton,NumDigits,&(V->x),0,0,0);
	i=B_CreateButton(LB,&E,&CL,PosX+=LB.B[i].SizeX+1,PosY,NumericButton,NumDigits,&(V->y),0,0,0);
	return B_CreateButton(LB,&E,&CL,PosX+LB.B[i].SizeX+1,PosY,NumericButton,NumDigits,&(V->z),0,0,0);
}

int SPG_CONV B_CreateNumericIntButton(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX,int PosY, int NumDigits, int * VToChange)
{
	return B_CreateButton(LB,&E,&CL,PosX,PosY,NumericIntButton,NumDigits,VToChange,0,0,0);
}

int SPG_CONV B_CreateCliquableNumericButton(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX,int PosY, int NumDigits, float * VToChange)
{
	int J=B_CreateButton(LB,&E,&CL,PosX,PosY,NumericButton,NumDigits,VToChange,0,0,0);
	LB.B[J].Mode=CliquableNumericButton;
	return J;
}

int SPG_CONV B_CreateCliquableNumericIntButton(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX,int PosY, int NumDigits, int * VToChange)
{
	int J=B_CreateButton(LB,&E,&CL,PosX,PosY,NumericIntButton,NumDigits,VToChange,0,0,0);
	LB.B[J].Mode=CliquableNumericIntButton;
	return J;
}

int SPG_CONV B_CreateHReglage(B_Lib &LB, G_Ecran& E, int PosX,int PosY, int SizeX, float * VToChange, float Increment, float VMin, float VMax)
{
	int G=B_CreateButton(LB,
		&E,0,PosX,PosY,
		AutoMultiDECButton,0,VToChange,*(DWORD*)&Increment,VMin,VMax);
	int D=B_CreateButton(LB,
		&E,0,PosX+SizeX-LB.SizeX[AutoINCSprite],PosY,
		AutoMultiINCButton,0,VToChange,*(DWORD*)&Increment,VMin,VMax);
	int J=B_CreateButton(LB,
		&E,0,PosX+LB.B[G].SizeX+2,PosY,
		ReglageHButton,0,VToChange,*(DWORD*)&Increment,VMin,VMax);

	LB.B[J].SizeX=SizeX-LB.B[G].SizeX-LB.B[D].SizeX-4;

	return J;
}

int SPG_CONV B_CreateHReglageNumeric(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX, int PosY, int SizeX, int NumDigits, float * VToChange, float Increment, float VMin, float VMax)
{
	int J;
	if((J=B_CreateHReglage(LB,E,PosX,PosY,SizeX,VToChange,Increment,VMin,VMax))==0) return 0;
	//J=B_CreateNumericButton(LB,E,CL,LB.B[J].PosX+(LB.B[J].SizeX>>1),LB.B[J].PosY+LB.B[J].SizeY+1,NumDigits,VToChange);
	J=B_CreateNumericButton(LB,E,CL,LB.B[J].PosX+(LB.B[J].SizeX>>1),LB.B[J].PosY+(LB.B[J].SizeY>>1),NumDigits,VToChange);
	LB.B[J].PosX=V_Max(0,
	(LB.B[J].PosX-(LB.B[J].SizeX>>1))
	);
	LB.B[J].PosY=V_Max(0,
	(LB.B[J].PosY-((LB.B[J].SizeY+1)>>1))
	);
	return J;
}

int SPG_CONV B_CreateHReglageCliquableNumeric(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX, int PosY, int SizeX, int NumDigits, float * VToChange, float Increment, float VMin, float VMax)
{
	int J;
	if((J=B_CreateHReglage(LB,E,PosX,PosY,SizeX,VToChange,Increment,VMin,VMax))==0) return 0;
	//J=B_CreateNumericButton(LB,E,CL,LB.B[J].PosX+(LB.B[J].SizeX>>1),LB.B[J].PosY+LB.B[J].SizeY+1,NumDigits,VToChange);
	J=B_CreateCliquableNumericButton(LB,E,CL,LB.B[J].PosX+(LB.B[J].SizeX>>1),LB.B[J].PosY+(LB.B[J].SizeY>>1),NumDigits,VToChange);
	LB.B[J].PosX=V_Max(0,
	(LB.B[J].PosX-(LB.B[J].SizeX>>1))
	);
	LB.B[J].PosY=V_Max(0,
	(LB.B[J].PosY-((LB.B[J].SizeY+1)>>1))
	);
	return J;
}

int SPG_CONV B_CreateVReglage(B_Lib &LB, G_Ecran& E, int PosX,int PosY, int SizeY, float * VToChange, float Increment, float VMin, float VMax)
{
	int H=B_CreateButton(LB,
		&E,0,PosX,PosY,
		AutoMultiINCButton,0,VToChange,*(DWORD*)&Increment,VMin,VMax);
	int B=B_CreateButton(LB,
		&E,0,PosX,PosY+SizeY-LB.SizeY[AutoINCSprite],
		AutoMultiDECButton,0,VToChange,*(DWORD*)&Increment,VMin,VMax);
	int J=B_CreateButton(LB,
		&E,0,PosX,PosY+LB.B[H].SizeY+2,
		ReglageVButton,0,VToChange,*(DWORD*)&Increment,VMin,VMax);

	LB.B[J].SizeY=SizeY-LB.B[H].SizeY-LB.B[B].SizeY-4;

	return J;
}

int SPG_CONV B_CreateVReglageNumeric(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX,int PosY, int SizeY, int NumDigits, float * VToChange, float Increment, float VMin, float VMax)
{
	int J;
	//if((J=B_CreateVReglage(LB,E,PosX,PosY+CL.SizeY+4,SizeY,VToChange,Increment,VMin,VMax))==0) return 0;
	if((J=B_CreateVReglage(LB,E,PosX,PosY,SizeY,VToChange,Increment,VMin,VMax))==0) return 0;
	J=B_CreateNumericButton(LB,E,CL,LB.B[J].PosX,LB.B[J].PosY+LB.B[J].SizeY+LB.SizeY[AutoINCSprite]+4,NumDigits,VToChange);
	LB.B[J].PosX=V_Max(0,
		(LB.B[J].PosX-((LB.B[J].SizeX-LB.SizeX[AutoINCSprite])>>1))
		);
	return J;
}

int SPG_CONV B_CreateVReglageCliquableNumeric(B_Lib &LB, G_Ecran& E, C_Lib& CL, int PosX,int PosY, int SizeY, int NumDigits, float * VToChange, float Increment, float VMin, float VMax)
{
	int J;
	//if((J=B_CreateVReglage(LB,E,PosX,PosY+CL.SizeY+4,SizeY,VToChange,Increment,VMin,VMax))==0) return 0;
	if((J=B_CreateVReglage(LB,E,PosX,PosY,SizeY,VToChange,Increment,VMin,VMax))==0) return 0;
	J=B_CreateCliquableNumericButton(LB,E,CL,LB.B[J].PosX,LB.B[J].PosY+LB.B[J].SizeY+LB.SizeY[AutoINCSprite]+4,NumDigits,VToChange);
	LB.B[J].PosX=V_Max(0,
		(LB.B[J].PosX-((LB.B[J].SizeX-LB.SizeX[AutoINCSprite])>>1))
		);
	return J;
}
/*
void B_Create2DReglage(B_Lib &LB, G_Ecran& E, int PosX,int PosY, int SizeX, int SizeY, float * VToChangeX, float VMinX, float VMaxX, float * VToChangeY, float VMinY, VmaxY)
{
}
*/


void SPG_CONV B_CloseButton(B_Generic& B)
{
	B.Etat=0;
	return;//au moins une fonction qui marchera toujours
}

void SPG_CONV B_PrintLabel(B_Lib& BL, int NumButton, C_Lib& CL, char * Label)
{
	CHECK(BL.Etat==0,"B_PrintLabel: Cette ButtonsLib n'existe pas",return);
	CHECK(BL.B[NumButton].E==0,"B_PrintLabel",return);
	CHECKTWO(V_IsBound(NumButton,1,MaxButton)==0,"B_PrintLabel: Ce bouton n'existe pas",Label,return);
	CHECKTWO(BL.B[NumButton].E==0,"B_PrintLabel: Ce bouton n'existe pas",Label,return);
	C_Print(*(BL.B[NumButton].E),BL.B[NumButton].PosX+(BL.B[NumButton].SizeX>>1),BL.B[NumButton].PosY-1,Label,CL,XCENTER|YDN|FONT_TRANSP);
	return;
}
//SizeX=6
//0 1 2 3 4 5
//# _ _ _ _ #
//0       M
//M=x/sizeX-2
//x=(sizeX-2)*M
void SPG_CONV B_DrawButton(B_Generic& B)
{
	TRY_BEGIN
	CHECK( ( (B.E==0) || (G_Etat((*B.E))==0) ),"B_DrawButton",return );
	//int G_BlitFromMem(G_Ecran& E, BYTE*Button, int Pitch, int SizeX,int SizeY,int X,int Y);
	IF_CD_G_CHECK(26,return);
	
	switch (B.Mode)
	{
#ifdef DebugList
	//securite inutile
	case NullButton:
		SPG_List("B_DrawButton: Un bouton nul se dessine");
		break;
#endif
		
	case ReglageHButton:
		G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.Black.Coul);
		G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.White.Coul);
		{
		int Dest=V_Round((B.SizeX-2)*(*B.VToChange-B.ValMin)/(B.ValMax-B.ValMin));
		Dest=V_Sature(Dest,0,B.SizeX-2);
		G_DrawRect(*B.E,
			B.PosX+1,B.PosY+1,
			B.PosX+1+Dest,
			B.PosY+B.SizeY-1,B.BC.FillColor.Coul);
		}
		break;
	case ReglageVButton:
		G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.Black.Coul);
		G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.White.Coul);
		{
		int Dest=V_Round((B.SizeY-2)*(B.ValMax-*B.VToChange)/(B.ValMax-B.ValMin));
		Dest=V_Sature(Dest,0,B.SizeY-2);
		G_DrawRect(*B.E,
			B.PosX+1,B.PosY+1+Dest,
			B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.FillColor.Coul);
		}
		break;
	case CliquableNumericButton:
	case NumericButton:
	case CliquableNumericIntButton:
	case NumericIntButton:
		G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.Black.Coul);
#ifdef SPG_General_USECarac
		G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.CL->BackGroundColor.Coul);
#endif
		{
			char BVal[64];
if((B.Mode==NumericIntButton)||(B.Mode==CliquableNumericIntButton))
{
	sprintf(BVal,"%d",*B.IntToChange);
}
else
{
			BVal[0]=0;
			CF_GetString(BVal, *B.VToChange, B.NumDigits);
}
			C_Print(*B.E,B.PosX+(B.SizeX>>1),B.PosY+2,BVal,*B.CL,XCENTER);
		}
		break;
	default:
		if( (B.PosX>0) && ((B.PosX+B.SizeX)<G_SizeX((*B.E))) && (B.PosY>0) && ((B.PosY+B.SizeY)<G_SizeY((*B.E))) )
		{
			G_BlitFromMem(*B.E,B.PosX,B.PosY,
			B.TP+((B.Etat&B_Click)?((*B.E).POCT*B.SizeX):0),
			B.Pitch,
			B.SizeX,B.SizeY);
		}
		break;
	}
	return;
	TRY_ENDG("B_DrawButton")
}

void SPG_CONV B_UpdateButton(B_Generic& B, int MouseX, int MouseY, int MouseState,int& ShouldFocus, DWORD ThreadId)
{
	MouseX-=B.E->PosX;
	MouseY-=B.E->PosY;
	//la fonction essentielle qui evite de cliquer tous les boutons
	//d'un seul clic
	if(MouseState==0) ShouldFocus=0;

	if((V_IsBound(MouseX,B.PosX,B.PosX+B.SizeX)&&
		V_IsBound(MouseY,B.PosY,B.PosY+B.SizeY))==0) 
	{//si le curseur n'est pas sur la surface du bouton on relache le click
		if (ShouldFocus==0) MouseState=0;
	}
	else if (MouseState) ShouldFocus=1;
	/*
	if ((V_IsBound(MouseX,B.PosX,B.PosX+LB.SizeX[B.Mode])&
		V_IsBound(MouseY,B.PosY,B.PosY+LB.SizeY[B.Mode]))||
		MouseState==0)
		*/
	CD_G_CHECK_EXIT(4,18);
	{
		
		switch (B.Mode)
		{
#ifdef DebugList
		case NullButton:
			SPG_List("Un bouton nul se promene");
			break;
#endif
		case ClickButton:
			if (MouseState)
			{
				if ((B.Etat&B_Click)==0)
				{
					B.Etat=B_Click|B_Change;
					B_DrawButton(B);
				}
				else
					B.Etat&=~B_Change;
			}
			else if (B.Etat&B_Click)
			{
				B.Etat=B_Normal|B_Change;
				B_DrawButton(B);
			}
			else
				B.Etat&=~B_Change;

			break;
		case AutoMultiINCButton://idem que clickbutton
		case AutoINCButton://idem que clickbutton
			if (MouseState)
			{
				if ((B.Etat&B_Click)==0)
				{
					B.Etat=B_Click|B_Change;
					B_DrawButton(B);
					*B.VToChange+=B.Increment;
					if (*B.VToChange>B.ValMax) *B.VToChange=B.ValMax;
				}
				else
				{
					B.Etat&=~B_Change;
					if (B.Mode==AutoMultiINCButton)
					{
					*B.VToChange+=B.Increment;
					if (*B.VToChange>B.ValMax) *B.VToChange=B.ValMax;
					}
				}
			}
			else if (B.Etat&B_Click)
			{
				B.Etat=B_Normal|B_Change;
				B_DrawButton(B);
			}
				else
					B.Etat&=~B_Change;
			break;
		case AutoMultiDECButton://idem que AutoINC
		case AutoDECButton://idem que AutoINC
			if (MouseState)
			{
				if ((B.Etat&B_Click)==0)
				{
					B.Etat=B_Click|B_Change;
					B_DrawButton(B);
					*B.VToChange-=B.Increment;
					if (*B.VToChange<B.ValMin) *B.VToChange=B.ValMin;
				}
				else
				{
					B.Etat&=~B_Change;
					if (B.Mode==AutoMultiDECButton)
					{
					*B.VToChange-=B.Increment;
					if (*B.VToChange<B.ValMin) *B.VToChange=B.ValMin;
					}
				}
			}
			else if (B.Etat&B_Click)
			{
				B.Etat=B_Normal|B_Change;
				B_DrawButton(B);
			}
				else
					B.Etat&=~B_Change;
			break;
		case CheckButton:
			if (MouseState)
			{
				if (((B.Etat&B_Click)==0)&&((B.Etat&B_Waiting)==0))
				{
					B.Etat=B_Click|B_Waiting|B_Change;
					B_DrawButton(B);
				}
				else if ((B.Etat&B_Waiting)==0)
				{
					B.Etat=B_Normal|B_Waiting|B_Change;
					B_DrawButton(B);
				}
				else
				{
					B.Etat&=~B_Change;
				}
			}
			else
				B.Etat&=~(B_Waiting|B_Change);
			
			break;
		case CheckIntButton:
			if (MouseState)
			{
				if (((B.Etat&B_Click)==0)&&((B.Etat&B_Waiting)==0))
				{
					B.Etat=B_Click|B_Waiting|B_Change;
					*B.IntToChange|=B.Flag;
					B_DrawButton(B);
				}
				else if ((B.Etat&B_Waiting)==0)
				{
					B.Etat=B_Normal|B_Waiting|B_Change;
					*B.IntToChange&=~B.Flag;
					//B_DrawButton(B);
				}
				else
				{
					B.Etat&=~B_Change;
				}
			}
			else
				B.Etat&=~(B_Waiting|B_Change);

			if ((*B.IntToChange&B.Flag)!=(B.OldInt&B.Flag))
			{
				if((*B.IntToChange&B.Flag)) 
				{
					B.Etat|=B_Click|B_Change; 
				}
				else
				{
					B.Etat&=~B_Click;
					B.Etat|=B_Normal|B_Change; 
				}
				B.OldInt=*B.IntToChange;
				B_DrawButton(B);
			}
			break;
		case ReglageHButton:
			if (MouseState)
			{
				*B.VToChange=B.ValMin+(MouseX-B.PosX)*(B.ValMax-B.ValMin)/(B.SizeX-2);
					if (*B.VToChange>B.ValMax) *B.VToChange=B.ValMax;
					if (*B.VToChange<B.ValMin) *B.VToChange=B.ValMin;
			}
			B.Etat=0;
			if (*B.VToChange!=B.OldV)
			{
			B_DrawButton(B);
			B.Etat|=B_Change;
			B.OldV=*B.VToChange;
			}
			break;
		case ReglageVButton:
			if (MouseState)
			{
				*B.VToChange=B.ValMax+(MouseY-(B.PosY+1))*(B.ValMin-B.ValMax)/(B.SizeY-2);
					if (*B.VToChange>B.ValMax) *B.VToChange=B.ValMax;
					if (*B.VToChange<B.ValMin) *B.VToChange=B.ValMin;
			}
			B.Etat=0;
			if (*B.VToChange!=B.OldV)
			{
			B_DrawButton(B);
			B.Etat|=B_Change;
			B.OldV=*B.VToChange;
			}
			break;
		case NumericIntButton:
			B.Etat=0;
			B_DrawButton(B);
			break;
		case NumericButton:
			B.Etat=0;
			if (*B.VToChange!=B.OldV)
			{
			B_DrawButton(B);
			B.Etat|=B_Change;
			B.OldV=*B.VToChange;
			}
			break;
		case CliquableNumericButton:
			if (MouseState)
			{
				if ((B.Etat&B_Click)==0)
				{
					B.Etat=B_Click|B_Change;
					B_EditNumericButton(B,ThreadId);
					B_DrawButton(B);
				}
				else
					B.Etat&=~B_Change;
			}
			else if (B.Etat&B_Click)
			{
				B.Etat=B_Normal|B_Change;
				B_DrawButton(B);
			}
			else if (B.Etat&B_Change)
			{
				B_DrawButton(B);
				B.Etat&=~B_Change;
			}

/*			
			if((B.Etat&(B_Click|B_Change))==(B_Click|B_Change))
			{
				B_EditNumericButton(B);
			}
*/			
			/*
			B.Etat=0;
			*/
			if (*B.VToChange!=B.OldV)
			{
			B_DrawButton(B);
			B.Etat|=B_Change;
			B.OldV=*B.VToChange;
			}
			break;

		case CliquableNumericIntButton:
			if (MouseState)
			{
				if ((B.Etat&B_Click)==0)
				{
					B.Etat=B_Click|B_Change;
					B_EditNumericIntButton(B,ThreadId);
					B_DrawButton(B);
				}
				else
					B.Etat&=~B_Change;
			}
			else if (B.Etat&B_Click)
			{
				B.Etat=B_Normal|B_Change;
				B_DrawButton(B);
			}
			else if (B.Etat&B_Change)
			{
				B_DrawButton(B);
				B.Etat&=~B_Change;
			}

/*			
			if((B.Etat&(B_Click|B_Change))==(B_Click|B_Change))
			{
				B_EditNumericButton(B);
			}
*/			
			/*
			B.Etat=0;
			*/
			if (*B.IntToChange!=B.OldInt)
			{
			B_DrawButton(B);
			B.Etat|=B_Change;
			B.OldInt=*B.IntToChange;
			}
			break;

			
		}
	}
	return;
}

#ifdef SPG_General_USEWindows

void SPG_CONV B_EditNumericButton(B_Generic& B, DWORD ThreadId)
{
	LTG_Enter(Global,LT_EditNumericButton,0);
	DbgCHECK(B.Mode!=CliquableNumericButton,"Bouton non editable");
	char BVal[64];
	BVal[0]=0;
	//CF_GetString(BVal, *B.VToChange, B.NumDigits);
	int LB=0;//strlen(BVal);
	int EditState=0;
#ifdef SPG_General_USETimer
	S_CreateTimer(CursorTimer,"Curseur B_EditNumericButton");
#endif
	bool CursorState=0;
	bool MustRedraw=true;
#ifdef SPG_General_USETimer
	S_StartTimer(CursorTimer);
#endif
	CHECK(ThreadId!=GetCurrentThreadId(),"B_EditNumericButton",return);
	SPG_WaitMouseRelease();
	while((EditState==0)&&(SPG_Global_MouseLeft==0))
	{
		DoEvents(SPG_DOEV_ALL_NO_WIN_EVENT);
		int HasMsg;
		MSG msg;
		if ((HasMsg=PeekMessage(&msg, (HWND) NULL, 0, 0,PM_REMOVE))!=0)
		{
			if(msg.message==WM_LBUTTONDOWN)
			{
					EditState=1;//quit
					break;
			}
			if((msg.message==WM_MBUTTONDOWN)||(msg.message==WM_RBUTTONDOWN))
			{
					EditState=2;//quit
					break;
			}
			if(msg.message==WM_CHAR)
			{
				
				switch((char)msg.wParam)
				{
				case 8:
					if(LB) 
					{
						LB--;
						BVal[LB]=0;
					}
					MustRedraw=true;
					break;
					//EditState=1;//quit
				case 13:
					EditState=1;//quit
					break;
				case 27:
					EditState=2;//quit, no change
					break;
				default:
					BVal[LB]=(char)msg.wParam;
					if(LB<(B.NumDigits+4))
					{
					LB++;
					}
					BVal[LB]=0;
					MustRedraw=true;
					break;
				}
			}
			else if (msg.message==WM_KEYDOWN)
			{
				switch((char)msg.wParam)
				{
				case 46://touche DEL
					if(LB) 
					{
						LB--;
						BVal[LB]=0;
					}
					MustRedraw=true;
					break;
				default:
					TranslateMessage(&msg);
					DispatchMessage(&msg);
					break;
				}
			}
			else
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}
		}

		/*
		if(EditState==1)
		{
			sscanf( BVal, "%f", *B.VToChange );
		}
		BVal[0]=0;
		CF_GetString(BVal, *B.VToChange, B.NumDigits);
		*/
#ifdef SPG_General_USETimer
		float T;
		S_GetTimerRunningTime(CursorTimer,T);
		if(T>0.5f)
		{
			S_StopTimer(CursorTimer);
			S_ResetTimer(CursorTimer);
			S_StartTimer(CursorTimer);
			MustRedraw=true;
			CursorState=!CursorState;
		}
#endif
		if(MustRedraw)
		{
			if(CursorState)
			{
			G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.ForeGround.Coul);
			G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.BackGround.Coul);
			}
			else
			{
			G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.BackGround.Coul);
			G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.ForeGround.Coul);
			}
		C_Print(*B.E,B.PosX+(B.SizeX>>1),B.PosY+2,BVal,*B.CL,XCENTER|FONT_TRANSP);
		MustRedraw=false;
		}

		if(HasMsg==0) {DoEvents(SPG_DOEV_UPDATE);G_BlitEcran(*B.E);}//|SPG_DOEV_BLITSCREEN);
	}
#ifdef SPG_General_USETimer
	S_StopTimer(CursorTimer);
	S_CloseTimer(CursorTimer);
#endif
	//B_RedrawButtonsLib(BL,1);
	if((EditState==1)&&(BVal[0]))
	{
		sscanf( BVal, "%f", B.VToChange );
	}
	
	BVal[0]=0;
	CF_GetString(BVal, *B.VToChange, B.NumDigits);
	G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.Black.Coul);
#ifdef SPG_General_USECarac
	G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.CL->BackGroundColor.Coul);
#endif
	C_Print(*B.E,B.PosX+(B.SizeX>>1),B.PosY+2,BVal,*B.CL,XCENTER);
	LTG_Exit(Global,LT_EditNumericButton,0);
	return;
}

//#error fin du fichier

void SPG_CONV B_EditNumericIntButton(B_Generic& B, DWORD ThreadId)
{
	LTG_Enter(Global,LT_EditNumericButton,0);
	DbgCHECK(B.Mode!=CliquableNumericIntButton,"Bouton non editable");
	char BVal[64];
	BVal[0]=0;
	//sprintf(BVal,"%d",*B.IntToChange);
	int LB=0;//strlen(BVal);
	int EditState=0;
#ifdef SPG_General_USETimer
	S_CreateTimer(CursorTimer,"Curseur B_EditNumericIntButton");
#endif
	bool CursorState=0;
	bool MustRedraw=true;
#ifdef SPG_General_USETimer
	S_StartTimer(CursorTimer);
#endif
	CHECK(ThreadId!=GetCurrentThreadId(),"B_EditNumericIntButton",return);
	SPG_WaitMouseRelease();
	while((EditState==0)&&(SPG_Global_MouseLeft==0))
	{
		DoEvents(SPG_DOEV_ALL_NO_WIN_EVENT);
		int HasMsg;
		MSG msg;
		if ((HasMsg=PeekMessage(&msg, (HWND) NULL, 0, 0,PM_REMOVE))!=0)
		{
			if(msg.message==WM_LBUTTONDOWN)
			{
					EditState=1;//quit
					break;
			}
			if((msg.message==WM_MBUTTONDOWN)||(msg.message==WM_RBUTTONDOWN))
			{
					EditState=2;//quit
					break;
			}
			if(msg.message==WM_CHAR)
			{
				
				switch((char)msg.wParam)
				{
				case 8:
					if(LB) 
					{
						LB--;
						BVal[LB]=0;
					}
					MustRedraw=true;
					break;
					//EditState=1;//quit
				case 13:
					EditState=1;//quit
					break;
				case 27:
					EditState=2;//quit, no change
					break;
				default:
					BVal[LB]=(char)msg.wParam;
					if(LB<(B.NumDigits+1))
					{
					LB++;
					}
					BVal[LB]=0;
					MustRedraw=true;
					break;
				}
			}
			else if (msg.message==WM_KEYDOWN)
			{
				switch((char)msg.wParam)
				{
				case 46://touche DEL
					if(LB) 
					{
						LB--;
						BVal[LB]=0;
					}
					MustRedraw=true;
					break;
				default:
					TranslateMessage(&msg);
					DispatchMessage(&msg);
					break;
				}
			}
			else
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}
		}

		/*
		if(EditState==1)
		{
			sscanf( BVal, "%f", *B.VToChange );
		}
		BVal[0]=0;
		CF_GetString(BVal, *B.VToChange, B.NumDigits);
		*/
#ifdef SPG_General_USETimer
		float T;
		S_GetTimerRunningTime(CursorTimer,T);
		if(T>0.5f)
		{
			S_StopTimer(CursorTimer);
			S_ResetTimer(CursorTimer);
			S_StartTimer(CursorTimer);
			MustRedraw=true;
			CursorState=!CursorState;
		}
#endif
		if(MustRedraw)
		{
			if(CursorState)
			{
			G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.ForeGround.Coul);
			G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.BackGround.Coul);
			}
			else
			{
			G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.BackGround.Coul);
			G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.ForeGround.Coul);
			}
		C_Print(*B.E,B.PosX+(B.SizeX>>1),B.PosY+2,BVal,*B.CL,XCENTER|FONT_TRANSP);
		MustRedraw=false;
		}


		if(HasMsg==0) {DoEvents(SPG_DOEV_UPDATE);G_BlitEcran(*B.E);}//|SPG_DOEV_BLITSCREEN);
	}
#ifdef SPG_General_USETimer
	S_StopTimer(CursorTimer);
	S_CloseTimer(CursorTimer);
#endif
	//B_RedrawButtonsLib(BL,1);
	if((EditState==1)&&(BVal[0]))
	{
		sscanf( BVal, "%d", B.IntToChange );
	}
	
	sprintf(BVal,"%d",*B.IntToChange);
	G_DrawOutRect(*B.E,B.PosX,B.PosY,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.BC.Black.Coul);
#ifdef SPG_General_USECarac
	G_DrawRect(*B.E,B.PosX+1,B.PosY+1,B.PosX+B.SizeX-1,B.PosY+B.SizeY-1,B.CL->BackGroundColor.Coul);
#endif
	C_Print(*B.E,B.PosX+(B.SizeX>>1),B.PosY+2,BVal,*B.CL,XCENTER);
	LTG_Exit(Global,LT_EditNumericButton,0);
	return;
}

#endif


#endif


