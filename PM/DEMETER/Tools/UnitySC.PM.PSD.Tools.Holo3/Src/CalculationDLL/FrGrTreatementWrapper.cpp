#include "stdafx.h"
#include "FrGrTreatementWrapper.h"

#include "H3_HoloMap_AltaType.h"

//#include "MeasureInfoClassDecl.h"

///initializes factory structures with default values. These values can be furter updated 
CFrGrTreatementWrapper::CFrGrTreatementWrapper(MIL_ID SystemID)
{
	m_crossImg=M_NULL;
	m_FrGrTreatment.AssociateWrapper(this);
	m_milSystemID = SystemID;

    m_curvatureConfig.fUserCurvatureDynamicsCoeff = 1;
    m_curvatureConfig.nTargetBackgroundGrayLevel = 20;
    m_curvatureConfig.fCalibratedNoise = 0.0055f;  // Dummy value coming from a Matlab calculation on PSD prototype. Unit is radians/pixel.

	m_filterFactory.Contraste_min_curvature = 15;   //15
	m_filterFactory.Intensite_min_curvature = 10;   //10

	m_bDoGlobalTopo= FALSE;
	m_bKeepPhaseAndMask = FALSE;
    m_bSlope = FALSE;
}

bool CFrGrTreatementWrapper::SetFilterConfig(FILTER_FACTORY config)
{
	m_filterFactory = config;
	return true;
}

bool CFrGrTreatementWrapper::SetGlobalTopoConfig(GLOBAL_TOPOCONFIG config)
{
	m_globalTopoConfig = config;
	return true;
}

bool CFrGrTreatementWrapper::SetCurvatureConfig(CURVATURE_CONFIG config)
{
	m_curvatureConfig = config;

	return true;
}

bool CFrGrTreatementWrapper::SetInputInfo(INPUT_INFO info, int periods[])
{
    m_IIC.AllocateData(info.NbPeriods, info.NbImgX, info.NbImgY, info.SizeX, info.SizeY);
    m_bDoGlobalTopo = (info.TypeOfFrame & GlobalTopoNX) || (info.TypeOfFrame & GlobalTopoNY) || (info.TypeOfFrame & GlobalTopoNZ) || (info.TypeOfFrame & GlobalTopoX) || (info.TypeOfFrame & GlobalTopoY) || (info.TypeOfFrame & GlobalTopoZ);
    bool bDoPhase = (info.TypeOfFrame & PhaseX) || (info.TypeOfFrame & PhaseY) || (info.TypeOfFrame & PhaseMask);
    m_bKeepPhaseAndMask = m_bDoGlobalTopo || bDoPhase;
    m_bSlope = (info.TypeOfFrame & UnwrappedPhaseX) || (info.TypeOfFrame & UnwrappedPhaseY);
    m_TypeOfFrame = info.TypeOfFrame;
    m_Periods = std::vector<int>(&periods[0], &periods[info.NbPeriods]);
    return true;
}

bool CFrGrTreatementWrapper::PerformCalculation()
{
    for (int i = 0; i < m_IIC.m_Images.size(); i++)
    {
        if (m_IIC.m_Images[i] == NULL)
            return false;
    }

    return m_FrGrTreatment.FrGr_PerformCalculation(&m_IIC);
    m_IIC.FreeData();
}

float CFrGrTreatementWrapper::PerformCurvatureCalibration()
{
    for (int i = 0; i < m_IIC.m_Images.size(); i++)
    {
        if (m_IIC.m_Images[i] == NULL)
            return false;
    }

    return m_FrGrTreatment.FrGr_PerformCurvatureCalibration(&m_IIC);
    m_IIC.FreeData();
}

bool CFrGrTreatementWrapper::PerformIncrementalCalculation(int period, char direction)
{
    return m_FrGrTreatment.FrGr_PerformIncrementalCalculation(&m_IIC, period, direction);
}

bool CFrGrTreatementWrapper::UpdateCurvatureConfig(CURVATURE_CONFIG value, TypeOfFrame typeOfFrame)
{
    m_curvatureConfig = value;
    return m_FrGrTreatment.FrGr_RecalculateOutputs(typeOfFrame);
}

