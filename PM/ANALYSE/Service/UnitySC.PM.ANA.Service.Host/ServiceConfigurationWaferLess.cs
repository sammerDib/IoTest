using System.IO;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Host
{
    public class ServiceConfigurationWaferLess
    {
        public static ServiceConfigurationWaferLess Init(string path)
        {

            if (!File.Exists(path))
                throw new FileNotFoundException("ServiceConfiguration Wafer Less file is missing");
            var serviceConfigWl = XML.Deserialize<ServiceConfigurationWaferLess>(path);

            return serviceConfigWl;
        }

        public ServiceAddress AnalyseServiceAddress;

        
    }
}
