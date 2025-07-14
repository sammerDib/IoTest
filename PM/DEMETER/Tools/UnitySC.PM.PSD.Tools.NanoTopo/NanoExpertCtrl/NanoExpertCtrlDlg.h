
// NanoExpertCtrlDlg.h : fichier d'en-tête
//

#pragma once


// boîte de dialogue CNanoExpertCtrlDlg
class CNanoExpertCtrlDlg : public CDialogEx
{
// Construction
public:
	CNanoExpertCtrlDlg(CWnd* pParent = NULL);	// constructeur standard

// Données de boîte de dialogue
	enum { IDD = IDD_NANOEXPERTCTRL_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// Prise en charge de DDX/DDV

private :
	bool OpenRegistryNano();
	void CloseRegistry();

	void InitTreatName();
	CString GetTreatName(int nID);

// Implémentation
protected:
	HICON m_hIcon;
	HKEY m_hRegNanoKey;

	CString m_csCoreName;
	CString m_csOrderName;
	CString m_csPrepareDataName;
	CString m_csReconstructName;
	CString m_csFilterName;
	CString m_csGenResName;

	// Fonctions générées de la table des messages
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnClose();
	afx_msg void OnBnClickedCoreSavedata();
	afx_msg void OnBnClickedCoreTiming();
	afx_msg void OnBnClickedCoreSaveslope();
	void SetReg(CString csTreatKey, int bValue, DWORD dwFlag);
	DWORD GetReg(CString csTreatKey);
	afx_msg void OnBnClickedOrderLog();
	afx_msg void OnBnClickedOrderDisplay();
	afx_msg void OnBnClickedOrderNx();
	afx_msg void OnBnClickedOrderNy();
	afx_msg void OnBnClickedPrepLog();
	afx_msg void OnBnClickedPrepDisplay();
	afx_msg void OnBnClickedPrepMask();
	afx_msg void OnBnClickedPrepErode();
	afx_msg void OnBnClickedPrepDilate();
	afx_msg void OnBnClickedPrepNx();
	afx_msg void OnBnClickedPrepNy();
	afx_msg void OnBnClickedPrepFftkiller();
	afx_msg void OnBnClickedReconstructLog();
	afx_msg void OnBnClickedReconstructDisplay();
	afx_msg void OnBnClickedReconstructReccoefmap();
	afx_msg void OnBnClickedReconstructRech();
	afx_msg void OnBnClickedReconstructRecvignettes();
	afx_msg void OnBnClickedReconstructSkipaffine();
	afx_msg void OnBnClickedReconstructSkipbandthread();
	afx_msg void OnBnClickedReconstructSkipquadthread2();
	afx_msg void OnBnClickedFilterLog();
	afx_msg void OnBnClickedFilterDisplay();
	afx_msg void OnBnClickedFilterRechin();
	afx_msg void OnBnClickedFilterRechfilter();
	afx_msg void OnBnClickedGenresLog();
	afx_msg void OnBnClickedGenresDisplay();
	afx_msg void OnBnClickedGenresFilterliss();
	afx_msg void OnBnClickedGenresRecerode();
	afx_msg void OnBnClickedGenresRecpv2();
	afx_msg void OnBnClickedGenresRecpv10();
	afx_msg void OnBnClickedGenresSkippv2();
	afx_msg void OnBnClickedGenresSkippv10();
};
