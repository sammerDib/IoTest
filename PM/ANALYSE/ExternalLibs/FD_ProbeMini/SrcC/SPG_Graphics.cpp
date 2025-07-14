 
#include "SPG_General.h"

#ifdef SPG_General_USEGraphics

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <memory.h>

#ifdef G_ECRAN_DRAWDIB
#pragma comment(lib,"vfw32.lib")
#endif

//nclude <stdio.h>
//nclude <string.h>


//#include "BRES.h"

/*
#ifdef SPG_General_PGLib
void SPG_FASTCONV G_LockEcran(G_Ecran& E)
{
	CHECK(E.surface==0,"surface inexistante",return);
	CHECK(E.Etat&G_MEMORYAVAILABLE,"Surface deja lockee",return);
	E.MECR=pglLockSurface(E.surface);
	E.Pitch=E.surface->pitch;
	E.Etat|=G_MEMORYAVAILABLE;
	return;
}

void SPG_FASTCONV G_UnlockEcran(G_Ecran& E)
{
	CHECK(E.surface==0,"surface inexistante",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"Surface deja unlockee",return);
	pglUnlockSurface(E.surface);
	E.Etat&=~G_MEMORYAVAILABLE;
	return;
}
#endif
*/

void SPG_CONV G_FillBitmapInfoUnsafe(void* BitmapInfo, G_Ecran& E, int YReverse)
{
	CHECK(BitmapInfo==0,"G_FillBitmapInfo",return);
	CHECK(E.Etat==0,"G_FillBitmapInfo",return);
	BITMAPINFO* bmpinf=(BITMAPINFO*)BitmapInfo;
	if(E.POCT==1)
	{
		memset(BitmapInfo,0,sizeof(BITMAPINFOHEADER)+1024);
		bmpinf->bmiHeader.biSize=40;//+256*4;//sizeof(BITMAPINFOHEADER);
		bmpinf->bmiHeader.biWidth=E.SizeX;
		bmpinf->bmiHeader.biHeight=YReverse?-E.SizeY:E.SizeY;
		bmpinf->bmiHeader.biPlanes=1;
		bmpinf->bmiHeader.biBitCount=(WORD)(E.POCT*8);
		for(int cc=0;cc<256;cc++)
		{
			bmpinf->bmiColors[cc].rgbBlue=(BYTE)cc;
			bmpinf->bmiColors[cc].rgbGreen=(BYTE)cc;
			bmpinf->bmiColors[cc].rgbRed=(BYTE)cc;
			bmpinf->bmiColors[cc].rgbReserved=0;
		}
	}
	else
	{
		memset(BitmapInfo,0,sizeof(BITMAPINFOHEADER));
		bmpinf->bmiHeader.biSize=40;//sizeof(BITMAPINFOHEADER);
		bmpinf->bmiHeader.biWidth=E.SizeX;
		bmpinf->bmiHeader.biHeight=YReverse?-E.SizeY:E.SizeY;
		bmpinf->bmiHeader.biPlanes=1;
		bmpinf->bmiHeader.biBitCount=(WORD)(E.POCT*8);
	}
	return;
}

void SPG_CONV G_FillBitmapInfo(void* BitmapInfo, G_Ecran& E, int YReverse)
{
	CHECK(BitmapInfo==0,"G_FillBitmapInfo",return);
	CHECK(E.Etat==0,"G_FillBitmapInfo",return);
	BITMAPINFO* bmpinf=(BITMAPINFO*)BitmapInfo;
	if(E.POCT==1)
	{
		SPG_Memset(BitmapInfo,0,sizeof(BITMAPINFOHEADER)+1024);
		bmpinf->bmiHeader.biSize=40;//+256*4;//sizeof(BITMAPINFOHEADER);
		bmpinf->bmiHeader.biWidth=E.SizeX;
		bmpinf->bmiHeader.biHeight=YReverse?-E.SizeY:E.SizeY;
		bmpinf->bmiHeader.biPlanes=1;
		bmpinf->bmiHeader.biBitCount=(WORD)(E.POCT*8);
		for(int cc=0;cc<256;cc++)
		{
			bmpinf->bmiColors[cc].rgbBlue=(BYTE)cc;
			bmpinf->bmiColors[cc].rgbGreen=(BYTE)cc;
			bmpinf->bmiColors[cc].rgbRed=(BYTE)cc;
			bmpinf->bmiColors[cc].rgbReserved=0;
		}
	}
	else
	{
		SPG_Memset(BitmapInfo,0,sizeof(BITMAPINFOHEADER));
		bmpinf->bmiHeader.biSize=40;//sizeof(BITMAPINFOHEADER);
		bmpinf->bmiHeader.biWidth=E.SizeX;
		bmpinf->bmiHeader.biHeight=YReverse?-E.SizeY:E.SizeY;
		bmpinf->bmiHeader.biPlanes=1;
		bmpinf->bmiHeader.biBitCount=(WORD)(E.POCT*8);
	}
	bmpinf->bmiHeader.biCompression=BI_RGB;
	bmpinf->bmiHeader.biSizeImage=bmpinf->bmiHeader.biWidth*abs(bmpinf->bmiHeader.biHeight)*(bmpinf->bmiHeader.biBitCount/8);
	return;
}

