#pragma once
#include "StdAfx.h"
#include "FrGrTreatement.h"
#include "MessageDefineFile.h"
#include "ErrorDefineFile.h"

#include "ImageInfoClass.h"
#include "ParametersInfoClass.h"
#include "H3Calculation.h"
#include "MeasureInfoClass.h"
#include "fde.h"
#include <ppl.h>

#include "FrGrTreatementWrapper.h"
//#define FRGR_DLL __declspec(dllexport)
#define FRGR_DLL

#define AMPLITUDE_IS_CONTRASTE 0
#define AMPLITUDE_IS_INTENSITY (1 - AMPLITUDE_IS_CONTRASTE)

//fct static////////////////////////////////////////////////////////////////////
static bool Copy(const H3_ARRAY2D_FLT32 & Im1, WORD* const pIm2, const float factor)
{
	long i=Im1.GetSize();
    float *pIm1 = Im1.GetData();
	WORD *pIm2_=pIm2;

	try{
		while(i--)
			(*(pIm2_++))=WORD( (*(pIm1++))*factor );
	}
	catch(...){
		SetLastTestError(FRGR_TREAT_COPY_ERROR);	
		return false;
	}

	return true;
}
/*-----------------------------------------------------------------------------
function    : Copy
description : copie une image (Flt32) en convertissant les données en niveaux de gris
in			: const H3_ARRAY2D_FLT32 & Im1: image à convertir en WORD
Out			: Im2 : image resultat (allouer AVANT d'entrer dans la fonction)
in			: const float min: floattant converti en 0
in			: const float max: floattant converti en 255
return		: bool (false si pb allocation pointeur)
Status		: 
-----------------------------------------------------------------------------*/
static bool Copy(const H3_ARRAY2D_FLT32 & Im1, H3_ARRAY2D_UINT16& Im2, const float min, const float max)
{
	long i=Im1.GetSize();
	float *pIm1=Im1.GetData();
    WORD *pIm2_ = Im2.GetData();
    float factor=255.0f/(max-min);

	try
	{
		while(i--)
		{
			float fTmp = (*(pIm1++));
			fTmp=__max(fTmp,min);
			fTmp=__min(fTmp,max);
			fTmp -= min;
			fTmp *=factor;

			WORD wTmp = WORD(fTmp);
			(*(pIm2_++))=wTmp;//WORD( ((*(pIm1++))-min)*factor );
		}
	}
	catch(...){
		SetLastTestError(FRGR_TREAT_COPY_ERROR);	
		return false;
	}

	return true;
}
/*-----------------------------------------------------------------------------
function    : Copy
description : copie une image (Flt32) en convertissant les données en niveaux de gris
in			: const H3_ARRAY2D_FLT32 & Im1: image à convertir en WORD
in			: const H3_ARRAY2D_UINT8  & Mask: masque appliqué sur l'image resultat (0 dans Mask => 0 dans resultat)
Out			: Im2: image resultat (allouer AVANT d'entrer dans la fonction)
in			: const float min: floattant converti en 0
in			: const float max: floattant converti en 255
return		: bool (false si pb allocation pointeur)
warning		:	Im1, Mask et *pIm2 doivent avoir la meme taille
Status		: 
-----------------------------------------------------------------------------*/
static void Copy(const H3_ARRAY2D_FLT32& Im1, const H3_ARRAY2D_UINT8& Mask, H3_ARRAY2D_UINT16& Im2, float fmin, float fmax)
{
    size_t imgSize = Im1.GetSize();
    float factor = 255.0f / (fmax - fmin);

    //for (size_t i = 0; i < imgSize; i++)
    concurrency::parallel_for(size_t(0), imgSize, [&](size_t i)
        {
            WORD wTmp;

            //Si l'on n'est pas dans la plaque via un test masque
            if (Mask[i] == 0L)
            {
                wTmp = 0L;
            }
            else
            {
                //Nous sommes dans la plaque
                float fTmp = Im1[i];
                fTmp = __max(fTmp, fmin);
                fTmp = __min(fTmp, fmax);
                fTmp -= fmin;
                fTmp *= factor;

                wTmp = (WORD)fTmp;
                wTmp = max(3, wTmp);
            }

            Im2[i] = wTmp;
        });
}


