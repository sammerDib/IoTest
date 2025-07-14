#include "SPG_SpecMacroHeader.h"
//SPG_SpecType=type d'element
//SPG_SpecBlockSize nombre d'element par allocation
//SPG_SpecMaxBlock maximum d'allocations
#define T_SPECLISTBLOC SPG_SpecName(SPECLISTBLOC,SPG_SpecType)
#define T_SPECLIST SPG_SpecName(SPECLIST,SPG_SpecType)

int SPG_CONV SPG_SpecName(SpecListCreate,SPG_SpecType)(T_SPECLIST& L)
{
	memset(&L,0,sizeof(T_SPECLIST));
	return L.Etat=-1;
}

void SPG_CONV SPG_SpecName(SpecListClose,SPG_SpecType)(T_SPECLIST& L)
{
	CHECK(L.Etat!=-1,"SpecListClose: Liste invalide",return);
	for(int i=0;i<((L.Count+SPG_SpecMaxBlock-1)/SPG_SpecBlockSize);i++)
	{
		SPG_MemFree(L.Bloc[i]);
	}
	memset(&L,0,sizeof(T_SPECLIST));
	return;
}

SPG_SpecType* SPG_CONV SPG_SpecName(SpecListAdd,SPG_SpecType)(T_SPECLIST& L)
{
	CHECK(L.Etat!=-1,"SpecListAdd: Liste invalide",return 0);

	if((L.Count%SPG_SpecBlockSize)==0)
	{
		if(L.Count/SPG_SpecBlockSize<SPG_SpecMaxBlock)
		{
			L.Bloc[L.Count/SPG_SpecBlockSize]=SPG_TypeAlloc(1,T_SPECLISTBLOC,"SpecListBlock");
		}
		else
		{
			DbgCHECK(1,"SpecListAdd: Liste remplie");
			return 0;
		}
	}
	SPG_SpecType* Element = L.Bloc[L.Count/SPG_SpecBlockSize]->Element+L.Count%SPG_SpecBlockSize;
	L.Count++;
	return Element;
}

SPG_SpecType* SPG_CONV SPG_SpecName(SpecListGet,SPG_SpecType)(T_SPECLIST& L, int Nr)
{
	CHECK(L.Etat!=-1,"SpecListGet: Liste invalide",return 0);
	CHECK(!V_IsBound(Nr,0,L.Count),"Numero invalide",return 0);
	return L.Bloc[Nr/SPG_SpecBlockSize]->Element+Nr%SPG_SpecBlockSize;
}

#undef T_SPECLISTBLOC
#undef T_SPECLIST
#undef SPG_SpecBlockSize
#undef SPG_SpecMaxBlock
#include "SPG_SpecMacroEnder.h"
