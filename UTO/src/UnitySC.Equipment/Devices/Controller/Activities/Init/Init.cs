using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    /// <summary>
    /// Manages events to send to Initialization activity. Supported Commands are:
    /// <list type="bullet">
    /// <item>Start the Activity</item> <item>Abort the Activity</item>
    /// </list>
    /// </summary>
    internal partial class Init : MachineActivity
    {
        #region Variables

        private readonly bool _isColdInit;
        private readonly Abstractions.Devices.Efem.Efem _efem;
        private readonly Abstractions.Devices.Robot.Robot _robot;
        private readonly Abstractions.Devices.Aligner.Aligner _aligner;
        private readonly Abstractions.Devices.Ffu.Ffu _ffu;
        private readonly Abstractions.Devices.LightTower.LightTower _lightTower;
        private readonly AbstractDataFlowManager _dataFlowManager;
        private bool _allDeviceIdleLock;
        private DateTime _allDeviceIdleStarTime;

        #endregion Variables

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="Init" /> class.</summary>
        /// <param name="isColdInit">Defines whether initialization of devices should be forced.</param>
        /// <param name="controller">The controller device instance.</param>
        public Init(bool isColdInit, Controller controller)
            : base(nameof(Init), controller)
        {
            // Store necessary parameters
            _isColdInit = isColdInit;
            _efem = Controller.TryGetDevice<Abstractions.Devices.Efem.Efem>();
            _aligner = _efem.TryGetDevice<Abstractions.Devices.Aligner.Aligner>();
            _robot = _efem.TryGetDevice<Abstractions.Devices.Robot.Robot>();
            _ffu = Controller.TryGetDevice<Abstractions.Devices.Ffu.Ffu>();
            _lightTower = Controller.TryGetDevice<Abstractions.Devices.LightTower.LightTower>();
            _dataFlowManager = Controller.TryGetDevice<AbstractDataFlowManager>();

            // Create the state machine
            CreateStateMachine();
            StateMachine = m_Init;
        }

        #endregion Constructor

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing">
        /// indicates whether the method call comes from a Dispose method (its value is true) or from a
        /// destructor (its value is false).
        /// </param>
        protected override void Dispose(bool disposing)
        {
            foreach (var devices in Controller.GetEquipment()
                         .AllDevices<UnityCommunicatingDevice>())
            {
                devices.StatusValueChanged -= Devices_StatusValueChanged;
            }

            _aligner.CommandExecutionStateChanged -= Devices_CommandExecutionStateChanged;

            if (_dataFlowManager != null)
            {
                _dataFlowManager.CommandExecutionStateChanged -=
                    Devices_CommandExecutionStateChanged;
            }

            foreach (var loadPort in _efem.AllDevices<Abstractions.Devices.LoadPort.LoadPort>())
            {
                loadPort.CommandExecutionStateChanged -= Devices_CommandExecutionStateChanged;
            }

            foreach (var pm in Controller
                         .AllDevices<Abstractions.Devices.ProcessModule.ProcessModule>())
            {
                pm.CommandExecutionStateChanged -= Devices_CommandExecutionStateChanged;
            }

            foreach (var substrateIdReader in _efem
                         .AllDevices<Abstractions.Devices.SubstrateIdReader.SubstrateIdReader>())
            {
                substrateIdReader.CommandExecutionStateChanged -=
                    Devices_CommandExecutionStateChanged;
            }

            _ffu.CommandExecutionStateChanged -= Devices_CommandExecutionStateChanged;

            m_Init.Dispose();
            base.Dispose(disposing);
        }

        #endregion IDisposable

        #region Overrides

        protected override ErrorDescription StartFinalizer()
        {
            _aligner.CommandExecutionStateChanged += Devices_CommandExecutionStateChanged;

            if (_dataFlowManager != null)
            {
                _dataFlowManager.CommandExecutionStateChanged +=
                    Devices_CommandExecutionStateChanged;
            }

            foreach (var loadPort in _efem.AllDevices<Abstractions.Devices.LoadPort.LoadPort>())
            {
                loadPort.CommandExecutionStateChanged += Devices_CommandExecutionStateChanged;
            }

            foreach (var pm in Controller
                         .AllDevices<Abstractions.Devices.ProcessModule.ProcessModule>())
            {
                pm.CommandExecutionStateChanged += Devices_CommandExecutionStateChanged;
            }

            foreach (var substrateIdReader in _efem
                         .AllDevices<Abstractions.Devices.SubstrateIdReader.SubstrateIdReader>())
            {
                substrateIdReader.CommandExecutionStateChanged +=
                    Devices_CommandExecutionStateChanged;
            }

            _ffu.CommandExecutionStateChanged += Devices_CommandExecutionStateChanged;

            return new ErrorDescription();
        }

        public override ErrorDescription Abort()
        {
            //Abort all devices involved in the activity
            _robot.InterruptAsync(InterruptionKind.Abort);
            _aligner.InterruptAsync(InterruptionKind.Abort);

            _dataFlowManager?.InterruptAsync(InterruptionKind.Abort);

            foreach (var pm in Controller
                         .AllDevices<Abstractions.Devices.ProcessModule.ProcessModule>())
            {
                pm.InterruptAsync(InterruptionKind.Abort);
            }

            foreach (var loadPort in _efem.AllDevices<Abstractions.Devices.LoadPort.LoadPort>())
            {
                loadPort.InterruptAsync(InterruptionKind.Abort);
            }

            foreach (var substrateIdReader in _efem
                         .AllDevices<Abstractions.Devices.SubstrateIdReader.SubstrateIdReader>())
            {
                substrateIdReader.InterruptAsync(InterruptionKind.Abort);
            }

            _ffu.InterruptAsync(InterruptionKind.Abort);

            return base.Abort();
        }

        protected override void ActivityExit(Event ev)
        {
            base.ActivityExit(ev);

            _timer?.Dispose();

            //Do not try to refresh arm extended interlocks because it will raise an exception if we are aborting devices
            if (ev is ActivityDoneEvent doneEvent && doneEvent.Status != CommandStatusCode.Ok)
            {
                return;
            }

            try
            {
                _efem.RefreshArmExtendedInterlocks(_robot);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion Overrides

        #region Handlers

        private void Devices_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(IUnityCommunicatingDevice.IsCommunicating))
            {
                PostEvent(new AnyConnectionStateChanged());
            }
        }

        private void Devices_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (e.NewState == ExecutionState.Success)
            {
                PostEvent(
                    new AnyCommandDone()); // required to synchronize all devices initialization end
            }

            switch (e.Execution.Context.Command.Name)
            {
                case nameof(IGenericDevice.Initialize) when e.NewState == ExecutionState.Failed:
                    PostEvent(
                        ActivityDoneEvent.OnException(
                            e.ExceptionThrown,
                            $"{e.Execution.Context.Command.Name} command failed on {e.Execution.Context.Device.Name}"));
                    break;
            }
        }

        #endregion Handlers

        #region Action State Machine

        private void ConnectAllEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    // Connect all devices to hardware
                    foreach (var devices in GetCommunicatingDevices())
                    {
                        devices.StatusValueChanged += Devices_StatusValueChanged;
                        if (!devices.IsCommunicationStarted)
                        {
                            devices.StartCommunication();
                        }
                    }
                },
                new ConnectAllStarted());
        }

        private void CheckAllDevicesConnectedEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    var allConnected = GetCommunicatingDevices().All(d => d.IsCommunicating);
                    if (allConnected)
                    {
                        PostEvent(new AllDevicesConnected());
                    }
                });
        }

        private void InitializeDiosEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    foreach (var dio in Controller.AllDevices<GenericRC5xx>())
                    {
                        dio.Initialize(_isColdInit);
                    }

                    foreach (var dio in Controller.AllDevices<EK9000>())
                    {
                        dio.Initialize(_isColdInit);
                    }
                },
                new DiosInitialized());
        }

        private void InitLightTowerEntry(Event ev)
        {
            PerformAction(() => { _lightTower.Initialize(_isColdInit); }, new LightTowerDone());
        }

        private void QuickInitRobotEntry(Event ev)
        {
            try
            {
                _robot.Initialize(false);
                PostEvent(new RobotDone());
            }
            catch
            {
                PostEvent(new RobotFailed());
            }
        }

        private void InitRobotEntry(Event ev)
        {
            PerformAction(() => { _robot.Initialize(true); }, new RobotDone());
        }

        private void StartInitAlignerEntry(Event ev)
        {
            PerformAction(() => { _aligner.InitializeAsync(_isColdInit); }, new AlignerStarted());
        }

        private void StartInitLoadPortsIfNeededEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    foreach (var loadPort in _efem
                                 .AllDevices<Abstractions.Devices.LoadPort.LoadPort>())
                    {
                        if (loadPort.IsInService)
                        {
                            loadPort.InitializeAsync(_isColdInit);
                        }
                    }
                },
                new AllLoadPortsStarted());
        }

        private void StartInitDataFlowManagerEntry(Event ev)
        {
            PerformAction(
                () => _dataFlowManager?.InitializeAsync(_isColdInit),
                new DataFlowManagerStarted());
        }

        private void StartInitProcessModulesIfNeededEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    foreach (var processModule in Controller
                                 .AllDevices<Abstractions.Devices.ProcessModule.ProcessModule>())
                    {
                        if (!processModule.IsOutOfService)
                        {
                            processModule.InitializeAsync(_isColdInit);
                        }
                    }
                },
                new AllProcessModulesStarted());
        }

        private void StartInitSubstrateIdReadersIfNeededEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    foreach (var substrateIdReader in _efem
                                 .AllDevices<Abstractions.Devices.SubstrateIdReader.
                                     SubstrateIdReader>())
                    {
                        var readerPositioner =
                            substrateIdReader
                                .TryGetDevice<
                                    Abstractions.Devices.ReaderPositioner.ReaderPositioner>();
                        if (substrateIdReader.State != OperatingModes.Idle
                            || _isColdInit
                            || (readerPositioner != null
                                && readerPositioner.State != OperatingModes.Idle))
                        {
                            substrateIdReader.InitializeAsync(_isColdInit);
                        }
                    }
                },
                new AllSubstrateIdReaders());
        }

        private void StartInitFfuEntry(Event ev)
        {
            PerformAction(() => { _ffu.InitializeAsync(_isColdInit); }, new FfuInitStarted());
        }

        private Timer _timer;

        private void SynchronizeDevicesEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    _timer ??= new Timer(CheckAllDeviceStateAction, null, 100, Timeout.Infinite);
                });
        }

        private void CheckAllDeviceStateAction(object _)
        {
            if (GetGenericDevices()
                .All(d => d.State == OperatingModes.Idle && string.IsNullOrEmpty(d.CurrentCommand)))
            {
                if (!_allDeviceIdleLock)
                {
                    _allDeviceIdleStarTime = DateTime.Now;
                }

                _allDeviceIdleLock = true;
            }
            else
            {
                _allDeviceIdleLock = false;
            }

            if (_allDeviceIdleLock && (DateTime.Now - _allDeviceIdleStarTime).Seconds >= 3)
            {
                PostEvent(new AllDevicesInitialized());
            }
            else
            {
                _timer.Change(100, Timeout.Infinite);
            }
        }

        #endregion Action State Machine

        #region Conditions

        private bool LightTowerInitRequired(Event ev)
        {
            return true;
        }

        private bool LightTowerInitNotRequired(Event ev)
        {
            return !LightTowerInitRequired(ev);
        }

        private bool RobotQuickInitRequired(Event ev)
        {
            return _robot.State == OperatingModes.Idle && !_isColdInit;
        }

        private bool RobotCompleteInitRequired(Event ev)
        {
            return !RobotQuickInitRequired(ev);
        }

        private bool FfuInitRequired(Event ev)
        {
            if (_ffu == null)
            {
                return false;
            }

            //FFU Init is always required if FFU is available
            return true;
        }

        private bool FfuInitNotRequired(Event ev)
        {
            return !FfuInitRequired(ev);
        }

        #endregion Conditions

        #region Private

        private IEnumerable<UnityCommunicatingDevice> GetCommunicatingDevices()
        {
            return Controller.GetEquipment()
                .AllDevices<UnityCommunicatingDevice>()
                .Except(GetOutOfServiceLoadPorts())
                .Except(GetOutOfServicePms());
        }

        private IEnumerable<GenericDevice> GetGenericDevices()
        {
            return Controller.AllDevices<GenericDevice>()
                .Except(GetOutOfServiceLoadPorts())
                .Except(GetOutOfServicePms());
        }

        private IEnumerable<Abstractions.Devices.LoadPort.LoadPort> GetOutOfServiceLoadPorts()
        {
            var loadPorts = Controller.AllDevices<Abstractions.Devices.LoadPort.LoadPort>();
            return loadPorts.Where(lp => !lp.IsInService);
        }

        private IEnumerable<Abstractions.Devices.ProcessModule.ProcessModule> GetOutOfServicePms()
        {
            var processModules = Controller
                .AllDevices<Abstractions.Devices.ProcessModule.ProcessModule>();
            return processModules.Where(lp => lp.IsOutOfService);
        }

        #endregion
    }
}
