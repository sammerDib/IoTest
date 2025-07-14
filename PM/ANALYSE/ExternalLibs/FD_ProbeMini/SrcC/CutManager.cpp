
#include "SPG_General.h"

#ifdef SPG_General_USECut

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#ifdef SPG_General_USEPRO
#include "PRO_Loader.h"
#define CUT_PRO 512
#endif

#include <stdio.h>
#include <string.h>
#include <float.h>
//#include <ctype.h>

//cree un profil 1D (float)
int SPG_CONV Cut_Create(Cut& C, int Len, float XScale, char* UnitX,char* UnitY)
{
	memset(&C,0,sizeof(Cut));
	CHECK(Len<=0,"Cut_Create: Longueur invalide",return 0);
	IF_CD_G_CHECK(24,return 0);
	C.NumS=Len;
	C.D=SPG_TypeAlloc(C.NumS,float,"CoupeD");
	CHECK(C.D==0,"Cut_Create: Allocation memoire echouee",return 0);
	C.Msk=SPG_TypeAlloc(C.NumS,BYTE,"CoupeMsk");
	CHECK(C.Msk==0,"Cut_Create: Allocation memoire echouee",Cut_Close(C);return 0);
	C.Decor=SPG_TypeAlloc(C.NumS,BYTE,"DecorMsk");
	CHECK(C.Decor==0,"Cut_Create: Allocation memoire echouee",Cut_Close(C);return 0);
	C.XScale=XScale;
	if (UnitX) strncpy(C.UnitX,UnitX,MaxUnit-1);
	if (UnitY) strncpy(C.UnitY,UnitY,MaxUnit-1);
	C.Etat=Cut_WithMEM;
	return -1;
}

int SPG_CONV Cut_Duplicate(Cut& C, Cut& Origin)
{
	CHECK(Cut_Init(C,Origin.NumS,Origin.D,Origin.Msk,Origin.Decor,Cut_WithMEM)==0,"Cut_Duplicate",return 0);
	Cut_SetScale(C,Origin.XScale,Origin.UnitX,Origin.UnitY);
	return -1;
}

#ifdef SPG_General_USEFFT
//cree un profil 1D complexe vide
int SPG_CONV CutX_Create(CutX& C, int Len, float XScale, char*UnitX,char*UnitY)
{
	CHECK(Cut_Create((Cut&)C,2*Len,XScale,UnitX,UnitY)==0,"CutX_Create: Cut_Create echoue",return 0);
	SPG_CatMemName(C.D,"X");
	C.NumS=Len;
	C.Etat|=Cut_COMPLEX;
	return -1;
}

//duplique un profil 1D complexe
int SPG_CONV CutX_Duplicate(CutX& CSrc, CutX& NewCut)
{
	return CutX_Init(NewCut,CSrc.NumS,CSrc.D,0,0,Cut_WithMEM);
}

//cree un profil 1D complexe avec les donnees existantes
int SPG_CONV CutX_Init(CutX& C, int Len, SPG_COMPLEX * D, BYTE * Msk, BYTE * Decor, int Mode)
{
	memset(&C,0,sizeof(CutX));

	if (Mode==Cut_WithMEM)
	{
		CutX_Create(C,Len,1,"","");
		if (C.Etat)
		{
		if (D) SPG_Memcpy(C.D,D,C.NumS*sizeof(SPG_COMPLEX));
		if (Msk) SPG_Memcpy(C.Msk,Msk,C.NumS*sizeof(BYTE));
		if (Decor) SPG_Memcpy(C.Decor,Decor,C.NumS*sizeof(BYTE));
		return -1;
		}
		else
			return 0;
	}
	else if (Mode==Cut_WithThisMEM)
	{
		C.Etat=Cut_WithMEM|Cut_COMPLEX;//lors du cut_close ce sera desalloue
		C.D=D;
		C.Msk=Msk;
		C.Decor=Decor;
		C.NumS=Len;
		C.XScale=1;
		C.UnitX[0]=0;
		C.UnitY[0]=0;
		return -1;
	}
	else  if (Mode==Cut_Alias)
	{
		C.Etat=Cut_Alias|Cut_COMPLEX;//lors du cut_close ce ne sera pas desalloue
		C.D=D;
		C.Msk=Msk;
		C.Decor=Decor;
		C.NumS=Len;
		C.XScale=1;
		C.UnitX[0]=0;
		C.UnitY[0]=0;
		return -1;
	}
#ifdef DebugList
	else
		SPG_List("Cut_Init: Mode invalide");
#endif
	return 0;
}

#endif

