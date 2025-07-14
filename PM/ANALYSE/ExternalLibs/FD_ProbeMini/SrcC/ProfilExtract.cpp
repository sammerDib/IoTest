
#include "SPG_General.h"

#ifdef SPG_General_USEPrXt

#include "SPG_Includes.h"

#include <string.h>

int SPG_CONV PrXt_Init(ProfilExtract& PrXt, G_Ecran& E, Profil& P)
{
	memset(&PrXt,0,sizeof(ProfilExtract));

	PrXt.P=&P;

	B_LoadButtonsLib(PrXt.BL,E,1,0,"..\\SrcC\\Interface","ButtonsGrey.bmp");

	C_LoadCaracLib(PrXt.CL,E,"..\\SrcC\\Carac","CaracNoir.bmp");

	G_InitSousEcran(PrXt.EPro,E,0,0,E.SizeX-PrXt.BL.SizeX[ClickSprite],3*E.SizeY/4-PrXt.BL.SizeY[ClickSprite]);
	G_InitSousEcran(PrXt.ECut,E,0,PrXt.EPro.SizeY+PrXt.BL.SizeY[ClickSprite],E.SizeX-4*PrXt.BL.SizeX[ClickSprite],E.SizeY-PrXt.EPro.SizeY-PrXt.BL.SizeY[ClickSprite]);
	G_InitSousEcran(PrXt.EBut,E,PrXt.ECut.SizeX,PrXt.ECut.PosY,E.SizeX-PrXt.ECut.SizeX,PrXt.ECut.SizeY);

	//PrXt.BSPro=B_CreateClickButton(PrXt.BL,PrXt.EBut,32,32);
	//B_PrintLabel(PrXt.BL,PrXt.BSPro,PrXt.CL,"Save\nProfil");
	PrXt.BSCut=B_CreateClickButton(PrXt.BL,PrXt.EBut,32,64);
	B_PrintLabel(PrXt.BL,PrXt.BSCut,PrXt.CL,"Save\nCut");
 	PrXt.BDZ=B_CreateNumericButton(PrXt.BL,PrXt.EBut,PrXt.CL,16,96,6,&PrXt.DeltaZ);
	B_PrintLabel(PrXt.BL,PrXt.BDZ,PrXt.CL,"DeltaZ");

	//position scroll initiale de la vue n&b
	PrXt.PosX=(P.H.SizeX-PrXt.EPro.SizeX)/2.0f;
	PrXt.PosY=(P.H.SizeY-PrXt.EPro.SizeY)/2.0f;
	
	//barres de defilement le cas echeant
	if(PrXt.PosX>0)
		B_CreateHReglage(PrXt.BL,E,0,PrXt.EPro.SizeY,PrXt.EPro.SizeX,&PrXt.PosX,16,0,(P_SizeX(P)-PrXt.EPro.SizeX));
	
	if(PrXt.PosY>0)
		B_CreateVReglage(PrXt.BL,E,PrXt.EPro.SizeX,0,PrXt.EPro.SizeY,&PrXt.PosY,16,0,(P_SizeY(P)-PrXt.EPro.SizeY));

	PrXt.iPosX=-V_FloatToInt(PrXt.PosX);
	PrXt.iPosY=V_FloatToInt(PrXt.EPro.SizeY-P_SizeY(P)+PrXt.PosY);

	B_RedrawButtonsLib(PrXt.BL,0);

	SP_Create(PrXt.SP,(*(PrXt.P)));

	PrXt.Etat=PrXt_OK;
	return -1;
}

void SPG_CONV PrXt_Close(ProfilExtract& PrXt)
{

	SP_Close(PrXt.SP);

	B_CloseButtonsLib(PrXt.BL);
	C_CloseCaracLib(PrXt.CL);

	G_CloseEcran(PrXt.EPro);
	G_CloseEcran(PrXt.EBut);
	G_CloseEcran(PrXt.ECut);

	memset(&PrXt,0,sizeof(ProfilExtract));

	return;
}

