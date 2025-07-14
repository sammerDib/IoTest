using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Controller.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.Equipment.GUI.Views.Panels.Setup.DeviceSettings.Controller
{
    public class ControllerSettingsPanel : ControllerConfigurationSettingsPanel<ControllerConfiguration>
    {
        #region Fields

        private readonly UnitySC.Equipment.Abstractions.Devices.Controller.Controller _controller;

        #endregion

        static ControllerSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(ControllerSettingsPanel), typeof(ControllerSettingsPanelView));
        }

        public ControllerSettingsPanel(UnitySC.Equipment.Abstractions.Devices.Controller.Controller controller, string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            _controller = controller;
        }

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _controller.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {

        }
    }
}
