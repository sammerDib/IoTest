using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Brooks.Devices.LightTower.BrooksLightTower.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LightTower;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.LightTower.BrooksLightTower
{
    public class BrooksLightTowerSettingsPanel : LightTowerConfigurationSettingsPanel<BrooksLightTowerConfiguration>
    {
        #region Fields

        private readonly UnitySC.Equipment.Abstractions.Devices.LightTower.LightTower _lightTower;

        #endregion

        #region Constructors

        static BrooksLightTowerSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(BrooksLightTowerSettingsPanel),typeof(BrooksLightTowerSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(BrooksLightTowerSettingsResource)));
        }

        public BrooksLightTowerSettingsPanel(
            UnitySC.Equipment.Abstractions.Devices.LightTower.LightTower lightTower,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _lightTower = lightTower;
        }

        #endregion

        #region Properties

        public string BrooksLightTowerName
        {
            get => ModifiedConfig?.BrooksLightTowerName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksLightTowerName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override
        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();
            OnPropertyChanged(nameof(BrooksLightTowerName));
        }

        protected override IConfigManager GetDeviceConfigManager() => _lightTower.ConfigManager;

        #endregion
    }
}
