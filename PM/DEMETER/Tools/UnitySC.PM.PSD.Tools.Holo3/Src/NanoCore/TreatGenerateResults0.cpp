#include "StdAfx.h"
#include "TreatGenerateResults0.h"

//#include "H3MatrixOperations.h"

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include "opencv2/imgproc/imgproc.hpp"
using namespace cv;

#define ZLIB_WINAPI
#include "zlib.h"
#pragma comment (lib , "zlibwapi")

#include "FreeImagePlus.h"
#ifdef _DEBUG
	#pragma comment (lib , "FreeImaged")
	#pragma comment (lib , "FreeImagePlusd")
#else
	#pragma comment (lib , "FreeImage")
	#pragma comment (lib , "FreeImagePlus")
#endif

#pragma comment( lib, "ole32.lib" ) // for IStorage & IStream
// {5434E461-D6E3-44A1-92F7-AAC518BB8BB8}
static const FMTID g_fmtid = {
	0x5434e461,
	0xd6e3, 
	0x44a1,
	{ 0x92, 0xf7, 0xaa, 0xc5, 0x18, 0xbb, 0x8b, 0xb8 } 
};

#define	DBG_REC_FILTERLISS	0X00000010
#define DBG_REC_ERODE		0X00000020
#define DBG_REC_PV2			0x00000040
#define DBG_REC_PV10		0x00000080
#define DBG_SKIP_PV2		0x00000100
#define DBG_SKIP_PV10		0x00000200

#include <omp.h> // for openMP directive //=> check also that /openMP is active in compiler properties

struct tPVSiteData
{
	float pv;
	int x;
	int y;
};

CLASS_TYPE  void ReduceMatrixPadArray(H3_MATRIX &p_oMatrix, int p_nDecalX, int p_nDecalY)
{
	H3_MATRIX otmp;
	otmp.Copy(p_oMatrix);
	p_oMatrix.ReAlloc(otmp.GetLi() - 2 * p_nDecalY, otmp.GetCo() - 2 * p_nDecalX );
	p_oMatrix.Fill((TYPE)0.0f);
	TYPE* pPtrData = p_oMatrix.GetData();

	for(unsigned long nj = 0; nj <p_oMatrix.GetLi(); nj++)
	{
		unsigned long lidxline		= (nj+p_nDecalY)*otmp.GetCo() + p_nDecalX ;		// index line start - big extented matrix 
		unsigned long lidxnewline	= nj*p_oMatrix.GetCo();							// index line start - reduce matrix

		for(unsigned long ni = 0; ni <p_oMatrix.GetCo(); ni++)
		{
			pPtrData[lidxnewline + ni] = otmp[lidxline + ni];
		}
	}
}

CTreatGenerateResults0::CTreatGenerateResults0()
{	
	InitializeCriticalSection(&m_sCriticalSection);

	for(int i =0; i<2; i++)
	{
		//m_hEventPVDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		//ASSERT(m_hEventPVDone[i] != 0);	
	}

	m_nPrmErodeRadius	= 28;
	m_fPixelSize		= 0.01f; // exprimé en micron µ (taille d'un pixel en micron)
	m_fLimitCoef		= 0.0f; //0.75f; // recette ?
	m_bUseDiskPV		= false;
	m_nMaxPVDisplay		= 50;
	m_nCurvePts			= 6000;
	m_uRegFlag = 0;
	m_fTHA2 = m_fTHA10 = 0.0f;
	m_nOffsetExpandX = 0;
	m_nOffsetExpandY = 0;

	m_bUseSitePVMap = false;
	m_fSiteWidthmm = 0.0f;
	m_fSiteHeightmm= 0.0f;
	m_fSiteOffsetXmm= 0.0f;
	m_fSiteOffsetYmm= 0.0f;
	m_fSiteThresh1nm= 0.0f;
	m_fSiteThresh2nm= 0.0f;
}

CTreatGenerateResults0::~CTreatGenerateResults0()
{
	/*for(int i =0; i<2; i++)
	{
		if (m_hEventPVDone[i] != 0)
		{
			CloseHandle(m_hEventPVDone[i]);
			m_hEventPVDone[i] = 0;
		}
	}
	 
     DeleteCriticalSection(&m_sCriticalSection);*/
}

bool CTreatGenerateResults0::Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder)
{ 
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("ErodeRadius"), m_nPrmErodeRadius))
		LogThis(1,3,Fmt(_T("{%s} Could not find [ErodeRadius] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("PixelSize"), m_fPixelSize))
		LogThis(1,3,Fmt(_T("{%s} Could not find [PixelSize] Parameter"), INanoTopoTreament::GetName()));

	int  nBool = 0;
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("UseDiskPV"),nBool))
		LogThis(1,3,Fmt(_T("{%s} Could not find [UseDiskPV] Parameter"), INanoTopoTreament::GetName()));
	m_bUseDiskPV = (nBool != 0);

	if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("LimitCoef"), m_fLimitCoef))
		LogThis(1,3,Fmt(_T("{%s} Could not find [LimitCoef] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("MaxPVDisplay"), m_nMaxPVDisplay))
		LogThis(1,3,Fmt(_T("{%s} Could not find [MaxPVDisplay] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("NbCurvePts"), m_nCurvePts))
		LogThis(1,3,Fmt(_T("{%s} Could not find [NbCurvePts] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("LissageSize"), m_nLissageSize))
		LogThis(1,3,Fmt(_T("{%s} Could not find [LissageSize] Parameter"), INanoTopoTreament::GetName()));

	m_uRegFlag = 0;
	if( ! GetRegistryFlag(m_uRegFlag, DBG_REC_PV2 | DBG_REC_PV10))
		LogThis(1,3,Fmt(_T("{%s} Could not reach Registry flag Parameter"), INanoTopoTreament::GetName()));

	m_fEdgeExclusion_mm = 0.0f;
	if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("EdgeExclusion"), m_fEdgeExclusion_mm))
		LogThis(1,3,Fmt(_T("{%s} Could not find [EdgeExclusion] Parameter"), INanoTopoTreament::GetName()));

	nBool = 0;
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("PVHiValue_IncludeMasks"),nBool))
		LogThis(1,3,Fmt(_T("{%s} Could not find [PVHiValue_IncludeMasks] Parameter"), INanoTopoTreament::GetName()));
	m_bUseMaskPVData = (nBool != 0);
	if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("PVHiValue_Thresh10mm"), m_fPV10HiValue_Thresh_nm))
		LogThis(1,3,Fmt(_T("{%s} Could not find [PVHiValue_Thresh10mm] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("PVHiValue_Thresh2mm"), m_fPV2HiValue_Thresh_nm))
		LogThis(1,3,Fmt(_T("{%s} Could not find [PVHiValue_Thresh2mm] Parameter"), INanoTopoTreament::GetName()));

	nBool = 0;
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("UseDataCompression"),nBool))
		LogThis(1,3,Fmt(_T("{%s} Could not find [UseDataCompression] Parameter"), INanoTopoTreament::GetName()));
	m_bUseDataCompression = (nBool != 0);
	nBool = 0;
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("UseResumeLotStats"),nBool))
		LogThis(1,3,Fmt(_T("{%s} Could not find [UseResumeLotStats] Parameter"), INanoTopoTreament::GetName()));
	m_bUseResumeLotStats = (nBool != 0);

	nBool = 0;
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("UseSitePVMap"),nBool))
		LogThis(1,3,Fmt(_T("{%s} Could not find [UseSitePVMap] Parameter"), INanoTopoTreament::GetName()));
	m_bUseSitePVMap = (nBool != 0);

	if(m_bUseSitePVMap)
	{
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteWidthmm"), m_fSiteWidthmm))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteWidthmm] Parameter"), INanoTopoTreament::GetName()));
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteHeightmm"), m_fSiteHeightmm))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteHeightmm] Parameter"), INanoTopoTreament::GetName()));
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteOffsetXmm"), m_fSiteOffsetXmm))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteOffsetXmm] Parameter"), INanoTopoTreament::GetName()));
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteOffsetYmm"), m_fSiteOffsetYmm))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteOffsetYmm] Parameter"), INanoTopoTreament::GetName()));
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteThresh1nm"), m_fSiteThresh1nm))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteThresh1nm] Parameter"), INanoTopoTreament::GetName()));
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteThresh2nm"), m_fSiteThresh2nm))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteThresh2nm] Parameter"), INanoTopoTreament::GetName()));
		m_fSiteLimitsFactor = 0.5;
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteLimitFactor"), m_fSiteLimitsFactor))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteLimitFactor] Parameter"), INanoTopoTreament::GetName()));
		m_fSiteTxtFactor = 1.0;
		if( ! FindTreatInitPrmFlt(p_pPrmMap,_T("SiteTxtFactor"), m_fSiteTxtFactor))
			LogThis(1,3,Fmt(_T("{%s} Could not find [SiteTxtFactor] Parameter"), INanoTopoTreament::GetName()));

	}

	if( (m_uDbgFlag|m_uRegFlag) & DBG_SHOW_DISPLAY)
	{
		cvNamedWindow( "PV10", CV_WINDOW_NORMAL| CV_WINDOW_KEEPRATIO);// Create a window for display.
		cvNamedWindow( "PV2", CV_WINDOW_NORMAL| CV_WINDOW_KEEPRATIO);// Create a window for display.
	}

	return true;
}

