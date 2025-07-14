using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.AlarmModeling;
using Agileo.EquipmentModeling;

using UnitySC.DataAccess.Dto;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.Controller;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.Recipes;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;

using Alarm = Agileo.AlarmModeling.Alarm;
using CollectionEventEventArgs = UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs.CollectionEventEventArgs;

namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager
{
    public partial class ToolControlManager
    {
        #region Fields

        private ToolControlDriver _driver;
        private bool _firstConnection = true;
        private bool _abortInProgress;

        #endregion

        #region Properties

        public List<ProcessJob> ProcessJobs { get; } = new();

        #endregion

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
                    //ToolControl follows state from controller so the device is initializing while calling init
                    //We need to remove this precondition to allow init on this device
                    DeviceType.RemovePrecondition(nameof(Initialize), typeof(IsNotBusy));
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        _driver = new ToolControlDriver(Logger);
                        _driver.ClientConnectionStateChanged += Driver_ClientConnectionStateChanged;
                        _driver.SendCollectionEventReceived += Driver_SendCollectionEventReceived;
                        _driver.SendDataSet_S13F13Received += Driver_SendDataSet_S13F13Received;
                        _driver.SendDataSet_S13F16Received += Driver_SendDataSet_S13F16Received;
                        _driver.EquipmentStateRecieved += Driver_EquipmentStateRecieved;
                        _driver.RequestChangeOperationModeReceived +=
                            Driver_RequestChangeOperationModeReceived;
                        _driver.GetUTOEquipmentStateReceived += Driver_GetUTOEquipmentStateReceived;
                        _driver.ProcessProgramModificationNotifyReceived +=
                            Driver_ProcessProgramModificationNotifyReceived;
                        _driver.AreJobsActiveReceived += Driver_AreJobsActiveReceived;
                    }
                    else
                    {
                        SetUpSimulatedMode();
                    }

                    var controller = this.GetEquipment().AllOfType<Controller>().FirstOrDefault();
                    if (controller != null)
                    {
                        controller.StatusValueChanged += Controller_StatusValueChanged;
                    }

                    var robot = this.GetEquipment().AllOfType<Robot>().FirstOrDefault();
                    if (robot != null)
                    {
                        robot.StatusValueChanged += Robot_StatusValueChanged;
                    }

                    StatusValueChanged += ToolControlManager_StatusValueChanged;
                    break;
                case SetupPhase.SetupDone:
                    foreach (var processModule in this.GetEquipment()
                                 .AllDevices<ToolControlProcessModule>())
                    {
                        processModule.SetDriver(_driver);
                        processModule.StatusValueChanged += ProcessModule_StatusValueChanged;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            _abortInProgress = false;
            base.InternalInitialize(mustForceInit);
            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            var success = false;
            var errorMessage = string.Empty;
            _driver.RaiseChangeOperationMode(
                OperationMode.Automatic,
                ref success,
                ref errorMessage);
            if (!success)
            {
                throw new InvalidOperationException(errorMessage);
            }

            Task.Run(
                () =>
                {
                    _driver.RaiseOnGetAllAvailableRecipeNames();
                    _driver.RaiseOnGetAvailableModules();
                    _driver.RaiseOnGetEquipmentState();
                });
        }

        protected override void InternalStartRecipe(
            MaterialRecipe materialRecipe,
            string processJobId)
        {
            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            var materialCarriers = new List<MaterialCarrier>();
            foreach (var material in materialRecipe.Wafers)
            {
                var materialCarrier =
                    materialCarriers.FirstOrDefault(x => x.Id == material.CarrierId);
                if (materialCarrier == null)
                {
                    materialCarriers.Add(
                        new MaterialCarrier(
                            material.CarrierId,
                            material.CarrierId,
                            new Slot[]
                            {
                                new(
                                    material.SourceSlot.ToString(),
                                    material.Source.Name,
                                    new Substrate(
                                        material.SubstrateId,
                                        material.LotId,
                                        material.AcquiredId))
                            }));
                    continue;
                }

                materialCarrier.Slots.Add(
                    new Slot(
                        material.SourceSlot.ToString(),
                        material.Source.Name,
                        new Substrate(material.SubstrateId, material.LotId, material.AcquiredId)));
            }

            var processJob = new ProcessJob(
                $"{materialRecipe.Recipe.ProductName}/{materialRecipe.Recipe.StepName}/{materialRecipe.Recipe.Name}",
                processJobId,
                materialCarriers.ToArray(),
                DateTime.Now);
            var success = _driver.RaiseOnCreateProcessJob(processJob);
            if (success)
            {
                ProcessJobs.Add(processJob);
                foreach (var processModule in this.GetEquipment()
                             .AllDevices<ToolControlProcessModule>())
                {
                    processModule.AddProcessJob(processJob);
                }

                var jobProgram = new UTOJobProgram();
                jobProgram.PMItems = new List<PMItem>();
                foreach (var item in processJob.FlowRecipe)
                {
                    var processModule = this.GetEquipment()
                        .AllDevices<ToolControlProcessModule>()
                        .FirstOrDefault(pm => pm.Configuration.ModuleId == item.ProcessModuleId);
                    if (processModule != null)
                    {
                        jobProgram.PMItems.Add(
                            new PMItem()
                            {
                                OrientationAngle = item.AngleInDegree,
                                PMType = processModule.ActorType
                            });
                    }
                }

                foreach (var wafer in materialRecipe.Wafers)
                {
                    wafer.JobProgram = jobProgram;
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"Not able to create process job with id {processJobId}. Please check ToolControl.");
            }
        }

        protected override void InternalAbortRecipe(string jobId)
        {
            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            var processJob = ProcessJobs.FirstOrDefault(p => p.Name == jobId);
            if (processJob == null)
            {
                throw new InvalidOperationException(
                    $"Try to abort process job with id {jobId} but does not exist");
            }

            _abortInProgress = true;
            _driver.RaiseOnAbortProcessJob(processJob);
            SignalEndOfProcessJob(processJob.Name);
        }

        protected override void InternalStartJobOnMaterial(DataflowRecipeInfo recipe, Wafer wafer)
        {
            //Do nothing in case of ToolControl process modules
        }

        protected override void InternalGetAvailableRecipes()
        {
            if (_driver == null)
            {
                throw new InvalidOperationException(
                    $"{Name} driver is not instantiated. Cannot execute the command.");
            }

            var availableRecipes = _driver.RaiseOnGetAllAvailableRecipeNames();
            AvailableRecipes = new List<DataflowRecipeInfo>();
            var id = 0;
            foreach (var recipe in availableRecipes)
            {
                var splittedRecipe = recipe.ToString().Split('/');
                AvailableRecipes.Add(
                    new DataflowRecipeInfo()
                    {
                        CreatedDate = DateTime.Now,
                        Id = id,
                        IdGuid = Guid.NewGuid(),
                        Name = splittedRecipe[2],
                        ProductId = 0,
                        ProductName = splittedRecipe[0],
                        StepId = 0,
                        StepName = splittedRecipe[1]
                    });
                id++;
            }
        }

        protected override void InternalStartCommunication()
        {
            _driver?.Start();
        }

        protected override void InternalStopCommunication()
        {
            _driver?.Dispose();
        }

        #endregion

        #region Events

        public event EventHandler<S13F13EventArgs> S13F13Raised;

        protected void OnS13F13Raised(ITableData_S13F13 tableData)
        {
            S13F13Raised?.Invoke(this, new S13F13EventArgs(tableData));
        }

        public event EventHandler<S13F16EventArgs> S13F16Raised;

        protected void OnS13F16Raised(ITableData_S13F16 tableData)
        {
            S13F16Raised?.Invoke(this, new S13F16EventArgs(tableData));
        }

        public event EventHandler<CollectionEventEventArgs> ToolControlCollectionEventRaised;

        protected void OnToolControlCollectionEventRaised(
            string collectionEventName,
            ISecsVariableList dataVariables)
        {
            ToolControlCollectionEventRaised?.Invoke(
                this,
                new CollectionEventEventArgs(collectionEventName, dataVariables));
        }

        #endregion

        #region Event Handlers

        private void Driver_ClientConnectionStateChanged(bool isConnected)
        {
            if (isConnected)
            {
                IsCommunicationStarted = true;
                IsCommunicating = true;

                //Need to register alarms only the first time we are connected
                if (!_firstConnection)
                {
                    return;
                }

                Task.Run(
                    () =>
                    {
                        Thread.Sleep(1000);
                        InternalGetAvailableRecipes();
                        var processModules = _driver.RaiseOnGetAvailableModules();
                        foreach (var processModule in this.GetEquipment()
                                     .AllOfType<ToolControlProcessModule>())
                        {
                            var foundProcessModule = processModules.FirstOrDefault(
                                pm => pm.Id == processModule.Configuration.ModuleId);
                            if (foundProcessModule != null)
                            {
                                processModule.SetProcessModuleName(foundProcessModule.Name);
                            }
                        }
                    });
                _firstConnection = false;
            }
            else
            {
                IsCommunicationStarted = false;
                IsCommunicating = false;
            }
        }

        private void ProcessModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not ToolControlProcessModule processModule)
            {
                return;
            }

            if (e.Status.Name == nameof(IDriveableProcessModule.ProcessModuleState))
            {
                switch (processModule.ProcessModuleState)
                {
                    case ProcessModuleState.Idle:
                        if (_abortInProgress)
                        {
                            _abortInProgress = false;
                            OnProcessModuleAcquisitionCompleted(
                                new ProcessModuleRecipeEventArgs(
                                    processModule,
                                    processModule.SelectedRecipe,
                                    RecipeTerminationState.failed));
                            OnProcessModuleRecipeCompleted(
                                new ProcessModuleRecipeEventArgs(
                                    processModule,
                                    processModule.SelectedRecipe,
                                    RecipeTerminationState.failed));
                            if (processModule.ActorType == processModule.Location.Wafer.JobProgram.PMItems.Last().PMType)
                            {
                                OnDataFlowRecipeCompleted(
                                    new DataFlowRecipeEventArgs(null,
                                        processModule.Location.Wafer.ProcessJobId,
                                        processModule.Location.Wafer.SubstrateId,
                                        DataflowRecipeStatus.Error));
                            }
                        }
                        else
                        {
                            OnProcessModuleAcquisitionCompleted(
                                new ProcessModuleRecipeEventArgs(
                                    processModule,
                                    processModule.SelectedRecipe,
                                    RecipeTerminationState.successfull));
                            OnProcessModuleRecipeCompleted(
                                new ProcessModuleRecipeEventArgs(
                                    processModule,
                                    processModule.SelectedRecipe,
                                    RecipeTerminationState.successfull));
                            if (processModule.ActorType == processModule.Location.Wafer.JobProgram.PMItems.Last().PMType)
                            {
                                OnDataFlowRecipeCompleted(
                                    new DataFlowRecipeEventArgs(null,
                                        processModule.Location.Wafer.ProcessJobId,
                                        processModule.Location.Wafer.SubstrateId,
                                        DataflowRecipeStatus.Terminated));
                            }
                        }

                        break;
                    case ProcessModuleState.Error:
                        OnProcessModuleAcquisitionCompleted(
                            new ProcessModuleRecipeEventArgs(
                                processModule,
                                processModule.SelectedRecipe,
                                RecipeTerminationState.failed));
                        OnProcessModuleRecipeCompleted(
                            new ProcessModuleRecipeEventArgs(
                                processModule,
                                processModule.SelectedRecipe,
                                RecipeTerminationState.failed));

                        if (processModule.ActorType == processModule.Location.Wafer.JobProgram.PMItems.Last().PMType)
                        {
                            OnDataFlowRecipeCompleted(
                                new DataFlowRecipeEventArgs(null,
                                    processModule.Location.Wafer.ProcessJobId,
                                    processModule.Location.Wafer.SubstrateId,
                                    DataflowRecipeStatus.Error));
                        }
                        break;
                    case ProcessModuleState.Active:
                        OnProcessModuleRecipeStarted(
                            new ProcessModuleRecipeEventArgs(
                                processModule,
                                processModule.SelectedRecipe));
                        break;
                }
            }
        }

        private void Driver_SendDataSet_S13F16Received(ITableData_S13F16 tableData)
        {
            Task.Run(() => { OnS13F16Raised(tableData); });
        }

        private void Driver_SendDataSet_S13F13Received(ITableData_S13F13 tableData)
        {
            Task.Run(() => { OnS13F13Raised(tableData); });
        }

        private void Driver_SendCollectionEventReceived(
            string collectionEventName,
            ISecsVariableList dataVariables)
        {
            Task.Run(
                () => { OnToolControlCollectionEventRaised(collectionEventName, dataVariables); });
        }

        private void Driver_RequestChangeOperationModeReceived(
            OperationMode operationMode,
            ref bool success,
            ref string errorMessage)
        {
            var controller = this.GetEquipment().AllOfType<Controller>().FirstOrDefault();
            if (controller == null)
            {
                success = false;
                errorMessage = "Equipment controller not created";
                return;
            }

            if (controller.State == OperatingModes.Executing)
            {
                success = false;
                errorMessage = "At least one process job is running";
                return;
            }

            if (controller.State == OperatingModes.Initialization)
            {
                success = false;
                errorMessage = "Equipment controller is initializing the system";
                return;
            }

            success = true;
        }

        private void Driver_EquipmentStateRecieved(EquipmentState equipmentState)
        {
            Task.Run(
                () =>
                {
                    switch (equipmentState)
                    {
                        case EquipmentState.Undefined:
                        case EquipmentState.Offline:
                        case EquipmentState.Error:
                        case EquipmentState.ShuttingDown:
                        case EquipmentState.Pending:
                        case EquipmentState.Suspended:
                            SetState(OperatingModes.Maintenance);
                            break;
                        case EquipmentState.Idle:
                        case EquipmentState.Initialized:
                            SetState(OperatingModes.Idle);
                            break;
                        case EquipmentState.Active:
                        case EquipmentState.StartingUp:
                            SetState(OperatingModes.Executing);
                            break;
                        case EquipmentState.Initializing:
                            SetState(OperatingModes.Initialization);
                            break;
                    }
                });
        }

        private void Robot_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not Robot robot || _driver == null || e.Status.Name != nameof(State))
            {
                return;
            }

            switch (robot.State)
            {
                case OperatingModes.Maintenance:
                    _driver.RaiseOnSetTransportModuleState(ModuleState.Error);
                    break;
                case OperatingModes.Initialization:
                    _driver.RaiseOnSetTransportModuleState(ModuleState.Initializing);
                    break;
                case OperatingModes.Idle:
                    _driver.RaiseOnSetTransportModuleState(ModuleState.Idle);
                    break;
                case OperatingModes.Executing:
                    _driver.RaiseOnSetTransportModuleState(ModuleState.Executing);
                    break;
            }
        }

        private void Controller_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not Controller controller
                || _driver == null
                || e.Status.Name != nameof(State))
            {
                return;
            }

            try
            {
                switch (controller.State)
                {
                    case OperatingModes.Maintenance:
                        _driver.RaiseOnUTOEquipmentStateChanged(EquipmentState.Idle);
                        break;
                    case OperatingModes.Initialization:
                        _driver.RaiseOnUTOEquipmentStateChanged(EquipmentState.Initializing);
                        break;
                    case OperatingModes.Idle:
                        _driver.RaiseOnUTOEquipmentStateChanged(EquipmentState.Idle);
                        break;
                    case OperatingModes.Executing:
                        _driver.RaiseOnUTOEquipmentStateChanged(EquipmentState.Active);
                        break;
                    case OperatingModes.Engineering:
                        _driver.RaiseOnUTOEquipmentStateChanged(EquipmentState.Suspended);
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void Driver_GetUTOEquipmentStateReceived(ref EquipmentState equipmentState)
        {
            var controller = this.GetEquipment().AllOfType<Controller>().FirstOrDefault();
            if (controller == null)
            {
                equipmentState = EquipmentState.Error;
                return;
            }

            switch (controller.State)
            {
                case OperatingModes.Maintenance:
                    equipmentState = EquipmentState.Idle;
                    break;
                case OperatingModes.Initialization:
                    equipmentState = EquipmentState.Initializing;
                    break;
                case OperatingModes.Idle:
                    equipmentState = EquipmentState.Idle;
                    break;
                case OperatingModes.Executing:
                    equipmentState = EquipmentState.Active;
                    break;
                case OperatingModes.Engineering:
                    equipmentState = EquipmentState.Suspended;
                    break;
            }
        }

        private void ToolControlManager_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(State))
            {
                switch (State)
                {
                    case OperatingModes.Maintenance:
                    case OperatingModes.Engineering:
                        DataflowState = TC_DataflowStatus.Maintenance;
                        break;
                    case OperatingModes.Initialization:
                        DataflowState = TC_DataflowStatus.Initializing;
                        break;
                    case OperatingModes.Idle:
                        DataflowState = TC_DataflowStatus.Idle;
                        break;
                    case OperatingModes.Executing:
                        DataflowState = TC_DataflowStatus.Executing;
                        break;
                }
            }
        }

        private void Driver_ProcessProgramModificationNotifyReceived(
            string arg1,
            PPChangeState arg2)
        {
            Task.Run(
                () =>
                {
                    var splittedRecipe = arg1.Split('/');
                    var recipe = new DataflowRecipeInfo()
                    {
                        CreatedDate = DateTime.Now,
                        Id = 0,
                        IdGuid = Guid.NewGuid(),
                        Name = splittedRecipe[2],
                        ProductId = 0,
                        ProductName = splittedRecipe[0],
                        StepId = 0,
                        StepName = splittedRecipe[1]
                    };
                    switch (arg2)
                    {
                        case PPChangeState.PPCreate:
                            OnDataFlowRecipeAdded(new DataFlowRecipeEventArgs(recipe));
                            break;
                        case PPChangeState.PPModify:
                            OnDataFlowRecipeModified(new DataFlowRecipeEventArgs(recipe));
                            break;
                        case PPChangeState.PPDelete:
                            OnDataFlowRecipeDeleted(new DataFlowRecipeEventArgs(recipe));
                            break;
                    }
                });
        }

        private bool Driver_AreJobsActiveReceived()
        {
            var controller = this.GetEquipment().AllOfType<Controller>().FirstOrDefault();
            if (controller != null)
            {
                return controller.State == OperatingModes.Executing;
            }

            return false;
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

        public override void ResetIsStopCancelAllJobsRequested()
        {
            //Do nothing in case of Tool Control
        }

        public void SendDataSetAck(TableDataResponse response)
        {
            _driver?.RaiseOnSendDataSetAck_S13F14(response);
        }

        public void DataSetRequest(TableDataRequest request)
        {
            _driver?.RaiseOnDataSetRequest_S13F15(request);
        }

        public RecipeDownloadResult DownloadRecipe(string recipeName)
        {
            var success = false;
            var errorMessage = string.Empty;
            var memoryStream = new MemoryStream();
            _driver.RaiseS7F5RequestPPIDChanged(
                recipeName,
                memoryStream,
                ref success,
                ref errorMessage);
            return new RecipeDownloadResult(recipeName, success, errorMessage, memoryStream);
        }

        public RecipeUploadResult UploadRecipe(string recipeName, Stream recipe)
        {
            var success = false;
            var errorMessage = string.Empty;
            _driver.RaiseS7F3Changed(recipeName, recipe, ref success, ref errorMessage);
            return new RecipeUploadResult(recipeName, success, errorMessage);
        }

        public DeleteRecipeResult DeleteRecipe(IEnumerable<string> recipeNames)
        {
            var success = false;
            var errorMessage = string.Empty;
            _driver.RaiseOnS7F17DeletePPID(recipeNames.ToArray(), ref success, ref errorMessage);
            return new DeleteRecipeResult(success, errorMessage);
        }

        public override void SwitchToManualMode()
        {
            if (ExecutionMode == ExecutionMode.Real)
            {
                Task.Run(
                    () =>
                    {
                        var success = false;
                        var errorMessage = string.Empty;
                        _driver?.RaiseChangeOperationMode(
                            OperationMode.Manual,
                            ref success,
                            ref errorMessage);
                    });
            }
        }

        public override void SignalEndOfProcessJob(string processJobId)
        {
            var processJob = ProcessJobs.FirstOrDefault(p => p.Name == processJobId);
            if (processJob == null)
            {
                return;
            }

            _driver.RaiseOnDeleteProcessJob(processJob);
            ProcessJobs.Remove(processJob);
            foreach (var processModule in
                     this.GetEquipment().AllDevices<ToolControlProcessModule>())
            {
                processModule.RemoveProcessJob(processJob);
            }
        }

        public override UTOJobProgram GetJobProgramFromRecipe(DataflowRecipeInfo recipe)
        {
            var pmItems = new List<PMItem>();
            foreach (var processModule in
                     this.GetEquipment().AllDevices<ToolControlProcessModule>())
            {
                pmItems.Add(
                    new PMItem() { OrientationAngle = 0, PMType = processModule.ActorType });
            }

            return new UTOJobProgram() { PMItems = pmItems };
        }

        public override DataflowRecipeInfo GetRecipeInfo(string recipeName)
        {
            return AvailableRecipes.FirstOrDefault(
                r => r.GetRecipePath() == recipeName);
        }

        public override string GetRecipeName(DataflowRecipeInfo recipe)
        {
            return recipe.GetRecipePath();
        }

        public override List<string> GetRecipeNames()
        {
            return AvailableRecipes.Select(
                r => r.GetRecipePath()).ToList();
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_driver != null)
                {
                    _driver.ClientConnectionStateChanged -= Driver_ClientConnectionStateChanged;
                    _driver.SendCollectionEventReceived -= Driver_SendCollectionEventReceived;
                    _driver.SendDataSet_S13F13Received -= Driver_SendDataSet_S13F13Received;
                    _driver.SendDataSet_S13F16Received -= Driver_SendDataSet_S13F16Received;
                    _driver.EquipmentStateRecieved -= Driver_EquipmentStateRecieved;
                    _driver.RequestChangeOperationModeReceived -=
                        Driver_RequestChangeOperationModeReceived;
                    _driver.GetUTOEquipmentStateReceived -= Driver_GetUTOEquipmentStateReceived;
                    _driver.ProcessProgramModificationNotifyReceived -=
                        Driver_ProcessProgramModificationNotifyReceived;
                    _driver.AreJobsActiveReceived -= Driver_AreJobsActiveReceived;
                    _driver.Dispose();
                    _driver = null;
                }

                var controller = this.GetEquipment().AllOfType<Controller>().FirstOrDefault();
                if (controller != null)
                {
                    controller.StatusValueChanged -= Controller_StatusValueChanged;
                }

                var robot = this.GetEquipment().AllOfType<Robot>().FirstOrDefault();
                if (robot != null)
                {
                    robot.StatusValueChanged -= Robot_StatusValueChanged;
                }

                StatusValueChanged -= ToolControlManager_StatusValueChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
