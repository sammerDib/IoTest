using System.ServiceModel;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using System.Collections.Generic;
using UnitySC.PM.Shared.ReformulationMessage;
using System;
using System.Linq;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FfuService : DuplexServiceBase<IFfuServiceCallback>, IFfuService
    {
        private readonly HardwareManager _hardwareManager;
        private const string DeviceName = "Ffu";
        private object _lock = new object(); 

        public FfuService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<Interface.Ffu.StateMessage>(this, (r, m) => { OnStateChanged(m.State); });
            messenger.Register<Interface.Ffu.StatusMessage>(this, (r, m) => { OnStatusChanged(m.Status); });
            messenger.Register<Interface.Ffu.CurrentSpeedMessage>(this, (r, m) => { OnCurrentSpeedChanged(m.CurrentSpeed_percentage); });
            messenger.Register<Interface.Ffu.TemperatureMessage>(this, (r, m) => { OnTemperatureChanged(m.Temperature); });
            messenger.Register<Interface.Ffu.WarningMessage>(this, (r, m) => { OnWarningChanged(m.Triggered); });
            messenger.Register<Interface.Ffu.AlarmMessage>(this, (r, m) => { OnAlarmChanged(m.Triggered); });
            messenger.Register<Interface.Ffu.CustomMessage>(this, (r, m) => { OnCustomChanged(m.Custom); });
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
                lock (_lock)
                {
                    try
                    {
                        foreach (var ffu in _hardwareManager?.Ffus.Values)
                        {
                            ffu?.PowerOn();
                        }
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
                lock (_lock)
                {
                    try
                    {
                        foreach (var ffu in _hardwareManager?.Ffus.Values)
                        {
                            ffu?.PowerOff();
                        }
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<VoidResult> SetSpeed(ushort speedPercent)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    try
                    {
                        foreach (var ffu in _hardwareManager?.Ffus.Values)
                        {
                            ffu?.SetSpeed(speedPercent);
                        }
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
                lock (_lock)
                {
                    try
                    {
                        foreach (var ffu in _hardwareManager?.Ffus.Values)
                        {
                            ffu?.TriggerUpdateEvent();
                        }
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
                lock (_lock)
                {
                    try
                    {
                        foreach (var ffu in _hardwareManager?.Ffus.Values)
                        {
                            ffu?.CustomCommand(custom);
                        }
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<Dictionary<string, ushort>> GetDefaultFfuValues()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                Dictionary<string, ushort> defaultValues = null;
                if (_hardwareManager?.Ffus.Count > 0)
                    defaultValues = _hardwareManager?.Ffus.Values.FirstOrDefault()?.GetDefaultFfuValues();
         
                return defaultValues ?? new Dictionary<string, ushort>();
            });
        }

        private void OnStateChanged(DeviceState state)
        {
            InvokeCallback(i => i.StateChangedCallback(state));
        }

        private void OnStatusChanged(string status)
        {
            InvokeCallback(i => i.StatusChangedCallback(status));
        }

        public void OnCurrentSpeedChanged(ushort value)
        {
            InvokeCallback(i => i.CurrentSpeedChangedCallback(value));
        }

        public void OnTemperatureChanged(double value)
        {
            InvokeCallback(i => i.TemperatureChangedCallback(value));
        }

        private void OnWarningChanged(bool value)
        {
            InvokeCallback(i => i.WarningChangedCallback(value));
        }

        private void OnAlarmChanged(bool value)
        {
            InvokeCallback(i => i.AlarmChangedCallback(value));
        }

        public void OnCustomChanged(string value)
        {
            InvokeCallback(i => i.CustomChangedCallback(value));
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }        
    }
}
