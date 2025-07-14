
// NanoExpertCtrlDlg.cpp : fichier d'implémentation
//

#include "stdafx.h"
#include "NanoExpertCtrl.h"
#include "NanoExpertCtrlDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

#define INI_FILE_DEFAULT_READING_STRING_VALUE_ _T("§§###@@@###§§")

// CORE
#define DBGFLG_SAVEDATA		0x00000001
#define DBGFLG_TIMING		0x00000002
#define DBGFLG_SAVESLOPE	0x00000004

//GENERAL
#define DBG_SHOWDEBUG_LOG	0x00000001
#define DBG_SHOW_DISPLAY	0x00000002

// degauchy
#define DBG_DEGAUCHREC_NX	0x00000010
#define	DBG_DEGAUCHREC_NY	0X00000020

// preparedata
#define DBG_REC_MASK		0x00000010
#define DBG_REC_ERODE		0x00000020
#define DBG_REC_DILATE		0x00000040
#define DBG_REC_NX			0x00000080
#define	DBG_REC_NY			0X00000100
#define DBG_REC_FFTKILLER	0x00000200

// reconstruct
#define	DBG_REC_COEFMAP				0X00000010
#define DBG_REC_H					0X00000020
#define DBG_REC_VIGNETTE			0x00000040
#define DBG_SKIP_AFFINETRANSFORM	0x00000080
#define DBG_SKIP_BANDTHREAD			0x00000100
#define DBG_SKIP_QUADTHREAD			0x00000200	

// filter
#define DBG_REC_HFILTER		0x00000010
#define DBG_REC_H_IN		0x00000020

// generate results
#define	DBG_REC_FILTERLISS	0X00000010
#define DBG_GEN_REC_ERODE	0X00000020
#define DBG_REC_PV2			0x00000040
#define DBG_REC_PV10		0x00000080
#define DBG_SKIP_PV2		0x00000100
#define DBG_SKIP_PV10		0x00000200


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


// boîte de dialogue CNanoExpertCtrlDlg




CNanoExpertCtrlDlg::CNanoExpertCtrlDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CNanoExpertCtrlDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_hRegNanoKey = 0;
}

void CNanoExpertCtrlDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CNanoExpertCtrlDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_CORE_SAVEDATA, &CNanoExpertCtrlDlg::OnBnClickedCoreSavedata)
	ON_BN_CLICKED(IDC_CORE_TIMING, &CNanoExpertCtrlDlg::OnBnClickedCoreTiming)
	ON_BN_CLICKED(IDC_CORE_SAVESLOPE, &CNanoExpertCtrlDlg::OnBnClickedCoreSaveslope)
	ON_BN_CLICKED(IDC_ORDER_LOG, &CNanoExpertCtrlDlg::OnBnClickedOrderLog)
	ON_BN_CLICKED(IDC_ORDER_DISPLAY, &CNanoExpertCtrlDlg::OnBnClickedOrderDisplay)
	ON_BN_CLICKED(IDC_ORDER_NX, &CNanoExpertCtrlDlg::OnBnClickedOrderNx)
	ON_BN_CLICKED(IDC_ORDER_NY, &CNanoExpertCtrlDlg::OnBnClickedOrderNy)
	ON_BN_CLICKED(IDC_PREP_LOG, &CNanoExpertCtrlDlg::OnBnClickedPrepLog)
	ON_BN_CLICKED(IDC_PREP_DISPLAY, &CNanoExpertCtrlDlg::OnBnClickedPrepDisplay)
	ON_BN_CLICKED(IDC_PREP_MASK, &CNanoExpertCtrlDlg::OnBnClickedPrepMask)
	ON_BN_CLICKED(IDC_PREP_ERODE, &CNanoExpertCtrlDlg::OnBnClickedPrepErode)
	ON_BN_CLICKED(IDC_PREP_DILATE, &CNanoExpertCtrlDlg::OnBnClickedPrepDilate)
	ON_BN_CLICKED(IDC_PREP_NX, &CNanoExpertCtrlDlg::OnBnClickedPrepNx)
	ON_BN_CLICKED(IDC_PREP_NY, &CNanoExpertCtrlDlg::OnBnClickedPrepNy)
	ON_BN_CLICKED(IDC_PREP_FFTKILLER, &CNanoExpertCtrlDlg::OnBnClickedPrepFftkiller)
	ON_BN_CLICKED(IDC_RECONSTRUCT_LOG, &CNanoExpertCtrlDlg::OnBnClickedReconstructLog)
	ON_BN_CLICKED(IDC_RECONSTRUCT_DISPLAY, &CNanoExpertCtrlDlg::OnBnClickedReconstructDisplay)
	ON_BN_CLICKED(IDC_RECONSTRUCT_RECCOEFMAP, &CNanoExpertCtrlDlg::OnBnClickedReconstructReccoefmap)
	ON_BN_CLICKED(IDC_RECONSTRUCT_RECH, &CNanoExpertCtrlDlg::OnBnClickedReconstructRech)
	ON_BN_CLICKED(IDC_RECONSTRUCT_RECVIGNETTES, &CNanoExpertCtrlDlg::OnBnClickedReconstructRecvignettes)
	ON_BN_CLICKED(IDC_RECONSTRUCT_SKIPAFFINE, &CNanoExpertCtrlDlg::OnBnClickedReconstructSkipaffine)
	ON_BN_CLICKED(IDC_RECONSTRUCT_SKIPBANDTHREAD, &CNanoExpertCtrlDlg::OnBnClickedReconstructSkipbandthread)
	ON_BN_CLICKED(IDC_RECONSTRUCT_SKIPQUADTHREAD2, &CNanoExpertCtrlDlg::OnBnClickedReconstructSkipquadthread2)
	ON_BN_CLICKED(IDC_FILTER_LOG, &CNanoExpertCtrlDlg::OnBnClickedFilterLog)
	ON_BN_CLICKED(IDC_FILTER_DISPLAY, &CNanoExpertCtrlDlg::OnBnClickedFilterDisplay)
	ON_BN_CLICKED(IDC_FILTER_RECHIN, &CNanoExpertCtrlDlg::OnBnClickedFilterRechin)
	ON_BN_CLICKED(IDC_FILTER_RECHFILTER, &CNanoExpertCtrlDlg::OnBnClickedFilterRechfilter)
	ON_BN_CLICKED(IDC_GENRES_LOG, &CNanoExpertCtrlDlg::OnBnClickedGenresLog)
	ON_BN_CLICKED(IDC_GENRES_DISPLAY, &CNanoExpertCtrlDlg::OnBnClickedGenresDisplay)
	ON_BN_CLICKED(IDC_GENRES_FILTERLISS, &CNanoExpertCtrlDlg::OnBnClickedGenresFilterliss)
	ON_BN_CLICKED(IDC_GENRES_RECERODE, &CNanoExpertCtrlDlg::OnBnClickedGenresRecerode)
	ON_BN_CLICKED(IDC_GENRES_RECPV2, &CNanoExpertCtrlDlg::OnBnClickedGenresRecpv2)
	ON_BN_CLICKED(IDC_GENRES_RECPV10, &CNanoExpertCtrlDlg::OnBnClickedGenresRecpv10)
	ON_BN_CLICKED(IDC_GENRES_SKIPPV2, &CNanoExpertCtrlDlg::OnBnClickedGenresSkippv2)
	ON_BN_CLICKED(IDC_GENRES_SKIPPV10, &CNanoExpertCtrlDlg::OnBnClickedGenresSkippv10)
END_MESSAGE_MAP()


// gestionnaires de messages pour CNanoExpertCtrlDlg

