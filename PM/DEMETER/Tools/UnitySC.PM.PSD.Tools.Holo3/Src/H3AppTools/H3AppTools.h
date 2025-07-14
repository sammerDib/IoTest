// H3AppTools.h : main header file for the H3APPTOOLS DLL
//

#if !defined(AFX_H3APPTOOLS_H__5BCF1970_3975_4317_A596_48774862FCFC__INCLUDED_)
#define AFX_H3APPTOOLS_H__5BCF1970_3975_4317_A596_48774862FCFC__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

#include "H3WinApp.h"

#if _MSC_VER > 1200	
	#pragma warning(disable : 4996)
#endif

/////////////////////////////////////////////////////////////////////////////
// CH3AppToolsApp
// See H3AppTools.cpp for the implementation of this class
//

class CH3AppToolsApp : public CH3WinApp
{
public:
	CH3AppToolsApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CH3AppToolsApp)
	public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CH3AppToolsApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_H3APPTOOLS_H__5BCF1970_3975_4317_A596_48774862FCFC__INCLUDED_)
