#pragma once

#include "CoVeLi_struct.h"
#include "H3_HoloMap_Base.h"
#include "MireHMAP.h"
#include <tuple>

#ifdef _DLL
#  ifdef COVELI_DLL
#    define H3_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3_EXPORT_DECL __declspec(dllimport)
#  endif 
#else
#  define H3_EXPORT_DECL
#endif


struct Calib_OutFiles {
    CString Res1, ResX, ResY;
};

struct Calib_data_4calibonly {
    float m_fPixSizeX, m_fPixSizeY;			//dimension des pixels ecran (mm)
    float m_fMireMonStepX, m_fMireMonStepY; //pas du reseau fin affiché sur l'ecran lors du calibrage

    unsigned long m_pixRef_Xscreen, m_pixRef_Yscreen; //position sur l'ecran d'un pixel remarquable (marquage)
    unsigned long m_screen_Xsz, m_screen_Ysz;//taille de l'ecran (en pixel)
};

class H3_EXPORT_DECL CH3_HoloMap_AltaType
{
public:
    CH3_HoloMap_AltaType(const CString& calibFolder);
    ~CH3_HoloMap_AltaType(void);
        
    CString m_CalibFolder;

    int Mesurer(SMesure& LeResultat, const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMaskconst, bool bUnwrappedPhase,
        const bool saveUnwrappedPhases,
        float Ratio_ActualPixelPerPeriod_divby_CalibOne = 1.0f,
        const std::tuple<float, float> pixel_imagePointAltitudeConnue = std::tuple<float, float>(0, 0),
        const float altitude = 0.0f,
        const std::tuple<unsigned long, unsigned long> pixel_imageMarquageEcran = std::tuple<unsigned long, unsigned long>(0, 0),
        const bool mesureType = false);

    int Init();
    // Faster initialization for unwrapping without topography: SDE currently not used.
    //int InitUnwrapReference(const CString & CH3Camera_C_File, const CString& SCalibResults_File, const CString& SCalibResults_FileX, const CString& SCalibResults_FileY);

    int CheckPosition(const CPoint P);

    bool Calibrer1(const CString & CH3Camera_C_File,
        const CString & CExtrinsic_param_ep_ObjRef_CamFrame_File,	//position du wafer ref dans le repere camera 
        const H3_ARRAY2D_FLT32 & UWMirrorX,							//pour determiner la position du wafer ref
        const H3_ARRAY2D_FLT32 & UWMirrorY,							//pour determiner la position du wafer ref
        H3_ARRAY2D_UINT8 & UWMirrorMask,
        const size_t  Pref[], const CMireHMAP & MHMap);

    H3_ARRAY2D_FLT32 Demoduler(const CH3Array<H3_ARRAY2D_FLT32> & pW, const CH3Array2D<unsigned char> pMask, const float ratio[], const unsigned long N) const;

    // IBE: Demodulation from a reference phase computed during calibration
    void DemodulateFromReferenceX(const H3_ARRAY2D_FLT32 & aWrappedPhase, const H3_ARRAY2D_UINT8 & aMask, float Ratio_ActualPixelPerPeriod_divby_CalibOne, H3_ARRAY2D_FLT32& aResult) const;
    void DemodulateFromReferenceY(const H3_ARRAY2D_FLT32 & aWrappedPhase, const H3_ARRAY2D_UINT8 & aMask, float Ratio_ActualPixelPerPeriod_divby_CalibOne, H3_ARRAY2D_FLT32& aResult) const;

    // IBE: Demodulation using a spatial method (based on 1D demodulation)
    H3_ARRAY2D_FLT32 DemodulateSpatially(const H3_ARRAY2D_FLT32 & WrappedPhase, const H3_ARRAY2D_UINT8 & aMask) const;

    // IBE: Removing the mean plane after demodulation
    void ShapeSubtraction(H3_ARRAY2D_FLT32 & UnwrappedPhase, const H3_ARRAY2D_UINT8 & Mask, const bool bMeanToZero) const;
    void PlaneSubtraction(H3_ARRAY2D_FLT32& UnwrappedPhase, const H3_ARRAY2D_UINT8& Mask) const;

    // TODO SDE load is not the opposite of save here!
    // Preparing the "Dark" output
    void DarkPositiveInteger(H3_ARRAY2D_FLT32& InputIm, const H3_ARRAY2D_UINT8& Mask, H3_ARRAY2D_UINT8& OutputIm) const;
    bool LoadSettings1();

    bool SaveSettings(const CString & iniFile, CString strSection = _T(""));

    // IBE
    int GetRefSizeCo() const;
    int GetRefSizeLi() const;

public:
    Calib_data_4calibonly* m_data_4calibonly;

    CString m_CH3Camera_C_File;
    CString m_CExtrinsic_param_ep_ObjRef_CamFrame_File;

    size_t m_Pix_ref[2];//position pixel dans l'image d'un point remarquable
    CMireHMAP m_MireHMAP;

    /*static*/ int m_nErrorTypeCalibSys;
    /*static*/ int m_nErrorTypeMeasure;

    Calib_OutFiles m_c_OutFiles;

private:
    void Alloc();
    void Free();
    H3_ARRAY2D_FLT32 m_UWXref, m_UWYref;
    CH3_HoloMap_Base *m_pH3_HoloMap_Base;
    float* m_PhaseMireHMAP2mm;
    float* m_phi_onPixScreenRef;
    bool isInitialised;
};