// Special version of function "Copy" for curvature with tool matching calibration
// fCalibrationDynamicsCoeff: Noise level of the calibration wafer, obtained by calibration of current machine (for tool matching)
// iNoiseGrayLevel: Gray level that must represent this calibrated noise level.
// fUserDynamicsCoeff: additional term from user, for special defects. Must be >0.
static void ConvertCurvatureToImage(const H3_ARRAY2D_FLT32& Im1, const H3_ARRAY2D_UINT8& Mask, H3_ARRAY2D_UINT16& Im2, float fCalibrationDynamicsCoeff, unsigned int nNoiseGrayLevel, float fUserDynamicsCoeff)
{
    size_t imgSize = Im1.GetSize();

    // We assume that the image background is at 0.
    // 
    //for (size_t i = 0; i < imgSize; i++)
    concurrency::parallel_for(size_t(0), imgSize, [&](size_t i)
        {
            WORD wTmp;

            //Si l'on n'est pas dans la plaque via un test masque
            if (Mask[i] == 0L)
            {
                wTmp = 0L;
            }
            else
            {
                //Nous sommes dans la plaque
                // Scaling based on the calibrated noise level for current machine, that we want to reach the value of iNoiseGrayLevel in the final image.
                
                // Using dynamics calibration to adjust background noise:
                float fTmp = Im1[i] / fCalibrationDynamicsCoeff * (float)nNoiseGrayLevel / fUserDynamicsCoeff + 128.0f;
                
                fTmp = __max(fTmp, 0.0f);
                fTmp = __min(fTmp, 255.0f);

                wTmp = (WORD)fTmp;
                wTmp = max(3, wTmp);
            }

            Im2[i] = wTmp;
        });
}


////////////////////////////////////////////////////////////////////////////////

CFrGrTreatement::CFrGrTreatement(void)
{
}

void CFrGrTreatement::AssociateWrapper(CFrGrTreatementWrapper* wrapper)
{
	m_wrapper = wrapper;
}

void CFrGrTreatement::ReportProgress(int step, const char* msg)
{
    //TODO FDE Faire une vraie callback qui envoie le progress à l'appli
    static std::chrono::time_point<std::chrono::high_resolution_clock> begin, end;

    if (step == 0)
    {
        begin = std::chrono::high_resolution_clock::now();
        printf("%s\n", msg);
    }
    else
    {
        end = std::chrono::high_resolution_clock::now();
        double elapsed = std::chrono::duration_cast<std::chrono::nanoseconds>(end - begin).count() / 1E9;
        printf("%s : %.3fs\n", msg, elapsed);
        begin = end;
    }
}

CFrGrTreatement::~CFrGrTreatement(void)
{
    for (auto it = m_ResultImages.begin(); it != m_ResultImages.end(); ++it)
    {
        auto ptr = it->second;
        delete it->second;
    }
}

/*-----------------------------------------------------------------------------
function    : CFrGrTreatement::FrGr_GetLastError
description : return the last process error with string format
in			: -
Out			: -
return		: int = Error number
Status		: Call this function after receive MessageError for 
-----------------------------------------------------------------------------*/

//FRGR_DLL void CFrGrTreatement::FrGr_GetLastError(CString* sErrorMessage)
//{
//	sErrorMessage->Format("%d : %s", m_uiError, m_strErrorMessage);
//}

/*-----------------------------------------------------------------------------
function    : 
description : 
in			: 
Out			: -
return		: -
Status		: 
-----------------------------------------------------------------------------*/

