using System.Globalization;

namespace UnitySC.PM.LIGHTSPEED.Client.CommonUI
{
    public class Bootstrapper
    {
        public static void Register()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }
    }
}
