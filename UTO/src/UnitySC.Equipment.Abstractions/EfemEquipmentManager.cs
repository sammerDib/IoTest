using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Controller;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.Ffu;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

using ProcessModule = UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule;

namespace UnitySC.Equipment.Abstractions
{
    public class EfemEquipmentManager : EquipmentManager
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfemEquipmentManager"/> class.
        /// </summary>
        /// <param name="equipmentFilePath">The equipment file path.</param>
        /// <param name="setup">Setup callback, called after load of equipment</param>
        public EfemEquipmentManager(
            string equipmentFilePath,
            Action<Agileo.EquipmentModeling.Equipment> setup = null)
            : base(equipmentFilePath, setup)
        {
            Controller = Equipment.TryGetDevice<Controller>();
            Efem = Controller?.TryGetDevice<Efem>();
            Aligner = Efem?.TryGetDevice<Aligner>();
            Robot = Efem?.TryGetDevice<Robot>();
            LightTower = Controller?.TryGetDevice<LightTower>();
            Ffu = Controller?.TryGetDevice<Ffu>();

            Efem?.GetDevices<LoadPort>().ToList().ForEach(lp => LoadPorts.Add(lp.InstanceId, lp));
            Controller?.GetDevices<ProcessModule>().ToList().ForEach(pm => ProcessModules.Add(pm.InstanceId, pm));
            Efem?.AllDevices<SubstrateIdReader>()
                .ToList()
                .ForEach(reader => SubstrateIdReaders.Add(reader.InstanceId, reader));
        }

        #endregion Constructors

        #region EFEM Devices Accessors

        public Controller Controller { get; }

        public Aligner Aligner { get; }

        public SortedList<int, SubstrateIdReader> SubstrateIdReaders { get; } = new();

        public Efem Efem { get; }

        public SortedList<int, LoadPort> LoadPorts { get; } = new();

        public Robot Robot { get; }

        public SortedList<int, ProcessModule> ProcessModules { get; } = new();

        public LightTower LightTower { get; }

        public Ffu Ffu { get; }

        #region ShortCut

        public SubstrateIdReader SubstrateIdReaderFront
            => Efem?.AllDevices<SubstrateIdReader>().FirstOrDefault(d => d.InstanceId == 1);

        public SubstrateIdReader SubstrateIdReaderBack
            => Efem?.AllDevices<SubstrateIdReader>().FirstOrDefault(d => d.InstanceId == 2);

        public LoadPort LoadPort1 =>
            LoadPorts.ContainsKey(1) ? LoadPorts[1] : null;

        public LoadPort LoadPort2 =>
            LoadPorts.ContainsKey(2) ? LoadPorts[2] : null;

        public ProcessModule ProcessModule1 =>
            ProcessModules.ContainsKey(1) ? ProcessModules[1] : null;

        public ProcessModule ProcessModule2 =>
            ProcessModules.ContainsKey(2) ? ProcessModules[2] : null;
        #endregion

        /// <summary>
        /// Gets all the instance of <see cref="UnityCommunicatingDevice"/> present inside the loaded equipment.
        /// </summary>
        public new IEnumerable<UnityCommunicatingDevice> CommunicatingDevices
            => Equipment.AllDevices<UnityCommunicatingDevice>();

        #endregion EFEM Devices Accessors

        public bool IsEquipmentEmpty()
        {
            return Aligner.Location.Wafer == null
                   && Robot.UpperArmLocation.Wafer == null
                   && Robot.LowerArmLocation.Wafer == null
                   && ProcessModules.Values.All(pm => pm.Location.Wafer == null);
        }

        public bool IsControllerOk()
        {
            return Controller.State is OperatingModes.Idle or OperatingModes.Executing;
        }
    }
}
