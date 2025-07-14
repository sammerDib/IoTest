// SettingMirePropertyPage.cpp : implementation file
//

#include "stdafx.h"
#include "H3DSLRCalibrationIHMrc.h"
#include "SettingMirePropertyPage.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CSettingMirePropertyPage property page

IMPLEMENT_DYNCREATE(CSettingMirePropertyPage, CPropertyPage)

CSettingMirePropertyPage::CSettingMirePropertyPage() : CPropertyPage(CSettingMirePropertyPage::IDD)
{
	//{{AFX_DATA_INIT(CSettingMirePropertyPage)
	m_strFile = _T("");
	//}}AFX_DATA_INIT
}

CSettingMirePropertyPage::~CSettingMirePropertyPage()
{
}

void CSettingMirePropertyPage::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSettingMirePropertyPage)
	DDX_Text(pDX, IDC_EDIT_FILEMIRE, m_strFile);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CSettingMirePropertyPage, CPropertyPage)
	//{{AFX_MSG_MAP(CSettingMirePropertyPage)
	ON_BN_CLICKED(IDC_BUTTON_FILEMIRE, OnButtonFilemire)
	ON_BN_CLICKED(IDC_DEFAULT, OnDefault)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSettingMirePropertyPage message handlers

void CSettingMirePropertyPage::OnButtonFilemire() 
{
	// TODO: Add your control notification handler code here
	CFileDialog FileOpenDlg(
		TRUE,"txt",NULL,
		OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
		"Fichiers TXT (Fichier de définition de mire) (*.txt)|*.txt||");
	if (FileOpenDlg.DoModal()==IDOK)
	{
		m_strFile=FileOpenDlg.GetPathName();
		UpdateData(FALSE);
	}
}


void CSettingMirePropertyPage::OnDefault() 
{
	// TODO: Add your control notification handler code here
	m_strFile=m_strFileOld;
	UpdateData(FALSE);

}

BOOL CSettingMirePropertyPage::OnApply() 
{
	// TODO: Add your specialized code here and/or call the base class
	UpdateData(TRUE);

	return CPropertyPage::OnApply();
}

BOOL CSettingMirePropertyPage::OnInitDialog() 
{
	CPropertyPage::OnInitDialog();
	
	// TODO: Add extra initialization here
	m_strFile=m_strFileOld;
	UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL CSettingMirePropertyPage::OnSetActive() 
{
	// TODO: Add your specialized code here and/or call the base class
	
	return CPropertyPage::OnSetActive();
}
