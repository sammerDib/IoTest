#pragma once

#include "H3Matrix.h"
#include "H3IOHoloMAP.h"

//struct CImageByte;
//struct CImageFloat;

// Boîte de dialogue CDlgMesure

class CDlgMesure : public CDialog
{
	DECLARE_DYNAMIC(CDlgMesure)

public:
	CDlgMesure(CWnd* pParent = NULL);   // constructeur standard
	virtual ~CDlgMesure();

// Données de boîte de dialogue
	enum { IDD = IDD_DLG_MESURE };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // Prise en charge de DDX/DDV

	DECLARE_MESSAGE_MAP()
public:
	void BrowsePhaseImg(UINT p_nId);
	afx_msg void OnBnClickedButtonMesure();
	afx_msg void OnBnClickedButtonSaveprm();
	afx_msg void OnBnClickedButtonBrswAcq1();
	afx_msg void OnBnClickedButtonBrswAcq2();
	afx_msg void OnBnClickedButtonBrswAcq3();
	afx_msg void OnBnClickedButtonBrswFolder();

	virtual BOOL OnInitDialog();
	virtual void OnOK();
	virtual void OnCancel();
	virtual void WinHelp(DWORD dwData, UINT nCmd = HELP_CONTEXT);

private :
	void EnabledUI(BOOL p_bEnable);

	bool SaveGreyImageByte(CString p_csFilepath, CImageByte* p_oMatrixByte);
	bool SaveGreyImageFlt32(CString p_csFilepath, CImageFloat* p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	bool SaveGreyImageFlt32(CString p_csFilepath, H3_MATRIX_FLT32& p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	bool SaveMatrix(H3_MATRIX_FLT32& p_oMatrix, CString p_csPath, CString p_csMatrixName);
	bool SaveBin(H3_MATRIX_FLT32& p_oMatrix, FILE* stream);

	bool Save3DA(H3_MATRIX_FLT32& p_oMatrix, CString p_csPath, CString p_csMatrixName);
	CButton m_ctrlBtnBrsw[3];
	CEdit m_ctrlEditAcq[3];
	CString m_csEditAcq[3];

	CButton m_ctrlBtnBrswOutFolder;
	CEdit m_ctrlEditOutFolder;
	CString m_csEditOutFolder;
public:
	afx_msg void OnEnChangeEditAltinitposX();
	float m_fAltiInit_um;
	UINT m_uAltInitPosX;
	UINT m_uAltInitPosY;
	UINT m_uRefPosInPicX;
	UINT m_uRefPosInPicY;
};
