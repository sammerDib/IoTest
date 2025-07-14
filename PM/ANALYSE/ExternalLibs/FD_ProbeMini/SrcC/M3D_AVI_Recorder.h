
#ifndef __M3D_AVI_RECORDER_H__
#define __M3D_AVI_RECORDER_H__
#ifdef UseAVI

typedef struct
{
	int Etat;
	int SizeX;
	int SizeY;
	int Pitch;
	int PosX;
	int PosY;
	int POCT;
	int StartDelay;
	void* SPG_AviStream;
	BYTE* E;
	HWND wndDesktop;
	HDC dcDesktop;
	HDC dcBitmap;
	HBITMAP cBitmap;
	BITMAPINFO binfo;
} M3D_AVI_RECORDER;

int M3D_AVI_RECORDER_Init(M3D_AVI_RECORDER& MAVI, int FPS=12, HWND hWnd=0, char* FileName="M3D_AVI.avi", int SizeX=0, int SizeY=0, int PosX=0, int PosY=0);
void M3D_AVI_RECORDER_Update(M3D_AVI_RECORDER& MAVI);
void M3D_AVI_RECORDER_Close(M3D_AVI_RECORDER& MAVI);

// #pragma message("Using M3D AVI RECORDER")

#endif // def  (UseAVI)
#endif // ndef (__M3D_AVI_RECORDER_H__)
