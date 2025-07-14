
#include "SPG_General.h"

#ifdef SPG_General_USECut

#include "SPG_Includes.h"

float SPG_CONV Cut_FindIntersectionInternal(float* D, int NumS, float Seuil, int Sens, int XStart)
{
	CHECK(D==0,"Cut_FindIntersection: Cut invalide",return -1);
	CHECK(!V_IsBound(XStart,0,NumS),"Cut_FindIntersectionInternal",return -1);
	if(Sens>0)//intersection dans le sens montant
	{
		for(int i=XStart;i<NumS-1;i++)
		{
			if((D[i]<=Seuil)&&(D[i+1]>Seuil))
			{
				return i+(Seuil-D[i])/(D[i+1]-D[i]);
			}
		}
	}
	else if(Sens<0)//intersection dans le sens descendant
	{
		for(int i=XStart;i<NumS-1;i++)
		{
			if((D[i]>=Seuil)&&(D[i+1]<Seuil))
			{
				return i+(Seuil-D[i])/(D[i+1]-D[i]);
			}
		}
	}
	return -1;
}

float SPG_CONV Cut_FindIntersection(Cut& C, float Seuil, int Sens, int XStart)
{
	CHECK(C.Etat==0,"Cut_FindIntersection: Cut invalide",return -1);
	return Cut_FindIntersectionInternal(C.D,C.NumS,Seuil,Sens,XStart);
}

int SPG_CONV Cut_FindCentralIntersection(Cut& C, float Seuil, int Sens, float& X0, float& SX)
{//première paire d'intersections
	float Ua=Cut_FindIntersection(C,Seuil,Sens);
	if(Ua<0) return 0;
	float Da=Cut_FindIntersection(C,Seuil,-Sens,Ua);
	if(Da<0) return 0;
	X0=Ua;
	SX=Da-Ua;
	while(1)
	{
		float Ub=Cut_FindIntersection(C,Seuil,Sens,X0+SX);
		if(Ub<0) return -1;
		float Db=Cut_FindIntersection(C,Seuil,-Sens,Ub);
		if(Db<0) return -1;

		if(fabs(Ub+Db-C.NumS)<fabs(2*X0+SX-C.NumS))
		{//il existe une autre paire mieux centrée
			X0=Ub;
			SX=Db-Ub;
		}
		else
		{//la nouvelle paire n'est pas mieux centrée, on a dépassé la paire centrale
			return -1;
		}
	}
}

//cree une coupe par concatenation
void SPG_CONV Cut_Merge2(Cut& CRes,Cut& C1,Cut& C2)
{
	CHECK((C1.Etat&&C2.Etat)==0,"Cut_Merge2: Cut invalide",return);
	Cut_Create(CRes,C1.NumS+C2.NumS,C1.XScale,C1.UnitX,C1.UnitY);
	CHECK(CRes.Etat==0,"Cut_Merge2: Cut echoue",return);
	int i;
	for (i=0;i<C1.NumS;i++)
	{
		CRes.D[i]=C1.D[i];
		if (C1.Msk) CRes.Msk[i]=C1.Msk[i];
		if (C1.Decor) CRes.Decor[i]=C1.Decor[i];
	}
	for (i=0;i<C2.NumS;i++)
	{
		CRes.D[i+C1.NumS]=C2.D[i];
		if (C2.Msk) CRes.Msk[i+C1.NumS]=C2.Msk[i];
		if (C2.Decor) CRes.Decor[i+C1.NumS]=C2.Decor[i];
	}
	return;
}

