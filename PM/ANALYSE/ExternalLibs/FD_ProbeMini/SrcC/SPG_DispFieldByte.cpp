
#include "SPG_General.h"

#ifdef SPG_General_USEDISPFIELD

#include "SPG_Includes.h"
#include "SPG_DispFieldConfig.h"

#include <memory.h>
#include <float.h>

void SPG_CONV SPG_DF_SetByteFrame(SPG_DISPFIELD& DF, float* restrict Dst, SPG_CAMPIXEL* restrict E, int Pitch)
{
	CHECK(DF.Etat==0,"SPG_DF_SetByteFrame",return);
	CHECK(Dst==0,"SPG_DF_SetByteFrame",return);
	CHECK(E==0,"SPG_DF_SetByteFrame",return);
	if(Pitch==0) Pitch=DF.SizeX;
	for(int y=0;y<DF.SizeY;y++)
	{
		for(int x=0;x<DF.SizeX;x++)
		{
			Dst[x]=(float)E[x];
		}
		Dst+=DF.SizeX;
		E+=Pitch;
	}
	return;
}

void SPG_CONV SPG_DF_UpdateByteFrame(SPG_DISPFIELD& DF, float* restrict Dst, SPG_CAMPIXEL* restrict E, int Pitch, float TimeCste_0_1)
{
	CHECK(DF.Etat==0,"SPG_DF_UpdateByteFrame",return);
	CHECK(Dst==0,"SPG_DF_UpdateByteFrame",return);
	CHECK(E==0,"SPG_DF_UpdateByteFrame",return);
	for(int y=0;y<DF.SizeY;y++)
	{
		for(int x=0;x<DF.SizeX;x++)
		{
			Dst[x]=TimeCste_0_1*Dst[x]+(1.0f-TimeCste_0_1)*(float)E[x];
			DF_CheckFloat(Dst[x],"SPG_DF_UpdateByteFrame");
		}
		Dst+=DF.SizeX;
		E+=Pitch;
	}
	return;
}

void SPG_CONV SPG_DF_SetByteLightFrame(SPG_DISPFIELD& DF, float* restrict Dst, SPG_CAMPIXEL* restrict E, int Pitch, float Mul, float Add)
{
	CHECK(DF.Etat==0,"SPG_DF_SetByteLightFrame",return);
	CHECK(Dst==0,"SPG_DF_SetByteLightFrame",return);
	CHECK(E==0,"SPG_DF_SetByteLightFrame",return);
	if(Pitch==0) Pitch=DF.SizeX;
	for(int y=0;y<DF.SizeY;y++)
	{
		for(int x=0;x<DF.SizeX;x++)
		{
			Dst[x]=Add+Mul*(float)E[x];
		}
		Dst+=DF.SizeX;
		E+=Pitch;
	}
	return;
}

void SPG_CONV SPG_DF_UpdateByteLightFrame(SPG_DISPFIELD& DF, float* restrict Dst, SPG_CAMPIXEL const* restrict E, const int Pitch, const float TimeCste_0_1, const float Mul, const float Add, const float NonLinearMaxAmplitude)
{
	CHECK(DF.Etat==0,"SPG_DF_UpdateByteLightFrame",return);
	CHECK(Dst==0,"SPG_DF_UpdateByteLightFrame",return);
	CHECK(E==0,"SPG_DF_UpdateByteLightFrame",return);
	if(NonLinearMaxAmplitude==0)
	{
		for(int y=0;y<DF.SizeY;y++)
		{
			for(int x=0;x<DF.SizeX;x++)
			{
				Dst[x]=TimeCste_0_1*Dst[x]+(1.0f-TimeCste_0_1)*(Add+Mul*(float)E[x]);
				DF_CheckFloat(Dst[x],"SPG_DF_UpdateByteFrame");
			}
			Dst+=DF.SizeX;
			E+=Pitch;
		}
	}
	else
	{
		for(int y=0;y<DF.SizeY;y++)
		{
			const float AT=TimeCste_0_1*NonLinearMaxAmplitude;
			for(int x=0;x<DF.SizeX;x++)
			{
				//Dst[x]=TimeCste_0_1*Dst[x]+(1.0f-TimeCste_0_1)*(Add+Mul*(float)E[x]);
//y3 = Dst - DeltaT * A * (1 - T) / (A + Abs(DeltaT))
				float Dest=Add+Mul*(float)E[x];
				float DeltaT=Dest-Dst[x];
				//float C=1.0f-TimeCste_0_1+V_Min(DeltaT*DeltaT*InvAmp,TimeCste_0_1);
				Dst[x]=Dest-AT*DeltaT/(NonLinearMaxAmplitude+fabs(DeltaT));
				DF_CheckFloat(Dst[x],"SPG_DF_UpdateByteFrame");
			}
			Dst+=DF.SizeX;
			E+=Pitch;
		}
	}
	return;
}

