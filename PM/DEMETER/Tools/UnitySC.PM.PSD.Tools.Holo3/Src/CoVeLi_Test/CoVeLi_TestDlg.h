
// CoVeLi_TestDlg.h : fichier d'en-tête
//

#pragma once


// boîte de dialogue CCoVeLi_TestDlg
class CCoVeLi_TestDlg : public CDialog
{
// Construction
public:
	CCoVeLi_TestDlg(CWnd* pParent = nullptr);	// constructeur standard

// Données de boîte de dialogue
	enum { IDD = IDD_COVELI_TEST_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// Prise en charge de DDX/DDV


// Implémentation
protected:
	HICON m_hIcon;

	// Fonctions générées de la table des messages
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedMesureZ0();
	afx_msg void OnBnClickedCalibSys();
	afx_msg void OnBnClickedButtonInit();
	afx_msg void OnBnClickedCalibCam();
	afx_msg void OnBnClickedMesureZ();

private:
    static CString m_CalibFolder;
};
