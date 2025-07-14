using System.Collections.ObjectModel;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.Simulation
{
    public class SimulatorViewModel : Notifier
    {
        public ObservableCollection<SimulatedDeviceViewModel> SimulatedDevices { get; } = new();

        public void Build(Package package)
        {
            SimulatedDevices.Clear();
            SimulatedDevices.AddRange(package.GetDevicesWithSimView().Select(d => new SimulatedDeviceViewModel(d)));
        }
    }
}
