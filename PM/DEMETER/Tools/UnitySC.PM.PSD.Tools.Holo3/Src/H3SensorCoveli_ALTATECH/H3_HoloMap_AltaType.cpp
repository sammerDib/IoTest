#include "stdafx.h"
#include "H3_HoloMap_AltaType.h"
#include "H3AppToolsdecl.h"
#include "h3array2d.h"
#include "H3Camera.h"
#include "Extrinsic_param.h"
#include "SCalibResult.h"
#include "H3ImageToolsDecl.h"
#include "H3UnwrapphaseDecl.h"

// These settings should be defined in the recipe!!!
constexpr auto DARKPERCENTAGEOFLOWSATURATION = 0.03;
constexpr auto DARKDYNAMICSCOEFF = 20;

CString strModule(_T("H3_HoloMap_AltaType"));

// demodule
// suppose que la phase W (qui est 'wrapped'=modulée) soit proche, une fois demodulée, de UW ('unwrapped') 
static H3_ARRAY2D_FLT32 demodule(const H3_ARRAY2D_FLT32 & W, const H3_ARRAY2D_FLT32 & UW,const H3_ARRAY2D_UINT8 & Mask)
{
	H3_ARRAY2D_FLT32 order=((UW-W)/fTWO_PI+0.5);
	order._floor();
	return (W+order*fTWO_PI);
}

//demodule
//input, plusieurs images modulées obtenues sur le meme objet (fixe) mais avec des reseaux différents mais de meme orientation
//la phase correspondant au reseau le plus fin est pW[0], le reseau le plus grossier est pW[N-1]
// toutes les images ont la meme taille (sinon big pb)
//input, un tableau contenant de N case dans la case i le facteur (pas reseau i)/(pas reseau i-1)
//dans la case 0 : 1 (pas lu)
static H3_ARRAY2D_FLT32 demodule(const CH3Array<H3_ARRAY2D_FLT32> & W, const CH3Array2D<unsigned char> & Mask, const float ratio[], const unsigned long N)
{
	if(N==0)
		return (W[0]);

	const size_t nLi=W[0].GetLi(),nCo=W[0].GetCo();
	H3_ARRAY2D_FLT32 UW=(W[0]), order;

	size_t i;
	float factor=1.0, old_factor=1.0;

    long nCol = (long)Mask.GetCo();

    order.Alloc(UW.GetLi(), UW.GetCo());
    for (i = 1; i < N; i++) {
        old_factor = factor;
        factor *= ratio[i];
        float old_factor_2PI = old_factor * (float)TWO_PI;
        //for (long j = order.GetSize() - 1; j >= 0; j--)
        concurrency::parallel_for((long)0, nCol, [&](long jx)
            {
                for (int jy = 0; jy < Mask.GetLi(); jy++)
                {
                    long nIndex = jy * nCol + jx;

                    if (Mask(jy, jx))
                    {
                        order[nIndex] = W[i][nIndex] * factor;
                        order[nIndex] -= UW[nIndex];
                        order[nIndex] /= old_factor_2PI;
                        order[nIndex] += 0.5;
                        order[nIndex] = floor(order[nIndex]);

                        UW[nIndex] += order[nIndex] * old_factor_2PI;
                    }
                    else
                        UW[nIndex] = NaN;
                }
            }
        );
    }
    return UW;
}

CH3_HoloMap_AltaType::CH3_HoloMap_AltaType(const CString& calibFolder) :
	m_data_4calibonly(nullptr),
	isInitialised(false),
	m_pH3_HoloMap_Base(nullptr)
{
    m_CalibFolder = calibFolder;
	m_nErrorTypeCalibSys=-1L;
	m_nErrorTypeMeasure= -1L;

	m_PhaseMireHMAP2mm =new float[2];
	m_PhaseMireHMAP2mm[0]=m_PhaseMireHMAP2mm[1]=NaN;

	m_phi_onPixScreenRef= new float[2];
	m_phi_onPixScreenRef[0]=m_phi_onPixScreenRef[1]=NaN;

	m_Pix_ref[0]= m_Pix_ref[1]= -1;//-1 >> size_t max
	Alloc();
}

CH3_HoloMap_AltaType::~CH3_HoloMap_AltaType(void)
{
	Free();
	isInitialised=false;
}


int CH3_HoloMap_AltaType::GetRefSizeCo() const 
{
	return m_UWXref.GetCo();
}
int CH3_HoloMap_AltaType::GetRefSizeLi() const 
{
	return m_UWXref.GetLi();
}


bool CH3_HoloMap_AltaType::SaveSettings(const CString & strFileName,CString strSection)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("SaveSettings()");
	CString strMsg("Impossible d'enregister le fichier de configuration.");
	H3DebugInfo(strModule,strFunction,"");

	if (strFileName.IsEmpty())
		return false;
	
	if(nullptr==m_data_4calibonly)
		return false;

	// Savegarde des parametres
	//...
	strSection = _T("SensorHoloMap3");
	H3WritePrivProfileInt(strSection,"PrefX",(int)m_Pix_ref[0],strFileName);
	H3WritePrivProfileInt(strSection,"PrefY",(int)m_Pix_ref[1],strFileName);

	strSection = strSection+_T("_SensorScreen");
	H3WritePrivProfileFloat(strSection,"PitchX",m_data_4calibonly->m_fPixSizeX,strFileName);
	H3WritePrivProfileFloat(strSection,"PitchY",m_data_4calibonly->m_fPixSizeY,strFileName);
	H3WritePrivProfileFloat(strSection,"PeriodX",m_data_4calibonly->m_fMireMonStepX,strFileName);
	H3WritePrivProfileFloat(strSection,"PeriodY",m_data_4calibonly->m_fMireMonStepY,strFileName);
	H3WritePrivProfile(strSection,"pixRef_Xscreen", (long)m_data_4calibonly->m_pixRef_Xscreen,strFileName);
	H3WritePrivProfile(strSection,"pixRef_Yscreen", (long)m_data_4calibonly->m_pixRef_Yscreen,strFileName);
	H3WritePrivProfile(strSection,"screen_Xsz", (long)m_data_4calibonly->m_screen_Xsz,strFileName);
	H3WritePrivProfile(strSection,"screen_Ysz", (long)m_data_4calibonly->m_screen_Ysz,strFileName);

	return true;
}

