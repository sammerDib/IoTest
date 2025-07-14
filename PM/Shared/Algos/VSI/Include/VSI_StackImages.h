/**
*** Project: Unity SW (?)
*** Organisation : Unity SC / Fogale Optique
***
*** File: VSI_StackImages.h
***
*** Object : *** private header for
***          for VSI StackImages processing
***
*** Date:			02/05/2023
*** Last update:	27/05/2023
*** Author : Bruno Luong
***
**/

#ifndef _VSI_STACKIMAGES_H
#define _VSI_STACKIMAGES_H

#include <math.h>
#include <limits.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>

//#ifdef _WIN64
#include "fftw3_x64.h"
//#else
//#include "fftw3_x32.h"
//#endif

#define CHECK_ARG					1
#define IFFT_NORMALIZED_FLAG		0

#define NUM_THREADS 12

enum {
	ENDIAN_UNKNOWN,
	ENDIAN_BIG,
	ENDIAN_LITTLE,
	ENDIAN_BIG_WORD,							/* Middle-endian, Honeywell 316 style */
	ENDIAN_LITTLE_WORD						/* Middle-endian, PDP-11 style */
};

/* Constants to be used as input argument for DBLCONV1 */
#define FULL_SHAPE					0
#define SAME_SHAPE					1
#define VALID_SHAPE					2

/* Constant used for spline1d function argument */
#define NO_SORTED				0
#define IS_SORTED				1
#define UNKNOWN_SORTED			-1

/* PLATFORM DEPENDENT */
#define __FLOAT_WORD_ORDER ENDIAN_LITTLE

#include "VSI_StackImages_IF.h"

static VSI_VERSION_TAG The_VSI_Version = "V1.1.01";

#define DEF_OUTPUTSELECTIONMASK		VSI_OUTPUTSELECTIONMASK_TOPOFLATTEN
#define DEF_PHASEMETHOD				VSI_PHASE_HARIHARAN
#define DEF_AMPLITUDEMETHOD			VSI_AMP_LARKIN
#define DEF_RESAMPLINGMETHOD		VSI_RESAMPLING_LINEAR
#define DEF_FREQUENCYMETHOD			VSI_FREQDETECT_FFT
#define DEF_WINDOWTYPE				VSI_WINDOWTYPE_TUKEY
#define DEF_EWIDTH					1.0
#define DEF_SPECTRUMSKEWNESS		TRUE
#define DEF_NWIN					7
#define DEF_PEAKMETHOD				VSI_PEAKMETHOD_BARYCENTRIC
#define DEF_GOLDENSEARCH_MAXITER	16				
#define DEF_LORELWIDTH				0.8
#define DEF_HIRELWIDTH				1.2
#define DEF_PEAKDETECTIONTHRESHOLD	0.1
#define DEF_USEFIXLAMBDA			TRUE
#define DEF_MASKTHRESHOLD			0.6
#define DEF_AMPLITUDECMPWINDOW		13
#define DEF_PHASEWINSIZE			5
#define DEF_SLOPECORRECTIONMETHOD	VSI_SLOPECORRECTIONMETHOD_MEDIAN
#define DEF_VERBOSE_LEVEL			VSI_VERBOSE_NONE
#define DEF_GRAPHICAL_MASK			VSI_GRAPHICALMASK_NONE

typedef struct
{
	Int16 nx, ny; /* Dimension of the image */
	UInt8* Data;  /* 2D array, row-major order */
} FringeImage;

typedef FringeImage* P_FringeImage;

typedef struct
{
	Int16 nx, ny;	/* Dimension of the image */
	Float64* Data;  /* 2D array, row-major order */
} DOUBLE_RM_Image;

/* This structure is used internally in double
   before casting to single as output argument */
typedef struct
{
	ErrorFlagType			Flag;					// contains warning or error code (usually VSI_OK_FLAG if no error)
	// e.g., SATURATED_INTEROGRAM
	DOUBLE_RM_Image*		Topo_Image;				// Image contains position of the peak, in  [m]
	DOUBLE_RM_Image*		TopoFlatten_Image;		// Image contains topo with slope corrected, in  [m]
	DOUBLE_RM_Image*		Amplitude_Image;		// Image contains Amplitude of the peak, [LSB]
	DOUBLE_RM_Image*		Phase_Image;			// Image contains phase at the peak, [rad]
	Float64					Lambda;					// estimated the central wavelength of the light source, [m]
	Float64					FWHMLambda;				// estimated bandwidth of the source, FWHM convention, [m].
	UInt32					Reserved[8];			// Reserved for internal used
	// NOTE: Mask: values of non reliable pixels with weak amplitude in Topo_Image and
	//	TopoFlatten_Image are replaced by NaN
} VSI_DblOutput_Type;


