#pragma once

#ifdef _DLL
#  ifdef __H3_IO_HOLOMAP__
#    define H3MEASURE_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3MEASURE_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3MEASURE_EXPORT_DECL
#endif

#include "H3IOHoloMAP.h"
#include <tuple>

class H3MEASURE_EXPORT_DECL CMeasureInfoClass
{
public:
	CMeasureInfoClass(void);
	~CMeasureInfoClass(void);

	void SetData(int nCrossX, int nCrossY, CImageFloat** pArrayOfWPicture, CImageByte*   pMPicture, const float ratio,
				const std::tuple<int, int> pixel_imageDuPointDAltitudeConnue= std::tuple<int, int>(0,0),
				const float altitude= 0.0f,
				const std::tuple<int, int> pixel_ref_inPicture= std::tuple<int, int>(0,0),
				const bool mesureType= false);
	void GetData(CImageFloat* pArrayOfPicture, const size_t nNbImages= 5);	// m_mesureType= false: VX VY VZ PX PY; sinon NX NY NZ X Y Z

protected:
	int m_nCrossX;	// Position en X d'un point remarquable(m_mesureType=false) ou position dans l'image du point d'altitude connue(m_mesureType=true)
	int m_nCrossY;	// Position en Y d'un point remarquable

	float m_ratio;

	std::tuple<float, float> m_pixel_imagePointAltitudeConnue;
	float m_altitude;

	std::tuple<int, int> m_pixel_ref_inPicture;

	bool m_mesureType;// false: le wafer est quasi plan ET coplanaire au wafer ref dans la position ref. true: wafer qcq, position qcq => images de phase démodulées.

    CImageFloat**  m_pArrayOfWPicture;	// Les images de phase (W -pour Wrapped ie modulées- si m_mesureType= false, démodulées (UW -pour UnWrapped-) sinon   )
	CImageByte*   m_pMPicture;			// Image Masque (M)

	CImageFloat*  m_pArrayOfPicture;	//les resultats

    H3MEASURE_EXPORT_DECL friend void Mesurer(CMeasureInfoClass *pMeasure, const bool bUnwrappedPhase, const bool saveUnwrappedPhases, int &nAppreciationMeasure);
};
