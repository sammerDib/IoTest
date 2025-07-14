// DlgCalibCam.cpp : fichier d'implémentation
//

#include "stdafx.h"
#include "NanoCalib.h"
#include "DlgCalibCam.h"
#include "afxdialogex.h"

#include "H3AppToolsDecl.h"
#include "CameraCalibInfoClass.h"
#include "Shlwapi.h"

#include "FreeImagePlus.h"
#ifdef _DEBUG
#pragma comment (lib , "FreeImaged")
#pragma comment (lib , "FreeImagePlusd")
#else
#pragma comment (lib , "FreeImage")
#pragma comment (lib , "FreeImagePlus")
#endif

// Initialisation des paramètres pour le calibrage caméra
const CString g_strCalibCamFile = CString(_CalibPaths._LastCalibPath) + _T("\\") + _CalibPaths._InputSettingsFile;


// Boîte de dialogue CDlgCalibCam

IMPLEMENT_DYNAMIC(CDlgCalibCam, CDialog)

CDlgCalibCam::CDlgCalibCam(CWnd* pParent /*=NULL*/)
    : CDialog(CDlgCalibCam::IDD, pParent)
{
    for (int i = 0; i < m_nbImages; i++)
    {
        m_csEditAcq[i] = _T("");
    }

    m_nSizeX = 0;
    m_nSizeY = 0;
    m_fStepX = 0.0f;
    m_fStepY = 0.0f;
}

CDlgCalibCam::~CDlgCalibCam()
{
}

void CDlgCalibCam::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_EDIT_SIZE_X, m_ctrlEditSizeX);
    DDX_Text(pDX, IDC_EDIT_SIZE_X, m_nSizeX);
    DDV_MinMaxInt(pDX, m_nSizeX, 0, 999999);
    DDX_Control(pDX, IDC_EDIT_SIZE_Y, m_ctrlEditSizeY);
    DDX_Text(pDX, IDC_EDIT_SIZE_Y, m_nSizeY);
    DDV_MinMaxInt(pDX, m_nSizeX, 0, 999999);
    DDX_Control(pDX, IDC_EDIT_STEP_X, m_ctrlEditStepX);
    DDX_Text(pDX, IDC_EDIT_STEP_X, m_fStepX);
    DDX_Control(pDX, IDC_EDIT_STEP_Y, m_ctrlEditStepY);
    DDX_Text(pDX, IDC_EDIT_STEP_Y, m_fStepY);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ1, m_ctrlBtnBrsw[0]);
    DDX_Control(pDX, IDC_EDIT_ACQ1, m_ctrlEditAcq[0]);
    DDX_Text(pDX, IDC_EDIT_ACQ1, m_csEditAcq[0]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ2, m_ctrlBtnBrsw[1]);
    DDX_Control(pDX, IDC_EDIT_ACQ2, m_ctrlEditAcq[1]);
    DDX_Text(pDX, IDC_EDIT_ACQ2, m_csEditAcq[1]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ3, m_ctrlBtnBrsw[2]);
    DDX_Control(pDX, IDC_EDIT_ACQ3, m_ctrlEditAcq[2]);
    DDX_Text(pDX, IDC_EDIT_ACQ3, m_csEditAcq[2]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ4, m_ctrlBtnBrsw[3]);
    DDX_Control(pDX, IDC_EDIT_ACQ4, m_ctrlEditAcq[3]);
    DDX_Text(pDX, IDC_EDIT_ACQ4, m_csEditAcq[3]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ5, m_ctrlBtnBrsw[4]);
    DDX_Control(pDX, IDC_EDIT_ACQ5, m_ctrlEditAcq[4]);
    DDX_Text(pDX, IDC_EDIT_ACQ5, m_csEditAcq[4]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ6, m_ctrlBtnBrsw[5]);
    DDX_Control(pDX, IDC_EDIT_ACQ6, m_ctrlEditAcq[5]);
    DDX_Text(pDX, IDC_EDIT_ACQ6, m_csEditAcq[5]);

    DDX_Control(pDX, IDC_BUTTON_CALIBCAM, m_ctrlBtnCalibCam);
}


