#include "StdAfx.h"
#include "TreatDegauchy0.h"

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include "opencv2/imgproc/imgproc.hpp"
using namespace cv;

//#include <omp.h> // for openMP directive //=> check also that /openMP is active in compiler properties

#include "FreeImagePlus.h"
#ifdef _DEBUG
	#pragma comment (lib , "FreeImaged")
	#pragma comment (lib , "FreeImagePlusd")
#else
	#pragma comment (lib , "FreeImage")
	#pragma comment (lib , "FreeImagePlus")
#endif

#define DBG_REC_NX			0x00000010
#define	DBG_REC_NY			0X00000020

const double g_dNaN = H3GetFPdNaN();

CTreatDegauchy0::CTreatDegauchy0()
{	
	
	for(int i =0; i<2; i++)
	{
		m_hEventThDone[i] = CreateEvent(0, FALSE, FALSE, 0);
		ASSERT(m_hEventThDone[i] != 0);	
	}

	m_uDbgFlag = 0;
	m_uRegFlag = 0;

	m_nPrmDegauchyOrder = 4;
	m_nPrmDegauchyStep = 20;
}

CTreatDegauchy0::~CTreatDegauchy0()
{
	for(int i =0; i<2; i++)
	{
		if (m_hEventThDone[i] != 0)
		{
			CloseHandle(m_hEventThDone[i]);
			m_hEventThDone[i] = 0;
		}
	}

	m_matNX.reset();
	m_matNY.reset();
}