//cree un profil 1D float avec les donnees existantes
int SPG_CONV Cut_Init(Cut& C, int Len, float * D, BYTE * Msk, BYTE * Decor, int Mode)
{
	memset(&C,0,sizeof(C));
	IF_CD_G_CHECK(31,return 0);

	if (Mode==Cut_WithMEM)
	{
		Cut_Create(C,Len,1,"","");
		if (C.Etat)
		{
		if (D) SPG_Memcpy(C.D,D,C.NumS*sizeof(float));
		if (Msk) SPG_Memcpy(C.Msk,Msk,C.NumS*sizeof(BYTE));
		if (Decor) SPG_Memcpy(C.Decor,Decor,C.NumS*sizeof(BYTE));
		return -1;
		}
		else
			return 0;
	}
	else if (Mode==Cut_WithThisMEM)
	{
		C.Etat=Cut_WithMEM;
		C.D=D;
		C.Msk=Msk;
		C.Decor=Decor;
		C.NumS=Len;
		C.XScale=1;
		C.UnitX[0]=0;
		C.UnitY[0]=0;
		return -1;
	}
	else  if (Mode==Cut_Alias)
	{
		C.Etat=Cut_Alias;
		C.D=D;
		C.Msk=Msk;
		C.Decor=Decor;
		C.NumS=Len;
		C.XScale=1;
		C.UnitX[0]=0;
		C.UnitY[0]=0;
		return -1;
	}
#ifdef DebugList
	else
		SPG_List("Cut_Init: Mode invalide");
#endif
	return 0;
}

//copie un profil sur un autre, CopyExt=(Cut_CopyMsk=copie aussi les masques)|(Cut_CopyDecor=copie aussi les decorations)
int SPG_CONV Cut_Copy(Cut& CDest, Cut& CSrc, int CopyExt)
{
	IF_CD_G_CHECK(27,return 0);
	CHECK(CDest.Etat==0,"Cut_Copy: Coupe destination nulle",return 0);
	CHECK(CSrc.Etat==0,"Cut_Copy: Coupe source nulle",return 0);
	CHECK(CDest.D==0,"Cut_Copy: Coupe destination nulle",return 0);
	CHECK(CSrc.D==0,"Cut_Copy: Coupe source nulle",return 0);
	CHECK(CDest.NumS<CSrc.NumS,"Cut_Copy: Destination trop petite",return 0);
	SPG_Memcpy(CDest.D,CSrc.D,CSrc.NumS*sizeof(float));
	if((CopyExt&Cut_CopyMsk)&&(CDest.Msk)&&(CSrc.Msk)) SPG_Memcpy(CDest.Msk,CSrc.Msk,CSrc.NumS*sizeof(BYTE));
	if((CopyExt&Cut_CopyDecor)&&(CDest.Decor)&&(CSrc.Decor)) SPG_Memcpy(CDest.Decor,CSrc.Decor,CSrc.NumS*sizeof(BYTE));
	Cut_CopyScale(CDest,CSrc);
	return -1;
}

#ifdef SPG_General_USEFFT
//cree une coupe complexe de taille FFTisable, FFT_FLAG=taille voulue (FFT_LOWER,FFT_UPPER,... dans fft.h)
int SPG_CONV Cut_CreateForFFT(Cut& C, CutX& CFFT, int FFT_FLAG)
{
	memset(&CFFT,0,sizeof(CutX));
	IF_CD_G_CHECK(6,return 0);
	CHECK(C.Etat==0,"Coupe invalide",return 0);
	CFFT.NumS=SFFT_GetAppropriateSize(C.NumS,FFT_FLAG);
	char UnitX[2*MaxUnit];
	strcpy(UnitX,"rad/");
	strncat(UnitX,C.UnitX,MaxUnit);
	CHECK(CutX_Create(CFFT,CFFT.NumS,(float)V_2PI/(CFFT.NumS*C.XScale),UnitX,C.UnitY)==0,"Cut_CreateForFFT: CutX_Create echoue",return 0);
	int LDef=(CFFT.NumS-C.NumS)>>1;
	memset(CFFT.Msk,1,LDef*sizeof(BYTE));
	memset(CFFT.Msk+LDef+C.NumS,1,(CFFT.NumS-LDef)*sizeof(BYTE));
	return -1;
}
#endif


void SPG_CONV Cut_SetScale(Cut& C, float XScale, char*UnitX,char*UnitY)
{
	if(C.Etat==0) return;
	C.XScale=XScale;
	if (UnitX) strncpy(C.UnitX,UnitX,MaxUnit-1);
	if (UnitY) strncpy(C.UnitY,UnitY,MaxUnit-1);
	return;
}

void SPG_CONV Cut_Close(Cut& C)
{
	//C.NumS=0;
	if (C.Etat&Cut_WithMEM)
	{
	DbgCHECK(C.D==0,"Cut_Close: Cut nul");
	if (C.D)
		SPG_MemFree(C.D);
	if (C.Msk)
		SPG_MemFree(C.Msk);
	if (C.Decor)
		SPG_MemFree(C.Decor);
	}
	/*
	C.D=0;
	C.Msk=0;
	C.Etat=0;
	*/
	memset(&C,0,sizeof(Cut));
}

#ifdef SPG_General_USEFFT

void SPG_CONV CutX_SetScale(CutX& C, float XScale, char*UnitX,char*UnitY)
{
	C.XScale=XScale;
	if (UnitX) strncpy(C.UnitX,UnitX,MaxUnit-1);
	if (UnitY) strncpy(C.UnitY,UnitY,MaxUnit-1);
	return;
}

void SPG_CONV CutX_Close(CutX& C)
{
	//C.NumS=0;
	if (C.Etat&Cut_WithMEM)
	{
	DbgCHECK(C.D==0,"CX_Close: Cut nul");
	if (C.D)
		SPG_MemFree(C.D);
	if (C.Msk)
		SPG_MemFree(C.Msk);
	if (C.Decor)
		SPG_MemFree(C.Decor);
	}
	/*
	C.D=0;
	C.Msk=0;
	C.Etat=0;
	*/
	memset(&C,0,sizeof(CutX));
}
#endif


