
#include "SPG_General.h"

#ifdef SPG_General_USEWLTG_GLOBAL

#include "SPG_Includes.h"

void SPG_CONV SPG_GlobalLogTimeInit()
{
	WLT_Create(Global.wt,1<<14,32,1<<9,"WLTG",Global.CPUClock); 
	WLT_SetGlobalRef(Global.wt);
//	WLT_SetGlobalCheckClbkHook(Global.WLTP);	

/*
	int LT_DOEVENTS;
	int LT_CLBK;
	int LT_BLIT;
	int LT_PEEKMSG;
	int LT_GETMSG;
	int LT_DISPMSG;
	int LT_SLEEP;
	int LT_WNDPROC;
	int LT_NETSEND;
	int LT_NETREAD;
	int LT_RENDER;
	int LT_MEMALLOC;
	int LT_MEMFREE;
*/

	//IDLE (sleep, waitforsingleobject)
	//SPINLOCK
	//EVENT
	//MEM
	//SCX

	//Ecran, Window, 
	LTG_Create(Global,LT_FLAG_NOCHECK    ,LT_DOEVENTS,0x00FF00);
	//LTG_Create(Global,LT_FLAG_NOCHECK	,LT_LISTMSGBOX,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_CLBK,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_BLIT,0x00FF00);
	LTG_Create(Global,LT_FLAG_IDCHECK  ,LT_PEEKMSG,0x00FF00);
	//LTG_Create(Global,LT_GETMSG,0x00FF00);
	//LTG_Create(Global,LT_DISPMSG,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_SLEEP,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_WNDPROC,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_NETSEND,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_NETREAD,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_RENDER,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK	 ,LT_MEM,0x00FF00);
	//LT_Create(Global,LT_MEMFREE,0x00FF00);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_OSAbnormalTermination,0x000000);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_AttemptToChangeOSAbnormalTermination,0x000000);
	LTG_Create(Global,LT_FLAG_NOCHECK   ,LT_EditNumericButton,0x000000);

	return;
}

void SPG_CONV SPG_GlobalLogTimeClose()
{
	WLT_ClearGlobalRef(Global.wt);
	WLT_Close(Global.wt);
	return;
}

#endif

