#if !defined(AFX_DISPLAYDLG_H__7FCEAA7E_0725_46F0_AECF_0E195830031E__INCLUDED_)
#define AFX_DISPLAYDLG_H__7FCEAA7E_0725_46F0_AECF_0E195830031E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// DisplayDlg.h : header file
//

#include "H3Display.h"


/////////////////////////////////////////////////////////////////////////////
// CDisplayDlg dialog

class CDisplayDlg : public CDialog
{
// Construction
public:
	void ResizeWindow(int cx,int cy);
	void Update();
	CDisplayDlg(CH3Display *pDisplay,CWnd* pParent = nullptr);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CDisplayDlg)
	enum { IDD = IDD_DISPLAY };
	CStatic	m_CImage;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CDisplayDlg)
	public:
	virtual BOOL DestroyWindow();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	CH3Display *m_pDisplay;

	// Generated message map functions
	//{{AFX_MSG(CDisplayDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnDestroy();
	afx_msg void OnClose();
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnPaint();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_DISPLAYDLG_H__7FCEAA7E_0725_46F0_AECF_0E195830031E__INCLUDED_)