bool CH3_HoloMap_AltaType::LoadSettings1()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("LoadSettings1()");
	CString strMsg("Impossible de charger le fichier de configuration.");
	H3DebugInfo(strModule,strFunction,"");
	CString str;

    CString strFileName = m_CalibFolder + "\\" + _CalibPaths._InputSettingsFile;
    CString strSection = "SensorHoloMap3";
	if (strFileName.IsEmpty())
	{
		H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::LoadSettings1  Impossible de charger le fichier de configuration.");
		// Erreur Code 10
		m_nErrorTypeCalibSys = 10;
		return false;
	}

	H3DebugInfo(strModule,strFunction,strFileName);

	// Lecture des parametres
	//...
    m_CH3Camera_C_File = _CalibPaths.CamCalibIntrinsicParamsPath(m_CalibFolder);
    m_CExtrinsic_param_ep_ObjRef_CamFrame_File = _CalibPaths.MatrixWaferToCameraPath(m_CalibFolder);

    m_c_OutFiles.Res1 = _CalibPaths.MatrixScreenToCamPath(m_CalibFolder);
    m_c_OutFiles.ResX = _CalibPaths.PhaseReferenceXPath(m_CalibFolder);
    m_c_OutFiles.ResY = _CalibPaths.PhaseReferenceYPath(m_CalibFolder);

    m_Pix_ref[0] = H3GetPrivProfileInt(strSection, "PrefX", strFileName);
    m_Pix_ref[1] = H3GetPrivProfileInt(strSection, "PrefY", strFileName);

	m_MireHMAP.LoadSettings2(strFileName,strSection+_T("_SensorScreen"));

	if(	m_CH3Camera_C_File.IsEmpty() || m_CExtrinsic_param_ep_ObjRef_CamFrame_File.IsEmpty() || m_c_OutFiles.Res1.IsEmpty() || m_c_OutFiles.ResX.IsEmpty() || 
		m_c_OutFiles.ResY.IsEmpty()    || m_Pix_ref[0]==-1 || m_Pix_ref[1]==-1 )
	{
		if(m_CH3Camera_C_File.IsEmpty())
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::LoadSettings1  Le chemin d'accès au fichier CalibCam_0.txt n'est pas renseigné dans le fichier SensorData.txt.");
			// Erreur Code 1
			m_nErrorTypeCalibSys = 1;
			return false;
		}
		if(m_CExtrinsic_param_ep_ObjRef_CamFrame_File.IsEmpty())
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::LoadSettings1  Le chemin d'accès au fichier EP_ref_CamFrame.txt n'est pas renseigné dans le fichier SensorData.txt.");
			// Erreur Code 2
			m_nErrorTypeCalibSys = 2;
			return false;
		}
		if(m_c_OutFiles.Res1.IsEmpty())
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::LoadSettings1  Le chemin d'accès au fichier Res1.txt n'est pas renseigné dans le fichier SensorData.txt.");
			// Erreur Code 21
			m_nErrorTypeCalibSys = 21;
			return false;
		}
		if(m_c_OutFiles.ResX.IsEmpty())
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::LoadSettings1  Le chemin d'accès au fichier ResX.klib n'est pas renseigné dans le fichier SensorData.txt.");
			// Erreur Code 22
			m_nErrorTypeCalibSys = 22;
			return false;
		}
		if(m_c_OutFiles.ResY.IsEmpty())
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::LoadSettings1  Le chemin d'accès au fichier ResY.klib n'est pas renseigné dans le fichier SensorData.txt.");
			// Erreur Code 23
			m_nErrorTypeCalibSys = 23;
			return false;
		}
	}
	return true;
}

H3_ARRAY2D_FLT32 CH3_HoloMap_AltaType::Demoduler(const CH3Array<H3_ARRAY2D_FLT32> & pW, const CH3Array2D<unsigned char> pMask, const float ratio[], const unsigned long N) const
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("Demoduler()");
	CString strMsg("");
	H3DebugInfo(strModule,strFunction,"");

	return demodule(pW, pMask, ratio,N);
}


// IBE: demodulation from a reference phase
void CH3_HoloMap_AltaType::DemodulateFromReferenceX(const H3_ARRAY2D_FLT32 & WPhaseX, const H3_ARRAY2D_UINT8 & aMask, float Ratio_ActualPixelPerPeriod_divby_CalibOne, H3_ARRAY2D_FLT32& aResult) const
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("DemodulateFromReferenceX()");
	CString strMsg("");
	H3DebugInfo(strModule, strFunction, "");

	aResult = demodule(WPhaseX, m_UWXref / Ratio_ActualPixelPerPeriod_divby_CalibOne, aMask)*Ratio_ActualPixelPerPeriod_divby_CalibOne;

	// Use the mask
	unsigned int nD1, nD2;
	for (nD1 = 0; nD1 < aMask.GetLi(); nD1++)
	{
		for (nD2 = 0; nD2 < aMask.GetCo(); nD2++)
		{
			aResult.SetAt(nD1, nD2, aMask.GetAt(nD1, nD2) ? aResult.GetAt(nD1, nD2) : NaN);
		}
	}
}


void CH3_HoloMap_AltaType::DemodulateFromReferenceY(const H3_ARRAY2D_FLT32 & WPhaseY, const H3_ARRAY2D_UINT8 & aMask, float Ratio_ActualPixelPerPeriod_divby_CalibOne, H3_ARRAY2D_FLT32& aResult) const
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("DemodulateFromReferenceY()");
	CString strMsg("");
	H3DebugInfo(strModule, strFunction, "");

	aResult = demodule(WPhaseY, m_UWYref / Ratio_ActualPixelPerPeriod_divby_CalibOne, aMask) * Ratio_ActualPixelPerPeriod_divby_CalibOne;

	// Use the mask
	unsigned int nD1, nD2;
	for (nD1 = 0; nD1 < aMask.GetLi(); nD1++)
	{
		for (nD2 = 0; nD2 < aMask.GetCo(); nD2++)
		{
			aResult.SetAt(nD1, nD2, aMask.GetAt(nD1, nD2) ? aResult.GetAt(nD1, nD2) : NaN);
		}
	}
}