#ifdef SPG_General_USECarac
/*
void SPG_CONV SPG_RoundHiDec(float& fMax, float fPartMax, int LogPart)
{
	if(fPartMax<=1.0f) 
		fMax=V_Signe(fMax)*powfInt(10,(float)LogPart);
	else if (fPartMax<=2.0f)
		fMax=V_Signe(fMax)*2*powfInt(10,LogPart);
	else if (fPartMax<=3.0f)
		fMax=V_Signe(fMax)*3*powfInt(10,LogPart);
	else if (fPartMax<=4.0f)
		fMax=V_Signe(fMax)*4*powfInt(10,LogPart);
	else if (fPartMax<=5.0f)
		fMax=V_Signe(fMax)*5*powfInt(10,LogPart);
	else if (fPartMax<=6.0f)
		fMax=V_Signe(fMax)*6*powfInt(10,LogPart);
	else if (fPartMax<=8.0f)
		fMax=V_Signe(fMax)*8*powfInt(10,LogPart);
	else
		fMax=V_Signe(fMax)*10*powfInt(10,LogPart);
	return;
}

void SPG_CONV SPG_RoundLowDec(float& fMin, float fPartMin, int LogPart)
{
	if(fPartMin<1.0f) 
		fMin=0;
	else if (fPartMin<2.0f)
		fMin=V_Signe(fMin)*powfInt(10,LogPart);
	else if (fPartMin<4.0f)
		fMin=V_Signe(fMin)*2*powfInt(10,LogPart);
	else if (fPartMin<5.0f)
		fMin=V_Signe(fMin)*4*powfInt(10,LogPart);
	else
		fMin=V_Signe(fMin)*5*powfInt(10,LogPart);
	return;
}

void SPG_CONV SPG_Scaler(float& fMin, float& fMax)
{
	if((fMin==0)&&(fMax==0)) return;

	if ((fMin>0)&&(fMax>0))
	{
		if ((3*fMin)<fMax)
		{//force zero en bas
			fMin=0;
		}
		else if ((fMax+fMin)<3*fMin)
		{//travaille avec un offset
			float Offset=fMin;
			int LogPart=V_Floor(log10(fabs(Offset)));
			int fPart=V_Floor(Offset/(powfInt(10,LogPart-1)));
			Offset=fPart*powfInt(10,LogPart-1);
			float fOMin=fMin-Offset;
			float fOMax=fMax-Offset;
			SPG_Scaler(fOMin,fOMax);
			fMin=fOMin+Offset;
			fMax=fOMax+Offset;
			return;
		}
	}
	if ((fMin<0)&&(fMax<0))
	{
		if ((3*fMax)>fMin)
		{//force zero en haut
			fMax=0;
		}
		else if ((fMax+fMin)>3*fMin)
		{//travaille avec un offset
			float Offset=fMax;
			int LogPart=V_Floor(log10(fabs(Offset)));
			int fPart=V_Floor(Offset/(powfInt(10,LogPart-1)));
			Offset=fPart*powfInt(10,LogPart-1);
			float fOMin=fMin-Offset;
			float fOMax=fMax-Offset;
			SPG_Scaler(fOMin,fOMax);
			fMin=fOMin+Offset;
			fMax=fOMax+Offset;
			return;
		}
	}
	if ((fMin<0)&&(fMax>0))
	{
		float t;
		if(
				(
				t=V_Max(fMax,(-fMin))
				)<
				2*(V_Min(fMax,(-fMin)))
			)
		{
			fMax=t;
			fMin=-t;
		}
	}

	int LogPart;
	if(fMax!=0)
	{
		if(fMin!=0)
		{
			int LogfMax=V_Floor(log10(fabs(fMax)));
			int LogfMin=V_Floor(log10(fabs(fMin)));
			LogPart=V_Max(LogfMax,LogfMin);
		}
		else
		{
			LogPart=V_Floor(log10(fabs(fMax)));
		}
	}
	else
	{
		if(fMin!=0)
		{
			LogPart=V_Floor(log10(fabs(fMin)));
		}
		else
			return;//cas impossible
	}

	float fPartMax=fabs(fMax)/(powfInt(10,LogPart));
	float fPartMin=fabs(fMin)/(powfInt(10,LogPart));

	if(fMax>=0) 
		SPG_RoundHiDec(fMax,fPartMax,LogPart);
	else 
		SPG_RoundLowDec(fMax,fPartMax,LogPart);
	if(fMin>=0)
		SPG_RoundLowDec(fMin,fPartMin,LogPart);
	else
		SPG_RoundHiDec(fMin,fPartMin,LogPart);

	return;
}
*/

