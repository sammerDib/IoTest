using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Global
{
    public class GlobalDeviceSupervisor : IGlobalDeviceService, IGlobalCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private DuplexServiceInvoker<IGlobalDeviceService> _globalDeviceService;
        private IMessenger _messenger;

        public GlobalDeviceSupervisor(ILogger<GlobalDeviceSupervisor> logger, IMessenger messenger, ActorType? actorType)
        {
            _logger = logger;
            _messenger = messenger;
            _instanceContext = new InstanceContext(this);
            var endPoint = "GlobalDeviceService";
            if (actorType != null)
                endPoint = actorType + endPoint;
            _globalDeviceService = new DuplexServiceInvoker<IGlobalDeviceService>(_instanceContext, endPoint, ClassLocator.Default.GetInstance<ILogger<IGlobalDeviceService>>(), _messenger, s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));
            GlobalDeviceVM.Update(GetDevices()?.Result);
        }

        public Response<List<GlobalDevice>> GetDevices()
        {
            return _globalDeviceService.TryInvokeAndGetMessages(s => s.GetDevices());
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _globalDeviceService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _globalDeviceService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        private GlobalDeviceVM _globalDeviceVM;

        public GlobalDeviceVM GlobalDeviceVM
        {
            get
            {
                if (_globalDeviceVM == null)
                    _globalDeviceVM = new GlobalDeviceVM(_messenger);
                return _globalDeviceVM;
            }
        }

        /// <summary>
        /// Callback method
        /// </summary>
        /// <param name="devices"></param>
        public void StatusChanged(List<GlobalDevice> devices)
        {
            GlobalDeviceVM.Update(devices);
        }
    }
}
