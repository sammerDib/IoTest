using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Efem;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Efem.BrooksEfem
{
    public class BrooksEfemSettingsPanel : EfemConfigurationSettingsPanel<BrooksEfemConfiguration>
    {
        #region Fields

        private readonly Devices.Efem.BrooksEfem.BrooksEfem _efem;

        #endregion

        #region Properties

        public string BrooksClientName
        {
            get => ModifiedConfig?.BrooksClientName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksClientName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksEfemName
        {
            get => ModifiedConfig?.BrooksEfemName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksEfemName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksDioName
        {
            get => ModifiedConfig?.BrooksDioName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksDioName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksLoadPort1LocationName
        {
            get => ModifiedConfig?.BrooksLoadPort1LocationName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksLoadPort1LocationName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksLoadPort2LocationName
        {
            get => ModifiedConfig?.BrooksLoadPort2LocationName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksLoadPort2LocationName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksLoadPort3LocationName
        {
            get => ModifiedConfig?.BrooksLoadPort3LocationName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksLoadPort3LocationName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksLoadPort4LocationName
        {
            get => ModifiedConfig?.BrooksLoadPort4LocationName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksLoadPort4LocationName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksProcessModuleALocationName
        {
            get => ModifiedConfig?.BrooksProcessModuleALocationName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksProcessModuleALocationName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksProcessModuleBLocationName
        {
            get => ModifiedConfig?.BrooksProcessModuleBLocationName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksProcessModuleBLocationName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksProcessModuleCLocationName
        {
            get => ModifiedConfig?.BrooksProcessModuleCLocationName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksProcessModuleCLocationName = value;
                OnPropertyChanged();
            }
        }

        public string AirNodeSignal
        {
            get => ModifiedConfig?.AirNodeSignal ?? string.Empty;
            set
            {
                ModifiedConfig.AirNodeSignal = value;
                OnPropertyChanged();
            }
        }

        public string PressureNodeSignal
        {
            get => ModifiedConfig?.PressureNodeSignal ?? string.Empty;
            set
            {
                ModifiedConfig.PressureNodeSignal = value;
                OnPropertyChanged();
            }
        }

        public string DoorSensor1NodeSignal
        {
            get => ModifiedConfig?.DoorSensor1NodeSignal ?? string.Empty;
            set
            {
                ModifiedConfig.DoorSensor1NodeSignal = value;
                OnPropertyChanged();
            }
        }

        public string DoorSensor2NodeSignal
        {
            get => ModifiedConfig?.DoorSensor2NodeSignal ?? string.Empty;
            set
            {
                ModifiedConfig.DoorSensor2NodeSignal = value;
                OnPropertyChanged();
            }
        }

        public string InterlockSensor1NodeSignal
        {
            get => ModifiedConfig?.InterlockSensor1NodeSignal ?? string.Empty;
            set
            {
                ModifiedConfig.InterlockSensor1NodeSignal = value;
                OnPropertyChanged();
            }
        }

        public string InterlockSensor2NodeSignal
        {
            get => ModifiedConfig?.InterlockSensor2NodeSignal ?? string.Empty;
            set
            {
                ModifiedConfig.InterlockSensor2NodeSignal = value;
                OnPropertyChanged();
            }
        }
        #endregion

        static BrooksEfemSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(BrooksEfemSettingsPanel), typeof(BrooksEfemSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(BrooksEfemSettingsResources)));
        }

        public BrooksEfemSettingsPanel(Devices.Efem.BrooksEfem.BrooksEfem efem, string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            _efem = efem;
        }

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _efem.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();
            OnPropertyChanged(nameof(BrooksClientName));
            OnPropertyChanged(nameof(BrooksEfemName));
            OnPropertyChanged(nameof(BrooksDioName));
            OnPropertyChanged(nameof(BrooksLoadPort1LocationName));
            OnPropertyChanged(nameof(BrooksLoadPort2LocationName));
            OnPropertyChanged(nameof(BrooksLoadPort3LocationName));
            OnPropertyChanged(nameof(BrooksLoadPort4LocationName));
            OnPropertyChanged(nameof(BrooksProcessModuleALocationName));
            OnPropertyChanged(nameof(BrooksProcessModuleBLocationName));
            OnPropertyChanged(nameof(BrooksProcessModuleCLocationName));
            OnPropertyChanged(nameof(AirNodeSignal));
            OnPropertyChanged(nameof(PressureNodeSignal));
            OnPropertyChanged(nameof(DoorSensor1NodeSignal));
            OnPropertyChanged(nameof(DoorSensor2NodeSignal));
            OnPropertyChanged(nameof(InterlockSensor1NodeSignal));
            OnPropertyChanged(nameof(InterlockSensor2NodeSignal));
        }
    }
}
