using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Efem.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Efem
{
    public class EfemSettingsPanel : EfemConfigurationSettingsPanel<EfemConfiguration>
    {
        #region Fields

        private readonly UnitySC.Equipment.Abstractions.Devices.Efem.Efem _efem;

        #endregion

        static EfemSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(EfemSettingsPanel), typeof(EfemSettingsPanelView));
        }

        public EfemSettingsPanel(UnitySC.Equipment.Abstractions.Devices.Efem.Efem efem, string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            _efem = efem;
        }

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _efem.ConfigManager;
        }
    }
}
