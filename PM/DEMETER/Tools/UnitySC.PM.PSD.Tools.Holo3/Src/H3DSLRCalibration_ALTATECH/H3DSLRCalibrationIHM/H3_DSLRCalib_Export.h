/*! 
* 	\file    H3__DSLRCalib_Export.h
* 	\brief   Entete de la dll COVELI
* 	\version 
* 	\author  CV
* 	\date    2012
* 	\remarks .
*/ 

#if !defined(AFX_H3__DSLRCalib_EXPORT_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_)
#define AFX_H3__DSLRCalib_EXPORT_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_

#ifdef _DLL
#  ifdef __H3_DSLR_CALIBRATION__
#    define H3_DSLRCalibExp_DECL __declspec(dllexport)
#  else
#    define H3_DSLRCalibExp_DECL __declspec(dllimport)
#endif  
#else
#  define H3_DSLRCalibExp_DECL
#endif

#include "H3array2d.h"

//////////////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
extern "C" {
#endif

///////////////////////////////////////////////////////////////////////////////////////////
H3_DSLRCalibExp_DECL long H3_DSLR_CalibCamera(const int nMireSizeX,
											  const int nMireSizeY,
											  const float fMireStepX,
											  const float fMireStepY,
											  const int nNbImg,
											  const CH3Array<H3_ARRAY2D_UINT8> & vMirePos,
                                              const CString& folder);

///////////////////////////////////////////////////////////////////////////////////////////
H3_DSLRCalibExp_DECL int H3_DSLR_GetErrorTypeCalibCam();

///////////////////////////////////////////////////////////////////////////////////////////
H3_DSLRCalibExp_DECL long H3_DSLR_GetExtrinsic(H3_ARRAY2D_UINT8* pImageWafRef, const CString& calibFolder);

#ifdef __cplusplus
}
#endif

#endif //!defined(AFX_H3_DSLRCalib_EXPORT_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_)
