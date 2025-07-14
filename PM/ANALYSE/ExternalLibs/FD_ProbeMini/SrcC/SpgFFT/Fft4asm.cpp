
#include "..\SPG_General.h"

//#define WIN32_LEAN_AND_MEAN
/* Family of radix 4 DIF FFTs for specific block sizes
 * August 1995, Phil Karn, KA9Q
 */
/*
fft1024=64*4*iFFT4GbigStep+4*fft256
fft256=16*4*iFFT4GbigStep+4*fft64
fft64=16*iFFT4GbigStep+4*ifft16
ifft16=4*FFT4G0+4*FFT4Z
FFT4GBigStep=FFT4G0
256G+4*(64G+4*(16G+4*(4G+4Z)))=
256G+4*(64G+4*(16G+16G+16Z))=
256G+4*(64G+4*(32G+16Z))=
256G+4*(64G+128G+64Z))=
256G+4*(192G+64Z))=
256G+768G+256Z=
1024G+256Z
1024G=
//108->110592 instructions

//mov 5
//add 4
//lea 2
//cmp 1
//jc 1
//fld 33
//fadd 22 (+11-11)
//fmul 12
//fxch 14
//fstp 14

256Z=
//46->11776 instructions

//mov 1
//add 1
//lea 0
//fld 15
//fadd 16 (+8-8)
//fmul 0
//fxch 5
//fstp 8

  TOTAL
122368instructions
*/
#include "..\WhatFFT.h"
/*
#ifndef UseFFT4ASM
#error UseFFT4ASM non défini
#endif
*/
#ifdef UseFFT4ASM

#pragma SPGMSG(__FILE__,__LINE__, "using FFT4 asm" )

#pragma warning( disable : 4514 )//unreferenced inline
#pragma warning( disable : 4201 )//nameless struct/union

#include <math.h>
#include "..\FFT.h"
#include "fft4asm.h"

extern float PI,DPI;

float TWIDDLES[MAXPOINTS];
float TWIDDLESfast[MAXPOINTSfast];

/*
void
fft2(SPG_COMPLEX x[2])
{
	SPG_COMPLEX tmp;

	tmp.re = x[0].re - x[1].re;
	tmp.im = x[0].im - x[1].im;
	x[0].re += x[1].re;
	x[0].im += x[1].im;
	x[1] = tmp;
return;
}
*/


/* FFT with arbitrary power-of-4 blocksize */
//fftn
void FFTCC
fftn(SPG_COMPLEX x[],int nu)
{
	int n,i,n4,tstep;

	/* Handle the values of nu that have specific functions */
	switch(nu){
	case 0:
		break;
	case 1:
		iFFT4GbigStep(x,8,4);
		break;
	case 2:
		fft16(x);
		break;
	case 3:
		fft64(x);
		break;
	case 4:
		fft256(x);
		break;
	case 5:
		fft1024(x);
		break;		
	default:
		n = (1 << (nu<<1));
		n4 = (n>>2);
		tstep = (MAXPOINTS >> (nu<<1));
		for(i=0;i<n4;i+=4)
		{
			iFFT4G(x+i,8*n4,4*i*tstep);
			iFFT4G(x+i+1,8*n4,4*i*tstep+4*tstep);
			iFFT4G(x+i+2,8*n4,4*i*tstep+8*tstep);
			iFFT4G(x+i+3,8*n4,4*i*tstep+12*tstep);
		}

		fftn(x,nu-1);
		fftn(x+n4,nu-1);
		fftn(x+2*n4,nu-1);
		fftn(x+3*n4,nu-1);
	}
	return;
}
//fftn

