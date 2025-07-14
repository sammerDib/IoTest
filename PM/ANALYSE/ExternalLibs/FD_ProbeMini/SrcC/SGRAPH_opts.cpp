
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH_OPTS
#ifndef SPG_General_PGLib

#include "SPG_Includes.h"

SG_OPTS* SPG_CONV SG_CreateOpts(SG_PDV& Vue, G_Ecran& E, int AutoUpdate)
{
	SG_OPTS* SO=SPG_TypeAlloc(1,SG_OPTS,"SGRAPH_OPTIONS");
	B_LoadButtonsLib(SO->BL,E,1,0,"..\\SrcC\\Interface","Buttons.bmp");
	C_LoadCaracLib(SO->CL,E,"..\\SrcC\\Carac","CaracNoir.bmp");
	SO->Ecran=E;
	SO->Vue=&Vue;
	SO->AutoUpdate=AutoUpdate;

	int BSpaceX=SO->BL.SizeX[ClickSprite]+SO->CL.SizeX;
	int BSpaceY=SO->BL.SizeY[ClickSprite]+SO->CL.SpaceY+2;
	int CurrentX=2;
	int CurrentY=2*SO->CL.SpaceY;

	SO->ForceFilaire=B_CreateCheckButton(SO->BL,E,CurrentX,CurrentY);
	B_PrintLabel(SO->BL,SO->ForceFilaire,SO->CL,"Force\nFilaire");
	CurrentX+=BSpaceX;
#ifdef SGE_EMC
	SO->ForceUni=B_CreateCheckButton(SO->BL,E,CurrentX,CurrentY);
	B_PrintLabel(SO->BL,SO->ForceUni,SO->CL,"Force\nUni");
#endif
	CurrentX+=BSpaceX;
	SO->NombreB=B_CreateNumericIntButton(SO->BL,E,SO->CL,CurrentX,CurrentY,5,(int*)&Vue.NombreB);
	B_PrintLabel(SO->BL,SO->NombreB,SO->CL,"Blocs");
	CurrentX+=8*SO->CL.SizeX;
	SO->TriFait=B_CreateNumericIntButton(SO->BL,E,SO->CL,CurrentX,CurrentY,5,(int*)&Vue.TriFait);
	B_PrintLabel(SO->BL,SO->TriFait,SO->CL,"Faces");
	/*
	CurrentX+=32;
	SO->TriBlocs=B_CreateCheckButton(SO->BL,E,CurrentX,CurrentY);
	B_PrintLabel(SO->BL,SO->TriBlocs,SO->CL,"Tri\nBlocs");
	CurrentX+=32;
	SO->NoTriFaces=B_CreateCheckButton(SO->BL,E,CurrentX,CurrentY);
	B_PrintLabel(SO->BL,SO->NoTriFaces,SO->CL,"NoTri\nFaces");
	*/
	CurrentX+=8*SO->CL.SizeX;
	if ((CurrentX+BSpaceX)>=E.SizeX)
	{
		CurrentX=2;
		CurrentY+=BSpaceY;
	}
#ifdef SGE_EMC
	SO->NoClearSky=B_CreateCheckButton(SO->BL,E,CurrentX,CurrentY);
	B_PrintLabel(SO->BL,SO->NoClearSky,SO->CL,"NoClear\nSky");
#endif
	/*
	CurrentX+=32;
	SO->UseGdi=B_CreateCheckButton(SO->BL,E,CurrentX,CurrentY);
	B_PrintLabel(SO->BL,SO->UseGdi,SO->CL,"Use\nGDI");
	*/

	/*
	CurrentX+=32;
	if ((CurrentX+128)>=E.SizeX)
	{
		CurrentX=8;
		CurrentY+=4+(SO->BL.SizeY[1]);
	}
	*/

		CurrentX=2;
		CurrentY+=BSpaceY;
	SO->DistMax=B_CreateHReglageCliquableNumeric(SO->BL,E,SO->CL,CurrentX,CurrentY,E.SizeX-8-CurrentX,4,&Vue.DMax,Vue.DMax/64,0,3*Vue.DMax);
	B_PrintLabel(SO->BL,SO->DistMax-1,SO->CL,"DistMax");
		CurrentY+=BSpaceY;
	SO->DistTex=B_CreateHReglageCliquableNumeric(SO->BL,E,SO->CL,CurrentX,CurrentY,E.SizeX-8-CurrentX,4,&Vue.DTex,Vue.DMax/64,0,3*Vue.DMax);
	B_PrintLabel(SO->BL,SO->DistTex-1,SO->CL,"DistTex");
		CurrentY+=BSpaceY;
#ifdef SGE_EMC
	SO->DistFog=B_CreateHReglageCliquableNumeric(SO->BL,E,SO->CL,CurrentX,CurrentY,E.SizeX-8-CurrentX,4,&Vue.DFog,Vue.DMax/64,0,3*Vue.DMax);
	B_PrintLabel(SO->BL,SO->DistFog-1,SO->CL,"DistFog");
#endif

	B_RedrawButtonsLib(SO->BL,0);

	if(SO->AutoUpdate) SPG_AddUpdateOnDoEvents((SPG_CALLBACK)SG_UpdateOpts,SO,0);

	return SO;
}

