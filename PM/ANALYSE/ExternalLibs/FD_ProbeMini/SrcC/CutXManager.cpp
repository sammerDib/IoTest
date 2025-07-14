
#include "SPG_General.h"

#ifdef SPG_General_USECut
#ifdef SPG_General_USEFFT

#include "SPG_Includes.h"
#include <memory.h>
extern float PI,DPI;

int SPG_CONV CutX_ExtractL(CutX& CRes, CutX& C, int First, int Len, int Mode)
{
	int R=CutX_Init(CRes,V_Min(Len,C.NumS-First),C.D+First,C.Msk?(C.Msk+First):0,C.Decor?(C.Decor+First):0,Mode);
	CutX_SetScale(CRes,C.XScale,C.UnitX,C.UnitY);
	return R;
}

int SPG_CONV CutX_Extract(CutX& CRes, CutX& C, int First, int Last, int Mode)
{
	return CutX_ExtractL(CRes,C,First,Last-First,Mode);
}

//place un profil float en profil complexe centre
int SPG_CONV Cut_PlaceForFFT(Cut& C, CutX& CX, int ClearX)
{
	CHECK(C.Etat==0,"CutX_GetModule: Cut source nul",return 0);
	CHECK(C.D==0,"CutX_GetModule: Cut source vide",return 0);
	CHECK(CX.NumS<C.NumS,"CutX_GetModule: Cut source trop petit (X)",return 0);
	CHECK(CX.Etat==0,"CutX_GetModule: Cut destination nul",return 0);
	CHECK(CX.D==0,"CutX_GetModule: Cut destination vide",return 0);
	CHECK(CX.Msk==0,"CutX_GetModule: Cut destination sans masque",return 0);

	if (ClearX) 
	{
		memset(CX.D,0,CX.NumS*sizeof(SPG_COMPLEX));
	}
	/*
	else
	{
		for(int x=0;x<CX.NumS;x++)
		{
			CX.D[x].im=0;
		}
	}
	*/
	memset(CX.Msk,1,CX.NumS*sizeof(BYTE));

	float * CStart=C.D+V_Max((C.NumS-CX.NumS)/2,0);
	BYTE * MStart=CX.Msk+V_Max((CX.NumS-C.NumS)/2,0);
	SPG_COMPLEX * CXStart=CX.D+V_Max((CX.NumS-C.NumS)/2,0);
	int xlen=V_Min(CX.NumS,C.NumS);
	for(int x=0;x<xlen;x++)
	{
		(*CXStart).re=*CStart;
		*MStart=0;
		MStart++;
		CStart++;
		CXStart++;
	}
	
	//SFFT_GENERAL(CX.D,CX.NumS);
	//SFFTSWAP

	return -1;
}

int SPG_CONV CutX_FFT(CutX& CX)
{
	IF_CD_G_CHECK(9,return 0);
	CHECK(CX.Etat==0,"CutX_FFT: Coupe invalide",return 0);
	SFFT_GENERAL(CX.D,CX.NumS);
	SFFTSWAP(CX.D,CX.NumS);
	return -1;
}

//astuce pour centrer les signaux au milieu plutot que sur 0
void SPG_CONV CutX_TranslateToCenter_FFT_AlternateSign(CutX& CX)
{
	CHECK(CX.Etat==0,"CutX_FFT: Coupe invalide",return);
	for(int x=0;x<CX.NumS;x+=2)
	{
		CX.D[x].re=-CX.D[x].re;
		CX.D[x].im=-CX.D[x].im;
	}

	return;
}

