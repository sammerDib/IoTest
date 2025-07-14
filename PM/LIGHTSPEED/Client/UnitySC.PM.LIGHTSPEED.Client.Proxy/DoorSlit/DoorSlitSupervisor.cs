using System;
using System.ServiceModel;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Client.Proxy.Acquisition
{
    public class DoorSlitSupervisor : IAcquisitionService, IAcquisitionServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private DuplexServiceInvoker<IAcquisitionService> _acquisitionService;
        private IMessenger _messenger;

        public DoorSlitSupervisor(ILogger<DoorSlitSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            _acquisitionService = new DuplexServiceInvoker<IAcquisitionService>(_instanceContext, "AcquisitionService", ClassLocator.Default.GetInstance<ILogger<IAcquisitionService>>(), messenger);
            _logger = logger;
            _messenger = messenger;
        }

        public Response<VoidResult> Close()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> Open()
        {
            return _acquisitionService.InvokeAndGetMessages(s => s.Open());
        }

        public void StatusChanged(int test)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            throw new NotImplementedException();
        }
    }
}
