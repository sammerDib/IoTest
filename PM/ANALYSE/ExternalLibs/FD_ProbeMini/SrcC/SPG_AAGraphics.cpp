 
#include "SPG_General.h"

#ifdef SPG_General_USEGEFFECT

#include "SPG_Includes.h"

#include <memory.h>

int SPG_CONV AG_Create(ANTIALIASECRAN& AG, G_Ecran E, DWORD BG, int K, int Blur)
{
	AG.Blur=Blur;
	AG.K=K;
	AG.BG.Coul=BG;
	G_InitMemoryEcran(AG.E,4,K*G_SizeX(E)+K-1,K*G_SizeY(E)+K-1);
	return -1;
}

int SPG_CONV AG_Close(ANTIALIASECRAN& AG)
{
	G_CloseEcran(AG.E);
	return -1;
}

static int SPG_CONV AG_BlitEcranA(ANTIALIASECRAN& AG, G_Ecran E)
{
	BYTE* EL=G_MECR(E);
	BYTE* AGL=G_MECR(AG.E);
	for(int y=0;y<G_SizeY(E);y++)
	{
		for(int x=0;x<G_SizeX(E);x++)
		{
			BYTE* XL=EL+x*G_POCT(E);
			PixCoul* XGL=(PixCoul*)(AGL+(AG.K*x)*G_POCT(AG.E));

			BYTE B=0;
			BYTE F=0;
			short SumR=0;
			short SumV=0;
			short SumB=0;

			for(int iy=0;iy<2*AG.K-1;iy++)
			{
				for(int ix=0;ix<2*AG.K-1;ix++)
				{
					if((XGL[ix].Coul&0xFFFFFF)==AG.BG.Coul)
					{
						B++;
					}
					else
					{
						SumB+=XGL[ix].B;
						SumV+=XGL[ix].V;
						SumR+=XGL[ix].R;
						F++;
					}
				}
				*(BYTE**)&XGL+=G_Pitch(AG.E);
			}
			if(F==0)
			{
				*(DWORD*)XL=AG.BG.Coul;
			}
			else
			{
				BYTE D=B+2*AG.K*F;
				*XL++=(2*AG.K*SumB+B*AG.BG.B)/D;
				*XL++=(2*AG.K*SumV+B*AG.BG.V)/D;
				*XL=(2*AG.K*SumR+B*AG.BG.R)/D;
			}
		}
		EL+=G_Pitch(E);
		AGL+=AG.K*G_Pitch(AG.E);
	}
	return -1;
}

static int SPG_CONV AG_BlitEcran2B(ANTIALIASECRAN& AG, G_Ecran E)
{
	BYTE* EL=G_MECR(E);
	BYTE* AGL=G_MECR(AG.E);
	for(int y=0;y<G_SizeY(E);y++)
	{
		for(int x=0;x<G_SizeX(E);x++)
		{
			BYTE* XL=EL+x*G_POCT(E);
			PixCoul* XGL=(PixCoul*)(AGL+(2*x)*G_POCT(AG.E));

			BYTE B=0;
			BYTE F=0;
			short SumR=0;
			short SumV=0;
			short SumB=0;

			for(int iy=0;iy<3;iy++)
			{
				for(int ix=0;ix<3;ix++)
				{
					short A=2;
					if( ( (ix==0)||(ix==2) )&&( (iy==0)||(iy==2))) A=1;
					if( (ix==1)&&(iy==1) ) A=3;
					if(((XGL[ix].Coul^AG.BG.Coul)&0xFFFFFF)==0)
					{
						B+=A;
					}
					else
					{
						SumB+=A*XGL[ix].B;
						SumV+=A*XGL[ix].V;
						SumR+=A*XGL[ix].R;
						F+=A;
					}
				}
				*(BYTE**)&XGL+=G_Pitch(AG.E);
			}
			if(F==0)
			{
				*(DWORD*)XL=((*(DWORD*)XL&0xFEFEFE)+(AG.BG.Coul&0xFEFEFE))>>1;
			}
			else
			{
#define W 4
				BYTE D=B+W*F;
				if( (((*(DWORD*)XL)^AG.BG.Coul)&0xFCFCFC)==0 )
				{
					XL[0]=(W*SumB+B*AG.BG.B)/D;
					XL[1]=(W*SumV+B*AG.BG.V)/D;
					XL[2]=(W*SumR+B*AG.BG.R)/D;
				}
				else
				{
					XL[0]=(XL[0]>>1)+(W*SumB+B*AG.BG.B)/(2*D);
					XL[1]=(XL[1]>>1)+(W*SumV+B*AG.BG.V)/(2*D);
					XL[2]=(XL[2]>>1)+(W*SumR+B*AG.BG.R)/(2*D);
				}
#undef W
			}
		}
		EL+=G_Pitch(E);
		AGL+=2*G_Pitch(AG.E);
	}
	return -1;
}

