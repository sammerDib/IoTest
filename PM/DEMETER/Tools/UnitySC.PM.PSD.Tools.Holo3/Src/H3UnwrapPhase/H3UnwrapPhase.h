// H3UnwrapPhase.h : main header file for the H3UNWRAPPHASE DLL
//

#if !defined(AFX_H3UNWRAPPHASE_H__50502A69_1BC4_431D_86C4_7BCA4B7AB607__INCLUDED_)
#define AFX_H3UNWRAPPHASE_H__50502A69_1BC4_431D_86C4_7BCA4B7AB607__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#if _MSC_VER > 1200	
	#pragma warning(disable : 4996)
#endif

#include "resource.h"		// main symbols
#include "H3WinApp.h"
#include "H3AppToolsDecl.h"
#include "H3Array2D.h"

/////////////////////////////////////////////////////////////////////////////
// CH3UnwrapPhaseApp
// See H3UnwrapPhase.cpp for the implementation of this class
//

class CH3UnwrapPhaseApp : public CH3WinApp
{
public:
	CH3UnwrapPhaseApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CH3UnwrapPhaseApp)
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CH3UnwrapPhaseApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_H3UNWRAPPHASE_H__50502A69_1BC4_431D_86C4_7BCA4B7AB607__INCLUDED_)