/*bool CTreatGenerateResults0::Exec( const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm )
{
	double dstart = GetPerfTime();

	m_MaskE.reset(new  H3_MATRIX_UINT8());
	m_Hf.reset(new  H3_MATRIX_FLT32());

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

	if(FindTreatPrmPtr(p_InputsPrm,"FoupID",p))
	{
		m_csFoupID = *((CString*) p); 	
	}

	if(FindTreatPrmPtr(p_InputsPrm,"OffsetExpand_X",p))
	{
		m_nOffsetExpandX = *((int*) p); 	
	}
	if(FindTreatPrmPtr(p_InputsPrm,"OffsetExpand_Y",p))
	{
		m_nOffsetExpandY = *((int*) p); 	
	}
	if(FindTreatPrmPtr(p_InputsPrm,"FilterType",p))
	{
		m_nFilterType = *((int*) p); 	
	}

	shared_ptr<void> pvMaskE;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"MaskE",pvMaskE))
	{
		m_MaskE = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskE);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("MaskE Size = %d x %d"), m_MaskE->GetCo(),m_MaskE->GetLi()));
	}
	pvMaskE.reset();
	ReduceMatrixPadArray(*m_MaskE.get(),m_nOffsetExpandX, m_nOffsetExpandY);
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("Retreive original size => %d x %d"), m_MaskE->GetCo(),m_MaskE->GetLi()));

	shared_ptr<void> pvH;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"Hf",pvH))
	{
		m_Hf = static_pointer_cast<H3_MATRIX_FLT32> (pvH);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("Hf Size = %d x %d"), m_Hf->GetCo(),m_Hf->GetLi()));
	}
	pvH.reset();
	ReduceMatrixPadArray(*m_Hf.get(),m_nOffsetExpandX, m_nOffsetExpandY);
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("Retreive original size => %d x %d"), m_Hf->GetCo(),m_Hf->GetLi()));

	m_PV10.reset(new  H3_MATRIX_FLT32(m_Hf->GetLi(), m_Hf->GetCo()));
	m_PV2.reset(new  H3_MATRIX_FLT32(m_Hf->GetLi(), m_Hf->GetCo()));

	m_oCVHf		=  Mat(m_Hf->GetLi(),	m_Hf->GetCo(),	CV_32F,	m_Hf->GetData(),	Mat::AUTO_STEP);
	m_oCVPV10	=  Mat(m_PV10->GetLi(),	m_PV10->GetCo(),CV_32F,	m_PV10->GetData(),	Mat::AUTO_STEP);
	m_oCVPV2	=  Mat(m_PV2->GetLi(),	m_PV2->GetCo(),	CV_32F,	m_PV2->GetData(),	Mat::AUTO_STEP);

	m_oCVHf2sav = m_oCVHf.clone();

	m_oLstPv10.clear();
	m_oLstPv2.clear();

	double dssStart = 0.0;
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		LogThis(1,1,Fmt(_T("##### Start Generate Results")));
		dssStart = GetPerfTime();
	}

	Mat oZeroPV = Mat::zeros(m_oCVHf.rows,m_oCVHf.cols,CV_32F);
	oZeroPV.copyTo(m_oCVPV10);
	oZeroPV.copyTo(m_oCVPV2);
	oZeroPV.release();

	// lissage
	if(m_nLissageSize > 0)
	{
		Mat Kernel = Mat::ones(m_nLissageSize,m_nLissageSize,CV_32F) / (m_nLissageSize * m_nLissageSize);
		filter2D(m_oCVHf,m_oCVHf,CV_32F,Kernel);
		Kernel.release();

		if(pSavData && (uDbgFlag & DBG_REC_FILTERLISS))
		{
			tSpT<H3_MATRIX_FLT32> elt;
			elt._cs  = _T("FilterLiss");
			elt._spT.reset(new H3_MATRIX_FLT32(*m_Hf.get())); // copy since data altered in following treatments
			elt._bImg = true;
			elt._bAutoScale = true;
			elt._bHbf = false;
			elt._bBin = true;
			pSavData->spListF32.push_back(elt);
		}
	}

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	// on ronge encore un peu les bords
	m_oCVMatMaskE = Mat(m_MaskE->GetLi(), m_MaskE->GetCo(), CV_8U, m_MaskE->GetData(), Mat::AUTO_STEP);
	if(m_nPrmErodeRadius > 0)
	{
		Mat elementErode = getStructuringElement( MORPH_ELLIPSE,	Size( 2*m_nPrmErodeRadius, 2*m_nPrmErodeRadius ),	Point( m_nPrmErodeRadius, m_nPrmErodeRadius ) );
		erode( m_oCVMatMaskE, m_oCVMatMaskE, elementErode );
		elementErode.release();

		if(pSavData && (uDbgFlag & DBG_REC_ERODE))
		{
			tSpT<H3_MATRIX_UINT8> elt;
			elt._cs  = _T("ResErode");
			elt._spT.reset(new H3_MATRIX_UINT8(*m_MaskE.get())); // copy since data altered in following treatments
			elt._bImg = true;
			elt._bAutoScale = true;
			elt._bHbf = false;
			elt._bBin = false;
			pSavData->spListU8.push_back(elt);
		}
	}

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	Mat oMskE32f;
	m_oCVMatMaskE.convertTo(oMskE32f,CV_32F);
	m_oCVHf = oMskE32f.mul(m_oCVHf); 


	//
	// Some EXTRA data computation
	//
	double dmin, dmax;
	minMaxLoc(m_oCVHf,&dmin,&dmax,nullptr,nullptr,m_oCVMatMaskE);
	
	// peak calculation 
	m_fPeak = static_cast<float> (dmax);
	// PV calculation
	m_fPV = static_cast<float>(dmax-dmin);
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Peak = %lf nm ---- PV = %lf nm"), m_fPeak, m_fPV));

	////////////////////////////////////////////////////////////////////////// 
	CWinThread* pThreadPV2;
	if(uDbgFlag & DBG_SKIP_PV2)
	{
		SetEvent(m_hEventPVDone[0]);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("# Skip PV2")));
	}
	else
	{
		pThreadPV2 = AfxBeginThread(&CTreatGenerateResults0::DoPV2, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
		pThreadPV2->m_bAutoDelete = TRUE; // the thread delete its self after completion
		pThreadPV2->ResumeThread();
		Sleep(10);
	}


	CWinThread* pThreadPV10;
	if(uDbgFlag & DBG_SKIP_PV10)
	{
		SetEvent(m_hEventPVDone[1]);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("# Skip PV10")));
	}
	else
	{
		pThreadPV10 = AfxBeginThread(&CTreatGenerateResults0::DoPV10, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
		pThreadPV10->m_bAutoDelete = TRUE; // the thread delete its self after completion
		pThreadPV10->ResumeThread();
	}

	
	//
	// RMS : root mean square calculation
	//
	Mat oCVHfSquare = m_oCVHf.mul(m_oCVHf); 
	Scalar fSum = sum(oCVHfSquare);
	Scalar fNs = sum(m_oCVMatMaskE);
	float  frmssq = fSum[0]/fNs[0];
	m_fRMS = sqrt(frmssq);

	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# RMS = %lf"), m_fRMS));

	CString csOutPath	= _T("C:\\Altasight\\Nano\\Data");
	void* pcs;
	if(FindTreatPrmPtr(p_InputsPrm,"OutPath",pcs))
	{
		csOutPath = *((CString*) pcs); 	
	}

	//
	// compute SITE PV
	//
	if(m_bUseSitePVMap)
	{
		ComputeSitePV(csOutPath);
	}

	dwEvent = WaitForMultipleObjects(sizeof(m_hEventPVDone) / sizeof(HANDLE), m_hEventPVDone, TRUE, INFINITE);
	double dssEnd = GetPerfTime();
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done Results Computation in %lf ms"), dssEnd - dssStart));


	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	if(pSavData && (uDbgFlag & DBG_REC_PV10))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("PV10");
		elt._spT.reset(new H3_MATRIX_FLT32(*m_PV10.get())); // copy since data altered in following treatments

		elt._bImg = true;
		elt._bAutoScale = false;
		elt._fMin = (float) 0;
		elt._fMax = (float) m_nMaxPVDisplay;

		elt._bHbf = false;
		elt._bBin = true;
		pSavData->spListF32.push_back(elt);
	}

	if(pSavData && (uDbgFlag & DBG_REC_PV2))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("PV2");
		elt._spT.reset(new H3_MATRIX_FLT32(*m_PV2.get())); // copy since data altered in following treatments

		elt._bImg = true;
		elt._bAutoScale = false;
		elt._fMin = (float) 0;
		elt._fMax = (float) m_nMaxPVDisplay;

		elt._bHbf = false;
		elt._bBin = true;
		pSavData->spListF32.push_back(elt);
	}

	dssStart = GetPerfTime();

	

	m_oCVPV10 = oMskE32f.mul(m_oCVPV10); 
	m_oCVPV2 = oMskE32f.mul(m_oCVPV2); 

	SaveADNFile(csOutPath, m_bUseDataCompression);
	if(m_bUseResumeLotStats)
		SaveStats(csOutPath);
	dssEnd = GetPerfTime();
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done Results record in %lf ms"), dssEnd - dssStart));



	// DEBUG - save profil X and Y
	if(0)
	{	
		CTime oTime = CTime::GetCurrentTime();
		CString csTime = oTime.Format(_T("ProfileXY-%m%d%Y_%H%M.csv"));
		CString csprofilesfiles = csOutPath + _T("\\");
		csprofilesfiles += csTime;
		CStdioFile oFile;
		int nProfX_y = 1624; int nProfX_Start_x = 240; int nProfX_End_x = 3364;
		int nProfY_x = 1780; int nProfY_Start_y = 62; int nProfY_End_y = (nProfY_Start_y + nProfX_End_x - nProfX_Start_x);

		if (oFile.Open(csprofilesfiles, CFile::modeCreate | CFile::modeWrite))
		{
			oFile.WriteString(Fmt("zX(@y=%d);zY(@x=%d);\n",nProfX_y,nProfY_x));
			int nNbPtsPcx = nProfX_End_x - nProfX_Start_x;
			for (int n=0; n<=nNbPtsPcx; n++)
			{
				float pX = m_oCVHf.at<float>(nProfX_y,nProfX_Start_x + n);
				float pY = m_oCVHf.at<float>(nProfY_Start_y + n,nProfY_x);
				oFile.WriteString(Fmt("%lf;%lf;\n",pX,pY));
			}
			oFile.Close();
		}
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
		pSavData->csPath	= csOutPath;
		pSavData->csPath += _T("\\Dbg\\");
		pSavData->csPath += m_csLotID;

		CWinThread* pThread = AfxBeginThread(&CTreatGenerateResults0::SaveData, pSavData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
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

	//AddTreatPrmSharedPtr(p_OutputsPrm,"Hf",shared_ptr<void>(m_Hf));
	
 	if(uDbgFlag & DBG_SHOW_DISPLAY)
	{	
		Mat draw;
		double dminVal =  0;
		double dmaxVal =  m_nMaxPVDisplay;
		m_oCVPV10.convertTo(draw, CV_8U, 255.0/(dmaxVal - dminVal), - dminVal * 255.0f / (dmaxVal - dminVal));
		imshow( "PV10", draw );

		m_oCVPV2.convertTo(draw, CV_8U, 255.0/(dmaxVal - dminVal), - dminVal * 255.0f / (dmaxVal - dminVal));
		imshow( "PV2", draw );
	}

	m_oCVPV10.release();
	m_PV10.reset();
	m_oCVPV2.release();
	m_PV2.reset();

	m_MaskE.reset();
	m_Hf.reset();

	return true;
}*/

