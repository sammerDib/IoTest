// H3Display.cpp: implementation of the CH3Display class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "H3DisplayTools.h"
#include "H3Display.h"

#include "dib.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

#define DEFAULT_SECTION			"H3Display"
#define DEFAULT_TYPE			H3_VGA
#define DEFAULT_DEVICE			0
#define DEFAULT_FORMAT			""
#define DEFAULT_EDGE_TIME		0

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

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
CH3Display::CH3Display()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("CH3Display()");
	CString strMsg=_T("Impossible de créer l'afficheur.");

	InitMembers();
	m_pImageDIB=(void *)new CDib();

	m_nGridShow=0;
	m_nColorBarShow=0;	
	m_nMaxScale = 0;
	m_nMinScale = 0;
	m_strTitle  = _T("");
	m_strXLabel = _T("");
	m_strYLabel = _T("");

}

CH3Display::~CH3Display()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	Free();
}

void CH3Display::Free()
{
	if (m_pImageDIB)
		delete (CDib *)m_pImageDIB;

	InitMembers();
}

void CH3Display::InitMembers()
{
	m_hWnd=NULL;
	m_nModifiedMsg=0;
	m_pImageDIB=NULL;
}

// Retourne true si l'objet est correctement alloue
bool CH3Display::IsAllocated()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (m_pImageDIB)
		return true;
	return false;
}

void CH3Display::SetClientWindow(HWND hWnd)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_hWnd=hWnd;
}

void CH3Display::SetModifiedMessage(UINT Msg)
{
	m_nModifiedMsg=Msg;
}

void CH3Display::SendModifiedMessage()
{
/*
	if (m_hWnd)
	{
		SendMessage(m_hWnd,m_nModifiedMsg,0,0);
	}
*/
}

void CH3Display::Set(H3_ARRAY2D_UINT8 &SrcBuf)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("Set(H3_ARRAY2D_UINT8)");
	CString strMsg=_T("Impossible d'afficher.");

	if (!SrcBuf.IsEmpty())
	{
		if (m_hWnd && m_pImageDIB)
		{
			CDib *pDIB=(CDib *)m_pImageDIB;
			pDIB->SetLUT(m_LUTBuf);
			pDIB->Set(SrcBuf);
			DisplayWnd(pDIB);
		}
		else
		{
			DisplayError(strModule,strFunction,strMsg+_T("\nm_hWnd est NULL."));
		}
	}
	else
	{
		DisplayError(strModule,strFunction,strMsg+_T("\nSrcBuf est vide."));
	}
}

void CH3Display::Set(H3_ARRAY2D_FLT32 &SrcBuf)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction=_T("Set()");
	CString strMsg=_T("Impossible d'afficher.");

	if (!SrcBuf.IsEmpty())
	{
		if (m_hWnd && m_pImageDIB)
		{
			CDib *pDIB=(CDib *)m_pImageDIB;
			pDIB->SetLUT(m_LUTBuf);
			pDIB->Set(SrcBuf);
			DisplayWnd(pDIB);
		}
		else
		{
			DisplayError(strModule,strFunction,strMsg+_T("\nm_hWnd est NULL."));
		}
	}
	else
	{
		DisplayError(strModule,strFunction,strMsg+_T("\nSrcBuf est vide."));
	}
}

void CH3Display::Set(H3_ARRAY2D_CPXFLT32 &SrcBuf)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
}

