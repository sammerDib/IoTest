#pragma once

#define NB_IMG_POS 20 //6

// Boîte de dialogue CDlgCalibCam

class CDlgCalibCam : public CDialog
{
	DECLARE_DYNAMIC(CDlgCalibCam)

public:
	CDlgCalibCam(CWnd* pParent = NULL);   // constructeur standard
	virtual ~CDlgCalibCam();

// Données de boîte de dialogue
	enum { IDD = IDD_DLG_CALIBCAM };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // Prise en charge de DDX/DDV

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedButtonCalibcam();
    bool DoCalibration();
	virtual BOOL OnInitDialog();

	virtual void OnOK();
	virtual void OnCancel();
	virtual void WinHelp(DWORD dwData, UINT nCmd = HELP_CONTEXT);

private :
	void EnabledUI(BOOL p_bEnable);

private :
	CEdit m_ctrlEditSizeX;
	CEdit m_ctrlEditSizeY;
	CEdit m_ctrlEditStepX;
	CEdit m_ctrlEditStepY;
	

	CButton m_ctrlBtnBrsw[NB_IMG_POS];
	CEdit m_ctrlEditAcq[NB_IMG_POS];
	

	CButton m_ctrlBtnCalibCam;

    void SaveSettings();

public:
    CString m_csEditAcq[NB_IMG_POS];
    int m_nbImages = 6;
    int m_nSizeX;
    int m_nSizeY;
    float m_fStepX;
    float m_fStepY;


	void BrowseAcqImg(UINT p_nId);
	afx_msg void OnBnClickedButtonBrswAcq1();
	afx_msg void OnBnClickedButtonBrswAcq2();
	afx_msg void OnBnClickedButtonBrswAcq3();
	afx_msg void OnBnClickedButtonBrswAcq4();
	afx_msg void OnBnClickedButtonBrswAcq5();
	afx_msg void OnBnClickedButtonBrswAcq6();
	afx_msg void OnBnClickedButtonSaveprm();
	afx_msg void OnMove(int x, int y);
};