//trace generique, (ne pas appeler directement, il existe des fonctions d'interface simplifiees)
void SPG_CONV Cut_Draw_General(Cut& C, G_Ecran& E, DWORD Couleur, C_Lib& CL, int ClearScreen, float fMin, float fMax, int DrawScale, int DrawLabels)
{
	CHECK(C.Etat==0,"Cut_Draw: Cut invalide",return);
	CHECK(E.Etat==0,"Cut_Draw: Ecran invalide",return);
	IF_CD_G_CHECK(28,return);

	if(ClearScreen)
	{
	G_DrawRect(E,
		0,0,
		E.SizeX,E.SizeY,
		0xffffffff);
	G_DrawOutRect(E,
		0,0,
		E.SizeX-1,E.SizeY-1,
		0);
	}

	if(fMin==fMax)
		Cut_FindMinMax(C,fMin,fMax);
	if (fMin==fMax) return;

	int MaxTickX=E.SizeX/(10*V_Max(CL.SizeX,4));	int MaxTickY=E.SizeY/(3*V_Max(CL.SpaceY,4)); SPG_Scale SCX,SCY;
	SC_Init(SCX,MaxTickX,1);	SC_Init(SCY,MaxTickY,0);
	SC_Set(SCY,fMin,fMax);		SC_Set(SCX,0,(C.NumS-1)*C.XScale);
	
	SCX.ScaleMin=SCX.fMin;SCX.ScaleMax=SCX.fMax;

	if(DrawScale)
	{
		if(SCX.ScaleMax>SCX.ScaleMin)
		{
			int Flag=YDN|XLEFT;
			for(int i=0;i<SCX.NumTick;i++)
			{
				int XLine=V_FloatToInt(SCX.T[i].Value*(G_SizeX(E)-1)/(SCX.ScaleMax-SCX.ScaleMin));
				G_DrawLine(E,XLine,0,XLine,E.SizeY-1,0);
				if(i>=SCX.NumTick/2) Flag|=XRIGHT;
				C_Print(E,XLine+((Flag&XRIGHT)?-2:2),G_SizeY(E)-3,SCX.T[i].Label,CL,Flag);
			}
		}

		if(SCY.ScaleMax>SCY.ScaleMin)
		{
			int Flag=XLEFT|YDN;
			for(int i=0;i<SCY.NumTick;i++)
			{
				int YLine=V_FloatToInt((SCY.ScaleMax-SCY.T[i].Value)*(G_SizeY(E)-1)/(SCY.ScaleMax-SCY.ScaleMin));
				G_DrawLine(E,0,YLine,E.SizeX-1,YLine,0);
				if(i>=SCY.NumTick/2) Flag&=~YDN;
				C_Print(E,2,YLine+((Flag&YDN)?-2:2),SCY.T[i].Label,CL,Flag);
			}
		}
	}

	DWORD CBlend=((Couleur>>1)&0x7F7F7F) + 0x7F7F7F;// 0xffffff-((0xfcfcfc-(Couleur&0xfcfcfc))>>2);
	float xFact=(E.SizeX-1)/(float)(V_Max(C.NumS,2)-1);
	float yFact=0;
	if(SCY.ScaleMax>SCY.ScaleMin) yFact=1.0f/(SCY.ScaleMax-SCY.ScaleMin)*(float)(E.SizeY-1);
	if(C.NumS<2*E.SizeX)
	{
		for(int i=0;i<C.NumS;i++)
		{
			if	(
					(i<(C.NumS-1))&&
					(
						(C.Msk==0)||
						(
							(C.Msk[i]==0)&&
							(C.Msk[i+1]==0)
						)
					)
				)//attention a l'ordre d'evaluation dans le if
			{
				G_DrawLineInt(E,
					i*xFact,
					(SCY.ScaleMax-C.D[i])*yFact,
					(i+1)*xFact,
					(SCY.ScaleMax-C.D[(i+1)])*yFact,
					Couleur);
			}

			if ((C.Decor)&&C.Decor[i])
			{
			G_DrawPixelInt(E,i*xFact-1,
				(SCY.ScaleMax-C.D[i])*yFact,Couleur^0xff0000);
			G_DrawPixelInt(E,i*xFact,
				(SCY.ScaleMax-C.D[i])*yFact,Couleur^0xff0000);
			G_DrawPixelInt(E,i*xFact+1,
				(SCY.ScaleMax-C.D[i])*yFact,Couleur^0xff0000);
			G_DrawPixelInt(E,i*xFact,
				(SCY.ScaleMax-C.D[i])*yFact-1,Couleur^0xff0000);
			G_DrawPixelInt(E,i*xFact,
				(SCY.ScaleMax-C.D[i])*yFact+1,Couleur^0xff0000);
			}

			if ((C.Msk)&&(C.Msk[i]))
			{
				G_DrawPixelInt(E,i*xFact,
					(SCY.ScaleMax-C.D[i])*yFact,Couleur^0xff8080);
			}
		}
	}
	else
	{
		int i=0;
		int DrawLine=0;
		int LastY=0;
		int IntervalMeanLen=C.NumS/E.SizeX;
		const int Step=1+IntervalMeanLen/64;
		for(int x=0;x<E.SizeX;x++)
		{
			const int IntervalEnd=((__int64)(x+1)*(__int64)C.NumS)/(__int64)E.SizeX;
			//BYTE Msk=0;
			BYTE Decor=0;
			float Min,Max;
			Max=Min=C.D[i];
			float Moy=0;
			int Count=0;
			if((C.Msk==0)&&(C.Decor==0))
			{
				for(;i<IntervalEnd;i+=Step)
				{
					float cd=C.D[i];
					Count++;
					Moy+=cd;
					if(cd<Min) Min=cd;
					if(cd>Max) Max=cd;
				}
			}
			else
			{
				for(;i<IntervalEnd;i+=Step)
				{
					if((C.Msk==0)||(C.Msk[i]==0))
					{
						float cd=C.D[i];
						if(cd<Min) Min=cd;
						if(cd>Max) Max=cd;
						Moy+=cd;
						Count++;
					}
					if(C.Decor) Decor|=C.Decor[i];
				}
			}
			if(Count==0)
			{
				G_DrawPixelInt(E,x,(SCY.ScaleMax-Min)*yFact,Couleur^0xff8080);
				DrawLine=0;
			}
			else
			{
				//G_DrawLineInt(E,x,(SCY.ScaleMax-Min)*yFact,x,(SCY.ScaleMax-Max)*yFact,CBlend);
				BYTE* pE=G_MECR(E)+x*G_POCT(E);
				for(int y = V_Max(0,(int)((SCY.ScaleMax-Max)*yFact)) ; y <= V_Min(G_SizeY(E)-1,(int)((SCY.ScaleMax-Min)*yFact)); y++)
				{
					DWORD& C=*(DWORD*)(pE+y*G_Pitch(E));
					C=((3*(C&0xFCFCFC))+(Couleur&0xFCFCFC))>>2;
				}
				int Y=(SCY.ScaleMax-Moy/Count)*yFact;
				if(DrawLine) 
				{
					G_DrawLineInt(E,x-1,LastY,x,Y,Couleur);
				}
				else
				{
					G_DrawPixelInt(E,x,Y,Couleur);
				}
				LastY=Y;
				DrawLine=1;
				//G_DrawPixelInt(E,x,(SCY.ScaleMax-Moy/Count)*yFact,Couleur);
				//G_DrawPixelInt(E,x,(SCY.ScaleMax-Min)*yFact,Couleur^0x808080);
				//G_DrawPixelInt(E,x,(SCY.ScaleMax-Max)*yFact,Couleur^0x808080);
				
				if(Decor)
				{
					G_DrawPixelInt(E,x,
						(SCY.ScaleMax-Moy/Count)*yFact,Couleur^0xff0000);
					G_DrawPixelInt(E,x,
						(SCY.ScaleMax-Moy/Count)*yFact,Couleur^0xff0000);
					G_DrawPixelInt(E,x,
						(SCY.ScaleMax-Moy/Count)*yFact,Couleur^0xff0000);
					G_DrawPixelInt(E,x,
						(SCY.ScaleMax-Moy/Count)*yFact-1,Couleur^0xff0000);
					G_DrawPixelInt(E,x,
						(SCY.ScaleMax-Moy/Count)*yFact+1,Couleur^0xff0000);
				}
				
			}
		}
	}
	if(DrawLabels)
	{
		char Msg[512];
		char XMin[16];
		char XMax[16];
		char YMin[16];
		char YMax[16];
		char DeltaY[16];
		XMin[0]=0;XMax[0]=0;YMin[0]=0;YMax[0]=0;DeltaY[0]=0;
		//CF_GetString(XMin,XfMin,5);	
		float XfMax=C.NumS*C.XScale;
		float YfMin,YfMax;
		Cut_FindMinMax(C,YfMin,YfMax);
		CF_GetString(XMax,XfMax,5);
		CF_GetString(YMin,YfMin,5);	CF_GetString(YMax,YfMax,5);
		CF_GetString(DeltaY,YfMax-YfMin,5);
		sprintf(Msg,"X: %s%s (%i samples)\nY: Min:%s%s Max:%s%s Delta:%s%s",
			XMax,C.UnitX,C.NumS,YMin,C.UnitY,YMax,C.UnitY,DeltaY,C.UnitY);
//#ifndef CHROMATIC
//		C_PrintUni(E,E.SizeX/4,7*E.SizeY/64+DrawLabels*2*CL.SpaceY,Msg,CL,0,Couleur);
//#else
		/* Rajouts MA pour permettre l'affichage multi echelle*/
		//C_PrintUni(E,E.SizeX/4,7*E.SizeY/64+DrawLabels*2*CL.SpaceY  + (DrawScale - 1) * 30,Msg,CL,0,Couleur);
		C_PrintUni(E,E.SizeX/4,7*E.SizeY/64 +DrawLabels*2*CL.SpaceY  + (DrawScale-1)*6*CL.SpaceY ,Msg,CL,0,Couleur);
		/* Fin de rajouts MA*/
//#endif
		//C_PrintWithBorder(E,E.SizeX/4,E.SizeY/8+DrawLabels*2*CL.SpaceY,Msg,CL,0,Couleur);
	}
	return;
}

