
#include "SPG_General.h"

#ifdef SPG_General_USEAVI
#include "SPG_Includes.h"
#include <memory.h>

#include "Config\SPG_Warning.h"
#include <windows.h>
#include <vfw.h>
#pragma comment(lib,"vfw32.lib")

int SPG_CONV SPG_AVI_StreamSave_Start(SPG_AVI_STREAM& SAS, char* FileName, int SizeX, int SizeY, int POCT, int FPS, int YReverse, int hWnd)
{
	memset(&SAS,0,sizeof(SPG_AVI_STREAM));
	SAS.AviSizeX=SizeX&0xfffffff0;
	SAS.AviSizeY=SizeY&0xfffffff0;
	SAS.AviPOCT=SAS.POCT=POCT;
	if(SAS.AviPOCT==4) SAS.AviPOCT=3;
	SAS.YReverse=YReverse;
	SAS.FPS=FPS;
	SAS.DT=1.0f/FPS;
	AVISTREAMINFO psi;
	memset(&psi,0,sizeof(AVISTREAMINFO));
	psi.fccType=streamtypeVIDEO;
	//SAS.psi.fccHandler=0;//mmioFOURCC('I','V','5','1');
	psi.dwScale=1;
	psi.dwRate=FPS;
	psi.dwStart=0;
	psi.rcFrame.right=SAS.AviSizeX;
	psi.rcFrame.bottom=SAS.AviSizeY;
	//SAS.psi.dwQuality=5000;
	AVIFileInit();
	DeleteFile(FileName);
	int FileOpen=AVIFileOpen((PAVIFILE*)&SAS.pfile,FileName,OF_WRITE|OF_CREATE,0);
	SPG_CHECKTWO(FileOpen!=0,"SPG_AVI_StreamSave_Start: AVIFileOpen failed (check .avi filename extension)",FileName,AVIFileExit();memset(&SAS,0,sizeof(SPG_AVI_STREAM));return 0);
	int CreateStream=AVIFileCreateStream((PAVIFILE)SAS.pfile, (PAVISTREAM*)&SAS.pavi, &psi);
	SPG_CHECKTWO(CreateStream!=0,"SPG_AVI_StreamSave_Start: AVIFileCreateStream failed",FileName,AVIFileRelease((PAVIFILE)SAS.pfile);AVIFileExit();memset(&SAS,0,sizeof(SPG_AVI_STREAM));return 0);

	AVICOMPRESSOPTIONS opts;
	AVICOMPRESSOPTIONS* aopts[1] = {&opts};
	memset(&opts, 0, sizeof(opts));
	int SaveOpts=AVISaveOptions((HWND)hWnd, 0, 1, (PAVISTREAM*)&SAS.pavi, (LPAVICOMPRESSOPTIONS*) &aopts);
	SPG_CHECKTWO(SaveOpts!=1,"SPG_AVI_StreamSave_Start: AVISaveOptions failed",FileName,;);
	if(SaveOpts==1)
	{
		int MakeCompressed = AVIMakeCompressedStream((PAVISTREAM*)&SAS.psCompressed, (PAVISTREAM)SAS.pavi, &opts, NULL);
		SPG_CHECKTWO(MakeCompressed!=0,"SPG_AVI_StreamSave_Start: AVIMakeCompressedStream failed",FileName,SAS.psCompressed=0;);
	}

	int PaletteSize=0;
	if(SAS.AviPOCT==1) PaletteSize=1024;
	SAS.binfo=SPG_MemAlloc(sizeof(BITMAPINFO)+PaletteSize,"BINFO AVI");
	if(PaletteSize)
	{
	for(int c=0;c<256;c++)
	{
		((BITMAPINFO*)SAS.binfo)->bmiColors[c].rgbBlue=c;
		((BITMAPINFO*)SAS.binfo)->bmiColors[c].rgbGreen=c;
		((BITMAPINFO*)SAS.binfo)->bmiColors[c].rgbRed=c;
		((BITMAPINFO*)SAS.binfo)->bmiColors[c].rgbReserved=0;
	}
	}
#define binfoH ((BITMAPINFO*)SAS.binfo)->bmiHeader
	binfoH.biSize=40;
	binfoH.biWidth=SAS.AviSizeX;
	binfoH.biHeight=SAS.AviSizeY;
	binfoH.biPlanes=1;
	binfoH.biBitCount=SAS.AviPOCT*8;
	binfoH.biCompression=BI_RGB;
	binfoH.biSizeImage=SAS.AviSizeX*SAS.AviSizeY*SAS.AviPOCT;
	binfoH.biXPelsPerMeter=0;
	binfoH.biYPelsPerMeter=0;
	if(SAS.AviPOCT==1)
		binfoH.biClrUsed=256;
	binfoH.biClrImportant=0;
	int SetFormat=-1;

	if(SAS.psCompressed)
	{
		SetFormat=AVIStreamSetFormat((PAVISTREAM)SAS.psCompressed,0,SAS.binfo,40+PaletteSize);
	}
	else
	{
		SetFormat=AVIStreamSetFormat((PAVISTREAM)SAS.pavi,0,SAS.binfo,40+PaletteSize);
	}
	SPG_CHECK(SetFormat!=0,"SPG_AVI_StreamSave_Start: AVIStreamSetFormat failed",SPG_AVI_StreamSave_Stop(SAS);return 0;);

	SAS.AviPitch=SAS.AviSizeX*SAS.AviPOCT;
	SAS.FrameSize=SAS.AviPitch*SAS.AviSizeY;
	SAS.OneFrame=SPG_TypeAlloc(SAS.FrameSize,BYTE,"BINFO FRAME");

	return SAS.Etat=-1;
}

