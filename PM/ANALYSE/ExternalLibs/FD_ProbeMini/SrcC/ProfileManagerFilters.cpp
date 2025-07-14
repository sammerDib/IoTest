
#include "SPG_General.h"

#ifdef SPG_General_USEProfil

//#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <float.h>

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

/*
Matrice 3x3, moments nuls
+0.25	-0.50	+0.25
-0.50	+1.00	-0.50
+0.25	-0.50	+0.25

Matrice 5x5, 6 coeffs à 1
+0.75	 0.00	-1.50	 0.00	+0.75
 0.00	-0.50	+1.00	-0.50	 0.00
-1.50	+1.00	+1.00	+1.00	-1.50
 0.00	-0.50	+1.00	-0.50	 0.00
+0.75	 0.00	-1.50	 0.00	+0.75
*/

void SPG_CONV P_Soften(Profil& P)
{//convolution par un carre 2x2
	int y;
	for (y=0;y<P_SizeX(P)*(P_SizeY(P)-1)-1;y++)
	{
			P_Data(P)[y]+=P_Data(P)[y+1]+P_Data(P)[y+P_SizeX(P)]+P_Data(P)[y+1+P_SizeX(P)];
	}
	return;
}


void SPG_CONV P_MedianFilter(Profil& P, Profil& Ptmp, int FilterSize)
{
	CHECK(P_Etat(P)==0,"P_MedianFilter: Profil nul",return);
	CHECK(P_Etat(Ptmp)==0,"P_MedianFilter: Profil temporaire nul",return);
	CHECK(P_Data(P)==0,"P_MedianFilter: Profil vide",return);
	CHECK(P_Data(Ptmp)==0,"P_MedianFilter: Profil temporaire vide",return);
	CHECK(P_Data(P)==0,"P_MedianFilter: Profil vide",return);
	CHECK(P_Data(Ptmp)==0,"P_MedianFilter: Profil temporaire vide",return);
	CHECK(P_SizeX(Ptmp)*P_SizeY(Ptmp)<P_SizeX(P)*P_SizeY(P),"Profil temporaire trop petit",return);
	CHECK(P_Data(Ptmp)==P_Data(P),"P_MedianFilter: JAMAIS FAIRE CA!",return);

	int y;
	for(y=FilterSize;y<P_SizeY(P)-FilterSize;y++)
	{
		int x;
	for(x=FilterSize;x<P_SizeX(P)-FilterSize;x++)
	{
		float MinOnX=V_Min(P_Data(P)[x-FilterSize+P_SizeX(P)*y],P_Data(P)[x+FilterSize+P_SizeX(P)*y]);
		float MaxOnX=V_Max(P_Data(P)[x-FilterSize+P_SizeX(P)*y],P_Data(P)[x+FilterSize+P_SizeX(P)*y]);
		float MinOnY=V_Min(P_Data(P)[x+P_SizeX(P)*(y-FilterSize)],P_Data(P)[x+P_SizeX(P)*(y+FilterSize)]);
		float MaxOnY=V_Max(P_Data(P)[x+P_SizeX(P)*(y-FilterSize)],P_Data(P)[x+P_SizeX(P)*(y+FilterSize)]);
		float MinC=V_Min(MinOnX,MinOnY);
		float MaxC=V_Max(MaxOnX,MaxOnY);
		float MinT=0.6f*MinC+0.4f*MaxC;
		float MaxT=0.4f*MinC+0.6f*MaxC;
		P_Data(Ptmp)[x+P_SizeX(P)*y]=V_Sature(P_Data(P)[x+P_SizeX(P)*y],MinT,MaxT);
	}
	}
	for(y=FilterSize;y<P_SizeY(P)-FilterSize;y++)
	{
	for(int x=FilterSize;x<P_SizeX(P)-FilterSize;x++)
	{
		P_Data(P)[x+P_SizeX(P)*y]=P_Data(Ptmp)[x+P_SizeX(P)*y];
	}
	}
	//SPG_Memcpy(P_Data(P),PtmP_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float));
	return;
}

void SPG_CONV P_Convolve(Profil& Pdest, Profil& Psrc, Profil& Kernel)
{
	CHECK(P_Etat(Psrc)==0,"P_Convolve",return);
	CHECK(P_Etat(Pdest)==0,"P_Convolve",return);
	CHECK(P_Etat(Kernel)==0,"P_Convolve",return);
	CHECK((P_SizeX(Pdest)!=P_SizeX(Psrc))||(P_SizeY(Pdest)!=P_SizeY(Psrc)),"P_Convolve",return);

	const int ox=P_SizeX(Kernel)>>1;
	const int oy=P_SizeY(Kernel)>>1;

	float* DstD=P_Data(Pdest);
	for(int y=0;y<P_SizeY(Psrc);y++)
	{
		for(int x=0;x<P_SizeX(Psrc);x++)
		{
			float Sum=0;
			float* KernelD=P_Data(Kernel);
			for(int j=0;j<P_SizeY(Kernel);j++)
			{
				int YR=(y-oy+j);
				float* SrcD=P_Data(Psrc)+V_Sature(YR,0,(P_SizeY(Psrc)-1))*P_SizeX(Psrc);
				for(int i=0;i<P_SizeX(Kernel);i++)
				{
					Sum+=SrcD[V_Sature((x+i-ox),0,(P_SizeX(Psrc)-1))]*KernelD[i];
				}
				KernelD+=P_SizeX(Kernel);
			}
			DstD[x]=Sum;
		}
		DstD+=P_SizeX(Pdest);
	}
	return;
}

