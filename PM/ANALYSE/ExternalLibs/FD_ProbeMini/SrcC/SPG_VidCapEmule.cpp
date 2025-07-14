
#include "SPG_General.h"

#ifdef SPG_General_USEVidCap

#include "SPG_Includes.h"
#include "SPG_SysInc.h"
#include <stdlib.h>
#include <string.h>

typedef struct
{
	int X;
	int Y;
	int DeltaX;
	int DeltaY;
	float wr;
	float wv;
	float wb;
	float wx1;
	float wx2;
	float wy1;
	float wy2;
} SVID_O;

#define SVID_NumO 16

typedef struct
{
	HWND hWndParent;
	HDC hDCParent;
	char WClassName[32];
	SVID_O o[SVID_NumO];
	BITMAPINFO BINFO;
} SVID_INTERNAL;

#define SVI (*(SVID_INTERNAL*)SV.SVID_Internal)

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
		WS_VISIBLE|WS_CAPTION|WS_MINIMIZEBOX|WS_SYSMENU ,
		PosX,
		PosY,
		128+2*GetSystemMetrics(SM_CXEDGE)+2,
		128+2*GetSystemMetrics(SM_CYEDGE)+GetSystemMetrics(SM_CYCAPTION)+2,
		NULL,
		NULL,
		(HINSTANCE)Global.hInstance,
		NULL )
		)==0,"SVID_Init",UnregisterClass(SVI.WClassName,(HINSTANCE)Global.hInstance);return 0);
	
	SV.Etat=SVID_OK;
	SVID_Configure(SV,DoConfig,DoConfig,DoConfig);

	for(int i=0;i<SVID_NumO;i++)
	{
		SVI.o[i].wr=(rand()/(float)RAND_MAX);
		SVI.o[i].wv=(rand()/(float)RAND_MAX);
		SVI.o[i].wb=(rand()/(float)RAND_MAX);
		SVI.o[i].wx1=(rand()/(float)RAND_MAX);
		SVI.o[i].wx2=(rand()/(float)RAND_MAX);
		SVI.o[i].wy1=(rand()/(float)RAND_MAX);
		SVI.o[i].wy2=(rand()/(float)RAND_MAX);
	}
	return SV.Etat;
}

void SPG_CONV SVID_Configure(SVID_STATE& SV, int ConfigSource, int ConfigFormat, int ConfigDisplay)
{
	if(SV.Etat==0) return;
	SVI.BINFO.bmiHeader.biBitCount=24;
	SVI.BINFO.bmiHeader.biWidth=320;//640;
	SVI.BINFO.bmiHeader.biHeight=240;//480;
	SetWindowPos(SVI.hWndParent,0,0,0,
		SVI.BINFO.bmiHeader.biWidth+2*GetSystemMetrics(SM_CXEDGE)+2,
		SVI.BINFO.bmiHeader.biHeight+2*GetSystemMetrics(SM_CYEDGE)+GetSystemMetrics(SM_CYCAPTION)+2,
		SWP_DEFERERASE|SWP_NOACTIVATE|SWP_NOCOPYBITS|SWP_NOMOVE|SWP_NOREPOSITION|SWP_NOSENDCHANGING|SWP_NOZORDER);
	SV.SizeX=SVI.BINFO.bmiHeader.biWidth;
	SV.SizeY=SVI.BINFO.bmiHeader.biHeight;
	SV.POCT=SVI.BINFO.bmiHeader.biBitCount/8;
	return;
}

int SPG_CONV SVID_Update(SVID_STATE& SV)
{
	if(SV.Etat==SVID_OK)
	{
		return -1;
	}
	else if(SV.Etat==(SVID_OK|SVID_CAPTURESTREAM))
	{
		float t=((float)GetTickCount())/1000;
		for(int i=0;i<SVID_NumO;i++)
		{
			int X=0.5f*SV.E.SizeX*(1+cos(t*SVI.o[i].wx1)*cos(t*SVI.o[i].wx2));
			int Y=0.5f*SV.E.SizeY*(1+cos(t*SVI.o[i].wy1)*cos(t*SVI.o[i].wy2));
			int R=127*(1+cos(t*SVI.o[i].wr));
			int V=127*(1+cos(t*SVI.o[i].wv));
			int B=127*(1+cos(t*SVI.o[i].wb));
			G_DrawRect(SV.E,X,Y,X+64,Y+64,V_Sature(R,0,255)|(V_Sature(V,0,255)<<8)|(V_Sature(B,0,255)<<16));
		}
		SPG_Sleep(25);
		int R=-1;
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
		SVI.hDCParent=GetDC(SVI.hWndParent);
		G_InitEcranDibSect(SV.E,(SVI.BINFO.bmiHeader.biBitCount+7)/8,SVI.BINFO.bmiHeader.biWidth,SVI.BINFO.bmiHeader.biHeight,(HDC)SVI.hDCParent,0);
		SV.EcranReady=0;
		SV.Etat|=SVID_CAPTURESTREAM;
		return -1;
	}
	return 0;
}

void SPG_CONV SVID_StopContinuousGrab(SVID_STATE& SV)
{
	if(SV.Etat==(SVID_OK|SVID_CAPTURESTREAM))
	{
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
	if(SVI.hWndParent) DestroyWindow(SVI.hWndParent);
	CHECK(UnregisterClass(SVI.WClassName,(HINSTANCE)Global.hInstance)==0,"SVID_Close",;);
	SPG_MemFree(SV.SVID_Internal);
	memset(&SV,0,sizeof(SVID_STATE));
	return;
}

#endif
