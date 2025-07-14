
#include "SPG_General.h"

#ifdef SPG_General_USEPrCV

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <string.h>
#include <stdio.h>

#define Is3DFullScreen

int SPG_CONV CV_Init(CV_State& CV,G_Ecran& E, Profil* P)
{
	memset(&CV,0,sizeof(CV_State));
	int BtSizeX=160;
#ifdef Is3DFullScreen
	CV.FullScreen=E;
	G_InitMemoryEcran(CV.ScreenBackup,G_POCT(E),G_SizeX(E),G_SizeY(E));
#endif
	if(G_InitSousEcran(CV.E,E,0,0,(E.SizeX-BtSizeX)&0xFFFFFFF8,E.SizeY)==0) return 0;
	if(G_InitSousEcran(CV.EBut,E,E.SizeX-BtSizeX,0,BtSizeX,E.SizeY)==0) return 0;
	if(B_LoadButtonsLib(CV.BL,E,1,0,"..\\SrcC\\Interface","ButtonsGrey.bmp")==0) return 0;

	if(C_LoadCaracLib(CV.CL,E,"..\\SrcC\\Carac","CaracNoir.bmp")==0) return 0;
	//C_LoadCaracLib(CV.CL,E,"..\\SrcC\\Carac","CNew8.bmp");

	int SpaceX=CV.BL.SizeX[ClickSprite]+2+3*CV.CL.SizeX;
	int SpaceY=CV.BL.SizeY[ClickSprite]+CV.CL.SpaceY+2;
	int CurX=2+CV.CL.SizeX;
	int CurY=2+2*CV.CL.SpaceY;
	CV.BLoad=B_CreateClickButton(CV.BL,CV.EBut,CurX,CurY);
	B_PrintLabel(CV.BL,CV.BLoad,CV.CL,"Load");
	CV.BFillB=B_CreateClickButton(CV.BL,CV.EBut,CurX+SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BFillB,CV.CL,"Fill\nBlanks");
	CV.BRemB=B_CreateClickButton(CV.BL,CV.EBut,CurX+2*SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BRemB,CV.CL,"Remove\nBorders");
	CurY+=SpaceY+CV.CL.SpaceY;
	CV.BSave=B_CreateClickButton(CV.BL,CV.EBut,CurX,CurY);
	B_PrintLabel(CV.BL,CV.BSave,CV.CL,"Save");
	CV.BJ0=B_CreateClickButton(CV.BL,CV.EBut,CurX+SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BJ0,CV.CL,"J0\nInvert");
	CV.BConv3x3=B_CreateClickButton(CV.BL,CV.EBut,CurX+2*SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BConv3x3,CV.CL,"Conv\n3x3");

	CurY+=SpaceY+CV.CL.SpaceY;
	CV.BQuit=B_CreateClickButton(CV.BL,CV.EBut,CurX,CurY);
	B_PrintLabel(CV.BL,CV.BQuit,CV.CL,"Quit");
#ifdef SPG_General_USEProfil3D
	CV.B3D=B_CreateClickButton(CV.BL,CV.EBut,CurX+SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.B3D,CV.CL,"Vue 3D");
#endif
	CV.BCrop=B_CreateClickButton(CV.BL,CV.EBut,CurX+2*SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BCrop,CV.CL,"Save\nSelection");
	CurY+=SpaceY+CV.CL.SpaceY;
	CV.BDivise=B_CreateClickButton(CV.BL,CV.EBut,CurX,CurY);
	B_PrintLabel(CV.BL,CV.BDivise,CV.CL,"Divise");
	CV.BDirFilter=B_CreateClickButton(CV.BL,CV.EBut,CurX+SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BDirFilter,CV.CL,"Directionnal\nFilter");
	CV.BSub=B_CreateClickButton(CV.BL,CV.EBut,CurX+2*SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BSub,CV.CL,"Substract");

	CurY+=SpaceY+CV.CL.SpaceY;
	CV.BLoadMsk=B_CreateClickButton(CV.BL,CV.EBut,CurX,CurY);
	B_PrintLabel(CV.BL,CV.BLoadMsk,CV.CL,"Load\nMask");
	CV.BSaveMsk=B_CreateClickButton(CV.BL,CV.EBut,CurX+SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BSaveMsk,CV.CL,"Save\nMask");
	CV.BRemDots=B_CreateCheckButton(CV.BL,CV.EBut,CurX+2*SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BRemDots,CV.CL,"Remove\nDots");

	CurY+=SpaceY+CV.CL.SpaceY;
	CV.BFlatten=B_CreateClickButton(CV.BL,CV.EBut,CurX,CurY);
	B_PrintLabel(CV.BL,CV.BFlatten,CV.CL,"Flatten");
	CV.BLevel=B_CreateClickButton(CV.BL,CV.EBut,CurX+SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BLevel,CV.CL,"Level");

	CurY+=SpaceY+CV.CL.SpaceY;
	CV.B2DFlatten=B_CreateClickButton(CV.BL,CV.EBut,CurX,CurY);
	B_PrintLabel(CV.BL,CV.B2DFlatten,CV.CL,"  2DFlatten");
	CV.BStepHeight=B_CreateClickButton(CV.BL,CV.EBut,CurX+SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BStepHeight,CV.CL,"Height");
	CV.BOther=B_CreateClickButton(CV.BL,CV.EBut,CurX+2*SpaceX,CurY);
	B_PrintLabel(CV.BL,CV.BOther,CV.CL,"Other");

	CV.ZMin=0.0f;
	CV.ZMax=1.0f;
	CV.RemDotsThreshold=2.0f;

	CurY+=SpaceY;
	CV.BSeuilBas=B_CreateVReglageCliquableNumeric(CV.BL,CV.EBut,CV.CL,CurX+SpaceX/2,CurY,CV.EBut.SizeY-CurY-32,6,&CV.ZMin,(CV.ZMax-CV.ZMin)/256.0f,CV.ZMin,CV.ZMax);
	CV.BSeuilHaut=B_CreateVReglageCliquableNumeric(CV.BL,CV.EBut,CV.CL,CurX+2*SpaceX,CurY,CV.EBut.SizeY-CurY-32,6,&CV.ZMax,(CV.ZMax-CV.ZMin)/256.0f,CV.ZMin,CV.ZMax);
	CV.BDotsThreshold=B_CreateVReglageCliquableNumeric(CV.BL,CV.EBut,CV.CL,CurX+7*SpaceX/2,CurY,CV.EBut.SizeY-CurY-32,6,&CV.RemDotsThreshold,0.1f,0.2f,5.0f);

	B_RedrawButtonsLib(CV.BL,0);

	INVJ0_Init(CV.INVJ0,2048);

	if(P)
	{
		P_Dupliquate(CV.P,*P);
		P_DupliquateWithMsk(CV.PDisplay,CV.P);
		CV_SetScale(CV);
		PrXt_Init(CV.PrXt,CV.E,CV.PDisplay);
		ZP_Init(CV.Selection,0,0,P_SizeX(CV.P),P_SizeY(CV.P));
		ZP_SelectProfil(CV.Selection,P_Header(CV.P));
	}
	return CV.Etat=-1;
}

