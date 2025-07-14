
#ifdef SPG_General_USESTEREOVIS

typedef struct
{
	float EyeDist;
	int EyeDistPix;
	float EyeConvergence;
	int Mode;
	G_Ecran ELeft;
	G_Ecran ERight;
	SG_PDV* Vue;
} SPG_STEREOVIS;

int SPG_CONV SPG_SV_Init(SPG_STEREOVIS& SV, SG_PDV& Vue, float EyeDist, int EyeDistPix, float EyeConvergence, int Mode);
void SPG_CONV SPG_SV_Close(SPG_STEREOVIS& SV);
void SPG_CONV SPG_SV_Render(SPG_STEREOVIS& SV);
void SPG_CONV SPG_SV_AdaptTextureColor(BYTE* M, int POCT, int Pitch, int SizeX, int SizeY);

#define SPG_SV_HALFSCREEN 1
#define SPG_SV_REDBLUE 2

#endif

