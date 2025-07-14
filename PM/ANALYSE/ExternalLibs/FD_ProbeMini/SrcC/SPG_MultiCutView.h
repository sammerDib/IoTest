
#define MCV_CURSORS 2

typedef struct
{
	DWORD Color;
	Cut C;
	BYTE Draw;
	float CursorPos[MCV_CURSORS];
} MCV_Cut;

typedef struct
{
	int Etat;
	int NumChannel;
	MCV_Cut* MultiCut;
	Cut** MultiCutSectionsPtr;
	Cut* MultiCutSections;
	DWORD* MultiCutColors;
	int MaxMesure;
	int NrMesure;
	int DisplaySize;
	int DisplayOrigin;
} SPG_MCV;

int SPG_CONV SPG_MCV_Init(SPG_MCV& MCV, int NumChannel, int MaxMesure);
void SPG_CONV SPG_MCV_Close(SPG_MCV& MCV);
void SPG_CONV SPG_MCV_Add(SPG_MCV& MCV, int Channel, float Value);
void SPG_CONV SPG_MCV_Update(SPG_MCV& MCV);
void SPG_CONV SPG_MCV_Draw(SPG_MCV& MCV, G_Ecran& E, C_Lib& CL);
void SPG_CONV SPG_MCV_Zoom(SPG_MCV& MCV, int Delta);
void SPG_CONV SPG_MCV_Scroll(SPG_MCV& MCV, int Delta);
void SPG_CONV SPG_MCV_SetCursor(SPG_MCV& MCV, int Channel, float Position);
