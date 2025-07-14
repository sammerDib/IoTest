/* Family of radix 4 DIF FFTs for specific block sizes
 * August 1995, Phil Karn, KA9Q
 */

#include "..\SPG_General.h"

#include "..\WhatFFT.h"
/*
#ifndef UseFFT4C
#error UseFFT4C non défini
#endif
*/
#ifdef UseFFT4C

#pragma SPGMSG(__FILE__,__LINE__, "using FFT4 C" )

#pragma warning( disable : 4514 )

#include <math.h>
#include "..\FFT.h"
#include "fft4c.h"

#define complex SPG_COMPLEX

extern float PI;

#define	COS(x) Twiddles[x]
#define SIN(x) Twiddles[x+MAXPOINTS/4]

float Twiddles[MAXPOINTS];

void FFTCC
fftinit(void)
{
	int i;

	Twiddles[0] = 1;
	Twiddles[MAXPOINTS/2] = -1;
	for(i=1;i<=MAXPOINTS/4;i++){
		Twiddles[i] = (float)cos(2*PI*i/MAXPOINTS);
		Twiddles[MAXPOINTS-i] = Twiddles[i];
		Twiddles[MAXPOINTS/2-i] = -Twiddles[i];
		Twiddles[MAXPOINTS/2+i] = Twiddles[MAXPOINTS/2-i];
	}		
}
/* Bitswap element addresses of SPG_COMPLEX array */
void FFTCC
fftswap(SPG_COMPLEX x[],int n)
{
	int j,n1,i,k;
	SPG_COMPLEX t;

	j = n >> 1;
        n1 = n - 1;
	for(i=1;i<n1;i++){	/* No need to touch first & last */
		if(i<j){	/* Don't swap twice! */
			t = x[i];
			x[i] = x[j];
			x[j] = t;
		}
		/* Scan counter from left looking for set bits. Clear
		 * each with a subtract and propagate a carry
		 * to the right. Fall out when we find a 0 bit.
		 */
		for(k=n>>1;k <= j;k >>= 1)
			j -= k;

		j += k;		/* Add in the propagated carry */
	}
}
/* Reversed bit counter. Add 1 to the high end of j, with n as the max value */
/*
int FFTCC
revinc(int j,int n)
{
	int n1 = n-1;
	int k;

	if(j >= n-1)
		return j;
	for(k=n>>1;k<=j;k>>=1)
		j -=k;
	j += k;
	return j;
}
*/
/*
void __inline 
fft2(SPG_COMPLEX x[2])
{
	SPG_COMPLEX tmp;

	tmp.re = x[0].re - x[1].re;
	tmp.im = x[0].im - x[1].im;
	x[0].re += x[1].re;
	x[0].im += x[1].im;
	x[1] = tmp;
}
*/
//LES INLINE
//#ifndef	PENTIUM
/* 4-point FFT butterfly on adjacent elements, unity twiddles */
/*
void __inline 
ifft4(SPG_COMPLEX x[4])
{
	SPG_COMPLEX tmp[3];

	tmp[2].re = x[0].re - x[1].im - x[2].re + x[3].im;
	tmp[2].im = x[0].im + x[1].re - x[2].im - x[3].re;

	tmp[0].re = x[0].re - x[1].re + x[2].re - x[3].re;
	tmp[0].im = x[0].im - x[1].im + x[2].im - x[3].im;

	tmp[1].re = x[0].re + x[1].im - x[2].re - x[3].im;
	tmp[1].im = x[0].im - x[1].re - x[2].im + x[3].re;

	x[0].re = x[0].re + x[1].re + x[2].re + x[3].re;
	x[0].im = x[0].im + x[1].im + x[2].im + x[3].im;

	x[1] = tmp[0];
	x[2] = tmp[1];
	x[3] = tmp[2];
	return;
}
*/
/* 4-point FFT butterfly on nonadjacent elements, stepped twiddles */
/*
void __inline 
ifft4g(SPG_COMPLEX *x,int astep,int tstep)
{
	SPG_COMPLEX b,c,d;

	d.re = x[0].re - x[astep].im - x[2*astep].re + x[3*astep].im;
	d.im = x[0].im + x[astep].re - x[2*astep].im - x[3*astep].re;

	c.re = x[0].re - x[astep].re + x[2*astep].re - x[3*astep].re;
	c.im = x[0].im - x[astep].im + x[2*astep].im - x[3*astep].im;

	b.re = x[0].re + x[astep].im - x[2*astep].re - x[3*astep].im;
	b.im = x[0].im - x[astep].re - x[2*astep].im + x[3*astep].re;

	x[0].re = x[0].re + x[astep].re + x[2*astep].re + x[3*astep].re;
	x[0].im = x[0].im + x[astep].im + x[2*astep].im + x[3*astep].im;

	if(tstep == 0){
		x[2*astep] = b;
		x[astep] = c;
		x[3*astep] = d;
	} else {
		x[2*astep].re = b.re*COS(tstep) - b.im*SIN(tstep);
		x[2*astep].im = b.im*COS(tstep) + b.re*SIN(tstep);
		x[astep].re = c.re*COS(2*tstep) - c.im*SIN(2*tstep);
		x[astep].im = c.im*COS(2*tstep) + c.re*SIN(2*tstep);
		x[3*astep].re = d.re*COS(3*tstep) - d.im*SIN(3*tstep);
		x[3*astep].im = d.im*COS(3*tstep) + d.re*SIN(3*tstep);
	}
	return;
}
*/
//LES PAS INLINE
void FFTCC
fft4(SPG_COMPLEX x[4])
{
#define DFX (x)
#include "cfft4.cpp"
#undef DFX
	/*
	SPG_COMPLEX tmp[3];

	tmp[2].re = x[0].re - x[1].im - x[2].re + x[3].im;
	tmp[2].im = x[0].im + x[1].re - x[2].im - x[3].re;

	tmp[0].re = x[0].re - x[1].re + x[2].re - x[3].re;
	tmp[0].im = x[0].im - x[1].im + x[2].im - x[3].im;

	tmp[1].re = x[0].re + x[1].im - x[2].re - x[3].im;
	tmp[1].im = x[0].im - x[1].re - x[2].im + x[3].re;

	x[0].re = x[0].re + x[1].re + x[2].re + x[3].re;
	x[0].im = x[0].im + x[1].im + x[2].im + x[3].im;

	x[1] = tmp[0];
	x[2] = tmp[1];
	x[3] = tmp[2];
	return;
	*/
}

