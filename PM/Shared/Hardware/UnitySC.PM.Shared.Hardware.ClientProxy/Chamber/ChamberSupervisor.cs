using CommunityToolkit.Mvvm.Messaging;

using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Chamber
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChamberSupervisor : IChamberService, IChamberServiceCallback
    {
        private InstanceContext _instanceContext;
        private DuplexServiceInvoker<IChamberService> _chamberService;
        private IMessenger _messenger;

        public ChamberSupervisor(IMessenger messenger, ActorType? actorType)
        {
            _instanceContext = new InstanceContext(this);
            string endPoint = "ChamberService";
            if (actorType != null)
            {
                endPoint = actorType + endPoint;
            }

            _chamberService = new DuplexServiceInvoker<IChamberService>(_instanceContext, endPoint, ClassLocator.Default.GetInstance<ILogger<IChamberService>>(), messenger, s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));
            _messenger = messenger;
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _chamberService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _chamberService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _chamberService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        void IChamberServiceCallback.UpdateDataAttributesCallback(List<DataAttribute> values)
        {
            _messenger.Send(new DataAttributesChangedMessages() { DataAttributes = values });
        }

        public Response<List<string>> GetWebcamUrls()
        {
            return _chamberService.TryInvokeAndGetMessages(s => s.GetWebcamUrls());
        }

        public async Task<Response<VoidResult>> ResetProcess()
        {
            var task = _chamberService.InvokeAndGetMessagesAsync(s => s.ResetProcess());
            await task;
            return task.Result;
        }

        public Response<bool> SetChamberLightState(bool value)
        {
            return _chamberService.TryInvokeAndGetMessages(s => s.SetChamberLightState(value));
        }
    }
}
