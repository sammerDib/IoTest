
#include "SPG_General.h"

#ifdef SPG_General_USEVidCap

#include "SPG_Includes.h"
//nclude "SPG_SysInc.h"
#include <memory.h>

#include <windows.h>
#include <vfw.h>
#include <string.h>
//vfw32.lib

typedef struct
{
#ifndef SPG_General_USEVidCapEmule
	CAPSTATUS      CapStatus ;
	CAPDRIVERCAPS  CapDriverCaps ;
	CAPTUREPARMS   CaptureParams ;
#endif
	HWND hWndParent;
	HDC hDCParent;
#ifndef SPG_General_USEVidCapEmule
	HWND hWndCap;
#endif
	char WClassName[32];
	BITMAPINFO BINFO;
} SVID_INTERNAL;

#define SVI (*(SVID_INTERNAL*)SV.SVID_Internal)

#define SleepEvents() {for(int i=0;i<5;i++) {DoEvents(SPG_DOEV_READ_WIN_EVENTS|SPG_DOEV_FLUSH_WIN_EVENTS);Sleep(5);}}

LRESULT CALLBACK capVideoStreamCallback(
  HWND hWnd,         
  LPVIDEOHDR lpVHdr  
)
{
	SVID_STATE& SV=*(SVID_STATE*)capGetUserData(hWnd);//(lpVHdr->dwUser);
	if(&SV)
	{
		if(SV.Etat&SVID_CAPTURESTREAM)
		{
	//seul le format video RGB24 est supporté
			if(SV.ReverseY)
			{
				for(int y=0;y<V_Min(SV.E.SizeY,lpVHdr->dwBytesUsed/SV.E.Pitch);y++)
				{
					memcpy(SV.E.MECR+(SV.E.SizeY-1-y)*SV.E.Pitch,lpVHdr->lpData+y*SV.E.Pitch,SV.E.Pitch);
				}
			}
			else
			{
				memcpy(SV.E.MECR,lpVHdr->lpData,V_Min(lpVHdr->dwBytesUsed,SV.E.Pitch*SV.E.SizeY));
			}
			SV.EcranReady=-1;
		}
	}
	return 0;
}
 
//la callback windows de fenetre
LRESULT CALLBACK SVID_WindowProc( HWND hwnd, // handle of window 
							UINT uMsg, // message identifier 
							WPARAM wParam,  // first message parameter 
							LPARAM lParam  // second message parameter 
							)
{
	switch(uMsg)
	{
		//case WM_CREATE:
		//	return 0;
		case WM_ERASEBKGND:
			return 0;
	}
	return DefWindowProc(hwnd,uMsg,wParam,lParam);
}

int SPG_CONV SVID_Init(SVID_STATE& SV, int PosX, int PosY, int capDeviceIndex, int DoConfig, int ReverseY)
{
	memset(&SV,0,sizeof(SVID_STATE));
	SV.SVID_Internal=SPG_MemAlloc(sizeof(SVID_INTERNAL),"SVID_Init");

	strcpy(SVI.WClassName,"SPG_VIDEO");
	
	WNDCLASSEX  wcex;

	wcex.cbSize=sizeof(WNDCLASSEX);
	wcex.style=CS_NOCLOSE;
	wcex.lpfnWndProc=(WNDPROC)SVID_WindowProc;
	wcex.cbClsExtra=0;
	wcex.cbWndExtra=0;
	wcex.hInstance=(HINSTANCE)Global.hInstance;
	wcex.hIcon=0;//LoadIcon(GetModuleHandle(0),MAKEINTRESOURCE(IDI_ICON));
	wcex.hCursor=LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground=0;//(HBRUSH)COLOR_WINDOW;
	wcex.lpszMenuName=NULL;
	wcex.lpszClassName=SVI.WClassName;
	wcex.hIconSm=wcex.hIcon;
	
	CHECK(RegisterClassEx(&wcex)==0,"SVID_Init",return 0);
	//WS_EX_TOOLWINDOW
	//WS_VISIBLE|WS_POPUP
	CHECK((
		SVI.hWndParent = CreateWindowEx(WS_EX_APPWINDOW,
		wcex.lpszClassName,
		"SPG_VIDEO",
		WS_CAPTION|WS_MINIMIZEBOX|WS_SYSMENU,//|WS_VISIBLE
		PosX,
		PosY,
		128+2*GetSystemMetrics(SM_CXEDGE)+2,
		128+2*GetSystemMetrics(SM_CYEDGE)+GetSystemMetrics(SM_CYCAPTION)+2,
		NULL,
		NULL,
		(HINSTANCE)Global.hInstance,
		NULL )
		)==0,"SVID_Init",UnregisterClass(SVI.WClassName,(HINSTANCE)Global.hInstance);SPG_MemFree(SV.SVID_Internal);return 0);
	
    SVI.hWndCap = capCreateCaptureWindow(
                    "SPG_VIDEO",
                    WS_CHILD|WS_VISIBLE,
                    0, 0, 
					128, 
					128,//160, 120,
                    SVI.hWndParent,               // parent window
                    1                   // child window id
              );
	SV.ReverseY=ReverseY;
	SV.Etat=SVID_OK;
	CHECK(SVI.hWndCap==0,"SVID_Init",SVID_Close(SV);return 0);
	SleepEvents();
    //capSetCallbackOnError(SV.hWndCap, SVID_ErrorCallback) ;
	//capSetCallbackOnStatus(SV.hWndCap, SVID_StatusCallback) ;
	CHECK((capDriverConnect(SVI.hWndCap, capDeviceIndex))==0,"SVID_Init",SVID_Close(SV);return 0);
	SleepEvents();
    capDriverGetCaps(SVI.hWndCap, &(SVI.CapDriverCaps), sizeof(CAPDRIVERCAPS)) ;
	SVID_Configure(SV,DoConfig,DoConfig,DoConfig);
	return SV.Etat;
}

