
#include "SPG_General.h"

#ifdef SPG_General_USEHIST

#include "SPG_Includes.h"
/*
#include "V_General.h"
#include "SPG_Mem.h"
#include "SPG_List.h"
#include "SPG_Graphics.h"

#include "SPG_Histogram256.h"
*/
#include <string.h>

int SPG_CONV HIST2D_Init(HIST2D_TYPE& H, int NumCaterX, int NumCaterY)
{
	memset(&H,0,sizeof(HIST2D_TYPE));
	H.NumCaterX=NumCaterX;
	H.NumCaterY=NumCaterY;
	return ((H.Cumul=SPG_TypeAlloc(NumCaterX*NumCaterY,HIST_BIN_TYPE,"Histogramme"))!=0);
}

int SPG_CONV HIST_Init(HIST_TYPE& H, int NumCater)
{
	memset(&H,0,sizeof(HIST_TYPE));
	H.NumCater=NumCater;
	return ((H.Cumul=SPG_TypeAlloc(NumCater,HIST_BIN_TYPE,"Histogramme"))!=0);
}

void SPG_CONV HIST2D_Close(HIST2D_TYPE& H)
{
	if(H.Cumul) SPG_MemFree(H.Cumul);
	memset(&H,0,sizeof(HIST2D_TYPE));
	return;
}

void SPG_CONV HIST_Close(HIST_TYPE& H)
{
	if(H.Cumul) SPG_MemFree(H.Cumul);
	memset(&H,0,sizeof(HIST_TYPE));
	return;
}

int SPG_CONV HIST_FindMax(HIST_TYPE& H)
{
	HIST_BIN_TYPE Max=0;
	for(int x=0;x<H.NumCater;x++)
	{
		if (H.Cumul[x]>Max) Max=H.Cumul[x];
	}
	return Max;
}

int SPG_CONV HIST_LocalSearch(HIST_TYPE& H, int IndexMin, int IndexMax)
{
	CHECK(IndexMin<0,"HIST_LocalSearch",return -1);
	CHECK(IndexMax>=H.NumCater,"HIST_LocalSearch",return -1);

	int pMax=IndexMin;
	HIST_BIN_TYPE Max=H.Cumul[IndexMin];
	for(int x=IndexMin+1;x<=IndexMax;x++)
	{
		if (H.Cumul[x]>Max) 
		{
			Max=H.Cumul[x];
			pMax=x;
		}
	}
	return pMax;
}

int SPG_CONV HIST2D_FindMax(HIST2D_TYPE& H)
{
	HIST_BIN_TYPE Max=0;
	for(int x=0;x<H.NumCaterX*H.NumCaterY;x++)
	{
		if (H.Cumul[x]>Max) Max=H.Cumul[x];
	}
	return Max;
}

void SPG_CONV HIST_Draw(HIST_TYPE& H, G_Ecran& E, DWORD BackGroundColor, DWORD Couleur, DWORD CouleurFlag)
{
	G_DrawRect(E,0,0,E.SizeX,E.SizeY,BackGroundColor);

	int MaxVal=HIST_FindMax(H);
	if(MaxVal==0) return;

	int Pas=V_Max(E.SizeX/H.NumCater,1);
	int Marge=(E.SizeX-Pas*H.NumCater)/2;
	int L=V_Max((Pas>>1),1);
	//check...
	for(int x=0;x<H.NumCater;x++)
	{
		int y=((E.SizeY-1)*H.Cumul[x])/MaxVal;
		G_DrawRect(E,Marge+Pas*x,E.SizeY-1-y,Marge+Pas*x+L,E.SizeY,(H.Cumul[x]==MaxVal)?CouleurFlag:Couleur);
	}

	return;
}

void SPG_CONV HIST2D_Stats(HIST2D_TYPE& H, float& MeanX, float& MeanY)
{
	int MaxVal=HIST2D_FindMax(H);
	CHECK(MaxVal==0,"HIST2D_Stats",return);
	//check...

	MeanX=0;
	MeanY=0;

	int SW=0;
	for(int ny=0;ny<H.NumCaterY;ny++)
	{
	for(int nx=0;nx<H.NumCaterX;nx++)
	{
		int W=H.Cumul[nx+H.NumCaterX*ny];
		MeanX+=nx*W;
		MeanY+=ny*W;
		SW+=W;
	}
	}
	MeanX/=SW;
	MeanY/=SW;
	return;
}