UINT CTreatGenerateResults0::DoPV2( void *p_pParameters )
{
	CTreatGenerateResults0* pObj = static_cast<CTreatGenerateResults0 *> (p_pParameters);

	// on recherche une fenetre de 2mm soit (2000µm)
	int nR_px = (int) (pObj->m_fPixelSize * 2000.f);
	float fLimit = 0.0f;
	if(pObj->m_bUseDiskPV)
	{
		pObj->MakeMaskPV(pObj->m_oCVMaskDisk2,nR_px);
		if(pObj->m_fLimitCoef)
		{
			Scalar fsum = sum(pObj->m_oCVMaskDisk2);
			fLimit = fsum[0] - 1;
		}
	}
	else
	{
		if(pObj->m_fLimitCoef)
			fLimit = (nR_px * nR_px) - 1;
	}
	pObj->DoPV(pObj->m_oCVPV2,nR_px,fLimit,pObj->m_oCVMaskDisk2, pObj->m_oLstPv2, pObj->m_fTHA2, pObj->m_hEventPVDone[0]);
	return 902;
}

UINT CTreatGenerateResults0::DoPV10( void *p_pParameters )
{
	CTreatGenerateResults0* pObj = static_cast<CTreatGenerateResults0 *> (p_pParameters);
	
	// on recherche une fenetre de 10mm soit (10000µm)
	int nR_px = (int) (pObj->m_fPixelSize * 10000.f);
	float fLimit = 0.0;
	
	if(pObj->m_bUseDiskPV)
	{
		pObj->MakeMaskPV(pObj->m_oCVMaskDisk10,nR_px);
		if(pObj->m_fLimitCoef)
		{
			Scalar fsum = sum(pObj->m_oCVMaskDisk10);
			fLimit = fsum[0] - 1;
		}
	}
	else
	{
		if(pObj->m_fLimitCoef)
			fLimit = (nR_px * nR_px) - 1;
	}
	pObj->DoPV(pObj->m_oCVPV10,nR_px,fLimit,pObj->m_oCVMaskDisk10, pObj->m_oLstPv10,  pObj->m_fTHA10, pObj->m_hEventPVDone[1]);
	return 910;
}

void CTreatGenerateResults0::MakeMaskPV(cv::Mat& p_oMaskPv, int p_nrsz)
{
	p_oMaskPv = getStructuringElement( MORPH_ELLIPSE,	Size( p_nrsz, p_nrsz ),	Point( p_nrsz/2, p_nrsz/2 ) );
}

void CTreatGenerateResults0::DoPV(cv::Mat& p_oPv, int p_nrsz, float p_fLimit, cv::Mat& p_oMaskDisk, std::list<tPtFlt>& p_oLstPv, float& p_fTHA, HANDLE& p_hEvt)
{
	int x,y;

	Mat ovX = Mat::zeros(1,m_oCVHf.rows*m_oCVHf.cols,CV_32F);
	std::list<float> PvX;
	std::list<float> PvY;

	//int nNumThreads = 8;
	int nHalfnrsz = p_nrsz/2;
	#pragma omp parallel private(x)
	{
		#pragma omp for schedule (dynamic, m_oCVHf.rows/20)
		for(y = nHalfnrsz; y<m_oCVHf.rows-nHalfnrsz; y = y + 2 )
		{
			if (m_bEmergencyStop)
			{
				continue ;
			}

			for(x = nHalfnrsz; x<m_oCVHf.cols-nHalfnrsz; x = x + 2)
			{
				Rect nRcAreaROI = Rect(x-nHalfnrsz,y-nHalfnrsz,p_nrsz,p_nrsz);
				Mat msk;
				if (m_bUseDiskPV)
					msk = p_oMaskDisk.mul(m_oCVMatMaskE(nRcAreaROI));
				else
					msk = m_oCVMatMaskE(nRcAreaROI);
				Scalar fsum = sum(msk);
				if(fsum[0] > p_fLimit) // si on a suffisamment d'info sur le bord sinon on reste à 0
				{
					double dmin, dmax;
					minMaxLoc(m_oCVHf(nRcAreaROI),&dmin,&dmax,nullptr,nullptr,msk);
					float fDiff = static_cast<float>(dmax-dmin) ;
					if(fDiff > 0.0f)
					{
						p_oPv.at<float>(y,x) = fDiff;
						ovX.at<float>(y*m_oCVHf.cols+x) = fDiff;

						p_oPv.at<float>(y,x+1) = fDiff;
						p_oPv.at<float>(y+1,x) = fDiff;
						p_oPv.at<float>(y+1,x+1) = fDiff;
					}
				}
			}
		}
	}

	// start cfdcalc
	if (m_bEmergencyStop)
	{
		SetEvent(p_hEvt);
		return ;
	}

	// sort ascending
	cv::sort(ovX,ovX, CV_SORT_EVERY_ROW | CV_SORT_ASCENDING );

	if (m_bEmergencyStop)
	{
		SetEvent(p_hEvt);
		return ;
	}

	#pragma omp parallel for ordered
	for(y = 0; y<ovX.total(); y++)
	{
		#pragma omp  ordered
		{
			if(ovX.at<float>(y) > 0)
				PvX.push_back(ovX.at<float>(y));
		}
	}
	ovX.release();

	// valeur du THA@0.05 c'est le 99.95nth element
	int nRank = (int) (PvX.size() * 0.0005f);
	if (nRank <= 0)
	{
		p_fTHA = 0.0f;
	}
	else
	{
		std::list<float>::iterator iter = PvX.end();
		std::advance(iter,-nRank);
		p_fTHA = *iter;
	}
	if ((m_uDbgFlag | m_uRegFlag) & DBG_SHOWDEBUG_LOG)
	{	
		LogThis(1,1,Fmt(_T("************THA@0.05 = %f -- inv rank = %d"),p_fTHA, nRank));
	}

	long lN = (long) PvX.size();
	
	//LogThis(1,1,Fmt(_T("******************* Start DO PV pVY")));
	long l;
	if (m_bEmergencyStop)
	{
		SetEvent(p_hEvt);
		return ;
	}

	#pragma omp parallel for ordered
	for (l=1;  l<=lN; l++)
	{
		#pragma omp  ordered
		{
			PvY.push_back((1.0f - ((float)(l) /(float)(lN))) * 100.0f);
		}
	}

	// remove duplicate
	if(PvX.size()  > 0)
	{
		std::list<float>::iterator itY = PvY.begin();
		std::list<float>::iterator itX = PvX.begin();
		std::list<float>::iterator itXplus1 = itX;
		std::list<float>::iterator itEndm = PvX.end();
		++itXplus1;
		--itEndm;
		while (itX != itEndm)
		{
			if (m_bEmergencyStop)
			{
				SetEvent(p_hEvt);
				return ;
			}

			if ( (*itXplus1) - (*itX) > 0)
			{
				++itY;
				++itX;
				++itXplus1;
			}
			else
			{
				itX = PvX.erase(itX);
				itY = PvY.erase(itY);
				itXplus1 = itX;
				++itXplus1;
			}
		}
	}
	// end cfdcalc

	// downgrade
	int nMod = ((int) PvX.size()) / m_nCurvePts;
	ASSERT(PvX.size() == PvY.size());
	int nIdx = 0;
	while (PvX.size() != 0)
	{
		if (m_bEmergencyStop)
		{
			SetEvent(p_hEvt);
			return ;
		}

		++nIdx;
		if(nIdx % nMod == 0)
		{
			tPtFlt oPt;
			oPt._x = PvX.front();
			oPt._y = PvY.front();
			p_oLstPv.push_back(oPt);
		}
		PvX.pop_front();
		PvY.pop_front();
	}

	SetEvent(p_hEvt);
}

