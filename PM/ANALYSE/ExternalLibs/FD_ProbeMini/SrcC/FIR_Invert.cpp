#include "SPG_General.h"

#ifdef SPG_General_USEFirInvert

#include "SPG_Includes.h"

void FFT_InvFir2D(float* Fir2D, int Size, float Epsilon)
{
	int FFTSize=1;
	int FFTN=0;
	while(FFTSize<4*Size)
	{
		FFTSize<<=2;
		FFTN++;
	}

	int Centrage=(FFTSize-Size)>>1;

	SPG_PtrAlloc(FS2D,FFTSize*FFTSize,SPG_COMPLEX,"Fir2Dsource");
	SPG_PtrAlloc(FD2D,FFTSize*FFTSize,SPG_COMPLEX,"Fir2Ddest");

	{for(int y=0;y<Size;y++)
	{
		{for(int x=0;x<Size;x++)
		{
			FS2D[(x+Centrage)+(y+Centrage)*FFTSize].re=Fir2D[x+y*Size];
		}}
		SFFT(FS2D+(y+Centrage)*FFTSize,FFTN);//FFT
		SFFTSWAP(FS2D+(y+Centrage)*FFTSize,FFTN,FFTSize);
		{for(int x=0;x<FFTSize;x++)
		{
			FD2D[(y+Centrage)+x*FFTSize]=FS2D[x+(y+Centrage)*FFTSize];//TRANSPOSE
		}}
	}}
	//BMP_WriteFloat((float*)FS2D,2*FFTSize,FFTSize,-25,25,"FS2D_A.bmp");
	//BMP_WriteFloat((float*)FS2D,2*FFTSize,FFTSize,-25,25,"FD2D_A.bmp");
	float A=Epsilon*Epsilon*FFTSize*FFTSize;
	{for(int y=0;y<FFTSize;y++)
	{
		SFFT(FD2D+y*FFTSize,FFTN);//FFT
		SFFTSWAP(FD2D+y*FFTSize,FFTN,FFTSize);
		{for(int x=0;x<FFTSize;x++)
		{
			
			float M=A+CX_Module2(FD2D[x+y*FFTSize]);//INVERSE
			FD2D[x+y*FFTSize].re/=M;
			FD2D[x+y*FFTSize].im/=(-M);
			
		}}
		SFFT(FD2D+y*FFTSize,FFTN);//FFT
		SFFTSWAP(FD2D+y*FFTSize,FFTN,FFTSize);
		{for(int x=0;x<FFTSize;x++)
		{
			FS2D[y+x*FFTSize]=FD2D[x+y*FFTSize];//TRANSPOSE
		}}
	}}
	//BMP_WriteFloat((float*)FS2D,2*FFTSize,FFTSize,-25,25,"FS2D_B.bmp");
	//BMP_WriteFloat((float*)FS2D,2*FFTSize,FFTSize,-25,25,"FD2D_B.bmp");
	/*
	for(y=0;y<Size;y++)
	{
		SFFT(FS2D+(FFTSize-(y+Centrage))*FFTSize,FFTN);//FFT
		SFFTSWAP(FS2D+(FFTSize-(y+Centrage))*FFTSize,FFTN,FFTSize);
		for(int x=0;x<Size;x++)
		{
			Fir2D[x+y*Size]=FS2D[FFTSize-(x+Centrage)+(FFTSize-(y+Centrage))*FFTSize].re/(FFTSize*FFTSize);
		}
	}
	*/
	{for(int y=0;y<Size;y++)
	{
		SFFT(FS2D+(y+Centrage)*FFTSize,FFTN);//FFT
		SFFTSWAP(FS2D+(y+Centrage)*FFTSize,FFTN,FFTSize);
		for(int x=0;x<Size;x++)
		{
			Fir2D[x+y*Size]=FS2D[x+Centrage+(y+Centrage)*FFTSize].re/(FFTSize*FFTSize);
		}
	}}

	SPG_MemFree(FS2D);
	SPG_MemFree(FD2D);
}

#endif

