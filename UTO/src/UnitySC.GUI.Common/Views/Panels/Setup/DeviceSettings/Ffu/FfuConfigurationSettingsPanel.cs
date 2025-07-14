using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Ffu.Configuration;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Ffu
{
    public abstract class FfuConfigurationSettingsPanel<T> : DeviceSettingsPanel<T> where T : FfuConfiguration, new()
    {
        #region Constructors

        protected FfuConfigurationSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(FfuSettingsResources)));
        }

        #endregion

        #region Properties

        public double FfuSpeedSetPoint
        {
            get => ModifiedConfig?.FfuSetPoint ?? 0;
            set
            {
                ModifiedConfig.FfuSetPoint = value;
                OnPropertyChanged();
            }
        }

        public double LowSpeedThresholdValue
        {
            get => ModifiedConfig?.LowSpeedThreshold ?? 0;
            set
            {
                ModifiedConfig.LowSpeedThreshold = value;
                OnPropertyChanged();
            }
        }

        public double LowPressureThresholdValue
        {
            get => ModifiedConfig?.LowPressureThreshold ?? 0;
            set
            {
                ModifiedConfig.LowPressureThreshold = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override void LoadEquipmentConfig()
        {
            OnPropertyChanged(nameof(LowSpeedThresholdValue));
            OnPropertyChanged(nameof(LowPressureThresholdValue));
        }

        #endregion
    }
}
