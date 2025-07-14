
#ifdef SPG_General_USEMECA

typedef struct
{
	V_VECT pos;//globale
	V_VECT Vitesse;//globale
	V_VECT Force;//globale
} MECA_Point;

typedef struct
{
	int NombreP;
	MECA_Point* inP;
	MECA_Point* outP;
	V_VECT* Point;

	int inDiscreteInteraction;

	SG_VECT BRef;
	float Rayon;

	V_REPERE Rep;
	V_VECT Translation;
	V_VECT Rotation;
	float Masse;
	float Moment;
	float Gravite;
} MECA_Bloc;

#define MECA_CONV SPG_CONV
#define MECA_FASTCONV SPG_FASTCONV

#include "SPG_Meca.agh"

#endif

