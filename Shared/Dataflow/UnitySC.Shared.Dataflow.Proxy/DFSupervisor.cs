using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Shared.Dataflow.Proxy
{
    /// <summary>
    /// Proxy to supervise hardware
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class DFSupervisor : IPMDFService, IPMDFServiceCB
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private DuplexServiceInvoker<IPMDFService> _pmdfServiceInvoker;
        private IPMTCManager _pmTCService;
        private IAlarmOperationsCB _alarmOperationsCB;

        public delegate void OnRecipeStausChangedHandler(DataflowRecipeInfo dfRecipe);

        private DateTime _lastConnectionDetectionTime = DateTime.MinValue;

        public bool IsConnected
        {
            get
            {
                return DateTime.Now.Subtract(_lastConnectionDetectionTime).TotalSeconds < 10;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DFSupervisor(ILogger<DFSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            _pmdfServiceInvoker = new DuplexServiceInvoker<IPMDFService>(_instanceContext, "PMDFService", ClassLocator.Default.GetInstance<SerilogLogger<IPMDFService>>(), messenger, null, ClassLocator.Default.GetInstance<ModuleConfiguration>().DataFlowAddress);
            _logger = logger;

            _pmTCService = ClassLocator.Default.GetInstance<IPMTCManager>();
            _alarmOperationsCB = ClassLocator.Default.GetInstance<IAlarmOperationsCB>();
        }

        #region Management methods

        public bool DoInitiateConnection()
        {
            try
            {
                SubscribeToChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CloseConnection()
        {
            _pmdfServiceInvoker.DisposeChannel();
        }

        public void OnChangedDFStatus(string value)
        {
        }

        #endregion Management methods

        #region IPMDFService Subscription

        public Response<VoidResult> SubscribeToChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error($"PM subscribe error: {ex.Message}");
                throw;
            }
            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PM UnSubscribe error");
            }
            return resp;
        }

        #endregion IPMDFService Subscription

        public void AreYouThere()
        {
            _lastConnectionDetectionTime = DateTime.Now;
        }

        #region IPMDFService

        public Response<VoidResult> StartRecipeRequest(Identity identity, Material material)
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.StartRecipeRequest(identity, material));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PM StartRecipeRequest error");
                throw;
            }
            return resp;
        }
        public Response<VoidResult> RecipeExecutionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState state)
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.RecipeExecutionComplete(identity, material, recipeKey, results, state));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PM RecipeExecutionComplete error");
                throw;
            }
            return resp;
        }

        public Response<VoidResult> RecipeAcquisitionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState state)
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.RecipeAcquisitionComplete(identity, material, recipeKey, results, state));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PM RecipeAcquisitionComplete error");
                throw;
            }
            return resp;
        }
        public Response<VoidResult> RecipeStarted(Identity identity, Material material)
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.RecipeStarted(identity, material));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PM RecipeExecutionComplete error");
                throw;
            }
            return resp;
        }
        public Response<VoidResult> NotifyError(Identity identity, Message errorMessage)
        {
            var resp = new Response<VoidResult>();
            try
            {
                _logger.Information($"Notify PM in error : {errorMessage.UserContent}");
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.NotifyError(identity, errorMessage));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Notify PM in error");
                throw;
            }
            return resp;
        }

        public Response<VoidResult> NotifyDataCollectionChanged(Identity identity, ModuleDataCollection pmDataCollection)
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.NotifyDataCollectionChanged(identity, pmDataCollection));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Module NotifyDataCollectionChanged in error");
                throw;
            }
            return resp;
        }

        public Response<VoidResult> NotifyFDCCollectionChanged(Identity identity, List<FDCData> fdcsDataCollection)
        {
            var resp = new Response<VoidResult>();
            try
            {
                bool silentLogger = !_logger.IsVerboseEnabled();
                resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.NotifyFDCCollectionChanged(identity, fdcsDataCollection), silentLogger);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "NotifyFDCCollectionChanged in error");
                throw;
            }
            return resp;
        }

        public Response<VoidResult> SendAda(Identity identity, Material material, string adaContent, String adaFullPathFileName)
        {
            var resp = new Response<VoidResult>();
            resp = _pmdfServiceInvoker.TryInvokeAndGetMessages(s => s.SendAda(identity, material,adaContent, adaFullPathFileName));
            return resp;
        }

        #endregion IPMDFService

        #region IPMDFServiceCB

        public void StartRecipeExecution(Identity identity, Guid? recipeKey, DataflowRecipeInfo dfRecipeInfo, Material material)
        {
            if ((identity.ActorType == _pmTCService.PMIdentity.ActorType) &&
                (identity.ChamberID == _pmTCService.PMIdentity.ChamberID))
            {
                Task.Run(() =>
                {
                    _pmTCService.StartRecipeExecution_pmtcs(recipeKey, dfRecipeInfo, material);
                });
            }
        }
        // Abort from DF to PM - Callback DF -> PM
        public void AbortRecipeExecution(Identity identity)
        {
            if ((identity != null) && (identity.ActorType == _pmTCService.PMIdentity.ActorType) &&
                (identity.ChamberID == _pmTCService.PMIdentity.ChamberID))
            {
                Task.Run(() =>
                {
                    _pmTCService.AbortRecipeExecution_pmtcs();
                });
            }
        }

        public bool AskAreYouThere()
        {
            return true;
        }

        public void OnErrorAcknowledged(Identity identity, ErrorID errorID)
        {

        }

        public void OnErrorReset(Identity identity, ErrorID errorID)
        {
            if ((identity != null) && (identity.ActorType == _pmTCService.PMIdentity.ActorType) &&
                (identity.ChamberID == _pmTCService.PMIdentity.ChamberID))
            {
                Task.Run(() =>
                {
                    _alarmOperationsCB.OnErrorReset(identity, errorID);
                });
            }
        }

        public void SetPMInCriticalErrorState(Identity identity, ErrorID errorID)
        {
            if ((identity != null) && (identity.ActorType == _pmTCService.PMIdentity.ActorType) &&
                (identity.ChamberID == _pmTCService.PMIdentity.ChamberID))
            {
                _alarmOperationsCB.SetCriticalErrorState(identity, errorID);
            }
        }

        public void RequestAllFDCsUpdate()
        {
            _pmTCService.RequestAllFDCsUpdate();
        }

        #endregion IPMDFServiceCB
    }
}
