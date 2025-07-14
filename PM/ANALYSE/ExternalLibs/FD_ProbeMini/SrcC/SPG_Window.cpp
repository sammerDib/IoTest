
#include "SPG_General.h"
#ifdef SPG_General_USEWindow
#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include "BreakHook.h"

#include <string.h>
#include <stdio.h>
//nclude <windows.h>


int SPG_CONV SPG_WindowCreateEcran(SPG_Window& SW);
void SPG_CONV SPG_WindowCloseEcran(SPG_Window& SW);

static LRESULT CALLBACK WindowProc( HWND hwnd, // handle of window 
							UINT uMsg, // message identifier 
							WPARAM wParam,  // first message parameter 
							LPARAM lParam  // second message parameter 
							)
{
	TRY_BEGIN

	//selectionner la chaine ci apres et Shift+F9 pour voir le message : uMsg,wm

	SPG_Window& SW=*(SPG_Window*)GetWindowLong(hwnd,GWL_USERDATA); 
	if((&SW==0) || (SW.Etat==0)) return DefWindowProc(hwnd,uMsg,wParam,lParam);

	LTG_Enter(SW,LT_WProc,uMsg);

	switch(uMsg)
	{
	case WM_CREATE:							LTG_Exit(SW,LT_WProc,0);		return 0;
	case WM_ERASEBKGND:				LTG_Exit(SW,LT_WProc,0);		return -1;
	case WM_SYSCOMMAND:				{	if((SW.SPGWindowType&SPGWT_Moveable)==0)	{	if((wParam&0xFFF0)==SC_MOVE)	{	LTG_Exit(SW,LT_WProc,0);	return 0;	}	}	}	break;
	case WM_ACTIVATE:						{	SW.Active=(wParam!=0);	}	break;
	case WM_PAINT:								{	if(SW.WCB==0)	{	if((SW.Interlocked)||(SPL_Enter(SW.L,"WindowProc:PAINT")==SPL_OK))	{		SW.Interlocked++;	SPG_BlitWindow(SW);	}	}	}	break;	//attention il y a shunt si une callback est définie dans Global mais que paint n'y est pas traité
	case WM_SIZE:
		{
			SW.Visible = (wParam!=SIZE_MINIMIZED)&&(wParam!=SIZE_MAXHIDE);

			if( (LOWORD(lParam)>0) && (HIWORD(lParam)>0) && (wParam!=SIZE_MINIMIZED) )
			{
				//DbgCHECK((SW.SPGWindowType&SPGWT_Sizeable)==0,"WindowProc");
				if((SW.SPGWindowType&SPGWT_Sizeable)==0) return 0; //use fenetre non sizeable qui est restaurée apres une minimisation recoit quand meme un WM_SIZE - return pour ne pas executer la callback de fenetre
				if((SW.Interlocked)||(SPL_Enter(SW.L,"WindowProc:WM_SIZE")==SPL_OK))
				{	
					SW.Interlocked++;
					if( (SW.SizeX<LOWORD(lParam)) || (SW.SizeY<HIWORD(lParam)) || (SW.SPGWindowType&SPGWT_AAGraphics) )
					{
						SW.SizeChanged=true;				SW.SizeX=LOWORD(lParam);			SW.SizeY=HIWORD(lParam);			SPG_WindowCloseEcran(SW);				SPG_WindowCreateEcran(SW);
					}
					else if((SW.SizeX!=LOWORD(lParam))||(SW.SizeY!=HIWORD(lParam)))
					{
						SW.SizeChanged=true;				SW.SizeX=LOWORD(lParam);			SW.SizeY=HIWORD(lParam);			SW.Ecran.SizeX=LOWORD(lParam);		SW.Ecran.SizeY=HIWORD(lParam); //pour reduire les dimensions on ne realloue pas
					}
				}
				else	{	LTG_Exit(SW,LT_WProc,1);	return 0;	} //ne pas continuer dans la callback de fenetre  si on n'a pas pu entrer dans le spinlock
			}
		}
		break;
	case WM_CLOSE:
	case WM_DESTROY:
		{
			if(SW.WCB) break; //si une callback est définie pour reagir à cet evenement
			LTG_Exit(SW,LT_WProc,0);		return 0;//sinon empeche la fenetre de se fermer d'elle meme
		}
	case WM_QUIT:
	case PBT_APMSUSPEND:
		break;
	case WM_MOUSEMOVE: 
		{
			if(((SW.CreateWindowFlag&WS_CAPTION)==0)&&(SW.SPGWindowType&SPGWT_Moveable)&&SW.MouseLeft&&(wParam&MK_LBUTTON)) 
			{
				SetWindowPos((HWND)SW.hWndWin,0,
					SW.ExternPosX+=(LOWORD(lParam)-SW.MouseX),		SW.ExternPosY+=(HIWORD(lParam)-SW.MouseY),
					0,0,SWP_NOOWNERZORDER|SWP_NOSENDCHANGING|SWP_NOSIZE|SWP_NOZORDER); 
			}
			else 	{	SW.MouseX=LOWORD(lParam)*SW.AG.K;		SW.MouseY=HIWORD(lParam)*SW.AG.K;	}
			
			if (wParam&MK_LBUTTON) SW.MouseLeft|=1; else SW.MouseLeft=0;
			
			if (wParam&MK_RBUTTON) SW.MouseRight|=1; else SW.MouseRight=0; 

			SW.UserInput=true;
		}
		break;
	case WM_LBUTTONDOWN:	{	SW.MouseX=LOWORD(lParam)*SW.AG.K;	SW.MouseY=HIWORD(lParam)*SW.AG.K;	SW.MouseLeft|=1;		SW.UserInput=true;	}	break;
	case WM_RBUTTONDOWN:	{	SW.MouseX=LOWORD(lParam)*SW.AG.K;	SW.MouseY=HIWORD(lParam)*SW.AG.K;	SW.MouseRight|=1;	SW.UserInput=true;	}	break;
	case WM_LBUTTONUP:			{	SW.MouseX=LOWORD(lParam)*SW.AG.K;	SW.MouseY=HIWORD(lParam)*SW.AG.K;	SW.MouseLeft=0;		SW.UserInput=true;	}	break;
	case WM_RBUTTONUP:			{	SW.MouseX=LOWORD(lParam)*SW.AG.K;	SW.MouseY=HIWORD(lParam)*SW.AG.K;	SW.MouseRight=0;	SW.UserInput=true;	}	break;
	case WM_KEYDOWN:
		{
			SW.UserInput=true;
			if(GetKeyState(VK_CONTROL)&32768)
			{
				switch(wParam)
				{
				case 'L':	SPG_List_ResetMsg();	break;
#ifdef DebugProgPrincipalTimer
				case 'T':
					{
						S_StopTimer(Global.T_Total);
						S_PrintRatio(&Global.T_Total,Global.TimerUsed);
						for(int t=0;t<Global.TimerUsed;t++)
						{
							S_TIMER& T=(&Global.T_Total)[t];
							S_ResetTimer(T);
						}
						S_StartTimer(Global.T_Total);
					}
					break;
#endif
				case 'S'://wParam
#ifdef SPG_General_USEFilesWindows
					if(SPL_Enter(SW.L,"WindowProc:CTRL+S")==SPL_OK)	{	SW.Interlocked=true;	G_SaveAs_ToBMP(SW.Ecran,"Ecran");	}
#endif
					break;
				case 'C':
					if(G_MECR(SW.Ecran))
					{
						if(OpenClipboard((HWND)SW.hWndWin))
						{
							if(EmptyClipboard())
							{
								BYTE* hData=(BYTE*)GlobalAlloc(GMEM_ZEROINIT|GMEM_MOVEABLE|GMEM_DDESHARE,
									sizeof(BITMAPINFOHEADER)+
									((G_POCT(SW.Ecran)==1)?1024:0)+
									G_SizeY(SW.Ecran)*((SW.Ecran.SizeX*SW.Ecran.POCT+3)&0xfffffffc));
								//SPG_MemAlloc(sizeof(BITMAPINFOHEADER)+((E.POCT==1)?1024:0)+Global.Ecran.Size,"Clipboard");
								if(hData)
								{
									BYTE* lpData=(BYTE*)GlobalLock(hData);
									G_FillBitmapInfoUnsafe(lpData,SW.Ecran,0);

									if(SPL_Enter(SW.L,"WindowProc:CTRL+C")==SPL_OK)	
									{	
										SW.Interlocked=true;	
										for(int y=0;y<G_SizeY(SW.Ecran);y++)
										{
											BYTE* hLine=lpData+sizeof(BITMAPINFOHEADER)+
												((G_POCT(SW.Ecran)==1)?1024:0)+
												(G_SizeY(SW.Ecran)-1-y)*((SW.Ecran.SizeX*SW.Ecran.POCT+3)&0xfffffffc);
											
											memcpy(
												hLine,
												G_MECR(SW.Ecran)+y*G_Pitch(SW.Ecran),
												G_SizeX(SW.Ecran)*G_POCT(SW.Ecran));
											
										}
									}
									GlobalUnlock(hData);
									SetClipboardData( CF_DIB, hData );
								}
							}
							CloseClipboard();
						}
					}
					break;
				case 'X':
					if(SPG_GLOBAL_ETAT(MUSTEXIT))
					{
						Global.Etat|=SPG_GLOBAL_FATALEXIT;
						//Global.Etat&=~SPG_GLOBAL_MUSTEXIT;
						Global.Etat&=~SPG_GLOBAL_PAUSE;
					}
					else
					{
						Global.Etat|=SPG_GLOBAL_MUSTEXIT;
						Global.Etat&=~SPG_GLOBAL_PAUSE;
					}
					break;
				case 'P':
					if(Global.Etat==SPG_GLOBAL_OK)
					{
						Global.Etat|=SPG_GLOBAL_PAUSE;
					}
					else
						Global.Etat&=~SPG_GLOBAL_PAUSE;
					break;
				case 'D':
					SPG_Config_Save("SrcC_Config.txt");
					break;
				}//wParam
			}//VK_CONTROL
		}//WM_KEYDOWN
		break;

	}//switch(uMsg)

	int v=0;

	if(SW.WCB)
	{
		LTG_EnterEH(SW,LT_Callback,0);
		v=SW.WCB(SW,SW.User,(DWORD)hwnd,(DWORD)uMsg,(DWORD)wParam,(DWORD)lParam);
#ifdef SPG_DEBUGCONFIG
		CHECK(SW.SizeChanged,"SPG_Window:SW.WCB",BreakHook());//le flag n' pas ete resette dans WM_RESIZE dans la callback -> implementation incomplete
#endif
		LTG_ExitEH(SW,LT_Callback,0);
	}
	else
	{
		v=DefWindowProc(hwnd,uMsg,wParam,lParam);
	}

	if(SW.Interlocked) {SW.Interlocked--; if(SW.Interlocked==0) SPL_Exit(SW.L);}
	LTG_Exit(SW,LT_WProc,0);
	return v;
	TRY_ENDGRZ("SPG_Window:WCB")
}

