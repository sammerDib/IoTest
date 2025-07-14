using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.PM.DMT.Service.Interface.Chamber;
using UnitySC.PM.DMT.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;

using ChuckSupervisor = UnitySC.PM.DMT.Client.Proxy.Chuck.ChuckSupervisor;
using ChamberSupervisor = UnitySC.PM.DMT.Client.Proxy.Chamber.ChamberSupervisor;
using PlcSupervisor = UnitySC.PM.Shared.Hardware.ClientProxy.Plc.PlcSupervisor;
using FfuSupervisor = UnitySC.PM.Shared.Hardware.ClientProxy.Ffu.FfuSupervisor;

namespace UnitySC.PM.DMT.Client
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

            ClassLocator.Default.Register<ClientConfiguration>(() => ClientConfiguration.Init(currentConfiguration.ClientConfigurationFilePath), true);
            
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register<ILogger, SerilogLogger<object>>();

            //Service used to display dialog
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<MainViewModel>(), frameworkDialogFactory: new CustomFrameworkDialogFactory()), true);

            // Main view model
            ClassLocator.Default.Register<MainViewModel>(true);

            // Used for UnitySC.PM.DMT.CommonUI => TC Integration
            ClassLocator.Default.Register<SharedSupervisors>(true);

            // Used for standalone application => All modules
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(ActorType.DEMETER), true);
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.DEMETER), true);
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetUserSupervisor(ActorType.DEMETER), true);
            //ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetChamberSupervisor(ActorType.DEMETER), true);
            ClassLocator.Default.Register(() => new ChuckSupervisor(ClassLocator.Default.GetInstance<ILogger<ChuckSupervisor>>(), 
            ClassLocator.Default.GetInstance<ILogger<IDMTChuckService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ActorType.DEMETER), true);
            ClassLocator.Default.Register(() => new ChamberSupervisor(ClassLocator.Default.GetInstance<ILogger<ChamberSupervisor>>(),
            ClassLocator.Default.GetInstance<ILogger<IDMTChamberService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ActorType.DEMETER), true);
            ClassLocator.Default.Register(() => new PlcSupervisor(ClassLocator.Default.GetInstance<ILogger<PlcSupervisor>>(),
            ClassLocator.Default.GetInstance<ILogger<IPlcService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ActorType.DEMETER), true);
            ClassLocator.Default.Register(() => new FfuSupervisor(ClassLocator.Default.GetInstance<ILogger<FfuSupervisor>>(),
            ClassLocator.Default.GetInstance<ILogger<IFfuService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ActorType.DEMETER), true);
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetFDCSupervisor(ActorType.DEMETER), true);

            UnitySC.PM.Shared.UI.Main.Bootstrapper.Register();

            DMT.CommonUI.Bootstrapper.Register();

            ClassLocator.Default.GetInstance<ExceptionManager>().Init();
        }
    }
}
