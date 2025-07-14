using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.AlarmModeling;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule.
    Configuration;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces;

using Alarm = Agileo.AlarmModeling.Alarm;
using TransferType = UnitySC.Shared.TC.Shared.Data.TransferType;

namespace UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule
{
    public partial class ToolControlProcessModule
        : IConfigurableDevice<ToolControlProcessModuleConfiguration>
    {
        #region Fields

        private ToolControlDriver Driver { get; set; }

        private TaskCompletionSource<bool> _tcsReadyToLoadUnload;
        private bool _setupPhaseInProgress;

        #endregion

        #region Properties

        public List<ProcessJob> ProcessJobs { get; } = new();

        #endregion

        #region Setup

        private void InstanceInitialization()
        {
            TransferState = EnumPMTransferState.NotReady;
            IsDoorOpen = false;
            IsReadyToLoadUnload = false;
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    _setupPhaseInProgress = true;
                    break;
                case SetupPhase.SettingUp:
                    break;
                case SetupPhase.SetupDone:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        Driver.ClientConnectionStateChanged += Driver_ClientConnectionStateChanged;
                        Driver.ModuleStateReceived += Driver_ModuleStateReceived;
                        Driver.ReadyForSubstrateLoadRecieved += Driver_ReadyToLoadReceived;
                        Driver.ReadyForSubstrateUnloadRecieved += Driver_ReadyToUnloadReceived;
                        Driver.TriggerSubstratePresentReceived +=
                            Driver_TriggerSubstratePresentReceived;
                    }

                    ProcessModuleName = Name;
                    _setupPhaseInProgress = false;
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
                TransferState = EnumPMTransferState.NotReady;
                IsDoorOpen = false;
                IsReadyToLoadUnload = false;
                if (Driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                //TODO Add in the interface
                //Driver.RaiseOnInitialize(Configuration.ModuleId);
                var currentModuleState = ModuleState.Error;
                Driver.RaiseOnGetProcessModuleState(Configuration.ModuleId, ref currentModuleState);
                UpdateModuleState(currentModuleState);
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
            TransferState = EnumPMTransferState.NotReady;
            IsDoorOpen = false;
            IsReadyToLoadUnload = false;
            base.InternalInterrupt(interruption, interruptedExecution);

            //TODO Add in the interface
            //Driver?.RaiseOnAbortModule(Configuration.ModuleId);
        }

        #endregion

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            Driver?.Start();
            UpdateIsCommunicationStartedStatus();
        }

        protected override void InternalStopCommunication()
        {
            Driver?.Dispose();
            UpdateIsCommunicationStartedStatus();
        }

        #endregion

        #region Commands

