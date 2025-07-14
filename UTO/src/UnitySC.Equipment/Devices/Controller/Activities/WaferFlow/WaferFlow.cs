using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;
using UnitySC.Equipment.Devices.Controller.EventArgs;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule;

using Job = UnitySC.Equipment.Devices.Controller.JobDefinition.Job;
using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;

namespace UnitySC.Equipment.Devices.Controller.Activities.WaferFlow
{
    public partial class WaferFlow : WaferMachineActivity.WaferMachineActivity
    {
        #region Fields

        private readonly Abstractions.Devices.Aligner.Aligner _aligner;

        private readonly Abstractions.Devices.Robot.Robot _robot;

        private readonly List<Abstractions.Devices.LoadPort.LoadPort> _loadPorts;

        private DriveableProcessModule _processModule;

        private readonly AbstractDataFlowManager _dataFlowManager;

        private readonly ActivityManager _alignerActivityManager;

        private AlignerActivity.AlignerActivity _alignerActivity;

        private RobotArm _selectedRobotArm;

        private bool _isAborted;

        private object _lockProcessModule = new object();

        #endregion

        #region Properties

        public List<Job> Jobs { get; }
        public bool IsSubstrateReaderEnabled { get; private set; }
        public bool IsSubstrateIdVerificationEnabled { get; private set; }

        #endregion

        #region Constructor

        public WaferFlow(Controller controller)
            : base(nameof(WaferFlow), controller)
        {
            Jobs = new List<Job>();

            _aligner = Efem.TryGetDevice<Abstractions.Devices.Aligner.Aligner>();
            _robot = Efem.TryGetDevice<Abstractions.Devices.Robot.Robot>();
            _loadPorts = Efem.GetDevices<Abstractions.Devices.LoadPort.LoadPort>().ToList();
            _dataFlowManager = Controller.TryGetDevice<AbstractDataFlowManager>();

            _alignerActivityManager = new ActivityManager(Logger);
            _alignerActivityManager.ActivityDone += AlignerActivityManager_ActivityDone;

            if (_dataFlowManager != null)
            {
                _dataFlowManager.ProcessModuleRecipeStarted +=
                    DataFlowManager_ProcessModuleRecipeStarted;
                _dataFlowManager.ProcessModuleAcquisitionCompleted +=
                    DataFlowManager_ProcessModuleAcquisitionCompleted;
                _dataFlowManager.DataFlowRecipeCompleted +=
                    DataFlowManager_DataFlowRecipeCompleted;
            }

            CreateStateMachine();
            StateMachine = m_WaferFlow;

            foreach (var device in controller.AllDevices<GenericDevice>())
            {
                device.StatusValueChanged += Device_StatusValueChanged;
            }
        }

        #endregion

        #region Public Methods

        public void AddJob(Job job)
        {
            Jobs.Add(job);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(
                @"
[Details]");
            stringBuilder.Append("Job name: ").Append(job.Name).AppendLine();
            stringBuilder.AppendLine("Operations planned:");
            foreach (var substrate in job.Wafers)
            {
                stringBuilder.Append("Source : LoadPort")
                    .Append(substrate.SourcePort)
                    .Append("/Slot")
                    .Append(substrate.SourceSlot)
                    .AppendLine();
            }

            Logger.Info("New job added: " + stringBuilder);
            OnJobStatusChanged(job, JobStatus.Created);
        }

        public void StartJob(Job job)
        {
            Logger.Info($"New job started: {job.ControlJobId}");
            OnJobStatusChanged(job, JobStatus.Queued);
            PostEvent(new Reevaluate());
        }

        public void ProceedWithSubstrate()
        {
            if (_alignerActivityManager.Activity is AlignerActivity.AlignerActivity activity)
            {
                activity.ProceedWithCurrentSubstrate();
            }
        }

        public void CancelSubstrate()
        {
            if (_alignerActivityManager.Activity is AlignerActivity.AlignerActivity activity)
            {
                activity.CancelCurrentSubstrate();
            }
        }

        public void StopJob(string jobName, StopConfig stopConfig)
        {
            if (Jobs.FirstOrDefault(x => x.Name == jobName) is not { } job)
            {
                throw new InvalidOperationException($"Job {jobName} is unknown");
            }

            if (job.Status == JobStatus.Stopping)
            {
                throw new InvalidOperationException("Stop execution already in progress.");
            }

            job.StopConfig = stopConfig;
            if (job.StopConfig == StopConfig.CancelProcess)
            {
                AbortDataFlowRecipe(job);
            }

            OnJobStatusChanged(job, JobStatus.Stopping);
            PostEvent(new StopRequested());
        }

        public void AbortJob(string jobName)
        {
            if (Jobs.FirstOrDefault(j => j.Name == jobName) is not { } job)
            {
                throw new InvalidOperationException($"Job {jobName} is unknown");
            }

            if (!_isAborted)
            {
                _isAborted = true;

                OnJobStatusChanged(job, JobStatus.Failed);

                foreach (var pmItem in job.Wafers.First().JobProgram.PMItems)
                {
                    var processModule = GetProcessModule(pmItem.PMType);
                    processModule.InterruptAsync(InterruptionKind.Abort);
                }

                _robot.InterruptAsync(InterruptionKind.Abort);
                _aligner.InterruptAsync(InterruptionKind.Abort);
                _alignerActivityManager.Abort();
                AbortDataFlowRecipe(job);
                Abort();
            }
        }

        public void Pause(string jobName)
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("Activity not started.");
            }

            if (Jobs.FirstOrDefault(j => j.Name == jobName) is not { } job)
            {
                throw new InvalidOperationException($"Job {jobName} is unknown");
            }

            //If paused already
            if (job.Status is JobStatus.Pausing or JobStatus.Paused)
            {
                throw new InvalidOperationException("Activity already pausing or paused.");
            }

            OnJobStatusChanged(job, JobStatus.Pausing);
            PostEvent(new Reevaluate());
        }

