
#include "SPG_General.h"

#ifdef SPG_General_USEGraphics

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

/*
#include "SPG_InhibitedFunctions.h"
#include "V_General.h"
#include "BRES.h"
#include "SPG_List.h"
#include "SPG_Graphics.h"
#if(_MSC_VER>=1300)
#include <windows.h>
#include <commdlg.h>
#else
#include "SPG_SysInc.h"
#endif
*/
/*
#ifdef G_ECRAN_DRAWDIB
#include "Config\SPG_Warning.h"
#include <memory.h>
#include <vfw.h>
#endif
*/

#include <string.h>
#include <stdlib.h>
#include <stdio.h>

//#define DebugRenderSegment
//#undef DebugRenderSegment

#define DEF_G_RENDERMODE 0

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawPixel")
void SPG_CONV G_DrawPixel(G_Ecran& E, int X, int Y, DWORD Coul)
{
//	CHECK(E.MECR==0,"G_DrawPixel: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawPixel: G_Ecran inaccessible",return);
	if (V_IsBound(X,0,E.SizeX)&&V_IsBound(Y,0,E.SizeY))
	{
		if (E.POCT==4)
		{
			*(DWORD*)(E.MECR+4*X+E.Pitch*Y)=Coul;
		}
		else if (E.POCT==3)
		{
			*(PixCoul24*)(E.MECR+3*X+E.Pitch*Y)=*(PixCoul24*)&Coul;
		}
		else if (E.POCT==2)
		{
			*(WORD*)(E.MECR+2*X+E.Pitch*Y)=(WORD)Coul;
		}
		else if (E.POCT==1)
		{
			E.MECR[X+E.Pitch*Y]=(BYTE)Coul;
		}
	}
	return;
}


#pragma SPGMSG(__FILE__,__LINE__,"G_DrawLine")
void SPG_CONV G_DrawLine(G_Ecran& E, int X0, int Y0, int X1, int Y1, DWORD Coul)
{
	TRY_BEGIN
	CHECK(G_Etat(E)==0,"G_DrawLine",return)
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawLine: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawLine: G_Ecran inaccessible",return);
#endif

	if(abs(X1-X0)>=abs(Y1-Y0))
	{
		if (X0>X1)
		{
			V_Swap(int,X0,X1);
			V_Swap(int,Y0,Y1);
		}
		else if (X0==X1)
		{
			return;
		}
		if (E.POCT==4)
		{
#define DEF_G_POCT 4
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
//#define DEF_G_FastLine
#include "GraphicsInlineX.cpp"
		}
		else if (E.POCT==3)
		{
#define DEF_G_POCT 3
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#include "GraphicsInlineX.cpp"
		}
		else if (E.POCT==2)
		{
#define DEF_G_POCT 2
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#include "GraphicsInlineX.cpp"
		}
		else if (E.POCT==1)
		{
#define DEF_G_POCT 1
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#include "GraphicsInlineX.cpp"
		}
	}
	else
	{
		if (Y0>Y1)
		{
			V_Swap(int,X0,X1);
			V_Swap(int,Y0,Y1);
		}
		else if (Y0==Y1)
		{
			return;
		}
//		float InvDy=1.0/(V_Max(1,Y1-Y0));
		if (E.POCT==4)
		{
#define DEF_G_POCT 4
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#include "GraphicsInlineY.cpp"
		}
		else if (E.POCT==3)
		{
#define DEF_G_POCT 3
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#include "GraphicsInlineY.cpp"
		}
		else if (E.POCT==2)
		{
#define DEF_G_POCT 2
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#include "GraphicsInlineY.cpp"
		}
		else if (E.POCT==1)
		{
#define DEF_G_POCT 1
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#include "GraphicsInlineY.cpp"
		}

	}
	return;
	TRY_ENDG("G_DrawLine")
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawRect")
void SPG_CONV G_DrawRect(G_Ecran& E, int X0, int Y0, int X1, int Y1, DWORD Coul)
{
	TRY_BEGIN
	CHECK(G_Etat(E)==0,"G_DrawRect",return);
#ifdef DebugGraphics
	CHECK(E.MECR==0,"G_DrawRect: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawRect: G_Ecran inaccessible",return);
#endif
#ifdef G_ECRAN_USEGDI
	if(E.Etat&G_ECRAN_USEGDI)
	{
		COLORREF CR;
		((BYTE*)&CR)[0]=((BYTE*)&Coul)[2];
		((BYTE*)&CR)[1]=((BYTE*)&Coul)[1];
		((BYTE*)&CR)[2]=((BYTE*)&Coul)[0];
		((BYTE*)&CR)[3]=0;
		HBRUSH B=CreateSolidBrush(CR);
		RECT RR={X0+E.PosX,Y0+E.PosY,X1+E.PosX,Y1+E.PosY};
		FillRect((HDC)E.hCompatDC,&RR,B);
		DeleteObject(B);
		return;
	}
#endif

	int XLEN=V_Min(X1,E.SizeX)-V_Max(X0,0);
	if (XLEN<=0) return;
	BYTE*TECR=(E.MECR+V_Max(Y0,0)*E.Pitch+V_Max(X0,0)*E.POCT);

	if(E.POCT==4)
	{
		/*
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 4
#include "InlineBuildCoulC.cpp"
		*/

	for (int Y=V_Max(Y0,0);Y<V_Min(Y1,E.SizeY);Y++)
	{
		__asm
		{
			push edi;
			mov edi,TECR;
			push eax;
			mov eax,Coul;
			push ecx;
			mov ecx,XLEN;
			rep stosd;
			pop ecx;
			pop eax;
			pop edi;
		}
		/*
		for (int X=0;X<XLEN;X++)
		{
			((DWORD*)TECR)[X]=ICC_Coul;
		}
		*/
		TECR+=E.Pitch;
	}
	/*
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	*/
	}
	else if (E.POCT==3)
	{
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 3
#include "InlineBuildCoulC.cpp"

	for (int Y=V_Max(Y0,0);Y<V_Min(Y1,E.SizeY);Y++)
	{
		for (int X=0;X<XLEN;X++)
		{
			*(PixCoul24*)(TECR+3*X)=ICC_Coul;
		}
		TECR+=E.Pitch;
	}
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	}
	else if (E.POCT==2)
	{
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 2
#include "InlineBuildCoulC.cpp"

	for (int Y=V_Max(Y0,0);Y<V_Min(Y1,E.SizeY);Y++)
	{
		for (int X=0;X<XLEN;X++)
		{
			((WORD*)TECR)[X]=ICC_Coul;
		}
		TECR+=E.Pitch;
	}
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	}
	else if (E.POCT==1)
	{
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 1
#include "InlineBuildCoulC.cpp"

	for (int Y=V_Max(Y0,0);Y<V_Min(Y1,E.SizeY);Y++)
	{
		for (int X=0;X<XLEN;X++)
		{
			TECR[X]=ICC_Coul;
		}
		TECR+=E.Pitch;
	}
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	}
	return;
	TRY_ENDG("G_DrawRect")
}


void SPG_CONV G_SoftenSoft(G_Ecran& E)
{
	//return;
	if (E.POCT==4)
	{

#pragma vector aligned
	for(int Y=0;Y<E.SizeY-1;Y++)
	{
		const DWORD* restrict MECR=(DWORD*)(E.MECR+Y*E.Pitch);
		DWORD* restrict MECR_Alias=(DWORD*)(E.MECR+Y*E.Pitch);
		const DWORD* restrict MECRL=(DWORD*)(E.MECR+Y*E.Pitch+E.Pitch);
		for(int X=0;X<E.SizeX-1;X++)
		{
			MECR_Alias[X]=
				(
				(MECR[X+1]&0xfcfcfc)+
				((MECR[X]&0xfcfcfc)<<1)+
				(MECRL[X]&0xfcfcfc)
				)>>2;
		}
	}

	}
	else if (E.POCT==3)
	{

	for(int Y=0;Y<E.SizeY-1;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-1;X++)
		{
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X+1)&0xfcfcfc)+
				((*(DWORD*)(MECR+3*X)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+3*X+E.Pitch)&0xfcfcfc)
				)>>2;
		}
	}

	}
	return;
}

