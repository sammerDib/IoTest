// H3_HoloMap_Base.h: interface for the CH3_HoloMap_Base class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3_HoloMap_Base_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_)
#define AFX_H3_HoloMap_Base_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "H3ARRAY2D.h"
#include "CoVeLi_Struct.h"
#include "SCalibResult.h"
#include "NormalPosition.h"

class CH3_HoloMap_Base  
{ 
public:
	CH3_HoloMap_Base();
	virtual ~CH3_HoloMap_Base();

	//>> entrer un calibrage cam fait ailleurs
	int Calibrer(SCalibResults& CR,
				const CH3Camera& C,//camera calibrée
				const CExtrinsic_param& ep_ObjRef_camFrame,	//la position du wafer ref dans le rep camera
				const H3_ARRAY2D_FLT32 & Mes_MirrorX, //pour determiner la position du wafer ref//Metrique sur la mireHMap
				const H3_ARRAY2D_FLT32 & Mes_MirrorY, //pour determiner la position du wafer ref//Metrique sur la mireHMap
				const H3_ARRAY2D_UINT8 & UWMirrorMask);

	int init(const CH3Camera & Cam, const CExtrinsic_param& waferToCam, const CExtrinsic_param& screenToCam);

	//mesure d'un objet 1)quasi plan et 2) dans le plan du miroir plan qui a servi au calibrage. A la fin z=0
	int Mesurer_Z0   (SMesure& Mes,const H3_ARRAY2D_FLT32 & aOnMireHMap_X, const H3_ARRAY2D_FLT32 & aOnMireHMap_Y, const H3_ARRAY2D_UINT8 & aOnMireHMap_MASK);

	//mesure d'un objet qcq dont on connait la valeur Z dans le rep de calibrage pour le point vu sur le pixel (nRefX,nrefY)
	int Mesurer_Zreal(SMesure& Mes,const H3_ARRAY2D_FLT32 & aOnMireHMap_X, const H3_ARRAY2D_FLT32 & aOnMireHMap_Y, const H3_ARRAY2D_UINT8 & aOnMireHMap_MASK, const size_t nPixRefX, const size_t nPixRefY, const float fRefZ);
	
	unsigned long Parametrer(unsigned long s){unsigned long oldstep=m_Step; m_Step=s; return oldstep;};

	//bool LoadSettings(CString strFileName=_T(""), CString strSection=_T(""));

private:
	CString m_strFileLog;
	long m_nFileLog;


	bool m_bSmoothEnd;

	//le repere piece est la reference
	CH3Camera mCam;
	CExtrinsic_param m_ep_MireHMAP;	//passage repere MireHMAP vers rep piece
	CExtrinsic_param m_ep_Cam;		//passage repere cam vers rep piece

	NormalPosition m_NP;
	unsigned long m_Step;			//1 pixel par carré de m_Step^2 est utilisé lors du calibrage
};


#endif // !defined(AFX_H3_HoloMap_Base_H__9D311E5B_2822_4AE0_9566_2C5EFB866A6E__INCLUDED_)
