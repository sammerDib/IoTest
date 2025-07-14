using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Simulation
{
    public class SimulatedDeviceViewModel : Notifier
    {
        public ISimDevice SimulatedDevice { get; }

        public SimulatedDeviceViewModel(ISimDevice simulatedDevice)
        {
            SimulatedDevice = simulatedDevice;
        }
    }
}
