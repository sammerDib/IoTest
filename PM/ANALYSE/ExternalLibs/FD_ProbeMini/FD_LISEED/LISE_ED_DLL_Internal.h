/*
 * $Id: LISE_ED_DLL_Internal.h 8298 2009-02-18 11:14:07Z S-PETITGRAND $
 */

#ifndef LISE_ED_DLL_INTERNAL_H
#define LISE_ED_DLL_INTERNAL_H

// Header version
#define LISE_ED_DLL_VERSION 4

#ifdef _DEBUG
#include <time.h>
#undef _WATCHTIME_
#else
#undef _WATCHTIME_
#endif
#include "../LiseHardwareConfiguration.h"

// DLL_OK et DLL_FAIL sont les valeurs de retour des fonctions DLL appelés par l'utilisateur. (toutes fonctions de Internal.cpp)
#define DLL_OK 1
#define DLL_FAIL 0

#define MAX_THICKNESS	(FP_LISECONSTS_MAXPEAKS-1)
#define MAX_PEAKS	FP_LISECONSTS_MAXPEAKS

#define LEDI_SLAVE_ED	1	// constante permettant de definir un Ed Slave dans le cas d'un double
//#define LEDI_TIMEOUT_GETTH	370
#define LEDI_TIMEOUT_GETTH	600
#define AUTO_GAIN_VALUE -2
// Paramétrage au niveau du préprocesseur
// Si on est pas en mode émulé
//efine DEVICECONNECTED //ce define est déplacé dans NIDAQmxConfig.h
// Si on utilise une référence optique
#define REFOPTIQUE
// bouttons pour la fenetre de parametrage
//#define SETTING_WINDOWS_SCROLLBAR

// Nom du module pour le fichier de log (si le log n'est pas défini en dehors du module)
#define LISEED_MODULE_NAME "LISELSLIDriver"

//nclude "..\LISE\LISE_Struct_Process.h"
//nclude "..\SrcC\SPG.h"

#ifndef FogaleProbeConfig_h
#error "Please enable FogaleProbe in SrcC\Config\SPG_General.h"
#endif

//nclude "LISE_ED_DLL_UI_Struct.h"
//nclude "../LISE/LISE_Struct.h"

#define RBP_Inc(RBP) {RBP.AbsN++;if((++RBP.N)>RBP.Len-1){RBP.N-=RBP.Len;}}
#define RBP_Dec(RBP) {RBP.AbsN--;if((--RBP.N)<0){RBP.N+=RBP.Len;}}
#define RBP_Add(RBP,a) {DbgCHECK((a<0)||(a>RBP.Len),"RBP_Add");RBP.AbsN+= a; RBP.N +=a; if(RBP.N>=RBP.Len){RBP.N -= RBP.Len;}}
#define RBP_Sub(RBP,a) {DbgCHECK((a<0)||(a>RBP.Len),"RBP_Sub");RBP.AbsN-=a;RBP.N-=a;if(RBP.N<0){RBP.N+=RBP.Len;}}
#define RBP_GetNMinusOne(RBP) RBP.N==0?RBP.Len-1:RBP.N-1
#define RBP_GetNPlusOne(RBP) RBP.N==RBP.Len-1?0:RBP.N+1
#define RBP_GetNMinusA(RBP,A)	(RBP.N-A<0)?(RBP.N-A+RBP.Len):(RBP.N-A)

#define INC_Index_Measure(a) {a++;}
#define RAZ_Index_Measure(a) {a=0;}

typedef enum
{
	Measurement,
	RecoupPower,
	Stopped

} MODE;

typedef enum
{
	None,
	Master,
	Slave

} DBL_ED_CFG;

typedef enum
{
	MeasSingleProbe,
	MeasBothProbe

} DBL_ED_MEAS_MODE;

#ifdef _WATCHTIME_

#define MAX_WATCH_POINT	128

typedef struct
{
	bool bInitialized;
	FILE* FileWatch;
	char Name[LONG_STR];
	char FileName[LONG_STR];
	clock_t LastTimeWatch;

	bool bWatchPointHeaderPrint;
	char WatchPointName[MAX_WATCH_POINT][MAXSTRING];
	int iNumWatchPoint;
	__int64 freq;
	__int64 i64startTimeWatch;

	double dTotalTimeLoop;	// temps total de boucle

} WATCH_TIME_STRUCT;
#endif

