#pragma once

#include "TreatmentInterface.h"
#include "H3Matrix.h"
#include <list>

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include "opencv2/imgproc/imgproc.hpp"
using namespace cv;


#include <opencv2/core/core.hpp>

class CTreatFilter0 : public INanoTopoTreament,
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

public:
	CTreatFilter0();

	virtual ~CTreatFilter0();

	HRESULT STDMETHODCALLTYPE	QueryInterface(REFIID iid, void **ppvObject);

	ULONG STDMETHODCALLTYPE		AddRef(void);

	ULONG STDMETHODCALLTYPE		Release(void);

	virtual bool Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder);

	virtual bool Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm);

	static bool SaveGreyImageFlt32(CString p_csFilepath, shared_ptr<H3_MATRIX_FLT32> p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);
	static bool SaveGreyImageUInt8(CString p_csFilepath, shared_ptr<H3_MATRIX_UINT8> p_oMatrix, int p_nMin = INT_MAX, int p_nMax = INT_MAX, bool bAutoscale = true);

	void Interp(Mat& p_oMskE);

private :
	static UINT SaveData(void *p_pParameters);

	shared_ptr<H3_MATRIX_FLT32> m_H;
	shared_ptr<H3_MATRIX_FLT32> m_Hf;

	int					m_nInterpAreaWidth;//interp
	bool				m_bIsInterpEnabled;

	int					m_nKernelSize;
	double				m_dSigma;
	double				m_dCoefDisplay;
	unsigned long		m_uRegFlag;
	CString				m_csLotID;
	int					m_nFilterType; // 0:  double gaussian; 1: FilterShrinking Fixed sigma; 2:  1: FilterShrinking variable sigma
	double				m_dVarSigPt1_x; // X coord pt de controle de la courbe de bezier de variation de sigma 
	double				m_dVarSigPt1_y; // Y coord pt de controle de la courbe de bezier de variation de sigma

	int		m_nVignStartPosX;
	int		m_nVignStartPosY;
	int		m_nVignSize;

	CRITICAL_SECTION m_sCriticalSection;
	cv::Mat		m_oCVHf;
};

extern "C"  HRESULT Create_TreatFilter0(REFIID iid, void **ppvObject);




