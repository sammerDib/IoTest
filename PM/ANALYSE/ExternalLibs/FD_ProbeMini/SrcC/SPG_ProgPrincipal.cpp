

#include "SPG_General.h"

#ifdef SPG_General_USEProgPrincipal

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include "BreakHook.h"

#include <float.h>
#include <string.h>
#include <stdio.h>
//abs
#define _CRTDBG_MAP_ALLOC  
#include <stdlib.h>  
#include <crtdbg.h>  
#define SPG_DefaultAVIFPS 16

#define DoEventsAllowRecurse
//il faudrait quand meme tester la recursivite dans la boucle des messages
#ifndef DoEventsAllowRecurse
#ifdef DebugList
int IsInDoevents;
#endif //DebugList
#endif //DoEventsAllowRecurse

#pragma warning( disable : 4706)//assignment within conditional expression

#ifdef FDE
//lit les evenements windows, appele les callback enregistrees et affiche l'ecran
//doevents peut s'appeler de n'importe quelle boucle bloquante du programme
void SPG_CONV DoEvents(int DoevUpdate)
{
	G_LogTimeEV(LT_DOEVENTS,DoevUpdate);
#ifndef DoEventsAllowRecurse
#ifdef DebugList
	IsInDoevents++;
	CHECK(IsInDoevents==2,"DoEvents: Appel recursif",IsInDoevents--;return);
#endif //DebugList
#endif //DoEventsAllowRecurse

#ifdef SPG_General_USEGlobal
	Global.DoEventsRecursion++;

	DbgCHECK( SPG_GLOBAL_ETAT(PAUSE)?(Global.DoEventsRecursion>3):(Global.DoEventsRecursion>2) , "DoEvents: Recursion");
	//if(Global.DoEventsRecursion>1) DoevUpdate=0;

	if(Global.DoEventsRecursion==1)//niveau de base
	{

		if(SPG_GLOBAL_ETAT(FATALEXIT))
		{
			SPG_WinMainClose();
			exit(0);
			//exitprocess?
		}


		while(SPG_GLOBAL_ETAT(PAUSE))
		{
#ifndef DoEventsAllowRecurse
#ifdef DebugList
			IsInDoevents--;
#endif
#endif
			DoEvents(SPG_DOEV_LOCK);
			DoEvents(SPG_DOEV_READEVENTS|SPG_DOEV_FLUSH_WIN_EVENTS);
			DoEvents(SPG_DOEV_BLITECRAN);
#ifndef DoEventsAllowRecurse
#ifdef DebugList
			IsInDoevents++;
#endif
#endif
		}
	}

#endif
	
#ifdef SPG_General_USENetwork_Protocol
	if(Global.SNP)
	{
		if (DoevUpdate&SPG_DOEV_LOCK) 
		{
			DoevUpdate&=~(SPG_DOEV_LOCK);
			DoevUpdate|=(SPG_DOEV_READEVENTS);
		}
	}
#endif // SPG_General_USENetwork_Protocol


#ifdef SPG_General_USEWindows
	if (DoevUpdate&SPG_DOEV_LOCK)
	{
		DoevUpdate=SPG_DOEV_LOCK;
//		G_LogTimeEN(LT_GETMSG);
		MSG msg;
#ifdef DebugProgPrincipalTimer
		S_StartTimer(Global.T_DoevLock);
#endif // DebugProgPrincipalTimer
		GetMessage(&msg, (HWND)0, 0, 0);
#ifdef DebugProgPrincipalTimer
		S_StopTimer(Global.T_DoevLock);
#endif // DebugProgPrincipalTimer
//		G_LogTimeEN(LT_DISPMSG);
//		if((Global.WCB==0)||(Global.WCB((DWORD)hwnd,(DWORD)uMsg,(DWORD)wParam,(DWORD)lParam)==0)) 
#ifdef SPG_General_USEGlobal
		if((Global.WCB==0)||(Global.WCB((DWORD)msg.hwnd,(DWORD)msg.message,(DWORD)msg.wParam,(DWORD)msg.lParam)==0)) 
#endif
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
//		G_LogTimeRN(LT_DISPMSG);
//		G_LogTimeRN(LT_GETMSG);
	}
#endif


#ifdef SPG_General_USEWindows
	if (DoevUpdate&SPG_DOEV_READ_WIN_EVENTS)
	{
#ifndef SPG_General_PGLib
#ifdef SPG_General_USEGlobal
		Global.Mouse.MouseInWindow=true;
#endif

		bool LockOnMouse=false;//si on ne flush pas il faut s'arreter de lire sur les mouse msg pour que les librairies de boutons soient mises à jour
		
		int PRetVal;
		MSG msg;
		//while (PeekMessage(&msg, (HWND) NULL, 0, 0,PM_REMOVE)!=0)
		//if ((LockOnMouse==false)&&(PeekMessage(&msg, (HWND)0, 0, 0,PM_REMOVE)))
		G_LogTimeEN(LT_PEEKMSG);
		while((LockOnMouse==false)&&(PRetVal=PeekMessage(&msg, (HWND)0, 0, 0,PM_REMOVE)))//assignement volontaire PRetVal
		{
			DbgCHECK(msg.message==WM_ENTERIDLE,"WM_ENTERIDLE");
			if((PRetVal==1)&&(msg.hwnd==0)&&(msg.message==WM_NULL)) break;
			if(	((DoevUpdate&SPG_DOEV_FLUSH_WIN_EVENTS)==0) && (
				(msg.message==WM_LBUTTONDOWN)||
				(msg.message==WM_RBUTTONDOWN)||
				(msg.message==WM_LBUTTONUP)||
				(msg.message==WM_RBUTTONUP)
				//||((msg.message==WM_MOUSEMOVE)&&(Global.MouseLeft|Global.MouseRight))
				)) LockOnMouse=true;
			//G_LogTimeEN(LT_DISPMSG);
	//		if((Global.WCB==0)||(Global.WCB((DWORD)hwnd,(DWORD)uMsg,(DWORD)wParam,(DWORD)lParam)==0)) 
#ifdef SPG_General_USEGlobal
			if((Global.WCB==0)||(Global.WCB((DWORD)msg.hwnd,(DWORD)msg.message,(DWORD)msg.wParam,(DWORD)msg.lParam)==0)) 
#endif
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
			}
			//G_LogTimeRN(LT_DISPMSG);
			if(DoevUpdate&SPG_DOEV_SINGLE_WIN_EVENTS) break;
		}
		G_LogTimeRN(LT_PEEKMSG);
#else // SPG_General_PGLib
		pglDoEvents(Global.common);
		pglUpdateMouseState(Global.common);
#endif // SPG_General_PGLib
	}