#pragma push

#pragma warning(disable:4731)

void SPG_CONV G_SoftenSoftAlign(G_Ecran& E)
{
	//return;
	CHECK(E.POCT!=4,"G_SoftenSoftAlign",return);
	//CHECK(E.SizeX&3,"G_SoftenSoftAlign",return);
	CHECK(E.Pitch&3,"G_SoftenSoftAlign",return);
	CHECK(((int)E.MECR)&3,"G_SoftenSoftAlign",return);
	const int SizeY=E.SizeY-1;
	const int SizeX=(E.SizeX-1)*4;
	const int Pitch=E.Pitch;
	for(int Y=0;Y<SizeY;Y++)
	{
		DWORD* restrict MECR=(DWORD*)(E.MECR+Y*E.Pitch);
		__asm
		{
			push esi;
			push edi;
			mov esi,MECR;
			mov edi,Pitch;
			mov ecx,SizeX;
			push ebp;//stack params no more available
			mov ebp,0xfcfcfc;//ebp est modifie mais restore
			neg ecx;
			sub esi,ecx;
			add edi,esi;
			mov eax,dword ptr [esi+ecx];
			and eax,ebp;
			ALIGN 16
G_SoftA:
			mov ebx,dword ptr [edi+ecx];//MML[0]=0xfcfcfc&ML[0]
			mov edx,dword ptr [esi+ecx+4];//MM[1]=0xfcfcfc&M[1]
			and ebx,ebp;
			and edx,ebp;
			add ebx,edx;
			lea eax,[eax*2+ebx];
			shr eax,2;
			mov dword ptr [esi+ecx],eax;
			add ecx,4;
			mov eax,edx;
			jnz G_SoftA;
			pop ebp;//ebp est modifie mais restore ici
			pop edi;
			pop esi;
		}
		
	}
	return;
}

#pragma pop

/*
void SPG_CONV G_SoftenSoftAlign(G_Ecran& E)
{
	//return;
	CHECK(E.POCT!=4,"G_SoftenSoftAlign",return);
	//CHECK(E.SizeX&3,"G_SoftenSoftAlign",return);
	CHECK(E.Pitch&3,"G_SoftenSoftAlign",return);
	CHECK(((int)E.MECR)&3,"G_SoftenSoftAlign",return);
	const int SizeY=E.SizeY-2;
	const int SizeX=(E.SizeX-1)*4;
	const int Pitch=E.Pitch;
	const int DPitch=Pitch+Pitch;
	for(int Y=0;Y<SizeY;Y+=2)
	{
		DWORD* restrict MECR=(DWORD*)(E.MECR+Y*E.Pitch);
		DWORD MECR_END=(DWORD)(E.MECR+Y*E.Pitch+SizeX);
		__asm
		{
			push esi;
			push edi;
			push eax;
			push ebx;
			push ecx;
			push edx;
			mov ecx,MECR;
			mov edx,Pitch;
			mov eax,[ecx];
			mov ebx,[ecx+edx];
		
G_SoftA:
			mov esi,[ecx+4];//P01
			mov edi,[ecx+edx+4];//P11
			mov edx,[ecx+edx*2];//P20
			and esi,0xfcfcfc;//M01
			lea eax,[eax*2+esi];//RX00
			add eax,ebx;//R00
			shr eax,2;
			mov [ecx],eax;//R00
			mov eax,esi;//M00
			and edi,0xfcfcfc;//M11
			lea ebx,[ebx*2+edi];//RX10
			and edx,0xfcfcfc;//M20
			add ebx,edx;//R10
			mov edx,Pitch;
			shr ebx,2;
			mov [ecx+edx],ebx;//R10
			add ecx,4;
			mov ebx,edi;//M10
			cmp ecx,MECR_END;
			jb G_SoftA;
			
			pop edx;
			pop ecx;
			pop ebx;
			pop eax;
			pop edi;
			pop esi;
		}
		
	}
	return;
}
*/