void SPG_CONV PrXt_SelectProfil(ProfilExtract& PrXt, Profil& P)
{
	PrXt.P=&P;
	PrXt.PosX=(P.H.SizeX-PrXt.EPro.SizeX)/2.0f;
	PrXt.PosY=(P.H.SizeY-PrXt.EPro.SizeY)/2.0f;
	SP_SelectProfil(PrXt.SP,P);
}

void SPG_CONV PrXt_Update(ProfilExtract& PrXt, int MouseX, int MouseY, int MouseLeft, int MouseRight)
{
	B_UpdateButtonsLib(PrXt.BL,MouseX,MouseY,MouseLeft);
	
	if (PrXt.P)
	{

	if (G_IsInEcran(PrXt.EPro,MouseX,MouseY)&&(PrXt.BL.FocusButton==0))
	{
		//gestion de la selection: add ou del
		if (MouseLeft)
		{
			if(PrXt.WaitMouseRelease==0)
			{
				SP_Add(PrXt.SP,MouseX-PrXt.iPosX,MouseY-PrXt.iPosY);
				PrXt.WaitMouseRelease=-1;
			}
		}
		else if (MouseRight)
		{
			if(PrXt.WaitMouseRelease==0)
			{
				SP_Del(PrXt.SP);
				PrXt.WaitMouseRelease=-1;
			}
		}
		else
		{
			PrXt.WaitMouseRelease=0;
		}
		
	}


		PrXt.iPosX=-V_FloatToInt(PrXt.PosX);
		PrXt.iPosY=V_FloatToInt(PrXt.EPro.SizeY-PrXt.P->H.SizeY+PrXt.PosY);
		P_Draw(*(PrXt.P),PrXt.EPro,PrXt.iPosX,PrXt.iPosY,0x00007F);
		//if(B_IsClick(PrXt.BL,PrXt.BSPro)) P_Save(*(PrXt.P),"Profil2D");
		SP_DrawLarge(PrXt.SP,PrXt.EPro,PrXt.iPosX,PrXt.iPosY);
		if(SP_NumSelPoints(PrXt.SP)>1)
		{
			Cut C;
			SP_ResampleCut(PrXt.SP,C,0.25);
			if(PrXt.SP.NSel)
			{
				int N0=0;
				float Z0=0;
				{
				for(int y=V_Max(PrXt.SP.PosY[0]-2,0);y<V_Min(PrXt.SP.PosY[0]+3,P_SizeY((*(PrXt.P))));y++)
				{
					for(int x=V_Max(PrXt.SP.PosX[0]-2,0);x<V_Min(PrXt.SP.PosX[0]+3,P_SizeX((*(PrXt.P))));x++)
					{
					   Z0+=P_Element((*(PrXt.P)),x,y);
					   N0++;
					}
				}
				}

				int N1=0;
				float Z1=0;
				{
				for(int y=V_Max(PrXt.SP.PosY[PrXt.SP.NSel-1]-2,0);y<V_Min(PrXt.SP.PosY[PrXt.SP.NSel-1]+3,P_SizeY((*(PrXt.P))));y++)
				{
					for(int x=V_Max(PrXt.SP.PosX[PrXt.SP.NSel-1]-2,0);x<V_Min(PrXt.SP.PosX[PrXt.SP.NSel-1]+3,P_SizeX((*(PrXt.P))));x++)
					{
					   Z1+=P_Element((*(PrXt.P)),x,y);
					   N1++;
					}
				}
				}

				if((N0>0)&&(N1>0))
					PrXt.DeltaZ=Z1/N1-Z0/N0;
			}
			if(B_IsChangedToClick(PrXt.BL,PrXt.BSCut)) Cut_Save(C,"Coupe");
			Cut_Draw(C,PrXt.ECut,0,PrXt.CL);
			Cut_Close(C);
		}
	}
	return;
}

#endif
