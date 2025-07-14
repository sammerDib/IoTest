#include "StdAfx.h"
#include "TreatFilter0.h"

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

#define DBG_REC_HFILTER		0x00000010
#define DBG_REC_H_IN		0x00000020

const float     g_fMinZeroSub = 10.f;


static void
	My_bilateralFilter_32f( const Mat& src, Mat& dst, int d,
	double sigma_color, double sigma_space,
	int borderType )
{
	int cn = src.channels();
	int i, j, k, maxk, radius;
	double minValSrc=-1, maxValSrc=1;
	const int kExpNumBinsPerChannel = 1 << 12;
	int kExpNumBins = 0;
	float lastExpVal = 1.f;
	float len, scale_index;
	Size size = src.size();

	CV_Assert( (src.type() == CV_32FC1 || src.type() == CV_32FC3) &&
		src.type() == dst.type() && src.size() == dst.size() &&
		src.data != dst.data );

	if( sigma_color <= 0 )
		sigma_color = 1;
	if( sigma_space <= 0 )
		sigma_space = 1;

	double gauss_color_coeff = -0.5/(sigma_color*sigma_color);
	double gauss_space_coeff = -0.5/(sigma_space*sigma_space);

	if( d <= 0 )
		radius = cvRound(sigma_space*1.5);
	else
		radius = d/2;
	radius = MAX(radius, 1);
	d = radius*2 + 1;
	// compute the min/max range for the input image (even if multichannel)

	cv::minMaxLoc( src.reshape(1), &minValSrc, &maxValSrc );

	// temporary copy of the image with borders for easy processing
	Mat temp;
	copyMakeBorder( src, temp, radius, radius, radius, radius, borderType );
	patchNaNs(temp);

	// allocate lookup tables
	vector<float> _space_weight(d*d);
	vector<int> _space_ofs(d*d);
	float* space_weight = &_space_weight[0];
	int* space_ofs = &_space_ofs[0];

	// assign a length which is slightly more than needed
	len = (float)(maxValSrc - minValSrc) * cn;
	kExpNumBins = kExpNumBinsPerChannel * cn;
	vector<float> _expLUT(kExpNumBins+2);
	float* expLUT = &_expLUT[0];

	scale_index = kExpNumBins/len;

	// initialize the exp LUT
	for( i = 0; i < kExpNumBins+2; i++ )
	{
		if( lastExpVal > 0.f )
		{
			double val =  i / scale_index;
			expLUT[i] = (float)std::exp(val * val * gauss_color_coeff);
			lastExpVal = expLUT[i];
		}
		else
			expLUT[i] = 0.f;
	}

	// initialize space-related bilateral filter coefficients
	for( i = -radius, maxk = 0; i <= radius; i++ )
		for( j = -radius; j <= radius; j++ )
		{
			double r = std::sqrt((double)i*i + (double)j*j);
			if( r > radius )
				continue;
			space_weight[maxk] = (float)std::exp(r*r*gauss_space_coeff);
			space_ofs[maxk++] = (int)(i*(temp.step/sizeof(float)) + j*cn);
		}

		for( i = 0; i < size.height; i++ )
		{
			const float* sptr = (const float*)(temp.data + (i+radius)*temp.step) + radius*cn;
			float* dptr = (float*)(dst.data + i*dst.step);

			if( cn == 1 )
			{
				for( j = 0; j < size.width; j++ )
				{
					float sum = 0, wsum = 0;
					float val0 = sptr[j];
					for( k = 0; k < maxk; k++ )
					{
						float val = sptr[j + space_ofs[k]];
						float alpha = (float)(std::abs(val - val0)*scale_index);
						int idx = cvFloor(alpha);
						alpha -= idx;
						float w = space_weight[k]*(expLUT[idx] + alpha*(expLUT[idx+1] - expLUT[idx]));
						sum += val*w;
						wsum += w;
					}
					dptr[j] = (float)(sum/wsum);
				}
			}
			else
			{
				assert( cn == 3 );
				for( j = 0; j < size.width*3; j += 3 )
				{
					float sum_b = 0, sum_g = 0, sum_r = 0, wsum = 0;
					float b0 = sptr[j], g0 = sptr[j+1], r0 = sptr[j+2];
					for( k = 0; k < maxk; k++ )
					{
						const float* sptr_k = sptr + j + space_ofs[k];
						float b = sptr_k[0], g = sptr_k[1], r = sptr_k[2];
						float alpha = (float)((std::abs(b - b0) +
							std::abs(g - g0) + std::abs(r - r0))*scale_index);
						int idx = cvFloor(alpha);
						alpha -= idx;
						float w = space_weight[k]*(expLUT[idx] + alpha*(expLUT[idx+1] - expLUT[idx]));
						sum_b += b*w; sum_g += g*w; sum_r += r*w;
						wsum += w;
					}
					wsum = 1.f/wsum;
					b0 = sum_b*wsum;
					g0 = sum_g*wsum;
					r0 = sum_r*wsum;
					dptr[j] = b0; dptr[j+1] = g0; dptr[j+2] = r0;
				}
			}
		}
}


