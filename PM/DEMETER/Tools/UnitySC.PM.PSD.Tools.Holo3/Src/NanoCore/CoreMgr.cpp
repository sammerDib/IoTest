#include "StdAfx.h"
#include "CoreMgr.h"
#include "TreatHandler.h"
#include "H3Matrix.h"

#include <omp.h>

#include "H3Array2D.h"
#include "H3AppToolsDecl.h"
#include "H3_HoloMap_AltaTypeExport.h"
#include "MeasureInfoClass.h"

#include <opencv2/core/core.hpp>
using namespace cv;

#include "FreeImagePlus.h"
#ifdef _DEBUG
#pragma comment (lib , "FreeImaged")
#pragma comment (lib , "FreeImagePlusd")
#else
#pragma comment (lib , "FreeImage")
#pragma comment (lib , "FreeImagePlus")
#endif


#define DBGFLG_SAVEDATA		0x00000001
#define DBGFLG_TIMING		0x00000002

static bool g_bInitCalibSys = false;

CCoreMgr::CCoreMgr(void)
{	
	m_pTreatDegauchyssage	= nullptr;
	m_pTreatPrepareData		= nullptr;
	m_pTreatReconstruct		= nullptr;
	m_pTreatFiltering		= nullptr;
	m_pTreatGenerateResults = nullptr;

	m_pInputPhasePictureArray = nullptr;

	tmapTreatInitPrm oMap;
	m_VecTreatmentName.reserve(TreatMax);
	m_VecTreatmentDbgFlag.reserve(TreatMax);
	m_VecTreatmentPrmMap.reserve(TreatMax);

	for (int i=0; i<TreatMax;i++ )
	{
		m_VecTreatmentName.push_back(_T(""));
		m_VecTreatmentDbgFlag.push_back(0);
		m_VecTreatmentPrmMap.push_back(oMap);
	}

#ifdef DEBUG
    SetFilesGeneration(DBGFLG_SAVEDATA);
#else
    m_uFlag = 0;
#endif
	m_nOffsetExpandX_px = 0;
	m_nOffsetExpandY_px = 0;
	m_nNUIEnable = 0;
}

CCoreMgr::~CCoreMgr(void)
{
	ResetMatrix();
	ReleaseTreatments();
	m_VecTreatmentName.clear();
	m_VecTreatmentDbgFlag.clear();
	m_VecTreatmentPrmMap.clear();
    DeleteInputPictures();
}

void CCoreMgr::DeleteTreatment(INanoTopoTreament* &p_ppTreatment)
{
	if(p_ppTreatment != nullptr)
	{
		CTreatHandler *pHandler = p_ppTreatment->GetHandler();
		if (pHandler)
		{
			pHandler->DestroyTreatment(); // release treatment
			delete pHandler;
		}
		p_ppTreatment = nullptr;
	}
}


bool CCoreMgr::SetTreatmentName( UINT p_nTreatID, CString p_csTreatName )
{
	if (p_nTreatID >= TreatMax)
	{
		LogThis(1,4,Fmt(_T("SetTreatmentName : Unkwown treatment ID number = %d for %s"), p_nTreatID, p_csTreatName));
		return false;
	}
	CString csOldName = m_VecTreatmentName[p_nTreatID];
	m_VecTreatmentName[p_nTreatID] = p_csTreatName;
	return true;
}

bool CCoreMgr::SetTreatmentDbgFlag( UINT p_nTreatID, unsigned int p_uDbgFlag )
{
	if (p_nTreatID >= TreatMax)
	{
		LogThis(1,4,Fmt(_T("SetTreatmentName : Unkwown treatment ID number = %d"), p_nTreatID));
		return false;
	}
	
	m_VecTreatmentDbgFlag[p_nTreatID] = p_uDbgFlag;
	return true;
}

bool CCoreMgr::SetTreatmentPrm( UINT p_nTreatID, CString p_csTreatPrm, CString p_csTreatValue )
{
	if (p_nTreatID >= TreatMax)
	{
		LogThis(1,4,Fmt(_T("SetTreatmentPrm : Unkwown treatment ID number = %d for Prm[ %s ]"), p_nTreatID,p_csTreatPrm));
		return false;
	}

	AddTreatInitPrmStr(m_VecTreatmentPrmMap[p_nTreatID],p_csTreatPrm,p_csTreatValue);
	return true;
}

bool CCoreMgr::InitTreatment(INanoTopoTreament** p_pTreatment, CString& p_csTreatmentName, tmapTreatInitPrm& p_pTreatmentPrmMap, unsigned int& p_uDbgFlag )
{
	bool	bTreatIsLocal = false;
	CString csLocal=_T(".local");
	if(p_csTreatmentName.Right(csLocal.GetLength()).CollateNoCase(csLocal) == 0)
		bTreatIsLocal = true;

	CString csTreatnameRedux = p_csTreatmentName;
	if (bTreatIsLocal)
	{
		csTreatnameRedux = csTreatnameRedux.Left(csTreatnameRedux.GetLength()-csLocal.GetLength());
	}

	if((*p_pTreatment) != 0)
	{
		if((*p_pTreatment)->GetName().CompareNoCase(csTreatnameRedux) == 0)
		{
			//it is the same treatment just update prm
			(*p_pTreatment)->Init(p_pTreatmentPrmMap, _calibFolder);
			return true;
		}
		else
		{
			// delete old treatment
			DeleteTreatment((*p_pTreatment));
			(*p_pTreatment) = nullptr;
		}
	}

	CTreatHandler* pTreatHandler = new CTreatHandler(); // instanciate handler this handler will be kept by the treatment and deleted in  CCoreMgr::DeleteTreatment
	HRESULT hr;
    if (bTreatIsLocal)
    {
        hr = pTreatHandler->CreateTreatmentLocal(p_pTreatment, csTreatnameRedux);
    }
	else	
		hr = pTreatHandler->CreateTreatmentDLL(p_pTreatment, csTreatnameRedux);


	if(FAILED(hr))
	{
		LogThis(1,4,Fmt(_T("Unable to Create treatment <%s>"),csTreatnameRedux));
		return false;
	}

	(*p_pTreatment)->SetDbgFlag(p_uDbgFlag); //SetDbgFlag SHOULD be called before Init !!!
	(*p_pTreatment)->Init(p_pTreatmentPrmMap, _calibFolder);
	return true;
}

