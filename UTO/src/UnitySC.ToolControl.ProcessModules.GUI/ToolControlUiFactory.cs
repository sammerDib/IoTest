using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.Thor;
using UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule;
using UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.Wotan;
using UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.EquipmentHandling.ProcessModule.ToolControlProcessModule;
using UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Thor;
using UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Wotan;
using UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.Setup.DeviceSettings.ProcessModule.ToolControlProcessModule;

namespace UnitySC.ToolControl.ProcessModules.GUI
{
    public class ToolControlUiFactory : UnityDeviceUiFactory
    {
        public override BusinessPanel CreatePanel(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                ToolControlProcessModule toolControlProcessModule => new
                    ToolControlProcessModuleSettingsPanel(
                        toolControlProcessModule,
                        toolControlProcessModule.Name,
                        PathIcon.ProcessModule),
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
                ToolControlProcessModule toolControlProcessModule =>
                    new ToolControlProcessModuleCardViewModel(toolControlProcessModule),
                _ => (UnityDeviceCardViewModel)null
            };
        }

        public override UnityDeviceCardViewModel GetProductionCardViewModel(Device device)
        {
            return device switch
            {
                Wotan wotan => new ProductionWotanViewModel(wotan),
                Thor thor => new ProductionThorViewModel(thor),
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
