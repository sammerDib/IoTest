
#include "SPG_General.h"

#ifdef SPG_General_USEPEAKDET2D

#include "SPG_Includes.h"
#include <string.h>

//#include "..\SrcC\SPG_SysInc.h"

void SPG_CONV PeakDet2D_GetLaserModele(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, float CameraAngleRotationRad, float CameraAngleTiltRad, int LaserMode)
{
	CHECK(D2D.Etat==0,"PeakDet2D_GetLaserModele",return);
	CHECK(PM.Etat==0,"PeakDet2D_GetLaserModele",return);

	int NumY=PM.CCDSizeX;

	for(int LaserID=0;LaserID<LASER_COUNT;LaserID++)
	{
		if((LaserMode==LASERMODE_IMPAIR)&&((ANGLE_FROM_ID(LaserID)&1)==0))
		{//LaserMode impair, efface les lasers pairs
			memset(PM.YModele[LaserID],0,NumY*sizeof(float));
			memset(PM.YModeleValid[LaserID],0,NumY*sizeof(BYTE));
		}
		else if((LaserMode==LASERMODE_PAIR)&&(ANGLE_FROM_ID(LaserID)&1))
		{//LaserMode pair, efface les lasers impairs
			memset(PM.YModele[LaserID],0,NumY*sizeof(float));
			memset(PM.YModeleValid[LaserID],0,NumY*sizeof(BYTE));
		}
		else
		{
			SPG_PeakModeleGetY(PM,CameraAngleRotationRad,CameraAngleTiltRad,LaserID,PM.YModele[LaserID],PM.YModeleValid[LaserID],NumY);
		}
	}


	for(int x=0;x<NumY;x++)
	{
		for(int LaserID=0;LaserID<LASER_COUNT;LaserID++)
		{
			PM.YModeleMin[LaserID][x]=PM.YModeleMax[LaserID][x]=PM.YModele[LaserID][x];
			PM.YModeleMinMaxValid[LaserID][x]=0;
			if(PM.YModeleValid[LaserID][x]==0) continue;

			{
				float ModeleY=PM.YModele[LaserID][x];
				float ModeleYMin=ModeleY-(PM.YSearchWidthUp);
				float ModeleYMax=ModeleY+(PM.YSearchWidthDn);

				for(int CrossingLaserID=0;CrossingLaserID<LASER_COUNT;CrossingLaserID++)
				{//traite les cas d'intersections de zones de recherche
					if(CrossingLaserID==LaserID) continue;
					if(PM.YModeleValid[CrossingLaserID][x]==0) continue;
					//if(PM.YModeleMinMaxValid[CrossingLaserID][x]==0) continue;
					//YModeleMinMaxValid ne peut être choisi comme critère car cet
					//état n'est pas disponible si CrossingLaserID>LaserID

					if(INCIDENCE_FROM_ID(CrossingLaserID)==1)//le laser 1 est le laser central, il est prioritaire
					{
						float CrossingY=PM.YModele[CrossingLaserID][x];
						float CrossingYMin=CrossingY-(PM.YSearchWidthUp);
						float CrossingYMax=CrossingY+(PM.YSearchWidthDn);

						if(CrossingY>ModeleY)
						{//le laser prioritaire est au dessus
							if(CrossingYMin<ModeleYMax) ModeleYMax=CrossingYMin;
							if(CrossingYMin<ModeleY)    ModeleY=CrossingYMin;
							if(CrossingYMin<ModeleYMin) ModeleYMin=CrossingYMin;
						}
						else
						{//le laser prioritaire est en dessous
							if(CrossingYMax>ModeleYMax) ModeleYMax=CrossingYMax;
							if(CrossingYMax>ModeleY)    ModeleY=CrossingYMax;
							if(CrossingYMax>ModeleYMin) ModeleYMin=CrossingYMax;
							
						}
					}
				}
				ModeleYMin=V_Max(ModeleYMin,D2D.SideEffect);
				ModeleYMax=V_Min(ModeleYMax,PM.CCDSizeY-D2D.SideEffect);

				if((ModeleYMin>=ModeleYMax)||
					(x<(D2D.SideEffect+2))||(x>=(D2D.SizeX-D2D.SideEffect-2)))
				{
					PM.YModeleMinMaxValid[LaserID][x]=0;
				}
				else
				{
					PM.YModeleMinMaxValid[LaserID][x]=1;
				}

				PM.YModeleMin[LaserID][x]=ModeleYMin;
				PM.YModeleMax[LaserID][x]=ModeleYMax;
			}
		}
	}
	return;
}

