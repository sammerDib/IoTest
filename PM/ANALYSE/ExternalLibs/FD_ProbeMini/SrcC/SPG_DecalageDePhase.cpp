
#include "SPG_General.h"

#ifdef SPG_General_USEDECPHASE

#include "SPG_Includes.h"
#include <memory.h>


static void SPG_CONV SPG_DecalageDePhase_Invalid(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);
static void SPG_CONV SPG_DecalageDePhase_TRI_120(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);
static void SPG_CONV SPG_DecalageDePhase_CARRE4_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);
static void SPG_CONV SPG_DecalageDePhase_LARKIN5_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);
static void SPG_CONV SPG_DecalageDePhase_HARIHARAN5_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);
static void SPG_CONV SPG_DecalageDePhase_HARIHARAN6_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);
static void SPG_CONV SPG_DecalageDePhase_LARKIN7_514(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);
static void SPG_CONV SPG_DecalageDePhase_HARIHARAN7_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);


int SPG_CONV SPG_DecalageDePhaseInit(DECALAGEDEPHASE& DP, DECALAGEDEPHASEID id)
{
	SPG_ZeroStruct(DP);
	DP.id=id;
	switch(DP.id)
	{
	case TRI_120:
		DP.NumSteps=3;
		DP.PhaseSteps=3;
		DP.F=SPG_DecalageDePhase_TRI_120;
		break;
	case CARRE4_90:
		DP.NumSteps=4;
		DP.PhaseSteps=4;
		DP.F=SPG_DecalageDePhase_CARRE4_90;
		break;
	case LARKIN5_90:
		DP.NumSteps=5;
		DP.PhaseSteps=4;
		DP.F=SPG_DecalageDePhase_LARKIN5_90;
		break;
	case HARIHARAN5_90:
		DP.NumSteps=5;
		DP.PhaseSteps=4;
		DP.F=SPG_DecalageDePhase_HARIHARAN5_90;
		break;
	case LARKIN7_514:
		DP.NumSteps=5;
		DP.PhaseSteps=5;
		DP.F=SPG_DecalageDePhase_LARKIN7_514;
		break;
	case HARIHARAN7_90:
		DP.NumSteps=7;
		DP.PhaseSteps=4;
		DP.F=SPG_DecalageDePhase_HARIHARAN7_90;
		break;
	default:
		DbgCHECK(1,"SPG_DecalageDePhaseInit");
		DP.F=SPG_DecalageDePhase_Invalid;
		return 0;
		//break;
	}

	CHECK(DP.F==0,"SPG_DecalageDePhaseInit",return 0);

	DP.Sqrt3=sqrtf(3.0);

	float* I=SPG_TypeAlloc(DP.NumSteps,float,"SPG_DecalageDePhase");
	float A=1,PC=0,PS=V_HPI;
	{for(int i=0;i<DP.NumSteps;i++)
	{
		I[i]=cos((2*i-DP.NumSteps)*V_PI/DP.PhaseSteps);
	}}

	SPG_DecalageDePhase_Process(DP,1,&PC,&A,1,I,1,1);
	DP.PhaseOfCos=PC;
	DP.AmplitudeNormalization=1/A;
	{for(int i=0;i<DP.NumSteps;i++)
	{
		I[i]=sin((2*i-DP.NumSteps)*V_PI/DP.PhaseSteps);
	}}
	SPG_DecalageDePhase_Process(DP,1,&PS,&A,1,I,1,1);
	DP.PhaseOfSin=PS;
	DP.Signe=atan2(PS,PC)/V_HPI;
	DP.AmplitudeNormalization=(DP.AmplitudeNormalization + 1/A)/2;
	SPG_MemFree(I);
	return -1;
}

void SPG_CONV SPG_DecalageDePhaseClose(DECALAGEDEPHASE& DP)
{
	SPG_ZeroStruct(DP);
	return;
}

void SPG_CONV SPG_DecalageDePhase_Process(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	CHECK(DP.F==0,"SPG_DecalageDePhase_Process",return);
	SPG_MemFastCheck();
	DP.F(DP,N,Phase,Amplitude,rPitch,I,iStep,iPitch);
	SPG_MemFastCheck();
	return;
}

static void SPG_CONV SPG_DecalageDePhase_Invalid(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	DbgCHECK(1,"SPG_DecalageDePhase_Invalid");
	return;
}

static void SPG_CONV SPG_DecalageDePhase_TRI_120(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	for(int i=0;i<N;i++)
	{
		float& I0=I[0]; float& I1=I[iStep]; float& I2=I[2*iStep];

		if(Phase) *Phase=atan2(DP.Sqrt3*(I1-I2),(2*(I0)-I1-I2));

		if(Amplitude)
		{
			float IMoy=(I0+I1+I2)*.333333333f;
			*Amplitude=sqrtf(SPG_SquareDiffMacro(I0,IMoy)+SPG_SquareDiffMacro(I1,IMoy)+SPG_SquareDiffMacro(I2,IMoy));
		}

		if(Phase) Phase+=rPitch;
		if(Amplitude) Amplitude+=rPitch;
		I+=iPitch;
	}
	return;
}

