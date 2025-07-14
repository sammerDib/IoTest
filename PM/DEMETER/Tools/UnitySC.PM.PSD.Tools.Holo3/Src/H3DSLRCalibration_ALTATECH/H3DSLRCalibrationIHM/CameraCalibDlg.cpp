// CameraCalibDlg.cpp : implementation file
//
#include "stdafx.h"
#include ".\\..\\H3DSLRCalibration\\H3DSLRCalibration.h"//pour __declspec

#include <direct.h>
#include <stdlib.h>
#include <stdio.h>
#include <math.h>

#include "H3DSLRCalibrationIHMrc.h"
#include "CameraCalibDlg.h"
#include "H3AppToolsDecl.h"
#include "ResultCalibrageDlg.h"
#pragma warning (disable: 4244)

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#define ETAPE_INITIAL	0
#define ETAPE_SELECTION_4POINTS	1
#define ETAPE_ACCEPTATIONREFU	2 
#define ETAPE_ACCEPTATION		3 

#define NOMBRE_IMAGE_MIN_CALIB	18
#define NB_CORNER_PER_MIRE (4L)		//nb de coin à cliquer
#define NB_PASS_IMAGE (3L)			//nb mire utiles par image

#define COLOR_ROUGE RGB(255,0,0)
#define COLOR_VERT RGB(0,255,0)
#define COLOR_BLEU RGB(0,0,255)
#define DEMO 0

//#define DEFAULT_IND_CAM (0)
//#define DEFAULT_MIN_NBIMAGE (18)
//#define DEFAULT_SAVE_CALIB (false)
//#define DEFAULT_GRAB_SOURCE 0

int CCameraCalibDlg::m_nErrorTypeCalibCam = -1L;

static CString strModule("CCameraCalibDlg");
/////////////////////////////////////////////////////////////////////////////
// CCameraCalibDlg dialog
CCameraCalibDlg::CCameraCalibDlg(const CString& calibFolder)
    : CDialog(CCameraCalibDlg::IDD)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("CCameraCalibDlg()");

    //{{AFX_DATA_INIT(CCameraCalibDlg)
    // NOTE: the ClassWizard will add member initialization here
    //}}AFX_DATA_INIT
    m_pCameraCalib = new CH3CameraCalib(calibFolder);

    m_nNombreImageMinCalib = 12;
    m_bSaveCalib = false;
    m_strSavePath.Empty();

    m_bCalibrageValid = false;

    m_nUse = TO_CALIBRATE_CAM;

    m_nGrabSource = 0; // 0: Images provenant du module MODERE de Altatech
    // 1: Images provenant du disque dur
    m_nNbPassImage = 0;
    m_nNbImageReel = 0;

    m_bCalibSystem = false;
    m_csEditInfoText = _T("");
}

void CCameraCalibDlg::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);
    //{{AFX_DATA_MAP(CCameraCalibDlg)
    DDX_Control(pDX, IDOK, m_cBtnOK);
    DDX_Control(pDX, IDC_BTN_REFUSER, m_cBtnRefuser);
    DDX_Control(pDX, IDC_BTN_ACQUERIR, m_cBtnAcquerir);
    DDX_Control(pDX, IDC_BTN_ACCEPTER, m_cBtnAccepter);
    DDX_Control(pDX, IDC_BTN_LOAD_CLIST, m_cBtnLoadCorrespList2);
    DDX_Control(pDX, IDC_BTN_ABANDONNER, m_cBtnAbandonner);
    DDX_Control(pDX, IDC_STATIC_IMAGE, m_CImage);
    //}}AFX_DATA_MAP
    DDX_Text(pDX, IDC_EDIT_INFO, m_csEditInfoText);
}


BEGIN_MESSAGE_MAP(CCameraCalibDlg, CDialog)
    //{{AFX_MSG_MAP(CCameraCalibDlg)
    ON_WM_LBUTTONDOWN()
    ON_WM_TIMER()
    ON_BN_CLICKED(IDC_BTN_ACQUERIR, OnBtnAcquerir)
    ON_BN_CLICKED(IDC_BTN_ABANDONNER, OnBtnAbandonner)
    ON_BN_CLICKED(IDC_BTN_REFUSER, OnBtnRefuser)
    ON_BN_CLICKED(IDC_BTN_ACCEPTER, OnBtnAccepter)
    ON_BN_CLICKED(IDC_BTN_LOAD_CLIST, OnBtnLoadCorrespList2)
    //	ON_WM_SIZING()
    //	ON_WM_SIZE()
    ON_WM_PAINT()
    ON_WM_MOVE()
    //}}AFX_MSG_MAP	
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CCameraCalibDlg message handlers

