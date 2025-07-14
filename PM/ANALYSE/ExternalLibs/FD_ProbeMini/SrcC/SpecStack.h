#include "SPG_SpecMacroHeader.h"
//SPG_SpecType=type d'element
//SPG_SpecMaxBlock maximum d'allocations
//SPG_SpecName est dans System\SystemH\SPG_SpecMacro.h
#define T_SPECSTACK SPG_SpecName(SPECSTACK,SPG_SpecType)

typedef struct
{
	int Etat;
	int Count;
	SPG_SpecType Stack[SPG_SpecMaxBlock];	
} T_SPECSTACK;

int SPG_SpecName(SpecStackInit,SPG_SpecType)(T_SPECSTACK& S);
void SPG_SpecName(SpecStackClose,SPG_SpecType)(T_SPECSTACK& S);
int SPG_SpecName(SpecStackPush,SPG_SpecType)(T_SPECSTACK& S, SPG_SpecType& E);
int SPG_SpecName(SpecStackPop,SPG_SpecType)(T_SPECSTACK& S, SPG_SpecType& E);
SPG_SpecType* SPG_SpecName(SpecStackPop,SPG_SpecType)(T_SPECSTACK& S);
/*
int SPG_SpecName(SpecStack,SPG_SpecType)(T_SPECSTACK& S, SPG_SpecType& E);
SPG_SpecType* SPG_SpecName(SpecStack,SPG_SpecType)(T_SPECSTACK& S);
*/
//#define SPG_SpecName(SpecStackCur,SPG_SpecType)	S.Stack[S.Count-1]

#undef T_SPECSTACK
#include "SPG_SpecMacroEnder.h"
