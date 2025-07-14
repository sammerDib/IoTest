
#include "stdafx.h"
#include "H3AppToolsDecl.h"

#include ".\\..\\H3DSLRCalibration\\H3DSLRCalibration.h"
#include "H3_DSLRCalib_Export.h"

#include "CameraCalibDlg.h"

#ifdef _DEBUG  
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#define new DEBUG_NEW
#endif

static const CString strModule("H3_DSLRCalib_Export");

//////////////////////////////////////////////////////////////////////////////////////
// Fonctions exportees
//////////////////////////////////////////////////////////////////////////////////////

extern "C" H3_DSLRCalibExp_DECL
long H3_DSLR_CalibCamera(const int nMireSizeX,
    const int nMireSizeY,
    const float fMireStepX,
    const float fMireStepY,
    const int nNbImg,
    const CH3Array<H3_ARRAY2D_UINT8> & vMirePos,
    const CString & calibFolder)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState())

        CH3Mire::m_iLiIntersection = nMireSizeY;
    CH3Mire::m_iCoIntersection = nMireSizeX;
    CH3Mire::m_fMireStepX = fMireStepX;
    CH3Mire::m_fMireStepY = fMireStepY;

    CString strCalib = calibFolder + "\\" + _CalibPaths._InputSettingsFile;
    CCameraCalibDlg Dlg(calibFolder);

    Dlg.SaveSettings(strCalib);

    Dlg.LoadSettings(strCalib);
    Dlg.SetUse(TO_CALIBRATE_CAM);

    unsigned long nbIm = vMirePos.GetSize();
    Dlg.m_vMirePos.ReAlloc(nbIm);
    if (nbIm > 0)
    {
        for (unsigned long i = 0; i < nbIm; i++)
        {
            Dlg.m_vMirePos[i] = vMirePos[i];
        }
    }

    Dlg.DoModal();
    return 0;
}

extern "C" H3_DSLRCalibExp_DECL int H3_DSLR_GetErrorTypeCalibCam()
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState())

    return CCameraCalibDlg::m_nErrorTypeCalibCam;
}

// long H3_DSLR_GetExtrinsic(H3_ARRAY2D_UINT8* pImageWafRef)
// charge les parametres de la camera dans c:\\altasight\\
// enregistre les param extrinseques dans C:\\AltaSight\\...\\EP_ref_CamFrame.txt
// enregistre aussi un mask dans C:\\AltaSight\\...\\UWMirrorMask.hbf//pas de lien avec ce que doit faire la fct mais pratique de le faire ici
long H3_DSLR_GetExtrinsic(H3_ARRAY2D_UINT8* pImageWafRef, const CString& calibFolder)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState())

        CString strSensorDataFile = calibFolder + "\\" + _CalibPaths._InputSettingsFile;
    CString strSection = _T("SensorHoloMap3");

    CString strCam_IntParam = _CalibPaths.CamCalibIntrinsicParamsPath(calibFolder);//calibrage camera// determiné ds ClikedCalib
    CString strCam_ExtParam = _CalibPaths.MatrixWaferToCameraPath(calibFolder);//position du wafer ref dans le repere camera//déterminé ici
    CString str_MaskDataFile = _CalibPaths.UWMirrorMaskPath(calibFolder);//position de la zone de calibrage (miroir plan) sur la mire//déterminé ici

    if (strCam_IntParam.IsEmpty())
        return -1;

    if (strCam_ExtParam.IsEmpty())
        return -2;

    CCameraCalibDlg Dlg(calibFolder), Dlg2(calibFolder);
    Dlg.LoadSettings(strSensorDataFile);
    Dlg2.LoadSettings(strSensorDataFile);

    Dlg.m_bCalibSystem = true;
    Dlg2.m_bCalibSystem = true;

    if (pImageWafRef != NULL)
    {
        Dlg.SetImage(*pImageWafRef);
    }

    Dlg.SetUse(TO_GET_CORNERS);

    INT_PTR ans = Dlg.DoModal();

    if (ans != IDOK)
    {
        return -3;
    }

    H3_ARRAY2D_PT2DFLT32 Pix = Dlg.GetPtIntersect();
    CH3CameraCalib C(calibFolder);
    bool b2 = true;

    b2 &= C.LoadSettings(strSensorDataFile);
    b2 &= (bool)C.LoadCalib(strCam_IntParam, 0);

    if (!b2)	//l'un des fichiers n'existe pas
    {
        return -4;
    }

    CExtrinsic_param EP;
    bool b3 = C.GetExtrinsic(EP, Pix);

    if (b3)
        EP.SaveCalib(strCam_ExtParam, 0);
    else
        return -5;

    // Mask pour le calcul de la position ecran via cartes de phases
//	AfxMessageBox("Selection de la zone du wafer sans mire pour calcul de phase");
    AfxMessageBox("Please select the area without grid wafer to perform phasis calculation");
    if (pImageWafRef != NULL)
    {
        Dlg2.SetImage(*pImageWafRef);
    }
    Dlg2.SetUse(TO_GET_AREA);
    ans = Dlg2.DoModal();

    if (ans != IDOK) {
        return -6;
    }

    //faire un masque avec ces point, masque qui sera utilisés lors du calibrage systeme
    H3_ARRAY2D_UINT8 mask = Dlg2.GetMask(C.ny, C.nx);
    mask.Save(str_MaskDataFile);

    Dlg.m_bCalibSystem = false;
    Dlg2.m_bCalibSystem = false;

    return 0;
}