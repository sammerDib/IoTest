namespace UnitySC.UTO.Controller.Remote.Constants
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "False positive")]
    internal static class ALIDs
    {
        public const int EfemAlarm = 303000;
        public const int ProcessModuleAlarm = 303001;
        public const int ControllerAlarm = 303002;
        public const int AlignerAlarm = 303003;
        public const int TcCommunicationTimeout = 303004;
        public const int CarrierIdReadFail = 303005;
        public const int EmergencyDoorOpened = 303006;
        public const int DoubleSlotDetected = 303007;
        public const int SpeedPressureDifferentialFailure = 303008;
        public const int MeasurementInterlock = 303009; //PM
        public const int RobotMovingFail = 303010;
        public const int RecipeValidationFailed = 303011; //PM
        public const int SoftwareFailed = 303012; //PM
        public const int SourceFail = 303013; //PM
        public const int StageFail = 303014; //PM
        public const int FanPowerOffAlarm = 303015; //PM
        public const int WaferBrokenInterlock = 303016;
        public const int DataFlowManagerAlarm = 303017;
        public const int SmokeDetected = 303018;
    }

    internal static class AlNames
    {
        public const string RemoteCommandFailed = "Remote command failed";
        public const string TcCommunicationTimeout = "TC communication timeout";
        public const string DoubleSlotDetected = "Double slot detected";
        public const string CarrierIDReadFail = "Carrier ID read fail";
    }

    internal static class E84Alarms
    {
        public const int TP1 = 843000;
        public const int TP2 = 843001;
        public const int TP3 = 843002;
        public const int TP4 = 843003;
        public const int TP5 = 843004;
        public const int TP6 = 843005;
        public const int HandOffError = 843006;
        public const int LightCurtainError = 843400;
    }
}
