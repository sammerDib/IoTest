
#ifdef SPG_General_USEACC

typedef struct
{
	float X;
	float Y;
	float Ponderation;
} SPG_ACC_VAL;

typedef struct
{
	int NumS;
//	int EmptyPlaces;
	int Pos;
	SPG_ACC_VAL* AV;
	float XMin;
	float XMax;
	float YMin;
	float YMax;
} SPG_ACCUMULATEUR;

#define Acc_Clear(Acc) Acc.Pos=0;

#include "SPG_Accumulateur.agh"

#endif

