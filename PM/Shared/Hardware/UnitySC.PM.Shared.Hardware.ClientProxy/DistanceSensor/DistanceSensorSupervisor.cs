using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.DistanceSensor
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class DistanceSensorSupervisor : IDistanceSensorService, IDistanceSensorServiceCallback
    {
        private IMessenger _messenger;
        private DuplexServiceInvoker<IDistanceSensorService> _distanceSensorService;

        private DistanceSensorVM _distanceSensorVM;

        /// <summary>
        /// Constructor
        /// </summary>
        public DistanceSensorSupervisor(ILogger<DistanceSensorSupervisor> logger, ILogger<IDistanceSensorService> serviceLogger, IMessenger messenger, ActorType? actorType)
        {
            var instanceContext = new InstanceContext(this);
            var endPoint = "DistanceSensorService";
            if (actorType != null)
                endPoint = actorType + endPoint;

            _distanceSensorService = new DuplexServiceInvoker<IDistanceSensorService>(instanceContext, endPoint, serviceLogger, messenger, 
                s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));
            _messenger = messenger;
        }

        public DistanceSensorVM DistanceSensorVM
        {
            get
            {
                if (_distanceSensorVM == null)
                {
                    _distanceSensorVM = new DistanceSensorVM(this);
                }
                return _distanceSensorVM;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _distanceSensorService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _distanceSensorService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _distanceSensorService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<VoidResult> CustomCommand(string customCmd)
        {
            return _distanceSensorService.TryInvokeAndGetMessages(s => s.CustomCommand(customCmd));
        }

        void IDistanceSensorServiceCallback.StateChangedCallback(DeviceState state)
        {
            _messenger.Send(new StateChangedMessage() { State = state });
        }

        void IDistanceSensorServiceCallback.DistanceChangedCallback(double distance)
        {
            _messenger.Send(new DistanceChangedMessages() { Distance = distance });
        }

        void IDistanceSensorServiceCallback.IdChangedCallback(string value)
        {
            _messenger.Send(new IdChangedMessage() { Id = value });
        }

        void IDistanceSensorServiceCallback.CustomChangedCallback(string value)
        {
            _messenger.Send(new CustomChangedMessage() { Custom = value });
        }        
    }
}