/*
SetFullScreen

 WINDOWPLACEMENT g_wpPrev = { sizeof(g_wpPrev) };

void OnLButtonUp(HWND hwnd, int x, int y, UINT keyFlags)
{
  DWORD dwStyle = GetWindowLong(hwnd, GWL_STYLE);
  if (dwStyle & WS_OVERLAPPEDWINDOW) {
    MONITORINFO mi = { sizeof(mi) };
    if (GetWindowPlacement(hwnd, &g_wpPrev) &&
        GetMonitorInfo(MonitorFromWindow(hwnd,
                       MONITOR_DEFAULTTOPRIMARY), &mi)) {
      SetWindowLong(hwnd, GWL_STYLE,
                    dwStyle & ~WS_OVERLAPPEDWINDOW);
      SetWindowPos(hwnd, HWND_TOP,
                   mi.rcMonitor.left, mi.rcMonitor.top,
                   mi.rcMonitor.right - mi.rcMonitor.left,
                   mi.rcMonitor.bottom - mi.rcMonitor.top,
                   SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
    }
  } else {
    SetWindowLong(hwnd, GWL_STYLE,
                  dwStyle | WS_OVERLAPPEDWINDOW);
    SetWindowPlacement(hwnd, &g_wpPrev);
    SetWindowPos(hwnd, NULL, 0, 0, 0, 0,
                 SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER |
                 SWP_NOOWNERZORDER | SWP_FRAMECHANGED);
  }
}

// add to WndProc
    HANDLE_MSG(hwnd, WM_LBUTTONUP, OnLButtonUp);

*/