void CH3Display::SetColorBarShow(CRect rcDestScale)
{
	/* Preparation image de l'echelle */
	H3_ARRAY2D_UINT8 SrcBufScale;
	SrcBufScale=VertRamp(256,256);
	CDib *pDIBScale=(CDib *)m_pImageDIB;
	pDIBScale->SetLUT(m_LUTBuf);
	pDIBScale->Set(SrcBufScale);

	/* Calcul zone de l'image */
	int cxDIBScale = pDIBScale->Width();     
	int cyDIBScale = pDIBScale->Height(); 
	CRect rcDIBScale;
	rcDIBScale.top = rcDIBScale.left = 0;
	rcDIBScale.right = cxDIBScale;
	rcDIBScale.bottom = cyDIBScale;
	int tempo=rcDestScale.left;
	rcDestScale.left=(rcDestScale.left-rcDestScale.right)/5+rcDestScale.right;
	/* Affichage de l'echelle */
	pDIBScale->Paint(GetDC(m_hWnd), &rcDestScale, &rcDIBScale);
	rcDestScale.left=tempo;

	/* Preparation du DC riture */
	CDC *pDC;
	pDC = new CDC;
	pDC = pDC->FromHandle(GetDC(m_hWnd));

	/* Preparation de la graduation */
	int i,x,y,trait = (rcDestScale.right - rcDestScale.left)/30;
	CRect rcScale;
	
	// Gestion de la graduation
	int nbGradue =((rcDestScale.bottom - rcDestScale.top)/30);
	if (nbGradue!=0)
	{
		/* Preparation du font d'ecriture text */
		CFont pFont;
		LOGFONT pLogFont;

		pLogFont.lfCharSet			= GB2312_CHARSET;
		pLogFont.lfClipPrecision	= CLIP_DEFAULT_PRECIS;
		pLogFont.lfEscapement		= 2;
		pLogFont.lfItalic			= false;
		pLogFont.lfOrientation		= 0;
		pLogFont.lfOutPrecision		= OUT_CHARACTER_PRECIS;
		pLogFont.lfPitchAndFamily	= DEFAULT_PITCH;
		pLogFont.lfQuality			= DRAFT_QUALITY;//DEFAULT_QUALITY;
		pLogFont.lfStrikeOut		= false;
		pLogFont.lfUnderline		= false;
		pLogFont.lfWeight			= FW_NORMAL;
		pLogFont.lfWidth			= 0;
		
		rcScale.left= (rcDestScale.left-rcDestScale.right)/5+rcDestScale.right+2;
		rcScale.right= rcDestScale.left;

		if(rcScale.Width()>6*8)
			pLogFont.lfHeight			= 16;
		else if(rcScale.Width()>6*5)
			pLogFont.lfHeight			= 11;
		else if(rcScale.Width()>6*4.5)
			pLogFont.lfHeight			= 10;
		else if(rcScale.Width()>6*3.5)
			pLogFont.lfHeight			= 8;
		else if(rcScale.Width()>6*3)
			pLogFont.lfHeight			= 7;
		else
			pLogFont.lfHeight			= 80000;

		pFont.CreateFontIndirect(&pLogFont);
		CFont *pOldFont = (CFont *) pDC->SelectObject(&pFont);
		
		/* boucle d'ecriture de la graduation de l'échelle */
		for(i=0;i<=nbGradue;i++)
		{
			/* tiret de Graduation */
			x = (rcDestScale.left-rcDestScale.right)/5+rcDestScale.right;//rcDestScale.left;
			y = rcDestScale.top + i * (rcDestScale.bottom - rcDestScale.top) /nbGradue;
			if (i!=0)
			{
				pDC->MoveTo(x+1,y);	
				pDC->LineTo(x+trait+1,y);
				x = rcDestScale.right;
				pDC->MoveTo(x,y);	
				pDC->LineTo(x-trait,y);
			}						
			rcScale.bottom=y+(pLogFont.lfHeight)/2;
			rcScale.top=y-(pLogFont.lfHeight)/2;
			
			/* text de graduation */
			rcScale.left= (rcDestScale.left-rcDestScale.right)/5+rcDestScale.right+2;
			rcScale.right= rcDestScale.left;

			CString strResultat,strFmt;

			double d=m_nMaxScale-m_nMinScale;
			if (d!=0)
			{
				if (fabs(d)<999) strFmt="%.3f";
				else if(fabs(d)<9999) strFmt="%.2f";
					else if (fabs(d)<99999) strFmt="%.1f";
						else if (fabs(d)<999999) strFmt="%.0f";
							else strFmt="%5.2g";
			}
			else
			{
				if (fabs(m_nMaxScale)<999) strFmt="%.3f";
				else if(fabs(m_nMaxScale)<9999) strFmt="%.2f";
					else if (fabs(m_nMaxScale)<99999) strFmt="%.1f";
						else if (fabs(m_nMaxScale)<999999) strFmt="%.0f";
							else strFmt="%5.2g";
			}


			float nResultat = (nbGradue-i)*((m_nMaxScale-m_nMinScale)/nbGradue )+ m_nMinScale; 
			strResultat.Format(strFmt,nResultat);
			
			pDC->DrawText(strResultat,rcScale,DT_LEFT );
		}
		pDC->SelectObject(pOldFont);
	}
}

