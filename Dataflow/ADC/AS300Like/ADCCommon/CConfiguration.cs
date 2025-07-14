using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnitySC.ADCAS300Like.Common
{
    public static class CConfiguration
    {
        // Used in case there is no multisize for loadport
        public static int LogADCResult_DieCollectionMax
        {
            get { return Win32Tools.GetIniFileInt(CValues.ADCConfigurationFile, "AUTOMATION", "LogADCResult_DieCollectionMax", 10); }
        }

        public static bool IsADCPostProcessCompleteEnable
        {
            get { return (Win32Tools.GetIniFileInt(CValues.ADCConfigurationFile, "AUTOMATION", "ADCPostProcessComplete_Enable", 0) == 1); }
        }
        public static bool ThroughtputCalculationEnable
        {
            get
            {
                return (Win32Tools.GetIniFileInt(CValues.ADCConfigurationFile, "GENERAL", "ThroughtputCalculationEnable", 0) == 1);
            }
        }


        public static int ADC_FormatVersionNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.ADCConfigurationFile, "GENERAL", "ADC_FormatVerionNumber", 99);
            }
        }
        public static int ADCDisconnectionError_DisconnectionAcceptedNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.ADCConfigurationFile, "GENERAL", "ADCDisconnectionError_DisconnectionAcceptedNumber", 1);
            }
        }
        public static int ADCAbortAllOnStartJob_DisconnectionAcceptedNumber
        {
            get
            {
                return Win32Tools.GetIniFileInt(CValues.ADCConfigurationFile, "GENERAL", "ADCAbortAllOnStartJob_DisconnectionAcceptedNumber", 1);
            }
        }


    }
}
