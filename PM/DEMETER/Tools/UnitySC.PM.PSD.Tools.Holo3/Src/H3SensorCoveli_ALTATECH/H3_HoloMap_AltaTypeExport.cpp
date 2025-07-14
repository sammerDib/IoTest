
#include "stdafx.h"
#include "Coveli.h"
#include "H3_HoloMap_AltaType.h"
#include "MireHMAP.h"
#include "H3_DSLRCalib_Export.h"
#include "H3_HoloMap_AltaTypeExport.h"

#ifdef _DEBUG  
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#define new DEBUG_NEW
#endif

static const CString strModule("CH3_HoloMap_BaseExport");
static CH3_HoloMap_AltaType* pH3_HoloMap = nullptr;
static CString HoloMap_AltaTypeExport_INI_File("HMap_AltaTypeExport.ini");

#define EXIT_IF_NOT_ALLOCATED(RetVal)												  \
if (!pCoveliApp)																      \
{																					  \
	H3DisplayError(strModule,strFunction,strMsg+_T("\nLe capteur n'est pas alloué."));\
	return RetVal;																      \
}

///cette fct est dans H3_Holomap_AltaType.cpp
///je ne sais pas ou elle serait le mieux, mais pas à 2 endroits
//demodule
//input, plusieurs images modulées obtenues sur le meme objet (fixe) mais avec des reseaux différents mais de meme orientation
//la phase correspondant au reseau le plus fin est pW[0], le reseau le plus grossier est pW[N-1]
// toutes les images ont la meme taille (sinon big pb)
//input, un tableau contenant de N case dans la case i le facteur (pas reseau i)/(pas reseau i-1)
//dans la case 0 : 1 (pas lu)
static H3_ARRAY2D_FLT32 demodule(const H3_ARRAY2D_FLT32** pW, const float ratio[], const unsigned long N)
{
    if (N == 0)
        return (*pW[0]);

    const size_t nLi = pW[0]->GetLi(), nCo = pW[0]->GetCo();
    H3_ARRAY2D_FLT32 UW = (*pW[0]), order;

    size_t i;
    float factor = 1.0, old_facor = 1.0;
    for (i = 1; i < N; i++) {
        old_facor = factor;
        factor *= ratio[i];
        order = (H3_ARRAY2D_FLT64)(*pW[i]);
        order *= factor;
        order -= UW;
        order /= old_facor * TWO_PI;
        order += 0.5;
        order._floor();

        UW += order * (old_facor * TWO_PI);
    }
    return UW;
}

//////////////////////////////////////////////////////////////////////////////////////
// Fonctions exportees

/////////////////////////////////////////////////////////////////////////////
extern "C" H3_EXPORT_DECL
void H3_HoloMap_AltaType_Demodule(H3_ARRAY2D_FLT32 & UW, const CH3Array<H3_ARRAY2D_FLT32> & wPhase, const CH3Array2D<unsigned char> &Mask, const float* ratio)
{
    UW = pH3_HoloMap->Demoduler(wPhase, Mask, ratio, wPhase.GetSize());
}

