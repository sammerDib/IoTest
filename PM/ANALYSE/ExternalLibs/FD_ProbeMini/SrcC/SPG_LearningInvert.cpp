
#include "..\SrcC\SPG.h"

#ifdef SPG_General_USENEWMATLIB

#include "SPG_LearningSet.h"


int SPG_CONV LearningSet_Invert(LEARNINGSTATE& L)
{
	CHECK(L.szInputSet<=0,"LearningSet_Invert",return 0);
	CHECK(L.szOutputSet<=0,"LearningSet_Invert",return 0);
	//CHECK(L.NumSet<L.szOutputSet,"LearningSet_Invert",return 0);
	CHECK(L.NumSet<L.szInputSet,"LearningSet_Invert",return 0);

	Matrix Y(L.szOutputSet,L.NumSet);
	Matrix X(L.szInputSet,L.NumSet);
	//Y = MX

	for(int f=0;f<L.NumSet;f++)
	{
		{for(int n=0;n<L.szInputSet;n++)
		{
			X(1+n,1+f)=L.S[f].Input[n];
		}}
		{for(int n=0;n<L.szOutputSet;n++)
		{
			Y(1+n,1+f)=L.S[f].Output[n];
		}}
	}

	//Y=MX
	//Yt = Xt.Mt
	//X.Yt = X.Xt.Mt
	//(X.Xt)-1 X.Yt = Mt

	Matrix Mt = (X * X.t()).i() * X * Y.t();

	float* M=L.M;
	for(int o=0;o<L.szOutputSet;o++)
	{
		for(int i=0;i<L.szInputSet;i++)
		{
			M[i]=Mt(1+i,1+o);
		}
		M+=L.szInputSet;
	}

	return -1;
}

#endif
