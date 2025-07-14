
#include "SPG_General.h"

#include "WhatFFT.h"

#if(defined(UseFFTW)||defined(UseFFTM3D))

#include "SPG_Includes.h"

#pragma SPGMSG(__FILE__,__LINE__,"using FFTW")

#include <memory.h>

#include "ComplexType.h"

#include "fftwcompat.h"

     fftw_plan p;
	 int planvalue=0;
	 fftw_complex* fftw_inplace_out=0;
	 /*
	 fftw_complex in[1024];
	 fftw_complex out[1024];
	 */

void SPG_CONV fftwinit(void)
{
	if (planvalue!=0) fftw_destroy_plan(p);  
    p = fftw_create_plan(1024, FFTW_FORWARD, FFTW_ESTIMATE);
	planvalue=1024;
//	fftw_inplace_out=0;
	if(fftw_inplace_out!=0) SPG_MemFree(fftw_inplace_out);
	fftw_inplace_out=(fftw_complex*)SPG_MemAlloc(planvalue*sizeof(fftw_complex),"FFTW OUT");
}

void SPG_CONV fftwclose(void)
{
	if (planvalue!=0)
	{
		fftw_destroy_plan(p);  
		planvalue=0;
	}
	if (fftw_inplace_out) SPG_MemFree(fftw_inplace_out);
	fftw_inplace_out=0;
}

int SPG_CONV fftwn(SPG_COMPLEX x[],int nu)
{
	int N=SFFT_Size(nu);

	if (N!=planvalue)
	{
	if (fftw_inplace_out) SPG_MemFree(fftw_inplace_out);
    fftw_destroy_plan(p);  
    p = fftw_create_plan(N, FFTW_FORWARD, FFTW_ESTIMATE);
	planvalue=N;
	fftw_inplace_out=(fftw_complex*)SPG_MemAlloc(N*sizeof(fftw_complex),"FFTW OUT");
	}
    fftw_one(p, (fftw_complex*)x, fftw_inplace_out);
	memcpy(x,fftw_inplace_out,N*sizeof(fftw_complex));
	return -1;
}


int SPG_CONV fftw_GENERAL(SPG_COMPLEX x[],int N)
{
	if (N!=planvalue)
	{
	if (fftw_inplace_out) SPG_MemFree(fftw_inplace_out);
    fftw_destroy_plan(p);  
    p = fftw_create_plan(N, FFTW_FORWARD, FFTW_ESTIMATE);
	planvalue=N;
	fftw_inplace_out=(fftw_complex*)SPG_MemAlloc(N*sizeof(fftw_complex),"FFTW OUT");
	}
	/*
	fftw_complex* in=(fftw_complex*)SPG_MemAlloc(N*sizeof(fftw_complex),"FFTW IN");
	memcpy(in,x,N*sizeof(fftw_complex));
	*/
    fftw_one(p, (fftw_complex*)x, fftw_inplace_out);
	//SPG_MemFree(in);
	//memcpy(out,in,N*sizeof(fftw_complex));
	memcpy(x,fftw_inplace_out,N*sizeof(fftw_complex));
	return -1;
}

int SPG_CONV fftw_GENERAL_IO(SPG_COMPLEX in[],SPG_COMPLEX out[],int FFTSize)
{
	if (FFTSize!=planvalue)
	{
	if (fftw_inplace_out) 
	{
		SPG_MemFree(fftw_inplace_out);
		fftw_inplace_out=0;
	}
    fftw_destroy_plan(p);  
    p = fftw_create_plan(FFTSize, FFTW_FORWARD, FFTW_ESTIMATE);
	planvalue=FFTSize;
	}
    fftw_one(p, (fftw_complex*)in, (fftw_complex*)out);
	return -1;
}

/*
...
{
     fftw_complex in[N], out[N];
     fftw_plan p;
     ...
     p = fftw_create_plan(N, FFTW_FORWARD, FFTW_ESTIMATE);
     ...
     fftw_one(p, in, out);
     ...
     fftw_destroy_plan(p);  
}
*/
#endif