        public void Resume(string jobName)
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("Activity not started.");
            }

            if (Jobs.FirstOrDefault(j => j.Name == jobName) is not { } job)
            {
                throw new InvalidOperationException($"Job {jobName} is unknown");
            }

            if (job.Status is not JobStatus.Paused)
            {
                throw new InvalidOperationException("Activity not paused.");
            }

            OnJobStatusChanged(job, JobStatus.Executing);
            PostEvent(new Reevaluate());
        }

        public void UpdateReaderBehavior(
            bool isSubstrateReaderEnabled,
            bool isSubstrateIdVerificationEnabled)
        {
            IsSubstrateReaderEnabled = isSubstrateReaderEnabled;
            IsSubstrateIdVerificationEnabled = isSubstrateIdVerificationEnabled;
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_dataFlowManager != null)
                {
                    _dataFlowManager.ProcessModuleRecipeStarted -=
                        DataFlowManager_ProcessModuleRecipeStarted;
                    _dataFlowManager.ProcessModuleAcquisitionCompleted -=
                        DataFlowManager_ProcessModuleAcquisitionCompleted;
                    _dataFlowManager.DataFlowRecipeCompleted -=
                        DataFlowManager_DataFlowRecipeCompleted;
                }

                foreach (var device in Controller.AllDevices<GenericDevice>())
                {
                    device.StatusValueChanged -= Device_StatusValueChanged;
                }
            }
        }

        protected override void ActivityExit(Event ev)
        {
            base.ActivityExit(ev);

            if (ev is ActivityDoneEvent doneEvent && doneEvent.Status != CommandStatusCode.Ok)
            {
                foreach (var job in Jobs)
                {
                    AbortJob(job.Name);
                }
            }
            else
            {
                //If activity already ended before it was possible to pause it, set the pause handler
                foreach (var job in Jobs.Where(
                             j => j.Status is JobStatus.Pausing or JobStatus.Paused))
                {
                    OnJobStatusChanged(job, JobStatus.Executing);
                }
            }
        }

        protected override void WaitForResume()
        {
            //Do nothing
        }

        #endregion

        #region Actions

        private void WaitOrdersEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    foreach (var job in Jobs.ToList())
                    {
                        if (job.Status == JobStatus.Pausing)
                        {
                            OnJobStatusChanged(job, JobStatus.Paused);
                        }

                        if (job.IsCompleted() || job.Status == JobStatus.Stopping)
                        {
                            var remainingWaferOnToolCount = SubstratesOnTool.Count(
                                substrate => ((Wafer)substrate).ProcessJobId == job.Name);
                            if (remainingWaferOnToolCount != 0)
                            {
                                continue;
                            }

                            switch (job.Status)
                            {
                                case JobStatus.Executing:
                                    OnJobStatusChanged(job, JobStatus.Completed);
                                    Jobs.RemoveAt(Jobs.FindIndex(j => j.Name == job.Name));
                                    break;
                                case JobStatus.Stopping:
                                    OnJobStatusChanged(job, JobStatus.Stopped);
                                    if (job.StopConfig != StopConfig.CancelProcess)
                                    {
                                        AbortDataFlowRecipe(job);
                                    }

                                    Jobs.RemoveAt(Jobs.FindIndex(j => j.Name == job.Name));
                                    break;
                            }
                        }
                    }

                    IsStopping = Jobs.Any(j => j.Status == JobStatus.Stopping);
                    Pausing = Jobs.Any(j => j.Status == JobStatus.Pausing);
                    Paused = Jobs.Any(j => j.Status == JobStatus.Paused);

                    if (_dataFlowManager is { IsStopCancelAllJobsRequested: true })
                    {
                        //Critical PM alarm 
                        if (IsCriticalPmAlarmUseCase())
                        {
                            _dataFlowManager.ResetIsStopCancelAllJobsRequested();
                            var processModulesInError =
                                Controller.AllDevices<DriveableProcessModule>()
                                    .Where(pm => pm.ProcessModuleState == ProcessModuleState.Error)
                                    .ToList();

                            foreach (var processModule in processModulesInError)
                            {
                                processModule.Interrupt(InterruptionKind.Abort);
                            }
                        }

                        //Major PM alarm 
                        if (Jobs.All(
                                j => j.Status is not JobStatus.Executing
                                    and not JobStatus.Stopping))
                        {
                            _dataFlowManager.ResetIsStopCancelAllJobsRequested();
                        }
                    }

                    if (Jobs.Count == 0 || Jobs.All(j=>j.Status is JobStatus.Created)
                        || _robot.State != OperatingModes.Idle)
                    {
                        return;
                    }

                    #region Pick In Load Port

                    if (CanPickOnLoadPort())
                    {
                        PostEvent(new PickLoadPortSelected());
                        return;
                    }

                    #endregion

                    #region Pick/Place In Aligner

                    //Place on aligner
                    if (AtLeastOneWaferNotAligned() && !IsWaferStopping() && !IsWaferPausing())
                    {
                        PostEvent(new AlignerSelected());
                        return;
                    }

                    //Pick on aligner
                    if (RobotEmpty()
                        && !IsAlignerEmpty()
                        && !IsAlignerWaferPausing()
                        && ((_robot.Configuration.LowerArm.IsEnabled
                             && _robot.Configuration.UpperArm.IsEnabled)
                            || GetProcessModule(
                                    _aligner.Location.Wafer.JobProgram.PMItems.First().PMType)
                                ?.Location.Wafer
                            == null))
                    {
                        PostEvent(new AlignerSelected());
                        return;
                    }

                    #endregion

                    #region Place In Load Port

                    //Substrate has been canceled and must return in load port
                    if (_robot.UpperArmLocation.Wafer is { IsAligned: true }
                        && _robot.LowerArmLocation.Wafer == null
                        && SubstrateCanceled()
                        && !IsWaferPausing())
                    {
                        PostEvent(new PlaceLoadPortSelected());
                        _selectedRobotArm = RobotArm.Arm1;
                        return;
                    }

                    //Substrate has been processed on all process modules
                    if (_robot.UpperArmLocation.Wafer != null
                        && _robot.UpperArmLocation.Wafer.ProcessModuleIndex
                        >= _robot.UpperArmLocation.Wafer.JobProgram.PMItems.Count
                        && !IsWaferPausing())
                    {
                        PostEvent(new PlaceLoadPortSelected());
                        _selectedRobotArm = RobotArm.Arm1;
                        return;
                    }

                    if (_robot.LowerArmLocation.Wafer != null
                        && _robot.LowerArmLocation.Wafer.ProcessModuleIndex
                        >= _robot.LowerArmLocation.Wafer.JobProgram.PMItems.Count
                        && !IsWaferPausing())
                    {
                        PostEvent(new PlaceLoadPortSelected());
                        _selectedRobotArm = RobotArm.Arm2;
                        return;
                    }

                    //One wafer on robot from job stopping
                    if (IsWaferStopping())
                    {
                        PostEvent(new PlaceLoadPortSelected());
                        _selectedRobotArm = IsUpperArmWaferStopping()
                            ? RobotArm.Arm1
                            : RobotArm.Arm2;
                        return;
                    }

                    #endregion

                    #region Pick/Place In Process Module

                    //Case when the wafer needs to go in a process module after an alignment
                    if (_robot.UpperArmLocation.Wafer is { IsAligned: true }
                        && _robot.LowerArmLocation.Wafer == null
                        && !SubstrateCanceled()
                        && !IsWaferPausing())
                    {
                        var processModule = GetProcessModule(
                            _robot.UpperArmLocation.Wafer.JobProgram
                                .PMItems[_robot.UpperArmLocation.Wafer.ProcessModuleIndex].PMType);
                        if (processModule.ProcessModuleState != ProcessModuleState.Error)
                        {
                            _processModule = processModule;
                            PostEvent(new ProcessModuleSelected());
                        }

                        return;
                    }

                    if (_robot.LowerArmLocation.Wafer is { IsAligned: true }
                        && _robot.UpperArmLocation.Wafer == null
                        && !SubstrateCanceled()
                        && !IsWaferPausing())
                    {
                        var processModule = GetProcessModule(
                            _robot.LowerArmLocation.Wafer.JobProgram
                                .PMItems[_robot.LowerArmLocation.Wafer.ProcessModuleIndex].PMType);
                        if (processModule.ProcessModuleState != ProcessModuleState.Error)
                        {
                            _processModule = processModule;
                            PostEvent(new ProcessModuleSelected());
                        }

                        return;
                    }

                    //Case when we need to unload process modules at the end of the job
                    if (RobotEmpty()
                        && (GetRemainingSubstratesCount() <= 0
                            || IsNextWaferOnTool()
                            || !_robot.Configuration.LowerArm.IsEnabled
                            || !_robot.Configuration.UpperArm.IsEnabled))
                    {
                        var occupiedProcessModules = Controller.AllDevices<DriveableProcessModule>()
                            .Where(pm => pm.Location.Wafer != null)
                            .ToList();

                        if (occupiedProcessModules.Any())
                        {
                            foreach (var pmType in occupiedProcessModules.First()
                                         .Location.Wafer.JobProgram.PMItems)
                            {
                                var processModule = GetProcessModule(pmType.PMType);
                                if (processModule.Location.Wafer != null
                                    && !IsPmWaferPausing(processModule)
                                    && processModule.ProcessModuleState != ProcessModuleState.Error
                                    && ((_robot.Configuration.LowerArm.IsEnabled
                                         && _robot.Configuration.UpperArm.IsEnabled)
                                        || processModule.Location.Wafer.ProcessModuleIndex
                                        >= processModule.Location.Wafer.JobProgram.PMItems.Count - 1
                                        || GetProcessModule(
                                                processModule.Location.Wafer.JobProgram
                                                    .PMItems[processModule.Location.Wafer
                                                                 .ProcessModuleIndex
                                                             + 1].PMType)
                                            .Location.Wafer
                                        == null))
                                {
                                    _processModule = processModule;
                                    PostEvent(new ProcessModuleSelected());
                                    return;
                                }
                            }
                        }
                    }

                    #endregion
                });
        }

        private void WaitOrdersExit(Event ev)
        {
            PerformAction(
                () =>
                {
                    foreach (var job in Jobs)
                    {
                        if (job.Status == JobStatus.Queued
                            && job.RemainingWafers.Count == job.Wafers.Count)
                        {
                            job.StartTime = DateTime.Now;
                            OnJobStatusChanged(job, JobStatus.Executing);
                        }
                    }
                });
        }

        #region Aligner Actions

        private void GoInFrontOfAlignerEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    var tasksToWait = new List<Task>();

                    if (_alignerActivityManager.Activity == null)
                    {
                        RobotArm robotArm;
                        EffectorType effectorType;

                        if (_aligner.Location.Wafer == null)
                        {
                            robotArm = RobotArm.Arm2;
                            effectorType = _robot.Configuration.LowerArm.EffectorType;
                            var waferDimensionOnRobot = _robot.LowerArmWaferDimension;
                            var materialTypeOnRobot = _robot.LowerArmLocation.Wafer?.MaterialType
                                                      ?? MaterialType.Unknown;

                            if ((_robot.UpperArmLocation.Wafer != null
                                 || !_robot.Configuration.LowerArm.IsEnabled)
                                && _robot.UpperArmLocation.Wafer is { IsAligned: false })
                            {
                                robotArm = RobotArm.Arm1;
                                effectorType = _robot.Configuration.UpperArm.EffectorType;
                                waferDimensionOnRobot = _robot.UpperArmWaferDimension;
                                materialTypeOnRobot = _robot.UpperArmLocation.Wafer?.MaterialType
                                                      ?? MaterialType.Unknown;
                            }

                            tasksToWait.Add(
                                _robot.GoToSpecifiedLocationAsync(_aligner, 1, robotArm, false));

                            tasksToWait.Add(
                                _aligner.PrepareTransferAsync(
                                    effectorType,
                                    waferDimensionOnRobot,
                                    materialTypeOnRobot));
                        }
                        else
                        {
                            robotArm = RobotArm.Arm1;
                            effectorType = _robot.Configuration.UpperArm.EffectorType;
                            if (!_robot.Configuration.UpperArm.IsEnabled)
                            {
                                robotArm = RobotArm.Arm2;
                                effectorType = _robot.Configuration.LowerArm.EffectorType;
                            }

                            tasksToWait.Add(
                                _robot.GoToSpecifiedLocationAsync(_aligner, 1, robotArm, true));

                            tasksToWait.Add(
                                _aligner.PrepareTransferAsync(
                                    effectorType,
                                    _aligner.WaferDimension,
                                    _aligner.Location.Wafer.MaterialType));
                        }
                    }

                    Task.WaitAll(tasksToWait.ToArray());
                },
                new RobotDone());
        }

        private void WaitAlignerActivityDoneEntry(Event ev)
        {
            if (_alignerActivityManager.Activity == null)
            {
                PostEvent(new AlignerActivityDone());
            }
        }

        private void SwapInAlignerEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    if (_aligner.Location.Wafer != null)
                    {
                        var robotArm = RobotArm.Arm1;
                        var effectorType = _robot.Configuration.UpperArm.EffectorType;
                        if (!_robot.Configuration.UpperArm.IsEnabled)
                        {
                            robotArm = RobotArm.Arm2;
                            effectorType = _robot.Configuration.LowerArm.EffectorType;
                        }

                        _aligner.PrepareTransfer(
                            effectorType,
                            _aligner.WaferDimension,
                            _aligner.Location.Wafer.MaterialType);

                        _robot.Pick(robotArm, _aligner, 1);

                        if (_robot.LowerArmLocation.Wafer != null
                            && _robot.Configuration.UpperArm.IsEnabled
                            && _robot.Configuration.LowerArm.IsEnabled)
                        {
                            _aligner.PrepareTransfer(
                                _robot.Configuration.LowerArm.EffectorType,
                                _robot.LowerArmWaferDimension,
                                _robot.LowerArmLocation.Wafer.MaterialType);

                            _robot.Place(RobotArm.Arm2, _aligner, 1);
                        }
                    }
                    else
                    {
                        var robotArm = RobotArm.Arm2;

                        if ((_robot.UpperArmLocation.Wafer != null
                             || !_robot.Configuration.LowerArm.IsEnabled)
                            && _robot.UpperArmLocation.Wafer is { IsAligned: false })
                        {
                            robotArm = RobotArm.Arm1;
                        }

                        _robot.Place(robotArm, _aligner, 1);
                    }
                },
                new RobotDone());
        }

        private void StartAlignerActivityEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    var wafer = _aligner.Location.Wafer;
                    var job = Jobs.Find(j => j.Name == wafer.ProcessJobId);

                    _alignerActivity = new AlignerActivity.AlignerActivity(
                        Controller,
                        wafer.JobProgram.PMItems[wafer.ProcessModuleIndex].OrientationAngle,
                        job.OcrProfile,
                        IsSubstrateReaderEnabled,
                        IsSubstrateIdVerificationEnabled);

                    _alignerActivity.SubstrateIdReadingHasBeenFinished +=
                        AlignerActivity_SubstrateIdReadingHasBeenFinished;
                    _alignerActivity.WaferAlignStart += AlignerActivity_WaferAlignStart;
                    _alignerActivity.WaferAlignEnd += AlignerActivity_WaferAlignEnd;

                    if(!Efem.CanReleaseRobot() && IsSubstrateReaderEnabled)
                    {
                        _alignerActivityManager.Run(_alignerActivity);
                    }
                    else
                    {
                        _alignerActivityManager.Start(_alignerActivity);
                    }
                },
                new AlignerActivityStarted());
        }

        private void StartAlignerActivityExit(Event ev)
        {
            if (_aligner.Location.Wafer != null)
            {
                _aligner.Location.Wafer.IsAligned = true;
            }
        }

        #endregion

        #region LoadPort Actions

        private void PickInLpEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    var wafer = GetNextWafer();
                    var loadPortDest = _loadPorts[wafer.SourcePort - 1];

                    var robotArm = (_robot.LowerArmLocation.Wafer == null
                                    && _robot.Configuration.LowerArm.IsEnabled
                                    && !EquipmentEmpty()
                                    && !Controller.Configuration.UseOnlyUpperArmToLoadEquipment)
                                   || !_robot.Configuration.UpperArm.IsEnabled
                        ? RobotArm.Arm2
                        : RobotArm.Arm1;

                    if (loadPortDest.Configuration.CloseDoorAfterRobotAction)
                    {
                        _robot.GoToSpecifiedLocation(
                            loadPortDest,
                            wafer.SourceSlot,
                            robotArm,
                            true);
                    }
                
                    loadPortDest.PrepareForTransfer();
                    Thread.Sleep(100);
                    _robot.Pick(robotArm, loadPortDest, wafer.SourceSlot);

                    if (!CanPickOnLoadPort())
                    {
                        loadPortDest.PostTransfer();
                    }
                },
                new RobotDone());
        }

        private void PlaceInLpEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    switch (_selectedRobotArm)
                    {
                        case RobotArm.Arm1:
                            if (_robot.UpperArmLocation.Wafer != null)
                            {
                                var loadPortDest =
                                    _loadPorts[_robot.UpperArmLocation.Wafer.SourcePort - 1];
                                if (loadPortDest.Configuration.CloseDoorAfterRobotAction)
                                {
                                    _robot.GoToSpecifiedLocation(
                                        loadPortDest,
                                        _robot.UpperArmLocation.Wafer.SourceSlot,
                                        _selectedRobotArm,
                                        false);
                                }

                           
                                loadPortDest.PrepareForTransfer();
                                Thread.Sleep(100);
                                _robot.Place(
                                    RobotArm.Arm1,
                                    loadPortDest,
                                    _robot.UpperArmLocation.Wafer.SourceSlot);

                                if (!(RobotFullyOccupied() && IsWaferStopping())
                                    && !(GetRemainingSubstratesCount() > 0
                                         && !IsNextWaferOnTool()
                                         && (RobotEmpty()
                                             || (!RobotFullyOccupied()
                                                 && IsAlignerEmpty()
                                                 && ProcessModulesEmpty()))))
                                {
                                    loadPortDest.PostTransfer();
                                }
                            }

                            break;
                        case RobotArm.Arm2:
                            if (_robot.LowerArmLocation.Wafer != null)
                            {
                                var loadPortDest =
                                    _loadPorts[_robot.LowerArmLocation.Wafer.SourcePort - 1];
                                if (loadPortDest.Configuration.CloseDoorAfterRobotAction)
                                {
                                    _robot.GoToSpecifiedLocation(
                                        loadPortDest,
                                        _robot.LowerArmLocation.Wafer.SourceSlot,
                                        _selectedRobotArm,
                                        false);
                                }

                                loadPortDest.PrepareForTransfer();
                                Thread.Sleep(100);
                                _robot.Place(
                                    RobotArm.Arm2,
                                    loadPortDest,
                                    _robot.LowerArmLocation.Wafer.SourceSlot);

                                if (!(RobotFullyOccupied() && IsWaferStopping())
                                    && !(GetRemainingSubstratesCount() > 0
                                         && !IsNextWaferOnTool()
                                         && (RobotEmpty()
                                             || (!RobotFullyOccupied()
                                                 && IsAlignerEmpty()
                                                 && ProcessModulesEmpty()))))
                                {
                                    loadPortDest.PostTransfer();
                                }
                            }

                            break;
                    }
                },
                new RobotDone());
        }

        #endregion

        #region Process Module Actions

        private void PreparePmEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    var transferType = TransferType.Pick;
                    MaterialType materialType;
                    SampleDimension sampleDimension;
                    RobotArm robotArm;

                    if (_processModule.Location.Wafer == null)
                    {
                        transferType = TransferType.Place;
                    }
                
                    if (transferType == TransferType.Pick)
                    {
                        robotArm = RobotArm.Arm2;
                        if (_robot.LowerArmLocation.Wafer != null
                            || !_robot.Configuration.LowerArm.IsEnabled)
                        {
                            robotArm = RobotArm.Arm1;
                        }

                        materialType = _processModule.Location.Wafer!.MaterialType;
                        sampleDimension = _processModule.Location.Wafer!.MaterialDimension;
                    }
                    else
                    {
                        robotArm = RobotArm.Arm1;
                        if (_robot.LowerArmLocation.Wafer != null
                            && _robot.Configuration.LowerArm.IsEnabled)
                        {
                            robotArm = RobotArm.Arm2;
                        }

                        materialType = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmLocation.Wafer!.MaterialType
                            : _robot.LowerArmLocation.Wafer!.MaterialType;
                        sampleDimension = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmWaferDimension
                            : _robot.LowerArmWaferDimension;
                    }

                    if (_processModule.TransferState == EnumPMTransferState.NotReady
                        || _processModule.TransferState
                        == EnumPMTransferState.ReadyToLoad_SlitDoorClosed
                        || _processModule.TransferState
                        == EnumPMTransferState.ReadyToUnload_SlitDoorClosed)
                    {
                        _processModule.PrepareTransferAsync(transferType, robotArm, materialType, sampleDimension);
                    }
                },
                new PmDone());
        }

        private void GoInFrontOfPmEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    if (_processModule.Location.Wafer != null)
                    {
                        var pickArm = RobotArm.Arm2;
                        if (_robot.LowerArmLocation.Wafer != null
                            || !_robot.Configuration.LowerArm.IsEnabled)
                        {
                            pickArm = RobotArm.Arm1;
                        }

                        _robot.GoToSpecifiedLocation(_processModule, 1, pickArm, true);
                    }
                    else
                    {
                        var placeArm = RobotArm.Arm1;
                        if (_robot.LowerArmLocation.Wafer != null
                            && _robot.Configuration.LowerArm.IsEnabled)
                        {
                            placeArm = RobotArm.Arm2;
                        }

                        _robot.GoToSpecifiedLocation(_processModule, 1, placeArm, false);
                    }
                },
                new RobotDone());
        }

        private void PrepareTransferOnPmEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    var transferType = TransferType.Pick;
                    MaterialType materialType;
                    SampleDimension sampleDimension;
                    RobotArm robotArm;

                    if (_processModule.Location.Wafer == null)
                    {
                        transferType = TransferType.Place;
                    }
                
                    if (transferType == TransferType.Pick)
                    {
                        robotArm = RobotArm.Arm2;
                        if (_robot.LowerArmLocation.Wafer != null
                            || !_robot.Configuration.LowerArm.IsEnabled)
                        {
                            robotArm = RobotArm.Arm1;
                        }

                        materialType = _processModule.Location.Wafer!.MaterialType;
                        sampleDimension = _processModule.Location.Wafer!.MaterialDimension;
                    }
                    else
                    {
                        robotArm = RobotArm.Arm1;
                        if (_robot.LowerArmLocation.Wafer != null
                            && _robot.Configuration.LowerArm.IsEnabled)
                        {
                            robotArm = RobotArm.Arm2;
                        }

                        materialType = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmLocation.Wafer!.MaterialType
                            : _robot.LowerArmLocation.Wafer!.MaterialType;
                        sampleDimension = robotArm == RobotArm.Arm1
                            ? _robot.UpperArmWaferDimension
                            : _robot.LowerArmWaferDimension;
                    }

                    try
                    {
                        _processModule.PrepareTransfer(transferType, robotArm, materialType, sampleDimension);

                        if (transferType == TransferType.Pick)
                        {
                            if (robotArm == RobotArm.Arm1 && _robot.LowerArmLocation.Wafer != null)
                            {
                                _processModule.SelectRecipe(_robot.LowerArmLocation.Wafer);
                            }
                            else if (robotArm == RobotArm.Arm2 && _robot.UpperArmLocation.Wafer != null)
                            {
                                _processModule.SelectRecipe(_robot.UpperArmLocation.Wafer);
                            }
                        }
                        else
                        {
                            _processModule.SelectRecipe(
                                robotArm == RobotArm.Arm1
                                    ? _robot.UpperArmLocation.Wafer
                                    : _robot.LowerArmLocation.Wafer);
                        }

                        PostEvent(new PmReadyToTransfer());
                    }
                    catch
                    {
                        PostEvent(new PmNotReadyToTransfer());
                    }
                });
        }

        private void WaitPmReadyToTransferEntry(Event ev)
        {
            _processModule.StatusValueChanged += ProcessModule_StatusValueChanged;
            _processModule.ReadyToTransfer += ProcessModule_ReadyToTransfer;
        }

        private void SwapInPmEntry(Event ev)
        {
            PerformAction(
                () =>
                {
                    if (_processModule.Location.Wafer != null)
                    {
                        _processModule.Location.Wafer.ProcessModuleIndex++;

                        //Reset IsAligned property if an alignment is needed between two PMs
                        if (_processModule.Location.Wafer.ProcessModuleIndex
                            < _processModule.Location.Wafer.JobProgram.PMItems.Count)
                        {
                            var pmItem =
                                _processModule.Location.Wafer.JobProgram.PMItems[_processModule
                                    .Location.Wafer.ProcessModuleIndex];

                            var previousPmItem =
                                _processModule.Location.Wafer.JobProgram.PMItems[
                                    _processModule.Location.Wafer.ProcessModuleIndex - 1];

                            if (Math.Abs(pmItem.OrientationAngle - previousPmItem.OrientationAngle)
                                >= 0.1)
                            {
                                _processModule.Location.Wafer.IsAligned = false;
                            }
                        }

                        var pickArm = RobotArm.Arm2;
                        if (_robot.LowerArmLocation.Wafer != null
                            || !_robot.Configuration.LowerArm.IsEnabled)
                        {
                            pickArm = RobotArm.Arm1;
                        }

                        _robot.Pick(pickArm, _processModule, 1);

                        switch (pickArm)
                        {
                            case RobotArm.Arm1 when _robot.LowerArmLocation.Wafer != null:
                                {
                                    if (_processModule is ToolControlProcessModule
                                        toolControlProcessModule)
                                    {
                                        toolControlProcessModule.PrepareTransfer(
                                            TransferType.Place,
                                            RobotArm.Arm2,
                                            _robot.LowerArmLocation.Wafer.MaterialType,
                                            _robot.LowerArmLocation.Wafer.MaterialDimension);
                                    }

                                    _robot.Place(RobotArm.Arm2, _processModule, 1);
                                }
                                break;
                            case RobotArm.Arm2 when _robot.UpperArmLocation.Wafer != null:
                                {
                                    if (_processModule is ToolControlProcessModule
                                        toolControlProcessModule)
                                    {
                                        toolControlProcessModule.PrepareTransfer(
                                            TransferType.Place,
                                            RobotArm.Arm1,
                                            _robot.UpperArmLocation.Wafer.MaterialType,
                                            _robot.UpperArmLocation.Wafer.MaterialDimension);
                                    }

                                    _robot.Place(RobotArm.Arm1, _processModule, 1);
                                }
                                break;
                        }
                    }
                    else
                    {
                        var placeArm = RobotArm.Arm1;
                        if (_robot.LowerArmLocation.Wafer != null
                            && _robot.Configuration.LowerArm.IsEnabled)
                        {
                            placeArm = RobotArm.Arm2;
                        }

                        _robot.Place(placeArm, _processModule, 1);
                    }
                },
                new RobotDone());
        }

        private void PostTransferAndStartPmEntry(Event ev)
        {
            var processModule = _processModule;
            PerformAction(
                () =>
                {
                    Task.Run(
                        () =>
                        {
                            processModule.PostTransfer();
                            if (processModule.Location.Wafer != null)
                            {
                                processModule.StartRecipe();
                            }
                        });
                },
                new PmDone(),
                false);
        }

        private void PmExit(Event ev)
        {
            lock (_lockProcessModule)
            {
                _processModule = null;
            }
        }

        #endregion

        #endregion

        #region Conditionals

        private bool AlignerEmpty(Event ev)
        {
            return _aligner.Location.Wafer == null;
        }

        private bool AlignerOccupied(Event ev)
        {
            return !AlignerEmpty(ev);
        }

        private bool PmEmptyOrIdle(Event ev)
        {
            var expectedWaferStatus =
                _processModule.WaferStatus is WaferStatus.Processed or WaferStatus.ProcessingFailed;

            if (PmOccupied()
                && _processModule.Location.Wafer.ProcessModuleIndex
                < _processModule.Location.Wafer.JobProgram.PMItems.Count - 1)
            {
                expectedWaferStatus = _processModule.WaferStatus is WaferStatus.Processing;
            }

            return PmEmpty()
                   || (_processModule.ProcessModuleState == ProcessModuleState.Idle
                       && PmOccupied()
                       && expectedWaferStatus);
        }

        private bool PmFullAndActive(Event ev)
        {
            var expectedWaferStatus =
                _processModule.WaferStatus is not WaferStatus.Processed
                    and not WaferStatus.ProcessingFailed;

            if (PmOccupied()
                && _processModule.Location.Wafer.ProcessModuleIndex
                < _processModule.Location.Wafer.JobProgram.PMItems.Count - 1)
            {
                expectedWaferStatus = _processModule.WaferStatus is not WaferStatus.Processed
                    and not WaferStatus.ProcessingFailed
                    and not WaferStatus.Processing;
            }

            return PmOccupied()
                   && (expectedWaferStatus
                       || _processModule.ProcessModuleState == ProcessModuleState.Active);
        }

        private bool PmEmpty()
        {
            return _processModule.Location.Wafer == null;
        }

        private bool PmOccupied()
        {
            return _processModule.Location.Wafer != null;
        }

        private bool IsWaferStopping()
        {
            if (Jobs.FirstOrDefault(
                    j => j.Status == JobStatus.Stopping
                         && j.Wafers.Exists(
                             w => _robot.UpperArmLocation.Wafer?.SubstrateId == w.SubstrateId
                                  || _robot.LowerArmLocation.Wafer?.SubstrateId
                                  == w.SubstrateId)) is { } job)
            {
                return job.StopConfig != StopConfig.FinishProcessForAllWafersOnTools;
            }

            return false;
        }

        private bool IsUpperArmWaferStopping()
        {
            return Jobs.Exists(
                j => j.Status == JobStatus.Stopping
                     && j.Wafers.Exists(
                         w => _robot.UpperArmLocation.Wafer?.SubstrateId == w.SubstrateId));
        }

        private bool IsWaferPausing()
        {
            return Jobs.Exists(
                j => j.Status is JobStatus.Pausing or JobStatus.Paused
                     && j.Wafers.Exists(
                         w => _robot.UpperArmLocation.Wafer?.SubstrateId == w.SubstrateId
                              || _robot.LowerArmLocation.Wafer?.SubstrateId == w.SubstrateId));
        }

        private bool IsAlignerWaferPausing()
        {
            return Jobs.Exists(
                j => j.Status is JobStatus.Pausing or JobStatus.Paused
                     && j.Wafers.Exists(
                         w => _aligner.Location.Wafer?.SubstrateId == w.SubstrateId));
        }

        private bool IsPmWaferPausing(DriveableProcessModule pm)
        {
            return Jobs.Exists(
                j => j.Status is JobStatus.Pausing or JobStatus.Paused
                     && j.Wafers.Exists(w => pm.Location.Wafer?.SubstrateId == w.SubstrateId));
        }

        private bool IsNextWaferOnTool()
        {
            foreach (var wafers in Jobs.Where(job => job.Status == JobStatus.Executing)
                         .Select(job => job.RemainingWafers))
            {
                if (wafers.Count <= 0)
                {
                    continue;
                }

                var wafer = wafers[0];
                return SubstratesOnTool.FirstOrDefault(w => w.Name == wafer.Name) != null;
            }

            return false;
        }

        private bool WaferAlignmentBetweenProcessModules()
        {
            //If one arm is disabled, if any wafer on process modules that are not the final one needs alignement
            //We returns true because alignment is needed
            if ((!_robot.Configuration.UpperArm.IsEnabled
                 || !_robot.Configuration.LowerArm.IsEnabled)
                && Controller.AllDevices<DriveableProcessModule>()
                    .Any(
                        pm => pm.Location.Wafer != null
                              && pm.Location.Wafer.ProcessModuleIndex
                              < pm.Location.Wafer.JobProgram.PMItems.Count - 1)
                && Jobs.Any(
                    j => j.Wafers.Any(
                        w => w.JobProgram.PMItems.Select(pm => pm.OrientationAngle)
                                 .Distinct()
                                 .Count()
                             > 1)))
            {
                return true;
            }

            if (_aligner.Location.Wafer == null)
            {
                return false;
            }

            return _aligner.Location.Wafer.JobProgram.PMItems.Select(pm => pm.OrientationAngle)
                       .Distinct()
                       .Count()
                   > 1;
        }

        #endregion

        #region Event Handlers

        private void DataFlowManager_DataFlowRecipeCompleted(
            object sender,
            DataFlowRecipeEventArgs e)
        {
            if (Jobs.FirstOrDefault(j => j.Name == e.ProcessJobId) is not { } job)
            {
                return;
            }

            if (job.WaferResultReceived.Contains(e.SubstrateId))
            {
                return;
            }

            job.WaferResultReceived.Add(e.SubstrateId);
            PostEvent(new Reevaluate());
        }

        private void DataFlowManager_ProcessModuleAcquisitionCompleted(
            object sender,
            ProcessModuleRecipeEventArgs e)
        {
            if (e.ProcessModule.Location.Substrate != null
                && e.ProcessModule.InstanceId
                == GetProcessModule(e.ProcessModule.Location.Wafer.JobProgram.PMItems.Last().PMType)
                    .InstanceId)
            {
                switch (e.RecipeTerminationState)
                {
                    case RecipeTerminationState.canceled:
                        e.ProcessModule.Location.Substrate.Status = WaferStatus.Aborted;
                        break;
                    case RecipeTerminationState.failed:
                        e.ProcessModule.Location.Substrate.Status = WaferStatus.ProcessingFailed;
                        break;
                    case RecipeTerminationState.successfull:
                        e.ProcessModule.Location.Substrate.Status = WaferStatus.Processed;
                        break;
                }
            }
        }

        private void DataFlowManager_ProcessModuleRecipeStarted(
            object sender,
            ProcessModuleRecipeEventArgs e)
        {
            if (e.ProcessModule.Location.Substrate != null)
            {
                e.ProcessModule.Location.Substrate.Status = WaferStatus.Processing;
            }
        }

        private void AlignerActivityManager_ActivityDone(object sender, ActivityEventArgs e)
        {
            if (e.Status == CommandStatusCode.Ok)
            {
                PostEvent(new AlignerActivityDone());
            }
        }

        private void AlignerActivity_SubstrateIdReadingHasBeenFinished(
            object sender,
            SubstrateIdReadingHasBeenFinishedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            _alignerActivity.SubstrateIdReadingHasBeenFinished -=
                AlignerActivity_SubstrateIdReadingHasBeenFinished;
            OnSubstrateIdReadingHasBeenFinished(e.IsSuccess, e.SubstrateId, e.AcquiredId);
        }

        private void AlignerActivity_WaferAlignStart(object sender, System.EventArgs e)
        {
            _alignerActivity.WaferAlignStart -= AlignerActivity_WaferAlignStart;
            OnWaferAlignStart();
        }

        private void AlignerActivity_WaferAlignEnd(object sender, System.EventArgs e)
        {
            _alignerActivity.WaferAlignEnd -= AlignerActivity_WaferAlignEnd;
            OnWaferAlignEnd();
        }

        private void ProcessModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not DriveableProcessModule processModule)
            {
                return;
            }

            switch (e.Status.Name)
            {
                case nameof(IDriveableProcessModule.TransferState):
                case nameof(IDriveableProcessModule.TransferValidationState):
                case nameof(IDriveableProcessModule.IsReadyToLoadUnload):

                    if (CheckPmIsReadyToTransfer(processModule))
                    {
                        processModule.StatusValueChanged -= ProcessModule_StatusValueChanged;
                        SendPmReadySmEvent(processModule);
                    }
                    break;
            }
        }

        private void ProcessModule_ReadyToTransfer(object sender, System.EventArgs e)
        {
            if (sender is not DriveableProcessModule processModule)
            {
                return;
            }

            if (CheckPmIsReadyToTransfer(processModule))
            {
                processModule.ReadyToTransfer -= ProcessModule_ReadyToTransfer;
                SendPmReadySmEvent(processModule);
            }
        }

        private bool CheckPmIsReadyToTransfer(DriveableProcessModule processModule)
        {
            if (processModule.ActorType is ActorType.Wotan or ActorType.Thor)
            {
                return true;
            }

            if (processModule.WaferPresence == WaferPresence.Present)
            {
                if (processModule.TransferState == EnumPMTransferState.ReadyToUnload_SlitDoorOpened
                    && processModule.IsReadyToLoadUnload)
                {
                    return true;
                }
            }
            else
            {
                if (processModule.TransferState == EnumPMTransferState.ReadyToLoad_SlitDoorOpened
                    && processModule.IsReadyToLoadUnload)
                {
                    return true;
                }
            }

            return false;
        }

        private void SendPmReadySmEvent(DriveableProcessModule processModule)
        {
            try
            {
                Logger.Debug($"Process module {processModule.InstanceId} is ready for transfer in WaferFlow");
                PostEvent(new PmReadyToTransfer());
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error occured in waferFlow");
            }
        }



        private void Device_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {

            if ((e.Status.Name.Equals(nameof(IGenericDevice.State))
                 && e.NewValue is OperatingModes.Idle)
                || (e.Status.Name.Equals(nameof(DriveableProcessModule.ProcessModuleState))
                    && e.NewValue is ProcessModuleState.Idle))
            {
                try
                {
                    PostEvent(new Reevaluate());
                }
                catch (Exception exception)
                {
                   Logger.Error(exception,"Error occured in waferFlow");
                }
            }

            if (_processModule == null)
            {
                return;
            }

            lock (_lockProcessModule)
            {
                if (sender is DriveableProcessModule pm
                    && _processModule != null
                    && pm.InstanceId == _processModule.InstanceId
                    && e.Status.Name == nameof(IDriveableProcessModule.ProcessModuleState)
                    && (ProcessModuleState)e.NewValue == ProcessModuleState.Error)
                {
                    try
                    {
                        PostEvent(new PmInError());
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, "Error occured in waferFlow");
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void AbortDataFlowRecipe(Job job)
        {
            if (_dataFlowManager.DataflowState == TC_DataflowStatus.Maintenance)
            {
                foreach (var processModule in Controller.AllDevices<DriveableProcessModule>()
                             .Where(pm => pm.ProcessModuleState == ProcessModuleState.Active))
                {
                    processModule.AbortRecipeAsync();
                }
            }
            else
            {
                _dataFlowManager?.AbortRecipeAsync(job.Name);
            }
        }

        private int GetRemainingSubstratesCount()
        {
            var remainingSubstratesCount = 0;
            foreach (var job in Jobs.Where(job => job.Status == JobStatus.Executing))
            {
                remainingSubstratesCount += job.RemainingWafers.Count;
            }

            return remainingSubstratesCount;
        }

        private Wafer GetNextWafer()
        {
            foreach (var job in Jobs.Where(job => job.Status == JobStatus.Executing))
            {
                if (job.RemainingWafers.Count <= 0)
                {
                    continue;
                }

                var wafer = job.RemainingWafers[0];
                job.RemainingWafers.RemoveAt(0);

                if (job.RemainingWafers.Count == 0)
                {
                    OnLastWaferEntry();
                }

                //Needed for job cycling
                wafer.ProcessJobId = job.Name;
                wafer.ControlJobId = job.ControlJobId;
                wafer.ProcessModuleIndex = 0;
                wafer.IsAligned = false;
                wafer.Status = WaferStatus.WaitingProcess;
                wafer.JobPosition = GetWaferJobPosition(job.Wafers, wafer);
                wafer.JobStartTime = job.StartTime;

                if (_dataFlowManager != null)
                {
                    var currentRecipe = _dataFlowManager.GetRecipeInfo(job.RecipeName);
                    _dataFlowManager.StartJobOnMaterial(currentRecipe, wafer);
                }

                return wafer;
            }

            //No substrate found
            return null;
        }

        private JobPosition GetWaferJobPosition(List<Wafer> wafers, Wafer wafer)
        {
            if (wafers.Count == 1)
            {
                return JobPosition.FirstAndLast;
            }

            if (wafers.First() == wafer)
            {
                return JobPosition.First;
            }

            if (wafers.Last() == wafer)
            {
                return JobPosition.Last;
            }

            return JobPosition.Between;
        }

        private DriveableProcessModule GetProcessModule(ActorType actor)
        {
            var processModules = Controller
                .AllDevices<DriveableProcessModule>()
                .Where(pm => !pm.IsOutOfService).ToList();

            if (processModules.Count == 0)
            {
                return null;
            }

            var processModule = processModules.FirstOrDefault(d => d.ActorType == actor);

            if (processModule == null)
            {
                throw new InvalidOperationException(
                    $"Process module of type {actor} not found in equipment tree");
            }

            return processModule;
        }

        private bool RobotFullyOccupied()
        {
            return (_robot.UpperArmLocation.Wafer != null
                    || !_robot.Configuration.UpperArm.IsEnabled)
                   && (_robot.LowerArmLocation.Wafer != null
                       || !_robot.Configuration.LowerArm.IsEnabled);
        }

        private bool RobotEmpty()
        {
            return _robot.UpperArmLocation.Wafer == null && _robot.LowerArmLocation.Wafer == null;
        }

        private bool IsAlignerEmpty()
        {
            return _aligner.Location.Wafer == null;
        }

        private bool ProcessModulesEmpty()
        {
            return Controller.AllDevices<Abstractions.Devices.ProcessModule.ProcessModule>()
                .All(pm => pm.Location.Wafer == null);
        }

        private bool EquipmentEmpty()
        {
            return RobotEmpty() && IsAlignerEmpty() && ProcessModulesEmpty();
        }

        private bool AtLeastOneWaferNotAligned()
        {
            return _robot.UpperArmLocation.Wafer is { IsAligned: false }
                   || _robot.LowerArmLocation.Wafer is { IsAligned: false };
        }

        private bool SubstrateCanceled()
        {
            if (_robot.UpperArmLocation.Wafer == null)
            {
                return false;
            }

            var acquiredId = _robot.UpperArmLocation.Wafer.AcquiredId;
            var job = Jobs.First(j => j.Name == _robot.UpperArmLocation.Wafer.ProcessJobId);

            return IsSubstrateReaderEnabled
                   && IsSubstrateIdVerificationEnabled
                   && job.OcrProfile != null
                   && string.IsNullOrWhiteSpace(acquiredId);
        }

        private bool IsCriticalPmAlarmUseCase()
        {
            var occupiedProcessModules = Controller.AllDevices<DriveableProcessModule>()
                .Where(pm => pm.Location.Wafer != null)
                .ToList();

            if (occupiedProcessModules.Count == 0)
            {
                return false;
            }

            return SubstratesOnTool.All(
                s => occupiedProcessModules.FirstOrDefault(
                         pm => pm.Location.Substrate.Name == s.Name
                               && pm.ProcessModuleState == ProcessModuleState.Error)
                     != null);
        }

        private bool CanPickOnLoadPort()
        {
            //If the aligner and PMs are empty, fully load the robot
            if (GetRemainingSubstratesCount() > 0
                && !IsNextWaferOnTool()
                && !WaferAlignmentBetweenProcessModules()
                && ((RobotEmpty()
                     && !Controller.Configuration.UseOnlyUpperArmToLoadEquipment
                     && _robot.Configuration.UpperArm.IsEnabled
                     && _robot.Configuration.LowerArm.IsEnabled)
                    || ((Controller.Configuration.UseOnlyUpperArmToLoadEquipment
                         || !_robot.Configuration.UpperArm.IsEnabled
                         || !_robot.Configuration.LowerArm.IsEnabled)
                        && RobotEmpty()
                        && IsAlignerEmpty())))
            {
                return true;
            }

            //Pick in load port only if we are starting the job and taking two wafers
            if (GetRemainingSubstratesCount() > 0
                && !IsNextWaferOnTool()
                && !RobotFullyOccupied()
                && IsAlignerEmpty()
                && ProcessModulesEmpty()
                && !Controller.Configuration.UseOnlyUpperArmToLoadEquipment
                && _robot.Configuration.UpperArm.IsEnabled
                && _robot.Configuration.LowerArm.IsEnabled)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
