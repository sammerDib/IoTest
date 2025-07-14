using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DistanceSensorService : DuplexServiceBase<IDistanceSensorServiceCallback>, IDistanceSensorService
    {
        private HardwareManager _hardwareManager;
        private const string DeviceName = "Distance sensor";

        public DistanceSensorService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;

            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<StateMessage>(this, (r, m) => { OnStatusChanged(m.State); });
            messenger.Register<DistanceMessage>(this, (r, m) => { OnDistanceChanged(m.Distance); });
            messenger.Register<IdMessage>(this, (r, m) => { OnIdChanged(m.Id); });
            messenger.Register<CustomMessage>(this, (r, m) => { OnCustomChanged(m.Custom); });
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

        public void OnDistanceChanged(double distance)
        {
            InvokeCallback(i => i.DistanceChangedCallback(distance));
        }

        public void OnIdChanged(string value)
        {
            InvokeCallback(i => i.IdChangedCallback(value));
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
