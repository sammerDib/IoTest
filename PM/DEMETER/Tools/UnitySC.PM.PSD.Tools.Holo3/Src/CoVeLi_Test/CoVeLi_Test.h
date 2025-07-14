
// CoVeLi_Test.h : fichier d'en-tête principal pour l'application PROJECT_NAME
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux


// CCoVeLi_TestApp:
// Consultez CoVeLi_Test.cpp pour l'implémentation de cette classe
//

class CCoVeLi_TestApp : public CWinAppEx
{
public:
	CCoVeLi_TestApp();

// Substitutions
	public:
	virtual BOOL InitInstance();

// Implémentation

	DECLARE_MESSAGE_MAP()
};

extern CCoVeLi_TestApp theApp;