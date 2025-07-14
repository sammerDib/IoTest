#include "StdAfx.h"
#include "TreatPrepareData0.h"

//#include "H3MatrixOperations.h"

#include <opencv2/core/core.hpp>
//#include <opencv2/highgui/highgui.hpp>
#include "opencv2/imgproc/imgproc.hpp"
using namespace cv;
//using namespace std; 

#include "fftw3.h"
#include <complex>

#include <omp.h> // for openMP directive //=> check also that /openMP is active in compiler properties

//#define FIP_EXPORTS
//#define _WIN32
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

template<class T> void fftShift(T *out, const T* in, size_t nx, size_t ny)
{
	const size_t hlen1 = (ny+1)/2;
	const size_t hlen2 = ny/2;
	const size_t shft1 = ((nx+1)/2)*ny + hlen1;
	const size_t shft2 = (nx/2)*ny + hlen2;

	const T* src = in;
	for(T* tgt = out; tgt < out + shft1 - hlen1; tgt += ny, src += ny) { // (nx+1)/2 times
		copy(src, src+hlen1, tgt + shft2);          //1->4
		copy(src+hlen1, src+ny, tgt+shft2-hlen2); } //2->3
	src = in;
	for(T* tgt = out; tgt < out + shft2 - hlen2; tgt += ny, src += ny ){ // nx/2 times
		copy(src+shft1, src+shft1+hlen2, tgt);         //4->1
		copy(src+shft1-hlen1, src+shft1, tgt+hlen2); } //3->2
};

CTreatPrepareData0::CTreatPrepareData0()
{	
	InitializeCriticalSection(&m_sCriticalSection);
	for(int i =0; i<2; i++)
	{
		m_hEventThDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventThDone[i] != 0);	
	}

	m_nPrmErodeRadius	= 0; //25
	m_nPrmDilateRadius	= 0; //50
	m_bUseFringeKiller	= false;
	m_uDbgFlag = 0;
	m_uRegFlag = 0;
}

CTreatPrepareData0::~CTreatPrepareData0()
{
	for(int i =0; i<2; i++)
	{
		if (m_hEventThDone[i] != 0)
		{
			CloseHandle(m_hEventThDone[i]);
			m_hEventThDone[i] = 0;
		}
	}
	DeleteCriticalSection(&m_sCriticalSection);
}

bool CTreatPrepareData0::Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder)
{ 	

	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("ErodeRadius"), m_nPrmErodeRadius))
		LogThis(1,3,Fmt(_T("{%s} Could not find [ErodeRadius] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("DilateRadius"), m_nPrmDilateRadius))
		LogThis(1,3,Fmt(_T("{%s} Could not find [DilateRadius] Parameter"), INanoTopoTreament::GetName()));
	
	int  nBool = 0;
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("UseFringeKiller"),nBool))
		LogThis(1,3,Fmt(_T("{%s} Could not find [UseFringeKiller] Parameter"), INanoTopoTreament::GetName()));
	m_bUseFringeKiller = (nBool != 0);

	m_uRegFlag = 0;
	if( ! GetRegistryFlag(m_uRegFlag, DBG_REC_NX | DBG_REC_NY))
		LogThis(1,3,Fmt(_T("{%s} Could not reach Registry flag Parameter"), INanoTopoTreament::GetName()));
	
	return true;
}

