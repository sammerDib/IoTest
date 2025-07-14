#pragma once
#include "stdafx.h"
#include "FrGrTreatement.h"
#include "mil.h"
#include "ImageInfoClass.h"
#include "FrGrGlobalTopo.h"
#include "CalculationDLL.h"

class CH3_HoloMap_AltaType;


class CFrGrTreatementWrapper
{
public:
	CFrGrTreatementWrapper(MIL_ID SystemID);
	~CFrGrTreatementWrapper(void);
    void SetGlobalTopo(CFrGrGlobalTopo* globalTopo) { m_FrGrGlobalTopo = globalTopo; }
    CFrGrGlobalTopo* GetGlobalTopo() { return m_FrGrGlobalTopo; }
    bool SetCurvatureConfig(CURVATURE_CONFIG config);
	bool SetFilterConfig(FILTER_FACTORY config);
	bool SetGlobalTopoConfig(GLOBAL_TOPOCONFIG config);
	bool SetInputInfo(INPUT_INFO info, int periods[]);
    bool PerformCalculation();
    float PerformCurvatureCalibration();
    bool PerformIncrementalCalculation(int period, char direction);
    bool UpdateCurvatureConfig(CURVATURE_CONFIG config, TypeOfFrame typeOfFrame);
    bool PerformGlobalTopo();
	bool SetInputImage(MIL_ID ImageID,int Index);
	MIL_ID GetResultImage(TypeOfFrame typeOfFrame, int index = 0);
    CH3GenericArray2D* AccessWrappedPhaseOrMask(TypeOfFrame typeOfFrame, int index = 0);
    bool GetIncrementalResultList(TypeOfFrame pTypeOfFrame[], int pIndex[], int& nbResults);
	bool SetCrossImg(long MILID);
	bool SearchScreenPixelReference_InPicture(int& PixRef_inPicture_X, int& PixRef_inPicture_Y, byte cThresholdValue, MIL_ID CrossImg, MIL_ID MaskImg);

public:
    CURVATURE_CONFIG m_curvatureConfig;
	FILTER_FACTORY m_filterFactory;
	GLOBAL_TOPOCONFIG m_globalTopoConfig;

	BOOL m_bKeepPhaseAndMask;
    BOOL m_bSlope;  // Slopes = unwrapped phases are designed for the hairline cracks project
	BOOL m_bDoGlobalTopo;
    TypeOfFrame m_TypeOfFrame;
    std::vector<int> m_Periods;

protected:
	CFrGrTreatement m_FrGrTreatment;
	CFrGrGlobalTopo* m_FrGrGlobalTopo;
	MIL_ID m_milSystemID;
	MIL_ID m_crossImg;
	CImageInfoClassInput m_IIC;
};