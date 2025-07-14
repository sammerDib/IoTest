#define TESTDFError

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.ADCAS300Like.Service;
using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Dataflow.Configuration;
using UnitySC.Dataflow.Operations.Interface.UTODF;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Dataflow.Shared.DF_Status;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Dataflow.Manager
{
    public class DFManager : IDFManager, ICommunicationOperationsCB, IAlarmOperationsCB, IDFPostProcessCB
    {
        private DFServerConfiguration _dfServerConfiguration;
        private IServiceDFConfigurationManager _serviceConfiguration;
        private ILogger _logger;
        private ServiceInvoker<IDbRecipeService> _dbRecipeService = null;
        private ICommunicationOperations _communicationOperations;

        /// <summary>
        /// list of WF recipes indexed by Guid and version. the key is KeyForAllVersion
        /// </summary>
        private SortedDictionary<string, DataflowRecipe> _listDataflowRecipe = new SortedDictionary<string, DataflowRecipe>();

        private object _listDFRecipeLock = new object();
        private IPMDFServiceCB _pmdfServiceCB;
        private IUTODFServiceCB _utodfServiceCB;
        private IAlarmOperations _alarmOperations;
        private IDFStatusVariableOperations _svOperations;
        private Identity _identity;
        private DFStateBase _currentDFStatus;
        private ADCsConnectionManager _adcsConnectionManager;
        private ServiceInvoker<ISendFdcService> _sendFdcService;
        private IMessenger _messenger;
        private SendFdcSupervisor _sendFdcSupervisor;

        public DFManager()
        {
            _dfServerConfiguration = ClassLocator.Default.GetInstance<DFServerConfiguration>();
            _serviceConfiguration = ClassLocator.Default.GetInstance<IServiceDFConfigurationManager>();
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            CurrentDFStatus = new DFNoneState();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _sendFdcSupervisor = ClassLocator.Default.GetInstance<SendFdcSupervisor>();
        }

        public void Init()
        {
            _svOperations = ClassLocator.Default.GetInstance<IDFStatusVariableOperations>();
            CurrentDFStatus = CurrentDFStatus.ChangeState_Initializing();
            _logger.Debug($"ToolKey = {_dfServerConfiguration.ToolKey} / OrientationAngle = {_dfServerConfiguration.OrientationAngle}");
            _logger.Debug($"Configuration path = {Path.GetFullPath(_serviceConfiguration.ConfigurationFolderPath)}");
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            _pmdfServiceCB = ClassLocator.Default.GetInstance<IPMDFServiceCB>();
            _utodfServiceCB = ClassLocator.Default.GetInstance<IUTODFServiceCB>();
            _alarmOperations = ClassLocator.Default.GetInstance<IAlarmOperations>();
            _communicationOperations = new CommunicationOperations();
            _communicationOperations.Init("Server DF / Client PM", this);
            _communicationOperations.SwitchState = EnableState.Enabled;
            _adcsConnectionManager = new ADCsConnectionManager();
            _adcsConnectionManager.Init();

            _identity = new Identity(_dfServerConfiguration.ToolKey, ActorType.DataflowManager, 1);
            CurrentDFStatus = CurrentDFStatus.ChangeState_Idle();

            _sendFdcSupervisor = ClassLocator.Default.GetInstance<SendFdcSupervisor>();
            _sendFdcSupervisor.SubscribeToChanges();
            _messenger.Register<SendFDCListMessage>(this, (r, m) => { UpdateFDCData(m.FDCsData); });
            TestErrors.ResetError();
        }
        // Update FDC from DF to UTO
        private void UpdateFDCData(List<FDCData> fdcsDataCollection)
        {
            if (fdcsDataCollection != null)
            {
                string msgLog = $"UpdateFDCData() from FDCManager via messenger, fDCsData data count ={fdcsDataCollection.Count}";
                _logger.Verbose(msgLog);
                Task.Run(() => _utodfServiceCB.NotifyFDCCollectionChanged(fdcsDataCollection));
            }
            else
                _logger.Error("UpdateFDCData() from FDCManager via messenger, fdcsDataCollection parameter is null");
        }

        public Identity Identity { get => _identity; }

        public DFStateBase CurrentDFStatus
        {
            get => _currentDFStatus;

            set
            {
                if (_currentDFStatus == null)
                    _currentDFStatus = value;
                if (_currentDFStatus.State != value.State)
                {
                    _currentDFStatus = value;
                    _logger.Information($"Current DF Status = {_currentDFStatus.State.ToString()}");
                }
                if (_svOperations != null) _svOperations.Update_DataflowState(_currentDFStatus.State);
            }
        }

        public TC_DataflowStatus State { get => CurrentDFStatus.State; }

        public List<DataflowRecipeInfo> GetAllRecipes(List<ActorType> actors, int toolkey)
        {
            _logger.Debug("Star GetAllRecipes service methode for Toolkey = {0}", toolkey);
            var dfRecipes = new List<DataflowRecipeInfo>();
            try
            {
                TestErrors.CheckTestError(ErrorID.GetAllRecipesfromDBError);
                // TODO : Check if it is the good way Add the ADC
                actors.Add(ActorType.ADC);

                dfRecipes = _dbRecipeService.Invoke(x => x.GetAllDataflow(actors, toolkey, false));
                _logger.Debug("{0} DF recipees find", dfRecipes.Count);
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.GetAllRecipesfromDBError, $"GetAllRecipes Exception: {ex.Message}");
            }
            return dfRecipes;
        }

        public UTOJobProgram StartRecipeDF(DataflowRecipeInfo dfRecipeInfo, string processJobID, List<Guid> wafersGuid)
        {
            var jobResult = new UTOJobProgram();
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeStartingError_StartDFRecipe);
                _logger.Information($"[CycleLog] -----------------------------------------------------------------------------------------------------------");
                _logger.Information($"[CycleLog] Start jobId = {processJobID} with DfRecipe = {dfRecipeInfo.Name} and {wafersGuid.Count} wafers");
                double orientationAngle = _dfServerConfiguration.OrientationAngle;
                _logger.Information($"[CycleLog] OrientationAngle = {orientationAngle} °");
                if (string.IsNullOrEmpty(processJobID))
                {
                    throw new Exception($"Impossible to make an StartRecipeDF because the processJobID is null");
                }
                DataflowRecipe currentDataFlowRecipe = LoadDataFlowRecipe(dfRecipeInfo, processJobID);
                foreach (var waferGuid in wafersGuid)
                {
                    DataflowInstance df = new DataflowInstance(currentDataFlowRecipe);
                    lock (currentDataFlowRecipe.StartedDataflowLock)
                    {
                        if (!currentDataFlowRecipe.StartedDataflow.ContainsKey(waferGuid))
                        {
                            currentDataFlowRecipe.StartedDataflow.Add(waferGuid, df);
                        }
                    }
                }
                if (currentDataFlowRecipe != null
                    && !_listDataflowRecipe.ContainsKey(processJobID))
                {
                    lock (_listDFRecipeLock)
                    {
                        _listDataflowRecipe.Add(processJobID, currentDataFlowRecipe);
                    }
                }

                //return
                var actorTypes = currentDataFlowRecipe.Actors.Values.Where(x => x.ActorType != ActorType.ADC).Select(x => x.ActorType).ToList();
                jobResult.PMItems = new List<PMItem>();
                foreach (var actorType in actorTypes)
                {
                    jobResult.PMItems.Add(new PMItem() { PMType = actorType, OrientationAngle = orientationAngle });
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.RecipeStartingError_StartDFRecipe, $"StartRecipeDF Exception: {ex.Message}");
            }
            return jobResult;
        }
        public UTOJobProgram GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo)
        {
            var jobResult = new UTOJobProgram();
            double orientationAngle = _dfServerConfiguration.OrientationAngle;
            try
            {
                _logger.Debug($"[GetUTOJobProgramForARecipeDF] ");

                DataflowRecipe currentDataFlowRecipe = LoadDataFlowRecipe(dfRecipeInfo, "");
                if (currentDataFlowRecipe != null)
                {
                    //return
                    var actorTypes = currentDataFlowRecipe.Actors.Values.Where(x => x.ActorType != ActorType.ADC).Select(x => x.ActorType).ToList();
                    jobResult.PMItems = new List<PMItem>();
                    foreach (var actorType in actorTypes)
                    {
                        jobResult.PMItems.Add(new PMItem() { PMType = actorType, OrientationAngle = orientationAngle });
                    }
                }
                else
                    throw new Exception("DataflowRecipe loading failed");
            }
            catch (Exception ex)
            {
                _logger.Information($"[GetUTOJobProgramForARecipeDF] return UTOJobProgram from a recipe DF Failed - {ex.Message + ex.StackTrace}");
            }
            return jobResult;
        }
        private DataflowRecipe LoadDataFlowRecipe(DataflowRecipeInfo dfRecipeInfo, string uniqueIdJob)
        {
            DataflowRecipe currentDataflowRecipe = null;
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeStartingError_LoadDFRecipe);
                bool toLoad = false;

                DataflowRecipeComponent recipeDatabase = _dbRecipeService?.Invoke(x => x.GetLastDataflow(dfRecipeInfo.IdGuid, false));
                Guid keyForAllVersion = recipeDatabase.Key;
                lock (_listDFRecipeLock)
                {
                    if (!_listDataflowRecipe.TryGetValue(uniqueIdJob, out currentDataflowRecipe))
                    {
                        toLoad = true;
                    }
                    else
                    {
                        // if we find the recipe and it is not of the same version, we recharge it.
                        toLoad = recipeDatabase.Version != currentDataflowRecipe.Version;
                    }
                }
                if (toLoad)
                {
                    currentDataflowRecipe = new DataflowRecipe();
                    currentDataflowRecipe.KeyForAllVersion = keyForAllVersion;
                    currentDataflowRecipe.DataflowRecipeComponent = recipeDatabase;
                    currentDataflowRecipe.DataflowRecipeInfo = dfRecipeInfo;
                    currentDataflowRecipe.Version = recipeDatabase.Version;

                    var composents = recipeDatabase.AllChilds();
                    var recipeActors = composents.Where(c => c.ActorType != ActorType.DataflowManager).ToList();

                    // creating DataflowActor
                    DataflowActorManager dfActorManager = null;
                    foreach (var ra in recipeActors)
                    {
                        string actorName = ra.ActorType.ToString();
                        switch (ra.ActorCategory)
                        {
                            case ActorCategory.ProcessModule:
                                dfActorManager = new PMActor(ra.ActorType.ToString(), ra.ActorType);
                                break;

                            case ActorCategory.PostProcessing:
                                actorName = GetActorNameForPostProcessing(currentDataflowRecipe, ra);
                                break;

                            default:
                                break;
                        }


                        ra.ActorID = actorName;
                        currentDataflowRecipe.Actors.Add(
                            actorName,

                            new DataflowActor(ra.ActorType.ToString(), ra.ActorType)
                            {
                                DataflowActorManager = dfActorManager,
                                DataflowActorRecipe = new DataflowActorRecipe() { KeyForAllVersion = ra.Key, Name = ra.Name },
                                Inputs = ra.Inputs.Select(i => new InputOutputDataType() { ResultType = i }).ToList(),
                                Outputs = ra.Outputs.Select(i => new InputOutputDataType() { ResultType = i }).ToList(),
                            }
                            );
                    }
                    // once the DataflowActors are created, we re-browse to create the links with the children
                    foreach (var ra in recipeActors)
                    {
                        if (currentDataflowRecipe.Actors.TryGetValue(ra.ActorID, out DataflowActor wfa))
                        {
                            foreach (var rac in ra.ChildRecipes)
                            {
                                if (currentDataflowRecipe.Actors.TryGetValue(rac.Component.ActorID, out DataflowActor wfac))
                                {
                                    wfa.ChildActors.Add(wfac);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.RecipeStartingError_LoadDFRecipe, $"LoadDataFlowRecipe Exception: {ex.Message}");
                throw ex; // Method private to DFManager => Needed to throw on level up
            }
            return currentDataflowRecipe;
        }

        private static string GetActorNameForPostProcessing(DataflowRecipe currentDataflowRecipe, DataflowRecipeComponent ra)
        {
            string actorName = ra.ActorType.ToString();
            string actorKey = currentDataflowRecipe.Actors.Keys.LastOrDefault(key => key.StartsWith(actorName));
            if (actorKey is null)
            {
                return actorName;
            }
            int index = actorKey == actorName ? 1 : int.Parse(actorKey.Substring(actorName.Length - 1)) + 1;
            return $"{actorName}{index}";
        }

        private void UpdateDataflowStatus()
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.UpdateDFStatusError);

                _logger.Debug($"Check DF Status");
                bool isRecipeExecuting = false;
                lock (_listDFRecipeLock)
                {
                    foreach (var currentDataflowRecipe in _listDataflowRecipe.Values)
                    {
                        if (currentDataflowRecipe != null)
                        {
                            lock (currentDataflowRecipe.StartedDataflowLock)
                            {
                                foreach (var dfi in currentDataflowRecipe.StartedDataflow.Values)
                                {
                                    if ((dfi != null) && (dfi.Status == DataflowRecipeStatus.Executing))
                                    {
                                        isRecipeExecuting = true;
                                        _logger.Debug($"Update DF Status executing : Df recipe name {currentDataflowRecipe.DataflowRecipeInfo.Name} [NbDFRecipes = {_listDataflowRecipe.Values.Count}]");
                                        break;
                                    }
                                }
                            }
                        }
                        if (isRecipeExecuting)
                            break;
                    }
                }
                if (isRecipeExecuting)
                    CurrentDFStatus = CurrentDFStatus.ChangeState_Executing();
                else
                {
                    CurrentDFStatus = CurrentDFStatus.ChangeState_Idle();
                    _logger.Debug($"Update DF Status IDLE : [NbDFRecipes = {_listDataflowRecipe.Values.Count}]");
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.UpdateDFStatusError, $"UpdateDataflowStatus Exception: {ex.Message}");
                throw ex; // Method private to DFManager => Needed to throw on level up
            }
        }

        public void StartJob_Material(DataflowRecipeInfo dfRecipeInfo, Material wafer)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeStartingError_StartJob);

                _logger.Information($"[CycleLog] Load material {wafer.ToString()}");
                lock (_listDFRecipeLock)
                {
                    if (!_listDataflowRecipe.TryGetValue(wafer?.ProcessJobID, out DataflowRecipe currentDataflowRecipe))
                    {
                        PrepareStartOfRecipe(dfRecipeInfo, wafer);
                        _listDataflowRecipe.TryGetValue(wafer?.ProcessJobID, out currentDataflowRecipe);
                    }
                    if (currentDataflowRecipe != null)
                    {
                        lock (currentDataflowRecipe.StartedDataflowLock)
                        {
                            if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                            {
                                if (dfi != null
                                    && (dfi.Status == DataflowRecipeStatus.Available
                                    || dfi.Status == DataflowRecipeStatus.Terminated))
                                {
                                    dfi.Wafer = wafer;
                                    _logger.Debug("Update status to Executing for DFRecipe {0} with wafer {1}", dfRecipeInfo.Name, wafer.ToString());
                                    dfi.Status = DataflowRecipeStatus.Executing;
                                }
                            }
                        }
                    }
                }
                UpdateDataflowStatus();
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.RecipeStartingError_StartJob, $"StartJob_Material Exception: {ex.Message}");
            }
        }

        public bool CanStartPMRecipe(Identity identity, Material wafer)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.PMRecipeStartingRequestFailed);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);

                try
                {
                    TestErrors.CheckTestError(ErrorID.InvalidPMRecipeStartingRequested);
                }
                catch
                {
                    currentDataflowRecipe = null;
                }

                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"CanStartPMRecipe Failed. Unknown recipe on material {wafer.ToString()}. PM / ChamberID : {identity.ActorType} / {identity.ChamberID}";
                    _logger.Warning(msgErr);
                    NotifyError(ErrorID.InvalidPMRecipeStartingRequested, msgErr);
                    return false;
                }
                DataflowActor dfa = currentDataflowRecipe.Actors.Values.FirstOrDefault(a => a.ActorType == identity?.ActorType);
                if (dfa != null)
                {
                    lock (currentDataflowRecipe.StartedDataflowLock)
                    {
                        if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                        {
                            if (IsActorAborted(dfi, dfa.ActorType))
                            {
                                _logger.Debug("[CanStartPMRecipe] Can start PM recipe = No. Recipe aborted");
                                return false;
                            }
                            else
                            {
                                dfi.ChangePMActorStatus(dfa.ActorType, ActorRecipeStatus.Available);
                                _logger.Debug("[CanStartPMRecipe] Can start PM recipe = Yes");
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.PMRecipeStartingRequestFailed, $"CanStartPMRecipe Exception: {ex.Message}");
            }
            return false;
        }

        public void RecipeExecutionComplete(Identity identity, Material wafer, Guid? recipeKey, string results, RecipeTerminationState status)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.InvalidPMRecipeCompleteError);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"RecipeExecutionComplete Failed. Unknown recipe on material {wafer.ToString()}. PM / ChamberID : {identity.ActorType} / {identity.ChamberID}";
                    throw new Exception(msgErr);
                }
                else
                {
                    DataflowActor dfa = currentDataflowRecipe.Actors.Values.FirstOrDefault(a => a.ActorType == identity?.ActorType);
                    if (dfa != null)
                    {
                        lock (currentDataflowRecipe.StartedDataflowLock)
                        {
                            if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                            {
                                ActorRecipeStatus arStatus = ActorRecipeStatus.Error;
                                if (status == RecipeTerminationState.successfull)
                                    arStatus = ActorRecipeStatus.Terminated;

                                dfi.ChangePMActorStatus(dfa.ActorType, arStatus);

                                _logger.Debug($"[RecipeExecutionComplete] the status of the actor's recipe is {arStatus} with wafer {wafer.ToString()}");
                                _logger.Debug($"[RecipeExecutionComplete] The actor's recipe / ChamberID = {identity.ActorType} / {identity.ChamberID}");
                            }
                            dfa.DataflowActorManager.Ended(identity.ActorType.ToString(), wafer);
                        }
                    }
                }

                UpdateDataflowStatus();
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.InvalidPMRecipeCompleteError, $"RecipeExecutionComplete Exception: {ex.Message}");
            }
        }

        public bool IfAllDataflowActorsAreTerminated(Material wafer)
        {
            bool actorsAreTerminated = false;
            try
            {
                TestErrors.CheckTestError(ErrorID.UpdateActorStatusError);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"Check that the actors in the data flow are interrupted. Unknown recipe on material {wafer.ToString()}";
                    throw new Exception(msgErr);
                }
                else
                {
                    lock (currentDataflowRecipe.StartedDataflowLock)
                    {
                        if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                        {
                            actorsAreTerminated = dfi.DataflowActorValues.All(item => item.DataflowActorStatus == ActorRecipeStatus.Terminated || item.DataflowActorStatus == ActorRecipeStatus.Warning || item.DataflowActorStatus == ActorRecipeStatus.Error);
                        }
                    }
                }
                _logger.Debug("All DataflowActors Are Terminated for wafer {0} ? {1}", wafer.ToString(), actorsAreTerminated ? "Yes" : "No");
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.UpdateActorStatusError, $"IfAllDataflowActorsAreTerminated Exception: {ex.Message}");
            }
            return actorsAreTerminated;
        }
        public bool IfAllDataflowPMActorsAreTerminated(Material wafer)
        {
            bool pmActorsAreTerminated = false;
            try
            {
                TestErrors.CheckTestError(ErrorID.UpdateActorStatusError);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"Check that the actors in the data flow are interrupted. Unknown recipe on material {wafer.ToString()}";
                    throw new Exception(msgErr);
                }
                else
                {
                    lock (currentDataflowRecipe.StartedDataflowLock)
                    {
                        if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                        {                            
                            pmActorsAreTerminated = dfi.DataflowActorValues
                                                    .Where(item => item.DataflowActor.ActorType.GetCatgory() == ActorCategory.ProcessModule)
                                                    .All(item => item.DataflowActorStatus == ActorRecipeStatus.Terminated 
                                                            || item.DataflowActorStatus == ActorRecipeStatus.Warning 
                                                            || item.DataflowActorStatus == ActorRecipeStatus.Error);
                        }
                    }
                }
                _logger.Debug("All Dataflow PM Actors Are Terminated for wafer {0} ? {1}", wafer.ToString(), pmActorsAreTerminated ? "Yes" : "No");
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.UpdateActorStatusError, $"IfAllDataflowPMActorsAreTerminated Exception: {ex.Message}");
            }
            return pmActorsAreTerminated;
        }

        public DataflowRecipeInfo GetAssociatedDataflowFullInfo(Material wafer)
        {
            var currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
            return currentDataflowRecipe?.DataflowRecipeInfo;
        }

        private DataflowRecipe GetDataflowRecipeByProcessJobID(string processJobID)
        {
            if (string.IsNullOrEmpty(processJobID)) return null;

            lock (_listDFRecipeLock)
            {
                return _listDataflowRecipe.TryGetValue(processJobID, out DataflowRecipe dataflowRecipe) ? dataflowRecipe : null;
            }
        }

        private Material GetMaterialFromDatflowRecipeByProcessJobID(string processJobID, Guid materialGuid)
        {
            try
            {
                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(processJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"GetMaterialFromDatflowRecipeByProcessJobID Failed. Unknown processJobID in DFRecipe list (jobID={processJobID.ToString()}).";
                    throw new Exception(msgErr);
                }
                else
                {
                    //get Dataflow Instance
                    lock (currentDataflowRecipe.StartedDataflowLock)
                    {
                        if (currentDataflowRecipe.StartedDataflow.TryGetValue(materialGuid, out DataflowInstance dfi))
                        {
                            return dfi.Wafer;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.MaterialIdentificationError, $"GetMaterialFromDatflowRecipeByProcessJobID Exception: {ex.Message}");
                return null;
            }
        }

        public List<string> GetJobIdsByDFRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            lock (_listDFRecipeLock)
            {
                List<string> jobIds = _listDataflowRecipe.Where(kv => kv.Value.DataflowRecipeInfo.IdGuid == dfRecipeInfo.IdGuid).Select(kv => kv.Key).ToList();

                return jobIds;
            }
        }

        private List<DataflowRecipe> GetAllDataflowRecipes()
        {
            List<DataflowRecipe> dataflowRecipes;
            lock (_listDFRecipeLock)
            {
                var wfrs = from wfr in _listDataflowRecipe.Values
                           from wf in wfr.StartedDataflow.Values
                           select wfr;
                dataflowRecipes = wfrs.ToList<DataflowRecipe>();
            }
            return dataflowRecipes;
        }

        public void UpdatePMActorStatusByWafer(Identity identity, Material wafer, ActorRecipeStatus status)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.UpdateActorStatusError);


                if (identity.ActorType.GetCatgory() != ActorCategory.ProcessModule)
                    return;

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"ActorStatusUpdated Failed. Unknown recipe on material {wafer?.ToString()}. PM / ChamberID : {identity.ActorType} / {identity.ChamberID}";
                    throw new Exception(msgErr);
                }
                else
                {
                    DataflowActor dfa = currentDataflowRecipe.Actors.Values.FirstOrDefault(a => a.ActorType == identity?.ActorType);
                    if (dfa != null)
                    {
                        lock (currentDataflowRecipe.StartedDataflowLock)
                        {
                            if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                            {
                                dfi.ChangePMActorStatus(identity.ActorType, status);
                                _logger.Debug("ActorStatusUpdated for the wafer {0} : {1}", wafer.ToString(), status.ToString());
                                _logger.Debug("The actor's recipe / ChamberID = {0} / {1}", identity.ActorType, identity.ChamberID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.UpdateActorStatusError, $"ActorStatusUpdatedByWafer Exception: {ex.Message}");
            }
        }

        public void UpdatePPActorStatusByWafer(Identity identity, Side side, String processJobID, Guid materialGUID, ActorRecipeStatus status)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.UpdateActorStatusError);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(processJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"DFPostProcessComplete Failed. Unknown processJobID on material {processJobID.ToString()}. ADC / ChamberID : {identity.ActorType}-{side} / {identity.ChamberID}";
                    throw new Exception(msgErr);
                }
                else
                {
                    //get Dataflow Instance
                    lock (currentDataflowRecipe.StartedDataflowLock)
                    {
                        if (currentDataflowRecipe.StartedDataflow.TryGetValue(materialGUID, out DataflowInstance dfi))
                        {
                            var material = dfi.Wafer;
                            // Get dataflowActor => Search ADC actor from DFRecipe with same ADCRecipe
                            var actorSideRecipe = GetPPRecipeByMaterialAndBySide(identity, material, side);
                            DataflowActor ppActor = currentDataflowRecipe.Actors.Values.FirstOrDefault(a => a.DataflowActorRecipe == actorSideRecipe);
                            if (ppActor != null)
                            {
                                dfi.ChangePPActorStatus(ppActor.ActorType, actorSideRecipe, status);
                                _logger.Debug($"[DFPostProcessComplete] ADC status is {status} with wafer {material.ToString()}");
                                _logger.Debug($"[DFPostProcessComplete] ADC recipe / ChamberID = {actorSideRecipe.Name} / {identity.ChamberID}");
                                ppActor.DataflowActorManager.Ended(identity.ActorType.ToString(), material);
                            }
                        }
                    }
                }

                UpdateDataflowStatus();
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.UpdateActorStatusError, $"ActorStatusUpdatedByWafer Exception: {ex.Message}");
            }
        }

        public void UpdateDFRecipeInstanceStatus(DataflowRecipeInfo dfRecipeInfo, Material wafer, DataflowRecipeStatus status)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.UpdateDFRecipeStatusError);

                _logger.Debug("UpdateDFRecipeInstanceStatus. DFRecipe : {0}. Wafer : {1}. Status : {2}", dfRecipeInfo.Name, wafer.ToString(), status.ToString());
                lock (_listDFRecipeLock)
                {
                    if (_listDataflowRecipe.TryGetValue(wafer?.ProcessJobID, out DataflowRecipe currentDataflowRecipe))
                    {
                        if (currentDataflowRecipe == null)
                        {
                            string msgErr = $"UpdateDFRecipeInstanceStatus Failed. DFRecipe : {dfRecipeInfo.Name}. Wafer : {wafer.ToString()}";
                            throw new Exception(msgErr);
                        }
                        else
                        {
                            lock (currentDataflowRecipe.StartedDataflowLock)
                            {
                                if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                                {
                                    if (dfi != null)
                                    {
                                        dfi.Status = status;
                                        _logger.Debug("UpdateDFRecipeInstanceStatus for the wafer {0} : {1}", wafer.ToString(), status.ToString());
                                        _logger.Debug("The DfRecipeInfo = {0}", dfRecipeInfo.Name);
                                    }
                                }
                            }
                        }
                    }
                }
                UpdateDataflowStatus();
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.UpdateDFRecipeStatusError, $"UpdateDFRecipeInstanceStatus Exception: {ex.Message}");
            }
        }

        public DataflowRecipeInfo GetDataflowRecipeInfo(Identity identity, String processJobID, Guid materialGUID)
        {
            _logger.Debug("Get DF recipe associated with the {0} ProcessJobID. PM/ChamberID : {1}/{2}", processJobID, identity.ActorType, identity.ChamberID);
            DataflowRecipeInfo dataflowRecipeInfo = null;
            try
            {
                TestErrors.CheckTestError(ErrorID.GetDataflowRecipeError);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(processJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"Unable to find the DF recipe associated with the ProcessJobID={processJobID} . PM / ChamberID : {identity.ActorType} / {identity.ChamberID}";
                    throw new Exception(msgErr);
                }
                else
                {
                    DataflowActor dfa = currentDataflowRecipe?.Actors.Values.FirstOrDefault(a => a.ActorType == identity?.ActorType);
                    if (dfa != null)
                    {
                        lock (currentDataflowRecipe.StartedDataflowLock)
                        {
                            if (currentDataflowRecipe.StartedDataflow.TryGetValue(materialGUID, out DataflowInstance dfi))
                            {
                                dataflowRecipeInfo = currentDataflowRecipe?.DataflowRecipeInfo;
                                _logger.Debug("Return {0} DFRecipe associated with the {1} wafer", dataflowRecipeInfo.Name, dfi.Wafer.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.GetDataflowRecipeError, $"GetDataflowRecipeInfo Exception: {ex.Message}");
            }
            return dataflowRecipeInfo;
        }

        public Guid? RetrieveTheAssociatedPMRecipeKey(Identity identity, Material wafer)
        {
            _logger.Debug("RetrieveTheAssociatedPMRecipeKey with the {0} wafer. PM/ChamberID : {1}/{2}", wafer.ToString(), identity.ActorType, identity.ChamberID);
            try
            {
                TestErrors.CheckTestError(ErrorID.GetPMRecipeError);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"Unable to find the PM RecipeKey associated with the {wafer.ToString()} wafer. PM / ChamberID : {identity.ActorType} / {identity.ChamberID}";
                    throw new Exception(msgErr);
                }
                else
                {
                    DataflowActor dfa = currentDataflowRecipe?.Actors.Values.FirstOrDefault(a => a.ActorType == identity?.ActorType);
                    if (dfa != null)
                    {
                        lock (currentDataflowRecipe.StartedDataflowLock)
                        {
                            if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                            {
                                _logger.Debug("Return {0} Guid associated with the {1} wafer", dfa.DataflowActorRecipe.KeyForAllVersion, wafer.ToString());
                                return dfa.DataflowActorRecipe.KeyForAllVersion;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.GetPMRecipeError, $"RetrieveTheAssociatedPMRecipeKey Exception: {ex.Message}");
            }
            return null;
        }

        public DataflowActorRecipe GetPPRecipeByMaterialAndBySide(Identity identity, Material wafer, Side waferSide)
        {
            var recipeInfo = new RecipeInfo();
            _logger.Debug("GetPPRecipeByMaterialAndBySide with the {0} wafer. PP/ChamberID : {1}/{2}", wafer.ToString(), identity.ActorType, identity.ChamberID);
            try
            {
                TestErrors.CheckTestError(ErrorID.GetPMRecipeError);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"Unable to find the PM RecipeKey associated with the {wafer.ToString()} wafer. PM / ChamberID : {identity.ActorType} / {identity.ChamberID}";
                    throw new Exception(msgErr);
                }
                else
                {
                    // TODO should retrieve the ADCs that are coming next
                    var adcDfas = currentDataflowRecipe?.Actors.Values.Where(a => a.ActorType == ActorType.ADC);
                    if (adcDfas != null)
                    {
                        foreach (var adcDfa in adcDfas)
                        {
                            if (adcDfa.Inputs.All(i => i.ResultType.GetSide() == waferSide
                                          || i.ResultType.GetSide() == Side.Unknown))
                                return adcDfa.DataflowActorRecipe;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.GetADCRecipeForSideError, $"GetPPRecipeByMaterialAndBySide Exception: {ex.Message}");
            }

            NotifyError(ErrorID.GetADCRecipeForSideError, $"GetPPRecipeByMaterialAndBySide Failed");

            return null;
        }


        public void RecipeStarted(Identity identity, Material wafer)
        {
            _logger.Debug("RecipeStarted with the {0} wafer. PM/ChamberID : {1}/{2}", wafer.ToString(), identity.ActorType, identity.ChamberID);
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeExecutionError_DFErrorOnRecipeStarted);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(wafer?.ProcessJobID);
                if (currentDataflowRecipe == null)
                {
                    string msgErr = $"RecipeStarted Failed. Unknown recipe on material {wafer.ToString()}. PM / ChamberID : {identity.ActorType} / {identity.ChamberID}";
                    throw new Exception(msgErr);
                }
                else
                {
                    DataflowActor dfa = currentDataflowRecipe.Actors.Values.FirstOrDefault(a => a.ActorType == identity?.ActorType);
                    if (dfa != null)
                    {
                        lock (currentDataflowRecipe.StartedDataflowLock)
                        {
                            if (currentDataflowRecipe.StartedDataflow.TryGetValue(wafer.GUIDWafer, out DataflowInstance dfi))
                            {
                                dfi.ChangePMActorStatus(identity.ActorType, ActorRecipeStatus.Executing);
                                //Useful for aborting a recipe.
                                dfi.AssignIdentity(identity);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.RecipeExecutionError_DFErrorOnRecipeStarted, $"RecipeStarted Exception: {ex.Message}");

                _logger.Debug($"[AbortRecipeExecution] Request for {identity.ToString()}");
                Task.Run(() => _pmdfServiceCB.AbortRecipeExecution(identity));
            }
        }

        public void SelectRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            throw new NotImplementedException();
        }

        public void AbortRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeAbortingFailed);

                _logger.Debug("AbortRecipe {0} .....", dfRecipeInfo.Name);
                var jodIds = GetJobIdsByDFRecipe(dfRecipeInfo);
                lock (_listDFRecipeLock)
                {
                    foreach (var jobId in jodIds)
                    {
                        _listDataflowRecipe.Remove(jobId);
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.RecipeAbortingFailed, $"AbortRecipe Exception: {ex.Message}");
            }
        }

        public void ReInitialize()
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.InitializationError);

                CurrentDFStatus = CurrentDFStatus.ChangeState_Initializing();
                _logger.Debug("Start of DF manager initialization");
                lock (_listDFRecipeLock)
                {
                    _listDataflowRecipe.Clear();
                }
                _alarmOperations.ReInitialize();
                CurrentDFStatus = CurrentDFStatus.ChangeState_Idle();
                _logger.Debug("DF manager initialized");
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.InitializationError, $"Initialization Exception: {ex.Message}");
            }
        }

        public void NotifyError(ErrorID errorID, string message)
        {
            if (_alarmOperations != null)
            {
                _logger.Error($"ErrorID: {errorID.ToString()} - {message}");
                _alarmOperations.SetAlarm(_identity, errorID);
            }
            else
            {
                string msgErr = $"Unable to run the AlarmOperations.SetAlarm() method";
                _logger.Error(msgErr);
            }
        }

        private void PrepareStartOfRecipe(DataflowRecipeInfo dfRecipeInfo, Material wafer)
        {
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeStartingError_PrepareDFRecipe);

                _logger.Debug("Prepare StartOfRecipe {0} with {1} wafer", dfRecipeInfo.Name, wafer.ToString());
                DataflowRecipe currentDataFlowRecipe = LoadDataFlowRecipe(dfRecipeInfo, wafer?.ProcessJobID);
                DataflowInstance df = new DataflowInstance(currentDataFlowRecipe);
                lock (currentDataFlowRecipe.StartedDataflowLock)
                {
                    if (!currentDataFlowRecipe.StartedDataflow.ContainsKey(wafer.GUIDWafer))
                    {
                        df.Wafer = wafer;
                        currentDataFlowRecipe.StartedDataflow.Add(wafer.GUIDWafer, df);
                    }
                }
                if (currentDataFlowRecipe != null
                        && !_listDataflowRecipe.ContainsKey(wafer?.ProcessJobID))
                {
                    lock (_listDFRecipeLock)
                    {
                        _listDataflowRecipe.Add(wafer?.ProcessJobID, currentDataFlowRecipe);
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.RecipeStartingError_PrepareDFRecipe, $"PrepareStartOfRecipe Exception: {ex.Message}");
            }
        }

        public List<JobRecipeInfo> GetRecipesToBeInterrupted(string jobId)
        {
            _logger.Debug($"Get recipes to be interrupted for JobId = {jobId}");
            var recipesToBeInterrupted = new List<JobRecipeInfo>();
            try
            {
                TestErrors.CheckTestError(ErrorID.GetRecipesToAbortError);

                lock (_listDFRecipeLock)
                {
                    List<DataflowRecipe> dataflowRecipes = new List<DataflowRecipe>();
                    const string allJobsIdentifier = "#AllJobs";
                    switch (jobId)
                    {
                        case allJobsIdentifier:
                            dataflowRecipes.AddRange(GetAllDataflowRecipes());
                            break;

                        default:
                            DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(jobId);
                            if (currentDataflowRecipe != null)
                                dataflowRecipes.Add(currentDataflowRecipe);
                            break;
                    }
                    if (!dataflowRecipes.Any())
                    {
                        string msgErr = $"Unknown recipe for job id {jobId}.";
                        throw new Exception(msgErr);
                    }
                    else
                    {
                        foreach (var currentDataflowRecipe in dataflowRecipes)
                        {
                            lock (currentDataflowRecipe.StartedDataflowLock)
                            {
                                foreach (var kvp in currentDataflowRecipe.StartedDataflow)
                                {
                                    DataflowInstance dataflowInstance = kvp.Value;
                                    foreach (DataflowActorValues actorValues in dataflowInstance.DataflowActorValues)
                                    {
                                        if (actorValues.DataflowActorStatus == ActorRecipeStatus.Executing)
                                        {
                                            Guid recipeKey = actorValues.DataflowActorRecipe.KeyForAllVersion;
                                            JobRecipeInfo jobRecipeInfo = new JobRecipeInfo(actorValues.DataflowActor.DataflowActorManager.Identity, recipeKey, dataflowInstance.Wafer);
                                            jobRecipeInfo.DataflowRecipeInfo = currentDataflowRecipe.DataflowRecipeInfo;
                                            recipesToBeInterrupted.Add(jobRecipeInfo);
                                            _logger.Debug("Recipe = {0}. PM/ChamberId = {1}/{2}", actorValues.DataflowActorRecipe.Name, actorValues.DataflowActor.DataflowActorManager.Identity.ActorType, actorValues.DataflowActor.DataflowActorManager.Identity.ChamberID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.GetRecipesToAbortError, $"GetRecipesToBeInterrupted Exception: {ex.Message}");
            }
            return recipesToBeInterrupted;
        }

        public void UpdateDataflowActorStatusForJobId(string jobId, ActorRecipeStatus status)
        {
            _logger.Debug($"Update DataflowActorStatus for JobId = {jobId} to {status.ToString()}");
            try
            {
                TestErrors.CheckTestError(ErrorID.RecipeExecutingError_DFUpdateStatus);

                DataflowRecipe currentDataflowRecipe = GetDataflowRecipeByProcessJobID(jobId);
                if (currentDataflowRecipe != null)
                {
                    lock (currentDataflowRecipe.StartedDataflowLock)
                    {
                        foreach (var kvp in currentDataflowRecipe.StartedDataflow)
                        {
                            DataflowInstance dataflowInstance = kvp.Value;
                            foreach (DataflowActorValues actorValues in dataflowInstance.DataflowActorValues)
                            {
                                actorValues.DataflowActorStatus = status;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.RecipeExecutingError_DFUpdateStatus, $"UpdateDFRecipeInstanceStatus Exception: {ex.Message}");
            }
        }

        private bool IsActorAborted(DataflowInstance dfi, ActorType actorType)
        {
            return dfi.RetrieveActorStatus(actorType) == ActorRecipeStatus.Aborted;
        }

        #region ICommunicationOperationsCB

        public void CommunicationEstablished()
        {
            _logger.Information("Connection DF-PM Succeed");
        }

        public void CommunicationInterrupted()
        {
            _logger.Information("Connection DF-PM Lost");
        }

        public void CommunicationCheck()
        {
            try
            {
                bool isHere = _pmdfServiceCB.AskAreYouThere();
                if (!isHere)
                    _communicationOperations.AttemptCommunicationFailedOrCommunicationLost();
                else
                    _communicationOperations.AttemptCommunicationSucceed();
            }
            catch (Exception ex)
            {
                _communicationOperations.AttemptCommunicationFailedOrCommunicationLost();
            }
        }

        public void OnErrorAcknowledged(Identity identity, ErrorID error)
        {
            // Acknowledgement for Datflow only
            // According to errorId, doing action in a switch case if needed
        }

        public void OnErrorReset(Identity identity, ErrorID error)
        {
            // Alarm reset for Datflow only
            // According to errorId, doing action in a switch case if needed
        }

        public void SetCriticalErrorState(Identity identity, ErrorID errorID)
        {
            switch (identity.ActorType)
            {
                case ActorType.Unknown:
                    break;

                case ActorType.DEMETER:
                case ActorType.BrightField2D:
                case ActorType.Darkfield:
                case ActorType.BrightFieldPattern:
                case ActorType.Edge:
                case ActorType.NanoTopography:
                case ActorType.LIGHTSPEED:
                case ActorType.BrightField3D:
                case ActorType.EdgeInspect:
                case ActorType.ANALYSE:
                case ActorType.HeLioS:
                case ActorType.Argos:
                    _pmdfServiceCB.SetPMInCriticalErrorState(identity, errorID);
                    break;

                case ActorType.HardwareControl:
                    break;

                case ActorType.ADC:
                    break;

                case ActorType.DataflowManager:
                    SetDFInCriticalErrorState(errorID);
                    break;

                default:
                    break;
            }
        }

        private void SetDFInCriticalErrorState(ErrorID errorID)
        {
            CurrentDFStatus = CurrentDFStatus.ChangeState_Maintenance_InError(errorID);
        }
        #endregion ICommunicationOperationsCB

        #region IDFPostProcessCB
        public void DFPostProcessStarted(Identity identity, Side side, String processJobID, Guid materialGuid)
        {
            try
            {
                var material = GetMaterialFromDatflowRecipeByProcessJobID(processJobID, materialGuid);
                if (material == null)
                    throw new Exception("Material was not identified from DFPostProcessStarted event");

                // Update PP Actor status
                UpdatePPActorStatusByWafer(identity, side, processJobID, materialGuid, ActorRecipeStatus.Executing);

                // Update TC DF Recipe status
                var dfRecipeInfo = GetDataflowRecipeInfo(identity, processJobID, materialGuid);
                Task.Run(() => _utodfServiceCB.DFRecipeProcessStarted(dfRecipeInfo));

            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.InvalidPPRecipeStartedError, $"DFRecipeProcessStarted Exception: {ex.Message}");
            }
        }

        public void DFPostProcessComplete(Identity identity, Side side, String processJobID, Guid materialGUID, DataflowRecipeStatus status)
        {
            try
            {
                var material = GetMaterialFromDatflowRecipeByProcessJobID(processJobID, materialGUID);
                if (material == null)
                    throw new Exception("Material was not identified from DFPostProcessComplete event");

                // Update PP Actor status
                var arStatus = status == DataflowRecipeStatus.Terminated ? ActorRecipeStatus.Terminated : ActorRecipeStatus.Error;
                UpdatePPActorStatusByWafer(identity, side, processJobID, materialGUID, arStatus);

                if (IfAllDataflowActorsAreTerminated(material) && (_dfServerConfiguration.EndProcessBehavior == DF_EndProcessBehavior.DFRecipCompleteAfterPostProcess))
                {
                    // Update TC DF Recipe status
                    var dfRecipeInfo = GetDataflowRecipeInfo(identity, processJobID, materialGUID);
                    Task.Run(() => _utodfServiceCB.DFRecipeProcessComplete(dfRecipeInfo, material, status));
                }

            }
            catch (Exception ex)
            {
                NotifyError(ErrorID.InvalidPPRecipeCompleteError, $"DFPostProcessComplete Exception: {ex.Message}");
            }
        }

        public Guid GetMaterialDF(CWaferReport waferReport)
        {
            return new Guid(waferReport.sWaferID);
        }
        #endregion
    }
}
