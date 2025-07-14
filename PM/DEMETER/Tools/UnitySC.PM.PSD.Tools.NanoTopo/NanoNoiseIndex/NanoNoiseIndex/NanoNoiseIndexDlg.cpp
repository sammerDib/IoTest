
// NanoNoiseIndexDlg.cpp : fichier d'implémentation
//
#include <stdio.h>
#include "stdafx.h"
#include "NanoNoiseIndex.h"
#include "NanoNoiseIndexDlg.h"
#include "afxdialogex.h"
#include "math.h"
#include  <memory>
#include <iostream>
#include <Eigen/Dense>
#include <Eigen/LU>
#include <fstream>
#include <iostream>
#include <string>
#include "CProfil.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

using namespace Eigen;
using namespace std;

// boîte de dialogue CAboutDlg utilisée pour la boîte de dialogue 'À propos de' pour votre application

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// Données de boîte de dialogue
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // Prise en charge de DDX/DDV

// Implémentation
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// boîte de dialogue CNanoNoiseIndexDlg

CNanoNoiseIndexDlg::CNanoNoiseIndexDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CNanoNoiseIndexDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_csEditFile = _T("");
	m_nFilterFrameSize = 400;
	m_nFilterOrder = 2;
	m_dNoiseIndex = 0.0;
	m_dSeuilPenalite = 1000.0;

}//D:\\devHC\\data\\LotID-518_20130301_153724_Profile.csv

void CNanoNoiseIndexDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT_CSVPATH, m_csEditFile);
	DDX_Text(pDX, IDC_EDIT_FRAMESZ, m_nFilterFrameSize);
	DDX_Text(pDX, IDC_EDIT_NOISEIDX, m_dNoiseIndex);
	DDX_Text(pDX, IDC_EDIT_ORDER, m_nFilterOrder);
}

BEGIN_MESSAGE_MAP(CNanoNoiseIndexDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_BUTTON_CALCUL, &CNanoNoiseIndexDlg::OnBnClickedButtonCalcul)
	ON_BN_CLICKED(IDC_BUTTON_BROWSE, &CNanoNoiseIndexDlg::OnBnClickedButtonBrowse)
	ON_WM_HELPINFO()
END_MESSAGE_MAP()


// gestionnaires de messages pour CNanoNoiseIndexDlg

BOOL CNanoNoiseIndexDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// Ajouter l'élément de menu "À propos de..." au menu Système.

	// IDM_ABOUTBOX doit se trouver dans la plage des commandes système.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Définir l'icône de cette boîte de dialogue. L'infrastructure effectue cela automatiquement
	//  lorsque la fenêtre principale de l'application n'est pas une boîte de dialogue
	SetIcon(m_hIcon, TRUE);			// Définir une grande icône
	SetIcon(m_hIcon, FALSE);		// Définir une petite icône


	// INIT FROM REGISTRY
	m_csEditFile = GetReg_CString(_T("CSVFile"),_T("C:\\Altasight"));
	m_nFilterFrameSize = GetReg_int(_T("FilterFrameSize"), 200);
	m_nFilterOrder =  GetReg_int(_T("FilterOrder"), 2);
	m_dSeuilPenalite = (double) GetReg_float(_T("PenaSeuil"),100.0);
	m_dNoiseIndex = 0.0;
	bool bDbg = (GetReg_int(_T("Dbg"), 0) != 0);
	CProfil::SetDbgMode(bDbg);
	UpdateData(FALSE);

	return TRUE;  // retourne TRUE, sauf si vous avez défini le focus sur un contrôle
}

void CNanoNoiseIndexDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// Si vous ajoutez un bouton Réduire à votre boîte de dialogue, vous devez utiliser le code ci-dessous
//  pour dessiner l'icône. Pour les applications MFC utilisant le modèle Document/Vue,
//  cela est fait automatiquement par l'infrastructure.

void CNanoNoiseIndexDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // contexte de périphérique pour la peinture

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// Centrer l'icône dans le rectangle client
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Dessiner l'icône
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

// Le système appelle cette fonction pour obtenir le curseur à afficher lorsque l'utilisateur fait glisser
//  la fenêtre réduite.
HCURSOR CNanoNoiseIndexDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CNanoNoiseIndexDlg::OnBnClickedButtonCalcul()
{
	CWaitCursor oWait;
	UpdateData(TRUE);

	CProfil x; 
	m_dSeuilPenalite = (double) GetReg_float(_T("PenaSeuil"),10.0);
	m_dNoiseIndex = x.ComputeNoiseIndex((LPCTSTR)m_csEditFile, m_nFilterFrameSize, m_nFilterOrder, m_dSeuilPenalite);	

	UpdateData(FALSE);

	return;
}

void CNanoNoiseIndexDlg::BrowseCSV()
{
	UpdateData(TRUE);

	std::auto_ptr<CFileDialog> dlgFileDialog(new CFileDialog(TRUE, NULL, NULL, OFN_FILEMUSTEXIST| OFN_HIDEREADONLY,
		"CSV Files (*.csv)|*.csv;|All Files (*.*)|*.*||"));
	dlgFileDialog->m_ofn.lpstrTitle = "cross section data selection";
	CString csPath = m_csEditFile;
	PathRemoveFileSpec(csPath.GetBuffer());
	csPath.ReleaseBuffer();
	dlgFileDialog->m_ofn.lpstrInitialDir = csPath;

	if (dlgFileDialog->DoModal()==IDOK)
	{
		csPath = dlgFileDialog->GetPathName();
		m_csEditFile = csPath;
		m_dNoiseIndex = 0.0;
		UpdateData(FALSE);
	}
}

