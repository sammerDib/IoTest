#ifndef TREATMENT_INTERFACE
#define TREATMENT_INTERFACE

#ifdef NT_TREATMENT_EXPORTS
	#define NT_TREATMENT_DLL  __declspec(dllexport)
#else
	#define NT_TREATMENT_DLL  __declspec(dllimport)
	#pragma comment (lib , "NanoCore")
#endif

#include <memory>	// std::tr1 extension WinSDK memory for smart pointer

class CTreatHandler;

typedef struct
{
	void*						_p;			// classic pointer ( call classic cast for downcasting (static or dynamic if needed )
	std::tr1::shared_ptr<void>	_sp;		// shared pointer (call std::tr1::static_pointer_cast<T> or std::tr1::dynamic_pointer_cast<T> for downcasting
} tTreatEltPtr;
typedef CMap<CString, LPCTSTR, tTreatEltPtr, tTreatEltPtr&> tmapTreatPrm;

#include <map>	// pour l'export necessité d'utiliser les std::map
typedef std::map<CString,CString> tmapTreatInitPrm;

extern "C" NT_TREATMENT_DLL const GUID FAR IID_INanoTreatment;

#define DBG_SHOWDEBUG_LOG			0x00000001
#define DBG_SHOW_DISPLAY			0x00000002

interface NT_TREATMENT_DLL INanoTopoTreament : public IUnknown
{
	friend class CTreatHandler;

	//
	// Methods.
	//
public:

	INanoTopoTreament()
	{
		m_ulRefCount = 0; 
		m_sName = _T("");
		m_uDbgFlag = 0;
		m_bEmergencyStop = false;
	};

	virtual ~INanoTopoTreament(){};

	virtual bool STDMETHODCALLTYPE  Init(tmapTreatInitPrm& p_pPrmMap, const CString& calibFolder) = 0;

	virtual bool STDMETHODCALLTYPE  Exec(const tmapTreatPrm& p_InputsPrm, tmapTreatPrm& p_OutputsPrm) = 0;

	inline CTreatHandler *	GetHandler()			{ return m_pHandler; }
	inline CString			GetName()	const		{ return m_sName; } 
	inline void				SetName(CString sName)	{ m_sName = sName; }
	inline unsigned int		GetDbgFlag()	const	{ return m_uDbgFlag; } 
	inline void				SetDbgFlag(unsigned int	p_uDbgFlag)	{ m_uDbgFlag = p_uDbgFlag; }
	inline void				EmergencyStop()			{ m_bEmergencyStop = true; }
	inline void				ClearEmergencyStop()	{ m_bEmergencyStop = false; }

	virtual bool GetRegistryFlag(unsigned long& p_ulRegValue, unsigned long p_dwDefaultValue)
	{
		// query for registry data if data doesn't exist value is created with default value 
		p_ulRegValue			= p_dwDefaultValue;
		HKEY hRegbase			= HKEY_CURRENT_USER;
		CString csNanoKey		= _T("Software\\Altatech\\Nanotopo");
		CString csTreatKey		= INanoTopoTreament::GetName();
		csTreatKey				+= "_";
		csTreatKey				+= "Flag";

		HKEY hTreatmentKey = 0;
		DWORD dwValue	= p_dwDefaultValue;  // init with default value
		DWORD dwData	= sizeof(dwValue);
		// OPen registry
		if (RegOpenKeyEx(hRegbase, csNanoKey, 0, KEY_ALL_ACCESS , &hTreatmentKey) == ERROR_SUCCESS) 
		{
			DWORD dwType = 0;
			// query registry data flag
			if (RegQueryValueEx(hTreatmentKey, csTreatKey, 0, &dwType, (BYTE*)(&dwValue), &dwData) != ERROR_SUCCESS)
			{	
				// data value doesnt exist add it with default value
				if (RegSetValueEx(hTreatmentKey, csTreatKey, 0, REG_DWORD, (BYTE *)(&dwValue), dwData) != ERROR_SUCCESS)
				{
					RegCloseKey(hTreatmentKey);
					return false;
				}
			}
			else
			{
				p_ulRegValue =  dwValue;
				ASSERT(dwType == REG_DWORD);
			}
			RegCloseKey(hTreatmentKey);
		}
		else
		{
			//key does not exist create it and insert default value
			if (RegCreateKeyEx(hRegbase, csNanoKey, 0, _T(""), REG_OPTION_NON_VOLATILE, KEY_WRITE, NULL, &hTreatmentKey, NULL) == ERROR_SUCCESS)
			{
				if (RegSetValueEx(hTreatmentKey, csTreatKey, 0, REG_DWORD, (BYTE *)(&dwValue), dwData) != ERROR_SUCCESS)
				{
					RegCloseKey(hTreatmentKey);
					return false;
				}
				RegCloseKey(hTreatmentKey);
			}
			else
				return false;
		}

		return true;
	}

protected :
	CTreatHandler*		m_pHandler;
	CString				m_sName;
	unsigned long		m_ulRefCount;		///< Compte de références sur l'objet
	unsigned int		m_uDbgFlag;
	bool				m_bEmergencyStop;
};