BOOL CCameraCalibDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    // TODO: Add extra initialization here
    m_nNbImage = 0;
    m_bGoodIntersection = false;
    if (m_nUse == TO_CALIBRATE_CAM) {
        ActiveBTN(ETAPE_INITIAL);
        SetTitre();
    }
    else {
        if (&m_aBufImage == NULL)
            ActiveBTN(ETAPE_INITIAL);
        else {
            if (m_aBufImage.GetSize() == 0)
                ActiveBTN(ETAPE_INITIAL);
            else
                ActiveBTN(ETAPE_SELECTION_4POINTS);
        }

        if (m_nUse == TO_GET_CORNERS)
        {
            SetTimer(1, 200, NULL);
            SetWindowText("select a chess board");
            m_csEditInfoText = "Please select a chessboards (4 points). Then Accept or Decline.";
            UpdateData(FALSE);
        }
        else if (m_nUse == TO_GET_AREA)
        {
            SetTimer(1, 200, NULL);
            SetWindowText("select the mirror");
            m_csEditInfoText = "Please select the Mirror zone (4 points). Then Accept or Decline.";
            UpdateData(FALSE);
        }
    }

    return TRUE;  // return TRUE unless you set the focus to a control
                  // EXCEPTION: OCX Property Pages should return FALSE
}

void CCameraCalibDlg::OnLButtonDown(UINT nFlags, CPoint point)
{
    CRect rcImageWin;
    GetClientRect(rcImageWin);

    CString strFunction = _T("OnLButtonDown"), msg;

    if (ETAPE_SELECTION_4POINTS == GetEtape())
    {
        CRect rcImageDisplay = m_Display.GetImageRect();
        CRect rcImage;
        m_CImage.GetWindowRect(rcImage);
        ScreenToClient(rcImage);
        if (rcImage.PtInRect(point))
        {
            CPoint point1(point.x - rcImage.left, point.y - rcImage.top);
            if (rcImageDisplay.PtInRect(point1))
            {
                if (m_List4Points.GetSize() < NB_CORNER_PER_MIRE)
                {
                    CPoint px(point1.x - rcImageDisplay.left, point1.y - rcImageDisplay.top);
                    H3_POINT2D_FLT32 pt;
                    H3_ARRAY2D_UINT8 tt(GetImageVideo());
                    if (tt.GetSize() == 0)
                    {
                        H3DisplayError("Image Vide");
                        return;
                    }
                    pt.x = ((float)(((float)px.x) * ((float)m_aBufImage.GetCo()))) / ((float)rcImageDisplay.Width());
                    pt.y = ((float)(((float)px.y) * ((float)m_aBufImage.GetLi()))) / ((float)rcImageDisplay.Height());

                    m_List4Points.Add(pt);
                    Draw4Points();

                    if (m_List4Points.GetSize() >= 4) {
                        if (m_nUse == TO_GET_AREA)
                        {
                            m_bGoodIntersection = true;
                            ActiveBTN(ETAPE_ACCEPTATIONREFU);

                            H3_ARRAY2D_UINT8 Image(GetImageVideo());
                            H3_ARRAY2D_UINT8 ImageMasque;
                            ImageMasque = GetMask(Image.GetLi(), Image.GetCo());
                            ImageMasque *= Image;
                            //Dessiner masque
                            if (m_bCalibSystem)
                            {
                                m_cBtnRefuser.EnableWindow(FALSE);
                            }
                            SetImage(ImageMasque);
                            m_Display.Draw(&m_CImage);

                            ReDrawScene();
                        }
                        else
                        {
                            if (!CalculImage())
                            {
                                OnBtnRefuser();
                            }
                            if (m_bCalibSystem)
                                m_cBtnRefuser.EnableWindow(FALSE);
                        }
                    }
                }
            }
        }

    }
    CDialog::OnLButtonDown(nFlags, point);

}


void CCameraCalibDlg::OnPaint()
{
    CPaintDC dc(this); // device context for painting
    CDialog::OnPaint();
    ReDrawScene();
}

void CCameraCalibDlg::OnBtnAcquerir()
{
    CString strFunction = _T("OnBtnAcquerir");
    H3DebugInfo(strModule, strFunction, "");

    {
        H3_ARRAY2D_UINT8 tt = GetImageVideo();
        SetImage(tt);
        if (tt.GetSize() == 0)
        {
            H3DisplayError("Empty Image");
            return;
        }
        else if (m_nNbImage == 0)
        {

            size_t nx = tt.GetCo();
            size_t ny = tt.GetLi();

            m_BufferMasqueMire = H3_ARRAY2D_UINT8(ny, nx);
            m_BufferMasqueMire.Fill(1);

            m_pCameraCalib->InitDefault(nx, ny);
        }
    }

    m_List4Points.RemoveAll();
    m_PointIntersec.ReAlloc(0, 0);

    //Mettre les boutons a jours
    ActiveBTN(ETAPE_SELECTION_4POINTS);
    //	AfxMessageBox("Veuillez cliquer les 4 coins de la mire.");
    //	AfxMessageBox("Please select the 4 chessboard grid corners.");
    m_csEditInfoText = "Please select the 4 chessboards grid corners. Then Accept or Decline.";
    UpdateData(FALSE);

    //suite: OnLButtonDown ...

}