CTreatFilter0::CTreatFilter0()
{	
	InitializeCriticalSection(&m_sCriticalSection);

	m_nKernelSize	= 200;
	m_dSigma		= 20.0;
	m_dCoefDisplay	= 10.0f;
	m_uRegFlag		= 0;

	m_nFilterType = 0; // double gaussian by default
	m_dVarSigPt1_x = 100.0;
	m_dVarSigPt1_y = 10.0;
	//pr interp
	m_nInterpAreaWidth = 10;
}

CTreatFilter0::~CTreatFilter0()
{
	DeleteCriticalSection(&m_sCriticalSection);
}

bool CTreatFilter0::Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder)
{ 

	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("KernelSize"), m_nKernelSize))
		LogThis(1,3,Fmt(_T("{%s} Could not find [KernelSize] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmDbl(p_pPrmMap,_T("Sigma"), m_dSigma))
		LogThis(1,3,Fmt(_T("{%s} Could not find [Sigma] Parameter"), INanoTopoTreament::GetName()));

	if( ! FindTreatInitPrmDbl(p_pPrmMap,_T("CoefDisplay"), m_dCoefDisplay))
		LogThis(1,3,Fmt(_T("{%s} Could not find [CoefDisplay] Parameter"), INanoTopoTreament::GetName()));

	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("FilterType"), m_nFilterType))
		LogThis(1,3,Fmt(_T("{%s} Could not find [FilterType] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmDbl(p_pPrmMap,_T("FS2Px"), m_dVarSigPt1_x))
		LogThis(1,3,Fmt(_T("{%s} Could not find [FS2Px] Parameter"), INanoTopoTreament::GetName()));
	if( ! FindTreatInitPrmDbl(p_pPrmMap,_T("FS2Py"), m_dVarSigPt1_y))
		LogThis(1,3,Fmt(_T("{%s} Could not find [FS2Py] Parameter"), INanoTopoTreament::GetName()));

	m_uRegFlag = 0;
	if( ! GetRegistryFlag(m_uRegFlag, DBG_REC_HFILTER))
		LogThis(1,3,Fmt(_T("{%s} Could not reach Registry flag Parameter"), INanoTopoTreament::GetName()));

	if((m_uDbgFlag|m_uRegFlag) & DBG_SHOW_DISPLAY)
		cvNamedWindow( "Filter window", CV_WINDOW_NORMAL| CV_WINDOW_KEEPRATIO);// Create a window for display.

	//new pour Interp
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("InterpAreaWidth"), m_nInterpAreaWidth))
		LogThis(1,3,Fmt(_T("{%s} Could not find [InterpAreaWidth] Parameter"), INanoTopoTreament::GetName()));
	int nb = 0;
	if( ! FindTreatInitPrmInt(p_pPrmMap,_T("InterpEnabled"), nb))
		LogThis(1,3,Fmt(_T("{%s} Could not find [InterpEnabled] Parameter"), INanoTopoTreament::GetName()));
	m_bIsInterpEnabled = (nb !=0);

	return true;
}

bool CTreatFilter0::Exec( const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm )
{
	double dstart = GetPerfTime();

	m_H.reset(new  H3_MATRIX_FLT32());
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
	if(FindTreatPrmPtr(p_InputsPrm,"VignStartPosX",p))
	{
		m_nVignStartPosX = *((int*) p); 	
	}
	if(FindTreatPrmPtr(p_InputsPrm,"VignStartPosY",p))
	{
		m_nVignStartPosY= *((int*) p); 	
	}
	if(FindTreatPrmPtr(p_InputsPrm,"VignSize",p))
	{
		m_nVignSize = *((int*) p); 	
	}

	shared_ptr<void> pvH;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"H",pvH))
	{
		m_H = static_pointer_cast<H3_MATRIX_FLT32> (pvH);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("H Size = %d x %d"), m_H->GetCo(),m_H->GetLi()));
	}
	pvH.reset();

	shared_ptr<H3_MATRIX_UINT8> oMaskErode;
	oMaskErode.reset(new H3_MATRIX_UINT8());
	shared_ptr<void> pvMaskErode;
	if(FindTreatPrmSharedPtr(p_InputsPrm,"MaskE",pvMaskErode))
	{
		oMaskErode = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskErode);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
			LogThis(1,1,Fmt(_T("Mask Erode Size = %d x %d"), oMaskErode->GetCo(),oMaskErode->GetLi()));
	}
	pvMaskErode.reset();
	Mat oCVMatMaskE	=  Mat(oMaskErode->GetLi(),	oMaskErode->GetCo(),	CV_8U,	oMaskErode->GetData(),	Mat::AUTO_STEP);
	Mat oCVMatMaskDist;
	if(m_nFilterType != 0)
	{
		oCVMatMaskE.copyTo(oCVMatMaskDist);
		// on erode de 1 le mask de base
		Mat elementErode = getStructuringElement( MORPH_RECT,	Size( 3, 3 ),	Point( 1, 1 ) );
		erode( oCVMatMaskE, oCVMatMaskE, elementErode );
	}

	Mat oMskE32f;
	oCVMatMaskE.convertTo(oMskE32f,CV_32F);
	Mat oCVMatMaskEInv;
	bitwise_not(oCVMatMaskE,oCVMatMaskEInv);
	oCVMatMaskEInv = oCVMatMaskEInv - 254; // pour ramener la dynamix à 0 - 1
	Mat oInvMskE32f;
	oCVMatMaskEInv.convertTo(oInvMskE32f,CV_32F);

	if(pSavData && (uDbgFlag & DBG_REC_H_IN))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("HmToTreat");
		elt._spT.reset(new H3_MATRIX_FLT32(*m_H.get())); // copy since data altered in following treatments
		elt._bImg = true;
		elt._bAutoScale = true;
		elt._fMin = (float) FLT_MAX;
		elt._fMax = (float) FLT_MAX;
		elt._bHbf = false;
		elt._bBin = true;
		pSavData->spListF32.push_back(elt);
	}
	
	// Init new Hf
	m_Hf.reset(new  H3_MATRIX_FLT32(m_H->GetLi(),  m_H->GetCo()));

	Mat oCVHm		=  Mat(m_H->GetLi(),	m_H->GetCo(),	CV_32F,	m_H->GetData(),		Mat::AUTO_STEP);
	m_oCVHf			=  Mat(m_Hf->GetLi(),	m_Hf->GetCo(),	CV_32F,	m_Hf->GetData(),	Mat::AUTO_STEP);
	
	double dssStartFilter = 0.0;
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		dssStartFilter = GetPerfTime();
		LogThis(1,1,Fmt(_T("##### Start Filter")));
	}

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	Mat oOut;
	if(m_nFilterType == 0) 
	{
		//
		// Double gaussian filter
		//
		Mat kernelGaussCoef = getGaussianKernel(2*m_nKernelSize+1, m_dSigma, CV_32F);
		sepFilter2D(oCVHm,oOut,CV_32F,kernelGaussCoef,kernelGaussCoef);
	}
	else 
	{	
		//
		// filter shrinking - (separable 2D filter paralize algorithm)
		//

		/*	// commented code use for debug if we want to check distance matrix
			shared_ptr<H3_MATRIX_FLT32> ospDist;
			ospDist.reset(new  H3_MATRIX_FLT32());
			ospDist->Copy(*m_H);
			Mat dist =  Mat(ospDist->GetLi(),	ospDist->GetCo(),	CV_32F,	ospDist->GetData(),	Mat::AUTO_STEP); */

		Mat dist;  // matrix of euclidien distance from mask border
		//compute distance matrix
		distanceTransform(oCVMatMaskDist, dist, CV_DIST_L2, 3);

		int i, j;
		vector<Mat> kernelCoefArray;
		// Compute vector of gaussian kernels for each distance from kernel size to 1 (0 kernel is compute but not use, it is mainly for index correspondance)
		
		//calcul des coefcient de bezier
		//point de controle
		//P0 (0,0.05) ; P1(x,y), P2(m_nKernelSize,SIGMA)
		double dP0x = 0.0;
		double dP0y = 0.05;
		double dP1x = m_dVarSigPt1_x; //50;
		double dP1y = m_dVarSigPt1_y; //30;//m_dSigma;
		double dP2x = m_nKernelSize;
		double dP2y = m_dSigma;
		double dtStep = 0.00001;

		vector<double> vB_X;
		vector<double> vB_Y;
		for(double t=0.0; t<=1.0; t+=dtStep)
		{
			double dx = dP0x * pow((1-t),2) + dP1x * 2.0 * t * (1-t) + dP2x * pow(t,2);
			if(dx < 0.0) dx = 0.0;
			if(dx > dP2x) dx = dP2x;
			vB_X.push_back( dx );

			double dy = dP0y * pow((1-t),2) + dP1y * 2.0 * t * (1-t) + dP2y * pow(t,2);
			if(dy < 0.0) dy = 0.0;
			if(dy > dP2y) dy = dP2y;
			vB_Y.push_back( dy );
		}
		
		int nINDEX_BEZIER = 0;
		
		for (int k=0; k<m_nKernelSize; k++)
		{
			if(m_nFilterType == 1)	// fixed sigma
				kernelCoefArray.push_back( getGaussianKernel(2*k+1, m_dSigma, CV_32F));
			else					// variable sigma
			{	
				//kernelCoefArray.push_back( getGaussianKernel(2*k+1, (2*k+1)/m_dSigma, CV_32F)); // Note de RT : it work at this time because kernel = 10 * sigma -- may be found another variation
				//kernelCoefArray.push_back( getGaussianKernel(2*k+1, ((2*k+1)/(2*m_nKernelSize+1)) * m_dSigma, CV_32F)); 
				//kernelCoefArray.push_back( getGaussianKernel(2*k+1, 0.1 * k + 0.05, CV_32F)); 
				double dx = vB_X[nINDEX_BEZIER];
				while( (dx < (double) k) && (nINDEX_BEZIER < vB_Y.size()) ) 
				{	
					nINDEX_BEZIER++;
					if(nINDEX_BEZIER < vB_X.size())
						dx = vB_X[nINDEX_BEZIER];
				}
				
				double dy = 0.0;
				if(nINDEX_BEZIER >= vB_Y.size())
					dy =  vB_Y[vB_Y.size() - 1];
				else
				{
					dy = vB_Y[nINDEX_BEZIER];
					nINDEX_BEZIER++;
				}
				kernelCoefArray.push_back( getGaussianKernel(2*k+1, dy, CV_32F));
//				LogThis(1,2,Fmt(_T("# K = %d ---- Kpp = %f +++ SIGMA = %f"), k, dx ,dy));
			}
		}
		kernelCoefArray.push_back(getGaussianKernel(2*m_nKernelSize+1, m_dSigma, CV_32F)); // max kernel should be always à sigma

		Mat oOutAux;
		oCVHm.copyTo(oOutAux); // init first pass matrix
		oCVHm.copyTo(oOut);	   // init second pass and final matrix

		// First pass - Horizontal 
		#pragma omp parallel private(j)
		{
			#pragma omp for schedule (dynamic)
			for (i = 0; i<dist.rows; i++)
			{
				for (j = 0; j<dist.cols; j++)
				{
					int nK = (int) floor(dist.at<float>(i,j));
					if( nK > 0)
					{
						if(nK > m_nKernelSize)
							nK = m_nKernelSize;
						double dSum = 0.0;
						for (int x = -nK; x<=nK; x++)
						{
							if ( (i+x < 0) || (i+x >= oCVHm.rows)) // avoid crash near bound limits
								continue;
							dSum += (kernelCoefArray[nK]).at<float>(nK+x) * oCVHm.at<float>(i+x,j);	// used oCVHm ! do not overwrite data otherwise result will be wrong 
						}
						oOutAux.at<float>(i,j) = (float) dSum;
					}
				}
			}
		}

		// Second pass - Vertical 
		#pragma omp parallel private(i)
		{
			#pragma omp for schedule (dynamic)
			for (j = 0; j<dist.cols; j++)
			{
				for (i = 0; i<dist.rows; i++)
				{
					int nK = (int) floor(dist.at<float>(i,j));
					if( nK > 0)
					{
						if(nK > m_nKernelSize)
							nK = m_nKernelSize;
						double dSum = 0.0;
						for (int y = -nK; y<=nK; y++)
						{
							if ( (j+y < 0) || (j+y >= oCVHm.cols)) // avoid crash near bound limits
								continue;
							dSum += (kernelCoefArray[nK]).at<float>(nK+y) * oOutAux.at<float>(i,j+y);	// used Auxilary matrix oOutAux ! do not overwrite data otherwise result will be wrong  			
						}
						oOut.at<float>(i,j) = (float) dSum;
					}
					else
						oOut.at<float>(i,j) = 0.0f;
				}
			}
		}
	}

	//
	// Finalyze filtering 
	//
	m_oCVHf = oCVHm - oOut;
	oOut.release();
	oCVHm.release();
	m_H.reset();

	if (m_bIsInterpEnabled)
	{
		double dstart = GetPerfTime();
		Interp(oCVMatMaskE);
		if(uDbgFlag & DBG_SHOWDEBUG_LOG)
		{
			double dEnd = GetPerfTime();
			LogThis(1,2,Fmt(_T("### Interpolation done in %lf ms"), dEnd-dstart));
		}
	}

	// filtre lisseur
