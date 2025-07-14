#include "StdAfx.h"
#include "CameraCalibInfoClass.h"
#include "H3_DSLRCalib_Export.h"

CCameraCalibInfoClass::CCameraCalibInfoClass(const CString& calibFolder)
{
    m_CalibFolder = calibFolder;
}

CCameraCalibInfoClass::~CCameraCalibInfoClass(void)
{
}

void CCameraCalibInfoClass::SetData(const unsigned int nMireSizeX,const unsigned int nMireSizeY,const float fMireStepX,const  float fMireStepY,const unsigned int nNbImg,
									CImageByte* pArrayOfVPicture)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString str;

	m_nMireSizeX = nMireSizeX;
	m_nMireSizeY = nMireSizeY;
	m_fMireStepX = fMireStepX;
	m_fMireStepY = fMireStepY;
	m_nNbImage   = nNbImg;
	m_pArrayOfVPicture = pArrayOfVPicture;

}

///////////////////////////////////////////////////////////////////////////////
void CalibrageCamera(CCameraCalibInfoClass *pCalibCam, int &nAppreciationCalibCamera)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState( ))

	const size_t nLi = pCalibCam->m_pArrayOfVPicture[0].nLi;
	const size_t nCo = pCalibCam->m_pArrayOfVPicture[0].nCo;

	CH3Array<H3_ARRAY2D_UINT8> vMirePos(pCalibCam->m_nNbImage);
	
	for(size_t i=0; i<pCalibCam->m_nNbImage; i++)
	{
		vMirePos[i].ReAlloc(nLi,nCo);
		vMirePos[i].Fill(0);
	}
	
	for(int i=0; i<(int)pCalibCam->m_nNbImage; i++)
	{
		size_t l=0L;
		for(size_t j=0; j<nLi; j++) 
		{
			for(size_t k=0; k<nCo; k++)
			{
				vMirePos[i].SetAt(j,k,pCalibCam->m_pArrayOfVPicture[i].pData[l]);
				l++;
			}
		}
	}

	// Calibrer camera
	H3_DSLR_CalibCamera(pCalibCam->m_nMireSizeX,
						pCalibCam->m_nMireSizeY, 
						pCalibCam->m_fMireStepX, 
						pCalibCam->m_fMireStepY, 
						pCalibCam->m_nNbImage, 
						vMirePos, pCalibCam->m_CalibFolder);

	// Lire appreciation sur le calibrage camera
	nAppreciationCalibCamera = H3_DSLR_GetErrorTypeCalibCam();
}