void* SPG_CONV G_CreateBitmapInfo(G_Ecran& E, int YReverse)
{
	BITMAPINFO* bmpinf=(BITMAPINFO*)SPG_MemAlloc(sizeof(BITMAPINFOHEADER)+((E.POCT==1)?1024:0),"BmpInfoEcran");
	G_FillBitmapInfo(bmpinf,E,YReverse);
	return E.bmpinf=(void*)bmpinf;
}

void SPG_CONV G_CloseBitmapInfo(G_Ecran& E)
{
	if(E.bmpinf) SPG_MemFree(E.bmpinf);
	E.bmpinf=0;
	return;
}

#pragma warning( disable : 4706)//assignment within conditional expression

//initialise une surface ecrivable
int SPG_CONV G_InitEcran(G_Ecran& E, int EType,
				BYTE* MECR, int Pitch, int POCT,
				int SizeX, int SizeY,
				int PosX, int PosY,
				void* HDCECR,
				bool YReverse)
{

	memset(&E,0,sizeof(G_Ecran));

	CHECK(SizeX<=0,"G_InitEcran: Taille X invalide",return 0);
	CHECK(SizeY<=0,"G_InitEcran: Taille Y invalide",return 0);
	CHECK((POCT!=1)&&(POCT!=2)&&(POCT!=3)&&(POCT!=4),"G_InitEcran: Profondeur de couleur invalide",return 0);
	IF_CD_G_CHECK(19,return 0);

	E.Etat=EType;

	E.SizeX=SizeX;
	E.SizeY=SizeY;
	E.PosX=PosX;
	E.PosY=PosY;
	E.POCT=POCT;
	E.Pitch=E.SizeX*POCT;
	E.Size=E.Pitch*E.SizeY;
	E.HECR=HDCECR;

	if(
		(E.Etat&(G_ECRAN_SETDIB|G_ALLOC_MEMOIRE|G_ALIAS_MEMOIRE))==G_ECRAN_SETDIB
		)
	{
		E.Etat|=G_ALLOC_MEMOIRE;
	}

	if (E.Etat&G_ALLOC_MEMOIRE)
	{//ALLOC
		E.Pitch=(E.Pitch+3)&0xfffffffc;
		E.Size=E.Pitch*E.SizeY;
		E.MECR=(BYTE*)SPG_MemAlloc(E.Size,"Memoire MECR G_Ecran");
		E.Etat|=G_MEMORYAVAILABLE;
	}
	else if (E.Etat&G_ALIAS_MEMOIRE)
	{//ALIAS
		E.MECR=MECR;
		E.Pitch=Pitch;
		E.Etat|=G_MEMORYAVAILABLE;
	}
	
	if (E.Etat&G_ECRAN_SETDIB)
	{//SetDibBitsToDevice
		CHECK(((E.Etat&G_ALLOC_MEMOIRE)||(E.Etat&G_ALIAS_MEMOIRE))==0,"G_ECRAN_SETDIB",return 0);
		G_CreateBitmapInfo(E,YReverse);
	}
#ifdef SPG_General_PGLib
	else if (E.Etat&G_ECRAN_PGL)
	{//PGL
		E.surface=pglCreateSurface(Global.display, SizeX, SizeY, POCT*8, 0);
		//E.Pitch=E.surface->pitch;//je ne sais plus pourquoi c'est necessaire
	}
#endif
#ifdef SPG_General_USEWindows
	else if (E.Etat&G_ECRAN_DIBSECT)
	{//DIBSECT
		if(E.hCompatDC=CreateCompatibleDC((HDC)E.HECR))//assignement volontaire
		{
			//E.POCT=4;//compensation du bug Matrox G550 en mode 24bits
			G_CreateBitmapInfo(E,YReverse);
			if (E.DibSect=CreateDIBSection((HDC)E.hCompatDC,(BITMAPINFO*)(E.bmpinf),DIB_RGB_COLORS,(void**)(&E.MECR),0,0))
			{ //assignement volontaire
				DIBSECTION DIBInfo;
				memset(&DIBInfo,0,sizeof(DIBSECTION ));
				GetObject(E.DibSect,sizeof(DIBSECTION),(void*)&DIBInfo);
				//memcpy(E.bmpinf,&(DIBInfo.dsBmih),sizeof(BITMAPINFOHEADER));
				((BITMAPINFO*)E.bmpinf)->bmiHeader.biSizeImage=DIBInfo.dsBmih.biSizeImage;
				((BITMAPINFO*)E.bmpinf)->bmiHeader.biPlanes=DIBInfo.dsBmih.biPlanes;
				((BITMAPINFO*)E.bmpinf)->bmiHeader.biBitCount=DIBInfo.dsBmih.biBitCount;
				E.Pitch=DIBInfo.dsBm.bmWidthBytes;
				E.Etat|=G_MEMORYAVAILABLE;
				E.OldSelect=SelectObject((HDC)E.hCompatDC,E.DibSect);
			}
			else
			{
				G_CloseBitmapInfo(E);
				DeleteDC((HDC)E.hCompatDC);
				G_InitEcranWindows(E,E.POCT,E.SizeX,E.SizeY,(HDC)E.HECR,YReverse);//G_ECRAN_SETDIB
			}
		}
		else
		{
			G_InitEcranWindows(E,E.POCT,E.SizeX,E.SizeY,(HDC)E.HECR,YReverse);//G_ECRAN_SETDIB
		}
	}
	else if (E.Etat&G_ECRAN_CBMP)
	{//Compatible bitmap
		if (GetDeviceCaps((HDC)HDCECR,BITSPIXEL)!=8*E.POCT) return G_InitEcranDibSect(E,E.POCT,E.SizeX,E.SizeY,HDCECR);

		if(E.hCompatDC=CreateCompatibleDC((HDC)E.HECR))//assignement volontaire
		{
			if (E.CompatBMP=CreateCompatibleBitmap((HDC)E.hCompatDC,E.SizeX,E.SizeY))
			{//assignement volontaire
				SPG_StackAllocZ(BITMAP,Binfo);//createbitmapindirect
				GetObject(E.CompatBMP,sizeof(BITMAP),&Binfo);
				E.MECR=(BYTE*)Binfo.bmBits;
				E.Pitch=Binfo.bmWidthBytes;
				E.POCT=(Binfo.bmBitsPixel+7)/8;
				E.SizeX=Binfo.bmWidth;
				E.SizeY=Binfo.bmHeight;
				E.Etat|=G_MEMORYAVAILABLE;
				E.OldSelect=SelectObject((HDC)E.hCompatDC,E.DibSect);
				SPG_StackCheck(Binfo);
			}
			else
			{
				DeleteDC((HDC)E.hCompatDC);
				return G_InitEcranDibSect(E,E.POCT,E.SizeX,E.SizeY,(HDC)E.HECR,YReverse);
			}
		}
		else
		{
			return G_InitEcranDibSect(E,E.POCT,E.SizeX,E.SizeY,(HDC)E.HECR,YReverse);
		}
	}
#ifdef G_ECRAN_DRAWDIB
	else if (E.Etat&G_ECRAN_DRAWDIB)
	{
		if(G_InitEcranWindows(E,E.POCT,E.SizeX,E.SizeY,(HDC)E.HECR,0))
		{
			if(E.Etat&G_ECRAN_SETDIB)
			{
				if(E.hDrawDib=DrawDibOpen())//assignement volontaire
				{
					E.Etat&=~G_ECRAN_SETDIB;
					E.Etat|=G_ECRAN_DRAWDIB;
				}
			}
		}
	}
#endif
#endif
	/*
	else
	{
#ifdef DebugList
		SPG_List("G_InitEcran: G_Ecran non supporté");
#endif
		return 0;
	}
	*/

#ifdef SPG_General_PGLib
	if ((E.Etat&G_ECRAN_PGL)==0)
	{
#endif
		CHECK(E.MECR==0,"G_InitEcran: G_Ecran vide",G_CloseEcran(E);return 0);
#ifdef SPG_General_PGLib
	}
#endif

	CD_G_CHECK_EXIT(22,11);
	return E.Etat;
}

