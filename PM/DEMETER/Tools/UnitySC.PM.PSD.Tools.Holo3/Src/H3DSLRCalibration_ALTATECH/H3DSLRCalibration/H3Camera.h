// H3Camera.h: interface for the CH3Camera class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3CAMERA_H__A3B32F3C_8833_40F3_8B0C_85E7AC264483__INCLUDED_)
#define AFX_H3CAMERA_H__A3B32F3C_8833_40F3_8B0C_85E7AC264483__INCLUDED_


#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifdef _DLL
#  ifdef __H3_DSLR_CALIBRATION__
#    define H3_CAMERA_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3_CAMERA_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3_CAMERA_EXPORT_DECL
#endif

#include "Extrinsic_param.h"

struct stParamIntrin
{
	H3_ARRAY_FLT32 afc;
	H3_ARRAY_FLT32 acc;
	H3_ARRAY_FLT32 akc;
	H3_ARRAY_FLT32 aalphac;
};

struct stParamIntrinErr
{
	H3_ARRAY_FLT32 afc_err;
	H3_ARRAY_FLT32 acc_err;
	H3_ARRAY_FLT32 akc_err;
	H3_ARRAY_FLT32 aalphac_err;
	H3_ARRAY_FLT32 apix_err;
};

class H3_CAMERA_EXPORT_DECL CH3Camera  
{
public:
	CH3Camera(size_t _nLi=2048,size_t _nCo=2048);
	virtual ~CH3Camera();
	void InitDefault(size_t _nx, size_t _ny);
	void Init(size_t _nx, size_t _ny,const stParamIntrin&,const stParamIntrinErr&);

protected:
	CString		m_strUnit;				// Unite de mesure, par defaut mm
	int m_nReprojectionSigma;

public:
	bool MFCalib(H3_ARRAY_PT2DFLT32 *Pt,unsigned int nbImage,H3_ARRAY2D_FLT64 Metric);
	void WinAffiche() const;
	BOOL SaveCalib(CString strFileName,int Indice=1);
	BOOL LoadCalib(CString strFileName,int Indice=1);
	stParamIntrin GetParamIntrin()const;
	stParamIntrinErr GetParamIntrinErr()const;

	CH3Camera & operator=(const CH3Camera& Cam);

	CString GetUnit();
	bool calibrage(CH3Array< strPos >& A_Pos );
	bool calib_optim(CH3Array< strPos >& A_Pos );
	bool init_param(CH3Array< strPos >& A_Pos);
	bool calib_iter(CH3Array< strPos >& A_Pos );
	bool compensate_distortion_oulu(const H3_ARRAY2D_FLT64& Pixelin, H3_ARRAY2D_FLT64& Pixelout) const;//plan retinien 
	bool compensate_distortion_oulu_px(const H3_ARRAY2D_FLT64 &Pixelin, H3_ARRAY2D_FLT64 &Pixelout) const;//ccd
	bool normalise(const H3_ARRAY2D_FLT64& Pixelin,H3_ARRAY2D_FLT64& Pixelout) const;
	bool apply_distortion(const H3_ARRAY2D_FLT64& Pixelin, H3_ARRAY2D_FLT64& Pixelout) const;//applique une distortion dans le plan retinien 
	bool Rect_Index(const Matrix& R, H3_ARRAY2D_FLT64 & outRectIndex , H3_ARRAY2D_FLT64 & outIndex , H3_ARRAY2D_FLT64 & outFactor)const;
	bool Rect_Index(const Matrix& R, const H3_ARRAY2D_FLT64 & inRectIndex, H3_ARRAY2D_FLT64 & outRectIndex)const;
	bool iRect_Index(const Matrix& R,const Matrix &KKrect, const H3_ARRAY2D_FLT64 & inRectIndex, H3_ARRAY2D_FLT64 & outRectIndex)const;

	size_t nx,ny;//nb pixels

	double fc[2];//focal lenght
	double cc[2];//principal point
	double alpha_c;//skew
	double kc[5];//distortion

	double pix_erreur[2]; //pixel erreur
	double fc_erreur[2];//focal lenght
	double cc_erreur[2];//principal point
	double alpha_c_erreur;//skew
	double kc_erreur[5];//distortion

	bool mb_is_alpha;//=1: compute skew
	bool mb_recompute_extrinsic;
	bool m_is_dist[5];//=[1 1 1 1 1] pour calculer les 5 composants de k

	bool mb_is_initialised;
	double m_fCalibPixelErr;
};

#endif // !defined(AFX_H3CAMERA_H__A3B32F3C_8833_40F3_8B0C_85E7AC264483__INCLUDED_)
