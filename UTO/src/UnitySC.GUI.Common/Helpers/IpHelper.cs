using System.Text.RegularExpressions;

namespace UnitySC.GUI.Common.Helpers
{
    public static class IpHelper
    {
        public static string ValidateIp(string ipAddress)
        {
            string errorMessage = null;
            var patern =
                @"^(((25[0-5])|2[0-4]\d|1\d\d|[1-9]\d|\d)\.((25[0-5])|2[0-4]\d|1\d\d|[1-9]\d|\d)\.((25[0-5])|2[0-4]\d|1\d\d|[1-9]\d|\d)\.((25[0-5])|2[0-4]\d|1\d\d|[1-9]\d|\d))$";
            var rgx = new Regex(patern);
            if (!rgx.IsMatch(ipAddress))
            {
                errorMessage = $"{ipAddress} is not a valid IP address.";
            }

            return errorMessage;
        }
    }
}