int SPG_CONV G_InitEcranWindows(G_Ecran& E, int POCT, int SizeX, int SizeY, void* HDCECR, bool YReverse)
{
	return G_InitEcran(E,G_ECRAN_SETDIB|G_ALLOC_MEMOIRE,
		(BYTE*)0,0,POCT,
		SizeX,SizeY,0,0,
		HDCECR,YReverse);
}

int SPG_CONV G_InitEcranDibSect(G_Ecran& E, int POCT, int SizeX, int SizeY, void* HDCECR, bool YReverse)
{
	return G_InitEcran(E,G_ECRAN_DIBSECT,
		(BYTE*)0,0,POCT,
		SizeX,SizeY,0,0,
		HDCECR,YReverse);
}

int SPG_CONV G_InitEcranCompatBMP(G_Ecran& E, int POCT, int SizeX, int SizeY, void* HDCECR, bool YReverse)
{
	return G_InitEcran(E,G_ECRAN_CBMP,
		(BYTE*)0,0,POCT,
		SizeX,SizeY,0,0,
		HDCECR,YReverse);
}

int SPG_CONV G_InitMemoryEcran(G_Ecran& E, int POCT, int SizeX, int SizeY)
{
	return G_InitEcran(E,G_ALLOC_MEMOIRE,
		(BYTE*)0,0,POCT,
		SizeX,SizeY,0,0,
		0);
}

