using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace TestRecipeRunLiveView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ClassLocator.Default.Register<RecipeRunLiveManagementViewModel>(true);
            ClassLocator.Default.Register<ServiceInvoker<IDbRecipeService>>(() => new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), ClassLocator.Default.GetInstance<IMessenger>(), new ServiceAddress() { Host = "localhost", Port = 2221 }));


            // PMs Shared supervisors
            ClassLocator.Default.Register<SharedSupervisors>(true);

            ClassLocator.Default.Register<GlobalStatusSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);


            var ExternalControls = new ExternalUserControls(new SerilogLogger<object>());
            // External user controls
            ClassLocator.Default.Register<ExternalUserControls>(() => ExternalControls);

            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            var args = Environment.GetCommandLineArgs();
            args = args.Skip(1).ToArray();
            var currentConfiguration = new ClientConfigurationManager(args);

            ClassLocator.Default.Register<IClientConfigurationManager>(() => currentConfiguration, true);

            ClassLocator.Default.Register<ClientConfiguration>(() => ClientConfiguration.Init(ClassLocator.Default.GetInstance<IClientConfigurationManager>().ClientConfigurationFilePath), true);

            ClassLocator.Default.Register<TestLiveMainViewModel>(true);

            // Dialogue service
            ClassLocator.Default.Register<IDialogOwnerService>(() => new DialogOwnerService(ClassLocator.Default.GetInstance<TestLiveMainViewModel>(), frameworkDialogFactory: new CustomFrameworkDialogFactory()), true);

            var directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var externalUserControlsPath = Path.Combine(directoryPath, @"..\..\..\..\..\ANALYSE\Client\UnitySC.PM.ANA.Client\bin\x64\Debug");
            ExternalControls.Init(externalUserControlsPath);

            ClassLocator.Default.Register<IUserSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetUserSupervisor(UnitySC.Shared.Data.Enum.ActorType.ANALYSE), true);

            //ExternalControls.Init(null);
            foreach (var pmInit in ExternalControls.PmInits)
            {
                if (pmInit.Value.ActorType == UnitySC.Shared.Data.Enum.ActorType.ANALYSE)
                {
                    pmInit.Value.BootStrap();
                }
            }


        }

    }
}
