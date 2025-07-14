using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Equipment.Devices.Controller.Throughput
{
    public class DeviceStatistics
    {
        #region Constructor

        public DeviceStatistics(Device device)
        {
            Device = device;
        }

        #endregion

        #region Properties

        public Device Device { get; }

        public List<DateTime> CheckPointsWaferPlaced { get; } = new();
        public List<DateTime> CheckPointsProcessStarted { get; } = new();
        public List<DateTime> CheckPointsProcessFinished { get; } = new();
        public List<DateTime> CheckPointsWaferRemoved { get; } = new();

        public string CurrentWaferIdPlaced { get; set; }
        public string InfoWafer { get; set; }
        public string InfoPm { get; set; }

        public string ProcessTime { get; set; }
        public string WaferCycleTime { get; set; }
        public string ThroughputMeasured { get; set; }

        public int WaferCounter { get; private set; }

        #endregion

        #region Public Methods

        public void CalculateWaferCounter()
        {
            var cptPlaced = CheckPointsWaferPlaced.Count;
            var cptStarted = CheckPointsProcessStarted.Count;
            var cptFinished = CheckPointsProcessFinished.Count;
            var cptRemoved = CheckPointsWaferRemoved.Count;

            WaferCounter = Convert.ToInt32(
                Math.Floor((cptPlaced + cptStarted + cptFinished + cptRemoved) / (double)4));
        }

        #endregion
    }

    public class ThroughputStatistics : IDisposable
    {
        #region Fields

        private List<DeviceStatistics> _deviceStatistics;
        private readonly List<DriveableProcessModule> _processModules;
        private readonly AbstractDataFlowManager _dataFlowManager;
        private readonly Controller _controller;

        #endregion

        #region Properties

        private double _throughput;

        public double Throughput
        {
            get => _throughput;
            set
            {
                if (_throughput != value)
                {
                    _throughput = value;
                    OnThroughputValueChanged(_throughput);
                }
            }
        }

        #endregion

        #region Constructor

        public ThroughputStatistics(Controller controller)
        {
            _controller = controller;
            _processModules = controller.AllDevices<DriveableProcessModule>().ToList();
            _dataFlowManager = controller.AllDevices<AbstractDataFlowManager>().First();
            _deviceStatistics = new List<DeviceStatistics>();
            foreach (var processModule in _processModules)
            {
                _deviceStatistics.Add(new DeviceStatistics(processModule));
                processModule.Location.PropertyChanged += ProcessModuleLocation_PropertyChanged;
            }

            _dataFlowManager.ProcessModuleRecipeStarted +=
                DataFlowManager_ProcessModuleRecipeStarted;
            _dataFlowManager.ProcessModuleRecipeCompleted +=
                DataFlowManager_ProcessModuleRecipeCompleted;
        }

        #endregion

        #region Events

        public event EventHandler<ThroughputEventArgs> ThroughputValueChanged;

        #endregion

        #region Event Handlers

        private void ProcessModuleLocation_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not MaterialLocation materialLocation
                || materialLocation.Container is not DriveableProcessModule processModule)
            {
                return;
            }

            if (processModule.Location.Wafer == null)
            {
                WaferRemoved(processModule.Name);
            }
            else if (processModule.Location.Wafer != null
                     && processModule.Location.Wafer.JobProgram != null
                     && processModule.InstanceId
                     == GetProcessModule(
                             processModule.Location.Wafer.JobProgram.PMItems.Last().PMType)
                         .InstanceId)
            {
                WaferPlaced(processModule.Name, processModule.Location.Wafer.SubstrateId);
            }
        }

        private void DataFlowManager_ProcessModuleRecipeCompleted(
            object sender,
            ProcessModuleRecipeEventArgs e)
        {
            if (e.ProcessModule.Location.Substrate != null
                && e.ProcessModule.Location.Wafer.JobProgram != null
                && e.ProcessModule.InstanceId
                == GetProcessModule(e.ProcessModule.Location.Wafer.JobProgram.PMItems.Last().PMType)
                    .InstanceId)
            {
                ProcessFinished(e.ProcessModule.Name);
            }
        }

        private void DataFlowManager_ProcessModuleRecipeStarted(
            object sender,
            ProcessModuleRecipeEventArgs e)
        {
            if (e.ProcessModule.Location.Substrate != null
                && e.ProcessModule.Location.Wafer.JobProgram != null
                && e.ProcessModule.InstanceId
                == GetProcessModule(
                        e.ProcessModule.Location.Wafer.JobProgram.PMItems.First().PMType)
                    .InstanceId)
            {
                ProcessStart(e.ProcessModule.Name);
            }
        }

        #endregion

        #region Private Methods

        private void WaferPlaced(string deviceName, string waferId)
        {
            var stats = _deviceStatistics.FirstOrDefault(d => d.Device.Name == deviceName);
            if (stats == null)
            {
                return;
            }

            stats.CurrentWaferIdPlaced = waferId;
            stats.CheckPointsWaferPlaced.Add(DateTime.Now);
            stats.CalculateWaferCounter();
            SendCalculationStatsStart(deviceName);
        }

        private void WaferRemoved(string deviceName)
        {
            var stats = _deviceStatistics.FirstOrDefault(d => d.Device.Name == deviceName);
            if (stats == null || stats.CheckPointsWaferPlaced.Count <= 0)
            {
                return;
            }

            stats.CheckPointsWaferRemoved.Add(DateTime.Now);
            stats.CalculateWaferCounter();
            stats.CurrentWaferIdPlaced = string.Empty;
        }

        private void ProcessStart(string deviceName)
        {
            var stats = _deviceStatistics.FirstOrDefault(d => d.Device.Name == deviceName);
            if (stats == null || stats.CheckPointsWaferPlaced.Count <= 0)
            {
                return;
            }

            stats.CheckPointsProcessStarted.Add(DateTime.Now);
            stats.CalculateWaferCounter();
        }

        private void ProcessFinished(string deviceName)
        {
            var stats = _deviceStatistics.FirstOrDefault(d => d.Device.Name == deviceName);
            if (stats == null || stats.CheckPointsWaferPlaced.Count <= 0)
            {
                return;
            }

            stats.CheckPointsProcessFinished.Add(DateTime.Now);
            stats.CalculateWaferCounter();
        }

        private void SendCalculationStatsStart(string deviceName)
        {
            var threadCalculation = new Thread(CalculateThroughputWaferStatsExecute);
            threadCalculation.Name = "THD_Calculation_Execute" + DateTime.Now.ToShortTimeString();
            threadCalculation.Start(deviceName);
        }

        private void CalculateThroughputWaferStatsExecute(object deviceName)
        {
            try
            {
                var stats =
                    _deviceStatistics.FirstOrDefault(d => d.Device.Name == deviceName.ToString());
                if (stats == null)
                {
                    return;
                }

                // Process time in PM per wafer
                if (stats.CheckPointsProcessStarted.Count > 0
                    && stats.CheckPointsProcessStarted.Count
                    == stats.CheckPointsProcessFinished.Count)
                {
                    var totalProcessTimeInPm = stats
                        .CheckPointsProcessFinished[stats.CheckPointsProcessFinished.Count - 1]
                        .Subtract(
                            stats.CheckPointsProcessStarted[stats.CheckPointsProcessStarted.Count
                                                            - 1]);
                    stats.ProcessTime = totalProcessTimeInPm.ToString(@"mm\:ss\.fff");
                    stats.InfoPm =
                        "Process Time = Time [Process Finished] - Time [Process started]";
                }

                //Throughtput
                if (stats.CheckPointsWaferPlaced.Count - 2 >= 0)
                {
                    var waferCycleTime = stats
                        .CheckPointsWaferPlaced[stats.CheckPointsWaferPlaced.Count - 1]
                        .Subtract(
                            stats.CheckPointsWaferPlaced[stats.CheckPointsWaferPlaced.Count - 2]);
                    stats.WaferCycleTime = waferCycleTime.ToString(@"mm\:ss\.fff");
                    var waferMaximumNumber = 5;
                    TimeSpan totalWaferCycleTime;
                    if (stats.CheckPointsWaferPlaced.Count <= waferMaximumNumber)
                    {
                        totalWaferCycleTime =
                            stats.CheckPointsWaferPlaced[stats.CheckPointsWaferPlaced.Count - 1]
                                .Subtract(stats.CheckPointsWaferPlaced[0]);

                        // Throughput wafer
                        Throughput = (stats.CheckPointsWaferPlaced.Count - 1)
                                     / totalWaferCycleTime.TotalSeconds
                                     * 3600;
                        stats.ThroughputMeasured = Throughput.ToString(
                            "G5",
                            CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        totalWaferCycleTime = stats
                            .CheckPointsWaferPlaced[stats.CheckPointsWaferPlaced.Count - 1]
                            .Subtract(
                                stats.CheckPointsWaferPlaced[stats.CheckPointsWaferPlaced.Count
                                                             - 1
                                                             - waferMaximumNumber]);

                        // Throughput wafer
                        Throughput = waferMaximumNumber / totalWaferCycleTime.TotalSeconds * 3600;
                        stats.ThroughputMeasured = Throughput.ToString(
                            "G5",
                            CultureInfo.InvariantCulture);
                        if (stats.CheckPointsWaferPlaced.Count - 1 - waferMaximumNumber >= 1)
                        {
                            stats.CheckPointsProcessFinished.RemoveAt(0);
                            stats.CheckPointsProcessStarted.RemoveAt(0);
                            stats.CheckPointsWaferPlaced.RemoveAt(0);
                            stats.CheckPointsWaferRemoved.RemoveAt(0);
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        private void OnThroughputValueChanged(double throughput)
        {
            ThroughputValueChanged?.Invoke(this, new ThroughputEventArgs(throughput));
        }

        private DriveableProcessModule GetProcessModule(ActorType actor)
        {
            var processModule = _controller.AllDevices<DriveableProcessModule>()
                                            .FirstOrDefault(pm => pm.ActorType == actor);

            if (processModule == null)
            {
                throw new InvalidOperationException(
                    $"Process module of type {actor} not found in equipment tree");
            }

            return processModule;
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            _deviceStatistics.Clear();
            _deviceStatistics = new List<DeviceStatistics>();
            foreach (var processModule in _processModules)
            {
                _deviceStatistics.Add(new DeviceStatistics(processModule));
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var processModule in _processModules)
            {
                processModule.Location.PropertyChanged -= ProcessModuleLocation_PropertyChanged;
            }

            _dataFlowManager.ProcessModuleRecipeStarted -=
                DataFlowManager_ProcessModuleRecipeStarted;
            _dataFlowManager.ProcessModuleRecipeCompleted -=
                DataFlowManager_ProcessModuleRecipeCompleted;
        }

        #endregion
    }
}