#ifdef SPG_General_PGLib
int SPG_CONV G_InitEcranFromPGLDisplay(G_Ecran& E, PGLDisplay* display)
{
	memset(&E,0,sizeof(G_Ecran));
	CHECK(display==0,"G_InitEcranPGL: display nul",return 0);
	CHECK(V_InclusiveBound(display->bpp/8,1,4)==0,"G_InitEcranPGL: PGLSurface: profondeur de couleur invalide",return 0);
	CHECK(V_InclusiveBound(display->width,1,16384)==0,"G_InitEcranPGL: PGLSurface: largeur invalide",return 0);
	CHECK(V_InclusiveBound(display->height,1,16384)==0,"G_InitEcranPGL: PGLSurface: hauteur invalide",return 0);
	//E.Pitch=((PGLDisplay*)surface)->pitch;
	E.POCT=(display->bpp/8);
	E.SizeX=(display->width);
	E.SizeY=(display->height);
	E.surface=0;//(PGLDisplay*)surface;
	E.Etat=G_ECRAN_PGL;
	return -1;
}

int SPG_CONV G_InitEcranFromPGLSurface(G_Ecran& E, PGLSurface* surface)
{
	memset(&E,0,sizeof(G_Ecran));
	CHECK(surface==0,"G_InitEcranPGL: surface nulle",return 0);
	CHECK(V_InclusiveBound(surface->bpp/8,1,4)==0,"G_InitEcranPGL: PGLSurface: profondeur de couleur invalide",return 0);
	CHECK(V_InclusiveBound(surface->width,1,16384)==0,"G_InitEcranPGL: PGLSurface: largeur invalide",return 0);
	CHECK(V_InclusiveBound(surface->height,1,16384)==0,"G_InitEcranPGL: PGLSurface: hauteur invalide",return 0);
	//E.Pitch=((PGLDisplay*)surface)->pitch;
	E.POCT=(surface->bpp/8);
	E.SizeX=(surface->width);
	E.SizeY=(surface->height);
	E.surface=surface;
	E.Etat=G_ECRAN_PGL;
	return -1;
}
#endif
/*
int SPG_CONV G_InitEcranFromDIB(G_Ecran& E)
{
}
*/
int SPG_CONV G_InitSousEcran(G_Ecran& E, G_Ecran& ERef,int PosX,int PosY, int SizeX,int SizeY)
{

	SPG_ZeroStruct(E);
	CHECK(G_Etat(ERef)==0,"G_InitSousEcran",return 0)
	
	//CHECK((ERef.Etat&G_MEMORYAVAILABLE)==0,"G_InitSousEcran: G_Ecran non supporté",return 0)
	CHECK(V_InclusiveBound(PosX+SizeX,0,ERef.SizeX)==0,"G_InitSousEcran: L'ecran ne rentre pas",return 0)
	CHECK(V_InclusiveBound(PosY+SizeY,0,ERef.SizeY)==0,"G_InitSousEcran: L'ecran ne rentre pas",return 0)
	CHECK(V_InclusiveBound(PosX,0,ERef.SizeX)==0,"G_InitSousEcran: L'ecran ne rentre pas",return 0)
	CHECK(V_InclusiveBound(PosY,0,ERef.SizeY)==0,"G_InitSousEcran: L'ecran ne rentre pas",return 0)
#ifdef SPG_General_PGLib
	if ((ERef.Etat&G_ECRAN_PGL)==0)
	{
#endif
		CHECK(ERef.MECR==0,"G_InitSousEcran: Ecran vide",return 0);
		CHECK((ERef.Etat&G_MEMORYAVAILABLE)==0,"G_InitSousEcran: Ecran non accessible",return 0);
#ifdef SPG_General_PGLib
	}
#endif
#ifdef SPG_General_PGLib
	if (ERef.Etat&G_ECRAN_PGL)
	{
		if (G_InitEcran(E,(ERef.Etat&~G_ALLOC_MEMOIRE)|G_ALIAS_MEMOIRE,
		PixEcrPTR(ERef,PosX,PosY),ERef.Pitch,ERef.POCT,
		SizeX,SizeY,
		ERef.PosX+PosX,
		ERef.PosY+PosY,
		ERef.HECR)==0) return 0;
		E.Etat&=~G_MEMORYAVAILABLE;
		E.surface=ERef.surface;
		return -1;
	}
	else
	{
#endif
		int IE=G_InitEcran(E,G_ALIAS_MEMOIRE,
		PixEcrPTR(ERef,PosX,PosY),ERef.Pitch,ERef.POCT,
		SizeX,SizeY,
		ERef.PosX+PosX,
		ERef.PosY+PosY,
		ERef.HECR);
			
#ifdef G_ECRAN_DRAWDIB
		if (ERef.Etat&G_ECRAN_DRAWDIB)
		{
			E.hDrawDib=ERef.hDrawDib;
		}
		else
#endif
		     if (ERef.Etat&G_ECRAN_DIBSECT)
		{
			E.hCompatDC=ERef.hCompatDC;
			E.DibSect=ERef.DibSect;
		}
		else if (ERef.Etat&G_ECRAN_CBMP)
		{
			E.hCompatDC=ERef.hCompatDC;
			E.CompatBMP=ERef.CompatBMP;
		}
		return IE;
#ifdef SPG_General_PGLib
	}
#endif	
}


