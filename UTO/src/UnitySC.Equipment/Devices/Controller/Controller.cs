using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using Castle.Core.Internal;

using UnitsNet;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Devices.Controller.Configuration;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.Equipment.Abstractions.Devices.LightTower.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.Equipment.Devices.Controller.Activities;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow;
using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;
using UnitySC.Equipment.Devices.Controller.EventArgs;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.Equipment.Devices.Controller.OperatingModeSM;
using UnitySC.Equipment.Devices.Controller.Throughput;

namespace UnitySC.Equipment.Devices.Controller
{
    public partial class Controller : IConfigurableDevice<ControllerConfiguration>
    {
        #region Properties

        public ObservableCollection<Job> Jobs
            => _waferFlow == null
                ? null
                : new ObservableCollection<Job>(_waferFlow.Jobs);

        #endregion

        #region Setup

        /// <inheritdoc />
        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    _userLogger = Agileo.Common.Logging.Logger.GetLogger("User");
                    _operatingModeSm = new ControllerOperatingModeSm(this);
                    _operatingModeSm.OnStateChanged += OperatingModeSm_OnStateChanged;
                    _operatingModeSm.UserErrorRaised += OperatingModeSm_UserErrorRaised;
                    _operatingModeSm.UserWarningRaised += OperatingModeSm_UserWarningRaised;
                    _operatingModeSm.UserInformationRaised += OperatingModeSm_UserInformationRaised;
                    _operatingModeSm.Start();
                    break;
                case SetupPhase.SettingUp:
                    _waferFlowActivityManager = new ActivityManager(Logger);
                    if (ExecutionMode == ExecutionMode.Simulated)
                    {
                        SetUpSimulatedMode();
                    }

                    //Set more time for initialize because it will take longer time than other devices to initialize
                    DeviceType.AllCommands().First(x => x.Name == nameof(Initialize)).Timeout =
                        Duration.FromSeconds(Configuration.InitializationTimeout);
                    break;
                case SetupPhase.SetupDone:
                    MaterialManager.MaterialMoved += MaterialManager_MaterialMoved;
                    if (this.AllDevices<DriveableProcessModule>().IsNullOrEmpty()
                        || this.AllDevices<AbstractDataFlowManager>().IsNullOrEmpty())
                    {
                        return;
                    }

                    _throughputStatistics = new ThroughputStatistics(this);
                    _throughputStatistics.ThroughputValueChanged +=
                        ThroughputStatistics_ThroughputValueChanged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region Fields

        private ThroughputStatistics _throughputStatistics;

        #endregion

        #region Configuration

        public new ControllerConfiguration Configuration
            => base.Configuration.Cast<ControllerConfiguration>();

