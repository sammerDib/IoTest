using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice
{
    [Device]
    public interface ICommunicatingDevice : IGenericDevice
    {
        [Command(Category = "Connection")]
        void Connect();

        [Command(Category = "Connection")]
        void Disconnect();

        [Status]
        bool IsConnected { get; }

        [Status(Documentation = "Indicate if device own a driver or use a parent driver")]
        bool IsSlaveDevice { get; }
    }
}