BOOL CNanoExpertCtrlDlg::OnInitDialog()
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

	InitTreatName();

	// query for registry data if data doesn't exist value is created with default value 
	if (false == OpenRegistryNano())
		return FALSE;

	// core flag
	DWORD dwFlag = 0;
	dwFlag = GetReg(m_csCoreName);
	CButton* pChkBox = 0;
	if(dwFlag & DBGFLG_SAVEDATA)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_CORE_SAVEDATA);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBGFLG_TIMING)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_CORE_TIMING);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBGFLG_SAVESLOPE)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_CORE_SAVESLOPE);
		pChkBox->SetCheck(BST_CHECKED);
	}

	// degauchy flag (order)
	dwFlag = GetReg(m_csOrderName);
	if(dwFlag & DBG_SHOWDEBUG_LOG)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_ORDER_LOG);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SHOW_DISPLAY)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_ORDER_DISPLAY);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_DEGAUCHREC_NX)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_ORDER_NX);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_DEGAUCHREC_NY)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_ORDER_NY);
		pChkBox->SetCheck(BST_CHECKED);
	}

	// prep data
	dwFlag = GetReg(m_csPrepareDataName);
	if(dwFlag & DBG_SHOWDEBUG_LOG)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_PREP_LOG);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_MASK)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_PREP_MASK);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_ERODE)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_PREP_ERODE);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_DILATE)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_PREP_DILATE);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_NX)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_PREP_NX);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_NY)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_PREP_NY);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_FFTKILLER)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_PREP_FFTKILLER);
		pChkBox->SetCheck(BST_CHECKED);
	}

	// reconstruct data
	dwFlag = GetReg(m_csReconstructName);
	if(dwFlag & DBG_SHOWDEBUG_LOG)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_LOG);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SHOW_DISPLAY)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_DISPLAY);
		pChkBox->SetCheck(BST_CHECKED);
	}

	if(dwFlag & DBG_REC_COEFMAP)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_RECCOEFMAP);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_H)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_RECH);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_VIGNETTE)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_RECVIGNETTES);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SKIP_AFFINETRANSFORM)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_SKIPAFFINE);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SKIP_BANDTHREAD)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_SKIPBANDTHREAD);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SKIP_QUADTHREAD)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_SKIPQUADTHREAD2);
		pChkBox->SetCheck(BST_CHECKED);
	}

	// filter data
	dwFlag = GetReg(m_csFilterName);
	if(dwFlag & DBG_SHOWDEBUG_LOG)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_FILTER_LOG);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SHOW_DISPLAY)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_FILTER_DISPLAY);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_HFILTER)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_FILTER_RECHFILTER);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_H_IN)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_FILTER_RECHIN);
		pChkBox->SetCheck(BST_CHECKED);
	}

	// generate results
	dwFlag = GetReg(m_csGenResName);
	if(dwFlag & DBG_SHOWDEBUG_LOG)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_LOG);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SHOW_DISPLAY)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_DISPLAY);
		pChkBox->SetCheck(BST_CHECKED);
	}

	if(dwFlag & DBG_REC_FILTERLISS)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_FILTERLISS);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_GEN_REC_ERODE)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_RECERODE);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_PV2)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_RECPV2);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_REC_PV10)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_RECPV10);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SKIP_PV2)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_SKIPPV2);
		pChkBox->SetCheck(BST_CHECKED);
	}
	if(dwFlag & DBG_SKIP_PV10)
	{
		pChkBox = (CButton*)GetDlgItem(IDC_GENRES_SKIPPV10);
		pChkBox->SetCheck(BST_CHECKED);
	}

	return TRUE;  // retourne TRUE, sauf si vous avez défini le focus sur un contrôle
}

void CNanoExpertCtrlDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CNanoExpertCtrlDlg::OnPaint()
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
HCURSOR CNanoExpertCtrlDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

bool CNanoExpertCtrlDlg::OpenRegistryNano()
{
	HKEY hRegbase			= HKEY_CURRENT_USER;
	CString csNanoKey		= _T("Software\\Altatech\\Nanotopo");

	// OPen registry
	if (RegOpenKeyEx(hRegbase, csNanoKey, 0, KEY_ALL_ACCESS , &m_hRegNanoKey) == ERROR_SUCCESS) 
	{
		
	}
	else
	{
		//key does not exist create it and insert default value
		if (RegCreateKeyEx(hRegbase, csNanoKey, 0, _T(""), REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &m_hRegNanoKey, NULL) == ERROR_SUCCESS)
		{
			
		}
		else
		{
			AfxMessageBox("Could not create HKCU\\Software\\Altatech\\Nanotopo !");
			return false;
		}
	}
	return true;
}