void SPG_CONV HIST2D_UpdateMean(HIST2D_TYPE& H, float Sigma, float& MeanX, float& MeanY)
{
	int MaxVal=HIST2D_FindMax(H);
	CHECK(MaxVal==0,"HIST2D_UpdateMean",return);
	//check...

	float SW=0;

	float Nx=MeanX;
	float Ny=MeanY;

	MeanX=0;
	MeanY=0;

	for(int ny=0;ny<H.NumCaterY;ny++)
	{
		float Y2=(ny-Ny)*(ny-Ny);
	for(int nx=0;nx<H.NumCaterX;nx++)
	{
		float D2=(nx-Nx)*(nx-Nx)+Y2;
		float W=H.Cumul[nx+H.NumCaterX*ny]/(Sigma*Sigma+D2);
		MeanX+=nx*W;
		MeanY+=ny*W;
		SW+=W;
	}
	}
	if(SW)
	{
		MeanX/=SW;
		MeanY/=SW;
	}
	return;
}

void SPG_CONV HIST2D_Draw(HIST2D_TYPE& H, G_Ecran& E)
{
	int MaxVal=HIST2D_FindMax(H);
	CHECK(MaxVal==0,"HIST2D_Draw",return);
	//check...
	for(int ny=0;ny<H.NumCaterY;ny++)
	{
	for(int n=0;n<H.NumCaterX;n++)
	{
		PixCoul Couleur;
		Couleur.R=Couleur.V=Couleur.B=(BYTE)((H.Cumul[n+H.NumCaterX*ny]*255)/MaxVal);
		G_DrawPixel(E,n,(E.SizeY-ny-1),Couleur.Coul);
	}
	}

	return;
}

void SPG_CONV HIST2D_DrawLog(HIST2D_TYPE& H, G_Ecran& E)
{
	int MaxVal=HIST2D_FindMax(H);
	CHECK(MaxVal==0,"HIST2D_Draw",return);
	//check...
	float vnorm=log((float)(1+MaxVal))/log((float)2);

	for(int ny=0;ny<H.NumCaterY;ny++)
	{
	for(int n=0;n<H.NumCaterX;n++)
	{
		PixCoul Couleur;
		float v=log(1+(float)H.Cumul[n+H.NumCaterX*ny])/log((float)2);
		Couleur.R=Couleur.V=Couleur.B=(BYTE)(v/vnorm*255);
		G_DrawPixel(E,n,(E.SizeY-ny-1),Couleur.Coul);
	}
	}

	return;
}

int SPG_CONV DrawHist2D_Init(DRAWHIST2D& H, int Size, float NormeMax)
{
	H.Size=Size;
	//NormeMin
	//NormeMax
	//log(Norme/NormeMin)/log(NormeMax/NormeMin)
	H.NormeMax=NormeMax;
	H.invNormeMax=1.0f/H.NormeMax;
	SPG_FillColorPalette(H.Color,DRAWVECTCOLOR);
	H.Count=SPG_TypeAlloc((2*H.Size+1)*(2*H.Size+1),float,"DrawHist2D_Init");
	DrawHist2D_Clear(H);
	return -1;
}

void SPG_CONV DrawHist2D_Close(DRAWHIST2D& H)
{
	SPG_MemFree(H.Count);
	memset(&H,0,sizeof(DRAWHIST2D));
	return;
}

void SPG_CONV DrawHist2D_Clear(DRAWHIST2D& H)
{
	memset(H.Count,0,(2*H.Size+1)*(2*H.Size+1)*sizeof(float));
	H.MaxCount=0;
	return;
}

