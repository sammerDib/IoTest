using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740;
using UnitySC.Readers.Cognex.GUI.Views.Panels.Setup.DeviceSettings.SubstrateIdReader.PC1740;

namespace UnitySC.Readers.Cognex.GUI
{
    public class CognexUiFactory : UnityDeviceUiFactory
    {
        public override BusinessPanel CreatePanel(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                PC1740 pc1740 => new PC1740SettingsPanel(pc1740, pc1740.Name, PathIcon.ReadTag),
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