FRGR_DLL bool CFrGrTreatement::FrGr_PerformCalculation(CImageInfoClassInput* pIIC)
{
    m_iProgressState = 0;
    SetLastTestError(WM_FRGR_NO_ERROR);
    ReportProgress(m_iProgressState++, "Starting calculation");

    // Calcul Intensité/Phase/Contraste en X
    //......................................
    H3CalcX.Allocate(pIIC->GetSizeY(), pIIC->GetSizeX(), pIIC->GetNbImX(), pIIC->GetNbPeriods());
    H3CalcX.m_wrapper = m_wrapper;

    if (!H3CalcX.ComputePhases(pIIC->m_Images))
        return false;
    if (!H3CalcX.LinkMasks())
        return false;
    pIIC->FreePartialData('X');
    ReportProgress(m_iProgressState++, "PhaseX computed");

    if (m_wrapper->m_TypeOfFrame & AmplitudeX)
        SaveAmplitude(AmplitudeX, H3CalcX);

    if (m_wrapper->m_bSlope || m_wrapper->m_bKeepPhaseAndMask)
    {
        SaveMask(PhaseMask, H3CalcX);
    }

    if (m_wrapper->m_bKeepPhaseAndMask)
    { 
        SavePhases(PhaseX, H3CalcX);
    }

    // Dépliement des Phases en X
    //...........................
    if (m_wrapper->m_TypeOfFrame & UnwrappedPhaseX)
    {
        UnwrapPhase(UnwrappedPhaseX, H3CalcX);
        ReportProgress(m_iProgressState++, "PhaseX unwrapped");
    }

    // Calcul Intensité/Phase/Contraste en Y
    //......................................
    H3CalcY.Allocate(pIIC->GetSizeY(), pIIC->GetSizeX(), pIIC->GetNbImY(), pIIC->GetNbPeriods());
    H3CalcY.m_wrapper = m_wrapper;

    std::vector<BYTE*> imagesY(pIIC->m_Images.begin() + pIIC->GetNbImX() * pIIC->GetNbPeriods(), pIIC->m_Images.end());
    if (!H3CalcY.ComputePhases(imagesY))
        return false;
    if (!H3CalcY.LinkMasks())
        return false;
    pIIC->FreePartialData('Y');
    ReportProgress(m_iProgressState++, "PhaseY computed");

    // Dark by the method of average-amplitude
    if (m_wrapper->m_TypeOfFrame & Dark)
        ComputeDark(Dark, H3CalcX, H3CalcY);
    if (m_wrapper->m_TypeOfFrame & AmplitudeY)
        SaveAmplitude(AmplitudeY, H3CalcY);
    if (m_wrapper->m_bKeepPhaseAndMask)
        SavePhases(PhaseY, H3CalcY);

    // Dépliement des Phases en Y
    //...........................
    if (m_wrapper->m_TypeOfFrame & UnwrappedPhaseX)
    {
        UnwrapPhase(UnwrappedPhaseY, H3CalcY);
        ReportProgress(m_iProgressState++, "PhaseY unwrapped");
    }

    // Calcul de la Dérivée X
    //.......................
    if (m_wrapper->m_TypeOfFrame & CurvatureX)
    {
        if (!H3CalcX.ComputeDerivX())
            return false;

        SaveCurvature(CurvatureX, H3CalcX);
        ReportProgress(m_iProgressState++, "dX computed");
    }

    // Calcul de la dérivée en Y
    //..........................
    if (m_wrapper->m_TypeOfFrame & CurvatureY)
    {
        if (!H3CalcY.ComputeDerivY())
            return false;

        SaveCurvature(CurvatureY, H3CalcY);
        ReportProgress(m_iProgressState++, "dY computed");

        // Temporary specific code for curvature calibration!!! 
        //float fBackground = CalibrateCuvatureDynamics(H3CalcX, H3CalcY);
    }

    return true;
}