float SPG_CONV ScanGetCorrection(SCANDATA& SD, int x, float a, float b)
{
	float xcn=(2.0f*x-SD.NumY)/SD.NumY;
	return a*xcn*xcn+b*xcn;
}

float SPG_CONV ScanGetDistance(SCANDATA& SD, float a, float b)
{
	float D=0.0f;
	int Count=0;

	for(int i=0;i<SD.NumY-1;i++)
	{
		if(SD.Yvalid[i]&&SD.Yvalid[i+1])
		{
			D+=fabs(
				 (SD.Y[i]-ScanGetCorrection(SD,i,a,b))
				-(SD.Y[i+1]-ScanGetCorrection(SD,i+1,a,b))
				);
			Count++;
		}
	}
	if(Count==0) return -1.0f;
	return D;
}

float SPG_CONV ScanOptimize(SCANDATA& SD, SCANAERA& SA, float& Min_x, float& Min_y)
{
	float Min_Distance=-1.0f;
	for(float y=SA.yMin;y<SA.yMax;y+=SA.yStep)
	{
		for(float x=SA.xMin;x<SA.xMax;x+=SA.xStep)
		{
			float D=ScanGetDistance(SD,x,y);
			if(D>=0.0f)
			{
				if(((Min_Distance>=0.0f)&&(D<Min_Distance))||(Min_Distance<0.0f))
				{
					Min_Distance=D;
					Min_x=x;
					Min_y=y;
				}
			}
		}
	}
	return Min_Distance;
}

void SPG_CONV ScanDefineScanAera(SCANAERA& SA, float xMin, float xMax, float yMin, float yMax, int NX, int NY)
{
	SA.xMin=xMin;
	SA.xMax=xMax;
	SA.yMin=yMin;
	SA.yMax=yMax;
	SA.xStep=(SA.xMax-SA.xMin)/NX;
	SA.yStep=(SA.yMax-SA.yMin)/NY;
	return;
}

void SPG_CONV ScanRefineScanAera(SCANAERA& SA, float Min_x, float Min_y, int NX, int NY)
{
	SA.xMin=V_Max(SA.xMin,Min_x-2*SA.xStep);
	SA.xMax=V_Min(SA.xMax,Min_x+2*SA.xStep);
	SA.yMin=V_Max(SA.yMin,Min_y-2*SA.yStep);
	SA.yMax=V_Min(SA.yMax,Min_y+2*SA.yStep);
	SA.xStep*=4.0f/NX;
	SA.yStep*=4.0f/NY;
	return;
}

void SPG_CONV PeakDet2D_LowPassFilter(float* Ysrc, float* Ydst, BYTE* Yvalid, int NumY)
{
	for(int x=0;x<NumY;x++)
	{
		if(Yvalid[x])
		{
			float Sum=0;
			for(int i=-1;i<=1;i++)
			{
				int xL=x-i;
				if(xL<0) xL=0;
				if(xL>=NumY) xL=NumY-1;
				if(Yvalid[xL]==0) xL=x;
				Sum+=Ysrc[xL];
			}
			Ydst[x]=Sum/3;
		}
		else
		{
			Ydst[x]=0;
		}
	}
}

