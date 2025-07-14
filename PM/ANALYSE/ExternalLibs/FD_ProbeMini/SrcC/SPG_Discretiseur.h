
#ifdef SPG_General_USEDCRZ

#define DCRZ_LEVEL_TYPE LONG
#define DCRZ_CUMUL_TYPE LONG
#define MAX_LEVEL_VALUE 0x0ffff
#define MAX_LEVEL_VALUEd2 0x07fff
#define MIN_LEVEL_SPACING 0x0f
#define MIN_LEVEL_SPACINGx2 0x1f

typedef struct
{
	int NumLevels;
	DCRZ_LEVEL_TYPE* LevelActuel;
	DCRZ_LEVEL_TYPE* LevelFutur;
	DCRZ_CUMUL_TYPE* Cumul;
} DCRZ_TYPE;


#include "SPG_Discretiseur.agh"

#define DCRZ_ResetCumul(DCRZ_VAR) memset(DCRZ_VAR.Cumul,0,DCRZ_VAR.NumLevels*sizeof(DCRZ_CUMUL_TYPE))
#define DCRZ_Add(DCRZ_VAR,RESULTAT) CHECK(V_IsBound(RESULTAT,0,DCRZ_VAR.NumLevels)==0,"DCRZ_Add: Valeur incorrecte",;);DCRZ_VAR.Cumul[RESULTAT]++;

//void DCRZ_Discretise(DCRZ_TYPE& DCRZ_VAR,float FLOAT_ZERO_ONE,int& RESULTAT);// {RESULTAT=0;float DCRZ_D_MACRO=FLOAT_ZERO_ONE; while((RESULTAT<(DCRZ_VAR.NumLevels-1))&&(DCRZ_D_MACRO*MAX_LEVEL_VALUE>DCRZ_VAR.LevelActuel[RESULTAT])) RESULTAT++;}
//#define DCRZ_Discretise(DCRZ_VAR,FLOAT_ZERO_ONE,RESULTAT) {RESULTAT=0;float DCRZ_D_MACRO=FLOAT_ZERO_ONE; while((RESULTAT<(DCRZ_VAR.NumLevels-1))&&(DCRZ_D_MACRO*MAX_LEVEL_VALUE>DCRZ_VAR.LevelActuel[RESULTAT])) RESULTAT++;}

#define DCRZ_Discretize DCRZ_Discretise

#define DCRZ_Synthetise(DCRZ_VAR,LEVEL) (DCRZ_VAR.LevelActuel[LEVEL]+DCRZ_VAR.LevelActuel[LEVEL+1])
#define DCRZ_Synthetize DCRZ_Synthetise
#define DCRZ_GetSynthNorm(DCRZ_VAR) (2*DCRZ_VAR.LevelActuel[DCRZ_VAR.NumLevels])

#endif

