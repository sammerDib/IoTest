using System.Xml.Linq;

namespace UnitySC.UTO.Controller.Remote.Constants
{
    internal static class FileNames
    {
        public static readonly string MessagesConfiguration = "MessagesConfiguration.xml";
        public static readonly string MessagesConfigurationBackup = "MessagesConfiguration_Backup.xml";
        public static readonly string CommonMessagesConfiguration = "CommonMessagesConfiguration.xml";
        public static readonly string DataItemFormats = "DataItemFormats.xml";
        public static readonly string MessageDescriptions = "MessageDescriptions.xml";
    }

    internal static class ConfigurationXNames
    {
        // Secs2DataItems
        public static readonly XName Secs2DataItems = "Secs2DataItems";
        public static readonly XName Secs2DataItem = "Secs2DataItem";
        public static readonly XName Secs2DataItemName = "Name";
        public static readonly XName Secs2DataItemType = "Type";

        // Variables / HostConfig
        public static readonly XName HostConfig = "HostConfig";
        public static readonly XName Variables = "Variables";
        public static readonly XName VID = "VID";
        public static readonly XName VName = "VName";
        public static readonly XName VUnits = "Units";
        public static readonly XName VType = "Type";
        public static readonly XName VWellKnowName = "WellKnowName";

        // SV
        public static readonly XName SV = "SV";
        public static readonly XName SVCEID = "CEID";

        // DV
        public static readonly XName DV = "DV";

        // HC
        public static readonly XName HC = "HC";

        // EC
        public static readonly XName EC = "EC";
        public static readonly XName ECValue = "Value";
        public static readonly XName ECMin = "Min";
        public static readonly XName ECMax = "Max";
        public static readonly XName ECDescription = "Description";

        // Reports
        public static readonly XName Reports = "Reports";
        public static readonly XName Report = "Report";
        public static readonly XName RERPTID = "RPTID";
        public static readonly XName RERPTName = "RPTName";
        public static readonly XName REVIDs = "VIDs";
        public static readonly XName REVID = "VID";

        // CollectionEvents
        public static readonly XName CollectionEvents = "CollectionEvents";
        public static readonly XName CollectionEvent = "CollectionEvent";
        public static readonly XName CEID = "CEID";
        public static readonly XName CEName = "CEName";
        public static readonly XName CEEnabled = "Enabled";
        public static readonly XName CERPTIDs = "RPTIDs";
        public static readonly XName CERPTID = "RPTID";
        public static readonly XName CEValidDVIDs = "ValidDVIDs";
        public static readonly XName CEValidDVID = "ValidDVID";

        // Alarms
        public static readonly XName Alarms = "Alarms";
        public static readonly XName Alarm = "Alarm";
        public static readonly XName ALID = "ALID";
        public static readonly XName ALCD = "ALCD";
        public static readonly XName ALTX = "ALTX";
        public static readonly XName ALName = "ALName";
        public static readonly XName ALEnabled = "Enabled";
        public static readonly XName ALSetCEID = "SetCEID";
        public static readonly XName ALClearCEID = "ClearCEID";

        // CommunicationPiping
        public static readonly XName CommunicationPiping = "CommunicationPiping";
        public static readonly XName CPIsAllowedOut = "IsAllowedOut";
        public static readonly XName CPIsAllowedIn = "IsAllowedIn";
        public static readonly XName CPMessages = "Messages";
        public static readonly XName CPMessage = "Message";
        public static readonly XName CPStream = "Stream";
        public static readonly XName CPFunction = "Function";
        public static readonly XName CPControlStates = "ControlStates";
        public static readonly XName CPControlState = "ControlState";

        // LimitsMonitoring
        public static readonly XName LimitsMonitoring = "LimitsMonitoring";
        public static readonly XName LMMonitoringVar = "MonitoringVar";
        public static readonly XName LMVID = "VID";
        public static readonly XName LMLimit = "Limit";
        public static readonly XName LMID = "ID";
        public static readonly XName LMUpperBound = "UpperBound";
        public static readonly XName LMLowerBound = "LowerBound";
        public static readonly XName LMIsEnabled = "IsEnabled";
    }
}