void SPG_CONV G_InitSplit4(G_Ecran& E, G_Ecran& EUL, G_Ecran& EUR, G_Ecran& EDL, G_Ecran& EDR)
{
	int SizeXL=G_SizeX(E)/2;
	int SizeXR=G_SizeX(E)-SizeXL;
	int SizeYU=G_SizeY(E)/2;
	int SizeYD=G_SizeY(E)-SizeYU;
	G_InitSousEcran(EUL,E,
		0,0,SizeXL,SizeYU);
	G_InitSousEcran(EUR,E,
		SizeXL,0,SizeXR,SizeYU);
	G_InitSousEcran(EDL,E,
		0,SizeYU,SizeXL,SizeYD);
	G_InitSousEcran(EDR,E,
		SizeXL,SizeYU,SizeXR,SizeYD);
	return;
}

void SPG_CONV G_CloseSplit4(G_Ecran& EUL, G_Ecran& EUR, G_Ecran& EDL, G_Ecran& EDR)
{
	G_CloseEcran(EUL);
	G_CloseEcran(EUR);
	G_CloseEcran(EDL);
	G_CloseEcran(EDR);
	return;
}

int SPG_CONV G_InitAliasMemEcran(G_Ecran& E, BYTE* MECR, int Pitch, int POCT, int SizeX, int SizeY)
{
	memset(&E,0,sizeof(G_Ecran));
	CHECK(MECR==0,"G_InitAliasMemEcran: Donnees sources nulles",return 0);
	return(G_InitEcran(E,G_ALIAS_MEMOIRE,MECR,Pitch,POCT,SizeX,SizeY,0,0,0));
}

#ifdef DebugMem
int SPG_CONV G_InitAliasMemEcranSafe(G_Ecran& E, BYTE* MECR, int Pitch, int POCT, int SizeX, int SizeY)
{
	memset(&E,0,sizeof(G_Ecran));
	CHECK(MECR==0,"G_InitAliasMemEcran: Donnees sources nulles",return 0);
	CHECK(SPG_MemIsValid(MECR,Pitch*SizeY)==0,"G_InitAliasMemEcranSafe",return 0);
	return(G_InitEcran(E,G_ALIAS_MEMOIRE,MECR,Pitch,POCT,SizeX,SizeY,0,0,0));
}
#endif

#ifdef SPG_General_USEFilesWindows
void SPG_CONV G_SaveAs_ToBMP(G_Ecran& E, char* SuggestedName)
{
	char RFile[MaxProgDir];
	strcpy(RFile,SuggestedName);
	if (SPG_GetSaveName(SPG_BMP,RFile,MaxProgDir)) G_SaveToBMP(RFile,E.MECR,E.POCT,E.Pitch,E.SizeX,E.SizeY);
}
#endif

void SPG_CONV G_SaveEcran(G_Ecran& E,char* FileName) 
{
	CHECK(E.Etat==0,"G_SaveEcran",return);
	G_SaveToBMP(FileName,E.MECR,E.POCT,E.Pitch,E.SizeX,E.SizeY);
	return;
}