extern "C" H3_EXPORT_DECL
void H3_HoloMap_AltaType_CalibSystem(const CH3Array<H3_ARRAY2D_FLT32> & wPhase,
    const H3_ARRAY2D_UINT8 & mireImage,
    const float fPixSizeX, const float fPixSizeY,
    const float fMireMonStepX, const float fMireMonStepY,
    const unsigned long pixRef_Xscreen, const unsigned long pixRef_Yscreen,
    const unsigned long screen_Xsz, const unsigned long screen_Ysz,
    const int nCrossX, const int nCrossY, const float* ratio, const CString & calibFolder)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("H3_HoloMap_AltaType_CalibSystem()");
    CString strMsg("Impossible de calibrer le système.");
    CString str;

    int iH3_HoloMap_AltaType_isAllocated = H3_HoloMap_AltaType_Alloc(calibFolder);
    if (iH3_HoloMap_AltaType_isAllocated != 0)
    {
        AfxMessageBox("le capteur n'est pas alloué");
        return;
    }

    if (nullptr == pH3_HoloMap->m_data_4calibonly)
        pH3_HoloMap->m_data_4calibonly = new Calib_data_4calibonly;
    //dimension des pixels de l'ecran
    pH3_HoloMap->m_data_4calibonly->m_fPixSizeX = fPixSizeX;
    pH3_HoloMap->m_data_4calibonly->m_fPixSizeY = fPixSizeY;

    //pas du reseau fin à l'ecran (en pixel)
    pH3_HoloMap->m_data_4calibonly->m_fMireMonStepX = fMireMonStepX;
    pH3_HoloMap->m_data_4calibonly->m_fMireMonStepY = fMireMonStepY;

    //dimension de l'ecran (en pixel)
    pH3_HoloMap->m_data_4calibonly->m_pixRef_Xscreen = pixRef_Xscreen;
    pH3_HoloMap->m_data_4calibonly->m_pixRef_Yscreen = pixRef_Yscreen;

    //dimension de l'ecran (en pixel)
    pH3_HoloMap->m_data_4calibonly->m_screen_Xsz = screen_Xsz;
    pH3_HoloMap->m_data_4calibonly->m_screen_Ysz = screen_Ysz;

    //position dans l'image camera d'un marquage de l'ecran 
    pH3_HoloMap->m_Pix_ref[0] = nCrossX;
    pH3_HoloMap->m_Pix_ref[1] = nCrossY;

    pH3_HoloMap->SaveSettings(calibFolder + "\\" + _CalibPaths._InputSettingsFile, _T(""));

    delete pH3_HoloMap->m_data_4calibonly;
    pH3_HoloMap->m_data_4calibonly = nullptr;

    long nError = H3_DSLR_GetExtrinsic((H3_ARRAY2D_UINT8*)&mireImage, calibFolder);
    switch (nError)
    {
    case 0:	// Le chargement des paramètres de la caméra a réussie, on continue le calibrage système...
        break;
    case -1:
        pH3_HoloMap->m_nErrorTypeCalibSys = 11; // L'entrée 'str_CamFile' dans le fichier 'C:\\AltaSight\\GlobalTopo\\SensorData.txt' n'est pas renseignée!...
        return;									// str_CamFile=C:\AltaSight\GlobalTopo\Calib_cam\CalibCam_0.txt
    case -2:
        pH3_HoloMap->m_nErrorTypeCalibSys = 12; // L'entrée 'ep_ObjRef_CamFrame_File' dans le fichier 'C:\\AltaSight\\GlobalTopo\\SensorData.txt' n'est pas renseignée!...
        return;									// ep_ObjRef_CamFrame_File=C:\AltaSight\GlobalTopo\Calib_cam\EP_ref_CamFrame.txt
    case -3:
    case -6:
        pH3_HoloMap->m_nErrorTypeCalibSys = 13; // Annulation de la procédure de calibrage système par l'opérateur!...
        return;
    case -4:
        pH3_HoloMap->m_nErrorTypeCalibSys = 14; // Le fichier 'C:\\AltaSight\\GlobalTopo\\SensorData.txt' ou 'C:\AltaSight\Nano\Calib_cam\EP_ref_CamFrame.txt' n'existe pas!...
        return;
    case -5:
        pH3_HoloMap->m_nErrorTypeCalibSys = 15; // Erreur lors de la récupération des paramètres extrinsèques de la mire!...
        return;
    default:
        pH3_HoloMap->m_nErrorTypeCalibSys = 9;
        return;
    }

    size_t nSize = wPhase.GetSize();
    size_t nLi = wPhase[0].GetLi();
    size_t nCo = wPhase[0].GetCo();

    // Extraire les images de phases en X
    CH3Array<H3_ARRAY2D_FLT32> pW_X(nSize / 2);

    for (size_t i = 0; i < nSize / 2; i++)
    {
        pW_X[i].ReAlloc(nLi, nCo);
        pW_X[i].Fill(0);
    }
    for (size_t i = 0, j = 0; i < nSize; i += 2, j++)
        pW_X[j] = wPhase[i];

    // Fake mask for phase unwrapping
    CH3Array2D<unsigned char> Mask(nLi, nCo);
    Mask.Fill(1);
    //
    // Demodulation des images de phases en X avec différents réseaux en une seule image de phase
    H3_ARRAY2D_FLT32 UW_X;
    UW_X = pH3_HoloMap->Demoduler(pW_X, Mask, ratio, pW_X.GetSize());
    //

    // Extraire les images de phases en Y
    CH3Array<H3_ARRAY2D_FLT32> pW_Y(nSize / 2);

    for (size_t i = 0; i < nSize / 2; i++)
    {
        pW_Y[i].ReAlloc(nLi, nCo);
        pW_Y[i].Fill(0);
    }

    for (size_t i = 1, j = 0; i < nSize; i += 2, j++)
        pW_Y[j] = wPhase[i];
    
    // Demodulation des images de phases en Y avec différents réseaux en une seule image de phase
    H3_ARRAY2D_FLT32 UW_Y;
    UW_Y = pH3_HoloMap->Demoduler(pW_Y, Mask, ratio, pW_Y.GetSize());
    //

    H3_ARRAY2D_UINT8	UWMirrorMask;
    CString strUWMirrorMask;
    CString strFileName = calibFolder + "\\" + _CalibPaths._InputSettingsFile;
    CString strSection;
    strSection = _T("SensorHoloMap3");
    strUWMirrorMask = _CalibPaths.UWMirrorMaskPath(calibFolder);
    UWMirrorMask.Load(strUWMirrorMask);

    // Calibrage systeme
    if (!pH3_HoloMap->LoadSettings1())
        return;

    if (!pH3_HoloMap->Calibrer1(pH3_HoloMap->m_CH3Camera_C_File, pH3_HoloMap->m_CExtrinsic_param_ep_ObjRef_CamFrame_File, UW_X, UW_Y, UWMirrorMask,
        pH3_HoloMap->m_Pix_ref,
        pH3_HoloMap->m_MireHMAP))
        return;
    //
}

