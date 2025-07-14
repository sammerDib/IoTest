#pragma once
#include <afxwin.h>
#include "NanoCalib.h"
class CMyCommandLineInfo :
	public CCommandLineInfo
{


    //   /CalibCamInput:"C:\Temp\Images Calib GlobalTopo\CalibCam" /MireSizeX:10 /MireSizeY:10 /MireStepX:7 /MireStepY:7 /CalibSysInput:"C:\Temp\Images Calib GlobalTopo\CalibSys" /PitchX:0.5 /PitchY:0.5 /PeriodX:16 /PeriodY:16 /ScreenSizeX:1900 /ScreenSizeY:1200 /ScreenRefPosX:950 /ScreenRefPosY:600 /CrossX:10 /CrossY:10 


    void CMyCommandLineInfo::ParseParam(const TCHAR* pszParam, BOOL bFlag, BOOL bLast)
    {

        if (bFlag)
        {
            CString param = pszParam;

       //     if (_stricmp(pszParam, "CalibCamInput") == 0)
       //     {
       ///*         m_FlagFound = true;
       //         m_ExpectFileNext = true;*/
       //     }

            if (strstr(pszParam, _T("CalibCamSourceImages")) != NULL)
            {
                m_clCalibCamSourceImagesFolder = GetStringParam(param);
            }

            if (strstr(pszParam, _T("MireSizeX")) != NULL)
            {
                m_clCalibCamMireSizeX=GetIntParam(param);
            }

            if (strstr(pszParam, _T("MireSizeY")) != NULL)
            {
                m_clCalibCamMireSizeY = GetIntParam(param);
            }

            if (strstr(pszParam, _T("MireStepX")) != NULL)
            {
                m_clCalibCamMireStepX = GetFloatParam(param);
            }

            if (strstr(pszParam, _T("MireStepY")) != NULL)
            {
                m_clCalibCamMireStepY = GetFloatParam(param);
            }

            if (strstr(pszParam, _T("CalibSysSourceImages")) != NULL)
            {
                m_clCalibSysSourceImagesFolder = GetStringParam(param);
            }

            if (strstr(pszParam, _T("PitchX")) != NULL)
            {
                m_clCalibSysPitchX = GetFloatParam(param);
            }

            if (strstr(pszParam, _T("PitchY")) != NULL)
            {
                m_clCalibSysPitchY = GetFloatParam(param);
            }

            if (strstr(pszParam, _T("PeriodX")) != NULL)
            {
                m_clCalibSysPeriodX = GetIntParam(param);
            }

            if (strstr(pszParam, _T("PeriodY")) != NULL)
            {
                m_clCalibSysPeriodY = GetIntParam(param);
            }

            if (strstr(pszParam, _T("ScreenSizeX")) != NULL)
            {
                m_clCalibSysScreenSizeX = GetIntParam(param);
            }

            if (strstr(pszParam, _T("ScreenSizeY")) != NULL)
            {
                m_clCalibSysScreenSizeY = GetIntParam(param);
            }

            if (strstr(pszParam, _T("ScreenRefPosX")) != NULL)
            {
                m_clCalibSysScreenRefPosX = GetIntParam(param);
            }

            if (strstr(pszParam, _T("ScreenRefPosY")) != NULL)
            {
                m_clCalibSysScreenRefPosY = GetIntParam(param);
            }

            if (strstr(pszParam, _T("CrossX")) != NULL)
            {
                m_clCalibSysCrossX = GetIntParam(param);
            }

            if (strstr(pszParam, _T("CrossY")) != NULL)
            {
                m_clCalibSysCrossY = GetIntParam(param);
            }
        }
   
      /*  if ((_wcsicmp(pszParam, L"l") == 0) && (bFlag == 1)) {
            m_bMyLFlag = 1;
        }
        if ((_wcsicmp(pszParam, L"d") == 0) && (bFlag == 1)) {
            m_bMyDFlag = 1;
        }*/
        //CCommandLineInfo::ParseParam(pszParam, bFlag, bLast);
    }

    int GetIntParam(CString param)
    {
        CString sParam = GetStringParam(param);
        return atoi(sParam);
    }

    float GetFloatParam(CString param)
    {
        CString sParam = GetStringParam(param);
        return atof(sParam);
    }

    CString GetStringParam(CString param)
    {
        param = param.Right(param.GetLength() - param.Find(':') - 1);
        param.Trim(' ');
        param.Trim('"');
        return param;
    }


};