void CTreatGenerateResults0::SaveStats(CString p_csOutPath)
{
	
	CString csResumeStatFile; // format - ResumeLotID-<FoupID>.csv

	csResumeStatFile = p_csOutPath + _T("\\");
	csResumeStatFile += _T("ResumeLotID-");
	csResumeStatFile += m_csFoupID;
	csResumeStatFile += _T(".csv");

	CTime oTime = CTime::GetCurrentTime();
	//CString csTime = oTime.Format(_T("%Y;%m;%d;%H;%M;%S;\n"));
	CString csTime = oTime.Format(_T("%m/%d/%Y %H:%M"));

	if(PathFileExists((LPCTSTR) csResumeStatFile))
	{
		// we append the current data to the file
		CStdioFile oFile;
		if (oFile.Open(csResumeStatFile, CFile::modeCreate | CFile::modeNoTruncate | CFile::modeWrite))
		{
			oFile.SeekToEnd();

			// Data
			oFile.WriteString(Fmt("%s,,%s,,,,,%0.2lf,%0.2lf,%0.2lf,%0.2lf,%0.2lf,,,,,,0\n",
				(LPCTSTR)csTime,(LPCTSTR)m_csLotID,m_fPV,m_fPeak,m_fTHA2,m_fTHA10,m_fRMS));
	
			oFile.Close();
		}
	}
	else
	{
		// we create the file and add the first labelled line

		CStdioFile oFile;
		if (oFile.Open(csResumeStatFile, CFile::modeCreate | CFile::modeWrite))
		{
		
			//
			// ENTER HEADER FILE
			//
			
			//Job Field:,{Name: Directory  Value: YIBBB204-01  Output path portion: (Directory)},,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Job Field:,{Name: Directory  Value: Output path portion: (Directory)},,,,,,,,,,,,,,,,\n"));
			//Acquired By:,{WaferSight 2 Master v2.3.0.17  WaferSight 2 Slave v2.3.0.17  WaferSight 2 308},,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Acquired By:,{Altasight system},,,,,,,,,,,,,,,,\n"));
			//Processed By:,WaferSight 2 v2.3.0.17,,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Processed By:,Altasight system,,,,,,,,,,,,,,,,\n"));
			//Recipe File:,Recipes:SOI:FAT_DG,,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Recipe File:,,,,,,,,,,,,,,,,,\n"));
			//Filtering:,{Filter Shape: Double Gaussian  Center Cutoff: 20 mm  Edge Cutoff: 1 mm},,,,,,,,,,,,,,,,
			oFile.WriteString(Fmt(_T("Filtering:,{Filter Shape: %d  Center Cutoff: 20 mm  Edge Cutoff: 1 mm},,,,,,,,,,,,,,,,\n"),m_nFilterType));
			//Data Directory:,G:\data\SOI\YIBBB204-01\,,,,,,,,,,,,,,,,
			oFile.WriteString(Fmt(_T("Data Directory:,%s,,,,,,,,,,,,,,,,\n"),(LPCTSTR)p_csOutPath));
			//Edge Exclusion:,{Front: {Pre-filter Edge Exclusion: 1.8 mm  Post-filter Edge Exclusion: 2 mm}  Back: {Pre-filter Edge Exclusion: 1.8 mm  Post-filter Edge Exclusion: 2 mm}},,,,,,,,,,,,,,,,
			oFile.WriteString(Fmt(_T("Edge Exclusion:,{Front: {Pre-filter Edge Exclusion: %0.1lf mm  Post-filter Edge Exclusion: %0.1lf mm}  Back: {Pre-filter Edge Exclusion: 0.0 mm  Post-filter Edge Exclusion: 0.0 mm}},,,,,,,,,,,,,,,,\n"),m_fEdgeExclusion_mm,m_fEdgeExclusion_mm));			
			//Masks:,{Front: {}  Back: {}},,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Masks:,{Front: {}  Back: {}},,,,,,,,,,,,,,,,\n"));
			//Transforms:,{},,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Transforms:,{},,,,,,,,,,,,,,,,\n"));
			//Categories:,{},,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Categories:,{},,,,,,,,,,,,,,,,\n"));
			//Category Types:,{},,,,,,,,,,,,,,,,
			oFile.WriteString(_T("Category Types:,{},,,,,,,,,,,,,,,,\n"));
			//==========,,,,,,,,,,,,,,,,,
			oFile.WriteString(_T("==========,,,,,,,,,,,,,,,,,\n"));
			//Acquisition Date/Time,Source Carrier ID,Wafer ID,Load Port,Source Slot,Job Cycle,Job Repeat,Front PV (nm),Front Peak (nm),Front THA (2 mm Circle PV) @ 0.05 % (nm),Front THA (10 mm Circle PV) @ 0.05 % (nm),Front RMS (nm),Back PV (nm),Back Peak (nm),Back THA (2 mm Circle PV) @ 0.05 % (nm),Back THA (10 mm Circle PV) @ 0.05 % (nm),Back RMS (nm),Rotation (deg)
			if(m_bUseDiskPV)
				oFile.WriteString(_T("Acquisition Date/Time,Source Carrier ID,Wafer ID,Load Port,Source Slot,Job Cycle,Job Repeat,Front PV (nm),Front Peak (nm),Front THA (2 mm Circle PV) @ 0.05 % (nm),Front THA (10 mm Circle PV) @ 0.05 % (nm),Front RMS (nm),Back PV (nm),Back Peak (nm),Back THA (2 mm Circle PV) @ 0.05 % (nm),Back THA (10 mm Circle PV) @ 0.05 % (nm),Back RMS (nm),Rotation (deg)\n"));
			else
				oFile.WriteString(_T("Acquisition Date/Time,Source Carrier ID,Wafer ID,Load Port,Source Slot,Job Cycle,Job Repeat,Front PV (nm),Front Peak (nm),Front THA (2 mm Square PV) @ 0.05 % (nm),Front THA (10 mm Square PV) @ 0.05 % (nm),Front RMS (nm),Back PV (nm),Back Peak (nm),Back THA (2 mm Square PV) @ 0.05 % (nm),Back THA (10 mm Square PV) @ 0.05 % (nm),Back RMS (nm),Rotation (deg)\n"));


			// Data
			oFile.WriteString(Fmt("%s,,%s,,,,,%0.2lf,%0.2lf,%0.2lf,%0.2lf,%0.2lf,,,,,,0\n",
									(LPCTSTR)csTime,(LPCTSTR)m_csLotID,m_fPV,m_fPeak,m_fTHA2,m_fTHA10,m_fRMS));
		
			oFile.Close();
		}
	}
}

