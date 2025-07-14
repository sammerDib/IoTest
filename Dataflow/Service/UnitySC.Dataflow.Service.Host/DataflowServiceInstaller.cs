using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace UnitySC.dataflow.Service.Host
{
    [RunInstaller(true)]
    public class DataflowServiceInstaller : Installer
    {
        public static readonly string ServiceName = "UnitySC.Dataflow.Service.Host";

        public DataflowServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            // Setup the Service Account type per your requirement
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            serviceInstaller.ServiceName = ServiceName;
            serviceInstaller.DisplayName = "UnitySC DataFlow Server Service";
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Description = "UnitySC running services for Dataflow server";

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
