#if !defined(AFX_RESULTCALIBRAGEDLG_H__BCA7D55E_E906_4584_B3D5_BA12A865F1E6__INCLUDED_)
#define AFX_RESULTCALIBRAGEDLG_H__BCA7D55E_E906_4584_B3D5_BA12A865F1E6__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ResultCalibrageDlg.h : header file
//


/////////////////////////////////////////////////////////////////////////////
// CResultCalibrageDlg dialog

class CResultCalibrageDlg : public CDialog
{
// Construction
public:
	CResultCalibrageDlg(bool ValidCalib, CString strRes, CWnd* pParent = nullptr);   // standard constructor

// Dialog Data
	//{{AFX_DATA(CResultCalibrageDlg)
	enum { IDD = IDD_RESULTCALIBRAGE };
	CStatic	m_stColor;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CResultCalibrageDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(CResultCalibrageDlg)
	afx_msg void OnPaint();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	bool m_bValidCalib;
	CString	m_strColor;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_RESULTCALIBRAGEDLG_H__BCA7D55E_E906_4584_B3D5_BA12A865F1E6__INCLUDED_)
