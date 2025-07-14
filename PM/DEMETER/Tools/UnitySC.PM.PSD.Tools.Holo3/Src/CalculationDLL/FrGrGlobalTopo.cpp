#pragma once
#include "stdafx.h"
#include "FrGrGlobalTopo.h"
#include "H3IOHoloMAP.h"
#include <tuple>
#include "MeasureInfoClass.h"

CFrGrGlobalTopo::CFrGrGlobalTopo(const CString& calibFolder)
    :CH3_HoloMap_AltaType(calibFolder)
{
	m_pArrayOfPicture = NULL;
	m_nArrayOfPictureSize = 0;
}

CFrGrGlobalTopo::~CFrGrGlobalTopo()
{
	if(m_pArrayOfPicture!=nullptr)
	{
		for(size_t i=0; i<m_nArrayOfPictureSize; i++)
		{
			if (m_pArrayOfPicture[i].pData!=nullptr)
			{
				delete[] m_pArrayOfPicture[i].pData;
				m_pArrayOfPicture[i].pData = nullptr;
			}
		}
		delete [] m_pArrayOfPicture;
		m_pArrayOfPicture=nullptr;
		m_nArrayOfPictureSize = 0;
	}
}

bool CFrGrGlobalTopo::PerformGlobalTopo(float Ratio, H3_ARRAY2D_FLT32& phaseMapX, H3_ARRAY2D_FLT32& phaseMapY, H3_ARRAY2D_UINT8& maskImage, const bool bUnwrappedPhase, tuple<float, float> pixel_imageDuPointDAltitudeConnue,std::tuple<int, int> pixel_ref_inPicture,float altitude)
{
	// Note RTI, nCrossX & nCrossY ne sont pas utilise ici 

    const size_t nNbWImages = 2; //deux images pour cette mesure
	
	//Les choses sérieuses commencent
    H3_ARRAY2D_FLT32* pArrayOfWPicture[nNbWImages] = { &phaseMapX , &phaseMapY };

	const bool  b_mesure_shape= true;	//oui pour mesurer le 'vrai' relief (plutot qu'une approximation 'quasi plan')

	int nAppreciationMeasure;
	AFX_MANAGE_STATE(AfxGetStaticModuleState( ))

	// Récuperer les images de phases modulees
	const size_t nLi = pArrayOfWPicture[0]->GetLi();
    const size_t nCo = pArrayOfWPicture[0]->GetCo();

	CH3Array<H3_ARRAY2D_FLT32> wPhase(nNbWImages);
	
	for(size_t i=0; i<nNbWImages; i++)
	{
		wPhase[i].ReAlloc(nLi,nCo);
		wPhase[i].Fill(0);

        float* ptr = pArrayOfWPicture[i]->GetData();
		for(size_t j=0; j<nLi; j++)
		{
			for(size_t k=0; k<nCo; k++)
			{
				wPhase[i].SetAt(j,k,*ptr++);
			}
		}
	}

	// Mesurer
	H3_ARRAY2D_FLT32 aWX, aWY;

	aWX = wPhase[0];
	aWY = wPhase[1];

	// pixel_imageDuPointDAltitudeConnue : point pixel dans l'image camera, proche d'un point de fixation, qui est d'altitude connu ici, cf altitude (on peut qvoir une qltitude initiale non nulle)
	// pixel_ref_inPicture : point pixel dans l'image camera, issu de l'image "cross", refletant la position dans la camera du point de reference ecran (attention doit etre indentique a celui de la calibration systeme) 
	SMesure LesResultats;
	int MesureRes = Mesurer (LesResultats, aWX, aWY, maskImage, bUnwrappedPhase,
        false,
        Ratio,
		pixel_imageDuPointDAltitudeConnue, 
		altitude,
		pixel_ref_inPicture,
		b_mesure_shape);
	aWX.Free();
	aWY.Free();

    // Lire appreciation sur la mesure
    if (MesureRes!=0)
    {
        nAppreciationMeasure = m_nErrorTypeMeasure;
        return false;
    }   
	nAppreciationMeasure=1;

	if(m_pArrayOfPicture!=nullptr)
	{
		for(size_t i=0; i<m_nArrayOfPictureSize; i++)
		{
			if (m_pArrayOfPicture[i].pData!=nullptr)
			{
				delete[] m_pArrayOfPicture[i].pData;
				m_pArrayOfPicture[i].pData = nullptr;
			}
		}
		delete [] m_pArrayOfPicture;
		m_pArrayOfPicture=nullptr;
		m_nArrayOfPictureSize = 0;
	}

	if(!b_mesure_shape)
	{
		// Recuperer les resultats :
		// - 3 cartographies vecteur normale : VX, VY, VZ
		// - 2 cartographies de position : PX, PY
		// - 1 masque >>>> pas retourné

		nNbImages = 5L;
		m_pArrayOfPicture = new CImageFloat[nNbImages];////SI PAS NULLPTR DEJA ALLOUE CV CHECK
		m_nArrayOfPictureSize = nNbImages;

	    nLi_res = LesResultats.aPts.GetLi();
	    nCo_res = LesResultats.aPts.GetCo();

		for(size_t i=0; i<nNbImages; ++i)
		{
			m_pArrayOfPicture[i].pData = new float[nLi_res*nCo_res];

			m_pArrayOfPicture[i].nLi = nLi_res;
			m_pArrayOfPicture[i].nCo = nCo_res;
		}
		for (size_t j=0; j<nLi_res*nCo_res; ++j)
		{
			m_pArrayOfPicture[0].pData[j] = LesResultats.aNs[j].x;	// VX
			m_pArrayOfPicture[1].pData[j] = LesResultats.aNs[j].y;	// VY
			m_pArrayOfPicture[2].pData[j] = LesResultats.aNs[j].z;	// VZ

			m_pArrayOfPicture[3].pData[j] = LesResultats.aPts[j].x;	// PX
			m_pArrayOfPicture[4].pData[j] = LesResultats.aPts[j].y;	// PY
		}
	}else{
		// Recuperer les resultats :
		// - 3 cartographies vecteur normale : VX, VY, VZ
		// - 3 cartographies de position : X, Y, Z
		// - 1 masque >>>> pas retourné

		nNbImages = 6L;
		m_pArrayOfPicture = new CImageFloat[nNbImages];////SI PAS NULLPTR DEJA ALLOUE CV CHECK
		m_nArrayOfPictureSize = nNbImages;

		 nLi_res = LesResultats.aPts.GetLi();
		 nCo_res = LesResultats.aPts.GetCo();

		//m_pArrayOfPicture->nLi = nLi_res;
		//m_pArrayOfPicture->nCo = nCo_res;

		for(size_t i=0; i<nNbImages; ++i)
		{
			m_pArrayOfPicture[i].pData = new float[nLi_res*nCo_res];

			m_pArrayOfPicture[i].nLi = nLi_res;
			m_pArrayOfPicture[i].nCo = nCo_res;
		}
		for (size_t j=0; j<nLi_res*nCo_res; ++j)
		{
			m_pArrayOfPicture[0].pData[j] = LesResultats.aNs[j].x;	// NX
			m_pArrayOfPicture[1].pData[j] = LesResultats.aNs[j].y;	// NY
			m_pArrayOfPicture[2].pData[j] = LesResultats.aNs[j].z;	// NZ

			m_pArrayOfPicture[3].pData[j] = LesResultats.aPts[j].x;	//  X
			m_pArrayOfPicture[4].pData[j] = LesResultats.aPts[j].y;	//  Y
			m_pArrayOfPicture[5].pData[j] = LesResultats.aPts[j].z;	//  Z
		}
	}

	return true;
}