
#ifdef SPG_General_USEGlobal

#define MaxProgDir 1024
#define MaxCallBack 32
#define MaxNrOrdre 4
#define MaxClassName 32
#define MAXCHECKCALLBACK 8

typedef void(SPG_CONV * SPG_CALLBACK)(void* User);
typedef void(SPG_CONV * SPG_CHECKCALLBACK)(void* User, const char* Txt);
typedef int(SPG_CONV * SPG_WINDOWCALLBACK)(DWORD hwnd, DWORD uMsg, DWORD wParam, DWORD lParam);

#ifndef SPG_General_PGLib
typedef struct
{
	int MouseX;
	int MouseY;
	int MouseLeft;
	int MouseRight;
	int MouseInWindow;
} SPG_MouseState;
#else
#define SPG_MouseState mouse
#endif

typedef struct
{
	SHORT MouseX;
	SHORT MouseY;
	BYTE MouseFlags;
} SPG_NetMouseState;

#define SPG_NETMOUSE_LEFT 1
#define SPG_NETMOUSE_RIGHT 2
#define SPG_NETMOUSE_INWINDOW 4

typedef struct
{
	SPG_CALLBACK Addr;
	void* Param;
	int NrOrdre;
	bool Recurse;
} Global_CALLBACK;

typedef struct
{
	SPG_CHECKCALLBACK Addr;
	void* Param;
	int Recurse;
} Check_CALLBACK;

#define SPG_Global_Etat Global.Etat
#define SPG_GLOBAL_INITIALISING 2
#define SPG_GLOBAL_CLOSING 4
#define SPG_GLOBAL_OK 1
#define SPG_GLOBAL_PAUSE 8
#define SPG_GLOBAL_MUSTEXIT 16
#define SPG_GLOBAL_MUSTQUIT 16
#define SPG_GLOBAL_FATALEXIT 32

#define SPG_GLOBAL_ETAT(Flag) (SPG_Global_Etat&SPG_GLOBAL_##Flag) 

//SPG_Global.cpp
void SPG_CONV SPG_AddCallbackOnCheck(SPG_CHECKCALLBACK AddrCallBack, void * ParamCallBack);
void SPG_CONV SPG_RemoveCallbackOnCheck(SPG_CHECKCALLBACK AddrCallBack, void * ParamCallBack);
bool SPG_CONV SPG_RunAllCheckCallbacks(const char* Txt);

void SPG_CONV SPG_AddUpdateOnDoEvents(SPG_CALLBACK AddrCallBack, void * ParamCallBack, int NrOrdre);
void SPG_CONV SPG_KillUpdateOnDoEvents(SPG_CALLBACK AddrCallBack, void * ParamCallBack);
//void SPG_CONV SPG_KillUpdateOnDoEventsByAddr(SPG_CALLBACK AddrCallBack);
//void SPG_CONV SPG_KillUpdateOnDoEventsByParam(void* ParamCallBack);


#ifdef SPG_General_USENetwork_Protocol
#include "SPG_Global_Network.agh"
#endif
#ifdef SPG_General_USEGEFFECT
#include "SPG_Global_MotionBlur.h"
#endif
//SleepEx = Alertable
#ifdef SPG_General_USEWLTG_GLOBAL
#define SPG_Sleep(ms) {G_LogTimeEV(LT_SLEEP,ms);SleepEx(ms,1);G_LogTimeRV(LT_SLEEP,ms);}
#else
#define SPG_Sleep(ms) SleepEx(ms,1)
#endif

#ifdef SPG_General_PGLib
#define SPG_Global_Mouse Global.common->mouse
#define SPG_Global_MouseX (int)(Global.common->mouse.x)
#define SPG_Global_MouseY (int)(Global.common->mouse.y)
#define SPG_Global_MouseLeft (int)(Global.common->mouse.b[0])
#define SPG_Global_MouseRight (int)(Global.common->mouse.b[1])
#define SPG_Global_MouseInWindow true
#else
#define SPG_Global_Mouse Global.Mouse
#define SPG_Global_MouseX Global.Mouse.MouseX
#define SPG_Global_MouseY Global.Mouse.MouseY
#define SPG_Global_MouseLeft Global.Mouse.MouseLeft
#define SPG_Global_MouseRight Global.Mouse.MouseRight
#define SPG_Global_MouseInWindow Global.Mouse.MouseInWindow
#endif

#define SPG_UnderNetControl ((Global.SNP)&&(SPG_IsValidNetAddr(Global.ControlSource)))

#endif
