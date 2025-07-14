using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.LightTower.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LightTower
{
    public class LightTowerSettingsPanel : LightTowerConfigurationSettingsPanel<LightTowerConfiguration>
    {
        #region Fields

        private readonly UnitySC.Equipment.Abstractions.Devices.LightTower.LightTower _lightTower;

        #endregion

        #region Constructors

        static LightTowerSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(LightTowerSettingsPanel),typeof(LightTowerSettingsPanelView));
        }

        public LightTowerSettingsPanel(
            UnitySC.Equipment.Abstractions.Devices.LightTower.LightTower lightTower,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _lightTower = lightTower;
        }

        #endregion


        #region Override

        protected override IConfigManager GetDeviceConfigManager() => _lightTower.ConfigManager;

        #endregion
    }
}
