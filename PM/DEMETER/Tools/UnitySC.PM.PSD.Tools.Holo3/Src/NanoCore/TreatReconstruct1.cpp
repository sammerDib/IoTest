#include "StdAfx.h"
#include "TreatReconstruct1.h"

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

#include <omp.h> // for openMP directive //=> check also that /openMP is active in compiler properties

#include <Eigen/Sparse>
#include <Eigen/SparseCholesky>
using namespace Eigen;

const double	g_dEPSILON = std::numeric_limits<double>::epsilon();
const float		g_fEPSILON = std::numeric_limits<float>::epsilon();
const float     g_fMinZeroSub = 10.f;
const  double	g_dPI=3.14159265358979;

#define	DBG_REC_COEFMAP				0X00000010
#define DBG_REC_H					0X00000020
#define DBG_REC_VIGNETTE			0x00000040
#define DBG_SKIP_AFFINETRANSFORM	0x00000080
#define DBG_SKIP_BANDTHREAD			0x00000100
#define DBG_SKIP_QUADTHREAD			0x00000200	


typedef enum 
{
	eTopMid = 0,// voisinage haut milieu des vignettes
	eTopR,		// voisinage haut droit des vignettes
	eMidR,		// voisinage droite milieu des vignettes
	eBotR,		// voisinage bas droit des vignettes
	eBotMid,	// voisinage bas milieu des vignettes
	eBotL,		// voisinage bas gauche des vignettes
	eMidL,		// voisinage gauche milieu des vignettes
	eTopL		// voisinage haut gauche des vignettes
}eVoisinageType;

struct tPrmThreadRecollage
{
	CTreatReconstruct1* _ptr;
	int		_nMaxI;
	int		_nMaxJ;
	int		_nRefI;
	int		_nRefJ;

	vector< vector<tVignetteRes> >* _pGridZ;
};

CTreatReconstruct1::CTreatReconstruct1()
{	
	InitializeCriticalSection(&m_sCriticalSection);
	for(int i =0; i<NB_QUADRANT; i++)
	{
		m_hEventQDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventQDone[i] != 0);	

		m_hEventQStart[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventQStart[i] != 0);	
	}
	for(int i =0; i<2; i++)
	{
		m_hEventBandDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventBandDone[i] != 0);	

		m_hEventHalfBandDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventHalfBandDone[i] != 0);	
	}

	m_lVignetteSize = 128;  // si m_lVignette = 128 
	m_nMargin		= 0;	// rab
	m_nVois			= 10;	// voisinage calcule de Z, cf compute vignette
	m_dSigma		= 0.5;	// en expert
	m_dAlpha		= 0.1;	// en expert
	m_uRegFlag		= 0;

	/* initialize random seed: */
	srand ( time(NULL) );

}

CTreatReconstruct1::~CTreatReconstruct1()
{
	for(int i =0; i<4; i++)
	{
		if (m_hEventQDone[i] != 0)
		{
			CloseHandle(m_hEventQDone[i]);
			m_hEventQDone[i] = 0;
		}

		if (m_hEventQStart[i] != 0)
		{
			CloseHandle(m_hEventQStart[i]);
			m_hEventQStart[i] = 0;
		}
	}

	for(int i =0; i<2; i++)
	{
		if (m_hEventBandDone[i] != 0)
		{
			CloseHandle(m_hEventBandDone[i]);
			m_hEventBandDone[i] = 0;
		}

		if (m_hEventHalfBandDone[i] != 0)
		{
			CloseHandle(m_hEventHalfBandDone[i]);
			m_hEventHalfBandDone[i] = 0;
		}
	}
	DeleteCriticalSection(&m_sCriticalSection);
}

bool CTreatReconstruct1::Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder)
{
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

bool CTreatReconstruct1::Exec( const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm )
{
	double dstart = GetPerfTime();

	m_matMaskDilate.reset(new H3_MATRIX_UINT8());
	m_matMaskErode.reset(new H3_MATRIX_UINT8());
	m_CoefMat.reset(new  H3_MATRIX_FLT32());

	DWORD dwEvent = 0;
	void *p = 0;
	unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag; 
	int nSaveData = 0;
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
	pvMaskDilate.reset();

	shared_ptr<void> pvMaskErode;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"MaskE",pvMaskErode))
	{
		m_matMaskErode = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskErode);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("Mask Erode Size = %d x %d"), m_matMaskErode->GetCo(),m_matMaskErode->GetLi()));
	}
	pvMaskErode.reset();

	shared_ptr<void> pvNX;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"NX",pvNX))
	{
		m_matNX = static_pointer_cast<H3_MATRIX_FLT32> (pvNX);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("NX Size = %d x %d"), m_matNX->GetCo(),m_matNX->GetLi()));
	}
	pvNX.reset();

	shared_ptr<void> pvNY;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"NY",pvNY))
	{
		m_matNY = static_pointer_cast<H3_MATRIX_FLT32> (pvNY);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("NY Size = %d x %d"), m_matNY->GetCo(),m_matNY->GetLi()));
	}
	pvNY.reset();

	// we assume here that all matrices has the same size
	ASSERT(m_matNX->GetCo() == m_matNY->GetCo());
	ASSERT(m_matNX->GetLi() == m_matNY->GetLi());
	ASSERT(m_matNX->GetCo() == m_matMaskDilate->GetCo());
	ASSERT(m_matNX->GetLi() == m_matMaskDilate->GetLi());
	ASSERT(m_matMaskErode->GetCo() == m_matMaskDilate->GetCo());
	ASSERT(m_matMaskErode->GetLi() == m_matMaskDilate->GetLi());
 
	m_lOriginalWidth_px		= m_matNX->GetCo();
	m_lOriginalHeight_px	= m_matNX->GetLi();

	// voisinage haut milieu des vignettes
	/*m_nVoisPos[eTopMid][0] = (m_lVignetteSize - m_nVois) / 2; 
	m_nVoisPos[eTopMid][1] = m_lVignetteSize / 4 ; 
	// voisinage haut droit des vignettes
	m_nVoisPos[eTopR][0] = 3 * m_lVignetteSize / 4;	// ie r2+r4 ie r-r4
	m_nVoisPos[eTopR][1] = m_lVignetteSize / 4;		// ie r2-r4 ie r4
	// voisinage milieu droite  des vignettes
	m_nVoisPos[eMidR][0] = 3 * m_lVignetteSize / 4; 
	m_nVoisPos[eMidR][1] = (m_lVignetteSize - m_nVois) / 2;
	// voisinage bas droit des vignettes
	m_nVoisPos[eBotR][0] = 3 * m_lVignetteSize / 4;	// ie r2+r4 ie r-r4
	m_nVoisPos[eBotR][1] = 3 * m_lVignetteSize / 4;  // ie r2+r4 ie r-r4
	// voisinage bas milieu des vignettes
	m_nVoisPos[eBotMid][0] = (m_lVignetteSize - m_nVois) / 2; 
	m_nVoisPos[eBotMid][1] = 3 * m_lVignetteSize / 4 ; 
	// voisinage bas gauche des vignettes
	m_nVoisPos[eBotL][0] = m_lVignetteSize / 4;		// ie r2-r4 ie r4
	m_nVoisPos[eBotL][1] = 3 * m_lVignetteSize / 4;	// ie r2+r4 ie r-r4
	// voisinage gauche milieu des vignettes
	m_nVoisPos[eMidL][0] =  m_lVignetteSize / 4; 
	m_nVoisPos[eMidL][1] = (m_lVignetteSize - m_nVois) / 2;
	// voisinage haut gauche des vignettes
	m_nVoisPos[eTopL][0] = m_lVignetteSize / 4;		// ie r4
	m_nVoisPos[eTopL][1] = m_lVignetteSize / 4;		// ie r4
	*/

	m_nVoisPos[eTopMid][0] = (m_lVignetteSize - m_nVois) / 2; 
	m_nVoisPos[eTopMid][1] = m_lVignetteSize / 4 - m_nVois / 2; 
	// voisinage haut droit des vignettes
	m_nVoisPos[eTopR][0] = 3 * m_lVignetteSize / 4 - m_nVois / 2;	// ie r2+r4 ie r-r4
	m_nVoisPos[eTopR][1] = m_lVignetteSize / 4 - m_nVois / 2;		// ie r2-r4 ie r4
	// voisinage milieu droite  des vignettes
	m_nVoisPos[eMidR][0] = 3 * m_lVignetteSize / 4 - m_nVois / 2; 
	m_nVoisPos[eMidR][1] = (m_lVignetteSize - m_nVois) / 2;
	// voisinage bas droit des vignettes
	m_nVoisPos[eBotR][0] = 3 * m_lVignetteSize / 4 - m_nVois / 2;	// ie r2+r4 ie r-r4
	m_nVoisPos[eBotR][1] = 3 * m_lVignetteSize / 4 - m_nVois / 2;  // ie r2+r4 ie r-r4
	// voisinage bas milieu des vignettes
	m_nVoisPos[eBotMid][0] = (m_lVignetteSize - m_nVois) / 2; 
	m_nVoisPos[eBotMid][1] = 3 * m_lVignetteSize / 4  - m_nVois / 2; 
	// voisinage bas gauche des vignettes
	m_nVoisPos[eBotL][0] = m_lVignetteSize / 4 - m_nVois / 2;		// ie r2-r4 ie r4
	m_nVoisPos[eBotL][1] = 3 * m_lVignetteSize / 4 - m_nVois / 2;	// ie r2+r4 ie r-r4
	// voisinage gauche milieu des vignettes
	m_nVoisPos[eMidL][0] =  m_lVignetteSize / 4 - m_nVois / 2; 
	m_nVoisPos[eMidL][1] = (m_lVignetteSize - m_nVois) / 2;
	// voisinage haut gauche des vignettes
	m_nVoisPos[eTopL][0] = m_lVignetteSize / 4 - m_nVois / 2;		// ie r4
	m_nVoisPos[eTopL][1] = m_lVignetteSize / 4 - m_nVois / 2;		// ie r4

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
		elt._fMin = FLT_MAX;
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

