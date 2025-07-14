// DlgMesure.cpp : fichier d'implémentation
//

#include "stdafx.h"
#include "NanoCalib.h"
#include "DlgMesure.h"
#include "afxdialogex.h"

//Holo3
#include "H3Array2D.h"
#include "H3AppToolsDecl.h"
#include "H3_HoloMap_AltaTypeExport.h"
#include "MeasureInfoClass.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


//
// Saturate cast
//
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

// Initialisation des paramètres pour le calibrage System 
const CString g_strCaliMesureFile = CString(_CalibPaths._LastCalibPath) + _T("\\") + _CalibPaths._CheckMesureFile;

#include <shlobj.h>

int CALLBACK BrowseForFolderCallback(HWND hwnd, UINT uMsg, LPARAM lp, LPARAM pData)
{
    char szPath[MAX_PATH];

    switch (uMsg)
    {
    case BFFM_INITIALIZED:
        SendMessage(hwnd, BFFM_SETSELECTION, TRUE, pData);
        break;

    case BFFM_SELCHANGED:
        if (SHGetPathFromIDList((LPITEMIDLIST)lp, szPath))
        {
            SendMessage(hwnd, BFFM_SETSTATUSTEXT, 0, (LPARAM)szPath);

        }
        break;
    }

    return 0;
}

BOOL BrowseFolders(HWND hwnd, LPSTR lpszFolder, LPSTR lpszTitle)
{
    BROWSEINFO bi;
    char szPath[MAX_PATH + 1];
    LPITEMIDLIST pidl;
    BOOL bResult = FALSE;

    LPMALLOC pMalloc;

    if (SUCCEEDED(SHGetMalloc(&pMalloc)))
    {
        bi.hwndOwner = hwnd;
        bi.pidlRoot = NULL;
        bi.pszDisplayName = NULL;
        bi.lpszTitle = lpszTitle;
        bi.ulFlags = BIF_STATUSTEXT; //BIF_EDITBOX 
        bi.lpfn = BrowseForFolderCallback;
        bi.lParam = (LPARAM)lpszFolder;

        pidl = SHBrowseForFolder(&bi);
        if (pidl)
        {
            if (SHGetPathFromIDList(pidl, szPath))
            {
                errno_t x = strcpy_s(lpszFolder, MAX_PATH, szPath);
                bResult = (x == 0) ? TRUE : FALSE;
            }

            pMalloc->Free(pidl);
            pMalloc->Release();

        }
    }

    return bResult;

}

// Boîte de dialogue CDlgMesure

IMPLEMENT_DYNAMIC(CDlgMesure, CDialog)

CDlgMesure::CDlgMesure(CWnd* pParent /*=NULL*/)
    : CDialog(CDlgMesure::IDD, pParent)
{
    for (int i = 0; i < 3; i++)
    {
        m_csEditAcq[i] = _T("");
    }
    m_csEditOutFolder = _T("");
    m_fAltiInit_um = 0.0f;
    m_uAltInitPosX = 0;
    m_uAltInitPosY = 0;
    m_uRefPosInPicX = 0;
    m_uRefPosInPicY = 0;
}

CDlgMesure::~CDlgMesure()
{
}

void CDlgMesure::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ1, m_ctrlBtnBrsw[0]);
    DDX_Control(pDX, IDC_EDIT_PHASE_ACQ1, m_ctrlEditAcq[0]);
    DDX_Text(pDX, IDC_EDIT_PHASE_ACQ1, m_csEditAcq[0]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ2, m_ctrlBtnBrsw[1]);
    DDX_Control(pDX, IDC_EDIT_PHASE_ACQ2, m_ctrlEditAcq[1]);
    DDX_Text(pDX, IDC_EDIT_PHASE_ACQ2, m_csEditAcq[1]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_ACQ3, m_ctrlBtnBrsw[2]);
    DDX_Control(pDX, IDC_EDIT_PHASE_ACQ3, m_ctrlEditAcq[2]);
    DDX_Text(pDX, IDC_EDIT_PHASE_ACQ3, m_csEditAcq[2]);

    DDX_Control(pDX, IDC_BUTTON_BRSW_FOLDER, m_ctrlBtnBrswOutFolder);
    DDX_Control(pDX, IDC_EDIT_FOLDER, m_ctrlEditOutFolder);
    DDX_Text(pDX, IDC_EDIT_FOLDER, m_csEditOutFolder);
    DDX_Text(pDX, IDC_EDIT_ALTINIT_VALUE, m_fAltiInit_um);
    DDX_Text(pDX, IDC_EDIT_ALTINITPOS_X, m_uAltInitPosX);
    DDX_Text(pDX, IDC_EDIT_ALTINITPOS_Y, m_uAltInitPosY);
    DDX_Text(pDX, IDC_EDIT_REFINPIC_X, m_uRefPosInPicX);
    DDX_Text(pDX, IDC_EDIT_REFINPIC_Y, m_uRefPosInPicY);
}