void SPG_CONV CV_Close(CV_State& CV)
{
	CHECK(CV.Etat==0,"CV_State nul",return);

	INVJ0_Close(CV.INVJ0);
	if(CV.PrXt.Etat) PrXt_Close(CV.PrXt);
	if(CV.Selection.Etat) ZP_Close(CV.Selection);
	if(CV.P3D.Etat) Profil3D_Close(CV.P3D);
	if(P_Etat(CV.PDisplay)) P_Close(CV.PDisplay);
	if(P_Etat(CV.P)) P_Close(CV.P);

	C_CloseCaracLib(CV.CL);
	B_CloseButtonsLib(CV.BL);
	G_CloseEcran(CV.EBut);
	G_CloseEcran(CV.E);
#ifdef Is3DFullScreen
	G_CloseEcran(CV.ScreenBackup);
#endif

	memset(&CV,0,sizeof(CV_State));
	return;
}

void SPG_CONV CV_SetScale(CV_State& CV)
{
	P_FindMinMax(CV.P,CV.ZMin,CV.ZMax);
	CV.BL.B[CV.BSeuilBas-3].ValMin=CV.ZMin;
	CV.BL.B[CV.BSeuilBas-3].ValMax=CV.ZMax;
	CV.BL.B[CV.BSeuilBas-2].ValMin=CV.ZMin;
	CV.BL.B[CV.BSeuilBas-2].ValMax=CV.ZMax;
	CV.BL.B[CV.BSeuilBas-1].ValMin=CV.ZMin;
	CV.BL.B[CV.BSeuilBas-1].ValMax=CV.ZMax;
	CV.BL.B[CV.BSeuilHaut-3].ValMin=CV.ZMin;
	CV.BL.B[CV.BSeuilHaut-3].ValMax=CV.ZMax;
	CV.BL.B[CV.BSeuilHaut-2].ValMin=CV.ZMin;
	CV.BL.B[CV.BSeuilHaut-2].ValMax=CV.ZMax;
	CV.BL.B[CV.BSeuilHaut-1].ValMin=CV.ZMin;
	CV.BL.B[CV.BSeuilHaut-1].ValMax=CV.ZMax;
	return;
}

