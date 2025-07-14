#include "StdAfx.h"
#include "TreatPrepareData2.h"

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include "opencv2/imgproc/imgproc.hpp"
using namespace cv;

#define CST_PI       3.14159265358979323846

#include "fftw3.h"
#include <complex>

#include "H3AppToolsDecl.h"
#pragma comment (lib , "H3AppTools")

#include <omp.h> // for openMP directive //=> check also that /openMP is active in compiler properties

#include "FreeImagePlus.h"
#ifdef _DEBUG
#pragma comment (lib , "FreeImaged")
#pragma comment (lib , "FreeImagePlusd")
#else
#pragma comment (lib , "FreeImage")
#pragma comment (lib , "FreeImagePlus")
#endif

#define DBG_REC_MASK		0x00000010
#define DBG_REC_ERODE		0x00000020
#define DBG_REC_DILATE		0x00000040
#define DBG_REC_NX			0x00000080
#define	DBG_REC_NY			0X00000100
#define DBG_REC_FFTKILLER	0x00000200

template<class T> void fftShift(T* out, const T* in, size_t nx, size_t ny)
{
    const size_t hlen1 = (ny + 1) / 2;
    const size_t hlen2 = ny / 2;
    const size_t shft1 = ((nx + 1) / 2) * ny + hlen1;
    const size_t shft2 = (nx / 2) * ny + hlen2;

    const T* src = in;
    for (T* tgt = out; tgt < out + shft1 - hlen1; tgt += ny, src += ny) { // (nx+1)/2 times
        copy(src, src + hlen1, tgt + shft2);          //1->4
        copy(src + hlen1, src + ny, tgt + shft2 - hlen2);
    } //2->3
    src = in;
    for (T* tgt = out; tgt < out + shft2 - hlen2; tgt += ny, src += ny) { // nx/2 times
        copy(src + shft1, src + shft1 + hlen2, tgt);         //4->1
        copy(src + shft1 - hlen1, src + shft1, tgt + hlen2);
    } //3->2
};

CLASS_TYPE  void ExpandMatrixPadArray(H3_MATRIX& p_oMatrix, int p_nDecalX, int p_nDecalY)
{
    H3_MATRIX otmp;
    otmp.Copy(p_oMatrix);
    p_oMatrix.ReAlloc(otmp.GetLi() + 2 * p_nDecalY, otmp.GetCo() + 2 * p_nDecalX);
    p_oMatrix.Fill((TYPE)0.0f);
    TYPE* pPtrData = p_oMatrix.GetData();

    for (unsigned long nj = 0; nj < otmp.GetLi(); nj++)
    {
        unsigned long lidxline = nj * otmp.GetCo();									// index line start - small matrix 
        unsigned long lidxnewline = (nj + p_nDecalY) * p_oMatrix.GetCo() + p_nDecalX;	// index line start - expanded matrix

        for (unsigned long ni = 0; ni < otmp.GetCo(); ni++)
        {
            pPtrData[lidxnewline + ni] = otmp[lidxline + ni];
        }
    }
}


CTreatPrepareData2::CTreatPrepareData2()
{
    InitializeCriticalSection(&m_sCriticalSection);
    for (int i = 0; i < 2; i++)
    {
        m_hEventThDone[i] = CreateEvent(0, FALSE, FALSE, 0);
        ASSERT(m_hEventThDone[i] != 0);
    }

    for (int i = 0; i < 2; i++)
    {
        m_hEventPrepDone[i] = CreateEvent(0, FALSE, FALSE, 0);
        ASSERT(m_hEventPrepDone[i] != 0);
    }

    m_hFringeKillerErodeDone = CreateEvent(0, FALSE, FALSE, 0);

    m_nPrmErodeRadius = 0; //25
    m_nPrmDilateRadius = 0; //50
    m_nPrmSmoothKernelSize = 0;//
    m_dDistStop = 10.0;
    m_nMeanVignHalfSize = 10;
    m_fNoiseLow = -0.8f;
    m_fNoiseHi = 0.8f;

    m_bUseFringeKiller = false;
    m_uDbgFlag = 0;
    m_uRegFlag = 0;
    m_nOffsetExpandX = 0;
    m_nOffsetExpandY = 0;
    m_nNUIEnable = 0;
    m_nNUIOrder = 4;
    m_nNUIStep = 1;
    m_nNUIFib = 0;
    m_nNUIErode = 19;
}

CTreatPrepareData2::~CTreatPrepareData2()
{
    for (int i = 0; i < 2; i++)
    {
        if (m_hEventThDone[i] != 0)
        {
            CloseHandle(m_hEventThDone[i]);
            m_hEventThDone[i] = 0;
        }
    }

    for (int i = 0; i < 2; i++)
    {
        if (m_hEventPrepDone[i] != 0)
        {
            CloseHandle(m_hEventPrepDone[i]);
            m_hEventPrepDone[i] = 0;
        }
    }

    if (m_hFringeKillerErodeDone != 0)
    {
        CloseHandle(m_hFringeKillerErodeDone);
        m_hFringeKillerErodeDone = 0;
    }

    m_matNX.reset();
    m_matNY.reset();
    m_matPX.reset();
    m_matPY.reset();
    m_matMask.reset();
    m_Erode.reset();
    m_Dilate.reset();

    DeleteCriticalSection(&m_sCriticalSection);
}

bool CTreatPrepareData2::Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder)
{
    /* initialize random seed: */
    srand(time(NULL));

    m_pSaveData = 0;

    if (!FindTreatInitPrmInt(p_pPrmMap, _T("ErodeRadius"), m_nPrmErodeRadius))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ErodeRadius] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("DilateRadius"), m_nPrmDilateRadius))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [DilateRadius] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmDbl(p_pPrmMap, _T("ProlongeDistStop"), m_dDistStop))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ProlongeDistStop] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("ProlongeSampleHSize"), m_nMeanVignHalfSize))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ProlongeSampleHSize] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmFlt(p_pPrmMap, _T("ProlongeNoiseHi"), m_fNoiseHi))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ProlongeNoiseHi] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmFlt(p_pPrmMap, _T("ProlongeNoiseLo"), m_fNoiseLow))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ProlongeNoiseLo] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("SmoothKernelSize"), m_nPrmSmoothKernelSize))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [SmoothKernelSize] Parameter"), INanoTopoTreament::GetName()));

    if (!FindTreatInitPrmDbl(p_pPrmMap, _T("ProPolarRdxMax"), m_dProlongePolarRdxMax))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ProPolarRdxMax] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmDbl(p_pPrmMap, _T("ProPolarRdxMin"), m_dProlongePolarRdxMin))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ProPolarRdxMin] Parameter"), INanoTopoTreament::GetName()));

    if (!FindTreatInitPrmInt(p_pPrmMap, _T("NuiOrd"), m_nNUIOrder))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [NuiOrd] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("NuiStp"), m_nNUIStep))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [NuiStp] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("NuiFib"), m_nNUIFib))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [NuiFib] Parameter"), INanoTopoTreament::GetName()));
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("NuiErd"), m_nNUIErode))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [NuiErd] Parameter"), INanoTopoTreament::GetName()));

    if (m_nPrmSmoothKernelSize > 1)
    {
        if (m_nPrmSmoothKernelSize % 2 == 0)
        {
            int nNewVal = m_nPrmSmoothKernelSize - 1;
            LogThis(1, 3, Fmt(_T("{%s} [SmoothKernelSize] Parameter should a ODD number; Modify Value [%d] => [%d]"), INanoTopoTreament::GetName(), m_nPrmSmoothKernelSize, nNewVal));
            m_nPrmSmoothKernelSize = nNewVal;
        }
    }

    int  nBool = 0;
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("UseFringeKiller"), nBool))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [UseFringeKiller] Parameter"), INanoTopoTreament::GetName()));
    m_bUseFringeKiller = (nBool != 0);

    m_uRegFlag = 0;
    if (!GetRegistryFlag(m_uRegFlag, DBG_REC_NX | DBG_REC_NY))
        LogThis(1, 3, Fmt(_T("{%s} Could not reach Registry flag Parameter"), INanoTopoTreament::GetName()));


    // Read area from init (rect top left and size) in order to launch
    bool bConstructStandardArea = false;
    CString csAreaFile;
    if (!FindTreatInitPrmStr(p_pPrmMap, _T("ProlongeAreasFile"), csAreaFile))
    {
        LogThis(1, 3, Fmt(_T("{%s} Could not find [ProlongeAreasFile] Parameter"), INanoTopoTreament::GetName()));
        bConstructStandardArea = true;
    }
    else
    {
        CStdioFile oFile;
        if (oFile.Open(csAreaFile, CFile::modeRead | CFile::shareDenyWrite) == false)
        {
            LogThis(1, 3, Fmt(_T("{%s} Could not Open [ProlongeAreasFile]=%s -- Use standard areas"), INanoTopoTreament::GetName(), csAreaFile));
            bConstructStandardArea = true;
        }
        else
        {
            // each line correspond to a Rect definition in pixel (TopLeft.X,TopLeft.Y,Width, Height)
            // Area should cover all the wafer border with a suffisant depth to work with ProlongeDistStop parameter
            // avoid area intersection - that could cause multithreading memory conflict
            CString csLine;
            int nX, nY, nW, nH;
            m_vProlongeAreas.clear();
            while (oFile.ReadString(csLine))
            {
                if (4 == sscanf(csLine.GetBuffer(), _T("%d %d %d %d"), &nX, &nY, &nW, &nH))
                {
                    m_vProlongeAreas.push_back(Rect(
                        nX /*+m_nOffsetExpandX*/,
                        nY /*+m_nOffsetExpandY*/,
                        nW /*+ 2 * m_nOffsetExpandX*/,
                        nH /*+ 2 * m_nOffsetExpandY*/));
                }
                else
                {
                    LogThis(1, 4, Fmt(_T("{%s} Error in reading [ProlongeAreasFile]=%s -- Use standard areas"), INanoTopoTreament::GetName(), csAreaFile));
                    bConstructStandardArea = true;
                    csLine.ReleaseBuffer();
                    break;
                }
                csLine.ReleaseBuffer();
            }
            oFile.Close();
        }
    }

    if (bConstructStandardArea)
        m_vProlongeAreas.clear();

    // Load Transform Redress Matrix 
    int nOffset_px = 0;
    if (!FindTreatInitPrmInt(p_pPrmMap, _T("RedressOffset"), nOffset_px))
        LogThis(1, 3, Fmt(_T("{%s} Could not find [RedressOffset] Parameter"), INanoTopoTreament::GetName()));
    LoadRedressMatrix(nOffset_px, calibFolder);

    // Fringe Killer parameters
    CString csFK_PrmX;
    if (!FindTreatInitPrmStr(p_pPrmMap, _T("FKPrmX"), csFK_PrmX))
    {
        // a = center frequency zone exclusion
        // b = filter larger size 
        // {coef, expo, offset} - parameter of fct {f(x) = -coef * |x - m|^ expo + offset} where 'm' is the center of spectrum
        // frequency are filtered if PH(x) > f(x) 
        // params are stored in that order [a, b, coef, expo, offset]
        csFK_PrmX = _T("50;8;0.0022;1.35;205");

        LogThis(1, 3, Fmt(_T("{%s} Could not find [FKPrmX] Parameter"), INanoTopoTreament::GetName()));
    }

    CString csFK_PrmY;
    if (!FindTreatInitPrmStr(p_pPrmMap, _T("FKPrmY"), csFK_PrmY))
    {
        // a = center frequency zone exclusion
        // b = filter larger size 
        // {coef, expo, offset} - parameter of fct {f(y) = -coef * |y - m|^ expo + offset} where 'm' is the center of spectrum
        // frequency are filtered if PH(y) > f(y) 
        // params are stored in that order [a, b, coef, expo, offset]
        csFK_PrmY = _T("50;8;0.0028;1.3;203");

        LogThis(1, 3, Fmt(_T("{%s} Could not find [FKPrmY] Parameter"), INanoTopoTreament::GetName()));
    }

    int nNbPrm;
    nNbPrm = sscanf_s(csFK_PrmX, _T("%lf;%lf;%lf;%lf;%lf"), &m_dFK_PrmX[0], &m_dFK_PrmX[1], &m_dFK_PrmX[2], &m_dFK_PrmX[3], &m_dFK_PrmX[4]);
    if (nNbPrm != 5)
    {
        LogThis(1, 3, Fmt(_T("{%s} Parsing error in [FKPrmX] Parameter"), INanoTopoTreament::GetName()));
        m_dFK_PrmX[0] = 50.0;
        m_dFK_PrmX[1] = 8.0;
        m_dFK_PrmX[2] = 0.0022;
        m_dFK_PrmX[3] = 1.35;
        m_dFK_PrmX[4] = 205.0;
    }

    nNbPrm = sscanf_s(csFK_PrmY, _T("%lf;%lf;%lf;%lf;%lf"), &m_dFK_PrmY[0], &m_dFK_PrmY[1], &m_dFK_PrmY[2], &m_dFK_PrmY[3], &m_dFK_PrmY[4]);
    if (nNbPrm != 5)
    {
        LogThis(1, 3, Fmt(_T("{%s} Parsing error in [FKPrmY] Parameter"), INanoTopoTreament::GetName()));
        m_dFK_PrmY[0] = 50.0;
        m_dFK_PrmY[1] = 8.0;
        m_dFK_PrmY[2] = 0.0028;
        m_dFK_PrmY[3] = 1.3;
        m_dFK_PrmY[4] = 203.0;
    }

    return true;
}

