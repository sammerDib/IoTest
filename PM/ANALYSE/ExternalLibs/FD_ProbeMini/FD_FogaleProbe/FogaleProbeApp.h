// FogaleProbeApp.h : main header file for the FOGALEPROBEAPP application
//

#if !defined(AFX_FOGALEPROBEAPP_H__C78DC78F_16CC_4F07_8E72_C05AA74771C4__INCLUDED_)
#define AFX_FOGALEPROBEAPP_H__C78DC78F_16CC_4F07_8E72_C05AA74771C4__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CFogaleProbeAppApp:
// See FogaleProbeApp.cpp for the implementation of this class
//

class CFogaleProbeAppApp : public CWinApp
{
public:
	CFogaleProbeAppApp();

// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFogaleProbeAppApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CFogaleProbeAppApp)
	afx_msg void OnThick1();
	afx_msg void OnGetthickloop();
	afx_msg void OnDotest();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FOGALEPROBEAPP_H__C78DC78F_16CC_4F07_8E72_C05AA74771C4__INCLUDED_)
