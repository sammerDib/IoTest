// H3Display.h: interface for the CH3Display class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3DISPLAY_H__7542718D_D48C_47C8_A792_435D58C3C91E__INCLUDED_)
#define AFX_H3DISPLAY_H__7542718D_D48C_47C8_A792_435D58C3C91E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "H3Array2D.h"
#include "H3Point2D.h"

#ifdef _DLL
#ifdef AFX_H3DISPLAYTOOLS_H__A772E6F7_F48B_4BB7_80D9_9BB511C7BF04__INCLUDED_
	#define H3DISPLAY_EXPORT_DECL __declspec(dllexport)
#else
	#define H3DISPLAY_EXPORT_DECL __declspec(dllimport)
#endif
#else
#define H3DISPLAY_EXPORT_DECL
#endif

#define H3_DISPLAY_BACKGROUND_COLOR		0	
#define H3_DISPLAY_LOWRANGE_COLOR		1
#define H3_DISPLAY_HIGHRANGE_COLOR		2

typedef struct 
{
	long nColorBarStyle;
	long nRangeMode;
	H3_ARRAY_FLT32 Range;
	CString strXLabel;
	CString strYLabel;
	CString strTitle;
	H3_ARRAY2D_UINT8 ColorMap1;
	H3_ARRAY2D_UINT8 ColorMap2;
	CString strScaleLabel;
} H3DISPLAY_SETTINGS;

class H3DISPLAY_EXPORT_DECL CH3Display  
{
public:
	CRect GetRectZoom();
	bool GetRectZoomActif();
	void SetRectZoom(CRect rcRect);
	void SetXProfileMax(float fAbsMax);
	bool GetXProfileView();
	void SetXProfileView(bool bView);
	void SetXProfileColor(COLORREF crColor);
	void SetXProfileStyle(long nStyle);

	bool m_bDrawAxePixel;//MF
	bool GetbDrawAxePixel();//MF
	void SetbDrawAxePixel(bool bDrawAxePixel);//MF

	bool ConvBufToImage(H3_ARRAY_INT32 & X,H3_ARRAY_INT32 & Y);
	void SetScaleLabel(CString strText);
	COLORREF GetBkColor();
	void SetBkColor(COLORREF crColor);
	CRect GetColorBarRect();
	CRect GetYLabelRect();
	CRect GetXLabelRect();
	CRect GetTitleRect();
	void SetColorMap2(H3_ARRAY2D_UINT8  & ColorMap);
	H3_ARRAY2D_UINT8 GetColorMap2();
	void SetRange(H3_ARRAY_FLT32 & Range);
	void SetSettings(H3DISPLAY_SETTINGS *pSettings);
	void GetSettings(H3DISPLAY_SETTINGS *pSettings);
	long GetColorBarStyle();

	float PowerTen(int PowerTen);
	void SetXPosMode(long nPos);
	long GetXPosMode();
	void SetYPosMode(long nPos);
	long GetYPosMode();

	H3_ARRAY2D_UINT8 GetColorMap1();
	bool SetColorMap1(const CString &strFilename);

	void SetROI(H3_RECT_INT32 rc);
	void SetROIStyle(long nStyle);
	void SetROIColor(COLORREF cr);
	void SetROI(H3_ARRAY_PT2DINT32 pts);

	LOGFONT GetLogFont();
	bool Save(const CString &strFilename);
	long GetStatisticsPP();
	void SetStatisticsPP(long Value);

	void Draw(CWnd *pWnd);
	void Draw();
	void Draw(CDC *pDC,CRect rcDest);
	void DrawColorBar(CDC *pDC,CRect &rcDestScale);
	void DrawImage(CDC *pDC,CRect &rcDest);
	void DrawDIB(CDC *pDC, void *pDIB, CRect &rcDest,bool bTrue=false);
	void DrawROI(CDC *pDC);

	H3_ARRAY_FLT32 GetRange();
	long GetRangeMode();
	void SetRangeMode(long nMode);

