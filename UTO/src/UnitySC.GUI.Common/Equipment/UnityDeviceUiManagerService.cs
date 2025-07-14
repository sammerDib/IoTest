using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.GUI.Common.Equipment.Aligner;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Equipment.ProcessModule;
using UnitySC.GUI.Common.Equipment.Robot;
using UnitySC.GUI.Common.Equipment.SubstrateIdReader;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Aligner;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.LoadPort;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.ProcessModule;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Robot;
using UnitySC.GUI.Common.Vendor.ApplicationServices.Services.DeviceUiManager;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.GUI.Common.Equipment
{
    public class UnityDeviceUiManagerService : DeviceUiManagerService
    {
        #region Properties

        public new List<UnityDeviceUiFactory> UiFactories { get; } = new();

        #endregion

        #region Setup

        public override void Setup()
        {
            // Get all loaded assemblies in the current application domain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            //Load all available UI dlls that were not already loaded
            var allAssemblyNames = new DirectoryInfo(Directory.GetCurrentDirectory())
                .GetFiles("*.GUI.dll", SearchOption.AllDirectories)
                .Select(x => x.FullName).Select(AssemblyName.GetAssemblyName);
            var unloadedAssemblies = allAssemblyNames.Where(
                x => !assemblies.Select(y => y.FullName).Contains(x.FullName));

            //Attempt to load those DLLs
            foreach (var assemblyName in unloadedAssemblies)
            {
                try
                {
                    var loadedAssembly = Assembly.Load(assemblyName.FullName);
                    assemblies.Add(loadedAssembly);
                }
                catch (Exception ex)
                {
                    Logger.Error(
                        ex,
                        $"Exception occured when loading UI dll at path {unloadedAssemblies}.");
                }
            }

            // Iterate through all loaded assemblies and check if they contain classes implementing DeviceUiFactory
            foreach (var assembly in assemblies)
            {
                try
                {
                    // Find the types that inherit from the generic base class
                    foreach (var factoryType in assembly.GetTypes()
                                 .Where(DoesTypeInheritFromUnityDeviceUiFactory))
                    {
                        try
                        {
                            UiFactories.Add((UnityDeviceUiFactory)Activator.CreateInstance(factoryType));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(
                                ex,
                                $"Exception occured during device UI factory instantiation for type {factoryType.Name}.");
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Logger.Error(
                        ex,
                        $"{nameof(ReflectionTypeLoadException)} occured during search of {nameof(DeviceUiFactory)} types in assembly {assembly.FullName}.");
                }
            }
        }

        private static bool DoesTypeInheritFromUnityDeviceUiFactory(Type type)
        {
            var currentType = type;
            while (currentType.BaseType is not null)
            {
                if (currentType.BaseType == typeof(UnityDeviceUiFactory))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        #endregion

        #region New methods

        public new BusinessPanel CreatePanelFrom(Device device, string deviceConfigRootPath = "")
        {
            foreach (var factory in UiFactories)
            {
                if (factory.CreatePanel(device, deviceConfigRootPath) is { } panel)
                {
                    return panel;
                }
            }

            return null;
        }

        public new BaseEditor CreateEditorFrom(Device device, string deviceConfigRootPath = "")
        {
            foreach (var factory in UiFactories)
            {
                if (factory.CreateEditor(device, deviceConfigRootPath) is { } editor)
                {
                    return editor;
                }
            }

            return null;
        }

        #endregion

        #region Get Equipment handling card

        public UnityDeviceCardViewModel GetEquipmentHandlingCardViewModel(Device device, bool needBase = false)
        {
            if (needBase)
            {
                return GetDefaultCardViewModel(device);
            }

            foreach (var factory in UiFactories)
            {
                if (factory.GetEquipmentHandlingCardViewModel(device) is { } panel)
                {
                    return panel;
                }
            }

            return GetDefaultCardViewModel(device);
        }

        private UnityDeviceCardViewModel GetDefaultCardViewModel(Device device)
        {
            return device switch
            {
                UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner aligner =>
                    new AlignerCardViewModel(aligner),
                UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort loadPort =>
                    new LoadPortCardViewModel(loadPort),
                UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule processModule =>
                    new ProcessModuleCardViewModel(processModule),
                UnitySC.Equipment.Abstractions.Devices.Robot.Robot robot => new RobotCardViewModel(
                    robot),
                UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader reader =>
                    new SubstrateIdReaderCardViewModel(reader),
                _ => null
            };
        }

        #endregion

        #region Get production card

        public UnityDeviceCardViewModel GetProductionCardViewModel(Device device)
        {
            foreach (var factory in UiFactories)
            {
                if (factory.GetProductionCardViewModel(device) is { } panel)
                {
                    return panel;
                }
            }

            return null;
        }

        #endregion

        #region Get Module

        public MachineModuleViewModel GetModuleViewModel(Device device)
        {
            foreach (var factory in UiFactories)
            {
                if (factory.GetModuleViewModel(device) is { } machineModule)
                {
                    return machineModule;
                }
            }

            return GetDefaultModuleViewModel(device);
        }

        private MachineModuleViewModel GetDefaultModuleViewModel(Device device)
        {
            return device switch
            {
                UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner aligner =>
                    new AlignerModuleViewModel(aligner),
                UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort loadPort =>
                    new LoadPortModuleViewModel(loadPort),
                UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule processModule =>
                    new ProcessModuleViewModel(processModule),
                UnitySC.Equipment.Abstractions.Devices.Robot.Robot robot =>
                    new RobotModuleViewModel(robot),
                _ => null
            };
        }

        #endregion
    }
}
