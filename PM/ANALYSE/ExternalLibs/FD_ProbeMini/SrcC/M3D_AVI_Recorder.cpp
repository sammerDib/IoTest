
#include "../StdAfx.h"

#ifdef UseAVI

#include "SPG.h"

extern TypApplication a;

int M3D_AVI_RECORDER_Init(M3D_AVI_RECORDER& MAVI, int FPS, HWND hWnd, char* FileName, int SizeX, int SizeY, int PosX, int PosY)
{
	CHECK(MAVI.Etat,"M3D_AVI_RECORDER_Init",return 0;);
	memset(&MAVI,0,sizeof(M3D_AVI_RECORDER));
	MAVI.SPG_AviStream=memAllocZ(sizeof(SPG_AVI_STREAM),"M3D_AVI");
	if(hWnd==0)
	{
		MAVI.SizeX=GetSystemMetrics(SM_CXSCREEN);//-64;
		MAVI.SizeY=GetSystemMetrics(SM_CYSCREEN);//-128;
		MAVI.PosX=0;//GetSystemMetrics(SM_CXSCREEN);//-32;
		MAVI.PosY=0;//GetSystemMetrics(SM_CYSCREEN);//-64;
		MAVI.wndDesktop=GetDesktopWindow();
		MAVI.dcDesktop=GetDC(MAVI.wndDesktop);
	}
	else
	{
		MAVI.wndDesktop=hWnd;
		MAVI.dcDesktop=GetDC(MAVI.wndDesktop);
		MAVI.SizeX=SizeX;//(GetDeviceCaps((HDC)MAVI.dcDesktop,HORZRES)&0xFFFFF0)-64;
		MAVI.SizeY=SizeY;//(GetDeviceCaps((HDC)MAVI.dcDesktop,VERTRES)&0xFFFFF0)-128;
		MAVI.PosX=PosX;
		MAVI.PosY=PosY;
		//MAVI.POCT=GetDeviceCaps((HDC)MAVI.dcDesktop,BITSPIXEL)/8;
	}
	MAVI.POCT=GetDeviceCaps((HDC)MAVI.dcDesktop,BITSPIXEL)/8;
	MAVI.dcBitmap=CreateCompatibleDC(MAVI.dcDesktop);
	MAVI.binfo.bmiHeader.biSize=sizeof(BITMAPINFO);
	MAVI.binfo.bmiHeader.biBitCount=8*MAVI.POCT;
	MAVI.binfo.bmiHeader.biWidth=MAVI.SizeX;
	MAVI.binfo.bmiHeader.biHeight=MAVI.SizeY;
	MAVI.binfo.bmiHeader.biPlanes=1;
	MAVI.binfo.bmiHeader.biSizeImage=MAVI.SizeX*MAVI.SizeY*MAVI.POCT;
	MAVI.Pitch=MAVI.SizeX*MAVI.POCT;

	MAVI.cBitmap=CreateDIBSection(
		MAVI.dcBitmap,
		&MAVI.binfo,
		DIB_RGB_COLORS,
		(void**)&MAVI.E,0,0);
	SelectObject(MAVI.dcBitmap,MAVI.cBitmap);

	if (!SPG_AVI_StreamSave_Start(
		*(SPG_AVI_STREAM*)MAVI.SPG_AviStream,
		FileName,
		MAVI.SizeX,MAVI.SizeY,MAVI.POCT,FPS))
	{
		memFree(MAVI.SPG_AviStream);
		return 0;
	}

	MAVI.StartDelay = 2;
	MAVI.Etat = -1;

	return MAVI.Etat;
}

void M3D_AVI_RECORDER_Update(M3D_AVI_RECORDER& MAVI)
{
	if(MAVI.Etat)
	{
		if(MAVI.StartDelay)
		{
			MAVI.StartDelay--;
		}
		else
		{
			CHECK(BitBlt(MAVI.dcBitmap,0,0,MAVI.SizeX,MAVI.SizeY,MAVI.dcDesktop,MAVI.PosX,MAVI.PosY,SRCCOPY)==0,"M3D_AVI_RECORDER_Update",return;);
			SPG_AVI_StreamSave_AddFrame(*(SPG_AVI_STREAM*)MAVI.SPG_AviStream,MAVI.E,MAVI.Pitch);
		}
	}
}

void M3D_AVI_RECORDER_Close(M3D_AVI_RECORDER& MAVI)
{
	if(MAVI.Etat)
	{
		MAVI.Etat=0;
		SPG_AVI_StreamSave_Stop(*(SPG_AVI_STREAM*)MAVI.SPG_AviStream);
		memFree(MAVI.SPG_AviStream);
		SelectObject(MAVI.dcBitmap,0);
		DeleteObject(MAVI.cBitmap);
		DeleteDC(MAVI.dcBitmap);
		ReleaseDC(MAVI.wndDesktop,MAVI.dcDesktop);
		logMessage(LOG_GENERAL, LOG_INFO, 0, "M3D AVI file saved");
	}
	return;
}

#endif

