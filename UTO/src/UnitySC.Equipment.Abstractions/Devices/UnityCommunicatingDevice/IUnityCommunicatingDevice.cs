using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

namespace UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice
{
    [Device(IsAbstract = true)]
    public interface IUnityCommunicatingDevice : IGenericDevice
    {
        [Command(Category = "Communication")]
        [Pre(Type = typeof(IsNotStarted))]
        void StartCommunication();

        [Command(Category = "Communication")]
        void StopCommunication();

        [Status]
        bool IsCommunicating { get; }

        [Status]
        bool IsCommunicationStarted { get; }
    }
}
