/**
*** Project: Unity SW (?)
*** Organisation : Unity SC / Fogale Optique
***
*** File: VSI_StackImages_IF.h
***
*** Object : *** interface header for
***          for VSI StackImages processing
***
*** Date:			02/05/2023
*** Last update:	29/06/2023
*** Author : Bruno Luong
***
**/

#ifndef _VSI_STACKIMAGES_IF_H
#define _VSI_STACKIMAGES_IF_H

#include ".\CommonDef.h"

// We are either compile for MATLAB or for Unity_SW
#ifdef MATLAB
#undef UNITY_SW
#else
#define UNITY_SW
#endif

#ifdef UNITY_SW
// Now use static library 
#define VSI_API(TYPE)  TYPE
// // Former code when use VSInative as a DLL
// // For Unity we compile with DLL export/import decoration
// #ifdef VSISIDLL_EXPORTS
// #define VSI_API(TYPE)  __declspec(dllexport) TYPE __stdcall
// #else
// #define VSI_API(TYPE)  __declspec(dllimport) TYPE __stdcall
// #endif
#else
// MATLAB, inclusion directe :
#define VSI_API(TYPE)  TYPE
#endif

typedef Int32 ErrorFlagType;

// insertion of ERROR with other ERRORS :
#define BASE_VSI_ERRORS						0

/* List of possible error flag returned by various VSI functions */
#define VSI_OK_FLAG							0

// Negative values are critical
#define VSI_NONSUPPORTED_WINDOWTYPE			(-1 + BASE_VSI_ERRORS)
#define VSI_UNKNOWN_FREQUENCYMETHOD			(-2 + BASE_VSI_ERRORS)
#define VSI_DF_SRCWAVELENGTH_ERROR			(-3 + BASE_VSI_ERRORS)
#define VSI_UNKNOWNPIXELSET					(-4 + BASE_VSI_ERRORS)
#define VSI_DP_SRCWAVELENGTH_ERROR			(-5 + BASE_VSI_ERRORS)
#define VSI_NOT_ENOUGHIMAGES				(-6 + BASE_VSI_ERRORS)
#define VSI_NOTVALIDFILENAME				(-7 + BASE_VSI_ERRORS)
#define VSI_READFILEFAILED					(-8 + BASE_VSI_ERRORS)
#define VSI_LP_ERROR						(-9 + BASE_VSI_ERRORS)
#define VSI_LP_FAILS						(-9 + BASE_VSI_ERRORS)
#define VSI_LP_NOT_ENOUGH_PIXELS			(-10 + BASE_VSI_ERRORS)
// -11 not applicable for VSI in C
#define VSI_HP_NOT_ENOUGH_PIXELS			(-12 + BASE_VSI_ERRORS)
#define VSI_OUTOFMEMORY						(-13 + BASE_VSI_ERRORS)
#define VSI_FFT_FAILS						(-14 + BASE_VSI_ERRORS)
#define VSI_FILE_ISSUE						(-15 + BASE_VSI_ERRORS)
#define VSI_DATACONTAIN0					(-16 + BASE_VSI_ERRORS)
#define VSI_NOTYET_IMPLEMENTED				(-75 + BASE_VSI_ERRORS)
#define VSI_GENERIC_ERROR					(-100 + BASE_VSI_ERRORS)

// Warning signal
#define SATURATED_INTEROGRAM				(1 + BASE_VSI_ERRORS)

#define VSI_VERSION_LENGTH					8

typedef char								VSI_VERSION_TAG[VSI_VERSION_LENGTH];

typedef Int32								ErrorFlagType;

typedef unsigned __int8						VSI_BOOL;

#define FALSE								0
#define TRUE								1

/* Enum for method to estimate the phase */
typedef enum
{
	VSI_PHASE_LARKIN						= 0,
	VSI_PHASE_HARIHARAN						= 1,
} VSI_PhaseMethod;

/* Enum for method to estimate the amplitude */
typedef enum
{
	VSI_AMP_LARKIN							= 0,
	VSI_AMP_HARIHARAN						= 1,
} VSI_AmplitudeMethod;

/* Enum for interpolation method used for resampling fringe
   so that 5-step demodulation methods with usage of integer index is exact.
   If set to VSI_RESAMPLING_NONE resampling is disabled. */
typedef enum
{
	VSI_RESAMPLING_NONE						= 0,
	VSI_RESAMPLING_LINEAR					= 1,
	VSI_RESAMPLING_CUBIC					= 2,
} VSI_ResamplingMethod;

/* Enum for Method used by to detect spatial frequency nu. */
typedef enum
{
	VSI_FREQDETECT_FFT						= 0,
	VSI_FREQDETECT_X0						= 1,
} VSI_FrequencyMethod;

/* Enum for WindowType used for windowing
   the signal before FFT.
  Only applicable when VSI_FREQDETECT_FFT is used */
