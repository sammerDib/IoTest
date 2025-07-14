using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.AlarmModeling;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;

using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.Configuration;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.Driver;
using UnitySC.DataFlow.ProcessModules.Drivers.WCF;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

using Alarm = Agileo.AlarmModeling.Alarm;
using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule
{
    public partial class UnityProcessModule : IConfigurableDevice<UnityProcessModuleConfiguration>
    {
        #region Fields

        private IProcessModuleIos IoModule { get; set; }

        private UnityProcessModuleDriver WcfDriver { get; set; }

        private bool IsWcfDriverConnected
        {
            get
            {
                if (ExecutionMode == ExecutionMode.Real)
                {
                    return WcfDriver is { IsConnected: true };
                }

                return true;
            }
        }

        private bool _firstConnection = true;
        private List<UnitySC.Shared.TC.Shared.Data.Alarm> _processModuleAlarms;
        private const int WcfAlarmIdOffSet = 100;

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            //Do nothing
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    var ioModule = this.GetTopDeviceContainer()
                        .AllDevices()
                        .FirstOrDefault(d => d is IProcessModuleIos);
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        WcfDriver = new UnityProcessModuleDriver(
                            Configuration.WcfConfiguration,
                            Logger);
                        WcfDriver.CommunicationEstablished += WcfDriver_CommunicationEstablished;
                        WcfDriver.CommunicationClosed += WcfDriver_CommunicationClosed;
                        WcfDriver.AlarmCleared += WcfDriver_AlarmCleared;
                        WcfDriver.AlarmRaised += WcfDriver_AlarmRaised;
                        WcfDriver.EquipmentConstantChanged += WcfDriver_EquipmentConstantChanged;
                        WcfDriver.EventFired += WcfDriver_EventFired;
                        WcfDriver.PmReadyToTransfer += WcfDriver_PmReadyToTransfer;
                        WcfDriver.StatusVariableChanged += WcfDriver_StatusVariableChanged;
                    }

                    if (ioModule is not IProcessModuleIos processModuleIos)
                    {
                        throw new InvalidOperationException(
                            $"Mandatory device of type {nameof(IProcessModuleIos)} is not found in equipment model tree.");
                    }

                    IoModule = processModuleIos;

                    if (IoModule != null)
                    {
                        IoModule.StatusValueChanged += IoModule_StatusValueChanged;
                    }

                    if (this.TryGetAlarmCenter(out var alarmCenter))
                    {
                        alarmCenter.Services.AlarmOccurrenceStateChanged +=
                            Services_AlarmOccurrenceStateChanged;
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                if (!WcfDriver.Initialization().Result)
                {
                    throw new InvalidOperationException($"{Name} initialization failed");
                }

                GetStatusVariables(new List<int>());
                UpdateSupportedWaferDimensions();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (IoModule != null)
            {
                IoModule.Interrupt(InterruptionKind.Abort);
            }
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            try
            {
                WcfDriver?.Connect();
                if (IoModule is { IsCommunicationStarted: false })
                {
                    IoModule.StartCommunication();
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStopCommunication()
        {
            try
            {
                WcfDriver?.Disconnect();
                if (IoModule != null)
                {
                    IoModule.StopCommunication();
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion ICommunicatingDevice Commands

        #region Commands

        protected override void InternalPrepareTransfer(
            TransferType transferType,
            RobotArm arm,
            MaterialType materialType,
            SampleDimension dimension)
        {
            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            if (!WcfDriver.PrepareForTransfer(
                        transferType,
                        new MaterialTypeInfo()
                        {
                            MaterialType = (int)materialType,
                            WaferDimension = WcfHelper.ConvertSampleDimensionToLength(dimension)
                        })
                    .Result)
            {
                throw new InvalidOperationException($"{Name} is not ready for transfer.");
            }

            if (!TransferValidationState)
            {
                throw new InvalidOperationException($"Transfer in {Name} is not authorized.");
            }
        }

        protected override void InternalPostTransfer()
        {
            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            WcfDriver.PostTransfer();
            CheckSubstrateDetectionError();
        }

        protected override void InternalSelectRecipe(Wafer wafer)
        {
            //Do nothing in case of Unity Process Module
        }

        protected override void InternalStartRecipe()
        {
            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            WcfDriver.StartRecipe();
        }

        protected override void InternalAbortRecipe()
        {
            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            WcfDriver.AbortRecipe();
        }

        protected override void InternalResetSmokeDetectorAlarm()
        {
            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            if (_processModuleAlarms.FirstOrDefault(a => a.Name == nameof(ErrorID.SmokeDetected)) is
                {
                    Active: true
                } alarm)
            {
                var alarmId = alarm.ID + WcfAlarmIdOffSet;
                ClearAlarmById(alarmId.ToString());
            }
        }

        #endregion

        #region Public Methods

        public override IEnumerable<Alarm> GetActiveAlarms()
        {
            return Alarms.Where(a => a.State == AlarmState.Set);
        }

        public override IEnumerable<EquipmentConstant> GetEquipmentConstants(List<int> ids)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return SimulateGetEquipmentConstants(ids);
            }

            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            //Return the entire list if ids list is null or empty
            if (ids == null || ids.Count == 0)
            {
                return WcfDriver?.ECGetAllRequest().Result;
            }

            return WcfDriver?.ECGetRequest(ids).Result;
        }

        public override bool SetEquipmentConstant(EquipmentConstant equipmentConstant)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return true;
            }

            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            return WcfDriver.ECSetRequest(equipmentConstant).Result;
        }

        public override IEnumerable<StatusVariable> GetStatusVariables(List<int> ids)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return SimulateGetStatusVariables(ids);
            }

            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            List<StatusVariable> statusVariables;

            //Return the entire list if ids list is null or empty
            if (ids == null || ids.Count == 0)
            {
                statusVariables = WcfDriver?.SVGetAllRequest().Result;
            }
            else
            {
                statusVariables = WcfDriver?.SVGetRequest(ids).Result;
            }

            OnStatusVariableChanged(statusVariables);
            return statusVariables;
        }

        public override IEnumerable<CommonEvent> GetCollectionEvents()
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return SimulateGetCollectionEvents();
            }

            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            return WcfDriver.GetAll().Result;
        }

        public override double GetAlignmentAngle()
        {
            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            return WcfDriver.GetAlignmentAngle().Result;
        }

        #endregion

        #region Private

        private void UpdateIsCommunicatingStatus()
        {
            var ioModuleIsCommunicatingStatusValue = false;
            if (IoModule is not null)
            {
                ioModuleIsCommunicatingStatusValue = IoModule.IsCommunicating;
            }

            IsCommunicating = ioModuleIsCommunicatingStatusValue && IsWcfDriverConnected;
        }

        private void UpdateIsCommunicationStartedStatus()
        {
            var ioModuleIsCommunicationStartedStatusValue = false;
            if (IoModule is not null)
            {
                ioModuleIsCommunicationStartedStatusValue = IoModule.IsCommunicationStarted;
            }

            IsCommunicationStarted =
                ioModuleIsCommunicationStartedStatusValue && IsWcfDriverConnected;
        }

        private void RefreshInterlocksStatuses()
        {
            switch (InstanceId)
            {
                case 1:
                    IsDoorOpen = IoModule.I_PM1_DoorOpened;
                    IsReadyToLoadUnload = IoModule.I_PM1_ReadyToLoadUnload;
                    break;
                case 2:
                    IsDoorOpen = IoModule.I_PM2_DoorOpened;
                    IsReadyToLoadUnload = IoModule.I_PM2_ReadyToLoadUnload;
                    break;
            }
        }
        #endregion

        #region Overrides

        protected override void HandleAlarmStateChanged(AlarmStateChangedEventArgs e)
        {
            if (_processModuleAlarms?.FirstOrDefault(
                    a => a.ID == e.Occurrence.Alarm.RelativeId - WcfAlarmIdOffSet) is { } alarm)
            {
                if (alarm.Active && !alarm.Acknowledged && alarm.Level == AlarmCriticality.Critical)
                {
                    //Aborts the current activity
                    Interrupt(InterruptionKind.Abort);

                    //Set the device in maintenance mode
                    if (State != OperatingModes.Maintenance)
                    {
                        SetState(OperatingModes.Maintenance);
                    }
                }

                return;
            }

            base.HandleAlarmStateChanged(e);
        }

        public override void LoadAlarms(
            IAlarmCenter alarmCenter,
            string relativeConfigurationDirectory,
            string deviceConfigRootPath = "")
        {
            if (ErrorProvider != null)
            {
                return;
            }

            if (string.IsNullOrEmpty(deviceConfigRootPath))
            {
                deviceConfigRootPath =
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            }

            var deviceResourcesDir = Path.GetFullPath(
                Path.Combine(deviceConfigRootPath, relativeConfigurationDirectory));
            var csvErrorFilePattern = Path.Combine(
                deviceResourcesDir,
                $"{{0}}_{nameof(UnityProcessModule)}.csv");
            ErrorProvider = this.LoadAlarms(
                alarmCenter,
                csvErrorFilePattern,
                $"{Name}_ErrorProvider",
                Logger);
        }

        protected override void HandleCommandExecutionStateChanged(CommandExecutionEventArgs e)
        {
            //The fact that PrepareTransfer command failed is normal because the process module is saying that it is not ready yet
            if (e.Execution.Context.Command.Name == nameof(IUnityProcessModule.PrepareTransfer)
                && e.NewState == ExecutionState.Failed)
            {
                CurrentCommand = string.Empty;
                switch (State)
                {
                    case OperatingModes.Maintenance:
                    case OperatingModes.Idle:
                        //Do nothing
                        break;
                    case OperatingModes.Initialization:
                        SetState(
                            e.NewState == ExecutionState.Success
                                ? OperatingModes.Idle
                                : OperatingModes.Maintenance);
                        break;
                    case OperatingModes.Executing:
                        SetState(
                            PreviousState == OperatingModes.Maintenance
                                ? OperatingModes.Maintenance
                                : OperatingModes.Idle);
                        break;
                }

                return;
            }

            base.HandleCommandExecutionStateChanged(e);
        }

        protected override void LoadMaterial(Wafer wafer)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return;
            }

            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            WcfDriver.LoadMaterial(WcfHelper.ToWcfMaterial(wafer));
        }

        protected override void UnloadMaterial(Wafer wafer)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return;
            }

            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            WcfDriver.UnloadMaterial();
        }

        protected override void UpdateSupportedWaferDimensions()
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return;
            }

            if (WcfDriver is not { IsConnected: true })
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            SupportedSampleDimensions =
                WcfHelper.ConvertLengthsToSampleDimensions(
                    WcfDriver.GetSupportedWaferDimensions().Result);
        }

        protected void UpdateWaferPresence(string value)
        {
            if (Enum.TryParse(value, out MaterialPresence materialPresence))
            {
                switch (materialPresence)
                {
                    case MaterialPresence.Unknown:
                        WaferPresence = WaferPresence.Unknown;
                        break;
                    case MaterialPresence.Present:
                        WaferPresence = WaferPresence.Present;
                        break;
                    case MaterialPresence.NotPresent:
                        WaferPresence = WaferPresence.Absent;
                        break;
                }
            }
        }

        protected void UpdateProcessModuleState(string value)
        {
            if (Enum.TryParse(value, out ProcessModuleState newProcessModuleState))
            {
                UpdateProcessModuleState(newProcessModuleState);
            }
        }

        protected void UpdateTransferState(string value)
        {
            if (Enum.TryParse(value, out EnumPMTransferState newTransferState))
            {
                TransferState = newTransferState;
            }
        }

        protected void UpdateTransferValidationState(string value)
        {
            if (bool.TryParse(value, out bool result))
            {
                TransferValidationState = result;
            }
        }
        #endregion

        #region Event Handlers

        private void IoModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status.Name)
            {
                case nameof(IoModule.I_PM1_DoorOpened):
                case nameof(IoModule.I_PM2_DoorOpened):
                case nameof(IoModule.I_PM1_ReadyToLoadUnload):
                case nameof(IoModule.I_PM2_ReadyToLoadUnload):
                    RefreshInterlocksStatuses();
                    break;

                //Same for IsCommunicating and IsCommunicationStarted
                case nameof(IoModule.IsCommunicating):
                    UpdateIsCommunicatingStatus();
                    if (!IoModule.IsCommunicating)
                    {
                        ResetInterlocksStatuses();
                    }
                    else
                    {
                        RefreshInterlocksStatuses();
                    }

                    break;
                case nameof(IoModule.IsCommunicationStarted):
                    UpdateIsCommunicationStartedStatus();
                    break;
            }

            if (State is not OperatingModes.Maintenance
                && (!IsCommunicating || IoModule.State == OperatingModes.Maintenance))
            {
                SetState(OperatingModes.Maintenance);
            }
        }

        private bool WcfDriver_StatusVariableChanged(
            object sender,
            StatusVariableChangedEventArgs e)
        {
            OnStatusVariableChanged(e.StatusVariables);
            return true;
        }

        private void WcfDriver_PmReadyToTransfer(object sender, EventArgs e)
        {
            OnReadyToTransfer();
        }

        private void WcfDriver_EventFired(object sender, EventFiredEventArgs e)
        {
            OnCollectionEventRaised(e.CommonEvent);
        }

        private bool WcfDriver_EquipmentConstantChanged(
            object sender,
            EquipmentConstantChangedEventArgs e)
        {
            OnEquipmentConstantChanged(e.EquipmentConstants);
            return true;
        }

        private void WcfDriver_AlarmRaised(object sender, AlarmRaisedEventArgs e)
        {
            foreach (var alarm in e.Alarms)
            {
                SetAlarmById((alarm.ID + WcfAlarmIdOffSet).ToString());
                if (alarm.Name == nameof(ErrorID.SmokeDetected))
                {
                    Task.Run(OnSmokeDetected);
                }
            }
        }

        private void WcfDriver_AlarmCleared(object sender, AlarmClearedEventArgs e)
        {
            foreach (var alarm in e.Alarms)
            {
                ClearAlarmById((alarm.ID + WcfAlarmIdOffSet).ToString());
            }
        }

        private void WcfDriver_CommunicationClosed(object sender, EventArgs e)
        {
            IsCommunicationStarted = false;
            IsCommunicating = false;
            SetState(OperatingModes.Maintenance);
        }

        private void WcfDriver_CommunicationEstablished(object sender, EventArgs e)
        {
            IsCommunicationStarted = true;
            IsCommunicating = true;

            //Need to register alarms only the first time we are connected
            if (!_firstConnection)
            {
                return;
            }

            _processModuleAlarms = WcfDriver.GetAllAlarms().Result;
            if (this.TryGetAlarmCenter(out var alarmCenter))
            {
                var alarmBuilder = alarmCenter.ModelBuilder;
                foreach (var alarm in _processModuleAlarms)
                {
                    var alarmId = alarm.ID + WcfAlarmIdOffSet;
                    var alarmKey = this.FormatAlarmUniqueKey(alarmId.ToString());
                    var deviceAlarm =
                        alarmBuilder.CreateAlarm(alarmKey, alarm.Description, alarmId);
                    Alarms.SafeAdd(deviceAlarm);
                    ErrorProvider.AddError(
                        new ErrorModel(
                            alarmId,
                            alarm.Description,
                            string.Empty,
                            alarm.Level == AlarmCriticality.Critical
                                ? AlarmCriticity.Critical
                                : AlarmCriticity.NonCritical));
                }

                alarmBuilder.AddAlarms(this);
            }

            _firstConnection = false;
        }

        private void Services_AlarmOccurrenceStateChanged(
            object sender,
            AlarmOccurrenceEventArgs args)
        {
            Task.Run(
                () =>
                {
                    if (_processModuleAlarms?.FirstOrDefault(
                            a => a.ID == args.AlarmOccurrence.Alarm.RelativeId - WcfAlarmIdOffSet)
                        is not { } alarm)
                    {
                        return;
                    }

                    alarm.Acknowledged = args.AlarmOccurrence.Acknowledged
                                         && args.AlarmOccurrence.State == AlarmState.Set;
                    alarm.Active = args.AlarmOccurrence.State == AlarmState.Set;
                    WcfDriver.NotifyAlarmChanged(alarm);
                    if (alarm.Active
                        && alarm.Acknowledged
                        && alarm.Level is AlarmCriticality.Information or AlarmCriticality.Warning)
                    {
                        ClearAlarmById(args.AlarmOccurrence.Alarm.RelativeId.ToString());
                    }
                });
        }

        #endregion

        #region Configuration

        public new UnityProcessModuleConfiguration Configuration
            => base.Configuration.Cast<UnityProcessModuleConfiguration>();

        public UnityProcessModuleConfiguration CreateDefaultConfiguration()
        {
            return new UnityProcessModuleConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(ProcessModule)}/{nameof(UnityProcessModule)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion Configuration

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (WcfDriver != null)
                {
                    if (IsCommunicating)
                    {
                        WcfDriver.Disconnect();
                    }

                    WcfDriver.CommunicationEstablished -= WcfDriver_CommunicationEstablished;
                    WcfDriver.CommunicationClosed -= WcfDriver_CommunicationClosed;
                    WcfDriver.AlarmCleared -= WcfDriver_AlarmCleared;
                    WcfDriver.AlarmRaised -= WcfDriver_AlarmRaised;
                    WcfDriver.EquipmentConstantChanged -= WcfDriver_EquipmentConstantChanged;
                    WcfDriver.EventFired -= WcfDriver_EventFired;
                    WcfDriver.PmReadyToTransfer -= WcfDriver_PmReadyToTransfer;
                    WcfDriver.StatusVariableChanged -= WcfDriver_StatusVariableChanged;
                    WcfDriver = null;
                }

                if (IoModule != null)
                {
                    IoModule.StatusValueChanged -= IoModule_StatusValueChanged;
                    IoModule = null;
                }

                if (this.TryGetAlarmCenter(out var alarmCenter))
                {
                    alarmCenter.Services.AlarmOccurrenceStateChanged -=
                        Services_AlarmOccurrenceStateChanged;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
