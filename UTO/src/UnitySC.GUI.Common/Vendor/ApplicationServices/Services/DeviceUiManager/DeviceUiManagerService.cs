using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;

using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.GUI.Common.Vendor.ApplicationServices.Services.DeviceUiManager
{
    public class DeviceUiManagerService
    {
        public DeviceUiManagerService()
        {
            Logger = Agileo.Common.Logging.Logger.GetLogger(nameof(DeviceUiManagerService));
        }

        public ILogger Logger { get; }

        public List<DeviceUiFactory> UiFactories { get; } = new();

        public BusinessPanel CreatePanelFrom(Device device, string deviceConfigRootPath = "")
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

        public BaseEditor CreateEditorFrom(Device device, string deviceConfigRootPath = "")
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

        public virtual void Setup()
        {
            // Get all loaded assemblies in the current application domain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            //Load all available UI dlls that were not already loaded
            var allAssemblyNames = new DirectoryInfo(Directory.GetCurrentDirectory())
                .GetFiles("*.UI.dll", SearchOption.AllDirectories)
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
                                 .Where(DoesTypeInheritFromDeviceUiFactory))
                    {
                        try
                        {
                            UiFactories.Add((DeviceUiFactory)Activator.CreateInstance(factoryType));
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

        private static bool DoesTypeInheritFromDeviceUiFactory(Type type)
        {
            var currentType = type;
            while (currentType.BaseType is not null)
            {
                if (currentType.BaseType == typeof(DeviceUiFactory))
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }
    }
}
