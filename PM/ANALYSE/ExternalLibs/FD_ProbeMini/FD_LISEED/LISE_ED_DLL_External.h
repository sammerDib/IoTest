/*
 * $Id: LISE_ED_DLL_External.h 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

//////////////////////////////////////////////////////////////////////
//																	//
//							Fogale Nanotech							//
//					       LISE ED DLL header						//
//																	//
//////////////////////////////////////////////////////////////////////


#ifdef INC_LISE_ED_DLL_INC
#error LISE_ED_DLL.h Included twice
#endif
#define	INC_LISE_ED_DLL_INC

// DLL_EXPORTS is defined only in the project which compiles the dll.
// In this case, the functions are declared dllexports. (add in C/C++ preprocessor definitions)

#ifdef LISEEDDRIVER_EXPORTS
#define DLLEXP __declspec(dllexport)
#else
//FDE #define DLLEXP __declspec(dllimport)
#define DLLEXP
#endif

#define LISE_ED_DLL_API __stdcall

// Return values (all functions)
#define DLL_OK 1
#define DLL_FAIL 0

#ifdef __cplusplus
#pragma message (__FILE__"(34): using extern C")
extern "C"
{
#endif

// DLL functions

// ####################################### Main ########################################

// Returns LISE_ED_DLL_VERSION (to check correspondance between header and DLL)
DLLEXP int LISE_ED_DLL_API LiseEDGetVersion();

// Returns Device Information
DLLEXP int LISE_ED_DLL_API LiseEDGetSystemCaps(char* Type,char* SerialNumber,double* Range,int* Frequency,double* GainMin, double* GainMax, double* GainStep);

// DLL initialisation
DLLEXP int LISE_ED_DLL_API LiseEDInit(char* configPath);

// Begin continuous acquisition and process
DLLEXP int LISE_ED_DLL_API LiseEDAcqStart();

// Stop continuous acquisition and process
DLLEXP int LISE_ED_DLL_API LiseEDAcqStop();

// Close DLL
DLLEXP int LISE_ED_DLL_API LiseEDClose();

// Set source power
DLLEXP int LISE_ED_DLL_API LiseEDSetSourcePower(double* Volts, int* Voie);

// Display Setting Window
DLLEXP int LISE_ED_DLL_API LiseEDDisplaySettingWindow(int ShowGainSetting);

// Destroy Setting Window
DLLEXP int LISE_ED_DLL_API LiseEDDestroySettingWindow();

// Set Stage Position Info
DLLEXP int LISE_ED_DLL_API LiseEDSetStagePositionInfo(double* XSystemPosition, double* YSystemPosition, double *ZSystemPosition);

//######################################## Results #######################################

// Get the number of samples of the last waveform period
DLLEXP int LISE_ED_DLL_API LiseEDGetNbSamplesWaveform(int* NbSamples);

// Get the last waveform period
DLLEXP int LISE_ED_DLL_API LiseEDGetWaveform(double* I,int* NbSamples,float* StepX,int Voie);

// Get the number of peaks of the last period
DLLEXP int LISE_ED_DLL_API LiseEDGetNbPeaksPeriod(int* NbPeaks,int* Voie);

// Get the peaks of the last period
DLLEXP int LISE_ED_DLL_API LiseEDGetPeaksPeriod(double* XPosRel, double* Intensite, double* Quality, int* Sens, int* Flag, int* NbPeaks,int* Voie);

// Read temperature
DLLEXP int LISE_ED_DLL_API LiseEDReadTemperature(double* Temperature);

// Read optical power received
DLLEXP int LISE_ED_DLL_API LiseEDReadPower(double* Power);

// Get thickness
DLLEXP int LISE_ED_DLL_API LiseEDGetThickness(double* Thickness, int* NbThickness, double *Quality, int _iNbThickness);

// Define sample
DLLEXP int LISE_ED_DLL_API LiseEDDefineSample(char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int *NbThickness);

//####################################### Sauvegardes ######################################

// Save the last period wafeform in a text file
DLLEXP int LISE_ED_DLL_API LiseEDSaveWaveForm(char* FileName,float* StepX);

// Save peaks in a text file
DLLEXP int LISE_ED_DLL_API LiseEDSavePeaks(char* FileName);

// Save thickness in a text file
DLLEXP int LISE_ED_DLL_API LiseEDAcqSaveThickness(char* FileName);

#ifdef __cplusplus
}
#endif