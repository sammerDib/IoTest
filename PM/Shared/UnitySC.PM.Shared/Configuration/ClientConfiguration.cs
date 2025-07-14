using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Configuration
{
    public class ClientConfiguration
    {
        public static ClientConfiguration Init(string path)
        {
            const string localhostAdress = "localhost";

            if (!File.Exists(path))
                throw new FileNotFoundException("ClientConfiguration file is missing");
            var clientConfig = XML.Deserialize<ClientConfiguration>(path);
 
            if (ClassLocator.Default.IsRegistered<IClientConfigurationManager>())
            {
                var useLocalAddresses = ClassLocator.Default.GetInstance<IClientConfigurationManager>().UseLocalAddresses;
                if (useLocalAddresses)
                {
                    if (clientConfig.DataAccessAddress != null)
                        clientConfig.DataAccessAddress.Host = localhostAdress;
                    if (clientConfig.DataflowAddress!=null)
                        clientConfig.DataflowAddress.Host = localhostAdress;
                    foreach (var actorAddress in clientConfig.ActorAddresses)
                    {
                        actorAddress.Address.Host = localhostAdress;
                    }
                }
            }
            return clientConfig;
        }

        public ServiceAddress DataAccessAddress;
        public ServiceAddress DataflowAddress;
        public List<ActorAddress> ActorAddresses;

        public static ServiceAddress GetServiceAddress(ActorType? actorType)
        {
            ServiceAddress address = null;
            if (actorType.HasValue)
            {
                if (actorType == ActorType.DataAccess)
                    return GetDataAccessAddress();
                if (actorType == ActorType.DataflowManager)
                    return GetDataflowAddress();
                try
                {
                    address = ClassLocator.Default.GetInstance<ClientConfiguration>().ActorAddresses.SingleOrDefault(x => x.Actor == actorType)?.Address;
                }
                catch
                {
                    ClassLocator.Default.GetInstance<ILogger>().Warning("Client configuration is missing");
                }
            }
            return address;
        }

        public static ServiceAddress GetDataAccessAddress()
        {
            ServiceAddress address = null;
            try
            {
                address = ClassLocator.Default.GetInstance<ClientConfiguration>().DataAccessAddress;
            }
            catch
            {
                ClassLocator.Default.GetInstance<ILogger>().Warning("Client configuration is missing");
            }
            return address;
        }

        public static ServiceAddress GetDataflowAddress()
        {
            ServiceAddress address = null;
            try
            {
                address = ClassLocator.Default.GetInstance<ClientConfiguration>().DataflowAddress;
            }
            catch
            {
                ClassLocator.Default.GetInstance<ILogger>().Warning("Client configuration is missing");
            }
            return address;
        }
    }

    public class ActorAddress
    {
        public ServiceAddress Address;

        [XmlAttribute("Actor")]
        public ActorType Actor;
    }
}
