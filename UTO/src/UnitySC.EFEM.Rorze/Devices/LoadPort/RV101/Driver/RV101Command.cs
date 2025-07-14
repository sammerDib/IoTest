using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101
{
    public class RV101Command : DriverCommand
    {
        public static RV101Command Initialization { get; } = new(1, nameof(LoadPortCommands.Initialization));
        public static RV101Command GetStatuses { get; } = new(2, nameof(GetStatuses));
        public static RV101Command Clamp { get; } = new(4, nameof(SecureCarrierCommand));
        public static RV101Command Unclamp { get; } = new(5, nameof(ReleaseCarrierCommand));
        public static RV101Command Close { get; } = new(6, nameof(ReleaseCarrierCommand));
        public static RV101Command Open { get; } = new(7, nameof(SecureCarrierCommand));
        public static RV101Command GetLastMapping { get; } = new(8, nameof(MappingPatternAcquisitionCommand));
        public static RV101Command Map { get; } = new(9, nameof(PerformWaferMappingCommand));
        public static RV101Command ReadCarrierId { get; } = new(10, nameof(ReadCarrierIdCommand));
        public static RV101Command ReleaseCarrier { get; } = new(11, nameof(ReleaseCarrierCommand));
        public static RV101Command SetSignalOutput { get; } = new(12, nameof(SetLightCommand));
        public static RV101Command SetDateAndTime { get; } = new(13, nameof(SetDateAndTimeCommand));
        public static RV101Command InitializeCommunication { get; } = new(14, nameof(InitializeCommunication));
        public static RV101Command Stop { get; } = new(15, nameof(StopCommand));
        public static RV101Command GetSystemDataConfig { get; } = new(16, nameof(GetDataSubCommand));
        public static RV101Command Dock { get; } = new(17, nameof(SecureCarrierCommand));
        public static RV101Command Undock { get; } = new(18, nameof(ReleaseCarrierCommand));
        public static RV101Command SetCarrierType { get; } = new(15, nameof(SetCarrierTypeCommand));
        public static RV101Command GetVersion { get; } = new(16, nameof(VersionAcquisitionCommand));

        public RV101Command(ushort id, string name) : base(id, name)
        {
        }
    }
}
