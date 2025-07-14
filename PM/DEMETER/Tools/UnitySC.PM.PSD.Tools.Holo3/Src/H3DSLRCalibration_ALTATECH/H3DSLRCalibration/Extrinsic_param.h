
// Extrinsic_param.h: interface for the CExtrinsic_param class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_EXTRINSIC_PARAM_H__A6302DF3_83B2_11D8_BF05_00095B087A04__INCLUDED_)
#define AFX_EXTRINSIC_PARAM_H__A6302DF3_83B2_11D8_BF05_00095B087A04__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000


#ifdef _DLL
#  ifdef __H3_DSLR_CALIBRATION__
#    define H3_EXTRINSIC_PARAM_EXPORT_DECL __declspec(dllexport)
//#    include "H3Camera.h"
#  else
#    define H3_EXTRINSIC_PARAM_EXPORT_DECL __declspec(dllimport)
//#    include "H3Camera_decl.h"
#endif
#else
#  define H3_EXTRINSIC_PARAM_EXPORT_DECL
#endif

class CH3Camera;
#include "CorrespList2.h"

struct PARAM_EXTRINSIC_STRUCT
{
	long nMaxIterHomography;
	long nMinTarget;

};

class H3_EXTRINSIC_PARAM_EXPORT_DECL CExtrinsic_param  
{
public:
	BOOL Load(CString strFileName,CString strEntry);
	BOOL Save(CString strFileName,CString strEntry);
	BOOL LoadCalib(CString strFileName,int Indice=1);
	BOOL SaveCalib(CString strFileName,int Indice=1);

	CExtrinsic_param();
	CExtrinsic_param(const Matrix& omc, const Matrix& Tc);
	virtual ~CExtrinsic_param();
	CExtrinsic_param operator =(const CExtrinsic_param& Src);
	CExtrinsic_param(const CExtrinsic_param& Src);

	bool compute_extrinsic(const H3_ARRAY2D_FLT64& Pix,const H3_ARRAY2D_FLT64& Metric, const CH3Camera& Cam,long MaxIter=20, double* pCond=nullptr, double thresh_cond=1e20);
	bool compute_extrinsic(CCorrespList2& CL, const CH3Camera& Cam,long MaxIter=20, double* pCond=nullptr, double thresh_cond=1e20);
	bool compute_ext_calib(CCorrespList2& CL, const CH3Camera& Cam,long MaxIter=20, double* pCond=nullptr, double thresh_cond=1e20);

	bool compute_extrinsic_refine(CCorrespList2& CL,const CH3Camera& Cam,long MaxIter=20,double* pCond=nullptr,double thresh_cond=1e20);
	bool compute_extrinsic_init(CCorrespList2& CL,const CH3Camera& Cam,bool &m_bPixelNorm);
public:
	bool projectPoints2(const H3_ARRAY2D_FLT64& Metrique_IN,
						const CH3Camera& Cam_IN,
						H3_ARRAY2D_FLT64& Pix_OUT,
						H3_ARRAY2D_FLT64* p_dxpdom=nullptr,
						H3_ARRAY2D_FLT64* p_dxpdT=nullptr,
						H3_ARRAY2D_FLT64* p_dxpdf=nullptr,
						H3_ARRAY2D_FLT64* p_dxpdc=nullptr,
						H3_ARRAY2D_FLT64* p_dxpdk=nullptr,
						H3_ARRAY2D_FLT64* p_dxpdalpha=nullptr);
	bool projectPoints2(const H3_ARRAY2D_FLT32& Metrique_IN,
						const CH3Camera& Cam_IN,
						H3_ARRAY2D_FLT32& Pix_OUT,
						H3_ARRAY2D_FLT32* p_dxpdom=nullptr,
						H3_ARRAY2D_FLT32* p_dxpdT=nullptr,
						H3_ARRAY2D_FLT32* p_dxpdf=nullptr,
						H3_ARRAY2D_FLT32* p_dxpdc=nullptr,
						H3_ARRAY2D_FLT32* p_dxpdk=nullptr,
						H3_ARRAY2D_FLT32* p_dxpdalpha=nullptr);

protected:
	H3_ARRAY2D_FLT64 rigid_motion(const H3_ARRAY2D_FLT64& X, H3_ARRAY2D_FLT64 *p_dYdom=nullptr,H3_ARRAY2D_FLT64 *p_dYdT=nullptr);
	H3_ARRAY2D_FLT32 rigid_motion(const H3_ARRAY2D_FLT32& X, H3_ARRAY2D_FLT32 *p_dYdom=nullptr,H3_ARRAY2D_FLT32 *p_dYdT=nullptr);
	void test();
public:	
	Matrix m_omc;
	Matrix m_Tc;
	CRotation m_Rc;

	bool m_bPixelNorm;
	PARAM_EXTRINSIC_STRUCT m_param;
};

class strPos
{
public:
	strPos(){mb_isActive=false;};
	~strPos(){}
public:
	bool mb_isActive;
	CExtrinsic_param m_ExtP;
	CCorrespList2	m_CList;
};

#endif // !defined(AFX_EXTRINSIC_PARAM_H__A6302DF3_83B2_11D8_BF05_00095B087A04__INCLUDED_)
