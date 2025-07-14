// MireHMAP.cpp: implementation of the CMireHMAP class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "coveli.h"
#include "MireHMAP.h"
#include "math.h"

#define DEFAULT_INI_FILE _T("MireHMAP.ini")

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

static CString strModule("MireHMAP");
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

/*! 
* 	\fn      : CMireHMAP::CMireHMAP
* 	\brief   : 
* 	\return  : 
* 	\remarks : 
*/ 
CMireHMAP::CMireHMAP():m_isInitialised(false)
{

}

/*! 
* 	\fn      : CMireHMAP::~CMireHMAP
* 	\brief   : 
* 	\return  : 
* 	\remarks : 
*/ 

CMireHMAP::~CMireHMAP()
{
	CString strFunction("~CMireHMAP()");

}

/*! 
* 	\fn      : CMireHMAP::GetStepX
* 	\brief   : StepX: pas de la mire Holomap dans la direction X
* 	\return  : float
* 	\remarks : 
*/ 

float CMireHMAP::GetStepX()const
{
	return m_fStepX;
}

/*! 
* 	\fn      : CMireHMAP::GetStepY
* 	\brief   : StepY: pas de la mire Holomap dans la direction Y
* 	\return  : float
* 	\remarks : 
*/ 

float CMireHMAP::GetStepY()const
{
	return m_fStepY;
}

/*! 
* 	\fn      H3_POINT3D_FLT32 Phase2Position(float PhaseX, float PhaseY)
* 	\brief   retourne la position 3D d'un point de la mire HoloMap dont la phase est passée en parametre
*				cette position est exprimée dans un repereMire HMAP: origine: le point de la mire sur lequel est la phase (0,0)
* 	\param   float PhaseX : 
* 	\param   float PhaseY : 
* 	\return  H3_POINT3D_FLT32
* 	\remarks 
*/ 
H3_POINT3D_FLT32 CMireHMAP::Phase2Position(const float PhaseX,const float PhaseY)const
{
	return H3_POINT3D_FLT32(m_fStepX*PhaseX/fTWO_PI,m_fStepY*PhaseY/fTWO_PI,0.0f);
}

/*! 
* 	\fn      bool CMireHMAP::ProcessPhaseToMetric(UWImage& UW)
* 	\brief   transforme une carte de phase en une position sur la mire Hmap
* 	\param   UWImage& UW
* 	\return  bool
* 	\remarks initialise m_aMesureX et m_aMesureY et m_aMesureZ(à 0)
*/
bool CMireHMAP::ProcessPhaseToMetric(S_3DCoord_onHMapMire& out_Pos,const UWImage& UW)
{
	CString strFunction("ProcessPhaseToMetric()");
	H3DebugInfo(strModule,strFunction,"");

	const unsigned long nSizeX=UW.GetCo();
	const unsigned long nSizeY=UW.GetLi();
	const unsigned long nSize =nSizeY*nSizeX;

	//passage ds le repere Mire. Le pixel qui affiche le zero de phase est l'orgine du rep ecran 
	float scaleX=m_fStepX/(fTWO_PI);
	float scaleY=m_fStepY/(fTWO_PI);

	out_Pos.a2dCoordX=UW.m_aX; 
	out_Pos.a2dCoordX *= scaleX;

	out_Pos.a2dCoordY=UW.m_aY;
	out_Pos.a2dCoordY *= scaleY;

	out_Pos.a2dCoordZ.ReAlloc(nSizeY,nSizeX);
	out_Pos.a2dCoordZ.Fill(0);

	out_Pos.a2dMask=UW.m_aMask;

	return true;
}

/*! 
* 	\fn      : CMireHMAP::LoadSettings
* 	\brief   : charge les parametres decrivant la mire Holomap
* 	\param   : CString strFileName : 
* 	\param   : CString strSection : 
* 	\return  : bool
* 	\remarks : 
*/ 
/*
bool CMireHMAP::LoadSettings(CString strFileName, CString strSection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("LoadSettings()"),msg;
	CString strMsg("Impossible de charger le fichier de configuration.");

	if (strFileName.IsEmpty() || strFileName==CString("DEFAULT"))
		strFileName=DEFAULT_INI_FILE;

	H3DebugInfo(strModule,strFunction,strFileName);

	if (!strSection.IsEmpty())
		strSection+=CString("_");
	strSection+=strModule;

	// Lecture des parametres
	//...
	m_fStepX=H3GetPrivProfileFloat(strSection,"fStepX",30.0f,strFileName);
	m_fStepY=H3GetPrivProfileFloat(strSection,"fStepY",40.0f,strFileName);

	m_isInitialised=true;

	// Enregistrer en tant que configuration par defaut
	SaveSettings();

	return true;
}
*/
bool CMireHMAP::LoadSettings2(CString strFileName, CString strSection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("LoadSettings2()"),msg;
	CString strMsg("Impossible de charger le fichier de configuration.");

	if (strFileName.IsEmpty() || strFileName==CString("DEFAULT"))
		strFileName=DEFAULT_INI_FILE;

	H3DebugInfo(strModule,strFunction,strFileName);

	// Lecture des parametres
	//...

	float fPitchX=H3GetPrivProfileFloat(strSection,"PitchX",strFileName);
	float fPitchY=H3GetPrivProfileFloat(strSection,"PitchY",strFileName);

	float fPeriodX=H3GetPrivProfileFloat(strSection,"PeriodX",strFileName);
	float fPeriodY=H3GetPrivProfileFloat(strSection,"PeriodY",strFileName);

	unsigned long pixRef_Xscreen=H3GetPrivProfileLong(strSection,_T("pixRef_Xscreen"),strFileName);
	unsigned long pixRef_Yscreen=H3GetPrivProfileLong(strSection,_T("pixRef_Yscreen"),strFileName);

	unsigned long screen_Xsz=H3GetPrivProfileLong(strSection,_T("screen_Xsz"),strFileName);
	unsigned long screen_Ysz=H3GetPrivProfileLong(strSection,_T("screen_Ysz"),strFileName);

	m_fStepX=fPitchX*fPeriodX;
	m_fStepY=fPitchY*fPeriodY;

	m_phiRef_X= (fTWO_PI/fPeriodX)*((float)pixRef_Xscreen- (float)screen_Xsz/2.0f);
	m_phiRef_Y= (fTWO_PI/fPeriodY)*((float)pixRef_Yscreen- (float)screen_Ysz/2.0f);
	   
	m_isInitialised=true;

	// Enregistrer en tant que configuration par defaut surtout si une variable n'etait pas initialisée
	//SaveSettings(strFileName,strSection);

	return true;
}

/*! 
* 	\fn      : CMireHMAP::SaveSettings
* 	\brief   : sauvegarde des parameteres decrivant la mire holomap
* 	\param   : CString strFileName : 
* 	\param   : CString strSection : 
* 	\return  : bool
* 	\remarks : 
*/ 
/*
bool CMireHMAP::SaveSettings(CString strFileName, CString strSection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("SaveSettings()");
	H3DebugInfo(strModule,strFunction,"");

	CString str;

	if (strFileName.IsEmpty())
		strFileName=DEFAULT_INI_FILE;

	H3DebugInfo(strModule,strFunction,strFileName);

	return true;
}
*/
