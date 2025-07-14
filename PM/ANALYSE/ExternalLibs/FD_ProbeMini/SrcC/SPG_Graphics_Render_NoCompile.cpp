
#include "SPG_General.h"

#ifdef SPG_General_USEGraphics

#include <string.h>

#include "SPG_Includes.h"

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawUniPoly4_32")
void SPG_CONV G_DrawUniPoly4_32(const G_Ecran& E, const G_PixCoord P[4], const PixCoul Coul)
{
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawPoly: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPoly: G_Ecran inaccessible",return);
#endif
	
#define NPoints 4
#define NextPoint(x) (x+1)&3
#define PrevPoint(x) (x-1)&3
#define ToPoint(x,n) (x+n)&3
	
#include "InlinePrepareSegment.cpp"

#define G_FASTCOPY
#undef DrawInclusive
#define DEF_G_POCT 4
#define DEF_G_RENDERMODE 1
#undef DEF_G_TEXTURE



{
//position du bord gauche
	BRES_Y BXG;
//position du bord droit
	BRES_Y BXD;
	
//point courant du polygone
	int CurGPoint=0;
	int CurDPoint=0;
//depart et arrivee (verticalement) des segments des bords droit et gauche
	int ygstop=G_TruePixel(P[PntG[CurGPoint]].y);
	int ydstop=G_TruePixel(P[PntD[CurDPoint]].y);
	
	{
		DWORD* restrict MECR=E.MECR+G_TruePixel(P[NYMin].y)*E.Pitch;
		for(int y=G_TruePixel(P[NYMin].y);y<G_TruePixel(P[NYMax].y);y++)
		{
			//si on arrive sur un nouveau segment
			while (ygstop<=y)
			{
				BRES_SET(BXG,P[PntG[CurGPoint]].x);
				BRES_NEWADDTO(BXG,P[PntG[CurGPoint+1]].y-P[PntG[CurGPoint]].y,P[PntG[CurGPoint+1]].x-P[PntG[CurGPoint]].x);
				CurGPoint++;
				ygstop=G_TruePixel(P[PntG[CurGPoint]].y);
			}
			while (ydstop<=y)
			{
				BRES_SET(BXD,P[PntD[CurDPoint]].x);
				BRES_NEWADDTO(BXD,P[PntD[CurDPoint+1]].y-P[PntD[CurDPoint]].y,P[PntD[CurDPoint+1]].x-P[PntD[CurDPoint]].x);
				CurDPoint++;
				ydstop=G_TruePixel(P[PntD[CurDPoint]].y);
			}
				

				const int xStart=G_TruePixel(BRES_GET(BXG));
				const int xStop=G_TruePixel(BRES_GET(BXD));
				for(int x=xStart;x<xStop;x++)
				{
//				{
//#define DEF_G_RENDERCOUL Coul
MECR[x]=Coul.Coul;
//if(x&1) break;
//#include "InlineCombineColor.cpp"
//				}
				}

#undef G_FASTCOPY
			BRES_NEXT(BXG);
			BRES_NEXT(BXD);
			(*(int*)&MECR)+=E.Pitch;
		}
	}
}



	return;
}

#endif

