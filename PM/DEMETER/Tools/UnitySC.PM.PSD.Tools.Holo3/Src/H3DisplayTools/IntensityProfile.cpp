///	\file    IntensityProfile.cpp
///	\brief   UImplémentation de la classe CIntensityProfile gérant les 
/// profiles
///	\version 1.0.0.0
///	\author  E.COLON
///	\date    13/05/2008
///	\remarks 
/// 

#include "stdafx.h"
#include "H3DisplayTools.h"
#include "IntensityProfile.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

static CString strModule("CIntensityProfile");

/////////////////////////////////////////////////////////////////////////////
///	\fn      CIntensityProfile()
///	\brief   Constructeur par défaut
/// \see	 
///	\author  E.COLON
///
CIntensityProfile::CIntensityProfile()
{
	m_bView=false;
	m_fPosition=0.5;
	m_bLocked=false;
	SetColor(CR_YELLOW);
	m_nStyle=0;
	m_fAbsMax=255;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      ~CIntensityProfile()
///	\brief   Destructeur
/// \see	 
///	\author  E.COLON
///
CIntensityProfile::~CIntensityProfile()
{

}

void CIntensityProfile::Init(const H3_RGB24 *pData, long nSizeX, long nSizeY, long nPitch)
{
	CString strFunction("Init()");

	if (!m_bView) return;
	if (m_bLocked) return;
	m_bLocked=true;

	if (pData==nullptr || nSizeX<=0 || nSizeY<=0 || nPitch<=0) 
	{
		H3DebugError(strModule,strFunction,"Parametre invalide");
		m_bLocked=false;
		return;
	}

	m_nSizeX=nSizeX;
	m_nSizeY=nSizeY;

	m_aX.ReAlloc(nSizeX);
	m_aY.ReAlloc(nSizeX);
	long i;
	for (i=0;i<3;i++)
	{
		m_aZ[i].ReAlloc(nSizeX);
	}

	SetColor(1,RGB(255,0,0));
	SetColor(2,RGB(0,255,0));
	SetColor(3,RGB(0,0,255));

	long nYPos=(long)(m_nSizeY*m_fPosition);
	long n=nYPos*nPitch;
	for (i=0;i<nSizeX;i++)
	{
		m_aX[i]=(float)i;
		m_aY[i]=(float)nYPos;
		m_aZ[0][i]=(float)pData[n].r;
		m_aZ[1][i]=(float)pData[n].g;
		m_aZ[2][i]=(float)pData[n].b;
		n++;
	}
	m_bLocked=false;
}


/////////////////////////////////////////////////////////////////////////////
///	\fn      void Init(const unsigned char *pData, long nSizeX, long nSizeY, long nPitch)
///	\brief   Initialise le profile à partir de l'image
/// \param   pData pointeur vers les données image de type unsigned char
/// \param   nSizeX dimension utile de l'image selon X
/// \param   nSizeY dimension utile de l'image selon Y
/// \param   nPitch dimension réelle d'une ligne
/// \remarks Le tracé ne se fait pas si le drapeau d'affichage est inactif, ou
/// si il n'y a rien à tracé ou si une opération de calcul du tracé est déja 
/// en cours
/// \see	 Draw()
///	\author  E.COLON
///
void CIntensityProfile::Init(const unsigned char *pData, long nSizeX, long nSizeY, long nPitch)
{
	CString strFunction("Init()");

	if (!m_bView) return;
	if (m_bLocked) return;
	m_bLocked=true;

	if (pData==nullptr || nSizeX<=0 || nSizeY<=0 || nPitch<=0) 
	{
		H3DebugError(strModule,strFunction,"Parametre invalide");
		m_bLocked=false;
		return;
	}

	m_nSizeX=nSizeX;
	m_nSizeY=nSizeY;

	m_aX.ReAlloc(nSizeX);
	m_aY.ReAlloc(nSizeX);
	m_aZ[0].ReAlloc(nSizeX);
	m_aZ[1].Free();
	m_aZ[2].Free();

	SetColor(CR_YELLOW);

	long nYPos=(long)(m_nSizeY*m_fPosition);
	long n=nYPos*nPitch;
	for (long i=0;i<nSizeX;i++)
	{
		m_aX[i]=(float)i;
		m_aY[i]=(float)nYPos;
		m_aZ[0][i]=(float)pData[n++];
	}
	m_bLocked=false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void Init(const unsigned short *pData, long nSizeX, long nSizeY, long nPitch)
///	\brief   Initialise le profile à partir de l'image
/// \param   pData pointeur vers les données image de type unsigned short
/// \param   nSizeX dimension utile de l'image selon X
/// \param   nSizeY dimension utile de l'image selon Y
/// \param   nPitch dimension réelle d'une ligne
/// \remarks Le tracé ne se fait pas si le drapeau d'affichage est inactif, ou
/// si il n'y a rien à tracé ou si une opération de calcul du tracé est déja 
/// en cours
/// \see	 Draw()
///	\author  E.COLON
///
void CIntensityProfile::Init(const unsigned short *pData, long nSizeX, long nSizeY, long nPitch)
{
	CString strFunction("Init()");

	if (!m_bView) return;
	if (m_bLocked) return;
	m_bLocked=true;

	if (pData==nullptr || nSizeX<=0 || nSizeY<=0 || nPitch<=0) 
	{
		H3DebugError(strModule,strFunction,"Parametre invalide");
		m_bLocked=false;
		return;
	}

	m_nSizeX=nSizeX;
	m_nSizeY=nSizeY;

	m_aX.ReAlloc(nSizeX);
	m_aY.ReAlloc(nSizeX);
	m_aZ[0].ReAlloc(nSizeX);
	m_aZ[1].Free();
	m_aZ[2].Free();

	SetColor(CR_YELLOW);

	long nYPos=(long)(m_nSizeY*m_fPosition);
	long n=nYPos*nPitch;
	for (long i=0;i<nSizeX;i++)
	{
		m_aX[i]=(float)i;
		m_aY[i]=(float)nYPos;
		m_aZ[0][i]=(float)pData[n++];
	}
	m_bLocked=false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void Init(const float *pData, long nSizeX, long nSizeY, long nPitch)
///	\brief   Initialise le profile à partir de l'image
/// \param   pData pointeur vers les données image de type float
/// \param   nSizeX dimension utile de l'image selon X
/// \param   nSizeY dimension utile de l'image selon Y
/// \param   nPitch dimension réelle d'une ligne
/// \remarks Le tracé ne se fait pas si le drapeau d'affichage est inactif, ou
/// si il n'y a rien à tracé ou si une opération de calcul du tracé est déja 
/// en cours
/// \see	 Draw()
///	\author  E.COLON
///
void CIntensityProfile::Init(const float *pData, long nSizeX, long nSizeY, long nPitch)
{
	CString strFunction("Init()");

	if (!m_bView) return;
	if (m_bLocked) return;
	m_bLocked=true;

	if (pData==nullptr || nSizeX<=0 || nSizeY<=0 || nPitch<=0) 
	{
		H3DebugError(strModule,strFunction,"Parametre invalide");
		m_bLocked=false;
		return;
	}


	m_nSizeX=nSizeX;
	m_nSizeY=nSizeY;

	m_aX.ReAlloc(nSizeX);
	m_aY.ReAlloc(nSizeX);
	m_aZ[0].ReAlloc(nSizeX);
	m_aZ[1].Free();
	m_aZ[2].Free();

	SetColor(CR_YELLOW);

	long nYPos=(long)(m_nSizeY*m_fPosition);
	long n=nYPos*nPitch;
	for (long i=0;i<nSizeX;i++)
	{
		m_aX[i]=(float)i;
		m_aY[i]=(float)nYPos;
		m_aZ[0][i]=(float)pData[n++];
	}
	m_bLocked=false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void Draw(CDC *pDC, CRect &rcDest)
///	\brief   Tracé du profile
/// \param   pDC descripteur d'affichage
/// \param   rcDest rectangle définissant la position du tracé
/// \remarks Le tracé ne se fait pas si le drapeau d'affichage est inactif, ou
/// si il n'y a rien à tracé ou si une opération de calcul du tracé est déja 
/// en cours
/// \see	 Init()
///	\author  E.COLON
///
void CIntensityProfile::Draw(CDC *pDC, CRect &rcDest)
{
	if (!m_bView) return;
	if (m_aX.IsEmpty()) return;
	if (m_bLocked) return;

	m_bLocked=true;

	CPen pen (PS_SOLID,m_nStyle,m_crColor[0]);
	CPen * pOldPen=pDC->SelectObject(&pen);

	// Marquer la position de la coupe
	{
		long nCutPos=(long)(rcDest.top+rcDest.Height()*m_fPosition);
		pDC->FillSolidRect(rcDest.left,nCutPos-1, 15, 3, m_crColor[0]);
		pDC->FillSolidRect(rcDest.right-15,nCutPos-1, 15, 3, m_crColor[0]);
	}

	// Tracer le profil
	for (long kk=0;kk<MAX_NB_PROFILES;kk++)
	{
		size_t nSize=m_aX.GetSize(); 
		long ypos=rcDest.top+rcDest.Height()/2;
		if (m_aZ[kk].GetSize()==nSize)
		{
//			pen.CreatePen(PS_SOLID,m_nStyle,m_crColor[kk+1]);
			pDC->SelectObject(&pen);

			for (long i=0;i<nSize;i+=4)
			{
	//			H3_POINT2D_FLT32 pt=H3_POINT2D_FLT32(m_aData[i].x,m_aData[i].y);
	//			pt=BufToImage(pt);

				float fx=m_aX[i]/m_nSizeX*rcDest.Width();
				float fy=m_aX[i]/m_nSizeY*rcDest.Height();

				float v=m_aZ[kk][i];
				v=v/(m_fAbsMax*2)*rcDest.Height();

				long yy=(long)(ypos-v);
				yy=__max(yy,rcDest.top);
				yy=__min(yy,rcDest.bottom);

				if (i==0)
					pDC->MoveTo((int)(rcDest.left+fx),yy);	
				else
					pDC->LineTo((int)(rcDest.left+fx),yy);	
			}
		}
	}

	pDC->SelectObject(pOldPen);
	DeleteObject(pen);

	m_bLocked=false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void GetView()
///	\brief   Retourne le drapeau d'affichage
/// \retval	 true le profile est affiché
/// \retval	 false le profile n'est pas affiché
/// \see	 SetView()
///	\author  E.COLON
///
bool CIntensityProfile::GetView()
{
	return m_bView;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void SetView(bool bView)
///	\brief   Fixe le drapeau d'affichage du profile
/// \param   bView affichage du profile si vrai
/// \see	 GetView()
///	\author  E.COLON
///
void CIntensityProfile::SetView(bool bView)
{
	m_bView=bView;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void SetAbsMax(float fAbsMax)
///	\brief   Fixe la valeur maximum du profil
/// \param   fAbsMax valeur maximum du profil au dessus de laquelle le profile
/// affiché est tronqué
/// \see	 GetAbsMax()
///	\author  E.COLON
///
void CIntensityProfile::SetAbsMax(float fAbsMax)
{
	m_fAbsMax=fAbsMax;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      float GetAbsMax()
///	\brief   retourne la valeur maximale du profil
///	\retval	 float Valeur maxi audela de laquelle le profile affiché est tronqué
/// \see	 SetAbsMax()
///	\author  E.COLON
///
float CIntensityProfile::GetAbsMax()
{
	return m_fAbsMax;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void SetColor(COLORREF crColor)
///	\brief   Fixe la couleur du profile
/// \param   crColor couleur RGB(r,v,b)
/// \see	 GetColor()
///	\author  E.COLON
///
void CIntensityProfile::SetColor(COLORREF crColor)
{
	for (long i=0;i<MAX_NB_PROFILES;i++)
		m_crColor[i]=crColor;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void SetColor(COLORREF crColor)
///	\brief   Fixe la couleur du profile
/// \param   crColor couleur RGB(r,v,b)
/// \see	 GetColor()
///	\author  E.COLON
///
void CIntensityProfile::SetColor(long nId,COLORREF crColor)
{
	CString strFunction("SetColor()");
	
	if (nId<0 || nId>MAX_NB_PROFILES)
	{
		H3DisplayError(strModule,strFunction,"Parametre 'nId' invalide");
		return;
	}
	m_crColor[nId]=crColor;
}


/////////////////////////////////////////////////////////////////////////////
///	\fn      SetStyle(long nStyle)
///	\brief   Fixe le style du profile
/// \param   nStyle style 0 valeur par défaut
/// \remarks Le style n'est pas implémenté pour l'instant
/// \see	 GetStyle()
///	\author  E.COLON
///
void CIntensityProfile::SetStyle(long nStyle)
{
	m_nStyle=nStyle;
}
