
typedef BYTE TBIN;
typedef int TSUM;

#define BINtoSUM(a,b) ((a)&(b))

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	TBIN* D;//profil binaire
} SPG_ProfilBin;

/*
typedef struct
{
	int MaxEdge;//taille totale du tableau des transitions
	int NumEdge;//nombre réel de transitions
	int* Edge;//position X dans l'image de la transition
} SPG_EdgeArray;

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	SPG_EdgeArray EdgeUp;//tableau des transitions UP dans l'image
	SPG_EdgeArray EdgeDn;//tableau des transitions DN dans l'image
} SPG_ProfilBinT;
*/

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	TSUM* D;
	int X;//position de current dans reference donnant le max de corrélation
	int Y;//position de current dans reference donnant le max de corrélation
	TSUM Max;
	TSUM Total;
	float ConfidenceRatio;//nombre entre 0 et 1, bonne corrélation si >0.5
} SPG_BinCorrResult;

typedef struct
{
	int Etat;

	SPG_ProfilBin Reference;
	SPG_ProfilBin Current;
//	SPG_ProfilBinT ReferenceT;
//	SPG_ProfilBinT CurrentT;

	SPG_BinCorrResult R;

} SPG_BinCorr;

typedef struct
{
	int SizeX;
	int SizeY;
	int Pitch1;
	TBIN* D1;
	int Pitch2;
	TBIN* D2;
} SPG_CommonRect;

//fonctions 'externes'
int BC_Init(SPG_BinCorr& BC, TBIN* Reference, int SizeX, int SizeY);
void BC_Close(SPG_BinCorr& BC);
int BC_FindFullPos(SPG_BinCorr& BC, TBIN* Current, int SizeX, int SizeY);
int BC_FindPos(SPG_BinCorr& BC, TBIN* Current, int SizeX, int SizeY, int OffsetX0, int OffsetY0, int OffsetX1, int OffsetY1);

//fonctions 'internes'
//int BC_Compute(SPG_BinCorr& BC, SPG_ProfilBinT ReferenceT, SPG_ProfilBin Current, SPG_BinCorrResult& BR); 

//Flags d'etat
#define BC_NULL 0
#define BC_WithMEM 1
#define BC_WithMem 1
#define BC_Alias 2
#define BC_AliasMem 2
#define BC_MemAlias 2

int SPG_GetCommonRect(SPG_CommonRect& CR, TBIN* D1, int SizeX1, int SizeY1, TBIN* D2, int SizeX2, int SizeY2, int OffsetX, int OffsetY);

TSUM BC_Compute(SPG_ProfilBin& Reference, SPG_ProfilBin& Current, int X, int Y);

int BC_CreateBin(SPG_ProfilBin& P, int SizeX, int SizeY, TBIN* D=0);
void BC_DeleteBin(SPG_ProfilBin& P);

int BC_CreateBinCorrResult(SPG_BinCorrResult& R, int SizeX, int SizeY);
void BC_DeleteBinCorrResult(SPG_BinCorrResult& R);

/*
int BC_CreateBinT(SPG_ProfilBin& PT, int SizeX, int SizeY);
int BC_DeleteBinT(SPG_ProfilBin& PT, int SizeX, int SizeY);
int BC_CreateBinCorrResult(SPG_BinCorrResult& R);
int BC_DeleteBinCorrResult(SPG_BinCorrResult& R);
int BC_Transform(SPG_ProfilBin& Reference, SPG_ProfilBinT& ReferenceT);
int BC_InvTransform(SPG_ProfilBin& Reference, SPG_ProfilBinT& ReferenceT);
*/