//	m_oCVCoefMat	=  Mat::ones(m_CoefMat->GetLi(),	m_CoefMat->GetCo(), CV_32F);

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

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	//
	// Compute All Height vignettes
	//

	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("##### Start Compute Construct Vignettes")));
	double dssStartConstruct = GetPerfTime();

	//La vignette "0" se situe au centre de l'image, negatif à gauche positif à droite, negatif en haut positif en bas pour l'indexation 

	int nStepX = m_lVignetteSize / 2;
	int nStepY = nStepX;

	// debug si step different
	/*nStepX = m_lVignetteSize - m_nVois;
	nStepY = nStepX;
	m_nVoisPos[eTopMid][0] = (m_lVignetteSize - m_nVois) / 2; 
	m_nVoisPos[eTopMid][1] = 0 ; 
	m_nVoisPos[eBotMid][0] = (m_lVignetteSize - m_nVois) / 2; 
	m_nVoisPos[eBotMid][1] = m_lVignetteSize - m_nVois; 
	m_nVoisPos[eMidR][0] = m_lVignetteSize - m_nVois; 
	m_nVoisPos[eMidR][1] = (m_lVignetteSize - m_nVois) / 2;
	m_nVoisPos[eMidL][0] =  0; 
	m_nVoisPos[eMidL][1] = (m_lVignetteSize - m_nVois) / 2;
	m_nVoisPos[eTopR][0] = m_lVignetteSize - m_nVois;
	m_nVoisPos[eTopR][1] = 0;	
	m_nVoisPos[eTopL][0] = 0;	
	m_nVoisPos[eTopL][1] = 0;	
	m_nVoisPos[eBotR][0] = m_lVignetteSize - m_nVois;
	m_nVoisPos[eBotR][1] = m_lVignetteSize - m_nVois; 
	m_nVoisPos[eBotL][0] = 0;		
	m_nVoisPos[eBotL][1] = m_lVignetteSize - m_nVois;	*/


	// on calcule les index sur un quadrant en partant du 0 
	int nHalfMAX_J = 1;
	if(nStepY != 0)
		nHalfMAX_J = (int) floor((float)(nLi2)/(float)nStepY);
	int nHalfMAX_I = 1;
	if(nStepX != 0)
		nHalfMAX_I = (int) floor((float)(nCo2)/(float)nStepX);
	int i,j;
	
	int nMarVignSize = m_lVignetteSize+2*m_nMargin;
	int nZeroVignPos_Y = nLi2 - nVign2;
	int nZeroVignPos_X = nCo2 - nVign2;
	int nFirstVignPosY = nZeroVignPos_Y - nHalfMAX_J * nStepY;
	int nFirstVignPosX = nZeroVignPos_X - nHalfMAX_I * nStepX;
	
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("{%s}  Stp =%d   HmxI =%d    HmxJ =%d"),	INanoTopoTreament::GetName(),nStepX,nHalfMAX_I,nHalfMAX_J));

	// Init Array
	tVignetteRes oVignette;
	oVignette._bUsed		= false;
	oVignette._bComputed	= false;
	oVignette._Z			= Mat::zeros(m_lVignetteSize,m_lVignetteSize,CV_32F);
	vector< vector<tVignetteRes> > oGridZ;
	oGridZ.reserve(2*nHalfMAX_J);

	for (j=0; j<2*nHalfMAX_J; j++)
	{
		vector<tVignetteRes> vRowi;
		vRowi.reserve(2*nHalfMAX_I);
		oVignette._nPosY = nFirstVignPosY + j * nStepY;
		for (i=0; i<2*nHalfMAX_I; i++)
		{
			oVignette._nPosX = nFirstVignPosX + i * nStepX;
			vRowi.push_back(oVignette);
		}
		oGridZ.push_back(vRowi);
	}

	int nSzH = m_oCVMatNX.rows;
	int nSzW = m_oCVMatNX.cols;

	m_oRg = Mat::zeros(m_lOriginalHeight_px,m_lOriginalWidth_px, CV_32F);
	m_oIterMapg = Mat(m_lOriginalHeight_px,m_lOriginalWidth_px, CV_32F, Scalar(0.0f/*g_fEPSILON*/));

	//
	// Compute Z Data
	//

	nFirstVignPosY += m_nMargin;
	nFirstVignPosX += m_nMargin;

	#pragma omp parallel private(i) 
	{	
		#pragma omp for schedule(dynamic)
		for (j=0; j<2*nHalfMAX_J; j++)
		{
			int nIdxY = nFirstVignPosY + j*nStepY - m_nMargin ;
			if ( nIdxY < 0 || nIdxY > (nSzH-nMarVignSize) || m_bEmergencyStop)
				continue;

			for (i=0; i<2*nHalfMAX_I; i++)
			{	
				int nIdxX = nFirstVignPosX + i*nStepX - m_nMargin;
				if ( nIdxX < 0 || nIdxX > (nSzW-nMarVignSize) || m_bEmergencyStop)
					continue;

				Rect rcVignetteRect(nIdxX , nIdxY, nMarVignSize, nMarVignSize);
				// Apply ROI to mask
				Mat oMM = m_oCVMatMaskD(rcVignetteRect).clone();
				// Check if this sample contain some wafer data
				Scalar oSumMsk = sum(oMM);
				if(oSumMsk[0] == 0)
				{
					//unused data sample go to the next one
					continue;
				}

				oGridZ[j][i]._bUsed = true;

				// Apply ROI to NX
				Mat oNNX = m_oCVMatNX(rcVignetteRect).clone();
				// Apply ROI to NY
				Mat oNNY = m_oCVMatNY(rcVignetteRect).clone();
				ComputeVignette(nIdxX,nIdxY,oNNX,oNNY,oMM,oGridZ[j][i]);

				tVignetteRes ov = oGridZ[j][i];
 				ASSERT(oGridZ[j][i]._nPosX == nIdxX );
				ASSERT(oGridZ[j][i]._nPosY == nIdxY );
				if(pSavData && (uDbgFlag & DBG_REC_VIGNETTE) /*&& ov._nPosX == 1736 && ov._nPosY == 1560*/)
				{
					H3_MATRIX_FLT32 oZmat(ov._Z.rows,ov._Z.cols);
					Mat zztemp( ov._Z.rows, ov._Z.cols, CV_32F, oZmat.GetData(), Mat::AUTO_STEP);
					oGridZ[j][i]._Z.copyTo(zztemp);

					tSpT<H3_MATRIX_FLT32> elt;
					elt._cs  = Fmt(_T("ZCoord_x%0.4d_y%0.4d_"), ov._nPosX, ov._nPosY);
					elt._spT.reset(new H3_MATRIX_FLT32(oZmat)); // copy since data altered in following treatments
					elt._bImg = true;
					elt._bHbf = false;
					elt._bBin = true;
					elt._fMin = FLT_MAX;
					pSavData->spListF32.push_back(elt);

// 					H3_MATRIX_FLT32 oNXmat(oNNX.rows,oNNX.cols);
// 					Mat nxtemp( oNNX.rows,oNNX.cols, CV_32F, oNXmat.GetData(), Mat::AUTO_STEP);
// 					oNNX.copyTo(nxtemp);
// 					elt._cs  = Fmt(_T("ZCoord_x%0.4d_y%0.4d_NX"),oGridZ[j][i]._nPosX,oGridZ[j][i]._nPosY);
// 					elt._spT.reset(new H3_MATRIX_FLT32(oNXmat)); // copy since data altered in following treatments
// 					pSavData->spListF32.push_back(elt);
// 
// 					H3_MATRIX_FLT32 oNYmat(oNNY.rows,oNNY.cols);
// 					Mat nytemp( oNNY.rows,oNNY.cols, CV_32F, oNYmat.GetData(), Mat::AUTO_STEP);
// 					oNNY.copyTo(nytemp);
// 					elt._cs  = Fmt(_T("ZCoord_x%0.4d_y%0.4d_NY"),oGridZ[j][i]._nPosX,oGridZ[j][i]._nPosY);
// 					elt._spT.reset(new H3_MATRIX_FLT32(oNYmat)); // copy since data altered in following treatments
// 					pSavData->spListF32.push_back(elt);
				}
			}
		}
	}

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	double dssEndConstruct = GetPerfTime();
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done All Construct in %lf ms"),  dssEndConstruct - dssStartConstruct));
	
	
// 	int nCrossYY = 1025;	
// 	int nCrossXXs = 350;
// 	int nCrossXXe = 650;

