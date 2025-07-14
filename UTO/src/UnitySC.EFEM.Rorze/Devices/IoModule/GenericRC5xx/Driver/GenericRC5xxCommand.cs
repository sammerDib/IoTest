using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver
{
    public class GenericRC5xxCommand : DriverCommand
    {
        public static GenericRC5xxCommand Initialization { get; } = new(1, nameof(Initialization));

        public static GenericRC5xxCommand SetOutputSignal { get; } =
            new(2, nameof(ChangeOutputSignalCommand));

        public static GenericRC5xxCommand SetDateAndTime { get; } =
            new(3, nameof(SetDateAndTimeCommand));

        public static GenericRC5xxCommand GetStatuses { get; } = new(4, nameof(GetStatuses));

        public GenericRC5xxCommand(ushort id, string name)
            : base(id, name)
        {
        }
    }
}