#define Cut_Draw_GeneralY Cut_Draw_General

/*
void SPG_CONV Cut_Draw_GeneralY(Cut& C, G_Ecran& E, DWORD Couleur, C_Lib& CL, int ClearScreen, float fMin, float fMax)
{
	CHECK(C.Etat==0,"Cut_Draw: Cut invalide",return);
	CHECK(E.Etat==0,"Cut_Draw: Ecran invalide",return);
	IF_CD_G_CHECK(28,return);

	if(ClearScreen)
	{
	G_DrawRect(E,
		0,0,
		E.SizeX,E.SizeY,
		0xffffffff);
	G_DrawOutRect(E,
		0,0,
		E.SizeX-1,E.SizeY-1,
		0);
	}


	if(fMin==fMax)
		Cut_FindMinMax(C,fMin,fMax);

	if (fMin==fMax) return;

	SPG_Scaler(fMin,fMax);

	int Marge=2;
	char Label[256];
	int i;
	int NumPasY=5;
	for(i=1;i<=NumPasY;i++)
	{
	G_DrawLine(E,
		0,
		V_FloatToInt(i*(E.SizeY-1)/NumPasY),
		E.SizeX-1,
		V_FloatToInt(i*(E.SizeY-1)/NumPasY),
		0);
	Label[0]=0;
	CF_GetString(Label,i*C.XScale*C.NumS/NumPasY,5);
	if (i==NumPasY) strcat(Label,C.UnitX);
	C_Print(E,E.SizeX-1,V_FloatToInt(i*(E.SizeY-1)/NumPasY)-Marge,Label,CL,FONT_TRANSP|YUP|XRIGHT);
	}

	int NumPasX=3;
	for(i=0;i<=NumPasX;i++)
	{
	G_DrawLine(E,
		V_FloatToInt(i*(E.SizeX-1)/NumPasX),
		0,
		V_FloatToInt(i*(E.SizeX-1)/NumPasX),
		E.SizeY-1,
		0
		);
	//sprintf(Label,"%.3g",i*(fMax-fMin)/10+fMin);
	Label[0]=0;
	CF_GetString(Label,i*(fMax-fMin)/NumPasX+fMin,5);
	if(i==NumPasX) 
	{
		strcat(Label,"\n'");
		strcat(Label,C.UnitY);
		strcat(Label,"'");
	}
	if (i) 
		C_Print(E,E.SizeX-1-i*(E.SizeX-1)/NumPasX+Marge,Marge,Label,CL,FONT_TRANSP);
	else
		C_Print(E,E.SizeX-1-Marge-CL.SizeY,Marge,Label,CL,FONT_TRANSP);
	}

	float xFact=(E.SizeY-1)/(float)(C.NumS-1);
	float yFact=1.0f/(fMax-fMin)*(float)(E.SizeX-1);
	for(i=0;i<C.NumS;i++)
	{
		if	(
				(i<(C.NumS-1))&&
				(
					(C.Msk==0)||
					(
						(C.Msk[i]==0)&&
						(C.Msk[i+1]==0)
					)
				)
			)//attention a l'ordre d'evaluation dans le if
		{
			G_DrawLineInt(E,
				(fMax-C.D[i])*yFact,
				i*xFact,
				(fMax-C.D[(i+1)])*yFact,
				(i+1)*xFact,
				Couleur);
		}
//
		if ((C.Decor)&&C.Decor[i])
		{
		G_DrawPixel(E,i*xFact-1,
			(fMax-C.D[i])*yFact,Couleur^0xff0000);
		G_DrawPixel(E,i*xFact,
			(fMax-C.D[i])*yFact,Couleur^0xff0000);
		G_DrawPixel(E,i*xFact+1,
			(fMax-C.D[i])*yFact,Couleur^0xff0000);
		G_DrawPixel(E,i*xFact,
			(fMax-C.D[i])*yFact-1,Couleur^0xff0000);
		G_DrawPixel(E,i*xFact,
			(fMax-C.D[i])*yFact+1,Couleur^0xff0000);
		}

		if ((C.Msk)&&(C.Msk[i]))
		{
		G_DrawPixel(E,i*xFact,
			(fMax-C.D[i])*yFact,Couleur&0x555555);
		}
//
	}
	return;
}
*/
//trace un profil en mode tout automatique
void SPG_CONV Cut_Draw(Cut& C, G_Ecran& E, DWORD Couleur, C_Lib& CL)
{
	float fMin=0; float fMax=0;
	Cut_FindMinMax(C,fMin,fMax);
	Cut_Draw_General(C,E,Couleur,CL,1,fMin,fMax);
	return;
}