void SPG_CONV G_Soften2x2(G_Ecran& E)
{
	CHECK(E.Etat==0,"G_Soften2x2",return);
	//return;
	if (E.POCT==4)
	{

	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-1;X++)
		{
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfefefe)+
				(*(DWORD*)(MECR+4*X+4)&0xfefefe)
				)>>1;
		}
		/*
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfefefe)
				)>>1;
				*/
	}
	for(Y=0;Y<E.SizeY-1;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfefefe)+
				(*(DWORD*)(MECR+4*X+E.Pitch)&0xfefefe)
				)>>1;
		}
	}
	/*
	{//last line
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfefefe)
				)>>1;
		}
	}
	*/

	}
	else if (E.POCT==3)
	{
	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-1;X++)
		{
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfefefe)+
				(*(DWORD*)(MECR+3*X+4)&0xfefefe)
				)>>1;
		}
		/*
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfefefe)
				)>>1;
				*/
	}
	for(Y=0;Y<E.SizeY-1;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfefefe)+
				(*(DWORD*)(MECR+3*X+E.Pitch)&0xfefefe)
				)>>1;
		}
	}
	/*
	{//last line
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfefefe)
				)>>1;
		}
	}
	*/

	}
	else if (E.POCT==1)
	{
	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-1;X++)
		{
			*(BYTE*)(MECR+X)=
				(
				(*(BYTE*)(MECR+X)&0xfe)+
				(*(BYTE*)(MECR+X+1)&0xfe)
				)>>1;
		}
		/*
			*(BYTE*)(MECR+X)=
				(
				(*(BYTE*)(MECR+X)&0xfe)
				)>>1;
				*/
	}
	for(Y=0;Y<E.SizeY-1;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(BYTE*)(MECR+X)=
				(
				(*(BYTE*)(MECR+X)&0xfe)+
				(*(BYTE*)(MECR+X+E.Pitch)&0xfe)
				)>>1;
		}
	}
	/*
	{//last line
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(BYTE*)(MECR+X)=
				(
				(*(BYTE*)(MECR+X)&0xfe)
				)>>1;
		}
	}
	*/

	}

	return;
}

void SPG_CONV G_Soften3x3(G_Ecran& E)
{
	CHECK(E.Etat==0,"G_Soften3x3",return);
	//return;
	if (E.POCT==4)
	{

	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-2;X++)
		{
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+4*X+4)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+4*X+8)&0xfcfcfc)
				)>>2;
		}
	}
	for(Y=0;Y<E.SizeY-2;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+4*X+E.Pitch)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+4*X+2*E.Pitch)&0xfcfcfc)
				)>>2;
		}
	}

	}
	else if (E.POCT==3)
	{

	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-2;X++)
		{
			BYTE Saved=MECR[3*X+3];
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+3*X+3)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+3*X+6)&0xfcfcfc)
				)>>2;
			MECR[3*X+3]=Saved;
		}
	}

	for(Y=0;Y<E.SizeY-2;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			BYTE Saved=MECR[3*X+3];
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+3*X+E.Pitch)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+3*X+2*E.Pitch)&0xfcfcfc)
				)>>2;
			MECR[3*X+3]=Saved;
		}
	}

	}
	else if (E.POCT==1)
	{

	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-2;X++)
		{
			*(BYTE*)(MECR+X)=
				(
				(*(BYTE*)(MECR+X)&0xfc)+
				((*(BYTE*)(MECR+X+1)&0xfc)<<1)+
				(*(BYTE*)(MECR+X+2)&0xfc)
				)>>2;
		}
	}
	for(Y=0;Y<E.SizeY-2;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(BYTE*)(MECR+X)=
				(
				(*(BYTE*)(MECR+X)&0xfc)+
				((*(BYTE*)(MECR+X+E.Pitch)&0xfc)<<1)+
				(*(BYTE*)(MECR+X+2*E.Pitch)&0xfc)
				)>>2;
		}
	}

	}
	return;
}

void SPG_CONV G_Soften3x3rz(G_Ecran& E)
{
	CHECK(E.Etat==0,"G_Soften3x3rz",return);
	CHECK((E.SizeX<3)&&(E.SizeY<3),"G_Soften3x3rz",return)
	G_Soften3x3(E);
	CHECK((E.Etat&(G_ALLOC_MEMOIRE|G_ALIAS_MEMOIRE))==0,"G_Soften3x3rz",return);
	E.SizeX-=2;
	E.SizeY-=2;
	return;
}
void SPG_CONV G_Soften3x3_Reversed(G_Ecran& E)
{
	//return;
	if (E.POCT==4)
	{

	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=E.SizeX-1;X>=2;X--)
		{
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+4*X-4)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+4*X-8)&0xfcfcfc)
				)>>2;
		}
	}
	for(Y=E.SizeY-1;Y>=2;Y--)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(DWORD*)(MECR+4*X)=
				(
				(*(DWORD*)(MECR+4*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+4*X-E.Pitch)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+4*X-2*E.Pitch)&0xfcfcfc)
				)>>2;
		}
	}

	}
	else if (E.POCT==3)
	{

	int Y;
	for(Y=0;Y<E.SizeY;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX-2;X++)
		{
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+3*X+3)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+3*X+6)&0xfcfcfc)
				)>>2;
		}
	}
	for(Y=0;Y<E.SizeY-2;Y++)
	{
		BYTE*MECR=E.MECR+Y*E.Pitch;
		for(int X=0;X<E.SizeX;X++)
		{
			*(DWORD*)(MECR+3*X)=
				(
				(*(DWORD*)(MECR+3*X)&0xfcfcfc)+
				((*(DWORD*)(MECR+3*X+E.Pitch)&0xfcfcfc)<<1)+
				(*(DWORD*)(MECR+3*X+2*E.Pitch)&0xfcfcfc)
				)>>2;
		}
	}

	}
	return;
}

