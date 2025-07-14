using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Chuck;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Chuck
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ChuckSupervisorEx : IEMEChuckService, IEMEChuckServiceCallback, IChuckServiceCallback
    {
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly ServiceInvoker<IEMEChuckService> _chuckService;

        public ChuckSupervisorEx(ILogger<IEMEChuckService> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            var customAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _chuckService = new DuplexServiceInvoker<IEMEChuckService>(new InstanceContext(this), "EMERAChuckService",
                logger, messenger, s => s.SubscribeToChanges(), customAddress);
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            throw new NotImplementedException();
        }

        public Response<bool> ClampWafer(WaferDimensionalCharacteristic wafer)
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.ClampWafer(wafer));
        }

        public Response<bool> ReleaseWafer(WaferDimensionalCharacteristic wafer)
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.ReleaseWafer(wafer));
        }

        public Response<ChuckState> GetCurrentState()
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.GetCurrentState());
        }

        public Response<VoidResult> RefreshAllValues()
        {
            return _chuckService.InvokeAndGetMessages(s => s.RefreshAllValues());
        }       

        public void UpdateChuckIsInLoadingPositionCallback(bool loadingPosition)
        {
            _messenger.Send(new ChuckLoadingPositionChangedMessage() { ChuckIsInLoadingPosition = loadingPosition });
        }

        public void UpdateWaferPresenceCallback(MaterialPresence waferPresence)
        {
            _messenger.Send(new WaferPresenceChangedMessage() { WaferPresence = waferPresence });
        }

        public void StateChangedCallback(ChuckState state)
        {
            _messenger.Send(state);
        }
    }
}
