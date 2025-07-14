#pragma once
#include "stdafx.h"
#include "H3_HoloMap_AltaType.h"
#include "H3IOHoloMAP.h"
#include <tuple>

class CFrGrGlobalTopo : public CH3_HoloMap_AltaType
{
public:
	CFrGrGlobalTopo(const CString& calibFolder);
	~CFrGrGlobalTopo();
	bool PerformGlobalTopo(float Ratio, H3_ARRAY2D_FLT32& phaseMapX, H3_ARRAY2D_FLT32& phaseMapY, H3_ARRAY2D_UINT8& maskImage, const bool bUnwrappedPhase, tuple<float, float> pixel_imageDuPointDAltitudeConnue,std::tuple<int, int> pixel_ref_inPicture,float altitude);

public :
    CImageFloat* m_pArrayOfPicture;
	int	m_nArrayOfPictureSize;

	size_t nNbImages;
	size_t nLi_res;
	size_t nCo_res;
};