
#include "SPG_General.h"

#ifdef SPG_General_USEFFT

/*
Interface vers les ffts
*/


//#include "WhatFFT.h"
#include "SPG_Includes.h"

//#include "FFT.h"

#ifdef UseFFT4ASM
	#include "fft4asm.h"
#pragma message("FFT.cpp UseFFT4ASM")
#endif
#ifdef UseFFT4C
	#include "fft4c.h"
#pragma message("FFT.cpp UseFFT4C")
#endif
#ifdef UseFFTW
	#include "fftwcompat.h"
#pragma message("FFT.cpp UseFFTW")
#endif

#include <math.h>

float DPI,PI;//,MPI;

void SFFTCC INITFFT(void)
{
//#error CompileFFT
	__asm
	{
		fldpi
		fst float ptr PI
		fld st
		fadd
		fstp float ptr DPI
	}

#ifdef UseFFT4ASM
	fftinit();
#endif
#ifdef UseFFT4C
	fftinit();
#endif
#ifdef UseFFTW
	fftwinit();
#endif

	CD_G_CHECK_EXIT(6,31);
	return;
}

void SFFTCC CLOSEFFT(void)
{
#ifdef UseFFT4ASM
#endif
#ifdef UseFFT4C
#endif
#ifdef UseFFTW
	fftwclose();
#endif

	return;
}

void SFFTCC SFFT(SPG_COMPLEX * D,int n)//n=puissance de 4
{
CHECK(D==0,"SFFT: Donnees invalides",return);
//	SetCoproState
#ifdef UseFFT4ASM
	fftn(D,n);
#endif
#ifdef UseFFT4C
	fftn(D,n);
#endif
#ifdef UseFFTW
	fftwn(D,n);
#endif

	CD_G_CHECK_EXIT(11,14);
	return;
}


#ifdef UseFFTW
void SFFTCC SFFT_GENERAL(SPG_COMPLEX * D,int n)//n=nombre de points
{
CHECK(D==0,"SFFT: Donnees invalides",return);
	fftw_GENERAL(D,n);
	return;
}
#endif

#ifdef UseFFTW
void SFFTCC SFFT_GENERAL_IO(SPG_COMPLEX * Din,SPG_COMPLEX * Dout,int n)
{
CHECK(Din==0,"SFFT: Donnees source invalides",return);
CHECK(Dout==0,"SFFT: Donnees destination invalides",return);
	fftw_GENERAL_IO(Din,Dout,n);
	CD_G_CHECK_EXIT(18,22);
	return;
}
#endif

#ifndef UseFFTW
void SFFTCC SFFTSWAP(SPG_COMPLEX * D,int NrTMax,int TMax)
{
CHECK(D==0,"SFFT: Donnees invalides",return);
#ifdef UseFFT4ASM
	fftswap(D,NrTMax,TMax);
#endif
#ifdef UseFFT4C
	fftswap(D,TMax);
#endif
	return;
}
#endif

int SFFTCC SFFT_GetAppropriateSize(int Size,int Flag)
{
#ifdef UseFFTW
	switch(Flag)
	{
	case FFT_MARGE_100:
		return SFFT_GetAppropriateSize(2*Size,FFT_UPPER);
	case FFT_MARGE_33:
		return SFFT_GetAppropriateSize(V_Round(1.33*Size),FFT_UPPER);
	case FFT_LOWER:
		{
		int Prefer=Size;
		int MaxPrimeFactor=Size;
		for(int i=2*Size/3+1;i<=Size;i++)
		{
			int BF=SFFT_GetBiggestPrimeFactor(i);
			if (BF<=MaxPrimeFactor)
			{
				MaxPrimeFactor=BF;
				Prefer=i;
			}
		}
		return Prefer;
		}
		break;
	default://FFT_UPPER
		{
		int Prefer=Size;
		int MaxPrimeFactor=Size;
		for(int i=Size;i<3*Size/2;i++)
		{
			int BF=SFFT_GetBiggestPrimeFactor(i);
			if (BF<MaxPrimeFactor)
			{
				MaxPrimeFactor=BF;
				Prefer=i;
			}
		}
		return Prefer;
		}
		break;
	}
#else
	switch(Flag)
	{
	case FFT_LOWER:
		{
		for(int Prefer=16;Prefer<Size;Prefer<<=2);
		return Prefer>>2;
		}
		break;
	default://FFT_UPPER
		{
		for(int Prefer=16;Prefer<Size;Prefer<<=2);
		return Prefer;
		}
		break;
	}
#endif
}

int SFFTCC SFFT_GetBiggestPrimeFactor(int n)
{
	int F=2;
	while(F*F<=n)
	{
		if((n%F)==0)
			n/=F;
		else
			F++;
	}
	return n;
}

/*
void FFTWNIO(SPG_COMPLEX in[],SPG_COMPLEX out[],int FFTSize)
{
	fftwnIO(SPG_COMPLEX in[],SPG_COMPLEX out[],int FFTSize);
}
*/

#endif