//fft1024
void FFTCC
fft1024(SPG_COMPLEX x[1024])
{
	int i;

#if ((MAXPOINTSfast<1024)||!AllowBigStep)
	for(i=0;i<256;i+=4)
	{
		iFFT4G(x+i,8*256,i*4*MAXPOINTS/1024);
		iFFT4G(x+i+1,8*256,i*4*MAXPOINTS/1024+4*MAXPOINTS/1024);
		iFFT4G(x+i+2,8*256,i*4*MAXPOINTS/1024+8*MAXPOINTS/1024);
		iFFT4G(x+i+3,8*256,i*4*MAXPOINTS/1024+12*MAXPOINTS/1024);
	}
#else
	for(i=0;i<256;i+=4)
	{
		iFFT4GbigStep(x+i,8*256,i*4*MAXPOINTSfast/1024);
		iFFT4GbigStep(x+i+1,8*256,i*4*MAXPOINTSfast/1024+4*MAXPOINTSfast/1024);
		iFFT4GbigStep(x+i+2,8*256,i*4*MAXPOINTSfast/1024+8*MAXPOINTSfast/1024);
		iFFT4GbigStep(x+i+3,8*256,i*4*MAXPOINTSfast/1024+12*MAXPOINTSfast/1024);
	}
#endif
		fft256(x);
		fft256(x+256);
		fft256(x+512);
		fft256(x+768);
return;
}
//fft1024

//fft256
void FFTCC
fft256(SPG_COMPLEX x[256])
{
	int i;

#if ((MAXPOINTSfast<256)||!AllowBigStep)
	for(i=0;i<64;i+=4)
	{
		iFFT4G(x+i,8*64,i*4*MAXPOINTS/256);
		iFFT4G(x+i+1,8*64,i*4*MAXPOINTS/256+4*MAXPOINTS/256);
		iFFT4G(x+i+2,8*64,i*4*MAXPOINTS/256+8*MAXPOINTS/256);
		iFFT4G(x+i+3,8*64,i*4*MAXPOINTS/256+12*MAXPOINTS/256);
	}
#else
	for(i=0;i<64;i+=4)
	{
		iFFT4GbigStep(x+i,8*64,i*4*MAXPOINTSfast/256);
		iFFT4GbigStep(x+i+1,8*64,i*4*MAXPOINTSfast/256+4*MAXPOINTSfast/256);
		iFFT4GbigStep(x+i+2,8*64,i*4*MAXPOINTSfast/256+8*MAXPOINTSfast/256);
		iFFT4GbigStep(x+i+3,8*64,i*4*MAXPOINTSfast/256+12*MAXPOINTSfast/256);
	}
#endif
		fft64(x);
		fft64(x+64);
		fft64(x+128);
		fft64(x+192);
return;
}
//fft256