/* 4-point FFT butterfly on nonadjacent elements, stepped twiddles */
void FFTFASTCC
ifft4g(SPG_COMPLEX *x,int astep,int tstep)
{
#define DFX (x)
#define DFASTEP (astep)
#define DFTSTEP (tstep)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP
/*
	SPG_COMPLEX b,c,d;

	d.re = x[0].re - x[astep].im - x[2*astep].re + x[3*astep].im;
	d.im = x[0].im + x[astep].re - x[2*astep].im - x[3*astep].re;

	c.re = x[0].re - x[astep].re + x[2*astep].re - x[3*astep].re;
	c.im = x[0].im - x[astep].im + x[2*astep].im - x[3*astep].im;

	b.re = x[0].re + x[astep].im - x[2*astep].re - x[3*astep].im;
	b.im = x[0].im - x[astep].re - x[2*astep].im + x[3*astep].re;

	x[0].re = x[0].re + x[astep].re + x[2*astep].re + x[3*astep].re;
	x[0].im = x[0].im + x[astep].im + x[2*astep].im + x[3*astep].im;

	if(tstep == 0){
		x[2*astep] = b;
		x[astep] = c;
		x[3*astep] = d;
	} else {
		x[2*astep].re = b.re*COS(tstep) - b.im*SIN(tstep);
		x[2*astep].im = b.im*COS(tstep) + b.re*SIN(tstep);
		x[astep].re = c.re*COS(2*tstep) - c.im*SIN(2*tstep);
		x[astep].im = c.im*COS(2*tstep) + c.re*SIN(2*tstep);
		x[3*astep].re = d.re*COS(3*tstep) - d.im*SIN(3*tstep);
		x[3*astep].im = d.im*COS(3*tstep) + d.re*SIN(3*tstep);
	}
	return;
	*/
}

//#endif
void FFTCC
fft16(SPG_COMPLEX x[16])
{
//int i;
#define DFX (x)
#define DFASTEP 4
#define DFTSTEP 0
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+1)
#define DFASTEP 4
#define DFTSTEP (MAXPOINTS/16)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+2)
#define DFASTEP 4
#define DFTSTEP (2*MAXPOINTS/16)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+3)
#define DFASTEP 4
#define DFTSTEP (3*MAXPOINTS/16)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

	/*
	fft4g(x,4,0);
	fft4g(x+1,4,MAXPOINTS/16);
	fft4g(x+2,4,2*MAXPOINTS/16);
	fft4g(x+3,4,3*MAXPOINTS/16);
	*/

#define DFX (x)
#include "cfft4.cpp"
#undef DFX
#define DFX (x+4)
#include "cfft4.cpp"
#undef DFX
#define DFX (x+8)
#include "cfft4.cpp"
#undef DFX
#define DFX (x+12)
#include "cfft4.cpp"
#undef DFX

	/*
	ifft4(x);
	ifft4(x+4);
	ifft4(x+8);
	ifft4(x+12);
	*/
	return;
}


