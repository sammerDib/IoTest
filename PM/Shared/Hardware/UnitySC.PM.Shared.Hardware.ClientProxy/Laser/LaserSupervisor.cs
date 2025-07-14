using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Laser
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class LaserSupervisor : ILaserService, ILaserServiceCallback
    {
        private InstanceContext _instanceContext;
        private IMessenger _messenger;
        private DuplexServiceInvoker<ILaserService> _laserService;

        private LaserVM _laserVM;

        /// <summary>
        /// Constructor
        /// </summary>
        public LaserSupervisor(ILogger<LaserSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            _laserService = new DuplexServiceInvoker<ILaserService>(_instanceContext, "HARDWARELaserService", ClassLocator.Default.GetInstance<SerilogLogger<ILaserService>>(), messenger, s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.HardwareControl));
            _messenger = messenger;
        }

        public LaserVM LaserVM
        {
            get
            {
                if (_laserVM == null)
                {
                    _laserVM = new LaserVM(this);
                }
                return _laserVM;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _laserService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _laserService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> PowerOn()
        {
            return _laserService.TryInvokeAndGetMessages(s => s.PowerOn());
        }

        public Response<VoidResult> PowerOff()
        {
            return _laserService.TryInvokeAndGetMessages(s => s.PowerOff());
        }

        public Response<VoidResult> SetPower(int power)
        {
            return _laserService.TryInvokeAndGetMessages(s => s.SetPower(power));
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _laserService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<VoidResult> CustomCommand(string customCmd)
        {
            return _laserService.TryInvokeAndGetMessages(s => s.CustomCommand(customCmd));
        }

        void ILaserServiceCallback.StateChangedCallback(DeviceState state)
        {
            _messenger.Send(new StateChangedMessage() { State = state });
        }

        void ILaserServiceCallback.PowerChangedCallback(double power)
        {
            _messenger.Send(new PowerChangedMessage() { Power = power });
        }

        void ILaserServiceCallback.InterlockStatusChangedCallback(string value)
        {
            _messenger.Send(new InterlockStatusChangedMessage() { InterlockStatus = value });
        }

        void ILaserServiceCallback.LaserTemperatureChangedCallback(double value)
        {
            _messenger.Send(new LaserTemperatureChangedMessage() { LaserTemperature = value });
        }

        void ILaserServiceCallback.CustomChangedCallback(string value)
        {
            _messenger.Send(new CustomChangedMessage() { Custom = value });
        }

        void ILaserServiceCallback.LaserPowerStatusChangedCallback(bool laserPowerStatus)
        {
            _messenger.Send(new LaserPowerStatusChangedMessage() { LaserPowerStatus = laserPowerStatus });
        }

        public void CrystalTemperatureChangedCallback(double value)
        {
            _messenger.Send(new CrystalTemperatureChangedMessage() { CrystalTemperature = value });
        }
    }
}
