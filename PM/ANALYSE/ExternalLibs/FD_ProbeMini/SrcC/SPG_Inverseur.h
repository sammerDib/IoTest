
#ifdef SPG_General_USEINVJ0

typedef struct
{
	float* D;
	int NumP;
	float Xmin;
	float Xmax;
	float Ymin;
	float Ymax;
} SPG_INVERSEUR;


int INVJ0_Init(SPG_INVERSEUR& I, int NumP);
float INVJ0_DichotomicFindX(float YSearch,float Xmin, float Xmax, float Tol);
void INVJ0_Invert(SPG_INVERSEUR& I, float& X, float Y);
void INVJ0_Close(SPG_INVERSEUR& I);

#endif

