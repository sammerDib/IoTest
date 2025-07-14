using Agileo.EquipmentModeling;
using UnitsNet;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201
{
    public partial class RE201
    {
        protected override void InternalSimulateGoToSlot(byte slot, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
            CurrentSlot = slot;
        }
    }
}