
#ifdef SPG_General_USECarac

typedef struct
{
	int Etat;

	int SizeX;
	int SizeY;
	int SpaceY;

	BYTE CharMin;
	BYTE CharMax;

	PixCoul BackGroundColor;

	G_Ecran EIMG;//bitmap correspondant aux etats des bouton,
	//dans le BON FORMAT D'ECRAN
} C_Lib;

#define XLEFT 0
#define XRIGHT 1
#define XCENTER 2
#define YUP 0
#define YDN 4
#define YCENTER 8
#define FONT_TRANSP 16

#include "SPG_Carac.agh"

#endif