void SPG_CONV Cut_Merge3(Cut& CRes,Cut& C1,Cut& C2,Cut& C3)
{
	CHECK((C1.Etat&&C2.Etat&&C3.Etat)==0,"Cut_Merge3: Cut invalide",return);
	Cut_Create(CRes,C1.NumS+C2.NumS+C3.NumS,C1.XScale,C1.UnitX,C1.UnitY);
	CHECK(CRes.Etat==0,"Cut_Merge3: Cut echoue",return);

	int j=0;
	int i;
	for (i=0;i<C1.NumS;i++)
	{
		CRes.D[j]=C1.D[i];
		if (C1.Msk) CRes.Msk[j]=C1.Msk[i];
		if (C1.Decor) CRes.Decor[j]=C1.Decor[i];
		j++;
	}
	for (i=0;i<C2.NumS;i++)
	{
		CRes.D[j]=C2.D[i];
		if (C2.Msk) CRes.Msk[j]=C2.Msk[i];
		if (C2.Decor) CRes.Decor[j]=C2.Decor[i];
		j++;
	}
	for (i=0;i<C3.NumS;i++)
	{
		CRes.D[j]=C3.D[i];
		if (C3.Msk) CRes.Msk[j]=C3.Msk[i];
		if (C3.Decor) CRes.Decor[j]=C3.Decor[i];
		j++;
	}
	return;
}

//initialise un sous profil, mode=C_WithMem ou C_Alias
int SPG_CONV Cut_ExtractL(Cut& CRes, Cut& C, int First, int Len, int Mode)
{
	CHECK((Mode!=Cut_WithMEM)&&(Mode!=Cut_Alias),"Cut_ExtractL: Le pointeur d'un sous profil n'est pas alloue a part entiere",return 0);
	CHECK(First>=C.NumS,"Cut_ExtractL",return 0);
	CHECK(Len<=0,"Cut_ExtractL",return 0);
	int R=Cut_Init(CRes,V_Min(Len,C.NumS-First),C.D+First,C.Msk?(C.Msk+First):0,C.Decor?(C.Decor+First):0,Mode);
	Cut_SetScale(CRes,C.XScale,C.UnitX,C.UnitY);
	return R;
}

int SPG_CONV Cut_Extract(Cut& CRes, Cut& C, int First, int Last, int Mode)
{
	return Cut_ExtractL(CRes,C,First,Last-First,Mode);
}

/*
float Cut_Get(Cut& C, int N)
{
	if V_IsBound(N,0,C.NumS)
	{
		return C.D[N];
	}
	else
		return 0;
}
*/

float SPG_CONV Cut_GetAverage(Cut& C)
{
	CHECK(C.Etat==0,"Cut_GetAverage: Cut source nul",return 0);
	CHECK(C.NumS==0,"Cut_GetAverage: Cut source vide (NumS)",return 0);
	CHECK(C.D==0,"Cut_GetAverage: Cut source vide (D)",return 0);
	float Average=0;
	for(int x=0;x<C.NumS;x++)
		Average+=C.D[x];
	Average/=C.NumS;
	return Average;
}

void SPG_CONV Cut_Add(Cut& C, Cut& CToAdd, float CoeffMult)
{
	CHECK(C.Etat==0,"Cut_Add: Cut destination nul",return);
	CHECK(C.D==0,"Cut_Add: Cut destination vide (D)",return);
	CHECK(CToAdd.Etat==0,"Cut_Add: Cut source nul",return);
	CHECK(CToAdd.D==0,"Cut_Add: Cut source vide (D)",return);
	DbgCHECK(C.NumS!=CToAdd.NumS,"Cut_Add: Tailles discordantes");
	for(int x=0;x<V_Min(C.NumS,CToAdd.NumS);x++)
	{
		C.D[x]+=CToAdd.D[x]*CoeffMult;
	}
	return;
}

void SPG_CONV Cut_Substract(Cut& C, Cut& CToSubstract)
{
	CHECK(C.Etat==0,"Cut_Substract: Cut destination nul",return);
	CHECK(C.D==0,"Cut_Substract: Cut destination vide (D)",return);
	CHECK(CToSubstract.Etat==0,"Cut_Substract: Cut source nul",return);
	CHECK(CToSubstract.D==0,"Cut_Substract: Cut source vide (D)",return);
	DbgCHECK(C.NumS!=CToSubstract.NumS,"Cut_Substract: Tailles discordantes");
	for(int x=0;x<V_Min(C.NumS,CToSubstract.NumS);x++)
	{
		C.D[x]-=CToSubstract.D[x];
	}
	return;
}

