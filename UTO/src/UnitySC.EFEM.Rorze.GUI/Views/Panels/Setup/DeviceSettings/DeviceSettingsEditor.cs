using System;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.Common.Logging;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings
{
    public abstract class DeviceSettingsEditor<T> : BaseEditor<T>, IDisposable
         where T : DeviceConfiguration, new()
    {
        #region Constructors

        static DeviceSettingsEditor()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DeviceSettingsResources)));
        }

        protected DeviceSettingsEditor(ILogger logger)
            : base(logger)
        {
            Rules.Add(DelegateRule.GreaterThanOrEqual(nameof(InitializationTimeout), () => InitializationTimeout, 0));
        }

        #endregion

        #region Properties

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
            ConfigManager.CurrentChanged += ConfigManager_OnCurrentChanged;
        }

        protected override IConfigManager GetConfigManager() => GetDeviceConfigManager();

        #endregion

        #region Private methods

        private void ConfigManager_OnCurrentChanged(object sender, ConfigurationChangedEventArgs e)
            => DispatcherHelper.DoInUiThread(LoadEquipmentConfig);

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ConfigManager.CurrentChanged -= ConfigManager_OnCurrentChanged;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
