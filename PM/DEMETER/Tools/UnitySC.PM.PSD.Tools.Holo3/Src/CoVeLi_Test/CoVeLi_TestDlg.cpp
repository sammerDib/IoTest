
// CoVeLi_TestDlg.cpp : fichier d'implémentation
//

#include "stdafx.h"
#include "CoVeLi_Test.h"
#include "CoVeLi_TestDlg.h"

#include "H3array2d.h"
#include "H3AppToolsDecl.h"
#include "H3_HoloMap_AltaTypeExport.h"
#include "CameraCalibInfoClass.h"
#include "SystemCalibInfoClass.h"
#include "MeasureInfoClass.h"
#include <tuple>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
// freeimage
//
#include "FreeImagePlus.h"
#ifdef _DEBUG
#pragma comment (lib , "FreeImaged")
#pragma comment (lib , "FreeImagePlusd")
#else
#pragma comment (lib , "FreeImage")
#pragma comment (lib , "FreeImagePlus")
#endif

typedef unsigned char uchar;
inline int  cvRound(double value)
{
#if (defined _MSC_VER && defined _M_X64) || (defined __GNUC__ && defined __x86_64__ && defined __SSE2__ && !defined __APPLE__)
    __m128d t = _mm_set_sd(value);
    return _mm_cvtsd_si32(t);
#elif defined _MSC_VER && defined _M_IX86
    int t;
    __asm
    {
        fld value;
        fistp t;
    }
    return t;
#elif defined HAVE_LRINT || defined CV_ICC || defined __GNUC__
#  ifdef HAVE_TEGRA_OPTIMIZATION
    TEGRA_ROUND(value);
#  else
    return (int)lrint(value);
#  endif
#else
    // while this is not IEEE754-compliant rounding, it's usually a good enough approximation
    return (int)(value + (value >= 0 ? 0.5 : -0.5));
#endif
}
template<typename _Tp> static inline _Tp saturate_cast(float v) { return _Tp(v); }
template<> inline uchar saturate_cast<uchar>(float v)
{
    int iv = cvRound(v); return (uchar)((unsigned)iv <= UCHAR_MAX ? iv : iv > 0 ? UCHAR_MAX : 0);
}

static bool SaveGreyImageFlt32(CString p_csFilepath, H3_ARRAY2D_FLT32& p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true)
{
    float* pData = p_oMatrixFloat.GetData();

    size_t  lCols = p_oMatrixFloat.GetCo();
    size_t  lLines = p_oMatrixFloat.GetLi();

    float fMin = FLT_MAX;
    float fMax = -FLT_MAX;

    bool bUseMinPrm = (p_fMin != FLT_MAX);
    bool bUseMaxPrm = (p_fMax != FLT_MAX);

    float a = 1.0f;
    float b = 0.0f;
    if (bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
    {
        for (long lItem = 0; lItem < p_oMatrixFloat.GetSize(); lItem++)
        {
            if (!_isnan(pData[lItem]))
            {
                if (!bUseMinPrm)
                    fMin = __min(fMin, pData[lItem]);
                if (!bUseMaxPrm)
                    fMax = __max(fMax, pData[lItem]);
            }
        }
    }
    else
    {
        if (!bUseMaxPrm)
            fMax = 255.0f;
        if (!bUseMinPrm)
            fMin = 0.0f;
    }

    if (bUseMinPrm)
    {
        fMin = p_fMin;
    }
    if (bUseMaxPrm)
    {
        fMax = p_fMax;
    }

    a = 255.0f / (fMax - fMin);
    b = -fMin * 255.0f / (fMax - fMin);

    fipImage oImg(FIT_BITMAP, (int)lCols, (int)lLines, 8);
    for (unsigned y = 0; y < oImg.getHeight(); y++)
    {
        //ici pb à resoudre pour affichage image
        BYTE* pbits = (BYTE*)oImg.getScanLine(y);
        for (unsigned x = 0; x < oImg.getWidth(); x++)
        {
            pbits[x] = saturate_cast<uchar>(pData[y * lCols + x] * a + b);
        }
    }
    oImg.flipVertical();

    CString SPth = p_csFilepath;
    SPth.Format("%s_%0.3lf_%0.3lf.png", p_csFilepath.Left(p_csFilepath.GetLength() - 4), fMax, fMin);
    BOOL bRes = oImg.save(SPth, 0);
    return (bRes != 0);
}


// boîte de dialogue CAboutDlg utilisée pour la boîte de dialogue 'À propos de' pour votre application
class CAboutDlg : public CDialog
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

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()

// boîte de dialogue CCoVeLi_TestDlg
CCoVeLi_TestDlg::CCoVeLi_TestDlg(CWnd* pParent)
    : CDialog(CCoVeLi_TestDlg::IDD, pParent)
{
    m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CCoVeLi_TestDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CCoVeLi_TestDlg, CDialog)
    ON_WM_SYSCOMMAND()
    ON_WM_PAINT()
    ON_WM_QUERYDRAGICON()
    //}}AFX_MSG_MAP
    ON_BN_CLICKED(IDC_MESURE, &CCoVeLi_TestDlg::OnBnClickedMesureZ0)
    //	ON_BN_CLICKED(IDC_CALIB_EXT_P, &CCoVeLi_TestDlg::OnBnClickedCalibExtP)
    ON_BN_CLICKED(IDC_CALIB_SYS, &CCoVeLi_TestDlg::OnBnClickedCalibSys)
    ON_BN_CLICKED(IDC_BUTTON_INIT, &CCoVeLi_TestDlg::OnBnClickedButtonInit)
    ON_BN_CLICKED(IDC_CALIB_CAM, &CCoVeLi_TestDlg::OnBnClickedCalibCam)
    ON_BN_CLICKED(IDC_MESURE_Z, &CCoVeLi_TestDlg::OnBnClickedMesureZ)
END_MESSAGE_MAP()


// gestionnaires de messages pour CCoVeLi_TestDlg
BOOL CCoVeLi_TestDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    // Ajouter l'élément de menu "À propos de..." au menu Système.

    // IDM_ABOUTBOX doit se trouver dans la plage des commandes système.
    ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
    ASSERT(IDM_ABOUTBOX < 0xF000);

    CMenu* pSysMenu = GetSystemMenu(FALSE);
    if (pSysMenu != nullptr)
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

    // TODO : ajoutez ici une initialisation supplémentaire

    return TRUE;  // retourne TRUE, sauf si vous avez défini le focus sur un contrôle
}