void SPG_CONV P_ConvolveFast(Profil& Pdest, Profil& Psrc, Profil& Kernel)
{
	CHECK(P_Etat(Psrc)==0,"P_Convolve",return);
	CHECK(P_Etat(Pdest)==0,"P_Convolve",return);
	CHECK(P_Etat(Kernel)==0,"P_Convolve",return);
	CHECK((P_SizeX(Pdest)!=P_SizeX(Psrc))||(P_SizeY(Pdest)!=P_SizeY(Psrc)),"P_Convolve",return);

	const int ox=P_SizeX(Kernel)>>1;
	const int oy=P_SizeY(Kernel)>>1;
	const int oxr=(P_SizeX(Kernel)+1)>>1;
	const int oyr=(P_SizeY(Kernel)+1)>>1;

	float* DstD=P_Data(Pdest)+oy*P_SizeX(Pdest);
	for(int y=oy;y<P_SizeY(Psrc)-oyr;y++)
	{
		for(int x=ox;x<P_SizeX(Psrc)-oxr;x++)
		{
			float Sum=0;
			float* KernelD=P_Data(Kernel);
			float* SrcD=P_Data(Psrc)+(y-oy)*P_SizeX(Psrc)+x-ox;
			for(int j=0;j<P_SizeY(Kernel);j++)
			{
				for(int i=0;i<P_SizeX(Kernel);i++)
				{
					Sum+=SrcD[i]*KernelD[i];
				}
				KernelD+=P_SizeX(Kernel);
				SrcD+=P_SizeX(Psrc);
			}
			DstD[x]=Sum;
		}
		DstD+=P_SizeX(Pdest);
	}
	/*
	{
		for(int y=0;y<oy;y++)
		{
			for(int x=0;x<P_SizeX(Pdest);x++)
			{
				P_Element(Pdest,x,y)=0;
			}
		}
		for(y=P_SizeY(Pdest)-oyr;y<P_SizeY(Pdest);y++)
		{
			for(int x=0;x<P_SizeX(Pdest);x++)
			{
				P_Element(Pdest,x,y)=0;
			}
		}
		for(y=oy;y<P_SizeY(Pdest)-oyr;y++)
		{
			for(int x=0;x<ox;x++)
			{
				P_Element(Pdest,x,y)=0;
			}
			for(x=P_SizeX(Pdest)-oxr;x<P_SizeX(Pdest);x++)
			{
				P_Element(Pdest,x,y)=0;
			}
		}
	}
	*/
	return;
}

/*
void SPG_CONV P_ConvolveFast(Profil& Pdest, Profil& Psrc, Profil& Kernel)
{
	CHECK(P_Etat(Psrc)==0,"P_ConvolveFast",return);
	CHECK(P_Etat(Pdest)==0,"P_ConvolveFast",return);
	CHECK(P_Etat(Kernel)==0,"P_ConvolveFast",return);
	CHECK((P_SizeX(Pdest)!=P_SizeX(Psrc))||(P_SizeY(Pdest)!=P_SizeY(Psrc)),"P_Convolve",return);

	const int ox=P_SizeX(Kernel)>>1;
	const int oy=P_SizeY(Kernel)>>1;
	for(int y=oy;y<P_SizeY(Psrc)-(P_SizeY(Kernel)-oy);y++)
	{
		float* DestLine=P_Data(Pdest)+y*P_SizeX(Pdest);
		float* SrcLine=P_Data(Psrc)+(y-oy)*P_SizeX(Psrc)-ox;
		for(int x=ox;x<P_SizeX(Psrc)-(P_SizeX(Kernel)-ox);x++)
		{
			float Sum=0;
			for(int j=0;j<P_SizeY(Kernel);j++)
			{
				float* kLine=P_Data(Kernel)+j*P_SizeX(Kernel);
				float* rLine=SrcLine+j*P_SizeX(Psrc);
				for(int i=0;i<P_SizeX(Kernel);i++)
				{
					Sum+=rLine[i]*kLine[i];
				}
			}
			DestLine[x]=Sum;
		}
	}
	return;
}
*/

#define P_FASTCONV 255

void SPG_CONV P_FastConvHighPassG(short* D, int Step, int N, int FilterSize, int nBitsShr)
{//convolution ligne par ligne
	CHECK(D==0,"P_FastConvLowPassG: Profil nul",return);
	CHECK(N<3,"P_FastConvLowPassG",return);
	FilterSize=V_Min(FilterSize,N-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	//const int InvSZ=1.0f/FilterSize;
	int Ring[P_FASTCONV+1];
	short const* restrict pLine=D; short* restrict pLmW=D;//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	int Sum=0;
	int x;
	for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}
	for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);pLine+=Step;}
	for(;x<N;x++) {*pLmW-=(Sum>>nBitsShr);pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*pLine)-Ring[(x-FilterSize)&P_FASTCONV];pLine+=Step;}
	for(;x<=N+Wing;x++) {*pLmW-=(Sum>>nBitsShr);pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*(pLine-Step))-Ring[(x-FilterSize)&P_FASTCONV];}
	return;
}

