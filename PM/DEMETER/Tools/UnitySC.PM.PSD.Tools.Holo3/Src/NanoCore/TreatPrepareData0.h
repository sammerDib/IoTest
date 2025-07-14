#pragma once

#include "TreatmentInterface.h"
#include "H3Matrix.h"
#include <list>



class CTreatPrepareData0 : public INanoTopoTreament,
						   public CWinThread
{

public :
	typedef struct  
	{
		UINT	nId;
		CString csName;
		CString csPath;
		shared_ptr<H3_MATRIX_FLT32> spNX;
		shared_ptr<H3_MATRIX_FLT32> spNY;
		shared_ptr<H3_MATRIX_UINT8> spMask;
		shared_ptr<H3_MATRIX_UINT8> spMaskE;
		shared_ptr<H3_MATRIX_UINT8> spMaskD;
	}tData2Save;

public:
	CTreatPrepareData0();

	virtual ~CTreatPrepareData0();

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

	int		m_nPrmErodeRadius;
	int		m_nPrmDilateRadius;
	bool    m_bUseFringeKiller;
	unsigned long	m_uRegFlag;
    CString	m_csLotID;

	shared_ptr<H3_MATRIX_FLT32> m_matNX;
	shared_ptr<H3_MATRIX_FLT32> m_matNY;
	shared_ptr<H3_MATRIX_FLT32> m_matPX;
	shared_ptr<H3_MATRIX_FLT32> m_matPY;

	shared_ptr<H3_MATRIX_UINT8> m_matMask;

	shared_ptr<H3_MATRIX_UINT8> m_Erode;
	shared_ptr<H3_MATRIX_UINT8> m_Dilate;
	shared_ptr<H3_MATRIX_UINT8> m_SE;

	CRITICAL_SECTION m_sCriticalSection;
};

extern "C"  HRESULT Create_TreatPrepareData0(REFIID iid, void **ppvObject);