typedef enum
{
	VSI_WINDOWTYPE_GAUSSIAN					= 0,
	VSI_WINDOWTYPE_TUKEY					= 1,
} VSI_WindowType;

/* Enum for method detect the peak location */
typedef enum
{
	VSI_PEAKMETHOD_CORRELATION				= 0,
	VSI_PEAKMETHOD_BARYCENTRIC				= 1,
	VSI_PEAKMETHOD_POLYNOMIALFIT			= 2,
} VSI_PeakMethod;

/* Enum for method detect the slope correction method */
typedef enum
{
	VSI_SLOPECORRECTIONMETHOD_NONE			= 0,
	VSI_SLOPECORRECTIONMETHOD_MEDIAN		= 1,
	VSI_SLOPECORRECTIONMETHOD_POLY			= 2,
} VSI_SlopeCorrectionMethod;

/* Enum for level of verbose */
typedef enum
{
	VSI_VERBOSE_NONE						= 0,
	VSI_VERBOSE_ERROR						= 1,
	VSI_VERBOSE_WARNING						= 2,
	VSI_VERBOSE_INFO						= 3,
} VSI_Verbose_Level;

/* Define integer mask for graphic output */
#define VSI_GRAPHICALMASK_NONE		                0
#define VSI_GRAPHICALMASK_AMPLITUDE_ANIMATION		0x01
#define VSI_GRAPHICALMASK_FRINGE_ANIMATION			0x02
#define VSI_GRAPHICALMASK_REFERENCE_STRONGESTFRINGE 0x04
#define VSI_GRAPHICALMASK_TOPO_IMAGE				0x08
#define VSI_GRAPHICALMASK_HISTOGRAM					0x10
#define VSI_GRAPHICALMASK_PHASE_RESIDUAL			0x20
#define VSI_GRAPHICALMASK_APPARENT_LIGHT_SPECTRUM	0x40

/* Define integer mask to set value of OutputSelectionMask in options */
#define VSI_OUTPUTSELECTIONMASK_NONE				0
#define VSI_OUTPUTSELECTIONMASK_TOPO				0x01
#define VSI_OUTPUTSELECTIONMASK_TOPOFLATTEN			0x02
#define VSI_OUTPUTSELECTIONMASK_AMPLITUDE			0x04
#define VSI_OUTPUTSELECTIONMASK_PHASE				0x08
#define VSI_OUTPUTSELECTIONMASK_ALL					0xFF

typedef struct
{
	Int16 nx, ny;	/* Dimension of the image */
	Float32* Data;  /* 2D array, row-major order */
} SINGLE_RM_Image;

/* Pointer to an 2D array of UInt8, row - major order */
typedef UInt8*				P_ImageData; 

typedef Int32				ErrorFlagType;

/* Structure of setting argument of VSI_StackImages() function */
typedef struct
{
	double					RuleStep;				//  step between two OPDs, [m],
													// To reverse the polaeizarion of topo, set to negative value
	double					LambdaCenter;			// wavelength of the light source, [m],
	double					FWHMLambda;				// spectral bandwidth of the light source, [m]
	double					NoiseLevel;				// typical sensor noise standard deviation; [LSB]
} VSI_StackImages_Setting_Type;

/* Structure of options argument of VSI_StackImages() function */
typedef struct
{
	UInt32						OutputSelectionMask;	// VSI_StackImages() output Selection Mask
	VSI_PhaseMethod				PhaseMethod;			// DemodulationMethod enumerate Larkin, Hariharan, method
														// to estimate the phase.
	VSI_AmplitudeMethod			AmplitudeMethod;		// DemodulationMethod enumerate Larkin, Hariharan, method
														// to estimate the amplitude
	VSI_ResamplingMethod		ResamplingMethod;		// interpolation method used for resampling fringe so that 5-step
														// demodulation methods with usage of integer index is exact.
														// If set to 'none' resampling is disabled.
	VSI_FrequencyMethod			FrequencyMethod;		// Method used by DetectFrequency() to detect spatial frequency nu
	VSI_WindowType				WindowType;				// Only applicable for 'fft' method. WindowType used for windowing the signal before FFT
	double						Ewidth;					// sample unit Only applicable for 'fft' method and WindowType 'gaussian'
														// The width of the Gaussian window function
	VSI_BOOL					SpectrumSkewness;		// If TRUE adjust the light central wavelength according to the
														// skewness of the spectrum, otherwise use the MODE of the spectrum
														// as nominal spatial frequency.
	Int32						nwin;					// number of samples used to smooth interferogram
														// signal for various tasks.Make sure nwin < 0.5 * fringe period.
	VSI_PeakMethod				PeakMethod;				// PeakMethods enumerate among correlation, barycentric, polynomialfit;
														// method detect the peak location.Default correlation
	Int32						maxiter;				// maximum number of iterations of goldensearch
														// of coherence length(width of the signal envelop)
	double						LoRelwidth, HiRelwidth;	// search bracket of the bandwidth. Normalized value.
														// they are relative to the nominal coherence length computed from
														// the light source wavelength and bandwidth.
	double						PeakDetectionRelThreshold; // double scalar in (0,1), threshold used
														// to truncate fringe data for peak detection.The value is relative
														// to the max amplitude.
														// The smaller value the more data used for peak detection.
	VSI_BOOL					UseFixLambda;			// scalar boolean, if TRUE use wavelength LambdaCenter
														// from Setting; if FALSE use wavelength estimated from the phase
														// PHI in Demodulation structure.
	double						MaskThreshold;			// relative threshold used to detect invalid pixels, 
														// due to strong diffraction from topo step or dusts
														// Smaller value will refult less point masked
														// Set to 0 to disable masking
														// NOTE: small amount of pixels might still masked by NoiseLevel
	Int32						AmplitudeCmpWindow;     // size of the window use to compare amplitude for
														// masking, pixel unit. Set to 0 or 1 to disable masking
	Int32						phasewinsize;			// window size used by PhaseSmoothing,
														// set to 0 or 1 to disable
														// to 3 for mild smoothing
														// to 5 for recommended value
	VSI_SlopeCorrectionMethod	SlopeCorrectionMethod;	// method to compute the slope to be removed.
	VSI_Verbose_Level			Verbose_Level;			// level of verbose
	UInt32						GraphicalMask;			// mask for graphic output, see VSI_GRAPHICALMASK_*
} VSI_StackImages_opt_Type;


