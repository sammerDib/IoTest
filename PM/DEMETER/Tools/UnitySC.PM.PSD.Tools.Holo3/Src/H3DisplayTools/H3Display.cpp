// H3Display.cpp: implementation of the CH3Display class.
//
//////////////////////////////////////////////////////////////////////
/*! 
* 	\file    : D:\...\H3Display.cpp
* 	\version : 
* 	\brief   : 
* 	\author  : E.COLON,M.FERLET
* 	\date    : XX/XX/XXXX
* 	\remarks : rajout des fonctions :
*					- void DrawYReglePixel(CDC *pDC,CRect &rcRect)
*								par M.FERLET le 03/08/2006
*					- void DrawYReglePixel(CDC *pDC,CRect &rcRect)
*								par M.FERLET le 03/08/2006
*					- void DrawXAxePixel(CDC *pDC,CRect &rcDest)
*								par M.FERLET le 03/08/2006
*					- bool GetbDrawAxePixel()
*								par M.FERLET le 03/08/2006
*					- void SetbDrawAxePixel(bool bDrawAxePixel)
*								par M.FERLET le 03/08/2006
*/ 
#include "stdafx.h"
#include "H3DisplayTools.h"
#include "H3Display.h"
#include "DisplayDlg.h"

#include "dib.h"
#include "IntensityProfile.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

H3_ARRAY_FLT64 Statistics(unsigned short *pData,long nSizeX,long nSizeY,long nPitch);
H3_ARRAY_FLT64 Statistics(unsigned char *pData,long nSizeX,long nSizeY,long nPitch);

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////



#define DEFAULT_SECTION			"H3Display"
#define DEFAULT_TYPE			H3_VGA
#define DEFAULT_DEVICE			0
#define DEFAULT_FORMAT			""
#define DEFAULT_EDGE_TIME		0

#define DEFAULT_COLORBAR_STYLE	1
#define DEFAULT_GRID_STYLE		0
#define	DEFAULT_ROI_STYLE		0
#define	DEFAULT_ROI_COLOR		CR_GREEN

#define	DEFAULT_X_INTENSITY_PROFIL_STYLE	0
#define	DEFAULT_X_INTENSITY_PROFIL_COLOR	CR_YELLOW



static CString strModule("CH3Display");

H3_ARRAY2D_UINT8 VertRamp(long nLi,long nCo)
{
	H3_ARRAY2D_UINT8 Buf(nLi,nCo);

	H3_UINT8 *p=(H3_UINT8 *)Buf.GetData();
	for (long i=0;i<nLi;i++)
	{
		H3_UINT8 v=(H3_UINT8)((long)(( (nLi-1)-i) * 256/nLi));
		for (long j=0;j<nCo;j++)
		{
			p[i*nCo+j]=v;
		}
	}	

	return Buf;
}

void DrawFillRect(CDC *pDC,CRect& rcDest,COLORREF crColor)
{
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	int nOldBkMode=pDC->SetBkMode(TRANSPARENT);

	CBrush Brush;
	Brush.CreateSolidBrush(crColor);
	CBrush *pOldBrush=(CBrush* )pDC->SelectObject(&Brush);
	CPen Pen;
	Pen.CreatePen(PS_NULL,1,crColor);

	CPen *pOldPen=(CPen *)pDC->SelectObject(&Pen);
	pDC->Rectangle(rcDest);
	pDC->SelectObject(pOldPen);

	pDC->SelectObject(pOldBrush);
DeleteObject(Pen);
DeleteObject(Brush);

	pDC->SetBkMode(nOldBkMode);
}

//////////////////////////////////////////////////////////////////////
// Constructeur par defaut
CH3Display::CH3Display()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("CH3Display()");
	CString strMsg=_T("Impossible de créer l'afficheur.");

	InitMembers();
	m_pImageDIB=(void *)new CDib();
	m_pColorBarDIB=(void *)new CDib();

	H3_ARRAY2D_UINT8 VertRampBuf=VertRamp(256,16);
	((CDib *)m_pColorBarDIB)->Set((unsigned char*)VertRampBuf.GetData(),VertRampBuf.GetCo(),VertRampBuf.GetLi(),VertRampBuf.GetCo());

	m_nScaleFactorPowerTen = 0;
	m_Range=H3_ARRAY_FLT32(4);m_Range.Fill(0);

	m_strTitle  = _T("");
	m_strXLabel = _T("");
	m_strYLabel = _T("");
	m_nRangeMode=2;
	m_nStatisticsPP=100;
	m_XPos=m_YPos=0;

	memset(&m_LogFont,0,sizeof(LOGFONT));
	m_LogFont.lfCharSet			= GB2312_CHARSET;
	m_LogFont.lfClipPrecision	= CLIP_DEFAULT_PRECIS;
	m_LogFont.lfEscapement		= 0;
	m_LogFont.lfItalic			= false;
	m_LogFont.lfOrientation		= 0;
	m_LogFont.lfOutPrecision	= OUT_CHARACTER_PRECIS;
	m_LogFont.lfPitchAndFamily	= DEFAULT_PITCH;
	m_LogFont.lfQuality			= DEFAULT_QUALITY;
	m_LogFont.lfStrikeOut		= false;
	m_LogFont.lfUnderline		= false;
	m_LogFont.lfWeight			= FW_NORMAL;
	m_LogFont.lfWidth			= 0;

	SetGridStyle(DEFAULT_GRID_STYLE);
	SetROIStyle(DEFAULT_ROI_STYLE);
	SetROIColor(DEFAULT_ROI_COLOR);
	SetColorBarStyle(DEFAULT_COLORBAR_STYLE);

	m_ColorMap1.ReAlloc(256,3);m_ColorMap2.Fill(0);
	m_ColorMap2.ReAlloc(8,3);m_ColorMap2.Fill(0);

	m_crBkColor=RGB(255,255,255);

	m_pXIntensityProfile=(void *)new CIntensityProfile;

	m_rcRectZoom=CRect(0,0,0,0);

}

//////////////////////////////////////////////////////////////////////
// Destructeur
CH3Display::~CH3Display()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	Free();
}

//////////////////////////////////////////////////////////////////////
// Fonction permettant de liberer les objets membres
void CH3Display::Free()
{
	if (m_pXIntensityProfile) {delete (CIntensityProfile*)m_pXIntensityProfile;m_pXIntensityProfile=nullptr;}
	if (m_pImageDIB) {delete (CDib *)m_pImageDIB;m_pImageDIB=nullptr;}
	if (m_pColorBarDIB) {delete (CDib *)m_pColorBarDIB;m_pColorBarDIB=nullptr;}
	if (m_pDisplayDlg) {delete (CDisplayDlg *)m_pDisplayDlg;m_pDisplayDlg=nullptr;}

	InitMembers();
}

//////////////////////////////////////////////////////////////////////
// Initialisation des membres
void CH3Display::InitMembers()
{
	m_pImageDIB=nullptr;
	m_pColorBarDIB=nullptr;
	m_pDisplayDlg=nullptr;
}

//////////////////////////////////////////////////////////////////////
// Retourne vrai si l'objet est correctement alloue
bool CH3Display::IsAllocated()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (m_pImageDIB)
		return true;
	return false;
}

//////////////////////////////////////////////////////////////////////
// Affichage d'une image couleur de type H3_ARRAY2D_RGB24
void CH3Display::Set(const H3_ARRAY2D_RGB24 &SrcBuf)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("Set()");
	CString strMsg=_T("Impossible d'afficher.");

	if (SrcBuf.IsEmpty())
	{
		return;
	}

	if (!m_pImageDIB || 
		!m_pColorBarDIB)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nm_pImageDIB ou m_pColorBarDIB est NULL."));
		return;
	}
	((CDib *)m_pImageDIB)->Set(SrcBuf);
}



//////////////////////////////////////////////////////////////////////
void CH3Display::Set(H3_RGB24 *pData, long nSizeX, long nSizeY, long nPitch)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("Set(H3_RGB24)");
	CString strMsg("Impossible d'afficher.");

	if (pData==nullptr || nSizeX<=0 || nSizeY<=0) return;

	if (!m_pImageDIB || !m_pColorBarDIB)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nm_pImageDIB ou m_pColorBarDIB est NULL."));
		return;
	}

	// Données de l'image
	((CDib *)m_pImageDIB)->Set(pData,nSizeX,nSizeY,nPitch);


	// Région d'intérêt
	SetROI(H3_RECT_INT32(0,0,nSizeX-1,nSizeY-1));

	// Initialiser le tracé du profil X
	((CIntensityProfile*)m_pXIntensityProfile)->Init(pData,nSizeX,nSizeY,nPitch);
}


