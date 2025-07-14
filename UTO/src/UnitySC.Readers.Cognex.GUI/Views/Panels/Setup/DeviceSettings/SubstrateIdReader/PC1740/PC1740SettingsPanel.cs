using System;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.SubstrateIdReader;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Configuration;

namespace UnitySC.Readers.Cognex.GUI.Views.Panels.Setup.DeviceSettings.SubstrateIdReader.PC1740
{
    public class PC1740SettingsPanel : SubstrateIdReaderSettingsPanel<PC1740Configuration>
    {
        #region Fields

        private readonly Readers.Cognex.Devices.SubstrateIdReader.PC1740.PC1740 _substrateIdReader;

        #endregion

        #region Constructor

        static PC1740SettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(PC1740SettingsPanel), typeof(PC1740SettingsPanelView));
        }

        public PC1740SettingsPanel(
            Readers.Cognex.Devices.SubstrateIdReader.PC1740.PC1740 substrateIdReader,
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

        public string RecipeFolderPath
        {
            get => ModifiedConfig?.RecipeFolderPath ?? string.Empty;
            set
            {
                ModifiedConfig.RecipeFolderPath = value;
                OnPropertyChanged();
            }
        }

        public bool UseOnlyOneT7
        {
            get => ModifiedConfig?.UseOnlyOneT7 ?? false;
            set
            {
                ModifiedConfig.UseOnlyOneT7 = value;
                OnPropertyChanged();
            }
        }

        public double Angle
        {
            get => ModifiedConfig?.T7Recipe?.Angle ?? 0;
            set
            {
                ModifiedConfig.T7Recipe ??=
                    new T7RecipeConfiguration();
                ModifiedConfig.T7Recipe.Angle = value;
                OnPropertyChanged();
            }
        }

        public double Angle8
        {
            get => ModifiedConfig?.T7Recipe?.Angle8 ?? 0;
            set
            {
                ModifiedConfig.T7Recipe ??=
                    new T7RecipeConfiguration();
                ModifiedConfig.T7Recipe.Angle8 = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => ModifiedConfig?.T7Recipe?.Date ?? DateTime.Now;
            set
            {
                ModifiedConfig.T7Recipe ??=
                    new T7RecipeConfiguration();
                ModifiedConfig.T7Recipe.Date = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => ModifiedConfig?.T7Recipe?.Name ?? string.Empty;
            set
            {
                ModifiedConfig.T7Recipe ??=
                    new T7RecipeConfiguration();
                ModifiedConfig.T7Recipe.Name = value;
                OnPropertyChanged();
            }
        }

        #region CommunicationConfiguration

        public CommunicationConfigurationSettingsEditor CommunicationConfig { get; private set; }

        #endregion

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _substrateIdReader.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            OnPropertyChanged(nameof(RecipeFolderPath));
            OnPropertyChanged(nameof(UseOnlyOneT7));
            OnPropertyChanged(nameof(Angle));
            OnPropertyChanged(nameof(Angle8));
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Name));

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
        }

        protected override bool ConfigurationEqualsLoaded()
        {
            return ObjectAreEquals(
                ModifiedConfig.CommunicationConfig,
                LoadedConfig.CommunicationConfig);
        }

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute() && !CommunicationConfig.HasErrors;
        }

        #endregion
    }
}