void SPG_CONV Cut_Divise(Cut& C, Cut& CDiviseur)
{
	CHECK(C.Etat==0,"Cut_Divise: Cut destination nul",return);
	CHECK(C.D==0,"Cut_Divise: Cut destination vide (D)",return);
	CHECK(CDiviseur.Etat==0,"Cut_Divise: Cut source nul",return);
	CHECK(CDiviseur.D==0,"Cut_Divise: Cut source vide (D)",return);
	DbgCHECK(C.NumS!=CDiviseur.NumS,"Cut_Divise: Tailles discordantes");
	for(int x=0;x<V_Min(C.NumS,CDiviseur.NumS);x++)
	{
		if(((C.Msk==0)||(C.Msk[x]==0))&&
			((CDiviseur.Msk==0)||(CDiviseur.Msk[x]==0))&&
			(CDiviseur.D[x]!=0))
		{
			C.D[x]/=CDiviseur.D[x];
		}
		else
		{
			C.D[x]=0;
			if(C.Msk) C.Msk[x]=1;
		}
	}
}

void SPG_CONV Cut_Normalize(Cut& C, Cut& CDiviseur)
{
	CHECK(C.Etat==0,"Cut_Divise: Cut destination nul",return);
	CHECK(C.D==0,"Cut_Divise: Cut destination vide (D)",return);
	CHECK(CDiviseur.Etat==0,"Cut_Divise: Cut source nul",return);
	CHECK(CDiviseur.D==0,"Cut_Divise: Cut source vide (D)",return);
	DbgCHECK(C.NumS!=CDiviseur.NumS,"Cut_Divise: Tailles discordantes");
	for(int x=0;x<V_Min(C.NumS,CDiviseur.NumS);x++)
	{
		if(((C.Msk==0)||(C.Msk[x]==0))&&
			((CDiviseur.Msk==0)||(CDiviseur.Msk[x]==0))&&
			(CDiviseur.D[x]!=0))
		{
			C.D[x]/=CDiviseur.D[x];
			if(C.D[x]>1) C.D[x]=1;
			if(C.D[x]<0) C.D[x]=0;
		}
		else
		{
			C.D[x]=0;
			if(C.Msk) C.Msk[x]=1;
		}
	}
	return;
}

void SPG_CONV Cut_SubR(Cut& C,float Bias)
{
	CHECK(C.Etat==0,"Cut_SubR: Cut source nul",return);
	CHECK(C.D==0,"Cut_SubR: Cut source vide (D)",return);
	for(int x=0;x<C.NumS;x++)
		C.D[x]-=Bias;
	return;
}

void SPG_CONV Cut_Unwrap_0_1(Cut& C)
{
	CHECK(C.Etat==0,"Cut_Unwrap_0_1: Cut source nul",return);
	CHECK(C.D==0,"Cut_Unwrap_0_1: Cut source vide (D)",return);

	float LastZ=C.D[0];
	int StepCount=0;
	for(int i=1;i<C.NumS;i++)
	{
		Cut_UnwrapGeneric(C.D[i],StepCount,LastZ);
	}

	return;
}

void SPG_CONV Cut_Unwrap_0_1_FromCenter(Cut& C)
{
	CHECK(C.Etat==0,"Cut_Unwrap_0_1: Cut source nul",return);
	CHECK(C.D==0,"Cut_Unwrap_0_1: Cut source vide (D)",return);

	float LastZ=C.D[C.NumS>>1];
	int StepCount=0;
	int i;
	for(i=1+(C.NumS>>1);i<C.NumS;i++)
	{
		Cut_UnwrapGeneric(C.D[i],StepCount,LastZ);
	}
	LastZ=C.D[C.NumS>>1];
	StepCount=0;
	for(i=(C.NumS>>1)-1;i>=0;i--)
	{
		Cut_UnwrapGeneric(C.D[i],StepCount,LastZ);
	}

	return;
}

