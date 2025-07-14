using Agileo.EquipmentModeling;

namespace UnitySC.Equipment.Abstractions.Devices.SmifLoadPort
{
    public partial class SmifLoadPort
    {
        protected abstract void InternalSimulateGoToSlot(byte slot, Tempomat tempomat);
    }
}