void CCoVeLi_TestDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
    if ((nID & 0xFFF0) == IDM_ABOUTBOX)
    {
        CAboutDlg dlgAbout;
        dlgAbout.DoModal();
    }
    else
    {
        CDialog::OnSysCommand(nID, lParam);
    }
}

// Si vous ajoutez un bouton Réduire à votre boîte de dialogue, vous devez utiliser le code ci-dessous
//  pour dessiner l'icône. Pour les applications MFC utilisant le modèle Document/Vue,
//  cela est fait automatiquement par l'infrastructure.

void CCoVeLi_TestDlg::OnPaint()
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
        CDialog::OnPaint();
    }
}

CString CCoVeLi_TestDlg::m_CalibFolder = _CalibPaths._LastCalibPath;

// Le système appelle cette fonction pour obtenir le curseur à afficher lorsque l'utilisateur fait glisser
//  la fenêtre réduite.
HCURSOR CCoVeLi_TestDlg::OnQueryDragIcon()
{
    return static_cast<HCURSOR>(m_hIcon);
}

void CCoVeLi_TestDlg::OnBnClickedCalibCam()
{
    CString str;

    // Initialisation des paramètres pour le calibrage caméra
    CString strCalibCam = m_CalibFolder + "\\" + _CalibPaths._InputSettingsFile;

    CString strSection;
    strSection = _T("CH3CameraCalib_CH3Mire");

    int nMireSizeX = H3GetPrivProfileInt(strSection, _T("nLi"), strCalibCam);
    int nMireSizeY = H3GetPrivProfileInt(strSection, _T("nCo"), strCalibCam);
    float fMireStepX = H3GetPrivProfileFloat(strSection, _T("StepX"), strCalibCam);
    float fMireStepY = H3GetPrivProfileFloat(strSection, _T("StepY"), strCalibCam);
    size_t   nNbVImg = 6L;
    //

    // Chargement des images vidéos MIRE pour 6 positions différentes
    CH3Array<H3_ARRAY2D_UINT8> vMirePos(nNbVImg);

    vMirePos[0].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\flipper_Ref.hbf");
    vMirePos[1].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\flipper_Ref+10.hbf");
    vMirePos[2].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\flipper_Ref-10.hbf");
    vMirePos[3].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\potence+10.hbf");
    vMirePos[4].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\potence-10.hbf");
    vMirePos[5].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\translation-50mm.hbf");
    //

    // Initialisation structure de données
    CImageByte* pArrayOfVPicture = new CImageByte[nNbVImg];
    for (size_t i = 0; i < nNbVImg; i++)
    {
        pArrayOfVPicture[i].nLi = vMirePos[i].GetLi();
        pArrayOfVPicture[i].nCo = vMirePos[i].GetCo();

        pArrayOfVPicture[i].pData = new BYTE[pArrayOfVPicture[i].nLi * pArrayOfVPicture[i].nCo];

        for (size_t j = 0; j < pArrayOfVPicture[i].nLi * pArrayOfVPicture[i].nCo; j++)
        {
            pArrayOfVPicture[i].pData[j] = vMirePos[i][j];
        }
    }
    //

    // Calibrage caméra
    CCameraCalibInfoClass	CameraCalib(m_CalibFolder);
    CameraCalib.SetData(nMireSizeX, nMireSizeY, fMireStepX, fMireStepY, nNbVImg, pArrayOfVPicture);

    int nAppreciationCalibCamera;
    CalibrageCamera(&CameraCalib, nAppreciationCalibCamera);

    CString strMsg;
    strMsg = _T("Les causes possibles sont:\n- Un choc subi par la mire.\n- Une image floue.\n- De la lumière parasite.");
    switch (nAppreciationCalibCamera)
    {
    case 0:
        AfxMessageBox("Le calibrage caméra a réussi.");
        break;
    case 1:
        AfxMessageBox("Les erreurs de reprojection suivant l'axe X sont supérieures\nà la valeur maximale autorisée soit 0.2.\n\n" + strMsg);
        break;
    case 2:
        AfxMessageBox("Les erreurs de reprojection suivant l'axe Y sont supérieures\nà la valeur maximale autorisée soit 0.2.\n\n" + strMsg);
        break;
    case 3:
        AfxMessageBox("Le calibrage n'a pas convergé. Veuillez contacter Holo3.");
        break;
    default:
        break;
    }
    //

    // Libérer mémoire
    if (pArrayOfVPicture != nullptr)
    {
        for (size_t i = 0; i < nNbVImg; i++)
        {
            if (pArrayOfVPicture[i].pData != nullptr)
            {
                delete[] pArrayOfVPicture[i].pData;
                pArrayOfVPicture[i].pData = nullptr;
            }
        }
        delete[] pArrayOfVPicture;
        pArrayOfVPicture = nullptr;
    }
}