//fonction générique de convolution d'une ligne
void SPG_CONV P_FastConvHighPassG(float* D, int Step, int N, int FilterSize)
{//convolution ligne par ligne
	CHECK(D==0,"P_FastConvLowPassG: Profil nul",return);
	CHECK(N<3,"P_FastConvLowPassG",return);
	FilterSize=V_Min(FilterSize,N-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	const float InvSZ=1.0f/FilterSize;
	float Ring[P_FASTCONV+1];
	float const* restrict pLine=D; float* restrict pLmW=D;//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	float Sum=0;
	int x;
	for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}
	for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);pLine+=Step;}
	for(;x<N;x++) {*pLmW-=Sum*InvSZ;pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*pLine)-Ring[(x-FilterSize)&P_FASTCONV];pLine+=Step;}
	for(;x<=N+Wing;x++) {*pLmW-=Sum*InvSZ;pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*(pLine-Step))-Ring[(x-FilterSize)&P_FASTCONV];}
	return;
}

void SPG_CONV P_FastConvLowPassG(short* D, int Step, int N, int FilterSize, int nBitsShr)
{//convolution ligne par ligne
	CHECK(D==0,"P_FastConvLowPassG: Profil nul",return);
	CHECK(N<3,"P_FastConvLowPassG",return);
	FilterSize=V_Min(FilterSize,N-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	//const int InvSZ=1.0f/FilterSize;
	int Ring[P_FASTCONV+1];
	short const* restrict pLine=D; short* restrict pLmW=D;//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	int Sum=0;
	int x;
	for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}
	for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);pLine+=Step;}
	for(;x<N;x++) {*pLmW=(Sum>>nBitsShr);pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*pLine)-Ring[(x-FilterSize)&P_FASTCONV];pLine+=Step;}
	for(;x<=N+Wing;x++) {*pLmW=(Sum>>nBitsShr);pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*(pLine-Step))-Ring[(x-FilterSize)&P_FASTCONV];}
	return;
}

//fonction générique de convolution d'une ligne
void SPG_CONV P_FastConvLowPassG(float* D, int Step, int N, int FilterSize)
{//convolution ligne par ligne
	CHECK(D==0,"P_FastConvLowPassG: Profil nul",return);
	CHECK(N<3,"P_FastConvLowPassG",return);
	FilterSize=V_Min(FilterSize,N-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	const float InvSZ=1.0f/FilterSize;
	float Ring[P_FASTCONV+1];
	float const* restrict pLine=D; float* restrict pLmW=D;//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	float Sum=0;
	int x;
	for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}
	for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);pLine+=Step;}
	for(;x<N;x++) {*pLmW=Sum*InvSZ;pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*pLine)-Ring[(x-FilterSize)&P_FASTCONV];pLine+=Step;}
	for(;x<=N+Wing;x++) {*pLmW=Sum*InvSZ;pLmW+=Step;
		Sum+=(Ring[x&P_FASTCONV]=*(pLine-Step))-Ring[(x-FilterSize)&P_FASTCONV];}
	return;
}

//fonction générique de convolution d'une phase (décomposée en rad + nr de tour)
void SPG_CONV P_FastConvLowPassPhase(float* D, int Step, int* K, int KStep, int N, int FilterSize)
{//convolution ligne par ligne
	CHECK(D==0,"P_FastConvLowPassG: Profil nul",return);
	CHECK(N<3,"P_FastConvLowPassG",return);
	FilterSize=V_Min(FilterSize,N-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	const float InvSZ=1.0f/FilterSize;
	float Ring[P_FASTCONV+1];
	int KRing[P_FASTCONV+1];
	float const* restrict pLine=D; float* restrict pLmW=D;//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	int const* restrict pKLine=K; int* restrict pKLmW=K;//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	float Sum=0; int KSum=0;
	int x;
	for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);KSum+=(KRing[x&P_FASTCONV]=*pKLine);}
	for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);pLine+=Step;KSum+=(KRing[x&P_FASTCONV]=*pKLine);pKLine+=KStep;}

	for(;x<N;x++) 
	{
		*pLmW=(Sum+(KSum%FilterSize)*V_DPI)*InvSZ; pLmW+=Step;
		//Q=KSum%FilterSize et (KSum-Q)/FilterSize si la convention entre % et /  n'est pas consistante (avec les nombres négatifs ca dépend du langage)
		*pKLmW=KSum/FilterSize; pKLmW+=KStep;

		Sum+=(Ring[x&P_FASTCONV]=*pLine)-Ring[(x-FilterSize)&P_FASTCONV];pLine+=Step;
		KSum+=(KRing[x&P_FASTCONV]=*pKLine)-KRing[(x-FilterSize)&P_FASTCONV];pKLine+=KStep;
	}

	for(;x<=N+Wing;x++) 
	{
		*pLmW=(Sum+(KSum%FilterSize)*V_DPI)*InvSZ; pLmW+=Step;
		*pKLmW=KSum/FilterSize; pKLmW+=KStep;
		//Q=KSum%FilterSize et (KSum-Q)/FilterSize si la convention entre % et /  n'est pas consistante (avec les nombres négatifs ca dépend du langage)
		Sum+=(Ring[x&P_FASTCONV]=*(pLine-Step))-Ring[(x-FilterSize)&P_FASTCONV];
		KSum+=(KRing[x&P_FASTCONV]=*(pKLine-KStep))-KRing[(x-FilterSize)&P_FASTCONV];
	}
	return;
}