// IBE: demodulation using a spatial method
H3_ARRAY2D_FLT32 CH3_HoloMap_AltaType::DemodulateSpatially(const H3_ARRAY2D_FLT32 & WPhase, const H3_ARRAY2D_UINT8 & aMask) const
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("DemodulateSpatially()");
	CString strMsg("");
	H3DebugInfo(strModule, strFunction, "");

	long i_med=WPhase.GetLi()/2, j_med=WPhase.GetCo()/2, i_mean=0, j_mean=0, nValid=0;
		for(size_t  i=0, j, k=0; i< WPhase.GetLi(); ++i)
			for(j=0; j< WPhase.GetCo(); ++j, ++k)
				//if(aMask[k])
				{
					i_mean += (i-i_med);
					j_mean += (j-j_med);
					nValid++;
				}

		/*if(	nValid == 0)
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Mesurer  le mask n'indique pas de point valide");
			// Erreur Code 3
			m_nErrorTypeMeasure = 3;
			return 3;
		}*/

		i_mean/= nValid; i_mean+= i_med;
		j_mean/= nValid; j_mean+= j_med;

		// Unwrapping
		return H3UnwrapPhase( WPhase, aMask, i_mean, j_mean, (long)0);
}


// IBE
// Removing the mean polynom of order 3 from the unwrapped phase, result goes in the same table.
// The function uses Holo3 methods.
// If bMeanToZero is true, it also subtracts the mean value of the result. Otherwise, it brings all values positive.
void CH3_HoloMap_AltaType::ShapeSubtraction(H3_ARRAY2D_FLT32& InOutTable, const H3_ARRAY2D_UINT8& Mask, const bool bMeanToZero) const
{
    unsigned int nNbLi = InOutTable.GetLi(), nNbCo = InOutTable.GetCo();

    // Downsampling
    size_t nDownSampFactor = 5;
    unsigned int nDownSizeLi = Mask.GetLi() / nDownSampFactor;
    unsigned int nDownSizeCo = Mask.GetCo() / nDownSampFactor;

    H3_ARRAY2D_FLT32 DownTable(nDownSizeLi, nDownSizeCo);
    H3_ARRAY2D_UINT8 DownMask(nDownSizeLi, nDownSizeCo);

    for (unsigned int l = 0; l < nDownSizeLi; l++)
    {
        for (unsigned int c = 0; c < nDownSizeCo; c++)
        {
            DownTable(l, c) = InOutTable(l * nDownSampFactor, c * nDownSampFactor);
            DownMask(l, c) = Mask(l * nDownSampFactor, c * nDownSampFactor);
        }
    }

    // Fit polynom of order 3
    //..........
    H3_ARRAY2D_FLT64 MatResSurf(4, 4);
    H3BestFitSurf2(DownTable, DownMask, MatResSurf, 3L);

    // Adaptation to new sampling
    // MatResSurf(0) is the constant term, not normalized.
    MatResSurf(1) = MatResSurf(1) / (float)nDownSampFactor;
    MatResSurf(2) = MatResSurf(2) / (float)nDownSampFactor / (float)nDownSampFactor;
    MatResSurf(3) = MatResSurf(3) / pow((float)nDownSampFactor, 3);

    MatResSurf(4) = MatResSurf(4) / (float)nDownSampFactor;
    MatResSurf(5) = MatResSurf(5) / (float)nDownSampFactor / (float)nDownSampFactor;
    MatResSurf(6) = MatResSurf(6) / pow((float)nDownSampFactor, 3);
    MatResSurf(7) = MatResSurf(7) / pow((float)nDownSampFactor, 4);
    
    MatResSurf(8) = MatResSurf(8) / (float)nDownSampFactor / (float)nDownSampFactor;
    MatResSurf(9) = MatResSurf(9) / pow((float)nDownSampFactor, 3);
    MatResSurf(10) = MatResSurf(10) / pow((float)nDownSampFactor, 4);
    MatResSurf(11) = MatResSurf(11) / pow((float)nDownSampFactor, 5);

    MatResSurf(12) = MatResSurf(12) / pow((float)nDownSampFactor, 3);
    MatResSurf(13) = MatResSurf(13) / pow((float)nDownSampFactor, 4);
    MatResSurf(14) = MatResSurf(14) / pow((float)nDownSampFactor, 5);
    MatResSurf(15) = MatResSurf(15) / pow((float)nDownSampFactor, 6);

    // Build the obtained surface
    //.........................
    MatResSurf = MatResSurf.Trans();
    H3_ARRAY2D_FLT32 ComputedShape(nNbLi, nNbCo);
    H3PolySurf(ComputedShape, H3_ARRAY2D_FLT32(MatResSurf), nNbCo, nNbLi);

    // Subtract shape
    //...............
    InOutTable -= ComputedShape;

    // Subtract the mean value of the center area
    // (assuming there are positive and negative slopes around a background that should be set to 0)
    //..............................................................................................
    if (bMeanToZero)
    {
        // Finding the mean value:
        /*double dSum = 0.0;
        unsigned int nCount = 0;
        for (unsigned int l = 0; l < mask_nb_l; l++)
        {
            for (unsigned int c = 0; c < mask_nb_c; c++)
            {
                float val = InOutTable(l + mask_lmin, c + mask_cmin);
                if (!_isnan(val))
                {
                    dSum += val;
                    nCount++;
                }
            }
        }
        dSum /= nCount;*/

        // TODO: check it works:
        double dSum = 0.0;
        unsigned int nCount = 0;
        for (unsigned int l = 0; l < Mask.GetLi(); l++)
        {
            for (unsigned int c = 0; c < Mask.GetCo(); c++)
            {
                float val = InOutTable(l, c);
                if (!_isnan(val) && Mask(l,c)==1)
                {
                    dSum += val;
                    nCount++;
                }
            }
        }
        dSum /= nCount;

        // Subtraction
        //TODO FDE on peut faire comme ça mais le calcul est alors en float au lieu de double et le résultat change légèrement
        //UnwrappedPhase -= (float)dSum;

        for (unsigned int l = 0; l < nNbLi; l++)
        {
            for (unsigned int c = 0; c < nNbCo; c++)
                InOutTable(l, c) = (float)(InOutTable(l, c) - dSum);
        }
    }

}