bool CTreatDegauchy0::Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder)
{ 	

	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("Order"), m_nPrmDegauchyOrder))
		LogThis(1,3,Fmt(_T("{%s} Could not find [Order] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("Step"), m_nPrmDegauchyStep))
		LogThis(1,3,Fmt(_T("{%s} Could not find [Step] Parameter"), INanoTopoTreament::GetName()));

	m_uRegFlag = 0;
	if( ! GetRegistryFlag(m_uRegFlag, DBG_REC_NX | DBG_REC_NY))
		LogThis(1,3,Fmt(_T("{%s} Could not reach Registry flag Parameter"), INanoTopoTreament::GetName()));
	
	return true;
}

bool CTreatDegauchy0::Exec( const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm )
{

	if(m_nPrmDegauchyOrder <= 0)
	{
		//no need of computation here skip treatment return immediately
		 return true;
	}

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	double dstart = GetPerfTime();

	m_matNX.reset(new  H3_MATRIX_FLT32());
	m_matNY.reset(new  H3_MATRIX_FLT32());

	void *p = 0;
	int nSaveData = 0;
	unsigned long uDbgFlag = m_uDbgFlag | m_uRegFlag; 
	
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
	pvMask.reset();

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
	
	double dss = 0.0;
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		LogThis(1,1,Fmt(_T("##### Start PreTreatment Threads { Order=%d ; Step =%d }"),m_nPrmDegauchyOrder, m_nPrmDegauchyStep));
		dss = GetPerfTime();
	}
	tThreadData DataX;
	DataX._Obj = this;
	DataX._SrcId = 0;

	CWinThread* pThreadX = AfxBeginThread(&CTreatDegauchy0::static_Degauchi,(void*) &DataX , THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadX->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadX->ResumeThread();

	tThreadData DataY;
	DataY._Obj = this;
	DataY._SrcId = 1;

	CWinThread* pThreadY = AfxBeginThread(&CTreatDegauchy0::static_Degauchi, (void*) &DataY, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
	pThreadY->m_bAutoDelete = TRUE; // the thread delete its self after completion
	pThreadY->ResumeThread();
	
	// Wait Threads to be completed
	DWORD dwEvent = WaitForMultipleObjects(sizeof(m_hEventThDone) / sizeof(HANDLE), m_hEventThDone, TRUE, INFINITE);
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		LogThis(1,1,Fmt(_T("# Done 2 in %lf ms"), GetPerfTime() - dss));

	if(pSavData && (uDbgFlag & DBG_REC_NX))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("NX");
		elt._spT.reset(new H3_MATRIX_FLT32(*m_matNX.get())); // copy since data altered in following treatments

		elt._bImg = true;
		elt._bAutoScale = true;
		elt._fMin = FLT_MAX;
		elt._fMax = FLT_MAX;
		elt._bHbf = false;
		elt._bBin = true;
		pSavData->spListF32.push_back(elt);
	}

	if(pSavData && (uDbgFlag & DBG_REC_NY))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("NY");
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

		CWinThread* pThread = AfxBeginThread(&CTreatDegauchy0::SaveData, pSavData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
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
		(*pCs) = Fmt(_T("(%s) exec in %0.3f ms "), "PreTreatment", dEnd - dstart);
	}

	m_matNX.reset();
	m_matNY.reset();

	return true;
}

UINT CTreatDegauchy0::static_Degauchi(void *p_pParameters )
{
	tThreadData* pData		= static_cast<tThreadData *> (p_pParameters);
	CTreatDegauchy0* pObj	= static_cast<CTreatDegauchy0 *> (pData->_Obj);
	pObj->Degauchi(pData->_SrcId);

	return 0;
}

bool CTreatDegauchy0::Degauchi(int p_nImgId)
{
	H3_MATRIX_FLT32 *pMatScr;
	switch(p_nImgId)
	{
	case 0: // NX
		pMatScr = m_matNX.get();
		break;
	case 1: // NY
		pMatScr = m_matNY.get();
		break;
	default:
		LogThis(1,4,Fmt("{%s} Unknow Img Id = %d", INanoTopoTreament::GetName(), p_nImgId));
		ASSERT(FALSE);
		return false;
	}

	const unsigned long nx=m_matMask->GetCo();
	const unsigned long ny=m_matMask->GetLi();

	//échantillonnage
	unsigned long k;
	unsigned long iStep,jStep,kStep,kStep0;	//index image entiere (colonne, ligne, total)
	unsigned long Step= (unsigned long) m_nPrmDegauchyStep;
	unsigned long FitOrder=(unsigned long) m_nPrmDegauchyOrder;
	const unsigned long nny=(ny-1L)/Step+1L/Step+1L/Step+1L,nnx=(nx-1L)/Step+1L;
	H3_MATRIX_FLT32 Src(nny,nnx);
	H3_MATRIX_UINT8 MatMask(nny,nnx);
	H3_MATRIX_FLT64 MatResSurf;

	byte	*pMMask=MatMask.GetData();
	float	*pSrc=Src.GetData();
	float	*ppSrc0=pMatScr->GetData();

	//attention:il est probablement couteux en temps de fitter l'ensemble des données
	//faire un échantillonnage pour obtenir la fonction de fit
	kStep0=0L;k=0L;
	for(jStep=0L; jStep<ny; jStep+=Step)
	{
		kStep=kStep0;
		for(iStep=0L; iStep<nx; iStep+=Step)
		{
			pMMask[k]= (m_matMask->GetData())[kStep];
			pSrc[k]=  ppSrc0[kStep];
			k++;
			kStep+=Step;
		}
		kStep0+=nx*Step;
	}

	//calcul du polynome
	//MatResSurf contient les coef du polynome
	if(!H3BestFitSurf(Src,MatMask,MatResSurf,FitOrder))
	{//H3BestFitSurf gère ses propres messages d'erreur

		SetEvent(m_hEventThDone[p_nImgId]);
		return false;
	}

	// Degauchissage
	unsigned long x,y;//coordonnées dans un tableau
	double dx,dy; //les memes en double
	long index;
	unsigned long pow_i,pow_j;//puissance
	double ze;
	double Yj;
	double YjXi;
	long ind_k,ind_k0;//index dans MatResSurf

	for (y=0L,index=0L,ind_k0=0L;y<ny;y++) 
	{
		dy=(double)y/double(Step);
		for (x=0L;x<nx;x++,index++) 
		{
			if(!(m_matMask->GetData())[index])
			{
				ppSrc0[index]=0.0f;
				continue;
			}
			ze=0.0f;
			ind_k0=0L;
			dx=(double)x/(double)Step;
			Yj=1.0f;
			for (pow_j=0L;pow_j<=FitOrder;pow_j++)
			{
				ind_k=ind_k0;
				YjXi=Yj;
				for (pow_i=0L;pow_i<=FitOrder-pow_j;pow_i++)
				{				
					ze+=YjXi*MatResSurf[ind_k];
					YjXi *= dx;
					ind_k++;
				}
				Yj *= dy;
				ind_k0+=(FitOrder+1L);
			}
			ppSrc0[index]-=(float)ze;
		}
	}

	SetEvent(m_hEventThDone[p_nImgId]);
	return true;
}

/*! 
* 	\fn      bool H3BestFitSurf(const H3_ARRAY2D_FLT32& Src,
				   const H3_ARRAY2D_UINT8& SrcMask,
				   H3_ARRAY2D_FLT64 & MatResSurf,
				   long FitOrder,long MatVal,
				   const H3_ARRAY2D_UINT8& MatCoef)
* 	\brief : fit des points de Src (si SrcMask>0) par une fonction polynomiale dont les coefs sont decrit dans MatCoef  
* 	\param   const H3_ARRAY2D_FLT32& Src : 
* 	\param   const H3_ARRAY2D_UINT8& SrcMask : 
* 	\param out H3_ARRAY2D_FLT64 & MatResSurf : coefficient du polynome. La matrice doit avoir autant d'elements que MatCoef
* 	\param   long FitOrder : 
* 	\param   long MatVal : nombre d'elements à 1 dans MatCoef (à priori)
* 	\param   const H3_ARRAY2D_FLT32& MatCoef : matrice des coefficients à chercher (matrice carré)
*   \                        
							| 0 | 1 | 2 |... puissace de x
						______________________
						0	| 1	| 0	| 1	|
						1	| 0	| 1	| 0	|
	   puissance de y  ...						on cherche ici un polynome du type a00.x0y0+a20.x2y0+a11.x1y1
* 	\return  bool		
* 	\remarks 
*/ 
bool CTreatDegauchy0::H3BestFitSurf(const H3_MATRIX_FLT32& Src,
				   const H3_MATRIX_UINT8& SrcMask,
				   H3_MATRIX_FLT64 & MatResSurf,
				   long FitOrder,long MatVal,
				   const H3_MATRIX_UINT8& MatCoef)
{
	CString str;

	long nSizeX=Src.GetCo();
	long nSizeY=Src.GetLi();

	long ValidElement=0;
	long NumRow=0;
	long i,j,k=0;

	if((nSizeX!=SrcMask.GetCo())||(nSizeY!=SrcMask.GetLi()))
	{
		LogThis(1,4,Fmt("{%s} H3BestFitSurf Error - Mask and Source Dimension are different",INanoTopoTreament::GetName()));
		return false;
	}

	//Initialiser les pointeurs sur les données
	H3_FLT32 *pSource=Src.GetData();
	H3_UINT8 *pMask=SrcMask.GetData();

	// Determination du nombre d'éléments valide à partir du masque
	for(long li=0L; li<nSizeY; li++) {
		for(long co=0; co<nSizeX; co++) {
			if ( (*pMask>0) /*&& (!_isnan(*pSource))*/ )
				ValidElement++;
			pMask++;pSource++;
		}
	}

	if(ValidElement<MatVal)
	{
		LogThis(1,4,Fmt("{%s} H3BestFitSurf Error - Fitting error n°2  ",INanoTopoTreament::GetName()));
		return false;
	}

	pMask=SrcMask.GetData();
	pSource=Src.GetData();

	// Creation et initialisation des matrices Y,XjYi,M,TM
	//H3_MATRIX_FLT64 M(ValidElement,MatVal),SURF(ValidElement,1),MATRESSURF(MatVal,1);
	H3_MATRIX_FLT64 MATRESSURF(MatVal,1),SURF(ValidElement,1);
	H3_MATRIX_FLT64 MM(MatVal,MatVal),MS(MatVal,1L),MatTmp(MatVal,1L);
	MM.Fill(0);
	MS.Fill(0);
	MatTmp.Fill(0);

	H3_FLT64 Yi,XjYi;
	// Calcul des coefficients de la matrice MatResSurf
	k=0L;
	for(long Ypix=0L; Ypix<nSizeY; Ypix++) {
		for(long Xpix=0L; Xpix<nSizeX; Xpix++) {
			if ((*pMask>0L)/* && (!_isnan(*pSource))*/ )
			{
				NumRow=0L;
				Yi=1.0;
				for(i=0L; i<=FitOrder; i++) {
					XjYi=Yi;
					for(j=0L; j<=FitOrder; j++)
					{									
						if (MatCoef(i,j)==1L) {
							MatTmp[NumRow]=XjYi;
							NumRow++; 
						}
						XjYi*=Xpix;
					}
					Yi*=Ypix;
				}
				SURF[k]=*pSource;

				for(i=0L;i<MatVal;i++)
				{
					MM(i,i)+=MatTmp[i]*MatTmp[i];
					for(j=i+1L;j<MatVal;j++)
						MM(j,i)+=MatTmp[i]*MatTmp[j];
					MS[i]+=MatTmp[i]*(*pSource);
				}

				for(i=0L;i<MatVal;i++)
					for(j=i+1L;j<MatVal;j++)
						MM(i,j)=MM(j,i);

				k++;
			}
			pMask++;pSource++;
		}
	}

	// Resoudre avec le critere des moindres carres

	//1 normalisation
	H3_MATRIX_FLT64 MatMaxCo(1L,MatVal),MatMaxLi(MatVal,1L);
	MatMaxCo.Fill(0.0);
	MatMaxLi.Fill(0.0);

	for(i=0L;i<MatVal;i++){
		for(j=0L;j<MatVal;j++){
			MatMaxCo[j]=__max(MatMaxCo[j],fabs(MM(i,j)));
			MatMaxLi[i]=__max(MatMaxLi[i],fabs(MM(i,j)));
		}
	}
	for(i=0L;i<MatVal;i++){
		MatMaxCo[i]=sqrt(MatMaxCo[i]);
		MatMaxLi[i]=sqrt(MatMaxLi[i]);
	}
	
	H3_MATRIX_FLT64 MM_tmp(MatVal,MatVal);
	for(i=0L;i<MatVal;i++)
		for(j=0L;j<MatVal;j++)
			MM_tmp(i,j)=MM(i,j)/(MatMaxCo[j]*MatMaxLi[i]);	
	
	//2 inversion
	H3_MATRIX_FLT64 iMM_tmp=MM_tmp.Inv();

	if(iMM_tmp.GetSize()==MatVal*MatVal){
		for(i=0L;i<MatVal;i++)
		for(j=0L;j<MatVal;j++)
			iMM_tmp(i,j)/=MatMaxCo[i]*MatMaxLi[j];

		MATRESSURF=iMM_tmp*MS;
	}
	else{
		LogThis(1,4,Fmt("{%s} H3BestFitSurf Error - Fitting error n°3  ",INanoTopoTreament::GetName()));
		return false;
	}

	if(MATRESSURF.GetSize()!=MatVal)
	{
		LogThis(1,4,Fmt("{%s} H3BestFitSurf Error - Fitting error n°4  ",INanoTopoTreament::GetName()));
		return false;
	}

	k=0L;
	for(i=0L; i<=FitOrder; i++) {
		for(j=0L; j<=FitOrder; j++) {
			if (MatCoef(i,j)==1L) {
				MatResSurf(i,j)=MATRESSURF[k];
				k++;
			}
			else
				MatResSurf(i,j)=0.0;
		}
	}
	return true;
}

/*! 
* 	\fn      bool H3BestFitSurf(const H3_ARRAY2D_FLT32& Src,const H3_ARRAY2D_UINT8& SrcMask,H3_ARRAY2D_FLT64 & MatResSurf,long FitOrder)
* 	\brief : appel simplifier de H3BestFitSurf
*/
bool CTreatDegauchy0::H3BestFitSurf(const H3_MATRIX_FLT32& Src, const H3_MATRIX_UINT8& SrcMask, H3_MATRIX_FLT64 & MatResSurf, long FitOrder)
{
	long i,j;
	long Imax=FitOrder+1;
	long Jmax=FitOrder+1;
	
	H3_MATRIX_UINT8 MatCoef(Imax,Jmax);
	long MatVal=0;
	MatCoef.Fill(0L);

	for(i=0L;i<Imax;i++)
	for(j=0L;j<Jmax-i;j++)
	{
		MatCoef(i,j)=1L;
		MatVal++;
	}
	MatResSurf.ReAlloc(MatCoef.GetLi(),MatCoef.GetCo());
	MatResSurf.Fill(g_dNaN);
	return(H3BestFitSurf(Src,SrcMask,MatResSurf,FitOrder,MatVal,MatCoef));
}

bool CTreatDegauchy0::SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
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

bool CTreatDegauchy0::SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin /*= INT_MAX*/, int p_nMax /*= INT_MAX*/, bool bAutoscale /*= true*/)
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

UINT CTreatDegauchy0::SaveData( void *p_pParameters )
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
				csFileName = Fmt(_T("%s\\Filter_%s_%d.hbf"), sGenPath, elt._cs, nId);
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

HRESULT CTreatDegauchy0::QueryInterface( REFIID iid, void **ppvObject )
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

ULONG CTreatDegauchy0::AddRef( void )
{
	m_ulRefCount++;
	return m_ulRefCount;
}

ULONG CTreatDegauchy0::Release( void )
{
	m_ulRefCount--;
	if (m_ulRefCount!=0) 
		return m_ulRefCount;
	delete this;     // Destruction de l'objet.
	return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}

extern "C"  NT_DLL HRESULT Create( REFIID iid, void **ppvObject )
{
	CTreatDegauchy0 *pObj = new CTreatDegauchy0();
	if (pObj==0) 
		return E_OUTOFMEMORY;
	return pObj->QueryInterface(iid, ppvObject);
}


