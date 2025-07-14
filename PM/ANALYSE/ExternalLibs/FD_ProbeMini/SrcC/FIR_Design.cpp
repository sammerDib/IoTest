
#include "SPG_General.h"

#ifdef SPG_General_USEFirDesign

#include "SPG_Includes.h"

#include <memory.h>

#define FIR_UnitLabel "x"

#ifdef UseFIRRect
//calcul du filtre convolutif
//sort une Cut complexe dont les parties reelle et imaginaire sont en quadrature
//MaxDetect_FIRLen est la longueur voulue du filtre (preferablement impaire)
//lambda min/max definit le gabarit (la frequence normalisee est z_step/lambda)
int SPG_CONV FD_InitFIR(CutX& MaxDetect_FIR, int MaxDetect_FIRLen, int MaxIter, float LambdaMin, float LambdaMax, float Z_Step)
{
	SPG_CHECK(MaxDetect_FIRLen<1,"FD_InitFIR: Parametre invalide",return 0);
	//pas assez echantillonné: moins de deux echantillons par frange
	SPG_CHECK(LambdaMin<2*Z_Step,"FD_InitFIR: Diminuer le pas d'echantillonnage",return 0);
	//la longueur du filtre est trop basse (moins de deux franges)
	SPG_CHECK(LambdaMax>MaxDetect_FIRLen*Z_Step,"FD_InitFIR: Allongez le filtre",return 0);
	SPG_CHECK(LambdaMin==LambdaMax,"FD_InitFIR: Longueur de cohérence infinie",return 0);
	
	CutX C;
	//Cut CRe;
	//Cut CIm;
	
	//calcule une taille de travail adaptée à la FFT et à la précision en fréquence voulue
	int NumS=SFFT_GetAppropriateSize(V_FloatToInt(4*(MaxDetect_FIRLen+(LambdaMax+LambdaMin)/(LambdaMax-LambdaMin))),FFT_MARGE_100);

	if(CutX_Create(C,NumS,Z_Step,FIR_UnitLabel,"Intensite")==0) return 0;
	SPG_SetMemName(C.D,"FD_InitFir CutX");
	//Cut_Create(CRe,NumS,Z_Step,FIR_UnitLabel,"Intensite");
	//Cut_Create(CIm,NumS,Z_Step,FIR_UnitLabel,"Intensite");
	
	//position du gabarit: frequence normalisee Z_Step/LambdaMax,
	//longueur de l'axe des frequences C.NumS
	float fMin=C.NumS*Z_Step/LambdaMax;
	float fMax=C.NumS*Z_Step/LambdaMin;
	int ifMin=(int)ceil(fMin);
	int ifMax=(int)floor(fMax);
	float E=0.1f;
	while(ifMin>=ifMax-1)
	{
		E+=0.1f;
		ifMin=(int)ceil(fMin-E);
		ifMax=(int)floor(fMax+E);
	}
	
	//genere le filtre dans l'espace de fourier
	
	for(int i=V_FloatToInt(fMin);i<=V_FloatToInt(fMax);i++)
	{
		C.D[i].re=1;
	}
	
	//passe en espace reel
	
	CutX_FFT(C);
	CutX_Conjugue(C);
	
	for(int NIter=0;NIter<MaxIter;NIter++)
	{
		
		//force des ajustements
		float Vitesse=1.0f-(float)NIter/(MaxIter-1);
		
		//CutX_GetRe(C,CRe);
		//CutX_GetIm(C,CIm);
		
		//trace dans l'espace reel
		//Cut_Draw2(CRe,CIm,G_Direct,0xff0000,0x000ff00,CL);
		
		
		//force le gabarit en espace reel
		
		{
			float VitesseToZero=V_Max((2*Vitesse-1),0);
			if(NIter>0.9*MaxIter)
			{
				SPG_DbgCHECK(VitesseToZero!=0.0,"Debug test");
			}
			
			for(i=((1+MaxDetect_FIRLen)>>1);i<C.NumS-(MaxDetect_FIRLen>>1);i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re;
				C.D[i].im=VitesseToZero*C.D[i].im;
			}
		}
		/*
		if(NIter==MaxIter-1)
		{
		CutX_GetRe(C,CRe);
		CutX_GetIm(C,CIm);
		Cut_Save(CRe,"LC_RE");
		Cut_Save(CIm,"LC_IM");
		}
		*/
		
		//G L=3 XL=2 L=4 XL=6 L=5 XL=3
		//D L=3 XR=6 L=4 XL=5 L=5 XL=4
		
		//0 1 2 3 4 5 6 7
		//1 2           0 L=3 (L+1)/2 7-L/2
		//2 3         0 1 L=4 (L+1)/2 7-L/2
		//2 3 4       0 1 L=5 (L+1)/2 7-L/2
		//3 4 5     0 1 2 L=6 (L+1)/2 7-L/2
		
		CutX_FFT(C);
		CutX_Conjugue(C);
		
		//force le gabarit dans l'espace de fourier
		float ReponseMoyenne=0;//reponse dans la bande passante
		for(int i=ifMin+1;i<=ifMax-1;i++)
		{
			ReponseMoyenne+=C.D[i].re;
		}
		
		//facteur de normalisation assurant la stabilite de l'algorithme
		float Rescale=(1.0f+fMax-fMin)/ReponseMoyenne;
		
		CutX_MulR(C,Rescale);
		
		SPG_StackAllocZ(SPG_COMPLEX,Zero);
		
		{
			float VitesseToZero=1.1f-1.2f*Vitesse;
			if(VitesseToZero<0.0f) VitesseToZero=0.0f;
			if(VitesseToZero>1.0f) VitesseToZero=1.0f;
			if(NIter>0.92f*MaxIter)
			{
				SPG_DbgCHECK(VitesseToZero!=1.0f,"Debug test");
			}
			for(i=ifMin+1;i<=ifMax-1;i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re+(1.0f-VitesseToZero);
				//sur les FIR pairs la partie imaginaire n'est pas nulle
				C.D[i].im=VitesseToZero*C.D[i].im;//0;
			}
		}
		//ces contraintes sont rapidement relachees, elles affectent le rapport s/b
		{
			float VitesseToZero=1.0f-6.0f*(Vitesse-0.2f)*(1.0f-Vitesse);
			if(VitesseToZero>1.0f) VitesseToZero=1.0f;
			SPG_DbgCHECK(VitesseToZero<0,"Debug test");
			if(NIter>0.9f*MaxIter)
			{
				SPG_DbgCHECK(VitesseToZero!=1.0f,"Debug test");
			}
			
			for(i=0;i<=ifMin;i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re;
				//sur les FIR pairs la partie imaginaire n'est pas nulle
				C.D[i].im=VitesseToZero*C.D[i].im;//0;
			}
			for(i=ifMax;i<=((1+C.NumS)>>1);i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re;
				//sur les FIR pairs la partie imaginaire n'est pas nulle
				C.D[i].im=VitesseToZero*C.D[i].im;//0;
			}
		}
		
		
		CX_MulR(C.D[0],Vitesse);//force la disparition du fond continu
		for(i=((1+C.NumS)>>1)+1;i<C.NumS;i++)
		{
			C.D[i]=Zero;
		}
		
		//CutX_GetRe(C,CRe);
		//CutX_GetIm(C,CIm);
		
		//trace dans l'espace de fourier
		//Cut_Draw2(CRe,CIm,G_Fourier,0xff0000,0x000ff00,CL);
		
		CutX_FFT(C);
		CutX_Conjugue(C);
		
		//DoEvents(SPG_DOEV_ALL);
		/*
		while(Global.MouseLeft==0) DoEvents(SPG_DOEV_LOCK);
		while(Global.MouseLeft) DoEvents(SPG_DOEV_LOCK);
		*/
		SPG_StackCheck(Zero);
}
//MaxDetect_FIRLen+=2;
CutX_Create(MaxDetect_FIR,MaxDetect_FIRLen,Z_Step,FIR_UnitLabel,"FIR");
SPG_SetMemName(MaxDetect_FIR.D,"FD_InitFIR:MaxDetectFir");

//G L=3 XL=2 L=4 XL=6 L=5 XL=3
//D L=3 XR=6 L=4 XL=5 L=5 XL=4

//0 1 2 3 4 5 6 7
//1 2           0 L=3 (L+1)/2 7-L/2 012
//2 3         0 1 L=4 (L+1)/2 7-L/2 0123
//2 3 4       0 1 L=5 (L+1)/2 7-L/2 01234
//3 4 5     0 1 2 L=6 (L+1)/2 7-L/2 012345

for(i=0;i<((1+MaxDetect_FIRLen)>>1);i++)
{
	MaxDetect_FIR.D[i+(MaxDetect_FIRLen>>1)]=C.D[i];
}
for(i=0;i<(MaxDetect_FIRLen>>1);i++)
{
	MaxDetect_FIR.D[i]=C.D[C.NumS-(MaxDetect_FIRLen>>1)+i];
}


/*
CutX_GetRe(MaxDetect_FIR,CRe);
CutX_GetIm(MaxDetect_FIR,CIm);
Cut_Save(CRe,"MaxDetect_FIR_RE");
Cut_Save(CIm,"MaxDetect_FIR_IM");
CutX_GetRe(C,CRe);
CutX_GetIm(C,CIm);
Cut_Save(CRe,"C_RE");
Cut_Save(CIm,"C_IM");
*/

//FD_DrawFIR(G_Direct,MaxDetect_FIR,CL);
//FD_DrawSpectrum(G_Fourier,MaxDetect_FIR,CL);
//PB_SaveFIR(MaxDetect_FIR);
//PB_SaveSpectrum(MaxDetect_FIR);

{
	float Rmoy=0;
	float Imoy=0;
	for(i=0;i<MaxDetect_FIR.NumS;i++)
	{
		Rmoy+=MaxDetect_FIR.D[i].re;
		Imoy+=MaxDetect_FIR.D[i].im;
	}
	Rmoy/=MaxDetect_FIR.NumS;
	Imoy/=MaxDetect_FIR.NumS;
	for(i=0;i<MaxDetect_FIR.NumS;i++)
	{
		MaxDetect_FIR.D[i].re-=Rmoy;
		MaxDetect_FIR.D[i].im-=Imoy;
	}
}


//SPG_BlitAndWaitForClick();
//DoEvents(SPG_DOEV_ALL);

//G_CloseEcran(G_Direct);
//G_CloseEcran(G_Fourier);

CutX_Close(C);
//Cut_Close(CRe);
//Cut_Close(CIm);

return -1;
}
#endif

