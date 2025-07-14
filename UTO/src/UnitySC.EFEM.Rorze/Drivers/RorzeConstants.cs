using Agileo.SemiDefinitions;

namespace UnitySC.EFEM.Rorze.Drivers
{
    public static class RorzeConstants
    {
        public const SampleDimension SubstrateDimension = SampleDimension.S300mm;
        public const Equipment.Abstractions.Enums.SubstrateType SubstrateType = Equipment.Abstractions.Enums.SubstrateType.Standard;

        public const int DefaultTcpPort          = 12000;
        public const int MaxReplyLength          = 100;
        public const int ClientConnectionTimeout = 1000;
        public const string TCPCommandPostfix    = "\r";
        public const string EndOfFrame           = "\x0d";

        public struct CommandTypeAbb
        {
            public const char Order  = 'o';
            public const char Ack    = 'a';
            public const char Nak    = 'n';
            public const char Cancel = 'c';
            public const char Event  = 'e';
        }

        public struct DeviceTypeAbb
        {
            public const string LoadPort = "STG";
            public const string Robot    = "TRB";
            public const string Aligner  = "ALN";
            public const string IO       = "DIO";
        }

        public struct Commands
        {
            public const string Connection = "CNCT";

            #region Common

            // Motion commands
            public const string OriginSearch = "ORGN";
            public const string StopMotion = "STOP";
            public const string PauseMotion = "PAUS";

            // Status operation commands
            public const string Event          = "EVNT";
            public const string ResetError     = "RSTA";
            public const string Initialize     = "INIT";
            public const string SetMotionSpeed = "SSPD";

            // Other commands
            public const string StatusAcquisition = "STAT";
            public const string GpioAcquisition   = "GPIO";
            public const string SetDateAndTime    = "STIM";
            public const string VersionAcquisition = "GVER";

            #endregion Common

            #region LoadPort

            // Motion commands
            public const string SecureCarrier  = "CLMP";
            public const string ReleaseCarrier = "UCLM";
            public const string PerformWaferMapping = "WMAP";
            public const string ReadCarrierId = "READ";

            // Other commands
            public const string MappingPatternAcquisition   = "GMAP";
            public const string StoppingPositionAcquisition = "GPOS";
            public const string CarrierTypeAcquisition      = "GWID";
            public const string SetCarrierType              = "SWID";
            public const string SetLight                    = "SPOT";

            //E84
            public const string E84Load = "LOAD";
            public const string E84Unload = "UNLD";

            #endregion LoadPort

            #region Robot

            // Motion commands
            public const string GoToPosVisitingHome   = "HOME";
            public const string LoadWafer             = "LOAD";
            public const string UnloadWafer           = "UNLD";
            public const string RetainWafer           = "CLMP";
            public const string ReleaseWaferRetention = "UCLM";
            public const string ExtendArm = "EXTD";
            public const string TransferWafer = "TRNS";
            public const string ExchangeWafer = "EXCH";

            // Other commands
            public const string GetWaferPresenceAndHistory = "GWID";

            #endregion Robot

            #region Aligner

            // Motion commands
            public const string ChuckSubstrate       = "CLMP";
            public const string CancelSubstrateChuck = "UCLM";
            public const string GoToHome             = "HOME";
            public const string Align                = "ALGN";

            // Other commands
            public const string GetSubstratePresence = "GWID";
            public const string SetSubstrateSize     = "SSIZ";
            public const string GetSubstrateSize     = "GSIZ";

            #endregion Aligner

            #region IO Cards

            public const string ExpansionIOSignalAcquisition = "GDIO";
            public const string ChangeOutputSignal           = "SDOU";

            #region DIO0

            public const string FanRotationalSpeedAcquisition    = "GREV";
            public const string PressureSensorsValuesAcquisition = "GPRS";
            public const string StartFanRotation                 = "MOVE";
            public const string StopFanRotation                  = "STOP";

            #endregion DIO0

            #endregion IO Cards
        }

        public struct SubCommands
        {
            public const string OriginSearch        = "ORGN";
            public const string Home                = "HOME";
            public const string TeachingOperation   = "STEP";
            public const string AbsolutePositionMovement = "EXTD";
            public const string PositionAcquisition = "GPOS";
            public const string DirectCommand       = "DCMD";
            public const string VersionAcquisition  = "GVER";

            public const string DataAcquisition    = "GTDT";
            public const string DataSetting        = "STDT";
            public const string DataInitialization = "INIT";
            public const string DataTransfer       = "TRDT";
            public const string ParameterAcquisition = "GPRM";
            public const string ParameterSetting   = "SPRM";
        }

        public struct ErrorCodes
        {
            public const string NoError = "00";
            public const string CarrierIdReadFail = "2100,";
        }

        public static int CharToInt(char input)
        {
            if (input >= '0' && input <= '9')
                return input - '0';
            if (input >= 'A' && input <= 'Z')
                return input - 'A' + 0xA;
            if (input >= 'a' && input <= 'z')
                return input - 'a' + 0xA;
            return -1;
        }
    }
}