void CCameraCalibDlg::OnBtnAbandonner()
{
    for (int i = 0; i < m_nNbImage; i++)
    {
        RemoveImage();
    }
    CDialog::OnCancel();
}

void CCameraCalibDlg::OnBtnRefuser()
{
    RemoveImage();
    ActiveBTN(ETAPE_INITIAL);
}

void CCameraCalibDlg::OnBtnAccepter()
{
    CString strFunction("OnBtnAccepter");

    CWaitCursor wait;

    if (m_nUse == TO_GET_CORNERS || m_nUse == TO_GET_AREA) {
        OnOK();
        return;
    }

    m_nNbImage++;
    m_BufferMasqueMire *= m_ImageMasqueMire;

    if (m_nNbPassImage >= NB_PASS_IMAGE - 1)
    {
        m_nNbImageReel++;
        m_nNbPassImage = 0;
        m_BufferMasqueMire.Fill(1);
    }
    else
    {
        m_nNbPassImage++;
    }

    if (m_bSaveCalib && m_nUse == TO_CALIBRATE_CAM)
    {
        if (m_aLastImage.GetSize() == 0)
        {
            H3DisplayError(strModule, strFunction, "Image Vide");
        }
        else {
            if (_chdir(m_strSavePath) == -1)
                _mkdir(m_strSavePath);

            CString strSaveCalibImageName;
            strSaveCalibImageName.Format("%s\\CalibVideo_%d_%d.hbf", m_strSavePath, m_nIndiceCamera, m_nNbImage);
            m_aLastImage.Save(strSaveCalibImageName);
        }
    }

    m_csEditInfoText = "Next Step...";

    if (m_nNbImage >= m_nNombreImageMinCalib)
    {
        m_bCalibrageValid = Calibrage();

        float ReprojectionErrorMaxi = m_pCameraCalib->m_fReprojectionErrorMaxi;

        if (m_pCameraCalib->pix_erreur[0] > ReprojectionErrorMaxi)
        {
            H3DebugInfo(strModule, strFunction, "Les erreurs de reprojection suivant l'axe X sont supérieures\nà la valeur maximale autorisée soit 0.2.");
            // Erreur Code 1
            m_nErrorTypeCalibCam = 1L;
        }
        else if (m_pCameraCalib->pix_erreur[1] > ReprojectionErrorMaxi)
        {
            H3DebugInfo(strModule, strFunction, "Les erreurs de reprojection suivant l'axe Y sont supérieures\nà la valeur maximale autorisée soit 0.2.");
            // Erreur Code 2
            m_nErrorTypeCalibCam = 2L;
        }
        else if (m_pCameraCalib->pix_erreur[0] < FLT_EPSILON || m_pCameraCalib->pix_erreur[1] < FLT_EPSILON)
        {
            H3DebugInfo(strModule, strFunction, "Le calibrage n'a pas convergé. Veuillez contacter Holo3.");
            // Erreur Code 3
            m_nErrorTypeCalibCam = 3L;
        }

        if (m_bCalibrageValid &&
            m_pCameraCalib->pix_erreur[0] < ReprojectionErrorMaxi &&
            m_pCameraCalib->pix_erreur[0] >FLT_EPSILON &&
            m_pCameraCalib->pix_erreur[1] < ReprojectionErrorMaxi &&
            m_pCameraCalib->pix_erreur[1] >FLT_EPSILON)
        {
            H3DebugInfo(strModule, strFunction, "Le calibrage caméra a réussi.");
            // Erreur Code 0
            m_nErrorTypeCalibCam = 0L;
            CResultCalibrageDlg dlg(true, _T("Calibration Successful"), this);
            dlg.DoModal();
        }
        else
        {
            CString strRes = _T("L'algorithme de calibrage n'a pas suffisamment convergé :\n"\
                "Les raisons peuvent venir de :\n"\
                "- Sous ou sur exposition des images.\n"\
                "- Mise au point non optimale de l'objectif.\n"\
                "- Instabilité de la mire.\n"\
                "- Mauvais fichier de définition de la mire.\n"\
                "Vous pouvez néamoins accepter ce calibrage.");
            CResultCalibrageDlg dlg(false, strRes, this);
            dlg.DoModal();
        }
        m_bCalibrageValid = true;
        CString strSaveCalib;
        CString strFileName = CString(_CalibPaths._LastCalibPath) + _T("") + _CalibPaths._InputSettingsFile;
        CString strSection;
        strSection = _T("SensorHoloMap3");

        BOOL b = m_pCameraCalib->SaveCalib(_CalibPaths.CamCalibIntrinsicParamsPath(_CalibPaths._LastCalibPath), m_nIndiceCamera);

        m_csEditInfoText = "Calibration done ! Click OK.";
    }

    ActiveBTN(ETAPE_INITIAL);
    UpdateData(FALSE);

}

