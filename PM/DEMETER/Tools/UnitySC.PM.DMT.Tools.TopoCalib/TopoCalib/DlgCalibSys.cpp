// DlgCalibSys.cpp : fichier d'implémentation
//

#include "stdafx.h"
#include "NanoCalib.h"
#include "DlgCalibSys.h"
#include "afxdialogex.h"

#include "H3AppToolsDecl.h"
#include "H3_HoloMap_AltaTypeExport.h"
#include "SystemCalibInfoClass.h"
#include <tuple>

#include "FreeImagePlus.h"
#ifdef _DEBUG
#pragma comment (lib , "FreeImaged")
#pragma comment (lib , "FreeImagePlusd")
#else
#pragma comment (lib , "FreeImage")
#pragma comment (lib , "FreeImagePlus")
#endif

/*#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
using namespace cv;
#ifdef _DEBUG
#pragma comment (lib , "opencv_core242d")
#pragma comment (lib , "opencv_highgui242d")
#pragma comment (lib , "opencv_imgproc242d")
#else
#pragma comment (lib , "opencv_core242")
#pragma comment (lib , "opencv_highgui242")
#pragma comment (lib , "opencv_imgproc242")
#endif*/


// Initialisation des paramètres pour le calibrage System 
const CString g_strCalibSystemFile = CString(_CalibPaths._LastCalibPath) + _T("\\") + _CalibPaths._InputSettingsFile;

// Boîte de dialogue CDlgCalibSys

IMPLEMENT_DYNAMIC(CDlgCalibSys, CDialog)
/*
bool DoSearchCross(Point2f* p_pSrc,std::string& sImgPath ,std::string& sTemplatePath)
{
    Mat searcimg ;
    Mat templ ;

    try
    {
        searcimg = imread( sImgPath, CV_LOAD_IMAGE_GRAYSCALE );
    }
    catch (const cv::Exception& ex)
    {
        CString csMsg;
        csMsg.Format("{DoSearchCross} Error \nLoad Img : %s", ex.what());
        AfxMessageBox(csMsg);
        return false;
    }
    if (searcimg.data == NULL)
    {
        CString csMsg;
        csMsg.Format("{DoSearchCross} Error \nCould not Load [%s] \nMissing file, Invalid Path or Not supported format", sImgPath.c_str());
        AfxMessageBox(csMsg);
        return false;
    }

    //threshold(searcimg, searcimg, 40.0, 255.0, THRESH_BINARY);
    //imwrite( "C:\\Altasight\\GlobalTopo\\tempCalib\\ImgTh.bmp", searcimg );

    try
    {
        templ = imread( sTemplatePath, CV_LOAD_IMAGE_GRAYSCALE );
    }
    catch (const cv::Exception& ex)
    {
        CString csMsg;
        csMsg.Format("{DoSearchCross} Error \nLoad Template : %s", ex.what());
        AfxMessageBox(csMsg);
        return false;
    }
    if (templ.data == NULL)
    {
        CString csMsg;
        csMsg.Format("{DoSearchCross} Error \nCould not Load [%s] \nMissing file,Invalid Path or Not Supported format", sTemplatePath.c_str());
        AfxMessageBox(csMsg);
        return false;
    }

    Point2f ptsarray[4];
    /// Create the result matrix
    int result_cols =  searcimg.cols - templ.cols + 1;
    int result_rows = searcimg.rows - templ.rows + 1;
    Mat img_display;
    searcimg.copyTo( img_display );
    cvtColor( img_display, img_display, CV_GRAY2BGR );
    Mat result;
    result.create( result_cols, result_rows, CV_32FC1 );

    /// Do the Matching and Normalize
    // "Method: \n 0: SQDIFF \n 1: SQDIFF NORMED \n 2: TM CCORR \n 3: TM CCORR NORMED \n 4: TM COEFF \n 5: TM COEFF NORMED";
    int match_method =TM_CCOEFF_NORMED;
    matchTemplate( searcimg, templ, result, match_method );

    normalize(result, result, 0, 255.0, NORM_MINMAX, CV_8UC1, Mat());

    //imwrite( "C:\\Altasight\\GlobalTopo\\tempCalib\\matchTemplate.bmp", result );
    Mat resultMask = result;
    threshold(result, resultMask, 180.0, 255.0, THRESH_BINARY);

    //threshold(result, resultMask, 240, 255.0, THRESH_BINARY);
    //imwrite( "C:\\Altasight\\GlobalTopo\\tempCalib\\matchTemplateTH.bmp", resultMask );

    Mat temp = resultMask.clone();
    vector< vector<Point> > contours;
    findContours(temp, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(templ.cols / 2, templ.rows / 2));

    cvtColor( result, result, CV_GRAY2BGR );
    vector< vector<Point> >::iterator i;
    int nCount = 0;
    for(i = contours.begin(); i != contours.end(); i++)
    {
        Moments m = moments(*i, false);
        Point2f centroid(m.m10 / m.m00, m.m01 / m.m00);
        if(nCount >= 0 && nCount < 4)
            ptsarray[nCount] = centroid;
        nCount++;
        circle(img_display, centroid, 10, Scalar(0, 255, 0), 10);
    }

    try
    {
        CString cs = g_csAltasightBaseDirPath + _T("\\tempCalib\\RedressSearchCross.png");
        imwrite((LPCTSTR) cs, img_display);
    }
    catch(...)
    {
        //do nothing
    }

    // order SRC point !!!
    float midx= (float)searcimg.cols/2.0f;
    float midy= (float)searcimg.rows/2.0f;
    for(int i=0; i<nCount||i<4;i++)
    {
        if(ptsarray[i].x < midx && ptsarray[i].y < midy)
        {
            // top left
            p_pSrc[0] =  ptsarray[i];
        }

        if(ptsarray[i].x > midx && ptsarray[i].y < midy)
        {
            // top right
            p_pSrc[1] =  ptsarray[i];
        }

        if(ptsarray[i].x > midx && ptsarray[i].y > midy)
        {
            // bottom right
            p_pSrc[2] =  ptsarray[i];
        }

        if(ptsarray[i].x < midx && ptsarray[i].y > midy)
        {
            // bottom right
            p_pSrc[3] =  ptsarray[i];
        }
    }
    return true;
}
*/
CDlgCalibSys::CDlgCalibSys(CWnd* pParent /*=NULL*/)
    : CDialog(CDlgCalibSys::IDD, pParent)
{
    for (int i = 0; i < NB_IMG_PHASE; i++)
    {
        m_csEditAcq[i] = _T("");
    }
    //m_csEditMask = _T("");
    m_csEditVideo = _T("");

    m_fPitchX = 0.0f;
    m_fPitchY = 0.0f;
    m_fPeriodX = 0.0f;
    m_fPeriodY = 0.0f;
    m_nCrossX = 0;
    m_nCrossY = 0;

    m_nScreenSizeX = 0;
    m_nScreenSizeY = 0;
    m_nScreenRefPosX = 0;
    m_nScreenRefPosY = 0;
}