#pragma SPGMSG(__FILE__,__LINE__,"G_DrawFog")
void SPG_CONV G_DrawFog(G_Ecran& E, PixCoul Coul, int Pos)
{
	CHECK(E.MECR==0,"G_DrawRect: G_Ecran vide",return);
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_DrawRect: G_Ecran inaccessible",return);

	BYTE*TECR;

	switch(Pos)
	{
	case 0:
		TECR=E.MECR+2*E.POCT+2*E.Pitch;
		break;
	case 1:
		TECR=E.MECR+0*E.POCT+0*E.Pitch;
		break;
	case 2:
		TECR=E.MECR+2*E.POCT+0*E.Pitch;
		break;
	case 3:
		TECR=E.MECR+0*E.POCT+2*E.Pitch;
		break;
	case 4:
		TECR=E.MECR+3*E.POCT+3*E.Pitch;
		break;
	case 5:
		TECR=E.MECR+1*E.POCT+1*E.Pitch;
		break;
	case 6:
		TECR=E.MECR+3*E.POCT+1*E.Pitch;
		break;
	case 7:
		TECR=E.MECR+1*E.POCT+3*E.Pitch;
		break;
	default:
#ifdef DebugList
		SPG_List("Mauvais fog");
#endif
		return;
	}

	if(E.POCT==4)
	{
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 4
#include "InlineBuildCoulC.cpp"

	for (int Y=0;Y<(E.SizeY>>2);Y++)
	{
		for (int X=0;X<E.SizeX;X+=4)
		{
			((DWORD*)TECR)[X]=ICC_Coul;
		}
		TECR+=E.Pitch<<2;
	}
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	}
	else if (E.POCT==3)
	{
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 3
#include "InlineBuildCoulC.cpp"

	for (int Y=0;Y<(E.SizeY>>2);Y++)
	{
		for (int X=0;X<E.SizeX;X+=4)
		{
			*(PixCoul24*)(TECR+3*X)=ICC_Coul;
		}
		TECR+=E.Pitch<<2;
	}
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	}
	else if (E.POCT==2)
	{
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 2
#include "InlineBuildCoulC.cpp"

	for (int Y=0;Y<(E.SizeY>>2);Y++)
	{
		for (int X=0;X<E.SizeX;X+=4)
		{
			((WORD*)TECR)[X]=ICC_Coul;
		}
		TECR+=E.Pitch<<2;
	}
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	}
	else if (E.POCT==1)
	{
#define DEF_G_RENDERCOUL (*(PixCoul*)&Coul)
#define DEF_G_POCT 1
#include "InlineBuildCoulC.cpp"

	for (int Y=0;Y<(E.SizeY>>2);Y++)
	{
		for (int X=0;X<E.SizeX;X+=4)
		{
			TECR[X]=ICC_Coul;
		}
		TECR+=E.Pitch<<2;
	}
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_POCT
	}
	return;
}

void SPG_CONV G_DrawOutRect(G_Ecran& E, int X0, int Y0, int X1, int Y1, DWORD Coul)
{
	G_DrawLine(E,X0,Y0,X1,Y0,Coul);
	G_DrawLine(E,X0,Y0,X0,Y1,Coul);
	G_DrawLine(E,X0,Y1,X1,Y1,Coul);
	G_DrawLine(E,X1,Y0,X1,Y1,Coul);
}

void SPG_CONV G_BlitEcran(G_Ecran& E)
{
	CHECK(E.Etat==0,"G_BlitEcran: Ecran invalide",return);
#ifdef G_ECRAN_DRAWDIB
	if (E.Etat&G_ECRAN_DRAWDIB)
	{
		DrawDibBegin(E.hDrawDib,(HDC)E.HECR,-1,-1,(BITMAPINFOHEADER*)E.bmpinf,E.SizeX,E.SizeY,0);
		DrawDibDraw(E.hDrawDib,(HDC)E.HECR,0,0,-1,-1,(BITMAPINFOHEADER*)E.bmpinf,E.MECR,0,0,E.SizeX,E.SizeY,0);

		//CHECK(BitBlt((HDC)E.HECR,E.PosX,E.PosY,E.SizeX,E.SizeY,(HDC)E.HdcDib,0,0,SRCCOPY)==0,"BitBlt echoue",return);
	}
	else
#endif
#ifdef G_ECRAN_DIBSECT
		if (E.Etat&G_ECRAN_DIBSECT)
	{
		CHECK(BitBlt((HDC)E.HECR,E.PosX,E.PosY,E.SizeX,E.SizeY,(HDC)E.hCompatDC,0,0,SRCCOPY)==0,"BitBlt echoue",return);
	}
	else 
#endif
#ifdef G_ECRAN_CBMP
		if (E.Etat&G_ECRAN_CBMP)
	{
		CHECK(BitBlt((HDC)E.HECR,E.PosX,E.PosY,E.SizeX,E.SizeY,(HDC)E.hCompatDC,0,0,SRCCOPY)==0,"BitBlt echoue",return);
	}
	else 
#endif
#ifdef G_ECRAN_SETDIB
		if (E.Etat&G_ECRAN_SETDIB) 
	{
		CHECK(SetDIBitsToDevice((HDC)E.HECR,E.PosX,E.PosY,E.SizeX,E.SizeY,0,0,0,E.SizeY,E.MECR,(BITMAPINFO*)E.bmpinf,DIB_RGB_COLORS)==0,"SetDiBitsToDevice echoue",return);
	}
	else
#endif
#ifdef SPG_General_PGLib
		if (E.Etat&G_ECRAN_PGL) 
	{
		pglBlitSurface(Global.display,0,E.surface,E.PosX,E.PosY,E.SizeX,E.SizeY,0,0,0);
	}
#endif
	{
		//DbgCHECK(1,"G_BlitEcran : Type invalide");
	}
	CD_G_CHECK_EXIT(27,23);
	return;
}

void SPG_CONV G_BlitEcranRect(G_Ecran& E,int PosX,int PosY,int SizeX, int SizeY)
{
	CHECK(E.Etat==0,"G_BlitEcranRect: Ecran invalide",return);
	CHECK((!V_InclusiveBound(PosX,0,E.SizeX))||(!V_InclusiveBound(PosY,0,E.SizeY)),"G_BlitEcranRect: Rectancle non inscrit dans l'ecran",return);
	CHECK((!V_InclusiveBound(PosX+SizeX,0,E.SizeX))||(!V_InclusiveBound(PosY+SizeY,0,E.SizeY)),"G_BlitEcranRect: Rectancle non inscrit dans l'ecran",return);
#ifdef G_ECRAN_DIBSECT
	if (E.Etat&G_ECRAN_DIBSECT)
	{
		CHECK(BitBlt((HDC)E.HECR,E.PosX+PosX,E.PosY+PosY,SizeX,SizeY,(HDC)E.hCompatDC,PosX,PosY,SRCCOPY)==0,"BitBlt echoue",return);
	}
	else 
#endif
#ifdef G_ECRAN_CBMP
		if (E.Etat&G_ECRAN_CBMP)
	{
		CHECK(BitBlt((HDC)E.HECR,E.PosX+PosX,E.PosY+PosY,SizeX,SizeY,(HDC)E.hCompatDC,PosX,PosY,SRCCOPY)==0,"BitBlt echoue",return);
	}
	else 
#endif
#ifdef G_ECRAN_SETDIB
		if (E.Etat&G_ECRAN_SETDIB) 
	{
		CHECK(SetDIBitsToDevice((HDC)E.HECR,E.PosX+PosX,E.PosY+PosY,SizeX,SizeY,PosX,E.SizeY-PosY-SizeY,0,E.SizeY,E.MECR,(BITMAPINFO*)E.bmpinf,DIB_RGB_COLORS)==0,"SetDiBitsToDevice echoue",return);
	}
		else
#endif
#ifdef SPG_General_PGLib
		if (E.Etat&G_ECRAN_PGL) 
	{
		pglBlitSurface(Global.display,0,E.surface,E.PosX+PosX,E.PosY+PosY,SizeX,SizeY,PosX,PosY,0);
	}
#endif
	{
		DbgCHECK(1,"G_BlitEcranRect : Ecran type invalide");
	}
	CD_G_CHECK_EXIT(17,5);
	return;
}

