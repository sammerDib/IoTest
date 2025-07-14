
// NanoNoiseIndexDlg.h : fichier d'en-tête
//

#pragma once


// boîte de dialogue CNanoNoiseIndexDlg
class CNanoNoiseIndexDlg : public CDialogEx
{
// Construction
public:
	CNanoNoiseIndexDlg(CWnd* pParent = NULL);	// constructeur standard

// Données de boîte de dialogue
	enum { IDD = IDD_NANONOISEINDEX_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// Prise en charge de DDX/DDV


// Implémentation
protected:
	HICON m_hIcon;
	HKEY  m_hRegKey;

	// Fonctions générées de la table des messages
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
	void BrowseCSV();

public:
	afx_msg void OnBnClickedButtonCalcul();
	
	CString m_csEditFile;
	afx_msg void OnBnClickedButtonBrowse();
	int m_nFilterFrameSize;
	double m_dSeuilPenalite; 
	double m_dNoiseIndex;
	int m_nFilterOrder;
	virtual void OnOK();
	virtual void OnCancel();
	afx_msg BOOL OnHelpInfo(HELPINFO* pHelpInfo);

	bool OpenRegistry();
	void CloseRegistry();

	CString GetReg_CString(CString p_csKeyName, CString p_csDefault = _T(""));
	void SetReg_CString(CString p_csKeyName, CString p_csVal);

	int GetReg_int(CString p_csKeyName, int p_nDefault = 0);
	void SetReg_int(CString p_csKeyName, int p_nVal);

	float GetReg_float(CString p_csKeyName, float p_fDefault = 0.0f);
	void SetReg_float(CString p_csKeyName, float p_fVal);

};
