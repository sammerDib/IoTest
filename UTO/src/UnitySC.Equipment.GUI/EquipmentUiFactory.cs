using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.Equipment.Devices.Controller;
using UnitySC.Equipment.GUI.Views.Panels.Setup.DeviceSettings.Controller;
using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.Equipment.GUI
{
    public class EquipmentUiFactory : UnityDeviceUiFactory
    {
        public override BusinessPanel CreatePanel(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                Controller controller => new ControllerSettingsPanel(
                    controller,
                    nameof(ControllerSettingsResources.BP_CONTROLLER_SETTINGS),
                    PathIcon.Setup),
                _ => null
            };
        }

        public override BaseEditor CreateEditor(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                _ => null
            };
        }

        public override UnityDeviceCardViewModel GetEquipmentHandlingCardViewModel(Device device)
        {
            return device switch
            {
                _ => null
            };
        }

        public override UnityDeviceCardViewModel GetProductionCardViewModel(Device device)
        {
            return device switch
            {
                _ => null
            };
        }

        public override MachineModuleViewModel GetModuleViewModel(Device device)
        {
            return device switch
            {
                _ => null
            };
        }
    }
}
