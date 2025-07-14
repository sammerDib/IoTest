
#include "SPG_General.h"

#ifdef SPG_General_USEHYP

#include "V_General.h"
#include "SPG_List.h"
#include "HYP_Walker.h"
#include "stdlib.h"
#include "string.h"

int SPG_CONV HYP_WalkerInit(HYP_SPACE& HP)
{
	memset(&HP,0,sizeof(HYP_SPACE));
	HP.Size=1;
	HP.NumDim=0;
	return HP.Etat=-1;
}

int SPG_CONV HYP_WalkerReInit(HYP_SPACE& HP)
{
	CHECK(HP.Etat==0,"HYP_WalkerReInit",return 0);
	HP.Size=1;
	for(int i=0;i<HYP_SIZE_MAX;i++)
	{
		if(HP.HV[i].Size)
			HP.Size*=HP.HV[i].Size;
		else
			break;
	}
	HP.NumDim=i;

	return HP.Etat;
}

void SPG_CONV HYP_WalkerComputeVal(HYP_SPACE& HP, HYP_SPACE_POS& HSP)
{
	CHECK(HP.Etat!=-1,"HYP_WalkerSetIncrement: Hyperespace non initialise",return);
	for(int i=0;i<HP.NumDim;i++)
	{
		HSP.Valeur[i]=HP.HV[i].Start+HP.HV[i].Step*(HSP.Position[i]+HSP.RandomPart*(rand()/(float)RAND_MAX-0.5f));
	}
}

int SPG_CONV HYP_WalkerSetIncrement(HYP_SPACE& HP, HYP_SPACE_POS& HSP, int Elements, float RandomPart)
{
	CHECK(HP.Etat!=-1,"HYP_WalkerSetIncrement: Hyperespace non initialise",return 0);
	CHECK(Elements==0,"HYP_WalkerSetIncrement: 0 Elements demandes",return 0);
	CHECK(Elements>HP.Size,"HYP_WalkerSetIncrement: Trop d'elements",Elements=HP.Size);

	int Step=HP.Size/Elements;
	HSP.RandomPart=RandomPart;//0<1
	//int StartPos=Step/2+(Step/2)*(HSP.RandomPart*rand())/(float)RAND_MAX;//ajouter un rand
	int StartPos=(Elements-Elements*(HP.Size/Elements))*rand()/RAND_MAX;

	for(int i=0;i<HP.NumDim;i++)
	{
		HSP.Position[i]=StartPos%HP.HV[i].Size;
		StartPos/=HP.HV[i].Size;
		HSP.Increment[i]=Step%HP.HV[i].Size;
		Step/=HP.HV[i].Size;
	}
	HYP_WalkerComputeVal(HP, HSP);
	return Elements*(HP.Size/Elements);
}

int SPG_CONV HYP_WalkerIncremente(HYP_SPACE& HP, HYP_SPACE_POS& HSP)
{
	CHECK(HP.Etat!=-1,"HYP_WalkerSetIncrement: Hyperespace non initialise",return 0);

	int Retenue=0;
	for(int i=0;i<HP.NumDim;i++)
	{
		HSP.Position[i]+=HSP.Increment[i]+Retenue;
		Retenue=0;
		while(HSP.Position[i]>=HP.HV[i].Size)
		{
			HSP.Position[i]-=HP.HV[i].Size;
			Retenue++;

		}
	}
	HYP_WalkerComputeVal(HP, HSP);
	return (Retenue==0);//on n'est pas au bout tant qu'il n'y a pas de retenue
}

#endif

