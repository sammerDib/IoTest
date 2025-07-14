using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Analyse;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Emera;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule;
using UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.EquipmentHandling.ProcessModule.UnityProcessModule;
using UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Analyse;
using UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Demeter;
using UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Emera;
using UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Setup.DeviceSettings.DataFlowManager;
using UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Setup.DeviceSettings.ProcessModule.UnityProcessModule;
using UnitySC.GUI.Common.Equipment;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.DataFlow.ProcessModules.GUI
{
    public class DataFlowUiFactory : UnityDeviceUiFactory
    {
        public override BusinessPanel CreatePanel(Device device, string deviceConfigRootPath = "")
        {
            return device switch
            {
                DataFlowManager dataFlowManager => new DataFlowManagerSettingsPanel(
                    dataFlowManager,
                    dataFlowManager.Name,
                    PathIcon.Setup),
                UnityProcessModule unityProcessModule => new UnityProcessModuleSettingsPanel(
                    unityProcessModule,
                    unityProcessModule.Name,
                    PathIcon.ProcessModule),
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
                UnityProcessModule unityProcessModule => new UnityProcessModuleCardViewModel(unityProcessModule),
                _ => null
            };
        }

        public override UnityDeviceCardViewModel GetProductionCardViewModel(Device device)
        {
            return device switch
            {
                Analyse analyse => new ProductionAnalyseViewModel(analyse),
                Demeter demeter => new ProductionDemeterViewModel(demeter),
                Emera emera => new ProductionEmeraViewModel(emera),
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