void SPG_CONV P_FastConvLowPass3D(float* D, int SizeX, int SizeY, int SizeZ, int FilterSizeX, int FilterSizeY, int FilterSizeZ)
{
	{
		//ligne par ligne
		float* Dz=D;//pointeur sur l'image
		for(int z=0;z<SizeZ;z++)
		{
			float* Dy=Dz;//pointeur sur la ligne
			for(int y=0;y<SizeY;y++)
			{
				P_FastConvLowPassG(Dy,1,SizeX,FilterSizeX);
				Dy+=SizeX;
			}
			Dz+=SizeX*SizeY;
		}
	}
	{
		//colonne par colonne
		float* Dz=D;//pointeur sur l'image
		for(int z=0;z<SizeZ;z++)
		{
			float* Dx=Dz;//pointeur sur la colonne
			for(int x=0;x<SizeX;x++)
			{
				P_FastConvLowPassG(Dx,SizeX,SizeY,FilterSizeY);
				Dx++;
			}
			Dz+=SizeX*SizeY;
		}
	}
	{
		//pixel par pixel
		float* Dy=D;//pointeur sur la ligne
		for(int y=0;y<SizeY;y++)
		{
			float* Dx=Dy;//pointeur sur la colonne
			for(int x=0;x<SizeX;x++)
			{
				P_FastConvLowPassG(Dx,SizeX*SizeY,SizeZ,FilterSizeZ);
				Dx++;
			}
			Dy+=SizeX;
		}
	}
	return;
}

void SPG_CONV P_FastConvLowPassH(const Profil& P, int FilterSize)
{//convolution ligne par ligne
	CHECK(P_Etat(P)==0,"P_FastConvLowPassH: Profil nul",return);
	CHECK(P_Data(P)==0,"P_FastConvLowPassH: Profil vide",return);
	CHECK(P_SizeX(P)<3,"P_FastConvLowPassH",return);
	FilterSize=V_Min(FilterSize,P_SizeX(P)-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	const float InvSZ=1.0f/FilterSize;
	float Ring[P_FASTCONV+1];
	float const* restrict pLine=P_Data(P); float* restrict pLmW=P_Data(P);//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	for(int y=0;y<P_SizeY(P);y++)
	{
		float Sum=0;
		int x;
		for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}//init bord ext gauche
		for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine++);}//init bord int gauche
		for(;x<P_SizeX(P);x++) {*pLmW++=Sum*InvSZ;	//ecriture du resultat
			Sum+=(Ring[x&P_FASTCONV]=*pLine++)-Ring[(x-FilterSize)&P_FASTCONV];}//actualisation de la moyenne glissante
		for(;x<=P_SizeX(P)+Wing;x++) {*pLmW++=Sum*InvSZ;//ecriture du resultat sur le bord interieur droit
			Sum+=(Ring[x&P_FASTCONV]=*(pLine-1))-Ring[(x-FilterSize)&P_FASTCONV];}
	}
	return;
}

void SPG_CONV P_FastConvLowPassV(const Profil& P, int FilterSize)
{//convolution colonne par colonne
	CHECK(P_Etat(P)==0,"P_FastConvLowPassV: Profil nul",return);
	CHECK(P_Data(P)==0,"P_FastConvLowPassV: Profil vide",return);
	CHECK(P_SizeX(P)<3,"P_FastConvLowPassV",return);
	FilterSize=V_Min(FilterSize,P_SizeY(P)-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	const float InvSZ=1.0f/FilterSize;
	float Ring[P_FASTCONV+1];
	float const* restrict pLine=P_Data(P); float* restrict pLmW=P_Data(P);//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	for(int y=0;y<P_SizeX(P);y++)
	{
		float Sum=0;
		int x;
		for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}
		for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);pLine+=P_SizeX(P);}
		for(;x<P_SizeY(P);x++) {*pLmW=Sum*InvSZ;pLmW+=P_SizeX(P);
			Sum+=(Ring[x&P_FASTCONV]=*pLine)-Ring[(x-FilterSize)&P_FASTCONV];pLine+=P_SizeX(P);}
		for(;x<=P_SizeY(P)+Wing;x++) {*pLmW=Sum*InvSZ;pLmW+=P_SizeX(P);
			Sum+=(Ring[x&P_FASTCONV]=*(pLine-P_SizeX(P)))-Ring[(x-FilterSize)&P_FASTCONV];}
		pLmW+=1-P_SizeY(P)*P_SizeX(P);//colonne suivante
		pLine+=1-P_SizeY(P)*P_SizeX(P);
	}
	return;
}

