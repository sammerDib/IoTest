
typedef struct
{
	int Etat;

	Profil Reference;//image de référence (flottant)

	Profil RTransform;//image de référence transformée (! taille différente de la référence)

	//taille caractéristique des motifs à chercher pour en déduire automatiquement les parametres
	int BumpSize;

	//parametres de SPG_AlignTransform
	float MaxIntensityLevel;//niveau max de la camera (256 en 8bits)
	float Threshold;//seuil de rugosité env.0.05f
	int Downsampling;//sous échantillonnage de la référence
	int AeraSize;//zone d'integration

	//déplacement en pixel permettant d'aligner le profil testé avec la référence
	int PosX;
	int PosY;
	float Weight;

} SPG_Align;

void SPG_CONV SPG_AlignTransform(Profil& Ref, Profil& T, int Downsampling, int AeraSize, float Threshold, float MaxIntensityLevel);
int SPG_CONV SPG_AlignInit(SPG_Align& SA, Profil& Reference, int BumpSize, float Threshold=0.05f, float MaxIntensityLevel=256.0f);
void SPG_CONV SPG_AlignClose(SPG_Align& SA);
void SPG_CONV SPG_AlignEcart(SPG_Align& SA, Profil& PTransform, float& Diff, float& Weight, int XTest, int YTest);
void SPG_CONV SPG_AlignGetPos(SPG_Align& SA, Profil& P, int& PosX, int& PosY, int SearchAera);