int SPG_CONV PeakDet2D_MinTotalVar(SCANDATA& SD, SCANAERA& SA, int NX, int NY, float& Min_x, float& Min_y)
{
	Min_x=0;
	Min_y=0;
	int MAX_ITER=26;
	int i;
	for(i=0;i<MAX_ITER;i++)
	{
		if(ScanOptimize(SD,SA,Min_x,Min_y)<0) break;
		ScanRefineScanAera(SA,Min_x,Min_y,NX,NY);
	}
	return (i==MAX_ITER);
}

float SPG_CONV PeakDet2D_GetMeanVal(float* Y, BYTE* Yvalid, int NumY)
{
	float Sum=0;
	int Count=0;
	for(int x=0;x<NumY;x++)
	{
		if(Yvalid[x])
		{
			Sum+=Y[x];
			Count++;
		}
	}
	if(Count==0) return 0;
	return Sum/Count;
}

void SPG_CONV PeakDet2D_Filter(float* Y, BYTE* Yvalid, int NumY, float* Ytmp)
{
	CHECK(Y==0,"PeakDet2D_Filter",return);
	CHECK(Yvalid==0,"PeakDet2D_Filter",return);
	CHECK(Ytmp==0,"PeakDet2D_Filter",return);

#ifdef SPG_DEBUGCONFIG
	{
		for(int xd=0;xd<NumY;xd++)
		{
			if(Yvalid[xd]) break;
		}
		if(xd==NumY) return;
	}
#endif

	PeakDet2D_LowPassFilter(Y,Ytmp,Yvalid,NumY);

	/*
NOP = 1000000
Plot[Log[(200/x)*((4/x)^((NOP/768)/(x*x)))], {x, 5, 12}]
XO = 7
NITER = Round[(NOP/768)/(XO*XO)] - 1;
P = (200/XO)*((4/XO)^(NITER));
N[NITER]
N[P]
	*/

	SCANDATA SD;
	SD.NumY=NumY;
	SD.Y=Ytmp;
	SD.Yvalid=Yvalid;

	SCANAERA SA;
	int NX=7;
	int NY=7;
	ScanDefineScanAera(SA,-100,100,-100,100,NX,NY);
	
	float a,b;
	if(PeakDet2D_MinTotalVar(SD,SA,NX,NY,a,b))
	{
		for(int x=0;x<NumY;x++)
		{
			Ytmp[x]-=ScanGetCorrection(SD,x,a,b);
		}
	}

	float S=PeakDet2D_GetMeanVal(Ytmp, Yvalid, NumY);
	for(int x=0;x<NumY;x++)
	{
		Y[x]=Ytmp[x]-S;
	}

/*
	//le plus simple: trouve le premier et dernier points valides et les place à zero
	for(int x1=0;x1<NumY;x1++)
	{
		if(Yvalid[x1]) break;
	}
	for(int x2=NumY-1;x2>=0;x2--)
	{
		if(Yvalid[x2]) break;
	}
	if(x1<x2)
	{
		for(int x=0;x<NumY;x++)
		{
			Ytmp[x]=Y[x1]+(x-x1)*(Y[x2]-Y[x1])/(x2-x1);
		}
		for(x=0;x<NumY;x++)
		{
			Y[x]-=Ytmp[x];
		}
	}
*/
	return;
}

