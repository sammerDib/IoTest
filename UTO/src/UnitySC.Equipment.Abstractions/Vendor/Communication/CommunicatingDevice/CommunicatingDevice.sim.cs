using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice
{
    public partial class CommunicatingDevice
    {
        protected virtual void InternalSimulateConnect(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromMilliseconds(200));
            IsConnected = true;
        }

        protected virtual void InternalSimulateDisconnect(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromMilliseconds(200));
            IsConnected = false;
        }
    }
}
