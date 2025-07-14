// MireHMAP.h: interface for the CMireHMAP class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MIREHMAP_H__5F1B0FD5_167A_406A_A493_26CFAA1465EE__INCLUDED_)
#define AFX_MIREHMAP_H__5F1B0FD5_167A_406A_A493_26CFAA1465EE__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "UWImage.h"
#include "CoVeLi_Struct.h"

class CMireHMAP  
{
public:
	CMireHMAP();
	virtual ~CMireHMAP();

	//bool SaveSettings(CString strFileName=CString(""), CString strSection=CString(""));
	//bool LoadSettings(CString strFileName=CString(""), CString strSection=CString(""));
	bool LoadSettings2(CString strFileName=CString(""), CString strSection=CString(""));

	float GetStepX()const;
	float GetStepY()const;

	H3_POINT3D_FLT32 Phase2Position(const float PhaseX,const float PhaseY)const;
	bool ProcessPhaseToMetric(S_3DCoord_onHMapMire& out_Pos,const UWImage& UW);

	H3_POINT2D_FLT32 Get_phi_on_ref()const{return H3_POINT2D_FLT32(m_phiRef_X,  m_phiRef_Y);};

protected:
	float m_fStepX;	//pas du reseau sur l'ecran en mm et en X
	float m_fStepY;	//pas du reseau sur l'ecran en mm et en Y

	float m_phiRef_X,  m_phiRef_Y;//phase affichée sur le pixel pixRef //calculée dans 'LoadSettings2' à partir des éléments chargés

	bool m_isInitialised;
};

#endif // !defined(AFX_MIREHMAP_H__5F1B0FD5_167A_406A_A493_26CFAA1465EE__INCLUDED_)