//trace un profil en mode tout automatique
void SPG_CONV Cut_DrawY(Cut& C, G_Ecran& E, DWORD Couleur, C_Lib& CL)
{
	float fMin,fMax;
	Cut_FindMinMax(C,fMin,fMax);
	Cut_Draw_GeneralY(C,E,Couleur,CL,1,fMin,fMax);
	return;
}
//trace un profil en mode tout automatique
void SPG_CONV Cut_DrawScaled(Cut& C, G_Ecran& E, DWORD Couleur, C_Lib& CL, float fMin, float fMax)
{
	Cut_Draw_General(C,E,Couleur,CL,1,fMin,fMax);
	return;
}

//trace une liste de profils
//multiscale=echelles Y dissociees
void SPG_CONV Cut_DrawList(Cut** CList, G_Ecran& E, DWORD* CouleurList, C_Lib& CL,int NList, int MultiScale, float fMin, float fMax)
{
	if(fMin==fMax)
	{
		fMin=FLT_MAX;
		fMax=-FLT_MAX;

		if(MultiScale==0)
		{
			for(int nc=0;nc<NList;nc++)
			{
				Cut C=*(CList[nc]);
				float lMin=FLT_MAX;
				float lMax=-FLT_MAX;
				Cut_FindMinMax(C,lMin,lMax);
				fMin=V_Min(lMin,fMin);
				fMax=V_Max(lMax,fMax);
			}
		}
	}//si fMin!=fMax on prend une echelle fixe telle que recue en parametre
	else
	{
		DbgCHECK(MultiScale!=0,"Cut_DrawList: fMin<>fMax (ignored)");
	}

	for(int nc=0;nc<NList;nc++)
	{
		if(MultiScale)
		{//prend une echelle courbe par courbe
			Cut_FindMinMax(*CList[nc],fMin,fMax);
		}
		Cut_Draw_General(*CList[nc],E,CouleurList[nc],CL,nc==0,fMin,fMax,nc+1);
	}
	return;
}