#endif

#ifdef SPG_General_USENetwork_Protocol
	if (DoevUpdate&SPG_DOEV_READ_NET_EVENTS)
	{
		if(Global.SNP)
		{
			SPG_NetworkUpdate(*(Global.SNP),Global.ControlSource,true,true);
		}
	}
#endif //SPG_General_USENetwork_Protocol

#ifdef MaxCallBack
	if (DoevUpdate&SPG_DOEV_UPDATE)
	{
		for(int CLBK=0;CLBK<Global.NumCallBack;CLBK++)
		{
			if ((Global.CallBack[CLBK].Recurse==false))
			{
	G_LogTimeEV(LT_CLBK,CLBK);
				Global.CallBack[CLBK].Recurse=true;
				Global.CallBack[CLBK].Addr(Global.CallBack[CLBK].Param);
				Global.CallBack[CLBK].Recurse=false;
	G_LogTimeRV(LT_CLBK,CLBK);
			}
		}
	}
	CD_G_CHECK_EXIT(17,7);
#endif

#ifdef SPG_General_USENetwork_Protocol
	if (DoevUpdate&SPG_DOEV_NETBLIT)
	{
		if(Global.SNP)
		{
			if(Global.ScreenSendState.MSND.Etat)
				SPG_Network_SendScreen(*(Global.SNP),Global.ScreenSendState);
		}
	}
#endif

#ifdef SPG_General_USEGraphics
	if((DoevUpdate&SPG_DOEV_BLITSCREEN)&&G_Etat(Global.Ecran))
	{
		G_LogTimeEN(LT_BLIT);

#ifdef SPG_General_PGLib
		pglSwapBuffers(Global.display);
#else

#ifdef SPG_General_USEGraphics
#ifdef DebugProgPrincipalTimer
		S_StartTimer(Global.T_DoevBlitEcran);
#endif

#ifdef SPG_General_USEGEFFECT
		if(Global.NumBlur==0)
		{
			if(!IsIconic((HWND)Global.hWndWin))
			{
				G_BlitEcran(Global.Ecran);
			}
#ifdef SPG_General_USEAVI
			if(Global.AVISG.Etat)
			{
				SPG_AVI_StreamSave_AddFrame(Global.AVISG,Global.Ecran.MECR,Global.Ecran.Pitch);
			}
#endif
		}
		else if(Global.NumBlur==1)
		{
			G_BlurCopy(Global.EcranSystem,Global.Ecran);
			if(!IsIconic((HWND)Global.hWndWin))
			{
				G_BlitEcran(Global.EcranSystem);
			}
#ifdef SPG_General_USEAVI
			if(Global.AVISG.Etat)
			{
				SPG_AVI_StreamSave_AddFrame(Global.AVISG,Global.EcranSystem.MECR,Global.EcranSystem.Pitch);
			}
#endif
		}
		else
		{
			G_MB_RenderFrom(Global.MB,Global.Ecran);
			if(!IsIconic((HWND)Global.hWndWin))
			{
				G_BlitEcran(Global.EcranSystem);
			}
#ifdef SPG_General_USEAVI
			if(Global.AVISG.Etat)
			{
				SPG_AVI_StreamSave_AddFrame(Global.AVISG,Global.EcranSystem.MECR,Global.EcranSystem.Pitch);
			}
#endif
		}
#else // SPG_General_USEGEFFECT
		if(!IsIconic((HWND)Global.hWndWin))
		{
			if(Global.Ecran.Etat) G_BlitEcran(Global.Ecran);
		}
#ifdef SPG_General_USEAVI
		if(Global.AVISG.Etat)
		{
			SPG_AVI_StreamSave_AddFrame(Global.AVISG,Global.Ecran.MECR,Global.Ecran.Pitch);
		}
#endif

#endif // SPG_General_USEGEFFECT // SPG_General_USEGraphics

#ifdef DebugProgPrincipalTimer
		S_StopTimer(Global.T_DoevBlitEcran);
#endif // DebugProgPrincipalTimer
#endif // SPG_General_USEGraphics // SPG_General_PGLib
#endif // SPG_General_PGLib
		G_LogTimeRN(LT_BLIT);
	}
#endif

#ifndef DoEventsAllowRecurse
#ifdef DebugList
	IsInDoevents--;
#endif
#endif
#ifdef SPG_General_USEGlobal
	Global.DoEventsRecursion--;
#endif
	
	G_LogTimeRV(LT_DOEVENTS,DoevUpdate);
	return;
}