void CTreatGenerateResults0::SaveADNFile(CString p_csOutPath, bool  p_bCompressData )
{
	// Save data in file 
	// during saving extension is tmpadn after save is complete fileextension is rename in adn

	// adn file contain 
	// Property set : WaferID; THA-PV10; THA-PV2; PixelSize; UseDiskPV; ErodeRadius;  
	// FoupID; CompressStatus; PV,PEAK,RMS
	// - CurvesPV10		* NbPoints
	//					* DataPoints
	// - CurvesPV2		* NbPoints
	//					* DataPoints
	// - HFilter	* widht | Heigth 
	//				* Max | Min | Mean | StdDev
	//				* Data
	// {Falcultatif}
	// - HGlobal	* widht | Heigth 
	//				* Max | Min | Mean | StdDev
	//				* Data
	// - ImgPV10	* widht | Heigth 
	//				* Max | Min | Mean | StdDev
	//				* Data
	// - ImgPV2		* widht | Heigth 
	//				* Max | Min | Mean | StdDev
	//				* Data

	//Creation des Storages

	HRESULT				hr = S_OK;
	ULONG				nBytesWritten = 0;
	IStorage			*pRootStg	= NULL;
	IStorage			*pCurvePV10Stg	= NULL;
	IStorage			*pCurvePV2Stg	= NULL;
	IStorage			*pHfStg		= NULL;

// 	IStorage			*pHGlobalStg = NULL;
// 	IStorage			*pPV10Stg	= NULL;
// 	IStorage			*pPV2Stg	= NULL;

	CString csADNFilePath = p_csOutPath + _T("\\");
	CString csTime = (CTime::GetCurrentTime()).Format(_T("_%Y%m%d_%H%M%S.adn"));
	CString csPrefix;
	csPrefix.Format("LotID-%s",m_csLotID);
	csADNFilePath +=csPrefix;
	csADNFilePath +=csTime;

	// Convert to a wchar_t*
	// You must first convert to a char * for this to work.
	const size_t origsize = strlen(csADNFilePath) + 1;
	size_t convertedChars = 0;
	WCHAR*  wszADNTempName = new WCHAR[origsize];
	mbstowcs_s(&convertedChars, wszADNTempName, origsize, csADNFilePath, _TRUNCATE);

	try
	{
		// Create an object with an IStorage interface. It is not 
		// necessary that it be a system-provided storage, such as 
		// that obtained by this call.  Any object that implements 
		// IStorage can be used.
		//WCHAR * wszADNTempName = L"c:\\Altasight\\Nano\\Data\\Test.tmpadn";
		hr = StgCreateStorageEx( wszADNTempName , 
			STGM_CREATE
			| STGM_READWRITE
			| STGM_SHARE_EXCLUSIVE,
			STGFMT_STORAGE,
			0, NULL, NULL,
			IID_IStorage,
			reinterpret_cast<void**>(&pRootStg) );
		if( FAILED(hr) ) 
			throw L"Failed ROOT StgCreateStorageEx";

		//
		// Properties
		//

		//WaferID; FoupID; THA-PV10; THA-PV2; PixelSize; UseDiskPV; ErodeRadius; 

		// Open Stream 
		IStream* pPropStream = NULL;
		hr = pRootStg->CreateStream(L"WaferID",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pPropStream);
		if( FAILED(hr) ) 
			throw L"Failed  CreateStream WaferID";
		int nLen = m_csLotID.GetLength();
		nBytesWritten = 0;
		pPropStream->Write(&nLen,sizeof(int),&nBytesWritten);
		pPropStream->Write(m_csLotID.GetBuffer(),m_csLotID.GetLength()*sizeof(TCHAR),&nBytesWritten);
		m_csLotID.ReleaseBuffer();
		pPropStream->Release(); pPropStream = NULL;

		hr = pRootStg->CreateStream(L"FoupID",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pPropStream);
		if( FAILED(hr) ) 
			throw L"Failed  CreateStream FoupID";
		nLen = m_csFoupID.GetLength();
		pPropStream->Write(&nLen,sizeof(int),&nBytesWritten);
		pPropStream->Write(m_csFoupID.GetBuffer(),m_csFoupID.GetLength()*sizeof(TCHAR),&nBytesWritten);
		m_csFoupID.ReleaseBuffer();
		pPropStream->Release(); pPropStream = NULL;

		hr = pRootStg->CreateStream(L"CompressStatus",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pPropStream);
		if( FAILED(hr) ) 
			throw L"Failed  CreateStream WaferID";
		int nIsCompress = (p_bCompressData ? 1 : 0);
		pPropStream->Write(&nIsCompress,sizeof(int),&nBytesWritten);
		pPropStream->Release(); pPropStream = NULL;

		hr = pRootStg->CreateStream(L"StatsData",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pPropStream);
		if( FAILED(hr) ) 
			throw L"Failed H Filter CreateStream StatsData";
		pPropStream->Write(&m_fPV,sizeof(m_fPV),&nBytesWritten);	
		pPropStream->Write(&m_fPeak,sizeof(m_fPeak),&nBytesWritten);	
		pPropStream->Write(&m_fRMS,sizeof(m_fRMS),&nBytesWritten);	
		pPropStream->Write(&m_nFilterType,sizeof(int),&nBytesWritten);	
		pPropStream->Release(); pPropStream = NULL;

		hr = pRootStg->CreateStream(L"THA",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pPropStream);
		if( FAILED(hr) ) 
			throw L"Failed  CreateStream THA";
		double dTHApv10 = (double) m_fTHA10; 
		pPropStream->Write(&dTHApv10,sizeof(double),&nBytesWritten);
		double dTHApv2 = (double) m_fTHA2; 
		pPropStream->Write(&dTHApv2,sizeof(double),&nBytesWritten);
		pPropStream->Release(); pPropStream = NULL;

		hr = pRootStg->CreateStream(L"Params",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pPropStream);
		if( FAILED(hr) ) 
			throw L"Failed  CreateStream Params";
		double dPixelSize = (double) m_fPixelSize;
		pPropStream->Write(&dPixelSize,sizeof(double),&nBytesWritten);
		int nUseDisk = (m_bUseDiskPV ? 1 : 0);
		pPropStream->Write(&nUseDisk,sizeof(int),&nBytesWritten);
		pPropStream->Write(&m_nPrmErodeRadius,sizeof(int),&nBytesWritten);
		double dEdgeExclusion_mm = (double) m_fEdgeExclusion_mm;
		pPropStream->Write(&dEdgeExclusion_mm,sizeof(double),&nBytesWritten);
		pPropStream->Release(); pPropStream = NULL;

		//
		// CURVES
		//

		// PV10 curve
		hr = pRootStg->CreateStorage(L"CurvesPV10",	STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pCurvePV10Stg);
		if( FAILED(hr) ) 
			throw L"Failed PV10 CURVES CreateStorage";

		// Open Stream NbPoints
		IStream* pNbPtspv10 = NULL;
		hr = pCurvePV10Stg->CreateStream(L"NbPts",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pNbPtspv10);
		if( FAILED(hr) ) 
			throw L"Failed PV10 CURVES CreateStream NbPts";

		ULONG nBytesWritten = 0;
		int nNbPoints = (int)m_oLstPv10.size();
		pNbPtspv10->Write(&nNbPoints,sizeof(int),&nBytesWritten);
		pNbPtspv10->Release(); pNbPtspv10 = NULL;

		// Open Stream DataPoints
		IStream* pDatapv10 = NULL;
		hr = pCurvePV10Stg->CreateStream(L"DataPts",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pDatapv10);
		if( FAILED(hr) ) 
			throw L"Failed PV10 CURVES CreateStream DataPts";
		if (p_bCompressData)
		{
			float* pDataPts = new float[nNbPoints * 2];
			tPtFlt oPt;
			int nIdx = 0;
			while(m_oLstPv10.size() > 0 && nIdx < (nNbPoints * 2))
			{
				oPt = m_oLstPv10.front();
				pDataPts[nIdx++]=oPt._x;
				pDataPts[nIdx++]=oPt._y;
				m_oLstPv10.pop_front();
			}

			Byte  *pIn = reinterpret_cast<Byte*>(pDataPts);
			uLong nInLen = nNbPoints * 2 * sizeof(float);
			uLong comprLen = nInLen;
			Byte  *compr = new Byte[nInLen];
			int err = compress(compr, &comprLen, pIn, nInLen);

			pDatapv10->Write(&comprLen,sizeof(uLong),&nBytesWritten);
			pDatapv10->Write(compr,comprLen * sizeof(Byte),&nBytesWritten);

			delete[]pDataPts;
			delete[]compr;
		}
		else
		{
			tPtFlt oPt;
			while(m_oLstPv10.size() > 0)
			{
				oPt = m_oLstPv10.front();
				pDatapv10->Write(&oPt._x,sizeof(oPt._x),&nBytesWritten);
				pDatapv10->Write(&oPt._y,sizeof(oPt._y),&nBytesWritten);
				m_oLstPv10.pop_front();
			}
		}
		pDatapv10->Release(); pDatapv10 = NULL;

		// save curve pv10
		pCurvePV10Stg->Release(); pCurvePV10Stg = NULL;

		// PV2 curve
		pRootStg->CreateStorage(L"CurvesPV2",	STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pCurvePV2Stg);
		if( FAILED(hr) ) 
			throw L"Failed PV2 CURVES CreateStorage";

		// Open Stream NbPoints
		IStream* pNbPtspv2 = NULL;
		hr = pCurvePV2Stg->CreateStream(L"NbPts",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pNbPtspv2);
		if( FAILED(hr) ) 
			throw L"Failed PV2 CURVES CreateStream NbPts";

		nBytesWritten = 0;
		nNbPoints = (int)m_oLstPv2.size();
		pNbPtspv2->Write(&nNbPoints,sizeof(int),&nBytesWritten);
		pNbPtspv2->Release(); pNbPtspv2 = NULL;

		// Open Stream DataPoints
		IStream* pDatapv2 = NULL;
		hr = pCurvePV2Stg->CreateStream(L"DataPts",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pDatapv2);
		if( FAILED(hr) ) 
			throw L"Failed PV2 CURVES CreateStream DataPts";
		if (p_bCompressData)
		{
			float* pDataPts = new float[nNbPoints * 2];
			tPtFlt oPt;
			int nIdx = 0;
			while(m_oLstPv2.size() > 0 && nIdx < (nNbPoints * 2))
			{
				oPt = m_oLstPv2.front();
				pDataPts[nIdx++]=oPt._x;
				pDataPts[nIdx++]=oPt._y;
				m_oLstPv2.pop_front();
			}

			Byte  *pIn = reinterpret_cast<Byte*>(pDataPts);
			uLong nInLen = nNbPoints * 2 * sizeof(float);
			uLong comprLen = nInLen;
			Byte  *compr = new Byte[nInLen];
			int err = compress(compr, &comprLen, pIn, nInLen);

			pDatapv2->Write(&comprLen,sizeof(uLong),&nBytesWritten);
			pDatapv2->Write(compr,comprLen * sizeof(Byte),&nBytesWritten);

			delete[]pDataPts;
			delete[]compr;
		}
		else
		{
			tPtFlt oPt;
			while(m_oLstPv2.size() > 0)
			{
				oPt = m_oLstPv2.front();
				pDatapv2->Write(&oPt._x,sizeof(oPt._x),&nBytesWritten);
				pDatapv2->Write(&oPt._y,sizeof(oPt._y),&nBytesWritten);
				m_oLstPv2.pop_front();
			}
		}
		pDatapv2->Release(); pDatapv2 = NULL;

		// save curve pv2
		pCurvePV2Stg->Release(); pCurvePV2Stg = NULL;

		//
		// H Filter
		//

		pRootStg->CreateStorage(L"HFilter",	STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pHfStg);
		if( FAILED(hr) ) 
			throw L"Failed HFilter CreateStorage";

		IStream* pHfStream = NULL;
		hr = pHfStg->CreateStream(L"Size",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pHfStream);
		if( FAILED(hr) ) 
			throw L"Failed H Filter CreateStream Size";

		nBytesWritten = 0;
		pHfStream->Write(&m_oCVHf.cols,sizeof(int),&nBytesWritten); // width
		pHfStream->Write(&m_oCVHf.rows,sizeof(int),&nBytesWritten); // height
		pHfStream->Release(); pHfStream = NULL;

		hr = pHfStg->CreateStream(L"Stats",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pHfStream);
		if( FAILED(hr) ) 
			throw L"Failed H Filter CreateStream Stats";

		double minVal, maxVal;
		minMaxLoc(m_oCVHf, &minVal, &maxVal); 
		Scalar scmean;	Scalar scstddev;
		meanStdDev(m_oCVHf,scmean,scstddev);
		pHfStream->Write(&maxVal,sizeof(maxVal),&nBytesWritten);				// MAX
		pHfStream->Write(&minVal,sizeof(minVal),&nBytesWritten);				// MIN
		pHfStream->Write(&scmean[0],sizeof(scmean[0]),&nBytesWritten);			// MEAN
		pHfStream->Write(&scstddev[0],sizeof(scstddev[0]),&nBytesWritten);		// STDDEV
		pHfStream->Release(); pHfStream = NULL;

		hr = pHfStg->CreateStream(L"Data",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pHfStream);
		if( FAILED(hr) ) 
			throw L"Failed H Filter CreateStream Data";

// 		float fstep =  512.0f / ((m_oCVHf2sav.rows - 1));
// 		for(int i=0;i<m_oCVHf2sav.rows;i++)
// 			for(int j=0;j<m_oCVHf2sav.cols;j++)
// 				m_oCVHf2sav.at<float>(i,j) = i * fstep;

	//	pHfStream->Write(m_oCVHf2sav.data(),sizeof(float)*m_oCVHf2sav.cols*m_oCVHf2sav.rows,&nBytesWritten);	// a tester

		// on calcule les index sur un quadrant en partant du 0 
/*		int nLi2 = m_oCVHf2sav.rows/2;
		int nCo2 = m_oCVHf2sav.cols/2;
		int nVign2	= 128 / 2;
		int nStepX=64;
		int nStepY=nStepX;
		int nHalfMAX_J = 1;
		if(nStepY != 0)
			nHalfMAX_J = (int) floor((float)(nLi2)/(float)nStepY);
		int nHalfMAX_I = 1;
		if(nStepX != 0)
			nHalfMAX_I = (int) floor((float)(nCo2)/(float)nStepX);
		int i,j;
		int nZeroVignPos_Y = nLi2 - nVign2;
		int nZeroVignPos_X = nCo2 - nVign2;
		int nFirstVignPosY = nZeroVignPos_Y - nHalfMAX_J * nStepY;
		int nFirstVignPosX = nZeroVignPos_X - nHalfMAX_I * nStepX;

		Point pt;
		float fffval = 0.0;

		// MIDDLE START REF
		pt.y = nFirstVignPosY + nHalfMAX_J * nStepY + nVign2;
		pt.x = nFirstVignPosX + nHalfMAX_I * nStepX + nVign2;
		fffval = 2000.0f;

		for (i = -10; i<=10; i++)
		{
			m_oCVHf2sav.at<float>(pt.y-1,	pt.x+i)	= fffval;
			m_oCVHf2sav.at<float>(pt.y,		pt.x+i)	= fffval;
			m_oCVHf2sav.at<float>(pt.y+1,	pt.x+i)	= fffval;
		}
		for (j = -10; j<=10; j++)
		{
			m_oCVHf2sav.at<float>(pt.y+j,	pt.x-1)	= fffval;
			m_oCVHf2sav.at<float>(pt.y+j,	pt.x)	= fffval;
			m_oCVHf2sav.at<float>(pt.y+j,	pt.x+1)	= fffval;
		}

		// All other cross
		for (j=0; j<2*nHalfMAX_J; j++)
		{
		
			pt.y = nFirstVignPosY + j * nStepY + nVign2;
			for (i=0; i<2*nHalfMAX_I; i++)
			{
				pt.x = nFirstVignPosX + i * nStepX + nVign2;

		
				if(j%2 == 0)
				{
					if(i%2 == 0)
						fffval = 1000.0f;
					else
						fffval = -1000.0f;
				}
				else
				{
					if(i%2 == 0)
						fffval = -1000.0f;
					else
						fffval = 1000.0f;
				}

				m_oCVHf2sav.at<float>(pt.y,pt.x)	= fffval;
				m_oCVHf2sav.at<float>(pt.y,pt.x-2)	= fffval;
				m_oCVHf2sav.at<float>(pt.y,pt.x-1)	= fffval;
				m_oCVHf2sav.at<float>(pt.y,pt.x+1)	= fffval;
				m_oCVHf2sav.at<float>(pt.y,pt.x+2)	= fffval;
				m_oCVHf2sav.at<float>(pt.y-2,pt.x)	= fffval;
				m_oCVHf2sav.at<float>(pt.y-1,pt.x)	= fffval;
				m_oCVHf2sav.at<float>(pt.y+1,pt.x)	= fffval;
				m_oCVHf2sav.at<float>(pt.y+2,pt.x)	= fffval;
			}
		}
*/

        // Write adn file
		if (p_bCompressData)
		{
			Byte  *pIn = reinterpret_cast<Byte*>( m_oCVHf2sav.data );
			uLong nInLen = m_oCVHf2sav.rows*m_oCVHf2sav.cols*sizeof(float);
			uLong comprLen = nInLen;
			Byte  *compr = new Byte[nInLen];
			int err = compress(compr, &comprLen, pIn, nInLen);

			pHfStream->Write(&comprLen,sizeof(uLong),&nBytesWritten);
			pHfStream->Write(compr,comprLen * sizeof(Byte),&nBytesWritten);

			delete[]compr;
		}
		else
		{
			for(int i=0;i<(m_oCVHf2sav.rows*m_oCVHf2sav.cols);i++)
			{
				pHfStream->Write(&(m_oCVHf2sav.at<float>(i)),sizeof(float),&nBytesWritten);	
			}
		}
		pHfStream->Release(); pHfStream = NULL;

		if(m_bUseMaskPVData)
		{
			hr = pHfStg->CreateStream(L"MaskPVData",STGM_CREATE | STGM_READWRITE | STGM_SHARE_EXCLUSIVE,0,0,&pHfStream);
			if( FAILED(hr) ) 
				throw L"Failed H Filter CreateStream MaskPVData";
			pHfStream->Write(&m_fPV10HiValue_Thresh_nm,sizeof(float),&nBytesWritten);
			pHfStream->Write(&m_fPV2HiValue_Thresh_nm,sizeof(float),&nBytesWritten);
			pHfStream->Write(&m_oCVPV10.cols,sizeof(int),&nBytesWritten);
			pHfStream->Write(&m_oCVPV10.rows,sizeof(int),&nBytesWritten);

			if(p_bCompressData)
			{	
				uLong nInLen = m_oCVHf2sav.rows*m_oCVHf2sav.cols*sizeof(Byte);
				Byte  *pIn = new Byte[nInLen];
				for(int i=0;i<(m_oCVPV10.rows*m_oCVPV10.cols);i++)
				{
					Byte bData = 0;
					if(m_oCVPV10.at<float>(i) >= m_fPV10HiValue_Thresh_nm)
					{
						bData |= 0x2;
					}
					if(m_oCVPV2.at<float>(i) >= m_fPV2HiValue_Thresh_nm)
					{
						bData |= 0x1;
					}
					pIn[i] = bData;
				}


				uLong comprLen = nInLen;
				Byte  *compr = new Byte[nInLen];
				int err = compress(compr, &comprLen, pIn, nInLen);

				pHfStream->Write(&comprLen,sizeof(uLong),&nBytesWritten);
				pHfStream->Write(compr,comprLen * sizeof(Byte),&nBytesWritten);

				delete[]compr;
			}
			else
			{
				for(int i=0;i<(m_oCVPV10.rows*m_oCVPV10.cols);i++)
				{
					BYTE bData = 0;
					if(m_oCVPV10.at<float>(i) >= m_fPV10HiValue_Thresh_nm)
					{
						bData |= 0x2;
					}
					if(m_oCVPV2.at<float>(i) >= m_fPV2HiValue_Thresh_nm)
					{
						bData |= 0x1;
					}
					pHfStream->Write(&bData,sizeof(BYTE),&nBytesWritten);	
				}
			}

			
			pHfStream->Release(); pHfStream = NULL;
		}


		// save 
		pHfStg->Release(); pHfStg = NULL;

	}
	catch( const WCHAR *pwszError )
	{
		CString csMsgerr(pwszError);
		LogThis(1,4,Fmt(_T("Error: %s (HR = %08x)"),csMsgerr,hr));
	}

	if( NULL != pHfStg )
		pHfStg->Release();
	if( NULL != pCurvePV2Stg )
		pCurvePV2Stg->Release();
	if( NULL != pCurvePV10Stg )
		pCurvePV10Stg->Release();
	if( NULL != pRootStg )
		pRootStg->Release();

	delete [] wszADNTempName;

}