bool CCoreMgr::InitTreatments(const CString& calibFolder)
{
    _calibFolder = calibFolder;
	bool bRes = true;
	
	//
	// Traitement Degauchy
	//
	if( ! InitTreatment(&m_pTreatDegauchyssage, m_VecTreatmentName[TreatDegauchy], m_VecTreatmentPrmMap[TreatDegauchy], m_VecTreatmentDbgFlag[TreatDegauchy]))
	{
		LogThis(1,4,Fmt(_T("Cannot complete Initialisation of Degauchy treatment (%s) - (Nb Init Prm = %d)"),m_VecTreatmentName[TreatDegauchy],m_VecTreatmentPrmMap[TreatDegauchy].size()));
		bRes = false;
	}

	//
	// Prepare data treament (extrapole, fringe killer ...)
	//
	if( ! InitTreatment(&m_pTreatPrepareData, m_VecTreatmentName[TreatPrepareData], m_VecTreatmentPrmMap[TreatPrepareData], m_VecTreatmentDbgFlag[TreatPrepareData]))
	{
		LogThis(1,4,Fmt(_T("Cannot complete Initialisation of Preparedata treatment (%s) - (Nb Init Prm = %d)"),m_VecTreatmentName[TreatPrepareData],m_VecTreatmentPrmMap[TreatPrepareData].size()));
		bRes  = false;
	}

	//
	// Reconstruct treament (Compute vignette height sample and gather them to generate non filtered height image)
	//
	if( ! InitTreatment(&m_pTreatReconstruct, m_VecTreatmentName[TreatReconstruct], m_VecTreatmentPrmMap[TreatReconstruct], m_VecTreatmentDbgFlag[TreatReconstruct]))
	{
		LogThis(1,4,Fmt(_T("Cannot complete Initialisation of Reconstruct treatment (%s) - (Nb Init Prm = %d)"),m_VecTreatmentName[TreatReconstruct],m_VecTreatmentPrmMap[TreatReconstruct].size()));
		bRes  = false;
	}

	//
	// Filter Image global heigths (H->Hf) 
	//
	if( ! InitTreatment(&m_pTreatFiltering, m_VecTreatmentName[TreatFilter], m_VecTreatmentPrmMap[TreatFilter], m_VecTreatmentDbgFlag[TreatFilter]))
	{
		LogThis(1,4,Fmt(_T("Cannot complete Initialisation of Filter treatment (%s) - (Nb Init Prm = %d)"),m_VecTreatmentName[TreatFilter],m_VecTreatmentPrmMap[TreatFilter].size()));
		bRes  = false;
	}

	//
	// Generate results (such as PV10 - PV5 - THA@0.05 and generate ADN files)
	//
	if( ! InitTreatment(&m_pTreatGenerateResults, m_VecTreatmentName[TreatGenerateResults], m_VecTreatmentPrmMap[TreatGenerateResults], m_VecTreatmentDbgFlag[TreatGenerateResults]))
	{
		LogThis(1,4,Fmt(_T("Cannot complete Initialisation of Results generation treatment (%s) - (Nb Init Prm = %d)"),m_VecTreatmentName[TreatGenerateResults],m_VecTreatmentPrmMap[TreatGenerateResults].size()));
		bRes  = false;
	}

	if(g_bInitCalibSys == false)
	{
        LogThis(1, 4, Fmt(_T("Init calib")));
		H3_InitSys(calibFolder);
		g_bInitCalibSys = true;
        LogThis(1, 4, Fmt(_T("Init sys")));
	}
	
	return true;
}

bool CCoreMgr::ReleaseTreatments()
{
	DeleteTreatment(m_pTreatDegauchyssage);
	DeleteTreatment(m_pTreatPrepareData);
	DeleteTreatment(m_pTreatReconstruct);
	DeleteTreatment(m_pTreatFiltering);
	DeleteTreatment(m_pTreatGenerateResults);

	return true;
}

void CCoreMgr::SaveIntermediateFiles(const CString& folder)
{
    CreateDir(*(CString*)&folder);

    if (m_matNX != 0)
    {
        m_matNX->CopyToNewFreeimage().save(folder + "\\NX.tif", TIFF_NONE);
    }
    if (m_matNY!= 0)
    {
        m_matNY->CopyToNewFreeimage().save(folder + "\\NY.tif", TIFF_NONE);
    }

    if (m_matPX != 0)
    {
        m_matPX->CopyToNewFreeimage().save(folder + "\\PX.tif", TIFF_NONE);
    }
    if (m_matPY != 0)
    {
        m_matPY->CopyToNewFreeimage().save(folder + "\\PY.tif", TIFF_NONE);
    }

    if (m_matMask != 0)
    {
        m_matMask->CopyToNewFreeimage().save(folder + "\\mask.tif", TIFF_NONE);
    }

    if (m_matMaskErode != 0)
    {
        m_matMaskErode->CopyToNewFreeimage().save(folder + "\\maksErode.tif", TIFF_NONE);
    }

    if (m_matMaskDilate != 0)
    {
        m_matMaskDilate->CopyToNewFreeimage().save(folder + "\\maskDilate.tif", TIFF_NONE);
    }

    if (m_matH != 0)
    {
        m_matH->CopyToNewFreeimage().save(folder + "\\H.tif", TIFF_NONE);
    }

    if (m_matHf != 0)
    {
        m_matHf->CopyToNewFreeimage().save(folder + "\\Hf.tif", TIFF_NONE);
    }
}


