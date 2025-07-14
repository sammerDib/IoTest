// NanoCore.cpp : définit les routines d'initialisation pour la DLL.
//

#include "stdafx.h"
#include "NanoCore.h"

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

// CNanoCoreApp

BEGIN_MESSAGE_MAP(CNanoCoreApp, CWinApp)
END_MESSAGE_MAP()


// construction CNanoCoreApp

CNanoCoreApp::CNanoCoreApp()
{
    m_bLaunchStop = false;
}


// Seul et unique objet CNanoCoreApp

CNanoCoreApp theApp;


// initialisation de CNanoCoreApp

BOOL CNanoCoreApp::InitInstance()
{
    CWinApp::InitInstance();

    //_CrtDumpMemoryLeaks();

    // Récupération du flag de debug courant
    int tmpFlag = _CrtSetDbgFlag(_CRTDBG_REPORT_FLAG);
    // Force la recherche des 'memory-leak' à la fin de l'application
    _CrtSetDbgFlag(tmpFlag | _CRTDBG_LEAK_CHECK_DF);

    //_CrtSetBreakAlloc(151);		// Numéro de bloc devant générer un point d'arrêt

    //_CrtSetReportMode( _CRT_ERROR, _CRTDBG_MODE_DEBUG );

    if (::CreateDirectory(_T("C:\\Temp"), NULL) == FALSE)
    {
        if (GetLastError() != ERROR_ALREADY_EXISTS)
        {
            AfxMessageBox(_T("NANOCORE.DLL : Could not create Directory C:\\Temp"), MB_OK | MB_ICONEXCLAMATION);
        }
    }
    return TRUE;
}

bool CNanoCoreApp::InitTreatments(const CString& calibFolder)
{
    return m_oCoreMgr.InitTreatments(calibFolder);
}
bool CNanoCoreApp::ReleaseTreatments()
{
    return m_oCoreMgr.ReleaseTreatments();
}


bool CNanoCoreApp::Calibrate(CString p_csCalibrationImagesPath)
{
    return false;
}

