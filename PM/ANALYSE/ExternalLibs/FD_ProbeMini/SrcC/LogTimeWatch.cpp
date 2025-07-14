
#include "SPG_General.h"

#ifdef SPG_General_USEWLTG

#include "SPG_Includes.h"

#include "SPG_SysInc.h"

#include <process.h>    /* _beginthread, _endthread */
#include <stdio.h>    /* _beginthread, _endthread */



int SPG_CONV WLTW_Callback(SPG_Window& SW, WLTW& w, DWORD hwnd, DWORD uMsg, DWORD wParam, DWORD lParam)
{
	switch(uMsg)
	{
	case WM_SIZE:
		{
			if(SW.SizeChanged)	{	SW.SizeChanged=false;	}
			return 0;
		}
	case WM_TIMER:
		{
			WLTW_Update(w);
			SPG_BlitWindow(SW);
			return 0;
		}
	}
	return DefWindowProc((HWND)hwnd,uMsg,wParam,lParam);
}

int SPG_CONV WLTW_InitWindow(WLTW& w)
{
	SPG_CreateWindow(w.SW,SPGWT_UserFriendly|SPGWT_Moveable|SPGWT_Sizeable|SPGWT_NoClose,G_STANDARDW,1,-1,480,192,0,"Notifications","WLT",(SPG_USERWINDOWCALLBACK)WLTW_Callback,&w,Global.hInstance);
	//C_LoadCaracLib(w.CL,w.SW.Ecran,"Carac\\","C5.bmp");
	C_LoadCaracLib(w.CL,w.SW.Ecran,"Carac\\","C6.bmp");
	w.TimerID=1;
	CHECK(SetTimer((HWND)w.SW.hWndWin,w.TimerID,50,0)!=w.TimerID,"WLTW_Init",;);

	w.SW.LT_Blit=LT_UID_HIDDEN;
	w.SW.LT_Callback=LT_UID_HIDDEN;
	w.SW.LT_WProc=LT_UID_HIDDEN;
	return -1;
}

void SPG_CONV WLTW_CloseWindow(WLTW& w)
{
	KillTimer((HWND)w.SW.hWndWin,w.TimerID);
	C_CloseCaracLib(w.CL);
	SPG_CloseWindow(w.SW);
	return;
}

int SPG_CONV WLTW_InitImport(WLTW& w)	{	CHECK(WLT_Import(w.wt,"WLTG")==0,"WLTW_Init",return 0);	WLTW_InitWindow(w);	return -1;	}

int SPG_CONV WLTW_CloseImport(WLTW& w)	{	WLT_Discard(w.wt);	WLTW_CloseWindow(w);	return -1;	}

int SPG_CONV WLTW_InitLocal(WLTW& w)	{	CHECK((w.wt.t=global_t)==0,"WLTW_Init",return 0);	WLTW_InitWindow(w);	return -1;	}

int SPG_CONV WLTW_CloseLocal(WLTW& w)	{	WLTW_CloseWindow(w);	w.wt.t=0;	return -1;	}

int SPG_CONV WLTW_Save(WLTW& w)	{	return LT_Save_TXT(w.wt.t,"WatchLogTime.txt");	}

int SPG_CONV WLTW_Update(WLTW& w)	{	LT_Display(w.wt.t,w.SW.Ecran,w.CL);	SPG_BlitWindow(w.SW);	return -1;	}

void SPG_CONV WLTW_DoMain()
{
	WLTW w;
	if(WLTW_InitImport(w))
	{
		while(GetAsyncKeyState(VK_ESCAPE)==0)
		{
				DoEvents(SPG_DOEV_ALL);
				Sleep(50);
				WLTW_Update(w);
		}
		//WLTW_Save(w);
		WLTW_CloseImport(w);
	}
	return;
}

static unsigned int __stdcall WLTW_ThreadDoSatLocal(void* User)
{
	WLTW& w=*(WLTW*)User;
	CHECK(WLTW_InitLocal(w)==0,"WLTW_DoSatLocal",w.Stopped=1;return 0);
	while(w.StopFlag==0)
	{
		MSG msg;
		GetMessage(&msg, (HWND)w.SW.hWndWin, 0, 0);
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
	WLTW_CloseLocal(w);
	w.Stopped=-1;//doit suivre et non preceder le close pour persister
	return 0;
}

void SPG_CONV WLTW_StartSatLocal(WLTW* &w)
{
	w=SPG_TypeAlloc(1,WLTW,"WLTW_DoSatLocal");
// (HANDLE)_beginthreadex(0,0,LoopThread,pLT,0,&LT.thThreadID);
	w->hThread=_beginthreadex(0,0,WLTW_ThreadDoSatLocal,w,0,&w->ThreadID);
	SPG_SetThreadName(w->ThreadID,"WLTW");
	LT_RegisterThreadName("WLTW",w->ThreadID);
	return;
}

void SPG_CONV WLTW_StopSatLocal(WLTW* &w)
{
	DWORD hThread=w->hThread;
	w->StopFlag=1;
	/*
	int i=0;
	while(w->Stopped==0)
	{
		Sleep(10);
		CHECK(i++>16,"WLTW_StopSatLocal",break);
	}
	*/
	WaitForSingleObject((HANDLE)hThread,500);
	SPG_MemFree(w);
	return;
}

#endif