//////////////////////////////////////////////////////////////////////
void CH3Display::Set(unsigned char *pData, long nSizeX, long nSizeY, long nPitch)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("Set(unsigned char)");
	CString strMsg=_T("Impossible d'afficher.");

	if (pData==nullptr || nSizeX<=0 || nSizeY<=0 || nPitch<=0) 
	{
		H3DebugError(strModule,strFunction,strMsg+_T("\nParametre invalide"));
		return;
	}

	if (!m_pImageDIB || !m_pColorBarDIB)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nm_pImageDIB ou m_pColorBarDIB est NULL."));
		return;
	}
	CalcRange(pData,nSizeX,nSizeY,nPitch);

	// Données de l'image
	((CDib *)m_pImageDIB)->Set(pData,nSizeX,nSizeY,nPitch,(unsigned char)m_Range[0],(unsigned char)m_Range[1]);

	// Fixer les palettes de l'image et de l'echelle
	((CDib *)m_pImageDIB)->SetColorMaps(m_ColorMap1,m_ColorMap2);
	((CDib *)m_pColorBarDIB)->SetColorMaps(m_ColorMap1,m_ColorMap2);

	// Région d'intérêt
	SetROI(H3_RECT_INT32(0,0,nSizeX-1,nSizeY-1));

	// Initialiser le tracé du profil X
	((CIntensityProfile*)m_pXIntensityProfile)->Init(pData,nSizeX,nSizeY,nPitch);
}



//////////////////////////////////////////////////////////////////////
// Zoom 
void CH3Display::SetRectZoom(CRect rcRect)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_rcRectZoom=rcRect;


}
bool CH3Display::GetRectZoomActif()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if( m_rcRectZoom.Height()==0 &&m_rcRectZoom.Height()==0)
		return false;
	return true;
}

CRect CH3Display::GetRectZoom()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return m_rcRectZoom;
}

//////////////////////////////////////////////////////////////////////
// Affichage d'une image de type H3_ARRAY2D_UINT8
void CH3Display::Set(const H3_ARRAY2D_UINT8 &SrcBuf)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	Set((unsigned char*)SrcBuf.GetData(),SrcBuf.GetCo(),SrcBuf.GetLi(),SrcBuf.GetCo());
}

//////////////////////////////////////////////////////////////////////
void CH3Display::Set(unsigned short *pData, long nSizeX, long nSizeY, long nPitch)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("Set(unsigned short)");
	CString strMsg=_T("Impossible d'afficher.");

	if (pData==nullptr || nSizeX<=0 || nSizeY<=0 || nPitch<=0) 
	{
		H3DebugError(strModule,strFunction,strMsg+_T("\nParametre invalide"));
		return;
	}

	if (!m_pImageDIB || 
		!m_pColorBarDIB)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nm_pImageDIB ou m_pColorBarDIB est NULL."));
		return;
	}
	CalcRange(pData,nSizeX,nSizeY,nPitch);

	// Données de l'image
	((CDib *)m_pImageDIB)->Set(pData,nSizeX,nSizeY,nPitch,(long)m_Range[0],(long)m_Range[1]);

	// Fixer les palettes de l'image et de l'echelle
	((CDib *)m_pImageDIB)->SetColorMaps(m_ColorMap1,m_ColorMap2);
	((CDib *)m_pColorBarDIB)->SetColorMaps(m_ColorMap1,m_ColorMap2);

	// Région d'intérêt
	SetROI(H3_RECT_INT32(0,0,nSizeX-1,nSizeY-1));

	// Initialiser le tracé du profil X
	((CIntensityProfile*)m_pXIntensityProfile)->Init(pData,nSizeX,nSizeY,nPitch);
}

//////////////////////////////////////////////////////////////////////
// Affichage d'une image de type H3_ARRAY2D_UINT16
void CH3Display::Set(const H3_ARRAY2D_UINT16 &SrcBuf)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	Set((unsigned short*)SrcBuf.GetData(),SrcBuf.GetCo(),SrcBuf.GetLi(),SrcBuf.GetCo());
}

//////////////////////////////////////////////////////////////////////
// Affichage d'une image de type H3_ARRAY2D_FLT32 
void CH3Display::Set(const H3_ARRAY2D_FLT32 &SrcBuf)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("Set()");
	CString strMsg=_T("Impossible d'afficher.");

	if (SrcBuf.IsEmpty()) return;

	if (!m_pImageDIB || 
		!m_pColorBarDIB)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nm_pImageDIB ou m_pColorBarDIB est NULL."));
		return;
	}

	CalcRange(SrcBuf);

	((CDib *)m_pImageDIB)->Set(SrcBuf,m_Range);

	((CDib *)m_pImageDIB)->SetColorMaps(m_ColorMap1,m_ColorMap2);
	((CDib *)m_pColorBarDIB)->SetColorMaps(m_ColorMap1,m_ColorMap2);

	SetROI(H3_RECT_INT32(0,0,SrcBuf.GetCo()-1,SrcBuf.GetLi()-1));
}

#define MIN_COLORBAR_WIDTH	3
#define MAX_COLORBAR_WIDTH	(rc1.Height()/10)
#define MAX_HEIGHT_FONT		18
#define MAX_GRADUATION		12


//////////////////////////////////////////////////////////////////////
// Tracé de la barre de couleur dans le DC spécifié et à l'emplacement
// défini par le rectangle rcDest
void CH3Display::DrawColorBar(CDC *pDC,CRect &rcDest)
{
	//////////////////////////////////////////////////////////////////
	// Verifications de principe car on ne sait jamais...
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	CRect rc1,rc2;
	if (!m_strScaleLabel.IsEmpty())
	{
		rc2=CRect(rcDest.TopLeft(), CSize(rcDest.Width(),rcDest.Height()/15) );
		rc1=CRect(CPoint(rc2.left,rc2.bottom), rcDest.Size()-CSize(0,rc2.Height()));
	}
	else
	{
		rc1=rcDest;
	}

	//////////////////////////////////////////////////////////////////
	// Formatage de la valeur
	CString strResultat,strFmtTextScale;  
	double d=fabs(m_Range[1]);
	if (fabs(m_Range[1])<fabs(m_Range[0]))
		d=fabs(m_Range[0]);

	strFmtTextScale="%5.2g";
	int nPuissanceDeDix=0;
	int a;
	if ( (fabs(d)>=.1)&&(fabs(d)<=1000) )
	{
		SetScaleFactorPowerTen(0);
		strFmtTextScale="%.3f";
		a=61;
	}
	else
	{
		for (a=-60;a<61;a=a+3)
		{

			if ( (fabs(d)>=(1*PowerTen(a)))&&(fabs(d)<(1001*PowerTen(a)))	) 
			{
				SetScaleFactorPowerTen(-a);
				strFmtTextScale="%.3f";
				nPuissanceDeDix=a;
			}
		}
	}

	int nMaxHeightFont = MAX_HEIGHT_FONT;
	if (pDC->IsPrinting( ))
	{
		nMaxHeightFont = 4096;
	}

	//////////////////////////////////////////////////////////////////
	// Tracer la barre de couleur
	long nColorBarWidth=(long)(rc1.Width()*0.25);
	if (nColorBarWidth>MAX_COLORBAR_WIDTH) nColorBarWidth=MAX_COLORBAR_WIDTH;
	if (nColorBarWidth<MIN_COLORBAR_WIDTH) nColorBarWidth=MIN_COLORBAR_WIDTH;
	long nSeparatorWidth=(long)(rc1.Width()*0.05);
	long nTextWidth=rc1.Width()-nColorBarWidth-nSeparatorWidth;

	//////////////////////////////////////////////////////////////////
	// Preparer la police pour l'ecriture du texte
	LOGFONT LogFont;
	memcpy(&LogFont,&m_LogFont,sizeof(LOGFONT));
	LogFont.lfHeight=nTextWidth/4;
	if (LogFont.lfHeight>nMaxHeightFont)
		LogFont.lfHeight=nMaxHeightFont;

	CFont Font;
	Font.CreateFontIndirect(&LogFont);

	CRect rcColorBar(
		CPoint(
			rc1.TopLeft()),
		CSize(
			nColorBarWidth,
			rc1.Height()));


	//////////////////////////////////////////////////////////////////
	// Determiner le nombre et le pas des graduations
	long nNumber=(rcColorBar.bottom - rcColorBar.top)/(LogFont.lfHeight);
	if(nNumber>MAX_GRADUATION)
		nNumber=MAX_GRADUATION;
	if (nPuissanceDeDix!=0)
		nNumber--;

	float fGraduationStep=(float)(rcColorBar.bottom - rcColorBar.top)/nNumber;

	DrawFillRect(pDC, CRect(rcColorBar.left,rcColorBar.top-1,rc1.right,rc1.bottom+1),m_crBkColor);

	if (!m_strScaleLabel.IsEmpty())
	{
		DrawFillRect(pDC, rc2,m_crBkColor);
		DrawText(pDC,m_strScaleLabel,rc2,DT_LEFT|DT_SINGLELINE,Font);
	}


	rcColorBar = CRect(
		CPoint(
			rc1.TopLeft()),
		CSize(
			nColorBarWidth,
			rc1.Height()));
	DrawDIB(pDC,(void *)m_pColorBarDIB,rcColorBar);
	
	//////////////////////////////////////////////////////////////////
	// Determiner la longueur de chaque trait
	long nGridLen=(long)(rcColorBar.Width()*0.2);	

	for (long i=nNumber;i>=0;i--)
	{
		if ( (i != 0) || (nPuissanceDeDix==0))
		{
			long y=(long)(fGraduationStep*i+rcColorBar.top+0.5);
			if(nPuissanceDeDix!=0)
				y-=(long)(fGraduationStep/2);

			// Tracer les graduations
			pDC->MoveTo(rcColorBar.left,y);	
			pDC->LineTo(rcColorBar.left+nGridLen,y);
			pDC->MoveTo(rcColorBar.right,y);	
			pDC->LineTo(rcColorBar.right-nGridLen,y);

			// Formatage de la valeur
			CString strResultat,strFmtTextScale;
			double d=fabs(m_Range[1]);
			if (fabs(m_Range[1])<fabs(m_Range[0]))
				d=fabs(m_Range[0]);

			strFmtTextScale="%5.2g";
			if ( (fabs(d)>=.1)&&(fabs(d)<=1000) )
			{
				SetScaleFactorPowerTen(0);
				strFmtTextScale="%.3f";
				a=61;
			}
			else
			{
				for (int a=-60;a<61;a=a+3)
				{

					if ( (fabs(d)>=(PowerTen(a)-1))&&(fabs(d)<(1000*PowerTen(a)))) 
					{
						SetScaleFactorPowerTen(-a);
						strFmtTextScale="%.3f";
						a=61;
					}
				}
			}

			float nResultat = (nNumber-i)*((m_Range[1]-m_Range[0])/nNumber)+ m_Range[0]; 
			if (nPuissanceDeDix!=0)
			{
				double Offset,Pas;
				Offset = m_Range[0]+((m_Range[1]-m_Range[0])/(nNumber+1))/2;
				Pas = ((m_Range[1]-m_Range[0]-2*(Offset-m_Range[0]))/(nNumber-1)); 
				nResultat = (float) ((nNumber-i) * Pas + Offset);
			}
			strResultat.Format(strFmtTextScale,nResultat * PowerTen(m_nScaleFactorPowerTen));
			
			CRect rcText(
				CPoint(rcColorBar.right+nSeparatorWidth,(int)(y-fGraduationStep/2)),
				CSize(nTextWidth,(int)fGraduationStep));

			DrawText(
				pDC,
				strResultat,
				rcText,
				DT_LEFT|DT_SINGLELINE|DT_VCENTER,
				Font);
		}
		else
		{
			if (i == 0)
			{
				CString strResultat=_T("x10");
				long y=(long)(fGraduationStep*i+rcColorBar.top+0.5);

				CRect rcText(
					CPoint( 
						(int)(rcColorBar.right+nSeparatorWidth),
						(int)(y-fGraduationStep/2)),
					CSize( 
						LogFont.lfHeight*2,
						(int)fGraduationStep ) );

				DrawText(
					pDC,
					strResultat,
					rcText,
					DT_RIGHT|DT_SINGLELINE|DT_VCENTER,
					Font);

				int temp=(int)(LogFont.lfHeight+LogFont.lfHeight*1.2);

				LogFont.lfHeight=nTextWidth/4*70/100;
				if (LogFont.lfHeight>nMaxHeightFont*70/100)
					LogFont.lfHeight=nMaxHeightFont*70/100;
#ifndef _DEBUG
				Font.CreateFontIndirect(&LogFont);
#else
TRACE("2éme Création des font, invalise");
#endif

				CRect rcTextScaleFactorPowerTen( 
					CPoint(
						rcText.right,
						(int)(y-fGraduationStep*3/4)), 
					CSize( 
						nColorBarWidth/2 ,
						(int)fGraduationStep ) );
				CString strScaleFactor;
				strScaleFactor.Format("%d",m_nScaleFactorPowerTen*-1);

				DrawText(pDC,
						strScaleFactor,
						rcTextScaleFactorPowerTen,
						DT_LEFT|DT_SINGLELINE|DT_VCENTER,
						Font);

			}
		}
	}
}

