
// NanoCalib.h : fichier d'en-tête principal pour l'application PROJECT_NAME
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux

extern bool g_bInitCalibSys;

// CNanoCalibApp :
// Consultez NanoCalib.cpp pour l'implémentation de cette classe
//

class CNanoCalibApp : public CWinApp
{
public:
	CNanoCalibApp();

// Substitutions
public:
    virtual BOOL InitInstance();

// Implémentation

	DECLARE_MESSAGE_MAP()
private:
    void OnException();
};

extern CNanoCalibApp theApp;
// Command line parameters
extern CString m_clCalibCamSourceImagesFolder;
extern int m_clCalibCamMireSizeX;
extern int m_clCalibCamMireSizeY;
extern float m_clCalibCamMireStepX;
extern float m_clCalibCamMireStepY;

extern CString m_clCalibSysSourceImagesFolder;
extern float m_clCalibSysPitchX;
extern float m_clCalibSysPitchY;
extern int m_clCalibSysPeriodX;
extern int m_clCalibSysPeriodY;
extern int m_clCalibSysScreenSizeX;
extern int m_clCalibSysScreenSizeY;
extern int m_clCalibSysScreenRefPosX;
extern int m_clCalibSysScreenRefPosY;
extern int m_clCalibSysCrossX;
extern int m_clCalibSysCrossY;