void SPG_CONV G_SaveToBMP(char * Name,BYTE* M, int POCT, int Pitch, int SizeX, int SizeY)
{
	CHECK(M==0,"G_SaveToBMP: G_Ecran inaccessible",return);
	CHECK((POCT!=1)&&(POCT!=3)&&(POCT!=4),"G_SaveToBMP: Profondeur de couleur invalide",return);
	CHECK(!V_InclusiveBound(SizeX,1,8192),"G_SaveToBMP: Largeur invalide",return);
	CHECK(!V_InclusiveBound(SizeY,1,8192),"G_SaveToBMP: Hauteur invalide",return);
	CHECK(Pitch<POCT*SizeX,"G_SaveToBMP: Pitch incorrect",return);

	int POCTF,SizeF;
	BYTE*ColorT;
	BYTE*MFF;

	BYTE HEAD[54];

	if((POCTF=POCT)==4) 
		POCTF=3;
	
	SizeF=(SizeX*POCTF+3)&0xfffffffc;
	//HEAD=SPG_MemAlloc(54,"Header Bitmap");
	
	memset(HEAD,0,54);
	HEAD[0]=0x42;
	HEAD[1]=0x4D;
	*(LONG*)(HEAD+2)=SizeF*SizeY+54+((POCT==1)?1024:0);//+Palette (1024);
	HEAD[6]=0;
	HEAD[7]=0;
	HEAD[8]=0;
	HEAD[9]=0;
	*(LONG*)(HEAD+10)=54+((POCT==1)?1024:0);//+palette (1024);
	*(LONG*)(HEAD+14)=40;
	*(LONG*)(HEAD+18)=SizeX;
	*(LONG*)(HEAD+22)=SizeY;
	*(WORD*)(HEAD+26)=1;
	*(WORD*)(HEAD+28)=(WORD)(POCTF*8);
	*(LONG*)(HEAD+30)=0;
	*(LONG*)(HEAD+34)=SizeF*SizeY;
	*(LONG*)(HEAD+38)=0;
	*(LONG*)(HEAD+42)=0;
	*(LONG*)(HEAD+46)=((POCT==1)?256:0);//256;//si 256 coul
	*(LONG*)(HEAD+50)=((POCT==1)?256:0);//256;//si 256 coul
	
	FILE*F=fopen(Name,"wb");
	CHECKTWO(F==0,"G_SaveToBMP: Impossible d'ouvrir le fichier",Name,return);
	
	fwrite(HEAD,54,1,F);
	
	//SPG_MemFree(HEAD);
	
	if(POCT==1)
	{
		ColorT=(BYTE*)SPG_MemAlloc(1024,"ColorTable Bitmap");
		LONG *CT;
		CT=(LONG *)ColorT;
		int i;
		for(i=0;i<=0x00ffffff;i+=0x00010101)
		{
			*CT++=i;
		}
		fwrite(ColorT,1024,1,F);
		SPG_MemFree(ColorT);
	}
	
	MFF=(BYTE*)SPG_MemAlloc(SizeF+4,"Ligne Bitmap");
	int y;
	//BYTE * MT;
	BYTE * MFT;
	//MT=M;
	for(y=SizeY-1; y>=0; y--)
	{
		MFT=MFF;

		BYTE*MT=M+Pitch*y;

		int x;
		for(x=0;x<SizeX;x++)
		{
			*MFT=*MT;
			*(MFT+1)=*(MT+1);//genere des acces trop loin en bmp 8 bits, d'ou le +4 dans l'alloc
			*(MFT+2)=*(MT+2);
			MFT+=POCTF;		
			MT+=POCT;
		}

		//MT+=Pitch-SizeX*POCT;
		
		fwrite(MFF,SizeF,1,F);
		
		CD_G_CHECK_EXIT(12,17);
	}

	SPG_MemFree(MFF);
	fclose(F);
	return;
}

