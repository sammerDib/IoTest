/*
 * $Id: LISE_ED_DLL_Internal.h 8298 2009-02-18 11:14:07Z S-PETITGRAND $
 */

#ifndef DBL_LISEED_DLL_INTERNAL_H
#define DBL_LISEED_DLL_INTERNAL_H

#include "../LiseHardwareConfiguration.h"
// Header version
#define DBL_LISE_ED_DLL_VERSION 1

//#include "../FD_LISEED/LISE_ED_DLL_Internal.h"

// DLL_OK et DLL_FAIL sont les valeurs de retour des fonctions DLL appelés par l'utilisateur. (toutes fonctions de Internal.cpp)
#define DLL_OK 1
#define DLL_FAIL 0

#define MAXSTR	1024

// Nom du module pour le fichier de log (si le log n'est pas défini en dehors du module)
#define DBL_LISEED_MODULE_NAME "DBL_LISE_ED_Driver"

// Status de la probe
#define PROBE_NON_INIT	0
#define	PROBE_INIT		1

typedef enum
{
	NoProbeDefine,
	BothProbe,
	UpProbe,
	DownProbe

} CONFIG_MEASURE;

typedef enum
{
	DblEdStopped,
	DblEdStarted

} DBL_PROBE_STATE;

// Structure LISE_ED interne au moduke
typedef struct
{
	int iStatusProbe;	//< Status de la sonde
	int iCurrentProbe;	//< dernière probe utilisée
	int iVisibleProbe;	//< index de la probe que l'on souhaite voir dans un getraw signal
	
	char strCfgFilePath[MAXSTR];	//< fichier de config de la sonde

	char strTypeDevice[MAXSTR];	//< Type du device

	char strCfgTileFirstProbe[MAXSTR];	//< fichier de config de la première sonde
	char strCfgTileSecondProbe[MAXSTR];	// fichier de config de la deuxième sonde
	char strLogCalibrationFile[MAXSTR];	// fichier de log pour toutes les valeurs de calibration

	FILE* FileLogCalib;	// fichier contenant les valeurs de cale étalon
	LISE_ED LiseEd[2];	// deux structure LiseEd classique
	double GainValue[2];	// valeur de gain pour chaque probe
	double CurrentValue;

	LOGFILE* Log;	//< fichier de log de la structure
	bool binternalLog;	//< booléen pour savoir si un fichier de cfg interne est défini

	bool bCalibrationCancel;		//< booléen pour savoir si on veut annuler la calibration

	double dThicknessUnkownLayer;	//< epaisseur de la couche inconnue
	double dToleranceUnknownLayer;	//< tolerance de la couche inconnue
	double dTotalThickness;		//< épaisseur totale
	double XCalibrationValue;	//< valeur de la calibration en X
	double YCalibrationValue;	//< valeur de la calibration en Y
	double ZCalibrationValue;	//< valeur de la calibration en Z
	double dCalibrateLowerAirgap;// Airgap de dessous mesuré
	double dCalibrateUpperAirgap;// Airgap de dessus mesuré
	double dCalibrateThicknessUsed;// Epaisseur mesurée durant calibration

	int iThicknessDefineProbe1;
	int iThicknessDefineProbe2;
	CONFIG_MEASURE CfgMeas;		//< config de l'état de la mesure pour savoir quelle probe on utilise

	int iWaitAfterSwitch;		//< temps d'attente après une commutation du switch

	bool bSuspendDuringDF;		//< booleen pour suspendre les GTh et GRSignal pendant le define sample
	//HANDLE ProbeMutex; //bool bProbeRessourceReserved;// Booleen pour reserver l utilisation de la probe: pas de getThickness durant une calibration ou un autoGain
	int iCalibrationMode;		//< mode pour effectuer la double calibration
	int iNbCalibrationRepeat;	//< nombre de repetitions pour effectuer une calibration

	DBL_PROBE_STATE ProbeState;		// etat de la probe double started ou stopped
	bool bProbeInCalibration;		// probe en calibration
	
	bool bGrabbedByThread;			// booleen de synchronisation entre deux threads

} DBL_LISE_ED;

// ####################################### Main ########################################

// Returns LISE_ED_DLL_VERSION (to check correspondance between header and DLL)
int DBL_LEDIGetVersion();