void CCoVeLi_TestDlg::OnBnClickedCalibSys()
{
    CString strSection;
    CString str;

    CWaitCursor wait;

    // Initialisation des paramètres pour le calibrage système
    CString strCalibSystem = m_CalibFolder + "\\" + _CalibPaths._InputSettingsFile;

    strSection = _T("SensorHoloMap3_SensorScreen");
    //dimension des pixels en mm
    float fPixSizeX = H3GetPrivProfileFloat(strSection, _T("PitchX"), strCalibSystem);
    float fPixSizeY = H3GetPrivProfileFloat(strSection, _T("PitchY"), strCalibSystem);

    //periode du reseau fin lors du calibrage (en pixels)
    float fMireMonStepX = H3GetPrivProfileFloat(strSection, _T("PeriodX"), strCalibSystem);
    float fMireMonStepY = H3GetPrivProfileFloat(strSection, _T("PeriodY"), strCalibSystem);

    //position du point reference sur l'ecran (pixel)
    unsigned long pixRef_Xscreen = H3GetPrivProfileInt(strSection, _T("pixRef_Xscreen"), strCalibSystem);
    unsigned long pixRef_Yscreen = H3GetPrivProfileInt(strSection, _T("pixRef_Yscreen"), strCalibSystem);

    //dimension de l'ecran( pixel)
    // IBE To do: ne devrait être lu dans le fichier SensorData qu'en standalone; dans Demeter il faut aller chercher l'info dans HardwareConfig.xml.
    unsigned long screen_Xsz = H3GetPrivProfileInt(strSection, _T("screen_Xsz"), strCalibSystem);
    unsigned long screen_Ysz = H3GetPrivProfileInt(strSection, _T("screen_Ysz"), strCalibSystem);


    strSection = _T("SensorHoloMap3");

    //position du point reference dans l'image
    unsigned int nCrossX = H3GetPrivProfileInt(strSection, _T("PrefX"), strCalibSystem);
    unsigned int nCrossY = H3GetPrivProfileInt(strSection, _T("PrefY"), strCalibSystem);

    // Chargement des images de phase (W) X1 Y1 X2 Y2 X3 Y3 du wafer reference
    size_t   nNbWImg = 6L;
    CH3Array<H3_ARRAY2D_FLT32> wPhase(nNbWImg);

    wPhase[0].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_2_X.hbf");
    wPhase[1].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_2_Y.hbf");
    wPhase[2].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_3_X.hbf");
    wPhase[3].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_3_Y.hbf");
    wPhase[4].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_4_X.hbf");
    wPhase[5].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_4_Y.hbf");
    //

    // Chargement de l'image vidéo du wafer reference
    H3_ARRAY2D_UINT8 vImage;
    CString FName_videoWaferRef(_T("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\flipper_Ref.hbf"));
    //CString FName_videoWaferRef= H3GetPrivProfileInt(strSection,_T("RefMirror_File"),_T(".hbf"),strCalibSystem);

    vImage.Load(FName_videoWaferRef);
    //

    // Initialisation structure de données
    CImageFloat* pArrayOfWPicture = new CImageFloat[nNbWImg];
    for (size_t i = 0; i < nNbWImg; i++)
    {
        pArrayOfWPicture[i].nLi = wPhase[i].GetLi();
        pArrayOfWPicture[i].nCo = wPhase[i].GetCo();

        pArrayOfWPicture[i].pData = new float[pArrayOfWPicture[i].nLi * pArrayOfWPicture[i].nCo];

        for (size_t j = 0; j < pArrayOfWPicture[i].nLi * pArrayOfWPicture[i].nCo; j++)
        {
            pArrayOfWPicture[i].pData[j] = wPhase[i][j];
        }
    }

    CImageByte mireImage;
    mireImage.nLi = vImage.GetLi();
    mireImage.nCo = vImage.GetCo();
    mireImage.pData = new BYTE[mireImage.nLi * mireImage.nCo];
    for (size_t i = 0; i < mireImage.nLi * mireImage.nCo; i++)
    {
        mireImage.pData[i] = vImage[i];
    }

    vImage.~CH3Array2D();
    wPhase.~CH3Array();

    // Calibrage système
    CSystemCalibInfoClass	SystemCalib(m_CalibFolder);
    float ratio[3] = { 1,16,16 };
    SystemCalib.SetData(pArrayOfWPicture, &mireImage,
        fPixSizeX, fPixSizeY,
        fMireMonStepX, fMireMonStepY,
        pixRef_Xscreen, pixRef_Yscreen,
        screen_Xsz, screen_Ysz,
        nCrossX, nCrossY,
        nNbWImg, ratio);

    int nAppreciationCalibSystem;
    SystemCalib.CalibrageSystem(nAppreciationCalibSystem);

    CString strMsg;
    strMsg = _T("");
    switch (nAppreciationCalibSystem)
    {
    case 0:
        strMsg = CString(_T("Le calibrage système a réussi."));
        AfxMessageBox(strMsg);
        break;
    case 1:
        strMsg = CString(_T("Le chemin d'accès au fichier CalibCam_0.txt n'est pas renseigné dans le fichier SensorData.txt.\n"));
        strMsg += CString(_T("Voir section [SensorHoloMap3] paramètre str_CamFile"));
        AfxMessageBox(strMsg);
        break;
    case 2:
        strMsg = CString(_T("Le chemin d'accès au fichier EP_ref_CamFrame.txt n'est pas renseigné dans le fichier SensorData.txt.\n"));
        strMsg += CString(_T("Voir section [SensorHoloMap3] paramètre ep_ObjRef_CamFrame_File"));
        AfxMessageBox(strMsg);
        break;
    case 21:
        strMsg = CString(_T("Le chemin d'accès au fichier Res1.txt n'est pas renseigné dans le fichier SensorData.txt.\n"));
        strMsg += CString(_T("Voir section [SensorHoloMap3] paramètre str_epFile"));
        AfxMessageBox(strMsg);
        break;
    case 22:
        strMsg = CString(_T("Le chemin d'accès au fichier ResX.klib n'est pas renseigné dans le fichier SensorData.txt.\n"));
        strMsg += CString(_T("Voir section [SensorHoloMap3] paramètre str_PhiXFile"));
        AfxMessageBox(strMsg);
        break;
    case 23:
        strMsg = CString(_T("Le chemin d'accès au fichier ResY.klib n'est pas renseigné dans le fichier SensorData.txt.\n"));
        strMsg += CString(_T("Voir section [SensorHoloMap3] paramètre str_PhiYFile"));
        AfxMessageBox(strMsg);
        break;
    case 3:
        strMsg = CString(_T("Le fichier d'initialisation de la caméra est incomplet."));
        AfxMessageBox(strMsg);
        break;
    case 4:
        strMsg = CString(_T("Les paramètres extrinsèques du wafer dans le repère caméra sont invalides."));
        AfxMessageBox(strMsg);
        break;
    case 5:
        strMsg = CString(_T("Au moins un des paramètres d'entrées est invalide."));
        AfxMessageBox(strMsg);
        break;
    case 6:
        strMsg = CString(_T("Il n'y a pas assez de pixels valides sur les images de phases."));
        AfxMessageBox(strMsg);
        break;
    case 7:
        strMsg = CString(_T("Les cartes de phases en X ou Y acquisent sur le wafer de référence sont trop bruitées."));
        AfxMessageBox(strMsg);
        break;
    case 8:
        strMsg = CString(_T("Le dossier Calib_Sys situé dans C:\\altasight\\Nano\\ n'existe pas."));
        AfxMessageBox(strMsg);
        break;
    case 9:
        strMsg = CString(_T("Une erreur s'est produite lors du calibrage système."));
        AfxMessageBox(strMsg);
        break;
    case 10:
        strMsg = CString(_T("Impossible de charger le fichier SensorData.txt se trouvant dans le dossier c:\\altasight\\Nano\\."));
        AfxMessageBox(strMsg);
        break;
    case 11:
        strMsg = CString(_T("L'entrée 'str_CamFile' dans le fichier 'C:\\AltaSight\\Nano\\SensorData.txt' n'est pas renseignée!...\n"));
        strMsg += CString(_T("str_CamFile=C:\\AltaSight\\Globaltopo\\Calib_cam\\CalibCam_0.txt"));
        AfxMessageBox(strMsg);
        break;
    case 12:
        strMsg = CString(_T("L'entrée 'ep_ObjRef_CamFrame_File' dans le fichier 'C:\\AltaSight\\Nano\\SensorData.txt' n'est pas renseignée!...\n"));
        strMsg += CString(_T("ep_ObjRef_CamFrame_File=C:\\AltaSight\\NGlobaltopoano\\Calib_cam\\EP_ref_CamFrame.txt"));
        AfxMessageBox(strMsg);
        break;
    case 13:
        strMsg = CString(_T("Annulation de la procédure de calibrage système par l'opérateur!..."));
        AfxMessageBox(strMsg);
        break;
    case 14:
        strMsg = CString(_T("Le fichier 'C:\\AltaSight\\Globaltopo\\SensorData.txt' ou 'C:\\AltaSight\\Globaltopo\\Calib_cam\\EP_ref_CamFrame.txt' n'existe pas!..."));
        AfxMessageBox(strMsg);
        break;
    case 15:
        strMsg = CString(_T("Erreur lors de la récupération des paramètres extrinsèques de la mire!..."));
        AfxMessageBox(strMsg);
        break;
    default:
        break;
    }
    //

    // Libérer mémoire
    if (pArrayOfWPicture != nullptr)
    {
        for (size_t i = 0; i < nNbWImg; i++)
        {
            if (pArrayOfWPicture[i].pData != nullptr)
            {
                delete[] pArrayOfWPicture[i].pData;
                pArrayOfWPicture[i].pData = nullptr;
            }
        }
        delete[] pArrayOfWPicture;
        pArrayOfWPicture = nullptr;
    }
    if (mireImage.pData != nullptr)
    {
        delete[] mireImage.pData;
        mireImage.pData = nullptr;
    }
    //
}

