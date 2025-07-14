
#define CUTVIEWCURSORS 2 //au moins deux, min et max
#define CUTVIEWMAXCUT 16
#define CUTVIEWLABEL 32
#define CUTVIEWEXTENSION 4
#define CUTVIEWEXTENSIONSTR ".pgc"

typedef struct
{
	int DrawStart;
	int DrawLen;
	Cut CDraw[CUTVIEWMAXCUT];//alias

	float X;
	float Z;
} CUTVIEWPARAMS;

typedef struct
{
	Cut C;
	float CursPosX[CUTVIEWCURSORS];
	DWORD CursPosD[CUTVIEWCURSORS];
	int bSelected;
	char Label[CUTVIEWLABEL+CUTVIEWEXTENSION];
} CUTVIEWDATA;

typedef struct
{
	int Etat;
	G_Ecran ECurve;
	G_Ecran EControl;//scale (par cut ou max), active curve, curve name, load/unload
	G_Ecran EScroll;
	B_Lib* BL;
	C_Lib* CL;
	int MultiScale;

	int bHScroll; //position X

	int bExit;
	int bLoad;
	int bSave;
	int bUnload;

	int bVScroll; //zoom

	int a;//active curve
	int n;//total number of curves
	CUTVIEWDATA CD[CUTVIEWMAXCUT];
	CUTVIEWPARAMS CP;
} CUTVIEW;

int SPG_CONV CView_Init(CUTVIEW& CW, G_Ecran& E, B_Lib* BL, C_Lib* CL, int PosX, int PosY, int SizeX, int SizeY, int MultiScale);
void SPG_CONV CView_Load(CUTVIEW& CW, Cut& C, const char* Label);
#define CView_pLoad(CW,C) CView_Load(CW,C,#C);

void SPG_CONV CView_Unload(CUTVIEW& CW, Cut& C);
void SPG_CONV CView_Close(CUTVIEW& CW);
int SPG_CONV CView_Update(CUTVIEW& CW, int MouseX, int MouseY, int MouseLeft, bool bForced=false);