//////////////////////////////////////////////////////////////////////
// Tracé du titre dans le DC spécifié et à l'emplacement défini par 
// le rectangle rcDest
void CH3Display::DrawTitle(CDC *pDC,CRect &rcDest)
{
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	LOGFONT LogFont;

	memcpy(&LogFont,&m_LogFont,sizeof(LOGFONT));

	float cxInch = (float)pDC->GetDeviceCaps(LOGPIXELSX);
	float cyInch = (float)pDC->GetDeviceCaps(LOGPIXELSY);

	long nLen=m_strTitle.GetLength();
	if (nLen!=0)
	{
		LogFont.lfHeight=-MulDiv(rcDest.Height(), (int)cyInch, 72);

		if ((abs(LogFont.lfHeight)*cxInch/cyInch*nLen)>rcDest.Width())
		{
			float Ratio=(float)(abs(LogFont.lfHeight)*cxInch/cyInch*nLen)/(float)rcDest.Width();
			LogFont.lfHeight=(long)(LogFont.lfHeight/Ratio);
		}
	}

	CFont Font;
	Font.CreateFontIndirect(&LogFont);

	DrawText(
		pDC,
		m_strTitle,
		rcDest,
		DT_CENTER|DT_SINGLELINE|DT_VCENTER,
		Font);

	m_rcTitle=rcDest;
}

//////////////////////////////////////////////////////////////////////
// Tracé du label X dans le DC spécifié et à l'emplacement défini par 
// le rectangle rcDest
void CH3Display::DrawXLabel(CDC *pDC,CRect &rcDest)
{
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	LOGFONT LogFont;

	memcpy(&LogFont,&m_LogFont,sizeof(LOGFONT));
	LogFont.lfHeight = rcDest.Height()-2;
	if(LogFont.lfHeight>16)
		LogFont.lfHeight=16;
	else if (LogFont.lfHeight<5)
		LogFont.lfHeight=8000;

	CFont Font;
	Font.CreateFontIndirect(&LogFont);

	DrawText(
		pDC,
		m_strXLabel,
		rcDest,
		DT_CENTER|DT_SINGLELINE|DT_VCENTER,
		Font);
}

//////////////////////////////////////////////////////////////////////
// Tracé du texte str dans le DC spécifié et à l'emplacement défini par 
// le rectangle rcDest. Pour les parametres nFormat et Font voir la doc
// microsoft je pense qu'elle est suffisement claire à ce sujet.
void CH3Display::DrawText(CDC *pDC,CString& str,CRect& rcDest,UINT nFormat,CFont& Font)
{
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	int nOldBkMode=pDC->SetBkMode(TRANSPARENT);

	CFont *pOldFont = (CFont *) pDC->SelectObject(&Font);
	pDC->DrawText(str,rcDest,nFormat);
	pDC->SelectObject(pOldFont);
DeleteObject(Font);
	pDC->SetBkMode(nOldBkMode);
}

//////////////////////////////////////////////////////////////////////
// Tracé du label Y dans le DC spécifié et à l'emplacement défini par 
// le rectangle rcDest
void CH3Display::DrawYLabel(CDC *pDC,CRect &rcDest)
{
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	LOGFONT LogFont;

	memcpy(&LogFont,&m_LogFont,sizeof(LOGFONT));
	LogFont.lfEscapement= 900;

	CFont Font;
	Font.CreateFontIndirect(&LogFont);

	DrawText(
		pDC,
		m_strYLabel,
		rcDest,
		DT_CENTER|DT_SINGLELINE|DT_VCENTER,
		Font);
}

//////////////////////////////////////////////////////////////////////
// Tracé d'un rectangle dans le DC spécifié et à l'emplacement défini 
// par le rectangle rcDest
void CH3Display::DrawRectangle(CDC *pDC,CRect &rcRect)
{
	if (!pDC) return;
	if (rcRect.IsRectEmpty()) return;

	pDC->MoveTo(rcRect.left,rcRect.top);
	pDC->LineTo(rcRect.right,rcRect.top);
	pDC->LineTo(rcRect.right,rcRect.bottom);
	pDC->LineTo(rcRect.left,rcRect.bottom);
	pDC->LineTo(rcRect.left,rcRect.top);
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetColorBarStyle(int nStyle)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_nColorBarStyle = nStyle;
}

//////////////////////////////////////////////////////////////////////
long CH3Display::GetColorBarStyle()
{
	return m_nColorBarStyle;
}

//////////////////////////////////////////////////////////////////////
// Cette fonction fixe le texte du titre
void CH3Display::SetTitle(const CString &strTitle)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_strTitle=strTitle;
}

//////////////////////////////////////////////////////////////////////
// Cette fonction fixe le texte du label X 
void CH3Display::SetXLabel(const CString &strXLabel)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_strXLabel=strXLabel;
}

//////////////////////////////////////////////////////////////////////
// Cette fonction fixe le texte du label Y 
void CH3Display::SetYLabel(const CString &strYLabel)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_strYLabel=strYLabel;
}

