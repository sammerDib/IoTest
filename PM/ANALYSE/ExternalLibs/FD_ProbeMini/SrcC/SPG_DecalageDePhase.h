

typedef enum
{
	INVALID=0,
	TRI_120,
	CARRE4_90,
	LARKIN5_90,
	HARIHARAN5_90,
	HARIHARAN6_90,
	LARKIN7_514,
	HARIHARAN7_90,
} DECALAGEDEPHASEID;

#define STRDECALAGEDEPHASEID 	"1:TRI_120 2:CARRE4_90 3:LARKIN5_90 4:HARIHARAN5_90 5:HARIHARAN6_90 6:LARKIN7_51.4 7:HARIHARAN7_90"


struct DECALAGEDEPHASE;

typedef void SPG_CONV DECALAGEDEPHASEFCT(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);

typedef struct DECALAGEDEPHASE
{
	DECALAGEDEPHASEID id;
	char* Name;
	int NumSteps;
	int PhaseSteps;
	float PhaseOfSin;
	float PhaseOfCos;
	float AmplitudeNormalization;
	float Signe;
	float Sqrt3;
	DECALAGEDEPHASEFCT* F;
} DECALAGEDEPHASE;

int SPG_CONV SPG_DecalageDePhaseInit(DECALAGEDEPHASE& DP, DECALAGEDEPHASEID id);
void SPG_CONV SPG_DecalageDePhaseClose(DECALAGEDEPHASE& DP);
void SPG_CONV SPG_DecalageDePhase_Process(DECALAGEDEPHASE& DP, int N, float* Phase, float* Amplitude, int rPitch, float* I, int iStep, int iPitch);











int SPG_CONV SPG_DecalageDePhase_StepPerLambda(int INum);
void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* Phase, float* Contraste);
void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* Phase, float* Contraste);
void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* I4, float* Phase, float* Contraste);
void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* I4, float* I5, float* Phase, float* Contraste);
void SPG_CONV SPG_DecalageDePhase(int SizeX, int SizeY, float* I0, float* I1, float* I2, float* I3, float* I4, float* I5, float* I6, float* Phase, float* Contraste);

#define SPG_SquareDiffMacro(A,B) (A-B)*(A-B)
#define SPG_Phase120(I0,I1,I2,Phase) Phase=atan2(Sqrt3*(I1-I2),(2*(I0)-I1-I2))
#define SPG_Contraste120(I0,I1,I2,Contraste) float LOCAL_MACRO_IMoy=(I0+I1+I2)*.333333333f;Contraste=sqrtf(SPG_SquareDiffMacro(I0,LOCAL_MACRO_IMoy)+SPG_SquareDiffMacro(I1,LOCAL_MACRO_IMoy)+SPG_SquareDiffMacro(I2,LOCAL_MACRO_IMoy))
#define SPG_DecalageDePhase120(I0,I1,I2,Phase,Contraste) {SPG_Phase120(I0,I1,I2,Phase);SPG_Contraste120(I0,I1,I2,Contraste);}