// IBE
// Removing the mean plane from the unwrapped phase
// The function uses Holo3 methods.
// It also subtracts the mean value of the result.
void CH3_HoloMap_AltaType::PlaneSubtraction(H3_ARRAY2D_FLT32& UnwrappedPhase, const H3_ARRAY2D_UINT8& Mask) const
{
    unsigned int nNbLi = UnwrappedPhase.GetLi(), nNbCo = UnwrappedPhase.GetCo();

    // Build a mask to use only the center part of the image
    //......................................................
    unsigned int mask_lmin = (int)floor(3.0 * nNbLi / 8.0) + 1;
    unsigned int mask_cmin = (int)floor(3.0 * nNbCo / 8.0) + 1;
    unsigned int mask_nb_l = (int)floor(nNbLi * 0.25);
    unsigned int mask_nb_c = (int)floor(nNbCo * 0.25);

    H3_ARRAY2D_UINT8 SquareMask(nNbLi, nNbCo);
    SquareMask.Fill(0);
    for (unsigned int l = 0; l < mask_nb_l; l++)
    {
        for (unsigned int c = 0; c < mask_nb_c; c++)
            SquareMask(l + mask_lmin, c + mask_cmin) = 1;
    }
    SquareMask *= Mask; // multiplication scalaire

    // Fit plane
    //..........
    H3_ARRAY2D_FLT64 MatResSurf(4, 4);
    H3BestFitSurf2(UnwrappedPhase, SquareMask, MatResSurf, 2L);

    // Build the obtained plane
    //.........................
    MatResSurf = MatResSurf.Trans();
    H3_ARRAY2D_FLT32 ComputedPlane(nNbLi, nNbCo);
    H3PolySurf(ComputedPlane, H3_ARRAY2D_FLT32(MatResSurf), nNbCo, nNbLi);

    // Subtract plane
    //...............
    UnwrappedPhase -= ComputedPlane;

    // Subtract the mean value of the center area
    // (assuming there are positive and negative slopes around a background that should be set to 0)
    //..............................................................................................

    // Finding the mean value:
    double dSum = 0.0;
    unsigned int nCount = 0;
    for (unsigned int l = 0; l < mask_nb_l; l++)
    {
        for (unsigned int c = 0; c < mask_nb_c; c++)
        {
            float val = UnwrappedPhase(l + mask_lmin, c + mask_cmin);
            if (!_isnan(val))
            {
                dSum += val;
                nCount++;
            }
        }
    }
    dSum /= nCount;

    // Subtraction
    //TODO FDE on peut faire comme ça mais le calcul est alors en float au lieu de double et le résultat change légèrement
    //UnwrappedPhase -= (float)dSum;

    for (unsigned int l = 0; l < nNbLi; l++)
    {
        for (unsigned int c = 0; c < nNbCo; c++)
            UnwrappedPhase(l, c) = (float)(UnwrappedPhase(l, c) - dSum);
    }

}

// Make the double Dark image a table of unsigned integers
// Uses the mask
void CH3_HoloMap_AltaType::DarkPositiveInteger(H3_ARRAY2D_FLT32& InputIm, const H3_ARRAY2D_UINT8& Mask, H3_ARRAY2D_UINT8& OutputIm) const
{
    // Goal: get all values positive, and ensure maximum of matching by bringing the background (25% lowest values) at a stable value.
    // This is done for "Dark" image stability.
    // But consequently, the numerical content of the "Dark" image is not really meaningful, it is arbitrary. However, no rescaling to the maximum.

    size_t nNbLi = (size_t)InputIm.GetLi(), nNbCo = InputIm.GetCo();

    // Look for max and min
    float fMax = -FLT_MAX;
    float fMin = FLT_MAX;
    unsigned int nMaskCount = 0;

    // Computing the max and min
    for (unsigned int l = 0; l < nNbLi; l++)
        {
            for (unsigned int c = 0; c < nNbCo; c++)
            {
                if (Mask(l, c) == 1)
                {
                    fMax = max(InputIm(l, c), fMax);
                    fMin = min(InputIm(l, c), fMin);
                    nMaskCount++;
                }
            }
        }

    // Histogram of the image on integer values between these extrema
    std::vector<unsigned int> vHisto;

    // Managing the case when the mask is entirely empty:
    if (fMax - fMin < 0)
    {
        concurrency::parallel_for(size_t(0), nNbLi, [&](size_t l)
            //for (unsigned int l = 0; l < nNbLi; l++)
            {
                for (unsigned int c = 0; c < nNbCo; c++)
                {
                    OutputIm(l, c) = 0;
                }
            });
    }
    else
    {
        vHisto.resize((size_t)floor(fMax - fMin) + 2);

        for (unsigned int p = 0; p < vHisto.size(); p++)
            vHisto[p] = 0;

        for (unsigned int l = 0; l < nNbLi; l++)
        {
            for (unsigned int c = 0; c < nNbCo; c++)
            {
                if (Mask(l, c) == 1)
                {
                    vHisto[(int)floor(InputIm(l, c) - fMin + 0.5f)]++;
                }
            }
        }

        // Looking for the limit of the 3% of lowest values
        unsigned int nSum = 0;
        unsigned int nCount = 0;

        while (nSum < (unsigned int)floor(DARKPERCENTAGEOFLOWSATURATION * nMaskCount))
        {
            nSum += vHisto[nCount];
            nCount++;
        }

        float fShift = -(nCount + fMin);

        // Final shift and conversion to integer
        //for (unsigned int l = 0; l < nNbLi; l++)
        concurrency::parallel_for(size_t(0), nNbLi, [&](size_t l)
            {
                for (unsigned int c = 0; c < nNbCo; c++)
                {
                    OutputIm(l, c) = (unsigned char)(min((unsigned char)max(floor((InputIm(l, c) + fShift + 0.5f) * DARKDYNAMICSCOEFF), 0.0f), 255))*Mask(l, c);
                }
            });
    }

}


