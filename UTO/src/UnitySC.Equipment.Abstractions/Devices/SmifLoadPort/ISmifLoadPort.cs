using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Conditions;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Devices.SmifLoadPort
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device(IsAbstract = true)]
    public interface ISmifLoadPort : ILoadPort
    {
        #region Commands

        [Command]
        [Pre(Type = typeof(IsInService))]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(CheckSlot))]
        void GoToSlot(byte slot);

        #endregion Commands

        #region Statuses

        [Status]
        byte CurrentSlot { get; }

        #endregion
    }
}