void CCameraCalibDlg::ActiveBTN(int choix)
{
    m_cBtnAcquerir.EnableWindow(FALSE);
    m_cBtnRefuser.EnableWindow(FALSE);
    m_cBtnAccepter.EnableWindow(FALSE);
    m_cBtnAbandonner.EnableWindow(FALSE);
    m_cBtnOK.EnableWindow(FALSE);

    SetEtape(choix);
    SetTitre();

    switch (choix)
    {
    case ETAPE_INITIAL:
    {
        m_cBtnAcquerir.EnableWindow(TRUE);
        m_cBtnAbandonner.EnableWindow(TRUE);

        if (m_bCalibrageValid)
            m_cBtnOK.EnableWindow(TRUE);
        break;
    }
    case ETAPE_SELECTION_4POINTS:
    {
        m_cBtnAbandonner.EnableWindow(TRUE);
        break;
    }
    case ETAPE_ACCEPTATIONREFU:
    {
        m_cBtnAbandonner.EnableWindow(TRUE);
        m_cBtnRefuser.EnableWindow(TRUE);
        if (m_bGoodIntersection)
            m_cBtnAccepter.EnableWindow(TRUE);

        break;
    }
    case ETAPE_ACCEPTATION:
    {
        m_cBtnAbandonner.EnableWindow(TRUE);
        m_cBtnRefuser.EnableWindow(TRUE);
        if (m_bGoodIntersection)
            m_cBtnAccepter.EnableWindow(TRUE);

        break;
    }
    default:
    {
        break;
    }
    }
    ReDrawScene();
}

void CCameraCalibDlg::SetImage(H3_ARRAY2D_UINT8& SrcBuf)
{
    m_aBufImage = SrcBuf;
    m_Display.SetColorBarStyle(0);
    {
        m_Display.SetRange(0, 255);
    }
    m_Display.SetColorMap1("LINEAR");
    m_Display.Set(m_aBufImage);

}

int CCameraCalibDlg::GetEtape()
{
    return m_nEtape;
}

void CCameraCalibDlg::SetEtape(int Etape)
{
    m_nEtape = Etape;
}

H3_ARRAY2D_UINT8 CCameraCalibDlg::GetImageVideo()
{
    CString strFunction("GetImageVideo");
    static int nimage = 1;
    H3_ARRAY2D_UINT8 a_DigBuf8;

    if (GetEtape() == ETAPE_INITIAL)
    {
        if (m_nGrabSource == 1)
        {
            // Source -> Fichier
            {
                CFileDialog FileOpenDlg(
                    TRUE, "hbf", NULL,
                    OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT,
                    "Fichiers HBF (Image vidéo Mire XY) (*.HBF)|*.hbf||");
                if (FileOpenDlg.DoModal() == IDOK)
                {
                    a_DigBuf8.Load(FileOpenDlg.GetPathName());
                }
                else
                {
                    a_DigBuf8 = H3_ARRAY2D_UINT8(0, 0);
                    H3DebugInfo(strModule, strFunction, "Annuler par l'operateur");
                    return false;
                }
            }
        }
        else
        {
            // Source -> Modere Altatech
            a_DigBuf8 = m_vMirePos[m_nNbImageReel];
            if (m_nNbPassImage > 0 && m_nNbPassImage < NB_PASS_IMAGE)
            {
                for (int i = 0; i < m_nNbPassImage; i++)
                {
                    a_DigBuf8 = a_DigBuf8 * m_BufferMasqueMire;
                }
            }
        }
    }
    else
    {
        a_DigBuf8 = m_aBufImage;
    }
    m_aLastImage = a_DigBuf8;
    return a_DigBuf8;
}

bool CCameraCalibDlg::CalculImage()
{
    CWinApp* pApp = AfxGetApp();
    pApp->DoWaitCursor(1);

    CString strFunction("CalculImage");
    H3DebugInfo(strModule, strFunction, "");

    //Creation de l'image masqué 
    H3_ARRAY2D_UINT8 Image(GetImageVideo());
    if (Image.GetSize() == 0)
    {
        H3DisplayError(strModule, strFunction, "L'image est vide");
        return false;
    }

    H3_ARRAY2D_UINT8 ImageMasque;
    ImageMasque = GetMask(Image.GetLi(), Image.GetCo());
    ImageMasque *= Image;
    //Dessiner masque
    SetImage(ImageMasque);
    m_Display.Draw(&m_CImage);
    //AddImage
    m_PointIntersec = AddImage(ImageMasque);

    if (m_PointIntersec.GetSize() == 0) {
        //AfxMessageBox("les intersections trouvées n'ont pas pu etre ordonnées.");
        AfxMessageBox("Intersection found cannot be classified.");
        return false;
    }
    else {
        H3_ARRAY2D_FLT32 PosPix(m_PointIntersec.GetSize(), 2);
        for (size_t i = 0; i < m_PointIntersec.GetSize(); i++) {
            PosPix(i, 0) = m_PointIntersec[i].x;
            PosPix(i, 1) = m_PointIntersec[i].y;
        }
        CString FileName;
        FileName.Format("Grid%d_Cam%d_IntersectionPoints.txt", m_nNbImage, m_nIndiceCamera);
        FileName = _CalibPaths.CamCalibLogFolder(_CalibPaths._LastCalibPath) + "\\" + FileName;
        PosPix.SaveASCII(FileName);
    }

    //Afficher les points
    DrawIntersecPoints(m_PointIntersec);

    //MAJ bouton
    ActiveBTN(ETAPE_ACCEPTATIONREFU);
    //suite: OnBtnAccepter // OnBtnRefuser

    pApp->DoWaitCursor(0);
    H3DebugInfo(strModule, strFunction, "out");
    return true;
}

