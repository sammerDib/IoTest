#pragma once

#include "TreatmentInterface.h"
#include "H3Matrix.h"
#include <list>

#include <opencv2/core/core.hpp>

class CTreatGenerateResults0 : public INanoTopoTreament,
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

	struct tPtFlt
	{
		float _x;
		float _y;
	};

public:
	CTreatGenerateResults0();

	virtual ~CTreatGenerateResults0();

	HRESULT STDMETHODCALLTYPE	QueryInterface(REFIID iid, void **ppvObject);

	ULONG STDMETHODCALLTYPE		AddRef(void);

	ULONG STDMETHODCALLTYPE		Release(void);

	virtual bool Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder);

	virtual bool Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm);

	static bool SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	static bool SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin = INT_MAX, int p_nMax = INT_MAX, bool bAutoscale = true);

	HANDLE m_hEventPVDone[2];	// Event end thread PV (10-2)

private :
	static UINT SaveData(void *p_pParameters);

	static UINT DoPV10(void *p_pParameters);
	static UINT DoPV2(void *p_pParameters);
	void MakeMaskPV(cv::Mat& p_oMaskPv, int p_nrsz);
	void DoPV(cv::Mat& p_oPv, int p_nrsz, float p_fLimit, cv::Mat& p_oMaskPv, std::list<tPtFlt>& p_oLstPv, float& p_fTHA, HANDLE& p_hEvt);
	void SaveADNFile(CString p_csOutPath, bool  p_bCompressData);
	void SaveStats(CString p_csOutPath);
	void ComputeSitePV(CString p_csOutPath);

	shared_ptr<H3_MATRIX_UINT8> m_MaskE;
	shared_ptr<H3_MATRIX_FLT32> m_Hf;
	shared_ptr<H3_MATRIX_FLT32> m_PV10;
	shared_ptr<H3_MATRIX_FLT32> m_PV2;

	cv::Mat m_oCVMatMaskE;
	cv::Mat m_oCVHf;	
	cv::Mat m_oCVPV10;
	cv::Mat m_oCVMaskDisk10;
	cv::Mat m_oCVPV2;
	cv::Mat m_oCVMaskDisk2;
	cv::Mat m_oCVHf2sav;	

	std::list<tPtFlt> m_oLstPv2;
	float m_fTHA2;
	std::list<tPtFlt> m_oLstPv10;
	float m_fTHA10;

	int 		 m_nPrmErodeRadius;
	float		 m_fPixelSize;		 // exprimé en micron µ (taille d'un pixel en micron)
	float		 m_fLimitCoef;		// [0.0f - 100.0f]
	bool		 m_bUseDiskPV;
	int			 m_nMaxPVDisplay;
	unsigned long m_uRegFlag;
	int			 m_nCurvePts;
	int			 m_nLissageSize;
	CString		m_csLotID;
	float		m_fEdgeExclusion_mm;
	CRITICAL_SECTION m_sCriticalSection;
	
	int			m_nFilterType;
	int			m_nOffsetExpandX;
	int			m_nOffsetExpandY;
	CString		m_csFoupID;
	float		m_fPV10HiValue_Thresh_nm;
	float		m_fPV2HiValue_Thresh_nm;
	bool		m_bUseMaskPVData;
	bool		m_bUseDataCompression;
	bool		m_bUseResumeLotStats;

	bool	m_bUseSitePVMap;
	float	m_fSiteWidthmm;
	float	m_fSiteHeightmm;
	float	m_fSiteOffsetXmm;
	float	m_fSiteOffsetYmm;
	float	m_fSiteThresh1nm;
	float	m_fSiteThresh2nm;
	float	m_fSiteLimitsFactor;
	float	m_fSiteTxtFactor;

	float m_fPeak;
	float m_fPV;
	float m_fRMS;

};

extern "C"  HRESULT Create_TreatGenerateResults0(REFIID iid, void **ppvObject);