/*	for (j=0; j<2*nHalfMAX_J; j++)
	{
		for (i=0; i<2*nHalfMAX_I; i++)
		{
		
			if( (oGridZ[j][i]._nPosY <  nCrossYY)  && ((oGridZ[j][i]._nPosY+128) >  nCrossYY ) )
			{
				if( (oGridZ[j][i]._nPosX > nCrossXXe ) || ((oGridZ[j][i]._nPosX+128) <  nCrossXXs) )
				{
					//do nothing
				}
				else
				{
					// save cut
					LogThis(1,4,Fmt(_T("#---- oGridZ[j][i] ----> i = %d;   j = %d --- [%d;%d]"), i,j,oGridZ[j][i]._nPosX,oGridZ[j][i]._nPosY));
					//oGridZ[j][i]._Z = Mat::ones(128,128,CV_32F) * -10000.0;
					CStdioFile oFile;
					if (oFile.Open(Fmt(_T("d:\\altasight\\vign_%d_%d.csv"),i,j), CFile::modeCreate | CFile::modeWrite))
					{
						for (int x=0; x<128; x++)
						{
							float fX = x+oGridZ[j][i]._nPosX;
							float fY = oGridZ[j][i]._Z.at<float>(nCrossYY-oGridZ[j][i]._nPosY,x);
							oFile.WriteString(Fmt(_T("%lf;%lf;\n"),fX,fY));
						}
						oFile.Close();
					}
				}
			}
			
		}
	}
*/