int SPG_CONV G_BlitFromMem(G_Ecran& E,int PosX,int PosY, BYTE* Button, int Pitch, int SizeX,int SizeY)
{
	TRY_BEGIN
	CHECK(E.Etat==0,"G_BlitFromMem",return 0);
	CHECK(V_IsBound(PosX,0,E.SizeX)==0,"G_BlitFromMem: Mauvais placement X",return 0);
	CHECK(V_IsBound(PosY,0,E.SizeY)==0,"G_BlitFromMem: Mauvais placement Y",return 0);
	CHECK(V_InclusiveBound(PosX+SizeX,0,E.SizeX)==0,"G_BlitFromMem: Mauvais placement X",return 0);
	CHECK(V_InclusiveBound(PosY+SizeY,0,E.SizeY)==0,"G_BlitFromMem: Mauvais placement Y",return 0);
#ifdef DebugGraphics
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_BlitFromMem: G_Ecran inaccessible",return 0);
	CHECK(Button==0,"G_BlitFromMem: Rien à copier",return 0);
	CHECK(V_IsBound(PosX,0,E.SizeX)==0,"G_BlitFromMem: Mauvais placement X",return 0);
	CHECK(V_IsBound(PosY,0,E.SizeY)==0,"G_BlitFromMem: Mauvais placement Y",return 0);
	CHECK(V_InclusiveBound(PosX+SizeX,0,E.SizeX)==0,"G_BlitFromMem: Mauvais placement X",return 0);
	CHECK(V_InclusiveBound(PosY+SizeY,0,E.SizeY)==0,"G_BlitFromMem: Mauvais placement Y",return 0);
#endif


	int LLine=SizeX*E.POCT;
	BYTE* BLine=Button;
	BYTE* ELine=PixEcrPTR(E,PosX,PosY);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		memcpy(ELine,BLine,LLine);
		BLine+=Pitch;
		ELine+=E.Pitch;
	}

	return -1;
	TRY_ENDGRZ("G_BlitFromMem")
}

int SPG_CONV G_BlitNEFromMem(G_Ecran& E,int X,int Y, BYTE*Button, int Pitch, int SizeX,int SizeY, PixCoul CoulTransp)
{
#ifdef DebugGraphics
	CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"G_BlitNEFromMem: G_Ecran inaccessible",return 0);
	CHECK(Button==0,"G_BlitFromMem: Rien à copier",return 0);
	CHECK(V_IsBound(X,0,E.SizeX)==0,"G_BlitFromMem: Mauvais placement X",return 0);
	CHECK(V_IsBound(Y,0,E.SizeY)==0,"G_BlitFromMem: Mauvais placement Y",return 0);
	CHECK(V_InclusiveBound(X+SizeX,0,E.SizeX)==0,"G_BlitFromMem: Mauvais placement X",return 0);
	CHECK(V_InclusiveBound(Y+SizeY,0,E.SizeY)==0,"G_BlitFromMem: Mauvais placement Y",return 0);
#endif


	int LLine=SizeX*E.POCT;
	if (E.POCT==4)
	{
	DWORD* BLine=(DWORD*)Button;
	DWORD* ELine=(DWORD*)PixEcrPTR(E,X,Y);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if (BLine[xt]!=CoulTransp.Coul) ELine[xt]=BLine[xt];
		}
		(*(int*)&BLine)+=Pitch;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==3)
	{
	BYTE* BLine=Button;
	BYTE* ELine=PixEcrPTR(E,X,Y);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<LLine;xt+=3)
		{
			if(memcmp(BLine+xt,&CoulTransp,3))
				*(PixCoul24*)(ELine+xt)=*(PixCoul24*)(BLine+xt);
		}
		(*(int*)&BLine)+=Pitch;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==2)
	{
	WORD CoulTranspL=G_Make16From24(CoulTransp);
	WORD* BLine=(WORD*)Button;
	WORD* ELine=(WORD*)PixEcrPTR(E,X,Y);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if (BLine[xt]!=CoulTranspL) ELine[xt]=BLine[xt];
		}
		(*(int*)&BLine)+=Pitch;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	else if (E.POCT==1)
	{
	BYTE CoulTranspL=G_Make8From24(CoulTransp);
	BYTE* BLine=Button;
	BYTE* ELine=PixEcrPTR(E,X,Y);
	int yt;
	for(yt=0;yt<SizeY;yt++)
	{
		int xt;
		for(xt=0;xt<SizeX;xt++)
		{
			if (BLine[xt]!=CoulTranspL) ELine[xt]=BLine[xt];
		}
		(*(int*)&BLine)+=Pitch;
		(*(int*)&ELine)+=E.Pitch;
	}
	}
	return -1;
}

