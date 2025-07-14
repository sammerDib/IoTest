
#include "SPG_General.h"

#ifdef SPG_General_USEDISPFIELD

#include "SPG_Includes.h"
#include "SPG_DispFieldConfig.h"

#include <memory.h>
#include <float.h>

#if(DF_VERSION==3)
#ifdef DF_PREFILTER
int SPG_CONV SPG_DF_Init(SPG_DISPFIELD& DF, int SizeX, int SizeY, float* InterpoleDeltaTmp, float NoiseLevel, float CorrelationThreshold, int PreFilterSize)
#else
int SPG_CONV SPG_DF_Init(SPG_DISPFIELD& DF, int SizeX, int SizeY, float* InterpoleDeltaTmp, float NoiseLevel, float CorrelationThreshold, int ReferenceIntegration)
#endif
#elif(DF_VERSION>=4)
int SPG_CONV SPG_DF_Init(SPG_DISPFIELD& DF, SPG_PIXINT& PX, int SizeX, int SizeY, float NoiseLevel, float CorrelationThreshold, int ReferenceIntegration, int SuperResolution)
#endif
{
	memset(&DF,0,sizeof(SPG_DISPFIELD));
#if(DF_VERSION>=4)
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"SPG_DF_Init",return 0);
#endif
	DF.SizeX=SizeX;DF.SizeY=SizeY;
	DF.Reference=SPG_TypeAlloc(2*DF.SizeX*DF.SizeY,float,"SPG_DF_Init:Reference");
	DF.Current=DF.Reference+DF.SizeX*DF.SizeY;//SPG_TypeAlloc(DF.SizeX*DF.SizeY,float,"SPG_DF_Init:Current");
	DF.GRInfo.GradRef=SPG_TypeAlloc(DF.SizeX*DF.SizeY,SPG_PIXGRADINFO,"SPG_DF_Init:GradRef");
#if(DF_VERSION==3)
	PIXINT_RELAX_Init(DF.PX,DF.SizeX,DF.SizeY,InterpoleDeltaTmp);
#elif(DF_VERSION>=4)
	DF.PX=&PX;
	int KernelSize=(2*PX.KernelSize+1)*(2*PX.KernelSize+1);
	DF.DX_Kernel=SPG_TypeAlloc(5*KernelSize,float,"SPG_DF_Init");
	DF.DD_Kernel=DF.DX_Kernel+KernelSize;
	DF.DY_Kernel=DF.DX_Kernel+2*KernelSize;
	DF.DE_Kernel=DF.DX_Kernel+3*KernelSize;
	DF.Interpole_Kernel=DF.DX_Kernel+4*KernelSize;

	PIXINT_GetDXKernel(PX,DF.DX_Kernel);
	//PIXINT_GetDDKernel(PX,DF.DD_Kernel);
	PIXINT_GetDYKernel(PX,DF.DY_Kernel);
	//PIXINT_GetDEKernel(PX,DF.DE_Kernel);
	const float INVSQRT2=0.7071f;
	for(int i=0;i<KernelSize;i++)
	{
		DF.DD_Kernel[i]=(DF.DX_Kernel[i]+DF.DY_Kernel[i])*INVSQRT2;
		DF.DE_Kernel[i]=(DF.DY_Kernel[i]-DF.DX_Kernel[i])*INVSQRT2;
		PIXINT_Round(DF.DX_Kernel[i]);
		PIXINT_Round(DF.DD_Kernel[i]);
		PIXINT_Round(DF.DY_Kernel[i]);
		PIXINT_Round(DF.DE_Kernel[i]);
		DF.DX_Kernel[i]/=SPG_DF_SENSNEG(PX.Oversampling+0.5f);
		DF.DD_Kernel[i]/=SPG_DF_SENSNEG(PX.Oversampling+0.5f);
		DF.DY_Kernel[i]/=SPG_DF_SENSNEG(PX.Oversampling+0.5f);
		DF.DE_Kernel[i]/=SPG_DF_SENSNEG(PX.Oversampling+0.5f);
	}
#ifdef DF_SAVECALIBR
	Text_Write(DF.DX_Kernel,(2*PX.KernelSize+1),(2*PX.KernelSize+1),"DX_Kernel.txt",4);
	Text_Write(DF.DD_Kernel,(2*PX.KernelSize+1),(2*PX.KernelSize+1),"DD_Kernel.txt",4);
	Text_Write(DF.DY_Kernel,(2*PX.KernelSize+1),(2*PX.KernelSize+1),"DY_Kernel.txt",4);
	Text_Write(DF.DE_Kernel,(2*PX.KernelSize+1),(2*PX.KernelSize+1),"DE_Kernel.txt",4);
#endif
#endif
#if(DF_VERSION==3)
	DF.InterpolationBorderSize=1;
#elif(DF_VERSION>=4)
	DF.InterpolationBorderSize=2;
#endif
	DF.ReferenceIntegration=ReferenceIntegration;
#ifdef DF_PREFILTER
	if(DF.PreFilterSize=PreFilterSize)
	{
		DF.PreFilterIn=SPG_TypeAlloc(DF.SizeX*DF.SizeY,float,"SPG_DF_Init:PreFilterIn");
		DF.PreFilterOut=SPG_TypeAlloc(DF.SizeX*DF.SizeY,float,"SPG_DF_Init:PreFilterOut");
	}
#endif
	DF.NoiseLevel=NoiseLevel;
	DF.CorrelationThreshold=CorrelationThreshold;
	DF.UxP=1;DF.UyP=0;
	DF.UxN=1;DF.UyN=0;
	DF.VxP=0;DF.VyP=1;
	DF.VxN=0;DF.VyN=1;

	DF.DS.SuperResolution=SuperResolution;
	if(DF.DS.SuperResolution)
	{
		WB_Init(DF.DS.WB,DF.SizeX,DF.SizeY,DF.DS.SuperResolution);
		DF.DS.PosX=0;
		DF.DS.PosY=0;
		DF.DS.SpeedIntegrationThreshold=0.5f/DF.DS.SuperResolution;
	}

	return DF.Etat=-1;
}

void SPG_CONV SPG_DF_Close(SPG_DISPFIELD& DF)
{
	CHECK(DF.Etat==0,"SPG_DF_Close",return);
	SPG_MemFree(DF.Reference);
	SPG_MemFree(DF.GRInfo.GradRef);
#if(DF_VERSION==3)
	PIXINT_RELAX_Close(DF.PX);
#elif(DF_VERSION>=4)
	//DF.PX=0 inclus dans le memset final
	SPG_MemFree(DF.DX_Kernel);//desalloue le bloc dx,dd,dy,de,Interpole
#endif
#ifdef DF_PREFILTER
	if(DF.PreFilterSize)
	{
		SPG_MemFree(DF.PreFilterIn);
		SPG_MemFree(DF.PreFilterOut);
	}
#endif
	if(DF.DS.SuperResolution) WB_Close(DF.DS.WB);

	memset(&DF,0,sizeof(SPG_DISPFIELD));
}