void SPG_CONV SG_UpdateOpts(SG_OPTS* SO)
{
	if (SO) 
	{
		B_UpdateButtonsLib(SO->BL,SPG_Global_MouseX,SPG_Global_MouseY,SPG_Global_MouseLeft);
		if (B_IsClick(SO->BL,SO->ForceFilaire))
		{
#ifdef SGE_EMC
			SO->Vue->RenderMode&=~SGR_NORMAL;
			SO->Vue->RenderMode|=SGR_FILAIRE;
			SO->Vue->RenderMode&=~SGR_UNI;
#else
			SO->Vue->Filaire=1;
#endif
		}
		else
		{
#ifdef SGE_EMC
			if (B_IsClick(SO->BL,SO->ForceUni))
			{
			SO->Vue->RenderMode&=~SGR_NORMAL;
			SO->Vue->RenderMode&=~SGR_FILAIRE;
			SO->Vue->RenderMode|=SGR_UNI;
			}
			else
			{
				SO->Vue->RenderMode|=SGR_NORMAL;
				SO->Vue->RenderMode&=~SGR_FILAIRE;
				SO->Vue->RenderMode&=~SGR_UNI;
			}
#else
			SO->Vue->Filaire=0;
#endif
		}
		/*
		if (B_IsClick(SO->BL,SO->TriBlocs))
		{
				SO->Vue->RenderMode|=SGR_BLOC_TRI;
		}
		else
		{
				SO->Vue->RenderMode&=~SGR_BLOC_TRI;
		}
		if (B_IsClick(SO->BL,SO->NoTriFaces))
		{
				SO->Vue->RenderMode|=SGR_FACE_NOTRI;
		}
		else
		{
				SO->Vue->RenderMode&=~SGR_FACE_NOTRI;
		}
		*/
#ifdef SGE_EMC
		if (B_IsClick(SO->BL,SO->NoClearSky))
		{
				SO->Vue->RenderMode|=SGR_SKY_NOCLEAR;
		}
		else
		{
				SO->Vue->RenderMode&=~SGR_SKY_NOCLEAR;
		}
#endif
		/*
		if (B_IsClick(SO->BL,SO->UseGdi))
		{
				SO->Vue->Ecran.Etat|=G_ECRAN_USEGDI;
		}
		else
		{
				SO->Vue->Ecran.Etat&=~G_ECRAN_USEGDI;
		}
		*/

	}
	return;
}

void SPG_CONV SG_CloseOpts(SG_OPTS* SO)
{
	if (SO) 
	{
		if(SO->AutoUpdate) SPG_KillUpdateOnDoEvents((SPG_CALLBACK)SG_UpdateOpts,SO);
		B_CloseButtonsLib(SO->BL);
		C_CloseCaracLib(SO->CL);
		SPG_MemFree(SO);
	}
	return;
}

#else
#pragma SPGMSG(__FILE__,__LINE__,"SGRAPH_opts incompatible avec PGLib")
#endif
#endif

