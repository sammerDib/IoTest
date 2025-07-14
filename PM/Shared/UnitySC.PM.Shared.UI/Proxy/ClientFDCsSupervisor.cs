using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UI
{
    public class ClientFDCsSupervisor : IClientFDCsService
    {
        #region Fields

        private InstanceContext _instanceContext;
        private ILogger _logger;
        private ServiceInvoker<IClientFDCsService> _clientFDCsService;
        private IMessenger _messenger;
        public ActorType? Actor { get; set; }

        #endregion Fields

        #region Constructors

        public ClientFDCsSupervisor(ILogger<ClientFDCsSupervisor> logger, IMessenger messenger, ActorType? actorType)
        {
            _instanceContext = new InstanceContext(this);
            var endPoint = "ClientFDCsService";
            Actor = (ActorType)actorType;
            if (actorType != null)
                endPoint = actorType + endPoint;
            _clientFDCsService = new ServiceInvoker<IClientFDCsService>( endPoint, ClassLocator.Default.GetInstance<SerilogLogger<IClientFDCsService>>(), messenger, ClientConfiguration.GetServiceAddress(actorType));
            _logger = logger;
            _messenger = messenger;
        }
        #endregion Constructors

        public bool IsChannelOpened()
        {
            if (_clientFDCsService is null)
                return false;
            return _clientFDCsService.IsChannelOpened();
        }

        public Response<VoidResult> ClientStarted(string name)
        {
            return _clientFDCsService.TryInvokeAndGetMessages(x => x.ClientStarted(name));
        }

        public Response<VoidResult> ClientIsRunning(string name)
        {
            return _clientFDCsService.TryInvokeAndGetMessages(x => x.ClientIsRunning(name));
        }

        public Response<VoidResult> ClientStopped(string name)
        {
            return _clientFDCsService.TryInvokeAndGetMessages(x => x.ClientStopped(name));
        }

        public Response<VoidResult> ApplicationModeLocalChanged(bool isInLocalMode)
        {
            return _clientFDCsService.TryInvokeAndGetMessages(x => x.ApplicationModeLocalChanged(isInLocalMode));
        }
    }
}
