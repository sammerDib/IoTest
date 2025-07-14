
typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	int Oversampling;
	int FullSizeX;
	int FullSizeY;
	float* restrict P;
	WORD* restrict W;
} SPG_WB;

int SPG_CONV WB_Init(SPG_WB& WB, int SizeX, int SizeY, int Oversampling);
void SPG_CONV WB_Close(SPG_WB& WB);
void SPG_CONV WB_Clear(SPG_WB& WB);
void SPG_CONV WB_Copy(SPG_WB& WB, BYTE* E, int Pitch);
void SPG_CONV WB_Copy(SPG_WB& WB, float* E, int xd, int yd);
void SPG_CONV WB_SmoothNormalize(SPG_WB& WB);
void SPG_CONV WB_BrutNormalize(SPG_WB& WB);
void SPG_CONV WB_Normalize(SPG_WB& WB);
void SPG_CONV WB_Save(SPG_WB& WB, char* SuggestedName);

