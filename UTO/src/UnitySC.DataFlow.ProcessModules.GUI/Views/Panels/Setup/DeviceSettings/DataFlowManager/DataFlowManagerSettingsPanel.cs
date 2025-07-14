using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager.Configuration;
using UnitySC.GUI.Common.Helpers;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings;

namespace UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Setup.DeviceSettings.DataFlowManager
{
    public class DataFlowManagerSettingsPanel : DeviceSettingsPanel<DataFlowManagerConfiguration>
    {
        #region Fields

        private readonly DataFlow.ProcessModules.Devices.DataFlowManager.DataFlowManager
            _dataFlowManager;

        #endregion

        #region Contructors

        static DataFlowManagerSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(DataFlowManagerSettingsPanel), typeof(DataFlowManagerSettingsPanelView));

            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(DataFlowManagerSettingsResources)));
        }

        public DataFlowManagerSettingsPanel(
            DataFlow.ProcessModules.Devices.DataFlowManager.DataFlowManager dataFlowManager,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _dataFlowManager = dataFlowManager;

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
                                nameof(DataFlowManagerSettingsResources
                                    .S_SETUP_DEVICE_SETTINGS_WCF_HOST_PORT_INVALID));
                        }

                        return string.Empty;
                    }));
        }

        #endregion

        #region Properties

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

        public bool UseOnlyRecipeNameAsId
        {
            get => ModifiedConfig?.UseOnlyRecipeNameAsId ?? false;
            set
            {
                ModifiedConfig.UseOnlyRecipeNameAsId = value;
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

        protected override bool ChangesNeedRestart
        {
            get
            {
                return !ObjectAreEquals(
                           ModifiedConfig?.WcfConfiguration.WcfHostIpAddressAsString,
                           LoadedConfig?.WcfConfiguration.WcfHostIpAddressAsString)
                       || !ObjectAreEquals(
                           ModifiedConfig?.WcfConfiguration.WcfHostPort,
                           LoadedConfig?.WcfConfiguration.WcfHostPort)
                       || !ObjectAreEquals(
                           ModifiedConfig?.WcfConfiguration.WcfServiceUriSegment,
                           LoadedConfig?.WcfConfiguration.WcfServiceUriSegment)
                       || !ObjectAreEquals(
                           ModifiedConfig?.WcfConfiguration.WcfCommunicationCheckDelay,
                           LoadedConfig?.WcfConfiguration.WcfCommunicationCheckDelay);
            }
        }

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _dataFlowManager.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            WCFHostIPAddressValue = ModifiedConfig.WcfConfiguration.WcfHostIpAddressAsString;
            OnPropertyChanged(nameof(WCFHostIPAddressValue));
            OnPropertyChanged(nameof(WCFHostPortValue));
            OnPropertyChanged(nameof(WCFServiceURISegmentValue));
            OnPropertyChanged(nameof(WCFTimeoutValue));
            OnPropertyChanged(nameof(WcfRetryNumber));
            OnPropertyChanged(nameof(UseOnlyRecipeNameAsId));
        }

        protected override bool ConfigurationEqualsLoaded()
        {
            return ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfHostIpAddressAsString,
                       LoadedConfig?.WcfConfiguration.WcfHostIpAddressAsString)
                   && ObjectAreEquals(
                        ModifiedConfig?.UseOnlyRecipeNameAsId,
                        LoadedConfig?.UseOnlyRecipeNameAsId)
                   && base.ConfigurationEqualsLoaded();
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return ObjectAreEquals(
                       ModifiedConfig?.WcfConfiguration.WcfHostIpAddressAsString,
                       CurrentConfig?.WcfConfiguration.WcfHostIpAddressAsString)
                   && ObjectAreEquals(
                       ModifiedConfig?.UseOnlyRecipeNameAsId,
                       CurrentConfig?.UseOnlyRecipeNameAsId)
                   && base.ConfigurationEqualsCurrent();
        }

        #endregion
    }
}
