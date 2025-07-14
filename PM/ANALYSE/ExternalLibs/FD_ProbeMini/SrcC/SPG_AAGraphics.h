
#ifdef SPG_General_USEGEFFECT

typedef struct
{
	G_Ecran E;
	PixCoul BG;//background color
	int K;
	int Blur;
} ANTIALIASECRAN;

int SPG_CONV AG_Create(ANTIALIASECRAN& AG, G_Ecran E, DWORD BG, int K, int Blur=0);
int SPG_CONV AG_Close(ANTIALIASECRAN& AG);
int SPG_CONV AG_BlitEcran(ANTIALIASECRAN& AG, G_Ecran E);

typedef struct
{
	int Etat;
	Profil P;
	Profil R[2];
} LIGHTECRAN;

int SPG_CONV AL_Init(LIGHTECRAN& AG, int SizeX, int SizeY);
void SPG_CONV AL_Close(LIGHTECRAN& AG);
int SPG_CONV AL_Process(LIGHTECRAN& AG, G_Ecran& ESrc, G_Ecran& EDest);

#endif