int SPG_CONV CV_UpdateButtons(CV_State& CV)
{
	int ForceUpdate=CV_OK;
#ifdef Is3DFullScreen
	if(CV.P3D.Etat==0)
		{
#endif
	B_UpdateButtonsLib(CV.BL,SPG_Global_MouseX,SPG_Global_MouseY,SPG_Global_MouseLeft);
	if(B_IsChangedToClick(CV.BL,CV.BFillB))
	{
		P_FillInTheBlanks(CV.P,0,1);
		CV_SetScale(CV);
	}
	if(B_IsChangedToClick(CV.BL,CV.BRemB))
	{
		P_RemoveBorder(CV.P,1);
		CV_SetScale(CV);
	}
	if(B_IsChangedToClick(CV.BL,CV.BLoad))
	{
		if(P_Etat(CV.PDisplay)) P_Close(CV.PDisplay);
		int OldSizeX=P_SizeX(CV.P);
		int OldSizeY=P_SizeY(CV.P);
		if(P_Etat(CV.P)) P_Close(CV.P);
		if(P_Load(CV.P,"Profil2D"))
		{
			P_DupliquateWithMsk(CV.PDisplay,CV.P);
			if((P_SizeX(CV.P)==OldSizeX)&&(P_SizeY(CV.P)==OldSizeY))
			{
			CV_SetScale(CV);
			}
			else
			{
			CV_SetScale(CV);
			B_SetAndRedraw(CV.BL,CV.B3D,B_Change);
			}
		}
		ForceUpdate|=CV_FORCEUPDATE;
	}
#ifdef SPG_General_USEProfil3D
	if(B_IsChangedToClick(CV.BL,CV.B3D))
	{
		if(CV.PrXt.Etat) PrXt_Close(CV.PrXt);
		if(CV.Selection.Etat) ZP_Close(CV.Selection);
		if(CV.P3D.Etat) Profil3D_Close(CV.P3D);
#ifdef Is3DFullScreen
		G_Copy(CV.ScreenBackup,CV.FullScreen);
		Profil3D_Init(CV.FullScreen,CV.P3D,CV.PDisplay,CV.IsStereo);
		CV.BL.FocusButton=0;
#else
		Profil3D_Init(CV.E,CV.P3D,CV.PDisplay,CV.IsStereo);
#endif
		SPG_WaitMouseRelease();
		//B_SetAndRedraw(CV.P3D.BL,CV.P3D.HiResDraw,(B_Click|B_Change));
	}
	if(B_IsChangedToNotClick(CV.BL,CV.B3D))
	{
		if(CV.PrXt.Etat) PrXt_Close(CV.PrXt);
		if(CV.Selection.Etat) ZP_Close(CV.Selection);
		if(CV.P3D.Etat) Profil3D_Close(CV.P3D);
		PrXt_Init(CV.PrXt,CV.E,CV.PDisplay);
		ZP_Init(CV.Selection,0,0,P_SizeX(CV.P),P_SizeY(CV.P));
		ZP_SelectProfil(CV.Selection,P_Header(CV.P));
		SPG_WaitMouseRelease();
	}
#endif
	if(B_IsChangedToClick(CV.BL,CV.BSave))
	{
		P_Save(CV.PDisplay,"Profil2D");
	}
	if(B_IsChangedToClick(CV.BL,CV.BJ0))
	{
		if(P_Etat(CV.P))
		{
			for(int i=0;i<P_SizeX(CV.P)*P_SizeY(CV.P);i++)
			{
				if((CV.P.Msk==0)||(CV.P.Msk[i]==0))
				{
					float Y=V_Sature(CV.P.D[i],0,1);
					INVJ0_Invert(CV.INVJ0,CV.P.D[i],Y);
				}
				else
				{
					CV.P.D[i]=0;
				}
			}
		CV_SetScale(CV);
		/*
		if(CV.P3D.Etat)
		{
			Profil3D_UpdateColorMaps(CV.P3D,0,1,1);
		}
		*/
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BConv3x3))
	{
		if(P_Etat(CV.P))
		{
			P_MaskConv3x3(CV.P);
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BSub))
	{
		Profil SubP;
		if(P_Load(SubP,"Profil2D"))
		{
			if((CV.P.H.SizeX==SubP.H.SizeX)&&(CV.P.H.SizeY==SubP.H.SizeY))
			{
				for(int i=0;i<CV.P.H.SizeX*CV.P.H.SizeY;i++)
				{
					if(
						((P_Msk(SubP)==0)||(SubP.Msk[i]==0))&&
						((P_Msk(CV.P)==0)||(CV.P.Msk[i]==0)))
					{
						CV.P.D[i]-=SubP.D[i];
					}
					else
					{
						CV.P.D[i]=0;
						if (P_Msk(CV.P)) CV.P.Msk[i]=1;
					}
				}
			}
			P_Close(SubP);
			CV_SetScale(CV);
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BDivise))
	{
		Profil DivP;
		if(P_Load(DivP,"Profil2D"))
		{
			P_Divise(CV.P,DivP);
			P_Close(DivP);
			CV_SetScale(CV);
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BDirFilter))
	{
		Profil Ptmp;
		P_Dupliquate(Ptmp,CV.P);
		P_Clear(Ptmp);
		{for(int y=0;y<P_SizeY(CV.P);y++)
		{
			for(int x=8;x<P_SizeX(CV.P)-8;x++)
			{
				P_Element(Ptmp,x,y)=
					(1.0f/86.0f)*(
					1*P_Element(CV.P,x-8,y)+
					2*P_Element(CV.P,x-7,y)+
					4*P_Element(CV.P,x-6,y)+
					7*P_Element(CV.P,x-5,y)+
					9*P_Element(CV.P,x-4,y)+
					10*P_Element(CV.P,x-3,y)+
					10*P_Element(CV.P,x-2,y)+
					10*P_Element(CV.P,x-1,y)+
					10*P_Element(CV.P,x,y)+
					10*P_Element(CV.P,x+1,y)+
					10*P_Element(CV.P,x+2,y)+
					10*P_Element(CV.P,x+3,y)+
					9*P_Element(CV.P,x+4,y)+
					7*P_Element(CV.P,x+5,y)+
					4*P_Element(CV.P,x+6,y)+
					2*P_Element(CV.P,x+7,y)+
					1*P_Element(CV.P,x+8,y)
					);
			}
		}}
		{for(int y=2;y<P_SizeY(CV.P)-2;y++)
		{
			for(int x=8;x<P_SizeX(CV.P)-8;x++)
			{
				P_Element(CV.P,x,y)+=(0.5f/6.0f)*(P_Element(Ptmp,x,y-2)+2*P_Element(Ptmp,x,y-1)+2*P_Element(Ptmp,x,y+1)+P_Element(Ptmp,x,y+2))-0.5f*P_Element(Ptmp,x,y);
			}
		}}
		P_Close(Ptmp);
		/*
		if(CV.P3D.Etat)
		{
			Profil3D_UpdateColorMaps(CV.P3D,0,1,1);
		}
		*/
	}
	if(B_IsChangedToClick(CV.BL,CV.BSaveMsk))
	{
		if(P_Msk(CV.P))
		{
			G_Ecran E;
			if(G_InitAliasMemEcran(E,P_Msk(CV.P),P_SizeX(CV.P),1,P_SizeX(CV.P),P_SizeY(CV.P)))
			{
				G_SaveAs_ToBMP(E,"Masque");
				G_CloseEcran(E);
			}
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BLoadMsk))
	{
			char FileName[MaxProgDir];
			strcpy(FileName,"Masque");
			SPG_GetLoadName(SPG_BMP,FileName,MaxProgDir);
			G_Ecran E;
			if(G_InitEcranFromFile(E,1,0,FileName))
			{
				CHECK((G_SizeX(E)!=P_SizeX(CV.P))||(G_SizeY(E)!=P_SizeY(CV.P)),"Taille incorrecte",;)
				V_Swap(BYTE*,P_Msk(CV.P),G_GetPix(E));
				G_CloseEcran(E);
			}
			if(P_Etat(CV.PDisplay)) P_Close(CV.PDisplay);
			P_DupliquateWithMsk(CV.PDisplay,CV.P);
	}
	if(B_IsChangedToClick(CV.BL,CV.BCrop))
	{
		if(CV.Selection.Etat)
		{
			SPG_StackAlloc(Profil,PCrop);
			P_Create(PCrop,
				ZP_SizeX(CV.Selection),ZP_SizeY(CV.Selection),
				P_XScale(CV.P),P_YScale(CV.P),
				P_UnitX(CV.P),P_UnitY(CV.P),P_UnitZ(CV.P),
				(int)P_Msk(CV.P));
			P_CopyAt(P_Data(PCrop),1,P_SizeX(PCrop),
				0,0,
				P_Data(CV.P),1,P_SizeX(CV.P),
				ZP_PosX(CV.Selection),ZP_PosY(CV.Selection),
				P_SizeX(PCrop),P_SizeY(PCrop));
			if(P_Msk(PCrop))
			{
				P8_CopyAt(P_Msk(PCrop),1,P_SizeX(PCrop),
					0,0,
					P_Msk(CV.P),1,P_SizeX(CV.P),
					ZP_PosX(CV.Selection),ZP_PosY(CV.Selection),
					P_SizeX(PCrop),P_SizeY(PCrop));
			}
			P_Save(PCrop,"Profil2DCrop");
			P_Close(PCrop);
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BFlatten))
	{
		if(CV.PrXt.Etat)
		{
			Profil* OldP=CV.PrXt.SP.P;
			CV.PrXt.SP.P=&CV.P;
			SP_Flatten(CV.PrXt.SP);
			CV.PrXt.SP.P=OldP;
			CV_SetScale(CV);
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.B2DFlatten))
	{
		if(CV.PrXt.Etat)
		{
			//float TiltX,TiltY;
			P_RemoveSurfaceTilt(CV.P,4);
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BStepHeight))
	{
		if(CV.PrXt.Etat)
		{
			float S0,S1;
			if(P_GetStepHeight(CV.P,S0,S1,4))
			{
				char Msg[128];
				sprintf(Msg,"Step heigth:%.3f",S1-S0);
				MessageBox((HWND)Global.hWndWin,Msg,"ProfilConvert",MB_OK);
				//SPG_List(Msg);
			}
		}
	}
	if(B_IsChangedToClick(CV.BL,CV.BLevel))
	{
		if(CV.PrXt.Etat)
		{
			Profil* OldP=CV.PrXt.SP.P;
			CV.PrXt.SP.P=&CV.P;
			SP_Level(CV.PrXt.SP);
			CV.PrXt.SP.P=OldP;
			CV_SetScale(CV);
		}
	}
#ifdef Is3DFullScreen
	}
	else
	{
	if(B_IsClick(CV.P3D.BL,CV.P3D.BQuit))
		{
			if(CV.PrXt.Etat) PrXt_Close(CV.PrXt);
			if(CV.Selection.Etat) ZP_Close(CV.Selection);
			if(CV.P3D.Etat) Profil3D_Close(CV.P3D);
			G_Copy(CV.FullScreen,CV.ScreenBackup);
			B_SetAndRedraw(CV.BL,CV.B3D,B_Normal);
			PrXt_Init(CV.PrXt,CV.E,CV.PDisplay);
			ZP_Init(CV.Selection,0,0,P_SizeX(CV.P),P_SizeY(CV.P));
			ZP_SelectProfil(CV.Selection,P_Header(CV.P));
			SPG_WaitMouseRelease();
		}
	}
#endif
	return ForceUpdate;
}