// Structure LISE_ED interne au moduke
typedef struct
{
// detection du sens

	bool bLiseEDConnected;	// permet de définir si le LISE ED est en mode connecté ou non

	float fNiveauHaut;	// Valeur du niveau haut du pulse dans Pulse Detection
	float fNiveauBas;	// Valeur du niveau bas du pulse dans Pulse Detection
	float fTolerance;	// Valeur de la tolérance sur la mesure du Pulse

	RING_BUFFER_POS PulseMoinsLeft;	// Indice sur buffer circulaire pour le Pulse Moins Left
	RING_BUFFER_POS PulseMoinsRight;// Indice sur buffer circulaire pour le Pulse Moins Right

	int PulsePlusWidthLastPeriod;	// largeur en échantillon du pulse

	int LtPlus;		// Nombre d'échantillons de la demi période (sens positif)
	int LtMoins;	// Nombre d'échantillons de la demi période (sens négatif)

	int NbrPics;	// Compteur pour la détection de pics sur la voie 1 (sens moins)
	int iNbrPicsVoie2;// Compteur pour la détection de pics sur la voie 2 (sens moins)

// Variables de détection de pulse	
	bool bFirstPulseSample;
	bool bPulseDetection;
	bool bPulse;
    int cptSensDetect;	// parametre pour la détection d'un pulse
	RING_BUFFER_POS PossiblePulsePlusLeft;
	RING_BUFFER_POS PossiblePulseMoinsLeft;

	int iRefOpt;	// Définition d'une référence optique
	bool bTheoOptRef;	// Utilisation de la position théorique du pic de rédférence

	int IndiceCopieSignalEmule;	// parametre pour la memorisation de la position du signal émulé

	int iPicSatureDansSimulation;	// Nombre de pics saturés dans le signal émulé que l'on ne lit pas

	LISE_ED_SETTINGS LSettings;

	float fPositionRefOpt;
	float fToleranceRefOpt;

	LISE Lise;		//<On inclue un LISE dans la structure LISE ED

	// Utilisation de l'autogain
	bool bUseAutoGain;
	// Valeur de sauvegarde du gain avant passage en veille du lise
	float BackUpSourceValue;

	int iMeasurementWaitTime;	//< Temps d'établissement pour attendre

	MODE AcquisitionMode;	//< mode d'acquisition pour savoir si on est en puissance recouplée ou en 
	DBL_ED_CFG DblEdMode;	//< Master, Slave, None
	DBL_ED_MEAS_MODE	DblEdMeasurementMode; //< Single ou Both probe

	int ThisProbe;
	int* CurrentProbeFromParentStruct;	//< pointeur sur la probe courante d'un double ED

	bool bEnableStandBy;

	// ajout d'une structure wztchtime pour surveiller les differents elements
#ifdef _WATCHTIME_
	WATCH_TIME_STRUCT WatchThreadLoop;
	WATCH_TIME_STRUCT WatchThreadUse;
#endif

	char ConfigPath[1024];

} LISE_ED;

// ####################################### Main ########################################

// Returns LISE_ED_DLL_VERSION (to check correspondance between header and DLL)
int LEDIGetVersion();

// set parameters internal to Lise Ed
int LEDISetParam(void* s,void* Param, int ParamID);

// get parameters internal to Lise Ed
int LEDIGetParam(void* s,void* Param, int ParamID);

// Return System Information
int LEDIGetSystemCaps(void* s,char* Type,char* SerialNumber,double& Range,int& Frequency,double& GainMin, double& GainMax, double& GainStep);

// DLL initialisation
int LEDIInit(void* s, char* configPath, LISE_HCONFIG* liseHConfig, LOGFILE* Log, double Param1, double Param2, double Param3);

// Begin continuous acquisition and process
int LEDIStartContinuousAcq(void* s);

// Begin Single Shot Acquisition acquisition and process
int LEDIStartSingleShotAcq(void* s);

// Begin acquisition without notion of continous or Single Shot (default mode Single Shot)
int LEDIAcqStart(void* s);

// Stop continuous acquisition and process
int LEDIAcqStop(void* s);

// Close DLL
int LEDIClose(void* s);

// Save the configuration
int LEDISaveConfig(void* s,char* ConfigFile,char* Password);

// Set source power
int LEDISetSourcePower(void* s,double Volts);

//######################################## Results #######################################

// Get the number of samples of the last waveform period
int LEDIGetNbSamplesWaveform(void* s,int* NbSamples);

// Get Waveform Securised
int LEDIGetRawSignal(char* Password, void* s,double* I,int* NbSamples,float* StepX,int Voie,float* SaturationValue, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks);

// Get the last waveform period
int LEDIGetWaveform(void* s,double* I,int* NbSamples,float* StepX, int Voie, double* SelectedPeaks, int* nbSelectedPeaks, double* DiscardedPeaks, int* nbDiscardedPeaks);

// Get the number of peaks of the last period
int LEDIGetNbPeaksPeriod(void* s,int* NbPeaks,int* Voie);

// Get the peaks of the last period
int LEDIGetPeaksPeriod(void* s,double* XPosRel, double* Intensite, double* Quality, int* Sens, int* Flag, int* NbPeaks,int* Voie);

// Read temperature
int LEDIReadTemperature(void* s,double* Temperature);

// Read optical power received
int LEDIReadPower(void* s,double* Power);

// Get thickness
int LEDIGetThickness(void* s,double* Thickness, double* Quality, int _iNbThickness);

// Get thicknesses
int LEDIGetThicknesses(void* s,double* Dates, double* Thicknesses, double* Quality,int NumValues);

// Define sample
int LEDIDefineSample(void* s,char *Name, char *SampleNumber, double *Thickness, double *Tolerance, double *Index, double *Intensity, int NbThickness, double Gain, double QualityThreshold);

// Set Stage Position Info
int LEDISetStagePositionInfo(void* s,double* XSystemPosition, double* YSystemPosition, double *ZSystemPosition);

//####################################### Sauvegardes ######################################

// Save the last period wafeform in a text file
int LEDISaveWaveForm(void* s,char* FileName,float* StepX);

// Save peaks in a text file
int LEDISavePeaks(void* s,char* FileName);

// Save thickness in a text file
int LEDIAcqSaveThickness(void* s,char* FileName);

int LEDICalibrateDark(void* s);

int LEDICalibrateThickness(void* s, float Value);

int LEDIRestartMeasurement(void* s);

//####################################### User interface ######################################

#include "LISE_ED_DLL_UI_Fct.h"

#endif

