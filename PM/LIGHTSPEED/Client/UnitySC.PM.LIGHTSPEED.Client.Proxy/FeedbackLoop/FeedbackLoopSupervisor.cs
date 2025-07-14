using System.ServiceModel;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Client.Proxy.FeedbackLoop
{
    public class FeedbackLoopSupervisor : IFeedbackLoopService, IFeedbackLoopServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private DuplexServiceInvoker<IFeedbackLoopService> _feedbackSLoopService;
        private IMessenger _messenger;

        public FeedbackLoopSupervisor(ILogger<FeedbackLoopSupervisor> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _instanceContext = new InstanceContext(this);
            _feedbackSLoopService = new DuplexServiceInvoker<IFeedbackLoopService>(_instanceContext, "FeedbackLoopService", ClassLocator.Default.GetInstance<ILogger<IFeedbackLoopService>>(), _messenger);

            SubscribeToChanges();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> InitializeUpdate()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.InitializeUpdate());
        }

        public Response<VoidResult> PowerOn()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.PowerOn());
        }

        public Response<VoidResult> PowerOff()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.PowerOff());
        }

        public Response<VoidResult> SetPower(int power)
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.SetPower(power));
        }

        public Response<VoidResult> SetCurrent(int current)
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.SetCurrent(current));
        }

        public Response<VoidResult> OpenShutterCommand()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.OpenShutterCommand());
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.CloseShutterCommand());
        }

        public Response<VoidResult> AttenuationHomePosition()
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.AttenuationHomePosition());
        }

        public Response<VoidResult> MoveAbsPosition(double position)
        {
            return _feedbackSLoopService.InvokeAndGetMessages(s => s.MoveAbsPosition(position));
        }

        void IFeedbackLoopServiceCallback.PowerChangedCallback(PowerIlluminationFlow flow, double power, double powerCal_mW, double rfactor)
        {
            _messenger.Send(new PowerChangedMessage() { Flow = flow, Power = power, PowerCal_mW = powerCal_mW, RFactor = rfactor });
        }

        void IFeedbackLoopServiceCallback.WavelengthChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new WavelengthChangedMessage() { Flow = flow, Wavelength = value });
        }

        void IFeedbackLoopServiceCallback.BeamDiameterChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new BeamDiameterChangedMessage() { Flow = flow, BeamDiameter = value });
        }

        void IFeedbackLoopServiceCallback.PowerLaserChangedCallback(double power)
        {
            _messenger.Send(new PowerLaserChangedMessage() { Power = power });
        }

        void IFeedbackLoopServiceCallback.InterlockStatusChangedCallback(string value)
        {
            _messenger.Send(new InterlockStatusChangedMessage() { InterlockStatus = value });
        }

        void IFeedbackLoopServiceCallback.LaserTemperatureChangedCallback(double value)
        {
            _messenger.Send(new LaserTemperatureChangedMessage() { LaserTemperature = value });
        }

        void IFeedbackLoopServiceCallback.PsuTemperatureChangedCallback(double value)
        {
            _messenger.Send(new PsuTemperatureChangedMessage() { PsuTemperature = value });
        }

        void IFeedbackLoopServiceCallback.ShutterIrisPositionChangedCallback(string value)
        {
            _messenger.Send(new ShutterIrisPositionChangedMessages() { ShutterIrisPosition = value });
        }

        void IFeedbackLoopServiceCallback.AttenuationPositionChangedCallback(double value)
        {
            _messenger.Send(new AttenuationPositionChangedMessages() { AttenuationPosition = value });
        }
    }
}
