using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.GUI.Common.Vendor.ApplicationServices.Services.DeviceUiManager
{
    public abstract class DeviceUiFactory
    {
        public abstract BusinessPanel CreatePanel(Device device, string deviceConfigRootPath = "");

        public abstract BaseEditor CreateEditor(Device device, string deviceConfigRootPath = "");
    }
}
