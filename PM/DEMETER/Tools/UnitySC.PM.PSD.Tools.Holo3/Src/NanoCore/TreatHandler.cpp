#include "StdAfx.h"
#include "TreatHandler.h"

// Insert HERE include of Local Treatments
#include "TreatPrepareData0.h"
#include "TreatReconstruct0.h"
#include "TreatReconstruct1.h"
#include "TreatFilter0.h"
#include "TreatGenerateResults0.h"

extern "C" const GUID IID_INanoTreatment = {0xFFFFFFFF,0xC9F3,0x11CF,{0xBF,0xC7,0x44,0x45,0x53,0x54,0x00,0x00}};

////////////////

CTreatHandler::CTreatHandler()
{
	m_pTreat = NULL;
	m_hLibrary = 0;

	GUID id = IID_INanoTreatment;
}

CTreatHandler::~CTreatHandler()
{

}

HRESULT CTreatHandler::CreateTreatmentDLL( INanoTopoTreament **ppTreat, CString csTreatName )
{
	HRESULT hr = S_OK;
	if ( (m_hLibrary = LoadLibrary(csTreatName)) == NULL )
	{
		LAST_WIN32_ERROR(hr);
		LogThis(1,4,Fmt(_T("Impossible de charger la DLL <%s>"),csTreatName));
		return E_FAIL;
	}
	typedef HRESULT ( * LPCREATE_TYPE)(REFIID iid, void **ppvObject);
	LPCREATE_TYPE pCreate;
	pCreate = (LPCREATE_TYPE) GetProcAddress(m_hLibrary, "Create");
	if (pCreate == NULL)
	{
		LAST_WIN32_ERROR(hr);
		LogThis(1,4,Fmt(_T("Imposible de trouver la fonction Create <%s>"), csTreatName));
		return E_FAIL;
	}
	if (FAILED (hr = (pCreate)(IID_INanoTreatment, (void **)  &m_pTreat)))
	{
		LAST_WIN32_ERROR(hr);
		LogThis(1,4,Fmt(_T("Imposible d'executer la fonction Create <%s>"), csTreatName));
		return E_FAIL;
	}

	m_pTreat->m_pHandler = this;
	m_pTreat->SetName(csTreatName);

	*ppTreat = m_pTreat;
	return S_OK;
}

HRESULT CTreatHandler::CreateTreatmentLocal( INanoTopoTreament **ppTreat, CString csTreatName )
{
	HRESULT hr = S_OK;
	CString Cs;

	typedef HRESULT ( * LPCREATE_TYPE)(REFIID iid, void **ppvObject);
	LPCREATE_TYPE pCreate = 0;

	if ( csTreatName.CompareNoCase(_T("TreatPrepareData0")) == 0 )
	{
		pCreate = (LPCREATE_TYPE) &Create_TreatPrepareData0;	
	}
	else if ( csTreatName.CompareNoCase(_T("TreatReconstruct0")) == 0 )
	{
		pCreate = (LPCREATE_TYPE) &Create_TreatReconstruct0;	
	}
	else if ( csTreatName.CompareNoCase(_T("TreatReconstruct1")) == 0 )
	{
		pCreate = (LPCREATE_TYPE) &Create_TreatReconstruct1;	
	}
	else if ( csTreatName.CompareNoCase(_T("TreatFilter0")) == 0 )
	{
		pCreate = (LPCREATE_TYPE) &Create_TreatFilter0;	
	}
	else if ( csTreatName.CompareNoCase(_T("TreatGenerateResults0")) == 0 )
	{
		pCreate = (LPCREATE_TYPE) &Create_TreatGenerateResults0;	
	}

	if (pCreate == NULL)
	{
		LAST_WIN32_ERROR(hr);
		LogThis(1,4,Fmt(_T("Imposible de trouver la fonction Create Local <%s>"), csTreatName));
		return E_FAIL;
	}
	if (FAILED (hr = (pCreate)(IID_INanoTreatment, (void **)  &m_pTreat)))
	{
		LAST_WIN32_ERROR(hr);
		LogThis(1,4,Fmt(_T("Imposible d'executer la fonction Create Local <%s>"), csTreatName));
		return E_FAIL;
	}

	m_pTreat->m_pHandler = this;
	m_pTreat->SetName(csTreatName);

	*ppTreat = m_pTreat;
	return S_OK;
}

HRESULT CTreatHandler::DestroyTreatment()
{
	if (m_pTreat != nullptr)
	{
		m_pTreat->Release();
		m_pTreat = nullptr;
	}

	FreeLibrary(m_hLibrary);
	m_hLibrary = 0;
	return S_OK;
}
