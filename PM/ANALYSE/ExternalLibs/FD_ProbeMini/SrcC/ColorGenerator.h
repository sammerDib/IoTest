
#ifdef SPG_General_USEColorGenerator

typedef struct
{
	PixCoul Moyenne;
	PixCoul Correl;
	PixCoul Aleat;
} ColorG;

#include "ColorGenerator.agh"

#define CG_RANDPM256 (rand()&511)-256
#define CG_COMPOSE(PXCOUL,COLG,EXT) {SHORT CGRC_A2=(SHORT)(CG_RANDPM256);int CGRC_Composante=COLG.Moyenne.EXT+((COLG.Correl.EXT*CGRC_A1)>>8)+((COLG.Aleat.EXT*CGRC_A2)>>8);PXCOUL.EXT=(BYTE)V_Sature(CGRC_Composante,0,255);}
#define CG_RandColor24(PXCOUL, COLG) {SHORT CGRC_A1=(SHORT)(CG_RANDPM256);CG_COMPOSE(PXCOUL,COLG,R);CG_COMPOSE(PXCOUL,COLG,V);CG_COMPOSE(PXCOUL,COLG,B);}
#define CG_RandColor(PXCOUL, COLG) CG_RandColor24(PXCOUL, COLG);PXCOUL.A=G_NORMAL_ALPHA
#define CG_Rand_RandColorG(COLORG,RANDMoy,RANDCorr,RANDAleat) CG_RandColor(COLORG.Moyenne,RANDMoy);CG_RandColor(COLORG.Correl,RANDCorr);CG_RandColor(COLORG.Aleat,RANDAleat)
#define CG_Declare_Rand_RandColorG(COLORG,RANDMoy,RANDCorr,RANDAleat) ColorG COLORG;CG_RandColor(COLORG.Moyenne,RANDMoy);CG_RandColor(COLORG.Correl,RANDCorr);CG_RandColor(COLORG.Aleat,RANDAleat)


#endif

