using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.ApplicationServices.Services.DeviceUiManager;

namespace UnitySC.GUI.Common.Equipment
{
    public abstract class UnityDeviceUiFactory : DeviceUiFactory
    {
        public abstract UnityDeviceCardViewModel GetEquipmentHandlingCardViewModel(Device device);
        public abstract UnityDeviceCardViewModel GetProductionCardViewModel(Device device);
        public abstract MachineModuleViewModel GetModuleViewModel(Device device);
    }
}
