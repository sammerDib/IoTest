#pragma once

#include <mil.h>
#include "nanocore.h"
#include "H3AppToolsDecl.h"

enum TypeOfFrame
{
    CurvatureX      = 1,
    CurvatureY      = 2,
    AmplitudeX      = 4,
    AmplitudeY      = 8,
    UnwrappedPhaseX = 16,
    UnwrappedPhaseY = 32,
    GlobalTopoNX    = 64,
    GlobalTopoNY    = 128,
    GlobalTopoNZ    = 256,
    GlobalTopoX     = 512,
    GlobalTopoY     = 1024,
    GlobalTopoZ     = 2048,
    PhaseX          = 4096,
    PhaseY          = 8192,
    PhaseMask       = 16384,
    Dark            = 32768,
};

struct CURVATURE_CONFIG
{
    unsigned int nTargetBackgroundGrayLevel;   // Should be in AlgorithmsConfiguration.xml
    float fUserCurvatureDynamicsCoeff;  // Comes from the recipe (default in GUI: 1). Range: >0.
    float fCalibratedNoise; // Comes from curvature calibration
    int nCalibrationPeriod; // Comes from curvature calibration    
};

struct FILTER_FACTORY
{
    int Contraste_min_curvature;
    int Intensite_min_curvature;
};

struct GLOBAL_TOPOCONFIG
{
    int KnownHeightPixelX, KnownHeightPixelY;
    int PixelRefX, PixelRefY;
    float Height;
    float Ratio;
    int CrossSearchThreshold;
    int FringePeriod;
    bool UnwrappedPhase;    // true when the phase sent to GlobalTopo has already been unwrapped (multiperiod)
};

struct INPUT_INFO
{
    int NbPeriods;
    int NbImgX, NbImgY;
    int SizeX, SizeY;
    TypeOfFrame TypeOfFrame;
};

extern "C" __declspec(dllexport) long CreateNewGlobalTopoInstance(const char* calibFolder);
extern "C" __declspec(dllexport) int  InitializeGlobalTopo(long lInstanceID);
extern "C" __declspec(dllexport) bool DeleteGlobalTopoInstance(long lInstanceID);
extern "C" __declspec(dllexport) int  CreateNewInstance(MIL_ID SystemID, long GlobalTopoInstanceIndex);

extern "C" __declspec(dllexport) bool SetCurvatureConfig(long lInstanceID, CURVATURE_CONFIG config);
extern "C" __declspec(dllexport) bool SetFilterConfig(long lInstanceID, FILTER_FACTORY config);
extern "C" __declspec(dllexport) bool SetGlobalTopoConfig(long lInstanceID, GLOBAL_TOPOCONFIG config);
extern "C" __declspec(dllexport) bool SetInputInfo(long lInstanceID, INPUT_INFO info, int periods[]);
extern "C" __declspec(dllexport) bool SetInputImage(long lInstanceID, MIL_ID ImageID, int Index);
extern "C" __declspec(dllexport) bool SetCrossImg(long lInstanceID, long MILID);
extern "C" __declspec(dllexport) bool PerformCalculation(long lInstanceID);
extern "C" __declspec(dllexport) bool PerformIncrementalCalculation(long lInstanceID, int period, char direction);
extern "C" __declspec(dllexport) bool UpdateCurvatureConfig(long lInstanceID, CURVATURE_CONFIG config, TypeOfFrame typeOfFrame);
extern "C" __declspec(dllexport) MIL_ID GetResultImage(long lInstanceID, TypeOfFrame typeOfFrame, int index = 0);
extern "C" __declspec(dllexport) CH3GenericArray2D* AccessWrappedPhaseOrMask(long lInstanceID, TypeOfFrame typeOfFrame, int index = 0);
extern "C" __declspec(dllexport) bool GetIncrementalResultList(long lInstanceID, TypeOfFrame pTypeOfFrame[], int pIndex[], int& nbResults);
extern "C" __declspec(dllexport) bool DeleteInstance(long lInstanceID);