#ifdef SPG_General_USEGlobal
#ifndef SPG_General_PGLib
//la callback windows de fenetre
static LRESULT CALLBACK WindowProc( HWND hwnd, // handle of window 
							UINT uMsg, // message identifier 
							WPARAM wParam,  // first message parameter 
							LPARAM lParam  // second message parameter 
							)
{
	G_LogTimeEV(LT_WNDPROC,uMsg);
#ifdef SPG_General_USENetwork_Protocol
float NetCT;
if(S_IsStarted(Global.NetControlTimer))
{
	S_GetTimerRunningTime(Global.NetControlTimer,NetCT);
	if(NetCT>1.0) S_StopTimer(Global.NetControlTimer);
}
#endif

	switch(uMsg)
	{
	case WM_CLOSE:
	case WM_DESTROY:
		{
			Global.Etat|=SPG_GLOBAL_MUSTEXIT;
			Global.Etat&=~SPG_GLOBAL_PAUSE;
		}
		return 0;
	case WM_QUIT:
	case PBT_APMSUSPEND:
		{
			Global.Etat|=SPG_GLOBAL_FATALEXIT;
		}
		break;
	case WM_MOUSEMOVE: 
#ifdef SPG_General_USENetwork_Protocol
	if(S_IsStarted(Global.NetControlTimer)==0)
#endif
	{
		IF_CD_G_CHECK(12,break);
		SPG_GlobalReadMousePos(lParam);
		if (wParam&MK_LBUTTON) 
			SPG_Global_MouseLeft|=1;
		else 
			SPG_Global_MouseLeft=0;
		if (wParam&MK_RBUTTON) 
			SPG_Global_MouseRight|=1; 
		else 
			SPG_Global_MouseRight=0; 
	}
	break;
cseWMNet(LBUTTONDOWN,lParam,Left,|=1);
cseWMNet(RBUTTONDOWN,lParam,Right,|=1);
cseWMNet(LBUTTONUP,lParam,Left,=0);
cseWMNet(RBUTTONUP,lParam,Right,=0); 
	case WM_SYSCOMMAND:
		if(Global.LockWindow)
		{
			if((wParam&0xFFF0)==SC_MOVE) 
				return 0;
		}
		break;
/*
	case WM_WINDOWPOSCHANGING:
		if(Global.LockWindow)
		{
		LPWINDOWPOS lpwp = (LPWINDOWPOS) lParam;
		lpwp->flags|=SWP_NOMOVE|SWP_NOSIZE;
		}
		break;
*/
//il n'est pas forcement souhaitable que WM_PAINT blitte
//a cause d'un probleme de recursivite lorsque
//G_BlitEcran genere un message (msgbox ou PAINT)
#ifdef SPG_General_USEGraphics
		/*
	case WM_PAINT:
		IF_CD_G_CHECK(17,break);
		G_BlitEcran(Global.Ecran);
		break;
		*/

	case WM_KEYDOWN:
		if(GetKeyState(VK_CONTROL)&32768)
		{
		switch(wParam)
		{
		case 'L':
			SPG_List_ResetMsg();
			break;
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
		case 'S':
#ifdef SPG_General_USEFilesWindows
#ifdef SPG_General_USEGEFFECT
			if(Global.NumBlur==0)
				G_SaveAs_ToBMP(Global.Ecran,"Ecran");
			else
				G_SaveAs_ToBMP(Global.EcranSystem,"Ecran");
#else
			G_SaveAs_ToBMP(Global.Ecran,"Ecran");
#endif
#endif
			break;
		case 'C':
			//clipboard
		//CHECKPOINTER_L_ELSE(G_MECR(Global.Ecran),Global.Ecran.Size,"Global.Ecran",;)
		//else
		{
			if(OpenClipboard((HWND)Global.hWndWin))
			{
			  if(EmptyClipboard())
			  {
				  BYTE* hData=(BYTE*)GlobalAlloc(GMEM_ZEROINIT|GMEM_MOVEABLE|GMEM_DDESHARE,
					  sizeof(BITMAPINFOHEADER)+
					  ((G_POCT(Global.Ecran)==1)?1024:0)+
					  G_SizeY(Global.Ecran)*((Global.Ecran.Pitch+3)&0xfffffffc));
				  //SPG_MemAlloc(sizeof(BITMAPINFOHEADER)+((E.POCT==1)?1024:0)+Global.Ecran.Size,"Clipboard");
				  if(hData)
				  {
						BYTE* lpData=(BYTE*)GlobalLock(hData);
						G_FillBitmapInfoUnsafe(lpData,Global.Ecran,0);
						for(int y=0;y<G_SizeY(Global.Ecran);y++)
						{
							BYTE* hLine=lpData+sizeof(BITMAPINFOHEADER)+
								((G_POCT(Global.Ecran)==1)?1024:0)+
								(G_SizeY(Global.Ecran)-1-y)*((Global.Ecran.Pitch+3)&0xfffffffc);
							
							memcpy(
								hLine,
								G_MECR(Global.Ecran)+y*G_Pitch(Global.Ecran),
								G_SizeX(Global.Ecran)*G_POCT(Global.Ecran));
								
						}
						GlobalUnlock(hData);
						SetClipboardData( CF_DIB, hData );
				  }
			  }
			  CloseClipboard();
			}
		}
 			/*
#ifdef SPG_General_USEGEFFECT
			if(Global.NumBlur==0)
				G_SaveAs_ToBMP(Global.Ecran,"Ecran");
			else
				G_SaveAs_ToBMP(Global.EcranSystem,"Ecran");
#else
			G_SaveAs_ToBMP(Global.Ecran,"Ecran");
#endif
			*/
			break;

#ifdef SPG_General_USEGEFFECT
		case 'B':
			if(Global.NumBlur<=0)
				SPG_InitMotionBlur(1);
			else if(Global.NumBlur<=1)
				SPG_InitMotionBlur(4);
			else if(Global.NumBlur<=4)
				SPG_InitMotionBlur(8);
			else
				SPG_InitMotionBlur(0);
			break;
#endif

#ifdef SPG_General_USEAVI
		case 'A':
			if(Global.AVISG.Etat)
			{
				SPG_AVI_StreamSave_Stop(Global.AVISG);
			}
			else
			{
				char Filename[MaxProgDir];
				strcpy(Filename,"Video.avi");
				if(SPG_GetSaveName(SPG_AVI,Filename,MaxProgDir))
				{
					SPG_AVI_StreamSave_Start(Global.AVISG,Filename,
						G_SizeX(Global.Ecran),G_SizeY(Global.Ecran),G_POCT(Global.Ecran),Global.AVIFPS==0?SPG_DefaultAVIFPS:Global.AVIFPS,1,Global.hWndWin);
				}
			}
			break;
#endif
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
		}
		}
		break;
/*
	case WM_CHAR:
		{
			if((wParam==19))//&&(lParam==2031617))
			{
				G_SaveAs_ToBMP(Global.Ecran,"Ecran");
			}
#ifdef DebugList
			if((wParam==12))//&&(lParam==2490369))
			{
				SPG_List_ResetMsg();
			}
#endif
#ifdef DebugMem
			if((wParam==13))//&&(lParam==2555905))
			{
				if(Global.MS.MemoryHookConsole==0)
				{
					SPG_MemSetHook();
					SPG_MemGetTotal(1);
				}
				else
				{
					SPG_MemCloseHook();
					SPG_MemGetTotal(0);
				}
			}
#endif
		}
		break;
		*/
		/*
SC_MONITORPOWER  
SC_SCREENSAVE
GetSystemPowerStatus
touchpad
		*/
#endif //SPG_General_USEGraphics
		case WM_CREATE:
			G_LogTimeRV(LT_WNDPROC,uMsg);
			return 0;
		case WM_ERASEBKGND:
			G_LogTimeRV(LT_WNDPROC,uMsg);
			return -1;
	}

	G_LogTimeRV(LT_WNDPROC,uMsg);
	return DefWindowProc(hwnd,uMsg,wParam,lParam);
}
#endif //SPG_General_PGLib

