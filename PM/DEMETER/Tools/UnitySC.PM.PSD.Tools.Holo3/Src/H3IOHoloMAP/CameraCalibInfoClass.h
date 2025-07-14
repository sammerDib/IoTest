#pragma once

#ifdef _DLL
#  ifdef __H3_IO_HOLOMAP__
#    define H3CAMERACALIB_IC_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3CAMERACALIB_IC_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3CAMERACALIB_IC_EXPORT_DECL
#endif
#include "H3IOHoloMAP.h"

class H3CAMERACALIB_IC_EXPORT_DECL CCameraCalibInfoClass
{
public:
	CCameraCalibInfoClass(const CString& calibFolder);
	virtual ~CCameraCalibInfoClass(void);

    CString m_CalibFolder;

	void SetData(const unsigned int nMireSizeX,
			     const unsigned int nMireSizeY,
			     const float fMireStepX, 
			     const float fMireStepY, 
			     const unsigned int nNbImg, 
			     CImageByte* pArrayOfVPicture);

protected:
	unsigned int m_nMireSizeX;		// Nombre d'intersections sur la mire de calibrage dans la direction horizontale X
	unsigned int m_nMireSizeY;		// Nombre d'intersections sur la mire de calibrage dans la direction verticale Y
	
	float m_fMireStepX;		// Pas de la mire de calibrage suivant la direction horizontale X
	float m_fMireStepY;		// Pas de la mire de calibrage suivant la direction verticale Y

	unsigned int m_nNbImage;			// Nombre d'images utilisées pour le calibrage de la caméra

	CImageByte* m_pArrayOfVPicture;	// Les images vidéo (V)

//public:
friend void H3CAMERACALIB_IC_EXPORT_DECL CalibrageCamera(CCameraCalibInfoClass *pCalibCam, int &nAppreciationCalibCamera);
};