void SPG_CONV SVID_Configure(SVID_STATE& SV, int ConfigSource, int ConfigFormat, int ConfigDisplay)
{
	if(SV.Etat==0) return;
	int MustRestartStream=0;
	SleepEvents();
	if(SV.Etat&SVID_CAPTURESTREAM)
	{
		SVID_StopContinuousGrab(SV);
		MustRestartStream=1;
	}
	SleepEvents();
	if(SVI.CapDriverCaps.fHasDlgVideoSource&&ConfigSource)
	{
        capDlgVideoSource(SVI.hWndCap);
		SleepEvents();
	}
    if(SVI.CapDriverCaps.fHasDlgVideoFormat&&ConfigFormat) 
	{
		capDlgVideoFormat(SVI.hWndCap);
		SleepEvents();
	}
    if(SVI.CapDriverCaps.fHasDlgVideoDisplay&&ConfigDisplay) 
	{
		capDlgVideoDisplay(SVI.hWndCap);
		SleepEvents();
	}

    capGetStatus(SVI.hWndCap, &SVI.CapStatus , sizeof(CAPSTATUS));

	SetWindowPos(SVI.hWndParent,0,0,0,
		SVI.CapStatus.uiImageWidth+2*GetSystemMetrics(SM_CXEDGE)+2,
		SVI.CapStatus.uiImageHeight+2*GetSystemMetrics(SM_CYEDGE)+GetSystemMetrics(SM_CYCAPTION)+2,
		SWP_DEFERERASE|SWP_NOACTIVATE|SWP_NOCOPYBITS|SWP_NOMOVE|SWP_NOREPOSITION|SWP_NOSENDCHANGING|SWP_NOZORDER);
	SetWindowPos(SVI.hWndCap,0,0,0,SVI.CapStatus.uiImageWidth,SVI.CapStatus.uiImageHeight,
		SWP_DEFERERASE|SWP_NOACTIVATE|SWP_NOCOPYBITS|SWP_NOMOVE|SWP_NOREPOSITION|SWP_NOREDRAW|SWP_NOSENDCHANGING|SWP_NOZORDER);

	capGetVideoFormat(SVI.hWndCap, &SVI.BINFO, sizeof(BITMAPINFO));

	SV.SizeX=SVI.BINFO.bmiHeader.biWidth;
	SV.SizeY=SVI.BINFO.bmiHeader.biHeight;
	SV.POCT=SVI.BINFO.bmiHeader.biBitCount/8;

	capCaptureGetSetup(SVI.hWndCap,&SVI.CaptureParams,sizeof(CAPTUREPARMS));

	capSetUserData(SVI.hWndCap,&SV);

	if(MustRestartStream)
	{
		SVID_StartContinuousGrab(SV);
	}
	return;
}