BEGIN_MESSAGE_MAP(CDlgCalibCam, CDialog)
    ON_BN_CLICKED(IDC_BUTTON_CALIBCAM, &CDlgCalibCam::OnBnClickedButtonCalibcam)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ1, &CDlgCalibCam::OnBnClickedButtonBrswAcq1)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ2, &CDlgCalibCam::OnBnClickedButtonBrswAcq2)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ3, &CDlgCalibCam::OnBnClickedButtonBrswAcq3)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ4, &CDlgCalibCam::OnBnClickedButtonBrswAcq4)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ5, &CDlgCalibCam::OnBnClickedButtonBrswAcq5)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ6, &CDlgCalibCam::OnBnClickedButtonBrswAcq6)
    ON_BN_CLICKED(IDC_BUTTON_SAVEPRM, &CDlgCalibCam::OnBnClickedButtonSaveprm)
    ON_WM_MOVE()
END_MESSAGE_MAP()


// Gestionnaires de messages de CDlgCalibCam


void CDlgCalibCam::OnBnClickedButtonCalibcam()
{
    CWaitCursor oWaitCursor;

    UpdateData(TRUE);

    EnabledUI(FALSE);

    DoCalibration();


}

bool CDlgCalibCam::DoCalibration()
{



    CString str;

    int nMireSizeX = m_nSizeX;
    int nMireSizeY = m_nSizeY;
    float fMireStepX = m_fStepX;
    float fMireStepY = m_fStepY;

    // Initialisation structure de données
    CImageByte* pArrayOfVPicture = new CImageByte[NB_IMG_POS];
    fipImage oImg[NB_IMG_POS];
    for (size_t i = 0; i < m_nbImages; i++)
    {
        // Chargement des images vidéos MIRE pour 6 positions différentes

        if (!oImg[i].load(m_csEditAcq[i]))
        {
            CString csMsg;
            csMsg.Format(_T("Could not load Acquisition Image n°%d : {%s}\n Wrong Path or file extension"), i + 1, m_csEditAcq[i]);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
            EnabledUI(TRUE);
            return false;
        }

        pArrayOfVPicture[i].CopyFrom(&(oImg[i]));

        //pArrayOfVPicture[i].nLi = oImg[i].getHeight();
        //pArrayOfVPicture[i].nCo = oImg[i].getWidth();
        //pArrayOfVPicture[i].pData = new BYTE[pArrayOfVPicture[i].nLi * pArrayOfVPicture[i].nCo];

        //oImg[i].flipVertical();

        //// cette methode comporte une faille dans le cas ou la scanwidth est superieur à la width.
        ////memcpy(pArrayOfVPicture[i].pData,oImg[i].accessPixels(),pArrayOfVPicture[i].nLi*pArrayOfVPicture[i].nCo*sizeof(BYTE)); // cette methode comporte une faille dans le cas ou la scanwidth est superieur à la width

        //// cette methode copy ligne à ligne selon la taille de la width en ne negligeant pas le pitch!!
        //BYTE* pRawLine = 0;
        //BYTE* pCur = 0;
        //int nHeight = oImg[i].getHeight();
        //int nWidth = oImg[i].getWidth();
        //int nPitch = oImg[i].getScanWidth();
        //for (int l = nHeight - 1; l >= 0; l--)
        //{
        //    pRawLine = (BYTE*)(pArrayOfVPicture[i].pData + (l * nWidth));
        //    pCur = oImg[i].getScanLine(l);
        //    memcpy(pRawLine, pCur, nWidth * sizeof(BYTE)); // Copie d'une ligne
        //}

        // pour convertir image en hbf
        /*H3_ARRAY2D_UINT8 oHbf;
        oHbf.LinkData(pArrayOfVPicture[i].nLi,pArrayOfVPicture[i].nCo,pArrayOfVPicture[i].pData);
        CString sNewName = m_csEditAcq[i];
        PathRenameExtension(sNewName.GetBuffer(),_T(".hbf"));
        sNewName.ReleaseBuffer();
        oHbf.Save(sNewName);*/
    }

    // Calibrage caméra
    CCameraCalibInfoClass	CameraCalib(_CalibPaths._LastCalibPath);
    CameraCalib.SetData(nMireSizeX, nMireSizeY, fMireStepX, fMireStepY, m_nbImages, pArrayOfVPicture);

    int nAppreciationCalibCamera = -1;
    CalibrageCamera(&CameraCalib, nAppreciationCalibCamera);

    CString strMsg;
    //strMsg = _T("Les causes possibles sont:\n- Un choc subi par la mire.\n- Une image floue.\n- De la lumière parasite.");
    strMsg = _T("The possible causes are :\n- a choc has been received by the mire.\n- a blurry image.\n- some parasitic light.");

    switch (nAppreciationCalibCamera)
    {
        // 	case 0:
        // 		AfxMessageBox("Camera Calibration is SUCCESSFUL.");
        // 		break;
    case 1:
        //	AfxMessageBox("Les erreurs de reprojection suivant l'axe X sont supérieures\nà la valeur maximale autorisée soit 0.2.\n\n"+strMsg);
        AfxMessageBox("Reprojection errors following X-axis are higher than\n maximal authorized value, i.e. 0,2.\n\n" + strMsg);
        break;
    case 2:
        //	AfxMessageBox("Les erreurs de reprojection suivant l'axe Y sont supérieures\nà la valeur maximale autorisée soit 0.2.\n\n"+strMsg);
        AfxMessageBox("Reprojection errors following Y-axis are higher than\n maximal authorized value, i.e. 0,2.\n\n" + strMsg);
        break;
    case 3:
        //	AfxMessageBox("Le calibrage n'a pas convergé. Veuillez contacter Holo3.");
        AfxMessageBox("Calibration did not converged. Please contact Holo3.");
        break;
    default:
        break;
    }

    // Libérer mémoire
    if (pArrayOfVPicture != nullptr)
    {
        for (size_t i = 0; i < m_nbImages; i++)
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

    EnabledUI(TRUE);

    if (nAppreciationCalibCamera == 0)
    {
        // Save Target/Mire params.
        SaveSettings();
        return true;
    }
    return false;
}

void CDlgCalibCam::EnabledUI(BOOL p_bEnable)
{
    m_ctrlEditSizeX.EnableWindow(p_bEnable);
    m_ctrlEditSizeY.EnableWindow(p_bEnable);
    m_ctrlEditStepX.EnableWindow(p_bEnable);
    m_ctrlEditStepY.EnableWindow(p_bEnable);

    for (int i = 0; i < m_nbImages; i++)
    {
        m_ctrlEditAcq[i].EnableWindow(p_bEnable);
        m_ctrlBtnBrsw[i].EnableWindow(p_bEnable);
    }

    m_ctrlBtnCalibCam.EnableWindow(p_bEnable);
}

BOOL CDlgCalibCam::OnInitDialog()
{
    CDialog::OnInitDialog();

    CString strSection = _T("CH3CameraCalib_CH3Mire");

    m_nSizeX = H3GetPrivProfileInt(strSection, _T("nLi"), g_strCalibCamFile);
    m_nSizeY = H3GetPrivProfileInt(strSection, _T("nCo"), g_strCalibCamFile);
    m_fStepX = H3GetPrivProfileFloat(strSection, _T("StepX"), g_strCalibCamFile);
    m_fStepY = H3GetPrivProfileFloat(strSection, _T("StepY"), g_strCalibCamFile);

    for (int i = 0; i < m_nbImages; i++)
    {
        CString csImgAcqKey;
        csImgAcqKey.Format(_T("PathImgAcq%d"), i + 1);
        //CString csDefaultPath;
        //csDefaultPath.Format(_T("%s\\Altatec_Images_CalibCamera\\pos%d.png"),g_csAltasightBaseDirPath,i+1);
        m_csEditAcq[i] = H3GetPrivProfileString(strSection, csImgAcqKey, g_strCalibCamFile);
    }

    return TRUE;  // return TRUE unless you set the focus to a control
    // EXCEPTION : les pages de propriétés OCX devraient retourner FALSE
}


void CDlgCalibCam::OnOK()
{
    // Do nothing to avoid exit panel
    //CDialog::OnOK();
}


void CDlgCalibCam::OnCancel()
{
    // Do nothing to avoid exit panel
    //CDialog::OnCancel();
}


void CDlgCalibCam::WinHelp(DWORD dwData, UINT nCmd)
{
    // Do nothing to avoid F1 mess
    //CDialog::WinHelp(dwData, nCmd);
}


void CDlgCalibCam::OnBnClickedButtonBrswAcq1()
{
    BrowseAcqImg(0);
}
void CDlgCalibCam::OnBnClickedButtonBrswAcq2()
{
    BrowseAcqImg(1);
}

void CDlgCalibCam::OnBnClickedButtonBrswAcq3()
{
    BrowseAcqImg(2);
}

void CDlgCalibCam::OnBnClickedButtonBrswAcq4()
{
    BrowseAcqImg(3);
}

void CDlgCalibCam::OnBnClickedButtonBrswAcq5()
{
    BrowseAcqImg(4);
}

void CDlgCalibCam::OnBnClickedButtonBrswAcq6()
{
    BrowseAcqImg(5);
}


void CDlgCalibCam::BrowseAcqImg(UINT p_nId)
{
    UpdateData(TRUE);

    std::auto_ptr<CFileDialog> dlgFileDialog(new CFileDialog(TRUE, NULL, NULL, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
        "Image Files (*.png; *.bmp; *.jpg; *.gif; *.tif)|*.png;*.bmp;*.jpg;*.gif;*.tif|All Files (*.*)|*.*||"));
    dlgFileDialog->m_ofn.lpstrTitle = "Calibration Image selection";
    CString csPath = m_csEditAcq[p_nId];
    PathRemoveFileSpec(csPath.GetBuffer());
    csPath.ReleaseBuffer();
    dlgFileDialog->m_ofn.lpstrInitialDir = csPath;

    if (dlgFileDialog->DoModal() == IDOK)
    {
        csPath = dlgFileDialog->GetPathName();
        m_csEditAcq[p_nId] = csPath;
        UpdateData(FALSE);
    }
}

void CDlgCalibCam::SaveSettings()
{
    CString strSection = _T("CH3CameraCalib_CH3Mire");
    bool bres;
    bres = H3WritePrivProfileInt(strSection, _T("nLi"), m_nSizeX, g_strCalibCamFile);
    bres = H3WritePrivProfileInt(strSection, _T("nCo"), m_nSizeY, g_strCalibCamFile);
    bres = H3WritePrivProfileFloat(strSection, _T("StepX"), m_fStepX, g_strCalibCamFile);
    bres = H3WritePrivProfileFloat(strSection, _T("StepY"), m_fStepY, g_strCalibCamFile);

    for (int i = 0; i < m_nbImages; i++)
    {
        CString csImgAcqKey;
        csImgAcqKey.Format(_T("PathImgAcq%d"), i + 1);
        bres = H3WritePrivProfileString(strSection, csImgAcqKey, m_csEditAcq[i], g_strCalibCamFile);
    }
}


void CDlgCalibCam::OnBnClickedButtonSaveprm()
{
    UpdateData(TRUE);
    SaveSettings();
}


void CDlgCalibCam::OnMove(int x, int y)
{
    CDialog::OnMove(x, y);

    // TODO: ajoutez ici le code de votre gestionnaire de messages
}