void CH3Display::SetTitleShow(CRect rcDestTitle)
{
	CDC *pDC;
	pDC = new CDC;
	pDC = pDC->FromHandle(GetDC(m_hWnd));

	/* Preparation du font d'ecriture text */
	CFont pFont;
	LOGFONT pLogFont;

	pLogFont.lfCharSet			= GB2312_CHARSET;
	pLogFont.lfClipPrecision	= CLIP_DEFAULT_PRECIS;
	pLogFont.lfEscapement		= 2;
	pLogFont.lfItalic			= false;
	pLogFont.lfOrientation		= 0;
	pLogFont.lfOutPrecision		= OUT_CHARACTER_PRECIS;
	pLogFont.lfPitchAndFamily	= DEFAULT_PITCH;
	pLogFont.lfQuality			= DRAFT_QUALITY;//DEFAULT_QUALITY;
	pLogFont.lfStrikeOut		= false;
	pLogFont.lfUnderline		= false;
	pLogFont.lfWeight			= FW_NORMAL;
	pLogFont.lfWidth			= 0;
	
	pLogFont.lfHeight			= rcDestTitle.Height();
	if(pLogFont.lfHeight>16)
		pLogFont.lfHeight=16;

	pFont.CreateFontIndirect(&pLogFont);
	CFont *pOldFont = (CFont *) pDC->SelectObject(&pFont);


	pDC->DrawText(m_strTitle,rcDestTitle,DT_CENTER);

	pDC->SelectObject(pOldFont);

}

void CH3Display::SetXLabelShow(CRect rcDestXLabel)
{
	CDC *pDC;
	pDC = new CDC;
	pDC = pDC->FromHandle(GetDC(m_hWnd));

	/* Preparation du font d'ecriture text */
	CFont pFont;
	LOGFONT pLogFont;

	pLogFont.lfCharSet			= GB2312_CHARSET;
	pLogFont.lfClipPrecision	= CLIP_DEFAULT_PRECIS;
	pLogFont.lfEscapement		= 2;
	pLogFont.lfItalic			= false;
	pLogFont.lfOrientation		= 0;
	pLogFont.lfOutPrecision		= OUT_CHARACTER_PRECIS;
	pLogFont.lfPitchAndFamily	= DEFAULT_PITCH;
	pLogFont.lfQuality			= DRAFT_QUALITY;//DEFAULT_QUALITY;
	pLogFont.lfStrikeOut		= false;
	pLogFont.lfUnderline		= false;
	pLogFont.lfWeight			= FW_NORMAL;
	pLogFont.lfWidth			= 0;
	
	pLogFont.lfHeight			= rcDestXLabel.Height();
	if(pLogFont.lfHeight>16)
		pLogFont.lfHeight=16;

	pFont.CreateFontIndirect(&pLogFont);
	CFont *pOldFont = (CFont *) pDC->SelectObject(&pFont);


	pDC->DrawText(m_strXLabel,rcDestXLabel,DT_CENTER  );

	pDC->SelectObject(pOldFont);

}

void CH3Display::SetYLabelShow(CRect rcDestYLabel)
{
	CDC *pDC;
	pDC = new CDC;
	pDC = pDC->FromHandle(GetDC(m_hWnd));

	/* Preparation du font d'ecriture text */
	CFont pFont;
	LOGFONT pLogFont;

	pLogFont.lfCharSet			= GB2312_CHARSET;
	pLogFont.lfClipPrecision	= CLIP_DEFAULT_PRECIS;
	pLogFont.lfEscapement		= 2;
	pLogFont.lfItalic			= false;
	pLogFont.lfOrientation		= 90;
	pLogFont.lfOutPrecision		= OUT_CHARACTER_PRECIS;
	pLogFont.lfPitchAndFamily	= DEFAULT_PITCH;
	pLogFont.lfQuality			= DRAFT_QUALITY;//DEFAULT_QUALITY;
	pLogFont.lfStrikeOut		= false;
	pLogFont.lfUnderline		= false;
	pLogFont.lfWeight			= FW_NORMAL;
	pLogFont.lfWidth			= 0;
	
	pLogFont.lfHeight			= rcDestYLabel.Height();
	if(pLogFont.lfHeight>16)
		pLogFont.lfHeight=16;

	pFont.CreateFontIndirect(&pLogFont);
	CFont *pOldFont = (CFont *) pDC->SelectObject(&pFont);


	pDC->DrawText(m_strYLabel,rcDestYLabel,DT_CENTER);

	pDC->SelectObject(pOldFont);
}

