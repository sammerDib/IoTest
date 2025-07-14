// H3Mire.cpp: implementation of the CH3Mire class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "H3Mire.h"
#include "H3AppToolsDecl.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#define DEFAULT_INI_FILE _T("c:\\temp\\mydefaultinifile.txt") 

static CString strModule("CH3Mire");

int CH3Mire::m_iLiIntersection=0;
int CH3Mire::m_iCoIntersection=0;

float CH3Mire::m_fMireStepX=0.0f;
float CH3Mire::m_fMireStepY=0.0f;

#define DEFAULT_STRFILEMIRE ""
//#define DEFAULT_INTERSECTIONS (10L)
//#define DEFAULT_MIRE_STEP (10.0F)

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CH3Mire::CH3Mire()
{
	m_aMetric=H3_ARRAY2D_FLT64(0, 0);
}

CH3Mire::~CH3Mire()
{
#if XML_FILE
	delete file;
	file = NULL;
#endif
}

///////////////////////////////////////////////////////////////////////////////
#if XML_FILE==0
bool CH3Mire::LoadSettings(
	CString strFileName,
	CString strSection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("LoadSettings()");
	H3DebugInfo(strModule,strFunction,"");

	//if (strSection.IsEmpty())
	//	strSection=_T("CH3Mire");

	m_iLiIntersection=H3GetPrivProfileInt(strSection,_T("nLi"),strFileName);
	m_iCoIntersection=H3GetPrivProfileInt(strSection,_T("nCo"),strFileName);

	m_fMireStepX=H3GetPrivProfileFloat(strSection,_T("StepX"),strFileName);
	m_fMireStepY=H3GetPrivProfileFloat(strSection,_T("StepY"),strFileName);

	CString strMire = H3GetPrivProfileString(strSection,_T("strFileMire"),strFileName);

	return LoadMire(strMire);
}

/////////////////////////////////////////////////////////////////////////////
// Enregistrement des reglages
/*! 
* 	\fn      bool CH3Mire::SaveSettings(CString strFileName,CString strSection)
* 	\author  M FERLET
* 	\brief   sauvegarde des parametres
* 	\param   CString strFileName : nom du fichier
* 	\param   CString strSection : nom de la section
* 	\return  bool
* 	\remarks 
*/ 
bool CH3Mire::SaveSettings(
	CString strFileName,
	CString strSection)
{

	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("SaveSettings()");
	H3DebugInfo(strModule,strFunction,"");
	
	if (m_strFileMire.IsEmpty())
		m_strFileMire = DEFAULT_STRFILEMIRE;

	if (strSection.IsEmpty())
		strSection=_T("CH3Mire");

	CString str=strSection;

	H3WritePrivProfileString(strSection,_T("strFileMire"),m_strFileMire,strFileName);
	H3WritePrivProfileInt(strSection,_T("nLi"),m_iLiIntersection,strFileName);
	H3WritePrivProfileInt(strSection,_T("nCo"),m_iCoIntersection,strFileName);
	H3WritePrivProfileFloat(strSection,_T("StepX"),m_fMireStepX,strFileName);
	H3WritePrivProfileFloat(strSection,_T("StepY"),m_fMireStepY,strFileName);

	return true;
}
#endif

///////////////////////////////////////////////////////////////////////////////
#if XML_FILE
bool CH3Mire::LoadSettings(

	H3XMLFile* file,
	CString strSection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("LoadSettings()");
	H3DebugInfo(strModule,strFunction,"");

	if (strSection.IsEmpty())
		strSection=_T("CH3Mire");

	CString str=strSection;

	m_iLiIntersection=file->GetProfileInt(str,_T("nLi"));
	m_iCoIntersection=file->GetProfileInt(str,_T("nCo"));

	CString strMire = file->GetProfileString(str,_T("strFileMire"));

	LoadMire(strMire);

	return true;
}

