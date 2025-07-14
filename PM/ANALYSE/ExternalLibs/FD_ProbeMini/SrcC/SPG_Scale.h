
#define SCALE_MAX_TICK 64

typedef struct
{
	double Value;
	char Label[32];
} SPG_ScaleTick;

typedef struct
{
	//int Etat;
	int Divisions;
	double TimeCste;
	int ExtraDecimal;

	double fMin;
	double fMax;
	
	double ScaleMin;
	double ScaleMax;
	int NumTick;
	SPG_ScaleTick T[SCALE_MAX_TICK];
} SPG_Scale;

void SPG_CONV SC_Init(SPG_Scale& SC, int Divisions=10, int ExtraDecimal=0, double TimeCste=1.0f);
void SPG_CONV SC_Set(SPG_Scale& SC, double fMin, double fMax);
void SPG_CONV SC_Merge(SPG_Scale& SC, double fMin, double fMax);
void SPG_CONV SC_Update(SPG_Scale& SC, double fMin, double fMax);