void SPG_CONV SPG_DF_SetCurrentByteFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* restrict E, int Pitch)
{
	if(Pitch==0) Pitch=DF.SizeX;
#ifdef DF_PREFILTER
	if(DF.PreFilterSize==0)
	{
		SPG_DF_SetByteFrame(DF,DF.Current,E,Pitch);
	}
	else
	{
		SPG_DF_SetByteFrame(DF,DF.PreFilterIn,E,Pitch);
		SPG_DF_HighPass(DF.Current,DF.PreFilterIn,DF.SizeX,DF.SizeX,DF.SizeY,DF.PreFilterSize);
	}
#else
		SPG_DF_SetByteFrame(DF,DF.Current,E,Pitch);
#endif
	return;
}

void SPG_CONV SPG_DF_UpdateCurrentByteFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* restrict E, int Pitch, float TimeCste_0_1)
{
#ifdef DF_PREFILTER
	if(DF.PreFilterSize==0)
	{
		SPG_DF_UpdateByteFrame(DF,DF.Current,E,Pitch,TimeCste_0_1);
	}
	else
	{
		SPG_DF_SetByteFrame(DF,DF.PreFilterIn,E,Pitch);
		SPG_DF_HighPass(DF.PreFilterOut,DF.PreFilterIn,DF.SizeX,DF.SizeX,DF.SizeY,DF.PreFilterSize);
		SPG_DF_UpdateFloatFrame(DF,DF.Current,DF.PreFilterOut,DF.SizeX,TimeCste_0_1);
	}
#else
	SPG_DF_UpdateByteFrame(DF,DF.Current,E,Pitch,TimeCste_0_1);
#endif
	return;
}

void SPG_CONV SPG_DF_SetCurrentByteLightFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* restrict E, int Pitch)
{
	CHECK(DF.Etat==0,"SPG_DF_SetCurrentByteLightFrame",return);
	CHECK(E==0,"SPG_DF_SetCurrentByteLightFrame",return);
	float Add=0;
	float Mul=1;
	float FirstOrder=DF.GRInfo.FirstOrder;
	float SecondOrder=DF.GRInfo.SecondOrder;
	int SizeX=DF.SizeX-2*DF.InterpolationBorderSize;
	int SizeY=DF.SizeY-2*DF.InterpolationBorderSize;
	float D=(SizeX*SizeY*SecondOrder - FirstOrder*FirstOrder);//variance des données de reference fois N
	if(D!=0)//math.D>=0
	{
		float Sum=0; float FirstMoment=0;
		SPG_GRADINFO& GRInfo=DF.GRInfo;
		SPG_PIXGRADINFO* GradRef=GRInfo.GradRef+DF.InterpolationBorderSize*DF.SizeX;
		SPG_CAMPIXEL* Current=E+DF.InterpolationBorderSize*Pitch;
		for(int y=DF.InterpolationBorderSize;y<DF.SizeY-DF.InterpolationBorderSize;y++)
		{
			float xSum=0; float xFirstMoment=0;
			for(int x=DF.InterpolationBorderSize;x<DF.SizeX-DF.InterpolationBorderSize;x++)
			{
				float B=(float)Current[x];
				xSum+=B;
				xFirstMoment+=B*GradRef[x].S;
			}
			Sum+=xSum; FirstMoment+=xFirstMoment;
			Current+=Pitch;
			GradRef+=DF.SizeX;
		}
		float invD=1.0f/D;
		//somme(xx) * Sum - somme(x) * FirstMoment = N somme(xx) b - b somme(x) somme(x)
		//b=(somme(xx) * Sum - somme(x) * FirstMoment)/(N somme(xx) - somme(x) somme(x))
		Add=(SecondOrder*Sum - FirstOrder*FirstMoment)*invD;
		//somme(x) * Sum - N * FirstMoment = a somme(x) somme(x) - a somme(xx) N
		//a=(somme(x) * Sum - N * FirstMoment)/(somme(x) somme(x) - a somme(xx) N)
		Mul=(SizeX*SizeY*FirstMoment - FirstOrder*Sum)*invD;

		Mul=V_Sature(Mul,0.2f,5.0f);
	}
	SPG_DF_SetByteLightFrame(DF,DF.Current,E,Pitch,1.0f/Mul,-Add/Mul);
	return;
}