int SPG_CONV CV_UpdateDisplay(CV_State& CV)
{
	int ForceUpdate=CV_OK;
	//CHECK(P_Data(CV.PDisplay)!=DebugTmp,"PDisplay pointeur modifie",DebugTmp=CV.PDisplay.D);
	if(B_HasFocus(CV.BL))
	{
		if(P_Etat(CV.PDisplay)&&P_Etat(CV.P))
		{
			if((CV.PDisplay.Msk)&&(CV.P.Msk)) SPG_Memcpy(CV.PDisplay.Msk,CV.P.Msk,CV.P.H.SizeX*CV.P.H.SizeY);

			Profil PIntermediate;

			if(B_IsClick(CV.BL,CV.BRemDots))
			{
				P_Dupliquate(PIntermediate,CV.P);
				P_NonLinearFilter(PIntermediate,CV.RemDotsThreshold*CV.RemDotsThreshold);
			}
			else
			{
				PIntermediate=CV.P;
			}

			if(P_Data(CV.PDisplay)!=P_Data(PIntermediate))
			{
				for(int i=0;i<P_SizeX(CV.P)*P_SizeY(CV.P);i++)
				{
					P_Data(CV.PDisplay)[i]=V_Sature(P_Data(PIntermediate)[i],CV.ZMin,CV.ZMax);
				}
			}
			if(P_Msk(CV.PDisplay)&&P_Msk(PIntermediate)&&(P_Msk(CV.PDisplay)!=P_Msk(PIntermediate)))
			{
 				for(int i=0;i<P_SizeX(CV.P)*P_SizeY(CV.P);i++)
				{
					P_Msk(CV.PDisplay)[i]=P_Msk(PIntermediate)[i];
				}
			}

			if(B_IsClick(CV.BL,CV.BRemDots))
			{
				P_Close(PIntermediate);
			}
		}
		/*
		if(CV.P3D.Etat)
		{
			Profil3D_UpdateColorMaps(CV.P3D,0,1,1);
		}
		*/
	}
	if(P_Etat(CV.PDisplay))
	{
		if(CV.Selection.Etat) 
		{
			int iPosX=CV.PrXt.iPosX;
			int iPosY=CV.PrXt.iPosY;
			ZP_Update(CV.Selection,SPG_Global_MouseX-iPosX,SPG_Global_MouseY-iPosY,SPG_Global_MouseLeft);
			{
				if(CV.PrXt.Etat) 
				{
					if(ZP_HasFocus(CV.Selection))
					{
						PrXt_Update(CV.PrXt,SPG_Global_MouseX,SPG_Global_MouseY,0,SPG_Global_MouseRight);
					}
					else
					{
						PrXt_Update(CV.PrXt,SPG_Global_MouseX,SPG_Global_MouseY,SPG_Global_MouseLeft,SPG_Global_MouseRight);
					}
				}
			}
			ZP_Draw(CV.Selection,CV.PrXt.EPro,CV.CL,CV.PrXt.iPosX,CV.PrXt.iPosY);
		}
		else
		{
			if(CV.PrXt.Etat) PrXt_Update(CV.PrXt,SPG_Global_MouseX,SPG_Global_MouseY,SPG_Global_MouseLeft,SPG_Global_MouseRight);
		}
		if(CV.P3D.Etat) 
		{
			Profil3D_Update(CV.P3D);
			ForceUpdate|=CV_FORCEUPDATE;
		}
	}
	return ForceUpdate;
}

int SPG_CONV CV_Update(CV_State& CV)
{
	CHECK(CV.Etat==0,"CV_State nul",return CV_ERROR);

	int ForceUpdate=CV_OK;

	ForceUpdate|=CV_UpdateButtons(CV);

	ForceUpdate|=CV_UpdateDisplay(CV);

	return ForceUpdate;
}


#endif

