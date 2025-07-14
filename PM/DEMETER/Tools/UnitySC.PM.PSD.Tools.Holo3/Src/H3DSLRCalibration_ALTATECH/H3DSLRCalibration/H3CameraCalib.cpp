// H3CameraCalib.cpp: implementation of the CH3CameraCalib class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "H3DSLRCalibration.h"
#include "H3CameraCalib.h"
#include "H3AppToolsDecl.h"
#include "H3Target.h"
#include "Extrinsic_param.h"
#include <stdio.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#define new DEBUG_NEW
#endif

#define DEFAULT_INI_FILE _T("c:\\temp\\mydefaultinifile.txt") 
#define MAX_REPROJECTION_ERROR_DEFAULT 0.2f

static CString strModule("CH3CameraCalib");

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CH3CameraCalib::CH3CameraCalib(const CString& calibFolder):
    m_Target(_CalibPaths.InputSettingsPath(calibFolder))
{
    m_CalibFolder = calibFolder;
    m_ID = _T("-1");
    m_nbImages = -1;

    m_Px.Alloc(0);
    H3SetDebugFile(_T("C:\\Temp\\Calib.log"));
    H3SetDebugLevel(H3_DEBUG_ENABLE);
    m_fReprojectionErrorMaxi = MAX_REPROJECTION_ERROR_DEFAULT;
    m_bIsCalibrated = false;
}

CH3CameraCalib::~CH3CameraCalib()
{
    if (!m_Pos.empty())
        m_Pos.clear();

#if XML_FILE
    delete file;
    file = NULL;
#endif
}

CH3CameraCalib& CH3CameraCalib::operator=(const CH3CameraCalib& CC)
{
    if (this == &CC) return *this;

    (*this).CH3Camera::operator=(CC);
    m_bIsCalibrated = CC.m_bIsCalibrated;
    m_fReprojectionErrorMaxi = CC.m_fReprojectionErrorMaxi;
    m_Mire = CC.m_Mire;
    m_nbImages = CC.m_nbImages;
    m_ID = CC.m_ID;

    m_Target = CC.m_Target;
    m_Pos = CC.m_Pos;

    return *this;
}

#if XML_FILE==0

bool CH3CameraCalib::LoadSettings(const CString& strFileName)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    CString strFunction("LoadSettings()");
    H3DebugInfo(strModule, strFunction, "");

    m_fReprojectionErrorMaxi = MAX_REPROJECTION_ERROR_DEFAULT;// H3GetPrivProfileFloat(strSection, _T("ReprojectionErrorMaxi"), MAX_REPROJECTION_ERROR_DEFAULT, strFileName); "ReprojectionErrorMaxi" does not seem to be part of the config/result files anymore.


    bool b = true;
    b &= m_Mire.LoadSettings(strFileName, _T("CH3CameraCalib_CH3Mire"));
    m_Target.LoadCalib(strFileName, 0);

    try
    {
        CH3Camera::LoadCalib(_CalibPaths.CamCalibIntrinsicParamsPath(m_CalibFolder), 0);
        m_bIsCalibrated = true;
    }
    catch (const std::exception&)
    {
        m_bIsCalibrated = false;
    }

    return b;
}

