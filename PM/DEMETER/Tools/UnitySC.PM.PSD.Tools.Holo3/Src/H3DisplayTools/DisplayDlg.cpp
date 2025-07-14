// DisplayDlg.cpp : implementation file
//

#include "stdafx.h"
#include "H3DisplayTools.h"
#include "DisplayDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CDisplayDlg dialog


CDisplayDlg::CDisplayDlg(CH3Display *pDisplay,CWnd* pParent /*=NULL*/)
	: CDialog(CDisplayDlg::IDD, pParent)
{
	m_pDisplay=pDisplay;

	//{{AFX_DATA_INIT(CDisplayDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void CDisplayDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CDisplayDlg)
	DDX_Control(pDX, IDC_IMAGE, m_CImage);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CDisplayDlg, CDialog)
	//{{AFX_MSG_MAP(CDisplayDlg)
	ON_WM_DESTROY()
	ON_WM_CLOSE()
	ON_WM_SIZE()
	ON_WM_PAINT()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CDisplayDlg message handlers

BOOL CDisplayDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
//	ResizeWindow(100,100);
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

void CDisplayDlg::OnDestroy() 
{
	CDialog::OnDestroy();
	
	// TODO: Add your message handler code here
	
}

void CDisplayDlg::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	
	CDialog::OnClose();
}

void CDisplayDlg::OnSize(UINT nType, int cx, int cy) 
{
	CDialog::OnSize(nType, cx, cy);
	
	// TODO: Add your message handler code here
//	ResizeWindow(cx,cy);
}

void CDisplayDlg::Update()
{

}

void CDisplayDlg::ResizeWindow(int cx, int cy)
{
	m_CImage.SetWindowPos(NULL,0,0,cx,cy,SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOMOVE);

	CRect MMRect;
	MMRect.SetRect(0,0,cx,cy);
   	CalcWindowRect(&MMRect);   
	SetWindowPos(NULL,0,0,MMRect.right,MMRect.bottom,SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOMOVE);		
	InvalidateRect(NULL,FALSE);	
}

BOOL CDisplayDlg::DestroyWindow() 
{
	// TODO: Add your specialized code here and/or call the base class
	
	return CDialog::DestroyWindow();
}

void CDisplayDlg::OnPaint() 
{
	CPaintDC dc(this); // device context for painting
	
	// TODO: Add your message handler code here
	
	// Do not call CDialog::OnPaint() for painting messages
	if (m_pDisplay)
	{
		m_pDisplay->Draw();
	}
}
