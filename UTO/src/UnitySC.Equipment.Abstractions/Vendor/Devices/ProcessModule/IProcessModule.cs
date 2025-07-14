using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device(IsAbstract = true)]
    [Interruption(Kind = InterruptionKind.Abort)]
    public interface IProcessModule : ICommunicatingDevice
    {
        #region IExtendedMaterialLocationContainer Commands & Statuses

        [Command(Documentation = "Prepare the device for a transfer")]
        [Pre(Type = typeof(CheckDriverConnected))]
        [Pre(Type = typeof(IsMaintenanceOrIdle))]
        void PrepareForTransfer(byte slot, TransferType transferType);

        [Command(Documentation = "Prepare the device for process")]
        [Pre(Type = typeof(CheckDriverConnected))]
        [Pre(Type = typeof(IsMaintenanceOrIdle))]
        void PrepareForProcess(byte slot, bool automaticStart = false);

        [Status]
        bool IsReadyForTransfer { get; }

        [Status]
        bool IsReadyToAcceptTransfer { get; }

        #endregion
    }
}
