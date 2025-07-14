
using System.Globalization;

using UnitySC.PP.ADC.Client.Proxy.Recipe;
using UnitySC.Shared.Tools;

namespace UnitySC.PP.ADC.Client.CommonUI
{
    public class Bootstrapper
    {
        public static void Register()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            ClassLocator.Default.Register<ADCRecipeSupervisor>(true);

        }
    }
}