void SPG_CONV CutX_AutoFiltre(CutX& CFFT,float Strength, float CoeffMult)
{
	CHECK(CFFT.Etat==0,"CutX_GetModule: Cut source nul",return);
	CHECK(CFFT.D==0,"CutX_GetModule: Cut source vide",return);

	float MMax=0;
	float LastModule=CX_Module2(CFFT.D[1]);
	float CurrentModule=CX_Module2(CFFT.D[2]);
	float NextModule=CX_Module2(CFFT.D[3]);
	
#define CUT_MODUL_SOMME .333333333f*(LastModule+CurrentModule+NextModule)

	{
		float M=pow(CUT_MODUL_SOMME,Strength*Strength*Strength*Strength);
		if (MMax<M) MMax=M;
		CFFT.D[1].re*=M;
		CFFT.D[1].im*=M;
	}
	int x;
	for(x=2;x<CFFT.NumS-2;x++)
	{
		float M=pow(CUT_MODUL_SOMME,Strength*Strength*Strength*Strength);
		if (MMax<M) MMax=M;
		CFFT.D[x].re*=M;
		CFFT.D[x].im*=M;
		LastModule=CurrentModule;
		CurrentModule=NextModule;
		NextModule=CX_Module2(CFFT.D[x+2]);
	}
	//x=CFFT.NumS-2
	{
		float M=pow(CUT_MODUL_SOMME,Strength*Strength*Strength*Strength);
		if (MMax<M) MMax=M;
		CFFT.D[x].re*=M;
		CFFT.D[x].im*=M;
		CFFT.D[x+1].re*=M;
		CFFT.D[x+1].im*=M;
	}

#undef CUT_MODUL_SOMME
	/*
	int x;
	for(x=1;x<CFFT.NumS;x++)
	{
		float M=pow(CX_Module2(CFFT.D[x]),Strength*Strength*Strength*Strength);
		if (MMax<M) MMax=M;
		CFFT.D[x].re*=M;
		CFFT.D[x].im*=M;
	}
	*/

	CFFT.D[0].re*=CoeffMult;
	CFFT.D[0].im=0;// *=CoeffMult;
	if(MMax==0) return;
	float invMMax=CoeffMult/MMax;
	for(x=1;x<CFFT.NumS;x++)
	{
		CFFT.D[x].re*=invMMax;
		CFFT.D[x].im*=invMMax;
	}
	return;
}

void SPG_CONV CutX_ExtrapoleToFFT(Cut& C, CutX& CFFT, int NIter, float fMin,float fMax, int NoReplaceLastIter)
{
	CHECK(C.Etat==0,"CutX_ExtrapoleToFFT: Coupe invalide",return);
	Cut_PlaceForFFT(C,CFFT,1);
	for(int n=0;n<NIter;n++)
	{
	CutX_FFT(CFFT);
	CutX_Conjugue(CFFT);
	CutX_MulR(CFFT,1.0f/sqrtf((float)CFFT.NumS));
	float Avancement=((float)(NIter-n))/NIter;
	CD_G_CHECK_EXIT(3,9);
	CutX_BandPassFilter(CFFT,fMin,fMax);
	CutX_AutoFiltre(CFFT,Avancement,1);
	CutX_FFT(CFFT);
	CutX_Conjugue(CFFT);
	CutX_MulR(CFFT,1.0f/sqrtf((float)CFFT.NumS));
	if ((n<NIter-1)||(NoReplaceLastIter==0)) Cut_PlaceForFFT(C,CFFT,0);
	}
	return;
}

void SPG_CONV CutX_Conjugue(CutX& CFFT)
{
	CHECK(CFFT.Etat==0,"CutX_Conjugue: Coupe invalide",return);
	for(int x=0;x<CFFT.NumS;x++)
	{
		CFFT.D[x].im=-CFFT.D[x].im;
	}
	return;
}

void SPG_CONV CutX_ToAnalytique(CutX& CFFT)
{
	CHECK(CFFT.Etat==0,"CutX_ToAnalytique: Coupe invalide",return);
	CFFT.D[0].re=0;
	CFFT.D[0].im=0;
	memset(CFFT.D+(CFFT.NumS>>1),0,((CFFT.NumS+1)>>1)*sizeof(SPG_COMPLEX));
	return;
}