bool CTreatPrepareData0::Exec( const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm )
{
	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	double dstart = GetPerfTime();

	m_Erode.reset(new  H3_MATRIX_UINT8());
	m_Dilate.reset(new  H3_MATRIX_UINT8());
	m_SE.reset(new  H3_MATRIX_UINT8());

	void *p = 0;
	int nSaveData = 0;
	unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag; 
	int i, j, nLines, nCols;
	
	if(FindTreatPrmPtr(p_InputsPrm,"Save",p))
	{
		nSaveData = *((int*) p); 	
	}
	tData2Save* pSavData = nSaveData ? new tData2Save : nullptr; // SHOULD be deleted in CTreatPrhepareData0::SaveData	or any error exit or exception

	if(FindTreatPrmPtr(p_InputsPrm,"LotID",p))
	{
		m_csLotID = *((CString*) p); 	
	}


	shared_ptr<void> pvMask;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"Mask",pvMask))
	{
		m_matMask = static_pointer_cast<H3_MATRIX_UINT8> (pvMask);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("Mask Size = %d x %d"), m_matMask->GetCo(),m_matMask->GetLi()));
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
	shared_ptr<void> pvPX;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"PX",pvPX))
	{
		m_matPX = static_pointer_cast<H3_MATRIX_FLT32> (pvPX);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("PX Size = %d x %d"), m_matPX->GetCo(),m_matPX->GetLi()));
	}

	shared_ptr<void> pvPY;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"PY",pvPY))
	{
		m_matPY = static_pointer_cast<H3_MATRIX_FLT32> (pvPY);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("PY Size = %d x %d"), m_matPY->GetCo(),m_matPY->GetLi()));
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

	//
	// Enlarge Zone ? 
	//
	
	//
	// Erode mask (launch threads by quadrant and wait after to complete NX and NY)
	//

	if(pSavData && (uDbgFlag & DBG_REC_MASK))
	{
		pSavData->spMask.reset(new H3_MATRIX_UINT8(*m_matMask.get())); // copy since data altered in following treatments
	}

	m_Erode->Copy(*(m_matMask.get()));
	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}
	
	double dssErode = 0.0;
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		LogThis(1,1,Fmt(_T("##### Start Mask MultiThreaded Erode")));
		dssErode = GetPerfTime();
	}

	CWinThread* pThreadErode = AfxBeginThread(&CTreatPrepareData0::static_ErodeCV, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadErode->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadErode->ResumeThread();

	//
	// perform differences on PX by column and differences on PY  by line
	//
	double dssMat = 0.0;
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		LogThis(1,1,Fmt(_T("##### Start Diff and etc")));
		dssMat = GetPerfTime();
	}

	m_matPX->DiffByColumns();
	m_matPY->DiffByLines();

	nLines	= m_matPX->GetLi();
	nCols	= m_matPX->GetCo();
	decltype(m_matPX->GetData()) pDataPx = m_matPX->GetData();
	decltype(m_matPY->GetData()) pDataPy = m_matPY->GetData();
	decltype(m_matNX->GetData()) pDataNx = m_matNX->GetData();
	decltype(m_matNY->GetData()) pDataNy = m_matNY->GetData();
	decltype(m_matPY->GetAt(0,0)) dMillions =  1000000.0;
	
	#pragma omp parallel for 
	for(long lItem = 0; lItem < nLines * nCols; lItem++)
 	{
			pDataNx[lItem] =  abs(pDataPx[lItem]) * pDataNx[lItem] * dMillions;
			pDataNy[lItem] =  abs(pDataPy[lItem]) * pDataNy[lItem] * dMillions;
			// Nan to Zero for NX & NY matrices (avoid another loops)
			if(_isnanf(pDataNx[lItem]))
				pDataNx[lItem] = 0;
			if(_isnanf(pDataNy[lItem]))
				pDataNy[lItem] = 0;
	}

	// PX & PY are no Longer used delete it
	pDataPx = pDataPy = 0; // to avoid using this pointer by mistake
	m_matPX.reset(); // release PX matrix memory
	m_matPY.reset(); // release PY matrix memory
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done Diff and etc in %lf ms"),GetPerfTime() - dssMat));

	// Wait Erode Thread to be completed
	DWORD dwEvent = WaitForSingleObject( m_hEventThDone[0],  INFINITE);
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		double dssEndErode = GetPerfTime();
		LogThis(1,1,Fmt(_T("# Done Erode in %lf ms"), dssEndErode - dssErode));
	}
	m_matMask.reset();

	if(pSavData && (m_uDbgFlag&DBG_REC_ERODE))
	{
		pSavData->spMaskE.reset(new H3_MATRIX_UINT8(*m_Erode.get())); // copy since data altered in following treatments
	}

	// 
	// Dilate
	//

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	m_Dilate->Copy(*(m_Erode.get()));

	double dssDilate = 0.0;
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		LogThis(1,1,Fmt(_T("##### Start Mask MultiThreaded Dilate")));
		dssDilate = GetPerfTime();
	}
	CWinThread* pThreadDilate = AfxBeginThread(&CTreatPrepareData0::static_DilateCV, this, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadDilate->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadDilate->ResumeThread();

	nLines	= m_matNX->GetLi();
	nCols	= m_matNY->GetCo();

	// we assume here mask erode has the same size than NX and NY
	// Apply mask on NX && NY and search for min

	float fMinNX = FLT_MAX;
	float fMinNY = FLT_MAX;

	#pragma omp parallel for private(j)
	for(i = 0; i < nLines; i++)
	{
		for(j = 0; j<nCols; j++)
		{
			long lItem= i*nCols+j;
 			pDataNx[lItem] *= m_Erode->GetAt(i,j);
 			pDataNy[lItem] *= m_Erode->GetAt(i,j);
			fMinNX = __min( fMinNX, pDataNx[lItem]);
			fMinNY = __min( fMinNY, pDataNy[lItem]);
		}
	}

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	//
	// Fringe Killer (using fftw 3.3)
	//

	if(m_bUseFringeKiller)
	{
		LogThis(1,1,Fmt(_T("##### Start FringeKillerNX")));
		double dssX = GetPerfTime();

		int a = 20, b1 = 80, b2 =250, nHThX=200;
		if( FringeKillerNX(fMinNX,a,b1,b2,nHThX) == false)
			return false;
		LogThis(1,1,Fmt(_T("# Done FringeKillerNX in %lf ms"),GetPerfTime() - dssX));

		LogThis(1,1,Fmt(_T("##### Start FringeKillerNY")));
		double dssY = GetPerfTime();

		int a2 = 300, a1 = 150, b = 3, nHThY=200;
		if( FringeKillerNY(fMinNY,a1,a2,b2,nHThY) == false)
			return false;
		LogThis(1,1,Fmt(_T("# Done FringeKillerNY in %lf ms"),GetPerfTime() - dssY));
		
		if(nSaveData && (uDbgFlag & DBG_REC_FFTKILLER))
		{
			bool  bSave;
			bSave = SaveGreyImageFlt32(_T("C:\\Altasight\\Nano\\Data\\FloatNX-AfterFringe.bmp"),m_matNX);
			bSave = SaveGreyImageFlt32(_T("C:\\Altasight\\Nano\\Data\\FloatNY-AfterFringe.bmp"),m_matNY);
		}
	}
	
	// Wait Dilate Threads to be completed
	dwEvent = WaitForSingleObject( m_hEventThDone[1],  INFINITE);
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done Dilate in %lf ms"),GetPerfTime() - dssDilate));

	if(pSavData && (uDbgFlag&DBG_REC_DILATE))
	{
		pSavData->spMaskD.reset(new H3_MATRIX_UINT8(*m_Dilate.get())); // copy since data altered in following treatments
	}

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
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

		if(uDbgFlag & DBG_REC_NX)
			pSavData->spNX.reset(new H3_MATRIX_FLT32(*m_matNX.get())); // copy since data altered in following treatments
		if(uDbgFlag & DBG_REC_NY)
			pSavData->spNY.reset(new H3_MATRIX_FLT32(*m_matNY.get())); // copy since data altered in following treatments

		CWinThread* pThread = AfxBeginThread(&CTreatPrepareData0::SaveData, pSavData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
		if (pThread == 0)
		{
			LogThis(1,4,Fmt(_T("(%s) AfxBeginThread() failed.\n"), INanoTopoTreament::GetName()));
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

	AddTreatPrmSharedPtr(p_OutputsPrm,"MaskE",shared_ptr<void>(m_Erode));
	AddTreatPrmSharedPtr(p_OutputsPrm,"MaskD",shared_ptr<void>(m_Dilate));

	return true;
}

bool CTreatPrepareData0::FringeKillerNX( double p_dMin, int p_na, int p_nb1, int p_nb2, int p_nHThresh )
{
	// using FFTW 3 Library

	int i, j;
	int nx = m_matNX->GetCo();
	ASSERT(nx > 0);
	int ny = m_matNX->GetLi();
	ASSERT(ny > 0);
	int nx2 = nx/2;
	int ny2 = ny/2;
	
	bool bCheckPrm = false;
	VERIFY( bCheckPrm = ((nx2 - p_na) >= 0 && (nx2 + p_na) < nx ) );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNX : Wrong parameter range : p_na (%d)"),p_na));
		return false;
	}
	VERIFY( bCheckPrm = ((ny2 - p_nb1) >= 0 && (ny2 + p_nb1) < ny && (ny2 + p_nb1) >= 0 && (ny2 - p_nb1) < ny ) );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNX : Wrong parameter range : p_nb1 (%d)"),p_nb1));
		return false;
	}
	VERIFY( bCheckPrm = ((ny2 - p_nb2) >= 0 && (ny2 + p_nb2) < ny && (ny2 + p_nb2) >= 0 && (ny2 - p_nb2) < ny ) );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNX : Wrong parameter range : p_nb2 (%d)"),p_nb2));
		return false;
	}
	VERIFY( bCheckPrm = p_nb1 >=0 && p_nb2>=0 );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNX : Wrong parameters p_nb1 (%d) and p_nb2 (%d) should be positive"),p_nb1,p_nb2));
		return false;
	}
	VERIFY( bCheckPrm = p_nb1 < p_nb2);
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNX : Wrong parameters p_nb1 (%d) should be inferior to p_nb2 (%d)"),p_nb1,p_nb2));
		return false;
	}

	complex<float> * pFFT_NX = new complex<float> [nx * ny]; // FFT
	complex<float> * pFFTs_NX= new complex<float> [nx * ny]; // FFT recentrée / fftshifthée

	for ( i = 0; i < (nx * ny); i++ )
	{
		pFFTs_NX[i] = complex<float>(((float)(m_matNX->GetData()[i])) - p_dMin,0.0f);
	}

	// create FFT forward plan real to complex
	fftwf_plan oPlan_Fft2; 
	oPlan_Fft2 = fftwf_plan_dft_2d ( nx, ny, reinterpret_cast<fftwf_complex*>(pFFTs_NX), reinterpret_cast<fftwf_complex*>(pFFT_NX), FFTW_FORWARD,  FFTW_ESTIMATE );

	// perform FFT 2D
	fftwf_execute ( oPlan_Fft2 );

	// Shift - center Frequency
	fftShift< complex<float> >(pFFTs_NX,pFFT_NX,nx,ny);

	float* pH = new float [nx * ny] ;
	for ( i = 0; i < (nx * ny); i++ )
	{
		pH[i] = 20.0 * log(abs(pFFTs_NX[i]));
	}

	// filter fringe
	for ( i = nx2 - p_na; i <= nx2 + p_na; i++)
	{
		for ( j = ny2 - p_nb2; j <= nx2 - p_nb1 ; j++)
		{
			if(pH[i*ny+j] > p_nHThresh)
				pFFTs_NX[i*ny+j] = complex<float>(0.0,0.0);
		}
		for ( j = ny2 + p_nb1; j <= nx2 + p_nb2 ; j++)
		{
			if(pH[i*ny+j] > p_nHThresh)
				pFFTs_NX[i*ny+j] = complex<float>(0.0,0.0);
		}
	}
	// Free H matrix
	delete[] pH; 
	pH = 0;

	// return to the real world, create FFT backward plan complex to real
	fftwf_plan oPlan_InverseFft2; 
	oPlan_InverseFft2 = fftwf_plan_dft_2d ( nx, ny, reinterpret_cast<fftwf_complex*>(pFFTs_NX), reinterpret_cast<fftwf_complex*>(pFFT_NX), FFTW_BACKWARD,  FFTW_ESTIMATE );

	// perform inverse FFT 2D
	fftwf_execute ( oPlan_InverseFft2 );

	// reapply offset to NX matrix and perform conversion if needed
	for ( i = 0; i < (nx * ny); i++ )
	{
		m_matNX->GetData()[i] = (abs(pFFT_NX[i]) + p_dMin);
	}

	// free memory
	delete[] pFFTs_NX;
	pFFTs_NX = 0;
	delete[] pFFT_NX;
	pFFT_NX = 0;

	fftwf_destroy_plan ( oPlan_Fft2 );
	fftwf_destroy_plan ( oPlan_InverseFft2 );

	return true;
}