        protected override void InternalPrepareTransfer(TransferType transferType, RobotArm arm, MaterialType materialType, SampleDimension dimension)
        {
            switch (transferType)
            {
                case TransferType.Pick
                    when TransferState == EnumPMTransferState.ReadyToUnload_SlitDoorOpened:
                case TransferType.Place
                    when TransferState == EnumPMTransferState.ReadyToLoad_SlitDoorOpened:
                    return;
            }

            TransferState = EnumPMTransferState.NotReady;
            IsDoorOpen = false;
            IsReadyToLoadUnload = false;
            if (Driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            _tcsReadyToLoadUnload = new TaskCompletionSource<bool>();
            switch (transferType)
            {
                case TransferType.Pick:
                    Driver.RaiseOnPrepareForSubstrateUnload(Configuration.ModuleId, (int)arm);
                    if (!_tcsReadyToLoadUnload.Task.Wait(30000))
                    {
                        _tcsReadyToLoadUnload?.TrySetResult(false);
                        _tcsReadyToLoadUnload = null;
                        throw new InvalidOperationException($"{Name} is not ready for transfer.");
                    }

                    break;
                case TransferType.Place:
                    Driver.RaiseOnPrepareForSubstrateLoad(Configuration.ModuleId, (int)arm);
                    if (!_tcsReadyToLoadUnload.Task.Wait(30000))
                    {
                        _tcsReadyToLoadUnload?.TrySetResult(false);
                        _tcsReadyToLoadUnload = null;
                        throw new InvalidOperationException($"{Name} is not ready for transfer.");
                    }

                    break;
            }
        }

        protected override void InternalPostTransfer()
        {
            //Do nothing
        }

        protected override void InternalSelectRecipe(Wafer wafer)
        {
            try
            {
                if (Driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                var processJob = GetProcessJobFromWafer(wafer);
                if (processJob == null)
                {
                    throw new InvalidOperationException(
                        $"Process job for wafer with Id {wafer.SubstrateId} not found");
                }

                var flowRecipe = GetFlowRecipeFromWafer(wafer);
                if (flowRecipe == null)
                {
                    throw new InvalidOperationException(
                        $"Flow recipe for wafer with Id {wafer.SubstrateId} not found");
                }

                //Useful to ensure that process module is really ready
                Thread.Sleep(500);
                Driver.RaiseOnSelectModuleRecipe(
                    processJob,
                    Configuration.ModuleId,
                    flowRecipe.ModuleRecipeId);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStartRecipe()
        {
            try
            {
                if (Driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                var processJob = GetProcessJobFromWafer(Location.Wafer);
                if (processJob == null)
                {
                    throw new InvalidOperationException(
                        $"Process job for wafer with Id {Location.Wafer.SubstrateId} not found");
                }

                var substrate = GetSubstrateFromWafer(Location.Wafer);
                if (substrate == null)
                {
                    throw new InvalidOperationException(
                        $"Substrate for wafer with Id {Location.Wafer.SubstrateId} not found");
                }

                var flowRecipe = GetFlowRecipeFromWafer(Location.Wafer);
                if (flowRecipe == null)
                {
                    throw new InvalidOperationException(
                        $"Flow recipe for wafer with Id {Location.Wafer.SubstrateId} not found");
                }

                Thread.Sleep(1000);
                Logger.Debug(
                    $"Start recipe for PJ with name = {processJob.Name}, GUID = {processJob.Id}, for module {ProcessModuleName} with GUID {Configuration.ModuleId} for substrate with name {substrate.Name} and GUID {substrate.Id}");
                Driver.RaiseOnExecuteModuleRecipe(
                    processJob,
                    Configuration.ModuleId,
                    flowRecipe.ModuleRecipeId,
                    substrate);
                SelectedRecipe = flowRecipe.ModuleRecipeName;
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalAbortRecipe()
        {
            try
            {
                if (Driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                //TODO Add in the interface
                //Driver.RaiseOnAbortProcessModuleRecipe(Configuration.ModuleId);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalResetSmokeDetectorAlarm()
        {
            //Do nothing in case of ToolControl process modules
        }

        #endregion

        #region Public Methods

        public override IEnumerable<Alarm> GetActiveAlarms()
        {
            return Alarms.Where(a => a.State == AlarmState.Set);
        }

        public override IEnumerable<EquipmentConstant> GetEquipmentConstants(List<int> ids)
        {
            //TODO to be implemented when driver updated
            return new List<EquipmentConstant>();
        }

        public override bool SetEquipmentConstant(EquipmentConstant equipmentConstant)
        {
            //TODO to be implemented when driver updated
            return false;
        }

        public override IEnumerable<StatusVariable> GetStatusVariables(List<int> ids)
        {
            //TODO to be implemented when driver updated
            return new List<StatusVariable>();
        }

        public override IEnumerable<CommonEvent> GetCollectionEvents()
        {
            //TODO to be implemented when driver updated
            return new List<CommonEvent>();
        }

        public override double GetAlignmentAngle()
        {
            if (Driver == null)
            {
                return Double.NaN;
            }

            return Driver.RaiseOnGetModuleAlignmentAngle(Configuration.ModuleId);
        }

        protected override void UpdateSupportedWaferDimensions()
        {
            TransferValidationState = true;
            SupportedSampleDimensions = new List<SampleDimension>()
            {
                SampleDimension.S100mm,
                SampleDimension.S150mm,
                SampleDimension.S200mm,
                SampleDimension.S300mm,
                SampleDimension.S450mm
            };
        }

        public void AddProcessJob(ProcessJob job)
        {
            ProcessJobs.Add(job);
        }

        public void RemoveProcessJob(ProcessJob job)
        {
            ProcessJobs.Remove(job);
        }

        public void SetDriver(ToolControlDriver driver)
        {
            Driver = driver;
        }

        public void SetProcessModuleName(string processModuleName)
        {
            ProcessModuleName = processModuleName;
        }

        #endregion

        #region Private Methods

        private void UpdateIsCommunicatingStatus()
        {
            IsCommunicating = Driver is { IsClientConnected: true }
                              || ExecutionMode == ExecutionMode.Simulated;
        }

        private void UpdateIsCommunicationStartedStatus()
        {
            IsCommunicationStarted = Driver is { IsStarted: true }
                                     || ExecutionMode == ExecutionMode.Simulated;
        }

        protected override void LoadMaterial(Wafer wafer)
        {
            if (ExecutionMode == ExecutionMode.Simulated || _setupPhaseInProgress)
            {
                return;
            }

            if (Driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            if (TransferState != EnumPMTransferState.ReadyToLoad_SlitDoorOpened)
            {
                InternalPrepareTransfer(TransferType.Place, RobotArm.Arm1, wafer.MaterialType, wafer.MaterialDimension);
            }

            Driver.RaiseOnSubstrateLoaded(Configuration.ModuleId, 0);
            Thread.Sleep(500);
            var substrate = GetSubstrateFromWafer(wafer)
                            ?? new Substrate(wafer.SubstrateId, wafer.LotId, wafer.AcquiredId);
            Driver.RaiseOnModuleSubstratePresent(Configuration.ModuleId, substrate);
        }

        protected override void UnloadMaterial(Wafer wafer)
        {
            if (ExecutionMode == ExecutionMode.Simulated || _setupPhaseInProgress)
            {
                return;
            }

            if (Driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            if (TransferState != EnumPMTransferState.ReadyToUnload_SlitDoorOpened)
            {
                InternalPrepareTransfer(TransferType.Pick, RobotArm.Arm1, wafer.MaterialType, wafer.MaterialDimension);
            }

            Driver.RaiseOnSubstrateUnloaded(Configuration.ModuleId, 0);
            Thread.Sleep(500);
            var substrate = GetSubstrateFromWafer(wafer)
                            ?? new Substrate(wafer.SubstrateId, wafer.LotId, wafer.AcquiredId);
            Driver.RaiseOnModuleSubstrateRemoved(Configuration.ModuleId, substrate);
        }

        private void UpdateModuleState(ModuleState moduleState)
        {
            switch (moduleState)
            {
                case ModuleState.Stopped:
                case ModuleState.Error:
                case ModuleState.Suspended:
                    ProcessModuleState = ProcessModuleState.Error;
                    SetState(OperatingModes.Maintenance);
                    break;
                case ModuleState.Initializing:
                    ProcessModuleState = ProcessModuleState.Initializing;
                    SetState(OperatingModes.Initialization);
                    break;
                case ModuleState.Idle:
                    ProcessModuleState = ProcessModuleState.Idle;
                    SetState(OperatingModes.Idle);
                    OnReadyToTransfer();
                    break;
                case ModuleState.Pending:
                    ProcessModuleState = ProcessModuleState.Offline;
                    SetState(OperatingModes.Maintenance);
                    break;
                case ModuleState.Executing:
                    ProcessModuleState = ProcessModuleState.Active;
                    SetState(OperatingModes.Executing);
                    break;
                case ModuleState.Stopping:
                    ProcessModuleState = ProcessModuleState.ShuttingDown;
                    SetState(OperatingModes.Maintenance);
                    break;
            }
        }

        private Substrate GetSubstrateFromWafer(Wafer wafer)
        {
            var materialCarrier = GetMaterialCarrierFromWafer(wafer);
            if (materialCarrier == null)
            {
                return null;
            }

            var slot =
                materialCarrier.Slots.FirstOrDefault(x => x.Substrate.Name == wafer.SubstrateId);
            if (slot == null)
            {
                return null;
            }

            return slot.Substrate;
        }

        private ProcessJob GetProcessJobFromWafer(Wafer wafer)
        {
            return ProcessJobs.FirstOrDefault(x => x.Name == wafer.ProcessJobId);
        }

        private IMaterialCarrier GetMaterialCarrierFromWafer(Wafer wafer)
        {
            var processJob = GetProcessJobFromWafer(wafer);
            if (processJob == null)
            {
                return null;
            }

            return processJob.MaterialCarriers.FirstOrDefault(x => x.Name == wafer.CarrierId);
        }

        private IFlowRecipeItem GetFlowRecipeFromWafer(Wafer wafer)
        {
            var processJob = GetProcessJobFromWafer(wafer);
            if (processJob == null)
            {
                return null;
            }

            return processJob.FlowRecipe.FirstOrDefault(
                x => x.ProcessModuleId == Configuration.ModuleId);
        }

        #endregion

        #region Event Handlers

        private void Driver_ClientConnectionStateChanged(bool isConnected)
        {
            Task.Run(
                () =>
                {
                    UpdateIsCommunicationStartedStatus();
                    UpdateIsCommunicatingStatus();
                    var processModules = Driver.RaiseOnGetAvailableModules();
                    foreach (var module in processModules)
                    {
                        if (module.Id == Configuration.ModuleId)
                        {
                            WaferPresence = module.IsSubstratePresent
                                ? WaferPresence.Present
                                : WaferPresence.Absent;
                        }
                    }
                });
        }

        private void Driver_ReadyToUnloadReceived(string moduleId, bool success)
        {
            if (moduleId != Configuration.ModuleId)
            {
                return;
            }

            Task.Run(
                () =>
                {
                    Logger.Debug($"ReadyToUnloadReceived with success = {success}");
                    if (success)
                    {
                        TransferState = EnumPMTransferState.ReadyToUnload_SlitDoorOpened;
                        IsDoorOpen = true;
                        IsReadyToLoadUnload = true;
                        _tcsReadyToLoadUnload?.TrySetResult(true);
                        OnReadyToTransfer();
                    }
                    else
                    {
                        _tcsReadyToLoadUnload?.TrySetResult(false);
                    }
                });
        }

        private void Driver_ReadyToLoadReceived(string moduleId, bool success)
        {
            if (moduleId != Configuration.ModuleId)
            {
                return;
            }

            Task.Run(
                () =>
                {
                    Logger.Debug($"ReadyToLoadReceived with success = {success}");
                    if (success)
                    {
                        TransferState = EnumPMTransferState.ReadyToLoad_SlitDoorOpened;
                        IsDoorOpen = true;
                        IsReadyToLoadUnload = true;
                        _tcsReadyToLoadUnload?.TrySetResult(true);
                        OnReadyToTransfer();
                    }
                    else
                    {
                        _tcsReadyToLoadUnload?.TrySetResult(false);
                    }
                });
        }

        private void Driver_ModuleStateReceived(string moduleId, ModuleState state)
        {
            if (moduleId != Configuration.ModuleId)
            {
                return;
            }

            Task.Run(
                () =>
                {
                    TransferState = EnumPMTransferState.NotReady;
                    IsDoorOpen = false;
                    IsReadyToLoadUnload = false;
                    UpdateModuleState(state);
                });
        }

        private void Driver_TriggerSubstratePresentReceived(string moduleId, bool value)
        {
            if (moduleId != Configuration.ModuleId)
            {
                return;
            }

            Task.Run(
                () =>
                {
                    WaferPresence = value
                        ? WaferPresence.Present
                        : WaferPresence.Absent;
                    CheckSubstrateDetectionError();
                });
        }

        #endregion

        #region IConfigurableDevice

        public new ToolControlProcessModuleConfiguration Configuration
            => base.Configuration.Cast<ToolControlProcessModuleConfiguration>();

        public ToolControlProcessModuleConfiguration CreateDefaultConfiguration()
        {
            return new ToolControlProcessModuleConfiguration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(ProcessModule)}/{nameof(ToolControlProcessModule)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Driver != null)
                {
                    Driver.ClientConnectionStateChanged -= Driver_ClientConnectionStateChanged;
                    Driver.ModuleStateReceived -= Driver_ModuleStateReceived;
                    Driver.ReadyForSubstrateLoadRecieved -= Driver_ReadyToLoadReceived;
                    Driver.ReadyForSubstrateUnloadRecieved -= Driver_ReadyToUnloadReceived;
                    Driver.TriggerSubstratePresentReceived -=
                        Driver_TriggerSubstratePresentReceived;
                    Driver.Dispose();
                    Driver = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
