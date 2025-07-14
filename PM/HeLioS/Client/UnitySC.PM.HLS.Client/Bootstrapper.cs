using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;

namespace UnitySC.PM.HLS.Client
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

            ClassLocator.Default.Register<IMessenger, Messenger>(true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Main view model
            ClassLocator.Default.Register<MainViewModel>(true);

            // Dialogue service
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainViewModel>(), frameworkDialogFactory: new CustomFrameworkDialogFactory()), true);

            // Used for UnitySC.PM.HLS.Client.CommonUI => TC Integration
            ClassLocator.Default.Register<SharedSupervisors>(true);

            // Used for standalone application => All modules
            ClassLocator.Default.Register<GlobalDeviceSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(UnitySC.Shared.Data.Enum.ActorType.HeLioS), true);
            ClassLocator.Default.Register<GlobalStatusSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(UnitySC.Shared.Data.Enum.ActorType.HeLioS), true);
            ClassLocator.Default.Register<IUserSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetUserSupervisor(UnitySC.Shared.Data.Enum.ActorType.HeLioS), true);
            ClassLocator.Default.Register<ChamberSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetChamberSupervisor(UnitySC.Shared.Data.Enum.ActorType.HeLioS), true);

            UnitySC.PM.Shared.UI.Main.Bootstrapper.Register();

            //CommonUI.Bootstrapper.Register(); // Todo

            //Modules.TestAlgo.Bootstrapper.Register();

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();
        }
    }
}
