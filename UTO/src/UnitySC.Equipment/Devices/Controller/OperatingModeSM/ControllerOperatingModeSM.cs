using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Devices.Controller.OperatingModeSM
{
    internal partial class ControllerOperatingModeSm : IUserInformationProvider
    {
        #region Fields

        private readonly Controller _controller;

        private readonly Abstractions.Devices.Efem.Efem _efem;

        private readonly Abstractions.Devices.Robot.Robot _robot;

        private readonly Abstractions.Devices.Aligner.Aligner _aligner;

        private readonly List<DriveableProcessModule> _processModules;

        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public ControllerOperatingModeSm(Controller controller)
            : this()
        {
            _logger = Logger.GetLogger($"{nameof(ControllerOperatingModeSm)}");

            _controller = controller;
            _processModules = _controller.AllDevices<DriveableProcessModule>().ToList();
            _efem = _controller.TryGetDevice<Abstractions.Devices.Efem.Efem>();

            _robot = _efem.TryGetDevice<Abstractions.Devices.Robot.Robot>();
            _aligner = _efem.TryGetDevice<Abstractions.Devices.Aligner.Aligner>();

            _controller.CommandExecutionStateChanged += Controller_CommandExecutionStateChanged;
            _controller.JobStatusChanged += Controller_JobStatusChanged;
            _controller.StatusValueChanged += Controller_StatusValueChanged;

            foreach (var device in _controller.GetDevices<GenericDevice>().ToList())
            {
                device.StatusValueChanged += GenericDevice_StatusValueChanged;
            }

            foreach (var loadPort in _controller.AllOfType<LoadPort>().ToList())
            {
                loadPort.StatusValueChanged += LoadPort_StatusValueChanged;
            }
        }

        ~ControllerOperatingModeSm() // finalizer is used because Dispose method is not overridable
        {
            if (_controller != null)
            {
                _controller.CommandExecutionStateChanged -= Controller_CommandExecutionStateChanged;
                _controller.JobStatusChanged -= Controller_JobStatusChanged;
                _controller.StatusValueChanged -= Controller_StatusValueChanged;
            }

            foreach (var device in _controller.GetDevices<GenericDevice>().ToList())
            {
                device.StatusValueChanged -= GenericDevice_StatusValueChanged;
            }

            foreach (var loadPort in _controller.AllOfType<LoadPort>().ToList())
            {
                loadPort.StatusValueChanged -= LoadPort_StatusValueChanged;
            }
        }

        #endregion

        #region StateMachine Conditions

        private bool SubDevicesIdle()
        {
            var result = _efem.State == OperatingModes.Idle
                         && _processModules.All(
                             x => x.ProcessModuleState == ProcessModuleState.Idle);
            _logger.Debug($"{nameof(SubDevicesIdle)} is called: Result => {result}");
            return result;
        }

        private bool SubDevicesIdleAndNoWaferPresent(Event _)
        {
            var result = SubDevicesIdle()
                         && !AnyWaferPresent()
                         && !AnyWaferIncoherence()
                         && !AllLpOutOfService()
                         && !AllPmOutOfService();
            _logger.Debug(
                $"{nameof(SubDevicesIdleAndNoWaferPresent)} is called: Result => {result}");
            return result;
        }

        private bool SubDevicesNotIdleOrWaferPresentOrWaferIncoherence(Event _)
        {
            var result = !SubDevicesIdle()
                         || AnyWaferPresent()
                         || AnyWaferIncoherence()
                         || AllLpOutOfService()
                         || AllPmOutOfService();
            _logger.Debug(
                $"{nameof(SubDevicesNotIdleOrWaferPresentOrWaferIncoherence)} is called: Result => {result}");
            return result;
        }

        private bool EngineeringModeAllowed(Event _)
        {
            var result = SubDevicesIdle()
                         && !AnyWaferPresentExceptPm()
                         && !AnyWaferIncoherence()
                         && !AllLpOutOfService()
                         && !AllPmOutOfService();
            _logger.Debug(
                $"{nameof(SubDevicesIdleAndNoWaferPresent)} is called: Result => {result}");
            return result;
        }
        #endregion

        #region Event Handlers

        private void Controller_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (!e.Status.Name.Equals(nameof(IGenericDevice.State))
                || e.NewValue is not OperatingModes.Maintenance)
            {
                return;
            }

            _logger.Debug(
                $"{nameof(Controller_StatusValueChanged)}: {nameof(MaintenanceRequested)} is called");
            PostEvent(new MaintenanceRequested(TraceLevelType.Error, GetErrorMessage(false)));
        }

        private void Controller_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            switch (e.Execution.Context.Command.Name)
            {
                case nameof(IController.Initialize):
                    switch (e.NewState)
                    {
                        case ExecutionState.Running:
                            _logger.Debug(
                                $"{nameof(Controller_CommandExecutionStateChanged)}: {nameof(InitRequested)} is called");
                            PostEvent(new InitRequested());
                            break;

                        case ExecutionState.Failed:
                            _logger.Debug(
                                $"{nameof(Controller_CommandExecutionStateChanged)}: {nameof(InitFailed)} is called");
                            PostEvent(new InitFailed());
                            break;

                        case ExecutionState.Success:
                            _logger.Debug(
                                $"{nameof(Controller_CommandExecutionStateChanged)}: {nameof(InitCompleted)} is called");
                            PostEvent(new InitCompleted());
                            break;
                    }

                    break;

                case nameof(IController.StartJobExecution):
                    switch (e.NewState)
                    {
                        case ExecutionState.Success:
                            _logger.Debug(
                                $"{nameof(Controller_CommandExecutionStateChanged)}: {nameof(JobExecutionStarted)} is called");
                            PostEvent(new JobExecutionStarted());
                            break;

                        case ExecutionState.Failed:
                            _logger.Debug(
                                $"{nameof(Controller_CommandExecutionStateChanged)}: {nameof(MaintenanceRequested)} is called");
                            PostEvent(
                                new MaintenanceRequested(
                                    TraceLevelType.Error,
                                    GetErrorMessage(false)));
                            break;
                    }

                    break;
            }
        }

        private void Controller_JobStatusChanged(
            object sender,
            EventArgs.JobStatusChangedEventArgs e)
        {
            switch (e.Job.Status)
            {
                case JobStatus.Completed:
                case JobStatus.Stopped:
                    if (_controller.Jobs.Count > 1)
                    {
                        return;
                    }

                    _logger.Debug(
                        $"{nameof(Controller_JobStatusChanged)}: {nameof(JobExecutionEnded)} is called");
                    PostEvent(new JobExecutionEnded());
                    break;
                case JobStatus.Failed:
                    _logger.Debug(
                        $"{nameof(Controller_JobStatusChanged)}: {nameof(MaintenanceRequested)} is called");
                    PostEvent(
                        new MaintenanceRequested(TraceLevelType.Error, GetErrorMessage(false)));
                    break;
            }
        }

        private void GenericDevice_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not GenericDevice device)
            {
                return;
            }

            if (sender is IDriveableProcessModule pm)
            {
                CheckProcessModuleState(pm, e);
            }

            CheckOperatingModes(device, e);
        }

        private void LoadPort_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            if (e.Status.Name != nameof(ILoadPort.State))
            {
                return;
            }

            if (loadPort.State != OperatingModes.Maintenance)
            {
                return;
            }

            if (!loadPort.IsInService
                || (loadPort.AccessMode == LoadingType.Auto
                    && loadPort.CurrentE84Error != E84Errors.None))
            {
                if (AllLpOutOfService())
                {
                    _logger.Debug(
                        $"{nameof(LoadPort_StatusValueChanged)}: {nameof(MaintenanceRequested)} is called");
                    PostEvent(
                        new MaintenanceRequested(TraceLevelType.Error, GetErrorMessage(false)));
                }

                return;
            }

            if (_controller.GetSubstrates().All(w => w.SourcePort != loadPort.InstanceId)
                || _controller.Jobs.Count == 0
                || !_controller.Jobs.Any(
                    j => j.RemainingWafers != null
                         && j.RemainingWafers.Count > 0
                         && j.RemainingWafers.Any(w => w.SourcePort == loadPort.InstanceId)))
            {
                return;
            }

            _logger.Debug(
                $"{nameof(LoadPort_StatusValueChanged)}: {nameof(MaintenanceRequested)} is called");
            PostEvent(new MaintenanceRequested(TraceLevelType.Error, GetErrorMessage(false)));
        }

        #endregion

        #region Public Methods

        public void RequestManualMode()
        {
            _logger.Debug($"{nameof(RequestManualMode)}: {nameof(MaintenanceRequested)} is called");
            PostEvent(new MaintenanceRequested(TraceLevelType.Warning, GetErrorMessage(true)));
        }

        public void RequestEngineeringMode()
        {
            _logger.Debug(
                $"{nameof(RequestEngineeringMode)}: {nameof(EngineeringRequested)} is called");
            PostEvent(new EngineeringRequested());
        }

        #endregion

        #region Private Methods

        private void CheckOperatingModes(GenericDevice device, StatusChangedEventArgs e)
        {
            if (e.Status.Name != nameof(IGenericDevice.State))
            {
                return;
            }

            switch ((OperatingModes)e.NewValue)
            {
                case OperatingModes.Maintenance:
                    if (device.PreviousState != OperatingModes.Initialization)
                    {
                        if (CheckAllDeviceStateOfSameType(device))
                        {
                            return;
                        }

                        _logger.Debug(
                            $"{nameof(CheckOperatingModes)}: {nameof(MaintenanceRequested)} is called");
                        PostEvent(
                            new MaintenanceRequested(TraceLevelType.Error, GetErrorMessage(false)));
                    }

                    break;
            }
        }

        private bool CheckAllDeviceStateOfSameType(GenericDevice device)
        {
            switch (device)
            {
                case LoadPort:
                    return _controller.AllOfType<LoadPort>()
                        .Any(d => d.State != OperatingModes.Maintenance);
                case Abstractions.Devices.Aligner.Aligner:
                    return _controller.AllOfType<Abstractions.Devices.Aligner.Aligner>()
                        .Any(d => d.State != OperatingModes.Maintenance);
                case Abstractions.Devices.ProcessModule.ProcessModule:
                    return _controller.AllOfType<Abstractions.Devices.ProcessModule.ProcessModule>()
                        .Any(d => d.State != OperatingModes.Maintenance);
                default:
                    return false;
            }
        }

        private void CheckProcessModuleState(
            IDriveableProcessModule processModule,
            StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                case nameof(IDriveableProcessModule.ProcessModuleState):
                    switch ((ProcessModuleState)e.NewValue)
                    {
                        case ProcessModuleState.Error:
                        case ProcessModuleState.Unknown:
                        case ProcessModuleState.ShuttingDown:
                            if (processModule.PreviousProcessModuleState
                                == ProcessModuleState.Initializing)
                            {
                                _logger.Debug(
                                    $"{nameof(CheckProcessModuleState)}: {nameof(InitFailed)} is called");
                                PostEvent(new InitFailed());
                            }

                            break;
                    }

                    break;

                case nameof(IDriveableProcessModule.IsOutOfService):

                    if (!processModule.IsOutOfService || _controller.Jobs == null)
                    {
                        return;
                    }

                    if (_controller.Jobs.Any(
                            job => job.Wafers.Any(
                                w => w.JobProgram.PMItems.Any(
                                    item => item.PMType == processModule.ActorType))))
                    {
                        var allPmIsOutOfService = _controller.AllDevices<DriveableProcessModule>()
                                                                    .Where(pm => pm.ActorType == processModule.ActorType)
                                                                    .All(d => d.IsOutOfService);

                        if (allPmIsOutOfService)
                        {
                            _logger.Debug(
                                $"{nameof(LoadPort_StatusValueChanged)}: {nameof(MaintenanceRequested)} is called");
                            PostEvent(
                                new MaintenanceRequested(
                                    TraceLevelType.Error,
                                    ControllerOperatingModeResources
                                        .ERROR_PM_OUT_OF_SERVICE_DURING_JOB));
                        }
                    }

                    break;
            }
        }

        private bool AnyWaferPresentExceptPm()
        {
            return _robot.UpperArmLocation.Substrate != null
                   || _robot.LowerArmLocation.Substrate != null
                   || _aligner.Location.Substrate != null;
        }

        private bool AnyWaferPresent()
        {
            return _processModules.Any(x => x.Location.Substrate != null)
                   || _robot.UpperArmLocation.Substrate != null
                   || _robot.LowerArmLocation.Substrate != null
                   || _aligner.Location.Substrate != null;
        }

        private bool AnyWaferIncoherence()
        {
            return _processModules.Any(
                       x => CheckWaferIncoherence(x.Location.Substrate, x.WaferPresence))
                   || CheckWaferIncoherence(
                       _robot.UpperArmLocation.Substrate,
                       _robot.UpperArmWaferPresence)
                   || CheckWaferIncoherence(
                       _robot.LowerArmLocation.Substrate,
                       _robot.LowerArmWaferPresence)
                   || CheckWaferIncoherence(_aligner.Location.Substrate, _aligner.WaferPresence);
        }

        private bool CheckWaferIncoherence(Substrate substrate, WaferPresence waferPresence)
        {
            return (waferPresence == WaferPresence.Present && substrate == null)
                   || (waferPresence == WaferPresence.Absent && substrate != null);
        }

        private bool AllLpOutOfService()
        {
            return _controller.AllOfType<LoadPort>().All(lp => !lp.IsInService);
        }

        private bool AllPmOutOfService()
        {
            return _controller.AllOfType<Abstractions.Devices.ProcessModule.ProcessModule>()
                .All(pm => pm.IsOutOfService);
        }

        private string GetErrorMessage(bool manualModeRequested)
        {
            if (manualModeRequested)
            {
                return LocalizationManager.GetString(
                    ControllerOperatingModeResources.ERROR_MANUAL_MODE_REQUESTED);
            }

            if (AllPmOutOfService())
            {
                return LocalizationManager.GetString(
                    ControllerOperatingModeResources.ERROR_ALL_PM_OUT_OF_SERVICE);
            }

            if (AllLpOutOfService())
            {
                return LocalizationManager.GetString(
                    ControllerOperatingModeResources.ERROR_ALL_LP_OUT_OF_SERVICE);
            }

            var disconnectedDevices = _controller.AllDevices<UnityCommunicatingDevice>()
                .Where(d => !d.IsCommunicating)
                .ToList();

            if (disconnectedDevices.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append(
                    LocalizationManager.GetString(
                        ControllerOperatingModeResources.ERROR_DEVICE_NOT_COMMUNICATING));
                foreach (var device in disconnectedDevices)
                {
                    sb.Append(device.Name);
                    if (device != disconnectedDevices.Last())
                    {
                        sb.Append(", ");
                    }
                }

                return sb.ToString();
            }

            var maintenanceDevices = _controller.AllDevices<GenericDevice>()
                .Where(d => d.State == OperatingModes.Maintenance)
                .ToList();
            if (maintenanceDevices.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append(
                    LocalizationManager.GetString(
                        ControllerOperatingModeResources.ERROR_DEVICE_IN_MAINTENANCE));
                foreach (var device in maintenanceDevices)
                {
                    sb.Append(device.Name);
                    if (device != maintenanceDevices.Last())
                    {
                        sb.Append(", ");
                    }
                }

                return sb.ToString();
            }

            if (_controller.GetSubstrates().Any())
            {
                return LocalizationManager.GetString(
                    ControllerOperatingModeResources.ERROR_WAFER_PRESENT);
            }

            return LocalizationManager.GetString(ControllerOperatingModeResources.ERROR_OCCURRED);
        }

        #endregion

        #region Actions

        private void MaintenanceEntryAction(Event ev)
        {
            if (ev is MaintenanceRequested maintenanceRequested)
            {
                switch (maintenanceRequested.Level)
                {
                    case TraceLevelType.Info:
                    case TraceLevelType.Debug:
                        OnUserInformationRaised(maintenanceRequested.Message);
                        break;
                    case TraceLevelType.Warning:
                        OnUserWarningRaised(maintenanceRequested.Message);
                        break;
                    case TraceLevelType.Error:
                    case TraceLevelType.Fatal:
                        OnUserErrorRaised(maintenanceRequested.Message);
                        break;
                }
            }
            else
            {
                //Init failed
                OnUserErrorRaised(GetErrorMessage(false));
            }
        }

        #endregion

        #region IUserInformationProvider

        public event EventHandler<UserInformationEventArgs> UserInformationRaised;

        protected void OnUserInformationRaised(string message)
        {
            UserInformationRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        public event EventHandler<UserInformationEventArgs> UserWarningRaised;

        protected void OnUserWarningRaised(string message)
        {
            UserWarningRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        public event EventHandler<UserInformationEventArgs> UserErrorRaised;

        protected void OnUserErrorRaised(string message)
        {
            UserErrorRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        #endregion
    }
}
