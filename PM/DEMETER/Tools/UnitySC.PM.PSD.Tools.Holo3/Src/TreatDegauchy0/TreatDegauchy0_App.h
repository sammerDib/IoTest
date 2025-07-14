// TreatDegauchy0.h : fichier d'en-tête principal pour la DLL TreatDegauchy0
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux


// CTreatDegauchy0App
// Consultez TreatDegauchy0.cpp pour l'implémentation de cette classe
//

class CTreatDegauchy0App : public CWinApp
{
public:
	CTreatDegauchy0App();

// Substitutions
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