void CTreatPrepareData2::LoadRedressMatrix(int p_nOffset_px, const CString& calibFolder)
{
    CString strSection;
    strSection = _T("Alta_RedressMatrix");

    CString inputSettingsFile = calibFolder + "\\" + _CalibPaths._InputSettingsFile;

    m_nCalibInvertNX = H3GetPrivProfileInt(strSection, _T("InvNx"), inputSettingsFile);		//1
    m_nCalibInvertNY = H3GetPrivProfileInt(strSection, _T("InvNy"), inputSettingsFile);		//0
    m_nCalibSwitchNi = H3GetPrivProfileInt(strSection, _T("SwitchNi"), inputSettingsFile);	//0

    float pt[4][2];
    pt[0][0] = H3GetPrivProfileFloat(strSection, _T("TL0"), inputSettingsFile);
    pt[0][1] = H3GetPrivProfileFloat(strSection, _T("TL1"), inputSettingsFile);
    pt[1][0] = H3GetPrivProfileFloat(strSection, _T("TR0"), inputSettingsFile);
    pt[1][1] = H3GetPrivProfileFloat(strSection, _T("TR1"), inputSettingsFile);
    pt[2][0] = H3GetPrivProfileFloat(strSection, _T("BR0"), inputSettingsFile);
    pt[2][1] = H3GetPrivProfileFloat(strSection, _T("BR1"), inputSettingsFile);
    pt[3][0] = H3GetPrivProfileFloat(strSection, _T("BL0"), inputSettingsFile);
    pt[3][1] = H3GetPrivProfileFloat(strSection, _T("BL1"), inputSettingsFile);

    Point2f dst[4];		// wafer µm
    Point2f src[4];    // TORDU image px
    int waferlength_um = H3GetPrivProfileInt(strSection, _T("WfLen"), inputSettingsFile); //300000;
    int nTargetWidth_px = H3GetPrivProfileInt(strSection, _T("ImWi"), inputSettingsFile); //3600;
    int nTargetHeight_px = H3GetPrivProfileInt(strSection, _T("ImHe"), inputSettingsFile); //3248;
    int pixeloffset = p_nOffset_px;
    int pxoffsetRecentrageX = (nTargetWidth_px - nTargetHeight_px) / 2;
    double pxsize = ((double)(nTargetHeight_px - 2 * pixeloffset) / (double)waferlength_um);

    // calib data maison
//	dst[0] = Point2f(60000.0f*pxsize + pixeloffset+pxoffsetRecentrageX, 60000.0f*pxsize + pixeloffset);	src[0] = Point2f(886,613.5);
//	dst[1] = Point2f(240000.0f*pxsize+ pixeloffset+pxoffsetRecentrageX, 60000.0f*pxsize + pixeloffset);	src[1] = Point2f(2751.5,594.5);
//	dst[2] = Point2f(240000.0f*pxsize+ pixeloffset+pxoffsetRecentrageX, 240000.0f*pxsize+ pixeloffset);	src[2] = Point2f(2756.5,2449.5);
//	dst[3] = Point2f(60000.0f*pxsize + pixeloffset+pxoffsetRecentrageX, 240000.0f*pxsize+ pixeloffset);	src[3] = Point2f(661.5,2430.5);

    //caliba data SEH
// 	dst[0] = Point2f(60000.0f*pxsize + pixeloffset+pxoffsetRecentrageX, 60000.0f*pxsize + pixeloffset);	src[0] = Point2f(863.36108,627.44446);
// 	dst[1] = Point2f(240000.0f*pxsize+ pixeloffset+pxoffsetRecentrageX, 60000.0f*pxsize + pixeloffset);	src[1] = Point2f(2768.1333,580.43335);
// 	dst[2] = Point2f(240000.0f*pxsize+ pixeloffset+pxoffsetRecentrageX, 240000.0f*pxsize+ pixeloffset);	src[2] = Point2f(2847.6162,2456.3738);
// 	dst[3] = Point2f(60000.0f*pxsize + pixeloffset+pxoffsetRecentrageX, 240000.0f*pxsize+ pixeloffset);	src[3] = Point2f(706.00000,2494.5000);

    dst[0] = Point2f(60000.0f * pxsize + pixeloffset + pxoffsetRecentrageX, 60000.0f * pxsize + pixeloffset);	src[0] = Point2f(pt[0][0], pt[0][1]);
    dst[1] = Point2f(240000.0f * pxsize + pixeloffset + pxoffsetRecentrageX, 60000.0f * pxsize + pixeloffset);	src[1] = Point2f(pt[1][0], pt[1][1]);
    dst[2] = Point2f(240000.0f * pxsize + pixeloffset + pxoffsetRecentrageX, 240000.0f * pxsize + pixeloffset);	src[2] = Point2f(pt[2][0], pt[2][1]);
    dst[3] = Point2f(60000.0f * pxsize + pixeloffset + pxoffsetRecentrageX, 240000.0f * pxsize + pixeloffset);	src[3] = Point2f(pt[3][0], pt[3][1]);

    // compute transform
    m_cvRedressTransform = getPerspectiveTransform(src, dst);
}

void CTreatPrepareData2::RedressMatrix(cv::Mat& p_MatToRedress)
{
    warpPerspective(p_MatToRedress, p_MatToRedress, m_cvRedressTransform, p_MatToRedress.size(), INTER_LINEAR /* | WARP_INVERSE_MAP*/);
}