bool CH3_HoloMap_AltaType::Calibrer1(const CString & CH3Camera_C_File ,
								     const CString & CExtrinsic_param_ep_ObjRef_CamFrame_File,	 
								     const H3_ARRAY2D_FLT32 & UWMirrorX,							
								     const H3_ARRAY2D_FLT32 & UWMirrorY,							
								     H3_ARRAY2D_UINT8 & UWMirrorMask,
								     const size_t Pref[],const CMireHMAP & MHMap)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("Calibrer1()");
	CString strMsg("");
	H3DebugInfo(strModule,strFunction,"");

	isInitialised=false;

    CH3CameraCalib C(m_CalibFolder);
	C.LoadCalib(CH3Camera_C_File,0);
	if(!C.mb_is_initialised)//le fichier decrivant la camera ne convient pas
	{
		H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Calibrer1  Le fichier d'initialisation de la caméra est incomplet.");
		// Erreur Code 3
		m_nErrorTypeCalibSys = 3;
		return false;
	}

	CExtrinsic_param ep_ObjRef_CamFrame;
	if (!ep_ObjRef_CamFrame.LoadCalib(CExtrinsic_param_ep_ObjRef_CamFrame_File,0))	//position du wafer ref dans le repere camera 
	{
		H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Calibrer1  Les paramètres extrinsèques du wafer dans le repère caméra sont invalides.");
		// Erreur Code 4
		m_nErrorTypeCalibSys = 4;
		return false;
	}

	size_t nbValid=0,index,k;
	for(index=0;index<UWMirrorMask.GetSize();index++)
	{
		if(UWMirrorMask[index]>0)
		{
			UWMirrorMask[index]=1;
			nbValid++;
		}
	}

	m_Pix_ref[0]=Pref[0];
	m_Pix_ref[1]=Pref[1];

/*	//check
	CString sm;
	sm.Format("UWMirrorMask Li=%d; Co=%d\nUWMirrorX Li=%d; Co=%d\nUWMirrorY Li=%d; Co=%d\nPref X=%d y=%d",
		UWMirrorMask.GetLi(),UWMirrorMask.GetCo(),
		UWMirrorX.GetLi(),UWMirrorX.GetCo(),
		UWMirrorY.GetLi(),UWMirrorY.GetCo(),
		Pref[1],Pref[0]);
	AfxMessageBox(sm);*/

	size_t nLi=UWMirrorMask.GetLi(),nCo=UWMirrorMask.GetCo();
	bool b=true;
	if(nLi!=UWMirrorX.GetLi()||nCo!=UWMirrorX.GetCo())
		b= false;
	if(nLi!=UWMirrorY.GetLi()||nCo!=UWMirrorY.GetCo())
		b= false;
	if(Pref[0]<0 || Pref[0]>=nCo || Pref[1]<0 || Pref[1]>=nLi )
		b= false;

	if (!b)
	{
		H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Calibrer1  Au moins un des paramètres d'entrées est invalide.");
		//Erreur Code 5
		m_nErrorTypeCalibSys = 5;
		return false;
	}

	//Passage en metrique dans le plan de la mire HMap
	H3_ARRAY2D_FLT32 Mes_MirrorX = UWMirrorX*(MHMap.GetStepX()/TWO_PI);
	H3_ARRAY2D_FLT32 Mes_MirrorY = UWMirrorY*(MHMap.GetStepY()/TWO_PI);

	SCalibResults CR;
	if(m_pH3_HoloMap_Base->Calibrer(CR,C,ep_ObjRef_CamFrame,Mes_MirrorX,Mes_MirrorY,UWMirrorMask)==0)
	{
		//on cherche un fit des carto de phase (UW) qui puisse servir à demoduler les cartos de phases (W) entrantes
		H3_ARRAY2D_FLT64 MatResSurfX(4,4),MatResSurfY(4,4);
		bool b1=H3BestFitSurf2(UWMirrorX,UWMirrorMask,MatResSurfX,3L);
		bool b2=H3BestFitSurf2(UWMirrorY,UWMirrorMask,MatResSurfY,3L);

		if(!(b1&&b2))
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Calibrer1  Il n'y a pas assez de pixels valides sur les images de phases.");
			//Erreur Code 6
			m_nErrorTypeCalibSys = 6;
			return false;
		}

		MatResSurfX=MatResSurfX.Trans();
		MatResSurfY=MatResSurfY.Trans();

		H3PolySurf(m_UWXref,H3_ARRAY2D_FLT32(MatResSurfX),nCo,nLi);
		H3PolySurf(m_UWYref,H3_ARRAY2D_FLT32(MatResSurfY),nCo,nLi);

		//verif: l'ecart entre la phase fittée et la phase initiale est inferieur à un seuil
		H3_ARRAY_FLT32 dUW_X(nbValid),statX;
		H3_ARRAY_FLT32 dUW_Y(nbValid),statY;

		for(index=0,k=0;index<UWMirrorMask.GetSize();index++)
			if(UWMirrorMask[index]>0){
				dUW_X[k]=UWMirrorX[index]-m_UWXref[index];
				dUW_Y[k]=UWMirrorY[index]-m_UWYref[index];
				k++;
			}

		statX=dUW_X.GetStatistics();
		statY=dUW_Y.GetStatistics();

		float	max_X=statX[5]+3*statX[6],
				max_Y=statY[5]+3*statY[6],
				min_X=statX[5]-3*statX[6],
				min_Y=statY[5]-3*statY[6],
				valseuil=(float)(0.05*TWO_PI);

        // FILE* Stream = fopen((char*)LPCTSTR("c:\\temp\\globalTopo.txt"), "a+t");
        // if (Stream)
        // {
        //    CString str ;
        //    fprintf(Stream, "max_X : %f  max_Y : %f  -min_X : %f  -min_Y : %f \n", max_X, max_Y, -min_X, - min_Y);
        //    fclose(Stream);
        // }

		if(max_X>valseuil ||max_Y>valseuil ||(-min_X)>valseuil ||(-min_Y)>valseuil)
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Calibrer1  Les cartes de phases en X ou Y acquises sur le wafer de référence sont trop bruitées.");
			//Erreur Code 7
			m_nErrorTypeCalibSys = 7;
			return false;
		}

		/////////////////////////////////Les resultats du calibrage
		CR.a2d_fit_coef_UWX=MatResSurfX;
		CR.a2d_fit_coef_UWY=MatResSurfY;

		m_PhaseMireHMAP2mm[0]=CR.f_MireHMAP_convert_phiX2mm=(MHMap.GetStepX()/TWO_PI);//coefficient sur l'ecran (mireHMAP)
		m_PhaseMireHMAP2mm[1]=CR.f_MireHMAP_convert_phiY2mm=(MHMap.GetStepY()/TWO_PI);

		H3_POINT2D_FLT32 phi_on_pix_ref= MHMap.Get_phi_on_ref();
		m_phi_onPixScreenRef[0]= CR.f_phi_on_pix_Xref= phi_on_pix_ref.x;
		m_phi_onPixScreenRef[1]= CR.f_phi_on_pix_Yref= phi_on_pix_ref.y;

		CR.Pref_X=Pref[0];
		CR.Pref_Y=Pref[1];

		int pb=CR.Save(m_c_OutFiles.Res1,m_c_OutFiles.ResX,m_c_OutFiles.ResY);
		if(pb)
		{
			H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Calibrer1  Le dossier Calib_Sys situé dans C:\\altasight\\GlobalTopo\\ n'existe pas.");
			//Erreur Code 8
			m_nErrorTypeCalibSys = 8;
			return false;
		}
		//////////////////////////////////////////////////////////
		isInitialised=true;
		//Erreur Code 0
		m_nErrorTypeCalibSys = 0;
		return true;
	}
	else
	{
		H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Calibrer1  Une erreur s'est produite lors du calibrage système.");
		//Erreur Code 9
		m_nErrorTypeCalibSys = 9;
		return false;
	}
}