void SPG_CONV SPG_WinCompute(SPG_Window& SW, float XAlign, float YAlign)
{

	/*
	if (SW.SPGWindowType&SPGWT_FullScreen) 
	{
		SW.SPGWindowType&=~SPGWT_UserFriendly;
		//SPG_SetFullScreen(SizeX,SizeY,Depth,SPGWindowType&SPGWT_ForceThisMode);
	}
	else
	{
	}
	*/
	
	//SW.ScreenX=GetSystemMetrics(SM_CXSCREEN);
	//SW.ScreenY=GetSystemMetrics(SM_CYSCREEN);
	SW.ScreenX=GetSystemMetrics(SM_CXVIRTUALSCREEN); //SM_CXSCREEN);
	SW.ScreenY=GetSystemMetrics(SM_CYVIRTUALSCREEN); //SM_CYSCREEN);

	if(SW.SPGWindowType&SPGWT_UserFriendly) 
	{
		if(SW.SPGWindowType&SPGWT_Sizeable)
		{
		   SW.CreateWindowFlag=WS_VISIBLE|WS_CAPTION|WS_MINIMIZEBOX|WS_MAXIMIZEBOX|WS_SYSMENU;
		}
		else
		{
		   SW.CreateWindowFlag=WS_VISIBLE|WS_CAPTION|WS_MINIMIZEBOX|WS_SYSMENU;
		}
	}
	else
	{
		SW.CreateWindowFlag=WS_VISIBLE|WS_POPUP;
	}

	if(SW.SPGWindowType&SPGWT_Sizeable) SW.CreateWindowFlag|=WS_SIZEBOX;

	int BX=0;
	int BY=0;
	if(SW.CreateWindowFlag&WS_CAPTION) 
	{
		if((SW.SPGWindowType&SPGWT_Sizeable)==0)
		{
			BX = 2*(GetSystemMetrics(SM_CXFIXEDFRAME));
			BY = 2*(GetSystemMetrics(SM_CYFIXEDFRAME))+GetSystemMetrics(SM_CYCAPTION)+1;
		}

		else
		{
			BX = 2*(GetSystemMetrics(SM_CXSIZEFRAME));
			BY = 2*(GetSystemMetrics(SM_CYSIZEFRAME))+GetSystemMetrics(SM_CYCAPTION)+1;
		}
	}

	int LockedXY=0;
	if((SW.SizeX==0)||(SW.SizeX>SW.ScreenX-BX)) {SW.SizeX=SW.ScreenX-BX;LockedXY++;} //SW.SPGWindowType&=~SPGWT_Moveable;}
	if((SW.SizeY==0)||(SW.SizeY>SW.ScreenY-BY)) {SW.SizeY=SW.ScreenY-BY;LockedXY++;} //SW.SPGWindowType&=~SPGWT_Moveable;}
	if(LockedXY==2) {SW.SPGWindowType&=~SPGWT_Moveable;} //full size window cannot be scrolled

	SW.ExternX=SW.SizeX+BX;
	SW.ExternY=SW.SizeY+BY;

	/*
	if(SW.FullScreen)
	{
		SW.ExternPosX=0;
		SW.ExternPosY=0;
	}
	else
	*/
	{
		SW.ExternPosX=V_Round((1+XAlign)*(SW.ScreenX-SW.ExternX)/2)+GetSystemMetrics(SM_XVIRTUALSCREEN);
		SW.ExternPosY=V_Round((1+YAlign)*(SW.ScreenY-SW.ExternY)/2)+GetSystemMetrics(SM_YVIRTUALSCREEN);
	}
	return;
}