H3_ARRAY2D_UINT8 CCameraCalibDlg::GetMask(size_t nLi, size_t nCo)
{
    H3_ARRAY2D_UINT8 buffermask(nLi, nCo);
    m_ImageMasqueMire = H3_ARRAY2D_UINT8(nLi, nCo);

    INT_PTR ip;

    INT_PTR nb = m_List4Points.GetSize();

    CPoint* pts;
    pts = new CPoint[nb];
    float fMoyenneX = 0;
    float fMoyenneY = 0;
    for (ip = 0; ip < nb; ip++)
    {
        pts[ip].x = m_List4Points[ip].x;
        pts[ip].y = m_List4Points[ip].y;

        fMoyenneX += m_List4Points[ip].x;
        fMoyenneY += m_List4Points[ip].y;

    }
    fMoyenneX = fMoyenneX / nb;
    fMoyenneY = fMoyenneY / nb;

    //Tri des 4 points
    int* Angle;
    Angle = new int[nb];
    int* Indice;
    Indice = new int[nb];
    for (ip = 0; ip < nb; ip++)
    {
        Angle[ip] = 4 + atan2(m_List4Points[ip].y - fMoyenneY, m_List4Points[ip].x - fMoyenneX);
        Indice[ip] = ip;

    }

    for (ip = 0; ip < nb; ip++)
    {
        for (int ipp = 0; ipp < nb - 1; ipp++)
        {
            if (Angle[ipp] < Angle[ipp + 1])
            {
                int temp = Angle[ipp];
                Angle[ipp] = Angle[ipp + 1];
                Angle[ipp + 1] = temp;

                temp = Indice[ipp];
                Indice[ipp] = Indice[ipp + 1];
                Indice[ipp + 1] = temp;
            }
        }
    }

    CPoint* pts2;
    pts2 = new CPoint[nb];
    for (ip = 0; ip < nb; ip++)
    {
        for (int ipp = 0; ipp < nb; ipp++)
        {
            if (ip == Indice[ipp])
                pts2[ip] = pts[ipp];
        }
    }

    //Dessin
    CRgn rgn;
    rgn.CreatePolygonRgn(pts2, nb, WINDING);
    size_t is = 0;
    for (size_t il = 0; il < nLi; il++)
        for (size_t ic = 0; ic < nCo; ic++)
        {
            CPoint pt(ic, il);
            if (rgn.PtInRegion(pt))
            {
                buffermask[is] = 1;
                m_ImageMasqueMire[is] = 0;
            }
            else
            {
                buffermask[is] = 0;
                m_ImageMasqueMire[is] = 1;
            }
            is++;
        }

    delete[] Indice;
    delete[] Angle;
    delete[] pts;
    delete[] pts2;
    return buffermask;
}

void CCameraCalibDlg::Draw4Points()
{
    size_t nb = m_List4Points.GetSize();
    if (nb == 4)
        return;

    CDC* pDC;
    pDC = GetDC();

    CPen pen(PS_SOLID, 1, COLOR_ROUGE);
    CPen* pOldCPen;
    pOldCPen = pDC->SelectObject(&pen);

    CRect rcImageWin;
    GetClientRect(rcImageWin);

    CRect rcImageDisplay = m_Display.GetImageRect();

    CRect rcImage;
    m_CImage.GetWindowRect(rcImage);
    ScreenToClient(rcImage);
    for (size_t i = 0; i < nb; i++)
    {

        H3_POINT2D_FLT32 pt, px;
        pt = m_List4Points.GetAt(i);
        px.x = ((int)((((float)(pt.x)) / ((float)(m_aBufImage.GetCo())) * ((float)(rcImageDisplay.Width()))) + ((float)(rcImageDisplay.left))));
        px.y = ((int)((((float)(pt.y)) / ((float)(m_aBufImage.GetLi())) * ((float)(rcImageDisplay.Height()))) + ((float)(rcImageDisplay.top))));
        px.x = px.x + rcImage.left;
        px.y = px.y + rcImage.top;

        if (i != 0)
            pDC->LineTo(px.x, px.y);

        pDC->MoveTo(px.x - 3, px.y);
        pDC->LineTo(px.x + 3, px.y);
        pDC->MoveTo(px.x, px.y - 3);
        pDC->LineTo(px.x, px.y + 3);

        pDC->MoveTo(px.x, px.y);

    }

    pDC->SelectObject(pOldCPen);
    pen.DeleteObject();
    ReleaseDC(pDC);
}

