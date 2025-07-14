using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.FDC
{
    [CallbackBehaviorAttribute(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FDCSupervisor : IFDCService,IFDCServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private DuplexServiceInvoker<IFDCService> _fdcService;
        private IMessenger _messenger;

        public ActorType? Actor { get; set; }

        public FDCSupervisor(ILogger<FDCSupervisor> logger, IMessenger messenger, ActorType? actorType)
        {
            _instanceContext = new InstanceContext(this);
            var endPoint = "FDCService";
            Actor = (ActorType)actorType;
            if (actorType != null)
                endPoint = actorType + endPoint;
            _fdcService = new DuplexServiceInvoker<IFDCService>(_instanceContext, endPoint, ClassLocator.Default.GetInstance<SerilogLogger<IFDCService>>(), messenger,s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));

           _logger = logger;
            _messenger = messenger;
        }
        public Response<List<FDCItemConfig>> GetFDCsConfig()
        {
            return _fdcService.TryInvokeAndGetMessages(s => s.GetFDCsConfig());

        }

        public Response<VoidResult> SetFDCsConfig(List<FDCItemConfig> fdcItemsConfig)
        {
            return _fdcService.TryInvokeAndGetMessages(s => s.SetFDCsConfig(fdcItemsConfig));
        }

        public Response<VoidResult> ResetFDC(string fdcName)
        {
            return _fdcService.TryInvokeAndGetMessages(s => s.ResetFDC(fdcName));

        }

        public Response<VoidResult> SetInitialCountdownValue(string fdcName, double initialCountdownValue)
        {
            return _fdcService.TryInvokeAndGetMessages(s => s.SetInitialCountdownValue(fdcName, initialCountdownValue));

        }
        public Response<FDCData> GetFDC(string fdcName)
        {
            return _fdcService.TryInvokeAndGetMessages(s => s.GetFDC(fdcName));
        }

        public void UpdateFDCDataCallback(FDCData fdcData)
        {
            _messenger.Send(new SendFDCMessage() { Data = fdcData });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _fdcService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _fdcService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }
    }
}