bool CCoreMgr::LoadPreCalibrateImages()
{
	// pour debug 
// 	m_csAcqImagesPath	= _T("C:\\Altasight\\Nano\\Data");
// 	m_csAcqImagesName	= _T("T37deg4");

	if (LoadFloatMatrices(MATID_NX) == false)
	{
		return false;
	}
	if (LoadFloatMatrices(MATID_NY) == false)
	{
		return false;
	}
	if (LoadFloatMatrices(MATID_PX) == false)
	{
		return false;
	}
	if (LoadFloatMatrices(MATID_PY) == false)
	{
		return false;
	}

	// Load Binary Mask
	FILE *pFile = 0;
	int nErr = 0;
	CString csFile;

	csFile.Format(_T("%s\\%s_mask.bin"),m_csAcqImagesPath,m_csAcqImagesName);
	if(fopen_s(&pFile, (LPCSTR)csFile,"rb+") == 0)
	{
		m_matMask.reset(new H3_MATRIX_UINT8());
		if(m_matMask->fLoadBIN(pFile) == false)
		{
			LogThis(1,4,Fmt(_T("Unable to Load Mask from BIN file : %s"),csFile));
			fclose(pFile);
			return false;
		}
        fclose(pFile);
    }
	else
	{
        // Try .tif format.
        csFile.Format(_T("%s\\%s_mask.tif"), m_csAcqImagesPath, m_csAcqImagesName);
        fipImage tif;
        if (!tif.load(csFile))
        {
            LogThis(1, 4, Fmt(_T("Unable to Open Mask from BIN or TIF file : %s"), csFile));
            return false;
        }
        CImageByte ib;
        ib.CopyFrom(&tif);
        m_matMask.reset(new H3_MATRIX_UINT8(ib.nLi, ib.nCo));
        for (long i = 0; i < (ib.nLi * ib.nCo); i++)
        {
            if (ib.pData[i] > 0)
            {
                (m_matMask->GetData())[i] = 1;
            }
            else
            {
                (m_matMask->GetData())[i] = 0;
            }
        }
	}

	LogThis(1,1,Fmt(_T("Mask successfully loaded from file : %s"),csFile));

	// for T0 & T1
//	int nx = 155; //170
//	int ny = 227; //220;
//	int nszx = 2701;
//	int nszy = 2501;
//	//H3_MATRIX_UINT8 temp = m_matMask->SubMat(30,280,2600-30+1,3020-280+1);
//	H3_MATRIX_UINT8 temp = m_matMask->SubMat(ny,nx,nszy,nszx);
//	m_matMask.reset(new H3_MATRIX_UINT8(temp));

	return true;
}


// IBE: for NanoTopo stand-alone with Tiff images as input
bool CCoreMgr::LoadPreCalibratedTiff()
{
    CString csFile1;
    fipImage tif;

    // NX
    csFile1.Format(_T("%s\\NX.tif"), m_csAcqImagesPath);

    
    if (!tif.load(csFile1))
    {
        LogThis(1, 4, Fmt(_T("Unable to Open NX from TIF file : %s"), csFile1));
        return false;
    }

    // Saving tif image in an object
    m_matNX.reset(new H3_MATRIX_FLT32());

    if (!m_matNX->Alloc(tif.getHeight(), tif.getWidth()))
        return false;

    float* pData = m_matNX->GetData();
    for (int iPix = tif.getHeight()-1; iPix >= 0 ; iPix--)   // Put the lines in correct order (starting from top left) for the next steps
    {
        memcpy(pData, tif.getScanLine(iPix), tif.getScanWidth());
        pData += tif.getWidth();
    }

    // NY
    csFile1.Format(_T("%s\\NY.tif"), m_csAcqImagesPath);

    if (!tif.load(csFile1))
    {
        LogThis(1, 4, Fmt(_T("Unable to Open NY from TIF file : %s"), csFile1));
        return false;
    }

    m_matNY.reset(new H3_MATRIX_FLT32());

    if (!m_matNY->Alloc(tif.getHeight(), tif.getWidth()))
        return false;

    pData = m_matNY->GetData();
    for (int iPix = tif.getHeight() - 1; iPix >= 0; iPix--)   // Put the lines in correct order (starting at top left) for the next steps
    {
        memcpy(pData, tif.getScanLine(iPix), tif.getScanWidth());
        pData += tif.getWidth();
    }

    // PX
    csFile1.Format(_T("%s\\PX.tif"), m_csAcqImagesPath);

    if (!tif.load(csFile1))
    {
        LogThis(1, 4, Fmt(_T("Unable to Open PX from TIF file : %s"), csFile1));
        return false;
    }

    m_matPX.reset(new H3_MATRIX_FLT32());

    m_matPX->Alloc(tif.getHeight(), tif.getWidth());

    pData = m_matPX->GetData();
    for (int iPix = tif.getHeight() - 1; iPix >= 0; iPix--)   // Put the lines in correct order (starting at top left) for the next steps
    {
        memcpy(pData, tif.getScanLine(iPix), tif.getScanWidth());
        pData += tif.getWidth();
    }

    // PY
    csFile1.Format(_T("%s\\PY.tif"), m_csAcqImagesPath);

    if (!tif.load(csFile1))
    {
        LogThis(1, 4, Fmt(_T("Unable to Open PY from TIF file : %s"), csFile1));
        return false;
    }

    m_matPY.reset(new H3_MATRIX_FLT32());

    m_matPY->Alloc(tif.getHeight(), tif.getWidth());

    pData = m_matPY->GetData();
    for (int iPix = tif.getHeight() - 1; iPix >= 0; iPix--)   // Put the lines in correct order (starting at top left) for the next steps
    {
        memcpy(pData, tif.getScanLine(iPix), tif.getScanWidth());
        pData += tif.getWidth();
    }

    // Mask
    csFile1.Format(_T("%s\\mask.tif"), m_csAcqImagesPath);

    if (!tif.load(csFile1))
    {
        LogThis(1, 4, Fmt(_T("Unable to Open Mask from TIF file : %s"), csFile1));
        return false;
    }
    CImageByte ib;
    ib.CopyFrom(&tif);
    m_matMask.reset(new H3_MATRIX_UINT8(ib.nLi, ib.nCo));

    for (long i = 0; i < (ib.nLi * ib.nCo); i++)
    {
        if (ib.pData[i] > 0)
        {
            (m_matMask->GetData())[i] = 1;
        }
        else
        {
            (m_matMask->GetData())[i] = 0;
        }
    }

    LogThis(1, 1, Fmt(_T("Mask successfully loaded from file : %s"), csFile1));

    return true;
}


