using System.IO;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Shared
{
    public class EmeClientConfiguration : ClientConfiguration
    {
        public bool EnableCycling = false;
        public string TestAppsCapturePath { get; set; }

        public new static EmeClientConfiguration Init(string path)
        {
            const string localhostAddress = "localhost";

            if (!File.Exists(path))
                throw new FileNotFoundException("ClientConfiguration file is missing");
            var clientConfig = XML.Deserialize<EmeClientConfiguration>(path);
            bool useLocalAddresses = ClassLocator.Default.GetInstance<IClientConfigurationManager>().UseLocalAddresses;
            if (!useLocalAddresses) return clientConfig;

            if (clientConfig.DataAccessAddress != null)
                clientConfig.DataAccessAddress.Host = localhostAddress;
            if (clientConfig.DataflowAddress != null)
                clientConfig.DataflowAddress.Host = localhostAddress;
            clientConfig.ActorAddresses.ForEach(address => address.Address.Host = localhostAddress);
            return clientConfig;
        }
    }
}
