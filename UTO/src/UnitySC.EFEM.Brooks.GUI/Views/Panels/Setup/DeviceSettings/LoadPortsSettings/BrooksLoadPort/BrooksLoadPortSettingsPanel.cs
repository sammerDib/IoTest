using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LoadPortsSettings;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.LoadPortsSettings.BrooksLoadPort
{
    public class BrooksLoadPortSettingsPanel : LoadPortSettingsPanel<BrooksLoadPortConfiguration>
    {
        #region Fields

        private readonly Devices.LoadPort.BrooksLoadPort.BrooksLoadPort _loadPort;

        #endregion

        #region Constructor

        static BrooksLoadPortSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(BrooksLoadPortSettingsPanel), typeof(BrooksLoadPortSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(BrooksLoadPortSettingsResources)));
        }

        public BrooksLoadPortSettingsPanel(Devices.LoadPort.BrooksLoadPort.BrooksLoadPort loadPort, string relativeId, IIcon icon = null)
            : base(loadPort, relativeId, icon)
        {
            _loadPort = loadPort;
        }

        #endregion

        #region Properties

        public string BrooksLoadPortName
        {
            get => ModifiedConfig?.BrooksLoadPortName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksLoadPortName = value;
                OnPropertyChanged();
            }
        }

        public string LightCurtainNodeSignal
        {
            get => ModifiedConfig?.LightCurtainNodeSignal ?? string.Empty;
            set
            {
                ModifiedConfig.LightCurtainNodeSignal = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager() => _loadPort.ConfigManager;

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            OnPropertyChanged(nameof(BrooksLoadPortName));
            OnPropertyChanged(nameof(LightCurtainNodeSignal));
        }

        #endregion
    }
}