int SPG_CONV G_InitEcranFromFile(G_Ecran& E, int POCT, int LoadYReverse, char*Name)
{
	int From8,SizeF,SizeX,SizeY;
	BYTE HEAD[54];
	
	memset(&E,0,sizeof(G_Ecran));
	
	FILE *F = fopen(Name,"rb");
	CHECKTWO(F==0,"G_InitEcranFromFile: Impossible d'ouvrir le fichier",Name,return 0);
	
	fread(HEAD,54,1,F);
	
	fseek(F,*(LONG*)(HEAD+10),SEEK_SET);

	SizeX = *(DWORD*)(HEAD+18);
	SizeY = *(DWORD*)(HEAD+22);

	if (*(WORD*)(HEAD+28)==8) 
		From8=1;
	else
		From8=0;

	if (From8==0) 
		SizeF = (SizeX*3+3)&0xfffffffc;
	else
		SizeF = (SizeX+3)&0xfffffffc;

	BYTE*UneLigne;
	UneLigne=(BYTE*)SPG_MemAlloc(SizeF,"Ligne de bitmap");
	
	CHECK(G_InitEcran(E,G_ALLOC_MEMOIRE,
		0,0,POCT,SizeX,SizeY,
		0,0,0)==0,"G_InitEcranFromFile: Creation ecran echouee",fclose(F);SPG_MemFree(UneLigne);return 0);
	
	int y;

if(LoadYReverse==0)
{
	for(y=SizeY-1; y>=0; y--)
	{
		fread(UneLigne,SizeF,1,F);
		//MT+=E.Pitch;
		int x;
		if(From8)
		{
			for(x=0; x<SizeX; x++)
			{
				DWORD C;
				DWORD CMOY=UneLigne[x]+(UneLigne[x]<<8)+(UneLigne[x]<<16);
				G_MakeCompatibleFrom24(E,CMOY,C);
				G_DrawPixel(E,x,y,C);
			}
		}
		else
		{
			for(x=0; x<SizeX; x++)
			{
				DWORD C;
				G_MakeCompatibleFromP24(E,(UneLigne+3*x),C);
				G_DrawPixel(E,x,y,C);
			}
		}
	}
}
else
{
	for(y=0; y<SizeY; y++)
	{
		fread(UneLigne,SizeF,1,F);
		//MT+=E.Pitch;
		int x;
		if(From8)
		{
			for(x=0; x<SizeX; x++)
			{
				DWORD C;
				DWORD CMOY=UneLigne[x]+(UneLigne[x]<<8)+(UneLigne[x]<<16);
				G_MakeCompatibleFrom24(E,CMOY,C);
				G_DrawPixel(E,x,y,C);
			}
		}
		else
		{
			for(x=0; x<SizeX; x++)
			{
				DWORD C;
				G_MakeCompatibleFromP24(E,(UneLigne+3*x),C);
				G_DrawPixel(E,x,y,C);
			}
		}
	}
}

	SPG_MemFree(UneLigne);
	
	fclose(F);
	
	return -1;
}

void SPG_CONV G_CloseEcran(G_Ecran& E)
{
//	CHECK(E.MECR==0,"G_DelEcran: G_Ecran vide");
#ifdef DebugList
	DbgCHECK(E.MECR==0,"G_CloseEcran: G_Ecran vide");
	//DbgCHECK(E.bmpinf==0,"G_CloseEcran: G_Ecran sans Bitmapinfo");
#endif
	if((E.MECR)&&(E.Etat&G_ALLOC_MEMOIRE))
		SPG_MemFree(E.MECR);
	//E.MECR=0;

	if(E.Etat&G_ECRAN_SETDIB)
	{
		G_CloseBitmapInfo(E);
	}
#ifdef SPG_General_PGLib
	else if((E.Etat&G_ECRAN_PGL)&&(E.Etat&G_ALLOC_MEMOIRE))
		pglDestroySurface(E.surface);
#endif
#ifdef G_ECRAN_DRAWDIB
	else if(E.Etat&G_ECRAN_DRAWDIB)
	{
		G_CloseBitmapInfo(E);
		DrawDibClose((HDRAWDIB)E.hDrawDib);
	}
#endif
	else if((E.Etat&G_ECRAN_CBMP)&&((E.Etat&G_ALIAS_MEMOIRE)==0))
	{
		SelectObject((HDC)E.hCompatDC,E.OldSelect);
		DeleteObject(E.CompatBMP);
		DeleteDC((HDC)E.hCompatDC);
	}
	else if((E.Etat&G_ECRAN_DIBSECT)&&((E.Etat&G_ALIAS_MEMOIRE)==0))
	{
		G_CloseBitmapInfo(E);
		SelectObject((HDC)E.hCompatDC,E.OldSelect);
		DeleteObject(E.DibSect);
		DeleteDC((HDC)E.hCompatDC);
	}
	//E.bmpinf=0;
	//E.Etat=0;
	//memset(&E,0,sizeof(G_Ecran));
	SPG_ZeroStruct(E);
	return;
}

/*
int G_IsInEcran(G_Ecran& E, int AbsoluteX, int AbsoluteY)
{
	return (
		V_IsBound(AbsoluteX,E.PosX,E.PosX+E.SizeX)&&
		V_IsBound(AbsoluteY,E.PosY,E.PosY+E.SizeY)
		);
}
*/
/*
DWORD SPG_FASTCONV G_MakeCompatibleFrom24(G_Ecran& E, DWORD Coul)
{
	if (E.POCT==4)
		return Coul&0xffffff;
	if (E.POCT==3)
		return Coul;
	if (E.POCT==2)
		return G_Make16From24(Coul);
	if (E.POCT==1)
		return G_Make8From24(Coul);
	return 0;
}
*/