BEGIN_MESSAGE_MAP(CDlgMesure, CDialog)
    ON_BN_CLICKED(IDC_BUTTON_MESURE, &CDlgMesure::OnBnClickedButtonMesure)
    ON_BN_CLICKED(IDC_BUTTON_SAVEPRM, &CDlgMesure::OnBnClickedButtonSaveprm)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ1, &CDlgMesure::OnBnClickedButtonBrswAcq1)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ2, &CDlgMesure::OnBnClickedButtonBrswAcq2)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ3, &CDlgMesure::OnBnClickedButtonBrswAcq3)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_FOLDER, &CDlgMesure::OnBnClickedButtonBrswFolder)
    ON_EN_CHANGE(IDC_EDIT_ALTINITPOS_X, &CDlgMesure::OnEnChangeEditAltinitposX)
END_MESSAGE_MAP()


// Gestionnaires de messages de CDlgMesure

void CDlgMesure::OnBnClickedButtonMesure()
{
    UpdateData(TRUE);

    CString str;

    CWaitCursor oWait;
    EnabledUI(FALSE);

    // Position du point remarquable
    unsigned int nCrossX = 0;
    unsigned int nCrossY = 0;

    // Initialisation structure de données	
    // Chargement des images de phases modulees (W) wx et wy

    CImageFloat** pArrayOfWPicture = new CImageFloat * [2];
    for (size_t i = 0; i < 2; i++)
    {
        pArrayOfWPicture[i] = new CImageFloat();
        if (pArrayOfWPicture[i]->Load(m_csEditAcq[i]) == false)
        {
            CString csMsg;
            csMsg.Format(_T("Could not load Phase Image n°%d : {%s}\n Wrong Path or file extension"), i + 1, m_csEditAcq[i]);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
            EnabledUI(TRUE);
            delete pArrayOfWPicture[i];
            return;
        }
    }


    // Chargement de l'image masque wmask
    CImageByte maskImage;
    if (maskImage.Load(m_csEditAcq[2]) == false)
    {
        CString csMsg;
        csMsg.Format(_T("Could not load Mask Image : {%s}\n Wrong Path or file extension"), m_csEditAcq[2]);
        AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        EnabledUI(TRUE);
        return;
    }

    /*SaveGreyImageFlt32("D:\\Projets\\Docs\\gloabltopo\\screenshot\\In_wX.png",pArrayOfWPicture);
    SaveGreyImageFlt32("D:\\Projets\\Docs\\gloabltopo\\screenshot\\In_wY.png",pArrayOfWPicture+1);
    SaveGreyImageByte("D:\\Projets\\Docs\\gloabltopo\\screenshot\\In_Msk.png",&maskImage);
    return;*/

    if (g_bInitCalibSys == false)
    {
        H3_InitSys(CString(_CalibPaths._LastCalibPath) + "\\");
        g_bInitCalibSys = true;
    }

    // Mesurer
    CMeasureInfoClass	measure;
    float fratio = 1.0f;
    tuple<int, int> pixel_imageDuPointDAltitudeConnue(m_uAltInitPosX, m_uAltInitPosY);
    const float altitude = m_fAltiInit_um / 1000.0F;			//altitude du point vu sur le pixel du point pixel_imageDuPointDAltitudeConnue
    tuple<int, int>  pixel_ref_inPicture(m_uRefPosInPicX, m_uRefPosInPicY);
    const bool  b_mesure_shape = true;	//oui pour mesurer le 'vrai' relief (plutot qu'une approximation 'quasi plan')

    //measure.SetData(nCrossX, nCrossY, pArrayOfWPicture, &maskImage, fratio);
    measure.SetData(nCrossX, nCrossY, pArrayOfWPicture, &maskImage, fratio,
        pixel_imageDuPointDAltitudeConnue, altitude,
        pixel_ref_inPicture,
        b_mesure_shape);


    int nAppreciationMeasure = -1;
    Mesurer(&measure, false, false, nAppreciationMeasure);

    // Resultats
    const size_t nLi = maskImage.nLi, nCo = maskImage.nCo, nSz = nLi * nCo;
    const size_t nNbImages = 6L;
    CImageFloat* pArrayOfPicture = new CImageFloat[nNbImages];
    for (size_t i = 0; i < nNbImages; i++)
    {
        pArrayOfPicture[i].pData = new float[nLi * nCo];
    }

    CString strMsg;
    strMsg = _T("");
    switch (nAppreciationMeasure)
    {
    case 0:
    {
        strMsg = CString(_T("The measure is successful."));
        AfxMessageBox(strMsg);

        measure.GetData(pArrayOfPicture, nNbImages);

        H3_MATRIX_FLT32  X(nLi, nCo);
        H3_MATRIX_FLT32  Y(nLi, nCo);
        H3_MATRIX_FLT32  Z(nLi, nCo);
        H3_MATRIX_FLT32 NX(nLi, nCo);
        H3_MATRIX_FLT32 NY(nLi, nCo);
        H3_MATRIX_FLT32 NZ(nLi, nCo);

        X.Fill(NaN);   Y.Fill(NaN);  Z.Fill(NaN);
        NX.Fill(NaN);  NY.Fill(NaN); NZ.Fill(NaN);
        //	X.Fill(0.0f);   Y.Fill(0.0f);  Z.Fill(0.0f); 
        //	NX.Fill(0.0f);  NY.Fill(0.0f); NZ.Fill(0.0f);

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

        PathAddBackslash(m_csEditOutFolder.GetBuffer(m_csEditOutFolder.GetLength() + 1));
        m_csEditOutFolder.ReleaseBuffer(m_csEditOutFolder.GetLength() + 1);
        CreateFolder(m_csEditOutFolder);

        if (SaveMatrix(NX, m_csEditOutFolder, _T("NX")) == false)
        {
            CString csMsg;
            csMsg.Format(_T("Could not Save Matrix NX in : {%s}"), m_csEditOutFolder);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        }
        if (SaveMatrix(NY, m_csEditOutFolder, _T("NY")) == false)
        {
            CString csMsg;
            csMsg.Format(_T("Could not Save Matrix NY in : {%s}"), m_csEditOutFolder);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        }
        if (SaveMatrix(NZ, m_csEditOutFolder, _T("NZ")) == false)
        {
            CString csMsg;
            csMsg.Format(_T("Could not Save Matrix NZ in : {%s}"), m_csEditOutFolder);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        }

        if (SaveMatrix(X, m_csEditOutFolder, _T("X")) == false)
        {
            CString csMsg;
            csMsg.Format(_T("Could not Save Matrix X in : {%s}"), m_csEditOutFolder);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        }
        if (SaveMatrix(Y, m_csEditOutFolder, _T("Y")) == false)
        {
            CString csMsg;
            csMsg.Format(_T("Could not Save Matrix Y in : {%s}"), m_csEditOutFolder);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        }

        // 			CString sDbgName = m_csEditAcq[2];
        // 			LPTSTR lp = PathFindFileName(sDbgName.GetBuffer());
        // 			sDbgName.ReleaseBuffer();
        // 			sDbgName =  CString(lp);
        // 			sDbgName = sDbgName.Left(sDbgName.GetLength()-8); //8== Mask.bin à retirer et remplacer par Z-h0
        // 			sDbgName += _T("Z-h0v3");
        // 			if(SaveMatrix(Z,m_csEditOutFolder, sDbgName)== false)
        if (SaveMatrix(Z, m_csEditOutFolder, _T("Z")) == false)
        {
            CString csMsg;
            csMsg.Format(_T("Could not Save Matrix Z in : {%s}"), m_csEditOutFolder);
            AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        }
    }
    break;
    case 1:
        //	strMsg = CString(_T("Le capteur n'est pas alloué!."));
        strMsg = CString(_T("Sensor is not assigned."));
        AfxMessageBox(strMsg);
        break;
    case 2:
        //	strMsg = CString(_T("Les données sont de tailles distinctes!."));
        strMsg = CString(_T("Data size are distinct !."));
        AfxMessageBox(strMsg);
        break;
    case 3:
        //AfxMessageBox("La cartographie de mesure est vide");
        AfxMessageBox(_T("Measure cartography is empty"));
        break;
    default:
        break;
    }



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

    EnabledUI(TRUE);
}