extern "C" H3_EXPORT_DECL
int H3_HoloMap_AltaType_GetErrorTypeCalibSys()
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState())
        CString strFunction("H3_HoloMap_AltaType_GetErrorTypeCalibSys()");
    CString strMsg("");
    CString str;

    return pH3_HoloMap->m_nErrorTypeCalibSys;
}

extern "C" H3_EXPORT_DECL
int H3_HoloMap_AltaType_GetErrorTypeMeasure()
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState())
        CString strFunction("H3_HoloMap_AltaType_GetErrorTypeMeasure()");
    CString strMsg("");
    CString str;

    return pH3_HoloMap->m_nErrorTypeMeasure;
}

extern "C" H3_EXPORT_DECL
void H3_InitSys(const CString & calibFolder)
{
    int iH3_HoloMap_AltaType_isAllocated = H3_HoloMap_AltaType_Alloc(calibFolder);

    if (iH3_HoloMap_AltaType_isAllocated != 0)
    {
        AfxMessageBox("le capteur n'est pas alloué");
        return;
    }

    CWaitCursor wait;

    //la suite
    if (!pH3_HoloMap->LoadSettings1())
        //CString CH3Camera_C_File   =H3GetPrivProfile(section,"str_CamFile" ,"",FileName);
        //CString SCalibResults_File =H3GetPrivProfile(section,"str_epFile"  ,"",FileName);
        //CString SCalibResults_FileX=H3GetPrivProfile(section,"str_PhiXFile","",FileName);
        //CString SCalibResults_FileY=H3GetPrivProfile(section,"str_PhiYFile","",FileName);

        //if(CH3Camera_C_File.IsEmpty()		|| 
        //	SCalibResults_File.IsEmpty()	|| 
        //	SCalibResults_FileX.IsEmpty()	|| 
        //	SCalibResults_FileY.IsEmpty() )
    {
        AfxMessageBox("le capteur n'est pas alloué car la section SensorHoloMap3 de SensorData.txt est incomplete");
        H3_HoloMap_AltaType_Free();
        return;
    }

    pH3_HoloMap->Init();
}