/*	for(j=15;j<=16;j++)
	{
		CStdioFile oFile;
		if (oFile.Open(Fmt(_T("d:\\altasight\\vign_5-11_%d.csv"),j), CFile::modeCreate | CFile::modeWrite))
		{
			for (int x=0; x<128; x++)
			{
				for(i=5;i<=11;i++)
				{	
					float fX = x+oGridZ[j][i]._nPosX;
					float fY = oGridZ[j][i]._Z.at<float>(nCrossYY-oGridZ[j][i]._nPosY,x);
					oFile.WriteString(Fmt(_T("%lf;%lf;"),fX,fY));
				}
				oFile.WriteString(Fmt(_T("\n")));
			}
			oFile.Close();
		}
	}*/

	//
	// Recollage des vignettes
	//

	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("##### Start Binding Vignettes")));
	double dssStartRecol = GetPerfTime();

	// Selctionne la vignette de reference - par défaut celle du middle wafer
	int nVRef_I = nHalfMAX_I;
	int nVRef_J = nHalfMAX_J;
	
	// vignette 0 du middle;
	m_oRg(		Rect(oGridZ[nVRef_J][nVRef_I]._nPosX,	oGridZ[nVRef_J][nVRef_I]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oGridZ[nVRef_J][nVRef_I]._Z.mul(m_oCVCoefMat);
	m_oIterMapg(Rect(oGridZ[nVRef_J][nVRef_I]._nPosX,	oGridZ[nVRef_J][nVRef_I]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= m_oCVCoefMat;
	oGridZ[nVRef_J][nVRef_I]._bComputed = true;
	
	// upper 
	RecolleVignette(oGridZ[nVRef_J-1][nVRef_I],(int)eBotMid);// top middle de la vignette 0;	
	// bottom
	RecolleVignette(oGridZ[nVRef_J+1][nVRef_I],(int)eTopMid);// Bottom middle de la vignette 0;	
	//AfxMessageBox("upper-bottom");

	// left
	RecolleVignette(oGridZ[nVRef_J][nVRef_I-1],(int)eMidR);// middle left  de la vignette 0;	
	// right
	RecolleVignette(oGridZ[nVRef_J][nVRef_I+1],(int)eMidL); // middle right de la vignette 0;
	//AfxMessageBox("left-right");

	// upper R
	RecolleVignette(oGridZ[nVRef_J-1][nVRef_I+1],(int)eBotL);// top right de la vignette 0;
	// Bottom R
	RecolleVignette(oGridZ[nVRef_J+1][nVRef_I+1],(int)eTopL);// bottom right  de la vignette 0;	
	//AfxMessageBox("R");

	// upper L
	RecolleVignette(oGridZ[nVRef_J-1][nVRef_I-1],(int)eBotR);// upper left de la vignette 0;
	// Bottom L
	RecolleVignette(oGridZ[nVRef_J+1][nVRef_I-1],(int)eTopR);// bottom left de la vignette 0;
	//AfxMessageBox("L");

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	tPrmThreadRecollage oPrm;
	oPrm._ptr = this;
	oPrm._nMaxI = 2*nHalfMAX_I;
	oPrm._nMaxJ = 2*nHalfMAX_J;
	oPrm._nRefI = nVRef_I;
	oPrm._nRefJ = nVRef_J;

	oPrm._pGridZ = &oGridZ;

	CWinThread* pThreadBR = AfxBeginThread(&CTreatReconstruct1::DoRecolRightBand, &oPrm, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadBR->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadBR->ResumeThread();
	Sleep(10);
//  DoRecolRightBand(&oPrm);
//  AfxMessageBox("DoRecolRightBand");

	CWinThread* pThreadBL = AfxBeginThread(&CTreatReconstruct1::DoRecolLeftBand, &oPrm, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadBL->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadBL->ResumeThread();
	Sleep(10);
// 	DoRecolLeftBand(&oPrm);
// 	AfxMessageBox("DoRecolLeftBand");

	CWinThread* pThreadQ1 = AfxBeginThread(&CTreatReconstruct1::DoRecolQ1, &oPrm, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadQ1->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ1->ResumeThread();
	Sleep(10);
// 	DoRecolQ1(&oPrm);
// 	AfxMessageBox("DoRecolQ1");

	CWinThread* pThreadQ3 = AfxBeginThread(&CTreatReconstruct1::DoRecolQ3, &oPrm, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadQ3->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ3->ResumeThread();
	Sleep(10);
// 	DoRecolQ3(&oPrm);
// 	AfxMessageBox("DoRecolQ3");

	CWinThread* pThreadQ2 = AfxBeginThread(&CTreatReconstruct1::DoRecolQ2, &oPrm, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadQ2->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ2->ResumeThread();
	Sleep(10);
// 	DoRecolQ2(&oPrm);
// 	AfxMessageBox("DoRecolQ2");
	
	CWinThread* pThreadQ4 = AfxBeginThread(&CTreatReconstruct1::DoRecolQ4, &oPrm, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadQ4->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadQ4->ResumeThread();
	Sleep(10);
// 	DoRecolQ4(&oPrm);
// 	AfxMessageBox("DoRecolQ4");

	dwEvent = WaitForMultipleObjects(sizeof(m_hEventBandDone) / sizeof(HANDLE), m_hEventBandDone, TRUE, INFINITE);

	// Meanwhile...
	Mat oMskE32f;
	oCVMatMaskE.convertTo(oMskE32f,CV_32F);
	Mat oCVMatMaskEInv;
	bitwise_not(oCVMatMaskE,oCVMatMaskEInv);
	oCVMatMaskEInv = oCVMatMaskEInv - 254; // pour ramener la dynamix à 0 - 1
	Mat oInvMskE32f;
	oCVMatMaskEInv.convertTo(oInvMskE32f,CV_32F);

	dwEvent = WaitForMultipleObjects(sizeof(m_hEventQDone) / sizeof(HANDLE), m_hEventQDone, TRUE, INFINITE);

	double dssEndRecol = GetPerfTime();
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done All Recol in %lf ms"),  dssEndRecol - dssStartRecol));

	////
	///////////////////////////////////////////////////////////////////////////
	// vignette 0 du middle; // rajout de la hauteur pour l'identifier sur image
// 	Mat oMatdbg = Mat::ones(128,128,CV_32F);
// 	m_oRg(		Rect(oGridZ[nVRef_J][nVRef_I]._nPosX,	 oGridZ[nVRef_J][nVRef_I]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg*0.2;
// 	m_oRg(		Rect(oGridZ[nVRef_J-2][nVRef_I-1]._nPosX,oGridZ[nVRef_J-2][nVRef_I-1]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg*0.1; // start Q1
// 	m_oRg(		Rect(oGridZ[nVRef_J-2][nVRef_I]._nPosX,	 oGridZ[nVRef_J-2][nVRef_I]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg*0.2; // start Q2
// 	m_oRg(		Rect(oGridZ[nVRef_J+2][nVRef_I-1]._nPosX,oGridZ[nVRef_J+2][nVRef_I-1]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg*0.3; // start Q3
// 	m_oRg(		Rect(oGridZ[nVRef_J+2][nVRef_I]._nPosX,	 oGridZ[nVRef_J+2][nVRef_I]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg*0.4; // start Q4

// 
// 	m_oRg(		Rect(oGridZ[nVRef_J-3][nVRef_I]._nPosX,	oGridZ[nVRef_J-3][nVRef_I]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg * 0.1;
// 	m_oRg(		Rect(oGridZ[nVRef_J+3][nVRef_I]._nPosX,	 oGridZ[nVRef_J+3][nVRef_I]._nPosY,m_lVignetteSize,m_lVignetteSize))+= oMatdbg * 0.1;
// 	m_oRg(		Rect(oGridZ[nVRef_J][nVRef_I+3]._nPosX,	oGridZ[nVRef_J][nVRef_I+3]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg * 0.1;
// 	m_oRg(		Rect(oGridZ[nVRef_J][nVRef_I-3]._nPosX,	oGridZ[nVRef_J][nVRef_I-3]._nPosY,m_lVignetteSize,m_lVignetteSize))	+= oMatdbg * 0.1;

	//
	// Finalize and compute Hm
	//

	oCVHm = (m_oRg / m_oIterMapg);

// 	CStdioFile oFile2;
// 	if (oFile2.Open(Fmt(_T("d:\\altasight\\vignRECOL.csv")), CFile::modeCreate | CFile::modeWrite))
// 	{
// 		for (int x=nCrossXXs; x<=nCrossXXe; x++)
// 		{
// 			float fX = (float)x;
// 			float fY = oCVHm.at<float>(nCrossYY,x);
// 			oFile2.WriteString(Fmt(_T("%lf;%lf;\n"),fX,fY));
// 		}
// 		oFile2.Close();
// 	}


// 	//Filter lisseur
// 	Mat kernelGaussCoef = getGaussianKernel(2*20+1, 3.0, CV_32F);
// 	sepFilter2D(oCVHm,oCVHm,CV_32F,kernelGaussCoef,kernelGaussCoef);

	// DEBUG - save profil X and Y
	if(0)
	{
		CString csOutPath	= _T("C:\\Altasight\\Nano\\Data");
		void* pcs;
		if(FindTreatPrmPtr(p_InputsPrm,"OutPath",pcs))
		{
			csOutPath = *((CString*) pcs); 	
		}

		Mat Hglobal;
		oCVHm.copyTo(Hglobal);
		Hglobal = oMskE32f.mul(Hglobal);

		CTime oTime = CTime::GetCurrentTime();
		CString csTime = oTime.Format(_T("HEIGHTMAP-ProfileXY-%m%d%Y_%H%M.csv"));
		CString csprofilesfiles = csOutPath + _T("\\");
		csprofilesfiles += csTime;
		CStdioFile oFile;
		int noffx = (oCVHm.cols - 3600)/2;
		int noffy = (oCVHm.rows - 3248)/2;
		int nProfX_y = 1624; int nProfX_Start_x = 240/*-20*/; int nProfX_End_x = 3364/*+40*/;
		int nProfY_x = 1780; int nProfY_Start_y = 62/*-20*/; int nProfY_End_y = (nProfY_Start_y + nProfX_End_x - nProfX_Start_x);

		if (oFile.Open(csprofilesfiles, CFile::modeCreate | CFile::modeWrite))
		{
			oFile.WriteString(Fmt("zX(@y=%d+%d);zY(@x=%d+%d);\n",nProfX_y,noffy,nProfY_x,noffx));
			int nNbPtsPcx = nProfX_End_x - nProfX_Start_x;
			for (int n=0; n<=nNbPtsPcx; n++)
			{
				float pX = Hglobal.at<float>(nProfX_y + noffy,nProfX_Start_x + n + noffx);
				float pY = Hglobal.at<float>(nProfY_Start_y + n + noffy,nProfY_x + noffx);
				oFile.WriteString(Fmt("%lf;%lf;\n",pX,pY));
			}
			oFile.Close();
		}
	}


	H3_MATRIX_FLT32 oHmPresent(oCVHm.rows,oCVHm.cols);
	Mat oCvHmPresentation( oCVHm.rows, oCVHm.cols, CV_32F, oHmPresent.GetData(), Mat::AUTO_STEP);
	oCVHm.copyTo(oCvHmPresentation);
	oCvHmPresentation = oMskE32f.mul(oCvHmPresentation);

 	double minpx, maxpx;
 	minMaxLoc(oCvHmPresentation, &minpx, &maxpx); //find minimum and maximum intensities
	if(minpx > 0.0) minpx = 0.0;
 	Mat oMins = Mat(m_lOriginalHeight_px,m_lOriginalWidth_px, CV_32F, Scalar(minpx-g_fMinZeroSub));
 	oCvHmPresentation += oInvMskE32f.mul(oMins);

	oMskE32f.release();
	oInvMskE32f.release();
	m_oIterMapg.release();
	m_oRg.release();

	if(pSavData &&  (uDbgFlag & DBG_REC_H))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("Hm");
		elt._spT.reset(new H3_MATRIX_FLT32(oHmPresent)); // copy since data altered in following treatments
		elt._bImg = true;
		elt._bHbf = false;
		elt._bBin = true;
		elt._fMin = (float)minpx;
		pSavData->spListF32.push_back(elt);
	}

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

		CWinThread* pThread = AfxBeginThread(&CTreatReconstruct1::SaveData, pSavData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
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

	if(FindTreatPrmPtr(p_OutputsPrm,"VignStartPosX",p))
	{
		int* pN = (int*) p;
		(*pN) = nFirstVignPosX;
	}
	if(FindTreatPrmPtr(p_OutputsPrm,"VignStartPosY",p))
	{
		int* pN = (int*) p;
		(*pN) = nFirstVignPosY;
	}
	if(FindTreatPrmPtr(p_OutputsPrm,"VignSize",p))
	{
		int* pN = (int*) p;
		(*pN) = (int)m_lVignetteSize;
	}
	AddTreatPrmSharedPtr(p_OutputsPrm,"H",shared_ptr<void>(m_H));

	if(uDbgFlag & DBG_SHOW_DISPLAY)
	{
		double minVal, maxVal;
		minMaxLoc(oCvHmPresentation, &minVal, &maxVal); //find minimum and maximum intensities
		minVal += g_fMinZeroSub;
		Mat draw;
		oCvHmPresentation.convertTo(draw, CV_8U, 255.0/(maxVal - minVal), - minVal * 255.0f / (maxVal - minVal));
		imshow( "Reconstruct window", draw );
	}

	m_matMaskDilate.reset();
	m_matMaskErode.reset();
	m_CoefMat.reset();
	m_H.reset();
	m_matNX.reset();
	m_matNY.reset();

	return true;
}

UINT CTreatReconstruct1::DoRecolRightBand( void *p_pParameters )
{
	tPrmThreadRecollage* pPrm = static_cast<tPrmThreadRecollage *> (p_pParameters);
	CTreatReconstruct1* pObj = pPrm->_ptr;

	// skip band thread - do nothing
	if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SKIP_BANDTHREAD)
	{
		SetEvent(pObj->m_hEventHalfBandDone[0]);
		SetEvent(pObj->m_hEventHalfBandDone[1]);
		SetEvent(pObj->m_hEventBandDone[0]);
		return 2;
	}

	int nRefI = pPrm->_nRefI;
	int nRefJ = pPrm->_nRefJ;
	int nMaxI = pPrm->_nMaxI;
	int nMaxJ = pPrm->_nMaxJ;
	int nRankToStartQRights = nRefI + nRefI/2 +1;

	double dss = GetPerfTime();
	if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("(%s) Start DoRecolRightBand.\n"),pObj->GetName()));

	// Band droite
	for (int i=nRefI+2; i<nMaxI; i++)
	{
		pObj->RecolleVignette((*(pPrm->_pGridZ))[nRefJ][i],eMidL);
		pObj->RecolleVignette((*(pPrm->_pGridZ))[nRefJ+1][i],eTopL);
		pObj->RecolleVignette((*(pPrm->_pGridZ))[nRefJ-1][i],eBotL);
		if( i == nRankToStartQRights)
		{
			SetEvent(pObj->m_hEventHalfBandDone[0]);
			SetEvent(pObj->m_hEventHalfBandDone[1]);
		}
	}
	
	if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("(%s) DoRecolRightBand done in %f.\n"),pObj->GetName(),GetPerfTime() - dss));
	SetEvent(pObj->m_hEventBandDone[0]);
	return 1;
}

UINT CTreatReconstruct1::DoRecolLeftBand( void *p_pParameters )
{
	tPrmThreadRecollage* pPrm = static_cast<tPrmThreadRecollage *> (p_pParameters);
	CTreatReconstruct1* pObj = pPrm->_ptr;

	int nRefI = pPrm->_nRefI;
	int nRefJ = pPrm->_nRefJ;
	int nMaxI = pPrm->_nMaxI;
	int nMaxJ = pPrm->_nMaxJ;
	int nRankToStartQLefts = nRefI/2 - 1;

	// skip band thread - do nothing
	if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SKIP_BANDTHREAD)
	{
		SetEvent(pObj->m_hEventQStart[0]); //start Q1
		SetEvent(pObj->m_hEventQStart[2]); //start Q3
		SetEvent(pObj->m_hEventBandDone[1]);
		return 2;
	}

	double dss = GetPerfTime();
	if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("(%s) Start DoRecolLeftBand.\n"),pObj->GetName()));

	// Band Left
	for (int i=nRefI-2; i>=0; i--)
	{
		pObj->RecolleVignette((*(pPrm->_pGridZ))[nRefJ][i],eMidR);
		pObj->RecolleVignette((*(pPrm->_pGridZ))[nRefJ+1][i],eTopR);
		pObj->RecolleVignette((*(pPrm->_pGridZ))[nRefJ-1][i],eBotR);
		if( i == nRankToStartQLefts)
		{
			SetEvent(pObj->m_hEventQStart[0]); //start Q1
			SetEvent(pObj->m_hEventQStart[2]); //start Q3
		}
	}
	if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("(%s) DoRecolLeftBand done in %f.\n"),pObj->GetName(),GetPerfTime() - dss));
	SetEvent(pObj->m_hEventBandDone[1]);
	return 1;
}