#ifdef UseFIRCos
//calcul du filtre convolutif
//sort une Cut complexe dont les parties reelle et imaginaire sont en quadrature
//MaxDetect_FIRLen est la longueur voulue du filtre (preferablement impaire)
//lambda min/max definit le gabarit (la frequence normalisee est z_step/lambda)
int SPG_CONV FD_InitFIR(CutX& MaxDetect_FIR, int MaxDetect_FIRLen, int MaxIter, float LambdaMin, float LambdaMax, float Z_Step)
{
	SPG_CHECK(MaxDetect_FIRLen<1,"FD_InitFIR: Parametre invalide",return 0);
	//pas assez echantillonné: moins de deux echantillons par frange
	SPG_CHECK(LambdaMin<2*Z_Step,"FD_InitFIR: Diminuer le pas d'echantillonnage",return 0);
	//la longueur du filtre est trop basse (moins de deux franges)
	SPG_CHECK(LambdaMax>MaxDetect_FIRLen*Z_Step,"FD_InitFIR: Allongez le filtre",return 0);
	SPG_CHECK(LambdaMin==LambdaMax,"FD_InitFIR: Longueur de cohérence infinie",return 0);
	
	CutX C;
	//Cut CRe;
	//Cut CIm;
	
	//calcule une taille de travail adaptée à la FFT et à la précision en fréquence voulue
	int NumS=SFFT_GetAppropriateSize(V_FloatToInt(4*(MaxDetect_FIRLen+(LambdaMax+LambdaMin)/(LambdaMax-LambdaMin))),FFT_MARGE_100);

	if(CutX_Create(C,NumS,Z_Step,FIR_UnitLabel,"Intensite")==0) return 0;
	SPG_SetMemName(C.D,"FD_InitFir CutX");
	//Cut_Create(CRe,NumS,Z_Step,FIR_UnitLabel,"Intensite");
	//Cut_Create(CIm,NumS,Z_Step,FIR_UnitLabel,"Intensite");
	
	//position du gabarit: frequence normalisee Z_Step/LambdaMax,
	//longueur de l'axe des frequences C.NumS
	float fMin=C.NumS*Z_Step/LambdaMax;
	float fMax=C.NumS*Z_Step/LambdaMin;
	int ifMin=(int)ceil(fMin);
	int ifMax=(int)floor(fMax);
	float E=0.1f;
	while(ifMin>=ifMax-1)
	{
		E+=0.1f;
		ifMin=(int)ceil(fMin-E);
		ifMax=(int)floor(fMax+E);
	}
	
	//genere le filtre dans l'espace de fourier
	
	float FourNrm=0;
	for(int i=ifMin;i<=ifMax;i++)
	{
		float CibleF = 0.5f - 0.5f * cos((float) V_2PI * (i + 1.0f - ifMin) / (ifMax + 2.0f - ifMin));
		C.D[i].re=CibleF;
		C.D[i].im=0;
		FourNrm+=C.D[i].re*C.D[i].re;//+C.D[i].im*C.D[i].im;
	}
	
	//passe en espace reel
	CutX_FFT(C);
	CutX_Conjugue(C);
	for(int NIter=0;NIter<MaxIter;NIter++)
	{
		//force des ajustements
		//Vitesse=1 au debut et 0 à la fin
		float Vitesse01=(float)NIter/(MaxIter-1);
		float Vitesse10=1-Vitesse01;
		
		//CutX_GetRe(C,CRe);
		//CutX_GetIm(C,CIm);
		
		//trace dans l'espace reel
		//Cut_Draw2(CRe,CIm,G_Direct,0xff0000,0x000ff00,CL);
		
		//force le gabarit en espace reel
		{
			for(i=0;i<C.NumS;i++)
			{
				float CibleR=1;
				if(i<(MaxDetect_FIRLen>>1))
					CibleR=0.5f-0.5f*cos(((float) V_2PI*i)/MaxDetect_FIRLen);
				if(i>(C.NumS-((1+MaxDetect_FIRLen)>>1)))
					CibleR=0.5f-0.5f*cos(((float) V_2PI*(C.NumS-i))/MaxDetect_FIRLen);

				C.D[i].re=(1-(0.9f*Vitesse01)*CibleR)*C.D[i].re;
				C.D[i].im=(1-(0.9f*Vitesse01)*CibleR)*C.D[i].im;
			}
		}
		/*
		if(NIter==MaxIter-1)
		{
		CutX_GetRe(C,CRe);
		CutX_GetIm(C,CIm);
		Cut_Save(CRe,"LC_RE");
		Cut_Save(CIm,"LC_IM");
		}
		*/
		
		//G L=3 XL=2 L=4 XL=6 L=5 XL=3
		//D L=3 XR=6 L=4 XL=5 L=5 XL=4
		
		//0 1 2 3 4 5 6 7
		//1 2           0 L=3 (L+1)/2 7-L/2
		//2 3         0 1 L=4 (L+1)/2 7-L/2
		//2 3 4       0 1 L=5 (L+1)/2 7-L/2
		//3 4 5     0 1 2 L=6 (L+1)/2 7-L/2
		
		CutX_FFT(C);
		CutX_Conjugue(C);
		
		float FourNrmDyn=0;
		for(int i=0;i<=((1+C.NumS)>>1);i++)
		{
			FourNrmDyn+=C.D[i].re*C.D[i].re+C.D[i].im*C.D[i].im;
		}
		FourNrmDyn=FourNrm/FourNrmDyn;
		
		SPG_StackAllocZ(SPG_COMPLEX,Zero);
		
		{
			for(i=0;i<=((1+C.NumS)>>1);i++)
			{
				float CibleF=0;
				if(V_InclusiveBound(i,ifMin,ifMax))
					CibleF=0.5f-0.5f*cos((float) V_2PI*(i+1.0f-ifMin)/(ifMax+2.0f-ifMin));
				C.D[i].re=Vitesse01*C.D[i].re*FourNrmDyn+Vitesse10*CibleF;
				//sur les FIR pairs la partie imaginaire n'est pas nulle
				C.D[i].im=Vitesse10*C.D[i].im*FourNrmDyn;
			}
		}
		CX_MulR(C.D[0],Vitesse10);//force la disparition du fond continu
		for(i=((1+C.NumS)>>1)+1;i<C.NumS;i++)
		{
			C.D[i]=Zero;
		}
		
		//CutX_GetRe(C,CRe);
		//CutX_GetIm(C,CIm);
		
		//trace dans l'espace de fourier
		//Cut_Draw2(CRe,CIm,G_Fourier,0xff0000,0x000ff00,CL);
		
		CutX_FFT(C);
		CutX_Conjugue(C);
		
		//DoEvents(SPG_DOEV_ALL);
		/*
		while(Global.MouseLeft==0) DoEvents(SPG_DOEV_LOCK);
		while(Global.MouseLeft) DoEvents(SPG_DOEV_LOCK);
		*/
		SPG_StackCheck(Zero);
}
//MaxDetect_FIRLen+=2;
CutX_Create(MaxDetect_FIR,MaxDetect_FIRLen,Z_Step,FIR_UnitLabel,"FIR");
SPG_SetMemName(MaxDetect_FIR.D,"FD_InitFIR:MaxDetectFir");

//G L=3 XL=2 L=4 XL=6 L=5 XL=3
//D L=3 XR=6 L=4 XL=5 L=5 XL=4

//0 1 2 3 4 5 6 7
//1 2           0 L=3 (L+1)/2 7-L/2 012
//2 3         0 1 L=4 (L+1)/2 7-L/2 0123
//2 3 4       0 1 L=5 (L+1)/2 7-L/2 01234
//3 4 5     0 1 2 L=6 (L+1)/2 7-L/2 012345

for(i=0;i<((1+MaxDetect_FIRLen)>>1);i++)
{
	MaxDetect_FIR.D[i+(MaxDetect_FIRLen>>1)]=C.D[i];
}
for(i=0;i<(MaxDetect_FIRLen>>1);i++)
{
	MaxDetect_FIR.D[i]=C.D[C.NumS-(MaxDetect_FIRLen>>1)+i];
}


/*
CutX_GetRe(MaxDetect_FIR,CRe);
CutX_GetIm(MaxDetect_FIR,CIm);
Cut_Save(CRe,"MaxDetect_FIR_RE");
Cut_Save(CIm,"MaxDetect_FIR_IM");
CutX_GetRe(C,CRe);
CutX_GetIm(C,CIm);
Cut_Save(CRe,"C_RE");
Cut_Save(CIm,"C_IM");
*/

//FD_DrawFIR(G_Direct,MaxDetect_FIR,CL);
//FD_DrawSpectrum(G_Fourier,MaxDetect_FIR,CL);
//PB_SaveFIR(MaxDetect_FIR);
//PB_SaveSpectrum(MaxDetect_FIR);

{
	float Rmoy=0;
	float Imoy=0;
	for(i=0;i<MaxDetect_FIR.NumS;i++)
	{
		Rmoy+=MaxDetect_FIR.D[i].re;
		Imoy+=MaxDetect_FIR.D[i].im;
	}
	Rmoy/=MaxDetect_FIR.NumS;
	Imoy/=MaxDetect_FIR.NumS;
	for(i=0;i<MaxDetect_FIR.NumS;i++)
	{
		MaxDetect_FIR.D[i].re-=Rmoy;
		MaxDetect_FIR.D[i].im-=Imoy;
	}
}


//SPG_BlitAndWaitForClick();
//DoEvents(SPG_DOEV_ALL);

//G_CloseEcran(G_Direct);
//G_CloseEcran(G_Fourier);

CutX_Close(C);
//Cut_Close(CRe);
//Cut_Close(CIm);

return -1;
}

