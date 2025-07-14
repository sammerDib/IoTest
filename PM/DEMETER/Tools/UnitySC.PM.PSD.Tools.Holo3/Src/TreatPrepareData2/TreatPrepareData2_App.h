// TreatPrepareData2.h : fichier d'en-tête principal pour la DLL TreatPrepareData2
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux


// CTreatPrepareData2App
// Consultez TreatPrepareData2.cpp pour l'implémentation de cette classe
//

class CTreatPrepareData2App : public CWinApp
{
public:
	CTreatPrepareData2App();

// Substitutions
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
