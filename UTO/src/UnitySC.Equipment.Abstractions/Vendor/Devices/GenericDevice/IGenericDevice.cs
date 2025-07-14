using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice
{
    [Device(IsAbstract = true)]
    [Interruption(Kind = InterruptionKind.Abort)]
    public interface IGenericDevice : IUserInformationProvider
    {
        [Command(Documentation = "Initializes the device if needed or if explicitly required.", Category = "Initialization")]
        [Pre(Type = typeof(IsCurrentActivityNull))]
        [Pre(Type = typeof(IsNotBusy))]
        void Initialize(bool mustForceInit);

        [Status]
        OperatingModes State { get; }

        [Status]
        OperatingModes PreviousState { get; }
    }
}