void SPG_CONV DrawHist2D_Add(DRAWHIST2D& H, float Vx, float Vy, float Weight)
{
	int X=V_Round(H.Size*Vx*H.invNormeMax);
	int Y=V_Round(H.Size*Vy*H.invNormeMax);
	//X=V_Sature(X,-H.Size,H.Size)+H.Size;
	//Y=V_Sature(Y,-H.Size,H.Size)+H.Size;
	if((V_InclusiveBound(X,-H.Size,H.Size))&&(V_InclusiveBound(Y,-H.Size,H.Size)))
	{
		X+=H.Size;
		Y+=H.Size;
		if((H.Count[X+Y*(2*H.Size+1)]+=Weight)>H.MaxCount)
		{
			H.MaxCount=H.Count[X+Y*(2*H.Size+1)];
		}
	}
	return;
}

void SPG_CONV DrawHist2D_Stats(DRAWHIST2D& H, float& Add, float& Mul)
{
	Add=0;
	Mul=1;
	float Sum=0;
	float FirstMoment=0;
	float FirstOrder=0;
	float SecondOrder=0;
	float N=0;
	for(int y=-H.Size;y<=H.Size;y++)
	{
		float Y=y*H.NormeMax;

		float xSum=0;
		float xFirstMoment=0;
		float xFirstOrder=0;
		float xSecondOrder=0;
		float xN=0;
		for(int x=-H.Size;x<=H.Size;x++)
		{
			float X=x*H.NormeMax;
			float C=(float)H.Count[x+H.Size+(y+H.Size)*(2*H.Size+1)];
			//reference=x current=y
			//y = ax + b
			//Sum         = somme(y) = somme(ax + b) = a somme(x) + Nb
			//FirstMoment = somme(xy) = somme(axx + bx) = a somme(xx) + b somme(x)
			//FirstOrder  = somme(x)
			//SecondOrder = somme(xx)
			xFirstOrder+=X*C;//Reference[x];
			xSecondOrder+=X*X*C;//Reference[x]*Reference[x];
			xSum+=Y*C;//Current[x];
			xFirstMoment+=Y*X*C;//Current[x]*Reference[x];
			xN+=C;
		}
		FirstOrder+=xFirstOrder;
		SecondOrder+=xSecondOrder;
		Sum+=xSum;
		FirstMoment+=xFirstMoment;
		N+=xN;
	}
	float D=(N*SecondOrder - FirstOrder*FirstOrder);//variance des données de reference fois N
	if(D!=0)//math.D>=0
	{
		float invD=1.0f/D;
		//somme(xx) * Sum - somme(x) * FirstMoment = N somme(xx) b - b somme(x) somme(x)
		//b=(somme(xx) * Sum - somme(x) * FirstMoment)/(N somme(xx) - somme(x) somme(x))
		Add=(SecondOrder*Sum - FirstOrder*FirstMoment)*invD;
		//somme(x) * Sum - N * FirstMoment = a somme(x) somme(x) - a somme(xx) N
		//a=(somme(x) * Sum - N * FirstMoment)/(somme(x) somme(x) - a somme(xx) N)
		Mul=(N*FirstMoment - FirstOrder*Sum)*invD;
	}

	return;
}

void SPG_CONV DrawHist2D(DRAWHIST2D& H, G_Ecran& E)
{
	float LogDyn=100.0f;
	float invNrm=1.0f/log(1.0f+LogDyn);
	float invLogDyn=LogDyn/H.MaxCount;
	G_DrawRect(E,0,0,2*H.Size+1,2*H.Size+1,0);
	G_DrawCircle(E,H.Size,H.Size,H.Size,0x808080);
	G_DrawCircle(E,H.Size,H.Size,H.Size/2,0x808080);
	G_DrawLine(E,0,H.Size,2*H.Size+1,H.Size,0x808080);
	G_DrawLine(E,H.Size,0,H.Size,2*H.Size+1,0x808080);
	for(int y=0;y<V_Min((2*H.Size+1),E.SizeY);y++)
	{
		BYTE* ELine=G_MECR(E)+y*G_Pitch(E);
		float* HLine=H.Count+y*(2*H.Size+1);
		for(int x=0;x<V_Min((2*H.Size+1),E.SizeX);x++)
		{
			int W=DRAWVECTCOLOR*log(1.0f+invLogDyn*HLine[x])*invNrm;
			if(W>0)
			{
			//G_DrawPixel(E,x,y,H.Color[V_Sature(W,0,(DRAWVECTCOLOR-1))].Coul);
			*(PixCoul24*)(ELine+x*G_POCT(E))=H.Color[V_Sature(W,0,(DRAWVECTCOLOR-1))].P24;
			}
		}
	}
	return;
}

