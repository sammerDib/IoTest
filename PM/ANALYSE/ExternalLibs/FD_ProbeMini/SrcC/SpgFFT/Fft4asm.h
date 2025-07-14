
#ifdef UseFFT4ASM

#define FFTCC __fastcall

void FFTCC fftinit(void);
void FFTCC fftswap(SPG_COMPLEX x[],int NrTMax,int TMax);
void FFTCC fftn(SPG_COMPLEX x[],int nu);
void FFTCC fft1024(SPG_COMPLEX x[1024]);
void FFTCC fft256(SPG_COMPLEX x[256]);
void FFTCC fft64(SPG_COMPLEX x[64]);
void FFTCC fft16(SPG_COMPLEX x[16]);

void __inline ifft16(SPG_COMPLEX x[16]);
void __inline iFFT4GbigStep(SPG_COMPLEX *x,int astep,int tstep);
void __inline iFFT4G(SPG_COMPLEX *x,int astep,int tstep);

/*#define		MAXPOINTS	1048576	*/
/*#define		MAXPOINTS	262144	*/
/*#define		MAXPOINTS	65536	*/
#define			MAXPOINTS	16384	//methode 1
/*#define		MAXPOINTS	4096	*///methode 2
/*#define		MAXPOINTS	1024	*///methode 3
/*#define		MAXPOINTS	256		*/
#define		MAXPOINTSfast	1024	//methode 3
/*
#undef PReverse
#define PReverse(i) ((((i)<<(LMAXPOINTS-PARTIALREVERSEBITSORDER))|((i)>>(PARTIALREVERSEBITSORDER)))&(MAXPOINTS-1))
#undef PReverse
//#define PReverse(i) (i)
*/
#undef AllowBigStep
#define AllowBigStep 1
/*
push ebx
push ecx
mov ebx,eax
mov ecx,eax
shl ebx,PARTIALREVERSEBITSORDER
shr ecx,LMAXPOINTS-1-PARTIALREVERSEBITSORDER
and ebx,(MAXPOINTS-1)
mov eax,ecx
or eax,ebx
pop ecx
pop ebx
*/

#endif

