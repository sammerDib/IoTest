/// 
///	\file    H3AppToolsMisc.cpp
///	\brief   Fonctions diverses exportees par la librairie H3AppTools
///	\version 1.0.5.0
///	\author  E.COLON
///	\date    04/09/2007
///	\remarks 
/// 

#include "stdafx.h"
#include "H3AppTools.h"
#include "H3AppToolsDecl.h"
#include <direct.h>
#include <io.h>
#include <fcntl.h>
#include <WinVer.h>
#include <math.h>
#include "tlhelp32.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// Macros
#define _LOG_TYPE_MESSAGE	H3LoadString(IDS_MESSAGE)
#define _LOG_TYPE_INFO		H3LoadString(IDS_INFORMATION)
#define _LOG_TYPE_WARNING	H3LoadString(IDS_WARNING)
#define _LOG_TYPE_ERROR		H3LoadString(IDS_ERROR)
#define _LOG_TYPE_UNDEFINED	H3LoadString(IDS_UNDEFINED)

/////////////////////////////////////////////////////////////////////////////
// Auteur : EC
// Date : 09/08/07
#define _H3_MODULE_ADD_ON \
	CString strAppName=MyGetApplicationName();	\
	CString strModule=AfxGetAppName();	\
	AFX_MANAGE_STATE(AfxGetStaticModuleState());	\
	CString strModuleExt;	\
	if (!strSubModule.IsEmpty())	\
		strModuleExt=strModule+CString(":")+strSubModule;	\

/////////////////////////////////////////////////////////////////////////////
// Variables globales
bool g_bErrorDisplay;		///< Autorisation d'affichage des messages d'erreur
bool g_bWarningDisplay;		///< Autorisation d'affichage des messages d'attention
bool g_bInfoDisplay;		///< Autorisation d'affichage des messages d'information		
CString g_strDebugFile;		///< Chemin et nom du fichier de trace courant (logfile)
long g_nDebugLevel;			///< Niveau de debogage


/////////////////////////////////////////////////////////////////////////////
// Decalarations de fonctions locales
void DebugMessage(const CString &strModule,const CString &strSubModule,const CString &strFunction,const CString &strType,const CString &strMessage);
long DisplayMessage(const CString &strModule,const CString &strFunction,const CString &strTitle,const CString &strMessage,UINT nType);

/////////////////////////////////////////////////////////////////////////////
// Identification du module
static CString strModule("H3AppToolsMisc");

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP CString H3LoadString(UINT nID)
///	\brief   Retourne la chaine de caracteres identifié par nID depuis la table des ressources
/// \param	 nID : l'identifiant de la resource à charger
///	\retval	 CString 
///
H3APPTOOLS_EXP CString H3LoadString(UINT nID)
{
	CString str;
	if (!str.LoadString(nID))
	{
		str.Format(_T("STRING_ID:%d"), nID);
	}
	return str;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3EnableError(bool bEnable)
///	\brief   Activation de l'affichage des messages d'erreur.
/// \param	 bEnable activation ou desactivation
///	\retval	 bool ancien etat
///  \see	 H3IsErrorEnabled(), H3EnableWarning(), H3EnableInfo()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP bool H3EnableError(bool bEnable)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	bool bOldState=g_bErrorDisplay;
	g_bErrorDisplay=bEnable;
	return bOldState;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3IsErrorEnabled()
///	\brief   Retourne l'etat d'affichage des messages d'erreur.
///	\retval	 true les messages sont affiches.
///	\retval	 false les messages ne sont pas affiches.
/// \see	 H3EnableError(), H3IsWarningEnabled(), H3IsInfoEnabled()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP bool H3IsErrorEnabled()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return g_bErrorDisplay;
}

