using System.Collections.Generic;

using ADCControls;

namespace AdcToRobot
{
    internal class AdcStatus
    {
        public static AdcStatus Instance = new AdcStatus();

        //-----------------------------------------------------------------
        // Variables Membres
        //-----------------------------------------------------------------
        public Dictionary<string, CWaferReport> WaferReportList = new Dictionary<string, CWaferReport>(); // clef = UniqueID
        public string LastErrorMessage;
        public bool IsConfigurationDatabaseConnected = true;
        public bool IsResultDatabaseConnected = true;

        public object mutex = new object();

        //=================================================================
        // 
        //=================================================================
        public CWaferReport GetOrCreateCWaferReport(string uniqueID)
        {
            CWaferReport creport;
            bool found = WaferReportList.TryGetValue(uniqueID, out creport);
            if (!found)
            {
                creport = new CWaferReport();
                creport.enWaferStatus = enWaferStatus.en_processing;
                WaferReportList.Add(uniqueID, creport);
            }

            return creport;
        }

    }
}
