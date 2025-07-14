
// NanoExpertCtrl.cpp : Définit les comportements de classe pour l'application.
//

#include "stdafx.h"
#include "NanoExpertCtrl.h"
#include "NanoExpertCtrlDlg.h"
#include "ExpertPassword.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CNanoExpertCtrlApp

BEGIN_MESSAGE_MAP(CNanoExpertCtrlApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// construction CNanoExpertCtrlApp

CNanoExpertCtrlApp::CNanoExpertCtrlApp()
{
	// prend en charge le Gestionnaire de redémarrage
	m_dwRestartManagerSupportFlags = AFX_RESTART_MANAGER_SUPPORT_RESTART;

	// TODO: ajoutez ici du code de construction,
	// Placez toutes les initialisations significatives dans InitInstance
}


// Seul et unique objet CNanoExpertCtrlApp

CNanoExpertCtrlApp theApp;


// initialisation de CNanoExpertCtrlApp

BOOL CNanoExpertCtrlApp::InitInstance()
{
	// InitCommonControlsEx() est requis sur Windows XP si le manifeste de l'application
	// spécifie l'utilisation de ComCtl32.dll version 6 ou ultérieure pour activer les
	// styles visuels.  Dans le cas contraire, la création de fenêtres échouera.
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// À définir pour inclure toutes les classes de contrôles communs à utiliser
	// dans votre application.
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();


	AfxEnableControlContainer();

	// Créer le gestionnaire de shell, si la boîte de dialogue contient
	// des contrôles d'arborescence ou de liste de shell.
	CShellManager *pShellManager = new CShellManager;


	CExpertPassword dlgpass;
	INT_PTR nRespssd = dlgpass.DoModal();
	if (nRespssd  == IDCANCEL)
	{
		AfxMessageBox("Wrong password !!!");
	}
	else if( nRespssd == IDOK)
	{
		// Initialisation standard
		// Si vous n'utilisez pas ces fonctionnalités et que vous souhaitez réduire la taille
		// de votre exécutable final, vous devez supprimer ci-dessous
		// les routines d'initialisation spécifiques dont vous n'avez pas besoin.
		// Changez la clé de Registre sous laquelle nos paramètres sont enregistrés
		// TODO: modifiez cette chaîne avec des informations appropriées,
		// telles que le nom de votre société ou organisation
		SetRegistryKey(_T("Applications locales générées par AppWizard"));

		CNanoExpertCtrlDlg dlg;
		m_pMainWnd = &dlg;
		INT_PTR nResponse = dlg.DoModal();
		if (nResponse == IDOK)
		{
			// TODO: placez ici le code définissant le comportement lorsque la boîte de dialogue est
			//  fermée avec OK
		}
		else if (nResponse == IDCANCEL)
		{
			// TODO: placez ici le code définissant le comportement lorsque la boîte de dialogue est
			//  fermée avec Annuler
		}
	}


	// Supprimer le gestionnaire de shell créé ci-dessus.
	if (pShellManager != NULL)
	{
		delete pShellManager;
	}

	// Lorsque la boîte de dialogue est fermée, retourner FALSE afin de quitter
	//  l'application, plutôt que de démarrer la pompe de messages de l'application.
	return FALSE;
}

