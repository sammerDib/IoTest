using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Host
{
    [RunInstaller(true)]
    public class MountainsServiceInstaller : Installer
    {
        public static readonly string ServiceName = "UnitySC.EP.Mountains.Service.Host";

        public MountainsServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Setup the Service Account type per your requirement
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.ServiceName = ServiceName;
            serviceInstaller.DisplayName = "UnitySC EP Mountains Server Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Description = "UnitySC service for External Mountains processing";

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
