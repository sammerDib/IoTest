/*! 
* 	\file    H3_HoloMap_AltaTypeDECL.h
* 	\brief   Entete de la dll COVELI
* 	\version 
* 	\author  CV
* 	\date    2012
* 	\remarks .
*/ 
#pragma once
#include <tuple>
#if !defined(AFX_H3_HoloMap_AltaTypeEXPORT_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_)
#define AFX_H3_HoloMap_AltaTypeEXPORT_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_

#ifdef _DLL
#  ifdef AFX_COVELI_H__4BD85349_6C93_453D_84BE_C8572D8A2F42__INCLUDED_
#    define H3_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3_EXPORT_DECL __declspec(dllimport)
#  endif  
#else
#  define H3_EXPORT_DECL
#endif

//////////////////////////////////////////////////////////////////////////////////
#if !defined(PT_COMPLET__INCLUDED_)
struct SMesure{
	H3_ARRAY2D_PT3DFLT32 aPts;
	H3_ARRAY2D_V3DFLT32  aNs;
	H3_ARRAY2D_INT8  aMask;
};
#endif

#ifdef __cplusplus
extern "C" {
#endif

/*! 
* 	\fn       H3_HoloMap_Base_IsAllocated
* 	\brief    Verifier si le capteur est alloué
* 	\return   1 si tout est OK.
* 	\remarks  Fonction non utile pour Metrelog
*/ 
H3_EXPORT_DECL long H3_HoloMap_AltaType_IsAllocated();	

/*! 
* 	\fn       H3_HoloMap_Base_Alloc
* 	\brief    Allocation du capteur
* 	\return   1 si tout est OK.
* 	\remarks  Fonction non utile pour Metrelog
*/
H3_EXPORT_DECL long H3_HoloMap_AltaType_Alloc(const CString& calibFolder);

/*! 
* 	\fn       H3_HoloMap_Base_Free
* 	\brief    Desallouer le capteur
* 	\remarks  Fonction non utile pour Metrelog
*/
H3_EXPORT_DECL void H3_HoloMap_AltaType_Free();

/*! 
* 	\fn       H3_HoloMap_AltaType_CalibSystem()
* 	\brief    calibrage systeme 
* 	\remarks  
*/ 
H3_EXPORT_DECL void H3_HoloMap_AltaType_CalibSystem(const CH3Array<H3_ARRAY2D_FLT32> & wPhase,
												    const H3_ARRAY2D_UINT8 &	mireImage,
												    const float fPixSizeX, const float fPixSizeY, 
												    const float fMireMonStepX, const float fMireMonStepY,
													const unsigned long pixRef_Xscreen, const unsigned long pixRef_Yscreen,
													const unsigned long screen_Xsz, const unsigned long screen_Ysz,
												    const int nCrossX, const int nCrossY, const float *ratio, const CString& calibFolder);

/*! 
* 	\fn       H3_HoloMap_AltaType_Demodule()
* 	\brief    demodulation d'images de phase 
* 	\remarks  n H3_ARRAY2D dans wPhase. Le pas du reseau utilisé pour obtenir wPhase[i] est ratio[i+1] fois plus grand que le pas utilisé pour wPhase[i+1] //ratio[0] non utilisé
*/ 
H3_EXPORT_DECL void H3_HoloMap_AltaType_Demodule(H3_ARRAY2D_FLT32& UW, const CH3Array<H3_ARRAY2D_FLT32> & wPhase, const CH3Array2D<unsigned char> & Mask, const float *ratio);

/*! 
* 	\fn       H3_HoloMap_AltaType_GetErrorTypeCalibSys()
* 	\brief    erreur calibrage systeme 
* 	\remarks  
*/ 
H3_EXPORT_DECL int H3_HoloMap_AltaType_GetErrorTypeCalibSys();

/*! 
* 	\fn       H3_HoloMap_AltaType_GetErrorTypeMeasure()
* 	\brief    erreur mesure 
* 	\remarks  
*/ 
H3_EXPORT_DECL int H3_HoloMap_AltaType_GetErrorTypeMeasure();

/*! 
* 	\fn       H3_InitSys
* 	\brief    initialise le systeme à partir des données issues du calibrage systeme
* 	\remarks  une partie des données est chargée depuis c:\altasight\...
*/ 
H3_EXPORT_DECL void H3_InitSys(const CString& calibFolder);

//H3_EXPORT_DECL void H3_InitSys2(CString CalibCameraFilePath,CString CalibResultFilePath,CString CalibResultFilePathX,CString CalibResultFilePathY);

#ifdef __cplusplus
}
#endif


#ifdef __cplusplus
extern "C++" {

/* int H3_HoloMap_AltaType_Mesurer(SMesure& LeResultat, const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMask,const float Ratio_ActualPixelPerPeriod_divby_CalibOne)
*  brief
*  param
*  remarks 
*/
//H3_EXPORT_DECL int H3_HoloMap_AltaType_Mesurer(SMesure& LeResultat, const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMask,const float Ratio_ActualPixelPerPeriod_divby_CalibOne= 1, const float altitude= 0.0f, const bool mesureType= false);
H3_EXPORT_DECL int H3_HoloMap_AltaType_Mesurer(SMesure& LeResultat, const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMask,
    const bool bUnwrappedPhase, const bool saveUnwrappedPhases,
    const float Ratio_ActualPixelPerPeriod_divby_CalibOne,
								const std::tuple<float, float> pixel_imagePointAltitudeConnue,
								const float altitude,
								const std::tuple<unsigned long, unsigned long> pixel_imageMarquageEcran,
								const bool mesureType);

// Fonctions utilisees uniquement par Altatec
H3_EXPORT_DECL BOOL H3_HoloMap_AltaType_CheckPosition(const CPoint P);

}
#endif // __cplusplus


#endif //!defined(AFX_H3_HoloMap_AltaTypeEXPORT_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_)
