// TestTreatment.h : fichier d'en-tête principal pour la DLL TestTreatment
//

#pragma once
 
#include "TreatmentInterface.h"

#ifdef NT_TEST_EXPORTS
	#define TEST_DLL  __declspec(dllexport)
#else
	#define TEST_DLL  __declspec(dllimport)
#endif

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux


// CTestTreatmentApp
// Consultez TestTreatment.cpp pour l'implémentation de cette classe
//

class CTestTreatmentApp : public CWinApp
{
public:
	CTestTreatmentApp();

// Substitutions
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};


class TEST_DLL CTestA : public INanoTopoTreament
{

	//
	// Methods.
	//
public:

	CTestA();

	virtual ~CTestA();

	HRESULT STDMETHODCALLTYPE	QueryInterface(REFIID iid, void **ppvObject);

	ULONG STDMETHODCALLTYPE		AddRef(void);

	ULONG STDMETHODCALLTYPE		Release(void);

	virtual bool Init(tmapTreatInitPrm& p_pPrmMap);

	virtual bool Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm)
	{
		AfxMessageBox(m_cs);
		LogThis(1,1,_T("J'ai cliquer sur OK !!!!"));
		return true;
	}

public:
	CString m_cs;
	int* m_pAr;

};

extern "C" TEST_DLL HRESULT Create(REFIID iid, void **ppvObject)
{
	CTestA *pObj = new CTestA();
	if (pObj==0) 
		return E_OUTOFMEMORY;
	return pObj->QueryInterface(iid, ppvObject);
}