int SPG_CONV PeakDet2D_GetLaserLine(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, float* Y, BYTE* Yvalid, int NumY, float CameraAngleRotationRad, float CameraAngleTiltRad, int ProcessingMode, int LaserMode, SPG_PEAKPOSITION** YParameters_LASER_COUNT)
{//combine results indique qu'il y a déjà un résultat dans float* Y et qu'on doit combiner le nouveau résultat avec l'ancien
	CHECK(D2D.Etat==0,"PeakDet2D_GetLaserLine",return 0);
	CHECK(PM.Etat==0,"PeakDet2D_GetLaserLine",return 0);
	CHECK(Y==0,"PeakDet2D_GetLaserLine",return 0);
	CHECK(NumY!=PM.CCDSizeX,"PeakDet2D_GetLaserLine",return 0);

	//LaserMode=0: tous les lasers LaserMode=1 lasers pairs LaserMode=2 lasers impairs
	PeakDet2D_GetLaserModele(D2D,PM,CameraAngleRotationRad,CameraAngleTiltRad,LaserMode);

	for(int LaserID=0;LaserID<LASER_COUNT;LaserID++)
	{
		SPG_PEAKPOSITION* YParam=0;
		if(YParameters_LASER_COUNT)
		{
			YParam=YParameters_LASER_COUNT[LaserID];
		}
		PeakDet2D_GetLine(D2D,
			PM.YMesure[LaserID],PM.YMesureValid[LaserID],
			PM.YModele[LaserID],PM.YModeleMin[LaserID],PM.YModeleMax[LaserID],PM.YModeleMinMaxValid[LaserID],
			YParam);

		for(int x=0;x<PM.CCDSizeX;x++)//ne conserve que l'écart par rapport au modele
		{
			PM.YMesureDelta[LaserID][x]=PM.YMesure[LaserID][x]-PM.YModele[LaserID][x];
		}
		if((ProcessingMode&PEAKDET2D_NOFILTER)==0) PeakDet2D_Filter(PM.YMesureDelta[LaserID],PM.YMesureValid[LaserID],NumY,PM.YFiltTmp);
	}


	int Incidence;
	for(Incidence=0;Incidence<LASER_INCIDENCE_COUNT;Incidence++)
	{
		for(int x=0;x<PM.CCDSizeX;x++)
		{
			float YAvgDiff=0;//valeur moyenne de la différence entre le modèle et la mesure
			int Sum=0;
			for(int Angle=0;Angle<LASER_ANGLE_COUNT;Angle++)
			{
				int LaserID=MAKE_LASER_ID(Incidence,Angle);
				if((PM.YMesureValid[LaserID][x])&&(PM.YModeleMinMaxValid[LaserID][x]))
				{
					YAvgDiff+=PM.YMesureDelta[LaserID][x];
					Sum++;
				}
			}
			if(Sum)
			{
				PM.YCombine[Incidence][x]=YAvgDiff/Sum;
				PM.YCombineValid[Incidence][x]=1;
			}
			else
			{
				PM.YCombine[Incidence][x]=0;
				PM.YCombineValid[Incidence][x]=0;
			}
		}
	}

	//TODO: soustraire la forme du signal des laser d'incidence 0 et 2 au signal du laser 1
	//extrait le profil de l'incidence nr 1 de 0 1 2 incidences possibles
	if(ProcessingMode&PEAKDET2D_INITIALRESULT)
	{
		Incidence=1;//calcule seulement le laser central
		for(int x=0;x<PM.CCDSizeX;x++)
		{
			Y[x]=PM.YCombine[Incidence][x];
			Yvalid[x]=PM.YCombineValid[Incidence][x];
		}
	}
	else if(ProcessingMode&PEAKDET2D_COMBINERESULT)
	{
		Incidence=1;

		int Count=0;//resultat antérieur
		int CountCombine=0;//nouveau resultat
		{for(int x=0;x<PM.CCDSizeX;x++)
		{
			if(Yvalid[x]) Count++;
			if(PM.YCombineValid[Incidence][x]) CountCombine++;
		}}

		int CountCommon=0;
		float SumCommon=0;
		{for(int x=0;x<PM.CCDSizeX;x++)
		{
			if((Yvalid[x])&&(PM.YCombineValid[Incidence][x]))
			{
				SumCommon+=PM.YCombine[Incidence][x]-Y[x];
				CountCommon++;
			}
		}}
		SumCommon/=V_Max(CountCommon,1);

		{for(int x=0;x<PM.CCDSizeX;x++)
		{
			if(Yvalid[x]==0)
			{//met le nouveal resultat
				if(Yvalid[x]=PM.YCombineValid[Incidence][x])//affectation volontaire
				{
					Y[x]=PM.YCombine[Incidence][x]-SumCommon;
				}
				else
				{
					Y[x]=0;
				}
			}
			else if(PM.YCombineValid[Incidence][x]==0)
			{//laisse le resultat antérieur
			}
			else//les deux sont valides
			{
				//option 1: combine les resultats
				//	Y[x]=(Y[x]+PM.YCombine[Incidence][x])/2;

				//option 2
				if(CountCombine>Count)
				{//si la nouvelle ligne contient plus de données que l'ancienne elle a la priorité
					Y[x]=PM.YCombine[Incidence][x]-SumCommon;
				}
			}
		}}
	}
	return PEAKDET_OK;
}

