/*! 
* 	\file    IntensityProfile.h
* 	\brief   Déclaration de la classe CIntensityProfile
* 	\version 
* 	\author  E.COLON
* 	\date    05/09/2007
* 	\remarks 
*/ 



#if !defined(AFX_INTENSITYPROFILE_H__CEC9AF0A_F9CF_4512_96F4_B6562C35AB5A__INCLUDED_)
#define AFX_INTENSITYPROFILE_H__CEC9AF0A_F9CF_4512_96F4_B6562C35AB5A__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define MAX_NB_PROFILES	3

//////////////////////////////////////////////////////////////////////
///
///	\class   CIntensityProfile 
///	\brief   Classe gérant le tracé de profiles d'intensité
///	\author  E.COLON
///	\date    13/05/2008
///	\bug     
///	\remarks 
///
class CIntensityProfile  
{
public:
	CIntensityProfile();
	virtual ~CIntensityProfile();
	void Init(const unsigned char *pData, long nSizeX, long nSizeY, long nPitch);
	void Init(const unsigned short *pData, long nSizeX, long nSizeY, long nPitch);
	void Init(const float *pData, long nSizeX, long nSizeY, long nPitch);
	void Init(const H3_RGB24 *pData, long nSizeX, long nSizeY, long nPitch);

	void Draw(CDC *pDC,CRect &rcDest);
	float GetAbsMax();
	void SetAbsMax(float fAbsMax);
	void SetView(bool bView);
	bool GetView();
	void SetStyle(long nStyle);
	void SetColor(COLORREF crColor);
	void SetColor(long nId,COLORREF crColor);


private:
	bool m_bView;					///< Drapeau d'affichage du profile
	float m_fPosition;				///< Position relative du profile
//	H3_ARRAY_PT3DFLT32 m_aData;		///< Données du profile

	H3_ARRAY_FLT32	m_aX;
	H3_ARRAY_FLT32	m_aY;
	H3_ARRAY_FLT32	m_aZ[MAX_NB_PROFILES];

	long m_nStyle;					///< Style de tracé (0 par défaut)
	COLORREF m_crColor[MAX_NB_PROFILES+1];		///< couleur du tracé
	bool m_bLocked;					///< Drapeau de blocage
	float m_fAbsMax;				///< Valeur absolue maximum 
	long m_nSizeX;					///< Dimension X de l'image source
	long m_nSizeY;					///< Dimension Y de l'image source
};

#endif // !defined(AFX_INTENSITYPROFILE_H__CEC9AF0A_F9CF_4512_96F4_B6562C35AB5A__INCLUDED_)
