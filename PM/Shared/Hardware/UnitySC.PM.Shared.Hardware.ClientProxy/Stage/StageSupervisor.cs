using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Stage;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Stage
{
    public class StageSupervisor : IStageServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private GalaSoft.MvvmLight.Messaging.IMessenger _messenger;
        private DuplexServiceInvoker<IStageService> _stageService;
        private IDialogOwnerService _dialogService;

        /// <summary>
        /// Constructor
        /// </summary>
        public StageSupervisor(ILogger<StageSupervisor> logger, GalaSoft.MvvmLight.Messaging.IMessenger messenger, IDialogOwnerService dialogService)
        {
            _instanceContext = new InstanceContext(this);
            _stageService = new DuplexServiceInvoker<IStageService>(_instanceContext, "StageService", ClassLocator.Default.GetInstance<SerilogLogger<IStageService>>(), messenger);
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }

        //public Response<VoidResult> ConnectStage()
        //{
        //    return _stageService.InvokeAndGetMessages(s => s.ConnectStage());
        //}
        public Response<VoidResult> SendCommandToStage(string commandToApply)
        {
            return _stageService.InvokeAndGetMessages(s => s.SendCommandToStage(commandToApply));
        }

        public Response<List<string>> SendControllerResponse()
        {
            return _stageService.InvokeAndGetMessages(s => s.SendControllerResponse());
        }

        public Response<int> GetStagePosition()
        {
            return _stageService.InvokeAndGetMessages(s => s.GetStagePosition());
        }

        public Response<int> GetChuckPosition()
        {
            return _stageService.InvokeAndGetMessages(s => s.GetChuckPosition());
        }

        public void PositionChangedCallback(PositionBase position)
        {
            throw new NotImplementedException();
        }

        public void StateChangedCallback(StageState state)
        {
            throw new NotImplementedException();
        }

        public void EndMoveCallback(bool target)
        {
            throw new NotImplementedException();
        }
    }
}