void DrawYLineModele(G_Ecran& E, float* Y, BYTE* Yvalid, DWORD Color)
{
	CHECK(E.Etat==0,"DrawYLine",return);
	CHECK(Y==0,"DrawYLine",return);
	DWORD HalfColor=(Color&0xFEFEFE)>>1;
	for(int x=0;x<G_SizeX(E);x++)
	{
		if((Yvalid[x]==0)&&((x&3)!=0)) continue;//les zones invalides sont pointillées
		int y=Y[x];
		{
			G_DrawPixel(E,x,(y-1),HalfColor);
			G_DrawPixel(E,x,y,Color);
			G_DrawPixel(E,x,(y+1),HalfColor);
		}
	}
	return;
}

void DrawYLineResult(G_Ecran& E, float* Y, BYTE* Yvalid, DWORD Color)
{
	CHECK(E.Etat==0,"DrawYLine",return);
	CHECK(Y==0,"DrawYLine",return);
	DWORD HalfColor=(Color&0xFEFEFE)>>1;
	for(int x=0;x<G_SizeX(E);x++)
	{
		if(Yvalid[x]==0) continue;//les zones invalides ne sont pas dessinées
		int y=Y[x];
		{
			G_DrawPixel(E,x,(y-1),HalfColor);
			G_DrawPixel(E,x,y,Color);
			G_DrawPixel(E,x,(y+1),HalfColor);
		}
	}
	return;
}

void DrawYLineResultChecked(G_Ecran& E, float* Y, BYTE* Yvalid, DWORD Color)
{
	CHECK(E.Etat==0,"DrawYLine",return);
	CHECK(Y==0,"DrawYLine",return);
	DWORD HalfColor=(Color&0xFEFEFE)>>1;
	for(int x=0;x<G_SizeX(E);x++)
	{
		if(Yvalid[x]==0) continue;//les zones invalides ne sont pas dessinées
		int y=Y[x];
		{
			G_DrawPixel(E,x,(y-1),HalfColor);
			G_DrawPixel(E,x,y,Color);
			G_DrawPixel(E,x,(y+1),HalfColor);
		}
	}
	return;
}

void DrawYLineSum(G_Ecran& E, float* Y1, float* Y2, BYTE* Yvalid, DWORD Color)
{
	CHECK(E.Etat==0,"DrawYLineSum",return);
	CHECK(Y1==0,"DrawYLineSum",return);
	CHECK(Y2==0,"DrawYLineSum",return);
	DWORD HalfColor=(Color&0xFEFEFE)>>1;
	for(int x=0;x<G_SizeX(E);x++)
	{
		if((Yvalid[x]==0)&&((x&3)!=0)) continue;//les zones invalides sont pointillées
		int y=Y1[x]+Y2[x];
		{
			G_DrawPixel(E,x,(y-1),HalfColor);
			G_DrawPixel(E,x,y,Color);
			G_DrawPixel(E,x,(y+1),HalfColor);
		}
	}
	return;
}

