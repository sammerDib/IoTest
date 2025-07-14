using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Result.CommonUI.Proxy
{
    public class ResultSupervisor : IResultServiceCallback
    {
        private readonly InstanceContext _instanceContext;
        private readonly ILogger _logger;
        private readonly DuplexServiceInvoker<IResultService> _resultService;
        private readonly IMessenger _messenger;

        public DuplexServiceInvoker<IResultService> Service => _resultService;

        public ResultSupervisor(ILogger<IResultService> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            try
            {
                _instanceContext = new InstanceContext(this);
                _resultService = new DuplexServiceInvoker<IResultService>(_instanceContext, "ResultService",
                    ClassLocator.Default.GetInstance<ILogger<IResultService>>(),
                    _messenger);
                SubscribeToChanges();
            }
            catch (Exception) { }
        }

        public void OnResultChanged(ResultNotificationMessage msg)
        {
            _messenger.Send(msg);
        }

        public void OnResultStatsChanged(ResultStatsNotificationMessage msg)
        {
            _messenger.Send(msg);
        }

        public void OnResultStateChanged(ResultStateNotificationMessage msg)
        {
            _messenger.Send(msg);
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _resultService.InvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _resultService.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }
    }
}