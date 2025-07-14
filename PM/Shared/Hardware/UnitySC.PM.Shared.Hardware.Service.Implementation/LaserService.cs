using System.ServiceModel;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using System.Collections.Generic;
using UnitySC.PM.Shared.ReformulationMessage;
using System;
using System.Linq;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LaserService : DuplexServiceBase<ILaserServiceCallback>, ILaserService
    {
        private readonly HardwareManager _hardwareManager;
        private const string DeviceName = "Laser";

        public LaserService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<Interface.Laser.LaserQuantum.StateMessage>(this, (r, m) => { OnStatusChanged(m.State); });
            messenger.Register<Interface.Laser.LaserQuantum.PowerMessage>(this, (r, m) => { OnPowerChanged(m.Power); });
            messenger.Register<Interface.Laser.LaserQuantum.InterlockStatusMessage>(this, (r, m) => { OnInterlockStatusChanged(m.InterlockStatus); });
            messenger.Register<Interface.Laser.LaserQuantum.LaserTemperatureMessage>(this, (r, m) => { OnLaserTemperatureChanged(m.LaserTemperature); });
            messenger.Register<Interface.Laser.LaserQuantum.CustomMessage>(this, (r, m) => { OnCustomChanged(m.Custom); });

            messenger.Register<Interface.Laser.Leukos.StateMessage>(this, (r, m) => { OnStatusChanged(m.State); });
            messenger.Register<Interface.Laser.Leukos.PowerMessage>(this, (r, m) => { OnPowerChanged(m.Power); });
            messenger.Register<Interface.Laser.Leukos.InterlockStatusMessage>(this, (r, m) => { OnInterlockStatusChanged(m.InterlockStatus); });
            messenger.Register<Interface.Laser.Leukos.LaserTemperatureMessage>(this, (r, m) => { OnLaserTemperatureChanged(m.LaserTemperature); });
            messenger.Register<Interface.Laser.Leukos.CustomMessage>(this, (r, m) => { OnCustomChanged(m.Custom); });

            messenger.Register<Interface.Laser.Leukos.LaserStatusMessage>(this, (r, m) => { OnLaserPowerStatusChanged(m.LaserStatus); });
            messenger.Register<Interface.Laser.Leukos.CrystalTemperatureMessage>(this, (r, m) => { OnCrystalTemperatureChanged(m.CrystalTemperature); });
        }

        public override void Init()
        {
            base.Init();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Subscribe();
            });
        }

        public Response<VoidResult> PowerOn()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        //At the moment we use the default laser.
                        //Further development to come. Refacto on the customer side
                        _hardwareManager?.Lasers.Values.FirstOrDefault().PowerOn();
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<VoidResult> PowerOff()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        _hardwareManager?.Lasers.Values.FirstOrDefault().PowerOff();
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<VoidResult> SetPower(int power)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        _hardwareManager?.Lasers.Values.FirstOrDefault().SetPower(power);
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        if (_hardwareManager.Lasers.Values.FirstOrDefault() != null)
                            _hardwareManager?.Lasers.Values.FirstOrDefault().TriggerUpdateEvent();
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<VoidResult> CustomCommand(string custom)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        _hardwareManager?.Lasers.Values.FirstOrDefault().CustomCommand(custom);
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        private void OnStatusChanged(DeviceState state)
        {
            InvokeCallback(i => i.StateChangedCallback(state));
        }

        public void OnPowerChanged(double value)
        {
            InvokeCallback(i => i.PowerChangedCallback(value));
        }

        public void OnInterlockStatusChanged(string value)
        {
            InvokeCallback(i => i.InterlockStatusChangedCallback(value));
        }

        private void OnLaserTemperatureChanged(double value)
        {
            InvokeCallback(i => i.LaserTemperatureChangedCallback(value));
        }

        public void OnCustomChanged(string value)
        {
            InvokeCallback(i => i.CustomChangedCallback(value));
        }

        private void OnCrystalTemperatureChanged(double value)
        {
            InvokeCallback(i => i.CrystalTemperatureChangedCallback(value));
        }

        public void OnLaserPowerStatusChanged(bool value)
        {
            InvokeCallback(i => i.LaserPowerStatusChangedCallback(value));
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }
    }
}