void SPG_CONV SPG_DF_UpdateCurrentByteLightFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* restrict E, int Pitch, float TimeCste_0_1, float NonLinearMaxAmplitude)
{
	CHECK(DF.Etat==0,"SPG_DF_UpdateCurrentByteLightFrame",return);
	CHECK(E==0,"SPG_DF_UpdateCurrentByteLightFrame",return);
	float Add=0;
	float Mul=1;
	float FirstOrder=DF.GRInfo.FirstOrder;
	float SecondOrder=DF.GRInfo.SecondOrder;
	int SizeX=DF.SizeX-2*DF.InterpolationBorderSize;
	int SizeY=DF.SizeY-2*DF.InterpolationBorderSize;
	float D=(SizeX*SizeY*SecondOrder - FirstOrder*FirstOrder);//variance des données de reference fois N
	if(D!=0)//math.D>=0
	{
		float Sum=0; float FirstMoment=0;
		SPG_GRADINFO& GRInfo=DF.GRInfo;
		SPG_PIXGRADINFO* GradRef=GRInfo.GradRef+DF.InterpolationBorderSize*DF.SizeX;
		SPG_CAMPIXEL* Current=E+DF.InterpolationBorderSize*Pitch;
		for(int y=DF.InterpolationBorderSize;y<DF.SizeY-DF.InterpolationBorderSize;y++)
		{
			float xSum=0; float xFirstMoment=0;
			for(int x=DF.InterpolationBorderSize;x<DF.SizeX-DF.InterpolationBorderSize;x++)
			{
				float B=(float)Current[x];
				xSum+=B;
				xFirstMoment+=B*GradRef[x].S;
			}
			Sum+=xSum; FirstMoment+=xFirstMoment;
			Current+=Pitch;
			GradRef+=DF.SizeX;
		}
		float invD=1.0f/D;
		//somme(xx) * Sum - somme(x) * FirstMoment = N somme(xx) b - b somme(x) somme(x)
		//b=(somme(xx) * Sum - somme(x) * FirstMoment)/(N somme(xx) - somme(x) somme(x))
		Add=(SecondOrder*Sum - FirstOrder*FirstMoment)*invD;
		//somme(x) * Sum - N * FirstMoment = a somme(x) somme(x) - a somme(xx) N
		//a=(somme(x) * Sum - N * FirstMoment)/(somme(x) somme(x) - a somme(xx) N)
		Mul=(SizeX*SizeY*FirstMoment - FirstOrder*Sum)*invD;

		Mul=V_Sature(Mul,0.2f,5.0f);
	}
	SPG_DF_UpdateByteLightFrame(DF,DF.Current,E,Pitch,TimeCste_0_1,1.0f/Mul,-Add/Mul,NonLinearMaxAmplitude);
	return;
}

void SPG_CONV SPG_DF_SetReferenceByteFrame(SPG_DISPFIELD& DF, SPG_CAMPIXEL* restrict E, int Pitch)
{
	if(Pitch==0) Pitch=DF.SizeX;
#ifdef DF_PREFILTER
	if(DF.PreFilterSize==0)
	{
		SPG_DF_SetByteFrame(DF,DF.Reference,E,Pitch);
	}
	else
	{
		SPG_DF_SetByteFrame(DF,DF.PreFilterIn,E,Pitch);
		SPG_DF_HighPass(DF.Reference,DF.PreFilterIn,DF.SizeX,DF.SizeX,DF.SizeY,DF.PreFilterSize);
	}
#else
	SPG_DF_SetByteFrame(DF,DF.Reference,E,Pitch);
#endif
	DF.ReferenceIntegrationCount=1;
#if(DF_VERSION==3)
	SPG_DF_ComputeIntensity(DF);
#endif
	SPG_DF_ComputeGradInfo(DF,DF.GRInfo,DF.Reference);
	SPG_DF_Calibrate(DF);
	return;
}

#endif