static void SPG_CONV SPG_DecalageDePhase_CARRE4_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	for(int i=0;i<N;i++)
	{
		float& I0=I[0]; float& I1=I[iStep]; float& I2=I[2*iStep]; float& I3=I[3*iStep];

		if(Phase)
		{
			float ICOMB=
			   ((3*(I1-I2))-(I0-I3))*
			   ((I0-I3)+(I1-I2));

			float num;
			if(ICOMB>0) 
				num=sqrt(ICOMB);
			else
			{
				num=0;
			}
			float denom=(I1+I2)-(I0+I3);
			if((I1-I2)<0) num=-num;
			//calcul d'un argument entre 0 et 2pi
			*Phase=atan2(num,denom);
		}
		if(Amplitude)
		{
			float m0=(I1-I2)+(I0-I3);
			float m1=(I1+I2)-(I0+I3);
			*Amplitude=sqrt(m0*m0+m1*m1);
		}

		if(Phase) Phase+=rPitch;
		if(Amplitude) Amplitude+=rPitch;
		I+=iPitch;
	}
	return;
}

// APEIMSource Demodulation.cpp 

static void SPG_CONV SPG_DecalageDePhase_LARKIN5_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	for(int i=0;i<N;i++)
	{
		float& I0=I[0]; float& I1=I[iStep]; float& I2=I[2*iStep]; float& I3=I[3*iStep]; float& I4=I[4*iStep];

        float SPI2I4 = (I1-I3)*(I1-I3);
		float A = SPI2I4 + (I2-I0)*(I2-I4);
		float SINP = 2*I2-I4-I0;
        float COSP2 = 4*SPI2I4 - (I0-I4)*(I0-I4);

		if(Amplitude) *Amplitude=0.5*sqrtf(fabsf(A));


        float c = sqrtf(fabsf(COSP2));

        if (I3<I1) c=-c;

		if(Phase) *Phase=atan2f(SINP,c);

		if(Phase) Phase+=rPitch;
		if(Amplitude) Amplitude+=rPitch;
		I+=iPitch;
	}
	return;
}

static void SPG_CONV SPG_DecalageDePhase_HARIHARAN5_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	for(int i=0;i<N;i++)
	{
		float& I0=I[0]; float& I1=I[iStep]; float& I2=I[2*iStep]; float& I3=I[3*iStep]; float& I4=I[4*iStep];

		float num=2*(I1-I3);
		float denom=(2*I2-I4-I0);
		if(Phase) *Phase=atan2(num,denom);
		if(Amplitude) *Amplitude=sqrt(num*num+denom*denom);

		if(Phase) Phase+=rPitch;
		if(Amplitude) Amplitude+=rPitch;
		I+=iPitch;
	}
	return;
}

static void SPG_CONV SPG_DecalageDePhase_HARIHARAN6_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	for(int i=0;i<N;i++)
	{
		float& I0=I[0]; float& I1=I[iStep]; float& I2=I[2*iStep]; float& I3=I[3*iStep]; float& I4=I[4*iStep]; float& I5=I[5*iStep];

		float num=-3*I1+4*I3-I5;
		float denom=I0-4*I2+3*I4;
		if(Phase) *Phase=atan2(num,denom);
		if(Amplitude) *Amplitude=sqrt(num*num+denom*denom);

		if(Phase) Phase+=rPitch;
		if(Amplitude) Amplitude+=rPitch;
		I+=iPitch;
	}
	return;
}

static void SPG_CONV SPG_DecalageDePhase_LARKIN7_514(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	for(int i=0;i<N;i++)
	{
		float& I0=I[0]; float& I1=I[iStep]; float& I2=I[2*iStep]; float& I3=I[3*iStep]; float& I4=I[4*iStep]; float& I5=I[5*iStep]; float& I6=I[5*iStep];

		double num=DP.Sqrt3*(I1+I2-I4-I5);
		double denom=(I0+I1-I2-2*I3-I4+I5+I6);
		if(Phase) *Phase=atan2(num,denom);
		if(Amplitude) *Amplitude=sqrt(num*num+denom*denom);

		if(Phase) Phase+=rPitch;
		if(Amplitude) Amplitude+=rPitch;
		I+=iPitch;
	}
	return;
}

