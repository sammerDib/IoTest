// Coveli.h : main header file for the COVELI DLL
//

#if !defined(AFX_COVELI_H__4BD85349_6C93_453D_84BE_C8572D8A2F42__INCLUDED_)
#define AFX_COVELI_H__4BD85349_6C93_453D_84BE_C8572D8A2F42__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols
#include "H3Matrix.h"
#include "H3WinApp.h"

void H3CoVeLi_DisplayError(const CString&a,const CString&b,const CString&c);
void H3CoVeLi_DebugError  (const CString&a,const CString&b,const CString&c);

/////////////////////////////////////////////////////////////////////////////
// CCoveliApp
// See Coveli.cpp for the implementation of this class
//

class CCoveliApp : public CH3WinApp
{
public:

	CCoveliApp();

private:
	CString m_strIniFile;

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCoveliApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CCoveliApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_COVELI_H__4BD85349_6C93_453D_84BE_C8572D8A2F42__INCLUDED_)
