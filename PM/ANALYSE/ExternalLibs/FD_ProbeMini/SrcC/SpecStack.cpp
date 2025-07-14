#include "SPG_SpecMacroHeader.h"
//SPG_SpecType=type d'element
//SPG_SpecBlockSize nombre d'element par allocation
//SPG_SpecMaxBlock maximum d'allocations
#define T_SPECSTACK SPG_SpecName(SPECSTACK,SPG_SpecType)

int SPG_SpecName(SpecStackInit,SPG_SpecType)(T_SPECSTACK& S)
{
	memset(&S,0,sizeof(T_SPECSTACK));
	return S.Etat=-1;
}

void SPG_SpecName(SpecStackClose,SPG_SpecType)(T_SPECSTACK& S)
{
	CHECK(S.Etat!=-1,"SpecStackClose: Stack invalide",return);
	memset(&S,0,sizeof(T_SPECSTACK));
	return;
}

int SPG_SpecName(SpecStackPush,SPG_SpecType)(T_SPECSTACK& S, SPG_SpecType& E)
{
	CHECK(S.Etat!=-1,"SpecStackPush: Stack invalide",return 0);

	CHECK(S.Count>=SPG_SpecMaxBlock,"SpecStackPush: Stack overflow",return 0);

	memcpy(S.Stack+S.Count,&E,sizeof(SPG_SpecType));

	return ++S.Count;
}

SPG_SpecType* SPG_SpecName(SpecStackPop,SPG_SpecType)(T_SPECSTACK& S)
{
	CHECK(S.Etat!=-1,"SpecStackPop: Stack invalide",return 0);

	CHECK(S.Count<=0,"SpecStackPop: Stack overflow",return 0);

	return S.Stack+(--S.Count);
}

int SPG_SpecName(SpecStackPop,SPG_SpecType)(T_SPECSTACK& S, SPG_SpecType& E)
{
	CHECK(S.Etat!=-1,"SpecStackPop: Stack invalide",return 0);

	CHECK(S.Count<=0,"SpecStackPop: Stack overflow",return 0);

	memcpy(&E,S.Stack+(--S.Count),sizeof(SPG_SpecType));

	return S.Count;
}

/*
SPG_SpecType* SPG_SpecName(SpecStack,SPG_SpecType)(T_SPECSTACK& S)
{
	CHECK(S.Etat!=-1,"SpecStackPop: Stack invalide",return 0);

	CHECK(S.Count<=0,"SpecStackPop: Stack overflow",return 0);

	return S.Stack+S.Count-1;
}

int SPG_SpecName(SpecStack,SPG_SpecType)(T_SPECSTACK& S, SPG_SpecType& E)
{
	CHECK(S.Etat!=-1,"SpecStackPop: Stack invalide",return 0);

	CHECK(S.Count<=0,"SpecStackPop: Stack overflow",return 0);

	memcpy(E,S.Stack+S.Count-1,sizeof(SPG_SpecType));

	return S.Count-11;
}
*/

#undef T_SPECSTACK
#include "SPG_SpecMacroEnder.h"
