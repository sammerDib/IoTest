using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver
{
    public class Dio0Command : DriverCommand
    {
        public static Dio0Command StopFanRotation { get; } = new(1, nameof(StopFanRotationCommand));
        public static Dio0Command StartFanRotation { get; } = new(2, nameof(StartFanRotationCommand));

        public Dio0Command(ushort id, string name) : base(id, name)
        {
        }
    }
}
