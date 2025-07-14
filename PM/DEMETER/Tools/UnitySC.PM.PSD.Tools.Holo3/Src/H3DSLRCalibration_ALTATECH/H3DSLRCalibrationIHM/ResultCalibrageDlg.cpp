// ResultCalibrageDlg.cpp : implementation file
//

#include "stdafx.h"
//#include ".\\..\\H3DSLRCalibration\\H3DSLRCalibration.h"
#include "H3DSLRCalibrationIHMrc.h"
#include "ResultCalibrageDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CResultCalibrageDlg dialog


CResultCalibrageDlg::CResultCalibrageDlg(bool ValidCalib, CString strRes, CWnd* pParent /*=NULL*/)
	: CDialog(CResultCalibrageDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CResultCalibrageDlg)
	m_strColor = _T("");
	//}}AFX_DATA_INIT
	m_bValidCalib = ValidCalib;
	m_strColor = strRes;
}


void CResultCalibrageDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CResultCalibrageDlg)
	DDX_Control(pDX, IDC_COLOR, m_stColor);
///	DDX_Text(pDX, IDC_COLOR, m_strColor);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CResultCalibrageDlg, CDialog)
	//{{AFX_MSG_MAP(CResultCalibrageDlg)
	ON_WM_PAINT()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CResultCalibrageDlg message handlers

void CResultCalibrageDlg::OnPaint() 
{
	CPaintDC dc(this); // device context for painting
	
	if (m_stColor.GetSafeHwnd() != NULL)
	{
		CDC* pCdc = m_stColor.GetDC() ;
		CRect rcC;
		m_stColor.GetClientRect(rcC);
		if (m_bValidCalib)
		{
			pCdc->FillSolidRect(rcC, RGB(0, 255, 0));
			rcC.DeflateRect(10, 10, 10, 10);
			pCdc->SetTextColor(RGB(0, 0, 0));
			pCdc->DrawText(m_strColor, rcC, DT_CENTER );
		}
		else
		{
			pCdc->FillSolidRect(rcC, RGB(255, 0, 0));
			rcC.DeflateRect(10, 10, 10, 10);
			pCdc->SetTextColor(RGB(0, 0, 0));
			pCdc->DrawText(m_strColor, rcC, DT_LEFT);
		}
		m_stColor.ReleaseDC(pCdc);
	}
}

BOOL CResultCalibrageDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
///	if (m_bValidCalib)
///		m_strColor = _T("Le Calibrage a réussi");
///	else
///		m_strColor = _T("Le Calibrage a echoué\n"\
///			"- Vérifier la définition de la mire\n"\
///			"- Nombre d'image de la mire insuffisant");
///
///	UpdateData(false);
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}
