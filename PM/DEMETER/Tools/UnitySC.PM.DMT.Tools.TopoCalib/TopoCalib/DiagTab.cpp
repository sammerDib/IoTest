
/* ****************** INCLUDES ******************* */
#include "stdafx.h"
#include "DiagTab.h"


/* ****************** METHODES ******************* */

CDiagTab::CDiagTab()
{
	m_nNbItem = 0;
	m_CurDlg = -1;
}

CDiagTab::~CDiagTab()
{
}


BEGIN_MESSAGE_MAP(CDiagTab, CTabCtrl)
	//{{AFX_MSG_MAP(COnglet)
	ON_NOTIFY_REFLECT(NM_CLICK, OnClick)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// COnglet message handlers

int CDiagTab::InsertItem(int nItem, CDialog *pDlg, CString sCaption)
{
	TC_ITEM TabCtrlItem;

	if ((nItem > m_nNbItem) || (m_nNbItem >= ONG_NBMAX))
		return -1;

	TabCtrlItem.mask = TCIF_TEXT;
	TabCtrlItem.pszText = (char *) ((LPCTSTR) sCaption);
	TabCtrlItem.cchTextMax = sCaption.GetLength();
	if ( CTabCtrl::InsertItem(nItem, &TabCtrlItem) < 0)
		return -1;

	Hide();
	for (int k = m_nNbItem-1; k >= nItem; k--)
		TabDlg[k+1] = TabDlg[k];
	TabDlg[nItem] = pDlg;
	m_nNbItem++;
	SetCurSel();
	return (nItem);
}

void CDiagTab::Hide()
{
	if (m_CurDlg >= 0)
	{
		TabDlg[m_CurDlg]->ShowWindow(SW_HIDE);
		m_CurDlg = -1;
	}
}

int CDiagTab::SetCurSel(int nItem)
{
	int previtem;
	Hide();

	if (nItem == -1)
		nItem = GetCurSel();
	if (nItem == -1)
		nItem = 0;
	previtem = CTabCtrl::SetCurSel(nItem);
	m_CurDlg = GetCurSel();

	if (m_CurDlg >= 0)
	{
		CRect rect;
		GetClientRect(&rect);
		AdjustRect(FALSE, &rect);

		TabDlg[m_CurDlg]->MoveWindow(rect, FALSE);
		TabDlg[m_CurDlg]->UpdateData(FALSE);		// remise à jour de la boite
		TabDlg[m_CurDlg]->ShowWindow(SW_SHOW);
	}

	return(previtem);
}

void CDiagTab::OnClick(NMHDR* pNMHDR, LRESULT* pResult) 
{
	SetCurSel();
	*pResult = 0;
}

void CDiagTab::Rename(int nItem, CString sCaption)
{
	TC_ITEM TabCtrlItem;

	TabCtrlItem.mask = TCIF_TEXT;
	TabCtrlItem.pszText = (char *) ((LPCTSTR) sCaption);
	TabCtrlItem.cchTextMax = sCaption.GetLength();
	SetItem(nItem, &TabCtrlItem);
	SetCurSel();
}

BOOL CDiagTab::DeleteItem(int nItem)
{
	if (nItem >= m_nNbItem)
		return FALSE;

	if ( ! CTabCtrl::DeleteItem(nItem) )
		return FALSE;

	Hide();
	for (int k =nItem; k < m_nNbItem-1; k++)
		TabDlg[k] = TabDlg[k+1];
	m_nNbItem--;
	SetCurSel();
	return TRUE;
}
