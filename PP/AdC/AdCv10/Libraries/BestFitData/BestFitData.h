// BestFitData.h : fichier d'en-tête principal pour la DLL BestFitData
//

#pragma once

#ifndef __AFXWIN_H__
	#error "incluez 'stdafx.h' avant d'inclure ce fichier pour PCH"
#endif

#include "resource.h"		// symboles principaux


// CBestFitDataApp
// Consultez BestFitData.cpp pour l'implémentation de cette classe
//

class CBestFitDataApp : public CWinApp
{
public:
	CBestFitDataApp();

// Substitutions
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};

extern "C" 
{
	__declspec(dllexport) int BestFit_Threshold(float* pIn, byte* pINMask, byte* pOut, int iSizeX, int iSizeY, int nNbSample, int nOrder, float fAcceptMin, float fAcceptMax, int nPitchIN, int nPitchMASK);
	__declspec(dllexport) int BestFitSurface(float* pIn, byte* pINMask, float* pOut, int iSizeX, int iSizeY, int nNbSample, int nOrder, int nPitchIN, int nPitchMASK);
	__declspec(dllexport) int BestFitFlattenSurface(float* pIn, byte* pInMask, float* pOut, int iSizeX, int iSizeY, int nSampleStep, int nOrder, int nPitchIN, int nPitchMASK);

}