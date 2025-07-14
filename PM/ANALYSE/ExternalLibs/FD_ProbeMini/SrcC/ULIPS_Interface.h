
#ifdef SPG_General_USEULIPSINTERFACE

typedef struct
{
	int Etat;
	ULIPS_List* L;
	G_Ecran E;
	C_Lib CL;
	B_Lib BL;
	
	SPG_NET_ADDR EditNetAddr;
	int IP0;
	int IP1;
	int IP2;
	int IP3;
	int Port;

	int bIP0;
	int bIP1;
	int bIP2;
	int bIP3;
	int bQuery;
	int bQueryList;
	int bDeleteMe;
	int bBusy;
	int bPort;

} ULIPS_Interface;

int SPG_CONV ULIPS_InitInterface(ULIPS_Interface& UI, ULIPS_List& L, G_Ecran& E);
void SPG_CONV ULIPS_UpdateInterface(ULIPS_Interface& UI);
void SPG_CONV ULIPS_CloseInterface(ULIPS_Interface& UI);
int SPG_CONV ULIPS_PrintElement(ULIPS_List& L, int ListEntry, char* Element);
void SPG_CONV ULIPS_PrintList(ULIPS_Interface& UI, char* List);

#endif