/*
void SPG_CONV SPG_DF_ComputeWeight(float& DW, float& W, float& D, float SigmaD, float SigmaT)
{
	//precision(T/D) = SigmaT/D+T*SigmaD/D²
	//1/prec = D/(SigmaT*(1+SigmaD/D))
	//il faut donc D>(SigmaT+SigmaT*SigmaD/D) sinon le bruit depasse
	//lorsque le gradient est faible devant le bruit la sensibilite diminue
	float fD=fabs(D);//-SigmaD;
	if(fD<=0) {W=0;DW=0;} 
	else	{
				float N=SigmaT*(1.0f+SigmaD/fD);
				W=fD/N;
				DW=V_Signe(D)*W/sqrt(D*D+N*N);
			}
	return;
}
*/

#if(DF_VERSION==5)
void SPG_CONV SPG_DF_Reduction(float& PosX, float& PosY, float X, float WX, float D, float WD, float Y, float WY, float E, float WE)
{
	const float SQRT2=1.414f;
	PosX=0;
	PosY=0;
	float Denom=2*WX*WY+2*WD*WE+(WX+WY)*(WD+WE);
	if(Denom>0)
	{
		float InvDenom=1.0f/Denom;
		PosX=(X*(2*WY+WD+WE)+D*(WE+WY)*SQRT2+Y*(WE-WD)-E*(WD+WY)*SQRT2)*InvDenom;
		PosY=(X*(WE-WD)+D*(WE+WX)*SQRT2+Y*(2*WX+WD+WE)+E*(WD+WX)*SQRT2)*InvDenom;
	}
	return;
}
#endif

#define SPG_DF_ComputeWeight(DW,W,D,SigmaD,SigmaT) {float fD=fabs(D);if(fD<=0) {W=0;DW=0;} else	{float N=SigmaT*(1.0f+SigmaD/fD);W=fD/N;DW=V_Signe(D)*W/sqrt(D*D+N*N);}}

//sigmaT=1
//sigmaD=3
//sigma=sigmaT+D/SigmaD

#define SPG_DF_ComputeWeight5(DW,W,D,SigmaD,SigmaT) {float fD=fabs(D);if(fD<=0) {W=0;DW=0;} else {float Erreur=SigmaT+SigmaD/fD;W=1.0f/(Erreur*Erreur);DW=V_Signe(D)*W/sqrt(D*D+Erreur*Erreur);}}

#if(DF_VERSION==3)
void SPG_CONV SPG_DF_ComputeIntensity(SPG_DISPFIELD& DF)
{//VERSION 3
	PIXINT_RELAX_Interpole(DF.PX,DF.Reference,(DF.ReferenceIntegrationCount==1)?2:16);
	return;
}//VERSION 3

void SPG_CONV SPG_DF_ComputeGradInfo(SPG_DISPFIELD& DF, SPG_GRADINFO& GRInfo, float* restrict Reference)
{//VERSION 3
	CHECK(DF.Etat==0,"SPG_DF_ComputeGradInfo",return);
	CHECK(GRInfo.GradRef==0,"SPG_DF_ComputeGradInfo",return);
	CHECK(Reference==0,"SPG_DF_ComputeGradInfo",return);
	const float INVSQRT2=0.707f;
	GRInfo.SWX=0;GRInfo.SWD=0;GRInfo.SWY=0;GRInfo.SWE=0;
	GRInfo.SWXX=0;GRInfo.SWYY=0;GRInfo.SWDE=0;
	GRInfo.SWXD=0;GRInfo.SWXE=0;GRInfo.SWYD=0;GRInfo.SWYE=0;
	GRInfo.FirstOrder=0;GRInfo.SecondOrder=0;

	SPG_PIXGRADINFO* GradRef=GRInfo.GradRef+DF.InterpolationBorderSize*DF.SizeX;
	Reference+=DF.InterpolationBorderSize*DF.SizeX;
	for(int y=DF.InterpolationBorderSize;y<DF.SizeY-DF.InterpolationBorderSize;y++)
	{
		float xSWX=0;float xSWD=0;float xSWY=0;float xSWE=0;//somme intermediaire pour la precision du calcul numerique
		float xSWXX=0;float xSWYY=0;float xSWDE=0;//somme intermediaire pour la precision du calcul numerique
		float xSWXD=0;float xSWXE=0;float xSWYD=0;float xSWYE=0;//somme intermediaire pour la precision du calcul numerique
		float xFirstOrder=0;float xSecondOrder=0;
		int yOffset=(y*DF_GRADSAMPLING)*(DF.PX.SizeX*DF_GRADSAMPLING);

		for(int x=DF.InterpolationBorderSize;x<DF.SizeX-DF.InterpolationBorderSize;x++)
		{//VERSION 3
			SPG_PIXGRADINFO& GR=GradRef[x];
			float B00=DF.PX.PixInterpole[DF_GRADSAMPLING*x+yOffset];
			float B10=DF.PX.PixInterpole[DF_GRADSAMPLING*x+yOffset+1];
			float B20=DF.PX.PixInterpole[DF_GRADSAMPLING*x+yOffset+2];

			float B01=DF.PX.PixInterpole[DF_GRADSAMPLING*(x+DF.PX.SizeX)+yOffset];
			//float B11=DF.PX.PixInterpole[DF_GRADSAMPLING*(x+DF.PX.SizeX)+yOffset+1];
			float B21=DF.PX.PixInterpole[DF_GRADSAMPLING*(x+DF.PX.SizeX)+yOffset+2];

			float B02=DF.PX.PixInterpole[DF_GRADSAMPLING*(x+2*DF.PX.SizeX)+yOffset];
			float B12=DF.PX.PixInterpole[DF_GRADSAMPLING*(x+2*DF.PX.SizeX)+yOffset+1];
			float B22=DF.PX.PixInterpole[DF_GRADSAMPLING*(x+2*DF.PX.SizeX)+yOffset+2];

			GR.S=Reference[x];//=(B00+B10+B20+B01+B11+B21+B02+B12+B22)/9
			xFirstOrder+=Reference[x];
			xSecondOrder+=Reference[x]*Reference[x];
			//DTX=4*d/dt
			//DTD=2*d/dt
			// /3  car 3 lignes, *4/3 car espacement B2-B0 = 3/4 fois pas interpixel
			float DX=(B00+B01+B02-B20-B21-B22)*4.0f/9.0f;//SigmaDX=4*NoiseLevel,SigmaTX=2*NoiseLevel
			float DY=(B00+B10+B20-B02-B12-B22)*4.0f/9.0f;//SigmaDY=4*NoiseLevel
			float DD=(DX+DY)*INVSQRT2;//SigmaDD=2*NoiseLevel,SigmaTD=1.414*NoiseLevel
			float DE=(DY-DX)*INVSQRT2;//SigmaDE=2*NoiseLevel
			//DX=0;DY=0;
			//DE=0;DD=0;
			float WX,WD,WY,WE;
			SPG_DF_ComputeWeight(GR.DWX,WX,DX,1.414f*DF.NoiseLevel,1.0f*DF.NoiseLevel);//calcule weight et weight/derivative SigmaT=sqrt(4termes)
			SPG_DF_ComputeWeight(GR.DWD,WD,DD,2.0f*DF.NoiseLevel,1.0f*DF.NoiseLevel);//SigmaT=sqrt(2termes)
			SPG_DF_ComputeWeight(GR.DWY,WY,DY,1.414f*DF.NoiseLevel,1.0f*DF.NoiseLevel);//calcule weight et weight/derivative
			SPG_DF_ComputeWeight(GR.DWE,WE,DE,2.0f*DF.NoiseLevel,1.0f*DF.NoiseLevel);
			
			xSWX+=WX;xSWD+=WD;xSWY+=WY;xSWE+=WE;
			//weights effectifs des projetés
			xSWXX+=WX; xSWYY+=WY;//1 mesure 1 estimation
			if((WD>0)&&(WE>0)) xSWDE+=2*WD*WE/(WD+WE);//2 mesures 2 estimations mais moitie pixels chacune
			if((WX>0)&&(WD>0)) xSWXD+=WX*WD/(WX+WD);//2 mesures, mais 4 estimations pas indépendantes
			if((WX>0)&&(WE>0)) xSWXE+=WX*WE/(WX+WE);//2 mesures, mais 4 estimations pas indépendantes
			if((WY>0)&&(WD>0)) xSWYD+=WY*WD/(WY+WD);//2 mesures, mais 4 estimations pas indépendantes
			if((WY>0)&&(WE>0)) xSWYE+=WY*WE/(WY+WE);//2 mesures, mais 4 estimations pas indépendantes
		}//VERSION 3
		GRInfo.SWX+=xSWX;GRInfo.SWD+=xSWD;GRInfo.SWY+=xSWY;GRInfo.SWE+=xSWE;
		GRInfo.SWXX+=xSWXX;GRInfo.SWYY+=xSWYY;GRInfo.SWDE+=xSWDE;
		GRInfo.SWXD+=xSWXD;GRInfo.SWXE+=xSWXE;GRInfo.SWYD+=xSWYD;GRInfo.SWYE+=xSWYE;
		GRInfo.FirstOrder+=xFirstOrder;GRInfo.SecondOrder+=xSecondOrder;
		GradRef+=DF.SizeX;
		Reference+=DF.SizeX;
	}
	if(GRInfo.SWX==0) GRInfo.SWX=1;
	if(GRInfo.SWD==0) GRInfo.SWD=1;
	if(GRInfo.SWY==0) GRInfo.SWY=1;
	if(GRInfo.SWE==0) GRInfo.SWE=1;
	return;
}
#elif(DF_VERSION>=4)

