using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201
{
    public static class Constants
    {
        public static IReadOnlyDictionary<string, DevicePart> DeviceParts;

        static Constants()
        {
            var motionAxisSubCommands = new List<string>(12)
            {
                RorzeConstants.SubCommands.OriginSearch,
                RorzeConstants.SubCommands.Home,
                RorzeConstants.SubCommands.TeachingOperation,
                RorzeConstants.SubCommands.AbsolutePositionMovement,
                RorzeConstants.SubCommands.PositionAcquisition,
                RorzeConstants.SubCommands.DirectCommand,
                RorzeConstants.SubCommands.VersionAcquisition,
                RorzeConstants.SubCommands.DataTransfer,
                RorzeConstants.SubCommands.DataAcquisition,
                RorzeConstants.SubCommands.DataSetting,
                RorzeConstants.SubCommands.ParameterAcquisition,
                RorzeConstants.SubCommands.ParameterSetting
            };

            var mapperSubCommands = new List<string>
            {
                RorzeConstants.SubCommands.AbsolutePositionMovement,
                RorzeConstants.SubCommands.PositionAcquisition,
                RorzeConstants.SubCommands.DirectCommand,
                RorzeConstants.SubCommands.DataTransfer,
                RorzeConstants.SubCommands.DataAcquisition,
                RorzeConstants.SubCommands.DataSetting,
                RorzeConstants.SubCommands.ParameterAcquisition,
                RorzeConstants.SubCommands.ParameterSetting
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
                { DevicePartIds.ZAxis, new DevicePart(DevicePartIds.ZAxis, motionAxisSubCommands) },
                { DevicePartIds.Rotation, new DevicePart(DevicePartIds.Rotation, motionAxisSubCommands) },
                { DevicePartIds.Mapper, new DevicePart(DevicePartIds.Mapper, mapperSubCommands) },
                {
                    DevicePartIds.StageData, new DevicePart(
                        DevicePartIds.StageData,
                        dataTableSubCommands.Concat(new[] { RorzeConstants.SubCommands.DataTransfer }))
                },
                {
                    DevicePartIds.SystemData, new DevicePart(
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
                { DevicePartIds.MaintenanceData, new DevicePart(DevicePartIds.MaintenanceData, dataTableSubCommands) },
                { DevicePartIds.MapperData, new DevicePart(DevicePartIds.MapperData, dataTableSubCommands) },
                {
                    DevicePartIds.ZAxisData, new DevicePart(
                        DevicePartIds.ZAxisData,
                        new List<string>
                        {
                            RorzeConstants.SubCommands.DataAcquisition,
                            RorzeConstants.SubCommands.DataSetting
                        })
                },
                {
                    DevicePartIds.CarrierIdentifyingData,
                    new DevicePart(DevicePartIds.CarrierIdentifyingData, dataTableSubCommands)
                },
                {
                    DevicePartIds.DataByCarrierType,
                    new DevicePart(DevicePartIds.DataByCarrierType, dataTableSubCommands)
                },
                { DevicePartIds.Counters, new DevicePart(DevicePartIds.Counters, dataTableSubCommands) },
                { DevicePartIds.ResolutionData, new DevicePart(DevicePartIds.ResolutionData, dataTableSubCommands) },
                {
                    DevicePartIds.CompensationMappingData, new DevicePart(
                        DevicePartIds.CompensationMappingData,
                        new List<string> { RorzeConstants.SubCommands.PositionAcquisition })
                }
            };

            DeviceParts = new ReadOnlyDictionary<string, DevicePart>(deviceParts);
        }

        public struct DevicePartIds
        {
            public const string ZAxis = "ZAX1";

            /// <summary>
            ///     Rotation.
            /// </summary>
            public const string Rotation = "ROT1";

            /// <summary>
            ///     Mapper.
            /// </summary>
            public const string Mapper = "RCA1";

            // Data
            public const string StageData = "DSTG";
            public const string SystemData = "DEQU";
            public const string ZAxisData = "DZAX";
            public const string InitializationData = "DRCI";
            public const string NormalOperationData = "DRCS";
            public const string MaintenanceData = "DMNT";
            public const string MapperData = "DMPR";
            public const string CarrierIdentifyingData = "DCST";
            public const string DataByCarrierType = "DPRM";
            public const string Counters = "DCNT";
            public const string ResolutionData = "DRES";
            public const string RawMappingData = "RCA1";
            public const string CompensationMappingData = "RCA2";
        }
    }
}
