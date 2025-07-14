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

class NT_DLL CTreatDegauchy0 :	public INanoTopoTreament,
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

	struct tThreadData
	{
		void*   _Obj;
		int		_SrcId;
	};

public:
	CTreatDegauchy0();

	virtual ~CTreatDegauchy0();

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

	static UINT static_Degauchi(void *p_pParameters);
	bool Degauchi(int p_nImgId);

	bool H3BestFitSurf(const H3_MATRIX_FLT32& Src, const H3_MATRIX_UINT8& SrcMask, H3_MATRIX_FLT64 & MatResSurf, long FitOrder);
	bool H3BestFitSurf(const H3_MATRIX_FLT32& Src, const H3_MATRIX_UINT8& SrcMask, H3_MATRIX_FLT64 & MatResSurf, long FitOrder,long MatVal, const H3_MATRIX_UINT8& MatCoef);

	shared_ptr<H3_MATRIX_FLT32> m_matNX;
	shared_ptr<H3_MATRIX_FLT32> m_matNY;	
	shared_ptr<H3_MATRIX_UINT8> m_matMask;	

	int m_nPrmDegauchyOrder;//ordre pour degauchissage (si <= 0 le degauchy est skippé) 
	int m_nPrmDegauchyStep; //pas d'echantillonage pour degauchissage
	CString	m_csLotID;

	unsigned long	m_uRegFlag;
};

extern "C"  NT_DLL HRESULT Create(REFIID iid, void **ppvObject);




