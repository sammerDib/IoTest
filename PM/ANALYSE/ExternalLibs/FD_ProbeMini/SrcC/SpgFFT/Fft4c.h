
#ifdef UseFFT4C
/*
typedef struct {
	float re;
	float im;
} SPG_COMPLEX;
*/
#define FFTCC __fastcall
#define FFTFASTCC __fastcall

void FFTCC fftinit(void);
void FFTCC fftswap(SPG_COMPLEX x[],int n);

void FFTCC fft2(SPG_COMPLEX x[2]);
void FFTCC fft4(SPG_COMPLEX x[4]);
void FFTFASTCC fft4g(SPG_COMPLEX *x,int astep,int tstep);

void FFTCC fft16(SPG_COMPLEX x[16]);
void FFTCC fft64(SPG_COMPLEX x[64]);
void FFTCC fft256(SPG_COMPLEX x[256]);
void FFTCC fft1024(SPG_COMPLEX x[1024]);
int FFTCC fftn(SPG_COMPLEX x[],int nu);

/*#define	MAXPOINTS	1048576 */
/*#define		MAXPOINTS	262144 */
/*#define		MAXPOINTS	65536 */
//#define		MAXPOINTS	16384
/*#define		MAXPOINTS	4096 */
#define		MAXPOINTS	1024
/*#define	MAXPOINTS	256*/


#endif