//////////////////////////////////////////////////////////////////////
// Cette fonction permet de forcer l'etendue d'affichage entre les
// 2 bornes spécifiées dans le tableau Range. Cette fonction force
// le mode d'affichage à 0 (fixe, fonction SetRangeMode()).
// Range[0] : borne min
// Range[1] : borne max
void CH3Display::SetRange(H3_ARRAY_FLT32 & Range)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("SetRange()");
	CString strMsg("Impossible de fixer l'étendue.");

	if (Range.GetSize()!=2)
	{
		H3DisplayError(strModule,strFunction,strMsg+_T("\nParamètre Range invalide."));
		return;
	}

	SetRangeMode(0);
	m_Range[2]=min(Range[0],Range[1]);
	m_Range[3]=max(Range[0],Range[1]);

	if (m_Range[3]==m_Range[2])
	{
		m_Range[3]=m_Range[3]+1;
		m_Range[2]=m_Range[2]-1;
	}

	m_Range[0]=m_Range[2];
	m_Range[1]=m_Range[3];
}

//////////////////////////////////////////////////////////////////////
// Cette fonction permet de forcer l'etendue d'affichage entre les
// bornes Low et High
void CH3Display::SetRange(double Low,double High)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	H3_ARRAY_FLT32 Range(2);
	Range[0]=(float)Low;
	Range[1]=(float)High;
	SetRange(Range);
}

//////////////////////////////////////////////////////////////////////
// Permet de fixer le mode d'affichage de l'etendue
// 0 : etendue fixe regler par la fct SetRange()
// 1 : etendue Min/Max
// 2 : etendue +/-1 EcartType
// 3 : etendue +/-2 EcartType
// 4 : etendue +/-3 EcartType
// 5 : etendue +/-1 EcartType
// 6 : etendue +/-2 EcartType
// 7 : etendue +/-3 EcartType
void CH3Display::SetRangeMode(long nMode)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_nRangeMode=nMode; 
}

//////////////////////////////////////////////////////////////////////
bool CH3Display::SetColorMap1(H3_ARRAY2D_UINT8 &ColorMap)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (ColorMap.GetLi()!=256 || ColorMap.GetCo()!=3)
	{
		m_ColorMap1=H3_ARRAY2D_UINT8(256,3);

		for (long i=0;i<256;i++) 
		{
			m_ColorMap1(i,0)=(BYTE)i;
			m_ColorMap1(i,1)=(BYTE)i;
			m_ColorMap1(i,2)=(BYTE)i;
		}
	}
	else
	{
		m_ColorMap1=ColorMap;
	}

	if (m_pImageDIB)
	{
		CDib *pDIB=(CDib *)m_pImageDIB;
		pDIB->SetColorMaps(m_ColorMap1,m_ColorMap2);
	}

	if (m_pColorBarDIB)
	{
		CDib *pDIB=(CDib *)m_pColorBarDIB;
		pDIB->SetColorMaps(m_ColorMap1,m_ColorMap2);
	}
	return true;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetGridStyle(int nStyle)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_nGridStyle=nStyle;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::DrawGrid(CDC *pDC,CRect rcImage)
{
	long hauteur=rcImage.bottom-rcImage.top;
	long largeur=rcImage.left-rcImage.right;

	switch(m_nGridStyle)
	{
		case 1:
			if ((hauteur>=10) && (largeur >=10))
			{
				float pashauteur=((float) hauteur+1)/((float) 10);
				float paslargeur=((float)largeur+1)/((float) 10);
				float tirethauteur=hauteur/50.0F;
				float tiretlargeur=largeur/50.0F;
				if (tiretlargeur>tirethauteur)
					tiretlargeur=tirethauteur;
				else
					tirethauteur=tiretlargeur;
				int x,y;

				//Grid horizontal
				long i;
				for (i=1 ; i<10 ; i++)
				{
					x=rcImage.right;
					y=rcImage.top;

					pDC->MoveTo((int)(x + i * paslargeur),y);	
					pDC->LineTo((int)(x + i * paslargeur),(int)(y+tirethauteur));

					y=rcImage.bottom;

					pDC->MoveTo((int)(x + i * paslargeur),y);	
					pDC->LineTo((int)(x + i * paslargeur),(int)(y-tirethauteur));
				}

				//Grid verticale
				for ( i=1 ; i<10 ; i++)
				{
					x=rcImage.right;
					y=rcImage.top;

					pDC->MoveTo(x ,(int)(y + i * pashauteur));	
					pDC->LineTo((int)(x + tiretlargeur),(int)(y + i * pashauteur));

					x=rcImage.left;

					pDC->MoveTo((int) x ,(int)(y + i * pashauteur));	
					pDC->LineTo((int)(x - tiretlargeur),(int)(y + i * pashauteur));
				}
			}
			break;

		case 2:
			break;
		default:
			break;
	}
}

//////////////////////////////////////////////////////////////////////
void CH3Display::DrawDIB(CDC *pDC, void *pDIB, CRect &rcDest,bool bTrue)
{
	if (!pDIB) return;

	DrawRectangle(pDC,rcDest);

	/////////////////////////////////////////////////////////////////////////////
	// EC 07/03/2002 etant donne le cadre, le point BottomRight devrait être 
	// decale d'un pixel à gauche et d'un pixel à droite mais ce n'est pas
	// necessaire. Il faudrait verifier pourquoi

	CRect rcImage(
		CPoint(rcDest.TopLeft()+CPoint(1,1)),
		CPoint(rcDest.BottomRight()) );

	((CDib*)pDIB)->Paint(*pDC,&rcImage,bTrue,m_rcRectZoom);
}

//////////////////////////////////////////////////////////////////////
void CH3Display::DrawImage(CDC *pDC, CRect &rcDest)
{
	DrawDIB(pDC,m_pImageDIB,rcDest,true);

	//////////////////////////////////////////////////////////////////
	// EC 07/03/2002 etant donne le cadre, le point BottomRight devrait
	// être decale d'un pixel à gauche et d'un pixel à droite mais ce 
	// n'est pas necessaire. Il faudrait verifier pourquoi
	CRect rcImage(
		CPoint(rcDest.TopLeft()+CPoint(1,1)),
		CPoint(rcDest.BottomRight()) );
	m_rcImage=rcImage;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::DisplayWnd(void *pmyDIB)
{
}

//////////////////////////////////////////////////////////////////////
void CH3Display::CalcRange(unsigned char *pData,long nSizeX,long nSizeY,long nPitch)
{
	switch(m_nRangeMode)
	{
	case 0:		// Fixe
		break;

	case 1:		// Min/Max
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)Stats[1];m_Range[1]=(float)Stats[2];
			m_Range[2]=(float)Stats[1];m_Range[3]=(float)Stats[2];
		}
		break;

	case 2:		// +/- 1 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)Stats[5]-(float)Stats[6];m_Range[1]=(float)Stats[5]+(float)Stats[6];
			m_Range[2]=(float)Stats[5]-(float)Stats[6];m_Range[3]=(float)Stats[5]+(float)Stats[6];
		}
		break;

	case 3:		// +/- 2 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-2*Stats[6]);m_Range[1]=(float)(Stats[5]+2*Stats[6]);
			m_Range[2]=(float)(Stats[5]-2*Stats[6]);m_Range[3]=(float)(Stats[5]+2*Stats[6]);
		}
		break;

	case 4:		// +/- 3 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-3*Stats[6]);m_Range[1]=(float)(Stats[5]+3*Stats[6]);
			m_Range[2]=(float)(Stats[5]-3*Stats[6]);m_Range[3]=(float)(Stats[5]+3*Stats[6]);
		}
		break;

	case 5:		// +/- 1 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-Stats[6]);m_Range[1]=(float)(Stats[5]+Stats[6]);
		}
		break;

	case 6:		// +/- 2 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-2*Stats[6]);m_Range[1]=(float)(Stats[5]+2*Stats[6]);
		}
		break;

	case 7:		// +/- 3 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-3*Stats[6]);m_Range[1]=(float)(Stats[5]+3*Stats[6]);
		}
		break;

	default:
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)Stats[1];m_Range[1]=(float)Stats[2];
			m_Range[2]=(float)Stats[1];m_Range[3]=(float)Stats[2];
		}
		break;
	}

	m_Range[0]=max(m_Range[0],0);
	m_Range[1]=min(m_Range[1],UCHAR_MAX);

	if (m_Range[0]==m_Range[1])
	{
		m_Range[0]=m_Range[0]-1;
		m_Range[1]=m_Range[1]+1;
	}


	m_Range[2]=max(m_Range[2],0);
	m_Range[3]=min(m_Range[3],UCHAR_MAX);

	if (m_Range[2]==m_Range[3])
	{
		m_Range[2]=m_Range[2]-1;
		m_Range[3]=m_Range[3]+1;
	}
}

//////////////////////////////////////////////////////////////////////
H3_ARRAY_FLT64 Statistics(unsigned short *pData,long nSizeX,long nSizeY,long nPitch)
{
	long N=0;
	double SumXi=0;
	double SumXi2=0;
	double Max=-DBL_MAX;
	double Min=DBL_MAX;
	double Mean;
	double Std;

	unsigned short *p=pData;
	for (long i=0;i<nSizeY;i++)
	{
		unsigned short *p1=p;
		for (long j=0;j<nSizeX;j++)
		{

			double Xi=*p1++;
			N++;
			SumXi+=Xi;
			SumXi2+=Xi*Xi;
			Max=__max(Max,Xi);
			Min=__min(Min,Xi);
		}
		p+=nPitch;
	}

	if (N>0)
	{
		Mean=SumXi/((double)N);
		Std=sqrt( (SumXi2/(double)N) -Mean*Mean);
	}
	else
	{
		Min=0;
		Max=0;
		SumXi=0;
		SumXi2=0;
		Mean=0;
		Std=0;
	}

	H3_ARRAY_FLT64 Res(7);
	Res[0]=N;
	Res[1]=Min;
	Res[2]=Max;
	Res[3]=SumXi;
	Res[4]=SumXi2;
	Res[5]=Mean;
	Res[6]=Std;

	return Res;
}

