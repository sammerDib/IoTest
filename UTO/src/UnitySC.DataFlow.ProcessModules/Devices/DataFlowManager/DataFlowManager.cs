using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agileo.AlarmModeling;
using Agileo.Common.Configuration;
using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;

using UnitySC.DataAccess.Dto;
using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager.Configuration;
using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager.Driver;
using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule;
using UnitySC.DataFlow.ProcessModules.Drivers.WCF;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

using Alarm = Agileo.AlarmModeling.Alarm;

namespace UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager
{
    public partial class DataFlowManager : IConfigurableDevice<DataFlowManagerConfiguration>
    {
        #region Fields

        private DataFlowManagerDriver _driver;
        private bool _firstConnection = true;
        private List<UnitySC.Shared.TC.Shared.Data.Alarm> _dataFlowAlarms;
        private const int WcfAlarmIdOffSet = 100;

        #endregion Fields

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    LoadConfiguration();
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        _driver = new DataFlowManagerDriver(Configuration.WcfConfiguration, Logger);
                        _driver.CommunicationEstablished += Driver_CommunicationEstablished;
                        _driver.CommunicationClosed += Driver_CommunicationClosed;
                        _driver.AlarmCleared += Driver_AlarmCleared;
                        _driver.AlarmRaised += Driver_AlarmRaised;
                        _driver.EquipmentConstantChanged += Driver_EquipmentConstantChanged;
                        _driver.EventFired += Driver_EventFired;
                        _driver.StatusVariableChanged += Driver_StatusVariableChanged;
                        _driver.DataFlowRecipeStarted += Driver_DataFlowRecipeStarted;
                        _driver.DataFlowRecipeCompleted += Driver_DataFlowRecipeCompleted;
                        _driver.DataFlowRecipeAdded += Driver_DataFlowRecipeAdded;
                        _driver.DataFlowRecipeDeleted += Driver_DataFlowRecipeDeleted;
                        _driver.ProcessModuleRecipeStarted += Driver_ProcessModuleRecipeStarted;
                        _driver.ProcessModuleRecipeCompleted += Driver_ProcessModuleRecipeCompleted;
                        _driver.ProcessModuleAcquisitionCompleted += Driver_ProcessModuleAcquisitionCompleted;
                        _driver.StopCancelAllJobsRequested += Driver_StopCancelAllJobsRequested;
                        _driver.FdcCollectionChanged += Driver_FdcCollectionChanged;
                    }
                    else
                    {
                        SetUpSimulatedMode();
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

        #endregion Setup

        #region Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            try
            {
                base.InternalInitialize(mustForceInit);
                if (_driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                _driver.Initialize();
                ResetIsStopCancelAllJobsRequested();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStartRecipe(
            MaterialRecipe materialRecipe,
            string processJobId)
        {
            try
            {
                if (_driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                var materials = materialRecipe.Wafers.Select(WcfHelper.ToWcfMaterial).ToList();
                var jobProgram = _driver.StartRecipeDF(
                        materialRecipe.Recipe,
                        processJobId,
                        materials.Select(x => x.GUIDWafer).ToList())
                    .Result;
                foreach (var wafer in materialRecipe.Wafers)
                {
                    wafer.JobProgram = jobProgram;
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalAbortRecipe(string jobId)
        {
            try
            {
                if (_driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                _driver.AbortJobID(jobId);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStartJobOnMaterial(DataflowRecipeInfo recipe, Wafer wafer)
        {
            try
            {
                if (_driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                _driver.StartJob_Material(recipe, WcfHelper.ToWcfMaterial(wafer));
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGetAvailableRecipes()
        {
            try
            {
                if (_driver == null)
                {
                    throw new InvalidOperationException(
                        $"{Name} driver is not instantiated. Cannot execute the command.");
                }

                var actorTypes = new List<ActorType> { ActorType.DataflowManager };
                foreach (var processModule in this.GetEquipment().AllDevices<UnityProcessModule>())
                {
                    actorTypes.Add(processModule.ActorType);
                }

                var recipes = _driver.GetAllDataflowRecipes(actorTypes).Result;
                AvailableRecipes = new List<DataflowRecipeInfo>(recipes);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalStartCommunication()
        {
            try
            {
                _driver?.Connect();
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
                _driver?.Disconnect();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion Commands

        #region Event Handlers

        private void Driver_ProcessModuleAcquisitionCompleted(object sender, ActorRecipeEventArgs e)
        {
            var processModule = this.GetEquipment()
                .AllDevices<UnityProcessModule>()
                .FirstOrDefault(pm => pm.ActorType == e.Actor);

            if (processModule != null)
            {
                OnProcessModuleAcquisitionCompleted(
                    new ProcessModuleRecipeEventArgs(
                        processModule,
                        e.RecipeName,
                        e.RecipeTerminationState));
            }
        }

        private void Driver_ProcessModuleRecipeCompleted(object sender, ActorRecipeEventArgs e)
        {
            var processModule = this.GetEquipment()
                                    .AllDevices<UnityProcessModule>()
                                    .FirstOrDefault(pm=> pm.ActorType == e.Actor);

            if (processModule != null)
            {
                OnProcessModuleRecipeCompleted(
                    new ProcessModuleRecipeEventArgs(
                        processModule,
                        e.RecipeName,
                        e.RecipeTerminationState));
            }
        }

        private void Driver_ProcessModuleRecipeStarted(object sender, ActorRecipeEventArgs e)
        {
            var processModule = this.GetEquipment()
                                    .AllDevices<UnityProcessModule>()
                                    .FirstOrDefault(pm => pm.ActorType == e.Actor);

            if (processModule != null)
            {
                OnProcessModuleRecipeStarted(
                    new ProcessModuleRecipeEventArgs(processModule, e.RecipeName));
            }
        }

        private void Driver_DataFlowRecipeDeleted(object sender, DataFlowRecipeEventArgs e)
        {
            AvailableRecipes.Remove(e.Recipe);
            OnDataFlowRecipeDeleted(e);
        }

        private void Driver_DataFlowRecipeAdded(object sender, DataFlowRecipeEventArgs e)
        {
            AvailableRecipes.Add(e.Recipe);
            OnDataFlowRecipeAdded(e);
        }

        private void Driver_DataFlowRecipeCompleted(object sender, DataFlowRecipeEventArgs e)
        {
            OnDataFlowRecipeCompleted(e);
        }

        private void Driver_DataFlowRecipeStarted(object sender, DataFlowRecipeEventArgs e)
        {
            OnDataFlowRecipeStarted(e);
        }

        private bool Driver_StatusVariableChanged(object sender, StatusVariableChangedEventArgs e)
        {
            if (e.StatusVariables.Find(sv => sv.Name == nameof(DataflowState)) is { } dataFlowState)
            {
                DataflowState = (TC_DataflowStatus)Enum.Parse(
                    typeof(TC_DataflowStatus),
                    dataFlowState.Value.ToString());
            }

            OnStatusVariableChanged(e);
            return true;
        }

        private void Driver_EventFired(object sender, CollectionEventEventArgs e)
        {
            OnCollectionEventRaised(e);
        }

        private bool Driver_EquipmentConstantChanged(
            object sender,
            EquipmentConstantChangedEventArgs e)
        {
            OnEquipmentConstantChanged(e);
            return true;
        }

        private void Driver_AlarmRaised(object sender, AlarmRaisedEventArgs e)
        {
            foreach (var alarm in e.Alarms)
            {
                SetAlarmById((alarm.ID + WcfAlarmIdOffSet).ToString());
            }
        }

        private void Driver_AlarmCleared(object sender, AlarmClearedEventArgs e)
        {
            foreach (var alarm in e.Alarms)
            {
                ClearAlarmById((alarm.ID + WcfAlarmIdOffSet).ToString());
            }
        }

        private void Driver_CommunicationClosed(object sender, EventArgs e)
        {
            IsCommunicationStarted = false;
            IsCommunicating = false;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationEstablished(object sender, EventArgs e)
        {
            IsCommunicationStarted = true;
            IsCommunicating = true;

            //For each connection
            _driver.RequestAllFDCsUpdate();

            //Need to register alarms only the first time we are connected
            if (!_firstConnection)
            {
                return;
            }

            InternalGetAvailableRecipes();
            _dataFlowAlarms = _driver.GetAllAlarms().Result;
            if (this.TryGetAlarmCenter(out var alarmCenter))
            {
                var alarmBuilder = alarmCenter.ModelBuilder;
                foreach (var alarm in _dataFlowAlarms)
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
                    if (_dataFlowAlarms?.FirstOrDefault(
                            a => a.ID == args.AlarmOccurrence.Alarm.RelativeId - WcfAlarmIdOffSet)
                        is not { } alarm)
                    {
                        return;
                    }

                    alarm.Acknowledged = args.AlarmOccurrence.Acknowledged
                                         && args.AlarmOccurrence.State == AlarmState.Set;
                    alarm.Active = args.AlarmOccurrence.State == AlarmState.Set;
                    _driver.NotifyAlarmChanged(alarm);
                    if (alarm.Active
                        && alarm.Acknowledged
                        && alarm.Level is AlarmCriticality.Information or AlarmCriticality.Warning)
                    {
                        ClearAlarmById(args.AlarmOccurrence.Alarm.RelativeId.ToString());
                    }
                });
        }

        private void Driver_StopCancelAllJobsRequested(object sender, EventArgs e)
        {
            IsStopCancelAllJobsRequested = true;
        }

        private void Driver_FdcCollectionChanged(object sender, FdcCollectionEventArgs e)
        {
            OnFdcCollectionChanged(e);
        }

        #endregion Event Handlers

        #region Override

        protected override void HandleAlarmStateChanged(AlarmStateChangedEventArgs e)
        {
            //do nothing
        }

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (DataflowState == TC_DataflowStatus.Maintenance)
            {
                SetState(OperatingModes.Maintenance);
            }
        }

        #endregion Override

        #region Configuration

        public IConfigManager ConfigManager { get; protected set; }

        public DataFlowManagerConfiguration Configuration
            => ConfigManager.Current.Cast<DataFlowManagerConfiguration>();

        public DataFlowManagerConfiguration CreateDefaultConfiguration()
        {
            return new DataFlowManagerConfiguration();
        }

        public string RelativeConfigurationDir => $"./Devices/{nameof(DataFlowManager)}/Resources";

        public void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        public void SetExecutionMode(ExecutionMode executionMode)
        {
            ExecutionMode = executionMode;
        }

        #endregion Configuration

        #region Public Methods

        public override void ResetIsStopCancelAllJobsRequested()
        {
            IsStopCancelAllJobsRequested = false;
            if (DataflowState == TC_DataflowStatus.Maintenance)
            {
                Interrupt(InterruptionKind.Abort);
            }
        }

        public override IEnumerable<Alarm> GetActiveAlarms()
        {
            return Alarms.Where(a => a.State == AlarmState.Set);
        }

        public override IEnumerable<EquipmentConstant> GetEquipmentConstants(List<int> ids)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return new List<EquipmentConstant>()
                {
                    new("MyEc", VidDataType.Boolean, "Ec description", false)
                };
            }

            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            //Return the entire list if ids list is null or empty
            if (ids == null || ids.Count == 0)
            {
                return _driver?.ECGetAllRequest().Result;
            }

            return _driver?.ECGetRequest(ids).Result;
        }

        public override bool SetEquipmentConstant(EquipmentConstant equipmentConstant)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return true;
            }

            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            return _driver.ECSetRequest(equipmentConstant).Result;
        }

        public override IEnumerable<StatusVariable> GetStatusVariables(List<int> ids)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return new List<StatusVariable>()
                {
                    new("MySv", VidDataType.Boolean, "Sv description")
                };
            }

            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            //Return the entire list if ids list is null or empty
            if (ids == null || ids.Count == 0)
            {
                return _driver?.SVGetAllRequest().Result;
            }

            return _driver?.SVGetRequest(ids).Result;
        }

        public override IEnumerable<CommonEvent> GetCollectionEvents()
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return new List<CommonEvent>();
            }

            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            return _driver.GetAll().Result;
        }

        public override void SwitchToManualMode()
        {
            //Do nothing in case of DataFlowManager
        }

        public override void SignalEndOfProcessJob(string processJobId)
        {
            //Do nothing
        }

        public override UTOJobProgram GetJobProgramFromRecipe(DataflowRecipeInfo recipe)
        {
            if (ExecutionMode == ExecutionMode.Simulated)
            {
                return new UTOJobProgram() { PMItems = new List<PMItem>() };
            }

            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            return _driver.GetUTOJobProgramForARecipeDF(recipe).Result;
        }

        public override DataflowRecipeInfo GetRecipeInfo(string recipeName)
        {
            if (AvailableRecipes.Count(r => r.GetRecipePath(Configuration.UseOnlyRecipeNameAsId) == recipeName) > 1)
            {
                var message = $"More than one recipe with the name {recipeName} detected. Cannot select the correct recipe";
                Logger.Error(message);
                OnUserErrorRaised(message);
                throw new InvalidOperationException(message);
            }

            return AvailableRecipes.FirstOrDefault(
                r => r.GetRecipePath(Configuration.UseOnlyRecipeNameAsId) == recipeName);
        }

        public override string GetRecipeName(DataflowRecipeInfo recipe)
        {
            return recipe.GetRecipePath(Configuration.UseOnlyRecipeNameAsId);
        }

        public override List<string> GetRecipeNames()
        {
            return AvailableRecipes.Select(
                r => r.GetRecipePath(Configuration.UseOnlyRecipeNameAsId)).ToList();
        }

        #endregion Public Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_driver != null)
                {
                    if (IsCommunicating)
                    {
                        _driver.Disconnect();
                    }

                    _driver.CommunicationEstablished -= Driver_CommunicationEstablished;
                    _driver.CommunicationClosed -= Driver_CommunicationClosed;
                    _driver.AlarmCleared -= Driver_AlarmCleared;
                    _driver.AlarmRaised -= Driver_AlarmRaised;
                    _driver.EquipmentConstantChanged -= Driver_EquipmentConstantChanged;
                    _driver.EventFired -= Driver_EventFired;
                    _driver.StatusVariableChanged -= Driver_StatusVariableChanged;
                    _driver.DataFlowRecipeStarted -= Driver_DataFlowRecipeStarted;
                    _driver.DataFlowRecipeCompleted -= Driver_DataFlowRecipeCompleted;
                    _driver.DataFlowRecipeAdded -= Driver_DataFlowRecipeAdded;
                    _driver.DataFlowRecipeDeleted -= Driver_DataFlowRecipeDeleted;
                    _driver.ProcessModuleRecipeStarted -= Driver_ProcessModuleRecipeStarted;
                    _driver.ProcessModuleRecipeCompleted -= Driver_ProcessModuleRecipeCompleted;
                    _driver.ProcessModuleAcquisitionCompleted -= Driver_ProcessModuleAcquisitionCompleted;
                    _driver.StopCancelAllJobsRequested -= Driver_StopCancelAllJobsRequested;
                    _driver.FdcCollectionChanged -= Driver_FdcCollectionChanged;
                    _driver = null;
                }

                if (this.TryGetAlarmCenter(out var alarmCenter))
                {
                    alarmCenter.Services.AlarmOccurrenceStateChanged -=
                        Services_AlarmOccurrenceStateChanged;
                }

                if (ExecutionMode == ExecutionMode.Simulated)
                {
                    DisposeSimulatedMode();
                }
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