static void SPG_CONV SPG_DecalageDePhase_HARIHARAN7_90(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch)
{
	for(int i=0;i<N;i++)
	{
		float& I0=I[0]; float& I1=I[iStep]; float& I2=I[2*iStep]; float& I3=I[3*iStep]; float& I4=I[4*iStep]; float& I5=I[5*iStep]; float& I6=I[5*iStep];

		float num=4*I1-2*I3+6*I5;
		float denom=-I0+7*I2-7*I4+I6;
		if(Phase) *Phase=atan2(num,denom);
		if(Amplitude) *Amplitude=sqrt(num*num+denom*denom);

		if(Phase) Phase+=rPitch;
		if(Amplitude) Amplitude+=rPitch;
		I+=iPitch;
	}
	/*
	//voir aussi 90°
  N = 3 (I3 - I5) - (I1 - I7)
  D = 4I4 - 2 (I2 + I6)
	*/
	return;
}





















































#define UseLarkin

int SPG_CONV SPG_DecalageDePhase_StepPerLambda(int INum)
{
	switch(INum)
	{
	case 1:
		return 1;
	case 2:
		return 2;
	case 3:
		return 3;
	case 4:
		return 4;
	case 5:
		return 4;
	case 6:
		return 4;
	case 7:
#ifdef UseLarkin
		return 7;
#else
		return 4;
#endif
	default:
		return 0;
	}
}

void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* Phase, float* Contraste)
{
	CHECK(I0==0,"DecalageDePhase",return);
	CHECK(I1==0,"DecalageDePhase",return);
	CHECK(I2==0,"DecalageDePhase",return);
	CHECK(Phase==0,"DecalageDePhase",return);
	float Sqrt3=sqrtf(3.0);
	if(Contraste)
	{
		for(int i=0;i<SizeX*SizeY;i++)
		{
			SPG_DecalageDePhase120(I0[i],I1[i],I2[i],Phase[i],Contraste[i]);
			/*
			Phase[i]=atan2(Sqrt3*(I1[i]-I2[i]),(2*I0[i]-I1[i]-I2[i]));
			float IMoy=(I0[i]+I1[i]+I2[i])/3;
			Contraste[i]=sqrtf((I0[i]-IMoy)*(I0[i]-IMoy)+(I1[i]-IMoy)*(I1[i]-IMoy)+(I2[i]-IMoy)*(I2[i]-IMoy));
			*/
		}
	}
	else
	{
		for(int i=0;i<SizeX*SizeY;i++)
		{
			Phase[i]=atan2(Sqrt3*(I1[i]-I2[i]),(2*I0[i]-I1[i]-I2[i]));
		}
	}
	return;
}

void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* Phase, float* Contraste)
{
	CHECK(I0==0,"DecalageDePhase",return);
	CHECK(I1==0,"DecalageDePhase",return);
	CHECK(I2==0,"DecalageDePhase",return);
	CHECK(I3==0,"DecalageDePhase",return);
	CHECK(Phase==0,"DecalageDePhase",return);
//Carré
	for(int i=0;i<SizeX*SizeY;i++)
	{
		float num,denom;
		float i0=I0[i];
		float i1=I1[i];
		float i2=I2[i];
		float i3=I3[i];

		float ICOMB=
		   ((3*(i1-i2))-(i0-i3))*
		   ((i0-i3)+(i1-i2));
		if(ICOMB>0) 
			num=sqrt(ICOMB);
		else
		{
			num=0;
		}
		denom= (i1+i2)-(i0+i3);
		if((i1-i2)<0) num=-num;
		//calcul d'un argument entre 0 et 2pi
		Phase[i]=atan2(num,denom);
		if(Contraste)
		{
			float m0=(i1-i2)+(i0-i3);
			float m1=(i1+i2)-(i0+i3);
			Contraste[i]=sqrt(m0*m0+m1*m1);
		}
	}
	return;
}