/////////////////////////////////////////////////////////////////////////////
// Enregistrement des reglages
/*! 
* 	\fn      bool CH3Mire::SaveSettings(CString strFileName,CString strSection)
* 	\author  M FERLET
* 	\brief   sauvegarde des parametres
* 	\param   CString strFileName : nom du fichier
* 	\param   CString strSection : nom de la section
* 	\return  bool
* 	\remarks 
*/ 
bool CH3Mire::SaveSettings(
	H3XMLFile* file,
	CString strSection)
{

	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("SaveSettings()");
	H3DebugInfo(strModule,strFunction,"");
	
	if (m_strFileMire.IsEmpty())
		m_strFileMire = DEFAULT_STRFILEMIRE;

	if (strSection.IsEmpty())
		strSection=_T("CH3Mire");

	CString str=strSection;

	file->SetProfileString(str,_T("strFileMire"),m_strFileMire);
	file->SetProfileInt(str,_T("nLi"),m_iLiIntersection);
	file->SetProfileInt(str,_T("nCo"),m_iCoIntersection);
	
	return true;
}
#endif
/////////////////////////////////////////////////////////////////////////////////////

int CH3Mire::GetLi()const
{
	return m_iLiIntersection;
}

int CH3Mire::GetCo()const
{
	return m_iCoIntersection;
}

float CH3Mire::GetMireStepX()const
{
	return m_fMireStepX;
}

float CH3Mire::GetMireStepY()const
{
	return m_fMireStepY;
}

int CH3Mire::GetNbTarget()const
{
	return m_iCoIntersection*m_iLiIntersection;
}

H3_ARRAY2D_FLT64 CH3Mire::GetMetric()const
{
	return m_aMetric;
}

bool CH3Mire::LoadMire(CString strFile)
{
	CString strFunction("LoadMire");

	if (!strFile.IsEmpty())
	{
		if (!H3FileExist(strFile))
		{
				H3DebugWarning(strModule, strFunction,_T("Le Fichier n'existe pas\n")+strFile);
				return  false;
		}
		else{
			H3DebugWarning(strModule, strFunction,strFile);
		}

		const unsigned long nbIntersection=m_iCoIntersection*m_iLiIntersection;
		size_t nLi,nCo;

		m_strFileMire=strFile;
		H3_ARRAY2D_FLT32 fTemp;
		fTemp.LoadASCII(m_strFileMire);

		nLi=fTemp.GetLi();
		nCo=fTemp.GetCo();

		if (nCo<3)
		{
			CString msg;
			msg.Format("Le Fichier n'est pas conforme (nCo=%d<3)\n",nCo);
			H3DebugWarning(strModule, strFunction,msg+strFile);
			return false;
		}
		if (nLi<nbIntersection)
		{
			CString msg;
			msg.Format("Le Fichier n'est pas conforme (nbIntersection=%d!=)\n",nCo);
			H3DebugWarning(strModule, strFunction,_T("Le Fichier n'est pas conforme (nbIntersection)\n")+strFile);
			return false;
		}

		m_aMetric=H3_ARRAY2D_FLT64(3,nLi);
		for (UINT u=0;u<nLi;u++)
		{
			m_aMetric(0,u)=fTemp(u,0);
			m_aMetric(1,u)=fTemp(u,1);
			m_aMetric(2,u)=fTemp(u,2);
		}

		return true;
	}
	else
	{
		m_aMetric=H3_ARRAY2D_FLT64(3,m_iCoIntersection*m_iLiIntersection);

		size_t i,j,k;
		for (i=0, k=0;  i<m_iLiIntersection; i++)
		{
			for (j=0; j<m_iCoIntersection; j++, k++)
			{
				m_aMetric(0,k) = j*m_fMireStepX;
				m_aMetric(1,k) = i*m_fMireStepY;
				m_aMetric(2,k) = 0;
			}
		}
		return true;
	}
}


