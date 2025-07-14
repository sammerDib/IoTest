#pragma once

#include <vector>

#include "TreatmentInterface.h"

#include "H3Matrix.h"
#include "H3IOHoloMAP.h"

enum TreatID
{
	TreatDegauchy        = 0,
	TreatPrepareData     = 1,
	TreatReconstruct     = 2,
	TreatFilter          = 3,
	TreatGenerateResults = 4,
	//---
	TreatMax
};

#define MATID_NX 0
#define MATID_NY 1
#define MATID_PX 2
#define MATID_PY 3

class CCoreMgr
{
public:

	CCoreMgr(void);

	virtual ~CCoreMgr(void);

	bool InitTreatments(const CString& calibFolder);
	bool InitTreatment(INanoTopoTreament** p_pTreatment, CString& p_csTreatmentName, tmapTreatInitPrm& p_pTreatmentPrmMap, unsigned int& p_uDbgFlag);

	bool ReleaseTreatments();

	/// <summary>
	/// Loads phases and mask, from:
    /// sourceType=0: .bin files.
    /// sourceType!=0: .tiff files.
    /// </summary>
	/// <param name="sourceType"></param>
	/// <returns></returns>
	bool LoadAcquisitionImages(int sourceType);

    /// <summary>
    /// Sets new images, which will be owned by this object.
    /// </summary>
    void GiveAcquisitionImages(CImageFloat* phaseX, CImageFloat* phaseY, CImageByte* phaseMask);

    /// <summary>
    /// Sets new images, which will only be referenced by this object.
    /// </summary>
    void ReferenceAcquisitionImages(CH3GenericArray2D* phaseX, CH3GenericArray2D* phaseY, CH3GenericArray2D* phaseMask);

	bool SlopeCalculation(int p_nPixelPeriod, const bool bUnwrappedPhase);
	bool Degauchyssage();
	bool PrepareData();
	bool Reconstruct();
	bool Filtering();
	bool GenerateResults();

	void EmergencyStop();
	void ClearEmergencyStop();

	bool LoadPreCalibrateImages();
    bool LoadPreCalibratedTiff();
	bool IsLogTimingEnable();

private :
    CString _calibFolder;
	bool LoadFloatMatrices(int p_nId);
	void DeleteTreatment(INanoTopoTreament* &p_ppTreatment);
	void ResetMatrix();
	void DeleteInputPictures();
	bool SaveMatrix(H3_MATRIX_FLT32& p_oMatrix, CString p_csPath, CString p_csMatrixName);
	bool SaveGreyImageFlt32(CString p_csFilepath, H3_MATRIX_FLT32& p_oMatrixFloat, float p_fMin = FLT_MAX, float p_fMax = FLT_MAX, bool bAutoscale = true);

public:
	CString m_csAcqImagesPath;
	CString m_csAcqImagesName;
	CString m_csAcqFOUPName;
	CString m_csResultsPath;

    void SaveIntermediateFiles(const CString& folder);

	shared_ptr<H3_MATRIX_FLT32> m_matNX;
	shared_ptr<H3_MATRIX_FLT32> m_matNY;
	shared_ptr<H3_MATRIX_FLT32> m_matPX;
	shared_ptr<H3_MATRIX_FLT32> m_matPY;

	shared_ptr<H3_MATRIX_UINT8> m_matMask;
	shared_ptr<H3_MATRIX_UINT8> m_matMaskErode;
	shared_ptr<H3_MATRIX_UINT8> m_matMaskDilate;

	shared_ptr<H3_MATRIX_FLT32> m_matH;
	shared_ptr<H3_MATRIX_FLT32> m_matHf;

	/// <summary>
	/// This data is first loaded, then copied to m_matNX, ..., m_matMask. TODO sde improve copy.
	/// </summary>
	CImageFloat**	m_pInputPhasePictureArray;
	CImageByte*		m_pInputMaskImage;

	bool	SetTreatmentName(UINT p_nTreatID, CString p_csTreatName);
	bool	SetTreatmentDbgFlag( UINT p_nTreatID, unsigned int p_uDbgFlag );
	bool	SetTreatmentPrm(UINT p_nTreatID, CString p_csTreatPrm, CString p_csTreatValue);
	int		SetFilesGeneration( long p_ulFlags );
	int     SetExpandOffsets(int p_nOffsetX, int p_nOffsetY);
	int		SetNUI(int p_nNUIEnable);
	bool	GetRegistryFlag(unsigned long p_dwDefaultValue);

	inline  bool  IsNUIActive() {return m_nNUIEnable != 0;}

private : 
	INanoTopoTreament* m_pTreatDegauchyssage;
	INanoTopoTreament* m_pTreatPrepareData;
	INanoTopoTreament* m_pTreatReconstruct;
	INanoTopoTreament* m_pTreatFiltering;
	INanoTopoTreament* m_pTreatGenerateResults;

	std::vector<tmapTreatInitPrm>  m_VecCorePrmMap;

	std::vector<CString>			m_VecTreatmentName;							// si le treamtent se finit par .local il s'agit d'un traitement local
	std::vector<unsigned int>		m_VecTreatmentDbgFlag;						// si le treamtent se finit par .local il s'agit d'un traitement local
	std::vector<tmapTreatInitPrm >	m_VecTreatmentPrmMap;

	long	m_uFlag;
	int		m_nOffsetExpandX_px;
	int		m_nOffsetExpandY_px;
	int		m_nNUIEnable; // Non Unuformity Improvement => if activated perfom degauchi during prepare stage not before

	int		m_nVignStartPosX;
	int		m_nVignStartPosY;
	int		m_nVignSize;
	int		m_nFilterType;
};