FRGR_DLL float CFrGrTreatement::FrGr_PerformCurvatureCalibration(CImageInfoClassInput* pIIC)
{
    m_iProgressState = 0;
    SetLastTestError(WM_FRGR_NO_ERROR);
    ReportProgress(m_iProgressState++, "Starting calibration");

    // Calcul Intensité/Phase/Contraste en X
    //......................................
    H3CalcX.Allocate(pIIC->GetSizeY(), pIIC->GetSizeX(), pIIC->GetNbImX(), pIIC->GetNbPeriods());
    H3CalcX.m_wrapper = m_wrapper;

    if (!H3CalcX.ComputePhases(pIIC->m_Images))
        return 0;
    if (!H3CalcX.LinkMasks())
        return 0;
    pIIC->FreePartialData('X');
    ReportProgress(m_iProgressState++, "PhaseX computed");

    /*if (m_wrapper->m_TypeOfFrame & AmplitudeX)
        SaveAmplitude(AmplitudeX, H3CalcX);*/

   /* if (m_wrapper->m_bSlope || m_wrapper->m_bKeepPhaseAndMask)
    {
        SaveMask(PhaseMask, H3CalcX);
    }*/

   /* if (m_wrapper->m_bKeepPhaseAndMask)
    {
        SavePhases(PhaseX, H3CalcX);
    }*/

    // Dépliement des Phases en X
    //...........................
  /*  if (m_wrapper->m_TypeOfFrame & UnwrappedPhaseX)
    {
        UnwrapPhase(UnwrappedPhaseX, H3CalcX);
        ReportProgress(m_iProgressState++, "PhaseX unwrapped");
    }*/

    // Calcul Intensité/Phase/Contraste en Y
    //......................................
    H3CalcY.Allocate(pIIC->GetSizeY(), pIIC->GetSizeX(), pIIC->GetNbImY(), pIIC->GetNbPeriods());
    H3CalcY.m_wrapper = m_wrapper;

    std::vector<BYTE*> imagesY(pIIC->m_Images.begin() + pIIC->GetNbImX() * pIIC->GetNbPeriods(), pIIC->m_Images.end());
    if (!H3CalcY.ComputePhases(imagesY))
        return 0;
    if (!H3CalcY.LinkMasks())
        return 0;
    pIIC->FreePartialData('Y');
    ReportProgress(m_iProgressState++, "PhaseY computed");

    
   /* if (m_wrapper->m_TypeOfFrame & AmplitudeY)
        SaveAmplitude(AmplitudeY, H3CalcY);
    if (m_wrapper->m_bKeepPhaseAndMask)
        SavePhases(PhaseY, H3CalcY);*/

    // Dépliement des Phases en Y
    //...........................
   /* if (m_wrapper->m_TypeOfFrame & UnwrappedPhaseX)
    {
        UnwrapPhase(UnwrappedPhaseY, H3CalcY);
        ReportProgress(m_iProgressState++, "PhaseY unwrapped");
    }*/

    // Calcul de la Dérivée X
    //.......................
    if (m_wrapper->m_TypeOfFrame & CurvatureX)
    {
        if (!H3CalcX.ComputeDerivX())
            return 0;

        SaveCurvature(CurvatureX, H3CalcX);
        ReportProgress(m_iProgressState++, "dX computed");
    }

    // Calcul de la dérivée en Y
    //..........................
    if (m_wrapper->m_TypeOfFrame & CurvatureY)
    {
        if (!H3CalcY.ComputeDerivY())
            return 0;

        SaveCurvature(CurvatureY, H3CalcY);
        ReportProgress(m_iProgressState++, "dY computed");

      
        float fBackground = CalibrateCuvatureDynamics(H3CalcX, H3CalcY);
        return fBackground;
    }

    return 0;
}




/*-----------------------------------------------------------------------------
function    :
description : Multiperiod deflectometry
in			:
Out			: -
return		: -
Status		:
-----------------------------------------------------------------------------*/
FRGR_DLL bool CFrGrTreatement::FrGr_PerformIncrementalCalculation(CImageInfoClassInput* pIIC, int period, char direction)
{
    ReportProgress(0, "Starting computation");
    if (direction == 'X')
    {
        // Calcul Intensité/Phase/Contraste en X
        //......................................
        if (period == 0)
        {
            m_iProgressState = 1;
            if (!H3CalcX.Allocate(pIIC->GetSizeY(), pIIC->GetSizeX(), pIIC->GetNbImX(), pIIC->GetNbPeriods()))
                return false;
        }
        H3CalcX.m_wrapper = m_wrapper;

        if (!H3CalcX.ComputePhases(pIIC->m_Images, period))
            return false;
        pIIC->FreePartialData(period, direction);

        // Calcul du Mask en X
        //....................
        if (period == 0)
        {
            if (!H3CalcX.LinkMasks())
                return false;

            if (m_wrapper->m_TypeOfFrame & AmplitudeX)
                SaveAmplitude(AmplitudeX, H3CalcX);
            if (m_wrapper->m_bKeepPhaseAndMask)
                SaveMask(PhaseMask, H3CalcX);
        }
        if (m_wrapper->m_bKeepPhaseAndMask)
            SavePhase(PhaseX, H3CalcX, period);
        ReportProgress(m_iProgressState++, "PhaseX initialized");

        // Calcul de la Dérivée X
        //.......................
        if (period == 0)
        {
            if (m_wrapper->m_TypeOfFrame & CurvatureX)
            {
                if (!H3CalcX.ComputeDerivX())
                    return false;

                SaveCurvature(CurvatureX, H3CalcX);
                ReportProgress(m_iProgressState++, "dX computed");
            }
        }

        // Dépliement des Phases en X
        //...........................
        if (pIIC->GetNbPeriods() > 1 && period == pIIC->GetNbPeriods() - 1)
        {
            if (m_wrapper->m_TypeOfFrame & UnwrappedPhaseX)
            {
                UnwrapPhase(UnwrappedPhaseX, H3CalcX);
                ReportProgress(m_iProgressState++, "PhaseX unwrapped");
            }
        }
    }
    else if (direction == 'Y')
    {
        // Calcul Intensité/Phase/Contraste en Y
        //......................................
        if (period == 0)
        {
            if (!H3CalcY.Allocate(pIIC->GetSizeY(), pIIC->GetSizeX(), pIIC->GetNbImY(), pIIC->GetNbPeriods()))
                return false;
        }
        H3CalcY.m_wrapper = m_wrapper;

        std::vector<BYTE*> images(pIIC->m_Images.begin() + pIIC->GetNbImX() * pIIC->GetNbPeriods(), pIIC->m_Images.end());
        if (!H3CalcY.ComputePhases(images, period))
            return false;
        pIIC->FreePartialData(period, direction);

        // Calcul du Mask en Y
        //....................
        if (period == 0)
        {
            if (!H3CalcY.LinkMasks())
                return false;

            // Dark image according to the method of sinusoid estimation B-A
            if (m_wrapper->m_TypeOfFrame & Dark)
                ComputeDark(Dark, H3CalcX, H3CalcY);
            if (m_wrapper->m_TypeOfFrame & AmplitudeY)
                SaveAmplitude(AmplitudeY, H3CalcY);
        }
        if (m_wrapper->m_bKeepPhaseAndMask)
            SavePhase(PhaseY, H3CalcY, period);
        ReportProgress(m_iProgressState++, "PhaseY initialized");

        // Calcul de la dérivée en Y
        //..........................
        if (period == 0)
        {
            if (m_wrapper->m_TypeOfFrame & CurvatureY)
            {
                if (!H3CalcY.ComputeDerivY())
                    return false;

                SaveCurvature(CurvatureY, H3CalcY);
                ReportProgress(m_iProgressState++, "dY computed");

                // Temporary specific code for curvature calibration!!! 
                //float fBackground = CalibrateCuvatureDynamics(H3CalcX, H3CalcY);
            }
        }

        // Dépliement des Phases en Y
        //...........................
        if (pIIC->GetNbPeriods() > 1 && period == pIIC->GetNbPeriods() - 1)
        {
            if (m_wrapper->m_TypeOfFrame & UnwrappedPhaseX)
            {
                UnwrapPhase(UnwrappedPhaseY, H3CalcY);
                ReportProgress(m_iProgressState++, "PhaseY unwrapped");
            }
        }
    }
    else
    {
        SetLastTestError(H3_CALCULATION_CONSTRUCT_ERROR);
        return false;
    }

    return true;
}