void CH3Display::SetTitle(CString strTitle)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_strTitle=strTitle;
}

void CH3Display::SetXLabel(CString strXLabel)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_strXLabel=strXLabel;
}

void CH3Display::SetYLabel(CString strYLabel)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_strYLabel=strYLabel;
}

void CH3Display::SetMaxMinScale(double nMaxScale,double nMinScale)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	m_nMinScale=nMinScale;
	m_nMaxScale=nMaxScale;
	if (m_nMaxScale==m_nMinScale)
	{
		m_nMaxScale=m_nMaxScale+10*m_nMaxScale/100;
		m_nMinScale=m_nMinScale-10*m_nMinScale/100;
	}
}

bool CH3Display::SetLUT(H3_ARRAY2D_UINT8 &LUTBuf)
{
	m_LUTBuf=LUTBuf;
	return true;
}

void CH3Display::SetGrid(int nShow)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	m_nGridShow=nShow;
}

void CH3Display::SetDrawGrid(CRect rcImage)
{
	long hauteur=rcImage.bottom-rcImage.top;
	long largeur=rcImage.left-rcImage.right;
	if(m_nGridShow==1)
	{
		if ((hauteur>=10) && (largeur >=10))
		{
			float pashauteur=((float) hauteur+1)/((float) 10);
			float paslargeur=((float)largeur+1)/((float) 10);
			float tirethauteur=hauteur/50;
			float tiretlargeur=largeur/50;
			if (tiretlargeur>tirethauteur)
				tiretlargeur=tirethauteur;
			else
				tirethauteur=tiretlargeur;
			int x,y;

			CDC *pDC;
			pDC = new CDC;
			pDC = pDC->FromHandle(GetDC(m_hWnd));

			//Grid horizontal
			for ( long i=1 ; i<10 ; i++)
			{
				x=rcImage.right;
				y=rcImage.top;

				pDC->MoveTo(x + i * paslargeur,y);	
				pDC->LineTo(x + i * paslargeur,y+tirethauteur);

				y=rcImage.bottom;

				pDC->MoveTo(x + i * paslargeur,y);	
				pDC->LineTo(x + i * paslargeur,y-tirethauteur);
			}

			//Grid verticale
			for ( i=1 ; i<10 ; i++)
			{
				x=rcImage.right;
				y=rcImage.top;

				pDC->MoveTo(x ,y + i * pashauteur);	
				pDC->LineTo(x + tiretlargeur,y + i * pashauteur);

				x=rcImage.left;

				pDC->MoveTo(x ,y + i * pashauteur);	
				pDC->LineTo(x - tiretlargeur,y + i * pashauteur);
			}
		}
	}	
	else if(m_nGridShow==2)
	{
		float pashauteur=((float) hauteur+1)/((float) 10);
		float paslargeur=((float)largeur+1)/((float) 10);

		CDC *pDC;
		pDC = new CDC;
		pDC = pDC->FromHandle(GetDC(m_hWnd));

		//Grid horizontal
		CBrush *pOldBrush = (CBrush *) pDC->SelectStockObject(NULL_BRUSH);
		CPen DashPen(PS_DOT,1,RGB(0,0,0));
		
		CPen * pOldPen;
		pOldPen=pDC->SelectObject(&DashPen);


		for ( long i=1 ; i<10 ; i++)
		{

			pDC->MoveTo(rcImage.right + i * paslargeur,rcImage.top);
			pDC->LineTo(rcImage.right + i * paslargeur,rcImage.bottom+1);	
		}

		//Grid verticale
		for ( i=1 ; i<10 ; i++)
		{
			pDC->MoveTo(rcImage.right ,rcImage.top + i * pashauteur);	
			pDC->LineTo(rcImage.left+1 ,rcImage.top + i * pashauteur);	
		}
		pDC->SelectObject(pOldBrush);
		pDC->SelectObject(pOldPen);
	}
}