void SPG_CONV Cut_Draw2(Cut& C1, Cut& C2, G_Ecran& E, DWORD Couleur1, DWORD Couleur2, C_Lib& CL)
{
	Cut* LCut[2]={&C1,&C2};
	DWORD LCol[2]={Couleur1,Couleur2};
	Cut_DrawList(LCut,E,LCol,CL,2,0);
	return;
}

void SPG_CONV Cut_Draw2MultiScale(Cut& C1, Cut& C2, G_Ecran& E, DWORD Couleur1, DWORD Couleur2, C_Lib& CL, float fMin, float fMax)
{
	Cut* LCut[2]={&C1,&C2};
	DWORD LCol[2]={Couleur1,Couleur2};
	Cut_DrawList(LCut,E,LCol,CL,2,1,fMin,fMax);
	return;
}

void SPG_CONV Cut_Draw3(Cut& C1, Cut& C2, Cut& C3, G_Ecran& E, DWORD Couleur1, DWORD Couleur2, DWORD Couleur3, C_Lib& CL)
{
	Cut* LCut[3]={&C1,&C2,&C3};
	DWORD LCol[3]={Couleur1,Couleur2,Couleur3};
	Cut_DrawList(LCut,E,LCol,CL,3,0);
	return;
}

void SPG_CONV Cut_Draw3Scaled(Cut& C1, Cut& C2, Cut& C3, G_Ecran& E, DWORD Couleur1, DWORD Couleur2, DWORD Couleur3, C_Lib& CL, float fMin, float fMax)
{
	Cut* LCut[3]={&C1,&C2,&C3};
	DWORD LCol[3]={Couleur1,Couleur2,Couleur3};
	Cut_DrawList(LCut,E,LCol,CL,3,0,fMin,fMax);
	return;
}

void SPG_CONV Cut_Draw4(Cut& C1, Cut& C2, Cut& C3, Cut& C4, G_Ecran& E, DWORD Couleur1, DWORD Couleur2, DWORD Couleur3, DWORD Couleur4, C_Lib& CL)
{
	Cut* LCut[4]={&C1,&C2,&C3,&C4};
	DWORD LCol[4]={Couleur1,Couleur2,Couleur3,Couleur4};
	Cut_DrawList(LCut,E,LCol,CL,4,0);
	return;
}
#endif

#ifdef SPG_General_USEFFT
void SPG_CONV CutX_Draw(CutX& C, G_Ecran& E, DWORD Couleur, C_Lib& CL)
{
	Cut CRe, CIm, CModule;

	Cut_Create(CRe,C.NumS,C.XScale,C.UnitX,C.UnitY);
	Cut_Create(CIm,C.NumS,C.XScale,C.UnitX,C.UnitY);
	//Cut_Create(CPhase,C.NumS,C.XScale,C.UnitX,C.UnitY);
	Cut_Create(CModule,C.NumS,C.XScale,C.UnitX,C.UnitY);

	for(int i=0;i<C.NumS;i++)
	{
		CRe.D[i]=C.D[i].re;
		CIm.D[i]=C.D[i].im;
		CModule.D[i]=sqrt(C.D[i].re*C.D[i].re+C.D[i].im*C.D[i].im);
	}

	Cut_Draw3(CRe,CIm,CModule,E,0x00ff00,0xff0000,0,CL);

	Cut_Close(CRe);
	Cut_Close(CIm);
	Cut_Close(CModule);
	return;
}
#endif

#pragma warning( disable : 4701)//local variable 'HorsZoneMax' may be used without having been initialized

