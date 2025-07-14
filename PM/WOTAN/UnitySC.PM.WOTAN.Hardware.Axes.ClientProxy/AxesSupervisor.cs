using System;
using System.Collections.Generic;
using System.ServiceModel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes
{
    public class AxesSupervisor : IAxesServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private GalaSoft.MvvmLight.Messaging.IMessenger _messenger;
        private DuplexServiceInvoker<IAxesService> _axesService;
        private IDialogOwnerService _dialogService;

        /// <summary>
        /// Constructor
        /// </summary>
        public AxesSupervisor(ILogger<AxesSupervisor> logger, GalaSoft.MvvmLight.Messaging.IMessenger messenger, IDialogOwnerService dialogService)
        {
            _instanceContext = new InstanceContext(this);
            _axesService = new DuplexServiceInvoker<IAxesService>(_instanceContext, "AxesService", ClassLocator.Default.GetInstance<SerilogLogger<IAxesService>>(), messenger);
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }

        public Response<VoidResult> SendCommandToAxes(string commandToApply)
        {
            return _axesService.InvokeAndGetMessages(s => s.SendCommandToAxes(commandToApply));
        }

        public Response<List<string>> SendControllerResponse()
        {
            return _axesService.InvokeAndGetMessages(s => s.SendControllerResponse());
        }

        public void PositionChangedCallback(PositionBase position)
        {
        }

        public void StateChangedCallback(AxesState state)
        {
        }

        public void EndMoveCallback(bool target)
        {
        }
    }
}