void CCameraCalibDlg::DrawIntersecPoints(H3_ARRAY_PT2DFLT32 PointIntersec)
{
    CDC* pDC;
    pDC = GetDC();

    CPen penRouge(PS_SOLID, 2, COLOR_ROUGE);
    CPen penVert(PS_SOLID, 2, COLOR_VERT);
    CPen penBleu(PS_SOLID, 2, COLOR_BLEU);

    CPen* pOldCPen;
    pOldCPen = pDC->SelectObject(&penRouge);

    CRect rcImageWin;
    GetClientRect(rcImageWin);

    CRect rcImageDisplay = m_Display.GetImageRect();

    CRect rcImage;
    m_CImage.GetWindowRect(rcImage);
    ScreenToClient(rcImage);
    size_t nb = PointIntersec.GetSize();
    for (size_t i = 0; i < nb; i++)
    {
        if (i == 0)
            pDC->SelectObject(&penVert);
        else if (i == 1)
            pDC->SelectObject(&penBleu);
        else
            pDC->SelectObject(&penRouge);
        H3_POINT2D_FLT32 pt, px;
        pt = PointIntersec.GetAt(i);
        px.x = ((int)((((float)(pt.x)) / ((float)(m_aBufImage.GetCo())) * ((float)(rcImageDisplay.Width()))) + ((float)(rcImageDisplay.left))));
        px.y = ((int)((((float)(pt.y)) / ((float)(m_aBufImage.GetLi())) * ((float)(rcImageDisplay.Height()))) + ((float)(rcImageDisplay.top))));
        px.x = px.x + rcImage.left;
        px.y = px.y + rcImage.top;

        pDC->MoveTo(px.x - 5, px.y);
        pDC->LineTo(px.x + 5, px.y);
        pDC->MoveTo(px.x, px.y - 5);
        pDC->LineTo(px.x, px.y + 5);
    }

    pDC->SelectObject(pOldCPen);

    penRouge.DeleteObject();
    penVert.DeleteObject();
    penBleu.DeleteObject();
    pOldCPen->DeleteObject();

    ReleaseDC(pDC);
}

void CCameraCalibDlg::SetTitre()
{
    CString strTitre;
    strTitre.Format("Calibrage de la caméra %d - Nombre d'images acceptées %d - Nombre d'images minimum requise %d",
        m_nIndiceCamera, m_nNbImage, m_nNombreImageMinCalib);
    SetWindowText(strTitre);
}

//////////////////////////////////////////////////////////////////////
H3_ARRAY2D_PT2DFLT32 CCameraCalibDlg::AddImage(H3_ARRAY2D_UINT8& ImageMasque)
{
    H3_ARRAY2D_PT2DFLT32 ListPoint(0, 0);

    if (!m_pCameraCalib->AddImage(ImageMasque, &ListPoint)) {
        m_bGoodIntersection = false;
    }
    else {
        if (ListPoint.GetSize() != m_pCameraCalib->GetMireLi() * m_pCameraCalib->GetMireCo())
            m_bGoodIntersection = false;
        else
            m_bGoodIntersection = true;
    }
    return ListPoint;
}

void CCameraCalibDlg::RemoveImage()
{
    m_pCameraCalib->RemoveLastImage();
}

bool CCameraCalibDlg::Calibrage()
{
    CString strFunction = "Calibrage";
    CWinApp* pApp = AfxGetApp();
    pApp->DoWaitCursor(1);

    bool bValid = m_pCameraCalib->Calib();

    pApp->DoWaitCursor(0);
    return bValid;
}

void CCameraCalibDlg::SetMireFile(CString MireFile)
{
    CString strFunction = "SetMireFile";
    CWinApp* pApp = AfxGetApp();
    pApp->DoWaitCursor(1);

    m_pCameraCalib->MireInit(MireFile);

    pApp->DoWaitCursor(0);
    return;
}