bool CNanoCoreApp::LaunchMesure(LPCTSTR p_csIn, LPCTSTR p_csOut, LPCTSTR p_csLotID, int p_nPixelPeriod, LPCTSTR p_csFOUPID, int sourceType, bool bUnwrappedPhase, CH3GenericArray2D* phaseX, CH3GenericArray2D* phaseY, CH3GenericArray2D* phaseMask)
{
    m_bLaunchStop = false;
    m_oCoreMgr.ClearEmergencyStop();

    // InitCore and InitTreaments should have been successfully called here
    //
    bool bUsePreCalibrateFile = /*(sourceType > 0) &&*/ (phaseX == 0);
    m_oCoreMgr.m_csAcqImagesName = p_csLotID;
    m_oCoreMgr.m_csAcqImagesPath = p_csIn;
    m_oCoreMgr.m_csResultsPath = p_csOut;
    m_oCoreMgr.m_csAcqFOUPName = p_csFOUPID;
    CreateDir(m_oCoreMgr.m_csResultsPath);
    m_oCoreMgr.GetRegistryFlag(0);

    double dssMesureStart = GetPerfTime();
    double dssStart, dssEnd;

    // Normals as input
    if (bUsePreCalibrateFile)
    {
        dssStart = GetPerfTime();
        if (!m_oCoreMgr.LoadPreCalibrateImages())		// chargement des fichier HBF / Tiff où la calibration est dejà appliquée 
        {
            if (!m_oCoreMgr.LoadPreCalibratedTiff())
                return false;
        }
            
        dssEnd = GetPerfTime();
        if (m_oCoreMgr.IsLogTimingEnable())
            LogThis(1, 0, Fmt(_T(">>>>>> LoadHBFImages done in %f ms"), dssEnd - dssStart));
        else
            LogThis(1, 1, Fmt(_T(">>>>>> LoadHBFImages done")));

    }
    // Phases as input
    else
    {
        dssStart = GetPerfTime();
        if (phaseX != 0)
        {
            // Reference images directly in memory.
            m_oCoreMgr.ReferenceAcquisitionImages(phaseX, phaseY, phaseMask);
        }
        else if (!m_oCoreMgr.LoadAcquisitionImages(sourceType))	// chargement des images de phase, cross et mask
            return false;

        dssEnd = GetPerfTime();
        if (m_oCoreMgr.IsLogTimingEnable())
            LogThis(1, 0, Fmt(_T(">>>>>> LoadAcquisitionImages done in %f ms"), dssEnd - dssStart));
        else
            LogThis(1, 1, Fmt(_T(">>>>>> LoadAcquisitionImages done")));
        if (m_bLaunchStop)
        {
            LogThis(1, 2, Fmt(_T("User Stop Request - Exit Measure")));
            return false;
        }

        dssStart = GetPerfTime();
        if (!m_oCoreMgr.SlopeCalculation(p_nPixelPeriod, bUnwrappedPhase))		// calcul des pentes à partir de phase (application de la calibration)
            return false;
        dssEnd = GetPerfTime();
        if (m_oCoreMgr.IsLogTimingEnable())
            LogThis(1, 0, Fmt(_T(">>>>>> SlopeCalculation done in %f ms"), dssEnd - dssStart));
        else
            LogThis(1, 1, Fmt(_T(">>>>>> SlopeCalculation done")));
        if (m_bLaunchStop)
        {
            LogThis(1, 2, Fmt(_T("User Stop Request - Exit Measure")));
            return false;
        }
    }

    m_oCoreMgr.SaveIntermediateFiles("c:\\temp\\0apresChargement");

    if (m_oCoreMgr.IsNUIActive() == false)
    {
        // When NUI (Non uniformity correction) is activated the degauchi is done in prepare data treatment otherwise it is done by degauchi treatment
        dssStart = GetPerfTime();
        if (!m_oCoreMgr.Degauchyssage())			// degauchissage
            return false;
        dssEnd = GetPerfTime();
        if (m_oCoreMgr.IsLogTimingEnable())
            LogThis(1, 0, Fmt(_T(">>>>>> OrderingData done in %f ms"), dssEnd - dssStart));
        else
            LogThis(1, 1, Fmt(_T(">>>>>> OrderingData done")));
        if (m_bLaunchStop)
        {
            LogThis(1, 2, Fmt(_T("User Stop Request - Exit Measure")));
            return false;
        }
    }

    m_oCoreMgr.SaveIntermediateFiles("c:\\temp\\1apresDegauchissage");

    dssStart = GetPerfTime();
    if (!m_oCoreMgr.PrepareData())				// Extrapolation && fringe killer
        return false;
    dssEnd = GetPerfTime();
    if (m_oCoreMgr.IsLogTimingEnable())
        LogThis(1, 0, Fmt(_T(">>>>>> PrepareData done in %f ms"), dssEnd - dssStart));
    else
        LogThis(1, 0, Fmt(_T(">>>>>> PrepareData done")));

    m_oCoreMgr.SaveIntermediateFiles("c:\\temp\\2apresExtrapolationEtFringekiller");

    if (m_bLaunchStop)
    {
        LogThis(1, 2, Fmt(_T("User Stop Request - Exit Measure")));
        return false;
    }

    dssStart = GetPerfTime();
    if (!m_oCoreMgr.Reconstruct())				// vignettage et reconstruction
        return false;
    dssEnd = GetPerfTime();
    if (m_oCoreMgr.IsLogTimingEnable())
        LogThis(1, 0, Fmt(_T(">>>>>> Reconstruct done in %f ms"), dssEnd - dssStart));
    else
        LogThis(1, 1, Fmt(_T(">>>>>> Reconstruct done")));
    if (m_bLaunchStop)
    {
        LogThis(1, 2, Fmt(_T("User Stop Request - Exit Measure")));
        return false;
    }

    m_oCoreMgr.SaveIntermediateFiles("c:\\temp\\3apresReconstruction");

    dssStart = GetPerfTime();
    if (!m_oCoreMgr.Filtering())				// filtrage
        return false;
    dssEnd = GetPerfTime();
    if (m_oCoreMgr.IsLogTimingEnable())
        LogThis(1, 0, Fmt(_T(">>>>>> Filtering done in %f ms"), dssEnd - dssStart));
    else
        LogThis(1, 1, Fmt(_T(">>>>>> Filtering done")));
    if (m_bLaunchStop)
    {
        LogThis(1, 2, Fmt(_T("User Stop Request - Exit Measure")));
        return false;
    }

    m_oCoreMgr.SaveIntermediateFiles("c:\\temp\\4apresFiltrage");

    dssStart = GetPerfTime();
    if (!m_oCoreMgr.GenerateResults())		// Génération des résultats PVx / THA@ ...
        return false;
    dssEnd = GetPerfTime();
    if (m_oCoreMgr.IsLogTimingEnable())
        LogThis(1, 0, Fmt(_T(">>>>>> GenerateResults done in %f ms"), dssEnd - dssStart));
    else
        LogThis(1, 1, Fmt(_T(">>>>>> GenerateResults done")));

    double dssMesureEnd = GetPerfTime();
    if (m_oCoreMgr.IsLogTimingEnable())
        LogThis(1, 2, Fmt(_T(">>>>>> Wafer ID {%s} ALL DONE in %f ms"), p_csLotID, dssMesureEnd - dssMesureStart));
    else
        LogThis(1, 2, Fmt(_T(">>>>>> Wafer ID {%s} ALL DONE"), p_csLotID));

    m_oCoreMgr.SaveIntermediateFiles("c:\\temp\\5apresResultats");

    return true;
}

