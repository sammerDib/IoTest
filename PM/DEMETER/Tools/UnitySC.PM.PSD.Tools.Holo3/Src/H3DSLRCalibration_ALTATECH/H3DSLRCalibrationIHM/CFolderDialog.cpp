// Original source found here: http://codeguru.earthweb.com/dialog/folder_dialog.shtml
// With a mod here: http://www.codeguru.com/mfc/comments/10116.shtml

/************************************
  REVISION LOG ENTRY
  Revision By: Mihai Filimon
  Revised on 6/1/98 4:50:35 PM
  Comments: MyFD.cpp : implementation file

  Revised on 11/4/99 4:03 PM
  Bob Sheehan
  Modified to work more like a standard
  CFileDialog

  Revised on 11/10/99 11:20PM
  Bob Sheehan
  removed dependencies on other include files SORRY !!

  Revised 11/27/03 10:43AM (Happy Thanksgiving!)
  lucky760@yahoo.com
  Allow user the choice of selecting files and folders
  or just folders by either passing the file filter
  for the third constructor argument or NULL.
  Also tidied up the formatting a bit and added unicode
  compatibility (using _T and such).
  Added more flexibility and functions to retrieve the
  selected path.

  Revised 8/20/04 1:04PM
  lucky760@yahoo.com
  Updated OnFolderChange() to properly
  allow users to use OFN_ALLOWMULTISELECT
 ************************************/

#include "stdafx.h"
#include "CFolderDialog.h"

#include <string>
using namespace std;

#include <DLGS.H>
#include <WINUSER.H>
#define GWL_WNDPROC         (-4)//in WinUser

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CFolderDialog

IMPLEMENT_DYNAMIC(CFolderDialog, CFileDialog)

WNDPROC CFolderDialog::m_wndProc = NULL;

//
// Function name  : CFolderDialog::CFolderDialog
// Description    : Constructor
// Argument       : BOOL bSelectDlg - TRUE to select/open item(s) FALSE to modify
//                : LPCTSTR lpszPath - the initial path
//                : LPCTSTR lpszFileFilter - Filter to use for file selection or
//                                           FOLDER_FILTER if only folders are to be selected
//                : DWORD dwFlags - same as CFileDialog flags
//
CFolderDialog::CFolderDialog(BOOL bSelectDlg, LPCTSTR lpszPath, LPCTSTR lpszFileFilter, 
							 DWORD dwFlags, CString strTitle)
: m_bModeSelect(bSelectDlg), m_pStr(nullptr),
  CFileDialog(TRUE, NULL, NULL, dwFlags | OFN_EXPLORER, lpszFileFilter)
{
  CString pPath;

  if ((lpszPath == nullptr) || *lpszPath == 0) {
    TCHAR tmpPath[MAX_PATH + 1];
    GetCurrentDirectory(MAX_PATH, tmpPath);
    pPath = tmpPath;
  }
  else
    pPath = lpszPath;
  
  if (GetFileAttributes(pPath) & FILE_ATTRIBUTE_DIRECTORY)
    m_pPath = pPath;
  else
    m_pPath = pPath.Left(pPath.ReverseFind(_T('\\')));

  m_ofn.lpstrInitialDir = m_pPath;
  m_strTitle = strTitle;

  m_bShowFiles = _tcsicmp(lpszFileFilter, FOLDER_FILTER) != 0;
}


BEGIN_MESSAGE_MAP(CFolderDialog, CFileDialog)
  //{{AFX_MSG_MAP(CFolderDialog)
  ON_WM_CREATE()
  //}}AFX_MSG_MAP
END_MESSAGE_MAP()

//
// Function name  : WindowProcNew
// Description    : Call this function when user navigate into CFileDialog.
// Return type    : LRESULT
// Argument       : HWND hwnd
// Argument       : UINT message
// Argument       : WPARAM wParam
// Argument       : LPARAM lParam
//
static LRESULT CALLBACK WindowProcNew(HWND hwnd,UINT message, WPARAM wParam, LPARAM lParam) {  
  if (message ==  WM_COMMAND)  {
    if (HIWORD(wParam) == BN_CLICKED)  {
      if (LOWORD(wParam) == IDOK) {
        if (CFileDialog* pDlg = (CFileDialog*)CWnd::FromHandle(hwnd)) {
          pDlg->EndDialog(IDOK);
          return NULL;
        }
      }
    }
  }

  return CallWindowProc(CFolderDialog::m_wndProc, hwnd, message, wParam, lParam);
}