void CNanoNoiseIndexDlg::OnBnClickedButtonBrowse()
{
	BrowseCSV();

// 	string path = "D:\\devHC\\data\\log.txt";
// 	FILE * pfile;
// 	errno_t err = fopen_s(&pfile,path.c_str(),"w+");
// 	fprintf (pfile, "%s\n",m_csEditFile);  
// 	fclose(pfile);
}


void CNanoNoiseIndexDlg::OnOK()
{
	//CDialogEx::OnOK();
}


void CNanoNoiseIndexDlg::OnCancel()
{
	CDialogEx::OnCancel();
}


BOOL CNanoNoiseIndexDlg::OnHelpInfo(HELPINFO* pHelpInfo)
{
	return FALSE;
}

bool CNanoNoiseIndexDlg::OpenRegistry()
{
	HKEY hRegbase			= HKEY_CURRENT_USER;
	CString csRegKey		= _T("Software\\Altatech\\NanoNoiseIndex");

	// OPen registry
	if (RegOpenKeyEx(hRegbase, csRegKey, 0, KEY_ALL_ACCESS , &m_hRegKey) == ERROR_SUCCESS) 
	{

	}
	else
	{
		//key does not exist create it and insert default value
		if (RegCreateKeyEx(hRegbase, csRegKey, 0, _T(""), REG_OPTION_NON_VOLATILE, KEY_WRITE | KEY_READ, NULL, &m_hRegKey, NULL) == ERROR_SUCCESS)
		{

		}
		else
		{
			AfxMessageBox(Fmt("FATAL ERROR : Could not createRegistry key  HKCU\\%s !", csRegKey));
			return false;
		}
	}
	return true;
}

void CNanoNoiseIndexDlg::CloseRegistry()
{
	if(m_hRegKey)
	{
		SetReg_CString( _T("CSVFile"),m_csEditFile);
		SetReg_int(_T("FilterFrameSize"), m_nFilterFrameSize);
		SetReg_int(_T("FilterOrder"), m_nFilterOrder);
		
		RegCloseKey(m_hRegKey);
		m_hRegKey = 0;
	}
}

CString CNanoNoiseIndexDlg::GetReg_CString(CString p_csKeyName, CString p_csDefault)
{
	CString csRes = p_csDefault;
	if(m_hRegKey)
	{
		DWORD l_dwData = 0;
		DWORD l_dwType = 0;
		if (RegQueryValueEx(m_hRegKey, p_csKeyName, 0, &l_dwType, 0, &l_dwData) != ERROR_SUCCESS)
		{
			TCHAR pcValeur[512] = {0};
			_tcscpy(pcValeur, p_csDefault);
			if (RegSetValueEx(m_hRegKey, p_csKeyName, 0, REG_SZ, (BYTE *)(pcValeur), (lstrlen(pcValeur) + 1) * sizeof(TCHAR)) != ERROR_SUCCESS)
			{
				CString cs;
				cs.Format("Cannot access to ADCCalibTool Registry Key : %s", p_csKeyName);
				AfxMessageBox(cs);
			}
		}
		else
		{
			ASSERT(l_dwType == REG_SZ);
			if (RegQueryValueEx(m_hRegKey, p_csKeyName, 0, &l_dwType, (BYTE*)(csRes.GetBuffer(l_dwData)), &l_dwData) != ERROR_SUCCESS)
			{
				csRes.ReleaseBuffer();
				CString cs;
				cs.Format("Cannot Read ADCCalibTool Registry Key : %s", p_csKeyName);
				AfxMessageBox(cs);
				csRes = p_csDefault;
			}
			csRes.ReleaseBuffer();
		}
	}
	return csRes;
}

int		CNanoNoiseIndexDlg::GetReg_int(CString p_csKeyName, int p_nDefault)
{
	int nres = p_nDefault;
	CString csVal = GetReg_CString(p_csKeyName, Fmt(_T("%d"),nres));
	nres = _ttoi(csVal);
	return nres;
}

float	CNanoNoiseIndexDlg::GetReg_float(CString p_csKeyName, float p_fDefault)
{
	float fres = p_fDefault;
	CString csVal = GetReg_CString(p_csKeyName, Fmt(_T("%lf"),fres));
	fres = _ttof(csVal);
	return fres;
}

void	CNanoNoiseIndexDlg::SetReg_CString(CString p_csKeyName, CString p_csVal)
{
	if(m_hRegKey)
	{
		TCHAR pcValeur[512] = {0};
		_tcscpy(pcValeur, p_csVal);
		if (RegSetValueEx(m_hRegKey, p_csKeyName, 0, REG_SZ, (BYTE *)(pcValeur), (lstrlen(pcValeur) + 1) * sizeof(TCHAR)) != ERROR_SUCCESS)
		{
			AfxMessageBox(Fmt(_T("Error : Registry could not write key {%s} = %s"), (LPCTSTR) p_csKeyName, pcValeur));
		}
	}
	else
	{
		AfxMessageBox("Error : Registry has not been properly opened !");
	}
}

void	CNanoNoiseIndexDlg::SetReg_int(CString p_csKeyName, int p_nVal)
{
	SetReg_CString(p_csKeyName, Fmt(_T("%d"),p_nVal));
}

void	CNanoNoiseIndexDlg::SetReg_float(CString p_csKeyName, float p_fVal)
{
	SetReg_CString(p_csKeyName, Fmt(_T("%lf"),p_fVal));
}