bool CNanoCoreApp::StopMesure(int p_nStopPrm)
{
    // Stop mesure -- p_nStopPrm could be use in future version if we want different kind of stop
    m_bLaunchStop = true;
    m_oCoreMgr.EmergencyStop();
    return true;
}

int CNanoCoreApp::SetTreatmentName(UINT p_nTreatmentID, LPCTSTR p_csTreatName)
{
    return m_oCoreMgr.SetTreatmentName(p_nTreatmentID, p_csTreatName);
}

int CNanoCoreApp::SetTreatmentDbgFlag(UINT p_nTreatmentID, UINT p_uDbgFlag)
{
    return m_oCoreMgr.SetTreatmentDbgFlag(p_nTreatmentID, p_uDbgFlag);
}

int CNanoCoreApp::SetTreatmentPrm(UINT p_nTreatmentID, LPCTSTR p_csTreatPrmName, LPCTSTR p_csTreatPrmValue)
{
    return m_oCoreMgr.SetTreatmentPrm(p_nTreatmentID, p_csTreatPrmName, p_csTreatPrmValue);
}

int CNanoCoreApp::SetFilesGeneration(long p_ulFlags)
{
    return m_oCoreMgr.SetFilesGeneration(p_ulFlags);
}

int CNanoCoreApp::SetExpandOffsets(int p_nOffsetX, int p_nOffsetY)
{
    return m_oCoreMgr.SetExpandOffsets(p_nOffsetX, p_nOffsetY);
}

int CNanoCoreApp::SetNUI(int p_nNUIEnable)
{
    return m_oCoreMgr.SetNUI(p_nNUIEnable);
}

extern "C" CalibPaths __declspec(dllexport) GetCalibFolderStructure()
{
    return _CalibPaths;
}

extern "C" int __declspec(dllexport) NanoTopoInit(LPCTSTR calibFolder)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    LogThis(1, 1, "InitNanoCore");
    bool bRes = theApp.InitTreatments(calibFolder);
    return (bRes ? 1 : 0);
}

extern "C" int __declspec(dllexport) NanoTopoRelease()
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    LogThis(1, 1, "ReleaseNanoCore");
    return theApp.ReleaseTreatments();
}

extern "C" int __declspec(dllexport) NanoTopoLaunchMesure(LPCTSTR p_csIn, LPCTSTR p_csOut, LPCTSTR p_csLotID, int p_nPixelPeriod, LPCTSTR p_csFOUPID, int sourceType, bool bUnwrappedPhase, CH3GenericArray2D * phaseX, CH3GenericArray2D * phaseY, CH3GenericArray2D * phaseMask)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    LogThis(1, 1, Fmt(_T("LaunchMesure FOUP ID : %s - Wafer ID : %s [In]={%s}"), p_csFOUPID, p_csLotID, p_csIn));
    bool bRes = theApp.LaunchMesure(p_csIn, p_csOut, p_csLotID, p_nPixelPeriod, p_csFOUPID, sourceType, bUnwrappedPhase, phaseX, phaseY, phaseMask);

    return bRes ? 1 : 0;
}

extern "C" int __declspec(dllexport) NanoTopoSetFilesGeneration(long p_ulFlags)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return theApp.SetFilesGeneration(p_ulFlags);
}

extern "C" int __declspec(dllexport) NanoTopoSetTreatmentName(UINT p_nTreatmentID, LPCTSTR p_csTreatName)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return theApp.SetTreatmentName(p_nTreatmentID, p_csTreatName);
}

extern "C" int __declspec(dllexport) NanoTopoSetTreatmentDbgFlag(UINT p_nTreatmentID, UINT p_uDbgFlag)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return theApp.SetTreatmentDbgFlag(p_nTreatmentID, p_uDbgFlag);
}

extern "C" int  __declspec(dllexport) NanoTopoSetTreatmentPrm(UINT p_nTreatmentID, LPCTSTR p_csTreatPrmName, LPCTSTR p_csTreatPrmValue)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return theApp.SetTreatmentPrm(p_nTreatmentID, p_csTreatPrmName, p_csTreatPrmValue);
}
extern "C" int  __declspec(dllexport) NanoTopoStopMesure(int p_nStopPrm)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return theApp.StopMesure(p_nStopPrm);
}

extern "C" int __declspec(dllexport) NanoTopoSetExpandOffsets(int p_nOffsetX, int p_nOffsetY)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return theApp.SetExpandOffsets(p_nOffsetX, p_nOffsetY);
}

extern "C" int __declspec(dllexport) NanoTopoSetNUI(int p_nNUIEnable)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    return theApp.SetNUI(p_nNUIEnable);
}