using System.Windows;

using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation.Alarms;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation
{
    public abstract class SimulationData : Notifier

    {
        #region Fields

        protected readonly GenericDevice.GenericDevice _device;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether current simulated command must fail or succeed.
        /// </summary>
        public bool
            IsCommandExecutionFailed
        {
            get;
            set;
        }

        public bool IsDeviceWithAlarms => AlarmSimulationUserControl != null;

        public AlarmSimulationUserControl AlarmSimulationUserControl { get; private set; }

        #endregion Properties

        #region Constructor

        /// <summary>
        ///     Only use in design mode
        /// </summary>
        protected SimulationData()
        {
        }

        protected SimulationData(GenericDevice.GenericDevice device)
        {
            _device = device;
            if (_device.Alarms.Count > 0)
            {
                Application.Current.Dispatcher.Invoke(
                    () => AlarmSimulationUserControl = new AlarmSimulationUserControl { DataContext = new AlarmSimulationViewModel(_device) });
            }
        }

        #endregion
    }
}