bool CCoreMgr::LoadFloatMatrices(int p_nId)
{
	H3_MATRIX_FLT32* pFm = 0;
	CString csId;

	switch(p_nId)
	{
	case MATID_NX: m_matNX.reset(new H3_MATRIX_FLT32()); pFm = m_matNX.get(); csId = _T("NX"); break;
	case MATID_NY: m_matNY.reset(new H3_MATRIX_FLT32()); pFm = m_matNY.get(); csId = _T("NY");  break;
	case MATID_PX: m_matPX.reset(new H3_MATRIX_FLT32()); pFm = m_matPX.get(); csId = _T("PX");  break;
	case MATID_PY: m_matPY.reset(new H3_MATRIX_FLT32()); pFm = m_matPY.get(); csId = _T("PY");  break;
	default:
		LogThis(1,4,Fmt(_T("Matrix ID = <%d> is not defined"), p_nId));
		return false;
		break;
	}

	FILE *pFile = 0;
	int nErr = 0;
	CString csFile;
	// CHECK BIN FILE
	csFile.Format(_T("%s\\%s_%s.bin"),m_csAcqImagesPath,m_csAcqImagesName,csId);
	if(fopen_s(&pFile, (LPCSTR)csFile,"rb+") == 0)
	{
		pFm->Free();
		if(pFm->fLoadBIN(pFile) == false)
		{
			LogThis(1,4,Fmt(_T("Unable to Load Matrix <%s> from BIN file : %s"),csId,csFile));
			fclose(pFile);
			return false;
		}
	}
	else
	{
		// CHECK HBF FILE
		csFile.Format(_T("%s\\%s_%s.hbf"),m_csAcqImagesPath,m_csAcqImagesName,csId);
		if(fopen_s(&pFile, (LPCSTR)csFile,"rb+") == 0)
		{
			pFm->Free();
			if(pFm->fLoadHBF(pFile) == false)
			{
				LogThis(1,4,Fmt(_T("Unable to Load Matrix <%s> from HBF file : %s"),csId,csFile));
				fclose(pFile);
				return false;
			}
		}
		else
		{
			LogThis(1,4,Fmt(_T("Unable to Open Matrix <%s> from HBF file : %s"),csId,csFile));
			return false;
		}
	}

	LogThis(1,1,Fmt(_T("Matrix <%s> successfully loaded from file : %s"),csId,csFile));
	fclose(pFile);
	return true;
}

void CCoreMgr::ReferenceAcquisitionImages(CH3GenericArray2D* pPhaseX, CH3GenericArray2D* pPhaseY, CH3GenericArray2D* pPhaseMask)
{
    CImageFloat* pX=new CImageFloat();
    CImageFloat* pY=new CImageFloat();
    CImageByte* pMask=new CImageByte();

    pX->reference(pPhaseX);
    pY->reference(pPhaseY);
    pMask->reference(pPhaseMask);

    GiveAcquisitionImages(pX, pY, pMask);
}

void CCoreMgr::GiveAcquisitionImages(CImageFloat* phaseX, CImageFloat* phaseY, CImageByte* phaseMask)
{
    DeleteInputPictures();
    m_pInputPhasePictureArray = new CImageFloat * [2];
    m_pInputPhasePictureArray[0] = phaseX;
    m_pInputPhasePictureArray[1] = phaseY;

    m_pInputMaskImage = phaseMask;
}

bool CCoreMgr::LoadAcquisitionImages(int sourceType)
{
    // Initialisation strucm_pInputPhasePictureArrayture de données	
    // Chargement des images de phases modulees (W) wx et wy

    DeleteInputPictures();
    m_pInputPhasePictureArray = new CImageFloat * [2];
    m_pInputPhasePictureArray[0] = 0;
    m_pInputPhasePictureArray[1] = 0;

    CString extension(".tif");
    if (sourceType == 0)
    {
        extension = ".bin";
    }

    CString strPhasX = m_csAcqImagesPath + _T("\\Pic_") + m_csAcqImagesName + _T("_wX") + extension;
    CString strPhasY = m_csAcqImagesPath + _T("\\Pic_") + m_csAcqImagesName + _T("_wY") + extension;
    CString strMsk = m_csAcqImagesPath + _T("\\Pic_") + m_csAcqImagesName + _T("_msk") + extension;

    if (sourceType != 0)
    {
        // Load tiff.
        fipImage tif;
        if (!tif.load(strPhasX))
        {
            LogThis(1, 4, Fmt(_T("Could not load Phase Image X : {%s} -- Wrong Path or file extension"), strPhasX));
            DeleteInputPictures();
            return false;
        }

        CImageFloat* pImage = new CImageFloat();
        pImage->CopyFrom(&tif);
        m_pInputPhasePictureArray[0] = pImage;

        if (!tif.load(strPhasY))
        {
            LogThis(1, 4, Fmt(_T("Could not load Phase Image Y : {%s} -- Wrong Path or file extension"), strPhasY));
            DeleteInputPictures();
            return false;
        }
        pImage = new CImageFloat();
        pImage->CopyFrom(&tif);
        m_pInputPhasePictureArray[1] = pImage;

        if (!tif.load(strMsk))
        {
            LogThis(1, 4, Fmt(_T("Could not load Mask Image : {%s} -- Wrong Path or file extension"), strMsk));
            DeleteInputPictures();
            return false;
        }
        m_pInputMaskImage = new CImageByte();
        m_pInputMaskImage->CopyFrom(&tif);
    }
    else
    {
        CImageFloat* pImage = new CImageFloat();
        if (pImage->Load(strPhasX))
        {
            m_pInputPhasePictureArray[0] = pImage;
        }
        else
        {
            delete(pImage);
            LogThis(1, 4, Fmt(_T("Could not load Phase Image X : {%s} -- Wrong Path or file extension"), strPhasX));
            DeleteInputPictures();
            return false;
        }

        pImage = new CImageFloat();
        if (pImage->Load(strPhasY))
        {
            m_pInputPhasePictureArray[1] = pImage;
        }
        else
        {
            delete(pImage);
            LogThis(1, 4, Fmt(_T("Could not load Phase Image Y : {%s} -- Wrong Path or file extension"), strPhasY));
            DeleteInputPictures();
            return false;
        }
        // // Inversion de la phase suite erreur calibration chez SEH 
        // 	for (int i = 0; i <m_pInputPhasePictureArray[1].nLi*m_pInputPhasePictureArray[1].nCo; i++ )
        // 	{
        // 		m_pInputPhasePictureArray[1].pData[i] *= -1.0f; 
        // 	}

            // Chargement de l'image masque wmask
        m_pInputMaskImage = new CImageByte();
        if (m_pInputMaskImage->Load(strMsk) == false)
        {
            LogThis(1, 4, Fmt(_T("Could not load Mask Image : {%s} -- Wrong Path or file extension"), strMsk));
            DeleteInputPictures();
            return false;
        }
    }

	return true;
}

