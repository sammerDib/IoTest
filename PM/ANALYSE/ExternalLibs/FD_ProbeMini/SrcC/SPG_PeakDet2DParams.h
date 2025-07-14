
typedef struct
{
	int Version;
	int SizeX;//1
	int SizeY;//2
	int Rotate;
	int SpatialFilterKernelSize;
	float LowPassTimeCste;
	float PowLow;//6
	float PowHigh;
	float MinSignalLevel;
	int RegulariseKernelSize;
	int WeightFilterMode;
	int WeightFilterKernelSize;
	float WeightFilterAngle;
	int MaxResultPerRow;//12
	int RegularisedRefine;//13
	float Selectivity;
	float Threshold;
	//...
} PEAKDET2DPARAMS;

#define PEAKDET2DPARAMS_VERSION 2

/*

#define PEAKDET2DPARAMSDEF_SizeX				256 //orientation de l'image
#define PEAKDET2DPARAMSDEF_SizeY				256 //orientation de l'image
#define PEAKDET2DPARAMSDEF_Rotate				0 //orientation de l'image
#define PEAKDET2DPARAMSDEF_SpatialFilterKernelSize 15 //taille du filtre spatial 2D
#define PEAKDET2DPARAMSDEF_LowPassTimeCste		0.3 //moyennage temporel entre images 0=pas de moyennage
#define PEAKDET2DPARAMSDEF_PowLow				0.2 //filtrage pour récuperer le fond continu
#define PEAKDET2DPARAMSDEF_PowHigh				5 //filtrage pour recuperer le niveau max du signal
#define PEAKDET2DPARAMSDEF_MinSignalLevel		6 //niveau mini de SignalHigh-NoiseLow à considérer (compense une régularisation insuffisante)
#define PEAKDET2DPARAMSDEF_RegulariseKernelSize 127 //regularisation niveaux high/low
#define PEAKDET2DPARAMSDEF_WeightFilterKernelSize	25 //regularisation ligne
#define PEAKDET2DPARAMSDEF_WeightFilterAngle	0.5 //rad	orientation ligne
#define PEAKDET2DPARAMSDEF_MaxResultPerRow		8 //longueur de la liste de pixels candidats à considérer
#define PEAKDET2DPARAMSDEF_RegularisedRefine	0 //1 ou 0
#define PEAKDET2DPARAMSDEF_Selectivity			100 //parametre de la loi de calcul de distance (PeakDet2D_GetDistance)
#define PEAKDET2DPARAMSDEF_Threshold			0.3 //seuil pour ignorer les points de faible intensité

*/

