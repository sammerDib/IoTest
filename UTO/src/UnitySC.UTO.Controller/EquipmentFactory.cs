using System;
using System.IO;
using System.Linq;

using Agileo.Common.Communication.TCPIP;
using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.GUI.Common.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Simulation;

namespace UnitySC.UTO.Controller
{
    /// <summary>
    /// Class responsible to instantiate the proper EquipmentManger and its associated configuration.
    /// </summary>
    public static class EquipmentFactory
    {
        private static SimulatorView _simulatorView;

        /// <summary>
        /// Instantiates equipment manager based on provided data or a default one.
        /// </summary>
        /// <param name="config">Equipment configuration containing data to identify equipment to instantiate.</param>
        /// <returns></returns>
        public static EquipmentManager CreateEquipmentManager(EquipmentConfiguration config)
        {
            var equipmentFileName = config.EquipmentFileName;
            var equipmentsFolder = new DirectoryInfo(config.EquipmentsFolderPath);
            if (string.IsNullOrEmpty(config.EquipmentFileName) && equipmentsFolder.Exists)
            {
                // Get the first Equipment file in Equipments folder because equipment name not built properly with provided data
                equipmentFileName = equipmentsFolder.EnumerateFiles("*.equipment", SearchOption.TopDirectoryOnly)
                    .Select(f => Path.GetFileName(f.FullName))
                    .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(equipmentFileName))
            {
                // No equipment model found in Equipments folder, stop application
                GUI.Common.App.Instance.Logger.Error(
                    "Cannot load Equipment because no equipments found at: '{EquipmentsFolder}'",
                    equipmentsFolder.FullName);
                throw new InvalidOperationException($"No equipment model found in '{equipmentsFolder.FullName}' folder.");
            }

            TCPPostman.IsSocketLogEnabled = config.IsSocketLogEnabled;

            var equipmentFullPath = Path.Combine(equipmentsFolder.FullName, equipmentFileName);
            return new ControllerEquipmentManager(equipmentFullPath, SetUpEquipment);
        }

        private static void SetUpEquipment(Agileo.EquipmentModeling.Equipment equipment)
        {
            foreach (var configurableDevice in equipment.AllOfType<IConfigurableDevice>())
            {
                // Load device configuration
                configurableDevice.LoadConfiguration(GUI.Common.App.Instance.Config.EquipmentConfig.DeviceConfigFolderPath);

                // Load alarms
                if (configurableDevice is GenericDevice genericDevice)
                {
                    genericDevice.LoadAlarms(
                        GUI.Common.App.Instance.AlarmCenter,
                        configurableDevice.RelativeConfigurationDir,
                        GUI.Common.App.Instance.Config.EquipmentConfig.DeviceConfigFolderPath);
                }
            }

            if (equipment.TryGetDevice<Equipment.Devices.Controller.Controller>() is {} controller)
            {
                controller.LoadAlarms(
                    GUI.Common.App.Instance.AlarmCenter,
                    controller.RelativeConfigurationDir,
                    GUI.Common.App.Instance.Config.EquipmentConfig.DeviceConfigFolderPath);
            }

            if (equipment.AllDevices<Efem>().First() is { } efem)
            {
                efem.SetDevicesFolderPath(GUI.Common.App.Instance.Config.EquipmentConfig.DeviceConfigFolderPath);
            }

            SetUpAlarms(equipment);

            OpenSimulationWindow(equipment);
        }

        private static void SetUpAlarms(Agileo.EquipmentModeling.Equipment equipment)
        {
            if (equipment.Container is Package pkg)
            {
                foreach (var equip in pkg.AllEquipments())
                {
                    equip.SetAlarmCenter(GUI.Common.App.Instance.AlarmCenter);
                    equip.Setup();
                }
            }
        }

        private static void OpenSimulationWindow(Agileo.EquipmentModeling.Equipment equipment)
        {
            if (_simulatorView == null
                && equipment.AllOfType<ISimDevice>()
                    .Any(simDevice => (simDevice as Device)?.ExecutionMode == ExecutionMode.Simulated))
            {
                GUI.Common.App.Instance.Dispatcher.Invoke(
                    () => _simulatorView = SimulatorView.Open(equipment.Container as Package));
            }
        }

        public static void CloseSimulationWindow()
        {
            DispatcherHelper.DoInUiThread(() => _simulatorView?.ForceClose());
        }
    }
}