void SPG_CONV SPG_DF_ComputeGradInfo(SPG_DISPFIELD& DF, SPG_GRADINFO& GRInfo, float* restrict Reference)
{//VERSION 4
	CHECK(DF.Etat==0,"SPG_DF_ComputeGradInfo",return);
	CHECK(GRInfo.GradRef==0,"SPG_DF_ComputeGradInfo",return);
	CHECK(Reference==0,"SPG_DF_ComputeGradInfo",return);
	CHECK(DF.ReferenceIntegrationCount==0,"SPG_DF_ComputeGradInfo",return);
//	const float INVSQRT2=0.707f;
//	const float SQRT2=1.414f;
	GRInfo.SWX=0;GRInfo.SWD=0;GRInfo.SWY=0;GRInfo.SWE=0;
#if(DF_VERSION==4)
	GRInfo.SWXX=0;GRInfo.SWYY=0;GRInfo.SWDE=0;
	GRInfo.SWXD=0;GRInfo.SWXE=0;GRInfo.SWYD=0;GRInfo.SWYE=0;
#endif
	GRInfo.FirstOrder=0;GRInfo.SecondOrder=0;
	int KernelSize=DF.PX->KernelSize;
	int KernelPitch=2*KernelSize+1;
	float* KernelDX=DF.DX_Kernel+KernelSize*(1+KernelPitch); float* KernelDD=DF.DD_Kernel+KernelSize*(1+KernelPitch);
	float* KernelDY=DF.DY_Kernel+KernelSize*(1+KernelPitch); float* KernelDE=DF.DE_Kernel+KernelSize*(1+KernelPitch);

	SPG_PIXGRADINFO* GradRef=GRInfo.GradRef+DF.InterpolationBorderSize*DF.SizeX;
	//Reference+=DF.InterpolationBorderSize*DF.SizeX;

	const float GradNoise=3.0f*DF.NoiseLevel/sqrt(V_Min(DF.ReferenceIntegrationCount,DF.ReferenceIntegration));

#ifdef DF_SAVEGRADINFO
	Profil PR,PDX,PDD,PDY,PDE,PWX,PWD,PWY,PWE;
	P_Create(PR,DF.SizeX,DF.SizeY);
	P_Create(PDX,DF.SizeX,DF.SizeY);
	P_Create(PDD,DF.SizeX,DF.SizeY);
	P_Create(PDY,DF.SizeX,DF.SizeY);
	P_Create(PDE,DF.SizeX,DF.SizeY);
	P_Create(PWX,DF.SizeX,DF.SizeY);
	P_Create(PWD,DF.SizeX,DF.SizeY);
	P_Create(PWY,DF.SizeX,DF.SizeY);
	P_Create(PWE,DF.SizeX,DF.SizeY);
#endif
	for(int y=DF.InterpolationBorderSize;y<DF.SizeY-DF.InterpolationBorderSize;y++)
	{
		float xSWX=0;float xSWD=0;float xSWY=0;float xSWE=0;//somme intermediaire pour la precision du calcul numerique
#if(DF_VERSION==4)
		float xSWXX=0;float xSWYY=0;float xSWDE=0;//somme intermediaire pour la precision du calcul numerique
		float xSWXD=0;float xSWXE=0;float xSWYD=0;float xSWYE=0;//somme intermediaire pour la precision du calcul numerique
#endif
		float xFirstOrder=0;float xSecondOrder=0;
		//int yOffset=(y*DF_GRADSAMPLING)*(DF.PX.SizeX*DF_GRADSAMPLING);

		for(int x=DF.InterpolationBorderSize;x<DF.SizeX-DF.InterpolationBorderSize;x++)
		{//VERSION 4
			SPG_PIXGRADINFO& GR=GradRef[x];
			GR.S=Reference[x+y*DF.SizeX];//=(B00+B10+B20+B01+B11+B21+B02+B12+B22)/9
			xFirstOrder+=GR.S;
			xSecondOrder+=GR.S*GR.S;
			//DTX=4*d/dt
			//DTD=2*d/dt
			// /3  car 3 lignes, *4/3 car espacement B2-B0 = 3/4 fois pas interpixel
			float DX=0;	float DD=0;	float DY=0;	float DE=0;
			for(int ky=-KernelSize;ky<=KernelSize;ky++)
			{
				//float* RefLine=Reference+V_Sature((y+ky),0,(DF.SizeY-1))*DF.SizeX;//safe
				float* RefLine=Reference+(y+ky)*DF.SizeX+x;//unsafe
				float* KernelDXL=KernelDX+ky*KernelPitch; float* KernelDDL=KernelDD+ky*KernelPitch; 
				float* KernelDYL=KernelDY+ky*KernelPitch; float* KernelDEL=KernelDE+ky*KernelPitch;
				for(int kx=-KernelSize;kx<=KernelSize;kx++)
				{
					//float R=RefLine[V_Sature((x+kx),0,(DF.SizeX-1))];//safe
					float R=RefLine[kx];//unsafe
					DX+=R*KernelDXL[kx]; DD+=R*KernelDDL[kx]; 
					DY+=R*KernelDYL[kx]; DE+=R*KernelDEL[kx];
				}
			}
			//DX=0;DY=0;
			//DE=0;DD=0;
			float WX,WD,WY,WE;
#if(DF_VERSION==4)
			SPG_DF_ComputeWeight(GR.DWX,WX,DX,4.0f*DF.NoiseLevel,DF.NoiseLevel);//calcule weight et weight/derivative SigmaT=sqrt(4termes)
			SPG_DF_ComputeWeight(GR.DWD,WD,DD,2.8284f*DF.NoiseLevel,DF.NoiseLevel);//SigmaT=sqrt(2termes)
			SPG_DF_ComputeWeight(GR.DWY,WY,DY,4.0f*DF.NoiseLevel,DF.NoiseLevel);//calcule weight et weight/derivative
			SPG_DF_ComputeWeight(GR.DWE,WE,DE,2.8284f*DF.NoiseLevel,DF.NoiseLevel);
#elif(DF_VERSION==5)
			SPG_DF_ComputeWeight5(GR.DWX,WX,DX,GradNoise,DF.NoiseLevel);//calcule weight et weight/derivative SigmaT=sqrt(4termes)
			SPG_DF_ComputeWeight5(GR.DWD,WD,DD,GradNoise,DF.NoiseLevel);//SigmaT=sqrt(2termes)
			SPG_DF_ComputeWeight5(GR.DWY,WY,DY,GradNoise,DF.NoiseLevel);//calcule weight et weight/derivative
			SPG_DF_ComputeWeight5(GR.DWE,WE,DE,GradNoise,DF.NoiseLevel);
			//WX=1/sigma²
			//WE=0;
			//WD=0;
			//WD=0;
#endif
#ifdef DF_SAVEGRADINFO
			P_Element(PR,x,y)=GR.S;
			P_Element(PDX,x,y)=DX; P_Element(PWX,x,y)=WX;
			P_Element(PDD,x,y)=DD; P_Element(PWD,x,y)=WD;
			P_Element(PDY,x,y)=DY; P_Element(PWY,x,y)=WY;
			P_Element(PDE,x,y)=DE; P_Element(PWE,x,y)=WE;
#endif
			
			xSWX+=WX;xSWD+=WD;xSWY+=WY;xSWE+=WE;
#if(DF_VERSION==4)
			//weights effectifs des projetés
			xSWXX+=WX; xSWYY+=WY;//1 mesure 1 estimation
			if((WD>0)&&(WE>0)) xSWDE+=2*WD*WE/(WD+WE);//2 mesures 2 estimations mais moitie pixels chacune
			if((WX>0)&&(WD>0)) xSWXD+=WX*WD/(WX+WD);//2 mesures, mais 4 estimations pas indépendantes
			if((WX>0)&&(WE>0)) xSWXE+=WX*WE/(WX+WE);//2 mesures, mais 4 estimations pas indépendantes
			if((WY>0)&&(WD>0)) xSWYD+=WY*WD/(WY+WD);//2 mesures, mais 4 estimations pas indépendantes
			if((WY>0)&&(WE>0)) xSWYE+=WY*WE/(WY+WE);//2 mesures, mais 4 estimations pas indépendantes
#endif
		}//VERSION 4
		GRInfo.SWX+=xSWX;GRInfo.SWD+=xSWD;GRInfo.SWY+=xSWY;GRInfo.SWE+=xSWE;
#if(DF_VERSION==4)
		GRInfo.SWXX+=xSWXX;GRInfo.SWYY+=xSWYY;GRInfo.SWDE+=xSWDE;
		GRInfo.SWXD+=xSWXD;GRInfo.SWXE+=xSWXE;GRInfo.SWYD+=xSWYD;GRInfo.SWYE+=xSWYE;
#endif
		GRInfo.FirstOrder+=xFirstOrder;GRInfo.SecondOrder+=xSecondOrder;
		GradRef+=DF.SizeX;
		//Reference+=DF.SizeX;
	}
#ifdef DF_SAVEGRADINFO
	P_SaveToFile(PR,"PR.bmp");P_Close(PR);
	P_SaveToFile(PDX,"PDX.bmp");P_Close(PDX);
	P_SaveToFile(PDD,"PDD.bmp");P_Close(PDD);
	P_SaveToFile(PDY,"PDY.bmp");P_Close(PDY);
	P_SaveToFile(PDE,"PDE.bmp");P_Close(PDE);
	P_SaveToFile(PWX,"PWX.bmp");P_Close(PWX);
	P_SaveToFile(PWD,"PWD.bmp");P_Close(PWD);
	P_SaveToFile(PWY,"PWY.bmp");P_Close(PWY);
	P_SaveToFile(PWE,"PWE.bmp");P_Close(PWE);
#endif
#if(DF_VERSION==4)
	if(GRInfo.SWX==0) GRInfo.SWX=1;	if(GRInfo.SWD==0) GRInfo.SWD=1; 
	if(GRInfo.SWY==0) GRInfo.SWY=1; if(GRInfo.SWE==0) GRInfo.SWE=1;
#endif

	if(DF.DS.SuperResolution) WB_Clear(DF.DS.WB);

	return;
}


