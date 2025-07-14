using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace WpfUnityControlRegisterValidation
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