CDlgCalibSys::~CDlgCalibSys()
{
}

void CDlgCalibSys::DoDataExchange(CDataExchange* pDX)
{
    CDialog::DoDataExchange(pDX);

    DDX_Control(pDX, IDC_EDIT_PITCH_X, m_ctrlEditPitchX);
    DDX_Text(pDX, IDC_EDIT_PITCH_X, m_fPitchX);
    DDX_Control(pDX, IDC_EDIT_PITCH_Y, m_ctrlEditPitchY);
    DDX_Text(pDX, IDC_EDIT_PITCH_Y, m_fPitchY);
    DDX_Control(pDX, IDC_EDIT_PERIOD_X, m_ctrlEditPeriodX);
    DDX_Text(pDX, IDC_EDIT_PERIOD_X, m_fPeriodX);
    DDX_Control(pDX, IDC_EDIT_PERIOD_Y, m_ctrlEditPeriodY);
    DDX_Text(pDX, IDC_EDIT_PERIOD_Y, m_fPeriodY);
    DDX_Control(pDX, IDC_EDIT_CROSS_X, m_ctrlEditCrossX);
    DDX_Text(pDX, IDC_EDIT_CROSS_X, m_nCrossX);
    DDX_Control(pDX, IDC_EDIT_CROSS_Y, m_ctrlEditCrossY);
    DDX_Text(pDX, IDC_EDIT_CROSS_Y, m_nCrossY);

    DDX_Text(pDX, IDC_EDIT_SCREENSZ_X, m_nScreenSizeX);
    DDX_Text(pDX, IDC_EDIT_SCREENSZ_Y, m_nScreenSizeY);
    DDX_Text(pDX, IDC_EDIT_SCREENREFPOS_X, m_nScreenRefPosX);
    DDX_Text(pDX, IDC_EDIT_SCREENREFPOS_Y, m_nScreenRefPosY);

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

    DDX_Control(pDX, IDC_BUTTON_BRSW_VIDEO, m_ctrlBtnBrswVideo);
    DDX_Control(pDX, IDC_EDIT_VIDEO, m_ctrlEditVideo);
    DDX_Text(pDX, IDC_EDIT_VIDEO, m_csEditVideo);

    //DDX_Control(pDX, IDC_BUTTON_BRSW_MASK, m_ctrlBtnBrswMask);
    //DDX_Control(pDX, IDC_EDIT_MASK, m_ctrlEditMask);
    //DDX_Text(pDX, IDC_EDIT_MASK, m_csEditMask);

    DDX_Control(pDX, IDC_BUTTON_CALIBSYS, m_ctrlBtnCalibSys);
}


BEGIN_MESSAGE_MAP(CDlgCalibSys, CDialog)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ1, &CDlgCalibSys::OnBnClickedButtonBrswAcq1)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ2, &CDlgCalibSys::OnBnClickedButtonBrswAcq2)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ3, &CDlgCalibSys::OnBnClickedButtonBrswAcq3)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ4, &CDlgCalibSys::OnBnClickedButtonBrswAcq4)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ5, &CDlgCalibSys::OnBnClickedButtonBrswAcq5)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_ACQ6, &CDlgCalibSys::OnBnClickedButtonBrswAcq6)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_VIDEO, &CDlgCalibSys::OnBnClickedButtonBrswVideo)
    ON_BN_CLICKED(IDC_BUTTON_BRSW_MASK, &CDlgCalibSys::OnBnClickedButtonBrswMask)
    ON_BN_CLICKED(IDC_BUTTON_SAVEPRM2, &CDlgCalibSys::OnBnClickedButtonSaveprm)
    ON_BN_CLICKED(IDC_BUTTON_CALIBSYS, &CDlgCalibSys::OnBnClickedButtonCalibsys)
END_MESSAGE_MAP()