void SPG_CONV SPG_DF_ComputePositionImmed(SPG_DISPFIELD& DF, float& PosX, float& PosY, float* restrict Reference, int RefPitch, float* restrict Current, int CurPitch)
{//VERSION 4  IMMED
	CHECK(DF.Etat==0,"SPG_DF_ComputePositionImmed",return);
	CHECK(Reference==0,"SPG_DF_ComputePositionImmed",return);
	CHECK(Current==0,"SPG_DF_ComputePositionImmed",return);

	//ComputePosition
#if(DF_VERSION==4)
	const float INVSQRT2=0.707f;const float SQRT2=1.414f;
#endif
	float VX=0;float VY=0;float VD=0;float VE=0;

	//GradInfo
	SPG_GRADINFOIMMED GRInfo;
	GRInfo.SWX=0;GRInfo.SWD=0;GRInfo.SWY=0;GRInfo.SWE=0;
#if(DF_VERSION==4)
	GRInfo.SWXX=0;GRInfo.SWYY=0;GRInfo.SWDE=0;
	GRInfo.SWXD=0;GRInfo.SWXE=0;GRInfo.SWYD=0;GRInfo.SWYE=0;
#endif
	//GRInfo.FirstOrder=0;GRInfo.SecondOrder=0;
	int KernelSize=DF.PX->KernelSize;
	int KernelPitch=2*KernelSize+1;
	float* KernelDX=DF.DX_Kernel+KernelSize*(1+KernelPitch); float* KernelDD=DF.DD_Kernel+KernelSize*(1+KernelPitch);
	float* KernelDY=DF.DY_Kernel+KernelSize*(1+KernelPitch); float* KernelDE=DF.DE_Kernel+KernelSize*(1+KernelPitch);
	//SPG_PIXGRADINFO* GradRef=GRInfo.GradRef+DF.InterpolationBorderSize*DF.SizeX;
	//Reference+=DF.InterpolationBorderSize*DF.SizeX;

	//ComputePosition
	Current+=DF.InterpolationBorderSize*CurPitch;

	for(int y=DF.InterpolationBorderSize;y<DF.SizeY-DF.InterpolationBorderSize;y++)
	{
		//GradInfo
		float xSWX=0;float xSWD=0;float xSWY=0;float xSWE=0;//somme intermediaire pour la precision du calcul numerique
#if(DF_VERSION==4)
		float xSWXX=0;float xSWYY=0;float xSWDE=0;//somme intermediaire pour la precision du calcul numerique
		float xSWXD=0;float xSWXE=0;float xSWYD=0;float xSWYE=0;//somme intermediaire pour la precision du calcul numerique
#endif
		//float xFirstOrder=0;float xSecondOrder=0;
		//int yOffset=(y*DF_GRADSAMPLING)*(DF.PX.SizeX*DF_GRADSAMPLING);

		//ComputePosition
 		float xVX=0;float xVD=0;float xVY=0;float xVE=0;//somme intermediaire pour la precision numerique

		for(int x=DF.InterpolationBorderSize;x<DF.SizeX-DF.InterpolationBorderSize;x++)
		{//VERSION 4 IMMED
			//GradInfo
			//SPG_PIXGRADINFO& GR=GradRef[x];
			SPG_PIXGRADINFO GR;
			GR.S=Reference[x+y*RefPitch];//=(B00+B10+B20+B01+B11+B21+B02+B12+B22)/9
			//xFirstOrder+=GRS;
			//xSecondOrder+=GRS*GRS;
			//DTX=4*d/dt
			//DTD=2*d/dt
			// /3  car 3 lignes, *4/3 car espacement B2-B0 = 3/4 fois pas interpixel
			float DX=0;	float DD=0;	float DY=0;	float DE=0;
			for(int ky=-KernelSize;ky<=KernelSize;ky++)
			{
				//float* RefLine=Reference+V_Sature((y+ky),0,(DF.SizeY-1))*RefPitch;//safe
				float* RefLine=Reference+(y+ky)*RefPitch+x;//unsafe
				float* KernelDXL=KernelDX+ky*KernelPitch; float* KernelDDL=KernelDD+ky*KernelPitch; 
				float* KernelDYL=KernelDY+ky*KernelPitch; float* KernelDEL=KernelDE+ky*KernelPitch;
				for(int kx=-KernelSize;kx<=KernelSize;kx++)
				{
					//float R=RefLine[V_Sature((x+kx),0,(DF.SizeX-1))];//safe
					float R=RefLine[kx];//unsafe
					DX+=R*KernelDXL[kx]; DD+=R*KernelDDL[kx]; 
					DY+=R*KernelDYL[kx]; DE+=R*KernelDEL[kx];
				}
			}
			float WX,WD,WY,WE;
#if(DF_VERSION==4)
			SPG_DF_ComputeWeight(GR.DWX,WX,DX,4.0f*DF.NoiseLevel,DF.NoiseLevel);//calcule weight et weight/derivative SigmaT=sqrt(4termes)
			SPG_DF_ComputeWeight(GR.DWD,WD,DD,2.8284f*DF.NoiseLevel,DF.NoiseLevel);//SigmaT=sqrt(2termes)
			SPG_DF_ComputeWeight(GR.DWY,WY,DY,4.0f*DF.NoiseLevel,DF.NoiseLevel);//calcule weight et weight/derivative
			SPG_DF_ComputeWeight(GR.DWE,WE,DE,2.8284f*DF.NoiseLevel,DF.NoiseLevel);
#elif(DF_VERSION==5)
			SPG_DF_ComputeWeight(GR.DWX,WX,DX,3.0f*DF.NoiseLevel,DF.NoiseLevel);//calcule weight et weight/derivative SigmaT=sqrt(4termes)
			SPG_DF_ComputeWeight(GR.DWD,WD,DD,3.0f*DF.NoiseLevel,DF.NoiseLevel);//SigmaT=sqrt(2termes)
			SPG_DF_ComputeWeight(GR.DWY,WY,DY,3.0f*DF.NoiseLevel,DF.NoiseLevel);//calcule weight et weight/derivative
			SPG_DF_ComputeWeight(GR.DWE,WE,DE,3.0f*DF.NoiseLevel,DF.NoiseLevel);
#endif

			//ComputePosition
			float B0=Current[x];
			float DTS=B0-GR.S;
			xVX+=GR.DWX*DTS;xVD+=GR.DWD*DTS;xVY+=GR.DWY*DTS;xVE+=GR.DWE*DTS;
			
			//GradInfo
			xSWX+=WX;xSWD+=WD;xSWY+=WY;xSWE+=WE;

#if(DF_VERSION==4)
			//weights effectifs des projetés
			xSWXX+=WX; xSWYY+=WY;//1 mesure 1 estimation
			if((WD>0)&&(WE>0)) xSWDE+=2*WD*WE/(WD+WE);//2 mesures 2 estimations mais moitie pixels chacune
			if((WX>0)&&(WD>0)) xSWXD+=WX*WD/(WX+WD);//2 mesures, mais 4 estimations pas indépendantes
			if((WX>0)&&(WE>0)) xSWXE+=WX*WE/(WX+WE);//2 mesures, mais 4 estimations pas indépendantes
			if((WY>0)&&(WD>0)) xSWYD+=WY*WD/(WY+WD);//2 mesures, mais 4 estimations pas indépendantes
			if((WY>0)&&(WE>0)) xSWYE+=WY*WE/(WY+WE);//2 mesures, mais 4 estimations pas indépendantes
#endif
		}//VERSION 4

		//GradInfo
		GRInfo.SWX+=xSWX;GRInfo.SWD+=xSWD;GRInfo.SWY+=xSWY;GRInfo.SWE+=xSWE;
#if(DF_VERSION==4)
		GRInfo.SWXX+=xSWXX;GRInfo.SWYY+=xSWYY;GRInfo.SWDE+=xSWDE;
		GRInfo.SWXD+=xSWXD;GRInfo.SWXE+=xSWXE;GRInfo.SWYD+=xSWYD;GRInfo.SWYE+=xSWYE;
#endif
		//ComputePosition
		VX+=xVX;VD+=xVD;VY+=xVY;VE+=xVE;
		Current+=CurPitch;
	}
#if(DF_VERSION==4)
	if(GRInfo.SWX==0) GRInfo.SWX=1;	if(GRInfo.SWD==0) GRInfo.SWD=1; 
	if(GRInfo.SWY==0) GRInfo.SWY=1; if(GRInfo.SWE==0) GRInfo.SWE=1;

	//ComputePosition
	VX/=GRInfo.SWX;VD/=GRInfo.SWD;VY/=GRInfo.SWY;VE/=GRInfo.SWE;

	float SVX=
		VX*GRInfo.SWXX+
		INVSQRT2*(VD-VE)*GRInfo.SWDE+
		(SQRT2*VD-VY)*GRInfo.SWYD+
		(VY-SQRT2*VE)*GRInfo.SWYE;
	float SVY=
		VY*GRInfo.SWYY+
		INVSQRT2*(VD+VE)*GRInfo.SWDE+
		(SQRT2*VD-VX)*GRInfo.SWXD+
		(SQRT2*VE+VX)*GRInfo.SWXE;

	float DWX=(GRInfo.SWXX+GRInfo.SWDE+GRInfo.SWYD+GRInfo.SWYE);
	float DWY=(GRInfo.SWYY+GRInfo.SWDE+GRInfo.SWXD+GRInfo.SWXE);
	if(DWX<=0) DWX=1; if(DWY<=0) DWY=1;
	PosX=SVX/DWX;PosY=SVY/DWY;
#ifdef DISPSTAURATION
	PosX=V_Sature(PosX,-DISPSTAURATION,DISPSTAURATION);
	PosY=V_Sature(PosY,-DISPSTAURATION,DISPSTAURATION);
#endif
#else
	SPG_DF_Reduction(PosX,PosY,VX,GRInfo.SWX,VD,GRInfo.SWD,VY,GRInfo.SWY,VE,GRInfo.SWE);
#ifdef DISPSTAURATION
	PosX=V_Sature(PosX,-DISPSTAURATION,DISPSTAURATION);
	PosY=V_Sature(PosY,-DISPSTAURATION,DISPSTAURATION);
#endif
#endif
	return;
}

