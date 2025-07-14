using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace UnitySC.DataAccess.Service.Host
{
    [RunInstaller(true)]
    public  class DataAccessServiceInstaller : Installer
    {
        public static readonly string ServiceName = "UnitySC.DataAccess.Service.Host";

        public DataAccessServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Setup the Service Account type per your requirement
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.ServiceName = ServiceName;
            serviceInstaller.DisplayName = "UnitySC DataAccess Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Description = "UnitySC running services for Database Access (User,  Tool, Recipes, Results ...)";

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }

    }
}
