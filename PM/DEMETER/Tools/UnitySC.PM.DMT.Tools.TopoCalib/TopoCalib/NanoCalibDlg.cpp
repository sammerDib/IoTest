
// NanoCalibDlg.cpp : fichier d'implémentation
//

#include "stdafx.h"
#include "NanoCalib.h"
#include "NanoCalibDlg.h"
#include "afxdialogex.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


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


// boîte de dialogue CNanoCalibDlg




CNanoCalibDlg::CNanoCalibDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CNanoCalibDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
    m_isSilent = false;
}

void CNanoCalibDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_TAB1, m_ctlrTab);
}

BEGIN_MESSAGE_MAP(CNanoCalibDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
END_MESSAGE_MAP()


// gestionnaires de messages pour CNanoCalibDlg

BOOL CNanoCalibDlg::OnInitDialog()
{
    try
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

        int i = 0;
        if (!m_oDlgCalibCam.Create(IDD_DLG_CALIBCAM, &m_ctlrTab))
        {
            return FALSE;
        }
        m_ctlrTab.InsertItem(i, (CDialog*)&m_oDlgCalibCam, _T("1 - Camera"));
        m_ctlrTab.SetCurSel(i);
        i++;

        if (!m_oDlgCalibSys.Create(IDD_DLG_CALIBSYS, &m_ctlrTab))
        {
            return FALSE;
        }
        m_ctlrTab.InsertItem(i, (CDialog*)&m_oDlgCalibSys, _T("2 - System"));
        m_ctlrTab.SetCurSel(i);
        i++;

        if (!m_oDlgMesure.Create(IDD_DLG_MESURE, &m_ctlrTab))
        {
            return FALSE;
        }
        m_ctlrTab.InsertItem(i, (CDialog*)&m_oDlgMesure, _T("3 - Mesure"));
        m_ctlrTab.SetCurSel(i);
        i++;

        m_ctlrTab.SetCurSel(0);


        if (m_isSilent)
        {
            bool bSucceded = false;
            if (!m_clCalibCamSourceImagesFolder.IsEmpty())
            {
                bSucceded = DoSilentCamCalib();
                if (!bSucceded)
                {
                    ::PostQuitMessage(0);
                }
            }

            if (!m_clCalibSysSourceImagesFolder.IsEmpty())
            {
                bSucceded = DoSilentSysCalib();
                if (!bSucceded)
                {
                    ::PostQuitMessage(0);
                }
            }

            if (bSucceded)
                ::PostQuitMessage(1);
        }
        return TRUE;  // retourne TRUE, sauf si vous avez défini le focus sur un contrôle
    }
    catch (std::exception& ex)
    {
        AfxMessageBox(ex.what());
        EndDialog(-1);
    }
}

bool CNanoCalibDlg::DoSilentSysCalib()
{
    // We search the Tif files
    CFileFind finder;

    // build a string with wildcards
    CString strWildcard(m_clCalibSysSourceImagesFolder);
    strWildcard += _T("\\*.*");

    // start working for files
    BOOL bWorking = finder.FindFile(strWildcard);

    int phasisImageIdx = 0;
    while (bWorking)
    {
        bWorking = finder.FindNextFile();

        // skip . and .. files; otherwise, we'd
        // recur infinitely!
        if (finder.IsDots())
            continue;

        if (finder.IsDirectory())
            continue; 

        CString sFilePath= finder.GetFilePath();

        // If the fileName is Standard_Reflectivity.tiff then it is the reference image
        if (sFilePath.Find("Standard_Reflectivity.tiff") >= 0)
            m_oDlgCalibSys.m_csEditVideo = sFilePath;
        else
        {
            // It is a phase file
            ASSERT(phasisImageIdx < NB_IMG_PHASE);
            m_oDlgCalibSys.m_csEditAcq[phasisImageIdx++] = sFilePath;
        }
    }
    finder.Close();

    m_oDlgCalibSys.SortPhasesImages();

    // we set the parameters from the command line
    m_oDlgCalibSys.m_fPitchX = m_clCalibSysPitchX;
    m_oDlgCalibSys.m_fPitchY = m_clCalibSysPitchY;
    m_oDlgCalibSys.m_fPeriodX = m_clCalibSysPeriodX;
    m_oDlgCalibSys.m_fPeriodY = m_clCalibSysPeriodY;
    m_oDlgCalibSys.m_nScreenSizeX = m_clCalibSysScreenSizeX;
    m_oDlgCalibSys.m_nScreenSizeY = m_clCalibSysScreenSizeY;
    m_oDlgCalibSys.m_nScreenRefPosX = m_clCalibSysScreenRefPosX;
    m_oDlgCalibSys.m_nScreenRefPosY = m_clCalibSysScreenRefPosY;
    m_oDlgCalibSys.m_nCrossX = m_clCalibSysCrossX;
    m_oDlgCalibSys.m_nCrossY = m_clCalibSysCrossY;

    // Silent Execution
    m_oDlgCalibSys._silentMode = true;
    return (m_oDlgCalibSys.DoCalibration());
}

bool CNanoCalibDlg::DoSilentCamCalib()
{
    // We search the Tif files
    CFileFind finder;

    // build a string with wildcards
    CString strWildcard(m_clCalibCamSourceImagesFolder);
    strWildcard += _T("\\*.tif");

    // start working for files
    BOOL bWorking = finder.FindFile(strWildcard);
    int fileIndex = 0;
    while (bWorking)
    {
        bWorking = finder.FindNextFile();

        // skip . and .. files; otherwise, we'd
        // recur infinitely!

        if (finder.IsDots())
            continue;

        m_oDlgCalibCam.m_csEditAcq[fileIndex] = finder.GetFilePath();
        fileIndex++;
    }

    // We set the nuber of images
    m_oDlgCalibCam.m_nbImages = fileIndex;

    finder.Close();

    m_oDlgCalibCam.m_nSizeX = m_clCalibCamMireSizeX;
    m_oDlgCalibCam.m_nSizeY = m_clCalibCamMireSizeY;
    m_oDlgCalibCam.m_fStepX = m_clCalibCamMireStepX;
    m_oDlgCalibCam.m_fStepY = m_clCalibCamMireStepY;

    // Silent Execution
    return (m_oDlgCalibCam.DoCalibration());
}

void CNanoCalibDlg::OnSysCommand(UINT nID, LPARAM lParam)
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

void CNanoCalibDlg::OnPaint()
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
HCURSOR CNanoCalibDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