/////////////////////////////////////////////////////////////////////////////
// Enregistrement des reglages
/*!
* 	\fn      bool CH3CameraCalib::SaveSettings(CString strFileName,CString strSection)
* 	\author  M FERLET
* 	\brief   sauvegarde des parametres
* 	\param   CString strFileName : nom du fichier
* 	\param   CString strSection : nom de la section
* 	\return  bool
* 	\remarks
*/
bool CH3CameraCalib::SaveSettings(
    const CString& strFileName)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("SaveSettings()");
    H3DebugInfo(strModule, strFunction, "");

    if (m_bIsCalibrated == true)
    {
        SaveCalib(strFileName, 0);
    }
    else
    {
        SaveCalib(strFileName, 0);
    }

    //	H3WritePrivProfileFloat(strSection,_T("ReprojectionErrorMaxi"),m_fReprojectionErrorMaxi,strFileName);

    //	m_Mire.SaveSettings(strFileName,strSection+_T("CH3Mire"));
    m_Target.SaveCalib(strFileName, 0);

    return true;
}
#else
bool CH3CameraCalib::LoadSettings(
    H3XMLFile* file,
    CString strSection)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());

    CString strFunction("LoadSettings()");
    H3DebugInfo(strModule, strFunction, "");


    fc[0] = file->GetProfileFloat(strSection, _T("fc1"));
    fc[1] = file->GetProfileFloat(strSection, _T("fc2"));
    cc[0] = file->GetProfileFloat(strSection, _T("cc1"));
    cc[1] = file->GetProfileFloat(strSection, _T("cc2"));
    kc[0] = file->GetProfileFloat(strSection, _T("kc1"));
    kc[1] = file->GetProfileFloat(strSection, _T("kc2"));
    kc[2] = file->GetProfileFloat(strSection, _T("kc3"));
    kc[3] = file->GetProfileFloat(strSection, _T("kc4"));
    kc[4] = file->GetProfileFloat(strSection, _T("kc5"));
    alpha_c = file->GetProfileFloat(strSection, _T("alphac"));

    m_fReprojectionErrorMaxi = file->GetProfileFloat(strSection, _T("ReprojectionErrorMaxi"));

    m_bIsCalibrated = true;
    if (fc[0] == DEFAULT_FC1)
        if (fc[1] == DEFAULT_FC2)
            if (cc[0] == DEFAULT_CC1)
                if (cc[1] == DEFAULT_CC2)
                    if (kc[0] == DEFAULT_KC1)
                        if (kc[1] == DEFAULT_KC2)
                            if (kc[2] == DEFAULT_KC3)
                                if (kc[3] == DEFAULT_KC4)
                                    if (kc[4] == DEFAULT_KC5)
                                        if (alpha_c == DEFAULT_ALPHAC)
                                        {
                                            m_bIsCalibrated = false;
                                        }

    m_Mire.LoadSettings(file, strSection + _T("CH3Mire"));

    m_pH3Target->LoadCalib(file, 0);
    return true;
}

