using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ExceptionDialogs;

namespace UnitySC.Dataflow.Client
{
    public class Bootstrapper
    {
        public static void Register(string[] args = null)
        {
            // Configuration
            var currentConfiguration = new ClientConfigurationManager(args);

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
            ClassLocator.Default.Register<IClientConfigurationManager>(() => currentConfiguration, true);

            ClassLocator.Default.Register<ClientConfiguration>(() => ClientConfiguration.Init(ClassLocator.Default.GetInstance<IClientConfigurationManager>().ClientConfigurationFilePath), true);


            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            // Used for UnitySC.PM.ANA.Client.CommonUI => TC Integration
            ClassLocator.Default.Register<SharedSupervisors>(true);

            // Used for standalone application => All modules
            //ClassLocator.Default.Register<GlobalDeviceSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(UnitySC.Shared.Data.Enum.ActorType.DataflowManager), true);
            ClassLocator.Default.Register<GlobalStatusSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);

            ClassLocator.Default.Register<IUserSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetUserSupervisor(UnitySC.Shared.Data.Enum.ActorType.DataflowManager), true);
            //ClassLocator.Default.Register<ChamberSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetChamberSupervisor(UnitySC.Shared.Data.Enum.ActorType.DataflowManager), true);

            UnitySC.PM.Shared.UI.Main.Bootstrapper.Register();
            ClassLocator.Default.GetInstance<ExceptionManager>().Init();

        }

    }
}
