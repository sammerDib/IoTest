using System;
using System.Collections.Generic;

using Agileo.Drivers;

using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EFEM.Controller.HostInterface
{
    public static class Constants
    {
        public const string EndOfFrame         = "\x0d";
        public const string CommandSeparator   = ":"; // Separate command name from arguments
        public const string ParameterSeparator = "/"; // Separate the list of parameters
        public const string ArgumentSeparator  = "_"; // Separate the list of arguments (inside a single parameter)

        public static IReadOnlyDictionary<char, Ratio> RobotSpeeds { get; } = new Dictionary<char, Ratio>
        {
            { '0', Ratio.FromPercent(005) },
            { '1', Ratio.FromPercent(010) },
            { '2', Ratio.FromPercent(015) },
            { '3', Ratio.FromPercent(020) },
            { '4', Ratio.FromPercent(025) },
            { '5', Ratio.FromPercent(030) },
            { '6', Ratio.FromPercent(035) },
            { '7', Ratio.FromPercent(040) },
            { '8', Ratio.FromPercent(045) },
            { '9', Ratio.FromPercent(050) },
            { 'A', Ratio.FromPercent(055) },
            { 'B', Ratio.FromPercent(060) },
            { 'C', Ratio.FromPercent(065) },
            { 'D', Ratio.FromPercent(070) },
            { 'E', Ratio.FromPercent(075) },
            { 'F', Ratio.FromPercent(080) },
            { 'G', Ratio.FromPercent(085) },
            { 'H', Ratio.FromPercent(090) },
            { 'I', Ratio.FromPercent(095) },
            { 'J', Ratio.FromPercent(100) },
        };

        public struct CommandType
        {
            public const char Order  = 'o';
            public const char Ack    = 'a';
            public const char Event  = 'e';
            public const char Cancel = 'c';
        }

        public struct Commands
        {
            // Common
            public const string Initialize         = "INIT";
            public const string SetLightTowerState = "STwr";
            public const string GetSystemStatus    = "SYSI";
            public const string GetGeneralStatuses = "STAT";
            public const string SetBuzzer          = "BZON";
            public const string GetPressure        = "GPRS";
            public const string SetTime            = "TIME";
            public const string SetFfuRpm          = "FFUS";
            public const string GetFfuRpm          = "GFAN";

            // LoadPort
            public const string Read                      = "READ";
            public const string Dock                      = "DOCK";
            public const string Undock                    = "UNDK";
            public const string GetMappingPattern         = "GMAP";
            public const string GetCarrierCapacityAndSize = "WFMX";
            public const string CarrierPresence           = "LPSR";
            public const string GetWaferSizeInLoadPort    = "LPSZ";
            public const string PerformWaferMapping       = "WMAP";
            public const string CloseDoor                 = "LPCL";
            public const string ClampOnLp                 = "LPCP";
            public const string SetLightOnLp              = "LPLO";
            public const string SetCarrierType            = "SCTY";
            public const string GetCarrierType            = "GCTY";

            // E84
            public const string EnableOrDisableE84  = "SE84";
            public const string SetTimeoutsE84      = "E84T";
            public const string GetE84InputSignals  = "E84I";
            public const string GetE84OutputSignals = "E84O";
            public const string GetE84Error         = "S84E";
            public const string SetE84OutputSignals = "E84S";
            public const string ResetE84 = "E84R";
            public const string AbortE84 = "E84A";

            // Robot
            public const string Home                  = "HOME";
            public const string Load                  = "LOAD";
            public const string ClampOnArm            = "RBCP";
            public const string Unload                = "UNLD";
            public const string PreparePick           = "WGET";
            public const string PreparePlace          = "WPUT";
            public const string SetRobotSpeed         = "SSPD";
            public const string GetWaferPresenceOnArm = "WARM";

            // Aligner
            public const string Align          = "ALGN";
            public const string ClampOnAligner = "ALCP";
            public const string GetOcrRecipes  = "GREC";
            public const string ReadWaferId    = "RDID";
            public const string ReadWaferIdOnly = "RIDO";
            public const string Centering       = "CNTR";
        }

        public enum Arm
        {
            Upper = 1,
            Lower = 2,
            Both  = 3
        }

        public enum Port
        {
            LP1       = 1,
            LP2       = 2,
            LP3       = 3,
            LP4       = 4,
            MCLPStage = 9
        }

        public const int Aligner = 6;

        public enum Stage
        {
            LP1       = 1,
            LP2       = 2,
            LP3       = 3,
            LP4       = 4,
            Tilt      = 5,
            Aligner   = 6,
            TiltPort2 = 7,
            TiltPort3 = 8,
            TiltPort4 = 9
        }

        public enum Unit
        {
            Robot   = 0,
            LP1     = 1,
            LP2     = 2,
            LP3     = 3,
            LP4     = 4,
            Aligner = 5
        }

        public enum AllUnit
        {
            Robot        = 0,
            LP1          = 1,
            LP2          = 2,
            LP3          = 3,
            LP4          = 4,
            Aligner      = 5,
            SafetyCover  = 6,
            VacuumSensor = 7,
            AirSensor    = 8
        }

        public enum EventResult
        {
            Success = 1,
            Error   = 2
        }

        public static IReadOnlyDictionary<ErrorCode, Error> Errors;

        #region Initializer

        static Constants()
        {
            var errors = new Dictionary<ErrorCode, Error>
            {
                {
                    ErrorCode.Normal, new Error
                    {
                        Type        = ((int)ErrorType.NoError).ToString(),
                        Code        = ((int)ErrorCode.Normal).ToString("X4"),
                        Description = "Normal",
                        Cause       = "System is normal status"
                    }
                },

                {
                    ErrorCode.ArmStateError, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.ArmStateError).ToString("X4"),
                        Description = "Arm State Error",
                        Cause       = "Robot arm status is abnormal",
                        Handling    =  "Please check arm status"
                    }
                },

                {
                    ErrorCode.RobotError, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RobotError).ToString("X4"),
                        Description = "Robot Error",
                        Cause       = "Robot abnormal",
                        Handling    =  "Need to initial Robot"
                    }
                },

                {
                    ErrorCode.RobotUpperArmHaveWafer, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RobotUpperArmHaveWafer).ToString("X4"),
                        Description = "Robot Upper Arm Have Wafer",
                        Cause       = "No Use"
                    }
                },

                {
                    ErrorCode.RobotLowerArmHaveWafer, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RobotLowerArmHaveWafer).ToString("X4"),
                        Description = "Robot Lower Arm Have Wafer",
                        Cause       = "No Use"
                    }
                },

                {
                    ErrorCode.RobotUpperArmNotHaveWafer, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RobotUpperArmNotHaveWafer).ToString("X4"),
                        Description = "Robot Upper Arm Not Have Wafer",
                        Cause       = "No Use"
                    }
                },

                {
                    ErrorCode.RobotLowerArmNotHaveWafer, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RobotLowerArmNotHaveWafer).ToString("X4"),
                        Description = "Robot Lower Arm Not Have Wafer Error",
                        Cause       = "No Use"
                    }
                },

                {
                    ErrorCode.RobotMoving, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RobotMoving).ToString("X4"),
                        Description = "Robot Moving",
                        Cause       = "Robot active",
                        Handling    = "Please resend command and check robot status"
                    }
                },

                {
                    ErrorCode.ArmNotAtHomePos, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.ArmNotAtHomePos).ToString("X4"),
                        Description = "Arm Not At Home Pos",
                        Cause       = "Check robot arm status for INIT command",
                        Handling    = "Need to initial by Robot"
                    }
                },

                {
                    ErrorCode.RobotLoadWaferRetryTimes, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RobotLoadWaferRetryTimes).ToString("X4"),
                        Description = "Robot Load Wafer Retry Times",
                        Cause       = "Event",
                        Handling    = "Warm Data For Robot Retry Times"
                    }
                },

                {
                    ErrorCode.AlignerSizeChangeError, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AlignerSizeChangeError).ToString("X4"),
                        Description = "Aligner Size Change Error",
                        Cause       = "Check Aligner Status",
                        Handling    = "Need to initial by Aligner"
                    }
                },

                {
                    ErrorCode.OcrCylinderPositionErrorFor12Inch, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.OcrCylinderPositionErrorFor12Inch).ToString("X4"),
                        Description = "OCR Cylinder Position Error For 12 inch",
                        Cause       = "Check OCR Cylinder Status"
                    }
                },

                {
                    ErrorCode.OcrCylinderPositionErrorFor8Inch, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.OcrCylinderPositionErrorFor8Inch).ToString("X4"),
                        Description = "OCR Cylinder Position Error For 8 inch",
                        Cause       = "Check OCR Cylinder Status"
                    }
                },

                {
                    ErrorCode.SystemWaferSizeAbnormal, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.SystemWaferSizeAbnormal).ToString("X4"),
                        Description = "System Wafer Size Abnormal",
                        Cause       = "Check System Wafer Size Setting"
                    }
                },

                {
                    ErrorCode.LoadPortDisable, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.LoadPortDisable).ToString("X4"),
                        Description = "LoadPort Disable",
                        Cause       = "Port status is disable",
                        Handling    = "Please check command parameter"
                    }
                },
                
                {
                    ErrorCode.CarrierNotLoad, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.CarrierNotLoad).ToString("X4"),
                        Description = "Carrier Not Load",
                        Cause       = "Carrier is not Loaded",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.CarrierLoaded, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.CarrierLoaded).ToString("X4"),
                        Description = "Carrier Loaded",
                        Cause       = "Carrier is Loaded",
                        Handling    = "Please check port status"
                    }
                },

                {
                    ErrorCode.CarrierNotPresent, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.CarrierNotPresent).ToString("X4"),
                        Description = "Carrier Not Present",
                        Cause       = "No Carrier on port",
                        Handling    = "Please check port status"
                    }
                },

                {
                    ErrorCode.LoadPortDoorClose, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.LoadPortDoorClose).ToString("X4"),
                        Description = "LoadPort Door Close",
                        Cause       = "Port door status is close",
                        Handling    = "Please check port status"
                    }
                },

                {
                    ErrorCode.RfidReadFailed, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.RfidReadFailed).ToString("X4"),
                        Description = "RFID Read Failed",
                        Cause       = "Port RFID read fail",
                        Handling    = "Please check RFID parameter"
                    }
                },

                {
                    ErrorCode.LoadPortError, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.LoadPortError).ToString("X4"),
                        Description = "LoadPort Error",
                        Cause       = "LoadPort abnormal",
                        Handling    = "Need to initial LoadPort"
                    }
                },

                {
                    ErrorCode.FoupCassetteSlotNotHaveWafer, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FoupCassetteSlotNotHaveWafer).ToString("X4"),
                        Description = "Foup / Cassette Slot Not Have Wafer",
                        Cause       = "Foup / Cassette slot not has wafer",
                        Handling    = "Please check command parameter or slot status"
                    }
                },

                {
                    ErrorCode.FoupCassetteSlotHaveWafer, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FoupCassetteSlotHaveWafer).ToString("X4"),
                        Description = "Foup / Cassette Slot Have Wafer",
                        Cause       = "Foup / Cassette Slot Has Wafer",
                        Handling    = "Please check command parameter or slot status"
                    }
                },

                {
                    ErrorCode.LoadPortMoving, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.LoadPortMoving).ToString("X4"),
                        Description = "LoadPort Moving",
                        Cause       = "Port Active",
                        Handling    = "Please resend command and check port status"
                    }
                },

                {
                    ErrorCode.LoadPortCarrierTypeUndefined, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.LoadPortCarrierTypeUndefined).ToString("X4"),
                        Description = "LoadPort carrier type undefined",
                        Cause       = "Carrier Type Undefined",
                        Handling    = "Please send SetCarrierType command"
                    }
                },

                {
                    ErrorCode.E84TpTimeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84TpTimeout).ToString("X4"),
                        Description = "E84 TP Timeout",
                        Cause       = "E84 TP Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.E84Tp1Timeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84Tp1Timeout).ToString("X4"),
                        Description = "E84 TP1 Timeout",
                        Cause       = "E84 TP1 Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.E84Tp2Timeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84Tp2Timeout).ToString("X4"),
                        Description = "E84 TP2 Timeout",
                        Cause       = "E84 TP2 Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.E84Tp3Timeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84Tp3Timeout).ToString("X4"),
                        Description = "E84 TP3 Timeout",
                        Cause       = "E84 TP3 Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.E84Tp4Timeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84Tp4Timeout).ToString("X4"),
                        Description = "E84 TP4 Timeout",
                        Cause       = "E84 TP4 Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.E84Tp5Timeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84Tp5Timeout).ToString("X4"),
                        Description = "E84 TP5 Timeout",
                        Cause       = "E84 TP5 Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.E84SignalError, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84SignalError).ToString("X4"),
                        Description = "E84 Signal Error",
                        Cause       = "E84 IO Error",
                        Handling    = "Check E84 IO Signal"
                    }
                },

                {
                    ErrorCode.LoadPortE84Connect, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.LoadPortE84Connect).ToString("X4"),
                        Description = "LoadPort E84 Connect",
                        Cause       = "",
                        Handling    = ""
                    }
                },

                {
                    ErrorCode.PleaseSelectRetryOrAbort, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.PleaseSelectRetryOrAbort).ToString("X4"),
                        Description = "Please Select Retry or Abort",
                        Cause       = "E84 Abnormal , need to send E84R(Retry) or E84A(Abort)",
                        Handling    = "Please send E84R or E84A"
                    }
                },

                {
                    ErrorCode.FitcE84StatusError, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84StatusError).ToString("X4"),
                        Description = "FITC E84 Status Error",
                        Cause       = "FITC E84 Status Error",
                        Handling    = "Check FITC Status"
                    }
                },

                {
                    ErrorCode.E84Td0Timeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84Td0Timeout).ToString("X4"),
                        Description = "E84 TD0 Timeout",
                        Cause       = "E84 TD0 Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.E84Cs0Timeout, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.E84Cs0Timeout).ToString("X4"),
                        Description = "E84 CS_0 Timeout",
                        Cause       = "E84 CS_0 Timeout",
                        Handling    = "Check E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError1, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError1).ToString("X4"),
                        Description = "FITC E84 Signal Error_Wait GO ON but CS_0,VALID,TR_REQ,BUSY,COMPT anyone ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError2, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError2).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait CS_0 ON but GO OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError3, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError3).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait CS_0 ON but VALID,TR_REQ,BUSY, COMPT anyone ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError4, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError4).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait VALID ON but GO,CS_0 anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError5, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError5).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait VALID ON but TR_REQ,BUSY,COMPT anyone ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError6, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError6).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait TR_REQ ON but GO,CS_0,VALID anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError7, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError7).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait TR_REQ ON but BUSY,COMPT anyone ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError8, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError8).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait BUSY ON but GO,CS_0,VALID,TR_REQ anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError9, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError9).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait BUSY ON but COMPT ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError10, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError10).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait BUSY OFF,TR_REQ OFF,COMPT ON but GO,CS_0,VALID anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError11, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError11).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait VALID OFF,COMPT OFF,CS_0 OFF but GO OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError12, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError12).ToString("X4"),
                        Description = "FITC E84 Signal Error_ Wait VALID OFF,COMPT OFF,CS_0 OFF but TR_REQ,BUSY anyone ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError13, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError13).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON,LOADING process, wait PS ON but GO,VALID,CS_0,TR_REQ,BUSY anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError14, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError14).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON, LOADING process, wait PS ON but COMPT ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError15, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError15).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON, LOADING process, wait PL ON but GO,VALID,CS_0,TR_REQ,BUSY anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError16, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError16).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON, LOADING process, wait PL ON but COMPT ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError17, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError17).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON, UNLOADING process, wait PL OFF but GO,VALID,CS_0,TR_REQ,BUSY anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError18, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError18).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON, UNLOADING process, wait PL OFF but COMPT ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError19, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError19).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON, UNLOADING process, wait PS OFF but GO,VALID,CS_0,TR_REQ,BUSY anyone OFF",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84SignalError20, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84SignalError20).ToString("X4"),
                        Description = "FITC E84 Signal Error_ BUSY ON, UNLOADING process, wait PS OFF but COMPT ON",
                        Cause       = "FITC E84 Signal Error",
                        Handling    = "Check FITC E84 Signal"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError1, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError1).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait GO ON but Presence Sensor(PS) or Placement Sensor(PL) Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError2, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError2).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait CS_0 ON but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError3, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError3).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait VALID ON but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError4, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError4).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error TA1 period but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError5, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError5).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait TR_REQ ON but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError6, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError6).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ TA2 period but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError7, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError7).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait BUSY ON but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError8, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError8).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ LOADING process,wait PS ON detected PL ON",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError9, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError9).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ LOADING process, wait PL ON, detected PS OFF",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError10, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError10).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ UNLOADING process, wait PL OFF detected PS OFF",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError11, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError11).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ UNLOADING process, wait PS OFF detected PL ON",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError12, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError12).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait BUSY OFF => PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError13, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError13).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ TA3 period => PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError14, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError14).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait TR_REQ OFF but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError15, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError15).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait COMPT ON but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError16, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError16).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait VALID OFF but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError17, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError17).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait COMPT OFF but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError18, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError18).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait CS_0 OFF but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },

                {
                    ErrorCode.FitcE84FoupSensorError19, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FitcE84FoupSensorError19).ToString("X4"),
                        Description = "FITC E84 Foup Sensor Error_ Wait GO OFF but PS or PL Signal Error",
                        Cause       = "FITC E84 Foup Sensor Signal Error",
                        Handling    = "Check FITC LoadPort Status"
                    }
                },
                
                {
                    ErrorCode.AlignerDisable, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AlignerDisable).ToString("X4"),
                        Description = "Aligner Disable",
                        Cause       = "Aligner status is disable",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.AlignerError, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AlignerError).ToString("X4"),
                        Description = "Aligner Error",
                        Cause       = "Aligner abnormal",
                        Handling    = "Need to initial Aligner"
                    }
                },

                {
                    ErrorCode.WaferOnAligner, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.WaferOnAligner).ToString("X4"),
                        Description = "Wafer On Aligner",
                        Cause       = "Aligner has wafer",
                        Handling    = "Please check command parameter or aligner status"
                    }
                },

                {
                    ErrorCode.NoWaferOnAligner, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.NoWaferOnAligner).ToString("X4"),
                        Description = "No Wafer On Aligner",
                        Cause       = "Aligner not has wafer",
                        Handling    = "Please check command parameter or aligner status"
                    }
                },

                {
                    ErrorCode.AlignerMoving, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AlignerMoving).ToString("X4"),
                        Description = "Aligner Moving",
                        Cause       = "Aligner active",
                        Handling    = "Please resend command and check aligner status"
                    }
                },

                {
                    ErrorCode.OcrRecipeNotSet, new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.OcrRecipeNotSet).ToString("X4"),
                        Description = "OCR Recipe Not Set",
                        Cause       = "OCR recipe abnormal",
                        Handling    = "Please check command parameter or OCR recipe data"
                    }
                },

                {
                    ErrorCode.NoThisCommand,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.NoThisCommand).ToString("X4"),
                        Description = "No This Command",
                        Cause       = "Abnormal command",
                        Handling    = "Please check command"
                    }
                },

                {
                    ErrorCode.AbnormalFormatOfCommand,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AbnormalFormatOfCommand).ToString("X4"),
                        Description = "Abnormal Format of Command",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.HaveNoEndCharacter,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.HaveNoEndCharacter).ToString("X4"),
                        Description = "Have No End Character",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.InvalidNumberOfParameters,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.InvalidNumberOfParameters).ToString("X4"),
                        Description = "Too Many/Too Few Parameter",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.AbnormalRangeOfParameter,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AbnormalRangeOfParameter).ToString("X4"),
                        Description = "Abnormal Range of Parameter",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.UnitNotInitial,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.UnitNotInitial).ToString("X4"),
                        Description = "Unit Not Initial",
                        Cause       = "Unit not initial",
                        Handling    = "Please initial"
                    }
                },

                {
                    ErrorCode.Moving,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.Moving).ToString("X4"),
                        Description = "Moving",
                        Cause       = "No Use"
                    }
                },

                {
                    ErrorCode.AbnormalParameterType,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AbnormalParameterType).ToString("X4"),
                        Description = "Abnormal type of parameter",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"

                    }
                },

                {
                    ErrorCode.AbnormalSlotNumber,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AbnormalSlotNumber).ToString("X4"),
                        Description = "Abnormal Number of Slot",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.ReadySignalIsFalse,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.ReadySignalIsFalse).ToString("X4"),
                        Description = "Ready Signal Is False",
                        Cause       = "Abnormal ready signal",
                        Handling    = "Please check ready signal"

                    }
                },

                {
                    ErrorCode.WaferStateError,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.WaferStateError).ToString("X4"),
                        Description = "Wafer State Error",
                        Cause       = "Robot arm status is abnormal",
                        Handling    = "Please check wafer status"

                    }
                },

                {
                    ErrorCode.ErrorOccurredState,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.ErrorOccurredState).ToString("X4"),
                        Description = "Error-Occurred State",
                        Cause       = "EFEM Unit Error (robot or aligner or port)",
                        Handling    = "Need to initial all unit"

                    }
                },

                {
                    ErrorCode.VacuumError,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.VacuumError).ToString("X4"),
                        Description = "Vacuum Error",
                        Cause       = "Abnormal vacuum signal",
                        Handling    = "Please check vacuum status and vacuum signal"

                    }
                },

                {
                    ErrorCode.AirError,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AirError).ToString("X4"),
                        Description = "Air Error",
                        Cause       = "Abnormal air signal",
                        Handling    = "Please check air status and air signal"

                    }
                },

                {
                    ErrorCode.SystemIsNotFinishedInitializing,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.SystemIsNotFinishedInitializing).ToString("X4"),
                        Description = "System Is Not Finished Initializing",
                        Cause       = "Unit Initializing",
                        Handling    = "Please check unit initial status"
                    }
                },

                {
                    ErrorCode.AbnormalParameter,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AbnormalParameter).ToString("X4"),
                        Description = "Abnormal Parameter",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.AbnormalReadOcrSettingValue,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AbnormalReadOcrSettingValue).ToString("X4"),
                        Description = "Abnormal Read OCR Setting Value",
                        Cause       = "Abnormal command parameter",
                        Handling    = "Please check command parameter"
                    }
                },

                {
                    ErrorCode.SafetyInterlockIsFalse,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.SafetyInterlockIsFalse).ToString("X4"),
                        Description = "Safety Interlock Is False",
                        Cause       = "Abnormal safety interlock signal",
                        Handling    = "Please check safety interlock status"
                    }
                },

                {
                    ErrorCode.AoiDataError,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AoiDataError).ToString("X4"),
                        Description = "AOI -1_-1",
                        Cause       = "AOI Data, if not to use then send -1_-1"
                    }
                },

                {
                    ErrorCode.AoiSystemError,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AoiSystemError).ToString("X4"),
                        Description = "AOI System Error",
                        Cause       = "Event",
                        Handling    = "AOI System Error"
                    }
                },

                {
                    ErrorCode.FfuAbnormal,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.FfuAbnormal).ToString("X4"),
                        Description = "FFU Abnormal",
                        Cause       = "Event",
                        Handling    = "FFU Abnormal"
                    }
                },

                {
                    ErrorCode.AreaSensorAbnormal,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.AreaSensorAbnormal).ToString("X4"),
                        Description = "Area Sensor Abnormal",
                        Cause       = "Abnormal area sensor signal",
                        Handling    = "Please check area sensor status"
                    }
                },

                {
                    ErrorCode.InitSettingValueAbnormal,
                    new Error
                    {
                        Type        = ((int)ErrorType.EfemError).ToString(),
                        Code        = ((int)ErrorCode.InitSettingValueAbnormal).ToString("X4"),
                        Description = "Rorze .ini Setting Value Abnormal",
                        Cause       = "Ini Setting Value Abnormal",
                        Handling    = "Please check Rorze.ini status"
                    }
                },

                {
                    ErrorCode.InMaintenanceMode,
                    new Error
                    {
                        Type        = ((int)ErrorType.MaintenanceError).ToString(),
                        Code        = ((int)ErrorCode.InMaintenanceMode).ToString("X4"),
                        Description = "In Maintenance Mode",
                        Cause       = "EFEM in maintenance mode",
                        Handling    = "Please check EFEM status"
                    }
                },
            };

            Errors = new Dictionary<ErrorCode, Error>(errors);
        }

        #endregion Initializer

        #region Helper Methods

        /// <summary>
        /// Converts a <see cref="Unit"/> enumeration into a load port number.
        /// </summary>
        /// <param name="loadPort">Only loadport unit will be accepted.</param>
        /// <returns>
        /// The loadport identifier (starting at 1) corresponding to <paramref name="loadPort"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="loadPort"/> is not a loadport enumeration.</exception>
        /// <remarks>Use of this method should be preferred in order to avoid problems in case the integer value of the enumerate changes.</remarks>
        public static int ToLoadPortId(Unit loadPort)
        {
            if (Unit.LP1 <= loadPort && loadPort <= Unit.LP4)
            {
                return (loadPort - Unit.LP1) + 1;
            }

            throw new ArgumentOutOfRangeException(nameof(loadPort), @"Only load port unit are allowed.");
        }

        /// <summary>
        /// Converts a load port number into a <see cref="Unit"/> enumeration.
        /// </summary>
        /// <param name="loadPort">Load port number. From 1 to N, with N being the maximum number of load ports.</param>
        /// <returns>
        /// The enumeration corresponding to <paramref name="loadPort"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="loadPort"/> cannot be converted into a loadport enumeration.</exception>
        /// <remarks>Use of this method should be preferred in order to avoid problems in case the integer value of the enumerate changes.</remarks>
        public static Unit ToLoadPortUnit(int loadPort)
        {
            if (!Enum.TryParse((Unit.LP1 + loadPort - 1).ToString(), out Unit unit)
                || !(Unit.LP1 <= unit && unit <= Unit.LP4))
            {
                throw new ArgumentOutOfRangeException(nameof(loadPort));
            }

            return unit;
        }

        /// <summary>
        /// Converts a <see cref="Port"/> into a <see cref="Unit"/> enumeration.
        /// </summary>
        /// <remarks>Use of this method should be preferred in order to avoid problems in case the integer value of the enumerate changes.</remarks>
        public static Unit ToLoadPortUnit(Port loadPort)
        {
            if (!Enum.TryParse((Unit.LP1 + (int)loadPort - 1).ToString(), out Unit unit)
                || !(Unit.LP1 <= unit && unit <= Unit.LP4))
            {
                // For now, we do not consider the Port.MCLPStage because we do not know how to consider it.
                throw new ArgumentOutOfRangeException(nameof(loadPort));
            }

            return unit;
        }

        /// <summary>
        /// Converts a load port number into a <see cref="Port"/> enumeration.
        /// </summary>
        /// <param name="loadPort">Load port number. From 1 to N, with N being the maximum number of load ports.</param>
        /// <returns>
        /// The enumeration corresponding to <paramref name="loadPort"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">When <paramref name="loadPort"/> cannot be converted into a loadport enumeration.</exception>
        /// <remarks>Use of this method should be preferred in order to avoid problems in case the integer value of the enumerate changes.</remarks>
        public static Port ToPort(int loadPort)
        {
            if ((!Enum.TryParse((Port.LP1 + loadPort - 1).ToString(), out Port port)
                 || !(Port.LP1 <= port && port <= Port.LP4 || Port.MCLPStage == port)))
                throw new ArgumentOutOfRangeException(nameof(loadPort));

            return port;
        }

        public static Port ToPort(Stage stage)
        {
            switch (stage)
            {
                case Stage.LP1:
                case Stage.LP2:
                case Stage.LP3:
                case Stage.LP4:
                    return (Port)(int)stage;
                case Stage.Tilt:
                    throw new InvalidOperationException(
                        $"Not possible to convert a {Stage.Tilt} into a {nameof(Port)}.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(stage));
            }
        }

        #endregion Helper Methods

        /// <summary>
        /// "Clock" messages belongings
        /// </summary>
        public const byte TimeFormat16Digits = 16;
        public const byte TimeFormat12Digits = 12;
    }
}