// 	double dsig=1.0;
// 	LogThis(1,2,Fmt(_T("###****** gaussian filtering [%0.1lf %0.1lf]"),2.0*3.0*dsig+1.0,dsig));
// 	double dstart2= GetPerfTime();
//  	Mat kernelGaussCoef3 = getGaussianKernel(2.0*3.0*dsig+1.0, dsig, CV_32F);
// 	sepFilter2D(m_oCVHf,m_oCVHf,CV_32F,kernelGaussCoef3,kernelGaussCoef3);
// 	double dEnd2 = GetPerfTime();
// 	LogThis(1,2,Fmt(_T("###****** gaussian filtering done in %lf ms"), dEnd2-dstart2));
// 	
/*
	int k= 7;
	double sColor = 0.1;
	double sSpace = 1.0;
	LogThis(1,2,Fmt(_T("###****** Bilateral filtering [ %d %0.1lf %0.1lf]"),k,sColor,sSpace));
	double dstart1 = GetPerfTime();
	Mat htemp; 
	m_oCVHf.copyTo(htemp);
	bilateralFilter(m_oCVHf, htemp, k, sColor, sSpace);
	htemp.copyTo(m_oCVHf);
	double dEnd1 = GetPerfTime();
	LogThis(1,2,Fmt(_T("###****** Bilateral filtering done in %lf ms"), dEnd1-dstart1));*/

	m_oCVHf = oMskE32f.mul(m_oCVHf);

	double minpx, maxpx;
	minMaxLoc(m_oCVHf, &minpx, &maxpx); //find minimum and maximum intensities
	if(minpx > 0.0) minpx = 0.0;
	Mat oMins = Mat(m_Hf->GetLi(),m_Hf->GetCo(), CV_32F, Scalar(minpx-g_fMinZeroSub));
	m_oCVHf += oInvMskE32f.mul(oMins);

