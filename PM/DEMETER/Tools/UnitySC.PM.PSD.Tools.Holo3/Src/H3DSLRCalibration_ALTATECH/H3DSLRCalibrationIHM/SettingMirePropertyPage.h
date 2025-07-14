#if !defined(AFX_SETTINGMIREPROPERTYPAGE_H__28EB2835_C191_44A6_B8A5_C14EFA6EB251__INCLUDED_)
#define AFX_SETTINGMIREPROPERTYPAGE_H__28EB2835_C191_44A6_B8A5_C14EFA6EB251__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SettingMirePropertyPage.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CSettingMirePropertyPage dialog

class CSettingMirePropertyPage : public CPropertyPage
{
	DECLARE_DYNCREATE(CSettingMirePropertyPage)

// Construction
public:
	CSettingMirePropertyPage();
	~CSettingMirePropertyPage();

	CString m_strFileOld;

// Dialog Data
	//{{AFX_DATA(CSettingMirePropertyPage)
	enum { IDD = IDD_SETTINGS_MIRE };
	CString	m_strFile;
	//}}AFX_DATA


// Overrides
	// ClassWizard generate virtual function overrides
	//{{AFX_VIRTUAL(CSettingMirePropertyPage)
	public:
	virtual BOOL OnApply();
	virtual BOOL OnSetActive();
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	// Generated message map functions
	//{{AFX_MSG(CSettingMirePropertyPage)
	afx_msg void OnButtonFilemire();
	afx_msg void OnDefault();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_SETTINGMIREPROPERTYPAGE_H__28EB2835_C191_44A6_B8A5_C14EFA6EB251__INCLUDED_)
