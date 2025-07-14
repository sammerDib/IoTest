using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.Dataflow.Configuration;
using UnitySC.Dataflow.Operations.Interface;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Dataflow.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class UTODFService : DuplexServiceBase<IUTODFServiceCB>, IUTODFService, IUTODFServiceCB,
                                                IAlarmServiceCB, IStatusVariableServiceCB, ICommonEventServiceCB, IEquipmentConstantServiceCB
    {
        private object _lock = new object();
        private DFServerConfiguration _dfServerConfiguration;

        private IUTODFOperations _utodfOperations;
        public IUTODFOperations UTODFOperations { get => _utodfOperations; set => _utodfOperations = value; }

        private IPMDFServiceCB _pmdfServiceCB;
        private SendFdcSupervisor _sendFdcSupervisor;

        public UTODFService(ILogger logger) : base(logger, ExceptionType.DataflowException)
        {
            _dfServerConfiguration = ClassLocator.Default.GetInstance<DFServerConfiguration>();
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        public override void Init()
        {
            base.Init();
            UTODFOperations = ClassLocator.Default.GetInstance<IUTODFOperations>();
            var configuration = ClassLocator.Default.GetInstance<IAutomationConfiguration>();
            UTODFOperations.AlarmOperations.Init();
            UTODFOperations.CEOperations.Init(configuration.CEConfigurationFilePath);
            UTODFOperations.SVOperations.Init(configuration.SVConfigurationFilePath);
            UTODFOperations.RecipeOperations.Init();
            _pmdfServiceCB = ClassLocator.Default.GetInstance<IPMDFServiceCB>();
            _sendFdcSupervisor = ClassLocator.Default.GetInstance<SendFdcSupervisor>();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to UTO change"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("UTO subscribed to Dataflow");
                base.Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to UTO change"));
            });
        }

        public void OnChangedDFStatus(string value)
        {
            InvokeCallback(i => i.OnChangedDFStatus(value));
        }

        Response<List<Alarm>> IAlarmService.GetAllAlarms()
        {
            return InvokeDataResponse<List<Alarm>>(messageContainer =>
            {
                try
                {
                    if (UTODFOperations?.AlarmOperations != null)
                    {
                        _logger.Information("Get active alarms for UTO");
                        List<Alarm> alarms = UTODFOperations.AlarmOperations.GetAllAlarms();
                        messageContainer.Add(new Message(MessageLevel.Information, "Alarms list sent = " + string.Join(",", alarms)));
                        return alarms;
                    }
                    else
                        throw new Exception("alarmsOperation is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new List<Alarm>();
                }
            });
        }

        Response<List<CommonEvent>> ICommonEventService.GetAll()
        {
            return InvokeDataResponse<List<CommonEvent>>(messageContainer =>
            {
                try
                {
                    if (UTODFOperations?.CEOperations != null)
                    {
                        _logger.Information("Get all CEID for UTO");
                        List<CommonEvent> ceids = UTODFOperations?.CEOperations.CEGetAll();
                        messageContainer.Add(new Message(MessageLevel.Information, "CEID List sent = " + string.Join(",", ceids)));
                        return ceids;
                    }
                    else
                        throw new Exception("ceOperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new List<CommonEvent>();
                }
            });
        }

        Response<List<EquipmentConstant>> IEquipmentConstantService.ECGetAllRequest()
        {
            return InvokeDataResponse<List<EquipmentConstant>>(messageContainer =>
            {
                try
                {
                    if (UTODFOperations?.ECOperations != null)
                    {
                        _logger.Information("Get all CEID for UTO");
                        List<EquipmentConstant> ecids = UTODFOperations?.ECOperations.ECGetAllRequest();
                        messageContainer.Add(new Message(MessageLevel.Information, "ECID list sent = " + string.Join(",", ecids)));
                        return ecids;
                    }
                    else
                        throw new Exception("_ecOperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new List<EquipmentConstant>();
                }
            });
        }

        public Response<List<EquipmentConstant>> ECGetRequest(List<int> ids)
        {
            return InvokeDataResponse<List<EquipmentConstant>>(messageContainer =>
            {
                try
                {
                    if (UTODFOperations?.ECOperations != null)
                    {
                        _logger.Information("Get ECID for UTO");
                        List<EquipmentConstant> ecids = UTODFOperations?.ECOperations.ECGetRequest(ids);
                        messageContainer.Add(new Message(MessageLevel.Information, "ECIDs sent"));
                        return ecids;
                    }
                    else
                        throw new Exception("_ecOperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new List<EquipmentConstant>();
                }
            });
        }

        Response<bool> IEquipmentConstantService.ECSetRequest(EquipmentConstant ecid)
        {
            return InvokeDataResponse<bool>(messageContainer =>
            {
                try
                {
                    if (UTODFOperations?.ECOperations != null)
                    {
                        _logger.Information("Set ECID from UTO");
                        bool status = UTODFOperations.ECOperations.ECSetRequest(ecid);
                        messageContainer.Add(new Message(MessageLevel.Information, $"ECID set from UTO: {ecid.Name} = {ecid.ValueAsString} [status {status.ToString()}]"));
                        return status;
                    }
                    else
                        throw new Exception("_ecOperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return false;
                }
            });
        }

        Response<VoidResult> IRecipeDFService.SelectRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Information("SelectRecipe from UTO");
                        UTODFOperations.RecipeOperations.SelectRecipe(dfRecipeInfo);
                        messageContainer.Add(new Message(MessageLevel.Information, $"SelectRecipe {dfRecipeInfo.Name} from UTO"));
                    }
                    else
                        throw new Exception("[SelectRecipe] UTODFperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                }
            });
        }

        Response<VoidResult> IRecipeDFService.StartJob_Material(DataflowRecipeInfo dfRecipeInfo, Material material)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Information("StartJob_Material from UTO");
                        UTODFOperations.RecipeOperations.StartJob_Material(dfRecipeInfo, material);

                        messageContainer.Add(new Message(MessageLevel.Information, $"Start JobID {material.ProcessJobID} with a material LP{material.LoadportID}.S{material.SlotID} from UTO"));
                        _logger.Debug("DFRecipeProcessStarted : " + dfRecipeInfo?.Name);
                        Task.Run(() => DFRecipeProcessStarted(dfRecipeInfo));
                        messageContainer.Add(new Message(MessageLevel.Information, $"Notify the UTO that the job {material.ProcessJobID} with a material LP {material.LoadportID}.S{material.SlotID} has started"));
                    }
                    else
                    {
                        _logger.Error("[StartJob_Material] UTODFperations is null !!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                }
            });
        }

        public Response<UTOJobProgram> StartRecipeDF(DataflowRecipeInfo dfRecipeInfo, string processJobID, List<Guid> wafersGuid)
        {
            return InvokeDataResponse<UTOJobProgram>(messageContainer =>
            {
                UTOJobProgram newJobProgram;
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Information("StartRecipeDF from UTO");
                        newJobProgram = UTODFOperations.RecipeOperations.StartRecipeDF(dfRecipeInfo, processJobID, wafersGuid);
                        messageContainer.Add(new Message(MessageLevel.Information, $"StartRecipe {dfRecipeInfo.Name} from UTO"));
                        return newJobProgram;
                    }
                    else
                    {
                        _logger.Error("[StartRecipeDF] UTODFperations is null !!");
                        return new UTOJobProgram();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new UTOJobProgram();
                }
            });
        }

        Response<VoidResult> IRecipeDFService.AbortRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Debug("[Request from UTO] AbortRecipe " + dfRecipeInfo.Name);
                        //Abort PM/PP recipes in progress.
                        var jobIds = UTODFOperations.RecipeOperations.GetJobIdsByDFRecipe(dfRecipeInfo);
                        foreach (var jobId in jobIds)
                        {
                            _logger.Debug($"[AbortRecipe] Request for jobId = {jobId}");

                            var jobRecipeInfoList = UTODFOperations.RecipeOperations.GetRecipesToBeInterrupted(jobId);
                            foreach (var jobRecipeInfoItem in jobRecipeInfoList)
                            {
                                _logger.Debug($"[AbortRecipeExecution] Request for {jobRecipeInfoItem.Identity.ToString()}");
                                Task.Run(() => _pmdfServiceCB.AbortRecipeExecution(jobRecipeInfoItem.Identity));
                                _logger.Debug($"[UpdateDFRecipeInstanceStatus] DFRecipe =  {jobRecipeInfoItem.DataflowRecipeInfo.Name} Wafer = {jobRecipeInfoItem.Wafer.ToString()} terminating status");
                                UTODFOperations.RecipeOperations.UpdateDFRecipeInstanceStatus(jobRecipeInfoItem.DataflowRecipeInfo, jobRecipeInfoItem.Wafer, DataflowRecipeStatus.Terminated);
                            }
                        }
                        _logger.Debug($"AbortRecipe {dfRecipeInfo.Name} from UTO");
                        UTODFOperations.RecipeOperations.AbortRecipe(dfRecipeInfo);
                        messageContainer.Add(new Message(MessageLevel.Information, $"AbortRecipe {dfRecipeInfo.Name} from UTO"));
                    }
                    else
                        throw new Exception("[AbortRecipe] UTODFperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                }
            });
        }

        Response<VoidResult> IRecipeDFService.AbortJobID(string jobID)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Debug($"[Request from UTO] Abort JobID {jobID}");
                        UTODFOperations.RecipeOperations.UpdateDataflowActorStatusForJobId(jobID, ActorRecipeStatus.Aborted);
                        var jobRecipeInfoList = UTODFOperations.RecipeOperations.GetRecipesToBeInterrupted(jobID);
                        foreach (var jobRecipeInfoItem in jobRecipeInfoList)
                        {
                            _logger.Debug($"[AbortRecipeExecution] Request for {jobRecipeInfoItem.Identity.ToString()}");
                            Task.Run(() => _pmdfServiceCB.AbortRecipeExecution(jobRecipeInfoItem.Identity));
                            _logger.Debug($"[UpdateDFRecipeInstanceStatus] DFRecipe =  {jobRecipeInfoItem.DataflowRecipeInfo.Name} Wafer = {jobRecipeInfoItem.Wafer.ToString()} terminating status");
                            UTODFOperations.RecipeOperations.UpdateDFRecipeInstanceStatus(jobRecipeInfoItem.DataflowRecipeInfo, jobRecipeInfoItem.Wafer, DataflowRecipeStatus.Terminated);
                        }
                    }
                    else
                        throw new Exception("[AbortJobID] UTODFperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                }
            });
        }

        Response<VoidResult> IRecipeDFService.Initialize()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Debug("[Initialize from UTO] Init");
                        UTODFOperations.RecipeOperations.ReInitialize();
                        messageContainer.Add(new Message(MessageLevel.Information, $"Initialize from UTO"));
                    }
                    else
                        throw new Exception("[Initialize] UTODFperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                }
            });
        }

        Response<List<DataflowRecipeInfo>> IRecipeDFService.GetAllDataflowRecipes(List<ActorType> actors)
        {
            return InvokeDataResponse<List<DataflowRecipeInfo>>(messageContainer =>
            {
                AutoSubscribe();
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Information("GetAllDataflowRecipes from UTO. TooKey = " + _dfServerConfiguration.ToolKey);
                        List<DataflowRecipeInfo> recipes = UTODFOperations.RecipeOperations.GetAllRecipes(actors, _dfServerConfiguration.ToolKey);
                        messageContainer.Add(new Message(MessageLevel.Information, "GetAllDataflowRecipes " + string.Join(",", recipes + " from UTO")));
                        return recipes;
                    }
                    else
                    {
                        throw new Exception("[GetAllDataflowRecipes] UTODFperations is null !!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new List<DataflowRecipeInfo>();
                }
            });
        }

        Response<List<StatusVariable>> IStatusVariableService.SVGetAllRequest()
        {
            return InvokeDataResponse<List<StatusVariable>>(messageContainer =>
            {
                try
                {
                    if (UTODFOperations?.SVOperations != null)
                    {
                        _logger.Information("SVGetAllRequest from UTO");
                        List<StatusVariable> svids = UTODFOperations.SVOperations.SVGetAllRequest();
                        messageContainer.Add(new Message(MessageLevel.Information, "SVGetAllRequest " + string.Join(",", svids + " from UTO")));
                        return svids;
                    }
                    else
                        throw new Exception("[SVGetAllRequest] SVOperations is null !!");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new List<StatusVariable>();
                }
            });
        }

        public Response<List<StatusVariable>> SVGetRequest(List<int> ids)
        {
            return InvokeDataResponse<List<StatusVariable>>(messageContainer =>
            {
                try
                {
                    if (UTODFOperations?.SVOperations != null)
                    {
                        _logger.Information("SVGetRequest from UTO");
                        List<StatusVariable> sids = UTODFOperations.SVOperations.SVGetRequest(ids);
                        messageContainer.Add(new Message(MessageLevel.Information, "SVGetRequest " + String.Join(",", sids) + " from UTO"));
                        return sids;
                    }
                    else
                    {
                        throw new Exception("[SVGetRequest] SVOperations is null !!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new List<StatusVariable>();
                }
            });
        }

        public void SetAlarm(Alarm alarmID)
        {
            SetAlarm(new List<Alarm> { alarmID });
        }

        public void SetAlarm(List<Alarm> alarms)
        {
            InvokeCallback(i => i.SetAlarm(alarms));
        }

        public void ResetAlarm(Alarm alarmID)
        {
            ResetAlarm(new List<Alarm>() { alarmID });
        }

        public void ResetAlarm(List<Alarm> alarms)
        {
            InvokeCallback(i => i.ResetAlarm(alarms));
        }

        public void FireEvent(CommonEvent ecid)
        {
            InvokeCallback(i => i.FireEvent(ecid));
        }

        public void SetECValue(EquipmentConstant equipmentConstant)
        {
            SetECValues(new List<EquipmentConstant> { equipmentConstant });
        }

        public void SetECValues(List<EquipmentConstant> equipmentConstants)
        {
            InvokeCallback(i => i.SetECValues(equipmentConstants));
        }

        public void DFRecipeProcessStarted(DataflowRecipeInfo dfRecipeInfo)
        {
            InvokeCallback(i => i.DFRecipeProcessStarted(dfRecipeInfo));
        }

        public void DFRecipeProcessComplete(DataflowRecipeInfo dfRecipeInfo, Material wafer, DataflowRecipeStatus status)
        {
            _logger.Information($"[CycleLog] DF Recipe Process Complete : material {wafer.ToString()}");
            InvokeCallback(i => i.DFRecipeProcessComplete(dfRecipeInfo, wafer, status));
        }

        public void DFRecipeAdded(DataflowRecipeInfo dfRecipeInfo)
        {
            InvokeCallback(i => i.DFRecipeAdded(dfRecipeInfo));
        }

        public void DFRecipeDeleted(DataflowRecipeInfo dfRecipeInfo)
        {
            InvokeCallback(i => i.DFRecipeDeleted(dfRecipeInfo));
        }

        public void PMRecipeProcessStarted(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer)
        {
            _logger.Information($"[CycleLog] PM {pmType} Recipe Acquisition Started : material {wafer.ToString()}");
            InvokeCallback(i => i.PMRecipeProcessStarted(pmType, dfRecipeInfo, wafer));
        }
        public void PMRecipeAcquisitionComplete(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer, RecipeTerminationState status)
        {
            _logger.Information($"[CycleLog] PM {pmType} Recipe Acquisition Complete : material {wafer.ToString()}");
            InvokeCallback(i => i.PMRecipeAcquisitionComplete(pmType, dfRecipeInfo, wafer, status)); // to UTO 
        }

        public void PMRecipeProcessComplete(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer, RecipeTerminationState status)
        {
            _logger.Information($"[CycleLog] PM {pmType} Recipe Process Complete : material {wafer.ToString()}");
            InvokeCallback(i => i.PMRecipeProcessComplete(pmType, dfRecipeInfo, wafer, status)); // to UTO

            try
            {
                if (UTODFOperations != null)
                {
                    // Update dataflowRecipe status
                    if (UTODFOperations.RecipeOperations.IfAllDataflowActorsAreTerminated(wafer))
                    {                        
                        _logger.Debug("Update dataflowRecipe status : Terminated");
                        UTODFOperations.RecipeOperations.UpdateDFRecipeInstanceStatus(dfRecipeInfo, wafer, DataflowRecipeStatus.Terminated);
                    }
                    // according to behavior in configuration, Signal DF recipe complete 
                    if (UTODFOperations.RecipeOperations.IfAllDataflowPMActorsAreTerminated(wafer) && (_dfServerConfiguration.EndProcessBehavior == DF_EndProcessBehavior.DFRecipCompleteAfterPMProcess))
                    {
                        //all Terminated
                        _logger.Debug("DFRecipeProcessComplete for the Wafer : " + wafer.ToString());
                        Task.Run(() => DFRecipeProcessComplete(UTODFOperations.RecipeOperations.GetAssociatedDataflowFullInfo(wafer), wafer, DataflowRecipeStatus.Terminated));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public void SVSetMessage(List<StatusVariable> statusVariables)
        {
            InvokeCallback(i => i.SVSetMessage(statusVariables));
        }

        public void SVSetMessage(StatusVariable statusVariables)
        {
            SVSetMessage(new List<StatusVariable>() { statusVariables });
        }

        public Response<VoidResult> NotifyAlarmChanged(Alarm alarm)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (UTODFOperations != null)
                {
                    _logger.Information("Notify Alarm Changed");
                    UTODFOperations.AlarmOperations.NotifyAlarmChanged(alarm);
                    messageContainer.Add(new Message(MessageLevel.Information, $"Alarm {alarm.Name} changed (Active={alarm.Active} Ack={alarm.Acknowledged}"));
                }
                else
                {
                    _logger.Error("UTOPMOperations is null !!");
                }
            });
        }

        Response<bool> IUTODFService.AreYouThere()
        {
            AutoSubscribe();
            return new Response<bool>() { Result = true };
        }

        public void NotifyFDCCollectionChanged(List<FDCData> fdcsDataCollection)
        {
            foreach (var fdcData in fdcsDataCollection)
            {
                _logger.Verbose($"Notify NotifyFDCCollectionChanged {fdcData.Name} : {fdcData.ValueFDC.Value}");
            }
            InvokeCallback(i => i.NotifyFDCCollectionChanged(fdcsDataCollection));
        }

        public void StopCancelAllJobs()
        {
            InvokeCallback(i => i.StopCancelAllJobs());
        }

        private void AutoSubscribe()
        {
            if (GetNbClientsConnected() <= 0)
            {
                lock (_lock)
                    base.Subscribe();
                _logger.Information("UTO subscribed to Dataflow");
            }
        }

        public Response<VoidResult> RequestAllFDCsUpdate()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (UTODFOperations != null)
                {
                    _logger.Verbose("Notify RequestAllFDCsUpdate to PM and its own FDCs");
                    ClassLocator.Default.GetInstance<FDCManager>().RequestAllFDCsUpdate();
                    _pmdfServiceCB.RequestAllFDCsUpdate();
                    _sendFdcSupervisor.RequestAllFDCsUpdate();
                    messageContainer.Add(new Message(MessageLevel.Information, $"Request all FDCs update"));
                }
                else
                {
                    _logger.Error("UTOPMOperations is null !!");
                }
            });
        }

        public Response<UTOJobProgram> GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo)
        {
            return InvokeDataResponse<UTOJobProgram>(messageContainer =>
            {
                UTOJobProgram newJobProgram;
                try
                {
                    if (UTODFOperations != null)
                    {
                        _logger.Information("GetUTOJobProgramForARecipeDF from UTO");
                        newJobProgram = UTODFOperations.RecipeOperations.GetUTOJobProgramForARecipeDF(dfRecipeInfo);
                        messageContainer.Add(new Message(MessageLevel.Information, $"GetUTOJobProgramForARecipeDF {dfRecipeInfo.Name} from UTO"));
                        return newJobProgram;
                    }
                    else
                    {
                        _logger.Error("[GetUTOJobProgramForARecipeDF] UTODFperations is null !!");
                        return new UTOJobProgram();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    messageContainer.Add(new Message(MessageLevel.Information, ex.Message));
                    return new UTOJobProgram();
                }
            });
        }
    }
}
