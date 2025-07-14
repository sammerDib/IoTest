using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Configuration;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.GlobalSettings
{
    public class GlobalSettingsPanel : SetupNodePanel<Agileo.GUI.Configuration.GlobalSettings>
    {
        #region Constructors

        static GlobalSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(GlobalSettingsPanelResources)));
        }

        public GlobalSettingsPanel()
            : this("DesignTime Constructor")
        {
        }

        public GlobalSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
        }

        #endregion Constructors

        #region Override

        protected override Agileo.GUI.Configuration.GlobalSettings GetNode(
            ApplicationConfiguration applicationConfiguration)
            => applicationConfiguration?.GlobalSettings;

        protected override bool ChangesNeedRestart => true;

        #endregion Override
    }
}