void CNanoExpertCtrlDlg::CloseRegistry()
{
	if(m_hRegNanoKey)
		RegCloseKey(m_hRegNanoKey);
}

void CNanoExpertCtrlDlg::InitTreatName()
{
	 m_csCoreName			= _T("Core_Flag");
	 m_csOrderName			= GetTreatName(0);
	 m_csPrepareDataName	= GetTreatName(1);
	 m_csReconstructName	= GetTreatName(2);
	 m_csFilterName			= GetTreatName(3);
	 m_csGenResName			= GetTreatName(4);

}

CString CNanoExpertCtrlDlg::GetTreatName( int nID )
{
	const CString csIniFile = _T("C:\\Altasight\\Nano\\IniRep\\NanoTopo.ini");
	const CString csSection = _T("Treatments");
	CString csTreatKey;
	csTreatKey.Format(_T("T%d"),nID);
	CString csResultName;

	TCHAR szBuffer[255] = {0};
	GetPrivateProfileString (csSection, csTreatKey, INI_FILE_DEFAULT_READING_STRING_VALUE_, &szBuffer[0], 254, csIniFile);

	CString csVal = szBuffer;
	if(csVal == INI_FILE_DEFAULT_READING_STRING_VALUE_)
	{
		csResultName.Empty();
		return csResultName;
	}
	csResultName = szBuffer;

	// remove .local if exist
	if(csResultName.Right(6).CompareNoCase(_T(".local")) == 0)
	{
		csResultName = csResultName.Left(csResultName.GetLength() - 6);
	}

	// add _Flag
	csResultName += _T("_Flag");
	return csResultName;
}

void CNanoExpertCtrlDlg::OnClose()
{
	CloseRegistry();

	CDialogEx::OnClose();
}

void CNanoExpertCtrlDlg::SetReg(CString csTreatKey, int bValue, DWORD dwFlag)
{
	DWORD dwType = 0;
	DWORD dwData = 0;
	DWORD dwSizeData	= sizeof(DWORD);
	if (RegQueryValueEx(m_hRegNanoKey, csTreatKey, 0, &dwType, (BYTE*)(&dwData), &dwSizeData) != ERROR_SUCCESS)
	{	
		dwData = 0;
	}
	
	if (bValue)
	{
		dwData |= dwFlag;
	}
	else
	{
		dwData &= ~(dwFlag);
	}

	if (RegSetValueEx(m_hRegNanoKey, csTreatKey, 0, REG_DWORD, (BYTE *)(&dwData), dwSizeData) != ERROR_SUCCESS)
	{
		CString cs;
		cs.Format("Cannot access to Nano Registry Key : %s", csTreatKey);
		AfxMessageBox(cs);
	}
}

DWORD CNanoExpertCtrlDlg::GetReg(CString csTreatKey)
{
	 DWORD dwFlag = 0;
	 DWORD dwType = 0;
	 DWORD dwData = 0;
	 DWORD dwSizeData	= sizeof(DWORD);
	 if (RegQueryValueEx(m_hRegNanoKey, csTreatKey, 0, &dwType, (BYTE*)(&dwData), &dwSizeData) != ERROR_SUCCESS)
	 {	
		 if (RegSetValueEx(m_hRegNanoKey, csTreatKey, 0, REG_DWORD, (BYTE *)(&dwData), dwSizeData) != ERROR_SUCCESS)
		 {
			 CString cs;
			 cs.Format("Cannot access to Nano Registry Key : %s", csTreatKey);
			 AfxMessageBox(cs);
			 return 0;
		 }
	 }
	 return dwData;
}

void CNanoExpertCtrlDlg::OnBnClickedCoreSavedata()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_CORE_SAVEDATA);
	SetReg(m_csCoreName,pChkBox->GetCheck(), DBGFLG_SAVEDATA);
}


void CNanoExpertCtrlDlg::OnBnClickedCoreTiming()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_CORE_TIMING);
	SetReg(m_csCoreName,pChkBox->GetCheck(), DBGFLG_TIMING);
}


void CNanoExpertCtrlDlg::OnBnClickedCoreSaveslope()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_CORE_SAVESLOPE);
	SetReg(m_csCoreName,pChkBox->GetCheck(), DBGFLG_SAVESLOPE);
}