void SPG_CONV P_FastConvHighPassH(const Profil& P, int FilterSize)
{//convolution ligne par ligne
	CHECK(P_Etat(P)==0,"P_FastConvHighPassH: Profil nul",return);
	CHECK(P_Data(P)==0,"P_FastConvHighPassH: Profil vide",return);
	CHECK(P_SizeX(P)<3,"P_FastConvHighPassH",return);
	FilterSize=V_Min(FilterSize,P_SizeX(P)-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	const float InvSZ=1.0f/FilterSize;
	float Ring[P_FASTCONV+1];
	float const* restrict pLine=P_Data(P); float* restrict pLmW=P_Data(P);//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	for(int y=0;y<P_SizeY(P);y++)
	{
		float Sum=0;
		int x;
		for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}
		for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine++);}
		for(;x<P_SizeX(P);x++) {*pLmW++-=Sum*InvSZ;	
			Sum+=(Ring[x&P_FASTCONV]=*pLine++)-Ring[(x-FilterSize)&P_FASTCONV];}
		for(;x<=P_SizeX(P)+Wing;x++) {*pLmW++-=Sum*InvSZ;
			Sum+=(Ring[x&P_FASTCONV]=*(pLine-1))-Ring[(x-FilterSize)&P_FASTCONV];}
	}
	return;
}

void SPG_CONV P_FastConvHighPassV(const Profil& P, int FilterSize)
{//convolution colonne par colonne
	CHECK(P_Etat(P)==0,"P_FastConvHighPassV: Profil nul",return);
	CHECK(P_Data(P)==0,"P_FastConvHighPassV: Profil vide",return);
	CHECK(P_SizeX(P)<3,"P_FastConvHighPassV",return);
	FilterSize=V_Min(FilterSize,P_SizeX(P)-1);
	FilterSize=V_Sature(FilterSize,3,P_FASTCONV); FilterSize|=1;
	const int Wing=FilterSize/2;
	const float InvSZ=1.0f/FilterSize;
	float Ring[P_FASTCONV+1];
	float const* restrict pLine=P_Data(P); float* restrict pLmW=P_Data(P);//pointeur de lecture sur donnees const, pointeur d'ecriture décalé au centre de l'intervalle de moyennage
	for(int y=0;y<P_SizeX(P);y++)
	{
		float Sum=0;
		int x;
		for(x=-Wing;x<0;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);}
		for(;x<=Wing;x++) {Sum+=(Ring[x&P_FASTCONV]=*pLine);pLine+=P_SizeX(P);}
		for(;x<P_SizeY(P);x++) {*pLmW-=Sum*InvSZ;pLmW+=P_SizeX(P);
			Sum+=(Ring[x&P_FASTCONV]=*pLine)-Ring[(x-FilterSize)&P_FASTCONV];pLine+=P_SizeX(P);}
		for(;x<=P_SizeY(P)+Wing;x++) {*pLmW-=Sum*InvSZ;pLmW+=P_SizeX(P);
			Sum+=(Ring[x&P_FASTCONV]=*(pLine-P_SizeX(P)))-Ring[(x-FilterSize)&P_FASTCONV];}
		//colonne suivante
		pLmW+=1-P_SizeY(P)*P_SizeX(P);
		pLine+=1-P_SizeY(P)*P_SizeX(P);
	}
	return;
}

void SPG_CONV P_FastConvLowPass(const Profil& P, const int FilterSize)
{//convolution par une matrice (FilterSize|1) x (FilterSize|1) centree
	CHECK(P_Etat(P)==0,"P_FastConvLowPass: Profil nul",return);
	CHECK(P_Data(P)==0,"P_FastConvLowPass: Profil vide",return);
	P_FastConvLowPassH(P,FilterSize);
	P_FastConvLowPassV(P,FilterSize);
	return;
}

void SPG_CONV P_FastConvHighPass(const Profil& P, const int FilterSize)
{//convolution par une matrice (FilterSize|1) x (FilterSize|1) centree
	CHECK(P_Etat(P)==0,"P_FastConvHighPass: Profil nul",return);
	CHECK(P_Data(P)==0,"P_FastConvHighPass: Profil vide",return);
	P_FastConvHighPassH(P,FilterSize);
	P_FastConvHighPassV(P,FilterSize);
	return;
}

//atttention si le profil est P_Alias
void SPG_CONV P_MaskConv3x3(Profil& P)
{
	CHECK(P_Etat(P)==0,"P_MaskConv3x3: Profil nul",return);
	CHECK(P_Data(P)==0,"P_MaskConv3x3: Profil vide",return);

	if(P_Msk(P)==0) 
	{
		P_FastConvLowPass(P,3);
		return;
	}

	Profil Ptmp;
	P_Duplicate(Ptmp,P);

	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(P_ElementMsk(P,x,y)==0)
			{
				int NTermes=0;
				float Sum=0;
				for(int ys=V_Max(y-1,0);ys<=V_Min(y+1,P_SizeY(P)-1);ys++)
				{
					for(int xs=V_Max(x-1,0);xs<=V_Min(x+1,P_SizeX(P)-1);xs++)
					{
						if(P_ElementMsk(P,xs,ys)==0)
						{
							Sum+=P_Element(P,xs,ys);
							NTermes++;
						}
					}
				}
				if(NTermes) 
				{
					P_Element(Ptmp,x,y)=Sum/NTermes;
				}
				else
					P_ElementMsk(P,x,y)=1;
			}
		}
	}


//operation INTERDITE si le profil est un alias	car d'anciens pointeurs
//subsistent ailleurs (vue que c'est un alias, c'est qu'un profil original
//existe et verra ses donnees indument desallouees

	if(P_Etat(P)&P_Alias)
	{
		P_Copy(P,Ptmp);
	}
	else
	{
		V_Swap(float*,P_Data(P),P_Data(Ptmp));
	}
	P_Close(Ptmp);
	return;
}

