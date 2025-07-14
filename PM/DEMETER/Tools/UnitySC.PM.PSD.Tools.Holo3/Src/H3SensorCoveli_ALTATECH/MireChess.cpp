// MireChess.cpp: implementation of the CMireChess class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "Coveli.h"
#include "tools.h"
#include "MireChess.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#define DEFAULT_CALIBXYZ_NBPTS_X				14
#define DEFAULT_CALIBXYZ_STEP_X					(65.0f)
#define DEFAULT_CALIBXYZ_NBPTS_Y				19
#define DEFAULT_CALIBXYZ_STEP_Y					(65.0f)
#define DEFAULT_CALIBXYZ_NBPTS_Z				5
#define DEFAULT_CALIBXYZ_STEP_Z					(10.0f)
#define DEFAULT_CALIBXYZ_PTSCOLOR				0		// Couleur

static CString strModule("CMireChess");
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CMireChess::CMireChess()
{

}

CMireChess::~CMireChess()
{
	CString strFunction("~CMireChess()");

}
//Nombre de point de la mire
long CMireChess::GetNbPtsX()const
{
	return m_nNbPtsX;
}

void CMireChess::SetNbPtsX(long nNbPtsX)
{
	m_nNbPtsX=nNbPtsX;
}

long CMireChess::GetNbPtsY()const
{
	return m_nNbPtsY;
}

void CMireChess::SetNbPtsY(long nNbPtsY)
{
	m_nNbPtsY=nNbPtsY;
}

// Pas de la mire

float CMireChess::GetStepX()const
{
	return m_fStepX;
}

void CMireChess::SetStepX(float fStepX)
{
	m_fStepX=fStepX;
}

float CMireChess::GetStepY()const
{
	return m_fStepY;
}

void CMireChess::SetStepY(float fStepY)
{
	m_fStepY=fStepY;
}

//Couleur des points
long CMireChess::GetPtsColor()const
{
	return m_nPtsColor;
}

void CMireChess::SetPtsColor(long nPtsColor)
{
	m_nPtsColor=nPtsColor;
}

//Lecture ecriture setting

bool CMireChess::LoadSettings(CString strFileName, CString strSection)
{
	CString strFunction("LoadSettings()");
	H3DebugInfo(strModule,strFunction,strFileName+CString(";")+strSection);

	CString str=strSection+_T("_")+strModule;
	m_nNbPtsX=H3GetPrivProfileInt(str,_T("NbPtsX"),DEFAULT_CALIBXYZ_NBPTS_X,strFileName);
	m_fStepX=H3GetPrivProfileFloat(str,_T("StepX"),DEFAULT_CALIBXYZ_STEP_X,strFileName);
	m_nNbPtsY=H3GetPrivProfileInt(str,_T("NbPtsY"),DEFAULT_CALIBXYZ_NBPTS_Y,strFileName);
	m_fStepY=H3GetPrivProfileFloat(str,_T("StepY"),DEFAULT_CALIBXYZ_STEP_Y,strFileName);
//	m_nNbPtsZ=H3GetPrivProfileInt(str,_T("NbPtsZ"),DEFAULT_CALIBXYZ_NBPTS_Z,strFileName);
//	m_StepZ=H3GetPrivProfileFloat(str,_T("StepZ"),DEFAULT_CALIBXYZ_STEP_Z,strFileName);
	m_nPtsColor=H3GetPrivProfileInt(str,_T("nPtsColor"),DEFAULT_CALIBXYZ_PTSCOLOR,strFileName);


	return true;
}

bool CMireChess::SaveSettings(CString strFileName, CString strSection)
{
	CString strFunction("SaveSettings()");
	H3DebugInfo(strModule,strFunction,strFileName+CString(";")+strSection);

	CString str=strSection+_T("_")+strModule;

	H3WritePrivProfileInt(str,_T("NbPtsX"),m_nNbPtsX,strFileName);
	H3WritePrivProfileFloat(str,_T("StepX"),m_fStepX,strFileName);
	H3WritePrivProfileInt(str,_T("NbPtsY"),m_nNbPtsY,strFileName);
	H3WritePrivProfileFloat(str,_T("StepY"),m_fStepY,strFileName);
//	H3WritePrivProfileInt(str,_T("NbPtsZ"),m_nNbPtsZ,strFileName);
//	H3WritePrivProfileFloat(str,_T("StepZ"),m_fStepZ,strFileName);
	H3WritePrivProfileInt(str,_T("nPtsColor"),m_nPtsColor,strFileName);

	return true;
}