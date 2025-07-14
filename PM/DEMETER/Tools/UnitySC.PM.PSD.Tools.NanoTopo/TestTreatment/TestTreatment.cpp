// TestTreatment.cpp : définit les routines d'initialisation pour la DLL.
//

#include "stdafx.h"
#include "TestTreatment.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: si cette DLL est liée dynamiquement aux DLL MFC,
//		toute fonction exportée de cette DLL qui appelle
//		MFC doit avoir la macro AFX_MANAGE_STATE ajoutée au
//		tout début de la fonction.
//
//		Par exemple :
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// corps de fonction normal ici
//		}
//
//		Il est très important que cette macro se trouve dans chaque
//		fonction, avant tout appel à MFC. Cela signifie qu'elle
//		doit être la première instruction dans la 
//		fonction, avant toute déclaration de variable objet
//		dans la mesure où leurs constructeurs peuvent générer des appels à la DLL
//		MFC.
//
//		Consultez les notes techniques MFC 33 et 58 pour plus de
//		détails.
//

// CTestTreatmentApp

BEGIN_MESSAGE_MAP(CTestTreatmentApp, CWinApp)
END_MESSAGE_MAP()


// construction CTestTreatmentApp

CTestTreatmentApp::CTestTreatmentApp()
{
	// TODO: ajoutez ici du code de construction,
	// Placez toutes les initialisations significatives dans InitInstance
}


// Seul et unique objet CTestTreatmentApp

CTestTreatmentApp theApp;


// initialisation de CTestTreatmentApp

BOOL CTestTreatmentApp::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

/////////////////*
/////////////////*
/////////////////*/

CTestA::CTestA()
{
	m_pAr = new int[100]; 
	memset((void*)m_pAr,0,sizeof(int));
}

CTestA::~CTestA()
{
	delete[] m_pAr; 
	m_pAr = 0;
}

bool CTestA::Init(tmapTreatInitPrm& p_pPrmMap)
{
	m_cs = _T("testA"); 
	return true;
}

HRESULT CTestA::QueryInterface( REFIID iid, void **ppvObject )
{
	*ppvObject=0;   // Toujours initialiser le pointeur renvoyé.
	if (iid==IID_IUnknown)
		*reinterpret_cast<IUnknown **>(ppvObject)= static_cast<IUnknown *>(this);
	else 
		if (iid==IID_INanoTreatment)
			*reinterpret_cast<INanoTopoTreament **>(ppvObject)= static_cast<INanoTopoTreament *>(this);
	if (*ppvObject==0) 
		return E_NOINTERFACE;
	AddRef();           // On incrémente le compteur de références.
	return NOERROR;
}

ULONG CTestA::AddRef( void )
{
	m_ulRefCount++;
	return m_ulRefCount;
}

ULONG CTestA::Release( void )
{
	m_ulRefCount--;
	if (m_ulRefCount!=0) 
		return m_ulRefCount;
	delete this;     // Destruction de l'objet.
	return 0;        // Ne pas renvoyer m_ulRefCount (il n'existe plus).
}
