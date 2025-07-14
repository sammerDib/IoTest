#pragma once

#include "H3IOHoloMAP.h"
#include "H3Array2D.h"
#include <vector>

class CFrGrTreatementWrapper;
const float fPIs2=(1.5707963267948966f);
const float fmPIs2=(-1.5707963267948966f);

class CH3Calculation
{
public:
	CH3Calculation(void);
	~CH3Calculation(void);

    bool Allocate(long ny, long nx, long nbFrames, long nbPeriods);
    bool ComputePhases(std::vector<BYTE*>& images);
    bool ComputePhases(std::vector<BYTE*>& images, int period);
    bool LinkMasks();
    bool ComputeDerivX();
    void ApplyMaskAndLevel();
	bool ComputeDerivY();

protected:
    void Compute_Mask(H3_ARRAY2D_UINT8&  mask, int iContrasteMin, int iIntensiteMin);

public:
    H3_ARRAY2D_UINT8 m_PhaseInterval;	    //1 si PhaseMap_1 entre -Pi/2 et Pi/2 
    H3_ARRAY2D_UINT8 m_Mask;				//1 si conditions respectées (intensité / contraste /...) 
    H3_ARRAY2D_UINT8 m_Mask_Save_Amplitude;	//1 si conditions respectées (intensité / contraste /...) 
    H3_ARRAY2D_UINT8 m_Mask_Save_Curvature;	//1 si conditions respectées (intensité / contraste /...) 
    H3_ARRAY2D_FLT32 m_Phase_Map_1;		    //phi. La phase dans [-Pi,Pi]
    H3_ARRAY2D_FLT32 m_Phase_Map_2;		    //phi. La phase dans [0,2Pi]
    H3_ARRAY2D_FLT32 m_Intensity;	        //I : l'intensité de chacune des images servant à calculer la phase est I[i]=I*(1+mcos(phi)) , i=0..nImage-1
    H3_ARRAY2D_FLT32 m_Contraste;		    //m: semi-amplitude of the fringe sinusoid
    H3_ARRAY2D_FLT32 m_Deriv;               // Dérivée en X ou Y
    H3_ARRAY2D_FLT32 m_Dark;                // Temporary "Dark" image for current direction with the new method: using estimations of amplitude and average of the sinusoid.
    H3_ARRAY2D_UINT8 m_Mask_Save_Dark;      // "Dark" image to save (only stored in the X map)
    std::vector<H3_ARRAY2D_FLT32> m_WrappedPhases;

	CFrGrTreatementWrapper* m_wrapper = NULL;

private:
    bool m_IsMaskInitialised = false;
    bool m_IsIntensityInitialised = false;
    bool m_IsContrasteInitialised = false;
    bool m_IsAllocated = false;

    unsigned long m_nx = 0; //le nombre de colonnes des images utilisées pour le calcul, et donc des resultats
	unsigned long m_ny = 0; //le nombre de lignes   des images utilisées pour le calcul, et donc des resultats
    int m_nbImagesPerPeriod = 0;  //le nombre d'images utilisées pour le calcul
    int m_nbPeriods = 0; //le nombre de périodes utilisées. Le nombre total d'images est m_nbImagesPerPeriod * m_nbPeriods
};
