using System.IO;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Configuration
{
    public class ClientConfigurationWaferLess
    {
        public static ClientConfigurationWaferLess Init(string path)
        {

            if (!File.Exists(path))
                throw new FileNotFoundException("ClientConfiguration Wafer Less file is missing");
            var clientConfigWl = XML.Deserialize<ClientConfigurationWaferLess>(path);

            return clientConfigWl;
        }

        public ServiceAddress AnalyseServiceAddress;
        public static ServiceAddress GetAnalyseServiceAddress()
        {

            ServiceAddress address = null;
            try
            {
                address = ClassLocator.Default.GetInstance<ClientConfigurationWaferLess>().AnalyseServiceAddress;
            }
            catch
            {
                ClassLocator.Default.GetInstance<ILogger>().Warning("Client configuration is missing");
            }
            return address;
        }
    }
}
