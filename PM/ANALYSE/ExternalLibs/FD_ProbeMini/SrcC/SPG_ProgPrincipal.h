
#ifdef SPG_General_USEProgPrincipal

#define SPG_GlobalReadMousePos(lParam) if(Global.Moveable&&SPG_Global_MouseLeft) SetWindowPos((HWND)Global.hWndWin,0,Global.TotalPosX+=(LOWORD(lParam)-SPG_Global_MouseX),Global.TotalPosY+=(HIWORD(lParam)-SPG_Global_MouseY),0,0,SWP_NOOWNERZORDER|SWP_NOSENDCHANGING|SWP_NOSIZE|SWP_NOZORDER); else {SPG_Global_MouseX=LOWORD(lParam);SPG_Global_MouseY=HIWORD(lParam);}
#define cseWM(eventName, lParam, side, operate) case WM_##eventName: SPG_GlobalReadMousePos(lParam); SPG_Global_Mouse##side operate; break;
#ifndef SPG_General_USENetwork_Protocol
#define cseWMNet(eventName, lParam, side, operate) case WM_##eventName: SPG_GlobalReadMousePos(lParam); SPG_Global_Mouse##side operate; break;
#else
#define cseWMNet(eventName, lParam, side, operate) case WM_##eventName: if(S_IsStarted(Global.NetControlTimer)==0){SPG_GlobalReadMousePos(lParam); SPG_Global_Mouse##side operate;} break;
#endif
#ifdef SPG_General_USEGlobal
#define SPG_WaitMouseRelease() {DoEvents(SPG_DOEV_READEVENTS);while((SPG_Global_MouseLeft|SPG_Global_MouseRight)&&(SPG_GLOBAL_ETAT(MUSTQUIT)==0)) DoEvents(SPG_DOEV_LOCK);}
#define SPG_WaitMouseDown() {DoEvents(SPG_DOEV_READEVENTS);while(((SPG_Global_MouseLeft|SPG_Global_MouseRight)==0)&&(SPG_GLOBAL_ETAT(MUSTQUIT)==0)) DoEvents(SPG_DOEV_LOCK);}
#define SPG_BlitAndWaitForClick() {DoEvents(SPG_DOEV_BLITSCREEN);SPG_WaitMouseDown();SPG_WaitMouseRelease();}
#define SPG_SetCurDirFrom(ResultFile) strcpy(Global.CurDir,ResultFile);SPG_PathOnly(Global.CurDir)
#else
#define SPG_WaitMouseRelease()
#define SPG_WaitMouseDown()
#define SPG_BlitAndWaitForClick() DoEvents(SPG_DOEV_BLITSCREEN)
#define SPG_SetCurDirFrom(ResultFile)
#endif


#define SPG_DOEV_LOCK 1
#ifdef SPG_General_USENetwork_Protocol
#define SPG_DOEV_READ_NET_EVENTS 2
#endif
#define SPG_DOEV_READ_WIN_EVENTS 4

#define SPG_DOEV_FLUSH_WIN_EVENTS 8

#define SPG_DOEV_SINGLE_WIN_EVENTS 16

/*
appelle les callbacks
*/
#define SPG_DOEV_UPDATE 32

#define SPG_DOEV_BLITSCREEN 64
#define SPG_DOEV_BLITECRAN SPG_DOEV_BLITSCREEN

#define SPG_DOEV_NETBLIT 128

#ifdef SPG_General_USENetwork_Protocol
#define SPG_DOEV_READEVENTS SPG_DOEV_READ_NET_EVENTS|SPG_DOEV_READ_WIN_EVENTS
#else
#define SPG_DOEV_READEVENTS SPG_DOEV_READ_WIN_EVENTS
#endif

#define SPG_DOEV_ALL_NOEVENT SPG_DOEV_UPDATE|SPG_DOEV_BLITSCREEN|SPG_DOEV_NETBLIT
#ifdef SPG_General_USENetwork_Protocol
#define SPG_DOEV_ALL_NO_WIN_EVENT SPG_DOEV_READ_NET_EVENTS|SPG_DOEV_UPDATE|SPG_DOEV_BLITSCREEN|SPG_DOEV_NETBLIT
#else
#define SPG_DOEV_ALL_NO_WIN_EVENT SPG_DOEV_ALL_NOEVENT
#endif
#ifdef SPG_General_USENetwork_Protocol
#define SPG_DOEV_ALL_NO_NET_EVENT SPG_DOEV_READ_WIN_EVENTS|SPG_DOEV_UPDATE|SPG_DOEV_BLITSCREEN|SPG_DOEV_NETBLIT
#endif
#define SPG_DOEV_ALL_NOBLIT SPG_DOEV_READEVENTS|SPG_DOEV_UPDATE|SPG_DOEV_NETBLIT

#define SPG_DOEV_ALL SPG_DOEV_READEVENTS|SPG_DOEV_UPDATE|SPG_DOEV_BLITSCREEN|SPG_DOEV_NETBLIT
#define SPG_DOEV_MIN SPG_DOEV_READEVENTS

#define SPG_SM_UserFriendly 1
#define SPG_SM_Moveable 2
#define SPG_SM_FullScreen 4
#define SPG_SM_ForceThisMode 8
#define SPG_SM_NoDisplay 16

void SPG_CONV FD_MainStart();
void SPG_CONV DoEvents(int DoevUpdate);
void SPG_CONV SPG_WinMainStart(int hInstance=0, 
							   int SizeX=0,
							   int SizeY=0,
							   int Depth=0,
							   int ScreenMode=SPG_SM_UserFriendly,
#if defined(SPG_General_USEGraphics)&&defined(SPG_General_USEWindows)
							   int TypeEcran=G_STANDARDW,
#else
							   int TypeEcran=0,
#endif
							   char* WName="SPG",
							   char* WCLName="WSPG",
#ifdef SPG_General_USEGlobal
							   SPG_WINDOWCALLBACK WCB=0,
#else
							   void* WCB=0,
#endif
							   int FastMath=1);
void SPG_CONV SPG_WinMainClose();

#define SPG_Initialise() SPG_WinMainStart(0,0,0,0,SPG_SM_NoDisplay,0,0,0,0,1)
#define SPG_InitDouble() SPG_WinMainStart(0,0,0,0,SPG_SM_NoDisplay,0,0,0,0,0)
#define SPG_Close() SPG_WinMainClose()

void SPG_CONV SPG_SetFullScreen(int& SizeX, int& SizeY, int& Depth, int ForceVideoMode);
void SPG_CONV SPG_ResetFullScreen();
void SPG_CONV SPG_WinMainComputeWindowSize(int& SizeX, int& SizeY, int& Depth, int& ScreenMode, int& WindowMode);
void SPG_CONV SPG_WinMainCreateWindow(int& WindowMode, int& ScreenMode, int& Depth, char* WName, char* WClassName, int hInstance);
void SPG_CONV SPG_WinMainCreateEcran(int& Depth, int& TypeEcran);


#endif