int SPG_CONV Cut_FindMax(Cut& C, float& Max)
{
	CHECK(C.Etat==0,"Cut_FindMax: Cut nul",Max=0;return -1);
	CHECK(C.D==0,"Cut_FindMax: Cut vide",Max=0;return -1);

	int i;
	for(i=0;i<C.NumS;i++)
	{
		if((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
		Max=*C.D;
		break;
		}
	}
	if (i==C.NumS)
	{
		Max=0;
		return -1;
	}

	int MxPos=i;
	for(i++;i<C.NumS;i++)
	{
		if ((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
			if(C.D[i]>Max)
			{
				Max=C.D[i];
				MxPos=i;
			}
		}
	}
	return MxPos;
}

int SPG_CONV Cut_FindMin(Cut& C, float& Min)
{
	CHECK(C.Etat==0,"Cut_FindMin: Cut nul",Min=0;return -1);
	CHECK(C.D==0,"Cut_FindMin: Cut vide",Min=0;return -1);

	int i;
	for(i=0;i<C.NumS;i++)
	{
		if((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
		Min=*C.D;
		break;
		}
	}
	if (i==C.NumS)
	{
		Min=0;
		return -1;
	}

	int MxPos=i;
	for(i++;i<C.NumS;i++)
	{
		if ((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
			if(C.D[i]<Min)
			{
				Min=C.D[i];
				MxPos=i;
			}
		}
	}
	return MxPos;
}

float SPG_CONV Cut_GetMax(Cut& C)
{
	CHECK(C.Etat==0,"Cut_FindMinMax: Cut nul",return 0);
	CHECK(C.D==0,"Cut_FindMax: Cut vide",return 0);

	float Max=0;
	int i;
	for(i=0;i<C.NumS;i++)
	{
		if((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
		Max=*C.D;
		break;
		}
	}
	if (i==C.NumS)
	{
		Max=0;
		return -1;
	}

	for(i++;i<C.NumS;i++)
	{
		if ((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
			if(C.D[i]>Max)
			{
				Max=C.D[i];
			}
		}
	}
	return Max;
}

void SPG_CONV Cut_FindMinMax(Cut& C, float& Min, float& Max)
{
	CHECK(C.Etat==0,"Cut_FindMinMax: Cut nul",Min=Max=0;return);
	CHECK(C.D==0,"Cut_FindMinMax: Cut vide",Min=Max=0;return);
	IF_CD_G_CHECK(14,return);
	int i;
	for(i=0;i<C.NumS;i++)
	{
		if((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
		Min=Max=C.D[i];
		break;
		}
	}
	if (i==C.NumS)
	{
		Min=Max=0;
		return;
	}
	for(i++;i<C.NumS;i++)
	{
		if ((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
		Min=V_Min(Min,C.D[i]);
		Max=V_Max(Max,C.D[i]);
		}
	}
	return;
}

void SPG_CONV Cut_MixAt(Cut& C, int Pos, Cut& Cadd, float Amplitude)
{
	CHECK(C.Etat==0,"Cut_MixAt: Cut nul",return);
	CHECK(C.D==0,"Cut_MixAt: Cut vide",return);
	for(int i=0;i<V_Min(C.NumS-Pos,Cadd.NumS);i++)
	{
		C.D[Pos+i]+=Amplitude*Cadd.D[i];
	}
	return;
}

void SPG_CONV Cut_Convolve(Cut& C, Cut& Filtre)
{
	CHECK(C.Etat==0,"Cut_Convolve: Cut nul",return);
	CHECK(C.D==0,"Cut_Convolve: Cut vide",return);
	CHECK(Filtre.Etat==0,"Cut_Convolve: Cut nul",return);
	CHECK(Filtre.D==0,"Cut_Convolve: Cut vide",return);
	int i;
	for(i=0;i<=C.NumS-Filtre.NumS;i++)
	{
		/*
		float Sum=0;
		for(int n=0;n<Filtre.NumS;n++)
		{
			Sum+=C.D[i+n]*Filtre.D[n];
		}
		C.D[i]=Sum;
		*/
		Cut_ConvolveSample(C,i,Filtre);
	}
	for(;i<C.NumS;i++)
	{
		C.D[i]=0;
	}
	return;
}

#endif

