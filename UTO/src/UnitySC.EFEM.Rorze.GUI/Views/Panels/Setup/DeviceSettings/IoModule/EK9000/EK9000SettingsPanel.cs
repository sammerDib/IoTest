using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication.Modbus;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.EK9000
{
    public class EK9000SettingsPanel : DeviceSettingsPanel<EK9000Configuration>
    {
        #region Fields

        private readonly EFEM.Rorze.Devices.IoModule.EK9000.EK9000 _ioModule;

        #endregion

        #region Constructor

        static EK9000SettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(EK9000SettingsPanel), typeof(EK9000SettingsPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(IoModuleSettingsResources)));
        }

        public EK9000SettingsPanel(
            EFEM.Rorze.Devices.IoModule.EK9000.EK9000 ioModule,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _ioModule = ioModule;
        }

        #endregion

        #region Properties

        public double CoefPressure
        {
            get => ModifiedConfig?.CoefPressure ?? 0;
            set
            {
                ModifiedConfig.CoefPressure = value;
                OnPropertyChanged();
            }
        }

        public double CoefSpeed
        {
            get => ModifiedConfig?.CoefSpeed ?? 0;
            set
            {
                ModifiedConfig.CoefSpeed = value;
                OnPropertyChanged();
            }
        }

        #region ModbusCommunicationConfiguration

        public ModbusConfigurationSettingsEditor ModbusCommunicationConfig { get; private set; }

        #endregion

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
            => _ioModule.ConfigManager;

        protected override void LoadEquipmentConfig()
        {
            OnPropertyChanged(nameof(CoefPressure));
            OnPropertyChanged(nameof(CoefSpeed));

            ModbusCommunicationConfig =
                new ModbusConfigurationSettingsEditor(ModifiedConfig?.ModbusConfiguration);
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            ModbusCommunicationConfig =
                new ModbusConfigurationSettingsEditor(ModifiedConfig?.ModbusConfiguration);
        }

        #endregion
    }
}
