
#ifdef SPG_General_USEPaint

#define SPG_PAINT_AERAS 256

//#define SPG_PAINT_DEBUG

typedef struct
{
	int Etat;
	BYTE EqvAeras[SPG_PAINT_AERAS];//liens de connexité entre les numeros de masques, remplacé par les numeros de regions ensuite
	int CurrentIndice;//prochain indice de région libre
	int Eqv;//index de base de région connexe
#ifdef SPG_PAINT_DEBUG
	C_Lib CL;
	SPG_Console C;
	G_Ecran E;
#endif
} SPG_PAINT;

int SPG_PaintInit(SPG_PAINT& P);
void SPG_CONV SPG_PaintClose(SPG_PAINT& P);
void SPG_CONV SPG_PaintFlushByte(SPG_PAINT& P, BYTE* Regions, int Imax);
int SPG_CONV SPG_PaintGetNextIndice(SPG_PAINT& P, BYTE* Regions, int Imax);
int SPG_CONV SPG_PaintSetEqv(SPG_PAINT& P, BYTE CP, BYTE OP);
int SPG_CONV SPG_PaintGetFirstEqv(SPG_PAINT& P);
int SPG_CONV SPG_PaintGetNextEqv(SPG_PAINT& P);
int SPG_CONV SPG_PaintFlushEqvTable(SPG_PAINT& P);
void SPG_CONV SPG_PaintRestartEqvTable(SPG_PAINT& P);
int SPG_CONV SPG_PaintProfil(Profil& P, float Threshold);

#endif