UINT CTreatReconstruct1::DoRecolQ1( void *p_pParameters )
{
	tPrmThreadRecollage* pPrm = static_cast<tPrmThreadRecollage *> (p_pParameters);
	CTreatReconstruct1* pObj = pPrm->_ptr;

	int nRefI = pPrm->_nRefI;
	int nRefJ = pPrm->_nRefJ;
	int nMaxI = pPrm->_nMaxI;
	int nMaxJ = pPrm->_nMaxJ;

	DWORD dwEvent = WaitForSingleObject(pObj->m_hEventQStart[0], INFINITE);
	if(dwEvent == WAIT_OBJECT_0)
	{
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SKIP_QUADTHREAD)
		{
			SetEvent(pObj->m_hEventQStart[1]); // start Q2
			SetEvent(pObj->m_hEventQDone[0]);
			return 2;
		}

		double dss = GetPerfTime();
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) Start DoRecolQ1.\n"),pObj->GetName()));
		// Q1
		for (int j=nRefJ-2; j>=0; j--)
		{
			for (int i=nRefI-1; i>=0; i--)
			{
				pObj->RecolleVignette((*(pPrm->_pGridZ))[j][i],eBotR);
			}

			if( j == (nRefJ - 5))
			{
				SetEvent(pObj->m_hEventQStart[1]); // start Q2
			}
		}
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) DoRecolQ1 done in %f.\n"),pObj->GetName(),GetPerfTime() - dss));
	}
	else
	{
		LogThis(1,4,Fmt(_T("(%s) DoRecolQ1 WaitForSingleObject failed.\n"),pObj->GetName()));
	}
	SetEvent(pObj->m_hEventQDone[0]);
	return 1;
}

