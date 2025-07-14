using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Controller.Configuration;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings;

namespace UnitySC.Equipment.GUI.Views.Panels.Setup.DeviceSettings.Controller
{
    public abstract class ControllerConfigurationSettingsPanel<T> : DeviceSettingsPanel<T>
        where T : ControllerConfiguration, new()
    {
        #region Constructors

        static ControllerConfigurationSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ControllerSettingsResources)));
        }

        protected ControllerConfigurationSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            
        }

        #endregion

        #region Overrides

        protected override bool ConfigurationEqualsLoaded() => true;

        #endregion
    }
}