//fft64
void FFTCC
fft64(SPG_COMPLEX x[64])
{
#if ((MAXPOINTSfast<64)||!AllowBigStep)
	iFFT4G(x,8*16,4*0);
	iFFT4G(x+1,8*16,4*MAXPOINTS/64);
	iFFT4G(x+2,8*16,4*2*MAXPOINTS/64);
	iFFT4G(x+3,8*16,4*3*MAXPOINTS/64);
	iFFT4G(x+4,8*16,4*4*MAXPOINTS/64);
	iFFT4G(x+5,8*16,4*5*MAXPOINTS/64);
	iFFT4G(x+6,8*16,4*6*MAXPOINTS/64);
	iFFT4G(x+7,8*16,4*7*MAXPOINTS/64);
	iFFT4G(x+8,8*16,4*8*MAXPOINTS/64);
	iFFT4G(x+9,8*16,4*9*MAXPOINTS/64);
	iFFT4G(x+10,8*16,4*10*MAXPOINTS/64);
	iFFT4G(x+11,8*16,4*11*MAXPOINTS/64);
	iFFT4G(x+12,8*16,4*12*MAXPOINTS/64);
	iFFT4G(x+13,8*16,4*13*MAXPOINTS/64);
	iFFT4G(x+14,8*16,4*14*MAXPOINTS/64);
	iFFT4G(x+15,8*16,4*15*MAXPOINTS/64);
#else
	iFFT4GbigStep(x,8*16,4*0);
	iFFT4GbigStep(x+1,8*16,4*MAXPOINTSfast/64);
	iFFT4GbigStep(x+2,8*16,4*2*MAXPOINTSfast/64);
	iFFT4GbigStep(x+3,8*16,4*3*MAXPOINTSfast/64);
	iFFT4GbigStep(x+4,8*16,4*4*MAXPOINTSfast/64);
	iFFT4GbigStep(x+5,8*16,4*5*MAXPOINTSfast/64);
	iFFT4GbigStep(x+6,8*16,4*6*MAXPOINTSfast/64);
	iFFT4GbigStep(x+7,8*16,4*7*MAXPOINTSfast/64);
	iFFT4GbigStep(x+8,8*16,4*8*MAXPOINTSfast/64);
	iFFT4GbigStep(x+9,8*16,4*9*MAXPOINTSfast/64);
	iFFT4GbigStep(x+10,8*16,4*10*MAXPOINTSfast/64);
	iFFT4GbigStep(x+11,8*16,4*11*MAXPOINTSfast/64);
	iFFT4GbigStep(x+12,8*16,4*12*MAXPOINTSfast/64);
	iFFT4GbigStep(x+13,8*16,4*13*MAXPOINTSfast/64);
	iFFT4GbigStep(x+14,8*16,4*14*MAXPOINTSfast/64);
	iFFT4GbigStep(x+15,8*16,4*15*MAXPOINTSfast/64);
#endif

	/*
	#if ((MAXPOINTSfast<64)||!AllowBigStep)
	for(i=0;i<16;i+=4)
	{
		iFFT4G(x+i,8*16,i*4*MAXPOINTS/64);
		iFFT4G(x+i+1,8*16,i*4*MAXPOINTS/64+4*MAXPOINTS/64);
		iFFT4G(x+i+2,8*16,i*4*MAXPOINTS/64+8*MAXPOINTS/64);
		iFFT4G(x+i+3,8*16,i*4*MAXPOINTS/64+12*MAXPOINTS/64);
	}
	#else
	for(i=0;i<16;i+=4)
	{
		iFFT4GbigStep(x+i,8*16,i*4*MAXPOINTSfast/64);
		iFFT4GbigStep(x+i+1,8*16,i*4*MAXPOINTSfast/64+4*MAXPOINTSfast/64);
		iFFT4GbigStep(x+i+2,8*16,i*4*MAXPOINTSfast/64+8*MAXPOINTSfast/64);
		iFFT4GbigStep(x+i+3,8*16,i*4*MAXPOINTSfast/64+12*MAXPOINTSfast/64);
	}
	#endif
	*/

	ifft16(x);
	ifft16(x+16);
	ifft16(x+32);
	ifft16(x+48);
return;
}
//fft64

//ifft16
void __inline
ifft16(SPG_COMPLEX x[16])
{
#if ((MAXPOINTSfast<16)||!AllowBigStep)
	iFFT4G(x,8*4,4*0);
	iFFT4G(x+1,8*4,4*MAXPOINTS/16);
	iFFT4G(x+2,8*4,4*2*MAXPOINTS/16);
	iFFT4G(x+3,8*4,4*3*MAXPOINTS/16);
#else
#undef COS
#undef SIN
#undef MAXPOINTS_I
#define	COS(x) float ptr TWIDDLESfast[(x)]
#define	SIN(x) float ptr TWIDDLESfast[(x)]
#define MAXPOINTS_I MAXPOINTSfast

#undef astep
#undef tstep
#undef AddToX
#undef fft4lbldone

#define astep 8*4
#define tstep 4*0
#define fft4lbldone fft4lbldone0
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#define astep 8*4
#define tstep 4*MAXPOINTSfast/16
#define fft4lbldone fft4lbldone1
#define AddToX 8
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#define astep 8*4
#define tstep 4*2*MAXPOINTSfast/16
#define fft4lbldone fft4lbldone2
#define AddToX 16
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#define astep 8*4
#define tstep 4*3*MAXPOINTSfast/16
#define fft4lbldone fft4lbldone3
#define AddToX 24
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#endif

	/*
	FFT4(x);
	FFT4(x+4);
	FFT4(x+8);
	FFT4(x+12);
	*/
#undef AddToX
#undef COS
#undef SIN
#define	COS(x) float ptr TWIDDLESfast[(x)]
#define	SIN(x) float ptr TWIDDLESfast[(x)]

#include "AsmFFT4Z.cpp"

#define AddToX 8*4
#include "AsmFFT4Z.cpp"
//add to X se cumule pour FFT4Z
#include "AsmFFT4Z.cpp"
//add to X se cumule pour FFT4Z
#include "AsmFFT4Z.cpp"
#undef AddToX

return;
}

