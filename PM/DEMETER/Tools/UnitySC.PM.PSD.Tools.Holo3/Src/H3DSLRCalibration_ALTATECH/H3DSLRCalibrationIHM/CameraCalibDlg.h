#if !defined(AFX_CAMERACALIBDLG_H__923D36C9_61B7_4753_8D0F_A0CBA00FB71F__INCLUDED_)
#define AFX_CAMERACALIBDLG_H__923D36C9_61B7_4753_8D0F_A0CBA00FB71F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// CameraCalibDlg.h : header file
//

#ifdef _DLL
#  ifdef __H3_DSLR_CALIBRATION__
#    define H3CAMERACALIBDLG_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3CAMERACALIBDLG_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3CAMERACALIBDLG_EXPORT_DECL
#endif

#include "H3DSLRCalibrationIHMrc.h"
#include "H3DisplayTools.h" 
#include "H3Display.h" 
#include ".\\..\\H3DSLRCalibration\\H3CameraCalib.h"

#include <afxtempl.h>

#define TO_CALIBRATE_CAM 0
#define TO_GET_CORNERS 1
#define TO_GET_AREA 2

/////////////////////////////////////////////////////////////////////////////
// CCameraCalibDlg dialog

class H3CAMERACALIBDLG_EXPORT_DECL CCameraCalibDlg : public CDialog
{
// Construction
public:
	void ReDrawScene();
	bool Calibrage();
	CCameraCalibDlg(const CString& calibFolder);
	virtual ~CCameraCalibDlg(){delete m_pCameraCalib;};
	void SetMireFile(CString MireFile);
	bool LoadSettings(const CString& strFileName);
	bool SaveSettings(const CString& strFileName);
	void SetUse(unsigned long ul);
	H3_ARRAY2D_PT2DFLT32 GetPtIntersect()const;

	H3_ARRAY_PT2DFLT32 GetSelectedPoints()const;
	H3_ARRAY2D_UINT8 GetMask(size_t nLi, size_t nCo);
	void SetImage(H3_ARRAY2D_UINT8 &SrcBuf);
	void SetEtape(int Etape);

	int m_nNombreImageMinCalib;
	bool m_bSaveCalib;
	CString m_strSavePath;
	bool m_bSaveRepport;
	CString	m_strSaveRepport;

	int m_nGrabSource;
	int m_nNbPassImage;
	CH3Array<H3_ARRAY2D_UINT8> m_vMirePos;
	static int m_nErrorTypeCalibCam;

	bool m_bCalibSystem;

private:
	bool m_bGoodIntersection;
	int m_nTimerVideoTimer;

	void SetTitre();
	int m_nIndiceCamera;
	H3_ARRAY2D_PT2DFLT32 AddImage(H3_ARRAY2D_UINT8 &ImageMasque);
	void RemoveImage();
	bool CalculImage();
	void Draw4Points();
	H3_ARRAY2D_UINT8 GetImageVideo();
	void DrawIntersecPoints(	H3_ARRAY_PT2DFLT32 PointIntersec);
	CArray< H3_POINT2D_FLT32 > m_List4Points;
	H3_ARRAY2D_UINT8 m_ImageMasqueMire;
	H3_ARRAY2D_UINT8 m_BufferMasqueMire;

	void ActiveBTN(int choix);
	int GetEtape();
	int m_nEtape;
	H3_ARRAY2D_PT2DFLT32 m_PointIntersec;

	size_t m_nNbImage;
	bool m_bCalibrageValid;
	CH3CameraCalib* m_pCameraCalib;
	int m_nUse;

	size_t m_nNbImageReel;

// Dialog Data
	//{{AFX_DATA(CCameraCalibDlg)
	enum { IDD = IDD_CAMERACALIB_DLG };
	CButton	m_cBtnOK;
	CButton	m_cBtnRefuser;
	CButton	m_cBtnAcquerir;
	CButton	m_cBtnAccepter;
	CButton	m_cBtnAbandonner;
	CButton	m_cBtnLoadCorrespList2;
	CStatic	m_CImage;
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CCameraCalibDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	H3_ARRAY2D_UINT8 m_aBufImage;
	H3_ARRAY2D_UINT8 m_aLastImage;

	CH3Display m_Display;

	// Generated message map functions
	//{{AFX_MSG(CCameraCalibDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	afx_msg void OnBtnAcquerir();
	afx_msg void OnBtnLoadCorrespList2();
	afx_msg void OnBtnAbandonner();
	afx_msg void OnBtnRefuser();
	afx_msg void OnBtnAccepter();
	afx_msg void OnMove(int x, int y);
//	afx_msg void OnSizing(UINT fwSide, LPRECT pRect);
//	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnPaint();
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
public:
	CString m_csEditInfoText;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_CAMERACALIBDLG_H__923D36C9_61B7_4753_8D0F_A0CBA00FB71F__INCLUDED_)
