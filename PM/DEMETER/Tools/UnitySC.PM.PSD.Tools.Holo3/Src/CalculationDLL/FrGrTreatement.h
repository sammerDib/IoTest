#pragma once
#include "StdAfx.h"
#include <map>
#include "H3Array.h"
#include "H3Array2D.h"
#include "CalculationDLL.h"
#include "H3Calculation.h"
#include "ImageInfoClass.h"

//struct CImageByte;
//class CImageInfoClass; 
//class CImageInfoClassInput; 
//class CParametersInfoClass;
//class CH3Calculation;
//class CFrGrTreatementWrapper;

/////////////////////////////////////////////////////////////////////////////////////////////

class CFrGrTreatement
{

public:
	CFrGrTreatement(void);
	~CFrGrTreatement(void);

private:
    void ReportProgress(int step, const char* msg);
    void SaveAmplitude(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc);
    void SavePhases(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc);
    void SavePhase(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc, int period);
    void SaveMask(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc);
    void SaveCurvature(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc);
    void UnwrapPhase(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc);
    void ComputeDark(TypeOfFrame typeOfFrame, CH3Calculation& H3CalcX, CH3Calculation& H3CalcY);  // For new Dark image
    float CalibrateCuvatureDynamics(CH3Calculation& H3CalcX, CH3Calculation& H3CalcY); // Called in curvature computation, if we are in a curvature calibration step

public:
	void FrGr_GetLastError(CString* sErrorMessage); 
    bool FrGr_PerformCalculation(CImageInfoClassInput* pIIC);
    float FrGr_PerformCurvatureCalibration(CImageInfoClassInput* pIIC);
    bool FrGr_PerformIncrementalCalculation(CImageInfoClassInput* pIIC, int period, char direction);
    bool FrGr_RecalculateOutputs(TypeOfFrame typeOfFrame);
    bool FrGr_GetIncrementalResultList(TypeOfFrame pTypeOfFrame[], int pIndex[], int& nbResults);

	void AssociateWrapper(CFrGrTreatementWrapper* CallbackContainer);

public:
    CFrGrTreatementWrapper* m_wrapper = NULL;
    std::map<TypeOfFrame, CH3GenericArray2D*> m_ResultImages;

private:
    // Stockage de la liste des résultats
    struct Result
    {
        Result(TypeOfFrame typeOfFrame)
        {
            TypeOfFrame = typeOfFrame;
            Index = 0;
        }
        Result(TypeOfFrame typeOfFrame, int index)
        {
            TypeOfFrame = typeOfFrame;
            Index = index;
        }
        TypeOfFrame TypeOfFrame;
        int Index;
    };
    std::vector<Result> m_PartialResults;

private:
    CH3Calculation H3CalcX;
    CH3Calculation H3CalcY;
    int m_iProgressState = 0;
};