#ifdef DebugList
#define ListVideoMode
#endif

#ifdef SPG_General_USEWindows
void SPG_CONV SPG_SetFullScreen(int& SizeX, int& SizeY, int& Depth, int ForceVideoMode)
{
if(ForceVideoMode==0)	
{
	SPG_ArrayStackAllocZ(DEVMODE,ScreenMode,1024);
	//getpixelformat
	//liste les modes video availables
	int m;
	for(m=0;m<1024;m++)
		if(!EnumDisplaySettings(0,m,ScreenMode+m)) break;
		
		SPG_ArrayStackAlloc(float,Valid,1024);
		
#ifdef ListVideoMode
		SPG_ArrayStackAlloc(char,ModeList,4096);
		sprintf(ModeList,"Modes video:");
		char ThisMode[64];
#endif
		int sc;
		for (sc=0;sc<m;sc++)
		{
			Valid[sc]=
				(float)(abs((int)(SizeX-ScreenMode[sc].dmPelsWidth))+
				abs((int)(SizeY-ScreenMode[sc].dmPelsHeight))+
				
				(1+(SizeX>>5))//favorise toujours depth> avec Depth+1 (pb 24 -> 16?32)
				*abs((int)(Depth+1-ScreenMode[sc].dmBitsPerPel)));
#ifdef ListVideoMode
			sprintf(ThisMode,"\n%ix%i %ibits",(int)ScreenMode[sc].dmPelsWidth,(int)ScreenMode[sc].dmPelsHeight,(int)ScreenMode[sc].dmBitsPerPel);
			strcat(ModeList,ThisMode);
#endif
		}
#ifdef ListVideoMode
		SPG_List(ModeList);
		SPG_ArrayStackCheck(ModeList);
#endif

		SPG_ArrayStackCheck(Valid);
		SPG_ArrayStackCheck(ScreenMode);
		
		float VMin=Valid[0];
		int scm=0;
		for (sc=1;sc<m;sc++)
		{
			if(Valid[sc]<=VMin) //<= favorise le plus bas mode de la liste (le plus haut refreshrate?)
			{
				VMin=Valid[sc];
				scm=sc;
			}
		}
		
		ScreenMode[scm].dmFields=DM_BITSPERPEL|DM_PELSWIDTH|DM_PELSHEIGHT;
		int R=ChangeDisplaySettings(ScreenMode+scm,0);

		if(R==DISP_CHANGE_SUCCESSFUL) 
		{
			Global.FullScreen=true;
			SizeX=ScreenMode[scm].dmPelsWidth;
			SizeY=ScreenMode[scm].dmPelsHeight;
			Depth=ScreenMode[scm].dmBitsPerPel;
		}
		else
		{
			Global.FullScreen=false;
			ChangeDisplaySettings(0,DM_BITSPERPEL|DM_PELSWIDTH|DM_PELSHEIGHT);
		}
}
else
{
		SPG_StackAllocZ(DEVMODE,ScreenMode);
		ScreenMode.dmSize=sizeof(DEVMODE);
		ScreenMode.dmFields=DM_BITSPERPEL|DM_PELSWIDTH|DM_PELSHEIGHT;
		ScreenMode.dmPelsWidth=SizeX;
		ScreenMode.dmPelsHeight=SizeY;
		ScreenMode.dmBitsPerPel=Depth;

		int R=ChangeDisplaySettings(&ScreenMode,0);

		if(R==DISP_CHANGE_SUCCESSFUL) 
		{
			Global.FullScreen=true;
			Global.Moveable=0;
		}
		else
		{
			Global.FullScreen=false;
			ChangeDisplaySettings(0,DM_BITSPERPEL|DM_PELSWIDTH|DM_PELSHEIGHT);
		}
		SPG_StackCheck(ScreenMode);
}
		
		return;
}

void SPG_CONV SPG_ResetFullScreen()
{
	if (Global.FullScreen) ChangeDisplaySettings(0,0);
	Global.FullScreen=false;
	return;
}
#endif


void SPG_CONV SPG_WinMainComputeWindowSize(int& SizeX, int& SizeY, int& Depth, int& ScreenMode, int& WindowMode)
{
	Global.Moveable=(ScreenMode&SPG_SM_Moveable)!=0;//remis a 0 en cas de passage en full screen
	if (ScreenMode&SPG_SM_FullScreen) 
	{
		SPG_SetFullScreen(SizeX,SizeY,Depth,ScreenMode&SPG_SM_ForceThisMode);
		ScreenMode&=~SPG_SM_UserFriendly;
	}
	else
	{
		Global.FullScreen=false;
	}
	
	if(ScreenMode&SPG_SM_UserFriendly) 
		WindowMode=WS_VISIBLE|WS_CAPTION|WS_MINIMIZEBOX|WS_SYSMENU;

	Global.TotalSzeX=GetSystemMetrics(SM_CXSCREEN);
	Global.TotalSzeY=GetSystemMetrics(SM_CYSCREEN);

	if ((SizeX!=0)&&(SizeY!=0)&&(SizeX<Global.TotalSzeX)&&(SizeY<Global.TotalSzeY)&&(Global.FullScreen==false))
	{
		Global.LockWindow=false;
		Global.TotalSzeX=SizeX;
		Global.TotalSzeY=SizeY;
		
	}
	else
	{
		Global.LockWindow=true;
		if((SizeX!=0)&&(SizeX<Global.TotalSzeX)) 
		{
			Global.TotalSzeX=SizeX;
			Global.LockWindow=false;
		}
		if((SizeY!=0)&&(SizeY<Global.TotalSzeY))
		{
			Global.TotalSzeY=SizeY;
			Global.LockWindow=false;
		}
	}

	if(WindowMode&WS_CAPTION) 
	{
		Global.WSzeX=Global.TotalSzeX-(2*GetSystemMetrics(SM_CXEDGE)+2);
		Global.WSzeY=Global.TotalSzeY-(2*GetSystemMetrics(SM_CYEDGE)+GetSystemMetrics(SM_CYCAPTION)+2);
		Global.WPosX=Global.TotalPosX+GetSystemMetrics(SM_CXEDGE)+1;
		Global.WPosY=Global.TotalPosY+GetSystemMetrics(SM_CYEDGE)+GetSystemMetrics(SM_CYCAPTION)+1;
		Global.TotalPosX=(GetSystemMetrics(SM_CXSCREEN)-Global.TotalSzeX)/2;
		Global.TotalPosY=(GetSystemMetrics(SM_CYSCREEN)-Global.TotalSzeY)/2;
	}
	else
	{
		Global.WSzeX=Global.TotalSzeX;
		Global.WSzeY=Global.TotalSzeY;
		Global.WPosX=0;
		Global.WPosY=0;
		if(Global.FullScreen)
		{
		Global.TotalPosX=0;
		Global.TotalPosY=0;
		}
		else
		{
		Global.TotalPosX=(GetSystemMetrics(SM_CXSCREEN)-Global.TotalSzeX)/2;
		Global.TotalPosY=(GetSystemMetrics(SM_CYSCREEN)-Global.TotalSzeY)/2;
		}
	}
	return;
}

