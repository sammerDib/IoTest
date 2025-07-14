
#ifdef SPG_General_USEInterpolateur

typedef struct
{
	int Etat;
	float Longueur;//longueur curviligne

	//droite
	V_VECT PStart;
	V_VECT DirStart;
	V_VECT DirStartNorm;
	V_VECT PStop;
	V_VECT DirStop;
	//arc de cercle
	V_VECT Center;
	V_VECT RayonStart;
	V_VECT RayonNorm;
	float TetaMax;
	/*
	//polynome
	float PolyFact;
	*/
	//spline
	float SPLongueur;//longueur lineaire
	
} InterpolationParameters;

#define IP_LINEAIRE 1
#define IP_CIRCULAIRE 2
#define IP_SPLINE 3


#include "Interpolateur.agh"

#endif


