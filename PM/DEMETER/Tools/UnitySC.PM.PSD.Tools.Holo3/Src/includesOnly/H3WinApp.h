/*! 
* 	\file    D:\...\Include\H3WinApp.h
* 	\brief   
* 	\version 
* 	\author  E.COLON
* 	\date    17/08/2007
* 	\remarks 
*/ 

#ifndef CH3_WINAPP__INCLUDED_
#define CH3_WINAPP__INCLUDED_

#include "H3AppToolsDecl.h"

/////////////////////////////////////////////////////////////////////////////
// Informations HOLO3
#define DEFAULT_COMPAGNY_NAME			_T("HOLO3")
#define DEFAULT_REGISTRY_KEY			DEFAULT_COMPAGNY_NAME

/////////////////////////////////////////////////////////////////////////////
// Constantes pour le gestionnaire d'erreurs
#define DEFAULT_DEBUG_LEVEL				H3_DEBUG_ERROR+H3_DEBUG_WARNING
#define DEFAULT_ENABLE_HOLO3_ERROR		true
#define DEFAULT_ENABLE_DEBUG_FILE		false

/////////////////////////////////////////////////////////////////////////////
// Constantes pour la gestion des langues
#define DEFAULT_LANGUAGE_FR				0						// Francais
#define DEFAULT_LANGUAGE_EN				1						// Anglais
#define DEFAULT_LANGUAGE_GE				2						// Allemand
#define DEFAULT_LANGUAGE				DEFAULT_LANGUAGE_FR
#define DEFAULT_LANGUAGE_DEF_FILENAME	CString(GetProgramPath()+CString("Language.ini"))

//////////////////////////////////////////////////////////////////////
///
///	\class   CH3WinApp 
///	\brief   Définition de la classe CH3WinApp
///	\author  E.COLON
///	\date    17/08/2007
///	\bug     
///	\remarks 
///
class CH3WinApp : public CWinApp
{
public:
	static void PumpMessages();
	int GetProfileInt(const CString &strSection,const CString &strEntry,const int nDefault);
	bool WriteProfileInt(const CString &strSection,const CString &strEntry,const int nValue);
	float GetProfileFloat(const CString &strSection,const CString &strEntry,const float fDefault);
	bool WriteProfileFloat(const CString &strSection,const CString &strEntry,const float fValue);
	bool GetProfileBool(const CString &strSection,const CString &strEntry,const bool bDefault);
	bool WriteProfileBool(const CString &strSection,const CString &strEntry,const bool bValue);
	CString GetProgramName();
	CString GetProgramPath();
	CString GetProgramFile();
	CString GetUserName();
	bool SetLanguage(int nLanguageID);
	long GetLanguage();
	void InitLanguageResource();
	void FreeLanguageResource();
	long GetInitLanguage();
	void InitErrorManager();
	void CloseErrorManager();

protected:
	int m_nLanguage;			///> Langue courante de l'interface (0=>FR,1=>EN,2=>GE)
};

