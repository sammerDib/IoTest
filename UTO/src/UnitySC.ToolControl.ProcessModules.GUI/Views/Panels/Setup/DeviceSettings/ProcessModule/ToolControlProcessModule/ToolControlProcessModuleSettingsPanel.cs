using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings;
using UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule.Configuration;

namespace UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.Setup.DeviceSettings.ProcessModule.
    ToolControlProcessModule
{
    public class ToolControlProcessModuleSettingsPanel
        : DeviceSettingsPanel<ToolControlProcessModuleConfiguration>
    {
        #region Fields

        private readonly
            ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule.
            ToolControlProcessModule _processModule;

        #endregion

        #region Constructors

        static ToolControlProcessModuleSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(ToolControlProcessModuleSettingsPanel), typeof(ToolControlProcessModuleSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(ToolControlProcessModuleSettingsResources)));
        }

        public ToolControlProcessModuleSettingsPanel(
            ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule.
                ToolControlProcessModule processModule,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _processModule = processModule;
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

        public string ModuleId
        {
            get => ModifiedConfig?.ModuleId;
            set
            {
                ModifiedConfig.ModuleId = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Overrides

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _processModule.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            OnPropertyChanged(nameof(ModuleId));
        }

        //We do not need to restart the software to update the ModuleId
        protected override bool ConfigurationEqualsLoaded()
        {
            return true;
        }

        #endregion
    }
}