// set parameters internal to Lise Ed
int DBL_LEDISetParam(void* s,void* Param, int ParamID);

// get parameters internal to Lise Ed
int DBL_LEDIGetParam(void* s,void* Param, int ParamID);

// Return System Information
int DBL_LEDIGetSystemCaps(void* s,char* Type,char* SerialNumber,double& Range,int& Frequency,double& GainMin, double& GainMax, double& GainStep);

// DLL initialisation
int DBL_LEDIInit(void* s, DBL_LISE_HCONFIG* HardwareConfigDual, LISE_HCONFIG* HardwareConfigTop, LISE_HCONFIG* HardwareConfigBottom, char* configPath, LOGFILE* Log, double Param1, double Param2, double Param3);

// Begin continuous acquisition and process
int DBL_LEDIStartContinuousAcq(void* s);

// Begin Single Shot Acquisition acquisition and process
int DBL_LEDIStartSingleShotAcq(void* s);

// Begin acquisition without notion of continous or Single Shot (default mode Single Shot)
int DBL_LEDIAcqStart(void* s);

// Stop continuous acquisition and process
int DBL_LEDIAcqStop(void* s);

// Close DLL
int DBL_LEDIClose(void* s);

// Save the configuration
int DBL_LEDISaveConfig(void* s,char* ConfigFile1,char* ConfigFile2,char* Password);

// Set source power
int DBL_LEDISetSourcePower(void* s,double Volts);

// Set AutoGain double
int SetParamAutoGain_Double(void* s, bool bAutoGain);

//######################################## Results #######################################

// Get the number of samples of the last waveform period
int DBL_LEDIGetNbSamplesWaveform(void* s,int* NbSamples);

// Get Waveform Securised
int DBL_LEDIGetRawSignal(char* Password, void* s,double* I,int* NbSamples,float* StepX,int Voie,float* SaturationValue, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks);

// Get the last waveform period
int DBL_LEDIGetWaveform(void* s,double* I,int* NbSamples,float* StepX, int Voie, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks);

// Get the number of peaks of the last period
int DBL_LEDIGetNbPeaksPeriod(void* s,int* NbPeaks,int* Voie);

// Get the peaks of the last period
int DBL_LEDIGetPeaksPeriod(void* s,double* XPosRel, double* Intensite, double* Quality, int* Sens, int* Flag, int* NbPeaks,int* Voie);

// Read temperature
int DBL_LEDIReadTemperature(void* s,double* Temperature);

// Read optical power received
int DBL_LEDIReadPower(void* s,double* Power);

// Get thickness
int DBL_LEDIGetThickness(void* s,double* Thickness, double* Quality,int _iNumThickness);

// Get thickness
int DBL_LEDIGetThicknesses(void* s, double* Dates, double* Thicknesses, double* Quality,int NumValues);

// Define sample
int DBL_LEDIDefineSample(void* s,char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int NbThickness, double Gain, double QualityThreshold);

// Set Stage Position Info
int DBL_LEDISetStagePositionInfo(void* s,double* XSystemPosition, double* YSystemPosition, double *ZSystemPosition);

//####################################### Sauvegardes ######################################

// Save the last period wafeform in a text file
int DBL_LEDISaveWaveForm(void* s,char* FileName,float* StepX);

// Save peaks in a text file
int DBL_LEDISavePeaks(void* s,char* FileName);

// Save thickness in a text file
int DBL_LEDIAcqSaveThickness(void* s,char* FileName);

int DBL_LEDICalibrateDark(void* s);

int DBL_LEDICalibrateThickness(void* s, float Value);

int DBL_LEDIRestartMeasurement(void* s);

//####################################### User interface ######################################

int DBL_LEDIOpenSettingsWindow(void* s,int ShowGainSetting);

int DBL_LEDIUpdateSettingsWindow(void* s);

int DBL_LEDICloseSettingsWindow(void* s);

//####################################### Double Specific functions ######################################

int DBL_LEDIGetSystemCapsDouble(void* s,char* Type,char* SerialNumber,double* Range,int* Frequency,double* GainMin, double* GainMax, double* GainStep);

int DBL_LEDIDefineSampleDouble(void* s,char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int NbThickness, double* Gain, double* QualityThreshold);

//#include "LISE_ED_DLL_UI_Fct.h"

#endif