void FFTCC
fft64(SPG_COMPLEX x[64])
{
	//int i,j;

#define DFX (x)
#define DFASTEP 16
#define DFTSTEP 0
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+1)
#define DFASTEP 16
#define DFTSTEP (1*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+2)
#define DFASTEP 16
#define DFTSTEP (2*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+3)
#define DFASTEP 16
#define DFTSTEP (3*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+4)
#define DFASTEP 16
#define DFTSTEP (4*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+5)
#define DFASTEP 16
#define DFTSTEP (5*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+6)
#define DFASTEP 16
#define DFTSTEP (6*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+7)
#define DFASTEP 16
#define DFTSTEP (7*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+8)
#define DFASTEP 16
#define DFTSTEP (8*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+9)
#define DFASTEP 16
#define DFTSTEP (9*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+10)
#define DFASTEP 16
#define DFTSTEP (10*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+11)
#define DFASTEP 16
#define DFTSTEP (11*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+12)
#define DFASTEP 16
#define DFTSTEP (12*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+13)
#define DFASTEP 16
#define DFTSTEP (13*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+14)
#define DFASTEP 16
#define DFTSTEP (14*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP

#define DFX (x+15)
#define DFASTEP 16
#define DFTSTEP (15*MAXPOINTS/64)
#include "cfft4g.cpp"
#undef DFX
#undef DFASTEP
#undef DFTSTEP


/*
	fft4g(x,16,0);
	fft4g(x+1,16,MAXPOINTS/64);
	fft4g(x+2,16,2*MAXPOINTS/64);
	fft4g(x+3,16,3*MAXPOINTS/64);
	fft4g(x+4,16,4*MAXPOINTS/64);
	fft4g(x+5,16,5*MAXPOINTS/64);
	fft4g(x+6,16,6*MAXPOINTS/64);
	fft4g(x+7,16,7*MAXPOINTS/64);
	fft4g(x+8,16,8*MAXPOINTS/64);
	fft4g(x+9,16,9*MAXPOINTS/64);
	fft4g(x+10,16,10*MAXPOINTS/64);
	fft4g(x+11,16,11*MAXPOINTS/64);
	fft4g(x+12,16,12*MAXPOINTS/64);
	fft4g(x+13,16,13*MAXPOINTS/64);
	fft4g(x+14,16,14*MAXPOINTS/64);
	fft4g(x+15,16,15*MAXPOINTS/64);
*/

	fft16(x);
	fft16(x+16);
	fft16(x+32);
	fft16(x+48);
}
void FFTCC
fft256(SPG_COMPLEX x[256])
{
	int i;

	for(i=0;i<64;i+=4)
	{
		ifft4g(x+i,64,i*MAXPOINTS/256);
		ifft4g(x+i+1,64,i*MAXPOINTS/256+MAXPOINTS/256);
		ifft4g(x+i+2,64,i*MAXPOINTS/256+2*MAXPOINTS/256);
		ifft4g(x+i+3,64,i*MAXPOINTS/256+3*MAXPOINTS/256);
	}

		fft64(x);
		fft64(x+64);
		fft64(x+128);
		fft64(x+192);
}
void FFTCC
fft1024(SPG_COMPLEX x[1024])
{
	int i;

	for(i=0;i<256;i+=4)
	{
		ifft4g(x+i,256,i*MAXPOINTS/1024);
		ifft4g(x+i+1,256,i*MAXPOINTS/1024+MAXPOINTS/1024);
		ifft4g(x+i+2,256,i*MAXPOINTS/1024+2*MAXPOINTS/1024);
		ifft4g(x+i+3,256,i*MAXPOINTS/1024+3*MAXPOINTS/1024);
	}

		fft256(x);
		fft256(x+256);
		fft256(x+512);
		fft256(x+768);
}

/* FFT with arbitrary power-of-4 blocksize */
int FFTCC
fftn(SPG_COMPLEX x[],int nu)
{
	int n,i,n4,tstep;

	if(nu < 0)
		return -1;

	/* Handle the values of nu that have specific functions */
	switch(nu){
	case 0:
		return 0;
	case 1:
		fft4(x);
		return 0;
	case 2:
		fft16(x);
		return 0;
	case 3:
		fft64(x);
		return 0;
	case 4:
		fft256(x);
		return 0;
	case 5:
		fft1024(x);
		return 0;
	}
	n = 1 << (2*nu);
	if(n > MAXPOINTS)
		return -1;

	n4 = n/4;
	tstep = MAXPOINTS >> (2*nu);

	for(i=0;i<n4;i+=4)
	{
		ifft4g(x+i,n4,i*tstep);
		ifft4g(x+i+1,n4,i*tstep+tstep);
		ifft4g(x+i+2,n4,i*tstep+2*tstep);
		ifft4g(x+i+3,n4,i*tstep+3*tstep);
	}
	fftn(x,nu-1);
	fftn(x+n4,nu-1);
	fftn(x+2*n4,nu-1);
	fftn(x+3*n4,nu-1);
	return 0;
}

#endif


