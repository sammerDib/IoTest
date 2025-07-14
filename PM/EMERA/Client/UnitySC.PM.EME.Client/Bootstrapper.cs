using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.EME.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.EME.Client.Proxy.Dispatcher;
using UnitySC.PM.EME.Client.Shared;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ExceptionDialogs;
using UnitySC.Shared.UI.ViewModel;

using PlcSupervisor = UnitySC.PM.Shared.Hardware.ClientProxy.Plc.PlcSupervisor;
using DistanceSensorSupervisor = UnitySC.PM.Shared.Hardware.ClientProxy.DistanceSensor.DistanceSensorSupervisor;

namespace UnitySC.PM.EME.Client
{
    public static class Bootstrapper
    {
        public static void Register(string[] args)
        {
            var configuration = new ClientConfigurationManager(args);
            ClassLocator.Default.Register<IClientConfigurationManager>(() => configuration, true);
            ClassLocator.Default.Register(() => configuration, true);

            ClassLocator.Default.Register<ClientConfiguration>(
                () => EmeClientConfiguration.Init(configuration.ClientConfigurationFilePath), true);
            ClassLocator.Default.Register(() => EmeClientConfiguration.Init(configuration.ClientConfigurationFilePath),
                true);

            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);

            RegisterLogging(configuration);
            RegisterViewModels();
            RegisterSharedSupervisors();
            RegisterDispatcher();

            Proxy.Bootstrapper.Register();

            PM.Shared.UI.Main.Bootstrapper.Register();

            Recipe.Bootstrapper.Register();

            RegisterDataAccessServices();
            
            if (Application.Current != null)
            {
                ClassLocator.Default.GetInstance<ExceptionManager>().Init();
            }
        }

        private static void RegisterDispatcher()
        {
            ClassLocator.Default.Register<IDispatcher, Dispatcher>();
        }

        private static void RegisterDataAccessServices()
        {
            ClassLocator.Default.Register(() => new ServiceInvoker<IToolService>("ToolService", ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress()));
        }

        private static void RegisterViewModels()
        {
            // Main view model
            ClassLocator.Default.Register<MainViewModel>(true);
            // PMViewModel
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<MainViewModel>().PMViewModel);
            // Navigation
            ClassLocator.Default.Register<INavigationManager, NavigationManagerForCalibration>(true);

            ClassLocator.Default.Register<IDialogOwnerService>(
                () => new DialogOwnerService(ClassLocator.Default.GetInstance<MainViewModel>(),
                    frameworkDialogFactory: new CustomFrameworkDialogFactory()), true);
            ClassLocator.Default.Register<NotifierVM>(true);

        }

        private static void RegisterSharedSupervisors()
        {
            ClassLocator.Default.Register<SharedSupervisors>(true);
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(ActorType.EMERA), true);
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.EMERA), true);
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetUserSupervisor(ActorType.EMERA), true);
            ClassLocator.Default.Register(
                () => ClassLocator.Default.GetInstance<SharedSupervisors>().GetFDCSupervisor(ActorType.EMERA), true);


            ClassLocator.Default.Register(() => new PlcSupervisor(ClassLocator.Default.GetInstance<ILogger<PlcSupervisor>>(),
            ClassLocator.Default.GetInstance<ILogger<IPlcService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ActorType.EMERA), true);

            ClassLocator.Default.Register(() => new DistanceSensorSupervisor(ClassLocator.Default.GetInstance<ILogger<DistanceSensorSupervisor>>(),
            ClassLocator.Default.GetInstance<ILogger<IDistanceSensorService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ActorType.EMERA), true);
        }

        private static void RegisterLogging(ClientConfigurationManager configuration)
        {
            SerilogInit.Init(configuration.LogConfigurationFilePath);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
        }
    }
}