void SPG_CONV SPG_AVI_StreamSave_Stop(SPG_AVI_STREAM& SAS)
{
	if(SAS.OneFrame) SPG_MemFree(SAS.OneFrame);
	if(SAS.psCompressed) AVIStreamRelease((PAVISTREAM)SAS.psCompressed);
	if(SAS.pavi) AVIStreamRelease((PAVISTREAM)SAS.pavi);
	if(SAS.binfo) SPG_MemFree(SAS.binfo);
	if(SAS.pfile) AVIFileRelease((PAVIFILE)SAS.pfile);
	AVIFileExit();
	memset(&SAS,0,sizeof(SPG_AVI_STREAM));
	return;
}

int SPG_CONV SPG_AVI_StreamSave_AddFrame(SPG_AVI_STREAM& SAS, BYTE* E, int Pitch)
{
	SPG_CHECK(SAS.Etat==0,"SPG_AVI_StreamSave_AddFrame",return 0;);
	SPG_CHECK(E==0,"SPG_AVI_StreamSave_AddFrame",return 0;);

	BYTE* OneFrame=SAS.OneFrame;

	if((SAS.POCT==SAS.AviPOCT)&&(Pitch==SAS.AviPitch)&&(SAS.YReverse==0))
	{
		OneFrame=E;
	}
	else if(SAS.POCT==SAS.AviPOCT)
	{
		if(SAS.YReverse)
		{
			for(int y=0;y<SAS.AviSizeY;y++)
			{
				memcpy(SAS.OneFrame+SAS.AviPitch*y,E+(SAS.AviSizeY-1-y)*Pitch,SAS.AviSizeX*SAS.POCT);
			}
		}
		else
		{
			for(int y=0;y<SAS.AviSizeY;y++)
			{
				memcpy(SAS.OneFrame+SAS.AviPitch*y,E+y*Pitch,SAS.AviSizeX*SAS.POCT);
			}
		}
	}
	else if((SAS.POCT==4)&&(SAS.AviPOCT==3))
	{
		if(SAS.YReverse)
		{
			for(int y=0;y<SAS.AviSizeY;y++)
			{
				int DstL=SAS.AviPitch*y;
				int SrcL=(SAS.AviSizeY-1-y)*Pitch;
				for(int x=0;x<SAS.AviSizeX;x++)
				{
					SAS.OneFrame[DstL+3*x]=E[SrcL+4*x];
					SAS.OneFrame[DstL+3*x+1]=E[SrcL+4*x+1];
					SAS.OneFrame[DstL+3*x+2]=E[SrcL+4*x+2];
				}
			}
		}
		else
		{
			for(int y=0;y<SAS.AviSizeY;y++)
			{
				int DstL=SAS.AviPitch*y;
				int SrcL=y*Pitch;
				for(int x=0;x<SAS.AviSizeX;x++)
				{
					SAS.OneFrame[DstL+3*x]=E[SrcL+4*x];
					SAS.OneFrame[DstL+3*x+1]=E[SrcL+4*x+1];
					SAS.OneFrame[DstL+3*x+2]=E[SrcL+4*x+2];
				}
			}
		}
	}
	/*
	//format invalide
	else
	{
	}
	*/
	long sm=0;//valeurs de retour de AVIStreamWrite pour le test d'erreur
	long sb=0;
	int zero=-1;
	if(SAS.psCompressed)
	{
		zero=AVIStreamWrite((PAVISTREAM)SAS.psCompressed,SAS.FrameNumber,1,OneFrame,SAS.FrameSize,0,&sm,&sb);
	}
	else
	{
		zero=AVIStreamWrite((PAVISTREAM)SAS.pavi,SAS.FrameNumber,1,OneFrame,SAS.FrameSize,0,&sm,&sb);
	}
	SAS.FrameNumber++;
	return zero==0;
}