int SPG_CONV G_Copy(G_Ecran& EDest, G_Ecran& ESrc, int xdest, int ydest)
{
	CHECK(ESrc.Etat==0,"G_Copy: G_Ecran vide",return 0);
	CHECK(EDest.Etat==0,"G_Copy: G_Ecran vide",return 0);
	CHECK((ESrc.Etat&G_MEMORYAVAILABLE)==0,"G_Copy: G_Ecran inaccessible",return 0);
	CHECK((EDest.Etat&G_MEMORYAVAILABLE)==0,"G_Copy: G_Ecran inaccessible",return 0);
	CHECK(xdest<0,"G_Copy: position invalide",return 0);
	CHECK(ydest<0,"G_Copy: position invalide",return 0);
	CHECK(G_SizeX(EDest)<G_SizeX(ESrc)+xdest,"G_Copy: G_Ecran destination trop petit",return 0);
	CHECK(G_SizeY(EDest)<G_SizeY(ESrc)+ydest,"G_Copy: G_Ecran destination trop petit",return 0);
	BYTE* DestLine=G_GetPix(EDest)+xdest*EDest.POCT+ydest*EDest.Pitch;
	BYTE* SrcLine=G_GetPix(ESrc);
	int LLine=ESrc.POCT*G_SizeX(ESrc);
	for(int y=0;y<G_SizeY(ESrc);y++)
	{
		BYTE* DestCol=DestLine;
		BYTE* SrcCol=SrcLine;
		if(ESrc.POCT==EDest.POCT)
		{
			memcpy(DestCol,SrcCol,LLine);
		}
		else if((ESrc.POCT==1)&&(EDest.POCT>=3))
		{
			for(int x=0;x<G_SizeX(ESrc);x++)
			{
				DestCol[2]=DestCol[1]=DestCol[0]=SrcCol[x];
				DestCol+=EDest.POCT;
			}
		}
		else if((ESrc.POCT>=3)&&(EDest.POCT==1))
		{
			for(int x=0;x<G_SizeX(ESrc);x++)
			{
				DestCol[x]=(SrcCol[0]+(SrcCol[1]<<1)+SrcCol[2])>>2;
				SrcCol+=ESrc.POCT;
			}
		}
		else if(((ESrc.POCT==3)&&(EDest.POCT==4))||((ESrc.POCT==4)&&(EDest.POCT==3)))
		{
		for(int x=0;x<G_SizeX(ESrc);x++)
		{
			*(PixCoul24*)DestCol=*(PixCoul24*)SrcCol;
			DestCol+=EDest.POCT;
			SrcCol+=ESrc.POCT;
		}
		}
		DestLine+=EDest.Pitch;
		SrcLine+=ESrc.Pitch;
	}
	return -1;
}

int SPG_CONV G_BlurCopy(G_Ecran& EDest, G_Ecran& ESrc, int xdest, int ydest)
{
	CHECK(ESrc.Etat==0,"G_BlurCopy: G_Ecran vide",return 0);
	CHECK(EDest.Etat==0,"G_BlurCopy: G_Ecran vide",return 0);
	CHECK((ESrc.Etat&G_MEMORYAVAILABLE)==0,"G_BlurCopy: G_Ecran inaccessible",return 0);
	CHECK((EDest.Etat&G_MEMORYAVAILABLE)==0,"G_BlurCopy: G_Ecran inaccessible",return 0);
	CHECK(ESrc.MECR==0,"G_BlurCopy",return 0);
	CHECK(EDest.MECR==0,"G_BlurCopy",return 0);
	CHECK(xdest<0,"G_BlurCopy: position invalide",return 0);
	CHECK(ydest<0,"G_BlurCopy: position invalide",return 0);
	CHECK(G_SizeX(EDest)<G_SizeX(ESrc)+xdest,"G_BlurCopy: G_Ecran destination trop petit",return 0);
	CHECK(G_SizeY(EDest)<G_SizeY(ESrc)+ydest,"G_BlurCopy: G_Ecran destination trop petit",return 0);
	BYTE* DestLine=G_GetPix(EDest)+xdest*EDest.POCT+ydest*EDest.Pitch;
	BYTE* SrcLine=G_GetPix(ESrc);
	if((EDest.POCT==4)&&(ESrc.POCT==4))
	{
	for(int y=0;y<G_SizeY(ESrc);y++)
	{
		DWORD* DestCol=(DWORD*)DestLine;
		DWORD* SrcCol=(DWORD*)SrcLine;
		for(int x=0;x<G_SizeX(ESrc);x++)
		{
			DestCol[x]=
				(
				(SrcCol[x]>>1)
				&0x7F7F7F7F)
				+
				(
				(DestCol[x]>>1)
				&0x7F7F7F7F);
		}
		((BYTE*)DestLine)+=EDest.Pitch;
		((BYTE*)SrcLine)+=ESrc.Pitch;
	}
	}
	else if((EDest.POCT==3)&&(ESrc.POCT==3))
	{
	for(int y=0;y<G_SizeY(ESrc);y++)
	{
		DWORD* DestCol=(DWORD*)DestLine;
		DWORD* SrcCol=(DWORD*)SrcLine;
		for(int x=0;x<(G_SizeX(ESrc)*3)/4;x++)
		{
			DestCol[x]=
				(
				(SrcCol[x]>>1)
				&0x7F7F7F7F)
				+
				(
				(DestCol[x]>>1)
				&0x7F7F7F7F);
		}
		((BYTE*)DestLine)+=EDest.Pitch;
		((BYTE*)SrcLine)+=ESrc.Pitch;
	}
	}
	else if((EDest.POCT==4)&&(ESrc.POCT==3))
	{
	for(int y=0;y<G_SizeY(ESrc);y++)
	{
		DWORD* DestCol=(DWORD*)DestLine;
		BYTE* SrcCol=SrcLine;
		for(int x=0;x<G_SizeX(ESrc)-1;x++)//a cause de l'acces dword
		{
			DestCol[x]=
				(
				((*(DWORD*)(SrcCol+3*x))>>1)
				&0x7F7F7F7F)
				+
				(
				(DestCol[x]>>1)
				&0x7F7F7F7F);
		}
		((BYTE*)DestLine)+=EDest.Pitch;
		((BYTE*)SrcLine)+=ESrc.Pitch;
	}
	}
	return -1;
}

int SPG_CONV G_ResampleNCopy(G_Ecran& EDest, G_Ecran& ESrc, int N)
{
	CHECK(ESrc.Etat==0,"G_ResampleCopy: G_Ecran vide",return 0);
	CHECK(EDest.Etat==0,"G_ResampleCopy: G_Ecran vide",return 0);
	CHECK((ESrc.Etat&G_MEMORYAVAILABLE)==0,"G_ResampleCopy: G_Ecran inaccessible",return 0);
	CHECK((EDest.Etat&G_MEMORYAVAILABLE)==0,"G_ResampleCopy: G_Ecran inaccessible",return 0);
	CHECK(N*G_SizeX(EDest)<G_SizeX(ESrc),"G_ResampleCopy: G_Ecran destination trop petit",return 0);
	CHECK(N*G_SizeY(EDest)<G_SizeY(ESrc),"G_ResampleCopy: G_Ecran destination trop petit",return 0);
	CHECK((ESrc.POCT!=3)&&(ESrc.POCT!=4),"G_ResampleCopy: Profondeur de couleur source non supportee",return 0);
	CHECK((EDest.POCT!=3)&&(EDest.POCT!=4),"G_ResampleCopy: Profondeur de couleur destination non supportee",return 0);
	BYTE* DestLine=G_GetPix(EDest);
	BYTE* SrcLine=G_GetPix(ESrc);
	for(int y=0;y<G_SizeY(ESrc);y+=N)
	{
		BYTE* DestCol=DestLine;
		BYTE* SrcCol=SrcLine;
		for(int x=0;x<G_SizeX(ESrc);x+=N)
		{
			*(PixCoul24*)DestCol=*(PixCoul24*)SrcCol;
			DestCol+=EDest.POCT;
			SrcCol+=N*ESrc.POCT;
		}
		DestLine+=EDest.Pitch;
		SrcLine+=N*ESrc.Pitch;
	}
	return -1;
}