void SPG_CONV P_MorphoFilterMax(Profil& Pdest, Profil& Psrc, Profil& Kernel)
{
	CHECK(P_Etat(Psrc)==0,"P_MorphoFilterMax",return);
	CHECK(P_Etat(Pdest)==0,"P_MorphoFilterMax",return);
	CHECK(P_Etat(Kernel)==0,"P_MorphoFilterMax",return);
	CHECK((P_SizeX(Pdest)!=P_SizeX(Psrc))||(P_SizeY(Pdest)!=P_SizeY(Psrc)),"P_MorphoFilterMax",return);

	const int ox=P_SizeX(Kernel)>>1;
	const int oy=P_SizeY(Kernel)>>1;

	float* DstD=P_Data(Pdest);
	for(int y=0;y<P_SizeY(Psrc);y++)
	{
		for(int x=0;x<P_SizeX(Psrc);x++)
		{
			float Sum=-FLT_MAX;
			float* KernelD=P_Data(Kernel);
			for(int j=0;j<P_SizeY(Kernel);j++)
			{
				int YR=(y-oy+j);
				float* SrcD=P_Data(Psrc)+V_Sature(YR,0,(P_SizeY(Psrc)-1))*P_SizeX(Psrc);
				for(int i=0;i<P_SizeX(Kernel);i++)
				{
					float h=SrcD[V_Sature((x+i-ox),0,(P_SizeX(Psrc)-1))]-KernelD[i];
					Sum=V_Max(Sum,h);
				}
				KernelD+=P_SizeX(Kernel);
			}
			DstD[x]=Sum;
		}
		DstD+=P_SizeX(Pdest);
	}
	return;
}

void SPG_CONV P_MorphoFilterMin(Profil& Pdest, Profil& Psrc, Profil& Kernel)
{
	CHECK(P_Etat(Psrc)==0,"P_MorphoFilterMin",return);
	CHECK(P_Etat(Pdest)==0,"P_MorphoFilterMin",return);
	CHECK(P_Etat(Kernel)==0,"P_MorphoFilterMin",return);
	CHECK((P_SizeX(Pdest)!=P_SizeX(Psrc))||(P_SizeY(Pdest)!=P_SizeY(Psrc)),"P_MorphoFilterMin",return);

	const int ox=P_SizeX(Kernel)>>1;
	const int oy=P_SizeY(Kernel)>>1;

	float* DstD=P_Data(Pdest);
	for(int y=0;y<P_SizeY(Psrc);y++)
	{
		for(int x=0;x<P_SizeX(Psrc);x++)
		{
			float Sum=FLT_MAX;
			float* KernelD=P_Data(Kernel);
			for(int j=0;j<P_SizeY(Kernel);j++)
			{
				int YR=(y-oy+j);
				float* SrcD=P_Data(Psrc)+V_Sature(YR,0,(P_SizeY(Psrc)-1))*P_SizeX(Psrc);
				for(int i=0;i<P_SizeX(Kernel);i++)
				{
					float h=SrcD[V_Sature((x+i-ox),0,(P_SizeX(Psrc)-1))]+KernelD[i];
					Sum=V_Min(Sum,h);
				}
				KernelD+=P_SizeX(Kernel);
			}
			DstD[x]=Sum;
		}
		DstD+=P_SizeX(Pdest);
	}
	return;
}

void SPG_CONV P_MorphoFilterMinMax(Profil& PMin, Profil& PMax, const Profil& Psrc, const Profil& Kernel)
{
	CHECK(P_Etat(Psrc)==0,"P_MorphoFilterMinMax",return);
	CHECK(P_Etat(PMin)==0,"P_MorphoFilterMinMax",return);
	CHECK(P_Etat(PMax)==0,"P_MorphoFilterMinMax",return);
	CHECK(P_Etat(Kernel)==0,"P_MorphoFilterMinMax",return);
	CHECK((P_SizeX(PMin)!=P_SizeX(Psrc))||(P_SizeY(PMin)!=P_SizeY(Psrc)),"P_MorphoFilterMinMax",return);
	CHECK((P_SizeX(PMax)!=P_SizeX(Psrc))||(P_SizeY(PMax)!=P_SizeY(Psrc)),"P_MorphoFilterMinMax",return);

	const int ox=P_SizeX(Kernel)>>1;
	const int oy=P_SizeY(Kernel)>>1;
	const int xm=(P_SizeX(Psrc)-1);

	float* DstDMin=P_Data(PMin);
	float* DstDMax=P_Data(PMax);
	for(int y=0;y<P_SizeY(Psrc);y++)
	{
		const int yoC=y-oy;
		for(int x=0;x<P_SizeX(Psrc);x++)
		{
			float DN=FLT_MAX;
			float UP=-FLT_MAX;
			const float* KernelD=P_Data(Kernel);
			const int xoC=x-ox;
			for(int j=0;j<P_SizeY(Kernel);j++)
			{
				float* SrcD=P_Data(Psrc)+V_Sature((yoC+j),0,(P_SizeY(Psrc)-1))*P_SizeX(Psrc);
				for(int i=0;i<P_SizeX(Kernel)-1;i+=2)
				{
					int xs0=V_Sature((i+xoC),0,xm);
					int xs1=V_Sature((i+1+xoC),0,xm);
					float S0=SrcD[xs0];
					float S1=SrcD[xs1];
					float K0=KernelD[i];
					float K1=KernelD[i+1];
					float d0=S0+K0;
					float d1=S1+K1;
					float u0=S0-K0;
					float u1=S1-K1;
					DN=V_Min(DN,d0);
					UP=V_Max(UP,u0);
					DN=V_Min(DN,d1);
					UP=V_Max(UP,u1);
				}
				KernelD+=P_SizeX(Kernel);
			}
			DstDMin[x]=DN;
			DstDMax[x]=UP;
		}
		DstDMin+=P_SizeX(PMin);
		DstDMax+=P_SizeX(PMax);
	}
	return;
}

