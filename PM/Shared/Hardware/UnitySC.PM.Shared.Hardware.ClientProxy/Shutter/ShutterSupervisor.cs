using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.ClientProxy.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Shutter
{
    [CallbackBehaviorAttribute(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ShutterSupervisor : IShutterServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<IShutterService> _shutterService;
        private IDialogOwnerService _dialogService;

        private ShutterVM _shutterVM;

        /// <summary>
        /// Constructor
        /// </summary>
        public ShutterSupervisor(ILogger<ShutterSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            _instanceContext = new InstanceContext(this);
            _shutterService = new DuplexServiceInvoker<IShutterService>(_instanceContext, "HARDWAREShutterService", ClassLocator.Default.GetInstance<SerilogLogger<IShutterService>>(), messenger, s => s.SubscribeToChanges());
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }

        public ShutterVM ShutterVM
        {
            get
            {
                if (_shutterVM == null)
                {
                    _shutterVM = new ShutterVM(this);
                }
                return _shutterVM;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _shutterService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _shutterService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _shutterService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<VoidResult> OpenShutterCommand()
        {
            return _shutterService.TryInvokeAndGetMessages(s => s.OpenShutterCommand());
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return _shutterService.TryInvokeAndGetMessages(s => s.CloseShutterCommand());
        }

        void IShutterServiceCallback.StateChangedCallback(string state)
        {
            throw new NotImplementedException();
        }

        void IShutterServiceCallback.ShutterIrisPositionChangedCallback(string shutterIrisPosition)
        {
            _messenger.Send(new ShutterIrisPositionChangedMessages() { ShutterIrisPosition = shutterIrisPosition });
        }
    }
}