bool CFrGrTreatementWrapper::SearchScreenPixelReference_InPicture(int& PixRef_inPicture_X, int& PixRef_inPicture_Y, byte cThresholdValue, MIL_ID CrossImg, MIL_ID MaskImg)
{
	bool bSuccess = false;
	if(CrossImg != M_NULL)
	{
		if(MaskImg != M_NULL)
		{
			MIL_INT sizeX2 = 0;
            MIL_INT sizeY2 = 0;
            MbufInquire(CrossImg, M_SIZE_X, &sizeX2);
            MbufInquire(CrossImg, M_SIZE_Y, &sizeY2);
			MIL_INT sizeX = 0;
            MIL_INT sizeY = 0;
            MbufInquire(MaskImg, M_SIZE_X, &sizeX);
            MbufInquire(MaskImg, M_SIZE_Y, &sizeY);
			ASSERT(sizeX2 == sizeX);
			ASSERT(sizeY2 == sizeY);

			MimArith(MaskImg,CrossImg,MaskImg, M_MULT+M_SATURATION ); // on applique le mask, evite reflet exterieur wafer

			// on binarize avec le threshold
			MIL_ID ID;
			MbufAlloc2d(m_milSystemID,sizeX,sizeY,1+M_UNSIGNED,M_IMAGE+M_PROC,&ID);	

			MIL_DOUBLE dTh = (double)cThresholdValue;
			MimBinarize(MaskImg,ID,M_GREATER_OR_EQUAL,dTh, M_NULL);

		//	MimArith(ID,255,MaskImg, M_MULT_CONST+M_SATURATION ); // on applique le mask, evite reflet exterieur wafer

			// on recherche le blob resultats
			MIL_ID MilBlobFeatureList;
			MIL_ID MilBlobResult;
			MIL_INT nTotalBlobs =0;  
			/* Allocate a feature list. */ 
			MblobAllocFeatureList(m_milSystemID, &MilBlobFeatureList); 
  
			/* Enable the Center Of Gravity feature calculation. */ 
			MblobSelectFeature(MilBlobFeatureList, M_CENTER_OF_GRAVITY); 

			/* Allocate a blob result buffer. */
			MblobAllocResult(m_milSystemID, &MilBlobResult); 

		    /* Calculate selected features for each blob. */ 
			MblobCalculate(ID, M_NULL, MilBlobFeatureList, MilBlobResult); 
 
			/* Get the total number of selected blobs. */ 
			MblobGetNumber(MilBlobResult, &nTotalBlobs); 

			if(nTotalBlobs == 1)
			{
				MIL_DOUBLE Cx,Cy;
				MblobGetResult(MilBlobResult, M_CENTER_OF_GRAVITY_X, &Cx); 
				MblobGetResult(MilBlobResult, M_CENTER_OF_GRAVITY_Y, &Cy); 
				PixRef_inPicture_X = (int)Cx;
				PixRef_inPicture_Y = (int)Cy;
			}// sinon pas ou trop de solution // on fait quoi?

			if(MilBlobResult != M_NULL)
			{
				MblobFree(MilBlobResult); 
				MilBlobResult = M_NULL;
			}

			if(MilBlobFeatureList != M_NULL)
			{
				MblobFree(MilBlobFeatureList); 
				MilBlobFeatureList = M_NULL;
			}

			if(ID != M_NULL)
			{
				MbufFree(ID);
				ID = M_NULL;
			}
		}	
	}
	return bSuccess;
}

bool CFrGrTreatementWrapper::PerformGlobalTopo()
{
    // Point de référence
    //...................
    tuple<float, float> pixel_imageDuPointDAltitudeConnue((float)m_globalTopoConfig.KnownHeightPixelX, (float)m_globalTopoConfig.KnownHeightPixelY);
    tuple<int, int> pixel_ref_inPicture(m_globalTopoConfig.PixelRefX, m_globalTopoConfig.PixelRefY);

    MIL_ID CrossCameraPicture = m_crossImg;
    if (CrossCameraPicture != M_NULL)
    {
        byte bThresholdValue = m_globalTopoConfig.CrossSearchThreshold;
        MIL_ID msk = GetResultImage(PhaseMask);
        int nX = m_globalTopoConfig.PixelRefX;
        int nY = m_globalTopoConfig.PixelRefY;
        SearchScreenPixelReference_InPicture(nX, nY, bThresholdValue, CrossCameraPicture, msk);
        if (msk != M_NULL)
        {
            MbufFree(msk);
            msk = M_NULL;
        }
        pixel_ref_inPicture = tuple<int, int>(nX, nY);

    }
    if (m_crossImg != NULL)
    {
        MbufFree(m_crossImg);
        m_crossImg = M_NULL;
    }

    // Calcul de la Global Topo
    //.........................
    H3_ARRAY2D_FLT32* phaseMapX;
    H3_ARRAY2D_FLT32* phaseMapY;
    H3_ARRAY2D_UINT8* maskImage = (H3_ARRAY2D_UINT8*)m_FrGrTreatment.m_ResultImages[PhaseMask];

    if (m_globalTopoConfig.UnwrappedPhase)
    {
        phaseMapX = (H3_ARRAY2D_FLT32*)m_FrGrTreatment.m_ResultImages[UnwrappedPhaseX];
        phaseMapY = (H3_ARRAY2D_FLT32*)m_FrGrTreatment.m_ResultImages[UnwrappedPhaseY];
    }
    else
    {
        phaseMapX = (H3_ARRAY2D_FLT32*)m_FrGrTreatment.m_ResultImages[PhaseX];
        phaseMapY = (H3_ARRAY2D_FLT32*)m_FrGrTreatment.m_ResultImages[PhaseY];
    }
    

    return m_FrGrGlobalTopo->PerformGlobalTopo(
        m_globalTopoConfig.Ratio,
        *phaseMapX, *phaseMapY, *maskImage, m_globalTopoConfig.UnwrappedPhase,
        pixel_imageDuPointDAltitudeConnue, pixel_ref_inPicture,
        m_globalTopoConfig.Height
    );
}

