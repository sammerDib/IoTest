using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LoadPortsSettings;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.LoadPortSettings.RV201
{
    public class RV201SettingsPanel : LoadPortSettingsPanel<RorzeLoadPortConfiguration>
    {
        #region Fields

        private readonly EFEM.Rorze.Devices.LoadPort.RV201.RV201 _loadPort;

        #endregion

        #region Constructor

        static RV201SettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(RV201SettingsPanel), typeof(RV201SettingsPanelView));
        }

        public RV201SettingsPanel(EFEM.Rorze.Devices.LoadPort.RV201.RV201 loadPort, string relativeId, IIcon icon = null)
            : base(loadPort, relativeId, icon)
        {
            _loadPort = loadPort;
        }

        #endregion

        #region Properties

        public bool UseDefaultPageIntervalForReading
        {
            get => ModifiedConfig?.UseDefaultPageIntervalForReading ?? false;
            set
            {
                ModifiedConfig.UseDefaultPageIntervalForReading = value;
                OnPropertyChanged();
            }
        }

        public uint CarrierIdStartPage
        {
            get => ModifiedConfig?.CarrierIdStartPage ?? 0;
            set
            {
                ModifiedConfig.CarrierIdStartPage = value;
                OnPropertyChanged();
            }
        }

        public uint CarrierIdStopPage
        {
            get => ModifiedConfig?.CarrierIdStopPage ?? 0;
            set
            {
                ModifiedConfig.CarrierIdStopPage = value;
                OnPropertyChanged();
            }
        }

        #region CommunicationConfiguration

        public CommunicationConfigurationSettingsEditor CommunicationConfig { get; private set; }

        #endregion

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
            => _loadPort.ConfigManager;

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            OnPropertyChanged(nameof(UseDefaultPageIntervalForReading));
            OnPropertyChanged(nameof(CarrierIdStartPage));
            OnPropertyChanged(nameof(CarrierIdStopPage));

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return base.ConfigurationEqualsCurrent()
                   && ObjectAreEquals(
                       ModifiedConfig.CarrierIdentificationConfig.CarrierIdStartIndex,
                       CurrentConfig.CarrierIdentificationConfig.CarrierIdStartIndex)
                   && ObjectAreEquals(
                       ModifiedConfig.CarrierIdentificationConfig.CarrierIdStopIndex,
                       CurrentConfig.CarrierIdentificationConfig.CarrierIdStopIndex)
                   && ObjectAreEquals(
                       ModifiedConfig.CarrierIdStartPage,
                       CurrentConfig.CarrierIdStartPage)
                   && ObjectAreEquals(
                       ModifiedConfig.CarrierIdStopPage,
                       CurrentConfig.CarrierIdStopPage)
                   && ObjectAreEquals(
                       ModifiedConfig.UseDefaultPageIntervalForReading,
                       CurrentConfig.UseDefaultPageIntervalForReading);
        }

        protected override bool ConfigurationEqualsLoaded()
            => ObjectAreEquals(ModifiedConfig.CommunicationConfig, LoadedConfig.CommunicationConfig);

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute() && !CommunicationConfig.HasErrors;
        }
        #endregion
    }
}