/*-----------------------------------------------------------------------------
function    :
description : Recalcule les images de courbures/amplitude avec les nouveaux settings
in			:
Out			: -
return		: -
Status		:
-----------------------------------------------------------------------------*/
bool CFrGrTreatement::FrGr_RecalculateOutputs(TypeOfFrame typeOfFrame)
{
    m_iProgressState = 0;
    ReportProgress(m_iProgressState++, "Starting computation");

    if (typeOfFrame & AmplitudeX)
    {
        if (H3CalcX.m_Intensity.IsEmpty())
            return false;
        SaveAmplitude(AmplitudeX, H3CalcX);
        ReportProgress(m_iProgressState++, "AmplitudeX initialized");
    }

    if (typeOfFrame & CurvatureX)
    {
        if (H3CalcX.m_Deriv.IsEmpty())
            return false;
        SaveCurvature(CurvatureX, H3CalcX);
        ReportProgress(m_iProgressState++, "CurvatureX computed");
    }

    if (typeOfFrame & AmplitudeY)
    {
        if (H3CalcY.m_Intensity.IsEmpty())
            return false;
        SaveAmplitude(AmplitudeY, H3CalcY);
        ReportProgress(m_iProgressState++, "AmplitudeY initialized");
    }

    if (typeOfFrame & CurvatureY)
    {
        if (H3CalcY.m_Deriv.IsEmpty())
            return false;
        SaveCurvature(CurvatureY, H3CalcY);
        ReportProgress(m_iProgressState++, "CurvatureY computed");
    }

    return true;
}

/*-----------------------------------------------------------------------------
function    :
description : Retourne la liste des résultats.
              La fonction est incrémentale car elle ne retourne que les nouveaux résultats.
in			:
Out			: -
return		: -
Status		:
-----------------------------------------------------------------------------*/
bool CFrGrTreatement::FrGr_GetIncrementalResultList(TypeOfFrame pTypeOfFrame[], int pIndex[], int& nbResults)
{
    if (m_PartialResults.size() > nbResults)
        return false;

    nbResults = m_PartialResults.size();

    for (int i = 0; i < nbResults; i++)
    {
        pTypeOfFrame[i] = m_PartialResults[i].TypeOfFrame;
        pIndex[i] = m_PartialResults[i].Index;
    }

    m_PartialResults.clear();
    return true;
}

