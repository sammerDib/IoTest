using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace UnitySC.PM.ANA.Service.Host
{
    [RunInstaller(true)]
    public class AnaServiceInstaller : Installer
    {
        public static readonly string ServiceName = "UnitySC.PM.ANA.Service.Host";

        public AnaServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Setup the Service Account type per your requirement
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.ServiceName = ServiceName;
            serviceInstaller.DisplayName = "UnitySC Analyse Server Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Description = "UnitySC running services for Analyse sever";

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }

    }
}