        public ControllerConfiguration CreateDefaultConfiguration() => new();

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Controller)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
            => ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);

        #endregion Configuration

        #region Fields

        private ControllerOperatingModeSm _operatingModeSm;
        private ActivityManager _waferFlowActivityManager;
        private WaferFlow _waferFlow;
        private ILogger _userLogger;

        #endregion Fields

        #region Events

        public event EventHandler<SubstrateIdReadingHasBeenFinishedEventArgs>
            SubstrateIdReadingHasBeenFinished;

        public event EventHandler WaferAlignStart;
        public event EventHandler WaferAlignEnd;
        public event EventHandler LastWaferEntry;
        public event EventHandler<JobStatusChangedEventArgs> JobStatusChanged;

        #endregion

        #region Private Methods

        private void InstanceInitialization()
        {
            //Do nothing
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_operatingModeSm != null)
                {
                    _operatingModeSm.OnStateChanged -= OperatingModeSm_OnStateChanged;
                    _operatingModeSm.UserErrorRaised -= OperatingModeSm_UserErrorRaised;
                    _operatingModeSm.UserWarningRaised -= OperatingModeSm_UserWarningRaised;
                    _operatingModeSm.UserInformationRaised -= OperatingModeSm_UserInformationRaised;
                    _operatingModeSm.Dispose();
                    _operatingModeSm = null;
                }

                if (_waferFlow != null)
                {
                    _waferFlow.SubstrateIdReadingHasBeenFinished -=
                        WaferFlow_SubstrateIdReadingHasBeenFinished;
                    _waferFlow.WaferAlignStart -= WaferFlow_WaferAlignStart;
                    _waferFlow.WaferAlignEnd -= WaferFlow_WaferAlignEnd;
                    _waferFlow.LastWaferEntry -= WaferFlow_LastWaferEntry;
                    _waferFlow.JobStatusChanged -= WaferFlowJobStatusChanged;
                    _waferFlow.OnTransitionChanged -= WaferFlow_OnTransitionChanged;
                }

                if (_throughputStatistics != null)
                {
                    _throughputStatistics.ThroughputValueChanged -=
                        ThroughputStatistics_ThroughputValueChanged;
                    _throughputStatistics.Dispose();
                }
            }

            DefineLightsState(LightTowerState.AllTheLightsOff);
            base.Dispose(disposing);
        }

        private void OnSubstrateIdReadingHasBeenFinished(
            bool isSuccess = false,
            string substrateId = "",
            string acquiredId = "")
        {
            if (SubstrateIdReadingHasBeenFinished != null)
            {
                var args = new SubstrateIdReadingHasBeenFinishedEventArgs(
                    isSuccess,
                    substrateId,
                    acquiredId);
                SubstrateIdReadingHasBeenFinished.Invoke(this, args);
            }
        }

        private void OnJobStatusChanged(Job job)
        {
            try
            {
                JobStatusChanged?.Invoke(this, new JobStatusChangedEventArgs(job));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnWaferAlignStart() => WaferAlignStart?.Invoke(this, System.EventArgs.Empty);

        private void OnWaferAlignEnd() => WaferAlignEnd?.Invoke(this, System.EventArgs.Empty);

        protected void OnLastWaferEntry() => LastWaferEntry?.Invoke(this, System.EventArgs.Empty);

        private void DefineLightsState(LightTowerState state)
        {
            var lightTower = this.AllDevices<LightTower>().First();
            if (lightTower is { IsCommunicating: true })
            {
                lightTower.DefineStateAsync(state);
            }
        }

        #endregion Private Methods

        #region IMaterialLocationPicker

        public override ReadOnlyCollection<MaterialLocation> PickSafeLocations(
            Device device,
            DeviceCommand command,
            Parameter parameter)
        {
            if (!(device is IMaterialLocationContainer))
            {
                return new ReadOnlyCollection<MaterialLocation>(new List<MaterialLocation>());
            }

            // Pick safe locations only for devices commands that make a movement.
            switch (device.DeviceType.Name)
            {
                case nameof(Robot):
                    return new ReadOnlyCollection<MaterialLocation>(
                        PickSafeLocationsForRobotCommands((Robot)device, command).ToList());
                default:
                    return new ReadOnlyCollection<MaterialLocation>(new List<MaterialLocation>());
            }
        }

        private static IEnumerable<MaterialLocation> PickSafeLocationsForRobotCommands<T>(
            T robot,
            INamedElement command)
            where T : Robot
        {
            // Get all location containers except the robot
            var materialLocationContainers = robot.GetEquipment()
                .AllOfType<IMaterialLocationContainer>()
                .Where(container => !(container is T));
            switch (command.Name)
            {
                case nameof(IRobot.GoToLocation):
                case nameof(IRobot.GoToTransferLocation):
                case nameof(IRobot.GoToSpecifiedLocation):
                    return GetSubstrateLocations(materialLocationContainers);
                case nameof(IRobot.Pick):
                    return GetSubstrateLocations(materialLocationContainers, false);
                case nameof(IRobot.Place):
                    return GetSubstrateLocations(materialLocationContainers, true, false);
                default:
                    return new List<MaterialLocation>();
            }
        }

        private static IEnumerable<MaterialLocation> GetSubstrateLocations(
            IEnumerable<IMaterialLocationContainer> materialLocationContainers,
            bool getEmpties = true,
            bool getBusies = true)
            => materialLocationContainers.SelectMany(
                materialLocationContainer => CheckMaterialLocationContainer(
                    materialLocationContainer,
                    getEmpties,
                    getBusies));

        private static IEnumerable<MaterialLocation> CheckMaterialLocationContainer(
            IMaterialLocationContainer materialLocationContainer,
            bool getEmpties = true,
            bool getBusies = true)
        {
            foreach (var materialLocation in materialLocationContainer.MaterialLocations)
            {
                if (materialLocation is SubstrateLocation
                    && ((getEmpties && materialLocation.Material == null)
                        || (getBusies && materialLocation.Material != null)))
                {
                    yield return materialLocation;
                }
                else if (materialLocation.Material is IMaterialLocationContainer locationMaterial)
                {
                    foreach (var location in CheckMaterialLocationContainer(
                                 locationMaterial,
                                 getEmpties,
                                 getBusies))
                    {
                        yield return location;
                    }
                }
            }
        }

        #endregion IMaterialLocationPicker

        #region Activity Management

        private void RunInitActivity(bool isColdInit)
        {
            try
            {
                if (!this.TryGetAlarmCenter(out var alarmCenter))
                {
                    return;
                }

                //Clear devices alarms
                this.AllDevices<GenericDevice>().ToList().ForEach(d => d.ClearAllAlarms());

                //Clear software alarms
                alarmCenter.Services.ClearAllAlarms();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error while clearing all alarms");
            }

            RunActivity(new Init(isColdInit, this));
        }

        public void StartLoadProcessModuleActivity(LoadToPmConfiguration config)
            => StartActivity(new LoadToPm(config));

        public void StartUnloadProcessModuleActivity(UnloadFromPmConfiguration config)
            => StartActivity(new UnloadFromPm(config));

        public void StartClearActivity() => StartActivity(new Clear(this, null));

        #endregion Activity Management

        #region Public Methods

        public void ProceedWithSubstrate() => _waferFlow?.ProceedWithSubstrate();

        public void CancelSubstrate() => _waferFlow?.CancelSubstrate();

        public void UpdateReaderBehavior(
            bool isSubstrateReaderEnabled,
            bool isSubstrateIdVerificationEnabled)
            => _waferFlow?.UpdateReaderBehavior(
                isSubstrateReaderEnabled,
                isSubstrateIdVerificationEnabled);

        public void UpdateLightTower()
        {
            switch (State)
            {
                case OperatingModes.Maintenance:
                    DefineLightsState(LightTowerState.AbnormalCondition);
                    break;
                case OperatingModes.Initialization:
                    DefineLightsState(LightTowerState.ProcessActive);
                    break;
                case OperatingModes.Idle:
                    DefineLightsState(LightTowerState.NormalCondition);
                    break;
                case OperatingModes.Executing:
                    DefineLightsState(LightTowerState.ProcessActive);
                    break;
                case OperatingModes.Engineering:
                    DefineLightsState(LightTowerState.EngineeringMode);
                    break;
            }
        }

        #endregion

        #region Device Commands

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            try
            {
                if (CurrentActivity != null)
                {
                    AbortActivity();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            try
            {
                if (_waferFlow != null)
                {
                    _waferFlow.SubstrateIdReadingHasBeenFinished -=
                        WaferFlow_SubstrateIdReadingHasBeenFinished;
                    _waferFlow.WaferAlignStart -= WaferFlow_WaferAlignStart;
                    _waferFlow.WaferAlignEnd -= WaferFlow_WaferAlignEnd;
                    _waferFlow.LastWaferEntry -= WaferFlow_LastWaferEntry;
                    _waferFlow.JobStatusChanged -= WaferFlowJobStatusChanged;
                    _waferFlow.OnTransitionChanged -= WaferFlow_OnTransitionChanged;
                    if (Jobs.Count != 0)
                    {
                        foreach (var job in Jobs)
                        {
                            _waferFlow.AbortJob(job.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            try
            {
                if (_waferFlowActivityManager.Activity is { IsStarted: true })
                {
                    _waferFlowActivityManager.Abort();
                    _waferFlowActivityManager.Remove();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            SetState(OperatingModes.Maintenance);
            base.InternalInterrupt(interruption, interruptedExecution);
        }

        protected virtual void InternalClean() => StartClearActivity();

        protected virtual void InternalLoadProcessModule(
            IMaterialLocationContainer loadPort,
            byte sourceSlot,
            RobotArm robotArm,
            Angle alignAngle,
            AlignType alignType,
            EffectorType effectorType,
            IMaterialLocationContainer processModule)
        {
            var config = new LoadToPmConfiguration(this.GetEquipment(), sourceSlot, effectorType);
            config.SetLoadPort((LoadPort)loadPort);
            config.SetRobotArm(robotArm);
            config.SetAlignAngle(alignAngle);
            config.SetAlignType(alignType);
            config.SetProcessModule((DriveableProcessModule)processModule);
            StartLoadProcessModuleActivity(config);
        }

        protected virtual void InternalUnloadProcessModule(
            IMaterialLocationContainer processModule,
            RobotArm robotArm,
            EffectorType effectorType,
            IMaterialLocationContainer loadPort,
            byte destinationSlot)
        {
            var config = new UnloadFromPmConfiguration(
                this.GetEquipment(),
                destinationSlot,
                effectorType);
            config.SetLoadPort((LoadPort)loadPort);
            config.SetRobotArm(robotArm);
            config.SetProcessModule((DriveableProcessModule)processModule);
            StartUnloadProcessModuleActivity(config);
        }

        protected override void InternalInitialize(bool mustForceInit)
        {
            base.InternalInitialize(mustForceInit);
            RunInitActivity(mustForceInit);

            //Start wafer flow activity if it is not started
            if (_waferFlowActivityManager.Activity == null)
            {
                _waferFlow = new WaferFlow(this);
                _waferFlow.SubstrateIdReadingHasBeenFinished +=
                    WaferFlow_SubstrateIdReadingHasBeenFinished;
                _waferFlow.WaferAlignStart += WaferFlow_WaferAlignStart;
                _waferFlow.WaferAlignEnd += WaferFlow_WaferAlignEnd;
                _waferFlow.LastWaferEntry += WaferFlow_LastWaferEntry;
                _waferFlow.JobStatusChanged += WaferFlowJobStatusChanged;
                _waferFlow.OnTransitionChanged += WaferFlow_OnTransitionChanged;
                _waferFlowActivityManager.Start(_waferFlow);
            }
        }

        protected virtual void InternalRequestManualMode() => _operatingModeSm?.RequestManualMode();

        protected virtual void InternalRequestEngineeringMode()
            => _operatingModeSm?.RequestEngineeringMode();

        protected virtual void InternalCreateJob(Job job) => _waferFlow?.AddJob(job);

        protected virtual void InternalStartJobExecution(Job job) => _waferFlow?.StartJob(job);

        protected virtual void InternalPause(string jobName) => _waferFlow?.Pause(jobName);

        protected virtual void InternalResume(string jobName) => _waferFlow?.Resume(jobName);

        protected virtual void InternalStop(string jobName, StopConfig stopConfig)
            => _waferFlow?.StopJob(jobName, stopConfig);

        #endregion

        #region Event Handlers

        private void WaferFlowJobStatusChanged(object sender, JobStatusChangedEventArgs e)
            => OnJobStatusChanged(e.Job);

        private void WaferFlow_LastWaferEntry(object sender, System.EventArgs e)
            => OnLastWaferEntry();

        private void WaferFlow_WaferAlignStart(object sender, System.EventArgs e)
            => OnWaferAlignStart();

        private void WaferFlow_WaferAlignEnd(object sender, System.EventArgs e)
            => OnWaferAlignEnd();

        private void WaferFlow_SubstrateIdReadingHasBeenFinished(
            object sender,
            SubstrateIdReadingHasBeenFinishedEventArgs e)
        {
            if (e != null)
            {
                OnSubstrateIdReadingHasBeenFinished(e.IsSuccess, e.SubstrateId, e.AcquiredId);
            }
        }

        private void MaterialManager_MaterialMoved(object sender, MaterialMovedEventArgs e)
        {
            var substrate = e.Material as Substrate;
            if (substrate == null)
            {
                return;
            }

            substrate.UpdateHistory(e.DateTime, e.NewLocation as SubstrateLocation);
            _userLogger.Info(
                $"Material {substrate.SimplifiedName} has moved from {e.OldLocation?.Name} to {e.NewLocation?.Name}");
        }

        private void OperatingModeSm_OnStateChanged(object sender, StateChangeArgs e)
        {
            if (!e.Activated)
            {
                return;
            }

            switch (e.Name)
            {
                case nameof(OperatingModes.Maintenance):
                    SetState(OperatingModes.Maintenance);
                    if (_waferFlow != null)
                    {
                        Interrupt(InterruptionKind.Abort);
                    }

                    var dataFlowManager =
                        this.AllOfType<IAbstractDataFlowManager>().FirstOrDefault();
                    dataFlowManager?.SwitchToManualMode();
                    break;
                case nameof(OperatingModes.Initialization):
                    SetState(OperatingModes.Initialization);
                    break;
                case nameof(OperatingModes.Idle):
                    SetState(OperatingModes.Idle);
                    break;
                case nameof(OperatingModes.Executing):
                    SetState(OperatingModes.Executing);
                    break;
                case nameof(OperatingModes.Engineering):
                    SetState(OperatingModes.Engineering);
                    break;
            }

            UpdateLightTower();
        }

        private void WaferFlow_OnTransitionChanged(object sender, TransitionChangeArgs e)
            => CurrentActivityStep = e.Destination;

        protected override void HandleCommandExecutionStateChanged(CommandExecutionEventArgs e)
        {
            //Do nothing, State is handled by Operating Mode SM
        }

        private void ThroughputStatistics_ThroughputValueChanged(
            object sender,
            ThroughputEventArgs e)
            => SubstrateThroughput = e.Throughput;

        private void OperatingModeSm_UserInformationRaised(
            object sender,
            UserInformationEventArgs e)
            => OnUserInformationRaised(e.Message);

        private void OperatingModeSm_UserWarningRaised(object sender, UserInformationEventArgs e)
            => OnUserWarningRaised(e.Message);

        private void OperatingModeSm_UserErrorRaised(object sender, UserInformationEventArgs e)
            => OnUserErrorRaised(e.Message);

        #endregion
    }
}