void CDlgMesure::OnBnClickedButtonSaveprm()
{
    UpdateData(TRUE);

    CString strSection;
    CString csDefaultPath;
    bool bres;
    strSection = _T("InputData");
    bres = H3WritePrivProfileString(strSection, "PhaseX", m_csEditAcq[0], g_strCaliMesureFile);
    bres = H3WritePrivProfileString(strSection, "PhaseY", m_csEditAcq[1], g_strCaliMesureFile);
    bres = H3WritePrivProfileString(strSection, "Mask", m_csEditAcq[2], g_strCaliMesureFile);

    bres = H3WritePrivProfileInt(strSection, "AltitudeInitPosition_X", (int)m_uAltInitPosX, g_strCaliMesureFile);
    bres = H3WritePrivProfileInt(strSection, "AltitudeInitPosition_Y", (int)m_uAltInitPosY, g_strCaliMesureFile);
    bres = H3WritePrivProfileFloat(strSection, "AltitudeInitPosition_Value_um", m_fAltiInit_um, g_strCaliMesureFile);
    bres = H3WritePrivProfileString(strSection, "RefScreenPositionInPic_X", (int)m_uRefPosInPicX, g_strCaliMesureFile);
    bres = H3WritePrivProfileString(strSection, "RefScreenPositionInPic_Y", (int)m_uRefPosInPicY, g_strCaliMesureFile);

    strSection = _T("OutputData");
    bres = H3WritePrivProfileString(strSection, _T("Folder"), m_csEditOutFolder, g_strCaliMesureFile);
}


