using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201
{
    public static class Constants
    {
        public struct DevicePartIds
        {
            public const string YAxis = "YAX1";
            public const string ZAxis = "ZAX1";

            /// <summary>
            /// Lifting/Lowering mechanism for reading the carrier ID.
            /// (Usable only for "System type=2 and 5" and "HOME")
            /// </summary>
            public const string LiftingMechanism = "BCR1";

            /// <summary>
            /// Carrier retaining mechanism.
            /// (Usable only for "System type=2" and "HOME")
            /// </summary>
            public const string CarrierRetainingMechanism = "PRS1";

            /// <summary>
            /// Stage rotating mechanism.
            /// (Usable only for "System type=4, 6" and "HOME")
            /// </summary>
            public const string RotatingMechanism = "ROT1";

            // Data
            public const string StageData               = "DSTG";
            public const string SystemData              = "DEQU";
            public const string InitializationData      = "DRCI";
            public const string NormalOperationData     = "DRCS";
            public const string MaintenanceData         = "DMNT";
            public const string MapperData              = "DMPR";
            public const string CarrierIdentifyingData  = "DCST";
            public const string DataByCarrierType       = "DPRM";
            public const string Counters                = "DCNT";
            public const string ResolutionData          = "DRES";
            public const string E84Parameter            = "DE84";
            public const string RawMappingData          = "RCA1";
            public const string CompensationMappingData = "RCA2";
        }

        public struct E84Errors
        {
            public const string Tp1Timeout        = "B0";
            public const string Tp2Timeout        = "B1";
            public const string Tp3Timeout_Load   = "B2";
            public const string Tp3Timeout_Unload = "B3";
            public const string Tp4Timeout        = "B4";
            public const string Tp5Timeout        = "B6";
            public const string Tp6Timeout        = "B7";

            public const string E84SignalError_TrReq     = "B8";
            public const string E84SignalError_Busy      = "B9";
            public const string E84SignalError_Placement = "BA";
            public const string E84SignalError_Complete  = "BB";
            public const string E84SignalError_Valid     = "BC";
            public const string E84SignalError_Cs0       = "BF";

            public const string CarrierImproperlyTaken = "9C";
            public const string CarrierImproperlyPlaced = "9E";
        }

        public const int E84RorzeLoadPortMask = 0x4040; // Bit6 (E84) & Bit14 (E84 sequence abnormal detection) => true
        public const int ExternalIoMask = 0x08;

        public static IReadOnlyDictionary<string, DevicePart> DeviceParts;

        static Constants()
        {
            var motionAxisSubCommands = new List<string>(7)
            {
                RorzeConstants.SubCommands.OriginSearch,
                RorzeConstants.SubCommands.Home,
                RorzeConstants.SubCommands.TeachingOperation,
                RorzeConstants.SubCommands.PositionAcquisition,
                RorzeConstants.SubCommands.DirectCommand,
                RorzeConstants.SubCommands.DataAcquisition,
                RorzeConstants.SubCommands.DataSetting
            };

            var dataTableSubCommands = new List<string>(3)
            {
                RorzeConstants.SubCommands.DataAcquisition,
                RorzeConstants.SubCommands.DataSetting,
                RorzeConstants.SubCommands.DataInitialization
            };

            // Define applicable sub-commands by Device Part
            var deviceParts = new Dictionary<string, DevicePart>
            {
                {
                    DevicePartIds.YAxis,
                    new DevicePart(DevicePartIds.YAxis, motionAxisSubCommands)
                },
                {
                    DevicePartIds.ZAxis,
                    new DevicePart(DevicePartIds.ZAxis, motionAxisSubCommands)
                },
                {
                    DevicePartIds.LiftingMechanism,
                    new DevicePart(DevicePartIds.LiftingMechanism, motionAxisSubCommands)
                },
                {
                    DevicePartIds.CarrierRetainingMechanism,
                    new DevicePart(DevicePartIds.CarrierRetainingMechanism, motionAxisSubCommands)
                },
                {
                    DevicePartIds.RotatingMechanism,
                    new DevicePart(DevicePartIds.RotatingMechanism, motionAxisSubCommands)
                },

                {
                    DevicePartIds.StageData,
                    new DevicePart(
                        DevicePartIds.StageData,
                            dataTableSubCommands.Concat(new []
                            {
                                RorzeConstants.SubCommands.DataTransfer
                            }))
                },
                {
                    DevicePartIds.SystemData,
                    new DevicePart(
                        DevicePartIds.SystemData,
                        new List<string>
                        {
                            RorzeConstants.SubCommands.DataAcquisition,
                            RorzeConstants.SubCommands.DataSetting
                        })
                },
                {
                    DevicePartIds.InitializationData,
                    new DevicePart(DevicePartIds.InitializationData, dataTableSubCommands)
                },
                {
                    DevicePartIds.NormalOperationData,
                    new DevicePart(DevicePartIds.NormalOperationData, dataTableSubCommands)
                },
                {
                    DevicePartIds.MaintenanceData,
                    new DevicePart(DevicePartIds.MaintenanceData, dataTableSubCommands)
                },
                {
                    DevicePartIds.MapperData,
                    new DevicePart(DevicePartIds.MapperData, dataTableSubCommands)
                },
                {
                    DevicePartIds.CarrierIdentifyingData,
                    new DevicePart(DevicePartIds.CarrierIdentifyingData, dataTableSubCommands)
                },
                {
                    DevicePartIds.DataByCarrierType,
                    new DevicePart(DevicePartIds.DataByCarrierType, dataTableSubCommands)
                },
                {
                    DevicePartIds.Counters,
                    new DevicePart(DevicePartIds.Counters, dataTableSubCommands)
                },
                {
                    DevicePartIds.ResolutionData,
                    new DevicePart(DevicePartIds.ResolutionData, dataTableSubCommands)
                },
                {
                    DevicePartIds.E84Parameter,
                    new DevicePart(DevicePartIds.E84Parameter, dataTableSubCommands)
                },
                {
                    DevicePartIds.RawMappingData,
                    new DevicePart(
                        DevicePartIds.RawMappingData,
                        new List<string>
                        {
                            RorzeConstants.SubCommands.PositionAcquisition
                        })
                },
                {
                    DevicePartIds.CompensationMappingData,
                    new DevicePart(
                        DevicePartIds.CompensationMappingData,
                        new List<string>
                        {
                            RorzeConstants.SubCommands.PositionAcquisition
                        })
                }
            };

            DeviceParts = new ReadOnlyDictionary<string, DevicePart>(deviceParts);
        }
    }
}