/* void CH3_HoloMap_AltaType::Alloc()
*  brief
*  param
*  remarks 
*/
void CH3_HoloMap_AltaType::Alloc()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if (m_pH3_HoloMap_Base!=nullptr)
	{
		AfxMessageBox("le capteur est deja alloué.");;
	}

	m_pH3_HoloMap_Base	= new CH3_HoloMap_Base();

	isInitialised=false;
}

/* void CH3_HoloMap_AltaType::Free()
*  brief
*  param
*  remarks 
*/
void CH3_HoloMap_AltaType::Free()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	if (m_pH3_HoloMap_Base!=nullptr)
	{
		delete m_pH3_HoloMap_Base;
	}
	m_pH3_HoloMap_Base= nullptr;

	delete [] m_PhaseMireHMAP2mm;	m_PhaseMireHMAP2mm=   nullptr;
	delete [] m_phi_onPixScreenRef; m_phi_onPixScreenRef= nullptr;

	m_UWXref.~CH3Array2D();
	m_UWYref.~CH3Array2D();
	isInitialised=false;
}

/* int CH3_HoloMap_AltaType::Mesurer(SMesure& LeResultat,const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMask, const float Ratio_ActualPixelPerPeriod_divby_CalibOne)
*  brief
*  param
*  remarks 
*/
int CH3_HoloMap_AltaType::Mesurer(SMesure& LeResultat,const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMask, bool bUnwrappedPhase, const bool saveUnwrappedPhases,
								  const float Ratio_ActualPixelPerPeriod_divby_CalibOne,
								  const std::tuple<float, float> pixel_imagePointAltitudeConnue,
								  const float altitude,
								  const std::tuple<unsigned long, unsigned long> pixel_imageMarquageEcran,
								  const bool mesureType)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	CString strFunction("H3_HoloMap_AltaType::Mesurer");
	H3DebugInfo(strModule,strFunction,"");

	CWaitCursor wait;

	if(m_pH3_HoloMap_Base==nullptr || !isInitialised)
	{
		return 1;
	}

	const size_t nLi=aMask.GetLi(),nCo=aMask.GetCo();
	if(!(aW_X.GetLi()==nLi && aW_X.GetCo()==nCo && aW_Y.GetLi()==nLi && aW_Y.GetCo()==nCo))
	{
		H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Mesurer  les données sont de tailles distinctes");
		// Erreur Code 2
		m_nErrorTypeMeasure = 2;
		return 2;
	}

    // This is a copy... Try to avoid it!!!
    H3_ARRAY2D_FLT32 UW_X(aW_X);
    H3_ARRAY2D_FLT32 UW_Y(aW_Y);

	if(!mesureType) // NanoTopo
	{
        if (!bUnwrappedPhase)
        {
            //demodulation des image de phase en utilisant une image de phase demodulée du wafer reference
            //H3_ARRAY2D_FLT32 UW_X=demodule(aW_X,m_UWXref,aMask),UW_Y=demodule(aW_Y,m_UWYref,aMask);
            if (fabs(Ratio_ActualPixelPerPeriod_divby_CalibOne - 1.0f) < FLT_EPSILON) {
                UW_X = demodule(aW_X, m_UWXref, aMask);
                UW_Y = demodule(aW_Y, m_UWYref, aMask);
            }
            else {
                UW_X = demodule(aW_X, m_UWXref / Ratio_ActualPixelPerPeriod_divby_CalibOne, aMask) * Ratio_ActualPixelPerPeriod_divby_CalibOne;
                UW_Y = demodule(aW_Y, m_UWYref / Ratio_ActualPixelPerPeriod_divby_CalibOne, aMask) * Ratio_ActualPixelPerPeriod_divby_CalibOne;
            }
        }

        //passage en metrique dans le plan de la mire HMap
        UW_X *= m_PhaseMireHMAP2mm[0];
        UW_Y *= m_PhaseMireHMAP2mm[1];

        if (!bUnwrappedPhase)
        {
            size_t i;
            for (i = 0; i < aMask.GetSize(); i++) {
                if (!aMask[i])
                    UW_X[i] = UW_Y[i] = NaN;
            }
        }

        if (saveUnwrappedPhases)
        {
            UW_X.Save("C:\\temp\\nanoTopoPhaseXUnwrapped.hbf");
            UW_Y.Save("C:\\temp\\nanoTopoPhaseYUnwrapped.hbf");
        }

		if (m_pH3_HoloMap_Base)
		{
			int err_code;
            err_code = m_pH3_HoloMap_Base->Mesurer_Z0(LeResultat, UW_X, UW_Y, aMask);

			if (err_code == 3)
			{
				H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Mesurer  La cartographie de mesure est vide");
				// Erreur Code 3
				m_nErrorTypeMeasure = 3;
				return 3;
			}
		}
	}
	else    // GlobalTopo
	{
        if (!bUnwrappedPhase) //demodulation spatiale
        {
            //le point de depart de la demodulation est au centre du wafer
            long i_med = nLi / 2, j_med = nCo / 2, i_mean = 0, j_mean = 0, nValid = 0;
            for (size_t i = 0, j, k = 0; i < nLi; ++i)
                for (j = 0; j < nCo; ++j, ++k)
                    if (aMask[k])
                    {
                        i_mean += (i - i_med);
                        j_mean += (j - j_med);
                        nValid++;
                    }

            if (nValid == 0)
            {
                H3DebugError(strModule, strFunction, "in CH3_HoloMap_AltaType::Mesurer  le mask n'indique pas de point valide");
                // Erreur Code 3
                m_nErrorTypeMeasure = 3;
                return 3;
            }

            i_mean /= nValid; i_mean += i_med;
            j_mean /= nValid; j_mean += j_med;

            //pour Remi B. : ici se tient la partie longue du traitement
            //les 2 appels à H3UnwrapPhase peuvent etre mis dans 2 threads (mais les thread VC12 ne sont pas compatibles avec VC10)
            UW_X = H3UnwrapPhase(aW_X, aMask, i_mean, j_mean, (long)0) * Ratio_ActualPixelPerPeriod_divby_CalibOne;
            UW_Y = H3UnwrapPhase(aW_Y, aMask, i_mean, j_mean, (long)0) * Ratio_ActualPixelPerPeriod_divby_CalibOne;
        }

        //H3_ARRAY2D_FLT32 UW_X;
        //H3_ARRAY2D_FLT32 UW_Y;

        //// Ne fonctionne pas car H3UnwrapPhase n'est pas thread safe 
        //concurrency::parallel_invoke(
        //    [&] {UW_X = H3UnwrapPhase(aW_X, aMask, i_mean, j_mean, (long)0) / Ratio_ActualPixelPerPeriod_divby_CalibOne; },
        //    [&] {UW_Y = H3UnwrapPhase(aW_Y, aMask, i_mean, j_mean, (long)0) / Ratio_ActualPixelPerPeriod_divby_CalibOne; }
        // );

		//fin de la partie longue du traitement

		//on veut une phase precise sur un pixel donné //lien entre la phase et l'ecran
		unsigned long xpixel_imageMarquageEcran = get<0>(pixel_imageMarquageEcran);
		unsigned long ypixel_imageMarquageEcran = get<1>(pixel_imageMarquageEcran);

        float on_aW_X_phi_onPixScreenRef, on_aW_Y_phi_onPixScreenRef;

        if (!bUnwrappedPhase)    // If the phase has already ben unwrapped by multi-period, it does not need any offset.
        {
            on_aW_X_phi_onPixScreenRef = aW_X(ypixel_imageMarquageEcran, xpixel_imageMarquageEcran);
            on_aW_Y_phi_onPixScreenRef = aW_Y(ypixel_imageMarquageEcran, xpixel_imageMarquageEcran);
            UW_X += m_phi_onPixScreenRef[0] - UW_X(ypixel_imageMarquageEcran, xpixel_imageMarquageEcran) + on_aW_X_phi_onPixScreenRef;
            UW_Y += m_phi_onPixScreenRef[1] - UW_Y(ypixel_imageMarquageEcran, xpixel_imageMarquageEcran) + on_aW_Y_phi_onPixScreenRef;
        }
        
		//passage en metrique dans le plan de la mire HMap
		UW_X *= m_PhaseMireHMAP2mm[0];
		UW_Y *= m_PhaseMireHMAP2mm[1];

        if (saveUnwrappedPhases)
        {
            UW_X.Save("C:\\Temp\\GlobalTopoPhaseXUnwrapped.hbf");
            UW_Y.Save("C:\\Temp\\GlobalTopoPhaseYUnwrapped.hbf");
        }

		if (m_pH3_HoloMap_Base)
		{
			//int err_code= m_pH3_HoloMap_Base->Mesurer_Zreal(LeResultat, UW_X, UW_Y, aMask, m_Pix_ref[0], m_Pix_ref[1], altitude);
			int err_code= m_pH3_HoloMap_Base->Mesurer_Zreal(LeResultat, UW_X, UW_Y, aMask, get<0>(pixel_imagePointAltitudeConnue), get<1>(pixel_imagePointAltitudeConnue), altitude);
			if (err_code == 3)
			{
				H3DebugError(strModule,strFunction,"in CH3_HoloMap_AltaType::Mesurer_Zreal  La cartographie de mesure est vide");
				// Erreur Code 3
				m_nErrorTypeMeasure = 3;
				return 3;
			}
		}
	}

	H3DebugInfo(strModule,strFunction,_T("out"));
	// Erreur Code 0
	m_nErrorTypeMeasure = 0;
	return 0;
}