/////////////////////////////////////////////////////////////////////////////
///	\fn      long CH3WinApp::GetInitLanguage()
///	\brief   Identificateur linguistique du prochain demarrage
/// \details Cette fonction retourne l'identificateur linguistique qui sera
///          utilisé au prochain demarrage de l'application. L'identificateur
///          linguistique est fixé à l'aide de la fonction SetLanguage().
/// \retval  long identificateur linguistique
/// \see     InitLanguageResource(),SetLanguage()
///	\author  E.COLON
/// 
inline long CH3WinApp::GetInitLanguage()
{
    try
    {
        return H3GetPrivProfileLong(CString("LanguageSettings"), CString("nLanguage"), DEFAULT_LANGUAGE_DEF_FILENAME);
    }
    catch (const std::exception& ex)
    {
        return DEFAULT_LANGUAGE;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3WinApp::InitLanguageResource()
///	\brief   Chargement de la ressource de langue
/// \details Cette fonction charge la DLL de resource de langue auxiliaire.
///          La ressource de langue est une DLL portant le nom du module
///          binaire courant avec les suffixes suivants :
///            _EN pour la version anglaise
///            _GE pour la version allemande
///          La version linguistique par defaut est la version francaise 
///          La fonction doit être appelee dans l'InitInstance() de chaque
///          module DLL ou application.
///          La fonction SetLanguage() permet de selectionner le module
///          liguistique à utiliser.
/// \see     FreeLanguageResource(),SetLanguage()
///	\author  E.COLON
/// 
inline void CH3WinApp::InitLanguageResource()
{
	m_nLanguage=GetInitLanguage();

	CString strSuffix;
	switch(m_nLanguage)
	{
	case DEFAULT_LANGUAGE_EN:
		strSuffix=_T("_EN");
		break;
	case DEFAULT_LANGUAGE_FR:
	default:
		break;
	}

	if (!strSuffix.IsEmpty())
	{
		CString strFilename=CString(AfxGetAppName())+strSuffix+CString(".dll");
		HINSTANCE dll=LoadLibrary(strFilename);
		if(dll)
			AfxSetResourceHandle(dll);
	}
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3WinApp::FreeLanguageResource()
///	\brief   Libere la ressource de langue
/// \details Cette fonction libere la DLL de resource auxiliaire si elle existe.
///          La fonction doit être appelee dans l'ExitInstance() de chaque
///          module DLL ou application.
/// \see     InitLanguageResource()
///	\author  E.COLON
/// 
inline void CH3WinApp::FreeLanguageResource()
{
	HMODULE hDLL = AfxGetResourceHandle();
	if (hDLL != AfxGetInstanceHandle())
	{
		 AfxSetResourceHandle(AfxGetInstanceHandle());
		 FreeLibrary(hDLL);
	}
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      CString CH3WinApp::GetUserName()
///	\brief   Retourne le nom de login
/// \details Cette fonction retourne le nom de login courant
/// \retval  CString login courant
/// \see     
///	\author  E.COLON
/// 
inline CString CH3WinApp::GetUserName()
{
	char szUserName[256];
	DWORD nSize=sizeof(szUserName);
	::GetUserName(szUserName,&nSize);
	return CString(szUserName);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      CString CH3WinApp::GetProgramFile()
///	\brief   Retourne le chemin et le nom de fichier de l'executable
/// \details Cette fonction retourne le chemin et le nom de fichier du 
///          programme executable
/// \retval  CString chemin et nom de fichier de l'executable
/// \see     GetProgramName(),GetProgramPath()
///	\author  E.COLON
/// 
inline CString CH3WinApp::GetProgramFile()
{
	char szFilename[MAX_PATH];
	::GetModuleFileName(NULL,szFilename,MAX_PATH);
	return CString(szFilename);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      CString CH3WinApp::GetProgramPath()
///	\brief   Retourne le chemin de l'executable
/// \details Cette fonction retourne le chemin du programme executable
/// \retval  CString chemin de l'executable
/// \see     GetProgramName(),GetProgramFile()
///	\author  E.COLON
/// 
inline CString CH3WinApp::GetProgramPath()
{
	CString strDrive,strDir,strName,strExt;
	H3SplitPath(GetProgramFile(),strDrive,strDir,strName,strExt);
	return strDrive+strDir;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      CString CH3WinApp::GetProgramName()
///	\brief   Retourne le nom de l'executable
/// \details Cette fonction retourne le nom du programme executable sans
///          le chemin ni l'extension.
/// \retval  CString nom de l'executable
/// \see     GetProgramPath(),GetProgramFile()
///	\author  E.COLON
/// 
inline CString CH3WinApp::GetProgramName()
{
	CString strDrive,strDir,strName,strExt;
	H3SplitPath(GetProgramFile(),strDrive,strDir,strName,strExt);
	return strName;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      float CH3WinApp::GetProfileFloat(const CString &strSection,const CString &strEntry,const float fDefault)
///	\brief   Retourne le contenu d'une clé de type float depuis la base de registre
/// \details Cette fonction permet d'interroger le contenu d'une clé de type
///          float dans la base de registre.
/// \param   strSection section
/// \param   strEntry nom de la clé
/// \param   fDefault valeur par défaut
/// \retval  float	contenu de la clé ou valeur par défaut
/// \see     WriteProfileInt(),WriteProfileFloat(),WriteProfileBool()
///	\author  E.COLON
/// 
inline float CH3WinApp::GetProfileFloat(const CString &strSection,const CString &strEntry,const float fDefault)
{
	CString strDefault;
	CString strValue;

	strDefault.Format("%g",fDefault);
	strValue=GetProfileString(strSection,strEntry,strDefault);

	return((float)atof(LPCTSTR(strValue)));
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3WinApp::WriteProfileFloat(const CString &strSection,const CString &strEntry,const float fValue)
///	\brief   Enregistre une clé de type float dans la base de registre
/// \details Cette fonction permet d'enregistrer une clé de type float dans
///          la base de registre.
/// \param   strSection section
/// \param   strEntry nom de la clé
/// \param   fValue valeur à enregistrer
/// \retval  true	succes de la fonction
/// \retval  false	echec de la fonction
/// \remark  Pour plus d'explications consulter CWinApp::WriteProfileString()
/// \see     GetProfileFloat(),GetProfileInt(),GetProfileString()
///	\author  E.COLON
/// 
inline bool CH3WinApp::WriteProfileFloat(const CString &strSection,const CString &strEntry,const float fValue)
{
  CString strValue;
  strValue.Format("%lg",fValue);

  return WriteProfileString(strSection,strEntry,strValue) ? true:false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3WinApp::GetProfileBool(const CString &strSection,const CString &strEntry,const bool bDefault)
///	\brief   Retourne le contenu d'une clé de type bool depuis la base de registre
/// \details Cette fonction permet d'interroger le contenu d'une clé de type
///          bool dans la base de registre.
/// \param   strSection section
/// \param   strEntry nom de la clé
/// \param   nDefault valeur par défaut
/// \retval  bool	contenu de la clé ou valeur par défaut
/// \see     WriteProfileInt(),WriteProfileFloat(),WriteProfileBool()
///	\author  E.COLON
/// 
inline bool CH3WinApp::GetProfileBool(const CString &strSection,const CString &strEntry,const bool bDefault)
{
	return GetProfileInt(strSection,strEntry, bDefault ? 1:0) ? true:false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3WinApp::WriteProfileBool(const CString &strSection,const CString &strEntry,const bool bValue)
///	\brief   Enregistre une clé de type bool dans la base de registre
/// \details Cette fonction permet d'enregistrer une clé de type bool dans
///          la base de registre.
/// \param   strSection section
/// \param   strEntry nom de la clé
/// \param   bValue valeur à enregistrer
/// \retval  true	succes de la fonction
/// \retval  false	echec de la fonction
/// \remark  Pour eviter les Warning  C4800 du compilateur preferer cette 
///          fonction à WriteProfileInt pour ecrire des booleens dans la base 
///          de registre. 
/// \see     GetProfileBool()
///	\author  E.COLON
/// 
inline bool CH3WinApp::WriteProfileBool(const CString &strSection,const CString &strEntry,const bool bValue)
{
  return WriteProfileInt(strSection,strEntry,(bValue)?1:0) ? true:false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      int CH3WinApp::GetProfileInt(const CString &strSection,const CString &strEntry,const int nDefault)
///	\brief   Retourne le contenu d'une clé de type int depuis la base de registre
/// \details Cette fonction permet d'interroger le contenu d'une clé de type
///          int dans la base de registre.
/// \param   strSection section
/// \param   strEntry nom de la clé
/// \param   nDefault valeur par défaut
/// \retval  int	contenu de la clé ou valeur par défaut
/// \see     WriteProfileInt(),WriteProfileFloat(),WriteProfileBool()
///	\author  E.COLON
/// 
inline int CH3WinApp::GetProfileInt(const CString &strSection,const CString &strEntry,const int nDefault)
{
	return CWinApp::GetProfileInt(strSection,strEntry,nDefault);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3WinApp::WriteProfileInt(const CString &strSection,const CString &strEntry,const int nValue)
///	\brief   Enregistre une clé de type int dans la base de registre
/// \details Cette fonction permet d'enregistrer une clé de type int dans
///          la base de registre.
/// \param   strSection section
/// \param   strEntry nom de la clé
/// \param   nValue valeur entiere à enregistrer
/// \retval  true	succes de la fonction
/// \retval  false	echec de la fonction
/// \see     GetProfileInt()
///	\author  E.COLON
/// 
inline bool CH3WinApp::WriteProfileInt(const CString &strSection,const CString &strEntry,const int nValue)
{
	return CWinApp::WriteProfileInt(strSection,strEntry,nValue) ? true:false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3WinApp::SetLanguage(int nLanguageID)
///	\brief   Fixe le code linguistique
/// \details Cette fonction fixe le code linguistique a utiliser au prochain
///          démarrage de l'application.
///          La fonction doit être appelée par le programme executable afin
///          d'appliquer le code linguistique à l'ensemble des sous-modules.
///          Pour informer les modules complémentaires (DLLs) de la langue 
///          utilisée l'information de langue est stockée dans le fichier 
///          'Language.ini' localisé dans le même dossier que l'executable
///          Syntaxe du fichier "language.ini":
///            [LanguageSettings]
///            nLanguage=0
/// \param	 nLanguageID code linguistique à utiliser
///            DEFAULT_LANGUAGE_FR=0	français
///            DEFAULT_LANGUAGE_EN=1	anglais
///            DEFAULT_LANGUAGE_GE=0	allemand
/// \retval  true	succes de la fonction
/// \retval  false	echec de la fonction
/// \remark	 Le changement de langue n'est effectif qu'au redemarrage de
///          l'application principale.
/// \see     SetLanguage(),FreeLanguageResource(),InitLanguageResource()
///	\author  E.COLON
/// 
inline bool CH3WinApp::SetLanguage(int nLanguageID)
{
	CString strFunction("SetLanguage");

	switch(nLanguageID)
	{
	case DEFAULT_LANGUAGE_FR:
	case DEFAULT_LANGUAGE_EN: 
		break;

	default:
		H3DisplayError(_H3MODULE,strFunction,"Paramètre 'nLanguageID' invalide");
		return false;
		break;
	}

	H3WritePrivProfileInt(_T("LanguageSettings"),_T("nLanguage"),nLanguageID,DEFAULT_LANGUAGE_DEF_FILENAME);

	return true;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      long CH3WinApp::GetLanguage()
///	\brief   Retourne le code linguistique courant
/// \details Cette fonction retourne le code linguistique courant :
///            DEFAULT_LANGUAGE_FR=0	français
///            DEFAULT_LANGUAGE_EN=1	anglais
///            DEFAULT_LANGUAGE_GE=0	allemand
/// \see     SetLanguage()
///	\author  E.COLON
/// 
inline long CH3WinApp::GetLanguage()
{
	return m_nLanguage;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3WinApp::InitErrorManager() 
///	\brief   Initialisation du gestionnaire d'erreur
/// \details Initialisation du gestionnaire d'erreurs HOLO3. Cette fonction 
///          doit être au demarrage de l'application principale (EXE), et
///          placée typiquement dans la fonction WinApp::InitInstace().
///          Si lors de la derniere utilisation le programme ne s'etait pas
///          termine correctement un message signalant que le fichier log est
///          conservé est affiché. Dans le cas contraire le fichier log est
///          ecrase par la nouvelle instance du programme.
/// \see     CloseErrorManager()
///	\author  E.COLON
/// 
inline void CH3WinApp::InitErrorManager()
{
	CString strSection=_H3MODULE;

	CString strDrive,strDir,strName,strExt;
	H3SplitPath(GetProgramFile(),strDrive,strDir,strName,strExt);
	CString strDefaultDebugFile=strDrive+strDir+strName+CString(".log");

	////////////////////////////////////////////////////////////////////////
	// EC 24/09/04 si le programme s'etait termine normalement précédemment alors
	// on efface l'eventuel fichier de debug, sinon on le renomme
	if (GetProfileBool(strSection,_T("bNormalTermination"),true))
	{
		DeleteFile(strDefaultDebugFile);
	}
	else
	{
		CString strDrive,strDir,strName,strExt;
		H3SplitPath(strDefaultDebugFile,strDrive,strDir,strName,strExt);
		COleDateTime dt=COleDateTime::GetCurrentTime();
		CString s;
		s.Format("%04d%02d%02d%02d%02d%02d",
			dt.GetYear(),dt.GetMonth(),dt.GetDay(),
			dt.GetHour(),dt.GetMinute(),dt.GetSecond());
		CString strFile=strDrive+strDir+strName+s+strExt;
		if (H3FileExist(strDefaultDebugFile))
		{
			rename(LPCTSTR(strDefaultDebugFile),LPCTSTR(strFile));
			H3DisplayWarning(
				CString("Lors de la dernière utilisation, le logiciel ")+GetProgramName()+
				CString (" ne s'est pas arrêté normalement.\n\n"\
				   "Le fichier de debogage a été enregistré sous :\n")+strFile);
		}
	}
	WriteProfileInt(strSection,_T("bNormalTermination"),false);

	H3SetDebugFile(strDefaultDebugFile);

	H3SetDebugLevel(GetProfileInt(strSection,_T("nDebugLevel"),DEFAULT_DEBUG_LEVEL));
	H3EnableError(GetProfileBool(strSection,_T("bH3ErrorPrint"),DEFAULT_ENABLE_HOLO3_ERROR));
} 

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3WinApp::CloseErrorManager() 
///	\brief   Ferme le gestionnaire d'erreur
/// \details Fermeture du gestionnaire d'erreurs HOLO3 demarre par la l'appel
///          a la fonction InitErrorManager(). Cette fonction doit être
///          appelee à la fermerture de l'application. Elle enregistre
///          dans la base de registre le niveau de deboggage courant et
///          elle positionne egalement la variable signalant la fin normale
///          de l'application.
/// \see     InitErrorManager()
///	\author  E.COLON
/// 
inline void CH3WinApp::CloseErrorManager()
{
	CString strSection=_H3MODULE;
	WriteProfileInt(strSection,_T("nDebugLevel"),H3GetDebugLevel());
	WriteProfileBool(strSection,_T("bH3ErrorPrint"),H3IsErrorEnabled());
	WriteProfileBool(strSection,_T("bNormalTermination"),true);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3WinApp ::PumpMessages() 
///	\brief   Traitement des messages 
/// \details Traitement des messages de la file, permet le raffraichissement 
///          des fenetres lors d'operations longues. La fonction doit être 
///          appelee dans la boucle.
///	\author  E.COLON
/// 
inline void CH3WinApp ::PumpMessages() 
{ 
   // Handle dialog messages 
	MSG msg; 
	while(PeekMessage(&msg, NULL, 0, 0, PM_REMOVE)) 
	{ 
		if (!AfxGetApp()->PreTranslateMessage(&msg)) 
		{ 
			::TranslateMessage(&msg); 
			::DispatchMessage(&msg); 
		}            
		AfxGetApp()->OnIdle(0);   // updates user interface 
		AfxGetApp()->OnIdle(1);   // frees temporary objects 
	} 
}


#endif
