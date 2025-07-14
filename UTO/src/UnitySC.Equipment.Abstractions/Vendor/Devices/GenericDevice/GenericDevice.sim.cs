using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice
{
    public partial class GenericDevice
    {
        protected virtual void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(5));
            ClearAllAlarms();
        }
    }
}
