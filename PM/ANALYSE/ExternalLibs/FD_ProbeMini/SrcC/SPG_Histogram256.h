
#ifdef SPG_General_USEHIST

#define HIST_BIN_TYPE DWORD
//#define HIST_MAX_CATER 256

typedef struct
{
	int NumCater;
	HIST_BIN_TYPE* Cumul;
} HIST_TYPE;

typedef struct
{
	int NumCaterX;
	int NumCaterY;
	HIST_BIN_TYPE* Cumul;
} HIST2D_TYPE;

typedef struct
{
	int Size;
	float NormeMax;
	float invNormeMax;
	float MaxCount;
	PixCoul Color[DRAWVECTCOLOR];
	float* restrict Count;
} DRAWHIST2D;

typedef struct
{
	int Count[256];
} DRAWHIST1D;

#define HIST_Add(HIST,N) HIST.Cumul[N]++

#define HIST2D_Add(HIST,NX,NY) {CHECK_ELSE(!(V_IsBound(NX,0,H.NumCaterX)&&V_IsBound(NY,0,H.NumCaterY)),"HIST2D_Add",;) else HIST.Cumul[NX+HIST.NumCaterX*NY]++;}
#define HIST2D_AddUnsafe(HIST,NX,NY) HIST.Cumul[NX+HIST.NumCaterX*NY]++

#define HIST_Clear(HIST) memset(HIST.Cumul,0,HIST.NumCater*sizeof(HIST_BIN_TYPE))
#define HIST2D_Clear(HIST) memset(HIST.Cumul,0,HIST.NumCaterX*HIST.NumCaterY*sizeof(HIST_BIN_TYPE))
//#define HIST_Init(HIST) HIST_Clear(HIST)

#include "SPG_Histogram256.agh"

#endif