//////////////////////////////////////////////////////////////////////
H3_ARRAY_FLT64 Statistics(unsigned char *pData,long nSizeX,long nSizeY,long nPitch)
{
	long N=0;
	double SumXi=0;
	double SumXi2=0;
	double Max=-DBL_MAX;
	double Min=DBL_MAX;
	double Mean;
	double Std; 

	unsigned char *p=pData;
	for (long i=0;i<nSizeY;i++)
	{
		unsigned char *p1=p;
		for (long j=0;j<nSizeX;j++)
		{

			double Xi=*p1++;
			N++;
			SumXi+=Xi;
			SumXi2+=Xi*Xi;
			Max=__max(Max,Xi);
			Min=__min(Min,Xi);
		}
		p+=nPitch;
	}

	if (N>0)
	{
		Mean=SumXi/((double)N);
		Std=sqrt( (SumXi2/(double)N) -Mean*Mean);
	}
	else
	{
		Min=0;
		Max=0;
		SumXi=0;
		SumXi2=0;
		Mean=0;
		Std=0;
	}

	H3_ARRAY_FLT64 Res(7);
	Res[0]=N;
	Res[1]=Min;
	Res[2]=Max;
	Res[3]=SumXi;
	Res[4]=SumXi2;
	Res[5]=Mean;
	Res[6]=Std;

	return Res;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::CalcRange(unsigned short *pData,long nSizeX,long nSizeY,long nPitch)
{
	switch(m_nRangeMode)
	{
	case 0:		// Fixe
		break;

	case 1:		// Min/Max
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)Stats[1];m_Range[1]=(float)Stats[2];
			m_Range[2]=(float)Stats[1];m_Range[3]=(float)Stats[2];
		}
		break;

	case 2:		// +/- 1 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-Stats[6]);m_Range[1]=(float)(Stats[5]+Stats[6]);
			m_Range[2]=(float)(Stats[5]-Stats[6]);m_Range[3]=(float)(Stats[5]+Stats[6]);
		}
		break;

	case 3:		// +/- 2 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-2*Stats[6]);m_Range[1]=(float)(Stats[5]+2*Stats[6]);
			m_Range[2]=(float)(Stats[5]-2*Stats[6]);m_Range[3]=(float)(Stats[5]+2*Stats[6]);
		}
		break;

	case 4:		// +/- 3 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-3*Stats[6]);m_Range[1]=(float)(Stats[5]+3*Stats[6]);
			m_Range[2]=(float)(Stats[5]-3*Stats[6]);m_Range[3]=(float)(Stats[5]+3*Stats[6]);
		}
		break;

	case 5:		// +/- 1 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-Stats[6]);m_Range[1]=(float)(Stats[5]+Stats[6]);
		}
		break;

	case 6:		// +/- 2 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-2*Stats[6]);m_Range[1]=(float)(Stats[5]+2*Stats[6]);
		}
		break;

	case 7:		// +/- 3 Sigma
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)(Stats[5]-3*Stats[6]);m_Range[1]=(float)(Stats[5]+3*Stats[6]);
		}
		break;

	default:
		{
			H3_ARRAY_FLT64 Stats=Statistics(pData,nSizeX,nSizeY,nPitch);
			m_Range[0]=(float)Stats[1];m_Range[1]=(float)Stats[2];
			m_Range[2]=(float)Stats[1];m_Range[3]=(float)Stats[2];
		}
		break;
	}

	m_Range[0]=max(m_Range[0],0);
	m_Range[1]=min(m_Range[1],USHRT_MAX);

	if (m_Range[0]==m_Range[1])
	{
		m_Range[0]=m_Range[0]-1;
		m_Range[1]=m_Range[1]+1;
	}

	m_Range[2]=max(m_Range[2],0);
	m_Range[3]=min(m_Range[3],USHRT_MAX);

	if (m_Range[2]==m_Range[3])
	{
		m_Range[2]=m_Range[2]-1;
		m_Range[3]=m_Range[3]+1;
	}
}

//////////////////////////////////////////////////////////////////////
void CH3Display::CalcRange(const H3_ARRAY2D_FLT32 &SrcBuf)
{
	switch(m_nRangeMode)
	{
	case 0:		// Manuel
		break;

	case 1:		// Min/Max
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[1];m_Range[1]=Stats[2];
			m_Range[2]=Stats[1];m_Range[3]=Stats[2];

		}
		break;

	case 2:		// +/- 1 Sigma
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[5]-Stats[6];m_Range[1]=Stats[5]+Stats[6];
			m_Range[2]=Stats[5]-Stats[6];m_Range[3]=Stats[5]+Stats[6];
		}
		break;

	case 3:		// +/- 2 Sigma
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[5]-2*Stats[6];m_Range[1]=Stats[5]+2*Stats[6];
			m_Range[2]=Stats[5]-2*Stats[6];m_Range[3]=Stats[5]+2*Stats[6];
		}
		break;

	case 4:		// +/- 3 Sigma
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[5]-3*Stats[6];m_Range[1]=Stats[5]+3*Stats[6];
			m_Range[2]=Stats[5]-3*Stats[6];m_Range[3]=Stats[5]+3*Stats[6];
		}
		break;

	case 5:		// +/- 1 Sigma
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[5]-Stats[6];m_Range[1]=Stats[5]+Stats[6];
			m_Range[2]=Stats[5]-Stats[6];m_Range[3]=Stats[5]+Stats[6];
		}
		break;

	case 6:		// +/- 2 Sigma
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[5]-2*Stats[6];m_Range[1]=Stats[5]+2*Stats[6];
		}
		break;

	case 7:		// +/- 3 Sigma
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[5]-3*Stats[6];m_Range[1]=Stats[5]+3*Stats[6];
		}
		break;

	default:
		{
			H3_ARRAY_FLT32 Stats=SrcBuf.GetStatistics(m_nStatisticsPP);
			m_Range[0]=Stats[1];m_Range[1]=Stats[2];
		}
		break;
	}

	m_Range[0]=max(m_Range[0],-FLT_MAX);
	m_Range[1]=min(m_Range[1],FLT_MAX);

	if (m_Range[0]==m_Range[1])
	{
		m_Range[0]=m_Range[0]-1;
		m_Range[1]=m_Range[1]+1;
	}

	m_Range[2]=max(m_Range[2],-FLT_MAX);
	m_Range[3]=min(m_Range[3],FLT_MAX);

	if (m_Range[2]==m_Range[3])
	{
		m_Range[2]=m_Range[2]-1;
		m_Range[3]=m_Range[3]+1;
	}
}

//////////////////////////////////////////////////////////////////////
CRect CH3Display::GetImageRect()
{
	return m_rcImage;
}

//////////////////////////////////////////////////////////////////////
H3_POINT2D_FLT32 CH3Display::ImageToBuf(H3_POINT2D_FLT32 &ptDIB)
{
	H3_POINT2D_FLT32 ptBuf;
	if (m_pImageDIB)
	{
		if(m_rcRectZoom.Height()==0 && m_rcRectZoom.Width()==0)
		{
			float nDIBSizeX=(float)(((CDib*)m_pImageDIB)->Width());
			float nDIBSizeY=(float)(((CDib*)m_pImageDIB)->Height());

			float nImageSizeX=(float)m_rcImage.Width();
			float nImageSizeY=(float)m_rcImage.Height();


			H3_POINT2D_FLT32 Scale(
				(nDIBSizeX)/(nImageSizeX),
				(nDIBSizeY)/(nImageSizeY));

			ptBuf=ptDIB*Scale;
		}
		else
		{

			float nDIBSizeX=(float)(((CDib*)m_pImageDIB)->Width());
			float nDIBSizeY=(float)(((CDib*)m_pImageDIB)->Height());

			float nImageSizeX=(float)m_rcImage.Width();
			float nImageSizeY=(float)m_rcImage.Height()
				;
			if(ptDIB.x<nImageSizeX/4 && ptDIB.y<nImageSizeY/4 )
			{

				H3_POINT2D_FLT32 Scale(
					(nDIBSizeX)/(nImageSizeX),
					(nDIBSizeY)/(nImageSizeY));

				ptBuf=ptDIB*Scale*4;			
			}
			else
			{
					
				float nZoomSizeX=(float)m_rcRectZoom.Width();
				float nZoomSizeY=(float)m_rcRectZoom.Height();

				H3_POINT2D_FLT32 Scale(
					(nZoomSizeX)/(nImageSizeX ),
					(nZoomSizeY)/(nImageSizeY ));

				ptBuf=ptDIB*Scale;

				ptBuf.x+=m_rcRectZoom.left;
				ptBuf.y=(ptBuf.y+(nDIBSizeY-m_rcRectZoom.bottom));
		 
			}
		}
	}

	return ptBuf;
}