int SPG_CONV SPG_WindowCreateEcran(SPG_Window& SW)
{
	LTG_EnterI(SW,LT_Blit,0);
	SW.HDCWND=(int)GetDC((HWND)SW.hWndWin);
#ifdef SPG_General_USEGEFFECT
	if(SW.SPGWindowType&SPGWT_AAGraphics)
	{
		CHECK(
		G_InitEcran(SW.EBlit,SW.EType,
			(BYTE*)0,0,(SW.Depth+7)/8,
			SW.SizeX,SW.SizeY,0,0,
			(HDC)SW.HDCWND)==0,
			"SPG_CreateWindow: G_InitEcran failed",;);
		AG_Create(SW.AG,SW.EBlit,0xFFFFFF,2); SPG_MemCatName(G_MECR(SW.AG.E),SW.WName);
		G_InitAliasMemEcran(SW.Ecran,G_MECR(SW.AG.E),G_Pitch(SW.AG.E),G_POCT(SW.AG.E),G_SizeX(SW.AG.E),G_SizeY(SW.AG.E));
	}
	else
#endif
	{
		SW.AG.K=1;
		CHECK(
		G_InitEcran(SW.Ecran,SW.EType,
			(BYTE*)0,0,(SW.Depth+7)/8,
			SW.SizeX,SW.SizeY,0,0,
			(HDC)SW.HDCWND)==0,
			"SPG_CreateWindow: G_InitEcran failed",;);
	}

#ifdef DebugMem
	if(SPG_MemIsBlock(G_MECR(SW.Ecran))) SPG_SetMemName(G_MECR(SW.Ecran),"SW.Ecran"); //dans le cas d'un DIB la memoire n'appartient pas au memory manager
#endif
	LTG_ExitI(SW,LT_Blit,0);
	return -1;
}