UINT CTreatReconstruct1::DoRecolQ3( void *p_pParameters )
{
	tPrmThreadRecollage* pPrm = static_cast<tPrmThreadRecollage *> (p_pParameters);
	CTreatReconstruct1* pObj = pPrm->_ptr;
	int nRefI = pPrm->_nRefI;
	int nRefJ = pPrm->_nRefJ;
	int nMaxI = pPrm->_nMaxI;
	int nMaxJ = pPrm->_nMaxJ;

	DWORD dwEvent = WaitForSingleObject(pObj->m_hEventQStart[2], INFINITE);
	if(dwEvent == WAIT_OBJECT_0)
	{
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SKIP_QUADTHREAD)
		{
			SetEvent(pObj->m_hEventQStart[3]); // start Q4
			SetEvent(pObj->m_hEventQDone[2]);
			return 2;
		}

		double dss = GetPerfTime();
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) Start DoRecolQ3.\n"),pObj->GetName()));
		// Q3
		for (int j=nRefJ+2; j<nMaxJ; j++)
		{
			for (int i=nRefI-1; i>=0; i--)
			{
				pObj->RecolleVignette((*(pPrm->_pGridZ))[j][i],eTopR);
			}

			if( j == (nRefJ + 5))
			{
				SetEvent(pObj->m_hEventQStart[3]); // start Q4
			}
		}
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) DoRecolQ3 done in %f.\n"),pObj->GetName(),GetPerfTime() - dss));
	}
	else
	{
		LogThis(1,4,Fmt(_T("(%s) DoRecolQ3 WaitForSingleObject failed.\n"),pObj->GetName()));
	}
	SetEvent(pObj->m_hEventQDone[2]);
	return 1;
}

UINT CTreatReconstruct1::DoRecolQ2( void *p_pParameters )
{
	tPrmThreadRecollage* pPrm = static_cast<tPrmThreadRecollage *> (p_pParameters);
	CTreatReconstruct1* pObj = pPrm->_ptr;
	int nRefI = pPrm->_nRefI;
	int nRefJ = pPrm->_nRefJ;
	int nMaxI = pPrm->_nMaxI;
	int nMaxJ = pPrm->_nMaxJ;

	DWORD dwEvent = WaitForSingleObject(pObj->m_hEventHalfBandDone[0], INFINITE);
	if (dwEvent != WAIT_OBJECT_0)
	{
		LogThis(1,4,Fmt(_T("(%s) DoRecolQ2 HalfBand waitevent failed.\n"),pObj->GetName()));
		return -1;
	}

	DWORD dwEvent2 = WaitForSingleObject(pObj->m_hEventQStart[1], INFINITE);
	if(dwEvent2 == WAIT_OBJECT_0)
	{
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SKIP_QUADTHREAD)
		{
			SetEvent(pObj->m_hEventQDone[1]);
			return 2;
		}

		double dss = GetPerfTime();
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) Start DoRecolQ2.\n"),pObj->GetName()));
		// Q2
		for (int j=nRefJ-2; j>=0; j--)
		{
			for (int i=nRefI; i<nMaxI; i++)
			{
				pObj->RecolleVignette((*(pPrm->_pGridZ))[j][i],eBotL);
			}
		}
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) DoRecolQ2 done in %f.\n"),pObj->GetName(),GetPerfTime() - dss));
	}
	else
	{
		LogThis(1,4,Fmt(_T("(%s) DoRecolQ2 WaitForSingleObject failed.\n"),pObj->GetName()));
	}
	SetEvent(pObj->m_hEventQDone[1]);
	return 1;
}

