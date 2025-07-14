#pragma once

#include "TreatmentInterface.h"

class CTreatHandler
{
	friend interface INanoTopoTreament;

	// ----------- METHODES -----------
public:
	/** Constructeur standard. */
	CTreatHandler();

	/** Destructeur standard. */
	virtual ~CTreatHandler();

	HRESULT		CreateTreatmentDLL(INanoTopoTreament **ppTreat, CString csTreatName);
	HRESULT		CreateTreatmentLocal( INanoTopoTreament **ppTreat, CString csTreatName );

	HRESULT		DestroyTreatment();

	// ----------- ATTRIBUTS -----------
private :
	INanoTopoTreament *		m_pTreat;			
	HINSTANCE				m_hLibrary;				

};

#define LAST_WIN32_ERROR(hr)	{int err = GetLastError(); hr = HRESULT_FROM_WIN32(err);}