#endif

#ifndef IsM3D
//calcul du filtre convolutif
//sort une Cut complexe dont les parties reelle et imaginaire sont en quadrature
//MaxDetect_FIRLen est la longueur voulue du filtre (preferablement impaire)
//lambda min/max definit le gabarit (la frequence normalisee est z_step/lambda)
int SPG_CONV FD_InitFIR(CutX& MaxDetect_FIR, int MaxDetect_FIRLen, int MaxIter, float LambdaMin, float LambdaMax, float Z_Step, G_Ecran& E, C_Lib& CL)
{
	SPG_CHECK(MaxDetect_FIRLen<1,"PB_InitFIR: Parametre invalide",return 0);
	//pas assez echantillonné: moins de deux echantillons par frange
	CHECKV(LambdaMin<2*Z_Step,"PB_InitFIR: Parametre invalide: Diminuer le pas en Z\nZ_StepMax=",LambdaMin/2,return 0);
	//la longueur du filtre est trop basse (moins de deux franges)
	CHECKV(2*LambdaMax>MaxDetect_FIRLen*Z_Step,"PB_InitFIR: Parametre invalide: Augmenter le pas en Z/Allonger le filtre\nZ_StepMin=",2*LambdaMax/MaxDetect_FIRLen,return 0);
	
	G_Ecran G_Direct;
	G_Ecran G_Fourier;
	G_InitSousEcran(G_Direct,E,0,0,E.SizeX,E.SizeY>>1);
	G_InitSousEcran(G_Fourier,E,0,E.SizeY>>1,E.SizeX,E.SizeY>>1);
	
	CutX C;
	Cut CRe;
	Cut CIm;
	
	//calcule une taille de travail adaptée à la FFT et à la précision en fréquence voulue
	int NumS=SFFT_GetAppropriateSize(4*(MaxDetect_FIRLen+(LambdaMax+LambdaMin)/(LambdaMax-LambdaMin)),FFT_MARGE_100);
	
	CutX_Create(C,NumS,Z_Step,FIR_UnitLabel,"Intensite");
	Cut_Create(CRe,NumS,Z_Step,FIR_UnitLabel,"Intensite");
	Cut_Create(CIm,NumS,Z_Step,FIR_UnitLabel,"Intensite");
	
	//position du gabarit: frequence normalisee Z_Step/LambdaMax,
	//longueur de l'axe des frequences C.NumS
	float fMin=C.NumS*Z_Step/LambdaMax;
	float fMax=C.NumS*Z_Step/LambdaMin;
	
	//genere le filtre dans l'espace de fourier
	
	{for(int i=fMin;i<=fMax;i++)
	{
		C.D[i].re=1;
	}}
	
	//passe en espace reel
	
	CutX_FFT(C);
	CutX_Conjugue(C);
	
	for(int NIter=0;NIter<MaxIter;NIter++)
	{
		
		//force des ajustements
		float Vitesse=1.0-(float)NIter/(MaxIter-1);
		
		CutX_GetRe(C,CRe);
		CutX_GetIm(C,CIm);
		
		//trace dans l'espace reel
		Cut_Draw2(CRe,CIm,G_Direct,0xff0000,0x000ff00,CL);
		
		
		//force le gabarit en espace reel
		
		{
			float VitesseToZero=V_Max((2*Vitesse-1),0);
			if(NIter>0.9*MaxIter)
			{
				DbgCHECK(VitesseToZero!=0.0,"Debug test");
			}
			
			{for(int i=((1+MaxDetect_FIRLen)>>1);i<C.NumS-(MaxDetect_FIRLen>>1);i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re;
				C.D[i].im=VitesseToZero*C.D[i].im;
			}}
		}
		/*
		if(NIter==MaxIter-1)
		{
		CutX_GetRe(C,CRe);
		CutX_GetIm(C,CIm);
		Cut_Save(CRe,"LC_RE");
		Cut_Save(CIm,"LC_IM");
		}
		*/
		
		//G L=3 XL=2 L=4 XL=6 L=5 XL=3
		//D L=3 XR=6 L=4 XL=5 L=5 XL=4
		
		//0 1 2 3 4 5 6 7
		//1 2           0 L=3 (L+1)/2 7-L/2
		//2 3         0 1 L=4 (L+1)/2 7-L/2
		//2 3 4       0 1 L=5 (L+1)/2 7-L/2
		//3 4 5     0 1 2 L=6 (L+1)/2 7-L/2
		
		CutX_FFT(C);
		CutX_Conjugue(C);
		
		//force le gabarit dans l'espace de fourier
		float ReponseMoyenne=0;//reponse dans la bande passante
		{for(int i=fMin;i<=fMax;i++)
		{
			ReponseMoyenne+=C.D[i].re;
		}}
		
		//facteur de normalisation assurant la stabilite de l'algorithme
		float Rescale=(1.0+fMax-fMin)/ReponseMoyenne;
		
		CutX_MulR(C,Rescale);
		
		SPG_StackAllocZ(SPG_COMPLEX,Zero);
		
		{
			float VitesseToZero=1.1-1.2*Vitesse;
			if(VitesseToZero<0.0) VitesseToZero=0.0;
			if(VitesseToZero>1.0) VitesseToZero=1.0;
			if(NIter>0.92*MaxIter)
			{
				DbgCHECK(VitesseToZero!=1.0,"Debug test");
			}
			{for(int i=fMin;i<=fMax;i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re+(1.0-VitesseToZero);
				//sur les FIR pairs la partie imaginaire n'est pas nulle
				C.D[i].im=VitesseToZero*C.D[i].im;//0;
			}}
		}
		//ces contraintes sont rapidement relachees, elles affectent le rapport s/b
		{
			float VitesseToZero=1.0-6.0*(Vitesse-0.2)*(1.0-Vitesse);
			if(VitesseToZero>1.0) VitesseToZero=1.0;
			DbgCHECK(VitesseToZero<0,"Debug test");
			if(NIter>0.9*MaxIter)
			{
				DbgCHECK(VitesseToZero!=1.0,"Debug test");
			}
			
			{for(int i=0;i<fMin;i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re;
				//sur les FIR pairs la partie imaginaire n'est pas nulle
				C.D[i].im=VitesseToZero*C.D[i].im;//0;
			}}
			{for(int i=fMax+1;i<=((1+C.NumS)>>1);i++)
			{
				C.D[i].re=VitesseToZero*C.D[i].re;
				//sur les FIR pairs la partie imaginaire n'est pas nulle
				C.D[i].im=VitesseToZero*C.D[i].im;//0;
			}}
		}
		
		
		CX_MulR(C.D[0],Vitesse);//force la disparition du fond continu
		{for(int i=((1+C.NumS)>>1)+1;i<C.NumS;i++)
		{
			C.D[i]=Zero;
		}}
		
		CutX_GetRe(C,CRe);
		CutX_GetIm(C,CIm);
		
		//trace dans l'espace de fourier
		Cut_Draw2(CRe,CIm,G_Fourier,0xff0000,0x000ff00,CL);
		
		CutX_FFT(C);
		CutX_Conjugue(C);
		
		DoEvents(SPG_DOEV_ALL);
		/*
		while(Global.MouseLeft==0) DoEvents(SPG_DOEV_LOCK);
		while(Global.MouseLeft) DoEvents(SPG_DOEV_LOCK);
		*/
		SPG_StackCheck(Zero);
}
//MaxDetect_FIRLen+=2;
CutX_Create(MaxDetect_FIR,MaxDetect_FIRLen,Z_Step,FIR_UnitLabel,"FIR");

//G L=3 XL=2 L=4 XL=6 L=5 XL=3
//D L=3 XR=6 L=4 XL=5 L=5 XL=4

//0 1 2 3 4 5 6 7
//1 2           0 L=3 (L+1)/2 7-L/2 012
//2 3         0 1 L=4 (L+1)/2 7-L/2 0123
//2 3 4       0 1 L=5 (L+1)/2 7-L/2 01234
//3 4 5     0 1 2 L=6 (L+1)/2 7-L/2 012345

{for(int i=0;i<((1+MaxDetect_FIRLen)>>1);i++)
{
	MaxDetect_FIR.D[i+(MaxDetect_FIRLen>>1)]=C.D[i];
}}
{for(int i=0;i<(MaxDetect_FIRLen>>1);i++)
{
	MaxDetect_FIR.D[i]=C.D[C.NumS-(MaxDetect_FIRLen>>1)+i];
}}


/*
CutX_GetRe(MaxDetect_FIR,CRe);
CutX_GetIm(MaxDetect_FIR,CIm);
Cut_Save(CRe,"MaxDetect_FIR_RE");
Cut_Save(CIm,"MaxDetect_FIR_IM");
CutX_GetRe(C,CRe);
CutX_GetIm(C,CIm);
Cut_Save(CRe,"C_RE");
Cut_Save(CIm,"C_IM");
*/

FD_DrawFIR(G_Direct,MaxDetect_FIR,CL);
FD_DrawSpectrum(G_Fourier,MaxDetect_FIR,CL);
//PB_SaveFIR(MaxDetect_FIR);
//PB_SaveSpectrum(MaxDetect_FIR);

{
	float Rmoy=0;
	float Imoy=0;
	{for(int i=0;i<MaxDetect_FIR.NumS;i++)
	{
		Rmoy+=MaxDetect_FIR.D[i].re;
		Imoy+=MaxDetect_FIR.D[i].im;
	}}
	Rmoy/=MaxDetect_FIR.NumS;
	Imoy/=MaxDetect_FIR.NumS;
	{for(int i=0;i<MaxDetect_FIR.NumS;i++)
	{
		MaxDetect_FIR.D[i].re-=Rmoy;
		MaxDetect_FIR.D[i].im-=Imoy;
	}}
}


//SPG_BlitAndWaitForClick();
DoEvents(SPG_DOEV_ALL);

G_CloseEcran(G_Direct);
G_CloseEcran(G_Fourier);

CutX_Close(C);
Cut_Close(CRe);
Cut_Close(CIm);

return -1;
}

