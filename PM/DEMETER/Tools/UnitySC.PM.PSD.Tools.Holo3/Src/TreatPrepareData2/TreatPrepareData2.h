#pragma once

#ifdef NT_EXPORTS
	#define NT_DLL  __declspec(dllexport)
#else
	#define NT_DLL  __declspec(dllimport)
#endif

#include "TreatmentInterface.h"
#include "H3Matrix.h"
#include <list>

#include <opencv2/core/core.hpp>

class NT_DLL CTreatPrepareData2 :	public INanoTopoTreament,
									public CWinThread
{

public :
	template < typename T >
	struct tSpT
	{
		CString _cs;
		shared_ptr<typename T> _spT;
		bool _bImg;
		bool _bHbf;
		bool _bBin;
		bool _bAutoScale;
		float _fMin;
		float _fMax;
	};

	struct tData2Save 
	{
		UINT nId;
		CString csName;
		CString csPath;
		list < tSpT<H3_MATRIX_UINT8> > spListU8;
		list < tSpT<H3_MATRIX_FLT32> > spListF32;
	};

private:
	struct Pt2Int
	{
		int x;
		int y;
	};

	struct tProlongData
	{
		void*	 _TreatPtr;
		cv::Mat  _Dist;
		cv::Mat  _Nx;
		cv::Mat  _Ny;
		cv::Mat  _Mask;
		cv::Mat  _MaskInv;
		HANDLE  _HEvent;
	};

public:
	CTreatPrepareData2();

	virtual ~CTreatPrepareData2();

	HRESULT STDMETHODCALLTYPE	QueryInterface(REFIID iid, void **ppvObject);

	ULONG STDMETHODCALLTYPE		AddRef(void);

	ULONG STDMETHODCALLTYPE		Release(void);

	virtual bool Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder);

	virtual bool Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm);

	static bool SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	static bool SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin = INT_MAX, int p_nMax = INT_MAX, bool bAutoscale = true);

	HANDLE m_hEventThDone[2];
	HANDLE m_hEventPrepDone[2];
	HANDLE m_hFringeKillerErodeDone;


private :
	static UINT SaveData(void *p_pParameters);

	static UINT static_ErodeCV(void *p_pParameters);
	void ErodeCV();
	
	static UINT static_DilateCV(void *p_pParameters);
	void DilateCV();

	static UINT static_PerformX(void *p_pParameters);
	void PerformPreparation_X();
	void FringeKillerNX(double p_dMin);

	static UINT static_PerformY(void *p_pParameters);
	void PerformPreparation_Y();
	void FringeKillerNY(double p_dMin);

	bool Prolonge_Bord();
	static UINT static_ProlongeBordVignette(void *p_pParameters);
	bool ProlongeBordVignette(cv::Mat& p_SubDist, cv::Mat& p_SubNx, cv::Mat& p_SubNy, cv::Mat& p_oSubMask, cv::Mat& p_oSubMaskInv, HANDLE p_hEvenDone);

	bool Prolonge_Bord_Polar();
	bool Prolonge_Bord_PolarCompute(cv::Mat& image, int nAngleSizeX, double dAngleStep, int nRadiusSizeY, double dRadiusMax, double dRadiusMin, double dCenterX, double dCenterY);
	inline int _NArrondi(double d){return (int)(d<0.0 ? d - 0.5 : d + 0.5);}

	void LoadRedressMatrix(int p_nOffset_px, const CString& calibFolder);
	void RedressMatrix(cv::Mat& p_MatToRedress);
	cv::Mat m_Dist;
	cv::Mat m_cvNX;
	cv::Mat m_cvNY;
	cv::Mat m_cvRedressTransform;
	cv::Mat m_cvMaskOriginal;

	double m_dDistStop;
	int	   m_nMeanVignHalfSize;
	float  m_fNoiseLow;
	float  m_fNoiseHi;
	CString	m_csLotID;

	double m_dFK_PrmX[5];		// Fringe Killer parameters String on X
	double m_dFK_PrmY[5];		// Fringe Killer parameters String on Y

	int		m_nPrmErodeRadius;
	int		m_nPrmDilateRadius;
	int		m_nPrmSmoothKernelSize;
	bool    m_bUseFringeKiller;
	unsigned long	m_uRegFlag;

	std::vector<cv::Rect> m_vProlongeAreas;

	shared_ptr<H3_MATRIX_FLT32> m_matNX;
	shared_ptr<H3_MATRIX_FLT32> m_matNY;
	shared_ptr<H3_MATRIX_FLT32> m_matPX;
	shared_ptr<H3_MATRIX_FLT32> m_matPY;

	shared_ptr<H3_MATRIX_UINT8> m_matMask;

	shared_ptr<H3_MATRIX_UINT8> m_Erode;
	shared_ptr<H3_MATRIX_UINT8> m_Dilate;

	CRITICAL_SECTION m_sCriticalSection;

	int m_nCalibInvertNX;
	int m_nCalibInvertNY;
	int m_nCalibSwitchNi;

	int m_nOffsetExpandX;
	int m_nOffsetExpandY;
	int m_nNUIEnable;
	int m_nNUIOrder;
	int m_nNUIStep;
	int m_nNUIFib;
	int m_nNUIErode;

	double m_dProlongePolarRdxMin;
	double m_dProlongePolarRdxMax;

	tData2Save *m_pSaveData;



	bool H3BestFitSurf(const H3_MATRIX_FLT32& Src, const H3_MATRIX_UINT8& SrcMask, H3_MATRIX_FLT64 & MatResSurf, long FitOrder);
	bool H3BestFitSurf(const H3_MATRIX_FLT32& Src, const H3_MATRIX_UINT8& SrcMask, H3_MATRIX_FLT64 & MatResSurf, long FitOrder,long MatVal, const H3_MATRIX_UINT8& MatCoef);
	bool Degauchi(H3_MATRIX_FLT32 *pMatScr, H3_MATRIX_UINT8* pMaskE );

};

extern "C"  NT_DLL HRESULT Create(REFIID iid, void **ppvObject);




