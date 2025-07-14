using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface.Chuck;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Client.Proxy.Chuck
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ChuckSupervisor : IDMTChuckService, IDMTChuckServiceCallback
    {
        private readonly ILogger _logger;
        private IMessenger _messenger;
        private readonly DuplexServiceInvoker<IDMTChuckService> _dmtChuckService;

        private ChuckVM _chuck;

        public ChuckSupervisor(ILogger<ChuckSupervisor> logger, ILogger<IDMTChuckService> serviceLogger, IMessenger messenger, ActorType? actorType)
        {
            var instanceContext = new InstanceContext(this);

            var endPoint = "ChuckService";
            if (actorType != null)
                endPoint = actorType + endPoint;
            _dmtChuckService = new DuplexServiceInvoker<IDMTChuckService>(instanceContext, endPoint, serviceLogger, messenger,
                s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));

            var res = _dmtChuckService.InvokeAndGetMessages(s => s.GetCurrentState());
            _logger = logger;
            _messenger = messenger;
        }

        public Response<ChuckState> GetCurrentState()
        {
            return _dmtChuckService.InvokeAndGetMessages(s => s.GetCurrentState());
        }

        public ChuckVM ChuckVM
        {
            get
            {
                if (_chuck == null)
                {
                    _chuck = new ChuckVM(this);
                }
                return _chuck;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _dmtChuckService.InvokeAndGetMessages(s => s.SubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Chuck subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _dmtChuckService.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Chuck unsubscribe error");
            }

            return resp;
        }

        public Response<VoidResult> RefreshAllValues()
        {
            return _dmtChuckService.InvokeAndGetMessages(s => s.RefreshAllValues());
        }

        public void UpdateWaferPresenceCallback(MaterialPresence waferPresence)
        {
            if (_chuck != null)
                _chuck.ChuckWaferPresence = waferPresence;
        }

        public void TagChangedCallback(string tag)
        {
            _messenger.Send(new TagChangedMessage() { Tag = tag });
        }

        public Response<RfidTag> GetTag()
        {
            return _dmtChuckService.TryInvokeAndGetMessages(s => s.GetTag());
        }

        public void UpdateChuckIsInLoadingPositionCallback(bool loadingPosition)
        {
            throw new NotImplementedException();
        }

        public void StateChangedCallback(ChuckState state)
        {
            _messenger.Send(state);
        }
    }
}
