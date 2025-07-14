
#define UW_NumAlgo 4

#define UW_AlgoName0 "Simple"
#define UW_AlgoName1 "Goldstein"
#define UW_AlgoName2 "GradientGuided"
#define UW_AlgoName3 "VarianceGuided"

typedef struct
{
//rien
} UNWRAPPING_SIMPLE;
typedef struct
{
//rien
} UNWRAPPING_GOLDSTEIN;
typedef struct
{
//rien
} UNWRAPPING_GRADGUIDED;
typedef struct
{
//rien
} UNWRAPPING_VARGUIDED;

typedef struct
{
	int Algo;
	UNWRAPPING_SIMPLE USimple;
	UNWRAPPING_GOLDSTEIN UGold;
	UNWRAPPING_GRADGUIDED UGradGuided;
	UNWRAPPING_VARGUIDED UVarGuided;
}  UNWRAPPING_PARAMS;

//params normaux: densite 1 bruit 0.6
//genere un profil de hauteur artificiel
void SPG_CONV UW_Generate(Profil& P,float Densite,float Bruit);
//simule le wrapping
void SPG_CONV UW_Wrap(Profil& P);
//deplie le profil inplace
void SPG_CONV UW_InPlaceUnwrap(Profil& P, UNWRAPPING_PARAMS& UWP);
void SPG_CONV UW_Unwrap(Profil& PsrcWrap, Profil& PdstUnwrap, UNWRAPPING_PARAMS& UWP);
void SPG_CONV UW_UnwrapWithQualMap(Profil& PsrcWrap, Profil& PdstUnwrap, Profil& Pqual, UNWRAPPING_PARAMS& UWP);