bool CTreatPrepareData2::Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm)
{

    if (m_bEmergencyStop)
    {
        LogThis(1, 3, Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
        return false;
    }

    double dstart = GetPerfTime();

    m_Erode.reset(new  H3_MATRIX_UINT8());
    m_Dilate.reset(new  H3_MATRIX_UINT8());

    void* p = 0;
    int nSaveData = 0;
    unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag;
    int i, j, nLines, nCols;

    if (FindTreatPrmPtr(p_InputsPrm, "Save", p))
    {
        nSaveData = *((int*)p);
    }
    tData2Save* pSavData = nSaveData ? new tData2Save : nullptr; // SHOULD be deleted in CTreatPrhepareData0::SaveData	or any error exit or exception
    m_pSaveData = pSavData;

    if (FindTreatPrmPtr(p_InputsPrm, "LotID", p))
    {
        m_csLotID = *((CString*)p);
    }

    shared_ptr<void> pvMask;
    if (FindTreatPrmSharedPtr(p_InputsPrm, "Mask", pvMask))
    {
        m_matMask = static_pointer_cast<H3_MATRIX_UINT8> (pvMask);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("Mask Size = %d x %d"), m_matMask->GetCo(), m_matMask->GetLi()));
    }
    pvMask.reset();

    shared_ptr<void> pvNX;
    if (FindTreatPrmSharedPtr(p_InputsPrm, "NX", pvNX))
    {
        m_matNX = static_pointer_cast<H3_MATRIX_FLT32> (pvNX);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("NX Size = %d x %d"), m_matNX->GetCo(), m_matNX->GetLi()));
    }
    pvNX.reset();

    shared_ptr<void> pvNY;
    if (FindTreatPrmSharedPtr(p_InputsPrm, "NY", pvNY))
    {
        m_matNY = static_pointer_cast<H3_MATRIX_FLT32> (pvNY);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("NY Size = %d x %d"), m_matNY->GetCo(), m_matNY->GetLi()));
    }
    pvNY.reset();

    shared_ptr<void> pvPX;
    if (FindTreatPrmSharedPtr(p_InputsPrm, "PX", pvPX))
    {
        m_matPX = static_pointer_cast<H3_MATRIX_FLT32> (pvPX);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("PX Size = %d x %d"), m_matPX->GetCo(), m_matPX->GetLi()));
    }
    pvPX.reset();

    shared_ptr<void> pvPY;
    if (FindTreatPrmSharedPtr(p_InputsPrm, "PY", pvPY))
    {
        m_matPY = static_pointer_cast<H3_MATRIX_FLT32> (pvPY);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("PY Size = %d x %d"), m_matPY->GetCo(), m_matPY->GetLi()));
    }
    pvPY.reset();

    if (FindTreatPrmPtr(p_InputsPrm, "OffsetExpand_X", p))
    {
        m_nOffsetExpandX = *((int*)p);
    }
    if (FindTreatPrmPtr(p_InputsPrm, "OffsetExpand_Y", p))
    {
        m_nOffsetExpandY = *((int*)p);
    }
    if (FindTreatPrmPtr(p_InputsPrm, "NUIEnable", p))
    {
        m_nNUIEnable = *((int*)p);
    }


    // we assume here that all matrices has the same size
    ASSERT(m_matPX->GetCo() == m_matPY->GetCo());
    ASSERT(m_matPX->GetLi() == m_matPY->GetLi());
    ASSERT(m_matNX->GetCo() == m_matNY->GetCo());
    ASSERT(m_matNX->GetLi() == m_matNY->GetLi());
    ASSERT(m_matPX->GetCo() == m_matNY->GetCo());
    ASSERT(m_matPX->GetLi() == m_matNY->GetLi());
    ASSERT(m_matNX->GetCo() == m_matMask->GetCo());
    ASSERT(m_matNX->GetLi() == m_matMask->GetLi());

    if (m_nNUIEnable != 0)
    {
        LogThis(1, 1, Fmt(_T("##### Ordering ")));
        //
        // NUI - NON UNIFORMITY IMPROVMENT
        //	   - Perform Dagauchi
        /////
        //on supprime l'oréole autour des données; --- on detour au mieux plus grand que l'erode PRM mais pas plus petit que 15(data NAN & bruit) et que l'erode PRM 
        //
        int nErodeRadius = m_nNUIErode;
        if (nErodeRadius < 15)
            nErodeRadius == 15;
        if (nErodeRadius < m_nPrmErodeRadius)
            nErodeRadius = m_nPrmErodeRadius;

        H3_MATRIX_UINT8 degauchimaskErode;
        degauchimaskErode.Copy(*(m_matMask.get()));
        Mat degauchicvMask = Mat(m_matMask->GetLi(), m_matMask->GetCo(), CV_8U, degauchimaskErode.GetData(), Mat::AUTO_STEP);
        //imwrite("D:\\Altasight\\Data\\DEGAUCH-originmsk.png",degauchicvMask);
        // redresse matrix
        RedressMatrix(degauchicvMask);
        //imwrite("D:\\Altasight\\Data\\DEGAUCH-Redressmsk.png",degauchicvMask);
        Mat elementErode = getStructuringElement(MORPH_ELLIPSE, Size(2 * nErodeRadius, 2 * nErodeRadius), Point(nErodeRadius, nErodeRadius));
        erode(degauchicvMask, degauchicvMask, elementErode);
        //imwrite("D:\\Altasight\\Data\\DEGAUCH-RedressErodemsk.png",degauchicvMask);
        // invert redress return to normal;
        warpPerspective(degauchicvMask, degauchicvMask, m_cvRedressTransform, degauchicvMask.size(), INTER_LINEAR | WARP_INVERSE_MAP);
        //imwrite("D:\\Altasight\\Data\\DEGAUCH-finalmsk.png",degauchicvMask);
        //on applique le degauchi
        Degauchi(m_matNX.get(), &degauchimaskErode);
        Degauchi(m_matNY.get(), &degauchimaskErode);
        LogThis(1, 1, Fmt(_T("##### Ordering NX & NY done")));
    }

    //
    ////////////////////////////////////////////////////////////////////
    //
    // MODIF EN CAS DE PB DE CALIB SUR LES SIGNES - pour miss vivi
    //

    // Inversion de signe du NX
    if (m_nCalibInvertNX != 0)
    {
        decltype(m_matNX->GetData()) pDataNx = m_matNX->GetData();
        for (long lItem = 0; lItem < m_matNX->GetLi() * m_matNX->GetCo(); lItem++)
        {
            pDataNx[lItem] = -1.0f * pDataNx[lItem];
        }
    }
    // Inversion de signe du NY
    if (m_nCalibInvertNY != 0)
    {
        decltype(m_matNY->GetData()) pDataNy = m_matNY->GetData();
        for (long lItem = 0; lItem < m_matNY->GetLi() * m_matNY->GetCo(); lItem++)
        {
            pDataNy[lItem] = -1.0f * pDataNy[lItem];
        }
    }

    // Echange PX<->PY & NX<->NY
    if (m_nCalibSwitchNi != 0)
    {
        decltype(m_matPX->GetData()) pDataPx = m_matPX->GetData();
        decltype(m_matPY->GetData()) pDataPy = m_matPY->GetData();
        decltype(m_matNX->GetData()) pDataNx = m_matNX->GetData();
        decltype(m_matNY->GetData()) pDataNy = m_matNY->GetData();
        for (long lItem = 0; lItem < m_matNX->GetLi() * m_matNX->GetCo(); lItem++)
        {
            float fTemp = pDataNx[lItem];
            pDataNx[lItem] = pDataNy[lItem];
            pDataNy[lItem] = fTemp;

            fTemp = pDataPx[lItem];
            pDataPx[lItem] = pDataPy[lItem];
            pDataPy[lItem] = fTemp;
        }
    }

    //
    // Erode mask
    //

    if (pSavData && (uDbgFlag & DBG_REC_MASK))
    {
        tSpT<H3_MATRIX_UINT8> elt;
        elt._cs = _T("MaskOriginal");
        elt._spT.reset(new H3_MATRIX_UINT8(*m_matMask.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = true;
        pSavData->spListU8.push_back(elt);
    }


    Mat cvMask = Mat(m_matMask->GetLi(), m_matMask->GetCo(), CV_8U, m_matMask->GetData(), Mat::AUTO_STEP);

    if (m_nNUIFib != 0)
    {
        // Filling the hole in masks due to modere settings acquisition  
        Mat elte = getStructuringElement(MORPH_RECT, Size(2 * m_nNUIFib + 1, 2 * m_nNUIFib + 1), Point(m_nNUIFib, m_nNUIFib));
        dilate(cvMask, cvMask, elte);
        erode(cvMask, cvMask, elte);
        // end filling
    }

    if (m_bUseFringeKiller)
        cvMask.copyTo(m_cvMaskOriginal);

    RedressMatrix(cvMask);

    if (pSavData && (uDbgFlag & DBG_REC_MASK))
    {
        tSpT<H3_MATRIX_UINT8> elt;
        elt._cs = _T("MaskRedress");
        elt._spT.reset(new H3_MATRIX_UINT8(*m_matMask.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = true;
        pSavData->spListU8.push_back(elt);
    }

    m_Erode->Copy(*(m_matMask.get()));
    if (m_bEmergencyStop)
    {
        LogThis(1, 3, Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
        return false;
    }

    double dssErode = 0.0;
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
    {
        LogThis(1, 1, Fmt(_T("##### Start Mask MultiThreaded Erode")));
        dssErode = GetPerfTime();
    }

    CWinThread* pThreadErode = AfxBeginThread(&CTreatPrepareData2::static_ErodeCV, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
    pThreadErode->m_bAutoDelete = TRUE; // the thread delete its self after completion
    pThreadErode->ResumeThread();

    //
    // perform differences on PX by column and differences on PY  by line
    //
    double dssMat = 0.0;
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
    {
        LogThis(1, 1, Fmt(_T("##### Start Diff and etc")));
        dssMat = GetPerfTime();
    }

    m_matPX->DiffByColumns();
    m_matPY->DiffByLines();

    nLines = m_matPX->GetLi();
    nCols = m_matPX->GetCo();
    decltype(m_matPX->GetData()) pDataPx = m_matPX->GetData();
    decltype(m_matPY->GetData()) pDataPy = m_matPY->GetData();
    decltype(m_matNX->GetData()) pDataNx = m_matNX->GetData();
    decltype(m_matNY->GetData()) pDataNy = m_matNY->GetData();
    decltype(m_matPY->GetAt(0, 0)) dMillions = 1000000.0;

#pragma omp parallel for 
    for (long lItem = 0; lItem < nLines * nCols; lItem++)
    {
        pDataNx[lItem] = std::abs(pDataPx[lItem]) * pDataNx[lItem] * dMillions;
        pDataNy[lItem] = std::abs(pDataPy[lItem]) * pDataNy[lItem] * dMillions;
        // Nan to Zero for NX & NY matrices (avoid another loops)
        if (_isnanf(pDataNx[lItem]))
            pDataNx[lItem] = 0;
        if (_isnanf(pDataNy[lItem]))
            pDataNy[lItem] = 0;
    }

    // PX & PY are no Longer used delete it
    pDataPx = pDataPy = 0; // to avoid using this pointer by mistake
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("# Done Diff and etc in %lf ms"), GetPerfTime() - dssMat));

    // Wait Erode Thread to be completed
    DWORD dwEvent = WaitForSingleObject(m_hEventThDone[0], INFINITE);
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
    {
        double dssEndErode = GetPerfTime();
        LogThis(1, 1, Fmt(_T("# Done Erode in %lf ms"), dssEndErode - dssErode));
    }
    m_matMask.reset();

    ExpandMatrixPadArray(*m_Erode.get(), m_nOffsetExpandX, m_nOffsetExpandY);

    if (pSavData && (uDbgFlag & DBG_REC_ERODE))
    {
        tSpT<H3_MATRIX_UINT8> elt;
        elt._cs = _T("MaskErode");
        elt._spT.reset(new H3_MATRIX_UINT8(*m_Erode.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = true;
        pSavData->spListU8.push_back(elt);
    }

    // 
    // Dilate
    //

    if (m_bEmergencyStop)
    {
        LogThis(1, 3, Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
        return false;
    }

    m_Dilate->Copy(*(m_Erode.get()));

    double dssDilate = 0.0;
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
    {
        LogThis(1, 1, Fmt(_T("##### Start Mask MultiThreaded Dilate")));
        dssDilate = GetPerfTime();
    }
    CWinThread* pThreadDilate = AfxBeginThread(&CTreatPrepareData2::static_DilateCV, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
    pThreadDilate->m_bAutoDelete = TRUE; // the thread delete its self after completion
    pThreadDilate->ResumeThread();

    nLines = m_matNX->GetLi();
    nCols = m_matNY->GetCo();

    m_cvNX = Mat(nLines, nCols, CV_32F, m_matNX->GetData(), Mat::AUTO_STEP);
    m_cvNY = Mat(nLines, nCols, CV_32F, m_matNY->GetData(), Mat::AUTO_STEP);

    // Wait Erode Thread of fringe killer if needed  to be completed
    if (m_bUseFringeKiller)
    {
        dwEvent = WaitForSingleObject(m_hFringeKillerErodeDone, INFINITE);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
        {
            double dssEndErode = GetPerfTime();
            LogThis(1, 1, Fmt(_T("# Done FK Erode in %lf ms"), dssEndErode - dssErode));
        }
    }

    CWinThread* pThreadX = AfxBeginThread(&CTreatPrepareData2::static_PerformX, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
    pThreadX->m_bAutoDelete = TRUE; // the thread delete its self after completion
    pThreadX->ResumeThread();

    CWinThread* pThreadY = AfxBeginThread(&CTreatPrepareData2::static_PerformY, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
    pThreadY->m_bAutoDelete = TRUE; // the thread delete its self after completion
    pThreadY->ResumeThread();

    // wait for perf X and perf Y to be donne
    dwEvent = WaitForMultipleObjects(sizeof(m_hEventPrepDone) / sizeof(HANDLE), m_hEventPrepDone, TRUE, INFINITE);
    if (m_bUseFringeKiller)
    {
        m_cvMaskOriginal.release();
    }

    // resize matrices
    ExpandMatrixPadArray(*m_matNX.get(), m_nOffsetExpandX, m_nOffsetExpandY);
    ExpandMatrixPadArray(*m_matNY.get(), m_nOffsetExpandX, m_nOffsetExpandY);
    // since matrix have been reallocated and resized we need to rebind data with opencv matrices and data buffers
    nLines = m_matNX->GetLi();
    nCols = m_matNY->GetCo();
    m_cvNX = Mat(nLines, nCols, CV_32F, m_matNX->GetData(), Mat::AUTO_STEP);
    m_cvNY = Mat(nLines, nCols, CV_32F, m_matNY->GetData(), Mat::AUTO_STEP);
    pDataNx = m_matNX->GetData();
    pDataNy = m_matNY->GetData();
    //
    // we assume here mask erode has the same size than NX and NY
    // Apply mask on NX && NY
    //
#pragma omp parallel for private(j)
    for (i = 0; i < nLines; i++)
    {
        for (j = 0; j < nCols; j++)
        {
            long lItem = i * nCols + j;
            pDataNx[lItem] *= m_Erode->GetAt(i, j);
            pDataNy[lItem] *= m_Erode->GetAt(i, j);
        }
    }

    if (m_bEmergencyStop)
    {
        LogThis(1, 3, Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
        return false;
    }

    //
    // Prolonge Bord
    //

    if (m_dProlongePolarRdxMax > 0.0)
        Prolonge_Bord_Polar();

    // Wait Dilate Threads to be completed
    dwEvent = WaitForSingleObject(m_hEventThDone[1], INFINITE);
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("# Done Dilate in %lf ms"), GetPerfTime() - dssDilate));

    if (pSavData && (uDbgFlag & DBG_REC_DILATE))
    {
        tSpT<H3_MATRIX_UINT8> elt;
        elt._cs = _T("MaskDilate");
        elt._spT.reset(new H3_MATRIX_UINT8(*m_Dilate.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = true;
        pSavData->spListU8.push_back(elt);
    }

    if (pSavData && (uDbgFlag & DBG_REC_NX))
    {
        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("NX");
        elt._spT.reset(new H3_MATRIX_FLT32(*m_matNX.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = true;
        pSavData->spListF32.push_back(elt);
    }

    if (pSavData && (uDbgFlag & DBG_REC_NY))
    {
        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("NY");
        elt._spT.reset(new H3_MATRIX_FLT32(*m_matNY.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = true;
        pSavData->spListF32.push_back(elt);
    }

    if (m_bEmergencyStop)
    {
        LogThis(1, 3, Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
        return false;
    }

    //
    // launch saving thread if needed
    //
    if (nSaveData != 0 && pSavData != nullptr)
    {
        static UINT uCount = 0;
        uCount++;
        pSavData->nId = uCount;
        pSavData->csName = INanoTopoTreament::GetName();
        pSavData->csPath = _T("C:\\Altasight\\Nano\\Data");
        void* pcs;
        if (FindTreatPrmPtr(p_InputsPrm, "OutPath", pcs))
        {
            pSavData->csPath = *((CString*)pcs);
        }
        pSavData->csPath += _T("\\Dbg\\");
        pSavData->csPath += m_csLotID;

        m_pSaveData = nullptr; // don't use member variable, pointer will be deleted in saving thread
        CWinThread* pThread = AfxBeginThread(&CTreatPrepareData2::SaveData, pSavData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
        if (pThread == 0)
        {
            LogThis(1, 4, Fmt(_T("(%s) AfxBeginThread() failed.\n"), INanoTopoTreament::GetName()));
            return false;
        }
        pThread->m_bAutoDelete = TRUE; // the thread delete its self after completion
        pThread->ResumeThread();
    }

    double dEnd = GetPerfTime();
    if (FindTreatPrmPtr(p_OutputsPrm, "CS", p))
    {
        CString* pCs = (CString*)p;
        (*pCs) = Fmt(_T("(%s) exec in %0.3f ms "), INanoTopoTreament::GetName(), dEnd - dstart);
    }

    AddTreatPrmSharedPtr(p_OutputsPrm, "MaskE", shared_ptr<void>(m_Erode));
    AddTreatPrmSharedPtr(p_OutputsPrm, "MaskD", shared_ptr<void>(m_Dilate));

    m_cvNX.release();
    m_cvNY.release();
    m_matNX.reset();
    m_matNY.reset();
    m_matPX.reset();
    m_matPY.reset();
    m_matMask.reset();
    m_Erode.reset();
    m_Dilate.reset();

    return true;
}

void CTreatPrepareData2::FringeKillerNX(double p_dMin)
{
    // using FFTW 3 Library
    int i, j;
    int nx = m_matNX->GetCo();
    ASSERT(nx > 0);
    int ny = m_matNX->GetLi();
    ASSERT(ny > 0);
    int nx2 = nx / 2;
    int ny2 = ny / 2;

    complex<float>* pFFT_NX = new complex<float>[nx * ny]; // FFT
    complex<float>* pFFTs_NX = new complex<float>[nx * ny]; // FFT recentrée / fftshifthée

    unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag;

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NX_0Before");
        elt._spT.reset(new H3_MATRIX_FLT32(*m_matNX.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }

    for (i = 0; i < (nx * ny); i++)
    {
        pFFTs_NX[i] = complex<float>(((float)(m_matNX->GetData()[i])) - p_dMin, 0.0f);
    }

    // create FFT forward plan real to complex
    fftwf_plan oPlan_Fft2;
    oPlan_Fft2 = fftwf_plan_dft_2d(ny, nx, reinterpret_cast<fftwf_complex*>(pFFTs_NX), reinterpret_cast<fftwf_complex*>(pFFT_NX), FFTW_FORWARD, FFTW_ESTIMATE | FFTW_MEASURE);

    // perform FFT 2D
    fftwf_execute(oPlan_Fft2);

    // Shift - center Frequency
    fftShift< complex<float> >(pFFTs_NX, pFFT_NX, ny, nx);

    float* pH = new float[nx * ny];
    for (i = 0; i < (nx * ny); i++)
    {
        pH[i] = 20.0f * log(abs(pFFTs_NX[i]));
    }

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        shared_ptr<H3_MATRIX_FLT32> mat;
        mat.reset(new  H3_MATRIX_FLT32(m_matNX->GetLi(), m_matNX->GetCo()));
        for (i = 0; i < (nx * ny); i++)
        {
            mat->GetData()[i] = pH[i];
        }

        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NX_FFT");
        elt._spT.reset(new H3_MATRIX_FLT32(*mat.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }

    int na = (int)m_dFK_PrmX[0]; // zone central d'exclusion du filtre
    int nb = (int)m_dFK_PrmX[1]; // largeur du filtre
    double dCoef = m_dFK_PrmX[2];
    double dExpo = m_dFK_PrmX[3];
    double dOff = m_dFK_PrmX[4];
    for (j = 0; j < nx; j++)
    {
        double dSeuilcalc = -dCoef * pow(abs(j - nx2), dExpo) + dOff;
        if (j < (nx2 - na) || j >(nx2 + na))
        {
            for (i = ny2 - nb; i <= ny2 + nb; i++)
            {
                if (pH[i * nx + j] > dSeuilcalc)
                    pFFTs_NX[i * nx + j] = complex<float>(0.0, 0.0);
            }
        }
    }

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        shared_ptr<H3_MATRIX_FLT32> mat;
        mat.reset(new  H3_MATRIX_FLT32(m_matNX->GetLi(), m_matNX->GetCo()));
        for (i = 0; i < (nx * ny); i++)
        {
            pH[i] = 20.0f * log(abs(pFFTs_NX[i]));
            mat->GetData()[i] = pH[i];
        }

        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NX_FFT_Filter");
        elt._spT.reset(new H3_MATRIX_FLT32(*mat.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = 0.0;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }


    // Free H matrix
    delete[] pH;
    pH = 0;

    // return to the real world, create FFT backward plan complex to real
    fftwf_plan oPlan_InverseFft2;
    oPlan_InverseFft2 = fftwf_plan_dft_2d(ny, nx, reinterpret_cast<fftwf_complex*>(pFFTs_NX), reinterpret_cast<fftwf_complex*>(pFFT_NX), FFTW_BACKWARD, FFTW_ESTIMATE | FFTW_MEASURE);

    // perform inverse FFT 2D
    fftwf_execute(oPlan_InverseFft2);

    // reapply offset to NX matrix and perform conversion if needed
    double dAppliedFactor = 1.0 / ((double)nx * (double)ny);
    for (i = 0; i < (nx * ny); i++)
    {
        m_matNX->GetData()[i] = (abs(pFFT_NX[i]) * dAppliedFactor + p_dMin);
    }

    // free memory
    delete[] pFFTs_NX;
    pFFTs_NX = 0;
    delete[] pFFT_NX;
    pFFT_NX = 0;

    fftwf_destroy_plan(oPlan_Fft2);
    fftwf_destroy_plan(oPlan_InverseFft2);

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NX_1After");
        elt._spT.reset(new H3_MATRIX_FLT32(*m_matNX.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }
}

void CTreatPrepareData2::FringeKillerNY(double p_dMin)
{
    // using FFTW 3 Library

    int i, j;
    int nx = m_matNY->GetCo();
    ASSERT(nx > 0);
    int ny = m_matNY->GetLi();
    ASSERT(ny > 0);
    int nx2 = nx / 2;
    int ny2 = ny / 2;

    complex<double>* pFFT_NY = new complex<double>[nx * ny]; // FFT
    complex<double>* pFFTs_NY = new complex<double>[nx * ny]; // FFT recentrée / fftshifthée

    unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag;

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NY_0Before");
        elt._spT.reset(new H3_MATRIX_FLT32(*m_matNY.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }

    // on applique l'offsetMin et l'on converti en double (NYOff = NY - OffsetMin)
    for (i = 0; i < (nx * ny); i++)
    {
        pFFTs_NY[i] = complex<float>(((double)(m_matNY->GetData()[i])) - p_dMin, 0.0f);
    }

    // create FFT forward plan real to complex
    fftw_plan oPlan_Fft2;
    oPlan_Fft2 = fftw_plan_dft_2d(ny, nx, reinterpret_cast<fftw_complex*>(pFFTs_NY), reinterpret_cast<fftw_complex*>(pFFT_NY), FFTW_FORWARD, FFTW_ESTIMATE);

    // perform FFT 2D
    fftw_execute(oPlan_Fft2);

    // Shift - center Frequency
    fftShift< complex<double> >(pFFTs_NY, pFFT_NY, ny, nx);

    double* pH = new double[nx * ny];
    for (i = 0; i < (nx * ny); i++)
    {
        pH[i] = 20.0 * log(abs(pFFTs_NY[i]));
    }

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        shared_ptr<H3_MATRIX_FLT32> mat;
        mat.reset(new  H3_MATRIX_FLT32(m_matNY->GetLi(), m_matNY->GetCo()));
        for (i = 0; i < (nx * ny); i++)
        {
            mat->GetData()[i] = pH[i];
        }

        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NY_FFT");
        elt._spT.reset(new H3_MATRIX_FLT32(*mat.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }

    // filter fringe
    int na = (int)m_dFK_PrmY[0]; // zone central d'exclusion du filtre
    int nb = (int)m_dFK_PrmY[1]; // largeur du filtre
    double dCoef = m_dFK_PrmY[2];
    double dExpo = m_dFK_PrmY[3];
    double dOff = m_dFK_PrmY[4];
    for (i = 0; i < ny; i++)
    {
        double dSeuilcalc = -dCoef * pow(abs(i - ny2), dExpo) + dOff;
        if (i < (ny2 - na) || i >(ny2 + na))
        {
            for (j = nx2 - nb; j <= nx2 + nb; j++)
            {
                if (pH[i * nx + j] > dSeuilcalc)
                    pFFTs_NY[i * nx + j] = complex<float>(0.0, 0.0);
            }
        }
    }

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        shared_ptr<H3_MATRIX_FLT32> mat;
        mat.reset(new  H3_MATRIX_FLT32(m_matNY->GetLi(), m_matNY->GetCo()));
        for (i = 0; i < (nx * ny); i++)
        {
            pH[i] = 20.0f * log(abs(pFFTs_NY[i]));
            mat->GetData()[i] = pH[i];
        }

        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NY_FFT_Filter");
        elt._spT.reset(new H3_MATRIX_FLT32(*mat.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = 0.0;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }

    // Free H matrix
    delete[] pH;
    pH = 0;

    // return to the real world, create FFT backward plan complex to real
    fftw_plan oPlan_InverseFft2;
    oPlan_InverseFft2 = fftw_plan_dft_2d(ny, nx, reinterpret_cast<fftw_complex*>(pFFTs_NY), reinterpret_cast<fftw_complex*>(pFFT_NY), FFTW_BACKWARD, FFTW_ESTIMATE);

    // perform inverse FFT 2D
    fftw_execute(oPlan_InverseFft2);

    // reapply offset to NY matrix and perform conversion if needed
    double dAppliedFactor = 1.0 / ((double)nx * (double)ny);
    for (i = 0; i < (nx * ny); i++)
    {
        m_matNY->GetData()[i] = (abs(pFFT_NY[i]) * dAppliedFactor + p_dMin);
    }

    // free memory
    delete[] pFFTs_NY;
    pFFTs_NY = 0;
    delete[] pFFT_NY;
    pFFT_NY = 0;

    fftw_destroy_plan(oPlan_Fft2);
    fftw_destroy_plan(oPlan_InverseFft2);

    if (m_pSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
    {
        tSpT<H3_MATRIX_FLT32> elt;
        elt._cs = _T("FK_NY_1After");
        elt._spT.reset(new H3_MATRIX_FLT32(*m_matNY.get())); // copy since data altered in following treatments

        elt._bImg = true;
        elt._bAutoScale = true;
        elt._fMin = FLT_MAX;
        elt._fMax = FLT_MAX;
        elt._bHbf = false;
        elt._bBin = false;
        EnterCriticalSection(&m_sCriticalSection);
        m_pSaveData->spListF32.push_back(elt);
        LeaveCriticalSection(&m_sCriticalSection);
    }
}


UINT CTreatPrepareData2::static_ErodeCV(void* p_pParameters)
{
    CTreatPrepareData2* pObj = static_cast<CTreatPrepareData2*> (p_pParameters);
    pObj->ErodeCV();
    return 041;
}

void CTreatPrepareData2::ErodeCV()
{
    if (m_nPrmErodeRadius > 0)
    {
        Mat matErodecv = Mat(m_Erode->GetLi(), m_Erode->GetCo(), CV_8U, m_Erode->GetData(), Mat::AUTO_STEP);
        Mat elementErode = getStructuringElement(MORPH_ELLIPSE, Size(2 * m_nPrmErodeRadius, 2 * m_nPrmErodeRadius), Point(m_nPrmErodeRadius, m_nPrmErodeRadius));
        erode(matErodecv, matErodecv, elementErode);
    }
    SetEvent(m_hEventThDone[0]);

    if (m_bUseFringeKiller)
    {
        if (m_nPrmErodeRadius > 0)
        {
            Mat elementErode = getStructuringElement(MORPH_ELLIPSE, Size(m_nPrmErodeRadius, m_nPrmErodeRadius), Point(m_nPrmErodeRadius / 2, m_nPrmErodeRadius / 2));
            erode(m_cvMaskOriginal, m_cvMaskOriginal, elementErode);
        }
        m_cvMaskOriginal.convertTo(m_cvMaskOriginal, CV_32F);
        SetEvent(m_hFringeKillerErodeDone);
    }
}

UINT CTreatPrepareData2::static_DilateCV(void* p_pParameters)
{
    CTreatPrepareData2* pObj = static_cast<CTreatPrepareData2*> (p_pParameters);
    pObj->DilateCV();
    return 042;
}

void CTreatPrepareData2::DilateCV()
{
    if (m_nPrmDilateRadius > 0)
    {
        Mat matDilatecv = Mat(m_Dilate->GetLi(), m_Dilate->GetCo(), CV_8U, m_Dilate->GetData(), Mat::AUTO_STEP);
        Mat elementDilate = getStructuringElement(MORPH_ELLIPSE, Size(2 * m_nPrmDilateRadius, 2 * m_nPrmDilateRadius), Point(m_nPrmDilateRadius, m_nPrmDilateRadius));
        dilate(matDilatecv, matDilatecv, elementDilate);
    }
    SetEvent(m_hEventThDone[1]);
}

UINT CTreatPrepareData2::static_PerformX(void* p_pParameters)
{
    CTreatPrepareData2* pObj = static_cast<CTreatPrepareData2*> (p_pParameters);
    pObj->PerformPreparation_X();
    return 043;
}

void CTreatPrepareData2::PerformPreparation_X()
{
    double dssX = GetPerfTime();
    if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("##### Start PerformPreparation_X")));

    if (m_bUseFringeKiller == true)
    {
        // perform Fringe killer - remove parasites fringes in Nx 
        if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("#### Start FKiller X")));

        double dssFKX = GetPerfTime();
        m_cvNX = m_cvMaskOriginal.mul(m_cvNX);
        if (m_nPrmSmoothKernelSize > 0)
            blur(m_cvNX, m_cvNX, cv::Size(m_nPrmSmoothKernelSize, m_nPrmSmoothKernelSize));

        double dMinX;
        minMaxLoc(m_cvNX, &dMinX);
        FringeKillerNX(dMinX);
        m_cvNX = m_cvMaskOriginal.mul(m_cvNX);

        if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("# Done FKiller X Done in %lf ms"), GetPerfTime() - dssFKX));

    }

    // Redress and Smooth Nx
    RedressMatrix(m_cvNX);

    // if we use Fringe killer smoothing need to be done before redressing otherwise we had to do it after
    if (m_bUseFringeKiller == false)
    {
        if (m_nPrmSmoothKernelSize > 0)
            blur(m_cvNX, m_cvNX, cv::Size(m_nPrmSmoothKernelSize, m_nPrmSmoothKernelSize));
    }

    if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("# Done PerformPreparation_X in %lf ms"), GetPerfTime() - dssX));
    SetEvent(m_hEventPrepDone[0]);
}

UINT CTreatPrepareData2::static_PerformY(void* p_pParameters)
{
    CTreatPrepareData2* pObj = static_cast<CTreatPrepareData2*> (p_pParameters);
    pObj->PerformPreparation_Y();
    return 044;
}

void CTreatPrepareData2::PerformPreparation_Y()
{
    double dssY = GetPerfTime();
    if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("##### Start PerformPreparation_Y")));

    if (m_bUseFringeKiller == true)
    {
        // perform Fringe killer - remove parasites fringes in Ny 
        if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("#### Start FKiller Y")));

        double dssFKY = GetPerfTime();
        m_cvNY = m_cvMaskOriginal.mul(m_cvNY);
        if (m_nPrmSmoothKernelSize > 0)
            blur(m_cvNY, m_cvNY, cv::Size(m_nPrmSmoothKernelSize, m_nPrmSmoothKernelSize));

        double dMinY;
        minMaxLoc(m_cvNY, &dMinY);
        FringeKillerNY(dMinY);
        m_cvNY = m_cvMaskOriginal.mul(m_cvNY);

        if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("# Done FKiller Y Done in %lf ms"), GetPerfTime() - dssFKY));

    }

    // Redress and Smooth Ny
    RedressMatrix(m_cvNY);

    // if we use Fringe killer smoothing need to be done before redressing otherwise we had to do it after
    if (m_bUseFringeKiller == false)
    {
        if (m_nPrmSmoothKernelSize > 0)
            blur(m_cvNY, m_cvNY, cv::Size(m_nPrmSmoothKernelSize, m_nPrmSmoothKernelSize));
    }

    if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("# Done PerformPreparation_Y in %lf ms"), GetPerfTime() - dssY));
    SetEvent(m_hEventPrepDone[1]);
}

bool CTreatPrepareData2::SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
{
    float* pData = p_oMatrixFloat->GetData();

    unsigned long  lCols = p_oMatrixFloat->GetCo();
    unsigned long  lLines = p_oMatrixFloat->GetLi();

    float fMin = FLT_MAX;
    float fMax = -FLT_MAX;

    bool bUseMinPrm = (p_fMin != FLT_MAX);
    bool bUseMaxPrm = (p_fMax != FLT_MAX);

    float a = 1.0f;
    float b = 0.0f;
    if (bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
    {
        for (long lItem = 0; lItem < p_oMatrixFloat->GetSize(); lItem++)
        {
            if (!bUseMinPrm)
                fMin = __min(fMin, pData[lItem]);
            if (!bUseMaxPrm)
                fMax = __max(fMax, pData[lItem]);
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

    fipImage oImg(FIT_BITMAP, lCols, lLines, 8);
    for (unsigned y = 0; y < oImg.getHeight(); y++)
    {
        //ici pb à resoudre pour affichage image
        BYTE* pbits = (BYTE*)oImg.getScanLine(y);
        for (unsigned x = 0; x < oImg.getWidth(); x++)
        {
            if (_isnanf(pData[y * lCols + x]))
                pbits[x] = 0;
            else
                pbits[x] = saturate_cast<uchar>(pData[y * lCols + x] * a + b);
        }
    }
    oImg.flipVertical();
    BOOL bRes = oImg.save(p_csFilepath, 0);
    return (bRes != 0);
}

bool CTreatPrepareData2::SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin /*= INT_MAX*/, int p_nMax /*= INT_MAX*/, bool bAutoscale /*= true*/)
{
    unsigned char* pData = p_oMatrix->GetData();

    unsigned long  lCols = p_oMatrix->GetCo();
    unsigned long  lLines = p_oMatrix->GetLi();

    float fMin = (float)INT_MAX;
    float fMax = (float)-INT_MAX;

    bool bUseMinPrm = (p_nMin != INT_MAX);
    bool bUseMaxPrm = (p_nMax != INT_MAX);

    float a = 1.0f;
    float b = 0.0f;
    if (bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
    {
        for (long lItem = 0; lItem < p_oMatrix->GetSize(); lItem++)
        {
            if (!bUseMinPrm)
                fMin = __min(fMin, pData[lItem]);
            if (!bUseMaxPrm)
                fMax = __max(fMax, pData[lItem]);
        }
    }
    else
    {
        if (!bUseMinPrm)
            fMax = 255.0f;
        if (!bUseMinPrm)
            fMin = 0.0f;
    }

    if (bUseMinPrm)
    {
        fMin = (float)p_nMin;
    }
    if (bUseMaxPrm)
    {
        fMax = (float)p_nMax;
    }

    a = 255.0f / (fMax - fMin);
    b = -fMin * 255.0f / (fMax - fMin);

    fipImage oImg(FIT_BITMAP, lCols, lLines, 8);
    for (unsigned y = 0; y < oImg.getHeight(); y++)
    {
        for (unsigned x = 0; x < oImg.getWidth(); x++)
        {
            BYTE indx = saturate_cast<uchar>(pData[y * lCols + x] * a + b);
            oImg.setPixelIndex(x, y, &indx);
        }
    }
    oImg.flipVertical();
    BOOL bRes = oImg.save(p_csFilepath, 0);
    return (bRes != 0);
}

UINT CTreatPrepareData2::SaveData(void* p_pParameters)
{
    tData2Save* pData = static_cast<tData2Save*>(p_pParameters);
    if (pData == nullptr)
        return 1;

    CString csFileName;
    CString sGenPath = pData->csPath;

    double dStart = GetPerfTime();
    UINT nId = pData->nId;
    CString csTreatName = pData->csName;
    LogThis(1, 1, Fmt(_T("(%s) ##### Start saving data = No %d"), csTreatName, nId));
    list < tSpT<H3_MATRIX_UINT8> > spListU8 = pData->spListU8;
    list < tSpT<H3_MATRIX_FLT32> > spListF32 = pData->spListF32;
    delete pData;
    pData = 0;

    // Assure Results Directory exist
    CreateDir(sGenPath);

    while (spListU8.size() != 0)
    {
        tSpT<H3_MATRIX_UINT8> elt = spListU8.front();
        if (elt._spT)
        {
            if (elt._bImg)
            {
                csFileName = Fmt(_T("%s\\%s_%s_%d.png"), sGenPath, csTreatName, elt._cs, nId);
                if (!SaveGreyImageUInt8(csFileName, elt._spT))
                    LogThis(1, 4, Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
                else
                    LogThis(1, 1, Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
            }
            if (elt._bHbf)
            {
                csFileName = Fmt(_T("%s\\%s_%s_%d.hbf"), sGenPath, csTreatName, elt._cs, nId);
                bool bRes = false;
                FILE* pFile = 0;
                if (fopen_s(&pFile, (LPCSTR)csFileName, "wb+") == 0)
                {
                    bRes = elt._spT->fSaveHBF(pFile);
                    fclose(pFile);
                }
                if (!bRes)
                    LogThis(1, 4, Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
                else
                    LogThis(1, 1, Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
            }
            if (elt._bBin)
            {
                csFileName = Fmt(_T("%s\\%s_%s_%d.bin"), sGenPath, csTreatName, elt._cs, nId);
                bool bRes = false;
                FILE* pFile = 0;
                if (fopen_s(&pFile, (LPCSTR)csFileName, "wb+") == 0)
                {
                    bRes = elt._spT->fSaveBIN(pFile);
                    fclose(pFile);
                }
                if (!bRes)
                    LogThis(1, 4, Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
                else
                    LogThis(1, 1, Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
            }
            elt._spT.reset();
        }
        spListU8.pop_front();
    }

    while (spListF32.size() != 0)
    {
        tSpT<H3_MATRIX_FLT32> elt = spListF32.front();
        if (elt._spT)
        {
            if (elt._bImg)
            {
                csFileName = Fmt(_T("%s\\%s_%s_%d.png"), sGenPath, csTreatName, elt._cs, nId);
                float fMax = FLT_MAX;
                float fMin = elt._fMin;
                if (!elt._bAutoScale)
                {
                    fMax = elt._fMax;
                    fMin = elt._fMin;
                }
                if (!SaveGreyImageFlt32(csFileName, elt._spT, fMin, fMax, elt._bAutoScale))
                    LogThis(1, 4, Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
                else
                    LogThis(1, 1, Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
            }
            if (elt._bHbf)
            {
                csFileName = Fmt(_T("%s\\Filter_%s_%d.hbf"), sGenPath, elt._cs, nId);
                bool bRes = false;
                FILE* pFile = 0;
                if (fopen_s(&pFile, (LPCSTR)csFileName, "wb+") == 0)
                {
                    bRes = elt._spT->fSaveHBF(pFile);
                    fclose(pFile);
                }
                if (!bRes)
                    LogThis(1, 4, Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
                else
                    LogThis(1, 1, Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
            }
            if (elt._bBin)
            {
                csFileName = Fmt(_T("%s\\%s_%s_%d.bin"), sGenPath, csTreatName, elt._cs, nId);
                bool bRes = false;
                FILE* pFile = 0;
                if (fopen_s(&pFile, (LPCSTR)csFileName, "wb+") == 0)
                {
                    bRes = elt._spT->fSaveBIN(pFile);
                    fclose(pFile);
                }
                if (!bRes)
                    LogThis(1, 4, Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
                else
                    LogThis(1, 1, Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
            }
            elt._spT.reset();
        }
        spListF32.pop_front();
    }


    double dEnd = GetPerfTime();
    LogThis(1, 2, Fmt(_T("(%s) ##### End saving data = No %d --- Exec in %lf"), csTreatName, nId, dEnd - dStart));

    return 0;
}

HRESULT CTreatPrepareData2::QueryInterface(REFIID iid, void** ppvObject)
{
    *ppvObject = 0;   // Toujours initialiser le pointeur renvoyé.
    if (iid == IID_IUnknown)
        *reinterpret_cast<IUnknown**>(ppvObject) = static_cast<IUnknown*>(this);
    else
        if (iid == IID_INanoTreatment)
            *reinterpret_cast<INanoTopoTreament**>(ppvObject) = static_cast<INanoTopoTreament*>(this);
    if (*ppvObject == 0)
        return E_NOINTERFACE;
    AddRef();           // On incrémente le compteur de références.
    return NOERROR;
}

ULONG CTreatPrepareData2::AddRef(void)
{
    m_ulRefCount++;
    return m_ulRefCount;
}

ULONG CTreatPrepareData2::Release(void)
{
    m_ulRefCount--;
    if (m_ulRefCount != 0)
        return m_ulRefCount;
    delete this;     // Destruction de l'objet.
    return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}

bool CTreatPrepareData2::Prolonge_Bord_Polar()
{
    int nWidth = m_Erode->GetCo();
    int nHeight = m_Erode->GetLi();

    double dCenterX = nWidth / 2;
    double dCenterY = nHeight / 2;
    double dRadiusMax = m_dProlongePolarRdxMax;//nWidth/2;
    double dRadiusMin = m_dProlongePolarRdxMin; //1100.0;

    double dssStart = GetPerfTime();

    // Compute destination image
    int nRadiusSizeY = dRadiusMax - dRadiusMin;
    double dAngleRange = 360.0;
    int nAngleSizeX = 0.0; // angle range / angle step
    double dAngleStep = 0.0;
    // angle step to perfom - look for the farther image corner from the center and compute angle for a small displacement in X and Y, take the smallest step
    double dCornerX[4] = { 0.0,	(double)nWidth,	(double)nWidth,	0.0 };
    double dCornerY[4] = { 0.0,	0.0,				(double)nHeight,	(double)nHeight };
    int nCorner = -1;
    double ddistmax = -1.0;

    //top left corner - 0 //top right corner - 1 //bottom right corner - 2 // bottom left corner - 3
    for (int k = 0; k < 4; k++)
    {
        double ddist = -1.0;
        ddist = sqrt(pow((dCenterX - dCornerX[k]), 2) + pow((dCenterY - dCornerY[k]), 2));
        if (ddistmax < ddist)
        {
            nCorner = k;
            ddistmax = ddist;
        }
    }

    double theta[3] = { 0.0,0.0,0.0 };
    theta[0] = atan2(dCenterY - dCornerY[nCorner], dCenterX - dCornerX[nCorner]);
    switch (nCorner)
    {
    case 0: //top left corner - 0
        theta[1] = atan2(dCenterY - dCornerY[nCorner], dCenterX - (dCornerX[nCorner] + 1));
        theta[2] = atan2(dCenterY - (dCornerY[nCorner] + 1), dCenterX - dCornerX[nCorner]);
        break;
    case 1://top right corner - 1 
        theta[1] = atan2(dCenterY - dCornerY[nCorner], dCenterX - (dCornerX[nCorner] - 1));
        theta[2] = atan2(dCenterY - (dCornerY[nCorner] + 1), dCenterX - dCornerX[nCorner]);
        break;
    case 2://bottom right corner - 2
        theta[1] = atan2(dCenterY - dCornerY[nCorner], dCenterX - (dCornerX[nCorner] - 1));
        theta[2] = atan2(dCenterY - (dCornerY[nCorner] - 1), dCenterX - dCornerX[nCorner]);
        break;
    case 3: // bottom left corner - 3
        theta[1] = atan2(dCenterY - dCornerY[nCorner], dCenterX - (dCornerX[nCorner] + 1));
        theta[2] = atan2(dCenterY - (dCornerY[nCorner] - 1), dCenterX - dCornerX[nCorner]);
        break;
    default: AfxMessageBox("error switch(nCorner)");
        break;
    }

    double diff1 = abs(theta[0] - theta[1]) / CST_PI * 180.0;
    double diff2 = abs(theta[0] - theta[2]) / CST_PI * 180.0;
    if (diff1 < diff2)
        dAngleStep = diff1;
    else
        dAngleStep = diff2;

    dAngleStep *= 0.5;

    nAngleSizeX = (int)(dAngleRange / dAngleStep);

    Prolonge_Bord_PolarCompute(m_cvNX, nAngleSizeX, dAngleStep, nRadiusSizeY, dRadiusMax, dRadiusMin, dCenterX, dCenterY);
    Prolonge_Bord_PolarCompute(m_cvNY, nAngleSizeX, dAngleStep, nRadiusSizeY, dRadiusMax, dRadiusMin, dCenterX, dCenterY);

    double dssEnd = GetPerfTime();
    if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("# Prolonge Polar X&Y done in %lf ms"), dssEnd - dssStart));

    return true;
}


bool CTreatPrepareData2::Prolonge_Bord_PolarCompute(Mat& image, int nAngleSizeX, double dAngleStep, int nRadiusSizeY, double dRadiusMax, double dRadiusMin, double dCenterX, double dCenterY)
{
    Mat oCVPolar = Mat::zeros(nRadiusSizeY + 1, nAngleSizeX + 1, CV_32F);

    // 	Mat dbg;	
    // 	double dminVal,dmaxVal;
    bool bSuccess = true;

    double dAngleMin = -180.0; // deg
    double dAngleMax = 180.0;  // deg
    int nIdxO = 0;
    int nIdxR = 0;
    double dAngl_dg;
    double dAngl_rd;
    double dRadius;
    for (dAngl_dg = dAngleMin; dAngl_dg <= dAngleMax; dAngl_dg += dAngleStep)
    {
        nIdxR = 0;
        dAngl_rd = dAngl_dg / 180.0 * CST_PI;
        for (dRadius = dRadiusMin; dRadius <= dRadiusMax; dRadius++)
        {
            double dX = dRadius * cos(dAngl_rd);
            double dY = dRadius * sin(dAngl_rd);

            int nX = _NArrondi(dX);
            int nY = _NArrondi(dY);
            nX += dCenterX;
            nY += dCenterY;

            if (nX >= 0 && nX < image.cols)
            {
                if (nY >= 0 && nY < image.rows)
                {
                    float fVal = image.at<float>(nY, nX);
                    oCVPolar.at<float>(nIdxR, nIdxO) = fVal;
                }

            }
            nIdxR++;
        }
        nIdxO++;
    }

    // 	minMaxLoc(oCVPolar,&dminVal,&dmaxVal);
    // 	oCVPolar.convertTo(dbg, CV_8U, 255.0/(dmaxVal - dminVal), - dminVal * 255.0f / (dmaxVal - dminVal));
    // 	bSuccess = cv::imwrite(_T("d:\\CvPolar.png"), dbg);


    for (int i = 0; i < oCVPolar.cols; i++)
    {
        int j = oCVPolar.rows - 1;
        float fvaldata = oCVPolar.at<float>(j, i);
        while ((j >= 0) && (fvaldata == 0.0f))
        {
            j--;
            if (j >= 0)
                fvaldata = oCVPolar.at<float>(j, i);
        }
        int nWaferEdgeRank = j - 5;
        int nCount = 0;
        for (j = nWaferEdgeRank + 1; (j < oCVPolar.rows) && ((nWaferEdgeRank - nCount) >= 0); j++, nCount++)
        {
            oCVPolar.at<float>(j, i) = oCVPolar.at<float>(nWaferEdgeRank - nCount, i);
        }
    }

    // 	minMaxLoc(oCVPolar,&dminVal,&dmaxVal);
    // 	oCVPolar.convertTo(dbg, CV_8U, 255.0/(dmaxVal - dminVal), - dminVal * 255.0f / (dmaxVal - dminVal));
    // 	bSuccess = cv::imwrite(_T("d:\\CvPolarMod.png"), dbg);

        // inverse polar
    nIdxO = 0;
    for (dAngl_dg = dAngleMin; dAngl_dg <= dAngleMax; dAngl_dg += dAngleStep)
    {
        nIdxR = 0;
        dAngl_rd = dAngl_dg / 180.0 * CST_PI;
        for (dRadius = dRadiusMin; dRadius <= dRadiusMax; dRadius++)
        {
            double dX = dRadius * cos(dAngl_rd);
            double dY = dRadius * sin(dAngl_rd);
            int nX = _NArrondi(dX);
            int nY = _NArrondi(dY);
            nX += dCenterX;
            nY += dCenterY;

            if (nX >= 0 && nX < image.cols)
            {
                if (nY >= 0 && nY < image.rows)
                {
                    image.at<float>(nY, nX) = oCVPolar.at<float>(nIdxR, nIdxO);
                }
            }

            nIdxR++;
        }
        nIdxO++;
    }

    // 	minMaxLoc(image,&dminVal,&dmaxVal);
    //  image.convertTo(dbg, CV_8U, 255.0/(dmaxVal - dminVal), - dminVal * 255.0f / (dmaxVal - dminVal));
    // 	bSuccess = cv::imwrite(_T("d:\\CvRes.png"), dbg);

    return bSuccess;
}

bool CTreatPrepareData2::Prolonge_Bord()
{
    int nWidth = m_Erode->GetCo();
    int nHeight = m_Erode->GetLi();

    double dssStart = GetPerfTime();

    if (m_vProlongeAreas.size() == 0)
    {
        // use standard area
        m_vProlongeAreas.push_back(Rect(0, 0, nWidth / 4, nHeight / 4));
        m_vProlongeAreas.push_back(Rect(nWidth / 4, 0, nWidth / 4, nHeight / 4));
        m_vProlongeAreas.push_back(Rect(2 * nWidth / 4, 0, nWidth / 4, nHeight / 4));
        m_vProlongeAreas.push_back(Rect(3 * nWidth / 4, 0, nWidth - (3 * nWidth / 4), nHeight / 4));
        m_vProlongeAreas.push_back(Rect(0, nHeight / 4, nWidth / 4, nHeight / 4));
        m_vProlongeAreas.push_back(Rect(0, 2 * nHeight / 4, nWidth / 4, nHeight / 4));
        m_vProlongeAreas.push_back(Rect(0, 3 * nHeight / 4, nWidth / 4, nHeight - (3 * nHeight / 4)));
        m_vProlongeAreas.push_back(Rect(nWidth / 4, 3 * nHeight / 4, nWidth / 4, nHeight - (3 * nHeight / 4)));
        m_vProlongeAreas.push_back(Rect(2 * nWidth / 4, 3 * nHeight / 4, nWidth / 4, nHeight - (3 * nHeight / 4)));
        m_vProlongeAreas.push_back(Rect(3 * nWidth / 4, 3 * nHeight / 4, nWidth - (3 * nWidth / 4), nHeight - (3 * nHeight / 4)));
        m_vProlongeAreas.push_back(Rect(3 * nWidth / 4, nHeight / 4, nWidth - (3 * nWidth / 4), nHeight / 4));
        m_vProlongeAreas.push_back(Rect(3 * nWidth / 4, 2 * nHeight / 4, nWidth - (3 * nWidth / 4), nHeight / 4));
    }

    Mat oMsk = Mat(nHeight, nWidth, CV_8U, m_Erode->GetData(), Mat::AUTO_STEP);
    Mat oMskInv;
    bitwise_not(oMsk, oMskInv);
    oMskInv = oMskInv - 254; // pour amener la dynamic à 0 - 1
    Mat dist;
    distanceTransform(oMskInv, dist, CV_DIST_L2, 3);

    Mat colorimage;
    cv::threshold(oMsk, colorimage, 0, 255, THRESH_BINARY);
    cvtColor(colorimage, colorimage, CV_GRAY2BGR);
    for (int i = 0; i < m_vProlongeAreas.size(); i++)
    {
        Point pt1(m_vProlongeAreas[i].x, m_vProlongeAreas[i].y);
        Point pt2(m_vProlongeAreas[i].x + m_vProlongeAreas[i].width, m_vProlongeAreas[i].y + m_vProlongeAreas[i].height);
        Scalar colorline;
        if (i % 2)
            colorline = Scalar(255, 0, 0);
        else
            colorline = Scalar(255, 212, 0);
        rectangle(colorimage, pt1, pt2, colorline, 3);
        std::string stxt;
        stxt = Fmt("%d", i + 1);
        putText(colorimage, stxt, cvPoint(pt1.x + 50, pt1.y + 100), FONT_HERSHEY_COMPLEX_SMALL, 2.5, colorline, 5, CV_AA);
    }

    try
    {
        imwrite("C:\\Temp\\NanoDBG\\ParamProlong.png", colorimage);
    }
    catch (...)
    {
        //do nothing
    }


    int nEnlargedWidth = nWidth + 2 * m_nMeanVignHalfSize;
    int nEnlargedHeight = nHeight + 2 * m_nMeanVignHalfSize;

    Rect rcC(m_nMeanVignHalfSize, m_nMeanVignHalfSize, nWidth, nHeight);
    m_Dist = Mat::ones(nEnlargedHeight, nEnlargedWidth, CV_32F) * 1000.0f;
    dist.copyTo(m_Dist(rcC));
    Mat _oEnlargedMsk = Mat::zeros(nEnlargedHeight, nEnlargedWidth, CV_8U);
    oMsk.copyTo(_oEnlargedMsk(rcC));
    Mat _oEnlargedMskInv = Mat::zeros(nEnlargedHeight, nEnlargedWidth, CV_8U);
    oMskInv.copyTo(_oEnlargedMskInv(rcC));
    Mat _oEnlarged_NX = Mat::zeros(nEnlargedHeight, nEnlargedWidth, CV_32F);
    m_cvNX.copyTo(_oEnlarged_NX(rcC));
    Mat _oEnlarged_NY = Mat::zeros(nEnlargedHeight, nEnlargedWidth, CV_32F);
    m_cvNY.copyTo(_oEnlarged_NY(rcC));

    // Init Thread array and Handles
    CWinThread** pThreadPtrArray = new CWinThread * [m_vProlongeAreas.size()];
    HANDLE* phEventArray = new HANDLE[m_vProlongeAreas.size()];

    for (int i = 0; i < m_vProlongeAreas.size(); i++)
    {
        phEventArray[i] = CreateEvent(0, FALSE, FALSE, 0);

        tProlongData* pData = new tProlongData();
        pData->_TreatPtr = (void*)this;
        pData->_HEvent = phEventArray[i];

        // Prepare Matrix
        Rect rcArea(m_vProlongeAreas[i].x, m_vProlongeAreas[i].y, m_vProlongeAreas[i].width + 2 * m_nMeanVignHalfSize, m_vProlongeAreas[i].height + 2 * m_nMeanVignHalfSize);
        pData->_Dist = m_Dist(rcArea);
        pData->_Nx = _oEnlarged_NX(rcArea);
        pData->_Ny = _oEnlarged_NY(rcArea);
        pData->_Mask = _oEnlargedMsk(rcArea);
        pData->_MaskInv = _oEnlargedMskInv(rcArea);

        pThreadPtrArray[i] = AfxBeginThread(&CTreatPrepareData2::static_ProlongeBordVignette, pData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
        pThreadPtrArray[i]->m_bAutoDelete = TRUE; // the thread delete its self after completion
        pThreadPtrArray[i]->ResumeThread();
    }
    // 
    DWORD dwEvent = WaitForMultipleObjects((DWORD)m_vProlongeAreas.size(), phEventArray, TRUE, INFINITE);
    double dssEnd = GetPerfTime();
    if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("# Prolonge All %d Areas in %lf ms"), m_vProlongeAreas.size(), dssEnd - dssStart));

    // copy back data to NX / NY
    _oEnlarged_NX(rcC).copyTo(m_cvNX);
    _oEnlarged_NY(rcC).copyTo(m_cvNY);

    for (int i = 0; i < m_vProlongeAreas.size(); i++)
    {
        CloseHandle(phEventArray[i]);
        phEventArray[i] = 0;
    }

    delete[] phEventArray;
    delete[] pThreadPtrArray;

    return true;
}

UINT CTreatPrepareData2::static_ProlongeBordVignette(void* p_pParameters)
{
    tProlongData* pData = static_cast<tProlongData*> (p_pParameters);
    CTreatPrepareData2* pObj = static_cast<CTreatPrepareData2*> (pData->_TreatPtr);
    pObj->ProlongeBordVignette((pData->_Dist), (pData->_Nx), (pData->_Ny), (pData->_Mask), (pData->_MaskInv), pData->_HEvent);

    delete pData;
    return 0;
}

bool CTreatPrepareData2::ProlongeBordVignette(cv::Mat& p_SubDist, cv::Mat& p_SubNx, cv::Mat& p_SubNy, cv::Mat& p_oSubMask, cv::Mat& p_oSubMaskInv, HANDLE p_hEvenDone)
{
    // Mask Bordures to avoid to be out the matrix
    Rect rc(m_nMeanVignHalfSize, m_nMeanVignHalfSize, p_SubNx.cols - 2 * m_nMeanVignHalfSize, p_SubNx.rows - 2 * m_nMeanVignHalfSize);
    p_oSubMaskInv = p_oSubMaskInv(rc);

    Mat oMskInv = Mat::zeros(p_SubNx.rows, p_SubNx.cols, CV_8U);
    p_oSubMaskInv.copyTo(oMskInv(rc));

    double dmin;
    cv::Point PtMin;
    cv::Scalar scX, scY;
    minMaxLoc(p_SubDist, &dmin, 0, &PtMin, 0, oMskInv);
    unsigned long ul = 0;
    unsigned long ulStop = (p_SubDist.rows * p_SubDist.cols) * 2L / 3L;
    float fDiffNoise = m_fNoiseHi - m_fNoiseLow;
    while ((dmin < m_dDistStop) && (PtMin.x > 0 && PtMin.y > 0) && (ul < ulStop) && !m_bEmergencyStop)
    {
        ++ul;
        try
        {
            Rect rc(PtMin.x - m_nMeanVignHalfSize, PtMin.y - m_nMeanVignHalfSize, 2 * m_nMeanVignHalfSize, 2 * m_nMeanVignHalfSize);
            scX = mean(p_SubNx(rc), p_oSubMask(rc));
            scY = mean(p_SubNy(rc), p_oSubMask(rc));

            p_SubNx.at<float>(PtMin) = scX[0] + m_fNoiseLow + (float)rand() / ((float)RAND_MAX / (fDiffNoise));
            p_SubNy.at<float>(PtMin) = scY[0] + m_fNoiseLow + (float)rand() / ((float)RAND_MAX / (fDiffNoise));
            p_SubDist.at<float>(PtMin) = 1000.f;
            p_oSubMask.at<BYTE>(PtMin) = 1;
            oMskInv.at<BYTE>(PtMin) = 0;

            minMaxLoc(p_SubDist, &dmin, 0, &PtMin, 0, oMskInv);
        }
        catch (cv::Exception exc)
        {
            LogThis(1, 4, Fmt(_T("ProlongeBord Exception = %s"), exc.msg.c_str()));
            break;
        }
        catch (...)
        {
            LogThis(1, 4, _T("ProlongeBord Unknown Exception"));
            break;
        }
    }

    SetEvent(p_hEvenDone);
    return true;
}

extern "C"  NT_DLL HRESULT Create(REFIID iid, void** ppvObject)
{
    CTreatPrepareData2* pObj = new CTreatPrepareData2();
    if (pObj == 0)
        return E_OUTOFMEMORY;
    return pObj->QueryInterface(iid, ppvObject);
}


bool CTreatPrepareData2::Degauchi(H3_MATRIX_FLT32* pMatScr, H3_MATRIX_UINT8* pMaskE)
{
    const unsigned long nx = pMaskE->GetCo();
    const unsigned long ny = pMaskE->GetLi();

    //échantillonnage
    unsigned long k;
    unsigned long iStep, jStep, kStep, kStep0;	//index image entiere (colonne, ligne, total)
    unsigned long Step = (unsigned long)m_nNUIStep;
    unsigned long FitOrder = (unsigned long)m_nNUIOrder;
    const unsigned long nny = (ny - 1L) / Step + 1L / Step + 1L / Step + 1L, nnx = (nx - 1L) / Step + 1L;
    H3_MATRIX_FLT32 Src(nny, nnx);
    H3_MATRIX_UINT8 MatMask(nny, nnx);
    H3_MATRIX_FLT64 MatResSurf;

    byte* pMMask = MatMask.GetData();
    float* pSrc = Src.GetData();
    float* ppSrc0 = pMatScr->GetData();

    //attention:il est probablement couteux en temps de fitter l'ensemble des données
    //faire un échantillonnage pour obtenir la fonction de fit
    kStep0 = 0L; k = 0L;
    for (jStep = 0L; jStep < ny; jStep += Step)
    {
        kStep = kStep0;
        for (iStep = 0L; iStep < nx; iStep += Step)
        {
            pMMask[k] = (pMaskE->GetData())[kStep];
            pSrc[k] = ppSrc0[kStep];
            k++;
            kStep += Step;
        }
        kStep0 += nx * Step;
    }

    //calcul du polynome
    //MatResSurf contient les coef du polynome
    if (!H3BestFitSurf(Src, MatMask, MatResSurf, FitOrder))
    {//H3BestFitSurf gère ses propres messages d'erreur

        return false;
    }

    // Degauchissage
    unsigned long x, y;//coordonnées dans un tableau
    double dx, dy; //les memes en double
    long index;
    unsigned long pow_i, pow_j;//puissance
    double ze;
    double Yj;
    double YjXi;
    long ind_k, ind_k0;//index dans MatResSurf

    for (y = 0L, index = 0L, ind_k0 = 0L; y < ny; y++)
    {
        dy = (double)y / double(Step);
        for (x = 0L; x < nx; x++, index++)
        {
            // 			if(!(pMaskE->GetData())[index])
            // 			{
            // 				ppSrc0[index]=0.0f;
            // 				continue;
            // 			}
            ze = 0.0f;
            ind_k0 = 0L;
            dx = (double)x / (double)Step;
            Yj = 1.0f;
            for (pow_j = 0L; pow_j <= FitOrder; pow_j++)
            {
                ind_k = ind_k0;
                YjXi = Yj;
                for (pow_i = 0L; pow_i <= FitOrder - pow_j; pow_i++)
                {
                    ze += YjXi * MatResSurf[ind_k];
                    YjXi *= dx;
                    ind_k++;
                }
                Yj *= dy;
                ind_k0 += (FitOrder + 1L);
            }
            ppSrc0[index] -= (float)ze;
        }
    }

    return true;
}

bool CTreatPrepareData2::H3BestFitSurf(const H3_MATRIX_FLT32& Src,
    const H3_MATRIX_UINT8& SrcMask,
    H3_MATRIX_FLT64& MatResSurf,
    long FitOrder, long MatVal,
    const H3_MATRIX_UINT8& MatCoef)
{
    CString str;

    long nSizeX = Src.GetCo();
    long nSizeY = Src.GetLi();

    long ValidElement = 0;
    long NumRow = 0;
    long i, j, k = 0;

    if ((nSizeX != SrcMask.GetCo()) || (nSizeY != SrcMask.GetLi()))
    {
        LogThis(1, 4, Fmt("{%s} H3BestFitSurf Error - Mask and Source Dimension are different", INanoTopoTreament::GetName()));
        return false;
    }

    //Initialiser les pointeurs sur les données
    H3_FLT32* pSource = Src.GetData();
    H3_UINT8* pMask = SrcMask.GetData();

    // Determination du nombre d'éléments valide à partir du masque
    for (long li = 0L; li < nSizeY; li++) {
        for (long co = 0; co < nSizeX; co++) {
            if ((*pMask > 0) /*&& (!_isnan(*pSource))*/)
                ValidElement++;
            pMask++; pSource++;
        }
    }

    if (ValidElement < MatVal)
    {
        LogThis(1, 4, Fmt("{%s} H3BestFitSurf Error - Fitting error n°2  ", INanoTopoTreament::GetName()));
        return false;
    }

    pMask = SrcMask.GetData();
    pSource = Src.GetData();

    // Creation et initialisation des matrices Y,XjYi,M,TM
    //H3_MATRIX_FLT64 M(ValidElement,MatVal),SURF(ValidElement,1),MATRESSURF(MatVal,1);
    H3_MATRIX_FLT64 MATRESSURF(MatVal, 1), SURF(ValidElement, 1);
    H3_MATRIX_FLT64 MM(MatVal, MatVal), MS(MatVal, 1L), MatTmp(MatVal, 1L);
    MM.Fill(0);
    MS.Fill(0);
    MatTmp.Fill(0);

    H3_FLT64 Yi, XjYi;
    // Calcul des coefficients de la matrice MatResSurf
    k = 0L;
    for (long Ypix = 0L; Ypix < nSizeY; Ypix++) {
        for (long Xpix = 0L; Xpix < nSizeX; Xpix++) {
            if ((*pMask > 0L)/* && (!_isnan(*pSource))*/)
            {
                NumRow = 0L;
                Yi = 1.0;
                for (i = 0L; i <= FitOrder; i++) {
                    XjYi = Yi;
                    for (j = 0L; j <= FitOrder; j++)
                    {
                        if (MatCoef(i, j) == 1L) {
                            MatTmp[NumRow] = XjYi;
                            NumRow++;
                        }
                        XjYi *= Xpix;
                    }
                    Yi *= Ypix;
                }
                SURF[k] = *pSource;

                for (i = 0L; i < MatVal; i++)
                {
                    MM(i, i) += MatTmp[i] * MatTmp[i];
                    for (j = i + 1L; j < MatVal; j++)
                        MM(j, i) += MatTmp[i] * MatTmp[j];
                    MS[i] += MatTmp[i] * (*pSource);
                }

                for (i = 0L; i < MatVal; i++)
                    for (j = i + 1L; j < MatVal; j++)
                        MM(i, j) = MM(j, i);

                k++;
            }
            pMask++; pSource++;
        }
    }

    // Resoudre avec le critere des moindres carres

    //1 normalisation
    H3_MATRIX_FLT64 MatMaxCo(1L, MatVal), MatMaxLi(MatVal, 1L);
    MatMaxCo.Fill(0.0);
    MatMaxLi.Fill(0.0);

    for (i = 0L; i < MatVal; i++) {
        for (j = 0L; j < MatVal; j++) {
            MatMaxCo[j] = __max(MatMaxCo[j], fabs(MM(i, j)));
            MatMaxLi[i] = __max(MatMaxLi[i], fabs(MM(i, j)));
        }
    }
    for (i = 0L; i < MatVal; i++) {
        MatMaxCo[i] = sqrt(MatMaxCo[i]);
        MatMaxLi[i] = sqrt(MatMaxLi[i]);
    }

    H3_MATRIX_FLT64 MM_tmp(MatVal, MatVal);
    for (i = 0L; i < MatVal; i++)
        for (j = 0L; j < MatVal; j++)
            MM_tmp(i, j) = MM(i, j) / (MatMaxCo[j] * MatMaxLi[i]);

    //2 inversion
    H3_MATRIX_FLT64 iMM_tmp = MM_tmp.Inv();

    if (iMM_tmp.GetSize() == MatVal * MatVal) {
        for (i = 0L; i < MatVal; i++)
            for (j = 0L; j < MatVal; j++)
                iMM_tmp(i, j) /= MatMaxCo[i] * MatMaxLi[j];

        MATRESSURF = iMM_tmp * MS;
    }
    else {
        LogThis(1, 4, Fmt("{%s} H3BestFitSurf Error - Fitting error n°3  ", INanoTopoTreament::GetName()));
        return false;
    }

    if (MATRESSURF.GetSize() != MatVal)
    {
        LogThis(1, 4, Fmt("{%s} H3BestFitSurf Error - Fitting error n°4  ", INanoTopoTreament::GetName()));
        return false;
    }

    k = 0L;
    for (i = 0L; i <= FitOrder; i++) {
        for (j = 0L; j <= FitOrder; j++) {
            if (MatCoef(i, j) == 1L) {
                MatResSurf(i, j) = MATRESSURF[k];
                k++;
            }
            else
                MatResSurf(i, j) = 0.0;
        }
    }
    return true;
}

const double g_dNaN = H3GetFPdNaN();
bool CTreatPrepareData2::H3BestFitSurf(const H3_MATRIX_FLT32& Src, const H3_MATRIX_UINT8& SrcMask, H3_MATRIX_FLT64& MatResSurf, long FitOrder)
{
    long i, j;
    long Imax = FitOrder + 1;
    long Jmax = FitOrder + 1;

    H3_MATRIX_UINT8 MatCoef(Imax, Jmax);
    long MatVal = 0;
    MatCoef.Fill(0L);

    for (i = 0L; i < Imax; i++)
        for (j = 0L; j < Jmax - i; j++)
        {
            MatCoef(i, j) = 1L;
            MatVal++;
        }
    MatResSurf.ReAlloc(MatCoef.GetLi(), MatCoef.GetCo());
    MatResSurf.Fill(g_dNaN);
    return(H3BestFitSurf(Src, SrcMask, MatResSurf, FitOrder, MatVal, MatCoef));
}