int SPG_CONV G_DownsampleNCopy(G_Ecran& EDest, G_Ecran& ESrc, int N)
{
	CHECK(ESrc.Etat==0,"G_ResampleCopy: G_Ecran vide",return 0);
	CHECK(EDest.Etat==0,"G_ResampleCopy: G_Ecran vide",return 0);
	CHECK((ESrc.Etat&G_MEMORYAVAILABLE)==0,"G_ResampleCopy: G_Ecran inaccessible",return 0);
	CHECK((EDest.Etat&G_MEMORYAVAILABLE)==0,"G_ResampleCopy: G_Ecran inaccessible",return 0);
	//CHECK(G_SizeX(EDest)<(G_SizeX(ESrc)/N),"G_ResampleCopy: G_Ecran destination trop petit",return 0);
	//CHECK(G_SizeY(EDest)<(G_SizeY(ESrc)/N),"G_ResampleCopy: G_Ecran destination trop petit",return 0);
	CHECK((ESrc.POCT==2)||(EDest.POCT==2),"G_ResampleCopy: Profondeur de couleur source non supportee",return 0);
	//CHECK((ESrc.POCT==1)&&(EDest.POCT!=1),"G_ResampleCopy: Profondeur de couleur destination non supportee",return 0);
	//CHECK((EDest.POCT==3)&&(ESrc.POCT!=3),"G_ResampleCopy: Profondeur de couleur destination non supportee",return 0);

	int SizeX=N*(V_Min(G_SizeX(EDest),(G_SizeX(ESrc)/N)));
	int SizeY=N*(V_Min(G_SizeY(EDest),(G_SizeY(ESrc)/N)));

	if((ESrc.POCT==3)&&(EDest.POCT==4))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<=SizeY-N;y+=N)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<=SizeX-N;x+=N)
			{
				for(int k=0;k<3;k++)
				{
					int Sum=0;
					for(int yi=0;yi<N;yi++)
					{
						for(int xi=0;xi<N;xi++)
						{
							Sum+=SrcCol[3*xi+yi*ESrc.Pitch+k];
						}
					}
					DestCol[k]=Sum/(N*N);
				}
				DestCol+=4;
				SrcCol+=3*N;
			}
			DestLine+=EDest.Pitch;
			SrcLine+=N*ESrc.Pitch;
		}
	}
	else if((ESrc.POCT==3)&&(EDest.POCT==3))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<=SizeY-N;y+=N)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<=SizeX-N;x+=N)
			{
				for(int k=0;k<3;k++)
				{
					int Sum=0;
					for(int yi=0;yi<N;yi++)
					{
						for(int xi=0;xi<N;xi++)
						{
							Sum+=SrcCol[3*xi+yi*ESrc.Pitch+k];
						}
					}
					DestCol[k]=Sum/(N*N);
				}
				DestCol+=3;
				SrcCol+=3*N;
			}
			DestLine+=EDest.Pitch;
			SrcLine+=N*ESrc.Pitch;
		}
	}
	else if((ESrc.POCT==1)&&(EDest.POCT==1))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<=SizeY-N;y+=N)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<=SizeX-N;x+=N)
			{
				int Sum=0;
				for(int yi=0;yi<N;yi++)
				{
					for(int xi=0;xi<N;xi++)
					{
						Sum+=SrcCol[xi+yi*ESrc.Pitch];
					}
				}
				*DestCol=Sum/(N*N);
				DestCol++;
				SrcCol+=N;
			}
			DestLine+=EDest.Pitch;
			SrcLine+=N*ESrc.Pitch;
		}
	}
	else if((ESrc.POCT==1)&&(EDest.POCT==3))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<=SizeY-N;y+=N)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<=SizeX-N;x+=N)
			{
				int Sum=0;
				for(int yi=0;yi<N;yi++)
				{
					for(int xi=0;xi<N;xi++)
					{
						Sum+=SrcCol[xi+yi*ESrc.Pitch];
					}
				}
				*(DestCol+2)=*(DestCol+1)=*DestCol=Sum/(N*N);
				DestCol+=3;
				SrcCol+=N;
			}
			DestLine+=EDest.Pitch;
			SrcLine+=N*ESrc.Pitch;
		}
	}
	else if((ESrc.POCT==1)&&(EDest.POCT==4))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<=SizeY-N;y+=N)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<=SizeX-N;x+=N)
			{
				int Sum=0;
				for(int yi=0;yi<N;yi++)
				{
					for(int xi=0;xi<N;xi++)
					{
						Sum+=SrcCol[xi+yi*ESrc.Pitch];
					}
				}
				*(DestCol+3)=*(DestCol+2)=*(DestCol+1)=*DestCol=Sum/(N*N);
				DestCol+=4;
				SrcCol+=N;
			}
			DestLine+=EDest.Pitch;
			SrcLine+=N*ESrc.Pitch;
		}
	}
	else if((ESrc.POCT==3)&&(EDest.POCT==1))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<=SizeY-N;y+=N)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<=SizeX-N;x+=N)
			{
				int Sum=0;
				for(int yi=0;yi<N;yi++)
				{
					for(int xi=0;xi<N;xi++)
					{
						Sum+=
							SrcCol[3*xi+yi*ESrc.Pitch]+
							2*SrcCol[3*xi+1+yi*ESrc.Pitch]+
							SrcCol[3*xi+2+yi*ESrc.Pitch];
					}
				}
				*DestCol=Sum/(4*N*N);
				DestCol++;
				SrcCol+=3*N;
			}
			DestLine+=EDest.Pitch;
			SrcLine+=N*ESrc.Pitch;
		}
	}
	else
	{
		DbgCHECK(1,"G_DownsampleNCopy: profondeur de couleur non supportée");
	}
	return -1;
}

