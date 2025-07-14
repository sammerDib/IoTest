
#include "SPG_General.h"

#ifdef SPG_General_USEGlobal
#ifdef SPG_General_USEGEFFECT

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <string.h>
#include <stdio.h>

void SPG_CONV SPG_InitMotionBlur(int Depth)
{
	if((Global.NumBlur==0)&&Depth)
	{
		//attention en mode MemHook l'allocation memoire
		//contenue dans InitEcran update la console et blitte !
		G_InitEcran(Global.EcranSystem,Global.Ecran.Etat,
			(BYTE*)0,0,Global.Ecran.POCT,
			Global.WSzeX,Global.WSzeY,0,0,
			(HDC)Global.HDCWND);
		Global.NumBlur=Depth;
		G_Copy(Global.EcranSystem,Global.Ecran);
		if(Global.NumBlur>1) G_MB_Init(Global.MB,Global.EcranSystem,Depth);
	}
	else if(Global.NumBlur&&Depth)
	{
		Global.NumBlur=Depth;
		if(Global.MB.Etat) G_MB_Close(Global.MB);
		if(Global.NumBlur>1) G_MB_Init(Global.MB,Global.EcranSystem,Global.NumBlur);
	}
	else if(Depth==0)
	{
		SPG_CloseMotionBlur();
	}
	return;
}

void SPG_CONV SPG_CloseMotionBlur()
{
	if(Global.MB.Etat) G_MB_Close(Global.MB);
	if(Global.NumBlur)
	{
		G_CloseEcran(Global.EcranSystem);
		Global.NumBlur=0;
	}
	return;
}

#endif
#endif
