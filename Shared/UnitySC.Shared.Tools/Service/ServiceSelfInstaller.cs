using System.Configuration.Install;

namespace UnitySC.Shared.Tools.Service
{
    public class ServiceSelfInstaller
    {
        public static bool InstallMe(string exePath)
        {
            bool result;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] {
                    exePath
                });
            }
            catch
            {
                result = false;
                return result;
            }
            result = true;
            return result;
        }

        public static bool UninstallMe(string exePath)
        {
            bool result;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] {
                    "/u",  exePath
                });
            }
            catch
            {
                result = false;
                return result;
            }
            result = true;
            return result;
        }

    }
}
