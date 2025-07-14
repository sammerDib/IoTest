using System;
using System.ServiceModel;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LSAcquisitionService : DuplexServiceBase<IAcquisitionServiceCallback>, IAcquisitionService
    {
        private HardwareManager _hardwareManager;

        public LSAcquisitionService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
        }

        public Response<VoidResult> Close()
        {
            throw new NotImplementedException("Close");
        }

        public Response<VoidResult> Open()
        {
            throw new NotImplementedException("Open");
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Acquisition UnSubscribe");
                Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to acquisition"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Acquisition Subscribe");
                Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to "));
            });
        }
    }
}