void SPG_CONV G_InitJauge(G_Ecran& E,G_Jauge& J,float*ValeurAMesurer, float VMin,float VMax, int Y0, DWORD Coul1, DWORD Coul2)
{
	J.X0=0;
	J.X1=E.SizeX;
	J.Y0=Y0;
	J.Y1=Y0+16;
	J.ValeurAMesurer=ValeurAMesurer;
	J.VMin=VMin;
	J.VMax=VMax;
	J.Coul1=Coul1;
	J.Coul2=Coul2;
	return;
}

void SPG_CONV G_CloseJauge(G_Jauge& J)
{
	J.ValeurAMesurer=0;
	return;
}

void SPG_CONV G_DrawJauge(G_Ecran& E, G_Jauge& J)
{
	CHECK(J.ValeurAMesurer==0,"Jauge non initialisee",return);
	G_DrawRect(E,J.X0,J.Y0,J.X1,J.Y1,J.Coul1);
	int V=int(
		(J.X1-J.X0)*
		((*J.ValeurAMesurer)-J.VMin)
		/(J.VMax-J.VMin));

	if (V>(J.X1-J.X0)) V=J.X1-J.X0;
	if (V<0) V=0;
	G_DrawRect(E,J.X0,J.Y0,J.X0+V,J.Y1,J.Coul2);
	return;
}

void SPG_CONV SPG_FillColorPalette(PixCoul* Color, int MaxColor)
{
	int LenColor=(MaxColor+5-1)/5;
	for(int i=0;i<MaxColor;i++)
	{
		int NumRamp=i/LenColor;
		int Index01=((i%LenColor)*255)/LenColor;
		int Index10=255-Index01;
		int R=0;int V=0;int B=0;
		switch(NumRamp)
		{
		case 0:
			B=Index01;
			break;
		case 1:
			B=255;
			V=Index01;
			break;
		case 2:
			B=Index10;
			V=255;
			break;
		case 3:
			V=255;
			R=Index01;
			break;
		case 4:
			V=Index10;
			R=255;
			break;
			/*
		case 5:
			B=Index01;
			V=Index01;
			R=255;
			break;
			*/
		}
		Color[i].R=V_Sature(R,0,255);
		Color[i].V=V_Sature(V,0,255);
		Color[i].B=V_Sature(B,0,255);
	}
	return;
}

int SPG_CONV DrawVector_Init(DRAWVECT& DV, float DrawScale, float ColorScale)
{
	memset(&DV,0,sizeof(DRAWVECT));
	DV.DrawScale=DrawScale;
	DV.ColorScale=ColorScale*DRAWVECTCOLOR;
	SPG_FillColorPalette(DV.Color,DRAWVECTCOLOR);
	return -1;
}

void SPG_CONV DrawVector_Close(DRAWVECT& DV)
{
	memset(&DV,0,sizeof(DRAWVECT));
	return;
}

void SPG_CONV DrawVector(DRAWVECT& DV, G_Ecran& E, int X, int Y, float VX, float VY)
{
	int i=sqrt(VX*VX+VY*VY)*DV.ColorScale;
	G_DrawLine(E,X-1,Y,X+V_Round(VX*DV.DrawScale),Y+V_Round(VY*DV.DrawScale),DV.Color[V_Sature(i,0,(DRAWVECTCOLOR-1))].Coul);
	G_DrawLine(E,X,Y,X+V_Round(VX*DV.DrawScale),Y+V_Round(VY*DV.DrawScale),DV.Color[V_Sature(i,0,(DRAWVECTCOLOR-1))].Coul);
	G_DrawLine(E,X+1,Y,X+V_Round(VX*DV.DrawScale),Y+V_Round(VY*DV.DrawScale),DV.Color[V_Sature(i,0,(DRAWVECTCOLOR-1))].Coul);
	G_DrawLine(E,X,Y-1,X+V_Round(VX*DV.DrawScale),Y+V_Round(VY*DV.DrawScale),DV.Color[V_Sature(i,0,(DRAWVECTCOLOR-1))].Coul);
	G_DrawLine(E,X,Y+1,X+V_Round(VX*DV.DrawScale),Y+V_Round(VY*DV.DrawScale),DV.Color[V_Sature(i,0,(DRAWVECTCOLOR-1))].Coul);
	return;
}

#endif