//
// If OFN_ALLOWMULTISELECT is used we need to really hack this thing to pieces
//

INT_PTR CFolderDialog::DoModal() {
  m_pStr = nullptr;
  m_nMaxFile = 0;

  if (m_ofn.Flags & OFN_ALLOWMULTISELECT) {
    m_pStr      = m_ofn.lpstrFile;
    m_nMaxFile  = m_ofn.nMaxFile;

    m_ofn.lpstrFile   = m_szFileName;
    *m_ofn.lpstrFile  = 0;
    m_ofn.nMaxFile    = sizeof(m_szFileName);
  }

INT_PTR nRet = CFileDialog::DoModal();

  if (m_ofn.Flags & OFN_ALLOWMULTISELECT) {
    m_ofn.lpstrFile = m_pStr;
    m_ofn.nMaxFile  = m_nMaxFile;
  }

  return nRet;
}

//
// This is called any time the list selection changes
void CFolderDialog::OnFolderChange() {
  TCHAR path[MAX_PATH];
  GetCurrentDirectory(MAX_PATH, path); 
  m_pPath = path;

  CWnd* pParent = GetParent()->GetDlgItem(lst2); 

  // get the list control 
  CListCtrl *pList = (CListCtrl*)pParent->GetDlgItem(1); 

  // currently selected item 
  int pos = pList->GetNextItem(-1, LVNI_ALL | LVNI_SELECTED); 

  TCHAR *pMultiNext = m_pStr;//m_ofn.lpstrFile;
  UINT nTotalLen = 0;
  CString strSelectedItems;
  int nItems = 0;

  CString selection;

  while (pos != -1) { 
    // create the full path... 
    selection = pList->GetItemText(pos, 0);
    strSelectedItems += _T("\"");
    strSelectedItems += selection + _T("\" ");    

    CString testStr = path;
    if (testStr.GetAt(testStr.GetLength() - 1) != _T('\\'))
      testStr += _T("\\");
    testStr += selection; 

    m_pPath = testStr;

    if (m_ofn.Flags & OFN_ALLOWMULTISELECT) {
      int nPathLen = nItems == 0 ? _tcslen(path) : 0;

      int nAddLen = nPathLen + 1 + selection.GetLength() + 1;
      nTotalLen += nAddLen + 1;

      if (nTotalLen > m_nMaxFile) {
        MessageBox(_T("Selected length is beyond capacity"), _T("Insufficient Memory"), MB_ICONEXCLAMATION);
        EndDialog(IDCANCEL);
      }

      // the first item in the list is the path of our current directory
      if (nItems == 0) {
        _tcscpy(pMultiNext, path);
        pMultiNext += nPathLen + 1;
      }

      // then each selected item follows with only its name (not path)
      // the final item has 2 NULLs terminating it
      _tcscpy(pMultiNext, selection);
      pMultiNext += selection.GetLength() + 1;
      *pMultiNext = NULL;

      ++nItems;

      pos = pList->GetNextItem(pos, LVNI_ALL | LVNI_SELECTED);
    }
    else
      break;
  }

  if (m_ofn.Flags & OFN_ALLOWMULTISELECT) {
    if (nItems > 1)
      GetParent()->GetDlgItem(edt1)->SetWindowText(strSelectedItems);
    else {
      _tcscpy(m_pStr, m_pPath);
      *(m_pStr + m_pPath.GetLength() + 1) = NULL;
      GetParent()->GetDlgItem(edt1)->SetWindowText(selection);
    }
  }
  else
    GetParent()->GetDlgItem(edt1)->SetWindowText(m_pPath);
} 

