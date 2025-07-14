
#include "SPG_General.h"

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <float.h>
#include <stdio.h>
//Size=0 -> maximized window
//depth=0 -> default
//depth!=0 -> conversion a l'affichage
//si fullscreen!=0 tous les parametres comptent
void SPG_CONV SPG_Initialise()
{
#ifdef DebugMem
#ifdef SPG_General_USEGlobal
	Global.StackCheckSet=0xAA55AA55;
	Global.StackCheckTest=0xAA55AA55;
#endif
#endif
	
	_controlfp( _PC_24, _MCW_PC );

	SPG_GET_STACK_INITIAL_POS();//debogage
	
#ifdef SPG_General_USETimer
#ifndef IntelSpeedStepFix
	S_StartInitTimer();//initialisation timer RDTSC
#endif
#endif

#ifdef SPG_General_USEFFT
	INITFFT();
#endif
	
#ifdef SPG_General_USETimer
#ifndef IntelSpeedStepFix
	SPG_Sleep(50);
#else
	int G=GetTickCount();
	S_StartInitTimer();
	G=GetTickCount();
	while((GetTickCount()-G)<200);
#endif
	S_StopInitTimer();
#endif
	return;
}

void SPG_CONV SPG_Close()
{
	SPG_List_ResetMsg();
	
#ifdef SPG_General_USEFFT
	CLOSEFFT();
#endif
#ifdef DebugMem
#ifdef SPG_General_USEGlobal
	if(Global.MS.MemoryHookConsole) SPG_MemCloseHook();
#endif
#endif
	
#ifdef SPG_General_PGLib
	pglAllFreed();
#else

#ifdef DebugMem
#ifdef DebugList
#ifdef SPG_General_USEGlobal
		DbgCHECK(Global.NumCallBack,"SPG_WinMainClose: Callback residuelle");
#endif
#endif
	SPG_MemCheck(0);
#endif
	
#endif
	
	return;
}