void SPG_CONV SPG_WinMainCreateWindow(int& WindowMode, int& ScreenMode, int& Depth, char* WName, char* WClassName, int hInstance)
{
	strncpy(Global.WClassName,WClassName,MaxClassName-1);
	
	WNDCLASSEX  wcex;

	wcex.cbSize=sizeof(WNDCLASSEX);
	wcex.style=0;//CS_NOCLOSE;
	wcex.lpfnWndProc=(WNDPROC)WindowProc;
	wcex.cbClsExtra=0;
	wcex.cbWndExtra=0;
	wcex.hInstance=(HINSTANCE)Global.hInstance;
	wcex.hIcon=0;//LoadIcon(GetModuleHandle(0),MAKEINTRESOURCE(IDI_ICON));
	wcex.hCursor=LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground=0;//(HBRUSH)COLOR_WINDOW;
	wcex.lpszMenuName=NULL;
	wcex.lpszClassName=fdwstring(Global.WClassName);
	wcex.hIconSm=wcex.hIcon;
	
#ifdef SPG_General_USECDCHECK
	IF_CD_G_CHECK(30,SPG_List("Ce programme est destiné à fonctionner\nà partir de son support original.\nsylvain.petitgrand@ief.u-psud.fr");exit(0));
#endif

	CHECK(RegisterClassEx(&wcex)==0,"SPG_WinMainStart",;);
	
	CHECK((
		Global.hWndWin = (int)CreateWindowEx(((ScreenMode&SPG_SM_UserFriendly)?WS_EX_APPWINDOW:WS_EX_TOOLWINDOW),
		wcex.lpszClassName,
		fdwstring(WName),
		WindowMode ,
		Global.TotalPosX,
		Global.TotalPosY,
		Global.TotalSzeX,
		Global.TotalSzeY,
		NULL,
		NULL,
		(HINSTANCE)hInstance,
		NULL )
		)==0,"SPG_WinMainStart",;);
	
	ShowWindow((HWND)Global.hWndWin, SW_SHOWNORMAL);
	DoEvents(SPG_DOEV_MIN);
	SetActiveWindow((HWND)Global.hWndWin);
	SPG_Sleep(25);
	DoEvents(SPG_DOEV_MIN);
	Global.HDCWND=(int)GetDC((HWND)Global.hWndWin);
	SPG_Sleep(25);
	DoEvents(SPG_DOEV_MIN);
	return;
}

void SPG_CONV SPG_WinMainCreateEcran(int& Depth, int& TypeEcran)
{
	if (Depth==0)
	{
		Depth=GetDeviceCaps((HDC)Global.HDCWND,BITSPIXEL);
		/*
		//GetDisplayMode
		PIXELFORMATDESCRIPTOR  pfd;
		memset(&pfd,0,sizeof(PIXELFORMATDESCRIPTOR));
		pfd.nSize=sizeof(PIXELFORMATDESCRIPTOR);
		int  iPixelFormat=GetPixelFormat((HDC)Global.HDCWND); 
		int i=GetLastError();
		// obtain detailed information about 
		// the device context's first pixel format 
		DescribePixelFormat((HDC)Global.HDCWND, iPixelFormat,  
		sizeof(PIXELFORMATDESCRIPTOR), &pfd); 
		Depth=pfd.cColorBits;
		*/
	}
	
	if(Depth==8)//&&(Global.FullScreen))
	{
		Global.Palette=(LOGPALETTE*)SPG_MemAlloc(4+256*4,"Palette");
		for(int i=0;i<256;i++)
		{
			((LOGPALETTE*)Global.Palette)->palPalEntry[i].peRed=(BYTE)i;
			((LOGPALETTE*)Global.Palette)->palPalEntry[i].peGreen=(BYTE)i;
			((LOGPALETTE*)Global.Palette)->palPalEntry[i].peBlue=(BYTE)i;
			((LOGPALETTE*)Global.Palette)->palPalEntry[i].peFlags=0;//PC_EXPLICIT;
		}
		((LOGPALETTE*)Global.Palette)->palVersion=0x300;
		((LOGPALETTE*)Global.Palette)->palNumEntries=256;
		Global.FullScreenPalette=CreatePalette((LOGPALETTE*)Global.Palette);
		if (Global.FullScreenPalette)
		{
			Global.OldPalette=SelectPalette((HDC)Global.HDCWND,(HPALETTE)Global.FullScreenPalette,0);
			RealizePalette((HDC)Global.HDCWND);
			//SPG_List1("Palette:",B);
		}
		
	}
	
	//G_InitEcranWindows(Global.Ecran,(Depth+7)/8,Global.WSzeX,Global.WSzeY,(HDC)Global.HDCWND);
	//G_InitEcranDibSect(Global.Ecran,(Depth+7)/8,Global.WSzeX,Global.WSzeY,(HDC)Global.HDCWND);
#ifdef SPG_General_USEGraphics
	
	CHECK(
	G_InitEcran(Global.Ecran,TypeEcran,
		(BYTE*)0,0,(Depth+7)/8,
		Global.WSzeX,Global.WSzeY,0,0,
		(HDC)Global.HDCWND)==0,
		"SPG_WinMainStart: G_InitEcran failed",;);

#ifdef DebugMem
	if(SPG_MemIsBlock(G_MECR(Global.Ecran))) SPG_SetMemName(G_MECR(Global.Ecran),"Global.Ecran");
#endif
#endif

	return;
}
#endif //SPG_General_USEGlobal

