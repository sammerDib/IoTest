using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Hardware.Shutter;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ShutterService : DuplexServiceBase<IShutterServiceCallback>, IShutterService
    {
        private readonly HardwareManager _hardwareManager;

        public ShutterService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<StateMessage>(this, (r, m) => { UpdateState(m.State); });
            messenger.Register<ShutterIrisPositionMessage>(this, (r, m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });
        }

        public override void Init()
        {
            base.Init();
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
                _hardwareManager.Shutters.Values.FirstOrDefault()?.TriggerUpdateEvent();
            });
        }

        public Response<VoidResult> OpenShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.Shutters.Values.FirstOrDefault()?.OpenIris();
            });
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.Shutters.Values.FirstOrDefault()?.CloseIris();
            });
        }

        public void UpdateState(string state)
        {
            InvokeCallback(i => i.StateChangedCallback(state));
        }

        public void UpdateShutterIrisPosition(string shutterIrisPosition)
        {
            InvokeCallback(i => i.ShutterIrisPositionChangedCallback(shutterIrisPosition));
        }
    }
}