int SPG_CONV SVID_Update(SVID_STATE& SV)
{
	if(SV.Etat==SVID_OK)
	{
		capGrabFrame(SVI.hWndCap);
		return -1;
	}
	else if(SV.Etat==(SVID_OK|SVID_CAPTURESTREAM))
	{
		int R=SV.EcranReady;
		SV.EcranReady=0;
		return R;
	}
	else
		return 0;
}

int SPG_CONV SVID_StartContinuousGrab(SVID_STATE& SV)
{
	if(SV.Etat==SVID_OK)
	{
		SleepEvents();
			CHECK(
		capSetCallbackOnVideoStream(SVI.hWndCap, capVideoStreamCallback)==0,
		"SVID_Configure",;);


		SVI.CaptureParams.fAbortLeftMouse=0;
		SVI.CaptureParams.fAbortRightMouse=0;
		SVI.CaptureParams.fCaptureAudio=0;
		SVI.CaptureParams.fLimitEnabled=0;
		SVI.CaptureParams.fMakeUserHitOKToCapture=0;
		SVI.CaptureParams.fStepCaptureAt2x=0;
		SVI.CaptureParams.fYield=-1;
		SVI.CaptureParams.vKeyAbort=0;
		SVI.CaptureParams.wPercentDropForError=0xFFFFFFFF;
		SVI.CaptureParams.wTimeLimit=0;
		SVI.CaptureParams.dwRequestMicroSecPerFrame=10000;//10ms=100FPS
		CHECK(
			capCaptureSetSetup(SVI.hWndCap,&SVI.CaptureParams,sizeof(CAPTUREPARMS))==0,
			"SVID_StartContinuousGrab",return 0);
		SleepEvents();
	//seul le RGB24 est supporté
	//G_InitEcranWindows(SVI.E,(SVI.BINFO.bmiHeader.biBitCount+7)/8,SVI.BINFO.bmiHeader.biWidth,SVI.BINFO.bmiHeader.biHeight,(HDC)Global.HDCWND);
	SVI.hDCParent=GetDC(SVI.hWndParent);
	G_InitEcranDibSect(SV.E,(SVI.BINFO.bmiHeader.biBitCount+7)/8,SVI.BINFO.bmiHeader.biWidth,SVI.BINFO.bmiHeader.biHeight,(HDC)SVI.hDCParent,0);
		SV.EcranReady=0;
		SV.Etat|=SVID_CAPTURESTREAM;
		CHECK(
			capCaptureSequenceNoFile(SVI.hWndCap)==0,
			"SVID_StartContinuousGrab",SVID_StopContinuousGrab(SV);return 0);
		return -1;
	}
	return 0;
}

void SPG_CONV SVID_StopContinuousGrab(SVID_STATE& SV)
{
	if(SV.Etat==(SVID_OK|SVID_CAPTURESTREAM))
	{
		SleepEvents();
		capCaptureStop(SVI.hWndCap);
		SleepEvents();
		//CHECK(
		capSetCallbackOnVideoStream(SVI.hWndCap, 0);//==0,
		//"SVID_StopContinuousGrab",;);

		if(SV.E.Etat) G_CloseEcran(SV.E);
		ReleaseDC(SVI.hWndParent,SVI.hDCParent);
		SV.EcranReady=0;
		SV.Etat&=~SVID_CAPTURESTREAM;
	}
	return;
}

void SPG_CONV SVID_Close(SVID_STATE& SV)
{
	CHECK(SV.Etat==0,"SVID_Close",return);
	if(SV.Etat&SVID_CAPTURESTREAM)
	{
		SVID_StopContinuousGrab(SV);
	}
	//G_CloseEcran(SV.E);
    //capSetCallbackOnError(SV.hWndCap, NULL) ;
    //capSetCallbackOnStatus(SV.hWndCap, NULL) ;
    //capSetCallbackOnYield(SV.hWndCap, NULL) ;
	SleepEvents();
    if(SVI.hWndCap) capDriverDisconnect (SVI.hWndCap);
	SleepEvents();
	if(SVI.hWndCap) DestroyWindow(SVI.hWndCap);
	SleepEvents();
	if(SVI.hWndParent) DestroyWindow(SVI.hWndParent);
	CHECK(UnregisterClass(SVI.WClassName,(HINSTANCE)Global.hInstance)==0,"SVID_Close",;);
	SPG_MemFree(SV.SVID_Internal);
	memset(&SV,0,sizeof(SVID_STATE));
	return;
}

#endif