bool CTreatPrepareData0::FringeKillerNY( double p_dMin, int p_na1, int p_na2, int p_nb, int p_nHThresh )
{
	// using FFTW 3 Library

	int i, j;
	int nx = m_matNY->GetCo();
	ASSERT(nx > 0);
	int ny = m_matNY->GetLi();
	ASSERT(ny > 0);
	int nx2 = nx/2;
	int ny2 = ny/2;

	bool bCheckPrm = false;
	VERIFY( bCheckPrm = ((ny2 - p_nb) >= 0 && (ny2 + p_nb) < ny ) );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNY : Wrong parameter range : p_nb (%d)"),p_nb));
		return false;
	}
	VERIFY( bCheckPrm = ((nx2 - p_na1) >= 0 && (nx2 + p_na1) < nx && (nx2 + p_na1) >= 0 && (nx2 - p_na1) < nx ) );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNY : Wrong parameter range : p_na1 (%d)"),p_na1));
		return false;
	}
	VERIFY( bCheckPrm = ((nx2 - p_na2) >= 0 && (nx2 + p_na2) < nx && (nx2 + p_na2) >= 0 && (nx2 - p_na2) < nx ) );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNY : Wrong parameter range : p_na2 (%d)"),p_na2));
		return false;
	}
	VERIFY( bCheckPrm = p_na1 >=0 && p_na2>=0 );
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNY : Wrong parameters p_na1 (%d) and p_ab2 (%d) should be positive"),p_na1,p_na2));
		return false;
	}
	VERIFY( bCheckPrm = p_na1 < p_na2);
	if(! bCheckPrm)
	{
		LogThis(1,4,Fmt(_T("FringeKillerNY : Wrong parameters p_nb1 (%d) should be inferior to p_nb2 (%d)"),p_na1,p_na2));
		return false;
	}

	complex<double> * pFFT_NY = new complex<double> [nx * ny]; // FFT
	complex<double> * pFFTs_NY= new complex<double> [nx * ny]; // FFT recentrée / fftshifthée
	
	// on applique l'offsetMin et l'on converti en double (NYOff = NY - OffsetMin)
	for ( i = 0; i < (nx * ny); i++ )
	{
		pFFTs_NY[i] = complex<float>(((double)(m_matNY->GetData()[i])) - p_dMin,0.0f);
	}

	// create FFT forward plan real to complex
	fftw_plan oPlan_Fft2; 
	oPlan_Fft2 = fftw_plan_dft_2d ( nx, ny, reinterpret_cast<fftw_complex*>(pFFTs_NY), reinterpret_cast<fftw_complex*>(pFFT_NY), FFTW_FORWARD,  FFTW_ESTIMATE );

	// perform FFT 2D
	fftw_execute ( oPlan_Fft2 );

	// Shift - center Frequency
	fftShift< complex<double> >(pFFTs_NY,pFFT_NY,nx,ny);

	double* pH = new double [nx * ny] ;
	for ( i = 0; i < (nx * ny); i++ )
	{
		pH[i] = 20.0 * log(abs(pFFTs_NY[i]));
	}

	// filter fringe
	for ( j = ny2 - p_nb; j <= ny2 + p_nb ; j++)
	{
		for ( i = nx2 - p_na2; i <= nx2 - p_na1; i++)
		{
			if(pH[i*ny+j] > p_nHThresh)
				pFFTs_NY[i*ny+j] = complex<double>(0.0,0.0);
		}
		for ( i = nx2 + p_na1; i <= nx2 + p_na2; i++)
		{
			if(pH[i*ny+j] > p_nHThresh)
				pFFTs_NY[i*ny+j] = complex<double>(0.0,0.0);
		}
	}
	// Free H matrix
	delete[] pH; 
	pH = 0;

	// return to the real world, create FFT backward plan complex to real
	fftw_plan oPlan_InverseFft2; 
	oPlan_InverseFft2 = fftw_plan_dft_2d ( nx, ny, reinterpret_cast<fftw_complex*>(pFFTs_NY), reinterpret_cast<fftw_complex*>(pFFT_NY), FFTW_BACKWARD,  FFTW_ESTIMATE );

	// perform inverse FFT 2D
	fftw_execute ( oPlan_InverseFft2 );

	// reapply offset to NY matrix and perform conversion if needed
	for ( i = 0; i < (nx * ny); i++ )
	{
		m_matNY->GetData()[i] = (abs(pFFT_NY[i]) + p_dMin);
	}

	// free memory
	delete[] pFFTs_NY;
	pFFTs_NY = 0;
	delete[] pFFT_NY;
	pFFT_NY = 0;

	fftw_destroy_plan ( oPlan_Fft2 );
	fftw_destroy_plan ( oPlan_InverseFft2 );

	return true;
}