/*-----------------------------------------------------------------------------
function    : CFrGrTreatement::SetLastTestError
description : Private function. 
in			: -
Out			: -
return		: -
Status		: Call this function for set error message
-----------------------------------------------------------------------------*/

//void CFrGrTreatement::SetLastTestError(int aError)
//{
//	// TO_HOLO3 : implemente all message error 
//	switch (aError)
//	{
//	case NO_ERROR:
//		m_uiError = aError; 
//		m_strErrorMessage = MSG_NO_ERROR; 
//			break; 
//
//	default:
//		break; 
//	}
//}



/*-----------------------------------------------------------------------------
function    :
description : Private function.
in			: -
Out			: -
return		: -
Status		:
-----------------------------------------------------------------------------*/
void CFrGrTreatement::SaveAmplitude(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc)
{
    H3_ARRAY2D_UINT16* amplitude = new H3_ARRAY2D_UINT16(H3Calc.m_Mask_Save_Amplitude.GetLi(), H3Calc.m_Mask_Save_Amplitude.GetCo());
    m_ResultImages[typeOfFrame] = amplitude;
    m_PartialResults.push_back(Result(typeOfFrame));

#if AMPLITUDE_IS_INTENSITY
    Copy(H3Calc.m_Intensity, H3Calc.m_Mask_Save_Amplitude, *amplitude, 0, 255);//l'intensité est codée sur 8 bits
#else
    Copy(H3CalcX.m_Contraste, H3CalcX.m_Mask_Save_Amplitude, image, -0.2f, 1.2f);//le contraste est entre 0 et 1 si le signal est sinusoidale.-0.2 à 1.2 pour autoriser des distortions
#endif
}


/*-----------------------------------------------------------------------------
function    : 
description : Private function.
in			: -
Out			: -
return		: -
Status		: 
-----------------------------------------------------------------------------*/
void CFrGrTreatement::SaveCurvature(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc)
{
    H3_ARRAY2D_UINT16* curvature = new H3_ARRAY2D_UINT16(H3Calc.m_Mask_Save_Curvature.GetLi(), H3Calc.m_Mask_Save_Curvature.GetCo());
    m_ResultImages[typeOfFrame] = curvature;
    m_PartialResults.push_back(Result(typeOfFrame));

    ConvertCurvatureToImage(H3Calc.m_Deriv, H3Calc.m_Mask_Save_Curvature, *curvature, m_wrapper->m_curvatureConfig.fCalibratedNoise, m_wrapper->m_curvatureConfig.nTargetBackgroundGrayLevel, m_wrapper->m_curvatureConfig.fUserCurvatureDynamicsCoeff);
}

/*-----------------------------------------------------------------------------
function    :
description : Private function.
in			: -
Out			: -
return		: -
Status		:
-----------------------------------------------------------------------------*/
void CFrGrTreatement::SavePhases(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc)
{
    for (int i = 0; i < H3Calc.m_WrappedPhases.size(); i++)
        SavePhase(typeOfFrame, H3Calc, i);
}

void CFrGrTreatement::SavePhase(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc, int period)
{
    H3_ARRAY2D_FLT32* phase = new H3_ARRAY2D_FLT32();
    H3_ARRAY2D_FLT32::TransferDataOwnership(H3Calc.m_WrappedPhases[period], *phase);
    m_ResultImages[(TypeOfFrame)(typeOfFrame + period)] = phase;
    m_PartialResults.push_back(Result(typeOfFrame, period));
}

/*-----------------------------------------------------------------------------
function    :
description : Private function.
in			: -
Out			: -
return		: -
Status		:
-----------------------------------------------------------------------------*/
void CFrGrTreatement::SaveMask(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc)
{
    H3_ARRAY2D_UINT8* mask = new H3_ARRAY2D_UINT8();
    H3_ARRAY2D_UINT8::TransferDataOwnership(H3Calc.m_Mask, *mask);

    m_ResultImages[typeOfFrame] = mask;
    m_PartialResults.push_back(Result(typeOfFrame));
}