int SPG_CONV SPG_AVI_Save(char* FileName, BYTE* E, int SizeX, int SizeY, int Pitch, int SizeP, int POCT, int NumS, int FPS, int YReverse, int hWnd)
{
	SPG_CHECKTWO(E==0,"SPG_AVI_Save",FileName,return 0;);
	SPG_AVI_STREAM SAS;
	if(SPG_AVI_StreamSave_Start(SAS,FileName,SizeX,SizeY,POCT,FPS,YReverse,hWnd))
	{
		for(int i=0;i<NumS;i++)
		{
			SPG_AVI_StreamSave_AddFrame(SAS,E+i*SizeP,Pitch);
		}
		SPG_AVI_StreamSave_Stop(SAS);
		return -1;
	}
	return 0;
}


int SPG_CONV SPG_AVI_StreamLoad_Start(SPG_AVI_STREAM& SAS, char* FileName, int& SizeX, int& SizeY, int& POCT, int& NumS, int& FPS, int YReverse)
{
	memset(&SAS,0,sizeof(SPG_AVI_STREAM));
	AVIFileInit();
	int FileOpen=AVIFileOpen((PAVIFILE*)&SAS.pfile,FileName,OF_READ,0);
	SPG_CHECKTWO(FileOpen!=0,"SPG_AVI_StreamLoad_Start: AVIFileOpen failed (check .avi filename extension)",FileName,AVIFileExit();memset(&SAS,0,sizeof(SPG_AVI_STREAM));return 0);
	int CreateStream=AVIFileGetStream((PAVIFILE)SAS.pfile, (PAVISTREAM*)&SAS.pavi, streamtypeVIDEO, 0);
	SPG_CHECKTWO(CreateStream!=0,"AVIFileGetStream failed",FileName,AVIFileRelease((PAVIFILE)SAS.pfile);AVIFileExit();memset(&SAS,0,sizeof(SPG_AVI_STREAM));return 0);
	AVISTREAMINFO psi;
	memset(&psi,0,sizeof(AVISTREAMINFO));
	SPG_CHECKTWO(AVIStreamInfo((PAVISTREAM)SAS.pavi,&psi,sizeof(AVISTREAMINFO))!=0,"SPG_AVI_StreamLoad_Start: AVIStreamInfo failed",FileName,
		AVIStreamRelease((PAVISTREAM)SAS.pavi);AVIFileRelease((PAVIFILE)SAS.pfile);AVIFileExit();memset(&SAS,0,sizeof(SPG_AVI_STREAM));return 0);

	FPS=SAS.FPS=psi.dwRate/psi.dwScale;
	SAS.DT=1.0f/FPS;
	SizeX=SAS.AviSizeX=psi.rcFrame.right;
	SizeY=SAS.AviSizeY=psi.rcFrame.bottom;
	POCT=SAS.AviPOCT=SAS.POCT=3;
	NumS=psi.dwLength;
	SAS.YReverse=YReverse;
	//AVIStreamGetFrameOpen((PAVIFILE)SAS.pfile, 

	SAS.binfo=SPG_MemAlloc(sizeof(BITMAPINFO),"BINFO AVI");
	binfoH.biSize=40;
	binfoH.biWidth=SAS.AviSizeX;
	binfoH.biHeight=SAS.AviSizeY;
	binfoH.biPlanes=1;
	binfoH.biBitCount=SAS.AviPOCT*8;
	binfoH.biCompression=BI_RGB;
	binfoH.biSizeImage=SAS.AviSizeX*SAS.AviSizeY*SAS.AviPOCT;
	binfoH.biXPelsPerMeter=0;
	binfoH.biYPelsPerMeter=0;

	SAS.GFrame=(void*)AVIStreamGetFrameOpen((PAVISTREAM)SAS.pavi,(BITMAPINFOHEADER*)SAS.binfo);
	SPG_CHECKTWO(SAS.GFrame==0,"SPG_AVI_StreamLoad_Start: AVIStreamGetFrameOpen failed",FileName,
		AVIStreamRelease((PAVISTREAM)SAS.pavi);AVIFileRelease((PAVIFILE)SAS.pfile);SPG_MemFree(SAS.binfo);AVIFileExit();memset(&SAS,0,sizeof(SPG_AVI_STREAM));return 0);

	return SAS.Etat=-1;
}

