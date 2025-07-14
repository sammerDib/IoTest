using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule
{
    public partial class ProcessModule
    {
        protected abstract void InternalSimulatePrepareForTransfer(
            byte slot,
            TransferType transferType,
            Tempomat tempomat);

        protected abstract void InternalSimulatePrepareForProcess(
            byte slot,
            bool automaticStart,
            Tempomat tempomat);
    }
}
