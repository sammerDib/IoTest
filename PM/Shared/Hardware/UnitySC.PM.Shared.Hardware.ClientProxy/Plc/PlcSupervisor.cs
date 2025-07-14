using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Plc
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class PlcSupervisor : IPlcService, IPlcServiceCallback
    {
        private readonly ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<IPlcService> _plcService;        

        private PlcVM _plcVM;
        
        public PlcSupervisor(ILogger<PlcSupervisor> logger, ILogger<IPlcService> serviceLogger, IMessenger messenger, ActorType? actorType)
        {
            var _instanceContext = new InstanceContext(this);

            var endPoint = "PlcService";
            if (actorType != null)
                endPoint = actorType + endPoint;
            
            _plcService = new DuplexServiceInvoker<IPlcService>(_instanceContext, endPoint, serviceLogger, messenger,
                s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));

            _logger = logger;
            _messenger = messenger;
        }

        public PlcVM PlcVM
        {
            get
            {
                if (_plcVM == null)
                {
                    _plcVM = new PlcVM(this);
                }
                return _plcVM;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _plcService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _plcService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _plcService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<VoidResult> SmokeDetectorResetAlarm()
        {
            return _plcService.TryInvokeAndGetMessages(s => s.SmokeDetectorResetAlarm());
        }

        public Response<VoidResult> Restart()
        {
            return _plcService.TryInvokeAndGetMessages(s => s.Restart());
        }

        public Response<VoidResult> Reboot()
        {
            return _plcService.TryInvokeAndGetMessages(s => s.Reboot());
        }

        public Response<VoidResult> CustomCommand(string customCmd)
        {
            return _plcService.TryInvokeAndGetMessages(s => s.CustomCommand(customCmd));
        }

        void IPlcServiceCallback.StateChangedCallback(string state)
        {
            throw new NotImplementedException();
        }

        void IPlcServiceCallback.IdChangedCallback(string value)
        {
            _messenger.Send(new IdChangedMessage() { Id = value });
        }

        void IPlcServiceCallback.CustomChangedCallback(string value)
        {
            _messenger.Send(new CustomChangedMessage() { Custom = value });
        }

        void IPlcServiceCallback.AmsNetIdChangedCallback(string value)
        {
            _messenger.Send(new AmsNetIdChangedMessage() { AmsNetId = value });
        }
    }
}