/////////////////////////////////////////////////////////////////////////////
CString MyGetApplicationName()
{
	char szFilename[_MAX_PATH*2];
	::GetModuleFileName(NULL,szFilename,sizeof(szFilename));
	CString strDrive,strDir,strName,strExt;
	H3SplitPath(CString(szFilename),strDrive,strDir,strName,strExt);
	return strName+strExt;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3DisplayError(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
///	\brief   Affichage d'un message d'erreur.
/// \details Cette fenetre affiche un message d'erreur ainsi que les noms de
///          la DLL, sous-module et fonction ayant genere le message.
///          Si le niveau de debogage le permet, le message est egalement 
///			 trace dans le fichier de debogage.
/// \param	 strSubModule Nom du sous-module dans lequel s'est produit l'erreur
///			 Typiquement il s'agit du nom du fichier source contenant l'appel.
/// \param	 strFunction Nom de la fonction ayant genere l'erreur.
/// \param	 strMessage Message d'erreur.
///	\retval	 IDOK le message a ete affiche et acquite par l'operateur
///	\retval	 IDCANCEL le message n'a pas ete affiche 
/// \see	 H3DisplayWarning(), H3DisplayInfo(), H3DisplayMessage()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP long H3DisplayError(const CString &strSubModule,const CString &strFunction,const CString &strMessage,bool bDisplaySysError)
{  
	_H3_MODULE_ADD_ON;  

	CString strMsg=strMessage;

	if (bDisplaySysError)
	{
		if (errno>0)
		{
			if (strMsg.IsEmpty())
			{
				strMsg=strerror(errno);
			}
			else
			{
				strMsg+=CString("\n");
				strMsg+=strerror(errno);			
			}
		}
	}

	if (g_nDebugLevel & H3_DEBUG_ERROR)
	{
		DebugMessage(strModule,strSubModule,strFunction,_LOG_TYPE_ERROR,strMsg);
	}

	if (g_bErrorDisplay)
	{
		CString strTitle=H3LoadString(IDS_ERROR)+CString(" : ")+strAppName;
		return DisplayMessage(strModuleExt,strFunction,strTitle,strMsg,MB_OK+MB_ICONERROR);
	}

	return IDCANCEL;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3DisplayError(const CString &strMessage)
///	\brief   Affichage d'un message d'erreur.
/// \param	 strMessage Message d'erreur.
///	\retval	 IDOK le message a ete affiche et acquite par l'operateur
///	\retval	 IDCANCEL le message n'a pas ete affiche 
/// \see	 H3DisplayWarning(), H3DisplayInfo(), H3DisplayMessage()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP long H3DisplayError(const CString &strMessage)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return H3DisplayError("","",strMessage);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3DisplayWarning(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
///	\brief   Affichage d'un message d'attention.
/// \details Cette fenetre affiche un message d'attention ainsi que les noms de
///          la DLL, sous-module et fonction ayant genere le message.
///          Si le niveau de debogage le permet, le message est egalement 
///			 trace dans le fichier de debogage.
/// \param	 strSubModule Nom du sous-module dans lequel la fonction a ete appelee.
///			 Typiquement il s'agit du nom du fichier source contenant l'appel.
/// \param	 strFunction Nom de la fonction ayant generee le message.
/// \param	 strMessage Message a afficher.
///	\retval	 IDOK le message a ete affiche et acquite par l'operateur
///	\retval	 IDCANCEL le message n'a pas ete affiche 
/// \see	 H3DisplayError(), H3DisplayInfo(), H3DisplayMessage()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP long H3DisplayWarning(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
{
	_H3_MODULE_ADD_ON;

	if (g_nDebugLevel & H3_DEBUG_WARNING)
	{
		DebugMessage(strModule,strSubModule,strFunction,_LOG_TYPE_WARNING,strMessage);
	}

	if (g_bWarningDisplay)
	{
		CString strTitle=H3LoadString(IDS_WARNING)+CString(" : ")+strAppName;
		return DisplayMessage(strModuleExt,strFunction,strTitle,strMessage,MB_OK+MB_ICONWARNING);
	}

	return IDCANCEL;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3DisplayWarning(const CString &strMessage)
///	\brief   Affichage d'un message d'attention.
/// \details Si le niveau de debogage le permet, le message est egalement 
///			 trace dans le fichier de debogage.
/// \param	 strMessage Message a afficher.
///	\retval	 IDOK le message a ete affiche et acquite par l'operateur
///	\retval	 IDCANCEL le message n'a pas ete affiche 
/// \see	 H3DisplayError(), H3DisplayInfo(), H3DisplayMessage()
///	\author  E.COLON
///
H3APPTOOLS_EXP long H3DisplayWarning(const CString &strMessage)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return H3DisplayWarning("","",strMessage);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3DisplayInfo(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
///	\brief   Affichage d'un message d'information.
/// \details Cette fenetre affiche un message d'information ainsi que les noms de
///          la DLL, sous-module et fonction ayant genere le message.
///          Si le niveau de debogage le permet, le message est egalement 
///			 trace dans le fichier de debogage.
/// \param	 strSubModule Nom du sous-module dans lequel la fonction a ete appelee.
///			 Typiquement il s'agit du nom du fichier source contenant l'appel.
/// \param	 strFunction Nom de la fonction ayant generee le message.
/// \param	 strMessage Message a afficher.
///	\retval	 IDOK le message a ete affiche et acquite par l'operateur
///	\retval	 IDCANCEL le message n'a pas ete affiche 
/// \see	 H3DisplayError(), H3DisplayWarning(), H3DisplayMessage()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP long H3DisplayInfo(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
{
	_H3_MODULE_ADD_ON;

	if (g_nDebugLevel & H3_DEBUG_INFO)
	{
		DebugMessage(strModule,strSubModule,strFunction,_LOG_TYPE_INFO,strMessage);
	}

	if (g_bInfoDisplay)
	{
		CString BoxTitle=H3LoadString(IDS_INFORMATION)+CString(" : ")+strAppName;
		return DisplayMessage(strModuleExt,strFunction,BoxTitle,strMessage,MB_OK+MB_ICONINFORMATION);
	}

	return IDCANCEL;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP bool H3DisplayInfo(const CString &strMessage)
///	\brief   Affichage d'un message d'information.
/// \details Si le niveau de debogage le permet, le message est egalement 
///			 trace dans le fichier de debogage.
/// \param	 strMessage Message a afficher.
///	\retval	 IDOK le message a ete affiche et acquite par l'operateur
///	\retval	 IDCANCEL le message n'a pas ete affiche 
/// \see	 H3DisplayError(), H3DisplayWarning(), H3DisplayMessage()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP long H3DisplayInfo(const CString &strMessage)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return H3DisplayInfo("","",strMessage);
}

/////////////////////////////////////////////////////////////////////////////
long DisplayMessage(const CString &strModule,const CString &strFunction,const CString &strTitle,const CString &strMessage,UINT nType)
{
	// Message 
	CString strBoxMessage;
	strBoxMessage=strMessage+"\n\n";
	if (strModule!=_T(""))
		strBoxMessage=strBoxMessage+H3LoadString(IDS_MODULE)+strModule+CString("\n");
	
	if (strFunction!=_T(""))
	 	strBoxMessage=strBoxMessage+H3LoadString(IDS_FUNCTION)+strFunction+CString("\n");

	// Afficher
	return MessageBox(GetFocus(),strBoxMessage,strTitle,nType);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3DisplayMessage(const CString &strSubModule,const CString &strFunction,const CString &strTitle,const CString &strMessage,UINT nType)
///	\brief   Affichage d'un message dans une boite de dialogue.
/// \details L'affichage est complete par les noms de la DLL, du sous-module
///          et de la fonction ayant genere le message.
/// \param	 strSubModule Nom du sous-module dans lequel la fonction a ete appelee.
///			 Typiquement il s'agit du nom du fichier source contenant l'appel.
/// \param	 strFunction Nom de la fonction ayant generee le message.
/// \param   strTitle Titre de la boite de dialogue. Si titre est vide alors
///          le titre affiche est le nom de l'application.
/// \param	 strMessage Message a afficher.
/// \param   nType Combinaison de flags definissant les boutons et icones. 
///          Pour plus d'information sur ce parametre consulter la fonction
///			 MessageBox() du SDK Microsoft.
///	\retval	 long identificateur du bouton appuye (voir MessageBox())
/// \remarks Les messages affiches par cette fonction ne sont pas traces dans 
///			 le fichier de debogage.
/// \see	 MessageBox()
///	\author  E.COLON
H3APPTOOLS_EXP long H3DisplayMessage(const CString &strSubModule,const CString &strFunction,const CString &strTitle,const CString &strMessage,UINT nType)
{
	_H3_MODULE_ADD_ON;

	CString strBoxTitle;
	if (!strTitle.IsEmpty())
		strBoxTitle=strTitle;
	else
		 strBoxTitle=strAppName;

	return DisplayMessage(strModuleExt,strFunction,strBoxTitle,strMessage,nType);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3DisplayMessage(const CString &strTitle,const CString &strMessage,UINT nType)
///	\brief   Affichage d'un message dans une boite de dialogue.
/// \param   strTitle Titre de la boite de dialogue
/// \param	 strMessage Message a afficher.
/// \param   nType Combinaison de flags definissant les boutons et icones. 
///          Pour plus d'information sur ce parametre consulter la fonction
///			 MessageBox() du SDK Microsoft.
///	\retval	 long identificateur du bouton appuye (voir MessageBox())
/// \remarks Les messages affiches par cette fonction ne sont pas traces dans 
///			 le fichier de debogage.
/// \see	 MessageBox()
///	\author  E.COLON
H3APPTOOLS_EXP long H3DisplayMessage(const CString &strTitle,const CString &strMessage,UINT nType)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return H3DisplayMessage("","",strTitle,strMessage,nType);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP long H3SetDebugLevel(long nLevel)
///	\brief   Cette fonction fixe le niveau de debogage.
///	\param   nLevel Niveau de debogage fixe par une combinaison des drapeaux
///			 suivants : \n
///				H3_DEBUG_DISABLE	Desactivation de tous les tracages\n
///				H3_DEBUG_ENABLE		Activation de tous les tracages\n
///				H3_DEBUG_ERROR		Activer le tracage des messages d'erreur\n
///				H3_DEBUG_WARNING	Activer le tracage des messages d'attention\n
///				H3_DEBUG_INFO		Activer le tracage des messages d'information\n
///				H3_DEBUG_MESSAGE	Activer le tracage des messages\n
///				
///	\retval	 long ancien niveau de debogage
///  \see	 H3GetDebugLevel()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP long H3SetDebugLevel(long nLevel)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	long nOldLevel=g_nDebugLevel;
	if (nLevel)
	{
		if (!g_strDebugFile.IsEmpty())
		{
			g_nDebugLevel=nLevel;
		}		
	}

	return nOldLevel;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP long H3GetDebugLevel()
///	\brief   Cette fonction retourne le niveau de debogage courant.	
///	\retval	 long niveau de debogage courant
/// \see	 H3SetDebugLevel()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP long H3GetDebugLevel()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return g_nDebugLevel;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP void H3DebugInfo(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
///	\brief   Ajoute un message de trace de type "Info" dans le fichier de debogage
/// \param	 strSubModule Nom du sous-module dans lequel la fonction a ete appelee.
///			 Typiquement il s'agit du nom du fichier source contenant l'appel.
/// \param	 strFunction Nom de la fonction ayant generee le message.
/// \param	 strMessage Message a afficher.
/// \see	 H3DebugError(),H3DebugWarning()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP void H3DebugInfo(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
{
	if (g_nDebugLevel & H3_DEBUG_INFO)
	{
		CString strAppName=MyGetApplicationName();
		CString strModule=AfxGetAppName();

		AFX_MANAGE_STATE(AfxGetStaticModuleState());

		DebugMessage(strModule,strSubModule,strFunction,_LOG_TYPE_INFO,strMessage);
	}
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP void H3DebugWarning(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
///	\brief   Ajoute un message de trace de type "Warning" dans le fichier de debogage
/// \param	 strSubModule Nom du sous-module dans lequel la fonction a ete appelee.
///			 Typiquement il s'agit du nom du fichier source contenant l'appel.
/// \param	 strFunction Nom de la fonction ayant generee le message.
/// \param	 strMessage Message a afficher.
/// \see	 H3DebugError(),H3DebugInfo()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP void H3DebugWarning(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
{
	if (g_nDebugLevel & H3_DEBUG_WARNING)
	{
		CString strAppName=MyGetApplicationName();
		CString strModule=AfxGetAppName();

		AFX_MANAGE_STATE(AfxGetStaticModuleState());

		DebugMessage(strModule,strSubModule,strFunction,_LOG_TYPE_WARNING,strMessage);
	}
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP void H3DebugError(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
///	\brief   Ajoute un message de trace de type "Error" dans le fichier de debogage
/// \param	 strSubModule Nom du sous-module dans lequel la fonction a ete appelee.
///			 Typiquement il s'agit du nom du fichier source contenant l'appel.
/// \param	 strFunction Nom de la fonction ayant generee le message.
/// \param	 strMessage Message a afficher.
/// \see	 H3DebugError(),H3DebugInfo()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP void H3DebugError(const CString &strSubModule,const CString &strFunction,const CString &strMessage)
{
	if (g_nDebugLevel & H3_DEBUG_ERROR)
	{
		CString strAppName=MyGetApplicationName();
		CString strModule=AfxGetAppName();

		AFX_MANAGE_STATE(AfxGetStaticModuleState());

		DebugMessage(strModule,strSubModule,strFunction,_LOG_TYPE_ERROR,strMessage);
	}
}

/////////////////////////////////////////////////////////////////////////////
bool AddMessageToFile(const CString &strFilename,const CString &strMessage)
{
	FILE *Stream=fopen((char*)LPCTSTR(strFilename),"a+t");
	if (Stream)
	{
		CString str=strMessage;
		str.Replace('\n',' ');
		fprintf(Stream,"%s\n",LPCTSTR(str)); 
		fclose(Stream);
		return true;
	}
	return false;
}

/////////////////////////////////////////////////////////////////////////////
void DebugMessage(const CString &strModule,const CString &strSubModule,const CString &strFunction,const CString &strType,const CString &strMessage)
{
	if (g_nDebugLevel)
	{
		COleDateTime DateTime;
		DateTime = COleDateTime::GetCurrentTime();
		CString strDateTime=DateTime.Format();

		CString str=
			strDateTime+
			CString("\t")+strType+
			CString("\t")+strModule+
			CString("\t")+strSubModule+
			CString("\t")+strFunction+
			CString("\t")+strMessage+
			CString("\n");

		AddMessageToFile(g_strDebugFile,str);
	}
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP void H3SetDebugFile(const CString &strDebugFile)
///	\brief   Fixe le nom du fichier de debogage
/// \param	 strDebugFile Chemin et nom du fichier
/// \see	 H3GetDebugFile(), H3SetDebugLevel()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP void H3SetDebugFile(const CString &strDebugFile)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	g_strDebugFile=strDebugFile;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP CString H3GetDebugFile()
///	\brief   Retourne le nom du fichier de debogage
/// \see	 H3SetDebugFile(), H3SetDebugLevel()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP CString H3GetDebugFile()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return g_strDebugFile;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool H3CreateDirectory(const CString &strPath,LPSECURITY_ATTRIBUTES lpSecurityAttributes)
///	\brief   Cette fonction cree une arborescence complete a partir de la racine.
///	\param   strPath Chaine de caracteres identifiant le chemin a creer. 
///					 Le nom de dossier doit se terminer par '\'.
///					 Exemple d'arborescence: "c:\\toto1\\toto2\\toto3\\"
///	\param   lpSecurityAttributes Structure de donnee definissant les attributs de securite
///	\retval	 true succes de la fonction
///	\retval	 false echec de la fonction
///	\remarks Pour plus d'informations, voir la fonction CreateDirectory() du SDK Microsoft
///  \see	 H3DirectoryExist(),H3ValidPath()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP bool H3CreateDirectory(const CString &strPath,LPSECURITY_ATTRIBUTES lpSecurityAttributes)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	bool bRetVal=false;

	if (!H3DirectoryExist(strPath))
	{
		long len=strPath.GetLength();
		if (len>1)
		{
			char *pszPath=new char [len+1];
			memset(pszPath,0,len+1);
			for (long i=0;i<len;i++)
			{
				pszPath[i]=strPath.GetAt(i);
				if (strPath.GetAt(i)=='\\')
				{
					bRetVal=CreateDirectory(pszPath,NULL)?true:false;
				}
			}
			delete [] pszPath;
		}
	}

	return bRetVal;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool H3DirectoryExist(const CString &strPathName)
///	\brief   Cette fonction verifie l'existence d'un chemin.
///	\param   strPathName Chaine de caracteres identifiant le chemin 
///	\retval	 true le dossier existe
///	\retval	 false le dossier n'existe pas
///	\remarks Le fait que le dossier existe ne signifie pas qu'il soit 
///			 accessible en ecriture.
/// \see	 H3CreateDirectory(),H3ValidPath()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP bool H3DirectoryExist(const CString &strPathName)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	char szBuffer[_MAX_PATH*2];

	// Sauver le repertoire courant
	if( _getcwd( szBuffer, sizeof(szBuffer) ) == NULL )
		return false;

	int res=_chdir( LPCTSTR(strPathName));

	_chdir(szBuffer);

	if (res) return false;
	return true;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool H3FileExist(const CString &strFilename)
///	\brief   Cette fonction verifie l'existence d'un fichier.
///	\param   strFilename Chemin et nom du fichier a tester 
///	\retval	 true le fichier existe
///	\retval	 false le fichier n'existe pas
///	\author  E.COLON
/// 
H3APPTOOLS_EXP bool H3FileExist(const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if (strFilename.IsEmpty())
		return false;

	FILE *Stream=nullptr;

	if (Stream=fopen(LPCTSTR(strFilename),"r"))
	{
		fclose(Stream);
		return true;
	}

	return false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP void H3SplitPath(const CString &strPath,CString &strDrive,CString &strDir,CString &strName,CString &strExt)
///	\brief   Decompose un chemin
///	\param   strPath Entree, chemin a decomposer
///	\param	 strDrive Sortie, lecteur (par exemple "C:")
/// \param	 strDir Sortie, chemin sans le lecteur
/// \param   strName Sortie, nom de fichier
/// \param   strExt Sortie, extension de fichier
/// \see	 H3FileName(), H3FileExt()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP void H3SplitPath(const CString &strPath,CString &strDrive,CString &strDir,CString &strName,CString &strExt)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	char szDrive[_MAX_DRIVE*2];
	char szDir[_MAX_DIR*2];
	char szName[_MAX_FNAME*2];
	char szExt[_MAX_EXT*2];

	_splitpath(LPCTSTR(strPath),szDrive,szDir,szName,szExt);

	strDrive=_T(szDrive);
	strDir=_T(szDir);
	strName=_T(szName);
	strExt=_T(szExt);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3APPTOOLS_EXP CString H3FileName(const CString &strFilename)
///	\brief   Extrait et retourne le nom d'un fichier
///	\param   strFilename Entree, chemin a decomposer
///	\retval	 CString nom de fichier
/// \see	 H3SplitPath(), H3FileExt()
///	\author  E.COLON
/// 
H3APPTOOLS_EXP CString H3FileName(const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strDrive,strPath,strName,strExt;
	H3SplitPath(strFilename,strDrive,strPath,strName,strExt);
	return strName;
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP CString H3GetPrivProfileString(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	char szTmp[2048];

	GetPrivateProfileString(
		LPCTSTR(strSection),
		LPCTSTR(strEntry),
		"",
		szTmp,
		sizeof(szTmp),
		LPCTSTR(strFilename));

    if (GetLastError() != 0)
    {
        throw std::runtime_error(strFilename+", "+ strSection+", "+strEntry+" missing");
    }

	return(_T(szTmp));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const CString &strString,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	return(WritePrivateProfileString(
	  LPCTSTR(strSection),
	  LPCTSTR(strEntry),
	  LPCTSTR(strString),
	  LPCTSTR(strFilename))?true:false);
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP float H3GetPrivProfileFloat(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strDefault;
	CString strValue;

	strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	return((float)atof(LPCTSTR(strValue)));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const float &fValue,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%g",fValue);

	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP double H3GetPrivProfileDouble(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;

	strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	return((float)atof(LPCTSTR(strValue)));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const double &dValue,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%lg",dValue);

	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP int H3GetPrivProfileInt(const CString &strSection,const CString &strEntry,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;

	strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	return((int)atoi(LPCTSTR(strValue)));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const int &nValue,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%d",nValue);
	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP long H3GetPrivProfileLong(const CString &strSection,const CString &strEntry,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;

	strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	return((long)atoi(LPCTSTR(strValue)));
}

/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const long &nValue,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%d",nValue);
	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

/////////////////////////////////////////////////////////////////////////////
// Parcours le flux jusqu'au premier caractere c ou EOF
bool fFind(FILE *Stream,char c)
{
	while(true)
	{
		char c1;
		if (fread(&c1,sizeof(char),1,Stream)!=1)
		{
			if (feof(Stream))
			{
				return false;
			}
		}
		if (c1==c)
		{
			return true;
		}
	}	
	return false;
}


/////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP CString H3fReadCString(FILE *Stream)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString str;

	// Determiner la taille du string jusqu'au premier
	// caractere '\0'
	long nInitialPos=ftell(Stream);
	bool bFound=fFind(Stream,'\0');
	long nFinalPos=ftell(Stream);
	long nOffset=nFinalPos-nInitialPos;
	fseek(Stream,-nOffset,SEEK_CUR);

	char *psz=nullptr;
	if (bFound)
	{
		unsigned long nLen=nOffset;

		psz=new char [nLen];
		if (fread(psz,sizeof(char),nLen,Stream)==nLen)
			str=_T(psz);
	}
	if(psz!=nullptr){
		delete [] psz;
		psz=nullptr;
	}

	return str;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP vector <double> H3GetPrivProfileVectorDouble(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	vector <double> values;

	long nSize=H3GetPrivProfileLong(strSection,strEntry+_T("_Size"),strFilename);
	if (nSize==0)
	{
        throw std::runtime_error(strFilename + ", " + strSection + ", " + strEntry + " error");
    }

	double FPNaN=H3GetFPNaN();
	values.resize(nSize);
	for (long i=0;i<nSize;i++)
	{
		CString strName;strName.Format("_%d",i);
		values[i]=H3GetPrivProfileDouble(strSection,strEntry+strName,strFilename);
	}

	return values;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const vector <double> &values,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	size_t nSize=values.size();//long nSize=values.size();
	H3WritePrivProfileInt(strSection,strEntry+_T("_Size"),(long)nSize,strFilename);

	for (size_t i=0;i<nSize;i++)
	{
		CString strName;strName.Format("_%d",i);
		if (!H3WritePrivProfile(strSection,strEntry+strName,values[i],strFilename))
			return FALSE;
	}

	return TRUE;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP vector <H3_POINT2D_FLT64> H3GetPrivProfileVector2DF64(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	vector <H3_POINT2D_FLT64> pts;

	long nSize=H3GetPrivProfileInt(strSection,strEntry+_T("_Size"),strFilename);
	if (nSize==0)
	{
        throw std::runtime_error(strFilename + ", " + strSection + ", " + strEntry + " missing");
    }

	double FPNaN=H3GetFPNaN();
	pts.resize(nSize);
	for (long i=0;i<nSize;i++)
	{
		CString strName;strName.Format("_%d",i);
		pts[i]=H3GetPrivProfile2DF64(strSection,strEntry+strName,strFilename);
	}

	return pts;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const vector <H3_POINT2D_FLT64> pts,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	size_t nSize=pts.size();
	H3WritePrivProfileInt(strSection,strEntry+_T("_Size"),(long)nSize,strFilename);

	for (size_t i=0;i<nSize;i++)
	{
		CString strName;strName.Format("_%d",i);
		if (!H3WritePrivProfile(strSection,strEntry+strName,pts[i],strFilename))
			return FALSE;
	}

	return TRUE;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP H3_POINT2D_FLT64 H3GetPrivProfile2DF64(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	H3_POINT2D_FLT64 pt;
    if (sscanf(LPCTSTR(strValue), "%lg %lg", &pt.x, &pt.y) != 2)
    {
        throw std::runtime_error(strFilename + ", " + strSection + ", " + strEntry + " error");
    }

	return pt;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const H3_POINT2D_FLT64 &pt,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	
	CString strValue;
	strValue.Format("%lg %lg",pt.x,pt.y);

	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP H3_RECT_FLT64 H3GetPrivProfileRectF64(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	H3_RECT_FLT64 rc;
    if (sscanf(LPCTSTR(strValue), "%lg %lg %lg %lg", &rc.left, &rc.top, &rc.right, &rc.bottom) != 4)
    {
        throw std::runtime_error(strFilename + ", " + strSection + ", " + strEntry + " error");
    }

	return rc;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const H3_RECT_FLT64 &rc,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%lg %lg %lg %lg",rc.left,rc.top,rc.right,rc.bottom);

	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,H3_MATRIX_FLT64 &mat,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%d %d ",mat.GetLi(),mat.GetCo());
	
	unsigned long i;
	for (i=0;i<mat.GetSize();i++)
	{
		CString s;
		s.Format("%.15lg ",mat[i]);
		strValue+=s;
	}

	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

////////////////////////////////////////////////////////////////////////////////
//H3APPTOOLS_EXP H3_MATRIX_FLT64 H3GetPrivProfileMatrixF64(const CString &strSection,const CString &strEntry,H3_MATRIX_FLT64 &matDefault,const CString &strFilename)
//{
//	AFX_MANAGE_STATE(AfxGetStaticModuleState());
//
//	CString strFunction("H3GetPrivProfile()");
//
//	////////////////////////////////////////////////////////////////////////////
//	// valeur par defaut
//	CString strDefault;
//	strDefault.Format("%d %d ",matDefault.GetLi(),matDefault.GetCo());
//	for (unsigned long i=0;i<matDefault.GetSize();i++)
//	{
//		CString s;
//		s.Format("%lg ",matDefault[i]);
//		strDefault+=s;
//	}
//
//	////////////////////////////////////////////////////////////////////////////
//	// Lire la chaine de caracteres
//	CString strValue=H3GetPrivProfileString(strSection,strEntry,strDefault,strFilename);
//
//	////////////////////////////////////////////////////////////////////////////
//	// convertir la chaine de caracteres en H3_MATRIX_FLT64
//	// EC 06/03/2006 Pour faire vite je ne gere que le cas d'une matrice à 3 
//	// elements mais il faudrait generaliser
//	long nLi,nCo;
//	sscanf(LPCTSTR(strValue),"%d %d",&nLi,&nCo);
//	if ((nLi*nCo)==0)
//		return H3_MATRIX_FLT64(0,0);
//
//
//	double v1,v2,v3;
//	sscanf(LPCTSTR(strValue),"%d %d %lg %lg %lg",&nLi,&nCo,&v1,&v2,&v3);
//
//	if ((nLi*nCo)!=3)
//	{
//		H3DisplayError(strModule,strFunction,"Cas de figure non gere pour l'instant");
//		return H3_MATRIX_FLT64(0,0);
//	}
//
//	H3_MATRIX_FLT64 matDest(nLi,nCo);
//	matDest[0]=v1;
//	matDest[1]=v2;
//	matDest[2]=v3;
//
//	return matDest;
//}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP H3_RECT_INT32 H3GetPrivProfileRectInt32(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	H3_RECT_INT32 rc;
    if (sscanf(LPCTSTR(strValue), "%d %d %d %d", &rc.left, &rc.top, &rc.right, &rc.bottom) != 4)
    {
        throw std::runtime_error(strFilename + ", " + strSection + ", " + strEntry + " missing");
    }

	return rc;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const H3_RECT_INT32 &rc,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%d %d %d %d",rc.left,rc.top,rc.right,rc.bottom);

	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP H3_POINT3D_FLT64 H3GetPrivProfile3DF64(const CString &strSection,const CString &strEntry,const CString &strFilename)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue=H3GetPrivProfileString(strSection,strEntry,strFilename);

	H3_POINT3D_FLT64 pt;
    if (sscanf(LPCTSTR(strValue), "%lg %lg %lg", &pt.x, &pt.y, &pt.z) != 3)
    {
        throw std::runtime_error(strFilename + ", " + strSection + ", " + strEntry + " missing");
    }

	return pt;
}

////////////////////////////////////////////////////////////////////////////////
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const H3_POINT3D_FLT64 &pt,const CString &strFilename) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strValue;
	strValue.Format("%lg %lg %lg",pt.x,pt.y,pt.z);

	return(H3WritePrivProfile(strSection,strEntry,strValue,strFilename));
}