/////////////////////////////////////////////////////////////////////////////
// Enregistrement des reglages
/*!
* 	\fn      bool CH3CameraCalib::SaveSettings(CString strFileName,CString strSection)
* 	\author  M FERLET
* 	\brief   sauvegarde des parametres
* 	\param   CString strFileName : nom du fichier
* 	\param   CString strSection : nom de la section
* 	\return  bool
* 	\remarks
*/
bool CH3CameraCalib::SaveSettings(
    H3XMLFile* file,
    CString strSection)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("SaveSettings()");
    H3DebugInfo(strModule, strFunction, "");

    //////////////////////////////////////////////////////////////////////////////////
    // Section _General
    if (m_bIsCalibrated == true)
    {
        file->SetProfileFloat(strSection, _T("fc1"), fc[0]);
        file->SetProfileFloat(strSection, _T("fc2"), fc[1]);
        file->SetProfileFloat(strSection, _T("cc1"), cc[0]);
        file->SetProfileFloat(strSection, _T("cc2"), cc[1]);
        file->SetProfileFloat(strSection, _T("kc1"), kc[0]);
        file->SetProfileFloat(strSection, _T("kc2"), kc[1]);
        file->SetProfileFloat(strSection, _T("kc3"), kc[2]);
        file->SetProfileFloat(strSection, _T("kc4"), kc[3]);
        file->SetProfileFloat(strSection, _T("kc5"), kc[4]);
        file->SetProfileFloat(strSection, _T("alphac"), alpha_c);
    }
    else
    {
        file->SetProfileFloat(strSection, _T("fc1"), DEFAULT_FC1);
        file->SetProfileFloat(strSection, _T("fc2"), DEFAULT_FC2);
        file->SetProfileFloat(strSection, _T("cc1"), DEFAULT_CC1);
        file->SetProfileFloat(strSection, _T("cc2"), DEFAULT_CC2);
        file->SetProfileFloat(strSection, _T("kc1"), DEFAULT_KC1);
        file->SetProfileFloat(strSection, _T("kc2"), DEFAULT_KC2);
        file->SetProfileFloat(strSection, _T("kc3"), DEFAULT_KC3);
        file->SetProfileFloat(strSection, _T("kc4"), DEFAULT_KC4);
        file->SetProfileFloat(strSection, _T("kc5"), DEFAULT_KC5);
        file->SetProfileFloat(strSection, _T("alphac"), DEFAULT_ALPHAC);
    }
    file->SetProfileFloat(strSection, _T("ReprojectionErrorMaxi"), m_fReprojectionErrorMaxi);

    m_Mire.SaveSettings(file, strSection + _T("CH3Mire"));
    m_pH3Target->SaveCalib(file, 0);

    return true;
}
#endif
///////////////////////////////////////////////////////////////////////////////////////////////
bool CH3CameraCalib::AddImage(const H3_ARRAY2D_FLT32& Ima, H3_ARRAY2D_PT2DFLT32* pPt)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("AddImage(flt)");
    H3DebugInfo(strModule, strFunction, "");

    if (m_nbImages <= 0) {
        nx = Ima.GetCo();
        ny = Ima.GetLi();
        InitDefault(nx, ny);
    }
    else {
        if (nx != Ima.GetCo() || ny != Ima.GetLi())
        {
            H3DebugInfo(strModule, strFunction, "Image de taille distinct de la premiere");
            return false;
        }
    }

    m_nbImages++;

    H3_ARRAY2D_PT2DFLT32 Px;//position pixel des intersections de la mire dans l'image
    strPos  A_pos;
    A_pos.mb_isActive = false;

    //Recherche des intersection de chaque image
    CH3Target Target(m_CalibFolder);
    Target.LoadCalib(m_CalibFolder + "\\" + _CalibPaths._InputSettingsFile, 0);
    //Cherche les intersections de la mire 
    Px = Target.Find(Ima, 1, false);

    if (pPt != nullptr)
        (*pPt) = Px;

    long nMireTarget = m_Mire.GetNbTarget();

    if (Px.GetSize() != nMireTarget)
    {
        m_Pos.push_back(A_pos);

        H3DebugError(strModule, strFunction, "1");
        CString strMes;
        strMes.Format("Nombre d'intersections invalide\nAttendu : %d\nTrouvé : %d __1", nMireTarget, Px.GetSize());
        H3DisplayError(strMes);
        return false;//cv100712
    }
    else
    {
        H3_ARRAY2D_FLT32 Temp(2, Px.GetSize());
        for (UINT u = 0; u < Px.GetSize(); u++)
        {
            Temp(0, u) = Px[u].x;
            Temp(1, u) = Px[u].y;
        }

        CCorrespList2 CL(Temp, m_Mire.GetMetric());

        return AddImage(CL);
    }
}

bool CH3CameraCalib::AddImage(const H3_ARRAY2D_UINT8& Ima, H3_ARRAY2D_PT2DFLT32* pPt)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("AddImage(uint8)");
    H3DebugInfo(strModule, strFunction, "");

    if (m_nbImages <= 0) {
        H3DebugInfo(strModule, strFunction, "initdefault");
        nx = Ima.GetCo();
        ny = Ima.GetLi();
        InitDefault(nx, ny);
    }
    else {
        H3DebugInfo(strModule, strFunction, "init_deja fait");
        if (nx != Ima.GetCo() || ny != Ima.GetLi())
        {
            H3DebugInfo(strModule, strFunction, "Image de taille distinct de la premiere");
            return false;
        }
    }
    H3DebugInfo(strModule, strFunction, "0");
    m_nbImages++;

    H3_ARRAY2D_PT2DFLT32 Px;//position pixel des intersections de la mire dans l'image
    strPos  A_pos;
    A_pos.mb_isActive = false;

    //Recherche des intersection de chaque image
    //Cherche les intersections de la mire 
    Px = m_Target.Find(Ima, 1, false);

    if (pPt != NULL)
        (*pPt) = Px;

    long nMireTarget = m_Mire.GetNbTarget();

    if (Px.GetSize() != nMireTarget)
    {
        m_Pos.push_back(A_pos);

        H3DebugError(strModule, strFunction, "1");
        CString strMes;
        strMes.Format("Nombre d'intersections invalide\nAttendu : %d\nTrouvé : %d __1", nMireTarget, Px.GetSize());
        H3DisplayError(strMes);
        return false;
    }
    else
    {
        H3_ARRAY2D_FLT32 Temp(2, Px.GetSize());
        for (size_t u = 0; u < Px.GetSize(); u++)
        {
            Temp(0, u) = Px[u].x;
            Temp(1, u) = Px[u].y;
        }

        CCorrespList2 CL(Temp, m_Mire.GetMetric());

        if (CL.m_bInitialised)
            return AddImage(CL);
        else
            return false;
    }
}

