using CIM300EXPERTLib;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Common
{
    public static class CConfiguration
    {
        // Angle beween aligner start position and coordinate origin : side robot's arm
        private static double m_Aligner_AngleOrigin = -1.0;

        // define the angle wise, when conterclockwise, value = 1 else (clockwise) value = -1, value initial = 0
        private static int m_AlignerRotationDirection = 0;

        public static String SoftwareName
        {
            get { return Win32Tools.GetIniFileString(CValues.CXTOOLCONTROL_FILE, "Tool Control", "SoftwareName", "ALTASight software"); ; }
        }

        public static enumEFEMConstructor EFEMConstructorName
        {
            get
            {
                // ecRORZE_JPN = 0, ecMECHATRONIC, ecRORZE_JPN_EC200, ecRORZE_TAI
                String lEFEMConstructor = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM", "EFEMConstructor", "Rorze").ToUpper();
                try
                {
                    return (enumEFEMConstructor)Enum.Parse(typeof(enumEFEMConstructor), "ec" + lEFEMConstructor);
                }
                catch 
                { 
                    return enumEFEMConstructor.ecUnknown; 
                }
            }
        }

        public static enumChamberType GetTypeChamber(String pFilePathName)
        {
            String strType = Win32Tools.GetIniFileString(pFilePathName, "Expert PM", "Type", "Unknown");
            switch (strType)
            {
                case "TOPOGRAPHY": return enumChamberType.ctTopography;
                case "DARKVIEW": return enumChamberType.ctDarkview;
                case "BRIGHTFIELD": return enumChamberType.ctBrightField;
                case "PSD": return enumChamberType.ctPSD;
                case "EDGE": return enumChamberType.ctEdge;
                case "REVIEW": return enumChamberType.ctReview;
                case "LIGHTSPEED": return enumChamberType.ctLightSpeed;
                case "PMTOOL": return enumChamberType.ctPMTool;
                default: return enumChamberType.ctNoProcessType;
            }
        }

        public static enumToolType GetToolTypeChamber(String pFilePathName)
        {
            String strType = Win32Tools.GetIniFileString(pFilePathName, "Expert PM", "ToolType", "Unknown");
            switch (strType)
            {
                case "CENTERING": return enumToolType.ttCentering;
                default: return enumToolType.ttNoToolType;
            }
        }

        public static String GetValueSection(bool pbBackside, String pFilePathName, String pKey)
        {
            enumChamberType lType = CConfiguration.GetTypeChamber(pFilePathName);
            switch (lType)
            {
                case enumChamberType.ctTopography:
                    if (!pbBackside)
                        return Win32Tools.GetIniFileString(pFilePathName, "Module Parameters Frontside", pKey, "Unknown");
                    else
                        return Win32Tools.GetIniFileString(pFilePathName, "Module Parameters Backside", pKey, "Unknown");

                case enumChamberType.ctDarkview:
                case enumChamberType.ctBrightField:
                case enumChamberType.ctReview:
                case enumChamberType.ctEdge:
                case enumChamberType.ctLightSpeed:
                    return GetValueSection(pFilePathName, pKey);

                case enumChamberType.ctPSD:
                    if (!pbBackside)
                        return Win32Tools.GetIniFileString(pFilePathName, "Module Parameters Frontside", pKey, "Unknown");
                    else
                        return Win32Tools.GetIniFileString(pFilePathName, "Module Parameters Backside", pKey, "Unknown");

                case enumChamberType.ctNoProcessType:
                default:
                    return "Unknown";
            }
        }

        public static String GetValueSection(String pFilePathName, String pKey)
        {
            return Win32Tools.GetIniFileString(pFilePathName, "Module Parameters", pKey, "Unknown");
        }

        public static int GetIntValueKey(bool pbBackside, String pFilePathName, String pKey, int pDefaultValue)
        {
            enumChamberType lType = CConfiguration.GetTypeChamber(pFilePathName);
            switch (lType)
            {
                case enumChamberType.ctTopography:
                    if (!pbBackside)
                        return Win32Tools.GetIniFileInt(pFilePathName, "Module Parameters Frontside", pKey, pDefaultValue);
                    else
                        return Win32Tools.GetIniFileInt(pFilePathName, "Module Parameters Backside", pKey, pDefaultValue);

                case enumChamberType.ctDarkview:
                case enumChamberType.ctBrightField:
                case enumChamberType.ctEdge:
                case enumChamberType.ctReview:
                case enumChamberType.ctLightSpeed:
                    return GetIntValueKey(pFilePathName, pKey, pDefaultValue);

                case enumChamberType.ctPSD:
                    if (!pbBackside)
                        return Win32Tools.GetIniFileInt(pFilePathName, "Module Parameters Frontside", pKey, pDefaultValue);
                    else
                        return Win32Tools.GetIniFileInt(pFilePathName, "Module Parameters Backside", pKey, pDefaultValue);

                case enumChamberType.ctNoProcessType:
                default:
                    return 0;
            }
        }

        public static int GetIntValueKey(String pFilePathName, String pKey, int pDefaultValue)
        {
            return Win32Tools.GetIniFileInt(pFilePathName, "Module Parameters", pKey, pDefaultValue);
        }

        //Module
        public static String GetModuleMachineName(String pFilePathName)
        {
            return GetValueSection(pFilePathName, "ServerName");
        }

        public static String GetModuleMachineName(bool pbBackside, String pFilePathName)
        {
            return GetValueSection(pbBackside, pFilePathName, "ServerName");
        }

        public static String GetModulePresentationImageFilePath(String pFilePathName)
        {
            String lResult = GetValueSection(pFilePathName, "PresentationImageFilePath");
            return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
        }

        public static short GetModulePortNum(String pFilePathName)
        {
            return Convert.ToInt16(GetIntValueKey(pFilePathName, "PortNum", 13600));
        }

        public static short GetModulePortNum(bool pbBackside, String pFilePathName)
        {
            return Convert.ToInt16(GetIntValueKey(pbBackside, pFilePathName, "PortNum", 13600));
        }

        public static String GetModuleViewerDir(String pFilePathName)
        {
            String lResult = GetValueSection(pFilePathName, "FrontViewerPath");
            return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
        }

        public static String GetModuleViewerDir(bool bBackside, String pFilePathName)
        {
            String lResult = "";
            if (!bBackside)
                lResult = GetValueSection(bBackside, pFilePathName, "FrontViewerPath");
            else
                lResult = GetValueSection(bBackside, pFilePathName, "BackViewerPath");

            return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
        }

        public static String GetModuleProcessDir(bool bBackside, String pFilePathName)
        {
            String lResult = "";
            if (!bBackside)
                lResult = GetValueSection(bBackside, pFilePathName, "FrontProcessPath");
            else
                lResult = GetValueSection(bBackside, pFilePathName, "BackProcessPath");

            return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
        }

        public static String GetModuleProcessDir(String pFilePathName)
        {
            String lResult = GetValueSection(pFilePathName, "ProcessDirectory");
            return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
        }

        public static String GetModuleADAFilePathName(bool bBackside, String pFilePathName)
        {
            String lResult = "";
            if (!bBackside)
                lResult = GetValueSection(bBackside, pFilePathName, "FrontAdaFilePath");
            else
                lResult = GetValueSection(bBackside, pFilePathName, "BackAdaFilePath");
            return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
        }

        public static String GetModuleADAFilePathName(String pFilePathName)
        {
            String lResult = GetValueSection(pFilePathName, "AdaFilePath");
            return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
        }

        public static int GetModuleADCCompletedTimeout(bool pbBackside, string pFilePathName)
        {
            int lValue = Convert.ToInt32(GetIntValueKey(false, pFilePathName, "ADCCompletedTimeout", 1800));
            if (lValue < 300)
                lValue = 300;
            return lValue;
        }

        public static string GetModuleImageExtension(string pFilePathName)
        {
            return GetValueSection(pFilePathName, "ImagesExtension");
        }

        public static bool GetModuleImportParametersEnabled(string pFilePathName)
        {
            return (GetIntValueKey(pFilePathName, "ImportParametersEnable", 0) == 1);
        }

        public static double MinProcessTimeInSec(string pFilePathName, String pProcessType)
        {
            if (pProcessType == "NONE")
                return 0;
            else
                return Convert.ToDouble(GetIntValueKey(false, pFilePathName, "MinimumProcessTimeInSec_Process" + pProcessType, 30));
        }

        public static double MinProcessTimeInSec(string pFilePathName)
        {
            return Convert.ToDouble(GetIntValueKey(false, pFilePathName, "MinimumProcessTimeInSec", 30));
        }

        // PSD
        public static bool GetBacksideEnabled(string pFilePathName)
        {
            return (Convert.ToInt32(GetIntValueKey(true, pFilePathName, "Enabled", 0)) == 1);
        }

        public static bool bDisableFFUPMAlarm(string pFilePathName)
        {
            String lResult = GetValueSection(pFilePathName, "DisableFFUPMAlarm");
            if (lResult.ToUpper() == "UNKNOWN")
                lResult = "0";
            return (Convert.ToInt32(lResult) == 1);
        }

        //Edge
        public static String GetEdgeMachineName
        {
            get { return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "ServerName", "Unknown"); }
        }

        public static String GrabEdgeADEFilePathName
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "EdgeADEFile", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeViewerDir
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Viewer", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeProcessDir
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Process", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeViewerDirSensorUp
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Viewer_SensorUp", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeProcessDirSensorUp
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Process_SensorUp", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeViewerDirSensorSide
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Viewer_SensorSide", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeProcessDirSensorSide
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Process_SensorSide", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeViewerDirSensorDown
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Viewer_SensorDown", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GrabEdgeProcessDirSensorDown
        {
            get
            {
                String ServerName = GetEdgeMachineName;
                String lResult = "\\\\" + ServerName + "\\" + Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "Edge", "Edge_Process_SensorDown", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static bool DisplayErrorWithGUI
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM", "DisplayErrorWithGUI", 0) == 1);
            }
        }

        public static bool ClearDeviceStepEqptIDAfterLaunchingJob
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "CleanDeviceStepEqmtIDAfterbLaunchingJob", 1) == 1);
            }
        }

        public static bool ShowEquipmentIDParameter
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "ShowEquipmentID", 1) == 1);
            }
        }

        public static String SortingRecipesPath
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AutoSplit", "SortingRecipePath", "Unknown");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String SortingType
        {
            get
            {
                //[AutoSplit]
                //ModeUsed = GRADING
                return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AutoSplit", "ModeUsed", "GRADING");
            }
        }

        public static bool SortingEnabled
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "AutoSplit", "Enabled", 0) == 1);
            }
        }

        public static int FDC_TriggeringPeriode_Minutes
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "AUTOMATION", "FDC_TriggeringPeriode_Minutes", 60);
            }
        }

        public static bool bMapperOnRobot
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "MapperOnRobot", 0) == 1);
            }
        }

        public static String GetModuleWaferIDReading_ImageFilePath(String pFilePathName)
        {
            return GetValueSection(pFilePathName, "WaferIDReading_ImageFilePath");
        }

        public static enumReaderType GetOCR_WaferIDType
        {
            get
            {
                enumReaderType lResultType = enumReaderType.rtNone;
                String lstrResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER", "WaferIDType", "NotFound");
                if (lstrResult != "NotFound")
                    lResultType = (enumReaderType)Convert.ToInt32(lstrResult);
                else
                    lResultType = enumReaderType.rtNone;
                return lResultType;
            }
        }

        public static enumOCReaderType GetOCR_ReaderType
        {
            get
            {
                enumOCReaderType lResultType;
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER", "ReaderType", "NotFound");
                if (lResult != "NotFound")
                    lResultType = (enumOCReaderType)Enum.Parse(typeof(enumOCReaderType), lResult);
                else
                    lResultType = enumOCReaderType.rtRORZE_DEFAULT;
                return lResultType;
            }
        }

        public static String GetOCR_IOSS_ImageDirectory
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER IOSS", "ImagesDirectory", "NotFound");
                if (lResult == "NotFound")
                    Win32Tools.WriteProfileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER IOSS", "ImagesDirectory", @"C:\CIMConnectProjects\Equipment1\OCR\Images");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GetOCR_IOSS_IPAddress
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER IOSS", "IPAddress", "NotFound");
                return lResult;
            }
        }

        public static String GetOCR_IOSS_RecipesDirectory
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER IOSS", "RecipesDirectory", "NotFound");
                return (lResult.EndsWith("\\") ? lResult : lResult + "\\");
            }
        }

        public static String GetOCR_IOSS_RecipesExtension
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER IOSS", "RecipesExtension", "NotFound");
                return lResult;
            }
        }

        public static int GetOCR_IOSS_AcceptanceThreshold
        {
            get
            {
                try
                {
                    String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER IOSS", "AcceptanceThreshold", "NotFound");
                    return ((lResult == "NotFound") ? 500 : Convert.ToInt32(lResult));
                }
                catch
                {
                    return 500;
                }
            }
        }

        public static String GetOCR_Mechatronic_ImageFilePathName
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER MECHATRONIC", "ImageFilePathName", "NotFound");
                if (lResult == "NotFound")
                    lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "WaferIDImageFilePathName", "NotFound");
                return lResult;
            }
        }

        public static String GetOCR_RT_ImageFilePathName
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER RORZETAI", "ImageFilePathName", "NotFound");
                if (lResult == "NotFound")
                    lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "WaferIDImageFilePathName", "NotFound");
                return lResult;
            }
        }

        public static int GetOCR_WaferSizeAvailable
        {
            get
            {
                int lResult = Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER", "WaferSizeAvailable", 0);
                return lResult;
            }
        }

        public static bool GetOCRCmd_oRIDO_Enable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER RORZETAI", "CmdOCRWithAlignmentEnable", 0) == 1);                
            }
        }

        public static String ADC_GetEventDefectResultAvailableName(enumConnection pServerType)
        {
            switch (pServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BACK:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_EDGE:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC EDGE", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF1:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF2:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF3:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 3", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF4:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 4", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_DF:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_PSD:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC PSD", "EventDefectResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_LS:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "EventDefectResultsAvailableName", "NotFound");

                default:
                    return "NotFound";
            }
        }

        public static String ADC_GetEventMeasurementResultAvailableName(enumConnection pServerType)
        {
            switch (pServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BACK:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_EDGE:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC EDGE", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF1:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF2:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF3:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 3", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF4:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 4", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_DF:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_PSD:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC PSD", "EventMeasurementResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_LS:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "EventMeasurementResultsAvailableName", "NotFound");

                default:
                    return "NotFound";
            }
        }

        public static String ClientApplication_GetEventName(AllEventID eventID)
        {
            switch (eventID)
            {
                case AllEventID.eEventAllPostProcess_Complete:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AUTOMATION", "PostProcessCompleteEventName", "NotFound");
                case AllEventID.eEventAllPostProcess_Failed:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AUTOMATION", "PostProcessFailedEventName", "NotFound");
                default:
                    return "";
            }
        }
        public static int PostProcess_GetDVID
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "AUTOMATION", "PostProcessCompleteDVID", -1);
            }
        }

        public static String ADC_GetEventAPCResultAvailableName(enumConnection pServerType)
        {
            switch (pServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BACK:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_EDGE:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC EDGE", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF1:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF2:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF3:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 3", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF4:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 4", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_DF:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_PSD:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC PSD", "EventAPCResultsAvailableName", "NotFound");

                case enumConnection.CONNECT_ADC_LS:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "EventAPCResultsAvailableName", "NotFound");

                default:
                    return "NotFound";
            }
        }

        public static String ADC_GetEventResultsErrorAvailableName(enumConnection pServerType)
        {
            switch (pServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_BACK:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_EDGE:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC EDGE", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_BF1:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_BF2:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_BF3:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 3", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_BF4:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 4", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_DF:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_PSD:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC PSD", "EventResultsErrorName", "NotFound");

                case enumConnection.CONNECT_ADC_LS:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "EventResultsErrorName", "NotFound");

                default:
                    return "NotFound";
            }
        }

        public static String ADC_GetFDCInfoVariableName(enumConnection pServerType)
        {
            switch (pServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_BACK:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_EDGE:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC EDGE", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF1:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF2:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF3:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 3", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_BF4:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 4", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_DF:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_PSD:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC PSD", "FDCInfoVariableName", "NotFound");

                case enumConnection.CONNECT_ADC_LS:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "FDCInfoVariableName", "NotFound");

                default:
                    return "NotFound";
            }
        }

        public static String ADC_GetValue(enumConnection pServerType, String key, String defaultValue)
        {
            switch (pServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", key, defaultValue);

                case enumConnection.CONNECT_ADC_BACK:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", key, defaultValue);

                case enumConnection.CONNECT_ADC_EDGE:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC EDGE", key, defaultValue);

                case enumConnection.CONNECT_ADC_BF1:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", key, defaultValue);

                case enumConnection.CONNECT_ADC_BF2:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", key, defaultValue);

                case enumConnection.CONNECT_ADC_BF3:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 3", key, defaultValue);

                case enumConnection.CONNECT_ADC_BF4:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 4", key, defaultValue);

                case enumConnection.CONNECT_ADC_DF:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", key, defaultValue);

                case enumConnection.CONNECT_ADC_PSD:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC PSD", key, defaultValue);

                case enumConnection.CONNECT_ADC_LS:
                    return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", key, defaultValue);

                default:
                    return defaultValue;
            }
        }

        public static int QRCodeFilterStart()
        {
            try
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER MECHATRONIC", "QRCodeFilterStart", 0);
            }
            catch
            {
                return 0;
            }
        }

        public static int QRCodeFilterNbChar()
        {
            try
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER MECHATRONIC", "QRCodeFilterNbChar", 0);
            }
            catch
            {
                return 0;
            }
        }

        public static int GetCarrierTypeDefinitionNbr
        {
            get
            {
                int lResult = Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "CarrierType", "NbCarrierType", 0);
                return lResult;
            }
        }

        public static int GetCarrierTypeEnable
        {
            get
            {
                int lResult = Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "CarrierType", "Enabled", 0);
                return lResult;
            }
        }

        public static enumCarrierTypeItemEnalbed GetCarrierTypeItemsEnabledFlags
        {
            get
            {
                int lResult = Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "CarrierType", "ItemsEnabled", 0);
                return (enumCarrierTypeItemEnalbed)lResult;
            }
        }

        public static bool GetbAskToConfirmWaferSizeOnLP
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "AskToConfirmWaferSizeOnLP", 1) == 1);
                return lResult;
            }
        }

        public static bool GetbAskToConfirmWaferThicknessOnLP
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "AskToConfirmWaferThicknessOnLP", 1) == 1);
                return lResult;
            }
        }

        public static bool GetbEnableAlignmentRetryAbort
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnableAlignmentRetryAbort", 0) == 1);
                return lResult;
            }
        }

        public static bool GetbEnableAlignmentRetryAbortInRemote
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnableAlignmentRetryAbortInRemote", 0) == 1);
                return lResult;
            }
        }

        public static bool GetbEnableAlignmentErrorClearAutomatically
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnableAlignmentErrorClearedAutomatically", 0) == 1);
                return lResult;
            }
        }

        public static bool GetbEnableMappingErrorClearedAutomatically
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnableMappingErrorClearedAutomatically", 0) == 1);
                return lResult;
            }
        }

        public static bool GetbEnableWaferIDReadErrorClearAutomatically
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnablewaferIDReadErrorClearedAutomatically", 0) == 1);
                return lResult;
            }
        }

        public static bool GetbEnable_MPPCSaturationTimeThresholdOverrun_ClearAutomatically
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "Enable_MPPCSaturationTimeThresholdOverrun_ClearAutomatically", 0) == 1);
                return lResult;
            }
        }

        public static enumOHTMethod GetOHTMethod
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.CIM300EXPERT_INI_FILE, "OHT", "Method", "NotFound");
                if (lResult == "EFEMContructor")
                    return enumOHTMethod.omEFEMConstructor;
                else
                    return enumOHTMethod.omDefault;
            }
        }

        public static bool bBuzzerEnabled
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "BuzzerEnable", "NotFound");
                if (lResult != "NotFound")
                    return (Convert.ToInt32(lResult) == 1);
                else
                    return false;
            }
        }

        public static bool bIsDatabaseSimulation
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "BuzzerEnable", "NotFound");
                if (lResult != "NotFound")
                    return (Convert.ToInt32(lResult) == 1);
                else
                    return false;
            }
        }

        public static bool b2ndAlignmentSensorEnabled
        {
            get
            {
                String lResult = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "2ndAlignmentExternalSensorEnable", "NotFound");
                if (lResult != "NotFound")
                    return (Convert.ToInt32(lResult) == 1);
                else
                    return false;
            }
        }

        public static int LoadportNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "LoadPort Count", 2);
            }
        }

        public static enumAlignerPlace AlignerPlace
        {
            get
            {
                if (Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "AlignerPlace", "Right").ToUpper() == "LEFT")
                    return enumAlignerPlace.apLeft;
                return enumAlignerPlace.apRight;
            }
        }

        public static bool bOnAbort_ProcessPMMustBeCompleted
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "General", "OnAbort_ProcessPMMustBeCompleted", 1) == 1);
            }
        }

        public static CLoadportDefinition GetLoadportDefinition(int pPortID)
        {
            CLoadportDefinition lLP = new CLoadportDefinition();
            String strLoadPortSection = "LoadPort_" + pPortID.ToString();
            lLP.PortID = pPortID;
            lLP.SubstrateType = LoadportSubstrateType(strLoadPortSection);
            lLP.LoadportType = Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, strLoadPortSection, "Type", 0);
            lLP.TagReaderAvailable = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, strLoadPortSection, "TagReaderAvailable", 0) == 1);
            return lLP;
        }

        public static int LoadportSubstrateType(String pLoadportSection)
        {
            String lLoadportSubstrateType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, pLoadportSection, "SubstrateType", "NotFound");
            if (lLoadportSubstrateType == "NotFound")
            {
                lLoadportSubstrateType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, pLoadportSection, "SubstrateType2", "NotFound");
                if (lLoadportSubstrateType == "NotFound")
                {
                    lLoadportSubstrateType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, pLoadportSection, "SubstrateType3", "NotFound");
                    if (lLoadportSubstrateType == "NotFound")
                        return -1;

                    // Configuration substrateType3 found
                    try
                    {
                        return Convert.ToInt32(lLoadportSubstrateType);
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
                else
                {
                    try
                    {
                        // Configuration substrateType2 found
                        // adaptation as substrateType3
                        int lCode = Convert.ToInt32(lLoadportSubstrateType);

                        if ((lCode & 1) == 1)
                            return lCode * 2 - 1; // Keep filmframe and add 0 to 3inches
                        else
                            return lCode * 2; // Add 0 to 3inches
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }
            else
            {
                // Configuration substrateType1 found
                // adaptation as substrateType3
                if (lLoadportSubstrateType == "1")
                    return 1;  // 1 = 0000 0001 => only film frame
                else
                    return 60; // 60 = 0011 1100 => all wafer size without film frame/3inches
            }
        }

        public static int ArmsType
        {
            get
            {
                String lArmsType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "ArmsType", "NotFound");
                if (lArmsType == "NotFound")
                {
                    lArmsType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "ArmsType2", "NotFound");
                    if (lArmsType == "NotFound")
                    {
                        lArmsType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "ArmsType3", "NotFound");

                        if (lArmsType == "NotFound")
                            return -1;
                        try
                        {
                            return Convert.ToInt32(lArmsType);
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        try
                        {
                            // Configuration ArmsType2 found
                            // adaptation as ArmsType3
                            int lCode = Convert.ToInt32(lArmsType);
                            int lCodeSup = Convert.ToInt32((lCode & 65280) / 255);
                            int lCodeInf = lCode & 255;

                            if (lCodeSup >= 32) return -1; // value is not valid upper than 0001 0000
                            if (lCodeInf >= 32) return -1; // value is not valid upper than 0001 0000

                            //High side
                            if ((lCodeSup & 1) == 1)
                                lCodeSup = lCodeSup * 2 - 1; // Keep filmframe and add 0 to 3inches
                            else
                                lCodeSup = lCodeSup * 2; // Add 0 to 3inches

                            //Low side
                            if ((lCodeInf & 1) == 1)
                                lCodeInf = lCodeInf * 2 - 1; // Keep filmframe and add 0 to 3inches
                            else
                                lCodeInf = lCodeInf * 2; // Add 0 to 3inches

                            lCode = lCodeSup * 256 + lCodeInf; // concat the both value
                            return lCode;
                        }
                        catch
                        {
                            return -1;
                        }
                    }
                }
                else
                {
                    // Configuration ArmsType1 found
                    // adaptation as ArmsType3
                    switch (lArmsType)
                    {
                        case "00":
                            return 15420; // 15420 = 0011 1100 0011 1100 => all wafer sizes and not film frame and not 3 inches
                        case "01":
                            return 15361; // 15361 = 0011 1100 0000 0001	=> Amr2 = all wafer sizes and not filmframe and not 3 inches; Arm1 = only film FRame
                        case "10":
                            return 316; //   316 = 0000 0001 0011 1100 => Arm2 = only film FRame; Amr1 = all wafer sizes and not filmframe and not 3inches;
                        case "11":
                            return 257;  //  257 = 0000 0001 0000 0001 => only film frame
                        default:
                            return -1;
                    }
                }
            }
        }

        public static int Aligner_SubstrateType
        {
            get
            {
                String lAlignerType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "Aligner_1", "SubstrateType", "NotFound");
                if (lAlignerType == "NotFound")
                {
                    lAlignerType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "Aligner_1", "SubstrateType2", "NotFound");
                    if (lAlignerType == "NotFound")
                    {
                        lAlignerType = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "Aligner_1", "SubstrateType3", "NotFound");
                        if (lAlignerType == "NotFound")
                            return -1;
                        else
                        {
                            // Configuration substrateType3 found
                            try
                            {
                                return Convert.ToInt32(lAlignerType);
                            }
                            catch (Exception)
                            {
                                return -1;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            // Configuration substrateType2 found
                            // adaptation as substrateType3
                            int lCode = Convert.ToInt32(lAlignerType);
                            if ((lCode & 1) == 1)
                                return lCode * 2 - 1; // Keep filmframe and add 0 to 3inches
                            else
                                return lCode * 2; // Add 0 to 3inches
                        }
                        catch
                        {
                            return -1;
                        }
                    }
                }
                else
                {
                    // Configuration substrateType1 found
                    // adaptation as substrateType3
                    if (lAlignerType == "1")
                        return 1;  // 1 = 0000 0001 => only film frame
                    else
                        return 60; // 60 = 0011 1100 => all wafer size without film frame/3inches
                }
            }
        }

        public static bool bDisplayMessageBeforeGoingInChamber
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "DisplayMessageBeforeGoingInPM", 1) == 1);
            }
        }

        public static bool bEnableTeachingExternalSensor
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnableTeachingExternalSensor", 0) == 1);
            }
        }

        public static double Aligner_AngleOrigin
        {
            get
            {
                String lAlignerAngleOrigin = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "Aligner_1", "AlignerAngleOrigin", "NotFound");
                if (lAlignerAngleOrigin == "NotFound")
                    lAlignerAngleOrigin = "180";
                m_Aligner_AngleOrigin = Convert.ToDouble(lAlignerAngleOrigin, CultureInfo.InvariantCulture);
                return m_Aligner_AngleOrigin;
            }
        }

        public static int AlignerRotationDirection
        {
            get
            {
                String lAlignerRotation = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "Aligner_1", "AlignerAlignSense", "NotFound");
                if (lAlignerRotation == "NotFound")
                    lAlignerRotation = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "Aligner_1", "AlignerRotationDirection", "NotFound");
                if (lAlignerRotation == "NotFound")
                    lAlignerRotation = "1";
                m_AlignerRotationDirection = Convert.ToInt32(lAlignerRotation, CultureInfo.InvariantCulture);
                return m_AlignerRotationDirection;
            }
        }

        public static List<int> GetInitUnitList()
        {
            List<int> lIDList = new List<int>();
            String lstrList = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Station Initialization", "UnitIDList", "NotFound");
            if (lstrList != "NotFound")
            {
                try
                {
                    String[] sTab = lstrList.Split(';');
                    for (int i = 0; i < sTab.Length; i++)
                    {
                        lIDList.Add(Convert.ToInt32(sTab[i]));
                    }
                }
                catch (Exception)
                {
                    lIDList.Clear();
                }
            }

            return lIDList;
        }

        public static int GetSpeeRobot
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "SpeedRobot", 50);
            }
        }

        public static int CurrentEFEMWorkingWaferSize
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "CurrentWorkingWaferSize", 8);
            }
            set
            {
                Win32Tools.WriteIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "CurrentWorkingWaferSize", value);
            }
        }

        public static bool bIsReaderOnAligner
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER", "ReaderIsOnAligner", 1) == 1);
            }
        }

        public static bool bDisplayWindowIfReadFailed
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER", "DisplayWindowToEnterWaferIDIfReadFailed", 1) == 1);
            }
        }

        public static bool bAreaSensorBlockedReverseSignalDetection
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "DIGITALIO", "AreaSensorBlockedReverseSignalDetection", 1) == 1);
            }
        }

        public static bool bEFEMDoorsReverseSignalDetection
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "DIGITALIO", "EFEMDoorsReverseSignalDetection", 0) == 1);
            }
        }

        public static int ArmNumberDisabled
        {
            get
            {
                int lArmNumberDisabled = Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "ArmNbrDisabled", -1);
                return lArmNumberDisabled;
            }
        }

        public static bool bDisplayLoadUnloadbuttonFromEFEM
        {
            get
            {
                bool lbLoadUnloadbuttonFromEFEMVisible = (Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "SimulationLoadUnloadButtonFromEFEM", "NotFound") == "1");
                return lbLoadUnloadbuttonFromEFEMVisible;
            }
        }

        public static bool GetbEnableRecipeNameInBacksideOCR
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER RORZETAI", "EnableRecipeNameInBackside", 1) == 1);
                return lResult;
            }
        }

        public static int WaferIDFilterStart()
        {
            try
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER RORZETAI", "WaferIDFilterStart", 0);
            }
            catch
            {
                return 0;
            }
        }

        public static int WaferIDFilterNbChar()
        {
            try
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "WAFERID READER RORZETAI", "WaferIDFilterNbChar", 0);
            }
            catch
            {
                return 0;
            }
        }

        public static int GetDelayToValidateSensorsChanging_ms(int pPortID)
        {
            try
            {
                String lstrLPId = "LoadPort_" + pPortID;
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, lstrLPId, "DelayToValidateSensorsChanging_ms", 600);
            }
            catch
            {
                return 0;
            }
        }

        public static bool bActivateDeactivateExternalSensorEnable
        {
            get
            {
                String lstrValue = Win32Tools.GetIniFileString(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "ActivateDeactivateExternalSensorEnable", "NotFound");
                if (lstrValue == "NotFound") // If NotFound, we consider this option is available because its configuration will disapear on long term.
                    lstrValue = "1";
                bool lbActivateDeactivateExternalSensorEnable = (lstrValue == "1");
                return lbActivateDeactivateExternalSensorEnable;
            }
        }

        public static int SizeFilesInMegaOctet
        {
            get { return Win32Tools.GetIniFileInt(CValues.CXTOOLCONTROL_FILE, "LOG", "SizeFilesInMegaOctet", 10); }
        }

        public static int FilesNbr
        {
            get { return Win32Tools.GetIniFileInt(CValues.CXTOOLCONTROL_FILE, "LOG", "FilesNbr", 10); }
        }

        public static int GetIOBitNumber_EFEMRT_IO(enumSysEventCodeIO_RT pIOName)
        {
            return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM RT IO", pIOName.ToString(), -1);
        }

        public static bool GetIOBitReverseSignal_EFEMRT_IO(enumSysEventCodeIO_RT pIOName)
        {
            return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM RT IO", pIOName.ToString() + "_ReverseSignal", -1) == 1);
        }

        public static bool GetIOBitEnable_EFEMRT_IO(enumSysEventCodeIO_RT pIOName)
        {
            return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM RT IO", pIOName.ToString() + "_Enabled", 0) == 1);
        }

        public static int GetFrameInput_BitNumber(int portID, enumFrmiEventCodeIO_RT pIOName)
        {
            return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FRAME PORT " + portID + " INPUT", pIOName.ToString() + "_BitNbr", -1);
        }

        public static bool GetFrameInput_ReverseSignal(int portID, enumFrmiEventCodeIO_RT pIOName)
        {
            return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FRAME PORT " + portID + " INPUT", pIOName.ToString() + "_ReverseSignal", -1) == 1);
        }

        public static bool GetFrameInput_Enable(int portID, enumFrmiEventCodeIO_RT pIOName)
        {
            return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FRAME PORT " + portID + " INPUT", pIOName.ToString() + "_Enabled", 0) == 1);
        }

        public static bool Get_EFEMRT_GotoCommandEnable
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "GotoCommandEnable", 0) == 1); }
        }

        public static int Get_TimeToHaveBetweenLoadingOrUnloading_ms
        {
            get { return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "TimeToHaveBetweenLoadingOrUnloading_ms", 2000); }
        }

        public static bool bDisplayWaferGUI_ChangeScale200
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "DisplayWaferGUI_ChangeScale200", 0) == 1); }
        }

        public static bool bPresencePlacementDetectionEnabled_EFEM_RT
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "LoadPort Configurations", "EFEM_RT_PresencePlacementDetectionEnabled", 1) == 1); }
        }

        public static bool bTurnCommandEnabled_EFEM_MECHATRONIC
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnableTurnCommand_MECHATRONIC", 0) == 1); }
        }

        public static bool bIsInterlockEventCriticalAlarm
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "IsInterlockEventCriticalAlarm", 1) == 1); }
        }

        public static bool bEFEMDoorAlarmDisabled
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "DisabledEFEMDoorAlarm", 0) == 1); }
        }

        public static bool bNeedSizeSelection
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EFEMNeedSizeSelection", 0) == 1); }
        }

        public static bool bIsEFEMSMIFSupported
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "SMIF", "Enable", 0) == 1); }
        }

        public static bool b3InchesReplace8Inches
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "Special Configuration", "3InchesReplaced8InchesEnabled", 0) == 1); }
        }

        public static bool DisplayPMPanelsOpened
        {
            get { return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "DisplayPMPanelsOpened", 0) == 1); }
        }

        // Used in case there is no multisize for loadport
        public static int GetWaferSizeAccordingToConfigurationSubtrateTypeCode(int pSubtrateTypeCode)
        {
            switch (pSubtrateTypeCode)
            {
                default:
                case 32:
                    return 12;

                case 16:
                    return 8;

                case 8:
                    return 6;

                case 4:
                    return 4;

                case 2:
                    return 3;
            }
        }

        // Used in case there is no multisize for loadport
        public static String GetMultiWaferSizeAccordingToConfigurationSubtrateTypeCode(int pSubtrateTypeCode)
        {
            String lSizeList = String.Empty;
            if ((pSubtrateTypeCode & 2) == 2)
                lSizeList = lSizeList + "3;";
            if ((pSubtrateTypeCode & 4) == 4)
                lSizeList = lSizeList + "4;";
            if ((pSubtrateTypeCode & 8) == 8)
                lSizeList = lSizeList + "6;";
            if ((pSubtrateTypeCode & 16) == 16)
                lSizeList = lSizeList + "8;";
            if ((pSubtrateTypeCode & 32) == 32)
                lSizeList = lSizeList + "12;";

            return lSizeList;
        }

        public static int GetConfigurationSubstrateTypeCodeAccordingToSize(int pWaferSize)
        {
            switch (pWaferSize)
            {
                default:
                case 12:
                    return 32;

                case 8:
                    return 16;

                case 6:
                    return 8;

                case 4:
                    return 4;

                case 3:
                    return 2;
            }
        }

        public static int GetFirstWaferSizeInInchesAccordingToConfigurationSubtrateTypeCode(enumSubstrateTypeDefinition pSubtrateTypeCode)
        {
            if ((pSubtrateTypeCode & enumSubstrateTypeDefinition.wtdSize300mm) == enumSubstrateTypeDefinition.wtdSize300mm) return 12;
            if ((pSubtrateTypeCode & enumSubstrateTypeDefinition.wtdSize200mm) == enumSubstrateTypeDefinition.wtdSize200mm) return 8;
            if ((pSubtrateTypeCode & enumSubstrateTypeDefinition.wtdSize150mm) == enumSubstrateTypeDefinition.wtdSize150mm) return 6;
            if ((pSubtrateTypeCode & enumSubstrateTypeDefinition.wtdSize100mm) == enumSubstrateTypeDefinition.wtdSize100mm) return 4;
            if ((pSubtrateTypeCode & enumSubstrateTypeDefinition.wtdSize75mm) == enumSubstrateTypeDefinition.wtdSize75mm) return 3;
            return 0;
        }

        public static String GetOrientationMarkUsed()
        {
            String FieldFound = Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "GENERAL", "UsingNotchFlat", "NotFound");
            if (FieldFound != "NotFound")
            {
                int iUseNotchFlat = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "UsingNotchFlat", 2);
                enumUseNotchFlat eUseNotchFlat = enumUseNotchFlat.nfAll;
                if ((iUseNotchFlat >= 0) && (iUseNotchFlat <= 2))
                    eUseNotchFlat = (enumUseNotchFlat)iUseNotchFlat;
                else
                    eUseNotchFlat = enumUseNotchFlat.nfAll;

                switch (eUseNotchFlat)
                {
                    case enumUseNotchFlat.nfAlwaysNotch:
                        return "Notch";

                    case enumUseNotchFlat.nfAlwaysFlat:
                        return "Flat";

                    case enumUseNotchFlat.nfAll:
                    default:
                        return "";
                }
            }
            else
            {
                FieldFound = Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "GENERAL", "OrientationMarkUsed", "NotFound");
                if (FieldFound != "NotFound")
                {
                    int iMarkType = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "OrientationMarkUsed", 3);
                    enumUseNotchFlat eUseNotchFlat = enumUseNotchFlat.nfAll;
                    if ((iMarkType >= 0) && (iMarkType <= 2))
                        eUseNotchFlat = (enumUseNotchFlat)iMarkType;
                    else
                        eUseNotchFlat = enumUseNotchFlat.nfAll;

                    switch (eUseNotchFlat)
                    {
                        case enumUseNotchFlat.nfAlwaysNotch:
                            return "Notch";

                        case enumUseNotchFlat.nfAlwaysFlat:
                            return "Flat";

                        case enumUseNotchFlat.nfAlwaysSubFlat:
                            return "SubFlat";

                        case enumUseNotchFlat.nfAll:
                        default:
                            return "";
                    }
                }
                else
                    return "";
            }
        }

        public static bool IsSubstrateTypeSupportedByPM(EFEMStationID pStationId, enumSubstrateTypeDefinition pSubstrateType)
        {
            int iExpertPM_Num = ((int)(pStationId - EFEMStationID.stidToolStation1)) + 1;
            enumSubstrateTypeDefinition lsubstrateTypePM = 0;
            for (int i = 1; i < 5; i++)
            {
                String lTypeFound = Win32Tools.GetIniFileString(CValues.CX_EXPERTPM + iExpertPM_Num.ToString() + ".ini", "Substrate Types", "Type" + i, "NotFound");
                if (lTypeFound != "NotFound")
                {
                    try
                    {
                        lsubstrateTypePM = (enumSubstrateTypeDefinition)Convert.ToInt32(lTypeFound);
                        if ((lsubstrateTypePM & pSubstrateType) == pSubstrateType)
                            return true;
                    }
                    catch
                    {
                        break;
                    }
                }
                else
                    break;
            }
            return false;
        }

        public static List<enumSubstrateTypeDefinition> GetAllPMSusbtrateTypes(EFEMStationID pStationId)
        {
            int iExpertPM_Num = ((int)(pStationId - EFEMStationID.stidToolStation1)) + 1;
            List<enumSubstrateTypeDefinition> lAllTypesFound = new List<enumSubstrateTypeDefinition>();

            String lTypeFound = Win32Tools.GetIniFileString(CValues.CX_EXPERTPM + iExpertPM_Num.ToString() + ".ini", "Substrate Types", "Type", "NotFound");
            if (lTypeFound != "NotFound")
            {
                try
                {
                    lAllTypesFound.Add((enumSubstrateTypeDefinition)Convert.ToInt32(lTypeFound));
                }
                catch
                {
                    lAllTypesFound.Clear();
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    lTypeFound = Win32Tools.GetIniFileString(CValues.CX_EXPERTPM + iExpertPM_Num.ToString() + ".ini", "Substrate Types", "Type" + i, "NotFound");
                    if (lTypeFound != "NotFound")
                    {
                        try
                        {
                            lAllTypesFound.Add((enumSubstrateTypeDefinition)Convert.ToInt32(lTypeFound));
                        }
                        catch
                        {
                            lAllTypesFound.Clear();
                            break;
                        }
                    }
                    else
                        break;
                }
            }
            return lAllTypesFound;
        }

        public static String Get_VidsToClearAsList
        {
            get
            {
                String lVids = Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AUTOMATION", "VidsToClearAsList", "NotFound");
                if (lVids == "NotFound")
                    return "";
                else
                    return lVids;
            }
        }

        public static String Get_VidsToClearAsDouble
        {
            get
            {
                String lVids = Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AUTOMATION", "VidsToClearAsDouble", "NotFound");
                if (lVids == "NotFound")
                    return "";
                else
                    return lVids;
            }
        }

        public static String Get_VidsToClearAsASCII
        {
            get
            {
                String lVids = Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AUTOMATION", "VidsToClearAsASCII", "NotFound");
                if (lVids == "NotFound")
                    return "";
                else
                    return lVids;
            }
        }

        public static String Get_VidsToClearAsU4
        {
            get
            {
                String lVids = Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "AUTOMATION", "VidsToClearAsU4", "NotFound");
                if (lVids == "NotFound")
                    return "";
                else
                    return lVids;
            }
        }

        public static int LogADCResult_DieCollectionMax
        {
            get { return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "AUTOMATION", "LogADCResult_DieCollectionMax", 10); }
        }

        public static bool bMPPCSaturationTimeMaximunExceeded_AbortJobEnable(string pFilePathName)
        {
            String lResult = GetValueSection(pFilePathName, "MPPCSaturationTimeMaximunExceeded_AbortJobEnable");
            if (lResult.ToUpper() == "UNKNOWN") lResult = "0";
            return (Convert.ToInt32(lResult) == 1);
        }

        public static int WaitingPostProcessResultsTimeout_sec(enumConnection m_ServerType)
        {
            switch (m_ServerType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                case enumConnection.CONNECT_GRAB_FRONT:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_ADC_BACK:
                case enumConnection.CONNECT_GRAB_BACK:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_GRAB_EDGE:
                case enumConnection.CONNECT_ADC_EDGE:
                case enumConnection.CONNECT_PMEDGE:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC EDGE", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_GRAB_DARKFIELD:
                case enumConnection.CONNECT_ADC_DF:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD1:
                case enumConnection.CONNECT_ADC_BF1:
                case enumConnection.CONNECT_ADC_BF_COMMON:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD2:
                case enumConnection.CONNECT_ADC_BF2:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD3:
                case enumConnection.CONNECT_ADC_BF3:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 3", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD4:
                case enumConnection.CONNECT_ADC_BF4:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 4", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.CONNECT_PMLS:
                case enumConnection.CONNECT_ADC_LS:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "WaitingPostProcessResultsTimeout_sec", 1200);
                case enumConnection.COUNT:
                default:
                    return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "AUTOMATION", "WaitingPostProcessResultsTimeout_sec", 1200);
            }

        }

        public static bool IsADCPostProcessCompleteEnable
        {
            get { return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "AUTOMATION", "ADCPostProcessComplete_Enable", 0) == 1); }
        }
        public static bool ThroughtputCalculationEnable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "ThroughtputCalculationEnable", 0) == 1);
            }
        }

        public static int ThroughtputWaferMaximumNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "ThroughtputWaferMaximumNumber", 0);
            }
        }

        public static String ThroughputFullFilePathName
        {
            get
            {
                return Win32Tools.GetIniFileString(CValues.PMALTASIGHT_INI, "GENERAL", "ThroughputFullFilePathName", "");
            }
        }

        public static bool GetbRetractArmOnReset
        {
            get
            {
                bool lResult = (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EnableRetractArmOnReset", 1) == 1);
                return lResult;
            }
        }

        public static bool bDisabled_VACUUM_EFEM_ALarm
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "DisabledVacuumEFEMAlarm", 0) == 1);
            }
        }

        public static bool bDisabled_FFU_EFEM_ALarm
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "DisabledFFUEFEMAlarm", 0) == 1);
            }
        }

        public static bool bEFEMLightCurtainPresent
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM Configuration", "EFEMLightCurtainPresent", 1) == 1);
            }
        }

        public static int RequiredFFUSpeed_RPM
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "RequiredSpeed_rpm", 1000);
            }
            set
            {
                Win32Tools.WriteIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "RequiredSpeed_rpm", value);
            }
        }

        public static int FFUSpeed_Tolerance_RPM
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "SpeedTolerance_rpm", 500);
            }
        }

        public static int RequiredFFUPressure_mPa
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "RequiredPressure_mPa", 1000);
            }
        }

        public static int FFUPressureTolerance_mPa
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "PressureTolerance_mPa", 500);
            }
        }

        public static int FFUMonitoringPeriod_sec
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "MonitoringPeriod_sec", 300);
            }
        }

        public static bool FFUSpeed_MonitoringEnable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "SpeedMonitoringEnable", 0) == 1);
            }
        }

        public static bool FFUPressure_MonitoringEnable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "PressureMonitoringEnable", 0) == 1);
            }
        }
        public static int FFULive_Duration
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "LiveDuration_minutes", 1);
            }
        }

        public static bool bFFUControlEnabled
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "FFUControlEnable", 0) == 1);
            }
        }
        public static bool bFFUSpeedReadValue_Enable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.RORZE_EFEM_CONFIGURATION, "EFEM FFU SETTINGS", "SpeedReadValue_Enable", 0) == 1);
            }
        }
        public static int ActiveJobsLimit
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "Cycling", "ActiveJobsLimit", 50);
            }
        }

        public static bool MultiRecipeEnable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "MultiRecipe option", "Enable", 0) == 1);
            }
        }
        public static bool CustomFieldEnable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "MultiRecipe option", "CustomFieldEnable", 0) == 1);
            }
        }
        public static int ADC_FormatVersionNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "ADC_FormatVerionNumber", 99);
            }
        }
        public static int ADCDisconnectionError_DisconnectionAcceptedNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "ADCDisconnectionError_DisconnectionAcceptedNumber", 1);
            }
        }
        public static int ADCAbortAllOnStartJob_DisconnectionAcceptedNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "ADCAbortAllOnStartJob_DisconnectionAcceptedNumber", 1);
            }
        }

        public static void ADCConfigurationSettings(enumConnection serverType, out bool isADCCheckingConnectionEnabled, out bool isADCEnabled)
        {
            switch (serverType)
            {
                case enumConnection.CONNECT_ADC_FRONT:
                    isADCCheckingConnectionEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "EnabledCheckingConnection", 0)==1);
                    isADCEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "Enabled", 0)==1);
                    break;

                case enumConnection.CONNECT_ADC_BACK:
                    isADCCheckingConnectionEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "EnabledCheckingConnection", 0)==1);
                    isADCEnabled =(Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "Enabled", 0)==1);
                    break;

                case enumConnection.CONNECT_ADC_EDGE:
                    isADCCheckingConnectionEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC EDGE", "EnabledCheckingConnection", 0)==1);
                    isADCEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC EDGE", "Enabled", 0)==1);
                    break;

                case enumConnection.CONNECT_ADC_BF1:
                    isADCCheckingConnectionEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "EnabledCheckingConnection", 0)==1);
                    isADCEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "Enabled", 0)==1);
                    break;

                case enumConnection.CONNECT_ADC_DF:
                    isADCCheckingConnectionEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "EnabledCheckingConnection", 0)==1);
                    isADCEnabled =(Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "Enabled", 0)==1);
                    break;

                case enumConnection.CONNECT_NANOTOPO:
                    isADCCheckingConnectionEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "Nanotopography", "EnabledCheckingConnection", 0)==1);
                    isADCEnabled =(Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "Nanotopography", "Enabled", 0)==1);
                    break;

                case enumConnection.CONNECT_ADC_LS:
                    isADCCheckingConnectionEnabled = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "EnabledCheckingConnection", 0)==1);
                    isADCEnabled =( Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "Enabled", 0)==1);
                    break;

                default:
                    isADCCheckingConnectionEnabled = false;
                    isADCEnabled=false;
                    break;
            }
        }

    }
}