bool CCameraCalibDlg::LoadSettings(const CString& strFileName)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    bool b;

    CString strFunction = "LoadSettings";
    CWinApp* pApp = AfxGetApp();
    pApp->DoWaitCursor(1);

    //__________________________________________________________________________________________________________________
    m_nIndiceCamera = H3GetPrivProfileInt("CameraCalibDlg", _T("nIndiceCamera"), strFileName);
    m_nNombreImageMinCalib = H3GetPrivProfileInt("CameraCalibDlg", _T("nNombreImageMinCalib"), strFileName);
    m_bSaveCalib = H3GetPrivProfileInt("CameraCalibDlg", _T("nbSaveCalibVideo"), strFileName);
    m_nGrabSource = H3GetPrivProfileInt("CameraCalibDlg", _T("nGrabSource"), strFileName);
    //__________________________________________________________________________________________________________________
    m_strSavePath = _CalibPaths.CamCalibIntrinsicParamsPath(_CalibPaths._LastCalibPath);

    b = m_pCameraCalib->LoadSettings(strFileName);

    if (!b)
        H3DebugInfo(strModule, strFunction, _T("erreur lors du chargement des données depuis: ") + strFileName);

    pApp->DoWaitCursor(0);
    return b;
}

bool CCameraCalibDlg::SaveSettings(const CString& strFileName)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    //bool b;

    CWinApp* pApp = AfxGetApp();
    pApp->DoWaitCursor(1);

    //__________________________________________________________________________________________________________________
    H3WritePrivProfileInt("CameraCalibDlg", _T("nGrabSource"), m_nGrabSource, strFileName);
    //__________________________________________________________________________________________________________________

    //b=m_pCameraCalib->SaveSettings(strFileName);

    pApp->DoWaitCursor(0);

    return true;
}

/*
void CCameraCalibDlg::OnSizing(UINT fwSide, LPRECT pRect)
{
    CDialog::OnSizing(fwSide, pRect);

    EASYSIZE_MINSIZE(614, 426, fwSide, pRect);

}

void CCameraCalibDlg::OnSize(UINT nType, int cx, int cy)
{
    CDialog::OnSize(nType, cx, cy);

    UPDATE_EASYSIZE;
}
*/

void CCameraCalibDlg::OnTimer(UINT_PTR nIDEvent)
{
    switch (nIDEvent)
    {
    case 1:
        KillTimer(1);
        m_Display.Draw(&m_CImage);
        break;
    default:
        break;
    }

    CDialog::OnTimer(nIDEvent);
}

void CCameraCalibDlg::OnOK()
{
    if (m_bSaveRepport && !m_strSaveRepport.IsEmpty())
    {
        CH3CameraCalib* pCameraCalib = nullptr;

        CString strMire = m_pCameraCalib->GetMireFileName();

        FILE* pstream = fopen(m_strSaveRepport, "a+t");
        if (pstream)
        {
            fprintf(pstream, "Calibrage de la caméra %d : \n", m_nIndiceCamera);
            fprintf(pstream, "Fichier de définition de la mire de calibrage %s : \n", (LPCTSTR)strMire);
            fprintf(pstream, "Nombre de positions de la mire %zu : \n", m_nNbImage);
            fprintf(pstream, "\tParamètres intrinsèques\n");
            fprintf(pstream, "\t\t- Distance focale x : %f\n", m_pCameraCalib->fc[0]);
            fprintf(pstream, "\t\t- Distance focale y : %f\n", m_pCameraCalib->fc[1]);
            fprintf(pstream, "\t\t- Centre optique x : %f\n", m_pCameraCalib->cc[0]);
            fprintf(pstream, "\t\t- Centre optique y : %f\n", m_pCameraCalib->cc[1]);
            fprintf(pstream, "\t\t- Distortions de rang 1 : %f\n", m_pCameraCalib->kc[0]);
            fprintf(pstream, "\t\t- Distortions de rang 2 : %f\n", m_pCameraCalib->kc[1]);
            fprintf(pstream, "\t\t- Distortions de rang 3 : %f\n", m_pCameraCalib->kc[2]);
            fprintf(pstream, "\t\t- Distortions de rang 4 : %f\n", m_pCameraCalib->kc[3]);
            fprintf(pstream, "\t\t- Distortions de rang 5 : %f\n", m_pCameraCalib->kc[4]);
            fprintf(pstream, "\tRésidus de minimisation\n");
            fprintf(pstream, "\t\t- Distance focale x : %f\n", m_pCameraCalib->fc_erreur[0]);
            fprintf(pstream, "\t\t- Distance focale y : %f\n", m_pCameraCalib->fc_erreur[1]);
            fprintf(pstream, "\t\t- Centre optique x : %f\n", m_pCameraCalib->cc_erreur[0]);
            fprintf(pstream, "\t\t- Centre optique y : %f\n", m_pCameraCalib->cc_erreur[1]);
            fprintf(pstream, "\t\t- Distortions de rang 1 : %f\n", m_pCameraCalib->kc_erreur[0]);
            fprintf(pstream, "\t\t- Distortions de rang 2 : %f\n", m_pCameraCalib->kc_erreur[1]);
            fprintf(pstream, "\t\t- Distortions de rang 3 : %f\n", m_pCameraCalib->kc_erreur[2]);
            fprintf(pstream, "\t\t- Distortions de rang 4 : %f\n", m_pCameraCalib->kc_erreur[3]);
            fprintf(pstream, "\t\t- Distortions de rang 5 : %f\n", m_pCameraCalib->kc_erreur[4]);
            fprintf(pstream, "\tErreur de reprojection\n");
            fprintf(pstream, "\t\t- Pixels x : %f\n", m_pCameraCalib->pix_erreur[0]);
            fprintf(pstream, "\t\t- Pixels y : %f\n", m_pCameraCalib->pix_erreur[1]);
            fclose(pstream);
        }
    }
    CDialog::OnOK();
}

