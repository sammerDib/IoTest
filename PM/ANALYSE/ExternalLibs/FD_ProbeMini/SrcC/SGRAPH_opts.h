
#ifdef SPG_General_USESGRAPH_OPTS

typedef struct
{
	B_Lib BL;
	C_Lib CL;
	G_Ecran Ecran;
	SG_PDV* Vue;

	int AutoUpdate;

	int ForceFilaire;
#ifdef SGE_EMC
	int ForceUni;
	/*
	int TriBlocs;
	int NoTriFaces;
	*/
	int NoClearSky;
	/*
	int UseGdi;
	*/
#endif

	int NombreB;
	int TriFait;

	int DistMax;
	int DistTex;
	int DistFog;

} SG_OPTS;

#include "SGRAPH_opts.agh"

#endif