bool CCoreMgr::SlopeCalculation(int p_nPixelPeriod, const bool bUnwrappedPhase)
{
	if(m_pInputPhasePictureArray == nullptr)
	{
		LogThis(1,4,Fmt(_T("Input Phase Data are empty !")));
		return false;
	}

	if(g_bInitCalibSys == false)
	{
		H3_InitSys(_calibFolder);// TODO sde second H3_InitSys really usefull?
		g_bInitCalibSys = true;
	}

	unsigned int nCrossX = 0;
	unsigned int nCrossY = 0;

    bool saveData = (m_uFlag & DBGFLG_SAVEDATA) != 0;

	// Mesurer
	CMeasureInfoClass	measure;
	float fratio = (float)p_nPixelPeriod / 16.0f ;
	measure.SetData(nCrossX, nCrossY, m_pInputPhasePictureArray, m_pInputMaskImage, fratio);
	int nAppreciationMeasure = -1;
	Mesurer(&measure, bUnwrappedPhase, saveData, nAppreciationMeasure);
	
	// Resultats
	const long nLi= (long)m_pInputMaskImage->nLi,nCo=(long)m_pInputMaskImage->nCo,nSz=nLi*nCo;
	const size_t nNbImages = 5L;
	CImageFloat* pArrayOfPicture = new CImageFloat[nNbImages];
	for(size_t i=0; i<nNbImages; i++)
	{
		pArrayOfPicture[i].pData = new float[nLi*nCo];
	}
	bool bSuccess = true;
	if(0 == nAppreciationMeasure)
	{
		measure.GetData(pArrayOfPicture);

		m_matMask.reset(new H3_MATRIX_UINT8(nLi,nCo));
		m_matPX.reset(new H3_MATRIX_FLT32(nLi,nCo));
		m_matPY.reset(new H3_MATRIX_FLT32(nLi,nCo));
		m_matNX.reset(new H3_MATRIX_FLT32(nLi,nCo));
		m_matNY.reset(new H3_MATRIX_FLT32(nLi,nCo));
		H3_MATRIX_FLT32 NZ(nLi,nCo);

		m_matPX->Fill(NaN);  m_matPY->Fill(NaN);  
		m_matNX->Fill(NaN);  m_matNY->Fill(NaN); NZ.Fill(NaN);
		// 	m_matPX->Fill(0.0f);  m_matPY->Fill(0.0f);  
		// 	m_matNX->Fill(0.0f);  m_matNY->Fill(0.0f); NZ.Fill(0.0f);

		#pragma omp parallel for 
		for(long i=0; i<(nLi*nCo); i++)
		{
            // Mask using only 0 an 1 values.
            if (m_pInputMaskImage->pData[i] > 0)
            {
                (m_matMask->GetData())[i] = 1;

                (m_matNX->GetData())[i] = pArrayOfPicture[0].pData[i];
                (m_matNY->GetData())[i] = pArrayOfPicture[1].pData[i];
                NZ[i] = pArrayOfPicture[2].pData[i];
                (m_matPX->GetData())[i] = pArrayOfPicture[3].pData[i];
                (m_matPY->GetData())[i] = pArrayOfPicture[4].pData[i];
            }
            else
            {
                (m_matMask->GetData())[i] = 0;
            }
		}

		if (saveData)
		{
            // save result for debug purpose
            CString sOutFolder = m_csResultsPath + _T("\\Dbg\\") + m_csAcqImagesName + _T("\\") ;
			CreateDir(sOutFolder);
			CString csTime = (CTime::GetCurrentTime()).Format(_T("%Y%m%d_%H%M%S"));
			
			CString csPrefixe;
			csPrefixe.Format(_T("Prec-%d-%s_"), p_nPixelPeriod, csTime);

			CString csFileName;
			csFileName.Format(_T("%s%s.bin"), sOutFolder, csPrefixe +_T("_mask"));
			FILE* pFile = 0;
			if(fopen_s(&pFile, (LPCSTR)csFileName,"wb+") == 0)
			{
				bool bSavRes =(*m_matMask.get()).fSaveBIN(pFile);
				fclose(pFile);
			}
			else
			{
				CString csMsg;
				csMsg.Format(_T("Could not Save Matrix Mask in : {%s}"), sOutFolder);
				LogThis(1,4,csMsg);
			}

			if(SaveMatrix(*m_matNX.get(),sOutFolder, csPrefixe + _T("_NX"))== false)
			{
				CString csMsg;
				csMsg.Format(_T("Could not Save Matrix NX in : {%s}"), sOutFolder);
				LogThis(1,4,csMsg);
			}
			if(SaveMatrix(*m_matNY.get(),sOutFolder, csPrefixe +_T("_NY"))== false)
			{
				CString csMsg;
				csMsg.Format(_T("Could not Save Matrix NY in : {%s}"), sOutFolder);
				LogThis(1,4,csMsg);
			}
// 			if(SaveMatrix(NZ,sOutFolder, csPrefixe + _T("_NZ"))== false)
// 			{
// 				CString csMsg;
// 				csMsg.Format(_T("Could not Save Matrix NZ in : {%s}"), sOutFolder);
// 				LogThis(1,4,csMsg);
// 			}

			if(SaveMatrix(*m_matPX.get(),sOutFolder,csPrefixe +_T("_PX"))== false)
			{
				CString csMsg;
				csMsg.Format(_T("Could not Save Matrix PX in : {%s}"), sOutFolder);
				LogThis(1,4,csMsg);
			}
			if(SaveMatrix(*m_matPY.get(),sOutFolder, csPrefixe +_T("_PY"))== false)
			{
				CString csMsg;
				csMsg.Format(_T("Could not Save Matrix PY in : {%s}"), sOutFolder);
				LogThis(1,4,csMsg);
			}
		}
	}
	else
	{
		LogThis(1,4,Fmt("H3 Measure Failed ! {Error Id code = %d}",nAppreciationMeasure));
		bSuccess = false;
	}


	// Libérer la mémoire
	if(pArrayOfPicture!=nullptr)
	{
		for(size_t i=0; i<nNbImages; i++)
		{
			if (pArrayOfPicture[i].pData!=nullptr)
			{
				delete[] pArrayOfPicture[i].pData;
				pArrayOfPicture[i].pData = nullptr;
			}
		}
		delete [] pArrayOfPicture;
		pArrayOfPicture=nullptr;
	}

	DeleteInputPictures();

	return bSuccess;
}

