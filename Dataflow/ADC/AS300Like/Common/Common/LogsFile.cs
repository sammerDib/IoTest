using System;
using System.Collections.Generic;
using System.Text;

using Common;

namespace Common
{
    public static class LogObject
    {
        private const string PMATALSIGHT_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMAltasightLog.txt";
        private const string PMPSD_FRONT_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMPSDFrontLog.txt";
        private const string PMPSD_BACK_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMPSDBackLog.txt";
        private const String PMDARKFIELD_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMDarkfieldLog.txt";
        private const String PMEDGE_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMEdgeLog.txt";
        private const String PMREVIEW_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMReviewLog.txt";
        private const String PMDARKFIELD_LOG_FILE_NAME = "PMDarkfieldLog";
        private const String PMBRIGHTFIELD1_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMBrightField1Log.txt";
        private const String PMBRIGHTFIELD1_LOG_FILE_NAME = "PMBrightField1Log";
        private const String PMBRIGHTFIELD2_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMBrightField2Log.txt";
        private const String PMBRIGHTFIELD2_LOG_FILE_NAME = "PMBrightField2Log";
        private const String PMBRIGHTFIELD3_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMBrightField3Log.txt";
        private const String PMBRIGHTFIELD3_LOG_FILE_NAME = "PMBrightField3Log";
        private const String PMBRIGHTFIELD4_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMBrightField4Log.txt";
        private const String PMBRIGHTFIELD4_LOG_FILE_NAME = "PMBrightField4Log";
        private const String PMLIGHTSPEED_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PMLightspeedLog.txt";
        private const String PMLIGHTSPEED_LOG_FILE_NAME = "PMLightspeedLog";
        private const String PMERROR_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\PmErrorsLogs.txt";
        private const String PMERROR_LOG_FILE_NAME = "PmErrorsLogs";
        private const String INIT_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\InitLogs.txt";
        private const String INIT_LOG_FILE_NAME = "InitLogs";
        private const string ADC_RESULTS_LOGS = @"C:\CIMConnectProjects\Equipment1\Projects\Logs\ADC_Results.txt";
        private const string ADC_FDC_LOGS = @"C:\CIMConnectProjects\Equipment1\Projects\Logs\ADC_FDCData.txt";
        public const string RECIPE_CHANGE_LOGS_PATH = @"C:\CIMConnectProjects\Equipment1\RecipesManagerFolder\Historic\";
        private const String E87_LOG_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\logs\\LoadportStateLog.txt";
        private const String E87_LOG_FILE_NAME = "LoadportStateLog";
        public const string RECIPE_CHANGE_EVENTS_LOGS_FILE_NAME = @"C:\CIMConnectProjects\Equipment1\Projects\logs\RecipeChangeEventLog.txt";
        public const String CEFEM_LOGS = @"C:\CIMConnectProjects\Equipment1\Projects\logs\CEFEMLog.txt";
        public const String RECIPE_ERROR_LOGS = @"C:\CIMConnectProjects\Equipment1\Projects\logs\RecipeErrorLog.txt";
        public const String APP_ERROR_LOGS = @"C:\CIMConnectProjects\Equipment1\Projects\logs\AppErrorLog.txt";

        public const String CJMANAGER_LOGS = "C:\\CIMConnectProjects\\Equipment1\\Projects\\Logs\\CJManagerlogs.txt";
        public const String CJMANAGER_NAME = "CJManagerlogs";
        public const String FFU_LOGS = "C:\\CIMConnectProjects\\Equipment1\\Projects\\Logs\\FFUlogs.txt";
        public const String FFU_LOGS_NAME = "FFULog";
        public const String CYCLE_PROGRAM_LOGS = "C:\\CIMConnectProjects\\Equipment1\\Projects\\Logs\\CycleProgramlogs.txt";
        public const String CYCLE_PROGRAM_LOGS_NAME = "CycleProgram";

