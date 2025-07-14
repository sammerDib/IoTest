using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Timers;

using Agileo.Common.Logging;
using Agileo.Common.Tracing;

using UnitySC.DataAccess.Dto;
using UnitySC.DataFlow.ProcessModules.Drivers.WCF;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager.Driver
{
    [CallbackBehavior(
        ConcurrencyMode = ConcurrencyMode.Reentrant,
        UseSynchronizationContext = false)]
    public class DataFlowManagerDriver : WcfDriver<IUTODFService>, IUTODFService, IUTODFServiceCB
    {
        #region Fields

        private readonly Timer _areYouThereTimer;
        private bool _areYouThereInProgress;
        private uint _currentRetryNumber;
        private readonly uint _maxRetryNumber;
        #endregion Fields

        #region Events

        public delegate bool EquipmentConstantChangedDelegate(
            object sender,
            EquipmentConstantChangedEventArgs e);

        public delegate bool StatusVariableChangedDelegate(
            object sender,
            StatusVariableChangedEventArgs e);

        public event EventHandler<AlarmRaisedEventArgs> AlarmRaised;

        public event EventHandler<AlarmClearedEventArgs> AlarmCleared;

        public event EventHandler StopCancelAllJobsRequested;

        public event EquipmentConstantChangedDelegate EquipmentConstantChanged;

        public event EventHandler<CollectionEventEventArgs> EventFired;

        public event StatusVariableChangedDelegate StatusVariableChanged;

        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeStarted;

        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeCompleted;

        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeAdded;

        public event EventHandler<DataFlowRecipeEventArgs> DataFlowRecipeDeleted;

        public event EventHandler<ActorRecipeEventArgs> ProcessModuleRecipeStarted;

        public event EventHandler<ActorRecipeEventArgs> ProcessModuleRecipeCompleted;

        public event EventHandler<ActorRecipeEventArgs> ProcessModuleAcquisitionCompleted;

        public event EventHandler<FdcCollectionEventArgs> FdcCollectionChanged;

        #endregion Events

        #region Constructor

        public DataFlowManagerDriver(WcfConfiguration config, ILogger logger)
            : base(config, logger)
        {
            var timeout = TimeSpan.FromSeconds(config.WcfCommunicationCheckDelay);
            _areYouThereTimer = new Timer(timeout.TotalMilliseconds);
            _areYouThereTimer.Elapsed += AreYouThereTimer_Elapsed; //modif cri
            _maxRetryNumber = config.WcfRetryNumber;
        }

        #endregion Constructor

        #region Overrides

        public override bool Connect()
        {
            try
            {
                SubscribeToChanges();
                IsConnected = true;
                _areYouThereTimer.Start();
            }
            catch (Exception e)
            {
                _logger.Debug(new TraceParam(e.Message), "Connect failed in DataFlow driver");
                IsConnected = false;
                throw;
            }

            return IsConnected;
        }

        public override void Disconnect()
        {
            try
            {
                _areYouThereTimer.Stop();
                UnSubscribeToChanges();
            }
            catch (Exception e)
            {
                _logger.Debug(new TraceParam(e.Message), "Disconnect failed in DataFlow driver");
            }

            IsConnected = false;
        }

        #endregion Overrides

        #region IUTODFService

        #region IAlarmService

        public Response<List<Alarm>> GetAllAlarms()
        {
            _logger.Debug($"Service {nameof(GetAllAlarms)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.GetAllAlarms());
            }
            catch (Exception ex)
            {
                _logger.Error("GetAllAlarms() : " + ex);
                return new Response<List<Alarm>>();
            }
        }

        public Response<VoidResult> NotifyAlarmChanged(Alarm alarm)
        {
            _logger.Debug($"Service {nameof(NotifyAlarmChanged)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.NotifyAlarmChanged(alarm));
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(NotifyAlarmChanged)} : " + ex);
                return new Response<VoidResult>();
            }
        }

        #endregion IAlarmService

        #region ICommonEventService

        public Response<List<CommonEvent>> GetAll()
        {
            _logger.Debug($"Service {nameof(GetAll)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.GetAll());
            }
            catch (Exception ex)
            {
                _logger.Error("GetAll() : " + ex);
                return new Response<List<CommonEvent>>();
            }
        }

        #endregion ICommonEventService

        #region IEquipmentConstantService

        public Response<List<EquipmentConstant>> ECGetAllRequest()
        {
            _logger.Debug($"Service {nameof(ECGetAllRequest)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.ECGetAllRequest());
            }
            catch (Exception ex)
            {
                _logger.Error("ECGetAllRequest() : " + ex);
                return new Response<List<EquipmentConstant>>();
            }
        }

        public Response<List<EquipmentConstant>> ECGetRequest(List<int> id)
        {
            _logger.Debug($"Service {nameof(ECGetRequest)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.ECGetRequest(id));
            }
            catch (Exception ex)
            {
                _logger.Error("ECGetRequest() : " + ex);
                return new Response<List<EquipmentConstant>>();
            }
        }

        public Response<bool> ECSetRequest(EquipmentConstant ecid)
        {
            _logger.Debug($"Service {nameof(ECSetRequest)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.ECSetRequest(ecid));
            }
            catch (Exception ex)
            {
                _logger.Error("ECSetRequest() : " + ex);
                return new Response<bool>();
            }
        }

        #endregion IEquipmentConstantService

        #region IRecipeDFService

        public Response<VoidResult> SelectRecipe(DataflowRecipeInfo dfRecipe)
        {
            _logger.Debug($"Service {nameof(SelectRecipe)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.SelectRecipe(dfRecipe));
            }
            catch (Exception ex)
            {
                _logger.Error("SelectRecipe() : " + ex);
                return new Response<VoidResult>();
            }
        }

        public Response<VoidResult> AbortRecipe(DataflowRecipeInfo dfRecipe)
        {
            _logger.Debug($"Service {nameof(AbortRecipe)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.AbortRecipe(dfRecipe));
            }
            catch (Exception ex)
            {
                _logger.Error("AbortRecipe() : " + ex);
                return new Response<VoidResult>();
            }
        }

        public Response<UTOJobProgram> StartRecipeDF(
            DataflowRecipeInfo dfRecipe,
            string processJobID,
            List<Guid> wafersGuid)
        {
            _logger.Debug($"Service {nameof(StartRecipeDF)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(
                    s => s.StartRecipeDF(dfRecipe, processJobID, wafersGuid));
            }
            catch (Exception ex)
            {
                _logger.Error("StartRecipeDF() : " + ex);
                return new Response<UTOJobProgram>();
            }
        }

        public Response<VoidResult> Initialize()
        {
            _logger.Debug($"Service {nameof(Initialize)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.Initialize());
            }
            catch (Exception ex)
            {
                _logger.Error("Initialize() : " + ex);
                return new Response<VoidResult>();
            }
        }

        public Response<List<DataflowRecipeInfo>> GetAllDataflowRecipes(List<ActorType> actors)
        {
            _logger.Debug($"Service {nameof(GetAllDataflowRecipes)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.GetAllDataflowRecipes(actors));
            }
            catch (Exception ex)
            {
                _logger.Error("GetAllDataflowRecipe() : " + ex);
                return new Response<List<DataflowRecipeInfo>>();
            }
        }

        public Response<VoidResult> StartJob_Material(
            DataflowRecipeInfo dfRecipe,
            Material material)
        {
            _logger.Debug($"Service {nameof(StartJob_Material)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(
                    s => s.StartJob_Material(dfRecipe, material));
            }
            catch (Exception ex)
            {
                _logger.Error("StartJob_Material() : " + ex);
                return new Response<VoidResult>();
            }
        }

        public Response<VoidResult> AbortJobID(string jobID)
        {
            _logger.Debug($"Service {nameof(AbortJobID)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(s => s.AbortJobID(jobID));
            }
            catch (Exception ex)
            {
                _logger.Error("AbortJobID() : " + ex);
                return new Response<VoidResult>();
            }
        }

        #endregion IRecipeDFService

        #region IStatusVariableService

        public Response<List<StatusVariable>> SVGetAllRequest()
        {
            _logger.Debug($"Service {nameof(SVGetAllRequest)} has been called");
            return _serviceInvoker.InvokeAndGetMessages(s => s.SVGetAllRequest());
        }

        public Response<List<StatusVariable>> SVGetRequest(List<int> id)
        {
            _logger.Debug($"Service {nameof(SVGetRequest)} has been called");
            return _serviceInvoker.InvokeAndGetMessages(s => s.SVGetRequest(id));
        }

        public Response<VoidResult> RequestAllFDCsUpdate()
        {
            _logger.Debug($"Service {nameof(RequestAllFDCsUpdate)} has been called");
            return _serviceInvoker.InvokeAndGetMessages(s => s.RequestAllFDCsUpdate());
        }

        #endregion IStatusVariableService

        public Response<UTOJobProgram> GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo)
        {
            _logger.Debug($"Service {nameof(GetUTOJobProgramForARecipeDF)} has been called");

            try
            {
                return _serviceInvoker.InvokeAndGetMessages(
                    s => s.GetUTOJobProgramForARecipeDF(dfRecipeInfo));
            }
            catch (Exception ex)
            {
                _logger.Error("GetUTOJobProgramForARecipeDF() : " + ex);
                return new Response<UTOJobProgram>();
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            _logger.Debug($"Service {nameof(SubscribeToChanges)} has been called");

            var resp = new Response<VoidResult>();
            try
            {
                resp = _serviceInvoker.InvokeAndGetMessages(s => s.SubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"DF UTO subscribe error : {ex.Message}");
                throw;
            }

            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            _logger.Debug($"Service {nameof(UnSubscribeToChanges)} has been called");

            var resp = new Response<VoidResult>();
            try
            {
                resp = _serviceInvoker.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "DF UTO UnSubscribe error");
            }

            return resp;
        }

        public Response<bool> AreYouThere()
        {
            _logger.Debug($"Service {nameof(AreYouThere)} has been called");

            var resp = new Response<bool>();
            try
            {
                resp = _serviceInvoker.InvokeAndGetMessages(s => s.AreYouThere());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"DF UTO AreYouThere error : {ex.Message}");
            }

            return resp;
        }

        private void AreYouThereTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsConnected) return;

            if (_areYouThereInProgress)
            {
                return;
            }
            try
            {
                _areYouThereInProgress = true;
                var isThere = AreYouThere().Result;
                if (isThere)
                {
                    _currentRetryNumber = 0;
                    return;
                }

                _currentRetryNumber++;
                if (_currentRetryNumber <= _maxRetryNumber)
                {
                    _logger.Debug($"Server is not detected anymore. Keep Dataflow as connected. retry #{_currentRetryNumber}/{_maxRetryNumber}");
                    return;
                }

                IsConnected = false;
                _logger.Debug("Server is not detected anymore after several retries. Notify connection closed.");
            }
            finally
            {
                _areYouThereInProgress = false;
            }
        }

        #endregion IUTODFService

        #region IUTODFServiceCB

        public void SetAlarm(List<Alarm> alarms)
        {
            _logger.Debug($"Callback {nameof(SetAlarm)} has been called by DataFlow");

            Task.Run(
                    () =>
                    {
                        try
                        {
                            AlarmRaised?.Invoke(this, new AlarmRaisedEventArgs(alarms));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("SetAlarm() : " + ex);
                        }
                    })
                .Wait();
        }

        void IUTODFServiceCB.ResetAlarm(List<Alarm> alarms)
        {
            _logger.Debug(
                $"Callback {nameof(IUTODFServiceCB.ResetAlarm)} has been called by DataFlow");

            Task.Run(
                    () =>
                    {
                        try
                        {
                            AlarmCleared?.Invoke(this, new AlarmClearedEventArgs(alarms));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("ResetAlarm() : " + ex);
                        }
                    })
                .Wait();
        }

        public void StopCancelAllJobs()
        {
            _logger.Debug(
                $"Callback {nameof(IUTODFServiceCB.StopCancelAllJobs)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    try
                    {
                        StopCancelAllJobsRequested?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"{nameof(IUTODFServiceCB.StopCancelAllJobs)}() : " + ex);
                    }
                });
        }

        public void FireEvent(CommonEvent ce)
        {
            _logger.Debug($"Callback {nameof(FireEvent)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    try
                    {
                        EventFired?.Invoke(this, new CollectionEventEventArgs(ce.Name, ce.DataVariables));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("FireEvent() : " + ex);
                    }
                });
        }

        public void SetECValues(List<EquipmentConstant> equipmentConstants)
        {
            _logger.Debug($"Callback {nameof(SetECValues)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    EquipmentConstantChanged?.Invoke(
                        this,
                        new EquipmentConstantChangedEventArgs(equipmentConstants));
                });
        }

        public void DFRecipeProcessStarted(DataflowRecipeInfo dfRecipeInfo)
        {
            _logger.Debug($"Callback {nameof(DFRecipeProcessStarted)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    DataFlowRecipeStarted?.Invoke(this, new DataFlowRecipeEventArgs(dfRecipeInfo));
                });
        }

        public void DFRecipeProcessComplete(
            DataflowRecipeInfo dfRecipeInfo,
            Material wafer,
            DataflowRecipeStatus status)
        {
            _logger.Debug(
                $"Callback {nameof(DFRecipeProcessComplete)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    DataFlowRecipeCompleted?.Invoke(
                        this,
                        new DataFlowRecipeEventArgs(dfRecipeInfo, wafer.ProcessJobID, wafer.SubstrateID, status));
                });
        }

        public void DFRecipeAdded(DataflowRecipeInfo dfRecipeInfo)
        {
            _logger.Debug($"Callback {nameof(DFRecipeAdded)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    DataFlowRecipeAdded?.Invoke(this, new DataFlowRecipeEventArgs(dfRecipeInfo));
                });
        }

        public void DFRecipeDeleted(DataflowRecipeInfo dfRecipeInfo)
        {
            _logger.Debug($"Callback {nameof(DFRecipeDeleted)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    DataFlowRecipeDeleted?.Invoke(this, new DataFlowRecipeEventArgs(dfRecipeInfo));
                });
        }

        public void PMRecipeProcessStarted(
            ActorType pmType,
            DataflowRecipeInfo dfRecipeInfo,
            Material wafer)
        {
            _logger.Debug($"Callback {nameof(PMRecipeProcessStarted)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    ProcessModuleRecipeStarted?.Invoke(
                        this,
                        new ActorRecipeEventArgs(pmType, dfRecipeInfo.Name));
                });
        }

        public void PMRecipeProcessComplete(
            ActorType pmType,
            DataflowRecipeInfo dfRecipeInfo,
            Material wafer,
            RecipeTerminationState status)
        {
            _logger.Debug(
                $"Callback {nameof(PMRecipeProcessComplete)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    ProcessModuleRecipeCompleted?.Invoke(
                        this,
                        new ActorRecipeEventArgs(pmType, dfRecipeInfo?.Name, status));
                });
        }

        public void SVSetMessage(List<StatusVariable> statusVariables)
        {
            _logger.Debug($"Callback {nameof(SVSetMessage)} has been called by DataFlow");

            Task.Run(
                () => StatusVariableChanged?.Invoke(
                    this,
                    new StatusVariableChangedEventArgs(statusVariables)));
        }

        public void OnChangedDFStatus(string value)
        {
            _logger.Debug($"Callback {nameof(OnChangedDFStatus)} has been called by DataFlow");

            //Do nothing
        }

        public void NotifyFDCCollectionChanged(List<FDCData> fdcDataCollection)
        {
            Task.Run(
                () =>
                {
                    FdcCollectionChanged?.Invoke(
                        this,
                        new FdcCollectionEventArgs(fdcDataCollection));
                });
        }

        public void PMRecipeAcquisitionComplete(
            ActorType pmType,
            DataflowRecipeInfo dfRecipeInfo,
            Material wafer,
            RecipeTerminationState status)
        {
            _logger.Debug(
                $"Callback {nameof(PMRecipeAcquisitionComplete)} has been called by DataFlow");

            Task.Run(
                () =>
                {
                    ProcessModuleAcquisitionCompleted?.Invoke(
                        this,
                        new ActorRecipeEventArgs(pmType, dfRecipeInfo?.Name, status));
                });
        }
        #endregion IUTODFServiceCB
    }
}