//dessine une coupe complexe
void SPG_CONV FD_DrawFIR(G_Ecran& E, CutX& C, C_Lib& CL)
{
	Cut CRe;
	Cut CIm;
	
	Cut_Create(CRe,C.NumS,C.XScale ,C.UnitX ,C.UnitY);
	Cut_Create(CIm,C.NumS,C.XScale ,C.UnitX ,C.UnitY);
	
	CutX_GetRe(C,CRe);
	CutX_GetIm(C,CIm);
	
	Cut_Draw2(CRe,CIm,E,0xff0000,0x000ff00,CL);
	
	Cut_Close(CRe);
	Cut_Close(CIm);
	
	return;
}

//dessine la fft des filtres (le gabarit en frequence)
void SPG_CONV FD_DrawSpectrum(G_Ecran& E, CutX& C, C_Lib& CL)
{
	Cut CRe;
	Cut CIm;
	CutX CFFT;
	
	int NumS=SFFT_GetAppropriateSize(4*C.NumS,FFT_MARGE_100);
	
	CutX_Create(CFFT,NumS,V_2PI/C.XScale ,"2Pi/"FIR_UnitLabel,C.UnitY);
	Cut_Create(CRe,NumS,V_2PI/C.XScale ,"2Pi/"FIR_UnitLabel,C.UnitY);
	Cut_Create(CIm,NumS,V_2PI/C.XScale ,"2Pi/"FIR_UnitLabel,C.UnitY);
	
	//traite le filtre reel
	
	CutX_GetRe(C,CRe);
	Cut_PlaceForFFT(CRe,CFFT,1);
	
	CutX_FFT(CFFT);
	CutX_Conjugue(CFFT);
	CutX_TranslateToCenter_FFT_AlternateSign(CFFT);
	
	CutX_GetModule(CFFT,CRe);
	
	
	//traite le filtre imaginaire
	CutX_GetIm(C,CIm);
	Cut_PlaceForFFT(CIm,CFFT,1);
	
	CutX_FFT(CFFT);
	CutX_Conjugue(CFFT);
	CutX_TranslateToCenter_FFT_AlternateSign(CFFT);
	
	CutX_GetModule(CFFT,CIm);
	
	Cut_Draw2(CRe,CIm,E,0xff0000,0x000ff00,CL);
	
	CutX_Close(CFFT);
	Cut_Close(CRe);
	Cut_Close(CIm);
	
	return;
}

