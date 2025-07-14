#include "SPG_SpecMacroHeader.h"
//SPG_SpecType=type d'element
//SPG_SpecBlockSize nombre d'element par allocation
//SPG_SpecMaxBlock maximum d'allocations
#define T_SPECLISTBLOC SPG_SpecName(SPECLISTBLOC,SPG_SpecType)
#define T_SPECLIST SPG_SpecName(SPECLIST,SPG_SpecType)

typedef struct
{
	SPG_SpecType Element[SPG_SpecBlockSize];
} T_SPECLISTBLOC;

typedef struct
{
	int Etat;
	int Count;
	SPG_SpecName(SPECLISTBLOC,SPG_SpecType)* Bloc[SPG_SpecMaxBlock];	
} T_SPECLIST;

int SPG_CONV SPG_SpecName(SpecListCreate,SPG_SpecType)(T_SPECLIST& L);
void SPG_CONV SPG_SpecName(SpecListClose,SPG_SpecType)(T_SPECLIST& L);
SPG_SpecType* SPG_CONV SPG_SpecName(SpecListAdd,SPG_SpecType)(T_SPECLIST& L);
SPG_SpecType* SPG_CONV SPG_SpecName(SpecListGet,SPG_SpecType)(T_SPECLIST& L, int Nr);

#undef T_SPECLISTBLOC
#undef T_SPECLIST
#undef SPG_SpecBlockSize
#undef SPG_SpecMaxBlock
#include "SPG_SpecMacroEnder.h"
