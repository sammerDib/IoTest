using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PlcService : DuplexServiceBase<IPlcServiceCallback>, IPlcService
    {
        private readonly HardwareManager _hardwareManager;

        public PlcService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();                       

            messenger.Register<StateMessage>(this, (r, m) => { UpdateState(m.State); });
            messenger.Register<IdMessage>(this, (r, m) => { UpdateId(m.Id); });
            messenger.Register<CustomMessage>(this, (r, m) => { UpdateCustom(m.Custom); });
            messenger.Register<AmsNetIdMessage>(this, (r, m) => { UpdateAmsNetId(m.AmsNetId); });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
            });
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Plc != null)
                    _hardwareManager.Plc.TriggerUpdateEvent();
            });
        }

        public Response<VoidResult> SmokeDetectorResetAlarm()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Plc != null)
                    _hardwareManager.Plc.SmokeDetectorResetAlarm();
            });
        }

        public Response<VoidResult> Restart()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Plc != null)
                    _hardwareManager.Plc.Restart();
            });
        }

        public Response<VoidResult> Reboot()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Plc != null)
                    _hardwareManager.Plc.Reboot();
            });
        }

        public Response<VoidResult> CustomCommand(string custom)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.Plc.CustomCommand(custom);
            });
        }

        public void UpdateState(string state)
        {
            InvokeCallback(i => i.StateChangedCallback(state));
        }

        public void UpdateId(string value)
        {
            InvokeCallback(i => i.IdChangedCallback(value));
        }

        public void UpdateCustom(string value)
        {
            InvokeCallback(i => i.CustomChangedCallback(value));
        }

        public void UpdateAmsNetId(string value)
        {
            InvokeCallback(i => i.AmsNetIdChangedCallback(value));
        }        
    }
}
