using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver
{
    public class RE201Command : DriverCommand
    {
        public static RE201Command Initialization { get; } = new(1, nameof(LoadPortCommands.Initialization));
        public static RE201Command GetStatuses { get; } = new(2, nameof(GetStatuses));
        public static RE201Command GoToSlot { get; } = new(3, nameof(HomeCommand));
        public static RE201Command Clamp { get; } = new(4, nameof(SecureCarrierCommand));
        public static RE201Command Unclamp { get; } = new(5, nameof(ReleaseCarrierCommand));
        public static RE201Command Close { get; } = new(6, nameof(ReleaseCarrierCommand));
        public static RE201Command Open { get; } = new(7, nameof(SecureCarrierCommand));
        public static RE201Command GetLastMapping { get; } = new(8, nameof(MappingPatternAcquisitionCommand));
        public static RE201Command Map { get; } = new(9, nameof(PerformWaferMappingCommand));
        public static RE201Command ReadCarrierId { get; } = new(10, nameof(ReadCarrierIdCommand));
        public static RE201Command ReleaseCarrier { get; } = new(11, nameof(ReleaseCarrierCommand));
        public static RE201Command SetSignalOutput { get; } = new(12, nameof(SetLightCommand));
        public static RE201Command SetDateAndTime { get; } = new(13, nameof(SetDateAndTimeCommand));
        public static RE201Command InitializeCommunication { get; } = new(14, nameof(InitializeCommunication));
        public static RE201Command SetCarrierType { get; } = new(15, nameof(SetCarrierTypeCommand));
        public static RE201Command GetSystemDataConfig { get; } = new (16, nameof(GetDataSubCommand));
        public static RE201Command SetSystemDataConfig { get; } = new(17, nameof(SetDataSubCommand));
        public static RE201Command GetVersion { get; } = new(18, nameof(VersionAcquisitionCommand));

        public RE201Command(ushort id, string name) : base(id, name)
        {
        }
    }
}
