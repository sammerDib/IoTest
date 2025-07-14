// CalibrageDlg.cpp : implementation file
//

#include "stdafx.h"
#include "H3DSLRCalibrationIHMrc.h"
#include "CalibrageDlg.h"

#include "CFolderDialog.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CCalibrageDlg dialog


CCalibrageDlg::CCalibrageDlg(stParamDLGCalibrageTotal *pParam/*=nullptr*/, CWnd* pParent /*=nullptr*/)
	: CDialog(CCalibrageDlg::IDD, pParent)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	//{{AFX_DATA_INIT(CCalibrageDlg)
	m_Param.m_strPassWord = "1234";
	m_Param.m_bCalibrerCamera0 = true;
	m_Param.m_strCFGFile ="";
	m_Param.m_strSaveImagePathName = "";
	m_Param.m_nImagesCount = 12;
	m_Param.m_bSaveRepport = false;
	//}}AFX_DATA_INIT
	if (pParam)
	{
		m_Param = *pParam;
	}

}


void CCalibrageDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CCalibrageDlg)
	DDX_Check(pDX, IDC_CAMERA0, m_Param.m_bCalibrerCamera0);
	DDX_Text(pDX, IDC_CFGFILE, m_Param.m_strCFGFile);
	DDX_Text(pDX, IDC_NIMAGES, m_Param.m_nImagesCount);
	DDX_Text(pDX, IDC_PASSWORD, m_Param.m_strPassWord);
	DDX_Text(pDX, IDC_SAVEPATH, m_Param.m_strSaveImagePathName);
	DDX_Check(pDX, IDC_SAVEREPPORT, m_Param.m_bSaveRepport);
	//}}AFX_DATA_MAP

	bool bEnable = true;
	GetDlgItem(IDOK)->EnableWindow(bEnable);
}


BEGIN_MESSAGE_MAP(CCalibrageDlg, CDialog)
	//{{AFX_MSG_MAP(CCalibrageDlg)
	ON_BN_CLICKED(IDC_BROWSECFG, OnBrowseCFGFileName)
	ON_BN_CLICKED(IDC_BROWSEIMAGES, OnBrowseImagesPathName)
	ON_BN_CLICKED(IDC_BTNPASSWORD, OnBtnPassWord)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCalibrageDlg message handlers


void CCalibrageDlg::OnBtnPassWord() 
{
	UpdateData(true);
}
void CCalibrageDlg::OnBrowseCFGFileName() 
{
	UpdateData(true);
	CFileDialog dlg(false,"cfg",NULL,
		OFN_EXPLORER | OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
		"Fichiers de configuration et calibrage (*.cfg)|*.cfg|"\
		"Tous les Fichiers (*.*)|*.*|");

	if (dlg.DoModal() != IDOK)
	{
		return;
	}

	CString strNewName = dlg.GetPathName();
	m_Param.m_strCFGFile = strNewName;
	UpdateData(false);
}

void CCalibrageDlg::OnBrowseImagesPathName() 
{
	UpdateData(true);
	CFolderDialog dlg(false, NULL, FOLDER_FILTER, OFN_HIDEREADONLY, _T("Sélectionner le répertoire de sauvegarde"));

	dlg.m_ofn.lpstrInitialDir = m_Param.m_strSaveImagePathName;
	if (dlg.DoModal() != IDOK)
	{
		return;
	}
	CString strRep = dlg.m_pPath;
	m_Param.m_strSaveImagePathName = strRep;
	UpdateData(false);
}

void CCalibrageDlg::OnOK() 
{
	UpdateData(true);
	
	CDialog::OnOK();
}