#endif

void SPG_CONV SPG_DF_Decorrelle(SPG_DISPFIELD& DF, float& PosX, float& PosY, float a, float b)
{
	float Mxx,Myx,Mxy,Myy;
	if(a>0) {Mxx=DF.UxP;Myx=DF.UyP;} else {Mxx=DF.UxN;Myx=DF.UyN;}
	if(b>0) {Mxy=DF.VxP;Myy=DF.VyP;} else {Mxy=DF.VxN;Myy=DF.VyN;}
	//a = PosX Mxx + PosY Mxy
	//b = PosX Myx + PosY Myy
	//PosX = ( a Myy - b Mxy ) / ( Mxx Myy - Mxy Myx )
	//PosY = ( a Myx - b Mxx ) / ( Mxy Myx - Mxx Myy )
	float D=Mxx*Myy-Mxy*Myx;
	if(D>0) 
	{
		float invD=1.0f/D;
		PosX=(a*Myy-b*Mxy)*invD;
		PosY=(b*Mxx-a*Myx)*invD;
	}
	else {PosX=0;PosY=0;}
	return;
}

void SPG_CONV SPG_DF_ComputePosition(SPG_DISPFIELD& DF, float& PosX, float& PosY, int UpdateSuperRes)
{
	CHECK(DF.Etat==0,"SPG_DF_ComputePosition",return);
#if(DF_VERSION==4)
	const float INVSQRT2=0.707f;const float SQRT2=1.414f;
#endif
	float VX=0;float VY=0;float VD=0;float VE=0;

	SPG_GRADINFO& GRInfo=DF.GRInfo;
	SPG_PIXGRADINFO* GradRef=GRInfo.GradRef+DF.InterpolationBorderSize*DF.SizeX;
	float* Current=DF.Current+DF.InterpolationBorderSize*DF.SizeX;
	for(int y=DF.InterpolationBorderSize;y<DF.SizeY-DF.InterpolationBorderSize;y++)
	{
		float xVX=0;float xVD=0;float xVY=0;float xVE=0;//somme intermediaire pour la precision numerique
		for(int x=DF.InterpolationBorderSize;x<DF.SizeX-DF.InterpolationBorderSize;x++)
		{
			SPG_PIXGRADINFO& GR=GradRef[x];
			float B0=Current[x];
			float DTS=B0-GR.S;
			xVX+=GR.DWX*DTS;xVD+=GR.DWD*DTS;xVY+=GR.DWY*DTS;xVE+=GR.DWE*DTS;
		}
		VX+=xVX;VD+=xVD;VY+=xVY;VE+=xVE;
		Current+=DF.SizeX;
		GradRef+=DF.SizeX;
	}
#if(DF_VERSION==4)
	VX/=GRInfo.SWX;VD/=GRInfo.SWD;VY/=GRInfo.SWY;VE/=GRInfo.SWE;
	float SVX=
		VX*GRInfo.SWXX+
		INVSQRT2*(VD-VE)*GRInfo.SWDE+
		(SQRT2*VD-VY)*GRInfo.SWYD+
		(VY-SQRT2*VE)*GRInfo.SWYE;
	float SVY=
		VY*GRInfo.SWYY+
		INVSQRT2*(VD+VE)*GRInfo.SWDE+
		(SQRT2*VD-VX)*GRInfo.SWXD+
		(SQRT2*VE+VX)*GRInfo.SWXE;

	float DWX=(GRInfo.SWXX+GRInfo.SWDE+GRInfo.SWYD+GRInfo.SWYE);
	float DWY=(GRInfo.SWYY+GRInfo.SWDE+GRInfo.SWXD+GRInfo.SWXE);
	if(DWX<=0) DWX=1; if(DWY<=0) DWY=1;

	SPG_DF_Decorrelle(DF,PosX,PosY,SVX/DWX,SVY/DWY);

	PosX=V_Sature(PosX,-DISPSTAURATION,DISPSTAURATION);
	PosY=V_Sature(PosY,-DISPSTAURATION,DISPSTAURATION);
#elif(DF_VERSION==5)
	SPG_DF_Reduction(PosX,PosY,VX,GRInfo.SWX,VD,GRInfo.SWD,VY,GRInfo.SWY,VE,GRInfo.SWE);
#ifdef DISPSTAURATION
	PosX=V_Sature(PosX,-DISPSTAURATION,DISPSTAURATION);
	PosY=V_Sature(PosY,-DISPSTAURATION,DISPSTAURATION);
#endif
#endif

	if(UpdateSuperRes)
	{
		if(DF.DS.SuperResolution)
		{
			if((fabs(DF.DS.PosX-PosX)<DF.DS.SpeedIntegrationThreshold)&&
				(fabs(DF.DS.PosY-PosY)<DF.DS.SpeedIntegrationThreshold))
			{
				float xf=(0.5f-PosX)*DF.DS.SuperResolution;
				float yf=(0.5f-PosY)*DF.DS.SuperResolution;
				int xd=V_Floor(xf);
				int yd=V_Floor(yf);
				float ix=xf-(float)xd;
				float iy=yf-(float)yd;
				CHECK((ix<0)||(iy<0),"SPG_DF_ComputePosition: Use Intel RebuildAll",;);
				if(
					V_IsBound(xd,0,DF.DS.SuperResolution)&&V_IsBound(yd,0,DF.DS.SuperResolution)
					&&V_IsBound(ix,0.2f,0.8f)&&V_IsBound(iy,0.2f,0.8f))
					//)
				{
					WB_Copy(DF.DS.WB,DF.Current,xd,yd);
					/*
					Profil P;
					P_Init(P,DF.Current,0,DF.SizeX,DF.SizeY,P_Alias);
					P_Draw(P,Global.Ecran,32,32);
					DoEvents(SPG_DOEV_ALL);
					SPG_Sleep(250);
					P_Close(P);
					*/
				}
			}
			DF.DS.PosX=PosX;
			DF.DS.PosY=PosY;
		}
	}
	return;
}
/*
void SPG_CONV SPG_DF_ComputePosition(SPG_DISPFIELD& DF, float& PosX, float& PosY)
{
	CHECK(DF.Etat==0,"SPG_DF_ComputePosition",return);
	SPG_DF_ComputePositionAt(DF,DF.GRInfo,PosX,PosY,
		DF.InterpolationBorderSize,DF.InterpolationBorderSize,
		DF.SizeX-2*DF.InterpolationBorderSize,DF.SizeY-2*DF.InterpolationBorderSize);
	return;
}
*/

