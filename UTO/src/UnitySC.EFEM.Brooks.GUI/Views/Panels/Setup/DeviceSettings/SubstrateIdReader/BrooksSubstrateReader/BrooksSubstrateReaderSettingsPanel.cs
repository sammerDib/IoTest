using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Brooks.Devices.SubstrateIdReader.BrooksSubstrateIdReader;
using UnitySC.EFEM.Brooks.Devices.SubstrateIdReader.BrooksSubstrateIdReader.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.SubstrateIdReader;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.SubstrateIdReader.BrooksSubstrateReader
{
    public class BrooksSubstrateReaderSettingsPanel : SubstrateIdReaderSettingsPanel<BrooksSubstrateIdReaderConfiguration>
    {
        #region Fields

        private readonly BrooksSubstrateIdReader _substrateIdReader;

        #endregion

        #region Constructor

        static BrooksSubstrateReaderSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(BrooksSubstrateReaderSettingsPanel), typeof(BrooksSubstrateReaderSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(BrooksSubstrateReaderSettingsResources)));
        }

        public BrooksSubstrateReaderSettingsPanel(
            BrooksSubstrateIdReader substrateIdReader,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _substrateIdReader = substrateIdReader;

            Rules.Add(
                new DelegateRule(
                    nameof(ImagePath),
                    () => string.IsNullOrEmpty(ImagePath)
                        ? LocalizationManager.GetString(
                            nameof(SubstrateIdReaderSettingsResources
                                .S_SETUP_SUBSTRATE_ID_READER_SETTINGS_RECIPE_FOLDER_PATH_INVALID))
                        : string.Empty));
        }

        #endregion

        #region Properties

        public string BrooksReaderName
        {
            get => ModifiedConfig?.BrooksReaderName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksReaderName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _substrateIdReader.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            OnPropertyChanged(nameof(BrooksReaderName));
        }

        #endregion
    }
}
