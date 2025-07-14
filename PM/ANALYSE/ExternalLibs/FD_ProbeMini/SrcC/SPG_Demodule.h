

#define DEMENVMAX 1
#define DEMLIVEDISP 2

typedef struct
{
	int Flag;

	int CurNumS;//echantillons en entrée
	int CurProcessed;//echantillons en sortie
	Cut Source;//source data : concatenation de tous les echantillons depuis le depart

	CutX* CAP;//demodulation data (amplitude/phase)
	CutX ALY;//signal analytique (partie imaginaire reconstruite) (provisoire)

	int f0;//frequence centrale de la demodulation
	int df;//largeur de la bande centrale de gain unitaire autour de la frequence centrale
	Cut weightF;//largeur de la transition vers le gain nul

	//la longueur totale d'un intervalle traité (FFT:fftSegNumS) se decompose en trois: weightT.NumS(ponderation avec l'intervalle precedent) + NumSegS + weightT.NumS (ponderation avec l'inttervalle suivant)
	int NumSegS;
	int fftSegNumS;
	Cut weightT;//largeur de la zone de recouvrement entre intervalles
	CutX Seg;//intervalle démodulé

	//Live display
	SPG_Window W;
	G_Ecran E_Src;
	G_Ecran E_Env;
	G_Ecran E_Phase;
	C_Lib CL;
	Cut Src;
	Cut Res;
	Cut SrcW; //fenetre de ponderation sur l'intervalle traité (constant)
	Cut EnvMax;
	Cut Env;
	Cut EnvW; //fenetre de ponderation en frequence (constant)
	Cut Phase;
} SPG_DEMODULEFFT;

typedef struct
{
	int Flag;

	int CurNumS;
	int CurProcessed;

	Cut Source;//source data

	CutX* CAP;//demodulation data (amplitude/phase)

	DECALAGEDEPHASE DEC;
} SPG_DEMODULEDECPHASE;

int SPG_CONV DemInit(SPG_DEMODULEFFT& D, CutX* CAP, int MaxNumS, int SegNumS, int weightTNumS, int weightFNumS, int f0, int df, int Flag, char* WName);
int SPG_CONV DemClose(SPG_DEMODULEFFT& D);

void SPG_CONV DemGetweightT(SPG_DEMODULEFFT& D, Cut& C);
void SPG_CONV DemGetweightF(SPG_DEMODULEFFT& D, Cut& C);

void SPG_CONV DemOpenSignal(SPG_DEMODULEFFT& D);
//void SPG_CONV DemClear(SPG_DEMODULEFFT& D);
void SPG_CONV DemUpdateSignal(SPG_DEMODULEFFT& D, short int* Signal, int Pitch, int NumS);
void SPG_CONV DemCloseSignal(SPG_DEMODULEFFT& D);


int SPG_CONV DemInit(SPG_DEMODULEDECPHASE& D, CutX* CAP, int MaxNumS, int DecalageDePhaseID);
int SPG_CONV DemClose(SPG_DEMODULEDECPHASE& D);

void SPG_CONV DemOpenSignal(SPG_DEMODULEDECPHASE& D);
//void SPG_CONV DemClear(SPG_DEMODULELARKIN& D);
void SPG_CONV DemUpdateSignal(SPG_DEMODULEDECPHASE& D, short int* Signal, int Pitch, int NumS);
void SPG_CONV DemCloseSignal(SPG_DEMODULEDECPHASE& D);