//laisse toujours passer la composante continue
void SPG_CONV CutX_BandPassFilter(CutX& CFFT, float fMin, float fMax)
{
	CHECK(CFFT.Etat==0,"CutX_BandPassFilter: Coupe invalide",return);
	CHECK(fMax>0.5,"CutX_BandPassFilter: fMax>0.5",fMax=0.5);
	int x;
	for(x=1;x<CFFT.NumS*fMin;x++)
	{
		CFFT.D[x].re=0;
		CFFT.D[x].im=0;
	}
	/*
	for(;x<CFFT.NumS*fMax;x++)
	{
		CFFT.D[x].re+=CFFT.D[x].re;
		CFFT.D[x].im+=CFFT.D[x].im;
	}
	*/
	for(x=CFFT.NumS*fMax;x<CFFT.NumS;x++)
	{
		CFFT.D[x].re=0;
		CFFT.D[x].im=0;
	}
	return;
}

int SPG_CONV CutX_GetModule(CutX& CX, Cut& C)
{
	CHECK(CX.Etat==0,"CutX_GetModule: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetModule: Cut source vide",return 0);
	CHECK(C.Etat==0,"CutX_GetModule: Cut destination nul",return 0);
	CHECK(C.D==0,"CutX_GetModule: Cut destination vide",return 0);
	CHECK(C.NumS<CX.NumS,"CutX_GetModule: Cut destination trop petit",return 0);
	IF_CD_G_CHECK(23,return 0);

	for(int x=0;x<C.NumS;x++)
		C.D[x]=CX_Module(CX.D[x]);
	
	return -1;
}

int SPG_CONV CutX_GetRe(CutX& CX, Cut& C)
{
	CHECK(CX.Etat==0,"CutX_GetRe: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetRe: Cut source vide",return 0);
	CHECK(C.Etat==0,"CutX_GetRe: Cut destination nul",return 0);
	CHECK(C.D==0,"CutX_GetRe: Cut destination vide",return 0);
	IF_CD_G_CHECK(17,return 0);

	for(int x=0;x<V_Min(C.NumS,CX.NumS);x++)
		C.D[x]=CX.D[x].re;
	
	return -1;
}

int SPG_CONV CutX_GetIm(CutX& CX, Cut& C)
{
	CHECK(CX.Etat==0,"CutX_GetIm: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetIm: Cut source vide",return 0);
	CHECK(C.Etat==0,"CutX_GetIm: Cut destination nul",return 0);
	CHECK(C.D==0,"CutX_GetIm: Cut destination vide",return 0);
	IF_CD_G_CHECK(25,return 0);

	for(int x=0;x<V_Min(C.NumS,CX.NumS);x++)
		C.D[x]=CX.D[x].im;
	
	return -1;
}

int SPG_CONV CutX_GetArg(CutX& CX, Cut& C)
{
	CHECK(CX.Etat==0,"CutX_GetArg: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetArg: Cut source vide",return 0);
	CHECK(C.Etat==0,"CutX_GetArg: Cut destination nul",return 0);
	CHECK(C.D==0,"CutX_GetArg: Cut destination vide",return 0);
	CHECK(C.NumS<CX.NumS,"CutX_GetArg: Cut destination trop petit",return 0);

	for(int x=0;x<C.NumS;x++)
	{
#define DefSPGArg1 CX.D[x]
#define DefSPGArg2 C.D[x]
#include "SPG_Argum_Inline.cpp"
	}
	
	return -1;
}

int SPG_CONV CutX_GetArg_0_2pi(CutX& CX, Cut& C)
{
	CHECK(CX.Etat==0,"CutX_GetArg: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetArg: Cut source vide",return 0);
	CHECK(C.Etat==0,"CutX_GetArg: Cut destination nul",return 0);
	CHECK(C.D==0,"CutX_GetArg: Cut destination vide",return 0);
	CHECK(C.NumS<CX.NumS,"CutX_GetArg: Cut destination trop petit",return 0);

	for(int x=0;x<C.NumS;x++)
	{
#define DefSPGArg1 CX.D[x]
#define DefSPGArg2 C.D[x]
#include "SPG_Argum_Inline_0_2pi.cpp"
	}
	
	return -1;
}

