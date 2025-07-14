using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.Configuration;
using UnitySC.GUI.Common.Helpers;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings;

namespace UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Setup.DeviceSettings.ProcessModule.UnityProcessModule
{
    public class UnityProcessModuleSettingsPanel
        : DeviceSettingsPanel<UnityProcessModuleConfiguration>
    {
        #region Fields

        private readonly
            DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.UnityProcessModule
            _unityProcessModule;

        #endregion

        #region Contructors

        static UnityProcessModuleSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(UnityProcessModuleSettingsPanel), typeof(UnityProcessModuleSettingsPanelView));

            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(UnityProcessModuleSettingsResources)));
        }

        public UnityProcessModuleSettingsPanel(
            DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.UnityProcessModule
                processModule,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _unityProcessModule = processModule;

            Rules.Add(
                new DelegateRule(
                    nameof(WCFHostIPAddressValue),
                    () => IpHelper.ValidateIp(WCFHostIPAddressValue)));
            Rules.Add(
                new DelegateRule(
                    nameof(WCFHostPortValue),
                    () =>
                    {
                        if (WCFHostPortValue >= 65535)
                        {
                            return LocalizationManager.GetString(
                                nameof(UnityProcessModuleSettingsResources
                                    .S_SETUP_DEVICE_SETTINGS_WCF_HOST_PORT_INVALID));
                        }

                        return string.Empty;
                    }));
        }

        #endregion

        #region Properties

        public bool IsOutOfService
        {
            get => ModifiedConfig?.IsOutOfService ?? false;
            set
            {
                ModifiedConfig.IsOutOfService = value;
                OnPropertyChanged();
            }
        }

        public uint WCFHostPortValue
        {
            get => ModifiedConfig?.WcfConfiguration.WcfHostPort ?? 0;
            set
            {
                ModifiedConfig.WcfConfiguration.WcfHostPort = value;
                OnPropertyChanged();
            }
        }

        private string _wcfHostIpAddressValue;

        public string WCFHostIPAddressValue
        {
            get => _wcfHostIpAddressValue;
            set
            {
                ModifiedConfig.WcfConfiguration.WcfHostIpAddressAsString = value;
                SetAndRaiseIfChanged(ref _wcfHostIpAddressValue, value);
            }
        }

        public string WCFServiceURISegmentValue
        {
            get => ModifiedConfig?.WcfConfiguration.WcfServiceUriSegment;
            set
            {
                ModifiedConfig.WcfConfiguration.WcfServiceUriSegment = value;
                OnPropertyChanged();
            }
        }

        public uint WCFTimeoutValue
        {
            get => ModifiedConfig?.WcfConfiguration.WcfCommunicationCheckDelay ?? 0;
            set
            {
                ModifiedConfig.WcfConfiguration.WcfCommunicationCheckDelay = value;
                OnPropertyChanged();
            }
        }

        public uint WcfRetryNumber
        {
            get => ModifiedConfig?.WcfConfiguration.WcfRetryNumber ?? 0;
            set
            {
                ModifiedConfig.WcfConfiguration.WcfRetryNumber = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Overrides

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _unityProcessModule.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            WCFHostIPAddressValue = ModifiedConfig.WcfConfiguration.WcfHostIpAddressAsString;
            OnPropertyChanged(nameof(IsOutOfService));
            OnPropertyChanged(nameof(WCFHostIPAddressValue));
            OnPropertyChanged(nameof(WCFHostPortValue));
            OnPropertyChanged(nameof(WCFServiceURISegmentValue));
            OnPropertyChanged(nameof(WCFTimeoutValue));
            OnPropertyChanged(nameof(WcfRetryNumber));
        }

        protected override bool ConfigurationEqualsLoaded()
        {
            return ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfHostIpAddressAsString,
                       LoadedConfig?.WcfConfiguration.WcfHostIpAddressAsString)
                   && ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfHostPort,
                       LoadedConfig?.WcfConfiguration.WcfHostPort)
                   && ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfServiceUriSegment,
                       LoadedConfig?.WcfConfiguration.WcfServiceUriSegment)
                   && ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfCommunicationCheckDelay,
                       LoadedConfig?.WcfConfiguration.WcfCommunicationCheckDelay);
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfHostIpAddressAsString,
                       CurrentConfig?.WcfConfiguration.WcfHostIpAddressAsString)
                   && ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfHostPort,
                       CurrentConfig?.WcfConfiguration.WcfHostPort)
                   && ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfServiceUriSegment,
                       CurrentConfig?.WcfConfiguration.WcfServiceUriSegment)
                   && ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfCommunicationCheckDelay,
                       CurrentConfig?.WcfConfiguration.WcfCommunicationCheckDelay)
                   && ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfRetryNumber,
                       CurrentConfig?.WcfConfiguration.WcfRetryNumber)
                   && base.ConfigurationEqualsCurrent();
        }

        #endregion
    }
}