typedef struct
{
	Int16	Width, Height;							// Dimensions of *(ImageArray[i])
													// the Width is row dimension and Height is column dimension
	UInt32	NumberOfImages;							// length of ImageArray (frame-length)
	P_ImageData *ImageArray;						// 1D array of pointer of Image,
													// length NumberOfImages.
													// Each points to an 2D array of UInt8, row-major order
} VSI_StackImages_Type;


typedef struct
{
	ErrorFlagType			Flag;					// contains warning or error code (usually VSI_OK_FLAG if no error)
													// e.g., SATURATED_INTEROGRAM
	SINGLE_RM_Image			*Topo_Image;			// Image contains position of the peak, in  [m]
	SINGLE_RM_Image			*TopoFlatten_Image;		// Image contains topo with slope corrected, in  [m]
	SINGLE_RM_Image			*Amplitude_Image;		// Image contains Amplitude of the peak, [LSB]
	SINGLE_RM_Image			*Phase_Image;			// Image contains phase at the peak, [rad]
	Float64					Lambda;					// estimated the central wavelength of the light source, [m]
	Float64					FWHMLambda;				// estimated bandwidth of the source, FWHM convention, [m].
	UInt32					Reserved[8];			// Reserved for internal used
													// NOTE: Mask: values of non reliable pixels with weak amplitude in Topo_Image and
													//	TopoFlatten_Image are replaced by NaN
} VSI_Output_Type;

#ifdef __cplusplus
extern "C" {
#endif

	/* Return the version tag of the VSI module */
	VSI_API(ErrorFlagType) VSI_Get_Version(VSI_VERSION_TAG *Tag);

    // Initialization  VSI modules 
	VSI_API(ErrorFlagType) VSI_InitModule(void);

	// Close VSI modules 
	VSI_API(ErrorFlagType) VSI_CloseModule(void);

	//*******************************************************************************
	// Return a structure with default options for VSI_StackImages function
	//*******************************************************************************
	VSI_API(ErrorFlagType) VSI_DefaultOptions(VSI_StackImages_opt_Type *options);

	//*******************************************************************************
	// Perform VSI algorithm on the stack of images
	// NOTES: the fields Topo_Image, TopoFlatten_Image, Amplitude_Image, and Phase_Image
	// of VSIResult are populated depending on the value of options->OutputSelectionMask,
	// If the mask bits are not set the corresponding field points to NULL.
	//*******************************************************************************
	VSI_API(ErrorFlagType) VSI_StackImages(const VSI_StackImages_Type*Images,	// Image stack
		const VSI_StackImages_Setting_Type *Setting,							// Input, acquisition setting
		const VSI_StackImages_opt_Type *options,								// Input, processing options
		VSI_Output_Type * VSIResult);											// Output, results

    VSI_API(ErrorFlagType) VSI_RTI_StackImages(const VSI_StackImages_Type* Images,	// Image stack
        const VSI_StackImages_Setting_Type* Setting,							// Input, acquisition setting
        const VSI_StackImages_opt_Type* options,								// Input, processing options
        VSI_Output_Type* VSIResult);											// Output, results

	//*******************************************************************************
	// release memory occupied by VSIResult
	// Caller should call this for every VSI_StackImages() and once the data
	// of VSIResult no longer needed
	//*******************************************************************************
	VSI_API(ErrorFlagType) VSI_FreeResultStruct(VSI_Output_Type * VSIResult);

	// function used for debugging, not documented
	void VSI_dbg_memstate();

#ifdef __cplusplus
}
#endif

#endif
