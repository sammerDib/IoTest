// TreatPrepareData1.h : fichier d'en-tête principal pour la DLL TreatPrepareData1
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux


// CTreatPrepareData1App
// Consultez TreatPrepareData1.cpp pour l'implémentation de cette classe
//

class CTreatPrepareData1App : public CWinApp
{
public:
	CTreatPrepareData1App();

// Substitutions
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