        //--------------------------------------------------------------------------------------------------------------
        //Log
        public static void InitLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, INIT_LOG_FILE, 10, 20);
        }

        public static void PMErrorLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, PMERROR_LOG_FILE, 10, 20);
        }

        public static void PMDarkfieldLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, PMDARKFIELD_LOG_FILE, 10, 20);
            //Win32Tools.MyBackup(PMATALSIGHT_LOG_FILE, ref m_PMALogCount, ref m_PMALogFileNum);
        }

        public static void CEFEMLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, CEFEM_LOGS, 10, 20);
            //Win32Tools.MyBackup(PMATALSIGHT_LOG_FILE, ref m_PMALogCount, ref m_PMALogFileNum);
        }

        public static void PMLog(enumConnection pServerType, string strLogMessage)
        {
            switch (pServerType)
            {
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD1: Win32Tools.MyLog(strLogMessage, PMBRIGHTFIELD1_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD2: Win32Tools.MyLog(strLogMessage, PMBRIGHTFIELD2_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD3: Win32Tools.MyLog(strLogMessage, PMBRIGHTFIELD3_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_BRIGHTFIELD4: Win32Tools.MyLog(strLogMessage, PMBRIGHTFIELD4_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_DARKFIELD: Win32Tools.MyLog(strLogMessage, PMDARKFIELD_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_EDGE: Win32Tools.MyLog(strLogMessage, PMEDGE_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_FRONT: Win32Tools.MyLog(strLogMessage, PMPSD_FRONT_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_BACK: Win32Tools.MyLog(strLogMessage, PMPSD_BACK_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_PSD: Win32Tools.MyLog(strLogMessage, PMPSD_FRONT_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_PMEDGE: Win32Tools.MyLog(strLogMessage, PMEDGE_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_GRAB_REVIEW: Win32Tools.MyLog(strLogMessage, PMREVIEW_LOG_FILE, 10, 20); break;
                case enumConnection.CONNECT_PMLS: Win32Tools.MyLog(strLogMessage, PMLIGHTSPEED_LOG_FILE, 10, 20); break;
                default: Win32Tools.MyLog("LogFile not found -  " + strLogMessage, PMBRIGHTFIELD1_LOG_FILE, 10, 20); break;
            }
        }

        public static void PMAltasightLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, PMATALSIGHT_LOG_FILE, 10, 20);
            //Win32Tools.MyBackup(PMATALSIGHT_LOG_FILE, ref m_PMALogCount, ref m_PMALogFileNum);
        }

        public static void ADCResultsLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, ADC_RESULTS_LOGS, 10, 20);
        }

        public static void ADCFDCLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, ADC_FDC_LOGS, 10, 20);
        }

        public static void RecipeChangeLog(String pNameFile, string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, pNameFile, 10, 20);
        }

        public static void E87Log(String strLogMessage, int pSizeInMo, int pRotationNbr)
        {
            Win32Tools.MyLog(strLogMessage, E87_LOG_FILE, pSizeInMo, pRotationNbr);
        }

        public static void ReciepErrorLog(String strLogMessage)
        {
            bool lbEnable = (Win32Tools.LogParametersEnable(CValues.CXTOOLCONTROL_FILE, "RECIPEErrorLogs") == 1);
            if (lbEnable)
            {
                int lRotationNbr = Win32Tools.LogParametersRotationNbrFiles(CValues.CXTOOLCONTROL_FILE, "RECIPEErrorLogs");
                int lSizeInMo = Win32Tools.LogParametersSizeInMo(CValues.CXTOOLCONTROL_FILE, "RECIPEErrorLogs");
                Win32Tools.MyLog(strLogMessage, RECIPE_ERROR_LOGS, lSizeInMo, lRotationNbr);
            }
        }

        public static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Win32Tools.MyLog(e.Exception.Message + "-StackTrace: " + e.Exception.StackTrace, APP_ERROR_LOGS, 10, 20);
        }

        public static void PostProcessCLog(string strLogMessage)
        {
            Win32Tools.MyLog(strLogMessage, CJMANAGER_LOGS, 10, 20);
        }
    }
}