/*-----------------------------------------------------------------------------
function    :
description : Private function.
in			: -
Out			: -
return		: -
Status		:
-----------------------------------------------------------------------------*/
void CFrGrTreatement::UnwrapPhase(TypeOfFrame typeOfFrame, CH3Calculation& H3Calc)
{
    H3_ARRAY2D_FLT32 demodulatedPhase;

    bool bDemodulateMultiPeriod = (m_wrapper->m_Periods.size() != 1);
    CFrGrGlobalTopo* globalTopo = m_wrapper->GetGlobalTopo();

    // Méthode multipériode
    //.....................
    if (bDemodulateMultiPeriod)
    {
        // Unwrapping
        CH3Array<H3_ARRAY2D_FLT32> H3WrappedPhases(H3Calc.m_WrappedPhases.size());
        for (int i = 0; i < H3Calc.m_WrappedPhases.size(); i++)
            H3WrappedPhases[i].LinkData(H3Calc.m_WrappedPhases[i]);
        std::vector<float> ratios(m_wrapper->m_Periods.size());
        ratios[0] = 1;
        for (int i = 1; i < ratios.size(); i++)
            ratios[i] = (float)m_wrapper->m_Periods[i] / (float)m_wrapper->m_Periods[i - 1];

        demodulatedPhase = globalTopo->Demoduler(H3WrappedPhases, H3Calc.m_Mask , &ratios[0], m_wrapper->m_Periods.size());

        // Remove tilt
        //globalTopo->PlaneSubtraction(demodulatedPhase, H3Calc.m_Mask);
    }

    // Méthode par rapport à la référence
    //...................................
    else
    {
        // IBE: Be careful: X and Y correspond to dimension 2 (FDE:dimension 1) then dimension 1 (FDE:dimension 0) in H3_ARRAY2D_FLT32 images, so they are used in this order: (Y,X).
        H3_ARRAY2D_FLT32 modulatedPhase = H3Calc.m_Phase_Map_1.Trans();

        //TODO FDE pourquoi on ne transpose pas le mask ?

        //unsigned int nCalibFringesSize = 32;		// TODO: write it in the calib file and reload it here!!!
        float ratio = (float)m_wrapper->m_Periods[0] / m_wrapper->m_globalTopoConfig.FringePeriod;

        // Unwrap
        if (typeOfFrame == UnwrappedPhaseX)
            globalTopo->DemodulateFromReferenceX(modulatedPhase, H3Calc.m_Mask, ratio, demodulatedPhase);
        else
            globalTopo->DemodulateFromReferenceY(modulatedPhase, H3Calc.m_Mask, ratio, demodulatedPhase);

        // Subtract mean plane from phase
        //globalTopo->PlaneSubtraction(demodulatedPhase, H3Calc.m_Mask);

        // Transpose the image to fit with the following usage
        demodulatedPhase = demodulatedPhase.Trans();
    }

    // Stockage du résultat
    //.....................
    H3_ARRAY2D_FLT32* result = new H3_ARRAY2D_FLT32();
    H3_ARRAY2D_FLT32::TransferDataOwnership(demodulatedPhase, *result);
    m_ResultImages[typeOfFrame] = result;
    m_PartialResults.push_back(Result(typeOfFrame));
}


void CFrGrTreatement::ComputeDark(TypeOfFrame typeOfFrame, CH3Calculation& H3CalcX, CH3Calculation& H3CalcY)
{
    CFrGrGlobalTopo* globalTopo = m_wrapper->GetGlobalTopo();
    bool bNoGlobalTopo = false;
    if (globalTopo == nullptr)
    {
        globalTopo = new CFrGrGlobalTopo("DummyFolder");
        bNoGlobalTopo = true;
    }

    // Save result
    // The final dark is the average of both directions. It is stored in H3CalcX.
 
    for (unsigned int nL = 0; nL < H3CalcX.m_Dark.GetLi(); nL++)
    {
        for (unsigned int nC = 0; nC < H3CalcX.m_Dark.GetCo(); nC++)
        {
            H3CalcX.m_Dark(nL, nC) = (H3CalcX.m_Dark(nL, nC) + H3CalcY.m_Dark(nL, nC)) * 0.5f;
        }
    }

    // Flattening
    globalTopo->ShapeSubtraction(H3CalcX.m_Dark, H3CalcX.m_Mask, false);

    // Getting everything positive and uint8
    H3CalcX.m_Mask_Save_Dark.ReAlloc(H3CalcX.m_Dark.GetLi(), H3CalcX.m_Dark.GetCo());
    globalTopo->DarkPositiveInteger(H3CalcX.m_Dark, H3CalcX.m_Mask, H3CalcX.m_Mask_Save_Dark);

    H3_ARRAY2D_UINT8* dark = new H3_ARRAY2D_UINT8();
    H3_ARRAY2D_UINT8::TransferDataOwnership(H3CalcX.m_Mask_Save_Dark, *dark);

    m_ResultImages[typeOfFrame] = dark;
    m_PartialResults.push_back(Result(typeOfFrame));

    if (bNoGlobalTopo)
        delete globalTopo;

}

