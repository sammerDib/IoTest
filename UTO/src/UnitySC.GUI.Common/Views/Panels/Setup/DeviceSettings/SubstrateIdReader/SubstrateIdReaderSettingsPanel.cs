using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.SubstrateIdReader
{
    public abstract class SubstrateIdReaderSettingsPanel<T> : DeviceSettingsPanel<T>
        where T : SubstrateIdReaderConfiguration, new()
    {
        #region Constructors

        static SubstrateIdReaderSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SubstrateIdReaderSettingsResources)));
        }

        protected SubstrateIdReaderSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            Rules.Add(
                new DelegateRule(
                    nameof(ImagePath),
                    () => string.IsNullOrEmpty(ImagePath)
                        ? LocalizationManager.GetString(
                            nameof(SubstrateIdReaderSettingsResources
                                .S_SETUP_SUBSTRATE_ID_READER_SETTINGS_IMAGE_PATH_INVALID))
                        : string.Empty));
        }

        #endregion

        #region Properties

        public string ImagePath
        {
            get => ModifiedConfig?.ImagePath ?? string.Empty;
            set
            {
                ModifiedConfig.ImagePath = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override void LoadEquipmentConfig()
        {
            OnPropertyChanged(nameof(ImagePath));
        }

        #endregion
    }
}