/*
#ifdef DebugFloat
void SPG_CONV SPG_CoproStatus(void*Z)
{	
	CHECK((_controlfp( _PC_24, _MCW_PC|_MCW_RC )&(_MCW_PC|_MCW_RC))!=_PC_24,"Copro SW modifié",return);
}
#endif
*/
#endif //FDE


void SPG_CONV FD_MainStart()
{
#ifdef SPG_General_USEGlobal
	DbgCHECK(Global.Etat!=0,"SPG_WinMainStart");
	SPG_ZeroStruct(Global);
	Global.Etat=SPG_GLOBAL_INITIALISING;
	Global.Size=sizeof(Global);
#endif
#ifdef SPG_DEBUGCONFIG
#ifdef SPG_General_USELOOPTHREAD
//	SPG_SetThreadName(GetCurrentThreadId(),"Main"); //pour l'IDE
#endif
#endif
#ifdef SPG_General_USEGlobal
#ifdef DebugMem
	Global.StackCheckSet=0xAA55AA55;
	Global.StackCheckTest=0xAA55AA55;
#endif
#endif

#ifdef DebugMem
	SPG_MemStateInit(8192,Global.MS);
	SetMemBreakSequence();
#endif
	
	SPG_GET_STACK_INITIAL_POS();//debogage


#ifdef SPG_General_USEFFT
	INITFFT();
#endif

	SPG_RegisterGlobalThreadName("Main",0);

#ifdef SPG_General_USEGlobal
	SPG_List_ResetMsg();
	Global.Etat|=SPG_GLOBAL_OK;
	Global.Etat&=~SPG_GLOBAL_INITIALISING;
#endif


#ifdef SPG_DEBUGCONFIG
	InitHook();
#endif

	return;
}

#ifdef FDE
//Size=0 -> maximized window
//depth=0 -> default
//depth!=0 -> conversion a l'affichage
//si fullscreen!=0 tous les parametres comptent
void SPG_CONV SPG_WinMainStart(int hInstance,
	int SizeX,
	int SizeY,
	int Depth,//8/15/16/24/32
	int ScreenMode,
	int TypeEcran,
	char* WName,
	char* WClassName,
#ifdef SPG_General_USEGlobal
	SPG_WINDOWCALLBACK WCB,
#else
	void* WCB,
#endif
	int FastMath)