void DrawYAera(G_Ecran& E, float* YMin, float* YMax, BYTE* YMinMaxValid, DWORD Color, int StartPos)
{
	CHECK(E.Etat==0,"DrawYAera",return);
	CHECK(YMin==0,"DrawYAera",return);
	CHECK(YMax==0,"DrawYAera",return);
	DWORD HalfColor=(Color&0xFEFEFE)>>1;
	for(int x=StartPos&3;x<G_SizeX(E);x+=4)
	{
		if((YMinMaxValid[x]==0)&&((x&3)!=0)) continue;//les zones invalides sont pointillées
		int yMin=YMin[x];
		int yMax=YMax[x];
		int yStart=((yMin+3)&~3)+((x>>2)&3);
		for(int y=yStart;y<yMax;y+=4)
		{
			G_DrawPixel(E,x,(y-1),HalfColor);
			G_DrawPixel(E,x,y,Color);
			G_DrawPixel(E,x,(y+1),HalfColor);
		}
	}
	return;
}

// ne dessine que le modele
void SPG_CONV PeakDet2D_DrawLaserModele(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, G_Ecran& E, float CameraAngleRotationRad, float CameraAngleTiltRad, int LaserMode)
{
	CHECK(D2D.Etat==0,"PeakDet2D_DrawLaserModele",return);
	CHECK(PM.Etat==0,"PeakDet2D_DrawLaserModele",return);
	CHECK(E.Etat==0,"PeakDet2D_DrawLaserModele",return);

	PeakDet2D_GetLaserModele(D2D,PM,CameraAngleRotationRad,CameraAngleTiltRad, LaserMode);

	for(int LaserID=0;LaserID<LASER_COUNT;LaserID++)
	{
		DWORD C=0x404040;
		if((INCIDENCE_FROM_ID(LaserID))==0)
		{
			C=0x104060*LaserID;
		}
		else if((INCIDENCE_FROM_ID(LaserID))==1)
		{
			C=0x106040*LaserID;
		}
		else if((INCIDENCE_FROM_ID(LaserID))==2)
		{
			C=0x604010*LaserID;
		}
		DrawYLineModele(E,PM.YModele[LaserID],PM.YModeleValid[LaserID],C);
		DrawYAera(E,PM.YModeleMin[LaserID],PM.YModeleMax[LaserID],PM.YModeleMinMaxValid[LaserID],C,LaserID);
	}
	return;
}

// dessine le dernier modele et le dernier resultat calculés
void SPG_CONV PeakDet2D_DrawLaserResult(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, G_Ecran& E)
{
	CHECK(D2D.Etat==0,"PeakDet2D_DrawLaserResult",return);
	CHECK(PM.Etat==0,"PeakDet2D_DrawLaserResult",return);
	CHECK(E.Etat==0,"PeakDet2D_DrawLaserResult",return);

	P_Draw(D2D.WeightApriori,E);

	for(int LaserID=0;LaserID<LASER_COUNT;LaserID++)
	{
		DWORD C=0x806080;
		if((INCIDENCE_FROM_ID(LaserID))==0)
		{
			C=0x104060*LaserID;
		}
		else if((INCIDENCE_FROM_ID(LaserID))==1)
		{
			C=0x108060*LaserID;
		}
		else if((INCIDENCE_FROM_ID(LaserID))==2)
		{
			C=0x806010*LaserID;
		}
		DrawYLineModele(E,PM.YModele[LaserID],PM.YModeleValid[LaserID],C);
		DrawYAera(E,PM.YModeleMin[LaserID],PM.YModeleMax[LaserID],PM.YModeleMinMaxValid[LaserID],C,LaserID);
		DrawYLineResult(E,PM.YMesure[LaserID],PM.YMesureValid[LaserID],C);
		/*
		if(LaserID==13)
		{
			int a=0;
			DrawYLineResultChecked(E,PM.YMesure[LaserID],PM.YModeleValid[LaserID],C);
		}

		DrawYLineSum(E,PM.YMesure[LaserID],PM.YModele[LaserID],PM.YMesureValid[LaserID],C);
		*/
	}
	return;
}


#endif
