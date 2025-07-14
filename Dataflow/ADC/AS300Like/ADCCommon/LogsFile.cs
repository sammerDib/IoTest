using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnitySC.ADCAS300Like.Common;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.ADCAS300Like.Common
{
    public static class LogObject
    {
        private const string ADC_COM_LOGS = @"C:\UnitySC\Logs\ADC_Comm.log";
        private const string ADC_FDC_LOGS = @"C:\UnitySC\Logs\ADC_FDCData.log";
        public const String CJMANAGER_LOGS = "C:\\UnitySC\\Logs\\CJManagerlogs.log";

        public static void ADCCommunicationLog(ADCType adcType, string strLogMessage, String dirPath="")
        {
            try
            {
                if (dirPath.IsNullOrEmpty())
                    Win32Tools.MyLog(strLogMessage, ADC_COM_LOGS, 10, 20);
                else
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);
                Win32Tools.MyLog(strLogMessage, dirPath + $"\\ADCComm_{adcType.ToString().Remove(0, 2)}-{DateTime.Now.ToString("yyyyMMdd")}.log", 10, 20);
            }
            catch
            { } // no log
        }

        public static void ADCFDCLog(string strLogMessage, String dirPath = "")
        {
            if (dirPath.IsNullOrEmpty())
                Win32Tools.MyLog(strLogMessage, ADC_FDC_LOGS, 10, 20);
            else
                Win32Tools.MyLog(strLogMessage, dirPath + "\\ADC_FDCData.log", 10, 20);
        }




        public static void PostProcessCLog(string strLogMessage, String dirPath = "")
        {
            if (dirPath.IsNullOrEmpty())
                Win32Tools.MyLog(strLogMessage, CJMANAGER_LOGS, 10, 20);
            else
                Win32Tools.MyLog(strLogMessage, dirPath + "\\CJManagerlogs.log", 10, 20);
        }
    }
}