#if(DF_VERSION==3)
void SPG_CONV SPG_DF_MoveReferenceToCurrent(SPG_DISPFIELD& DF, int dX, int dY)
{//VERSION 3
	float* Reference=DF.PX.PixInterpole;
	int RefSizeX=DF_GRADSAMPLING*DF.PX.SizeX;
	int RefSizeY=DF_GRADSAMPLING*DF.PX.SizeY;
	float* Current=DF.Current;
#ifdef DF_SAVECALIBR
	int sX=dX;
	int sY=dY;
#endif

	for(int y=0;y<DF.SizeY;y++)
	{//VERSION 3
		int y0=(V_Sature((y*DF_GRADSAMPLING+dY),0,(RefSizeY-1)))*RefSizeX;
		int y1=(V_Sature((y*DF_GRADSAMPLING+dY+1),0,(RefSizeY-1)))*RefSizeX;
		int y2=(V_Sature((y*DF_GRADSAMPLING+dY+2),0,(RefSizeY-1)))*RefSizeX;
		for(int x=0;x<DF.SizeX;x++)
		{
			int x0=(V_Sature((x*DF_GRADSAMPLING+dX),0,(RefSizeX-1)));
			int x1=(V_Sature((x*DF_GRADSAMPLING+dX+1),0,(RefSizeX-1)));
			int x2=(V_Sature((x*DF_GRADSAMPLING+dX+2),0,(RefSizeX-1)));

			float B00=Reference[x0+y0];float B10=Reference[x1+y0];float B20=Reference[x2+y0];
			float B01=Reference[x0+y1];float B11=Reference[x1+y1];float B21=Reference[x2+y1];
			float B02=Reference[x0+y2];float B12=Reference[x1+y2];float B22=Reference[x2+y2];

			Current[x]=(B00+B10+B20+B01+B11+B21+B02+B12+B22)/9;
		}
		Current+=DF.SizeX;
	}
#ifdef DF_SAVECALIBR
	Profil P;
	P_Init(P,DF.Current,0,DF.SizeX,DF.SizeY,P_Alias);
	if(sX>0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentXP.bmp");
	else if(sX<0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentXN.bmp");
	else if(sY>0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentYP.bmp");
	else if(sY<0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentYN.bmp");
	P_Close(P);
#endif
	return;
}
#elif(DF_VERSION>=4)
void SPG_CONV SPG_DF_MoveReferenceToCurrent(SPG_DISPFIELD& DF, int dX, int dY)
{//VERSION 4
	CHECK(DF.Etat==0,"SPG_DF_MoveReferenceToCurrent",return);
	float* Current=DF.Current;
#ifdef DF_SAVECALIBR
	int sX=dX;
	int sY=dY;
#endif
	PIXINT_GetInterpolationKernel(*(DF.PX),SPG_DF_SENSNEG(dX),SPG_DF_SENSNEG(dY),DF.Interpole_Kernel);
	int KernelSize=DF.PX->KernelSize;
	int KernelPitch=2*KernelSize+1;
	float* Interpole_KernelCenter=DF.Interpole_Kernel+KernelSize*(1+KernelPitch);
	for(int y=0;y<DF.SizeY;y++)
	{//VERSION 4
		for(int x=0;x<DF.SizeX;x++)
		{
			float Sum=0;
			for(int ky=-KernelSize;ky<=KernelSize;ky++)
			{
				float* RefLine=DF.Reference+V_Sature((y+ky),0,(DF.SizeY-1))*DF.SizeX;
				float* KernelL=Interpole_KernelCenter+ky*KernelPitch;
				for(int kx=-KernelSize;kx<=KernelSize;kx++)
				{
					float R=RefLine[V_Sature((x+kx),0,(DF.SizeX-1))];
					Sum+=R*KernelL[kx];
				}
			}
			Current[x]=Sum;
		}
		Current+=DF.SizeX;
	}
#ifdef DF_SAVECALIBR
	Profil P;
	P_Init(P,DF.Current,0,DF.SizeX,DF.SizeY,P_Alias);
	if(sX>0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentXP.bmp");
	else if(sX<0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentXN.bmp");
	else if(sY>0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentYP.bmp");
	else if(sY<0)
		BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),0,255.99,"CalibrCurrentYN.bmp");
	P_Close(P);
#endif
	return;
}
#endif

void SPG_CONV SPG_DF_Calibrate(SPG_DISPFIELD& DF)
{
	DF.UxP=1;DF.UyP=0;
	DF.UxN=1;DF.UyN=0;
	DF.VxP=0;DF.VyP=1;
	DF.VxN=0;DF.VyN=1;

	CHECK(DF.Etat==0,"SPG_DF_Calibrate",return);

#ifndef DF_NOCALIBR
	float UxP,UyP,UxN,UyN,VxP,VyP,VxN,VyN;

	SPG_DF_MoveReferenceToCurrent(DF,DF_CALIBRATIONSCALE,0);
	SPG_DF_ComputePosition(DF,UxP,UyP);
	SPG_DF_MoveReferenceToCurrent(DF,-DF_CALIBRATIONSCALE,0);
	SPG_DF_ComputePosition(DF,UxN,UyN);

	SPG_DF_MoveReferenceToCurrent(DF,0,DF_CALIBRATIONSCALE);
	SPG_DF_ComputePosition(DF,VxP,VyP);
	SPG_DF_MoveReferenceToCurrent(DF,0,-DF_CALIBRATIONSCALE);
	SPG_DF_ComputePosition(DF,VxN,VyN);
#if(DF_VERSION==3)
	int MOVESTEP=PIXINT_RELAX_OVERSAMPLING;
#elif(DF_VERSION>=4)
	float MOVESTEP=((float)(2*DF.PX->Oversampling+1))/DF_CALIBRATIONSCALE;
#endif

	DF.UxP=UxP*-MOVESTEP;
	DF.UyP=UyP*-MOVESTEP;
	DF.UxN=UxN*MOVESTEP;
	DF.UyN=UyN*MOVESTEP;

	DF.VxP=VxP*-MOVESTEP;
	DF.VyP=VyP*-MOVESTEP;
	DF.VxN=VxN*MOVESTEP;
	DF.VyN=VyN*MOVESTEP;

	if(DF.UxP<DF.CorrelationThreshold) DF.UxP=DF.CorrelationThreshold;
	if(DF.UxN<DF.CorrelationThreshold) DF.UxN=DF.CorrelationThreshold;
	if(DF.VyP<DF.CorrelationThreshold) DF.VyP=DF.CorrelationThreshold;
	if(DF.VyN<DF.CorrelationThreshold) DF.VyN=DF.CorrelationThreshold;

	float InvCorrelation=0.25f/DF.CorrelationThreshold;

	if(fabs(DF.UyP)>InvCorrelation) DF.UyP=V_Signe(DF.UyP)*InvCorrelation;
	if(fabs(DF.UyN)>InvCorrelation) DF.UyN=V_Signe(DF.UyN)*InvCorrelation;
	if(fabs(DF.VxP)>InvCorrelation) DF.VxP=V_Signe(DF.VxP)*InvCorrelation;
	if(fabs(DF.VxN)>InvCorrelation) DF.VxN=V_Signe(DF.VxN)*InvCorrelation;
#endif
	SPG_DF_MoveReferenceToCurrent(DF,0,0);
	return;
}

#ifdef DF_PREFILTER
void SPG_CONV SPG_DF_HighPass(float* restrict Dst, float* restrict Src, int SrcPitch, int SizeX, int SizeY, int Width)
{
	/*
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			int Count=0;
			float Sum=0;
			for(int ys=V_Max(y-Width,0);ys<V_Min(y+Width+1,SizeY);ys++)
			{
				for(int xs=V_Max(x-Width,0);xs<V_Min(x+Width+1,SizeX);xs++)
				{
					Sum+=Src[xs+ys*SrcPitch];
					Count++;
				}
			}
			Dst[x+y*SizeX]=Src[x+y*SrcPitch]-(Count?(Sum/Count):0);
		}
	}
	*/
	for(int y=0;y<SizeY;y++)
	{
		memcpy(Dst+y*SizeX,Src+y*SrcPitch,SizeX*sizeof(float));
	}
	Profil PDst;
	P_Init(PDst,Dst,0,SizeX,SizeY,P_Alias);
	P_FastConvHighPass(PDst,2*Width);
	P_Close(PDst);
	return;
}
#endif

void SPG_CONV SPG_DF_LightControl(float& Mul, float& Add, float* restrict Reference, int PitchRef, int SizeX, int SizeY, float* restrict Current, int PitchCur)
{
	Add=0;
	Mul=1;
	CHECK(Reference==0,"SPG_DF_LightControl",return);
	CHECK(Current==0,"SPG_DF_LightControl",return);
	CHECK((SizeX<=0)||(SizeY<=0),"SPG_DF_LightControl",return);
	if(PitchRef==0) PitchRef=SizeX;
	if(PitchCur==0) PitchCur=SizeX;
	float Sum=0; float FirstMoment=0; float FirstOrder=0; float SecondOrder=0;
	for(int y=0;y<SizeY;y++)
	{
		float xSum=0; float xFirstMoment=0; float xFirstOrder=0; float xSecondOrder=0;
		for(int x=0;x<SizeX;x++)
		{
			//reference=x current=y
			//y = ax + b
			//Sum         = somme(y) = somme(ax + b) = a somme(x) + Nb
			//FirstMoment = somme(xy) = somme(axx + bx) = a somme(xx) + b somme(x)
			//FirstOrder  = somme(x)
			//SecondOrder = somme(xx)
			xFirstOrder+=Reference[x];
			xSecondOrder+=Reference[x]*Reference[x];
			xSum+=Current[x];
			xFirstMoment+=Current[x]*Reference[x];
		}
		FirstOrder+=xFirstOrder; SecondOrder+=xSecondOrder; Sum+=xSum; FirstMoment+=xFirstMoment;
		Reference+=PitchRef;
		Current+=PitchCur;
	}
	float D=(SizeX*SizeY*SecondOrder - FirstOrder*FirstOrder);//variance des données de reference fois N
	if(D!=0)//math.D>=0
	{
		float invD=1.0f/D;
		//somme(xx) * Sum - somme(x) * FirstMoment = N somme(xx) b - b somme(x) somme(x)
		//b=(somme(xx) * Sum - somme(x) * FirstMoment)/(N somme(xx) - somme(x) somme(x))
		Add=(SecondOrder*Sum - FirstOrder*FirstMoment)*invD;
		//somme(x) * Sum - N * FirstMoment = a somme(x) somme(x) - a somme(xx) N
		//a=(somme(x) * Sum - N * FirstMoment)/(somme(x) somme(x) - a somme(xx) N)
		Mul=(SizeX*SizeY*FirstMoment - FirstOrder*Sum)*invD;
	}
	return;
}

int SPG_CONV SPG_DF_UpdateReferenceWithCurrent(SPG_DISPFIELD& DF)
{
	float* Reference=DF.Reference;
	float* Current=DF.Current;
	float TimeCste=1.0f-1.0f/(1+V_Min(DF.ReferenceIntegrationCount,DF.ReferenceIntegration));
	//RerenceIntegrationCount=1	TimeCste=0.5f ou moins
	//RerenceIntegrationCount=2	TimeCste=0.6666f ou moins
	//RerenceIntegrationCount=3	TimeCste=0.75f ou moins
	//le nombre d'images integrees fixe la vitesse de rafraichissement
	//le parametre fixe une vitesse minimum
	DF.ReferenceIntegrationCount++;

	for(int y=0;y<DF.SizeY;y++)
	{
		for(int x=0;x<DF.SizeX;x++)
		{
			Reference[x]=TimeCste*Reference[x]+(1.0f-TimeCste)*Current[x];
			DF_CheckFloat(Reference[x],"SPG_DF_UpdateReferenceWithCurrent");
		}
		Reference+=DF.SizeX;
		Current+=DF.SizeX;
	}
	if((DF.ReferenceIntegrationCount==DF.ReferenceIntegration)&&(DF.ReferenceIntegrationCount>0))
	{
#if(DF_VERSION==3)
		SPG_DF_ComputeIntensity(DF);
#endif
		SPG_DF_ComputeGradInfo(DF,DF.GRInfo,DF.Reference);
		SPG_DF_Calibrate(DF);
		return -1;
	}
	return 0;
}

#endif