//extern "C" H3_EXPORT_DECL
//void H3_InitSys2(CString CalibCameraFilePath,CString CalibResultFilePath,CString CalibResultFilePathX,CString CalibResultFilePathY)
//{
//	//A FAIRE______________________________________
//	int iH3_HoloMap_AltaType_isAllocated=H3_HoloMap_AltaType_Alloc("H3_InitSys2 non utilisé?");
//
//	if(iH3_HoloMap_AltaType_isAllocated!=0)
//	{
//	
//		return ;
//	}
//
//	if(CalibCameraFilePath.IsEmpty()		|| 
//		CalibResultFilePath.IsEmpty()	|| 
//		CalibResultFilePathX.IsEmpty()	|| 
//		CalibResultFilePathY.IsEmpty() )
//	{
//		//gestion d'erreur
//	}
//
// 	int val=pH3_HoloMap->Init(CalibCameraFilePath , CalibResultFilePath , CalibResultFilePathX , CalibResultFilePathY );
//}

/* long H3_HoloMap_Base_IsAllocated()
*  brief
*  param
*  remarks
*/
extern "C" H3_EXPORT_DECL
long H3_HoloMap_AltaType_IsAllocated()
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    if (!pH3_HoloMap)
    {
        return 0;
    }
    return 1;
}

/* long H3_HoloMap_AltaType_Alloc()
*  brief
*  param
*  remarks
*/
extern "C" H3_EXPORT_DECL
long H3_HoloMap_AltaType_Alloc(const CString & calibFolder)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("H3_HoloMap_Base_Alloc()");
    CString strMsg("Impossible d'allouer le capteur.");

    if (pH3_HoloMap != nullptr)
    {
        //H3DisplayError(strModule,strFunction,strMsg+_T("\nLe capteur est déjà alloué."));
        return 0;
    }

    pH3_HoloMap = new CH3_HoloMap_AltaType(calibFolder);


    if (pH3_HoloMap == nullptr)
    {
        H3DisplayError(strModule, strFunction, strMsg + _T("\nEchec de la fonction new 1."));
        return 1;
    }
    else
    {
    }

    return 0;
}

/* void H3_HoloMap_AltaType_Free()
*  brief
*  param
*  remarks
*/
extern "C" H3_EXPORT_DECL
void H3_HoloMap_AltaType_Free()
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    //AfxMessageBox("in export H3_HoloMap_AltaType_Free()");

    if (pH3_HoloMap != nullptr)
    {
        delete pH3_HoloMap;
    }
    pH3_HoloMap = nullptr;


}

/* int H3_HoloMap_AltaType_Mesurer(SMesure& LeResultat, const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMask,const float Ratio_ActualPixelPerPeriod_divby_CalibOne)
*  brief
*  param
* bUnwrappedPhase: true if a phase already unwrapped is provided (IBE).
*  remarks
*/
extern "C++" H3_EXPORT_DECL
int H3_HoloMap_AltaType_Mesurer(SMesure & LeResultat, const H3_ARRAY2D_FLT32 & aW_X, const H3_ARRAY2D_FLT32 & aW_Y, const H3_ARRAY2D_UINT8 & aMask, const bool bUnwrappedPhase,
    const bool saveUnwrappedPhases,
    const float Ratio_ActualPixelPerPeriod_divby_CalibOne,
    const std::tuple<float, float> pixel_imagePointAltitudeConnue,
    const float altitude,
    const std::tuple<unsigned long, unsigned long> pixel_imageMarquageEcran,
    const bool mesureType)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    CWaitCursor wait;

    CString strFunction("H3_HoloMap_Base_Mesurer");
    H3DebugInfo(strModule, strFunction, "");

    if ((pH3_HoloMap == nullptr))
    {
        // Erreur Code 1
        H3DebugError(strModule, strFunction, "in Export CH3_HoloMap_AltaType::Mesurer  pH3_HoloMap==NULL");
        return 1;
    }
    else
    {
        return pH3_HoloMap->Mesurer(LeResultat, aW_X, aW_Y, aMask, bUnwrappedPhase,
            saveUnwrappedPhases,
            Ratio_ActualPixelPerPeriod_divby_CalibOne,
            pixel_imagePointAltitudeConnue, altitude,
            pixel_imageMarquageEcran,
            mesureType);
    }
}

/* bool H3_HoloMap_Base_CheckPosition()
*  brief
*  param
*  remarks
*/
extern "C++" H3_EXPORT_DECL
int H3_HoloMap_AltaType_CheckPosition(const CPoint P)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    if (pH3_HoloMap == nullptr)
    {
        return 1;
    }
    else
        return pH3_HoloMap->CheckPosition(P);
}