typedef Int32 ArrayType;
enum
{
	VSI_REAL = 0,					// double, or f64
	VSI_COMPLEX = 1,				// double complex, real + imaginary in f64
	VSI_FLOAT32 = 2,				// float, f32
	VSI_ADC_1L = 4,					// 2*int16 interleaved IF-RO
	// as written, then format doesn't match double lise install.
	VSI_ADC_2L_FIRST = 5,			// 4*int16 interleaved IF-RO
	VSI_ADC_2L_2ND = 6,				// 4*int16 interleaved IF-RO
	VSI_INT32 = 7					// int32
};

#pragma pack(push, 1)
typedef struct
{
	double real, imag;
} ComplexDouble;
/* Interleaved ID/RO channels in ADC acquisition*/
typedef struct
{
	Int16 IF, RO;
} ADC_Pair;
typedef struct
{
	Int16 IF1, RO1;
	Int16 IF2, RO2;
} ADC_Pair_2L;
#pragma pack(pop)

typedef union
{
	double* Pr;						// use for VSI_REAL
	ComplexDouble* Pc;				// use for VSI_COMPLEX
	Float32* Pf32;					// use for VSI_FLOAT32
	ADC_Pair* PADC;					// use for VSI_ADC
	ADC_Pair_2L* PADC_2L;			// use for VSI_ADC for 2 lises
	Int32* Pi32;					// use for VSI_INT32
} NumPtrType;

typedef struct
{
	ArrayType Type;
	int n;									// length of the array
	NumPtrType data;						// length == n
} NumArrayType;

/* structure used by DetectPeak and SelectAllWindows */
#define NOFOUND_INDEX -1
typedef struct
{
	int istart, istop;
	int n;
	double* data;
} IndexIntervalType;

/* First order polynomial */
typedef struct
{
	double PL1, PL2;
} PLType;

/* Structure used by Detect peak (by Gaussian correlation method) */
typedef struct
{
	double* AllocateMemory;
	int nmax;	// max size, allocate base on this 
	int n;		// effective size, should not be larger than nmax

	const IndexIntervalType* Ai; // Input of findGaussPeak

	double LambdaCenter;	// input of findGaussPeak
	double FWHMLambda;		// input and output of findGaussPeak
	double Amax;			// output of findGaussPeak
	double dxpeak;			// output of findGaussPeak
	double iopt;			// output of goldensearch

	double* dx;
	double* A2;
	double* n1;
	double* model;
	double* model2;
	double* c;
	double* n2;
	double* corr;
	double* tmp;			// length 2*nmax
} DetectPeakWSType;

/* Structure output of GaussianCorr */
typedef struct
{
	double f, imax, Amax;
} GaussianCorrOutputType;

/* Stack of Image concatenated in 3D array */
typedef struct
{
	Int16 nx, ny, nz;	/* Dimension of the image */
	Float64* Data;		/* 3D array, row-major order */
} ImageStack_Type;

/* Demodulation result structure */
typedef struct
{
	Int16 nx, ny, np;			// new resampling step (<=1)
	Int16 nt;					// (third) dimension of xsampling, A, phi
	double samplingstep;		// sampling vector, corresponds to the
								// third (page) index of the input ImTable.
	double PeriodFrameLength;	// period of the (resampling) fringe, sample unit
	// The three array bellow are (nx x ny x np) arrays
	// np+PeriodFrameLength ~ nz
	Float64* xsampling;         // 
	Float64* phi;				// phase at the mid points
	Float64* A;					// amplitude at the mid points
	Float64* ImResampling;		// subsampled of ImageStack_Type at the mid points the mid points
	// these pointers of transposed data are handled by DetectPeak
	// and populated as NULL by Demodulation
	Float64* phi_transposed;
	Float64* A_transposed;
} Demodulation_Type;

/* Use as options by SelectAllWindows() function */
typedef struct
{
	int FilterNWin;						// length of FilterKernel
	double* FilterKernel;				// Kernel coefficients of length FilterNWin, if NULL use rectangle kernel
	double NoiseLevel;					// typical sensor noise standard deviation; [LSB]
	double MinLength;					// minimum sample length of the interval to to be
										// recorgnize as valid peak, sample unit
	double PeakDetectionRelThreshold;	// threshold used
										// to truncate fringe data for peak detection.The value is relative
										// to the max amplitude.
	double* tmp;						// temporary buffer, the length must be at least 2*data length
										// if NULL, SelectAllWindows will allocate memory
} SelectAllWindowsOptions_Type;

#ifdef __cplusplus
extern "C" {
#endif

#ifdef __cplusplus
}
#endif

#endif
