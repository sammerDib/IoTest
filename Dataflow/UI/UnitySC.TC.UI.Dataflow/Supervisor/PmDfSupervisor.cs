using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using UnitySC.Shared.Data;
using Material = UnitySC.Shared.Data.Material;


namespace UnitySC.TC.UI.Dataflow
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class PmDfSupervisor : IPMDFService, IPMDFServiceCB
    {
        private InstanceContext _instanceContext;
        private DuplexServiceInvoker<IPMDFService> _pmDfService;
        private ILogger _logger;
        private IMessenger _messenger;

        public PmDfSupervisor()
        {
            _messenger = new WeakReferenceMessenger();

            _logger = new SerilogLogger<IPMDFService>();
            _instanceContext = new InstanceContext(this);
            //"net.tcp://localhost:2222"
            ServiceAddress _serviceAddress = new ServiceAddress() { Host = "localhost", Port = 2222 };
            _pmDfService = new DuplexServiceInvoker<IPMDFService>(_instanceContext, "PMDFService", ClassLocator.Default.GetInstance<SerilogLogger<IPMDFService>>(), _messenger, s => s.SubscribeToChanges(), _serviceAddress);
        }

        public Response<VoidResult> StartRecipeRequest(Identity identity, Material material)
        {
            return _pmDfService.InvokeAndGetMessages(l => l.StartRecipeRequest(identity, material));
        }
        public Response<VoidResult> RecipeExecutionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState status)
        {
            return _pmDfService.InvokeAndGetMessages(l => l.RecipeExecutionComplete(identity, material, recipeKey, results, status));
        }

        public Response<VoidResult> RecipeStarted(Identity identity, Material wafer)
        {
            return _pmDfService.InvokeAndGetMessages(l => l.RecipeStarted(identity, wafer));
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _pmDfService.InvokeAndGetMessages(l => l.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _pmDfService.InvokeAndGetMessages(l => l.UnSubscribeToChanges());
        }
        public Response<VoidResult> NotifyError(Identity identity, Message errorMessage)
        {
            return _pmDfService.InvokeAndGetMessages(l => l.NotifyError(identity, errorMessage));
        }

        //Callback
        public void AreYouThere()
        {
        }

        public void StartRecipeExecution(Identity identity, Guid? recipeKey, DataflowRecipeInfo dfRecipeInfo, Material wafer)
        {
        }

        public void AbortRecipeExecution(Identity identity)
        {
        }

        public bool AskAreYouThere()
        {
            return true;
        }

        public Response<VoidResult> NotifyDataCollectionChanged(Identity identity, ModuleDataCollection moduleDataCollection)
        {
            return _pmDfService.InvokeAndGetMessages(l => l.NotifyDataCollectionChanged(identity, moduleDataCollection));
        } 

        public void OnErrorAcknowledged(Identity identity, ErrorID error)
        {
            throw new NotImplementedException();
        }

        public void OnErrorReset(Identity identity, ErrorID error)
        {
            throw new NotImplementedException();
        }

        public void SetPMInCriticalErrorState(Identity identity, ErrorID error)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> NotifyFDCCollectionChanged(Identity identity, List<FDCData> dataCollection)
        {
            throw new NotImplementedException();
        }

        public void RequestAllFDCsUpdate()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SendAda(Identity identity, Material material, string adaContent, String adaFullPathFileName)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> RecipeAcquisitionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState status)
        {
            throw new NotImplementedException();
        }
    }
}