void CDlgMesure::OnBnClickedButtonBrswAcq1()
{
    BrowsePhaseImg(0);
}


void CDlgMesure::OnBnClickedButtonBrswAcq2()
{
    BrowsePhaseImg(1);
}


void CDlgMesure::OnBnClickedButtonBrswAcq3()
{
    BrowsePhaseImg(2);
}


void CDlgMesure::OnBnClickedButtonBrswFolder()
{
    UpdateData(TRUE);

    CString csTitle = _T("Please select Output Folder");
    BrowseFolders(this->GetSafeHwnd(), m_csEditOutFolder.GetBuffer(MAX_PATH), csTitle.GetBuffer());
    m_csEditOutFolder.ReleaseBuffer(MAX_PATH);
    csTitle.ReleaseBuffer();

    UpdateData(FALSE);
}


BOOL CDlgMesure::OnInitDialog()
{
    CDialog::OnInitDialog();

    CString strSection;

    strSection = _T("InputData");
    //csDefaultPath = g_csAltasightBaseDirPath + _T("\\Data_Test\\PhaseX.bin");
    m_csEditAcq[0] = H3GetPrivProfileString(strSection, "PhaseX", g_strCaliMesureFile);
    //csDefaultPath = g_csAltasightBaseDirPath + _T("\\Data_Test\\PhaseY.bin");
    m_csEditAcq[1] = H3GetPrivProfileString(strSection, "PhaseY", g_strCaliMesureFile);
    //csDefaultPath = g_csAltasightBaseDirPath + _T("\\Data_Test\\Mask.bin");
    m_csEditAcq[2] = H3GetPrivProfileString(strSection, "Mask", g_strCaliMesureFile);

    m_uAltInitPosX = H3GetPrivProfileInt(strSection, "AltitudeInitPosition_X", g_strCaliMesureFile);
    m_uAltInitPosY = H3GetPrivProfileInt(strSection, "AltitudeInitPosition_Y", g_strCaliMesureFile);
    m_fAltiInit_um = H3GetPrivProfileFloat(strSection, "AltitudeInitPosition_Value_um", g_strCaliMesureFile);
    m_uRefPosInPicX = H3GetPrivProfileInt(strSection, "RefScreenPositionInPic_X", g_strCaliMesureFile);
    m_uRefPosInPicY = H3GetPrivProfileInt(strSection, "RefScreenPositionInPic_Y", g_strCaliMesureFile);

    strSection = _T("OutputData");
    //csDefaultPath = g_csAltasightBaseDirPath + _T("\\Data_Test");
    m_csEditOutFolder = H3GetPrivProfileString(strSection, _T("Folder"), g_strCaliMesureFile);

    return TRUE;  // return TRUE unless you set the focus to a control
    // EXCEPTION : les pages de propriétés OCX devraient retourner FALSE
}


