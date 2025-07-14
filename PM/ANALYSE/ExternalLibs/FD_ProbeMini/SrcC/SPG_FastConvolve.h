

typedef short RS_SAMPLE;
//typedef float RS_SAMPLE; //inutile et plus lent

//comparer les perfos
//typedef int RS_SUM;
typedef float RS_SUM; //plus rapide en release, mais int suffirait


typedef unsigned char RB_SAMPLE;
typedef int RB_SUM;

typedef struct
{
	RS_SAMPLE* __restrict Ring;
	int x;
	int FilterLen;
	
	float invFilterLen;

	//float dwinvFilterLen;

	//int Delay;//delai additionnel
	int RingMsk;
	int RingLen;
	RS_SUM Sum;
} RSFASTCONVOLVE;

int SPG_CONV SPG_FastConvInit(RSFASTCONVOLVE& F, int FilterLen);
void SPG_CONV SPG_FastConvReset(RSFASTCONVOLVE& F);
void SPG_CONV SPG_FastConvClose(RSFASTCONVOLVE& F);

#define SPG_FastConvFiltIn(F,SampleIn) { F.Sum -= F.Ring[F.RingMsk&(F.x-F.FilterLen)]; F.Sum += F.Ring[F.RingMsk&F.x++] = SampleIn;} //SampleIn,Out : RS_SAMPLE, Ring : RS_SUM (car peut dépasser [-32768,+32767]
#define SPG_FastConvzIn(F, SampleIn) { F.Ring[F.RingMsk&F.x++] = SampleIn; }

//Differentes implementations de la normalisation
//efine SPG_FastConvFiltOut(F) F.Sum/F.FilterLen
#define SPG_FastConvFiltOut(G) G.Sum*G.invFilterLen	//G.Sum/G.FilterLen //la division flottante par multiplication est plus rapide
#define SPG_FastConvzOut(G) G.Ring[(G.x-G.FilterLen)&G.RingMsk]
#define SPG_FastConvFilt(G,SampleG) {SPG_FastConvFiltIn(G,SampleG);SampleG=SPG_FastConvFiltOut(G);}
#define SPG_FastConvz(G,SampleG) {SPG_FastConvzIn(G,SampleG);SampleG=SPG_FastConvzOut(G);}

typedef struct
{
	RB_SAMPLE* __restrict Ring;
	int x;
	int FilterLen;
	
	float invFilterLen;

	//float dwinvFilterLen;

	//int Delay;//delai additionnel
	int RingMsk;
	int RingLen;
	//RB_SUM Sum;
} RBFASTCONVOLVE;

int SPG_CONV SPG_FastConvInit(RBFASTCONVOLVE& F, int FilterLen);
void SPG_CONV SPG_FastConvReset(RBFASTCONVOLVE& F);
void SPG_CONV SPG_FastConvClose(RBFASTCONVOLVE& F);

typedef struct
{
	RBFASTCONVOLVE FBZ;
	RSFASTCONVOLVE FL0;
	RSFASTCONVOLVE FH0;
	RSFASTCONVOLVE FZ;
	RSFASTCONVOLVE FL1;
	RSFASTCONVOLVE FH1;
	RSFASTCONVOLVE FLZ;
	RSFASTCONVOLVE FHZ;
} FASTCONVOLVE_CHANNEL_FILT;

FASTCONVOLVE_CHANNEL_FILT* SPG_CONV CF_Init(int NumChannels, double FrequencyHz, float* fMin, float* fMax);
int SPG_CONV CF_Check(FASTCONVOLVE_CHANNEL_FILT& CF); //zero -> ok 1-> une anomalie sur la somme
void SPG_CONV CF_Reset(int NumChannels, FASTCONVOLVE_CHANNEL_FILT* CF);
void SPG_CONV CF_Close(int NumChannels, FASTCONVOLVE_CHANNEL_FILT* &CF);
void SPG_CONV CF_Process(int NumChannels, FASTCONVOLVE_CHANNEL_FILT* CF, short* DataInOutPureDelay, short* DataOutFilt, int NumS);
void SPG_CONV CF_Process(int NumChannels, FASTCONVOLVE_CHANNEL_FILT* CF, short* DataInOutPureDelay, short* DataOutFilt, BYTE* BDataInOutPureDelay, int NumS);
//void SPG_CONV CF_ProcessShowFiltered(int NumChannels, RSFASTCONVOLVE_CHANNEL_FILT* CF, RS_SAMPLE* DataInOutPureDelay, RS_SAMPLE* DataOutFilt, int NumS);

#ifdef SPG_General_USECONNEXION
int SPG_CONV BackupMe(char* decl, RSFASTCONVOLVE& F, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, RBFASTCONVOLVE& F, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, FASTCONVOLVE_CHANNEL_FILT& F, SCX_CONNEXION* C);
#endif