void CH3Display::DisplayWnd(void *pmyDIB)
{
	CDib *pDIB=(CDib *)pmyDIB;
/*set*/
	int cxDIB = pDIB->Width();     
	int cyDIB = pDIB->Height(); 
	CRect rcDIB;
	rcDIB.top = rcDIB.left = 0;
	rcDIB.right = cxDIB;
	rcDIB.bottom = cyDIB;
	CRect rcDest;

	//Taille image dynamique
	double TauxCompression=0.85;
	double TauxCompressionWidth=0.85;

	if(m_nColorBarShow!=0)
		TauxCompressionWidth=0.75;
	
	::GetClientRect(m_hWnd,rcDest);

	int tempHeigth=rcDest.bottom-rcDest.top;
	int tempWidth=rcDest.right-rcDest.left;

/*rapport X/Y*/
	tempWidth=rcDest.right-rcDest.left;
	tempHeigth=tempWidth*cyDIB/cxDIB;
	if (tempHeigth>rcDest.bottom-rcDest.top)
	{
		tempHeigth=rcDest.bottom-rcDest.top;
		tempWidth=cxDIB*tempHeigth/cyDIB;
	}
/*/rapport X/Y*/

//	rcDest.top=tempHeigth*((1-TauxCompression)/2);
//	rcDest.bottom=rcDest.top+TauxCompression*tempHeigth;
//	rcDest.right=tempWidth*((1-TauxCompression)/2);
//	rcDest.left=rcDest.right+TauxCompressionWidth*tempWidth;

/*Centrage Image*/
	rcDest.top=(rcDest.Height())/2-TauxCompression*tempHeigth/2;
	rcDest.bottom=rcDest.top+TauxCompression*tempHeigth;
	rcDest.right=(rcDest.Width())/2-TauxCompressionWidth*tempWidth/2;
	rcDest.left=rcDest.right+TauxCompressionWidth*tempWidth;
/*/Centrage Image*/

	//Affichage Dib image
	pDIB->Paint(GetDC(m_hWnd), &rcDest, &rcDIB);
	
	//Affichage echelle
	if(m_nColorBarShow!=0)
		if(m_nColorBarShow==1)
		{
			CRect rcDestScale;

			//Taille image dynamique
			
			::GetClientRect(m_hWnd,rcDestScale);
			int tempHeigth=rcDestScale.bottom-rcDestScale.top;
			int tempWidth=rcDestScale.right-rcDestScale.left;

			rcDestScale.top=rcDest.top;//tempHeigth*((1-TauxCompression)/2);
			rcDestScale.bottom=rcDest.bottom;//rcDestScale.top+TauxCompression*tempHeigth;

			rcDestScale.right= rcDest.left + tempWidth/20;
			rcDestScale.left = tempWidth-5;// - tempWidth/20;//rcDestScale.right + tempWidth/20;
						
			SetColorBarShow(rcDestScale);
		}
	//Affichage Titre
	CRect rcDestTitle;
	
	::GetClientRect(m_hWnd,rcDestTitle);
	tempHeigth = rcDestTitle.bottom - rcDestTitle.top;
	tempWidth  = rcDestTitle.right - rcDestTitle.left;

	rcDestTitle.top    = rcDestTitle.top+1;
	rcDestTitle.bottom = rcDest.top-1;
	rcDestTitle.right  = rcDest.right;
	rcDestTitle.left   = rcDest.left;

	SetTitleShow(rcDestTitle);

	//Affichage XLabel
	CRect rcDestXLabel;
	
	::GetClientRect(m_hWnd,rcDestXLabel);
	tempHeigth = rcDestXLabel.bottom - rcDestXLabel.top;
	tempWidth  = rcDestXLabel.right - rcDestXLabel.left;

	rcDestXLabel.top    = rcDest.bottom+1;
	rcDestXLabel.bottom = rcDestXLabel.bottom-1;
	rcDestXLabel.right  = rcDest.right;
	rcDestXLabel.left   = rcDest.left;

	SetXLabelShow(rcDestXLabel);


	//Affichage YLabel
	CRect rcDestYLabel;
	
	::GetClientRect(m_hWnd,rcDestYLabel);
	tempHeigth = rcDestYLabel.bottom - rcDestYLabel.top;
	tempWidth  = rcDestYLabel.right - rcDestYLabel.left;

	rcDestYLabel.top    = rcDest.top;
	rcDestYLabel.bottom = rcDest.bottom;
	rcDestYLabel.right  = rcDestYLabel.left+1;
	rcDestYLabel.left   = rcDest.right-1;
	SetYLabelShow(rcDestYLabel);

	//Affichage Grid
	if (m_nGridShow!=0)
		SetDrawGrid(rcDest);


}