//////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////
#define _RESCALE_ARRAY_INT32(A,S)		\
{										\
	size_t n=A.GetSize();					\
	H3_INT32 *pA=A.GetData();			\
	while (n)							\
	{									\
		float f=(float)(*pA);			\
		f=f / S + 0.5F;					\
		*pA=(H3_INT32)f;				\
		n--;pA++;						\
	}									\
}

//////////////////////////////////////////////////////////////////////
bool CH3Display::ConvBufToImage(H3_ARRAY_INT32 &X, H3_ARRAY_INT32 &Y)
{
	if (!m_pImageDIB) return false;

	float nDIBSizeX=(float)((CDib*)m_pImageDIB)->Width();
	float nDIBSizeY=(float)((CDib*)m_pImageDIB)->Height();

	float nImageSizeX=(float)m_rcImage.Width();
	float nImageSizeY=(float)m_rcImage.Height();

	float ScaleX=(nDIBSizeX)/(nImageSizeX);
	float ScaleY=(nDIBSizeY)/(nImageSizeY);

	_RESCALE_ARRAY_INT32(X,ScaleX);
	_RESCALE_ARRAY_INT32(Y,ScaleY);

	return true;
}


//////////////////////////////////////////////////////////////////////
// Reaffiche l'image courante dans la zone client de la fenetre pWnd
void CH3Display::Draw(CWnd *pWnd)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (!pWnd) return;

	CDC *pDC=pWnd->GetDC();

	CRect rcDest;
	pWnd->GetClientRect(rcDest);
	Draw(pDC,rcDest);

	pWnd->ReleaseDC(pDC);
}

//////////////////////////////////////////////////////////////////////
// Reaffichage l'image courante dans la fenetre cliente
void CH3Display::Draw()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CDisplayDlg *pDisplayDlg=(CDisplayDlg *)m_pDisplayDlg;
	// Si il n'y a pas de fenetre cliente alors en creer une ...
	if (!pDisplayDlg)
	{
		pDisplayDlg=new CDisplayDlg(this);
		m_pDisplayDlg=(void *)pDisplayDlg;

		pDisplayDlg->Create(IDD_DISPLAY);
		pDisplayDlg->ShowWindow(SW_SHOW);
	}

	Draw(&(pDisplayDlg->m_CImage));
}

//////////////////////////////////////////////////////////////////////
void CH3Display::Draw(CDC *pDC,CRect rcDest)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	// Verification parametres
	if (!pDC) return;
	if (rcDest.IsRectEmpty()) return;

	// Initialisations
	CDib *pDIB=(CDib *)m_pImageDIB;
	if (!pDIB->IsValid()) return;
	int cxDIB = pDIB->Width();     
	int cyDIB = pDIB->Height(); 

	// Calculer taille de l'image
	CSize ImageSize(rcDest.Width(),rcDest.Width()*cyDIB/cxDIB);
	if (ImageSize.cy>rcDest.Height())
	{
		ImageSize.cy=rcDest.Height();
		ImageSize.cx=ImageSize.cy*cxDIB/cyDIB;
	}

	// Bordure pour le titre
	long nTopBorder=0;
	{
		nTopBorder=(long)(rcDest.Height()*0.05);
		if (ImageSize.cy>(rcDest.Height()-nTopBorder))
		{
			ImageSize.cy-=nTopBorder;
			ImageSize.cx=ImageSize.cy*cxDIB/cyDIB;
		}
	}

	// Bordure pour le labelX
	long nBottomBorder=0;
	{
		nBottomBorder=(long)(rcDest.Height()*0.05);
		if (ImageSize.cy>(rcDest.Height()-nTopBorder-nBottomBorder))
		{
			ImageSize.cy-=nBottomBorder;
			ImageSize.cx=ImageSize.cy*cxDIB/cyDIB;
		}
	}

	// Bordure pour le labelY
	long nLeftBorder=0;
	{
		nLeftBorder=(long)(rcDest.Width()*0.05);
		if (ImageSize.cx>(rcDest.Width()-nLeftBorder))
		{
			ImageSize.cx-=nLeftBorder;
			ImageSize.cy=ImageSize.cx*cyDIB/cxDIB;
		}
	}

	// Bordure droite
	long nRightBorder=0;
	if (m_nColorBarStyle==0)
	{
		nRightBorder=(long)(rcDest.Width()*0.05);
		if (ImageSize.cx>(rcDest.Width()-nLeftBorder-nRightBorder))
		{
			ImageSize.cx-=nRightBorder;
			ImageSize.cy=ImageSize.cx*cyDIB/cxDIB;
		}
	}


	// Echelle
	long nVSeparator=0;
	CSize ColorBarSize(0,0);
	if (m_nColorBarStyle!=0)
	{
		nVSeparator=(long)(rcDest.Width()*0.05);
		ColorBarSize.cx=(long)(rcDest.Width()*0.2);
		if (ImageSize.cx>(rcDest.Width()-nLeftBorder-nRightBorder-nVSeparator-ColorBarSize.cx))
		{
			ImageSize.cx=rcDest.Width()-nLeftBorder-nRightBorder-nVSeparator-ColorBarSize.cx;
			ImageSize.cy=ImageSize.cx*cyDIB/cxDIB;
		}
		ColorBarSize.cy=ImageSize.cy;
	}

	//Affichage DIB image
	CRect rcImage(
		CPoint(
			nLeftBorder,
			nTopBorder),
		ImageSize);

	m_rcColorBar=CRect(
		CPoint(
			rcImage.right+nVSeparator,
			rcImage.top),
		ColorBarSize);

	m_rcTitle=CRect(
		CPoint(
			rcImage.left,
			0),
		CSize(
			rcImage.Width(),
			rcImage.top) );

	m_rcXLabel=CRect(
		CPoint(
			rcImage.left,
			rcImage.bottom),
		CSize(
			rcImage.Width(),
			nBottomBorder) );

	m_rcYLabel=CRect(
		CPoint(
			0,
			rcImage.top),
		CSize(
			nLeftBorder,
			rcImage.Height() ));

	// Centrage
	long nWidth=nLeftBorder+ImageSize.cx+nVSeparator+ColorBarSize.cx+nRightBorder;
	long nHeight=nTopBorder+ImageSize.cy+nBottomBorder;
	CPoint ptOffset(0,0);

	switch (m_XPos)
	{
		case 1:	// Gauche
			ptOffset.x=rcDest.left;
			break;
		case 2:	// Droite
			ptOffset.x=rcDest.left+rcDest.Width()-nWidth;
			break;
		case 0:	// Centre 
		default:
			ptOffset.x=rcDest.left+(rcDest.Width()-nWidth)/2;
			break;
	}

	switch (m_YPos)
	{
		case 1:	// Gauche
			ptOffset.y=rcDest.top;
			break;
		case 2: // Droite
			ptOffset.y=rcDest.top+rcDest.Height()-nHeight;
			break;
		case 0:	// Centre
		default:
			ptOffset.y=rcDest.top+(rcDest.Height()-nHeight)/2;
			break;
	}

	rcImage.OffsetRect(ptOffset);
	m_rcColorBar.OffsetRect(ptOffset);
	m_rcTitle.OffsetRect(ptOffset);
	m_rcXLabel.OffsetRect(ptOffset);
	m_rcYLabel.OffsetRect(ptOffset);

	// Affichages
	if (m_nColorBarStyle==0)
	{
		DrawFillRect(pDC, CRect(0,rcImage.top,rcImage.left+1,rcImage.bottom+1), m_crBkColor);
		DrawFillRect(pDC, CRect(0,0,rcDest.right+1,rcImage.top+1), m_crBkColor);
		DrawFillRect(pDC, CRect(0,rcImage.bottom,rcDest.right+1,rcDest.bottom+1), m_crBkColor);
		DrawFillRect(pDC, CRect(rcImage.right,rcImage.top-1,rcDest.right+1,rcImage.bottom+1), m_crBkColor);
	}
	else
	{
		DrawFillRect(pDC, CRect(0,rcImage.top,rcImage.left+1,rcImage.bottom+1), m_crBkColor);
		DrawFillRect(pDC, CRect(0,0,rcDest.right+1,rcImage.top+1), m_crBkColor);
		DrawFillRect(pDC, CRect(0,rcImage.bottom,rcDest.right+1,rcDest.bottom+1), m_crBkColor);
		DrawFillRect(pDC, CRect(rcImage.right,rcImage.top-1,m_rcColorBar.left+1,m_rcColorBar.bottom+1),m_crBkColor);
		DrawFillRect(pDC, CRect(m_rcColorBar.right-1,m_rcColorBar.top-1,rcDest.right+1,rcDest.bottom+1),m_crBkColor);
		DrawColorBar(pDC,m_rcColorBar);
	}

	DrawImage(pDC,rcImage);
	DrawROI(pDC);
	DrawTitle(pDC,m_rcTitle);
	DrawXLabel(pDC,m_rcXLabel);
	DrawYLabel(pDC,m_rcYLabel);
	((CIntensityProfile*)m_pXIntensityProfile)->Draw(pDC,rcImage);
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetStatisticsPP(long Value)
{
	m_nStatisticsPP=Value;
}

