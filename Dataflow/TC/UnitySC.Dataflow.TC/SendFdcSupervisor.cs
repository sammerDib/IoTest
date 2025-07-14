using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Dataflow.Configuration;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Manager
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class SendFdcSupervisor : ISendFdcService, ISendFdcServiceCallback
    {
        private InstanceContext _instanceContext;
        private DuplexServiceInvoker<ISendFdcService> _sendFdcService;
        private ILogger _logger;
        private IMessenger _messenger;

        public SendFdcSupervisor()
        {
            _messenger=ClassLocator.Default.GetInstance<IMessenger>();

            _logger = new SerilogLogger<ISendFdcService>();
            _instanceContext = new InstanceContext(this);
            ServiceAddress _serviceAddress = ClassLocator.Default.GetInstance<DFServerConfiguration>().DataAccessAddress;
            _sendFdcService = new DuplexServiceInvoker<ISendFdcService>(_instanceContext, "SendFdcService", ClassLocator.Default.GetInstance<SerilogLogger<ISendFdcService>>(), _messenger, s => s.SubscribeToChanges(), _serviceAddress);
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _sendFdcService.InvokeAndGetMessages(l => l.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _sendFdcService.InvokeAndGetMessages(l => l.UnSubscribeToChanges());
        }
        public Response<VoidResult> RequestAllFDCsUpdate()
        {
            return _sendFdcService.InvokeAndGetMessages(l => l.RequestAllFDCsUpdate());
        }

        //Callback
        public void SendFDCs(List<FDCData> fdcsToSend)
        {
            _messenger.Send(new SendFDCListMessage() { FDCsData = fdcsToSend });
        }

     
    }
}