void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* I4, float* Phase, float* Contraste)
{
	CHECK(I0==0,"DecalageDePhase",return);
	CHECK(I1==0,"DecalageDePhase",return);
	CHECK(I2==0,"DecalageDePhase",return);
	CHECK(I3==0,"DecalageDePhase",return);
	CHECK(I4==0,"DecalageDePhase",return);
	CHECK(Phase==0,"DecalageDePhase",return);
#ifdef UseLarkin

	//APEIM\APEIMSource\Source\Demodulation.cpp
	/*
		RowVector I1 = I.Row(ptrframe-dj+1);
		RowVector I2 = I.Row(ptrframe-dj+2);
		RowVector I3 = I.Row(ptrframe-dj+3);
		RowVector I4 = I.Row(ptrframe-dj+4);
		RowVector I5 = I.Row(ptrframe-dj+5);

        RowVector SPI2I4 = SP(I2-I4,I2-I4);
		RowVector A = SPI2I4 + SP(I3-I1,I3-I5);
		RowVector SINP = 2*I3-I5-I1;
        RowVector COSP2 = 4*SPI2I4 - SP(I1-I5,I1-I5);
		RowVector O = ( I1 + 2*I3 + I5) / 4; 

		for(pix=0;pix <nbpixel;pix++)
		{
			demod->Amplitude[(ptrframe-demod->first)*nbpixel+pix]=sqrtf(fabsf(TypReal(A(pix+1))))/2.0f;
            TypReal c = (TypReal)sqrt(fabs(COSP2(pix+1)));
            if (I4(pix+1)<I2(pix+1)) c=-c;
			demod->Phase[(ptrframe-demod->first)*nbpixel+pix]=atan2f((TypReal)SINP(pix+1),c);
			demod->Offset[(ptrframe-demod->first)*nbpixel+pix] = (TypReal)O(pix+1);
		}
	*/


	for(int i=0;i<SizeX*SizeY;i++)
	{
        float SPI2I4 = (I1[i]-I3[i])*(I1[i]-I3[i]);
		float A = SPI2I4 + (I2[i]-I0[i])*(I2[i]-I4[i]);
		float SINP = 2*I2[i]-I4[i]-I0[i];
        float COSP2 = 4*SPI2I4 - (I0[i]-I4[i])*(I0[i]-I4[i]);

		if(Contraste)
		{
			Contraste[i]=0.5*sqrtf(fabsf(A));
		}

        float c = sqrtf(fabsf(COSP2));

        if (I3[i]<I1[i]) c=-c;

		Phase[i]=atan2f(SINP,c);

	}
#else
//Phase Shifting Interferometry.nb James C. Wyant (1998) 
//Schwider-Hariharan 5 steps
	for(int i=0;i<SizeX*SizeY;i++)
	{
		float num=2*(I1[i]-I3[i]);
		float denom=(2*I2[i]-I4[i]-I0[i]);
		//RowVector O = ( I1 + 2*I3 + I5) / 4; 
		Phase[i]=atan2(num,denom);
		if(Contraste)
		{
			Contraste[i]=sqrt(num*num+denom*denom);
		}
	}
#endif
	return;
}

void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* I4, float* I5, float* Phase, float* Contraste)
{
	CHECK(I0==0,"DecalageDePhase",return);
	CHECK(I1==0,"DecalageDePhase",return);
	CHECK(I2==0,"DecalageDePhase",return);
	CHECK(I3==0,"DecalageDePhase",return);
	CHECK(I4==0,"DecalageDePhase",return);
	CHECK(I5==0,"DecalageDePhase",return);
	CHECK(Phase==0,"DecalageDePhase",return);
//Phase Shifting Interferometry.nb James C. Wyant (1998) 
//Schwider-Hariharan 6 steps
	for(int i=0;i<SizeX*SizeY;i++)
	{
		float num=-3*I1[i]+4*I3[i]-I5[i];
		float denom=I0[i]-4*I2[i]+3*I4[i];
		Phase[i]=atan2(num,denom);
		if(Contraste)
		{
			Contraste[i]=sqrt(num*num+denom*denom);
		}
	}
	return;
}

void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* I4, float* I5, float* I6, float* Phase, float* Contraste)
{
	CHECK(I0==0,"DecalageDePhase",return);
	CHECK(I1==0,"DecalageDePhase",return);
	CHECK(I2==0,"DecalageDePhase",return);
	CHECK(I3==0,"DecalageDePhase",return);
	CHECK(I4==0,"DecalageDePhase",return);
	CHECK(I5==0,"DecalageDePhase",return);
	CHECK(I6==0,"DecalageDePhase",return);
	CHECK(Phase==0,"DecalageDePhase",return);
#ifdef UseLarkin
//Larkin 7 steps 51.4°
	float MSqrt3=-sqrtf(3.0);
	for(int i=0;i<SizeX*SizeY;i++)
	{
		double num=MSqrt3*(I1[i]+I2[i]-I4[i]-I5[i]);
		double denom=(I0[i]+I1[i]-I2[i]-2*I3[i]-I4[i]+I5[i]+I6[i]);
		Phase[i]=atan2(num,denom);
		if(Contraste)
		{
			Contraste[i]=sqrt(num*num+denom*denom);
		}
	}
#else
//Phase Shifting Interferometry.nb James C. Wyant (1998) 
//Schwider-Hariharan 7 steps 90°
	for(int i=0;i<SizeX*SizeY;i++)
	{
		float num=4*I1[i]-2*I3[i]+6*I5[i];
		float denom=-I0[i]+7*I2[i]-7*I4[i]+I6[i];
		Phase[i]=atan2(num,denom);
		if(Contraste)
		{
			Contraste[i]=sqrt(num*num+denom*denom);
		}
	}
#endif
	/*
	//voir aussi 90°
  N = 3 (I3 - I5) - (I1 - I7)
  D = 4I4 - 2 (I2 + I6)
	*/
	return;
}

#endif

