// H3DisplayTools.h : main header file for the H3DISPLAYTOOLS DLL
//

#if !defined(AFX_H3DISPLAYTOOLS_H__A772E6F7_F48B_4BB7_80D9_9BB511C7BF04__INCLUDED_)
#define AFX_H3DISPLAYTOOLS_H__A772E6F7_F48B_4BB7_80D9_9BB511C7BF04__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols
#include "H3AppToolsDecl.h"
#include "H3WinApp.h"

#define CR_GREEN				RGB(0,255,0)
#define CR_BLUE					RGB(255,0,0)
#define CR_RED					RGB(0,0,255)
#define CR_YELLOW				RGB(255,255,0)


/////////////////////////////////////////////////////////////////////////////
// CH3DisplayToolsApp
// See H3DisplayTools.cpp for the implementation of this class
//

class CH3DisplayToolsApp : public CH3WinApp
{
public:
	CH3DisplayToolsApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CH3DisplayToolsApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CH3DisplayToolsApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_H3DISPLAYTOOLS_H__A772E6F7_F48B_4BB7_80D9_9BB511C7BF04__INCLUDED_)
