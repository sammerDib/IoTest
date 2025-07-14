#pragma once

#include "TreatmentInterface.h"
#include "H3Matrix.h"
#include <list>

#include <opencv2/core/core.hpp>

#define NB_QUADRANT 4

struct tVignetteRes
{
	bool _bUsed;
	bool _bComputed;
	int	 _nPosX;
	int  _nPosY;
	cv::Mat _Z;
};

class CTreatReconstruct1 : public INanoTopoTreament,
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
		float _fMin;
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
	CTreatReconstruct1();

	virtual ~CTreatReconstruct1();

	HRESULT STDMETHODCALLTYPE	QueryInterface(REFIID iid, void **ppvObject);

	ULONG STDMETHODCALLTYPE		AddRef(void);

	ULONG STDMETHODCALLTYPE		Release(void);

	virtual bool Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder);

	virtual bool Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm);

	static bool SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	static bool SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin = INT_MAX, int p_nMax = INT_MAX, bool bAutoscale = true);

	HANDLE m_hEventQDone[4];	// Event indicate the end of thread quadrant
	HANDLE m_hEventQStart[4];	// Event starting thread quadrant
	HANDLE m_hEventBandDone[2];		// Event indicate the end of thread Band
	HANDLE m_hEventHalfBandDone[2];

private :
	static UINT SaveData(void *p_pParameters);

	cv::Mat AffineTransform(const cv::Mat& p_oNNX,const cv::Mat& p_oNNY);
	void ComputeVignette( int p_nIdxX,int p_nIdxY, const cv::Mat& p_oNNX, const cv::Mat& p_oNNY, cv::Mat& p_oMM, tVignetteRes& p_oVign);
	void RecolleVignette( tVignetteRes& p_ov, int p_nVoisType);
	
	static UINT DoRecolRightBand( void *p_pParameters );
	static UINT DoRecolLeftBand( void *p_pParameters );
	static UINT DoRecolQ1( void *p_pParameters );
	static UINT DoRecolQ2( void *p_pParameters );
	static UINT DoRecolQ3( void *p_pParameters );
	static UINT DoRecolQ4( void *p_pParameters );

	shared_ptr<H3_MATRIX_FLT32> m_matNX;
	shared_ptr<H3_MATRIX_FLT32> m_matNY;
	shared_ptr<H3_MATRIX_UINT8> m_matMaskDilate;
	shared_ptr<H3_MATRIX_UINT8> m_matMaskErode;

	shared_ptr<H3_MATRIX_FLT32> m_CoefMat;
	shared_ptr<H3_MATRIX_FLT32> m_H;

	cv::Mat m_oCVMatMaskD;	//Dilate Mask
	cv::Mat m_oCVMatNX;
	cv::Mat m_oCVMatNY;
	cv::Mat m_oCVCoefMat;

	cv::Mat m_oRg;
	cv::Mat m_oIterMapg;

	long	m_lOriginalWidth_px;
	long	m_lOriginalHeight_px;
	int m_nVoisPos[8][2]; //[][0]=> X | [][1]=> Y

	long	m_lVignetteSize;
	int		m_nMargin;
	int		m_nVois;

	double	m_dSigma;
	double	m_dAlpha;
	unsigned long	m_uRegFlag;
	CString	m_csLotID;

	CRITICAL_SECTION m_sCriticalSection;
};

extern "C"  HRESULT Create_TreatReconstruct1(REFIID iid, void **ppvObject);