void SPG_CONV SPG_AVI_StreamLoad_Stop(SPG_AVI_STREAM& SAS)
{
	if(SAS.GFrame) AVIStreamGetFrameClose((PGETFRAME)SAS.GFrame);
	if(SAS.pavi) AVIStreamRelease((PAVISTREAM)SAS.pavi);
	if(SAS.binfo) SPG_MemFree(SAS.binfo);
	if(SAS.pfile) AVIFileRelease((PAVIFILE)SAS.pfile);
	AVIFileExit();
	memset(&SAS,0,sizeof(SPG_AVI_STREAM));
	return;
}

int SPG_CONV SPG_AVI_StreamLoad_AddFrame(SPG_AVI_STREAM& SAS, BYTE* E, int Pitch)
{
	SPG_CHECK(SAS.Etat==0,"SPG_AVI_StreamLoad_AddFrame",return 0;);
	SPG_CHECK(E==0,"SPG_AVI_StreamLoad_AddFrame",return 0;);
	SAS.OneFrame=(BYTE*)AVIStreamGetFrame((PGETFRAME)SAS.GFrame,SAS.FrameNumber++)+sizeof(BITMAPINFOHEADER);
	SPG_CHECK(SAS.OneFrame==0,"SPG_AVI_StreamLoad_AddFrame: AVIStreamGetFrame failed",return 0);

	if(SAS.YReverse)
	{
		for(int y=0;y<SAS.AviSizeY;y++)
		{
			memcpy(E+y*Pitch,SAS.OneFrame+(SAS.AviSizeY-1-y)*SAS.AviSizeX*SAS.AviPOCT,SAS.AviSizeX*SAS.AviPOCT);
		}
	}
	else
	{
		for(int y=0;y<SAS.AviSizeY;y++)
		{
			memcpy(E+y*Pitch,SAS.OneFrame+y*SAS.AviSizeX*SAS.AviPOCT,SAS.AviSizeX*SAS.AviPOCT);
		}
	}
	return -1;
}

int SPG_CONV SPG_AVI_Load(char* FileName, BYTE*& E, int& SizeX, int& SizeY, int& POCT, int& NumS, int& FPS, int YReverse)
{
	SPG_AVI_STREAM SAS;
	if(SPG_AVI_StreamLoad_Start(SAS,FileName,SizeX,SizeY,POCT,NumS,FPS,YReverse))
	{
		int Pitch=SizeX*POCT;
		int SizeP=Pitch*SizeY;
		E=SPG_TypeAlloc(SizeP*NumS,BYTE,"SPG_AVI_Load");
		for(int i=0;i<NumS;i++)
		{
			SPG_AVI_StreamLoad_AddFrame(SAS,E+i*SizeP,Pitch);
		}
		SPG_AVI_StreamLoad_Stop(SAS);
		return -1;
	}
	return 0;
}
#endif

