#include "StdAfx.h"
#include "TreatReconstruct0.h"

#include "H3MatrixOperations.h"

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include "opencv2/imgproc/imgproc.hpp"
using namespace cv;

#include "FreeImagePlus.h"
#ifdef _DEBUG
	#pragma comment (lib , "FreeImaged")
	#pragma comment (lib , "FreeImagePlusd")
#else
	#pragma comment (lib , "FreeImage")
	#pragma comment (lib , "FreeImagePlus")
#endif

#include <Eigen/Sparse>
#include <Eigen/SparseCholesky>

using namespace Eigen;

const double	g_dEPSILON = std::numeric_limits<double>::epsilon();
const float		g_fEPSILON = std::numeric_limits<float>::epsilon();
const  double	g_dPI=3.14159265358979;

//#include <omp.h> // for openMP directive //=> check also that /openMP is active in compiler properties

// colormap = get(gcf,'Colormap'); en matlab
/*
0         0    0.5625
	0         0    0.6250
	0         0    0.6875
	0         0    0.7500
	0         0    0.8125
	0         0    0.8750
	0         0    0.9375
	0         0    1.0000
	0    0.0625    1.0000
	0    0.1250    1.0000
	0    0.1875    1.0000
	0    0.2500    1.0000
	0    0.3125    1.0000
	0    0.3750    1.0000
	0    0.4375    1.0000
	0    0.5000    1.0000
	0    0.5625    1.0000
	0    0.6250    1.0000
	0    0.6875    1.0000
	0    0.7500    1.0000
	0    0.8125    1.0000
	0    0.8750    1.0000
	0    0.9375    1.0000
	0    1.0000    1.0000
	0.0625    1.0000    0.9375
	0.1250    1.0000    0.8750
	0.1875    1.0000    0.8125
	0.2500    1.0000    0.7500
	0.3125    1.0000    0.6875
	0.3750    1.0000    0.6250
	0.4375    1.0000    0.5625
	0.5000    1.0000    0.5000
	0.5625    1.0000    0.4375
	0.6250    1.0000    0.3750
	0.6875    1.0000    0.3125
	0.7500    1.0000    0.2500
	0.8125    1.0000    0.1875
	0.8750    1.0000    0.1250
	0.9375    1.0000    0.0625
	1.0000    1.0000         0
	1.0000    0.9375         0
	1.0000    0.8750         0
	1.0000    0.8125         0
	1.0000    0.7500         0
	1.0000    0.6875         0
	1.0000    0.6250         0
	1.0000    0.5625         0
	1.0000    0.5000         0
	1.0000    0.4375         0
	1.0000    0.3750         0
	1.0000    0.3125         0
	1.0000    0.2500         0
	1.0000    0.1875         0
	1.0000    0.1250         0
	1.0000    0.0625         0
	1.0000         0         0
	0.9375         0         0
	0.8750         0         0
	0.8125         0         0
	0.7500         0         0
	0.6875         0         0
	0.6250         0         0
	0.5625         0         0
	0.5000         0         0
*/

#define	DBG_REC_COEFMAP				0X00000010
#define DBG_REC_H					0X00000020
#define DBG_SKIP_AFFINETRANSFORM	0x00000040
#define DBG_SKIP_BANDTHREAD			0x00000080
#define DBG_SKIP_QUADTHREAD			0x00000100	

CTreatReconstruct0::CTreatReconstruct0()
{	
	InitializeCriticalSection(&m_sCriticalSection);
	for(int i =0; i<NB_QUADRANT; i++)
	{
		m_hEventQDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventQDone[i] != 0);	
	}
	for(int i =0; i<4; i++)
	{
		m_hEventBandDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventBandDone[i] != 0);	
	}

	m_lVignetteSize = 128;  // si m_lVignette = 128 
	m_nMargin		= 0;	// rab
	m_nVois			= 10;	// voisinage calcule de Z, cf compute vignette
	m_dSigma		= 0.5;	// en expert
	m_dAlpha		= 0.1;	// en expert
	m_uDbgFlag		= 0;
	m_uRegFlag		= 0;
}

CTreatReconstruct0::~CTreatReconstruct0()
{
	for(int i =0; i<4; i++)
	{
		if (m_hEventQDone[i] != 0)
		{
			CloseHandle(m_hEventQDone[i]);
			m_hEventQDone[i] = 0;
		}
	}

	for(int i =0; i<4; i++)
	{
		if (m_hEventBandDone[i] != 0)
		{
			CloseHandle(m_hEventBandDone[i]);
			m_hEventBandDone[i] = 0;
		}
	}
	DeleteCriticalSection(&m_sCriticalSection);
}

