using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface;

using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.UI.Hardware.Controller;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.UI.Proxy
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ControllersSupervisor : IControllerService, IControllerServiceCallback
    {
        private InstanceContext _instanceContext;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private ActorType _actorType;
        private DuplexServiceInvoker<IControllerService> _controllerService;

        private readonly IDialogOwnerService _dialogService;
        private ControllersVM _controllersVM;

        /// <summary>
        /// Constructor
        /// </summary>
        public ControllersSupervisor(ActorType actorType, ILogger<ControllersSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            _instanceContext = new InstanceContext(this);
            _logger = logger;
            _messenger = messenger;
            _actorType = actorType;
            _dialogService = dialogService;
            _controllersVM = new ControllersVM(this, logger);
        }

        public void Init()
        {
            var endPoint = "ControllerService";
            endPoint = _actorType + endPoint;
            _controllerService = new DuplexServiceInvoker<IControllerService>(_instanceContext,
                                endPoint, ClassLocator.Default.GetInstance<SerilogLogger<IControllerService>>(),
                               _messenger, s => s.SubscribeToChanges(), 
                               ClientConfiguration.GetServiceAddress(_actorType));

        }
        public ControllersVM ControllersVM
        {
            get
            {
                return _controllersVM;
            }
        }

        public Response<ControllerConfig> GetControllerById(string deviceId)
        {
            return _controllerService.TryInvokeAndGetMessages(s => s.GetControllerById(deviceId));
        }

        public Response<List<string>> GetControllersIds()
        {
            return _controllerService.TryInvokeAndGetMessages(s => s.GetControllersIds());
        }

        public Response<bool> GetDigitalIoState(string deviceId, string ioId)
        {
            return _controllerService.TryInvokeAndGetMessages(s => s.GetDigitalIoState(deviceId, ioId));
        }

        public Response<double> GetAnalogIoValue(string deviceId, string ioId)
        {
            return _controllerService.TryInvokeAndGetMessages(s => s.GetAnalogIoValue(deviceId, ioId));
        }

        public Response<List<IOControllerConfig>> GetControllersIOs()
        {
            return _controllerService.TryInvokeAndGetMessages(s => s.GetControllersIOs());
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _controllerService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Controller subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _controllerService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Controller unsubscribe error");
            }
            return resp;
        }

        public Response<VoidResult> SetDigitalIoState(string deviceId, string ioId, bool value)
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _controllerService.TryInvokeAndGetMessages(s => s.SetDigitalIoState(deviceId, ioId, value));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Controller SetDigitalIoState error");
            }
            return resp;
        }

        public Response<VoidResult> SetAnalogIoValue(string deviceId, string ioId, double value)
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _controllerService.TryInvokeAndGetMessages(s => s.SetAnalogIoValue(deviceId, ioId, value));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Controller SetAnalogIoValue error");
            }
            return resp;
        }

        public void UpdateStatusOfIosCallback(List<DataAttribute> dataAttributes)
        {
            if (_controllersVM == null)
                return;
            _controllersVM.UpdateListsOfIODisplayed(dataAttributes);
        }

        public Response<VoidResult> StartIoRefreshTask()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _controllerService.TryInvokeAndGetMessages(s => s.StartIoRefreshTask());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Controller StartIOTask error");
            }
            return resp;
        }

        public Response<VoidResult> StopIoRefreshTask()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _controllerService.TryInvokeAndGetMessages(s => s.StopIoRefreshTask());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Controller StopIOTask error");
            }
            return resp;
        }
    }
}