// 	// Write RIN files
// 	cv::Point2f pt(1805+30, 1622+200);
// 	double dAngle = -270.0;
// 	cv::Mat r = cv::getRotationMatrix2D(pt, dAngle, 1.0);
// 	cv::warpAffine(m_oCVHf, m_oCVHf, r, cv::Size(m_oCVHf.cols, m_oCVHf.rows));
// 	CString css = Fmt("D:\\Altasight\\Data\\Data_altattech_wafer_SEH_01-2014\\refBS-ROT\\res\\%d.RIN",(int)(-dAngle));
// 	bool bRes = false;
// 	FILE* pFile = 0;
// 	if(fopen_s(&pFile, (LPCSTR)css,"wb+") == 0)
// 	{
// 		bRes = m_Hf->fSaveHBF(pFile);
// 		fclose(pFile);
// 	}
// 	if( ! bRes)
// 		LogThis(1,4,Fmt(_T("============> Fail to save <%s>  "), css));
// 	else
// 		LogThis(1,1,Fmt(_T("============> Save <%s>  "), css));


	// COMPUTE MULTI ANGLE ADN FROM RIN FILES
// 	H3_MATRIX_FLT32 o0;
// 	H3_MATRIX_FLT32 o90;
// 	H3_MATRIX_FLT32 o180;
// 	H3_MATRIX_FLT32 o270;
// 	bool bRes = false;
// 	CString css;
// 	FILE* pFile = 0;
// 	double dAngle = 0.0;
// 	css = Fmt("D:\\Altasight\\Data\\Data_altattech_wafer_SEH_01-2014\\refBS-ROT\\res\\%d.RIN",(int)(-dAngle));
// 	if(fopen_s(&pFile, (LPCSTR)css,"rb+") == 0)
// 	{
// 		bRes = o0.fLoadHBF(pFile);
// 		fclose(pFile);
//  	}
// 	if( ! bRes)
// 		LogThis(1,4,Fmt(_T("============> Fail to load <%s>  "), css));
// 	else
// 		LogThis(1,1,Fmt(_T("============> Save <%s>  "), css));
// 	dAngle = -90.0;
// 	css = Fmt("D:\\Altasight\\Data\\Data_altattech_wafer_SEH_01-2014\\refBS-ROT\\res\\%d.RIN",(int)(-dAngle));
// 	if(fopen_s(&pFile, (LPCSTR)css,"rb+") == 0)
// 	{
// 		bRes = o90.fLoadHBF(pFile);
// 		fclose(pFile);
// 	}
// 	if( ! bRes)
// 		LogThis(1,4,Fmt(_T("============> Fail to load <%s>  "), css));
// 	else
// 		LogThis(1,1,Fmt(_T("============> LOAD <%s>  "), css));
// 	dAngle = -180.0;
// 	css = Fmt("D:\\Altasight\\Data\\Data_altattech_wafer_SEH_01-2014\\refBS-ROT\\res\\%d.RIN",(int)(-dAngle));
// 	if(fopen_s(&pFile, (LPCSTR)css,"rb+") == 0)
// 	{
// 		bRes = o180.fLoadHBF(pFile);
// 		fclose(pFile);
// 	}
// 	if( ! bRes)
// 		LogThis(1,4,Fmt(_T("============> Fail to load <%s>  "), css));
// 	else
// 		LogThis(1,1,Fmt(_T("============> LOAD <%s>  "), css));
// 	dAngle = -270.0;
// 	css = Fmt("D:\\Altasight\\Data\\Data_altattech_wafer_SEH_01-2014\\refBS-ROT\\res\\%d.RIN",(int)(-dAngle));
// 	if(fopen_s(&pFile, (LPCSTR)css,"rb+") == 0)
// 	{
// 		bRes = o270.fLoadHBF(pFile);
// 		fclose(pFile);
// 	}
// 	if( ! bRes)
// 		LogThis(1,4,Fmt(_T("============> Fail to load <%s>  "), css));
// 	else
// 		LogThis(1,1,Fmt(_T("============> LOAD <%s>  "), css));
// 	Mat oCVH0		=  Mat(o0.GetLi(),	o0.GetCo(),	CV_32F,	o0.GetData(),		Mat::AUTO_STEP);
// 	Mat oCVH90		=  Mat(o90.GetLi(),	o90.GetCo(),	CV_32F,	o90.GetData(),		Mat::AUTO_STEP);
// 	Mat oCVH180		=  Mat(o180.GetLi(),	o180.GetCo(),	CV_32F,	o180.GetData(),		Mat::AUTO_STEP);
// 	Mat oCVH270		=  Mat(o270.GetLi(),	o270.GetCo(),	CV_32F,	o270.GetData(),		Mat::AUTO_STEP);
// 	m_oCVHf = (oCVH0 + oCVH90 + oCVH180 + oCVH270) / 4.0;


	/*H3_MATRIX_FLT32 oHfPresent(m_oCVHf.rows,m_oCVHf.cols);
	Mat m_oCVHfPresentation( m_oCVHf.rows, m_oCVHf.cols, CV_32F, oHfPresent.GetData(), Mat::AUTO_STEP);
	m_oCVHf.copyTo(m_oCVHfPresentation);

 	double minpx, maxpx;
 	minMaxLoc(m_oCVHfPresentation, &minpx, &maxpx); //find minimum and maximum intensities
 	if(minpx > 0.0) minpx = 0.0;
 	Mat oMins = Mat(oHfPresent.GetLi(),oHfPresent.GetCo(), CV_32F, Scalar(minpx-g_fMinZeroSub));
 	m_oCVHfPresentation += oInvMskE32f.mul(oMins);*/
	
	oMskE32f.release();
	oInvMskE32f.release();
	
	if(uDbgFlag & DBG_SHOWDEBUG_LOG)
	{
		double dssEndFilter = GetPerfTime();
		LogThis(1,1,Fmt(_T("# Done Filter in %lf ms"), dssEndFilter - dssStartFilter ));
	}

	double dminVal, dmaxVal;
	Scalar scmean;	Scalar scstddev;
	Mat ocv = m_oCVHf(Rect(m_Hf->GetCo()/2 - m_Hf->GetCo()/4,	m_Hf->GetLi()/2 - m_Hf->GetLi()/4,	m_Hf->GetCo()/2,	m_Hf->GetLi()/2));
	meanStdDev(ocv,scmean,scstddev);
	dmaxVal = m_dCoefDisplay * scstddev[0];
	dminVal = - dmaxVal;

	if (m_bEmergencyStop)
	{
		LogThis(1,3,Fmt(_T("{%s} Emergency Exit"), INanoTopoTreament::GetName()));
		return false;
	}

	if(pSavData && (uDbgFlag & DBG_REC_HFILTER))
	{
		tSpT<H3_MATRIX_FLT32> elt;
		elt._cs  = _T("Hfilter");
		elt._spT.reset(new H3_MATRIX_FLT32(*(m_Hf.get()))); // copy since data altered in following treatments
		
		elt._bImg = true;
		elt._bAutoScale = false;
		elt._fMin = (float) dminVal;
		elt._fMax = (float) dmaxVal;

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

		CWinThread* pThread = AfxBeginThread(&CTreatFilter0::SaveData, pSavData, THREAD_PRIORITY_NORMAL, 0, CREATE_SUSPENDED);
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
	if(FindTreatPrmPtr(p_OutputsPrm,"FilterType",p))
	{
		int* pFilterType = (int*) p;
		(*pFilterType) = m_nFilterType;
	}

	AddTreatPrmSharedPtr(p_OutputsPrm,"Hf",shared_ptr<void>(m_Hf));

	
	if(uDbgFlag & DBG_SHOW_DISPLAY)
	{
		Mat draw;
		m_oCVHf.convertTo(draw, CV_8U, 255.0/(dmaxVal - dminVal), - dminVal * 255.0f / (dmaxVal - dminVal));
		imshow( "Filter window", draw );
	}

// 	int nCrossYY = 1025;	
// 	int nCrossXXs = 350;
// 	int nCrossXXe = 650;
// 	CStdioFile oFile2;
// 	if (oFile2.Open(Fmt(_T("d:\\altasight\\vignFILTER.csv")), CFile::modeCreate | CFile::modeWrite))
// 	{
// 		for (int x=nCrossXXs; x<=nCrossXXe; x++)
// 		{
// 			float fX = (float)x;
// 			float fY = m_oCVHf.at<float>(nCrossYY,x);
// 			oFile2.WriteString(Fmt(_T("%lf;%lf;\n"),fX,fY));
// 		}
// 		oFile2.Close();
// 	}


	m_H.reset();
	m_Hf.reset();

	return true;
}

