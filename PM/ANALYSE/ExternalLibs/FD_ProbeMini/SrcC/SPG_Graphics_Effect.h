#ifdef SPG_General_USEGraphics
#ifdef SPG_General_USEGEFFECT

#define G_MB_MaxBlur 8

typedef struct
{
	int Etat;
	G_Ecran EMegaPitch;
	G_Ecran EDraw;
	int NumBlur;
	int CurrentDraw;
	G_Ecran* EDest;
} G_MotionBlur;

#define G_MB_Ecran(MB) MB.EDraw

int SPG_CONV G_MB_Init(G_MotionBlur& MB, G_Ecran& EDest, int NumBlur);
void SPG_CONV G_MB_Close(G_MotionBlur& MB);
void SPG_CONV G_MB_Render(G_MotionBlur& MB);
void SPG_CONV G_MB_RenderFrom(G_MotionBlur& MB, G_Ecran& E);

#endif
#endif