void SPG_CONV P_CircularHighPass(Profil& Pdst, Profil& Psrc, int HighPassRadius)
{
	CHECK(P_Etat(Psrc)==0,"P_CircularHighPass: Profil nul",return);
	CHECK(P_Data(Psrc)==0,"P_CircularHighPass: Profil vide",return);
	CHECK(P_Etat(Pdst)==0,"P_CircularHighPass: Profil nul",return);
	CHECK(P_Data(Pdst)==0,"P_CircularHighPass: Profil vide",return);
	CHECK((P_SizeX(Pdst)!=P_SizeX(Psrc))||(P_SizeY(Pdst)!=P_SizeY(Psrc)),"P_CircularHighPass",return);

	int* RadiusLUT=SPG_TypeAlloc(1+2*HighPassRadius,int,"RadiusLUT");
	{
		for(int ym=0;ym<=2*HighPassRadius;ym++)
			RadiusLUT[ym]=sqrtf(HighPassRadius*HighPassRadius-(ym-HighPassRadius)*(ym-HighPassRadius));
	}

	P_Copy(Pdst,Psrc);

	int sx=P_SizeX(Psrc);
	int sy=P_SizeY(Psrc);
	int sxm1=sx-1;
	int sym1=sy-1;
	
	for(int y=0;y<sy;y++)
	{
		float* D2=P_Data(Pdst)+y*sx;
		BYTE* M2=P_Msk(Pdst)+y*sx;
		for(int x=0;x<sx;x++)
		{
			float Sum=0;
			int Count=0;
			for(int ym=V_Max((y-HighPassRadius),0);ym<=V_Min((y+HighPassRadius),sym1);ym++)
			{
				int R=RadiusLUT[ym-y+HighPassRadius];
				float* D=P_Data(Psrc)+ym*sx;
				BYTE* M=P_Msk(Psrc)+ym*sx;
				for(int xm=V_Max((x-R),0);xm<=V_Min((x+R),sxm1);xm++)
				{
					if(M[xm]==0)
					{
						Count++;
						Sum+=D[xm];
					}
				}
			}
			if(Count)
			{
				D2[x]-=Sum/Count;
			}
			else
			{
				D2[x]=0;
				M2[x]=1;
			}
		}
	}
	SPG_MemFree(RadiusLUT);
	return;
}

void SPG_CONV P_LowPassMax(const Profil& PCourant, const Profil& Reference, const int KernelSize, const int IterMaxReplace)
{
	CHECK(P_Etat(PCourant)==0,"P_LowPassMax",return);
	CHECK(P_Etat(Reference)==0,"P_LowPassMax",return);
	CHECK((P_SizeX(PCourant)!=P_SizeX(Reference))||(P_SizeY(PCourant)!=P_SizeY(Reference)),"P_LowPassMax",return);
	const float TimeCste=2/(3.0f+IterMaxReplace);
	for(int k=0;k<IterMaxReplace;k++)
	{
		for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
		{
				float D=P_Data(Reference)[i]-P_Data(PCourant)[i];
				if(D>0) {P_Data(PCourant)[i]+=2*D;}
				else {P_Data(PCourant)[i]+=TimeCste*D;}
		}
		P_FastConvLowPass(PCourant, KernelSize);
	}
	return;
}

