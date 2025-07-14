#include "StdAfx.h"
#include "SystemCalibInfoClass.h"
#include "H3array2d.h"
#include "H3_HoloMap_AltaTypeExport.h"

CSystemCalibInfoClass::CSystemCalibInfoClass(const CString& calibFolder)
{
    m_CalibFolder = calibFolder;
}

CSystemCalibInfoClass::~CSystemCalibInfoClass(void)
{
}

void CSystemCalibInfoClass::SetData(CImageFloat* pArrayOfWPicture,
								    CImageByte* pVPicture,
								    float fPixSizeX, float fPixSizeY, 
								    float fMireMonStepX, float fMireMonStepY,
									unsigned long pixRef_Xscreen,unsigned long pixRef_Yscreen,
									unsigned long screen_Xsz,unsigned long screen_Ysz,
								    unsigned int nCrossX, unsigned int nCrossY,unsigned int nNbWImg,const float *ratio)
{
//	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString str;

//	CWaitCursor wait;

	m_fPixSizeX = fPixSizeX;
	m_fPixSizeY = fPixSizeY;			
	m_fMireMonStepX = fMireMonStepX;
	m_fMireMonStepY = fMireMonStepY;
	m_pixRef_Xscreen = pixRef_Xscreen;
	m_pixRef_Yscreen = pixRef_Yscreen;
	m_screen_Xsz = screen_Xsz;
	m_screen_Ysz = screen_Ysz;
	m_nCrossX = nCrossX;
	m_nCrossY = nCrossY;
	m_nNbWImage = nNbWImg;
	m_ratio = ratio;

	m_pArrayOfWPicture = pArrayOfWPicture;
	m_pVPicture = pVPicture;
}

///////////////////////////////////////////////////////////////////////////////
void CSystemCalibInfoClass::CalibrageSystem(int &nAppreciationCalibSystem)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState( ))

	// Récuperer les phases
	size_t nLi = m_pArrayOfWPicture[0].nLi;
	size_t nCo = m_pArrayOfWPicture[0].nCo;

	CH3Array<H3_ARRAY2D_FLT32> wPhase(m_nNbWImage);
	for(size_t i=0; i<m_nNbWImage; i++)
	{
		wPhase[i].ReAlloc(nLi,nCo);
		wPhase[i].Fill(0);
	}
	
	for(size_t i=0; i<m_nNbWImage; i++)
	{
		for(size_t j=0; j<nLi*nCo; j++)
		{
			wPhase[i][j]=m_pArrayOfWPicture[i].pData[j];
		}
	}
	//

	// Récuperer l'image vidéo de la mire
	nLi = m_pVPicture->nLi;
	nCo = m_pVPicture->nCo;
	H3_ARRAY2D_UINT8	mireImage(nLi,nCo);
	mireImage.Fill(0);
	for(size_t i=0; i<nLi*nCo; i++)
	{
		mireImage[i] = m_pVPicture->pData[i];
	}
	//

	H3_HoloMap_AltaType_CalibSystem(wPhase,
								    mireImage,
								    m_fPixSizeX,
								    m_fPixSizeY,
								    m_fMireMonStepX,
								    m_fMireMonStepY,
									m_pixRef_Xscreen, m_pixRef_Yscreen,
									m_screen_Xsz,     m_screen_Ysz,
								    m_nCrossX,
								    m_nCrossY,
									m_ratio, m_CalibFolder);

	// Lire appreciation sur le calibrage systeme
	nAppreciationCalibSystem = H3_HoloMap_AltaType_GetErrorTypeCalibSys();
}