/*
void __inline
fft4(SPG_COMPLEX x[4])
{
#include "AsmFFT4Z.cpp"
}
*/

#undef COS
#undef SIN
#undef MAXPOINTS_I
#define	COS(x) float ptr TWIDDLESfast[(x)]
#define	SIN(x) float ptr TWIDDLESfast[(x)]
#define MAXPOINTS_I MAXPOINTSfast
void __inline
iFFT4GbigStep(SPG_COMPLEX *x,int astep,int tstep)
{
#include "AsmFFT4GO.cpp"
}


#undef COS
#undef SIN
#undef MAXPOINTS_I
#define	COS(x) float ptr TWIDDLES[(x)]
#define	SIN(x) float ptr TWIDDLES[(x)]
#define MAXPOINTS_I MAXPOINTS

void __inline
iFFT4G(SPG_COMPLEX *x,int astep,int tstep)
{
#include "AsmFFT4GO.cpp"
}

void FFTCC
fft16(SPG_COMPLEX x[16])
{
	//int i;
#if ((MAXPOINTSfast<16)||!AllowBigStep)
	iFFT4G(x,8*4,4*0);
	iFFT4G(x+1,8*4,4*MAXPOINTS/16);
	iFFT4G(x+2,8*4,4*2*MAXPOINTS/16);
	iFFT4G(x+3,8*4,4*3*MAXPOINTS/16);
#else

#undef COS
#undef SIN
#undef MAXPOINTS_I
#define	COS(x) float ptr TWIDDLESfast[(x)]
#define	SIN(x) float ptr TWIDDLESfast[(x)]
#define MAXPOINTS_I MAXPOINTSfast

#undef astep
#undef tstep
#undef AddToX
#undef fft4lbldone

#define astep 8*4
#define tstep 4*0
#define fft4lbldone fft4lbldone0
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#define astep 8*4
#define tstep 4*MAXPOINTSfast/16
#define fft4lbldone fft4lbldone1
#define AddToX 8
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#define astep 8*4
#define tstep 4*2*MAXPOINTSfast/16
#define fft4lbldone fft4lbldone2
#define AddToX 16
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#define astep 8*4
#define tstep 4*3*MAXPOINTSfast/16
#define fft4lbldone fft4lbldone3
#define AddToX 24
#include "AsmFFT4GO.cpp"
#undef astep
#undef tstep
#undef fft4lbldone
#undef AddToX

#endif

#undef AddToX
#undef COS
#undef SIN
#define	COS(x) float ptr TWIDDLESfast[(x)]
#define	SIN(x) float ptr TWIDDLESfast[(x)]

#include "AsmFFT4Z.cpp"

#define AddToX 8*4
#include "AsmFFT4Z.cpp"

#include "AsmFFT4Z.cpp"

#include "AsmFFT4Z.cpp"
#undef AddToX

return;
}