static int SPG_CONV AG_BlitEcran2(ANTIALIASECRAN& AG, G_Ecran E)
{
	BYTE* EL=G_MECR(E);
	BYTE* AGL=G_MECR(AG.E);
	for(int y=0;y<G_SizeY(E);y++)
	{
		for(int x=0;x<G_SizeX(E);x++)
		{
			BYTE* XL=EL+x*G_POCT(E);
			PixCoul* XGL=(PixCoul*)(AGL+(2*x)*G_POCT(AG.E));

			BYTE B=0;
			BYTE F=0;
			short SumR=0;
			short SumV=0;
			short SumB=0;

			for(int iy=0;iy<3;iy++)
			{
				for(int ix=0;ix<3;ix++)
				{
					short A=2;
					if( ( (ix==0)||(ix==2) )&&( (iy==0)||(iy==2))) A=1;
					if( (ix==1)&&(iy==1) ) A=3;
					if(((XGL[ix].Coul^AG.BG.Coul)&0xFFFFFF)==0)
					{
						B+=A;
					}
					else
					{
						SumB+=A*XGL[ix].B;
						SumV+=A*XGL[ix].V;
						SumR+=A*XGL[ix].R;
						F+=A;
					}
				}
				*(BYTE**)&XGL+=G_Pitch(AG.E);
			}
			if(F==0)
			{
				*(DWORD*)XL=AG.BG.Coul;
			}
			else
			{
#define W 4
				BYTE D=B+W*F;
				XL[0]=(W*SumB+B*AG.BG.B)/D;
				XL[1]=(W*SumV+B*AG.BG.V)/D;
				XL[2]=(W*SumR+B*AG.BG.R)/D;
#undef W
			}
		}
		EL+=G_Pitch(E);
		AGL+=2*G_Pitch(AG.E);
	}
	return -1;
}

int SPG_CONV AG_BlitEcran(ANTIALIASECRAN& AG, G_Ecran E)
{
	if(AG.K==2) 
		if(AG.Blur)
		{
			return AG_BlitEcran2B(AG,E); 
		}
		else
		{
			return AG_BlitEcran2(AG,E); 
		}
	else return AG_BlitEcranA(AG,E);
}


int SPG_CONV AL_Init(LIGHTECRAN& AG, int SizeX, int SizeY)
{
	SPG_ZeroStruct(AG);
	P_Create(AG.P,SizeX,SizeY);
	P_Create(AG.R[0],SizeX,SizeY);
	P_Create(AG.R[1],SizeX,SizeY);
	return AG.Etat=-1;
}

void SPG_CONV AL_Close(LIGHTECRAN& AG)
{
	P_Close(AG.P);
	P_Close(AG.R[0]);
	P_Close(AG.R[1]);
	SPG_ZeroStruct(AG);
	return;
}

int SPG_CONV AL_Process(LIGHTECRAN& AG, G_Ecran& ESrc, G_Ecran& EDest)
{
	CHECK(G_Etat(ESrc)==0,"AL_Process",return 0);
	CHECK(G_Etat(EDest)==0,"AL_Process",return 0);
	{
		float* p=P_Data(AG.P);
		BYTE* M=G_MECR(ESrc);
		for(int y=0;y<P_SizeY(AG.P);y++)
		{
			for(int x=0;x<P_SizeX(AG.P);x++)
			{
				int sum=(int)M[x*G_POCT(ESrc)]+(int)M[x*G_POCT(ESrc)+1]+(int)M[x*G_POCT(ESrc)+2];
				*p++=sum;
			}

			M+=G_Pitch(ESrc);
		}
	}

	P_RemoveOffset(AG.P);
	P_Clear(AG.R[0]);
	P_Clear(AG.R[1]);

	int N=4;
	int n=0;
	for(int i=0;i<N;i++)
	{{
		int sD=3+5*(N-1-i);
		int sF=sD*sD+1;
		//P_FastConvLowPass(R[n],sD);

		float* D=P_Data(AG.R[n])+P_SizeX(AG.R[n]);
		float* V=P_Data(AG.P)+P_SizeX(AG.P);
		for(int y=1;y<P_SizeY(AG.R[n])-1;y++)
		{
			for(int x=1;x<P_SizeX(AG.R[n])-1;x++)
			{
				D[x] += sF*V[x];
			}
			D+=P_SizeX(AG.R[n]);
			V+=P_SizeX(AG.P);
		}

		P_FastConvLowPass(AG.R[n],sD);
	}}

	float G=0.05/(N*N*N*256);//0.000004;
	float L=0.012/(N*N*N*256);
	int PX,PY;
	if(L>0) P_FindMax(AG.R[n],PX,PY); else P_FindMin(AG.R[n],PX,PY);
	float M=P_Element(AG.R[n],PX,PY);

	{
		float* D=P_Data(AG.R[n])+P_SizeX(AG.R[n]);
		for(int y=1;y<P_SizeY(AG.R[n])-1;y++)
		{
			BYTE* BS=PixEcrPTR(ESrc,0,y);
			BYTE* BD=PixEcrPTR(EDest,0,y);
			float* upD=D-1-P_SizeX(AG.R[n]);
			float* dnD=D+1+P_SizeX(AG.R[n]);
			for(int x=1;x<P_SizeX(AG.R[n])-1;x++)
			{
				float F=G*(upD[x]-dnD[x])+L*(D[x]-M);
				BD[0]=V_Sature(BS[0]*(1+F),0,255);
				BD[1]=V_Sature(BS[1]*(1+F),0,255);
				BD[2]=V_Sature(BS[2]*(1+F),0,255);
				BS+=G_POCT(ESrc);
				BD+=G_POCT(EDest);
			}
			D+=P_SizeX(AG.R[n]);
		}
	}

	return -1;
}










#endif