int SPG_CONV G_OversampleNCopy(G_Ecran& EDest, G_Ecran& ESrc, int N)
{
	CHECK(ESrc.Etat==0,"G_ResampleCopy: G_Ecran vide",return 0);
	CHECK(EDest.Etat==0,"G_ResampleCopy: G_Ecran vide",return 0);
	CHECK((ESrc.Etat&G_MEMORYAVAILABLE)==0,"G_OversampleNCopy: G_Ecran inaccessible",return 0);
	CHECK((EDest.Etat&G_MEMORYAVAILABLE)==0,"G_OversampleNCopy: G_Ecran inaccessible",return 0);
	CHECK(G_SizeX(EDest)<N*G_SizeX(ESrc),"G_OversampleNCopy: G_Ecran destination trop petit",return 0);
	CHECK(G_SizeY(EDest)<N*G_SizeY(ESrc),"G_OversampleNCopy: G_Ecran destination trop petit",return 0);

	if((ESrc.POCT==1)&&(EDest.POCT==1))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<G_SizeY(ESrc);y++)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<G_SizeX(ESrc);x++)
			{
				BYTE* iyDestLine=DestCol;
				for(int iy=0;iy<N;iy++)
				{
					BYTE* ixDestCol=iyDestLine;
					for(int ix=0;ix<N;ix++)
					{
						*ixDestCol=*SrcCol;
						ixDestCol++;
					}
					iyDestLine+=EDest.Pitch;
				}
				DestCol+=N;
				SrcCol++;
			}
			DestLine+=N*EDest.Pitch;
			SrcLine+=ESrc.Pitch;
		}
	}
	else if((ESrc.POCT==1)&&(EDest.POCT>=3))
	{
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<G_SizeY(ESrc);y++)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<G_SizeX(ESrc);x++)
			{
				BYTE* iyDestLine=DestCol;
				for(int iy=0;iy<N;iy++)
				{
					BYTE* ixDestCol=iyDestLine;
					for(int ix=0;ix<N;ix++)
					{
						//*ixDestCol=*SrcCol;
						memset(ixDestCol,*SrcCol,EDest.POCT);
						ixDestCol+=EDest.POCT;
					}
					iyDestLine+=EDest.Pitch;
				}
				DestCol+=N*EDest.POCT;
				SrcCol++;
			}
			DestLine+=N*EDest.Pitch;
			SrcLine+=ESrc.Pitch;
		}
	}
	else
	{
		CHECK((ESrc.POCT!=3)&&(ESrc.POCT!=4),"G_OversampleNCopy: Profondeur de couleur source non supportee",return 0);
		CHECK((EDest.POCT!=3)&&(EDest.POCT!=4),"G_OversampleNCopy: Profondeur de couleur destination non supportee",return 0);
		BYTE* DestLine=G_GetPix(EDest);
		BYTE* SrcLine=G_GetPix(ESrc);
		for(int y=0;y<G_SizeY(ESrc);y++)
		{
			BYTE* DestCol=DestLine;
			BYTE* SrcCol=SrcLine;
			for(int x=0;x<G_SizeX(ESrc);x++)
			{
				BYTE* iyDestLine=DestCol;
				for(int iy=0;iy<N;iy++)
				{
					BYTE* ixDestCol=iyDestLine;
					for(int ix=0;ix<N;ix++)
					{
						*(PixCoul24*)ixDestCol=*(PixCoul24*)SrcCol;
						ixDestCol+=EDest.POCT;
					}
					iyDestLine+=EDest.Pitch;
				}
				DestCol+=N*EDest.POCT;
				SrcCol+=ESrc.POCT;
			}
			DestLine+=N*EDest.Pitch;
			SrcLine+=ESrc.Pitch;
		}
	}
	return -1;
}

void SPG_CONV G_Convolve(G_Ecran& E, int* Filter, int SizeX, int SizeY)
{
	CHECK(E.Etat==0,"G_Convolve",return);
	CHECK(Filter==0,"G_Convolve",return);
	int Norm=0;
	for(int i=0;i<SizeX*SizeY;i++)
	{
		Norm+=Filter[i];
	}
	CHECK(Norm==0,"G_Convolve",return);
	for(int y=0;y<G_SizeY(E)-SizeY;y++)
	{
		for(int x=0;x<G_SizeX(E)-SizeX;x++)
		{
			for(int p=0;p<G_POCT(E);p++)
			{
				int BSum=0;
				for(int yl=0;yl<SizeY;yl++)
				{
					for(int xl=0;xl<SizeX;xl++)
					{
						BSum+=G_MECR(E)[p+(x+xl)*G_POCT(E)+(y+yl)*G_Pitch(E)]*Filter[xl+yl*SizeX];
					}
				}
				BSum/=Norm;
				G_MECR(E)[p+x*G_POCT(E)+y*G_Pitch(E)]=V_Sature(BSum,0,255);
			}
		}
	}
	return;
}

void SPG_CONV G_DrawCircle(G_Ecran& E, int xc, int yc, int r, DWORD Coul, int Sect)
{
	if(Sect<=0) Sect=(r+15)/3;
	int LastX=xc;
	int LastY=yc+r;
	for(int S=1;S<=Sect;S++)
	{
		int X=xc+r*sin(V_DPI*S/Sect);
		int Y=yc+r*cos(V_DPI*S/Sect);
		G_DrawLine(E,X,Y,LastX,LastY,Coul);
		LastX=X;
		LastY=Y;
	}
	return;
}

void SPG_CONV G_DrawCircleF(G_Ecran& E, float xc, float yc, float r, DWORD Coul, float Sect)
{
	if(Sect<=0) Sect=(r+15)/3;
	float LastX=xc;
	float LastY=yc+r;
	for(int S=1;S<=Sect;S++)
	{
		float X=xc+r*sin(V_DPI*S/Sect);
		float Y=yc+r*cos(V_DPI*S/Sect);
		G_DrawLine(E,X,Y,LastX,LastY,Coul);
		LastX=X;
		LastY=Y;
	}
	return;
}

#undef DEF_G_RENDERMODE

#endif