UINT CTreatPrepareData0::static_ErodeCV(void *p_pParameters)
{
	CTreatPrepareData0* pObj = static_cast<CTreatPrepareData0 *> (p_pParameters);
	pObj->ErodeCV();
	return 041;
}

void CTreatPrepareData0::ErodeCV()
{
	if(m_nPrmErodeRadius > 0)
	{
		Mat matErodecv = Mat(m_Erode->GetLi(), m_Erode->GetCo(), CV_8U, m_Erode->GetData(), Mat::AUTO_STEP);
		Mat elementErode = getStructuringElement( MORPH_ELLIPSE,	Size( 2*m_nPrmErodeRadius, 2*m_nPrmErodeRadius ),	Point( m_nPrmErodeRadius, m_nPrmErodeRadius ) );
		erode( matErodecv, matErodecv, elementErode );
	}
	SetEvent(m_hEventThDone[0]);
}

UINT CTreatPrepareData0::static_DilateCV(void *p_pParameters)
{
	CTreatPrepareData0* pObj = static_cast<CTreatPrepareData0 *> (p_pParameters);
	pObj->DilateCV();
	return 042;
}

void CTreatPrepareData0::DilateCV()
{
	if(m_nPrmDilateRadius > 0)
	{
		Mat matDilatecv = Mat(m_Dilate->GetLi(), m_Dilate->GetCo(), CV_8U, m_Dilate->GetData(), Mat::AUTO_STEP);
		Mat elementDilate = getStructuringElement( MORPH_ELLIPSE,	Size( 2*m_nPrmDilateRadius, 2*m_nPrmDilateRadius ),	Point( m_nPrmDilateRadius, m_nPrmDilateRadius ) );
		dilate( matDilatecv, matDilatecv, elementDilate );
	}
	SetEvent(m_hEventThDone[1]);
}

