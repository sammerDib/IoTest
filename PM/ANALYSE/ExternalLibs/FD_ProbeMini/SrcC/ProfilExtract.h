
#ifdef SPG_General_USEPrXt

typedef struct
{
	int Etat;

	G_Ecran EPro;//ecran principal
	G_Ecran EBut;//boutons
	G_Ecran ECut;//coupe,en bas
	
	B_Lib BL;
	C_Lib CL;

	//int BSPro;
	int BSCut;
	int BDZ;
	int WaitMouseRelease;

	Profil* P;//donnees source
	SelectionProfile SP;
	Cut C;
	float DeltaZ;

	float PosX;
	float PosY;
	int iPosX;
	int iPosY;

} ProfilExtract;

#define PrXt_OK 1

#include "ProfilExtract.agh"

#endif
