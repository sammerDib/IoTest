using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420;
using UnitySC.EFEM.Rorze.Devices.Efem.MediumSizeEfem;
using UnitySC.EFEM.Rorze.Devices.Efem.SlimEfem;
using UnitySC.EFEM.Rorze.Devices.Efem.UniversalEfem;
using UnitySC.EFEM.Rorze.Devices.Ffu;
using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx;
using UnitySC.EFEM.Rorze.Devices.LightTower;
using UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201;
using UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x;
using UnitySC.EFEM.Rorze.GUI.UIComponents.Components.Equipment.Modules.LoadPort.LayingPlanLoadPort;
using UnitySC.EFEM.Rorze.GUI.UIComponents.Components.Equipment.Modules.Robot.MapperRR75x;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Aligner.RA420;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.EK9000;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.GenericRC5xx;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.LoadPortSettings.LayingPlanLoadPort;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.LoadPortSettings.RE201;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.LoadPortSettings.RV101;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.LoadPortSettings.RV201;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.MapperRR75x;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.RR75x;
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

namespace UnitySC.EFEM.Rorze.GUI
{
    public class RorzeUiFactory : UnityDeviceUiFactory
    {
        public override BusinessPanel CreatePanel(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                RA420 ra420 => new RA420SettingsPanel(
                    ra420,
                    nameof(AlignerSettingsResources.BP_ALIGNER_SETTINGS),
                    PathIcon.Setup),
                EK9000 ek9000 => new EK9000SettingsPanel(
                    ek9000,
                    nameof(IoModuleSettingsResources.BP_IO_MODULE_SETTINGS),
                    PathIcon.Setup),
                GenericRC5xx => new GenericRC5xxSettingsPanel(
                    IoModuleSettingsResources.BP_IO_MODULE_SETTINGS,
                    PathIcon.Setup),
                RV101 rv101 => new RV101SettingsPanel(rv101, rv101.Name, PathIcon.LoadPorts),
                RV201 rv201 => new RV201SettingsPanel(rv201, rv201.Name, PathIcon.LoadPorts),
                RE201 re201 => new RE201SettingsPanel(re201, re201.Name, PathIcon.LoadPorts),
                MapperRR75x mapperRr75X => new MapperRR75xSettingsPanel(
                    mapperRr75X,
                    nameof(RobotSettingsResources.BP_ROBOT_SETTINGS),
                    PathIcon.Setup),
                RR75x rr75X => new RR75xSettingsPanel(
                    rr75X,
                    nameof(RobotSettingsResources.BP_ROBOT_SETTINGS),
                    PathIcon.Setup),
                SlimEfem slimEfem => new EfemSettingsPanel(
                    slimEfem,
                    nameof(EfemSettingsResources.BP_EFEM_SETTINGS),
                    PathIcon.Setup),
                UniversalEfem universalEfem => new EfemSettingsPanel(
                    universalEfem,
                    nameof(EfemSettingsResources.BP_EFEM_SETTINGS),
                    PathIcon.Setup),
                MediumSizeEfem mediumSizeEfem => new EfemSettingsPanel(
                    mediumSizeEfem,
                    nameof(EfemSettingsResources.BP_EFEM_SETTINGS),
                    PathIcon.Setup),
                Ffu ffu => new FfuSettingsPanel(
                    ffu,
                    nameof(FfuSettingsResources.BP_FFU_SETTINGS),
                    PathIcon.Setup),
                LightTower lightTower => new LightTowerSettingsPanel(
                    lightTower,
                    nameof(LightTowerSettingsPanelResources.S_SETUP_LIGHTTOWER_CONFIG),
                    PathIcon.Setup),
                LayingPlanLoadPort layingPlanLoadPort => new LayingPlanLoadPortSettingsPanel(
                    layingPlanLoadPort,
                    layingPlanLoadPort.Name,
                    PathIcon.LoadPort),
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
                RA420 ra420 => new EnhAlignerCardViewModel(ra420),
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
                LayingPlanLoadPort layingPlanLoadPort => new LayingPlanLoadPortModuleViewModel(layingPlanLoadPort),
                MapperRR75x mapperRr75X => new MapperRR75xModuleViewModel(mapperRr75X),
                _ => null
            };
        }
    }
}