bool CTreatFilter0::SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
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

bool CTreatFilter0::SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin /*= INT_MAX*/, int p_nMax /*= INT_MAX*/, bool bAutoscale /*= true*/)
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

UINT CTreatFilter0::SaveData( void *p_pParameters )
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

HRESULT CTreatFilter0::QueryInterface( REFIID iid, void **ppvObject )
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

ULONG CTreatFilter0::AddRef( void )
{
	m_ulRefCount++;
	return m_ulRefCount;
}

ULONG CTreatFilter0::Release( void )
{
	m_ulRefCount--;
	if (m_ulRefCount!=0) 
		return m_ulRefCount;
	delete this;     // Destruction de l'objet.
	return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}

		


extern "C"  HRESULT Create_TreatFilter0( REFIID iid, void **ppvObject )
{
	CTreatFilter0 *pObj = new CTreatFilter0();
	if (pObj==0) 
		return E_OUTOFMEMORY;
	return pObj->QueryInterface(iid, ppvObject);
}


void CTreatFilter0::Interp(Mat& p_oMskE)
{	
	int i,j;
	int nStep = m_nVignSize / 2;
	float fnbInterpolPts = (float) 2*m_nInterpAreaWidth + 1;

	// parcours colonne par colonne
	#pragma omp parallel private(j)
	{
		#pragma omp for schedule (dynamic)
		for (i=m_nVignStartPosX;i<m_oCVHf.cols; i += nStep)
		{
			if( (i-m_nInterpAreaWidth) >= 0  && (i+m_nInterpAreaWidth)<m_oCVHf.cols )
			{
				for (j=m_nVignStartPosY; j<m_oCVHf.rows; j++)
				{
					if(j >= 0)
					{	
						// on teste si cette ligne est totalement include dans notre mask
						Scalar fsum = sum(p_oMskE(Rect(i-m_nInterpAreaWidth,j,(int)fnbInterpolPts,1)));
						if(fsum[0] == fnbInterpolPts)
						{
							// calcul coef directeur et ordonnée à l'origine
							float fPente = ( m_oCVHf.at<float>(j,i+m_nInterpAreaWidth) -  m_oCVHf.at<float>(j,i-m_nInterpAreaWidth) )/ fnbInterpolPts;
							float fOrigin =  m_oCVHf.at<float>(j,i-m_nInterpAreaWidth) - (fPente * float(i-m_nInterpAreaWidth) );	
							for (int k=(1+i-m_nInterpAreaWidth); k< (2*m_nInterpAreaWidth + i-m_nInterpAreaWidth); k++)
							{
								m_oCVHf.at<float>(j,k) = fPente * (float)k + fOrigin;
							}
						}
					}
				}
			}
		}
	}

	// parcours ligne par ligne
	#pragma omp parallel private(i)
	{
		#pragma omp for schedule (dynamic)
		for (j=m_nVignStartPosY; j<m_oCVHf.rows; j += nStep)
		{
			if( (j-m_nInterpAreaWidth) >= 0  && (j+m_nInterpAreaWidth)<m_oCVHf.rows )
			{		
				for (i=m_nVignStartPosX;i<m_oCVHf.cols; i++)
				{
					if(i >= 0)
					{	
						// on teste si cette ligne est totalement include dans notre mask
						Scalar fsum = sum(p_oMskE(Rect(i,j-m_nInterpAreaWidth,1,(int)fnbInterpolPts)));
						if(fsum[0] == fnbInterpolPts)
						{
							// calcul coef directeur et ordonnée à l'origine
							float fPente = ( m_oCVHf.at<float>(j+m_nInterpAreaWidth,i) -  m_oCVHf.at<float>(j-m_nInterpAreaWidth,i) )/ fnbInterpolPts;
							float fOrg =  m_oCVHf.at<float>(j-m_nInterpAreaWidth,i) - (fPente * float(j-m_nInterpAreaWidth) );	
							for (int k=(1+j-m_nInterpAreaWidth); k< (2*m_nInterpAreaWidth + j-m_nInterpAreaWidth); k++)
							{
								m_oCVHf.at<float>(k,i) = fPente * (float)k + fOrg;
							}
						}
					}
				}
			}
		}
	}
}