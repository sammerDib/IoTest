// NanoCore.h : fichier d'en-tête principal pour la DLL NanoCore
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux
#include "CoreMgr.h"
#include "H3AppToolsDecl.h"

// CNanoCoreApp
// Consultez NanoCore.cpp pour l'implémentation de cette classe
//

class CNanoCoreApp : public CWinApp
{
public:
	CNanoCoreApp();

// Substitutions
public:
	virtual BOOL InitInstance();

	bool Calibrate(CString p_csCalibrationImagesPath);

	bool InitTreatments(const CString& calibFolder);
	bool ReleaseTreatments();
    
	/// <summary>
	/// See NanoTopoLaunchMesure(...).
	/// </summary>
	bool LaunchMesure(LPCTSTR p_csIn, LPCTSTR p_csOut, LPCTSTR p_csLotID, int p_nPixelPeriod, LPCTSTR p_csFOUPID, int sourceType, bool bUnwrappedPhase, CH3GenericArray2D* phaseX = 0, CH3GenericArray2D* phaseY = 0, CH3GenericArray2D* phaseMask = 0);

	bool StopMesure( int p_nStopPrm );
	int  SetTreatmentName(UINT p_nTreatmentID, LPCTSTR p_csTreatName);
	int  SetTreatmentDbgFlag( UINT p_nTreatmentID, UINT p_uDbgFlag );
	int  SetTreatmentPrm(UINT p_nTreatmentID, LPCTSTR p_csTreatPrmName, LPCTSTR p_csTreatPrmValue);
	int  SetFilesGeneration(long p_ulFlags);
	int  SetExpandOffsets(int p_nOffsetX, int p_nOffsetY);
	int	 SetNUI(int p_nNUIEnable);

private :

	CCoreMgr m_oCoreMgr;
	bool m_bLaunchStop;

	DECLARE_MESSAGE_MAP()
	
};

extern "C" CalibPaths __declspec(dllexport) GetCalibFolderStructure();

extern "C" int __declspec(dllexport) NanoTopoInit(LPCTSTR calibFolder);

extern "C" int __declspec(dllexport) NanoTopoRelease();

// CH3GenericArray2D not nuls: uses phases images from memory.
// Else,
// sourceType<0 => loads phases from tif files (*3).
// sourceType=0 => loads phases from bin files (*3),
// sourceType>0 => loads normals from hbf files (*5).
extern "C" int __declspec(dllexport) NanoTopoLaunchMesure(LPCTSTR p_csIn, LPCTSTR p_csOut, LPCTSTR p_csLotID, int p_nPixelPeriod, LPCTSTR p_csFOUPID, int sourceType, bool bUnwrappedPhase, CH3GenericArray2D * phaseX = 0, CH3GenericArray2D * phaseY = 0, CH3GenericArray2D * phaseMask = 0);

extern "C" int __declspec(dllexport) NanoTopoStopMesure(int p_nStopPrm);

extern "C" int __declspec(dllexport) NanoTopoSetFilesGeneration(long p_ulFlags);

extern "C" int __declspec(dllexport) NanoTopoSetTreatmentName(UINT p_nTreatmentID, LPCTSTR p_csTreatName);
extern "C" int __declspec(dllexport) NanoTopoSetTreatmentDbgFlag(UINT p_nTreatmentID, UINT p_uDbgFlag);
extern "C" int __declspec(dllexport) NanoTopoSetTreatmentPrm(UINT p_nTreatmentID, LPCTSTR p_csTreatPrmName, LPCTSTR p_csTreatPrmValue);

extern "C" int __declspec(dllexport) NanoTopoSetExpandOffsets(int p_nOffsetX, int p_nOffsetY);
extern "C" int __declspec(dllexport) NanoTopoSetNUI(int p_nNUIEnable);

