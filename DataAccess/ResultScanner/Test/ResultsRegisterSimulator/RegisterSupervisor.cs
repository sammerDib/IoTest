using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace ResultsRegisterSimulator
{
    public class RegisterSupervisor
    {
        private InstanceContext _instanceContext;
        private ServiceInvoker<IRegisterResultService> _registerService;
        public ServiceInvoker<IRegisterResultService> Service => _registerService;
        private ILogger _logger;
        private IMessenger _messenger;

        public RegisterSupervisor(ILogger<IResultService> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            try
            {
                _instanceContext = new InstanceContext(this);
                _registerService = new ServiceInvoker<IRegisterResultService>("RegisterResultService",
                    ClassLocator.Default.GetInstance<ILogger<IRegisterResultService>>(),
                    _messenger);
            }
            catch (Exception) { }
        }
    }
}
