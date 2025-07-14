// H3DSLRCalibration.h : fichier d'en-tête principal pour la DLL H3DSLRCalibration
//
#ifndef __H3_DSLR_CALIBRATION__
#define __H3_DSLR_CALIBRATION__


#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux


// CH3DSLRCalibrationApp
// Consultez H3DSLRCalibration.cpp pour l'implémentation de cette classe
//

class CH3DSLRCalibrationApp : public CWinAppEx//CWinApp
{
public:
	CH3DSLRCalibrationApp();

// Substitutions
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};


#endif __H3_DSLR_CALIBRATION__