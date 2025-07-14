using System.ServiceModel;

using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ClientFDCsService : BaseService, IClientFDCsService
    {
        #region Fields

        private ClientFDCs _clientFDCs;

        #endregion Fields

        #region Constructors

        public ClientFDCsService(ILogger logger) : base(logger, ExceptionType.ToolSetupException)
        {
            _clientFDCs = ClassLocator.Default.GetInstance<ClientFDCs>();
        }

        #endregion Constructors

        public Response<VoidResult> ClientStarted(string name)
        {
            return InvokeVoidResponse(_ => _clientFDCs.ClientStarted(name));
        }

        public Response<VoidResult> ClientIsRunning(string name)
        {
            return InvokeVoidResponse(_ => _clientFDCs.ClientIsRunning(name));
        }

        public Response<VoidResult> ClientStopped(string name)
        {
            return InvokeVoidResponse(_ => _clientFDCs.ClientStopped(name));
        }

        public Response<VoidResult> ApplicationModeLocalChanged(bool isInLocalMode)
        {
            return InvokeVoidResponse(_ => _clientFDCs.ApplicationModeLocalChanged(isInLocalMode));
        }
    }
}