bool CDlgCalibSys::SaveGreyImageFlt32(CString p_csFilepath, CImageFloat* p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
{
    float* pData = p_oMatrixFloat->pData;

    unsigned long  lCols = (unsigned long)p_oMatrixFloat->nCo;
    unsigned long  lLines = (unsigned long)p_oMatrixFloat->nLi;

    //float fMin = FLT_MAX;
    //float fMax = -FLT_MAX;

    //bool bUseMinPrm = (p_fMin != FLT_MAX);
    //bool bUseMaxPrm = (p_fMax != FLT_MAX);

    //float a = 1.0f;
    //float b = 0.0f;
    //if (bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
    //{

    //    for (long lItem = 0; lItem < (long)(lCols * lLines); lItem++)
    //    {
    //        if (!_isnan(pData[lItem]))
    //        {
    //            if (!bUseMinPrm)
    //                fMin = __min(fMin, pData[lItem]);
    //            if (!bUseMaxPrm)
    //                fMax = __max(fMax, pData[lItem]);
    //        }
    //    }
    //}
    //else
    //{
    //    if (!bUseMaxPrm)
    //        fMax = 255.0f;
    //    if (!bUseMinPrm)
    //        fMin = 0.0f;
    //}

    //if (bUseMinPrm)
    //{
    //    fMin = p_fMin;
    //}
    //if (bUseMaxPrm)
    //{
    //    fMax = p_fMax;
    //}

    //if (fMax != fMin)
    //{
    //    a = 255.0f / (fMax - fMin);
    //    b = -fMin * 255.0f / (fMax - fMin);
    //}

    fipImage oImg(FIT_FLOAT, lCols, lLines, 32);
    for (unsigned y = 0; y < oImg.getHeight(); y++)
    {
        //ici pb à resoudre pour affichage image
        float* pbits = (float*)oImg.getScanLine(y);
        for (unsigned x = 0; x < oImg.getWidth(); x++)
        {
            pbits[x] = (pData[y * lCols + x]);
        }
    }
    oImg.flipVertical();
    BOOL bRes = oImg.save(p_csFilepath, TIFF_NONE);
    return (bRes != 0);
}







// Gestionnaires de messages de CDlgCalibSys
void CDlgCalibSys::OnBnClickedButtonCalibsys()
{
    CWaitCursor oWaitCursor;

    UpdateData(TRUE);
    EnabledUI(FALSE);

    SortPhasesImages();
    DoCalibration();
}

bool CDlgCalibSys::ComparePhaseNames(CString leftPhaseName, CString rightPhaseName, bool bAscending)
{
    bool bIsHigher = false;
    if (GetPeriodFromPhaseName(leftPhaseName) > GetPeriodFromPhaseName(rightPhaseName))
        bIsHigher = true;
    else
        if ((GetPeriodFromPhaseName(leftPhaseName) == GetPeriodFromPhaseName(rightPhaseName)) && (GetOrientationFromPhaseName(leftPhaseName).Compare(GetOrientationFromPhaseName(rightPhaseName)) > 0))
            bIsHigher = true;

    return (bAscending ? !bIsHigher : bIsHigher);
}

CString CDlgCalibSys::GetOrientationFromPhaseName(CString phaseName)
{
    // the fileName must be Im[Period]pix_Phase_[X or Y].TIFF
    int period = 0;
    CString orientation;

    CString fileName = phaseName.Mid(phaseName.ReverseFind('\\') + 1);

    char charOrientation;

    sscanf(fileName, _T("Im%dpix_Phase_%c"), &period, &charOrientation);

    orientation = charOrientation;
    return orientation;
}

int CDlgCalibSys::GetPeriodFromPhaseName(CString phaseName)
{
    // the fileName must be Im[Period]pix_Phase_[X or Y].TIFF
    int period = 0;

    CString fileName = phaseName.Mid(phaseName.ReverseFind('\\') + 1);
    char charOrientation;
    sscanf(fileName, _T("Im%dpix_Phase_%c"), &period, &charOrientation);
    return period;
}

void CDlgCalibSys::SortPhasesImages()
{
    bool bAscending = true;
    // We sort the phase files
    std::sort(
        (CString*)&m_csEditAcq,                                           // position of first element
        ((CString*)&m_csEditAcq) + NB_IMG_PHASE,                           // position one past the final element 
        [bAscending, this]
    (const CString& left, const CString& right)              // lambda formal parameters list
        { return ComparePhaseNames(left, right, bAscending); } // lambda body
    );

    int periods[3] = { 0,0,0 };
    for (int i = 0; i < NB_IMG_PHASE; i++)
    {
        int period = GetPeriodFromPhaseName(m_csEditAcq[i]);
        // add the period to the array if it is not already there
        if ((periods[0] != period) && (periods[1] != period) && (periods[2] != period))
        {
            // it is a new period, we add it
            if (periods[0] == 0)
                periods[0] = period;
            else
                if (periods[1] == 0)
                    periods[1] = period;
                else
                    if (periods[2] == 0)
                        periods[2] = period;
        }
    }

    // we set the ratios between the phase periods
    m_arrayRatios[0] = 1;
    m_arrayRatios[1] = periods[1] / periods[0];
    m_arrayRatios[2] = periods[2] / periods[1];
}

bool CDlgCalibSys::DoCalibration()
{
    float fPixSizeX = m_fPitchX;
    float fPixSizeY = m_fPitchY;
    float fMireMonStepX = m_fPeriodX;
    float fMireMonStepY = m_fPeriodY;
    unsigned int nCrossX = m_nCrossX;
    unsigned int nCrossY = m_nCrossY;

    // Chargement des images de phase (W) X1 Y1 X2 Y2 X3 Y3

    // Initialisation structure de données
    CImageFloat* pArrayOfWPicture = new CImageFloat[NB_IMG_PHASE];
    for (size_t i = 0; i < NB_IMG_PHASE; i++)
    {
        // If the file is a tif
        CString imagePath = m_csEditAcq[i];

        if (imagePath.Find("TIFF", imagePath.GetLength() - 4) >= 0 || imagePath.Find("tiff", imagePath.GetLength() - 4) >= 0 || imagePath.Find("Tiff", imagePath.GetLength() - 4) >= 0)
        {
            fipImage image;

            image.load(imagePath);

            //          image.save("c:\\temp\\BeforeFlip.TIF");

            image.flipVertical();

            //         image.save("c:\\temp\\AfterFlip.TIF");

            float* pRawLine = 0;
            BYTE* pCur = 0;
            int nHeight = image.getHeight();
            int nWidth = image.getWidth();
            float* pRawImage = new float[nWidth * nHeight];

            int nPitch = image.getScanWidth();
            for (int l = nHeight - 1; l >= 0; l--)
            {
                pRawLine = (float*)(pRawImage + (l * nWidth));
                pCur = image.getScanLine(l);
                int lineSize = nWidth * sizeof(float);
                memcpy(pRawLine, pCur, lineSize); // Copie d'une ligne
            }
            pArrayOfWPicture[i] = CImageFloat(pRawImage, nWidth, nHeight);

            // We save a TIF file for debug
            //CString path = m_csEditAcq[i];
            //CString fileName = path.Mid(path.ReverseFind('\\') + 1);
            //CString fileNameWoExt = fileName.Left(fileName.ReverseFind('.'));
            //SaveGreyImageFlt32("c:\\temp\\"+ fileNameWoExt+".TIF", &pArrayOfWPicture[i]);
        }
        else
        {
            // If the file is a bin
            if (pArrayOfWPicture[i].Load(m_csEditAcq[i]) == false)
            {
                CString csMsg;
                csMsg.Format(_T("Could not load Phase Image n°%d : {%s}\n Wrong Path or file extension"), i + 1, m_csEditAcq[i]);
                AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
                EnabledUI(TRUE);
                return false;
            }


            // We save a TIF file for debug
            //CString path = m_csEditAcq[i];
            //CString fileName = path.Mid(path.ReverseFind('\\') + 1);
            //CString fileNameWoExt = fileName.Left(fileName.ReverseFind('.'));
            //SaveGreyImageFlt32("c:\\temp\\"+ fileNameWoExt+".TIF", &pArrayOfWPicture[i]);
        }

        // pour convertir image en hbf
    /*	H3_ARRAY2D_FLT32 oHbf;
        oHbf.LinkData(pArrayOfWPicture[i].nLi,pArrayOfWPicture[i].nCo,pArrayOfWPicture[i].pData);
        CString sNewName =m_csEditAcq[i];
        BOOL b = PathRenameExtension(sNewName.GetBuffer(),_T(".hbf"));
        ASSERT(b);
        sNewName.ReleaseBuffer();
        oHbf.Save(sNewName);*/

    }

    // Chargement de l'image vidéo
    CImageByte mireImage;
    fipImage oImgVideo;
    if (!oImgVideo.load(m_csEditVideo))
    {
        CString csMsg;
        csMsg.Format(_T("Could not load Video Image : {%s}\n Wrong Path or file extension"), m_csEditVideo);
        AfxMessageBox(csMsg, MB_OK | MB_ICONERROR);
        EnabledUI(TRUE);
        return false;
    }
    else
    {
        mireImage.nLi = oImgVideo.getHeight();
        mireImage.nCo = oImgVideo.getWidth();
        mireImage.pData = new BYTE[mireImage.nLi * mireImage.nCo];
        oImgVideo.flipVertical();
        // cette methode comporte une faille dans le cas ou la scanwidth est superieur à la width.
        //memcpy(mireImage.pData,oImgVideo.accessPixels(),mireImage.nLi*mireImage.nCo*sizeof(BYTE));

        // cette methode copy ligne à ligne selon la taille de la width en ne negligeant pas le pitch!!
        BYTE* pRawLine = 0;
        BYTE* pCur = 0;
        int nHeight = oImgVideo.getHeight();
        int nWidth = oImgVideo.getWidth();
        int nPitch = oImgVideo.getScanWidth();
        for (int l = nHeight - 1; l >= 0; l--)
        {
            pRawLine = (BYTE*)(mireImage.pData + (l * nWidth));
            pCur = oImgVideo.getScanLine(l);
            memcpy(pRawLine, pCur, nWidth * sizeof(BYTE)); // Copie d'une ligne
        }

        // pour convertir image en hbf
    /*	H3_ARRAY2D_UINT8 oHbf;
        oHbf.LinkData(mireImage.nLi,mireImage.nCo,mireImage.pData);
        CString sNewName =m_csEditVideo;
        PathRenameExtension(sNewName.GetBuffer(),_T(".hbf"));
        sNewName.ReleaseBuffer();
        oHbf.Save(sNewName);*/


        // Utilisée pour lA Nanocalibration seuleument
/*		Point2f src[4];
        string sImgPath = (LPCTSTR)m_csEditVideo;
        string sTemplateCross = g_csAltasightBaseDirPath + _T("\\CalibTemplCross.png");
        if ( false == DoSearchCross(&src[0],sImgPath, sTemplateCross))
        {
            EnabledUI(TRUE);
            return;
        }

        CString strSection;
        strSection=_T("Alta_RedressMatrix");
        H3WritePrivProfileFloat(strSection,_T("TL0"),src[0].x,g_strCalibSystemFile); // Top left cross position X
        H3WritePrivProfileFloat(strSection,_T("TL1"),src[0].y,g_strCalibSystemFile); // Top left cross position Y
        H3WritePrivProfileFloat(strSection,_T("TR0"),src[1].x,g_strCalibSystemFile); // Top right cross position X
        H3WritePrivProfileFloat(strSection,_T("TR1"),src[1].y,g_strCalibSystemFile); // Top right cross position Y
        H3WritePrivProfileFloat(strSection,_T("BR0"),src[2].x,g_strCalibSystemFile); // bottom right cross position X
        H3WritePrivProfileFloat(strSection,_T("BR1"),src[2].y,g_strCalibSystemFile); // bottom right cross position Y
        H3WritePrivProfileFloat(strSection,_T("BL0"),src[3].x,g_strCalibSystemFile); // bottom left cross position X
        H3WritePrivProfileFloat(strSection,_T("BL1"),src[3].y,g_strCalibSystemFile); // bottom left cross position Y
        H3WritePrivProfileInt(strSection,_T("WfLen"),300000,g_strCalibSystemFile);	//wafer lenght in µm;
        H3WritePrivProfileInt(strSection,_T("ImWi"),3600,g_strCalibSystemFile);		//image width in px;
        H3WritePrivProfileInt(strSection,_T("ImHe"),3248,g_strCalibSystemFile);		//image height in px*/
    }

    // Chargement de l'image masque
/*	CImageByte crossImage;
    fipImage oImgMask;
    if( ! oImgMask.load(m_csEditMask))
    {
        CString csMsg;
        csMsg.Format(_T("Could not load Video Mask : {%s}\n Wrong Path or file extension"), m_csEditMask);
        AfxMessageBox(csMsg,MB_OK|MB_ICONERROR);
        EnabledUI(TRUE);
        return;
    }
    else
    {
        crossImage.nLi = oImgMask.getHeight();
        crossImage.nCo = oImgMask.getWidth();
        crossImage.pData = new BYTE[crossImage.nLi*crossImage.nCo];
        oImgMask.flipVertical();
        memcpy(crossImage.pData,oImgMask.accessPixels(),mireImage.nLi*mireImage.nCo*sizeof(BYTE));
    }*/


    // Calibrage système
    CSystemCalibInfoClass	SystemCalib(CString(_CalibPaths._LastCalibPath) + "\\");
    /*float ratio[3]={1,16,8};*/


    unsigned long pixRef_Xscreen = m_nScreenRefPosX; // /!\ Paramètres n'existant pas dans la version NanoTopography de 2013
    unsigned long pixRef_Yscreen = m_nScreenRefPosY;// /!\ Paramètres n'existant pas dans la version NanoTopography de 2013
    unsigned long screen_Xsz = m_nScreenSizeX;	// /!\ Paramètres n'existant pas dans la version NanoTopography de 2013
    unsigned long screen_Ysz = m_nScreenSizeY;	// /!\ Paramètres n'existant pas dans la version NanoTopography de 2013

    SystemCalib.SetData(pArrayOfWPicture,
        &mireImage,
        fPixSizeX, fPixSizeY,
        fMireMonStepX, fMireMonStepY,
        pixRef_Xscreen, pixRef_Yscreen, // Paramètres n'existant pas dans la version NanoTopography de 2013
        screen_Xsz, screen_Ysz,			// Paramètres n'existant pas dans la version NanoTopography de 2013
        nCrossX, nCrossY, NB_IMG_PHASE, m_arrayRatios);

    int nAppreciationCalibSystem = -1;
    SystemCalib.CalibrageSystem(nAppreciationCalibSystem);
    CString strMsg;
    strMsg = _T("");
    switch (nAppreciationCalibSystem)
    {
    case 0:
        // strMsg = CString(_T("Le calibrage système a réussi."));
        strMsg = CString(_T("System calibration is successful."));
        AfxMessageBox(strMsg);
        g_bInitCalibSys = true;

        break;
    case 1:
        // 		strMsg = CString(_T("Le chemin d'accès au fichier CalibCam_0.txt n'est pas renseigné dans le fichier SensorData.txt.\n"));
        // 		strMsg+= CString(_T("Voir section [SensorHoloMap3] paramètre str_CamFile"));
        strMsg = CString(_T("CalibCam_0.txt file access is not present in SensorData.txt file.\n"));
        strMsg += CString(_T("See [SensorHoloMap3] section, str_CamFile parameter"));
        AfxMessageBox(strMsg);
        break;
    case 2:
        // 		strMsg = CString(_T("Le chemin d'accès au fichier EP_ref_CamFrame.txt n'est pas renseigné dans le fichier SensorData.txt.\n"));
        // 		strMsg+= CString(_T("Voir section [SensorHoloMap3] paramètre ep_ObjRef_CamFrame_File"));
        strMsg = CString(_T("EP_ref_CamFrame.txt file access is not present in SensorData.txt file.\n"));
        strMsg += CString(_T("See [SensorHoloMap3] section, ep_ObjRef_CamFrame_File parameter"));
        AfxMessageBox(strMsg);
        break;
    case 21:
        //		strMsg = CString(_T("Le chemin d'accès au fichier Res1.txt n'est pas renseigné dans le fichier SensorData.txt.\n"));
        //		strMsg+= CString(_T("Voir section [SensorHoloMap3] paramètre str_epFile"));
        strMsg = CString(_T("Res1.txt file access is not present in SensorData.txt file.\n"));
        strMsg += CString(_T("See [SensorHoloMap3] section, str_epFile parameter"));
        AfxMessageBox(strMsg);
        break;
    case 22:
        //		strMsg = CString(_T("Le chemin d'accès au fichier ResX.klib n'est pas renseigné dans le fichier SensorData.txt.\n"));
        //		strMsg+= CString(_T("Voir section [SensorHoloMap3] paramètre str_PhiXFile"));
        strMsg = CString(_T("ResX.klib file access is not present in SensorData.txt file.\n"));
        strMsg += CString(_T("See [SensorHoloMap3] section, str_PhiXFile parameter"));
        AfxMessageBox(strMsg);
        break;
    case 23:
        //		strMsg = CString(_T("Le chemin d'accès au fichier ResY.klib n'est pas renseigné dans le fichier SensorData.txt.\n"));
        //		strMsg+= CString(_T("Voir section [SensorHoloMap3] paramètre str_PhiYFile"));
        strMsg = CString(_T("ResY.klib file access is not present in SensorData.txt file.\n"));
        strMsg += CString(_T("See [SensorHoloMap3] section, str_PhiYFile parameter"));
        AfxMessageBox(strMsg);
        break;
    case 3:
        //		strMsg = CString(_T("Le fichier d'initialisation de la caméra est incomplet."));
        strMsg = CString(_T("Camera initialisation file is incomplete."));
        AfxMessageBox(strMsg);
        break;
    case 4:
        //		strMsg = CString(_T("Les paramètres extrinsèques du wafer dans le repère caméra sont invalides."));
        strMsg = CString(_T("Wafer extrinsic parameters in camera landmark are invalid."));
        AfxMessageBox(strMsg);
        break;
    case 5:
        //		strMsg = CString(_T("Au moins un des paramètres d'entrées est invalide."));
        strMsg = CString(_T("At least one entry parameter is invalid."));
        AfxMessageBox(strMsg);
        break;
    case 6:
        //		strMsg = CString(_T("Il n'y a pas assez de pixels valides sur les images de phases."));
        strMsg = CString(_T("There are not enough valid pixels for phasis pictures."));
        AfxMessageBox(strMsg);
        break;
    case 7:
        //		strMsg = CString(_T("Les cartes de phases en X ou Y acquisent sur le wafer de référence sont trop bruitées."));
        strMsg = CString(_T("X or Y axis phasis data for reference wafer are too noisy."));
        AfxMessageBox(strMsg);
        break;
    case 8:
        //		strMsg = CString(_T("Le dossier Calib_Sys situé dans C:\\altasight\\Nano\\ n'existe pas."));
        strMsg = CString(_T("Calib_Sys folder does not exist in ")) + _CalibPaths._LastCalibPath;
        AfxMessageBox(strMsg);
        break;
    case 9:
        //		strMsg = CString(_T("Une erreur s'est produite lors du calibrage système."));
        strMsg = CString(_T("An error occured during system calibration."));
        AfxMessageBox(strMsg);
        break;
    case 10:
        //		strMsg = CString(_T("Impossible de charger le fichier SensorData.txt se trouvant dans le dossier c:\\altasight\\Globaltopo\\."));
        strMsg = CString(_T("Impossible to load SensorData.txt file in C:\\altasight\\Globaltopo\\."));
        AfxMessageBox(strMsg);
        break;
    case 11:
        //		strMsg = CString(_T("L'entrée 'str_CamFile' dans le fichier 'C:\\AltaSight\\Globaltopo\\SensorData.txt' n'est pas renseignée!...\n"));
        strMsg = CString(_T(" 'str_CamFile' entry in 'C:\\AltaSight\\Globaltopo\\SensorData.txt' does not exist...\n"));
        strMsg += CString(_T("str_CamFile=C:\\AltaSight\\Globaltopo\\Calib_cam\\CalibCam_0.txt"));
        AfxMessageBox(strMsg);
        break;
    case 12:
        //		strMsg = CString(_T("L'entrée 'ep_ObjRef_CamFrame_File' dans le fichier 'C:\\AltaSight\\Globaltopo\\SensorData.txt' n'est pas renseignée!...\n"));
        strMsg = CString(_T(" 'ep_ObjRef_CamFrame_File' entry in 'C:\\AltaSight\\Globaltopo\\SensorData.txt' does not exist...\n"));
        strMsg += CString(_T("ep_ObjRef_CamFrame_File=C:\\AltaSight\\Globaltopo\\Calib_cam\\EP_ref_CamFrame.txt"));
        AfxMessageBox(strMsg);
        break;
    case 13:
        //		strMsg = CString(_T("Annulation de la procédure de calibrage système par l'opérateur!..."));
        strMsg = CString(_T("System calibration procedure was cancelled by operator !..."));
        AfxMessageBox(strMsg);
        break;
    case 14:
        //		strMsg = CString(_T("Le fichier 'C:\\AltaSight\\Globaltopo\\SensorData.txt' ou 'C:\\AltaSight\\Globaltopo\\Calib_cam\\EP_ref_CamFrame.txt' n'existe pas!..."));
        strMsg = CString(_T("'C:\\AltaSight\\Globaltopo\\SensorData.txt' or 'C:\\AltaSight\\Globaltopo\\Calib_cam\\EP_ref_CamFrame.txt' folder does not exist !..."));
        AfxMessageBox(strMsg);
        break;
    case 15:
        //strMsg = CString(_T("Erreur lors de la récupération des paramètres extrinsèques de la mire!..."));
        strMsg = CString(_T("Unable to get grid extrinsinc parameters!..."));
        AfxMessageBox(strMsg);
        break;
    default:
        break;
    }
    //
    // Libérer mémoire
    if (pArrayOfWPicture != nullptr)
    {
        for (size_t i = 0; i < NB_IMG_PHASE; i++)
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
    // 	if (crossImage.pData!=nullptr)
    // 	{
    // 		delete[] crossImage.pData;
    // 		crossImage.pData = nullptr;
    // 	}


        /*
        CString strMsg;
        strMsg = _T("");
        switch (nAppreciationCalibSystem)
        {
        case 0:
            AfxMessageBox("System Calibration is SUCCESSFUL.");
            break;
        case 1:
            AfxMessageBox("ERROR 1.\n\n"+strMsg);
            break;
        case 2:
            AfxMessageBox("ERROR 2.\n\n"+strMsg);
            break;
        case 3:
            AfxMessageBox("ERROR 3");
            break;
        default:
            break;
        }*/

    if (!_silentMode)
    {
        EnabledUI(TRUE);
    }

    return (nAppreciationCalibSystem == 0);
}

void CDlgCalibSys::EnabledUI(BOOL p_bEnable)
{
    m_ctrlEditPitchX.EnableWindow(p_bEnable);
    m_ctrlEditPitchY.EnableWindow(p_bEnable);
    m_ctrlEditPeriodX.EnableWindow(p_bEnable);
    m_ctrlEditPeriodY.EnableWindow(p_bEnable);
    m_ctrlEditCrossX.EnableWindow(p_bEnable);
    m_ctrlEditCrossY.EnableWindow(p_bEnable);

    for (int i = 0; i < NB_IMG_PHASE; i++)
    {
        m_ctrlEditAcq[i].EnableWindow(p_bEnable);
        m_ctrlBtnBrsw[i].EnableWindow(p_bEnable);
    }
    m_ctrlBtnBrswVideo.EnableWindow(p_bEnable);
    m_ctrlEditVideo.EnableWindow(p_bEnable);
    m_ctrlBtnBrswMask.EnableWindow(p_bEnable);
    m_ctrlEditMask.EnableWindow(p_bEnable);

    m_ctrlBtnCalibSys.EnableWindow(p_bEnable);
}

BOOL CDlgCalibSys::OnInitDialog()
{
    CDialog::OnInitDialog();

    CString strSection;
    strSection = _T("SensorHoloMap3_SensorScreen");
    m_fPitchX = H3GetPrivProfileFloat(strSection, _T("PitchX"), g_strCalibSystemFile);
    m_fPitchY = H3GetPrivProfileFloat(strSection, _T("PitchY"), g_strCalibSystemFile);
    m_fPeriodX = H3GetPrivProfileFloat(strSection, _T("PeriodX"), g_strCalibSystemFile);
    m_fPeriodY = H3GetPrivProfileFloat(strSection, _T("PeriodY"), g_strCalibSystemFile);

    m_nScreenSizeX = H3GetPrivProfileInt(strSection, _T("SizeX"), g_strCalibSystemFile);
    m_nScreenSizeY = H3GetPrivProfileInt(strSection, _T("SizeY"), g_strCalibSystemFile);
    m_nScreenRefPosX = H3GetPrivProfileInt(strSection, _T("pixRefX"), g_strCalibSystemFile);
    m_nScreenRefPosY = H3GetPrivProfileInt(strSection, _T("pixRefY"), g_strCalibSystemFile);


    strSection = _T("SensorHoloMap3");
    m_nCrossX = H3GetPrivProfileInt(strSection, _T("PrefX"), g_strCalibSystemFile);
    m_nCrossY = H3GetPrivProfileInt(strSection, _T("PrefY"), g_strCalibSystemFile);

    for (int i = 0; i < NB_IMG_PHASE; i++)
    {
        CString csImgAcqKey;
        csImgAcqKey.Format(_T("PathPhaseAcq%d"), i + 1);
        //csDefaultPath.Format(_T("%s\\MultiPhi4Calibrage\\Phase%d_%d.bin"),g_csAltasightBaseDirPath,i+1,i+1);
        m_csEditAcq[i] = H3GetPrivProfileString(strSection, csImgAcqKey, g_strCalibSystemFile);
    }
    //csDefaultPath = g_csAltasightBaseDirPath + _T("\\MultiPhi4Calibrage\\video.tif");
    m_csEditVideo = H3GetPrivProfileString(strSection, _T("PathImgVideo"), g_strCalibSystemFile);
    //csDefaultPath = g_csAltasightBaseDirPath +_T("\\MultiPhi4Calibrage\\Mask.bmp");
    //m_csEditMask = H3GetPrivProfileString(strSection, _T("PathImgMask"), g_strCalibSystemFile);

    UpdateData(FALSE);

    return TRUE;  // return TRUE unless you set the focus to a control
    // EXCEPTION : les pages de propriétés OCX devraient retourner FALSE
}


void CDlgCalibSys::OnOK()
{
    // Do nothing to avoid exit panel
    //CDialog::OnOK();
}


void CDlgCalibSys::OnCancel()
{
    // Do nothing to avoid exit panel
    //CDialog::OnCancel();
}


void CDlgCalibSys::WinHelp(DWORD dwData, UINT nCmd)
{
    // Do nothing to avoid F1 mess
    //CDialog::WinHelp(dwData, nCmd);
}


void CDlgCalibSys::OnBnClickedButtonBrswAcq1()
{

    /*Point2f src[4];
    string sImgPath = "C:\\Altasight\\Data\\Data_Calib_SEH_oct12\\flipper_Ref-MotifX.tif";
    string sTemplateCross = g_csAltasightBaseDirPath + _T("\\CalibTemplX.png");
    if ( false == DoSearchCross(&src[0],sImgPath, sTemplateCross))
    {
        EnabledUI(TRUE);
        return;
    }*/

    BrowsePhaseImg(0);
}
void CDlgCalibSys::OnBnClickedButtonBrswAcq2()
{
    BrowsePhaseImg(1);
}

void CDlgCalibSys::OnBnClickedButtonBrswAcq3()
{
    BrowsePhaseImg(2);
}

void CDlgCalibSys::OnBnClickedButtonBrswAcq4()
{
    BrowsePhaseImg(3);
}

void CDlgCalibSys::OnBnClickedButtonBrswAcq5()
{
    BrowsePhaseImg(4);
}

void CDlgCalibSys::OnBnClickedButtonBrswAcq6()
{
    BrowsePhaseImg(5);
}

void CDlgCalibSys::OnBnClickedButtonBrswVideo()
{
    BrowseAcqImg(0);
}

void CDlgCalibSys::OnBnClickedButtonBrswMask()
{
    BrowseAcqImg(1);
}

void CDlgCalibSys::BrowsePhaseImg(UINT p_nId)
{
    UpdateData(TRUE);

    std::auto_ptr<CFileDialog> dlgFileDialog(new CFileDialog(TRUE, NULL, NULL, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
        "Phase Files (*.bin)|*.bin|All Files (*.*)|*.*||"));
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

void CDlgCalibSys::BrowseAcqImg(UINT p_nId)
{
    UpdateData(TRUE);

    CString* pRef;
    switch (p_nId)
    {
    case  0:
        pRef = &m_csEditVideo;
        break;
    case 1:
        throw std::runtime_error("Not used anymore.");
        //pRef = &m_csEditMask;
        break;
    default:
        return;
    }

    std::auto_ptr<CFileDialog> dlgFileDialog(new CFileDialog(TRUE, NULL, NULL, OFN_FILEMUSTEXIST | OFN_HIDEREADONLY,
        "Image Files (*.png; *.bmp; *.jpg; *.gif; *.tif)|*.png;*.bmp;*.jpg;*.gif;*.tif|All Files (*.*)|*.*||"));
    dlgFileDialog->m_ofn.lpstrTitle = "Calibration Image selection";
    CString csPath = *pRef;
    PathRemoveFileSpec(csPath.GetBuffer());
    csPath.ReleaseBuffer();
    dlgFileDialog->m_ofn.lpstrInitialDir = csPath;

    if (dlgFileDialog->DoModal() == IDOK)
    {
        csPath = dlgFileDialog->GetPathName();
        *pRef = csPath;
        UpdateData(FALSE);
    }
}


void CDlgCalibSys::OnBnClickedButtonSaveprm()
{
    UpdateData(TRUE);

    bool bres;
    CString strSection;
    strSection = _T("SensorHoloMap3_SensorScreen");
    bres = H3WritePrivProfileFloat(strSection, _T("PitchX"), m_fPitchX, g_strCalibSystemFile);
    bres = H3WritePrivProfileFloat(strSection, _T("PitchY"), m_fPitchY, g_strCalibSystemFile);
    bres = H3WritePrivProfileFloat(strSection, _T("PeriodX"), m_fPeriodX, g_strCalibSystemFile);
    bres = H3WritePrivProfileFloat(strSection, _T("PeriodY"), m_fPeriodY, g_strCalibSystemFile);

    bres = H3WritePrivProfileInt(strSection, _T("SizeX"), m_nScreenSizeX, g_strCalibSystemFile);
    bres = H3WritePrivProfileInt(strSection, _T("SizeY"), m_nScreenSizeY, g_strCalibSystemFile);
    bres = H3WritePrivProfileInt(strSection, _T("pixRefX"), m_nScreenRefPosX, g_strCalibSystemFile);
    bres = H3WritePrivProfileInt(strSection, _T("pixRefY"), m_nScreenRefPosY, g_strCalibSystemFile);

    strSection = _T("SensorHoloMap3");
    bres = H3WritePrivProfileInt(strSection, _T("PrefX"), m_nCrossX, g_strCalibSystemFile);
    bres = H3WritePrivProfileInt(strSection, _T("PrefY"), m_nCrossY, g_strCalibSystemFile);

    for (int i = 0; i < NB_IMG_PHASE; i++)
    {
        CString csImgAcqKey;
        csImgAcqKey.Format(_T("PathPhaseAcq%d"), i + 1);
        bres = H3WritePrivProfileString(strSection, csImgAcqKey, m_csEditAcq[i], g_strCalibSystemFile);
    }
    bres = H3WritePrivProfileString(strSection, _T("PathImgVideo"), m_csEditVideo, g_strCalibSystemFile);
    //bres = H3WritePrivProfileString(strSection, _T("PathImgMask"), m_csEditMask, g_strCalibSystemFile);
}

