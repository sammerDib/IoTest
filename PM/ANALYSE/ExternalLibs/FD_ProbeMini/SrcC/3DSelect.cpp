
#include "SPG_General.h"

#ifdef SPG_General_USE3DSelect


#include "SPG_Includes.h"

//abs
#include <stdlib.h>


//recupere le numero d'un point par XECR et YECR
int SPG_CONV S_GetPoint(SG_Bloc& B, int Xmouse,int Ymouse,int Radius)
{
	CHECK((B.MemFaces==0),"S_GetPoint: Bloc vide",return -1);
	CHECK((B.MemPoints==0),"S_GetPoint: Bloc vide",return -1);

	int Dist=Radius;
	int PNumber=-1;
	int x;
	for(x=0;x<B.NombreP;x++)
	{
		if((abs(B.MemPoints[x].XECR-Xmouse)+abs(B.MemPoints[x].YECR-Ymouse))<Dist)
		{/*
			if (PNumber!=-1)
			{
				if (B.MemPoints[PNumber].Prof>B.MemPoints[x].Prof)
				{
					PNumber=x;
					Dist=abs(B.MemPoints[x].XECR-Xmouse)+abs(B.MemPoints[x].YECR-Ymouse);
				}
			}
			else
			{*/
			PNumber=x;
			Dist=abs(B.MemPoints[x].XECR-Xmouse)+abs(B.MemPoints[x].YECR-Ymouse);
			//}
		}
	}
	return PNumber;
}

#endif
