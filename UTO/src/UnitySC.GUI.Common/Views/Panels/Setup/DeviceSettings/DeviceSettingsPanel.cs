using System;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings
{
    public abstract class DeviceSettingsPanel<T> : SetupPanel<T>
        where T : DeviceConfiguration, new()
    {
        #region Constructors

        static DeviceSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DeviceSettingsResources)));
        }

        protected DeviceSettingsPanel()
            : base("DesignTime constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        protected DeviceSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            Rules.Add(
                new DelegateRule(
                    nameof(InitializationTimeout),
                    () =>
                    {
                        if (InitializationTimeout is < 0)
                        {
                            return LocalizationManager.GetString(
                                nameof(DeviceSettingsResources.S_SETUP_DEVICE_SETTINGS_INIT_TIMEOUT_INVALID));
                        }

                        return string.Empty;
                    }));
        }

        #endregion

        #region Properties

        private bool _errorVisibility;

        public bool SettingsVisibility => !_errorVisibility;

        public bool ErrorVisibility
        {
            get => _errorVisibility;
            set
            {
                _errorVisibility = value;
                OnPropertyChanged();
            }
        }

        public int InitializationTimeout
        {
            get => ModifiedConfig?.InitializationTimeout ?? 0;
            set
            {
                ModifiedConfig.InitializationTimeout = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract IConfigManager GetDeviceConfigManager();

        protected abstract void LoadEquipmentConfig();

        #endregion

        #region Override

        public override void OnSetup()
        {
            base.OnSetup();

            LoadEquipmentConfig();
            OnPropertyChanged(nameof(InitializationTimeout));
            App.Instance.ConfigurationManager.CurrentChanged += ConfigManager_OnCurrentChanged;
        }

        protected override IConfigManager GetConfigManager() => GetDeviceConfigManager();

        protected override bool ChangesNeedRestart => true;

        protected override void Dispose(bool disposing)
        {
            App.Instance.ConfigurationManager.CurrentChanged -= ConfigManager_OnCurrentChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Private methods

        private void ConfigManager_OnCurrentChanged(object sender, ConfigurationChangedEventArgs e)
            => DispatcherHelper.DoInUiThread(LoadEquipmentConfig);

        #endregion
    }
}