void CDlgMesure::OnOK()
{
    //	CDialog::OnOK();
}


void CDlgMesure::OnCancel()
{
    //	CDialog::OnCancel();
}


void CDlgMesure::WinHelp(DWORD dwData, UINT nCmd)
{
    //CDialog::WinHelp(dwData, nCmd);
}

void CDlgMesure::EnabledUI(BOOL p_bEnable)
{
    for (int i = 0; i < 3; i++)
    {
        m_ctrlEditAcq[i].EnableWindow(p_bEnable);
        m_ctrlBtnBrsw[i].EnableWindow(p_bEnable);
    }
    m_ctrlBtnBrswOutFolder.EnableWindow(p_bEnable);
    m_ctrlEditOutFolder.EnableWindow(p_bEnable);
}

void CDlgMesure::BrowsePhaseImg(UINT p_nId)
{
    UpdateData(TRUE);

    std::auto_ptr<CFileDialog> dlgFileDialog(new CFileDialog(TRUE, NULL, NULL, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
        "Bin Files (*.bin)|*.bin|All Files (*.*)|*.*||"));
    dlgFileDialog->m_ofn.lpstrTitle = "Calibration Phase selection";
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

bool CDlgMesure::SaveBin(H3_MATRIX_FLT32& p_oMatrix, FILE* stream)
{
    if (stream)
    {
        int nLi = (int)p_oMatrix.GetLi();
        int nCo = (int)p_oMatrix.GetCo();
        ::fwrite(&nLi, sizeof(int), 1, stream);
        ::fwrite(&nCo, sizeof(int), 1, stream);
        if (nLi * nCo == 0)
        {
            return true;
        }

        if (::fwrite(p_oMatrix.GetData(), sizeof(H3_FLT32), nLi * nCo, stream) == nLi * nCo)
        {
            return true;
        }
    }
    return false;
}

static inline void float2Bytes(float val, byte* bytes_array)
{
    // Create union of shared memory space
    union {
        float float_variable;
        byte temp_array[4];
    } u;
    // Overite bytes of union with float variable
    u.float_variable = val;
    // Assign bytes to input array
    memcpy(bytes_array, u.temp_array, 4);
}

static inline void ConverfloatArray2BytesArray(float* pfArray, byte* pucArray, int nbFloatArrayElement)
{
    for (int i = 0; i < nbFloatArrayElement; i++)
    {
        float2Bytes(pfArray[i], (pucArray + i * 4));
    }
}

bool CDlgMesure::Save3DA(H3_MATRIX_FLT32& p_oMatrix, CString p_csPath, CString p_csMatrixName)
{
    CString csFileName;
    csFileName.Format(_T("%s%s.3da"), p_csPath, p_csMatrixName);
    bool bRes = false;

    // save Bin file
    FILE* pFile = 0;
    if (fopen_s(&pFile, (LPCSTR)csFileName, "wb+") == 0)
    {
        int nFormatVERSION = 1;
        int nLi = (int)p_oMatrix.GetLi(); // Y Heigth
        int nCo = (int)p_oMatrix.GetCo(); // X width
        int nszflt = sizeof(float);
        int nSzTot = nLi * nCo * nszflt;
        unsigned int lCompressSize = 0; // on ne compresse pas ici pour eviter d'embarquer zlibwapi.dll
        byte* pbArray = new byte[nSzTot];
        float* pfArray = p_oMatrix.GetData();
        for (int i = 0; i < nLi * nCo; i++)
            pfArray[i] *= 1000.0f; // passage de mm en µm 
        ConverfloatArray2BytesArray(pfArray, pbArray, nLi * nCo);

        ::fwrite(&nFormatVERSION, sizeof(int), 1, pFile);
        ::fwrite(&nLi, sizeof(int), 1, pFile);
        ::fwrite(&nCo, sizeof(int), 1, pFile);
        ::fwrite(&nszflt, sizeof(int), 1, pFile);
        ::fwrite(&nSzTot, sizeof(int), 1, pFile);
        ::fwrite(&lCompressSize, sizeof(unsigned int), 1, pFile);
        ::fwrite(pbArray, sizeof(byte), nSzTot, pFile);
        fclose(pFile);

        delete[] pbArray;
        pbArray = NULL;

        bRes = true;
    }
    return bRes;
}



bool CDlgMesure::SaveMatrix(H3_MATRIX_FLT32& p_oMatrix, CString p_csPath, CString p_csMatrixName)
{
    CString csFileName;
    csFileName.Format(_T("%s%s.bin"), p_csPath, p_csMatrixName);
    bool bRes = true;

    // save Bin file
    /*FILE* pFile = 0;
    if(fopen_s(&pFile, (LPCSTR)csFileName,"wb+") == 0)
    {
        bRes = SaveBin(p_oMatrix,pFile);
        fclose(pFile);
    }*/

    // Save PNG			
    if (bRes)
    {
        csFileName.Format(_T("%s%s.png"), p_csPath, p_csMatrixName);
        bRes = SaveGreyImageFlt32(csFileName, p_oMatrix);
    }

    //Save 3da
    if (bRes)
    {
        bRes = Save3DA(p_oMatrix, p_csPath, p_csMatrixName);
    }
    return bRes;
}

bool CDlgMesure::SaveGreyImageFlt32(CString p_csFilepath, H3_MATRIX_FLT32& p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
{
    float* pData = p_oMatrixFloat.GetData();

    unsigned long  lCols = (unsigned long)p_oMatrixFloat.GetCo();
    unsigned long  lLines = (unsigned long)p_oMatrixFloat.GetLi();

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

    if ((fMax != fMin) && (fMin != FLT_MAX) && (fMax != -FLT_MAX))
    {
        a = 255.0f / (fMax - fMin);
        b = -fMin * 255.0f / (fMax - fMin);
    }

    fipImage oImg(FIT_BITMAP, lCols, lLines, 8);
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
    BOOL bRes = oImg.save(p_csFilepath, 0);
    return (bRes != 0);
}

bool CDlgMesure::SaveGreyImageFlt32(CString p_csFilepath, CImageFloat* p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
{
    float* pData = p_oMatrixFloat->pData;

    unsigned long  lCols = (unsigned long)p_oMatrixFloat->nCo;
    unsigned long  lLines = (unsigned long)p_oMatrixFloat->nLi;

    float fMin = FLT_MAX;
    float fMax = -FLT_MAX;

    bool bUseMinPrm = (p_fMin != FLT_MAX);
    bool bUseMaxPrm = (p_fMax != FLT_MAX);

    float a = 1.0f;
    float b = 0.0f;
    if (bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
    {

        for (long lItem = 0; lItem < (long)(lCols * lLines); lItem++)
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

    if (fMax != fMin)
    {
        a = 255.0f / (fMax - fMin);
        b = -fMin * 255.0f / (fMax - fMin);
    }

    fipImage oImg(FIT_BITMAP, lCols, lLines, 8);
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
    BOOL bRes = oImg.save(p_csFilepath, 0);
    return (bRes != 0);
}

bool CDlgMesure::SaveGreyImageByte(CString p_csFilepath, CImageByte* p_oMatrixByte)
{
    byte* pData = p_oMatrixByte->pData;
    unsigned long  lCols = (unsigned long)p_oMatrixByte->nCo;
    unsigned long  lLines = (unsigned long)p_oMatrixByte->nLi;

    fipImage oImg(FIT_BITMAP, lCols, lLines, 8);
    for (unsigned y = 0; y < oImg.getHeight(); y++)
    {
        //ici pb à resoudre pour affichage image
        BYTE* pbits = (BYTE*)oImg.getScanLine(y);
        for (unsigned x = 0; x < oImg.getWidth(); x++)
        {
            pbits[x] = saturate_cast<uchar>(pData[y * lCols + x]);
        }
    }
    oImg.flipVertical();
    BOOL bRes = oImg.save(p_csFilepath, 0);
    return (bRes != 0);
}

void CDlgMesure::OnEnChangeEditAltinitposX()
{
    // TODO:  S'il s'agit d'un contrôle RICHEDIT, le contrôle ne
    // envoyez cette notification sauf si vous substituez CDialog::OnInitDialog()
    // fonction et appelle CRichEditCtrl().SetEventMask()
    // avec l'indicateur ENM_CHANGE ajouté au masque grâce à l'opérateur OR.

    // TODO:  Ajoutez ici le code de votre gestionnaire de notification de contrôle
}
