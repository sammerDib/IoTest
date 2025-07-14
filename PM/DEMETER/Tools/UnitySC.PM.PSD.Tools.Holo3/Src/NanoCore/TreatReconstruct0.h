#pragma once

#include "TreatmentInterface.h"
#include "H3Matrix.h"
#include <list>

#include <opencv2/core/core.hpp>

#define NB_QUADRANT 4


class CTreatReconstruct0 : public INanoTopoTreament,
						   public CWinThread
{
	
public:
	template < typename T >
	struct tSpT
	{
		CString _cs;
		shared_ptr<typename T> _spT;
		bool _bImg;
		bool _bHbf;
		bool _bBin;
	};

	struct tData2Save 
	{
		UINT nId;
		CString csName;
		CString csPath;
		list < tSpT<H3_MATRIX_UINT8> > spListU8;
		list < tSpT<H3_MATRIX_FLT32> > spListF32;
	};

public:
	CTreatReconstruct0();

	virtual ~CTreatReconstruct0();

	HRESULT STDMETHODCALLTYPE	QueryInterface(REFIID iid, void **ppvObject);

	ULONG STDMETHODCALLTYPE		AddRef(void);

	ULONG STDMETHODCALLTYPE		Release(void);

	virtual bool Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder);

	virtual bool Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm);

	static bool SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	static bool SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin = INT_MAX, int p_nMax = INT_MAX, bool bAutoscale = true);

	HANDLE m_hEventQDone[4];	// Event end thread quadrant
	HANDLE m_hEventBandDone[4];	// Event end thread Band


private :
	static UINT SaveData(void *p_pParameters);

	static UINT DoQ1(void *p_pParameters);
	static UINT DoQ2(void *p_pParameters);
	static UINT DoQ3(void *p_pParameters);
	static UINT DoQ4(void *p_pParameters);
	void DoQuadrant(
		int		p_nQID,
		int		p_nStepX,
		int		p_nStepY,
		int		p_nFirstVignPosInRoi_X,
		int		p_nFirstVignPosInRoi_Y,
		HANDLE& p_hEvt);

	static UINT DoBand12( void *p_pParameters );
	static UINT DoBand34( void *p_pParameters );
	static UINT DoBand13( void *p_pParameters );
	static UINT DoBand24( void *p_pParameters );
	void DoBand(
		int		p_nBandID,
		int		p_nStepX,
		int		p_nStepY,
		int		p_nFirstVignPosInRoi_X,
		int		p_nFirstVignPosInRoi_Y,
		HANDLE& p_hEvt);
	void DoMiddle();

	cv::Mat AffineTransform(cv::Mat& p_oNNX, cv::Mat& p_oNNY);
	void ComputeVignette(int p_nIdxX,int p_nIdxY, cv::Mat& p_oNNX,cv::Mat& p_oNNY,cv::Mat& p_oMM,cv::Mat& p_oR,cv::Mat& p_oIterMap, int p_nVoisX, int p_nVoisY);
	
	shared_ptr<H3_MATRIX_FLT32> m_matNX;
	shared_ptr<H3_MATRIX_FLT32> m_matNY;
	shared_ptr<H3_MATRIX_UINT8> m_matMaskDilate;
	shared_ptr<H3_MATRIX_UINT8> m_matMaskErode;

	shared_ptr<H3_MATRIX_FLT32> m_CoefMat;

	shared_ptr<H3_MATRIX_FLT32> m_R[NB_QUADRANT];
	shared_ptr<H3_MATRIX_FLT32> m_IterationMap[NB_QUADRANT];

	shared_ptr<H3_MATRIX_FLT32> m_H;

	cv::Mat m_oCVMatMaskD;	//Dilate Mask
	cv::Mat m_oCVMatNX;
	cv::Mat m_oCVMatNY;
	cv::Mat m_oCVCoefMat;
	cv::Mat m_oCVR[NB_QUADRANT];
	cv::Mat m_oCVIterationMap[NB_QUADRANT];
	cv::Mat m_oCVBandR[4];
	cv::Mat m_oCVBandIterationMap[4];
	cv::Mat m_oCVMidR;
	cv::Mat m_oCVMidIterationMap;

	long	m_lOriginalWidth_px;
	long	m_lOriginalHeight_px;
	long	m_lVignetteSize;
	int		m_nMargin;
	int		m_nVois;
	double	m_dSigma;
	double	m_dAlpha;
	unsigned long m_uRegFlag;
	CString	m_csLotID;

	CRITICAL_SECTION m_sCriticalSection;
};

extern "C"  HRESULT Create_TreatReconstruct0(REFIID iid, void **ppvObject);




