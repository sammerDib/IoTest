#pragma once

#ifdef _DLL
#  ifdef __H3_IO_HOLOMAP__
#    define H3SYSTEMCALIB_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3SYSTEMCALIB_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3SYSTEMCALIB_EXPORT_DECL
#endif

#include "H3IOHoloMAP.h"

class H3SYSTEMCALIB_EXPORT_DECL CSystemCalibInfoClass
{
public:
	CSystemCalibInfoClass(const CString& calibFolder);
	~CSystemCalibInfoClass(void);

    CString m_CalibFolder;

	void SetData(CImageFloat* pArrayOfWPicture,
			     CImageByte* pVPicture,
			     float fPixSizeX, float fPixSizeY, 
			     float fMireMonStepX, float fMireMonStepY,
				 unsigned long pixRef_Xscreen,unsigned long pixRef_Yscreen,
				 unsigned long screen_Xsz,unsigned long screen_Ysz,
			     unsigned int nCrossX,unsigned  int nCrossY,unsigned int nNbWImg,const float *ratio);

    void CalibrageSystem(int& nAppreciationCalibSystem);

protected:
	float m_fPixSizeX, m_fPixSizeY;					// Dimension des pixels moniteurs en millimetre
	float m_fMireMonStepX, m_fMireMonStepY;			// Pas de la mire de calibrage en pixel
	unsigned long m_pixRef_Xscreen,  m_pixRef_Yscreen;//position sur l'ecran (en pixel) d'un point remaquable (marquage)
	unsigned long m_screen_Xsz,m_screen_Ysz;		//dimension de l'ecran (en pixel)
	unsigned int m_nCrossX, m_nCrossY;				// Position d'un point remarquable (dans l'image - en pixel -)
	unsigned int m_nNbWImage;						// Nombre d'images utilisées pour le calibrage système
	const float *m_ratio;					

	CImageFloat* m_pArrayOfWPicture;		// Les images de phase (W) X1 Y1 X2 Y2 X3 Y3
	CImageByte* m_pVPicture;				// Image vidéo (V)
};
