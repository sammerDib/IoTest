// H3Process.h: interface for the CH3Process class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3PROCESS_H__4CDC551D_698E_48DC_A0F5_1385D02A72A0__INCLUDED_)
#define AFX_H3PROCESS_H__4CDC551D_698E_48DC_A0F5_1385D02A72A0__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//#ifdef AFX_H3APPTOOLS_H__5BCF1970_3975_4317_A596_48774862FCFC__INCLUDED_
//	#define H3PROGRESS_EXPORT_DECL __declspec(dllexport)
//#else
//	#define H3PROGRESS_EXPORT_DECL __declspec(dllimport)
//#endif  
#define H3PROGRESS_EXPORT_DECL __declspec(dllimport)

typedef enum tagH3_PROCESS_STATUS { 
	H3_PROCESS_DONE	= 0,
	H3_PROCESS_STARTED = 1,
	H3_PROCESS_CANCELED	= 2,
	H3_PROCESS_ERROR = 3
} H3_PROCESS_STATUS;

class H3PROGRESS_EXPORT_DECL CH3Process  
{
public:
	void Set(bool bView, H3_PROCESS_STATUS nStatus);
	void Set(long nPos, const CString &strMsg);
	void Set(bool bView,H3_PROCESS_STATUS nStatus,long nPos,const CString &strMsg);
	bool IsCancelRequested();
	void SetMessage(const CString &strMsg);
	H3_PROCESS_STATUS GetStatus();
	void SetStatus(H3_PROCESS_STATUS status);
	void SetTitle(const CString &strTitle);
	void SetStep(int nStep);
	void SetPos(int nPos);
	void SetRange(int nLower, int nUpper);
	void SetView(bool bView);
	CH3Process();
	virtual ~CH3Process();

protected:
	bool m_bDone;
	bool m_bStarted;
	CString m_strTitle;
	CString m_strStatus;
	long m_nProgress;
	void *m_pProgressDlg;
	H3_PROCESS_STATUS m_nStatus;
};

#endif // !defined(AFX_H3PROCESS_H__4CDC551D_698E_48DC_A0F5_1385D02A72A0__INCLUDED_)
