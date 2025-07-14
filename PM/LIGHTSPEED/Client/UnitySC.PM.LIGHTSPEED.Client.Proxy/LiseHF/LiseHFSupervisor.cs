using System.ServiceModel;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Client.Proxy.LiseHF
{
    public class LiseHFSupervisor : ILiseHFService, ILiseHFServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private DuplexServiceInvoker<ILiseHFService> _liseHFService;
        private IMessenger _messenger;

        public LiseHFSupervisor(ILogger<LiseHFSupervisor> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _instanceContext = new InstanceContext(this);
            _liseHFService = new DuplexServiceInvoker<ILiseHFService>(_instanceContext, "LiseHFService", ClassLocator.Default.GetInstance<ILogger<ILiseHFService>>(), _messenger);

            SubscribeToChanges();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> PowerOn()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.PowerOn());
        }

        public Response<VoidResult> PowerOff()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.PowerOff());
        }

        public Response<VoidResult> SetPower(int power)
        {
            return _liseHFService.InvokeAndGetMessages(s => s.SetPower(power));
        }

        public Response<VoidResult> AttenuationMoveAbsPosition(ServoPosition position)
        {
            return _liseHFService.InvokeAndGetMessages(s => s.AttenuationMoveAbsPosition(position));
        }

        public Response<VoidResult> AttenuationHomePosition()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.AttenuationHomePosition());
        }

        public Response<VoidResult> FastAttenuationMoveAbsPosition(double position)
        {
            return _liseHFService.InvokeAndGetMessages(s => s.FastAttenuationMoveAbsPosition(position));
        }

        public Response<VoidResult> OpenShutterCommand()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.OpenShutterCommand());
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.CloseShutterCommand());
        }

        public Response<VoidResult> InitializeUpdate()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.InitializeUpdate());
        }

        public Response<VoidResult> AttenuationRefresh()
        {
            return _liseHFService.InvokeAndGetMessages(s => s.AttenuationRefresh());
        }

        void ILiseHFServiceCallback.LaserPowerStatusChangedCallback(bool laserPowerOn)
        {
            _messenger.Send(new LaserPowerStatusChangedMessage() { LaserPowerOn = laserPowerOn });
        }

        void ILiseHFServiceCallback.InterlockStatusChangedCallback(string value)
        {
            _messenger.Send(new InterlockStatusChangedMessage() { InterlockStatus = value });
        }

        void ILiseHFServiceCallback.LaserTemperatureChangedCallback(double value)
        {
            _messenger.Send(new LaserTemperatureChangedMessage() { LaserTemperature = value });
        }

        void ILiseHFServiceCallback.CrystalTemperatureChangedCallback(double value)
        {
            _messenger.Send(new CrystalTemperatureChangedMessage() { CrystalTemperature = value });
        }

        void ILiseHFServiceCallback.AttenuationPositionChangedCallback(double value)
        {
            _messenger.Send(new AttenuationPositionChangedMessage() { AttenuationPosition = value });

        }
        void ILiseHFServiceCallback.FastAttenuationPositionChangedCallback(double value)
        {
            _messenger.Send(new FastAttenuationPositionChangedMessage() { FastAttenuationPosition = value });
        }


        void ILiseHFServiceCallback.ShutterIrisPositionChangedCallback(string value)
        {
            _messenger.Send(new ShutterIrisPositionChangedMessages() { ShutterIrisPosition = value });
        }
    }
}
