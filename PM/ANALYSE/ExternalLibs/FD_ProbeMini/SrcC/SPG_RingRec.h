
#ifdef SPG_General_USERINGREC

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	int SizeP;
	int NumS;
	BYTE* D;
} RING_REC;

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	int SizeP;
	int NumS;
	BYTE* D;
	WORD* StartIndex;
} M3D_RGR;

#include "SPG_RingRec.agh"

#define RGR_Element(RGR,x,y,n) RGR.D[x+RGR.SizeX*y+RGR.SizeP*n]
#define RGR_ElementAddr(RGR,x,y,n) RGR.D+x+RGR.SizeX*y+RGR.SizeP*n
#define RGR_Plane(RGR,n) (RGR.D+RGR.SizeP*(n))

#define RGR_OK 1

#endif