// Helper functions for map treatment manipulation
NT_TREATMENT_DLL inline void AddTreatPrmPtr(tmapTreatPrm& p_Map, LPCSTR p_csKey, void* p_pPtr)
{
	tTreatEltPtr Elt;
	Elt._p = p_pPtr;
	p_Map[p_csKey]=Elt;
}
NT_TREATMENT_DLL inline void AddTreatPrmSharedPtr(tmapTreatPrm& p_Map, LPCSTR p_csKey, std::tr1::shared_ptr<void>& p_pSharedPtr)
{
	tTreatEltPtr Elt;
	Elt._sp = p_pSharedPtr;
	p_Map[p_csKey]=Elt;
}

NT_TREATMENT_DLL inline bool FindTreatPrm(const tmapTreatPrm& p_Map, LPCSTR p_csKey, tTreatEltPtr& p_Elt)
{
	if( ! p_Map.Lookup(p_csKey,p_Elt))
	{
		p_Elt._p = 0;
		return false;
	}
	return true;
}

NT_TREATMENT_DLL inline bool FindTreatPrmPtr(const tmapTreatPrm& p_Map, LPCSTR p_csKey, void* &p_pPtr)
{
	tTreatEltPtr Elt;
	bool bRes = FindTreatPrm(p_Map,p_csKey,Elt);
	p_pPtr = Elt._p;
	return bRes;
}

NT_TREATMENT_DLL inline bool FindTreatPrmSharedPtr(const tmapTreatPrm& p_Map, LPCSTR p_csKey, std::tr1::shared_ptr<void>& p_pSharedPtr)
{
	tTreatEltPtr Elt;
	bool bRes = FindTreatPrm(p_Map,p_csKey,Elt);
	p_pSharedPtr = Elt._sp;
	return bRes;
}

//////////////////////////////////////////////////////////////////////////

NT_TREATMENT_DLL inline void AddTreatInitPrmStr(tmapTreatInitPrm& p_Map, LPCSTR p_csKey, CString& p_csValue)
{
	p_Map[p_csKey]=p_csValue;
}

NT_TREATMENT_DLL inline void AddTreatInitPrmInt(tmapTreatInitPrm& p_Map, LPCSTR p_csKey, int& p_nValue)
{
	CString cs;
	cs.Format(_T("%d"),p_nValue);
	p_Map[p_csKey]=cs;
}

NT_TREATMENT_DLL inline void AddTreatInitPrmDbl(tmapTreatInitPrm& p_Map, LPCSTR p_csKey, double& p_dValue)
{
	CString cs;
	cs.Format(_T("%lf"),p_dValue);
	p_Map[p_csKey]=cs;
}

NT_TREATMENT_DLL inline void AddTreatInitPrmFlt(tmapTreatInitPrm& p_Map, LPCSTR p_csKey, float& p_fValue)
{
	CString cs;
	cs.Format(_T("%f"),p_fValue);
	p_Map[p_csKey]=cs;
}

NT_TREATMENT_DLL inline bool FindTreatInitPrmStr(const tmapTreatInitPrm& p_Map, LPCSTR p_csKey, CString& p_csValue)
{
	tmapTreatInitPrm::const_iterator it = p_Map.find(p_csKey);
	if( it == p_Map.end())
	{
		p_csValue = _T("");
		return false;
	}
	p_csValue = (*it).second;
	return true;
}

NT_TREATMENT_DLL inline bool FindTreatInitPrmInt(const tmapTreatInitPrm& p_Map, LPCSTR p_csKey, int& p_nValue)
{
	CString csValue;
	if( ! FindTreatInitPrmStr(p_Map,p_csKey,csValue))
	{
		p_nValue = 0;
		return false;
	}
	if(csValue.IsEmpty())
	{
		p_nValue = 0;
		return false;
	}
	
	p_nValue = atoi((LPCSTR)csValue);
	return true;
}

NT_TREATMENT_DLL inline bool FindTreatInitPrmDbl(const tmapTreatInitPrm& p_Map, LPCSTR p_csKey, double& p_dValue)
{
	CString csValue;
	if( ! FindTreatInitPrmStr(p_Map,p_csKey,csValue))
	{
		p_dValue = 0.0;
		return false;
	}
	if(csValue.IsEmpty())
	{
		p_dValue = 0.0;
		return false;
	}

	p_dValue = atof((LPCSTR)csValue);
	return true;
}

NT_TREATMENT_DLL inline bool FindTreatInitPrmFlt(const tmapTreatInitPrm& p_Map, LPCSTR p_csKey, float& p_fValue)
{
	CString csValue;
	if( ! FindTreatInitPrmStr(p_Map,p_csKey,csValue))
	{
		p_fValue = 0.0f;
		return false;
	}
	if(csValue.IsEmpty())
	{
		p_fValue = 0.0f;
		return false;
	}

	p_fValue = static_cast<float>( atof((LPCSTR)csValue) );
	return true;
}
#endif