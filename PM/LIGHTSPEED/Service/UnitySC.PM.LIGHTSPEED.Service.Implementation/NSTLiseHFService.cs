using System.ServiceModel;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.AttenuationFilter;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Laser;
using UnitySC.PM.Shared.Hardware.PolarisationFilter;
using UnitySC.PM.Shared.Hardware.Shutter;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NSTLiseHFService : DuplexServiceBase<ILiseHFServiceCallback>, ILiseHFService
    {
        private HardwareManager _hardwareManager;

        public string PowermeterId { get; set; }

        public NSTLiseHFService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<Shared.Hardware.Laser.Leukos.LaserStatusMessage>(this, (m) => { UpdateLaserPowerStatus(m.LaserStatus); });
            messenger.Register<Shared.Hardware.Laser.Leukos.InterlockStatusMessage>(this, (m) => { UpdateInterlockStatus(m.InterlockStatus); });
            messenger.Register<Shared.Hardware.Laser.Leukos.LaserTemperatureMessage>(this, (m) => { UpdateLaserTemperature(m.LaserTemperature); });
            messenger.Register<Shared.Hardware.Laser.Leukos.CrystalTemperatureMessage>(this, (m) => { UpdateCrystalTemperature(m.CrystalTemperature); });
            messenger.Register<Shared.Hardware.AttenuationFilter.AttenuationPositionMessage>(this, (m) => { UpdateAttenuationPosition(m.Position); });
            messenger.Register<Shared.Hardware.FastAttenuation.FastAttenuationPositionMessage>(this, (m) => { UpdateFastAttenuationPosition(m.Position); });
            messenger.Register<ShutterIrisPositionMessage>(this, (m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });
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

        public Response<VoidResult> PowerOn()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("PowerOn");
                    _hardwareManager.Laser.PowerOn();
                    messageContainer.Add(new Message(MessageLevel.Information, "PowerOn"));
                }
            });
        }

        public Response<VoidResult> PowerOff()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("PowerOff");
                    _hardwareManager.Laser.PowerOff();
                    messageContainer.Add(new Message(MessageLevel.Information, "PowerOff"));
                }
            });
        }

        public Response<VoidResult> SetPower(int power)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("SetPower");
                    _hardwareManager.Laser.SetPower(power);
                    messageContainer.Add(new Message(MessageLevel.Information, "SetPower"));
                }
            });
        }

        public Response<VoidResult> SetCurrent(int current)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("SetCurrent");
                    _hardwareManager.Laser.SetCurrent(current);
                    messageContainer.Add(new Message(MessageLevel.Information, "SetCurrent"));
                }
            });
        }

        public Response<VoidResult> AttenuationMoveAbsPosition(ServoPosition position)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                double pos = (int)position;
                _logger.Debug("Attenuation - MoveAbsPosition " + "[" + pos.ToString() + "]");                
                _hardwareManager.AttenuationFilter.MoveAbsPosition(pos);

                messageContainer.Add(new Message(MessageLevel.Information, "Attenuation - MoveAbsPosition " + "[" + pos.ToString() + "]"));
            });
        }

        public Response<VoidResult> AttenuationHomePosition()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("Attenuation - HomePosition");
                _hardwareManager.AttenuationFilter.HomePosition();
                messageContainer.Add(new Message(MessageLevel.Information, "Attenuation - HomePosition"));
            });
        }

        public Response<VoidResult> FastAttenuationMoveAbsPosition(double position)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("FastAttenuation - MoveAbsPosition " + "[" + position.ToString() + "]");
                _hardwareManager.FastAttenuation.MoveAbsPosition(position);

                messageContainer.Add(new Message(MessageLevel.Information, "FastAttenuation - MoveAbsPosition " + "[" + position.ToString() + "]"));
            });
        }
        public Response<VoidResult> OpenShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Shutter != null)
                {
                    _logger.Information("OpenShutterCommand");
                    _hardwareManager.Shutter.OpenShutter();
                    messageContainer.Add(new Message(MessageLevel.Information, "OpenShutterCommand"));
                }
            });
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Shutter != null)
                {
                    _logger.Information("CloseShutterCommand");
                    _hardwareManager.Shutter.CloseShutter();
                    messageContainer.Add(new Message(MessageLevel.Information, "CloseShutterCommand"));
                }
            });
        }

        public Response<VoidResult> InitializeUpdate()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("InitializeUpdate");
                if (_hardwareManager.Laser != null)
                    _hardwareManager.Laser.InitializeUpdate();
                if (_hardwareManager.AttenuationFilter != null)
                    _hardwareManager.AttenuationFilter.InitializeUpdate();
                if (_hardwareManager.Shutter != null)
                    _hardwareManager.Shutter.InitializeUpdate();
                if (_hardwareManager.PolarisationFilter != null)
                    _hardwareManager.PolarisationFilter.InitializeUpdate();
                if (_hardwareManager.OpticalPowermeters.Count != 0)
                { 
                    foreach (var powermeter in _hardwareManager.OpticalPowermeters)
                    {
                        _hardwareManager.OpticalPowermeters[powermeter.Key].InitializeUpdate();
                    }
                    messageContainer.Add(new Message(MessageLevel.Information, "InitializeUpdate"));
                }
            });
        }

        public Response<VoidResult> AttenuationRefresh()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.AttenuationFilter != null)
                    _hardwareManager.AttenuationFilter.InitializeUpdate();
            });
        }

        public void UpdateLaserPowerStatus(bool value)
        {
            InvokeCallback(i => i.LaserPowerStatusChangedCallback(value));
        }

        public void UpdateInterlockStatus(string value)
        {
            InvokeCallback(i => i.InterlockStatusChangedCallback(value));
        }

        private void UpdateLaserTemperature(double value)
        {
            InvokeCallback(i => i.LaserTemperatureChangedCallback(value));
        }

        private void UpdateCrystalTemperature(double value)
        {
            InvokeCallback(i => i.CrystalTemperatureChangedCallback(value));
        }
        
        public void UpdateAttenuationPosition(double position)
        {
            InvokeCallback(i => i.AttenuationPositionChangedCallback(position));
        }

        public void UpdateFastAttenuationPosition(double position)
        {
            InvokeCallback(i => i.FastAttenuationPositionChangedCallback(position));
        }

        public void UpdateShutterIrisPosition(string value)
        {
            InvokeCallback(i => i.ShutterIrisPositionChangedCallback(value));
        }
    }
}