bool CTreatReconstruct0::Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder)
{ 
	m_matMaskDilate.reset(new H3_MATRIX_UINT8());
	m_matMaskErode.reset(new H3_MATRIX_UINT8());

	m_CoefMat.reset(new  H3_MATRIX_FLT32());
	for(int i = 0; i<NB_QUADRANT; i++)
	{
		m_R[i].reset(new  H3_MATRIX_FLT32());
		m_IterationMap[i].reset(new  H3_MATRIX_FLT32());
	
	}
	m_H.reset(new  H3_MATRIX_FLT32());

	int nInt = (int) m_lVignetteSize;
 	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("PartSize"), nInt))
		LogThis(1,3,Fmt(_T("{%s} Could not find [PartSize] Parameter"), INanoTopoTreament::GetName()));
	m_lVignetteSize = nInt;

	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("PartMargin"), m_nMargin))
		LogThis(1,3,Fmt(_T("{%s} Could not find [PartMargin] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("MeanNeightborSize"), m_nVois))
		LogThis(1,3,Fmt(_T("{%s} Could not find [MeanNeightborSize] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmDbl(p_pPrmMap,_T("GaussSigma"), m_dSigma))
		LogThis(1,3,Fmt(_T("{%s} Could not find [GaussSigma] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmDbl(p_pPrmMap,_T("ThreshAlpha"), m_dAlpha))
		LogThis(1,3,Fmt(_T("{%s} Could not find [ThreshAlpha] Parameter"), INanoTopoTreament::GetName()));
	
	m_uRegFlag = 0;
	if( ! GetRegistryFlag(m_uRegFlag, DBG_REC_H))
		LogThis(1,3,Fmt(_T("{%s} Could not reach Registry flag Parameter"), INanoTopoTreament::GetName()));

	if((m_uDbgFlag|m_uRegFlag) & DBG_SHOW_DISPLAY)
	{
		cvNamedWindow( "Reconstruct window", CV_WINDOW_NORMAL| CV_WINDOW_KEEPRATIO);// Create a window for display.
	}

	return true;
}

bool CTreatReconstruct0::Exec( const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm )
{
	double dstart = GetPerfTime();

	DWORD dwEvent = 0;
	void *p = 0;
	int nSaveData = 0;
	unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag; 

	if(FindTreatPrmPtr(p_InputsPrm,"Save",p))
	{
		nSaveData = *((int*) p); 	
	}
	tData2Save* pSavData = nSaveData ? new tData2Save : nullptr; // SHOULD be deleted in this::SaveData	or any error exit or exception

	if(FindTreatPrmPtr(p_InputsPrm,"LotID",p))
	{
		m_csLotID = *((CString*) p); 	
	}


	shared_ptr<void> pvMaskDilate;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"MaskD",pvMaskDilate))
	{
		m_matMaskDilate = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskDilate);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("Mask Dilate Size = %d x %d"), m_matMaskDilate->GetCo(),m_matMaskDilate->GetLi()));
	}

	shared_ptr<void> pvMaskErode;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"MaskE",pvMaskErode))
	{
		m_matMaskErode = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskErode);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("Mask Erode Size = %d x %d"), m_matMaskErode->GetCo(),m_matMaskErode->GetLi()));
	}

	shared_ptr<void> pvNX;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"NX",pvNX))
	{
		m_matNX = static_pointer_cast<H3_MATRIX_FLT32> (pvNX);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("NX Size = %d x %d"), m_matNX->GetCo(),m_matNX->GetLi()));
	}

	shared_ptr<void> pvNY;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"NY",pvNY))
	{
		m_matNY = static_pointer_cast<H3_MATRIX_FLT32> (pvNY);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("NY Size = %d x %d"), m_matNY->GetCo(),m_matNY->GetLi()));
	}

	// we assume here that all matrices has the same size
	ASSERT(m_matNX->GetCo() == m_matNY->GetCo());
	ASSERT(m_matNX->GetLi() == m_matNY->GetLi());
	ASSERT(m_matNX->GetCo() == m_matMaskDilate->GetCo());
	ASSERT(m_matNX->GetLi() == m_matMaskDilate->GetLi());
	ASSERT(m_matMaskErode->GetCo() == m_matMaskDilate->GetCo());
	ASSERT(m_matMaskErode->GetLi() == m_matMaskDilate->GetLi());
 
	m_lOriginalWidth_px		= m_matNX->GetCo();
	m_lOriginalHeight_px	= m_matNX->GetLi();

	//
	// Create Coef Map
	//
	ASSERT(m_CoefMat.get() != nullptr);
	*(m_CoefMat.get()) = CH3MatrixOperations::Alta_CreateCoefMap3(m_lVignetteSize);

	if(pSavData && (uDbgFlag & DBG_REC_COEFMAP))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("CoefMatH3");
		elt._spT.reset(new H3_MATRIX_FLT32(*m_CoefMat.get())); // copy since data altered in following treatments
		elt._bImg = true;
		elt._bHbf = false;
		elt._bBin = true;
		pSavData->spListF32.push_back(elt);
	}

	// Init new H
	m_H.reset(new  H3_MATRIX_FLT32(m_lOriginalHeight_px, m_lOriginalWidth_px));

	// Convert in Open CV use love to use ROI to avoid copy-hell
	Mat oCVMatMaskD	=  Mat(m_lOriginalHeight_px,	m_lOriginalWidth_px,	CV_8U,	m_matMaskDilate->GetData(),	Mat::AUTO_STEP);
	Mat oCVMatMaskE	=  Mat(m_lOriginalHeight_px,	m_lOriginalWidth_px,	CV_8U,	m_matMaskErode->GetData(),	Mat::AUTO_STEP);
	Mat oCVMatNX	=  Mat(m_lOriginalHeight_px,	m_lOriginalWidth_px,	CV_32F,	m_matNX->GetData(),			Mat::AUTO_STEP);
	Mat oCVMatNY	=  Mat(m_lOriginalHeight_px,	m_lOriginalWidth_px,	CV_32F,	m_matNY->GetData(),			Mat::AUTO_STEP);
	Mat oCVHm		=  Mat(m_lOriginalHeight_px,	m_lOriginalWidth_px,	CV_32F,	m_H->GetData(),				Mat::AUTO_STEP);

	m_oCVCoefMat	=  Mat(m_CoefMat->GetLi(),	m_CoefMat->GetCo(), CV_32F,	m_CoefMat->GetData(),	Mat::AUTO_STEP);

	// Enlarge in function of margin (rab), NX, NY & Dilated Mask
	m_oCVMatMaskD	= Mat::zeros(m_lOriginalHeight_px + 2 * m_nMargin  , m_lOriginalWidth_px + 2* m_nMargin,	CV_8U);
	m_oCVMatNX		= Mat::zeros(m_lOriginalHeight_px + 2 * m_nMargin  , m_lOriginalWidth_px + 2* m_nMargin,	CV_32F);
	m_oCVMatNY		= Mat::zeros(m_lOriginalHeight_px + 2 * m_nMargin  , m_lOriginalWidth_px + 2* m_nMargin,	CV_32F);

	Rect rcRoi(m_nMargin, m_nMargin, m_lOriginalWidth_px, m_lOriginalHeight_px);
	oCVMatMaskD.copyTo(m_oCVMatMaskD(rcRoi));
	oCVMatNX.copyTo(m_oCVMatNX(rcRoi));
	oCVMatNY.copyTo(m_oCVMatNY(rcRoi));

	int nLi2	= m_lOriginalHeight_px / 2;
	int nCo2	= m_lOriginalWidth_px / 2;
	int nVign2	= m_lVignetteSize / 2;

	// Init R(i) and IterationMap(i) for quadrant 
	for(int i = 0; i<NB_QUADRANT; i++)
	{
		m_oCVR[i]				=  Mat::zeros(nLi2,	nCo2,	CV_32F);
		m_oCVIterationMap[i]	=  Mat(nLi2,	nCo2,	CV_32F,	 Scalar(g_fEPSILON));
	}

	// Initialize bands
	// Band 12 (between Q1 & Q2 - Top Band) 
	m_oCVBandR[0]				=  Mat::zeros(nLi2,	m_lVignetteSize, CV_32F);
	m_oCVBandIterationMap[0]	=  Mat		 (nLi2,	m_lVignetteSize, CV_32F, Scalar(g_fEPSILON));
	// Band 34 (between Q3 & Q4 - Bottom Band) 
	m_oCVBandR[1]				=  Mat::zeros(nLi2,	m_lVignetteSize, CV_32F);
	m_oCVBandIterationMap[1]	=  Mat		 (nLi2,	m_lVignetteSize, CV_32F, Scalar(g_fEPSILON));
	// Band 13 (between Q1 & Q3 - Left Band) 
	m_oCVBandR[2]				=  Mat::zeros(m_lVignetteSize, nCo2, CV_32F);
	m_oCVBandIterationMap[2]	=  Mat		 (m_lVignetteSize, nCo2, CV_32F,	 Scalar(g_fEPSILON));
	// Band 24 (between Q2 & Q4 - Right Band) 
	m_oCVBandR[3]				=  Mat::zeros(m_lVignetteSize, nCo2, CV_32F);
	m_oCVBandIterationMap[3]	=  Mat		 (m_lVignetteSize, nCo2, CV_32F,	 Scalar(g_fEPSILON));
	// Middle aka img Center
	m_oCVMidR					=  Mat::zeros(m_lVignetteSize,	m_lVignetteSize,	CV_32F);
	m_oCVMidIterationMap		=  Mat(m_lVignetteSize,	m_lVignetteSize,	CV_32F,	 Scalar(g_fEPSILON));

	Eigen::setNbThreads(1);

	//
	// Start Threads Bands
	//
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("##### Start Band Area Threads")));
	double dssStartBands = GetPerfTime();

	CWinThread* pThreadB12 = AfxBeginThread(&CTreatReconstruct0::DoBand12, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThreadB12->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadB12->ResumeThread();
	
	Sleep(10);

	CWinThread* pThreadB34 = AfxBeginThread(&CTreatReconstruct0::DoBand34, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThreadB34->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadB34->ResumeThread();

	Sleep(10);

	CWinThread* pThread13 = AfxBeginThread(&CTreatReconstruct0::DoBand13, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThread13->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThread13->ResumeThread();

	Sleep(10);

	CWinThread* pThreadB24 = AfxBeginThread(&CTreatReconstruct0::DoBand24, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThreadB24->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadB24->ResumeThread();
 
	Sleep(10);

	//
	// Start Threads Quadrants
	//
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("##### Start Quadrant Threads")));
	double dssStartQuadrant = GetPerfTime();

	CWinThread* pThreadQ1 = AfxBeginThread(&CTreatReconstruct0::DoQ1, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThreadQ1->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ1->ResumeThread();

	//WaitForSingleObject(m_hEventQDone[0], INFINITE);
	Sleep(10);

	CWinThread* pThreadQ2 = AfxBeginThread(&CTreatReconstruct0::DoQ2, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThreadQ2->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ2->ResumeThread();

	//WaitForSingleObject(m_hEventQDone[1],  INFINITE);
	Sleep(10);

	CWinThread* pThreadQ3 = AfxBeginThread(&CTreatReconstruct0::DoQ3, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThreadQ3->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ3->ResumeThread();

	//WaitForSingleObject(m_hEventQDone[2], INFINITE);
	Sleep(10);

	CWinThread* pThreadQ4 = AfxBeginThread(&CTreatReconstruct0::DoQ4, this, THREAD_PRIORITY_NORMAL/*THREAD_PRIORITY_NORMAL*/, 0, CREATE_SUSPENDED);
	pThreadQ4->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ4->ResumeThread();

	// Meanwhile initialize R and iteration map globl for the wafer
	Mat Rg = Mat::zeros(m_lOriginalHeight_px,m_lOriginalWidth_px, CV_32F);
	Mat IterMapg = Mat(m_lOriginalHeight_px,m_lOriginalWidth_px, CV_32F, Scalar(g_fEPSILON));

	// Meanwhile Perform vignette in the center image
	double dssStartMid =  GetPerfTime();
	DoMiddle();
	double dssEndMid = GetPerfTime();
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done Middle in %lf ms"), dssEndMid - dssStartMid));

	Rg(		 Rect(nCo2 - nVign2,	nLi2-nVign2,	m_lVignetteSize,	m_lVignetteSize))	+= m_oCVMidR;				//Mid Center
	IterMapg(Rect(nCo2 - nVign2,	nLi2-nVign2,	m_lVignetteSize,	m_lVignetteSize))	+= m_oCVMidIterationMap;	//Mid Center
	m_oCVMidR.release();
	m_oCVMidIterationMap.release();

	dwEvent = WaitForMultipleObjects(sizeof(m_hEventBandDone) / sizeof(HANDLE), m_hEventBandDone, TRUE, INFINITE);
	double dssEndBands = GetPerfTime();
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done All %d Bands in %lf ms"), 4, dssEndBands - dssStartBands));

	// Meanwhile we handle Ri and iteration maps from Bands

	Rg(Rect(nCo2-nVign2,	0,				m_lVignetteSize,	nLi2))		+= m_oCVBandR[0]; //B12
	Rg(Rect(nCo2-nVign2,	nLi2,			m_lVignetteSize,	nLi2))		+= m_oCVBandR[1]; //B34
	Rg(Rect(0,				nLi2-nVign2,	nCo2,		m_lVignetteSize))	+= m_oCVBandR[2]; //B13
	Rg(Rect(nCo2,			nLi2-nVign2,	nCo2,		m_lVignetteSize))	+= m_oCVBandR[3]; //B34

	IterMapg(Rect(nCo2-nVign2,	0,				m_lVignetteSize,	nLi2))		+= m_oCVBandIterationMap[0]; //B12
	IterMapg(Rect(nCo2-nVign2,	nLi2,			m_lVignetteSize,	nLi2))		+= m_oCVBandIterationMap[1]; //B34
	IterMapg(Rect(0,			nLi2-nVign2,	nCo2,		m_lVignetteSize))	+= m_oCVBandIterationMap[2]; //B13
	IterMapg(Rect(nCo2,			nLi2-nVign2,	nCo2,		m_lVignetteSize))	+= m_oCVBandIterationMap[3]; //B34

	for(int i = 0; i<4; i++)
	{
		m_oCVBandR[i].release();
		m_oCVBandIterationMap[i].release();
	}

	// Meanwhile prepare erode mask
	Mat oMskE32f;
	oCVMatMaskE.convertTo(oMskE32f,CV_32F);

	dwEvent = WaitForMultipleObjects(sizeof(m_hEventQDone) / sizeof(HANDLE), m_hEventQDone, TRUE, INFINITE);
	double dssEndQuadrant = GetPerfTime();
	LogThis(1,1,Fmt(_T("# Done All %d Quadrants in %lf ms"), NB_QUADRANT, dssEndQuadrant - dssStartQuadrant));

	//  we handle Ri and iteration maps from quadrants
	
	Rg(Rect(0,		0,		nCo2,	nLi2 )) += m_oCVR[0]; //Q1
	Rg(Rect(nCo2,	0,		nCo2,	nLi2))	+= m_oCVR[1]; //Q2
	Rg(Rect(0,		nLi2,	nCo2,	nLi2))	+= m_oCVR[2]; //Q3
	Rg(Rect(nCo2,	nLi2,	nCo2,	nLi2))  += m_oCVR[3]; //Q4

	IterMapg(Rect(0,	0,		nCo2,	nLi2 )) += m_oCVIterationMap[0]; //Q1
	IterMapg(Rect(nCo2,	0,		nCo2,	nLi2))	+= m_oCVIterationMap[1]; //Q2
	IterMapg(Rect(0,	nLi2,	nCo2,	nLi2))	+= m_oCVIterationMap[2]; //Q3
	IterMapg(Rect(nCo2,	nLi2,	nCo2,	nLi2))  += m_oCVIterationMap[3]; //Q4

	for(int i = 0; i<4; i++)
	{
		m_oCVR[i].release();
		m_oCVIterationMap[i].release();
	}

	//
	// Finalize and compute Hm
	//
	oCVHm = oMskE32f.mul(Rg / IterMapg); 

	if(pSavData &&  (uDbgFlag & DBG_REC_H))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("Hm");
		elt._spT.reset(new H3_MATRIX_FLT32(*m_H.get())); // copy since data altered in following treatments
		elt._bImg = true;
		elt._bHbf = false;
		elt._bBin = true;
		pSavData->spListF32.push_back(elt);
	}


	//
	// launch saving thread if needed
	//
	if(nSaveData != 0 && pSavData != nullptr)
	{
		static UINT uCount = 0;
		uCount++;
		pSavData->nId		= uCount;
		pSavData->csName	= INanoTopoTreament::GetName();
		pSavData->csPath	= _T("C:\\Altasight\\Nano\\Data");
		void* pcs;
		if(FindTreatPrmPtr(p_InputsPrm,"OutPath",pcs))
		{
			pSavData->csPath = *((CString*) pcs); 	
		}
		pSavData->csPath += _T("\\Dbg\\");
		pSavData->csPath += m_csLotID;

		CWinThread* pThread = AfxBeginThread(&CTreatReconstruct0::SaveData, pSavData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
		if (pThread == 0)
		{
			LogThis(1,4,Fmt(_T("(%s) AfxBeginThread() failed.\n"),INanoTopoTreament::GetName()));
			return false;
		}
		pThread->m_bAutoDelete = TRUE; // the thread delete its self after completion
		pThread->ResumeThread();
	}

	double dEnd = GetPerfTime();
	if(FindTreatPrmPtr(p_OutputsPrm,"CS",p))
	{
		CString* pCs = (CString*) p;
		(*pCs) = Fmt(_T("(%s) exec in %0.3f ms "), INanoTopoTreament::GetName(), dEnd - dstart);
	}

	AddTreatPrmSharedPtr(p_OutputsPrm,"H",shared_ptr<void>(m_H));

	if(uDbgFlag & DBG_SHOW_DISPLAY)
	{
		double minVal, maxVal;
		minMaxLoc(oCVHm, &minVal, &maxVal); //find minimum and maximum intensities
		Mat draw;
		oCVHm.convertTo(draw, CV_8U, 255.0/(maxVal - minVal), - minVal * 255.0f / (maxVal - minVal));

		imshow( "Reconstruct window", draw );
	}

	return true;
}


void CTreatReconstruct0::ComputeVignette( int p_nIdxX,int p_nIdxY, cv::Mat& p_oNNX,cv::Mat& p_oNNY,cv::Mat& p_oMM,cv::Mat& p_oR,cv::Mat& p_oIterMap, int p_nVoisX, int p_nVoisY )
{
	// Affine Transform optim
	Mat Z = AffineTransform(p_oNNX,p_oNNY);
	
// 	Mat Z = Mat::ones(p_oNNX.rows,p_oNNX.cols,CV_32F);
// 	for(int i=32;i<64+32;i++ )
// 		for(int j=32;j<64+32;j++)
// 			Z.at<float>(i,j)=3.0f;
// 
// 	for(int i=32+16;i<32+16+32;i++ )
// 		for(int j=32+16;j<32+16+32;j++)
// 			Z.at<float>(i,j)=6.0f;
// 
// 	for(int i=96;i<96+10;i++ )
// 		for(int j=96;j<96+10;j++)
// 			Z.at<float>(i,j)=16.0f;

	// On enleve la marge et on Apply le Mask
 	p_oMM		= p_oMM(Range(m_nMargin,p_oMM.rows-m_nMargin),Range(m_nMargin,p_oMM.cols-m_nMargin));
 	p_oMM.convertTo(p_oMM,CV_32F); // OpenCV specific on doit multtipler des matrice de même type !!
	Z		= Z(Range(m_nMargin,Z.rows-m_nMargin),Range(m_nMargin,Z.cols-m_nMargin));
	Z		= Z.mul(p_oMM);

 	//int nloff	= 3 * m_lVignetteSize / 4; // correspond à (m_lVignetteSize/2 + (m_lVignetteSize/2)/2)) 
 	//Rect rcVoisinage(p_nIdxX+m_nMargin+nloff,p_nIdxY+m_nMargin+nloff,m_nVois,m_nVois);
	
	//p_nVoisX = p_nVoisY	= (m_lVignetteSize / 2) - ( m_nVois / 2);
	
	Rect rcVoisinage(p_nIdxX+p_nVoisX,p_nIdxY+p_nVoisY,m_nVois,m_nVois);
	if(p_nVoisX>=0 && p_nVoisY>=0)
	{
		Mat oPi = p_oR(rcVoisinage) / p_oIterMap(rcVoisinage);
		Mat oOffset = Z(Rect(p_nVoisX,p_nVoisY,m_nVois, m_nVois)) - oPi;
		Scalar scOff = mean(oOffset);
		Z = Z - scOff;
	}

	// application de la pyramide et du masque réduit sans margin
	Z = Z.mul(m_oCVCoefMat.mul(p_oMM)); 
	//Z = Z.mul(m_oCVCoefMat); 

	// mise à jour des matrices d'iteration map et Ri dans le bon voisinage de la matrix parent
	rcVoisinage = Rect(p_nIdxX,p_nIdxY,m_lVignetteSize,m_lVignetteSize);
	p_oR(rcVoisinage) += Z;
	p_oIterMap(rcVoisinage) += m_oCVCoefMat;
	//p_oIterMap(rcVoisinage) += Mat::ones(m_oCVCoefMat.rows,m_oCVCoefMat.cols,CV_32F);
}


cv::Mat CTreatReconstruct0::AffineTransform( Mat& p_oNNX, Mat& p_oNNY )
{
// for debug purpose
// 	FILE* pfileNNX = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\NNXcpp.csv"),"w");
// 	FILE* pfileNNY = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\NNYcpp.csv"),"w");
// 	for(int i=0;i<p_oNNX.rows;i++)
// 	{
// 		if (pfileNNX) 
// 			fprintf( pfileNNX	, "%f; %f; %f; %f; %f; %f; %f; %f; %f; %f;\n", p_oNNX.at<float>(i,0),p_oNNX.at<float>(i,1),p_oNNX.at<float>(i,2),p_oNNX.at<float>(i,3),p_oNNX.at<float>(i,4),p_oNNX.at<float>(i,5),p_oNNX.at<float>(i,6),p_oNNX.at<float>(i,7), p_oNNX.at<float>(i,8),p_oNNX.at<float>(i,9));
// 		if (pfileNNY) 
// 			fprintf( pfileNNY	, "%f; %f; %f; %f; %f; %f; %f; %f; %f; %f;\n", p_oNNY.at<float>(i,0),p_oNNY.at<float>(i,1),p_oNNY.at<float>(i,2),p_oNNY.at<float>(i,3),p_oNNY.at<float>(i,4),p_oNNY.at<float>(i,5),p_oNNY.at<float>(i,6),p_oNNY.at<float>(i,7), p_oNNY.at<float>(i,8),p_oNNY.at<float>(i,9));
// 
// 	}
// 	fclose(pfileNNX);
// 	fclose(pfileNNY);

/*	char Buffer[2048]; 
	int nLigneIdx = 0;
	int nColIdx = 0;
	char seps[]   = " ;\t\n";
	char *token = NULL;
	char *next_token = NULL;
	FILE* pfileNNXmatlab = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\NNXMatlab.csv"),"r");
	while ( EOF != fscanf( pfileNNXmatlab, "%s\n", Buffer ))
	{
		token = strtok_s( Buffer, seps, &next_token);
		_CRT_FLOAT crtflt;
		nColIdx = 0;
		while ((token != NULL))
		{
			int nretval = _atoflt(&crtflt, token);
			if(nretval == 0)
				p_oNNX.at<float>(nLigneIdx,nColIdx)  = crtflt.f;
			else
				ASSERT(0);
			token = strtok_s( NULL, seps, &next_token);
			nColIdx++;
		}
		nLigneIdx++;
	}
	fclose(pfileNNXmatlab);

	FILE* pfileNNYmatlab = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\NNYMatlab.csv"),"r");
	nLigneIdx = 0;
	while ( EOF != fscanf( pfileNNYmatlab, "%s\n", Buffer ))
	{
		token = strtok_s( Buffer, seps, &next_token);
		_CRT_FLOAT crtflt;
		nColIdx = 0;
		while ((token != NULL))
		{
			int nretval = _atoflt(&crtflt, token);
			if(nretval == 0)
				p_oNNY.at<float>(nLigneIdx,nColIdx)  = crtflt.f;
			else
				ASSERT(0);
			token = strtok_s( NULL, seps, &next_token);
			nColIdx++;
		}
		nLigneIdx++;
	}
	fclose(pfileNNYmatlab);*/

	ASSERT(p_oNNX.rows == m_lVignetteSize);
	ASSERT(p_oNNX.cols == m_lVignetteSize);
	ASSERT(p_oNNX.rows == p_oNNY.rows);
	ASSERT(p_oNNX.cols == p_oNNY.cols);

	if((m_uDbgFlag|m_uRegFlag) & DBG_SKIP_AFFINETRANSFORM)
	{
		return Mat::ones(p_oNNX.rows,p_oNNX.cols,CV_32F);
	}


// 	float f100 = p_oNNX.at<float>(0,0);
// 	float f110 = p_oNNX.at<float>(1,0);
// 	float f120 = p_oNNX.at<float>(2,0);
// 	float f101 = p_oNNX.at<float>(0,1);
// 	float f102 = p_oNNX.at<float>(0,2);

	//double dss= GetPerfTime();

	Mat t11 =  Mat::zeros(p_oNNX.rows,p_oNNX.cols,CV_32F);
	Mat t22 =  Mat::zeros(p_oNNX.rows,p_oNNX.cols,CV_32F);
	Mat t12 =  Mat::zeros(p_oNNX.rows,p_oNNX.cols,CV_32F);

	pow(p_oNNX,2,t11);
	pow(p_oNNY,2,t22);
	multiply(p_oNNX,p_oNNY,t12);

	int nKsize = (int) floor(6.0*m_dSigma);
	if (nKsize < 3)
	{
		nKsize = 3;
	}
	if(  nKsize%2 == 0) // kernel size should be odd and positive
		nKsize += 1;
	Mat kernelGaussCoef = getGaussianKernel(nKsize, m_dSigma, CV_32F);
	Mat kernelgauss = kernelGaussCoef * kernelGaussCoef.t();

	// Gaussian
//  	filter2D(t11,t11,CV_32F,kernelgauss);
//  	filter2D(t22,t22,CV_32F,kernelgauss);
//  	filter2D(t12,t12,CV_32F,kernelgauss);

	GaussianBlur(t11,t11,cv::Size(nKsize,nKsize),m_dSigma);
	GaussianBlur(t22,t22,cv::Size(nKsize,nKsize),m_dSigma);
	GaussianBlur(t12,t12,cv::Size(nKsize,nKsize),m_dSigma);

	kernelGaussCoef.release();
	kernelgauss.release();
	//LogThis(1,1,Fmt(_T("****** Gaussian filter + init done in %lf ms"), GetPerfTime() - dss));

	//dss= GetPerfTime();

	// calcul des valeurs propres 
	Mat m1;	
	pow((t11-t22),2,m1);
	Mat m2;
	pow(t12,2,m2);
	Mat Imgpart = m1+4.0f*m2;
	m1.release();
	m2.release();
	sqrt(Imgpart,Imgpart);
	Mat EigD_1 = (t22+t11+Imgpart)/2.0;
	//Mat EigD_2 = (t22+t11-Imgpart)/2.0;
	Imgpart.release();

	double dEigD_1Max;
	minMaxLoc(EigD_1,nullptr,&dEigD_1Max);
	double dThresh = 0.01 * dEigD_1Max; // 1% du max 

	Mat wat(2,2,CV_32F);
	Mat mEig = Mat::zeros(2,2,CV_32F);
	Mat vEigen = Mat::zeros(2,2,CV_32F);;
	Mat dEigen;
	for(int j=0;j<t11.cols;j++)
	{
		for(int i=0;i<t11.rows;i++)
		{
			wat.at<float>(0,0) = t11.at<float>(i,j);
			wat.at<float>(0,1) = t12.at<float>(i,j);
			wat.at<float>(1,0) = t12.at<float>(i,j);
			wat.at<float>(1,1) = t22.at<float>(i,j);

// 			float f11 = wat.at<float>(0,0);
// 			float f22 = wat.at<float>(1,1);
// 			float f12 = wat.at<float>(0,1);

			eigen(wat,dEigen,vEigen);

			if(dEigen.at<float>(0) > dEigen.at<float>(1))
			{
				//on inverse les valeurs et vecteurs propres // due to OPEN CV use of eigen fct ...

				float ftmp = dEigen.at<float>(0);
				dEigen.at<float>(0) = dEigen.at<float>(1);
				dEigen.at<float>(1) = ftmp;

				Mat rr;
				vEigen.col(1).copyTo(rr);
				vEigen.col(0).copyTo(vEigen.col(1));
				rr.copyTo(vEigen.col(0));
				rr.release();
			}

// 			float vp0 = dEigen.at<float>(0);
// 			float vp1 = dEigen.at<float>(1);
// 			float ve00 = vEigen.at<float>(0,0);
// 			float ve01 = vEigen.at<float>(0,1);
// 			float ve10 = vEigen.at<float>(1,0);
// 			float ve11 = vEigen.at<float>(1,1);

			float fL1 = 1.0f;
			float feigd1 = EigD_1.at<float>(i,j);
			if(dThresh < EigD_1.at<float>(i,j))
			{
				fL1 = (float)(m_dAlpha + 1.0 - std::exp(-g_dPI / (std::pow( EigD_1.at<float>(i,j), 4)) ));  
			}
			dEigen.at<float>(0) = 1.0f; // L2(ii,jj);
			dEigen.at<float>(1) = fL1;	// L1(ii,jj);

			mEig.at<float>(0,0) = dEigen.at<float>(0);
			mEig.at<float>(1,1) = dEigen.at<float>(1);

			wat = vEigen * mEig * vEigen.t();

// 			float f11r = wat.at<float>(0,0);
// 			float f22r = wat.at<float>(1,1);
// 			float f12r = wat.at<float>(0,1);
// 			float f21r = wat.at<float>(1,0);

			t11.at<float>(i,j) = wat.at<float>(0,0);
			t22.at<float>(i,j) = wat.at<float>(1,1);
			t12.at<float>(i,j) = wat.at<float>(0,1);

		}
	}
	EigD_1.release();
	wat.release();
	dEigen.release();
	mEig.release();
	vEigen.release();
/*	int niDx = 0;
	//#pragma omp parallel private(niDx)
	{
		int nNumThreads = 4;
		#pragma omp parallel  for num_threads(nNumThreads) private(niDx) schedule (dynamic, t11.cols / 2*nNumThreads)	
		//#pragma omp parallel for 
		for(int j=0;j<t11.cols;j++)
		{
			for(niDx=0;niDx<t11.rows;niDx++)
			{
				Mat wat(2,2,CV_32F);
				Mat mEig = Mat::zeros(2,2,CV_32F);
				Mat vEigen = Mat::zeros(2,2,CV_32F);;
				Mat dEigen;

				wat.at<float>(0,0) = t11.at<float>(niDx,j);
				wat.at<float>(0,1) = t12.at<float>(niDx,j);
				wat.at<float>(1,0) = t12.at<float>(niDx,j);
				wat.at<float>(1,1) = t22.at<float>(niDx,j);

				// 			float f11 = wat.at<float>(0,0);
				// 			float f22 = wat.at<float>(1,1);
				// 			float f12 = wat.at<float>(0,1);

				eigen(wat,dEigen,vEigen);

				if(dEigen.at<float>(0) > dEigen.at<float>(1))
				{
					//on inverse les valeurs et vecteurs propres // due to OPEN CV use of eigen fct ...

					float ftmp = dEigen.at<float>(0);
					dEigen.at<float>(0) = dEigen.at<float>(1);
					dEigen.at<float>(1) = ftmp;

					Mat rr;
					vEigen.col(1).copyTo(rr);
					vEigen.col(0).copyTo(vEigen.col(1));
					rr.copyTo(vEigen.col(0));
					rr.release();
				}

				// 			float vp0 = dEigen.at<float>(0);
				// 			float vp1 = dEigen.at<float>(1);
				// 			float ve00 = vEigen.at<float>(0,0);
				// 			float ve01 = vEigen.at<float>(0,1);
				// 			float ve10 = vEigen.at<float>(1,0);
				// 			float ve11 = vEigen.at<float>(1,1);

				float fL1 = 1.0f;
				float feigd1 = EigD_1.at<float>(niDx,j);
				if(dThresh < EigD_1.at<float>(niDx,j))
				{
					fL1 = (float)(m_dAlpha + 1.0 - std::exp(-PI / (std::pow( EigD_1.at<float>(niDx,j), 4)) ));  
				}
				dEigen.at<float>(0) = 1.0f; // L2(ii,jj);
				dEigen.at<float>(1) = fL1;	// L1(ii,jj);

				mEig.at<float>(0,0) = dEigen.at<float>(0);
				mEig.at<float>(1,1) = dEigen.at<float>(1);

				wat = vEigen * mEig * vEigen.t();

				// 			float f11r = wat.at<float>(0,0);
				// 			float f22r = wat.at<float>(1,1);
				// 			float f12r = wat.at<float>(0,1);
				// 			float f21r = wat.at<float>(1,0);

				t11.at<float>(niDx,j) = wat.at<float>(0,0);
				t22.at<float>(niDx,j) = wat.at<float>(1,1);
				t12.at<float>(niDx,j) = wat.at<float>(0,1);

			}
		}
	}
	EigD_1.release();
//	wat.release();
// 	dEigen.release();
// 	mEig.release();
// 	vEigen.release();
	//LogThis(1,1,Fmt(_T("****** Eigen done in %lf ms"), GetPerfTime() - dss));
*/
/*	if(0)
	{
		FILE* pfile11 = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\D11cpp.csv"),"w");
		FILE* pfile22 = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\D22cpp.csv"),"w");
		FILE* pfile12 = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\D12cpp.csv"),"w");

		for(int i=0;i<t11.rows;i++)
		{
			if (pfile11) 
				fprintf( pfile11	, "%f; %f; %f; %f; %f; %f; %f; %f; %f; %f;\n", t11.at<float>(i,0),t11.at<float>(i,1),t11.at<float>(i,2),t11.at<float>(i,3),t11.at<float>(i,4),t11.at<float>(i,5),t11.at<float>(i,6),t11.at<float>(i,7), t11.at<float>(i,8),t11.at<float>(i,9));
			if (pfile22) 
				fprintf( pfile22	, "%f; %f; %f; %f; %f; %f; %f; %f; %f; %f;\n", t22.at<float>(i,0),t22.at<float>(i,1),t22.at<float>(i,2),t22.at<float>(i,3),t22.at<float>(i,4),t22.at<float>(i,5),t22.at<float>(i,6),t22.at<float>(i,7), t22.at<float>(i,8),t22.at<float>(i,9));
			if (pfile12) 
				fprintf( pfile12	, "%f; %f; %f; %f; %f; %f; %f; %f; %f; %f;\n", t12.at<float>(i,0),t12.at<float>(i,1),t12.at<float>(i,2),t12.at<float>(i,3),t12.at<float>(i,4),t12.at<float>(i,5),t12.at<float>(i,6),t12.at<float>(i,7), t12.at<float>(i,8),t12.at<float>(i,9));

		}
		fclose(pfile11);
		fclose(pfile12);
		fclose(pfile22);

		FILE* pfileD11matlab = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\D11Matlab.csv"),"r");
		nLigneIdx = 0;
		while ( EOF != fscanf( pfileD11matlab, "%s\n", Buffer ))
		{
			token = strtok_s( Buffer, seps, &next_token);
			_CRT_FLOAT crtflt;
			nColIdx = 0;
			while ((token != NULL))
			{
				int nretval = _atoflt(&crtflt, token);
				if(nretval == 0)
					t11.at<float>(nLigneIdx,nColIdx)  = crtflt.f;
				else
					ASSERT(0);
				token = strtok_s( NULL, seps, &next_token);
				nColIdx++;
			}
			nLigneIdx++;
		}
		fclose(pfileD11matlab);

		FILE* pfileD22matlab = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\D22Matlab.csv"),"r");
		nLigneIdx = 0;
		while ( EOF != fscanf( pfileD22matlab, "%s\n", Buffer ))
		{
			token = strtok_s( Buffer, seps, &next_token);
			_CRT_FLOAT crtflt;
			nColIdx = 0;
			while ((token != NULL))
			{
				int nretval = _atoflt(&crtflt, token);
				if(nretval == 0)
					t22.at<float>(nLigneIdx,nColIdx)  = crtflt.f;
				else
					ASSERT(0);
				token = strtok_s( NULL, seps, &next_token);
				nColIdx++;
			}
			nLigneIdx++;
		}
		fclose(pfileD22matlab);

		FILE* pfileD12matlab = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\D12Matlab.csv"),"r");
		nLigneIdx = 0;
		while ( EOF != fscanf( pfileD12matlab, "%s\n", Buffer ))
		{
			token = strtok_s( Buffer, seps, &next_token);
			_CRT_FLOAT crtflt;
			nColIdx = 0;
			while ((token != NULL))
			{
				int nretval = _atoflt(&crtflt, token);
				if(nretval == 0)
					t12.at<float>(nLigneIdx,nColIdx)  = crtflt.f;
				else
					ASSERT(0);
				token = strtok_s( NULL, seps, &next_token);
				nColIdx++;
			}
			nLigneIdx++;
		}
		fclose(pfileD12matlab);
	}*/

// 	// For debug purpose only
// 		p_oNNX =  Mat::ones(16,16,CV_32F);
// 		p_oNNY =  Mat::ones(16,16,CV_32F);
// 		t11 = Mat::zeros(16,16,CV_32F);
// 		t22 = Mat::zeros(16,16,CV_32F);
// 		t12 = Mat::zeros(16,16,CV_32F);
// 		float fdbgidx = 1.f;
// 	
// 		for(int j=0;j<t11.cols;j++)
// 		{
// 			for(int i=0;i<t11.rows;i++)	
// 			{
// 				t11.at<float>(i,j) = fdbgidx;
// 				fdbgidx++;
// 			}
// 		}
// 		t22 = 10.0f * t11;
// 		t12 = 10.0f * t22; 

	//
	// Creation de la matrice sparse  -- Laplacian_matrix_tensor
	//
	//dss= GetPerfTime();

	int nH		= t11.rows;				// should be 128
	int nSize2	= t11.cols * t11.rows;	// should be (128)² 
	ASSERT(m_lVignetteSize == nH);
	ASSERT(m_lVignetteSize*m_lVignetteSize == nSize2);

	// Use Eigen Templates in the following code 

	// NOTICE should perform transpose of tii matrix since opencv store it in colmajor and need to use it in rowmajor storage (due to matlab algorithm)
	Mat D11 = t11.t();
	Mat D12 = t12.t();
	Mat D22 = t22.t();

	//
	// Create Sparse Matrix (ie laplacian_matrix_tensor_diag) - en dense size => 128²x128²
	//  
	SparseMatrix<float> A(nSize2,nSize2);
	VectorXf f(nSize2);
	VectorXf z(nSize2);

	Mat vSumbyRow = Mat::zeros(1,	nSize2, CV_32F);

	for(int i = 0; i<nSize2; i++)
	{
		A.startVec(i);	// en colMajor Sparse matrix on doit remplir les elements non zeros par ligne(row) croissante impérativement (performance issue otherwise) 
		// les elt sont donc renseigner col par col d'où la génération de la diag principal dans un 2eme temps. on sommes les lignes dans la première passe.
		// seul les elts non nul sont insérés

		// Upper triangle 
		if (i > (nH-1) )		// Diag n° (nH)  ===>  D11+D12 
		{
			float fnH = D11.at<float>(i-nH) + D12.at<float>(i-nH);
			if(fnH)
			{
				A.insertBack((i-nH),i)		+= fnH;
				vSumbyRow.at<float>(i-nH)	+= fnH;
			}
		}

		if (i > (nH-2) )		// Diag n° (nH-1)  ===>  ( 0 | -D12 ) & 0 at each COLUMN multiple of nH in dense matrix
		{
			//diag starts filled with 1 zeros at rank 0 and then each multiple of nH in column replace with 0
			int nIdx = i-(nH-1);
			int nColMultiple = (i +1) % nH;	// this (i+1) in modulo is due to indexation correspondence with matlab
			float fnHm1 = 0.0f; 
			if( (nIdx >= 1) &&  (nColMultiple != 0 ))  
				fnHm1 = -D12.at<float>(nIdx-1);
			
			if(fnHm1)
			{
				A.insertBack(i-(nH-1),i)		+= fnHm1;
				vSumbyRow.at<float>(i-(nH-1))	+= fnHm1;
			}
		}

		if (i > 0)				// Diag n° +1	===>  (D22+D12) & 0 at each LINE multiple of nH  in dense matrix
		{
			int nIdx = i-1;
			int nLineMultiple = (i-1 +1) % nH;	// this (i-1+1) in modulo is due to indexation correspondence with matlab
			float fn1 = 0.0f; 
			if( nLineMultiple != 0 )
				fn1 = (D22.at<float>(nIdx) + D12.at<float>(nIdx));
			
			if(fn1)
			{
				A.insertBack(i-1,i)			+= fn1;
				vSumbyRow.at<float>(i-1)	+= fn1;
			}
		}

		// Main diagonal		==> wait until end to perform row sum
		A.insertBack(i,i)			+= 1; // cree un non zero

		// Lower triangle
		if (i < (nSize2)-1)				// Diag n° -1	===>  (D22+D12) & 0 at each COL multiple of nH  in dense matrix
		{
			int nColMultiple = (i +1) % nH;	// this (i +1) in modulo is due to indexation correspondence with matlab, here its COL multiple since lower is transpose of upper triangle
			float mfn1 = 0.0f; 
			if( nColMultiple != 0)
				mfn1 = (D22.at<float>(i) + D12.at<float>(i));

			if(mfn1)
			{
				A.insertBack(i+1,i)			+= mfn1;
				vSumbyRow.at<float>(i+1)	+= mfn1;
			}
		}

		if (i < (nSize2)-(nH-1) )		// Diag n° -(nH-1)  ===>  ( 0 | -D12 ) & 0 at each LINE multiple of nH in dense matrix
		{
			//diag starts filled with 1 zeros at rank 0 and then each multiple of nH in column replace with 0
			int nIdx = i;
			int nLineMultiple = (i+(nH-1) +1) % nH;	// this (i+(nH-1) +1) in modulo is due to indexation correspondence with matlab , here its LINE multiple since lower is transpose of upper triangle
			float mfnHm1 = 0.0f;
			if( (nIdx >= 1) &&  (nLineMultiple != 0 ))  
				mfnHm1 = -D12.at<float>(nIdx-1);

			if(mfnHm1)
			{
				A.insertBack(i+(nH-1),i)		+= mfnHm1;
				vSumbyRow.at<float>(i+(nH-1))	+= mfnHm1;
			}
		}

		if (i < (nSize2)-nH )	// Diag n° -(nH)  ===>  D11+D12 
		{
			float mfnH = D11.at<float>(i) + D12.at<float>(i);
			if( mfnH)
			{
				A.insertBack((i+nH),i)		+= mfnH;
				vSumbyRow.at<float>(i+nH)	+= mfnH;
			}
		}
	}
	A.finalize();

	for(int i = 0; i<nSize2; i++)
	{
		A.coeffRef(i,i) = -vSumbyRow.at<float>(i);
	}
	A.finalize();
	vSumbyRow.release();

	D11.release();
	D22.release();
	D12.release();

//	LogThis(1,1,Fmt(_T("****** Sparse Matrix created in %lf ms"), GetPerfTime() - dss));

	//
	// Compute_f_tensor
	//

//	dss = GetPerfTime();

	Mat Zerocol = Mat::zeros(p_oNNX.rows,1,CV_32F);
	Mat Zerorow = Mat::zeros(1,p_oNNY.cols,CV_32F);
	Zerocol.copyTo(p_oNNX.col(p_oNNY.cols-1));	// last column to zero
	Zerorow.copyTo(p_oNNY.row(p_oNNY.rows-1)); // last row to zero

	Mat gx11;
	Mat gy22;
	Mat gx12; Mat gy12;
	Mat gx21; Mat gy21;

	multiply(p_oNNX,t11,gx11);
	multiply(p_oNNY,t22,gy22);

	multiply(p_oNNX,t12,gx12); // gx.*d12 (où d12 = d21)
	multiply(p_oNNY,t12,gy12); // gy.*d21 (où d12 = d21)
	gx21 = gy12.clone();	   // gy.*d12 (où d12 = d21)
	gy21 = gx12.clone();	   // gx.*d21 (où d12 = d21)
	gx12.row(gx12.rows-1).copyTo(gx21.row(gx21.rows-1)); //copy gx12 last row to gx21 last row
	gy12.col(gy12.cols-1).copyTo(gy21.col(gy21.cols-1)); //copy gx12 last col to gx21 last col
	Zerocol.copyTo(gx21.col(gx21.cols-1));	// last column to zero
	Zerorow.copyTo(gy21.row(gy21.rows-1));	// last row to zero

	gx12.release();
	gy12.release();

	// Need to clone since we don't want to overwrite gxii and gyii matrices
	Mat gxx1 =  gx11.clone();
	Mat gyy1 =  gy22.clone();
	Mat gxx2 =  gx21.clone();
	Mat gyy2 =  gy21.clone();

	ASSERT((t11.cols * t11.rows) == nSize2);
	//FILE* pfile = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\Fcpp.csv"),"w");
	//float* pdata = &(gyy1.at<float>(0,0));
	for(int j=0;j<t11.cols;j++)
	{
		for(int i=0;i<t11.rows;i++)	
		{
			// f1 = gxx1 + gyy1 
			//  where
			//gyy1(j,k) = gy22(j,k) - gy22(j-1,k);
			//gxx1(j,k) = gx11(j,k) - gx11(j,k-1);
			// 
			// f2 = gxx2 + gyy2
			//  where
			//gyy(j,k) = gy21(j,k) - gy21(j-1,k);
			//gxx(j,k) = gx21(j,k) - gx21(j,k-1);
			//
			// finally f = f1 + f2 

			if((i-1)>=0)
			{
				gyy1.at<float>(i,j) -=  gy22.at<float>(i-1,j);
				gyy2.at<float>(i,j) -=  gy21.at<float>(i-1,j);
			}
			if((j-1)>=0)
			{
				gxx1.at<float>(i,j) -= gx11.at<float>(i,j-1);
				gxx2.at<float>(i,j) -= gx21.at<float>(i,j-1);
			}

			f(i+j*t11.cols) = gyy1.at<float>(i,j) +  gyy2.at<float>(i,j) + gxx1.at<float>(i,j) + gxx2.at<float>(i,j);

// 			if (pfile)
// 			{
// 				fprintf( pfile	, "%f;\n", f(i+j*t11.cols) );
// 			}
		}
	}

//	fclose(pfile);

	gx11.release(); gy22.release(); 
	gx21.release(); gy21.release(); 
	gxx1.release(); gxx2.release(); 
	gyy1.release(); gyy2.release(); 

// 	pfile = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\Fcpp2.csv"),"w");
// 	if (pfile)
// 	{
// 		for(int i=0;i<(t11.cols*t11.rows);i++)
// 		{
// 			fprintf( pfile	, "%f;\n", f(i) );
// 		}
// 		fclose(pfile);
// 	}
	
//	LogThis(1,1,Fmt(_T("****** Vector created in %lf ms"), GetPerfTime() - dss));	

	SimplicialCholesky< SparseMatrix<float> > oSolver;
//	dss = GetPerfTime();
	oSolver.compute(A);
	//LogThis(1,1,Fmt(_T("****** Sparse Matrix COMPUTE in %lf ms"), GetPerfTime() - dss));
	//dss = GetPerfTime();
	z = oSolver.solve(f);
	//LogThis(1,1,Fmt(_T("****** Sparse Matrix SOLVED in %lf ms ===> [%s] "), GetPerfTime() - dss, (((oSolver.info()==Success) ? _T("Success") : _T(" !!!!!! FAIL !!!!!!!!"))) ));

	//dss = GetPerfTime();
	Mat oZRes = Mat::ones(p_oNNX.rows,p_oNNX.cols,CV_32F);
	if(oSolver.info()==Success)
	{
		//LogThis(1,1,_T("* vignette OK"));
		// copy and reshape
		for(int i=0;i<(p_oNNX.rows*p_oNNX.cols);i++)
		{
			oZRes.at<float>(i) = z(i);
		}
		oZRes = oZRes.t();

// 		FILE* pfileZ = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\Zcpp.csv"),"w");
// 		if (pfileZ)
// 		{
// 			for(int i=0;i<oZRes.rows;i++)
// 			{
// 				for(int j=0;j<oZRes.cols;j++)
// 				{
// 					fprintf( pfileZ	, "%f;", oZRes.at<float>(i,j));
// 				}
// 				fprintf( pfileZ	, "\n");
// 			}
// 			fclose(pfileZ);
// 		}

	}
 	else
		LogThis(1,4,_T("****** FAIL SOLVING vignette"));
//	LogThis(1,1,Fmt(_T("****** Convert Z to ZMat in %lf ms "), GetPerfTime() - dss));


/*	MatrixXf Ad = A.toDense();
	pfile = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\vD0.csv"),"w");
	if(pfile)
	{
		VectorXf d0 = Ad.diagonal();
		for( int i = 0; i< d0.rows(); i++)
			fprintf( pfile	, "%f\n", d0(i));
		fclose(pfile);
	}

	pfile = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\vD1m1.csv"),"w");
	if(pfile)
	{
		VectorXf d1 = Ad.diagonal(1);
		VectorXf d1m = Ad.diagonal(-1);
		ASSERT(d1.rows() == d1m.rows());
		for( int i = 0; i< d1.rows(); i++)
			fprintf( pfile, "%f; %f;\n", d1(i), d1m(i));
		fclose(pfile);
	}

	pfile = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\vDH1mH1.csv"),"w");
	if(pfile)
	{
		VectorXf dH1 = Ad.diagonal(nH-1);
		VectorXf dH1m = Ad.diagonal(-(nH-1));
		ASSERT(dH1.rows() == dH1m.rows());
		for( int i = 0; i< dH1.rows(); i++)
			fprintf( pfile	, "%f;%f;\n", dH1(i), dH1m(i));
		fclose(pfile);
	}

	pfile = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\vDHmH.csv"),"w");
	if(pfile)
	{
		VectorXf dH = Ad.diagonal(nH);
		VectorXf dHm = Ad.diagonal(-nH);
		ASSERT(dH.rows() == dHm.rows());
		for( int i = 0; i< dH.rows(); i++)
			fprintf( pfile	, "%f;%f;\n", dH(i), dHm(i));
		fclose(pfile);
	}*/

// 	FILE* pfileZ = fopen(_T("C:\\Altasight\\Nano\\Data\\Dbg\\ADense_cpp.csv"),"w");
// 	if (pfileZ)
// 	{
// 		for(int i=0;i<Ad.rows();i++)
// 		{
// 			for(int j=0;j<Ad.cols();j++)
// 			{
// 				fprintf( pfileZ	, "%f;", Ad(i,j));
// 			}
// 			fprintf( pfileZ	, "\n");
// 		}
// 		fclose(pfileZ);
// 	}

	return oZRes;
}

void CTreatReconstruct0::DoQuadrant(
	int		p_nQID,
	int		p_nStepX,
	int		p_nStepY,
	int		p_nFirstVignPosInRoi_X,
	int		p_nFirstVignPosInRoi_Y,
	HANDLE& p_hEvt)
{	
	// skip quadrant thread - do nothing
	if((m_uDbgFlag|m_uRegFlag) & DBG_SKIP_QUADTHREAD)
	{
		SetEvent(p_hEvt);
		return;
	}

	int nMarVignSizeX = m_lVignetteSize+2*m_nMargin;
	int nMarVignSizeY = m_lVignetteSize+2*m_nMargin;

	// Apply ROI to Quadrant ID (Note RT :  voir si Copy necessaire si risque de concurence d'accés normalement OK car ne fait que lire)

	int nQVoisX = 0;
	int nQVoisY = 0; 
	Rect rcQuadrantArea;
	switch(p_nQID)
	{
	case 1: // Q1 top left quadrant
		rcQuadrantArea = Rect(0,						0,						m_lOriginalWidth_px / 2 + 2*m_nMargin, m_lOriginalHeight_px/2 + 2*m_nMargin);
		// voisinage bas droit des vignettes
		nQVoisX = 3 * m_lVignetteSize / 4;	// ie r2+r4 ie r-r4
		nQVoisY = 3 * m_lVignetteSize / 4;  // ie r2+r4 ie r-r4
		break;
	case 2: // Q2 top right quadrant
		rcQuadrantArea = Rect(m_lOriginalWidth_px / 2,	0,						m_lOriginalWidth_px / 2 + 2*m_nMargin, m_lOriginalHeight_px/2 + 2*m_nMargin);
		// voisinage bas gauche des vignettes
		nQVoisX = m_lVignetteSize / 4;		// ie r2-r4 ie r4
		nQVoisY = 3 * m_lVignetteSize / 4;	// ie r2+r4 ie r-r4
		break;
	case 3: // Q3 Bottom left quadrant
		rcQuadrantArea = Rect(0,						m_lOriginalHeight_px/2,	m_lOriginalWidth_px / 2 + 2*m_nMargin, m_lOriginalHeight_px/2 + 2*m_nMargin);
		// voisinage haut droit des vignettes
		nQVoisX = 3 * m_lVignetteSize / 4;	// ie r2+r4 ie r-r4
		nQVoisY = m_lVignetteSize / 4;		// ie r2-r4 ie r4
		break;
	case 4: // Q4 Bottom right quadrant
		rcQuadrantArea = Rect(m_lOriginalWidth_px / 2,	m_lOriginalHeight_px/2,	m_lOriginalWidth_px / 2 + 2*m_nMargin, m_lOriginalHeight_px/2 + 2*m_nMargin);
		// voisinage haut gauche des vignettes
		nQVoisX = m_lVignetteSize / 4;		// ie r4
		nQVoisY = m_lVignetteSize / 4;		// ie r4
		break;
	default:
		LogThis(1,4,Fmt(_T("Unknow Quadrant ID %d"),p_nQID));
		return;
	}

	// At this point be sure that Original matrice have been enlarge of 2*p_nAbsMargin
	Mat oQiNx		= m_oCVMatNX(rcQuadrantArea);
	Mat oQiNy		= m_oCVMatNY(rcQuadrantArea);
	Mat oQiMaskD	= m_oCVMatMaskD(rcQuadrantArea);
	
	int nMAX_J = 1;
	if(p_nStepY != 0)
		nMAX_J = (int) floor((float)(oQiNx.rows-2*m_nMargin)/(float)abs(p_nStepY));
	int nMAX_I = 1;
	if(p_nStepX != 0)
		nMAX_I = (int) floor((float)(oQiNx.cols-2*m_nMargin)/(float)abs(p_nStepX));

	for(int j = 0; j < nMAX_J; j++)
	{
		int nIdxY = p_nFirstVignPosInRoi_Y + j*(p_nStepY) - m_nMargin ;
		if ( nIdxY < 0 || nIdxY > (oQiNx.rows-nMarVignSizeY) )
			continue;

		for(int i = 0; i < nMAX_I; i++)
		{	
			int nIdxX = p_nFirstVignPosInRoi_X + i*(p_nStepX) - m_nMargin;
			if ( nIdxX < 0 || nIdxX > (oQiNx.cols-nMarVignSizeX) )
				continue;
	
			// Apply ROI to mask
			Mat oMM = Mat(oQiMaskD, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();
			// Check if this sample contain some wafer data
			Scalar oSumMsk = sum(oMM);
			if(oSumMsk[0] == 0)
			{
				//unused data sample go to the next one
				continue;
			}

			// Apply ROI to NX
			Mat oNNX = Mat(oQiNx, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();
			// Apply ROI to NY
			Mat oNNY = Mat(oQiNy, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();

			ComputeVignette(nIdxX,nIdxY,oNNX,oNNY,oMM,m_oCVR[p_nQID-1],m_oCVIterationMap[p_nQID-1],nQVoisX,nQVoisY);

		}
	}
	SetEvent(p_hEvt);
}

UINT CTreatReconstruct0::DoQ1( void *p_pParameters )
{
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2 ;
	int nPosX = pObj->m_lOriginalWidth_px/2		+ pObj->m_nMargin - pObj->m_lVignetteSize;
	int nPosY =	pObj->m_lOriginalHeight_px/2	+ pObj->m_nMargin - pObj->m_lVignetteSize;
	pObj->DoQuadrant(1,-nDef,-nDef,nPosX,nPosY,pObj->m_hEventQDone[0]);
	return 241;
}

UINT CTreatReconstruct0::DoQ2( void *p_pParameters )
{
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2 ;
	int nPosX = pObj->m_nMargin;
	int nPosY =	pObj->m_lOriginalHeight_px/2	+ pObj->m_nMargin - pObj->m_lVignetteSize;
	pObj->DoQuadrant(2,nDef,-nDef,nPosX,nPosY,pObj->m_hEventQDone[1]);
	return 242;
}

UINT CTreatReconstruct0::DoQ3( void *p_pParameters )
{
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2 ;
	int nPosX = pObj->m_lOriginalWidth_px/2	+ pObj->m_nMargin - pObj->m_lVignetteSize;
	int nPosY = pObj->m_nMargin;
	pObj->DoQuadrant(3,-nDef,nDef,nPosX,nPosY,pObj->m_hEventQDone[2]);
	return 243;
}

UINT CTreatReconstruct0::DoQ4( void *p_pParameters )
{
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2 ;
	int nPosX = pObj->m_nMargin;
	int nPosY = pObj->m_nMargin;
	pObj->DoQuadrant(4,nDef,nDef,nPosX,nPosY,pObj->m_hEventQDone[3]);
	return 244;
}

void CTreatReconstruct0::DoBand(
	int		p_nBandID,
	int		p_nStepX,
	int		p_nStepY,
	int		p_nFirstVignPosInRoi_X,
	int		p_nFirstVignPosInRoi_Y,
	HANDLE& p_hEvt)
{
	// skip band thread - do nothing
	if((m_uDbgFlag|m_uRegFlag) & DBG_SKIP_BANDTHREAD)
	{
		SetEvent(p_hEvt);
		return;
	}

	int nMarVignSizeX = m_lVignetteSize+2*m_nMargin;
	int nMarVignSizeY = m_lVignetteSize+2*m_nMargin;

	int nr2		= m_lVignetteSize/2;

	// Apply ROI to Band ID 
	int nBVoisX = 0;
	int nBVoisY = 0; 
	Rect rcBandArea;
	switch(p_nBandID)
	{
	case 1: // B12 top band
		rcBandArea = Rect(m_lOriginalWidth_px/2 - nr2,	0,								nMarVignSizeX,								m_lOriginalHeight_px/2 + 2*m_nMargin);
		// voisinage bas milieu des vignettes
		nBVoisX = (m_lVignetteSize - m_nVois) / 2; 
		nBVoisY = 3 * m_lVignetteSize / 4 ; 
		break;
	case 2: // B34 bottom band
		rcBandArea = Rect(m_lOriginalWidth_px/2 - nr2,	m_lOriginalHeight_px/2,			nMarVignSizeX,								m_lOriginalHeight_px/2 + 2*m_nMargin);
		// voisinage haut milieu des vignettes
		nBVoisX = (m_lVignetteSize - m_nVois) / 2; 
		nBVoisY = m_lVignetteSize / 4 ; 
		break;
	case 3: // B13 left band
		rcBandArea = Rect(0,							m_lOriginalHeight_px/2  - nr2,	m_lOriginalWidth_px / 2 + 2*m_nMargin,	nMarVignSizeY);
		// voisinage droite milieu des vignettes
		nBVoisX = 3 * m_lVignetteSize / 4; 
		nBVoisY = (m_lVignetteSize - m_nVois) / 2;
		break;
	case 4: // B24 right band
		rcBandArea = Rect(m_lOriginalWidth_px/2 ,		m_lOriginalHeight_px/2 - nr2 ,	m_lOriginalWidth_px / 2 + 2*m_nMargin,	nMarVignSizeY);
		// voisinage gauche milieu des vignettes
		nBVoisX =  m_lVignetteSize / 4; 
		nBVoisY = (m_lVignetteSize - m_nVois) / 2;
		break;
	default:
		LogThis(1,4,Fmt(_T("Unknow Band ID %d"),p_nBandID));
		return;
	}

	// At this point be sure that Original matrice have been enlarge of 2*p_nAbsMargin
	Mat oBiNx		= m_oCVMatNX(rcBandArea);
	Mat oBiNy		= m_oCVMatNY(rcBandArea);
	Mat oBiMaskD	= m_oCVMatMaskD(rcBandArea);

	int nMAX_J = 1;
	if(p_nStepY != 0)
		nMAX_J = (int) floor((float)(oBiNx.rows-2*m_nMargin)/(float)abs(p_nStepY));
	int nMAX_I = 1;
	if(p_nStepX != 0)
		nMAX_I = (int) floor((float)(oBiNx.cols-2*m_nMargin)/(float)abs(p_nStepX));

	for(int j = 0; j < nMAX_J; j++)
	{
		int nIdxY = p_nFirstVignPosInRoi_Y + j*(p_nStepY) - m_nMargin ;
		if ( nIdxY < 0 || nIdxY > (oBiNx.rows-nMarVignSizeY) )
			continue;

		for(int i = 0; i < nMAX_I; i++)
		{	
			int nIdxX = p_nFirstVignPosInRoi_X + i*(p_nStepX) - m_nMargin;
			if ( nIdxX < 0 || nIdxX > (oBiNx.cols-nMarVignSizeX) )
				continue;

			// Apply ROI to mask
			Mat oMM = Mat(oBiMaskD, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();
			// Check if this sample contain some wafer data
			Scalar oSumMsk = sum(oMM);
			if(oSumMsk[0] == 0)
			{
				//unused data sample go to the next one
				continue;
			}

			// Apply ROI to NX
			Mat oNNX = Mat(oBiNx, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();
			// Apply ROI to NY
			Mat oNNY = Mat(oBiNy, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();

			ComputeVignette(nIdxX,nIdxY,oNNX,oNNY,oMM,m_oCVBandR[p_nBandID-1],m_oCVBandIterationMap[p_nBandID-1], nBVoisX, nBVoisY);
		}
	}
	SetEvent(p_hEvt);
}

UINT CTreatReconstruct0::DoBand12( void *p_pParameters )
{
	// Top Band
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2 ;
	int nPosX = pObj->m_nMargin;
	int nPosY =	pObj->m_lOriginalHeight_px/2	+ pObj->m_nMargin - pObj->m_lVignetteSize;
	pObj->DoBand(1,0,-nDef,nPosX,nPosY,pObj->m_hEventBandDone[0]);
	return 12;
}

UINT CTreatReconstruct0::DoBand34( void *p_pParameters )
{
	// Bottom Band
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2;
	int nPosX = pObj->m_nMargin;
	int nPosY =	pObj->m_nMargin;
	pObj->DoBand(2,0,nDef,nPosX,nPosY,pObj->m_hEventBandDone[1]);
	return 34;
}

UINT CTreatReconstruct0::DoBand13( void *p_pParameters )
{
	// Left Band
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2 ;
	int nPosX = pObj->m_lOriginalWidth_px/2	+ pObj->m_nMargin - pObj->m_lVignetteSize;
	int nPosY = pObj->m_nMargin ;
	pObj->DoBand(3,-nDef,0,nPosX,nPosY,pObj->m_hEventBandDone[2]);
	return 13;
}

UINT CTreatReconstruct0::DoBand24( void *p_pParameters )
{
	// Left Band
	CTreatReconstruct0* pObj = static_cast<CTreatReconstruct0 *> (p_pParameters);
	int nDef  = pObj->m_lVignetteSize/2 ;
	int nPosX = pObj->m_nMargin;
	int nPosY =	pObj->m_nMargin;
	pObj->DoBand(4,nDef,0,nPosX,nPosY,pObj->m_hEventBandDone[3]);
	return 24;
}

void CTreatReconstruct0::DoMiddle()
{
	int nMarVignSizeX = m_lVignetteSize+2*m_nMargin;
	int nMarVignSizeY = m_lVignetteSize+2*m_nMargin;
	int nr2 = m_lVignetteSize/2;

	Rect rcMiddleArea = Rect(m_lOriginalWidth_px/2 - nr2,	m_lOriginalHeight_px/2  - nr2,		nMarVignSizeX,		nMarVignSizeY);

	Mat oiNx		= m_oCVMatNX(rcMiddleArea);
	Mat oiNy		= m_oCVMatNY(rcMiddleArea);
	Mat oiMaskD		= m_oCVMatMaskD(rcMiddleArea);

	int nIdxY = 0;
	int nIdxX = 0;

	// Apply ROI to mask
	Mat oMM = Mat(oiMaskD, Rect(nIdxX , nIdxY, nMarVignSizeX, nMarVignSizeY)).clone();
	// Check if this sample contain some wafer data
	Scalar oSumMsk = sum(oMM);
	if(oSumMsk[0] == 0)
	{
		//unused data sample go to the next one
		return;
	}

	// Apply ROI to NX
	Mat oNNX = Mat(oiNx, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();
	// Apply ROI to NY
	Mat oNNY = Mat(oiNy, Rect(nIdxX , nIdxY, nMarVignSizeX,nMarVignSizeY )).clone();

	int nVoisMid =-1; //m_lVignetteSize/2 - m_nVois/2;
	ComputeVignette(nIdxX,nIdxY,oNNX,oNNY,oMM,m_oCVMidR,m_oCVMidIterationMap,nVoisMid,nVoisMid);
}


bool CTreatReconstruct0::SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
{
	float* pData = p_oMatrixFloat->GetData();

	unsigned long  lCols = p_oMatrixFloat->GetCo();
	unsigned long  lLines = p_oMatrixFloat->GetLi();

	float fMin = FLT_MAX;
	float fMax = - FLT_MAX;

	bool bUseMinPrm = (p_fMin != FLT_MAX);
	bool bUseMaxPrm = (p_fMax != FLT_MAX);

	float a = 1.0f;
	float b = 0.0f;
	if(bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
	{
		for(long lItem = 0; lItem<p_oMatrixFloat->GetSize(); lItem++)
		{
			if(!bUseMinPrm)
				fMin = __min( fMin, pData[lItem]);
			if(!bUseMaxPrm)
				fMax = __max( fMax, pData[lItem]);
		}
	}
	else
	{
		if(!bUseMaxPrm)
			fMax = 255.0f;
		if(!bUseMinPrm)
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

	a = 255.0f / (fMax- fMin);
	b = - fMin * 255.0f / (fMax - fMin);

	fipImage oImg(FIT_BITMAP,lCols, lLines,8);
	for(unsigned y = 0; y < oImg.getHeight(); y++)
	{
		//ici pb à resoudre pour affichage image
		BYTE* pbits = (BYTE*) oImg.getScanLine(y);
		for(unsigned x = 0; x < oImg.getWidth(); x++)
		{
			pbits[x] =saturate_cast<uchar>(pData[y*lCols+x] * a + b) ;
		}
	}
	oImg.flipVertical();
	BOOL bRes = oImg.save(p_csFilepath, 0);
	return (bRes !=0) ;
}

bool CTreatReconstruct0::SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin /*= INT_MAX*/, int p_nMax /*= INT_MAX*/, bool bAutoscale /*= true*/)
{
	unsigned char* pData = p_oMatrix->GetData();

	unsigned long  lCols	= p_oMatrix->GetCo();
	unsigned long  lLines	= p_oMatrix->GetLi();

	float fMin = (float)	INT_MAX;
	float fMax = (float) -	INT_MAX;

	bool bUseMinPrm = (p_nMin != INT_MAX);
	bool bUseMaxPrm = (p_nMax != INT_MAX);

	float a = 1.0f;
	float b = 0.0f;
	if(bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
	{
		for(long lItem = 0; lItem<p_oMatrix->GetSize(); lItem++)
		{
			if(!bUseMinPrm)
				fMin = __min( fMin, pData[lItem]);
			if(!bUseMaxPrm)
				fMax = __max( fMax, pData[lItem]);
		}
	}
	else
	{
		if(!bUseMaxPrm)
			fMax = 255.0f;
		if(!bUseMinPrm)
			fMin = 0.0f;
	}

	if (bUseMinPrm)
	{
		fMin = (float) p_nMin;
	}
	if (bUseMaxPrm)
	{
		fMax = (float)  p_nMax;
	}

	a = 255.0f / (fMax- fMin);
	b = - fMin * 255.0f / (fMax - fMin);

	fipImage oImg(FIT_BITMAP,lCols, lLines,8);
	for(unsigned y = 0; y < oImg.getHeight(); y++)
	{
		for(unsigned x = 0; x < oImg.getWidth(); x++)
		{
			BYTE indx  = saturate_cast<uchar>(pData[y*lCols+x] * a + b) ; 
			oImg.setPixelIndex(x,y,&indx);
		}
	}
	oImg.flipVertical();
	BOOL bRes = oImg.save(p_csFilepath, 0);
	return (bRes != 0 );
}

UINT CTreatReconstruct0::SaveData( void *p_pParameters )
{
	tData2Save* pData  = static_cast<tData2Save *>(p_pParameters);
	if(pData == nullptr)
		return 1;

	CString csFileName;

	CString sGenPath = pData->csPath;

	double dStart = GetPerfTime();
	UINT nId = pData->nId;
	CString csTreatName = pData->csName;
	LogThis(1,1,Fmt(_T("(%s) ##### Start saving data = No %d"),csTreatName,nId));
	list < tSpT<H3_MATRIX_UINT8> > spListU8  = pData->spListU8;
	list < tSpT<H3_MATRIX_FLT32> > spListF32 = pData->spListF32;
	delete pData; 
	pData = 0;

	// Assure Results Directory exist
	CreateDir(sGenPath);

	while(spListU8.size() != 0)
	{
		tSpT<H3_MATRIX_UINT8> elt = spListU8.front();
		if(elt._spT)
		{
			if (elt._bImg)
			{
				csFileName = Fmt(_T("%s\\%s_%s_%d.png"), sGenPath, csTreatName, elt._cs, nId);
				if(! SaveGreyImageUInt8(csFileName,elt._spT))
					LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
				else
					LogThis(1,1,Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
			}
			if (elt._bHbf)
			{
				csFileName = Fmt(_T("%s\\%s_%s_%d.hbf"), sGenPath, csTreatName, elt._cs, nId);
				bool bRes = false;
				FILE* pFile = 0;
				if(fopen_s(&pFile, (LPCSTR)csFileName,"wb+") == 0)
				{
					bRes = elt._spT->fSaveHBF(pFile);
					fclose(pFile);
				}
				if( ! bRes)
					LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
				else
					LogThis(1,1,Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
			}
			if (elt._bBin)
			{
				csFileName = Fmt(_T("%s\\%s_%s_%d.bin"), sGenPath, csTreatName, elt._cs, nId);
				bool bRes = false;
				FILE* pFile = 0;
				if(fopen_s(&pFile, (LPCSTR)csFileName,"wb+") == 0)
				{
					bRes = elt._spT->fSaveBIN(pFile);
					fclose(pFile);
				}
				if( ! bRes)
					LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
				else
					LogThis(1,1,Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
			}
			elt._spT.reset();
		}
		spListU8.pop_front();
	}

	while(spListF32.size() != 0)
	{
		tSpT<H3_MATRIX_FLT32> elt = spListF32.front();
		if(elt._spT)
		{
			if (elt._bImg)
			{
				csFileName = Fmt(_T("%s\\%s_%s_%d.png"), sGenPath, elt._cs, nId);
				if(! SaveGreyImageFlt32(csFileName,elt._spT))
					LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
				else
					LogThis(1,1,Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
			}
			if (elt._bHbf)
			{
				csFileName = Fmt(_T("%s\\%s_%s_%d.hbf"), sGenPath, csTreatName, elt._cs, nId);
				bool bRes = false;
				FILE* pFile = 0;
				if(fopen_s(&pFile, (LPCSTR)csFileName,"wb+") == 0)
				{
					bRes = elt._spT->fSaveHBF(pFile);
					fclose(pFile);
				}
				if( ! bRes)
					LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
				else
					LogThis(1,1,Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
			}
			if (elt._bBin)
			{
				csFileName = Fmt(_T("%s\\%s_%s_%d.bin"), sGenPath, csTreatName, elt._cs, nId);
				bool bRes = false;
				FILE* pFile = 0;
				if(fopen_s(&pFile, (LPCSTR)csFileName,"wb+") == 0)
				{
					bRes = elt._spT->fSaveBIN(pFile);
					fclose(pFile);
				}
				if( ! bRes)
					LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
				else
					LogThis(1,1,Fmt(_T("(%s) Save <%s>  "), csTreatName, csFileName));
			}
			elt._spT.reset();
		}
		spListF32.pop_front();
	}


	double dEnd = GetPerfTime();
	LogThis(1,2,Fmt(_T("(%s) ##### End saving data = No %d --- Exec in %lf"),csTreatName, nId, dEnd-dStart));

	return 0;
}

HRESULT CTreatReconstruct0::QueryInterface( REFIID iid, void **ppvObject )
{
	*ppvObject=0;   // Toujours initialiser le pointeur renvoyé.
	if (iid==IID_IUnknown)
		*reinterpret_cast<IUnknown **>(ppvObject)= static_cast<IUnknown *>(this);
	else 
		if (iid==IID_INanoTreatment)
			*reinterpret_cast<INanoTopoTreament **>(ppvObject)= static_cast<INanoTopoTreament *>(this);
	if (*ppvObject==0) 
		return E_NOINTERFACE;
	AddRef();           // On incrémente le compteur de références.
	return NOERROR;
}

ULONG CTreatReconstruct0::AddRef( void )
{
	m_ulRefCount++;
	return m_ulRefCount;
}

ULONG CTreatReconstruct0::Release( void )
{
	m_ulRefCount--;
	if (m_ulRefCount!=0) 
		return m_ulRefCount;
	delete this;     // Destruction de l'objet.
	return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}

extern "C"  HRESULT Create_TreatReconstruct0( REFIID iid, void **ppvObject )
{
	CTreatReconstruct0 *pObj = new CTreatReconstruct0();
	if (pObj==0) 
		return E_OUTOFMEMORY;
	return pObj->QueryInterface(iid, ppvObject);
}