void CCoVeLi_TestDlg::OnBnClickedMesureZ0()
{
    CString str;

    CWaitCursor wait;

    // Position du point remarquable
    unsigned int nCrossX = 0;
    unsigned int nCrossY = 0;

    // Chargement des images de phases modulees (W) wx et wy
    size_t nNbWImg = 2L;
    CH3Array<H3_ARRAY2D_FLT32> wPhase(nNbWImg);
    wPhase[0].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_2_X.hbf");
    wPhase[1].Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_2_Y.hbf");

    // Chargement de l'image masque wmask
    H3_ARRAY2D_UINT8 mMask;
    mMask.Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Mask_2.hbf");

    // Initialisation structure de données
    CImageFloat** pArrayOfWPicture = new CImageFloat * [nNbWImg];
    for (size_t i = 0; i < nNbWImg; i++)
    {
        pArrayOfWPicture[i] = new CImageFloat();
        pArrayOfWPicture[i]->nLi = wPhase[i].GetLi();
        pArrayOfWPicture[i]->nCo = wPhase[i].GetCo();

        pArrayOfWPicture[i]->pData = new float[pArrayOfWPicture[i]->nLi * pArrayOfWPicture[i]->nCo];

        for (size_t j = 0; j < pArrayOfWPicture[i]->nLi * pArrayOfWPicture[i]->nCo; j++)
        {
            pArrayOfWPicture[i]->pData[j] = wPhase[i][j];
        }
    }
    CImageByte maskImage;
    maskImage.nLi = mMask.GetLi();
    maskImage.nCo = mMask.GetCo();
    maskImage.pData = new BYTE[maskImage.nLi * maskImage.nCo];
    for (size_t i = 0; i < maskImage.nLi * maskImage.nCo; i++)
    {
        maskImage.pData[i] = mMask[i];
    }
    //

    // Mesurer
    float ratio = 1;
    CMeasureInfoClass	measure;
    measure.SetData(nCrossX, nCrossY, pArrayOfWPicture, &maskImage, ratio);

    int nAppreciationMeasure;
    Mesurer(&measure, false,  false, nAppreciationMeasure);

    CString strMsg;
    strMsg = _T("");
    switch (nAppreciationMeasure)
    {
    case 0:
    {
        strMsg = CString(_T("La mesure a réussi."));

        // Resultats
        const size_t nLi = maskImage.nLi, nCo = maskImage.nCo, nSz = nLi * nCo;
        const size_t nNbImages = 5L;

        CImageFloat* pArrayOfPicture = new CImageFloat[nNbImages];
        for (size_t i = 0; i < nNbImages; i++)
        {
            pArrayOfPicture[i].pData = new float[nLi * nCo];
        }

        measure.GetData(pArrayOfPicture);

        H3_ARRAY2D_FLT32 PX(nLi, nCo);
        H3_ARRAY2D_FLT32 PY(nLi, nCo);
        H3_ARRAY2D_FLT32 NX(nLi, nCo);
        H3_ARRAY2D_FLT32 NY(nLi, nCo);
        H3_ARRAY2D_FLT32 NZ(nLi, nCo);

        PX.Fill(NaN);  PY.Fill(NaN);
        NX.Fill(NaN);  NY.Fill(NaN); NZ.Fill(NaN);

        for (size_t i = 0; i < nLi * nCo; i++)
        {
            if (maskImage.pData[i])
            {
                NX[i] = pArrayOfPicture[0].pData[i];
                NY[i] = pArrayOfPicture[1].pData[i];
                NZ[i] = pArrayOfPicture[2].pData[i];
                PX[i] = pArrayOfPicture[3].pData[i];
                PY[i] = pArrayOfPicture[4].pData[i];
            }
        }

        NX.Save("c:\\altasight\\Nano\\data_test\\ResMesZ0\\NX.hbf");
        NY.Save("c:\\altasight\\Nano\\data_test\\ResMesZ0\\NY.hbf");
        NZ.Save("c:\\altasight\\Nano\\data_test\\ResMesZ0\\NZ.hbf");
        PX.Save("c:\\altasight\\Nano\\data_test\\ResMesZ0\\PX.hbf");
        PY.Save("c:\\altasight\\Nano\\data_test\\ResMesZ0\\PY.hbf");

        // Libérer mémoire
        if (pArrayOfPicture != nullptr)
        {
            for (size_t i = 0; i < nNbImages; i++)
            {
                if (pArrayOfPicture[i].pData != nullptr)
                {
                    delete[] pArrayOfPicture[i].pData;
                    pArrayOfPicture[i].pData = nullptr;
                }
            }
            delete[] pArrayOfPicture;
            pArrayOfPicture = nullptr;
        }
        //

        AfxMessageBox(strMsg);
    }
    break;
    case 1:
        strMsg = CString(_T("Le capteur n'est pas alloué!."));
        AfxMessageBox(strMsg);
        break;
    case 2:
        strMsg = CString(_T("Les données sont de tailles distinctes!."));
        AfxMessageBox(strMsg);
        break;
    case 3:
        AfxMessageBox("La cartographie de mesure est vide");
        break;
    default:
        break;
    }
    //

    // Libérer mémoire
    if (pArrayOfWPicture != nullptr)
    {
        for (size_t i = 0; i < nNbWImg; i++)
        {
            delete pArrayOfWPicture[i];
        }
        delete[] pArrayOfWPicture;
        pArrayOfWPicture = nullptr;
    }

    if (maskImage.pData != nullptr)
    {
        delete[] maskImage.pData;
        maskImage.pData = nullptr;
    }
    //
}

