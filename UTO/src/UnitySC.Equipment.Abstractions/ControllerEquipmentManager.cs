using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;

namespace UnitySC.Equipment.Abstractions
{
    public class ControllerEquipmentManager : EfemEquipmentManager
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerEquipmentManager"/> class.
        /// </summary>
        /// <param name="equipmentFilePath">The equipment file path.</param>
        /// <param name="setup">Setup callback, called after load of equipment</param>
        public ControllerEquipmentManager(
            string equipmentFilePath,
            Action<Agileo.EquipmentModeling.Equipment> setup = null)
            : base(equipmentFilePath, setup)
        {
            ProcessModules.Clear();
            Controller?.GetDevices<DriveableProcessModule>().ToList().ForEach(pm => ProcessModules.Add(pm.InstanceId, pm));
        }

        #endregion Constructors

        public new SortedList<int, DriveableProcessModule> ProcessModules { get; } = new();

        public new DriveableProcessModule ProcessModule1 =>
            ProcessModules.ContainsKey(1) ? ProcessModules[1] : null;

        public new DriveableProcessModule ProcessModule2 =>
            ProcessModules.ContainsKey(2) ? ProcessModules[2] : null;
    }
}
