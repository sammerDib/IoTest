
#ifdef SPG_General_USEFFT

#include "WhatFFT.h"
#include "ComplexType.h"

void SFFTCC INITFFT(void);
void SFFTCC CLOSEFFT(void);
void SFFTCC SFFT(SPG_COMPLEX * D,int n);//size=4^n
void SFFTCC SFFT_GENERAL(SPG_COMPLEX * D,int n);//size=n
void SFFTCC SFFT_GENERAL_IO(SPG_COMPLEX * Din,SPG_COMPLEX * Dout,int n);//size=n

#ifdef UseFFTW
#define SFFTSWAP(D,i,n)
#else
void SFFTCC SFFTSWAP(SPG_COMPLEX * D,int n);
void SFFTCC SFFTSWAP(SPG_COMPLEX * D,int NrTMax,int TMax);
#endif

int SFFTCC SFFT_GetAppropriateSize(int Size,int Flag);
int SFFTCC SFFT_GetBiggestPrimeFactor(int n);

#define FFT_LOWER 0
#define FFT_UPPER 1
#define FFT_MARGE_33 2
#define FFT_MARGE_100 3

#ifndef PI_ALREADY_DEFINED_AS_DOUBLE
//extern float PI,DPI;
#endif

#endif