bool CCoreMgr::Degauchyssage()
{
	bool bRes = true;
	if (m_pTreatDegauchyssage)
	{
		tmapTreatPrm oIn;
		int nSaveData = m_uFlag & DBGFLG_SAVEDATA;
		AddTreatPrmPtr(oIn,"Save",(void*)&nSaveData);
		AddTreatPrmPtr(oIn,"OutPath",(void*)&m_csResultsPath);
		AddTreatPrmPtr(oIn,"LotID", (void*)&m_csAcqImagesName); 
		AddTreatPrmSharedPtr(oIn,"NX",shared_ptr<void>(m_matNX));
		AddTreatPrmSharedPtr(oIn,"NY",shared_ptr<void>(m_matNY));
		AddTreatPrmSharedPtr(oIn,"Mask",shared_ptr<void>(m_matMask));

		tmapTreatPrm oOut;
		CString csOut = _T("");
		AddTreatPrmPtr(oOut,"CS",(void*) &csOut);

		bRes = m_pTreatDegauchyssage->Exec(oIn,oOut);

		if( ! bRes)
			LogThis(1,4,Fmt(_T("PreTreatment /!\\ FAILURE /!\\ = %s"),csOut));

		// clear Map & SharedPointer...
		oIn.RemoveAll();

		// NX and NY have been modified  m_matNX & m_matNY

		// clear Map & SharedPointer...
		oOut.RemoveAll();
	}
	else
	{
		LogThis(1,4,Fmt(_T("Degauchy treatment is not initialized")));
	}

	return bRes;
}

bool CCoreMgr::PrepareData()
{
	bool bRes = true;
	if (m_pTreatPrepareData)
	{
		tmapTreatPrm oIn;
		int nSaveData = m_uFlag & DBGFLG_SAVEDATA;
		AddTreatPrmPtr(oIn,"Save",(void*)&nSaveData);
		AddTreatPrmPtr(oIn,"OutPath",(void*)&m_csResultsPath);
		AddTreatPrmPtr(oIn,"LotID", (void*)&m_csAcqImagesName);
		AddTreatPrmSharedPtr(oIn,"NX",shared_ptr<void>(m_matNX));
		AddTreatPrmSharedPtr(oIn,"NY",shared_ptr<void>(m_matNY));
		AddTreatPrmSharedPtr(oIn,"PX",shared_ptr<void>(m_matPX));
		AddTreatPrmSharedPtr(oIn,"PY",shared_ptr<void>(m_matPY));
		AddTreatPrmSharedPtr(oIn,"Mask",shared_ptr<void>(m_matMask));
		AddTreatPrmPtr(oIn,"OffsetExpand_X",(void*)&m_nOffsetExpandX_px);
		AddTreatPrmPtr(oIn,"OffsetExpand_Y",(void*)&m_nOffsetExpandY_px);

		AddTreatPrmPtr(oIn,"NUIEnable",(void*)&m_nNUIEnable);

		tmapTreatPrm oOut;
		CString csOut = _T("");
		AddTreatPrmPtr(oOut,"CS",(void*) &csOut);

		m_matMaskDilate.reset(new H3_MATRIX_UINT8());

		bRes = m_pTreatPrepareData->Exec(oIn,oOut);
		
		if( ! bRes)
			LogThis(1,4,Fmt(_T("PrepareData treatment /!\\ FAILURE /!\\ = %s"),csOut));

		// clear Map & SharedPointer...
		oIn.RemoveAll();

		// release no more used data for memory sakes
		m_matMask.reset();
		m_matPX.reset();
		m_matPY.reset();

		shared_ptr<void> pvMaskE;
		if(FindTreatPrmSharedPtr(oOut,"MaskE",pvMaskE))
		{
			m_matMaskErode = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskE);
			//LogThis(1,1,Fmt(_T("Mask Erode Size = %d x %d"), m_matMaskErode->GetCo(),m_matMaskErode->GetLi()));
		}
		else
		{
			LogThis(1,4,Fmt(_T("PrepareData : Could not retreive Erode Mask Matrix ")));
			bRes = false;
		}
		pvMaskE.reset();

		shared_ptr<void> pvMaskD;
		if(FindTreatPrmSharedPtr(oOut,"MaskD",pvMaskD))
		{
			m_matMaskDilate = static_pointer_cast<H3_MATRIX_UINT8> (pvMaskD);
			//LogThis(1,1,Fmt(_T("Mask Dilate Size = %d x %d"), m_matMaskDilate->GetCo(),m_matMaskDilate->GetLi()));
		}
		else
		{
			LogThis(1,4,Fmt(_T("PrepareData : Could not retreive Dilate Mask Matrix ")));
			bRes = false;
		}
		pvMaskD.reset();

		// clear Map & SharedPointer...
		oOut.RemoveAll();
	}
	else
	{
		LogThis(1,4,Fmt(_T("PrepareData treatment is not initialized")));
	}

	return bRes;
}

bool CCoreMgr::Reconstruct()
{
	bool bRes = true;
	if (m_pTreatReconstruct)
	{
		tmapTreatPrm oIn;
		int nSaveData =  m_uFlag & DBGFLG_SAVEDATA;
		AddTreatPrmPtr(oIn,"Save",(void*)&nSaveData);
		AddTreatPrmPtr(oIn,"OutPath",(void*)&m_csResultsPath);
		AddTreatPrmPtr(oIn,"LotID", (void*)&m_csAcqImagesName);
		AddTreatPrmSharedPtr(oIn,"NX",shared_ptr<void>(m_matNX));
		AddTreatPrmSharedPtr(oIn,"NY",shared_ptr<void>(m_matNY));
		AddTreatPrmSharedPtr(oIn,"MaskD",shared_ptr<void>(m_matMaskDilate));
		AddTreatPrmSharedPtr(oIn,"MaskE",shared_ptr<void>(m_matMaskErode));

		tmapTreatPrm oOut;
		CString csOut = _T("");
		AddTreatPrmPtr(oOut,"CS",(void*) &csOut);
		AddTreatPrmPtr(oOut,"VignStartPosX",(void*) &m_nVignStartPosX);
		AddTreatPrmPtr(oOut,"VignStartPosY",(void*) &m_nVignStartPosY);
		AddTreatPrmPtr(oOut,"VignSize",(void*) &m_nVignSize);

		bRes = m_pTreatReconstruct->Exec(oIn,oOut);
		if(! bRes )
			LogThis(1,4,Fmt(_T("Reconstruct treatment /!\\ FAILURE /!\\ = %s"),csOut));

		// clear Map & SharedPointer...
		oIn.RemoveAll();

		// release no more used data for memory sakes
		m_matMaskDilate.reset();
		//m_matMaskErode.reset(); // needed for Generate results and filter
		m_matNX.reset();
		m_matNY.reset();

		shared_ptr<void> pvH;
		if(FindTreatPrmSharedPtr(oOut,"H",pvH))
		{
			m_matH = static_pointer_cast<H3_MATRIX_FLT32> (pvH);
			//LogThis(1,1,Fmt(_T("Mask H Size = %d x %d"), m_matH->GetCo(),m_matH->GetLi()));
		}
		else
		{
			LogThis(1,4,Fmt(_T("Reconstruct : Could not retreive H Matrix ")));
			bRes = false;
		}
		pvH.reset();

		// clear Map & SharedPointer...
		oOut.RemoveAll();
	}
	else
	{
		LogThis(1,4,Fmt(_T("Reconstruct treatment is not initialized")));
	}
	return true;
}