void FFTCC
fftswap(SPG_COMPLEX x[],int NrTMax,int TMax)
{
	int PF01,PF11,TMaxS4;
	int PF10,PF10MPF01,PF11MPF10;
__asm
{
		mov eax,TMax//1024
		shl eax,3-1//SPG_COMPLEX*TMax /2=4096
		mov PF10,eax//4096
		shr eax,1//2048
		mov PF01,eax//2048
		or eax,PF10//6144
		mov PF11,eax//6144
		sub eax,PF10
		mov PF11MPF10,eax
		mov eax,PF10
		sub eax,PF01
		mov PF10MPF01,eax

		mov eax,TMax
		shr eax,2//256
		mov TMaxS4,eax//256 voire 128
		dec byte ptr NrTMax//5->4
//mov eax,0x00000001
xor eax,eax
mov cl,byte ptr NrTMax//4
mov edx,eax//0
mov ebx, eax//0
sw:
//00  00
//01  10
//10  01
//11  11
//1023=11.11.11.11.11=2+4*2

	revb:
	shr eax,1
	rcl edx,1 
	shr eax,1
	rcl edx,1 
	dec cl
	jnz revb//edx=76543210 de eax=XX01234567
mov eax,ebx//valeur directe/4 (increment 1 pour 4)
shl edx,3// *SPG_COMPLEX
shl eax,3+2//valeur directe*SPG_COMPLEX
add edx,x
add eax,x
push ebx

cmp edx,eax//edx >= eax rien a faire
jae nomov3

mov ebx,[eax]
mov ecx,[eax+4]
mov esi,[edx]
mov edi,[edx+4]
mov [eax],esi
			mov esi,edx
mov [eax+4],edi
			add eax,16
			add edx,PF01
mov [esi],ebx
			cmp edx,eax
mov [esi+4],ecx
			jae nomov3

mov ebx,[eax]
mov ecx,[eax+4]
mov esi,[edx]
mov edi,[edx+4]
mov [eax],esi
			mov esi,edx
mov [eax+4],edi
			add eax,8-16
			add edx,PF10MPF01
mov [esi],ebx
			cmp edx,eax
mov [esi+4],ecx
			jae nomov3

mov ebx,[eax]
mov ecx,[eax+4]
mov esi,[edx]
mov edi,[edx+4]
mov [eax],esi
			mov esi,edx
mov [eax+4],edi
			add eax,24-8
			add edx,PF11MPF10
mov [esi],ebx
			cmp edx,eax
mov [esi+4],ecx
			jae nomov3

mov ebx,[eax]
mov ecx,[eax+4]
mov esi,[edx]
mov edi,[edx+4]
mov [edx],ebx
mov [edx+4],ecx
mov [eax],esi
mov [eax+4],edi
nomov3:

pop eax
xor edx,edx
inc eax
mov cl,byte ptr NrTMax
cmp eax,TMaxS4
mov ebx,eax
jb sw
}
return;
}

void FFTCC
fftinit(void)
{
	int i;

	TWIDDLES[0] = 1;
	TWIDDLES[MAXPOINTS/2] = -1;
	for(i=1;i<=MAXPOINTS/4;i++){
		TWIDDLES[i] = (float)cos(2*PI*i/MAXPOINTS);
		TWIDDLES[MAXPOINTS-i] = TWIDDLES[i];
		TWIDDLES[MAXPOINTS/2-i] = -TWIDDLES[i];
		TWIDDLES[MAXPOINTS/2+i] = TWIDDLES[MAXPOINTS/2-i];
	}
	
	TWIDDLESfast[0] = 1;
	TWIDDLESfast[MAXPOINTSfast/2] = -1;
	for(i=1;i<=MAXPOINTSfast/4;i++){
		TWIDDLESfast[i] = (float)cos(2*PI*i/MAXPOINTSfast);
		TWIDDLESfast[MAXPOINTSfast-i] = TWIDDLESfast[i];
		TWIDDLESfast[MAXPOINTSfast/2-i] = -TWIDDLESfast[i];
		TWIDDLESfast[MAXPOINTSfast/2+i] = TWIDDLESfast[MAXPOINTSfast/2-i];
	}
return;
}

#endif

