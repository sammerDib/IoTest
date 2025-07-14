#if !defined(AFX_CALIBRAGEDLG_H__865E2AA4_D67A_45EA_9B1B_A1C745BCDEF7__INCLUDED_)
#define AFX_CALIBRAGEDLG_H__865E2AA4_D67A_45EA_9B1B_A1C745BCDEF7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CalibrageDlg.h : header file
//

typedef struct
{
	CString	m_strCorrectPassWord;
	CString	m_strPassWord;
	BOOL	m_bCalibrerCamera0;
	bool	m_bCalibrerCamera0Enable;
	CString	m_strCFGFile;
	CString	m_strSaveImagePathName;
	int		m_nImagesCount;
	BOOL	m_bSaveRepport;
	CString	m_strSaveRepport;

}stParamDLGCalibrageTotal;

/////////////////////////////////////////////////////////////////////////////
// CCalibrageDlg dialog

class CCalibrageDlg : public CDialog
{
// Construction
public:
	CCalibrageDlg(stParamDLGCalibrageTotal *pParam = nullptr, CWnd* pParent = nullptr);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CCalibrageDlg)
	enum { IDD = IDD_CALIBRAGE };
	CString	m_strPassWord;
	BOOL	m_bCalibrerCamera0;
	CString	m_strCFGFile;
	CString	m_strSaveImagePathName;
	int		m_nImagesCount;
	BOOL	m_bSaveRepport;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCalibrageDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CCalibrageDlg)
	afx_msg void OnBrowseCFGFileName();
	afx_msg void OnBrowseImagesPathName();
	afx_msg void OnBtnPassWord();
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

public:
	stParamDLGCalibrageTotal m_Param;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CALIBRAGEDLG_H__865E2AA4_D67A_45EA_9B1B_A1C745BCDEF7__INCLUDED_)
