
#ifdef SPG_General_USECut
#ifdef SPG_General_USEProfil
#ifdef SPG_General_USEGraphics

#define MaxSel 128

typedef struct
{
	int Etat;
	Profil * P;
	int NSel;
	int PosX[MaxSel];
	int PosY[MaxSel];
} SelectionProfile;

typedef struct
{
	int Etat;
	ProfilHeader * PH;
	int PosX;
	int PosY;
	int SizeX;
	int SizeY;
} ZoneProfile;

#include "SelectionProfile.agh"


#define SP_NumSelPoints(SP) SP.NSel

#define ZP_SEL_CORNER_MSK 0xf0
#define ZP_SEL_CORNER_0 0x10
#define ZP_SEL_CORNER_1 0x20
#define ZP_SEL_CORNER_2 0x40
#define ZP_SEL_CORNER_3 0x80

#define ZP_HasFocus(ZP) (ZP.Etat&ZP_SEL_CORNER_MSK)
#define ZP_PosX(ZP) ZP.PosX
#define ZP_PosY(ZP) ZP.PosY
#define ZP_Xmin(ZP) ZP.PosX
#define ZP_Ymin(ZP) ZP.PosY
#define ZP_Xmax(ZP) (ZP.PosX+ZP.SizeX-1)
#define ZP_Ymax(ZP) (ZP.PosY+ZP.SizeY-1)
#define ZP_SizeX(ZP) ZP.SizeX
#define ZP_SizeY(ZP) ZP.SizeY
#define ZP_CornerCoordX0(ZP) ZP_Xmin(ZP)
#define ZP_CornerCoordY0(ZP) ZP_Ymin(ZP)
#define ZP_CornerCoord0(ZP) ZP_CornerCoordX0(ZP),ZP_CornerCoordY0(ZP)

#define ZP_CornerCoordX1(ZP) ZP_Xmax(ZP)
#define ZP_CornerCoordY1(ZP) ZP_Ymin(ZP)
#define ZP_CornerCoord1(ZP) ZP_CornerCoordX1(ZP),ZP_CornerCoordY1(ZP)

#define ZP_CornerCoordX2(ZP) ZP_Xmax(ZP)
#define ZP_CornerCoordY2(ZP) ZP_Ymax(ZP)
#define ZP_CornerCoord2(ZP) ZP_CornerCoordX2(ZP),ZP_CornerCoordY2(ZP)

#define ZP_CornerCoordX3(ZP) ZP_Xmin(ZP)
#define ZP_CornerCoordY3(ZP) ZP_Ymax(ZP)
#define ZP_CornerCoord3(ZP) ZP_CornerCoordX3(ZP),ZP_CornerCoordY3(ZP)

#endif
#endif
#endif

