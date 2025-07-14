using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice
{
    public partial class UnityCommunicatingDevice
    {
        protected virtual void InternalSimulateStartCommunication(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromMilliseconds(200));
            IsCommunicationStarted = true;
            IsCommunicating = true;
        }

        protected virtual void InternalSimulateStopCommunication(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromMilliseconds(200));
            IsCommunicating = false;
            IsCommunicationStarted = false;
        }
    }
}