UINT CTreatReconstruct1::DoRecolQ4( void *p_pParameters )
{
	tPrmThreadRecollage* pPrm = static_cast<tPrmThreadRecollage *> (p_pParameters);
	CTreatReconstruct1* pObj = pPrm->_ptr;
	int nRefI = pPrm->_nRefI;
	int nRefJ = pPrm->_nRefJ;
	int nMaxI = pPrm->_nMaxI;
	int nMaxJ = pPrm->_nMaxJ;

	DWORD dwEvent = WaitForSingleObject(pObj->m_hEventHalfBandDone[1], INFINITE);
	if (dwEvent != WAIT_OBJECT_0)
	{
		LogThis(1,4,Fmt(_T("(%s) DoRecolQ4 HalfBand waitevent failed.\n"),pObj->GetName()));
		return -1;
	}

	DWORD dwEvent2 = WaitForSingleObject(pObj->m_hEventQStart[3], INFINITE);
	if(dwEvent2 == WAIT_OBJECT_0)
	{
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SKIP_QUADTHREAD)
		{
			SetEvent(pObj->m_hEventQDone[3]);
			return 2;
		}

		double dss = GetPerfTime();
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) Start DoRecolQ4.\n"),pObj->GetName()));
		// Q4
		for (int j=nRefJ+2;  j<nMaxJ; j++)
		{
			for (int i=nRefI; i<nMaxI; i++)
			{
				pObj->RecolleVignette((*(pPrm->_pGridZ))[j][i],eTopL);
			}
		}
		if((pObj->m_uDbgFlag | pObj->m_uRegFlag) & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("(%s) DoRecolQ4 done in %f.\n"),pObj->GetName(),GetPerfTime() - dss));
	}
	else
	{
		LogThis(1,4,Fmt(_T("(%s) DoRecolQ4 WaitForSingleObject failed.\n"),pObj->GetName()));
	}
	SetEvent(pObj->m_hEventQDone[3]);
	return 1;
}
void CTreatReconstruct1::RecolleVignette( tVignetteRes& p_ov, int p_nVoisType )
{
	if(p_ov._bUsed && !p_ov._bComputed)
	{
		Rect rcVoisinage(p_ov._nPosX+m_nVoisPos[p_nVoisType][0],p_ov._nPosY+m_nVoisPos[p_nVoisType][1],m_nVois,m_nVois);

		Mat oPi = m_oRg(rcVoisinage) / m_oIterMapg(rcVoisinage);
		Mat oOffset = p_ov._Z(Rect(m_nVoisPos[p_nVoisType][0],m_nVoisPos[p_nVoisType][1],m_nVois, m_nVois)) - oPi;
		Scalar scOff = mean(oOffset);
		Mat Z = p_ov._Z - scOff;

	/*	int nCrossYY = 1025;	
		int nCrossXXs = 350;
		int nCrossXXe = 650;
		if( (p_ov._nPosY <  nCrossYY)  && ((p_ov._nPosY+128) >  nCrossYY ) )
		{
			if( (p_ov._nPosX > nCrossXXe ) || ((p_ov._nPosX+128) <  nCrossXXs) )
			{
				//do nothing
			}
			else
			{
				// save cut
				CStdioFile oFile;
				if (oFile.Open(Fmt(_T("d:\\altasight\\vignOFFSET_%d_%d.csv"),p_ov._nPosX,p_ov._nPosY ), CFile::modeCreate | CFile::modeWrite))
				{
					for (int x=0; x<128; x++)
					{
						float fX = x+p_ov._nPosX;
						float fY = Z.at<float>(nCrossYY-p_ov._nPosY,x);
						oFile.WriteString(Fmt(_T("%lf;%lf;\n"),fX,fY));
					}
					oFile.Close();
				}
			}
		}*/


		// application de la pyramide
		Z = Z.mul(m_oCVCoefMat); 

		// mise à jour des matrices d'iteration map et Ri dans le bon voisinage de la matrix parent
		rcVoisinage = Rect(p_ov._nPosX,p_ov._nPosY,m_lVignetteSize,m_lVignetteSize);
		m_oRg(rcVoisinage)			+= Z;
		m_oIterMapg(rcVoisinage)	+= m_oCVCoefMat;

		p_ov._bComputed = true;

// 	//	int nx = 1075;int ny = 960;
// 	//		int nszx = 1200; int nszy = 1000;
// 			int nx = 100;int ny = 500;
// 			int nszx = 1200; int nszy = 1000;
// 			if ( (p_ov._nPosX>=nx && p_ov._nPosY >= ny) && (p_ov._nPosX<(nx+nszx) && p_ov._nPosY < (ny+nszy)))
// 			{
// 				Scalar scmean;	Scalar scstddev;
// 				Rect myvois(nx,ny,nszx,nszy);
// 				double aminVal, amaxVal; Mat drawR;	Mat Rview;
// 				Mat kernelGaussCoeff = getGaussianKernel(2*200+1, 20.0, CV_32F);
// 				Mat oOut;
// 				Rview =  m_oRg(myvois) / m_oIterMapg(myvois);
// 				sepFilter2D(Rview,oOut,CV_32F,kernelGaussCoeff,kernelGaussCoeff); 
// 				Rview = Rview - oOut;
// 				//minMaxLoc(Rview, &aminVal, &amaxVal); //find minimum and maximum intensities
// 				//meanStdDev(Rview(Rect(Rview.cols/4,Rview.rows/4,Rview.cols/2, Rview.rows/2)),scmean,scstddev);
// 				amaxVal = 38.05;
// 				aminVal = - amaxVal;
// 				amaxVal = 0.000001;
// 				aminVal = - amaxVal;
// 				Rview.convertTo(drawR, CV_8U, 255.0/(amaxVal - aminVal), - aminVal * 255.0f / (amaxVal - aminVal));
// 				imshow( "Reconstruct window", drawR );
// 				//AfxMessageBox("d");
// 				Sleep(5);
// 			}
		}
		else
		{
	 		if(p_ov._bComputed)
	 			LogThis(1,4,Fmt(_T("Vignette already computed"),!p_ov._bComputed));
		}
}


