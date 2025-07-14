// FogaleProbeAppDlg.h : header file
//

#if !defined(AFX_FOGALEPROBEAPPDLG_H__58860D77_AAAD_4F33_843C_ACC7D2C2B4AF__INCLUDED_)
#define AFX_FOGALEPROBEAPPDLG_H__58860D77_AAAD_4F33_843C_ACC7D2C2B4AF__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CFogaleProbeAppDlg dialog

class CFogaleProbeAppDlg : public CDialog
{
// Construction
public:
	CFogaleProbeAppDlg(CWnd* pParent = NULL);	// standard constructor

	int StuffedTxbox(int RID); // désolé pour le dérangement je voulais juste faire une fonction toute simple et apparemment il faut la déclarer là pour que ca marche

// Dialog Data
	//{{AFX_DATA(CFogaleProbeAppDlg)
	enum { IDD = IDD_FOGALEPROBEAPP_DIALOG };
	CString	m_labelVersion;
	CString	m_ReturnFunction;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CFogaleProbeAppDlg)
	public:
	virtual BOOL DestroyWindow();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CFogaleProbeAppDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnGetversion();
	afx_msg void OnProbeinit();
	afx_msg void OnGetthickness();
	afx_msg void OnGetRawSignal();
	afx_msg void OnDefinesample();
	afx_msg void OnStart();
	afx_msg void OnStop();
	afx_msg void OnCloseprobe();
	afx_msg void OnDoSettings();
	afx_msg void OnOpenSettingWindow();
	afx_msg void OnCloseSettingWindow();
	afx_msg void OnLabelversion();
	afx_msg void OnUpdateSettingWindow();
	afx_msg void OnDoPowerTest();
	afx_msg void OnDoXYTest();
	afx_msg void OnDoFocusTest();
	afx_msg void OnDoMatTest();
	afx_msg void OnDoCalTest();
	afx_msg void OnBnClickedProbe1();
	afx_msg void OnBnClickedProbe2();

	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_FOGALEPROBEAPPDLG_H__58860D77_AAAD_4F33_843C_ACC7D2C2B4AF__INCLUDED_)
