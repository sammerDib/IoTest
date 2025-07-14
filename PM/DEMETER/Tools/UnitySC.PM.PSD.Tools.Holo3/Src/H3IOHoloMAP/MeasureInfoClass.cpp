#include "StdAfx.h"
#include "MeasureInfoClass.h"
#include "H3array2d.h"
#include "H3_HoloMap_AltaTypeExport.h"

CMeasureInfoClass::CMeasureInfoClass(void)
{
}

CMeasureInfoClass::~CMeasureInfoClass(void)
{
/*	const size_t nNbImages = 5L;

	if(m_pArrayOfPicture!=nullptr)
	{
		for(size_t i=0; i<nNbImages; i++)
		{
			if (m_pArrayOfPicture[i].pData!=nullptr)
			{
				delete[] m_pArrayOfPicture[i].pData;
				m_pArrayOfPicture[i].pData = nullptr;
			}
		}
		delete [] m_pArrayOfPicture;
		m_pArrayOfPicture=nullptr;
	}*/
}

void CMeasureInfoClass::SetData(int nCrossX, int nCrossY, CImageFloat** pArrayOfWPicture, CImageByte*   pMPicture, const float ratio,
								const tuple<int, int> pixel_imageDuPointDAltitudeConnue, const float altitude,
								const tuple<int, int> pixel_ref_inPicture,
								const bool mesureType)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString str;

	//CWaitCursor wait;

	m_nCrossX = nCrossX;
	m_nCrossY = nCrossY;
	m_pArrayOfWPicture = pArrayOfWPicture;
	m_pMPicture = pMPicture;
	m_ratio = ratio;
	m_pixel_imagePointAltitudeConnue= pixel_imageDuPointDAltitudeConnue;
	m_altitude = altitude;
	m_pixel_ref_inPicture= pixel_ref_inPicture;
	m_mesureType = mesureType;
}


void CMeasureInfoClass::GetData(CImageFloat* pArrayOfPicture, const size_t nNbImages)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString str;
	//CWaitCursor wait;

	for (size_t i = 0; i<nNbImages; ++i)
	{
		if(! (pArrayOfPicture[i].nLi == m_pArrayOfPicture[i].nLi)
		    &(pArrayOfPicture[i].nCo == m_pArrayOfPicture[i].nCo)
			&(pArrayOfPicture[i].pData != nullptr))
		{
				delete [] pArrayOfPicture[i].pData;
				pArrayOfPicture[i].nLi= m_pArrayOfPicture[i].nLi;
				pArrayOfPicture[i].nCo= m_pArrayOfPicture[i].nCo;

				pArrayOfPicture[i].pData= new float[m_pArrayOfPicture[i].nLi*m_pArrayOfPicture[i].nCo];
		}

		for (size_t j=0, jmax=m_pArrayOfPicture[i].nLi*m_pArrayOfPicture[i].nCo; j<jmax; ++j)
		{
			pArrayOfPicture[i].pData[j]= m_pArrayOfPicture[i].pData[j];
		}
	}
}

///////////////////////////////////////////////////////////////////////////////

