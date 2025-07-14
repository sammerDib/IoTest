
#ifdef SPG_General_USEFFT

#define SFFTCC SPG_CONV
#define SFFT_Size(N) (1<<(N+N))

//FFTW
#undef UseFFTW
//#define UseFFTW

//Karn's FFT ASM
//#undef UseFFT4ASM
//#define UseFFT4ASM

//Karn's FFT C
//#undef UseFFT4C
#define UseFFT4C

#endif


