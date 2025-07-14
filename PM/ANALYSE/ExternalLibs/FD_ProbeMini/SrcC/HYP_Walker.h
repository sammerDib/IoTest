
#ifdef SPG_General_USEHYP

#define HYP_SIZE_MAX 16

typedef struct
{
	float Start;
	float Step;
	int Size;
} HYP_VAR;

typedef struct
{
	int Etat;
	HYP_VAR HV[HYP_SIZE_MAX];
	int NumDim;
	int Size;//produit des Steps
} HYP_SPACE;

typedef struct
{
	int Position[HYP_SIZE_MAX];
	int Increment[HYP_SIZE_MAX];
	float Valeur[HYP_SIZE_MAX];
	float RandomPart;
} HYP_SPACE_POS;

#define HYP_RAND_01 (rand()&1)
#define HYP_RAND_03 (rand()&3)
#define HYP_RAND_07 (rand()&7)
//AxisSize==0 donne une variable constante (Step==0)
//AxisSize==1 donne une composante aleatoire (Step<>0) sauf DimInit0
#define HYP_DimInitStatic(HV,Amp) {HV.Start=Amp;HV.Step=0;HV.Size=1;} 
//intervalle [-Amp +Amp]
#define HYP_DimInitSym(HV,Amp,AxisSize) if(AxisSize==0) {HYP_DimInitStatic(HV,Amp);} else {HV.Start=-Amp;HV.Step=2*Amp/(V_Min(AxisSize,2)-1);HV.Size=AxisSize;};
//intervalle [0 Amp]
#define HYP_DimInit0(HV,Amp,AxisSize) if(AxisSize<=1) {HYP_DimInitStatic(HV,Amp);} else {HV.Start=0;HV.Step=Amp/(AxisSize-1);HV.Size=AxisSize;};
//intervalle ]0 Amp]
#define HYP_DimInitNZ(HV,Amp,AxisSize) if(AxisSize==0) {HYP_DimInitStatic(HV,Amp);} else {HV.Start=HV.Step=Amp/AxisSize;HV.Size=AxisSize;};
//intervalle [AmpMin AmpMax]
#define HYP_DimInitI(HV,AmpMin,AmpMax,AxisSize) if(AxisSize==0) {HYP_DimInitStatic(HV,0.5*(AmpMin+AmpMax));} else  if(AxisSize==1) {HV.Start=0.5*(AmpMin+AmpMax);HV.Step=(AmpMax-AmpMin);HV.Size=1;} else {HV.Start=AmpMin;HV.Step=(AmpMax-AmpMin)/(AxisSize-1);HV.Size=AxisSize;};
//intervalle [Amp/Var Amp*Var]
#define HYP_DimInitRel(HV,Amp,Var,AxisSize) HYP_DimInitI(HV,Amp/Var,Amp*Var,AxisSize);

#define HYP_WalkerClose(HP) {DbgCHECK(HP.Etat!=-1,"");memset(&HP,0,sizeof(HYP_SPACE));}

#include "HYP_Walker.agh"

#endif

