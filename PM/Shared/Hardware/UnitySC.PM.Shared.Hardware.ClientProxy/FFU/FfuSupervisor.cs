using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Ffu
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class FfuSupervisor : IFfuService, IFfuServiceCallback
    {
        private readonly ILogger _logger;
        private IMessenger _messenger;
        private readonly DuplexServiceInvoker<IFfuService> _ffuService;

        private FfuVM _ffuVM;

        /// <summary>
        /// Constructor
        /// </summary>
        public FfuSupervisor(ILogger<FfuSupervisor> logger, ILogger<IFfuService> serviceLogger, IMessenger messenger, ActorType? actorType)
        {
            var instanceContext = new InstanceContext(this);

            var endPoint = "FfuService";
            if (actorType != null)
                endPoint = actorType + endPoint;
            _ffuService = new DuplexServiceInvoker<IFfuService>(instanceContext, endPoint, serviceLogger, messenger,
                s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));

            _logger = logger;
            _messenger = messenger;
        }

        public FfuVM FfuVM
        {
            get
            {
                if (_ffuVM == null)
                {
                    _ffuVM = new FfuVM(this);
                }
                return _ffuVM;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> PowerOn()
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.PowerOn());
        }

        public Response<VoidResult> PowerOff()
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.PowerOff());
        }

        public Response<VoidResult> SetSpeed(ushort speedPercent)
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.SetSpeed(speedPercent));
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<VoidResult> CustomCommand(string customCmd)
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.CustomCommand(customCmd));
        }

        public Response<Dictionary<string, ushort>> GetDefaultFfuValues()
        {
            return _ffuService.TryInvokeAndGetMessages(s => s.GetDefaultFfuValues());
        }

        void IFfuServiceCallback.StateChangedCallback(DeviceState state)
        {
            _messenger.Send(new StateChangedMessage() { State = state });
        }

        void IFfuServiceCallback.StatusChangedCallback(string status)
        {
            _messenger.Send(new StatusChangedMessage() { Status = status });
        }

        void IFfuServiceCallback.CurrentSpeedChangedCallback(ushort speedPercent)
        {
            _messenger.Send(new CurrentSpeedChangedMessage() { CurrentSpeed = speedPercent });
        }

        void IFfuServiceCallback.TemperatureChangedCallback(double value)
        {
            _messenger.Send(new TemperatureChangedMessage() { Temperature = value });
        }

        void IFfuServiceCallback.WarningChangedCallback(bool value)
        {
            _messenger.Send(new WarningChangedMessage() { Warning = value });
        }

        void IFfuServiceCallback.AlarmChangedCallback(bool value)
        {
            _messenger.Send(new AlarmChangedMessage() { Alarm = value });
        }

        void IFfuServiceCallback.CustomChangedCallback(string value)
        {
            _messenger.Send(new CustomChangedMessage() { Custom = value });
        }       
    }
}
