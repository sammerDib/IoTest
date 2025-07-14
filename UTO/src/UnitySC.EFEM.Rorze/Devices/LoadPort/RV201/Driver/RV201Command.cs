using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver
{
    public class RV201Command : DriverCommand
    {
        public static RV201Command Initialization { get; } = new(1, nameof(LoadPortCommands.Initialization));
        public static RV201Command GetStatuses { get; } = new(2, nameof(GetStatuses));
        public static RV201Command Clamp { get; } = new(4, nameof(SecureCarrierCommand));
        public static RV201Command Unclamp { get; } = new(5, nameof(ReleaseCarrierCommand));
        public static RV201Command Close { get; } = new(6, nameof(ReleaseCarrierCommand));
        public static RV201Command Open { get; } = new(7, nameof(SecureCarrierCommand));
        public static RV201Command GetLastMapping { get; } = new(8, nameof(MappingPatternAcquisitionCommand));
        public static RV201Command Map { get; } = new(9, nameof(PerformWaferMappingCommand));
        public static RV201Command ReadCarrierId { get; } = new(10, nameof(ReadCarrierIdCommand));
        public static RV201Command ReleaseCarrier { get; } = new(11, nameof(ReleaseCarrierCommand));
        public static RV201Command SetSignalOutput { get; } = new(12, nameof(SetLightCommand));
        public static RV201Command SetDateAndTime { get; } = new(13, nameof(SetDateAndTimeCommand));
        public static RV201Command InitializeCommunication { get; } = new(14, nameof(InitializeCommunication));
        public static RV201Command Stop { get; } = new(15, nameof(StopCommand));
        public static RV201Command GetSystemDataConfig { get; } = new(16, nameof(GetDataSubCommand));
        public static RV201Command Dock { get; } = new(17, nameof(SecureCarrierCommand));
        public static RV201Command Undock { get; } = new(18, nameof(ReleaseCarrierCommand));
        public static RV201Command SetE84Parameters { get; } = new(19, nameof(SetDataSubCommand));
        public static RV201Command E84Load { get; } = new(20, nameof(E84LoadCommand));
        public static RV201Command E84Unload { get; } = new(21, nameof(E84UnloadCommand));
        public static RV201Command SetSystemDataConfig { get; } = new(22, nameof(SetDataSubCommand));
        public static RV201Command SetCarrierType { get; } = new(23, nameof(SetCarrierTypeCommand));
        public static RV201Command GetVersion { get; } = new(24, nameof(VersionAcquisitionCommand));

        public RV201Command(ushort id, string name) : base(id, name)
        {
        }
    }
}