void SPG_CONV P_LowPassMin(const Profil& PCourant, const Profil& Reference, const int KernelSize, const int IterMaxReplace)
{
	CHECK(P_Etat(PCourant)==0,"P_LowPassMin",return);
	CHECK(P_Etat(Reference)==0,"P_LowPassMin",return);
	CHECK((P_SizeX(PCourant)!=P_SizeX(Reference))||(P_SizeY(PCourant)!=P_SizeY(Reference)),"P_LowPassMin",return);
	const float TimeCste=2/(3.0f+IterMaxReplace);
	for(int k=0;k<IterMaxReplace;k++)
	{
		for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
		{
				float D=P_Data(Reference)[i]-P_Data(PCourant)[i];
				if(D<0) {P_Data(PCourant)[i]+=2*D;}
				else {P_Data(PCourant)[i]+=TimeCste*D;}
		}
		P_FastConvLowPass(PCourant, KernelSize);
	}
	return;
}
/*
void SPG_CONV P_LowPassMax(Profil& PCourant, Profil& Reference, int KernelSize, int IterMaxReplace, float TimeCste)
{
	CHECK(P_Etat(PCourant)==0,"P_LowPassMax",return);
	CHECK(P_Etat(Reference)==0,"P_LowPassMax",return);
	CHECK((P_SizeX(PCourant)!=P_SizeX(Reference))||(P_SizeY(PCourant)!=P_SizeY(Reference)),"P_LowPassMax",return);
	const int xStart=KernelSize/2;
	const int yStart=KernelSize/2;
	const int xStop=P_SizeX(PCourant)-KernelSize/2;
	const int yStop=P_SizeY(PCourant)-KernelSize/2;
	for(int k=0;k<IterMaxReplace;k++)
	{
		P_FastConvLowPass(PCourant, KernelSize);
		for(int y=yStart;y<yStop;y++)
		{
			float	   * const DCourant=P_Data(PCourant)+y*P_SizeX(PCourant);
			float const* const DReference=P_Data(Reference)+y*P_SizeX(Reference);
			for(int x=xStart;x<xStop;x++)
			{
				float D=DReference[x]-DCourant[x];
				if(D>0)
				{
					DCourant[x]+=2*D;
				}
				else
				{
					DCourant[x]+=TimeCste*D;
				}
			}
		}
	}
	return;
}

void SPG_CONV P_LowPassMin(Profil& PCourant, Profil& Reference, int KernelSize, int IterMaxReplace, float TimeCste)
{
	CHECK(P_Etat(PCourant)==0,"P_LowPassMin",return);
	CHECK(P_Etat(Reference)==0,"P_LowPassMin",return);
	CHECK((P_SizeX(PCourant)!=P_SizeX(Reference))||(P_SizeY(PCourant)!=P_SizeY(Reference)),"P_LowPassMin",return);
	const int xStart=KernelSize/2;
	const int yStart=KernelSize/2;
	const int xStop=P_SizeX(PCourant)-KernelSize/2;
	const int yStop=P_SizeY(PCourant)-KernelSize/2;
	for(int k=0;k<IterMaxReplace;k++)
	{
		P_FastConvLowPass(PCourant, KernelSize);
		for(int y=yStart;y<yStop;y++)
		{
			float	   * const DCourant=P_Data(PCourant)+y*P_SizeX(PCourant);
			float const* const DReference=P_Data(Reference)+y*P_SizeX(Reference);
			for(int x=xStart;x<xStop;x++)
			{
				float D=DReference[x]-DCourant[x];
				if(D<0)
				{
					DCourant[x]+=2*D;
				}
				else
				{
					DCourant[x]+=TimeCste*D;
				}
			}
		}
	}
	P_FastConvLowPass(PCourant, KernelSize);
	return;
}
*/
void SPG_CONV P_LowPassMaxP(Profil& PCourant, Profil& Reference, int KernelSize, int IterMaxReplace, float Exposant)
{
	{for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
	{
		P_Data(PCourant)[i]=pow(P_Data(PCourant)[i],Exposant);
		P_Data(Reference)[i]=pow(P_Data(Reference)[i],Exposant);
	}}
	for(int k=0;k<IterMaxReplace;k++)
	{
		P_FastConvLowPass(PCourant, KernelSize);
		for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
		{
			if(P_Data(PCourant)[i]<P_Data(Reference)[i])
			{
				P_Data(PCourant)[i]+=2*(P_Data(Reference)[i]-P_Data(PCourant)[i]);
			}
		}
	}
	P_FastConvLowPass(PCourant, KernelSize);
	{for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
	{
		P_Data(PCourant)[i]=V_Max(P_Data(PCourant)[i],P_Data(Reference)[i]);
	}}
	{for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
	{
		if(P_Data(PCourant)[i]<=0) 
		{
			P_Data(PCourant)[i]=0;
		}
		else
		{
			P_Data(PCourant)[i]=pow(P_Data(PCourant)[i],1/Exposant);
		}
		P_Data(Reference)[i]=pow(P_Data(Reference)[i],1/Exposant);
	}}
	return;
}

void SPG_CONV P_LowPassMinP(Profil& PCourant, Profil& Reference, int KernelSize, int IterMaxReplace, float Exposant)
{
	{for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
	{
		P_Data(PCourant)[i]=pow(P_Data(PCourant)[i],Exposant);
		P_Data(Reference)[i]=pow(P_Data(Reference)[i],Exposant);
	}}
	for(int k=0;k<IterMaxReplace;k++)
	{
		P_FastConvLowPass(PCourant, KernelSize);
		for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
		{
			if(P_Data(PCourant)[i]>P_Data(Reference)[i])
			{
				P_Data(PCourant)[i]+=2*(P_Data(Reference)[i]-P_Data(PCourant)[i]);
			}
			P_Data(PCourant)[i]=V_Min(P_Data(PCourant)[i],P_Data(Reference)[i]);
		}
	}
	P_FastConvLowPass(PCourant, KernelSize);
	{for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
	{
		P_Data(PCourant)[i]=V_Min(P_Data(PCourant)[i],P_Data(Reference)[i]);
	}}
	{for(int i=0;i<P_SizeX(PCourant)*P_SizeY(PCourant);i++)
	{
		if(P_Data(PCourant)[i]<=0) 
		{
			P_Data(PCourant)[i]=0;
		}
		else
		{
			P_Data(PCourant)[i]=pow(P_Data(PCourant)[i],1/Exposant);
		}
		P_Data(Reference)[i]=pow(P_Data(Reference)[i],1/Exposant);
	}}
	return;
}
#endif