bool CH3CameraCalib::AddImage(const CCorrespList2& CL)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("AddImage(CL)");
    H3DebugInfo(strModule, strFunction, "");

    strPos  A_pos;
    A_pos.mb_isActive = false;
    A_pos.m_CList = CL;

#if 1
    if (!CL.m_bInitialised) {
        CString msg;
        msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 0");
        H3DebugWarning(strModule, strFunction, msg);
        return false;
    }
    else {
        A_pos.mb_isActive = true;
        m_Pos.push_back(A_pos);
    }
#else
    if (CL.H.GetSize() == 0)
    {
        CString msg;
        msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 1");
        H3DebugWarning(strModule, strFunction, msg);

        m_Pos.push_back(A_pos);
        return false;
    }
    else
    {
        if (!_finite(A_pos.m_CList.H(0)))//en fait, ici, H n'est pas encore initialisée (Homographie initialisée par computeExtrinsic)
        {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 2");
            H3DebugWarning(strModule, strFunction, msg);

            m_Pos.push_back(A_pos);
        }
        else {
            A_pos.mb_isActive = true;
            m_Pos.push_back(A_pos);
        }
    }
#endif

    m_Px = CL.Pixel;

    return true;
}

bool CH3CameraCalib::AddImage(const H3_ARRAY2D_PT2DFLT32& CornersPos, const size_t ImagesLi, const size_t ImagesCo)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("AddImage(PT)");
    H3DebugInfo(strModule, strFunction, "");

    if (m_nbImages == 0) {
        nx = ImagesCo;
        ny = ImagesLi;
        InitDefault(nx, ny);
    }
    else {
        if (nx != ImagesCo || ny != ImagesLi) {
            H3DebugInfo(strModule, strFunction, "Image de taille distinct de la premiere");
            return false;
        }
    }
    m_nbImages++;

    strPos  A_pos;
    A_pos.mb_isActive = false;

    long nMireTarget = m_Mire.GetNbTarget();

    if (CornersPos.GetSize() != nMireTarget)
    {
        m_Pos.push_back(A_pos);

        H3DebugError(strModule, strFunction, "1");
        CString strMes;
        strMes.Format("Nombre d'intersections invalide\nAttendu : %d\nTrouvé : %d __2", nMireTarget, CornersPos.GetSize());
        H3DisplayError(strMes);
    }
    else
    {
        H3_ARRAY2D_FLT32 Temp(2, CornersPos.GetSize());
        for (size_t u = 0; u < CornersPos.GetSize(); u++)
        {
            Temp(0, u) = CornersPos[u].x;
            Temp(1, u) = CornersPos[u].y;
        }

        CCorrespList2 CL(Temp, m_Mire.GetMetric());
        A_pos.m_CList = CL;

#if 1
        if (!CL.m_bInitialised) {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 0");
            H3DebugWarning(strModule, strFunction, msg);
            return false;
        }
        else {
            A_pos.mb_isActive = true;
            m_Pos.push_back(A_pos);
        }
#else
        if (CL.H.GetSize() == 0)
        {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 1");
            H3DebugWarning(strModule, strFunction, msg);

            m_Pos.push_back(A_pos);
            return false;
        }
        else
        {
            if (!_finite(CL.H(0)))
            {
                CString msg;
                msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 2");
                H3DebugWarning(strModule, strFunction, msg);

                m_Pos.push_back(A_pos);
            }
            else {
                A_pos.mb_isActive = true;
                m_Pos.push_back(A_pos);
            }
        }
#endif
    }

    return true;
}

