// Original source found here: http://codeguru.earthweb.com/dialog/folder_dialog.shtml
// With a mod here: http://www.codeguru.com/mfc/comments/10116.shtml
#if !defined(AFX_MYFD_H__F9CB9441_F91B_11D1_8610_0040055C08D9__INCLUDED_)
#define AFX_MYFD_H__F9CB9441_F91B_11D1_8610_0040055C08D9__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000
// MyFD.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// CFolderDialog dialog

static const TCHAR *FOLDER_FILTER = _T(" | ||");

class CFolderDialog : public CFileDialog {
  DECLARE_DYNAMIC(CFolderDialog)

  CString   m_pPath;
  BOOL      m_bModeSelect;
  BOOL      m_bShowFiles;
  TCHAR     *m_pStr;
  UINT      m_nMaxFile;
  CString	m_strTitle;
  
  static WNDPROC  m_wndProc;
  void            SetEdt1(CString inStr);  
  void            GetEdt1(CString &inStr);  
  
public:
  CFolderDialog(BOOL bSelectDlg=TRUE, LPCTSTR lpszPath=nullptr, LPCTSTR lpszFileFilter=FOLDER_FILTER, 
    DWORD dwFlags=OFN_PATHMUSTEXIST | OFN_HIDEREADONLY, CString strTitle = _T("Répertoire"));

  virtual INT_PTR DoModal();

  CString       GetSelPath()      { return m_pPath; }
  CString       GetParentDir()    { return m_pPath.Left(m_pPath.ReverseFind(_T('\\'))); }
  BOOL          DidSelectFolder() { return GetFileAttributes(m_pPath) & FILE_ATTRIBUTE_DIRECTORY; }
  
protected:
  virtual void  OnInitDone();
  virtual void  OnFolderChange(); 
  virtual void  OnFileNameChange();
  virtual BOOL  OnFileNameOK();
  
protected:
  //{{AFX_MSG(CFolderDialog)
  virtual BOOL OnInitDialog();
  afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
  //}}AFX_MSG
  DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Developer Studio will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MYFD_H__F9CB9441_F91B_11D1_8610_0040055C08D9__INCLUDED_)
