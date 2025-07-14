using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

namespace UnitySC.EquipmentController.Simulator.EquipmentData
{
    /// <summary>
    /// Class containing all data about device EFEM (global equipment).
    /// It also provide accessors to individual devices.
    /// It's objective is not to provide any logic or automatic behavior.
    /// It only aims to concentrate all known data sent from the EfemController.
    /// </summary>
    internal class EfemData
    {
        internal EfemData()
        {
            RobotData       = new RobotData();
            AlignerData     = new AlignerData();
            SignalTowerData = new SignalTowerData();
            OcrData         = new OcrData();

            foreach (var port in Enum.GetValues(typeof(Constants.Port)).Cast<Constants.Port>())
                LoadPortsData.Add(port, new LoadPortData());
        }

        internal OperationMode OperationMode { get; set; }

        internal bool SafetyDoorSensor { get; set; }

        internal bool VacuumSensor { get; set; }

        internal bool AirSensor { get; set; }

        internal double Pressure { get; set; }

        internal uint FfuSpeed { get; set; }

        #region Subdevice Data

        internal AlignerData AlignerData { get; }

        internal SortedList<Constants.Port, LoadPortData> LoadPortsData { get; } = new();

        internal RobotData RobotData { get; }

        internal SignalTowerData SignalTowerData { get; }

        internal OcrData OcrData { get; }

        #endregion Subdevice Data
    }
}
