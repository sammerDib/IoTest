using UnitySC.PP.ADC.Client.Proxy.Recipe;
using UnitySC.Shared.Tools;

namespace UnitySC.PP.ADC.Client.Proxy
{
    public class ServiceLocator
    {

        public static ADCRecipeSupervisor ADCRecipeSupervisor => ClassLocator.Default.GetInstance<ADCRecipeSupervisor>();

    }
}