void SPG_CONV SPG_WindowCloseEcran(SPG_Window& SW)
{
	LTG_EnterC(SW,LT_Blit,0);
	if(SW.Ecran.Etat) G_CloseEcran(SW.Ecran);
#ifdef SPG_General_USEGEFFECT
	if(SW.SPGWindowType&SPGWT_AAGraphics)
	{
		AG_Close(SW.AG);
		G_CloseEcran(SW.EBlit);
	}
#endif
	ReleaseDC((HWND)SW.hWndWin,(HDC)SW.HDCWND);
	LTG_ExitC(SW,LT_Blit,0);
	return;
}

int SPG_CONV SPG_CreateWindow(SPG_Window& SW, int SPGWindowType, int EType, float XAlign, float YAlign, int SizeX, int SizeY, int Depth, const char* WName, const char* WClassName, SPG_USERWINDOWCALLBACK WCB, void* User, int hInstance)
{
	SPG_ZeroStruct(SW);
	if((SPGWindowType==SPGWT_NoDisplay)||(WName==0)||(WClassName==0)) return 0;

	LTG_CreateDescr(SW,0  ,LT_WProc,0,WName);
	LTG_CreateDescr(SW,0  ,LT_Callback,0,WName);
	LTG_CreateDescr(SW,0  ,LT_Blit,0,WName);

	SPL_Init(SW.L,100,WName);

	SW.SPGWindowType=SPGWindowType;
	SW.EType=EType;
	SW.SizeX=SizeX;
	SW.SizeY=SizeY;
	SW.Depth=Depth;
	SW.WCB=WCB;
	SW.User=User;
	SW.ThreadID=GetCurrentThreadId();

	strncpy(SW.WName,WName,WMaxName-1);
	strncpy(SW.WClassName,WClassName,WMaxName-1);
	SW.hInstance=hInstance;
	if(SW.hInstance==0) SW.hInstance=(int)GetModuleHandle(0);

	SPG_WinCompute(SW, XAlign, YAlign);

	WNDCLASSEX  wcex;
	wcex.cbSize=sizeof(WNDCLASSEX);
	wcex.style=(SPGWindowType&SPGWT_NoClose)?CS_NOCLOSE:0;
	wcex.lpfnWndProc=(WNDPROC)WindowProc;
	wcex.cbClsExtra=0;
	wcex.cbWndExtra=32;
	wcex.hInstance=(HINSTANCE)SW.hInstance;
	wcex.hIcon=0;//LoadIcon(GetModuleHandle(0),MAKEINTRESOURCE(IDI_ICON));
	wcex.hCursor=LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground=0;//(HBRUSH)COLOR_WINDOW;
	wcex.lpszMenuName=NULL;
	wcex.lpszClassName=SW.WClassName;
	wcex.hIconSm=wcex.hIcon;

	//CHECK(RegisterClassEx(&wcex)==0,"SPG_CreateWindow",;);

	RegisterClassEx(&wcex);
	
	CHECK((
		SW.hWndWin = (int)CreateWindowEx(((SPGWindowType&SPGWT_UserFriendly)?WS_EX_APPWINDOW:WS_EX_TOOLWINDOW),
		wcex.lpszClassName,
		SW.WName,
		SW.CreateWindowFlag,
		SW.ExternPosX,
		SW.ExternPosY,
		SW.ExternX,
		SW.ExternY,
		NULL,
		NULL,
		(HINSTANCE)SW.hInstance,
		NULL)
		)==0,"SPG_CreateWindow",;);

	//int we=GetLastError();

	
	ShowWindow((HWND)SW.hWndWin, SW_SHOWNORMAL);
	//DoEvents(SPG_DOEV_MIN);
	if(SPGWindowType&SPGWT_Topmost)
	{
		SetWindowPos((HWND)SW.hWndWin,(HWND)-1,0,0,0,0,SWP_NOMOVE | SWP_NOSIZE);
	}
	else
	{
		SetActiveWindow((HWND)SW.hWndWin);
	}
	//SPG_Sleep(25);
	//DoEvents(SPG_DOEV_MIN);
	SW.HDCWND=(int)GetDC((HWND)SW.hWndWin);
	//SPG_Sleep(25);
	//DoEvents(SPG_DOEV_MIN);

	if (SW.Depth==0) SW.Depth=GetDeviceCaps((HDC)SW.HDCWND,BITSPIXEL);
	
	if(SW.Depth==8)//&&(SW.FullScreen))
	{
		SW.Palette=(LOGPALETTE*)SPG_MemAlloc(4+256*4,"Palette");
		for(int i=0;i<256;i++)
		{
			((LOGPALETTE*)SW.Palette)->palPalEntry[i].peRed=(BYTE)i;
			((LOGPALETTE*)SW.Palette)->palPalEntry[i].peGreen=(BYTE)i;
			((LOGPALETTE*)SW.Palette)->palPalEntry[i].peBlue=(BYTE)i;
			((LOGPALETTE*)SW.Palette)->palPalEntry[i].peFlags=0;//PC_EXPLICIT;
		}
		((LOGPALETTE*)SW.Palette)->palVersion=0x300;
		((LOGPALETTE*)SW.Palette)->palNumEntries=256;
		SW.FullScreenPalette=CreatePalette((LOGPALETTE*)SW.Palette);
		if (SW.FullScreenPalette)
		{
			SW.OldPalette=SelectPalette((HDC)SW.HDCWND,(HPALETTE)SW.FullScreenPalette,0);
			RealizePalette((HDC)SW.HDCWND);
			//SPG_List1("Palette:",B);
		}
	}

	ReleaseDC((HWND)SW.hWndWin,(HDC)SW.HDCWND);

	SPG_WindowCreateEcran(SW);

	SW.Etat=1;
	SW.Active=1;
	SW.Visible=1;
	SetWindowLong((HWND)SW.hWndWin,GWL_USERDATA,(int)&SW);//sinon les messages de blit peuvent arriver sur un ecran incomplètement initialisé
	CHECK(GetWindowLong((HWND)SW.hWndWin,GWL_USERDATA)!=(int)&SW,"SPG_CreateWindow",;);
	SPL_Exit(SW.L);
	return -1;
}

