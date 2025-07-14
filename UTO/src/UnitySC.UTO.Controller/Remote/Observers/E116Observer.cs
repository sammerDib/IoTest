using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E116;

using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.UTO.Controller.Remote.Services;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class E116Observer : E30StandardSupport
    {
        #region Fields

        private readonly IE116Standard _e116Standard;

        //Equipment
        private Equipment.Devices.Controller.Controller _controller;

        //Modules
        private Robot _robot;
        private Aligner _aligner;
        private List<LoadPort> _loadPorts;
        private List<SubstrateIdReader> _substrateIdReaders;
        private List<DriveableProcessModule> _processModules;

        #endregion

        #region Constructor

        public E116Observer(IE116Standard e116Standard, IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
            _e116Standard = e116Standard;
        }

        #endregion

        #region Overrides

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            _controller = Equipment.AllOfType<Equipment.Devices.Controller.Controller>().First();

            var efem = _controller.TryGetDevice<Efem>();

            _robot = efem.TryGetDevice<Robot>();
            _aligner = efem.TryGetDevice<Aligner>();
            _loadPorts = efem.AllDevices<LoadPort>().ToList();
            _substrateIdReaders = efem.AllDevices<SubstrateIdReader>().ToList();
            _processModules = _controller.AllDevices<DriveableProcessModule>().ToList();

            _robot.CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;
            _aligner.CommandExecutionStateChanged += Aligner_CommandExecutionStateChanged;

            foreach (var loadPort in _loadPorts)
            {
                loadPort.CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
            }

            foreach (var substrateIdReader in _substrateIdReaders)
            {
                substrateIdReader.CommandExecutionStateChanged +=
                    SubstrateIdReader_CommandExecutionStateChanged;
            }

            foreach (var processModule in _processModules)
            {
                processModule.CommandExecutionStateChanged +=
                    ProcessModule_CommandExecutionStateChanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _robot.CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;
                _aligner.CommandExecutionStateChanged -= Aligner_CommandExecutionStateChanged;

                foreach (var loadPort in _loadPorts)
                {
                    loadPort.CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                }

                foreach (var substrateIdReader in _substrateIdReaders)
                {
                    substrateIdReader.CommandExecutionStateChanged -=
                        SubstrateIdReader_CommandExecutionStateChanged;
                }

                foreach (var processModule in _processModules)
                {
                    processModule.CommandExecutionStateChanged -=
                        ProcessModule_CommandExecutionStateChanged;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        #region Robot

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (sender is not Robot robot)
            {
                return;
            }

            switch (e.NewState)
            {
                case ExecutionState.Running:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                        robot.Name,
                        GetRobotTaskType(e.Execution.Context.Command.Name),
                        e.Execution.Context.Command.Name);
                    break;
                case ExecutionState.Success:
                    if (robot.LowerArmWaferPresence == WaferPresence.Present
                        || robot.UpperArmWaferPresence == WaferPresence.Present)
                    {
                        _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                            robot.Name,
                            TaskType.Waiting,
                            "Wait orders (Material present).");
                    }
                    else
                    {
                        _e116Standard.IntegrationServices.NotifyModuleTrackerIdle(robot.Name);
                    }

                    break;
                case ExecutionState.Failed:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBlocked(
                        robot.Name,
                        BlockedReason.ErrorCondition,
                        e.ExceptionThrown.Message);
                    break;
            }
        }

        private TaskType GetRobotTaskType(string commandName)
        {
            switch (commandName)
            {
                case nameof(IRobot.Initialize):
                    return TaskType.EquipmentMaintenance;

                case nameof(IRobot.Pick):
                case nameof(IRobot.Place):
                case nameof(IRobot.GoToLocation):
                case nameof(IRobot.GoToHome):
                case nameof(IRobot.GoToSpecifiedLocation):
                    return TaskType.Support;

                default:
                    return TaskType.Unspecified;
            }
        }

        #endregion

        #region Aligner

        private void Aligner_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not Aligner aligner)
            {
                return;
            }

            switch (e.NewState)
            {
                case ExecutionState.Running:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                        aligner.Name,
                        GetAlignerTaskType(e.Execution.Context.Command.Name),
                        e.Execution.Context.Command.Name);
                    break;
                case ExecutionState.Success:
                    if (aligner.WaferPresence == WaferPresence.Present)
                    {
                        _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                            aligner.Name,
                            TaskType.Waiting,
                            "Wait orders (Material present).");
                    }
                    else
                    {
                        _e116Standard.IntegrationServices.NotifyModuleTrackerIdle(aligner.Name);
                    }

                    break;
                case ExecutionState.Failed:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBlocked(
                        aligner.Name,
                        BlockedReason.ErrorCondition,
                        e.ExceptionThrown.Message);
                    break;
            }
        }

        private TaskType GetAlignerTaskType(string commandName)
        {
            switch (commandName)
            {
                case nameof(IAligner.Initialize):
                    return TaskType.EquipmentMaintenance;

                case nameof(IAligner.Align):
                case nameof(IAligner.Clamp):
                case nameof(IAligner.Unclamp):
                case nameof(IAligner.PrepareTransfer):
                    return TaskType.Support;

                default:
                    return TaskType.Unspecified;
            }
        }

        #endregion

        #region Loadport

        private void LoadPort_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            switch (e.NewState)
            {
                case ExecutionState.Running:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                        loadPort.Name,
                        GetLoadPortTaskType(e.Execution.Context.Command.Name),
                        e.Execution.Context.Command.Name);
                    break;
                case ExecutionState.Success:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerIdle(loadPort.Name);
                    break;
                case ExecutionState.Failed:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBlocked(
                        loadPort.Name,
                        BlockedReason.ErrorCondition,
                        e.ExceptionThrown.Message);
                    break;
            }
        }

        private TaskType GetLoadPortTaskType(string commandName)
        {
            switch (commandName)
            {
                case nameof(ILoadPort.Initialize):
                    return TaskType.EquipmentMaintenance;

                default:
                    return TaskType.Unspecified;
            }
        }

        #endregion

        #region SubstrateIdReader

        private void SubstrateIdReader_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not SubstrateIdReader reader)
            {
                return;
            }

            switch (e.NewState)
            {
                case ExecutionState.Running:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                        reader.Name,
                        GetReaderTaskType(e.Execution.Context.Command.Name),
                        e.Execution.Context.Command.Name);
                    break;
                case ExecutionState.Success:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerIdle(reader.Name);
                    break;
                case ExecutionState.Failed:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBlocked(
                        reader.Name,
                        BlockedReason.ErrorCondition,
                        e.ExceptionThrown.Message);
                    break;
            }
        }

        private TaskType GetReaderTaskType(string commandName)
        {
            switch (commandName)
            {
                case nameof(ISubstrateIdReader.Initialize):
                    return TaskType.EquipmentMaintenance;

                default:
                    return TaskType.Unspecified;
            }
        }

        #endregion

        #region ProcessModule

        private void ProcessModule_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not DriveableProcessModule processModule)
            {
                return;
            }

            switch (e.NewState)
            {
                case ExecutionState.Running:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                        processModule.Name,
                        GetProcessModuleTaskType(e.Execution.Context.Command.Name),
                        e.Execution.Context.Command.Name);
                    break;
                case ExecutionState.Success:
                    if (processModule.WaferPresence == WaferPresence.Present)
                    {
                        _e116Standard.IntegrationServices.NotifyModuleTrackerBusy(
                            processModule.Name,
                            TaskType.Waiting,
                            "Wait orders (Material present).");
                    }
                    else
                    {
                        _e116Standard.IntegrationServices.NotifyModuleTrackerIdle(
                            processModule.Name);
                    }

                    break;
                case ExecutionState.Failed:
                    _e116Standard.IntegrationServices.NotifyModuleTrackerBlocked(
                        processModule.Name,
                        BlockedReason.ErrorCondition,
                        e.ExceptionThrown.Message);
                    break;
            }
        }

        private TaskType GetProcessModuleTaskType(string commandName)
        {
            switch (commandName)
            {
                case nameof(IDriveableProcessModule.Initialize):
                    return TaskType.EquipmentMaintenance;

                case nameof(IDriveableProcessModule.PrepareTransfer):
                case nameof(IDriveableProcessModule.PostTransfer):
                    return TaskType.Support;

                case nameof(IDriveableProcessModule.StartRecipe):
                case nameof(IDriveableProcessModule.AbortRecipe):
                    return TaskType.Process;

                default:
                    return TaskType.Unspecified;
            }
        }

        #endregion

        #endregion
    }
}