bool CCoreMgr::Filtering()
{
	bool bRes = true;
	if (m_pTreatFiltering)
	{
		tmapTreatPrm oIn;
		int nSaveData =  m_uFlag & DBGFLG_SAVEDATA;
		AddTreatPrmPtr(oIn,"Save",(void*)&nSaveData);
		AddTreatPrmPtr(oIn,"OutPath",(void*)&m_csResultsPath);
		AddTreatPrmPtr(oIn,"LotID", (void*)&m_csAcqImagesName);
		AddTreatPrmSharedPtr(oIn,"H",shared_ptr<void>(m_matH));
		AddTreatPrmSharedPtr(oIn,"MaskE",shared_ptr<void>(m_matMaskErode));
		AddTreatPrmPtr(oIn,"VignStartPosX",(void*)&m_nVignStartPosX);
		AddTreatPrmPtr(oIn,"VignStartPosY",(void*)&m_nVignStartPosY);
		AddTreatPrmPtr(oIn,"VignSize",(void*)&m_nVignSize);

		tmapTreatPrm oOut;
		CString csOut = _T("");
		AddTreatPrmPtr(oOut,"CS",(void*) &csOut);
		AddTreatPrmPtr(oOut,"FilterType",(void*) &m_nFilterType);

		bRes = m_pTreatFiltering->Exec(oIn,oOut);
		if( ! bRes )
			LogThis(1,4,Fmt(_T("Filter treatment /!\\ FAILURE /!\\ = %s"),csOut));

		// clear Map & SharedPointer...
		oIn.RemoveAll();

		// release no more used data for memory sakes
		m_matH.reset(); // ?

		shared_ptr<void> pvHf;
		if(FindTreatPrmSharedPtr(oOut,"Hf",pvHf))
		{
			m_matHf = static_pointer_cast<H3_MATRIX_FLT32> (pvHf);
			//LogThis(1,1,Fmt(_T("Mask Hf Size = %d x %d"), m_matHf->GetCo(),m_matHf->GetLi()));
		}
		else
		{
			LogThis(1,4,Fmt(_T("Filter : Could not retreive Hf Matrix ")));
			bRes = false;
		}
		pvHf.reset();

		// clear Map & SharedPointer...
		oOut.RemoveAll();
	}
	else
	{
		LogThis(1,4,Fmt(_T("Filter treatment is not initialized")));
	}
	return true;
}

bool CCoreMgr::GenerateResults()
{
	bool bRes = true;
	if (m_pTreatGenerateResults)
	{
		tmapTreatPrm oIn;
		int nSaveData =  m_uFlag & DBGFLG_SAVEDATA;
		AddTreatPrmPtr(oIn,"Save",(void*)&nSaveData);
		AddTreatPrmPtr(oIn,"OutPath",(void*)&m_csResultsPath);
		AddTreatPrmPtr(oIn,"LotID", (void*)&m_csAcqImagesName);
		AddTreatPrmSharedPtr(oIn,"MaskE",shared_ptr<void>(m_matMaskErode));
		AddTreatPrmSharedPtr(oIn,"Hf",shared_ptr<void>(m_matHf));
		AddTreatPrmPtr(oIn,"OffsetExpand_X",(void*)&m_nOffsetExpandX_px);
		AddTreatPrmPtr(oIn,"OffsetExpand_Y",(void*)&m_nOffsetExpandY_px);
		AddTreatPrmPtr(oIn,"FoupID",(void*)&m_csAcqFOUPName);
		AddTreatPrmPtr(oIn,"FilterType",(void*)&m_nFilterType);

		tmapTreatPrm oOut;
		CString csOut = _T("");
		AddTreatPrmPtr(oOut,"CS",(void*) &csOut);

		bRes = m_pTreatGenerateResults->Exec(oIn,oOut);
		if(! bRes )
			LogThis(1,4,Fmt(_T("GenerateResults treatment /!\\ FAILURE /!\\ = %s"),csOut));

		// clear Map & SharedPointer...
		oIn.RemoveAll();

		// release no more used data for memory sakes
		m_matH.reset();
		m_matHf.reset();
		m_matMaskErode.reset();

		// clear Map & SharedPointer...
		oOut.RemoveAll();
	}
	else
	{
		LogThis(1,4,Fmt(_T("GenerateResults treatment is not initialized")));
	}
	return true;
}

void CCoreMgr::ResetMatrix()
{
	m_matNX.reset();
	m_matNY.reset();
	m_matPX.reset();
	m_matPY.reset();
	m_matMask.reset();
	m_matMaskErode.reset();
	m_matMaskDilate.reset();
	m_matH.reset();
	m_matHf.reset();
}

void CCoreMgr::EmergencyStop()
{
	if(m_pTreatDegauchyssage) m_pTreatDegauchyssage->EmergencyStop();
	if(m_pTreatPrepareData) m_pTreatPrepareData->EmergencyStop();
	if(m_pTreatReconstruct) m_pTreatReconstruct->EmergencyStop();
	if(m_pTreatFiltering) m_pTreatFiltering->EmergencyStop();
	if(m_pTreatGenerateResults) m_pTreatGenerateResults->EmergencyStop();
	ResetMatrix();
}

void CCoreMgr::ClearEmergencyStop()
{
	if(m_pTreatDegauchyssage) m_pTreatDegauchyssage->ClearEmergencyStop();
	if(m_pTreatPrepareData) m_pTreatPrepareData->ClearEmergencyStop();
	if(m_pTreatReconstruct) m_pTreatReconstruct->ClearEmergencyStop();
	if(m_pTreatFiltering) m_pTreatFiltering->ClearEmergencyStop();
	if(m_pTreatGenerateResults) m_pTreatGenerateResults->ClearEmergencyStop();
}

int CCoreMgr::SetFilesGeneration( long p_ulFlags )
{
	m_uFlag = p_ulFlags;
	return 0;
}