int SPG_CONV SPG_CloseWindow(SPG_Window& SW)
{
	SPL_Enter(SW.L,"SPG_CloseWindow");
	SW.Etat=0;
	SPG_WindowCloseEcran(SW);
	if (SW.OldPalette) SelectObject((HDC)SW.HDCWND,SW.OldPalette);
	if (SW.FullScreenPalette) DeleteObject(SW.FullScreenPalette);
	if (SW.Palette) SPG_MemFree(SW.Palette);
	DoEvents(SPG_DOEV_MIN|SPG_DOEV_FLUSH_WIN_EVENTS);
	if(SW.hWndWin)
	{
		DestroyWindow((HWND)SW.hWndWin);
		UnregisterClass(SW.WClassName,(HINSTANCE)SW.hInstance);
	}
	SPL_Close(SW.L);
	SPG_ZeroStruct(SW);
	return -1;
}

void SPG_CONV SPG_BlitWindow(SPG_Window& SW)
{
	if(SW.Etat==0) return;
	LTG_EnterEH(SW,LT_Blit,0);
#ifdef SPG_General_USEGEFFECT
	if(SW.SPGWindowType&SPGWT_AAGraphics)
	{
		AG_BlitEcran(SW.AG,SW.EBlit);
		G_BlitEcran(SW.EBlit);
	}
	else
#endif
	{
		if(SW.Ecran.Etat) G_BlitEcran(SW.Ecran);
	}
	LTG_ExitEH(SW,LT_Blit,0);
	return;
}


int SPG_CONV SPG_CreateWindow(SPG_Window* &SW, int SPGWindowType, int EType, int XAlign, int YAlign, int SizeX, int SizeY, int Depth, const char* WName, const char* WClassName, SPG_USERWINDOWCALLBACK WCB, void* User, int hInstance)
{
	SW=SPG_TypeAlloc(1,SPG_Window,"CreateWindow");
	if(SPG_CreateWindow(*SW,SPGWindowType,EType,XAlign,YAlign,SizeX,SizeY,Depth,WName,WClassName,WCB,User,hInstance))
	{
		return -1;
	}
	else
	{
		SPG_MemFree(SW);
		return 0;
	}
}

int SPG_CONV SPG_CloseWindow(SPG_Window* &SW)
{
	CHECK(SW==0,"SPG_CloseWindow",return 0);
	SPG_CloseWindow(*SW);
	SPG_MemFree(SW);
	return -1;
}

void SPG_CONV SPG_BlitWindow(SPG_Window* &SW)
{
	CHECK(SW==0,"SPG_BlitWindow",return);
	SPG_BlitWindow(*SW);
	return;
}


#endif

