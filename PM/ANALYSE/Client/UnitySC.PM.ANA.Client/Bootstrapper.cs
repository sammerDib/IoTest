using System.IO;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures;
using UnitySC.PM.ANA.Client.Proxy.Configuration;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Proxy;
using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.PM.ANA.EP.Shared;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.ClientProxy.FDC;
using UnitySC.PM.Shared.Hardware.ClientProxy.Ffu;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.UI;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;

namespace UnitySC.PM.ANA.Client
{
    public class Bootstrapper
    {
        private const string ClientConfigurationWlFileName = "ClientConfigurationWaferLess.xml";
        
        public static void Register(string[] args = null)
        {
            // Configuration
            var currentConfiguration = new ClientConfigurationManager(args);
            var externalProcessingConfiguration = ExternalProcessingConfiguration.Init(currentConfiguration.ConfigurationFolderPath);

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
            ClassLocator.Default.Register<IClientConfigurationManager>(() => currentConfiguration, true);

            ClassLocator.Default.Register<ClientConfiguration>(() => ClientConfiguration.Init(ClassLocator.Default.GetInstance<IClientConfigurationManager>().ClientConfigurationFilePath), true);

            
            ClassLocator.Default.Register<ClientConfigurationWaferLess>(() => ClientConfigurationWaferLess.Init(Path.Combine(ClassLocator.Default.GetInstance<IClientConfigurationManager>().ConfigurationFolderPath, ClientConfigurationWlFileName)), true);


            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Main view model
            ClassLocator.Default.Register<MainViewModel>(true);

            // PMViewModel
            ClassLocator.Default.Register<PMViewModel>(() => ClassLocator.Default.GetInstance<MainViewModel>().PMViewModel, true);

            // Dialogue service
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainViewModel>(), frameworkDialogFactory: new CustomFrameworkDialogFactory()), true);

            // External processing
            // Only for test don't use directerly MountainsSupervisor
            if (externalProcessingConfiguration.Mountains != null)
            {
                if (externalProcessingConfiguration.Mountains.IsHostedByPM)
                {
                    ClassLocator.Default.Register<MountainsConfiguration>(() => externalProcessingConfiguration.Mountains, true);
                    ClassLocator.Default.Register<MountainsActiveXSupervisor>(true);
                }
                ClassLocator.Default.Register(typeof(IMountainsGatewayService), typeof(MountainsGatewayService), true);
                ClassLocator.Default.Register<MountainsSupervisor>(() => new MountainsSupervisor(externalProcessingConfiguration.Mountains.Address), true);
            }

            // Used for UnitySC.PM.ANA.Client.CommonUI => TC Integration
            ClassLocator.Default.Register<SharedSupervisors>(true);

            // Used for standalone application => All modules
            ClassLocator.Default.Register<GlobalDeviceSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            ClassLocator.Default.Register<GlobalStatusSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            ClassLocator.Default.Register<IUserSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetUserSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            ClassLocator.Default.Register<ChamberSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetChamberSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            ClassLocator.Default.Register<FDCSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetFDCSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            ClassLocator.Default.Register<ClientFDCsSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetClientFDCsSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            ClassLocator.Default.Register<DBMaintenanceSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetDBMaintenanceSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);

            ClassLocator.Default.Register(() => new PlcSupervisor(ClassLocator.Default.GetInstance<ILogger<PlcSupervisor>>(),
            ClassLocator.Default.GetInstance<ILogger<IPlcService>>(), ClassLocator.Default.GetInstance<IMessenger>(), UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);
            ClassLocator.Default.Register(() => new FfuSupervisor(ClassLocator.Default.GetInstance<ILogger<FfuSupervisor>>(),
            ClassLocator.Default.GetInstance<ILogger<IFfuService>>(), ClassLocator.Default.GetInstance<IMessenger>(), UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);

            //Controller supervisor
            ClassLocator.Default.Register(() => new ControllersSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE,
                                                ClassLocator.Default.GetInstance<ILogger<ControllersSupervisor>>(),
                                                ClassLocator.Default.GetInstance<IMessenger>(),
                                                ClassLocator.Default.GetInstance<IDialogOwnerService>()), true);

            ClassLocator.Default.Register<PointsPresetsManager>(true);

            UnitySC.PM.Shared.UI.Main.Bootstrapper.Register();

            CommonUI.Bootstrapper.Register();

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();
        }
    }
}
