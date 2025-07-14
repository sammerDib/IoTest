using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Brooks.Devices.Ffu.BrooksFfu.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Ffu;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Ffu.BrooksFfu
{
    public class BrooksFfuSettingsPanel : FfuConfigurationSettingsPanel<BrooksFfuConfiguration>
    {
        #region Fields

        private readonly Devices.Ffu.BrooksFfu.BrooksFfu _ffu;

        #endregion

        #region Constructors

        static BrooksFfuSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(BrooksFfuSettingsPanel), typeof(BrooksFfuSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(BrooksFfuSettingsResources)));
        }

        public BrooksFfuSettingsPanel(
            Devices.Ffu.BrooksFfu.BrooksFfu ffu,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _ffu = ffu;
        }

        #endregion

        #region Properties

        public string BrooksFfuName
        {
            get => ModifiedConfig?.BrooksFfuName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksFfuName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();
            OnPropertyChanged(nameof(BrooksFfuName));
        }

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _ffu.ConfigManager;
        }

        #endregion
    }
}