int SPG_CONV CutX_GetArg_0_1(CutX& CX, Cut& C)
{
	CHECK(CX.Etat==0,"CutX_GetArg: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetArg: Cut source vide",return 0);
	CHECK(C.Etat==0,"CutX_GetArg: Cut destination nul",return 0);
	CHECK(C.D==0,"CutX_GetArg: Cut destination vide",return 0);
	CHECK(C.NumS<CX.NumS,"CutX_GetArg: Cut destination trop petit",return 0);

	for(int x=0;x<C.NumS;x++)
	{
#define DefSPGArg1 CX.D[x]
#define DefSPGArg2 C.D[x]
#include "SPG_Argum_Inline_0_1.cpp"
	}
	
	return -1;
}

int SPG_CONV CutX_MulR(CutX& CX, float Coeff)
{
	CHECK(CX.Etat==0,"CutX_MulR: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_MulR: Cut source vide",return 0);
	for(int x=0;x<CX.NumS;x++)
	{
		CX_MulR(CX.D[x],Coeff);
	}
	return -1;
}

int SPG_CONV CutX_MulCut(CutX& CX, Cut C)
{
	CHECK(CX.Etat==0,"CutX_MulCut: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_MulCut: Cut source vide",return 0);
	CHECK(C.Etat==0,"CutX_MulCut: Cut operande nul",return 0);
	CHECK(C.D==0,"CutX_MulCut: Cut operande vide",return 0);
	CHECK(C.NumS<CX.NumS,"CutX_MulCut: Cut operande trop court",return 0);
	for(int x=0;x<CX.NumS;x++)
	{
		CX_MulR(CX.D[x],C.D[x]);
	}
	return -1;
}

int SPG_CONV CutX_MulCutX(CutX& CX, CutX& CXoper)
{
	CHECK(CX.Etat==0,"CutX_GetModule: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetModule: Cut source vide",return 0);
	CHECK(CXoper.Etat==0,"CutX_GetModule: Cut operandeX nul",return 0);
	CHECK(CXoper.D==0,"CutX_GetModule: Cut operandeX vide",return 0);
	CHECK(CXoper.NumS<CX.NumS,"CutX_MulCut: Cut operande trop court",return 0);
	for(int x=0;x<CX.NumS;x++)
	{
		SPG_COMPLEX operS=CX.D[x];
		CX_Mul(operS,CXoper.D[x],CX.D[x]);
	}
	return -1;
}

int SPG_CONV CutX_MulComplementCutX(CutX& CX, CutX& CXoper)
{
	CHECK(CX.Etat==0,"CutX_GetModule: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetModule: Cut source vide",return 0);
	CHECK(CXoper.Etat==0,"CutX_GetModule: Cut operandeX nul",return 0);
	CHECK(CXoper.D==0,"CutX_GetModule: Cut operandeX vide",return 0);
	CHECK(CXoper.NumS<CX.NumS,"CutX_MulCut: Cut operande trop court",return 0);
	for(int x=0;x<CX.NumS;x++)
	{
		SPG_COMPLEX operS=CX.D[x];
		CX_MulComplement(operS,CXoper.D[x],CX.D[x]);
	}
	return -1;
}

float SPG_CONV CutX_GetNorme(CutX& CX)
{
	CHECK(CX.Etat==0,"CutX_GetModule: Cut source nul",return 0);
	CHECK(CX.D==0,"CutX_GetModule: Cut source vide",return 0);
	float Nrm=0;
	for(int x=0;x<CX.NumS;x++)
	{
		Nrm+=CX_Module2(CX.D[x]);
	}
	return sqrt(Nrm);
}

void SPG_CONV CutX_SubAt(CutX& CX, CutX& Correction, float Coeff, int AtPos)
{
	CHECK(CX.Etat==0,"CutX_GetModule: Cut source nul",return);
	CHECK(CX.D==0,"CutX_GetModule: Cut source vide",return);
	CHECK(Correction.Etat==0,"CutX_GetModule: Cut Correction nul",return);
	CHECK(Correction.D==0,"CutX_GetModule: Cut Correction vide",return);
	CHECK(Correction.NumS<CX.NumS,"CutX_MulCut: Cut Correction trop court",return);

	for(int x=0;x<CX.NumS;x++)
	{
		CX.D[x].re-=Correction.D[AtPos].re*Coeff;
		CX.D[x].im-=Correction.D[AtPos].im*Coeff;
		AtPos++;
		if (AtPos>=Correction.NumS) AtPos=0;
	}
	return;
}