int SPG_CONV Cut_FindTwoMax(Cut& C, int& PosOfMax1, int& PosOfMax2,int Exclusion)
{
	PosOfMax2=0;
	float CurrentMax=C.D[0];
	PosOfMax1=0;
	int x;
	for(x=1;x<C.NumS;x++)
	{//recherche le maximum absolu
		if (C.D[x]>CurrentMax)
		{
		CurrentMax=C.D[x];
		PosOfMax1=x;
		}
	}

	int ExclusionG=PosOfMax1;//le pic est ici
	int ExclusionD=PosOfMax1;
	float SecondaryMaxG=CurrentMax;//on place un seuil pour detourer le pic
	float SecondaryMaxD=CurrentMax;//on place un seuil pour detourer le pic

	bool Move;
	do
	{
//on elargit la zone et on abaisse le seuil
		int OldEx=ExclusionG;
		ExclusionG-=Exclusion;//agrandit la zone
		ExclusionG=V_Max(ExclusionG,0);
		Move=false;
		for(x=ExclusionG;x<OldEx;x++)
		{//recherche le nouveau seuil
			if (C.D[x]<SecondaryMaxG) 
			{
				SecondaryMaxG=C.D[x];
				Move=true;//le seuil a encore baissé
			}
		}
	} while(Move);

//	bool Move;
	do
	{
//on elargit la zone et on abaisse le seuil
		int OldEx=ExclusionD;
		ExclusionD+=Exclusion;//agrandit la zone
		ExclusionD=V_Min(ExclusionD,C.NumS);
		Move=false;
		for(x=OldEx;x<ExclusionD;x++)
		{//recherche le nouveau seuil
			if (C.D[x]<SecondaryMaxD) 
			{
				SecondaryMaxD=C.D[x];
				Move=true;//le seuil a encore baissé
			}
		}
	} while(Move);


	bool FoundAValidOne=false;
	{//recherche le maximum dans la zone restante (hors exclusion)
		float HorsZoneMax;
		for(x=0;x<C.NumS;x++)
		{
			if((x<ExclusionG)||(x>=ExclusionD))
			{
					if ((FoundAValidOne==false)||(C.D[x]>HorsZoneMax))//pas de pb de variable non initialisee
					{//FoundAValidOne==false a la première itération et initialise HorsZoneMax
						HorsZoneMax=C.D[x];
						PosOfMax2=x;
						FoundAValidOne=true;
					}
			}
		}
	}
	return FoundAValidOne;
}

int SPG_CONV Cut_ScaledFindTwoMaxAt(Cut& C,int& PosOfMax1, int& PosOfMax2, float& Z0,float& Z1,float ZDiffMin,float ZDiffMax)
{
	//if(C.Decor) memset(C.Decor,0,C.NumS*sizeof(BYTE));
	CHECK(C.XScale==0,"Cut_ScaledFindTwoMaxAt: Pas d'echelle X specifiee",return 0);
	CHECK(ZDiffMax<=ZDiffMin,"Cut_ScaledFindTwoMaxAt: Bornes incorectes",return 0);
	CHECK(ZDiffMax<0,"Cut_ScaledFindTwoMaxAt: Bornes incorectes",return 0);
	CHECK(ZDiffMin<0,"Cut_ScaledFindTwoMaxAt: Bornes incorectes",return 0);

	float CurrentMax;
	//int PosOfMax1;
	if((PosOfMax1=Cut_FindMax(C,CurrentMax))==-1) return 0;
	
	Z0=PosOfMax1*C.XScale;

	int ExclusionG=PosOfMax1;//le pic est ici
	int ExclusionD=PosOfMax1;
	float SecondaryMaxG=CurrentMax;//on place un seuil pour detourer le pic
	float SecondaryMaxD=CurrentMax;//on place un seuil pour detourer le pic

	int Exclusion=3;//V_Round(ZDiffMin/C.XScale);
	//if(Exclusion<1) Exclusion=1;
	int IterMax=V_Round(ZDiffMax/C.XScale/Exclusion);
	if(IterMax<1) IterMax=1;


	bool Move;
	int Iter=0;
	//on cherche la fin de la descente
	//int PosOfMax2;
	do
	{
//on elargit la zone et on abaisse le seuil
		int OldEx=ExclusionG;
		ExclusionG-=Exclusion;
		ExclusionG=V_Max(ExclusionG,0);
		Move=false;
		for(int x=ExclusionG;x<OldEx;x++)
		{
			if (C.D[x]<SecondaryMaxG) 
			{
				SecondaryMaxG=C.D[x];
				Move=true;
			}
		}
		Iter++;
	} while((Move)&&(Iter<IterMax));

//	bool WarningEMaxLeft=(Iter==IterMax);

	//on cherche la fin de la descente
//	bool Move;
	Iter=0;
	do
	{
//on elargit la zone et on abaisse le seuil
		int OldEx=ExclusionD;
		ExclusionD+=Exclusion;
		ExclusionD=V_Min(ExclusionD,C.NumS);
		Move=false;
		for(int x=OldEx;x<ExclusionD;x++)
		{
			if (C.D[x]<SecondaryMaxD) 
			{
				SecondaryMaxD=C.D[x];
				Move=true;
			}
		}
		Iter++;
	} while((Move)&&(Iter<IterMax));

	//ca descend a fond des deux cotes
//	bool WarningEMaxRight=(Iter==IterMax);
	//if(WarningEMaxRight&&WarningEMaxLeft) return 0;

	bool FoundAValidOne=false;
	{
		float HorsZoneMax;
	for(int x=0;x<C.NumS;x++)
	{
		if((x<ExclusionG)||(x>=ExclusionD))
		{
				if ((FoundAValidOne==false)||(C.D[x]>HorsZoneMax))
				{
					HorsZoneMax=C.D[x];
					PosOfMax2=x;
					FoundAValidOne=true;
				}
		}
	}
	}

	if(!FoundAValidOne) return 0;

	if(PosOfMax2<PosOfMax1)
	{
		Z1=Z0;
		Z0=PosOfMax2*C.XScale;
	}
	else
	{
		Z1=PosOfMax2*C.XScale;
	}

	if(!V_InclusiveBound((Z1-Z0),ZDiffMin,ZDiffMax)) return 0;

	if(C.Decor) C.Decor[PosOfMax1]=C.Decor[PosOfMax2]=1;

	return true;
}

#endif

