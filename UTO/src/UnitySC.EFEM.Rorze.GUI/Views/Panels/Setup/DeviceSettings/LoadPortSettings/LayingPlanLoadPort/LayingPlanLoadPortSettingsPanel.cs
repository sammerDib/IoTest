using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.GUI.Common;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LoadPortsSettings;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.LoadPortSettings.
    LayingPlanLoadPort
{
    public class LayingPlanLoadPortSettingsPanel : LoadPortSettingsPanel<LoadPortConfiguration>
    {
        #region Fields

        private readonly Devices.LoadPort.LayingPlanLoadPort.LayingPlanLoadPort _loadPort;

        #endregion

        #region Constructor

        static LayingPlanLoadPortSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(LayingPlanLoadPortSettingsPanel), typeof(LayingPlanLoadPortSettingsPanelView));
        }

        public LayingPlanLoadPortSettingsPanel(
            Devices.LoadPort.LayingPlanLoadPort.LayingPlanLoadPort loadPort,
            string relativeId,
            IIcon icon = null)
            : base(loadPort, relativeId, icon)
        {
            _loadPort = loadPort;
        }

        #endregion

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _loadPort.LoadDeviceConfiguration(
                App.Instance.Config.EquipmentConfig.DeviceConfigFolderPath,
                Logger,
                _loadPort.InstanceId);
        }
    }
}
