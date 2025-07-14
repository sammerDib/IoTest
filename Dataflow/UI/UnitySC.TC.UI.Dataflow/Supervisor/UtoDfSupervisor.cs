using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Controls;

using UnitySC.Shared.Data;
using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.TC.UI.Dataflow
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class UtoDfSupervisor : IUTODFService, IUTODFServiceCB
    {
        private InstanceContext _instanceContext;
        private DuplexServiceInvoker<IUTODFService> _utoDfService;
        private ILogger _logger;
        private IMessenger _messenger;

        public UtoDfSupervisor()
        {
            _messenger = new WeakReferenceMessenger();
            _logger = new SerilogLogger<IUTODFService>();
            _instanceContext = new InstanceContext(this);
            //"net.tcp://localhost:2222"
            ServiceAddress _serviceAddress = new ServiceAddress() { Host = "localhost", Port = 2222 };

            _utoDfService = new DuplexServiceInvoker<IUTODFService>(_instanceContext, "UTODFService", ClassLocator.Default.GetInstance<SerilogLogger<IUTODFService>>(), _messenger, s => s.SubscribeToChanges(), _serviceAddress);
            // _iPMDFService = new DuplexServiceInvoker<IPMDFService>(_instanceContext, "PMDFService", ClassLocator.Default.GetInstance<SerilogLogger<IPMDFService>>(), messenger, s => s.SubscribeToChanges(), _serviceAddress);            
        }

        public Response<VoidResult> AbortJobID(string jobID)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.AbortJobID(jobID));
        }
        public Response<VoidResult> AbortRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.AbortRecipe(dfRecipeInfo));
        }


        public void AreYouThere()
        {

        }

        public void DFRecipeAdded(DataflowRecipeInfo dfRecipeInfo)
        {

        }

        public void DFRecipeDeleted(DataflowRecipeInfo dfRecipeInfo)
        {
            throw new NotImplementedException();
        }

        public void DFRecipeProcessComplete(DataflowRecipeInfo dfRecipeInfo, Material wafer, DataflowRecipeStatus status)
        {
            throw new NotImplementedException();
        }

        public void DFRecipeProcessStarted(DataflowRecipeInfo dfRecipeInfo)
        {

        }

        public Response<List<EquipmentConstant>> ECGetAllRequest()
        {
            return _utoDfService.InvokeAndGetMessages(l => l.ECGetAllRequest());
        }

        public Response<List<EquipmentConstant>> ECGetRequest(List<int> id)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.ECGetRequest(id));
        }

        public Response<bool> ECSetRequest(EquipmentConstant ecid)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.ECSetRequest(ecid));
        }

        public void FireEvent(CommonEvent ce)
        {
            throw new NotImplementedException();
        }

        public Response<List<CommonEvent>> GetAll()
        {
            return _utoDfService.InvokeAndGetMessages(l => l.GetAll());
        }

        public Response<List<Alarm>> GetAllAlarms()
        {
            return _utoDfService.InvokeAndGetMessages(l => l.GetAllAlarms());
        }

        public Response<List<DataflowRecipeInfo>> GetAllDataflowRecipes(List<ActorType> actors)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.GetAllDataflowRecipes(actors));
        }

        public Response<VoidResult> Initialize()
        {
            return _utoDfService.InvokeAndGetMessages(l => l.Initialize());
        }

        public Response<VoidResult> NotifyAlarmChanged(Alarm alarm)
        {
            throw new NotImplementedException();
        }

        public void NotifyDataCollectionChanged(ModuleDataCollection moduleDataCollection)
        {
            throw new NotImplementedException();
        }

        public void NotifyFDCCollectionChanged(List<FDCData> fdcDataCollection)
        {
            throw new NotImplementedException();
        }

        public void OnChangedDFStatus(string value)
        {
            throw new NotImplementedException();
        }


        public void PMRecipeProcessComplete(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer, RecipeTerminationState status)
        {

        }

        public void PMRecipeProcessStarted(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer)
        {

        }

        public Response<VoidResult> RequestAllFDCsUpdate()
        {
            throw new NotImplementedException();
        }

        public void ResetAlarm(List<Alarm> alarms)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> ResetAlarmFromUTO(Alarm alarm)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SelectRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.SelectRecipe(dfRecipeInfo));
        }

        public void SetAlarm(List<Alarm> alarms)
        {

        }

        public void SetECValues(List<EquipmentConstant> equipmentConstants)
        {

        }

        public Response<VoidResult> StartJob_Material(DataflowRecipeInfo dfRecipeInfo, Material material)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.StartJob_Material(dfRecipeInfo, material));
        }

        public Response<UTOJobProgram> StartRecipeDF(DataflowRecipeInfo dfRecipeInfo, string processJobID, List<Guid> wafersGuid)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.StartRecipeDF(dfRecipeInfo, processJobID, wafersGuid));
        }
        public Response<UTOJobProgram> GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.GetUTOJobProgramForARecipeDF(dfRecipeInfo));
        }
        public void StopCancelAllJobs()
        {
             
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _utoDfService.InvokeAndGetMessages(l => l.SubscribeToChanges());
        }

        public Response<List<StatusVariable>> SVGetAllRequest()
        {
            return _utoDfService.InvokeAndGetMessages(l => l.SVGetAllRequest());
        }

        public Response<List<StatusVariable>> SVGetRequest(List<int> id)
        {
            return _utoDfService.InvokeAndGetMessages(l => l.SVGetRequest(id));
        }

        public void SVSetMessage(List<StatusVariable> statusVariables)
        {

        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _utoDfService.InvokeAndGetMessages(l => l.UnSubscribeToChanges());
        }

        Response<bool> IUTODFService.AreYouThere()
        {
            throw new NotImplementedException();
        }

        public void PMRecipeAcquisitionComplete(ActorType pmType, DataflowRecipeInfo dfRecipeInfo, Material wafer, RecipeTerminationState status)
        {
            throw new NotImplementedException();
        }
    }
}
