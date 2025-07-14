// H3ImageTools.h : main header file for the H3IMAGETOOLS DLL
//

#if !defined(AFX_H3IMAGETOOLS_H__4E542AE6_FA41_406F_9889_096978E36A80__INCLUDED_)
#define AFX_H3IMAGETOOLS_H__4E542AE6_FA41_406F_9889_096978E36A80__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols
#include "H3AppToolsDecl.h"
#include ".\H3ImageToolsDecl.h"


/////////////////////////////////////////////////////////////////////////////
// CH3ImageToolsApp
// See H3ImageTools.cpp for the implementation of this class
//


class CH3ImageToolsApp : public CWinApp
{
public:
	CH3ImageToolsApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CH3ImageToolsApp)
	public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CH3ImageToolsApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_H3IMAGETOOLS_H__4E542AE6_FA41_406F_9889_096978E36A80__INCLUDED_)