bool CTreatGenerateResults0::SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
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

bool CTreatGenerateResults0::SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin /*= INT_MAX*/, int p_nMax /*= INT_MAX*/, bool bAutoscale /*= true*/)
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
		if(!bUseMinPrm)
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

UINT CTreatGenerateResults0::SaveData( void *p_pParameters )
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
				csFileName = Fmt(_T("%s\\%s_%s_%d.png"), sGenPath, csTreatName, elt._cs, nId);
				float fMax = FLT_MAX;
				float fMin = fMax;
				if (!elt._bAutoScale)
				{
					fMax = elt._fMax;
					fMin = elt._fMin;
				}
				if(! SaveGreyImageFlt32(csFileName,elt._spT,fMin,fMax,elt._bAutoScale))
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

HRESULT CTreatGenerateResults0::QueryInterface( REFIID iid, void **ppvObject )
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

ULONG CTreatGenerateResults0::AddRef( void )
{
	m_ulRefCount++;
	return m_ulRefCount;
}

ULONG CTreatGenerateResults0::Release( void )
{
	m_ulRefCount--;
	if (m_ulRefCount!=0) 
		return m_ulRefCount;
	delete this;     // Destruction de l'objet.
	return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}

void CTreatGenerateResults0::ComputeSitePV( CString p_csOutPath )
{
	int nSizeX_PVspec = (int) (m_fPixelSize * m_fSiteWidthmm * 1000.f); // 25mm
	int nSizeY_PVspec = (int) (m_fPixelSize * m_fSiteHeightmm * 1000.f); // 8mm
	int nOffSetX =  (int) (m_fPixelSize * m_fSiteOffsetXmm * 1000.f);
	int nOffSetY =  (int) (m_fPixelSize * m_fSiteOffsetYmm * 1000.f);

	// decoupe de la zone wafer

	Mat oSPVColorImg =  Mat::ones(m_oCVHf.rows,m_oCVHf.cols,CV_8U) * 192;
	cvtColor(oSPVColorImg,oSPVColorImg, CV_GRAY2BGR);

	int x,y;
	int nHalfnrsz_X = nSizeX_PVspec/2;
	int nHalfnrsz_Y = nSizeY_PVspec/2;
	float fmskLimit = ((nSizeX_PVspec * nSizeY_PVspec) - 1) * m_fSiteLimitsFactor;
	//LogThis(1,1,Fmt(_T("---------------------> limit msk SITE PV %0.2lf/%d"),fmskLimit, nSizeX_PVspec * nSizeY_PVspec));
	vector< vector <tPVSiteData> > vSitePV;
	for(y = nHalfnrsz_Y+nOffSetY; y<m_oCVHf.rows-nHalfnrsz_Y; y = y + nSizeY_PVspec )
	{
		if (m_bEmergencyStop)
			continue ;

		if( (y- nHalfnrsz_Y) < 0)
			continue;
		if( (y - nHalfnrsz_Y + nSizeY_PVspec) >= m_oCVHf.rows)
			break;

		tPVSiteData oSiteData;
		vector <tPVSiteData> vSPV_X;
		for(x = nHalfnrsz_X+nOffSetX; x<m_oCVHf.cols-nHalfnrsz_X; x = x + nSizeX_PVspec)
		{
			if( (x - nHalfnrsz_X) < 0)
				continue;
			if( (x - nHalfnrsz_X + nSizeX_PVspec) >= m_oCVHf.cols)
				break;
			
			oSiteData.x=x;
			oSiteData.y=y;

			Rect nRcAreaROI = Rect(x-nHalfnrsz_X,y-nHalfnrsz_Y,nSizeX_PVspec,nSizeY_PVspec);
			Mat msk;
			msk = m_oCVMatMaskE(nRcAreaROI);
			Scalar fsum = sum(msk);
			float fdiffnm = 255.0f;
			if(fsum[0] > fmskLimit) // si on a suffisamment d'info sur le bord sinon on reste à 0
			{
				double dmin, dmax;
				minMaxLoc(m_oCVHf(nRcAreaROI),&dmin,&dmax,nullptr,nullptr,msk);
				fdiffnm = static_cast<float>(dmax-dmin) ;
			}

			if(fdiffnm == 255.0f)
			{
				rectangle(oSPVColorImg, nRcAreaROI, Scalar(192,192,192), CV_FILLED);
			}
			else if (fdiffnm >= m_fSiteThresh2nm)
			{
				rectangle(oSPVColorImg, nRcAreaROI, Scalar(0,0,255), CV_FILLED);
			}
			else if (fdiffnm >= m_fSiteThresh1nm)
			{
				rectangle(oSPVColorImg, nRcAreaROI, Scalar(0,255,255), CV_FILLED);
			}
			else
			{
				rectangle(oSPVColorImg, nRcAreaROI, Scalar(0,255,0), CV_FILLED);
			}
			rectangle(oSPVColorImg, nRcAreaROI, Scalar(255,255,255));
			oSiteData.pv = fdiffnm;
			vSPV_X.push_back(oSiteData);
		}
		vSitePV.push_back(vSPV_X);
	}

	// Use mask to draw wafer circle
	Mat mmmwf;
	cvtColor(m_oCVMatMaskE,mmmwf, CV_GRAY2BGR);
	oSPVColorImg = mmmwf.mul(oSPVColorImg);

	// we need to execute a second pass after circle wafer to add labels on SitesPv boxes
	// add value in boxes
	int adjX = (int)42.0 * m_fSiteTxtFactor;
	int adjY = (int)25 * m_fSiteTxtFactor;
	for (y=0; y<vSitePV.size(); y++) 
	{
		for (x=0; x< (vSitePV[y]).size(); x++)
		{
			
			float fdiffnm =  (vSitePV[y])[x].pv;
			//LogThis(1,1,Fmt(_T("---- TEXT org [ %d %d] %lf"),nCountX-nHalfnrsz_X,nCountY-nHalfnrsz_Y,fdiffnm));
			if(fdiffnm != 255.0)
			{
				std::string stxt;
				stxt = Fmt("%0.1lf",fdiffnm);
				int nCountX = (vSitePV[y])[x].x;
				int nCountY = (vSitePV[y])[x].y;
				putText(oSPVColorImg, stxt, cvPoint(nCountX-adjX,nCountY+adjY), FONT_HERSHEY_COMPLEX_SMALL, 2.0 * m_fSiteTxtFactor, Scalar(90,90,90), 4 * m_fSiteTxtFactor, CV_AA);
			}
		}
	}
	// Add legend
	rectangle(oSPVColorImg, Rect( 80,  80, 40,40), Scalar(0,0,255), CV_FILLED); 
	CString cstxt;
	cstxt.Format(">= %0.1lf nm",m_fSiteThresh2nm);
	putText(oSPVColorImg, (LPCTSTR)cstxt, cvPoint(140,80+25), FONT_HERSHEY_COMPLEX_SMALL, 2.2, Scalar(255,255,255),4, CV_AA);
	rectangle(oSPVColorImg, Rect( 80, 130, 40,40), Scalar(0,255,255), CV_FILLED);
	cstxt.Format("< %0.1lf nm",m_fSiteThresh2nm);
	putText(oSPVColorImg, (LPCTSTR)cstxt, cvPoint(140,130+25), FONT_HERSHEY_COMPLEX_SMALL, 2.2, Scalar(255,255,255),4, CV_AA);
	rectangle(oSPVColorImg, Rect( 80, 180, 40,40), Scalar(0,255,0), CV_FILLED);
	cstxt.Format("< %0.1lf nm",m_fSiteThresh1nm);
	putText(oSPVColorImg, (LPCTSTR)cstxt, cvPoint(140,180+25), FONT_HERSHEY_COMPLEX_SMALL, 2.2, Scalar(255,255,255),4, CV_AA);

	// Save image
	CString csFilePath = p_csOutPath + _T("\\");
	CString csPrefix;
	csPrefix.Format("LotID-%s-SITEPV.png",m_csLotID);
	csFilePath +=csPrefix;
	imwrite((LPCTSTR)csFilePath,oSPVColorImg);

}