	H3_POINT2D_FLT32 ImageToBuf(H3_POINT2D_FLT32 &pt);
	H3_POINT2D_FLT32 BufToImage(H3_POINT2D_FLT32 &pt);
	H3_ARRAY_PT2DFLT32 BufToImage(H3_ARRAY_PT2DFLT32 &pts);

	CRect GetImageRect();


	void SetGridStyle(int nShow); // 0 : off 1 : tiret en bordure d'image 2 : grille ligne pointiee
	bool SetColorMap1(H3_ARRAY2D_UINT8 &LUTBuf);
	void SetRange(double Low,double High);
	void SetYLabel(const CString &strYLabel);
	void SetXLabel(const CString &strXLabel);
	void SetTitle(const CString &strTitle);
	void SetColorBarStyle(int nStyle);

	void Set(const H3_ARRAY2D_UINT8 &SrcBuf);
	void Set(const H3_ARRAY2D_UINT16 &SrcBuf);
	void Set(const H3_ARRAY2D_FLT32 &SrcBuf);
	void Set(const H3_ARRAY2D_RGB24 &SrcBuf);
	void Set(unsigned short *pData,long nSizeX, long nSizeY, long nPitch);
	void Set(unsigned char  *pData,long nSizeX, long nSizeY, long nPitch);
	void Set(H3_RGB24 *pData, long nSizeX, long nSizeY, long nPitch);

	bool IsAllocated();

	// Constructeurs / destructeur
	CH3Display();
	virtual ~CH3Display();


private:
	void Free();
	void SetScaleFactorPowerTen(int nScaleFactorPowerTen=1);


private:
	CRect m_rcRectZoom;
	int m_nScaleFactorPowerTen;
	int GetScaleFactor();

	void *m_pXIntensityProfile;			///> Profile d'intensité X
	COLORREF m_crBkColor;

	int m_nColorBarStyle;//0 : off
						//1 : vertical
						//2 : horizontal pas réalisé
	long m_XPos;
	long m_YPos;
	struct ROI_STRUCT
	{
		long nStyle;
		COLORREF crColor;
		H3_ARRAY_PT2DFLT32 pts;
	}m_ROI;

	long m_nStatisticsPP;
	void *m_pDisplayDlg;
public:
	void *m_pImageDIB;
private:
	void *m_pColorBarDIB;
	void InitMembers();

	H3_ARRAY_FLT32 m_Range;
	H3_ARRAY2D_UINT8 m_ColorMap1;	// Couleurs avant plan
	H3_ARRAY2D_UINT8 m_ColorMap2;	// Couleurs arriere plan

	long m_nRangeMode;
	void CalcRange(unsigned char *pData,long nSizeX, long nSizeY, long nPitch);
	void CalcRange(unsigned short *pData,long nSizeX, long nSizeY, long nPitch);
	void CalcRange(const H3_ARRAY2D_FLT32 &SrcBuf);

	CRect m_rcImage;
	CRect m_rcTitle;
	CRect m_rcXLabel;
	CRect m_rcYLabel;
	CRect m_rcColorBar;

	LOGFONT m_LogFont;
	void DisplayWnd(void *pmyDIB);
	int m_nGridStyle;
	CString m_strTitle;
	CString m_strXLabel;
	CString m_strYLabel;
	CString m_strScaleLabel;
	void DrawGrid(CDC *pDC,CRect rcImage);
	void DrawTitle(CDC *pDC,CRect &rcDest);
	void DrawXLabel(CDC *pDC,CRect &rcDest);
	void DrawYLabel(CDC *pDC,CRect &rcDest);
	void DrawRectangle(CDC *pDC,CRect &rcRect);
	void DrawText(CDC *pDC,CString& str,CRect &rcRect,UINT nFormat,CFont &Font);

	void DrawXAxePixel(CDC *pDC,CRect &rcDest);//MF
	void DrawYAxePixel(CDC *pDC,CRect &rcDest);//MF
	void DrawXReglePixel(CDC *pDC,CRect &rcRect); //MF;
	void DrawYReglePixel(CDC *pDC,CRect &rcRect); //MF;
};

#endif // !defined(AFX_H3DISPLAY_H__7542718D_D48C_47C8_A792_435D58C3C91E__INCLUDED_)
