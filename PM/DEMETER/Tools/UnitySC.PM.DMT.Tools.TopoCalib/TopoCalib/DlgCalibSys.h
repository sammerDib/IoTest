#pragma once

#define NB_IMG_PHASE 6

struct CImageFloat;


// Boîte de dialogue CDlgCalibSys

class CDlgCalibSys : public CDialog
{
    DECLARE_DYNAMIC(CDlgCalibSys)

public:
    CDlgCalibSys(CWnd* pParent = NULL);   // constructeur standard
    virtual ~CDlgCalibSys();

    // Données de boîte de dialogue
    enum { IDD = IDD_DLG_CALIBSYS };

protected:
    virtual void DoDataExchange(CDataExchange* pDX);    // Prise en charge de DDX/DDV

    DECLARE_MESSAGE_MAP()

public:

    virtual BOOL OnInitDialog();

    virtual void OnOK();
    virtual void OnCancel();
    virtual void WinHelp(DWORD dwData, UINT nCmd = HELP_CONTEXT);

private:
    void EnabledUI(BOOL p_bEnable);

private:
    CEdit m_ctrlEditPitchX;
    CEdit m_ctrlEditPitchY;
    CEdit m_ctrlEditPeriodX;
    CEdit m_ctrlEditPeriodY;
    CEdit m_ctrlEditCrossX;
    CEdit m_ctrlEditCrossY;



    CButton m_ctrlBtnBrsw[NB_IMG_PHASE];
    CEdit m_ctrlEditAcq[NB_IMG_PHASE];

    CButton m_ctrlBtnBrswVideo;
    CEdit m_ctrlEditVideo;

    CButton m_ctrlBtnBrswMask;
    CEdit m_ctrlEditMask;

    CButton m_ctrlBtnCalibSys;

    bool SaveGreyImageFlt32(CString p_csFilepath, CImageFloat* p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);

public:

    /// <summary>
    /// Silent mode (without main window) when integrated in Demeter.
    /// </summary>
    bool _silentMode = false;

    float m_fPitchX;
    float m_fPitchY;
    float m_fPeriodX;
    float m_fPeriodY;
    int m_nCrossX;
    int m_nCrossY;

    int m_nScreenSizeX;
    int m_nScreenSizeY;
    int m_nScreenRefPosX;
    int m_nScreenRefPosY;

    /// <summary>
    /// Phases images filepaths.
    /// </summary>
    CString m_csEditAcq[NB_IMG_PHASE];

    /// <summary>
    /// Sorts m_csEditAcq from smallest period to biggest period (according to file names), and inits m_arrayRatios.
    /// </summary>
    void SortPhasesImages();

    bool ComparePhaseNames(CString leftPhaseName, CString rightPhaseName, bool bAscending);
    int GetPeriodFromPhaseName(CString phaseName);
    CString GetOrientationFromPhaseName(CString phaseName);

    CString m_csEditVideo;

    /// <summary>
    /// Ratios between phases are now detected from file names.
    /// </summary>
    float m_arrayRatios[3] = { -46,-46,-46 };

    bool DoCalibration();
    void BrowsePhaseImg(UINT p_nId);
    void BrowseAcqImg(UINT p_nId);
    afx_msg void OnBnClickedButtonBrswAcq1();
    afx_msg void OnBnClickedButtonBrswAcq2();
    afx_msg void OnBnClickedButtonBrswAcq3();
    afx_msg void OnBnClickedButtonBrswAcq4();
    afx_msg void OnBnClickedButtonBrswAcq5();
    afx_msg void OnBnClickedButtonBrswAcq6();

    afx_msg void OnBnClickedButtonBrswVideo();
    afx_msg void OnBnClickedButtonBrswMask();

    afx_msg void OnBnClickedButtonSaveprm();

    afx_msg void OnBnClickedButtonCalibsys();

};
