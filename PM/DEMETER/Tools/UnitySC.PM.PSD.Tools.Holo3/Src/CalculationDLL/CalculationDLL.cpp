// CalculationDLL.cpp : définit les routines d'initialisation pour la DLL.
//
#pragma once
#include "stdafx.h"
#include "CalculationDLLApp.h"
#include "CalculationDLL.h"
#include "H3_HoloMap_AltaType.h"
#include "H3AppToolsDecl.h"

#include <map>
#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: si cette DLL est liée dynamiquement aux DLL MFC,
//		toute fonction exportée de cette DLL qui appelle
//		MFC doit avoir la macro AFX_MANAGE_STATE ajoutée au
//		tout début de la fonction.
//
//		Par exemple :
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// corps de fonction normal ici
//		}
//
//		Il est très important que cette macro se trouve dans chaque
//		fonction, avant tout appel à MFC. Cela signifie qu'elle
//		doit être la première instruction dans la 
//		fonction, avant toute déclaration de variable objet
//		dans la mesure où leurs constructeurs peuvent générer des appels à la DLL
//		MFC.
//
//		Consultez les notes techniques MFC 33 et 58 pour plus de
//		détails.
//


static std::map<int, CFrGrGlobalTopo*> s_globalTopoInstances;
static std::map<int, CFrGrTreatementWrapper*> s_calculationInstances;
static long m_lInstanceIndex = 0;
static long m_lGlobalTopoInstanceIndex=0;
static GLOBALTOPO m_globalTopoConfig;

extern "C" long __declspec(dllexport) CreateNewGlobalTopoInstance(const char* calibFolder)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    s_globalTopoInstances[m_lGlobalTopoInstanceIndex] = new CFrGrGlobalTopo(calibFolder);
    return m_lGlobalTopoInstanceIndex++;
}

extern "C" int __declspec(dllexport) CreateNewInstance(MIL_ID SystemID, long GlobalTopoInstanceIndex)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    ImageJMilSystemID = SystemID;

    CFrGrGlobalTopo* pGlobalTopoInstance = NULL;
    if (GlobalTopoInstanceIndex >= 0)
    {
        pGlobalTopoInstance = s_globalTopoInstances.find(GlobalTopoInstanceIndex)->second;
        if (pGlobalTopoInstance == NULL)
            return -1;
    }

    s_calculationInstances[m_lInstanceIndex]  = new CFrGrTreatementWrapper(SystemID);
    s_calculationInstances[m_lInstanceIndex]->SetGlobalTopo(pGlobalTopoInstance);
    return m_lInstanceIndex++;
}

extern "C" int __declspec(dllexport) InitializeGlobalTopo(long lInstanceID)
{
    //Initialisation de la topographie globale
    //GLOBALTOPO Config;
    //Config.sCalibResult.Format("%s", CalibResultFilePath);
    //Config.sCalibResultFileX.Format("%s", CalibResultXFilePath);
    //Config.sCalibResultFileY.Format("%s", CalibResultYFilePath);
    //Config.sCameraCalibFile.Format("%s", CameraCalibFilePath);

    CFrGrGlobalTopo* pGlobalTopoInstance = s_globalTopoInstances.find(lInstanceID)->second;
    if (pGlobalTopoInstance == NULL)
        return -1;

    pGlobalTopoInstance->LoadSettings1();
    return pGlobalTopoInstance->Init();
}

extern "C" bool __declspec(dllexport) SetCurvatureConfig(long lInstanceID, CURVATURE_CONFIG config)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->SetCurvatureConfig(config);
}


extern "C" bool __declspec(dllexport) SetGlobalTopoConfig(long lInstanceID, GLOBAL_TOPOCONFIG config)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->SetGlobalTopoConfig(config);
}

extern "C" bool __declspec(dllexport) SetFilterConfig(long lInstanceID, FILTER_FACTORY config)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->SetFilterConfig(config);
}

extern "C" bool __declspec(dllexport) DeleteInstance(long lInstanceID)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    s_calculationInstances.erase(lInstanceID);
    delete pInstance;
    return true;
}

extern "C" bool __declspec(dllexport) DeleteGlobalTopoInstance(long lInstanceID)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CH3_HoloMap_AltaType* pGlobalTopoInstance = s_globalTopoInstances.find(lInstanceID)->second;
    if (pGlobalTopoInstance == NULL)
        return false;
    s_globalTopoInstances.erase(lInstanceID);
    delete pGlobalTopoInstance;
    return true;
}

extern "C" __declspec(dllexport) bool SetInputInfo(long lInstanceID, INPUT_INFO info, int periods[])
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    ImageJSizeX = info.SizeX;
    ImageJSizeY = info.SizeY;

    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->SetInputInfo(info, periods);
}

extern "C" bool __declspec(dllexport)  PerformCalculation(long lInstanceID)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;

    bool CalculationResult = pInstance->PerformCalculation();
    if (CalculationResult && pInstance->m_bDoGlobalTopo)
        return pInstance->PerformGlobalTopo();
    return CalculationResult;
}

extern "C" float __declspec(dllexport)  PerformCurvatureCalibration(long lInstanceID)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return 0;

    float CalibrationResult = pInstance->PerformCurvatureCalibration();
   
    return CalibrationResult;
}

extern "C" bool __declspec(dllexport)  PerformIncrementalCalculation(long lInstanceID, int period, char direction)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;

    bool CalculationResult = pInstance->PerformIncrementalCalculation(period, direction);

    if (period==0 && direction=='Y' && CalculationResult && pInstance->m_bDoGlobalTopo && !pInstance->m_globalTopoConfig.UnwrappedPhase) 
        return pInstance->PerformGlobalTopo();
    else if (direction == 'Y' && CalculationResult && pInstance->m_bDoGlobalTopo && pInstance->m_globalTopoConfig.UnwrappedPhase && period == pInstance->m_Periods.size() - 1)
        return pInstance->PerformGlobalTopo();

    return CalculationResult;
}

extern "C" bool __declspec(dllexport) UpdateCurvatureConfig(long lInstanceID, CURVATURE_CONFIG value, TypeOfFrame typeOfFrame)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;

    return pInstance->UpdateCurvatureConfig(value, typeOfFrame);
}

extern "C" __declspec(dllexport) MIL_ID GetResultImage(long lInstanceID, TypeOfFrame typeOfFrame, int index)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->GetResultImage(typeOfFrame, index);
}

extern "C" __declspec(dllexport) CH3GenericArray2D * AccessWrappedPhaseOrMask(long lInstanceID, TypeOfFrame typeOfFrame, int index)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->AccessWrappedPhaseOrMask(typeOfFrame, index);
}

extern "C" __declspec(dllexport) bool GetIncrementalResultList(long lInstanceID, TypeOfFrame pTypeOfFrame[], int pIndex[], int& nbResults)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->GetIncrementalResultList(pTypeOfFrame, pIndex, nbResults);
}

extern "C" __declspec(dllexport) bool SetCrossImg(long lInstanceID, long MILID)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->SetCrossImg(MILID);
}

extern "C" bool __declspec(dllexport) SetInputImage(long lInstanceID, MIL_ID ImageID, int Index)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CFrGrTreatementWrapper* pInstance = s_calculationInstances.find(lInstanceID)->second;
    if (pInstance == NULL)
        return false;
    return pInstance->SetInputImage(ImageID, Index);
}