void CCameraCalibDlg::ReDrawScene()
{
    m_Display.Draw(&m_CImage);
    if (GetEtape() == ETAPE_SELECTION_4POINTS)
        Draw4Points();

    if (GetEtape() == ETAPE_ACCEPTATIONREFU)
        DrawIntersecPoints(m_PointIntersec);
}

void CCameraCalibDlg::OnBtnLoadCorrespList2()
{
    CString strFunction = _T("OnBtnOpenfile");

    // Source -> Fichier contenant les intersection
    if (m_nNbImage == 0)
    {
        CFileDialog FileOpenDlg(TRUE, "txt", NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, "Fichiers txt (descriptif camera) (*.TXT)|*.txt|*.asc||");

        if (FileOpenDlg.DoModal() == IDOK)
        {
            long nx = H3GetPrivProfileLong("CameraParams", "nx", FileOpenDlg.GetPathName());
            long ny = H3GetPrivProfileLong("CameraParams", "ny", FileOpenDlg.GetPathName());

            if (nx < 0 || ny < 0)
            {
                H3DebugInfo(strModule, strFunction, "mauvais parametres (nx/ny)");
                return;
            }
            else
            {
                m_pCameraCalib->InitDefault(nx, ny);
            }
        }
        else
        {
            H3DebugInfo(strModule, strFunction, "Annuler par l'operateur");
            return;
        }
    }
    else
    {
        CString msg;
        msg.Format("le nombre de fichier image deja chargé est de %d", m_nNbImage);
        AfxMessageBox(msg);
    }

    CCorrespList2 CL;
    {
        CFileDialog FileOpenDlg(TRUE, "txt", NULL, OFN_HIDEREADONLY | OFN_OVERWRITEPROMPT, "Fichiers txt (Liste de Correspondance coord3D/pixel MireXY) (*.TXT)|*.txt|*.asc||");

        if (FileOpenDlg.DoModal() == IDOK)
        {
            bool b = CL.Load(FileOpenDlg.GetPathName());

            if (!b)	//il n'y a probablement que les coord pixels dans le fichier
            {
                H3_ARRAY2D_FLT32 Pix;
                Pix.LoadASCII(FileOpenDlg.GetPathName());
                CL = CCorrespList2(Pix.Trans(), m_pCameraCalib->GetMireMetric());
            }
        }
        else
        {
            H3DebugInfo(strModule, strFunction, "Annuler par l'operateur");
            return;
        }
    }

#if 1
    if (!CL.m_bInitialised)
    {
        CString msg;
        msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 0");
        H3DebugWarning(strModule, strFunction, msg);
        return;
    }
#else
    if (CL.H.GetSize() == 0)
    {
        CString msg;
        msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 1");
        H3DebugWarning(strModule, strFunction, msg);
        return;
    }
    else
    {
        if (!_finite(CL.H(0)))
        {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 2");
            H3DebugWarning(strModule, strFunction, msg);
            return;
        }
        else
        {
            //EP.compute_extrinsic(CL,(*this));
        }
    }
#endif

    m_pCameraCalib->AddImage(CL);

    OnBtnAccepter();
}

void CCameraCalibDlg::SetUse(unsigned long ul)
{
    switch (ul)
    {
    case TO_CALIBRATE_CAM:
        m_nUse = TO_CALIBRATE_CAM;
        break;
    case TO_GET_CORNERS:
        m_nUse = TO_GET_CORNERS;
        break;
    case TO_GET_AREA:
        m_nUse = TO_GET_AREA;
        break;
    default:
        break;
    }
    return;
}

H3_ARRAY2D_PT2DFLT32 CCameraCalibDlg::GetPtIntersect()const
{
    return m_PointIntersec;
}

void CCameraCalibDlg::OnMove(int x, int y)
{
    CDialog::OnMove(x, y);

    //ReDrawScene();
    //m_CImage.Invalidate();
}


H3_ARRAY_PT2DFLT32 CCameraCalibDlg::GetSelectedPoints()const
{
    size_t nbPt = m_List4Points.GetSize(), i;

    H3_ARRAY_PT2DFLT32 RetArray(nbPt);
    if (!m_List4Points.IsEmpty())
    {
        for (i = 0; i < nbPt; i++)
        {
            RetArray[i] = m_List4Points.GetAt(i);
        }
    }
    return RetArray;
}