extern "C"  HRESULT Create_TreatGenerateResults0( REFIID iid, void **ppvObject )
{
	CTreatGenerateResults0 *pObj = new CTreatGenerateResults0();
	if (pObj==0) 
		return E_OUTOFMEMORY;
	return pObj->QueryInterface(iid, ppvObject);
}



// IBE: temp to accelerate NanoTopo
// The true function Exec is at line 196
bool CTreatGenerateResults0::Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm)
{
    double dstart = GetPerfTime();

    m_MaskE.reset(new  H3_MATRIX_UINT8());
    m_Hf.reset(new  H3_MATRIX_FLT32());

    DWORD dwEvent = 0;
    void* p = 0;
    int nSaveData = 0;
    unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag;
    if (FindTreatPrmPtr(p_InputsPrm, "Save", p))
    {
        nSaveData = *((int*)p);
    }
    tData2Save* pSavData = nSaveData ? new tData2Save : nullptr; // SHOULD be deleted in this::SaveData	or any error exit or exception

    if (FindTreatPrmPtr(p_InputsPrm, "LotID", p))
    {
        m_csLotID = *((CString*)p);
    }

    if (FindTreatPrmPtr(p_InputsPrm, "FoupID", p))
    {
        m_csFoupID = *((CString*)p);
    }

    if (FindTreatPrmPtr(p_InputsPrm, "OffsetExpand_X", p))
    {
        m_nOffsetExpandX = *((int*)p);
    }
    if (FindTreatPrmPtr(p_InputsPrm, "OffsetExpand_Y", p))
    {
        m_nOffsetExpandY = *((int*)p);
    }
    if (FindTreatPrmPtr(p_InputsPrm, "FilterType", p))
    {
        m_nFilterType = *((int*)p);
    }

    shared_ptr<void> pvMaskE;
    if (FindTreatPrmSharedPtr(p_InputsPrm, "MaskE", pvMaskE))
    {
        m_MaskE = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskE);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("MaskE Size = %d x %d"), m_MaskE->GetCo(), m_MaskE->GetLi()));
    }
    pvMaskE.reset();
    ReduceMatrixPadArray(*m_MaskE.get(), m_nOffsetExpandX, m_nOffsetExpandY);
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("Retreive original size => %d x %d"), m_MaskE->GetCo(), m_MaskE->GetLi()));

    shared_ptr<void> pvH;
    if (FindTreatPrmSharedPtr(p_InputsPrm, "Hf", pvH))
    {
        m_Hf = static_pointer_cast<H3_MATRIX_FLT32> (pvH);
        if (uDbgFlag & DBG_SHOWDEBUG_LOG)
            LogThis(1, 1, Fmt(_T("Hf Size = %d x %d"), m_Hf->GetCo(), m_Hf->GetLi()));
    }
    pvH.reset();
    ReduceMatrixPadArray(*m_Hf.get(), m_nOffsetExpandX, m_nOffsetExpandY);
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
        LogThis(1, 1, Fmt(_T("Retreive original size => %d x %d"), m_Hf->GetCo(), m_Hf->GetLi()));

    m_oCVHf = Mat(m_Hf->GetLi(), m_Hf->GetCo(), CV_32F, m_Hf->GetData(), Mat::AUTO_STEP);

    m_oCVHf2sav = m_oCVHf.clone();

    double dssStart = 0.0;
    if (uDbgFlag & DBG_SHOWDEBUG_LOG)
    {
        LogThis(1, 1, Fmt(_T("##### Start Generate Results")));
        dssStart = GetPerfTime();
    }

    // lissage
    if (m_nLissageSize > 0)
    {
        Mat Kernel = Mat::ones(m_nLissageSize, m_nLissageSize, CV_32F) / (m_nLissageSize * m_nLissageSize);
        filter2D(m_oCVHf, m_oCVHf, CV_32F, Kernel);
        Kernel.release();

        if (pSavData && (uDbgFlag & DBG_REC_FILTERLISS))
        {
            tSpT<H3_MATRIX_FLT32> elt;
            elt._cs = _T("FilterLiss");
            elt._spT.reset(new H3_MATRIX_FLT32(*m_Hf.get())); // copy since data altered in following treatments
            elt._bImg = true;
            elt._bAutoScale = true;
            elt._bHbf = false;
            elt._bBin = true;
            pSavData->spListF32.push_back(elt);
        }
    }

    if (m_bEmergencyStop)
    {
        LogThis(1, 3, Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
        return false;
    }

    // on ronge encore un peu les bords
    m_oCVMatMaskE = Mat(m_MaskE->GetLi(), m_MaskE->GetCo(), CV_8U, m_MaskE->GetData(), Mat::AUTO_STEP);
    if (m_nPrmErodeRadius > 0)
    {
        Mat elementErode = getStructuringElement(MORPH_ELLIPSE, Size(2 * m_nPrmErodeRadius, 2 * m_nPrmErodeRadius), Point(m_nPrmErodeRadius, m_nPrmErodeRadius));
        erode(m_oCVMatMaskE, m_oCVMatMaskE, elementErode);
        elementErode.release();

        if (pSavData && (uDbgFlag & DBG_REC_ERODE))
        {
            tSpT<H3_MATRIX_UINT8> elt;
            elt._cs = _T("ResErode");
            elt._spT.reset(new H3_MATRIX_UINT8(*m_MaskE.get())); // copy since data altered in following treatments
            elt._bImg = true;
            elt._bAutoScale = true;
            elt._bHbf = false;
            elt._bBin = false;
            pSavData->spListU8.push_back(elt);
        }
    }

    if (m_bEmergencyStop)
    {
        LogThis(1, 3, Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
        return false;
    }

    Mat oMskE32f;
    m_oCVMatMaskE.convertTo(oMskE32f, CV_32F);
    m_oCVHf = oMskE32f.mul(m_oCVHf);

    CString csOutPath = _T("C:\\Altasight\\Nano\\Data");
    void* pcs;
    if (FindTreatPrmPtr(p_InputsPrm, "OutPath", pcs))
    {
        csOutPath = *((CString*)pcs);
    }
    SaveADNFile(csOutPath, m_bUseDataCompression);

    m_MaskE.reset();
    m_Hf.reset();

    return true;
}