// The curvature calibration step consists in acquiring an image of the calibration wafer, computing the raw curvature maps and saving their background level. Input of the function: raw curvature maps including mask. Output of the function: background stdev.
// The output is the background standard deviation averaged on X and Y maps.
float CFrGrTreatement::CalibrateCuvatureDynamics(CH3Calculation& H3CalcX, CH3Calculation& H3CalcY)
{
    if (H3CalcX.m_Deriv.GetLi() != H3CalcY.m_Deriv.GetLi() || H3CalcX.m_Deriv.GetCo() != H3CalcY.m_Deriv.GetCo())
        return 0.0f;

    // Computing mean in the wafer. Aim: filtering to remove the abnormal pixels (patterns and dust) and keep only the background.
    // Important: the mean is assumed 0 due to function ApplyMaskAnLevel(), so it is not re-computed.
    float fAbsMeanInMaskX = 0.0f, fAbsMeanInMaskY = 0.0f;
    int iCount = 0;

    concurrency::parallel_for(size_t(0), H3CalcX.m_Deriv.GetLi(), [&](size_t nL)
    //for (unsigned int nL = 0; nL < H3CalcX.m_Deriv.GetLi(); nL++)
    {
        for (unsigned int nC = 0; nC < H3CalcX.m_Deriv.GetCo(); nC++)
        {
            if (H3CalcX.m_Mask(nL, nC))
            {
                fAbsMeanInMaskX += abs(H3CalcX.m_Deriv(nL, nC));
                fAbsMeanInMaskY += abs(H3CalcY.m_Deriv(nL, nC));
                iCount++;
            }
        }
    });

    fAbsMeanInMaskX /= (float)iCount;
    fAbsMeanInMaskY /= (float)iCount;

    // Standard deviation of the filtered background
    float fFilteredMeanX = 0.0f, fFilteredSquaredMeanX = 0.0f, fFilteredMeanY = 0.0f, fFilteredSquaredMeanY = 0.0f;
    int iCountX = 0, iCountY = 0;

    concurrency::parallel_for(size_t(0), H3CalcX.m_Deriv.GetLi(), [&](size_t nL)
        //for (unsigned int nL = 0; nL < H3CalcX.m_Deriv.GetLi(); nL++)
        {
            for (unsigned int nC = 0; nC < H3CalcX.m_Deriv.GetCo(); nC++)
            {
                // X map
                if (H3CalcX.m_Mask(nL, nC) && (H3CalcX.m_Deriv(nL, nC) > -3.0f * fAbsMeanInMaskX) && (H3CalcX.m_Deriv(nL, nC) < 3.0f * fAbsMeanInMaskX))
                {
                    fFilteredMeanX += H3CalcX.m_Deriv(nL, nC);
                    fFilteredSquaredMeanX += H3CalcX.m_Deriv(nL, nC) * H3CalcX.m_Deriv(nL, nC);
                    iCountX++;
                }

                // Y map
                if (H3CalcX.m_Mask(nL, nC) && (H3CalcY.m_Deriv(nL, nC) > -3.0f * fAbsMeanInMaskY) && (H3CalcY.m_Deriv(nL, nC) < 3.0f * fAbsMeanInMaskY))
                {
                    fFilteredMeanY += H3CalcY.m_Deriv(nL, nC);
                    fFilteredSquaredMeanY += H3CalcY.m_Deriv(nL, nC) * H3CalcY.m_Deriv(nL, nC);
                    iCountY++;
                }
            }
        });

    fFilteredMeanX /= iCountX;
    fFilteredMeanY /= iCountY;
    fFilteredSquaredMeanX /= iCountX;
    fFilteredSquaredMeanY /= iCountY;

    // Average standard deviation
    float fStdX = sqrt(fFilteredSquaredMeanX - fFilteredMeanX * fFilteredMeanX);
    float fStdY = sqrt(fFilteredSquaredMeanY - fFilteredMeanY * fFilteredMeanY);
    float fStd = 0.5f * (fStdX + fStdY);

    return fStd;
}