/*int CH3_HoloMap_AltaType::CheckPosition(const CPoint P)
*  brief un pixel de l'ecran (mireHMAP) doit etre stable dans l'image si le wafer est convenablement orienté
*  plutot que de verifier un ecart en pixel, on regarde la variation de phase engendrée
*  param
*  remarks 
*/
int CH3_HoloMap_AltaType::CheckPosition(const CPoint P)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	if(	fabs(m_UWXref(P.y,P.x)-m_UWXref(m_Pix_ref[1],m_Pix_ref[0]))>(fPI/2) ||
		fabs(m_UWYref(P.y,P.x)-m_UWYref(m_Pix_ref[1],m_Pix_ref[0]))>(fPI/2) ) return 1;

	return 0;
}

/* int CH3_HoloMap_AltaType::Init(const CString & CH3Camera_C_File, const CString& SCalibResults_File, const CString& SCalibResults_FileX, const CString& SCalibResults_FileY)
*  brief
*  param
*  remarks 
*/
int CH3_HoloMap_AltaType::Init()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	isInitialised=false;

	CString strFunction("H3_HoloMap_AltaType_Init");
	H3DebugInfo(strModule,strFunction,"");

	if(m_pH3_HoloMap_Base == nullptr )
	{
		AfxMessageBox("in CH3_HoloMap_AltaType::Init  m_pH3_HoloMap_Base == nullptr");
		return 1;
	}

	CH3Camera Cam;
    Cam.LoadCalib(m_CH3Camera_C_File, 0);

	SCalibResults CR;
    int test = CR.Load(m_c_OutFiles.Res1, m_c_OutFiles.ResX, m_c_OutFiles.ResY);
	if(test != 0)
	{
		AfxMessageBox("in CH3_HoloMap_AltaType::Init  CalibResults mal initialisé");
		return 1;
	}

    CExtrinsic_param waferToCam;
    if (!waferToCam.LoadCalib(m_CExtrinsic_param_ep_ObjRef_CamFrame_File, 0))
    {
        AfxMessageBox("in CH3_HoloMap_AltaType::Init  Cannot load EP_ref_CamFrame.txt");
        return 1;
    }
	
    int res = m_pH3_HoloMap_Base->init(Cam, waferToCam, CR.ep_MireHMAP);
	if(res!=0)
		return res;

	bool b=true;
	b&=H3PolySurf(m_UWXref,H3_ARRAY2D_FLT32(CR.a2d_fit_coef_UWX),Cam.nx,Cam.ny);
	b&=H3PolySurf(m_UWYref,H3_ARRAY2D_FLT32(CR.a2d_fit_coef_UWY),Cam.nx,Cam.ny);

	m_PhaseMireHMAP2mm[0]=CR.f_MireHMAP_convert_phiX2mm;
	m_PhaseMireHMAP2mm[1]=CR.f_MireHMAP_convert_phiY2mm;

	m_phi_onPixScreenRef[0]= CR.f_phi_on_pix_Xref;
	m_phi_onPixScreenRef[1]= CR.f_phi_on_pix_Yref;

	m_Pix_ref[0]=CR.Pref_X;
	m_Pix_ref[1]=CR.Pref_Y;

	if(b)
		isInitialised=true;
	else{
		isInitialised=false;
		return 1;
	}

	return 0;
}



