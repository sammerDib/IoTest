using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Aligner
{
    public abstract class AlignerConfigurationSettingsPanel<T> : DeviceSettingsPanel<T>
        where T : AlignerConfiguration, new()
    {
        #region Constructors

        static AlignerConfigurationSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(AlignerSettingsResources)));
        }

        protected AlignerConfigurationSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            Rules.Add(
                new DelegateRule(
                    nameof(AlignOffsetValue),
                    () =>
                    {
                        if (AlignOffsetValue is < 0 or >= 360)
                        {
                            return LocalizationManager.GetString(
                                nameof(AlignerSettingsResources.S_SETUP_DEVICE_SETTINGS_ALIGN_OFFSET_INVALID));
                        }

                        return string.Empty;
                    }));
        }

        #endregion

        #region Properties

        public double AlignOffsetValue
        {
            get => ModifiedConfig?.AlignOffset ?? 0;
            set
            {
                ModifiedConfig.AlignOffset = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override void LoadEquipmentConfig()
        {
            OnPropertyChanged(nameof(AlignOffsetValue));
        }

        #endregion
    }
}
