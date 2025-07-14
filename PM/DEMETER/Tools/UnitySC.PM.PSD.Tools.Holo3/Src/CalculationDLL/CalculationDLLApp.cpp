// CalculationDLL.cpp : définit les routines d'initialisation pour la DLL.
//
#pragma once
#include "stdafx.h"
#include "CalculationDLLApp.h"
#include "H3_HoloMap_AltaType.h"
#include "H3AppToolsDecl.h"
#include "ImageJ.h"
#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CCalculationDLLApp

BEGIN_MESSAGE_MAP(CCalculationDLLApp, CWinApp)
END_MESSAGE_MAP()


// construction CCalculationDLLApp

CCalculationDLLApp::CCalculationDLLApp()
{
	// TODO: ajoutez ici du code de construction,
	// Placez toutes les initialisations significatives dans InitInstance
}


// Seul et unique objet CCalculationDLLApp

CCalculationDLLApp theApp;


// initialisation de CCalculationDLLApp
BOOL CCalculationDLLApp::InitInstance()
{
	CWinApp::InitInstance();
	return TRUE;
}

CCalculationDLLApp::~CCalculationDLLApp(void)
{
}