bool CFrGrTreatementWrapper::SetCrossImg(long MILID)
{
	//m_crossImg=MILID;

    if (m_crossImg != M_NULL)
    {
        MbufFree(m_crossImg);
        m_crossImg = M_NULL;
    }

    // We clone the MIL image 
    MIL_INT sizeX = 0;
    MIL_INT sizeY = 0;
    MbufInquire(MILID, M_SIZE_X, &sizeX);
    MbufInquire(MILID, M_SIZE_Y, &sizeY);
   
    // on binarize avec le threshold
    MbufAlloc2d(m_milSystemID, sizeX, sizeY, 8 + M_UNSIGNED, M_IMAGE + M_PROC, &m_crossImg);
    MbufCopy(MILID, m_crossImg);

  
	return true;
}

MIL_ID CFrGrTreatementWrapper::GetResultImage(TypeOfFrame typeOfFrame, int index)
{
    if (typeOfFrame == GlobalTopoX)
    {
        MIL_ID ID;
        MbufAlloc2d(m_milSystemID, m_FrGrGlobalTopo->nCo_res, m_FrGrGlobalTopo->nLi_res, 32 + M_FLOAT, M_IMAGE + M_PROC, &ID);
        MbufPut2d(ID, 0, 0, m_FrGrGlobalTopo->nCo_res, m_FrGrGlobalTopo->nLi_res, m_FrGrGlobalTopo->m_pArrayOfPicture[3].pData);
        return ID;
    }

    if (typeOfFrame == GlobalTopoY)
    {
        MIL_ID ID;
        MbufAlloc2d(m_milSystemID, m_FrGrGlobalTopo->nCo_res, m_FrGrGlobalTopo->nLi_res, 32 + M_FLOAT, M_IMAGE + M_PROC, &ID);
        MbufPut2d(ID, 0, 0, m_FrGrGlobalTopo->nCo_res, m_FrGrGlobalTopo->nLi_res, m_FrGrGlobalTopo->m_pArrayOfPicture[4].pData);
        return ID;
    }

    if (typeOfFrame == GlobalTopoZ)
    {
        MIL_ID ID;
        MbufAlloc2d(m_milSystemID, m_FrGrGlobalTopo->nCo_res, m_FrGrGlobalTopo->nLi_res, 32 + M_FLOAT, M_IMAGE + M_PROC, &ID);
        MbufPut2d(ID, 0, 0, m_FrGrGlobalTopo->nCo_res, m_FrGrGlobalTopo->nLi_res, m_FrGrGlobalTopo->m_pArrayOfPicture[5].pData);
        return ID;
    }

    auto pair = m_FrGrTreatment.m_ResultImages.find((TypeOfFrame)(typeOfFrame + index));
    if (pair == m_FrGrTreatment.m_ResultImages.end())
        return -1;

    CH3GenericArray2D* image = pair->second;

    long type = image->GetTypeSize();
    if (image->IsFloatingPoint())
        type |= M_FLOAT;
    else
        type |= M_UNSIGNED;

    MIL_ID ID;
    MbufAlloc2d(m_milSystemID, image->GetCo(), image->GetLi(), type, M_IMAGE + M_PROC, &ID);
    MbufPut(ID, image->GetDataAsVoid());

    return ID;
}

CH3GenericArray2D* CFrGrTreatementWrapper::AccessWrappedPhaseOrMask(TypeOfFrame typeOfFrame, int index)
{
    auto pair = m_FrGrTreatment.m_ResultImages.find((TypeOfFrame)(typeOfFrame + index));
    if (pair == m_FrGrTreatment.m_ResultImages.end())
        return 0;

    return pair->second;
}

bool CFrGrTreatementWrapper::GetIncrementalResultList(TypeOfFrame pTypeOfFrame[], int pIndex[], int& nbResults)
{
    return m_FrGrTreatment.FrGr_GetIncrementalResultList(pTypeOfFrame, pIndex, nbResults);
}

bool CFrGrTreatementWrapper::SetInputImage(MIL_ID ImageID, int Index)
{
    if (m_IIC.m_Images[Index] != NULL)
        return false;

    int imageSize = m_IIC.GetSizeX()*m_IIC.GetSizeY();
    m_IIC.m_Images[Index] = new BYTE[imageSize];
    MbufGet(ImageID, m_IIC.m_Images[Index]);
    return true;
}

CFrGrTreatementWrapper::~CFrGrTreatementWrapper()
{
	if(m_crossImg!=M_NULL)
	{
		MbufFree(m_crossImg);
		m_crossImg= M_NULL;
	}
}