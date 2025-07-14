using System.ServiceModel;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.AttenuationFilter;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Shutter;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LSFeedbackLoopService : DuplexServiceBase<IFeedbackLoopServiceCallback>, IFeedbackLoopService
    {
        private HardwareManager _hardwareManager;

        public string PowermeterId { get; set; }

        public LSFeedbackLoopService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<Shared.Hardware.OpticalPowermeter.PowerMessage>(this, (m) => { UpdatePower(m.Flow, m.Power, m.PowerCal_mW, m.RFactor); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.WavelengthMessage>(this, (m) => { UpdateWavelength(m.Flow, m.Wavelength); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.BeamDiameterMessage>(this, (m) => { UpdateBeamDiameter(m.Flow, m.BeamDiameter); });
            messenger.Register<Shared.Hardware.Laser.LaserQuantum.PowerMessage>(this, (m) => { UpdatePowerLaser(m.Power); });
            messenger.Register<Shared.Hardware.Laser.LaserQuantum.InterlockStatusMessage>(this, (m) => { UpdateInterlockStatus(m.InterlockStatus); });
            messenger.Register<Shared.Hardware.Laser.LaserQuantum.LaserTemperatureMessage>(this, (m) => { UpdateLaserTemperature(m.LaserTemperature); });
            messenger.Register<Shared.Hardware.Laser.LaserQuantum.PsuTemperatureMessage>(this, (m) => { UpdatePsuTemperature(m.PsuTemperature); });
            messenger.Register<ShutterIrisPositionMessage>(this, (m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });
            messenger.Register<AttenuationPositionMessage>(this, (m) => { UpdateAttenuationPosition(m.Position); });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to RotatorsKitCalibration change"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to RotatorsKitCalibration change"));
            });
        }

        public Response<VoidResult> InitializeUpdate()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("FeedbackLoop initialize update");
                if (_hardwareManager.Laser != null)
                {
                    _hardwareManager.Laser.InitializeUpdate();
                }

                foreach (var powermeter in _hardwareManager.OpticalPowermeters)
                {
                    _hardwareManager.OpticalPowermeters[powermeter.Key].InitializeUpdate();
                }

                if (_hardwareManager.AttenuationFilter != null)
                {
                    _hardwareManager.AttenuationFilter.InitializeUpdate();
                }

                if (_hardwareManager.Shutter != null)
                {
                    _hardwareManager.Shutter.InitializeUpdate();
                }
                messageContainer.Add(new Message(MessageLevel.Information, "FeedbackLoop initialize update"));
            });
        }

        public Response<VoidResult> PowerOn()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("PowerOn");
                _hardwareManager.Laser.PowerOn();
                messageContainer.Add(new Message(MessageLevel.Information, "PowerOn"));
            });
        }

        public Response<VoidResult> PowerOff()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("PowerOff");
                _hardwareManager.Laser.PowerOff();
                messageContainer.Add(new Message(MessageLevel.Information, "PowerOff"));
            });
        }

        public Response<VoidResult> SetPower(int power)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("SetPower");
                _hardwareManager.Laser.SetPower(power);
                messageContainer.Add(new Message(MessageLevel.Information, "SetPower"));
            });
        }

        public Response<VoidResult> SetCurrent(int current)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("SetCurrent");
                _hardwareManager.Laser.SetCurrent(current);
                messageContainer.Add(new Message(MessageLevel.Information, "SetCurrent"));
            });
        }

        public Response<VoidResult> OpenShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("OpenShutterCommand");
                _hardwareManager.Shutter.OpenShutter();
                messageContainer.Add(new Message(MessageLevel.Information, "OpenShutterCommand"));
            });
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("CloseShutterCommand");
                _hardwareManager.Shutter.CloseShutter();
                messageContainer.Add(new Message(MessageLevel.Information, "CloseShutterCommand"));
            });
        }

        public Response<VoidResult> AttenuationHomePosition()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("AttenuationHomePosition");
                _hardwareManager.AttenuationFilter.HomePosition();
                messageContainer.Add(new Message(MessageLevel.Information, "AttenuationHomePosition"));
            });
        }

        public Response<VoidResult> MoveAbsPosition(double position)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("MoveAbsPosition " + "[" + position.ToString() + "]");
                _hardwareManager.AttenuationFilter.MoveAbsPosition(position);
                messageContainer.Add(new Message(MessageLevel.Information, "MoveAbsPosition " + "[" + position.ToString() + "]"));
            });
        }

        public void UpdatePower(PowerIlluminationFlow flow, double power, double powerCal_mW, double rfactor)
        {
            InvokeCallback(i => i.PowerChangedCallback(flow, power, powerCal_mW, rfactor));
        }

        public void UpdateWavelength(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.WavelengthChangedCallback(flow, value));
        }

        public void UpdateBeamDiameter(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.BeamDiameterChangedCallback(flow, value));
        }

        public void UpdatePowerLaser(double value)
        {
            InvokeCallback(i => i.PowerLaserChangedCallback(value));
        }

        public void UpdateInterlockStatus(string value)
        {
            InvokeCallback(i => i.InterlockStatusChangedCallback(value));
        }

        private void UpdateLaserTemperature(double value)
        {
            InvokeCallback(i => i.LaserTemperatureChangedCallback(value));
        }

        private void UpdatePsuTemperature(double value)
        {
            InvokeCallback(i => i.PsuTemperatureChangedCallback(value));
        }

        public void UpdateShutterIrisPosition(string value)
        {
            InvokeCallback(i => i.ShutterIrisPositionChangedCallback(value));
        }

        public void UpdateAttenuationPosition(double value)
        {
            InvokeCallback(i => i.AttenuationPositionChangedCallback(value));
        }
    }
}