int CCoreMgr::SetExpandOffsets(int p_nOffsetX, int p_nOffsetY)
{
	m_nOffsetExpandX_px = p_nOffsetX;
	m_nOffsetExpandY_px = p_nOffsetY;
	return 0;
}

 int CCoreMgr::SetNUI(int p_nNUIEnable)
 {
	 m_nNUIEnable = p_nNUIEnable;
	 return 0;
 }

bool CCoreMgr::SaveMatrix(H3_MATRIX_FLT32& p_oMatrix, CString p_csPath, CString p_csMatrixName)
{
	CString csFileName;
	csFileName.Format(_T("%s%s.bin"), p_csPath, p_csMatrixName);
	bool bRes = false;

	// save Bin file
	FILE* pFile = 0;
	if(fopen_s(&pFile, (LPCSTR)csFileName,"wb+") == 0)
	{
		bRes =p_oMatrix.fSaveBIN(pFile);
		fclose(pFile);
	}

// 	if(bRes)
// 	{
// 		csFileName.Format(_T("%s%s.png"), p_csPath, p_csMatrixName);
// 		bRes = SaveGreyImageFlt32(csFileName,p_oMatrix);
// 	}
	return true;
}

bool CCoreMgr::SaveGreyImageFlt32(CString p_csFilepath, H3_MATRIX_FLT32& p_oMatrixFloat, float p_fMin /*= FLT_MAX*/, float p_fMax /*= FLT_MAX*/, bool bAutoscale /*= true*/)
{
	float* pData = p_oMatrixFloat.GetData();

	unsigned long  lCols = p_oMatrixFloat.GetCo();
	unsigned long  lLines = p_oMatrixFloat.GetLi();

	float fMin = FLT_MAX;
	float fMax = - FLT_MAX;

	bool bUseMinPrm = (p_fMin != FLT_MAX);
	bool bUseMaxPrm = (p_fMax != FLT_MAX);

	float a = 1.0f;
	float b = 0.0f;
	if(bAutoscale && (!bUseMinPrm || !bUseMaxPrm))
	{
		for(long lItem = 0; lItem<p_oMatrixFloat.GetSize(); lItem++)
		{
			if(!bUseMinPrm)
				fMin = __min( fMin, pData[lItem]);
			if(!bUseMaxPrm)
				fMax = __max( fMax, pData[lItem]);
		}
	}
	else
	{
		if(!bUseMaxPrm)
			fMax = 255.0f;
		if(!bUseMinPrm)
			fMin = 0.0f;
	}

	if (bUseMinPrm)
	{
		fMin = p_fMin;
	}
	if (bUseMaxPrm)
	{
		fMax = p_fMax;
	}

	a = 255.0f / (fMax- fMin);
	b = - fMin * 255.0f / (fMax - fMin);

	fipImage oImg(FIT_BITMAP,lCols, lLines,8);
	for(unsigned y = 0; y < oImg.getHeight(); y++)
	{
		//ici pb à resoudre pour affichage image
		BYTE* pbits = (BYTE*) oImg.getScanLine(y);
		for(unsigned x = 0; x < oImg.getWidth(); x++)
		{
			pbits[x] =saturate_cast<uchar>(pData[y*lCols+x] * a + b) ;
		}
	}
	oImg.flipVertical();
	BOOL bRes = oImg.save(p_csFilepath, 0);
	return (bRes !=0) ;
}

void CCoreMgr::DeleteInputPictures()
{
	if(m_pInputPhasePictureArray!=nullptr)
    {
        if (m_pInputPhasePictureArray[0] != 0)
        {
            delete m_pInputPhasePictureArray[0];
        }
        if (m_pInputPhasePictureArray[1] != 0)
        {
            delete m_pInputPhasePictureArray[1];
        }

		delete [] m_pInputPhasePictureArray;
		m_pInputPhasePictureArray=nullptr;
	}

    if (m_pInputMaskImage != 0)
    {
        delete(m_pInputMaskImage);
        m_pInputMaskImage = 0;
    }
}

bool CCoreMgr::IsLogTimingEnable()
{
	return ((m_uFlag & DBGFLG_TIMING) != 0);
}

bool CCoreMgr::GetRegistryFlag(unsigned long p_dwDefaultValue)
{
	// query for registry data if data doesn't exist value is created with default value 
	unsigned long& p_ulRegValue = p_dwDefaultValue;
	HKEY hRegbase			= HKEY_CURRENT_USER;
	CString csNanoKey		= _T("Software\\Altatech\\Nanotopo");
	CString csTreatKey		= "Core";
	csTreatKey				+= "_";
	csTreatKey				+= "Flag";

	HKEY hTreatmentKey = 0;
	DWORD dwValue	= p_dwDefaultValue;  // init with default value
	DWORD dwData	= sizeof(dwValue);
	// OPen registry
	if (RegOpenKeyEx(hRegbase, csNanoKey, 0, KEY_ALL_ACCESS , &hTreatmentKey) == ERROR_SUCCESS) 
	{
		DWORD dwType = 0;
		// query registry data flag
		if (RegQueryValueEx(hTreatmentKey, csTreatKey, 0, &dwType, (BYTE*)(&dwValue), &dwData) != ERROR_SUCCESS)
		{	
			// data value doesnt exist add it with default value
			if (RegSetValueEx(hTreatmentKey, csTreatKey, 0, REG_DWORD, (BYTE *)(&dwValue), dwData) != ERROR_SUCCESS)
			{
				RegCloseKey(hTreatmentKey);
				m_uFlag	|= p_ulRegValue;
				return false;
			}
		}
		else
		{
			p_ulRegValue =  dwValue;
			ASSERT(dwType == REG_DWORD);
		}
		RegCloseKey(hTreatmentKey);
	}
	else
	{
		//key does not exist create it and insert default value
		if (RegCreateKeyEx(hRegbase, csNanoKey, 0, _T(""), REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hTreatmentKey, NULL) == ERROR_SUCCESS)
		{
			if (RegSetValueEx(hTreatmentKey, csTreatKey, 0, REG_DWORD, (BYTE *)(&dwValue), dwData) != ERROR_SUCCESS)
			{
				RegCloseKey(hTreatmentKey);
				m_uFlag	|= p_ulRegValue;
				return false;
			}
			RegCloseKey(hTreatmentKey);
		}
		else
			return false;
	}

	m_uFlag	|= p_ulRegValue;
	return true;
}