bool CTreatPrepareData0::SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
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
		BYTE* pbits = (BYTE*) oImg.getScanLine(y);
		for(unsigned x = 0; x < oImg.getWidth(); x++)
		{
			pbits[x] = saturate_cast<uchar>(pData[y*lCols+x] * a + b) ;
		}
	}
	oImg.flipVertical();
	BOOL bRes = oImg.save(p_csFilepath, 0);
	return (bRes !=0) ;
}

bool CTreatPrepareData0::SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin /*= INT_MAX*/, int p_nMax /*= INT_MAX*/, bool bAutoscale /*= true*/)
{
	unsigned char* pData = p_oMatrix->GetData();

	unsigned long  lCols	= p_oMatrix->GetCo();
	unsigned long  lLines	= p_oMatrix->GetLi();

	float fMin = INT_MAX;
	float fMax = - INT_MAX;

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
		fMin = p_nMin;
	}
	if (bUseMaxPrm)
	{
		fMax = p_nMax;
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

UINT CTreatPrepareData0::SaveData( void *p_pParameters )
{
	tData2Save* pData  = static_cast<tData2Save *>(p_pParameters);
	if(pData == nullptr)
		return 1;

	CString csFileName;

	UINT nId = pData->nId;
	CString csTreatName = pData->csName;
	CString sGenPath = pData->csPath;

	LogThis(1,1,Fmt(_T("(%s) ##### Start saving data = No %d"),csTreatName,nId));
	double dStart = GetPerfTime();
	shared_ptr<H3_MATRIX_FLT32> pNX = pData->spNX;
	shared_ptr<H3_MATRIX_FLT32> pNY = pData->spNY;
	shared_ptr<H3_MATRIX_UINT8> pMsk = pData->spMask;
	shared_ptr<H3_MATRIX_UINT8> pMskE = pData->spMaskE;
	shared_ptr<H3_MATRIX_UINT8> pMskD = pData->spMaskD;
	delete pData;
	pData = 0;

	// Assure Results Directory exist
	CreateDir(sGenPath);

	if(pNX)
	{
		csFileName = Fmt(_T("%s\\%s_NX_%d.png"), sGenPath, csTreatName, nId);
		if(! SaveGreyImageFlt32(csFileName,pNX))
			LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
		pNX.reset();
	}

	if(pNY)
	{
		csFileName = Fmt(_T("%s\\%s_NY_%d.png"), sGenPath, csTreatName, nId);
		if(! SaveGreyImageFlt32(csFileName,pNY))
			LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
		pNY.reset();
	}

	if(pMsk)
	{
		csFileName = Fmt(_T("%s\\%s_Mask0_%d.png"), sGenPath, csTreatName, nId);
		if(! SaveGreyImageUInt8(csFileName,pMsk,0,1,false))
			LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
		pMsk.reset();
	}

	if(pMskE)
	{
		csFileName = Fmt(_T("%s\\%s_MaskErode_%d.png"), sGenPath, csTreatName, nId);
		if(! SaveGreyImageUInt8(csFileName,pMskE,0,1,false))
			LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
		pMskE.reset();
	}

	if(pMskD)
	{
		csFileName = Fmt(_T("%s\\%s_MaskDilate_%d.png"), sGenPath, csTreatName, nId);
		if(! SaveGreyImageUInt8(csFileName,pMskD,0,1,false))
			LogThis(1,4,Fmt(_T("(%s) Fail to save <%s>  "), csTreatName, csFileName));
		pMskD.reset();
	}

	double dEnd = GetPerfTime();
	LogThis(1,2,Fmt(_T("(%s) ##### End saving data = No %d --- Exec in %lf"),csTreatName, nId, dEnd-dStart));

	return 0;
}

HRESULT CTreatPrepareData0::QueryInterface( REFIID iid, void **ppvObject )
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

ULONG CTreatPrepareData0::AddRef( void )
{
	m_ulRefCount++;
	return m_ulRefCount;
}

ULONG CTreatPrepareData0::Release( void )
{
	m_ulRefCount--;
	if (m_ulRefCount!=0) 
		return m_ulRefCount;
	delete this;     // Destruction de l'objet.
	return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}


extern "C"  HRESULT Create_TreatPrepareData0( REFIID iid, void **ppvObject )
{
	CTreatPrepareData0 *pObj = new CTreatPrepareData0();
	if (pObj==0) 
		return E_OUTOFMEMORY;
	return pObj->QueryInterface(iid, ppvObject);
}


