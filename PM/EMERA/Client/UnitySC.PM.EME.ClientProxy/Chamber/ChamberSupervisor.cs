using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Chamber;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Chamber
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ChamberSupervisor : IEMEChamberService, IEMEChamberServiceCallback
    {
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly DuplexServiceInvoker<IEMEChamberService> _service;

        public ChamberSupervisor(ILogger<IEMEChamberService> logger, IMessenger messenger)
        {
            var instanceContext = new InstanceContext(this);
            var serviceAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _service = new DuplexServiceInvoker<IEMEChamberService>(instanceContext, "EMERAChamberService", logger,
                messenger, s => s.SubscribeToChanges(), serviceAddress);

            _logger = logger;
            _messenger = messenger;
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _service.InvokeAndGetMessages(s => s.SubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Chamber subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _service.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Chamber unsubscribe error");
            }

            return resp;
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _service.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<EMEChamberConfig> GetChamberConfiguration()
        {
            return _service.TryInvokeAndGetMessages(s => s.GetChamberConfiguration());
        }

        public void UpdateIsInMaintenanceCallback(bool value)
        {
            _messenger.Send(new IsInMaintenanceChangedMessage() { IsInMaintenance = value });
        }

        public void UpdateArmNotExtendedCallback(bool value)
        {
            _messenger.Send(new ArmNotExtendedChangedMessage() { ArmNotExtended = value });
        }

        public void UpdateInterlocksCallback(InterlockMessage interlock)
        {
            _messenger.Send(interlock);
        }

        public void UpdateSlitDoorPositionCallback(SlitDoorPosition position)
        {
            _messenger.Send(new SlitDoorPositionChangedMessage() { SlitDoorPosition = position });
        }
        
        public Response<VoidResult> OpenSlitDoor()
        {
            return _service.TryInvokeAndGetMessages(s => s.OpenSlitDoor());
        }

        public Response<VoidResult> CloseSlitDoor()
        {
            return _service.TryInvokeAndGetMessages(s => s.CloseSlitDoor());
        }
    }
}
