using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

using UnitySC.Shared.Logger;

namespace UnitySC.Shared.Tools.Service
{
    public abstract class BaseServer
    {
        public Dictionary<BaseService, ServiceHost> Hosts { get; private set; }
        public ILogger Logger { get; private set; }

        public BaseServer(ILogger logger)
        {
            Logger = logger;
            Hosts = new Dictionary<BaseService, ServiceHost>();
        }

        public abstract void Start();

        public abstract void Stop();


        public void StartService(BaseService service)
        {
            StartService(service, null);
        }

        public void StartService(BaseService service, ServiceAddress hostAddress)
        {
            service.Init();

            ServiceHost host= new ServiceHost(service);

            if (!(hostAddress is null))
            {
                var uri = host.Description.Endpoints[0].Address.Uri;
                var newUri = new UriBuilder(uri);
                newUri.Host = hostAddress.Host;
                newUri.Port = hostAddress.Port;
                //host.BaseAddresse = newUri.Uri;
                host = new ServiceHost(service, newUri.Uri);

                foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
                {
                    host.SetEndpointAddress(endpoint, "");
                }
            }

            foreach (var endpoint in host.Description.Endpoints)
                Logger.Information($"Creating {host.Description.Name} service on {endpoint.Address}");

            host.Open();
            Hosts.Add(service, host);
        }

     

        public void StopService(BaseService service, ServiceHost host)
        {
            Logger.Information($"Stop {host.Description.Name} service..");
            if (Hosts.ContainsKey(service))
                Hosts.Remove(service);
            host.Abort();
            // TODO We should use Close(), but we need to close communications channels on client side or we will wait aroune 90sec before
            // Analyse Server close. (10 secondes per service)
            service.Shutdown();
        }

        public void StopAllServiceHost()
        {
            var hoststcopy = new Dictionary<BaseService, ServiceHost>(Hosts);
            foreach (var kvp in hoststcopy)
            {
                StopService(service: kvp.Key, host: kvp.Value);
            }
        }
    }
}
