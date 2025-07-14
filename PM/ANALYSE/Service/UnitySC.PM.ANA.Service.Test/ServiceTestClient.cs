using UnitySC.Shared.Logger;
using System.ServiceModel;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Test
{
    public class ServiceTestClient<T>
    {
        protected readonly DuplexServiceInvoker<T> ServiceInvoker;
        private ServiceHost _host;
        private readonly InstanceContext _instanceContext;
        private readonly ILogger<T> _logger;

        public ServiceTestClient(string serviceName, InstanceContext instanceContext)
        {
            _instanceContext = instanceContext;
            _logger = new SerilogLogger<T>();
            var messenger = new GalaSoft.MvvmLight.Messaging.Messenger();
            ServiceInvoker = new DuplexServiceInvoker<T>(_instanceContext, serviceName, _logger, messenger);
        }

        public virtual void SetUpService(T service)
        {
            _host = new ServiceHost(service);
            foreach (var endpoint in _host.Description.Endpoints)
                _logger.Information($"Creating {_host.Description.Name} service on {endpoint.Address}");
            _host.Open();
            _logger.Information($"Started {_host.Description.Name} service");
        }

        public virtual void TearDownService()
        {
            _host.Close();
            _logger.Information($"Stopped {_host.Description.Name} service");
        }
    }
}