int SPG_CONV DrawHist1D_Init(DRAWHIST1D& H)
{
	memset(&H,0,sizeof(DRAWHIST1D));
	return -1;
}

void SPG_CONV DrawHist1D_Close(DRAWHIST1D& H)
{
	memset(&H,0,sizeof(DRAWHIST1D));
	return;
}

void SPG_CONV DrawHist1D_Clear(DRAWHIST1D& H)
{
	memset(&H.Count,0,256*sizeof(int));
	return;
}

void SPG_CONV DrawHist1D_FillDiff(DRAWHIST1D& H, BYTE* restrict ERef, int PitchRef, BYTE* restrict ECur, int PitchCur, int SizeX, int SizeY)
{
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			H.Count[(BYTE)(ECur[x]-ERef[x])]++;
		}
		ERef+=PitchRef;
		ECur+=PitchCur;
	}
	return;
}

void SPG_CONV DrawHist1D_Add(DRAWHIST1D& H, float V, float BinSize)
{
	H.Count[(BYTE)(int)(V/BinSize)]++;
	return;
}

void SPG_CONV DrawHist1D_FillDiff(DRAWHIST1D& H, float* ERef, int PitchRef, float* ECur, int PitchCur, int SizeX, int SizeY, float BinSize)
{
	float invBinSize=1.0f/BinSize;
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			H.Count[(BYTE)(int)((ECur[x]-ERef[x])*invBinSize)]++;
		}
		ERef+=PitchRef;
		ECur+=PitchCur;
	}
	return;
}

//Moy=(float)FirstOrder/Sum
//EcartType=sqrt(((float)SecondOrder-(float)FirstOrder*FirstOrder/Sum)/Sum)
void SPG_CONV DrawHist1D_Stats(DRAWHIST1D& H, int& Sum, int& FirstOrder, int& SecondOrder)
{
	Sum=0;
	FirstOrder=0;
	SecondOrder=0;
	for(int x=-128;x<128;x++)
	{
		int Count=H.Count[(BYTE)x];
		Sum+=Count;
		FirstOrder+=x*Count;
		SecondOrder+=x*x*Count;
	}
	if(((float)SecondOrder-(float)FirstOrder*FirstOrder/Sum)<0)
	{
		SecondOrder=0;
		FirstOrder=0;
	}
	return;
}

int SPG_CONV DrawHist1D_FindMax(DRAWHIST1D& H)
{
	int Max=0;
	for(int x=0;x<256;x++)
	{
		if (H.Count[x]>Max) Max=H.Count[x];
	}
	return Max;
}

void SPG_CONV DrawHist1D(DRAWHIST1D& H, G_Ecran& E, DWORD BackGroundColor, DWORD Couleur, DWORD CouleurFlag)
{
	CHECK(E.Etat==0,"DrawHist1D",return);
	G_DrawRect(E,0,0,E.SizeX,E.SizeY,BackGroundColor);

	int MaxVal=DrawHist1D_FindMax(H);

	int Pas=V_Max(E.SizeX/256,1);
	int Marge=(E.SizeX-Pas*256)/2;
	int L=V_Max((Pas>>1),1);
	float Nrm=1.0f/(G_SizeY(E));
	//check...
	//x/(1+x*x/max)
	//max/(1+max)
	for(int x=0;x<256;x++)
	{
		int Count=H.Count[x^128];
		int y=Count/(1.0f+Nrm*Count);
		G_DrawRect(E,Marge+Pas*x,E.SizeY-1-y,Marge+Pas*x+L,E.SizeY,(Count==MaxVal)?CouleurFlag:Couleur);
	}

	return;
}

#endif