// Specific initialization used when it is require to unwrap phases from reference without doing topography (because the full initialization is too slow).
// The HoloMap will not be initialized!
//int CH3_HoloMap_AltaType::InitUnwrapReference(const CString & CH3Camera_C_File, const CString& SCalibResults_File, const CString& SCalibResults_FileX, const CString& SCalibResults_FileY)
//{
//	AFX_MANAGE_STATE(AfxGetStaticModuleState());
//
//	isInitialised = false;
//
//	CString strFunction("H3_HoloMap_AltaType_InitUnwrapReference");
//	H3DebugInfo(strModule, strFunction, "");
//
//	if (m_pH3_HoloMap_Base == nullptr)
//	{
//		AfxMessageBox("in CH3_HoloMap_AltaType::InitUnwrapReference  m_pH3_HoloMap_Base == nullptr");
//		return 1;
//	}
//
//	CH3Camera Cam;
//	Cam.LoadCalib(CH3Camera_C_File, 0);
//
//	SCalibResults CR;
//	int test = CR.Load(SCalibResults_File, SCalibResults_FileX, SCalibResults_FileY);
//	if (test != 0)
//	{
//		AfxMessageBox("in CH3_HoloMap_AltaType::InitUnwrapReference  CalibResults mal initialisé");
//		return 1;
//	}
//
//	bool b = true;
//	b &= H3PolySurf(m_UWXref, H3_ARRAY2D_FLT32(CR.a2d_fit_coef_UWX), Cam.nx, Cam.ny);
//	b &= H3PolySurf(m_UWYref, H3_ARRAY2D_FLT32(CR.a2d_fit_coef_UWY), Cam.nx, Cam.ny);
//
//	m_PhaseMireHMAP2mm[0] = CR.f_MireHMAP_convert_phiX2mm;
//	m_PhaseMireHMAP2mm[1] = CR.f_MireHMAP_convert_phiY2mm;
//
//	m_phi_onPixScreenRef[0] = CR.f_phi_on_pix_Xref;
//	m_phi_onPixScreenRef[1] = CR.f_phi_on_pix_Yref;
//
//	m_Pix_ref[0] = CR.Pref_X;
//	m_Pix_ref[1] = CR.Pref_Y;
//
//	if (b)
//		isInitialised = true;
//	else {
//		isInitialised = false;
//		return 1;
//	}
//
//	return 0;
//}