void CTreatReconstruct1::ComputeVignette(int p_nIdxX,int p_nIdxY, const cv::Mat& p_oNNX, const cv::Mat& p_oNNY, cv::Mat& p_oMM, tVignetteRes& p_oVign)
{
	// Affine Transform optim
	Mat Z = AffineTransform(p_oNNX,p_oNNY);

	// On enleve la marge et on Apply le Mask
	Rect rcVignCenter(m_nMargin,m_nMargin,m_lVignetteSize,m_lVignetteSize);
 	p_oMM		= p_oMM(rcVignCenter);
 	p_oMM.convertTo(p_oMM,CV_32F); // OpenCV specific on doit multtipler des matrice de même type !!

	Z		= Z(rcVignCenter);
	Z		= Z.mul(p_oMM);

	p_oVign._Z = Z;
}


cv::Mat CTreatReconstruct1::AffineTransform(const Mat& p_oNNX,const Mat& p_oNNY )
{
	ASSERT(p_oNNX.rows == m_lVignetteSize + 2 * m_nMargin);
	ASSERT(p_oNNX.cols == m_lVignetteSize + 2 * m_nMargin);
	ASSERT(p_oNNX.rows == p_oNNY.rows);
	ASSERT(p_oNNX.cols == p_oNNY.cols);

	if((m_uDbgFlag | m_uRegFlag) & DBG_SKIP_AFFINETRANSFORM)
	{
		float fNoiseMax = 2;
		float fNoiseLow = -2;
		float fDiffNoise = fNoiseMax - fNoiseLow;
		return Mat::ones(p_oNNX.rows,p_oNNX.cols,CV_32F) * (1.0f + fNoiseLow + (float)rand()/((float)RAND_MAX/(fDiffNoise)));
	}

	Mat t11 =  Mat::zeros(p_oNNX.rows,p_oNNX.cols,CV_32F);
	Mat t22 =  Mat::zeros(p_oNNX.rows,p_oNNX.cols,CV_32F);
	Mat t12 =  Mat::zeros(p_oNNX.rows,p_oNNX.cols,CV_32F);

	cv::pow(p_oNNX,2,t11);
	cv::pow(p_oNNY,2,t22);
	cv::multiply(p_oNNX,p_oNNY,t12);

	int nKsize = (int) floor(6.0*m_dSigma);
	if (nKsize < 3)
	{
		nKsize = 3;
	}
	if(  nKsize%2 == 0) // kernel size should be odd and positive
		nKsize += 1;

	GaussianBlur(t11,t11,cv::Size(nKsize,nKsize),m_dSigma);
	GaussianBlur(t22,t22,cv::Size(nKsize,nKsize),m_dSigma);
	GaussianBlur(t12,t12,cv::Size(nKsize,nKsize),m_dSigma);

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

	int niDx = 0;
	Mat wat(2,2,CV_32F);
	Mat mEig = Mat::zeros(2,2,CV_32F);
	Mat vEigen = Mat::zeros(2,2,CV_32F);;
	Mat dEigen;

	for(int j=0;j<t11.cols;j++)
	{
		for(niDx=0;niDx<t11.rows;niDx++)
		{
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

			float fL1 = 1.0f;
			float feigd1 = EigD_1.at<float>(niDx,j);
			if(dThresh < EigD_1.at<float>(niDx,j))
			{
				fL1 = (float)(m_dAlpha + 1.0 - std::exp(-g_dPI / (std::pow( EigD_1.at<float>(niDx,j), 4)) ));  
			}
			dEigen.at<float>(0) = 1.0f; // L2(ii,jj);
			dEigen.at<float>(1) = fL1;	// L1(ii,jj);

			mEig.at<float>(0,0) = dEigen.at<float>(0);
			mEig.at<float>(1,1) = dEigen.at<float>(1);

			wat = vEigen * mEig * vEigen.t();

			t11.at<float>(niDx,j) = wat.at<float>(0,0);
			t22.at<float>(niDx,j) = wat.at<float>(1,1);
			t12.at<float>(niDx,j) = wat.at<float>(0,1);

		}
	}

	EigD_1.release();
	wat.release();
	dEigen.release();
	mEig.release();
	vEigen.release();
	//LogThis(1,1,Fmt(_T("****** Eigen done in %lf ms"), GetPerfTime() - dss));

	//
	// Creation de la matrice sparse  -- Laplacian_matrix_tensor
	//
	//dss= GetPerfTime();

	int nH		= t11.rows;				// should be 128 aka m_lVignetteSize
	int nSize2	= t11.cols * t11.rows;	// should be (128)² aka m_lVignetteSize²
	ASSERT(m_lVignetteSize + 2 * m_nMargin == nH);
	ASSERT((m_lVignetteSize + 2 * m_nMargin)* (m_lVignetteSize + 2 * m_nMargin) == nSize2);

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
		}
	}

	gx11.release(); gy22.release(); 
	gx21.release(); gy21.release(); 
	gxx1.release(); gxx2.release(); 
	gyy1.release(); gyy2.release(); 

	SimplicialCholesky< SparseMatrix<float> > oSolver;
	oSolver.compute(A);
	z = oSolver.solve(f);
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

	return oZRes;
}

bool CTreatReconstruct1::SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
{
	float* pData = p_oMatrixFloat->GetData();

	unsigned long  lCols = p_oMatrixFloat->GetCo();
	unsigned long  lLines = p_oMatrixFloat->GetLi();

	float fMin = FLT_MAX;
	float fMax = - FLT_MAX;

// 	p_fMin = (float)-950.0f;
// 	p_fMax = (float)1900.0f;


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

	CStdioFile oFile;
    if (oFile.Open(p_csFilepath + _T(".txt"), CFile::modeCreate | CFile::modeWrite))
    {
		oFile.WriteString(Fmt(_T("GreyScale [0 , 255] <=> [%lf , %lf]"),fMin,fMax));
	}

	return (bRes !=0) ;
}

bool CTreatReconstruct1::SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin /*= INT_MAX*/, int p_nMax /*= INT_MAX*/, bool bAutoscale /*= true*/)
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

UINT CTreatReconstruct1::SaveData( void *p_pParameters )
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
				if(! SaveGreyImageFlt32(csFileName,elt._spT,elt._fMin))
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

HRESULT CTreatReconstruct1::QueryInterface( REFIID iid, void **ppvObject )
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

ULONG CTreatReconstruct1::AddRef( void )
{
	m_ulRefCount++;
	return m_ulRefCount;
}

ULONG CTreatReconstruct1::Release( void )
{
	m_ulRefCount--;
	if (m_ulRefCount!=0) 
		return m_ulRefCount;
	delete this;     // Destruction de l'objet.
	return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}

extern "C"  HRESULT Create_TreatReconstruct1( REFIID iid, void **ppvObject )
{
	CTreatReconstruct1 *pObj = new CTreatReconstruct1();
	if (pObj==0) 
		return E_OUTOFMEMORY;
	return pObj->QueryInterface(iid, ppvObject);
}