void CNanoExpertCtrlDlg::OnBnClickedOrderLog()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_ORDER_LOG);
	SetReg(m_csOrderName,pChkBox->GetCheck(), DBG_SHOWDEBUG_LOG);
}


void CNanoExpertCtrlDlg::OnBnClickedOrderDisplay()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_ORDER_DISPLAY);
	SetReg(m_csOrderName,pChkBox->GetCheck(), DBG_SHOW_DISPLAY);
}


void CNanoExpertCtrlDlg::OnBnClickedOrderNx()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_ORDER_NX);
	SetReg(m_csOrderName,pChkBox->GetCheck(), DBG_DEGAUCHREC_NX);
}


void CNanoExpertCtrlDlg::OnBnClickedOrderNy()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_ORDER_NY);
	SetReg(m_csOrderName,pChkBox->GetCheck(), DBG_DEGAUCHREC_NY);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepLog()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_LOG);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_SHOWDEBUG_LOG);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepDisplay()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_DISPLAY);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_SHOW_DISPLAY);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepMask()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_MASK);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_REC_MASK);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepErode()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_ERODE);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_REC_ERODE);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepDilate()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_DILATE);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_REC_DILATE);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepNx()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_NX);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_REC_NX);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepNy()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_NY);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_REC_NY);
}


void CNanoExpertCtrlDlg::OnBnClickedPrepFftkiller()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_PREP_FFTKILLER);
	SetReg(m_csPrepareDataName,pChkBox->GetCheck(), DBG_REC_FFTKILLER);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructLog()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_LOG);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_SHOWDEBUG_LOG);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructDisplay()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_DISPLAY);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_SHOW_DISPLAY);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructReccoefmap()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_RECCOEFMAP);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_REC_COEFMAP);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructRech()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_RECH);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_REC_H);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructRecvignettes()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_RECVIGNETTES);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_REC_VIGNETTE);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructSkipaffine()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_SKIPAFFINE);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_SKIP_AFFINETRANSFORM);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructSkipbandthread()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_SKIPBANDTHREAD);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_SKIP_BANDTHREAD);
}


void CNanoExpertCtrlDlg::OnBnClickedReconstructSkipquadthread2()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_RECONSTRUCT_SKIPQUADTHREAD2);
	SetReg(m_csReconstructName,pChkBox->GetCheck(), DBG_SKIP_QUADTHREAD);
}


void CNanoExpertCtrlDlg::OnBnClickedFilterLog()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_FILTER_LOG);
	SetReg(m_csFilterName,pChkBox->GetCheck(), DBG_SHOWDEBUG_LOG);
}


void CNanoExpertCtrlDlg::OnBnClickedFilterDisplay()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_FILTER_DISPLAY);
	SetReg(m_csFilterName,pChkBox->GetCheck(), DBG_SHOW_DISPLAY);
}


void CNanoExpertCtrlDlg::OnBnClickedFilterRechin()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_FILTER_RECHIN);
	SetReg(m_csFilterName,pChkBox->GetCheck(), DBG_REC_H_IN);
}


void CNanoExpertCtrlDlg::OnBnClickedFilterRechfilter()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_FILTER_RECHFILTER);
	SetReg(m_csFilterName,pChkBox->GetCheck(), DBG_REC_HFILTER);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresLog()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_LOG);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_SHOWDEBUG_LOG);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresDisplay()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_DISPLAY);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_SHOW_DISPLAY);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresFilterliss()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_FILTERLISS);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_REC_FILTERLISS);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresRecerode()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_RECERODE);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_GEN_REC_ERODE);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresRecpv2()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_RECPV2);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_REC_PV2);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresRecpv10()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_RECPV10);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_REC_PV10);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresSkippv2()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_SKIPPV2);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_SKIP_PV2);
}


void CNanoExpertCtrlDlg::OnBnClickedGenresSkippv10()
{
	CButton* pChkBox = 0;
	pChkBox = (CButton*)GetDlgItem(IDC_GENRES_SKIPPV10);
	SetReg(m_csGenResName,pChkBox->GetCheck(), DBG_SKIP_PV10);
}