{
	//_controlfp( _PC_24, _MCW_PC|_MCW_RC );

#ifdef SPG_General_USEGlobal
	DbgCHECK(Global.Etat != 0, "SPG_WinMainStart");
	SPG_ZeroStruct(Global);
	Global.Etat = SPG_GLOBAL_INITIALISING;
	Global.Size = sizeof(Global);
#endif
#ifdef SPG_DEBUGCONFIG
#ifdef SPG_General_USELOOPTHREAD
	SPG_SetThreadName(GetCurrentThreadId(), "Main"); //pour l'IDE
#endif
#endif
#ifdef SPG_General_USEGlobal
#ifdef DebugMem
	Global.StackCheckSet = 0xAA55AA55;
	Global.StackCheckTest = 0xAA55AA55;
#endif
#endif

#ifdef DebugMem
	SPG_MemStateInit(8192, Global.MS);
	SetMemBreakSequence();
#endif

	SPG_GET_STACK_INITIAL_POS();//debogage


	if (FastMath == 1)
	{
		_control87(_PC_24 | _MCW_EM, _MCW_PC | _MCW_RC | _MCW_EM);
	}
	else if (FastMath == 0)
	{
	}
	else if (FastMath == -1)
	{
#ifndef SPG_General_FastMath
		_control87(_PC_64 | _MCW_EM, _MCW_PC | _MCW_RC | _MCW_EM);
#endif
	}

#ifdef SPG_General_USETimer
#ifndef IntelSpeedStepFix
	S_StartInitTimer();//initialisation timer RDTSC
#endif
#endif

#ifdef SPG_General_PGLib
	Global.common = pglCreateCommon(WName);
	if (SizeX == 0) SizeX = 640;
	if (SizeY == 0) SizeY = 480;
	if (Depth == 0) Depth = 32;
	PGLDisplayParams params = { SizeX,SizeY,Depth,16,0,0,0,0 };
	strcpy(params.title, WName);
	params.flags = PGL_2D | PGL_3D | PGL_OPENGL;
	Global.display = pglCreateDisplay(Global.common, &params);
	pglAttachDisplayToCommon(Global.common, Global.display);
	G_InitEcranFromPGLDisplay(Global.Ecran, Global.display);
#elif defined(SPG_General_USEGlobal)

	int WindowMode = WS_VISIBLE | WS_POPUP;
	if ((ScreenMode&SPG_SM_NoDisplay) == 0)
	{
		SPG_WinMainComputeWindowSize(SizeX, SizeY, Depth, ScreenMode, WindowMode);
	}

	//le CRT utilise GetModuleHandle pour appeler WinMain
	if ((Global.hInstance = hInstance) == 0) Global.hInstance = (int)GetModuleHandle(0);

#ifdef SPG_General_USECDCHECK
	GetModuleFileName((HINSTANCE)Global.hInstance, Global.ProgDir, MaxProgDir - 1);
	strcpy(Global.ProgName, SPG_NameOnly(Global.ProgDir));
	CD_CheckFile(Global.CD_UID, Global.ProgDir);
	/*
	FILE*F=fopen("c:\\SylvainDumpTemp.h","wb+");
	if(F==0) MessageBox(0,"Fichier non cree","SPG_ProgPrincipal",0);
	for(int i=0;i<32;i++)
	{
	char Line[64];
	sprintf(Line,"#define CD_CHECK_KEY_B%i 0x%.02X\r\n",i,Global.CD_UID.B[i]);
	printf(Line);
	fwrite(Line,strlen(Line),1,F);
	}
	fclose(F);
	MessageBox(0,"OK","SPG_ProgPrincipal",0);
	*/
#endif

	Global.WCB = WCB;

	if (((ScreenMode&SPG_SM_NoDisplay) == 0) && WName&&WClassName)
	{
		SPG_WinMainCreateWindow(WindowMode, ScreenMode, Depth, WName, WClassName, hInstance);
		SPG_WinMainCreateEcran(Depth, TypeEcran);
	}
#endif


#ifdef SPG_General_USEGlobal
#ifdef SPG_General_USEFiles
#ifndef SPG_General_USECDCHECK
	GetModuleFileName((HINSTANCE)Global.hInstance, Global.ProgDir, MaxProgDir - 1);
	strcpy(Global.ProgName, SPG_NameOnly(Global.ProgDir));
#endif
	SPG_PathOnly(Global.ProgDir);
	strcpy(Global.LogDir, Global.ProgDir);
	strcat(Global.LogDir, "Log\\FogaleProbe\\");
	CreateDirectory(Global.LogDir, 0);
#endif

#ifdef SPG_General_USEFilesWindows
	strcpy(Global.CurDir, Global.ProgDir);
#endif
#endif //SPG_General_USEGlobal

#ifdef DebugMem
	//	SPG_MemSetHook();
#endif

#ifdef SPG_General_USEAVI
	Global.AVIFPS = SPG_DefaultAVIFPS;
#endif

#ifdef SPG_General_USEFFT
	INITFFT();
#endif

#ifdef SPG_General_USETimer
#ifndef IntelSpeedStepFix
	SPG_Sleep(50);
#else
	int G = GetTickCount();
	S_StartInitTimer();
	G = GetTickCount();
	while ((GetTickCount() - G)<50);
#endif
	S_StopInitTimer();
#endif

	SPG_GlobalLogTimeInit();//doit etre apres S_StopInitTimer
	SPG_RegisterGlobalThreadName("Main", 0);

	if ((ScreenMode&SPG_SM_NoDisplay) == 0)
	{
#ifdef SPG_General_USEGraphics

#ifdef SPG_General_USEGlobal
#ifdef DebugMem
		if (SPG_MemIsBlock(G_GetPix(Global.Ecran))) SPG_SetMemName(G_GetPix(Global.Ecran), "Ecran principal");
#endif
#endif

#ifdef SPG_General_USETimer
		SPG_ArrayStackAlloc(char, Name, 128);
		sprintf(Name, "Global.Ecran CPU %dMHz", (int)(Global.CPUClock / 1000000));
#ifdef DebugMem
		if (Global.Ecran.bmpinf) SPG_SetMemName(Global.Ecran.bmpinf, Name);
#endif
		SPG_ArrayStackCheck(Name);
#endif
#endif
		DoEvents(SPG_DOEV_MIN);
	}

#ifdef DebugProgPrincipalTimer
	Global.TimerUsed = 0;
	S_InitTimer(Global.T_Total, "Total");
	S_InitTimer(Global.T_DoevBlitEcran, "DoevBlit");
	S_InitTimer(Global.T_DoevLock, "DoevLock");
	Global.TimerUsed += 3;
#ifdef DebugRenderTimer
	S_InitTimer(Global.T_SGE_TransformAndRender, "3DRender");
	S_InitTimer(Global.T_SGE_CalcPointsCoord, "-CalcCoord");
	S_InitTimer(Global.T_SGE_RefreshTriBuff, "-Tri");
	S_InitTimer(Global.T_SGE_FinishRender, "-Render");
	S_InitTimer(Global.T_SGE_CalcLight, "--Light");
	S_InitTimer(Global.T_SGE_G_ClearSky, "--ClearSky");
	S_InitTimer(Global.T_SGE_G_Draw, "--Draw");
	Global.TimerUsed += 7;
#endif
#ifdef DebugGraphicsTimer
	S_InitTimer(Global.T_GraphicsRender, "--GraphicsRender");
	S_InitTimer(Global.T_InlinePrepareSegment, "---InlPrepareSeg");
	S_InitTimer(Global.T_InlineRenderSegment, "---InlRenderSeg");
	Global.TimerUsed += 3;
#endif
#ifdef DebugProfilManagerTimer
	S_InitTimer(Global.T_P_Draw, "P_Draw");
	S_InitTimer(Global.T_P8_Draw, "P8_Draw");
	Global.TimerUsed += 2;
#endif
#ifdef DebugNetworkTimer
	S_InitTimer(Global.T_NET_Snd, "NetSnd");
	S_InitTimer(Global.T_NET_Rcv, "NetRcv");
	Global.TimerUsed += 2;
#endif
	S_StartTimer(Global.T_Total);
#endif
	/*
	#ifdef DebugFloat
	SPG_AddUpdateOnDoEvents((SPG_CALLBACK)SPG_CoproStatus,0,0);
	#endif
	*/

#ifdef SPG_General_USECONNEXION
	sciCreateInterfaces(Global.SCI);
#endif


#ifdef SPG_General_USEGlobal
	SPG_List_ResetMsg();
	Global.Etat |= SPG_GLOBAL_OK;
	Global.Etat &= ~SPG_GLOBAL_INITIALISING;
#endif


#ifdef SPG_DEBUGCONFIG
	InitHook();
#endif

	return;
}