void CFolderDialog::OnFileNameChange() {
  OnFolderChange();
} 

// Function name  : CFolderDialog::OnInitDone
// Description    : For update the wiew of CFileDialog
void CFolderDialog::OnInitDone()
{
  if (!m_bShowFiles) {
    HideControl(cmb1);
    HideControl(stc2);
  }

  CWnd* pFD = GetParent();
  CRect rectCancel; 
  pFD->GetDlgItem(IDCANCEL)->GetWindowRect(rectCancel);
  pFD->ScreenToClient(rectCancel);

  CString titleStr;
  if (!m_bModeSelect)
    titleStr = _T("Ouvrir ...");
  else
    titleStr = _T("Selectionner");

  SetControlText(IDOK, titleStr);

//MT
//   if (!m_bModeSelect) {
//     if (m_ofn.Flags & OFN_ALLOWMULTISELECT)
//         titleStr = _T("Open Folders");
//       else
//       titleStr = _T("Open Folder");
//   }
//   else {
//     if (m_bShowFiles) {      
//       if (m_ofn.Flags & OFN_ALLOWMULTISELECT)
//         titleStr = _T("Select Files or Folders");
//       else
//         titleStr = _T("Select File or Folder");
//     }
//     else {
//       if (m_ofn.Flags & OFN_ALLOWMULTISELECT)
//         titleStr = _T("Select Folders");
//       else
//         titleStr = _T("Select Folder");
//     }
//   }
// 
//   pFD->SetWindowText(titleStr);
   pFD->SetWindowText(m_strTitle);
//MT
  
  if (m_bShowFiles) {
    // if selecting files as well, don't label the file "folder"
    titleStr = _T("Fichier :");

    // adjust the label and edit sizes accordingly
    CRect rect;
    pFD->GetDlgItem(stc3)->GetWindowRect(rect);
    pFD->ScreenToClient(rect);
    pFD->GetDlgItem(stc3)->SetWindowPos(0, 0, 0, rect.Width() - 20, rect.Height(), SWP_NOMOVE | SWP_NOZORDER);

    pFD->GetDlgItem(edt1)->GetWindowRect(rect);
    pFD->GetDlgItem(edt1)->EnableWindow(false);

    pFD->ScreenToClient(rect);
    pFD->GetDlgItem(edt1)->SetWindowPos(0, rect.left - 10, rect.top, 0, 0, SWP_NOSIZE | SWP_NOZORDER);      
  }
  else
    titleStr = _T("Répertoire :");

  pFD->GetDlgItem(stc3)->SetWindowText(titleStr);
  pFD->GetDlgItem(edt1)->SetWindowText(m_pPath);
  pFD->GetDlgItem(edt1)->EnableWindow(false);;

  m_wndProc = (WNDPROC)SetWindowLongPtr(pFD->m_hWnd, GWL_WNDPROC, (LONG_PTR)WindowProcNew);
}

void CFolderDialog::SetEdt1(CString inStr) {
  GetParent()->GetDlgItem(edt1)->SetWindowText(inStr);
  m_pPath = inStr;
}

void CFolderDialog::GetEdt1(CString &inStr) {
  GetParent()->GetDlgItem(edt1)->GetWindowText(inStr);
}

BOOL CFolderDialog::OnInitDialog() {
  CFileDialog::OnInitDialog();
  
  GetParent()->GetDlgItem(edt1)->SetWindowText(m_pPath);
  
  return TRUE;  // return TRUE unless you set the focus to a control
                // EXCEPTION: OCX Property Pages should return FALSE
}

int CFolderDialog::OnCreate(LPCREATESTRUCT lpCreateStruct) {
  if (CFileDialog::OnCreate(lpCreateStruct) == -1)
    return -1;
  
  GetParent()->GetDlgItem(edt1)->SetWindowText(m_pPath);
  
  return 0;
}

BOOL CFolderDialog::OnFileNameOK() {
  CString str;
  GetEdt1(str);
  if (str.Find(_T('*')) != string::npos || str.Find(_T('?')) != string::npos)
    return 1;
  return 0;
}
