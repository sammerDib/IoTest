using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Chiller
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ChillerSupervisor : IChillerSupervisor
    {
        private readonly IMessenger _messenger;
        private readonly DuplexServiceInvoker<IChillerService> _chillerService;

        public ChillerSupervisor(ILogger<IChillerService> logger, IMessenger messenger)
        {
            _messenger = messenger;

            var instanceContext = new InstanceContext(this);
            var serviceAddress = ClientConfiguration.GetServiceAddress(ActorType.EMERA);
            _chillerService = new DuplexServiceInvoker<IChillerService>(instanceContext, "EMERAChillerService", logger,
                messenger, s => s.SubscribeToChanges(), serviceAddress);
        }

        public void UpdateTemperatureCallback(double value)
        {
            _messenger.Send(new TemperatureChangedMessage { Temperature = value });
        }

        public void UpdateConstantFanSpeedModeCallback(ConstFanSpeedMode value)
        {
            _messenger.Send(new FanSpeedModeChangedMessage { ConstFanSpeedMode = value });
        }
        
        public void UpdateFanSpeedCallback(double value)
        {
            _messenger.Send(new FanSpeedChangedMessage { FanSpeed = value });
        }

        public void UpdateMaxCompressionSpeedCallback(double value)
        {
            _messenger.Send(new CompressionChangedMessage { Compression = value });
        }

        public void UpdateAlarms(AlarmDetection value)
        {
            _messenger.Send(new AlarmChangedMessage { Alarm = value });
        }

        public void UpdateLeaks(LeakDetection value)
        {
            _messenger.Send(new LeakDetectionChangedMessage { Leak = value });
        }

        public void UpdateMode(ChillerMode value)
        {
            _messenger.Send(new ChillerModeChangedMessage { Mode = value });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _chillerService.TryInvokeAndGetMessages(l => l.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _chillerService.TryInvokeAndGetMessages(l => l.UnSubscribeToChanges());
        }

        public Response<VoidResult> Reset()
        {
            return _chillerService.InvokeAndGetMessages(s => s.Reset());
        }

        public Response<VoidResult> SetTemperature(double value)
        {
            return _chillerService.InvokeAndGetMessages(s => s.SetTemperature(value));
        }

        public Response<VoidResult> SetConstFanSpeedMode(ConstFanSpeedMode mode)
        {
            return _chillerService.InvokeAndGetMessages(s => s.SetConstFanSpeedMode(mode));
        }

        public Response<VoidResult> SetChillerMode(ChillerMode mode)
        {
            return _chillerService.InvokeAndGetMessages(s => s.SetChillerMode(mode));
        }

        public Response<VoidResult> SetFanSpeed(double value)
        {
            return _chillerService.InvokeAndGetMessages(s => s.SetFanSpeed(value));
        }

        public Response<VoidResult> SetMaxCompressionSpeed(double value)
        {
            return _chillerService.InvokeAndGetMessages(s => s.SetMaxCompressionSpeed(value));
        }
    }
}
