using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.EFEM.Brooks.Devices.Aligner.BrooksAligner;
using UnitySC.EFEM.Brooks.Devices.Efem.BrooksEfem;
using UnitySC.EFEM.Brooks.Devices.Ffu.BrooksFfu;
using UnitySC.EFEM.Brooks.Devices.LightTower.BrooksLightTower;
using UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort;
using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot;
using UnitySC.EFEM.Brooks.Devices.SubstrateIdReader.BrooksSubstrateIdReader;
using UnitySC.EFEM.Brooks.GUI.Equipment.Robot;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Aligner.BrooksAligner;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Efem.BrooksEfem;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Ffu.BrooksFfu;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.LightTower.BrooksLightTower;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.LoadPortsSettings.BrooksLoadPort;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Robot.BrooksRobot;
using UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.SubstrateIdReader.BrooksSubstrateReader;
using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Equipment.Aligner.Enhanced;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Aligner;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Efem;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Ffu;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LightTower;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Robot;

namespace UnitySC.EFEM.Brooks.GUI
{
    public class BrooksUiFactory : UnityDeviceUiFactory
    {
        public override BusinessPanel CreatePanel(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                BrooksAligner aligner => new BrooksAlignerSettingsPanel(
                    aligner,
                    nameof(AlignerSettingsResources.BP_ALIGNER_SETTINGS),
                    PathIcon.Setup),
                BrooksEfem efem => new BrooksEfemSettingsPanel(
                    efem,
                    nameof(EfemSettingsResources.BP_EFEM_SETTINGS),
                    PathIcon.Setup),
                BrooksFfu ffu => new BrooksFfuSettingsPanel(
                    ffu,
                    nameof(FfuSettingsResources.BP_FFU_SETTINGS),
                    PathIcon.Setup),
                BrooksLoadPort loadPort => new BrooksLoadPortSettingsPanel(
                    loadPort,
                    loadPort.Name,
                    PathIcon.LoadPorts),
                BrooksSubstrateIdReader reader => new BrooksSubstrateReaderSettingsPanel(
                    reader,
                    reader.Name,
                    PathIcon.ReadTag),
                BrooksLightTower lightTower => new BrooksLightTowerSettingsPanel(
                    lightTower,
                    nameof(LightTowerSettingsPanelResources.S_SETUP_LIGHTTOWER_CONFIG),
                    PathIcon.Setup),
                BrooksRobot robot => new BrooksRobotSettingsPanel(
                    robot,
                    nameof(RobotSettingsResources.BP_ROBOT_SETTINGS),
                    PathIcon.Setup),
                _ => null
            };
        }

        public override BaseEditor CreateEditor(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                _ => null
            };
        }

        public override UnityDeviceCardViewModel GetEquipmentHandlingCardViewModel(Device device)
        {
            return device switch
            {
                BrooksRobot robot => new BrooksRobotCardViewModel(robot),
                BrooksAligner aligner => new EnhAlignerCardViewModel(aligner),
                _ => null
            };
        }

        public override UnityDeviceCardViewModel GetProductionCardViewModel(Device device)
        {
            return device switch
            {
                _ => null
            };
        }

        public override MachineModuleViewModel GetModuleViewModel(Device device)
        {
            return device switch
            {
                _ => null
            };
        }
    }
}