//bool CH3CameraCalib::GetExtrinsic(CExtrinsic_param& EP, const H3_ARRAY2D_UINT8 &Ima,bool OriginOnMire)const
//ima est une image de la mire d'ou va etre extrait Px
//Px: tableau (m_Mire.GetLi(),m_Mire.GetCo()) de position pixels (representant les pos pixel de sommets de la mire observée sur une image)
//out: parametre extrinseques de la mire
//brief: la mire est definie par m_Mire (coordonnées metriques des sommets)
bool CH3CameraCalib::GetExtrinsic(CExtrinsic_param& EP, const H3_ARRAY2D_UINT8& Ima, bool OriginOnMire)const
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("GetExtrinsic()");
    H3DebugInfo(strModule, strFunction, "");

    if (!m_bIsCalibrated)
        return false;

    if (m_nbImages == 0) {//pas possible car calibré
        return false;
    }
    else {
        if (nx != Ima.GetCo() || ny != Ima.GetLi()) {
            H3DebugInfo(strModule, strFunction, "la taille de l'Image est distincte de celle des image du calibrage");
            return false;
        }
    }

    H3_ARRAY_PT2DFLT32 Px;//position pixel des intersections de la mire dans l'image
    strPos  A_pos;

    //Recherche des intersection de chaque image
    CH3Target Target(m_CalibFolder);

    //Cherche les intersections de la mire 
    Px = Target.Find(Ima, 1, OriginOnMire);

    long nMireTarget = m_Mire.GetNbTarget();

    if (Px.GetSize() != nMireTarget)
    {
        H3DebugError(strModule, strFunction, "1");
        CString strMes;
        strMes.Format("Nombre d'intersections invalide\nAttendu : %d\nTrouvé : %d __3", nMireTarget, Px.GetSize());
        H3DisplayError(strMes);
    }
    else
    {
        H3_ARRAY2D_FLT32 Temp(2, Px.GetSize());
        for (size_t u = 0; u < Px.GetSize(); u++)
        {
            Temp(0, u) = Px[u].x;
            Temp(1, u) = Px[u].y;
        }

        CCorrespList2 CL(Temp, m_Mire.GetMetric());

#if 1
        if (!CL.m_bInitialised) {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 0");
            H3DebugWarning(strModule, strFunction, msg);
            return false;
        }
#else
        if (CL.H.GetSize() == 0)
        {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 1");
            H3DebugWarning(strModule, strFunction, msg);
            return false;
        }
        else
        {
            if (!_finite(CL.H(0)))
            {
                CString msg;
                msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 2");
                H3DebugWarning(strModule, strFunction, msg);
                return false;
            }
            else {
                EP.compute_extrinsic(CL, (*this));
            }
        }
#endif
    }

    return true;
}

//bool CH3CameraCalib::GetExtrinsic(CExtrinsic_param& EP, const H3_ARRAY2D_PT2DFLT32 &Px)const
//Px: tableau (m_Mire.GetLi(),m_Mire.GetCo()) de position pixels (representant les pos pixel de sommets de la mire observée sur une image)
//out: parametre extrinseques de la mire
//brief: la mire est definie par m_Mire (coordonnées metriques des sommets)
bool CH3CameraCalib::GetExtrinsic(CExtrinsic_param& EP, const H3_ARRAY2D_PT2DFLT32& Px)const
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());
    CString strFunction("GetExtrinsic(Px)");
    H3DebugInfo(strModule, strFunction, "");

    if (!m_bIsCalibrated)
        return false;

    if (m_nbImages == 0) {//pas possible car pas calibré
        return false;
    }

    strPos  A_pos;

    long nMireTarget = m_Mire.GetNbTarget();

    if (Px.GetSize() != nMireTarget)
    {
        H3DebugError(strModule, strFunction, "1");
        CString strMes;
        strMes.Format("Nombre d'intersections invalide\nAttendu : %d\nTrouvé : %d __4", nMireTarget, Px.GetSize());
        H3DisplayError(strMes);
    }
    else
    {
        H3_ARRAY2D_FLT32 Temp(2, Px.GetSize());
        for (size_t u = 0; u < Px.GetSize(); u++)
        {
            Temp(0, u) = Px[u].x;
            Temp(1, u) = Px[u].y;
        }

        CCorrespList2 CL(Temp, m_Mire.GetMetric());
#if 1
        if (!CL.m_bInitialised) {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 0");
            H3DebugWarning(strModule, strFunction, msg);
            return false;
        }
        else {
            EP.compute_extrinsic(CL, (*this));
        }
#else
        if (CL.H.GetSize() == 0)
        {
            CString msg;
            msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 1");
            H3DebugWarning(strModule, strFunction, msg);
            return false;
        }
        else
        {
            if (!_finite(CL.H(0)))
            {
                CString msg;
                msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 2");
                H3DebugWarning(strModule, strFunction, msg);
                return false;
            }
            else {
                EP.compute_extrinsic(CL, (*this));
            }
        }
#endif
    }

    return true;
}

