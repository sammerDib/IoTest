using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.ClientProxy.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Spectrometer
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class SpectrometerSupervisor : ISpectroService, ISpectroServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<ISpectroService> _spectroService;

        private SpectrometerVM _spectroVM;

        /// <summary>
        /// Constructor
        /// </summary>
        public SpectrometerSupervisor(ILogger<SpectrometerSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            _spectroService = new DuplexServiceInvoker<ISpectroService>(_instanceContext, "HARDWARESpectroService", ClassLocator.Default.GetInstance<SerilogLogger<ISpectroService>>(), messenger, s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.HardwareControl));
            _logger = logger;
            _messenger = messenger;
        }

        public SpectrometerVM SpectroVM
        {
            get
            {
                if (_spectroVM == null)
                {
                    _spectroVM = new SpectrometerVM(this);
                }
                return _spectroVM;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _spectroService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _spectroService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<SpectroSignal> DoMeasure(SpectrometerParamBase param)
        {
            return _spectroService.TryInvokeAndGetMessages(s => s.DoMeasure(param));
        }

        public Response<VoidResult> StartContinuousAcquisition(SpectrometerParamBase param)
        {
            return _spectroService.TryInvokeAndGetMessages(s => s.StartContinuousAcquisition(param));
        }

        public Response<VoidResult> StopContinuousAcquisition()
        {
            return _spectroService.TryInvokeAndGetMessages(s => s.StopContinuousAcquisition());
        }

        void ISpectroServiceCallback.StateChangedCallback(DeviceState state)
        {
            _messenger.Send(new StateChangedMessage() { State = state });
        }

        void ISpectroServiceCallback.RawMeasuresCallback(SpectroSignal spectroSignal)
        {
            SpectroVM.SetRawSignal(spectroSignal);
        }
    }
}
