// CalculationDLL.h : fichier d'en-tête principal pour la DLL CalculationDLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif
#include "FrGrTreatementWrapper.h"
#include "FrGrTreatement.h"
#include "H3Array2D.h"

#include "resource.h"		// symboles principaux


// CCalculationDLLApp
// Consultez CalculationDLL.cpp pour l'implémentation de cette classe
//

struct GLOBALTOPO
{
	CString sCameraCalibFile;
	CString sCalibResult;
	CString sCalibResultFileX;
	CString sCalibResultFileY;
};


class CCalculationDLLApp : public CWinApp
{
public:
	CCalculationDLLApp();
	~CCalculationDLLApp(void);

// Substitutions
public:
	virtual BOOL InitInstance();
	DECLARE_MESSAGE_MAP()
public:
};