void CH3CameraCalib::SetParamIntrin(const stParamIntrin& ParamIntrin)
{
    fc[0] = ParamIntrin.afc[0];
    fc[1] = ParamIntrin.afc[1];
    cc[0] = ParamIntrin.acc[0];
    cc[1] = ParamIntrin.acc[1];
    kc[0] = ParamIntrin.akc[0];
    kc[1] = ParamIntrin.akc[1];
    kc[2] = ParamIntrin.akc[2];
    kc[3] = ParamIntrin.akc[3];
    kc[4] = ParamIntrin.akc[4];
    kc[0] = ParamIntrin.akc[0];
    kc[1] = ParamIntrin.akc[1];
    alpha_c = ParamIntrin.aalphac[0];

    m_bIsCalibrated = true;
}

void CH3CameraCalib::SetParamIntrinErr(const stParamIntrinErr& ParamIntrin)
{
    fc_erreur[0] = ParamIntrin.afc_err[0];
    fc_erreur[1] = ParamIntrin.afc_err[1];
    cc_erreur[0] = ParamIntrin.acc_err[0];
    cc_erreur[1] = ParamIntrin.acc_err[1];
    kc_erreur[0] = ParamIntrin.akc_err[0];
    kc_erreur[1] = ParamIntrin.akc_err[1];
    kc_erreur[2] = ParamIntrin.akc_err[2];
    kc_erreur[3] = ParamIntrin.akc_err[3];
    kc_erreur[4] = ParamIntrin.akc_err[4];
    kc_erreur[0] = ParamIntrin.akc_err[0];
    kc_erreur[1] = ParamIntrin.akc_err[1];
    alpha_c_erreur = ParamIntrin.aalphac_err[0];
    pix_erreur[0] = ParamIntrin.apix_err[0];
    pix_erreur[1] = ParamIntrin.apix_err[1];
}

#define TEST_GETEXTRINSIC 0
bool CH3CameraCalib::Calib()
{
    size_t validImages = 0;

    vector<strPos>::iterator i;
    size_t j = 0;
    for (i = m_Pos.begin(); i != m_Pos.end(); ++i)
    {
        if ((*i).mb_isActive)
            validImages++;
    }

    CH3Array< strPos > a_strPos(validImages);
    for (i = m_Pos.begin(), j = 0; i != m_Pos.end(); ++i)
    {
        if ((*i).mb_isActive) {
            a_strPos[j] = (*i);
            j++;
        }
    }

    bool valid = calibrage(a_strPos);
    if (valid)
    {
        m_bIsCalibrated = true;

#if TEST_GETEXTRINSIC
        CExtrinsic_param EP;
        CString strIndex;
        for (i = m_Pos.begin(), j = 0; i != m_Pos.end(); ++i)
        {
            if ((*i).mb_isActive) {
                strIndex.Format("%d", j);
                (*i).m_ExtP.compute_extrinsic((*i).m_CList, *this);
                (*i).m_ExtP.Save("c:\\temp\\EP_calibCam.txt", strIndex);
            }
            j++;
        }
#endif
    }
    return valid;
}

void CH3CameraCalib::RemoveLastImage()
{
    if (m_nbImages > 0) {
        m_nbImages--;
        m_Pos.pop_back();
    }
}