void SPG_CONV SPG_WinMainClose()
{
#ifdef SPG_General_USEGlobal
	CHECK(Global.Etat&=SPG_GLOBAL_CLOSING,"SPG_WinMainClose: reentry",return);
	CHECK(Global.Etat&SPG_GLOBAL_OK,"SPG_WinMainClose",;);
	SPG_List_ResetMsg();
	Global.Etat|=SPG_GLOBAL_CLOSING;
	Global.Etat&=~SPG_GLOBAL_OK;
#endif


#ifdef SPG_General_USECONNEXION
	sciDestroyInterfaces(Global.SCI);
#endif


#ifdef DebugProgPrincipalTimer
	S_StopTimer(Global.T_Total);

	S_PrintRatio(&Global.T_Total,Global.TimerUsed);

	S_CloseTimer(Global.T_Total);
	S_CloseTimer(Global.T_DoevBlitEcran);
	S_CloseTimer(Global.T_DoevLock);
#ifdef DebugRenderTimer
	S_CloseTimer(Global.T_SGE_TransformAndRender);
	S_CloseTimer(Global.T_SGE_CalcPointsCoord);
	S_CloseTimer(Global.T_SGE_RefreshTriBuff);
	S_CloseTimer(Global.T_SGE_FinishRender);
	S_CloseTimer(Global.T_SGE_CalcLight);
	S_CloseTimer(Global.T_SGE_G_ClearSky);
	S_CloseTimer(Global.T_SGE_G_Draw);
#endif
#ifdef DebugGraphicsTimer
	S_CloseTimer(Global.T_GraphicsRender);
	S_CloseTimer(Global.T_InlinePrepareSegment);
	S_CloseTimer(Global.T_InlineRenderSegment);
#endif
#ifdef DebugProfilManagerTimer
	S_CloseTimer(Global.T_P_Draw);
	S_CloseTimer(Global.T_P8_Draw);
#endif
#ifdef DebugNetworkTimer
	S_CloseTimer(Global.T_NET_Snd);
	S_CloseTimer(Global.T_NET_Rcv);
#endif
#endif //DebugProgPrincipalTimer

	/*
#ifdef DebugFloat
	SPG_KillUpdateOnDoEventsByAddr((SPG_CALLBACK)SPG_CoproStatus);
#endif
	*/

#ifdef SPG_General_USEFFT
	CLOSEFFT();
#endif

#ifdef SPG_General_USEGEFFECT
	SPG_CloseMotionBlur();
#endif

#ifdef SPG_General_USEAVI
	if(Global.AVISG.Etat)
	{
		SPG_AVI_StreamSave_Stop(Global.AVISG);
	}
#endif
	
#ifdef SPG_General_PGLib
	pglAttachDisplayToCommon(Global.common,0);
	pglDestroyDisplay(Global.display);
	pglDestroyCommon(Global.common);
	pglAllFreed();
#else // SPG_General_PGLib
#ifdef SPG_General_USEGlobal

#ifdef SPG_General_USEGraphics
	if(Global.Ecran.Etat) G_CloseEcran(Global.Ecran);
#endif
	
	if (Global.OldPalette) SelectObject((HDC)Global.HDCWND,Global.OldPalette);
	if (Global.FullScreenPalette) DeleteObject(Global.FullScreenPalette);
	if (Global.Palette) SPG_MemFree(Global.Palette);
	
	DoEvents(SPG_DOEV_MIN|SPG_DOEV_FLUSH_WIN_EVENTS);

	SPG_GlobalLogTimeClose();

#ifdef DebugMem
#ifdef DebugList
		DbgCHECK(Global.NumCallBack,"SPG_WinMainClose: Callback residuelle");
#endif //DebugList
	SPG_MemStateCheck(Global.MS);

	//SPG_ArrayStackAlloc(char,Path,MaxProgDir); SPG_ConcatPath(Path,Global.LogDir,"SPG_Mem_Log_SPG_WinMainClose.txt");
	////Path[-2]=(char)-1;
	//SPG_MemStateDump(Path,Global.MS);
	//SPG_ArrayStackCheck(Path);
	SPG_MemStateClose(Global.MS);
#endif //DebugMem
	
	if(Global.hWndWin)
	{
		ReleaseDC((HWND)Global.hWndWin,(HDC)Global.HDCWND);
		DestroyWindow((HWND)Global.hWndWin);
		CHECK(UnregisterClass(Global.WClassName,(HINSTANCE)Global.hInstance)==0,"SPG_WinMainClose",;);
	}
	SPG_ResetFullScreen();
#endif
#endif // SPG_General_PGLib

#ifdef SPG_General_USEGlobal
	Global.Etat&=~SPG_GLOBAL_CLOSING;
#endif
	return;
}


#endif // SPG_General_USEProgPrincipal
#endif //FDE

#if 0
//Exemple de procedure principale
//include "..\SrcC\SPG.h"
//include "..\SrcC\SPG_SysInc.h"
//librairies kernel32.lib user32.lib gdi32.lib comdlg32.lib vfw32.lib 
//note: les executables doivent se compiler dans le dossier racine, par dans debug ou release
//Project, Settings, Link, Output File

//attention la présence des différents modules est assurée par les defines
//qui sont inclus par SrcC Files\Configuration\SPG_General.h qui inclus l'un des fichiers de Config

//libc: C/C++ code generation Pentium Pro Fastcall 16 octets Single Threaded

int SPG_CONV CallBack(DWORD hwnd, DWORD uMsg, DWORD wParam, DWORD lParam)
{
	return 0;
}

int WINAPI WinMain( HINSTANCE hInstance, 
				   // handle to current instance 
				   
				   HINSTANCE hPrevInstance, 
				   // handle to previous instance 
				   
				   LPSTR lpCmdLine, 
				   // pointer to command line 
				   
				   int nCmdShow 
				   // show state of window 
				   
				   )
{
	SPG_WinMainStart((int)hInstance,0,0,0,SPG_SM_UserFriendly,G_ECRAN_DIBSECT,"ProgPrincipal","SrcC",CallBack);

	SPG_WinMainClose();
	return 0;
}


int __cdecl main()
{
}

int __cdecl main( int argc, char *argv[ ], char *envp[ ] )
{
}

#endif // 0

/*
SrcC
	Configuration

	CHECK

	Memory

	System specific general modules (fichiers, fenetres, timer, config files + dialogbox)

	Log ? -> Connexion

	General format hex, IPv4, float, double, i32, i64, ...

	typedef struct
	{
		int N;//elements
		int Len;//bytes total
		int size;//bytes par element
		int step;//step
	} SPG_ARRAY_DESCR;

	Math
		Newmat
		FFTW
		SPGFFT

	Graphiques

	Video VFW, DShow
	Son Windows

	Connexions (RS,UDP,TCP,Files,Video,Son,Log)


Verifier fonctionnement G_DIBSURFACE

logique de B_Lib a revoir
*/
