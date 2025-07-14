using System;
using System.Collections.Generic;
using System.Text;

namespace UnitySC.ADCAS300Like.Common
{
    public enum ADCType { atPSD_FRONTSIDE = 0, atPSD_BACKSIDE, atEDGE, atLIGHTSPEED, atCOUNT }

    public class ADCItemConfig
    {
        public bool Enabled;
        public ADCType ADCType;
        public int ChamberID;
        public bool EnabledCheckingConnection;
        public bool EnabledCheckingRecipeExist;
        public string ServerName;
        public int PortNum;
        public bool AutoConnection;
        public int DelayAutoConnect_Sec;
        public int Timeout;
        public string EventDefectResultAvailableName;
        public string EventMeasurementResultAvailableName;
        public string EventResultsErrorName;
        public string EventAPCResultAvailableName;
        public string AdaInputPath;
    }
}
