using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Ffu
{
    public class FfuSettingsPanel : FfuConfigurationSettingsPanel<FfuConfiguration>
    {
        #region Fields

        private readonly UnitySC.Equipment.Abstractions.Devices.Ffu.Ffu _ffu;

        #endregion

        #region Constructors

        static FfuSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(FfuSettingsPanel), typeof(FfuSettingsPanelView));
        }

        public FfuSettingsPanel(
            UnitySC.Equipment.Abstractions.Devices.Ffu.Ffu ffu,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _ffu = ffu;
        }

        #endregion

        #region override

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _ffu.ConfigManager;
        }

        #endregion
    }
}
