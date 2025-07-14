
using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Brooks.Devices.Aligner.BrooksAligner.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Aligner;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Aligner.BrooksAligner
{
    public class BrooksAlignerSettingsPanel : AlignerConfigurationSettingsPanel<BrooksAlignerConfiguration>
    {
        #region Fields

        private readonly Devices.Aligner.BrooksAligner.BrooksAligner _aligner;

        #endregion

        #region Constructors
        static BrooksAlignerSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(BrooksAlignerSettingsPanel), typeof(BrooksAlignerSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(BrooksAlignerSettingsResources)));
        }

        public BrooksAlignerSettingsPanel(Devices.Aligner.BrooksAligner.BrooksAligner aligner, string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            _aligner = aligner;
        }

        #endregion

        #region Properties

        public string BrooksAlignerName
        {
            get => ModifiedConfig?.BrooksAlignerName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksAlignerName = value;
                OnPropertyChanged();
            }
        }

        public string BrooksChuckName
        {
            get => ModifiedConfig?.BrooksChuckName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksChuckName = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
            => _aligner.ConfigManager;

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();
            OnPropertyChanged(nameof(BrooksAlignerName));
            OnPropertyChanged(nameof(BrooksChuckName));

        }

        #endregion
    }
}
