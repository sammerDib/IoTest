namespace UnitySC.UTO.Controller.Logging
{
    public static class EventLogConstant
    {
        public const string EventLog = nameof(EventLog);
        public const string Header = "Applied Samsung Standard Logging Spec Version: 2.0";
        public const string EventLogFolderPath = @"C:\Logs\EventLog";

        public class TransferLog
        {
            public const string LogType = "XFR";

            public const string Move = nameof(Move);
            public const string Get = nameof(Get);
            public const string Put = nameof(Put);
            public const string Rotate = nameof(Rotate);
            public const string Exchange = nameof(Exchange);
            public const string Extend = nameof(Extend);
            public const string Retract = nameof(Retract);
            public const string Pick = nameof(Pick);
            public const string Place = nameof(Place);
        }

        public class LotEventLog
        {
            public const string LogType = "LEH";

            public const string CarrierLoad = nameof(CarrierLoad);
            public const string CarrierUnLoad = nameof(CarrierUnLoad);
            public const string ProcessJobStart = nameof(ProcessJobStart);
            public const string ProcessJobEnd = nameof(ProcessJobEnd);
        }

        public class FunctionLog
        {
            public const string LogType = "FNC";

            public const string AlignerStageUp = nameof(AlignerStageUp);
            public const string AlignerStageDown = nameof(AlignerStageDown);
            public const string Align = nameof(Align);

            public const string LPPIO = nameof(LPPIO);
            public const string LPChuck = nameof(LPChuck);
            public const string LPDeChuck = nameof(LPDeChuck);
            public const string LPForward = nameof(LPForward);
            public const string LPBackward = nameof(LPBackward);
            public const string ReadID = nameof(ReadID);
            public const string FoupDoorOpen = nameof(FoupDoorOpen);
            public const string FoupDoorClose = nameof(FoupDoorClose);
            public const string Mapping = nameof(Mapping);
        }

        public class ConfigurationLog
        {
            public const string LogType = "CFG";
        }
    }
}