void SPG_CONV CutX_SubXAt(CutX& CX, CutX& Correction, SPG_COMPLEX CoeffX, float Coeff, int AtPos)
{
	CHECK(CX.Etat==0,"CutX_GetModule: Cut source nul",return);
	CHECK(CX.D==0,"CutX_GetModule: Cut source vide",return);
	CHECK(Correction.Etat==0,"CutX_GetModule: Cut Correction nul",return);
	CHECK(Correction.D==0,"CutX_GetModule: Cut Correction vide",return);
	CHECK(Correction.NumS<CX.NumS,"CutX_MulCut: Cut Correction trop court",return);

	SPG_COMPLEX CTotal=CoeffX;
	CX_MulR(CTotal,Coeff);

	while(AtPos<0) AtPos+=Correction.NumS;
	while(AtPos>=Correction.NumS) AtPos-=Correction.NumS;

	for(int x=0;x<CX.NumS;x++)
	{
		SPG_COMPLEX CorrectionX;
		CX_Mul(CTotal,Correction.D[AtPos],CorrectionX);
		CX.D[x].re-=CorrectionX.re;
		CX.D[x].im-=CorrectionX.im;
		AtPos++;
		if (AtPos>=Correction.NumS) AtPos=0;
	}
	return;
}

int SPG_CONV CutX_FindMax(CutX& C)
{
	CHECK(C.Etat==0,"CutX_FindMax: CutX nul",return -1);
	CHECK(C.D==0,"CutX_FindMax: CutX vide",return -1);

	int i;
	int found=0;
	float Max  = 0.0;
	for(i=0;i<C.NumS-1;i++)
	{
		if((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
		Max=CX_Module2(C.D[i]);//Max est initialisé
		found=-1;
		break;
		}
	}
	if (found==0) return -1;//on sort si Max n'est pas initialisé

	int MxPos=i;
	for(i=i;i<C.NumS-1;i++)
	{
		if ((C.Msk==0)||(C.Msk[i]==0))//attention a l'ordre d'evaluation
		{
			if(CX_Module2(C.D[i])>Max)
			{
				Max=CX_Module2(C.D[i]);
				MxPos=i;
			}
		}
	}
	return MxPos;
}

void SPG_CONV CutX_FindMaximumCorrel(CutX& S1,CutX& S2,SPG_COMPLEX& Correl,int& SubPos,float fMin,float fMax)
{
	CHECK(S1.Etat==0,"CutX_FindMaximumCorrel: CutX S1 nul",SubPos=-1;return);
	CHECK(S1.D==0,"CutX_FindMaximumCorrel: CutX S1 vide",SubPos=-1;return);
	CHECK(S2.Etat==0,"CutX_FindMaximumCorrel: CutX S2 nul",SubPos=-1;return);
	CHECK(S2.D==0,"CutX_FindMaximumCorrel: CutX S2 vide",SubPos=-1;return);
	CHECK(S1.NumS!=S2.NumS,"CutX_FindMaximumCorrel: Tailles discordantes",return);

	CutX SX1;
	CutX SX2;
	CutX_Duplicate(S1,SX1);
	CutX_Duplicate(S2,SX2);
	CutX_FFT(SX1);
	CutX_FFT(SX2);
	CutX_MulComplementCutX(SX1,SX2);
	SX1.D[0].re=0;
	SX1.D[0].im=0;
	CutX_BandPassFilter(SX1,fMin,fMax);
	CutX_FFT(SX1);
	CHECK((SubPos=CutX_FindMax(SX1))==-1,"CutX_FindMaximumCorrel: Recherche echouee",return);
	/*
	Correl.re=SX1.D[SubPos].re;
	Correl.im=SX1.D[SubPos].im;
	*/
	Correl=SX1.D[SubPos];
	CutX_Close(SX1);
	CutX_Close(SX2);
	return;
}

#endif

#endif
