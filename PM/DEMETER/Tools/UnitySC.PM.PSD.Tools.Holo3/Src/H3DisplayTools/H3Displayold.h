// H3Display.h: interface for the CH3Display class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3DISPLAY_H__7542718D_D48C_47C8_A792_435D58C3C91E__INCLUDED_)
#define AFX_H3DISPLAY_H__7542718D_D48C_47C8_A792_435D58C3C91E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


#include "H3Array2D.h"

#ifdef AFX_H3DISPLAYTOOLS_H__A772E6F7_F48B_4BB7_80D9_9BB511C7BF04__INCLUDED_
	#define H3DISPLAY_EXPORT_DECL __declspec(dllexport)
#else
	#define H3DISPLAY_EXPORT_DECL __declspec(dllimport)
#endif

class H3DISPLAY_EXPORT_DECL CH3Display  
{
public:
	void Set(H3_ARRAY2D_CPXFLT32 &SrcBuf);
	void Set(H3_ARRAY2D_FLT32 &SrcBuf);
	void SetGrid(int nShow);//0 : off
							//1 : tiret en bordure d'image
							//2 : grille ligne pointie
	bool SetLUT(H3_ARRAY2D_UINT8 &LUTBuf);
	void SetMaxMinScale(double nMaxScale,double nMinScale);
	void SetYLabel(CString strYLabel);
	void SetXLabel(CString strXLabel);
	void SetTitle(CString strTitle);
	void SetColorBar(int nColorBarShow);
	int m_nColorBarShow;//0 : off
						//1 : vertical
						//2 : horizontal pas réalisé

	void Set(H3_ARRAY2D_UINT8 &SrcBuf);
	void SetModifiedMessage(UINT Msg);
	void SetClientWindow(HWND hWnd);
	bool IsAllocated();
//	void Display(H3_ARRAY2D_UINT8 &SrcBuf,H3_ARRAY_UINT8 ParamBuf);

	// Constructeurs / destructeur
	CH3Display();
	virtual ~CH3Display();

protected:
	void *m_pImageDIB;
	void SendModifiedMessage();
	void Free();
	void InitMembers();


	UINT m_nModifiedMsg;
	HWND m_hWnd;
private:
	void DisplayWnd(void *pmyDIB);
	H3_ARRAY2D_UINT8 m_LUTBuf;
	int m_nGridShow;
	double m_nMaxScale;
	double m_nMinScale;
	CString m_strTitle;
	CString m_strXLabel;
	CString m_strYLabel;
	void SetDrawGrid(CRect rcImage);
	void SetTitleShow(CRect rcDestTitle);
	void SetXLabelShow(CRect rcDestXLabel);
	void SetYLabelShow(CRect rcDestYLabel);
	void SetColorBarShow(CRect rcDestScale);
};

#endif // !defined(AFX_H3DISPLAY_H__7542718D_D48C_47C8_A792_435D58C3C91E__INCLUDED_)
