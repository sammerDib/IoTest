using UnitySC.PM.ANA.Client.Proxy.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Helpers
{
    public static class ServiceAddressHelper
    {
        public static ServiceAddress GetAnalyseServiceAddress1()
        {
            var isWaferLessMode = ClassLocator.Default.GetInstance<IClientConfigurationManager>().IsWaferLessMode;

            if (isWaferLessMode)
            {
                return ClientConfigurationWaferLess.GetAnalyseServiceAddress();
            }
            return ClientConfiguration.GetServiceAddress(ActorType.ANALYSE);
        }
    }
}
