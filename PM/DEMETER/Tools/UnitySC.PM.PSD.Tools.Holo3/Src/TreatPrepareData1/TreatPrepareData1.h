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

class NT_DLL CTreatPrepareData1 :	public INanoTopoTreament,
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
	CTreatPrepareData1();

	virtual ~CTreatPrepareData1();

	HRESULT STDMETHODCALLTYPE	QueryInterface(REFIID iid, void **ppvObject);

	ULONG STDMETHODCALLTYPE		AddRef(void);

	ULONG STDMETHODCALLTYPE		Release(void);

	virtual bool Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder);

	virtual bool Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm);

	static bool SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	static bool SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin = INT_MAX, int p_nMax = INT_MAX, bool bAutoscale = true);

	HANDLE m_hEventThDone[2];


private :
	static UINT SaveData(void *p_pParameters);

	static UINT static_ErodeCV(void *p_pParameters);
	void ErodeCV();
	
	static UINT static_DilateCV(void *p_pParameters);
	void DilateCV();

	bool FringeKillerNX(double p_dMin, int p_na, int p_nb1, int p_nb2, int p_nHThresh);
	bool FringeKillerNY(double p_dMin, int p_na1, int p_na2, int p_nb, int p_nHThresh);

	bool Prolonge_Bord();
	static UINT static_ProlongeBordVignette(void *p_pParameters);
	bool ProlongeBordVignette(cv::Mat& p_SubDist, cv::Mat& p_SubNx, cv::Mat& p_SubNy, cv::Mat& p_oSubMask, cv::Mat& p_oSubMaskInv, HANDLE p_hEvenDone);

	cv::Mat m_Dist;
	cv::Mat m_cvNX;
	cv::Mat m_cvNY;
	double m_dDistStop;
	int	   m_nMeanVignHalfSize;
	float  m_fNoiseLow;
	float  m_fNoiseHi;
	CString	m_csLotID;

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
};

extern "C"  NT_DLL HRESULT Create(REFIID iid, void **ppvObject);