//sauve les filtres convolutifs
void SPG_CONV FD_SaveFIR(CutX& C)
{
	Cut CRe;
	Cut CIm;
	
	Cut_Create(CRe,C.NumS,C.XScale ,C.UnitX ,C.UnitY);
	Cut_Create(CIm,C.NumS,C.XScale ,C.UnitX ,C.UnitY);
	
	CutX_GetRe(C,CRe);
	CutX_GetIm(C,CIm);
	
	Cut_Save(CRe,"FIR_Coeffs_Re");
	Cut_Save(CIm,"FIR_Coeffs_Im");
	
	Cut_Close(CRe);
	Cut_Close(CIm);
	
	return;
}

//sauve la fft des filtres (le gabarit en frequence)
void SPG_CONV FD_SaveSpectrum(CutX& C)
{
	Cut CRe;
	Cut CIm;
	CutX CFFT;
	
	int NumS=SFFT_GetAppropriateSize(4*C.NumS,FFT_MARGE_100);
	
	CutX_Create(CFFT,NumS,V_2PI/C.XScale ,"2Pi/"FIR_UnitLabel,C.UnitY);
	Cut_Create(CRe,NumS,V_2PI/C.XScale ,"2Pi/"FIR_UnitLabel,C.UnitY);
	Cut_Create(CIm,NumS,V_2PI/C.XScale ,"2Pi/"FIR_UnitLabel,C.UnitY);
	
	//traite le filtre reel
	
	CutX_GetRe(C,CRe);
	Cut_PlaceForFFT(CRe,CFFT,1);
	
	CutX_FFT(CFFT);
	CutX_Conjugue(CFFT);
	CutX_TranslateToCenter_FFT_AlternateSign(CFFT);
	
	CutX_GetModule(CFFT,CRe);
	
	
	//traite le filtre imaginaire
	CutX_GetIm(C,CIm);
	Cut_PlaceForFFT(CIm,CFFT,1);
	
	CutX_FFT(CFFT);
	CutX_Conjugue(CFFT);
	CutX_TranslateToCenter_FFT_AlternateSign(CFFT);
	
	CutX_GetModule(CFFT,CIm);
	
	Cut_Save(CRe,"FIR_Fourier_Re");
	Cut_Save(CIm,"FIR_Fourier_Im");
	
	CutX_Close(CFFT);
	Cut_Close(CRe);
	Cut_Close(CIm);
	
	return;
}

#endif
#endif

