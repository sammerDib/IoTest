// H3Mire.h: interface for the CH3Mire class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3MIRE_H__859677F2_9116_479B_B9B0_979C11D41258__INCLUDED_)
#define AFX_H3MIRE_H__859677F2_9116_479B_B9B0_979C11D41258__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "h3array2d.h"

#define XML_FILE 0
#if XML_FILE
	#include "H3XMLFile.h"
#endif

class CH3Mire  
{
public:
	bool LoadMire(CString strFile);
	CH3Mire();
	virtual ~CH3Mire();

#if XML_FILE
	bool SaveSettings(H3XMLFile* file,CString strSection=CString(""));
	bool LoadSettings(H3XMLFile* file,CString strSection=CString(""));
#else
	bool SaveSettings(CString strFileName,CString strSection);
	bool LoadSettings(CString strFileName,CString strSection);
#endif

	H3_ARRAY2D_FLT64 GetMetric()const;
	int GetLi()const;
	int GetCo()const;
	int GetNbTarget()const;
	static int m_iLiIntersection;
	static int m_iCoIntersection;

	float GetMireStepX()const;
	float GetMireStepY()const;
	static float m_fMireStepX;
	static float m_fMireStepY;

	CString GetFileMire() {return m_strFileMire;};
private:
	H3_ARRAY2D_FLT64 m_aMetric;
	CString m_strFileMire;
#if XML_FILE
	H3XMLFile* file;
#endif
};

#endif // !defined(AFX_H3MIRE_H__859677F2_9116_479B_B9B0_979C11D41258__INCLUDED_)