//////////////////////////////////////////////////////////////////////
long CH3Display::GetStatisticsPP()
{
	return m_nStatisticsPP;
}

//////////////////////////////////////////////////////////////////////
bool CH3Display::Save(const CString &strFilename)
{
	return false;
}

//////////////////////////////////////////////////////////////////////
LOGFONT CH3Display::GetLogFont()
{
	return m_LogFont;
}

//////////////////////////////////////////////////////////////////////
long CH3Display::GetRangeMode()
{
	return m_nRangeMode;
}

//////////////////////////////////////////////////////////////////////
H3_ARRAY_FLT32 CH3Display::GetRange()
{
	H3_ARRAY_FLT32 Res(2);
	if (m_Range.GetSize()==4)
	{
		Res[0]=m_Range[2];
		Res[1]=m_Range[3];
	}
	else
	{
		Res[0]=0;
		Res[1]=0;
	}

	return Res;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetROI(H3_RECT_INT32 rc)
{
	m_ROI.pts=H3_ARRAY_PT2DFLT32(4);

	m_ROI.pts[0]=H3_POINT2D_FLT32((float)rc.left,(float)rc.top);
	m_ROI.pts[1]=H3_POINT2D_FLT32((float)rc.right,(float)rc.top);
	m_ROI.pts[2]=H3_POINT2D_FLT32((float)rc.right,(float)rc.bottom);
	m_ROI.pts[3]=H3_POINT2D_FLT32((float)rc.left,(float)rc.bottom);

}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetROI(H3_ARRAY_PT2DINT32 pts)
{
	m_ROI.pts=(H3_ARRAY_PT2DFLT32)pts;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetROIColor(COLORREF crColor)
{
	m_ROI.crColor=crColor;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetROIStyle(long nStyle)
{
	m_ROI.nStyle=nStyle;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::DrawROI(CDC *pDC)
{
	if (m_ROI.nStyle==0)
		return;

	if (m_ROI.pts.GetSize()<3)
		return;

	H3_ARRAY_PT2DFLT32 pts=BufToImage(m_ROI.pts);
	float EcartX = (BufToImage(H3_POINT2D_FLT32(1,0)) - BufToImage(H3_POINT2D_FLT32(0,0))).x/2;
	float EcartY = (BufToImage(H3_POINT2D_FLT32(0,1)) - BufToImage(H3_POINT2D_FLT32(0,0))).y/2;
	pts+=H3_POINT2D_FLT32((float)m_rcImage.left,(float)m_rcImage.top);

	CPen pen (PS_SOLID,m_ROI.nStyle,m_ROI.crColor);
	CPen * pOldPen=pDC->SelectObject(&pen);
	
	pDC->MoveTo((int)(pts[0].x+EcartX),(int)(pts[0].y+EcartY));	
	for (unsigned long i=1;i<m_ROI.pts.GetSize();i++)
	{
		pDC->LineTo((int)(pts[i].x+EcartX),(int)(pts[i].y+EcartY));
	}
	pDC->LineTo((int)(pts[0].x+EcartX),(int)(pts[0].y+EcartY));

	pDC->SelectObject(pOldPen);
	DeleteObject(pen);
}

//////////////////////////////////////////////////////////////////////
bool CH3Display::SetColorMap1(const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("SetColorMap()");
	FILE *stream;

	H3_ARRAY2D_UINT8 ColorMap(256,3);

	if ((stream=fopen(LPCTSTR(strFilename),"rt"))) 
	{
		int a,b,c;

		for (unsigned long i=0;i<ColorMap.GetLi()-1;i++) 
		{
			fscanf(stream,"%d %d %d",&a,&b,&c);
			ColorMap(i,0)=(BYTE)a;
			ColorMap(i,1)=(BYTE)b;
			ColorMap(i,2)=(BYTE)c;
		}
		fclose(stream);
		return SetColorMap1(ColorMap);
	}

	return SetColorMap1(H3_ARRAY2D_UINT8(0,0));
}

//////////////////////////////////////////////////////////////////////
H3_ARRAY2D_UINT8 CH3Display::GetColorMap1()
{
	return m_ColorMap1;
}

//////////////////////////////////////////////////////////////////////
// Position X dans la zone cliente
// 0 : centre	(defaut)
// 1   gauche
// 2   droite
void CH3Display::SetXPosMode(long nPos)
{
	m_XPos=nPos;
}

//////////////////////////////////////////////////////////////////////
long CH3Display::GetXPosMode()
{
	return m_XPos;
}

//////////////////////////////////////////////////////////////////////
// Position X dans la zone cliente
// 0 : centre	(defaut)
// 1   haut
// 2   bas
void CH3Display::SetYPosMode(long nPos)
{
	m_YPos=nPos;
}

//////////////////////////////////////////////////////////////////////
long CH3Display::GetYPosMode()
{
	return m_YPos;
}

//////////////////////////////////////////////////////////////////////
int CH3Display::GetScaleFactor()
{
	return m_nScaleFactorPowerTen;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetScaleFactorPowerTen(int nScaleFactorPowerTen)
{
	m_nScaleFactorPowerTen = nScaleFactorPowerTen;
}

//////////////////////////////////////////////////////////////////////
float CH3Display::PowerTen(int PowerTen)
{
	float valeur=1;


	for (int i=1;i<=abs(PowerTen);i++)
	{
		valeur=10*valeur;
	}

	if (PowerTen<0)
		return ((float)(1/valeur));

	return valeur;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::GetSettings(H3DISPLAY_SETTINGS *pSettings)
{
	pSettings->nColorBarStyle=m_nColorBarStyle;
	pSettings->nRangeMode=m_nRangeMode;
	pSettings->Range=GetRange();
	pSettings->strTitle=m_strTitle;
	pSettings->strXLabel=m_strXLabel;
	pSettings->strYLabel=m_strYLabel;
	pSettings->ColorMap1=GetColorMap1();
	pSettings->ColorMap2=GetColorMap2();
	pSettings->strScaleLabel=m_strScaleLabel;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetSettings(H3DISPLAY_SETTINGS *pSettings)
{
	SetColorBarStyle(pSettings->nColorBarStyle);

	SetRangeMode(pSettings->nRangeMode);
	if (pSettings->nRangeMode==0)
		SetRange(pSettings->Range);

	SetTitle(pSettings->strTitle);
	SetXLabel(pSettings->strXLabel);
	SetYLabel(pSettings->strYLabel);
	SetColorMap1(pSettings->ColorMap1);
	SetColorMap2(pSettings->ColorMap2);
	SetScaleLabel(pSettings->strScaleLabel);
}

//////////////////////////////////////////////////////////////////////
H3_ARRAY2D_UINT8 CH3Display::GetColorMap2()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("GetColorMap2()");

	return m_ColorMap2;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetColorMap2(H3_ARRAY2D_UINT8 & ColorMap)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("SetColorMap2()");

	m_ColorMap2=ColorMap;

	if (ColorMap.GetCo()!=3 || ColorMap.GetLi()!=8)
	{
		H3DisplayError(strModule,strFunction,_T("\nParamètre ColorMap invalide."));
		return;
	}

	m_ColorMap2=ColorMap;

	if (m_pImageDIB)
	{
		CDib *pDIB=(CDib *)m_pImageDIB;
		pDIB->SetColorMaps(m_ColorMap1,m_ColorMap2);
	}
}

//////////////////////////////////////////////////////////////////////
CRect CH3Display::GetTitleRect()
{
	return m_rcTitle;
}

//////////////////////////////////////////////////////////////////////
CRect CH3Display::GetXLabelRect()
{
	return m_rcXLabel;
}

//////////////////////////////////////////////////////////////////////
CRect CH3Display::GetYLabelRect()
{
	return m_rcYLabel;
}

//////////////////////////////////////////////////////////////////////
CRect CH3Display::GetColorBarRect()
{
	return m_rcColorBar;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetBkColor(COLORREF crColor)
{
	m_crBkColor=crColor;
}

//////////////////////////////////////////////////////////////////////
COLORREF CH3Display::GetBkColor()
{
	return m_crBkColor;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetScaleLabel(CString strText)
{
	m_strScaleLabel=strText;
}

//MF
/*! 
* 	\fn      : CH3Display::DrawXAxePixel
* 	\brief   : Dessiner la Regle Pixel sur l'axe X 
* 	\param   : CDC *pDC : 
* 	\param   : CRect &rcDest : 
* 	\return  : void
* 	\remarks : 
*/ 

void CH3Display::DrawXAxePixel(CDC *pDC,CRect &rcDest)
{
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	LOGFONT LogFont;

	memcpy(&LogFont,&m_LogFont,sizeof(LOGFONT));
	LogFont.lfHeight = rcDest.Height()-2;
	if(LogFont.lfHeight>16)
		LogFont.lfHeight=16;
	else if (LogFont.lfHeight<5)
		LogFont.lfHeight=8000;

	CFont Font;
	Font.CreateFontIndirect(&LogFont);


	CString strMin;strMin.Format(" 0");
	CString strMax;strMax.Format("%d ",((CDib*)m_pImageDIB)->Width());
	CString strPixel("Pixel");
 
	DrawText(
	pDC,
	strMin,
	rcDest,
	DT_LEFT|DT_SINGLELINE|DT_VCENTER,
	Font);

	DrawText(
	pDC,
	strMax,
	rcDest,
	DT_RIGHT|DT_SINGLELINE|DT_VCENTER,
	Font);

	DrawText(
	pDC,
	strPixel,
	rcDest,
	DT_CENTER|DT_SINGLELINE|DT_VCENTER,
	Font);

	DrawXReglePixel(pDC,rcDest);
}


/*! 
* 	\fn      : DrawXReglePixel
* 	\brief   : dessin d'une regle horizontal
* 	\param   : CDC *pDC : 
* 	\param   : CRect &rcRect : 
* 	\return  : void
* 	\remarks : 
*/ 

void CH3Display::DrawXReglePixel(CDC *pDC,CRect &rcRect)
{
	if (!pDC) return;
	if (rcRect.IsRectEmpty()) return;

	float fStep=(rcRect.right-rcRect.left)/10.0F;

	//premier
	pDC->MoveTo(rcRect.left,rcRect.top);
	pDC->LineTo(rcRect.left,rcRect.bottom-(rcRect.bottom-rcRect.top)*1/6);		
	//dernier
	pDC->MoveTo(rcRect.right,rcRect.top);
	pDC->LineTo(rcRect.right,rcRect.bottom-(rcRect.bottom-rcRect.top)*1/6);		


	int i;
	for (i=1;i<6;i++)
	{
		pDC->MoveTo((int)(rcRect.left+fStep*i),rcRect.top);
		pDC->LineTo((int)(rcRect.left+fStep*i),(int)(rcRect.bottom-(rcRect.bottom-rcRect.top)*4/6));		
	}
	for (i=1;i<5;i++)
	{
		pDC->MoveTo((int)(rcRect.right-fStep*i),rcRect.top);
		pDC->LineTo((int)(rcRect.right-fStep*i),(int)(rcRect.bottom-(rcRect.bottom-rcRect.top)*4/6));		
	}
	for (i=0;i<6;i++)
	{
		pDC->MoveTo((int)(rcRect.left+fStep/2+fStep*i),rcRect.top);
		pDC->LineTo((int)(rcRect.left+fStep/2+fStep*i),(int)(rcRect.bottom-(rcRect.bottom-rcRect.top)*5/6));		
	}
	for (i=0;i<4;i++)
	{
		pDC->MoveTo((int)(rcRect.right-fStep/2-fStep*i),rcRect.top);
		pDC->LineTo((int)(rcRect.right-fStep/2-fStep*i),(int)(rcRect.bottom-(rcRect.bottom-rcRect.top)*5/6));		
	}
}

/*! 
* 	\fn      : CH3Display::DrawYAxePixel
* 	\brief   : Dessiner la Regle Pixel sur l'axe Y 
* 	\param   : CDC *pDC : 
* 	\param   : CRect &rcDest : 
* 	\return  : void
* 	\remarks : 
*/void CH3Display::DrawYAxePixel(CDC *pDC,CRect &rcDest)
{
	if (rcDest.IsRectEmpty()) return;
	if (!pDC) return;

	LOGFONT LogFont;

	memcpy(&LogFont,&m_LogFont,sizeof(LOGFONT));
	LogFont.lfHeight = rcDest.Height()-2;
	if(LogFont.lfHeight>16)
		LogFont.lfHeight=16;
	else if (LogFont.lfHeight<5)
		LogFont.lfHeight=8000;

	CFont Font;
	Font.CreateFontIndirect(&LogFont);


	CString strMin;strMin.Format("0");
	CString strMax;strMax.Format("%d",((CDib*)m_pImageDIB)->Height());
	CString strPixel("Pixel");
 
	DrawText(
	pDC,
	strMax,
	rcDest,
	DT_BOTTOM|DT_SINGLELINE,
	Font);

	DrawText(
	pDC,
	strMin,
	rcDest,
	DT_TOP|DT_SINGLELINE,
	Font);

	DrawText(
	pDC,
	strPixel,
	rcDest,
	DT_SINGLELINE|DT_VCENTER,
	Font);

	DrawYReglePixel(pDC,rcDest);

}

/*! 
* 	\fn      : DrawYReglePixel
* 	\brief   : dessin d'une regle verticale
* 	\param   : CDC *pDC : 
* 	\param   : CRect &rcRect : 
* 	\return  : void
* 	\remarks : 
*/ 
void CH3Display::DrawYReglePixel(CDC *pDC,CRect &rcRect)
{
	if (!pDC) return;
	if (rcRect.IsRectEmpty()) return;


	float fStep=(rcRect.bottom-rcRect.top)/10.0F;


	//premier
	pDC->MoveTo(rcRect.right,rcRect.top);
	pDC->LineTo(rcRect.left+(rcRect.right-rcRect.left)*1/6,rcRect.top);		
	//dernier
	pDC->MoveTo(rcRect.right,rcRect.bottom);
	pDC->LineTo(rcRect.left+(rcRect.right-rcRect.left)*1/6,rcRect.bottom);		

	int i;
	for (i=1;i<6;i++)
	{
		pDC->MoveTo(rcRect.right,(int)(rcRect.top+fStep*i));
		pDC->LineTo((int)(rcRect.left+(rcRect.right-rcRect.left)*4/6),(int)(rcRect.top+fStep*i));		
	}
	for (i=1;i<5;i++)
	{
		pDC->MoveTo(rcRect.right,(int)(rcRect.bottom-fStep*i));
		pDC->LineTo((int)(rcRect.left+(rcRect.right-rcRect.left)*4/6),(int)(rcRect.bottom-fStep*i));		
	}

	for (i=0;i<6;i++)
	{
		pDC->MoveTo(rcRect.right,(int)(rcRect.top+fStep*i+fStep/2));
		pDC->LineTo((int)(rcRect.left+(rcRect.right-rcRect.left)*5/6),(int)(rcRect.top+fStep*i+fStep/2));		
	}
	for (i=0;i<4;i++)
	{
		pDC->MoveTo(rcRect.right,(int)(rcRect.bottom-fStep*i-fStep/2));
		pDC->LineTo((int)(rcRect.left+(rcRect.right-rcRect.left)*5/6),(int)(rcRect.bottom-fStep*i-fStep/2));		
	}

}

//////////////////////////////////////////////////////////////////////
bool CH3Display::GetbDrawAxePixel()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return m_bDrawAxePixel;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetbDrawAxePixel(bool bDrawAxePixel)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_bDrawAxePixel=bDrawAxePixel;
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetXProfileColor(COLORREF crColor)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	((CIntensityProfile*)m_pXIntensityProfile)->SetColor(crColor);
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetXProfileStyle(long nStyle)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	((CIntensityProfile*)m_pXIntensityProfile)->SetStyle(nStyle);
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetXProfileView(bool bView)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	((CIntensityProfile*)m_pXIntensityProfile)->SetView(bView);
}

//////////////////////////////////////////////////////////////////////
bool CH3Display::GetXProfileView()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return ((CIntensityProfile*)m_pXIntensityProfile)->GetView();
}

//////////////////////////////////////////////////////////////////////
void CH3Display::SetXProfileMax(float fAbsMax)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	((CIntensityProfile*)m_pXIntensityProfile)->SetAbsMax(fAbsMax);
}

//////////////////////////////////////////////////////////////////////
H3_ARRAY_PT2DFLT32 CH3Display::BufToImage(H3_ARRAY_PT2DFLT32 & pts)
{
	H3_ARRAY_PT2DFLT32 ptsImage(pts);

	if (m_pImageDIB)
	{
		float nDIBSizeX=(float)((CDib*)m_pImageDIB)->Width();
		float nDIBSizeY=(float)((CDib*)m_pImageDIB)->Height();

		float nImageSizeX=(float)m_rcImage.Width();
		float nImageSizeY=(float)m_rcImage.Height();

		H3_POINT2D_FLT32 Scale(
			(nDIBSizeX)/(nImageSizeX),
			(nDIBSizeY)/(nImageSizeY));


		ptsImage/=Scale;
	}

	return ptsImage;

}


//////////////////////////////////////////////////////////////////////
H3_POINT2D_FLT32 CH3Display::BufToImage(H3_POINT2D_FLT32 & ptBuf)
{
	H3_POINT2D_FLT32 ptDIB;
	if (m_pImageDIB)
	{
		float nDIBSizeX=(float)(((CDib*)m_pImageDIB)->Width());
		float nDIBSizeY=(float)(((CDib*)m_pImageDIB)->Height());

		float nImageSizeX=(float)m_rcImage.Width();
		float nImageSizeY=(float)m_rcImage.Height();


		H3_POINT2D_FLT32 Scale(
			(nDIBSizeX)/(nImageSizeX),
			(nDIBSizeY)/(nImageSizeY));

		ptDIB=ptBuf/Scale;
	}

	return ptDIB;
}
