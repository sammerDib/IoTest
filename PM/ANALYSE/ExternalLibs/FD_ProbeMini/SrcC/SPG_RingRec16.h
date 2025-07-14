
#ifdef SPG_General_USERINGREC16

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	int SizeP;
	int NumS;
	WORD* D;
} RING_REC16;

#include "SPG_RingRec16.agh"

#define RGR16_Element(RGR,x,y,n) RGR.D[x+RGR.SizeX*y+RGR.SizeP*n]
#define RGR16_ElementAddr(RGR,x,y,n) RGR.D+x+RGR.SizeX*y+RGR.SizeP*n
#define RGR16_Plane(RGR,n) (RGR.D+RGR.SizeP*(n))

#define RGR_OK 1

#endif