H3MEASURE_EXPORT_DECL void Mesurer(CMeasureInfoClass *pMeasure, const bool bUnwrappedPhase, const bool saveUnwrappedPhases, int &nAppreciationMeasure)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState( ))

	// Récuperer les images de phases modulees
	const size_t nLi = pMeasure->m_pArrayOfWPicture[0]->nLi;
	const size_t nCo = pMeasure->m_pArrayOfWPicture[0]->nCo;

	const size_t nNbWImage = 2L;
	CH3Array<H3_ARRAY2D_FLT32> wPhase(nNbWImage);
	
	for(size_t i=0; i<nNbWImage; i++)
	{
		wPhase[i].ReAlloc(nLi,nCo);
		wPhase[i].Fill(0);

		size_t l=0L;
		for(size_t j=0; j<nLi; j++)
		{
			for(size_t k=0; k<nCo; k++)
			{
				wPhase[i].SetAt(j,k,pMeasure->m_pArrayOfWPicture[i]->pData[l]);
				l++;
			}
		}
	}

	// Récuperer l'image du masque
	H3_ARRAY2D_UINT8	maskImage(nLi,nCo);
	maskImage.Fill(0);
	for(size_t i=0; i<nLi*nCo; i++)
	{
		maskImage[i] = pMeasure->m_pMPicture->pData[i];
	}
	//

	// Mesurer
	H3_ARRAY2D_FLT32 aWX, aWY;

	aWX = wPhase[0];
	aWY = wPhase[1];

	SMesure LesResultats;
    if (H3_HoloMap_AltaType_Mesurer(LesResultats, aWX, aWY, maskImage, bUnwrappedPhase, saveUnwrappedPhases, pMeasure->m_ratio,
									pMeasure->m_pixel_imagePointAltitudeConnue, 
									pMeasure->m_altitude,
									pMeasure->m_pixel_ref_inPicture,
									pMeasure->m_mesureType)==1)
	{
		nAppreciationMeasure=1;
		return;
	}
	else
	{
		// Lire appreciation sur la mesure
		nAppreciationMeasure = H3_HoloMap_AltaType_GetErrorTypeMeasure();
	}
	//

	aWX.Free();
	aWY.Free();
	maskImage.Free();

	if(!pMeasure->m_mesureType)
	{
		// Recuperer les resultats :
		// - 3 cartographies vecteur normale : VX, VY, VZ
		// - 2 cartographies de position : PX, PY
		// - 1 masque >>>> pas retourné

		const size_t nNbImages = 5L;
		pMeasure->m_pArrayOfPicture = new CImageFloat[nNbImages];////SI PAS NULLPTR DEJA ALLOUE CV CHECK

		const size_t nLi_res = LesResultats.aPts.GetLi();
		const size_t nCo_res = LesResultats.aPts.GetCo();

		pMeasure->m_pArrayOfPicture->nLi = nLi_res;
		pMeasure->m_pArrayOfPicture->nCo = nCo_res;

		for(size_t i=0; i<nNbImages; ++i)
		{
			pMeasure->m_pArrayOfPicture[i].pData = new float[nLi_res*nCo_res];

			pMeasure->m_pArrayOfPicture[i].nLi = nLi_res;
			pMeasure->m_pArrayOfPicture[i].nCo = nCo_res;
		}
		for (size_t j=0; j<nLi_res*nCo_res; ++j)
		{
			pMeasure->m_pArrayOfPicture[0].pData[j] = LesResultats.aNs[j].x;	// VX
			pMeasure->m_pArrayOfPicture[1].pData[j] = LesResultats.aNs[j].y;	// VY
			pMeasure->m_pArrayOfPicture[2].pData[j] = LesResultats.aNs[j].z;	// VZ

			pMeasure->m_pArrayOfPicture[3].pData[j] = LesResultats.aPts[j].x;	// PX
			pMeasure->m_pArrayOfPicture[4].pData[j] = LesResultats.aPts[j].y;	// PY
		}
	}else{
		// Recuperer les resultats :
		// - 3 cartographies vecteur normale : VX, VY, VZ
		// - 3 cartographies de position : X, Y, Z
		// - 1 masque >>>> pas retourné

		const size_t nNbImages = 6L;
		pMeasure->m_pArrayOfPicture = new CImageFloat[nNbImages];////SI PAS NULLPTR DEJA ALLOUE CV CHECK

		const size_t nLi_res = LesResultats.aPts.GetLi();
		const size_t nCo_res = LesResultats.aPts.GetCo();

		pMeasure->m_pArrayOfPicture->nLi = nLi_res;
		pMeasure->m_pArrayOfPicture->nCo = nCo_res;

		for(size_t i=0; i<nNbImages; ++i)
		{
			pMeasure->m_pArrayOfPicture[i].pData = new float[nLi_res*nCo_res];

			pMeasure->m_pArrayOfPicture[i].nLi = nLi_res;
			pMeasure->m_pArrayOfPicture[i].nCo = nCo_res;
		}
		for (size_t j=0; j<nLi_res*nCo_res; ++j)
		{
			pMeasure->m_pArrayOfPicture[0].pData[j] = LesResultats.aNs[j].x;	// NX
			pMeasure->m_pArrayOfPicture[1].pData[j] = LesResultats.aNs[j].y;	// NY
			pMeasure->m_pArrayOfPicture[2].pData[j] = LesResultats.aNs[j].z;	// NZ

			pMeasure->m_pArrayOfPicture[3].pData[j] = LesResultats.aPts[j].x;	//  X
			pMeasure->m_pArrayOfPicture[4].pData[j] = LesResultats.aPts[j].y;	//  Y
			pMeasure->m_pArrayOfPicture[5].pData[j] = LesResultats.aPts[j].z;	//  Z
		}
	}
}