void CCoVeLi_TestDlg::OnBnClickedMesureZ()
{
    CString str;

    CWaitCursor wait;

    // Position du point remarquable
    unsigned int nCrossX = 1500;
    unsigned int nCrossY = 1500;

    // Chargement des images de phases modulees (W) wx et wy de la mesure du wafer
    /*H3_ARRAY2D_FLT32 wPhaseX;
    H3_ARRAY2D_FLT32 wPhaseY;

    //toutes les images de phase ont la meme taille
    wPhaseX.Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_2_X.hbf");
    wPhaseY.Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Picture_2_Y.hbf");

    // Chargement de l'image masque wmask
    /*H3_ARRAY2D_UINT8 mMask;
    mMask.Load("C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\Mask_2.hbf");

    //le point remarquable doit au moins etre dans le mask
    if(0==mMask(nCrossY,nCrossX))
    {
        str = _T("Le point connue n'est pas dans le masque.");
        AfxMessageBox(str);
        return;
    }*/

    // Initialisation structure de données
    /*const size_t nLi= wPhaseX.GetLi(), nCo= wPhaseX.GetCo();
    CImageFloat* pArrayOfWPicture = new CImageFloat[2];
    for(size_t i=0; i<2; i++)
    {
        pArrayOfWPicture[i].nLi = nLi;
        pArrayOfWPicture[i].nCo = nCo;

        pArrayOfWPicture[i].pData = new float[nLi*nCo];
    }
    for (size_t j=0; j< nLi*nCo; j++)
    {
        pArrayOfWPicture[0].pData[j] = wPhaseX[j];
        pArrayOfWPicture[1].pData[j] = wPhaseY[j];
    }*/

    /*CImageByte maskImage;
    maskImage.nLi = mMask.GetLi();
    maskImage.nCo = mMask.GetCo();
    maskImage.pData = new BYTE[maskImage.nLi*maskImage.nCo];
    for (size_t i=0; i<maskImage.nLi*maskImage.nCo; i++)
    {
        maskImage.pData[i] = mMask[i];
    }*/


    CImageFloat** pArrayOfWPicture = new CImageFloat * [2];
    //CString sPhaseBinPath_X = "D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\Manual_PSD_TEST_Phase_X.bin";
    //CString sPhaseBinPath_Y = "D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\Manual_PSD_TEST_Phase_Y.bin";
    //CString sMaskBinPath	= "D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\Manual_PSD_TEST_Mask.bin";

    CString sPhaseBinPath_X = "D:\\Altasight\\pourVincentv1\\ImagesCalibv1\\phase16\\Manual_PSD_TEST_Phase_X.bin";
    CString sPhaseBinPath_Y = "D:\\Altasight\\pourVincentv1\\ImagesCalibv1\\phase16\\Manual_PSD_TEST_Phase_Y.bin";
    CString sMaskBinPath = "D:\\Altasight\\pourVincentv1\\ImagesCalibv1\\phase16\\Manual_PSD_TEST_Mask.bin";

    pArrayOfWPicture[0] = new CImageFloat();
    if (pArrayOfWPicture[0]->Load(sPhaseBinPath_X) == false)
    {
        CString csMsg;
        csMsg.Format(_T("Could not load Phase X Image : {%s}\n Wrong Path or file extension"), sPhaseBinPath_X);
        AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        delete pArrayOfWPicture[0];
    }
    pArrayOfWPicture[1] = new CImageFloat();
    if (pArrayOfWPicture[1]->Load(sPhaseBinPath_X) == false)
    {
        CString csMsg;
        csMsg.Format(_T("Could not load Phase X Image : {%s}\n Wrong Path or file extension"), sPhaseBinPath_Y);
        AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        delete pArrayOfWPicture[1];
    }

    CImageByte maskImage;
    if (maskImage.Load(sMaskBinPath) == false)
    {
        CString csMsg;
        csMsg.Format(_T("Could not load Mask Image : {%s}\n Wrong Path or file extension"), sMaskBinPath);
        AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
    }

    //

    // Mesurer
    //nCrossX=1572;
    //nCrossY=1866;

    const float ratio = 1;				//ratio periode reseau mesure /periode ref

    tuple<int, int> pixel_imageDuPointDAltitudeConnue(530, 2310);
    const float altitude = 0.0;			//altitude du point vu sur le pixel nCrossX, nCrossY


    //tuple<int, int> pixel_imageDuPointDAltitudeConnue(530,2310);
    //const float altitude=0.1;			//altitude du point vu sur le pixel nCrossX, nCrossY

    //tuple<int, int> pixel_imageDuPointDAltitudeConnue(460,590);
    //const float altitude=200.0;			//altitude du point vu sur le pixel nCrossX, nCrossY

    tuple<int, int>  pixel_ref_inPicture(1572, 1866);

    const bool  b_mesure_shape = true;	//oui pour mesurer le 'vrai' relief (plutot qu'une approximation 'quasi plan')
    CMeasureInfoClass	measure;
    measure.SetData(nCrossX, nCrossY, pArrayOfWPicture, &maskImage, ratio,
        pixel_imageDuPointDAltitudeConnue, altitude,
        pixel_ref_inPicture,
        b_mesure_shape);

    int nAppreciationMeasure;
    Mesurer(&measure, false, false, nAppreciationMeasure);

    CString strMsg;
    strMsg = _T("");
    switch (nAppreciationMeasure)
    {
    case 0:
    {
        strMsg = CString(_T("La mesure a réussi."));

        // Resultats
        const size_t nLi = maskImage.nLi, nCo = maskImage.nCo, nSz = nLi * nCo;
        const size_t nNbImages = 6L;

        CImageFloat* pArrayOfPicture = new CImageFloat[nNbImages];
        for (size_t i = 0; i < nNbImages; i++)
        {
            pArrayOfPicture[i].pData = new float[nLi * nCo];
        }

        measure.GetData(pArrayOfPicture, nNbImages);

        H3_ARRAY2D_FLT32  X(nLi, nCo);
        H3_ARRAY2D_FLT32  Y(nLi, nCo);
        H3_ARRAY2D_FLT32  Z(nLi, nCo);
        H3_ARRAY2D_FLT32 NX(nLi, nCo);
        H3_ARRAY2D_FLT32 NY(nLi, nCo);
        H3_ARRAY2D_FLT32 NZ(nLi, nCo);

        X.Fill(NaN);   Y.Fill(NaN);  Z.Fill(NaN);
        NX.Fill(NaN);  NY.Fill(NaN); NZ.Fill(NaN);

        for (size_t i = 0, imax = nLi * nCo; i < imax; i++)
        {
            if (maskImage.pData[i])
            {
                NX[i] = pArrayOfPicture[0].pData[i];
                NY[i] = pArrayOfPicture[1].pData[i];
                NZ[i] = pArrayOfPicture[2].pData[i];
                X[i] = pArrayOfPicture[3].pData[i];
                Y[i] = pArrayOfPicture[4].pData[i];
                Z[i] = pArrayOfPicture[5].pData[i];
            }
        }

        NX.Save("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\NX.hbf");
        NY.Save("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\NY.hbf");
        NZ.Save("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\NZ.hbf");
        X.Save("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\ X.hbf");
        Y.Save("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\ Y.hbf");
        Z.Save("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\ Z.hbf");

        SaveGreyImageFlt32("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\NX.png", NX);
        SaveGreyImageFlt32("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\NY.png", NY);
        SaveGreyImageFlt32("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\NZ.png", NZ);
        SaveGreyImageFlt32("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\X.png", X);
        SaveGreyImageFlt32("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\Y.png", Y);
        SaveGreyImageFlt32("D:\\Altasight\\pourVincentv1\\ImagesTest\\wafer\\testCoveli\\Z.png", Z);

        // Libérer mémoire
        if (pArrayOfPicture != nullptr)
        {
            for (size_t i = 0; i < nNbImages; i++)
            {
                if (pArrayOfPicture[i].pData != nullptr)
                {
                    delete[] pArrayOfPicture[i].pData;
                    pArrayOfPicture[i].pData = nullptr;
                }
            }
            delete[] pArrayOfPicture;
            pArrayOfPicture = nullptr;
        }
        //

        AfxMessageBox(strMsg);
    }
    break;
    case 1:
        strMsg = CString(_T("Le capteur n'est pas alloué!."));
        AfxMessageBox(strMsg);
        break;
    case 2:
        strMsg = CString(_T("Les données sont de tailles distinctes!."));
        AfxMessageBox(strMsg);
        break;
    case 3:
        AfxMessageBox("La cartographie de mesure est vide");
        break;
    default:
        break;
    }
    //

    // Libérer mémoire
    if (pArrayOfWPicture != nullptr)
    {
        for (size_t i = 0; i < 2; i++)
        {
            delete pArrayOfWPicture[i];
        }
        delete[] pArrayOfWPicture;
        pArrayOfWPicture = nullptr;
    }

    if (maskImage.pData != nullptr)
    {
        delete[] maskImage.pData;
        maskImage.pData = nullptr;
    }
    //
}


void CCoVeLi_TestDlg::OnBnClickedButtonInit()
{
    // TODO : ajoutez ici le code de votre gestionnaire de notification de contrôle
    H3_InitSys(m_CalibFolder);
    AfxMessageBox("H3_InitSys DONE !!!");
}

